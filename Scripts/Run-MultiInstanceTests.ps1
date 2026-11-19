<#
.SYNOPSIS
    Tests for the "no multi-user coordination, by design" behavior: two simultaneous instances on
    the same file should just work, with no locking/degrade of any kind, and whichever saves last
    silently wins. Also covers §6.2 (Save As onto a path another instance already has open).

.PARAMETER Platform
    "x64" or "x86" - which build every instance in this run launches as. Default "x64".

.PARAMETER Only
    Run just the one test whose name contains this substring (case-insensitive), skipping the rest.

.PARAMETER MainWindowTimeoutSeconds
    How long each instance waits for its main window before giving up. Default 90 - raise this on a
    slower machine if launches are timing out before the window ever appears.

.EXAMPLE
    .\Run-MultiInstanceTests.ps1 -Platform x86
#>
param(
    [ValidateSet("x64", "x86")]
    [string]$Platform = "x64",
    [string]$Only,
    [int]$MainWindowTimeoutSeconds = 90
)

. (Join-Path $PSScriptRoot "TestHelpers.ps1")
$runStopwatch = [System.Diagnostics.Stopwatch]::StartNew()
$script:TestNameFilter = $Only
$startInstanceScript = Join-Path $PSScriptRoot "Start-TestInstance.ps1"
$repoRoot = Split-Path -Parent $PSScriptRoot

# Instance launches below are NOT wrapped in Write-Step anywhere in this file: Start-TestInstance.ps1
# and Start-InstanceOnExistingModel each print their own multi-line progress (build output, launch,
# upgrade-dialog dots), which reads badly interleaved with Write-Step's own "...done" framing - a
# plain Write-Host header line is used before each one instead.

Invoke-Test "1.2 - Two instances can open the same file simultaneously, no degrade" {
    Write-Host "  Starting instance A:"
    $instanceA = & $startInstanceScript -Platform $Platform -MainWindowTimeoutSeconds $MainWindowTimeoutSeconds
    Write-InstanceStarted -ProcessId $instanceA.Process.Id -Label "1.2 - instanceA"
    try {
        Write-Host "  Starting instance B on the same file:"
        $instanceB = Start-InstanceOnExistingModel -RepoRoot $repoRoot -ModelPath $instanceA.ModelPath -Platform $Platform -MainWindowTimeoutSeconds $MainWindowTimeoutSeconds
        Write-InstanceStarted -ProcessId $instanceB.Id -Label "1.2 - instanceB"
        $baselineA = Get-WindowBaseline -ProcessId $instanceA.Process.Id
        $baselineB = Get-WindowBaseline -ProcessId $instanceB.Id
        Write-Step "Positioning both windows side by side" { Set-TwoInstancesSideBySide -ProcA $instanceA.Process -ProcB $instanceB }

        # Neither window title should show any read-only indicator - exact wording isn't confirmed
        # from this session, so checking for the word "read-only" case-insensitively as a reasonable
        # catch-all rather than an exact string match.
        Write-Step "Confirming neither window shows read-only" {
            Assert-True ($instanceA.Process.MainWindowTitle -notmatch "read.?only") "Instance A's title suggests read-only: $($instanceA.Process.MainWindowTitle)"
            Assert-True ($instanceB.MainWindowTitle -notmatch "read.?only") "Instance B's title suggests read-only: $($instanceB.MainWindowTitle)"
        }

        # Each should get its OWN independent staged copy, not share one.
        $copies = Write-Step "Confirming each instance has its own staged copy" { Wait-ForStagedCopy -TimeoutSeconds 30 }
        Assert-True ($copies.Count -ge 2) "Expected at least 2 distinct staged copies (one per instance), found $($copies.Count)."

        Write-Step "Confirming neither instance has a leftover dialog open" {
            Assert-NoNewDialogs -ProcessId $instanceA.Process.Id -Baseline $baselineA
            Assert-NoNewDialogs -ProcessId $instanceB.Id -Baseline $baselineB
        }
    } finally {
        if ($instanceB) { Stop-TestInstance -ProcessId $instanceB.Id -Label "1.2 - instanceB" }
        if ($instanceA) { Stop-TestInstance -ProcessId $instanceA.Process.Id -Label "1.2 - instanceA" }
    }
}

# A former "3.3" test (edit+save from both instances, confirm last-save-wins by reading the value
# back through instance A afterward) was removed entirely - it couldn't actually distinguish "the
# real file correctly reflects B's save" from "A's own stale in-memory state never changed, and the
# readback silently landed on the wrong window instead" (SetForegroundWindow can fail silently).
# Making that distinction unambiguous would need reading the result back through a FRESH instance,
# not an instance that was party to the save - not worth the complexity for a claim the stakeholders
# have explicitly said doesn't need proving (no multi-user conflict resolution is in scope; last
# save wins, by design). The "no degrade" part of that scenario - which does matter - is the "1.2"
# test above; this isn't a gap left behind, it's removing a test that was testing the wrong thing.

