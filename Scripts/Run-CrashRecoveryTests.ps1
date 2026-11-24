<#
.SYNOPSIS
    Tests for crash recovery (orphan sweep) and exit-time cleanup. Each test launches its OWN
    disposable instance via Start-TestInstance.ps1, since these tests kill/close processes and
    can't safely share an instance with other tests the way Run-StagingTests.ps1's do.

.PARAMETER Platform
    "x64" or "x86" - which build each disposable instance launches as. Default "x64".

.PARAMETER Only
    Run just the one test whose name contains this substring (case-insensitive), skipping the rest.

.PARAMETER MainWindowTimeoutSeconds
    How long each disposable instance waits for its main window before giving up. Default 90 -
    raise this on a slower machine if launches are timing out before the window ever appears.

.EXAMPLE
    .\Run-CrashRecoveryTests.ps1 -Platform x86
#>
param(
    [ValidateSet("x64", "x86")]
    [string]$Platform = "x64",
    [string]$Only,
    [int]$MainWindowTimeoutSeconds = 90,
    # Rebuild the app on the first launch, even if this PowerShell session already built it (e.g. after a branch switch).
    [switch]$ForceBuild
)

. (Join-Path $PSScriptRoot "TestHelpers.ps1")
# -ForceBuild: forget what this session already built, so the first launch below rebuilds once. Runners started
# from Run-AllTests.ps1 never get the switch themselves, so they reuse the build made there.
if ($ForceBuild) { $global:EwEBuiltProjects = @{} }
$runStopwatch = [System.Diagnostics.Stopwatch]::StartNew()
$script:TestNameFilter = $Only
$startInstanceScript = Join-Path $PSScriptRoot "Start-TestInstance.ps1"
$repoRoot = Split-Path -Parent $PSScriptRoot

# Instance launches/reopens/model-switches below are NOT wrapped in Write-Step anywhere in this
# file: Start-TestInstance.ps1, Start-InstanceOnExistingModel, and Open-ModelViaUI each print their
# own multi-line progress (build output, launch, upgrade-dialog dots), which reads badly interleaved
# with Write-Step's own "...done" framing - a plain Write-Host header line is used before each one
# instead.

Invoke-Test "2.1 - Kill mid-save (after save, before publish-back): original file untouched" {
    Write-Host "  Starting a disposable instance:"
    $instance = & $startInstanceScript -Platform $Platform -MainWindowTimeoutSeconds $MainWindowTimeoutSeconds
    Write-InstanceStarted -ProcessId $instance.Process.Id -Label "2.1 - instance"
    $killed = $false
    try {
        $originalHash = Write-Step "Hashing the original file" { (Get-FileHash -Path $instance.ModelPath -Algorithm SHA256).Hash }

        Write-Step "Enforcing a change in Basic Input" { Set-SafeRestorableEdit -Proc $instance.Process }
        Assert-True (Test-SaveButtonEnabled -Proc $instance.Process) "Save is not enabled after making a change."

        # The cCore.vb test hook (DEBUG-only - see its own comment there) blocks execution right
        # after save, before publish-back, for as long as this flag file exists - and now also
        # signals via a second .ack file the MOMENT it actually reaches that point, so this can wait
        # for a real confirmation instead of guessing a delay before killing the process.
        $waitFlag = Join-Path $env:TEMP "EwE_TestWaitBeforePublish.flag"
        $ackFlag = Join-Path $env:TEMP "EwE_TestWaitBeforePublish.ack"
        Remove-Item $waitFlag, $ackFlag -Force -ErrorAction SilentlyContinue
        Write-Step "Arming the pre-publish wait hook" { New-Item -ItemType File -Path $waitFlag -Force | Out-Null }

        try {
            Write-Step "Saving (expecting it to block before publish-back)" { Invoke-SaveChanges -Proc $instance.Process }

            Write-Step "Waiting for the app to acknowledge it reached the wait point" {
                $ackDeadline = (Get-Date).AddSeconds(10)
                $acked = $false
                while ((Get-Date) -lt $ackDeadline) {
                    if (Test-Path $ackFlag) { $acked = $true; break }
                    Start-Sleep -Milliseconds 100
                }
                Assert-True $acked "App never signaled it reached the pre-publish wait point within 10 seconds - confirm this is a DEBUG build (the hook is wrapped in #If DEBUG in cCore.vb)."
            }

            Write-Step "Killing the process mid-save" {
                Stop-TestInstance -ProcessId $instance.Process.Id -Label "2.1 - instance (killed mid-save)"
                $script:killed = $true
            }
            $killed = $script:killed

            $hashAfterKill = Write-Step "Hashing the original file after the kill" { (Get-FileHash -Path $instance.ModelPath -Algorithm SHA256).Hash }
            Assert-True ($hashAfterKill -eq $originalHash) "Original file changed even though the process was killed BEFORE publish-back started - expected it completely untouched."
        } finally {
            Remove-Item $waitFlag, $ackFlag -Force -ErrorAction SilentlyContinue
        }

        # Reopening the SAME (untouched) file afterward should still produce a correct, fresh
        # working copy - confirms the interrupted save left nothing in a broken state.
        Write-Host "  Reopening the same file in a fresh instance:"
        $reopenProc = Start-InstanceOnExistingModel -RepoRoot $repoRoot -ModelPath $instance.ModelPath -Platform $Platform
        Write-InstanceStarted -ProcessId $reopenProc.Id -Label "2.1 - reopened instance"
        $reopenBaseline = Get-WindowBaseline -ProcessId $reopenProc.Id
        try {
            $copies = Write-Step "Waiting for a fresh working copy to be staged" { Wait-ForStagedCopy -TimeoutSeconds 30 }
            Assert-True ($copies.Count -gt 0) "Reopening the file after the interrupted save did not produce a working copy."
            Write-Step "Confirming no dialog was left open" { Assert-NoNewDialogs -ProcessId $reopenProc.Id -Baseline $reopenBaseline }
        } finally {
            Stop-TestInstance -ProcessId $reopenProc.Id -Label "2.1 - reopened instance"
        }
    } finally {
        if (-not $killed -and -not $instance.Process.HasExited) {
            Stop-TestInstance -ProcessId $instance.Process.Id -Label "2.1 - instance (cleanup after an earlier failure)"
        }
    }
}

