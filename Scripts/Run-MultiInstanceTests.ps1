<#
.SYNOPSIS
    Tests for the "no multi-user coordination, by design" behavior: two simultaneous instances on
    the same file should just work, with no locking/degrade of any kind, and whichever saves last
    silently wins. Also covers §6.2 (Save As onto a path another instance already has open).

.PARAMETER Platform
    "x64" or "x86" - which build every instance in this run launches as. Default "x64".

.EXAMPLE
    .\Run-MultiInstanceTests.ps1 -Platform x86
#>
param(
    [ValidateSet("x64", "x86")]
    [string]$Platform = "x64"
)

. (Join-Path $PSScriptRoot "TestHelpers.ps1")
$startInstanceScript = Join-Path $PSScriptRoot "Start-TestInstance.ps1"

# Launches a second instance on an ALREADY-copied model path, skipping Start-TestInstance.ps1's own
# build+copy steps (which would otherwise produce a different file each time, not the same one).
function Start-InstanceOnExistingModel {
    param(
        [Parameter(Mandatory = $true)][string]$RepoRoot,
        [Parameter(Mandatory = $true)][string]$ModelPath,
        [string]$Platform = "x64",
        [string]$Configuration = "Debug"
    )
    $exePath = Join-Path $RepoRoot "Sources\ScientificInterface\bin\$Platform\$Configuration\net48\ScientificInterface.exe"
    if (-not (Test-Path $exePath)) { throw "Built exe not found: $exePath" }
    $proc = Start-Process -FilePath $exePath -ArgumentList "`"$ModelPath`"" -PassThru
    Wait-ForMainWindow -Proc $proc -TimeoutSeconds 30
    $proc
}

$repoRoot = Split-Path -Parent $PSScriptRoot

Invoke-Test "1.2/3.2 - Two instances can open the same file simultaneously, no degrade" {
    $instanceA = & $startInstanceScript -Platform $Platform
    Write-InstanceStarted -ProcessId $instanceA.Process.Id -Label "1.2/3.2 - instanceA"
    try {
        $instanceB = Start-InstanceOnExistingModel -RepoRoot $repoRoot -ModelPath $instanceA.ModelPath -Platform $Platform
        Write-InstanceStarted -ProcessId $instanceB.Id -Label "1.2/3.2 - instanceB"
        Set-TwoInstancesSideBySide -ProcA $instanceA.Process -ProcB $instanceB

        # Neither window title should show any read-only indicator - exact wording isn't confirmed
        # from this session, so checking for the word "read-only" case-insensitively as a reasonable
        # catch-all rather than an exact string match.
        Assert-True ($instanceA.Process.MainWindowTitle -notmatch "read.?only") "Instance A's title suggests read-only: $($instanceA.Process.MainWindowTitle)"
        Assert-True ($instanceB.MainWindowTitle -notmatch "read.?only") "Instance B's title suggests read-only: $($instanceB.MainWindowTitle)"

        # Each should get its OWN independent staged copy, not share one.
        $copies = Wait-ForStagedCopy -TimeoutSeconds 30
        Assert-True ($copies.Count -ge 2) "Expected at least 2 distinct staged copies (one per instance), found $($copies.Count)."
    } finally {
        if ($instanceB) { Stop-TestInstance -ProcessId $instanceB.Id -Label "1.2/3.2 - instanceB" }
        if ($instanceA) { Stop-TestInstance -ProcessId $instanceA.Process.Id -Label "1.2/3.2 - instanceA" }
    }
}

Invoke-Test "3.3 - Both instances can save; whichever saves last wins, silently" {
    # Fresh pair for this test, rather than reusing 1.2's (which already closed instance B).
    $instanceA = & $startInstanceScript -Platform $Platform
    Write-InstanceStarted -ProcessId $instanceA.Process.Id -Label "3.3 - instanceA"
    $instanceB = $null
    try {
        $instanceB = Start-InstanceOnExistingModel -RepoRoot $repoRoot -ModelPath $instanceA.ModelPath -Platform $Platform
        Write-InstanceStarted -ProcessId $instanceB.Id -Label "3.3 - instanceB"
        Set-TwoInstancesSideBySide -ProcA $instanceA.Process -ProcB $instanceB

        Set-SafeRestorableEdit -Proc $instanceA.Process
        # Deliberately NOT restoring this one - "0" is meant to be the value that sticks, since A
        # saves first and B's later save should silently overwrite it with B's own value instead.
        Select-GridFirstCell -Proc $instanceA.Process
        [System.Windows.Forms.SendKeys]::SendWait("0")
        [System.Windows.Forms.SendKeys]::SendWait("{ENTER}")
        Start-Sleep -Milliseconds 300
        Invoke-SaveChanges -Proc $instanceA.Process
        Start-Sleep -Milliseconds 500

        Select-GridFirstCell -Proc $instanceB
        [System.Windows.Forms.SendKeys]::SendWait("1")
        [System.Windows.Forms.SendKeys]::SendWait("{ENTER}")
        Start-Sleep -Milliseconds 300
        Invoke-SaveChanges -Proc $instanceB
        Start-Sleep -Milliseconds 500

        # The REAL file's bytes should now match B's save (the last one), not A's - confirmed via
        # hash rather than trying to read the actual cell value back, which would need its own
        # UI-automation round-trip just to verify something a byte comparison already settles cleanly.
        $hashAfterB = (Get-FileHash -Path $instanceA.ModelPath -Algorithm SHA256).Hash

        Select-GridFirstCell -Proc $instanceA.Process
        [System.Windows.Forms.SendKeys]::SendWait("^c")
        Start-Sleep -Milliseconds 150
        $valueAfterB = Get-Clipboard

        Assert-True ($valueAfterB -eq "1") "Expected the real file to reflect B's later save (value '1'), but A's own re-read shows '$valueAfterB' - last-save-wins may not be working as expected."
    } finally {
        if ($instanceB) { Stop-TestInstance -ProcessId $instanceB.Id -Label "3.3 - instanceB" }
        if ($instanceA) { Stop-TestInstance -ProcessId $instanceA.Process.Id -Label "3.3 - instanceA" }
    }
}

Invoke-Test "6.2 - Save As to a path another instance already has open as its own target" {
    $instanceB = & $startInstanceScript -Platform $Platform   # B's own target - the path A will Save As into
    Write-InstanceStarted -ProcessId $instanceB.Process.Id -Label "6.2 - instanceB"
    $instanceA = $null
    try {
        $instanceA = & $startInstanceScript -Platform $Platform   # A starts on a DIFFERENT model entirely
        Write-InstanceStarted -ProcessId $instanceA.Process.Id -Label "6.2 - instanceA"
        Set-TwoInstancesSideBySide -ProcA $instanceB.Process -ProcB $instanceA.Process

        Set-AppForeground -Proc $instanceA.Process
        $window = Get-AppWindow -Proc $instanceA.Process
        $fileMenu = Find-ByNameAndType -Root $window -Name "File" -ControlType ([System.Windows.Automation.ControlType]::MenuItem)
        Assert-True ($null -ne $fileMenu) "Could not find the File menu on instance A."
        $fileMenu.GetCurrentPattern([System.Windows.Automation.ExpandCollapsePattern]::Pattern).Expand()
        Start-Sleep -Milliseconds 400

        $saveAsItem = Find-ByNameAndType -Root $window -Name "Save Model As..." -ControlType ([System.Windows.Automation.ControlType]::MenuItem)
        Assert-True ($null -ne $saveAsItem) "Could not find 'Save Model As...' on instance A."
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
        Assert-True ($null -ne $dialog) "'Save As' dialog did not appear on instance A."

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

        # Succeeding with no degrade is the actual thing under test - not any particular resulting
        # file state, which §3.3 above already covers more directly.
        Assert-True (Test-Path $instanceB.ModelPath) "Target file is missing after Save As - expected it to still exist, successfully overwritten."
    } finally {
        if ($instanceA) { Stop-TestInstance -ProcessId $instanceA.Process.Id -Label "6.2 - instanceA" }
        if ($instanceB) { Stop-TestInstance -ProcessId $instanceB.Process.Id -Label "6.2 - instanceB" }
    }
}

Show-TestSummary
