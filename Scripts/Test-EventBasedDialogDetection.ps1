<#
.SYNOPSIS
    EXPERIMENTAL - a small, standalone test of UI Automation's event model
    (Start-WindowEventWatch/Get-WatchedWindowEvent/Stop-WindowEventWatch in TestHelpers.ps1), as an
    alternative to the repeated FindFirst polling every other dialog-detection spot in this toolkit
    uses. Deliberately does NOT touch Start-TestInstance.ps1 or Run-StagingTests.ps1 - this launches
    the process itself, so the existing (working, polling-based) upgrade-dialog handling never gets
    a chance to consume the dialog before this experiment's own event-based code sees it.

    Covers exactly two cases, both expecting a dialog:
      1. The startup "database will be updated to a newer version" prompt.
      2. The lock-contention dialog, when Save is clicked while the real file is held locked.

    Does NOT yet implement the broader "check after every substep, fail and restart if something
    UNEXPECTED shows up" pattern - this is only proving the underlying mechanism works at all
    before building anything larger on top of it.

.PARAMETER RepoRoot
    Root of the Eii.Ecopath repo. Defaults to two levels up from this script's own folder.

.PARAMETER Platform
    "x64" or "x86". Default "x64".

.EXAMPLE
    .\Test-EventBasedDialogDetection.ps1
#>
param(
    [string]$RepoRoot = (Split-Path -Parent $PSScriptRoot),
    [ValidateSet("x64", "x86")]
    [string]$Platform = "x64",
    [string]$Configuration = "Debug"
)

. (Join-Path $PSScriptRoot "TestHelpers.ps1")

$sourceModelPath = Join-Path $RepoRoot "Database\Anchovy Bay Spatial.ewemdb"
if (-not (Test-Path $sourceModelPath)) { throw "Source model not found: $sourceModelPath" }

$workFolder = Join-Path $env:TEMP "EwE-TestRun\EventTest-$(Get-Random)"
New-Item -ItemType Directory -Path $workFolder -Force | Out-Null
$modelPath = Join-Path $workFolder (Split-Path -Leaf $sourceModelPath)
Copy-Item $sourceModelPath $modelPath -Force

$exePath = Join-Path $RepoRoot "Sources\ScientificInterface\bin\$Platform\$Configuration\net48\ScientificInterface.exe"
if (-not (Test-Path $exePath)) { throw "Built exe not found: $exePath - build it first (see Start-TestInstance.ps1)." }

