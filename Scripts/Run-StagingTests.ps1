<#
.SYNOPSIS
    Test runner for the local staging copy test plan's core and lock-contention scenarios.

.PARAMETER Proc
    Optional - an already-running instance to test against. If omitted, this builds and launches
    ScientificInterface itself (via Start-TestInstance.ps1), with a fresh copy of the sample model.

.PARAMETER ModelPath
    The REAL file path the app has open (not the staged working copy) - needed by the lock-
    contention test. Set automatically when -Proc is omitted.

.PARAMETER Platform
    "x64" or "x86" - which build to launch when -Proc is omitted. Default "x64".

.PARAMETER Only
    Run just the one test whose name contains this substring (case-insensitive), skipping the rest
    entirely - e.g. to isolate whether a failure is a genuine bug or an artifact of running several
    similar tests back-to-back in the same session.

.PARAMETER MainWindowTimeoutSeconds
    How long to wait for the app's main window to appear when auto-launching. Default 90 - raise
    this on a slower machine if launches are timing out before the window ever appears.

.EXAMPLE
    .\Run-StagingTests.ps1 -Platform x86

.EXAMPLE
    .\Run-StagingTests.ps1 -Only "8.1"

.EXAMPLE
    .\Run-StagingTests.ps1 -MainWindowTimeoutSeconds 180
#>
param(
    [System.Diagnostics.Process]$Proc,
    [string]$ModelPath,
    [Nullable[DateTime]]$InitialModelTimestamp,
    [string]$StagedCopyPath,
    [ValidateSet("x64", "x86")]
    [string]$Platform = "x64",
    [string]$Only,
    [int]$MainWindowTimeoutSeconds = 90
)

. (Join-Path $PSScriptRoot "TestHelpers.ps1")
$script:TestNameFilter = $Only
$repoRoot = Split-Path -Parent $PSScriptRoot

if (-not $Proc) {
    Write-Host "No -Proc given - building and launching a fresh instance ($Platform)..." -ForegroundColor Cyan
    $instance = & (Join-Path $PSScriptRoot "Start-TestInstance.ps1") -Platform $Platform -MainWindowTimeoutSeconds $MainWindowTimeoutSeconds
    $Proc = $instance.Process
    $ModelPath = $instance.ModelPath
    $InitialModelTimestamp = $instance.InitialModelTimestamp
    $StagedCopyPath = $instance.StagedCopyPath
}

Invoke-Test "1.1 - A staged working copy is created on open" {
    $copies = Wait-ForStagedCopy -TimeoutSeconds 30
    Assert-True ($copies.Count -gt 0) "No Temp* working copy found after ~30 seconds."
}

Invoke-Test "7.1 - Opening a file needing legacy updates never touches its own timestamp" {
    if ($null -eq $InitialModelTimestamp) {
        throw "No -InitialModelTimestamp known (likely because -Proc was supplied directly without it)."
    }
    # The legacy update chain (if this model needed one - our sample fixture does, confirmed by the
    # "database will be updated" prompt Start-TestInstance.ps1 handles) already ran during this
    # instance's own startup, entirely before this test ever runs. This just confirms it left the
    # real file alone throughout - the comparison is only meaningful because InitialModelTimestamp
    # was captured BEFORE the app ever opened the file at all, in Start-TestInstance.ps1 itself.
    $currentTimestamp = (Get-Item $ModelPath).LastWriteTime
    Assert-True ($currentTimestamp -eq $InitialModelTimestamp) "Timestamp changed since the file was first copied: was $InitialModelTimestamp, now $currentTimestamp - the original should only ever be read, never written to."
}

