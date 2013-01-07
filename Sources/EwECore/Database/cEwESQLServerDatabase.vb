' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 2 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see <http://www.gnu.org/licenses/gpl-2.0.html>. 
'
' Copyright 1991-2013 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'

#Region " Imports "

Option Strict On

Imports System.IO
Imports System.Data.OleDb
Imports System.Reflection
Imports System.Text
Imports EwECore.DataSources
Imports EwEUtils.Database
Imports EwEUtils.Utilities
Imports EwEUtils.Core
Imports EwEUtils.SystemUtilities
Imports System.Data.SqlClient

#End Region ' Imports

#If DEBUG Then

Namespace Database

    ''' =======================================================================
    ''' <summary>
    ''' Database class specialized for storing and writing EwE data to Microsoft 
    ''' SQL Server databases.
    ''' </summary>
    ''' <remarks>
    ''' This class wraps Microsoft SQL Server specifics such as connection
    ''' strings.
    ''' </remarks>
    ''' =======================================================================
    Public Class cEwESQLServerDatabase
        Inherits cEwEDatabase

#Region " Private vars "

        ''' <summary>A connection to an SQL connection database, if any.</summary>
        Public m_conn As SqlConnection = Nothing
        ''' <summary>The connection string to connect to a SQL Express database.</summary>
        ''' <remarks>Hard-coded to Tampa Bay for testing purposes.</remarks>
        Private m_strConnection As String = "Server=.\SQLEXPRESS;Database=TampaBay;Trusted_Connection=True;"
        ''' <summary>File name to access database.</summary>
        Private m_strFileName As String = ""

#End Region ' Private vars

