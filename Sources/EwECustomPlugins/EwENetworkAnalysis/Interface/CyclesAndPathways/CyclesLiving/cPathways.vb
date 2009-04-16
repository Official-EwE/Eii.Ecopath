'==============================================================================
'
' $Log: cPathways.vb,v $
' Revision 1.4  2009/04/16 00:11:57  joeh
' Add "Imports System.Windows.Forms" statement
'
' Revision 1.3  2009/04/15 18:14:56  joeh
' Set m_Panel.AutoScroll = False
'
' Revision 1.2  2008/11/25 23:44:07  joeh
' Copy and paste in cells of data grid view
'
' Revision 1.1  2008/09/26 07:30:48  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.14  2008/06/25 01:53:42  joeh
' Ecosim NA indice plots are displayed in the same form where we have the NA tree view - Take 2
'
' Revision 1.13  2008/06/24 18:08:39  joeh
' Ecosim NA indice plots are displayed in the same form where  we have the NA tree view - Take 2
'
' Revision 1.12  2007/06/28 19:20:43  joeh
' Switch to wait cursor when displaying data
'
' Revision 1.11  2007/06/22 19:12:47  joeh
' Modify GetInstance()
'
' Revision 1.10  2007/06/22 00:35:31  joeh
' Add Option Strict On and Option Explicit On
'
' Revision 1.9  2007/06/21 23:49:36  joeh
' Move hard coded strings into the resource file
'
' Revision 1.8  2007/06/21 00:14:39  joeh
' Rename SetUpPanel() to DisplayData()
'
' Revision 1.7  2007/06/20 18:13:59  joeh
' add header to the top of the file so that CVS will log the file with every update
'
'
'==============================================================================
Option Strict On
Option Explicit On

Imports System.Windows.Forms
Imports ZedGraph

Namespace CyclesLiving

    Public Class cPathways
        Private Shared m_PathwaysInstnace As cPathways

        Private m_NetworkManager As cNetworkManager
        'Private m_Panel As Windows.Forms.Panel
        Private Shared m_Panel As Panel

        Public Shared Function GetInstance(ByVal NetworkManager As cNetworkManager, ByVal Panel As Windows.Forms.Panel) As cPathways
            m_Panel = Panel

            If m_PathwaysInstnace Is Nothing Then m_PathwaysInstnace = New cPathways(NetworkManager, Panel)
            Return m_PathwaysInstnace
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
            RemoveToolStrip()

            SetUpGridColumn()

            'Set up grid rows
            DataGrid.RowHeadersVisible = False

            ReDim strRowContent(DataGrid.Columns.Count)
            m_NetworkManager.FindPathwaysCycles()
            If m_NetworkManager.PathWays.Count > 0 Then
                DataGrid.RowCount = m_NetworkManager.PathWays.Count + 1
                DataGrid.Rows(0).DefaultCellStyle.WrapMode = DataGridViewTriState.True
                DataGrid.Rows(0).DefaultCellStyle.BackColor = Drawing.Color.MintCream
                DataGrid.Rows(0).Frozen = True
                DataGrid.Rows(0).Height = FIRST_ROW_HEIGHT

                strRowContent(0) = My.Resources.COL_HDR_PATH_NUM
                strRowContent(1) = My.Resources.COL_HDR_CYC
                DataGrid.Rows(0).SetValues(strRowContent)
                DataGrid.Rows(0).Visible = True

                For intPathwayIndex As Integer = 0 To m_NetworkManager.PathWays.Count - 1
                    strRowContent(0) = CStr(intPathwayIndex + 1)
                    strRowContent(1) = CStr(m_NetworkManager.PathWays.Item(intPathwayIndex))
                    DataGrid.Rows(intPathwayIndex + 1).SetValues(strRowContent)
                    DataGrid.Rows(intPathwayIndex + 1).Visible = True
                Next
            Else
                DataGrid.RowCount = 2
                DataGrid.Rows(0).DefaultCellStyle.WrapMode = DataGridViewTriState.True
                DataGrid.Rows(0).DefaultCellStyle.BackColor = Drawing.Color.MintCream
                DataGrid.Rows(0).Frozen = True
                DataGrid.Rows(0).Height = FIRST_ROW_HEIGHT

                strRowContent(0) = My.Resources.COL_HDR_PATH_NUM
                strRowContent(1) = My.Resources.COL_HDR_CYC
                DataGrid.Rows(0).SetValues(strRowContent)
                DataGrid.Rows(0).Visible = True

                strRowContent(0) = My.Resources.ROW_HDR_NO_PATH_FOUND
                strRowContent(1) = ""
                DataGrid.Rows(1).SetValues(strRowContent)
                DataGrid.Rows(1).Visible = True
            End If
            DataGrid.ClearSelection()
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

            DataGrid.Columns(1).Width = 660
            DataGrid.Columns(1).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
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
