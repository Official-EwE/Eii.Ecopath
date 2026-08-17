<#
.SYNOPSIS
    Convert .mdb or .eweaccdb files to .ewesqlite using mdbtools-win.
.PARAMETER generateExe
	If specified, generates mdb2sqlite.exe using ps2exe.
.PARAMETER inDatabase
    Path to the .mdb or .eweaccdb file to convert.
.PARAMETER outDatabase
    Path to the output .ewesqlite file.
.PARAMETER CleanCache
    If specified, removes the .Cache folder (downloaded/installed mdbtools,
    and any leftover conversion.sql/sqlite_error.txt temp files) and exits,
    without performing a conversion. Use this to force a fresh mdbtools
    download/install on the next run, or to clean up after troubleshooting.
.EXAMPLE
    .\mdb2sqlite.ps1 -inDatabase "C:\path\to\input.eweaccdb"
    On Linux: ./run-ps1.sh mdb2sqlite.ps1 -inDatabase "C:\path\to\input.eweaccdb"
    Converts the specified .mdb file to a .ewesqlite file using the same name but with .ewesqlite extension.    
.EXAMPLE
    .\mdb2sqlite.ps1 -inDatabase "C:\path\to\input.eweaccdb" -outDatabase "C:\path\to\output.ewesqlite"
    On Linux: ./run-ps1.sh mdb2sqlite.ps1 -inDatabase "C:\path\to\input.eweaccdb" -outDatabase "C:\path\to\output.ewesqlite"
    Converts the specified .mdb file to a .ewesqlite file.
.EXAMPLE
    .\mdb2sqlite.ps1 -generateExe
    This is only for Windows users.
    Generates the mdb2sqlite.exe executable from this script.
.EXAMPLE
    .\mdb2sqlite.ps1 -CleanCache
    Removes the .Cache folder and exits.
.LINK
    https://www.sqlite.org/download.html - Source of SQLite tools
    https://github.com/mdbtools/mdbtools - Source of mdbtools
    https://github.com/mdbtools/mdbtools/tree/dev/doc - Documentation for mdbtools
    https://github.com/lsgunth/mdbtools-win - Windows build of mdbtools
.NOTES
    Exit codes:
      0 - Success.
      1 - General failure (missing/invalid arguments, missing sqlite3.exe,
          overwrite declined, sqlite3 import failure).
      3 - Network failure: unable to download/install mdbtools (Windows:
          mdbtools-win zip download; Linux: git clone / apt update / apt
          upgrade / apt install). Requires an internet connection for
          first-time setup on a given machine (subsequent runs reuse the
          cached mdbtools installation). On Linux this is only used from the
          shell directly, so it's mainly for consistency with the Windows
          behavior and for this documentation, rather than anything checking
          it programmatically.
#>

param (
    [string]$inDatabase,
    [string]$outDatabase = $null,
	[switch]$generateExe,
    [switch]$CleanCache
)

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

function Restore-Ps2Exe {
	if (-not (Get-Module -ListAvailable -Name ps2exe)) {
		Write-Host "ps2exe module not found. Installing..."
		Install-Module ps2exe -Force -Scope CurrentUser
	} else {
		Write-Host "ps2exe module is already installed."
	}
}

