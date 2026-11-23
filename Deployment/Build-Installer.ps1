<#
.SYNOPSIS
  Builds ScientificInterface (Release) and compiles the Inno Setup installers.
  By default both x86 and x64 are built; installers go to Deployment\Output\x86 and \x64.
.PARAMETER Platform   Filter: x86, x64 or both (default).
.PARAMETER SemVersion Overrides the iss SemVersion default.
.PARAMETER ForceBuild Rebuild ScientificInterface even if already built this session.
.PARAMETER Ide        Build only, then open the iss in the Inno Setup IDE (it ignores /D defines,
                      so the IDE compiles whatever Compile64Bit the iss defaults to).
.PARAMETER IsccPath   Explicit path to ISCC.exe (or Compil32.exe with -Ide).
.EXAMPLE
  .\Deployment\Build-Installer.ps1                    # x86 + x64
  .\Deployment\Build-Installer.ps1 -Platform x64      # x64 only
#>
param(
    [string]$RepoRoot = (Split-Path $PSScriptRoot -Parent),
    [ValidateSet('x86', 'x64')][string[]]$Platform = @('x86', 'x64'),
    [string]$Configuration = 'Release',
    [string]$SemVersion,
    [switch]$ForceBuild,
    [switch]$Ide,
    [string]$IsccPath
)
$ErrorActionPreference = 'Stop'
$Platform = $Platform | Select-Object -Unique

$proj   = Join-Path $RepoRoot 'Sources\ScientificInterface\ScientificInterface.vbproj'
$iss    = Join-Path $RepoRoot 'Deployment\EwE6_relpath.iss'
foreach ($p in $proj, $iss) { if (-not (Test-Path $p)) { throw "Not found: $p" } }

if (-not $global:EwEBuiltProjects) { $global:EwEBuiltProjects = @{} }  # shared with Start-TestInstance.ps1

# EwEImportHelper has its own release process (committed binaries, see Tools\EwEImportHelper\README.md);
# it is never built here. x64 builds only copy it from Publish\win-x86, so just check it is there.
$helperExe = Join-Path $RepoRoot 'Tools\EwEImportHelper\Publish\win-x86\EwEImportHelper.exe'
if ($Platform -contains 'x64' -and -not (Test-Path $helperExe)) {
    throw "Missing $helperExe. Publish it per Tools\EwEImportHelper\README.md (or pull the committed binaries)."
}

# Locate Inno Setup
$exe = if ($Ide) { 'Compil32.exe' } else { 'ISCC.exe' }
if (-not $IsccPath) {
    $roots = @(${env:ProgramFiles(x86)}, $env:ProgramFiles, "$env:LocalAppData\Programs") | Where-Object { $_ }
    $IsccPath = $roots | ForEach-Object {
        Get-ChildItem $_ -Directory -Filter 'Inno Setup*' -ErrorAction SilentlyContinue |
            Sort-Object Name -Descending | ForEach-Object { Join-Path $_.FullName $exe }
    } | Where-Object { Test-Path $_ } | Select-Object -First 1
}
if (-not $IsccPath -or -not (Test-Path $IsccPath)) { throw "$exe not found. Install Inno Setup or pass -IsccPath." }

# Build and package each platform
foreach ($plat in $Platform) {
    $out = Join-Path $RepoRoot "Sources\ScientificInterface\bin\$plat\$Configuration\net48"
    $key = "$proj|$Configuration|$plat".ToLower()
    if ($ForceBuild -or -not $global:EwEBuiltProjects[$key] -or -not (Test-Path (Join-Path $out 'ScientificInterface.exe'))) {
        Write-Host "Building $Configuration|$plat ..." -ForegroundColor Cyan
        & dotnet build $proj -c $Configuration "-p:Platform=$plat"
        if ($LASTEXITCODE -ne 0) { throw "dotnet build ($plat) failed ($LASTEXITCODE)" }
        $global:EwEBuiltProjects[$key] = $true
    } else {
        Write-Host "Build $plat already done this session (use -ForceBuild to redo)." -ForegroundColor DarkGray
    }

    if (-not (Test-Path (Join-Path $out 'ScientificInterface.exe'))) { throw "ScientificInterface.exe missing in $out" }
    if (-not (Get-ChildItem $out -Filter *.dll -ErrorAction SilentlyContinue)) { throw "No DLLs in $out" }
    if ($plat -eq 'x64' -and -not (Test-Path (Join-Path $out 'EwEImportHelper\EwEImportHelper.exe'))) {
        throw "EwEImportHelper\EwEImportHelper.exe not in $out. Re-run with -ForceBuild to recopy it."
    }
    foreach ($t in 'mdb2sqlite', 'migrator') {
        if (-not (Test-Path (Join-Path $out $t))) { Write-Warning "[$plat] tool folder '$t' not in output (SQLite branch only)." }
    }

    if ($Ide) { continue }
    $outDir = Join-Path $RepoRoot "Deployment\Output\$plat"
    New-Item -ItemType Directory -Force $outDir | Out-Null
    $isccArgs = @("/DCompile64Bit=$(if ($plat -eq 'x64') { '1' } else { '0' })", "/O$outDir")
    if ($SemVersion) { $isccArgs += "/DSemVersion=$SemVersion" }
    $isccArgs += $iss
    Write-Host "Compiling installer ($plat) ..." -ForegroundColor Cyan
    & $IsccPath @isccArgs
    if ($LASTEXITCODE -ne 0) { throw "ISCC ($plat) failed ($LASTEXITCODE)" }
}

if ($Ide) {
    Write-Host "Opening $iss in the Inno Setup IDE ..." -ForegroundColor Cyan
    Start-Process $IsccPath -ArgumentList "`"$iss`""
    return
}
Write-Host "Done. Installers:" -ForegroundColor Green
foreach ($plat in $Platform) {
    Get-ChildItem (Join-Path $RepoRoot "Deployment\Output\$plat") -Filter *.exe | ForEach-Object { Write-Host "  $($_.FullName)" }
}
