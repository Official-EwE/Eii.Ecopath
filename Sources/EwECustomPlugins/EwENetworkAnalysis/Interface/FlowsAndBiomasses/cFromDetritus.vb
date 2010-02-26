#Region " Imports "

Option Strict On
Option Explicit On

Imports System.Windows.Forms
Imports ZedGraph
Imports EwEUtils.Utilities
Imports ScientificInterfaceShared.Controls

#End Region ' Imports

<CLSCompliant(False)> _
Public Class cFromDetritus
    Inherits cContentManager

    Public Sub New()
        '
    End Sub

    Public Overrides Function Attach(ByVal manager As cNetworkManager, _
                                    ByVal datagrid As DataGridView, _
                                    ByVal graph As ZedGraphControl, _
                                    ByVal plot As ucPlot, _
                                    ByVal toolstrip As ToolStrip, _
                                    ByVal uic As cUIContext) As Boolean
        Dim bSucces As Boolean = MyBase.Attach(manager, datagrid, graph, plot, toolstrip, uic)
        Me.Grid.Visible = bSucces
        Return bSucces
    End Function

    Public Overrides Sub DisplayData()

        Dim strRowContent() As String
        Dim asSum() As Single

        SetUpGridColumn()

        'Set up grid rows
        Grid.RowHeadersVisible = False
        Grid.RowCount = NetworkManager.nTrophicLevels + 2
        Grid.Rows(0).DefaultCellStyle.WrapMode = DataGridViewTriState.True
        Grid.Rows(0).DefaultCellStyle.BackColor = Drawing.Color.MintCream
        Grid.Rows(0).Frozen = True
        Grid.Rows(0).Height = FIRST_ROW_HEIGHT

        ReDim strRowContent(Grid.Columns.Count)
        ReDim asSum(Grid.Columns.Count)
        strRowContent(0) = My.Resources.COL_HDR_TRP_LVL_FLOW
        strRowContent(1) = My.Resources.COL_HDR_IMPORT
        strRowContent(2) = My.Resources.COL_HDR_CONSUM_PREDAT
        strRowContent(3) = My.Resources.COL_HDR_EXPORT
        strRowContent(4) = My.Resources.COL_HDR_FLOW_DET
        strRowContent(5) = My.Resources.COL_HDR_RESP
        strRowContent(6) = My.Resources.COL_HDR_THROUGHPUT
        Grid.Rows(0).SetValues(strRowContent)
        Grid.Visible = True

        For i As Integer = NetworkManager.nTrophicLevels To 1 Step -1
            strRowContent(0) = cStringUtils.ToRoman(i)
            If i = 1 Then
                strRowContent(1) = Me.StyleGuide.FormatNumber(NetworkManager.DetImport(i))
                asSum(1) = asSum(1) + NetworkManager.DetImport(i)
            Else
                strRowContent(1) = ""
            End If
            strRowContent(2) = Me.StyleGuide.FormatNumber(NetworkManager.DetConsByPred(i))
            asSum(2) = asSum(2) + NetworkManager.DetConsByPred(i)
            strRowContent(3) = Me.StyleGuide.FormatNumber(NetworkManager.DetExport(i))
            asSum(3) = asSum(3) + NetworkManager.DetExport(i)
            strRowContent(4) = Me.StyleGuide.FormatNumber(NetworkManager.DetToDetritus(i))
            asSum(4) = asSum(4) + NetworkManager.DetToDetritus(i)
            strRowContent(5) = Me.StyleGuide.FormatNumber(NetworkManager.DetRespiration(i))
            asSum(5) = asSum(5) + NetworkManager.DetRespiration(i)
            strRowContent(6) = Me.StyleGuide.FormatNumber(NetworkManager.DetThroughtput(i))
            asSum(6) = asSum(6) + NetworkManager.DetThroughtput(i)
            Grid.Rows(NetworkManager.nTrophicLevels - i + 1).SetValues(strRowContent)
            Grid.Rows(NetworkManager.nTrophicLevels - i + 1).Visible = True
        Next

        strRowContent(0) = My.Resources.ROW_HDR_SUM
        For i As Integer = 1 To Grid.Columns.Count - 1
            strRowContent(i) = Me.StyleGuide.FormatNumber(asSum(i))
        Next
        Grid.Rows(Grid.RowCount - 1).SetValues(strRowContent)
        Grid.Rows(Grid.RowCount - 1).Visible = True
        Grid.ClearSelection()

    End Sub

    Private Sub SetUpGridColumn()

        Grid.ReadOnly = True
        'DataGrid.RowCount = 1
        Grid.ColumnCount = 7

        SetGridColumnPropertyDefault(Grid)

        Grid.Columns(0).Frozen = True
        Grid.Columns(0).DefaultCellStyle.BackColor = Drawing.Color.MintCream

    End Sub

End Class