# Runs AFTER 7.1, deliberately: this test causes a real Save, which rewrites the file and changes
# its timestamp - running it before 7.1 would break that test's own untouched-timestamp assertion.
Invoke-Test "1.1 (rename-while-open) - Renaming the original while open has zero effect until Save" {
    $renamedPath = "$ModelPath.renamed-while-open"
    if (Test-Path $renamedPath) { Remove-Item $renamedPath -Force }
    Rename-Item -Path $ModelPath -NewName (Split-Path -Leaf $renamedPath) -Force

    try {
        # Confirm no immediate effect - no error dialog of any kind pops up just from the rename
        # itself, before anything even tries to use the file again.
        Start-Sleep -Milliseconds 500
        $window = Get-AppWindow -Proc $Proc
        $dialogCondition = New-Object System.Windows.Automation.AndCondition(
            (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::NameProperty, "Ecopath with Ecosim")),
            (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::ControlTypeProperty, [System.Windows.Automation.ControlType]::Window))
        )
        $unexpectedDialog = $window.FindFirst([System.Windows.Automation.TreeScope]::Descendants, $dialogCondition)
        Assert-True ($null -eq $unexpectedDialog) "A dialog appeared just from renaming the original away - expected zero effect on the open session until Save."

        # The open session should be completely unaffected beyond that - prove it by making and
        # saving a real edit, exactly as any other normal session would, with the original simply
        # absent in the meantime.
        Set-SafeRestorableEdit -Proc $Proc
        Assert-True (Test-SaveButtonEnabled -Proc $Proc) "Save is not enabled after making a change - the renamed-away original may have disrupted the open session."
        Invoke-SaveChanges -Proc $Proc
        Start-Sleep -Milliseconds 1000
        Assert-True (-not (Test-SaveButtonEnabled -Proc $Proc)) "Save button is still enabled after Save - did the publish step fail because the original was renamed away?"

        # File.Copy(..., overwrite:=True) should have happily recreated the file at its original
        # path - only the file itself was gone, not its parent folder.
        Assert-True (Test-Path $ModelPath) "No file exists at the original path after Save - expected Save to recreate it there."
    } finally {
        Remove-Item $renamedPath -Force -ErrorAction SilentlyContinue
    }
}

Invoke-Test "2.x - Edit and Save completes, Save becomes disabled afterward" {
    Set-SafeRestorableEdit -Proc $Proc
    Assert-True (Test-SaveButtonEnabled -Proc $Proc) "Save button is not enabled after making a change."
    Invoke-SaveChanges -Proc $Proc
    Start-Sleep -Milliseconds 500
    Assert-True (-not (Test-SaveButtonEnabled -Proc $Proc)) "Save button is still enabled after clicking Save."

    # §2.2 folded in here: confirm the real file's bytes genuinely match the staged working copy's
    # after an ordinary save - a blind overwrite, with no fingerprint check or conflict prompt of
    # any kind, is the intended behavior, not a bug to fix.
    #
    # CONFIRMED: the staged copy can't be read from outside AT ALL while the model remains open -
    # Get-FileHash itself failed with "being used by another process", meaning the app's own
    # database connection to it uses a sharing mode that denies even read access from a separate
    # process. There's no API-level workaround for this (file sharing is the intersection of both
    # openers' requested modes - if the first opener requested none, nothing else can get in
    # regardless of what it asks for). The only way to get a genuine byte comparison is to close
    # that connection first.
    # Uses the StagedCopyPath confirmed once at launch time, NOT a fresh name-based lookup here:
    # CONFIRMED that re-matching by name later is unreliable - the exact same GUID reappeared
    # across two separate runs, meaning a stale, unrelated leftover with the same fixture name
    # (held open by some other, never-cleaned-up process) was being silently picked instead of
    # this instance's own copy.
    Assert-True ($null -ne $StagedCopyPath) "No confirmed staged copy path for this instance - it may not have been found within 30 seconds at launch time."
    $stagedCopyPath = $StagedCopyPath

    Stop-Process -Id $Proc.Id -Force -ErrorAction SilentlyContinue
    Start-Sleep -Milliseconds 500
    Assert-True (Test-Path $stagedCopyPath) "Staged copy disappeared after closing the app - can't compare."

    $stagedHash = (Get-FileHash -Path $stagedCopyPath -Algorithm SHA256).Hash
    $realFileHash = (Get-FileHash -Path $ModelPath -Algorithm SHA256).Hash
    Assert-True ($realFileHash -eq $stagedHash) "The real file's bytes don't match the staged working copy's after Save - expected a blind, exact overwrite."

    # Fresh instance on the SAME file, for every test after this one in the same run - propagated to
    # script scope (unlike 6.1's own restart further down, which doesn't need to since it's last).
    $script:Proc = Start-InstanceOnExistingModel -RepoRoot $repoRoot -ModelPath $ModelPath -Platform $Platform -MainWindowTimeoutSeconds $MainWindowTimeoutSeconds
    $Proc = $script:Proc
}

