' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace DataSources

    ''' =======================================================================
    ''' <summary>
    ''' Base interface for all EwE data access.
    ''' </summary>
    ''' <remarks>
    ''' <para>All data access must be implemented through this interface.</para>
    ''' <para>New Data Sources can be added by inheriting from this interface.</para>
    ''' </remarks>
    ''' =======================================================================
    Public Interface IEwEDataSource
        Inherits IDisposable

#Region " Generic "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' States whether the data source has unsaved changes.
        ''' </summary>
        ''' <returns>True if the data source has pending changes.</returns>
        ''' -------------------------------------------------------------------
        Function IsModified() As Boolean

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' States whether the data source can be edited.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Function IsReadOnly() As Boolean

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' States whether this data source is read-only specifically because
        ''' another session already holds a write lock on it, as opposed to
        ''' being read-only for any other reason.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Function IsLockedByAnotherSession() As Boolean

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Path of the local, disposable read-only copy this session is
        ''' actually connected to, if any. Empty when this session is not
        ''' operating on a local copy (e.g. it holds the write lock, or this
        ''' data source type has no such concept).
        ''' </summary>
        ''' -------------------------------------------------------------------
        Function LocalReadOnlyCopyPath() As String

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
        ''' <param name="datasourceType">Type of data source to open; specify
        ''' <see cref="eDataSourceTypes.NotSet"/> to automatically determine the
        ''' type of data source.</param>
        ''' <param name="bReadOnly">Flag stating whether a data source should be
        ''' opened as read-only.</param>
        ''' <returns>True if opened successfully.</returns>
        ''' -------------------------------------------------------------------
        Function Open(strName As String, core As cCore,
                      Optional datasourceType As eDataSourceTypes = eDataSourceTypes.NotSet,
                      Optional bReadOnly As Boolean = False) As eDatasourceAccessType

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Create the data source connection, possibly overwriting an existing data source
        ''' </summary>
        ''' <param name="strName">Name of the data source to create.</param>
        ''' <param name="strModelName">Name to assign to the model.</param>
        ''' <param name="core"><see cref="cCore">Core instance</see> that holds the 
        ''' datastructures to read to, and write from.</param>
        ''' <returns>True if successful.</returns>
        ''' -------------------------------------------------------------------
        Function Create(strName As String, strModelName As String, core As cCore) As eDatasourceAccessType

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' States whether a data source is already open.
        ''' </summary>
        ''' <returns>True if the data source is open.</returns>
        ''' -------------------------------------------------------------------
        Function IsOpen() As Boolean

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Close the data source connection
        ''' </summary>
        ''' <returns>True if successful.</returns>
        ''' -------------------------------------------------------------------
        Function Close() As Boolean

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Flag a core object as changed in the data source. The data source
        ''' will consult this information when performing incremental saves.
        ''' </summary>
        ''' <param name="cc">The <see cref="eCoreComponentType">core component</see>
        ''' that changed.</param>
        ''' -------------------------------------------------------------------
        Sub SetChanged(cc As eCoreComponentType)

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the connection to the data (file, database, stream, other?) that
        ''' this data source operates on.
        ''' </summary>
        ''' -------------------------------------------------------------------
        ReadOnly Property Connection() As Object

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the name of the connection to the data (file, database, stream, 
        ''' other?) that this data source operates on.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Function ToString() As String

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the directory associated with a data source.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Function Directory() As String

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the file name associated with a data source.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Function FileName() As String

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the file extension associated with a data source.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Function Extension() As String

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns the version of the data source.
        ''' </summary>
        ''' <returns>A version number.</returns>
        ''' -------------------------------------------------------------------
        Function Version() As Single

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Start a database transaction.
        ''' </summary>
        ''' <returns>
        ''' True if successful.
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
        ''' True if successful.
        ''' </returns>
        ''' <remarks>
        ''' Transactions cannot be nested.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Function EndTransaction(bCommit As Boolean) As Boolean

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Compact a database.
        ''' </summary>
        ''' <param name="strTarget">The target identifying the a new database
        ''' to compact into. If left blank, the current database is compacted 
        ''' and no new database is generated.</param>
        ''' <returns>True if successful.</returns>
        ''' -------------------------------------------------------------------
        Function Compact(strTarget As String) As eDatasourceAccessType

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' States whether the data source is able to compact.
        ''' </summary>
        ''' <param name="strTarget">The target identifying the a new database
        ''' to compact into. If left blank, the current database is compacted 
        ''' and no new database is generated.</param>
        ''' <returns>True if successful.</returns>
        ''' -------------------------------------------------------------------
        Function CanCompact(strTarget As String) As Boolean

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' States whether the operating system supports a given type of EwE 
        ''' <see cref="eDataSourceTypes">data source</see>.
        ''' </summary>
        ''' <param name="dst">The type of EwE <see cref="eDataSourceTypes">data source</see>
        ''' to test.</param>
        ''' <returns>True if the system appears to support the type of EwE datas source.</returns>
        ''' -------------------------------------------------------------------
        Function IsOSSupported(dst As eDataSourceTypes) As Boolean

#End Region ' Generic

    End Interface

End Namespace