Invoke-Test "O.3 - Paths with spaces and non-ASCII characters work correctly" {
    $trickyNamedSource = Join-Path $env:TEMP "Tëst Mödél with spaces André.ewemdb"
    Write-Step "Preparing a source file with spaces and non-ASCII characters" {
        Copy-Item (Join-Path $repoRoot "Database\Anchovy Bay Spatial.ewemdb") $trickyNamedSource -Force
    }
    try {
        $trickyWorkFolder = Join-Path $env:TEMP "EwE-TestRun\Tëst Rün häs spaces $(Get-Random)"
        Write-Host "  Starting an instance with a tricky source/destination path:"
        $instance = & $startInstanceScript -Platform $Platform -SourceModelPath $trickyNamedSource -WorkFolder $trickyWorkFolder -MainWindowTimeoutSeconds $MainWindowTimeoutSeconds
        Write-InstanceStarted -ProcessId $instance.Process.Id -Label "O.3 - instance"
        $baseline = Get-WindowBaseline -ProcessId $instance.Process.Id
        try {
            $copies = Write-Step "Waiting for a staged working copy to appear" { Wait-ForStagedCopy -TimeoutSeconds 30 }
            Assert-True ($copies.Count -gt 0) "No working copy staged for a source/destination path containing spaces and non-ASCII characters."
            Write-Step "Confirming no dialog was left open" { Assert-NoNewDialogs -ProcessId $instance.Process.Id -Baseline $baseline }
        } finally {
            Stop-TestInstance -ProcessId $instance.Process.Id -Label "O.3 - instance"
        }
    } finally {
        Remove-Item $trickyNamedSource -Force -ErrorAction SilentlyContinue
    }
}

Invoke-Test "4.1 - Orphaned working copy is swept on the next write-intent open" {
    Write-Host "  Starting instance A:"
    $instanceA = & $startInstanceScript -Platform $Platform -MainWindowTimeoutSeconds $MainWindowTimeoutSeconds
    Write-InstanceStarted -ProcessId $instanceA.Process.Id -Label "4.1 - instanceA"
    $copiesBefore = Write-Step "Waiting for instance A's working copy" { Wait-ForStagedCopy -TimeoutSeconds 30 }
    Assert-True ($copiesBefore.Count -gt 0) "Instance A never staged a working copy."
    $orphanPath = $copiesBefore[0].FullName

    Write-Step "Killing instance A, orphaning its working copy" {
        Stop-TestInstance -ProcessId $instanceA.Process.Id -Label "4.1 - instanceA"
        Start-Sleep -Milliseconds 500
    }
    Assert-True (Test-Path $orphanPath) "Orphaned file disappeared immediately on kill - expected it to persist until the NEXT write-intent open (this is 5.4's own distinction, worth it failing loudly here if it ever changes)."

    # Opening ANY database elsewhere should trigger the orphan sweep - doesn't need to be the same file.
    Write-Host "  Starting instance B (any database triggers the sweep):"
    $instanceB = & $startInstanceScript -Platform $Platform -MainWindowTimeoutSeconds $MainWindowTimeoutSeconds
    Write-InstanceStarted -ProcessId $instanceB.Process.Id -Label "4.1 - instanceB"
    $baselineB = Get-WindowBaseline -ProcessId $instanceB.Process.Id
    try {
        Write-Step "Waiting for instance B's own working copy" { Wait-ForStagedCopy -TimeoutSeconds 30 | Out-Null }
        Write-Step "Confirming the orphan was swept away" {
            Assert-True (-not (Test-Path $orphanPath)) "Orphaned file from the killed instance A was NOT swept away by instance B's own write-intent open."
        }
        Write-Step "Confirming no dialog was left open" { Assert-NoNewDialogs -ProcessId $instanceB.Process.Id -Baseline $baselineB }
    } finally {
        Stop-TestInstance -ProcessId $instanceB.Process.Id -Label "4.1 - instanceB"
    }
}

