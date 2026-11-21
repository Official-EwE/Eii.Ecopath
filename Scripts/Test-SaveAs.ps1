<#
.SYNOPSIS
    Opens the File menu and clicks "Save Model As...". Matches by Name + ControlType, not
    AutomationId - confirmed via Accessibility Insights that this app does not populate
    AutomationId on ANY control checked so far (menu items, the Save button, the grid's edit
    control) - a standing rule for this whole toolkit, not just this one script.

.PARAMETER Proc
    The app's Process object.

.PARAMETER TargetPath
    If given, a full absolute path (e.g. "C:\Temp\MyModel.eweaccdb") typed directly into the dialog's
    "File name:" field, followed by clicking Save - a standard Windows common dialog accepts a full
    path there directly, navigating and saving in one step with no need to touch the address bar's
    breadcrumb toolbar at all (which would need switching into edit mode first - the same kind of
    only-exists-in-a-certain-interaction-mode problem the grid gave us, avoided entirely this way).
    If omitted, the dialog is opened and left open for manual inspection/cancellation, same as before.
#>
param(
    [Parameter(Mandatory = $true)]
    [System.Diagnostics.Process]$Proc,
    [string]$TargetPath
)

Add-Type -AssemblyName UIAutomationClient, UIAutomationTypes

if (-not ("Win32FgSaveAs" -as [type])) {
    Add-Type @"
using System;
using System.Runtime.InteropServices;
public class Win32FgSaveAs {
    [DllImport("user32.dll")]
    public static extern bool SetForegroundWindow(IntPtr hWnd);
    [DllImport("user32.dll")]
    public static extern IntPtr GetForegroundWindow();
    [DllImport("user32.dll")]
    public static extern void mouse_event(uint dwFlags, int dx, int dy, uint dwData, UIntPtr dwExtraInfo);
    public const uint MOUSEEVENTF_LEFTDOWN = 0x02;
    public const uint MOUSEEVENTF_LEFTUP = 0x04;
}
"@
}

Write-Host "Bringing app window to foreground..."
$sfwResult = [Win32FgSaveAs]::SetForegroundWindow($Proc.MainWindowHandle)
Start-Sleep -Milliseconds 300
if ([Win32FgSaveAs]::GetForegroundWindow() -ne $Proc.MainWindowHandle) {
    Write-Host "WARNING: app is not actually foreground (SetForegroundWindow returned $sfwResult) - click into it manually once if this persists." -ForegroundColor Red
}

$window = [System.Windows.Automation.AutomationElement]::FromHandle($Proc.MainWindowHandle)

Write-Host "Looking for the File menu item (Name='File', ControlType=MenuItem)..."
$fileMenu = $window.FindFirst(
    [System.Windows.Automation.TreeScope]::Descendants,
    (New-Object System.Windows.Automation.AndCondition(
        (New-Object System.Windows.Automation.PropertyCondition(
            [System.Windows.Automation.AutomationElement]::NameProperty, "File")),
        (New-Object System.Windows.Automation.PropertyCondition(
            [System.Windows.Automation.AutomationElement]::ControlTypeProperty, [System.Windows.Automation.ControlType]::MenuItem))
    ))
)
if ($null -eq $fileMenu) {
    Write-Host "Not found." -ForegroundColor Red
    return
}
Write-Host "  Found: Name='$($fileMenu.Current.Name)', ControlType=$($fileMenu.Current.ControlType.ProgrammaticName)"

Write-Host "Opening it..."
$patterns = $fileMenu.GetSupportedPatterns()
if ($patterns.Contains([System.Windows.Automation.ExpandCollapsePattern]::Pattern)) {
    $fileMenu.GetCurrentPattern([System.Windows.Automation.ExpandCollapsePattern]::Pattern).Expand()
} elseif ($patterns.Contains([System.Windows.Automation.InvokePattern]::Pattern)) {
    $fileMenu.GetCurrentPattern([System.Windows.Automation.InvokePattern]::Pattern).Invoke()
} else {
    Write-Host "  File menu supports neither ExpandCollapsePattern nor InvokePattern - can't open it this way." -ForegroundColor Red
    return
}
Start-Sleep -Milliseconds 400

