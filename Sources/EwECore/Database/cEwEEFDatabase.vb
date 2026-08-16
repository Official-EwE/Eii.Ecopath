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

Namespace Database
    Public Class cEwEEFDatabase
        Inherits cEwEDatabase

        Private m_dbContext As EwEDbContext = Nothing
        Private m_strFileName As String = ""
        Private m_isReadOnly As Boolean = False

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
        ''' <param name="strDatabase">The file name of the .sqlite to create.</param>
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

        Public Overrides Function SaveAs(strDatabaseTo As String, strModelName As String, Optional bOverwrite As Boolean = False, Optional databaseType As eDataSourceTypes = eDataSourceTypes.NotSet) As eDatasourceAccessType
            Try
                If File.Exists(strDatabaseTo) AndAlso Not bOverwrite Then
                    Return eDatasourceAccessType.Failed_CannotSave
                End If
                File.Copy(m_strFileName, strDatabaseTo, True)
                EnsureDbContext(strDatabaseTo)
                m_strFileName = strDatabaseTo
                Return eDatasourceAccessType.Success
            Catch ex As Exception
                Return eDatasourceAccessType.Failed_Unknown
            End Try
        End Function

        Public Overrides Function Open(strDatabase As String, Optional databaseType As eDataSourceTypes = eDataSourceTypes.NotSet, Optional bReadOnly As Boolean = False) As eDatasourceAccessType
            Try
                If Not File.Exists(strDatabase) Then
                    Return eDatasourceAccessType.Failed_FileNotFound
                End If
                EnsureDbContext(strDatabase)
                m_strFileName = strDatabase
                m_isReadOnly = bReadOnly
                ' Ensure connection is opened
                GetOpenConnection()
                Return eDatasourceAccessType.Opened
            Catch ex As Exception
                Return eDatasourceAccessType.Failed_Unknown
            End Try
        End Function

        Public Overrides Sub Close()
            Dim conn As IDbConnection = GetConnection()
            If conn IsNot Nothing AndAlso conn.State = ConnectionState.Open Then
                conn.Close()
            End If
            If m_dbContext IsNot Nothing Then
                m_dbContext.Dispose()
                m_dbContext = Nothing
            End If
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
                    pragmasCmd.CommandText = "PRAGMA journal_mode=WAL; PRAGMA synchronous=NORMAL; PRAGMA cache_size=-64000; PRAGMA temp_store=MEMORY;"
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
            Return cmd
        End Function

        Protected Overrides Function HasTable(strTableName As String) As Boolean
            If m_dbContext Is Nothing Then Return False
            Return m_dbContext.GetEntityTypeByTableName(strTableName) IsNot Nothing
        End Function
        
        Public Overrides Function GetReader(strSQL As String) As IDataReader
            Dim command As IDbCommand = Nothing
            Dim reader As cCoercedDataReader = Nothing
            Try
                command = Me.CreateDBCommand(strSQL)
                reader = New cCoercedDataReader(command.ExecuteReader())
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