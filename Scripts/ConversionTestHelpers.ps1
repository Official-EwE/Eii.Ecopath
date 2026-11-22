$script:ConversionRepoRoot = Split-Path -Parent $PSScriptRoot

# ---------------------------------------------------------------------------------------------
# The x86 import helper: build (cheap if up to date - this also picks up its `models` mode), locate.
# ---------------------------------------------------------------------------------------------
function Get-ImportHelperExe {
    if ($script:ImportHelperExe -and (Test-Path $script:ImportHelperExe)) { return $script:ImportHelperExe }
    $proj = Join-Path $script:ConversionRepoRoot "Tools\EwEImportHelper\Source\EwEImportHelper.vbproj"
    if (-not (Test-Path $proj)) { throw "Project file not found: $proj" }
    # Same once-per-session table as Start-TestInstance.ps1 (new PowerShell session = rebuild).
    if (-not $global:EwEBuiltProjects) { $global:EwEBuiltProjects = @{} }
    $buildKey = "$proj|Debug|x86".ToLowerInvariant()
    if (-not $global:EwEBuiltProjects[$buildKey]) {
        # A helper process left over from an earlier (killed) run keeps its DLLs locked and fails the build.
        foreach ($leftover in @(Get-Process -Name "EwEImportHelper" -ErrorAction SilentlyContinue)) {
            $info = Get-CimInstance Win32_Process -Filter "ProcessId=$($leftover.Id)" -ErrorAction SilentlyContinue
            Write-Host "Stopping leftover EwEImportHelper (PID $($leftover.Id), parent PID $($info.ParentProcessId), started $($leftover.StartTime)) - it would lock the build..." -ForegroundColor Yellow
            & taskkill.exe /F /T /PID $leftover.Id 2>&1 | ForEach-Object { Write-Host "  $_" -ForegroundColor DarkGray }
            if (-not $leftover.WaitForExit(5000)) {
                Write-Host "  PID $($leftover.Id) is STILL running after a forced kill (elevated, or stuck?). Kill it by hand (Task Manager, or an elevated 'taskkill /F /PID $($leftover.Id)'), then run again." -ForegroundColor Red
            }
        }
        Write-Host "Building EwEImportHelper (Debug|x86)..."
        $buildOutput = & dotnet build $proj -c Debug -p:Platform=x86 --nologo -v q 2>&1
        if ($LASTEXITCODE -ne 0) {
            $buildOutput | ForEach-Object { Write-Host $_ }
            throw "Building EwEImportHelper failed (dotnet build exit code $LASTEXITCODE)."
        }
        $global:EwEBuiltProjects[$buildKey] = $true
    }
    $exe = Get-ChildItem -Path (Join-Path (Split-Path -Parent $proj) "bin") -Recurse -Filter "EwEImportHelper.exe" -File -ErrorAction SilentlyContinue |
        Sort-Object LastWriteTime -Descending | Select-Object -First 1
    if ($null -eq $exe) { throw "EwEImportHelper.exe not found under the project's bin folder after building." }
    $script:ImportHelperExe = $exe.FullName
    $script:ImportHelperExe
}

# Runs the helper hidden with output captured. Returns ExitCode, StdOut lines and StdErr text.
function Invoke-ImportHelperQuery {
    param(
        [Parameter(Mandatory = $true)][string]$HelperExe,
        [Parameter(Mandatory = $true)][string[]]$Arguments,
        [int]$TimeoutSeconds = 120
    )
    $psi = New-Object System.Diagnostics.ProcessStartInfo
    $psi.FileName = $HelperExe
    $psi.UseShellExecute = $false
    $psi.CreateNoWindow = $true
    $psi.RedirectStandardOutput = $true
    $psi.RedirectStandardError = $true
    # Quote each argument (paths contain spaces and commas).
    $psi.Arguments = ($Arguments | ForEach-Object { '"' + ($_ -replace '"', '\"') + '"' }) -join ' '
    $proc = [System.Diagnostics.Process]::Start($psi)
    # Read stderr asynchronously so a full pipe can't block the child while stdout is being read.
    $stderrTask = $proc.StandardError.ReadToEndAsync()
    $stdout = $proc.StandardOutput.ReadToEnd()
    if (-not $proc.WaitForExit($TimeoutSeconds * 1000)) {
        try { $proc.Kill() } catch { }
        return [PSCustomObject]@{ ExitCode = -1; StdOut = @(); StdErr = "timed out after $TimeoutSeconds seconds" }
    }
    [PSCustomObject]@{
        ExitCode = $proc.ExitCode
        StdOut   = @($stdout -split "`r?`n" | Where-Object { $_ -ne "" })
        StdErr   = $stderrTask.Result.Trim()
    }
}


# Number of models in a legacy .mdb, via the helper's non-interactive `models` mode. 0 if unknown.
function Get-LegacyModelCount {
    param([Parameter(Mandatory = $true)][string]$ModelPath)
    $result = Invoke-ImportHelperQuery -HelperExe (Get-ImportHelperExe) -Arguments @("models", $ModelPath)
    $count = 0
    if ($result.ExitCode -ne 0 -or $result.StdOut.Count -eq 0 -or -not [int]::TryParse($result.StdOut[0], [ref]$count)) { return 0 }
    $count
}

