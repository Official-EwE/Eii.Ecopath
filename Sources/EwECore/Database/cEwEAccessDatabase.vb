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

#End Region ' Imports

Namespace Database

    ''' =======================================================================
    ''' <summary>
    ''' Database class specialized for storing and writing EwE data to Microsoft 
    ''' Access databases.
    ''' </summary>
    ''' <remarks>
    ''' This class wraps Microsoft Aaarghcess specifics such as connection
    ''' strings, deals with deficiencies in the SQL implementation of Bill's 
    ''' beast, and provids default field values from the Access DB schema.
    ''' </remarks>
    ''' =======================================================================
    Public Class cEwEAccessDatabase
        Inherits cEwEDatabase

#Region " Private vars "

        ''' <summary>A connection to an OleDb database, if any.</summary>
        Public m_conn As OleDbConnection = Nothing
        ''' <summary>The connection string to connect to a MDB database.</summary>
        Private m_strConnectionMDB As String = "Provider=Microsoft.Jet.OLEDB.4.0; Data Source={0};"
        ''' <summary>The connection string to connect to a ACCDB database.</summary>
        Private m_strConnectionACCDB As String = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Persist Security Info=False;"
        ''' <summary>File name to access database.</summary>
        Private m_strFileName As String = ""

#End Region ' Private vars

#Region " Generic "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Create a new M$ Access database.
        ''' </summary>
        ''' <param name="strDatabase">The file name of the .MDB to create.</param>
        ''' <param name="bOverwrite">States whether an existing database may be overwritten.</param>
        ''' <param name="format">Database format type to use. If not set, the 
        ''' database type is deducted from the <paramref name="strDatabase">database</paramref>.</param>
        ''' <returns>A <see cref="eDatasourceAccessType">eDatasourceAccessType</see> value</returns>
        ''' <remarks>Note that this will NOT open the newly created database.</remarks>
        ''' -------------------------------------------------------------------
        Public Overrides Function Create(ByVal strDatabase As String, _
                ByVal strModelName As String, _
                Optional ByVal bOverwrite As Boolean = False, _
                Optional ByVal format As eDataSourceTypes = eDataSourceTypes.NotSet) As eDatasourceAccessType

            Dim strSource As String = ""
            Dim datResult As eDatasourceAccessType = eDatasourceAccessType.Success

            If format = eDataSourceTypes.NotSet Then
                format = cDataSourceFactory.GetSupportedType(strDatabase)
            End If

            Select Case format
                Case eDataSourceTypes.Access2003
                    strSource = "EwE6.mdb"
                    cLog.Write("Create DB: selected MDB format")
                Case eDataSourceTypes.Access2007
                    strSource = "EwE6.accdb"
                    cLog.Write("Create DB: selected ACCDB format")
                Case Else
                    datResult = eDatasourceAccessType.Failed_UnknownType
                    cLog.Write("Create DB: cannot determine format")
            End Select

            If (datResult = eDatasourceAccessType.Success) Then

                ' Save resource file
                If cResourceUtils.SaveResourceToFile(strSource, strDatabase, bOverwrite, Assembly.GetExecutingAssembly()) Then
                    Try
                        'Try to open the database to update the model name
                        Dim db As New cEwEAccessDatabase()
                        datResult = db.Open(strDatabase, format)
                        If (datResult = eDatasourceAccessType.Opened) Then
                            db.Execute(String.Format("UPDATE EcopathModel SET Name='{0}', Author='{1}' WHERE ModelID=1", strModelName, cSystemUtils.GetUserName()))
                            Try
                                ' Egg - over-easy but slightly obfuscated ;)
                                If strModelName.ToLower().Contains(cStringUtils.Shift("Dbsm!Xbmufst").ToLower()) Then
                                    db.Execute(String.Format("UPDATE EcopathGroup SET GroupName='{0}' WHERE GroupID=1", cStringUtils.Shift("Dijdlfo!tiju")))
                                    db.Execute(String.Format("UPDATE EcopathFleet SET FleetName='{0}' WHERE FleetID=1", cStringUtils.Shift("Tfbm!cbtifst")))
                                End If
                            Catch ex As Exception
                                ' Do not let eggs make the pot explode
                                cLog.Write("Create DB: found a rotten egg: " & ex.Message)
                            End Try
                            db.Close()
                        Else
                            cLog.Write("Create DB: Unable to open DB using required drivers, check driver installation.")
                        End If
                        db = Nothing
                    Catch ex As Exception
                        cLog.Write(ex)
                        datResult = eDatasourceAccessType.Failed_Unknown
                    End Try
                Else
                    'Unable to write to target location
                    cLog.Write("Create DB: Unable to save to target location " & strDatabase)
                    datResult = eDatasourceAccessType.Failed_CannotSave
                End If
            End If
            Return datResult

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

            Dim datResult As eDatasourceAccessType = eDatasourceAccessType.Success
            Dim strDatabaseFrom As String = Me.Name
            Dim bSucces As Boolean = True

            If databaseType = eDataSourceTypes.NotSet Then
                databaseType = cDataSourceFactory.GetSupportedType(strDatabaseTo)
            End If

            ' Databases are copied from one spot to another, not using proper database replication
            ' Therefore, check if source and target types will remain unchanged
            If databaseType <> cDataSourceFactory.GetSupportedType(strDatabaseFrom) Then
                Return eDatasourceAccessType.Failed_TransferTypes
            End If

            Me.Close()

            ' Test if we can create a new DB at the intended location
            datResult = Me.Create(strDatabaseTo, strModelName, bOverwrite)

            ' Succes?
            If (datResult = eDatasourceAccessType.Success) Then

                ' #Yes: this is painful... File Copy the current DB on top of the newly created DB
                Try
                    ' Can copy databse from old to new MDB?
                    System.IO.File.Copy(strDatabaseFrom, strDatabaseTo, True)
                Catch ex As Exception
                    ' #Failure
                    datResult = eDatasourceAccessType.Failed_CannotSave
                End Try

                datResult = Me.Open(strDatabaseTo, databaseType)
                'Able to open?
                If datResult = eDatasourceAccessType.Opened Then
                    ' #Yes: Fix model name after copying
                    Me.Execute(String.Format("UPDATE EcopathModel SET NAME='{0}' WHERE (ModelID=1)", strModelName))
                Else
                    ' #No: Open ye olde database
                    Me.Open(strDatabaseFrom)
                End If
            End If
            Return datResult

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Open a connection to a M$ Access database.
        ''' </summary>
        ''' <param name="strDatabase">The database to open.</param>
        ''' <param name="databaseType">Type to use to open the database. Set this
        ''' to 'NotSet' to auto-detect the database type.</param>
        ''' <returns>True if connected succesfully.</returns>
        ''' -------------------------------------------------------------------
        Public Overrides Function Open(ByVal strDatabase As String, _
                                       Optional ByVal databaseType As eDataSourceTypes = eDataSourceTypes.NotSet) As eDatasourceAccessType

            ' Pre
            Debug.Assert(Not String.IsNullOrEmpty(strDatabase), "Invalid data source specified")
            Debug.Assert(Not Me.IsConnected(), "Connection already open, close first")

            Dim datResult As eDatasourceAccessType = eDatasourceAccessType.Failed_Unknown

            ' Does file exist?
            If Not File.Exists(strDatabase) Then Return eDatasourceAccessType.Failed_FileNotFound

            ' Test read-only file attributes
            If (File.GetAttributes(strDatabase) And FileAttributes.ReadOnly) = FileAttributes.ReadOnly Then
                Return eDatasourceAccessType.Failed_ReadOnly
            End If

            ' Need to auto-detect database type?
            If databaseType = eDataSourceTypes.NotSet Then
                ' #Yes: auto-detect
                databaseType = cDataSourceFactory.GetSupportedType(strDatabase)
            End If

            Me.m_conn = New OleDbConnection()

            ' Try to assemble connection string
            Select Case databaseType
                Case eDataSourceTypes.Access2003
                    Me.m_conn.ConnectionString = String.Format(m_strConnectionMDB, strDatabase)
                Case eDataSourceTypes.Access2007
                    Me.m_conn.ConnectionString = String.Format(m_strConnectionACCDB, strDatabase)
                Case eDataSourceTypes.NotSet
                    Me.m_conn.ConnectionString = ""
                    datResult = eDatasourceAccessType.Failed_UnknownType
            End Select

            If Not String.IsNullOrEmpty(Me.m_conn.ConnectionString) Then

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
                Case eDataSourceTypes.Access2003
                    conn.ConnectionString = String.Format(m_strConnectionMDB, strDatabase)
                Case eDataSourceTypes.Access2007
                    conn.ConnectionString = String.Format(m_strConnectionACCDB, strDatabase)
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
        ''' -------------------------------------------------------------------
        Public Overrides Function CanCompact(ByVal strConnectionFrom As String, ByVal strConnectionTo As String) As Boolean

            Dim compact As IDatabaseCompact = cDatabaseCompactFactory.GetDatabaseCompact(strConnectionFrom)
            If (compact Is Nothing) Then Return False
            Return compact.CanCompact()

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Compact the current M$ Access database.
        ''' </summary>
        ''' <param name="strFileFrom">Source database to compact.</param>
        ''' <param name="strFileTo">Target database to compact to. Can be left blank.</param>
        ''' <returns>True if succesful.</returns>
        ''' <remarks>
        ''' Only MDB databases can be compacted for now. Note that the database
        ''' cannot be <see cref="IsConnected">connected</see> when compacting.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Overrides Function Compact(ByVal strFileFrom As String, _
                                          ByVal strFileTo As String) As eDatasourceAccessType

            ' Fix params
            If String.IsNullOrEmpty(strFileTo) Then strFileTo = strFileFrom

            Dim bCompactToOriginal As Boolean = (String.Compare(strFileFrom, strFileTo, True) = 0)
            Dim strConnectionFrom As String = ""
            Dim strConnectionTo As String = ""
            Dim strConnection As String = ""
            Dim result As eDatasourceAccessType = eDatasourceAccessType.Success
            Dim comp As IDatabaseCompact = cDatabaseCompactFactory.GetDatabaseCompact(strFileFrom)

            Select Case cDataSourceFactory.GetSupportedType(strFileFrom)
                Case eDataSourceTypes.Access2003
                    strConnection = Me.m_strConnectionMDB
                Case eDataSourceTypes.Access2007
                    ' Accdb needs different compaction engine, no idea how to do that for now
                    strConnection = Me.m_strConnectionACCDB
                Case Else
                    ' Not supported
                    strConnection = ""
            End Select

            ' No connection string for compacting this type of database?
            If String.IsNullOrEmpty(strConnection) Then Return eDatasourceAccessType.Failed_OSUnsupported
            ' Cannot compact when connected
            Debug.Assert(Me.IsConnected = False)

            Try

                ' #Yes: try to compact
                Dim strDBToOrg As String = strFileTo
                ' Identical database specified for in and out?
                If (bCompactToOriginal) Then
                    ' #Yes: compact DB to temp location
                    strFileTo = System.IO.Path.GetTempFileName()
                End If

                If File.Exists(strFileTo) Then
                    Try
                        File.Delete(strFileTo)
                    Catch ex As Exception
                        Return eDatasourceAccessType.Failed_CannotSave
                    End Try
                End If

                ' Set up connections
                strConnectionFrom = String.Format(strConnection, strFileFrom)
                strConnectionTo = String.Format(strConnection, strFileTo)

                ' Attempt to Compact
                result = comp.Compact(strFileFrom, strConnectionFrom, strFileTo, strConnectionTo)
                ' Is successful?
                If (result <> eDatasourceAccessType.Success) Then
                    ' #No: report compact failure
                    Return result
                End If

                ' Is succesfully compacted?
                If File.Exists(strFileTo) Then
                    ' #Yes: Need to copy from temp location?
                    If (bCompactToOriginal) Then
                        ' #Yes: Overwrite original db with compacted db
                        File.Copy(strFileTo, strDBToOrg, True)
                        ' Delete temp location compacted database
                        File.Delete(strFileTo)
                    End If
                End If
                Return eDatasourceAccessType.Success

            Catch ex As Exception
                ' Wow
            End Try

            Return eDatasourceAccessType.Failed_Unknown

        End Function

        Public Overrides Function MaxDBVersion() As Single
            Return cDatabaseUpdater.MaxSupportedVersion
        End Function

#End Region ' Overrides

#Region " Private helper methods "

#If 0 Then

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' A superb attempt to validate and fix datarow values right before cramming it into a MDB.
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="args"></param>
        ''' <remarks>
        ''' Although a brilliant idea, this does not work. For some reason Access' Required field
        ''' flag does not translate in a proper AllowDBNull value; AllowDBNull remains True for
        ''' every single field.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Private Sub OnRowUpdating(ByVal sender As Object, ByVal args As OleDbRowUpdatingEventArgs)

            Dim drow As DataRow = args.Row
            Dim dtable As DataTable = drow.Table
            Dim dtSchema As DataTable = Nothing

            ' Is this an INSERT or UPDATE command?
            If args.StatementType = StatementType.Insert Or args.StatementType = StatementType.Update Then

                ' #Yes: check for DBNull values in every column
                For Each col As DataColumn In dtable.Columns
                    ' Is this a DBnull value?
                    If Convert.IsDBNull(drow(col.ColumnName)) Then
                        ' #Yes, hmmm.. now fix only fields that may not be null, do not autoincrement 
                        ' and cannot be unique to prevent conflicts when substituting default value.
                        If (Not col.AllowDBNull) And _
                           (Not col.AutoIncrement) And _
                           (Not col.Unique) And _
                           (Not col.ReadOnly) Then

                            ' Store default value the cell
                            drow(col.ColumnName) = col.DefaultValue
                            ' Tell the row to shut up
                            drow.AcceptChanges()
                            Console.WriteLine("   - Applied default value to {0}.{1}, value {2}", args.TableMapping.SourceTable, col.ColumnName, col.DefaultValue)
                        End If
                    End If
                Next
            End If
        End Sub

#End If

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Event handler, called whenever a <see cref="DataRow">DataRow</see> is updated 
        ''' into the database via an adapter to ensure that Autonumber values are properly
        ''' reflected in the DataRow.
        ''' </summary>
        ''' <remarks>
        ''' <para>This handler solves a problem that occurs when inserting or updating
        ''' rows with an Autonumber ID. Whenever such a row is created and written to the
        ''' database, the number reflected in the <see cref="DataRow">DataRow</see> will 
        ''' differ from the actual value in the database.</para>
        ''' <para>The solution implemented here only works for M$ Access 2000 and newer.</para>
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Private Sub OnRowUpdated(ByVal sender As Object, ByVal args As OleDbRowUpdatedEventArgs)
            ' Include a variable and a command to retrieve the identity value from the Access database.
            Dim nIDNew As Integer = 0
            Dim cmd As OleDbCommand = Nothing
            Dim drow As DataRow = args.Row
            Dim dtable As DataTable = drow.Table

            ' Only worry about INSERT commands (where the new Autonum value is defined)
            If args.StatementType = StatementType.Insert Then
                ' #Yes

                drow = args.Row
                dtable = drow.Table

                If dtable Is Nothing Then Return

                ' Check every column
                For Each col As DataColumn In dtable.Columns
                    ' Only update Autonumber fields
                    If col.AutoIncrement Then
                        ' Prepare query to obtain the actual ID.
                        ' Note that this only works for Access 2000 and higher.
                        cmd = New OleDbCommand("SELECT @@IDENTITY", Me.m_conn)
                        ' Retrieve the identity value
                        nIDNew = CInt(cmd.ExecuteScalar())
                        ' Store it in the row
                        drow(col.ColumnName) = nIDNew
                        ' Tell the row to shut up
                        drow.AcceptChanges()

                        Console.WriteLine("   - Fixed Autonumber column {0}.{1}, value {2}", args.TableMapping.SourceTable, col.ColumnName, nIDNew)
                    End If
                Next
            End If
        End Sub

#End Region ' Careful with that axe, Eugene

    End Class

End Namespace
