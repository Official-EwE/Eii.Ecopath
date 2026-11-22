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
    .\Run-ConversionHandlingTests.ps1 -Platform x64
.EXAMPLE
    .\Run-ConversionHandlingTests.ps1 -Platform x86 -Only "EcopathBass"
#>
param(
    [ValidateSet("x64", "x86")]
    [string]$Platform = "x64",
    [string]$Only,
    [int]$MainWindowTimeoutSeconds = 90,
    # Pause on a failing test (app left open) until Enter is pressed.
    [switch]$PauseOnFailure,
    # Skip every test before the first one whose name contains this text.
    [string]$From
)

if ($PauseOnFailure) { $env:EWE_TEST_PAUSE_ON_FAIL = "1" }
if ($From) { $env:EWE_TEST_FROM = $From }

. (Join-Path $PSScriptRoot "TestHelpers.ps1")
. (Join-Path $PSScriptRoot "ConversionTestHelpers.ps1")
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
    if (-not (Test-ShouldRunTest -Name $Name)) { return }

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

# Gironde and gulfmexico: single-model Jet3 files. On x64 they go through the x86 import helper's
# console; on x86 the app imports Jet3 in-process, so the same files go through the import wizard
# instead (the helper console never appears there).
#
# gulfmexico is currently expected to FAIL on both routes: it hits a real, reproducible
# SalinityForcingShapeID import bug (not a test-harness issue) - the test will start passing once
# that's fixed, like any other regression test for a known bug.
if ($Platform -eq "x64") {
    Invoke-ConversionTestEntry -Name "Conversion: Gironde estuary.MDB imports successfully (Import-x64Helper)" -FileName "Gironde estuary.MDB" -Test {
        param($modelPath)
        Invoke-HelperMenuImportTest -ModelPath $modelPath -ModelNumber 1 -ModelCount 1 -Label "Gironde estuary test instance"
    }
    Invoke-ConversionTestEntry -Name "Conversion: gulfmexico.MDB imports successfully (Import-x64Helper)" -FileName "gulfmexico.MDB" -Test {
        param($modelPath)
        Invoke-HelperMenuImportTest -ModelPath $modelPath -ModelNumber 1 -ModelCount 1 -Label "gulfmexico test instance"
    }
} else {
    Invoke-ConversionTestEntry -Name "Conversion: Gironde estuary.MDB imports successfully (Import-Wizard32bitJet3)" -FileName "Gironde estuary.MDB" -Test {
        param($modelPath)
        Invoke-WizardMenuImportTest -ModelPath $modelPath -ModelNumber 1 -Label "Gironde estuary wizard test instance"
    }
    Invoke-ConversionTestEntry -Name "Conversion: gulfmexico.MDB imports successfully (Import-Wizard32bitJet3)" -FileName "gulfmexico.MDB" -Test {
        param($modelPath)
        Invoke-WizardMenuImportTest -ModelPath $modelPath -ModelNumber 1 -Label "gulfmexico wizard test instance"
    }
}

Invoke-ConversionTestEntry -Name "Conversion: Kenya, Lake Nakuru - 1972.mdb refers to EwE5 (TooOld-Jet3x64-confirmed)" -FileName "Kenya, Lake Nakuru - 1972.mdb" -Test {
    param($modelPath)
    Invoke-ReferralDialogTest -ModelPath $modelPath -Label "Kenya Lake Nakuru test instance"
}

Invoke-ConversionTestEntry -Name "Conversion: EcopathMSY.MDB refers to EwE5 (TooOld-DirectOpen)" -FileName "EcopathMSY.MDB" -Test {
    param($modelPath)
    Invoke-ReferralDialogTest -ModelPath $modelPath -Label "EcopathMSY test instance"
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

# Standalone run only (not dot-sourced by Run-AllTests): don't leave these settings behind in the PowerShell session.
if ($MyInvocation.InvocationName -ne '.') {
    Remove-Item Env:\EWE_TEST_PAUSE_ON_FAIL, Env:\EWE_TEST_FROM -ErrorAction SilentlyContinue
}
