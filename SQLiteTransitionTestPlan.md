# SQLite Transition — Test Plan

Items from your original list are kept as-is. Additions/extensions from reviewing
the migration work are marked **[NEW]**, and existing items I expanded are marked
**[EXTENDED]** with the addition noted.

## 1. Database creation & conversion

- Create completely new SQLite database.
- Open existing Access database, either 2003 or 2007, to have it converted and
  opening up a SQLite database.
- Once converted is always converted. Re-opening an Access database should open
  an existing converted SQLite database.
  - Test a re-conversion by removing the existing SQLite file.
- **[NEW] Test opening an Access database with a *stale/partial* SQLite file
  present** (e.g. a previous conversion that crashed or was killed mid-write).
  Confirm whether this is silently treated as "already converted" (opening a
  broken file) or detected and re-converted - this determines whether
  `cDataSourceFactory.GetSupportedType` needs a validity check beyond a bare
  `File.Exists`.
- **[NEW] Test conversion when the `mdb2sqlite` tool/folder is missing** from
  the output directory (e.g. incomplete deployment) - confirm a clear error is
  shown rather than a silent failure or crash.
- **[NEW] Test conversion when the target `.sqlite` file is locked** by another
  process (e.g. left open in a SQLite browser) during a re-conversion attempt.
- **[NEW] Test conversion of a large/complex real-world database** (many
  groups, fleets, time series, scenarios) - both for correctness and for
  reasonable conversion time.
- **[NEW] Test conversion of a currently-open Access database** - confirm the
  model is fully closed (and the Access file unlocked) before `mdb2sqlite.exe`
  attempts to read it.
- **[NEW] Verify the seeded `.sqlite` starter file (`EwE6.sqlite` embedded
  resource) has its `__EFMigrationsHistory` correctly pre-seeded** - creating a
  new database should not attempt to re-run the baseline migration's
  `CREATE TABLE` statements against a schema that's already there.

## 2. Saving

- Save opened database to SQLite database.
- Cannot save to Access format anymore.
- **[NEW] Save the same model twice in a row without closing/reopening in
  between**, for every table that uses the delete-and-rewrite save pattern
  (`EcopathDietComp`/`EcopathCatch` and similar). This is a specific regression
  test for the stale-`ChangeTracker`-entity bug found during development -
  confirm no "same primary key"/`UNIQUE constraint` errors on the second save.
- **[NEW] Save, close the app entirely, reopen, save again** - same intent as
  above but across a fresh `DbContext`/process lifetime rather than just
  repeated in-session saves.
- **[NEW] Kill the app (or pull power) mid-save**, then reopen the database.
  Confirm the `.sqlite-wal`/`.sqlite-shm` files get correctly replayed/
  checkpointed on next open, and no data is silently lost or the file left
  unopenable.

## 3. Legacy database update chain

- **[NEW] Open a genuinely old Access database** (several versions behind,
  ideally from "Release 6.2 and before") and confirm the full legacy update
  chain (`RunAllUpdates`) still runs correctly end-to-end on the Access side,
  then converts to SQLite afterward at the final version.
- **[NEW] Confirm `RunAllUpdates` never attempts to run legacy update SQL
  against a SQLite-backed database** - i.e. `SupportsLegacyDatabaseUpdates()`
  correctly short-circuits for `cEwEEFDatabase`, and correctly substitutes the
  Access sub-database for `cEwEVersusDatabase` (bypassing the EF side).
- Do Access database updates using different regional settings, including
  `nl_NL`; read the `UpdateLog` table and check if the version numbers are
  correct.
  - **[EXTENDED]** Also check other tables/columns known to store numeric or
    date values written via raw SQL text rather than parameterized queries
    (search for remaining `String.Format(...VALUES...)` patterns), not just
    `UpdateLog.Version` - the fix applied to `SetVersion` doesn't cover every
    call site that could be affected by the same locale/thousands-separator
    issue.

## 4. Comparison / versus mode (Debug only)

- Test if you can open an Access and SQLite database in comparison mode. Open
  an Access database to use it (using `cEwEVersusDatabase` and
  `cEwEVersusDbWriter` classes) together with `Tools/ws-client.html`.
  - See if all comparisons are equal; if not, differences will show.
  - Test `cEwEVersusDbWriter` by making changes to the model; see if
    comparisons are still equal.
  - Go through all Ecopath screens, do updates, deletion, save the model.
- **[NEW] Systematically diff every table**, not just the ones touched while
  clicking through the UI - e.g. a scripted pass that opens a versus-database
  and runs `GetReader("SELECT * FROM <table>")` for every known table, to catch
  a mismatch on a table nobody happened to touch manually during testing.
- **[NEW] Test the post-commit diff specifically for tables with composite
  keys** (`EcopathCatch`, `EcopathDietComp`, etc.) - these are the tables most
  likely to reveal key-ordering or matching bugs versus single-key tables.

## 5. Full UI walkthrough

- Open a SQLite database and go through all Ecopath screens, do updates,
  deletions, save the model.
- **[NEW] Test the EwEValueChain plugin against a SQLite-backed database.**
  This plugin's data model (`cFlowDiagram`, `cUnit`, `cParameters`, etc.) uses
  the `cOOPStorable`/OOP persistence subsystem, which relies on Access/OleDb-
  specific SQL (`ALTER TABLE ADD PRIMARY KEY/FOREIGN KEY`, and an
  `OleDbConnection`-typed method signature) that has not been made
  SQLite-compatible. Confirm the plugin fails gracefully (or is disabled) for
  SQLite-backed models, rather than throwing an unhandled exception or
  corrupting data.