<#
.SYNOPSIS
    Shared building blocks for the legacy-model conversion test scripts (Run-ConversionHandlingTests.ps1
    and Run-ConversionTests.ps1). Dot-sourced AFTER TestHelpers.ps1; not meant to be run directly.

.DESCRIPTION
    Expects the dot-sourcing script to define these script-level variables (the functions read them
    from the caller's scope): $Platform, $MainWindowTimeoutSeconds and $startInstanceScript.
#>

# Launches the app with the legacy file DIRECTLY as its command-line argument (Start-TestInstance's
# own -SourceModelPath) rather than starting with a safe default model and opening the real target
# via the UI afterward - confirmed via manual testing that this is simpler and works fine: the app
# opens straight into the response window (e.g. the import helper's console) with no separate
# "open" step needed at all.
#
# No pre-open idle baseline is taken here, unlike Open-LegacyModelAndWait - there IS no safe idle
# moment to capture one: the response window can already be open by the time Start-TestInstance.ps1
# itself returns, since the import starts as part of normal startup, not as a separate later step.
# Wait-ForWindowByName (a plain name search, no "is this NEW" comparison) sidesteps that race
# entirely. The baseline used for the later Assert-NoNewDialogs check is captured only ONCE that
# window is confirmed present - deliberately INCLUDING it, not excluding it: closing it later isn't
# "new" relative to this baseline either way, while anything that shows up AFTER this point (like
# gulfmexico's follow-up "Import failed" error dialog, which only appears once the outcome is known)
# is correctly still caught as unexpected.
function Open-LegacyModelDirectly {
    param(
        [Parameter(Mandatory = $true)][string]$ModelPath,
        [string]$ExpectedWindowName = "Importing legacy Ecopath 5 model...",
        [int]$WaitTimeoutSeconds = 30
    )
    # No pre-launch cleanup of earlier conversion output needed: Start-TestInstance copies the model
    # into a fresh, uniquely-named EwE-TestRun\<random> folder every run, so the conversion output
    # always lands in an empty folder (and that folder is cleaned up afterwards).
    # A ScientificInterface / EwEImportHelper left over from an earlier test (any platform) can show
    # its own import window and confuse both the test and the user. Conversion tests are single-instance,
    # so anything still running here is a leftover: say so, and stop it.
    foreach ($leftover in @(Get-Process -Name "ScientificInterface","EwEImportHelper" -ErrorAction SilentlyContinue)) {
        Write-Host "  Stopping leftover $($leftover.ProcessName) (PID $($leftover.Id), $($leftover.Path))" -ForegroundColor Yellow
        & taskkill.exe /F /T /PID $leftover.Id 2>&1 | Out-Null
    }
    Write-Host "  Starting a disposable instance (model opened directly via the command line):"
    # -SkipMainWindowWait: CONFIRMED (user's own run) that the ordinary main-window wait never
    # reliably completes for this launch - the import console/wizard that appears instead isn't
    # recognized by .NET as the process's main window, so that wait just hard-times-out at 90s
    # doing nothing useful, and along the way can also false-positive-click an unidentified
    # same-titled "Yes" dialog that isn't actually the one it's meant for. We go straight to
    # waiting for the specific window we actually care about by name instead (below).
    $instance = & $startInstanceScript -Platform $Platform -SourceModelPath $ModelPath -MainWindowTimeoutSeconds $MainWindowTimeoutSeconds -SkipMainWindowWait
    Write-InstanceStarted -ProcessId $instance.Process.Id -Label "conversion test instance"

    $firstWindow = Write-Step "Waiting for the response window" {
        Wait-ForWindowByName -ProcessId $instance.Process.Id -Name $ExpectedWindowName -TimeoutSeconds $WaitTimeoutSeconds
    }
    if ($null -eq $firstWindow) {
        Stop-TestInstance -ProcessId $instance.Process.Id -Label "conversion test instance (no response window)"
        throw "No window named '$ExpectedWindowName' appeared within $WaitTimeoutSeconds seconds after launching directly with '$ModelPath'."
    }
    $baseline = Get-WindowBaseline -ProcessId $instance.Process.Id

    [PSCustomObject]@{ Instance = $instance; Baseline = $baseline; FirstWindow = $firstWindow }
}

# ===========================================================================
# Import-x64Helper - fully automated (well characterized from real screenshots: the console's
# title, its Edit-control log text, the "Continue" button, and the secondary "Import failed." OK
# dialog on failure are all directly confirmed, not guessed).
# ===========================================================================

# A conversion test using the x64 helper ALWAYS expects success - there is no "expected failure"
# variant. gulfmexico.MDB currently fails this (a real SalinityForcingShapeID import bug, not a
# test-harness issue) and that's exactly right: this test SHOULD fail until that bug is fixed, the
# same way any other regression test would. No special-casing of the specific error text, and no
# dismissing the follow-up "Import failed. See log file: ..." dialog that appears on failure - left
# alone, Assert-NoNewDialogs (called by every caller of this function, same as every other test in
# this suite) already catches it as an unexpected leftover window, which is a perfectly good reason
# for the test to fail on its own if the success assertion somehow didn't already catch it first.
function Test-HelperImportSucceeds {
    param([Parameter(Mandatory = $true)]$Setup)
    $console = $Setup.FirstWindow
    Assert-True ($console.Name -eq "Importing legacy Ecopath 5 model...") `
        "Expected the legacy-import helper's console window (title 'Importing legacy Ecopath 5 model...'), got '$($console.Name)' ($($console.ControlType))."

    $consoleElement = Get-WindowElementByHandle -Handle $console.NativeWindowHandle
    # "--- Process exited" (success or failure both end this way, confirmed via screenshot) is the
    # generic completion marker - just enough to know the console is actually done, not partway
    # through printing. Whether it succeeded is then judged on the exact success string alone.
    $finalText = Write-Step "Waiting for the import to finish" {
        Wait-ForEditControlText -WindowElement $consoleElement -ContainsAnyOf @("--- Process exited") -TimeoutSeconds 120
    }

    try {
        Write-Step "Dismissing the import console (Continue)" { Invoke-ButtonByName -WindowElement $consoleElement -Name "Continue" }
    } catch {
        # Diagnostic: what is actually in that window / on screen right now?
        Write-Host "`n    [diagnostic] Continue not found. Console window UIA tree:" -ForegroundColor Yellow
        try { Write-UiaTreeDump -Element $consoleElement } catch { Write-Host "    (window is gone: $($_.Exception.Message))" -ForegroundColor Yellow }
        throw
    }
    # Assert-NoNewDialogs (run by the caller right after this returns) does a single immediate
    # check with no settle time of its own - give the app a moment to actually finish whatever
    # clicking Continue kicks off (closing the console, bringing up its real main window) before
    # that check runs, so a transient loading window mid-transition isn't mistaken for something
    # unexpected.
    Start-Sleep -Milliseconds 1000

    # Only the last 5 lines, not the whole console log - the full text can run to hundreds of
    # lines for a large model (confirmed: 27000+ characters for gulfmexico), and the last few
    # lines are exactly where the real failure (or success) marker lives either way - the rest is
    # just noise once something has already failed.
    $finalLines = @($finalText -split "`r?`n" | Where-Object { $_ -ne "" })
    # Guarded explicitly, not left to the range operator alone: PowerShell's ".." counts DOWNWARD
    # when the end is less than the start (e.g. "0..-1" is two elements, 0 and -1 - and -1 as an
    # array INDEX means "last element", so an empty $finalLines would otherwise silently produce
    # garbage instead of an obviously-empty result.
    $lastLines = if ($finalLines.Count -eq 0) {
        "<empty>"
    } else {
        $finalLines[[Math]::Max(0, $finalLines.Count - 5)..($finalLines.Count - 1)] -join "`n"
    }
    Assert-True ($finalText -like "*Import completed successfully*") "Expected the import to succeed, got (last 5 lines):`n$lastLines"
}

