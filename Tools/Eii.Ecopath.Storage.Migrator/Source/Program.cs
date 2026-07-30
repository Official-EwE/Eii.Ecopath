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
// Usage:
//   Eii.Ecopath.Storage.Migrator.exe <path-to-sqlite-file>
//
// Exit codes:
//   0 - Success (migrations applied, or database was already up to date)
//   1 - General failure (migration threw an exception)
//   2 - Invalid arguments, or the input file was not found

using System;
using System.IO;
using System.Linq;
using Eii.Ecopath.Storage;
using Microsoft.EntityFrameworkCore;

namespace Eii.Ecopath.Storage.Migrator
{
    internal class Program
    {
        private static int Main(string[] args)
        {
            if (args.Length != 1 || string.IsNullOrWhiteSpace(args[0]))
            {
                Console.Error.WriteLine("Usage: Eii.Ecopath.Storage.Migrator <path-to-sqlite-file>");
                return 2;
            }

            string sqliteFilePath = args[0];

            if (!File.Exists(sqliteFilePath))
            {
                Console.Error.WriteLine($"File not found: {sqliteFilePath}");
                return 2;
            }

            try
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
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Migration failed: {ex}");
                return 1;
            }
        }
    }
}
