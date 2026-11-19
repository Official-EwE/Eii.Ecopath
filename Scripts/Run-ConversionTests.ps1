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
    [int]$MainWindowTimeoutSeconds = 90
)

. (Join-Path $PSScriptRoot "TestHelpers.ps1")
. (Join-Path $PSScriptRoot "ConversionTestHelpers.ps1")
$script:TestNameFilter = $Only
$startInstanceScript = Join-Path $PSScriptRoot "Start-TestInstance.ps1"
$repoRoot = Split-Path -Parent $PSScriptRoot

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

# ---------------------------------------------------------------------------------------------
# The x86 import helper: build (cheap if up to date - this also picks up its `models` mode), locate.
# ---------------------------------------------------------------------------------------------
function Get-ImportHelperExe {
    $proj = Join-Path $repoRoot "Tools\EwEImportHelper\Source\EwEImportHelper.vbproj"
    if (-not (Test-Path $proj)) { throw "Project file not found: $proj" }
    Write-Host "Building EwEImportHelper (Debug|x86)..."
    $buildOutput = & dotnet build $proj -c Debug -p:Platform=x86 --nologo -v q 2>&1
    if ($LASTEXITCODE -ne 0) {
        $buildOutput | ForEach-Object { Write-Host $_ }
        throw "Building EwEImportHelper failed (dotnet build exit code $LASTEXITCODE)."
    }
    $exe = Get-ChildItem -Path (Join-Path (Split-Path -Parent $proj) "bin") -Recurse -Filter "EwEImportHelper.exe" -File -ErrorAction SilentlyContinue |
        Sort-Object LastWriteTime -Descending | Select-Object -First 1
    if ($null -eq $exe) { throw "EwEImportHelper.exe not found under the project's bin folder after building." }
    $exe.FullName
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
        $plan.Route = "referral dialog"
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
    $plan.Route = if ($Platform -eq "x64" -and $isJet3) { "helper console menu" } else { "import wizard" }
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
                if ($plan.Route -eq "helper console menu") {
                    $testName = "Conversion sweep: $modelLabel imports via the helper console"
                    $body = { Invoke-HelperMenuImportTest -ModelPath $script:SweepModelPath -ModelNumber $script:SweepModelNumber -ModelCount $script:SweepModelCount -Label $script:SweepLabel }
                } else {
                    $testName = "Conversion sweep: $modelLabel imports via the wizard"
                    $body = { Invoke-WizardMenuImportTest -ModelPath $script:SweepModelPath -ModelNumber $script:SweepModelNumber -Label $script:SweepLabel }
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