# ===========================================================================
# TooOld - fully automated (characterized from real Accessibility Insights screenshots): a
# standard MessageBox-style dialog, titled "Ecopath with Ecosim" - genuinely ControlType.Text for
# the message (unlike the import console, no ClassName workaround needed here - this one exposes
# cleanly) and a genuine ControlType.Button named "OK" (ditto). No conversion is attempted at all
# for this tag family, so no output files to clean up beforehand.
#
# Title alone is NOT enough to confirm this is the right dialog: CONFIRMED (via the earlier
# Wait-ForMainWindow bug) that "Ecopath with Ecosim" is reused as this app's generic title across
# several different dialogs - so after finding a window with that name, the message TEXT is also
# checked against known marker phrases before treating it as the real thing and clicking OK.
# The dialog's own title, set in code (frmEwE6.SendMessage strTitle -> cMessage.Title; resource
# PROMPT_ERROR_IMPORT_EWE5_TOO_OLD_CAPTION, English text below). Previously it shared the generic
# "Ecopath with Ecosim" caption with the splash screen and main window, which made it impossible
# to tell apart by title - the splash was matched first. Must match the resx value exactly.
$ReferralDialogCaption = "Outdated Ecopath 5 database"

function Test-ReferralDialogAppears {
    param(
        [Parameter(Mandatory = $true)]$Setup,
        [Parameter(Mandatory = $true)][string[]]$ExpectedTextMarkers
    )
    $dialog = $Setup.FirstWindow
    Assert-True ($dialog.Name -eq $ReferralDialogCaption) `
        "Expected the EwE5-referral dialog (title '$ReferralDialogCaption'), got '$($dialog.Name)' ($($dialog.ControlType))."

    $dialogElement = Get-WindowElementByHandle -Handle $dialog.NativeWindowHandle
    # Collect the text of EVERY descendant with a non-empty Name (Text controls, but also the Pane /
    # link-label / edit forms PowerShell's UIA client exposes WinForms controls as - the referral
    # message carries a hyperlink, so its text control is not necessarily a plain Text element).
    $messageParts = @()
    try {
        foreach ($el in $dialogElement.FindAll([System.Windows.Automation.TreeScope]::Descendants, [System.Windows.Automation.Condition]::TrueCondition)) {
            $elName = try { $el.Current.Name } catch { "" }
            if (-not [string]::IsNullOrWhiteSpace($elName)) { $messageParts += $elName }
        }
    } catch { }
    $messageText = ($messageParts -join " ")

    $matchedMarker = $false
    foreach ($marker in $ExpectedTextMarkers) {
        if ($messageText -like "*$marker*") { $matchedMarker = $true; break }
    }
    if (-not $matchedMarker) {
        Write-Host "`n    [diagnostic] Referral dialog text not recognised. Dialog UIA tree:" -ForegroundColor Yellow
        Write-UiaTreeDump -Element $dialogElement
    }
    Assert-True $matchedMarker `
        "Expected the dialog's text to mention one of: $($ExpectedTextMarkers -join ', ') - the title matched but the content didn't match the one expected here. Got: '$messageText'"

    Write-Step "Dismissing the EwE5-referral dialog (OK)" { Invoke-ButtonByName -WindowElement $dialogElement -Name "OK" }
}

# Whole scenario for a file the app refers to EwE5 (TooOld): launch with the model on the command
# line, expect the "Outdated Ecopath 5 database" dialog, dismiss it, check for unexpected dialogs.
function Invoke-ReferralDialogTest {
    param(
        [Parameter(Mandatory = $true)][string]$ModelPath,
        [string]$Label = "EwE5-referral test instance"
    )
    $setup = Open-LegacyModelDirectly -ModelPath $ModelPath -ExpectedWindowName $ReferralDialogCaption
    try {
        Test-ReferralDialogAppears -Setup $setup -ExpectedTextMarkers @(
            "outdated version of Ecopath 5",
            "not supported by the EwE6 import process"
        )
        Assert-NoNewDialogs -ProcessId $setup.Instance.Process.Id -Baseline $setup.Baseline
    } finally {
        Stop-TestInstance -ProcessId $setup.Instance.Process.Id -Label $Label
    }
}

# ===========================================================================
# Import wizard (dlgImportDatabase, window title "Model conversion utility") - characterized from
# real Accessibility Insights screenshots. Flow: intro page -> Next -> "Select model(s) to import"
# page (a grid whose checkbox cells are NOT exposed individually) -> select the first model ->
# Next -> progress page (list + progress bar) -> OK becomes enabled when finished.
#
# The error case (e.g. the NoNullAllowedException "Import failed..." dialog) is deliberately NOT
# detected by caption here: while the conversion runs, the wizard is polled for NEW windows
# (Assert-NoNewDialogs against the baseline) and the test fails with that dialog's own text.

# Selects the model at -Index (0 = first, 1 = second, ...) in the wizard's grid. The checkbox itself isn't addressable, so (as
# confirmed manually) clicking the top-left corner of the grid's value pane toggles it. That pane is
# the first Pane sibling right after the grid's scroll bar in the UIA tree.

# Selects the model at -Index (0 = first, 1 = second, ...) in the wizard's grid. The checkbox itself isn't addressable, so (as
# confirmed manually) clicking the top-left corner of the grid's value pane toggles it. That pane is
# the first Pane sibling right after the grid's scroll bar in the UIA tree. Prints what it found and
# where it clicks; on any failure dumps the wizard's UIA tree so the real structure can be seen.
# For Index 0: one click on the first row's cell selects it. For Index > 0: that same click selects
# the first model, a second click (after the double-click interval has passed) deselects it again,
# then - with the grid still focused - Down moves to the next row and Space ticks it.
function Select-WizardModel {
    param(
        [Parameter(Mandatory = $true)]$WizardElement,
        [int]$Index = 0
    )
    # CONFIRMED via the tree dump: PowerShell's UIA client exposes the grid's scroll bar as a Pane
    # (not ControlType.ScrollBar), identifiable by ClassName "WindowsForms10.SCROLLBAR...".
    $paneCondition = New-Object System.Windows.Automation.PropertyCondition(
        [System.Windows.Automation.AutomationElement]::ControlTypeProperty, [System.Windows.Automation.ControlType]::Pane)
    $scrollBar = $null
    $deadline = (Get-Date).AddSeconds(10)
    while ($null -eq $scrollBar -and (Get-Date) -lt $deadline) {
        foreach ($candidate in $WizardElement.FindAll([System.Windows.Automation.TreeScope]::Descendants, $paneCondition)) {
            $candidateClass = try { $candidate.Current.ClassName } catch { "" }
            if ($candidateClass -like "*SCROLLBAR*") { $scrollBar = $candidate; break }
        }
        if ($null -eq $scrollBar) { Start-Sleep -Milliseconds 300 }
    }
    $sibling = $null
    if ($null -eq $scrollBar) {
        # A grid with a single model (e.g. Gironde) has nothing to scroll, so the wizard shows no scroll
        # bar at all. Fall back to the grid's value pane directly: the right-hand one of the grid's
        # equally tall, side-by-side Panes (name column left, value column right), i.e. the tall Pane
        # (100-400 px) with the largest X. Only meaningful for the first (only) model.
        if ($Index -gt 0) {
            Write-Host "`n    [diagnostic] No ScrollBar descendant found. Wizard UIA tree:" -ForegroundColor Yellow
            Write-UiaTreeDump -Element $WizardElement
            throw "Could not find the model grid (its scroll bar) on the wizard's model-selection page after ~10 seconds (UIA tree dumped above)."
        }
        $best = $null
        foreach ($candidate in $WizardElement.FindAll([System.Windows.Automation.TreeScope]::Descendants, $paneCondition)) {
            $r = try { $candidate.Current.BoundingRectangle } catch { continue }
            if ($r.Height -lt 100 -or $r.Height -gt 400 -or $r.Width -lt 100 -or $r.Width -ge 400) { continue }
            if ($null -eq $best -or $r.X -gt $best.Current.BoundingRectangle.X) { $best = $candidate }
        }
        if ($null -eq $best) {
            Write-Host "`n    [diagnostic] No scroll bar and no grid value pane found. Wizard UIA tree:" -ForegroundColor Yellow
            Write-UiaTreeDump -Element $WizardElement
            throw "Could not find the model grid on the wizard's model-selection page (UIA tree dumped above)."
        }
        Write-Host "`n    [diagnostic] No scroll bar (single-model grid); using the value pane at $($best.Current.BoundingRectangle)" -ForegroundColor DarkGray
        $sibling = $best
    } else {
        Write-Host "`n    [diagnostic] ScrollBar found, rect=$($scrollBar.Current.BoundingRectangle)" -ForegroundColor DarkGray

        $walker = [System.Windows.Automation.TreeWalker]::ControlViewWalker
        $parent = $walker.GetParent($scrollBar)
        Write-Host "    [diagnostic] Scroll bar's parent and its children:" -ForegroundColor DarkGray
        $child = $walker.GetFirstChild($parent)
        while ($null -ne $child) {
            $ct = try { $child.Current.ControlType.ProgrammaticName } catch { "?" }
            Write-Host "      $ct name='$($child.Current.Name)' class='$($child.Current.ClassName)' rect=$($child.Current.BoundingRectangle) focusable=$($child.Current.IsKeyboardFocusable)" -ForegroundColor DarkGray
            $child = $walker.GetNextSibling($child)
        }

        $sibling = $walker.GetNextSibling($scrollBar)
        while ($null -ne $sibling -and $sibling.Current.ControlType -ne [System.Windows.Automation.ControlType]::Pane) {
            $sibling = $walker.GetNextSibling($sibling)
        }
        if ($null -eq $sibling) {
            Write-Host "    [diagnostic] No Pane after the scroll bar. Wizard UIA tree:" -ForegroundColor Yellow
            Write-UiaTreeDump -Element $WizardElement
            throw "Could not find the grid's value pane next to its scroll bar (UIA tree dumped above)."
        }

    }

    $rect = $sibling.Current.BoundingRectangle
    $clickX = [int]($rect.X) + 5
    $clickY = [int]($rect.Y) + 5
    Write-Host "    [diagnostic] Chosen pane rect=$rect; clicking at ($clickX, $clickY)" -ForegroundColor DarkGray
    [System.Windows.Forms.Cursor]::Position = New-Object System.Drawing.Point($clickX, $clickY)
    Start-Sleep -Milliseconds 150
    $actual = [System.Windows.Forms.Cursor]::Position
    Write-Host "    [diagnostic] Cursor is now at ($($actual.X), $($actual.Y))" -ForegroundColor DarkGray
    Clear-ConsoleFromClickTarget
    [Win32TestRunner]::mouse_event([Win32TestRunner]::MOUSEEVENTF_LEFTDOWN, 0, 0, 0, [UIntPtr]::Zero)
    [Win32TestRunner]::mouse_event([Win32TestRunner]::MOUSEEVENTF_LEFTUP, 0, 0, 0, [UIntPtr]::Zero)
    Start-Sleep -Milliseconds 300

    if ($Index -gt 0) {
        # Wait out the system double-click interval so the second click is a separate click.
        Start-Sleep -Milliseconds 800
        Write-Host "    [diagnostic] Clicking again to deselect the first model, then Down x$Index + Space" -ForegroundColor DarkGray
        Clear-ConsoleFromClickTarget
        [Win32TestRunner]::mouse_event([Win32TestRunner]::MOUSEEVENTF_LEFTDOWN, 0, 0, 0, [UIntPtr]::Zero)
        [Win32TestRunner]::mouse_event([Win32TestRunner]::MOUSEEVENTF_LEFTUP, 0, 0, 0, [UIntPtr]::Zero)
        Start-Sleep -Milliseconds 300
        for ($i = 0; $i -lt $Index; $i++) {
            [System.Windows.Forms.SendKeys]::SendWait("{DOWN}")
            Start-Sleep -Milliseconds 200
        }
        [System.Windows.Forms.SendKeys]::SendWait(" ")
        Start-Sleep -Milliseconds 300
    }
}

