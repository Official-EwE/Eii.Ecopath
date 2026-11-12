' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.Globalization
Imports System.IO
Imports EwECore
Imports EwECore.Common
Imports EwECore.Database
Imports EwECore.DataSources
Imports EwEUtils.Utilities

Namespace EwEImportHelper

    ''' <summary>
    ''' x86 helper process for genuine legacy Jet3 database files, which a 64-bit EwE process cannot open
    ''' at all (no 64-bit build of the classic Jet OLEDB provider exists, and ACE cannot reliably open
    ''' true Jet3 files). Runs the same, unmodified EwECore import logic, just from an x86 process.
    ''' </summary>
    ''' <remarks>
    ''' Two modes, chosen by the first argument:
    '''
    ''' "version" &lt;source path&gt;
    '''   Prints the real Ecopath version number (invariant culture) to stdout as the only line, exit
    '''   code 0. On failure: nothing on stdout, a brief reason on stderr, exit code 1. Run hidden, with
    '''   stdout redirected - see cEwEAccessDatabase.TryGetLegacyVersionViaX86Helper, the only caller.
    '''
    ''' "import" &lt;source path&gt; &lt;output folder&gt; &lt;result file path&gt; [--force]
    '''   Lists the model(s) in the source, prompts interactively if there is more than one, creates the
    '''   target .eweaccdb, and performs the conversion, printing progress to this console as it goes.
    '''   Writes the result file: first line "OK"/"EXISTS"/"FAIL", second line the target path (OK/EXISTS)
    '''   or a human-readable reason (FAIL). Exit code 0 for OK/EXISTS, 1 for FAIL - the result file is
    '''   authoritative; the exit code is a quick first check only.
    '''   "--force": if the target file already exists, delete and recreate it (result "OK") instead of
    '''   leaving it untouched (result "EXISTS").
    ''' </remarks>
    Module Program

        ''' <remarks>
        ''' STAThread: good practice for a process doing COM-based OLEDB work (cEwEAccessDatabase, via
        ''' cModelImporter). A common real-world failure here is the x86 ACE OLEDB provider not being
        ''' registered (only the 64-bit one is, if just the main app's needs were ever considered) -
        ''' surfaced via cEwEAccessDatabase.ClearOpenFailureLog/GetOpenFailureLog, not by this attribute.
        ''' </remarks>
        <STAThread()>
        Function Main(args As String()) As Integer

            Dim iExitCode As Integer

            If args.Length = 0 Then
                Console.Error.WriteLine("Usage: EwEImportHelper.exe version ""<source>""")
                Console.Error.WriteLine("       EwEImportHelper.exe import ""<source>"" ""<output folder>"" ""<result file>""")
                iExitCode = 1

            Else
                Select Case args(0).ToLowerInvariant()

                    Case "version"
                        If args.Length <> 2 Then
                            Console.Error.WriteLine("Usage: EwEImportHelper.exe version ""<source>""")
                            iExitCode = 1
                        Else
                            iExitCode = RunVersionCheck(args(1))
                        End If

                    Case "import"
                        Dim strImportUsage As String = "Usage: EwEImportHelper.exe import ""<source>"" ""<output folder>"" ""<result file>"" [--force]"
                        If args.Length = 4 Then
                            iExitCode = RunImport(args(1), args(2), args(3), bForceOverwrite:=False)
                        ElseIf args.Length = 5 AndAlso String.Equals(args(4), "--force", StringComparison.OrdinalIgnoreCase) Then
                            iExitCode = RunImport(args(1), args(2), args(3), bForceOverwrite:=True)
                        Else
                            Console.Error.WriteLine(strImportUsage)
                            iExitCode = 1
                        End If

                    Case Else
                        Console.Error.WriteLine($"Unknown mode '{args(0)}'. Expected 'version' or 'import'.")
                        iExitCode = 1

                End Select
            End If

            Return iExitCode

        End Function

#Region " version mode "

        Private Function RunVersionCheck(strPath As String) As Integer

            If Not File.Exists(strPath) Then
                Console.Error.WriteLine($"File not found: {strPath}")
                Return 1
            End If

            Dim db As New cEwEAccessDatabase()

            Try
                ' Read-only: this mode exists purely to read one value.
                Dim access As eDatasourceAccessType = db.Open(strPath, eDataSourceTypes.NotSet, bReadOnly:=True)
                If access <> eDatasourceAccessType.Opened Then
                    Console.Error.WriteLine($"Could not open '{strPath}' (result: {access}).")
                    Return 1
                End If

                Dim sVersion As Single = db.GetVersion()
                If sVersion = 0.0! Then
                    Console.Error.WriteLine($"'{strPath}' opened, but no version could be determined.")
                    Return 1
                End If

                Console.WriteLine(sVersion.ToString(CultureInfo.InvariantCulture))
                Return 0

            Catch ex As Exception
                Console.Error.WriteLine($"Unexpected error reading '{strPath}': {ex.Message}")
                Return 1

            Finally
                If db.IsConnected() Then db.Close()
            End Try

        End Function

#End Region ' version mode

#Region " import mode "

        ''' <param name="bForceOverwrite">Delete and recreate an existing target file instead of
        ''' leaving it untouched - see "--force" in this module's class-level remarks.</param>
        Private Function RunImport(strSource As String, strOutputFolder As String, strResultFile As String, Optional bForceOverwrite As Boolean = False) As Integer

            Try
                If Not File.Exists(strSource) Then
                    Return WriteFailResult(strResultFile, $"Source file not found: {strSource}")
                End If

                If String.IsNullOrWhiteSpace(strOutputFolder) Then
                    strOutputFolder = Path.GetDirectoryName(strSource)
                End If
                If String.IsNullOrWhiteSpace(strOutputFolder) Then
                    strOutputFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                End If

                ' Headless cCore/cPluginManager, no plugins loaded - fine here since this helper only ever
                ' handles a confirmed Jet3 Access file (Access2003/Access2007), never the plugin-importer fallback.
                Using core As New cCore()

                    Console.WriteLine("Opening source database...")

                    Dim dbImp As IModelImporter = cModelImporterFactory.GetModelImporter(core, strSource, New cPluginManager())
                    If dbImp Is Nothing OrElse Not dbImp.Open(strSource) Then
                        Return WriteFailResult(strResultFile, $"Could not open '{strSource}' for import.")
                    End If

                    Dim models() As cExternalModelInfo = dbImp.Models
                    dbImp.Close() ' matches cImportWizard's own pattern: list, then close; Import() below re-opens internally

                    If models Is Nothing OrElse models.Length = 0 Then
                        Return WriteFailResult(strResultFile, $"'{strSource}' contains no importable models.")
                    End If

                    Dim selected As cExternalModelInfo = Nothing
                    If models.Length = 1 Then
                        selected = models(0)
                        Console.WriteLine($"One model found: {selected.Name}")
                    Else
                        Console.WriteLine($"{models.Length} models found in this file:")
                        For i As Integer = 0 To models.Length - 1
                            Console.WriteLine($"  {i + 1}. {models(i).Name}")
                        Next
                        Dim iChoice As Integer = 0
                        Do
                            ' WriteLine (not Write): a redirected pipe reader only sees a full line
                            ' once it ends in a newline. This exact text ("Enter a number (1-") is
                            ' also what cImportProgressDialog matches on to detect the prompt - keep
                            ' both in sync if this text changes.
                            Console.WriteLine($"Enter a number (1-{models.Length}):")
                            Dim strInput As String = Console.ReadLine()
                            Integer.TryParse(strInput, iChoice)
                        Loop While iChoice < 1 OrElse iChoice > models.Length
                        selected = models(iChoice - 1)
                    End If

                    ' Matches cImportWizard.EwE6ModelName's own target-naming convention.
                    Const format As eDataSourceTypes = eDataSourceTypes.Access2007 ' ACCDB - the calling (64-bit) process needs ACE to open this
                    Dim strTargetPath As String = Path.Combine(strOutputFolder, selected.Name)
                    strTargetPath += cDataSourceFactory.GetDefaultExtension(format)
                    strTargetPath = cFileUtils.ToValidFileName(strTargetPath, True)

                    If File.Exists(strTargetPath) Then
                        If bForceOverwrite Then
                            Console.WriteLine($"Target file already exists, overwriting (--force specified): {strTargetPath}")
                            Try
                                File.Delete(strTargetPath)
                            Catch ex As Exception
                                Return WriteFailResult(strResultFile, $"--force was specified, but the existing target file could not be deleted: {strTargetPath}. {ex.Message}")
                            End Try
                            ' Falls through to the normal Create() path below, as if the file never existed.
                        Else
                            ' A target with this name from a prior run is success, not failure.
                            Console.WriteLine($"Target file already exists, reusing: {strTargetPath}")
                            Return WriteExistsResult(strResultFile, strTargetPath)
                        End If
                    End If

                    ' Subscribes to core's message publisher for progress/log text - EwECore-internal, UI-agnostic,
                    ' so printing it here is just another subscriber. cProgressMessage reports Source=External;
                    ' plain log messages report Source=DataSource.
                    Dim syncContext As Threading.SynchronizationContext = Threading.SynchronizationContext.Current
                    Dim handlerExternal As New cMessageHandler(AddressOf OnCoreMessage, eCoreComponentType.External, eMessageType.Any, syncContext)
                    Dim handlerDataSource As New cMessageHandler(AddressOf OnCoreMessage, eCoreComponentType.DataSource, eMessageType.Any, syncContext)
                    core.Messages.AddMessageHandler(handlerExternal)
                    core.Messages.AddMessageHandler(handlerDataSource)

                    Console.WriteLine($"Creating target: {strTargetPath}")
                    Dim dbTarget As New cEwEAccessDatabase()
                    ' Cleared before, read after Create(): see ClearOpenFailureLog's doc comment.
                    cEwEAccessDatabase.ClearOpenFailureLog()
                    Dim createResult As eDatasourceAccessType = dbTarget.Create(strTargetPath, selected.ID, True, format, core.DefaultAuthor)
                    If createResult <> eDatasourceAccessType.Success AndAlso createResult <> eDatasourceAccessType.Opened Then
                        ' createResult alone (e.g. "Failed_OSUnsupported") is just an enum name, not specific enough
                        ' to act on. GetOpenFailureLog() gives the real reason: Create() first copies the embedded
                        ' template (rarely fails), then opens it via OLEDB to set the model name/author - if that
                        ' fails, both Open()'s own Catch block and Create()'s own follow-up code log separately, so
                        ' GetOpenFailureLog() returns the full, ordered sequence, not just the last message.
                        Dim strReason As String = $"Could not create target database (result: {createResult})."
                        Dim strDetail As String = cEwEAccessDatabase.GetOpenFailureLog()
                        If Not String.IsNullOrWhiteSpace(strDetail) Then
                            strReason &= Environment.NewLine & strDetail
                        End If
                        ' A common machine-setup issue, not a bug: the 32-bit (x86) Access Database Engine is
                        ' missing (only the 64-bit one is installed). Matched on the provider name, not the whole
                        ' sentence, since that is the one part .NET/OS locale will never change. Do NOT suggest
                        ' uninstalling the 64-bit one - /quiet below installs both side by side instead, which a
                        ' plain install refuses to do.
                        If strDetail.IndexOf("ACE.OLEDB", StringComparison.OrdinalIgnoreCase) >= 0 Then
                            strReason &= Environment.NewLine & Environment.NewLine &
                                "This machine is missing the 32-bit (x86) Access Database Engine - only the 64-bit one appears to be installed, which this x86 helper process cannot use." & Environment.NewLine &
                                "To fix this:" & Environment.NewLine &
                                "  1. Download the 32-bit (x86) ""Microsoft Access Database Engine 2016 Redistributable"" from Microsoft's website - NOT the x64 one." & Environment.NewLine &
                                "  2. From an ELEVATED (Run as administrator) command prompt, run:" & Environment.NewLine &
                                "       AccessDatabaseEngine.exe /quiet" & Environment.NewLine &
                                "     (plain double-click install will refuse, since a 64-bit version is already installed - /quiet specifically skips that check and installs both side by side)"
                        End If
                        Return WriteFailResult(strResultFile, strReason)
                    End If

                    dbTarget.Open(strTargetPath)

                    Console.WriteLine("Importing...")
                    Dim strLogFile As String = ""
                    Dim bSucces As Boolean = dbImp.Import(selected, dbTarget, strLogFile)

                    dbTarget.Close()

                    core.Messages.RemoveMessageHandler(handlerExternal)
                    core.Messages.RemoveMessageHandler(handlerDataSource)

                    If Not bSucces Then
                        Return WriteFailResult(strResultFile, $"Import failed. See log file: {strLogFile}", strTargetPath)
                    End If

                    Console.WriteLine("Import completed successfully.")
                    Return WriteOkResult(strResultFile, strTargetPath)

                End Using

            Catch ex As Exception
                Return WriteFailResult(strResultFile, $"Unexpected error: {ex.Message}")
            End Try

        End Function

        ''' <summary>
        ''' Receives every progress/log message sent during the import and prints it to this console.
        ''' Matches EwECore.cCore.CoreMessageDelegate exactly (ByRef cMessage) - required, not optional,
        ''' for cMessageHandler to accept this as its delegate.
        ''' </summary>
        Private Sub OnCoreMessage(ByRef message As cMessage)
            Dim strText As String = message.Message
            If String.IsNullOrWhiteSpace(strText) Then Return

            Dim progressMsg As cProgressMessage = TryCast(message, cProgressMessage)
            If progressMsg IsNot Nothing Then
                Dim iPercent As Integer = CInt(Math.Round(progressMsg.Progress * 100.0!))
                Console.WriteLine($"[{iPercent,3}%] {strText}")
            Else
                Console.WriteLine(strText)
            End If
        End Sub

        Private Function WriteOkResult(strResultFile As String, strTargetPath As String) As Integer
            File.WriteAllLines(strResultFile, {"OK", strTargetPath})
            Return 0
        End Function

        ''' <summary>
        ''' Target file already existed and was left untouched - success, distinct from "OK"
        ''' (a freshly created file), but same exit code and a usable file path either way.
        ''' </summary>
        Private Function WriteExistsResult(strResultFile As String, strTargetPath As String) As Integer
            File.WriteAllLines(strResultFile, {"EXISTS", strTargetPath})
            Return 0
        End Function

        ''' <param name="strPartialTargetPath">If non-empty and the file exists, written as a third
        ''' result-file line - a partially converted database with some real data, worth the caller
        ''' offering to open despite the failure. Only pass this once Create() has already succeeded
        ''' (a Create() failure leaves only the blank embedded template, with no imported data at all).</param>
        Private Function WriteFailResult(strResultFile As String, strReason As String, Optional strPartialTargetPath As String = "") As Integer
            Console.Error.WriteLine(strReason)
            Try
                If Not String.IsNullOrWhiteSpace(strPartialTargetPath) AndAlso File.Exists(strPartialTargetPath) Then
                    File.WriteAllLines(strResultFile, {"FAIL", strReason, strPartialTargetPath})
                Else
                    File.WriteAllLines(strResultFile, {"FAIL", strReason})
                End If
            Catch
                ' Result file itself could not be written - caller treats a missing file as failure too.
            End Try
            Return 1
        End Function

#End Region ' import mode

    End Module

End Namespace