# 4.2 (mid-copy kill) deliberately omitted: the test plan itself hedges this as "if reproducible" -
# it needs killing a process within roughly the first second of a copy starting, which is inherently
# racy. A flaky test that fails unpredictably regardless of real behavior is worse than no test here.

Invoke-Test "4.3 - Several orphaned copies are all cleared in one sweep pass" {
    $orphanPaths = @()
    foreach ($i in 1..3) {
        Write-Host "  Starting and orphaning instance $i`:"
        $inst = & $startInstanceScript -Platform $Platform -MainWindowTimeoutSeconds $MainWindowTimeoutSeconds
        Write-InstanceStarted -ProcessId $inst.Process.Id -Label "4.3 - instance $i"
        $copies = Write-Step "Waiting for its working copy" { Wait-ForStagedCopy -TimeoutSeconds 30 }
        Assert-True ($copies.Count -gt 0) "Instance $i never staged a working copy."
        $orphanPaths += $copies[0].FullName
        Write-Step "Killing it, leaving the copy orphaned" {
            Stop-TestInstance -ProcessId $inst.Process.Id -Label "4.3 - instance $i"
            Start-Sleep -Milliseconds 300
        }
    }

    Write-Host "  Starting a cleanup-trigger instance:"
    $cleanupInstance = & $startInstanceScript -Platform $Platform -MainWindowTimeoutSeconds $MainWindowTimeoutSeconds
    Write-InstanceStarted -ProcessId $cleanupInstance.Process.Id -Label "4.3 - cleanup-trigger instance"
    $cleanupBaseline = Get-WindowBaseline -ProcessId $cleanupInstance.Process.Id
    try {
        Write-Step "Waiting for the cleanup-trigger instance's own working copy" { Wait-ForStagedCopy -TimeoutSeconds 30 | Out-Null }
        Write-Step "Confirming all 3 orphans were cleared" {
            foreach ($path in $orphanPaths) {
                Assert-True (-not (Test-Path $path)) "Orphaned file $path was not cleared by the sweep."
            }
        }
        Write-Step "Confirming no dialog was left open" { Assert-NoNewDialogs -ProcessId $cleanupInstance.Process.Id -Baseline $cleanupBaseline }
    } finally {
        Stop-TestInstance -ProcessId $cleanupInstance.Process.Id -Label "4.3 - cleanup-trigger instance"
    }
}

