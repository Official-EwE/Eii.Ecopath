' SPDX-License-Identifier: EUPL-1.2
' Entity Framework implementation of cEwEDatabase for SQLite/EF
Imports System.Data
Imports System.IO
Imports Eii.Ecopath.Storage
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
            If m_dbContext Is Nothing Then
                m_dbContext = New EwEDbContext()
            End If
#If NET48
            m_dbContext.Database.Initialize(True)
#Else
            m_dbContext.Database.Migrate()
#End If
        End Sub

        Public Function GetDbContext() As EwEDbContext
            Return m_dbContext
        End Function

        Public Overrides Function Create(strDatabase As String, strModelName As String, Optional bOverwrite As Boolean = False, Optional format As eDataSourceTypes = eDataSourceTypes.NotSet, Optional strAuthor As String = "") As eDatasourceAccessType
            ' For EF, just set up the SQLite file and context
            Try
                If File.Exists(strDatabase) AndAlso Not bOverwrite Then
                    Return eDatasourceAccessType.Failed_CannotSave
                End If
                EnsureDbContext(strDatabase)
                m_strFileName = strDatabase
                Return eDatasourceAccessType.Success
            Catch ex As Exception
                Return eDatasourceAccessType.Failed_Unknown
            End Try
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
#If NET48
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

        Public Overrides Function GetReader(strSQL As String) As IDataReader
            Dim reader As cCoercedDataReader = Nothing
            Try
                Using command As IDbCommand = Me.CreateDBCommand(strSQL)
                    reader = New cCoercedDataReader(command.ExecuteReader())
                End Using
            Catch ex As Exception
#If VERBOSE_LEVEL >= 1 Then
                Console.WriteLine("GetReader error: {0}", ex.Message)
#End If
                m_logger.LogError(ex, "cEwEEFDatabase.GetReader(" & strSQL & ")")
                reader = Nothing
            End Try
            reader.PropTypes = GetDbContext().GetPropTypes(DataReaderDiff.GetTableName(reader))
            Return reader
        End Function

    End Class
End Namespace

