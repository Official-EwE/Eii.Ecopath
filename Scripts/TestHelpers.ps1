<#
.SYNOPSIS
    Shared test framework (pass/fail tracking, summary) and UI-automation helpers, dot-sourced by
    every Run-*.ps1 test runner in this toolkit. Consolidated here after several runners started
    duplicating the same Win32 interop and element-finding code.

.EXAMPLE
    . (Join-Path $PSScriptRoot "TestHelpers.ps1")
#>

Add-Type -AssemblyName UIAutomationClient, UIAutomationTypes, System.Windows.Forms, System.Drawing

# Checked against Win32Rect specifically (the newest addition), not Win32TestRunner: .NET cannot
# redefine a type once loaded, so a session that already has an OLDER Win32TestRunner loaded (from
# before Win32Rect was added here) would otherwise skip this whole block and silently never define
# Win32Rect at all - exactly what happened once already. A clear, actionable error here instead of
# letting that surface later as a confusing "Cannot find type" failure deep inside some other call.
if (-not ("Win32Rect" -as [type])) {
    if ("Win32TestRunner" -as [type]) {
        throw "A stale version of Win32TestRunner is already loaded in this PowerShell session, from before Win32Rect was added. .NET cannot redefine a type once loaded - please close this PowerShell window, open a fresh one, and try again."
    }
    Add-Type @"
using System;
using System.Runtime.InteropServices;
public struct Win32Rect {
    public int Left;
    public int Top;
    public int Right;
    public int Bottom;
}
public class Win32TestRunner {
    [DllImport("user32.dll")]
    public static extern bool SetForegroundWindow(IntPtr hWnd);
    [DllImport("user32.dll")]
    public static extern void mouse_event(uint dwFlags, int dx, int dy, uint dwData, UIntPtr dwExtraInfo);
    [DllImport("user32.dll")]
    public static extern bool GetWindowRect(IntPtr hWnd, out Win32Rect lpRect);
    [DllImport("user32.dll")]
    public static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);
    public const uint MOUSEEVENTF_LEFTDOWN = 0x02;
    public const uint MOUSEEVENTF_LEFTUP = 0x04;
}
"@
}

# ===== Test framework =====

# NOT unconditionally reassigned: Run-AllTests.ps1 dot-sources multiple runner scripts in sequence,
# each of which dot-sources this file again - resetting the list here every time would lose all
# results from every stage but the last. Guarded so it's only created once per session.
if (-not $script:TestResults) {
    $script:TestResults = New-Object System.Collections.Generic.List[PSCustomObject]
}

# Set by Run-AllTests.ps1 before running each platform's pass (e.g. "x64"), so results from
# different platforms stay distinguishable in one combined summary - left $null when a runner is
# used standalone, where there's only ever one platform in play anyway.
if (-not (Test-Path variable:script:CurrentPlatformLabel)) {
    $script:CurrentPlatformLabel = $null
}

function Add-TestResult {
    param(
        [Parameter(Mandatory = $true)][string]$Name,
        [Parameter(Mandatory = $true)][ValidateSet("PASS", "FAIL", "SKIP")][string]$Result,
        [int]$DurationMs = 0,
        [string]$Reason = ""
    )
    $displayName = if ($script:CurrentPlatformLabel) { "[$script:CurrentPlatformLabel] $Name" } else { $Name }
    $script:TestResults.Add([PSCustomObject]@{ Name = $displayName; Result = $Result; DurationMs = $DurationMs; Reason = $Reason })
}

function Invoke-Test {
    param(
        [Parameter(Mandatory = $true)][string]$Name,
        [Parameter(Mandatory = $true)][scriptblock]$Test
    )
    # Set by a runner's own -Only parameter, if given - a substring match (case-insensitive) against
    # the test name, letting a single test run in isolation (e.g. to tell apart a genuine bug from
    # an artifact of running several similar tests back-to-back in one session). Silently skipped,
    # not even recorded, so an -Only run's summary only ever shows what was actually asked for.
    if ($script:TestNameFilter -and $Name -notlike "*$script:TestNameFilter*") {
        return
    }
    $displayName = if ($script:CurrentPlatformLabel) { "[$script:CurrentPlatformLabel] $Name" } else { $Name }
    Write-Host "`n=== $displayName ===" -ForegroundColor Cyan
    $sw = [System.Diagnostics.Stopwatch]::StartNew()
    try {
        & $Test
        $sw.Stop()
        Add-TestResult -Name $Name -Result "PASS" -DurationMs $sw.ElapsedMilliseconds
        Write-Host "PASS ($($sw.ElapsedMilliseconds) ms)" -ForegroundColor Green
    } catch {
        $sw.Stop()
        Add-TestResult -Name $Name -Result "FAIL" -DurationMs $sw.ElapsedMilliseconds -Reason $_.Exception.Message
        Write-Host "FAIL ($($sw.ElapsedMilliseconds) ms): $($_.Exception.Message)" -ForegroundColor Red
    }
}

