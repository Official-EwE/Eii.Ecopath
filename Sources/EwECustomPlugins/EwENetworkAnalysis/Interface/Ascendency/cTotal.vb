'==============================================================================
'
' $Log: cTotal.vb,v $
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
    Private Shared m_TotalInstance As cTotal

    Private m_NetworkManager As cNetworkManager
    'Private m_Panel As Windows.Forms.Panel
    Private Shared m_Panel As Windows.Forms.Panel

    Public Shared Function GetInstance(ByVal NetworkManager As cNetworkManager, ByVal Panel As Windows.Forms.Panel) As cTotal
        m_Panel = Panel

        If m_TotalInstance Is Nothing Then m_TotalInstance = New cTotal(NetworkManager, Panel)
        Return m_TotalInstance
    End Function

    Private Sub New()
        '
    End Sub

    Private Sub New(ByVal NetworkManager As cNetworkManager, ByVal Panel As Windows.Forms.Panel)
        Me.New()
        m_NetworkManager = NetworkManager
        m_Panel = Panel
    End Sub

    Public Sub DisplayData()
        'Dim ToolStrip As Windows.Forms.ToolStrip = _
        '    CType(m_Panel.Controls("tsNetworkAnalysis"), Windows.Forms.ToolStrip)
        Dim DataGrid As Windows.Forms.DataGridView = _
            CType(m_Panel.Controls("dgvNetworkAnalysis"), Windows.Forms.DataGridView)
        Dim strRowContent() As String

        Cursor.Current = Cursors.WaitCursor
        RemoveToolStrip()

        SetUpGridColumn()

        'Set up grid rows
        DataGrid.RowHeadersVisible = False
        DataGrid.RowCount = 6
        DataGrid.Rows(0).DefaultCellStyle.WrapMode = DataGridViewTriState.True
        DataGrid.Rows(0).DefaultCellStyle.BackColor = Drawing.Color.MintCream
        DataGrid.Rows(0).Frozen = True
        DataGrid.Rows(0).Height = FIRST_ROW_HEIGHT

        ReDim strRowContent(DataGrid.Columns.Count)
        strRowContent(0) = My.Resources.COL_HDR_SOURCE
        strRowContent(1) = My.Resources.COL_HDR_ASCEND_FLOWBIT
        strRowContent(2) = My.Resources.COL_HDR_ASCEND_PCT
        strRowContent(3) = My.Resources.COL_HDR_OVERHEAD_FLOWBIT
        strRowContent(4) = My.Resources.COL_HDR_OVERHEAD_PCT
        strRowContent(5) = My.Resources.COL_HDR_CAPACITY_FLOWBIT
        strRowContent(6) = My.Resources.COL_HDR_CAPACITY_PCT
        DataGrid.Rows(0).SetValues(strRowContent)
        DataGrid.Rows(0).Visible = True

        strRowContent(0) = My.Resources.ROW_HDR_IMPORT
        strRowContent(1) = m_NetworkManager.AscendancyImportTotal.ToString("F1")
        strRowContent(2) = m_NetworkManager.AscendancyImportPer.ToString("F1")
        strRowContent(3) = m_NetworkManager.OverheadImportTotal.ToString("F1")
        strRowContent(4) = m_NetworkManager.OverheadImportPer.ToString("F1")
        strRowContent(5) = m_NetworkManager.CapacityImportTotal.ToString("F1")
        strRowContent(6) = m_NetworkManager.CapacityImportPer.ToString("F1")
        DataGrid.Rows(1).SetValues(strRowContent)
        DataGrid.Rows(1).Visible = True

        strRowContent(0) = My.Resources.ROW_HDR_INTN_FLOW
        strRowContent(1) = m_NetworkManager.AscendancyInternalFlowTotal.ToString("F1")
        strRowContent(2) = m_NetworkManager.AscendancyInternalFlowPer.ToString("F1")
        strRowContent(3) = m_NetworkManager.OverheadFlowTotal.ToString("F1")
        strRowContent(4) = m_NetworkManager.OverheadFlowPer.ToString("F1")
        strRowContent(5) = m_NetworkManager.CapacityFlowTotal.ToString("F1")
        strRowContent(6) = m_NetworkManager.CapacityFlowPer.ToString("F1")
        DataGrid.Rows(2).SetValues(strRowContent)
        DataGrid.Rows(2).Visible = True

        strRowContent(0) = My.Resources.ROW_HDR_EXPORT
        strRowContent(1) = m_NetworkManager.AscendancyExportTotal.ToString("F1")
        strRowContent(2) = m_NetworkManager.AscendancyExportPer.ToString("F1")
        strRowContent(3) = m_NetworkManager.OverheadExportTotal.ToString("F1")
        strRowContent(4) = m_NetworkManager.OverheadExportPer.ToString("F1")
        strRowContent(5) = m_NetworkManager.CapacityExportTotal.ToString("F1")
        strRowContent(6) = m_NetworkManager.CapacityExportPer.ToString("F1")
        DataGrid.Rows(3).SetValues(strRowContent)
        DataGrid.Rows(3).Visible = True

        strRowContent(0) = My.Resources.ROW_HDR_RESP
        strRowContent(1) = m_NetworkManager.AscendancyRespTotal.ToString("F1")
        strRowContent(2) = m_NetworkManager.AscendancyRespPer.ToString("F1")
        strRowContent(3) = m_NetworkManager.OverheadRespTotal.ToString("F1")
        strRowContent(4) = m_NetworkManager.OverheadRespPer.ToString("F1")
        strRowContent(5) = m_NetworkManager.CapacityRespTotal.ToString("F1")
        strRowContent(6) = m_NetworkManager.CapacityRespPer.ToString("F1")
        DataGrid.Rows(4).SetValues(strRowContent)
        DataGrid.Rows(4).Visible = True

        strRowContent(0) = My.Resources.ROW_HDR_TOTAL
        strRowContent(1) = m_NetworkManager.AscendancyTotalsTotal.ToString("F1")
        strRowContent(2) = m_NetworkManager.AscendancyTotalsPer.ToString("F1")
        strRowContent(3) = m_NetworkManager.OverheadTotalsTotal.ToString("F1")
        strRowContent(4) = m_NetworkManager.OverheadTotalsPer.ToString("F1")
        strRowContent(5) = m_NetworkManager.CapacityTotalsTotal.ToString("F1")
        strRowContent(6) = m_NetworkManager.CapacityTotalsPer.ToString("F1")
        DataGrid.Rows(5).SetValues(strRowContent)
        DataGrid.Rows(5).Visible = True

        DataGrid.ClearSelection()
        Cursor.Current = Cursors.Default

    End Sub

    Private Sub SetUpGridColumn()
        Dim DataGrid As Windows.Forms.DataGridView = _
            CType(m_Panel.Controls("dgvNetworkAnalysis"), Windows.Forms.DataGridView)
        Dim GraphPane As ZedGraphControl = _
            CType(m_Panel.Controls("zgcNetworkAnalysis"), ZedGraphControl)
        Dim LogoPanel As Windows.Forms.TableLayoutPanel = _
            CType(m_Panel.Controls("tlpNetworkAnalysis"), Windows.Forms.TableLayoutPanel)

        m_Panel.AutoScroll = False
        LogoPanel.Visible = False
        GraphPane.Visible = False
        DataGrid.ReadOnly = True
        DataGrid.Visible = True
        'DataGrid.RowCount = 1
        DataGrid.ColumnCount = 7

        SetGridColumnPropertyDefault(DataGrid)

        DataGrid.Columns(0).DefaultCellStyle.BackColor = Drawing.Color.MintCream
        DataGrid.Columns(0).Frozen = True
    End Sub

    Private Sub RemoveToolStrip()
        Dim ToolStrip As Windows.Forms.ToolStrip = _
            CType(m_Panel.Controls("tsNetworkAnalysis"), Windows.Forms.ToolStrip)
        Dim DataGrid As Windows.Forms.DataGridView = _
            CType(m_Panel.Controls("dgvNetworkAnalysis"), Windows.Forms.DataGridView)

        If Not ToolStrip Is Nothing Then
            m_Panel.Controls.RemoveByKey("tsNetworkAnalysis")
            DataGrid.Dock = Windows.Forms.DockStyle.Fill
        End If
    End Sub

End Class
