' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Option Explicit On

Imports System.Windows.Forms
Imports ZedGraph
Imports ScientificInterfaceShared.Controls


Public Class cTotal
    Inherits cContentManager

    Public Sub New()
        '
    End Sub

    Public Overrides Function Attach(manager As cNetworkManager,
                                    datagrid As DataGridView,
                                    graph As ZedGraphControl,
                                    plot As ucPlot,
                                    toolstrip As ToolStrip,
                                         info As Control,
                                    uic As cUIContext) As Boolean
        Dim bSucces As Boolean = MyBase.Attach(manager, datagrid, graph, plot, toolstrip, info, uic)
        Me.Grid.Visible = bSucces
        Return bSucces
    End Function

    Public Overrides Sub DisplayData()
        Dim astrRowContent() As String

        Me.SetUpGridColumn()

        'Set up grid rows
        Me.Grid.RowHeadersVisible = False
        Me.Grid.RowCount = 6
        Me.Grid.Rows(0).DefaultCellStyle.WrapMode = DataGridViewTriState.True
        Me.Grid.Rows(0).DefaultCellStyle.BackColor = Drawing.SystemColors.Control
        Me.Grid.Rows(0).Frozen = True
        Me.Grid.Rows(0).Height = FIRST_ROW_HEIGHT

        ReDim astrRowContent(Me.Grid.Columns.Count)
        astrRowContent(0) = My.Resources.COL_HDR_SOURCE
        astrRowContent(1) = My.Resources.COL_HDR_ASCEND_FLOWBIT
        astrRowContent(2) = My.Resources.COL_HDR_ASCEND_PCT
        astrRowContent(3) = My.Resources.COL_HDR_OVERHEAD_FLOWBIT
        astrRowContent(4) = My.Resources.COL_HDR_OVERHEAD_PCT
        astrRowContent(5) = My.Resources.COL_HDR_CAPACITY_FLOWBIT
        astrRowContent(6) = My.Resources.COL_HDR_CAPACITY_PCT
        Me.Grid.Rows(0).SetValues(astrRowContent)
        Me.Grid.Rows(0).Visible = True

        astrRowContent(0) = My.Resources.ROW_HDR_IMPORT
        astrRowContent(1) = Me.StyleGuide.FormatNumber(Me.NetworkManager.AscendancyImportTotal)
        astrRowContent(2) = Me.StyleGuide.FormatNumber(Me.NetworkManager.AscendancyImportPer)
        astrRowContent(3) = Me.StyleGuide.FormatNumber(Me.NetworkManager.OverheadImportTotal)
        astrRowContent(4) = Me.StyleGuide.FormatNumber(Me.NetworkManager.OverheadImportPer)
        astrRowContent(5) = Me.StyleGuide.FormatNumber(Me.NetworkManager.CapacityImportTotal)
        astrRowContent(6) = Me.StyleGuide.FormatNumber(Me.NetworkManager.CapacityImportPer)
        Me.Grid.Rows(1).SetValues(astrRowContent)
        Me.Grid.Rows(1).Visible = True

        astrRowContent(0) = My.Resources.ROW_HDR_INTN_FLOW
        astrRowContent(1) = Me.StyleGuide.FormatNumber(Me.NetworkManager.AscendancyInternalFlowTotal)
        astrRowContent(2) = Me.StyleGuide.FormatNumber(Me.NetworkManager.AscendancyInternalFlowPer)
        astrRowContent(3) = Me.StyleGuide.FormatNumber(Me.NetworkManager.OverheadFlowTotal)
        astrRowContent(4) = Me.StyleGuide.FormatNumber(Me.NetworkManager.OverheadFlowPer)
        astrRowContent(5) = Me.StyleGuide.FormatNumber(Me.NetworkManager.CapacityFlowTotal)
        astrRowContent(6) = Me.StyleGuide.FormatNumber(Me.NetworkManager.CapacityFlowPer)
        Me.Grid.Rows(2).SetValues(astrRowContent)
        Me.Grid.Rows(2).Visible = True

        astrRowContent(0) = My.Resources.ROW_HDR_EXPORT
        astrRowContent(1) = Me.StyleGuide.FormatNumber(Me.NetworkManager.AscendancyExportTotal)
        astrRowContent(2) = Me.StyleGuide.FormatNumber(Me.NetworkManager.AscendancyExportPer)
        astrRowContent(3) = Me.StyleGuide.FormatNumber(Me.NetworkManager.OverheadExportTotal)
        astrRowContent(4) = Me.StyleGuide.FormatNumber(Me.NetworkManager.OverheadExportPer)
        astrRowContent(5) = Me.StyleGuide.FormatNumber(Me.NetworkManager.CapacityExportTotal)
        astrRowContent(6) = Me.StyleGuide.FormatNumber(Me.NetworkManager.CapacityExportPer)
        Me.Grid.Rows(3).SetValues(astrRowContent)
        Me.Grid.Rows(3).Visible = True

        astrRowContent(0) = My.Resources.ROW_HDR_RESP
        astrRowContent(1) = Me.StyleGuide.FormatNumber(Me.NetworkManager.AscendancyRespTotal)
        astrRowContent(2) = Me.StyleGuide.FormatNumber(Me.NetworkManager.AscendancyRespPer)
        astrRowContent(3) = Me.StyleGuide.FormatNumber(Me.NetworkManager.OverheadRespTotal)
        astrRowContent(4) = Me.StyleGuide.FormatNumber(Me.NetworkManager.OverheadRespPer)
        astrRowContent(5) = Me.StyleGuide.FormatNumber(Me.NetworkManager.CapacityRespTotal)
        astrRowContent(6) = Me.StyleGuide.FormatNumber(Me.NetworkManager.CapacityRespPer)
        Me.Grid.Rows(4).SetValues(astrRowContent)
        Me.Grid.Rows(4).Visible = True

        astrRowContent(0) = My.Resources.ROW_HDR_TOTAL
        astrRowContent(1) = Me.StyleGuide.FormatNumber(Me.NetworkManager.AscendancyTotalsTotal)
        astrRowContent(2) = Me.StyleGuide.FormatNumber(Me.NetworkManager.AscendancyTotalsPer)
        astrRowContent(3) = Me.StyleGuide.FormatNumber(Me.NetworkManager.OverheadTotalsTotal)
        astrRowContent(4) = Me.StyleGuide.FormatNumber(Me.NetworkManager.OverheadTotalsPer)
        astrRowContent(5) = Me.StyleGuide.FormatNumber(Me.NetworkManager.CapacityTotalsTotal)
        astrRowContent(6) = Me.StyleGuide.FormatNumber(Me.NetworkManager.CapacityTotalsPer)
        Me.Grid.Rows(5).SetValues(astrRowContent)
        Me.Grid.Rows(5).Visible = True

        Me.Grid.ClearSelection()

    End Sub

    Public Overrides Function PageTitle() As String
        ' ToDo: globalize this
        Return "Total ascendency"
    End Function

    Private Sub SetUpGridColumn()

        Me.Grid.ReadOnly = True
        'DataGrid.RowCount = 1
        Me.Grid.ColumnCount = 7

        SetGridColumnPropertyDefault(Me.Grid)

        Me.Grid.Columns(0).DefaultCellStyle.BackColor = Drawing.SystemColors.Control
        Me.Grid.Columns(0).Frozen = True

    End Sub

End Class
