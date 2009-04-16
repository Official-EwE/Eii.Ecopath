'==============================================================================
'
' $Log: cPathways.vb,v $
' Revision 1.7  2009/04/16 00:11:59  joeh
' Add "Imports System.Windows.Forms" statement
'
' Revision 1.6  2009/04/15 18:14:58  joeh
' Set m_Panel.AutoScroll = False
'
' Revision 1.5  2009/04/14 18:21:12  joeh
' Add separator to tool strip
'
' Revision 1.4  2009/04/09 20:04:49  joeh
' Add "Bar graph" button to plot bar graph for MTI
'
' Revision 1.3  2008/11/28 01:58:33  joeh
' Implement new MTI plot and save MTI plot as emf file
'
' Revision 1.2  2008/11/25 23:14:04  joeh
' Copy and paste in cells of data grid view
'
' Revision 1.1  2008/09/26 07:30:50  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.16  2008/06/25 01:53:43  joeh
' Ecosim NA indice plots are displayed in the same form where we have the NA tree view - Take 2
'
' Revision 1.15  2008/06/24 18:08:40  joeh
' Ecosim NA indice plots are displayed in the same form where  we have the NA tree view - Take 2
'
' Revision 1.14  2008/06/24 00:52:28  joeh
' Ecosim NA indice plots are no longer displayed in a pop up form, rather they are displayed in the same form where  we have the NA tree view
'
' Revision 1.13  2007/06/28 19:20:44  joeh
' Switch to wait cursor when displaying data
'
' Revision 1.12  2007/06/22 19:12:48  joeh
' Modify GetInstance()
'
' Revision 1.11  2007/06/22 00:35:31  joeh
' Add Option Strict On and Option Explicit On
'
' Revision 1.10  2007/06/21 23:49:37  joeh
' Move hard coded strings into the resource file
'
' Revision 1.9  2007/06/21 00:14:40  joeh
' Rename SetUpPanel() to DisplayData()
'
' Revision 1.8  2007/06/20 18:13:59  joeh
' add header to the top of the file so that CVS will log the file with every update
'
'
'==============================================================================
Option Strict On
Option Explicit On

Imports System.Windows.Forms
Imports EwECore
Imports ZedGraph

