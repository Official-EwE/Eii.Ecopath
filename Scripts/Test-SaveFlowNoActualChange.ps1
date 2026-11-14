<#
.SYNOPSIS
    Selects the grid's first cell, copies its current value via the clipboard, overwrites it twice
    (0, then 1) to reliably mark the model as modified, restores the ORIGINAL value by pasting it
    back (not retyping it - avoids any formatting mismatch, e.g. "1,000" vs "1"), then clicks Save.
    Net effect: Save fires, but the saved value is byte-for-byte identical to what it was before
    this script ran.

.PARAMETER Proc
    The app's Process object (e.g. from Start-Process -PassThru, or Get-Process).

.PARAMETER PaneOffsetX
.PARAMETER PaneOffsetY
    Same meaning as in Test-EditCellEndToEnd.ps1 - passed through to the repeated cell-selection
    routine below, which is the same logic that script already confirmed reliable.
#>
param(
    [Parameter(Mandatory = $true)]
    [System.Diagnostics.Process]$Proc,
    [int]$PaneOffsetX = 10,
    [int]$PaneOffsetY = 10
)

Add-Type -AssemblyName UIAutomationClient, UIAutomationTypes, System.Windows.Forms, System.Drawing

# Distinct type name, and an existence check besides: Add-Type cannot redefine a type once loaded
# into a session, which previously broke Test-EditCellEndToEnd.ps1 when run repeatedly after edits -
# this avoids that regardless of what else has already run in the current session.
if (-not ("Win32MouseSaveFlow" -as [type])) {
    Add-Type @"
using System;
using System.Runtime.InteropServices;
public class Win32MouseSaveFlow {
    [DllImport("user32.dll")]
    public static extern void mouse_event(uint dwFlags, int dx, int dy, uint dwData, UIntPtr dwExtraInfo);
    [DllImport("user32.dll")]
    public static extern bool SetForegroundWindow(IntPtr hWnd);
    [DllImport("user32.dll")]
    public static extern IntPtr GetForegroundWindow();
    public const uint MOUSEEVENTF_LEFTDOWN = 0x02;
    public const uint MOUSEEVENTF_LEFTUP = 0x04;
}
"@
}

function Select-GridCell {
    param([System.Diagnostics.Process]$Proc, [int]$OffsetX, [int]$OffsetY)

    $sfwResult = [Win32MouseSaveFlow]::SetForegroundWindow($Proc.MainWindowHandle)
    Start-Sleep -Milliseconds 300
    $actualForeground = [Win32MouseSaveFlow]::GetForegroundWindow()
    if ($actualForeground -ne $Proc.MainWindowHandle) {
        Write-Host "  WARNING: app is not actually foreground (throttled focus-steal?) - click into it manually once if this keeps happening." -ForegroundColor Red
    }

    $window = [System.Windows.Automation.AutomationElement]::FromHandle($Proc.MainWindowHandle)
    $basicInputWindow = $window.FindFirst(
        [System.Windows.Automation.TreeScope]::Descendants,
        (New-Object System.Windows.Automation.AndCondition(
            (New-Object System.Windows.Automation.PropertyCondition(
                [System.Windows.Automation.AutomationElement]::NameProperty, "Basic input")),
            (New-Object System.Windows.Automation.PropertyCondition(
                [System.Windows.Automation.AutomationElement]::ControlTypeProperty, [System.Windows.Automation.ControlType]::Window))
        ))
    )
    if ($null -eq $basicInputWindow) { throw "Could not find the 'Basic input' window." }

    $paneCondition = New-Object System.Windows.Automation.PropertyCondition(
        [System.Windows.Automation.AutomationElement]::ControlTypeProperty, [System.Windows.Automation.ControlType]::Pane)
    $outerPane = $basicInputWindow.FindFirst([System.Windows.Automation.TreeScope]::Descendants, $paneCondition)
    if ($null -eq $outerPane) { throw "Could not find any Pane inside 'Basic input'." }

    $allNestedPanes = $outerPane.FindAll([System.Windows.Automation.TreeScope]::Descendants, $paneCondition)
    $pane = if ($allNestedPanes.Count -eq 0) {
        $outerPane
    } else {
        $allNestedPanes | Sort-Object { $_.Current.BoundingRectangle.Width * $_.Current.BoundingRectangle.Height } -Descending | Select-Object -First 1
    }
    $paneRect = $pane.Current.BoundingRectangle

    $clickX = [int]$paneRect.X + $OffsetX
    $clickY = [int]$paneRect.Y + $OffsetY
    [System.Windows.Forms.Cursor]::Position = New-Object System.Drawing.Point($clickX, $clickY)
    Start-Sleep -Milliseconds 100
    [Win32MouseSaveFlow]::mouse_event([Win32MouseSaveFlow]::MOUSEEVENTF_LEFTDOWN, 0, 0, 0, [UIntPtr]::Zero)
    [Win32MouseSaveFlow]::mouse_event([Win32MouseSaveFlow]::MOUSEEVENTF_LEFTUP, 0, 0, 0, [UIntPtr]::Zero)
    Start-Sleep -Milliseconds 100
    [Win32MouseSaveFlow]::mouse_event([Win32MouseSaveFlow]::MOUSEEVENTF_LEFTDOWN, 0, 0, 0, [UIntPtr]::Zero)
    [Win32MouseSaveFlow]::mouse_event([Win32MouseSaveFlow]::MOUSEEVENTF_LEFTUP, 0, 0, 0, [UIntPtr]::Zero)
    Start-Sleep -Milliseconds 300

    return $window
}

