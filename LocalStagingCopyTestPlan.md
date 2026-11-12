# Local Staging Copy - Test Plan (master / Access-only)

Covers `cEwEDatabase`'s local-working-copy mechanism (`StageLocalWorkingCopy`
/ `PublishLocalCopyToSource` / `CleanUpLocalWorkingCopy`) and its supporting
cleanup machinery, as backported to the `master` branch - Access only, no
SQLite/EF. Every scenario here, except where noted, applies identically on
the `cEwEEFDatabase` branch, since this mechanism lives in `cEwEDatabase.vb`
itself and never was format-specific.

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

## 1. Opening - staging creation

- [x] **1.1.** Confirm a `Temp<name>.<GUID>.eweaccdb` working copy (never
  `.tmp` as the extension - `cDataSourceFactory.GetSupportedType`
  determines type purely from `Path.GetExtension`, so a `.tmp`-suffixed
  copy would be invisible to it) appears under `%LocalAppData%\Ecopath
  with Ecosim\DBs\` on opening a database for read-write
  (`EnableLocalStaging = True` in `ScientificInterface`). This confirms a
  copy gets *created*, not that the live session is actually *using* it -
  pair with the check below.
  - **Rename the original while the model is open, before saving.** If
    staging is genuinely in effect, this has **zero effect** on the open
    session - the live connection never touches the original at all
    until Save, and `File.Copy(..., overwrite:=True)` will happily
    recreate the file at that path when Save eventually runs, since only
    the file was gone, not its parent folder. If the connection were
    (incorrectly) against the original, this causes an immediate, obvious
    error instead.
- [ ] **1.2.** Two distinct scenarios, easy to conflate - keep them
  separate:
  - **Old/incompatible-version file:** confirm **no** working copy is
    created (staging is write-intent-only) and that the title bar
    *correctly does* show read-only - the one legitimate remaining case
    for it.
  - **Normal, current-version file:** confirm the app **never** enters
    read-only mode or shows it in the title bar, under any
    circumstances - including a scenario that *would* have triggered the
    old, now-removed lock-degrade behavior (the same file already open
    elsewhere - see §3 below). This confirms the old behavior is
    actually gone, not just unreachable through the one path tested.
  - While here: confirm `%TEMP%\EwEReadOnlyCopies\` (the old mechanism's
    folder, before the pivot to `%LocalAppData%\Ecopath with
    Ecosim\DBs\`) is empty, stale, or doesn't exist at all - no
    lingering code path should still be writing there.
- [ ] **1.3.** **Any caller that never sets `EnableLocalStaging`** (e.g.
  `EwERunConsole`): confirm opening a database creates no working copy
  at all, and connects directly to the real file - staging is strictly
  opt-in, not on by default for every `EwECore.dll` consumer.

## 2. Saving - publish-to-source

- [ ] **2.1.** **Kill the app (or pull power) mid-save, before the
  publish-back step** (i.e. the write to the local working copy is still
  in progress). Confirm the *original* file is untouched (it hasn't been
  touched yet at this point) and reopening produces a fresh, correct
  working copy.
  - **Use a breakpoint, not timing/guesswork** - this is deterministic,
    not reaction-speed-dependent. In `cCore.Save()`, set a breakpoint on
    `If bSucces Then` (the line immediately after the save call(s),
    right before the publish block begins). When it hits, the save has
    definitely fully returned and `PublishToSource()` definitely hasn't
    started yet. From a separate terminal, kill the process while
    stopped there: `Stop-Process -Name "ScientificInterface" -Force`
    (or `-Id <pid>` if more than one instance is running).
- [ ] **2.2.** **Step through the copy-sidecar-delete-rename sequence in
  `PublishLocalCopyToSource`**, confirming the real file's state is
  correct after each of the three lines: still unchanged after the
  sidecar copy, missing (with `<realfile>.publishing` holding the
  complete new content) after the delete, correctly updated after the
  rename. No process-kill needed - a step-through is sufficient to
  confirm each intermediate state directly, and `File.Move`'s atomicity
  on the same volume is documented platform behavior, not something this
  code's own logic could get wrong.
- [ ] **2.3.** Save, then confirm the original file's bytes now match the
  working copy's, with no fingerprint check or conflict prompt of any
  kind - a blind overwrite is the intended behavior, not a bug to fix.
- [ ] **2.4.** **Hold the original file open elsewhere, at two different
  durations**, using the same script both times (just change
  `-Seconds`):
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
    UI block.
  - **Long hold (~5s+, past the window):** confirm the "cannot write
    changes, locked by X" dialog appears, with Retry and Cancel.
    - [ ] **2.4a.** Release the lock, click **Retry** - confirm the save
      completes and the dialog closes.
    - [ ] **2.4b.** Click **Cancel** instead - confirm `Save()` returns
      `False` (nothing silently reports success), the app returns to
      normal use, and the in-memory model is unaffected (the user can
      try Save again later).
- [ ] **2.5.** Confirm the dialog names the actual locking process when
  detectable (e.g. shows "Windows PowerShell" or similar for a
  `powershell.exe` holder via `cRestartManagerLookup`), and falls back
  to generic "another application" phrasing when detection finds
  nothing or fails for any reason - neither outcome should ever throw
  or block the retry/cancel flow itself.

## 3. No multi-user coordination (by design)

- [x] **3.1.** No locking of any kind exists - a deliberate product
  decision (avoiding concurrent edits to the same file is entirely the
  user's own responsibility; last save wins with no detection or
  warning).
- [ ] **3.2.** Two instances, same machine, open the same `.eweaccdb` for
  read-write at the same time - confirm this simply succeeds for both,
  with no error, no dialog, and no read-only degrade of any kind. Each
  gets its own independent working copy.
- [ ] **3.3.** Edit and save from **both** instances - confirm whichever
  saves last silently overwrites the other's changes (no fingerprint
  check, no conflict warning) - this is the intended, accepted behavior,
  not a bug to chase.

## 4. Crash recovery

(Process killed, not a normal close - see §5 below for the graceful-close
path.)

- [ ] **4.1.** Kill a session's process directly (Task Manager /
  `taskkill`, not a normal close), then open **any** database (not
  necessarily the same one) on that machine afterward - confirm the
  orphaned working copy is swept away on that next write-intent open,
  not left behind indefinitely.
- [ ] **4.2.** If reproducible: kill a session's process **mid-copy**
  (e.g. trigger open on a large file and kill the process within the
  first second) - confirms the marker-before-copy ordering actually
  prevents a concurrent sweep from deleting a copy that's still being
  written.
- [ ] **4.3.** Leave several crashed sessions' worth of orphaned copies in
  `%LocalAppData%\Ecopath with Ecosim\DBs\`, then open one more
  write-intent session - confirm the sweep clears all of them in that
  single pass, not just the most recent one.

## 5. Exit-time cleanup

(Normal, graceful app close - a different mechanism from crash recovery
above.)

- [ ] **5.1.** Open and close **several** read-write sessions in one app
  run (so each potentially leaves its own marker-less copy behind, per
  the known limitation at the top of this document), then close the app
  normally - confirm every one of those copies is gone from
  `%LocalAppData%\Ecopath with Ecosim\DBs\` shortly after the app fully
  exits, in a single pass.
- [ ] **5.2.** **Regression test for a real bug found and fixed during
  development**: close a model, then close the whole application -
  confirm the app closes promptly, with no perceptible hang or delay.
  An earlier draft of this mechanism used a synchronous, sleep-based
  retry on `Close()` that measurably delayed application shutdown; that
  was removed specifically because of this.
- [ ] **5.3.** Open the app with `EnableLocalStaging` off (or never open
  any database), then close it - confirm no cleanup script or process
  appears at all. The exit-time hook only registers the first time a
  session actually stages a working copy.
- [ ] **5.4.** Kill the app (not a normal close) after leaving a working
  copy behind - confirm the exit-time sweep does **not** run
  (`AppDomain.ProcessExit` isn't reliably raised on a hard kill) - the
  file should instead sit until the orphan sweep catches it on the next
  write-intent `Open()`, per §4 above.

## 6. SaveAs re-staging

- [ ] **6.1.** From a session, use Save As to a new path - confirm the old
  working copy is cleaned up and a fresh one is staged for the new
  target, and that a second instance can open the *original* file
  afterward with no restriction of any kind.
- [ ] **6.2.** Save As to a path where **another session already has that
  exact target file open** - confirm this simply succeeds for both,
  consistent with §3 above, rather than any kind of degrade or
  rejection.

## 7. Environment / edge cases

- [ ] **7.1.** File path containing spaces and non-ASCII characters, for
  both the real file and the resulting working-copy path.
- [ ] **7.2.** A **large** Access file (if any real-world models approach
  this) - confirm the copy-on-open time is acceptable and the UI doesn't
  appear to hang with no feedback during the copy.
- [ ] **7.3.** Run the multi-instance test (§3.2) on a locked-down,
  IT-managed machine with active antivirus/on-access scanning -
  confirms the rapid create/delete file patterns involved aren't
  disrupted by AV interference in a way a dev machine wouldn't reveal.
- [ ] **7.4.** Confirm behavior when `%LocalAppData%` itself is unusually
  restricted or redirected (e.g. roaming profile, group policy) - the
  working-copy folder should still resolve somewhere writable and local.

## 8. Legacy database update chain safety

- [ ] **8.1.** **Confirm the original Access file's last-write time never
  changes while opening an old database that needs its legacy update
  chain (`RunAllUpdates`) to run.** With staging active, the update
  chain runs against the staged working copy - the original is read
  once (to make the copy) and never written to. Note the file's
  timestamp before opening, confirm it's unchanged after the updates
  have completed and the model is fully loaded.

## 9. Regression checks (unrelated fixes found during this work)

These touch core code paths unrelated to staging specifically, found and
fixed while chasing the connection-release investigation that led to the
exit-time cleanup mechanism above - included here because they landed in
the same files this backport touches.

- [ ] **9.1.** Open a model with Ecopath sample data (ratings, group
  samples, diet composition samples, catch samples) and confirm all of
  it still loads correctly - `LoadEcopathSamples`/`LoadGroupSamples`/
  `LoadDietSamples`/`LoadGroupCatchSamples` were all changed to properly
  close their reader.
- [ ] **9.2.** Perform a save (or any other transaction-wrapped
  operation) and confirm it still commits correctly, and that a
  deliberately-failed save still rolls back correctly -
  `CommitTransaction`/`RollbackTransaction` now dispose the transaction
  object, in addition to committing/rolling it back.
- [ ] **9.3.** Perform several saves in a row and confirm each one still
  works correctly - `cDBDataSource`'s various `SaveXXX()` methods now
  correctly fold `ReleaseWriter`'s own return value into the overall
  success flag, rather than discarding it; a failed write/commit should
  now actually be reported as a failed save, not silently appear to
  succeed.

## Appendix: Useful test commands

**Hold a file open exclusively, to simulate another process/tool locking
it** (a database tool left open, OneDrive mid-sync, antivirus scanning
it, etc.) - referenced from §2.4 above:

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
