'==============================================================================
'
' $Log: cSummaryPathways.vb,v $
' Revision 1.1  2008/09/26 07:30:49  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.17  2008/06/25 01:53:42  joeh
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
' Revision 1.12  2007/06/22 00:35:31  joeh
' Add Option Strict On and Option Explicit On
'
' Revision 1.11  2007/06/21 23:49:36  joeh
' Move hard coded strings into the resource file
'
' Revision 1.10  2007/06/21 00:14:40  joeh
' Rename SetUpPanel() to DisplayData()
'
' Revision 1.9  2007/06/20 18:13:59  joeh
' add header to the top of the file so that CVS will log the file with every update
'
'
'==============================================================================
Option Strict On
Option Explicit On

Imports System.Windows.Forms
Imports ZedGraph

Namespace TL1ToConsumer

    Public Class cSummaryPathways
        Private Shared m_SummaryPathwaysInstnace As cSummaryPathways

        Private m_NetworkManager As cNetworkManager
        'Private m_Panel As Windows.Forms.Panel
        Private Shared m_Panel As Windows.Forms.Panel

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
            Dim DataGrid As Windows.Forms.DataGridView = _
                CType(m_Panel.Controls("dgvNetworkAnalysis"), Windows.Forms.DataGridView)
            Dim strRowContent() As String

            Cursor.Current = Cursors.WaitCursor
            'RemoveToolStrip() at the end

            SetUpGridColumn()

            'Set up grid rows
            DataGrid.RowHeadersVisible = False
            DataGrid.RowCount = 2

            ReDim strRowContent(DataGrid.Columns.Count)
            strRowContent(0) = My.Resources.ROW_HDR_TOTAL_NUM_PATH
            strRowContent(1) = CStr(m_NetworkManager.PathWays.Count)
            DataGrid.Rows(0).SetValues(strRowContent)
            DataGrid.Rows(0).Visible = True

            strRowContent(0) = My.Resources.ROW_HDR_MEAN_PATH_LEN
            If m_NetworkManager.PathWays.Count = 0 Then
                strRowContent(1) = My.Resources.ROW_HDR_NOT_APP
            Else
                strRowContent(1) = (m_NetworkManager.NumArrows / m_NetworkManager.PathWays.Count).ToString("F2")
            End If
            DataGrid.Rows(1).SetValues(strRowContent)
            DataGrid.Rows(1).Visible = True

            DataGrid.ClearSelection()

            RemoveToolStrip()
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
            DataGrid.ColumnCount = 2

            SetGridColumnPropertyDefault(DataGrid)

            DataGrid.Columns(0).Frozen = True
            DataGrid.Columns(0).DefaultCellStyle.BackColor = Drawing.Color.MintCream
            DataGrid.Columns(0).Width = 400
            DataGrid.Columns(0).HeaderText = My.Resources.COL_HDR_PARAM

            DataGrid.Columns(1).HeaderText = My.Resources.COL_HDR_VALUE
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

End Namespace

