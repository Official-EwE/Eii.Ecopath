<#
.SYNOPSIS
    Runs DialogChecker.exe's unfiltered "dump" mode against an already-running instance - lists
    literally every descendant element it can reach under the main window, no condition at all.
    Open the dialog you want to investigate yourself first (e.g. Save Model As...), keep it open,
    find its PID (Get-Process ScientificInterface), then run this against that PID.

.PARAMETER ProcessId
    The already-running instance's PID.

.EXAMPLE
    .\Debug-DumpOpenWindows.ps1 -ProcessId 36832
#>
param(
    [Parameter(Mandatory = $true)][int]$ProcessId
)

. (Join-Path $PSScriptRoot "TestHelpers.ps1")

$exePath = Get-DialogCheckerExe
Write-Host "Dumping every reachable descendant for PID $ProcessId ..."
$output = & $exePath $ProcessId "dump"

$output | ForEach-Object {
    if ($_ -match "Save As") {
        Write-Host $_ -ForegroundColor Green
    } else {
        Write-Host $_
    }
}

$matchCount = ($output | Select-String "Save As").Count
Write-Host "`n'Save As' appears in $matchCount line(s) of the dump." -ForegroundColor Cyan