Namespace TL1ToPreyToConsumer

    Public Class cPathways
        Public Event AddToolStrip()

        Private Shared m_PathwaysInstnace As cPathways

        Private m_NumGroups As Integer
        Private m_GroupNames() As String
        Private m_NetworkManager As cNetworkManager
        'Private m_Panel As Windows.Forms.Panel
        Private Shared m_Panel As Panel

        Public Shared Function GetInstance(ByVal NetworkManager As cNetworkManager, ByVal Panel As Windows.Forms.Panel) As cPathways
            m_Panel = Panel

            If m_PathwaysInstnace Is Nothing Then m_PathwaysInstnace = New cPathways(NetworkManager, Panel)
            Return m_PathwaysInstnace
        End Function

        Private Sub New()
            Dim core As cCore = cCore.GetInstance

            m_NumGroups = core.nGroups
        End Sub

        Private Sub New(ByVal NetworkManager As cNetworkManager, ByVal Panel As Windows.Forms.Panel)
            Me.New()

            m_NetworkManager = NetworkManager
            m_Panel = Panel

            ReDim m_GroupNames(m_NumGroups - 1)
            For intIndex As Integer = 0 To m_NumGroups - 1
                m_GroupNames(intIndex) = m_NetworkManager.GroupName(intIndex + 1)
            Next
        End Sub

        Public Sub DisplayData()
            Cursor.Current = Cursors.WaitCursor
            SetUpGridColumn()

            SetUpToolStrip()

            'Set up grid rows
            'SetUpGridRow() will be triggered when SetUpToolStrip() is executed
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

        Private Sub SetUpToolStrip()
            Dim ToolStrip As ToolStrip = _
                CType(m_Panel.Controls("tsNetworkAnalysis"), ToolStrip)
            Dim ToolStripLabel1 As ToolStripLabel = New ToolStripLabel
            Dim ToolStripLabel2 As ToolStripLabel = New ToolStripLabel
            Dim ToolStripLabel3 As ToolStripLabel = New ToolStripLabel
            Dim ToolStripCombo1 As ToolStripComboBox = New ToolStripComboBox
            Dim ToolStripCombo2 As ToolStripComboBox = New ToolStripComboBox
            Dim ToolStripPrgBar As ToolStripProgressBar = New ToolStripProgressBar
            Dim ToolStripButton1 As ToolStripButton = New ToolStripButton
            Dim ToolStripButton2 As ToolStripButton = New ToolStripButton
            Dim ToolStripButton3 As ToolStripButton = New ToolStripButton
            Dim ToolStripButton4 As ToolStripButton = New ToolStripButton
            Dim ToolStripButton5 As ToolStripButton = New ToolStripButton
            Dim ToolStripSep1 As ToolStripSeparator = New ToolStripSeparator

            RemoveToolStrip()
            RaiseEvent AddToolStrip()

            ToolStrip = CType(m_Panel.Controls("tsNetworkAnalysis"), ToolStrip)
            ToolStripLabel1 = CType(ToolStrip.Items("tslblSelection1"), ToolStripLabel)
            ToolStripLabel2 = CType(ToolStrip.Items("tslblSelection2"), ToolStripLabel)
            ToolStripLabel3 = CType(ToolStrip.Items("tslblProgressBar"), ToolStripLabel)
            ToolStripCombo1 = CType(ToolStrip.Items("tscmbSelection1"), ToolStripComboBox)
            ToolStripCombo2 = CType(ToolStrip.Items("tscmbSelection2"), ToolStripComboBox)
            ToolStripPrgBar = CType(ToolStrip.Items("tspgbProgressBar"), ToolStripProgressBar)
            ToolStripButton1 = CType(ToolStrip.Items("tsbtnCancel"), ToolStripButton)
            ToolStripButton2 = CType(ToolStrip.Items("tsbtnOutputIndicesCSV"), ToolStripButton)
            ToolStripButton3 = CType(ToolStrip.Items("tsbtnOutputGraphEMF"), ToolStripButton)
            ToolStripButton4 = CType(ToolStrip.Items("tsbtnPrintGraph"), ToolStripButton)
            ToolStripButton5 = CType(ToolStrip.Items("tsbtnGraphMTI"), ToolStripButton)
            ToolStripSep1 = CType(ToolStrip.Items("tssepGraphMTI"), ToolStripSeparator)

            ToolStripLabel1.Visible = True
            ToolStripLabel1.Text = My.Resources.LBL_PATH_TO
            ToolStripCombo1.Visible = True
            ToolStripCombo1.Items.Clear()
            For intIndex As Integer = 0 To m_NumGroups - 1 - 4
                ToolStripCombo1.Items.Add(CStr(intIndex + 1) + ", " + m_GroupNames(intIndex))
            Next
            'This will or will NOT trigger SetUpGridRow()
            ToolStripCombo1.Text = CStr(1) + ", " + m_GroupNames(0)

            ToolStripLabel2.Visible = True
            ToolStripLabel2.Text = My.Resources.LBL_PATH_VIA
            ToolStripCombo2.Visible = True
            ToolStripCombo2.Items.Clear()
            For intIndex As Integer = 0 To m_NumGroups - 1 - 4
                ToolStripCombo2.Items.Add(CStr(intIndex + 1) + ", " + m_GroupNames(intIndex))
            Next
            'This will trigger SetUpGridRow()
            ToolStripCombo2.Text = CStr(1) + ", " + m_GroupNames(0)

            ToolStripLabel3.Visible = False
            ToolStripPrgBar.Visible = False
            ToolStripButton1.Visible = False
            ToolStripButton2.Visible = False
            ToolStripButton3.Visible = False
            ToolStripButton4.Visible = False
            ToolStripButton5.Visible = False

            ToolStripSep1.Visible = False

            ToolStrip.Refresh()
        End Sub

        Public Sub SetUpGridRow(ByVal intSelection1 As Integer, ByVal intSelection2 As Integer)
            Dim DataGrid As DataGridView = _
                CType(m_Panel.Controls("dgvNetworkAnalysis"), DataGridView)
            Dim strRowContent() As String

            DataGrid.RowHeadersVisible = False

            ReDim strRowContent(DataGrid.Columns.Count)
            m_NetworkManager.FindPathwaysToConsumerViaPrey(intSelection1, intSelection2)
            If m_NetworkManager.PathWays.Count > 0 Then
                DataGrid.RowCount = m_NetworkManager.PathWays.Count + 1
                DataGrid.Rows(0).DefaultCellStyle.WrapMode = DataGridViewTriState.True
                DataGrid.Rows(0).DefaultCellStyle.BackColor = Drawing.Color.MintCream
                DataGrid.Rows(0).Frozen = True
                DataGrid.Rows(0).Height = FIRST_ROW_HEIGHT

                strRowContent(0) = My.Resources.COL_HDR_PATH_NUM
                strRowContent(1) = My.Resources.COL_HDR_PATH_VIA_PREY
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
                strRowContent(1) = My.Resources.COL_HDR_PATH_VIA_PREY
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
