#Region " Imports "

Option Strict On

Imports System.IO
Imports EwECore.Database
Imports EwEPlugin
Imports EwEUtils.Database
Imports EwEUtils.Core

#End Region ' Imports

Namespace DataSources

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
        ''' Clears any modified flags (use with care!)
        ''' </summary>
        ''' -------------------------------------------------------------------
        Sub ClearChanged()

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

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Start a database transaction.
        ''' </summary>
        ''' <returns>
        ''' True if succesful.
        ''' </returns>
        ''' <remarks>
        ''' Transactions cannot be nested.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Function BeginTransaction() As Boolean

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' End a database transaction.
        ''' </summary>
        ''' <param name="bCommit">States whether the transaction should be 
        ''' committed (True) or reverted (False).</param>
        ''' <returns>
        ''' True if succesful.
        ''' </returns>
        ''' <remarks>
        ''' Transactions cannot be nested.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Function EndTransaction(ByVal bCommit As Boolean) As Boolean

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Compact a database.
        ''' </summary>
        ''' <param name="strTarget">The target identifying the a new database
        ''' to compact into. If left blank, the current database is compacted 
        ''' and no new database is generated.</param>
        ''' <returns>True if succesful.</returns>
        ''' -------------------------------------------------------------------
        Function Compact(ByVal strTarget As String) As eDatasourceAccessType

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' States whether the datasource is able to compact.
        ''' </summary>
        ''' <param name="strTarget">The target identifying the a new database
        ''' to compact into. If left blank, the current database is compacted 
        ''' and no new database is generated.</param>
        ''' <returns>True if succesful.</returns>
        ''' -------------------------------------------------------------------
        Function CanCompact(ByVal strTarget As String) As Boolean

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' States whether the local OS supports connecting to a datasource
        ''' of a given type.
        ''' </summary>
        ''' <param name="dst"></param>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Function IsOSSupported(ByVal dst As eDataSourceTypes) As Boolean

#End Region ' Generic

    End Interface

End Namespace