# Progress percentage (0-100) from the wizard's progress bar, or $null if it can't be read.
function Get-WizardProgressPercent {
    param([Parameter(Mandatory = $true)]$WizardElement)
    try {
        $barCondition = New-Object System.Windows.Automation.PropertyCondition(
            [System.Windows.Automation.AutomationElement]::ControlTypeProperty, [System.Windows.Automation.ControlType]::ProgressBar)
        $bar = $WizardElement.FindFirst([System.Windows.Automation.TreeScope]::Descendants, $barCondition)
        if ($null -eq $bar) { return $null }
        try {
            $valueText = $bar.GetCurrentPattern([System.Windows.Automation.ValuePattern]::Pattern).Current.Value
            if ($valueText -match '(\d+(\.\d+)?)') { return [double]$Matches[1] }
        } catch { }
        try {
            $range = $bar.GetCurrentPattern([System.Windows.Automation.RangeValuePattern]::Pattern).Current
            if ($range.Maximum -gt $range.Minimum) {
                return 100.0 * ($range.Value - $range.Minimum) / ($range.Maximum - $range.Minimum)
            }
        } catch { }
    } catch { }
    $null
}

# Polls until the wizard's OK button is enabled (and progress, if readable, is 100%) - throwing
# immediately if any unexpected new window (e.g. an import-failure dialog) shows up meanwhile.
function Wait-ForWizardConversion {
    param(
        [Parameter(Mandatory = $true)]$WizardElement,
        [Parameter(Mandatory = $true)]$Setup,
        [int]$TimeoutSeconds = 180
    )
    $deadline = (Get-Date).AddSeconds($TimeoutSeconds)
    $lastPrint = Get-Date
    while ((Get-Date) -lt $deadline) {
        Assert-NoNewDialogs -ProcessId $Setup.Instance.Process.Id -Baseline $Setup.Baseline

        # Fail fast instead of polling a window that no longer exists (seen: the wizard vanished and the
        # loop spun for the full timeout reporting "progress ?, OK enabled False").
        $setupProc = $Setup.Instance.Process
        $setupProc.Refresh()
        if ($setupProc.HasExited) {
            throw "The app process (PID $($setupProc.Id)) exited during the wizard conversion (exit code $($setupProc.ExitCode)) - it crashed or closed itself."
        }
        $wizardName = $null
        try { $wizardName = $WizardElement.Current.Name } catch { }
        if ($null -eq $wizardName) {
            $remaining = @(Get-OpenWindows -ProcessId $setupProc.Id | ForEach-Object { "'$($_.Name)'" })
            throw "The wizard window disappeared during the conversion while the app (PID $($setupProc.Id)) is still running. Windows it has now: $(if ($remaining.Count) { $remaining -join ', ' } else { '(none)' })."
        }

        $percent = Get-WizardProgressPercent -WizardElement $WizardElement
        $okEnabled = $false
        try {
            $okButton = Find-ButtonByName -Root $WizardElement -Name "OK"
            if ($null -ne $okButton) { $okEnabled = [bool]$okButton.Current.IsEnabled }
        } catch { }

        if ($okEnabled -and ($null -eq $percent -or $percent -ge 100)) { return }

        if (((Get-Date) - $lastPrint).TotalSeconds -ge 3) {
            $shown = if ($null -ne $percent) { "$percent%" } else { "?" }
            Write-Host "`n    [waiting] progress: $shown  OK enabled: $okEnabled" -NoNewline -ForegroundColor DarkGray
            $lastPrint = Get-Date
        }
        Start-Sleep -Milliseconds 500
    }
    # Diagnostics: which process owns the window we were watching, and is it the x86 one we launched?
    try {
        $ownerPid = $WizardElement.Current.ProcessId
        $owner = Get-Process -Id $ownerPid -ErrorAction SilentlyContinue
        Write-Host "`n    [diagnostic] Watched window '$($WizardElement.Current.Name)' belongs to PID $ownerPid ($($owner.Path)); expected PID $($Setup.Instance.Process.Id)." -ForegroundColor Yellow
        foreach ($other in @(Get-Process -Name "ScientificInterface","EwEImportHelper" -ErrorAction SilentlyContinue)) {
            Write-Host "    [diagnostic] running: $($other.ProcessName) PID $($other.Id) $($other.Path)" -ForegroundColor Yellow
        }
    } catch { }
    throw "The wizard's conversion did not finish (OK enabled) within $TimeoutSeconds seconds."
}

