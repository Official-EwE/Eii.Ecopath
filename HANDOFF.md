# EwE Local Staging Copy — Master Branch Backport — Hand-off

## LATEST: script split + folder-driven sweep (UNVERIFIED - written without being able to run PowerShell)

Script layout now:

| Script | Role |
|---|---|
| `Scripts/ConversionTestHelpers.ps1` | Shared, dot-sourced: `Open-LegacyModelDirectly`, `Invoke-ReferralDialogTest`, `Invoke-WizardMenuImportTest -ModelNumber`, `Invoke-HelperMenuImportTest -ModelNumber [-ModelCount]`, `Test-HelperImportSucceeds`, `Test-WizardImportSucceeds`, etc. The caller must define `$Platform`, `$MainWindowTimeoutSeconds`, `$startInstanceScript`. |
| `Scripts/Run-ConversionHandlingTests.ps1` | (renamed from the old `Run-ConversionTests.ps1`) Fixed, named cases for the known sample models. Every `Run-ConversionTests.ps1` mention in the older sections below means THIS script. |
| `Scripts/Run-ConversionTests.ps1` | NEW generic sweep: `-ModelsFolder` (prompted if omitted, remembered in gitignored `.conversion-models-folder.txt`), `-Recurse`, `-Platform`, `-Only`, `-PlanOnly`. |

How the sweep decides, per `.mdb`, using the x86 `EwEImportHelper.exe`:
1. Jet3 = header byte at 0x14 is 0 (same as `IsLegacyJetFormat`).
2. `helper version <file>`: < 1.6 TooOld (referral dialog test); 1.6-1.73 Importable; else skipped as not legacy.
3. Importable: `helper models <file>` (NEW non-interactive mode in `Program.vb`: first line = count, then `N<TAB>name`) gives the model count/names.
4. Route: x64 + Jet3 -> helper console menu test per model; otherwise -> import wizard test per model.
`-PlanOnly` prints the classification table without running any test - run that first.

Needs the `models` mode in `Sources/EwEImportHelper/Program.vb` (delivered in the VB zip; based on the earlier copy of that file).

---

## Kenya, Lake Nakuru - 1972.mdb (TooOld-Jet3x64-confirmed) is now automated
## too, from real Accessibility Insights screenshots

