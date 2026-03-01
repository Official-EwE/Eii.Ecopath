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

        Public Overrides Function Create(strDatabase As String, strModelName As String, Optional bOverwrite As Boolean = False, Optional format As eDataSourceTypes = eDataSourceTypes.NotSet, Optional strAuthor As String = "") As eDatasourceAccessType
            Return dbAccessDatabase.Create(strDatabase, strModelName, bOverwrite, format, strAuthor)
        End Function

        Public Function Versus(strDatabase As String) As eDatasourceAccessType
            Return dbEfDatabase.Create(strDatabase, "", False, eDataSourceTypes.Sqlite)
        End Function


        Public Overrides Function Open(strDatabase As String, Optional databaseType As eDataSourceTypes = eDataSourceTypes.NotSet, Optional bReadOnly As Boolean = False) As eDatasourceAccessType
            Return dbAccessDatabase.Open(strDatabase, databaseType, bReadOnly)
        End Function

        Public Overrides ReadOnly Property Name As String

        Public Overrides Function SaveAs(strDatabaseTo As String, strModelName As String, Optional bOverwrite As Boolean = False, Optional databaseType As eDataSourceTypes = eDataSourceTypes.NotSet) As eDatasourceAccessType
            Throw New NotImplementedException
        End Function

        Public Overrides Function MaxDBVersion() As Single
            Throw New NotImplementedException
        End Function

        Public Overrides Function Compact(strFileFrom As String, strFileTo As String) As eDatasourceAccessType
            Throw New NotImplementedException
        End Function

        Public Overrides Function CanCompact(strConnectionFrom As String, strConnectionTo As String) As Boolean
            Throw New NotImplementedException
        End Function

        Public Overrides Function GetConnection() As IDbConnection
            Return Me.dbAccessDatabase.GetConnection()
        End Function

        Public Overrides Function CanConnect(dst As eDataSourceTypes) As Boolean
            Throw New NotImplementedException
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
                    secondaryReader = command.ExecuteReader()
                End Using
            Catch ex As Exception
#If VERBOSE_LEVEL >= 1 Then
                Console.WriteLine("GetReader error: {0}", ex.Message)
#End If
                m_logger.LogError(ex, "cEwEDatabase.GetReader(" & strSQL & ")")
                Return Nothing
            End Try
            Return New cEwEVersusDataReader(primaryReader, secondaryReader)
        End Function


        ' Add methods to compare data retrieval between dbPrimary and dbSecondary as needed
    End Class
End Namespace

