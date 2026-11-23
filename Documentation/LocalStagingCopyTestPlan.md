# Local Staging Copy - Test Plan (master / Access-only)

Covers `cEwEDatabase`'s local-working-copy mechanism (`StageLocalWorkingCopy`
/ `PublishLocalCopyToSource` / `CleanUpLocalWorkingCopy`) and its supporting
cleanup machinery, as backported to the `master` branch - Access only, no
SQLite/EF. Every scenario here, except where noted, applies identically on
the `cEwEEFDatabase` branch, since this mechanism lives in `cEwEDatabase.vb`
itself and never was format-specific.

**AUTOMATED** marks an item covered by the PowerShell test suite in
`Scripts\` (`Run-StagingTests.ps1`, `Run-CrashRecoveryTests.ps1`,
`Run-MultiInstanceTests.ps1`, or `Run-AllTests.ps1` to run everything).
Where only part of an item is covered, it's marked **PARTIALLY AUTOMATED**
with a note on exactly what is and isn't checked, and the checkbox stays
unticked - only a genuinely complete item gets `[x]`.

**This is not a locking mechanism.** There is no lock of any kind
protecting the original file, and no multi-user/multi-session detection
anywhere. Avoiding concurrent edits to the same file is entirely the
user's own responsibility - last save wins, with no detection or warning.
What this *does* protect is a single session's own Save from cloud-sync
(OneDrive etc.) interference, by shrinking the window the real file is
ever touched down to one fast copy per Save, rather than the entire time
a model is open.

**Known, accepted limitation - not a bug if observed during testing**: a
session's local working copy in `%LocalAppData%\Ecopath with Ecosim\DBs\`
may remain on disk for as long as the app keeps running after that
session closes, even after `Close()` runs. The underlying connection does
not always release its file handle immediately, regardless of how
thoroughly `Close()` disposes things in-process. This is expected. It
gets cleaned up either by the exit-time sweep on normal app close, or by
the orphan sweep on the next write-intent `Open()` anywhere on the
machine (the crash-recovery path). Don't file this as a defect if seen
mid-session with the app still open.

## Core tests

### 1. Opening - staging creation

- [x] **1.1. (AUTOMATED - Run-StagingTests.ps1)** Confirm a
  `Temp<name>.<GUID>.eweaccdb` working copy (never `.tmp` as the extension
  - `cDataSourceFactory.GetSupportedType` determines type purely from
  `Path.GetExtension`, so a `.tmp`-suffixed copy would be invisible to it)
  appears under `%LocalAppData%\Ecopath with Ecosim\DBs\` on opening a
  database for read-write (`EnableLocalStaging = True` in
  `ScientificInterface`). This confirms a copy gets *created*, not that
  the live session is actually *using* it - pair with the check below.
  - **Rename the original while the model is open, before saving.** If
    staging is genuinely in effect, this has **zero effect** on the open
    session - the live connection never touches the original at all
    until Save, and `File.Copy(..., overwrite:=True)` will happily
    recreate the file at that path when Save eventually runs, since only
    the file was gone, not its parent folder. If the connection were
    (incorrectly) against the original, this causes an immediate, obvious
    error instead. *Automated as a separate test, run after §7.1 (since it
    causes a real Save, which would otherwise break that test's own
    untouched-timestamp assertion if run first) - confirms no dialog
    appears immediately from the rename, then that a normal edit+Save
    still completes cleanly and recreates a file at the original path.*
- [ ] **1.2. (PARTIALLY AUTOMATED - Run-MultiInstanceTests.ps1)** Three
  scenarios in one pass - open the same file from a second instance while
  the first is still open, then check all three:
  - **Old/incompatible-version file:** confirm **no** working copy is
    created (staging is write-intent-only) and that the title bar
    *correctly does* show read-only - the one legitimate remaining case
    for it. *Not automated - no old/incompatible-version fixture file
    available.*
  - **Normal, current-version file, opened from a second instance while
    the first already has it open:** confirm the app **never** enters
    read-only mode or shows it in the title bar - the behavior the old,
    now-removed lock-degrade mechanism would have triggered here. Each
    instance should simply get its own independent working copy, with no
    error, dialog, or degrade of any kind (this also covers what used to
    be a separate "§3.2" multi-instance item - it's the same scenario).
    *Automated - the "1.2" test.*
- [x] **1.3. (AUTOMATED - Run-NoStagingCallerTests.ps1)** **Any caller
  that never sets `EnableLocalStaging`** (e.g. `EwERunConsole`): confirm
  opening a database creates no working copy at all, and connects
  directly to the real file - staging is strictly opt-in, not on by
  default for every `EwECore.dll` consumer.
  - Automated with no running app and no UI Automation at all, unlike
    every other test here - loads `EwECore.dll` directly and calls into
    `cCore` the same minimal way a real non-UI consumer does (confirmed
    via `Eii.Ecopath.Runner.Services`' own `CoreService.cs`: `new
    cCore()` + `.PluginManager = new cPluginManager()` +
    `.LoadModel(path)`, with `EnableLocalStaging` never touched
    anywhere in that sequence).

### 2. Saving - publish-to-source

- [x] **2.1. (AUTOMATED - Run-CrashRecoveryTests.ps1)** **Kill the app (or
  pull power) mid-save, before the publish-back step** (i.e. the write to
  the local working copy is still in progress). Confirm the *original*
  file is untouched (it hasn't been touched yet at this point) and
  reopening produces a fresh, correct working copy.
  - Automated via the `EwE_TestWaitBeforePublish` DEBUG-only test hook in
    `cCore.Save()` (with its own `.ack` signal file, so the kill happens
    deterministically rather than via a guessed delay) rather than a
    manual breakpoint - same deterministic principle, no human needed.
- [x] **2.2. (AUTOMATED - Run-StagingTests.ps1)** Save, then confirm the
  original file's bytes now match the working copy's, with no
  fingerprint check or conflict prompt of any kind - a blind overwrite
  is the intended behavior, not a bug to fix.
  - Folded into the "2.x" test (below) as a SHA256 hash comparison
    between the real file and the staged working copy, taken right
    after Save completes.
- [ ] **2.3. (PARTIALLY AUTOMATED - Run-StagingTests.ps1)** **Hold the
  original file open elsewhere, at two different durations**, using the
  same script both times (just change `-Seconds`):
  ```powershell
  $path = "D:\path\to\your\model.eweaccdb"  # the REAL path, not the staged copy
  $fs = [System.IO.File]::Open($path, 'Open', 'ReadWrite', 'None')
  Start-Sleep -Seconds 1.5   # short run: ~1.5s. long run: ~5s+
  $fs.Close()
  Write-Host "Lock released."
  ```
  Run it, and *while it's sleeping*, trigger Save in EwE - the timing
  only works if Save's first attempt actually lands while the lock is
  still held.
  - **Short hold (~1.5s, inside the ~3-second retry window):** confirm
    the internal retry (`cEwEDatabase.PublishLocalCopyToSource`, ~6
    attempts, 500ms apart) resolves silently - no dialog, no noticeable
    UI block. *Automated - the "2.3-short" test.*
  - **Long hold (~5s+, past the window):** confirm the "cannot write
    changes, locked by X" dialog appears, with Retry and Cancel -
    release the lock and click **Retry** (confirm the save completes and
    the dialog closes), then repeat and click **Cancel** instead
    (confirm `Save()` returns `False`, the app returns to normal use,
    and the in-memory model is unaffected - the user can try Save again
    later). While the dialog is up either time, also note whether it
    names the actual locking process (e.g. "Windows PowerShell" for a
    `powershell.exe` holder) or falls back to generic "another
    application" phrasing - neither outcome should throw or block
    Retry/Cancel.
    *Automated: the dialog appearing, naming/logging its message text,
    and the Cancel path (also exercised more thoroughly by §8.1 below,
    which found and led to fixing a real bug in this exact path).*
    *Not automated: clicking Retry/OK and confirming the save then
    completes successfully.*

### 3. No multi-user coordination (by design)

- [x] **3.1.** No locking of any kind exists - a deliberate product
  decision (avoiding concurrent edits to the same file is entirely the
  user's own responsibility; last save wins with no detection or
  warning).

(A former "3.2" - edit and save from both instances, confirm whichever
saves last wins - was removed entirely, not automated or left as a
manual item: the stakeholders have explicitly said multi-user conflict
resolution is out of scope, so there's nothing left to actually prove
here beyond what §1.2 already covers - that two instances coexist
without interfering with each other. The only way to *make* "last save
wins" an unambiguous claim would be reading the result back through a
fresh instance rather than one of the two that just saved, since an
already-open instance's own in-memory state has no reason to reflect a
different instance's later write to the same file - not worth the extra
complexity for a claim nobody's asking to have proven.)

### 4. Crash recovery

(Process killed, not a normal close - see §5 below for the graceful-close
path.)

- [x] **4.1. (AUTOMATED - Run-CrashRecoveryTests.ps1)** Kill a session's
  process directly (Task Manager / `taskkill`, not a normal close), then
  open **any** database (not necessarily the same one) on that machine
  afterward - confirm the orphaned working copy is swept away on that
  next write-intent open, not left behind indefinitely.
- [x] **4.2. (AUTOMATED - Run-CrashRecoveryTests.ps1, as "4.3")** Leave
  several crashed sessions' worth of orphaned copies in
  `%LocalAppData%\Ecopath with Ecosim\DBs\`, then open one more
  write-intent session - confirm the sweep clears all of them in that
  single pass, not just the most recent one.
  - The script's own test is named "4.3" internally (a numbering
    mismatch from when it was written) - it's this item, not a separate
    one; §O.2 immediately below it in that script is the actual "4.2" by
    this plan's own numbering (deliberately not automated - see below).

### 5. Exit-time cleanup

(Normal, graceful app close - a different mechanism from crash recovery
above.)

- [x] **5.1. (AUTOMATED - Run-CrashRecoveryTests.ps1)** Open and close
  **several** read-write sessions in one app run (so each potentially
  leaves its own marker-less copy behind, per the known limitation at
  the top of this document), then close the app normally - confirm
  every one of those copies is gone from `%LocalAppData%\Ecopath with
  Ecosim\DBs\` shortly after the app fully exits, in a single pass.
  - Closes and reopens the same model twice within one continuous run
    (via `Close-CurrentModel`/`Open-ModelViaUI` in `TestHelpers.ps1`,
    confirmed working, including handling the database-upgrade prompt
    that can also appear on a UI-driven Open, not just at initial
    launch), tracking every cycle's own staged copy and confirming all
    of them - not just the last - are gone after the final close.
- [x] **5.2. (AUTOMATED - Run-CrashRecoveryTests.ps1)** **Regression test
  for a real bug found and fixed during development**: close a model,
  then close the whole application - confirm the app closes promptly,
  with no perceptible hang or delay. An earlier draft of this mechanism
  used a synchronous, sleep-based retry on `Close()` that measurably
  delayed application shutdown; that was removed specifically because of
  this.
  - Automated with an actual stopwatch, asserting under 5 seconds - not
    just "did it eventually close."
- [ ] **5.3.** Open the app with `EnableLocalStaging` off (or never open
  any database), then close it - confirm no cleanup script or process
  appears at all. The exit-time hook only registers the first time a
  session actually stages a working copy.
- [x] **5.4. (AUTOMATED - Run-CrashRecoveryTests.ps1)** Kill the app (not
  a normal close) after leaving a working copy behind - confirm the
  exit-time sweep does **not** run (`AppDomain.ProcessExit` isn't
  reliably raised on a hard kill) - the file should instead sit until the
  orphan sweep catches it on the next write-intent `Open()`, per §4
  above.

### 6. SaveAs re-staging

- [ ] **6.1. (PARTIALLY AUTOMATED - Run-StagingTests.ps1 and
  Run-MultiInstanceTests.ps1)** From a session, use Save As to a new path
  - confirm the old working copy is cleaned up and a fresh one is staged
  for the new target, and that a second instance can open the *original*
  file afterward with no restriction of any kind. While here, also try
  Save As to a path another session already has open as its own target -
  expect the same no-degrade behavior as §1.2, just reached via Save
  As instead of Open.
  - *Automated: the new target file gets created successfully ("6.1"),
    and Save As onto a path another instance already has open completes
    with no degrade ("6.2").*
  - *Not automated: confirming the OLD working copy is actually cleaned
    up after Save As, and that the original file remains openable by a
    second instance afterward.*
  - **TO BE REVIEWED:** confirmed directly from `cDataSourceFactory.vb`'s
    source - `GetSupportedType()` checks `cSystemUtils.Is64BitProcess()`
    *inside* its `.mdb`/`.ewemdb` case specifically, resolving to
    `Access2007` (default extension `.eweaccdb`) on a 64-bit process, but
    `Access2003` (`.ewemdb`) on a 32-bit one - for the exact same source
    file. Combined with `frmEwE6.vb`'s own `OnSaveModelAs` comment
    (`JS 27Jul08: Only able to save in current file format`), this means
    **the same `.ewemdb` model's "Save Model As..." targets a different
    file format depending on which bitness of the app opened it** - x64
    offers/produces `.eweaccdb`, x86 offers/produces `.ewemdb`. Not
    clearly a bug (the `Is64BitProcess()` check appears to exist for an
    unrelated reason - routing legacy Jet3 files through ACE on 64-bit,
    since no 64-bit Jet provider exists at all - and may be having this
    side effect only incidentally here), but worth a deliberate decision
    from whoever owns this behavior rather than leaving it as an
    unexamined side effect: is a 32-bit user being silently kept on the
    older Access 2003 format when saving as a *new* file name actually
    intended, or should Save As resolve the target format some other
    way, independent of process bitness?

### 7. Legacy database update chain safety

- [x] **7.1. (AUTOMATED - Run-StagingTests.ps1)** **Confirm the original
  Access file's last-write time never changes while opening an old
  database that needs its legacy update chain (`RunAllUpdates`) to run.**
  With staging active, the update chain runs against the staged working
  copy - the original is read once (to make the copy) and never written
  to. Note the file's timestamp before opening, confirm it's unchanged
  after the updates have completed and the model is fully loaded.
  - The sample fixture used by the test suite (`Anchovy Bay
    Spatial.ewemdb`) already triggers this update chain on every run, so
    no separate old-format fixture was needed.

### 8. Regression checks (unrelated fixes found during this work)

These touch core code paths unrelated to staging specifically, found and
fixed while chasing the connection-release investigation that led to the
exit-time cleanup mechanism above - included here because they landed in
the same files this backport touches.

- [x] **8.1. (AUTOMATED - Run-StagingTests.ps1)** **Deliberately trigger a
  failed save** (hold the real file locked past the retry window - same
  mechanism as §2.3's long hold - then click Cancel instead of Retry) and
  confirm it's actually reported as a failure: Save stays enabled
  afterward (the in-memory change is still pending, not silently
  discarded) and the real file is byte-for-byte unchanged. This is the
  one that actually matters - `cDBDataSource`'s `SaveXXX()` methods once
  discarded `ReleaseWriter`'s own return value, meaning a save could
  report success even when the underlying write genuinely failed. (The
  other two fixes from this same investigation -
  `LoadEcopathSamples`/`LoadGroupSamples`/`LoadDietSamples`/
  `LoadGroupCatchSamples` now properly closing their reader, and
  `CommitTransaction`/`RollbackTransaction` now disposing the
  transaction object - are leak fixes with no simple pass/fail signal
  from outside the process; normal day-to-day use of the app is the
  practical check for those, not a dedicated test here.)
  - **This test found a real, previously-unknown bug**, since fixed:
    cancelling the publish-back dialog correctly set `bSucces = False`
    in `cCore.Save()`, but nothing downstream told the data source it
    was still modified - `SaveChanges()` had already (correctly, from
    its own narrow perspective) cleared the dirty flag after persisting
    to the local staged copy, before the publish-back step even began.
    Fixed by calling `Me.DataSource.SetChanged(eCoreComponentType.
    DataSource)` in the Cancel branch, using the data source's own
    existing public API - no new methods needed.

## Optional / exotic (best-effort, skip if time or environment is short)

- [ ] **O.1.** Step through `PublishLocalCopyToSource`'s copy-sidecar-
  delete-rename sequence in the debugger, confirming the real file's
  state after each line (unchanged after the sidecar copy; missing, with
  `<realfile>.publishing` holding the full new content, after the
  delete; correctly updated after the rename). `File.Move`'s atomicity
  on the same volume is documented platform behavior, not something this
  code's own logic could get wrong - §2.1/§2.2 above already give solid
  end-to-end confidence without this. Not automatable by nature
  (debugger step-through).
- [ ] **O.2.** If reproducible: kill a session's process **mid-copy**
  (e.g. trigger open on a large file and kill the process within the
  first second) - confirms the marker-before-copy ordering actually
  prevents a concurrent sweep from deleting a copy that's still being
  written. Deliberately not automated: inherently racy (needs killing a
  process within roughly the first second of a copy starting) - a flaky
  test that fails unpredictably regardless of real behavior is worse
  than no test here.
- [x] **O.3. (AUTOMATED - Run-CrashRecoveryTests.ps1)** File path
  containing spaces and non-ASCII characters, for both the real file and
  the resulting working-copy path.
- [ ] **O.5.** Run the multi-instance test (§1.2) on a locked-down,
  IT-managed machine with active antivirus/on-access scanning -
  confirms the rapid create/delete file patterns involved aren't
  disrupted by AV interference in a way a dev machine wouldn't reveal.
  Skip if no such machine is readily available. Not automatable from
  here - needs a genuinely different, IT-managed machine.
- [ ] **O.6.** Confirm behavior when `%LocalAppData%` itself is unusually
  restricted or redirected (e.g. roaming profile, group policy) - the
  working-copy folder should still resolve somewhere writable and local.
  Skip if no such environment is readily available. Not automatable from
  here - needs a genuinely different environment.

(Large-file copy-on-open timing was dropped from this list - acceptable
unless/until stakeholders raise it as an actual concern.)

## Appendix: Useful test commands

**Hold a file open exclusively, to simulate another process/tool locking
it** (a database tool left open, OneDrive mid-sync, antivirus scanning
it, etc.) - referenced from §2.3 above:

```powershell
$path = "C:\path\to\file"
$fs = [System.IO.File]::Open($path, 'Open', 'ReadWrite', 'None')
Write-Host "Lock held on $path - press Enter to release..."
Read-Host
$fs.Close()
```

Run this, leave it sitting at the prompt, then trigger the operation
under test (Save, or an open) from the actual app. Press Enter to
release the lock once you've observed the expected behavior.

**Standalone repro confirming the exact HResult the retry logic checks
for** - useful if the retry/dialog mechanism doesn't seem to be firing
correctly, to rule out a mismatched `HResult` filter as the cause:

```powershell
$path = "C:\path\to\test.tmp"
[System.IO.File]::WriteAllText($path, "test")
$fs = [System.IO.File]::Open($path, 'Open', 'ReadWrite', 'None')

try {
    [System.IO.File]::Copy($path, "$path.copy", $true)
} catch {
    Write-Host "HResult: $($_.Exception.HResult)"
}

$fs.Close()
```

Expected output: `-2147024864` (`ERROR_SHARING_VIOLATION`). If a
different value comes back, `cEwEDatabase.PublishLocalCopyToSource`'s
`Catch ex As IOException When ex.HResult = ...` filter won't match it -
the exception falls through to the generic `Catch` instead, and fails
immediately without retrying at all.
