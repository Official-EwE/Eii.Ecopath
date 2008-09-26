'==============================================================================
'
' $Log: cEwEAccessDatabase.vb,v $
' Revision 1.1  2008/09/26 07:30:17  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.16  2008/07/25 14:21:10  jeroens
' Fixing improved file access feedback
'
' Revision 1.15  2008/07/25 03:00:45  jeroens
' Incorporating new file extensions (w Joe)
' Adding error diagnostics on file access
'
' Revision 1.14  2008/07/25 01:39:10  joeh
' Modify to cater the generic datasource engine in the core
'
' Revision 1.13  2008/07/25 00:05:27  jeroens
' Uses proper supporting class to detect type of conection to use
'
' Revision 1.12  2008/07/09 14:37:02  jeroens
' Added accdb generation
'
' Revision 1.11  2008/07/09 13:28:30  jeroens
' Added accdb format support
'
' Revision 1.10  2008/03/14 01:52:52  jeroens
' Fixed CLS compliancy warning
'
' Revision 1.9  2008/02/13 03:54:38  jeroens
' New model name and author are set
'
' Revision 1.8  2007/12/13 17:15:27  jeroens
' * Changed SaveModelAs / Database replication structure
'
' Revision 1.7  2007/09/17 02:45:27  jeroens
' * Database created with a model name
'
' Revision 1.6  2007/09/05 14:15:00  jeroens
' * Disabled autonum correction logic since EwE no longer uses autonum values
'
' Revision 1.5  2007/08/27 17:38:14  jeroens
' + Added Property FileName
'
' Revision 1.4  2007/07/25 03:08:38  jeroens
' * Moved cEwEDatabase to EwEUtils
'
' Revision 1.3  2007/02/27 04:02:31  jeroens
' + Added mustoverride property Name
'
' Revision 1.2  2006/07/03 04:28:24  jeroens
' - Removed CreateDBCommand
' * GetAdapter extents base class implementation
'
' Revision 1.1  2006/07/01 04:23:49  jeroens
' + Initial version, split off from cEwEDatabase containing all MDB/OleDb specific logic
'
'==============================================================================

Option Strict On

Imports System.Data.OleDb
Imports EwECore.DataSources
Imports EwEUtils.Database

Namespace Database

    ''' <summary>
    ''' 
    ''' </summary>
    Public Class cEwEAccessDatabase
        : Inherits cEwEDatabase

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
        ''' <returns>A <see cref="eAccessType">eAccessType</see> value</returns>
        ''' <remarks>Note that this will NOT open the newly created database.</remarks>
        ''' -------------------------------------------------------------------
        Public Overrides Function Create(ByVal strDatabase As String, _
                ByVal strModelName As String, _
                Optional ByVal bOverwrite As Boolean = False) As eAccessType

            Dim strSource As String = ""
            Dim datResult As eAccessType = eAccessType.Created

            Select Case cDataSourceFactory.GetSupportedType(strDatabase)
                Case cDataSourceFactory.eDataSourceTypes.MDB
                    strSource = "EwE6.mdb"
                Case cDataSourceFactory.eDataSourceTypes.ACCDB
                    strSource = "EwE6.accdb"
                Case Else
                    datResult = eAccessType.Failed_UnknownType
            End Select

            If (datResult = eAccessType.Created) Then

                If cCoreResources.SaveResourceToFile(strSource, strDatabase, bOverwrite) Then
                    Try
                        'Try to open the database to update the model name
                        Dim db As New cEwEAccessDatabase()
                        datResult = db.Open(strDatabase)
                        If (datResult = eAccessType.Opened) Then
                            db.Execute(String.Format("UPDATE EcopathModel SET Name='{0}', Author='{1}' WHERE ModelID=1", strModelName, EwEUtils.SystemUtilities.GetUserName()))
                            db.Close()
                        End If
                        db = Nothing
                    Catch ex As Exception
                        datResult = eAccessType.Failed_Unknown
                    End Try
                Else
                    'Unable to write to target location
                    datResult = eAccessType.Failed_CannotSave
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
                Optional ByVal bOverwrite As Boolean = False) As eAccessType

            Dim datResult As eAccessType = eAccessType.Created
            Dim strDatabaseFrom As String = Me.Name
            Dim bSucces As Boolean = True

            ' Databases are copied from one spot to another, not using proper database replication
            ' Therefore, check if source and target types will remain unchanged
            If cDataSourceFactory.GetSupportedType(strDatabaseTo) <> cDataSourceFactory.GetSupportedType(strDatabaseFrom) Then
                Return eAccessType.Failed_TransferTypes
            End If

            Me.Close()

            ' Test if we can create a new DB at the intended location
            datResult = Me.Create(strDatabaseTo, strModelName, bOverwrite)

            ' Succes?
            If (datResult = eAccessType.Created) Then

                ' #Yes: this is painful... File Copy the current DB on top of the newly created DB
                Try
                    ' Can copy databse from old to new MDB?
                    System.IO.File.Copy(strDatabaseFrom, strDatabaseTo, True)
                Catch ex As Exception
                    ' #Failure
                    datResult = eAccessType.Failed_CannotSave
                End Try

                datResult = Me.Open(strDatabaseTo)
                'Able to open?
                If datResult = eAccessType.Opened Then
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
        ''' <returns>True if connected succesfully.</returns>
        ''' -------------------------------------------------------------------
        Public Overrides Function Open(ByVal strDatabase As String) As eAccessType

            ' Pre
            Debug.Assert(Not String.IsNullOrEmpty(strDatabase), "Invalid data source specified")
            Debug.Assert(Not Me.IsConnected(), "Connection already open, close first")

            Dim datResult As eAccessType = eAccessType.Opened

            Me.m_conn = New OleDbConnection()

            Select Case cDataSourceFactory.GetSupportedType(strDatabase)
                Case cDataSourceFactory.eDataSourceTypes.MDB
                    Me.m_conn.ConnectionString = String.Format(m_strConnectionMDB, strDatabase)
                Case cDataSourceFactory.eDataSourceTypes.ACCDB
                    Me.m_conn.ConnectionString = String.Format(m_strConnectionACCDB, strDatabase)
                Case cDataSourceFactory.eDataSourceTypes.NotSupported
                    datResult = eAccessType.Failed_UnknownType
            End Select
            If datResult = eAccessType.Opened Then

                Try
                    Me.m_conn.Open()
                Catch e As Exception
                    Console.WriteLine("** Exception {0} when opening Access db {1}", e.Message, strDatabase)
                    datResult = eAccessType.Failed_OSUnsupported
                End Try

                ' All well: store file name
                Me.m_strFileName = strDatabase
                ' Report succes
                If Not Me.IsConnected() Then
                    datResult = eAccessType.Failed_Unknown
                End If

            End If

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

            Catch ex As Exception
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
