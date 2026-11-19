' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.IO

Namespace Import

    ''' <summary>
    ''' Spawns EwEImportHelper.exe in "import" mode for a confirmed legacy Jet3 Ecopath 5 file - the
    ''' one case this (64-bit) process cannot import itself, since ACE cannot open a genuine Jet3 file.
    ''' The helper runs the entire import (create target + convert) in one process, since a live
    ''' database connection cannot be handed across a process boundary.
    ''' </summary>
    ''' <remarks>
    ''' Bypasses Import.dlgImportDatabase/cImportWizard for this case - its progress UI is driven by
    ''' events in this process, with nothing to show while the real work happens in a separate one.
    ''' Runs the helper hidden with stdout/stderr/stdin all redirected, and shows
    ''' cImportProgressDialog in place of its console window.
    ''' </remarks>
    Public Class cLegacyImportLauncher

        ''' <summary>
        ''' Quotes one argument for ProcessStartInfo.Arguments, per the CommandLineToArgvW rules.
        ''' </summary>
        ''' <remarks>
        ''' ProcessStartInfo.ArgumentList does this automatically but needs .NET Core 2.0+/.NET 5+;
        ''' ScientificInterface targets net48. Duplicated from EwECore's identical
        ''' cEwEAccessDatabase.QuoteArgument rather than shared - small enough not to need a new home.
        ''' </remarks>
        Private Shared Function QuoteArgument(strArg As String) As String
            Dim sb As New Text.StringBuilder()
            Dim chQuote As Char = Convert.ToChar(34)
            Dim chBackslash As Char = "\"c
            sb.Append(chQuote)
            Dim i As Integer = 0
            While i < strArg.Length
                Dim nBackslashes As Integer = 0
                While i < strArg.Length AndAlso strArg(i) = chBackslash
                    nBackslashes += 1
                    i += 1
                End While
                If i = strArg.Length Then
                    sb.Append(New String(chBackslash, nBackslashes * 2))
                ElseIf strArg(i) = chQuote Then
                    sb.Append(New String(chBackslash, nBackslashes * 2 + 1))
                    sb.Append(chQuote)
                    i += 1
                Else
                    sb.Append(New String(chBackslash, nBackslashes))
                    sb.Append(strArg(i))
                    i += 1
                End If
            End While
            sb.Append(chQuote)
            Return sb.ToString()
        End Function

        ''' <summary>Runs the full Ecopath-5-to-EwE6 import for a confirmed legacy Jet3 file.</summary>
        ''' <param name="strSource">The legacy Jet3 database to import.</param>
        ''' <param name="strImportedFileName">Set to the imported EwE6 database's path on True. Can
        ''' also be set on False, when the import created a real target with some data before failing
        ''' partway through - the caller may offer to open that partial result anyway. Untouched for a
        ''' failure before any target file existed.</param>
        ''' <param name="strErrorMessage">Set to a human-readable reason if this returns False.</param>
        ''' <returns>True if the import completed successfully.</returns>
        Public Shared Function TryRunFullImport(strSource As String, ByRef strImportedFileName As String, ByRef strErrorMessage As String) As Boolean

            Dim strHelperPath As String = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "EwEImportHelper", "EwEImportHelper.exe")
            If Not File.Exists(strHelperPath) Then
                strErrorMessage = $"EwEImportHelper.exe was not found at the expected location: {strHelperPath}"
                Return False
            End If

            Dim strResultFile As String = Path.Combine(Path.GetTempPath(), $"EwEImportHelper_{Guid.NewGuid()}.result")
            Dim strOutputFolder As String = Path.GetDirectoryName(strSource)

            Try
                Dim psi As New Diagnostics.ProcessStartInfo() With {
                    .FileName = strHelperPath,
                    .UseShellExecute = False,
                    .CreateNoWindow = True,
                    .RedirectStandardOutput = True,
                    .RedirectStandardError = True,
                    .RedirectStandardInput = True
                }
                psi.Arguments = String.Join(" ", {
                    QuoteArgument("import"),
                    QuoteArgument(strSource),
                    QuoteArgument(strOutputFolder),
                    QuoteArgument(strResultFile)
                })

                Dim iExitCode As Integer

                ' Not wrapped in Using: cImportProgressDialog needs proc to stay alive for its own
                ' whole lifetime (its input box writes to proc.StandardInput throughout), not just up
                ' to WaitForExit. Disposed explicitly below once the dialog has closed.
                Dim proc As Diagnostics.Process = Diagnostics.Process.Start(psi)
                Try
                    Using dlg As New cImportProgressDialog(proc)
                        dlg.ShowDialog() ' Blocks until Continue is clicked, which only enables on exit.
                    End Using
                    iExitCode = proc.ExitCode
                Finally
                    proc.Dispose()
                End Try

                If Not File.Exists(strResultFile) Then
                    strErrorMessage = $"EwEImportHelper.exe exited with code {iExitCode} and wrote no result file."
                    Return False
                End If

                Dim lines() As String = File.ReadAllLines(strResultFile)
                If lines.Length < 2 Then
                    strErrorMessage = $"EwEImportHelper.exe exited with code {iExitCode} and wrote an incomplete result file."
                    Return False
                End If

                ' "EXISTS" (target from an earlier run, left untouched) counts as success, same as "OK".
                If String.Equals(lines(0), "OK", StringComparison.OrdinalIgnoreCase) OrElse
                   String.Equals(lines(0), "EXISTS", StringComparison.OrdinalIgnoreCase) Then
                    strImportedFileName = lines(1)
                    If File.Exists(strImportedFileName) Then Return True
                    strErrorMessage = $"The import reported success but the expected output file is missing: {strImportedFileName}"
                    Return False
                End If

                strErrorMessage = If(lines.Length >= 2 AndAlso Not String.IsNullOrWhiteSpace(lines(1)),
                                     lines(1),
                                     $"EwEImportHelper.exe exited with code {iExitCode}.")

                ' Optional 3rd line: a partially converted target the caller may still offer to open.
                If lines.Length >= 3 AndAlso Not String.IsNullOrWhiteSpace(lines(2)) AndAlso File.Exists(lines(2)) Then
                    strImportedFileName = lines(2)
                End If

                Return False

            Catch ex As Exception
                strErrorMessage = $"Failed to run EwEImportHelper.exe: {ex.Message}"
                Return False

            Finally
                Try
                    If File.Exists(strResultFile) Then File.Delete(strResultFile)
                Catch
                End Try
            End Try

        End Function

    End Class

End Namespace
