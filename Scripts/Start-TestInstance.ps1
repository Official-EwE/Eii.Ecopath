<#
.SYNOPSIS
    Builds ScientificInterface (a normal, incremental build - only recompiles what changed) and
    launches it with a fresh copy of a test model, returning the started Process once its main
    window is ready. Removes the need to manually launch the app and capture its Process object
    before running tests.

.PARAMETER RepoRoot
    Root of the Eii.Ecopath repo. Defaults to two levels up from this script's own folder
    (Scripts\ -> repo root), matching where this toolkit has lived all session - override if moved.

.PARAMETER Platform
    "x64" or "x86" - which build output to launch. Default "x64".

.PARAMETER Configuration
    Default "Debug".

.PARAMETER SourceModelPath
    The model file to copy and open. Defaults to Database\Anchovy Bay Spatial.ewemdb - a real, named
    sample model, not Sources\EwECore\Resources\EwE6.mdb (the blank TEMPLATE Create() uses to
    generate brand-new models internally, not something meant to be opened directly as-is - tried
    first, and ruled out: staging never activated for it even with a renamed extension).

.PARAMETER WorkFolder
    Where the model gets copied to before opening. Defaults to a fresh, randomly-named folder under
    %TEMP%. An "original" subfolder holds an untouched copy alongside the one actually opened, for
    diffing/restoring against later if ever needed - the one actually passed to the app is a
    separate copy one level up, since tests are expected to modify it.

.EXAMPLE
    $instance = .\Start-TestInstance.ps1
    $proc = $instance.Process
    $instance.ModelPath   # the REAL file passed to the app - not the staged working copy
#>
param(
    [string]$RepoRoot = (Split-Path -Parent $PSScriptRoot),
    [ValidateSet("x64", "x86")]
    [string]$Platform = "x64",
    [string]$Configuration = "Debug",
    [string]$SourceModelPath,
    [string]$WorkFolder
)

if (-not $SourceModelPath) {
    $SourceModelPath = Join-Path $RepoRoot "Database\Anchovy Bay Spatial.ewemdb"
}
if (-not (Test-Path $SourceModelPath)) {
    throw "Source model not found: $SourceModelPath"
}

if (-not $WorkFolder) {
    $WorkFolder = Join-Path $env:TEMP "EwE-TestRun-$(Get-Random)"
}
$originalFolder = Join-Path $WorkFolder "original"
New-Item -ItemType Directory -Path $originalFolder -Force | Out-Null

$modelFileName = Split-Path -Leaf $SourceModelPath
Copy-Item $SourceModelPath (Join-Path $originalFolder $modelFileName) -Force
$workingModelPath = Join-Path $WorkFolder $modelFileName
Copy-Item $SourceModelPath $workingModelPath -Force
Write-Host "Model copied to: $workingModelPath (untouched copy also kept in .\original\)"

$vbproj = Join-Path $RepoRoot "Sources\ScientificInterface\ScientificInterface.vbproj"
if (-not (Test-Path $vbproj)) {
    throw "Project file not found: $vbproj"
}

Write-Host "Building ScientificInterface ($Configuration|$Platform)..."
# Piped through Write-Host, not left to print directly: an external command's own stdout lines
# become part of THIS SCRIPT's pipeline output in PowerShell, not just console text - left
# unsuppressed, they'd get captured alongside the real $proc object by a caller doing
# "$Proc = & Start-TestInstance.ps1", producing an array instead of a single Process (confirmed -
# this is exactly what happened). Routing through Write-Host keeps the build log fully visible
# without it polluting this script's actual return value.
dotnet build $vbproj -c $Configuration -p:Platform=$Platform | ForEach-Object { Write-Host $_ }
if ($LASTEXITCODE -ne 0) {
    throw "Build failed (dotnet build exit code $LASTEXITCODE)."
}

$exePath = Join-Path $RepoRoot "Sources\ScientificInterface\bin\$Platform\$Configuration\net48\ScientificInterface.exe"
if (-not (Test-Path $exePath)) {
    throw "Built exe not found at the expected path: $exePath"
}

. (Join-Path $PSScriptRoot "TestHelpers.ps1")

Write-Host "Starting: $exePath `"$workingModelPath`""
$proc = Start-Process -FilePath $exePath -ArgumentList "`"$workingModelPath`"" -PassThru

# A "your model database will be updated to a newer version" confirmation can appear BEFORE
# MainWindowHandle is even populated - a loop that only waits on that handle would hang/time out
# with nobody ever clicking it. Same generic dialog shape as the lock-contention dialog (confirmed
# via Accessibility Insights: Name="Ecopath with Ecosim", ControlType=Window, with Yes/No buttons),
# watched for here since the usual FromHandle(MainWindowHandle) approach isn't available yet -
# searching the whole desktop, scoped to this process's own PID, instead.
Write-Host "Waiting for the main window to appear (watching for a database-upgrade prompt too)..."
Wait-ForMainWindow -Proc $proc -TimeoutSeconds 30

Write-Host "Ready - PID $($proc.Id), model folder: $WorkFolder" -ForegroundColor Green
return [PSCustomObject]@{
    Process   = $proc
    ModelPath = $workingModelPath
}
