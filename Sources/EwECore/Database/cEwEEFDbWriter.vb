' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
'
' Phase 1 role: SQLite needs to work through the same DataTable/DataRow-shaped
' writer API the rest of the codebase already uses (dt.Rows.Find, BeginEdit/EndEdit,
' etc). Microsoft.Data.Sqlite does not implement DbDataAdapter/DbCommandBuilder, so
' instead of hand-writing a mapping per table, this class derives everything it
' needs (columns, CLR types, primary key) via plain reflection + DataAnnotations
' attributes ([Table], [Column], [Key]) - the SAME approach already used by
' EwEDbContext.GetEntityTypeByTableName / GetPropTypes. Deliberately avoids
' Microsoft.EntityFrameworkCore.Metadata (IEntityType/IProperty/IKey) and the
' EF-Core-only DbContext.Add/Find/Remove<T> conveniences, since EwECore builds
' against both net48 (EF6) and net10 (EF Core) - this class only relies on the
' handful of members common to both: DbContext.Set<T>()/SaveChanges(), and
' DbSet<T>.Add(objEntity)/Remove(objEntity) as plain instance methods.

Imports System.Collections
Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel.DataAnnotations.Schema
Imports System.Data
Imports System.Linq
Imports System.Reflection
Imports Eii.Ecopath.Storage
Imports Microsoft.Extensions.Logging