try {
    # Watch registered BEFORE Start-Process, not after - CONFIRMED this matters: the dialog this is
    # meant to catch can appear very early in startup, and an event handler never catches anything
    # retroactively, so registering after launching risks missing it entirely if it's already fired
    # by the time the handler is listening. No -ProcessId here either, for the same reason - it
    # doesn't exist yet at this point; filtering happens later, when reading from the queue.
    Write-Host "Starting the watch, then launching the process..."
    $watch = Start-WindowEventWatch
    $proc = Start-Process -FilePath $exePath -ArgumentList "`"$modelPath`"" -PassThru

    try {
        # ===== Case 1: the startup upgrade dialog =====
        Write-Host "`n=== Case 1: startup upgrade dialog, via the event queue ==="
        $event = Get-WatchedWindowEvent -Watch $watch -TimeoutSeconds 30 -ProcessId $proc.Id
        if ($null -eq $event) {
            throw "No window-opened event arrived within 30 seconds at all - either the dialog didn't appear, or the event watch itself isn't working."
        }
        Write-Host "Event queue yielded: Name='$($event.Name)' ControlType=$($event.ControlType)"
        if ($event.Name -ne "Ecopath with Ecosim") {
            Write-Host "NOTE: this doesn't look like the upgrade dialog by name - may be some other window opening first (e.g. the main window itself, if it also raises this event)." -ForegroundColor Yellow
        }

        # The event only told us a window opened - a normal FindFirst still gets the real reference
        # to actually interact with it (the callback deliberately never enqueues the live element).
        $root = [System.Windows.Automation.AutomationElement]::RootElement
        $dialogCondition = New-Object System.Windows.Automation.AndCondition(@(
            (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::ProcessIdProperty, $proc.Id)),
            (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::NameProperty, "Ecopath with Ecosim")),
            (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::ControlTypeProperty, [System.Windows.Automation.ControlType]::Window))
        ))
        $dialog = $root.FindFirst([System.Windows.Automation.TreeScope]::Children, $dialogCondition)
        if ($null -eq $dialog) {
            throw "Event fired, but a follow-up FindFirst couldn't locate the actual dialog element."
        }
        # Name only, NOT ControlType=Button: confirmed earlier this session this dialog's buttons
        # report ControlType.Pane.
        $yesCondition = New-Object System.Windows.Automation.PropertyCondition(
            [System.Windows.Automation.AutomationElement]::NameProperty, "Yes")
        $yesButton = $dialog.FindFirst([System.Windows.Automation.TreeScope]::Descendants, $yesCondition)
        if ($null -eq $yesButton) { throw "Found the dialog but not its 'Yes' button." }
        Invoke-ElementClick -Element $yesButton
        Write-Host "Clicked Yes via the event-detected dialog." -ForegroundColor Green

        # Now wait for the main window normally - same as every other test in this toolkit.
        Write-Host "`nWaiting for the main window..."
        $deadline = (Get-Date).AddSeconds(60)
        while ($proc.MainWindowHandle -eq [IntPtr]::Zero -and (Get-Date) -lt $deadline) {
            Start-Sleep -Milliseconds 300
            $proc.Refresh()
        }
        if ($proc.MainWindowHandle -eq [IntPtr]::Zero) { throw "Main window never appeared within 60 seconds." }
        Write-Host "Main window ready." -ForegroundColor Green

        # ===== Case 2: the lock-contention dialog =====
        Write-Host "`n=== Case 2: lock-contention dialog, via the event queue ==="
        Set-SafeRestorableEdit -Proc $proc

        $readyFlag = Join-Path $env:TEMP "EwE-TestRun\LockJob-$(Get-Random).flag"
        if (Test-Path $readyFlag) { Remove-Item $readyFlag -Force }
        $lockJob = Start-Job -ScriptBlock {
            param($path, $holdSeconds, $readyFlagPath)
            $fs = [System.IO.File]::Open($path, 'Open', 'ReadWrite', 'None')
            New-Item -ItemType File -Path $readyFlagPath -Force | Out-Null
            Start-Sleep -Seconds $holdSeconds
            $fs.Close()
            Remove-Item $readyFlagPath -Force -ErrorAction SilentlyContinue
        } -ArgumentList $modelPath, 6, $readyFlag

        try {
            $lockDeadline = (Get-Date).AddSeconds(10)
            while (-not (Test-Path $readyFlag) -and (Get-Date) -lt $lockDeadline) { Start-Sleep -Milliseconds 100 }
            if (-not (Test-Path $readyFlag)) { throw "Lock-holding job never signaled it had acquired the lock within 10 seconds." }

            Invoke-SaveChanges -Proc $proc

            $event2 = Get-WatchedWindowEvent -Watch $watch -TimeoutSeconds 10 -ProcessId $proc.Id
            if ($null -eq $event2) {
                throw "No window-opened event arrived within 10 seconds of triggering Save under lock."
            }
            Write-Host "Event queue yielded: Name='$($event2.Name)' ControlType=$($event2.ControlType)"

            $window = Get-AppWindow -Proc $proc
            $lockDialog = $window.FindFirst([System.Windows.Automation.TreeScope]::Descendants, $dialogCondition)
            if ($null -eq $lockDialog) { throw "Event fired, but a follow-up FindFirst couldn't locate the actual lock-contention dialog." }

            $cancelCondition = New-Object System.Windows.Automation.PropertyCondition(
                [System.Windows.Automation.AutomationElement]::NameProperty, "Cancel")
            $cancelButton = $lockDialog.FindFirst([System.Windows.Automation.TreeScope]::Descendants, $cancelCondition)
            if ($null -eq $cancelButton) { throw "Found the lock-contention dialog but not its 'Cancel' button." }
            Invoke-ElementClick -Element $cancelButton
            Write-Host "Clicked Cancel via the event-detected dialog." -ForegroundColor Green
        } finally {
            Wait-Job $lockJob -Timeout 10 | Out-Null
            Remove-Job $lockJob -Force -ErrorAction SilentlyContinue
        }

        Write-Host "`nBOTH CASES PASSED." -ForegroundColor Green
    } finally {
        Stop-WindowEventWatch -Watch $watch
    }
} finally {
    if ($proc -and -not $proc.HasExited) { Stop-Process -Id $proc.Id -Force -ErrorAction SilentlyContinue }
    Remove-Item $workFolder -Recurse -Force -ErrorAction SilentlyContinue
}
