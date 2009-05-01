'==============================================================================
'
' $Log: cRelativeFlows.vb,v $
' Revision 1.7  2009/05/01 17:42:51  jeroens
' Inherited from cContentManager
'
' Revision 1.6  2009/04/17 01:06:59  joeh
' Make MixedTrophicImpactUC not visible when needed
'
' Revision 1.5  2009/04/15 23:22:25  joeh
' Add "Imports System.Windows.Forms" statement
'
' Revision 1.4  2009/04/15 18:14:47  joeh
' Set m_Panel.AutoScroll = False
'
' Revision 1.3  2008/12/04 01:14:16  joeh
' Add ucPlotOfMixedTrophicImpact
'
' Revision 1.2  2008/11/25 05:47:34  joeh
' Copy and paste in cells of data grid view
'
' Revision 1.1  2008/09/26 07:30:56  sherman
' --== DELETED HISTORY ==--
'
'==============================================================================
Option Strict On
Option Explicit On

Imports System.Windows.Forms
Imports ZedGraph

Public Class cRelativeFlows
    Inherits cContentManager

    Public Sub New()
    End Sub

    Public Overrides Sub Attach(ByVal manager As cNetworkManager, _
                                  ByVal datagrid As DataGridView, _
                                  ByVal graph As ZedGraphControl, _
                                  ByVal plot As ucPlot)
        MyBase.Attach(manager, datagrid, graph, plot)
        Me.DataGrid.Visible = True
    End Sub

    Public Overrides Sub DisplayData()

        Dim strRowContent() As String

        SetUpGridColumn(NetworkManager.nTrophicLevels)

        'Set up grid rows
        DataGrid.RowHeadersVisible = False
        DataGrid.RowCount = NetworkManager.nGroups + 1
        DataGrid.Rows(0).DefaultCellStyle.WrapMode = DataGridViewTriState.True
        DataGrid.Rows(0).DefaultCellStyle.BackColor = Drawing.Color.MintCream
        DataGrid.Rows(0).Frozen = True
        DataGrid.Rows(0).Height = FIRST_ROW_HEIGHT

        ReDim strRowContent(DataGrid.Columns.Count)
        strRowContent(0) = ""
        strRowContent(1) = My.Resources.COL_HDR_GRP_NAME_TRP_LVL
        For j As Integer = 1 To NetworkManager.nTrophicLevels
            strRowContent(j + 1) = CRoman(j)
        Next
        DataGrid.Rows(0).SetValues(strRowContent)
        DataGrid.Rows(0).Visible = True

        For i As Integer = 1 To NetworkManager.nGroups
            strRowContent(0) = CStr(i)
            strRowContent(1) = NetworkManager.GroupName(i)
            For j As Integer = 1 To NetworkManager.nTrophicLevels
                strRowContent(j + 1) = (NetworkManager.RelativeFlow(i, j)).ToString("F4")
            Next
            'DataGrid.Rows.Add(strary)
            DataGrid.Rows(i).SetValues(strRowContent)
            DataGrid.Rows(i).Visible = True

            'DataGrid.Rows(i - 1).HeaderCell.Value = CStr(i)
            'DataGrid.Rows(i - 1).HeaderCell.Style.BackColor = Drawing.Color.Beige
        Next
        DataGrid.ClearSelection()

    End Sub

    Private Sub SetUpGridColumn(ByVal iNumTrophicLevels As Integer)

        'DataGrid.RowCount = 1
        DataGrid.ColumnCount = iNumTrophicLevels + 2

        SetGridColumnPropertyDefault(DataGrid)

        DataGrid.Columns(0).DefaultCellStyle.BackColor = Drawing.Color.MintCream
        DataGrid.Columns(0).Frozen = True
        DataGrid.Columns(0).Width = ID_COL_WIDTH '55

        DataGrid.Columns(1).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
        DataGrid.Columns(1).DefaultCellStyle.BackColor = Drawing.Color.MintCream
        DataGrid.Columns(1).Frozen = True
        DataGrid.Columns(1).Width = GRP_NAME_COL_WIDTH

        'DataGrid.Rows(i - 1).HeaderCell.Value = CStr(i)
        'DataGrid.Rows(i - 1).HeaderCell.Style.BackColor = Drawing.Color.Beige

    End Sub

End Class
