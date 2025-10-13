<#
.SYNOPSIS
    Convert .mdb or .eweaccdb files to .sqlite using mdbtools-win.
.PARAMETER generateExe
	If specified, generates mdb2sqlite.exe using ps2exe.
.PARAMETER inDatabase
    Path to the .mdb or .eweaccdb file to convert.
.PARAMETER outDatabase
    Path to the output .sqlite file.
.EXAMPLE
    .\mdb2sqlite.ps1 -inDatabase "C:\path\to\input.eweaccdb"
    Converts the specified .mdb file to a .sqlite file using the same name but with .sqlite extension.
.EXAMPLE
    .\mdb2sqlite.ps1 -inDatabase "C:\path\to\input.eweaccdb" -outDatabase "C:\path\to\output.sqlite"
    Converts the specified .mdb file to a .sqlite file.
.EXAMPLE
    .\mdb2sqlite.ps1 -generateExe
    Generates the mdb2sqlite.exe executable from this script.
.LINK
    https://github.com/mdbtools/mdbtools - Source of mdbtools
    https://github.com/mdbtools/mdbtools/tree/dev/doc - Documentation for mdbtools
    https://github.com/lsgunth/mdbtools-win - Windows build of mdbtools
#>

param (
    [string]$inDatabase,
    [string]$outDatabase = $null,
	[switch]$generateExe    
)

function Restore-Ps2Exe {
	if (-not (Get-Module -ListAvailable -Name ps2exe)) {
		Write-Host "ps2exe module not found. Installing..."
		Install-Module ps2exe -Force -Scope CurrentUser
	} else {
		Write-Host "ps2exe module is already installed."
	}
}

function Ensure-MdbtoolsWinCache {
    param (
        [string]$scriptDir
    )      
	$cacheFolder = Join-Path $scriptDir '.Cache'
	$targetFolder = Join-Path $cacheFolder 'mdbtools-win'
	if (-not (Test-Path $targetFolder)) {
		Write-Host "Setting up .Cache/mdbtools-win..."
		if (-not (Test-Path $cacheFolder)) {
			New-Item -ItemType Directory -Path $cacheFolder | Out-Null
		}
		$zipUrl = 'https://github.com/lsgunth/mdbtools-win/archive/refs/heads/master.zip'
		$zipPath = Join-Path $cacheFolder 'master.zip'
		Write-Host "Downloading mdbtools-win master.zip..."
		Invoke-WebRequest -Uri $zipUrl -OutFile $zipPath
		Write-Host "Extracting master.zip..."
		Add-Type -AssemblyName System.IO.Compression.FileSystem
		[System.IO.Compression.ZipFile]::ExtractToDirectory($zipPath, $cacheFolder)
		Remove-Item $zipPath
		$extractedFolder = Join-Path $cacheFolder 'mdbtools-win-master'
		if (Test-Path $extractedFolder) {
			Rename-Item -Path $extractedFolder -NewName 'mdbtools-win'
			Write-Host "Renamed mdbtools-win-master to mdbtools-win."
		} else {
			Write-Host "Extraction failed: mdbtools-win-master folder not found."
		}
	}
}

function Convert-MdbToSqlite {
    param (
        [string]$scriptDir,
        [string]$inDatabase,
        [string]$outDatabase
    )

    Ensure-MdbtoolsWinCache -scriptDir $scriptDir

    $cacheFolder = Join-Path $scriptDir '.Cache'
    $conversionSql = Join-Path $cacheFolder 'conversion.sql'
    $mdbtoolsFolder = Join-Path $cacheFolder 'mdbtools-win'

    $schemaExe = Join-Path $mdbtoolsFolder 'mdb-schema.exe'
    $tablesExe = Join-Path $mdbtoolsFolder 'mdb-tables.exe'
    $exportExe = Join-Path $mdbtoolsFolder 'mdb-export.exe'

    # Check for required mdbtools executables
    $missingExe = @()
    foreach ($exe in @($schemaExe, $tablesExe, $exportExe)) {
        if (-not (Test-Path $exe)) {
            $missingExe += $exe
        }
    }
    if ($missingExe.Count -gt 0) {
        Write-Error "Missing required mdbtools executables: $($missingExe -join ', ')"
        return
    }

    Write-Host "Starting conversion: $inDatabase to $outDatabase"
    Write-Host "Generating conversion.sql script..."
    "BEGIN;" | Set-Content -Path $conversionSql

    Write-Host "Extracting schema using mdb-schema.exe..."
    $schemaOut = & $schemaExe $inDatabase "sqlite"
    Add-Content -Path $conversionSql -Value $schemaOut

    Write-Host "Getting table names using mdb-tables.exe..."
    $tablesOut = & $tablesExe -1 $inDatabase
    $tables = $tablesOut -split "`r?`n" | Where-Object { $_ -ne "" }
    Write-Host "Found tables: $($tables -join ', ')"

    Write-Host "Exporting tables using mdb-export.exe..."
    foreach ($table in $tables) {
        Write-Host "Exporting table: $table"
        $tableSql = & $exportExe -I sqlite $inDatabase $table
        # Replace inf and -inf with NULL in SQL insert statements
        $tableSql = $tableSql -replace '(\(|,)-?inf(?=,|\))', '$1NULL'
        Add-Content -Path $conversionSql -Value $tableSql
    }

    Write-Host "Finalizing conversion.sql script..."
    "COMMIT;" | Add-Content -Path $conversionSql

    Write-Host "Creating SQLite database and importing data..."
    $sqliteDll = Join-Path $scriptDir 'System.Data.SQLite.dll'
    $connectionString = "Data Source=$outDatabase;Version=3;"
    Add-Type -Path $sqliteDll
    $sqliteConnectionType = [System.Data.SQLite.SQLiteConnection]
    $conn = New-Object $sqliteConnectionType($connectionString)
    $conn.Open()
    $sql = Get-Content -Path $conversionSql -Raw
    $cmd = $conn.CreateCommand()
    $cmd.CommandText = $sql
    $hasError = $False
    try {
        $affectedRows = $cmd.ExecuteNonQuery()
        Write-Host "SQL import completed. Rows affected: $affectedRows"
    } catch {
        Show-SimpleError "Error executing SQL: $($_.Exception.Message)"
        $hasError = $True
    }
    $conn.Close()
    $conn.Dispose()
    $cmd = $null
    $conn = $null
    [GC]::Collect()
    [GC]::WaitForPendingFinalizers()
    if ($hasError) {
        exit 1
    }
    Write-Host "Conversion complete: $outDatabase created."
}


