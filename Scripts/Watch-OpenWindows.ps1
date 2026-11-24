<#
.SYNOPSIS
    Launches an instance (or attaches to one you already have running, via -ProcessId), then polls
    DialogChecker.exe every second for DurationSeconds, printing whatever windows it currently sees.
    Trigger dialogs yourself while this runs (Save Model As..., hold the real file locked and Save,
    etc.) to watch them get detected live, including their own text content where they have any.

.PARAMETER ProcessId
    Attach to an already-running instance instead of launching a fresh one.

.PARAMETER Platform
    "x64" or "x86" - only used when launching a fresh instance. Default "x64".

.PARAMETER DurationSeconds
    How long to keep polling. Default 120.

.EXAMPLE
    .\Watch-OpenWindows.ps1

.EXAMPLE
    .\Watch-OpenWindows.ps1 -ProcessId 24852
#>
param(
    [int]$ProcessId,
    [ValidateSet("x64", "x86")]
    [string]$Platform = "x64",
    [int]$DurationSeconds = 120
)

. (Join-Path $PSScriptRoot "TestHelpers.ps1")

$ownInstance = $false
if ($ProcessId) {
    $proc = Get-Process -Id $ProcessId
    Write-Host "Attached to existing PID $ProcessId."
} else {
    Write-Host "Starting a fresh instance:"
    $instance = & (Join-Path $PSScriptRoot "Start-TestInstance.ps1") -Platform $Platform
    $proc = $instance.Process
    $ownInstance = $true
}

try {
    Write-Host "`nPolling PID $($proc.Id) every second for $DurationSeconds seconds - trigger dialogs now."
    $deadline = (Get-Date).AddSeconds($DurationSeconds)
    $lastSeenHandles = @()

    while ((Get-Date) -lt $deadline) {
        $windows = @(Get-OpenWindows -ProcessId $proc.Id)
        $currentHandles = $windows | ForEach-Object { $_.NativeWindowHandle }

        $newWindows = $windows | Where-Object { $_.NativeWindowHandle -notin $lastSeenHandles }
        foreach ($w in $newWindows) {
            $contentPart = if ($w.TextContent) { " -- `"$($w.TextContent)`"" } else { "" }
            Write-Host "[+] '$($w.Name)' ($($w.ControlType)) handle=$($w.NativeWindowHandle)$contentPart" -ForegroundColor Green
        }

        $closedHandles = $lastSeenHandles | Where-Object { $_ -notin $currentHandles }
        foreach ($h in $closedHandles) {
            Write-Host "[-] window closed (handle $h)" -ForegroundColor DarkGray
        }

        $lastSeenHandles = $currentHandles
        Start-Sleep -Seconds 1
    }
} finally {
    if ($ownInstance) {
        Write-Host "`nClosing the instance this script started."
        Stop-Process -Id $proc.Id -Force -ErrorAction SilentlyContinue
        Remove-Item (Split-Path -Parent $instance.ModelPath) -Recurse -Force -ErrorAction SilentlyContinue
    }
}
