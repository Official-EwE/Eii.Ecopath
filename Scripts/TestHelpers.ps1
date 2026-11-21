<#
.SYNOPSIS
    Shared test framework (pass/fail tracking, summary) and UI-automation helpers, dot-sourced by
    every Run-*.ps1 test runner in this toolkit. Consolidated here after several runners started
    duplicating the same Win32 interop and element-finding code.

.EXAMPLE
    . (Join-Path $PSScriptRoot "TestHelpers.ps1")
#>

Add-Type -AssemblyName UIAutomationClient, UIAutomationTypes, System.Windows.Forms, System.Drawing

# ---------------------------------------------------------------------------------------------
# Elevation. The tests drive the app with real mouse/keyboard input and UI Automation, and kill the
# processes they start. Windows blocks both across privilege levels (UIPI; "Access is denied" when
# killing an elevated process from a normal prompt), and a process left behind by an elevated run
# can then never be cleaned up by a normal one - it locks DLLs and fails the next build. So every
# run uses ONE level, and that level is elevated: a non-elevated start relaunches itself elevated
# (a new window - the UAC prompt appears once; the window stays open when the run ends).
# Set the environment variable EWE_TEST_NO_ELEVATE=1 to skip this (e.g. a CI agent that is already
# in a suitable session, or when deliberately running everything non-elevated).
function Test-IsElevated {
    $principal = New-Object System.Security.Principal.WindowsPrincipal([System.Security.Principal.WindowsIdentity]::GetCurrent())
    $principal.IsInRole([System.Security.Principal.WindowsBuiltInRole]::Administrator)
}

