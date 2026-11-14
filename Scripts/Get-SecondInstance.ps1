<#
.SYNOPSIS
    Returns the OTHER running instance of the same process as $Proc - i.e. not $Proc itself - for
    driving a two-simultaneous-instances test (§1.2/§3.2 in the test plan: both instances can open
    the same file with no degrade of any kind). Every other script in this set already works
    generically from a Process object (FromHandle on its own MainWindowHandle, never a shared
    global), so the SAME scripts work unchanged against $proc2 - nothing instance-specific to add.

.PARAMETER Proc
    The FIRST instance's Process object - the one to exclude when picking the other one.

.EXAMPLE
    $proc = Get-Process ScientificInterface | Select-Object -First 1
    # ... launch (or already have running) a second instance ...
    $proc2 = .\Get-SecondInstance.ps1 -Proc $proc
    .\Test-EditCellEndToEnd.ps1 -Proc $proc2   # same script, the other window
#>
param(
    [Parameter(Mandatory = $true)]
    [System.Diagnostics.Process]$Proc
)

$candidates = Get-Process -Name $Proc.ProcessName | Where-Object { $_.Id -ne $Proc.Id }

if ($candidates.Count -eq 0) {
    throw "No other running '$($Proc.ProcessName)' process found - start a second instance first."
}
if ($candidates.Count -gt 1) {
    Write-Host "WARNING: more than one other '$($Proc.ProcessName)' process found - returning the first. Verify this is the right one if more than two instances are running." -ForegroundColor Yellow
}

$candidates | Select-Object -First 1
