<#
.SYNOPSIS
    Folder-driven conversion tests: point it at a folder of legacy Ecopath 5 .mdb files and it works
    out, per file, how the app will handle it - then runs the matching test for the file, and for
    every (sub)model inside it.

.DESCRIPTION
    For each .mdb in the folder, using the x86 EwEImportHelper tool (a genuine 32-bit process, so the
    classic Jet provider can open any legacy .mdb, including Jet3):

      1. Jet3 or not        - the file header byte at offset 0x14 (0 = Jet3), same check the app uses.
      2. Ecopath version    - `EwEImportHelper.exe version <file>`.
      3. Compatibility      - from that version, same thresholds as cEwEDatabase.CompatibilityFromVersion:
                                < 1.6        TooOld     -> the app refers you to EwE5 (dialog test)
                                1.6 - 1.73   Importable -> convert and test every model (below)
                                anything else is not a legacy Ecopath 5 file and is skipped.
      4. Number of models   - `EwEImportHelper.exe models <file>` (non-interactive listing).

    How an Importable file is converted depends on the platform it is opened on:
      x64, Jet3 file       -> the helper's console menu    (Invoke-HelperMenuImportTest, once per model)
      x86, or non-Jet3     -> the in-process import wizard (Invoke-WizardMenuImportTest, once per model)

    Each test opens a fresh disposable instance with a fresh temp copy of the model (see
    Start-TestInstance.ps1), so conversions never touch your originals.

    Fixed, named cases live in Run-ConversionHandlingTests.ps1; this script is the generic sweep.

.PARAMETER ModelsFolder
    Folder holding the .mdb files. If omitted you are asked (the last answer is remembered in a
    gitignored `.conversion-models-folder.txt` next to this script).

.PARAMETER Recurse
    Also pick up .mdb files in subfolders.

.PARAMETER Platform
    "x64" or "x86" - which build each disposable instance launches as. Default "x64".

.PARAMETER Only
    Run just the tests whose name contains this substring (case-insensitive). The classification
    still covers every file.

.PARAMETER Ui
    Drive the real app through the UI instead (the original behavior): the referral dialog test for
    TooOld files, the helper console for Jet3 on x64, the import wizard otherwise. Slow - one app
    instance per model. Without -Ui (the default) the sweep is HEADLESS: every importable model is
    converted by running the helper's `import` mode directly and judging its result file; TooOld
    files are only classified (the referral dialog itself is covered by
    Run-ConversionHandlingTests.ps1, which keeps testing the user experience through the UI).
    Headless always uses the x86 helper, so -Platform only matters together with -Ui.

.PARAMETER PlanOnly
    Classify the files and print the plan (file, Jet3?, version, compatibility, models, test route),
    but do not run any test.

.PARAMETER MainWindowTimeoutSeconds
    How long each disposable instance waits for its main window before giving up. Default 90.

.EXAMPLE
    .\Run-ConversionTests.ps1 -ModelsFolder D:\Downloads\EwE5Databases -PlanOnly
.EXAMPLE
    .\Run-ConversionTests.ps1 -ModelsFolder D:\Downloads\EwE5Databases -Platform x86 -Only "Ecopath.MDB"
#>
param(
    [string]$ModelsFolder,
    [switch]$Recurse,
    [ValidateSet("x64", "x86")]
    [string]$Platform = "x64",
    [string]$Only,
    [switch]$PlanOnly,
    [switch]$Ui,
    [int]$MainWindowTimeoutSeconds = 90,
    # Pause on a failing test (app left open) until Enter is pressed.
    [switch]$PauseOnFailure,
    # Skip every test before the first one whose name contains this text.
    [string]$From,
    # Rebuild the app on the first launch, even if this PowerShell session already built it (e.g. after a branch switch).
    [switch]$ForceBuild
)

if ($PauseOnFailure) { $env:EWE_TEST_PAUSE_ON_FAIL = "1" }
if ($From) { $env:EWE_TEST_FROM = $From }

. (Join-Path $PSScriptRoot "TestHelpers.ps1")
# -ForceBuild: forget what this session already built, so the first launch below rebuilds once. Runners started
# from Run-AllTests.ps1 never get the switch themselves, so they reuse the build made there.
if ($ForceBuild) { $global:EwEBuiltProjects = @{} }
. (Join-Path $PSScriptRoot "ConversionTestHelpers.ps1")
$script:TestNameFilter = $Only
$startInstanceScript = Join-Path $PSScriptRoot "Start-TestInstance.ps1"

# Same thresholds as cEwEDatabase.CompatibilityFromVersion (cDBVERSION_EWE5_MIN / _MAX).
$Ewe5MinVersion = 1.6
$Ewe5MaxVersion = 1.73

# ---------------------------------------------------------------------------------------------
# Input folder
# ---------------------------------------------------------------------------------------------
$rememberedFolderFile = Join-Path $PSScriptRoot ".conversion-models-folder.txt"

