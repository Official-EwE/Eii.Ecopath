Imports System.ComponentModel
Imports System.Data.OleDb
Imports System.Reflection
Imports EwECore.Database
Imports EwEUtils.Logging
Imports EwEUtils.Utilities
Imports Microsoft.Extensions.Logging
Imports Debug = System.Diagnostics.Debug

Namespace ValueChainMigrator.LegacyData

#Region " OOP Public classes "


#End Region ' OOP Public classes

    Public Class cLegacyOOPStorage

        Private m_db As cEwEDatabase = Nothing
        Private ReadOnly m_logger As ILogger = LoggingContext.CreateLogger(Of cLegacyOOPStorage)()
        Private m_strSelfName As String = ""

        Public Sub New(db As cEwEDatabase)
            Me.m_db = db

            Dim strTable As String = Me.OOPGetTableName(GetType(cOOPStorable))

            Me.m_iNextDBID = 1
            If Me.m_db.HasTable(strTable) Then Me.m_iNextDBID = CInt(Me.m_db.GetValue(String.Format("SELECT MAX(DBID) FROM {0}", strTable), 0)) + 1

            ' Create schema verification cache
            Me.m_OOPObjectSchemaVerified = New List(Of Type)
            ' Create saved object cache
            Me.m_OOPObjectCache = New cOOPObjectCache()

        End Sub

