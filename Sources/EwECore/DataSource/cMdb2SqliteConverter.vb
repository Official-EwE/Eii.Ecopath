' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).

#If NET48 Then

Imports System.Diagnostics
Imports System.IO
Imports System.Reflection

Namespace Database

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Wraps the external mdb2sqlite.exe conversion tool, expected in a
    ''' "mdb2sqlite" subfolder next to the running executable.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cMdb2SqliteConverter

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Converts an Access database to SQLite by running mdb2sqlite.exe.
        ''' Currently always runs with its console window visible - see the
        ''' commented-out hidden/captured-output path below if that changes.
        ''' </summary>
        ''' <param name="strMdbFilename">Full path to the .mdb/.accdb file to convert.</param>
        ''' <exception cref="FileNotFoundException">The source file or mdb2sqlite.exe itself could not be found.</exception>
        ''' <exception cref="cMdb2SqliteConversionException">mdb2sqlite.exe exited with a non-zero code, or could not be started.</exception>
        ''' -------------------------------------------------------------------
        Public Shared Sub ConvertMdbToSqlite(strMdbFilename As String)

            If Not File.Exists(strMdbFilename) Then
                Throw New FileNotFoundException($"File not found: {strMdbFilename}", strMdbFilename)
            End If

            Dim strAppDir As String = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
            Dim strToolPath As String = Path.Combine(strAppDir, "mdb2sqlite", "mdb2sqlite.exe")

            If Not File.Exists(strToolPath) Then
                Throw New FileNotFoundException($"mdb2sqlite.exe not found at: {strToolPath}", strToolPath)
            End If

            ' mdb2sqlite.exe was compiled from a .ps1 via ps2exe, which hosts the
            ' PowerShell engine internally - it runs as a normal standalone
            ' executable and does not need to be launched through a separate
            ' powershell.exe process.
            Dim strMdbPathEscaped As String = strMdbFilename.Replace("""", "\""")

            Dim psi As New ProcessStartInfo() With {
                .FileName = strToolPath,
                .Arguments = $"-inDatabase ""{strMdbPathEscaped}""",
                .UseShellExecute = False,
                .RedirectStandardOutput = False,
                .RedirectStandardError = False,
                .CreateNoWindow = False,
                .WorkingDirectory = Path.GetDirectoryName(strToolPath)
            }

            Try
                Using proc As Process = Process.Start(psi)
                    proc.WaitForExit()

                    If proc.ExitCode <> 0 Then
                        Throw New cMdb2SqliteConversionException(
                            $"mdb2sqlite.exe exited with code {proc.ExitCode} (see console output above)", proc.ExitCode)
                    End If
                End Using
            Catch ex As cMdb2SqliteConversionException
                Throw
            Catch ex As Exception
                Throw New cMdb2SqliteConversionException(ex.Message, -1, ex)
            End Try

            ' --- Hidden + captured-output alternative (kept for later use) ---
            ' If we later want to run hidden and feed the tool's output into the
            ' app's own status log instead of a visible console window, swap the
            ' block above for this one. Output can be redirected/captured OR
            ' painted to a visible console - not both at once - which is why
            ' this isn't just a toggle on the same code path.
            '
            ' Dim sbOutput As New StringBuilder()
            ' Dim sbError As New StringBuilder()
            ' Dim psiHidden As New ProcessStartInfo() With {
            '     .FileName = strToolPath,
            '     .Arguments = $"-inDatabase ""{strMdbPathEscaped}""",
            '     .UseShellExecute = False,
            '     .RedirectStandardOutput = True,
            '     .RedirectStandardError = True,
            '     .CreateNoWindow = True,
            '     .WorkingDirectory = Path.GetDirectoryName(strToolPath)
            ' }
            ' Try
            '     Using proc As Process = Process.Start(psiHidden)
            '         AddHandler proc.OutputDataReceived, Sub(sender, e) If e.Data IsNot Nothing Then sbOutput.AppendLine(e.Data)
            '         AddHandler proc.ErrorDataReceived, Sub(sender, e) If e.Data IsNot Nothing Then sbError.AppendLine(e.Data)
            '         proc.BeginOutputReadLine()
            '         proc.BeginErrorReadLine()
            '         proc.WaitForExit()
            '         If proc.ExitCode <> 0 Then
            '             Dim strMessage As String = If(sbError.Length > 0, sbError.ToString(), $"mdb2sqlite.exe exited with code {proc.ExitCode}")
            '             Throw New cMdb2SqliteConversionException(strMessage, proc.ExitCode)
            '         End If
            '         ' e.g. feed sbOutput.ToString()/sbError.ToString() into the app's status log here
            '     End Using
            ' Catch ex As cMdb2SqliteConversionException
            '     Throw
            ' Catch ex As Exception
            '     Throw New cMdb2SqliteConversionException(ex.Message, -1, ex)
            ' End Try

        End Sub

    End Class

End Namespace

#End If