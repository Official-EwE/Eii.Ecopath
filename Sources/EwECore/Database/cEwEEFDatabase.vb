' SPDX-License-Identifier: EUPL-1.2
' Entity Framework implementation of cEwEDatabase for SQLite/EF
Imports System.Data
Imports System.IO
Imports Eii.Ecopath.Storage
Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Infrastructure

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
                Dim conn As IDbConnection = GetConnection()
                If conn IsNot Nothing AndAlso conn.State <> ConnectionState.Open Then
                    conn.Open()
                End If
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

        Public Overrides Function GetAdapter(strSQL As String) As IDataAdapter
            Throw New NotSupportedException("EF database does not support SQL adapters.")
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
            Return 1.0F ' Or whatever is appropriate for your EF schema
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
            Dim conn As IDbConnection = GetConnection()
            If conn Is Nothing Then Return Nothing
            If conn.State <> ConnectionState.Open Then
                conn.Open()
            End If
            Dim cmd As IDbCommand = conn.CreateCommand()
            cmd.CommandText = strSQL
            Return cmd
        End Function

    End Class
End Namespace

