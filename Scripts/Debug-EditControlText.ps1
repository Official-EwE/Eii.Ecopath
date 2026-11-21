<#
.SYNOPSIS
    Diagnoses the "Waiting for the import to finish..." stall by exercising the EXACT SAME
    retrieval path the real test uses (Get-WindowElementByHandle + Get-EditControlValue), against
    an already-running import console, with full visibility into every step along the way - so we
    can see exactly where it diverges from what Accessibility Insights sees (which has already
    confirmed the Edit control DOES expose a working ValuePattern with the full console text).

.PARAMETER ProcessId
    PID of the already-running ScientificInterface.exe instance showing the console. Find it with
    Get-Process ScientificInterface (or Task Manager) while the "Importing legacy Ecopath 5
    model..." window is open and NOT yet dismissed.

.PARAMETER WindowName
    Exact title of the console window to target. Default "Importing legacy Ecopath 5 model...".

.PARAMETER Watch
    If set, after the one-shot diagnostic, polls the Edit control's Value every second for
    -WatchSeconds and prints its growing length/tail - to directly confirm (or rule out) that
    incremental updates as the import progresses are actually visible from PowerShell.

.PARAMETER WatchSeconds
    How long -Watch keeps polling. Default 60.

.EXAMPLE
    # Start the app yourself first, e.g.:
    #   ScientificInterface.exe "D:\Downloads\EwE5 Databases\Gironde estuary.MDB"
    # then, while the console is still open:
    .\Debug-EditControlText.ps1 -ProcessId 38904 -Watch
#>
param(
    [Parameter(Mandatory = $true)][int]$ProcessId,
    [string]$WindowName = "Importing legacy Ecopath 5 model...",
    [switch]$Watch,
    [int]$WatchSeconds = 60
)

. (Join-Path $PSScriptRoot "TestHelpers.ps1")

function Write-Section {
    param([string]$Title)
    Write-Host "`n=== $Title ===" -ForegroundColor Cyan
}

Write-Section "Step 1: listing this process's top-level windows (via DialogChecker.exe - the same path Wait-ForWindowByName uses)"
$allWindows = Get-OpenWindows -ProcessId $ProcessId
if ($allWindows.Count -eq 0) {
    Write-Host "No windows at all were returned for PID $ProcessId - wrong PID, or DialogChecker.exe itself failed. Stopping here." -ForegroundColor Red
    return
}
$allWindows | Format-Table Name, ControlType, NativeWindowHandle, IsOffscreen -AutoSize | Out-Host

$target = $allWindows | Where-Object { $_.Name -eq $WindowName } | Select-Object -First 1
if ($null -eq $target) {
    Write-Host "No window named exactly '$WindowName' was found among the ones listed above." -ForegroundColor Red
    Write-Host "If the real title differs even slightly (trailing space, different punctuation), that alone would explain everything downstream failing - compare it character-for-character against what's listed above." -ForegroundColor Yellow
    return
}
Write-Host "Found it - NativeWindowHandle: $($target.NativeWindowHandle) (hex 0x$($target.NativeWindowHandle.ToString("X8")))" -ForegroundColor Green

Write-Section "Step 2: resolving an AutomationElement from that handle (Get-WindowElementByHandle)"
$consoleElement = Get-WindowElementByHandle -Handle $target.NativeWindowHandle
if ($null -eq $consoleElement) {
    Write-Host "FromHandle() returned `$null for this handle - the handle itself may be stale/invalid by the time we got here." -ForegroundColor Red
    return
}
$resolvedName = try { $consoleElement.Current.Name } catch { "<threw: $($_.Exception.Message)>" }
$resolvedClass = try { $consoleElement.Current.ClassName } catch { "<threw: $($_.Exception.Message)>" }
$resolvedType = try { $consoleElement.Current.ControlType.ProgrammaticName } catch { "<threw: $($_.Exception.Message)>" }
Write-Host "Resolved element - Name: '$resolvedName'  ClassName: '$resolvedClass'  ControlType: $resolvedType"
if ($resolvedName -ne $WindowName) {
    Write-Host "WARNING: this doesn't match the expected window name - FromHandle may have resolved the wrong element." -ForegroundColor Yellow
}

Write-Section "Step 3: searching for a ControlType.Edit descendant (same condition Get-EditControlValue uses)"
$editCondition = New-Object System.Windows.Automation.PropertyCondition(
    [System.Windows.Automation.AutomationElement]::ControlTypeProperty, [System.Windows.Automation.ControlType]::Edit)
$editElement = $null
try {
    $editElement = $consoleElement.FindFirst([System.Windows.Automation.TreeScope]::Descendants, $editCondition)
} catch {
    Write-Host "FindFirst(Descendants, Edit) THREW: $($_.Exception.Message)" -ForegroundColor Red
}

