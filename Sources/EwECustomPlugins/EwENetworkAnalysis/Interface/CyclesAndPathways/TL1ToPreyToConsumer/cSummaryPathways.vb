'==============================================================================
'
' $Log: cSummaryPathways.vb,v $
' Revision 1.4  2009/04/16 00:11:59  joeh
' Add "Imports System.Windows.Forms" statement
'
' Revision 1.3  2009/04/15 18:14:58  joeh
' Set m_Panel.AutoScroll = False
'
' Revision 1.2  2008/11/25 23:14:04  joeh
' Copy and paste in cells of data grid view
'
' Revision 1.1  2008/09/26 07:30:50  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.17  2008/06/25 01:53:43  joeh
' Ecosim NA indice plots are displayed in the same form where we have the NA tree view - Take 2
'
' Revision 1.16  2008/06/24 18:08:40  joeh
' Ecosim NA indice plots are displayed in the same form where  we have the NA tree view - Take 2
'
' Revision 1.15  2007/06/28 19:20:18  joeh
' Switch to wait cursor when displaying data
'
' Revision 1.14  2007/06/26 21:16:58  joeh
' Add wait cursor when set up grid
'
' Revision 1.13  2007/06/22 19:12:48  joeh
' Modify GetInstance()
'
' Revision 1.12  2007/06/22 00:35:32  joeh
' Add Option Strict On and Option Explicit On
'
' Revision 1.11  2007/06/21 23:49:37  joeh
' Move hard coded strings into the resource file
'
' Revision 1.10  2007/06/21 00:14:40  joeh
' Rename SetUpPanel() to DisplayData()
'
' Revision 1.9  2007/06/20 18:14:00  joeh
' add header to the top of the file so that CVS will log the file with every update
'
'
'==============================================================================
Option Strict On
Option Explicit On

Imports System.Windows.Forms
Imports ZedGraph

Namespace TL1ToPreyToConsumer

    Public Class cSummaryPathways
        Private Shared m_SummaryPathwaysInstnace As cSummaryPathways

        Private m_NetworkManager As cNetworkManager
        'Private m_Panel As Windows.Forms.Panel
        Private Shared m_Panel As Panel

        Public Shared Function GetInstance(ByVal NetworkManager As cNetworkManager, ByVal Panel As Windows.Forms.Panel) As cSummaryPathways
            m_Panel = Panel

            If m_SummaryPathwaysInstnace Is Nothing Then m_SummaryPathwaysInstnace = New cSummaryPathways(NetworkManager, Panel)
            Return m_SummaryPathwaysInstnace
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
            Dim DataGrid As DataGridView = _
                CType(m_Panel.Controls("dgvNetworkAnalysis"), DataGridView)
            Dim strRowContent() As String

            Cursor.Current = Cursors.WaitCursor
            'RemoveToolStrip()  at the end

            SetUpGridColumn()

            'Set up grid rows
            DataGrid.RowHeadersVisible = False
            DataGrid.RowCount = 3
            DataGrid.Rows(0).DefaultCellStyle.WrapMode = DataGridViewTriState.True
            DataGrid.Rows(0).DefaultCellStyle.BackColor = Drawing.Color.MintCream
            DataGrid.Rows(0).Frozen = True
            DataGrid.Rows(0).Height = FIRST_ROW_HEIGHT

            ReDim strRowContent(DataGrid.Columns.Count)
            strRowContent(0) = My.Resources.COL_HDR_PARAM
            strRowContent(1) = My.Resources.COL_HDR_VALUE
            DataGrid.Rows(0).SetValues(strRowContent)
            DataGrid.Rows(0).Visible = True

            strRowContent(0) = My.Resources.ROW_HDR_TOTAL_NUM_PATH
            strRowContent(1) = CStr(m_NetworkManager.PathWays.Count)
            DataGrid.Rows(1).SetValues(strRowContent)
            DataGrid.Rows(1).Visible = True

            strRowContent(0) = My.Resources.ROW_HDR_MEAN_PATH_LEN
            If m_NetworkManager.PathWays.Count = 0 Then
                strRowContent(1) = My.Resources.ROW_HDR_NOT_APP
            Else
                strRowContent(1) = (m_NetworkManager.NumArrows / m_NetworkManager.PathWays.Count).ToString("F2")
            End If
            DataGrid.Rows(2).SetValues(strRowContent)
            DataGrid.Rows(2).Visible = True

            DataGrid.ClearSelection()

            RemoveToolStrip()
            Cursor.Current = Cursors.Default

        End Sub

        Private Sub SetUpGridColumn()
            Dim DataGrid As DataGridView = _
                CType(m_Panel.Controls("dgvNetworkAnalysis"), DataGridView)
            Dim GraphPane As ZedGraphControl = _
                CType(m_Panel.Controls("zgcNetworkAnalysis"), ZedGraphControl)
            Dim LogoPanel As TableLayoutPanel = _
                CType(m_Panel.Controls("tlpNetworkAnalysis"), TableLayoutPanel)

            m_Panel.AutoScroll = False
            LogoPanel.Visible = False
            GraphPane.Visible = False
            DataGrid.ReadOnly = True
            DataGrid.Visible = True
            DataGrid.ColumnCount = 2

            SetGridColumnPropertyDefault(DataGrid)

            DataGrid.Columns(0).Frozen = True
            DataGrid.Columns(0).DefaultCellStyle.BackColor = Drawing.Color.MintCream
            DataGrid.Columns(0).Width = 400
        End Sub

        Private Sub RemoveToolStrip()
            Dim ToolStrip As ToolStrip = _
                CType(m_Panel.Controls("tsNetworkAnalysis"), ToolStrip)
            Dim DataGrid As DataGridView = _
                CType(m_Panel.Controls("dgvNetworkAnalysis"), DataGridView)

            If Not ToolStrip Is Nothing Then
                m_Panel.Controls.RemoveByKey("tsNetworkAnalysis")
                DataGrid.Dock = DockStyle.Fill
            End If
        End Sub

    End Class

End Namespace