#Region " OOP public interfaces "

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
        Protected Function ReadObjectKey(iDBID As Integer) As cOOPKey

            Dim reader As IDataReader = Nothing
            Dim strTypeName As String = ""
            Dim tOrg As Type = Nothing
            Dim strSQL As String = ""
            Dim objKey As cOOPKey = Nothing

            ' Construct SQL statement to query the runtime information from the 
            ' OOP core table 'cOOPStorable'.
            strSQL = String.Format("SELECT {0}, DBID FROM {1} WHERE DBID={2}", OOP_CLASSNAMECOL, Me.OOPGetTableName(GetType(cOOPStorable)), iDBID)
            reader = Me.m_db.GetReader(strSQL)
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
                Me.m_db.ReleaseReader(reader)
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
        Protected Function ReadObjectKeys(t As Type, Optional bIncludeInherited As Boolean = True) As cOOPKey()

            Dim strTypeName As String = ""
            Dim strSQL As String = ""
            Dim reader As IDataReader = Nothing
            Dim objKey As cOOPKey = Nothing
            Dim lKeys As New List(Of cOOPKey)
            Dim tOrg As Type = Nothing
            Dim bInclude As Boolean = True

            strSQL = String.Format("SELECT {0}, DBID FROM {1} ORDER BY DBID ASC", OOP_CLASSNAMECOL, Me.OOPGetTableName(GetType(cOOPStorable)))
            reader = Me.m_db.GetReader(strSQL)

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
                    Me.m_db.ReleaseReader(reader)

                Catch ex As Exception
                    m_logger.LogError(ex, "cEwEDatabase.ReadObjectKeys(" & strSQL & ")")
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
        Public Function ReadObject(key As cOOPKey) As cOOPStorable
            If (key Is Nothing) Then
                Return Nothing
            End If
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
        Public Function ReadObject(t As Type, iDBID As Integer) As cOOPStorable
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
        Public Function ReadObjects(t As Type,
                                    Optional bIncludeInherited As Boolean = True) As cOOPStorable()

            Dim lObjs As New List(Of cOOPStorable)

            If (Not Me.m_db.HasTable(OOPGetTableName(GetType(cOOPStorable)))) Then Return lObjs.ToArray()

            Dim aKeys As cOOPKey() = Me.ReadObjectKeys(t, bIncludeInherited)
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

        ''' <summary>Assembly for OOP transaction.</summary>
        Private m_assTransaction As Assembly = Nothing

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Starts an OOP database transaction. In addition to a regular 
        ''' <see cref="BeginTransaction">transaction</see> this method also
        ''' opens all adapters for <see cref="cOOPStorable"/>-derived classes
        ''' in a given assembly to reduce the amount of adapter traffic while
        ''' the transaction is open.
        ''' </summary>
        ''' <param name="ass">The assembly to load types from.</param>
        ''' <returns>True if successful.</returns>
        ''' -------------------------------------------------------------------
        Public Function OOPBeginTransaction(ass As Assembly, Optional bCreateSchema As Boolean = False) As Boolean

            ' Sanity checks
            Debug.Assert(ass IsNot Nothing)
            Debug.Assert(Me.m_assTransaction Is Nothing)

            Dim bSuccess As Boolean = True

            If Not Me.m_db.BeginTransaction() Then Return False

            Me.m_assTransaction = ass

            For Each t As Type In Me.m_assTransaction.GetTypes()
                If t.IsSubclassOf(GetType(cOOPStorable)) Then
                    If (bCreateSchema) Then
                        bSuccess = bSuccess And OOPUpdateObjectSchema(t)
                    End If

                    Dim strTable As String = Me.OOPGetTableName(t)
                    While Not OOPIsBaseClass(t)
                        Me.OOPGetAdapter(strTable)
                        t = t.BaseType
                    End While
                End If
            Next
            Return bSuccess

        End Function

        Public Function OOPCommitTransaction(Optional bCommit As Boolean = True) As Boolean

            ' Sanity checks
            Debug.Assert(Me.m_assTransaction IsNot Nothing)

            For Each t As Type In Me.m_assTransaction.GetTypes()
                If t.IsSubclassOf(GetType(cOOPStorable)) Then
                    Dim strTable As String = Me.OOPGetTableName(t)
                    While Not OOPIsBaseClass(t)
                        Me.OOPReleaseAdapter(strTable)
                        t = t.BaseType
                    End While
                End If
            Next
            Debug.Assert(Not OOPHasOpenAdapters())

            Me.m_assTransaction = Nothing
            Return Me.m_db.CommitTransaction(bCommit)

        End Function

        Public Function OOPRollbackTransaction() As Boolean

            ' Sanity checks
            Debug.Assert(Me.m_assTransaction IsNot Nothing)

            For Each t As Type In Me.m_assTransaction.GetTypes()
                If t.IsAssignableFrom(GetType(cOOPStorable)) Then
                    Dim strTable As String = Me.OOPGetTableName(t)
                    While Not OOPIsBaseClass(t)
                        Me.OOPReleaseAdapter(strTable)
                        t = t.BaseType
                    End While
                End If
            Next
            Debug.Assert(Not OOPHasOpenAdapters())

            Me.m_assTransaction = Nothing
            Return Me.m_db.RollbackTransaction()

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Write an <see cref="cOOPStorable">OOPstorable-derived</see> instance 
        ''' to the database.
        ''' </summary>
        ''' <param name="obj">The object to save.</param>
        ''' <remarks>
        ''' This method will verify - and update if  necessary - the database
        ''' schema for this class of object and its base classes. This check 
        ''' is performed only the very first time a particular object is written 
        ''' to the database; consecutive save attempts will bypass the check for
        ''' performance reasons.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Function WriteObject(obj As cOOPStorable) As Boolean

#If DEBUG Then
            ' Sanity check
            Debug.Assert(Not obj.Deleted, "Object is deleted and cannot not be saved.")
