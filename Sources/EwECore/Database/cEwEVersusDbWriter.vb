' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
'
' Phase 1 role: keep the Access (OleDb/DataAdapter) database as the single
' source of truth that all existing call sites read and write exactly as
' before, while transparently mirroring every row Add/Change/Delete into the
' Entity Framework / SQLite side so the two databases stay in sync and can be
' validated against each other. No call site changes: GetDataTable() still
' returns a plain DataTable, dt.Rows.Find/BeginEdit/EndEdit all work unchanged.
' The EF side is purely a passenger here - Phase 2 is what makes it the driver.

Imports System.Data
Imports System.Linq
Imports EwEUtils.NetUtilities
Imports Microsoft.Extensions.Logging

Namespace Database

    Public Class cEwEVersusDbWriter
        Implements IEwEDbWriter

        Private ReadOnly m_accessWriter As IEwEDbWriter
        Private ReadOnly m_efWriter As cEwEEFDbWriter
        Private ReadOnly m_strTable As String
        Private ReadOnly m_getEfPropTypes As Func(Of String, Dictionary(Of String, Type))
        Private ReadOnly m_logger As ILogger
        Private m_bDisposed As Boolean = False

        Public Property RefCount As Integer Implements IEwEDbWriter.RefCount

        ''' ---------------------------------------------------------------
        ''' <summary>
        ''' Constructor. Both writers are expected to already be connected
        ''' (e.g. via db.GetWriter(strTable) on each side) - this class does
        ''' not connect anything itself, it only wires the two together.
        ''' </summary>
        ''' <param name="accessWriter">The primary (Access) writer. Its DataTable
        ''' remains the one and only thing callers interact with.</param>
        ''' <param name="efWriter">The secondary (EF/SQLite) writer, mirrored automatically.</param>
        ''' <param name="strTable">Table name, for diagnostics.</param>
        ''' <param name="getEfPropTypes">Optional callback returning expected CLR types per
        ''' column for the EF side, used to coerce values before comparing (same pattern as
        ''' cEwEVersusDatabase.GetReader / cCoercedDataReader). Pass Nothing to skip coercion.</param>
        ''' <param name="logger">Optional logger for diagnostics.</param>
        ''' ---------------------------------------------------------------
        Public Sub New(accessWriter As IEwEDbWriter, efWriter As cEwEEFDbWriter, strTable As String,
                       Optional getEfPropTypes As Func(Of String, Dictionary(Of String, Type)) = Nothing,
                       Optional logger As ILogger = Nothing)

            Me.m_accessWriter = accessWriter
            Me.m_efWriter = efWriter
            Me.m_strTable = strTable
            Me.m_getEfPropTypes = getEfPropTypes
            Me.m_logger = logger

            AddHandler Me.m_accessWriter.GetDataTable().RowChanged, AddressOf OnAccessRowChanged
            AddHandler Me.m_accessWriter.GetDataTable().RowDeleting, AddressOf OnAccessRowDeleting

        End Sub

#Region " Mirroring: Access DataTable events -> EF DataTable "

        Private Sub OnAccessRowChanged(sender As Object, e As DataRowChangeEventArgs)

            If Not Me.m_efWriter.IsConnected() Then Return
            Dim efTable As DataTable = Me.m_efWriter.GetDataTable()

            Try
                Select Case e.Action
                    Case DataRowAction.Add
                        Dim newRow As DataRow = efTable.NewRow()
                        Me.CopyMatchingColumns(e.Row, newRow, DataRowVersion.Current)
                        efTable.Rows.Add(newRow)

                    Case DataRowAction.Change
                        Dim efRow As DataRow = Me.FindEfRow(e.Row, DataRowVersion.Current)
                        If efRow Is Nothing Then
                            ' Row exists on the Access side but not yet mirrored (e.g. edited
                            ' right after being added in the same batch) - add it instead.
                            efRow = efTable.NewRow()
                            Me.CopyMatchingColumns(e.Row, efRow, DataRowVersion.Current)
                            efTable.Rows.Add(efRow)
                        Else
                            efRow.BeginEdit()
                            Me.CopyMatchingColumns(e.Row, efRow, DataRowVersion.Current)
                            efRow.EndEdit()
                        End If
                End Select
            Catch ex As Exception
                Me.m_logger?.LogError(ex, "cEwEVersusDbWriter({0}): failed to mirror row change to EF side", Me.m_strTable)
            End Try

        End Sub

        Private Sub OnAccessRowDeleting(sender As Object, e As DataRowChangeEventArgs)

            If Not Me.m_efWriter.IsConnected() Then Return

            Try
                Dim efRow As DataRow = Me.FindEfRow(e.Row, DataRowVersion.Original)
                efRow?.Delete()
            Catch ex As Exception
                Me.m_logger?.LogError(ex, "cEwEVersusDbWriter({0}): failed to mirror row delete to EF side", Me.m_strTable)
            End Try

        End Sub

        Private Function FindEfRow(accessRow As DataRow, version As DataRowVersion) As DataRow
            Dim efTable As DataTable = Me.m_efWriter.GetDataTable()
            If efTable.PrimaryKey Is Nothing OrElse efTable.PrimaryKey.Length = 0 Then Return Nothing
            Dim keys = efTable.PrimaryKey.Select(Function(c) accessRow(c.ColumnName, version)).ToArray()
            Return efTable.Rows.Find(keys)
        End Function

        Private Sub CopyMatchingColumns(source As DataRow, target As DataRow, version As DataRowVersion)
            For Each col As DataColumn In source.Table.Columns
                If target.Table.Columns.Contains(col.ColumnName) Then
                    target(col.ColumnName) = source(col.ColumnName, version)
                End If
            Next
        End Sub

#End Region

#Region " IEwEDbWriter "

        Public Function GetDataTable() As DataTable Implements IEwEDbWriter.GetDataTable
            Return Me.m_accessWriter.GetDataTable()
        End Function

        Public Function NewRow() As DataRow Implements IEwEDbWriter.NewRow
            Return Me.m_accessWriter.NewRow()
        End Function

        Public Sub AddRow(drow As DataRow) Implements IEwEDbWriter.AddRow
            Me.m_accessWriter.AddRow(drow)
        End Sub

        Public Function RemoveRow(drow As DataRow) As Boolean Implements IEwEDbWriter.RemoveRow
            Return Me.m_accessWriter.RemoveRow(drow)
        End Function

        Public Function GetRow(nRow As Integer) As DataRow Implements IEwEDbWriter.GetRow
            Return Me.m_accessWriter.GetRow(nRow)
        End Function

        Public Function GetTableName() As String Implements IEwEDbWriter.GetTableName
            Return Me.m_strTable
        End Function

        Public Function IsConnected() As Boolean Implements IEwEDbWriter.IsConnected
            Return Me.m_accessWriter.IsConnected() AndAlso Me.m_efWriter.IsConnected()
        End Function

        Public Function IsDisposed() As Boolean Implements IEwEDbWriter.IsDisposed
            Return Me.m_bDisposed
        End Function

        ''' ---------------------------------------------------------------
        ''' <summary>
        ''' Commits both writers, then compares the two DataTables row by row
        ''' using the same DataReaderDiff comparison the read-side versus
        ''' reader uses, and logs any mismatches. Both tables are already
        ''' in sync in memory (via the row-event mirroring above), so this
        ''' is a re-query-free sanity check, not a re-fetch.
        ''' </summary>
        ''' ---------------------------------------------------------------
        Public Function Commit() As Boolean Implements IEwEDbWriter.Commit

            Dim bAccess As Boolean = Me.m_accessWriter.Commit()
            Dim bEF As Boolean = Me.m_efWriter.Commit()

            Me.CompareCommittedTables()

            Return bAccess AndAlso bEF

        End Function

        Private Sub CompareCommittedTables()

            Try
                Dim accessReader As IDataReader = Me.m_accessWriter.GetDataTable().CreateDataReader()
                Dim efReader As IDataReader = Me.m_efWriter.GetDataTable().CreateDataReader()

                If Me.m_getEfPropTypes IsNot Nothing Then
                    Dim coerced As New cCoercedDataReader(efReader)
                    coerced.PropTypes = Me.m_getEfPropTypes(Me.m_strTable)
                    efReader = coerced
                End If

                Dim rowDiffs As New List(Of DataReaderDiff.RowDiff)()
                Dim iRow As Integer = 0

                While accessReader.Read()
                    efReader.Read()
                    iRow += 1
                    rowDiffs.AddRange(DataReaderDiff.CompareCurrentRow(accessReader, efReader))
                End While

                If rowDiffs.Any() Then
                    DataReaderDiff.BroadcastDiffs(accessReader, efReader, rowDiffs, iRow)
                End If

            Catch ex As Exception
                Me.m_logger?.LogError(ex, "cEwEVersusDbWriter({0}): post-commit comparison failed", Me.m_strTable)
            End Try

        End Sub

        Public Function Disconnect(Optional bSaveChanges As Boolean = True) As Boolean Implements IEwEDbWriter.Disconnect
            Dim bAccess As Boolean = Me.m_accessWriter.Disconnect(bSaveChanges)
            Dim bEF As Boolean = Me.m_efWriter.Disconnect(bSaveChanges)
            Return bAccess AndAlso bEF
        End Function

        Public Sub Dispose() Implements IDisposable.Dispose
            If Me.m_bDisposed Then Return
            Me.m_bDisposed = True

            RemoveHandler Me.m_accessWriter.GetDataTable().RowChanged, AddressOf OnAccessRowChanged
            RemoveHandler Me.m_accessWriter.GetDataTable().RowDeleting, AddressOf OnAccessRowDeleting

            Me.m_accessWriter.Dispose()
            Me.m_efWriter.Dispose()
        End Sub

#End Region

    End Class

End Namespace