#Region " Generic "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Create a new SQL Server EwE database.
        ''' </summary>
        ''' <param name="strDatabase">The name of the database to create.</param>
        ''' <param name="bOverwrite">States whether an existing database may be overwritten.</param>
        ''' <param name="format">Database format type to use. If not set, the 
        ''' database type is deducted from the <paramref name="strDatabase">database</paramref>.</param>
        ''' <returns>A <see cref="eDatasourceAccessType">eDatasourceAccessType</see> value</returns>
        ''' <remarks>Note that this will NOT open the newly created database.</remarks>
        ''' -------------------------------------------------------------------
        Public Overrides Function Create(ByVal strDatabase As String, _
                ByVal strModelName As String, _
                Optional ByVal bOverwrite As Boolean = False, _
                Optional ByVal format As eDataSourceTypes = eDataSourceTypes.NotSet, _
                Optional ByVal strAuthor As String = "") As eDatasourceAccessType

            Return eDatasourceAccessType.Failed_CannotSave

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Save a given Access database to a new destination, and open this 
        ''' new database.
        ''' </summary>
        ''' <param name="strDatabaseTo">Target database name.</param>
        ''' <param name="strModelName">New name to assign to the model.</param>
        ''' <param name="bOverwrite">States whether any model in the way will be obliterated.</param>
        ''' <returns>True if succesful.</returns>
        ''' -------------------------------------------------------------------
        Public Overrides Function SaveAs(ByVal strDatabaseTo As String, _
                ByVal strModelName As String, _
                Optional ByVal bOverwrite As Boolean = False, _
                Optional ByVal databaseType As eDataSourceTypes = eDataSourceTypes.NotSet) As eDatasourceAccessType

            Return eDatasourceAccessType.Failed_CannotSave

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Open a connection to a SQL Server database.
        ''' </summary>
        ''' <param name="strDatabase">The database to open.</param>
        ''' <param name="databaseType">Type to use to open the database. Set this
        ''' to 'NotSet' to auto-detect the database type.</param>
        ''' <returns>True if connected succesfully.</returns>
        ''' -------------------------------------------------------------------
        Public Overrides Function Open(ByVal strDatabase As String, _
                                       Optional ByVal databaseType As eDataSourceTypes = eDataSourceTypes.NotSet, _
                                       Optional ByVal bReadOnly As Boolean = False) As eDatasourceAccessType

            ' Pre
            Debug.Assert(Not String.IsNullOrEmpty(strDatabase), "Invalid data source specified")
            Debug.Assert(Not Me.IsConnected(), "Connection already open, close first")

            Dim datResult As eDatasourceAccessType = eDatasourceAccessType.Failed_Unknown

            ' Does file exist?
            If Not File.Exists(strDatabase) Then Return eDatasourceAccessType.Failed_FileNotFound

            ' Need to auto-detect database type?
            If databaseType = eDataSourceTypes.NotSet Then
                ' #Yes: auto-detect
                databaseType = cDataSourceFactory.GetSupportedType(strDatabase)
            End If

            Me.m_conn = New SqlClient.SqlConnection()

            ' Try to assemble connection string
            Select Case databaseType
                Case eDataSourceTypes.SQLServer
                    Try
                        Me.m_conn.ConnectionString = Me.m_strConnection ' String.Format(m_strConnection, ".\SQLEXPRESS", "EWE6SQL")
                    Catch ex As Exception

                    End Try
                Case eDataSourceTypes.NotSet
                    Me.m_conn.ConnectionString = ""
                    datResult = eDatasourceAccessType.Failed_UnknownType
            End Select

            If Not String.IsNullOrEmpty(Me.m_conn.ConnectionString) Then

                Me.IsReadOnly = bReadOnly

                Try

                    ' Try to open the connection
                    Me.m_conn.Open()
                    ' Set status
                    datResult = eDatasourceAccessType.Opened
                    ' All well: store file name
                    Me.m_strFileName = strDatabase

                Catch ex As OleDbException

                    Select Case ex.ErrorCode
                        Case -2147467259
                            ' File not found
                            datResult = eDatasourceAccessType.Failed_FileNotFound
                        Case Else
                            ' OleDb got into trouble
                            datResult = eDatasourceAccessType.Failed_Unknown
                    End Select
                    cLog.Write(String.Format("Open DB: OleDbException {0}, {1} when opening '{2}'", ex.Message, ex.ErrorCode, Me.m_conn.ConnectionString))

                Catch ex As InvalidOperationException
                    datResult = eDatasourceAccessType.Failed_OSUnsupported
                    cLog.Write(String.Format("Open DB: InvalidOperationException {0} when opening {1}", ex.Message, strDatabase))

                Catch ex As Exception
                    datResult = eDatasourceAccessType.Failed_Unknown
                    cLog.Write(String.Format("Open DB: Exception {0} when opening {1}", ex.Message, strDatabase))

                End Try

            End If

            Return datResult

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Close the current M$ Access connection.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overrides Sub Close()

            ' Pre
            Debug.Assert(Me.IsConnected(), "Cannot close a connection that is not open")

            Me.m_conn.Close()
            Me.m_conn.Dispose()
            Me.m_conn = Nothing

            ' Clear file name
            Me.m_strFileName = ""
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the name of the connected database.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overrides ReadOnly Property Name() As String
            Get
                Return m_strFileName
            End Get
        End Property

#End Region ' Generic

#Region " Overrides "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Obtains and configures a <see cref="OleDbDataAdapter">OleDbDataAdapter</see>
        ''' for the current M$ Access database.
        ''' </summary>
        ''' <param name="strSQL">The SQL query to obtain the adaper for.</param>
        ''' <returns>A <see cref="OleDbDataAdapter">OleDbDataAdapter</see> if
        ''' successful, or Nothing when an error occurred.</returns>
        ''' <remarks>
        ''' <para>The returned adapter is initialized with default insert, update 
        ''' and delete commands based on the provided query.</para>
        ''' <para>The obtained OleDbDataAdapter should be released via 
        ''' <see cref="ReleaseAdapter">ReleaseAdapter</see>.</para></remarks>
        ''' -------------------------------------------------------------------
        Public Overrides Function GetAdapter(ByVal strSQL As String) As IDataAdapter

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

                ' JS 04apr06: Disabled unreliable event, handled generically in cEwEDatabase
                '' Handle event to fix invalid DBNull values with their defaults
                'AddHandler adapter.RowUpdating, New OleDbRowUpdatingEventHandler(AddressOf OnRowUpdating)

                ' JS 05sep07: Disabled since EwE no longer uses Autonumbered values
                '' Handle event to implement Access Autonumber ID fix
                'AddHandler adapter.RowUpdated, New OleDbRowUpdatedEventHandler(AddressOf OnRowUpdated)

            Catch ex As InvalidOperationException
                cLog.Write(String.Format("Table in query '{0}' seems to be missing a primary key: {1}", strSQL, ex.Message))
                Debug.Assert(False, String.Format("Table in query '{0}' seems to be missing a primary key: {1}", strSQL, ex.Message))
                adapter = Nothing

            Catch ex As Exception
                cLog.Write(String.Format("Error when opening adapter for query {0}: {1}", strSQL, ex.Message))
                Debug.Assert(False, String.Format("Error when opening adapter for query {0}: {1}", strSQL, ex.Message))
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
        ''' <param name="dst">The datasource type to test.</param>
        ''' <returns>True if the OS can connect to a given datasource type.</returns>
        ''' -------------------------------------------------------------------
        Public Overrides Function CanConnect(ByVal dst As EwEUtils.Core.eDataSourceTypes) As Boolean

            Dim conn As OleDbConnection = New OleDbConnection()
            Dim strDatabase As String = "~doesnotexist~"
            Dim datResult As eDatasourceAccessType = eDatasourceAccessType.Opened

            ' Try to assemble connection string
            Select Case dst
                Case eDataSourceTypes.SQLServer
                    conn.ConnectionString = Me.m_strConnection ' String.Format(m_strConnection, ".\SQLEXPRESS", strDatabase)
                Case eDataSourceTypes.NotSet
                    conn.ConnectionString = ""
                    datResult = eDatasourceAccessType.Failed_UnknownType
            End Select

            If Not String.IsNullOrEmpty(conn.ConnectionString) Then
                Try
                    conn.Open()
                    conn.Close() ' Can't be, but hey
                Catch ex As InvalidOperationException
                    datResult = eDatasourceAccessType.Failed_OSUnsupported
                Catch ex As Exception
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
        ''' <remarks>Not supported (yet)</remarks>
        ''' -------------------------------------------------------------------
        Public Overrides Function CanCompact(ByVal strConnectionFrom As String, ByVal strConnectionTo As String) As Boolean

            Return False

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Compact the current M$ Access database.
        ''' </summary>
        ''' <param name="strFileFrom">Source database to compact.</param>
        ''' <param name="strFileTo">Target database to compact to. Can be left blank.</param>
        ''' <returns>True if succesful.</returns>
        ''' <remarks>
        ''' Not supported (yet)
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Overrides Function Compact(ByVal strFileFrom As String, _
                                          ByVal strFileTo As String) As eDatasourceAccessType

            Return eDatasourceAccessType.Failed_OSUnsupported

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

#End If
