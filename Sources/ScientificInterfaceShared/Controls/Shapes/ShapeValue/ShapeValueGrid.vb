'==============================================================================
'
' $Log: ShapeValueGrid.vb,v $
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
    Private m_displayMode As eDisplayMode = eDisplayMode.Monthly

    Private Enum eDisplayMode As Integer
        ''' <summary>Display values per year, month</summary>
        Monthly
        ''' <summary>Display values per index</summary>
        Index
    End Enum

    Public Sub SetValues(ByVal iNumValues As Integer, ByVal shape As cShapeData)
        Me.m_iNumValues = iNumValues
        Me.m_bSuppressZeroes = (TypeOf shape Is cTimeSeries)
        Me.m_shape = shape

        ' Determine display mode
        If TypeOf (shape) Is cMediationFunction Then
            Me.m_displayMode = eDisplayMode.Index
        Else
            Me.m_displayMode = eDisplayMode.Monthly
        End If
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

        Select Case Me.m_displayMode

            Case eDisplayMode.Index
                Me.Redim(Me.m_iNumValues + 1, 2)
                Me(0, 0) = New EwEColumnHeaderCell(My.Resources.HEADER_INDEX)
                Me(0, 1) = New EwEColumnHeaderCell(My.Resources.HEADER_VALUE)

            Case eDisplayMode.Monthly
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

        For iValue As Integer = 0 To Me.m_iNumValues - 1

            sValue = 0.0!

            If Me.m_shape IsNot Nothing Then
                If iValue <= Me.m_shape.XMax Then
                    sValue = Me.m_shape.ShapeData(iValue + 1)
                End If
            End If

            Select Case Me.m_displayMode

                Case eDisplayMode.Index

                    cell = New EwECell(CStr(iValue + iStartIndex + 1), GetType(String))
                    cell.Style = StyleGuide.eStyleFlags.NotEditable
                    Me(iValue + 1, 0) = cell

                    cell = New EwECell(sValue, GetType(Single))
                    cell.SuppressZero = Me.m_bSuppressZeroes
                    Me(iValue + 1, 1) = cell

                Case eDisplayMode.Monthly

                    Dim strLabel0 As String = ""
                    Dim strLabel1 As String = ""
                    Dim iYear As Integer = iStartIndex + 1 + CInt(Math.Floor(iValue / 12))
                    Dim iMonth As Integer = 1 + (iValue Mod 12)
                    Dim d As New Date(iYear, iMonth, 1)

                    If iMonth = 1 Then strLabel0 = CStr(iYear) Else strLabel0 = ""
                    strLabel1 = d.ToString("MMM")

                    cell = New EwECell(strLabel0, GetType(String))
                    cell.Style = StyleGuide.eStyleFlags.NotEditable
                    Me(iValue + 1, 0) = cell

                    cell = New EwECell(strLabel1, GetType(String))
                    cell.Style = StyleGuide.eStyleFlags.NotEditable
                    Me(iValue + 1, 1) = cell

                    cell = New EwECell(sValue, GetType(Single))
                    cell.SuppressZero = Me.m_bSuppressZeroes
                    Me(iValue + 1, 2) = cell

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
