#Region " Imports "

Option Strict On
Imports EwEUtils.Core
Imports Microsoft.Office.Interop.Access

#End Region ' Imports

Namespace Database

    ''' =======================================================================
    ''' <summary>
    ''' Database compact class for Office DAO databases.
    ''' </summary>
    ''' =======================================================================
    Public Class cCompactDAO
        Implements IDatabaseCompact

#Region " Private vars "

        ''' <summary>Global, one time flags.</summary>
        Private Shared s_bEngineSearched As Boolean = False
        ''' <summary>Global, one time flags.</summary>
        Private Shared s_bEngineFound As Boolean = False

#End Region ' Private vars 

#Region " IDatabaseCompact implementation "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns whether the OS supports compacting databases via DAO.
        ''' </summary>
        ''' <returns>True if the OS supports compacting databases via DAO.</returns>
        ''' -------------------------------------------------------------------
        Public Function CanCompact() As Boolean _
            Implements IDatabaseCompact.CanCompact
            Return Me.DetectDAO()
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Compact a database via DAO.
        ''' </summary>
        ''' <param name="strFileFrom">Source database location.</param>
        ''' <param name="strConnectionFrom">Source database connection string.</param>
        ''' <param name="strFileTo">Target database location.</param>
        ''' <param name="strConnectionTo">Target database connection string.</param>
        ''' <returns>A <see cref="eDatasourceAccessType">database access
        ''' result code</see>.</returns>
        ''' -------------------------------------------------------------------
        Public Function Compact(ByVal strFileFrom As String, _
                                ByVal strConnectionFrom As String, _
                                ByVal strFileTo As String, _
                                ByVal strConnectionTo As String) As EwEUtils.Core.eDatasourceAccessType _
            Implements IDatabaseCompact.Compact

            ' Safety check: can compact at all?
            If Not Me.CanCompact() Then
                ' #No: abort
                Return eDatasourceAccessType.Failed_OSUnsupported
            End If

            Try
                ' Create engine
                Dim engine As Dao.DBEngine = New Dao.DBEngine()

                ' Able to get JET engine?
                If (engine Is Nothing) Then Return eDatasourceAccessType.Failed_OSUnsupported

                ' Compact DB
                engine.CompactDatabase(strFileFrom, strFileTo)
                ' Return result

                Return eDatasourceAccessType.Success
            Catch ex As Exception
                ' Woops
                Return eDatasourceAccessType.Failed_OSUnsupported
            End Try

            Return eDatasourceAccessType.Failed_Unknown
        End Function

#End Region ' IDatabaseCompact implementation

#Region " Internals "

        Function DetectDAO() As Boolean

            If cCompactDAO.s_bEngineSearched Then Return cCompactDAO.s_bEngineFound

            Try

                ' Create engine
                Dim engine As New Dao.DBEngine()
                ' Do something with the engine
                Dim strVersion As String = engine.Version
                ' Still alive? Nice, presume it all works then
                cCompactDAO.s_bEngineFound = True

            Catch ex As Exception
                cCompactDAO.s_bEngineFound = False
            End Try
            cCompactDAO.s_bEngineSearched = True

            Return cCompactDAO.s_bEngineFound

        End Function

#End Region ' Internals

    End Class

End Namespace ' Database
