'==============================================================================
'
' $Log: cCatchByTrophicLevel.vb,v $
' Revision 1.1  2009/06/15 14:15:27  jeroens
' Flattened directory structure
'
' Revision 1.12  2009/06/03 19:26:27  jeroens
' Uses EwEUtils ToRoman
'
' Revision 1.11  2009/05/30 00:00:48  jeroens
' Toolstrip usage centralized
'
' Revision 1.10  2009/05/28 12:37:03  jeroens
' Properly named utility classes StyleGuide and ZedGraphHelper
'
' Revision 1.9  2009/05/19 13:41:06  jeroens
' Content manager derived pages will take care of updating NA run state
'
' Revision 1.8  2009/05/02 01:51:31  jeroens
' Updated to cControlManager FN name change
'
' Revision 1.7  2009/05/01 17:42:52  jeroens
' Inherited from cContentManager
'
' Revision 1.6  2009/04/28 19:00:26  jeroens
' Revamped to be able to use styleguide hide groups, rather than an isolated hidegroups interface
'
' Revision 1.5  2009/04/17 01:07:00  joeh
' Make MixedTrophicImpactUC not visible when needed
'
' Revision 1.4  2009/04/15 23:22:26  joeh
' Add "Imports System.Windows.Forms" statement
'
' Revision 1.3  2009/04/15 18:14:48  joeh
' Set m_Panel.AutoScroll = False
'
' Revision 1.2  2008/11/25 20:55:40  joeh
' Copy and paste in cells of data grid view
'
' Revision 1.1  2008/09/26 07:30:55  sherman
' --== DELETED HISTORY ==--
'
'==============================================================================

Option Strict On
Option Explicit On

Imports EwECore
Imports EwEUtils.Utilities
Imports ZedGraph
Imports System.Windows.Forms
Imports ScientificInterfaceShared.Style

Public Class cCatchByTrophicLevel
    Inherits cContentManager

    Public Sub New()
        '
    End Sub

    Public Overrides Function Attach(ByVal manager As cNetworkManager, _
                                     ByVal datagrid As DataGridView, _
                                     ByVal graph As ZedGraphControl, _
                                     ByVal plot As ucPlot, _
                                     ByVal toolstrip As ToolStrip) As Boolean
        Dim bSucces As Boolean = MyBase.Attach(manager, datagrid, graph, plot, toolstrip)
        Me.Grid.Visible = bSucces
        Return bSucces
    End Function

    Public Overrides Sub DisplayData()

        Dim strRowContent() As String
        Dim core As cCore = cCore.GetInstance
        Dim bShowItem As Boolean = True
        Dim CatchGroupsShown() As Single

        SetUpGridColumn()

        'Set up grid rows
        Grid.RowHeadersVisible = False
        Grid.RowCount = NetworkManager.nTrophicLevels + 1
        Grid.Rows(0).DefaultCellStyle.WrapMode = DataGridViewTriState.True
        Grid.Rows(0).DefaultCellStyle.BackColor = Drawing.Color.MintCream
        Grid.Rows(0).Frozen = True
        Grid.Rows(0).Height = FIRST_ROW_HEIGHT

        'Calculate non-hidden data
        ReDim CatchGroupsShown(NetworkManager.nGroups)
        For i As Integer = 1 To NetworkManager.nGroups
            ' bShowItem = sg.GroupVisible(i)
            If bShowItem Then
                For j As Integer = 1 To NetworkManager.nTrophicLevels
                    If NetworkManager.RelativeFlow(i, j) = 0 Then
                    Else
                        If i <= core.nLivingGroups Then
                            CatchGroupsShown(j) = CatchGroupsShown(j) + NetworkManager.RelativeFlow(i, j) * NetworkManager.CatchByGroup(i)
                        End If
                    End If
                Next
            End If
        Next

        ReDim strRowContent(Grid.Columns.Count)
        strRowContent(0) = My.Resources.COL_HDR_TRP_LVL
        strRowContent(1) = My.Resources.COL_HDR_TOTAL_TKM2YR
        strRowContent(2) = My.Resources.COL_HDR_NONHIDDEN
        Grid.Rows(0).SetValues(strRowContent)
        Grid.Rows(0).Visible = True

        For i As Integer = NetworkManager.nTrophicLevels To 1 Step -1
            strRowContent(0) = StringUtils.ToRoman(i)
            strRowContent(1) = Me.StyleGuide.FormatNumber(NetworkManager.CatchByTrophicLevel(i))
            strRowContent(2) = Me.StyleGuide.FormatNumber(CatchGroupsShown(i))
            Grid.Rows(NetworkManager.nTrophicLevels - i + 1).SetValues(strRowContent)
            Grid.Rows(NetworkManager.nTrophicLevels - i + 1).Visible = True
        Next

        Grid.ClearSelection()
    End Sub

    Private Sub SetUpGridColumn()

        'DataGrid.RowCount = 1
        Grid.ColumnCount = 3

        SetGridColumnPropertyDefault(Grid)

        Grid.Columns(0).Frozen = True
        Grid.Columns(0).DefaultCellStyle.BackColor = Drawing.Color.MintCream

    End Sub

End Class
