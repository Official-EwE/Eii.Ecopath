'==============================================================================
'
' $Log: cByGroup.vb,v $
' Revision 1.1  2008/09/26 07:30:48  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.19  2008/06/25 01:53:39  joeh
' Ecosim NA indice plots are displayed in the same form where we have the NA tree view - Take 2
'
' Revision 1.18  2008/06/24 18:08:37  joeh
' Ecosim NA indice plots are displayed in the same form where  we have the NA tree view - Take 2
'
' Revision 1.17  2007/07/09 23:05:48  joeh
' Move hard coded strings to resource file
'
' Revision 1.16  2007/07/07 00:11:03  joeh
' Decrease column width
'
' Revision 1.15  2007/07/06 00:44:59  joeh
' Move hard coded strings to resource file
'
' Revision 1.14  2007/06/28 19:22:51  joeh
' Switch to wait cursor when displaying data
'
' Revision 1.13  2007/06/22 19:12:45  joeh
' Modify GetInstance()
'
' Revision 1.12  2007/06/22 00:35:28  joeh
' Add Option Strict On and Option Explicit On
'
' Revision 1.11  2007/06/21 23:49:35  joeh
' Move hard coded strings into the resource file
'
' Revision 1.10  2007/06/21 18:08:45  joeh
' Make the 2 in km2 to superscript
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

