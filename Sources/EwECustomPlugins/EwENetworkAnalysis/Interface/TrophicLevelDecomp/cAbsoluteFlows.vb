'==============================================================================
'
' $Log: cAbsoluteFlows.vb,v $
' Revision 1.9  2009/05/19 13:41:10  jeroens
' Content manager derived pages will take care of updating NA run state
'
' Revision 1.8  2009/05/02 01:51:29  jeroens
' Updated to cControlManager FN name change
'
' Revision 1.7  2009/05/01 17:42:51  jeroens
' Inherited from cContentManager
'
' Revision 1.6  2009/04/17 01:06:59  joeh
' Make MixedTrophicImpactUC not visible when needed
'
' Revision 1.5  2009/04/15 23:22:25  joeh
' Add "Imports System.Windows.Forms" statement
'
' Revision 1.4  2009/04/15 18:14:46  joeh
' Set m_Panel.AutoScroll = False
'
' Revision 1.3  2008/12/04 01:14:47  joeh
' Add ucPlotOfMixedTrophicImpact
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

Imports System.Windows.Forms
Imports ZedGraph

Public Class cAbsoluteFlows
    Inherits cContentManager

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

        Dim strRowContent() As String

        SetUpGridColumn(NetworkManager.nTrophicLevels)

        'Set up grid rows
        Grid.RowHeadersVisible = False
        Grid.RowCount = NetworkManager.nGroups + 2
        Grid.Rows(0).DefaultCellStyle.WrapMode = DataGridViewTriState.True
        Grid.Rows(0).DefaultCellStyle.BackColor = Drawing.Color.MintCream
        Grid.Rows(0).Frozen = True
        Grid.Rows(0).Height = FIRST_ROW_HEIGHT
        'DataGrid.RowHeadersDefaultCellStyle.BackColor = Drawing.Color.Beige

        ReDim strRowContent(Grid.Columns.Count)
        strRowContent(0) = ""
        strRowContent(1) = My.Resources.COL_HDR_GRP_NAME_TRP_LVL
        For j As Integer = 1 To NetworkManager.nTrophicLevels
            strRowContent(j + 1) = CRoman(j)
        Next
        Grid.Rows(0).SetValues(strRowContent)
        Grid.Rows(0).Visible = True

        For i As Integer = 1 To NetworkManager.nGroups
            strRowContent(0) = CStr(i)
            strRowContent(1) = NetworkManager.GroupName(i)
            For j As Integer = 1 To NetworkManager.nTrophicLevels
                strRowContent(j + 1) = (NetworkManager.AbsoluteFlow(i, j)).ToString("F4")
            Next
            'DataGrid.Rows.Add(strary)
            Grid.Rows(i).SetValues(strRowContent)
            Grid.Rows(i).Visible = True

            'DataGrid.Rows(i - 1).HeaderCell.Value = CStr(i)
            'DataGrid.Rows(i - 1).HeaderCell.Style.BackColor = Drawing.Color.Beige
        Next

        strRowContent(0) = ""
        strRowContent(1) = My.Resources.ROW_HDR_TOTAL
        For j As Integer = 1 To NetworkManager.nTrophicLevels
            strRowContent(j + 1) = (NetworkManager.AbsoluteFlowTotal(j)).ToString("F4")
        Next
        Grid.Rows(Grid.RowCount - 1).SetValues(strRowContent)
        Grid.Rows(Grid.RowCount - 1).Visible = True
        Grid.ClearSelection()

    End Sub

    Private Sub SetUpGridColumn(ByVal iNumTrophicLevels As Integer)

        'DataGrid.RowCount = 1
        Grid.ColumnCount = iNumTrophicLevels + 2

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