Characterized directly from the user's own screenshots this time, not
guessed: a standard MessageBox-style dialog, titled "Ecopath with Ecosim" -
genuinely `ControlType.Text` for the message and a genuine `ControlType.Button`
named "OK" (unlike the import console, no `ClassName` workaround needed here
- this one exposes cleanly to PowerShell's own UIA client).

**Important wrinkle, confirmed by this very dialog existing:** "Ecopath with
Ecosim" really is reused as this app's generic dialog title across several
different dialogs - exactly what the earlier `Wait-ForMainWindow` false-match
bug implied before we had direct proof. So the new test doesn't trust the
title alone: after `Wait-ForWindowByName` finds a window named "Ecopath with
Ecosim", `Test-ReferralDialogAppears` also reads its message text (via
`ControlType.Text`) and checks it against known marker phrases ("outdated
version of Ecopath 5", "not supported by the EwE6 import process") before
treating it as confirmed and clicking OK - the same caution principle as the
earlier bug, applied up front this time instead of discovered the hard way.

No conversion is attempted at all for this tag family, so
`Open-LegacyModelDirectly` is called with no `-OutputFileNames` (nothing to
clean up beforehand).

New shared helper: `Test-ReferralDialogAppears` in `Run-ConversionTests.ps1`
- reusable for EcopathMSY.MDB (`TooOld-DirectOpen`) next, which per the user's
earlier description shows the same dialog shape, just reached a different way
(`DirectOpen` instead of the x64-helper detection path) - likely needs no new
code at all, just a new `Invoke-ConversionTestEntry` call once we have a
screenshot confirming the same message text applies there too.

**Next step, concretely:** run
`.\Scripts\Run-ConversionTests.ps1 -Only "Kenya"` and confirm it PASSes -
dialog detected, text verified, OK clicked, no leftover unexpected windows.

## STATUS: gulfmexico now also CONFIRMED correct — fails for the real,
## expected reason (`SalinityForcingShapeID`), not the DialogChecker.exe
## crash from the Fourth bug below. Both conversion tests are now fully
## working end to end.

Per the user's own real run, output now reads (abridged):
```
Import failed with an unhandled exception: ArgumentException: Column
'SalinityForcingShapeID' does not belong to table Table.
Import of model Florida Bays 46026 failed at 19/11/2026 16:26:44
Import failed. See log file: ...\GulfTotal_import_log.txt
--- Process exited with code 1 ---
FAIL (24063 ms): Expected the import to succeed, got: '...'
```
This is the known, pre-existing app bug the test is SUPPOSED to catch - a
correct FAIL, not a test-harness problem. Two follow-up quality-of-life
fixes applied based on the user's own feedback on this output:

- **Failure message truncated to the last 5 lines** of the console log,
  not the whole thing - the full text can run to tens of thousands of
  characters for a large model (confirmed: 27000+ for gulfmexico), and the
  real failure/success marker always lives in the last few lines regardless
  - the rest is noise once something's already failed. Applied in
  `Test-HelperImportSucceeds` (`Run-ConversionTests.ps1`).
- **ScriptStackTrace no longer printed for a plain `Assert-True` failure**
  (the single most common kind of test failure there is, this one
  included) - its top frame is always `at Assert-True, ...` by
  construction, which never adds anything beyond what the assertion
  message itself already says. It's suppressed by checking whether the
  stack trace's top frame is `Assert-True` specifically; it's still printed
  in full for any OTHER kind of exception (a real engine/COM error, like
  the call-depth-overflow bug it was originally added to catch), where it
  remains genuinely useful. Applied in `Invoke-Test`'s catch block
  (`TestHelpers.ps1`).

## Fourth bug, found on the gulfmexico run right after Gironde passed:
## DialogChecker.exe's whole window-listing call dies if ANY one window
## goes stale mid-enumeration — read this FIRST

Gironde passed cleanly (see the STATUS section just below). The very next
test, gulfmexico (expected to FAIL on the known `SalinityForcingShapeID`
import bug - that part's fine), instead died with:
```
FAIL (10708 ms): DialogChecker.exe failed (exit code 2): DialogChecker.exe :
Error: The target element corresponds to UI that is no longer available
(for example, the parent window has closed).
...
at MS.Internal.AutomationProxies.ProxySimple.get_BoundingRectangle()
```
right after "Waiting for the response window... done" - i.e. a real test
infrastructure failure, not the expected import failure, and the user noted
the import dialog was STILL open on screen at the time.

**Root cause:** `Program.cs`'s main loop reads `window.Current.BoundingRectangle`
completely unprotected, unlike every other per-window property
(`Name`/`ControlType`/`NativeWindowHandle`/`IsOffscreen`, all already behind
`SafeGetXXX` wrappers). gulfmexico's import fails fast, and the console
window can apparently close or get replaced out from under this tool's own
enumeration while it's still iterating the window list it captured a moment
earlier - a real, confirmed race, not a hypothetical one. Once ANY window in
that batch goes stale by the time its properties are read, the raw
`BoundingRectangle` read throws, and because it sat OUTSIDE any per-window
try/catch, it killed the entire call - exit code 2, nothing printed for ANY
window, including perfectly healthy ones.

**Fix:** added a `SafeGetRect` helper (same pattern as the existing
`SafeGetLong`/`SafeGetBool`/`SafeGetString`) and wrapped the ENTIRE
per-window body in its own try/catch, not just the one call that happened to
be the one that threw this time - so a future stale-window hit on some
OTHER property fails the same safe way: that one window is skipped (with a
one-line note on stderr), and every other window in the same call still gets
reported normally, instead of the whole enumeration dying.

**Next step, concretely:** re-run the gulfmexico test (or the full suite)
and confirm it now gets PAST this infrastructure error and fails (if it
still fails) for the real, expected reason instead - an "Import failed" /
`SalinityForcingShapeID`-related message, or whatever `Assert-NoNewDialogs`
actually reports for the leftover error dialog.

## STATUS: Gironde is CONFIRMED PASSING end-to-end as of the user's own
## real run (gulfmexico's own infra bug above is separate, found right
## after)

```
Ready (unconfirmed) - PID 22516, model folder: ...
[PID 22516] Started (conversion test instance)
  Waiting for the response window... done
  Waiting for the import to finish... done
  Dismissing the import console (Continue)... done
[PID 22516] Stopping (Gironde estuary test instance)...
[PID 22516] Stopped (Gironde estuary test instance).
PASS (12557 ms)
```

That's the full chain working: launch → `-SkipMainWindowWait` sidesteps the
old `Wait-ForMainWindow` hang/false-click entirely → `Wait-ForWindowByName`
finds the console → `Get-EditControlValue`'s hardened fallback (require
non-empty before trusting either path) correctly reads the growing log text
→ marker match → Continue clicked → clean shutdown → PASS. One real bug
fixed per layer, confirmed by the user's own hands rather than guessed from
code reading alone - see the three sections below (still kept, in order, for
anyone tracing back through what each one actually was).

**Not yet run/confirmed:** the gulfmexico test (expected to FAIL, on the
known `SalinityForcingShapeID` import bug - that's the correct/expected
outcome, not a new problem), and the second instance no longer failing to
start afterward (should be fine now that the first test's cleanup actually
runs, but not independently confirmed).

**Next step, concretely:** run the full suite (`.\Scripts\Run-ConversionTests.ps1`,
no `-Only` filter) and confirm gulfmexico FAILs for the expected reason
(a message mentioning `SalinityForcingShapeID`, or whatever the actual
"Import failed" / unexpected-dialog text turns out to be) rather than
hanging or erroring some other way.

## Third bug, found via Debug-EditControlText.ps1: the console's text was
## never reachable as a ControlType.Edit + ValuePattern at all

With the recursion and `Wait-ForMainWindow` bugs both fixed, the Gironde
test got as far as "Waiting for the import to finish..." and stalled there.
Per the user's ask, I added `Scripts/Debug-EditControlText.ps1` - a
standalone script that walks the exact same retrieval path
(`Get-WindowElementByHandle` → `FindFirst(Descendants, ControlType.Edit)` →
`ValuePattern.Current.Value`) step by step, with every step's result printed
instead of swallowed.

**What it found, against the real running console:** no `ControlType.Edit`
descendant exists at all from PowerShell's own UI Automation client. Dumping
every descendant instead showed the console's whole text sitting as the
`Name` property of a plain `ControlType.Pane` element, whose underlying
Win32 window class name is `WindowsForms10.EDIT.app.0...` - i.e. it IS the
real Win32 EDIT control under the hood, but PowerShell's
`System.Windows.Automation` client doesn't expose it as `ControlType.Edit`
with a working `ValuePattern` the way Accessibility Insights' own (different)
UI Automation client stack does. Confirmed directly from the user's own
`-Watch` output: the full log text, including the final
"--- Process exited with code 0 ---" line, was sitting right there in that
Pane's `Name`, the entire time - just never where the old code was looking.

**Fix, in two places (both had the identical assumption):**
- `Get-EditControlValue` in `TestHelpers.ps1` now tries the original
  `ControlType.Edit` + `ValuePattern` route first (kept as the "proper" path
  in case it's ever genuinely available), and falls back to searching ALL
  descendants for one whose `ClassName` contains `"EDIT"` (case-insensitive),
  returning its `Name` instead. Matched by `ClassName`, not by searching for
  known log text - the log text IS the `Name` here, so matching on content
  would be circular.
- `SafeGetEditValue` in `Tools/DialogChecker/Program.cs` (the C# tool used
  for `Get-OpenWindows`'s own `TextContent` column, shown in
  `Assert-NoNewDialogs` failure messages) had the exact same assumption and
  got the identical fallback, so unexpected-dialog error messages for this
  kind of control show real content too, not an empty string.
`Get-DialogCheckerExe` rebuilds automatically when `Program.cs` is newer than
the compiled `.exe` (already-existing logic), so no manual rebuild step is
needed before the next run.

**Next step, concretely:** re-run `.\Scripts\Run-ConversionTests.ps1`. The
Gironde test should now get past "Waiting for the import to finish...",
correctly read "Import completed successfully." + "--- Process exited...",
click Continue, and pass (assuming `Assert-NoNewDialogs` finds nothing else
unexpected afterward).

## Second bug, found right after the recursion fix: Wait-ForMainWindow hangs
## on the legacy-conversion launch — read this FIRST (above the recursion
## section and MAJOR UPDATE #3)

With the recursion bug fixed, the user re-ran it and got a new, different
failure. Console output (abridged):
```
Database-upgrade prompt detected - clicking Yes.................. done.
Ready - PID 38904, model folder: ...
```
...then the app visibly sat on the "Importing legacy Ecopath 5 model..."
console, not progressing, until manually clicking Continue - or, left alone,
eventually:
```
FAIL (94644 ms): Timed out waiting ~90 seconds for the main window to appear.
--- ScriptStackTrace ---
at Wait-ForMainWindow, TestHelpers.ps1: line 408
at <ScriptBlock>, Start-TestInstance.ps1: line 116
at Open-LegacyModelDirectly, Run-ConversionTests.ps1: line 174
...
```
The user asked five sharp questions; answering each directly:

**1. Why the invalid "Database-upgrade prompt" detection?** `Start-TestInstance.ps1`'s
`Wait-ForMainWindow` searches the whole desktop for a window owned by this
process, named EXACTLY `"Ecopath with Ecosim"`, `ControlType.Window` - written
to catch one specific "your database will be updated" confirmation. But that
exact title/shape is already known (per that function's own comment) to be
reused by at least one OTHER confirmation in this app (the lock-contention
dialog). For the legacy-conversion launch there's evidently yet ANOTHER
window sharing that same generic title - almost certainly the main
ScientificInterface shell window itself, which plausibly carries that generic
app name as its title before it's replaced with something model-specific once
loading finishes. The check isn't specific enough to tell these apart.

**2. Why does it think it clicked Yes?** Once something matches as
"$upgradeDialog", the code searches its ENTIRE descendant tree
(`TreeScope.Descendants`) for anything named exactly "Yes" and clicks it. If
what matched is actually the (much larger) main shell window rather than a
small real popup, that search can find and click ANY unrelated element
named "Yes" buried anywhere in that whole tree - not necessarily a real
button meant to be clicked at all.

**3. Does the event-driven system ever get a chance to detect the import
console?** No - confirmed by the stack trace itself. `Wait-ForWindowByName`
(which looks for "Importing legacy Ecopath 5 model..." specifically) is only
ever reached AFTER `Start-TestInstance.ps1` returns, and that in turn only
returns after `Wait-ForMainWindow` finishes. Getting stuck inside
`Wait-ForMainWindow` means `Wait-ForWindowByName` is never even called.

**4./5.** Not reached yet in this run, but already handled correctly once
reached: `Test-HelperImportSucceeds` already clicks "Continue" after seeing
"--- Process exited" in the console log, and `Assert-NoNewDialogs` is indeed
a single immediate check with no settle delay of its own - see the fix below
for that one.

**Root cause:** `Wait-ForMainWindow` (and its whole "wait for
`$proc.MainWindowHandle` to become non-zero" approach) is simply the wrong
tool for the legacy-conversion launch. The user's own earlier manual test
already established the real behavior for this launch: the app goes directly
to the import console, with no separate "main window" step first. Forcing
this launch through the same main-window wait used for ordinary model opens
was the mismatch - not a timing fluke, an architectural one.

**Fix:**
- Added an opt-in `-SkipMainWindowWait` switch to `Start-TestInstance.ps1`:
  when passed, it starts the process and returns immediately (no
  `Wait-ForMainWindow` call, no staged-copy-detection wait - neither is used
  or needed by the conversion tests), leaving all waiting to the caller.
  Default behavior (no switch) is completely unchanged, so every other
  `Run-*Tests.ps1` script that depends on the normal main-window wait is
  unaffected.
- `Open-LegacyModelDirectly` in `Run-ConversionTests.ps1` now passes
  `-SkipMainWindowWait`, going straight from launch to its own
  `Wait-ForWindowByName` for the import console - exactly matching the
  user's own manually-observed flow, and sidestepping the false "Ecopath with
  Ecosim"/"Yes" detection entirely (that code path is never invoked for this
  launch anymore).
- Added a 1-second `Start-Sleep` in `Test-HelperImportSucceeds`, right after
  clicking Continue and before returning control to the caller's
  `Assert-NoNewDialogs` check - a small settle margin for whatever UI
  transition Continue kicks off, addressing question 5 directly.

**What this does NOT resolve:** the identity of that "Ecopath with Ecosim"
generic-titled window/button is still not confirmed - I'm fairly confident
it's the main shell window's own generic startup title, not a real
unidentified dialog, but that's an inference from the code and symptoms, not
something either of us has actually seen on screen. Since the conversion
tests no longer go through `Wait-ForMainWindow` at all, this no longer
blocks them either way - but if a FUTURE test (one of the 5 still-manual
ones, or any other new launch) needs that wait, this ambiguity is worth
pinning down for real.

**Added for exactly that, per the user's own follow-up ask:**
`Wait-ForMainWindow` now prints a one-time diagnostic block the first time it
matches the "upgrade dialog" condition - `Name`, `AutomationId`, `ClassName`,
`NativeWindowHandle` (decimal AND hex), `ProcessId`, and `BoundingRectangle`.
The most directly useful one for cross-referencing against Accessibility
Insights is `NativeWindowHandle` - Insights shows the same value (often
labeled "HWND") in an element's advanced/native properties; the hex form
printed here is formatted the same way Insights displays it, so it's a
straight string match, no conversion needed. `BoundingRectangle` is the
other quick tell: the real small upgrade popup and the full-size main shell
window will have very different sizes, visible at a glance even without
opening Insights at all. Each property read is wrapped individually (not one
big try/catch around all of them) so one throwing doesn't hide the others
that would have printed fine.

Printed once per detection (not spammed every ~200ms poll iteration) right
before the usual "Database-upgrade prompt detected - clicking Yes..." line,
so it's easy to spot in the console output. This only fires on a path that
no longer runs for the conversion tests (per the fix above), so it won't
show up there - it's for whenever `Wait-ForMainWindow` is used again (the
other `Run-*Tests.ps1` scripts that already depend on it, or a future
conversion test that ends up needing a real main-window wait after all).

**Next step, concretely:** re-run `.\Scripts\Run-ConversionTests.ps1` and
confirm the Gironde test now runs start to finish without the console
hanging, press-Continue needed, or opening Task Manager - it should now
behave exactly like the user's own original manual walkthrough.

## "call depth overflow" on the real Gironde run — ROOT CAUSE FOUND AND
## FIXED — read this FIRST, above even MAJOR UPDATE #3

The user ran `Run-ConversionTests.ps1` for real (first real execution against
the actual app). Prerequisite setup worked fine (prompted once, saved
config). Then:

```
=== Conversion: Gironde estuary.MDB imports successfully (Import-x64Helper) ===
FAIL (37652 ms): The script failed due to call depth overflow.

=== Conversion: gulfmexico.MDB imports successfully (Import-x64Helper) ===
```
...and the user had to Ctrl+C out of the second test, since the app failed to
even open/start.

"Call depth overflow" is `System.Management.Automation.ScriptCallDepthException`
— a REAL PowerShell interpreter limit on nested script/function calls (~1000
deep by default). It is not a simple assertion failure, and it is not
something a brace/paren-balance check (the only verification available in
this sandbox - no `pwsh` here) would ever catch, since it's about runtime call
depth, not syntax.

**What I did to try to find it (all static reading, no execution possible
here):** read every function body in `Scripts/TestHelpers.ps1` (963 lines,
all of it), `Scripts/Run-ConversionTests.ps1`, and `Scripts/Start-TestInstance.ps1`
end to end, specifically looking for a function that calls itself directly, or
two functions that call each other, which is the only way this specific
exception occurs. Also ran a regex self-reference scan over every `function`
block in both files. **Found no literal self-recursion or mutual recursion
anywhere in the delivered code.** The call chain for the Gironde test
(`Invoke-ConversionTestEntry` → `Invoke-Test` → test scriptblock →
`Open-LegacyModelDirectly` → `Write-Step` → `Wait-ForWindowByName` →
`Get-OpenWindows` → `Get-DialogCheckerExe`) is only ~8 frames deep even fully
unrolled, nowhere near 1000 - so whatever is actually recursing hundreds of
times isn't in that obvious chain, or isn't recursing the way I'd expect from
reading the text alone. Also checked (and ruled out) the one other plausible
real bug I could see: the import console's `Edit` control text is already
run through `Escape()` in `Tools/DialogChecker/Program.cs` before being
written tab-separated to stdout, so an embedded newline/tab in the console's
multi-line scrollback log can't be corrupting `Get-OpenWindows`' line-based
parsing - that was my leading alternate theory and it's a dead end.

**Root cause (confirmed via the user's own second run, which showed the
actual stack trace - the identical frame `at <ScriptBlock>,
Run-ConversionTests.ps1: line 80` repeated over and over):**

`Run-ConversionTests.ps1` line 80 is:
```powershell
Invoke-Test $Name { & $Test $modelPath }
```
`Invoke-Test` (in `TestHelpers.ps1`) had its OWN parameter also named
`$Test` (`[scriptblock]$Test`). A bare `{ ... }` scriptblock literal in
PowerShell is NOT a lexical closure by default - it resolves variable names
dynamically, against whatever scope happens to be invoking it, not the scope
where it was written. So once this scriptblock was bound to `Invoke-Test`'s
own `$Test` parameter and then run via `& $Test` inside `Invoke-Test`, the
`$Test` *referenced inside the scriptblock itself* resolved to - itself.
Every invocation called itself again, forever: a single scriptblock calling
itself through an accidental name collision between an outer closure variable
and an inner function's same-named parameter, not a chain of different
functions calling each other. That's exactly why the trace was one repeated
line rather than a growing sequence of different ones.

**Fix:** renamed `Invoke-Test`'s parameter from `$Test` to `$TestBody`
(and the one internal `& $Test` to `& $TestBody`) in `TestHelpers.ps1`. No
other caller anywhere in the toolkit passes that parameter by name (checked
every `Invoke-Test` call site in `Run-StagingTests.ps1`,
`Run-CrashRecoveryTests.ps1`, `Run-MultiInstanceTests.ps1`, and
`Run-NoStagingCallerTests.ps1` - every single one passes the scriptblock
positionally), so this is a safe, isolated rename with no other call sites to
update.

The diagnostic `ScriptStackTrace` printing added to `Invoke-Test`'s catch
block (see below) is what actually surfaced this - kept in place since it's
generically useful for any future failure, not just this one.

Still worth a sanity check on the next run: the second (gulfmexico) test
failing to even start and needing a Ctrl+C was very likely just a side
effect of the first test's infinite recursion never reaching its own
`finally { Stop-TestInstance }` block - so a zombie `ScientificInterface.exe`
was probably still holding the lock/foreground. With the real bug fixed, this
should resolve on its own; if the app still fails to start on a fresh run,
check Task Manager for a leftover `ScientificInterface.exe` first.

**What I changed (this delivery):** `Invoke-Test`'s `catch` block in
`TestHelpers.ps1` now also prints `$_.ScriptStackTrace` (wrapped in its own
try/catch so it can never itself throw a second, more confusing error) right
after the normal "FAIL (...)" line, under a `--- ScriptStackTrace ---`
banner - this is what actually pinpointed the bug above, and is worth keeping
for any future failure. Plus the `$Test` → `$TestBody` rename fixing the
actual recursion.

**Next step, concretely:** re-run `.\Scripts\Run-ConversionTests.ps1` from a
fresh PowerShell window (to clear out any lingering state from the crashed
run) and confirm the Gironde and gulfmexico tests now actually execute
end-to-end (Gironde should PASS; gulfmexico is expected to FAIL on the known
`SalinityForcingShapeID` bug, which is fine/expected).

## MAJOR UPDATE #3 (same ongoing chat, written well after "MAJOR UPDATE #2"
## below) — read this first

Covers the newest chapter: automated tests for the model-kind-detection
feature from MAJOR UPDATE #2 below, built against real sample legacy
model files the user found and tested by hand.

**Sample models found so far** (see MAJOR UPDATE #2's own table below for
the full list/notes) - six models, all found on x64 so far: Gironde
estuary.MDB (`Import-x64Helper`, succeeds), Kenya, Lake Nakuru -
1972.mdb (`TooOld-Jet3x64-confirmed`), gulfmexico.MDB
(`Import-x64Helper`, **fails** on a `SalinityForcingShapeID` column
error), Ecopath.MDB (`Import-WizardNonJet3`), EcopathMSY.MDB
(`TooOld-DirectOpen`), GBR_Linked_ecosystem.MDB (`Import-x64Helper`,
shows a multi-submodel picker first, then **fails** with an `EatEffBad`
error - also expected to hit `Import-Wizard32bitJet3` on x86, same
file, not yet tried).

**Design decisions from this session, all per explicit user direction:**
- Scope, for now: only the tags that mean "this model needs conversion
  handling" - `TooOld-*`, `Import-WizardNonJet3`, `Import-Wizard32bitJet3`,
  `Import-x64Helper`. Explicitly OUT of scope: `Current-EwE6`,
  `Future-NewerVersion`, `Invalid-Unknown`, and the `EII`/`EIIXML`/`EcoBase`
  cases.
- `Jet3x64-unconfirmed` gets NO test of its own (it's not a reachable
  outcome a sample file can trigger on demand - the helper would need to
  genuinely fail/go missing) and, after reconsidering, NO post-test guard
  either (an earlier idea - assert the status bar never shows this tag
  after a conversion test - was explicitly dropped).
- **Tests do NOT read the `[ModelKind]` status-bar tag at all.** That
  diagnostic (MAJOR UPDATE #2, DEBUG-only) was for the user's OWN manual
  exploration, finding which sample file hits which branch - not for the
  automated tests. Tests instead confirm the actual OBSERVABLE behavior:
  which dialog appears (EwE5 referral vs. import wizard vs. the x64
  helper's console) and, for the helper, whether the import's own console
  output reports success or a specific failure. Anything else - an
  unexpected extra dialog - already fails the test via the existing
  baseline/`Assert-NoNewDialogs` mechanism, same as every other test in
  this suite.
- Sample model files are real third-party data, not committed to the
  repo. A new prerequisite-check script handles this (see below).

**New: `Scripts/Test-ConversionPrerequisites.ps1`** - dot-sourced,
defines `Confirm-ConversionTestPrerequisites -RequiredFileNames <list>`.
Always run at the start of `Run-ConversionTests.ps1` (even under
`-Only`), cheap enough not to matter. State lives in a small, **gitignored**
config file next to the scripts: `Scripts\.conversion-test-config.txt`
(two lines: `TestModelsFolder=<path>` and
`SkipModels=<comma-separated filenames>`) - **add this file to
.gitignore**, not yet done since this chat has no direct repo access.
- No config yet, or a configured folder that's gone missing (drive
  unplugged, renamed): prompts interactively for the folder. Empty answer
  = skip ALL conversion tests on this machine, recorded so it's never
  asked again. A real folder gets checked (recursively) against every
  known required file name; anything missing is listed, and the user can
  retype the folder to retry, or press Enter to accept skipping just
  those specific files.
- Every later run re-verifies file presence fresh (self-healing: a file
  that later shows up un-skips itself, one that vanishes gets skipped) -
  without ever re-prompting interactively just because a file is still
  missing. Only prompts again if the folder itself stops existing.
- Returns `.Bypassed`, `.TestModelsFolder`, `.SkippedModels`, and
  `.ModelPaths` (a filename → full resolved path map for everything that
  WAS found), so the test runner never has to re-search.

**New: `Scripts/Run-ConversionTests.ps1`** - the test runner. Each test
gets its OWN disposable instance (same reasoning as
`Run-CrashRecoveryTests.ps1` - these tests change app/model state in big,
not-easily-undone ways), opens the legacy file via a new
`Open-LegacyModelViaUI` helper (same navigation as the existing
`Open-ModelViaUI`, but without ITS built-in wait for the unrelated
generic "database will be updated" upgrade dialog - the conversion flow
can show three entirely different windows instead, and that wait would
just cost 5 seconds for nothing here), then waits for whatever new
window appears first via a new generic `Wait-ForNewWindow` helper
(baseline-diff polling, same `Get-OpenWindows`/`Get-WindowBaseline`
machinery already in the suite - deliberately doesn't assume WHICH of
the three outcomes is coming).

**Fully automated (well characterized from the user's own screenshots -
confirmed window title, confirmed Edit-control log text, confirmed
"Continue" button):** both use the same `Test-HelperImportSucceeds` -
ALWAYS expects success, no "expected failure" variant. No special-casing
of any specific error text, and no explicit handling of the follow-up
"Import failed. See log file: ..." dialog either - if the import fails,
that dialog is simply left as an unexpected leftover window, which
`Assert-NoNewDialogs` already catches as a failure on its own, same
mechanism as every other test in this suite.
- Gironde estuary.MDB → passes (imports successfully).
- gulfmexico.MDB → **currently fails**, correctly: a real, reproducible
  `SalinityForcingShapeID` import bug, not a test-harness issue. This
  test will start passing again once that bug is fixed, same as any
  other regression test for a known bug.

**Launch mechanism for these two, per user correction**: the legacy file
is passed DIRECTLY as the command-line argument to the app
(`Start-TestInstance.ps1 -SourceModelPath <file>`), confirmed by the
user's own manual testing to open straight into the import console with
no separate "open" step needed - not started with a safe default model
and opened afterward via File > Open. New helper
`Open-LegacyModelDirectly` (`Run-ConversionTests.ps1`) does this; it
can't take an idle baseline before launch (no safe idle moment exists -
the console can already be open by the time `Start-TestInstance.ps1`
itself returns, since the import starts as part of normal startup), so
instead it uses a new `Wait-ForWindowByName` helper (`TestHelpers.ps1`,
a plain name search, no "is this new" baseline comparison, sidestepping
that race) to confirm the console is present, THEN captures the baseline
for the later `Assert-NoNewDialogs` check - deliberately including the
console itself in that baseline (closing it later isn't "new" either
way; anything appearing AFTER this point, like gulfmexico's follow-up
error dialog, is still correctly caught).

**Critical fix, per user: leftover output from a PRIOR conversion of the
SAME source file must be deleted before each test runs.** The import flow
checks for its own previous output and SKIPS the conversion entirely if
it's already there - so a stale file from an earlier run (or a
crashed/interrupted one) could make the response window never appear,
or worse, silently make a test "pass" without actually exercising the
conversion at all. New `Clear-ConversionOutputFiles` (called at the very
start of `Open-LegacyModelDirectly`, before the process even launches)
deletes a list of named output files in the SAME folder as the source
model, plus always clears `*.laccdb`/`*.ldb` (Access lock files a
crashed prior attempt can leave behind) regardless of model. The output
filenames do NOT follow any predictable pattern from the source .MDB's
own filename (confirmed by the user - they're the model's own internal
name) - each test names its own expected outputs explicitly:
- Gironde estuary.MDB → `Gironde estuarine model_import_log.txt`,
  `Gironde estuarine model.eweaccdb`, `Gironde estuarine model.vc.sqlite`
- gulfmexico.MDB → `GulfTotal.eweaccdb`, `GulfTotal_import_log.txt`

**Still open**: the same stale-output problem presumably applies to the
still-manual tests too (Ecopath.MDB's wizard import, GBR's helper/wizard
import) once those get automated - their own output filenames aren't
known yet, so no cleanup list exists for them yet either. Worth asking
the user for those filenames at the same time as the screenshots already
requested for those dialogs.

The older `Open-LegacyModelAndWait` (File > Open UI navigation +
`Wait-ForNewWindow` baseline-diff) is kept, unused by these two tests -
still needed for whenever the TooOld tests get automated, since those
specifically need to demonstrate "a DIFFERENT model was already open and
stays open" - something a command-line-launched model can't show, only
one file can be passed at startup.

**`Tools/DialogChecker/Program.cs` gained one capability** for this:
`TextContent` now ALSO falls back to reading an `Edit` control
descendant's live `Value` (via `ValuePattern`), not just a `Text`
element's `Name` - needed because the import console keeps its whole
scrollback log in an `Edit` control, confirmed via the user's own
Accessibility Insights screenshot (`ControlType.Edit(50004)`), which the
existing `ControlType.Text`-only search finds nothing in at all. New
helpers in `TestHelpers.ps1`: `Get-WindowElementByHandle` (generalizes
the existing `Get-AppWindow`'s `FromHandle` technique to any window, not
just the main one), `Get-EditControlValue`, `Wait-ForEditControlText`
(polls until the log contains a success/failure marker string),
`Invoke-ButtonByName`.

**NOT yet automated - left as `Invoke-ManualTest` checklists, on
purpose:** the EwE5-referral dialog (Kenya Lake Nakuru, EcopathMSY), the
import wizard dialog (Ecopath.MDB), and GBR_Linked_ecosystem.MDB's
multi-submodel picker (both its x64 and x86 variants). Zero confirmed
Accessibility-Insights data exists yet for any of these three UI
surfaces - guessing their real button names/structure blind is exactly
the kind of mistake that cost real time on `DialogChecker` earlier in
this project, so they're intentionally left for a human to run by hand
for now. **Next step: get an Accessibility Insights screenshot of each
(ideally right as the dialog/wizard/picker is showing), then convert each
manual checklist into a real automated test the same way the two above
were built.**

---

## MAJOR UPDATE #2 (same ongoing chat, written well after "MAJOR UPDATE"
## below) — read this first

Everything below this point, including the original "MAJOR UPDATE",
describes work on the **local-staging-copy backport**. This update
covers a **separate, later chapter in the same chat**: a full PowerShell
UI-Automation test suite was built and iterated on extensively, and a
new, small feature was added to help build out further test coverage
for an entirely different area (legacy/old-model detection on open).
**Sandbox state did not survive between the two chapters** - this
section itself was reconstructed from `/mnt/user-data/outputs/` after a
sandbox reset, which is exactly why this document matters: if picking
this up in a fresh chat, start from the files already in
`/mnt/user-data/outputs/`, not from assuming a prior sandbox exists.

### The UI-Automation test suite

Lives in `Scripts\` and `Tools\DialogChecker\` (delivered as
`LocalStagingTestAutomation.zip`), driving a real, running
`ScientificInterface.exe` via `System.Windows.Automation` from
PowerShell - not mocks. Four runners
(`Run-StagingTests.ps1`/`Run-CrashRecoveryTests.ps1`/
`Run-MultiInstanceTests.ps1`/`Run-NoStagingCallerTests.ps1`), tied
together by `Run-AllTests.ps1` (`-Platforms x64|x86`, combined summary).
**As of the last full run, all tests pass on both platforms** (aside
from one known-flaky, unrelated orphan-sweep timing issue on x86's
`4.3`, not yet root-caused - worth a few isolated re-runs if picked up
again).

**Real bugs this suite found and fixed, not just test-harness issues:**
- `6.2` (Save As onto another instance's own open file): never handled
  the standard Windows overwrite-confirmation prompt ("...already
  exists. Do you want to replace it?") - the whole Save As flow was
  silently stuck waiting on it indefinitely. Fixed in the test, not the
  app (this is correct, expected OS behavior the test needed to handle).
- `5.1/5.2`: closing and reopening a model recreates its entire
  sub-window tree from scratch (new handles for "Basic input" etc. even
  though they're logically the same panel) - a one-time-only baseline
  snapshot wrongly flagged all of this as unexpected. Fixed by
  re-capturing the baseline after every close/reopen cycle.
- `Run-AllTests.ps1` itself: `Run-StagingTests.ps1` never closed its own
  shared instance at the end, so it could still be running when the
  next platform's pass started, interfering with that pass's own
  orphan-sweep tests via the shared staging folder. Fixed: it now
  explicitly closes whatever instance it itself launched.

**`Tools\DialogChecker\`** - a small standalone C# console tool (own
`.csproj`, rebuilds automatically whenever its `Program.cs` is newer
than the last build), wired in via `TestHelpers.ps1`'s
`Get-WindowBaseline`/`Assert-NoNewDialogs`/`Register-DialogCheck` as an
extra safety net: a test captures a baseline when idle, and either
`Invoke-Test` itself (for `Run-StagingTests.ps1`'s one shared instance
across the whole file) or the test's own code (for the other three
runners, where every test stops its own disposable instance within its
own body - the automatic "after the test body" timing doesn't help
there, since the process is already gone by then) asserts no new window
has appeared since. Took several rounds to get right - worth knowing if
touching this again:
- Detection is `TrueCondition` search + **client-side** filtering by
  `ControlType.Window`, NOT a `PropertyCondition` passed directly to
  `FindAll` (even a plain positive one - confirmed, the hard way, twice,
  that this specific mixed-provider tree silently fails to match via a
  server-side condition, while `TrueCondition` reliably reaches
  everything).
- `Get-WindowBaseline` polls until the window set actually **settles**
  (two consecutive snapshots matching) rather than trusting a single
  snapshot taken immediately after launch - `Start-TestInstance.ps1`
  only waits for the main window, not every sub-panel.
- A classic PowerShell gotcha bit this twice: `Where-Object` unwraps a
  single match into a bare scalar with no `.Count` property, silently
  `$null`, making `.Count -gt 0` false even with a real match. Fixed
  with `@(...)` wrapping in `Assert-NoNewDialogs` and
  `Show-TestSummary`'s own pass/fail/skip counts.
- An earlier event-based approach (`WindowOpenedEvent`,
  `Test-EventBasedDialogDetection.ps1`) was abandoned: it registers
  without error, but the callback never actually fires for any window at
  all - PowerShell scriptblocks cast to a .NET delegate don't reliably
  execute when invoked from a thread other than PowerShell's own, which
  is how UI Automation's event delivery happens. Left in place with its
  own comments explaining this, in case it's ever worth revisiting with
  a proper C# shim instead.

### NEW, small feature: model-kind-detection status-bar tags (just added,
### NOT yet tested by the user)

Separate from the staging backport and the test suite above - this is
about **old-model detection on Open**, a different area of the app
entirely (`frmEwE6.CovertToEwE6`, `cDataSourceFactory.GetCompatibility`,
from the earlier "Legacy Jet3 Import" chapter of this same chat).

**The ask:** opening an old model routes to one of several places
(refer the user to EwE5, the in-process import wizard, or the 64-bit
process's x86 `EwEImportHelper`) depending on file format, byte-level
Jet version, and process bitness. The user wants to enumerate exactly
which test branches this needs, then find/prepare a real sample model
file for each one and automate it - and asked for a status-bar message
reporting which branch was detected, to make that file-hunting process
recognizable while testing manually.

**Delivered as `ModelKindDetectionFix.zip`** (`cDatasourceFactory.vb`,
`frmEwE6.vb`). Not yet confirmed compiling or working by the user -
treat as freshly written, same scrutiny as anything else this session
gets on its first real run.

**The full taxonomy** (9 distinct, testable code paths - more nuanced
than the three outcomes named in the request, since `GetCompatibility`
has internal sub-paths, like the x86 helper's version-check succeeding
vs. failing vs. never running, that aren't visible from its return value
alone):

| Tag (exact status-bar text) | Code path | Maps to |
|---|---|---|
| `TooOld-Jet3x64-confirmed` | 64-bit, confirmed Jet3, x86 helper ran and confirmed a real too-old version | Refer to EwE5 |
| `TooOld-Jet3x64-unconfirmed` | 64-bit, confirmed Jet3, helper itself failed/missing/timed out → real version couldn't be confirmed, conservative block | Refer to EwE5 (different code, same message) |
| `TooOld-DirectOpen` | 32-bit (any), or 64-bit non-Jet3-byte file, opened directly, genuinely too old | Refer to EwE5 (no helper involved) |
| `Import-x64Helper` | 64-bit, confirmed Jet3, Importable → full import via x86 `EwEImportHelper` | 64-bit EwEImportHelper |
| `Import-Wizard32bitJet3` | 32-bit process, Jet3-by-signature, Importable → in-process import wizard | 32-bit model conversion utility |
| `Import-WizardNonJet3` | Either platform, non-Jet3-byte EwE5-era file, Importable → same wizard, different precondition | Also the import wizard - worth its own test, different code path |
| `Current-EwE6` | Already current format | baseline sanity, not "old model" |
| `Future-NewerVersion` | Newer than this build supports | not "old model", shares the same routing |
| `Invalid-Unknown` | Unrecognized/corrupt file | not "old model", shares the same routing |

**Implementation:**
- `cDataSourceFactory.GetCompatibility` gained a new `Optional ByRef
  strDetectionPath As String` out-parameter (backward compatible - it
  had exactly one caller, confirmed by grep, so this was low-risk) set
  to `"Jet3x64-confirmed"` / `"Jet3x64-unconfirmed"` / `"DirectOpen"` (or
  the plain `eDataSourceTypes` name for the EII/EIIXML/EcoBase/NotSet
  cases) - this is the only place that distinction is actually knowable,
  since the caller has no way to tell "helper ran and confirmed TooOld"
  apart from "helper itself failed" otherwise. (Named for what we
  actually know - whether a real version number could be confirmed -
  rather than for the helper being the implementation detail that got
  us there.)
- `frmEwE6.CovertToEwE6` combines this with its own, already-visible
  `IsLegacyJetFormat`/`Is64BitProcess` check (for the `Importable` case's
  own sub-routing) into one `strModelKindTag`, captured **before** any
  later reassignment of `comp` (a successful import changes `comp` to
  `EwE6`, which would otherwise make a successful 32-bit-Jet3 import
  indistinguishable from a successful non-Jet3 one).
- New `Private Sub ReportDetectedModelKind(strTag As String)` writes
  `"[ModelKind] " & strTag` via `Me.m_ssMain.SetStatusText(strTag, 0)`
  directly - **not** through `ShowProgress`/`SendMessage`. Documented,
  real risk worth knowing: `ShowProgress`'s own wait-cursor-stack
  bookkeeping (see the "Status feedback" region in `frmEwE6.vb`) can
  clear or overwrite arbitrary status text shortly after this runs, as
  part of the surrounding Open-model flow's own normal Start/Finished
  pair. This diagnostic accepts that risk deliberately: the intended use
  is checking the status bar **immediately** after triggering an open
  (the same pattern this suite already uses for transient dialog text
  elsewhere), not relying on the tag staying visible indefinitely. If it
  turns out to vanish too fast to be useful even for that, the fix would
  be capturing it somewhere more persistent (the window title, a
  dedicated new status-strip panel) rather than fighting
  `ShowProgress`'s own stack.
- Marked throughout as a diagnostic aid, not permanent product behavior
  - safe to strip out once real per-branch test coverage exists for all
  nine tags above.

**Sample models found so far, mapped to their tag (user's own notes,
2026-11-18, all found/tested in the 64-bit build so far):**

| Model file | Tag | Notes |
|---|---|---|
| `Gironde estuary.MDB` | `Import-x64Helper` | Imports cleanly |
| `Kenya, Lake Nakuru - 1972.mdb` | `TooOld-Jet3x64-confirmed` | |
| `gulfmexico.MDB` | `Import-x64Helper` | **Import fails**: `ArgumentException: Column 'SalinityForcingShapeID' do[es not belong...]`, during Ecospace shape import (many "already exists as ID ###" lines first). Full dialog text, Accessibility Insights tree, and the resulting "Import failed" error dialog are all captured in screenshots from this message. **Good first candidate to automate as a test**, since it's a real, reproducible failure, not just a happy path. |
| `Ecopath.MDB` | `Import-WizardNonJet3` | |
| `EcopathMSY.MDB` | `TooOld-DirectOpen` | |
| `GBR_Linked_ecosystem.MDB` | `Import-x64Helper` | Has multiple submodels - shows a model-selection menu first. After picking one, import fails with an `EatEffBad` error (different error than gulfmexico's). **To be looked at later; also a good test candidate.** |

Also captured: a full successful import transcript (`SEAK 1999...`
completing at 100% with "Import completed successfully") - useful as
the happy-path counterpart when building the `Import-x64Helper` test.

Not yet found: `Jet3x64-unconfirmed` (needs a way to make the x86 helper
itself fail/time out/go missing, not just find a file), `DirectOpen`'s
non-TooOld outcomes, `Current-EwE6`, `Future-NewerVersion`,
`Invalid-Unknown`, `EII`/`EIIXML`/`EcoBase` cases. All of the above were
found while running the 64-bit build - the 32-bit build still needs its
own pass, since several tags (`Import-Wizard32bitJet3` in particular)
can only be reached there.

**Next steps for this feature, not yet done:**
1. User compiles and confirms it builds (brand new code, zero
   confirmation yet).
2. For each of the 9 tags above, find or construct a real sample model
   file that triggers it, confirm the status bar shows the expected
   tag, note the file. (In progress - see table above.)
3. Automate a test per branch in the existing UI-Automation suite
   (`Scripts\`), following the same patterns already established there
   (`Start-TestInstance.ps1`, `Find-ByNameAndType`,
   `Wait-ForMainWindow`'s own upgrade-dialog handling, etc.) - reading
   the status-bar text via `DialogChecker`-style UI Automation, or a
   simpler direct `FindFirst` on the status strip control, whichever
   turns out more reliable once tried. Starting candidate: `gulfmexico.MDB`
   (`Import-x64Helper`, reproducible `SalinityForcingShapeID` import
   failure) - a real failure case, not just a happy path.
4. Once all 9 are covered, this feature (the tag-reporting code itself)
   can be removed per the "safe to remove" note above - it was never
   meant to be permanent.

---

## Context

Repo: `https://github.com/Official-EwE/Eii.Ecopath` (VB.NET, net48 WinForms +
net10.0, multi-targeted). Two branches involved:
- `cEwEEFDatabase` — has SQLite/EF support, where the local-staging-copy
  feature (save-safety against cloud-sync corruption, e.g. OneDrive) was
  originally built and tested.
