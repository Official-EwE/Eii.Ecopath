#Region " Imports "

Option Strict On
Imports EwEUtils.Core
Imports EwECore.DataSources

#End Region ' Imports

Namespace Database

    ''' =======================================================================
    ''' <summary>
    ''' Helper class, returns a database compact engine for compacting a 
    ''' database for an underlying database engine.
    ''' </summary>
    ''' =======================================================================
    Public Class cDatabaseCompactFactory

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get <see cref="IDatabaseCompact">database compact engine</see> for
        ''' a given database.
        ''' </summary>
        ''' <param name="strFileName">The complete path to the database to compact.</param>
        ''' <returns>An instance of a <see cref="IDatabaseCompact">database compact engine</see>,
        ''' or Null / Nothing if an error occurred.</returns>
        ''' -------------------------------------------------------------------
        Shared Function GetDatabaseCompact(ByVal strFileName As String) As IDatabaseCompact

            Select Case cDataSourceFactory.GetSupportedType(strFileName)
                Case eDataSourceTypes.MDB
                    ' MDB databases compacted via JRO
                    Return New cCompactJRO()
                Case eDataSourceTypes.ACCDB
                    ' ACCDB databases compacted via DAO
                    Return New cCompactDAO()
                Case Else
                    ' Not supported
            End Select

            Return Nothing

        End Function

    End Class

End Namespace ' Database
