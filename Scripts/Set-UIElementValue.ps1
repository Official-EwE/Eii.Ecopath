<#
.SYNOPSIS
    Finds a UI Automation element by Name/AutomationId/ControlType and sets its value via
    ValuePattern - more robust than SendKeys for text input, since it sets the value as one atomic
    UI Automation action rather than simulating individual keystrokes (no focus-timing issues, no
    risk of a stray keystroke landing in the wrong place).

.PARAMETER Root
    Element to search within. Defaults to the whole desktop. Pass a specific window's element
    (e.g. via FromHandle on a Process's MainWindowHandle) to scope the search to one app.

.PARAMETER Name
.PARAMETER AutomationId
.PARAMETER ControlType
    Any combination of these three is combined with AND. At least one is required.

.PARAMETER Value
    The text to set.

.PARAMETER PressEnterToCommit
    After setting the value, send Enter via SendKeys - needed here because setting a grid cell's
    ValuePattern alone does not commit it; this app's grid only commits (and enables Save) once
    Enter is actually pressed, per manual observation.

.EXAMPLE
    # First: double-click (or F2) a grid cell in the app to put it into edit mode.
    $proc = Get-Process ScientificInterface
    $window = [System.Windows.Automation.AutomationElement]::FromHandle($proc.MainWindowHandle)
    .\Set-UIElementValue.ps1 -Root $window -ControlType "edit" -Value "1.5" -PressEnterToCommit
#>
param(
    [System.Windows.Automation.AutomationElement]$Root,
    [string]$Name,
    [string]$AutomationId,
    [string]$ControlType,
    [Parameter(Mandatory = $true)]
    [string]$Value,
    [switch]$PressEnterToCommit
)

Add-Type -AssemblyName UIAutomationClient, UIAutomationTypes, System.Windows.Forms

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
    # ControlTypeProperty, not LocalizedControlTypeProperty - see Find-UIElement.ps1's own comment
    # on this same lookup for why.
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

if (-not $element.GetSupportedPatterns().Contains([System.Windows.Automation.ValuePattern]::Pattern)) {
    Write-Host "This element does not support ValuePattern - it can't be set this way." -ForegroundColor Red
    return
}

# SetFocus first: SendKeys below (if -PressEnterToCommit) sends to whatever control currently has
# keyboard focus, which may not be this element unless explicitly focused first.
$element.SetFocus()

$valuePattern = $element.GetCurrentPattern([System.Windows.Automation.ValuePattern]::Pattern)
$valuePattern.SetValue($Value)
Write-Host "Value set to '$Value'." -ForegroundColor Green

if ($PressEnterToCommit) {
    Start-Sleep -Milliseconds 150  # let focus actually settle before sending the key
    [System.Windows.Forms.SendKeys]::SendWait("{ENTER}")
    Write-Host "Enter sent to commit." -ForegroundColor Green
}
