# seed-ef-migrations-history.ps1

Seeds the `__EFMigrationsHistory` table in a SQLite database, marking one or
more already-generated EF Core migrations as already applied - without ever
running their `Up()` methods against that database.

## Why this exists

`Database.Migrate()` doesn't compare a migration's generated schema against
what's actually in the database file - it only checks `__EFMigrationsHistory`
to see which migrations have already been recorded, and runs whatever isn't
recorded yet, in order.

That's a problem the first time you adopt an *already-populated* database
into EF Core migrations - which is exactly the situation with `EwE6.sqlite`:
its schema was scaffolded **from** a real, already-complete database, so the
resulting `InitialBaseline` migration's `Up()` contains full `CREATE TABLE`
statements for tables that already physically exist in the file. Running it
for real would just fail with "table already exists". Marking that migration
as already-applied - without ever executing it - is the standard fix for
this situation, and this script automates doing that safely.

**This is normally only needed once per database**, for whichever migration(s)
already reflect a schema that predates EF Core knowing about it. Every
migration generated *after* that point applies completely normally via an
ordinary `Database.Migrate()` call - see the "Migrations" section in
`Eii.Ecopath.Storage`'s own README for the full workflow, including when a
freshly-added migration also needs to be applied to `EwE6.sqlite` itself.

## What it does

1. Scans a `Migrations` folder for generated migration files
   (`<timestamp>_<Name>.cs`, skipping `*.Designer.cs` and
   `*ModelSnapshot.cs`).
2. For each one, reads its matching `.Designer.cs` file to extract the exact
   `MigrationId` (from the `[Migration("...")]` attribute) and
   `ProductVersion` (from the `"ProductVersion"` model annotation) - never
   typed in by hand, so it can't drift out of sync with what EF actually
   generated.
3. Creates `__EFMigrationsHistory` in the target `.sqlite` file if it doesn't
   exist yet, and inserts a row for each migration found.

Safe to re-run: uses `CREATE TABLE IF NOT EXISTS` and `INSERT OR IGNORE`, so
running it again (e.g. after a new migration was added) won't duplicate or
error on rows that are already there.

## Usage

```powershell
.\seed-ef-migrations-history.ps1
```

Run with no arguments, it assumes the standard repo layout:

| Parameter          | Default (relative to this script)                                                    |
|---------------------|---------------------------------------------------------------------------------------|
| `-SqliteFile`        | `..\..\Sources\EwECore\Resources\EwE6.sqlite` (this repo's own starter resource file)  |
| `-MigrationsFolder`  | `..\..\..\Eii.Ecopath.Storage\Eii.Ecopath.Storage\dotnet\Migrations` (sibling repo)    |
| `-Sqlite3Path`       | `..\mdb2sqlite\sqlite3.exe` (shipped alongside the mdb2sqlite tool, not assumed to be on PATH) |

Each default is printed before use. If any assumed location doesn't
actually exist, the script fails with a clear error naming the exact path
it tried and telling you which parameter to use to override it - it never
silently guesses further.

### Overriding a default

```powershell
.\seed-ef-migrations-history.ps1 -MigrationsFolder "D:\SomewhereElse\Eii.Ecopath.Storage\dotnet\Migrations"
```

Any subset of the three parameters can be overridden independently; the
others keep using their computed defaults.

## Verifying it worked

Check the row(s) landed:

```powershell
..\mdb2sqlite\sqlite3.exe "..\..\Sources\EwECore\Resources\EwE6.sqlite" "SELECT * FROM __EFMigrationsHistory;"
```

The real proof, though, is that migrations actually behave correctly
afterward - copy the seeded `.sqlite` file somewhere disposable and run it
through `Eii.Ecopath.Storage.Migrator.exe` (see `Tools\Eii.Ecopath.Storage.Migrator`).
You're looking for:

```
Database is already up to date. No migrations applied.
```

not a "table already exists" error. If you see the latter, the seeding
didn't take effect as expected - re-check the row(s) from the verification
step above against the exact `MigrationId` the script reported when it ran.