Public Class cByGroup
    Private Shared m_ByGroupInstance As cByGroup

    Private m_NetworkManager As cNetworkManager
    'Private m_Panel As Windows.Forms.Panel
    Private Shared m_Panel As Windows.Forms.Panel

    Public Shared Function GetInstance(ByVal NetworkManager As cNetworkManager, ByVal Panel As Windows.Forms.Panel) As cByGroup
        m_Panel = Panel

        If m_ByGroupInstance Is Nothing Then m_ByGroupInstance = New cByGroup(NetworkManager, Panel)
        Return m_ByGroupInstance
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
        Dim DataGrid As Windows.Forms.DataGridView = _
            CType(m_Panel.Controls("dgvNetworkAnalysis"), Windows.Forms.DataGridView)
        Dim strRowContent() As String

        Cursor.Current = Cursors.WaitCursor
        RemoveToolStrip()

        SetUpGridColumn()

        'Set up grid rows
        DataGrid.RowHeadersVisible = False
        DataGrid.RowCount = m_NetworkManager.nGroups + 3

        ReDim strRowContent(DataGrid.Columns.Count)
        For i As Integer = 1 To m_NetworkManager.nGroups
            strRowContent(0) = CStr(i)
            strRowContent(1) = m_NetworkManager.GroupName(i)
            strRowContent(2) = m_NetworkManager.AscendancyByGroup(i).ToString("F4")
            strRowContent(3) = m_NetworkManager.OverheadByGroup(i).ToString("F4")
            strRowContent(4) = m_NetworkManager.CapacityByGroup(i).ToString("F4")
            strRowContent(5) = m_NetworkManager.InformationByGroup(i).ToString("F4")
            strRowContent(6) = m_NetworkManager.ThroughputByGroup(i).ToString("F4")
            DataGrid.Rows(i - 1).SetValues(strRowContent)
            DataGrid.Rows(i - 1).Visible = True
        Next

        strRowContent(0) = ""
        strRowContent(1) = My.Resources.ROW_HDR_IMPORT
        strRowContent(2) = ""
        strRowContent(3) = ""
        strRowContent(4) = ""
        strRowContent(5) = ""
        strRowContent(6) = m_NetworkManager.ThroughputByGroup(m_NetworkManager.nGroups + 1).ToString("F4")
        DataGrid.Rows(m_NetworkManager.nGroups).SetValues(strRowContent)
        DataGrid.Rows(m_NetworkManager.nGroups).Visible = True

        strRowContent(0) = ""
        strRowContent(1) = My.Resources.ROW_HDR_TOTAL
        strRowContent(2) = m_NetworkManager.AscendencyTotal.ToString("F4")
        strRowContent(3) = m_NetworkManager.OverheadTotal.ToString("F4")
        strRowContent(4) = m_NetworkManager.CapacityTotal.ToString("F4")
        If m_NetworkManager.ThroughputTotal > 0 Then
            strRowContent(5) = (m_NetworkManager.AscendencyTotal / m_NetworkManager.ThroughputTotal).ToString("F4")
        Else
            strRowContent(5) = ""
        End If
        strRowContent(6) = m_NetworkManager.ThroughputTotal.ToString("F4")
        DataGrid.Rows(m_NetworkManager.nGroups + 1).SetValues(strRowContent)
        DataGrid.Rows(m_NetworkManager.nGroups + 1).Visible = True

        strRowContent(0) = ""
        strRowContent(1) = My.Resources.ROW_HDR_PCT
        strRowContent(2) = (m_NetworkManager.AscendencyTotal / m_NetworkManager.CapacityTotal * 100.0).ToString("F4")
        strRowContent(3) = (m_NetworkManager.OverheadTotal / m_NetworkManager.CapacityTotal * 100.0).ToString("F4")
        strRowContent(4) = (m_NetworkManager.CapacityTotal / m_NetworkManager.CapacityTotal * 100.0).ToString("F4")
        strRowContent(5) = ""
        strRowContent(6) = ""
        DataGrid.Rows(m_NetworkManager.nGroups + 2).SetValues(strRowContent)
        DataGrid.Rows(m_NetworkManager.nGroups + 2).Visible = True

        DataGrid.ClearSelection()
        Cursor.Current = Cursors.default
    End Sub

    Private Sub SetUpGridColumn()
        Dim DataGrid As Windows.Forms.DataGridView = _
            CType(m_Panel.Controls("dgvNetworkAnalysis"), Windows.Forms.DataGridView)
        Dim GraphPane As ZedGraphControl = _
            CType(m_Panel.Controls("zgcNetworkAnalysis"), ZedGraphControl)
        Dim LogoPanel As Windows.Forms.TableLayoutPanel = _
            CType(m_Panel.Controls("tlpNetworkAnalysis"), Windows.Forms.TableLayoutPanel)

        LogoPanel.Visible = False
        GraphPane.Visible = False
        DataGrid.ReadOnly = True
        DataGrid.Visible = True
        'DataGrid.RowCount = 1
        DataGrid.ColumnCount = 7

        SetGridColumnPropertyDefault(DataGrid)

        DataGrid.Columns(0).DefaultCellStyle.BackColor = Drawing.Color.MintCream
        DataGrid.Columns(0).Frozen = True
        DataGrid.Columns(0).Width = ID_COL_WIDTH

        DataGrid.Columns(1).DefaultCellStyle.Alignment = Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGrid.Columns(1).DefaultCellStyle.BackColor = Drawing.Color.MintCream
        DataGrid.Columns(1).Frozen = True
        DataGrid.Columns(1).Width = GRP_NAME_COL_WIDTH

        'For intIndex As Integer = 2 To 4
        '    DataGrid.Columns(intIndex).Width = 120
        'Next

        DataGrid.Columns(0).HeaderText = ""
        DataGrid.Columns(1).HeaderText = My.Resources.COL_HDR_GRP_NAME
        DataGrid.Columns(2).HeaderText = My.Resources.COL_HDR_ASCEND
        DataGrid.Columns(3).HeaderText = My.Resources.COL_HDR_OVERHEAD
        DataGrid.Columns(4).HeaderText = My.Resources.COL_HDR_CAPACITY
        DataGrid.Columns(5).HeaderText = My.Resources.COL_HDR_INFO
        DataGrid.Columns(6).HeaderText = My.Resources.COL_HDR_THROUGHPUT_UNIT
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
