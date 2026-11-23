<#
.SYNOPSIS
  Builds ScientificInterface (Release) and compiles the Inno Setup installer.
.PARAMETER Platform   x86 (32-bit installer, default) or x64 (64-bit installer).
.PARAMETER SemVersion Overrides the iss SemVersion default.
.PARAMETER ForceBuild Rebuild everything, including the x86 EwEImportHelper publish folder.
.PARAMETER Ide        Open the iss in the Inno Setup IDE instead of compiling with ISCC.
                      The IDE ignores /D defines, so x64 needs Compile64Bit edited in the iss.
.PARAMETER IsccPath   Explicit path to ISCC.exe (or Compil32.exe with -Ide).
.EXAMPLE
  .\Deployment\Build-Installer.ps1 -Platform x64
#>
param(
    [string]$RepoRoot = (Split-Path $PSScriptRoot -Parent),
    [ValidateSet('x86', 'x64')][string]$Platform = 'x86',
    [string]$Configuration = 'Release',
    [string]$SemVersion,
    [switch]$ForceBuild,
    [switch]$Ide,
    [string]$IsccPath
)
$ErrorActionPreference = 'Stop'

$proj   = Join-Path $RepoRoot 'Sources\ScientificInterface\ScientificInterface.vbproj'
$helper = Join-Path $RepoRoot 'Tools\EwEImportHelper\Source\EwEImportHelper.vbproj'
$pubDir = Join-Path $RepoRoot 'Tools\EwEImportHelper\Publish\win-x86'
$iss    = Join-Path $RepoRoot 'Deployment\EwE6_relpath.iss'
$out    = Join-Path $RepoRoot "Sources\ScientificInterface\bin\$Platform\$Configuration\net48"
foreach ($p in $proj, $helper, $iss) { if (-not (Test-Path $p)) { throw "Not found: $p" } }

if (-not $global:EwEBuiltProjects) { $global:EwEBuiltProjects = @{} }  # shared with Start-TestInstance.ps1

# 1. EwEImportHelper (x64 builds only, as in ScientificInterface.vbproj): standalone x86 tool, published by copy into Tools\...\Publish\win-x86
$helperExe = Join-Path $pubDir 'EwEImportHelper.exe'
$helperNew = $false
if ($Platform -eq 'x64' -and ($ForceBuild -or -not (Test-Path $helperExe))) {
    Write-Host "Building EwEImportHelper (x86) ..." -ForegroundColor Cyan
    & dotnet build $helper -c $Configuration "-p:Platform=x86"
    if ($LASTEXITCODE -ne 0) { throw "EwEImportHelper build failed ($LASTEXITCODE)" }
    $hOut = Join-Path $RepoRoot "Tools\EwEImportHelper\Source\bin\x86\$Configuration\net48"
    if (-not (Test-Path (Join-Path $hOut 'EwEImportHelper.exe'))) { throw "EwEImportHelper.exe missing in $hOut" }
    New-Item -ItemType Directory -Force $pubDir | Out-Null
    Get-ChildItem $hOut -File | Where-Object { $_.Extension -ne '.pdb' } |
        Copy-Item -Destination $pubDir -Force
    $helperNew = $true
}

# 2. ScientificInterface (rebuilt if the helper was just published, as it copies from that folder)
$key = "$proj|$Configuration|$Platform".ToLower()
if ($ForceBuild -or $helperNew -or -not $global:EwEBuiltProjects[$key] -or
    -not (Test-Path (Join-Path $out 'ScientificInterface.exe'))) {
    Write-Host "Building $Configuration|$Platform ..." -ForegroundColor Cyan
    & dotnet build $proj -c $Configuration "-p:Platform=$Platform"
    if ($LASTEXITCODE -ne 0) { throw "dotnet build failed ($LASTEXITCODE)" }
    $global:EwEBuiltProjects[$key] = $true
} else {
    Write-Host "Build already done this session (use -ForceBuild to redo)." -ForegroundColor DarkGray
}

if (-not (Test-Path (Join-Path $out 'ScientificInterface.exe'))) { throw "ScientificInterface.exe missing in $out" }
if (-not (Get-ChildItem $out -Filter *.dll -ErrorAction SilentlyContinue)) { throw "No DLLs in $out" }
if ($Platform -eq 'x64' -and -not (Test-Path (Join-Path $out 'EwEImportHelper\EwEImportHelper.exe'))) {
    throw "EwEImportHelper\EwEImportHelper.exe not in $out. Re-run with -ForceBuild."
}
foreach ($t in 'mdb2sqlite', 'migrator') {
    if (-not (Test-Path (Join-Path $out $t))) { Write-Warning "Tool folder '$t' not found in output (SQLite branch only)." }
}

# 3. Inno Setup
$exe = if ($Ide) { 'Compil32.exe' } else { 'ISCC.exe' }
if (-not $IsccPath) {
    $roots = @(${env:ProgramFiles(x86)}, $env:ProgramFiles, "$env:LocalAppData\Programs") | Where-Object { $_ }
    $IsccPath = $roots | ForEach-Object {
        Get-ChildItem $_ -Directory -Filter 'Inno Setup*' -ErrorAction SilentlyContinue |
            Sort-Object Name -Descending | ForEach-Object { Join-Path $_.FullName $exe }
    } | Where-Object { Test-Path $_ } | Select-Object -First 1
}
if (-not $IsccPath -or -not (Test-Path $IsccPath)) { throw "$exe not found. Install Inno Setup or pass -IsccPath." }

if ($Ide) {
    Write-Host "Opening $iss in the Inno Setup IDE ..." -ForegroundColor Cyan
    Start-Process $IsccPath -ArgumentList "`"$iss`""
    return
}

$isccArgs = @("/DCompile64Bit=$(if ($Platform -eq 'x64') { '1' } else { '0' })")
if ($SemVersion) { $isccArgs += "/DSemVersion=$SemVersion" }
$isccArgs += $iss
Write-Host "Compiling installer ($Platform) ..." -ForegroundColor Cyan
& $IsccPath @isccArgs
if ($LASTEXITCODE -ne 0) { throw "ISCC failed ($LASTEXITCODE)" }
Write-Host "Done. Installer is in the iss output folder (default: Deployment\Output)." -ForegroundColor Green
