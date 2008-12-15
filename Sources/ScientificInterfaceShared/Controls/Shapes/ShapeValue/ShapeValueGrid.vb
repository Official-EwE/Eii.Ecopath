'==============================================================================
'
' $Log: ShapeValueGrid.vb,v $
' Revision 1.1  2008/12/15 15:36:39  jeroens
' Moved from ScInt
'
' Revision 1.1  2008/09/26 07:31:43  sherman
' --== DELETED HISTORY ==--
'
'==============================================================================

#Region " Imports "

Option Strict On
Imports EwECore
Imports ScientificInterfaceShared.Controls.EwEGrid
Imports ScientificInterfaceShared.Style

#End Region ' Imports

<CLSCompliant(False)> _
Public Class ShapeValueGrid
    Inherits EwEGrid

    Private m_iNumValues As Integer = 50
    Private m_bSuppressZeroes As Boolean = False
    Private m_shape As cShapeData = Nothing

    Public Sub SetValues(ByVal iNumValues As Integer, ByVal shape As cShapeData)
        Me.m_iNumValues = iNumValues
        Me.m_bSuppressZeroes = (TypeOf shape Is cTimeSeries)
        Me.m_shape = shape
        Me.InitLayout()
    End Sub

    Public Sub ApplyValues(Optional ByVal shape As cShapeData = Nothing)
        Dim cell As EwECell = Nothing
        Dim asNewValues() As Single
        Dim iValue As Integer = 1
        Dim iCell As Integer = 1
        Dim iNumValues As Integer = Me.m_iNumValues

        If (shape Is Nothing) Then shape = Me.m_shape

        If shape.IsSeasonal Then iNumValues = Me.m_shape.XMax
        ReDim asNewValues(iNumValues)

        For iValue = 1 To shape.XMax
            cell = DirectCast(Me(iCell, 1), EwECell)
            asNewValues(iValue) = CSng(cell.Value)
            iCell += 1
            If iCell > Me.m_iNumValues Then iCell = 1
        Next
        shape.ShapeData = asNewValues
    End Sub

    Public Sub SetEmpty(ByVal iNumValues As Integer, ByVal iFirstValueLabel As Integer, ByVal bSuppressZeroes As Boolean)
        Me.m_iNumValues = iNumValues
        Me.m_bSuppressZeroes = bSuppressZeroes
        Me.m_shape = Nothing
        Me.InitLayout()
    End Sub

    Protected Overrides Sub InitStyle()
        MyBase.InitStyle()

        Me.FixedColumns = 1

        Me.Redim(Me.m_iNumValues + 1, 2)
        Me(0, 0) = New EwEColumnHeaderCell(My.Resources.HEADER_TIME)
        Me(0, 1) = New EwEColumnHeaderCell(My.Resources.HEADER_VALUE)

    End Sub

    Protected Overrides Sub FillData()

        Dim core As cCore = cCore.GetInstance()
        Dim cell As EwECell = Nothing
        Dim sValue As Single = 0.0!
        Dim iStartIndex As Integer = core.EcosimFirstYear

        If iStartIndex > 0 Then iStartIndex -= 1

        For iValue As Integer = 1 To Me.m_iNumValues

            cell = New EwECell(iValue + iStartIndex, GetType(Integer))
            cell.Style = StyleGuide.eStyleFlags.NotEditable
            cell.SuppressZero = True
            Me(iValue, 0) = cell

            sValue = 0.0!

            If Me.m_shape IsNot Nothing Then
                If iValue <= Me.m_shape.XMax Then
                    sValue = Me.m_shape.ShapeData(iValue)
                End If
            End If

            cell = New EwECell(sValue, GetType(Single))
            cell.SuppressZero = Me.m_bSuppressZeroes
            Me(iValue, 1) = cell
        Next

    End Sub

    Protected Overrides Sub FinishStyle()
        MyBase.FinishStyle()

        Me.Columns(0).Width = 70
        'Me.Columns(1).Width = 70
    End Sub

    Protected Overrides Sub bm_colSelectClick(ByVal sender As Object, ByVal e As SourceGrid2.PositionEventArgs)
        ' Allow col selectionn
        MyBase.bm_colSelectClick(sender, e)
    End Sub

    Protected Overrides Sub bm_rowSelectClick(ByVal sender As Object, ByVal e As SourceGrid2.PositionEventArgs)
        ' Do not allow col selections
        'MyBase.bm_rowSelectClick(sender, e)
    End Sub

    Protected Overrides Sub bm_tlCellClick(ByVal sender As Object, ByVal e As SourceGrid2.PositionEventArgs)
        ' Do not allow entire grid selections
        'MyBase.bm_tlCellClick(sender, e)
    End Sub

End Class