# Pauses for a manual check that isn't reliably automatable yet. Records the person's own answer.
function Invoke-ManualTest {
    param(
        [Parameter(Mandatory = $true)][string]$Name,
        [Parameter(Mandatory = $true)][string]$Instructions
    )
    if ($script:TestNameFilter -and $Name -notlike "*$script:TestNameFilter*") {
        return
    }
    $displayName = if ($script:CurrentPlatformLabel) { "[$script:CurrentPlatformLabel] $Name" } else { $Name }
    Write-Host "`n=== $displayName (MANUAL) ===" -ForegroundColor Yellow
    Write-Host $Instructions
    $response = Read-Host "Did this pass? (y/n/skip)"
    switch ($response.ToLowerInvariant()) {
        "skip" { Add-TestResult -Name $Name -Result "SKIP" }
        "y"    { Add-TestResult -Name $Name -Result "PASS" -Reason "(manual)" }
        default { Add-TestResult -Name $Name -Result "FAIL" -Reason (Read-Host "Failure reason") }
    }
}

# Prints "  <description>..." with no newline, runs Action, then " done" on success. An exception
# from Action propagates normally (so the test's own Assert-True/throw-based failure handling in
# Invoke-Test still works) - on failure, the description line is left exactly as printed, with no
# "done" after it, which is itself useful: the output shows precisely which step a test got to
# before it failed. Returns whatever Action itself returns, so a step that finds/computes something
# a later step needs can still be used normally.
function Write-Step {
    param(
        [Parameter(Mandatory = $true)][string]$Description,
        [Parameter(Mandatory = $true)][scriptblock]$Action
    )
    Write-Host "  $Description..." -NoNewline
    $result = & $Action
    Write-Host " done" -ForegroundColor Green
    $result
}

function Assert-True {
    param([bool]$Condition, [string]$Message)
    if (-not $Condition) { throw $Message }
}

function Format-Duration {
    param([Parameter(Mandatory = $true)][TimeSpan]$TimeSpan)
    if ($TimeSpan.TotalMinutes -ge 1) {
        "{0}m {1}s" -f [int]$TimeSpan.TotalMinutes, $TimeSpan.Seconds
    } else {
        "{0:N1}s" -f $TimeSpan.TotalSeconds
    }
}

function Show-TestSummary {
    Write-Host "`n`n========== TEST SUMMARY ==========" -ForegroundColor Cyan
    $script:TestResults | Format-Table Name, Result, DurationMs, Reason -AutoSize | Out-Host

    $passed = ($script:TestResults | Where-Object { $_.Result -eq "PASS" }).Count
    $failed = ($script:TestResults | Where-Object { $_.Result -eq "FAIL" }).Count
    $skipped = ($script:TestResults | Where-Object { $_.Result -eq "SKIP" }).Count
    $total = $script:TestResults.Count

    $color = if ($failed -gt 0) { "Red" } else { "Green" }
    Write-Host "Total: $total   Passed: $passed   Failed: $failed   Skipped: $skipped" -ForegroundColor $color

    if ($failed -gt 0) {
        Write-Host "`nFailures:" -ForegroundColor Red
        $script:TestResults | Where-Object { $_.Result -eq "FAIL" } | ForEach-Object {
            Write-Host "  - $($_.Name): $($_.Reason)" -ForegroundColor Red
        }
    }
}

# ===== UI-automation helpers =====

function Set-AppForeground {
    param([Parameter(Mandatory = $true)][System.Diagnostics.Process]$Proc)
    [Win32TestRunner]::SetForegroundWindow($Proc.MainWindowHandle) | Out-Null
    Start-Sleep -Milliseconds 300
}

function Get-AppWindow {
    param([Parameter(Mandatory = $true)][System.Diagnostics.Process]$Proc)
    [System.Windows.Automation.AutomationElement]::FromHandle($Proc.MainWindowHandle)
}

function Find-ByNameAndType {
    param($Root, [string]$Name, [System.Windows.Automation.ControlType]$ControlType, $Scope = [System.Windows.Automation.TreeScope]::Descendants)
    $condition = New-Object System.Windows.Automation.AndCondition(
        (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::NameProperty, $Name)),
        (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::ControlTypeProperty, $ControlType))
    )
    $Root.FindFirst($Scope, $condition)
}

