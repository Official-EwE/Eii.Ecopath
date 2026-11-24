<#
.SYNOPSIS
    Validates DialogChecker.exe directly, before wiring Assert-NoUnexpectedDialogs into the full
    test suite - launches a fresh instance, lists its open windows, confirms Assert-NoUnexpectedDialogs
    passes with just the main window open, then opens a real dialog (the File menu's "Save Model
    As..." Save As dialog) and confirms it's correctly detected as unexpected.

.PARAMETER Platform
    "x64" or "x86". Default "x64".

.EXAMPLE
    .\Debug-DialogChecker.ps1
#>
param(
    [ValidateSet("x64", "x86")]
    [string]$Platform = "x64"
)

. (Join-Path $PSScriptRoot "TestHelpers.ps1")

Write-Host "Starting a fresh instance:"
$instance = & (Join-Path $PSScriptRoot "Start-TestInstance.ps1") -Platform $Platform
$proc = $instance.Process

try {
    Write-Host "`n=== Listing open windows with the app idle ==="
    Get-OpenWindows -ProcessId $proc.Id | Format-Table Name, ControlType, NativeWindowHandle, IsOffscreen -AutoSize

    Write-Host "Capturing a baseline, then confirming Assert-NoNewDialogs passes against it..."
    $baseline = Get-WindowBaseline -ProcessId $proc.Id
    try {
        Assert-NoNewDialogs -ProcessId $proc.Id -Baseline $baseline
        Write-Host "PASS - no new windows since the baseline." -ForegroundColor Green
    } catch {
        Write-Host "FAIL (unexpected, since nothing should have changed yet): $($_.Exception.Message)" -ForegroundColor Red
    }

    Write-Host "`n=== Opening a real dialog (Save Model As...) and confirming it's detected ==="
    Set-AppForeground -Proc $proc
    $window = Get-AppWindow -Proc $proc
    $fileMenu = Find-ByNameAndType -Root $window -Name "File" -ControlType ([System.Windows.Automation.ControlType]::MenuItem)
    $fileMenu.GetCurrentPattern([System.Windows.Automation.ExpandCollapsePattern]::Pattern).Expand()
    Start-Sleep -Milliseconds 400
    $saveAsItem = Find-ByNameAndType -Root $window -Name "Save Model As..." -ControlType ([System.Windows.Automation.ControlType]::MenuItem)
    Invoke-ElementClick -Element $saveAsItem

    # Polled for, not a fixed sleep: CONFIRMED a fixed short wait here was the likely cause of a
    # repeated, flip-flopping "should have been flagged, but wasn't" result - the dialog can
    # genuinely take longer than that to fully appear, same as every other spot in this suite that
    # waits for it polls for up to several seconds rather than assuming a fixed delay is enough.
    $dialogCondition = New-Object System.Windows.Automation.AndCondition(
        (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::NameProperty, "Save As")),
        (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::ControlTypeProperty, [System.Windows.Automation.ControlType]::Window))
    )
    $saveAsDialog = $null
    for ($i = 0; $i -lt 30; $i++) {
        $saveAsDialog = $window.FindFirst([System.Windows.Automation.TreeScope]::Descendants, $dialogCondition)
        if ($null -ne $saveAsDialog) { break }
        Start-Sleep -Milliseconds 200
    }
    if ($null -eq $saveAsDialog) {
        Write-Host "Save As dialog never appeared within ~6 seconds - can't continue this check." -ForegroundColor Red
    }

    Get-OpenWindows -ProcessId $proc.Id | Format-Table Name, ControlType, NativeWindowHandle, IsOffscreen -AutoSize

    try {
        Assert-NoNewDialogs -ProcessId $proc.Id -Baseline $baseline
        Write-Host "FAIL (unexpected - the Save As dialog should have been flagged, but wasn't)." -ForegroundColor Red
    } catch {
        Write-Host "PASS - correctly flagged: $($_.Exception.Message)" -ForegroundColor Green
    }

    # Close the dialog we opened for this test, so the process can exit cleanly afterward.
    $dialogCondition = New-Object System.Windows.Automation.AndCondition(
        (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::NameProperty, "Save As")),
        (New-Object System.Windows.Automation.PropertyCondition([System.Windows.Automation.AutomationElement]::ControlTypeProperty, [System.Windows.Automation.ControlType]::Window))
    )
    $dialog = $window.FindFirst([System.Windows.Automation.TreeScope]::Descendants, $dialogCondition)
    if ($dialog) {
        $cancelButton = Find-ByNameAndType -Root $dialog -Name "Cancel" -ControlType ([System.Windows.Automation.ControlType]::Button)
        if ($cancelButton) { Invoke-ElementClick -Element $cancelButton }
    }
} finally {
    Stop-Process -Id $proc.Id -Force -ErrorAction SilentlyContinue
    Remove-Item (Split-Path -Parent $instance.ModelPath) -Recurse -Force -ErrorAction SilentlyContinue
}