function Show-SimpleError {
    param ($message)
    Write-Host $message -ForegroundColor Red
}

function Show-Notice {
    param (
        [string]$message
    )
    # ANSI escape code for setting the background color to dark gray
    $darkGrayBackground = [char]27 + '[48;2;64;64;64m'
    # ANSI escape code for resetting the background and foreground colors to the default
    $resetColors = [char]27 + '[0m'
    # Display text with dark gray background and light gray foreground
    Write-Host "${darkGrayBackground}$message${resetColors}" -ForegroundColor Green
}

function Show-Warning {
    param (
        [string]$message
    )
    Write-Warning $message # -ForegroundColor Orange
}

if ($MyInvocation.MyCommand.CommandType -eq "ExternalScript") {
    $scriptDir = Split-Path -Parent -Path $MyInvocation.MyCommand.Definition
} else {
    $scriptDir = Split-Path -Parent -Path ([Environment]::GetCommandLineArgs()[0]) 
    if (!$scriptDir) {
        $scriptDir = "."
    }
}

$scriptFile = Join-Path $scriptDir 'mdb2sqlite.ps1'
if (-not (Test-Path $scriptFile -PathType Leaf)) {
    Show-Warning "Warning: $scriptFile does not exist in the current directory. Unable to use Get-Help."
}
Write-Host "Script dir: $scriptDir"

if ($generateExe) {
	Restore-Ps2Exe
	Write-Host "Running Invoke-ps2exe to generate mdb2sqlite.exe..."
    $inputFile = $scriptFile
    $outputFile = [System.IO.Path]::ChangeExtension($inputFile, ".exe")
    Invoke-ps2exe -inputFile $inputFile -outputFile $outputFile
    exit 0
}

Show-Notice "Add command line argument -Help to show the full help"
if ($args -contains "-Help" -or $args -contains "-?") {
    Get-Help $scriptFile -Full | Out-String
    exit 0
}


$errMsg = $False
if (-not $inDatabase) {
    Show-SimpleError "Input database file path (-inDatabase) is required."
    $errMsg = $True
}
if ($inDatabase) {
    if (-not (Test-Path $inDatabase -PathType Leaf)) {
        Show-SimpleError "Input database file '$inDatabase' does not exist."
        $errMsg = $True
    }
}
if ($errMsg) {
    Get-Help $scriptFile -Examples | Out-String
    exit 1
}

# If outDatabase is not set, use inDatabase path with .sqlite extension
if (-not $outDatabase) {
    $outDatabase = [System.IO.Path]::ChangeExtension($inDatabase, ".sqlite")
    Write-Host "Output database not specified. Using: $outDatabase"
}

if (Test-Path $outDatabase -PathType Leaf) {
    $prompt = "Output database file '$outDatabase' already exists. Overwrite? (y/n)"
    $response = Read-Host $prompt
    if ($response -notin @('y', 'Y')) {
        Show-Warning "Aborted: Output file will not be overwritten."
        exit 1
    }
    try {
        Remove-Item $outDatabase -ErrorAction Stop
    } catch {
        Show-SimpleError "Error deleting existing output file: $($_.Exception.Message)"
        exit 1
    }
}

Convert-MdbToSqlite -scriptDir $scriptDir -inDatabase $inDatabase -outDatabase $outDatabase
Write-Host "Conversion finished."
exit 0
