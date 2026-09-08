' SPDX-License-Identifier: EUPL-1.2
' This file implements cEwEVersusDatabase for comparing two databases
Imports EwECore.Database
Imports Microsoft.Extensions.Logging

Namespace Database
    Public Class cEwEVersusDatabase
        Inherits cEwEDatabase

        Private dbAccessDatabase As cEwEAccessDatabase
        Private dbEfDatabase As cEwEEFDatabase

        Public Sub New(accessDatabase As cEwEDatabase, efDatabase As cEwEEFDatabase)
            dbAccessDatabase = accessDatabase
            dbEfDatabase = efDatabase
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Closes both wrapped databases. The base cEwEDatabase.Close() only
        ''' resets a version field and has no knowledge of dbAccessDatabase/
        ''' dbEfDatabase - without this override, closing a versus-database
        ''' left both connections (in particular the Access/OleDb one, with
        ''' its exclusive file lock) open indefinitely, causing a spurious
        ''' "already open" error the next time the same Access file was
        ''' opened again.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overrides Sub Close()
            Try
                dbAccessDatabase?.Close()
            Catch ex As Exception
                m_logger.LogError(ex, "cEwEVersusDatabase.Close(): failed to close the Access database")
            End Try
            Try
                dbEfDatabase?.Close()
            Catch ex As Exception
                m_logger.LogError(ex, "cEwEVersusDatabase.Close(): failed to close the EF/SQLite database")
            End Try
            MyBase.Close()
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns the underlying Access database this versus-database wraps,
        ''' for use by code that needs to explicitly bypass the EF/SQLite side -
        ''' see cDatabaseUpdater.RunAllUpdates and SupportsLegacyDatabaseUpdates.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Function GetAccessDatabase() As cEwEAccessDatabase
            Return Me.dbAccessDatabase
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' The legacy update chain is not run against the versus-database
        ''' itself - cDatabaseUpdater.RunAllUpdates instead substitutes
        ''' GetAccessDatabase() and runs updates against that directly,
        ''' bypassing the EF/SQLite side entirely (which does not support the
        ''' legacy update chain's Access-specific SQL - see
        ''' cEwEEFDatabase.SupportsLegacyDatabaseUpdates).
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overrides Function SupportsLegacyDatabaseUpdates() As Boolean
            Return False
        End Function

        Public Overrides Function Create(strDatabase As String, strModelName As String, Optional bOverwrite As Boolean = False, Optional format As eDataSourceTypes = eDataSourceTypes.NotSet, Optional strAuthor As String = "") As eDatasourceAccessType
            Dim atResult As eDatasourceAccessType = dbAccessDatabase.Create(strDatabase, strModelName, bOverwrite, format, strAuthor)
            ' Replace extension with .ewesqlite for EF database
            Dim efDatabasePath As String = System.IO.Path.ChangeExtension(strDatabase, ".ewesqlite")
            Return atResult And dbEfDatabase.Create(efDatabasePath, "", False, eDataSourceTypes.Sqlite)
        End Function

        Public Overrides Function Open(strDatabase As String, Optional databaseType As eDataSourceTypes = eDataSourceTypes.NotSet, Optional bReadOnly As Boolean = False) As eDatasourceAccessType
            Dim atResult As eDatasourceAccessType = dbAccessDatabase.Open(strDatabase, databaseType, bReadOnly)
            ' Replace extension with .ewesqlite for EF database
            Dim efDatabasePath As String = System.IO.Path.ChangeExtension(strDatabase, ".ewesqlite")
            Return atResult And dbEfDatabase.Open(efDatabasePath, eDataSourceTypes.Sqlite, bReadOnly)
        End Function

        Public Overrides ReadOnly Property Name As String
            Get
                Return dbAccessDatabase.Name
            End Get
        End Property

        Public Overrides Function SaveAs(strDatabaseTo As String, strModelName As String, Optional bOverwrite As Boolean = False, Optional databaseType As eDataSourceTypes = eDataSourceTypes.NotSet) As eDatasourceAccessType
            Throw New NotImplementedException
        End Function

        Public Overrides Function MaxDBVersion() As Single
            Throw New NotImplementedException
        End Function

        Public Overrides Function Compact(strFileFrom As String, strFileTo As String) As eDatasourceAccessType
            return Me.dbAccessDatabase.Compact(strFileFrom , strFileTo)
        End Function

        Public Overrides Function CanCompact(strConnectionFrom As String, strConnectionTo As String) As Boolean
            return Me.dbAccessDatabase.CanCompact(strConnectionFrom , strConnectionTo)
        End Function

        Public Overrides Function GetConnection() As IDbConnection
            Return Me.dbAccessDatabase.GetConnection()
        End Function

        Public Overrides Function CanConnect(dst As eDataSourceTypes) As Boolean
            Return Me.dbAccessDatabase.CanConnect(dst)
        End Function

        Public Overrides ReadOnly Property Directory As String
        Public Overrides ReadOnly Property FileName As String
        Public Overrides ReadOnly Property Extension As String

        Public Overrides Function GetReader(strSQL As String) As IDataReader
            Dim primaryReader As IDataReader = Nothing
            Dim secondaryReader As IDataReader = Nothing
            Try
                Using command As IDbCommand = Me.CreateDBCommand(strSQL)
                    primaryReader = command.ExecuteReader()
                End Using
                Using command As IDbCommand = dbEfDatabase.GetConnection().CreateCommand()
                    command.CommandText = strSQL
                    secondaryReader = command.ExecuteReader(CommandBehavior.KeyInfo)
                End Using
            Catch ex As Exception
#If VERBOSE_LEVEL >= 1 Then
                Console.WriteLine("GetReader error: {0}", ex.Message)
#End If
                m_logger.LogError(ex, "cEwEVersusDatabase.GetReader(" & strSQL & ")")
                Return Nothing
            End Try
            Dim vsReader = New cEwEVersusDataReader(primaryReader, secondaryReader)
            vsReader.SetFuncGetPropTypes(Function(t) dbEfDatabase.GetDbContext().GetPropTypes(t))
            Return vsReader
        End Function


        Public Overrides Function GetWriter(strTable As String) As IEwEDbWriter
            Dim accessWriter As IEwEDbWriter = dbAccessDatabase.GetWriter(strTable)
            Dim efWriter As New cEwEEFDbWriter(dbEfDatabase.GetDbContext(), strTable, m_logger)
            Dim writer As IEwEDbWriter = New cEwEVersusDbWriter(
                accessWriter, efWriter, strTable,
                Function(t) dbEfDatabase.GetDbContext().GetPropTypes(t),
                m_logger)
            writer.RefCount += 1
            Return writer
        End Function

        ' Add methods to compare data retrieval between dbPrimary and dbSecondary as needed
    End Class
End Namespace

