'==============================================================================
'
' $Log: cFlowFromDetritus.vb,v $
' Revision 1.2  2008/11/25 23:14:03  joeh
' Copy and paste in cells of data grid view
'
' Revision 1.1  2008/09/26 07:30:56  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.16  2008/06/25 01:53:40  joeh
' Ecosim NA indice plots are displayed in the same form where we have the NA tree view - Take 2
'
' Revision 1.15  2008/06/24 18:08:38  joeh
' Ecosim NA indice plots are displayed in the same form where  we have the NA tree view - Take 2
'
' Revision 1.14  2007/07/09 19:44:45  joeh
' Move hard coded strings to resource file
'
' Revision 1.13  2007/07/07 00:11:03  joeh
' Decrease column width
'
' Revision 1.12  2007/06/28 19:22:09  joeh
' Switch to wait cursor when displaying data
'
' Revision 1.11  2007/06/22 19:12:45  joeh
' Modify GetInstance()
'
' Revision 1.10  2007/06/21 00:14:38  joeh
' Rename SetUpPanel() to DisplayData()
'
' Revision 1.9  2007/06/20 18:13:56  joeh
' add header to the top of the file so that CVS will log the file with every update
'
'
'==============================================================================
Option Strict On
Option Explicit On

Imports System.Windows.Forms
Imports ZedGraph

Public Class cFlowFromDetritus
    Private Shared m_FlowFromDetritusInstance As cFlowFromDetritus

    Private m_NetworkManager As cNetworkManager
    'Private m_Panel As Windows.Forms.Panel
    Private Shared m_Panel As Windows.Forms.Panel

    Public Shared Function GetInstance(ByVal NetworkManager As cNetworkManager, ByVal Panel As Windows.Forms.Panel) As cFlowFromDetritus
        m_Panel = Panel

        If m_FlowFromDetritusInstance Is Nothing Then m_FlowFromDetritusInstance = New cFlowFromDetritus(NetworkManager, Panel)
        Return m_FlowFromDetritusInstance
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
        DataGrid.RowCount = m_NetworkManager.nGroups
        DataGrid.Rows(0).DefaultCellStyle.WrapMode = DataGridViewTriState.True
        DataGrid.Rows(0).DefaultCellStyle.BackColor = Drawing.Color.MintCream
        DataGrid.Rows(0).Frozen = True
        DataGrid.Rows(0).Height = FIRST_ROW_HEIGHT

        ReDim strRowContent(DataGrid.Columns.Count)
        strRowContent(0) = ""
        strRowContent(1) = My.Resources.COL_HDR_GRP_NAME
        strRowContent(2) = ""
        DataGrid.Rows(0).SetValues(strRowContent)
        DataGrid.Rows(0).Visible = True

        For i As Integer = 1 To m_NetworkManager.nGroups - 1
            strRowContent(0) = CStr(i)
            strRowContent(1) = m_NetworkManager.GroupName(i)
            strRowContent(2) = m_NetworkManager.FlowFromDetritus(i).ToString("F4")
            DataGrid.Rows(i).SetValues(strRowContent)
            DataGrid.Rows(i).Visible = True
        Next
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

        LogoPanel.Visible = False
        GraphPane.Visible = False
        DataGrid.ReadOnly = True
        DataGrid.Visible = True
        'DataGrid.RowCount = 1
        DataGrid.ColumnCount = 3

        SetGridColumnPropertyDefault(DataGrid)

        DataGrid.Columns(0).DefaultCellStyle.BackColor = Drawing.Color.MintCream
        DataGrid.Columns(0).Frozen = True
        DataGrid.Columns(0).Width = ID_COL_WIDTH

        DataGrid.Columns(1).DefaultCellStyle.Alignment = Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGrid.Columns(1).DefaultCellStyle.BackColor = Drawing.Color.MintCream
        DataGrid.Columns(1).Frozen = True
        DataGrid.Columns(1).Width = GRP_NAME_COL_WIDTH
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