## 6. Release / packaging

- Are `Eii.Ecopath.Storage`, `EwE6.sqlite` up-to-date? Any new updates after
  `cDBUpdate6_70_19.vb`?
- Test if released NuGet `Eii.Ecopath.EwECore` package does not have source
  code embedded.
- **[NEW] Confirm `<DebugType>`/`<EmbedAllSources>` settings are scoped
  correctly per configuration** (Debug vs Release) in the shipped package -
  verify Release builds don't embed source into PDBs if that's not intended.
- **[NEW] Test the x86 build specifically for the native SQLite binary.**
  The custom `CopyEsqlite3ToOutputRoot` MSBuild target keys off
  `$(PlatformTarget)`, which may not actually be set to `x86` the way the
  target's condition assumes - verify the correct (`win-x86` vs `win-x64`)
  `e_sqlite3.dll` actually lands in the x86 output folder, not just x64/AnyCPU.
- **[NEW] Verify `mdb2sqlite.exe`/`.ps1` are actually present in the shipped
  output** under the `mdb2sqlite\` subfolder (per the `.vbproj` copy-to-output
  rule), for both x86 and x64 release builds.

## 7. Cross-platform / cross-target

- Test EwECore cross-platform, Linux (shell only) and Windows (from shell,
  from ScientificInterface).
- **[NEW] Test both net48 (EF6) and net10 (EF Core) builds explicitly for the
  SQLite path.** The EF-side code has real branching between these two targets
  (`Database.Initialize` vs `Database.Migrate`, `Database.Connection` vs
  `Database.GetDbConnection()`, `DbContextTransaction` vs
  `IDbContextTransaction`) - a bug specific to one TFM's EF flavor could easily
  pass testing done only on the other.

## 8. Other feature areas bundled into this release

- Test all image generation paths (to be determined). Reason: we changed from
  System.Drawing to SkiaSharp in EwECore.
  ([commit](https://github.com/Official-EwE/Eii.Ecopath/commit/5d3dc2d3c08269a97f2ed13aa50f3da54595f269))
- Extended logging:
  - In eiixml
    ([commit](https://github.com/Official-EwE/Eii.Ecopath/commit/134710f7a00143ad3440204750e5ae11effdb0f8))
  - Add TraceErrorLogger to enhance logging with Trace support
    ([commit](https://github.com/Official-EwE/Eii.Ecopath/commit/2562445d84ab4323affe9a3f6413043dda0c2aef))
- Speed optimalisations
  ([commit](https://github.com/Official-EwE/Eii.Ecopath/commit/879c56ddee00d0a520cbf9aaed1b295e3ea943c9)).
- **[NEW] Since this release bundles several independent changes (SQLite
  migration, SkiaSharp, logging, speed optimizations), test that these don't
  interact badly with each other** - e.g. confirm the new logging changes
  correctly capture SQLite-path errors (`cEwEEFDatabase`/`cEwEEFDbWriter`'s
  `ILogger` calls).

## 9. Not yet covered above - worth deciding whether in scope

- **[NEW] Performance comparison, Access vs SQLite**, for typical and
  large models: load time, save time, and the versus-database's post-commit
  diff overhead specifically (since that runs an extra full comparison pass on
  every write while in comparison mode).
- **[NEW] Concurrent/multi-instance access.** Access's connection strings use
  `Mode=Share Exclusive`; confirm what the equivalent expectation is for
  SQLite (WAL mode allows multiple readers + one writer) and whether two app
  instances opening the same `.sqlite` file behave as intended (blocked,
  allowed, or undefined).
- **[NEW] `Compact`/`SaveAs` behavior for SQLite.** `cEwEEFDatabase.Compact`
  currently returns `Failed_DeprecatedOperation` and `CanCompact` returns
  `False`. Confirm the UI correctly hides/disables the "Compact" option for
  SQLite-backed databases rather than presenting an action that always fails.

## 10. Post-migration-setup (groundwork, not yet done)

EF Core migrations haven't actually been set up yet - these are the
prerequisites that need to exist before the migration-path tests below (or
the `__EFMigrationsHistory`-related item in §1) can be meaningfully run.
There are no already-converted user `.sqlite` files to worry about yet - this
only needs to be right in `EwE6.sqlite` before any release that lets a user
create or convert to a SQLite file for the first time.

- Generate the baseline migration reflecting the current model
  (`dotnet ef migrations add InitialBaseline`).
- Seed `__EFMigrationsHistory` in the `EwE6.sqlite` starter resource file
  itself, marking `InitialBaseline` as already applied - so
  `Database.Migrate()` doesn't try to re-run `CREATE TABLE` against a schema
  that's already there. Read the exact `MigrationId`/`ProductVersion` values
  from the generated migration rather than hardcoding them, so they can't
  drift out of sync.
- Once the above exists:
  - **Test the EF Core migration path with a dummy migration.** Add a
    trivial, no-op (or otherwise harmless) migration after the baseline, then
    verify that creating a new SQLite database correctly applies it and
    records it in `__EFMigrationsHistory`. This validates the full
    `dotnet ef migrations add` -> `Database.Migrate()` pipeline end-to-end
    without needing a real schema change to test against - i.e. confirms new
    databases don't just get the baseline schema but also pick up whatever
    ships after it.
  - Re-run the `__EFMigrationsHistory`-related item in §1 (verifying
    `EwE6.sqlite` doesn't trigger a "table already exists" failure) now that
    there's an actual migration/history to check against.
