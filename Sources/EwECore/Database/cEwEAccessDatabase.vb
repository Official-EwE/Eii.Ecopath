' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.Data.OleDb
Imports System.Globalization
Imports System.IO
Imports System.Reflection
Imports EwECore.DataSources
Imports EwEUtils.Utilities
Imports Microsoft.Extensions.Logging
Imports Debug = System.Diagnostics.Debug

Namespace Database

    ''' =======================================================================
    ''' <summary>
    ''' Database class specialized for storing and writing EwE data to Microsoft 
    ''' Access databases.
    ''' </summary>
    ''' <remarks>
    ''' This class wraps Microsoft Access specifics such as connection
    ''' strings, deals with deficiencies in the SQL implementation of Bill's 
    ''' beast, and provides default field values from the Access DB schema.
    ''' </remarks>
    ''' =======================================================================
    Public Class cEwEAccessDatabase
        Inherits cEwEDatabase

#Region " Private vars "

        ''' <summary>A connection to an OleDb database, if any.</summary>
        Public m_conn As OleDbConnection = Nothing
        ''' <summary>
        ''' The connection string to connect to a MDB database. No longer
        ''' opened in exclusive mode - multi-user/multi-session protection
        ''' was removed entirely as a deliberate product decision (a
        ''' concurrent open by another session is now the user's own
        ''' responsibility to avoid).
        ''' </summary>
        Private m_strConnectionMDB As String = "Provider=Microsoft.Jet.OLEDB.4.0; Data Source={0};"
        ' Download from http://www.microsoft.com/download/en/details.aspx?displaylang=en&id=13255.
        ' only works if office is 64 bit too. To detect
        ' To determine if office 2010 is 32 bit or 64 bit, we could check a registry key named
        ' bitness. For more information, please refer to this article: http://technet.microsoft.com/en-us/library/ee681792.aspx.
        ' Also here is a resource with a same question you could refer to:
        ' Detect whether Office 2010 is 32bit or 64bit via the registry (http://stackoverflow.com/questions/2203980/detect-whether-office-2010-is-32bit-or-64bit-via-the-registry).
        ''' <summary>
        ''' The connection string to connect to a ACCDB database. No longer
        ''' opened in exclusive mode - see m_strConnectionMDB's own doc
        ''' comment above.
        ''' </summary>
        Private m_strConnectionACCDB As String = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};"

        ''' <summary>File name to access database.</summary>
        Private m_strFileName As String = ""
        Private ReadOnly m_logger As ILogger = LoggingContext.CreateLogger(Of cEwEAccessDatabase)()

        ''' <summary>
        ''' Accumulates messages logged by Open()/Create() on a failure path - Shared (process-wide),
        ''' so a caller like Create() below, which opens its own internal cEwEAccessDatabase instance,
        ''' can still have the real reason read back by someone who never held that instance (e.g.
        ''' EwEImportHelper's Program.vb, which only has its own dbTarget).
        ''' </summary>
        ''' <remarks>
        ''' Appends, never overwrites - a failed Create() call can log more than one message before
        ''' giving up, and only the full sequence is reliably useful. Does not auto-clear on every
        ''' Open() call (called constantly, for unrelated reasons) - a caller wanting a clean trace for
        ''' one operation calls ClearOpenFailureLog() before it and GetOpenFailureLog() after.
        ''' Not safe for concurrent capture from multiple threads at once (each would see a mix of the
        ''' other's messages) - fine for EwEImportHelper's single-threaded usage.
        ''' </remarks>
        Private Shared ReadOnly s_openFailureLog As New List(Of String)
        Private Shared ReadOnly s_openFailureLogLock As New Object()

        ''' <summary>Clears the accumulated failure log - call before the operation to capture.</summary>
        Public Shared Sub ClearOpenFailureLog()
            SyncLock s_openFailureLogLock
                s_openFailureLog.Clear()
            End SyncLock
        End Sub

        ''' <summary>Appends one message; blank/whitespace-only messages are ignored.</summary>
        Public Shared Sub AppendOpenFailureLog(strMessage As String)
            If String.IsNullOrWhiteSpace(strMessage) Then Return
            SyncLock s_openFailureLogLock
                s_openFailureLog.Add(strMessage)
            End SyncLock
        End Sub

        ''' <summary>Everything accumulated since the last ClearOpenFailureLog(), one message per line.</summary>
        Public Shared Function GetOpenFailureLog() As String
            SyncLock s_openFailureLogLock
                Return String.Join(Environment.NewLine, s_openFailureLog)
            End SyncLock
        End Function

