<#
.SYNOPSIS
    Seeds the __EFMigrationsHistory table in a SQLite database, marking one
    or more already-generated EF Core migrations as already applied.
.DESCRIPTION
    For adopting an existing database (e.g. EwE6.sqlite, whose schema was
    already scaffolded FROM a real populated database) into EF Core
    migrations, without Database.Migrate() trying to replay each migration's
    CREATE TABLE statements against a schema that's already physically there.

    Reads MigrationId and ProductVersion directly out of each migration's
    generated *.Designer.cs file (via the [Migration("...")] attribute and
    the "ProductVersion" model annotation), rather than requiring them to be
    typed in by hand - so this can't drift out of sync with what was actually
    generated.

    Safe to re-run: uses CREATE TABLE IF NOT EXISTS and INSERT OR IGNORE.
.PARAMETER SqliteFile
    Path to the .sqlite file to seed. If not specified, defaults to
    EwE6.sqlite in this repo's own Sources\EwECore\Resources folder,
    relative to this script's own location (assumes this script is running
    from Eii.Ecopath\Tools\seed-ef-migrations-history).
.PARAMETER MigrationsFolder
    Path to the Migrations folder containing the generated migration files.
    If not specified, assumes a sibling Eii.Ecopath.Storage repo folder next
    to Eii.Ecopath (this script's own repo), i.e.
    ..\..\..\Eii.Ecopath.Storage\Eii.Ecopath.Storage\dotnet\Migrations
    relative to this script. If that assumed layout isn't present, an error
    is shown asking you to specify -MigrationsFolder explicitly rather than
    guessing further.
.PARAMETER Sqlite3Path
    Path to the sqlite3 executable. Not assumed to be on PATH - if not
    specified, defaults to the copy shipped alongside the mdb2sqlite tool
    (..\mdb2sqlite\sqlite3.exe, relative to this script). If that's not
    present either, an error is shown asking you to specify -Sqlite3Path
    explicitly.
.EXAMPLE
    .\seed-ef-migrations-history.ps1
    Uses both default locations - EwE6.sqlite in this repo, and a sibling
    Eii.Ecopath.Storage repo's dotnet\Migrations folder.
.EXAMPLE
    .\seed-ef-migrations-history.ps1 -MigrationsFolder "D:\SomewhereElse\Eii.Ecopath.Storage\dotnet\Migrations"
    Overrides the migrations location, e.g. if Eii.Ecopath.Storage isn't a
    sibling folder on this machine.
#>
param(
    [string]$SqliteFile,
    [string]$MigrationsFolder,
    [string]$Sqlite3Path
)

# Default SqliteFile: this repo's own EwE6.sqlite resource, relative to
# this script's own location (Eii.Ecopath\Tools\seed-ef-migrations-history),
# regardless of the current working directory it's invoked from.
if (-not $SqliteFile) {
    $SqliteFile = Join-Path $PSScriptRoot "..\..\Sources\EwECore\Resources\EwE6.sqlite"
    Write-Host "No -SqliteFile specified, defaulting to: $SqliteFile"
}

# Default MigrationsFolder: assumes a sibling Eii.Ecopath.Storage repo
# folder next to Eii.Ecopath (this script's own repo), with the usual
# Eii.Ecopath.Storage\Eii.Ecopath.Storage\dotnet\Migrations layout.
if (-not $MigrationsFolder) {
    $MigrationsFolder = Join-Path $PSScriptRoot "..\..\..\Eii.Ecopath.Storage\Eii.Ecopath.Storage\dotnet\Migrations"
    Write-Host "No -MigrationsFolder specified, defaulting to: $MigrationsFolder"
}

if (-not (Test-Path $SqliteFile -PathType Leaf)) {
    Write-Error "SQLite file not found: $SqliteFile`nIf EwE6.sqlite isn't in the expected location, specify it explicitly with -SqliteFile."
    exit 1
}
if (-not (Test-Path $MigrationsFolder -PathType Container)) {
    Write-Error "Migrations folder not found: $MigrationsFolder`nThis script assumes Eii.Ecopath.Storage sits as a sibling folder next to Eii.Ecopath. If your folder layout is different, specify the correct location explicitly with -MigrationsFolder."
    exit 1
}

# Default Sqlite3Path: sqlite3.exe is not assumed to be on PATH - it ships
# alongside the mdb2sqlite tool (a sibling folder under Tools\), so default
# to that copy rather than requiring a separate install.
if (-not $Sqlite3Path) {
    $Sqlite3Path = Join-Path $PSScriptRoot "..\mdb2sqlite\sqlite3.exe"
    Write-Host "No -Sqlite3Path specified, defaulting to: $Sqlite3Path"
}

if (-not (Test-Path $Sqlite3Path -PathType Leaf)) {
    Write-Error "sqlite3 executable not found: $Sqlite3Path`nSpecify the correct location explicitly with -Sqlite3Path."
    exit 1
}

# Migration files look like <timestamp>_<Name>.cs, with a matching
# <timestamp>_<Name>.Designer.cs holding the [Migration(...)] attribute and
# ProductVersion annotation. Exclude the *.Designer.cs files themselves and
# the *ModelSnapshot.cs file from this listing - we only want the main
# migration file names, then look up each one's own Designer.cs alongside it.
$migrationFiles = Get-ChildItem -Path $MigrationsFolder -Filter "*.cs" |
    Where-Object { $_.Name -notmatch "\.Designer\.cs$" -and $_.Name -notmatch "ModelSnapshot\.cs$" } |
    Sort-Object Name

if ($migrationFiles.Count -eq 0) {
    Write-Error "No migration files found in $MigrationsFolder"
    exit 1
}

Write-Host "Found $($migrationFiles.Count) migration(s):"
$migrationFiles | ForEach-Object { Write-Host "  - $($_.Name)" }
Write-Host ""

$rows = @()
foreach ($file in $migrationFiles) {
    $designerFile = Join-Path $file.Directory.FullName ($file.BaseName + ".Designer.cs")
    if (-not (Test-Path $designerFile -PathType Leaf)) {
        Write-Warning "No matching Designer.cs found for $($file.Name), skipping"
        continue
    }
    $designerContent = Get-Content $designerFile -Raw

    $migrationIdMatch = [regex]::Match($designerContent, '\[Migration\("([^"]+)"\)\]')
    if (-not $migrationIdMatch.Success) {
        Write-Warning "Could not find [Migration(...)] attribute in $designerFile, skipping"
        continue
    }
    $migrationId = $migrationIdMatch.Groups[1].Value

    $productVersionMatch = [regex]::Match($designerContent, 'ProductVersion",\s*"([^"]+)"')
    if (-not $productVersionMatch.Success) {
        Write-Warning "Could not find ProductVersion annotation in $designerFile - defaulting to 'unknown'"
        $productVersion = "unknown"
    } else {
        $productVersion = $productVersionMatch.Groups[1].Value
    }

    $rows += [PSCustomObject]@{ MigrationId = $migrationId; ProductVersion = $productVersion }
}

if ($rows.Count -eq 0) {
    Write-Error "No valid migrations found to seed - nothing was written."
    exit 1
}

Write-Host "Will seed the following row(s) into __EFMigrationsHistory in $SqliteFile :"
$rows | Format-Table -AutoSize

$sql = @"
CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" TEXT NOT NULL CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY,
    "ProductVersion" TEXT NOT NULL
);
"@

foreach ($row in $rows) {
    $mid = $row.MigrationId.Replace("'", "''")
    $pv = $row.ProductVersion.Replace("'", "''")
    $sql += "`nINSERT OR IGNORE INTO ""__EFMigrationsHistory"" (""MigrationId"", ""ProductVersion"") VALUES ('$mid', '$pv');"
}

$tempSqlFile = [System.IO.Path]::GetTempFileName()
try {
    Set-Content -Path $tempSqlFile -Value $sql

    Write-Host ""
    Write-Host "Running against $SqliteFile..."
    Get-Content $tempSqlFile | & $Sqlite3Path $SqliteFile
    if ($LASTEXITCODE -ne 0) {
        Write-Error "sqlite3 exited with code $LASTEXITCODE"
        exit 1
    }
} finally {
    Remove-Item $tempSqlFile -ErrorAction SilentlyContinue
}

Write-Host ""
Write-Host "Done. Verify with:"
Write-Host "  $Sqlite3Path `"$SqliteFile`" `"SELECT * FROM __EFMigrationsHistory;`""
