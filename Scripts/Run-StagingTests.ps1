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

.EXAMPLE
    .\Run-StagingTests.ps1 -Platform x86
#>
param(
    [System.Diagnostics.Process]$Proc,
    [string]$ModelPath,
    [ValidateSet("x64", "x86")]
    [string]$Platform = "x64"
)

. (Join-Path $PSScriptRoot "TestHelpers.ps1")

if (-not $Proc) {
    Write-Host "No -Proc given - building and launching a fresh instance ($Platform)..." -ForegroundColor Cyan
    $instance = & (Join-Path $PSScriptRoot "Start-TestInstance.ps1") -Platform $Platform
    $Proc = $instance.Process
    $ModelPath = $instance.ModelPath
}

Invoke-Test "1.1 - A staged working copy is created on open" {
    $copies = Wait-ForStagedCopy -TimeoutSeconds 30
    Assert-True ($copies.Count -gt 0) "No Temp* working copy found after ~30 seconds."
}

Invoke-Test "2.x - Edit and Save completes, Save becomes disabled afterward" {
    Set-SafeRestorableEdit -Proc $Proc
    Assert-True (Test-SaveButtonEnabled -Proc $Proc) "Save button is not enabled after making a change."
    Invoke-SaveChanges -Proc $Proc
    Start-Sleep -Milliseconds 500
    Assert-True (-not (Test-SaveButtonEnabled -Proc $Proc)) "Save button is still enabled after clicking Save."
}

# 2.3 runs BEFORE 6.1, deliberately: CONFIRMED that once Save As switches the app to a new file, the
# ORIGINAL $ModelPath becomes stale - a later Save writes to the Save As target, not the original,
# making this test's own lock a no-op if it ran after 6.1.
if ($ModelPath) {
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
        $readyFlag = Join-Path $env:TEMP "EwE-LockJob-Ready-$(Get-Random).flag"
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

# Runs LAST, deliberately - see the comment above the 2.3 block for why.
Invoke-Test "6.1 - Save As creates a new file at the target path" {
    $targetPath = Join-Path $env:TEMP "EwE-AutoTest-SaveAs-$(Get-Random).eweaccdb"
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
    [System.Windows.Forms.SendKeys]::SendWait((ConvertTo-SendKeysLiteral $targetPath))
    Start-Sleep -Milliseconds 100
    [System.Windows.Forms.SendKeys]::SendWait("{ENTER}")
    Start-Sleep -Milliseconds 1000

    Assert-True (Test-Path $targetPath) "Expected file was not created: $targetPath"
    Remove-Item $targetPath -Force -ErrorAction SilentlyContinue
}

Invoke-ManualTest "8.1 - Sample data (ratings, diet composition, catch samples) loads correctly" `
    "Navigate to a data page with sample data (e.g. Diet composition) via the Navigator tree and confirm it loads without error."

Show-TestSummary
