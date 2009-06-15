'==============================================================================
'
' $Log: cBiomassByTrophicLevel.vb,v $
' Revision 1.1  2009/06/15 14:15:27  jeroens
' Flattened directory structure
'
' Revision 1.13  2009/06/06 02:00:19  jeroens
' Implemented VC request
' Added DisplayGroups
'
' Revision 1.12  2009/06/03 19:26:26  jeroens
' Uses EwEUtils ToRoman
'
' Revision 1.11  2009/05/30 00:00:48  jeroens
' Toolstrip usage centralized
'
' Revision 1.10  2009/05/28 12:37:02  jeroens
' Properly named utility classes StyleGuide and ZedGraphHelper
'
' Revision 1.9  2009/05/19 13:41:07  jeroens
' Content manager derived pages will take care of updating NA run state
'
' Revision 1.8  2009/05/02 01:51:30  jeroens
' Updated to cControlManager FN name change
'
' Revision 1.7  2009/05/01 17:42:52  jeroens
' Inherited from cContentManager
'
' Revision 1.6  2009/04/28 19:00:26  jeroens
' Revamped to be able to use styleguide hide groups, rather than an isolated hidegroups interface
'
' Revision 1.5  2009/04/17 01:06:59  joeh
' Make MixedTrophicImpactUC not visible when needed
'
' Revision 1.4  2009/04/15 23:22:26  joeh
' Add "Imports System.Windows.Forms" statement
'
' Revision 1.3  2009/04/15 18:14:47  joeh
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

Public Class cBiomassByTrophicLevel
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
        Me.Toolstrip.Visible = bSucces
        Me.ToolstripShowDisplayGroups(bSucces)
        Return bSucces
    End Function

    Public Overrides Sub DisplayData()

        Dim astrRowContent() As String

        Dim core As cCore = cCore.GetInstance
        Dim bShowItem As Boolean = True
        Dim asBiomassGroupsShown() As Single
        Dim asMassDetritusShown() As Single

        SetUpGridColumn()

        'Set up grid rows
        Grid.RowHeadersVisible = False
        Grid.RowCount = NetworkManager.nTrophicLevels + 1
        Grid.Rows(0).DefaultCellStyle.WrapMode = DataGridViewTriState.True
        Grid.Rows(0).DefaultCellStyle.BackColor = Drawing.Color.MintCream
        Grid.Rows(0).Frozen = True
        Grid.Rows(0).Height = FIRST_ROW_HEIGHT

        'Calculate non-hidden data
        ReDim asBiomassGroupsShown(NetworkManager.nGroups)
        ReDim asMassDetritusShown(NetworkManager.nGroups)

        For i As Integer = 1 To NetworkManager.nGroups

            If Me.StyleGuide.GroupVisible(i) Then
                For j As Integer = 1 To NetworkManager.nTrophicLevels
                    If NetworkManager.RelativeFlow(i, j) = 0 Then
                    Else
                        If i <= core.nLivingGroups Then
                            asBiomassGroupsShown(j) += NetworkManager.RelativeFlow(i, j) * NetworkManager.BiomassByGroup(i)
                        Else
                            asMassDetritusShown(j) += NetworkManager.RelativeFlow(i, j) * NetworkManager.BiomassByGroup(i)
                        End If
                    End If
                Next
            End If
        Next

        ReDim astrRowContent(Grid.Columns.Count)
        astrRowContent(0) = My.Resources.COL_HDR_TRP_LVL
        astrRowContent(1) = My.Resources.COL_HDR_LIVING_TKM2
        astrRowContent(2) = My.Resources.COL_HDR_DETRITUS_TKM2
        astrRowContent(3) = My.Resources.COL_HDR_TOTAL_TKM2
        astrRowContent(4) = My.Resources.COL_HDR_NONHIDDEN
        Grid.Rows(0).SetValues(astrRowContent)
        Grid.Rows(0).Visible = True

        For i As Integer = NetworkManager.nTrophicLevels To 1 Step -1
            astrRowContent(0) = StringUtils.ToRoman(i)
            astrRowContent(1) = Me.StyleGuide.FormatNumber(asBiomassGroupsShown(i))
            If i = 1 Then
                astrRowContent(2) = Me.StyleGuide.FormatNumber(asMassDetritusShown(i))
            Else
                astrRowContent(2) = ""
            End If
            astrRowContent(3) = Me.StyleGuide.FormatNumber(asBiomassGroupsShown(i) + asMassDetritusShown(i))
            astrRowContent(4) = Me.StyleGuide.FormatNumber(NetworkManager.BiomassByTrophicLevel(i) + NetworkManager.DetritusByTrophicLevel(i))
            Grid.Rows(NetworkManager.nTrophicLevels - i + 1).SetValues(astrRowContent)
            Grid.Rows(NetworkManager.nTrophicLevels - i + 1).Visible = True
        Next

        Grid.ClearSelection()
    End Sub

    Private Sub SetUpGridColumn()

        ' JS: add columns Living, detritus

        Grid.ReadOnly = True
        'DataGrid.RowCount = 1
        Grid.ColumnCount = 5

        SetGridColumnPropertyDefault(Grid)

        Grid.Columns(0).Frozen = True
        Grid.Columns(0).DefaultCellStyle.BackColor = Drawing.Color.MintCream

    End Sub

End Class
