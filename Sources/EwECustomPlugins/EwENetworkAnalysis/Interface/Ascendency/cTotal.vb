'==============================================================================
'
' $Log: cTotal.vb,v $
' Revision 1.7  2009/05/02 01:51:21  jeroens
' Updated to cControlManager FN name change
'
' Revision 1.6  2009/05/01 17:42:54  jeroens
' Inherited from cContentManager
'
' Revision 1.5  2009/04/17 01:07:01  joeh
' Make MixedTrophicImpactUC not visible when needed
'
' Revision 1.4  2009/04/16 00:29:47  joeh
' Add "Import.System.Windows.Forms" statement
'
' Revision 1.3  2009/04/15 18:14:49  joeh
' Set m_Panel.AutoScroll = False
'
' Revision 1.2  2008/11/25 20:55:41  joeh
' Copy and paste in cells of data grid view
'
' Revision 1.1  2008/09/26 07:30:48  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.15  2008/06/25 01:53:39  joeh
' Ecosim NA indice plots are displayed in the same form where we have the NA tree view - Take 2
'
' Revision 1.14  2008/06/24 18:08:37  joeh
' Ecosim NA indice plots are displayed in the same form where  we have the NA tree view - Take 2
'
' Revision 1.13  2007/06/28 19:21:22  joeh
' Switch to wait cursor when displaying data
'
' Revision 1.12  2007/06/22 19:12:45  joeh
' Modify GetInstance()
'
' Revision 1.11  2007/06/22 00:35:28  joeh
' Add Option Strict On and Option Explicit On
'
' Revision 1.10  2007/06/21 23:49:35  joeh
' Move hard coded strings into the resource file
'
' Revision 1.9  2007/06/21 00:14:38  joeh
' Rename SetUpPanel() to DisplayData()
'
' Revision 1.8  2007/06/20 18:13:55  joeh
' add header to the top of the file so that CVS will log the file with every update
'
'
'==============================================================================
Option Strict On
Option Explicit On

Imports System.Windows.Forms
Imports ZedGraph

Public Class cTotal
    Inherits cContentManager

    Public Sub New()
        '
    End Sub

    Public Overrides Sub Attach(ByVal manager As cNetworkManager, _
                                  ByVal datagrid As DataGridView, _
                                  ByVal graph As ZedGraphControl, _
                                  ByVal plot As ucPlot)
        MyBase.Attach(manager, datagrid, graph, plot)
        Me.Grid.Visible = True
    End Sub

    Public Overrides Sub DisplayData()
        Dim strRowContent() As String

        SetUpGridColumn()

        'Set up grid rows
        Grid.RowHeadersVisible = False
        Grid.RowCount = 6
        Grid.Rows(0).DefaultCellStyle.WrapMode = DataGridViewTriState.True
        Grid.Rows(0).DefaultCellStyle.BackColor = Drawing.Color.MintCream
        Grid.Rows(0).Frozen = True
        Grid.Rows(0).Height = FIRST_ROW_HEIGHT

        ReDim strRowContent(Grid.Columns.Count)
        strRowContent(0) = My.Resources.COL_HDR_SOURCE
        strRowContent(1) = My.Resources.COL_HDR_ASCEND_FLOWBIT
        strRowContent(2) = My.Resources.COL_HDR_ASCEND_PCT
        strRowContent(3) = My.Resources.COL_HDR_OVERHEAD_FLOWBIT
        strRowContent(4) = My.Resources.COL_HDR_OVERHEAD_PCT
        strRowContent(5) = My.Resources.COL_HDR_CAPACITY_FLOWBIT
        strRowContent(6) = My.Resources.COL_HDR_CAPACITY_PCT
        Grid.Rows(0).SetValues(strRowContent)
        Grid.Rows(0).Visible = True

        strRowContent(0) = My.Resources.ROW_HDR_IMPORT
        strRowContent(1) = NetworkManager.AscendancyImportTotal.ToString("F1")
        strRowContent(2) = NetworkManager.AscendancyImportPer.ToString("F1")
        strRowContent(3) = NetworkManager.OverheadImportTotal.ToString("F1")
        strRowContent(4) = NetworkManager.OverheadImportPer.ToString("F1")
        strRowContent(5) = NetworkManager.CapacityImportTotal.ToString("F1")
        strRowContent(6) = NetworkManager.CapacityImportPer.ToString("F1")
        Grid.Rows(1).SetValues(strRowContent)
        Grid.Rows(1).Visible = True

        strRowContent(0) = My.Resources.ROW_HDR_INTN_FLOW
        strRowContent(1) = NetworkManager.AscendancyInternalFlowTotal.ToString("F1")
        strRowContent(2) = NetworkManager.AscendancyInternalFlowPer.ToString("F1")
        strRowContent(3) = NetworkManager.OverheadFlowTotal.ToString("F1")
        strRowContent(4) = NetworkManager.OverheadFlowPer.ToString("F1")
        strRowContent(5) = NetworkManager.CapacityFlowTotal.ToString("F1")
        strRowContent(6) = NetworkManager.CapacityFlowPer.ToString("F1")
        Grid.Rows(2).SetValues(strRowContent)
        Grid.Rows(2).Visible = True

        strRowContent(0) = My.Resources.ROW_HDR_EXPORT
        strRowContent(1) = NetworkManager.AscendancyExportTotal.ToString("F1")
        strRowContent(2) = NetworkManager.AscendancyExportPer.ToString("F1")
        strRowContent(3) = NetworkManager.OverheadExportTotal.ToString("F1")
        strRowContent(4) = NetworkManager.OverheadExportPer.ToString("F1")
        strRowContent(5) = NetworkManager.CapacityExportTotal.ToString("F1")
        strRowContent(6) = NetworkManager.CapacityExportPer.ToString("F1")
        Grid.Rows(3).SetValues(strRowContent)
        Grid.Rows(3).Visible = True

        strRowContent(0) = My.Resources.ROW_HDR_RESP
        strRowContent(1) = NetworkManager.AscendancyRespTotal.ToString("F1")
        strRowContent(2) = NetworkManager.AscendancyRespPer.ToString("F1")
        strRowContent(3) = NetworkManager.OverheadRespTotal.ToString("F1")
        strRowContent(4) = NetworkManager.OverheadRespPer.ToString("F1")
        strRowContent(5) = NetworkManager.CapacityRespTotal.ToString("F1")
        strRowContent(6) = NetworkManager.CapacityRespPer.ToString("F1")
        Grid.Rows(4).SetValues(strRowContent)
        Grid.Rows(4).Visible = True

        strRowContent(0) = My.Resources.ROW_HDR_TOTAL
        strRowContent(1) = NetworkManager.AscendancyTotalsTotal.ToString("F1")
        strRowContent(2) = NetworkManager.AscendancyTotalsPer.ToString("F1")
        strRowContent(3) = NetworkManager.OverheadTotalsTotal.ToString("F1")
        strRowContent(4) = NetworkManager.OverheadTotalsPer.ToString("F1")
        strRowContent(5) = NetworkManager.CapacityTotalsTotal.ToString("F1")
        strRowContent(6) = NetworkManager.CapacityTotalsPer.ToString("F1")
        Grid.Rows(5).SetValues(strRowContent)
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
