<#
.SYNOPSIS
    §1.3 - confirms a caller that never sets EnableLocalStaging (e.g. EwERunConsole) creates no
    working copy at all. Unlike every other test in this suite, this one needs no running app and
    no UI Automation whatsoever - it loads EwECore.dll directly and calls into cCore itself, the
    same minimal way a real, working non-UI consumer does.

    The exact sequence here is confirmed from Eii.Ecopath.Runner.Services' own CoreService.cs (a
    real, shipped, non-UI consumer of EwECore.dll - the same project family as EwERunConsole):
        Core = new cCore();
        Core.PluginManager = new cPluginManager();
        Core.LoadModel(modelFile);
    No explicit InitCore() call appears there at all, even though LoadModel()'s own source asserts
    "The Core has not been initialized. Call InitCore() first." if it's skipped - meaning cCore's
    own constructor must already call InitCore() internally. EnableLocalStaging is never set
    anywhere in this sequence, unlike ScientificInterface's own explicit opt-in at startup - exactly
    the condition this test needs.

.PARAMETER RepoRoot
    Root of the Eii.Ecopath repo. Defaults to two levels up from this script's own folder.

.PARAMETER Platform
    "x64" or "x86" - which build of EwECore.dll to load. Default "x64".

.PARAMETER Configuration
    Default "Debug".

.PARAMETER SourceModelPath
    The model file to copy and load. Defaults to the same sample model every other runner uses.

.EXAMPLE
    .\Run-NoStagingCallerTests.ps1
#>
param(
    [string]$RepoRoot = (Split-Path -Parent $PSScriptRoot),
    [ValidateSet("x64", "x86")]
    [string]$Platform = "x64",
    [string]$Configuration = "Debug",
    [string]$SourceModelPath,
    [string]$Only,
    # Rebuild the app on the first launch, even if this PowerShell session already built it (e.g. after a branch switch).
    [switch]$ForceBuild
)

. (Join-Path $PSScriptRoot "TestHelpers.ps1")
# -ForceBuild: forget what this session already built, so the first launch below rebuilds once. Runners started
# from Run-AllTests.ps1 never get the switch themselves, so they reuse the build made there.
if ($ForceBuild) { $global:EwEBuiltProjects = @{} }
$runStopwatch = [System.Diagnostics.Stopwatch]::StartNew()
$script:TestNameFilter = $Only

if (-not $SourceModelPath) {
    $SourceModelPath = Join-Path $RepoRoot "Database\Anchovy Bay Spatial.ewemdb"
}
if (-not (Test-Path $SourceModelPath)) {
    throw "Source model not found: $SourceModelPath"
}

$binFolder = Join-Path $RepoRoot "Sources\EwECore\bin\$Platform\$Configuration\net48"
$coreDllPath = Join-Path $binFolder "EwECore.dll"

