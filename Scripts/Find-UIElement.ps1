<#
.SYNOPSIS
    Finds a UI Automation element by Name/AutomationId/ControlType and either draws a temporary
    highlight box around it on screen, or invokes it directly - the reverse direction of a
    hover-based inspector like Accessibility Insights: instead of "hover an element, see its
    properties", this is "specify properties, see (or trigger) which element matches".

.PARAMETER Root
    Element to search within. Defaults to the whole desktop. Pass a specific window's element
    (e.g. via FromHandle on a Process's MainWindowHandle) to scope the search to one app.

.PARAMETER Name
.PARAMETER AutomationId
.PARAMETER ControlType
    Any combination of these three is combined with AND. At least one is required.
    NOTE: confirmed via Accessibility Insights that this app does not populate AutomationId on any
    control checked so far (menu items, buttons, the grid's edit control) - use Name + ControlType
    for anything in this particular app; -AutomationId is kept here for general-purpose use only.

.PARAMETER HighlightSeconds
    How long the highlight box stays visible. Default 3. Ignored if -Invoke is used.

.PARAMETER Invoke
    Instead of drawing a highlight box, call InvokePattern.Invoke() on the matched element directly -
    e.g. to confirm a found button actually triggers the expected action, independent of whether the
    (separate) highlight-drawing code is positioning its box correctly. Fails clearly if the element
    doesn't support InvokePattern (e.g. an Edit control, which uses ValuePattern instead).

.EXAMPLE
    .\Find-UIElement.ps1 -Name "Save changes" -ControlType "button"

.EXAMPLE
    $proc = Get-Process ScientificInterface
    $window = [System.Windows.Automation.AutomationElement]::FromHandle($proc.MainWindowHandle)
    .\Find-UIElement.ps1 -Root $window -Name "Save changes" -ControlType "button" -Invoke
#>
param(
    [System.Windows.Automation.AutomationElement]$Root,
    [string]$Name,
    [string]$AutomationId,
    [string]$ControlType,
    [int]$HighlightSeconds = 3,
    [switch]$Invoke
)

Add-Type -AssemblyName UIAutomationClient, UIAutomationTypes, System.Windows.Forms, System.Drawing

# UI Automation's BoundingRectangle always reports true physical screen pixels. A plain
# PowerShell-hosted WinForms window is NOT DPI-aware by default, so without this, Windows silently
# rescales where it actually draws the window on any display running other than 100% scaling - the
# highlight box ends up in the wrong place even though the coordinates given to it were correct.
# Irrelevant to -Invoke (no window is drawn in that path) but harmless to set regardless.
try {
    [System.Windows.Forms.Application]::SetHighDpiMode([System.Windows.Forms.HighDpiMode]::PerMonitorV2) | Out-Null
} catch {
    # Older .NET/PowerShell combinations may not expose SetHighDpiMode at all - if so, the highlight
    # box may still be mispositioned on a scaled display; nothing further to do about it here.
}

if ($null -eq $Root) {
    $Root = [System.Windows.Automation.AutomationElement]::RootElement
}

$conditions = New-Object System.Collections.Generic.List[System.Windows.Automation.Condition]
if ($Name) {
    $conditions.Add((New-Object System.Windows.Automation.PropertyCondition(
        [System.Windows.Automation.AutomationElement]::NameProperty, $Name)))
}
if ($AutomationId) {
    $conditions.Add((New-Object System.Windows.Automation.PropertyCondition(
        [System.Windows.Automation.AutomationElement]::AutomationIdProperty, $AutomationId)))
}
if ($ControlType) {
    # ControlTypeProperty (a stable ControlType object), not LocalizedControlTypeProperty (a
    # localized, possibly inconsistently-cased display string) - case-insensitive lookup by name
    # against the known static ControlType members (Edit, Button, MenuItem, Pane, Window, etc.).
    $controlTypeField = [System.Windows.Automation.ControlType].GetField(
        $ControlType, [System.Reflection.BindingFlags]::Public -bor [System.Reflection.BindingFlags]::Static -bor [System.Reflection.BindingFlags]::IgnoreCase)
    if ($null -eq $controlTypeField) {
        throw "Unknown -ControlType '$ControlType' - expected a System.Windows.Automation.ControlType member name (Edit, Button, MenuItem, Pane, Window, etc.)."
    }
    $resolvedControlType = $controlTypeField.GetValue($null)
    $conditions.Add((New-Object System.Windows.Automation.PropertyCondition(
        [System.Windows.Automation.AutomationElement]::ControlTypeProperty, $resolvedControlType)))
}

if ($conditions.Count -eq 0) {
    throw "Specify at least one of -Name, -AutomationId, -ControlType."
}

$condition = if ($conditions.Count -eq 1) {
    $conditions[0]
} else {
    New-Object System.Windows.Automation.AndCondition($conditions.ToArray())
}

$element = $Root.FindFirst([System.Windows.Automation.TreeScope]::Descendants, $condition)

if ($null -eq $element) {
    Write-Host "No matching element found under the given root." -ForegroundColor Red
    return
}

Write-Host "Found: '$($element.Current.Name)' [$($element.Current.ControlType.ProgrammaticName)]"
Write-Host "  AutomationId: '$($element.Current.AutomationId)'"
Write-Host "  BoundingRectangle: $($element.Current.BoundingRectangle)"

if ($Invoke) {
    if (-not $element.GetSupportedPatterns().Contains([System.Windows.Automation.InvokePattern]::Pattern)) {
        Write-Host "This element does not support InvokePattern - it can't be clicked this way (an Edit control, for example, uses ValuePattern instead)." -ForegroundColor Red
        return
    }
    $invokePattern = $element.GetCurrentPattern([System.Windows.Automation.InvokePattern]::Pattern)
    $invokePattern.Invoke()
    Write-Host "Invoked." -ForegroundColor Green
    return
}

$rect = $element.Current.BoundingRectangle
if ($rect.IsEmpty -or $rect.Width -le 0 -or $rect.Height -le 0) {
    Write-Host "Element has no visible on-screen rectangle (not currently rendered) - nothing to highlight." -ForegroundColor Yellow
    return
}

# Click-through would need an extended window style (WS_EX_TRANSPARENT) via P/Invoke - not worth
# the extra complexity here, since this box is only ever on screen briefly and the script isn't
# clicking through it anyway.
$highlight = New-Object System.Windows.Forms.Form
$highlight.FormBorderStyle = 'None'
$highlight.StartPosition = 'Manual'
$highlight.ShowInTaskbar = $false
$highlight.TopMost = $true
$highlight.BackColor = [System.Drawing.Color]::Lime
$highlight.Opacity = 0.35
$highlight.Location = New-Object System.Drawing.Point([int]$rect.X, [int]$rect.Y)
$highlight.Size = New-Object System.Drawing.Size([int]$rect.Width, [int]$rect.Height)
$highlight.Show()

Start-Sleep -Seconds $HighlightSeconds
$highlight.Close()

# Returned (not just highlighted) so it can be captured and chained as the next call's -Root -
# e.g. $dialog = .\Find-UIElement.ps1 -Root $window -Name "Save As" -ControlType "Window"
return $element
