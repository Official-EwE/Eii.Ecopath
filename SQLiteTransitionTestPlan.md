# SQLite Transition — Test Plan

## 1. Database creation & conversion

- [x] **1.1.** Create completely new SQLite database.
- Open existing Access database, to have it converted and opening up a SQLite database.
  - [ ] **1.2.** Access 2003 database 'Open model...'
  - [ ] **1.3.** Access 2003 database 'Open recent model...'
  - [ ] **1.4.** Access 2003 database using drag-and-drop
  - [ ] **1.5.** Access 2003 database by double clicking an Windows opening assocaited EwE
  - [x] **1.6.** Access 2007 database 'Open model...'  (.eweaccdb)
  - [x] **1.7.** Access 2007 database 'Open recent model...'  (.eweaccdb)
  - [x] **1.8.** Access 2007 database using drag-and-drop  (.eweaccdb) 
  - [x] **1.9.** Access 2007 database by double clicking an Windows opening assocaited EwE
- [x] **1.10.** Test re-opening an model
- Open an existing SQLite database directly (not via Access conversion). Only
  `.ewesqlite` is OS-associated with EwE - `.sqlite` stays openable for
  backward/generic compatibility (same relationship as `.eweaccdb` vs
  `.accdb`), so double-click is only meaningful for `.ewesqlite`.
  - [x] **1.11.** `.sqlite` file 'Open model...'
  - [x] **1.12.** `.sqlite` file 'Open recent model...'
  - [x] **1.13.** `.sqlite` file using drag-and-drop
  - [x] **1.14.** `.ewesqlite` file 'Open model...'
  - [x] **1.15.** `.ewesqlite` file 'Open recent model...'
  - [x] **1.16.** `.ewesqlite` file using drag-and-drop
  - [x] **1.17.** `.ewesqlite` file by double clicking a Windows-associated file
- [x] **1.18.** Once converted is always converted. Re-opening an Access database should open
  an existing converted SQLite database.
  (To test a re-conversion, remove the existing SQLite file)
  - [x] **1.18a.** Confirm the produced/detected companion file uses `.ewesqlite`, not
    `.sqlite`, for both Access 2003 (`.mdb`/`.ewemdb`) and Access 2007
    (`.accdb`/`.eweaccdb`) sources.
- [ ] **1.19.** **Test opening an Access database with a *stale/partial* SQLite file
  present** (e.g. a previous conversion that crashed or was killed mid-write).
  Confirm whether this is silently treated as "already converted" (opening a
  broken file) or detected and re-converted - this determines whether
  `cDataSourceFactory.GetSupportedType` needs a validity check beyond a bare
  `File.Exists`.
- [ ] **1.20.** **Test conversion when the `mdb2sqlite` tool/folder is missing** from
  the output directory (e.g. incomplete deployment) - confirm a clear error is
  shown rather than a silent failure or crash.
- [ ] **1.21.** **Test conversion when the target `.ewesqlite` file is held open** by
  another process (e.g. left open in a SQLite browser) during a
  re-conversion attempt. There is no app-level lock protecting this
  anymore - confirm what actually happens: does `mdb2sqlite.exe`'s write
  fail with a clear, surfaced error, or something more confusing? (Unlike
  `cEwEDatabase.PublishLocalCopyToSource`, this specific write has no
  retry-then-dialog mechanism of its own.)
- [ ] **1.22.** **Test conversion of a large/complex real-world database** (many
  groups, fleets, time series, scenarios) - both for correctness and for
  reasonable conversion time.
- [ ] **1.23.** **Test conversion of a currently-open Access database** - confirm the
  model is fully closed (releasing its OleDb connection) before
  `mdb2sqlite.exe` attempts to read it. Access no longer opens in exclusive
  mode, so this is about the connection genuinely being released, not
  about any lock being freed.
- [ ] **1.24.** **Open the same Access file for conversion from two instances at the
  same time** - confirm this is no longer prevented in any way (no
  companion lock, no exclusive Access mode): each instance stages its own
  independent local copy and runs its own conversion. Confirm the
  migration-staging marker (`cEwEExclusivityLock`, on the staged copy
  itself, not the original) correctly prevents the orphan sweep from
  deleting either instance's copy mid-migration, even though an unrelated
  database open elsewhere on the same machine can trigger that sweep at
  any moment.
