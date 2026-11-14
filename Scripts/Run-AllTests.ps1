<#
.SYNOPSIS
    Runs every test runner in this toolkit (Run-StagingTests, Run-CrashRecoveryTests,
    Run-MultiInstanceTests) once for x64 and once for x86, with one combined summary at the end
    covering all six runs, each test labeled with the platform it ran under.

.PARAMETER Platforms
    Which platforms to run. Default both. Pass just one (e.g. -Platforms x64) to skip the other
    without needing to fall back to running the individual scripts by hand.

.EXAMPLE
    .\Run-AllTests.ps1

.EXAMPLE
    .\Run-AllTests.ps1 -Platforms x64
#>
param(
    [ValidateSet("x64", "x86")]
    [string[]]$Platforms = @("x64", "x86")
)

. (Join-Path $PSScriptRoot "TestHelpers.ps1")

$runners = @(
    "Run-StagingTests.ps1",
    "Run-CrashRecoveryTests.ps1",
    "Run-MultiInstanceTests.ps1"
)

foreach ($platform in $Platforms) {
    $script:CurrentPlatformLabel = $platform
    foreach ($runner in $runners) {
        Write-Host "`n`n################ $runner ($platform) ################" -ForegroundColor Magenta
        # Dot-sourced, not called via & - needed so each runner's results land in THIS script's own
        # $script:TestResults list instead of a separate, discarded child scope, which is what would
        # happen with a normal call. Each runner still prints its own (progressively more complete)
        # summary at its own end too, same as running it standalone - a little redundant by the time
        # this script's own final summary prints the same accumulated list once more, but harmless.
        . (Join-Path $PSScriptRoot $runner) -Platform $platform
    }
}

$script:CurrentPlatformLabel = $null
Write-Host "`n`n################ COMBINED RESULTS - ALL PLATFORMS ################" -ForegroundColor Magenta
Show-TestSummary