function Install-Mdbtools {
    param (
        [string]$scriptDir
    )
    if ($IsWindows -or $env:OS -eq 'Windows_NT') {
        $cacheFolder = Join-Path $scriptDir '.Cache'
        $targetFolder = Join-Path $cacheFolder 'mdbtools-win'
        if (-not (Test-Path $targetFolder)) {
            Write-Host "Setting up .Cache/mdbtools-win..."
            if (-not (Test-Path $cacheFolder)) {
                New-Item -ItemType Directory -Path $cacheFolder | Out-Null
            }
            $zipUrl = 'https://github.com/Official-EwE/mdbtools-win/archive/refs/heads/master.zip'
            $zipPath = Join-Path $cacheFolder 'master.zip'
            try {
                Write-Host "Downloading mdbtools-win master.zip..."
                Invoke-WebRequest -Uri $zipUrl -OutFile $zipPath -ErrorAction Stop
            } catch {
                Show-SimpleError "Unable to download mdbtools-win - an internet connection is required for first-time setup. ($($_.Exception.Message))"
                exit 3
            }
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
        # Add $targetFolder to PATH for this session
        $env:PATH = "$targetFolder;$env:PATH"
        Write-Host "Added $targetFolder to PATH."
    } else { # Non-Windows (Linux or MacOS)
        Write-Host "Detected Linux. Checking for mdbtools..."
        $mdbtoolsInstalled = $null -ne (Get-Command mdb-schema -ErrorAction SilentlyContinue)
        if (-not $mdbtoolsInstalled) {
            # we need to build from source as apt-get version is too old, we need v1.0.1 or later
            Write-Host "mdbtools not found. Installing from source..."
            Remove-Item -Recurse -Force -ErrorAction SilentlyContinue ./.Cache/mdbtools
            sudo git clone https://github.com/mdbtools/mdbtools.git ./.Cache/mdbtools
            if ($LASTEXITCODE -ne 0) {
                Show-SimpleError "Unable to clone mdbtools - an internet connection is required for first-time setup."
                exit 3
            }
            sudo apt update
            if ($LASTEXITCODE -ne 0) {
                Show-SimpleError "Unable to run 'apt update' - an internet connection is required for first-time setup."
                exit 3
            }
            sudo apt upgrade -y
            if ($LASTEXITCODE -ne 0) {
                Show-SimpleError "Unable to run 'apt upgrade' - an internet connection is required for first-time setup."
                exit 3
            }
            sudo apt install -y libtool automake autoconf gettext pkg-config bison flex libglib2.0-* make
            if ($LASTEXITCODE -ne 0) {
                Show-SimpleError "Unable to run 'apt install' - an internet connection is required for first-time setup."
                exit 3
            }
            Set-Location ./.Cache/mdbtools
            sudo autoreconf -i -f
            sudo ./configure --disable-dependency-trackingmd
            sudo make
            sudo make install
            sudo ldconfig
            Set-Location $scriptDir
        } else {
            Write-Host "mdbtools is already installed."
        }
    }
}

function Install-Sqlite {
    param (
        [string]$scriptDir
    )
    if ($IsWindows -or $env:OS -eq 'Windows_NT') {
        $sqliteExe = Join-Path $scriptDir 'sqlite3.exe'
        if (-not (Test-Path $sqliteExe -PathType Leaf)) {
            Show-SimpleError "sqlite.exe not found in $scriptDir. Please download sqlite.exe and place it in the script directory."
            exit 1
        }
    } else { # Non-Windows (assuming Linux-based and supporting "apt"  so Ubuntu/Debian)
        Write-Host "Detected Linux. Checking for sqlite3..."
        $sqliteInstalled = $null -ne (Get-Command sqlite3 -ErrorAction SilentlyContinue)
        if (-not $sqliteInstalled) {
            Write-Host "sqlite3 not found. Installing via apt..."
            sudo apt update
            sudo apt install -y sqlite3
        } else {
            Write-Host "sqlite3 is already installed."
        }
    }    
}

function Convert-MdbToSqlite {
    param (
        [string]$scriptDir,
        [string]$inDatabase,
        [string]$outDatabase
    )

    Install-Mdbtools -scriptDir $scriptDir
    Install-Sqlite -scriptDir $scriptDir

    $cacheFolder = Join-Path $scriptDir '.Cache'
    $conversionSql = Join-Path $cacheFolder 'conversion.sql'

    # Use executables from PATH, no .exe extension
    $schemaExe = "mdb-schema"
    $tablesExe = "mdb-tables"
    $exportExe = "mdb-export"

    # Check for required mdbtools executables in PATH
    $missingExe = @()
    foreach ($exe in @($schemaExe, $tablesExe, $exportExe)) {
        if (-not (Get-Command $exe -ErrorAction SilentlyContinue)) {
            $missingExe += $exe
        }
    }
    if ($missingExe.Count -gt 0) {
        Write-Error "Missing required mdbtools executables in PATH: $($missingExe -join ', ')"
        return
    }

    Write-Host "Starting conversion: $inDatabase to $outDatabase"
    Write-Host "Generating conversion.sql script..."
    "BEGIN;" | Set-Content -Path $conversionSql

    Write-Host "Extracting schema using mdb-schema..."
    $schemaOut = & $schemaExe --no-not-null $inDatabase "sqlite"
    Add-Content -Path $conversionSql -Value $schemaOut

    Write-Host "Getting table names using mdb-tables..."
    $tablesOut = & $tablesExe -1 $inDatabase
    $tables = $tablesOut -split "`r?`n" | Where-Object { $_ -ne "" }
    Write-Host "Found tables: $($tables -join ', ')"

    Write-Host "Exporting tables using mdb-export..."
    foreach ($table in $tables) {
        Write-Host "Exporting table: $table"
        $tableSql = & $exportExe -q "'" -I sqlite $inDatabase $table
        # Replace inf and -inf with NULL in SQL insert statements
        $tableSql = $tableSql -replace '(\(|,)-?inf(?=,|\))', '$1NULL'
        Add-Content -Path $conversionSql -Value $tableSql
    }

    Write-Host "Finalizing conversion.sql script..."
    "COMMIT;" | Add-Content -Path $conversionSql

	Write-Host "Removing empty primary keys..."
	$inputFile = ""
	$tempFile = [System.IO.Path]::GetTempFileName()
	Get-Content $conversionSql | ForEach-Object {
		if ($_ -notmatch ', PRIMARY KEY \(\)') {
			$_
		}
	} | Set-Content $tempFile
	Move-Item $tempFile $conversionSql -Force

    Write-Host "Creating SQLite database and importing data..."
    $errorFile = Join-Path $cacheFolder 'sqlite_error.txt'
    if ($IsWindows -or $env:OS -eq 'Windows_NT') {
        $importCmd = "cmd /c `"sqlite3 `"`"$outDatabase`"`" < `"`"$conversionSql`"`" 2> `"`"$errorFile`"`" `" "
    } else {
        $importCmd = "/bin/bash -c 'sqlite3 `"$outDatabase`" < `"$conversionSql`" 2> `"$errorFile`"'"
    }

    Write-Host "Running: $importCmd"    
    Invoke-Expression $importCmd
    $errorText = if (Test-Path $errorFile) { Get-Content $errorFile -Raw } else { '' }
    if ($errorText) {
        Show-SimpleError "sqlite3 import failed: $errorText"
        exit 1
    } else {
        Write-Host "Conversion complete: $outDatabase created."
    }
}

# as documented by https://github.com/MScholtes/PS2EXE?tab=readme-ov-file#script-variables
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
    Show-Warning "Warning: $scriptFile does not exist in the current directory. -generateExe will not work from here."
}
Write-Host "Script dir: $scriptDir"

if ($generateExe) {
    if (-Not ($IsWindows -or ($env:OS -eq 'Windows_NT'))) {
        Show-SimpleError "Error: -generateExe option is only supported on Windows. Found: $env:OS"
        exit 1
    }
    if ($MyInvocation.MyCommand.CommandType -ne "ExternalScript") {
        Show-SimpleError "Error: -generateExe must be run via the .ps1 script, not the compiled .exe - a running exe cannot overwrite its own file. Use: powershell -ExecutionPolicy Bypass -File `"$scriptFile`" -generateExe"
        exit 1
    }
	Restore-Ps2Exe
	Write-Host "Running Invoke-ps2exe to generate mdb2sqlite.exe..."
    $inputFile = $scriptFile
    $outputFile = [System.IO.Path]::ChangeExtension($inputFile, ".exe")
    Invoke-ps2exe -inputFile $inputFile -outputFile $outputFile
    exit 0
}

if ($CleanCache) {
    $cacheFolder = Join-Path $scriptDir '.Cache'
    if (Test-Path $cacheFolder) {
        Write-Host "Removing $cacheFolder..."
        Remove-Item -Recurse -Force $cacheFolder
        Write-Host "Cache cleared."
    } else {
        Write-Host "Nothing to clean - $cacheFolder does not exist."
    }
    exit 0
}

Show-Notice "Add command line argument -? to show the full help (-? -examples / -? -detailed / -? -full for more). Use -CleanCache to clear the .Cache folder."

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
    exit 1
}

# If outDatabase is not set, use inDatabase path with .ewesqlite extension
if (-not $outDatabase) {
    $outDatabase = [System.IO.Path]::ChangeExtension($inDatabase, ".ewesqlite")
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
