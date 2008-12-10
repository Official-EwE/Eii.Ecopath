'==============================================================================
'
' $Log: IEwEDataSource.vb,v $
' Revision 1.2  2008/12/10 02:00:32  jeroens
' Moved datasource types to EwEUtils
'
' Revision 1.1  2008/09/26 07:30:13  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.46  2008/07/25 14:21:12  jeroens
' Fixing improved file access feedback
'
' Revision 1.45  2008/07/25 03:00:47  jeroens
' Incorporating new file extensions (w Joe)
' Adding error diagnostics on file access
'
' Revision 1.44  2008/07/25 01:39:10  joeh
' Modify to cater the generic datasource engine in the core
'
' Revision 1.43  2008/07/25 00:06:43  jeroens
' Included new file extensions
'
' Revision 1.42  2008/07/09 14:36:28  jeroens
' Added generic type test
'
' Revision 1.41  2008/07/09 13:29:27  jeroens
' Added accdb format support
'
' Revision 1.40  2008/06/06 15:55:57  joeb
' Moved eDataTypes to EwEUtils.Core
'
' Revision 1.39  2007/12/18 22:19:24  jeroens
' + Added interface for controlling datasource transactions
'
' Revision 1.38  2007/12/13 17:15:28  jeroens
' * Changed SaveModelAs / Database replication structure
'
' Revision 1.37  2007/12/09 22:12:20  jeroens
' + Added IsModified
'
' Revision 1.36  2007/09/17 02:45:35  jeroens
' * Database created with a model name
'
' Revision 1.35  2007/07/25 03:08:08  jeroens
' * Moved cEwEDatabase to EwEUtils
'
' Revision 1.34  2007/07/21 14:49:31  jeroens
' + Version exposed by datasource
'
' Revision 1.33  2007/07/17 16:24:25  jeroens
' + Added basis for copying across datasources
' * Changed TS support
'
' Revision 1.32  2007/07/13 17:24:34  jeroens
' - Removed Forcing namespace
'
' Revision 1.31  2007/04/10 17:24:15  jeroens
' * Change flags can be set w/o specifyng a DBID
'
' Revision 1.30  2007/02/27 03:58:36  jeroens
' - Removed FileName. This class uses an abstract connection; filename countered this principle. Instead, the function ToString has been added that must be overridden to provide a string representation of a datasources' connection
'
'==============================================================================

Option Strict On

Imports System.IO
Imports EwECore.Database
Imports EwEPlugin
Imports EwEUtils.Database
Imports EwEUtils.Core

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
        Function Open(ByVal strName As String, ByVal core As cCore) As cEwEDatabase.eAccessType

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
        Function Create(ByVal strName As String, ByVal strModelName As String, ByVal core As cCore) As cEwEDatabase.eAccessType

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
        ''' <param name="dataType">The <see cref="eDataTypes">Type</see> of the object that changed.</param>
        ''' <param name="iDBID">The database ID of the object that changed.</param>
        ''' -------------------------------------------------------------------
        Sub SetChanged(ByVal dataType As eDataTypes, Optional ByVal iDBID As Integer = 0)

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

#End Region ' Generic

    End Interface

End Namespace

