'==============================================================================
'
' $Log: cAbsoluteFlows.vb,v $
' Revision 1.1  2008/09/26 07:30:55  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.16  2008/06/25 01:53:38  joeh
' Ecosim NA indice plots are displayed in the same form where we have the NA tree view - Take 2
'
' Revision 1.15  2008/06/24 18:08:36  joeh
' Ecosim NA indice plots are displayed in the same form where  we have the NA tree view - Take 2
'
' Revision 1.14  2007/07/09 19:44:44  joeh
' Move hard coded strings to resource file
'
' Revision 1.13  2007/07/07 00:11:03  joeh
' Decrease column width
'
' Revision 1.12  2007/06/28 19:22:51  joeh
' Switch to wait cursor when displaying data
'
' Revision 1.11  2007/06/22 19:12:44  joeh
' Modify GetInstance()
'
' Revision 1.10  2007/06/22 00:35:28  joeh
' Add Option Strict On and Option Explicit On
'
' Revision 1.9  2007/06/21 00:14:37  joeh
' Rename SetUpPanel() to DisplayData()
'
' Revision 1.8  2007/06/20 18:13:54  joeh
' add header to the top of the file so that CVS will log the file with every update
'
'
'==============================================================================
Option Strict On
Option Explicit On

Imports System.Windows.Forms
Imports ZedGraph

Public Class cAbsoluteFlows
    Private Shared m_AbsoluteFlowsInstnace As cAbsoluteFlows

    Private m_NetworkManager As cNetworkManager
    'Private m_Panel As Windows.Forms.Panel
    Private Shared m_Panel As Windows.Forms.Panel

    Public Shared Function GetInstance(ByVal NetworkManager As cNetworkManager, ByVal Panel As Windows.Forms.Panel) As cAbsoluteFlows
        m_Panel = Panel

        If m_AbsoluteFlowsInstnace Is Nothing Then m_AbsoluteFlowsInstnace = New cAbsoluteFlows(NetworkManager, Panel)
        Return m_AbsoluteFlowsInstnace
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

        SetUpGridColumn(m_NetworkManager.nTrophicLevels)

        'Set up grid rows
        DataGrid.RowHeadersVisible = False
        DataGrid.RowCount = m_NetworkManager.nGroups + 1

        'DataGrid.RowHeadersDefaultCellStyle.BackColor = Drawing.Color.Beige

        ReDim strRowContent(DataGrid.Columns.Count)
        For i As Integer = 1 To m_NetworkManager.nGroups
            strRowContent(0) = CStr(i)
            strRowContent(1) = m_NetworkManager.GroupName(i)
            For j As Integer = 1 To m_NetworkManager.nTrophicLevels
                strRowContent(j + 1) = (m_NetworkManager.AbsoluteFlow(i, j)).ToString("F4")
            Next
            'DataGrid.Rows.Add(strary)
            DataGrid.Rows(i - 1).SetValues(strRowContent)
            DataGrid.Rows(i - 1).Visible = True

            'DataGrid.Rows(i - 1).HeaderCell.Value = CStr(i)
            'DataGrid.Rows(i - 1).HeaderCell.Style.BackColor = Drawing.Color.Beige
        Next

        strRowContent(0) = ""
        strRowContent(1) = My.Resources.ROW_HDR_TOTAL
        For j As Integer = 1 To m_NetworkManager.nTrophicLevels
            strRowContent(j + 1) = (m_NetworkManager.AbsoluteFlowTotal(j)).ToString("F4")
        Next
        DataGrid.Rows(DataGrid.RowCount - 1).SetValues(strRowContent)
        DataGrid.Rows(DataGrid.RowCount - 1).Visible = True
        DataGrid.ClearSelection()
        Cursor.Current = Cursors.Default
    End Sub

    Private Sub SetUpGridColumn(ByVal iNumTrophicLevels As Integer)
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
        DataGrid.ColumnCount = iNumTrophicLevels + 2

        SetGridColumnPropertyDefault(DataGrid)

        DataGrid.Columns(0).DefaultCellStyle.BackColor = Drawing.Color.MintCream
        DataGrid.Columns(0).Frozen = True
        DataGrid.Columns(0).Width = ID_COL_WIDTH

        DataGrid.Columns(1).DefaultCellStyle.Alignment = Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGrid.Columns(1).DefaultCellStyle.BackColor = Drawing.Color.MintCream
        DataGrid.Columns(1).Frozen = True
        DataGrid.Columns(1).Width = GRP_NAME_COL_WIDTH

        DataGrid.Columns(0).HeaderText = ""
        DataGrid.Columns(1).HeaderText = My.Resources.COL_HDR_GRP_NAME_TRP_LVL
        For i As Integer = 1 To m_NetworkManager.nTrophicLevels
            DataGrid.Columns(i + 1).HeaderText = CRoman(i)
        Next
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