function Test-WizardImportSucceeds {
    param(
        [Parameter(Mandatory = $true)]$Setup,
        [int]$ModelIndex = 0,
        # Number of models in the file. With exactly one the wizard pre-selects it (and shows no
        # scroll bar), so the selection step is skipped and Next is pressed directly.
        [int]$ModelCount = 0
    )
    $wizardHandle = $Setup.FirstWindow.NativeWindowHandle
    $wizardElement = Get-WindowElementByHandle -Handle $wizardHandle

    Write-Step "Intro page: Next" { Invoke-ButtonByName -WindowElement $wizardElement -Name "Next" }
    Start-Sleep -Milliseconds 500
    if ($ModelCount -eq 1) {
        Write-Host "  Single model in this file - already selected, not selecting anything."
    } else {
        Write-Step "Selecting model #$($ModelIndex + 1)" { Select-WizardModel -WizardElement $wizardElement -Index $ModelIndex }
    }
    Write-Step "Model-selection page: Next" { Invoke-ButtonByName -WindowElement $wizardElement -Name "Next" }
    Write-Host "  Converting (watching for unexpected dialogs)..." -NoNewline
    Wait-ForWizardConversion -WizardElement $wizardElement -Setup $Setup
    Write-Host " done" -ForegroundColor Green
    Write-Step "Finishing the wizard (OK)" { Invoke-ButtonByName -WindowElement $wizardElement -Name "OK" }
    Start-Sleep -Milliseconds 1000
}

