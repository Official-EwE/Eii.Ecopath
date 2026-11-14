<#
.SYNOPSIS
    Brings the app to foreground, finds the 'Basic input' window and the largest nested Pane inside
    it (the actual grid content area - see this script's own history for why "largest of several
    sibling panes" rather than "first found" or "first nested"), and double-clicks near that pane's
    own top-left corner, computed from its LIVE BoundingRectangle rather than a hardcoded screen
    coordinate. Confirmed to reliably select/focus the first grid cell.
    Deliberately stops there: the resulting in-grid edit overlay is NOT reachable via UI Automation
    tree-walking at all (confirmed repeatedly - FindFirst/FindAll find nothing, scoped or unscoped,
    despite the control visibly existing and holding keyboard focus). The toolbar box next to
    "Apply" is the actual target for setting a value - a separate, ordinary control, reachable
    normally - once a cell is selected via this script.

.PARAMETER Proc
    The app's Process object (e.g. from Start-Process -PassThru, or Get-Process).

.PARAMETER PaneOffsetX
.PARAMETER PaneOffsetY
    Click position as a small offset into the found pane's own top-left corner (queried fresh every
    run) - not a hardcoded absolute screen coordinate, and not an offset from the whole 'Basic
    input' window either, since the pane should wrap the grid's data rows more tightly (excluding
    the toolbar/column headers), making a small, mostly-window-position-independent offset more
    likely to land on row 1 reliably. Tune if it doesn't land on the intended cell.
#>
param(
    [Parameter(Mandatory = $true)]
    [System.Diagnostics.Process]$Proc,
    [int]$PaneOffsetX = 10,
    [int]$PaneOffsetY = 10
)

Add-Type -AssemblyName UIAutomationClient, UIAutomationTypes, System.Windows.Forms, System.Drawing
Add-Type @"
using System;
using System.Runtime.InteropServices;
public class Win32Mouse {
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

function Click-Left {
    [Win32Mouse]::mouse_event([Win32Mouse]::MOUSEEVENTF_LEFTDOWN, 0, 0, 0, [UIntPtr]::Zero)
    [Win32Mouse]::mouse_event([Win32Mouse]::MOUSEEVENTF_LEFTUP, 0, 0, 0, [UIntPtr]::Zero)
}

Write-Host "Bringing app window to foreground..."
$sfwResult = [Win32Mouse]::SetForegroundWindow($Proc.MainWindowHandle)
Start-Sleep -Milliseconds 300

$actualForeground = [Win32Mouse]::GetForegroundWindow()
if ($actualForeground -ne $Proc.MainWindowHandle) {
    Write-Host "WARNING: the app is NOT actually the foreground window (SetForegroundWindow returned $sfwResult, but actual foreground handle is $actualForeground, expected $($Proc.MainWindowHandle))." -ForegroundColor Red
    Write-Host "Windows may be throttling repeated focus-stealing from this script. Click into the app window manually once, then re-run this script immediately after." -ForegroundColor Red
}

$window = [System.Windows.Automation.AutomationElement]::FromHandle($Proc.MainWindowHandle)

Write-Host "Looking for the 'Basic input' window..."
$basicInputWindow = $window.FindFirst(
    [System.Windows.Automation.TreeScope]::Descendants,
    (New-Object System.Windows.Automation.AndCondition(
        (New-Object System.Windows.Automation.PropertyCondition(
            [System.Windows.Automation.AutomationElement]::NameProperty, "Basic input")),
        (New-Object System.Windows.Automation.PropertyCondition(
            [System.Windows.Automation.AutomationElement]::ControlTypeProperty, [System.Windows.Automation.ControlType]::Window))
    ))
)
if ($null -eq $basicInputWindow) {
    Write-Host "Could not find the 'Basic input' window at all." -ForegroundColor Red
    return
}
Write-Host "  Found. BoundingRectangle: $($basicInputWindow.Current.BoundingRectangle)"

Write-Host "Looking for the first Pane inside it..."
$paneCondition = New-Object System.Windows.Automation.PropertyCondition(
    [System.Windows.Automation.AutomationElement]::ControlTypeProperty, [System.Windows.Automation.ControlType]::Pane)
$outerPane = $basicInputWindow.FindFirst([System.Windows.Automation.TreeScope]::Descendants, $paneCondition)
if ($null -eq $outerPane) {
    Write-Host "Could not find any Pane inside 'Basic input'." -ForegroundColor Red
    return
}
Write-Host "  Found. BoundingRectangle: $($outerPane.Current.BoundingRectangle)"

# The outer pane has SEVERAL sibling children at the next level (confirmed via Accessibility
# Insights: a pane, two scroll bars, then several more panes) - FindFirst was grabbing whichever one
# happened to come first in document order, which turned out to be a tiny 17x17 corner widget
# (likely a spreadsheet-style "select all" button), not the actual grid content. Enumerating ALL of
# them and picking the one with the LARGEST area instead - the real scrollable data area should be
# far bigger than any corner widget or decorative element at that level.
Write-Host "Looking for all Panes nested inside the outer one, picking the largest..."
$allNestedPanes = $outerPane.FindAll([System.Windows.Automation.TreeScope]::Descendants, $paneCondition)
if ($allNestedPanes.Count -eq 0) {
    Write-Host "No nested Panes found - falling back to the outer one." -ForegroundColor Yellow
    $pane = $outerPane
} else {
    $pane = $allNestedPanes | Sort-Object { $_.Current.BoundingRectangle.Width * $_.Current.BoundingRectangle.Height } -Descending | Select-Object -First 1
    Write-Host "  ($($allNestedPanes.Count) candidates found; largest selected)"
}
$paneRect = $pane.Current.BoundingRectangle
Write-Host "  Found. BoundingRectangle: $paneRect"

$clickX = [int]$paneRect.X + $PaneOffsetX
$clickY = [int]$paneRect.Y + $PaneOffsetY

Write-Host "Double-clicking at ($clickX, $clickY) - pane's top-left plus ($PaneOffsetX, $PaneOffsetY)..."
[System.Windows.Forms.Cursor]::Position = New-Object System.Drawing.Point($clickX, $clickY)
Start-Sleep -Milliseconds 100
Click-Left
Start-Sleep -Milliseconds 100
Click-Left
Start-Sleep -Milliseconds 300

Write-Host "Done - the target cell should now be selected/focused." -ForegroundColor Green