Namespace Database

    Public Class cEwEEFDbWriter
        Implements IEwEDbWriter

        Private m_ctx As EwEDbContext = Nothing
        Private m_entityType As Type = Nothing
        Private m_dbSet As Object = Nothing               ' the DbSet(Of TEntity) instance
        Private m_props As PropertyInfo() = Nothing        ' all mapped scalar properties, reflection order
        Private m_keyProps As PropertyInfo() = Nothing     ' subset decorated with <Key>, same order
        Private m_dt As DataTable = Nothing
        Private m_strTable As String = ""
        Private m_bDisposed As Boolean = False
        Private m_logger As ILogger = Nothing

        ''' <summary>Tracked entities keyed by a composite key string, so Commit()
        ''' never has to depend on EF's own (possibly differently-ordered) key definition.</summary>
        Private ReadOnly m_entityByKey As New Dictionary(Of String, Object)()

        Public Property RefCount As Integer Implements IEwEDbWriter.RefCount

        ''' ---------------------------------------------------------------
        ''' <summary>
        ''' Constructor. Attempts to connect and fill from the given
        ''' EwEDbContext. Check <see cref="IsConnected"/> after construction
        ''' to verify success - mirrors the existing cEwEDbWriter convention
        ''' of not throwing on a failed connect.
        ''' </summary>
        ''' ---------------------------------------------------------------
        Public Sub New(ctx As EwEDbContext, strTable As String, Optional logger As ILogger = Nothing)
            Me.m_logger = logger
            Me.Connect(ctx, strTable)
        End Sub

        Private Function Connect(ctx As EwEDbContext, strTable As String) As Boolean

            Try
                Me.m_ctx = ctx
                Me.m_strTable = strTable

                Me.m_entityType = ctx.GetEntityTypeByTableName(strTable)
                If Me.m_entityType Is Nothing Then
                    Me.m_logger?.LogError("cEwEEFDbWriter.Connect({0}): no EF entity type mapped to this table", strTable)
                    Me.m_ctx = Nothing
                    Return False
                End If

                Me.m_props = Me.m_entityType.GetProperties(BindingFlags.Instance Or BindingFlags.Public)
                Me.m_keyProps = Me.m_props.Where(Function(p) p.GetCustomAttribute(Of KeyAttribute)() IsNot Nothing).ToArray()
                If Me.m_keyProps.Length = 0 Then
                    Me.m_logger?.LogWarning("cEwEEFDbWriter.Connect({0}): entity has no [Key] properties; row edit/delete tracking will not work", strTable)
                End If

                Dim setMethod As MethodInfo = ctx.GetType().GetMethod("Set", Array.Empty(Of Type)())
                Me.m_dbSet = setMethod.MakeGenericMethod(Me.m_entityType).Invoke(ctx, Nothing)

                ' The DbContext is long-lived and shared across every writer/save for
                ' the lifetime of the app - if this table was ever read or saved
                ' earlier in the session, EF's ChangeTracker may still be holding
                ' those entities as Unchanged, with no idea that a raw SQL DELETE
                ' (bypassing EF entirely) may since have removed them from the
                ' database. Left in place, those stale entities collide with the
                ' brand-new instances Commit() creates for "Added" rows - EF doesn't
                ' catch this at Add()-time, only later inside SaveChanges() itself
                ' (after the real INSERT has already been sent to the database),
                ' which is why the failure looks like a duplicate-key/constraint
                ' error rather than an immediate, obvious in-memory conflict.
                Me.DetachTrackedEntitiesOfThisType()

                Me.m_dt = New DataTable(strTable)
                For Each prop As PropertyInfo In Me.m_props
                    Me.m_dt.Columns.Add(Me.ColumnName(prop), If(Nullable.GetUnderlyingType(prop.PropertyType), prop.PropertyType))
                Next
                If Me.m_keyProps.Length > 0 Then
                    Me.m_dt.PrimaryKey = Me.m_keyProps.Select(Function(p) Me.m_dt.Columns(Me.ColumnName(p))).ToArray()
                End If

                Me.Fill()
                Return True

            Catch ex As Exception
                Me.m_logger?.LogError(ex, "cEwEEFDbWriter.Connect({0})", strTable)
                Me.m_ctx = Nothing
                Me.m_dt = Nothing
                Return False
            End Try

        End Function

        ''' <summary>Column name for a property: [Column("X")] if present, else the property name - same rule as EwEDbContext.GetPropTypes.</summary>
        Private Function ColumnName(prop As PropertyInfo) As String
            Dim colAttr = prop.GetCustomAttribute(Of ColumnAttribute)()
            Return If(colAttr?.Name, prop.Name)
        End Function

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Detaches any entities of this writer's entity type that are currently
        ''' tracked by the DbContext, without touching entities of any other type.
        ''' See the call site in Connect() for why this is necessary.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Private Sub DetachTrackedEntitiesOfThisType()
#If NET48 Then
            ' EF6: DbContext.ChangeTracker.Entries() returns DbEntityEntry, with a
            ' settable .State of type System.Data.Entity.EntityState.
            Dim entries = Me.m_ctx.ChangeTracker.Entries().
                Where(Function(e) e.Entity.GetType() Is Me.m_entityType).ToList()
            For Each entry In entries
                entry.State = System.Data.Entity.EntityState.Detached
            Next
#Else
            ' EF Core: DbContext.ChangeTracker.Entries() returns EntityEntry, with a
            ' settable .State of type Microsoft.EntityFrameworkCore.EntityState.
            Dim entries = Me.m_ctx.ChangeTracker.Entries().
                Where(Function(e) e.Entity.GetType() Is Me.m_entityType).ToList()
            For Each entry In entries
                entry.State = Microsoft.EntityFrameworkCore.EntityState.Detached
            Next
#End If
        End Sub

        Private Function KeyStringForEntity(objEntity As Object) As String
            Return String.Join("|", Me.m_keyProps.Select(Function(p) Convert.ToString(p.GetValue(objEntity))))
        End Function

        Private Function KeyStringForRow(drow As DataRow, version As DataRowVersion) As String
            Return String.Join("|", Me.m_keyProps.Select(Function(p) Convert.ToString(drow(Me.ColumnName(p), version))))
        End Function

        Private Sub Fill()

            Me.m_entityByKey.Clear()
            Dim dbSetEnum As IEnumerable = CType(Me.m_dbSet, IEnumerable)

            For Each objEntity As Object In dbSetEnum
                Dim drow As DataRow = Me.m_dt.NewRow()
                For Each prop As PropertyInfo In Me.m_props
                    Dim objValue As Object = prop.GetValue(objEntity)
                    drow(Me.ColumnName(prop)) = If(objValue, DBNull.Value)
                Next
                Me.m_dt.Rows.Add(drow)

                If Me.m_keyProps.Length > 0 Then
                    Me.m_entityByKey(Me.KeyStringForEntity(objEntity)) = objEntity
                End If
            Next
            Me.m_dt.AcceptChanges()

        End Sub

        Public Function NewRow() As DataRow Implements IEwEDbWriter.NewRow
            Return Me.m_dt.NewRow()
        End Function

        Public Sub AddRow(drow As DataRow) Implements IEwEDbWriter.AddRow
            Me.m_dt.Rows.Add(drow)
        End Sub

        Public Function RemoveRow(drow As DataRow) As Boolean Implements IEwEDbWriter.RemoveRow
            Me.m_dt.Rows.Remove(drow)
            Return True
        End Function

        Public Function GetRow(nRow As Integer) As DataRow Implements IEwEDbWriter.GetRow
            Return Me.m_dt.Rows(nRow)
        End Function

        Public Function GetDataTable() As DataTable Implements IEwEDbWriter.GetDataTable
            Return Me.m_dt
        End Function

        Public Function GetTableName() As String Implements IEwEDbWriter.GetTableName
            Return Me.m_strTable
        End Function

        Public Function IsConnected() As Boolean Implements IEwEDbWriter.IsConnected
            Return Me.m_ctx IsNot Nothing AndAlso Me.m_dt IsNot Nothing
        End Function

        Public Function IsDisposed() As Boolean Implements IEwEDbWriter.IsDisposed
            Return Me.m_bDisposed
        End Function

        ''' ---------------------------------------------------------------
        ''' <summary>
        ''' Commits pending changes. Added rows become new entities passed to
        ''' DbSet.Add(); Modified/Deleted rows are matched to already-tracked
        ''' entity instances via the internal key dictionary (never via EF's
        ''' own Find(), to avoid depending on its composite-key ordering).
        ''' </summary>
        ''' ---------------------------------------------------------------
        Public Function Commit() As Boolean Implements IEwEDbWriter.Commit

            If Not Me.IsConnected() Then Return False
            If Me.m_dt.GetChanges() Is Nothing Then Return True

            Try
                Dim dbSetType As Type = Me.m_dbSet.GetType()
                Dim addMethod As MethodInfo = dbSetType.GetMethod("Add", New Type() {Me.m_entityType})
                Dim removeMethod As MethodInfo = dbSetType.GetMethod("Remove", New Type() {Me.m_entityType})

                For Each drow As DataRow In Me.m_dt.Select(Nothing, Nothing, DataViewRowState.Added)
                    Dim objEntity = Activator.CreateInstance(Me.m_entityType)
                    For Each prop As PropertyInfo In Me.m_props
                        Dim objValue = drow(Me.ColumnName(prop))
                        prop.SetValue(objEntity, If(objValue Is DBNull.Value, Nothing, objValue))
                    Next
                    addMethod.Invoke(Me.m_dbSet, New Object() {objEntity})
                    If Me.m_keyProps.Length > 0 Then Me.m_entityByKey(Me.KeyStringForEntity(objEntity)) = objEntity
                Next

                For Each drow As DataRow In Me.m_dt.Select(Nothing, Nothing, DataViewRowState.ModifiedCurrent)
                    If Me.m_keyProps.Length = 0 Then Continue For
                    Dim strKey = Me.KeyStringForRow(drow, DataRowVersion.Original)
                    Dim objEntity As Object = Nothing
                    If Me.m_entityByKey.TryGetValue(strKey, objEntity) Then
                        For Each prop As PropertyInfo In Me.m_props
                            Dim objValue = drow(Me.ColumnName(prop))
                            prop.SetValue(objEntity, If(objValue Is DBNull.Value, Nothing, objValue))
                        Next
                        ' Key value(s) may have changed as part of the edit - re-index
                        Me.m_entityByKey.Remove(strKey)
                        Me.m_entityByKey(Me.KeyStringForEntity(objEntity)) = objEntity
                    Else
                        Me.m_logger?.LogWarning("cEwEEFDbWriter.Commit({0}): no tracked entity found for modified row (key={1})", Me.m_strTable, strKey)
                    End If
                Next

                For Each drow As DataRow In Me.m_dt.Select(Nothing, Nothing, DataViewRowState.Deleted)
                    If Me.m_keyProps.Length = 0 Then Continue For
                    Dim strKey = Me.KeyStringForRow(drow, DataRowVersion.Original)
                    Dim objEntity As Object = Nothing
                    If Me.m_entityByKey.TryGetValue(strKey, objEntity) Then
                        removeMethod.Invoke(Me.m_dbSet, New Object() {objEntity})
                        Me.m_entityByKey.Remove(strKey)
                    Else
                        Me.m_logger?.LogWarning("cEwEEFDbWriter.Commit({0}): no tracked entity found for deleted row (key={1})", Me.m_strTable, strKey)
                    End If
                Next

                Me.m_ctx.SaveChanges()
                Me.m_dt.AcceptChanges()
                Return True

            Catch ex As Exception
                Me.m_logger?.LogError(ex, "cEwEEFDbWriter.Commit({0})", Me.m_strTable)
                Return False
            End Try

        End Function

        Public Function Disconnect(Optional bSaveChanges As Boolean = True) As Boolean Implements IEwEDbWriter.Disconnect

            If Not Me.IsConnected() Then Return False

            Dim bSucces As Boolean = True
            If bSaveChanges Then bSucces = Me.Commit()

            Me.m_dt = Nothing
            Me.m_ctx = Nothing
            Me.m_strTable = ""
            Me.m_entityByKey.Clear()
            Return bSucces

        End Function

        Public Sub Dispose() Implements IDisposable.Dispose
            If Not Me.m_bDisposed Then
                Me.m_bDisposed = True
                If Me.IsConnected() Then Me.Disconnect(True)
            End If
            GC.SuppressFinalize(Me)
        End Sub

    End Class

End Namespace