#End If

            Dim t As Type = obj.GetType()
            Dim api As PropertyInfo() = Me.OOPGetStorableProperties(t)
            Dim piKey As PropertyInfo = Nothing
            Dim bSucces As Boolean = True

            '' Make sure database schema is up to date to accomodate this object
            'bSucces = Me.OOPUpdateObjectSchema(t)

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
        ''' <returns>True if successful.</returns>
        ''' -------------------------------------------------------------------
        Public Function DeleteObject(obj As cOOPStorable) As Boolean

            Dim bSucces As Boolean = True

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
        ''' <returns>True if successful.</returns>
        ''' -------------------------------------------------------------------
        Private Function DeleteObjectRecursive(obj As cOOPStorable, t As Type) As Boolean

            Dim strSQL As String = String.Format("DELETE FROM {0} WHERE DBID={1}", Me.OOPGetTableName(t), obj.DBID)
            Dim bSucces As Boolean = True

            ' Able to delete object at class level 't' from the database
            If Me.m_db.Execute(strSQL) Then
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

        ''' <summary>The unique ID to assigne to new cOOPStorable instances
        ''' in the database.</summary>
        Private m_iNextDBID As Integer = -1
        ''' <summary>Cache of all objects already saved into the database.
        ''' Keeping a cache is much faster than having to query the database
        ''' for every potential save.</summary>
        Private m_OOPObjectCache As cOOPObjectCache = Nothing
        ''' <summary>Cache of all object for which the database schema has
        ''' already been verifyfied since the last time the database
        ''' was opened.</summary>
        Private m_OOPObjectSchemaVerified As List(Of Type) = Nothing

#End Region ' OOP Admin vars

