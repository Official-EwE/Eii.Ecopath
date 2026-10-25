' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.Data.OleDb
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.IO
Imports System.Reflection
Imports System.Runtime.InteropServices
Imports EwEUtils.Utilities
Imports Microsoft.Extensions.Logging
Imports Debug = System.Diagnostics.Debug

#If VERBOSE Then
#Const VERBOSE_LEVEL = 4
#End If

' Writer caching does not work when outside a transaction. Code should be changed:
' - Keep all writers in dicitionary, commit when releasing the transaction

Namespace Database

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Generic base class for implementing a DBMS-specific EwE database
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public MustInherit Class cEwEDatabase

#Region " Class cEwEDbWriter "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Helper class that eases the process of adding records to a table.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Class cEwEDbWriter
            Implements IDisposable, IEwEDbWriter

            ''' <summary>Reference count, used by GetWriter/ReleaseWriter bookkeeping.</summary>
            Public Property RefCount As Integer Implements IEwEDbWriter.RefCount
                Get
                    Return Me.m_refcount
                End Get
                Set(value As Integer)
                    Me.m_refcount = value
                End Set
            End Property

            ''' <summary>Database to write to</summary>
            Private m_db As cEwEDatabase = Nothing
            ''' <summary>Table in the database to write to</summary>
            Private m_strTable As String = ""
            ''' <summary>DataSet contains a mirror of the indicated table</summary>
            Private m_ds As DataSet = Nothing
            ''' <summary>DataTable that mirrors the indicated table</summary>
            Private m_dt As DataTable = Nothing
            ''' <summary>Adapter to sync table content back and forth</summary>
            Private m_apt As IDataAdapter = Nothing

            Private m_dtSchema As DataTable = Nothing
            Private m_bDisposed As Boolean = False

            Friend m_refcount As Integer = 0

#If DEBUG Then
            Private ReadOnly m_ID As Integer = 0
            Private Shared s_IDnext As Integer = 1
#End If

            ''' ---------------------------------------------------------------
            ''' <summary>
            ''' <para>Constructor, initializes a new instance of a cEwEDbWriter.</para>
            ''' </summary>
            ''' <param name="db">The <see cref="cEwEDatabase">cEwEDatabase</see> to read from.</param>
            ''' <param name="strTable">The name of the table to link to.</param>
            ''' <remarks>
            ''' <para>This method will attempt to connect and read the table into its internal
            ''' structures. It might be prudent to validate whether the instance is connected
            ''' by calling <see cref="IsConnected">IsConnected</see> prior to using it.</para>
            ''' </remarks>
            ''' ---------------------------------------------------------------
            Public Sub New(ByRef db As cEwEDatabase, strTable As String)
#If DEBUG Then
                Me.m_ID = s_IDnext
                s_IDnext += 1
#End If
#If VERBOSE_LEVEL > 2 Then
                Debug.WriteLine("DB writer " & Me.m_ID & " created(" & strTable & ")")
#End If
                Me.Connect(db, strTable)
            End Sub

            ''' ---------------------------------------------------------------
            ''' <summary>
            ''' Trash me!
            ''' </summary>
            ''' ---------------------------------------------------------------
            Public Sub Dispose() Implements IDisposable.Dispose
                If (Me.m_bDisposed = False) Then
                    Me.m_bDisposed = True
#If VERBOSE_LEVEL > 2 Then
                    Debug.WriteLine("DB writer " & Me.m_ID & " disposed, refcount " & me.m_refcount)
#End If
                    If Me.IsConnected Then Me.Disconnect(True)
                End If
                GC.SuppressFinalize(Me)
            End Sub

            ''' ---------------------------------------------------------------
            ''' <summary>
            ''' Attempts to connect to the database and read the table data.
            ''' </summary>
            ''' <param name="db">The <see cref="cEwEDatabase">cEwEDatabase</see> to read from.</param>
            ''' <param name="strTable">The name of the table to link to.</param>
            ''' <returns>True if connected.</returns>
            ''' ---------------------------------------------------------------
            Public Function Connect(ByRef db As cEwEDatabase, strTable As String) As Boolean

                ' Pre
                Debug.Assert(db IsNot Nothing, "Need a valid database")
                Debug.Assert(Not String.IsNullOrEmpty(strTable), "Need a table name")

                Dim conn As IDbConnection = db.GetConnection()

                ' Remember these
                Me.m_db = db
                Me.m_strTable = strTable
                Me.m_dtSchema = Nothing

                ' OLEDB hack
                If TypeOf conn Is OleDbConnection Then
                    Me.m_dtSchema = DirectCast(conn, OleDbConnection).GetSchema("Columns", New String() {Nothing, Nothing, strTable, Nothing})
                End If

                ' Get adapter
                Me.m_apt = Me.m_db.GetAdapter(String.Format("Select * from {0}", strTable))
                ' Adapter gotten succesfully?
                If (Me.m_apt IsNot Nothing) Then
                    ' #Yes: Get dataset
                    Me.m_ds = Me.m_db.GetDataSet(Me.m_apt, strTable)
                    ' Dataset obtained succesfully?
                    If (Me.m_ds IsNot Nothing) Then
                        ' #Yes: read the data
                        Me.m_apt.Fill(Me.m_ds)
                        ' Set up DataTable for making modifications
                        Me.m_dt = Me.m_ds.Tables(0)
                    Else
                        ' #No: dataset failed, release adapter
                        Me.m_db.ReleaseAdapter(Me.m_apt)
                        Me.m_apt = Nothing
                        ' Release the rest as well, why not
                        Me.m_db = Nothing
                        Me.m_strTable = ""
                    End If
                End If
                ' Return connection state
                Return Me.IsConnected()

            End Function

            ''' ---------------------------------------------------------------
            ''' <summary>
            ''' Commit all pending changes in the cEwEDBWriter without closing
            ''' the writer; the writer is left open for further database operations.
            ''' </summary>
            ''' ---------------------------------------------------------------
            Public Function Commit() As Boolean Implements IEwEDbWriter.Commit

                ' Optimizations
                If Not Me.IsConnected Then Return False
                If Not Me.m_ds.HasChanges() Then Return True

                If Me.m_dtSchema IsNot Nothing Then
                    ' Fix unwanted nulls in new and modified rows
                    Dim adrows() As DataRow = Me.m_dt.Select()
                    For Each drow As DataRow In adrows
                        If drow.RowState = DataRowState.Added Or drow.RowState = DataRowState.Modified Then
                            Me.FixUnwantedDBNulls(drow)
                        End If
                    Next
                End If
                Return Me.m_db.CommitDataSet(Me.m_ds, Me.m_apt, Me.m_strTable)

            End Function

            ''' ---------------------------------------------------------------
            ''' <summary>
            ''' Disconnects the cEwEDBWriter from the database.
            ''' </summary>
            ''' <param name="bSaveChanges">States whether changes need to be saved (true)
            ''' or discarded (false).</param>
            ''' ---------------------------------------------------------------
            Public Function Disconnect(Optional bSaveChanges As Boolean = True) As Boolean Implements IEwEDbWriter.Disconnect

                Dim bSucces As Boolean = False
                If Not Me.IsConnected Then Return bSucces

                If bSaveChanges Then
                    bSucces = Me.Commit()
                End If

                bSucces = bSucces And Me.m_db.ReleaseDataSet(Me.m_ds)
                bSucces = bSucces And Me.m_db.ReleaseAdapter(Me.m_apt)

                Me.m_dt = Nothing
                Me.m_ds = Nothing
                Me.m_apt = Nothing
                Me.m_db = Nothing
                Me.m_strTable = ""

#If VERBOSE_LEVEL > 2 Then
                Debug.WriteLine("DB writer " & Me.m_ID & " disconnected")
