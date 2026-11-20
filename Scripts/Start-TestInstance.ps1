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

.PARAMETER MainWindowTimeoutSeconds
    How long to wait for the main window to appear before giving up. Default 90 - raise this if a
    slower machine needs more time (confirmed to matter in practice: 30s wasn't always enough).

.EXAMPLE
    $instance = .\Start-TestInstance.ps1
    $proc = $instance.Process
    $instance.ModelPath              # the REAL file passed to the app - not the staged working copy
    $instance.InitialModelTimestamp  # that file's LastWriteTime at the moment it was copied, before
                                      # the app ever opened it - for confirming nothing later wrote to it
    $instance.StagedCopyPath         # the staged working copy THIS instance created, confirmed new at
                                      # launch time - not something to re-derive later by name-matching
#>
param(
    [string]$RepoRoot = (Split-Path -Parent $PSScriptRoot),
    [ValidateSet("x64", "x86")]
    [string]$Platform = "x64",
    [string]$Configuration = "Debug",
    [string]$SourceModelPath,
    [string]$WorkFolder,
    [int]$MainWindowTimeoutSeconds = 90,
    # Added for the legacy-conversion tests: CONFIRMED those launches never reliably populate
    # $proc.MainWindowHandle at all within any reasonable timeout - the window that actually
    # appears first (the x64 helper's import console, or the import wizard) apparently isn't
    # recognized by .NET as "the" main window the way a normal model-open is, so the ordinary
    # Wait-ForMainWindow wait below just hard-times-out doing nothing useful for that flow. Worse,
    # along the way it can ALSO false-positive-match its own "Ecopath with Ecosim" database-upgrade
    # detection against some OTHER same-titled dialog that isn't the upgrade prompt at all (that
    # title/shape is reused by several unrelated confirmations in this app, confirmed via the
    # lock-contention dialog sharing it already) and click an unidentified "Yes" button blind - not
    # something to do for a flow whose actual dialog sequence isn't characterized yet. Callers for
    # that flow should pass this switch and do their OWN waiting (by window name, not handle) once
    # this function returns right after launch.
    [switch]$SkipMainWindowWait,
    # Rebuild even if this session already built the app (see "Build once per PowerShell session").
    [switch]$ForceBuild
)

if (-not $SourceModelPath) {
    $SourceModelPath = Join-Path $RepoRoot "Database\Anchovy Bay Spatial.ewemdb"
}
if (-not (Test-Path $SourceModelPath)) {
    throw "Source model not found: $SourceModelPath"
}

if (-not $WorkFolder) {
    $WorkFolder = Join-Path $env:TEMP "EwE-TestRun\$(Get-Random)"
}
$originalFolder = Join-Path $WorkFolder "original"
New-Item -ItemType Directory -Path $originalFolder -Force | Out-Null

$modelFileName = Split-Path -Leaf $SourceModelPath
Copy-Item $SourceModelPath (Join-Path $originalFolder $modelFileName) -Force
$workingModelPath = Join-Path $WorkFolder $modelFileName
Copy-Item $SourceModelPath $workingModelPath -Force
# Captured HERE, before the app ever opens it - the one point this timestamp is guaranteed to
# reflect nothing but the copy itself, for §7.1 (legacy update chain must never touch this file).
$initialModelTimestamp = (Get-Item $workingModelPath).LastWriteTime
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
# Build once per PowerShell session: the first launch for a given project/configuration/platform
# builds, every later launch in the same session (every test, every runner under Run-AllTests.ps1)
# reuses it. Kept in a $global: table because this script runs in a child scope. Pass -ForceBuild,
# or start a new PowerShell session, after changing source code.
if (-not $global:EwEBuiltProjects) { $global:EwEBuiltProjects = @{} }
$buildKey = "$vbproj|$Configuration|$Platform".ToLowerInvariant()
if ($global:EwEBuiltProjects[$buildKey] -and -not $ForceBuild) {
    Write-Host "Already built in this session ($Configuration|$Platform) - skipping the build (use -ForceBuild to rebuild)."
} else {
    dotnet build $vbproj -c $Configuration -p:Platform=$Platform | ForEach-Object { Write-Host $_ }
    if ($LASTEXITCODE -ne 0) {
        throw "Build failed (dotnet build exit code $LASTEXITCODE)."
    }
    $global:EwEBuiltProjects[$buildKey] = $true
}

$exePath = Join-Path $RepoRoot "Sources\ScientificInterface\bin\$Platform\$Configuration\net48\ScientificInterface.exe"
if (-not (Test-Path $exePath)) {
    throw "Built exe not found at the expected path: $exePath"
}

. (Join-Path $PSScriptRoot "TestHelpers.ps1")

# Snapshotted BEFORE the process even starts, not after Wait-ForMainWindow returns: staging happens
# DURING that same startup sequence (as part of opening the model), so a baseline taken afterward
# could already include this instance's own staged copy - making it invisible to a later "what's
# new since baseline" check. Confirmed this exact ordering bug once already.
$baseName = [IO.Path]::GetFileNameWithoutExtension($workingModelPath)
$copiesBefore = Get-StagedCopies | ForEach-Object { $_.FullName }

Write-Host "Starting: $exePath `"$workingModelPath`""
$proc = Start-Process -FilePath $exePath -ArgumentList "`"$workingModelPath`"" -PassThru

# A "your model database will be updated to a newer version" confirmation can appear BEFORE
# MainWindowHandle is even populated - a loop that only waits on that handle would hang/time out
# with nobody ever clicking it. Same generic dialog shape as the lock-contention dialog (confirmed
# via Accessibility Insights: Name="Ecopath with Ecosim", ControlType=Window, with Yes/No buttons),
# watched for here since the usual FromHandle(MainWindowHandle) approach isn't available yet -
# searching the whole desktop, scoped to this process's own PID, instead.
if ($SkipMainWindowWait) {
    Write-Host "Skipping the main-window wait (-SkipMainWindowWait) - returning right after launch; the caller does its own waiting by window name." -ForegroundColor DarkGray
    Write-Host "Ready (unconfirmed) - PID $($proc.Id), model folder: $WorkFolder" -ForegroundColor Green
    $stagedCopyPath = $null
} else {
    Write-Host "Waiting for the main window to appear (watching for a database-upgrade prompt too)..."
    Wait-ForMainWindow -Proc $proc -TimeoutSeconds $MainWindowTimeoutSeconds

    Write-Host "Ready - PID $($proc.Id), model folder: $WorkFolder" -ForegroundColor Green

    # Confirmed NEW since the baseline taken before launch - not re-derived later by matching
    # Get-StagedCopies against a filename pattern. CONFIRMED that re-matching by name later is
    # unreliable: if an old, unrelated staged copy with the same fixture name is still sitting in
    # the folder (e.g. a leftover from an earlier run whose own process was never fully cleaned
    # up), a later name-based lookup can silently pick THAT one instead of this instance's own -
    # the exact same GUID reappeared across two separate runs, which is what exposed this.
    $stagedCopyPath = $null
    $stagedDeadline = (Get-Date).AddSeconds(30)
    while ($null -eq $stagedCopyPath -and (Get-Date) -lt $stagedDeadline) {
        $newMatch = Get-StagedCopies | Where-Object { $_.Name -like "Temp$baseName*" -and $_.FullName -notin $copiesBefore } | Select-Object -First 1
        if ($newMatch) { $stagedCopyPath = $newMatch.FullName }
        else { Start-Sleep -Milliseconds 300 }
    }
    if (-not $stagedCopyPath) {
        Write-Host "Could not confirm a NEW staged copy for '$baseName' within 30 seconds - StagedCopyPath will be `$null." -ForegroundColor Yellow
    }
}

return [PSCustomObject]@{
    Process                = $proc
    ModelPath              = $workingModelPath
    InitialModelTimestamp  = $initialModelTimestamp
    StagedCopyPath         = $stagedCopyPath
}