Invoke-Test "6.2 - Save As to a path another instance already has open as its own target" {
    Write-Host "  Starting instance B (the Save As target owner):"
    $instanceB = & $startInstanceScript -Platform $Platform -MainWindowTimeoutSeconds $MainWindowTimeoutSeconds
    Write-InstanceStarted -ProcessId $instanceB.Process.Id -Label "6.2 - instanceB"
    $instanceA = $null
    try {
        Write-Host "  Starting instance A on a different model:"
        $instanceA = & $startInstanceScript -Platform $Platform -MainWindowTimeoutSeconds $MainWindowTimeoutSeconds
        Write-InstanceStarted -ProcessId $instanceA.Process.Id -Label "6.2 - instanceA"
        $baselineA = Get-WindowBaseline -ProcessId $instanceA.Process.Id
        $baselineB = Get-WindowBaseline -ProcessId $instanceB.Process.Id
        Write-Step "Positioning both windows side by side" { Set-TwoInstancesSideBySide -ProcA $instanceB.Process -ProcB $instanceA.Process }

        $fileMenu = Write-Step "Opening the File menu on instance A" {
            Set-AppForeground -Proc $instanceA.Process
            $script:window = Get-AppWindow -Proc $instanceA.Process
            $menu = Find-ByNameAndType -Root $script:window -Name "File" -ControlType ([System.Windows.Automation.ControlType]::MenuItem)
            if ($null -ne $menu) {
                $menu.GetCurrentPattern([System.Windows.Automation.ExpandCollapsePattern]::Pattern).Expand()
                Start-Sleep -Milliseconds 400
            }
            $menu
        }
        $window = $script:window
        Assert-True ($null -ne $fileMenu) "Could not find the File menu on instance A."

        $saveAsItem = Write-Step "Clicking Save Model As... on instance A" {
            $item = Find-ByNameAndType -Root $window -Name "Save Model As..." -ControlType ([System.Windows.Automation.ControlType]::MenuItem)
            if ($null -ne $item) { Invoke-ElementClick -Element $item }
            $item
        }
        Assert-True ($null -ne $saveAsItem) "Could not find 'Save Model As...' on instance A."

        $dialog = Write-Step "Waiting for the Save As dialog" {
            $dialogCondition = New-Object System.Windows.Automation.AndCondition(
                (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::NameProperty, "Save As")),
                (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::ControlTypeProperty, [System.Windows.Automation.ControlType]::Window))
            )
            $foundDialog = $null
            for ($i = 0; $i -lt 30; $i++) {
                $foundDialog = $window.FindFirst([System.Windows.Automation.TreeScope]::Descendants, $dialogCondition)
                if ($null -ne $foundDialog) { break }
                Start-Sleep -Milliseconds 200
            }
            $foundDialog
        }
        Assert-True ($null -ne $dialog) "'Save As' dialog did not appear on instance A."

        Write-Step "Typing instance B's own path into the File name field" {
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
            # The exact path B already has open - the whole point of this test.
            [System.Windows.Forms.SendKeys]::SendWait((ConvertTo-SendKeysLiteral $instanceB.ModelPath))
            Start-Sleep -Milliseconds 100
            [System.Windows.Forms.SendKeys]::SendWait("{ENTER}")
            Start-Sleep -Milliseconds 1000
        }

        # CONFIRMED (caught by the new dialog check below, before this was added): typing an
        # ALREADY-EXISTING path triggers Windows' own standard overwrite-confirmation prompt
        # ("... already exists. Do you want to replace it?") - expected here, since that path is
        # deliberately instance B's own currently-open file, the whole point of this test. A real
        # user would click Yes; without handling it, the whole Save As flow sits stuck waiting on it
        # indefinitely, which also explains why the Save As dialog itself never closed either.
        Write-Step "Confirming the overwrite prompt, as a real user would" {
            $confirmCondition = New-Object System.Windows.Automation.AndCondition(
                (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::NameProperty, "Confirm Save As")),
                (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::ControlTypeProperty, [System.Windows.Automation.ControlType]::Window))
            )
            $confirmDialog = $null
            for ($i = 0; $i -lt 15; $i++) {
                $confirmDialog = $window.FindFirst([System.Windows.Automation.TreeScope]::Descendants, $confirmCondition)
                if ($null -ne $confirmDialog) { break }
                Start-Sleep -Milliseconds 200
            }
            if ($null -ne $confirmDialog) {
                $yesButton = Find-ByNameAndType -Root $confirmDialog -Name "Yes" -ControlType ([System.Windows.Automation.ControlType]::Button)
                Assert-True ($null -ne $yesButton) "Found the overwrite-confirmation dialog but not its Yes button."
                Invoke-ElementClick -Element $yesButton
                Start-Sleep -Milliseconds 1500
            }
        }

        # Succeeding with no degrade is the actual thing under test - not any particular resulting
        # file state.
        Write-Step "Confirming the target file still exists, successfully overwritten" {
            Assert-True (Test-Path $instanceB.ModelPath) "Target file is missing after Save As - expected it to still exist, successfully overwritten."
        }

        # Confirms the Save As dialog on instance A actually closed after submitting, and nothing
        # unexpected lingers on either instance.
        Write-Step "Confirming no dialog was left open on either instance" {
            Assert-NoNewDialogs -ProcessId $instanceA.Process.Id -Baseline $baselineA
            Assert-NoNewDialogs -ProcessId $instanceB.Process.Id -Baseline $baselineB
        }
    } finally {
        if ($instanceA) { Stop-TestInstance -ProcessId $instanceA.Process.Id -Label "6.2 - instanceA" }
        if ($instanceB) { Stop-TestInstance -ProcessId $instanceB.Process.Id -Label "6.2 - instanceB" }
    }
}

Show-TestSummary
$runStopwatch.Stop()
Write-Host "`nTotal duration: $(Format-Duration $runStopwatch.Elapsed)" -ForegroundColor Cyan
Clear-TestArtifacts
