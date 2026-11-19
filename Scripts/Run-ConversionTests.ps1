<#
.SYNOPSIS
    Tests for the legacy-model detection/conversion flow (frmEwE6.CovertToEwE6 /
    cDataSourceFactory.GetCompatibility - see HANDOFF.md's tag table). Each test opens a REAL,
    user-supplied legacy Ecopath 5 / Access model file and confirms the app responds the way that
    file's tag says it should: an EwE5-referral message, the in-process import wizard, or the x86
    import helper's console - and, for the helper, whether the import itself then succeeds or fails.

    These sample model files are real third-party data, not committed to this repo. See
    Test-ConversionPrerequisites.ps1 for how a machine without them yet gets asked for a folder
    path (once), and how individual missing files are skipped (shown as SKIP, not FAIL) rather than
    blocking the whole run.

.PARAMETER Platform
    "x64" or "x86" - which build each disposable instance launches as. Default "x64". Several tags
    (Import-Wizard32bitJet3 in particular) can ONLY be reached on x86 - run both platforms to cover
    the full tag set, same as the other Run-*Tests.ps1 scripts in this toolkit.

.PARAMETER Only
    Run just the one test whose name contains this substring (case-insensitive), skipping the rest.
    The prerequisite check itself still always runs in full, even with -Only applied - it's cheap,
    and a stale/missing config would otherwise go unnoticed until some OTHER day's -Only run needed
    the very file that was never checked for.

.PARAMETER MainWindowTimeoutSeconds
    How long each disposable instance waits for its main window before giving up. Default 90.

.EXAMPLE
    .\Run-ConversionTests.ps1 -Platform x64
.EXAMPLE
    .\Run-ConversionTests.ps1 -Platform x86 -Only "EcopathBass"
#>
param(
    [ValidateSet("x64", "x86")]
    [string]$Platform = "x64",
    [string]$Only,
    [int]$MainWindowTimeoutSeconds = 90
)

. (Join-Path $PSScriptRoot "TestHelpers.ps1")
. (Join-Path $PSScriptRoot "Test-ConversionPrerequisites.ps1")
$script:TestNameFilter = $Only
$startInstanceScript = Join-Path $PSScriptRoot "Start-TestInstance.ps1"

# Every model file ANY conversion test (automated or still-manual, below) needs - kept as one list
# so the prerequisite check and the skip-list cover all of them, even the ones not yet automated.
$allRequiredFileNames = @(
    "Gironde estuary.MDB",
    "gulfmexico.MDB",
    "Kenya, Lake Nakuru - 1972.mdb",
    "EcopathMSY.MDB",
    "Ecopath.MDB",
    "GBR_Linked_ecosystem.MDB"
)

$prereqs = Confirm-ConversionTestPrerequisites -RequiredFileNames $allRequiredFileNames

# Runs a conversion test's body with the SKIP-if-missing/bypassed check factored out, so each test
# below only has to say its own name, file, and what to actually do with it - not repeat this
# check. Deliberately does NOT go through Invoke-Test for the skip case: Invoke-Test always records
# its own PASS/FAIL after running the body, which would be wrong for a model that was never
# attempted at all.
function Invoke-ConversionTestEntry {
    param(
        [Parameter(Mandatory = $true)][string]$Name,
        [Parameter(Mandatory = $true)][string]$FileName,
        [Parameter(Mandatory = $true)][scriptblock]$Test
    )
    if ($script:TestNameFilter -and $Name -notlike "*$script:TestNameFilter*") { return }

    if ($prereqs.Bypassed -or ($prereqs.SkippedModels -contains $FileName)) {
        $displayName = if ($script:CurrentPlatformLabel) { "[$script:CurrentPlatformLabel] $Name" } else { $Name }
        Write-Host "`n=== $displayName ===" -ForegroundColor Cyan
        Write-Host "SKIPPED (model file not found): $FileName" -ForegroundColor Yellow
        Add-TestResult -Name $Name -Result "SKIP" -Reason "model file not found: $FileName"
        return
    }

    $modelPath = $prereqs.ModelPaths[$FileName]
    Invoke-Test $Name { & $Test $modelPath }
}

# Shared first half of every conversion test: start a fresh disposable instance (own instance per
# test, same reasoning as Run-CrashRecoveryTests.ps1 - these tests change app/model state in big,
# not-easily-undone ways, so sharing an instance across tests isn't safe here), capture an idle
# baseline, open the legacy file via the UI, then wait for WHATEVER new window shows up first - the
# caller inspects it to tell the three possible outcomes apart.
#
# Only needed for a test that must demonstrate the app's behavior with a DIFFERENT model already
# open first (confirmed via the EwE5-referral case: "the originally open model stays open" is only
# a meaningful check if some other model really was open before the legacy file was tried) - a
# model passed directly on the command line at launch can't show that, since only one file can be
# given at startup. NOT currently used by the two automated tests below (see
# Open-LegacyModelDirectly instead) - kept for whenever the TooOld/wizard/GBR tests get automated.
function Open-LegacyModelAndWait {
    param(
        [Parameter(Mandatory = $true)][string]$ModelPath,
        [int]$WaitTimeoutSeconds = 30
    )
    Write-Host "  Starting a disposable instance:"
    $instance = & $startInstanceScript -Platform $Platform -MainWindowTimeoutSeconds $MainWindowTimeoutSeconds
    Write-InstanceStarted -ProcessId $instance.Process.Id -Label "conversion test instance"

    $baseline = Write-Step "Capturing idle window baseline" { Get-WindowBaseline -ProcessId $instance.Process.Id }
    Write-Step "Opening the legacy model via the UI" { Open-LegacyModelViaUI -Proc $instance.Process -Path $ModelPath }
    $firstWindow = Write-Step "Waiting for a response window" {
        Wait-ForNewWindow -ProcessId $instance.Process.Id -Baseline $baseline -TimeoutSeconds $WaitTimeoutSeconds
    }
    if ($null -eq $firstWindow) {
        Stop-TestInstance -ProcessId $instance.Process.Id -Label "conversion test instance (no response window)"
        throw "No new window appeared after opening '$ModelPath' within $WaitTimeoutSeconds seconds - expected an EwE5 referral message, an import wizard, or the legacy-import helper console."
    }

    [PSCustomObject]@{ Instance = $instance; Baseline = $baseline; FirstWindow = $firstWindow }
}

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

    Write-Step "Dismissing the import console (Continue)" { Invoke-ButtonByName -WindowElement $consoleElement -Name "Continue" }
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

Invoke-ConversionTestEntry -Name "Conversion: Gironde estuary.MDB imports successfully (Import-x64Helper)" -FileName "Gironde estuary.MDB" -Test {
    param($modelPath)
    $setup = Open-LegacyModelDirectly -ModelPath $modelPath
    try {
        Test-HelperImportSucceeds -Setup $setup
        Assert-NoNewDialogs -ProcessId $setup.Instance.Process.Id -Baseline $setup.Baseline
    } finally {
        Stop-TestInstance -ProcessId $setup.Instance.Process.Id -Label "Gironde estuary test instance"
    }
}

# Currently expected to FAIL: gulfmexico.MDB hits a real, reproducible SalinityForcingShapeID
# import bug (not a test-harness issue) - this test will start passing again once that's fixed,
# the same as any other regression test for a known bug. Same success-only check as every other
# Import-x64Helper test, nothing special added for the known failure.
Invoke-ConversionTestEntry -Name "Conversion: gulfmexico.MDB imports successfully (Import-x64Helper)" -FileName "gulfmexico.MDB" -Test {
    param($modelPath)
    $setup = Open-LegacyModelDirectly -ModelPath $modelPath
    try {
        Test-HelperImportSucceeds -Setup $setup
        Assert-NoNewDialogs -ProcessId $setup.Instance.Process.Id -Baseline $setup.Baseline
    } finally {
        Stop-TestInstance -ProcessId $setup.Instance.Process.Id -Label "gulfmexico test instance"
    }
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
    $textCondition = New-Object System.Windows.Automation.PropertyCondition(
        [System.Windows.Automation.AutomationElement]::ControlTypeProperty, [System.Windows.Automation.ControlType]::Text)
    $textElement = $null
    try { $textElement = $dialogElement.FindFirst([System.Windows.Automation.TreeScope]::Descendants, $textCondition) } catch { }
    $messageText = if ($null -ne $textElement) { try { $textElement.Current.Name } catch { "" } } else { "" }

    $matchedMarker = $false
    foreach ($marker in $ExpectedTextMarkers) {
        if ($messageText -like "*$marker*") { $matchedMarker = $true; break }
    }
    Assert-True $matchedMarker `
        "Expected the dialog's text to mention one of: $($ExpectedTextMarkers -join ', ') - the title matched but the content didn't match the one expected here. Got: '$messageText'"

    Write-Step "Dismissing the EwE5-referral dialog (OK)" { Invoke-ButtonByName -WindowElement $dialogElement -Name "OK" }
}

Invoke-ConversionTestEntry -Name "Conversion: Kenya, Lake Nakuru - 1972.mdb refers to EwE5 (TooOld-Jet3x64-confirmed)" -FileName "Kenya, Lake Nakuru - 1972.mdb" -Test {
    param($modelPath)
    $setup = Open-LegacyModelDirectly -ModelPath $modelPath -ExpectedWindowName $ReferralDialogCaption
    try {
        Test-ReferralDialogAppears -Setup $setup -ExpectedTextMarkers @(
            "outdated version of Ecopath 5",
            "not supported by the EwE6 import process"
        )
        Assert-NoNewDialogs -ProcessId $setup.Instance.Process.Id -Baseline $setup.Baseline
    } finally {
        Stop-TestInstance -ProcessId $setup.Instance.Process.Id -Label "Kenya Lake Nakuru test instance"
    }
}

# ===========================================================================
# Import wizard / GBR's submodel picker - NOT YET AUTOMATED. I have no confirmed
# Accessibility-Insights capture of the import wizard dialog or GBR's multi-submodel picker, and
# guessing their button names/structure blind is exactly the kind of mistake that cost real time
# on DialogChecker earlier in this project. Left as manual checklists (Invoke-ManualTest) for now -
# send a screenshot of each and I'll convert it to a real automated test the same way the three
# above were built.
# ===========================================================================

Invoke-ConversionTestEntry -Name "Conversion: EcopathMSY.MDB refers to EwE5 (TooOld-DirectOpen)" -FileName "EcopathMSY.MDB" -Test {
    param($modelPath)
    $setup = Open-LegacyModelDirectly -ModelPath $modelPath -ExpectedWindowName $ReferralDialogCaption
    try {
        Test-ReferralDialogAppears -Setup $setup -ExpectedTextMarkers @(
            "outdated version of Ecopath 5",
            "not supported by the EwE6 import process"
        )
        Assert-NoNewDialogs -ProcessId $setup.Instance.Process.Id -Baseline $setup.Baseline
    } finally {
        Stop-TestInstance -ProcessId $setup.Instance.Process.Id -Label "EcopathMSY test instance"
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
function Write-UiaTreeDump {
    param(
        [Parameter(Mandatory = $true)]$Element,
        [int]$MaxDepth = 8,
        [int]$MaxNodes = 120
    )
    $walker = [System.Windows.Automation.TreeWalker]::ControlViewWalker
    $script:uiaDumpCount = 0
    function Write-UiaNode {
        param($Node, [int]$Depth)
        if ($script:uiaDumpCount -ge $MaxNodes -or $Depth -gt $MaxDepth) { return }
        $script:uiaDumpCount++
        $type = try { $Node.Current.ControlType.ProgrammaticName -replace '^ControlType\.', '' } catch { "?" }
        $name = try { $Node.Current.Name } catch { "?" }
        $cls = try { $Node.Current.ClassName } catch { "?" }
        $rect = try { $Node.Current.BoundingRectangle } catch { "?" }
        $focusable = try { $Node.Current.IsKeyboardFocusable } catch { "?" }
        Write-Host ("    {0}{1} name='{2}' class='{3}' rect={4} focusable={5}" -f ("  " * $Depth), $type, $name, $cls, $rect, $focusable) -ForegroundColor DarkGray
        $child = $walker.GetFirstChild($Node)
        while ($null -ne $child) {
            Write-UiaNode -Node $child -Depth ($Depth + 1)
            $child = $walker.GetNextSibling($child)
        }
    }
    Write-UiaNode -Node $Element -Depth 0
}

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
    if ($null -eq $scrollBar) {
        Write-Host "`n    [diagnostic] No ScrollBar descendant found. Wizard UIA tree:" -ForegroundColor Yellow
        Write-UiaTreeDump -Element $WizardElement
        throw "Could not find the model grid (its scroll bar) on the wizard's model-selection page after ~10 seconds (UIA tree dumped above)."
    }
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

    $rect = $sibling.Current.BoundingRectangle
    $clickX = [int]($rect.X) + 5
    $clickY = [int]($rect.Y) + 5
    Write-Host "    [diagnostic] Chosen pane rect=$rect; clicking at ($clickX, $clickY)" -ForegroundColor DarkGray
    [System.Windows.Forms.Cursor]::Position = New-Object System.Drawing.Point($clickX, $clickY)
    Start-Sleep -Milliseconds 150
    $actual = [System.Windows.Forms.Cursor]::Position
    Write-Host "    [diagnostic] Cursor is now at ($($actual.X), $($actual.Y))" -ForegroundColor DarkGray
    [Win32TestRunner]::mouse_event([Win32TestRunner]::MOUSEEVENTF_LEFTDOWN, 0, 0, 0, [UIntPtr]::Zero)
    [Win32TestRunner]::mouse_event([Win32TestRunner]::MOUSEEVENTF_LEFTUP, 0, 0, 0, [UIntPtr]::Zero)
    Start-Sleep -Milliseconds 300

    if ($Index -gt 0) {
        # Wait out the system double-click interval so the second click is a separate click.
        Start-Sleep -Milliseconds 800
        Write-Host "    [diagnostic] Clicking again to deselect the first model, then Down x$Index + Space" -ForegroundColor DarkGray
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

        $percent = Get-WizardProgressPercent -WizardElement $WizardElement
        $okEnabled = $false
        try {
            $okButton = Find-ByNameAndType -Root $WizardElement -Name "OK" -ControlType ([System.Windows.Automation.ControlType]::Button)
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
    throw "The wizard's conversion did not finish (OK enabled) within $TimeoutSeconds seconds."
}

function Test-WizardImportSucceeds {
    param(
        [Parameter(Mandatory = $true)]$Setup,
        [int]$ModelIndex = 0
    )
    $wizardHandle = $Setup.FirstWindow.NativeWindowHandle
    $wizardElement = Get-WindowElementByHandle -Handle $wizardHandle

    Write-Step "Intro page: Next" { Invoke-ButtonByName -WindowElement $wizardElement -Name "Next" }
    Start-Sleep -Milliseconds 500
    Write-Step "Selecting model #$($ModelIndex + 1)" { Select-WizardModel -WizardElement $wizardElement -Index $ModelIndex }
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
        [string]$Label = "wizard import test instance"
    )
    $setup = Open-LegacyModelDirectly -ModelPath $ModelPath -ExpectedWindowName "Model conversion utility"
    try {
        Test-WizardImportSucceeds -Setup $setup -ModelIndex ($ModelNumber - 1)
        Assert-NoNewDialogs -ProcessId $setup.Instance.Process.Id -Baseline $setup.Baseline
    } finally {
        Stop-TestInstance -ProcessId $setup.Instance.Process.Id -Label $Label
    }
}

# Known failing case for now: the first model (Bass) hits a NoNullAllowedException during import.
Invoke-ConversionTestEntry -Name "Conversion: EcopathBass - Ecopath.MDB imports the first model via the wizard (Import-WizardNonJet3)" -FileName "Ecopath.MDB" -Test {
    param($modelPath)
    Invoke-WizardMenuImportTest -ModelPath $modelPath -ModelNumber 1 -Label "EcopathBass wizard test instance"
}

Invoke-ConversionTestEntry -Name "Conversion: EcopathBeam2000 - Ecopath.MDB imports the second model via the wizard (Import-WizardNonJet3)" -FileName "Ecopath.MDB" -Test {
    param($modelPath)
    Invoke-WizardMenuImportTest -ModelPath $modelPath -ModelNumber 2 -Label "EcopathBeam2000 wizard test instance"
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
function Invoke-HelperMenuImportTest {
    param(
        [Parameter(Mandatory = $true)][string]$ModelPath,
        [Parameter(Mandatory = $true)][int]$ModelNumber,
        [string]$Label = "helper import test instance"
    )
    $setup = Open-LegacyModelDirectly -ModelPath $ModelPath
    try {
        $consoleElement = Get-WindowElementByHandle -Handle $setup.FirstWindow.NativeWindowHandle
        Write-Step "Choosing submodel $ModelNumber" { Submit-HelperModelChoice -ConsoleElement $consoleElement -Choice "$ModelNumber" }
        Test-HelperImportSucceeds -Setup $setup
        Assert-NoNewDialogs -ProcessId $setup.Instance.Process.Id -Baseline $setup.Baseline
    } finally {
        Stop-TestInstance -ProcessId $setup.Instance.Process.Id -Label $Label
    }
}

if ($Platform -eq "x64") {
    # Known failing case for now (EatEffBad import error) - evaluated from the log like every other
    # helper import test.
    Invoke-ConversionTestEntry -Name "Conversion: GBR_Linked_ecosystem.MDB - first submodel imports via the helper (Import-x64Helper)" -FileName "GBR_Linked_ecosystem.MDB" -Test {
        param($modelPath)
        Invoke-HelperMenuImportTest -ModelPath $modelPath -ModelNumber 1 -Label "GBR_Linked_ecosystem test instance"
    }
} else {
    # x86: the same model goes through the in-process Import Wizard instead of the helper console.
    Invoke-ConversionTestEntry -Name "Conversion: GBR_Linked_ecosystem.MDB - first submodel imports via the wizard (Import-Wizard32bitJet3)" -FileName "GBR_Linked_ecosystem.MDB" -Test {
        param($modelPath)
        Invoke-WizardMenuImportTest -ModelPath $modelPath -ModelNumber 1 -Label "GBR_Linked_ecosystem wizard test instance"
    }
}

Show-TestSummary