- [ ] **1.25.** **Verify the seeded `.sqlite` starter file (`EwE6.sqlite` embedded
  resource) has its `__EFMigrationsHistory` correctly pre-seeded** - creating a
  new database should not attempt to re-run the baseline migration's
  `CREATE TABLE` statements against a schema that's already there. (Note:
  `EwE6.sqlite` itself is expected to keep the plain `.sqlite` extension as
  an internal embedded resource, regardless of the `.ewesqlite` convention
  used for user-facing files - see the open question about `.sqlite` support
  in general before assuming this needs to change too.)

## 2. Saving

- [x] **2.1.** Save opened database to SQLite database.
- [ ] **2.2.** Cannot save to Access format anymore.
- [ ] **2.3.** **Save the same model twice in a row without closing/reopening in
  between**, for every table that uses the delete-and-rewrite save pattern
  (`EcopathDietComp`/`EcopathCatch` and similar). This is a specific regression
  test for the stale-`ChangeTracker`-entity bug found during development -
  confirm no "same primary key"/`UNIQUE constraint` errors on the second save.
- [ ] **2.4.** **Save, close the app entirely, reopen, save again** - same intent as
  above but across a fresh `DbContext`/process lifetime rather than just
  repeated in-session saves.
- [x] **2.5.** **Kill the app (or pull power) mid-save, before the publish-back
  step** (i.e. `SaveChanges()` is still writing to the local working
  copy). Confirm the *original* file is untouched (it hasn't been touched
  yet at this point) and reopening produces a fresh, correct working copy
  - note: journal mode is `DELETE`, not `WAL`, so there are no
  `-wal`/`-shm` sidecar files to check for.
  - **Use a breakpoint, not timing/guesswork** - this is deterministic,
    not reaction-speed-dependent. In `cCore.Save()`, set a breakpoint on
    `If bSucces Then` (the line immediately after the `SaveChanges(True)`
    call(s), right before the publish block begins). When it hits,
    `SaveChanges()` has definitely fully returned and
    `PublishToSource()` definitely hasn't started yet - no ambiguity
    about which side of the boundary you're on. From a separate
    terminal, kill the process while stopped there:
    `Stop-Process -Name "ScientificInterface" -Force` (or
    `-Id <pid>` if more than one instance is running).
- [x] **2.6.** **Step through the copy-sidecar-delete-rename sequence in
  `PublishLocalCopyToSource`**, confirming the real file's state is
  correct after each of the three lines: still unchanged after the
  sidecar copy, missing (with `<realfile>.publishing` holding the
  complete new content) after the delete, correctly updated after the
  rename. No process-kill needed - a step-through is sufficient to
  confirm each intermediate state directly, and `File.Move`'s atomicity
  on the same volume is documented platform behavior, not something
  this code's own logic could get wrong.

**Local staging / publish-to-source (OneDrive save-safety)**