Write-Host "Looking for 'Save Model As...' (Name match, ControlType=MenuItem) - confirmed via" `
    "Accessibility Insights to nest directly under the File menu item once expanded..."
$saveAsItem = $window.FindFirst(
    [System.Windows.Automation.TreeScope]::Descendants,
    (New-Object System.Windows.Automation.AndCondition(
        (New-Object System.Windows.Automation.PropertyCondition(
            [System.Windows.Automation.AutomationElement]::NameProperty, "Save Model As...")),
        (New-Object System.Windows.Automation.PropertyCondition(
            [System.Windows.Automation.AutomationElement]::ControlTypeProperty, [System.Windows.Automation.ControlType]::MenuItem))
    ))
)
if ($null -eq $saveAsItem) {
    Write-Host "Not found." -ForegroundColor Red
    return
}
Write-Host "  Found: Name='$($saveAsItem.Current.Name)'"

# A real Win32 mouse click, not InvokePattern.Invoke() - the same approach already proven reliable
# for the grid cell. InvokePattern is a COM call into the target app, and this item's click handler
# opens a MODAL dialog that blocks until closed - meaning Invoke() itself would hang for as long as
# that dialog stays open (confirmed: it did). Moving that call to a background thread to avoid the
# wait was the wrong fix and crashed the whole PowerShell host - UI Automation's COM objects are
# STA-affinitized, bound to the thread that created them; calling into them from any other thread
# (MTA or a different STA) is a cross-apartment violation, which can surface as a native crash
# rather than a catchable .NET exception. A raw mouse click sidesteps all of this: it is not a COM
# call at all, just an OS-level event posted into the window's queue, returning immediately - the
# same non-blocking behavior a real human click has.
Add-Type -AssemblyName System.Windows.Forms, System.Drawing
$itemRect = $saveAsItem.Current.BoundingRectangle
$centerX = [int]($itemRect.X + $itemRect.Width / 2)
$centerY = [int]($itemRect.Y + $itemRect.Height / 2)
[System.Windows.Forms.Cursor]::Position = New-Object System.Drawing.Point($centerX, $centerY)
Start-Sleep -Milliseconds 100

Write-Host "Clicking it..."
[Win32FgSaveAs]::mouse_event([Win32FgSaveAs]::MOUSEEVENTF_LEFTDOWN, 0, 0, 0, [UIntPtr]::Zero)
[Win32FgSaveAs]::mouse_event([Win32FgSaveAs]::MOUSEEVENTF_LEFTUP, 0, 0, 0, [UIntPtr]::Zero)

Write-Host "Waiting for the 'Save As' dialog to appear..."
# Name AND ControlType=Window - same lesson as the 'Basic input' lookup earlier in this toolkit:
# matching on Name alone risks finding some other element that happens to share the same text
# (a title/icon label, say) instead of the actual dialog window - which would make every search
# "inside" it silently find nothing, exactly as just happened.
$saveAsDialogCondition = New-Object System.Windows.Automation.AndCondition(
    (New-Object System.Windows.Automation.PropertyCondition(
        [System.Windows.Automation.AutomationElement]::NameProperty, "Save As")),
    (New-Object System.Windows.Automation.PropertyCondition(
        [System.Windows.Automation.AutomationElement]::ControlTypeProperty, [System.Windows.Automation.ControlType]::Window))
)
$saveAsDialog = $null
for ($i = 0; $i -lt 30; $i++) {
    $saveAsDialog = $window.FindFirst([System.Windows.Automation.TreeScope]::Descendants, $saveAsDialogCondition)
    if ($null -ne $saveAsDialog) { break }
    Start-Sleep -Milliseconds 200
}
if ($null -eq $saveAsDialog) {
    Write-Host "Could not find the 'Save As' dialog after waiting ~6 seconds." -ForegroundColor Red
    return
}

if (-not $TargetPath) {
    Write-Host "Done - the Save File dialog is open (no -TargetPath given, stopping here)." -ForegroundColor Green
    return
}

# CONFIRMED (via FindAll enumerating every Edit/ComboBox under this dialog - only the unrelated
# search box turned up) that the actual "File name:" INPUT is not reachable via tree-walking at all,
# despite being visible and hoverable in Accessibility Insights - the same category of problem as
# the grid's own edit overlay earlier in this session. The LABEL beside it ("File name:",
# ControlType=Text, AutomationId='SaveDialogLabel') IS reachable, though, and the input sits
# immediately to its right at the same vertical position in every standard Windows Save dialog -
# using that as a coordinate anchor instead of searching for the input directly.
Write-Host "Finding the 'File name:' label to anchor a click on the actual input field beside it..."
$fileNameLabel = $saveAsDialog.FindFirst(
    [System.Windows.Automation.TreeScope]::Descendants,
    (New-Object System.Windows.Automation.AndCondition(
        (New-Object System.Windows.Automation.PropertyCondition(
            [System.Windows.Automation.AutomationElement]::NameProperty, "File name:")),
        (New-Object System.Windows.Automation.PropertyCondition(
            [System.Windows.Automation.AutomationElement]::ControlTypeProperty, [System.Windows.Automation.ControlType]::Text))
    ))
)
if ($null -eq $fileNameLabel) {
    Write-Host "Could not find the 'File name:' label." -ForegroundColor Red
    return
}
$labelRect = $fileNameLabel.Current.BoundingRectangle
$clickX = [int]($labelRect.X + $labelRect.Width + 10)
$clickY = [int]($labelRect.Y + $labelRect.Height / 2)

Write-Host "Clicking the input field at ($clickX, $clickY), selecting all, typing the path, and pressing Enter to save..."
[System.Windows.Forms.Cursor]::Position = New-Object System.Drawing.Point($clickX, $clickY)
Start-Sleep -Milliseconds 150
[Win32FgSaveAs]::mouse_event([Win32FgSaveAs]::MOUSEEVENTF_LEFTDOWN, 0, 0, 0, [UIntPtr]::Zero)
[Win32FgSaveAs]::mouse_event([Win32FgSaveAs]::MOUSEEVENTF_LEFTUP, 0, 0, 0, [UIntPtr]::Zero)
Start-Sleep -Milliseconds 150

[System.Windows.Forms.SendKeys]::SendWait("^a")
Start-Sleep -Milliseconds 100
# SendKeys treats {}()%^~+ as special - escape anything that appears in the actual path so it types
# as literal text rather than being interpreted as a key combination or placeholder.
$escapedPath = $TargetPath -replace '([{}()%^~+])', '{$1}'
[System.Windows.Forms.SendKeys]::SendWait($escapedPath)
Start-Sleep -Milliseconds 100
[System.Windows.Forms.SendKeys]::SendWait("{ENTER}")

Write-Host "Done - saved to: $TargetPath" -ForegroundColor Green
