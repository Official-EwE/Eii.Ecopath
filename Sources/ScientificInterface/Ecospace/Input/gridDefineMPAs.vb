' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports SourceGrid2
Imports SharedResources = ScientificInterfaceShared.My.Resources

Namespace Ecospace

    ''' <summary>
    ''' Grid to create, rename and delete MPAs
    ''' </summary>

    Public Class gridEditMPA
        Inherits cEwEGrid

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' The engine taking care of all the nasty bits.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Class cDefineMPAsEngine
            Inherits cDefineItemsEngine

            Private Shared MONTHS(cCore.N_MONTHS) As Boolean

            Public Sub New(core As cCore)
                MyBase.New(core, eCoreCounterTypes.nMPAs, SharedResources.DEFAULT_NEWMPA_NUM)
            End Sub

            Protected Overrides Function GetCoreItem(iIndex As Integer) As cCoreInputOutputBase
                Return Me.Core.EcospaceMPAs(iIndex)
            End Function

            Protected Overrides Function CreateCoreItem(item As cItemInfo, iIndex As Integer, ByRef iDBID As Integer) As Boolean
                Return Me.Core.AddEcospaceMPA(item.Name, iIndex, MONTHS, iDBID)
            End Function

            Protected Overrides Function MoveCoreItem(item As cItemInfo, iIndex As Integer) As Boolean
                Return Me.Core.MoveEcospaceMPA(item.DBID, iIndex)
            End Function

            Protected Overrides Function DeleteCoreItem(item As cItemInfo) As Boolean
                Return Me.Core.RemoveEcospaceMPA(item.DBID)
            End Function

        End Class

        ''' <summary>A number representing the row that contains the first MPA</summary>
        Private Const iFIRSTMPAROW As Integer = 1

        ''' <summary>Update lock, used to distinguish between code updates and
        ''' user updates of grid cells. When grid cells are updated from within
        ''' the code, an update lock should be active to prevent edit/update recursion.</summary>
        Private m_iUpdateLock As Integer = 0

        Private m_engine As cDefineMPAsEngine = Nothing

        ''' <summary>Enumerated type defining the columns in this grid.</summary>
        Private Enum eColumnTypes
            Index = 0
            Name
            Status
        End Enum

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Create the grid
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Sub New()
            MyBase.New()
            Me.FixedColumnWidths = False
        End Sub

        Public Overrides ReadOnly Property SuppressQuickEdits As Boolean
            Get
                Return True
            End Get
        End Property

