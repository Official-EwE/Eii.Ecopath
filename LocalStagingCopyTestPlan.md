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

## Core tests

### 1. Opening - staging creation

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
- [ ] **1.2.** Three scenarios in one pass - open the same file from a
  second instance while the first is still open, then check all three:
  - **Old/incompatible-version file:** confirm **no** working copy is
    created (staging is write-intent-only) and that the title bar
    *correctly does* show read-only - the one legitimate remaining case
    for it.
  - **Normal, current-version file, opened from a second instance while
    the first already has it open:** confirm the app **never** enters
    read-only mode or shows it in the title bar - the behavior the old,
    now-removed lock-degrade mechanism would have triggered here. Each
    instance should simply get its own independent working copy, with no
    error, dialog, or degrade of any kind (this also covers what used to
    be a separate "§3.2" multi-instance item - it's the same scenario).
  - *In passing:* confirm `%TEMP%\EwEReadOnlyCopies\` (the old
    mechanism's folder, before the pivot to `%LocalAppData%\Ecopath with
    Ecosim\DBs\`) is empty, stale, or doesn't exist - a one-time sanity
    check, not something to re-check every pass.
- [ ] **1.3.** **Any caller that never sets `EnableLocalStaging`** (e.g.
  `EwERunConsole`): confirm opening a database creates no working copy
  at all, and connects directly to the real file - staging is strictly
  opt-in, not on by default for every `EwECore.dll` consumer.

### 2. Saving - publish-to-source

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
- [ ] **2.2.** Save, then confirm the original file's bytes now match the
  working copy's, with no fingerprint check or conflict prompt of any
  kind - a blind overwrite is the intended behavior, not a bug to fix.
  Together with 2.1 above, this gives end-to-end confidence in the
  publish sequence without needing to step through its three internal
  lines (copy sidecar / delete / rename) individually - that level of
  detail is covered under "Optional / exotic" below if you want it.
- [ ] **2.3.** **Hold the original file open elsewhere, at two different
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

### 3. No multi-user coordination (by design)

- [x] **3.1.** No locking of any kind exists - a deliberate product
  decision (avoiding concurrent edits to the same file is entirely the
  user's own responsibility; last save wins with no detection or
  warning).
- [ ] **3.2.** Edit and save from **both** instances already open from
  §1.2 above - confirm whichever saves last silently overwrites the
  other's changes (no fingerprint check, no conflict warning) - this is
  the intended, accepted behavior, not a bug to chase.

### 4. Crash recovery

(Process killed, not a normal close - see §5 below for the graceful-close
path.)

- [ ] **4.1.** Kill a session's process directly (Task Manager /
  `taskkill`, not a normal close), then open **any** database (not
  necessarily the same one) on that machine afterward - confirm the
  orphaned working copy is swept away on that next write-intent open,
  not left behind indefinitely.
- [ ] **4.2.** Leave several crashed sessions' worth of orphaned copies in
  `%LocalAppData%\Ecopath with Ecosim\DBs\`, then open one more
  write-intent session - confirm the sweep clears all of them in that
  single pass, not just the most recent one.

### 5. Exit-time cleanup

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

### 6. SaveAs re-staging

- [ ] **6.1.** From a session, use Save As to a new path - confirm the old
  working copy is cleaned up and a fresh one is staged for the new
  target, and that a second instance can open the *original* file
  afterward with no restriction of any kind. While here, also try Save
  As to a path another session already has open as its own target -
  expect the same no-degrade behavior as §1.2/§3.2, just reached via
  Save As instead of Open.

### 7. Legacy database update chain safety

- [ ] **7.1.** **Confirm the original Access file's last-write time never
  changes while opening an old database that needs its legacy update
  chain (`RunAllUpdates`) to run.** With staging active, the update
  chain runs against the staged working copy - the original is read
  once (to make the copy) and never written to. Note the file's
  timestamp before opening, confirm it's unchanged after the updates
  have completed and the model is fully loaded.

### 8. Regression checks (unrelated fixes found during this work)

These touch core code paths unrelated to staging specifically, found and
fixed while chasing the connection-release investigation that led to the
exit-time cleanup mechanism above - included here because they landed in
the same files this backport touches. One ordinary usage session covers
all three:

- [ ] **8.1.** Open a model with Ecopath sample data (ratings, group
  samples, diet composition samples, catch samples) and confirm it all
  loads correctly (`LoadEcopathSamples`/`LoadGroupSamples`/
  `LoadDietSamples`/`LoadGroupCatchSamples` were all changed to properly
  close their reader); perform a few ordinary saves and confirm each
  commits correctly (`CommitTransaction`/`RollbackTransaction` now
  dispose the transaction object, and `cDBDataSource`'s `SaveXXX()`
  methods now correctly fold `ReleaseWriter`'s return value into the
  overall success flag); then deliberately trigger a failed save and
  confirm it actually rolls back and reports failure, rather than
  silently appearing to succeed.

## Optional / exotic (best-effort, skip if time or environment is short)

- [ ] **O.1.** Step through `PublishLocalCopyToSource`'s copy-sidecar-
  delete-rename sequence in the debugger, confirming the real file's
  state after each line (unchanged after the sidecar copy; missing, with
  `<realfile>.publishing` holding the full new content, after the
  delete; correctly updated after the rename). `File.Move`'s atomicity
  on the same volume is documented platform behavior, not something this
  code's own logic could get wrong - §2.1/§2.2 above already give solid
  end-to-end confidence without this.
- [ ] **O.2.** If reproducible: kill a session's process **mid-copy**
  (e.g. trigger open on a large file and kill the process within the
  first second) - confirms the marker-before-copy ordering actually
  prevents a concurrent sweep from deleting a copy that's still being
  written.
- [ ] **O.3.** File path containing spaces and non-ASCII characters, for
  both the real file and the resulting working-copy path.
- [ ] **O.4.** A **large** Access file (if any real-world models approach
  this) - confirm the copy-on-open time is acceptable and the UI doesn't
  appear to hang with no feedback during the copy.
- [ ] **O.5.** Run the multi-instance test (§1.2/§3.2) on a locked-down,
  IT-managed machine with active antivirus/on-access scanning - confirms
  the rapid create/delete file patterns involved aren't disrupted by AV
  interference in a way a dev machine wouldn't reveal. Skip if no such
  machine is readily available.
- [ ] **O.6.** Confirm behavior when `%LocalAppData%` itself is unusually
  restricted or redirected (e.g. roaming profile, group policy) - the
  working-copy folder should still resolve somewhere writable and local.
  Skip if no such environment is readily available.

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