Invoke-Test "1.3 - A caller that never sets EnableLocalStaging creates no working copy" {
    if ($ForceBuild -or -not (Test-Path $coreDllPath)) {
        Write-Step "Building EwECore.dll ($Configuration|$Platform)" {
            $vbproj = Join-Path $RepoRoot "Sources\EwECore\EwECore.vbproj"
            dotnet build $vbproj -c $Configuration -p:Platform=$Platform | ForEach-Object { Write-Host $_ }
            if ($LASTEXITCODE -ne 0) { throw "Build failed (dotnet build exit code $LASTEXITCODE)." }
        }
    }
    Assert-True (Test-Path $coreDllPath) "EwECore.dll still not found at: $coreDllPath"

    $workFolder = Join-Path $env:TEMP "EwE-TestRun\NoStaging-$(Get-Random)"
    $workingModelPath = Write-Step "Preparing a working copy of the sample model" {
        New-Item -ItemType Directory -Path $workFolder -Force | Out-Null
        $modelPath = Join-Path $workFolder (Split-Path -Leaf $SourceModelPath)
        Copy-Item $SourceModelPath $modelPath -Force
        $modelPath
    }

    # Loaded from a byte array, NOT via Add-Type -Path / Assembly.LoadFrom: those lock the DLL file
    # on disk for the rest of this PowerShell process's lifetime, which would break a later
    # `dotnet build` of EwECore within the same session (e.g. a subsequent Start-TestInstance.ps1
    # call). Assembly.Load(byte[]) loads the assembly fully into memory with no file lock at all.
    #
    # Every DLL in the output folder is loaded this way UPFRONT, eagerly, rather than relying on a
    # lazy AssemblyResolve handler - CONFIRMED that New-Object's own type lookup doesn't trigger
    # AssemblyResolve at all (that event only fires for the CLR's own internal reference resolution,
    # e.g. EwECore.dll's code referencing a type from another assembly) - it only searches whatever
    # is ALREADY loaded into the AppDomain. Loading everything up front sidesteps that gap entirely.
    Write-Step "Loading every DLL in the output folder (byte-array, no file locks)" {
        Get-ChildItem -Path $binFolder -Filter "*.dll" | ForEach-Object {
            try {
                [Reflection.Assembly]::Load([IO.File]::ReadAllBytes($_.FullName)) | Out-Null
            } catch {
                # Not every DLL in this folder is necessarily a loadable managed assembly - skip quietly.
            }
        }
    }

    try {
        # PluginManager deliberately left unset: LoadModel()'s own source null-checks it
        # ("If (Me.PluginManager IsNot Nothing) Then Me.PluginManager.OpenDatabase(...)") and skips
        # the plugin step gracefully when it's not set - CoreService.cs sets it because it needs
        # real plugin functionality for its own broader purpose, but this test only cares whether
        # staging activates, which has nothing to do with plugins. Simpler than chasing down which
        # assembly cPluginManager actually lives in (not EwECore's own output folder, confirmed).
        $core = Write-Step "Creating a cCore instance" { New-Object EwECore.cCore }

        # Snapshotted BEFORE LoadModel, not just checked after: Get-StagedCopies has no notion of
        # "new" at all - it just lists everything currently in the folder. Given how many UI
        # Automation tests this session have staged a copy of this exact same fixture, a leftover
        # from an earlier, unrelated run would share the identical "TempAnchovy Bay Spatial.<GUID>"
        # naming pattern and could easily be mistaken for something THIS call just created - only
        # comparing before/after makes that unambiguous.
        $copiesBefore = Write-Step "Snapshotting the staging folder before loading" { Get-StagedCopies | ForEach-Object { $_.FullName } }

        $loaded = Write-Step "Loading the model directly via EwECore.dll" { $core.LoadModel($workingModelPath) }
        Assert-True $loaded "LoadModel() returned False - couldn't open the sample model directly via EwECore.dll."

        # The actual point of this test: EnableLocalStaging was never set anywhere above (unlike
        # ScientificInterface's own explicit startup opt-in) - staging must not have activated.
        Write-Step "Confirming no NEW staged copy appeared" {
            $newCopies = Get-StagedCopies | Where-Object { $_.FullName -notin $copiesBefore }
            if ($newCopies.Count -gt 0) {
                $copyPaths = ($newCopies | ForEach-Object { $_.FullName }) -join "; "
                Assert-True $false "A NEW staged working copy appeared after LoadModel(), even though EnableLocalStaging was never set: $copyPaths - expected staging to be strictly opt-in, not on by default for every EwECore.dll consumer."
            }
        }

        Write-Step "Closing the model" {
            $core.CloseModel()
            $core.Dispose()
        }
    } finally {
        Remove-Item $workFolder -Recurse -Force -ErrorAction SilentlyContinue
    }
}

Show-TestSummary
$runStopwatch.Stop()
Write-Host "`nTotal duration: $(Format-Duration $runStopwatch.Elapsed)" -ForegroundColor Cyan
Clear-TestArtifacts
