#Region " Imports "

Option Strict On
Imports System.IO
Imports EwECore.Database
Imports EwEUtils.Core
Imports EwEUtils.Database

#End Region ' Imports

Namespace DataSources

    ''' =======================================================================
    ''' <summary>
    ''' Factory for creating data sources
    ''' </summary>
    ''' =======================================================================
    Public Class cDataSourceFactory

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns an EwE <see cref="eDataSourceTypes">datasource type</see> that
        ''' will be able to interact with the provided file name.
        ''' </summary>
        ''' <param name="strFile">Name of the file.</param>
        ''' <returns>A <see cref="eDataSourceTypes">datasource type</see>
        ''' indicating what type of EwE datasource will be able to interact with
        ''' the provided file name.</returns>
        ''' -------------------------------------------------------------------
        Public Shared Function GetSupportedType(ByVal strFile As String) As eDataSourceTypes
            Select Case Path.GetExtension(strFile).ToLower
                Case ".eii" : Return eDataSourceTypes.EII
                Case ".mdb", ".ewemdb" : Return eDataSourceTypes.MDB
                Case ".accdb", ".eweaccdb" : Return eDataSourceTypes.ACCDB
            End Select
            Return eDataSourceTypes.NotSet
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns the default extension for a given <see cref="eDataSourceTypes">datasource type</see>.
        ''' </summary>
        ''' <param name="dst">The <see cref="eDataSourceTypes">datasource type</see> to query.</param>
        ''' <returns>A string providing a file extension, or an empty string if
        ''' the given datasource type is not supported.</returns>
        ''' -------------------------------------------------------------------
        Public Shared Function GetDefaultExtension(ByVal dst As eDataSourceTypes) As String
            Select Case dst
                Case eDataSourceTypes.MDB : Return ".ewemdb"
                Case eDataSourceTypes.EII : Return ".eii"
                Case eDataSourceTypes.ACCDB : Return ".eweaccdb"
            End Select
            Return ""
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Create a data source onto an existing <see cref="cEwEDatabase">EwE database</see>.
        ''' </summary>
        ''' <param name="db"><see cref="cEwEDatabase">cEwEDatabase</see> to create a datasource for.</param>
        ''' <param name="ds">The newly created datasource.</param>
        ''' <returns>A <see cref="eStatusFlags">Status flag</see> that indicates the valid</returns>
        ''' -------------------------------------------------------------------
        Public Shared Function Create(ByRef db As cEwEDatabase, ByRef ds As IEwEDataSource) As eStatusFlags

            Dim nResult As eStatusFlags = eStatusFlags.OK

            If TypeOf db Is cEwEAccessDatabase Then
                ' Create a DB datasource on a MS Access database
                ds = New cDBDataSource(db)
            End If
            Return nResult

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Create a data source for a given <see cref="eDataSourceTypes">type of EwE datasource</see>.
        ''' </summary>
        ''' <param name="dst"><see cref="eDataSourceTypes">Type of EwE datasource</see> to create.</param>
        ''' <returns>A <see cref="IEwEDataSource">IEwEDataSource</see> or 
        ''' Nothing if creation failed</returns>
        ''' -------------------------------------------------------------------
        Public Shared Function Create(ByVal dst As eDataSourceTypes) As IEwEDataSource

            Dim nResult As eStatusFlags = eStatusFlags.OK

            Select Case dst
                Case eDataSourceTypes.EII
                    Return New cEIIDataSource()
                Case eDataSourceTypes.MDB, eDataSourceTypes.ACCDB
                    ' Create a DB datasource on a MS Access database
                    Return New cDBDataSource(New cEwEAccessDatabase())
                Case Else
                    '
            End Select

            'Failure
            Return Nothing

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Create a data source for a given file name.
        ''' </summary>
        ''' <param name="strFileName">The file to create the data source for.</param>
        ''' <returns>A <see cref="IEwEDataSource">IEwEDataSource</see> or 
        ''' Nothing if creation failed</returns>
        ''' <remarks>The factory will attempt to decipher from the file name
        ''' which <see cref="eDataSourceTypes">type of EwE datasource</see>
        ''' is requred.</remarks>
        ''' -------------------------------------------------------------------
        Public Shared Function Create(ByVal strFileName As String) As IEwEDataSource
            Return Create(GetSupportedType(strFileName))
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' States whether the operating system supports a given type of EwE 
        ''' <see cref="eDataSourceTypes">data source</see>.
        ''' </summary>
        ''' <param name="dst">The type of EwE <see cref="eDataSourceTypes">data source</see>
        ''' to test.</param>
        ''' <returns>True if the system appears to support the given type of
        ''' data source. The check is implemented by the actual data sources. 
        ''' Implementations can range from simple file checks to online driver 
        ''' validations.</returns>
        ''' -------------------------------------------------------------------
        Public Shared Function IsOSSupported(ByVal dst As eDataSourceTypes) As Boolean

            Dim ds As IEwEDataSource = cDataSourceFactory.Create(dst)
            If ds Is Nothing Then Return False
            Return ds.IsOSSupported(dst)

        End Function

    End Class

End Namespace ' DataSources