# Relaunches the calling runner script elevated, with its own bound parameters, then exits this
# (non-elevated) run. The caller is whichever script dot-sourced this file: dot-sourcing shares
# scope, so $PSBoundParameters read here are that runner's own.
function Request-ElevationIfNeeded {
    # EWE_TEST_ELEVATION_DONE is inherited by every child script/process, so only the FIRST (top-level)
    # runner ever decides about elevation. Without it, each script a runner invokes (Start-TestInstance.ps1
    # etc.) dot-sources this file again and would open yet another elevated window - and its `exit`
    # would end the child script while the runner carried on with no instance.
    if ($env:EWE_TEST_NO_ELEVATE -eq "1" -or $env:EWE_TEST_ELEVATION_DONE -eq "1") { return }
    $env:EWE_TEST_ELEVATION_DONE = "1"
    if (Test-IsElevated) { return }
    # Already relaunched once in this session (a nested runner got here after the outer one did): never twice.
    if ($global:EwEElevationRelaunched) { throw "This run was relaunched elevated in another window; aborting this non-elevated copy." }

    # The OUTERMOST script on the call stack is the one to relaunch (e.g. Run-AllTests.ps1, not each
    # runner it calls) - relaunching a nested runner would open one window per runner while the outer
    # non-elevated script carried on. Its own bound parameters come from the call stack frame.
    $frame = Get-PSCallStack | Where-Object { $_.ScriptName -and $_.ScriptName -ne $PSCommandPath } | Select-Object -Last 1
    if (-not $frame) { return }   # not started from a script file: nothing to relaunch
    $callerScript = $frame.ScriptName
    $bound = $frame.InvocationInfo.BoundParameters

    $argList = @("-NoExit", "-ExecutionPolicy", "Bypass", "-File", "`"$callerScript`"")
    if ($bound) {
        foreach ($key in $bound.Keys) {
            $value = $bound[$key]
            if ($value -is [System.Management.Automation.SwitchParameter]) {
                if ($value.IsPresent) { $argList += "-$key" }
            } elseif ($value -is [array]) {
                $argList += "-$key"
                $argList += ($value | ForEach-Object { "`"$_`"" }) -join ","
            } else {
                $argList += "-$key"
                $argList += "`"$value`""
            }
        }
    }

    Write-Host "Not elevated - relaunching '$([System.IO.Path]::GetFileName($callerScript))' as administrator in a new window (confirm the UAC prompt)." -ForegroundColor Yellow
    $host_exe = (Get-Process -Id $PID).Path
    try {
        Start-Process -FilePath $host_exe -Verb RunAs -WorkingDirectory (Get-Location).Path -ArgumentList $argList -ErrorAction Stop
    } catch {
        Write-Host "Could not elevate ($($_.Exception.Message)). Continuing NON-elevated; set EWE_TEST_NO_ELEVATE=1 to silence this." -ForegroundColor Red
        return
    }
    $global:EwEElevationRelaunched = $true
    throw "Relaunched elevated in a new window; this non-elevated run stops here."
}
Request-ElevationIfNeeded


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

# Called from WITHIN a test body, right after an app instance becomes idle/ready, to have
# Invoke-Test automatically verify no new dialog appeared by the time the test body finishes - the
# test itself only needs to say WHICH process and WHAT baseline, not perform the check itself. Can
# be called more than once per test (e.g. after a mid-test restart) - each registration is checked
# independently, and one whose process has since exited/been replaced is skipped harmlessly rather
# than treated as an error, since a deliberate restart is not a "leftover" in the sense this cares
# about.
function Register-DialogCheck {
    param(
        [Parameter(Mandatory = $true)][int]$ProcessId,
        [Parameter(Mandatory = $true)][long[]]$Baseline
    )
    $script:PendingDialogChecks.Add([PSCustomObject]@{ ProcessId = $ProcessId; Baseline = $Baseline })
}

function Invoke-Test {
    param(
        [Parameter(Mandatory = $true)][string]$Name,
        # Named $TestBody, NOT $Test: CONFIRMED root cause of the "call depth overflow" bug - a
        # caller like "Invoke-Test $Name { & $Test $modelPath }" passes a plain, non-closure
        # scriptblock whose own "$Test" is resolved dynamically against the CALLING scope at
        # invocation time, not captured lexically where it was written. Once that scriptblock was
        # bound to this function's OWN parameter (which used to be named $Test too), invoking it
        # via "& $Test" below made its inner "$Test" resolve to itself - a single scriptblock
        # calling itself forever, which is exactly what the repeated identical
        # "Run-ConversionTests.ps1: line 80" stack frame showed. Renaming this parameter removes
        # the collision entirely, regardless of what any caller happens to name its own variables.
        [Parameter(Mandatory = $true)][scriptblock]$TestBody
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
    $script:PendingDialogChecks = [System.Collections.Generic.List[object]]::new()
    $sw = [System.Diagnostics.Stopwatch]::StartNew()
    try {
        & $TestBody

        # Only run once the test body itself has already succeeded - if it already failed, that
        # failure is reported as-is, rather than risking this check's own result masking it.
        foreach ($check in $script:PendingDialogChecks) {
            $checkedProc = Get-Process -Id $check.ProcessId -ErrorAction SilentlyContinue
            if ($checkedProc -and -not $checkedProc.HasExited) {
                Assert-NoNewDialogs -ProcessId $check.ProcessId -Baseline $check.Baseline
            }
        }

        $sw.Stop()
        Add-TestResult -Name $Name -Result "PASS" -DurationMs $sw.ElapsedMilliseconds
        Write-Host "PASS ($($sw.ElapsedMilliseconds) ms)" -ForegroundColor Green
    } catch {
        $sw.Stop()
        Add-TestResult -Name $Name -Result "FAIL" -DurationMs $sw.ElapsedMilliseconds -Reason $_.Exception.Message
        Write-Host "FAIL ($($sw.ElapsedMilliseconds) ms): $($_.Exception.Message)" -ForegroundColor Red
        # Printed separately from the one-line summary above, and wrapped in its own try/catch:
        # for a genuine recursion bug (e.g. "call depth overflow") or any other real engine/COM
        # exception, ScriptStackTrace is exactly what pinpoints which function was calling which -
        # a one-line Exception.Message alone can't show that. But NOT printed for a plain
        # Assert-True failure (the common case - most test failures are just "expected X, got Y"):
        # its top frame is always "at Assert-True, ...", every single time, by construction - it
        # never says anything a reader couldn't already tell from the assertion message itself, so
        # printing it is pure noise for the single most common kind of failure there is. Some
        # terminating errors don't populate a stack trace at all, so this stays silent rather than
        # throwing a second, more confusing error on top of the real one.
        try {
            $isPlainAssertionFailure = $_.ScriptStackTrace -and ($_.ScriptStackTrace -split "`r?`n")[0].TrimStart() -like "at Assert-True,*"
            if ($_.ScriptStackTrace -and -not $isPlainAssertionFailure) {
                Write-Host "--- ScriptStackTrace ---" -ForegroundColor DarkYellow
                Write-Host $_.ScriptStackTrace -ForegroundColor DarkYellow
                Write-Host "------------------------" -ForegroundColor DarkYellow
            }
        } catch {
        }
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

    # @(...) forces an array even when exactly one result matches - see Assert-NoNewDialogs's own
    # comment on this same PowerShell gotcha. Without it, a run with exactly one pass/fail/skip (an
    # -Only run, most likely) would silently show 0 for that count instead of 1.
    $passed = @($script:TestResults | Where-Object { $_.Result -eq "PASS" }).Count
    $failed = @($script:TestResults | Where-Object { $_.Result -eq "FAIL" }).Count
    $skipped = @($script:TestResults | Where-Object { $_.Result -eq "SKIP" }).Count
    $total = $script:TestResults.Count

    $color = if ($failed -gt 0) { "Red" } else { "Green" }
    Write-Host "Total: $total   Passed: $passed   Failed: $failed   Skipped: $skipped" -ForegroundColor $color

    if ($failed -gt 0) {
        Write-Host "`nFailures:" -ForegroundColor Red
        $script:TestResults | Where-Object { $_.Result -eq "FAIL" } | ForEach-Object {
            Write-Host "  - $($_.Name): $($_.Reason)" -ForegroundColor Red
            Write-Host ""   # blank line between failures - reasons can span several lines
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
# Files only (-File): the legacy-import launcher's own "TempImport.<guid>" work FOLDERS live in the same
# folder and are a different mechanism (cLegacyImportLauncher), not staged model working copies.
function Get-StagedCopies {
    $stagingDir = Join-Path $env:LocalAppData "Ecopath with Ecosim\DBs"
    if (-not (Test-Path $stagingDir)) { return @() }
    Get-ChildItem -Path $stagingDir -Filter "Temp*" -File -ErrorAction SilentlyContinue | Where-Object { $_.Extension -ne ".ldb" }
}

# Waits for a just-started process's main window, handling the "your model database will be
# updated to a newer version" confirmation along the way if it appears - it can show up BEFORE
# MainWindowHandle is even populated, so a caller that only waits on that handle would hang with
# nobody clicking it. Shared by every launch site in this toolkit (Start-TestInstance.ps1 and
# Run-MultiInstanceTests.ps1's own second-instance launcher) after this gap showed up in one but not
# the other.
# Caption of the "your model database will be updated to a newer version" Yes/No prompt. Set in code
# (cCore: cFeedbackMessage.Caption <- CoreMessages.DATABASE_UPDATE_PROMPT_CAPTION) so it no longer
# shares the generic "Ecopath with Ecosim" caption with the main window, splash and other dialogs.
# Must match the English resx value exactly.
$DatabaseUpgradePromptCaption = "Update model database"

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
        (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::NameProperty, $DatabaseUpgradePromptCaption)),
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
                $upgradeDialogSeen = $true
                Write-Host "Database-upgrade prompt detected - clicking Yes..." -NoNewline
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
    # Child processes first (e.g. the legacy-import helper console the app launches): killing only the
    # parent leaves them running, and a leftover EwEImportHelper.exe keeps its DLLs locked, which
    # fails the next build of the helper.
    function Stop-ChildProcessTree {
        param([int]$ParentId)
        foreach ($child in @(Get-CimInstance Win32_Process -Filter "ParentProcessId=$ParentId" -ErrorAction SilentlyContinue)) {
            Stop-ChildProcessTree -ParentId $child.ProcessId
            Stop-Process -Id $child.ProcessId -Force -ErrorAction SilentlyContinue
        }
    }
    Stop-ChildProcessTree -ParentId $ProcessId
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
        (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::NameProperty, $DatabaseUpgradePromptCaption)),
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
# currently uses. Scoped to AutomationElement.RootElement (the whole desktop) - not filtered by
# ProcessId at registration time at all (CONFIRMED this matters: the caller doesn't know the target
# process's PID until AFTER Start-Process returns, and the dialog this is meant to catch can appear
# very early in startup - filtering at registration would mean either registering after the process
# already started, which can race the dialog's own appearance and silently miss it since a handler
# never catches an event retroactively, or needing the PID before it exists at all). Every
# window-opened event, from any process, is enqueued with its own ProcessId attached; the caller
# filters by PID when READING from the queue instead, by which point it's known.
function Start-WindowEventWatch {
    $queue = [System.Collections.Concurrent.ConcurrentQueue[object]]::new()

    # .GetNewClosure() captures $queue AT DEFINITION TIME: the callback runs on a UI-Automation-owned
    # thread, not this script's own, so it can't rely on resolving it against whatever scope happens
    # to be active when it actually fires.
    $handler = {
        param($sender, $e)
        try {
            # Only simple, already-cached properties read here, and only a plain data object
            # enqueued - NOT the raw $sender element itself, to avoid any risk of touching a UI
            # Automation COM object from the wrong thread later. A caller that needs to actually
            # interact with the dialog does a normal FindFirst afterward to get a real reference.
            $queue.Enqueue([PSCustomObject]@{
                Name        = $sender.Current.Name
                ControlType = $sender.Current.ControlType.ProgrammaticName
                ProcessId   = $sender.Current.ProcessId
            })
        } catch {
            # Deliberately minimal and defensive - anything beyond reading a couple of basic
            # properties is avoided entirely in a cross-thread callback like this.
        }
    }.GetNewClosure()

    $delegate = [System.Windows.Automation.AutomationEventHandler]$handler
    try {
        [System.Windows.Automation.Automation]::AddAutomationEventHandler(
            [System.Windows.Automation.WindowPattern]::WindowOpenedEvent,
            [System.Windows.Automation.AutomationElement]::RootElement,
            [System.Windows.Automation.TreeScope]::Subtree,
            $delegate
        )
        Write-Host "  (AddAutomationEventHandler registered without throwing.)" -ForegroundColor DarkGray
    } catch {
        Write-Host "  (AddAutomationEventHandler THREW: $($_.Exception.Message))" -ForegroundColor Red
        throw
    }

    [PSCustomObject]@{
        Queue    = $queue
        Delegate = $delegate
    }
}

# Pops one event from the queue matching ProcessId (if given - omit to accept any process's event),
# waiting up to TimeoutSeconds if nothing matches yet (0 = check once, don't wait at all). Discards
# non-matching events permanently rather than re-queuing them - fine for a single watch dedicated to
# one process for its whole lifetime, which is the only way this is used so far. Returns $null if
# nothing matched in time.
function Get-WatchedWindowEvent {
    param(
        [Parameter(Mandatory = $true)]$Watch,
        [int]$TimeoutSeconds = 0,
        [Nullable[int]]$ProcessId = $null
    )
    $deadline = (Get-Date).AddSeconds($TimeoutSeconds)
    do {
        $result = $null
        while ($Watch.Queue.TryDequeue([ref]$result)) {
            if ($null -eq $ProcessId -or $result.ProcessId -eq $ProcessId) { return $result }
        }
        if ($TimeoutSeconds -gt 0) {
            # UNCONFIRMED HYPOTHESIS being tested here: UI Automation's event delivery relies on COM
            # callbacks being pumped through an active Windows message loop - a plain console script
            # has none running at all, which could mean events are genuinely being raised but never
            # actually delivered to our callback. DoEvents() explicitly pumps pending messages each
            # iteration, which a normal WinForms/WPF app gets for free just by having its own loop
            # running, but a bare PowerShell script does not.
            [System.Windows.Forms.Application]::DoEvents()
            Start-Sleep -Milliseconds 200
        }
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

# Finds/builds Tools\DialogChecker\DialogChecker.exe - rebuilds whenever Program.cs is newer than
# the existing exe (or the exe is missing entirely), not just once per session via a cached path.
# CONFIRMED this matters: an existence-only check happily kept reusing a stale build from an earlier
# session after Program.cs changed (still showing the exe's OLD usage message, from before a new
# command-line argument was added) - a cheap timestamp comparison catches that every time, at
# negligible cost given this project builds in ~2 seconds.
function Get-DialogCheckerExe {
    $toolRoot = Join-Path (Split-Path -Parent $PSScriptRoot) "Tools\DialogChecker"
    $exePath = Join-Path $toolRoot "bin\Debug\net48\DialogChecker.exe"
    $sourcePath = Join-Path $toolRoot "Program.cs"

    $needsBuild = (-not (Test-Path $exePath)) -or
                  ((Get-Item $sourcePath).LastWriteTime -gt (Get-Item $exePath).LastWriteTime)

    if ($needsBuild) {
        Write-Host "  Building DialogChecker.exe (missing, or Program.cs changed since the last build)..." -ForegroundColor DarkGray
        $csproj = Join-Path $toolRoot "DialogChecker.csproj"
        dotnet build $csproj -c Debug | ForEach-Object { Write-Host "    $_" -ForegroundColor DarkGray }
        if ($LASTEXITCODE -ne 0) { throw "Failed to build DialogChecker.exe (dotnet build exit code $LASTEXITCODE)." }
    }
    if (-not (Test-Path $exePath)) { throw "DialogChecker.exe still not found at: $exePath" }
    $exePath
}

# Returns every TOP-LEVEL window currently open for a given process, via the small standalone
# DialogChecker.exe tool rather than PowerShell's own UI Automation interop - a plain, synchronous
# call-and-respond check, deliberately not an event watcher (see DialogChecker's own Program.cs
# comment for why that approach didn't pan out).
function Get-OpenWindows {
    param([Parameter(Mandatory = $true)][int]$ProcessId)
    $exePath = Get-DialogCheckerExe
    # stderr captured (NOT 2>$null) and included in the thrown message on failure - CONFIRMED a
    # swallowed stderr once gave a completely uninformative "exit code 2" with no way to tell what
    # had actually gone wrong inside the tool.
    $stderrFile = [System.IO.Path]::GetTempFileName()
    try {
        $output = & $exePath $ProcessId 2>$stderrFile
        if ($LASTEXITCODE -ne 0) {
            $stderrText = Get-Content $stderrFile -Raw -ErrorAction SilentlyContinue
            throw "DialogChecker.exe failed (exit code $LASTEXITCODE): $stderrText"
        }
    } finally {
        Remove-Item $stderrFile -Force -ErrorAction SilentlyContinue
    }
    $output | Where-Object { $_ } | ForEach-Object {
        $parts = $_ -split "`t"
        [PSCustomObject]@{
            Name               = $parts[0]
            ControlType        = $parts[1]
            NativeWindowHandle = [int64]$parts[2]
            X                  = [double]$parts[3]
            Y                  = [double]$parts[4]
            Width              = [double]$parts[5]
            Height             = [double]$parts[6]
            IsOffscreen        = [bool]::Parse($parts[7])
            TextContent        = if ($parts.Count -gt 8) { $parts[8] } else { "" }
        }
    }
}

# Snapshots every window handle currently open for a process - meant to be called once, right after
# the app is confirmed idle (freshly launched, or between tests), as the baseline Assert-NoNewDialogs
# later compares against.
#
# Polls for the window set to actually SETTLE (two consecutive snapshots matching), rather than
# trusting a single snapshot taken immediately after launch/restart: CONFIRMED that
# Start-InstanceOnExistingModel/Start-TestInstance.ps1 only wait for the MAIN window to appear, not
# for every normal sub-panel (like "Basic input") to finish initializing as its own separate window a
# moment later - a single-snapshot baseline taken right then can miss completely ordinary app chrome
# that simply hadn't registered yet, which then gets wrongly flagged as "new" later.
function Get-WindowBaseline {
    param(
        [Parameter(Mandatory = $true)][int]$ProcessId,
        [int]$TimeoutSeconds = 5
    )
    $deadline = (Get-Date).AddSeconds($TimeoutSeconds)
    $previous = $null
    while ((Get-Date) -lt $deadline) {
        $current = @(Get-OpenWindows -ProcessId $ProcessId | ForEach-Object { $_.NativeWindowHandle } | Sort-Object)
        if ($null -ne $previous -and @(Compare-Object $previous $current).Count -eq 0) {
            return $current
        }
        $previous = $current
        Start-Sleep -Milliseconds 400
    }
    # Didn't settle within the timeout - return the last snapshot anyway rather than failing outright;
    # a baseline that's still slightly stale is better than none, and a genuinely unstable app would
    # show up as repeated Assert-NoNewDialogs failures elsewhere, not a hidden failure here.
    $previous
}

# Fails loudly if any window NEW since a baseline (from Get-WindowBaseline) is currently open - meant
# to run right before moving on from a test/step, to catch a leftover dialog that would otherwise
# silently carry over and potentially confuse whatever runs next.
#
# Deliberately NOT "anything besides the main window": CONFIRMED this app's own normal UI already
# contains many elements with their own distinct native window handles that are NOT dialogs at all -
# "Basic input" (itself ControlType.Window), "Navigator", toolbars, and multiple anonymous panes are
# all ordinary, always-present parts of the main window, not unexpected popups. Comparing against a
# baseline taken while the app was known to be idle avoids flagging any of this normal chrome, while
# still catching anything genuinely new.
function Assert-NoNewDialogs {
    param(
        [Parameter(Mandatory = $true)][int]$ProcessId,
        [Parameter(Mandatory = $true)][long[]]$Baseline
    )
    $windows = Get-OpenWindows -ProcessId $ProcessId
    # @(...) forces an array even when exactly one item matches - CONFIRMED PowerShell otherwise
    # unwraps a single-match Where-Object result into a bare scalar, which has no .Count property at
    # all (silently $null, not an error), making ".Count -gt 0" false even with a real match - this
    # exact gap was why a single genuinely-unexpected window (nothing else) went undetected.
    $unexpected = @($windows | Where-Object { $_.NativeWindowHandle -notin $Baseline })
    if ($unexpected.Count -gt 0) {
        $details = ($unexpected | ForEach-Object {
            $contentPart = if ($_.TextContent) { " - '$($_.TextContent)'" } else { "" }
            "'$($_.Name)' ($($_.ControlType))$contentPart"
        }) -join "; "
        throw "Unexpected NEW window(s) appeared since the baseline: $details"
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

# ===========================================================================
# Conversion-test helpers (legacy Ecopath 5 model detection/import flow)
# ===========================================================================

# Opens a model via the File menu, same navigation as Open-ModelViaUI, but WITHOUT that function's
# own built-in wait for the generic "database will be updated" upgrade dialog at the end: the
# conversion tests need to wait for a DIFFERENT, and not-yet-fixed, set of possible windows instead
# (the EwE5-referral message, the import wizard, or the x64 helper's console) - baked-in waiting for
# the wrong dialog here would just cost 5 real seconds on every single conversion test for nothing.
function Open-LegacyModelViaUI {
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
}

# Polls until a window NOT present in Baseline (from Get-WindowBaseline) appears, or times out.
# Deliberately generic (no assumption about which of the three conversion-flow outcomes - the
# EwE5-referral message, the import wizard, or the x64 helper's console - is about to show up):
# the caller inspects the returned record's Name/ControlType/TextContent to tell them apart.
# Returns $null on timeout rather than throwing, so the caller can decide what "nothing appeared
# at all" means for its own test (usually itself a failure, but worth a distinct message from
# "the wrong window appeared").
function Wait-ForNewWindow {
    param(
        [Parameter(Mandatory = $true)][int]$ProcessId,
        [Parameter(Mandatory = $true)][long[]]$Baseline,
        [int]$TimeoutSeconds = 30
    )
    $deadline = (Get-Date).AddSeconds($TimeoutSeconds)
    while ((Get-Date) -lt $deadline) {
        $current = Get-OpenWindows -ProcessId $ProcessId
        $newOnes = @($current | Where-Object { $_.NativeWindowHandle -notin $Baseline })
        if ($newOnes.Count -gt 0) {
            # Prefer a genuinely top-level-looking window (reasonable size) over an incidental
            # nested pane that might also be new - same spirit as DialogChecker's own
            # ControlType.Window filtering, applied here to PICK one when several are new at once.
            return @($newOnes | Sort-Object { $_.Width * $_.Height } -Descending)[0]
        }
        Start-Sleep -Milliseconds 300
    }
    $null
}

# Polls for a window with an EXACT Name, for a process, with no baseline involved at all - unlike
# Wait-ForNewWindow, which only works when there's a safe moment to snapshot "before" state first.
# Needed for a model passed directly on the command line (e.g. a legacy Ecopath 5 .MDB opened via
# -SourceModelPath): its response window (the import helper's console, say) can already be open by
# the time Start-TestInstance.ps1 even returns - there's no idle moment left to baseline against
# before it appears, so a diff-based wait would race it and could miss it outright.
function Wait-ForWindowByName {
    param(
        [Parameter(Mandatory = $true)][int]$ProcessId,
        [Parameter(Mandatory = $true)][string]$Name,
        [int]$TimeoutSeconds = 30
    )
    $deadline = (Get-Date).AddSeconds($TimeoutSeconds)
    while ((Get-Date) -lt $deadline) {
        $current = Get-OpenWindows -ProcessId $ProcessId
        $match = @($current | Where-Object { $_.Name -eq $Name })
        if ($match.Count -gt 0) { return $match[0] }
        Start-Sleep -Milliseconds 300
    }
    $null
}

# Live AutomationElement for a window already known by its NativeWindowHandle (e.g. from
# Wait-ForNewWindow/Get-OpenWindows) - same FromHandle technique Get-AppWindow already uses for the
# main window, just generalized to any handle. PowerShell's own UIA client talks directly to the
# real window from here; DialogChecker.exe is only ever used for the generic
# has-anything-new-appeared check, never for the actual interaction.
function Get-WindowElementByHandle {
    param([Parameter(Mandatory = $true)][long]$Handle)
    [System.Windows.Automation.AutomationElement]::FromHandle([IntPtr]$Handle)
}

# Reads the live Value of the first Edit-control descendant of a window element - the import
# helper's console keeps its whole scrollback there (confirmed via Accessibility Insights
# screenshot: ControlType.Edit(50004), not a Text element), not in any element's Name.
function Get-EditControlValue {
    param([Parameter(Mandatory = $true)]$WindowElement)

    # Diagnostic trail for this ONE call, read by Wait-ForEditControlText's own periodic status
    # line - not returned as part of the normal string result, so every existing caller is
    # unaffected, but it means the NEXT run shows exactly which path actually fired instead of just
    # an empty/non-empty string with no way to tell why.
    $script:LastEditControlValueDiagnostic = "not run yet"

    # Preferred path: a genuine ControlType.Edit exposing a working, NON-EMPTY ValuePattern.
    # Deliberately requires non-empty before returning - CONFIRMED (the live test run still showing
    # length=0 for 24+ seconds straight, well past any plausible startup race) that this app can
    # have SOME other, unrelated, genuinely empty ControlType.Edit element elsewhere in the window
    # (not the log console at all) that matches this condition and supports ValuePattern just
    # fine - returning ITS (always-empty) value immediately, without ever reaching the fallback
    # below that would have found the real one. An empty value here is therefore NOT trusted as
    # final - treated the same as "nothing found", falling through instead.
    $editCondition = New-Object System.Windows.Automation.PropertyCondition(
        [System.Windows.Automation.AutomationElement]::ControlTypeProperty, [System.Windows.Automation.ControlType]::Edit)
    # ALL matching Edit elements are considered, and the LONGEST non-empty value wins: some windows
    # (e.g. the legacy-import console while it asks "Please enter the model number and press
    # Enter") hold BOTH the big log and a small separate input box, and the log is always the longer.
    $bestValue = ""
    try {
        foreach ($editElement in $WindowElement.FindAll([System.Windows.Automation.TreeScope]::Descendants, $editCondition)) {
            try {
                $viaValue = $editElement.GetCurrentPattern([System.Windows.Automation.ValuePattern]::Pattern).Current.Value
                if ($viaValue.Length -gt $bestValue.Length) { $bestValue = $viaValue }
            } catch { }
        }
    } catch { }
    if (-not [string]::IsNullOrEmpty($bestValue)) {
        $script:LastEditControlValueDiagnostic = "via ControlType.Edit + ValuePattern (non-empty, longest)"
        return $bestValue
    }

    # Fallback: CONFIRMED via Debug-EditControlText.ps1's own dump that this app's import-log
    # console is NOT exposed to PowerShell's System.Windows.Automation client as ControlType.Edit
    # with a working ValuePattern at all - unlike what Accessibility Insights' own (different) UIA
    # client stack showed. From here it shows up as plain ControlType.Pane, whose underlying Win32
    # window class name contains "EDIT" (WindowsForms10.EDIT.*), with the control's ENTIRE text
    # sitting directly in its own Name property instead of behind any pattern. Matched by
    # ClassName, not by searching for known log text (that would be circular - the log text IS the
    # Name here), and deliberately not scoped to any one ControlType, since ControlType.Edit is
    # exactly what this control turned out NOT to report as. Also requires non-empty before
    # returning, for the same reason as above - and keeps scanning past an empty-Name match rather
    # than stopping at the first ClassName hit, in case more than one element matches.
    try {
        $all = $WindowElement.FindAll([System.Windows.Automation.TreeScope]::Descendants, [System.Windows.Automation.Condition]::TrueCondition)
        $classNameMatches = 0
        $bestName = ""
        foreach ($el in $all) {
            $className = try { $el.Current.ClassName } catch { "" }
            if ($className -like "*EDIT*") {
                $classNameMatches++
                $viaName = try { $el.Current.Name } catch { "" }
                # Longest wins (see the primary path's note): the separate small input box is named
                # after its prompt, while the log's Name is the whole log text.
                if ($viaName.Length -gt $bestName.Length) { $bestName = $viaName }
            }
        }
        if (-not [string]::IsNullOrEmpty($bestName)) {
            $script:LastEditControlValueDiagnostic = "via ClassName-contains-EDIT fallback ($classNameMatches match(es), longest non-empty)"
            return $bestName
        }
        $script:LastEditControlValueDiagnostic = "found $($all.Count) descendant(s), $classNameMatches ClassName-EDIT match(es), none had non-empty text"
    } catch {
        $script:LastEditControlValueDiagnostic = "FindAll(Descendants) threw: $($_.Exception.Message)"
    }
    ""
}

# Polls a log-console window's Edit value until it contains one of the given marker substrings
# (e.g. "Import completed successfully" / "Import failed"), or times out. Returns the final text
# either way - the caller decides what "timed out, here's what it said" means for its own test.
function Wait-ForEditControlText {
    param(
        [Parameter(Mandatory = $true)]$WindowElement,
        [Parameter(Mandatory = $true)][string[]]$ContainsAnyOf,
        [int]$TimeoutSeconds = 60
    )
    $deadline = (Get-Date).AddSeconds($TimeoutSeconds)
    $text = ""
    # Printed every ~3s (not every 500ms poll - that would spam), so a run that stalls here shows
    # LIVE, in the normal test output, without needing a separate debug script afterward: whether
    # Get-EditControlValue is returning nothing at all (length staying 0 the whole time - the
    # retrieval itself is still broken), returning text that's growing normally but never matching
    # any marker (a marker-text mismatch, not a retrieval problem), or genuinely not changing after
    # some point (the import itself may be stuck, not this script).
    $lastPrint = Get-Date
    while ((Get-Date) -lt $deadline) {
        $text = Get-EditControlValue -WindowElement $WindowElement
        foreach ($marker in $ContainsAnyOf) {
            if ($text -like "*$marker*") { return $text }
        }
        if (((Get-Date) - $lastPrint).TotalSeconds -ge 3) {
            $tail = if ($text.Length -gt 0) { $text.Substring([Math]::Max(0, $text.Length - 60)).Replace("`r", "").Replace("`n", " / ") } else { "<empty>" }
            Write-Host "`n    [waiting] text length=$($text.Length)  tail: $tail  ($script:LastEditControlValueDiagnostic)" -NoNewline -ForegroundColor DarkGray
            $lastPrint = Get-Date
        }
        Start-Sleep -Milliseconds 500
    }
    $text
}

# Finds and clicks a button by its exact Name within a window element - thin wrapper over the same
# Find-ByNameAndType + Invoke-ElementClick pattern used throughout this file, named explicitly for
# conversion-test call sites (clicking "Continue" on the import console, "OK" on a message dialog).
# Finds a button by its exact Name: a genuine Button first, else - because PowerShell's UIA client
# often exposes a WinForms button as a Pane (CONFIRMED: the import console's Continue and the
# wizard's OK are both Pane name='...' class='WindowsForms10.BUTTON...') - any element with that
# Name whose ClassName contains BUTTON. Returns $null when there is none.
function Find-ButtonByName {
    param($Root, [string]$Name)
    $button = Find-ByNameAndType -Root $Root -Name $Name -ControlType ([System.Windows.Automation.ControlType]::Button)
    if ($null -ne $button) { return $button }
    $nameCondition = New-Object System.Windows.Automation.PropertyCondition(
        [System.Windows.Automation.AutomationElement]::NameProperty, $Name)
    foreach ($candidate in $Root.FindAll([System.Windows.Automation.TreeScope]::Descendants, $nameCondition)) {
        $candidateClass = try { $candidate.Current.ClassName } catch { "" }
        if ($candidateClass -like "*BUTTON*") { return $candidate }
    }
    $null
}

function Invoke-ButtonByName {
    param(
        [Parameter(Mandatory = $true)]$WindowElement,
        [Parameter(Mandatory = $true)][string]$Name
    )
    $button = Find-ButtonByName -Root $WindowElement -Name $Name
    if ($null -eq $button) { throw "Could not find a '$Name' button in this window." }
    Invoke-ElementClick -Element $button
}