# A raw mouse click, not InvokePattern.Invoke(): several controls in this app show a MODAL dialog
# synchronously from their own click handler (Save Model As..., Save when locked) - Invoke() blocks
# for as long as that dialog stays open, since it's a COM call into the target process. A raw click
# is not a COM call at all, so it can never block on whatever the handler goes on to do.
function Invoke-ElementClick {
    param([Parameter(Mandatory = $true)]$Element)
    $rect = $Element.Current.BoundingRectangle
    [System.Windows.Forms.Cursor]::Position = New-Object System.Drawing.Point([int]($rect.X + $rect.Width / 2), [int]($rect.Y + $rect.Height / 2))
    Start-Sleep -Milliseconds 100
    [Win32TestRunner]::mouse_event([Win32TestRunner]::MOUSEEVENTF_LEFTDOWN, 0, 0, 0, [UIntPtr]::Zero)
    [Win32TestRunner]::mouse_event([Win32TestRunner]::MOUSEEVENTF_LEFTUP, 0, 0, 0, [UIntPtr]::Zero)
}

function ConvertTo-SendKeysLiteral {
    param([string]$Text)
    $Text -replace '([{}()%^~+])', '{$1}'
}

# Condensed from this session's own grid-cell investigation - see Test-EditCellEndToEnd.ps1 for the
# full history of why this specific approach (pane-size heuristic, live-rectangle-relative click,
# not the dialog's own AutomationId/Name - none exist for this control) was needed.
function Select-GridFirstCell {
    param([Parameter(Mandatory = $true)][System.Diagnostics.Process]$Proc)
    Set-AppForeground -Proc $Proc
    $window = Get-AppWindow -Proc $Proc
    # Polling, not a single immediate check: the main window appearing doesn't guarantee the
    # 'Basic input' sub-window has finished initializing yet - confirmed to matter when this runs
    # right after a fresh restart mid-session (e.g. immediately following "2.x"'s own restart step).
    $basicInputWindow = $null
    $basicInputDeadline = (Get-Date).AddSeconds(15)
    while ($null -eq $basicInputWindow -and (Get-Date) -lt $basicInputDeadline) {
        $basicInputWindow = Find-ByNameAndType -Root $window -Name "Basic input" -ControlType ([System.Windows.Automation.ControlType]::Window)
        if ($null -eq $basicInputWindow) { Start-Sleep -Milliseconds 300 }
    }
    if ($null -eq $basicInputWindow) { throw "Could not find the 'Basic input' window after ~15 seconds." }

    $paneCondition = New-Object System.Windows.Automation.PropertyCondition(
        [System.Windows.Automation.AutomationElement]::ControlTypeProperty, [System.Windows.Automation.ControlType]::Pane)
    $outerPane = $basicInputWindow.FindFirst([System.Windows.Automation.TreeScope]::Descendants, $paneCondition)
    if ($null -eq $outerPane) { throw "Could not find any Pane inside 'Basic input'." }

    $allNestedPanes = $outerPane.FindAll([System.Windows.Automation.TreeScope]::Descendants, $paneCondition)
    # @(...)[0], not relying on Select-Object -First 1 alone, to guarantee a single element no
    # matter what - confirmed this matters: a prior version of this hit "System.Object[] does not
    # contain a method named 'op_Addition'" further down, consistent with $pane ending up as an
    # array rather than one element in some cases.
    $pane = if ($allNestedPanes.Count -eq 0) {
        $outerPane
    } else {
        @($allNestedPanes | Sort-Object { $_.Current.BoundingRectangle.Width * $_.Current.BoundingRectangle.Height } -Descending)[0]
    }
    $rect = $pane.Current.BoundingRectangle
    # Cast-then-add computed into their own variables first, not inline in the Point constructor -
    # removes any ambiguity in how an inline "[int]$x + 10" expression gets parsed/evaluated.
    $clickX = [int]($rect.X) + 10
    $clickY = [int]($rect.Y) + 10
    [System.Windows.Forms.Cursor]::Position = New-Object System.Drawing.Point($clickX, $clickY)
    Start-Sleep -Milliseconds 100
    [Win32TestRunner]::mouse_event([Win32TestRunner]::MOUSEEVENTF_LEFTDOWN, 0, 0, 0, [UIntPtr]::Zero)
    [Win32TestRunner]::mouse_event([Win32TestRunner]::MOUSEEVENTF_LEFTUP, 0, 0, 0, [UIntPtr]::Zero)
    Start-Sleep -Milliseconds 100
    [Win32TestRunner]::mouse_event([Win32TestRunner]::MOUSEEVENTF_LEFTDOWN, 0, 0, 0, [UIntPtr]::Zero)
    [Win32TestRunner]::mouse_event([Win32TestRunner]::MOUSEEVENTF_LEFTUP, 0, 0, 0, [UIntPtr]::Zero)
    Start-Sleep -Milliseconds 300
}

