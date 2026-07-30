// Eii.Ecopath.Storage.Migrator
//
// Applies pending EF Core migrations to a SQLite database file. Exists so
// net48 consumers of Eii.Ecopath.Storage (which cannot run EF Core - a
// net10.0-only dependency there - in-process) can shell out to this
// self-contained tool instead, applying migrations through the exact same
// EF Core migration history (__EFMigrationsHistory) that any net10.0 app
// applies directly and in-process. This is what keeps a .sqlite file
// consistent regardless of which kind of app (net48 ScientificInterface,
// or a net10.0 app) last touched it.
//
// Also handles seeding a freshly mdb2sqlite-converted file: since Access
// databases are always brought fully up to date (via the legacy
// cDatabaseUpdater/RunAllUpdates chain) before conversion, a newly
// converted .sqlite file's schema already matches the latest EF Core model
// exactly - it just has no __EFMigrationsHistory table yet, since mdb2sqlite
// exports data directly and never goes through EF at all. --seed-baseline
// marks every migration currently known to this compiled assembly as
// already-applied, without ever running any of their Up() methods, so
// Database.Migrate() doesn't try to re-run CREATE TABLE against a schema
// that's already fully there.
//
// Usage:
//   Eii.Ecopath.Storage.Migrator.exe <path-to-sqlite-file>
//   Eii.Ecopath.Storage.Migrator.exe <path-to-sqlite-file> --seed-baseline
//
// Exit codes:
//   0 - Success (migrations applied/seeded, or database was already up to date)
//   1 - General failure (an exception was thrown)
//   2 - Invalid arguments, or the input file was not found

using System;
using System.IO;
using System.Linq;
using Eii.Ecopath.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Eii.Ecopath.Storage.Migrator
{
    internal class Program
    {
        private static int Main(string[] args)
        {
            if (args.Length < 1 || args.Length > 2 || string.IsNullOrWhiteSpace(args[0]))
            {
                Console.Error.WriteLine("Usage: Eii.Ecopath.Storage.Migrator <path-to-sqlite-file> [--seed-baseline]");
                return 2;
            }

            string sqliteFilePath = args[0];
            bool seedBaseline = args.Length == 2 && args[1].Equals("--seed-baseline", StringComparison.OrdinalIgnoreCase);

            if (args.Length == 2 && !seedBaseline)
            {
                Console.Error.WriteLine($"Unrecognized argument: {args[1]}");
                return 2;
            }

            if (!File.Exists(sqliteFilePath))
            {
                Console.Error.WriteLine($"File not found: {sqliteFilePath}");
                return 2;
            }

            try
            {
                return seedBaseline ? SeedBaseline(sqliteFilePath) : ApplyMigrations(sqliteFilePath);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"{(seedBaseline ? "Seeding" : "Migration")} failed: {ex}");
                return 1;
            }
        }

        private static int ApplyMigrations(string sqliteFilePath)
        {
            EwEDbContext.DefaultSQLiteFilePath = sqliteFilePath;
            using var context = new EwEDbContext();

            var pending = context.Database.GetPendingMigrations().ToList();
            if (pending.Count == 0)
            {
                Console.WriteLine("Database is already up to date. No migrations applied.");
                return 0;
            }

            Console.WriteLine($"Applying {pending.Count} pending migration(s): {string.Join(", ", pending)}");
            context.Database.Migrate();
            Console.WriteLine("Migration completed successfully.");
            return 0;
        }

        private static int SeedBaseline(string sqliteFilePath)
        {
            EwEDbContext.DefaultSQLiteFilePath = sqliteFilePath;
            using var context = new EwEDbContext();

            var migrationsAssembly = context.GetService<IMigrationsAssembly>();
            var historyRepository = context.GetService<IHistoryRepository>();

            var migrationIds = migrationsAssembly.Migrations.Keys.OrderBy(id => id).ToList();
            if (migrationIds.Count == 0)
            {
                Console.WriteLine("No migrations found in this assembly - nothing to seed.");
                return 0;
            }

            var alreadyApplied = historyRepository.Exists()
                ? historyRepository.GetAppliedMigrations().Select(r => r.MigrationId).ToHashSet()
                : new System.Collections.Generic.HashSet<string>();

            var productVersion = typeof(Microsoft.EntityFrameworkCore.DbContext).Assembly.GetName().Version?.ToString() ?? "unknown";

            var connection = context.Database.GetDbConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();

            using (var createCmd = connection.CreateCommand())
            {
                createCmd.Transaction = transaction;
                createCmd.CommandText = historyRepository.GetCreateIfNotExistsScript();
                createCmd.ExecuteNonQuery();
            }

            int seededCount = 0;
            foreach (var migrationId in migrationIds)
            {
                if (alreadyApplied.Contains(migrationId))
                {
                    continue;
                }
                using var insertCmd = connection.CreateCommand();
                insertCmd.Transaction = transaction;
                insertCmd.CommandText = historyRepository.GetInsertScript(new HistoryRow(migrationId, productVersion));
                insertCmd.ExecuteNonQuery();
                seededCount++;
            }

            transaction.Commit();

            Console.WriteLine(seededCount > 0
                ? $"Seeded {seededCount} migration(s) as already applied: {string.Join(", ", migrationIds)}"
                : "All known migrations were already recorded - nothing to seed.");
            return 0;
        }
    }
}

