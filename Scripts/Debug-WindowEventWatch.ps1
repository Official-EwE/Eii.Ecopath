<#
.SYNOPSIS
    DIAGNOSTIC - prints every window-opened event the watch receives, from any process, with no
    filtering and no timeout-based failure. Open Notepad (or anything else) while this runs, to see
    whether the underlying event mechanism works at all, independent of the EwE app itself.

.PARAMETER DurationSeconds
    How long to watch for. Default 60.

.EXAMPLE
    .\Debug-WindowEventWatch.ps1
#>
param([int]$DurationSeconds = 60)

. (Join-Path $PSScriptRoot "TestHelpers.ps1")

Write-Host "Watching for ANY window-opened event for $DurationSeconds seconds - try opening Notepad now."
$watch = Start-WindowEventWatch
try {
    $deadline = (Get-Date).AddSeconds($DurationSeconds)
    $seenAny = $false
    while ((Get-Date) -lt $deadline) {
        $event = Get-WatchedWindowEvent -Watch $watch -TimeoutSeconds 1
        if ($null -ne $event) {
            $seenAny = $true
            Write-Host "EVENT: Name='$($event.Name)' ControlType=$($event.ControlType) ProcessId=$($event.ProcessId)" -ForegroundColor Green
        }
    }
} finally {
    Stop-WindowEventWatch -Watch $watch
}

if (-not $seenAny) {
    Write-Host "`nNo events received at all during the whole window." -ForegroundColor Red
} else {
    Write-Host "`nDone." -ForegroundColor Cyan
}