#Region " OOP Amin interfaces "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Internal admin helper; clear the saved object cache.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Sub OOPFlushObjectCache()
            Me.m_OOPObjectCache.Clear()
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Internal admin helper; clear the verified schema cache.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Sub OOPFlushSchemaCache()
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
            Public Sub AddObject(obj As cOOPStorable)
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
            Public Sub RemoveObject(obj As cOOPStorable)
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
            Public Function GetObject(iDBID As Integer) As cOOPStorable
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
            Public Function HasObject(iDBID As Integer) As Boolean
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
            Public Sub New(strCol As String, strTable As String, bInherited As Boolean)
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
        ''' <returns>True if successful.</returns>
        ''' -------------------------------------------------------------------
        Private Function OOPCreateObjectTable(t As Type) As Boolean

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
            bSucces = Me.m_db.Execute(strQuery)

            ' Create primary key for this table
            strQuery = String.Format("ALTER TABLE {0} ADD PRIMARY KEY (DBID)", Me.OOPGetTableName(t))
            bSucces = bSucces And Me.m_db.Execute(strQuery)

            ' Create all FKs
            For Each fk As OOP_sFKInfo In lFK
                strQuery = String.Format("ALTER TABLE {2} ADD FOREIGN KEY ({1}) REFERENCES {0} (DBID) ON DELETE CASCADE",
                    fk.TableName,
                    fk.ColumnName, Me.OOPGetTableName(t))
                bSucces = bSucces And Me.m_db.Execute(strQuery)
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
        ''' <returns>True if successful.</returns>
        ''' <remarks>
        ''' <para>This method does not recurse; a single type is processed for a 
        ''' single table. To process an enitre object inheritance tree, use
        ''' <see cref="OOPUpdateObjectSchema">OOPUpdateObjectSchema</see>.</para>
        ''' <para>Note that this method does not handle column datatype 
        ''' conversions; only mssing columns are added.</para>
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Private Function OOPUpdateObjectTable(t As Type, conn As OleDbConnection) As Boolean

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
                bSucces = Me.m_db.Execute(strSQL)
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
        ''' True if successful.
        ''' </returns>
        ''' -------------------------------------------------------------------
        Private Function OOPUpdateObjectSchema(t As Type) As Boolean

            Dim conn As OleDbConnection = DirectCast(Me.m_db.GetConnection(), OleDbConnection)
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
        <Browsable(False)>
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
            Public Sub New(strTable As String, adapter As IDataAdapter)
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
        ''' <returns>A database adapter if successful, or Nothing if an error occurred.</returns>
        ''' <remarks>
        ''' An adapter obtained via this method must be released via 
        ''' <see cref="OOPReleaseAdapter">OOPReleaseAdapter</see>
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Protected Function OOPGetAdapter(strTable As String) As IDataAdapter
            Dim wl As cOOPAdapterCacheEntry = Nothing
            If Not m_dtOOPAdapterCache.ContainsKey(strTable) Then
                wl = New cOOPAdapterCacheEntry(strTable, Me.m_db.GetAdapter("SELECT * FROM " + strTable))
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
        Protected Function OOPReleaseAdapter(strTable As String) As Boolean
            Dim wl As cOOPAdapterCacheEntry = Nothing
            If m_dtOOPAdapterCache.ContainsKey(strTable) Then
                wl = m_dtOOPAdapterCache(strTable)
                wl.RemoveRef()
                If wl.Released() Then
                    m_dtOOPAdapterCache.Remove(strTable)
                    Return Me.m_db.ReleaseAdapter(wl.Adapter)
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
        Private Function OOPGetTableName(t As Type) As String
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
        Private Function OOPGetColumnName(pi As PropertyInfo) As String
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
        Private Function OOPIsBaseClass(t As Type) As Boolean
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
        Private Function OOPGetKeyProperty(t As Type) As PropertyInfo
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
        Private Function OOPGetStorableProperties(t As Type) As PropertyInfo()
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
        Private Function OOPGetColumnType(pi As PropertyInfo) As String
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
        Private Function OOPGetPropertyDefaultValue(pi As PropertyInfo) As Object
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
        Private Function OOPTypeToString(t As Type) As String

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
        Private Function OOPStringToType(strType As String) As Type

            ' Split assembly short name from type name
            Dim i As Integer = strType.LastIndexOf("."c)
            Dim bit As String = "EwEValueChainPlugin.ValueChainMigrator.LegacyData." + strType.Substring(i + 1)
            Dim ass As Assembly = Me.GetType().Assembly
            Return ass.GetType(bit, False)

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
        Private Function OOPIsForeignKeyProperty(pi As PropertyInfo) As Boolean
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
        Private Function OOPReadObject(t As Type, iDBID As Integer, piKey As PropertyInfo) As cOOPStorable

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
        Private Function OOPReadObjectRecursive(t As Type, objRead As cOOPStorable, piKey As PropertyInfo, iDBID As Integer) As Boolean

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
                strSQL = String.Format("SELECT * FROM {0} WHERE DBID={1}", strTable, iDBID)
                reader = Me.m_db.GetReader(strSQL)
                Try
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
                                                    ' Links could not be restored
                                                    Return False
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
                                    Dim bRead As Boolean = False
                                    If Me.m_db.HasColumn(reader, strColumnName) Then
                                        Try
                                            ' Special cases
                                            If pi.PropertyType Is GetType(Boolean) Then
                                                pi.SetValue(objRead, Convert.ToBoolean(reader(strColumnName)), Nothing)
                                            Else
                                                pi.SetValue(objRead, Me.m_db.ReadSafe(reader, strColumnName, Nothing), Nothing)
                                            End If
                                        Catch ex As Exception
                                            bRead = False
                                        End Try
                                    End If

                                    If Not bRead Then
                                        ' ToDo: assign property default value (which can be obtained from pi.Attributes
                                        'pi.SetValue(objRead, pi.Attributes, Nothing)
                                        'Console.WriteLine("Read: skipped col {0}.{1} ({2})", strColumnName, strTable, strColumnType)
                                    End If
                                End If
                            End If
                        End If
                    Next

                Catch ex As Exception
                    Console.WriteLine("Read: error when reading {0}: {1}", strTable, ex.Message)
                    bSucces = False
                End Try

                Me.m_db.ReleaseReader(reader)
                reader = Nothing

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
        Private Function OOPReadListItems(list As cOOPStorableList) As Boolean

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
            reader = Me.m_db.GetReader(strSQL)

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
        Private Function OOPWriteListItems(list As cOOPStorableList) As Boolean

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
            ds = Me.m_db.GetDataSet(adapter, strTable)
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

            Me.m_db.CommitDataSet(ds, adapter, strTable)

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
        Private Function OOPWriteObjectRecursive(t As Type, obj As cOOPStorable, piKey As PropertyInfo) As Boolean

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

                    ds = Me.m_db.GetDataSet(adapter, Me.OOPGetTableName(t))
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

                Me.m_db.CommitDataSet(ds, adapter, strTable)

                Me.m_db.ReleaseDataSet(ds)
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

    End Class
End Namespace