# Whole scenario for a menu-based wizard import: launch with the model on the command line, step
# through the wizard selecting model number -ModelNumber (1-based: 1 = first row, 2 = second, ...),
# wait for the conversion to finish, then check for unexpected dialogs. Number N means N-1 presses
# of Down in the grid (see Select-WizardModel).
function Invoke-WizardMenuImportTest {
    param(
        [Parameter(Mandatory = $true)][string]$ModelPath,
        [Parameter(Mandatory = $true)][int]$ModelNumber,
        # 0 = ask the import helper how many models the file has.
        [int]$ModelCount = 0,
        [string]$Label = "wizard import test instance"
    )
    if ($ModelCount -le 0) { $ModelCount = Get-LegacyModelCount -ModelPath $ModelPath }
    $setup = Open-LegacyModelDirectly -ModelPath $ModelPath -ExpectedWindowName "Model conversion utility"
    try {
        Test-WizardImportSucceeds -Setup $setup -ModelIndex ($ModelNumber - 1) -ModelCount $ModelCount
        Assert-NoNewDialogs -ProcessId $setup.Instance.Process.Id -Baseline $setup.Baseline
    } finally {
        Stop-TestInstance -ProcessId $setup.Instance.Process.Id -Label $Label
    }
}

# ===========================================================================
# GBR_Linked_ecosystem.MDB on x64: the legacy-import helper's console lists the 16 submodels and
# asks for a number ("Please enter the model number and press Enter." input box, prompt
# "Enter a number (1-16):" in the log). Type 1 + Enter, then it's the same flow as every other helper
# import: wait for "--- Process exited ---", evaluate the log, Continue.
function Submit-HelperModelChoice {
    param(
        [Parameter(Mandatory = $true)]$ConsoleElement,
        [Parameter(Mandatory = $true)][string]$Choice
    )
    $promptText = "Enter a number"
    $log = Wait-ForEditControlText -WindowElement $ConsoleElement -ContainsAnyOf @($promptText) -TimeoutSeconds 60
    if ($log -notlike "*$promptText*") { throw "The helper never asked for a model number within 60 seconds. Console text: '$log'" }

    # The input box: matched by its accessible name, whatever ControlType PowerShell's UIA client
    # reports for it (Insights shows Edit; this client has sometimes reported EDIT-class controls
    # as Pane instead).
    $inputName = "Please enter the model number and press Enter."
    $inputBox = $null
    foreach ($candidate in $ConsoleElement.FindAll([System.Windows.Automation.TreeScope]::Descendants, [System.Windows.Automation.Condition]::TrueCondition)) {
        $candidateName = try { $candidate.Current.Name } catch { "" }
        if ($candidateName -eq $inputName) {
            $inputBox = $candidate
            $candidateType = try { $candidate.Current.ControlType.ProgrammaticName } catch { "?" }
            Write-Host "`n    [diagnostic] Input box found: $candidateType class='$($candidate.Current.ClassName)' rect=$($candidate.Current.BoundingRectangle)" -ForegroundColor DarkGray
            if ($candidateType -like "*Edit*") { break }
        }
    }
    if ($null -eq $inputBox) {
        Write-Host "`n    [diagnostic] Input box not found by name. Console UIA tree:" -ForegroundColor Yellow
        Write-UiaTreeDump -Element $ConsoleElement
        throw "Could not find the model-number input box ('$inputName')."
    }
    Invoke-ElementClick -Element $inputBox
    Start-Sleep -Milliseconds 300
    [System.Windows.Forms.SendKeys]::SendWait($Choice)
    Start-Sleep -Milliseconds 200
    [System.Windows.Forms.SendKeys]::SendWait("{ENTER}")
}

