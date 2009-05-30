'==============================================================================
'
' $Log: cImpactData.vb,v $
' Revision 1.10  2009/05/30 00:00:53  jeroens
' Toolstrip usage centralized
'
' Revision 1.9  2009/05/19 13:41:12  jeroens
' Content manager derived pages will take care of updating NA run state
'
' Revision 1.8  2009/05/02 01:51:28  jeroens
' Updated to cControlManager FN name change
'
' Revision 1.7  2009/05/01 17:42:59  jeroens
' Inherited from cContentManager
'
' Revision 1.6  2009/04/17 01:07:05  joeh
' Make MixedTrophicImpactUC not visible when needed
'
' Revision 1.5  2009/04/15 23:37:39  joeh
' Add "Imports System.Windows.Forms" statement
'
' Revision 1.4  2009/04/15 18:14:54  joeh
' Set m_Panel.AutoScroll = False
'
' Revision 1.3  2008/12/04 01:14:15  joeh
' Add ucPlotOfMixedTrophicImpact
'
' Revision 1.2  2008/11/25 20:55:41  joeh
' Copy and paste in cells of data grid view
'
' Revision 1.1  2008/09/26 07:30:54  sherman
' --== DELETED HISTORY ==--
'
'==============================================================================
Option Strict On
Option Explicit On

Imports System.Windows.Forms
Imports ZedGraph

Public Class cImpactData
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

        Dim astrRowContent() As String

        SetUpGridColumn(NetworkManager.nGroups, NetworkManager.nFleets)

        'Set up grid rows
        Grid.RowHeadersVisible = False
        Grid.RowCount = NetworkManager.nGroups + NetworkManager.nFleets + 1
        Grid.Rows(0).DefaultCellStyle.WrapMode = DataGridViewTriState.True
        Grid.Rows(0).DefaultCellStyle.BackColor = Drawing.Color.MintCream
        Grid.Rows(0).Frozen = True
        Grid.Rows(0).Height = FIRST_ROW_HEIGHT

        ReDim astrRowContent(Grid.Columns.Count)
        astrRowContent(0) = ""
        astrRowContent(1) = My.Resources.COL_HDR_IMPACTING_IMPACTED
        For intIndex As Integer = 1 To NetworkManager.nGroups
            astrRowContent(intIndex + 1) = NetworkManager.GroupName(intIndex)
        Next
        For intIndex As Integer = 1 To NetworkManager.nFleets
            astrRowContent(NetworkManager.nGroups + intIndex + 1) = NetworkManager.FleetName(intIndex)
        Next
        Grid.Rows(0).SetValues(astrRowContent)
        Grid.Rows(0).Visible = True

        For i As Integer = 1 To NetworkManager.nGroups + NetworkManager.nFleets
            astrRowContent(0) = CStr(i)
            If i <= NetworkManager.nGroups Then
                astrRowContent(1) = NetworkManager.GroupName(i)
            Else
                astrRowContent(1) = NetworkManager.FleetName(i - NetworkManager.nGroups)
            End If
            For j As Integer = 1 To NetworkManager.nGroups + NetworkManager.nFleets
                astrRowContent(j + 1) = Me.StyleGuide.FormatNumber(NetworkManager.MixedTrophicImpacts(i, j))
            Next
            Grid.Rows(i).SetValues(astrRowContent)
            Grid.Rows(i).Visible = True
        Next
        Grid.ClearSelection()
    End Sub

    Private Sub SetUpGridColumn(ByVal iNumGroups As Integer, ByVal iNumFleets As Integer)

        Grid.ReadOnly = True
        'DataGrid.RowCount = 1
        Grid.ColumnCount = iNumGroups + iNumFleets + 2

        SetGridColumnPropertyDefault(Grid)

        Grid.Columns(0).DefaultCellStyle.BackColor = Drawing.Color.MintCream
        Grid.Columns(0).Frozen = True
        Grid.Columns(0).Width = ID_COL_WIDTH

        Grid.Columns(1).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
        Grid.Columns(1).DefaultCellStyle.BackColor = Drawing.Color.MintCream
        Grid.Columns(1).Frozen = True
        Grid.Columns(1).Width = GRP_NAME_COL_WIDTH

    End Sub

End Class