#End If
                Return bSucces
            End Function

            ''' ---------------------------------------------------------------
            ''' <summary>
            ''' Returns whether the cEwEDBWriter is currently <see cref="Connect">connected</see>.
            ''' </summary>
            ''' <returns>True if connected.</returns>
            ''' ---------------------------------------------------------------
            Public Function IsConnected() As Boolean Implements IEwEDbWriter.IsConnected
                Return Me.m_apt IsNot Nothing
            End Function

            ''' ---------------------------------------------------------------
            ''' <summary>
            ''' Returns whether the cEwEDBWriter ahs been disposed
            ''' ''' </summary>
            ''' <returns>True if disposed.</returns>
            ''' ---------------------------------------------------------------
            Public Function IsDisposed() As Boolean Implements IEwEDbWriter.IsDisposed
                Return Me.m_bDisposed
            End Function

            ''' ---------------------------------------------------------------
            ''' <summary>
            ''' Returns an empty row for the given table to populate values into.
            ''' </summary>
            ''' <returns>An empty row</returns>
            ''' <remarks>Note that this empty row is not yet added to the table.
            ''' If the row is populated to satisfaction, call <see cref="AddRow">AddRow</see>
            ''' to add it to the the list of rows waiting to be added to the database.</remarks>
            ''' ---------------------------------------------------------------
            Public Function NewRow() As DataRow Implements IEwEDbWriter.NewRow
                Try
                    Return Me.m_dt.NewRow()
                Catch ex As Exception
                    Debug.Assert(False, ex.Message)
                    Return Nothing
                End Try
            End Function

            ''' ---------------------------------------------------------------
            ''' <summary>
            ''' Adds a row previously obtained from <see cref="NewRow">NewRow</see>
            ''' to the list of rows waiting to be added to the database.
            ''' </summary>
            ''' <remarks>
            ''' <para>This method will preserve and re-align a sequence field if specified
            ''' in the <see cref="cEwEDbWriter">Constructor</see>.</para>
            ''' <para>Use <see cref="cEwEDbWriter.RemoveRow">RemoveRow</see> to protect the
            ''' row sequence during deletes.</para>
            ''' </remarks>
            ''' ---------------------------------------------------------------
            Public Sub AddRow(drow As DataRow) Implements IEwEDbWriter.AddRow
                'Me.FixStringLengths(drow)
                Me.m_dt.Rows.Add(drow)
            End Sub

            ''' ---------------------------------------------------------------
            ''' <summary>
            ''' Returns an arbitrary row maintained in the writer.
            ''' </summary>
            ''' <param name="drow">The datarow to delete.</param>
            ''' <returns>The row.</returns>
            ''' <remarks>
            ''' <para>This method will preserve and re-align a sequence field if specified
            ''' in the <see cref="cEwEDbWriter">Constructor</see>.</para>
            ''' <para>Use <see cref="cEwEDbWriter.AddRow">AddRow</see> to protect the
            ''' row sequence during additions.</para>
            ''' </remarks>
            ''' ---------------------------------------------------------------
            Public Function RemoveRow(drow As DataRow) As Boolean Implements IEwEDbWriter.RemoveRow
                Me.m_dt.Rows.Remove(drow)
                Return True
            End Function

            ''' ---------------------------------------------------------------
            ''' <summary>
            ''' Returns an arbitrary row maintained in the writer
            ''' </summary>
            ''' <param name="nRow">The row number to retrieve</param>
            ''' <returns>The row</returns>
            ''' <remarks>This method might not be necessary?</remarks>
            ''' ---------------------------------------------------------------
            Public Function GetRow(nRow As Integer) As DataRow Implements IEwEDbWriter.GetRow
                Return Me.m_dt.Rows(nRow)
            End Function

            ''' ---------------------------------------------------------------
            ''' <summary>
            ''' Overridden to close the cEwEDbWriter if not already closed.
            ''' </summary>
            ''' ---------------------------------------------------------------
            Protected Overrides Sub Finalize()
#If VERBOSE_LEVEL > 2 Then
                Debug.WriteLine("DB writer " & Me.m_ID & " finalized")