#Region " Grid interaction "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Initialize the grid.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Protected Overrides Sub InitStyle()

            MyBase.InitStyle()

            Me.ContextMenu = Nothing

            ' Redim columns
            Me.Redim(1, System.Enum.GetValues(GetType(eColumnTypes)).Length)

            Me(0, eColumnTypes.Index) = New cEwEColumnHeaderCell()
            Me(0, eColumnTypes.Name) = New cEwEColumnHeaderCell(SharedResources.HEADER_MPA)
            Me(0, eColumnTypes.Status) = New cEwEColumnHeaderCell(SharedResources.HEADER_STATUS)

            ' Fix index column only; MPA name column cannot be fixed because it must be editable
            Me.FixedColumns = 1

            Me.Columns(eColumnTypes.Index).AutoSizeMode = SourceGrid2.AutoSizeMode.None
            Me.Columns(eColumnTypes.Name).AutoSizeMode = SourceGrid2.AutoSizeMode.EnableStretch
            Me.Columns(eColumnTypes.Status).AutoSizeMode = SourceGrid2.AutoSizeMode.EnableAutoSize
            Me.AutoStretchColumnsToFitWidth = True

            Me.Selection.EnableMultiSelection = True
            Me.Selection.SelectionMode = GridSelectionMode.Row

        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Overridden to first create a snapshot of the MPA/stanza configuration
        ''' in the current EwE model. The grid will be populated from this local
        ''' administration.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Protected Overrides Sub FillData()

            If (Me.UIContext Is Nothing) Then Return

            If (Me.m_engine Is Nothing) Then
                Me.m_engine = New cDefineMPAsEngine(Me.Core)
            End If

            ' Brute-force update grid
            Me.UpdateGrid()

        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Brute-force resize the gird if necessary, and repopulate with data from 
        ''' the local administration.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Sub UpdateGrid()

            Dim item As cItemInfo = Nothing
            Dim ewec As cEwECell = Nothing

            ' Create missing rows
            For iRow As Integer = Me.Rows.Count To Me.m_engine.Items.Count
                Me.AddRow()

                ewec = New cEwECell(0, GetType(Integer))
                ewec.Style = cStyleGuide.eStyleFlags.Names Or cStyleGuide.eStyleFlags.NotEditable
                Me(iRow, eColumnTypes.Index) = ewec

                Me(iRow, eColumnTypes.Name) = New Cells.Real.Cell("", GetType(String))
                Me(iRow, eColumnTypes.Name).Behaviors.Add(Me.EwEEditHandler)

                Me(iRow, eColumnTypes.Status) = New cEwEStatusCell(eItemStatusTypes.Original)
            Next

            ' Delete obsolete rows
            While Me.Rows.Count > Me.m_engine.Items.Count + 1
                Me.Rows.Remove(Me.Rows.Count - iFIRSTMPAROW)
            End While

            ' Sanity check whether grid can accomodate all MPAs + header
            Debug.Assert(Me.Rows.Count = Me.m_engine.Items.Count + 1)

            ' Populate rows
            For iRow As Integer = 1 To Me.m_engine.Items.Count
                Me.UpdateRow(iRow)
            Next iRow

            Me.StretchColumnsToFitWidth()

        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Refresh the content of the Row with the given index.
        ''' </summary>
        ''' <param name="iRow">The index of the row to refresh.</param>
        ''' -----------------------------------------------------------------------
        Private Sub UpdateRow(iRow As Integer)

            Dim item As cItemInfo = Me.m_engine.Items(iRow - iFIRSTMPAROW)

            Me.AllowUpdates = False

            Me(iRow, eColumnTypes.Index).Value = CStr(iRow)
            Me(iRow, eColumnTypes.Name).Value = item.Name
            Me(iRow, eColumnTypes.Status).Value = item.Status

            Me.AllowUpdates = True

        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Called when the user has finished editing a cell. Handled to update 
        ''' local admin based on cell value changes.
        ''' </summary>
        ''' <returns>
        ''' True if the edit operation is allowed, False to cancel the edit operation.
        ''' </returns>
        ''' <remarks>
        ''' This method differs from OnCellValueChanged; at the end of an edit
        ''' operation it is once again safe to alter the value of the cell that was
        ''' just edited for text and combo box controls. *sigh*
        ''' </remarks>
        ''' -----------------------------------------------------------------------
        Protected Overrides Function OnCellEdited(p As Position, cell As Cells.ICellVirtual) As Boolean

            If Not Me.AllowUpdates Then Return True

            Dim item As cItemInfo = Me.m_engine.Items(p.Row - iFIRSTMPAROW)

            Select Case DirectCast(p.Column, eColumnTypes)

                Case eColumnTypes.Name
                    Dim strName As String = CStr(cell.GetValue(p))
                    If Not Me.m_engine.IsNameUnique(strName, Nothing) Then
                        ' Change is not allowed
                        Me.UpdateRow(p.Row)
                        ' Report failure
                        Return False
                    End If
                    ' Allow name change
                    item.Name = strName

                Case Else
                    Return False

            End Select

            Return True

        End Function

#End Region ' Grid interaction

#Region " Row manipulation "

        ' ToDo: allow multi-select row movement

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Toggle delete status for all selected rows
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Sub ToggleDeleteRow()

            For Each iRow As Integer In Me.SelectedRows
                Dim iMPA As Integer = iRow - iFIRSTMPAROW
                Me.m_engine.ToggleDeleteItem(Me.m_engine.Items(iMPA))
            Next
            Me.UpdateGrid()

        End Sub

        ''' <summary>
        ''' States whether a row holds a MPA.
        ''' </summary>
        ''' <param name="iRow"></param>
        ''' <returns></returns>
        Public Function IsMPARow(Optional iRow As Integer = -1) As Boolean
            If iRow = -1 Then iRow = Me.SelectedRow()
            Return (iRow >= iFIRSTMPAROW) And (iRow < Me.RowsCount)
        End Function

        ''' <summary>
        ''' States whether the MPA on a row is flagged for deletion.
        ''' </summary>
        Public Function IsFlaggedForDeletionRow(Optional iRow As Integer = -1) As Boolean
            If iRow = -1 Then iRow = Me.SelectedRow()
            If Not Me.IsMPARow(iRow) Then Return False

            Dim iMPA As Integer = iRow - iFIRSTMPAROW
            Dim item As cItemInfo = Me.m_engine.Items(iMPA)
            Return item.FlaggedForDeletion

        End Function

        ''' <summary>
        ''' Add a row by creating a new MPA.
        ''' </summary>
        Public Sub InsertRow()
            If Not Me.CanAddRow() Then Return
            Me.CreateMPA()
        End Sub

        ''' <summary>
        ''' Create a new MPA.
        ''' </summary>
        Private Sub CreateMPA()

            Dim info As cItemInfo = Me.m_engine.AddItem(-1)

            Me.UpdateGrid()
            Me.SelectRow(info)

        End Sub

        ''' <summary>
        ''' States whether a row can be inserted at the indicated position.
        ''' </summary>
        Public Function CanAddRow() As Boolean
            Return True
        End Function

        ''' <summary>
        ''' Move row up, switching positions with the row above it.
        ''' </summary>
        Public Sub MoveRowsUp()

            Dim rows As New List(Of Integer)
            rows.AddRange(Me.SelectedRows())
            rows.Sort()

            If (rows.Count = 0) Then Return

            For i As Integer = 0 To rows.Count - 1
                Dim bCanMove As Boolean = Me.CanMoveRowUp(rows(i))
                If (i > 0) Then
                    bCanMove = bCanMove And (rows(i) - 1 > rows(i - 1))
                End If
                If (bCanMove) Then
                    Me.MoveRow(rows(i), rows(i) - 1)
                    rows(i) -= 1
                End If
            Next

            For i As Integer = 0 To rows.Count - 1
                Me.SelectRow(rows(i), i > 0)
            Next

        End Sub

        ''' <summary>
        ''' States whether row(s) can be moved up.
        ''' </summary>
        Public Function CanMoveRowUp(Optional iRow As Integer = -1) As Boolean
            If iRow = -1 Then
                Dim rows As New List(Of Integer)
                rows.AddRange(Me.SelectedRows())
                rows.Sort()
                For i As Integer = 0 To rows.Count - 1
                    Dim bCanMove As Boolean = Me.CanMoveRowUp(rows(i))
                    If (i > 0) Then
                        bCanMove = bCanMove And (rows(i) - 1 > rows(i - 1))
                    End If
                    If bCanMove Then Return True
                Next
                Return False
            End If
            Return (iRow > iFIRSTMPAROW)
        End Function

        ''' <summary>
        ''' Move row down, switching positions with the row below it.
        ''' </summary>
        Public Sub MoveRowsDown()
            Dim rows As New List(Of Integer)
            rows.AddRange(Me.SelectedRows())
            rows.Sort()

            If (rows.Count = 0) Then Return

            For i As Integer = rows.Count - 1 To 0 Step -1
                Dim bCanMove As Boolean = Me.CanMoveRowDown(rows(i))
                If (i < rows.Count - 1) Then
                    bCanMove = bCanMove And (rows(i) + 1 < rows(i + 1))
                End If
                If (bCanMove) Then
                    Me.MoveRow(rows(i), rows(i) + 1)
                    rows(i) += 1
                End If
            Next

            For i As Integer = 0 To rows.Count - 1
                Me.SelectRow(rows(i), i > 0)
            Next
        End Sub

        ''' <summary>
        ''' States whether row(s) can be moved down.
        ''' </summary>
        Public Function CanMoveRowDown(Optional iRow As Integer = -1) As Boolean
            If iRow = -1 Then
                Dim rows As New List(Of Integer)
                rows.AddRange(Me.SelectedRows())
                rows.Sort()
                For i As Integer = 0 To rows.Count - 1
                    Dim bCanMove As Boolean = Me.CanMoveRowDown(rows(i))
                    If (i < rows.Count - 1) Then
                        bCanMove = bCanMove And (rows(i) + 1 < rows(i + 1))
                    End If
                    If bCanMove Then Return True
                Next
                Return False
            End If
            Return (iRow < Me.RowsCount - 1)
        End Function

        ''' <summary>
        ''' Move one row to another position.
        ''' </summary>
        Private Sub MoveRow(iFromRow As Integer, iToRow As Integer)

            Dim item As cItemInfo = Me.m_engine.Items(iFromRow - iFIRSTMPAROW)
            Me.m_engine.MoveItem(item, iToRow - iFIRSTMPAROW)

            For i As Integer = Math.Min(iFromRow, iToRow) To Math.Max(iFromRow, iToRow)
                Me.UpdateRow(i)
            Next i

        End Sub

#End Region ' Row manipulation 

#Region " Admin "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Update lock, should be set when modifying cell values from the code
        ''' to prevent recursive update/notification loops.
        ''' </summary>
        ''' <returns>True when no update lock is active.</returns>
        ''' <remarks>
        ''' Update locks are cumulative: setting this lock twice will require 
        ''' clearing it twice to allow updates to happen.
        ''' </remarks>
        ''' -----------------------------------------------------------------------
        Private Property AllowUpdates() As Boolean
            Get
                Return (Me.m_iUpdateLock = 0)
            End Get
            Set(value As Boolean)
                If value Then
                    Me.m_iUpdateLock += 1
                Else
                    Me.m_iUpdateLock -= 1
                End If
            End Set
        End Property

#Region " Selection extension "

        Private Overloads Sub SelectRow(item As cItemInfo)
            For iMPA As Integer = 0 To Me.m_engine.Items.Count - 1
                If ReferenceEquals(Me.m_engine.Items(iMPA), item) Then
                    Me.SelectRow(iMPA + iFIRSTMPAROW)
                End If
            Next
        End Sub

#End Region ' Selection extension

#End Region ' Admin

#Region " Apply changes "

        Public Function Apply() As Boolean
            Return Me.m_engine.Apply()
        End Function

#End Region ' Apply changes

    End Class

End Namespace