function Invoke-SaveChanges {
    param([Parameter(Mandatory = $true)][System.Diagnostics.Process]$Proc)
    $window = Get-AppWindow -Proc $Proc
    $saveButton = Find-ByNameAndType -Root $window -Name "Save changes" -ControlType ([System.Windows.Automation.ControlType]::Button)
    if ($null -eq $saveButton) { throw "Could not find the 'Save changes' button." }
    Invoke-ElementClick -Element $saveButton
}

function Test-SaveButtonEnabled {
    param([Parameter(Mandatory = $true)][System.Diagnostics.Process]$Proc)
    $window = Get-AppWindow -Proc $Proc
    $saveButton = Find-ByNameAndType -Root $window -Name "Save changes" -ControlType ([System.Windows.Automation.ControlType]::Button)
    if ($null -eq $saveButton) { throw "Could not find the 'Save changes' button." }
    $saveButton.Current.IsEnabled
}

# Makes a safe, fully-restored edit via the clipboard (see Test-SaveFlowNoActualChange.ps1 for the
# original design) - enables Save without permanently changing the model's real data. Does NOT
# itself click Save; callers decide when to do that.
function Set-SafeRestorableEdit {
    param([Parameter(Mandatory = $true)][System.Diagnostics.Process]$Proc)
    Select-GridFirstCell -Proc $Proc
    [System.Windows.Forms.SendKeys]::SendWait("^c")
    Start-Sleep -Milliseconds 150
    $originalValue = Get-Clipboard
    if ([string]::IsNullOrWhiteSpace($originalValue)) { throw "Clipboard was empty after Ctrl+C - selection may have failed." }

    [System.Windows.Forms.SendKeys]::SendWait("0")
    [System.Windows.Forms.SendKeys]::SendWait("{ENTER}")
    Start-Sleep -Milliseconds 300

    Select-GridFirstCell -Proc $Proc
    [System.Windows.Forms.SendKeys]::SendWait("^v")
    [System.Windows.Forms.SendKeys]::SendWait("{ENTER}")
    Start-Sleep -Milliseconds 300
}

# Polls the staging folder for Temp*-prefixed working copies (any extension - CONFIRMED this
# session that the staged copy keeps the source file's own extension, not normalized to .eweaccdb).
function Get-StagedCopies {
    $stagingDir = Join-Path $env:LocalAppData "Ecopath with Ecosim\DBs"
    if (-not (Test-Path $stagingDir)) { return @() }
    Get-ChildItem -Path $stagingDir -Filter "Temp*" -ErrorAction SilentlyContinue | Where-Object { $_.Extension -ne ".ldb" }
}

