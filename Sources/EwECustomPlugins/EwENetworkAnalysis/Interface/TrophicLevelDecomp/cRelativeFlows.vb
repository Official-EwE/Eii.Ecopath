'==============================================================================
'
' $Log: cRelativeFlows.vb,v $
' Revision 1.4  2009/04/15 18:14:47  joeh
' Set m_Panel.AutoScroll = False
'
' Revision 1.3  2008/12/04 01:14:16  joeh
' Add ucPlotOfMixedTrophicImpact
'
' Revision 1.2  2008/11/25 05:47:34  joeh
' Copy and paste in cells of data grid view
'
' Revision 1.1  2008/09/26 07:30:56  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.15  2008/06/25 01:53:38  joeh
' Ecosim NA indice plots are displayed in the same form where we have the NA tree view - Take 2
'
' Revision 1.14  2008/06/24 18:08:37  joeh
' Ecosim NA indice plots are displayed in the same form where  we have the NA tree view - Take 2
'
' Revision 1.13  2007/07/07 00:11:03  joeh
' Decrease column width
'
' Revision 1.12  2007/06/28 19:24:33  joeh
' Switch to wait cursor when displaying data
'
' Revision 1.11  2007/06/22 00:35:28  joeh
' Add Option Strict On and Option Explicit On
'
' Revision 1.10  2007/06/21 23:49:37  joeh
' Move hard coded strings into the resource file
'
' Revision 1.9  2007/06/21 00:14:37  joeh
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

Public Class cRelativeFlows
    Private Shared m_RelativeFlowsInstnace As cRelativeFlows
    Private m_NetworkManager As cNetworkManager
    'Private m_Panel As Windows.Forms.Panel   
    Private Shared m_Panel As Windows.Forms.Panel   '???

    Public Shared Function GetInstance(ByVal NetworkManager As cNetworkManager, ByVal Panel As Windows.Forms.Panel) As cRelativeFlows
        m_Panel = Panel  '???

        If m_RelativeFlowsInstnace Is Nothing Then m_RelativeFlowsInstnace = New cRelativeFlows(NetworkManager, Panel)
        Return m_RelativeFlowsInstnace
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
        DataGrid.Rows(0).DefaultCellStyle.WrapMode = DataGridViewTriState.True
        DataGrid.Rows(0).DefaultCellStyle.BackColor = Drawing.Color.MintCream
        DataGrid.Rows(0).Frozen = True
        DataGrid.Rows(0).Height = FIRST_ROW_HEIGHT

        ReDim strRowContent(DataGrid.Columns.Count)
        strRowContent(0) = ""
        strRowContent(1) = My.Resources.COL_HDR_GRP_NAME_TRP_LVL
        For j As Integer = 1 To m_NetworkManager.nTrophicLevels
            strRowContent(j + 1) = CRoman(j)
        Next
        DataGrid.Rows(0).SetValues(strRowContent)
        DataGrid.Rows(0).Visible = True

        For i As Integer = 1 To m_NetworkManager.nGroups
            strRowContent(0) = CStr(i)
            strRowContent(1) = m_NetworkManager.GroupName(i)
            For j As Integer = 1 To m_NetworkManager.nTrophicLevels
                strRowContent(j + 1) = (m_NetworkManager.RelativeFlow(i, j)).ToString("F4")
            Next
            'DataGrid.Rows.Add(strary)
            DataGrid.Rows(i).SetValues(strRowContent)
            DataGrid.Rows(i).Visible = True

            'DataGrid.Rows(i - 1).HeaderCell.Value = CStr(i)
            'DataGrid.Rows(i - 1).HeaderCell.Style.BackColor = Drawing.Color.Beige
        Next
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

        m_Panel.AutoScroll = False
        LogoPanel.Visible = False
        GraphPane.Visible = False
        DataGrid.ReadOnly = True
        DataGrid.Visible = True
        'DataGrid.RowCount = 1
        DataGrid.ColumnCount = iNumTrophicLevels + 2

        SetGridColumnPropertyDefault(DataGrid)

        DataGrid.Columns(0).DefaultCellStyle.BackColor = Drawing.Color.MintCream
        DataGrid.Columns(0).Frozen = True
        DataGrid.Columns(0).Width = ID_COL_WIDTH '55

        DataGrid.Columns(1).DefaultCellStyle.Alignment = Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGrid.Columns(1).DefaultCellStyle.BackColor = Drawing.Color.MintCream
        DataGrid.Columns(1).Frozen = True
        DataGrid.Columns(1).Width = GRP_NAME_COL_WIDTH

        'DataGrid.Rows(i - 1).HeaderCell.Value = CStr(i)
        'DataGrid.Rows(i - 1).HeaderCell.Style.BackColor = Drawing.Color.Beige
    End Sub

    Private Sub RemoveToolStrip()
        'Dim ToolStrip As Windows.Forms.ToolStrip = _
        '    CType(m_Panel.Controls("tsNetworkAnalysis"), Windows.Forms.ToolStrip)
        Dim DataGrid As Windows.Forms.DataGridView = _
            CType(m_Panel.Controls("dgvNetworkAnalysis"), Windows.Forms.DataGridView)

        'If Not ToolStrip Is Nothing Then
        m_Panel.Controls.RemoveByKey("tsNetworkAnalysis")
        DataGrid.Dock = Windows.Forms.DockStyle.Fill
        'End If
    End Sub

End Class
