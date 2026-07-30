' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).

#If NET48 Then

Imports System.Diagnostics
Imports System.IO
Imports System.Reflection
Imports System.Text

Namespace Database

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Wraps the external Eii.Ecopath.Storage.Migrator.exe tool, which applies
    ''' pending EF Core migrations to a SQLite database file. Exists because
    ''' net48 cannot run EF Core (a net10.0-only dependency of
    ''' Eii.Ecopath.Storage there) in-process - see
    ''' cEwEEFDatabase.EnsureDbContext.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cSqliteMigrator

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Applies any pending EF Core migrations to the given SQLite file.
        ''' Always applies directly, silently, with no separate "check first"
        ''' step - Database.Migrate() (inside the migrator tool) is already a
        ''' cheap no-op when the database is current, and net48 cannot
        ''' enumerate pending migrations itself anyway (that information only
        ''' exists in the net10.0 assembly).
        ''' </summary>
        ''' <param name="strSqliteFilename">Full path to the .sqlite file to migrate.</param>
        ''' <exception cref="FileNotFoundException">The .sqlite file, or the migrator tool itself, could not be found.</exception>
        ''' <exception cref="cSqliteMigratorException">The migrator tool ran but exited with a non-zero code, or could not be started.</exception>
        ''' -------------------------------------------------------------------
        Public Shared Sub MigrateDatabase(strSqliteFilename As String)

            If Not File.Exists(strSqliteFilename) Then
                Throw New FileNotFoundException($"File not found: {strSqliteFilename}", strSqliteFilename)
            End If

            Dim strAppDir As String = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
            Dim strToolPath As String = Path.Combine(strAppDir, "migrator", "Eii.Ecopath.Storage.Migrator.exe")

            If Not File.Exists(strToolPath) Then
                Throw New FileNotFoundException($"Eii.Ecopath.Storage.Migrator.exe not found at: {strToolPath}", strToolPath)
            End If

            Dim strSqlitePathEscaped As String = strSqliteFilename.Replace("""", "\""")

            ' Unlike cMdb2SqliteConverter (a one-off, user-initiated action
            ' where a visible console window is acceptable), this runs
            ' silently on every database open - a console window popping up
            ' every time would be jarring. Output is captured instead, so it
            ' can be included in the exception message on failure.
            Dim psi As New ProcessStartInfo() With {
                .FileName = strToolPath,
                .Arguments = $"""{strSqlitePathEscaped}""",
                .UseShellExecute = False,
                .RedirectStandardOutput = True,
                .RedirectStandardError = True,
                .CreateNoWindow = True,
                .WorkingDirectory = Path.GetDirectoryName(strToolPath)
            }

            Dim sbOutput As New StringBuilder()
            Dim sbError As New StringBuilder()

            Try
                Using proc As Process = Process.Start(psi)
                    AddHandler proc.OutputDataReceived, Sub(sender, e) If e.Data IsNot Nothing Then sbOutput.AppendLine(e.Data)
                    AddHandler proc.ErrorDataReceived, Sub(sender, e) If e.Data IsNot Nothing Then sbError.AppendLine(e.Data)
                    proc.BeginOutputReadLine()
                    proc.BeginErrorReadLine()
                    proc.WaitForExit()

                    If proc.ExitCode <> 0 Then
                        Dim strMessage As String = If(sbError.Length > 0, sbError.ToString(), $"Eii.Ecopath.Storage.Migrator.exe exited with code {proc.ExitCode}")
                        Throw New cSqliteMigratorException(strMessage, proc.ExitCode)
                    End If
                End Using
            Catch ex As cSqliteMigratorException
                Throw
            Catch ex As Exception
                Throw New cSqliteMigratorException(ex.Message, -1, ex)
            End Try

        End Sub

    End Class

End Namespace

#End If