# Waits for a just-started process's main window, handling the "your model database will be
# updated to a newer version" confirmation along the way if it appears - it can show up BEFORE
# MainWindowHandle is even populated, so a caller that only waits on that handle would hang with
# nobody clicking it. Shared by every launch site in this toolkit (Start-TestInstance.ps1 and
# Run-MultiInstanceTests.ps1's own second-instance launcher) after this gap showed up in one but not
# the other.
function Wait-ForMainWindow {
    param(
        [Parameter(Mandatory = $true)][System.Diagnostics.Process]$Proc,
        [int]$TimeoutSeconds = 90
    )
    $root = [System.Windows.Automation.AutomationElement]::RootElement
    # Name only, NOT ControlType=Button, for the Yes button: CONFIRMED via Accessibility Insights
    # that this dialog exposes its buttons as ControlType.Pane, not Button - likely bridged through
    # the legacy IAccessible/MSAA proxy (AutomationId is a raw numeric Win32 control ID).
    $upgradeDialogCondition = New-Object System.Windows.Automation.AndCondition(@(
        (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::ProcessIdProperty, $Proc.Id)),
        (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::NameProperty, "Ecopath with Ecosim")),
        (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::ControlTypeProperty, [System.Windows.Automation.ControlType]::Window))
    ))
    $yesCondition = New-Object System.Windows.Automation.PropertyCondition(
        [System.Windows.Automation.AutomationElement]::NameProperty, "Yes")

    $deadline = (Get-Date).AddSeconds($TimeoutSeconds)
    $upgradeDialogSeen = $false
    while ($Proc.MainWindowHandle -eq [IntPtr]::Zero -and (Get-Date) -lt $deadline) {
        $upgradeDialog = $root.FindFirst([System.Windows.Automation.TreeScope]::Children, $upgradeDialogCondition)
        if ($null -ne $upgradeDialog) {
            if (-not $upgradeDialogSeen) {
                Write-Host "Database-upgrade prompt detected - clicking Yes..." -NoNewline
                $upgradeDialogSeen = $true
            } else {
                Write-Host "." -NoNewline
            }
            # A benign, expected race: the dialog can genuinely disappear (because a PRIOR
            # iteration's own click already succeeded) in the narrow window between being found
            # here and being searched/clicked below - confirmed via RPC_E_SERVER_DIED_DNE, the COM
            # error for exactly that. Treated the same as simply not finding the Yes button this
            # iteration, rather than letting a raw COM exception surface for something expected.
            try {
                $yesButton = $upgradeDialog.FindFirst([System.Windows.Automation.TreeScope]::Descendants, $yesCondition)
                if ($null -ne $yesButton) { Invoke-ElementClick -Element $yesButton }
            } catch {
            }
        } elseif ($upgradeDialogSeen) {
            Write-Host " done." -ForegroundColor Green
            $upgradeDialogSeen = $false
        }
        Start-Sleep -Milliseconds 200
        $Proc.Refresh()
    }
    if ($upgradeDialogSeen) { Write-Host "" }
    if ($Proc.MainWindowHandle -eq [IntPtr]::Zero) {
        throw "Timed out waiting ~$TimeoutSeconds seconds for the main window to appear."
    }
}

# Consistent, labeled start/stop logging for every launched instance, so a run's console output
# alone shows which PID belongs to which test and whether it was actually cleaned up.
function Write-InstanceStarted {
    param([Parameter(Mandatory = $true)][int]$ProcessId, [Parameter(Mandatory = $true)][string]$Label)
    Write-Host "[PID $ProcessId] Started ($Label)" -ForegroundColor DarkCyan
}

function Stop-TestInstance {
    param([Parameter(Mandatory = $true)][int]$ProcessId, [Parameter(Mandatory = $true)][string]$Label)
    Write-Host "[PID $ProcessId] Stopping ($Label)..." -ForegroundColor DarkGray
    Stop-Process -Id $ProcessId -Force -ErrorAction SilentlyContinue
    Write-Host "[PID $ProcessId] Stopped ($Label)." -ForegroundColor DarkGray
}

# Positions two instances' windows so they're visually distinguishable during a multi-instance
# test, rather than stacked exactly on top of each other. Measures A's REAL current size first
# (GetWindowRect), rather than assuming a fixed size, then picks the first layout that actually
# fits the primary screen's working area: stacked vertically, then side by side, then diagonal
# corners as a last resort. Both windows end up the SAME size as A's original (not resized to fit
# more tightly) - this is about separating them visually, not minimizing footprint.
function Set-TwoInstancesSideBySide {
    param(
        [Parameter(Mandatory = $true)][System.Diagnostics.Process]$ProcA,
        [Parameter(Mandatory = $true)][System.Diagnostics.Process]$ProcB
    )

    $rectA = New-Object Win32Rect
    if (-not [Win32TestRunner]::GetWindowRect($ProcA.MainWindowHandle, [ref]$rectA)) {
        Write-Host "Could not measure instance A's window - skipping positioning." -ForegroundColor Yellow
        return
    }
    $width = $rectA.Right - $rectA.Left
    $height = $rectA.Bottom - $rectA.Top

    $workArea = [System.Windows.Forms.Screen]::PrimaryScreen.WorkingArea

    if (($height * 2) -le $workArea.Height) {
        [Win32TestRunner]::MoveWindow($ProcA.MainWindowHandle, $workArea.X, $workArea.Y, $width, $height, $true) | Out-Null
        [Win32TestRunner]::MoveWindow($ProcB.MainWindowHandle, $workArea.X, $workArea.Y + $height, $width, $height, $true) | Out-Null
        Write-Host "Positioned instances stacked vertically ($($width)x$($height) each)." -ForegroundColor DarkGray
    } elseif (($width * 2) -le $workArea.Width) {
        [Win32TestRunner]::MoveWindow($ProcA.MainWindowHandle, $workArea.X, $workArea.Y, $width, $height, $true) | Out-Null
        [Win32TestRunner]::MoveWindow($ProcB.MainWindowHandle, $workArea.X + $width, $workArea.Y, $width, $height, $true) | Out-Null
        Write-Host "Positioned instances side by side ($($width)x$($height) each)." -ForegroundColor DarkGray
    } else {
        [Win32TestRunner]::MoveWindow($ProcA.MainWindowHandle, $workArea.X, $workArea.Y, $width, $height, $true) | Out-Null
        [Win32TestRunner]::MoveWindow($ProcB.MainWindowHandle, $workArea.X + $workArea.Width - $width, $workArea.Y + $workArea.Height - $height, $width, $height, $true) | Out-Null
        Write-Host "Neither stacked nor side-by-side fits - positioned instances diagonally (top-left / bottom-right)." -ForegroundColor DarkGray
    }
}

