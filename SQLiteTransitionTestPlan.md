# SQLite Transition — Test Plan

## 1. Database creation & conversion

- [x] Create completely new SQLite database.
- Open existing Access database, to have it converted and opening up a SQLite database.
  - [ ] Access 2003 database 'Open model...'
  - [ ] Access 2003 database 'Open recent model...'
  - [ ] Access 2003 database using drag-and-drop
  - [ ] Access 2003 database by double clicking an Windows opening assocaited EwE
  - [x] Access 2007 database 'Open model...'  (.eweaccdb)
  - [x] Access 2007 database 'Open recent model...'  (.eweaccdb)
  - [x] Access 2007 database using drag-and-drop  (.eweaccdb) 
  - [x] Access 2007 database by double clicking an Windows opening assocaited EwE
- [x] Test re-opening an model
- Open an existing SQLite database directly (not via Access conversion). Only
  `.ewesqlite` is OS-associated with EwE - `.sqlite` stays openable for
  backward/generic compatibility (same relationship as `.eweaccdb` vs
  `.accdb`), so double-click is only meaningful for `.ewesqlite`.
  - [x] `.sqlite` file 'Open model...'
  - [x] `.sqlite` file 'Open recent model...'
  - [x] `.sqlite` file using drag-and-drop
  - [x] `.ewesqlite` file 'Open model...'
  - [x] `.ewesqlite` file 'Open recent model...'
  - [x] `.ewesqlite` file using drag-and-drop
  - [x] `.ewesqlite` file by double clicking a Windows-associated file
- [x] Once converted is always converted. Re-opening an Access database should open
  an existing converted SQLite database.
  (To test a re-conversion, remove the existing SQLite file)
  - [x] Confirm the produced/detected companion file uses `.ewesqlite`, not
    `.sqlite`, for both Access 2003 (`.mdb`/`.ewemdb`) and Access 2007
    (`.accdb`/`.eweaccdb`) sources.
- [ ] **Test opening an Access database with a *stale/partial* SQLite file
  present** (e.g. a previous conversion that crashed or was killed mid-write).
  Confirm whether this is silently treated as "already converted" (opening a
  broken file) or detected and re-converted - this determines whether
  `cDataSourceFactory.GetSupportedType` needs a validity check beyond a bare
  `File.Exists`.
- [ ] **Test conversion when the `mdb2sqlite` tool/folder is missing** from
  the output directory (e.g. incomplete deployment) - confirm a clear error is
  shown rather than a silent failure or crash.
- [ ] **Test conversion when the target `.ewesqlite` file is locked** by another
  process (e.g. left open in a SQLite browser) during a re-conversion attempt.
- [ ] **Test conversion of a large/complex real-world database** (many
  groups, fleets, time series, scenarios) - both for correctness and for
  reasonable conversion time.
- [ ] **Test conversion of a currently-open Access database** - confirm the
  model is fully closed (and the Access file unlocked) before `mdb2sqlite.exe`
  attempts to read it.
- [ ] **Verify the seeded `.sqlite` starter file (`EwE6.sqlite` embedded
  resource) has its `__EFMigrationsHistory` correctly pre-seeded** - creating a
  new database should not attempt to re-run the baseline migration's
  `CREATE TABLE` statements against a schema that's already there. (Note:
  `EwE6.sqlite` itself is expected to keep the plain `.sqlite` extension as
  an internal embedded resource, regardless of the `.ewesqlite` convention
  used for user-facing files - see the open question about `.sqlite` support
  in general before assuming this needs to change too.)

## 2. Saving

- [ ] Save opened database to SQLite database.
- [ ] Cannot save to Access format anymore.
- [ ] **Save the same model twice in a row without closing/reopening in
  between**, for every table that uses the delete-and-rewrite save pattern
  (`EcopathDietComp`/`EcopathCatch` and similar). This is a specific regression
  test for the stale-`ChangeTracker`-entity bug found during development -
  confirm no "same primary key"/`UNIQUE constraint` errors on the second save.
- [ ] **Save, close the app entirely, reopen, save again** - same intent as
  above but across a fresh `DbContext`/process lifetime rather than just
  repeated in-session saves.
- [ ] **Kill the app (or pull power) mid-save**, then reopen the database.
  Confirm the `.sqlite-wal`/`.sqlite-shm` (or `.ewesqlite-wal`/`.ewesqlite-shm`)
  files get correctly replayed/checkpointed on next open, and no data is
  silently lost or the file left unopenable.

## 3. Legacy database update chain

- [x] **Open a genuinely old Access database** (several versions behind,
  ideally from "Release 6.2 and before") and confirm the full legacy update
  chain (`RunAllUpdates`) still runs correctly end-to-end on the Access side,
  thesn converts to SQLite afterward at the final version.
