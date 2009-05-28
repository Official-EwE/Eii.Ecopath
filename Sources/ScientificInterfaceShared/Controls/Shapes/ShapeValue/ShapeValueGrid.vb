'==============================================================================
'
' $Log: ShapeValueGrid.vb,v $
' Revision 1.6  2009/05/28 12:37:47  jeroens
' Properly named utility classes StyleGuide and ZedGraphHelper
'
' Revision 1.5  2009/05/02 19:01:54  jeroens
' Fixed index
'
' Revision 1.4  2009/03/23 20:21:31  jeroens
' Fixed issue 599
'
' Revision 1.3  2009/03/11 18:26:10  jeroens
' Added Year mode (for time series)
'
' Revision 1.2  2009/03/11 00:31:18  jeroens
' Able to show year+month/indexed columns
'
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
    Private m_displayMode As frmShapeValue.eDisplayMode = frmShapeValue.eDisplayMode.Monthly

    Public Sub SetValues(ByVal iNumValues As Integer, ByVal shape As cShapeData, ByVal displayMode As frmShapeValue.eDisplayMode)

        Me.m_iNumValues = iNumValues
        Me.m_bSuppressZeroes = (TypeOf shape Is cTimeSeries)
        Me.m_shape = shape
        Me.m_displayMode = displayMode

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
            Select Case Me.m_displayMode

                Case frmShapeValue.eDisplayMode.Index, _
                     frmShapeValue.eDisplayMode.Yearly
                    cell = DirectCast(Me(iCell, 1), EwECell)

                Case frmShapeValue.eDisplayMode.Monthly
                    cell = DirectCast(Me(iCell, 2), EwECell)

            End Select

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

        Select Case Me.m_displayMode

            Case frmShapeValue.eDisplayMode.Index
                Me.Redim(Me.m_iNumValues + 1, 2)
                Me(0, 0) = New EwEColumnHeaderCell(My.Resources.HEADER_INDEX)
                Me(0, 1) = New EwEColumnHeaderCell(My.Resources.HEADER_VALUE)

            Case frmShapeValue.eDisplayMode.Yearly
                Me.Redim(Me.m_iNumValues + 1, 2)
                Me(0, 0) = New EwEColumnHeaderCell(My.Resources.HEADER_YEAR)
                Me(0, 1) = New EwEColumnHeaderCell(My.Resources.HEADER_VALUE)

            Case frmShapeValue.eDisplayMode.Monthly
                Me.Redim(Me.m_iNumValues + 1, 3)
                Me(0, 0) = New EwEColumnHeaderCell(My.Resources.HEADER_YEAR)
                Me(0, 1) = New EwEColumnHeaderCell(My.Resources.HEADER_MONTH)
                Me(0, 2) = New EwEColumnHeaderCell(My.Resources.HEADER_VALUE)

        End Select

        Me.FixedColumnWidths = False

    End Sub

    Protected Overrides Sub FillData()

        Dim core As cCore = cCore.GetInstance()
        Dim cell As EwECell = Nothing
        Dim iStartIndex As Integer = core.EcosimFirstYear
        Dim sValue As Single = 0.0!

        If iStartIndex > 0 Then iStartIndex -= 1

        For iValue As Integer = 1 To Me.m_iNumValues

            sValue = 0.0!

            If Me.m_shape IsNot Nothing Then
                If iValue <= Me.m_shape.XMax Then
                    sValue = Me.m_shape.ShapeData(iValue)
                End If
            End If

            Select Case Me.m_displayMode

                Case frmShapeValue.eDisplayMode.Index

                    cell = New EwECell(CStr(iValue + 1), GetType(String))
                    cell.Style = cStyleGuide.eStyleFlags.NotEditable
                    Me(iValue, 0) = cell

                    cell = New EwECell(sValue, GetType(Single))
                    cell.SuppressZero = Me.m_bSuppressZeroes
                    Me(iValue, 1) = cell

                Case frmShapeValue.eDisplayMode.Yearly

                    cell = New EwECell(CStr(iValue + iStartIndex), GetType(String))
                    cell.Style = cStyleGuide.eStyleFlags.NotEditable
                    Me(iValue, 0) = cell

                    cell = New EwECell(sValue, GetType(Single))
                    cell.SuppressZero = Me.m_bSuppressZeroes
                    Me(iValue, 1) = cell

                Case frmShapeValue.eDisplayMode.Monthly

                    Dim strLabel0 As String = ""
                    Dim strLabel1 As String = ""
                    Dim iYear As Integer = iStartIndex + CInt(Math.Floor(iValue / 12))
                    Dim iMonth As Integer = 1 + ((iValue - 1) Mod 12)
                    Dim d As New Date(1, iMonth, 1)

                    If iMonth = 1 Then strLabel0 = CStr(iYear) Else strLabel0 = ""
                    strLabel1 = d.ToString("MMM")

                    cell = New EwECell(strLabel0, GetType(String))
                    cell.Style = cStyleGuide.eStyleFlags.NotEditable
                    Me(iValue, 0) = cell

                    cell = New EwECell(strLabel1, GetType(String))
                    cell.Style = cStyleGuide.eStyleFlags.NotEditable
                    Me(iValue, 1) = cell

                    cell = New EwECell(sValue, GetType(Single))
                    cell.SuppressZero = Me.m_bSuppressZeroes
                    Me(iValue, 2) = cell

            End Select

        Next

    End Sub

    Protected Overrides Sub FinishStyle()
        MyBase.FinishStyle()
    End Sub

    Protected Overrides Sub bm_colSelectClick(ByVal sender As Object, ByVal e As SourceGrid2.PositionEventArgs)
        ' Allow col selection
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