function Resolve-ModelsFolder {
    param([string]$Requested)
    $default = ""
    if (Test-Path $rememberedFolderFile) { $default = (Get-Content $rememberedFolderFile -ErrorAction SilentlyContinue | Select-Object -First 1) }
    $candidate = $Requested
    while ($true) {
        if ([string]::IsNullOrWhiteSpace($candidate)) {
            $hint = if ($default) { " [$default]" } else { "" }
            $candidate = Read-Host "Folder holding the legacy model files (.mdb) to convert and test$hint"
            if ([string]::IsNullOrWhiteSpace($candidate)) { $candidate = $default }
        }
        if (-not [string]::IsNullOrWhiteSpace($candidate) -and (Test-Path -LiteralPath $candidate -PathType Container)) {
            $resolved = (Resolve-Path -LiteralPath $candidate).Path
            Set-Content -Path $rememberedFolderFile -Value $resolved
            return $resolved
        }
        Write-Host "Not a folder: '$candidate'" -ForegroundColor Yellow
        $candidate = ""
        $Requested = ""
    }
}

# Genuine legacy Jet3 (pre-Access-2000) = the version byte at offset 0x14 is 0 (the same check as
# cEwEAccessDatabase.IsLegacyJetFormat).
function Test-IsJet3File {
    param([Parameter(Mandatory = $true)][string]$Path)
    try {
        $stream = [System.IO.File]::OpenRead($Path)
        try {
            if ($stream.Length -le 0x14) { return $false }
            $stream.Seek(0x14, [System.IO.SeekOrigin]::Begin) | Out-Null
            return ($stream.ReadByte() -eq 0)
        } finally { $stream.Dispose() }
    } catch { return $false }
}

function Get-ModelFilePlan {
    param(
        [Parameter(Mandatory = $true)][System.IO.FileInfo]$File,
        [Parameter(Mandatory = $true)][string]$HelperExe
    )
    $isJet3 = Test-IsJet3File -Path $File.FullName
    $plan = [ordered]@{
        File = $File; Jet3 = $isJet3; Version = $null; Compatibility = "Unreadable"
        Models = @(); Route = "-"; Note = ""
    }

    $versionResult = Invoke-ImportHelperQuery -HelperExe $HelperExe -Arguments @("version", $File.FullName)
    if ($versionResult.ExitCode -ne 0 -or $versionResult.StdOut.Count -eq 0) {
        $plan.Note = "version check failed: $($versionResult.StdErr)"
        return [PSCustomObject]$plan
    }
    $version = 0.0
    if (-not [double]::TryParse($versionResult.StdOut[0], [System.Globalization.NumberStyles]::Float, [System.Globalization.CultureInfo]::InvariantCulture, [ref]$version) -or $version -eq 0.0) {
        $plan.Note = "unparseable version '$($versionResult.StdOut[0])'"
        return [PSCustomObject]$plan
    }
    $plan.Version = $version

    if ($version -lt $Ewe5MinVersion) {
        $plan.Compatibility = "TooOld"
        $plan.Route = if ($Ui) { "referral dialog" } else { "classification only" }
        return [PSCustomObject]$plan
    }
    if ($version -gt $Ewe5MaxVersion) {
        $plan.Compatibility = "NotLegacy"
        $plan.Note = "version $version is not a legacy Ecopath 5 file - skipped"
        return [PSCustomObject]$plan
    }

    $plan.Compatibility = "Importable"
    $modelsResult = Invoke-ImportHelperQuery -HelperExe $HelperExe -Arguments @("models", $File.FullName)
    if ($modelsResult.ExitCode -ne 0 -or $modelsResult.StdOut.Count -eq 0) {
        $plan.Note = "model listing failed: $($modelsResult.StdErr)"
        return [PSCustomObject]$plan
    }
    $models = @()
    foreach ($line in ($modelsResult.StdOut | Select-Object -Skip 1)) {
        $parts = $line -split "`t", 2
        if ($parts.Count -eq 2) { $models += [PSCustomObject]@{ Number = [int]$parts[0]; Name = $parts[1] } }
    }
    $plan.Models = $models
    $plan.Route = if (-not $Ui) { "headless helper import" } elseif ($Platform -eq "x64" -and $isJet3) { "helper console menu" } else { "import wizard" }
    [PSCustomObject]$plan
}

# ---------------------------------------------------------------------------------------------
# Classify
# ---------------------------------------------------------------------------------------------
$folder = Resolve-ModelsFolder -Requested $ModelsFolder
$files = @(Get-ChildItem -LiteralPath $folder -Filter "*.mdb" -File -Recurse:$Recurse | Sort-Object FullName)
if ($files.Count -eq 0) {
    Write-Host "No .mdb files found in '$folder'." -ForegroundColor Yellow
    return
}
Write-Host "Found $($files.Count) .mdb file(s) in '$folder'."