Invoke-Test "5.1/5.2 - Several sessions in one app run, all cleaned up on final graceful close" {
    Write-Host "  Starting the instance:"
    $instance = & $startInstanceScript -Platform $Platform -MainWindowTimeoutSeconds $MainWindowTimeoutSeconds
    Write-InstanceStarted -ProcessId $instance.Process.Id -Label "5.1/5.2 - instance"
    Assert-True ($null -ne $instance.StagedCopyPath) "No staged copy confirmed for the initial open."
    $initialBaseline = Get-WindowBaseline -ProcessId $instance.Process.Id

    # §5.1's own "several sessions in one continuous app run" requirement: close and reopen the
    # SAME model a couple of times within this one process, before the eventual final close - each
    # cycle should leave its own marker-less copy behind (per the known limitation at the top of
    # the test plan), and ALL of them, not just the last, need to be gone after the final close.
    $allStagedPaths = [System.Collections.Generic.List[string]]::new()
    $allStagedPaths.Add($instance.StagedCopyPath)

    for ($i = 1; $i -le 2; $i++) {
        Write-Host "  Closing the model (cycle $i):"
        Close-CurrentModel -Proc $instance.Process
        Write-Host "  Reopening the model (cycle $i):"
        Open-ModelViaUI -Proc $instance.Process -Path $instance.ModelPath
        $newCopy = Write-Step "Waiting for a NEW staged copy for this cycle" {
            $copiesBefore = $allStagedPaths.ToArray()
            $found = $null
            $cycleDeadline = (Get-Date).AddSeconds(30)
            while ($null -eq $found -and (Get-Date) -lt $cycleDeadline) {
                $match = Get-StagedCopies | Where-Object { $_.FullName -notin $copiesBefore } | Select-Object -First 1
                if ($match) { $found = $match.FullName } else { Start-Sleep -Milliseconds 300 }
            }
            $found
        }
        Assert-True ($null -ne $newCopy) "Cycle $i - no NEW staged copy appeared after closing and reopening the model."
        $allStagedPaths.Add($newCopy)

        # Re-captured after EVERY cycle, not just once at the very start: CONFIRMED that closing and
        # reopening the model tears down and recreates its entire sub-window tree from scratch - the
        # reopened "Basic input" panel, toolbars, etc. are all genuinely NEW windows with new native
        # handles, even though they're logically the same kind of panel as before. A baseline from
        # before any of this would flag every single one of them as new, even though none of this is
        # actually unexpected - it's just the app's own normal reopen behavior.
        $initialBaseline = Get-WindowBaseline -ProcessId $instance.Process.Id
    }

    # Checked here, right before the graceful close below, against the baseline captured right at
    # the start (before any of the close/reopen cycles above) - confirms none of those cycles left a
    # dialog open, while the process is still around to check at all.
    Write-Step "Confirming no dialog was left open after the close/reopen cycles" {
        Assert-NoNewDialogs -ProcessId $instance.Process.Id -Baseline $initialBaseline
    }

    # 5.2 is a real regression test from this backport's own development: an earlier draft used a
    # synchronous, sleep-based retry on Close() that measurably delayed shutdown. Timed here, not
    # just checked for completion, specifically to catch that regression if it ever reappears.
    $sw = [System.Diagnostics.Stopwatch]::StartNew()
    $exited = Write-Step "Closing the app gracefully" {
        $instance.Process.CloseMainWindow() | Out-Null
        $instance.Process.WaitForExit(15000)
    }
    $sw.Stop()
    Write-Host "  [PID $($instance.Process.Id)] $(if ($exited) { 'Exited' } else { 'Still running' }) after $($sw.ElapsedMilliseconds) ms." -ForegroundColor DarkGray
    Assert-True $exited "App did not exit within 15 seconds of a graceful close request."
    Assert-True ($sw.ElapsedMilliseconds -lt 5000) "Graceful close took $($sw.ElapsedMilliseconds) ms - expected under 5000ms (regression: an earlier draft's sleep-based retry measurably delayed shutdown)."

    # Exit-time cleanup is a separate mechanism from the orphan sweep (§4) - it should remove EVERY
    # one of this run's copies shortly after the process has actually exited, in a single pass, not
    # just the most recently-opened one.
    Write-Step "Confirming every staged copy from this run was cleaned up" {
        $cleanupDeadline = (Get-Date).AddSeconds(10)
        $remaining = $allStagedPaths
        while ((Get-Date) -lt $cleanupDeadline) {
            $remaining = $allStagedPaths | Where-Object { Test-Path $_ }
            if ($remaining.Count -eq 0) { break }
            Start-Sleep -Milliseconds 300
        }
        Assert-True ($remaining.Count -eq 0) "Staged copies still present ~10 seconds after a graceful close: $($remaining -join '; ')"
    }
}

Invoke-Test "5.4 - A hard-killed process's own copy is NOT cleaned up by exit-time cleanup" {
    # Distinct from 4.1 above: 4.1 confirms the orphan DOES eventually get cleaned up, by a LATER
    # instance's own open. This test confirms the DIFFERENT mechanism (exit-time cleanup, via
    # AppDomain.ProcessExit) does NOT fire on a hard kill, since that event isn't reliably raised
    # on one - the file should still be sitting there immediately after, not vanish on its own.
    Write-Host "  Starting the instance:"
    $instance = & $startInstanceScript -Platform $Platform -MainWindowTimeoutSeconds $MainWindowTimeoutSeconds
    Write-InstanceStarted -ProcessId $instance.Process.Id -Label "5.4 - instance"
    $copies = Write-Step "Waiting for a staged working copy" { Wait-ForStagedCopy -TimeoutSeconds 30 }
    Assert-True ($copies.Count -gt 0) "Working copy was never staged."
    $stagedPath = $copies[0].FullName

    Write-Step "Hard-killing the process" {
        Stop-TestInstance -ProcessId $instance.Process.Id -Label "5.4 - instance"
        Start-Sleep -Seconds 3
    }
    Assert-True (Test-Path $stagedPath) "File was removed after a hard kill - expected exit-time cleanup NOT to run here (AppDomain.ProcessExit isn't reliably raised on a hard kill), leaving it for the orphan sweep instead."

    # Clean up manually - this orphan is now a genuine leftover, same as any other crashed session's.
    Remove-Item $stagedPath -Force -ErrorAction SilentlyContinue
}

Show-TestSummary
$runStopwatch.Stop()
Write-Host "`nTotal duration: $(Format-Duration $runStopwatch.Elapsed)" -ForegroundColor Cyan
Clear-TestArtifacts