if ($null -eq $editElement) {
    Write-Host "No Edit descendant found. Dumping EVERY descendant PowerShell can see, to compare against the Accessibility Insights tree:" -ForegroundColor Yellow
    try {
        $all = $consoleElement.FindAll([System.Windows.Automation.TreeScope]::Descendants, [System.Windows.Automation.Condition]::TrueCondition)
        Write-Host "$($all.Count) descendant(s) found:"
        foreach ($el in $all) {
            $n = try { $el.Current.Name } catch { "<unavailable>" }
            $ct = try { $el.Current.ControlType.ProgrammaticName } catch { "<unavailable>" }
            $cn = try { $el.Current.ClassName } catch { "<unavailable>" }
            Write-Host "  Name: '$n'  ControlType: $ct  ClassName: '$cn'"
        }
    } catch {
        Write-Host "FindAll(Descendants, TrueCondition) ALSO threw: $($_.Exception.Message)" -ForegroundColor Red
    }
    Write-Host "`nIf the Edit control genuinely isn't reachable this way from PowerShell (even though Accessibility Insights sees it fine), the most likely explanations are: a UAC/integrity-level mismatch between this PowerShell session and the target process (e.g. one is elevated, the other isn't - UI Automation generally can't cross that boundary), or this PowerShell process being 32-bit/64-bit in a way that doesn't match. Run 'powershell -Command `"[Environment]::Is64BitProcess`"' and compare against the target app's own bitness to rule that out." -ForegroundColor Yellow
    return
}

$editName = try { $editElement.Current.Name } catch { "<unavailable>" }
$editClass = try { $editElement.Current.ClassName } catch { "<unavailable>" }
$editAutomationId = try { $editElement.Current.AutomationId } catch { "<unavailable>" }
$editRect = try { $editElement.Current.BoundingRectangle } catch { "<unavailable>" }
Write-Host "Found an Edit element - Name: '$editName'  ClassName: '$editClass'  AutomationId: '$editAutomationId'  BoundingRectangle: $editRect" -ForegroundColor Green

Write-Section "Step 4: checking which patterns this element actually supports from PowerShell's side"
try {
    $supported = $editElement.GetSupportedPatterns()
    foreach ($p in $supported) {
        Write-Host "  Supported: $($p.ProgrammaticName)"
    }
    if (-not ($supported | Where-Object { $_.ProgrammaticName -like "*Value*" })) {
        Write-Host "ValuePattern is NOT in the supported-patterns list from PowerShell - even though Accessibility Insights showed it. This would point at a genuine environment difference (process elevation/bitness/session) rather than a code bug." -ForegroundColor Red
    }
} catch {
    Write-Host "GetSupportedPatterns() threw: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Section "Step 5: reading the value via ValuePattern (what Get-EditControlValue actually does)"
try {
    $valuePatternObj = $editElement.GetCurrentPattern([System.Windows.Automation.ValuePattern]::Pattern)
    $value = $valuePatternObj.Current.Value
    Write-Host "SUCCESS - Value length: $($value.Length)" -ForegroundColor Green
    Write-Host "--- last 300 characters ---"
    Write-Host $value.Substring([Math]::Max(0, $value.Length - 300))
    Write-Host "--- end ---"
} catch {
    Write-Host "GetCurrentPattern(ValuePattern.Pattern) THREW: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "(This is the exact call Get-EditControlValue makes with no try/catch around it - if it throws here, it would also throw there, which would surface as the WHOLE TEST failing with this exception message, not a silent empty string. If you instead see the test just time out after 120s with an empty string, this call is NOT the one failing - Get-EditControlValue must be returning before even reaching it, i.e. FindFirst is returning `$null in that context even though it worked here.)" -ForegroundColor Yellow
}

if ($Watch) {
    Write-Section "Step 6: watching the Value grow for up to $WatchSeconds second(s) (Ctrl+C to stop early)"
    $deadline = (Get-Date).AddSeconds($WatchSeconds)
    while ((Get-Date) -lt $deadline) {
        $text = Get-EditControlValue -WindowElement $consoleElement
        $tail = if ($text.Length -gt 0) { $text.Substring([Math]::Max(0, $text.Length - 60)).Replace("`r", "").Replace("`n", " / ") } else { "<empty>" }
        Write-Host ("[{0}] len={1,-6} tail: {2}" -f (Get-Date -Format "HH:mm:ss"), $text.Length, $tail)
        if ($text -like "*Process exited*") {
            Write-Host "Saw '--- Process exited' - this is what Wait-ForEditControlText is waiting for. Stopping watch." -ForegroundColor Green
            break
        }
        Start-Sleep -Milliseconds 1000
    }
}