#End Region ' Private vars

#Region " Generic "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Create a new M$ Access database.
        ''' </summary>
        ''' <param name="strDatabase">The file name of the .MDB to create.</param>
        ''' <param name="strAuthor">Name of the author to assign.</param>
        ''' <param name="strModelName">Name of the model to use.</param>
        ''' <param name="bOverwrite">States whether an existing database may be overwritten.</param>
        ''' <param name="format">Database format type to use. If not set, the 
        ''' database type is deducted from the <paramref name="strDatabase">database</paramref>.</param>
        ''' <returns>A <see cref="eDatasourceAccessType">eDatasourceAccessType</see> value</returns>
        ''' <remarks>Note that this will NOT open the newly created database.</remarks>
        ''' -------------------------------------------------------------------
        Public Overrides Function Create(strDatabase As String,
                strModelName As String,
                Optional bOverwrite As Boolean = False,
                Optional format As eDataSourceTypes = eDataSourceTypes.NotSet,
                Optional strAuthor As String = "") As eDatasourceAccessType

            Dim strSource As String = ""
            Dim datResult As eDatasourceAccessType = eDatasourceAccessType.Success

            If format = eDataSourceTypes.NotSet Then
                format = cDataSourceFactory.GetSupportedType(strDatabase)
            End If

            Select Case format
                Case eDataSourceTypes.Access2003
                    strSource = "EwE6.mdb"
                    m_logger.LogInformation("Create DB: selected MDB format")
                Case eDataSourceTypes.Access2007
                    strSource = "EwE6.accdb"
                    m_logger.LogInformation("Create DB: selected ACCDB format")
                Case Else
                    datResult = eDatasourceAccessType.Failed_UnknownType
                    m_logger.LogInformation("Create DB: cannot determine format")
            End Select

            If (datResult = eDatasourceAccessType.Success) Then

                ' Save resource file
                If cResourceUtils.SaveResourceToFile(strSource, strDatabase, bOverwrite, Assembly.GetExecutingAssembly()) Then
                    Try
                        'Try to open the database to update the model name
                        Dim db As New cEwEAccessDatabase()
                        datResult = db.Open(strDatabase, format)
                        If (datResult = eDatasourceAccessType.Opened) Then
                            db.Execute(cStringUtils.Localize("UPDATE EcopathModel SET Name='{0}', Author='{1}' WHERE ModelID=1", strModelName, strAuthor))
                            Try
                                ' Egg - over-easy but slightly obfuscated ;)
                                If strModelName.ToLower().Contains(cStringUtils.Shift("Dbsm!Xbmufst").ToLower()) Then
                                    db.Execute(cStringUtils.Localize("UPDATE EcopathGroup SET GroupName='{0}' WHERE GroupID=1", cStringUtils.Shift("Dijdlfo!tiju")))
                                    db.Execute(cStringUtils.Localize("UPDATE EcopathFleet SET FleetName='{0}' WHERE FleetID=1", cStringUtils.Shift("Tfbm!cbtifst")))
                                End If
                            Catch ex As Exception
                                ' Do not let eggs make the pot explode
                                m_logger.LogError(ex, "Create DB: found a rotten egg: " & ex.Message)
                            End Try
                            db.Close()
                        Else
                            cEwEAccessDatabase.AppendOpenFailureLog("Create DB: Unable to open DB using required drivers, check driver installation.")
                            m_logger.LogError("Create DB: Unable to open DB using required drivers, check driver installation.")
                        End If
                        db = Nothing
                    Catch ex As Exception
                        cEwEAccessDatabase.AppendOpenFailureLog(ex.Message)
                        m_logger.LogError(ex, "Create DB: Exception when updating model name: " & ex.Message)
                        datResult = eDatasourceAccessType.Failed_Unknown
                    End Try
                Else
                    'Unable to write to target location
                    cEwEAccessDatabase.AppendOpenFailureLog("Create DB: Unable to save to target location " & strDatabase)
                    m_logger.LogError("Create DB: Unable to save to target location " & strDatabase)
                    datResult = eDatasourceAccessType.Failed_CannotSave
                End If
            End If
            Return datResult

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Save a given Access database to a new destination, and open this 
        ''' new database.
        ''' </summary>
        ''' <param name="strDatabaseTo">Target database name.</param>
        ''' <param name="strModelName">New name to assign to the model.</param>
        ''' <param name="bOverwrite">States whether any model in the way will be obliterated.</param>
        ''' <returns>True if successful.</returns>
        ''' -------------------------------------------------------------------
        Public Overrides Function SaveAs(strDatabaseTo As String,
                strModelName As String,
                Optional bOverwrite As Boolean = False,
                Optional databaseType As eDataSourceTypes = eDataSourceTypes.NotSet) As eDatasourceAccessType

            Dim datResult As eDatasourceAccessType = eDatasourceAccessType.Success
            Dim strDatabaseFrom As String = Me.Name
            Dim bSucces As Boolean = True

            If databaseType = eDataSourceTypes.NotSet Then
                databaseType = cDataSourceFactory.GetSupportedType(strDatabaseTo)
            End If

            ' Databases are copied from one spot to another, not using proper database replication
            ' Therefore, check if source and target types will remain unchanged
            If databaseType <> cDataSourceFactory.GetSupportedType(strDatabaseFrom) Then
                Return eDatasourceAccessType.Failed_TransferTypes
            End If

            ' This session's actual, up-to-date data lives in the local
            ' working copy (if any was staged - a write-intent open always
            ' has one); strDatabaseFrom is stale, frozen at whatever it was
            ' when the working copy was staged. Close() below deletes the
            ' working copy via CleanUpLocalWorkingCopy(), so detach it from
            ' Me first - this leaves the file in place on disk for the copy
            ' further down, instead of losing this session's only
            ' up-to-date copy of the data before we can use it.
            Dim strDataToCarryOver As String = If(String.IsNullOrEmpty(Me.m_strLocalWorkingCopyPath), strDatabaseFrom, Me.m_strLocalWorkingCopyPath)
            Dim strLeftoverWorkingCopy As String = Me.m_strLocalWorkingCopyPath
            Me.m_strLocalWorkingCopyPath = ""

            Me.Close()

            ' Test if we can create a new DB at the intended location
            datResult = Me.Create(strDatabaseTo, strModelName, bOverwrite)

            ' Success?
            If (datResult = eDatasourceAccessType.Success) Then

                ' #Yes: this is painful... File Copy the current DB on top of the newly created DB
                Try
                    ' Can copy database from old to new MDB?
                    System.IO.File.Copy(strDataToCarryOver, strDatabaseTo, True)
                Catch ex As Exception
                    ' #Failure
                    datResult = eDatasourceAccessType.Failed_CannotSave
                End Try

                datResult = Me.Open(strDatabaseTo, databaseType)
                'Able to open?
                If datResult = eDatasourceAccessType.Opened Then
                    ' #Yes: Fix model name after copying
                    Me.Execute(cStringUtils.Localize("UPDATE EcopathModel SET NAME='{0}' WHERE (ModelID=1)", strModelName))
                Else
                    ' #No: Open ye olde database
                    Me.Open(strDatabaseFrom)
                End If
            End If

            ' Now safe to remove the detached working copy, if there was one.
            If Not String.IsNullOrEmpty(strLeftoverWorkingCopy) Then
                Try
                    If File.Exists(strLeftoverWorkingCopy) Then File.Delete(strLeftoverWorkingCopy)
                Catch
                End Try
            End If

            Return datResult

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Reads the Jet engine version byte directly from the MDB file
        ''' header (a single byte at a fixed offset near the start of the
        ''' file - the same byte Jet/ACE itself reads internally to decide
        ''' how to parse the file) and reports whether it indicates a
        ''' genuinely too-old (legacy Jet3, pre-Access-2000) format.
        ''' </summary>
        ''' <remarks>
        ''' Used by cDataSourceFactory.GetCompatibility to identify a
        ''' pre-EwE6, pre-Access-2000 Ecopath 5 database (as TooOld) even
        ''' when it cannot actually be opened: on a 64-bit process there is
        ''' no 64-bit build of the classic Microsoft.Jet.OLEDB.4.0 provider
        ''' at all, so a genuine Jet3 file - which only the classic
        ''' provider can reliably open - gets routed through ACE instead
        ''' (see cDataSourceFactory.GetSupportedType), and ACE cannot open
        ''' it. On 32-bit, the classic provider opens it fine, and
        ''' cEwEDatabase.Compatibility (which does need a working
        ''' connection) reaches the same TooOld conclusion by reading the
        ''' actual Ecopath 5 version number stored inside the file.
        ''' Byte-value mapping confirmed directly against mdbtools' own
        ''' source (include/mdbtools.h.in, MDB_VER_* constants): 0 =
        ''' MDB_VER_JET3 (genuinely too old - the only case this reports
        ''' True for), 1 = MDB_VER_JET4 (Access 2000-2003 - ACE opens this
        ''' fine), 2/3 = ACCDB 2007/2010+.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Shared Function IsLegacyJetFormat(strPath As String) As Boolean
            Const iVersionByteOffset As Integer = &H14 ' 20 decimal
            Const bJet3VersionByte As Byte = 0 ' MDB_VER_JET3 - see remarks above

            Try
                Using fs As New FileStream(strPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
                    If fs.Length <= iVersionByteOffset Then Return False ' not a real/parseable Jet file at all - let the normal Open() path report that
                    fs.Seek(iVersionByteOffset, SeekOrigin.Begin)
                    Return fs.ReadByte() = bJet3VersionByte
                End Using

            Catch ex As Exception
                ' Purely a pre-check - if this itself fails for any reason
                ' (permissions, a genuinely unreadable file, etc.), fall
                ' through to the normal Open() path and let IT report the
                ' real error, rather than this diagnostic masking it.
                Return False
            End Try
        End Function

        ''' <summary>
        ''' Quotes a single argument for a ProcessStartInfo.Arguments string, per CommandLineToArgvW.
        ''' </summary>
        ''' <remarks>
        ''' ProcessStartInfo.ArgumentList would avoid manual quoting entirely but needs .NET Core
        ''' 2.0+/.NET 5+; this file targets net48 (one of EwECore's two TargetFrameworks) too.
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
                    ' Trailing backslashes, right before the closing
                    ' quote: double them so they stay literal.
                    sb.Append(New String(chBackslash, nBackslashes * 2))
                ElseIf strArg(i) = chQuote Then
                    ' Backslashes immediately followed by a literal
                    ' quote: double them, then escape the quote itself.
                    sb.Append(New String(chBackslash, nBackslashes * 2 + 1))
                    sb.Append(chQuote)
                    i += 1
                Else
                    ' Backslashes followed by anything else: literal,
                    ' no doubling needed.
                    sb.Append(New String(chBackslash, nBackslashes))
                    sb.Append(strArg(i))
                    i += 1
                End If
            End While
            sb.Append(chQuote)
            Return sb.ToString()
        End Function

        ''' <summary>
        ''' Reads a legacy Jet3 file's real Ecopath version number by spawning EwEImportHelper.exe, a
        ''' separate x86 process - the classic Microsoft.Jet.OLEDB.4.0 provider it uses has no 64-bit
        ''' build, so this is the one way to read this value for a file ACE cannot open.
        ''' </summary>
        ''' <param name="strPath">The database file to read.</param>
        ''' <param name="sVersion">Set to the version number found, if this returns True.</param>
        ''' <returns>True only if the helper ran, exited 0, and printed a parseable number - any other
        ''' outcome (missing, timeout, non-zero exit, bad output) returns False; callers fall back to
        ''' their own conservative default either way.</returns>
        ''' <remarks>
        ''' Looks for EwEImportHelper.exe in an "EwEImportHelper" subfolder of the current process's own
        ''' base directory - built and published out of band from Tools\EwEImportHelper (same pattern as
        ''' Tools\mdb2sqlite and Tools\Eii.Ecopath.Storage.Migrator); the host app copies its published
        ''' output there via a None/Link item in its own .vbproj.
        ''' Only called on 64-bit (see cDataSourceFactory.GetCompatibility) - on 32-bit the classic
        ''' provider already opens Jet3 files directly.
        ''' </remarks>
        Public Shared Function TryGetLegacyVersionViaX86Helper(strPath As String, ByRef sVersion As Single) As Boolean

            Const iTimeoutMs As Integer = 15000

            Dim strHelperPath As String = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "EwEImportHelper", "EwEImportHelper.exe")
            If Not File.Exists(strHelperPath) Then Return False

            Try
                Dim psi As New Diagnostics.ProcessStartInfo() With {
                    .FileName = strHelperPath,
                    .UseShellExecute = False,
                    .CreateNoWindow = True,
                    .RedirectStandardOutput = True,
                    .RedirectStandardError = True
                }
                psi.Arguments = QuoteArgument("version") & " " & QuoteArgument(strPath)

                Using proc As Diagnostics.Process = Diagnostics.Process.Start(psi)
                    ' WaitForExit before reading output: output here is guaranteed tiny (a single short
                    ' number), so no deadlock risk - reading first would block on ReadToEnd() (only
                    ' returns at EOF) before this timeout ever applied, defeating its purpose.
                    If Not proc.WaitForExit(iTimeoutMs) Then
                        Try
                            proc.Kill()
                        Catch
                        End Try
                        Return False
                    End If

                    If proc.ExitCode <> 0 Then
                        ' Debug.WriteLine, not m_logger: this is an expected-to-sometimes-fail path
                        ' (caller already falls back to its own default), not worth the app's log file.
                        Dim strError As String = proc.StandardError.ReadToEnd()
                        System.Diagnostics.Debug.WriteLine($"TryGetLegacyVersionViaX86Helper: helper exited with code {proc.ExitCode} for '{strPath}': {strError.Trim()}")
                        Return False
                    End If

                    Dim strOutput As String = proc.StandardOutput.ReadToEnd()
                    Return Single.TryParse(strOutput.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, sVersion)
                End Using

            Catch ex As Exception
                ' Helper could not be started, or anything else went wrong -
                ' either way, no real version number to go on.
                System.Diagnostics.Debug.WriteLine($"TryGetLegacyVersionViaX86Helper: exception for '{strPath}': {ex.Message}")
                Return False
            End Try

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Open a connection to a M$ Access database.
        ''' </summary>
        ''' <param name="strDatabase">The database to open.</param>
        ''' <param name="databaseType">Type to use to open the database. Set this
        ''' to 'NotSet' to auto-detect the database type.</param>
        ''' <returns>True if connected successfully.</returns>
        ''' -------------------------------------------------------------------
        Public Overrides Function Open(strDatabase As String,
                                       Optional databaseType As eDataSourceTypes = eDataSourceTypes.NotSet,
                                       Optional bReadOnly As Boolean = False) As eDatasourceAccessType

            ' Preconditions
            Debug.Assert(Not String.IsNullOrEmpty(strDatabase), "Invalid data source specified")
            Debug.Assert(Not Me.IsConnected(), "Connection already open, close first")

            Dim datResult As eDatasourceAccessType = eDatasourceAccessType.Failed_Unknown
            Dim bTryAgain As Boolean = False

            ' Does file exist?
            If Not File.Exists(strDatabase) Then Return eDatasourceAccessType.Failed_FileNotFound

            ' Test read-only file attributes
            If (File.GetAttributes(strDatabase) And FileAttributes.ReadOnly) = FileAttributes.ReadOnly Then
                bReadOnly = True
                ' Return eDatasourceAccessType.Failed_ReadOnly
            End If

            Dim strEffectivePath As String = strDatabase

            If Not bReadOnly AndAlso cEwEDatabase.EnableLocalStaging Then
                ' Write-intent open, with staging enabled: stages a local
                ' working copy to protect the eventual Save from cloud-sync
                ' interference. There is no lock of any kind here -
                ' multi-user/multi-session protection was removed entirely
                ' as a deliberate product decision.
                '
                ' A caller-requested read-only open (the older/incompatible
                ' DB version case - the only remaining use of bReadOnly),
                ' OR any caller that hasn't opted into EnableLocalStaging
                ' at all (e.g. EwERunConsole - see EnableLocalStaging's own
                ' doc comment), needs none of this: connects directly to
                ' the real file below, exactly as before this whole
                ' feature existed.
                Try
                    strEffectivePath = Me.StageLocalWorkingCopy(strDatabase)
                Catch ex As Exception
                    m_logger.LogError(ex, "cEwEAccessDatabase.Open('{0}') failed to stage a local working copy: {1}", strDatabase, ex.Message)
                    Return eDatasourceAccessType.Failed_Unknown
                End Try
            End If

            ' Need to auto-detect database type?
            If databaseType = eDataSourceTypes.NotSet Then
                ' #Yes: auto-detect
                databaseType = cDataSourceFactory.GetSupportedType(strDatabase)
            End If

            Me.m_conn = New OleDbConnection()

            Do
                ' Assemble connection string - against the local working
                ' copy for a write-intent open, or the real file directly
                ' for a read-only open (see above).
                Select Case databaseType
                    Case eDataSourceTypes.Access2003
                        Me.m_conn.ConnectionString = cStringUtils.Localize(Me.m_strConnectionMDB, strEffectivePath)
                    Case eDataSourceTypes.Access2007
                        Me.m_conn.ConnectionString = cStringUtils.Localize(Me.m_strConnectionACCDB, strEffectivePath)
                    Case eDataSourceTypes.NotSet
                        Me.m_conn.ConnectionString = ""
                        datResult = eDatasourceAccessType.Failed_UnknownType
                End Select

                If Not String.IsNullOrEmpty(Me.m_conn.ConnectionString) Then

                    ' Whoah!
                    If bReadOnly Then Me.m_conn.ConnectionString &= "Mode=Read;"
                    ' Remember read-only state
                    Me.IsReadOnly = bReadOnly

                    Try

                        ' Try to open the connection
                        Me.m_conn.Open()
                        ' Set status
                        datResult = eDatasourceAccessType.Opened
                        ' All well: store file name - the REAL path, not the working copy
                        Me.m_strFileName = strDatabase

                    Catch ex As OleDbException
                        If ex.HResult = -2147467259 Then
                            datResult = eDatasourceAccessType.Failed_AlreadyInUse
                        Else
                            datResult = eDatasourceAccessType.Failed_Unknown
                        End If
                        ' OleDb got into trouble
                        cEwEAccessDatabase.AppendOpenFailureLog(ex.Message)
                        m_logger.LogError(ex, cStringUtils.Localize("Open DB: OleDbException {0}, {1} when opening '{2}'", ex.Message, ex.ErrorCode, Me.m_conn.ConnectionString))

                    Catch ex As InvalidOperationException
                        datResult = eDatasourceAccessType.Failed_OSUnsupported
                        cEwEAccessDatabase.AppendOpenFailureLog(ex.Message)
                        m_logger.LogError(ex, cStringUtils.Localize("Open DB: InvalidOperationException {0} when opening {1}", ex.Message, strDatabase))

                    Catch ex As Exception
                        datResult = eDatasourceAccessType.Failed_Unknown
                        cEwEAccessDatabase.AppendOpenFailureLog(ex.Message)
                        m_logger.LogError(ex, cStringUtils.Localize("Open DB: Exception {0} when opening {1}", ex.Message, strDatabase))

                    End Try

                    If (datResult <> eDatasourceAccessType.Opened) And (databaseType = eDataSourceTypes.Access2003) Then
                        databaseType = eDataSourceTypes.Access2007
                        bTryAgain = True
                    Else
                        bTryAgain = False
                    End If

                End If

            Loop Until bTryAgain = False

            If datResult <> eDatasourceAccessType.Opened Then
                Me.CleanUpLocalWorkingCopy()
            End If

            Return datResult

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Close the current M$ Access connection.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overrides Sub Close()

            ' Preconditions
            Debug.Assert(Me.IsConnected(), "Cannot close a connection that is not open")

            If Me.IsConnected Then
                Try
                    Me.m_conn.Close()
                Catch ex As Exception

                End Try
            End If

            If (Me.m_conn IsNot Nothing) Then
                Me.m_conn.Dispose()
                Me.m_conn = Nothing
            End If

            Me.CleanUpLocalWorkingCopy() ' no-op if this session never staged one (a genuine read-only open)

            ' Clear file name
            Me.m_strFileName = ""

            MyBase.Close()

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the name of the connected database.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overrides ReadOnly Property Name() As String
            Get
                Return Me.m_strFileName
            End Get
        End Property

#End Region ' Generic

#Region " Overrides "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Obtains and configures a <see cref="OleDbDataAdapter">OleDbDataAdapter</see>
        ''' for the current M$ Access database.
        ''' </summary>
        ''' <param name="strSQL">The SQL query to obtain the adapter for.</param>
        ''' <returns>A <see cref="OleDbDataAdapter">OleDbDataAdapter</see> if
        ''' successful, or Nothing when an error occurred.</returns>
        ''' <remarks>
        ''' <para>The returned adapter is initialized with default insert, update 
        ''' and delete commands based on the provided query.</para>
        ''' <para>The obtained OleDbDataAdapter should be released via 
        ''' <see cref="ReleaseAdapter">ReleaseAdapter</see>.</para></remarks>
        ''' -------------------------------------------------------------------
        Public Overrides Function GetAdapter(strSQL As String) As IDataAdapter

            Dim adapter As OleDbDataAdapter = DirectCast(MyBase.GetAdapter(strSQL), OleDbDataAdapter)

            ' Sanity check
            If adapter Is Nothing Then
                Return adapter
            End If

            Dim cmdBuilder As New OleDbCommandBuilder(adapter)

            Try
                adapter.ContinueUpdateOnError = False

                ' Configure adapter
                adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey
                adapter.InsertCommand = cmdBuilder.GetInsertCommand(True)
                adapter.UpdateCommand = cmdBuilder.GetUpdateCommand(True)
                adapter.DeleteCommand = cmdBuilder.GetDeleteCommand(True)

            Catch ex As InvalidOperationException
                m_logger.LogError(ex, cStringUtils.Localize("Table in query '{0}' seems to be missing a primary key: {1}", strSQL, ex.Message))
                Debug.Assert(False, cStringUtils.Localize("Table in query '{0}' seems to be missing a primary key: {1}", strSQL, ex.Message))
                adapter = Nothing

            Catch ex As Exception
                m_logger.LogError(ex, cStringUtils.Localize("Error when opening adapter for query {0}: {1}", strSQL, ex.Message))
                Debug.Assert(False, cStringUtils.Localize("Error when opening adapter for query {0}: {1}", strSQL, ex.Message))
                adapter = Nothing
            End Try

            Return adapter

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns the current M$ Access database connection.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overrides Function GetConnection() As IDbConnection
            Return Me.m_conn
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns whether the database can connect to an indicated type.
        ''' </summary>
        ''' <param name="dst">The data source type to test.</param>
        ''' <returns>True if the OS can connect to a given data source type.</returns>
        ''' -------------------------------------------------------------------
        Public Overrides Function CanConnect(dst As eDataSourceTypes) As Boolean

            Dim conn As OleDbConnection = New OleDbConnection()
            Dim strDatabase As String = "~doesnotexist~"
            Dim datResult As eDatasourceAccessType = eDatasourceAccessType.Opened

            ' Try to assemble connection string
            Select Case dst
                Case eDataSourceTypes.Access2003
                    conn.ConnectionString = cStringUtils.Localize(Me.m_strConnectionMDB, strDatabase)
                Case eDataSourceTypes.Access2007
                    conn.ConnectionString = cStringUtils.Localize(Me.m_strConnectionACCDB, strDatabase)
                Case Else
                    conn.ConnectionString = ""
                    datResult = eDatasourceAccessType.Failed_UnknownType
            End Select

            If Not String.IsNullOrWhiteSpace(conn.ConnectionString) Then
                Try
                    conn.Open()
                    conn.Close() ' Can't be, but hey
                Catch ex As InvalidOperationException
                    datResult = eDatasourceAccessType.Failed_OSUnsupported
                Catch ex As OleDbException
                    ' At least the OleDB drivers were loaded successfully...
                    datResult = eDatasourceAccessType.Success
                Catch ex As Exception
                    ' Hmm
                End Try
            End If

            Return (datResult <> eDatasourceAccessType.Failed_OSUnsupported)

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns if the compact database engine is available.
        ''' </summary>
        ''' <param name="strConnectionFrom"></param>
        ''' <param name="strConnectionTo"></param>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Public Overrides Function CanCompact(strConnectionFrom As String, strConnectionTo As String) As Boolean

            Dim compact As IDatabaseCompact = cDatabaseCompactFactory.GetDatabaseCompact(strConnectionFrom)
            If (compact Is Nothing) Then Return False
            Return compact.CanCompact()

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Compact the current M$ Access database.
        ''' </summary>
        ''' <param name="strFileFrom">Source database to compact.</param>
        ''' <param name="strFileTo">Target database to compact to. Can be left blank.</param>
        ''' <returns>True if successful.</returns>
        ''' <remarks>
        ''' Only MDB databases can be compacted for now. Note that the database
        ''' cannot be <see cref="IsConnected">connected</see> when compacting.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Overrides Function Compact(strFileFrom As String,
                                          strFileTo As String) As eDatasourceAccessType

            '  If read-only: report only status and abort
            If (Me.IsReadOnly) Then Return eDatasourceAccessType.Failed_ReadOnly

            ' Fix parameters
            If String.IsNullOrEmpty(strFileTo) Then strFileTo = strFileFrom

            Dim bCompactToOriginal As Boolean = (String.Compare(strFileFrom, strFileTo, True) = 0)
            Dim strConnectionFrom As String = ""
            Dim strConnectionTo As String = ""
            Dim strConnection As String = ""
            Dim result As eDatasourceAccessType = eDatasourceAccessType.Success
            Dim comp As IDatabaseCompact = cDatabaseCompactFactory.GetDatabaseCompact(strFileFrom)

            Select Case cDataSourceFactory.GetSupportedType(strFileFrom)
                Case eDataSourceTypes.Access2003
                    strConnection = Me.m_strConnectionMDB
                Case eDataSourceTypes.Access2007
                    ' Accdb needs different compaction engine, no idea how to do that for now
                    strConnection = Me.m_strConnectionACCDB
                Case Else
                    ' Not supported
                    strConnection = ""
            End Select

            ' No connection string for compacting this type of database?
            If String.IsNullOrEmpty(strConnection) Then Return eDatasourceAccessType.Failed_OSUnsupported
            ' Cannot compact when connected
            Debug.Assert(Me.IsConnected = False)

            Try

                ' #Yes: try to compact
                Dim strDBToOrg As String = strFileTo
                ' Identical database specified for in and out?
                If (bCompactToOriginal) Then
                    ' #Yes: compact DB to temp location
                    strFileTo = System.IO.Path.GetTempFileName()
                End If

                If File.Exists(strFileTo) Then
                    Try
                        File.Delete(strFileTo)
                    Catch ex As Exception
                        Return eDatasourceAccessType.Failed_CannotSave
                    End Try
                End If

                ' Set up connections
                strConnectionFrom = cStringUtils.Localize(strConnection, strFileFrom)
                strConnectionTo = cStringUtils.Localize(strConnection, strFileTo)

                ' Attempt to Compact
                result = comp.Compact(strFileFrom, strConnectionFrom, strFileTo, strConnectionTo)
                ' Is successful?
                If (result <> eDatasourceAccessType.Success) Then
                    ' #No: report compact failure
                    Return result
                End If

                ' Is successfully compacted?
                If File.Exists(strFileTo) Then
                    ' #Yes: Need to copy from temp location?
                    If (bCompactToOriginal) Then
                        ' #Yes: Overwrite original db with compacted db
                        File.Copy(strFileTo, strDBToOrg, True)
                        ' Delete temp location compacted database
                        File.Delete(strFileTo)
                    End If
                End If
                Return eDatasourceAccessType.Success

            Catch ex As Exception
                ' Wow
            End Try

            Return eDatasourceAccessType.Failed_Unknown

        End Function

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="cEwEDatabase.MaxDBVersion"/>
        ''' -------------------------------------------------------------------
        Public Overrides Function MaxDBVersion() As Single
            Return cDatabaseUpdater.MaxSupportedVersion
        End Function

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="cEwEDatabase.Directory"/>
        ''' -------------------------------------------------------------------
        Public Overrides ReadOnly Property Directory() As String
            Get
                Return Path.GetDirectoryName(Me.m_strFileName)
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="cEwEDatabase.FileName"/>
        ''' -------------------------------------------------------------------
        Public Overrides ReadOnly Property FileName() As String
            Get
                Return Path.GetFileNameWithoutExtension(Me.m_strFileName)
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="cEwEDatabase.Extension"/>
        ''' -------------------------------------------------------------------
        Public Overrides ReadOnly Property Extension() As String
            Get
                Return Path.GetExtension(Me.m_strFileName)
            End Get
        End Property

#End Region ' Overrides

    End Class

End Namespace
