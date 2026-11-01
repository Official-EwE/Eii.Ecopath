# Local Staging Copy - master branch backport

This contains the local-staging-copy (OneDrive/cloud-sync save-safety)
feature, backported from the `cEwEEFDatabase` branch to `master` - Access
only, no SQLite/EF.

## How this was built

Both branches were cloned directly from
`https://github.com/Official-EwE/Eii.Ecopath` (sparse-checked-out to just
`Sources/EwECore` and `Sources/ScientificInterface`), diffed file-by-file
after normalizing line endings (master's `cEwEDatabase.vb` uses bare `LF`;
everything else uses `CRLF` - a pure line-ending difference that otherwise
makes every line look changed), and the staging-specific hunks applied
directly to copies of master's own current files - never a blind overwrite
of either branch's version.

**This has not been compiled.** Every edit was verified by locating its
exact anchor text in master's real file first, but there's no substitute
for an actual build. Expect to fix a handful of small issues, the same as
every other round of changes in this project.

## `Sources/` - apply these over your master checkout

Each file here is master's own file with the staging-relevant changes
applied - not a diff, the complete file ready to drop in.

| File | What changed |
|---|---|
| `EwECore/Database/cEwEDatabase.vb` | The staging mechanism itself (`EnableLocalStaging`, `StageLocalWorkingCopy`, `PublishLocalCopyToSource`, `CleanUpLocalWorkingCopy`, orphan sweep, exit-time cleanup). Also: `m_logger` changed `Private`→`Protected` (required - subclasses log through it from the staging error-handling pattern), `CommitTransaction`/`RollbackTransaction` now dispose the transaction object, `GetHistory()`/`GetEwEVersion()` readers wrapped in `Using`. |
| `EwECore/Database/cEwEAccessDatabase.vb` | `Mode=Share Exclusive` removed from both connection strings. Staging wired into `Open()` (stage on write-intent, connect to the staged copy, clean up on failure) and `Close()`. `SaveAs()` detaches the working copy before `Close()` deletes it, uses it as the copy source, cleans it up afterward. |
| `EwECore/Database/cEwEExclusivityLock.vb` | New file - the crash-orphan marker mechanism. One doc-comment fix from the source branch (referenced a class that doesn't exist here). |
| `EwECore/Database/cRestartManagerLookup.vb` | New file - resolves a friendly app name for whatever process holds a locked file, for the retry dialog. Ported from `WHYBBE/WpfFileUnlocker` (MIT licensed) during the original work. |
| `EwECore/Core/cCore.vb` | `Save()` gained the publish-retry-and-dialog block: calls `PublishToSource()`, shows a Retry/Cancel dialog naming the locking app if it fails. |
| `EwECore/DataSource/IEwEDataSource.vb` | New interface member `PublishToSource() As Boolean`. |
| `EwECore/DataSource/Implementations/cDBDataSource.vb` | `PublishToSource()` implementation (delegates to the wrapped database). Plus 34 occurrences of a real, separate correctness fix: `Me.m_db.ReleaseWriter(writer, True)` was discarding its return value - now folded into the overall success flag (`bSucces = bSucces And ...`), so a failed write/commit is actually reported as a failed save. |
| `EwECore/DataSource/Implementations/cEIIDataSource.vb` / `cEIIXMLDataSource.vb` | `PublishToSource()` no-op (both formats are always read-only). |
| `ScientificInterface/EwE6ApplicationFramework.vb` | `cEwEDatabase.EnableLocalStaging = True` at startup - opt-in for this interactive app specifically. |

**Confirmed needing no changes:** `cDatasourceFactory.vb` - its Access-side
logic in `GetCompatibility` was already correct (no overwrite of `Open()`'s
real return value with a hardcoded `Failed_FileNotFound`, unlike a bug
found and fixed on the SQLite side of the source branch).

**Deliberately not ported** - genuinely SQLite/EF-specific, or unrelated to
staging entirely:
- The `cEwEDbWriter`/`IEwEDbWriter` interface extraction (needed only so
  `cEwEEFDatabase.GetWriter()` could override with a different concrete
  return type - meaningless with one writer type)
- A SQL-parameterization feature (`Execute`'s optional `SQLParams`) - a
  real, independent improvement, but unrelated to staging; worth porting
  separately if wanted
- A handful of `Public`→`Public Overridable` accessibility changes that
  exist purely to support a future SQLite subclass
- `cEIIXMLDataSource.vb`'s new `m_logger`-based logging (unrelated feature
  mixed into the same diff as its `PublishToSource` addition)
- Everything about the Access-to-SQLite conversion/migration flow itself
  (`frmEwE6.vb`'s `LoadEcopathModelHandlingConversion`, `cMdb2SqliteConverter.vb`,
  `cEwEVersusDatabase.vb`, comparison mode) - none of it exists without
  SQLite to convert to

## `Sources/EwECore.Staging/` and `Sources/EwECore.Staging.Tests/` - standalone test project

A separate, standalone pair of projects, deliberately placed as **siblings**
of `Sources/EwECore/` rather than inside it - `EwECore.vbproj` is an
SDK-style project that compiles every `.vb` file found anywhere under its
own folder by default, so nesting a second, unrelated project inside it
would get its files unintentionally swept into the real build. An
isolated, compilable subset: the real staging code from the patched
`cEwEDatabase.vb` above (trimmed to just the "Private vars and constants"
and "Open and close" regions, which contain the entirety of the staging
logic and nothing else), plus `cEwEExclusivityLock.vb` unmodified, plus
minimal stubs (`Enums.vb`, `LoggingContext.vb`) and a test-only concrete
subclass (`TestDatabase.vb`) needed because `cEwEDatabase` is `MustInherit`.

```bash
cd Sources/EwECore.Staging.Tests
dotnet test
```

No `.sln` is included - trivial to generate if wanted, from
`Sources/`:
```bash
dotnet new sln -n EwECoreStagingTests
dotnet sln add EwECore.Staging/EwECore.Staging.vbproj
dotnet sln add EwECore.Staging.Tests/EwECore.Staging.Tests.vbproj
```

12 tests, matching exactly what was already run and passed on the source
branch's version of this same test project - the underlying mechanism is
identical, only the trimmed-file extraction changed. One is marked `Skip`
(Windows-specific sharing-violation semantics); remove the `Skip` to run
it for real.

## `LocalStagingCopyTestPlan.md` - manual test plan

30 items, extracted and reworded specifically for this Access-only branch
from the full test plan developed alongside the original feature work -
everything SQLite/EF/comparison-mode-specific left out, the rest adjusted
where it referenced something (the Access-to-SQLite migration step,
`.ewesqlite` paths) that doesn't exist here.