# Launches a second instance on an ALREADY-copied model path, skipping Start-TestInstance.ps1's own
# build+copy steps (which would otherwise produce a different file each time, not the same one).
# Shared by Run-MultiInstanceTests.ps1 and §2.1's reopen-after-kill step, after both needed it.
function Start-InstanceOnExistingModel {
    param(
        [Parameter(Mandatory = $true)][string]$RepoRoot,
        [Parameter(Mandatory = $true)][string]$ModelPath,
        [string]$Platform = "x64",
        [string]$Configuration = "Debug",
        [int]$MainWindowTimeoutSeconds = 90
    )
    $exePath = Join-Path $RepoRoot "Sources\ScientificInterface\bin\$Platform\$Configuration\net48\ScientificInterface.exe"
    if (-not (Test-Path $exePath)) { throw "Built exe not found: $exePath" }
    $proc = Start-Process -FilePath $exePath -ArgumentList "`"$ModelPath`"" -PassThru
    Wait-ForMainWindow -Proc $proc -TimeoutSeconds $MainWindowTimeoutSeconds
    $proc
}

# Removes the one shared %TEMP%\EwE-TestRun\ parent folder this toolkit's own scripts write
# everything under (each run gets its own subfolder/file there - test-run folders, Save As targets,
# lock-job ready flags) - meant to be called once at the end of each runner. One shared parent,
# deleted whole, rather than a wildcard match against several separately-named top-level items - any
# artifact this toolkit creates lives under this one root, so nothing else needs listing here. The
# one exception is EwE_TestWaitBeforePublish.flag/.ack, which is hardcoded into cCore.vb's own DEBUG
# hook (production code, not ours to rename or relocate for a cleanup convention) - its own test
# already deletes it explicitly in a finally block, so it was never relying on this cleanup anyway.
# Deliberately does NOT touch %LocalAppData%\Ecopath with Ecosim\DBs\ - that's the real app's own
# shared staging folder, not scratch space these tests own, and a genuinely active session elsewhere
# on the machine could have a legitimate file there. -ErrorAction SilentlyContinue: a folder still
# locked by a lingering process is skipped quietly, not an error.
function Clear-TestArtifacts {
    $root = Join-Path $env:TEMP "EwE-TestRun"
    if (Test-Path $root) {
        Remove-Item $root -Recurse -Force -ErrorAction SilentlyContinue
    }
}

# Closes the currently-open model via File > Close Model. "Close Model" is an UNCONFIRMED menu
# item name - a reasonable first guess matching the "Save Model As..." naming pattern already
# confirmed this session, but genuinely untested. If it doesn't match, the real name needs
# confirming via Accessibility Insights, same as every other first-attempt UI target here.
function Close-CurrentModel {
    param([Parameter(Mandatory = $true)][System.Diagnostics.Process]$Proc)
    Set-AppForeground -Proc $Proc
    $window = Get-AppWindow -Proc $Proc
    $fileMenu = Find-ByNameAndType -Root $window -Name "File" -ControlType ([System.Windows.Automation.ControlType]::MenuItem)
    if ($null -eq $fileMenu) { throw "Could not find the File menu." }
    $fileMenu.GetCurrentPattern([System.Windows.Automation.ExpandCollapsePattern]::Pattern).Expand()
    Start-Sleep -Milliseconds 400
    $closeItem = Find-ByNameAndType -Root $window -Name "Close Model" -ControlType ([System.Windows.Automation.ControlType]::MenuItem)
    if ($null -eq $closeItem) { throw "Could not find 'Close Model' - the real menu item name may differ; confirm via Accessibility Insights." }
    Invoke-ElementClick -Element $closeItem
    Start-Sleep -Milliseconds 1000
}

# Opens a model via File > Open Model..., handling the resulting common Open dialog the same way
# Save As's own dialog is handled (same structure, confirmed this session - the actual filename
# input isn't reachable via tree-walking either, so this anchors on the "File name:" label instead,
# same as Save As). "Open Model..." and the dialog's own Name="Open" are both UNCONFIRMED guesses.
function Open-ModelViaUI {
    param(
        [Parameter(Mandatory = $true)][System.Diagnostics.Process]$Proc,
        [Parameter(Mandatory = $true)][string]$Path
    )
    Set-AppForeground -Proc $Proc
    $window = Get-AppWindow -Proc $Proc
    $fileMenu = Find-ByNameAndType -Root $window -Name "File" -ControlType ([System.Windows.Automation.ControlType]::MenuItem)
    if ($null -eq $fileMenu) { throw "Could not find the File menu." }
    $fileMenu.GetCurrentPattern([System.Windows.Automation.ExpandCollapsePattern]::Pattern).Expand()
    Start-Sleep -Milliseconds 400
    $openItem = Find-ByNameAndType -Root $window -Name "Open Model..." -ControlType ([System.Windows.Automation.ControlType]::MenuItem)
    if ($null -eq $openItem) { throw "Could not find 'Open Model...' - the real menu item name may differ; confirm via Accessibility Insights." }
    Invoke-ElementClick -Element $openItem

    $dialogCondition = New-Object System.Windows.Automation.AndCondition(
        (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::NameProperty, "Open")),
        (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::ControlTypeProperty, [System.Windows.Automation.ControlType]::Window))
    )
    $dialog = $null
    for ($i = 0; $i -lt 30; $i++) {
        $dialog = $window.FindFirst([System.Windows.Automation.TreeScope]::Descendants, $dialogCondition)
        if ($null -ne $dialog) { break }
        Start-Sleep -Milliseconds 200
    }
    if ($null -eq $dialog) { throw "'Open' dialog did not appear - its real Name may differ from the assumed 'Open'." }

    $label = Find-ByNameAndType -Root $dialog -Name "File name:" -ControlType ([System.Windows.Automation.ControlType]::Text)
    if ($null -eq $label) { throw "Could not find the 'File name:' label in the Open dialog." }
    $labelRect = $label.Current.BoundingRectangle
    [System.Windows.Forms.Cursor]::Position = New-Object System.Drawing.Point([int]($labelRect.X + $labelRect.Width + 10), [int]($labelRect.Y + $labelRect.Height / 2))
    Start-Sleep -Milliseconds 150
    [Win32TestRunner]::mouse_event([Win32TestRunner]::MOUSEEVENTF_LEFTDOWN, 0, 0, 0, [UIntPtr]::Zero)
    [Win32TestRunner]::mouse_event([Win32TestRunner]::MOUSEEVENTF_LEFTUP, 0, 0, 0, [UIntPtr]::Zero)
    Start-Sleep -Milliseconds 150

    [System.Windows.Forms.SendKeys]::SendWait("^a")
    Start-Sleep -Milliseconds 100
    [System.Windows.Forms.SendKeys]::SendWait((ConvertTo-SendKeysLiteral $Path))
    Start-Sleep -Milliseconds 100
    [System.Windows.Forms.SendKeys]::SendWait("{ENTER}")
    Start-Sleep -Milliseconds 500

    # Opening a model via the UI (not just at initial process launch) can ALSO trigger the
    # "database will be updated to a newer version" confirmation - same generic dialog as
    # Wait-ForMainWindow handles at launch, but scoped to the already-known main window here rather
    # than a whole-desktop search, since MainWindowHandle already exists by this point (unlike at
    # initial launch, which is the only reason that search was needed there). Same Name-only match
    # for the Yes button too (ControlType.Pane, not Button - confirmed earlier this session).
    $upgradeDialogCondition = New-Object System.Windows.Automation.AndCondition(
        (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::NameProperty, "Ecopath with Ecosim")),
        (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::ControlTypeProperty, [System.Windows.Automation.ControlType]::Window))
    )
    $yesCondition = New-Object System.Windows.Automation.PropertyCondition(
        [System.Windows.Automation.AutomationElement]::NameProperty, "Yes")
    $upgradeDeadline = (Get-Date).AddSeconds(5)
    $upgradeDialog = $null
    while ($null -eq $upgradeDialog -and (Get-Date) -lt $upgradeDeadline) {
        $upgradeDialog = $window.FindFirst([System.Windows.Automation.TreeScope]::Descendants, $upgradeDialogCondition)
        if ($null -eq $upgradeDialog) { Start-Sleep -Milliseconds 200 }
    }
    if ($null -ne $upgradeDialog) {
        Write-Host "Database-upgrade prompt detected after Open - clicking Yes..."
        $yesButton = $upgradeDialog.FindFirst([System.Windows.Automation.TreeScope]::Descendants, $yesCondition)
        if ($null -ne $yesButton) {
            Invoke-ElementClick -Element $yesButton
            Start-Sleep -Milliseconds 500
        } else {
            Write-Host "Dialog detected but its 'Yes' button wasn't found." -ForegroundColor Yellow
        }
    }
}

# EXPERIMENTAL - first attempt at UI Automation's real event model (WindowOpenedEvent), as an
# alternative to the repeated FindFirst polling every dialog-detection spot in this toolkit
# currently uses. Scoped to AutomationElement.RootElement (the whole desktop), filtering by
# ProcessId inside the callback itself - this is what lets it also catch a dialog that appears
# BEFORE MainWindowHandle exists (the startup upgrade prompt), since there's no "this process's own
# window" root to scope to yet at that point.
function Start-WindowEventWatch {
    param([Parameter(Mandatory = $true)][int]$ProcessId)

    $queue = [System.Collections.Concurrent.ConcurrentQueue[object]]::new()

    # .GetNewClosure() captures $queue/$ProcessId AT DEFINITION TIME: the callback runs on a
    # UI-Automation-owned thread, not this script's own, so it can't rely on resolving these against
    # whatever scope happens to be active when it actually fires.
    $handler = {
        param($sender, $e)
        try {
            if ($sender.Current.ProcessId -eq $ProcessId) {
                # Only simple, already-cached properties read here, and only a plain data object
                # enqueued - NOT the raw $sender element itself, to avoid any risk of touching a UI
                # Automation COM object from the wrong thread later. A caller that needs to actually
                # interact with the dialog does a normal FindFirst afterward to get a real reference.
                $queue.Enqueue([PSCustomObject]@{
                    Name        = $sender.Current.Name
                    ControlType = $sender.Current.ControlType.ProgrammaticName
                })
            }
        } catch {
            # Deliberately minimal and defensive - anything beyond reading a couple of basic
            # properties is avoided entirely in a cross-thread callback like this.
        }
    }.GetNewClosure()

    $delegate = [System.Windows.Automation.AutomationEventHandler]$handler
    [System.Windows.Automation.Automation]::AddAutomationEventHandler(
        [System.Windows.Automation.WindowPattern]::WindowOpenedEvent,
        [System.Windows.Automation.AutomationElement]::RootElement,
        [System.Windows.Automation.TreeScope]::Subtree,
        $delegate
    )

    [PSCustomObject]@{
        Queue     = $queue
        Delegate  = $delegate
        ProcessId = $ProcessId
    }
}

# Pops one event from the queue, waiting up to TimeoutSeconds if it's empty (0 = check once, don't
# wait at all). Returns $null if nothing showed up in time.
function Get-WatchedWindowEvent {
    param(
        [Parameter(Mandatory = $true)]$Watch,
        [int]$TimeoutSeconds = 0
    )
    $deadline = (Get-Date).AddSeconds($TimeoutSeconds)
    $result = $null
    do {
        if ($Watch.Queue.TryDequeue([ref]$result)) { return $result }
        if ($TimeoutSeconds -gt 0) { Start-Sleep -Milliseconds 200 }
    } while ($TimeoutSeconds -gt 0 -and (Get-Date) -lt $deadline)
    $null
}

function Stop-WindowEventWatch {
    param([Parameter(Mandatory = $true)]$Watch)
    try {
        [System.Windows.Automation.Automation]::RemoveAutomationEventHandler(
            [System.Windows.Automation.WindowPattern]::WindowOpenedEvent,
            [System.Windows.Automation.AutomationElement]::RootElement,
            $Watch.Delegate
        )
    } catch {
        # Best-effort - if this fails, the handler may linger for the rest of the PowerShell
        # session, same class of risk as other "did this really clean up" concerns elsewhere here.
    }
}

function Wait-ForStagedCopy {
    param([int]$TimeoutSeconds = 30)
    $deadline = (Get-Date).AddSeconds($TimeoutSeconds)
    $copies = @()
    while ((Get-Date) -lt $deadline) {
        $copies = Get-StagedCopies
        if ($copies.Count -gt 0) { break }
        Start-Sleep -Milliseconds 300
    }
    $copies
}