- [x] **Confirm `RunAllUpdates` never attempts to run legacy update SQL
  against a SQLite-backed database** - i.e. `SupportsLegacyDatabaseUpdates()`
  correctly short-circuits for `cEwEEFDatabase`, and correctly substitutes the
  Access sub-database for `cEwEVersusDatabase` (bypassing the EF side).
  => the legacy update will show a dialog if the UpdateLog table is out-of-date but it won't do any migrations
  => EF migrations need to be make sure to also update the UpdateLog as I did with 6.702
- [ ] Do Access database updates using different regional settings, including
  `nl_NL`; read the `UpdateLog` table and check if the version numbers are
  correct.
  - Also check other tables/columns known to store numeric or
    date values written via raw SQL text rather than parameterized queries
    (search for remaining `String.Format(...VALUES...)` patterns), not just
    `UpdateLog.Version` - the fix applied to `SetVersion` doesn't cover every
    call site that could be affected by the same locale/thousands-separator
    issue.

## 4. Comparison / versus mode (Debug only)

- [ ] Test if you can open an Access and SQLite database in comparison mode. Open
  an Access database to use it (using `cEwEVersusDatabase` and
  `cEwEVersusDbWriter` classes) together with `Tools/ws-client.html`.
  - See if all comparisons are equal; if not, differences will show.
  - Test `cEwEVersusDbWriter` by making changes to the model; see if
    comparisons are still equal.
  - Go through all Ecopath screens, do updates, deletion, save the model.
- [ ] **Systematically diff every table**, not just the ones touched while
  clicking through the UI - e.g. a scripted pass that opens a versus-database
  and runs `GetReader("SELECT * FROM <table>")` for every known table, to catch
  a mismatch on a table nobody happened to touch manually during testing.
- [ ] **Test the post-commit diff specifically for tables with composite
  keys** (`EcopathCatch`, `EcopathDietComp`, etc.) - these are the tables most
  likely to reveal key-ordering or matching bugs versus single-key tables.
- [ ] Confirm comparison mode is only ever entered via an Access 2007
  (`.accdb`/`.eweaccdb`) source with an `.ewesqlite` companion - per the
  `HandleAccessToSqliteConversion` refactor, Access 2003 (`.mdb`/`.ewemdb`)
  sources should skip reconversion silently without ever offering this
  Debug prompt.

## 5. Full UI walkthrough

- [ ] Open a SQLite database and go through all Ecopath screens, do updates,
  deletions, save the model.
- [ ] **Test the EwEValueChain plugin against a SQLite-backed database.**
  This plugin's data model (`cFlowDiagram`, `cUnit`, `cParameters`, etc.) uses
  the `cOOPStorable`/OOP persistence subsystem, which relies on Access/OleDb-
  specific SQL (`ALTER TABLE ADD PRIMARY KEY/FOREIGN KEY`, and an
  `OleDbConnection`-typed method signature) that has not been made
  SQLite-compatible. Confirm the plugin fails gracefully (or is disabled) for
  SQLite-backed models, rather than throwing an unhandled exception or
  corrupting data.

## 6. Release / packaging

- [x] Are `Eii.Ecopath.Storage`, `EwE6.sqlite` up-to-date? Any new updates after
  `cDBUpdate6_70_19.vb`?
  => no need. EF migrations will update it automatically.
  see **Confirm `RunAllUpdates` never attempts to run legacy update SQL against a SQLite-backed database**
- [ ] Test if released NuGet `Eii.Ecopath.EwECore` package does not have source
  code embedded. (only the Debug version should have it ??)
- [ ] **Decide on and test debugging (step-into) support for released NuGet
  packages.** Confirming Release builds don't embed source (previous bullet)
  raises the flip side: how do consumers of `Eii.Ecopath.EwECore`/
  `Eii.Ecopath.Storage` step into that code while debugging their own
  projects? Options worth evaluating: publishing `.snupkg` symbol packages
  (with Source Link) to a symbol server, or additionally publishing
  debug-configuration NuGet packages to an authorized/private NuGet feed
  specifically for internal debugging use. Whichever approach is chosen,
  test that stepping into the package's code actually works from a
  consuming project.
- [ ] **Confirm `<DebugType>`/`<EmbedAllSources>` settings are scoped
  correctly per configuration** (Debug vs Release) in the shipped package -
  verify Release builds don't embed source into PDBs if that's not intended.