Write-Host "Step 1: selecting the cell and copying its current value..."
Select-GridCell -Proc $Proc -OffsetX $PaneOffsetX -OffsetY $PaneOffsetY | Out-Null
[System.Windows.Forms.SendKeys]::SendWait("^c")
Start-Sleep -Milliseconds 150
$originalValue = Get-Clipboard
Write-Host "  Captured original value: '$originalValue'" -ForegroundColor Green
if ([string]::IsNullOrWhiteSpace($originalValue)) {
    Write-Host "  WARNING: clipboard came back empty - Ctrl+C may not have landed on the selected cell. Aborting rather than risk restoring a blank value." -ForegroundColor Red
    return
}

Write-Host "Step 2: entering 0..."
[System.Windows.Forms.SendKeys]::SendWait("0")
[System.Windows.Forms.SendKeys]::SendWait("{ENTER}")
Start-Sleep -Milliseconds 300

Write-Host "Step 3: re-selecting the cell, entering 1..."
Select-GridCell -Proc $Proc -OffsetX $PaneOffsetX -OffsetY $PaneOffsetY | Out-Null
[System.Windows.Forms.SendKeys]::SendWait("1")
[System.Windows.Forms.SendKeys]::SendWait("{ENTER}")
Start-Sleep -Milliseconds 300

Write-Host "Step 4: re-selecting the cell, restoring the original value via paste..."
Select-GridCell -Proc $Proc -OffsetX $PaneOffsetX -OffsetY $PaneOffsetY | Out-Null
[System.Windows.Forms.SendKeys]::SendWait("^v")
[System.Windows.Forms.SendKeys]::SendWait("{ENTER}")
Start-Sleep -Milliseconds 300

Write-Host "Step 5: clicking Save changes..."
$window = [System.Windows.Automation.AutomationElement]::FromHandle($Proc.MainWindowHandle)
$saveCondition = New-Object System.Windows.Automation.AndCondition(
    (New-Object System.Windows.Automation.PropertyCondition(
        [System.Windows.Automation.AutomationElement]::NameProperty, "Save changes")),
    (New-Object System.Windows.Automation.PropertyCondition(
        [System.Windows.Automation.AutomationElement]::ControlTypeProperty, [System.Windows.Automation.ControlType]::Button))
)
$saveButton = $window.FindFirst([System.Windows.Automation.TreeScope]::Descendants, $saveCondition)
if ($null -eq $saveButton) {
    Write-Host "  Could not find the Save changes button." -ForegroundColor Red
    return
}
$invokePattern = $saveButton.GetCurrentPattern([System.Windows.Automation.InvokePattern]::Pattern)
$invokePattern.Invoke()

Write-Host "Done. Value should be restored to '$originalValue' and the save should have gone through." -ForegroundColor Green