- `master` — Access-only, no SQLite/EF. **This is the branch all work in
  this chat is backporting the staging feature onto.**

The feature: on a write-intent `Open()`, stage a private local working
copy under `%LocalAppData%\Ecopath with Ecosim\DBs\`; edit that copy;
`Save()` blind-overwrites the real file via copy-sidecar/delete/rename.
Shrinks the corruption window from "the whole time the file is open" to
one fast copy per save.

## MAJOR UPDATE (same ongoing chat, written well after the rest of this
## document) — read this first, before anything below

Everything below this point describes where things stood **before** a
full PowerShell UI-automation test suite was built in this same chat.
That suite now exists, has been run (both x64 and x86), and has
**superseded almost everything in "Still pending" below** — either by
automating it, or by explicitly, deliberately deciding not to (with a
documented reason) after actually weighing it, not by default.

- **`LocalStagingCopyTestPlan.md` now marks every item AUTOMATED,
  PARTIALLY AUTOMATED (with a precise note on exactly what is and isn't
  covered), or left unticked with a stated reason** - see that file
  directly for the authoritative, current, item-by-item status rather
  than the "Still pending" list further down in this document, which is
  now stale.
- **A real, previously-unknown bug was found and fixed** by the new
  automated test suite (not a pre-existing manual-test finding): in
  `cCore.vb`'s `Save()`, cancelling the publish-back dialog correctly
  set `bSucces = False`, but nothing downstream told the data source it
  was still modified - `SaveChanges()` had already (correctly, from its
  own narrow perspective) cleared the dirty flag after persisting to
  the local staged copy, before the publish-back step even began. The
  Save button stayed disabled and the app behaved as though the save
  had succeeded, even though the real file was never actually updated -
  a genuine silent-data-loss bug. Fixed with one line:
  `Me.DataSource.SetChanged(eCoreComponentType.DataSource)` in the
  Cancel branch, using the data source's own existing public API (no
  new methods needed - confirmed via `IEwEDataSource`/`cDBDataSource`
  that `DataSource` is already a member of `cDBDataSource`'s own
  `s_EcopathComponents`, so this correctly restores both `IsModified()`
  and `IsEcopathModified()`).
- **A genuine, pre-existing, platform-dependent app behavior was
  discovered and worth knowing about for any future Save-As-related
  work**: `cDataSourceFactory.GetSupportedType()` checks
  `cSystemUtils.Is64BitProcess()` *inside* its `.mdb`/`.ewemdb` case
  specifically - resolving to `Access2007` (default extension
  `.eweaccdb`, via `GetDefaultExtension()`) on a 64-bit process, but
  `Access2003` (`.ewemdb`) on a 32-bit one, for the exact same source
  file. An `.accdb`/`.eweaccdb` source has no such branch - always
  `Access2007`. Combined with `frmEwE6.vb`'s `OnSaveModelAs` (comment:
  `JS 27Jul08: Only able to save in current file format (save as
  between formats not supported by the core)`), this means **"Save
  Model As..." on the same `.ewemdb` file targets `.eweaccdb` on x64
  but `.ewemdb` on x86** - not a bug, just something to account for
  explicitly (as the test suite now does) rather than assume.
- A DEBUG-only test hook was added to `cCore.Save()` (a flag file
  checked right after save, before publish-back, with a `.ack` signal
  file so an external script can act deterministically rather than
  guessing a delay) - zero cost or behavior change in a Release build.
  Used by the suite's kill-mid-save test.
- The test suite itself lives in `Scripts\` alongside the backport
  (`Run-StagingTests.ps1`, `Run-CrashRecoveryTests.ps1`,
  `Run-MultiInstanceTests.ps1`, `Run-AllTests.ps1` to run everything on
  both platforms with one combined summary) - built via UI Automation
  against a real, running `ScientificInterface.exe`, not mocks.

## Where things stand right now (as of BEFORE the update above - kept
## for history)

**The backport compiles and the user has confirmed it works correctly**,
including a just-fixed bug (see below). The user is mid-way through
`LocalStagingCopyTestPlan.md` (30 items) on their own machine — **this
chat has not seen the results of most of that manual test plan**, only
the compile-error fixes and one specific bug hunt below.

## IMPORTANT: sandbox state does not carry over

Everything below was done via sparse git checkouts and scratch files in
the previous session's sandbox (`/home/claude/...`), which will **not**
exist in a new chat. A new chat will need to re-clone/re-fetch from
GitHub as needed. The master branch commit this work was done against:
`ac9f7ecc76b287f5993e714ee2c6b00c997d6cac` (2026-10-30) — check whether
master has moved since.

## Files changed on master (delivered individually across this chat,
## not as a single zip — the user has already integrated all of them)

1. **`Sources/EwECore/Database/cEwEDatabase.vb`** — the staging mechanism
   itself (from the original master-branch backport zip, done in an
   earlier chat — see that zip's own `STAGING_BACKPORT_NOTES.md` for
   full detail). One fix made *in this chat*: an XML-doc-comment bug —
   literal `<name>` in prose inside a `'''` comment was being parsed as
   an invalid XML tag (`BC42304` warnings). Fixed by escaping to
   `&lt;name&gt;`.

2. **`Sources/EwECore/Database/cEwEAccessDatabase.vb`** — from the
   original backport (staging wired into Open/Close/SaveAs, exclusive
   mode removed). **New in this chat:** `Public Shared
   IsLegacyJetFormat(path As String) As Boolean` — reads the single Jet
   engine version byte at file offset `0x14` and returns True only for
   byte value `0` (genuine Jet3/pre-Access-2000). **Byte-value mapping
   verified directly against mdbtools' own source**
   (`include/mdbtools.h.in`, `MDB_VER_JET3=0, MDB_VER_JET4=0x01,
   MDB_VER_ACCDB_2007=0x02, MDB_VER_ACCDB_2010=0x03`) — not trusted from
   memory, actually fetched and checked (`git clone
   https://github.com/mdbtools/mdbtools.git`).

   **History worth knowing:** an *earlier, wrong* approach put this
   check directly inside `Open()` itself (short-circuiting before
   ACE/OLEDB, returning `Failed_OSUnsupported`). This was **reverted** —
   it prevented `cEwEDatabase.Compatibility()` from ever running at all,
   which masked a *more correct*, pre-existing "outdated Ecopath 5
   database" message. See next file for the actual fix.

3. **`Sources/EwECore/DataSource/cDatasourceFactory.vb`** — **new in
   this chat.** `GetCompatibility`'s Access-case branch now calls
   `cEwEAccessDatabase.IsLegacyJetFormat(strDatabase)` *before*
   attempting `db.Open()`. On a hit: sets
   `comp = eCompatibilityTypes.TooOld` and
   `access = eDatasourceAccessType.Failed_UnknownType` directly, with
   zero connection attempt. Every other case (Jet4+, ACCDB, non-legacy)
   falls through to the original, unchanged `Open()`/`Compatibility()`
   logic unchanged.

4. **`Sources/ScientificInterface/frmEwE6.vb`** — **new in this chat,
   and the one file needing care if continuing work on it.** Only
   change: in `CovertToEwE6`, the access-error gate
   (`If (access <> eDatasourceAccessType.Opened) Then ...`) now also
   excludes the case where `comp = TooOld`
   (`AndAlso (comp <> cEwEDatabase.eCompatibilityTypes.TooOld)`), so a
   legacy-Jet3 file (which never successfully opens on 64-bit) still
   falls through to the existing `Select Case comp` block's own,
   pre-existing `TooOld` branch — showing the correct "outdated Ecopath
   5 database" message — instead of being swallowed by the generic
   access-error report first.

   **⚠️ Known mistake, now corrected, but worth double-checking if
   picking this file up again:** partway through this chat, the
   assistant accidentally worked from the **`cEwEEFDatabase` branch's**
   copy of `frmEwE6.vb` (with `eLoadSourceType.Internal`,
   `cEwEEFDatabase`, `cEwEVersusDatabase`,
   `eDataSourceTypes.AccessVsSqlite/Sqlite`,
   `FILEFILTER_SAVE_SQLITE`, `cMdb2SqliteConverter` etc. still in it —
   none of which exist on master) instead of master's own file. This
   caused ~14 compile errors. **Fixed**: the file was rebuilt from
   master's actual `frmEwE6.vb` with *only* the one `TooOld`/`access`
   fix above applied — confirmed zero SQLite references remain. All
   nine other backport files were checked and confirmed clean (this
   contamination was isolated to this one file, since it wasn't part of
   the original backport zip — it was fetched separately, later, for
   this specific investigation).

   `eLoadSourceType.Internal` is **confirmed NOT needed on master** —
   its only purpose in the EF branch was marking a transient load during
   the Access-to-SQLite migration flow, which doesn't exist on master at
   all. Master's own `AddModelMRU` call is and should remain
   unconditional.

## Root-cause chain for the just-fixed bug (useful if similar issues
## come up again)

- No 64-bit build of the classic `Microsoft.Jet.OLEDB.4.0` provider
  exists at all → every `.mdb`/`.accdb` on 64-bit gets routed through
  ACE (`Microsoft.ACE.OLEDB.12.0`) regardless of actual internal format
  (`cDataSourceFactory.GetSupportedType`).
- ACE cannot open a genuine Jet3 file at all → `OleDbConnection.Open()`
  throws `OleDbException` with `HResult = -2147467259` (`E_FAIL`, a
  generic code) → **pre-existing**, unrelated-to-staging bug: this gets
  unconditionally mapped to `Failed_AlreadyInUse`, producing a
  misleading "already open" message for a file that isn't locked at
  all. **Left as-is per the user's decision** — flagged as a known,
  separate, low-priority issue, not fixed in this chat.
- On 32-bit, the classic Jet provider opens the same file fine, letting
  `cEwEDatabase.Compatibility()` → `GetVersion()` run its real check:
  queries `[UpdateLog]` first (doesn't exist in old Ecopath-5-era
  files), falls back to `[Database specifications]` (which does exist),
  reads the real Ecopath 5 version number, and correctly returns
  `TooOld`.
- This is why `IsLegacyJetFormat` was needed: it's the only way to reach
  the same `TooOld` conclusion on 64-bit, since ACE can never establish
  the connection `Compatibility()` needs to read that real version
  number.

## Still pending (per the priority list established earlier) - STALE, see
## the "MAJOR UPDATE" section near the top of this document instead

Test plan items not yet confirmed by the user in this chat, roughly in
priority order discussed earlier:
1. **2.4/2.4a/2.4b** — hold-lock/retry/dialog, `cRestartManagerLookup` —
   never run against real Access/OleDb, only EF/SQLite originally.
2. **2.1/2.2** — kill-mid-save, step through copy-sidecar-delete-rename
   — never run against a real Access connection.
3. **6.1/6.2** — SaveAs re-staging — most intricate untested code.
4. **8.1** — legacy update chain (`RunAllUpdates`) + staging interaction.
5. **3.2/3.3** — multi-instance, no exclusive mode.
6. **5.1–5.4** — exit-time cleanup (the one area automated tests can't
   cover at all — needs a real process-lifecycle test).
7. **9.1–9.3** — regression checks (reader/transaction disposal,
   `ReleaseWriter` return-value fix).
8. Lower priority: 1.2/1.3, 4.1–4.3, 7.1–7.4.

Also still open: the standalone `EwECore.Staging`/`EwECore.Staging.Tests`
test project (`dotnet test`, 11 pass + 1 skip expected) — not yet
confirmed run against the current state of the backport in this chat.

## Known, deliberately-left-alone issues

- The misleading `Failed_AlreadyInUse` message for any genuine `E_FAIL`
  OleDbException (not just true locks) — pre-existing, unrelated to
  staging, user explicitly deferred fixing this.
- `.laccdb` lock files appearing in the staging folder — this is ACE's
  own automatic lock file, not something the staging code creates or
  can rename; confirmed no collision risk because staged copies always
  carry a GUID in their base filename.

## Useful artifacts from earlier in this chat (may need regenerating)

- `LocalStagingCopy_MasterBackport.zip` — the original full backport
  deliverable (9 source files + standalone test project under
  `Sources/EwECore.Staging*` as siblings of `Sources/EwECore/`, NOT
  nested inside it — nesting broke the real build via implicit
  glob-include — plus `STAGING_BACKPORT_NOTES.md` and
  `LocalStagingCopyTestPlan.md`). The three newest fixes (this
  session's `cEwEAccessDatabase.vb`/`cDatasourceFactory.vb`/
  `frmEwE6.vb` changes) are **not yet folded into that zip** — it would
  need regenerating if a fresh, complete deliverable is wanted.