# Whole scenario for a menu-based legacy-import-helper import: launch with the model on the command
# line, answer the helper's "Enter a number" prompt with -ModelNumber (1-based, as listed in the
# console), then evaluate the log like every other helper import and check for unexpected dialogs.
# -ModelCount: how many models the file holds. A file with exactly ONE model gets no prompt at all
# from the helper ("One model found: ..." and it carries on), so nothing is typed in that case;
# 0 (default) means "unknown - assume the prompt appears".
function Invoke-HelperMenuImportTest {
    param(
        [Parameter(Mandatory = $true)][string]$ModelPath,
        [Parameter(Mandatory = $true)][int]$ModelNumber,
        [int]$ModelCount = 0,
        [string]$Label = "helper import test instance"
    )
    $setup = Open-LegacyModelDirectly -ModelPath $ModelPath
    try {
        $consoleElement = Get-WindowElementByHandle -Handle $setup.FirstWindow.NativeWindowHandle
        if ($ModelCount -ne 1) {
            Write-Step "Choosing submodel $ModelNumber" { Submit-HelperModelChoice -ConsoleElement $consoleElement -Choice "$ModelNumber" }
        }
        Test-HelperImportSucceeds -Setup $setup
        Assert-NoNewDialogs -ProcessId $setup.Instance.Process.Id -Baseline $setup.Baseline
    } finally {
        Stop-TestInstance -ProcessId $setup.Instance.Process.Id -Label $Label
    }
}