- [x] **2.7.** Confirm a `Temp<name>.<GUID><real-extension>` working copy
  (e.g. `.ewesqlite`/`.eweaccdb` - never `.tmp`, see §1's extension fix)
  appears under `%LocalAppData%\Ecopath with Ecosim\DBs\` on opening a
  database for read-write in `ScientificInterface`
  (`EnableLocalStaging = True` there). This confirms a copy gets
  *created*, not that the live session is actually *using* it - a bug
  could create the copy correctly and still connect to the original
  somewhere else, so pair this with one of the following:
  - **Primary check - rename the original while the model is open,
    before saving.** Doesn't depend on guessing when edits actually hit
    disk (they likely don't until Save, regardless of whether staging is
    working correctly - so comparing last-write times proves nothing
    either way, which is why that approach was dropped). If staging is
    genuinely in effect, this should have **zero effect** on the open
    session - the live connection never touches the original at all
    until Save, and `File.Copy(..., overwrite:=True)` will happily
    recreate the file at that path when Save eventually runs, since only
    the file was gone, not its parent folder. If the connection were
    (incorrectly) against the original, this would cause an immediate,
    obvious error.
- [ ] **2.8.** Two distinct scenarios, easy to conflate - keep them separate:
  - **Old/incompatible-version file:** confirm **no** working copy is
    created (staging is write-intent-only) and that the title bar
    *correctly does* show read-only - this is the one legitimate
    remaining case for it.
  - **Normal, current-version file:** confirm the app **never** enters
    read-only mode or shows it in the title bar, under any
    circumstances - including a scenario that *would* have triggered
    the old, now-removed lock-degrade behavior (e.g. the same file
    already open elsewhere - see §9.2/§11's "no coordination between
    instances"). This confirms the old behavior is actually gone, not
    just unreachable through the one path we happened to test.
  - While here: confirm `%TEMP%\EwEReadOnlyCopies\` (the old mechanism's
    folder, before the pivot to `%LocalAppData%\Ecopath with
    Ecosim\DBs\`) is empty, stale, or doesn't exist at all - no lingering
    code path should still be writing there.
- [x] **2.9.** Save, then confirm the original file's bytes now match the working
  copy's, with no fingerprint check or conflict prompt of any kind - a
  blind overwrite is the intended behavior, not a bug to fix.
- [x] **2.10.** **Hold the original file open elsewhere, at two different
  durations**, using the same script both times (just change
  `-Seconds`):
  ```powershell
  $path = "D:\path\to\your\model.eweaccdb"  # or .ewesqlite - the REAL path, not the staged copy
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
    UI block. Not directly proven by the long-hold test below - that one
    exhausts the internal retry entirely rather than succeeding partway
    through it - but accepted as adequate coverage, since it's the exact
    same retry loop either way, just a different iteration count.
  - **Long hold (~5s+, past the window):** confirm the "cannot write
    changes, locked by X" dialog appears, with Retry and Cancel.
    - [x] **2.10a.** Release the lock, click **Retry** - confirm the save
      completes and the dialog closes.
    - [x] **2.10b.** Click **Cancel** instead - confirm `Save()` returns
      `False` (nothing silently reports success), the app returns to
      normal use, and the in-memory model is unaffected (the user can
      try Save again later).
- [x] **2.12.** Confirm the dialog names the actual locking process when detectable
  (e.g. shows "Windows PowerShell" or similar for a `powershell.exe`
  holder via `cRestartManagerLookup`), and falls back to generic
  "another application" phrasing when detection finds nothing or fails
  for any reason - neither outcome should ever throw or block the
  retry/cancel flow itself.
- [ ] **2.13.** **`EwERunConsole` (or any caller that never sets
  `EnableLocalStaging`)**: confirm opening a database creates no working
  copy at all, and connects directly to the real file - staging is
  strictly opt-in, not on by default for every `EwECore.dll` consumer.

## 3. Legacy database update chain

- [x] **3.1.** **Open a genuinely old Access database** (several versions behind,
  ideally from "Release 6.2 and before") and confirm the full legacy update
  chain (`RunAllUpdates`) still runs correctly end-to-end on the Access side,
  thesn converts to SQLite afterward at the final version.
- [x] **3.2.** **Confirm `RunAllUpdates` never attempts to run legacy update SQL
  against a SQLite-backed database** - i.e. `SupportsLegacyDatabaseUpdates()`
  correctly short-circuits for `cEwEEFDatabase`, and correctly substitutes the
  Access sub-database for `cEwEVersusDatabase` (bypassing the EF side).
  => the legacy update will show a dialog if the UpdateLog table is out-of-date but it won't do any migrations
  => EF migrations need to be make sure to also update the UpdateLog as I did with 6.702
- [ ] **3.3.** **Confirm the original Access file's last-write time never changes
  during the whole open-migrate-convert flow.** `RunAllUpdates` now runs
  against a staged local copy (`frmEwE6.LoadEcopathModelHandlingConversion`),
  never the original - the original should be read exactly once, to make
  that copy, and never written to at all. A quick way to check: note the
  file's timestamp before opening, confirm it's unchanged after the
  `.ewesqlite` has been produced.
- [ ] **3.4.** Do Access database updates using different regional settings, including
  `nl_NL`; read the `UpdateLog` table and check if the version numbers are
  correct.
  - Also check other tables/columns known to store numeric or
    date values written via raw SQL text rather than parameterized queries
    (search for remaining `String.Format(...VALUES...)` patterns), not just
    `UpdateLog.Version` - the fix applied to `SetVersion` doesn't cover every
    call site that could be affected by the same locale/thousands-separator
    issue.

## 4. Comparison / versus mode (Debug only)

- [ ] **4.1.** Test if you can open an Access and SQLite database in comparison mode. Open
  an Access database to use it (using `cEwEVersusDatabase` and
  `cEwEVersusDbWriter` classes) together with `Tools/ws-client.html`.
  - See if all comparisons are equal; if not, differences will show.
  - Test `cEwEVersusDbWriter` by making changes to the model; see if
    comparisons are still equal.
  - Go through all Ecopath screens, do updates, deletion, save the model.
- [ ] **4.2.** **Systematically diff every table**, not just the ones touched while
  clicking through the UI - e.g. a scripted pass that opens a versus-database
  and runs `GetReader("SELECT * FROM <table>")` for every known table, to catch
  a mismatch on a table nobody happened to touch manually during testing.
- [ ] **4.3.** **Test the post-commit diff specifically for tables with composite
  keys** (`EcopathCatch`, `EcopathDietComp`, etc.) - these are the tables most
  likely to reveal key-ordering or matching bugs versus single-key tables.
- [ ] **4.4.** Confirm comparison mode is only ever entered via an Access 2007
  (`.accdb`/`.eweaccdb`) source with an `.ewesqlite` companion - per the
  `HandleAccessToSqliteConversion` refactor, Access 2003 (`.mdb`/`.ewemdb`)
  sources should skip reconversion silently without ever offering this
  Debug prompt.

## 5. Full UI walkthrough

- [ ] **5.1.** Open a SQLite database and go through all Ecopath screens, do updates,
  deletions, save the model.
- [ ] **5.2.** **Test the EwEValueChain plugin against a SQLite-backed database.**
  This plugin's data model (`cFlowDiagram`, `cUnit`, `cParameters`, etc.) uses
  the `cOOPStorable`/OOP persistence subsystem, which relies on Access/OleDb-
  specific SQL (`ALTER TABLE ADD PRIMARY KEY/FOREIGN KEY`, and an
  `OleDbConnection`-typed method signature) that has not been made
  SQLite-compatible. Confirm the plugin fails gracefully (or is disabled) for
  SQLite-backed models, rather than throwing an unhandled exception or
  corrupting data.

## 6. Release / packaging

- [x] **6.1.** Are `Eii.Ecopath.Storage`, `EwE6.sqlite` up-to-date? Any new updates after
  `cDBUpdate6_70_19.vb`?
  => no need. EF migrations will update it automatically.
  see **Confirm `RunAllUpdates` never attempts to run legacy update SQL against a SQLite-backed database**
- [ ] **6.2.** Test if released NuGet `Eii.Ecopath.EwECore` package does not have source
  code embedded. (only the Debug version should have it ??)
- [ ] **6.3.** **Decide on and test debugging (step-into) support for released NuGet
  packages.** Confirming Release builds don't embed source (previous bullet)
  raises the flip side: how do consumers of `Eii.Ecopath.EwECore`/
  `Eii.Ecopath.Storage` step into that code while debugging their own
  projects? Options worth evaluating: publishing `.snupkg` symbol packages
  (with Source Link) to a symbol server, or additionally publishing
  debug-configuration NuGet packages to an authorized/private NuGet feed
  specifically for internal debugging use. Whichever approach is chosen,
  test that stepping into the package's code actually works from a
  consuming project.
- [ ] **6.4.** **Confirm `<DebugType>`/`<EmbedAllSources>` settings are scoped
  correctly per configuration** (Debug vs Release) in the shipped package -
  verify Release builds don't embed source into PDBs if that's not intended.
- [ ] **6.5.** **Test the x86 build specifically for the native SQLite binary.**
  The custom `CopyEsqlite3ToOutputRoot` MSBuild target keys off
  `$(PlatformTarget)`, which may not actually be set to `x86` the way the
  target's condition assumes - verify the correct (`win-x86` vs `win-x64`)
  `e_sqlite3.dll` actually lands in the x86 output folder, not just x64/AnyCPU.
- [ ] **6.6.** **Verify `mdb2sqlite.exe` is actually present in the shipped
  output** under the `mdb2sqlite\` subfolder (per the `.vbproj` copy-to-output
  rule), for both x86 and x64 release builds.
- [ ] **6.7.** **Check and fix TODOs, search for: "Todo: localize the message"** - several
  user-facing messages added during the SQLite migration (conversion
  failures, network-error prompts, etc.) were left as hardcoded English
  strings with this marker rather than going through the normal
  localization/resource mechanism.
- [ ] **6.8.** **Confirm the Inno Setup installer registers the `.ewesqlite` file
  association** (launching `ScientificInterface.exe` on double-click), and
  explicitly does *not* register a system-wide association for bare
  `.sqlite` - it should remain openable from within the app without becoming
  "the default app" for `.sqlite` files in general.

## 7. Cross-platform / cross-target

- [x] **7.1.** Test EwECore cross-platform, Linux (shell only) and Windows (from shell,
  from ScientificInterface).
  => tested Linux shell using MSP - net10 only
  => tested ScientificInterface from Windows x64 - net48 only
- [ ] **7.2.** **Test both net48 (EF6) and net10 (EF Core) builds explicitly for the
  SQLite path.** The EF-side code has real branching between these two targets
  (`Database.Initialize` vs `Database.Migrate`, `Database.Connection` vs
  `Database.GetDbConnection()`, `DbContextTransaction` vs
  `IDbContextTransaction`) - a bug specific to one TFM's EF flavor could easily
  pass testing done only on the other.
- [ ] **7.3.** **Run a full functional pass on x86, not just x64.** Development so far
  has mostly happened on x64 - this is broader than the x86 native-binary
  deployment check in §6 (which only confirms the right `e_sqlite3.dll`
  lands in the output folder). Actually create/open/save both Access and
  SQLite databases, run a conversion, and exercise the versus-database
  comparison mode on an x86 build, since a platform that's barely been run
  at all could have other, unrelated issues that packaging checks alone
  wouldn't surface.

## 8. Other feature areas bundled into this release

- [ ] **8.1.** Test all image generation paths (to be determined). Reason: we changed from
  System.Drawing to SkiaSharp in EwECore.
  ([commit](https://github.com/Official-EwE/Eii.Ecopath/commit/5d3dc2d3c08269a97f2ed13aa50f3da54595f269))
- [ ] **8.2.** Extended logging:
  - In eiixml
    ([commit](https://github.com/Official-EwE/Eii.Ecopath/commit/134710f7a00143ad3440204750e5ae11effdb0f8))
  - Add TraceErrorLogger to enhance logging with Trace support
    ([commit](https://github.com/Official-EwE/Eii.Ecopath/commit/2562445d84ab4323affe9a3f6413043dda0c2aef))
- [ ] **8.3.** Speed optimalisations
  ([commit](https://github.com/Official-EwE/Eii.Ecopath/commit/879c56ddee00d0a520cbf9aaed1b295e3ea943c9)).
- [ ] **8.4.** **Since this release bundles several independent changes (SQLite
  migration, SkiaSharp, logging, speed optimizations), test that these don't
  interact badly with each other** - e.g. confirm the new logging changes
  correctly capture SQLite-path errors (`cEwEEFDatabase`/`cEwEEFDbWriter`'s
  `ILogger` calls).

## 9. Not yet covered above - worth deciding whether in scope

- [ ] **9.1.** **Performance comparison, Access vs SQLite**, for typical and
  large models: load time, save time, and the versus-database's post-commit
  diff overhead specifically (since that runs an extra full comparison pass on
  every write while in comparison mode).
- [x] **9.2.** **Concurrent/multi-instance access.** No locking of any kind exists,
  for either Access or SQLite - a deliberate product decision (avoiding
  concurrent edits to the same file is entirely the user's own
  responsibility; last save wins with no detection or warning). See §11
  for the full test list covering the local-staging-copy mechanism that
  *is* still in place (a different concern - protecting a single
  session's own save from cloud-sync interference, not multi-user
  conflicts).
- [ ] **9.3.** **`Compact`/`SaveAs` behavior for SQLite.** `cEwEEFDatabase.Compact`
  currently returns `Failed_DeprecatedOperation` and `CanCompact` returns
  `False`. Confirm the UI correctly hides/disables the "Compact" option for
  SQLite-backed databases rather than presenting an action that always fails.

## 10. Post-migration-setup (groundwork, not yet done)

EF Core migrations haven't actually been set up yet - these are the
prerequisites that need to exist before the migration-path tests below (or
the `__EFMigrationsHistory`-related item in §1) can be meaningfully run.
There are no already-converted user `.sqlite`/`.ewesqlite` files to worry
about yet - this only needs to be right in `EwE6.sqlite` before any release
that lets a user create or convert to a SQLite file for the first time.

- [x] **10.1.** Generate the baseline migration reflecting the current model
  (`dotnet ef migrations add InitialBaseline`).
- [x] **10.2.** Seed `__EFMigrationsHistory` in the `EwE6.sqlite` starter resource file
  itself, marking `InitialBaseline` as already applied - so
  `Database.Migrate()` doesn't try to re-run `CREATE TABLE` against a schema
  that's already there. Read the exact `MigrationId`/`ProductVersion` values
  from the generated migration rather than hardcoding them, so they can't
  drift out of sync.
- Once the above exists:
  - [x] **10.3.** **Test the EF Core migration path with a dummy migration.** Add a
    trivial, no-op (or otherwise harmless) migration after the baseline, then
    verify that creating a new SQLite database correctly applies it and
    records it in `__EFMigrationsHistory`. This validates the full
    `dotnet ef migrations add` -> `Database.Migrate()` pipeline end-to-end
    without needing a real schema change to test against - i.e. confirms new
    databases don't just get the baseline schema but also pick up whatever
    ships after it.
  - [x] **10.4.** Re-run the `__EFMigrationsHistory`-related item in §1 (verifying
    `EwE6.sqlite` doesn't trigger a "table already exists" failure) now that
    there's an actual migration/history to check against.
  - [x] **10.5.** **Verify `Database.Initialize(True)` on the net48/EF6 side behaves
    correctly against a file the net10.0 migrator tool has already brought
    up to date.** Reading `net48\EwEDbContext.cs`, no `Database.SetInitializer`
    or Migrations configuration is visible for this context, so the default
    `CreateDatabaseIfNotExists` initializer should just see the file already
    exists and no-op - but this should be confirmed by actually running it,
    not assumed from reading the code alone, given the possibility of a
    `SetInitializer` call or `Seed()` override existing elsewhere in the
    codebase that hasn't been reviewed here.

## 11. Local staging copy - lifecycle, cleanup, and multi-instance behavior

Covers `cEwEDatabase`'s local-working-copy mechanism (`StageLocalWorkingCopy`
/ `PublishLocalCopyToSource` / `CleanUpLocalWorkingCopy`) and its supporting
cleanup machinery. This is **not** a locking mechanism - there is no lock of
any kind protecting the original file, for either Access or SQLite, and no
multi-user/multi-session detection anywhere. See §2's "Local staging /
publish-to-source" tests for the save-path behavior (retry, dialog,
`cRestartManagerLookup`); this section covers the working copy's own
lifecycle instead - creation, cleanup, and what happens when nothing
coordinates between two independent sessions.

**Known, accepted limitation - not a bug if observed during testing**: a
session's local working copy in `%LocalAppData%\Ecopath with Ecosim\DBs\`
may remain on disk for as long as the app keeps running after that
session closes, even after `Close()` runs. The underlying connection does
not always release its file handle immediately, regardless of how
thoroughly `Close()` disposes things in-process. This is expected. It
gets cleaned up either by the exit-time sweep on normal app close (see
below), or by the orphan sweep on the next write-intent `Open()` anywhere
on the machine (the crash-recovery path). Don't file this as a defect if
seen mid-session with the app still open.

**No coordination between instances (by design)**

- [ ] **11.1.** Two instances, same machine, open the same `.ewesqlite` for
  read-write at the same time - confirm this now simply succeeds for
  both, with no error, no dialog, and no read-only degrade of any kind.
  Each gets its own independent working copy.
- [ ] **11.2.** Edit and save from **both** instances - confirm whichever saves
  last silently overwrites the other's changes (no fingerprint check, no
  conflict warning) - this is the intended, accepted behavior, not a bug
  to chase.
- [ ] **11.3.** Repeat for Access: open the same `.mdb`/`.accdb` from two instances
  at once - confirm both proceed independently (each stages its own copy
  for the migration step, per §1/§3), with no "already in use" error from
  either the app or from Access's own connection (no longer opened in
  exclusive mode).

**Crash recovery** (process killed, not a normal close - see "Exit-time
cleanup" below for the graceful-close path)

- [ ] **11.4.** Kill a session's process directly (Task Manager / `taskkill`, not a
  normal close), then open **any** database (not necessarily the same
  one) on that machine afterward - confirm the orphaned working copy is
  swept away on that next write-intent open, not left behind
  indefinitely.
- [ ] **11.5.** If reproducible: kill a session's process **mid-copy** (e.g. trigger
  open on a large file and kill the process within the first second) -
  confirms the marker-before-copy ordering actually prevents a concurrent
  sweep from deleting a copy that's still being written.
- [ ] **11.6.** Leave several crashed sessions' worth of orphaned copies in
  `%LocalAppData%\Ecopath with Ecosim\DBs\`, then open one more
  write-intent session - confirm the sweep clears all of them in that
  single pass, not just the most recent one.

**Exit-time cleanup** (normal, graceful app close - a different mechanism
from crash recovery above)

- [ ] **11.7.** Open and close **several** read-write sessions in one app run (so
  each potentially leaves its own marker-less copy behind, per the known
  limitation above), then close the app normally - confirm every one of
  those copies is gone from `%LocalAppData%\Ecopath with Ecosim\DBs\`
  shortly after the app fully exits, in a single pass.
- [ ] **11.8.** **Regression test for a real bug found and fixed during
  development**: close a model, then close the whole application -
  confirm the app closes promptly, with no perceptible hang or delay. An
  earlier draft of this mechanism used a synchronous, sleep-based retry
  on `Close()` that measurably delayed application shutdown; that was
  removed specifically because of this.
- [ ] **11.9.** Open the app with `EnableLocalStaging` off (or never open any
  database), then close it - confirm no cleanup script or process appears
  at all. The exit-time hook only registers the first time a session
  actually stages a working copy, so a session that never stages one
  should incur zero extra activity on close.
- [ ] **11.10.** Kill the app (not a normal close) after leaving a working copy
  behind - confirm the exit-time sweep does **not** run
  (`AppDomain.ProcessExit` isn't reliably raised on a hard kill) - the
  file should instead sit until the orphan sweep catches it on the next
  write-intent `Open()`, per the crash-recovery tests above.

**SaveAs re-staging**

- [ ] **11.11.** From a session, use Save As to a new path - confirm the old working
  copy is cleaned up and a fresh one is staged for the new target, and
  that a second instance can open the *original* file afterward with no
  restriction of any kind (nothing to release - there was never a lock).
- [ ] **11.12.** Save As to a path where **another session already has that exact
  target file open** - confirm this now simply succeeds for both,
  consistent with "no coordination between instances" above, rather than
  any kind of degrade or rejection.

**Environment/edge cases**

- [ ] **11.13.** File path containing spaces and non-ASCII characters, for both the
  real file and the resulting working-copy path.
- [ ] **11.14.** A **large** `.ewesqlite`/Access file (if any real-world models
  approach this) - confirm the copy-on-open time is acceptable and the UI
  doesn't appear to hang with no feedback during the copy.
- [ ] **11.15.** Run the multi-instance test on a locked-down, IT-managed machine
  with active antivirus/on-access scanning - confirms the rapid
  create/delete file patterns involved aren't disrupted by AV
  interference in a way a dev machine wouldn't reveal.
- [ ] **11.16.** Confirm behavior when `%LocalAppData%` itself is unusually
  restricted or redirected (e.g. roaming profile, group policy) - the
  working-copy folder should still resolve somewhere writable and local.

**Comparison/versus mode**

- [ ] **11.17.** Open the same model in comparison mode (`cEwEVersusDatabase`) from a
  second instance while a first instance holds it - confirm this does not
  crash, consistent with "no coordination between instances" above. Low
  priority given Access/comparison mode is being deprecated.
- [ ] **11.18.** Confirm `cEwEVersusDatabase`'s `PublishLocalCopyToSource` override
  correctly publishes **both** wrapped databases (Access and EF/SQLite)
  and that a failure on one side doesn't silently swallow a failure on
  the other.

**Regression checks for incidental fixes found during the leak
investigation** - these touch core code paths unrelated to staging or
locking specifically, found and fixed while chasing the connection-release
mystery that led to the exit-time cleanup mechanism above:

- [ ] **11.19.** Open a model with Ecopath sample data (ratings, group samples, diet
  composition samples, catch samples) and confirm all of it still loads
  correctly - `LoadEcopathSamples`/`LoadGroupSamples`/`LoadDietSamples`/
  `LoadGroupCatchSamples` were all changed to properly close their reader.
- [ ] **11.20.** Perform a save (or any other transaction-wrapped operation) and
  confirm it still commits correctly, and that a deliberately-failed
  save still rolls back correctly - `CommitTransaction`/
  `RollbackTransaction` now dispose the transaction object, in addition
  to committing/rolling it back.
