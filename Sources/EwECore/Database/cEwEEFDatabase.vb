' SPDX-License-Identifier: EUPL-1.2
' Entity Framework implementation of cEwEDatabase for SQLite/EF
Imports System.Data
Imports System.IO
Imports System.Reflection
Imports Eii.Ecopath.Storage
Imports EwECore.DataSources
Imports EwEUtils.Utilities
Imports Microsoft.EntityFrameworkCore
Imports Microsoft.Extensions.Logging
#If NET48 Then
Imports System.Data.SQLite
#Else
Imports Microsoft.Data.Sqlite
#End If

Namespace Database
    Public Class cEwEEFDatabase
        Inherits cEwEDatabase

        Private m_dbContext As EwEDbContext = Nothing
        Private m_strFileName As String = ""
        ''' <summary>Tracks every command created via CreateDBCommand, so Close() can force-dispose all of them regardless of whether individual callers/readers ever did.</summary>
        Private ReadOnly m_openCommands As New List(Of IDbCommand)
        ''' <summary>Tracks every reader created via GetReader(), so Close() can force-dispose all of them regardless of whether the caller ever did - see Close()'s own comment on disposal order.</summary>
        Private ReadOnly m_openReaders As New List(Of IDataReader)

        ' Private helper to ensure DbContext is initialized and migrated
        Private Sub EnsureDbContext(strDatabase As String)
            EwEDbContext.DefaultSQLiteFilePath = strDatabase
        #If NET48 Then
            ' net48 cannot run EF Core in-process (Eii.Ecopath.Storage's EwEDbContext
            ' is net10.0-only there) - shell out to a self-contained net10.0 tool to
            ' apply any pending migrations BEFORE this process's own EF6 context
            ' ever touches the file.
            cSqliteMigrator.MigrateDatabase(strDatabase)
        #End If
            If m_dbContext Is Nothing Then
                m_dbContext = New EwEDbContext()
            End If
        #If NET48 Then
            m_dbContext.Database.Initialize(True)
        #Else
            m_dbContext.Database.Migrate()
        #End If
        End Sub

        Public Function GetDbContext() As EwEDbContext
            Return m_dbContext
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Create a new SQLite database, seeded from an embedded starter
        ''' resource (mirrors cEwEAccessDatabase.Create - EnsureDbContext's
        ''' Migrate()/Initialize(True) call needs the file to already exist;
        ''' it does not create a schema from nothing).
        ''' </summary>
        ''' <param name="strDatabase">The file name of the .ewesqlite to create.</param>
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
                Case eDataSourceTypes.Sqlite
                    strSource = "EwE6.sqlite"
                Case Else
                    datResult = eDatasourceAccessType.Failed_UnknownType
                    m_logger.LogInformation("Create DB: cannot determine format")
            End Select

            If (datResult = eDatasourceAccessType.Success) Then

                ' Copy the starter resource to strDatabase FIRST - EnsureDbContext's
                ' Migrate()/Initialize(True) call (via cSqliteMigrator on net48)
                ' needs the file to already physically exist; it brings an
                ' existing file up to date, it does not create one from nothing.
                If cResourceUtils.SaveResourceToFile(strSource, strDatabase, bOverwrite, Assembly.GetExecutingAssembly()) Then
                    Try
                        EnsureDbContext(strDatabase)
                        m_strFileName = strDatabase

                        Me.Execute("UPDATE EcopathModel SET Name = ?, Author = ? WHERE ModelID = 1",
                                   New Object() {strModelName, strAuthor})
                        Try
                            ' Egg - over-easy but slightly obfuscated ;)
                            If strModelName.ToLower().Contains(cStringUtils.Shift("Dbsm!Xbmufst").ToLower()) Then
                                Me.Execute("UPDATE EcopathGroup SET GroupName = ? WHERE GroupID = 1",
                                           New Object() {cStringUtils.Shift("Dijdlfo!tiju")})
                                Me.Execute("UPDATE EcopathFleet SET FleetName = ? WHERE FleetID = 1",
                                           New Object() {cStringUtils.Shift("Tfbm!cbtifst")})
                            End If
                        Catch ex As Exception
                            ' Do not let eggs make the pot explode
                            m_logger.LogError(ex, "Create DB: found a rotten egg: " & ex.Message)
                        End Try

                        Me.Close()
                        datResult = eDatasourceAccessType.Success

                    Catch ex As Exception
                        m_logger.LogError(ex, "Create DB: Exception when updating model name: " & ex.Message)
                        datResult = eDatasourceAccessType.Failed_Unknown
                    End Try
                Else
                    ' Unable to write to target location
                    m_logger.LogError("Create DB: Unable to save to target location " & strDatabase)
                    datResult = eDatasourceAccessType.Failed_CannotSave
                End If
            End If
            Return datResult

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Save the current database to a new destination, and continue
        ''' operating on that new destination.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overrides Function SaveAs(strDatabaseTo As String, strModelName As String, Optional bOverwrite As Boolean = False, Optional databaseType As eDataSourceTypes = eDataSourceTypes.NotSet) As eDatasourceAccessType
            Try
                If File.Exists(strDatabaseTo) AndAlso Not bOverwrite Then
                    Return eDatasourceAccessType.Failed_CannotSave
                End If

                Dim bIsStaged As Boolean = Not String.IsNullOrEmpty(Me.m_strLocalWorkingCopyPath)

                ' Write the current data to the new target - this becomes
                ' the new real/source location. If this session was never
                ' staged (EnableLocalStaging was False, or this is a
                ' read-only session), m_dbContext is already connected
                ' directly to m_strFileName, so that IS the up-to-date data
                ' to copy from - there is no separate working copy to read
                ' instead.
                File.Copy(If(bIsStaged, Me.m_strLocalWorkingCopyPath, Me.m_strFileName), strDatabaseTo, True)

                If Not bIsStaged Then
                    ' Not staged: reconnect directly to the new target,
                    ' exactly as a normal non-staged Open() would - no
                    ' working copy, matching this session's existing
                    ' unprotected mode.
                    EnsureDbContext(strDatabaseTo)
                    m_strFileName = strDatabaseTo
                    Return eDatasourceAccessType.Success
                End If

                ' Re-stage: continue working locally against a fresh working
                ' copy of the new target, exactly as a normal Open() would.
                ' SaveAs re-points this instance without going through
                ' Open(), so this has to be handled here too.
                Me.CleanUpLocalWorkingCopy()
                Dim strWorkingCopy As String = Me.StageLocalWorkingCopy(strDatabaseTo)

                EnsureDbContext(strWorkingCopy)
                m_strFileName = strDatabaseTo
                Return eDatasourceAccessType.Success
            Catch ex As Exception
                Return eDatasourceAccessType.Failed_Unknown
            End Try
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Open a connection to a SQLite database. There is no lock of any
        ''' kind - if another session already has this file open, this call
        ''' succeeds regardless; avoiding that conflict is entirely the
        ''' caller's own responsibility.
        ''' </summary>
        ''' <remarks>
        ''' A write-intent open (bReadOnly = False, the normal case) stages a
        ''' local working copy when EnableLocalStaging is on, connecting to
        ''' that rather than the real file directly - see
        ''' cEwEDatabase.StageLocalWorkingCopy(). This protects the eventual
        ''' Save from a cloud-sync client (OneDrive etc.) rewriting the real
        ''' file out from under a live connection. There is no lock of any
        ''' kind involved - multi-user/multi-session conflicts are entirely
        ''' the caller's own responsibility.
        ''' A caller-requested read-only open (the older/incompatible DB
        ''' version case - the only remaining use of bReadOnly), or any
        ''' caller that hasn't opted into EnableLocalStaging at all (e.g.
        ''' EwERunConsole), connects directly to the real file, exactly as
        ''' before this feature existed.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Overrides Function Open(strDatabase As String, Optional databaseType As eDataSourceTypes = eDataSourceTypes.NotSet, Optional bReadOnly As Boolean = False) As eDatasourceAccessType
            Try
                If Not File.Exists(strDatabase) Then
                    Return eDatasourceAccessType.Failed_FileNotFound
                End If

                Dim strEffectivePath As String = strDatabase

                If Not bReadOnly AndAlso cEwEDatabase.EnableLocalStaging Then
                    Try
                        strEffectivePath = Me.StageLocalWorkingCopy(strDatabase)
                    Catch ex As Exception
                        m_logger.LogError(ex, "cEwEEFDatabase.Open('{0}') failed to stage a local working copy: {1}", strDatabase, ex.Message)
                        Return eDatasourceAccessType.Failed_Unknown
                    End Try
                End If

                EnsureDbContext(strEffectivePath)
                m_strFileName = strDatabase ' Name/Directory/FileName/Extension keep reporting the REAL path
                Me.IsReadOnly = bReadOnly
                GetOpenConnection()

                Return eDatasourceAccessType.Opened

            Catch ex As Exception
                Me.CleanUpLocalWorkingCopy()
                m_logger.LogError(ex, "cEwEEFDatabase.Open('{0}') failed: {1}", strDatabase, ex.Message)
                Return eDatasourceAccessType.Failed_Unknown
            End Try
        End Function

        Public Overrides Sub Close()
            ' Disposal order matters here, and previously ran backwards:
            ' the connection was being closed BEFORE its own commands/
            ' readers were disposed. Per System.Data.SQLite's own
            ' documented requirement for a connection to fully release -
            ' "all SQLiteCommand and SQLiteDataReader objects associated
            ' with the connection [should be] properly disposed" - the
            ' correct order is children first: readers, then commands,
            ' THEN the connection itself.

            ' Force-dispose every reader this instance ever created via
            ' GetReader(), regardless of whether the caller ever did.
            ' Disposing cCoercedDataReader cascades into disposing the
            ' real ADO.NET reader it wraps (see cCoercedDataReader.
            ' Dispose()). Must happen before the commands below - a
            ' command with a still-open reader attached to it can itself
            ' be uncooperative to dispose, or fail to fully release,
            ' depending on the provider.
            SyncLock Me.m_openReaders
                For Each rdr As IDataReader In Me.m_openReaders
                    Try
                        rdr.Dispose()
                    Catch
                    End Try
                Next
                Me.m_openReaders.Clear()
            End SyncLock

            ' Force-dispose every command this instance ever created via
            ' CreateDBCommand, regardless of whether the reader/caller that
            ' used it was ever properly disposed. Must happen BEFORE the
            ' connection/context are closed/disposed below - an undisposed
            ' command can keep the underlying connection from ever fully
            ' releasing, even after the context itself is disposed.
            SyncLock Me.m_openCommands
                For Each cmd As IDbCommand In Me.m_openCommands
                    Try
                        cmd.Dispose()
                    Catch
                    End Try
                Next
                Me.m_openCommands.Clear()
            End SyncLock

            Dim conn As IDbConnection = GetConnection()
            If conn IsNot Nothing AndAlso conn.State = ConnectionState.Open Then
                conn.Close()
            End If

            ' Retest of something the original connection-never-releases
            ' investigation already tried once (on both providers) and
            ' found no effect - worth retrying now that several other
            ' disposal bugs in this file (including the reader/command
            ' ordering above) have since been fixed, in case one of those
            ' was confounding the earlier result. Disposing a pooled
            ' connection returns it to the provider's internal pool rather
            ' than necessarily closing the underlying native sqlite3*
            ' handle immediately (a deliberate reuse optimization) -
            ' ClearPool explicitly forces that handle closed instead.
            ' net48 (System.Data.SQLite) and net10 (Microsoft.Data.Sqlite)
            ' are different libraries with their own, separately-typed
            ' ClearPool - handled as two explicit branches rather than one
            ' relying on VB's case-insensitive name matching between
            ' "SQLiteConnection" and "SqliteConnection" to resolve to
            ' whichever is actually imported for a given target.
#If NET48 Then
            If TypeOf conn Is SQLiteConnection Then
                Try
                    SQLiteConnection.ClearPool(DirectCast(conn, SQLiteConnection))
                Catch ex As Exception
                    m_logger.LogWarning(ex, "cEwEEFDatabase.Close(): SQLiteConnection.ClearPool failed - {0}", ex.Message)
                End Try
            End If
#Else
            If TypeOf conn Is SqliteConnection Then
                Try
                    SqliteConnection.ClearPool(DirectCast(conn, SqliteConnection))
                Catch ex As Exception
                    m_logger.LogWarning(ex, "cEwEEFDatabase.Close(): SqliteConnection.ClearPool failed - {0}", ex.Message)
                End Try
            End If
#End If

            If m_dbContext IsNot Nothing Then
                m_dbContext.Dispose()
                m_dbContext = Nothing
            End If

            Me.CleanUpLocalWorkingCopy()
            m_strFileName = ""
        End Sub

        Public Overrides ReadOnly Property Name As String
            Get
                Return m_strFileName
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Obtains and configures a <see cref="System.Data.IDataAdapter"/> for
        ''' the current SQLite database via the underlying DbConnection.
        ''' </summary>
        ''' <param name="strSQL">The SQL query to obtain the adapter for.</param>
        ''' <returns>A <see cref="System.Data.IDataAdapter"/> if successful,
        ''' or Nothing when an error occurred.</returns>
        ''' <remarks>
        ''' <para>The returned adapter is initialized with default insert, update
        ''' and delete commands based on the provided query.</para>
        ''' <para>The obtained adapter should be released via
        ''' <see cref="ReleaseAdapter">ReleaseAdapter</see>.</para>
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Overrides Function GetAdapter(strSQL As String) As IDataAdapter
            If m_dbContext Is Nothing Then Return Nothing

            Try
                Dim conn As IDbConnection = GetOpenConnection()
                If conn Is Nothing Then Return Nothing

                Dim dbConn As System.Data.Common.DbConnection = TryCast(conn, System.Data.Common.DbConnection)
                If dbConn Is Nothing Then Return Nothing

                Dim factory As System.Data.Common.DbProviderFactory = System.Data.Common.DbProviderFactories.GetFactory(dbConn)
                If factory Is Nothing Then
                    Throw New NotSupportedException("No DbProviderFactory found for connection type: " & dbConn.GetType().Name)
                End If

                Dim adapter As System.Data.Common.DbDataAdapter = TryCast(factory.CreateDataAdapter(), System.Data.Common.DbDataAdapter)
                If adapter Is Nothing Then
                    Throw New NotSupportedException("IDataAdapter is not supported for provider: " & factory.GetType().Name & ". Use GetDbContext() to access entities directly via Entity Framework.")
                End If

                Dim cmd As System.Data.Common.DbCommand = dbConn.CreateCommand()
                cmd.CommandText = strSQL
                adapter.SelectCommand = cmd
                adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey

                Dim cmdBuilder As System.Data.Common.DbCommandBuilder = factory.CreateCommandBuilder()
                cmdBuilder.DataAdapter = adapter
                adapter.InsertCommand = DirectCast(cmdBuilder.GetInsertCommand(True), System.Data.Common.DbCommand)
                adapter.UpdateCommand = DirectCast(cmdBuilder.GetUpdateCommand(True), System.Data.Common.DbCommand)
                adapter.DeleteCommand = DirectCast(cmdBuilder.GetDeleteCommand(True), System.Data.Common.DbCommand)

                Return adapter
            Catch ex As NotSupportedException
                Throw
            Catch ex As InvalidOperationException
                m_logger.LogError(ex, cStringUtils.Localize("Table in query '{0}' seems to be missing a primary key: {1}", strSQL, ex.Message))
                Return Nothing
            Catch ex As Exception
                m_logger.LogError(ex, cStringUtils.Localize("Error when opening adapter for query '{0}': {1}", strSQL, ex.Message))
                Return Nothing
            End Try
        End Function

        Public Overrides Function GetConnection() As IDbConnection
            If m_dbContext IsNot Nothing Then
#If NET48 Then
                Return m_dbContext.Database.Connection
#Else
                Return m_dbContext.Database.GetDbConnection()
#End If
            End If
            Return Nothing
        End Function

        Private Function GetOpenConnection() As IDbConnection
            Dim conn As IDbConnection = GetConnection()
            If conn Is Nothing Then Return Nothing
            If conn.State <> ConnectionState.Open Then
                conn.Open()
                ' Run PRAGMAs once on first open
                Using pragmasCmd As IDbCommand = conn.CreateCommand()
                    pragmasCmd.CommandText = "PRAGMA journal_mode=DELETE; PRAGMA synchronous=FULL; PRAGMA cache_size=-64000; PRAGMA temp_store=MEMORY;"
                    pragmasCmd.ExecuteNonQuery()
                End Using
            End If
            Return conn
        End Function

        Public Overrides Function CanConnect(dst As eDataSourceTypes) As Boolean
            Return dst = eDataSourceTypes.Sqlite
        End Function

        Public Overrides Function CanCompact(strConnectionFrom As String, strConnectionTo As String) As Boolean
            Return False ' Not supported for EF/SQLite
        End Function

        Public Overrides Function Compact(strFileFrom As String, strFileTo As String) As eDatasourceAccessType
            Return eDatasourceAccessType.Failed_DeprecatedOperation
        End Function

        Public Overrides Function MaxDBVersion() As Single
            Return cDatabaseUpdater.MaxSupportedVersion
        End Function

        Public Overrides ReadOnly Property Directory As String
            Get
                Return Path.GetDirectoryName(m_strFileName)
            End Get
        End Property

        Public Overrides ReadOnly Property FileName As String
            Get
                Return Path.GetFileNameWithoutExtension(m_strFileName)
            End Get
        End Property

        Public Overrides ReadOnly Property Extension As String
            Get
                Return Path.GetExtension(m_strFileName)
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' The legacy cDBUpdate update chain (see cDatabaseUpdater.RunAllUpdates)
        ''' was written for, and several of its historical updates rely on,
        ''' Access/OleDb-specific SQL that SQLite does not support at all
        ''' (ALTER COLUMN, ADD/DROP CONSTRAINT, ADD PRIMARY KEY/FOREIGN KEY via
        ''' ALTER TABLE). This database's schema is versioned separately, via
        ''' EF Core migrations, so the legacy chain should never run against it.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overrides Function SupportsLegacyDatabaseUpdates() As Boolean
            Return False
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Not supported - see <see cref="SupportsLegacyDatabaseUpdates"/>.
        ''' Throws rather than silently attempting SQL that SQLite cannot run,
        ''' in case this is ever reached by a path other than the (guarded)
        ''' legacy update chain.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overrides Function DropColumn(strTable As String, strColumn As String) As Boolean
            Throw New NotSupportedException(
                "cEwEEFDatabase.DropColumn() is not supported - SQLite schema changes go through EF Core migrations instead. See SupportsLegacyDatabaseUpdates().")
        End Function

        ' Additional EF-specific methods can be added here as needed
        Protected Overrides Function CreateDBCommand(strSQL As String) As IDbCommand
            ' For EF, create a command from the underlying DbConnection
            If m_dbContext Is Nothing Then Return Nothing
            Dim conn As IDbConnection = GetOpenConnection()
            If conn Is Nothing Then Return Nothing
            Dim cmd As IDbCommand = conn.CreateCommand()
            cmd.CommandText = strSQL
            SyncLock Me.m_openCommands
                Me.m_openCommands.Add(cmd)
            End SyncLock
            Return cmd
        End Function

        Public Overrides Function HasTable(strTableName As String) As Boolean
            If m_dbContext Is Nothing Then Return False
            Return m_dbContext.GetEntityTypeByTableName(strTableName) IsNot Nothing
        End Function

        Public Overrides Function GetReader(strSQL As String) As IDataReader
            Dim command As IDbCommand = Nothing
            Dim reader As cCoercedDataReader = Nothing
            Try
                command = Me.CreateDBCommand(strSQL)
                reader = New cCoercedDataReader(command.ExecuteReader())
                SyncLock Me.m_openReaders
                    Me.m_openReaders.Add(reader)
                End SyncLock
            Catch ex As Exception
#If VERBOSE_LEVEL >= 1 Then
                Console.WriteLine("GetReader error: {0}", ex.Message)
#End If
                m_logger.LogError(ex, "cEwEEFDatabase.GetReader(" & strSQL & ")")
                command?.Dispose()
                reader = Nothing
            End Try
            If reader IsNot Nothing Then
                reader.PropTypes = GetDbContext().GetPropTypes(DataReaderDiff.GetTableName(reader))
            End If
            Return reader
        End Function

        ''' ---------------------------------------------------------------
        ''' <summary>
        ''' Returns an EF-backed writer for the given table. Overridden because
        ''' the base implementation constructs a cEwEDbWriter, which relies on
        ''' GetAdapter() - and Microsoft.Data.Sqlite has no DbDataAdapter to give it.
        ''' </summary>
        ''' ---------------------------------------------------------------
        Public Overrides Function GetWriter(strTable As String) As IEwEDbWriter
            Dim writer As IEwEDbWriter = New cEwEEFDbWriter(Me.m_dbContext, strTable, Me.m_logger)
            writer.RefCount += 1
            Return writer
        End Function

    End Class
End Namespace
