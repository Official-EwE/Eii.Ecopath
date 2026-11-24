<#
.SYNOPSIS
    Prerequisite check for the legacy-model conversion tests (Run-ConversionHandlingTests.ps1, and
    Run-AllTests.ps1 when it includes them). Dot-sourced; defines Confirm-ConversionTestPrerequisites.

.DESCRIPTION
    The conversion tests need real sample Ecopath 5 / legacy Access model files to exercise each
    detection branch (see HANDOFF.md's tag table) - files that are NOT committed to this repo, since
    they're real third-party model data, not test fixtures we own. This script is how a machine
    that doesn't have them yet gets asked for them, exactly once, and how later runs remember the
    answer without asking again every single time.

    State lives in a small, gitignored config file next to this script:
    `.conversion-test-config.txt` - two lines, `TestModelsFolder=<path>` and
    `SkipModels=<comma-separated filenames>`. Add this file to .gitignore; it's machine-specific and
    points at a path (and set of missing files) that only means something on the machine that wrote
    it.

    First run (no config yet, or a previously-configured folder that no longer exists): prompts for
    the folder. An empty answer means "skip ALL conversion tests on this machine" - recorded as
    TestModelsFolder= (blank) so every subsequent run bypasses silently without asking again. A real
    folder is checked against every required file name; anything missing is listed, and the user can
    either retype the folder to try again, or press Enter to accept skipping just those specific
    files (not everything) - this choice is what gets written to SkipModels.

    Every later run (config already has a valid folder) re-checks file presence fresh each time and
    silently updates SkipModels if it's out of date - e.g. the user later drops a previously-missing
    file into the folder, or removes one - without ever re-prompting interactively just because a
    file is still missing. It only prompts again if the folder itself has gone away.

    Search is recursive (Get-ChildItem -Recurse), so model files can live in subfolders of the
    configured folder, not just directly inside it.

.EXAMPLE
    . (Join-Path $PSScriptRoot "Test-ConversionPrerequisites.ps1")
    $prereqs = Confirm-ConversionTestPrerequisites -RequiredFileNames $allConversionTestFileNames
    if ($prereqs.Bypassed) { # skip the whole conversion-test block }
    $prereqs.ModelPaths["gulfmexico.MDB"]  # full resolved path, for any file that wasn't skipped
#>

function Get-ConversionModelMatch {
    param(
        [Parameter(Mandatory = $true)][string]$Folder,
        [Parameter(Mandatory = $true)][string[]]$RequiredFileNames
    )
    $found = @(Get-ChildItem -Path $Folder -Recurse -File -ErrorAction SilentlyContinue)
    $paths = @{}
    foreach ($f in $found) {
        # First match wins if a name appears more than once under the folder - ambiguous, but not
        # worth failing over; whichever test uses it will still exercise the right code path.
        if (-not $paths.ContainsKey($f.Name)) { $paths[$f.Name] = $f.FullName }
    }
    $matchedNames = New-Object 'System.Collections.Generic.HashSet[string]' ([StringComparer]::OrdinalIgnoreCase)
    foreach ($name in $paths.Keys) { [void]$matchedNames.Add($name) }
    $missing = @($RequiredFileNames | Where-Object { -not $matchedNames.Contains($_) })
    [PSCustomObject]@{ Paths = $paths; Missing = $missing }
}

function Confirm-ConversionTestPrerequisites {
    param(
        [Parameter(Mandatory = $true)][string[]]$RequiredFileNames
    )

    $configPath = Join-Path $PSScriptRoot ".conversion-test-config.txt"

    $folder = $null
    $configExists = Test-Path $configPath
    if ($configExists) {
        foreach ($line in (Get-Content $configPath)) {
            if ($line -match '^TestModelsFolder=(.*)$') { $folder = $Matches[1] }
        }
    }

    # A configured-but-now-blank folder means "bypass all, already answered" - NOT a reason to
    # re-prompt. Only a MISSING config, or a configured non-blank folder that no longer exists
    # (drive unplugged, path renamed), re-triggers the interactive setup below.
    $folderGoneMissing = ($null -ne $folder -and $folder -ne "" -and -not (Test-Path $folder))
    $needsSetup = (-not $configExists) -or $folderGoneMissing

    if ($folderGoneMissing) {
        Write-Host "Configured test-models folder no longer exists: $folder" -ForegroundColor Yellow
    }

    if ($needsSetup) {
        Write-Host ""
        Write-Host "=== Conversion-test prerequisite setup ===" -ForegroundColor Cyan
        Write-Host "The legacy-model conversion tests need real sample model files that aren't" -ForegroundColor Cyan
        Write-Host "committed to this repo. This is asked once; the answer is saved to:" -ForegroundColor Cyan
        Write-Host "  $configPath" -ForegroundColor DarkGray
        Write-Host ""

        $folder = $null
        $skips = @()
        while ($true) {
            if ($null -eq $folder) {
                $folder = Read-Host "Path to the folder containing the test model files (leave empty to skip ALL conversion tests)"
            }
            if ([string]::IsNullOrWhiteSpace($folder)) {
                $folder = ""
                $skips = @($RequiredFileNames)
                break
            }
            if (-not (Test-Path $folder)) {
                Write-Host "That path does not exist." -ForegroundColor Yellow
                $folder = $null
                continue
            }

            $match = Get-ConversionModelMatch -Folder $folder -RequiredFileNames $RequiredFileNames
            if ($match.Missing.Count -eq 0) {
                $skips = @()
                break
            }

            Write-Host "Not found under '$folder': $($match.Missing -join ', ')" -ForegroundColor Yellow
            $retry = Read-Host "Press Enter to skip these and continue, or enter a different folder path to try again"
            if ([string]::IsNullOrWhiteSpace($retry)) {
                $skips = $match.Missing
                break
            }
            $folder = $retry
        }

        "TestModelsFolder=$folder" | Set-Content $configPath
        "SkipModels=$($skips -join ',')" | Add-Content $configPath
        Write-Host "Saved." -ForegroundColor DarkGray
        Write-Host ""
    }

    if ($folder -eq "" -or $null -eq $folder) {
        Write-Host "No test-models folder configured - ALL conversion tests will be skipped." -ForegroundColor Yellow
        return [PSCustomObject]@{
            Bypassed         = $true
            TestModelsFolder = $null
            SkippedModels    = @($RequiredFileNames)
            ModelPaths       = @{}
        }
    }

    # Fresh check against the (now confirmed valid) folder - whether it just came from the setup
    # block above, or was already configured from a prior run. Self-healing: this is the ONLY
    # place SkipModels is derived for an already-configured folder, so a file that's since appeared
    # un-skips itself, and one that's since vanished gets skipped, without ever re-prompting.
    $match = Get-ConversionModelMatch -Folder $folder -RequiredFileNames $RequiredFileNames
    $skips = $match.Missing

    if (-not $needsSetup) {
        $recordedSkips = @()
        foreach ($line in (Get-Content $configPath)) {
            if ($line -match '^SkipModels=(.*)$') {
                $recordedSkips = @($Matches[1] -split ',' | Where-Object { $_ })
            }
        }
        if (@(Compare-Object $recordedSkips $skips).Count -ne 0) {
            "TestModelsFolder=$folder" | Set-Content $configPath
            "SkipModels=$($skips -join ',')" | Add-Content $configPath
        }
    }

    if ($skips.Count -gt 0) {
        Write-Host "Conversion tests skipped (model file not found): $($skips -join ', ')" -ForegroundColor Yellow
    }

    [PSCustomObject]@{
        Bypassed         = $false
        TestModelsFolder = $folder
        SkippedModels    = @($skips)
        ModelPaths       = $match.Paths
    }
}