# ===========================================================================
# Headless import: runs the helper's `import` mode directly (hidden, no app, no UI) in a throwaway
# folder and judges the result file it writes. For files with more than one model the helper asks
# for a number on stdin ("Enter a number (1-N):") - it is piped in here. Throws on failure, with
# the reason from the result file (first line OK/EXISTS/FAIL, second line the target path or the
# reason). Does NOT cover anything the app itself does (console window, wizard, dialogs).
function Test-HeadlessImportSucceeds {
    param(
        [Parameter(Mandatory = $true)][string]$ModelPath,
        [Parameter(Mandatory = $true)][int]$ModelNumber,
        [int]$ModelCount = 0,
        [int]$TimeoutSeconds = 600
    )
    $helperExe = Get-ImportHelperExe
    $workFolder = Join-Path $env:TEMP "EwE-HeadlessImport\$(Get-Random)"
    $outFolder = Join-Path $workFolder "out"
    New-Item -ItemType Directory -Path $outFolder -Force | Out-Null
    $resultFile = Join-Path $workFolder "result.txt"
    $proc = $null
    try {
        $psi = New-Object System.Diagnostics.ProcessStartInfo
        $psi.FileName = $helperExe
        $psi.UseShellExecute = $false
        $psi.CreateNoWindow = $true
        $psi.RedirectStandardInput = $true
        $psi.RedirectStandardOutput = $true
        $psi.RedirectStandardError = $true
        $psi.Arguments = (@("import", $ModelPath, $outFolder, $resultFile) | ForEach-Object { '"' + ($_ -replace '"', '\"') + '"' }) -join ' '
        $proc = [System.Diagnostics.Process]::Start($psi)
        $stderrTask = $proc.StandardError.ReadToEndAsync()
        if ($ModelCount -ne 1) { $proc.StandardInput.WriteLine([string]$ModelNumber) }
        $proc.StandardInput.Close()

        # Read stdout line by line so progress can be shown while the import runs. The helper prints
        # "[ 42%] message" lines for progress; the last few lines are kept for a failure report.
        $recent = New-Object System.Collections.Generic.Queue[string]
        $watch = [System.Diagnostics.Stopwatch]::StartNew()
        $lastPercent = -1
        $readTask = $proc.StandardOutput.ReadLineAsync()
        while ($true) {
            if ($readTask.Wait(1000)) {
                $line = $readTask.Result
                if ($null -eq $line) { break }   # end of stream: the helper has exited
                if ($line.Trim() -ne "") {
                    $recent.Enqueue($line)
                    if ($recent.Count -gt 3) { [void]$recent.Dequeue() }
                    if ($line -match '^\[\s*(\d+)%\]') { $lastPercent = [int]$Matches[1] }
                }
                $readTask = $proc.StandardOutput.ReadLineAsync()
            }
            $elapsed = [int]$watch.Elapsed.TotalSeconds
            $percentText = if ($lastPercent -ge 0) { "$lastPercent%" } else { "starting..." }
            Write-Host -NoNewline "`r  Importing: $percentText ($elapsed s)    "
            if ($elapsed -gt $TimeoutSeconds) {
                try { $proc.Kill() } catch { }
                Write-Host ""
                throw "Headless import timed out after $TimeoutSeconds seconds."
            }
        }
        Write-Host -NoNewline "`r                                        `r"
        $proc.WaitForExit()
        $lines = @()
        if (Test-Path $resultFile) { $lines = @(Get-Content $resultFile) }
        $status = if ($lines.Count -gt 0) { $lines[0] } else { "NO-RESULT" }
        if ($status -ne "OK" -and $status -ne "EXISTS") {
            $reason = if ($lines.Count -gt 1) { ($lines | Select-Object -Skip 1) -join " | " } else { $stderrTask.Result.Trim() }
            $tail = $recent.ToArray() -join " / "
            throw "Headless import result '$status' (exit code $($proc.ExitCode)): $reason`n  Last output: $tail"
        }
        $target = $lines[1]
        if (-not (Test-Path $target) -or (Get-Item $target).Length -eq 0) {
            throw "Helper reported $status but the target database is missing or empty: $target"
        }
        Write-Host "  Imported headlessly: $(Split-Path -Leaf $target) ($([math]::Round((Get-Item $target).Length / 1KB)) KB)"
    } finally {
        # Ctrl+C / a failure must not leave the helper running in the background (it would also hold
        # files in the folder removed below).
        if ($null -ne $proc -and -not $proc.HasExited) { try { $proc.Kill() } catch { } }
        Remove-Item -Path $workFolder -Recurse -Force -ErrorAction SilentlyContinue
    }
}