$helperExe = Get-ImportHelperExe
$script:ImportHelperExe = $helperExe
Write-Host "Using $helperExe`n"

$plans = @()
foreach ($file in $files) {
    Write-Host "Classifying $($file.Name)..." -NoNewline
    $plan = Get-ModelFilePlan -File $file -HelperExe $helperExe
    Write-Host " $($plan.Compatibility)" -ForegroundColor DarkGray
    $plans += $plan
}

Write-Host "`n========== PLAN ($Platform) ==========" -ForegroundColor Cyan
$plans | ForEach-Object {
    [PSCustomObject]@{
        File          = $_.File.Name
        Jet3          = $_.Jet3
        Version       = $_.Version
        Compatibility = $_.Compatibility
        Models        = $_.Models.Count
        Route         = $_.Route
        Note          = $_.Note
    }
} | Format-Table -AutoSize | Out-Host

if ($PlanOnly) { return }

# ---------------------------------------------------------------------------------------------
# Run
# ---------------------------------------------------------------------------------------------
foreach ($plan in $plans) {
    $fileName = $plan.File.Name
    $modelPath = $plan.File.FullName

    switch ($plan.Compatibility) {
        "TooOld" {
            if (-not $Ui) {
                if (-not $script:TestNameFilter -or "Conversion sweep: $fileName" -like "*$script:TestNameFilter*") {
                    Write-Host "`nTooOld (version $($plan.Version)): $fileName - classified only; the referral dialog is covered by Run-ConversionHandlingTests.ps1 (or use -Ui)." -ForegroundColor DarkGray
                    Add-TestResult -Name "Conversion sweep: $fileName" -Result "SKIP" -Reason "TooOld (version $($plan.Version)): classified only, no headless conversion"
                }
                continue
            }
            $testName = "Conversion sweep: $fileName refers to EwE5 (TooOld)"
            $script:SweepModelPath = $modelPath
            $script:SweepLabel = "$fileName referral test instance"
            $body = { Invoke-ReferralDialogTest -ModelPath $script:SweepModelPath -Label $script:SweepLabel }
            Invoke-Test $testName $body
        }
        "Importable" {
            if ($plan.Models.Count -eq 0) {
                Add-TestResult -Name "Conversion sweep: $fileName" -Result "FAIL" -Reason "No models could be listed. $($plan.Note)"
                continue
            }
            foreach ($model in $plan.Models) {
                $modelNumber = $model.Number
                $modelCount = $plan.Models.Count
                $modelLabel = "$fileName model $modelNumber/$modelCount '$($model.Name)'"
                # Handed to the test body via script-scope variables, NOT a closure: GetNewClosure() binds
                # the block to a dynamic module that cannot see this script's functions (the dot-sourced
                # helpers), which fails with "term is not recognized".
                $script:SweepModelPath = $modelPath
                $script:SweepModelNumber = $modelNumber
                $script:SweepModelCount = $modelCount
                $script:SweepLabel = "$modelLabel test instance"
                if (-not $Ui) {
                    $testName = "Conversion sweep (headless): $modelLabel imports"
                    $body = { Test-HeadlessImportSucceeds -ModelPath $script:SweepModelPath -ModelNumber $script:SweepModelNumber -ModelCount $script:SweepModelCount }
                } elseif ($plan.Route -eq "helper console menu") {
                    $testName = "Conversion sweep: $modelLabel imports via the helper console"
                    $body = { Invoke-HelperMenuImportTest -ModelPath $script:SweepModelPath -ModelNumber $script:SweepModelNumber -ModelCount $script:SweepModelCount -Label $script:SweepLabel }
                } else {
                    $testName = "Conversion sweep: $modelLabel imports via the wizard"
                    $body = { Invoke-WizardMenuImportTest -ModelPath $script:SweepModelPath -ModelNumber $script:SweepModelNumber -ModelCount $script:SweepModelCount -Label $script:SweepLabel }
                }
                Invoke-Test $testName $body
            }
        }
        default {
            $reason = if ($plan.Note) { $plan.Note } else { "not a legacy Ecopath 5 file ($($plan.Compatibility))" }
            if (-not $script:TestNameFilter -or "Conversion sweep: $fileName" -like "*$script:TestNameFilter*") {
                Write-Host "`nSKIPPED $fileName - $reason" -ForegroundColor Yellow
                Add-TestResult -Name "Conversion sweep: $fileName" -Result "SKIP" -Reason $reason
            }
        }
    }
}

Show-TestSummary

# Standalone run only (not dot-sourced by Run-AllTests): don't leave these settings behind in the PowerShell session.
if ($MyInvocation.InvocationName -ne '.') {
    Remove-Item Env:\EWE_TEST_PAUSE_ON_FAIL, Env:\EWE_TEST_FROM -ErrorAction SilentlyContinue
}