- [ ] **Test the x86 build specifically for the native SQLite binary.**
  The custom `CopyEsqlite3ToOutputRoot` MSBuild target keys off
  `$(PlatformTarget)`, which may not actually be set to `x86` the way the
  target's condition assumes - verify the correct (`win-x86` vs `win-x64`)
  `e_sqlite3.dll` actually lands in the x86 output folder, not just x64/AnyCPU.
- [ ] **Verify `mdb2sqlite.exe` is actually present in the shipped
  output** under the `mdb2sqlite\` subfolder (per the `.vbproj` copy-to-output
  rule), for both x86 and x64 release builds.
- [ ] **Check and fix TODOs, search for: "Todo: localize the message"** - several
  user-facing messages added during the SQLite migration (conversion
  failures, network-error prompts, etc.) were left as hardcoded English
  strings with this marker rather than going through the normal
  localization/resource mechanism.
- [ ] **Confirm the Inno Setup installer registers the `.ewesqlite` file
  association** (launching `ScientificInterface.exe` on double-click), and
  explicitly does *not* register a system-wide association for bare
  `.sqlite` - it should remain openable from within the app without becoming
  "the default app" for `.sqlite` files in general.

## 7. Cross-platform / cross-target

- [x] Test EwECore cross-platform, Linux (shell only) and Windows (from shell,
  from ScientificInterface).
  => tested Linux shell using MSP - net10 only
  => tested ScientificInterface from Windows x64 - net48 only
- [ ] **Test both net48 (EF6) and net10 (EF Core) builds explicitly for the
  SQLite path.** The EF-side code has real branching between these two targets
  (`Database.Initialize` vs `Database.Migrate`, `Database.Connection` vs
  `Database.GetDbConnection()`, `DbContextTransaction` vs
  `IDbContextTransaction`) - a bug specific to one TFM's EF flavor could easily
  pass testing done only on the other.
- [ ] **Run a full functional pass on x86, not just x64.** Development so far
  has mostly happened on x64 - this is broader than the x86 native-binary
  deployment check in §6 (which only confirms the right `e_sqlite3.dll`
  lands in the output folder). Actually create/open/save both Access and
  SQLite databases, run a conversion, and exercise the versus-database
  comparison mode on an x86 build, since a platform that's barely been run
  at all could have other, unrelated issues that packaging checks alone
  wouldn't surface.

## 8. Other feature areas bundled into this release

- [ ] Test all image generation paths (to be determined). Reason: we changed from
  System.Drawing to SkiaSharp in EwECore.
  ([commit](https://github.com/Official-EwE/Eii.Ecopath/commit/5d3dc2d3c08269a97f2ed13aa50f3da54595f269))
- [ ] Extended logging:
  - In eiixml
    ([commit](https://github.com/Official-EwE/Eii.Ecopath/commit/134710f7a00143ad3440204750e5ae11effdb0f8))
  - Add TraceErrorLogger to enhance logging with Trace support
    ([commit](https://github.com/Official-EwE/Eii.Ecopath/commit/2562445d84ab4323affe9a3f6413043dda0c2aef))
- [ ] Speed optimalisations
  ([commit](https://github.com/Official-EwE/Eii.Ecopath/commit/879c56ddee00d0a520cbf9aaed1b295e3ea943c9)).
- [ ] **Since this release bundles several independent changes (SQLite
  migration, SkiaSharp, logging, speed optimizations), test that these don't
  interact badly with each other** - e.g. confirm the new logging changes
  correctly capture SQLite-path errors (`cEwEEFDatabase`/`cEwEEFDbWriter`'s
  `ILogger` calls).

## 9. Not yet covered above - worth deciding whether in scope

- [ ] **Performance comparison, Access vs SQLite**, for typical and
  large models: load time, save time, and the versus-database's post-commit
  diff overhead specifically (since that runs an extra full comparison pass on
  every write while in comparison mode).
- [x] **Concurrent/multi-instance access.** Access's connection strings use
  `Mode=Share Exclusive`; SQLite instead uses an app-level companion lock
  file with automatic read-only degrade - see §11 for the full test list
  covering this mechanism specifically. (Kept here, checked, so this item's
  history/rationale link isn't lost - the actual tests live in §11.)
- [ ] **`Compact`/`SaveAs` behavior for SQLite.** `cEwEEFDatabase.Compact`
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

- [x] Generate the baseline migration reflecting the current model
  (`dotnet ef migrations add InitialBaseline`).
- [x] Seed `__EFMigrationsHistory` in the `EwE6.sqlite` starter resource file
  itself, marking `InitialBaseline` as already applied - so
  `Database.Migrate()` doesn't try to re-run `CREATE TABLE` against a schema
  that's already there. Read the exact `MigrationId`/`ProductVersion` values
  from the generated migration rather than hardcoding them, so they can't
  drift out of sync.
- Once the above exists:
  - [x] **Test the EF Core migration path with a dummy migration.** Add a
    trivial, no-op (or otherwise harmless) migration after the baseline, then
    verify that creating a new SQLite database correctly applies it and
    records it in `__EFMigrationsHistory`. This validates the full
    `dotnet ef migrations add` -> `Database.Migrate()` pipeline end-to-end
    without needing a real schema change to test against - i.e. confirms new
    databases don't just get the baseline schema but also pick up whatever
    ships after it.
  - [x] Re-run the `__EFMigrationsHistory`-related item in §1 (verifying
    `EwE6.sqlite` doesn't trigger a "table already exists" failure) now that
    there's an actual migration/history to check against.
  - [x] **Verify `Database.Initialize(True)` on the net48/EF6 side behaves
    correctly against a file the net10.0 migrator tool has already brought
    up to date.** Reading `net48\EwEDbContext.cs`, no `Database.SetInitializer`
    or Migrations configuration is visible for this context, so the default
    `CreateDatabaseIfNotExists` initializer should just see the file already
    exists and no-op - but this should be confirmed by actually running it,
    not assumed from reading the code alone, given the possibility of a
    `SetInitializer` call or `Seed()` override existing elsewhere in the
    codebase that hasn't been reviewed here.

## 11. Concurrent access / companion lock mechanism

Covers the app-level companion-lock feature that replaces Access's
`Mode=Share Exclusive` for SQLite: first opener gets read-write access;
every subsequent opener degrades to read-only automatically rather than
failing. See the design handoff for this feature for the rationale behind
each mechanism referenced below.

**Known, accepted limitation - not a bug if observed during testing**: a
read-only session's local copy in `%TEMP%\EwEReadOnlyCopies\` may remain
on disk, with its `.marker` already gone, for as long as the app keeps
running after that session closes - see "The local-copy connection never
releases" in `FILE_LOCK_HANDOFF.md` for the full investigation. This is
expected. It gets cleaned up either by the exit-time sweep on normal app
close (see below), or by `CleanUpOrphanedReadOnlyCopies()` on the next
read-only `Open()` anywhere on the machine (the crash-recovery path).
Don't file this as a defect if seen mid-session with the app still open.

**Basic exclusivity and degrade**

- [ ] Two instances, same machine, open the same `.ewesqlite`: first opens
  read-write, second opens read-only with no error and no dialog - only the
  title bar/status-bar read-only indicator changes.
- [ ] Confirm a `.ewesqlite.lock` file appears next to the data file while
  the writer session is open, and disappears when that session closes
  cleanly.
- [ ] Second (read-only) session: confirm edits are possible in the UI (Save
  button stays enabled by design) but pressing Save produces the "opened in
  read-only mode and cannot be saved" message and does not write anything.
- [ ] Second session: confirm **Save As** still works and produces a fully
  independent, fully writable new file.
- [ ] Confirm the new diagnostic message ("Model 'X' is currently open
  elsewhere - opened here from a local read-only copy") fires for the
  second session and names the correct model.
- [ ] Close the first (writer) session, then open a **third** instance -
  confirm it acquires write access (not read-only), proving the lock was
  actually released on a clean `Close()`, not just on process exit.
- [ ] Repeat the two-instance test across the **actual shared/network drive**
  intended for deployment, not just two instances on one PC - this is the
  one that exercises `FileShare.None` over SMB specifically, which is a
  materially different reliability question than same-machine locking.

**Crash recovery** (process killed, not a normal close - see "Exit-time
cleanup" below for the graceful-close path)

- [ ] Kill the writer's process directly (Task Manager / `taskkill`, not a
  normal close) - another instance should be able to acquire write access
  immediately afterward, with no leftover `.lock` blocking it.
- [ ] Kill a read-only session's process, then open anything read-only
  afterward on the same machine - confirm the orphaned copy is swept away
  on that next open via `CleanUpOrphanedReadOnlyCopies()`, not left behind
  indefinitely.
- [ ] If reproducible: kill a session's process **mid-copy** (e.g. trigger
  open on a large file and kill the process within the first second) -
  confirms the marker-before-copy ordering actually prevents a concurrent
  sweep from deleting a copy that's still being written.
- [ ] Leave several crashed sessions' worth of orphaned copies in
  `%TEMP%\EwEReadOnlyCopies\`, then open one more read-only session - confirm
  the sweep clears all of them in that single pass, not just the most recent
  one.

**Exit-time cleanup** (normal, graceful app close - a different mechanism
from crash recovery above)

- [ ] Open and close **several** read-only sessions in one app run (so
  each leaves its own marker-less copy behind), then close the app
  normally - confirm every one of those copies is gone from
  `%TEMP%\EwEReadOnlyCopies\` shortly after the app fully exits, in a
  single pass - not just the most recently closed one.
- [ ] **Regression test for a real bug found and fixed during
  development**: close a read-only model, then close the whole
  application - confirm the app closes promptly, with no perceptible
  hang or delay. An earlier draft of this mechanism used a synchronous,
  sleep-based retry on `Close()` that measurably delayed application
  shutdown; that was removed specifically because of this.
- [ ] Open the app, do **only** normal read-write work (no read-only
  session ever opened), then close it - confirm no cleanup script or
  process appears at all. The exit-time hook only registers the first
  time a session actually creates a local read-only copy, so a
  pure-writer session should incur zero extra activity on close.
- [ ] Kill the app (not a normal close) after leaving a read-only
  session's copy behind - confirm the exit-time sweep does **not** run
  (`AppDomain.ProcessExit` isn't reliably raised on a hard kill) - the
  file should instead sit until `CleanUpOrphanedReadOnlyCopies()` catches
  it on the next read-only `Open()`, per the crash-recovery tests above.

**Schema/migration correctness for the read-only copy**

- [ ] Open an older-but-still-supported-schema `.ewesqlite` file read-only
  (second session) while a current build is running - confirm the local
  copy is migrated to current schema and the session works normally, rather
  than the reader silently operating against a stale schema or throwing on
  a missing column.
- [ ] Run the above specifically on the **net48 build**, since this path now
  shells out to the net10.0 Migrator subprocess against the *local temp
  copy* rather than the original network path - confirm the subprocess
  handles a `%TEMP%\EwEReadOnlyCopies\<guid>.ewesqlite` argument correctly.

**SaveAs lock transfer**

- [ ] From a writer session, use Save As to a new path - confirm the
  original file's `.lock` is released and a new `.lock` appears next to the
  new file, and that a second instance opening the *original* file
  afterward can now acquire write access on it.
- [ ] From a writer session, Save As to a path where **another session
  already holds the lock** on that exact target path (e.g. someone already
  has that destination file open as writer) - confirm this session degrades
  to read-only on the new file rather than silently believing it still has
  exclusive access.

**Environment/edge cases**

- [ ] File path containing spaces and non-ASCII characters, for both the
  real file and the resulting temp copy path.
- [ ] Shared folder where the current user has **read-only permissions** (no
  write access at all) - first user to open should fail to create the
  `.lock` file gracefully and degrade straight to read-only, rather than
  crashing or showing an unrelated error.
- [ ] A **large** `.ewesqlite` file (if any real-world models approach this) -
  confirm the copy-on-open time is acceptable and the UI doesn't appear to
  hang with no feedback during the copy.
- [ ] Run the two-instance test on a locked-down, IT-managed machine with
  active antivirus/on-access scanning - confirms the rapid create/delete
  file patterns involved aren't disrupted by AV interference in a way a dev
  machine wouldn't reveal.
- [ ] Confirm behavior when `%TEMP%` itself is unusually restricted or
  redirected (e.g. roaming profile, group policy redirecting `TEMP` to a
  network path) - the read-only copy folder should still resolve somewhere
  writable and local.

**Comparison/versus mode**

- [ ] Open the same model in comparison mode (`cEwEVersusDatabase`) from a
  second instance while a first instance holds it - confirm this does not
  crash. Known limitation to confirm rather than be surprised by: the
  versus-database itself does not currently surface
  `IsLockedByAnotherSession`/read-only status even if its wrapped EF side
  degraded, so the status bar may not reflect this correctly in comparison
  mode specifically. Low priority given Access/comparison mode is being
  deprecated, but worth confirming the behavior rather than leaving it
  unknown.

**Regression checks for incidental fixes found during the leak
investigation** - these touch core code paths unrelated to locking
specifically, found and fixed while chasing the connection-release
mystery documented in `FILE_LOCK_HANDOFF.md`:

- [ ] Open a model with Ecopath sample data (ratings, group samples, diet
  composition samples, catch samples) and confirm all of it still loads
  correctly - `LoadEcopathSamples`/`LoadGroupSamples`/`LoadDietSamples`/
  `LoadGroupCatchSamples` were all changed to properly close their reader.
- [ ] Perform a save (or any other transaction-wrapped operation) and
  confirm it still commits correctly, and that a deliberately-failed
  save still rolls back correctly - `CommitTransaction`/
  `RollbackTransaction` now dispose the transaction object, in addition
  to committing/rolling it back.
