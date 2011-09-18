#Region " Imports "

Option Strict On
Imports EwECore
Imports EwEUtils.Core
Imports EwEUtils.Utilities
Imports ScientificInterfaceShared.Controls
Imports ScientificInterfaceShared.Controls.EwEGrid
Imports ScientificInterfaceShared.Style
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Base grid class for showing <see cref="cForcingFunction">forcing function</see>-derived
''' shapes.
''' </summary>
''' ---------------------------------------------------------------------------
Public MustInherit Class gridForcingBase
    Inherits gridShapeBase

    ''' <summary>Rows in the grid</summary>
    Private Enum eRowType As Integer
        Header = 0
        Thumbnail
        Name
        FirstTime
    End Enum

    Public Sub New()
        MyBase.New()
    End Sub

#Region " Grid overrides "

    Protected Overrides Sub FillData()

        If Me.UIContext Is Nothing Then Return

        Dim ats As cShapeData() = Me.Shapes
        Dim iNumShapes As Integer = ats.Length
        Dim iNumPoints As Integer = 0
        Dim iNumHeaders As Integer = 0
        Dim cell As SourceGrid2.Cells.ICell = Nothing

        If Me.IsSeasonal Then
            iNumPoints = cCore.N_MONTHS
        Else
            iNumPoints = Me.Core.nEcosimTimeSteps
            For Each s As cShapeData In ats
                iNumPoints = Math.Max(iNumPoints, s.XMax)
            Next
        End If
        Me.Redim(iNumPoints + [Enum].GetValues(GetType(eRowType)).Length, iNumShapes + 1)

        cApplicationStatusNotifier.StartProgress(Me.UIContext.Core, SharedResources.STATUS_UPDATING)

        ' Create row headers
        Me(eRowType.Header, 0) = New EwEColumnHeaderCell(SharedResources.HEADER_INDEX)
        Me(eRowType.Thumbnail, 0) = New EwERowHeaderCell(SharedResources.HEADER_IMAGE)
        Me(eRowType.Name, 0) = New EwERowHeaderCell(SharedResources.HEADER_NAME)

        ' Create row header cells
        For i As Integer = 0 To iNumPoints - 1
            If Me.IsSeasonal Then
                Me(eRowType.FirstTime + i, 0) = New EwERowHeaderCell(cDateUtils.GetMonthName(i + 1))
            Else
                Me(eRowType.FirstTime + i, 0) = New EwERowHeaderCell(CStr(Me.Core.EcosimFirstYear + i))
            End If
        Next

        ' Populate shape columns
        For i As Integer = 0 To iNumShapes - 1

            Me.Shape(i + 1) = ats(i)

            Me(eRowType.Header, i + 1) = New EwEColumnHeaderCell(CStr(ats(i).Index))

            cell = New SourceGrid2.Cells.Real.Cell
            cell.Value = ats(i)
            cell.VisualModel = New cVisualModelThumbnail(Me.Handler)
            Me(eRowType.Thumbnail, i + 1) = cell

            cell = New EwECell(ats(i).Name, GetType(String))
            cell.Behaviors.Add(Me.EwEEditHandler)
            Me(eRowType.Name, i + 1) = cell

            For j As Integer = 0 To Math.Min(iNumPoints, ats(i).XMax) - 1
                cell = New EwECell(ats(i).ShapeData(j + 1), GetType(Single))
                cell.Behaviors.Add(Me.EwEEditHandler)
                Me(eRowType.FirstTime + j, i + 1) = cell
            Next
            For j As Integer = ats(i).XMax To iNumPoints - 1
                cell = New EwECell(0, GetType(Integer), cStyleGuide.eStyleFlags.NotEditable Or cStyleGuide.eStyleFlags.Null)
                Me(eRowType.FirstTime + j, i + 1) = cell
            Next
        Next

        cApplicationStatusNotifier.EndProgress(Me.UIContext.Core)

    End Sub

    Protected Overrides Sub FinishStyle()
        MyBase.FinishStyle()
        Me.Rows(eRowType.Thumbnail).Height = 48
        For i As Integer = 1 To Me.ColumnsCount - 1
            Me.Columns(i).Width = Math.Max(Me.Columns(i).Width, 48)
        Next
        ' Fix rows up to (not including) name, because name needs to be editable. Fixed cells cannot be editable
        Me.FixedRows = eRowType.Name
        ' Fix header column
        Me.FixedColumns = 1
    End Sub

#End Region ' Grid overrides

#Region " Edits "

    Dim m_bInLocalEdit As Boolean = False

    Protected Overrides Function OnCellEdited(ByVal p As SourceGrid2.Position, _
                                              ByVal cell As SourceGrid2.Cells.ICellVirtual) As Boolean

        Dim shape As cShapeData = Me.Shape(p.Column)

        Me.m_bInLocalEdit = True
        If (Me.IsInBatchEdit) Then shape.LockUpdates()

        Select Case DirectCast(p.Row, eRowType)
            Case eRowType.Name
                shape.Name = CStr(cell.GetValue(p))
            Case Else
                Dim iTime As Integer = p.Row - eRowType.FirstTime
                shape.ShapeData(iTime) = CSng(cell.GetValue(p))
        End Select

        If (Me.IsInBatchEdit) Then
            shape.UnlockUpdates(False)
            Me.InvalidateShape(shape)
        End If
        Me.m_bInLocalEdit = False

        Return MyBase.OnCellEdited(p, cell)
    End Function

    Protected Overrides Function OnCellValueChanged(ByVal p As SourceGrid2.Position, _
                                                    ByVal cell As SourceGrid2.Cells.ICellVirtual) As Boolean
        Me.OnCellEdited(p, cell)
        Return MyBase.OnCellValueChanged(p, cell)
    End Function

#End Region ' Edits

#Region " Updates "

    Protected Overrides Sub OnRefreshed(ByVal sender As cShapeGUIHandler)

        If Me.IsInBatchEdit Then
            Return
        End If

        ' Unpleasant: a refresh can be triggered from an external edit or by 
        ' this very interface in response to a cell edit. If a cell edit is in
        ' progress the grid content cannot be refreshed.

        ' In local cell edit?
        If Me.m_bInLocalEdit Then
            ' #Yes: just invalidate the thumbnail
            Me.InvalidateRange(New SourceGrid2.Range(eRowType.Thumbnail, 0, eRowType.Thumbnail, Me.ColumnsCount - 1))
        Else
            ' #No: refresh the whole lot
            Me.RefreshContent()
        End If

    End Sub

#End Region ' Updates

End Class
