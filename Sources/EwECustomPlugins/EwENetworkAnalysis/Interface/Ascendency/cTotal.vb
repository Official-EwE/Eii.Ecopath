#Region " Imports "

Option Strict On
Option Explicit On

Imports System.Windows.Forms
Imports ZedGraph

#End Region ' Imports

<CLSCompliant(False)> _
Public Class cTotal
    Inherits cContentManager

    Public Sub New()
        '
    End Sub

    Public Overrides Function Attach(ByVal manager As cNetworkManager, _
                                    ByVal datagrid As DataGridView, _
                                    ByVal graph As ZedGraphControl, _
                                    ByVal plot As ucPlot, _
                                    ByVal toolstrip As ToolStrip) As Boolean
        Dim bSucces As Boolean = MyBase.Attach(manager, datagrid, graph, plot, toolstrip)
        Me.Grid.Visible = bSucces
        Return bSucces
    End Function

    Public Overrides Sub DisplayData()
        Dim astrRowContent() As String

        SetUpGridColumn()

        'Set up grid rows
        Grid.RowHeadersVisible = False
        Grid.RowCount = 6
        Grid.Rows(0).DefaultCellStyle.WrapMode = DataGridViewTriState.True
        Grid.Rows(0).DefaultCellStyle.BackColor = Drawing.Color.MintCream
        Grid.Rows(0).Frozen = True
        Grid.Rows(0).Height = FIRST_ROW_HEIGHT

        ReDim astrRowContent(Grid.Columns.Count)
        astrRowContent(0) = My.Resources.COL_HDR_SOURCE
        astrRowContent(1) = My.Resources.COL_HDR_ASCEND_FLOWBIT
        astrRowContent(2) = My.Resources.COL_HDR_ASCEND_PCT
        astrRowContent(3) = My.Resources.COL_HDR_OVERHEAD_FLOWBIT
        astrRowContent(4) = My.Resources.COL_HDR_OVERHEAD_PCT
        astrRowContent(5) = My.Resources.COL_HDR_CAPACITY_FLOWBIT
        astrRowContent(6) = My.Resources.COL_HDR_CAPACITY_PCT
        Grid.Rows(0).SetValues(astrRowContent)
        Grid.Rows(0).Visible = True

        astrRowContent(0) = My.Resources.ROW_HDR_IMPORT
        astrRowContent(1) = NetworkManager.AscendancyImportTotal.ToString("F1")
        astrRowContent(2) = NetworkManager.AscendancyImportPer.ToString("F1")
        astrRowContent(3) = NetworkManager.OverheadImportTotal.ToString("F1")
        astrRowContent(4) = NetworkManager.OverheadImportPer.ToString("F1")
        astrRowContent(5) = NetworkManager.CapacityImportTotal.ToString("F1")
        astrRowContent(6) = NetworkManager.CapacityImportPer.ToString("F1")
        Grid.Rows(1).SetValues(astrRowContent)
        Grid.Rows(1).Visible = True

        astrRowContent(0) = My.Resources.ROW_HDR_INTN_FLOW
        astrRowContent(1) = NetworkManager.AscendancyInternalFlowTotal.ToString("F1")
        astrRowContent(2) = NetworkManager.AscendancyInternalFlowPer.ToString("F1")
        astrRowContent(3) = NetworkManager.OverheadFlowTotal.ToString("F1")
        astrRowContent(4) = NetworkManager.OverheadFlowPer.ToString("F1")
        astrRowContent(5) = NetworkManager.CapacityFlowTotal.ToString("F1")
        astrRowContent(6) = NetworkManager.CapacityFlowPer.ToString("F1")
        Grid.Rows(2).SetValues(astrRowContent)
        Grid.Rows(2).Visible = True

        astrRowContent(0) = My.Resources.ROW_HDR_EXPORT
        astrRowContent(1) = NetworkManager.AscendancyExportTotal.ToString("F1")
        astrRowContent(2) = NetworkManager.AscendancyExportPer.ToString("F1")
        astrRowContent(3) = NetworkManager.OverheadExportTotal.ToString("F1")
        astrRowContent(4) = NetworkManager.OverheadExportPer.ToString("F1")
        astrRowContent(5) = NetworkManager.CapacityExportTotal.ToString("F1")
        astrRowContent(6) = NetworkManager.CapacityExportPer.ToString("F1")
        Grid.Rows(3).SetValues(astrRowContent)
        Grid.Rows(3).Visible = True

        astrRowContent(0) = My.Resources.ROW_HDR_RESP
        astrRowContent(1) = NetworkManager.AscendancyRespTotal.ToString("F1")
        astrRowContent(2) = NetworkManager.AscendancyRespPer.ToString("F1")
        astrRowContent(3) = NetworkManager.OverheadRespTotal.ToString("F1")
        astrRowContent(4) = NetworkManager.OverheadRespPer.ToString("F1")
        astrRowContent(5) = NetworkManager.CapacityRespTotal.ToString("F1")
        astrRowContent(6) = NetworkManager.CapacityRespPer.ToString("F1")
        Grid.Rows(4).SetValues(astrRowContent)
        Grid.Rows(4).Visible = True

        astrRowContent(0) = My.Resources.ROW_HDR_TOTAL
        astrRowContent(1) = NetworkManager.AscendancyTotalsTotal.ToString("F1")
        astrRowContent(2) = NetworkManager.AscendancyTotalsPer.ToString("F1")
        astrRowContent(3) = NetworkManager.OverheadTotalsTotal.ToString("F1")
        astrRowContent(4) = NetworkManager.OverheadTotalsPer.ToString("F1")
        astrRowContent(5) = NetworkManager.CapacityTotalsTotal.ToString("F1")
        astrRowContent(6) = NetworkManager.CapacityTotalsPer.ToString("F1")
        Grid.Rows(5).SetValues(astrRowContent)
        Grid.Rows(5).Visible = True

        Grid.ClearSelection()

    End Sub

    Private Sub SetUpGridColumn()

        Grid.ReadOnly = True
        'DataGrid.RowCount = 1
        Grid.ColumnCount = 7

        SetGridColumnPropertyDefault(Grid)

        Grid.Columns(0).DefaultCellStyle.BackColor = Drawing.Color.MintCream
        Grid.Columns(0).Frozen = True

    End Sub

End Class
