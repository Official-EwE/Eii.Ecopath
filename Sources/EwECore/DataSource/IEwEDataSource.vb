'==============================================================================
'
' $Log: IEwEDataSource.vb,v $
' Revision 1.6  2009/02/26 00:57:29  jeroens
' Added DB compact
'
' Revision 1.5  2009/02/08 17:35:04  jeroens
' Can now force datasource type when opening a new source
'
' Revision 1.4  2009/01/29 16:10:50  jeroens
' Moved cEwEDatabase.eAccessTypes to shared enums
'
' Revision 1.3  2009/01/16 23:51:20  jeroens
' Datasource no longer maitains data state by datatype, but by eCoreComponentType
'
' Revision 1.2  2008/12/10 02:00:32  jeroens
' Moved datasource types to EwEUtils
'
' Revision 1.1  2008/09/26 07:30:13  sherman
' --== DELETED HISTORY ==--
'
'==============================================================================

#Region " Imports "

Option Strict On

Imports System.IO
Imports EwECore.Database
Imports EwEPlugin
Imports EwEUtils.Database
Imports EwEUtils.Core

#End Region ' Imports

Namespace DataSources

#Region " Data source factory "

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Factory for creating data sources
    ''' </summary>
    ''' -------------------------------------------------------------------
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
        ''' Create a data source.
        ''' </summary>
        ''' <param name="db"><see cref="cEwEDatabase">cEwEDatabase</see> to create a datasource for.</param>
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
        ''' Create a data source.
        ''' </summary>
        ''' <param name="dst"><see cref="eDataSourceTypes">Type of the datasource</see> to create.</param>
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
        ''' Create a data source.
        ''' </summary>
        ''' <param name="strFileName">The file to create the data source for.</param>
        ''' <returns>A <see cref="IEwEDataSource">IEwEDataSource</see> or 
        ''' Nothing if creation failed</returns>
        ''' -------------------------------------------------------------------
        Public Shared Function Create(ByVal strFileName As String) As IEwEDataSource
            Return Create(GetSupportedType(strFileName))
        End Function

    End Class

#End Region ' Data source factory

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Base interface for all EwE data access.
    ''' </summary>
    ''' <remarks>
    ''' <para>All data access must be implemented through this interface.</para>
    ''' <para>New Data Sources can be added by inheriting from this interface.</para>
    ''' <para>See <see cref="cEIIDataSource">cEIIDataSource</see> for an example of
    ''' an EII file reading data source.</para>
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Public Interface IEwEDataSource

#Region " Generic "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' States whether the datasource has unsaved changes.
        ''' </summary>
        ''' <returns>True if the datasource has pending changes.</returns>
        ''' -------------------------------------------------------------------
        Function IsModified() As Boolean

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Open an existing data source connection
        ''' </summary>
        ''' <param name="strName">Name of the data source to open. How this parameter
        ''' is interpreted depends on the type of data source that is opened.</param>
        ''' <param name="core"><see cref="cCore">Core instance</see> that holds the 
        ''' datastructures to read to, and write from.</param>
        ''' <returns>True if opened successfully.</returns>
        ''' -------------------------------------------------------------------
        Function Open(ByVal strName As String, ByVal core As cCore, Optional ByVal datasourceType As eDataSourceTypes = eDataSourceTypes.NotSet) As eDatasourceAccessType

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Create the data source connection, possibly overwriting an existing data source
        ''' </summary>
        ''' <param name="strName">Name of the datasource to create.</param>
        ''' <param name="strModelName">Name to assign to the model.</param>
        ''' <param name="core"><see cref="cCore">Core instance</see> that holds the 
        ''' datastructures to read to, and write from.</param>
        ''' <returns>True if succesful.</returns>
        ''' -------------------------------------------------------------------
        Function Create(ByVal strName As String, ByVal strModelName As String, ByVal core As cCore) As eDatasourceAccessType

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' States whether a datasource is already open.
        ''' </summary>
        ''' <returns>True if the datasource is open.</returns>
        ''' -------------------------------------------------------------------
        Function IsOpen() As Boolean

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Close the data source connection
        ''' </summary>
        ''' <returns>True if succesful.</returns>
        ''' -------------------------------------------------------------------
        Function Close() As Boolean

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Flag a core object as changed in the datasource. The datasource
        ''' will consult this information when performing incremental saves.
        ''' </summary>
        ''' <param name="cc">The <see cref="eCoreComponentType">core component</see>
        ''' that changed.</param>
        ''' -------------------------------------------------------------------
        Sub SetChanged(ByVal cc As eCoreComponentType)

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the connection to the data (file, database, stream, other?) that
        ''' this datasource operates on.
        ''' </summary>
        ''' -------------------------------------------------------------------
        ReadOnly Property Connection() As Object

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the name of the connection to the data (file, database, stream, 
        ''' other?) that this datasource operates on.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Function ToString() As String

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns the version of the datasource.
        ''' </summary>
        ''' <returns>A version number.</returns>
        ''' -------------------------------------------------------------------
        Function Version() As Single

        Function BeginTransaction() As Boolean
        Function EndTransaction(ByVal bCommit As Boolean) As Boolean

        Function Compact(ByVal strTarget As String) As Boolean

#End Region ' Generic

    End Interface

End Namespace

