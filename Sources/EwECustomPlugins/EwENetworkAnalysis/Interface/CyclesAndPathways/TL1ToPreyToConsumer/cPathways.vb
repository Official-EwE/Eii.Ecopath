'==============================================================================
'
' $Log: cPathways.vb,v $
' Revision 1.11  2009/05/19 13:41:10  jeroens
' Content manager derived pages will take care of updating NA run state
'
' Revision 1.10  2009/05/02 01:51:26  jeroens
' Updated to cControlManager FN name change
'
' Revision 1.9  2009/05/01 17:43:06  jeroens
' Inherited from cContentManager
'
' Revision 1.8  2009/04/17 01:07:08  joeh
' Make MixedTrophicImpactUC not visible when needed
'
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
'==============================================================================

Option Strict On
Option Explicit On

Imports System.Windows.Forms
Imports EwECore
Imports ZedGraph

Namespace TL1ToPreyToConsumer

    Public Class cPathways
        Inherits cContentManager

        Private m_bInUpdate As Boolean = False

        Public Sub New()
        End Sub

        Public Overrides Function Attach(ByVal manager As cNetworkManager, _
                                      ByVal datagrid As DataGridView, _
                                      ByVal graph As ZedGraphControl, _
                                      ByVal plot As ucPlot) As Boolean
            Dim bSucces As Boolean = MyBase.Attach(manager, datagrid, graph, plot)
            Me.Grid.Visible = bSucces
            Return bSucces
        End Function

        Public Overrides Sub DisplayData()

            Grid.ColumnCount = 2

            SetGridColumnPropertyDefault(Grid)

            Grid.Columns(0).Frozen = True
            Grid.Columns(0).DefaultCellStyle.BackColor = Drawing.Color.MintCream

            Grid.Columns(1).Width = 660
            Grid.Columns(1).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft

        End Sub

        Public Overrides Function RequiresToolstrip() As Boolean
            Return True
        End Function

        Public Overrides Sub SetUpToolStrip(ByVal ts As ToolStrip)

            MyBase.SetupToolstrip(ts)

            Dim tslbl1 As ToolStripLabel = DirectCast(ts.Items("tslblSelection1"), ToolStripLabel)
            Dim tslbl2 As ToolStripLabel = DirectCast(ts.Items("tslblSelection2"), ToolStripLabel)
            Dim tscmb1 As ToolStripComboBox = DirectCast(ts.Items("tscmbSelection1"), ToolStripComboBox)
            Dim tscmb2 As ToolStripComboBox = DirectCast(ts.Items("tscmbSelection2"), ToolStripComboBox)

            tslbl1.Visible = True
            tslbl1.Text = My.Resources.LBL_PATH_TO
            tscmb1.Visible = True
            tscmb1.Items.Clear()

            tslbl2.Visible = True
            tslbl2.Text = My.Resources.LBL_PATH_VIA
            tscmb2.Visible = True
            tscmb2.Items.Clear()

            ' JS 01May09: Should '4' not be 'Me.NetworkManager.nDetritusGroups'?
            For iGroup As Integer = 1 To Me.NetworkManager.nGroups - 4
                tscmb1.Items.Add(String.Format(My.Resources.LABEL_INDEXED, iGroup, Me.NetworkManager.GroupName(iGroup)))
                tscmb2.Items.Add(String.Format(My.Resources.LABEL_INDEXED, iGroup, Me.NetworkManager.GroupName(iGroup)))
            Next

            ts.Refresh()

            ' Suppress the first SelectedIndex-caused update
            Me.m_bInUpdate = True
            tscmb1.SelectedIndex = 0
            Me.m_bInUpdate = False
            ' Yo!
            tscmb2.SelectedIndex = 0

        End Sub

        Public Overrides Sub UpdateData(ByVal iSel1 As Integer, ByVal iSel2 As Integer)
            Dim strRowContent() As String

            If Me.m_bInUpdate Then Return

            Grid.RowHeadersVisible = False

            ReDim strRowContent(Grid.Columns.Count)
            Me.NetworkManager.FindPathwaysToConsumerViaPrey(iSel1, iSel2)
            If Me.NetworkManager.PathWays.Count > 0 Then
                Grid.RowCount = Me.NetworkManager.PathWays.Count + 1
                Grid.Rows(0).DefaultCellStyle.WrapMode = DataGridViewTriState.True
                Grid.Rows(0).DefaultCellStyle.BackColor = Drawing.Color.MintCream
                Grid.Rows(0).Frozen = True
                Grid.Rows(0).Height = FIRST_ROW_HEIGHT

                strRowContent(0) = My.Resources.COL_HDR_PATH_NUM
                strRowContent(1) = My.Resources.COL_HDR_PATH_VIA_PREY
                Grid.Rows(0).SetValues(strRowContent)
                Grid.Rows(0).Visible = True

                For intPathwayIndex As Integer = 0 To Me.NetworkManager.PathWays.Count - 1
                    strRowContent(0) = CStr(intPathwayIndex + 1)
                    strRowContent(1) = CStr(Me.NetworkManager.PathWays.Item(intPathwayIndex))
                    Grid.Rows(intPathwayIndex + 1).SetValues(strRowContent)
                    Grid.Rows(intPathwayIndex + 1).Visible = True
                Next
            Else
                Grid.RowCount = 2
                Grid.Rows(0).DefaultCellStyle.WrapMode = DataGridViewTriState.True
                Grid.Rows(0).DefaultCellStyle.BackColor = Drawing.Color.MintCream
                Grid.Rows(0).Frozen = True
                Grid.Rows(0).Height = FIRST_ROW_HEIGHT

                strRowContent(0) = My.Resources.COL_HDR_PATH_NUM
                strRowContent(1) = My.Resources.COL_HDR_PATH_VIA_PREY
                Grid.Rows(0).SetValues(strRowContent)
                Grid.Rows(0).Visible = True

                strRowContent(0) = My.Resources.ROW_HDR_NO_PATH_FOUND
                strRowContent(1) = ""
                Grid.Rows(1).SetValues(strRowContent)
                Grid.Rows(1).Visible = True
            End If
            Grid.ClearSelection()
        End Sub

    End Class

End Namespace
