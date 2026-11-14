<#
.SYNOPSIS
    Tests for crash recovery (orphan sweep) and exit-time cleanup. Each test launches its OWN
    disposable instance via Start-TestInstance.ps1, since these tests kill/close processes and
    can't safely share an instance with other tests the way Run-StagingTests.ps1's do.

.PARAMETER Platform
    "x64" or "x86" - which build each disposable instance launches as. Default "x64".

.EXAMPLE
    .\Run-CrashRecoveryTests.ps1 -Platform x86
#>
param(
    [ValidateSet("x64", "x86")]
    [string]$Platform = "x64"
)

. (Join-Path $PSScriptRoot "TestHelpers.ps1")
$startInstanceScript = Join-Path $PSScriptRoot "Start-TestInstance.ps1"

Invoke-Test "4.1 - Orphaned working copy is swept on the next write-intent open" {
    $instanceA = & $startInstanceScript -Platform $Platform
    Write-InstanceStarted -ProcessId $instanceA.Process.Id -Label "4.1 - instanceA"
    $copiesBefore = Wait-ForStagedCopy -TimeoutSeconds 30
    Assert-True ($copiesBefore.Count -gt 0) "Instance A never staged a working copy."
    $orphanPath = $copiesBefore[0].FullName

    Stop-TestInstance -ProcessId $instanceA.Process.Id -Label "4.1 - instanceA"
    Start-Sleep -Milliseconds 500
    Assert-True (Test-Path $orphanPath) "Orphaned file disappeared immediately on kill - expected it to persist until the NEXT write-intent open (this is 5.4's own distinction, worth it failing loudly here if it ever changes)."

    # Opening ANY database elsewhere should trigger the orphan sweep - doesn't need to be the same file.
    $instanceB = & $startInstanceScript -Platform $Platform
    Write-InstanceStarted -ProcessId $instanceB.Process.Id -Label "4.1 - instanceB"
    try {
        Wait-ForStagedCopy -TimeoutSeconds 30 | Out-Null
        Assert-True (-not (Test-Path $orphanPath)) "Orphaned file from the killed instance A was NOT swept away by instance B's own write-intent open."
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
        $instance = & $startInstanceScript -Platform $Platform
        Write-InstanceStarted -ProcessId $instance.Process.Id -Label "4.3 - instance $i"
        $copies = Wait-ForStagedCopy -TimeoutSeconds 30
        Assert-True ($copies.Count -gt 0) "Instance $i never staged a working copy."
        $orphanPaths += $copies[0].FullName
        Stop-TestInstance -ProcessId $instance.Process.Id -Label "4.3 - instance $i"
        Start-Sleep -Milliseconds 300
    }

    $cleanupInstance = & $startInstanceScript -Platform $Platform
    Write-InstanceStarted -ProcessId $cleanupInstance.Process.Id -Label "4.3 - cleanup-trigger instance"
    try {
        Wait-ForStagedCopy -TimeoutSeconds 30 | Out-Null
        foreach ($path in $orphanPaths) {
            Assert-True (-not (Test-Path $path)) "Orphaned file $path was not cleared by the sweep."
        }
    } finally {
        Stop-TestInstance -ProcessId $cleanupInstance.Process.Id -Label "4.3 - cleanup-trigger instance"
    }
}

Invoke-Test "5.1/5.2 - Graceful close removes the staged copy promptly, with no hang" {
    $instance = & $startInstanceScript -Platform $Platform
    Write-InstanceStarted -ProcessId $instance.Process.Id -Label "5.1/5.2 - instance"
    $copies = Wait-ForStagedCopy -TimeoutSeconds 30
    Assert-True ($copies.Count -gt 0) "Working copy was never staged."
    $stagedPath = $copies[0].FullName

    # 5.2 is a real regression test from this backport's own development: an earlier draft used a
    # synchronous, sleep-based retry on Close() that measurably delayed shutdown. Timed here, not
    # just checked for completion, specifically to catch that regression if it ever reappears.
    $sw = [System.Diagnostics.Stopwatch]::StartNew()
    Write-Host "[PID $($instance.Process.Id)] Closing gracefully (5.1/5.2 - instance)..." -ForegroundColor DarkGray
    $instance.Process.CloseMainWindow() | Out-Null
    $exited = $instance.Process.WaitForExit(15000)
    $sw.Stop()
    Write-Host "[PID $($instance.Process.Id)] $(if ($exited) { 'Exited' } else { 'Still running' }) (5.1/5.2 - instance), after $($sw.ElapsedMilliseconds) ms." -ForegroundColor DarkGray
    Assert-True $exited "App did not exit within 15 seconds of a graceful close request."
    Assert-True ($sw.ElapsedMilliseconds -lt 5000) "Graceful close took $($sw.ElapsedMilliseconds) ms - expected under 5000ms (regression: an earlier draft's sleep-based retry measurably delayed shutdown)."

    # Exit-time cleanup is a separate mechanism from the orphan sweep (§4) - it should remove the
    # copy shortly after the process has actually exited, without needing another instance to open.
    $cleanupDeadline = (Get-Date).AddSeconds(10)
    $stillThere = $true
    while ((Get-Date) -lt $cleanupDeadline) {
        if (-not (Test-Path $stagedPath)) { $stillThere = $false; break }
        Start-Sleep -Milliseconds 300
    }
    Assert-True (-not $stillThere) "Staged copy was not removed by exit-time cleanup within ~10 seconds of a graceful close."
}

Invoke-Test "5.4 - A hard-killed process's own copy is NOT cleaned up by exit-time cleanup" {
    # Distinct from 4.1 above: 4.1 confirms the orphan DOES eventually get cleaned up, by a LATER
    # instance's own open. This test confirms the DIFFERENT mechanism (exit-time cleanup, via
    # AppDomain.ProcessExit) does NOT fire on a hard kill, since that event isn't reliably raised
    # on one - the file should still be sitting there immediately after, not vanish on its own.
    $instance = & $startInstanceScript -Platform $Platform
    Write-InstanceStarted -ProcessId $instance.Process.Id -Label "5.4 - instance"
    $copies = Wait-ForStagedCopy -TimeoutSeconds 30
    Assert-True ($copies.Count -gt 0) "Working copy was never staged."
    $stagedPath = $copies[0].FullName

    Stop-TestInstance -ProcessId $instance.Process.Id -Label "5.4 - instance"
    Start-Sleep -Seconds 3
    Assert-True (Test-Path $stagedPath) "File was removed after a hard kill - expected exit-time cleanup NOT to run here (AppDomain.ProcessExit isn't reliably raised on a hard kill), leaving it for the orphan sweep instead."

    # Clean up manually - this orphan is now a genuine leftover, same as any other crashed session's.
    Remove-Item $stagedPath -Force -ErrorAction SilentlyContinue
}

Show-TestSummary