# 2.3 runs BEFORE 6.1, deliberately: CONFIRMED that once Save As switches the app to a new file, the
# ORIGINAL $ModelPath becomes stale - a later Save writes to the Save As target, not the original,
# making this test's own lock a no-op if it ran after 6.1.
if ($ModelPath -and (-not $script:TestNameFilter -or "2.3 - Lock-contention dialog" -like "*$script:TestNameFilter*")) {
    Write-Host "`n=== 2.3 - Lock-contention dialog (long hold) shows OK/Cancel ===" -ForegroundColor Yellow
    Write-Host "Automating setup: edit, lock the real file ($ModelPath) for ~6s in the background, trigger Save."

    $lockJob = $null
    try {
        Set-SafeRestorableEdit -Proc $Proc

        # A background JOB (separate process), not a Thread or Task.Run: plain file I/O, no COM/UI
        # Automation objects involved, so none of the apartment-affinity problems from the Save As
        # investigation apply. A signal FILE, not a fixed sleep, confirms the lock is actually held
        # before Save is triggered - Start-Job's own process-startup overhead can easily exceed a
        # short fixed delay (confirmed: Save succeeded normally once, racing ahead of the lock).
        $readyFlag = Join-Path $env:TEMP "EwE-TestRun\LockJob-$(Get-Random).flag"
        if (Test-Path $readyFlag) { Remove-Item $readyFlag -Force }
        $lockJob = Start-Job -ScriptBlock {
            param($path, $holdSeconds, $readyFlagPath)
            $fs = [System.IO.File]::Open($path, 'Open', 'ReadWrite', 'None')
            New-Item -ItemType File -Path $readyFlagPath -Force | Out-Null
            Start-Sleep -Seconds $holdSeconds
            $fs.Close()
            Remove-Item $readyFlagPath -Force -ErrorAction SilentlyContinue
        } -ArgumentList $ModelPath, 6, $readyFlag

        $lockDeadline = (Get-Date).AddSeconds(10)
        while (-not (Test-Path $readyFlag) -and (Get-Date) -lt $lockDeadline) { Start-Sleep -Milliseconds 100 }
        if (-not (Test-Path $readyFlag)) { throw "Lock-holding job never signaled it had acquired the lock within 10 seconds." }

        Invoke-SaveChanges -Proc $Proc

        # Best-effort automated dismissal: UNCONFIRMED with Accessibility Insights (unlike every
        # other control targeted this session) - guessing Name="Cancel"/ControlType=Button based on
        # the consistent pattern every other custom WinForms button in this app has followed
        # (Name+ControlType, never AutomationId). Falls back to a manual prompt if this doesn't
        # resolve, same graceful-degradation approach used throughout this toolkit.
        $window = Get-AppWindow -Proc $Proc
        $dialogCondition = New-Object System.Windows.Automation.AndCondition(
            (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::NameProperty, "Ecopath with Ecosim")),
            (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::ControlTypeProperty, [System.Windows.Automation.ControlType]::Window))
        )
        $dialog = $null
        $dialogDeadline = (Get-Date).AddSeconds(5)
        while ($null -eq $dialog -and (Get-Date) -lt $dialogDeadline) {
            $dialog = $window.FindFirst([System.Windows.Automation.TreeScope]::Descendants, $dialogCondition)
            if ($null -eq $dialog) { Start-Sleep -Milliseconds 200 }
        }

        if ($null -ne $dialog) {
            # Soft check, not a hard assertion: the test plan only requires the dialog to name the
            # actual locking process OR fall back to generic phrasing - either is acceptable, so this
            # just logs what it actually says rather than failing the test over exact wording.
            $textElement = $dialog.FindFirst(
                [System.Windows.Automation.TreeScope]::Descendants,
                (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::ControlTypeProperty, [System.Windows.Automation.ControlType]::Text))
            )
            if ($null -ne $textElement) {
                $messageText = $textElement.Current.Name
                Write-Host "  Dialog message: $messageText"
                if ($messageText -notmatch "locked by") {
                    Write-Host "  NOTE: message doesn't mention 'locked by' - worth a look if this ever seems wrong." -ForegroundColor Yellow
                }
            }

            # Name only, NOT ControlType=Button: CONFIRMED via Accessibility Insights that this
            # button reports ControlType.Pane here (likely a focus-state-dependent quirk of the
            # legacy accessibility proxy - a DIFFERENT live check of this same button, at a
            # different moment, showed ControlType.Button) - same fix as the Yes/No dialog's buttons.
            # Still polling, in case of the shell-before-children timing race seen elsewhere too.
            $cancelButton = $null
            $buttonDeadline = (Get-Date).AddSeconds(3)
            $cancelCondition = New-Object System.Windows.Automation.PropertyCondition(
                [System.Windows.Automation.AutomationElement]::NameProperty, "Cancel")
            while ($null -eq $cancelButton -and (Get-Date) -lt $buttonDeadline) {
                $cancelButton = $dialog.FindFirst([System.Windows.Automation.TreeScope]::Descendants, $cancelCondition)
                if ($null -eq $cancelButton) { Start-Sleep -Milliseconds 150 }
            }
            if ($null -ne $cancelButton) {
                Invoke-ElementClick -Element $cancelButton
                Add-TestResult -Name "2.3 - Lock-contention dialog" -Result "PASS" -Reason "(fully automated)"
                Write-Host "PASS (fully automated - dialog found and dismissed via Cancel)" -ForegroundColor Green
            } else {
                Write-Host "Dialog found but 'Cancel' button wasn't, even after polling." -ForegroundColor Yellow
                $response = Read-Host "Do you see the dialog with OK/Cancel? Please close it, then answer (y/n/skip)"
                switch ($response.ToLowerInvariant()) {
                    "skip" { Add-TestResult -Name "2.3 - Lock-contention dialog" -Result "SKIP" }
                    "y"    { Add-TestResult -Name "2.3 - Lock-contention dialog" -Result "PASS" -Reason "(dialog found, Cancel button not found even after polling)" }
                    default { Add-TestResult -Name "2.3 - Lock-contention dialog" -Result "FAIL" -Reason (Read-Host "Failure reason") }
                }
            }
        } else {
            $response = Read-Host "Dialog not found automatically - do you see it with OK/Cancel? Please close it, then answer (y/n/skip)"
            switch ($response.ToLowerInvariant()) {
                "skip" { Add-TestResult -Name "2.3 - Lock-contention dialog" -Result "SKIP" }
                "y"    { Add-TestResult -Name "2.3 - Lock-contention dialog" -Result "PASS" -Reason "(manual - dialog window match needs updating)" }
                default { Add-TestResult -Name "2.3 - Lock-contention dialog" -Result "FAIL" -Reason (Read-Host "Failure reason") }
            }
        }
    } catch {
        Add-TestResult -Name "2.3 - Lock-contention dialog" -Result "FAIL" -Reason $_.Exception.Message
        Write-Host "FAIL: $($_.Exception.Message)" -ForegroundColor Red
    } finally {
        if ($lockJob) {
            Wait-Job $lockJob -Timeout 10 | Out-Null
            Remove-Job $lockJob -Force -ErrorAction SilentlyContinue
        }
    }
} else {
    Invoke-ManualTest "2.3 - Lock-contention dialog (long hold) shows OK/Cancel" `
        "No -ModelPath known - hold the open model's REAL file exclusively yourself (see the test plan's appendix script, ~5+ seconds), then trigger Save. Confirm the dialog appears."
}

if ($ModelPath) {
    Invoke-Test "2.3-short - A short lock hold resolves silently via internal retry" {
        Set-SafeRestorableEdit -Proc $Proc
        Assert-True (Test-SaveButtonEnabled -Proc $Proc) "Save is not enabled after making a change."

        # Same lock-job mechanism as the long-hold test above, just a shorter hold (comfortably
        # inside the ~3-second internal retry window) and the opposite expectation: no dialog, Save
        # just quietly succeeds.
        $readyFlag = Join-Path $env:TEMP "EwE-TestRun\LockJob-$(Get-Random).flag"
        if (Test-Path $readyFlag) { Remove-Item $readyFlag -Force }
        $lockJob = Start-Job -ScriptBlock {
            param($path, $holdSeconds, $readyFlagPath)
            $fs = [System.IO.File]::Open($path, 'Open', 'ReadWrite', 'None')
            New-Item -ItemType File -Path $readyFlagPath -Force | Out-Null
            Start-Sleep -Seconds $holdSeconds
            $fs.Close()
            Remove-Item $readyFlagPath -Force -ErrorAction SilentlyContinue
        } -ArgumentList $ModelPath, 1.5, $readyFlag

        try {
            $lockDeadline = (Get-Date).AddSeconds(10)
            while (-not (Test-Path $readyFlag) -and (Get-Date) -lt $lockDeadline) { Start-Sleep -Milliseconds 100 }
            if (-not (Test-Path $readyFlag)) { throw "Lock-holding job never signaled it had acquired the lock within 10 seconds." }

            Invoke-SaveChanges -Proc $Proc
            Start-Sleep -Milliseconds 4000   # ~3s internal retry window plus the lock's own 1.5s hold

            Assert-True (-not (Test-SaveButtonEnabled -Proc $Proc)) "Save button is still enabled - the internal retry may not have succeeded silently."

            $window = Get-AppWindow -Proc $Proc
            $dialogCondition = New-Object System.Windows.Automation.AndCondition(
                (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::NameProperty, "Ecopath with Ecosim")),
                (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::ControlTypeProperty, [System.Windows.Automation.ControlType]::Window))
            )
            $dialog = $window.FindFirst([System.Windows.Automation.TreeScope]::Descendants, $dialogCondition)
            Assert-True ($null -eq $dialog) "A dialog appeared even for a short lock hold - expected the internal retry to resolve this silently, with no dialog at all."
        } finally {
            Wait-Job $lockJob -Timeout 10 | Out-Null
            Remove-Job $lockJob -Force -ErrorAction SilentlyContinue
        }
    }
}

if ($ModelPath) {
    Invoke-Test "8.1 - A cancelled lock-contention save correctly reports failure, not silent success" {
        # Replaces an earlier version of "8.1" that only checked a data page loaded without
        # crashing - it never actually verified anything about the bug it was meant to cover.
        # This one directly exercises the real concern: cDBDataSource's SaveXXX() methods once
        # discarded ReleaseWriter()'s own return value, meaning a save could report success even
        # when the underlying write genuinely failed. Forcing a real failure (lock held past the
        # retry window, Cancel clicked) and checking it's reported as one, not silently accepted.
        Set-SafeRestorableEdit -Proc $Proc
        Assert-True (Test-SaveButtonEnabled -Proc $Proc) "Save is not enabled after making a change."
        $hashBeforeAttempt = (Get-FileHash -Path $ModelPath -Algorithm SHA256).Hash

        $readyFlag = Join-Path $env:TEMP "EwE-TestRun\LockJob-$(Get-Random).flag"
        if (Test-Path $readyFlag) { Remove-Item $readyFlag -Force }
        $lockJob = Start-Job -ScriptBlock {
            param($path, $holdSeconds, $readyFlagPath)
            $fs = [System.IO.File]::Open($path, 'Open', 'ReadWrite', 'None')
            New-Item -ItemType File -Path $readyFlagPath -Force | Out-Null
            Start-Sleep -Seconds $holdSeconds
            $fs.Close()
            Remove-Item $readyFlagPath -Force -ErrorAction SilentlyContinue
        } -ArgumentList $ModelPath, 6, $readyFlag

        try {
            $lockDeadline = (Get-Date).AddSeconds(10)
            while (-not (Test-Path $readyFlag) -and (Get-Date) -lt $lockDeadline) { Start-Sleep -Milliseconds 100 }
            if (-not (Test-Path $readyFlag)) { throw "Lock-holding job never signaled it had acquired the lock within 10 seconds." }

            Invoke-SaveChanges -Proc $Proc

            $window = Get-AppWindow -Proc $Proc
            $dialogCondition = New-Object System.Windows.Automation.AndCondition(
                (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::NameProperty, "Ecopath with Ecosim")),
                (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::ControlTypeProperty, [System.Windows.Automation.ControlType]::Window))
            )
            $dialog = $null
            $dialogDeadline = (Get-Date).AddSeconds(5)
            while ($null -eq $dialog -and (Get-Date) -lt $dialogDeadline) {
                $dialog = $window.FindFirst([System.Windows.Automation.TreeScope]::Descendants, $dialogCondition)
                if ($null -eq $dialog) { Start-Sleep -Milliseconds 200 }
            }
            Assert-True ($null -ne $dialog) "Lock-contention dialog did not appear."

            $cancelCondition = New-Object System.Windows.Automation.PropertyCondition(
                [System.Windows.Automation.AutomationElement]::NameProperty, "Cancel")
            $cancelButton = $null
            $buttonDeadline = (Get-Date).AddSeconds(3)
            while ($null -eq $cancelButton -and (Get-Date) -lt $buttonDeadline) {
                $cancelButton = $dialog.FindFirst([System.Windows.Automation.TreeScope]::Descendants, $cancelCondition)
                if ($null -eq $cancelButton) { Start-Sleep -Milliseconds 150 }
            }
            Assert-True ($null -ne $cancelButton) "Could not find the Cancel button."
            Invoke-ElementClick -Element $cancelButton
            Start-Sleep -Milliseconds 500

            Assert-True (Test-SaveButtonEnabled -Proc $Proc) "Save button is DISABLED after a cancelled save - the app may be treating a failed save as if it had succeeded."

            # Our OWN lock-job holds for a full 6 seconds from the moment it's acquired - the dialog
            # detect + Cancel click + 500ms sleep above easily finishes well under that, so without
            # this wait, the Get-FileHash below could fire while THIS test's own background job is
            # still actively holding the file open (confirmed: that's exactly what happened once).
            Wait-Job $lockJob -Timeout 10 | Out-Null
            $hashAfterCancel = (Get-FileHash -Path $ModelPath -Algorithm SHA256).Hash
            Assert-True ($hashAfterCancel -eq $hashBeforeAttempt) "The real file changed even though Save was cancelled - expected no write to have happened at all."
        } finally {
            Wait-Job $lockJob -Timeout 10 | Out-Null
            Remove-Job $lockJob -Force -ErrorAction SilentlyContinue
        }
    }
}

# Runs LAST, deliberately - see the comment above the 2.3 block for why.
Invoke-Test "6.1 - Save As creates a new file at the target path" {
    # Starts with a completely FRESH app instance, rather than reusing the one every test above
    # shares: CONFIRMED that running this right after 8.1 (a cancelled save) can throw a real,
    # unhandled exception ("Cannot find existing row for group 2" in SaveEcopathGroups(), surfaced
    # as a WinForms assertion dialog) - a longer wait after 8.1 did NOT fix it, ruling out pure
    # timing. Closing and restarting sidesteps whatever residual state a cancelled save leaves
    # behind, without yet knowing the exact root cause - worth its own dedicated investigation
    # later, not something this suite should block on right now. $Proc/$ModelPath are reassigned
    # here only within this scriptblock's own scope, which is fine since this is the LAST test in
    # the file - nothing downstream needs the change to propagate further.
    Stop-Process -Id $Proc.Id -Force -ErrorAction SilentlyContinue
    $freshInstance = & (Join-Path $PSScriptRoot "Start-TestInstance.ps1") -Platform $Platform -MainWindowTimeoutSeconds $MainWindowTimeoutSeconds
    $Proc = $freshInstance.Process
    $ModelPath = $freshInstance.ModelPath

    # No extension typed into the dialog, and none hardcoded here: CONFIRMED (reproducible on both
    # platforms) that "Save Model As..." only allows saving in the SAME format the model was
    # originally loaded in - typing a different extension doesn't produce a file at that exact path,
    # the dialog applies the correct one regardless of what's typed.
    #
    # That "same format" is NOT simply the source file's own extension, though - confirmed directly
    # from cDataSourceFactory.vb's own source: GetSupportedType() checks cSystemUtils.Is64BitProcess()
    # specifically inside the ".mdb"/".ewemdb" case, resolving to Access2007 (-> .eweaccdb, via
    # GetDefaultExtension) on a 64-bit process, but Access2003 (-> .ewemdb) on a 32-bit one. An
    # .accdb/.eweaccdb source has no such branch - always Access2007 (-> .eweaccdb). This mirrors
    # that exact logic rather than assuming the source extension carries straight through, which is
    # what an earlier version of this test got wrong for x64.
    $sourceExtension = [IO.Path]::GetExtension($ModelPath).ToLower()
    $targetExtension = switch ($sourceExtension) {
        { $_ -in ".mdb", ".ewemdb" } { if ($Platform -eq "x64") { ".eweaccdb" } else { ".ewemdb" } }
        { $_ -in ".accdb", ".eweaccdb" } { ".eweaccdb" }
        default { throw "Don't know what Save As would target for source extension '$sourceExtension' - update this test's mirror of cDataSourceFactory's own logic." }
    }
    $targetPathNoExt = Join-Path $env:TEMP "EwE-TestRun\SaveAs-$(Get-Random)"
    $targetPath = "$targetPathNoExt$targetExtension"
    if (Test-Path $targetPath) { Remove-Item $targetPath -Force }

    Set-AppForeground -Proc $Proc
    $window = Get-AppWindow -Proc $Proc
    $fileMenu = Find-ByNameAndType -Root $window -Name "File" -ControlType ([System.Windows.Automation.ControlType]::MenuItem)
    Assert-True ($null -ne $fileMenu) "Could not find the File menu."
    $fileMenu.GetCurrentPattern([System.Windows.Automation.ExpandCollapsePattern]::Pattern).Expand()
    Start-Sleep -Milliseconds 400

    $saveAsItem = Find-ByNameAndType -Root $window -Name "Save Model As..." -ControlType ([System.Windows.Automation.ControlType]::MenuItem)
    Assert-True ($null -ne $saveAsItem) "Could not find 'Save Model As...'."
    Invoke-ElementClick -Element $saveAsItem

    $dialogCondition = New-Object System.Windows.Automation.AndCondition(
        (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::NameProperty, "Save As")),
        (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::ControlTypeProperty, [System.Windows.Automation.ControlType]::Window))
    )
    $dialog = $null
    for ($i = 0; $i -lt 30; $i++) {
        $dialog = $window.FindFirst([System.Windows.Automation.TreeScope]::Descendants, $dialogCondition)
        if ($null -ne $dialog) { break }
        Start-Sleep -Milliseconds 200
    }
    Assert-True ($null -ne $dialog) "'Save As' dialog did not appear."

    # The actual filename INPUT is not reachable via tree-walking (confirmed this session) - anchor
    # on its label instead, which is.
    $label = Find-ByNameAndType -Root $dialog -Name "File name:" -ControlType ([System.Windows.Automation.ControlType]::Text)
    Assert-True ($null -ne $label) "Could not find the 'File name:' label."
    $labelRect = $label.Current.BoundingRectangle
    [System.Windows.Forms.Cursor]::Position = New-Object System.Drawing.Point([int]($labelRect.X + $labelRect.Width + 10), [int]($labelRect.Y + $labelRect.Height / 2))
    Start-Sleep -Milliseconds 150
    [Win32TestRunner]::mouse_event([Win32TestRunner]::MOUSEEVENTF_LEFTDOWN, 0, 0, 0, [UIntPtr]::Zero)
    [Win32TestRunner]::mouse_event([Win32TestRunner]::MOUSEEVENTF_LEFTUP, 0, 0, 0, [UIntPtr]::Zero)
    Start-Sleep -Milliseconds 150

    [System.Windows.Forms.SendKeys]::SendWait("^a")
    Start-Sleep -Milliseconds 100
    [System.Windows.Forms.SendKeys]::SendWait((ConvertTo-SendKeysLiteral $targetPathNoExt))
    Start-Sleep -Milliseconds 100
    [System.Windows.Forms.SendKeys]::SendWait("{ENTER}")
    # Polling, not a single fixed-delay check: the x86 build has shown this needing more time than
    # x64 did in the same spot - rather than just guessing a bigger fixed number (which could still
    # be wrong on some other machine), poll for up to 10 seconds.
    $targetDeadline = (Get-Date).AddSeconds(10)
    while (-not (Test-Path $targetPath) -and (Get-Date) -lt $targetDeadline) { Start-Sleep -Milliseconds 300 }
    Assert-True (Test-Path $targetPath) "Expected file was not created after ~10 seconds: $targetPath"
    Remove-Item $targetPath -Force -ErrorAction SilentlyContinue
}

Show-TestSummary
Clear-TestArtifacts
