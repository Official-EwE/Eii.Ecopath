#Region " Imports "

Option Strict On

Imports System
Imports System.Diagnostics
Imports System.Data
Imports System.Data.OleDb
Imports System.Data.SqlClient
Imports System.Data.Common
Imports System.Reflection
Imports System.ComponentModel
Imports System.Collections.Generic
Imports EwEUtils.Core
Imports EwEUtils.Utilities

#End Region ' Imports

#If VERBOSE Then
#Const VERBOSE_LEVEL = 4
#End If

Namespace Database

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Generic base class for implementing a DBMS-specific EwE database
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public MustInherit Class cEwEDatabase

#Region " Class cEwEDbWriter "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Helper class that eases the process of adding records to a table.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Class cEwEDbWriter

            ''' <summary>Database to write to</summary>
            Private m_db As cEwEDatabase = Nothing
            ''' <summary>Table in the database to write to</summary>
            Private m_strTable As String = ""
            ''' <summary>Field dictating sequence order of rows that must be maintained</summary>
            Private m_strSequenceField As String = ""
            ''' <summary>Sequence subgroup filter</summary>
            Private m_strSequenceFilter As String = ""
            ''' <summary>DataSet contains a mirror of the indicated table</summary>
            Private m_ds As DataSet = Nothing
            ''' <summary>DataTable that mirrors the indicated table</summary>
            Private m_dt As DataTable = Nothing
            ''' <summary>Adapter to sync table content back and forth</summary>
            Private m_apt As IDataAdapter = Nothing

            Private m_dtSchema As DataTable = Nothing

            ''' ---------------------------------------------------------------
            ''' <summary>
            ''' <para>Constructor, initializes a new instance of a cEwEDbWriter.</para>
            ''' </summary>
            ''' <param name="db">The <see cref="cEwEDatabase">cEwEDatabase</see> to read from.</param>
            ''' <param name="strTable">The name of the table to link to.</param>
            ''' <param name="strSequenceField">Field that dictates the order of items in a particular table. 
            ''' The writer will safeguard the order of items if this field is provided.</param>
            ''' <param name="strSequenceFilter">Optional sequence field subgrouping clause, such
            ''' as 'ScenarioID=2'. The writer will manage the sequence order of only those rows
            ''' that match this filter.</param>
            ''' <remarks>
            ''' <para>This method will attempt to connect and read the table into its internal
            ''' structures. It might be prudent to validate whether the instance is connected
            ''' by calling <see cref="IsConnected">IsConnected</see> prior to using it.</para>
            ''' </remarks>
            ''' ---------------------------------------------------------------
            Public Sub New(ByRef db As cEwEDatabase, ByVal strTable As String, _
                    Optional ByVal strSequenceField As String = "", Optional ByVal strSequenceFilter As String = "")
                Me.Connect(db, strTable, strSequenceField, strSequenceFilter)
            End Sub

            ''' ---------------------------------------------------------------
            ''' <summary>
            ''' Attempts to connect to the database and read the table data.
            ''' </summary>
            ''' <param name="db">The <see cref="cEwEDatabase">cEwEDatabase</see> to read from.</param>
            ''' <param name="strTable">The name of the table to link to.</param>
            ''' <returns>True if connected.</returns>
            ''' ---------------------------------------------------------------
            Public Function Connect(ByRef db As cEwEDatabase, ByVal strTable As String, _
                    Optional ByVal strSequenceField As String = "", Optional ByVal strSequenceFilter As String = "") As Boolean

                ' Pre
                Debug.Assert(db IsNot Nothing, "Need a valid database")
                Debug.Assert(Not String.IsNullOrEmpty(strTable), "Need a table name")

                Dim conn As IDbConnection = db.GetConnection()

                ' Remember these
                Me.m_db = db
                Me.m_strTable = strTable
                Me.m_strSequenceField = strSequenceField
                Me.m_strSequenceFilter = strSequenceFilter
                Me.m_dtSchema = Nothing

                ' OLEDB hack
                If TypeOf conn Is OleDbConnection Then
                    Me.m_dtSchema = DirectCast(conn, OleDbConnection).GetSchema("Columns", New String() {Nothing, Nothing, strTable, Nothing})
                End If

                ' Get adapter
                Me.m_apt = Me.m_db.GetAdapter(String.Format("Select * from {0}", strTable))
                ' Adapter gotten succesfully?
                If (Me.m_apt IsNot Nothing) Then
                    ' #Yes: Get dataset
                    Me.m_ds = Me.m_db.GetDataSet(Me.m_apt, strTable)
                    ' Dataset obtained succesfully?
                    If (Me.m_ds IsNot Nothing) Then
                        ' #Yes: read the data
                        Me.m_apt.Fill(Me.m_ds)
                        ' Set up DataTable for making modifications
                        Me.m_dt = Me.m_ds.Tables(0)
                    Else
                        ' #No: dataset failed, release adapter
                        Me.m_db.ReleaseAdapter(Me.m_apt)
                        Me.m_apt = Nothing
                        ' Release the rest as well, why not
                        Me.m_db = Nothing
                        Me.m_strTable = ""
                    End If
                End If
                ' Return connection state
                Return Me.IsConnected()

            End Function

            ''' ---------------------------------------------------------------
            ''' <summary>
            ''' Commit all pending changes in the cEwEDBWriter without closing
            ''' the writer; the writer is left open for further database operations.
            ''' </summary>
            ''' ---------------------------------------------------------------
            Public Sub Commit()

                ' Optimizations
                If Not Me.IsConnected Then Return
                If Not Me.m_ds.HasChanges() Then Return

                If Me.m_dtSchema IsNot Nothing Then
                    ' Fix unwanted nulls in new and modified rows
                    Dim adrows() As DataRow = Me.m_dt.Select()
                    For Each drow As DataRow In adrows
                        If drow.RowState = DataRowState.Added Or drow.RowState = DataRowState.Modified Then
                            Me.FixUnwantedDBNulls(drow)
                        End If
                    Next
                End If
                Me.m_db.CommitDataSet(Me.m_ds, Me.m_apt, Me.m_strTable)

            End Sub

            ''' ---------------------------------------------------------------
            ''' <summary>
            ''' Disconnects from the database.
            ''' </summary>
            ''' <param name="bSaveChanges">States whether changes need to be saved (true)
            ''' or discarded (false).</param>
            ''' ---------------------------------------------------------------
            Public Sub Disconnect(Optional ByVal bSaveChanges As Boolean = True)

                If Not Me.IsConnected Then Return

                If bSaveChanges Then
                    Me.Commit()
                End If

                Me.m_db.ReleaseDataSet(Me.m_ds)
                Me.m_db.ReleaseAdapter(Me.m_apt)

                Me.m_dt = Nothing
                Me.m_ds = Nothing
                Me.m_apt = Nothing
                Me.m_db = Nothing
                Me.m_strTable = ""
            End Sub

            ''' ---------------------------------------------------------------
            ''' <summary>
            ''' States whether currently connected.
            ''' </summary>
            ''' <returns>True if connected.</returns>
            ''' ---------------------------------------------------------------
            Public Function IsConnected() As Boolean
                Return Me.m_apt IsNot Nothing
            End Function

            ''' ---------------------------------------------------------------
            ''' <summary>
            ''' Returns an empty row for the given table to populate values into.
            ''' </summary>
            ''' <returns>An empty row</returns>
            ''' <remarks>Note that this empty row is not yet added to the table. 
            ''' If the row is populated to satisfaction, call <see cref="AddRow">AddRow</see>
            ''' to add it to the the list of rows waiting to be added to the database.</remarks>
            ''' ---------------------------------------------------------------
            Public Function NewRow() As DataRow
                Try
                    Return Me.m_dt.NewRow()
                Catch ex As Exception
                    Debug.Assert(False, ex.Message)
                    Return Nothing
                End Try
            End Function

            ''' ---------------------------------------------------------------
            ''' <summary>
            ''' Adds a row previously obtained from <see cref="NewRow">NewRow</see>
            ''' to the list of rows waiting to be added to the database.
            ''' </summary>
            ''' <remarks>
            ''' <para>This method will preserve and re-align a sequence field if specified
            ''' in the <see cref="cEwEDbWriter">Constructor</see>.</para>
            ''' <para>Use <see cref="cEwEDbWriter.RemoveRow">RemoveRow</see> to protect the
            ''' row sequence during deletes.</para>
            ''' </remarks>
            ''' ---------------------------------------------------------------
            Public Sub AddRow(ByVal drow As DataRow)

                Dim nSequence As Integer = 0      ' Seq. no of the new row
                Dim nSequenceIndex As Integer = 0 ' Seq. counter of the table
                Dim nSeqTemp As Integer = 1       ' Temporary sequence number
                Dim rowsTemp() As DataRow = Nothing
                Dim rowTemp As DataRow = Nothing

                ' Need to update sequence field?
                If (Not String.IsNullOrEmpty(Me.m_strSequenceField)) Then
                    ' #Yes: Get sequence number from new row
                    nSequence = CInt(drow(Me.m_strSequenceField))
                    ' Sort table on existing sequence numbers
                    rowsTemp = Me.m_dt.Select(Me.m_strSequenceFilter, String.Format("{0} ASC", Me.m_strSequenceField))

                    ' Must determine new sequence number?
                    If (nSequence <= 0) Then
                        ' #Yes: Find last sequence number to add row last in sequence
                        ' Are there any rows at all?
                        If rowsTemp.Length > 0 Then
                            ' #Yes: get sequence number from the last row
                            rowTemp = rowsTemp(rowsTemp.Length - 1)
                            ' New row will be placed after this
                            nSequence = CInt(rowTemp(Me.m_strSequenceField)) + 1
                        Else
                            ' #No, there are no rows. Start at the beginning
                            nSequence = 1
                        End If
                    End If ' Must determine new seq number

                    ' Re-align sequence numbers in the table
                    For nRow As Integer = 0 To rowsTemp.Length - 1

                        ' Get the row
                        rowTemp = rowsTemp(nRow)
                        ' Is this a valid row?
                        If (rowTemp IsNot Nothing) Then

                            ' #Yes: Get sequence number of this row
                            If Not Convert.IsDBNull(rowTemp(Me.m_strSequenceField)) Then
                                nSeqTemp = CInt(rowTemp(Me.m_strSequenceField))
                            Else
                                nSeqTemp = nSequence
                            End If

                            ' Is this the spot where the new row should go?
                            If (nSeqTemp = nSequence) Then
                                ' #Yes: leave a spot in the sequence index
                                nSequenceIndex += 2
                            Else
                                ' #No: just increment the sequence index
                                nSequenceIndex += 1
                            End If

                            ' Update row sequence number
                            rowTemp(Me.m_strSequenceField) = nSequenceIndex

                        End If ' Is a valid row
                    Next ' Re-align sequence numbers
                End If ' Need to update sequence field

                ' Now finally add the new row
                Me.m_dt.Rows.Add(drow)

            End Sub

            ''' ---------------------------------------------------------------
            ''' <summary>
            ''' Returns an arbitrary row maintained in the writer.
            ''' </summary>
            ''' <param name="drow">The datarow to delete.</param>
            ''' <returns>The row.</returns>
            ''' <remarks>
            ''' <para>This method will preserve and re-align a sequence field if specified
            ''' in the <see cref="cEwEDbWriter">Constructor</see>.</para>
            ''' <para>Use <see cref="cEwEDbWriter.AddRow">AddRow</see> to protect the
            ''' row sequence during additions.</para>
            ''' </remarks>
            ''' ---------------------------------------------------------------
            Public Function RemoveRow(ByVal drow As DataRow) As Boolean

                Dim nSequence As Integer = 0      ' Seq. no of the new row
                Dim rowsTemp() As DataRow = Nothing
                Dim rowTemp As DataRow = Nothing

                ' Need to update sequence field?
                If (Not String.IsNullOrEmpty(Me.m_strSequenceField)) Then
                    ' #Yes: Get sequence number from new row
                    nSequence = CInt(drow(Me.m_strSequenceField))
                    ' Remove the row
                    Me.m_dt.Rows.Remove(drow)
                    ' Sort remaining rows by sequence number
                    rowsTemp = Me.m_dt.Select(String.Format("{0} > {1}", Me.m_strSequenceField, nSequence), _
                            String.Format("{0} ASC", Me.m_strSequenceField))

                    ' Re-align sequence numbers in these rows
                    For nRow As Integer = 0 To rowsTemp.Length - 1
                        ' Get the row
                        rowTemp = rowsTemp(nRow)
                        ' Is this a valid row?
                        If (rowTemp IsNot Nothing) Then
                            ' #Yes: Get sequence number of this row
                            nSequence = CInt(rowTemp(Me.m_strSequenceField))
                            ' Tuck it back in, lowered by one
                            rowTemp(Me.m_strSequenceField) = nSequence - 1
                        End If ' Is a valid row
                    Next ' Re-align sequence numbers
                End If ' Need to update sequence field

                Return True
            End Function

            ''' ---------------------------------------------------------------
            ''' <summary>
            ''' Returns an arbitrary row maintained in the writer
            ''' </summary>
            ''' <param name="nRow">The row number to retrieve</param>
            ''' <returns>The row</returns>
            ''' <remarks>This method might not be necessary?</remarks>
            ''' ---------------------------------------------------------------
            Public Function GetRow(ByVal nRow As Integer) As DataRow
                Return Me.m_dt.Rows(nRow)
            End Function

            ''' ---------------------------------------------------------------
            ''' <summary>
            ''' Overridden to close the cEwEDbWriter if not already closed.
            ''' </summary>
            ''' ---------------------------------------------------------------
            Protected Overrides Sub Finalize()
                If Me.IsConnected Then Me.Disconnect(True)
                MyBase.Finalize()
            End Sub

            ''' ---------------------------------------------------------------
            ''' <summary>
            ''' Get a reference to the DataTable for the current writer
            ''' </summary>
            ''' <returns></returns>
            ''' ---------------------------------------------------------------
            Public Function GetDataTable() As DataTable
                Return Me.m_dt
            End Function

            ''' ---------------------------------------------------------------
            ''' <summary>
            ''' Helper method; replaces DBNull values that are specified as not 
            ''' Nullable in the underlying Access database schema with the default 
            ''' value in the schema.
            ''' </summary>
            ''' <param name="drow">The row to fix.</param>
            ''' ---------------------------------------------------------------
            Private Sub FixUnwantedDBNulls(ByRef drow As DataRow)

                Dim bIsValueNull As Boolean = False
                Dim bIsNullable As Boolean = False
                Dim bHasDefault As Boolean = False
                Dim ciENU As New Globalization.CultureInfo(1033) ' "en-US"
                Dim columnDataType As Data.OleDb.OleDbType = OleDbType.IUnknown
                Dim strColumnName As String = ""
                Dim strColumnDefault As String = ""

                For Each drowSchema As DataRow In Me.m_dtSchema.Rows
                    strColumnName = CStr(drowSchema("COLUMN_NAME"))

                    bIsValueNull = drow.IsNull(strColumnName)
                    bIsNullable = CBool(drowSchema("IS_NULLABLE"))
                    bHasDefault = CBool(drowSchema("COLUMN_HASDEFAULT"))

                    If bIsValueNull And Not bIsNullable Then
                        If bHasDefault Then

                            ' Set default using common datatype conversions to bypass language-specific
                            ' problems caused by misinterpreted decimal separators, etc

                            ' Get column data type
                            columnDataType = CType(drowSchema("DATA_TYPE"), Data.OleDb.OleDbType)
                            ' Get default value for this column (it's a string, regardless of column datatype. Brilliant)
                            strColumnDefault = CStr(drowSchema("COLUMN_DEFAULT"))

                            ' Convert defaults for common data types. Add others when needed.
                            Select Case columnDataType

                                Case OleDbType.WChar
                                    ' Access weirdness: fix double quotes problems
                                    drow(strColumnName) = strColumnDefault.Replace("""", "")

                                Case OleDbType.Boolean
                                    drow(strColumnName) = Boolean.Parse(strColumnDefault)

                                Case OleDbType.SmallInt
                                    drow(strColumnName) = CType(strColumnDefault, Int16)

                                Case OleDbType.Integer
                                    drow(strColumnName) = CInt(strColumnDefault)

                                Case OleDbType.Single
                                    Try
                                        drow(strColumnName) = Single.Parse(strColumnDefault, ciENU)
                                    Catch ex As Exception
                                        Debug.Assert(False)
                                        drow(strColumnName) = 0.0!
                                    End Try

                                Case OleDbType.Double
                                    Try
                                        drow(strColumnName) = Double.Parse(strColumnDefault, ciENU)
                                    Catch ex As Exception
                                        Debug.Assert(False)
                                        drow(strColumnName) = 0.0
                                    End Try

                                Case OleDbType.Currency
                                    ' ToDo_JS: Consider what to do here; test possible issues across locales
                                    Debug.Assert(False, "Currency defaults not properly supported in the EwE database logic")
                                    drow(strColumnName) = strColumnDefault

                                Case Else
                                    ' Unexpected datatype encountered
#If VERBOSE_LEVEL >= 2 Then
                                    Console.WriteLine("   - Default {0} for column {1}: unexpected datatype {2}", drow(strColumnName), strColumnName, columnDataType.ToString())
#End If
                                    ' Set the default and hope for the best
                                    drow(strColumnName) = strColumnDefault

                            End Select
                        End If
                    End If
                Next
            End Sub

        End Class

#End Region ' Class cEwEDbWriter

#Region " Open and close "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Create a new database.
        ''' </summary>
        ''' <param name="strDatabase">The database to create.</param>
        ''' <param name="strModelName">Name of the new model.</param>
        ''' <param name="bOverwrite">States whether an existing database may be overwritten.</param>
        ''' <returns>True of created succesfully.</returns>
        ''' <remarks>Note that this will NOT open the newly created database.</remarks>
        ''' -------------------------------------------------------------------
        Public MustOverride Function Create(ByVal strDatabase As String, _
                ByVal strModelName As String, _
                Optional ByVal bOverwrite As Boolean = False, _
                Optional ByVal databaseType As eDataSourceTypes = eDataSourceTypes.NotSet) As eDatasourceAccessType

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Open a connection to a database.
        ''' </summary>
        ''' <param name="strDatabase">The database to open.</param>
        ''' <param name="databaseType">Type to use to open the database. Set this
        ''' to 'NotSet' to auto-detect the database type.</param>
        ''' <returns>True if connected succesfully.</returns>
        ''' -------------------------------------------------------------------
        Public MustOverride Function Open(ByVal strDatabase As String, _
                                          Optional ByVal databaseType As eDataSourceTypes = eDataSourceTypes.NotSet) As eDatasourceAccessType

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Close an open connection.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public MustOverride Sub Close()

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the name of the connected database.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public MustOverride ReadOnly Property Name() As String

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Save a given database to a new destination, and open this new database.
        ''' </summary>
        ''' <param name="strDatabaseTo">Target database name.</param>
        ''' <param name="strModelName">New name to assign to the model.</param>
        ''' <param name="bOverwrite">States whether any model in the way will be obliterated.</param>
        ''' <returns>True if succesful.</returns>
        ''' -------------------------------------------------------------------
        Public MustOverride Function SaveAs(ByVal strDatabaseTo As String, _
                ByVal strModelName As String, _
                Optional ByVal bOverwrite As Boolean = False, _
                Optional ByVal databaseType As eDataSourceTypes = eDataSourceTypes.NotSet) As eDatasourceAccessType

#End Region ' Open and close

#Region " Maintenance "

        Public Overridable Function Compact(ByVal strConnectionFrom As String, ByVal strConnectionTo As String) As Boolean
            Return False
        End Function

        Public Overridable Function CanCompact(ByVal strConnectionFrom As String, ByVal strConnectionTo As String) As Boolean
            Return False
        End Function

#End Region ' Maintenance

#Region " Connection "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns the current database connection.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public MustOverride Function GetConnection() As IDbConnection

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' States whether there is a database connection that is open.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Function IsConnected() As Boolean
            Dim conn As IDbConnection = Me.GetConnection()

            If (conn Is Nothing) Then Return False
            Return (conn.State = ConnectionState.Open)
        End Function

#End Region ' Connection

#Region " Transaction "

        ''' <summary>The current transaction, if any.</summary>
        Private m_transaction As IDbTransaction = Nothing

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Begins a transaction for the current <see cref="GetConnection">Connection</see>.
        ''' </summary>
        ''' <returns>True if succesful.</returns>
        ''' <remarks>19may07: status experimental</remarks>
        ''' -------------------------------------------------------------------
        Public Function BeginTransaction() As Boolean
            If Not (Me.m_transaction Is Nothing) Then Return False
            Me.m_transaction = Me.GetConnection.BeginTransaction()
            Return True
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Commits a transaction previously initiated via <see cref="BeginTransaction">BeginTransaction</see>.
        ''' </summary>
        ''' <param name="bRollbackOnError">Flag stating whether the transaction needs
        ''' to automatically rollback when the commit process fails.</param>
        ''' <returns>True if the commit operation succeeded.</returns>
        ''' -------------------------------------------------------------------
        Public Function CommitTransaction(Optional ByVal bRollbackOnError As Boolean = True) As Boolean
            If (Me.m_transaction Is Nothing) Then Return False
            Try
                Me.m_transaction.Commit()
                Me.m_transaction = Nothing
                Return True
            Catch ex As Exception
                Console.WriteLine("cEwEDatabase: Transaction commit failed: {0}", ex.Message)
                If (bRollbackOnError) Then Me.RollbackTransaction()
            End Try
            Return False
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Commits a transaction to the current <see cref="GetConnection">Connection</see>.
        ''' </summary>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Public Function RollbackTransaction() As Boolean
            Try
                Transaction.Rollback()
                Me.m_transaction = Nothing
                Return True
            Catch ex As Exception
                Console.WriteLine("cEwEDatabase: Transaction rollback failed: {0}", ex.Message)
                Return False
            End Try
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Helper method; internally exposes the current active transaction.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Protected Function Transaction() As IDbTransaction
            Return Me.m_transaction
        End Function

#End Region ' Transaction

#Region " DB helper methods "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns an <see cref="IDbCommand">IDbCommand</see> for the current DBMS
        ''' </summary>
        ''' <param name="strSQL">Query to create the IDbCommand with.</param>
        ''' <returns>Nothing if an error occurred.</returns>
        ''' -------------------------------------------------------------------
        Public Overridable Function CreateDBCommand(ByVal strSQL As String) As IDbCommand

            Dim conn As IDbConnection = Me.GetConnection()
            Dim cmd As IDbCommand = Nothing

            Try
                If TypeOf conn Is OleDbConnection Then
                    cmd = New OleDbCommand(strSQL, DirectCast(conn, OleDbConnection), DirectCast(Me.Transaction(), OleDbTransaction))
                Else
                    cmd = New SqlCommand(strSQL, DirectCast(conn, SqlConnection), DirectCast(Me.Transaction(), SqlTransaction))
                End If
                Return cmd
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
                Return Nothing
            End Try

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns a <see cref="IDataReader">IDataReader</see> with a collection
        ''' of readonly records from the currently open connection.
        ''' </summary>
        ''' <param name="strSQL">The query to obtain the records.</param>
        ''' <returns></returns>
        ''' <remarks>The obtained IDataReader should be released via <see cref="ReleaseReader">ReleaseReader</see>.</remarks>
        ''' -------------------------------------------------------------------
        Public Overridable Function GetReader(ByVal strSQL As String) As IDataReader

            Dim reader As IDataReader = Nothing
            Try
                Using command As IDbCommand = Me.CreateDBCommand(strSQL)
                    reader = command.ExecuteReader()
                End Using
            Catch ex As Exception
                Console.WriteLine("GetReader error: {0}", ex.Message)
                reader = Nothing
            End Try
            Return reader

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Releases the set of readonly records previously obtained by calling
        ''' <see cref="GetReader">GetReader</see>.
        ''' </summary>
        ''' <param name="reader">The <see cref="IDataReader">IDataReader</see> to release.</param>
        ''' <returns>True if succesful.</returns>
        ''' -------------------------------------------------------------------
        Public Overridable Function ReleaseReader(ByVal reader As IDataReader) As Boolean
            Try
                reader.Close()
            Catch ex As Exception
                Debug.Assert(False, Me.ToString & ".ReleaseReader() Error: " & ex.Message)
                Return False
            End Try
            Return True
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns a <see cref="cEwEDbWriter">cEwEDbWriter</see> for
        ''' the given table in the database.
        ''' </summary>
        ''' <param name="strTable">The table to connect the EwEDbWriter to.</param>
        ''' <param name="strSquenceFieldName">Field name that indicates a sequence 
        ''' number that needs to be maintained when adding new rows. This parameter
        ''' is optional since only a few tables in EwE have a sequence field.</param>
        ''' <param name="strSquenceFilter">Sequence field subfilter. Only those rows
        ''' that match this filter will have their sequence field maintained.</param>
        ''' <returns>A writer that is connected if the table was available in the database.</returns>
        ''' -------------------------------------------------------------------
        Public Overridable Function GetWriter(ByVal strTable As String, _
                Optional ByVal strSquenceFieldName As String = "", _
                Optional ByVal strSquenceFilter As String = "") As cEwEDbWriter
            Return New cEwEDbWriter(Me, strTable, strSquenceFieldName, strSquenceFilter)
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Releases a writer previously created via <see cref="GetWriter">GetWriter</see>.
        ''' </summary>
        ''' <param name="writer">The writer to release</param>
        ''' <param name="bSaveChanges">States whether changes should be written (true) or discarded (false).</param>
        ''' <returns>True if succesful.</returns>
        ''' -------------------------------------------------------------------
        Public Overridable Function ReleaseWriter(ByRef writer As cEwEDbWriter, Optional ByVal bSaveChanges As Boolean = True) As Boolean
            writer.Disconnect(bSaveChanges)
            Return True
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns a scalar value from the current open connection.
        ''' </summary>
        ''' <param name="strSQL">The query to execute.</param>
        ''' <returns>The scalar value returned from the query.</returns>
        ''' -------------------------------------------------------------------
        Public Overridable Function GetValue(ByVal strSQL As String) As Object

            Dim value As Object = Nothing
            Try
                Using command As IDbCommand = Me.CreateDBCommand(strSQL)
                    value = command.ExecuteScalar()
                End Using
            Catch ex As Exception
                Console.WriteLine("** DB error '{0}' on query '{1}'", ex.Message, strSQL)
                value = Nothing
            End Try
            Return value
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Obtains an <see cref="IDataAdapter">IDataAdapter</see> for the
        ''' current open connection.
        ''' </summary>
        ''' <param name="strSQL">The SQL query to obtain the adaper for.</param>
        ''' <returns>An <see cref="IDataAdapter">IDataAdapter</see> if
        ''' successful, or Nothing when an error occurred.</returns>
        ''' <remarks>
        ''' <para>The obtained IDataAdapter should be released via 
        ''' <see cref="ReleaseAdapter">ReleaseAdapter</see>.</para></remarks>
        ''' -------------------------------------------------------------------
        Public Overridable Function GetAdapter(ByVal strSQL As String) As IDataAdapter

            Dim cmd As IDbCommand = Me.CreateDBCommand(strSQL)
            Try
                If TypeOf cmd Is OleDbCommand Then
                    Return New OleDbDataAdapter(DirectCast(cmd, OleDbCommand))
                Else
                    Return New SqlDataAdapter(DirectCast(cmd, SqlCommand))
                End If
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
            End Try

            Return Nothing

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Releases an <see cref="IDataAdapter">IDataAdapter</see> 
        ''' previously obtained from <see cref="GetAdapter">GetAdapter</see>.
        ''' </summary>
        ''' <param name="adapter">The <see cref="IDataAdapter">IDataAdapter</see> 
        ''' to release.</param>
        ''' <returns>True if succesful.</returns>
        ''' -------------------------------------------------------------------
        Public Overridable Function ReleaseAdapter(ByRef adapter As IDataAdapter) As Boolean
            ' Nothing to do
            Return True
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Obtains a <see cref="DataSet">DataSet</see> for modifying records.
        ''' </summary>
        ''' <param name="adapter">The <see cref="IDataAdapter">IDataAdapter</see> to fill the <see cref="DataSet">DataSet</see> from.</param>
        ''' <param name="strTable">The name of the table to fill the <see cref="DataSet">DataSet</see> from.</param>
        ''' <returns>The <see cref="DataSet">DataSet</see> if succesful, or Nothing if an error occurred.</returns>
        ''' <remarks>The obtained <see cref="DataSet">DataSet</see> should be released via <see cref="ReleaseDataSet">ReleaseWriter</see>.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Overridable Function GetDataSet(ByVal adapter As IDataAdapter, ByVal strTable As String) As DataSet
            Dim ds As New DataSet()
            Try
                adapter.Fill(ds)
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
                ds = Nothing
            End Try
            Return ds
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Commits all the pending changes in the <see cref="DataSet">DataSet</see>. This will
        ''' leave the DataSet open for further operations.
        ''' </summary>
        ''' <param name="dset">The <see cref="DataSet">DataSet</see> to commit</param>
        ''' <param name="adapter">The <see cref="IDataAdapter">OleDbDataAdapter</see> to write to the database</param>
        ''' <param name="strTable">The table to update</param>
        ''' <returns>True if succesful</returns>
        ''' -------------------------------------------------------------------
        Public Overridable Function CommitDataSet(ByVal dset As DataSet, ByVal adapter As IDataAdapter, ByVal strTable As String) As Boolean
            Dim bSucces As Boolean = True

            ' Is adapter specified?
            If (adapter Is Nothing) Then
                ' #No adapter = no need to update database. Done
                Return True
            End If

            ' Table name optional, no need to Assert
            Try
                adapter.Update(dset)
            Catch ex As Exception
                ' Woops
                Console.WriteLine("Error {0} updating {1}", ex.Message, strTable)
                bSucces = False
            End Try
            ' Report result
            Return bSucces

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Releases a <see cref="DataSet">DataSet</see> previously obtained via 
        ''' <see cref="GetDataSet">GetDataSet</see>.
        ''' </summary>
        ''' <param name="dset">The writer to release.</param>
        ''' <param name="adapter">The <see cref="IDataAdapter">IDataAdapter</see>
        ''' to commit any changes to. If this parameter is left blank, any changes made to
        ''' the dataset and its data are discarded.</param>
        ''' <param name="strTable">The name of the table to update.</param>
        ''' <returns>True if succesful.</returns>
        ''' -------------------------------------------------------------------
        Public Overridable Function ReleaseDataSet(ByVal dset As DataSet, Optional ByVal adapter As IDataAdapter = Nothing, Optional ByVal strTable As String = "") As Boolean
            Return Me.CommitDataSet(dset, adapter, strTable)
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Executes a SQL command that does not return any information.
        ''' </summary>
        ''' <param name="strSQL">The query to execute.</param>
        ''' <returns>True if succesful.</returns>
        ''' -------------------------------------------------------------------
        Public Overridable Function Execute(ByVal strSQL As String) As Boolean

            Dim bSucces As Boolean = True
            Try
                Using command As IDbCommand = Me.CreateDBCommand(strSQL)
                    command.ExecuteNonQuery()
                End Using
            Catch ex As Exception
                Console.WriteLine("* DB exception '{0}' on '{1}'", ex.Message, strSQL)
                bSucces = False
            End Try
            Return bSucces

        End Function

#End Region ' DB helper methods

#Region " OOP "

#Region " OOP public interfaces "

#Region " OOP Public classes "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Base class for implementing objects that can be stored in this type
        ''' of database.
        ''' </summary>
        ''' -------------------------------------------------------------------
        <Serializable()> _
        Public MustInherit Class cOOPStorable

#Region " Privates "

            ''' <summary>Unique ID of an instance of cOOPStorable</summary>
            Private m_iDBID As Integer = cDBID_INVALID ' Key not assigned yet

            ''' <summary>Flag stating whether an cOOPStorabe instance is allowed to 
            ''' broadcast <see cref="OnChanged">OnChanged</see>events.</summary>
            Private m_bAllowEvents As Boolean = True

            ''' <summary>Flag preventing looped updates.</summary>
            Private m_bInUpdate As Boolean = False

#If DEBUG Then

            ''' <summary>Flag stating whether the object is deleted from the database.</summary>
            ''' <remarks>This flag is deliberately only available at debug time.</remarks>
            Private m_bDeleted As Boolean = False

#End If

#End Region ' Privates

#Region " Constructor "

            ''' <summary>Default ID for newly created cOOPStorable instances.</summary>
            Public Const cDBID_INVALID As Integer = 0

            Public Sub New()
                Me.m_iDBID = cDBID_INVALID
            End Sub

#End Region ' Constructor

#Region " Public properties "

            ''' ---------------------------------------------------------------
            ''' <summary>
            ''' Event to notify that instance unit has changed
            ''' </summary>
            ''' <param name="obj">The <see cref="cOOPStorable">instance</see>
            ''' that changed</param>
            ''' ---------------------------------------------------------------
            Public Event OnChanged(ByVal obj As cOOPStorable)

            ''' ---------------------------------------------------------------
            ''' <summary>
            ''' The unique ID of any object in the database. The database
            ''' manages this property exclusively although public read 
            ''' access is allowed.
            ''' </summary>
            ''' ---------------------------------------------------------------
            <Browsable(False)> _
            Public Property DBID() As Integer
                Get
                    Return Me.m_iDBID
                End Get
                Friend Set(ByVal value As Integer)
                    Me.m_iDBID = value
                End Set
            End Property

            ''' ---------------------------------------------------------------
            ''' <summary>
            ''' Get/set whether this instance is allowed to send
            ''' <see cref="OnChanged">change events</see>.
            ''' </summary>
            ''' ---------------------------------------------------------------
            <Browsable(False)> _
            Public Property AllowEvents() As Boolean
                Get
                    Return Me.m_bAllowEvents
                End Get
                Set(ByVal value As Boolean)
                    Me.m_bAllowEvents = value
                    If m_bAllowEvents Then Me.SetChanged()
                End Set
            End Property

#End Region ' Public properties

#Region " Public interfaces "

            ''' ---------------------------------------------------------------
            ''' <summary>
            ''' Copy the content of another instance of cOOPStorable.
            ''' </summary>
            ''' <param name="objSrc">The source ofject to copy content from.</param>
            ''' ---------------------------------------------------------------
            Public Overridable Sub CopyFrom(ByVal objSrc As cOOPStorable)
                Dim apiSrc As PropertyInfo() = Nothing
                Dim apiTgt As PropertyInfo() = Nothing
                Dim piSrc As PropertyInfo = Nothing
                Dim piTgt As PropertyInfo = Nothing

                If (objSrc Is Nothing) Then Return

                ' Copy all copyable properties
                apiSrc = objSrc.GetType().GetProperties()
                apiTgt = Me.GetType().GetProperties()
                For Each piSrc In apiSrc
                    If String.Compare(piSrc.Name, "DBID") <> 0 Then
                        For Each piTgt In apiTgt
                            If piSrc.Name = piTgt.Name Then
                                Try
                                    If piTgt.CanWrite() Then
                                        piTgt.SetValue(Me, piSrc.GetValue(objSrc, Nothing), Nothing)
                                    End If
                                Catch ex As Exception
#If VERBOSE_LEVEL >= 2 Then
                                ' Ok, this did not work
                                Console.WriteLine("Woops: failed to copy prop {0} : {1}", piTgt.Name, ex.Message)
#End If
                                End Try
                            End If
                        Next
                    End If
                Next
            End Sub

#End Region ' Public interfaces

#Region " Internals "

            ''' ---------------------------------------------------------------
            ''' <summary>
            ''' Helper method, states whether the content of an instance has
            ''' been modified externally.
            ''' </summary>
            ''' ---------------------------------------------------------------
            <Browsable(False)> _
            Protected Sub SetChanged()
                If Me.m_bAllowEvents Then
                    If (Me.m_bInUpdate = False) Then
                        ' Set deadlonk prevention lock
                        Me.m_bInUpdate = True
                        ' Raise event
                        RaiseEvent OnChanged(Me)
                        ' Release deadlonk prevention lock
                        Me.m_bInUpdate = False
                    End If
                End If
            End Sub

#If DEBUG Then

            ''' ---------------------------------------------------------------
            ''' <summary>
            ''' Helper method, states whether the object has been deleted from 
            ''' the database and thus cannot be saved.
            ''' </summary>
            ''' <remarks>
            ''' This method is only available at debug time.
            ''' </remarks>
            ''' ---------------------------------------------------------------
            <Browsable(False)> _
            Friend Property Deleted() As Boolean
                Get
                    Return Me.m_bDeleted
                End Get
                Set(ByVal value As Boolean)
                    Me.m_bDeleted = value
                End Set
            End Property

#End If

#End Region ' Internals

        End Class

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Strong-typed list for cOOPStorable instances.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Class cOOPStorableList
            Inherits cOOPStorable
            Implements IList(Of cOOPStorable)

            Private m_list As New List(Of cOOPStorable)

            Public Sub Add(ByVal item As cOOPStorable) _
                Implements ICollection(Of cOOPStorable).Add
                Debug.Assert(Not Me.Contains(item), "Item already present in list")
                Me.m_list.Add(item)
            End Sub

            Public Sub Clear() _
                Implements ICollection(Of cOOPStorable).Clear
                Me.m_list.Clear()
            End Sub

            Public Function Contains(ByVal item As cOOPStorable) As Boolean _
                Implements ICollection(Of cOOPStorable).Contains
                Return Me.m_list.Contains(item)
            End Function

            Public Sub CopyTo(ByVal array() As cOOPStorable, ByVal arrayIndex As Integer) _
                Implements ICollection(Of cOOPStorable).CopyTo
                Me.m_list.CopyTo(array)
            End Sub

            <Browsable(False)> _
            Public ReadOnly Property Count() As Integer _
                Implements ICollection(Of cOOPStorable).Count
                Get
                    Return Me.m_list.Count
                End Get
            End Property

            <Browsable(False)> _
            Public ReadOnly Property IsReadOnly() As Boolean _
                Implements ICollection(Of cOOPStorable).IsReadOnly
                Get
                    Return False
                End Get
            End Property

            Public Function Remove(ByVal item As cOOPStorable) As Boolean _
                Implements ICollection(Of cOOPStorable).Remove
                ' ToDo: remember this to actively erase item from DB?
                Me.m_list.Remove(item)
            End Function

            Public Function GetEnumerator() As IEnumerator(Of cOOPStorable) _
                Implements IEnumerable(Of cOOPStorable).GetEnumerator
                Return Me.m_list.GetEnumerator()
            End Function

            Public Function IndexOf(ByVal item As cOOPStorable) As Integer _
                Implements IList(Of cOOPStorable).IndexOf
                Return Me.m_list.IndexOf(item)
            End Function

            Public Sub Insert(ByVal index As Integer, ByVal item As cOOPStorable) _
                Implements IList(Of cOOPStorable).Insert
                Debug.Assert(Not Me.Contains(item), "Item already present in list")
                Me.m_list.Insert(index, item)
            End Sub

            <Browsable(False)> _
            Default Public Property Item(ByVal index As Integer) As cOOPStorable _
                Implements IList(Of cOOPStorable).Item
                Get
                    Return Me.m_list.Item(index)
                End Get
                Set(ByVal value As cOOPStorable)
                    Me.m_list.Item(index) = value
                End Set
            End Property

            Public Sub RemoveAt(ByVal index As Integer) _
                Implements IList(Of cOOPStorable).RemoveAt
                Me.m_list.RemoveAt(index)
            End Sub

            Private Function GetEnumaarghAarghAargh() As System.Collections.IEnumerator _
                Implements System.Collections.IEnumerable.GetEnumerator
                Return Nothing
            End Function

        End Class

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Class for querying if cOOPStorable instances are stored in the 
        ''' database.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Class cOOPKey

#Region " Privates "

            ''' <summary>The ID for an object in the database</summary>
            Private m_iDBID As Integer = cOOPStorable.cDBID_INVALID
            ''' <summary>The runtime type that was used to write the
            ''' record in the database with this ID.</summary>
            Private m_tOriginating As Type = Nothing

#End Region ' Privates

#Region " Constructor "

            ''' ---------------------------------------------------------------
            ''' <summary>
            ''' Construct a new cOOPKey instance.
            ''' </summary>
            ''' <param name="t">The runtime type to generate the key for.</param>
            ''' <param name="iDBID">The unique ID that this key is generated for.</param>
            ''' ---------------------------------------------------------------
            Friend Sub New(ByVal t As Type, ByVal iDBID As Integer)

                ' Sanity checks
                Debug.Assert(t IsNot Nothing)
                Debug.Assert(iDBID <> cOOPStorable.cDBID_INVALID)

                Me.m_tOriginating = t
                Me.m_iDBID = iDBID

            End Sub

#End Region ' Constructor

#Region " Properties "

            ''' ---------------------------------------------------------------
            ''' <summary>
            ''' Get the unique ID for the key.
            ''' </summary>
            ''' ---------------------------------------------------------------
            Public ReadOnly Property DBID() As Integer
                Get
                    Return Me.m_iDBID
                End Get
            End Property

            ''' ---------------------------------------------------------------
            ''' <summary>
            ''' Get the runtime type for the key.
            ''' </summary>
            ''' ---------------------------------------------------------------
            Public ReadOnly Property OriginatingType() As Type
                Get
                    Return Me.m_tOriginating
                End Get
            End Property

#End Region ' Properties

        End Class

#End Region ' OOP Public classes

#Region " OOP Read "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Read an cOOPKey for a ID in the database. If all goes well a 
        ''' <see cref="cOOPKey">object key</see> is returned, which links the
        ''' ID value to the originating runtime type.
        ''' </summary>
        ''' <param name="iDBID">
        ''' The ID to retrieve an <see cref="cOOPKey">object key</see> for.
        ''' </param>
        ''' <returns>
        ''' A <see cref="cOOPKey">object key</see> instance, or null if the ID 
        ''' was not present in the database, or if the originating runtime
        ''' type was not present in the current loaded set of assemblies.
        ''' </returns>
        ''' -------------------------------------------------------------------
        Protected Function ReadObjectKey(ByVal iDBID As Integer) As cOOPKey

            ' Sanity check
            If Not Me.m_bOOPEnabled Then Return Nothing

            Dim reader As IDataReader = Nothing
            Dim strTypeName As String = ""
            Dim tOrg As Type = Nothing
            Dim strSQL As String = ""
            Dim objKey As cOOPKey = Nothing

            ' Construct SQL statement to query the runtime information from the 
            ' OOP core table 'cOOPStorable'.
            strSQL = String.Format("SELECT {0}, DBID FROM {1} WHERE DBID={2}", OOP_CLASSNAMECOL, Me.OOPGetTableName(GetType(cOOPStorable)), iDBID)
            reader = Me.GetReader(strSQL)
            Try
                ' Don't try this at home
                reader.Read()
                ' Get the original type name from the database
                strTypeName = CStr(reader(OOP_CLASSNAMECOL))
                ' Try to find originating type in the set of loaded assemblies
                tOrg = Me.OOPStringToType(strTypeName)
                ' Succes?
                If (tOrg IsNot Nothing) Then objKey = New cOOPKey(tOrg, iDBID)
                ' Done
                Me.ReleaseReader(reader)
            Catch ex As Exception

            End Try
            ' Return constructed key, if any
            Return objKey
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Read keys for all objects of a given type from the database.
        ''' </summary>
        ''' <param name="t">Type to filter by.</param>
        ''' <param name="bIncludeInherited">Flag indicating whether objects 
        ''' inherited from <paramref name="t">t</paramref> are allowed to be 
        ''' returned as well.</param>
        ''' <returns>An array of <see cref="cOOPKey">object keys</see>.</returns>
        ''' -------------------------------------------------------------------
        Protected Function ReadObjectKeys(ByVal t As Type, Optional ByVal bIncludeInherited As Boolean = True) As cOOPKey()

            ' Sanity check
            If Not Me.m_bOOPEnabled Then Return Nothing

            Dim strTypeName As String = ""
            Dim strSQL As String = ""
            Dim reader As IDataReader = Nothing
            Dim objKey As cOOPKey = Nothing
            Dim lKeys As New List(Of cOOPKey)
            Dim tOrg As Type = Nothing
            Dim bInclude As Boolean = True

            strSQL = String.Format("SELECT {0}, DBID FROM {1} ORDER BY DBID ASC", OOP_CLASSNAMECOL, Me.OOPGetTableName(GetType(cOOPStorable)))
            reader = Me.GetReader(strSQL)

            If reader IsNot Nothing Then
                Try
                    While reader.Read

                        ' Get type name
                        strTypeName = CStr(reader(OOP_CLASSNAMECOL))
                        ' Try to find orginating type in currently loaded assemblies
                        tOrg = OOPStringToType(strTypeName)

                        ' Was originating type succesfully located?
                        If (tOrg IsNot Nothing) Then
                            ' #Yes: allowed to include inherited classes?
                            If bIncludeInherited Then
                                ' #Yes: include when tOrg is inherited - or equals - from t
                                bInclude = t.IsAssignableFrom(tOrg)
                            Else
                                ' #No: include only when t equals tOrg
                                bInclude = (t Is tOrg)
                            End If
                        Else
                            ' #No: do not include
                            bInclude = False
                        End If

                        ' Include?
                        If bInclude Then
                            ' #Yes: generate key
                            objKey = New cOOPKey(tOrg, CInt(reader("DBID")))
                            ' Add new key to the list to return
                            lKeys.Add(objKey)
                        End If

                    End While
                    Me.ReleaseReader(reader)

                Catch ex As Exception

                End Try
            End If

            ' Present results
            Return lKeys.ToArray()

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Reads an object from the database.
        ''' </summary>
        ''' <param name="key"><see cref="cOOPKey">Object key</see> to read the 
        ''' object for.</param>
        ''' <returns>A <see cref="cOOPStorable">cOOPStorable</see>-derived 
        ''' instance, or null if an error occurred.</returns>
        ''' -------------------------------------------------------------------
        Public Function ReadObject(ByVal key As cOOPKey) As cOOPStorable
            Return Me.ReadObject(key.OriginatingType, key.DBID)
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Reads an object from the database.
        ''' </summary>
        ''' <param name="t">The type of the object to read.</param>
        ''' <param name="iDBID">The ID of the object to read.</param>
        ''' <returns>A <see cref="cOOPStorable">cOOPStorable</see>-derived 
        ''' instance, or null if an error occurred.</returns>
        ''' -------------------------------------------------------------------
        Public Function ReadObject(ByVal t As Type, ByVal iDBID As Integer) As cOOPStorable
            If Not Me.m_bOOPEnabled Then Return Nothing
            Dim piKey As PropertyInfo = Me.OOPGetKeyProperty(t)
            Return OOPReadObject(t, iDBID, piKey)
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Reads all objects of a given type from the database.
        ''' </summary>
        ''' <param name="t">The type to read objects for.</param>
        ''' <param name="bIncludeInherited">Flag indicating whether objects 
        ''' inherited from <paramref name="t">t</paramref> are allowed to be 
        ''' read as well.</param>
        ''' <returns>
        ''' An array of <see cref="cOOPStorable">cOOPStorable</see>-derived 
        ''' instances.
        ''' </returns>
        ''' -------------------------------------------------------------------
        Public Function ReadObjects(ByVal t As Type, _
                                    Optional ByVal bIncludeInherited As Boolean = True) As cOOPStorable()

            If Not Me.m_bOOPEnabled Then Return Nothing

            Dim aKeys As cOOPKey() = Me.ReadObjectKeys(t, bIncludeInherited)
            Dim lObjs As New List(Of cOOPStorable)
            Dim obj As cOOPStorable = Nothing
            Dim piKey As PropertyInfo = Me.OOPGetKeyProperty(t)

            For iKey As Integer = 0 To aKeys.Length - 1
                obj = Me.OOPReadObject(aKeys(iKey).OriginatingType, aKeys(iKey).DBID, piKey)
                If obj IsNot Nothing Then lObjs.Add(obj)
            Next
            Return lObjs.ToArray()

        End Function

#End Region ' OOP Read

#Region " OOP Write "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Write an <see cref="cOOPStorable">OOPstorable-derived</see> instance 
        ''' to the database.
        ''' </summary>
        ''' <param name="obj">The object to save.</param>
        ''' <remarks>
        ''' This method will verifuy - and update if  necessary - the database
        ''' schema for this class of object and its base classes. This check 
        ''' is performed only the very first time a particular object is written 
        ''' to the database; consecutive save attempts will bypass the check for
        ''' performance reasons.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Function WriteObject(ByVal obj As cOOPStorable) As Boolean

            ' Friendly sanity check
            If Not Me.m_bOOPEnabled Then Return False

#If DEBUG Then
            ' Sanity check
            Debug.Assert(Not obj.Deleted, "Object is deleted and cannot not be saved.")
#End If

            Dim t As Type = obj.GetType()
            Dim api As PropertyInfo() = Me.OOPGetStorableProperties(t)
            Dim piKey As PropertyInfo = Nothing
            Dim bSucces As Boolean = True

            ' Make sure database schema is up to date to accomodate this object
            bSucces = Me.OOPUpdateObjectSchema(t)

            ' Everything well?
            If bSucces Then

                ' #Yes: get the property that holds the primary key (DBID)
                piKey = Me.OOPGetKeyProperty(t)

                ' Does the database already have an ID for this object? 
                ' cOOPStorable instances that are not saved yet will have an
                ' invalid database ID, which will need to be properly set
                ' before the object can be saved to the database.
                If (obj.DBID = cOOPStorable.cDBID_INVALID) Then
                    ' #Yes: invalid ID must be set
                    obj.DBID = Me.m_iNextDBID
                    ' Increment next ID for next object.
                    Me.m_iNextDBID += 1
                End If

                ' Add to saved object cache to prevent looped saving. Assume all is well
                Me.m_OOPObjectCache.AddObject(obj)
                ' Write the object
                bSucces = Me.OOPWriteObjectRecursive(t, obj, piKey)

                ' Failure?!
                If Not bSucces Then
                    ' #Yes: remove object from the saved object cache
                    Me.m_OOPObjectCache.RemoveObject(obj)
                End If
            End If

            ' Report outcome
            Return bSucces

        End Function

#End Region ' OOP Write

#Region " OOP Delete "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Delete an <see cref="cOOPStorable">cOOPStorable-derived</see> 
        ''' instance from the database.
        ''' </summary>
        ''' <param name="obj">The object to delete.</param>
        ''' <returns>True if succesful.</returns>
        ''' -------------------------------------------------------------------
        Public Function DeleteObject(ByVal obj As cOOPStorable) As Boolean

            Dim bSucces As Boolean = True

            ' Friendly sanity check
            If Not Me.m_bOOPEnabled Then Return False
            If (obj Is Nothing) Then Return False
            If (obj.DBID = 0) Then Return True

            ' Delete object recursively
            bSucces = Me.DeleteObjectRecursive(obj, obj.GetType())

            ' All well?
            If bSucces Then
                ' #Yes: remove the object from the saved object cache
                Me.m_OOPObjectCache.RemoveObject(obj)
            End If

            ' Report outcome
            Return bSucces

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Delete an cOOPStorable-derived instance from the database, starting
        ''' at the given class level, and recursively deleting the direct
        ''' base class instance if applicable.
        ''' </summary>
        ''' <param name="obj">The object to delete.</param>
        ''' <param name="t">The class level to delete from the database.</param>
        ''' <returns>True if succesful.</returns>
        ''' -------------------------------------------------------------------
        Private Function DeleteObjectRecursive(ByVal obj As cOOPStorable, ByVal t As Type) As Boolean

            Dim strSQL As String = String.Format("DELETE FROM {0} WHERE DBID={1}", Me.OOPGetTableName(t), obj.DBID)
            Dim bSucces As Boolean = True

            ' Able to delete object at class level 't' from the database
            If Me.Execute(strSQL) Then
                Try
                    ' #Yes: remove from local cache
                    Me.m_OOPObjectCache.RemoveObject(obj)
                Catch ex As Exception
                    ' #woops: panic
                    bSucces = False
                End Try
            Else
                ' #woops: panic
                bSucces = False
            End If

            ' Not the base class?
            If Not Me.OOPIsBaseClass(t) Then
                ' #Good, delete direct base class next
                bSucces = bSucces And Me.DeleteObjectRecursive(obj, t.BaseType)
            End If

#If DEBUG Then
            ' Set debug-mode verification flag
            obj.Deleted = bSucces
#End If

            ' Report outcome
            Return bSucces

        End Function

#End Region ' OOP Delete

#End Region ' OOP public interfaces

#Region " OOP Admin "

#Region " OOP Admin vars "

        ''' <summary>Flag stating whether the database has been prepared 
        ''' for OOP object access. To prepare the database for OOP access
        ''' refer to <see cref="OOPEnabled">OOPEnabled</see>.</summary>
        Private m_bOOPEnabled As Boolean = False
        ''' <summary>The unique ID to assigne to new cOOPStorable instances
        ''' in the database.</summary>
        Private m_iNextDBID As Integer = -1
        ''' <summary>Cache of all objects already saved into the database.
        ''' Keeping a cache is much faster than having to query the database
        ''' for every potential save.</summary>
        Private m_OOPObjectCache As cOOPObjectCache = Nothing
        ''' <summary>Cache of all object for which the database schema has
        ''' already been verifyfied during since the last time the database
        ''' was opened.</summary>
        Private m_OOPObjectSchemaVerified As List(Of Type) = Nothing

#End Region ' OOP Admin vars

#Region " OOP Amin interfaces "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Turn on or off OOP capabilities
        ''' </summary>
        ''' -------------------------------------------------------------------
        Protected Property OOPEnabled() As Boolean
            Get
                Return Me.m_bOOPEnabled
            End Get
            Set(ByVal bEnable As Boolean)

                ' Optimization
                If (bEnable = Me.m_bOOPEnabled) Then Return

                ' Must turn OOP capabilities on?
                If bEnable Then
                    ' #Yes: determine next unique ID
                    Try
                        Me.m_iNextDBID = CInt(Me.GetValue(String.Format("SELECT MAX(DBID) FROM {0}", Me.OOPGetTableName(GetType(cOOPStorable))))) + 1
                    Catch ex As Exception
                        Me.m_iNextDBID = 1
                    End Try

                    ' Create schema verification cache
                    Me.m_OOPObjectSchemaVerified = New List(Of Type)
                    ' Create saved object cache
                    Me.m_OOPObjectCache = New cOOPObjectCache()
                Else
                    ' #No: reset OOP capabilities
                    Me.m_iNextDBID = 0
                    Me.m_OOPObjectSchemaVerified = Nothing
                    Me.m_OOPObjectCache = Nothing
                End If
                ' Yo!
                Me.m_bOOPEnabled = bEnable
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Internal admin helper; clear the saved object cache.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Protected Sub OOPFlushObjectCache()
            Me.m_OOPObjectCache.Clear()
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Internal admin helper; clear the verified schema cache.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Protected Sub OOPFlushSchemaCache()
            Me.m_OOPObjectSchemaVerified.Clear()
        End Sub

#End Region ' OOP Amin interfaces

#Region " OOP Admin internals "

#Region " OOP Object cache "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Helper class, maintains a dictionary of processed 
        ''' <see cref="cOOPStorable">objects</see> for reassembling item links.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Class cOOPObjectCache

            ''' <summary>The cache.</summary>
            Private m_dtObjectCache As New Dictionary(Of Integer, cOOPStorable)

            ''' ---------------------------------------------------------------
            ''' <summary>Add an object to the cache.</summary>
            ''' <param name="obj">The object to add.</param>
            ''' ---------------------------------------------------------------
            Public Sub AddObject(ByVal obj As cOOPStorable)
                If Not HasObject(obj.DBID) Then
                    Me.m_dtObjectCache(obj.DBID) = obj
                End If
            End Sub

            ''' ---------------------------------------------------------------
            ''' <summary>
            ''' Remove an object from the cache.
            ''' </summary>
            ''' <param name="obj">The object to remove.</param>
            ''' ---------------------------------------------------------------
            Public Sub RemoveObject(ByVal obj As cOOPStorable)
                If HasObject(obj.DBID) Then
                    Me.m_dtObjectCache.Remove(obj.DBID)
                End If
            End Sub

            ''' ---------------------------------------------------------------
            ''' <summary>
            ''' Retrieves an object from the cache.
            ''' </summary>
            ''' <param name="iDBID">The ID of the object to retrieve.</param>
            ''' <returns>An object, or nothing if the object is not present
            ''' in the cache.</returns>
            ''' ---------------------------------------------------------------
            Public Function GetObject(ByVal iDBID As Integer) As cOOPStorable
                If HasObject(iDBID) Then
                    Return Me.m_dtObjectCache(iDBID)
                Else
                    Return Nothing
                End If
            End Function

            ''' ---------------------------------------------------------------
            ''' <summary>
            ''' States whether an object with a given database ID is present
            ''' in the cache.
            ''' </summary>
            ''' <param name="iDBID">The ID of the object to find.</param>
            ''' <returns>True if the object is present in the cache.</returns>
            ''' ---------------------------------------------------------------
            Public Function HasObject(ByVal iDBID As Integer) As Boolean
                Return Me.m_dtObjectCache.ContainsKey(iDBID)
            End Function

            ''' ---------------------------------------------------------------
            ''' <summary>
            ''' Clears the object cache.
            ''' </summary>
            ''' ---------------------------------------------------------------
            Public Sub Clear()
                Me.m_dtObjectCache.Clear()
            End Sub

        End Class

#End Region ' OOP Object cache

#Region " OOP Schema management "

        ''' ---------------------------------------------------------------
        ''' <summary>
        ''' OOP foreign key
        ''' </summary>
        ''' ---------------------------------------------------------------
        Private Structure OOP_sFKInfo
            Public Sub New(ByVal strCol As String, ByVal strTable As String, ByVal bInherited As Boolean)
                Me.ColumnName = strCol
                Me.TableName = strTable
                Me.Inherited = bInherited
            End Sub
            Public ColumnName As String
            Public TableName As String
            Public Inherited As Boolean
        End Structure

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Create a table for a <see cref="cOOPStorable">cOOPStorable</see>-derived class 
        ''' </summary>
        ''' <param name="t">The <see cref="Type">type</see> to build the table for.</param>
        ''' <returns>True if succesful.</returns>
        ''' -------------------------------------------------------------------
        Private Function OOPCreateObjectTable(ByVal t As Type) As Boolean

            ' Get all storable properties for type t
            Dim api As PropertyInfo() = Me.OOPGetStorableProperties(t)
            Dim strColumnName As String = ""
            Dim strColumnType As String = ""
            Dim strQuery As String = ""
            Dim sbClause As New Text.StringBuilder
            Dim bSucces As Boolean = False
            Dim lFK As New List(Of OOP_sFKInfo)

            ' Iterate through all 
            For Each pi As PropertyInfo In api
                strColumnName = Me.OOPGetColumnName(pi)
                strColumnType = Me.OOPGetColumnType(pi)

                If Not String.IsNullOrEmpty(strColumnName) And Not String.IsNullOrEmpty(strColumnType) Then
                    If sbClause.Length > 0 Then sbClause.Append(", ")
                    sbClause.Append("[" & strColumnName & "] " & strColumnType)
                End If

                If Me.OOPIsForeignKeyProperty(pi) Then
                    Me.OOPUpdateObjectSchema(pi.PropertyType)
                    lFK.Add(New OOP_sFKInfo(strColumnName, Me.OOPGetTableName(pi.PropertyType), False))
                End If
            Next

            If (sbClause.Length = 0) Then Return True

            ' Add class name as first column for base classes only
            If Me.OOPIsBaseClass(t) Then
                sbClause.Insert(0, OOP_CLASSNAMECOL + " TEXT(64), ")
            Else
                lFK.Insert(0, New OOP_sFKInfo("DBID", Me.OOPGetTableName(t.BaseType), True))
            End If

            ' Create table
            strQuery = String.Format("CREATE TABLE {0} ({1})", Me.OOPGetTableName(t), sbClause.ToString)
            bSucces = Me.Execute(strQuery)

            ' Create primary key for this table
            strQuery = String.Format("ALTER TABLE {0} ADD PRIMARY KEY (DBID)", Me.OOPGetTableName(t))
            bSucces = bSucces And Me.Execute(strQuery)

            ' Create all FKs
            For Each fk As OOP_sFKInfo In lFK
                strQuery = String.Format("ALTER TABLE {2} ADD FOREIGN KEY ({1}) REFERENCES {0} (DBID) ON DELETE CASCADE", _
                    fk.TableName, _
                    fk.ColumnName, Me.OOPGetTableName(t))
                bSucces = bSucces And Me.Execute(strQuery)
            Next

            If Not bSucces Then
#If VERBOSE_LEVEL >= 1 Then
                Console.WriteLine("Failed to create table scheme {0}", Me.OOPGetTableName(t))
#End If
            End If
            Return bSucces

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Update column definitions for a single given cOOPStorable-derived 
        ''' type within an existing table.
        ''' </summary>
        ''' <param name="t">The type to update the database schema for.</param>
        ''' <param name="conn">The database connection to update the database
        ''' schema.</param>
        ''' <returns>True if succesful.</returns>
        ''' <remarks>
        ''' <para>This method does not recurse; a single type is processed for a 
        ''' single table. To process an enitre object inheritance tree, use
        ''' <see cref="OOPUpdateObjectSchema">OOPUpdateObjectSchema</see>.</para>
        ''' <para>Note that this method does not handle column datatype 
        ''' conversions; only mssing columns are added.</para>
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Private Function OOPUpdateObjectTable(ByVal t As Type, ByVal conn As OleDbConnection) As Boolean

            Dim dt As DataTable = Nothing
            Dim api As PropertyInfo() = Me.OOPGetStorableProperties(t)
            Dim lpiMissing As New List(Of PropertyInfo)
            Dim strTable As String = Me.OOPGetTableName(t)
            Dim strName As String = ""
            Dim strType As String = ""
            Dim strSQL As String = ""
            Dim sbClauses As New System.Text.StringBuilder
            Dim bSucces As Boolean = True
            Dim bFound As Boolean = False

            ' Obtain the list of columns for the desired table
            dt = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Columns, New String() {Nothing, Nothing, strTable, Nothing})

            ' Obtain a list of storable property that do not have a column in the associated schema
            For iProp As Integer = 0 To api.Length - 1
                strName = Me.OOPGetColumnName(api(iProp))
                bFound = False
                For Each drow As DataRow In dt.Rows
                    If String.Compare(CStr(drow("COLUMN_NAME")), strName, True) = 0 Then bFound = True
                Next drow
                If Not bFound Then lpiMissing.Add(api(iProp))
            Next

            ' No missing columns? #Goody: all done
            If lpiMissing.Count = 0 Then Return True

            ' Add missing columns to the database schema
            For Each pi As PropertyInfo In lpiMissing
                strName = Me.OOPGetColumnName(pi)
                strType = Me.OOPGetColumnType(pi)
                If Not String.IsNullOrEmpty(strName) And Not String.IsNullOrEmpty(strType) Then
                    If sbClauses.Length > 0 Then sbClauses.Append(", ")
                    sbClauses.Append(String.Format("{0} {1}", strName, strType))
                End If
            Next

            If (sbClauses.Length > 0) Then
                ' M$ Access does not like brackets in 'ALTER TABLE <name> ADD (<clause(s)>)'
                strSQL = String.Format("ALTER TABLE {0} ADD {1}", Me.OOPGetTableName(t), sbClauses.ToString)
                bSucces = Me.Execute(strSQL)
            End If

            If Not bSucces Then
#If VERBOSE_LEVEL >= 1 Then
                Console.WriteLine("Failed to update table scheme {0}", Me.OOPGetTableName(t))
#End If
            End If

            Return bSucces

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Update the database schema for storing a class of a given type. The
        ''' entire inheritance tree of the given type is processed, new tables
        ''' are created and table columns are added when necessary.
        ''' </summary>
        ''' <param name="t">The type to update the database schema for.</param>
        ''' <returns>
        ''' True if succesful.
        ''' </returns>
        ''' -------------------------------------------------------------------
        Private Function OOPUpdateObjectSchema(ByVal t As Type) As Boolean

            Dim conn As OleDbConnection = DirectCast(Me.GetConnection(), OleDbConnection)
            Dim strTable As String = ""
            Dim dt As DataTable = Nothing
            Dim bIsBaseClass As Boolean = False
            Dim bSucces As Boolean = True ' Ommmm

            ' Already verified?
            If (Me.m_OOPObjectSchemaVerified.IndexOf(t) <> -1) Then Return True
            ' Immediately flag as verified to prevent self-links to cause verification loops
            Me.m_OOPObjectSchemaVerified.Add(t)

            ' Not the base class?
            If Not Me.OOPIsBaseClass(t) Then
                ' #Good, write base class first
                bSucces = bSucces And Me.OOPUpdateObjectSchema(t.BaseType)
            End If

            ' Process this class
            strTable = t.Name()
            ' Obtain the list of columns for the desired table
            dt = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, New String() {Nothing, Nothing, strTable, "TABLE"})

            ' Does table exist?
            If (dt.Rows.Count = 0) Then
                ' #No: create it
                bSucces = bSucces And Me.OOPCreateObjectTable(t)
            Else
                ' #Yes: Update table
                bSucces = bSucces And Me.OOPUpdateObjectTable(t, conn)
            End If

            Return bSucces

        End Function

#End Region ' Schema management

#Region " OOP shared adapters "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Single adapter entry in the 
        ''' <see cref="m_dtOOPAdapterCache">adapter cache</see>.
        ''' </summary>
        ''' <remarks>
        ''' The adapter cache is meant to speed up object access during a single
        ''' database transaction while writing nested classes to the OOP database.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        <Browsable(False)> _
        Private Class cOOPAdapterCacheEntry

#Region " Privates "

            ''' <summary>Name of the table that an adapter references to.</summary>
            Private m_strTable As String
            ''' <summary>The cached adapter.</summary>
            Private m_adapter As IDataAdapter
            ''' <summary>Number of references to a cached adapater.</summary>
            Private m_iRefCount As Integer = 0

#End Region ' Privates

#Region " Constructor "

            ''' ---------------------------------------------------------------
            ''' <summary>
            ''' Contructs a new cache entry.
            ''' </summary>
            ''' <param name="strTable">The table the adapter was obtained for.</param>
            ''' <param name="adapter">The database adapter that was returned for
            ''' this table.</param>
            ''' ---------------------------------------------------------------
            Public Sub New(ByVal strTable As String, ByVal adapter As IDataAdapter)
                Me.m_strTable = strTable
                Me.m_adapter = adapter
                Me.m_iRefCount = 0
            End Sub

#End Region ' Constructor

#Region " Public interfaces "

            ''' ---------------------------------------------------------------
            ''' <summary>
            ''' Increase adapter usage count.
            ''' </summary>
            ''' ---------------------------------------------------------------
            Public Sub AddRef()
                Me.m_iRefCount += 1
            End Sub

            ''' ---------------------------------------------------------------
            ''' <summary>
            ''' Decrease adapter usage count.
            ''' </summary>
            ''' ---------------------------------------------------------------
            Public Sub RemoveRef()
                Me.m_iRefCount -= 1
            End Sub

            ''' ---------------------------------------------------------------
            ''' <summary>
            ''' States if the adapter is no longer used.
            ''' </summary>
            ''' <returns>
            ''' True if released.
            ''' </returns>
            ''' ---------------------------------------------------------------
            Public Function Released() As Boolean
                Return Me.m_iRefCount = 0
            End Function

            ''' ---------------------------------------------------------------
            ''' <summary>
            ''' Returns the referenced adapter.
            ''' </summary>
            ''' ---------------------------------------------------------------
            Public ReadOnly Property Adapter() As IDataAdapter
                Get
                    Return Me.m_adapter
                End Get
            End Property

            ''' ---------------------------------------------------------------
            ''' <summary>
            ''' Returns the name of the referenced table.
            ''' </summary>
            ''' ---------------------------------------------------------------
            Public ReadOnly Property Table() As String
                Get
                    Return Me.m_strTable
                End Get
            End Property

#End Region ' Public interfaces

        End Class

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Cache of open database adapters.
        ''' </summary>
        ''' <remarks>
        ''' <para>When writing an OOP class structure, objects are written recursively
        ''' to the database, ensuring that baseclass information is written first. Linked
        ''' objects are written whenever a reference is encountered. Due to this unpredictable
        ''' flow, chances are that database tables need to accessed for writing several
        ''' times when writing a single object instance.</para>
        ''' <para>A database will deny multiple adapter request for writing. To overcome this
        ''' problem, the adapter cache maintains a list of open adapters available while saving
        ''' OOP data which can be reused until the entire write operation is done.</para>
        ''' <para>Adapters are obtained via <see cref="OOPGetAdapter">OOPGetAdapter</see>,
        ''' and are released via <see cref="OOPReleaseAdapter">OOPReleaseAdapter</see>.</para>
        ''' </remarks>
        ''' ------------------------------------------------------------------- 
        Private m_dtOOPAdapterCache As New Dictionary(Of String, cOOPAdapterCacheEntry)

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Obtain a database adapter from the adapter cache.
        ''' </summary>
        ''' <param name="strTable">Table name to obtain the adapter for.</param>
        ''' <returns>A database adapter if succesful, or Nothing if an error occurred.</returns>
        ''' <remarks>
        ''' An adapter obtained via this method must be released via 
        ''' <see cref="OOPReleaseAdapter">OOPReleaseAdapter</see>
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Protected Function OOPGetAdapter(ByVal strTable As String) As IDataAdapter
            Dim wl As cOOPAdapterCacheEntry = Nothing
            If Not m_dtOOPAdapterCache.ContainsKey(strTable) Then
                wl = New cOOPAdapterCacheEntry(strTable, Me.GetAdapter("SELECT * FROM " + strTable))
                m_dtOOPAdapterCache(strTable) = wl
            Else
                wl = m_dtOOPAdapterCache(strTable)
            End If
            wl.AddRef()
            Return wl.Adapter
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Release a database adapter from the adapter cache that was previously
        ''' obtained via <see cref="OOPGetAdapter">OOPGetAdapter</see>.
        ''' </summary>
        ''' <param name="strTable">Table name to release the adapter for.</param>
        ''' <returns>True if the adapter was released succesfully, or False 
        ''' if an error occurred.</returns>
        ''' -------------------------------------------------------------------
        Protected Function OOPReleaseAdapter(ByVal strTable As String) As Boolean
            Dim wl As cOOPAdapterCacheEntry = Nothing
            If m_dtOOPAdapterCache.ContainsKey(strTable) Then
                wl = m_dtOOPAdapterCache(strTable)
                wl.RemoveRef()
                If wl.Released() Then
                    m_dtOOPAdapterCache.Remove(strTable)
                    Return Me.ReleaseAdapter(wl.Adapter)
                End If
                Return True
            Else
                Debug.Assert(False)
            End If
            Return False
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Helper method, states if there are open adapters left in the adapter cache.
        ''' </summary>
        ''' <returns>True if there are any open adapters left in the cache.</returns>
        ''' <remarks>There should be no more open adapters left when a write
        ''' operation is complete.</remarks>
        ''' -------------------------------------------------------------------
        Protected Function OOPHasOpenAdapters() As Boolean
            Return (Me.m_dtOOPAdapterCache.Count > 0)
        End Function

#End Region ' OOP shared adapters

#End Region ' OOP Admin internals

#End Region ' Admin

#Region " OOP internals "

#Region " Helpers "

        ''' <summary>
        ''' Reserved column name for storing class names in the OOP database schema
        ''' </summary>
        Private Const OOP_CLASSNAMECOL As String = "xCLASS_NAMEx"

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Helper method, returns a SQL table name for a given class/type.
        ''' </summary>
        ''' <param name="t">The class to return the table name for.</param>
        ''' <returns>A table name for the given class.</returns>
        ''' <remarks>
        ''' This method will convert invalid namespace characters into
        ''' valid SQL table name characters.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Private Function OOPGetTableName(ByVal t As Type) As String
            ' Get class name (including namespaces)
            Dim strName As String = t.Name()
            ' Replace invalid SQL characters
            Return strName.Replace(".", "_").Replace("+", "_")
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Helper method, returns a SQL column name for a given property.
        ''' </summary>
        ''' <param name="pi">The property info instance to obtain a column
        ''' name for.</param>
        ''' <returns>A column name.</returns>
        ''' -------------------------------------------------------------------
        Private Function OOPGetColumnName(ByVal pi As PropertyInfo) As String
            Return pi.Name()
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Helper method, states whether a given class is inherited directly 
        ''' from Object.
        ''' </summary>
        ''' <param name="t">The class to test.</param>
        ''' <returns>True if class <paramref name="t">t</paramref> is directly
        ''' inherited from Object.</returns>
        ''' -------------------------------------------------------------------
        Private Function OOPIsBaseClass(ByVal t As Type) As Boolean
            Return t.BaseType Is GetType(Object)
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the property that holds the primary key for a 
        ''' <see cref="cOOPStorable">cOOPStorable</see>-derived <see cref="Type">Type</see>.
        ''' </summary>
        ''' <param name="t">The <see cref="cOOPStorable">cOOPStorable</see>-derived 
        ''' <see cref="Type">Type</see> to get the primary key for.</param>
        ''' <returns>A <see cref="PropertyInfo">PropertyInfo</see> instance, or
        ''' nothing if the primary key property was not found. Which is not good;
        ''' this will probably only occur when the class was not properly derived.</returns>
        ''' -------------------------------------------------------------------
        Private Function OOPGetKeyProperty(ByVal t As Type) As PropertyInfo
            Return t.GetProperty("DBID")
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Helper method; returns all writable public properties that are either
        ''' directly declared by a provided <paramref name="t">class</paramref>,
        ''' or that serves as the primary key to the class structure. Only
        ''' properties of classes derived from <see cref="cOOPStorable">cOOPStorable</see>
        ''' are returned.
        ''' </summary>
        ''' <param name="t">The <see cref="cOOPStorable">cOOPStorable</see>-derived
        ''' <see cref="Type">Type</see> to find storable properties for.</param>
        ''' <returns>An array of <see cref="PropertyInfo">PropertyInfo</see> instances.</returns>
        ''' -------------------------------------------------------------------
        Private Function OOPGetStorableProperties(ByVal t As Type) As PropertyInfo()
            Dim lpi As New List(Of PropertyInfo)
            Dim bAllowed As Boolean = False

            ' ToDo: test if Type t is derived of cOOPStorable?
            If GetType(cOOPStorable).IsAssignableFrom(t) Then
                For Each pi As PropertyInfo In t.GetProperties()
                    ' Allow (props declared directly in this class) AND (the property is writable)
                    bAllowed = t.Equals(pi.DeclaringType) And (pi.CanWrite())
                    ' Also allow primary key
                    bAllowed = bAllowed Or (pi.Name = "DBID")
                    ' Allowed?
                    If (bAllowed) Then
                        ' #Yes: add it
                        lpi.Add(pi)
                    End If
                Next
            End If

            Return lpi.ToArray()
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Helper method, returns the SQL data type for a given property.
        ''' </summary>
        ''' <param name="pi">The property to obtain the SQL data type for.</param>
        ''' <returns>
        ''' An SQL data type name, or an empty string if the property value type
        ''' was not supported. The list of supported data types can easily be 
        ''' extended.
        ''' </returns>
        ''' -------------------------------------------------------------------
        Private Function OOPGetColumnType(ByVal pi As PropertyInfo) As String
            Dim strType As String = pi.PropertyType.ToString()
            Select Case strType
                Case "System.Double"
                    Return "DOUBLE"
                Case "System.Single"
                    Return "SINGLE"
                Case "System.Int64"
                    Return "LONG" ' BIGINT?
                Case "System.Int32"
                    Return "INTEGER"
                Case "System.Int16"
                    Return "SHORT" ' SMALLINT?
                    'Case "System.Byte"
                    '    Return "SHORT"
                Case "System.Boolean"
                    ' I'm refusing to use Access 'YESNO' because it's not portable
                    Return "SHORT"
                Case "System.String"
                    ' Perform property browsable attribute length check?
                    Return "TEXT(255)"
                Case Else
                    ' Check for FK
                    If OOPIsForeignKeyProperty(pi) Then
                        ' Store DBID of FK
                        Return "INTEGER"
                    End If
                    ' This list can be greatly extended
            End Select
            Return ""
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the default value for a property.
        ''' </summary>
        ''' <param name="pi">The property info to get the default value for.</param>
        ''' <returns>
        ''' </returns>
        ''' <remarks>
        ''' JS 06Mar09: not implemented yet; need to figure out how to get to
        ''' the actual 'Default' attribute.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Private Function OOPGetPropertyDefaultValue(ByVal pi As PropertyInfo) As Object
            Dim pd As PropertyDescriptor = cPropertyConverter.FindOrigPropertyDescriptor(pi)
            Return Nothing
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Assembly name cache, added to optimize the process of relocating types
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private m_dtAssemblyNames As New Dictionary(Of String, Assembly)

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Helper method, translats a class/type to a unique string.
        ''' </summary>
        ''' <param name="t">The class to convert.</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' The counterpart of this method, <see cref="OOPStringToType">OOPStringToType</see>,
        ''' can be used to find the originating type from a string.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Private Function OOPTypeToString(ByVal t As Type) As String

            ' Include assembly short name in the type name. This enables
            ' the OOP database logic to relocate the type from its original
            ' assembly, even if similar class names exist in similar namespaces
            ' in different asssemblies. Yes, it's far fetched, but hey...
            Return t.Assembly.GetName.Name + "!" + t.FullName()

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Helper method, locates the originating type from a type string.
        ''' </summary>
        ''' <param name="strType">The type name to locate the originating type
        ''' for.</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' The counterpart of this method, <see cref="OOPTypeToString">OOPTypeToString</see>,
        ''' can be used to create the string for a type.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Private Function OOPStringToType(ByVal strType As String) As Type

            ' Split assembly short name from type name
            Dim astr As String() = strType.Split(CChar("!"))
            Dim ass As Assembly = Nothing

            ' Optimization: cache assembly names
            If m_dtAssemblyNames.Count = 0 Then
                For Each ass In AppDomain.CurrentDomain.GetAssemblies()
                    m_dtAssemblyNames.Add(ass.GetName.Name, ass)
                Next
            End If

            ' Try to find type name in the named assembly 
            Try
                ass = Me.m_dtAssemblyNames(astr(0))
                ' Found assembly! Now return the contained type (fingers crossed)
                Return ass.GetType(astr(1))
            Catch ex As Exception

            End Try
            Return Nothing

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Helper method, determines if the value type of a given property 
        ''' is, or is inherited from, cOOPStorable.
        ''' </summary>
        ''' <param name="pi">The property info instance to test.</param>
        ''' <returns>
        ''' True if the value type of a given property is, or is inherited from,
        ''' cOOPStorable.
        ''' </returns>
        ''' -------------------------------------------------------------------
        Private Function OOPIsForeignKeyProperty(ByVal pi As PropertyInfo) As Boolean
            ' Is a ref to another cOOPStorable?
            If GetType(cOOPStorable).IsAssignableFrom(pi.PropertyType) Then
                ' Is NOT an indexed prop
                Return (pi.GetIndexParameters.Length = 0)
            End If
            Return False
        End Function

#End Region ' Helpers

#Region " Read "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Read a single cOOPStorable-derived instance from the database.
        ''' </summary>
        ''' <param name="t">The class/type of the object to read.</param>
        ''' <param name="iDBID">The database ID of the object to read.</param>
        ''' <param name="piKey">The property in the <paramref name="t">given type</paramref> 
        ''' where the <paramref name="iDBID">database ID</paramref> should be stored.</param>
        ''' <returns>
        ''' A cOOPStorable-derived instance, or nothing if an error occurred.
        ''' </returns>
        ''' -------------------------------------------------------------------
        Private Function OOPReadObject(ByVal t As Type, ByVal iDBID As Integer, ByVal piKey As PropertyInfo) As cOOPStorable

            Dim objRead As cOOPStorable = Nothing
            Try
                objRead = CType(System.Activator.CreateInstance(t), cOOPStorable)
                objRead.DBID = iDBID
            Catch ex As Exception
                Return Nothing
            End Try

            ' Able to read the object with the primary key?
            If Me.OOPReadObjectRecursive(t, objRead, piKey, iDBID) Then
                ' #Yes: remember the instance
                Me.m_OOPObjectCache.AddObject(objRead)
                ' Return the object that was successfully read
                Return objRead
            Else
                ' Report failure
                Return Nothing
            End If
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Recursively read a single cOOPStorable-derived instance from the 
        ''' database along the inheritance tree.
        ''' </summary>
        ''' <param name="t">The type in the class hierarchy of instance 
        ''' <paramref name="objRead">objRead</paramref> to read.</param>
        ''' <param name="objRead">Object to read.</param>
        ''' <param name="piKey">Property in the object that holds the database
        ''' ID.</param>
        ''' <param name="iDBID">Database ID value of the object being read.</param>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Private Function OOPReadObjectRecursive(ByVal t As Type, ByVal objRead As cOOPStorable, ByVal piKey As PropertyInfo, ByVal iDBID As Integer) As Boolean

            Dim api As PropertyInfo() = Me.OOPGetStorableProperties(t)
            Dim strTable As String = Me.OOPGetTableName(t)
            Dim strColumnName As String = Me.OOPGetColumnName(piKey)
            Dim strColumnType As String = Me.OOPGetColumnName(piKey)
            Dim strValue As String = ""
            Dim strSQL As String = ""
            Dim reader As IDataReader = Nothing
            Dim bIsBaseClass As Boolean = False
            Dim bSucces As Boolean = True

#If VERBOSE_LEVEL >= 2 Then
            Console.WriteLine("Reading {0}.{1}", strTable, iDBID)
#End If

            ' Not the base class?
            If Not Me.OOPIsBaseClass(t) Then
                ' #Indeed: read base class first
                bSucces = bSucces And Me.OOPReadObjectRecursive(t.BaseType, objRead, piKey, iDBID)
            End If

            ' All good so far?
            If bSucces Then
                ' #Yes: read this specific class from the database
                Try
                    strSQL = String.Format("SELECT * FROM {0} WHERE DBID={1}", strTable, iDBID)
                    reader = Me.GetReader(strSQL)

                    ' Grab one single record
                    reader.Read()

                    ' For all properties in the given type
                    For Each pi As PropertyInfo In api
                        ' Extract database equivalents
                        strColumnName = Me.OOPGetColumnName(pi)
                        strColumnType = Me.OOPGetColumnType(pi)

                        ' Is the data type of this particular property supported?
                        If Not String.IsNullOrEmpty(strColumnType) Then
                            ' Is this NOT the primary key?
                            If String.Compare("DBID", strColumnName) <> 0 Then
                                ' Is this a foreign key property?
                                If Me.OOPIsForeignKeyProperty(pi) Then
                                    ' #Yes: read FK
                                    Try
                                        ' Get FK ID
                                        Dim iLinkedDBID As Integer = CInt(reader(strColumnName))
                                        Dim objFK As cOOPStorable = Nothing

                                        ' Has object attached?
                                        If iLinkedDBID > 0 Then
                                            ' FK object not read yet?
                                            If Not Me.m_OOPObjectCache.HasObject(iLinkedDBID) Then
                                                ' #Yes: read object into cache
                                                If Me.ReadObject(Me.ReadObjectKey(iLinkedDBID)) Is Nothing Then
#If VERBOSE_LEVEL >= 1 Then
                                                    Console.WriteLine("Read: fk object {0} failed to load for {1}.{2}", iLinkedDBID, strColumnName, strTable)
#End If
                                                End If
                                            End If
                                            ' Get the object
                                            objFK = Me.m_OOPObjectCache.GetObject(iLinkedDBID)
                                        End If
                                        ' Store FK
                                        pi.SetValue(objRead, objFK, Nothing)

                                    Catch ex As Exception
                                        Console.WriteLine("Read: failed to read FK {0}.{1}: {2}", strColumnName, strTable, ex.Message)
                                        bSucces = False
                                    End Try
                                Else ' Me.OOPIsForeignKeyProperty(pi)
                                    ' #No: just read the property value
                                    Try
                                        ' Special cases
                                        If pi.PropertyType Is GetType(Boolean) Then
                                            pi.SetValue(objRead, Convert.ToBoolean(reader(strColumnName)), Nothing)
                                        Else
                                            pi.SetValue(objRead, reader(strColumnName), Nothing)
                                        End If
                                    Catch ex As Exception
                                        ' ToDo: assign property default value (which can be obtained from pi.Attributes
                                        'pi.SetValue(objRead, pi.Attributes, Nothing)
                                        Console.WriteLine("Read: skipped col {0}.{1} ({2})", strColumnName, strTable, strColumnType)
                                    End Try
                                End If
                            End If
                        End If
                    Next

                Catch ex As Exception
                    Console.WriteLine("Read: error when reading {0}: {1}", strTable, ex.Message)
                    bSucces = False
                End Try
            End If

            If GetType(cOOPStorableList).Equals(t) Then
                bSucces = bSucces And Me.OOPReadListItems(DirectCast(objRead, cOOPStorableList))
            End If

            Return bSucces
        End Function

        ''' <summary>
        ''' Helper method, write contents of list 
        ''' </summary>
        ''' <param name="list"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function OOPReadListItems(ByVal list As cOOPStorableList) As Boolean

            Dim strTable As String = "cOOPStorableListItems"
            Dim strSQL As String = ""
            Dim reader As IDataReader = Nothing
            Dim item As cOOPStorable = Nothing
            Dim key As cOOPKey = Nothing
            Dim bSucces As Boolean = True

#If VERBOSE_LEVEL >= 2 Then
            Console.WriteLine("Reading list items {0}", list.DBID)
#End If

            strSQL = String.Format("SELECT * FROM {0} WHERE DBID={1}", strTable, list.DBID)
            reader = Me.GetReader(strSQL)

            Try
                While reader.Read
                    key = Me.ReadObjectKey(CInt(reader("item")))
                    item = Me.ReadObject(key)
                    If item IsNot Nothing Then
                        list.Add(item)
                    End If
                End While
            Catch ex As Exception
                bSucces = False
                Console.WriteLine("Error {0} reading list {1}", ex.Message, list.DBID)
            End Try
            Return bSucces
        End Function

#End Region ' Read

#Region " Write "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Helper method, write contents of a list.
        ''' </summary>
        ''' <param name="list"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' -------------------------------------------------------------------
        Private Function OOPWriteListItems(ByVal list As cOOPStorableList) As Boolean

            Dim adapter As IDataAdapter = Nothing
            Dim item As cOOPStorable = Nothing
            Dim strTable As String = "cOOPStorableListItems"
            Dim drow As DataRow = Nothing
            Dim iRow As Integer = 0
            Dim nRows As Integer = 0
            Dim ds As DataSet = Nothing
            Dim dt As DataTable = Nothing
            Dim bSucces As Boolean = True

            adapter = Me.OOPGetAdapter(strTable)

            ' Clear list from DB
            ds = Me.GetDataSet(adapter, strTable)
            dt = ds.Tables(0)
            nRows = dt.Rows.Count
            iRow = 0

            ' Remove current rows for the list
            While iRow < nRows - 1
                drow = dt.Rows(iRow)
                If CInt(drow("DBID")) = list.DBID Then
                    dt.Rows.RemoveAt(iRow) : nRows -= 1
                Else
                    iRow += 1
                End If
            End While

            ' Write new items
            For iItem As Integer = 0 To list.Count - 1
                item = list(iItem)
                bSucces = bSucces And Me.WriteObject(item)
                If bSucces Then
                    drow = dt.NewRow()
                    drow("DBID") = list.DBID
                    drow("Item") = item.DBID
                    dt.Rows.Add(drow)
                End If
            Next iItem

            Me.CommitDataSet(ds, adapter, strTable)

            Me.OOPReleaseAdapter(strTable)

            ds = Nothing
            dt = Nothing
            adapter = Nothing

            Return True
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="t"></param>
        ''' <param name="obj"></param>
        ''' <param name="piKey"></param>
        ''' <returns></returns>
        Private Function OOPWriteObjectRecursive(ByVal t As Type, ByVal obj As cOOPStorable, ByVal piKey As PropertyInfo) As Boolean

            Dim api As PropertyInfo() = Me.OOPGetStorableProperties(t)
            Dim strTable As String = Me.OOPGetTableName(t)
            Dim strColumnName As String = ""
            Dim strColumnType As String = ""
            Dim adapter As IDataAdapter = Nothing
            Dim ds As DataSet = Nothing
            Dim dt As DataTable = Nothing
            Dim drow As DataRow = Nothing
            Dim bNewRow As Boolean = False
            Dim bIsBaseClass As Boolean = Me.OOPIsBaseClass(t)
            Dim bSucces As Boolean = True

            ' Not the base class?
            If Not bIsBaseClass Then
                bSucces = Me.OOPWriteObjectRecursive(t.BaseType, obj, piKey)
            End If

            If bSucces Then

                Try
                    adapter = Me.OOPGetAdapter(strTable)

                    ds = Me.GetDataSet(adapter, Me.OOPGetTableName(t))
                    dt = ds.Tables(0)

                    drow = dt.Rows.Find(piKey.GetValue(obj, Nothing))

                    bNewRow = (drow Is Nothing)
                    If bNewRow Then
                        drow = dt.NewRow()
                        If bIsBaseClass Then
                            ' Write baseclass class name
                            drow(OOP_CLASSNAMECOL) = Me.OOPTypeToString(obj.GetType())
                        End If
                    Else
                        drow.BeginEdit()
                    End If

                    For Each pi As PropertyInfo In api
                        strColumnName = Me.OOPGetColumnName(pi)
                        strColumnType = Me.OOPGetColumnType(pi)

                        ' Is column type supported?
                        If Not String.IsNullOrEmpty(strColumnType) Then
                            ' #Yes: is this a foreign key?
                            If Me.OOPIsForeignKeyProperty(pi) Then
                                ' #Yes: write foreign key value
                                Dim objFK As cOOPStorable = DirectCast(pi.GetValue(obj, Nothing), cOOPStorable)
                                Dim iDBIDFK As Integer = 0
                                ' Has linked object attached?
                                If (objFK IsNot Nothing) Then
                                    ' #Yes: get DBID for linked object. 
                                    '     ! Note that this ID might not yet be assigned
                                    iDBIDFK = objFK.DBID
                                    ' Test if referenced object needs to be stored first
                                    If Not Me.m_OOPObjectCache.HasObject(iDBIDFK) Then
                                        ' Write linked object
                                        If Me.WriteObject(objFK) Then
                                            ' Just in case, obtain DBID again in case WriteObject assigned this
                                            iDBIDFK = objFK.DBID
                                        Else
#If VERBOSE_LEVEL >= 1 Then
                                            Console.WriteLine("Unable to write FK object {0} when writing {1} as {2}", objFK.DBID, obj, strTable)
#End If
                                            iDBIDFK = 0
                                        End If
                                    End If
                                End If
                                ' Write FK key value
                                drow(strColumnName) = iDBIDFK
                            Else
                                ' #No: Just write supported value
                                drow(strColumnName) = pi.GetValue(obj, Nothing)
                            End If
                        Else
#If VERBOSE_LEVEL >= 2 Then
                            Console.WriteLine("Column type {0} not supported when writing {1} as {2}", strColumnName, obj, strTable)
#End If
                        End If
                    Next

                    If bNewRow Then dt.Rows.Add(drow) Else drow.EndEdit()

                Catch ex As Exception
#If VERBOSE_LEVEL >= 1 Then
                    Console.WriteLine("Error {0} while saving {1} as {2}", ex.Message, obj, t.Name)
#End If
                    bSucces = False
                End Try

                Me.CommitDataSet(ds, adapter, strTable)

                Me.ReleaseDataSet(ds)
                Me.OOPReleaseAdapter(strTable)

            End If

            adapter = Nothing
            ds = Nothing
            dt = Nothing

            If GetType(cOOPStorableList).Equals(t) Then
                bSucces = bSucces And Me.OOPWriteListItems(DirectCast(obj, cOOPStorableList))
            End If

            Return bSucces
        End Function

#End Region ' Write

#End Region ' OOP internal helper methods

#End Region ' OOP

#Region " EwE versioning "

        Private m_sVersion As Single = 0.0

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns the current version of the connected EwE database.
        ''' </summary>
        ''' <returns>
        ''' A Single value with the version latest version number of the connected database.
        ''' </returns>
        ''' -------------------------------------------------------------------
        Public Function GetVersion() As Single

            If Me.m_sVersion = 0.0 Then
                Try
                    ' Try EwE6 version first
                    Me.m_sVersion = CSng(Me.GetValue("Select Max(Version) FROM [UpdateLog]"))
                    If (Me.m_sVersion = 0.0) Then
                        ' Try EwE5 version
                        Me.m_sVersion = CSng(Me.GetValue("Select Max(Version) FROM [Database specifications]"))
                    End If
                Catch ex As Exception
                    Me.m_sVersion = 0.0
                End Try
            End If
            Return Me.m_sVersion

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Updates the version of the database
        ''' </summary>
        ''' <param name="sVersion">The version to set</param>
        ''' <param name="strComments">The description to use</param>
        ''' <returns>True if succesful</returns>
        ''' <remarks>This method only allows setting the version on an EwE6 database.</remarks>
        ''' -------------------------------------------------------------------
        Public Function SetVersion(ByVal sVersion As Single, ByVal strComments As String) As Boolean

            Dim dtNow As Date = Date.Now()
            Dim strSQL As String = String.Format("INSERT INTO UpdateLog VALUES('{0}', '{2}', '{1}')", sVersion, strComments, dtNow.ToShortDateString())
            Dim bSucces As Boolean = True
            Try
                bSucces = Me.Execute(strSQL)
                Me.m_sVersion = sVersion
            Catch ex As Exception
                bSucces = False
            End Try
            Return bSucces

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Extract the major version number from a given version number.
        ''' </summary>
        ''' <param name="sVersion">The version number to examine.</param>
        ''' <returns>The major version number of the given version number.</returns>
        ''' <remarks>
        ''' <para>'6.0' returns '6'</para>
        ''' <para>'2.93' returns '2'</para>
        ''' <para>'-4.4' returns '4'</para>
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Shared Function GetMajorVersion(ByVal sVersion As Single) As Single
            Return CSng(Math.Sign(sVersion) * Math.Floor(Math.Abs(sVersion)))
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Extract the minor version number from a given version number.
        ''' </summary>
        ''' <param name="sVersion">The version number to examine.</param>
        ''' <returns>The minor version number of the given version number.</returns>
        ''' <remarks>
        ''' <para>'6.0' returns '0.0'</para>
        ''' <para>'2.93' returns '0.93'</para>
        ''' <para>'-4.4' returns '0.4'</para>
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Shared Function GetMinorVersion(ByVal sVersion As Single) As Single
            Dim sAbsVersion As Single = Math.Abs(sVersion)
            Return CSng(sAbsVersion - Math.Floor(sAbsVersion))
        End Function

#End Region ' EwE versioning

#Region " Driver information "

        ' get the database/server details and display them
        'Dim dt As DataTable = conn.GetSchema("DataSourceInformation")
        'output-label.Text = String.Format("{0} (version {1})", _
        '            dt.Rows(0)("DataSourceProductName").ToString(), _
        '            dt.Rows(0)("DataSourceProductVersion").ToString())

#End Region ' Driver information

    End Class

End Namespace