#End If
                ' JS 18May16: Disconnecting upon disposal is very likely to fail,
                ' because the underlying transaction has probably already perished.
                ' Instead, users should explicitly Disconnect
                Debug.Assert(Not Me.IsConnected(), "Database adapter for " & Me.m_strTable & " not explicitly released!")

                ' Changes may get lost here, which is the consequence of not properly releasing
                If Me.IsConnected Then
                    Me.Disconnect(Me.m_db.Transaction IsNot Nothing)
                End If

                MyBase.Finalize()
            End Sub

            ''' ---------------------------------------------------------------
            ''' <summary>
            ''' Get a reference to the DataTable for the current writer.
            ''' </summary>
            ''' <returns></returns>
            ''' ---------------------------------------------------------------
            Public Function GetDataTable() As DataTable Implements IEwEDbWriter.GetDataTable
                Return Me.m_dt
            End Function

            ''' ---------------------------------------------------------------
            ''' <summary>
            ''' Get the table name of the writer.
            ''' </summary>
            ''' <returns></returns>
            ''' ---------------------------------------------------------------
            Public Function GetTableName() As String Implements IEwEDbWriter.GetTableName
                Return Me.m_strTable
            End Function

            ''' ---------------------------------------------------------------
            ''' <summary>
            ''' Helper method; replaces DBNull values that are specified as not
            ''' Nullable in the underlying Access database schema with the default
            ''' value in the schema.
            ''' </summary>
            ''' <param name="drow">The row to fix.</param>
            ''' ---------------------------------------------------------------
            Private Sub FixUnwantedDBNulls(ByRef drow As DataRow)

                Dim bIsValueNull As Boolean = False
                Dim bIsNullable As Boolean = False
                Dim bHasDefault As Boolean = False
                Dim columnDataType As Data.OleDb.OleDbType = OleDbType.IUnknown
                Dim strColumnName As String = ""

                For Each drowSchema As DataRow In Me.m_dtSchema.Rows
                    strColumnName = CStr(drowSchema("COLUMN_NAME"))

                    bIsValueNull = drow.IsNull(strColumnName)
                    bIsNullable = CBool(drowSchema("IS_NULLABLE"))
                    bHasDefault = CBool(drowSchema("COLUMN_HASDEFAULT"))

                    If bIsValueNull And Not bIsNullable Then

                        ' Set default using common data type conversions to bypass language-specific
                        ' problems caused by misinterpreted decimal separators, etc

                        ' Get column data type
                        columnDataType = CType(drowSchema("DATA_TYPE"), Data.OleDb.OleDbType)

                        ' Convert defaults for common data types. Add others when needed.
                        Select Case columnDataType

                            Case OleDbType.WChar
                                If (bHasDefault) Then
                                    ' Get default value for this column (it's a string, regardless of column datatype. Brilliant)
                                    ' Access weirdness: fix double quotes problems
                                    drow(strColumnName) = CStr(drowSchema("COLUMN_DEFAULT")).Replace("""", "")
                                Else
                                    drow(strColumnName) = String.Empty
                                End If

                            Case OleDbType.Boolean
                                If (bHasDefault) Then
                                    ' Get default value for this column (it's a string, regardless of column datatype. Brilliant)
                                    ' Work-around for Access yes/no values
                                    Dim dummy As Boolean
                                    Boolean.TryParse(CStr(drowSchema("COLUMN_DEFAULT")), dummy)
                                    drow(strColumnName) = dummy
                                Else
                                    drow(strColumnName) = False
                                End If

                            Case OleDbType.SmallInt
                                If (bHasDefault) Then
                                    drow(strColumnName) = CType(CStr(drowSchema("COLUMN_DEFAULT")), Int16)
                                Else
                                    drow(strColumnName) = 0
                                End If

                            Case OleDbType.Integer
                                If (bHasDefault) Then
                                    drow(strColumnName) = CInt(CStr(drowSchema("COLUMN_DEFAULT")))
                                Else
                                    drow(strColumnName) = 0
                                End If

                            Case OleDbType.Single
                                Try
                                    If bHasDefault Then
                                        drow(strColumnName) = cStringUtils.ConvertToSingle(CStr(drowSchema("COLUMN_DEFAULT")), 0.0!)
                                    Else
                                        drow(strColumnName) = 0.0!
                                    End If
                                Catch ex As Exception
                                    drow(strColumnName) = 0.0!
                                End Try

                            Case OleDbType.Double
                                Try
                                    If bHasDefault Then
                                        drow(strColumnName) = cStringUtils.ConvertToDouble(CStr(drowSchema("COLUMN_DEFAULT")), 0.0#)
                                    Else
                                        drow(strColumnName) = 0.0#
                                    End If
                                Catch ex As Exception
                                    Debug.Assert(False)
                                    drow(strColumnName) = 0.0#
                                End Try

                            Case OleDbType.Currency
                                ' ToDo_JS: Consider what to do here; test possible issues across locales
                                Debug.Assert(False, "Currency defaults not properly supported in the EwE database logic")

                            Case Else
                                ' Unexpected datatype encountered
#If VERBOSE_LEVEL >= 2 Then
                                    Console.WriteLine("   - Default {0} for column {1}: unexpected datatype {2}", drow(strColumnName), strColumnName, columnDataType.ToString())
#End If
                                ' Set the default and hope for the best
                                If (bHasDefault) Then
                                    drow(strColumnName) = CStr(drowSchema("COLUMN_DEFAULT"))
                                End If

                        End Select
                    End If
                Next
            End Sub

            ''' ---------------------------------------------------------------
            ''' <summary>
            ''' Helper method; replaces DBNull values that are specified as not
            ''' nullable in the underlying Access database schema with the default
            ''' value in the schema.
            ''' </summary>
            ''' <param name="drow">The row to fix.</param>
            ''' ---------------------------------------------------------------
            Private Sub FixStringLengths(drow As DataRow)

                Debug.Assert(False, "This code does not work. iMaxLen will always be unknown due to a bug in Access according to MSDN")
                Dim columnDataType As Data.OleDb.OleDbType = OleDbType.IUnknown
                Dim strColumnName As String = ""
                Dim iMaxLen As Integer = 0
                Dim iLen As Integer = 0

                For Each drowSchema As DataRow In Me.m_dtSchema.Rows

                    strColumnName = CStr(drowSchema("COLUMN_NAME"))
                    columnDataType = CType(drowSchema("DATA_TYPE"), Data.OleDb.OleDbType)

                    Select Case columnDataType
                        Case OleDbType.WChar, OleDbType.VarWChar, OleDbType.LongVarChar
                            Dim strVal As String = CStr(drow(strColumnName))

                            iMaxLen = CInt(drowSchema("CHARACTER_MAXIMUM_LENGTH"))
                            iLen = strVal.Length

                            If (iLen > iMaxLen) Then
                                drow(strColumnName) = strVal.Substring(0, iMaxLen)
                            End If

                    End Select
                Next
            End Sub

        End Class

#End Region ' Class cEwEDbWriter

#Region " Private vars and constants "

        ''' <summary>Current database version.</summary>
        Private m_sVersion As Single = 0.0
        ''' <summary>EwE version that produces the current database version.</summary>
        Private m_strEwEversion As String = ""
        ''' <summary>Database read-only state.</summary>
        Private m_bIsReadonly As Boolean = False
        ''' <summary>Directory associated with the database.</summary>
        Private m_strDirectory As String = ""

        ''' <summary>Oldest EwE5 version number supported</summary>
        Private Const cDBVERSION_EWE5_MIN As Single = 1.6!
        ''' <summary>Newest EwE5 version number supported</summary>
        Private Const cDBVERSION_EWE5_MAX As Single = 1.73!
        Protected ReadOnly m_logger As ILogger = LoggingContext.CreateLogger(Of cEwEDatabase)()

#End Region ' Private vars and constants

#Region " Open and close "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Create a new database.
        ''' </summary>
        ''' <param name="strDatabase">The database to create.</param>
        ''' <param name="strModelName">Name of the new model.</param>
        ''' <param name="strAuthor">Name of the author to add. May be omitted.</param>
        ''' <param name="databaseType">Type of the database to create. May be omitted.</param>
        ''' <param name="bOverwrite">States whether an existing database may be overwritten.</param>
        ''' <returns>True of created succesfully.</returns>
        ''' <remarks>Note that this will NOT open the newly created database.</remarks>
        ''' -------------------------------------------------------------------
        Public MustOverride Function Create(strDatabase As String,
                strModelName As String,
                Optional bOverwrite As Boolean = False,
                Optional databaseType As eDataSourceTypes = eDataSourceTypes.NotSet,
                Optional strAuthor As String = "") As eDatasourceAccessType

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Open a connection to a database.
        ''' </summary>
        ''' <param name="strDatabase">The database to open.</param>
        ''' <param name="databaseType">Type to use to open the database. Set this
        ''' to 'NotSet' to auto-detect the database type.</param>
        ''' <returns>True if connected succesfully.</returns>
        ''' -------------------------------------------------------------------
        Public MustOverride Function Open(strDatabase As String,
                                          Optional databaseType As eDataSourceTypes = eDataSourceTypes.NotSet,
                                          Optional bReadOnly As Boolean = False) As eDatasourceAccessType

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Close an open connection.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overridable Sub Close()
            Me.m_sVersion = 0.0!
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the name of the connected database.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public MustOverride ReadOnly Property Name() As String

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Save a given database to a new destination, and open this new database.
        ''' </summary>
        ''' <param name="strDatabaseTo">Target database name.</param>
        ''' <param name="strModelName">New name to assign to the model.</param>
        ''' <param name="bOverwrite">States whether any model in the way will be obliterated.</param>
        ''' <returns>True if successful.</returns>
        ''' -------------------------------------------------------------------
        Public MustOverride Function SaveAs(strDatabaseTo As String,
                strModelName As String,
                Optional bOverwrite As Boolean = False,
                Optional databaseType As eDataSourceTypes = eDataSourceTypes.NotSet) As eDatasourceAccessType

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set whether the database is read-only.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property IsReadOnly() As Boolean
            Get
                Return Me.m_bIsReadonly
            End Get
            Protected Set(value As Boolean)
                Me.m_bIsReadonly = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Opt-in switch for the local working-copy staging mechanism below.
        ''' Defaults to False: EwECore.dll is used by more than just the
        ''' interactive ScientificInterface app (e.g. EwERunConsole, or any
        ''' other future console/API consumer, per cCoreService), and
        ''' staging is pure cost with no possible benefit for a caller that
        ''' never calls Save() - it pays for a File.Copy on every open to
        ''' guard against a corruption scenario (a live editing session
        ''' colliding with a cloud sync client) that can't occur if nothing
        ''' ever writes back to the source file at all.
        ''' </summary>
        ''' <remarks>
        ''' A single, application-level flag rather than a per-call
        ''' parameter threaded through Open() - set ONCE, at startup, by
        ''' the interactive host application (see
        ''' EwE6ApplicationFramework.Main()). A per-call parameter would
        ''' put the burden on every future Core.LoadModel() call site to
        ''' remember to opt in correctly; a single startup flag means every
        ''' call anywhere in that host, present or future, is automatically
        ''' covered with nothing to remember at each call site. Any other
        ''' consumer of EwECore.dll that never sets this gets the safe
        ''' default (no staging, no unnecessary cost) with no action needed
        ''' on their part either.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Shared Property EnableLocalStaging As Boolean = False

        ''' =======================================================================
        ''' <summary>
        ''' Local working-copy staging, shared by every concrete database type.
        ''' </summary>
        ''' <remarks>
        ''' <para>Cloud-sync clients (OneDrive, Dropbox, etc.) can rewrite a
        ''' file on disk out from under an application that has it open,
        ''' which can corrupt an Access or SQLite database mid-save. To guard
        ''' against this, every successful Open() stages a private local
        ''' working copy under
        ''' "%LocalAppData%\Ecopath with Ecosim\DBs\" and the actual database
        ''' connection (OleDb or EF/SQLite) is pointed at that copy, never at
        ''' the original file, for the entire session - but only when
        ''' EnableLocalStaging (above) is True. When it's False (the
        ''' default), Open() connects directly to the real file, exactly as
        ''' if this whole mechanism did not exist.</para>
        ''' <para>On Save, PublishLocalCopyToSource() blind-overwrites the
        ''' real file with the working copy's current bytes - no fingerprint
        ''' check, no conflict prompt.</para>
        ''' <para><b>Multi-user/multi-session protection is explicitly out of
        ''' scope</b> - a deliberate product decision. There is no lock of
        ''' any kind (no companion sidecar, no native Access exclusive mode)
        ''' anywhere in this class or its subclasses. If two sessions -
        ''' anywhere, on any kind of storage, including a plain local network
        ''' share - open the same file at the same time, that is the user's
        ''' own responsibility to avoid; the last one to save simply wins,
        ''' with no detection and no warning. This mechanism protects a
        ''' single session's own save from cloud-sync interference; it does
        ''' not, and is not intended to, protect against another session
        ''' editing the same file concurrently.</para>
        ''' </remarks>
        ''' =======================================================================
        Protected m_strRealFilePath As String = ""
        Protected m_strLocalWorkingCopyPath As String = ""
        Private ReadOnly m_workingCopyMarker As New cEwEExclusivityLock()

        Private Const cWorkingCopySubfolder As String = "Ecopath with Ecosim\DBs"

        Private Shared ReadOnly s_processExitHookLock As New Object()
        Private Shared s_processExitHookRegistered As Boolean = False

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Stages a local, disposable working copy of strRealPath. Only the
        ''' file NAME of strRealPath is used - its directory is discarded, so
        ''' the working folder stays flat regardless of where models are
        ''' actually stored, per this feature's specification.
        ''' </summary>
        ''' <param name="strRealPath">The real (possibly cloud-synced) file
        ''' to stage a working copy of.</param>
        ''' <returns>The local working copy's full path. Callers should open
        ''' their actual database connection against THIS path, not
        ''' strRealPath.</returns>
        ''' -------------------------------------------------------------------
        Protected Function StageLocalWorkingCopy(strRealPath As String) As String
            cEwEDatabase.CleanUpOrphanedWorkingCopies()

            Dim strFolder As String = cEwEDatabase.GetWorkingCopyFolder()
            ' The real file's own extension is kept at the end (not ".tmp")
            ' so cDataSourceFactory.GetSupportedType - which determines a
            ' file's data source type purely from Path.GetExtension - still
            ' correctly recognizes the staged copy if it's ever opened
            ' directly by path, e.g. via frmEwE6.LoadEcopathModel. A ".tmp"
            ' extension here made the staged copy invisible to that
            ' detection entirely.
            Dim strTempName As String = "Temp" & Path.GetFileNameWithoutExtension(strRealPath) & "." & Guid.NewGuid().ToString() & Path.GetExtension(strRealPath)
            Dim strTempPath As String = Path.Combine(strFolder, strTempName)

            ' No retry here, deliberately - unlike PublishLocalCopyToSource
            ' (Save), there is no unsaved work at risk on Open, and no
            ' user-facing Retry/Cancel dialog paired with this path. A
            ' retry loop would only add latency before the same eventual
            ' failure, with no better outcome to show for it.
            File.Copy(strRealPath, strTempPath, True)
            Me.m_workingCopyMarker.TryAcquire(strTempPath & ".marker")
            cEwEDatabase.EnsureProcessExitHookRegistered()

            Me.m_strRealFilePath = strRealPath
            Me.m_strLocalWorkingCopyPath = strTempPath
            Return strTempPath
        End Function

        ' A sharing violation (ERROR_SHARING_VIOLATION) or lock violation
        ' (ERROR_LOCK_VIOLATION) is the specific, transient condition worth
        ' retrying - e.g. a cloud-sync client (OneDrive etc.) briefly
        ' holding the real file open to hash or sync it. Other IOExceptions
        ' (disk full, path too long, media removed) are not transient in
        ' the same way and should fail immediately rather than retry
        ' pointlessly.
        ''' <summary>HRESULT for ERROR_SHARING_VIOLATION (0x80070020), as the signed Integer .NET exceptions actually report.</summary>
        Private Const HR_ERROR_SHARING_VIOLATION As Integer = -2147024864
        ''' <summary>HRESULT for ERROR_LOCK_VIOLATION (0x80070021), as the signed Integer .NET exceptions actually report.</summary>
        Private Const HR_ERROR_LOCK_VIOLATION As Integer = -2147024863
        Private Const cPublishRetryAttempts As Integer = 6
        Private Const cPublishRetryDelayMs As Integer = 500

        ''' <summary>
        ''' Sidecar suffix used to stage the new content next to the real
        ''' file before the atomic rename in PublishLocalCopyToSource -
        ''' never written to directly by anything else, so collisions with
        ''' unrelated files/processes aren't a concern.
        ''' </summary>
        Private Const cPublishSidecarSuffix As String = ".publishing"

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Copies the local working copy's current bytes back over the real
        ''' file, unconditionally - no fingerprint check, no conflict
        ''' detection, no prompt. Called once per explicit user Save action
        ''' (see cCore.Save()), never per internal transaction.
        ''' </summary>
        ''' <returns>
        ''' True on success (or if nothing was staged - a no-op still
        ''' counts as success). False if the real file remained locked
        ''' (e.g. by a cloud-sync client) after exhausting all retry
        ''' attempts, or if some other, non-retryable I/O error occurred.
        ''' </returns>
        ''' <remarks>
        ''' <para>Callers must ensure all pending writes are actually
        ''' committed to the local working copy (e.g. via
        ''' CommitTransaction()/SaveChanges()) BEFORE calling this - it
        ''' operates purely on the working copy's current bytes on disk, it
        ''' does not itself flush anything.</para>
        ''' <para>Publishes via copy-to-sidecar, delete-original,
        ''' rename-sidecar-to-original - NOT a direct overwrite. A crash
        ''' mid-copy leaves the real file completely untouched (the sidecar
        ''' is written next to it, never in place of it, until the rename).
        ''' A crash after the delete but before the rename leaves the real
        ''' file genuinely missing - but the fully-intact new content sits
        ''' right next to it as "<name>.publishing", trivially recoverable
        ''' by hand even though there is no automated recovery for this.
        ''' Either way, what the direct-overwrite approach this replaced
        ''' could do - leave the real file's bytes themselves partially
        ''' written and corrupted - is no longer possible: the rename
        ''' itself is a single, atomic filesystem metadata operation on the
        ''' same volume (confirmed for NTFS), not a byte-by-byte copy, so
        ''' it cannot itself be interrupted into a half-done state.</para>
        ''' <para>Retries the whole sequence from the top, mechanically,
        ''' with a short delay, purely to ride out a brief sharing
        ''' violation on the real path (delete or rename, most likely) -
        ''' no UI, no dialog, no awareness of who is holding the file. A
        ''' caller that wants to ask the user to retry after this returns
        ''' False (see cCore.Save()) is responsible for that itself.</para>
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Overridable Function PublishLocalCopyToSource() As Boolean
            If String.IsNullOrEmpty(Me.m_strLocalWorkingCopyPath) OrElse String.IsNullOrEmpty(Me.m_strRealFilePath) Then Return True

            Dim strSidecar As String = Me.m_strRealFilePath & cPublishSidecarSuffix

            For iAttempt As Integer = 1 To cPublishRetryAttempts
                Try
                    ' 1. Write the new content next to the real file, never
                    ' in place of it - the real file is untouched by this
                    ' step regardless of outcome.
                    File.Copy(Me.m_strLocalWorkingCopyPath, strSidecar, True)

                    ' 2. Remove the real file - a safe no-op if it doesn't
                    ' exist yet (first-ever publish to a brand-new path).
                    ' .NET's File.Move has no cross-target-platform
                    ' (net48/net10) overwrite overload, so the delete has
                    ' to happen explicitly, as its own step, before the
                    ' rename below.
                    If File.Exists(Me.m_strRealFilePath) Then File.Delete(Me.m_strRealFilePath)

                    ' 3. Atomic rename, same volume - either this fully
                    ' completes or it doesn't happen at all; there is no
                    ' partially-renamed state for it to be interrupted into.
                    File.Move(strSidecar, Me.m_strRealFilePath)

                    Return True

                Catch ex As IOException When ex.HResult = HR_ERROR_SHARING_VIOLATION OrElse ex.HResult = HR_ERROR_LOCK_VIOLATION
                    If iAttempt = cPublishRetryAttempts Then
                        m_logger.LogWarning(ex, "cEwEDatabase.PublishLocalCopyToSource('{0}'): still locked after {1} attempts.", Me.m_strRealFilePath, cPublishRetryAttempts)
                        Return False
                    End If
                    System.Threading.Thread.Sleep(cPublishRetryDelayMs)

                Catch ex As Exception
                    ' Not a sharing/lock violation - not the transient
                    ' condition this retry loop exists for. Fail
                    ' immediately. Note: if this fires after the delete but
                    ' before the rename, strSidecar still holds the
                    ' complete, correct content - not cleaned up here
                    ' deliberately, since it may be the only remaining copy
                    ' of the user's latest changes.
                    m_logger.LogError(ex, "cEwEDatabase.PublishLocalCopyToSource('{0}'): {1}", Me.m_strRealFilePath, ex.Message)
                    Return False
                End Try
            Next

            Return False
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Releases the working-copy marker (always succeeds immediately)
        ''' and deletes the local working copy, if any. Call from each
        ''' subclass's Close() override, AFTER that subclass has finished
        ''' closing its own connection to the working copy.
        ''' </summary>
        ''' <remarks>
        ''' This delete does NOT retry. A long-standing, still-unresolved
        ''' issue (predating this staging feature entirely) holds a
        ''' database connection's underlying file handle open for the rest
        ''' of THIS process's own lifetime, regardless of Close() - not a
        ''' brief pooling delay a short retry could ride out. A sleep-based
        ''' retry here was tried once already and found to have no effect
        ''' beyond risking an application-shutdown hang, and was removed
        ''' for that reason. If the delete fails, the file is simply left
        ''' behind - safe to do, since the marker above is already released
        ''' regardless, so the orphan sweep will correctly identify and
        ''' clean it up on a later Open() anywhere on this machine, or the
        ''' exit-time sweep will on this process's own clean exit (the one
        ''' point this handle is actually known to release).
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Protected Sub CleanUpLocalWorkingCopy()
            Me.m_workingCopyMarker.Release()
            If String.IsNullOrEmpty(Me.m_strLocalWorkingCopyPath) Then Return
            cEwEDatabase.TryDeleteFileOnce(Me.m_strLocalWorkingCopyPath)
            Me.m_strLocalWorkingCopyPath = ""
            Me.m_strRealFilePath = ""
        End Sub

        Private Shared Function GetWorkingCopyFolder() As String
            Dim strFolder As String = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                cWorkingCopySubfolder)
            System.IO.Directory.CreateDirectory(strFolder) ' no-op if it already exists
            Return strFolder
        End Function

        Private Shared Function TryDeleteFileOnce(strPath As String) As Boolean
            Try
                If Not File.Exists(strPath) Then Return True
                File.Delete(strPath)
                Return True
            Catch
                Return False
            End Try
        End Function

        Private Shared Sub TryDeleteFile(strPath As String)
            Try
                If File.Exists(strPath) Then File.Delete(strPath)
            Catch
            End Try
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Sweeps the working-copy folder for files left behind by a
        ''' crashed session (no live ".marker"). Runs at the start of every
        ''' staging operation. Only ever needs to reason about this same
        ''' machine, since %LocalAppData% is always local.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Shared Sub CleanUpOrphanedWorkingCopies()
            Try
                For Each strFile As String In System.IO.Directory.GetFiles(cEwEDatabase.GetWorkingCopyFolder())
                    If strFile.EndsWith(".marker", StringComparison.OrdinalIgnoreCase) Then Continue For
                    If Not Path.GetFileName(strFile).StartsWith("Temp", StringComparison.OrdinalIgnoreCase) Then Continue For

                    Dim strMarkerFile As String = strFile & ".marker"
                    Using probe As New cEwEExclusivityLock()
                        If probe.TryAcquire(strMarkerFile) Then
                            ' Marker was free - nobody's using this copy; it's an orphan from a crash.
                            probe.Release()
                            cEwEDatabase.TryDeleteFile(strFile)
                            cEwEDatabase.TryDeleteFile(strMarkerFile)
                        End If
                        ' TryAcquire returned False: a live session still holds this copy - leave it alone.
                    End Using
                Next
            Catch
                ' Folder access issue - non-fatal, just skip cleanup this pass
            End Try
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Registers the AppDomain.ProcessExit cleanup hook, once, the first
        ''' time this process actually stages a working copy - a session
        ''' that never opens anything never pays any cost at exit.
        ''' ProcessExit is a base .NET runtime event, not WinForms-specific,
        ''' so this works identically for the net48 ScientificInterface app
        ''' or any other host built on EwECore.
        ''' </summary>
        ''' <remarks>
        ''' Not guaranteed to fire on a hard kill or fatal crash - that's
        ''' fine, CleanUpOrphanedWorkingCopies() (the sweep on the next
        ''' staging operation anywhere on this machine) remains the real
        ''' guarantee for those cases.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Private Shared Sub EnsureProcessExitHookRegistered()
            If s_processExitHookRegistered Then Return
            SyncLock s_processExitHookLock
                If s_processExitHookRegistered Then Return
                AddHandler AppDomain.CurrentDomain.ProcessExit, AddressOf cEwEDatabase.OnProcessExit
                s_processExitHookRegistered = True
            End SyncLock
        End Sub

        Private Shared Sub OnProcessExit(sender As Object, e As EventArgs)
            cEwEDatabase.ScheduleDetachedFolderSweep()
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Schedules one detached script that, once this process's own PID
        ''' disappears from the OS process list, deletes every file in the
        ''' working-copy folder that has no adjacent ".marker" file. Handles
        ''' the case
        ''' where a working copy's underlying connection does not release
        ''' its file handle until the whole process exits, regardless of how
        ''' thoroughly Close() disposes things in-process.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Shared Sub ScheduleDetachedFolderSweep()
            Try
                Dim iPid As Integer = Process.GetCurrentProcess().Id
                Dim strFolder As String = cEwEDatabase.GetWorkingCopyFolder()
                Dim psi As New ProcessStartInfo() With {
                    .UseShellExecute = False,
                    .CreateNoWindow = True,
                    .WindowStyle = ProcessWindowStyle.Hidden
                }

                If RuntimeInformation.IsOSPlatform(OSPlatform.Windows) Then
                    Dim strScriptPath As String = Path.Combine(Path.GetTempPath(), "EwECleanup_" & Guid.NewGuid().ToString("N") & ".bat")
                    Dim strScript As String =
                        "@echo off" & vbCrLf &
                        "for /L %%i in (1,1,180) do (" & vbCrLf & ' ~1/sec, so ~3 minutes cap
                        "    tasklist /FI ""PID eq " & iPid & """ 2>NUL | find """ & iPid & """ >NUL" & vbCrLf &
                        "    if errorlevel 1 (" & vbCrLf & ' PID no longer found - process has exited
                        "        for %%f in (""" & strFolder & "\*"") do (" & vbCrLf &
                        "            echo %%~xf | findstr /I /X "".marker"" >NUL" & vbCrLf &
                        "            if errorlevel 1 if not exist ""%%f.marker"" del /F /Q ""%%f""" & vbCrLf &
                        "        )" & vbCrLf &
                        "        del /F /Q ""%~f0""" & vbCrLf &
                        "        exit /b 0" & vbCrLf &
                        "    )" & vbCrLf &
                        "    ping -n 2 127.0.0.1 >nul" & vbCrLf &
                        ")" & vbCrLf &
                        "del /F /Q ""%~f0""" & vbCrLf
                    File.WriteAllText(strScriptPath, strScript)
                    psi.FileName = strScriptPath

                Else
                    Dim strScriptPath As String = Path.Combine(Path.GetTempPath(), "ewe_cleanup_" & Guid.NewGuid().ToString("N") & ".sh")
                    Dim strScript As String =
                        "#!/bin/sh" & vbLf &
                        "for i in $(seq 1 180); do" & vbLf & ' ~1/sec, so ~3 minutes cap
                        "    if ! kill -0 " & iPid & " 2>/dev/null; then" & vbLf & ' PID no longer exists - process has exited
                        "        for f in """ & strFolder & """/*; do" & vbLf &
                        "            case ""$f"" in" & vbLf &
                        "                *.marker) continue ;;" & vbLf &
                        "            esac" & vbLf &
                        "            [ -f ""$f.marker"" ] || rm -f ""$f""" & vbLf &
                        "        done" & vbLf &
                        "        rm -f ""$0""" & vbLf &
                        "        exit 0" & vbLf &
                        "    fi" & vbLf &
                        "    sleep 1" & vbLf &
                        "done" & vbLf &
                        "rm -f ""$0""" & vbLf
                    File.WriteAllText(strScriptPath, strScript)
                    Try
                        Process.Start(New ProcessStartInfo("chmod", "+x """ & strScriptPath & """") With {.UseShellExecute = False})?.WaitForExit()
                    Catch
                    End Try
                    psi.FileName = strScriptPath
                End If

                Process.Start(psi)
            Catch
                ' Best-effort only - CleanUpOrphanedWorkingCopies() is the guaranteed eventual cleanup path regardless.
            End Try
        End Sub

#End Region ' Open and close

#Region " Compatibility "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Enumberated type, used to indicate database compatibility modes
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Enum eCompatibilityTypes As Integer
            ''' <summary>Combatibility unknown. Most likely the accessed file is not an EwE database.</summary>
            Unknown = 0
            ''' <summary>An older database, too old to be imported.</summary>
            TooOld
            ''' <summary>An older database that can be imported.</summary>
            Importable
            ''' <summary>EwE6 database that is supported.</summary>
            EwE6
            ''' <summary>A database that is of a newer format and is not supported.</summary>
            Future
        End Enum

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' States the level of compatibility with the EwE code.
        ''' </summary>
        ''' <returns>A <see cref="eCompatibilityTypes">database compatibility</see>
        ''' indicator.</returns>
        ''' -------------------------------------------------------------------
        Public Function Compatibility() As eCompatibilityTypes
            Dim sVersion As Single = Me.GetVersion()
            If (sVersion = 0.0!) Then Return eCompatibilityTypes.Unknown
            If (sVersion < cDBVERSION_EWE5_MIN) Then Return eCompatibilityTypes.TooOld
            If (sVersion <= cDBVERSION_EWE5_MAX) Then Return eCompatibilityTypes.Importable
            If (sVersion <= Me.MaxDBVersion) Then Return eCompatibilityTypes.EwE6
            Return eCompatibilityTypes.Future
        End Function

        Public MustOverride Function MaxDBVersion() As Single

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' States whether this database supports being driven through the
        ''' legacy cDBUpdate update chain (see cDatabaseUpdater.RunAllUpdates).
        ''' </summary>
        ''' <remarks>
        ''' The legacy update chain was written for, and is only guaranteed to
        ''' be compatible with, Access/OleDb SQL syntax - several historical
        ''' updates use statements SQLite does not support (ALTER COLUMN, ADD/
        ''' DROP CONSTRAINT, ADD PRIMARY KEY/FOREIGN KEY via ALTER TABLE). This
        ''' defaults to True here so existing (Access) behavior is unchanged;
        ''' override to return False for any database type that should never
        ''' have this update chain run against it.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Overridable Function SupportsLegacyDatabaseUpdates() As Boolean
            Return True
        End Function

#End Region ' Compatibility

#Region " Maintenance "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Compact an EwE database.
        ''' </summary>
        ''' <param name="strFileFrom">Source database to compact.</param>
        ''' <param name="strFileTo">Target database to compact to. If left
        ''' blank, the source database is replaced with a compacted version.</param>
        ''' <returns>True if successful.</returns>
        ''' -------------------------------------------------------------------
        Public MustOverride Function Compact(strFileFrom As String,
                                             strFileTo As String) As eDatasourceAccessType

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns if a compact engine is available for the underlying database.
        ''' </summary>
        ''' <param name="strConnectionFrom">Compact source.</param>
        ''' <param name="strConnectionTo">Compact target.</param>
        ''' <returns>True if a compact engine is available for the underlying
        ''' database.</returns>
        ''' -------------------------------------------------------------------
        Public MustOverride Function CanCompact(strConnectionFrom As String,
                                                strConnectionTo As String) As Boolean

#End Region ' Maintenance

#Region " Connection "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns the current database connection.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public MustOverride Function GetConnection() As IDbConnection

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' States whether there is a database connection that is open.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Function IsConnected() As Boolean
            Dim conn As IDbConnection = Me.GetConnection()

            If (conn Is Nothing) Then Return False
            Return (conn.State = ConnectionState.Open)
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns whether the database can connect to an indicated type.
        ''' </summary>
        ''' <param name="dst">The datasource type to test.</param>
        ''' <returns>True if the OS can connect to a given datasource type.</returns>
        ''' -------------------------------------------------------------------
        Public MustOverride Function CanConnect(dst As eDataSourceTypes) As Boolean

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get a directory associated with this database.
        ''' </summary>
        ''' <seealso cref="Extension"/>
        ''' <seealso cref="FileName"/>
        ''' -------------------------------------------------------------------
        Public MustOverride ReadOnly Property Directory() As String

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get a file name associated with this database, excluding the
        ''' file extension.
        ''' </summary>
        ''' <seealso cref="Extension"/>
        ''' <seealso cref="Directory"/>
        ''' -------------------------------------------------------------------
        Public MustOverride ReadOnly Property FileName() As String

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get a file extension associated with this database.
        ''' </summary>
        ''' <seealso cref="FileName"/>
        ''' <seealso cref="Directory"/>
        ''' -------------------------------------------------------------------
        Public MustOverride ReadOnly Property Extension() As String

#End Region ' Connection

#Region " Transaction "

        ''' <summary>The current transaction, if any.</summary>
        Private m_transaction As IDbTransaction = Nothing

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Begins a transaction for the current <see cref="GetConnection">Connection</see>.
        ''' </summary>
        ''' <returns>True if successful.</returns>
        ''' <seealso cref="RollbackTransaction()"/>
        ''' <seealso cref="CommitTransaction(Boolean)"/>
        ''' -------------------------------------------------------------------
        Public Function BeginTransaction() As Boolean
            If (Me.m_transaction IsNot Nothing) Then Return False
            Try
                Me.m_transaction = Me.GetConnection.BeginTransaction()
                Return True
            Catch ex As Exception
                m_logger.LogError(ex, "cEwEDatabase.BeginTransaction()")
                Return False
            End Try
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Commits a transaction previously initiated via <see cref="BeginTransaction">BeginTransaction</see>.
        ''' </summary>
        ''' <param name="bRollbackOnError">Flag stating whether the transaction needs
        ''' to automatically rollback when the commit process fails.</param>
        ''' <returns>True if the commit operation succeeded.</returns>
        ''' <seealso cref="BeginTransaction()"/>
        ''' <seealso cref="RollbackTransaction()"/>
        ''' -------------------------------------------------------------------
        Public Function CommitTransaction(Optional bRollbackOnError As Boolean = True) As Boolean
            If (Me.m_transaction Is Nothing) Then Return False
            Try
                'Me.ReleaseCachedWriters(True)
                Me.m_transaction.Commit()
                Me.m_transaction.Dispose()
                Me.m_transaction = Nothing
                Return True
            Catch ex As Exception
#If VERBOSE_LEVEL >= 1 Then
                Console.WriteLine("cEwEDatabase: Transaction commit failed: {0}", ex.Message)
#End If
                m_logger.LogError(ex, "CommitTransaction")
                If (bRollbackOnError) Then Me.RollbackTransaction()
            End Try
            Return False
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Commits a transaction to the current <see cref="GetConnection">Connection</see>.
        ''' </summary>
        ''' <returns></returns>
        ''' <seealso cref="BeginTransaction()"/>
        ''' <seealso cref="CommitTransaction(Boolean)"/>
        ''' -------------------------------------------------------------------
        Public Function RollbackTransaction() As Boolean

            If (Me.Transaction Is Nothing) Then Return False
            Try
                'Me.ReleaseCachedWriters(False)
                Me.m_transaction.Rollback()
                Me.m_transaction.Dispose()
                Me.m_transaction = Nothing
                Return True
            Catch ex As Exception
#If VERBOSE_LEVEL >= 1 Then
                Console.WriteLine("cEwEDatabase: Transaction rollback failed: {0}", ex.Message)
#End If
                m_logger.LogError(ex, "RollbackTransaction")
                Return False
            End Try
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Helper method; internally exposes the current active transaction.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Protected Function Transaction() As IDbTransaction
            Return Me.m_transaction
        End Function

#End Region ' Transaction

#Region " DB helper methods "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns an <see cref="IDbCommand"/> for the current DBMS
        ''' </summary>
        ''' <param name="strSQL">Query to create the IDbCommand with.</param>
        ''' <returns>Nothing if an error occurred.</returns>
        ''' -------------------------------------------------------------------
        Protected Overridable Function CreateDBCommand(strSQL As String) As IDbCommand

            Dim conn As IDbConnection = Me.GetConnection()
            Dim cmd As IDbCommand = Nothing

            Try
                If TypeOf conn Is OleDbConnection Then
                    cmd = New OleDbCommand(strSQL, DirectCast(conn, OleDbConnection), DirectCast(Me.Transaction(), OleDbTransaction))
                Else
                    cmd = New SqlCommand(strSQL, DirectCast(conn, SqlConnection), DirectCast(Me.Transaction(), SqlTransaction))
                End If
                Return cmd
            Catch ex As Exception
                m_logger.LogError(ex, "cEwEDatabase.CreateDBCommand(" & strSQL & ")")
                Return Nothing
            End Try

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns a <see cref="IDataReader"/> with a collection
        ''' of readonly records from the currently open connection.
        ''' <seealso cref="ReleaseReader"/>
        ''' </summary>
        ''' <param name="strSQL">The query to obtain the records.</param>
        ''' <returns></returns>
        ''' <remarks>The obtained IDataReader should be released via <see cref="ReleaseReader"/>.</remarks>
        ''' -------------------------------------------------------------------
        Public Overridable Function GetReader(strSQL As String) As IDataReader

            Dim reader As IDataReader = Nothing
            Try
                Using command As IDbCommand = Me.CreateDBCommand(strSQL)
                    reader = command.ExecuteReader()
                End Using
            Catch ex As Exception
#If VERBOSE_LEVEL >= 1 Then
                Console.WriteLine("GetReader error: {0}", ex.Message)
#End If
                m_logger.LogError(ex, "cEwEDatabase.GetReader(" & strSQL & ")")
                reader = Nothing
            End Try
            Return reader

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Releases the set of readonly records previously obtained by calling
        ''' <see cref="GetReader"/>.
        ''' <seealso cref="GetReader"/>
        ''' </summary>
        ''' <param name="reader">The <see cref="IDataReader"/> to release.</param>
        ''' <returns>True if successful.</returns>
        ''' -------------------------------------------------------------------
        Public Overridable Function ReleaseReader(reader As IDataReader) As Boolean
            Try
                reader.Close()
            Catch ex As Exception
                m_logger.LogError(ex, "cEwEDatabase.ReleaseReader()")
                Debug.Assert(False, Me.ToString & ".ReleaseReader() Error: " & ex.Message)
                Return False
            End Try
            Return True
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns a <see cref="cEwEDbWriter"/> for
        ''' the given table in the database.
        ''' <seealso cref="ReleaseWriter"/>
        ''' </summary>
        ''' <param name="strTable">The table to connect the EwEDbWriter to.</param>
        ''' -------------------------------------------------------------------
        Public Overridable Function GetWriter(strTable As String) As IEwEDbWriter

            Dim key As String = strTable.ToLower()
            Dim writer As IEwEDbWriter = Nothing
            Dim bIsValid As Boolean = False

            ' The writer may have perished due to a rollback 
            If (writer IsNot Nothing) Then
                bIsValid = writer.IsConnected()
            End If

            ' No valid writer? 
            If (Not bIsValid) Then
                ' #Ok: create a new one
                writer = New cEwEDbWriter(Me, strTable)
            End If

            writer.RefCount += 1
            Return writer

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Releases a writer previously created via <see cref="GetWriter"/>.
        ''' <seealso cref="GetWriter"/>
        ''' </summary>
        ''' <param name="writer">The writer to release</param>
        ''' <param name="bSaveChanges">States whether changes should be written (true) or discarded (false).</param>
        ''' <returns>True if successful.</returns>
        ''' -------------------------------------------------------------------
        Public Overridable Function ReleaseWriter(writer As IEwEDbWriter, Optional bSaveChanges As Boolean = True) As Boolean

            Dim bSuccess As Boolean = False

            writer.RefCount -= 1
            bSuccess = writer.Disconnect(bSaveChanges)
            writer.Dispose()

            Return bSuccess
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns a scalar value from the current open connection.
        ''' </summary>
        ''' <param name="strSQL">The query to execute.</param>
        ''' <param name="objDefault">A default to return in case a value could not be returned.</param>
        ''' <returns>The scalar value returned from the query.</returns>
        ''' -------------------------------------------------------------------
        Public Overridable Function GetValue(strSQL As String, Optional objDefault As Object = Nothing) As Object

            Dim value As Object = objDefault
            Try
                Using command As IDbCommand = Me.CreateDBCommand(strSQL)
                    value = command.ExecuteScalar()
                    If Convert.IsDBNull(value) Then
                        value = objDefault
                    End If
                End Using
            Catch ex As Exception
#If VERBOSE_LEVEL >= 2 Then
                Console.WriteLine("** DB error '{0}' on query '{1}'", ex.Message, strSQL)
#End If
                m_logger.LogError(ex, "cEwEDatabase.GetValue(" & strSQL & ")")
            End Try
            Return value
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Obtains an <see cref="IDataAdapter"/> for the current open connection.
        ''' <seealso cref="ReleaseAdapter"/>
        ''' </summary>
        ''' <param name="strSQL">The SQL query to obtain the adaper for.</param>
        ''' <returns>An <see cref="IDataAdapter"/> if successful, or Nothing if 
        ''' an error occurred.</returns>
        ''' <remarks>
        ''' <para>The obtained IDataAdapter should be released via 
        ''' <see cref="ReleaseAdapter"/>.</para></remarks>
        ''' -------------------------------------------------------------------
        Public Overridable Function GetAdapter(strSQL As String) As IDataAdapter

            Dim cmd As IDbCommand = Me.CreateDBCommand(strSQL)
            Try
                If TypeOf cmd Is OleDbCommand Then
                    Return New OleDbDataAdapter(DirectCast(cmd, OleDbCommand))
                Else
                    Return New SqlDataAdapter(DirectCast(cmd, SqlCommand))
                End If
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
            End Try

            Return Nothing

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Releases an <see cref="IDataAdapter"/> previously obtained via
        ''' <see cref="GetAdapter"/>.
        ''' </summary>
        ''' <param name="adapter">The <see cref="IDataAdapter"/> to release.</param>
        ''' <returns>True if successful.</returns>
        ''' -------------------------------------------------------------------
        Public Overridable Function ReleaseAdapter(ByRef adapter As IDataAdapter) As Boolean
            ' Nothing to do
            Return True
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Executes a SQL command that does not return any information.
        ''' </summary>
        ''' <param name="strSQL">The query to execute. When SQLParams is provided,
        ''' this should contain the provider-appropriate positional placeholder
        ''' syntax for the target connection (e.g. "?" for OleDb) in place of any
        ''' parameter values.</param>
        ''' <param name="SQLParams">Optional array of values to bind as real,
        ''' typed command parameters, in the same order as the placeholders in
        ''' strSQL. Use this instead of formatting values directly into the SQL
        ''' text - embedding numbers or dates as string literals leaves them open
        ''' to being misinterpreted by the database engine's own locale-sensitive
        ''' text-to-value conversion (e.g. a decimal point being read as a
        ''' thousands separator on a non-US-formatted machine). A Nothing element
        ''' is bound as DBNull.</param>
        ''' <returns>True if successful.</returns>
        ''' -------------------------------------------------------------------
        Public Overridable Function Execute(strSQL As String, Optional SQLParams As Object() = Nothing) As Boolean

            Dim bSucces As Boolean = True
            Try
                Using command As IDbCommand = Me.CreateDBCommand(strSQL)
                    If SQLParams IsNot Nothing Then
                        For Each objParam In SQLParams
                            Dim param As IDbDataParameter = command.CreateParameter()
                            param.Value = If(objParam, DBNull.Value)
                            command.Parameters.Add(param)
                        Next
                    End If
                    command.ExecuteNonQuery()
                End Using
            Catch ex As Exception
#If VERBOSE_LEVEL >= 2 Then
                Console.WriteLine("* DB exception '{0}' on '{1}'", ex.Message, strSQL)
#End If
                m_logger.LogError(ex, "cEwEDatabase.Execute(" & strSQL & ")")
                bSucces = False
            End Try
            Return bSucces

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns the name of a primary key of a table.
        ''' </summary>
        ''' <param name="strTable">The table name to obtain the primary key for.</param>
        ''' <returns>A name, or an empty string when no primary key was found.</returns>
        ''' <remarks>
        ''' After http://www.koders.com/csharp/fidE6A0EFDE719732D025C3D41E95CC26214E50188C.aspx
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Overridable Function GetPkKeyName(strTable As String) As String

            Dim conn As IDbConnection = Me.GetConnection()
            Dim dtKeys As DataTable = Nothing
            Dim strPKKey As String = ""

            ' Execute oledb variant
            If (TypeOf conn Is OleDbConnection) Then

                Dim cdb As OleDbConnection = DirectCast(conn, OleDbConnection)
                ' Get PK keys schema information for entire DB
                dtKeys = cdb.GetOleDbSchemaTable(OleDbSchemaGuid.Primary_Keys, New Object() {Nothing, Nothing, strTable})
                ' Sanity checks, pk may not be defined
                If (dtKeys.Rows.Count = 0) Then Return strPKKey
                ' Return whatever was found
                '   JS: could be that 'COLUMN_NAME' should be used here!!
                strPKKey = CStr(dtKeys.Rows(0)("PK_NAME"))
                'strPKKey = CStr(drKeys(0)("COLUMN_NAME"))
            End If

            If (TypeOf conn Is SqlConnection) Then
                ' Not implemented yet
                Throw New NotImplementedException("cEwEDatabase.GetPkKeyName() not implemented for SqlConnections")
            End If

            Return strPKKey

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns the name of a foreign key between a given column in a table to 
        ''' another table.
        ''' </summary>
        ''' <param name="strTableFrom">The table where the foreign key is defined.</param>
        ''' <param name="strColumn">The column in the <paramref name="strTableFrom">source table</paramref>.</param>
        ''' <param name="strTableTo">The table where the foreign key links to.</param>
        ''' <returns>A name, or an empty string when no foreign key was found.</returns>
        ''' -------------------------------------------------------------------
        Public Overridable Function GetFkKeyName(strTableFrom As String,
                                                 strTableTo As String,
                                                 strColumn As String) As String

            Dim conn As IDbConnection = Me.GetConnection()
            Dim dtKeys As DataTable = Nothing
            Dim strFKKey As String = ""

            ' Execute oledb variant
            If (TypeOf conn Is OleDbConnection) Then

                Try
                    Dim cdb As OleDbConnection = DirectCast(conn, OleDbConnection)
                    ' Get PK keys schema information for entire DB
                    dtKeys = cdb.GetOleDbSchemaTable(OleDbSchemaGuid.Foreign_Keys, New Object() {Nothing, Nothing, strTableFrom})
                    ' Sanity checks, pk may not be defined
                    If (dtKeys.Rows.Count = 0) Then Return strFKKey
                    For Each drow As DataRow In dtKeys.Rows
                        If (String.Compare(CStr(drow("PK_TABLE_NAME")), strTableFrom) = 0) And
                           (String.Compare(CStr(drow("FK_TABLE_NAME")), strTableTo) = 0) And
                           (String.Compare(CStr(drow("FK_COLUMN_NAME")), strColumn) = 0) Then
                            strFKKey = CStr(drow("FK_NAME"))
                            Exit For
                        End If
                    Next
                Catch ex As Exception
                End Try
            End If

            If (TypeOf conn Is SqlConnection) Then
                ' Not implemented yet
                Throw New NotImplementedException("cEwEDatabase.GetFkKeyName() not implemented for SqlConnections")
            End If

            Return strFKKey

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns the name of an index for a given column in a table.
        ''' </summary>
        ''' <param name="strTable"></param>
        ''' <param name="strColumn">The column to remove the index from, if any.</param>
        ''' <returns>A name, or an empty string when no idnex was found.</returns>
        ''' -------------------------------------------------------------------
        Public Overridable Function GetIndexName(strTable As String,
                                                 strColumn As String) As String

            Dim conn As IDbConnection = Me.GetConnection()
            Dim dtIndexes As DataTable = Nothing
            Dim strIndex As String = ""

            ' Execute oledb variant
            If (TypeOf conn Is OleDbConnection) Then

                Try
                    Dim cdb As OleDbConnection = DirectCast(conn, OleDbConnection)
                    ' Get PK keys schema information for entire DB
                    dtIndexes = cdb.GetSchema("Indexes")
                    ' Sanity checks, pk may not be defined
                    If (dtIndexes.Rows.Count = 0) Then Return strIndex
                    For Each drow As DataRow In dtIndexes.Rows
                        If (String.Compare(CStr(drow("TABLE_NAME")), strTable) = 0) And
                           (String.Compare(CStr(drow("COLUMN_NAME")), strColumn) = 0) Then
                            strIndex = CStr(drow("INDEX_NAME"))
                            Exit For
                        End If
                    Next
                Catch ex As Exception
                End Try
            End If

            If (TypeOf conn Is SqlConnection) Then
                ' Not implemented yet
                Throw New NotImplementedException("cEwEDatabase.GetIndexName() not implemented for SqlConnections")
            End If

            Return strIndex

        End Function

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Helper method, reads data from a column that may not exist. In that case,
        ''' an optional default value is returned
        ''' </summary>
        ''' <param name="reader">The <see cref="IDataReader">IDataReader</see> to read from.</param>
        ''' <param name="strField">The name of the DB field (column) to read.</param>
        ''' <param name="objValueDefault">A default value to return if the field could not be read.</param>
        ''' <param name="objValueIgnore">Value to interpret as 'no value'. When encountered, the default value will be returned.</param>
        ''' <returns>The value of the requested column, or the provided default if an error occurred.</returns>
        ''' -----------------------------------------------------------------------
        Public Function ReadSafe(reader As IDataReader,
                                 strField As String,
                                 Optional objValueDefault As Object = Nothing,
                                 Optional objValueIgnore As Object = CSng(-9999)) As Object

            Dim objResult As Object = Nothing

            If (reader Is Nothing) Then Return objValueDefault

            Try
                If Me.HasColumn(reader, strField) Then
                    objResult = reader.Item(strField)
                End If
            Catch ex As IndexOutOfRangeException
                ' Ugh
            Catch ex As InvalidOperationException
                'Console.WriteLine("DB: field '{0}' has no value, returning provided default '{1}'", strField, objValueDefault)
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
                Console.WriteLine("DB: Exception {2} occurred while accessing field '{0}', returning provided default '{1}'", strField, objValueDefault, ex.ToString)
            End Try

            If (objResult Is Nothing) Then
                objResult = objValueDefault
            ElseIf (objValueIgnore IsNot Nothing) _
                And Not (Convert.IsDBNull(objResult)) _
                And Not (Convert.IsDBNull(objValueIgnore)) Then

                ' Compare ignore values
                If TypeOf objResult Is String Then
                    Try
                        If (String.Compare(CStr(objResult), Convert.ToString(objValueIgnore), True) = 0) Then
                            objResult = objValueDefault
                        End If
                    Catch ex As Exception
                    End Try
                ElseIf TypeOf objResult Is Boolean Then
                    Try
                        If (CBool(objResult) = Convert.ToBoolean(objValueIgnore)) Then
                            objResult = objValueDefault
                        End If
                    Catch ex As Exception
                    End Try
                Else
                    Try
                        If (CSng(objResult) = Convert.ToSingle(objValueIgnore)) Then
                            objResult = objValueDefault
                        End If
                    Catch ex As Exception
                    End Try
                End If

            End If

            If (Convert.IsDBNull(objResult)) Then
                objResult = objValueDefault
            End If

            Return objResult
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Safely drop a column by first removing any indexes on that column.
        ''' </summary>
        ''' <param name="strTable">The table to remove the column from.</param>
        ''' <param name="strColumn">The name of the column to remove.</param>
        ''' <returns>True if successful.</returns>
        ''' -------------------------------------------------------------------
        Public Overridable Function DropColumn(strTable As String, strColumn As String) As Boolean

            Dim bSuccess As Boolean = True

            Dim strIndex As String = Me.GetIndexName(strTable, strColumn)
            If (Not String.IsNullOrWhiteSpace(strIndex)) Then
                bSuccess = bSuccess And Me.Execute("DROP Index " & strIndex & " ON " & strTable)
            End If
            Return bSuccess And Me.Execute("ALTER TABLE " & strTable & " DROP COLUMN " & strColumn)

        End Function

#End Region ' DB helper methods

#Region " Internals "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Obtains a <see cref="DataSet">DataSet</see> for modifying records.
        ''' </summary>
        ''' <param name="adapter">The <see cref="IDataAdapter">IDataAdapter</see> to fill the <see cref="DataSet">DataSet</see> from.</param>
        ''' <param name="strTable">The name of the table to fill the <see cref="DataSet">DataSet</see> from.</param>
        ''' <returns>The <see cref="DataSet">DataSet</see> if successful, or Nothing if an error occurred.</returns>
        ''' <remarks>The obtained <see cref="DataSet">DataSet</see> should be released via <see cref="ReleaseDataSet">ReleaseWriter</see>.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Overridable Function GetDataSet(adapter As IDataAdapter, strTable As String) As DataSet
            Dim ds As New DataSet()
            Try
                adapter.Fill(ds)
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
                ds = Nothing
            End Try
            Return ds
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Commits all the pending changes in the <see cref="DataSet">DataSet</see>. This will
        ''' leave the DataSet open for further operations.
        ''' </summary>
        ''' <param name="dset">The <see cref="DataSet">DataSet</see> to commit</param>
        ''' <param name="adapter">The <see cref="IDataAdapter">OleDbDataAdapter</see> to write to the database</param>
        ''' <param name="strTable">The table to update</param>
        ''' <returns>True if successful</returns>
        ''' -------------------------------------------------------------------
        Public Overridable Function CommitDataSet(dset As DataSet, adapter As IDataAdapter, strTable As String) As Boolean
            Dim bSucces As Boolean = True

            ' Is adapter specified?
            If (adapter Is Nothing) Then
                ' #No adapter = no need to update database. Done
                Return True
            End If

            ' Table name optional, no need to Assert
            Try
                adapter.Update(dset)
            Catch ex As Exception
                ' Woops
                bSucces = False
            End Try
            ' Report result
            Return bSucces

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Releases a <see cref="DataSet">DataSet</see> previously obtained via 
        ''' <see cref="GetDataSet">GetDataSet</see>.
        ''' </summary>
        ''' <param name="dset">The writer to release.</param>
        ''' <param name="adapter">The <see cref="IDataAdapter">IDataAdapter</see>
        ''' to commit any changes to. If this parameter is left blank, any changes made to
        ''' the dataset and its data are discarded.</param>
        ''' <param name="strTable">The name of the table to update.</param>
        ''' <returns>True if successful.</returns>
        ''' -------------------------------------------------------------------
        Public Overridable Function ReleaseDataSet(dset As DataSet, Optional adapter As IDataAdapter = Nothing, Optional strTable As String = "") As Boolean
            Return Me.CommitDataSet(dset, adapter, strTable)
        End Function

        Public Function HasColumn(reader As IDataReader, strColumnName As String) As Boolean
            If (reader Is Nothing) Then Return False
            reader.GetSchemaTable().DefaultView.RowFilter = "ColumnName= '" + strColumnName + "'"
            Return (reader.GetSchemaTable().DefaultView.Count > 0)
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns whether a given table exists in the open connection.
        ''' </summary>
        ''' <param name="strTableName">The table to check.</param>
        ''' <returns>True if the table exists in the open connection.</returns>
        ''' -------------------------------------------------------------------
        Public Overridable Function HasTable(strTableName As String) As Boolean

            If (Me.GetConnection() Is Nothing) Then Return False

            If (TypeOf Me.GetConnection() Is OleDbConnection) Then
                Dim dtSchema As DataTable = DirectCast(Me.GetConnection(), OleDbConnection).GetOleDbSchemaTable(OleDb.OleDbSchemaGuid.Tables, New Object() {Nothing, Nothing, strTableName, "TABLE"})
                Return dtSchema.Rows.Count > 0
            Else
                Throw New NotImplementedException("HasTable not implemented for SQL databases")
            End If
            Return False

        End Function

#End Region ' Internals

#Region " EwE versioning "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns the current version of the connected EwE database.
        ''' </summary>
        ''' <returns>
        ''' A Single value with the version latest version number of the connected database.
        ''' </returns>
        ''' -------------------------------------------------------------------
        Public Function GetVersion() As Single

            If Me.m_sVersion = 0.0 Then
                Try
                    ' Try EwE6 version first
                    Me.m_sVersion = CSng(Me.GetValue("Select Max(Version) FROM [UpdateLog]"))
                    If (Me.m_sVersion = 0.0) Then
                        ' Try EwE5 version
                        Me.m_sVersion = CSng(Me.GetValue("Select Max(Version) FROM [Database specifications]"))
                    End If
                Catch ex As Exception
                    Me.m_sVersion = 0.0
                End Try
            End If
            Return Me.m_sVersion

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns the current version of the connected EwE database.
        ''' </summary>
        ''' <returns>
        ''' A Single value with the version latest version number of the connected database.
        ''' </returns>
        ''' -------------------------------------------------------------------
        Public Function GetEwEVersion() As String

            If (String.IsNullOrWhiteSpace(Me.m_strEwEversion)) Then
                Dim sVersion As Single = GetVersion()
                Try
                    ' Try EwE6 version first
                    Using reader As IDataReader = Me.GetReader("SELECT EwEVersion FROM UpdateLog WHERE Version=" & cStringUtils.FormatSingle(sVersion))
                        While reader.Read()
                            Me.m_strEwEversion = cStringUtils.Localize(My.Resources.CoreDefaults.GENERIC_VERSION, CStr(Me.ReadSafe(reader, "EwEVersion", "")))
                        End While
                    End Using

                    ' If that didn't work
                    If String.IsNullOrWhiteSpace(Me.m_strEwEversion) Then
                        If (sVersion > 6.0 And sVersion < 7) Then
                            Me.m_strEwEversion = My.Resources.CoreDefaults.GENERIC_VERSION_PREVIOUS
                        Else
                            Me.m_strEwEversion = My.Resources.CoreDefaults.GENERIC_VERSION_ANCIENT
                        End If
                    End If

                Catch ex As Exception
                    Me.m_strEwEversion = My.Resources.CoreDefaults.GENERIC_VERSION_PREVIOUS
                End Try
            End If
            Return Me.m_strEwEversion

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Updates the version of the database
        ''' </summary>
        ''' <param name="sVersion">The version to set.</param>
        ''' <param name="strRemark">The remark to add to the update.</param>
        ''' <returns>True if successful</returns>
        ''' <remarks>This method only allows setting the version on an EwE6 database.</remarks>
        ''' -------------------------------------------------------------------
        Public Function SetVersion(sVersion As Single, strRemark As String) As Boolean

            Dim version As Version = cAssemblyUtils.GetVersion()
            Dim dtNow As Date = Date.Now()
            Dim strSQL As String = ""
            Dim SQLParams As Object()

            If (sVersion < 6.120003!) Then
                strSQL = "INSERT INTO UpdateLog ([Version], [Remark], [Date]) VALUES (?, ?, ?)"
                SQLParams = New Object() {sVersion, strRemark, dtNow.Date}
            Else
                strSQL = "INSERT INTO UpdateLog ([Version], [Remark], [Date], [EwEVersion]) VALUES (?, ?, ?, ?)"
                SQLParams = New Object() {sVersion, strRemark, dtNow.Date, version.ToString()}
            End If
            Dim bSucces As Boolean = True
            Try
                bSucces = Me.Execute(strSQL, SQLParams)
                Me.m_sVersion = sVersion
            Catch ex As Exception
                bSucces = False
            End Try
            Return bSucces

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Extract the major version number from a given version number.
        ''' </summary>
        ''' <param name="sVersion">The version number to examine.</param>
        ''' <returns>The major version number of the given version number.</returns>
        ''' <remarks>
        ''' <para>'6.0' returns '6'</para>
        ''' <para>'2.93' returns '2'</para>
        ''' <para>'-4.4' returns '4'</para>
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Shared Function GetMajorVersion(sVersion As Single) As Single
            Return CSng(Math.Sign(sVersion) * Math.Floor(Math.Abs(sVersion)))
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Extract the minor version number from a given version number.
        ''' </summary>
        ''' <param name="sVersion">The version number to examine.</param>
        ''' <returns>The minor version number of the given version number.</returns>
        ''' <remarks>
        ''' <para>'6.0' returns '0.0'</para>
        ''' <para>'2.93' returns '0.93'</para>
        ''' <para>'-4.4' returns '0.4'</para>
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Shared Function GetMinorVersion(sVersion As Single) As Single
            Dim sAbsVersion As Single = Math.Abs(sVersion)
            Return CSng(sAbsVersion - Math.Floor(sAbsVersion))
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Database change log item.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Class cHistoryItem
            Private m_strVersion As String
            Private m_strComments As String
            Private m_date As DateTime

            ''' <summary>
            ''' 
            ''' </summary>
            ''' <param name="strVersion"></param>
            ''' <param name="strComments"></param>
            ''' <param name="strDate"></param>
            ''' <remarks></remarks>
            Friend Sub New(strVersion As String, strComments As String, strDate As String)
                Me.m_strVersion = strVersion
                Me.m_strComments = strComments
                Me.m_date = Date.Parse(strDate)
            End Sub

            ''' -------------------------------------------------------------------
            ''' <summary>
            ''' Get the version number of a particular history item.
            ''' </summary>
            ''' -------------------------------------------------------------------
            Public ReadOnly Property Version() As String
                Get
                    Return Me.m_strVersion
                End Get
            End Property

            ''' -------------------------------------------------------------------
            ''' <summary>
            ''' Get the comments to a particular history item.
            ''' </summary>
            ''' -------------------------------------------------------------------
            Public ReadOnly Property Comments() As String
                Get
                    Return Me.m_strComments
                End Get
            End Property

            ''' -------------------------------------------------------------------
            ''' <summary>
            ''' Get the date of a particular history item.
            ''' </summary>
            ''' -------------------------------------------------------------------
            Public ReadOnly Property [Date]() As DateTime
                Get
                    Return Me.m_date
                End Get
            End Property

        End Class

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns the change log of the database.
        ''' </summary>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Public Function GetHistory() As cHistoryItem()
            Dim lHistory As New List(Of cHistoryItem)
            Dim item As cHistoryItem = Nothing
            Using r As IDataReader = Me.GetReader("SELECT * FROM UpdateLog ORDER BY Date ASC, Version ASC")
                While r.Read
                    Try
                        item = New cHistoryItem(CStr(r("Version")), CStr(r("Remark")), CStr(r("Date")))
                        lHistory.Add(item)
                    Catch ex As Exception
                        ' Whoah! Unable to parse the date?!
                    End Try
                End While
            End Using
            Return lHistory.ToArray
        End Function

#End Region ' EwE versioning

    End Class

End Namespace
