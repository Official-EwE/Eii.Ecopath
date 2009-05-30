'==============================================================================
'
' $Log: cFlowFromDetritus.vb,v $
' Revision 1.9  2009/05/30 00:00:50  jeroens
' Toolstrip usage centralized
'
' Revision 1.8  2009/05/19 13:41:05  jeroens
' Content manager derived pages will take care of updating NA run state
'
' Revision 1.7  2009/05/02 01:51:20  jeroens
' Updated to cControlManager FN name change
'
' Revision 1.6  2009/05/01 17:42:55  jeroens
' Inherited from cContentManager
'
' Revision 1.5  2009/04/17 01:07:03  joeh
' Make MixedTrophicImpactUC not visible when needed
'
' Revision 1.4  2009/04/16 00:29:49  joeh
' Add "Import.System.Windows.Forms" statement
'
' Revision 1.3  2009/04/15 18:14:51  joeh
' Set m_Panel.AutoScroll = False
'
' Revision 1.2  2008/11/25 23:14:03  joeh
' Copy and paste in cells of data grid view
'
' Revision 1.1  2008/09/26 07:30:56  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.16  2008/06/25 01:53:40  joeh
' Ecosim NA indice plots are displayed in the same form where we have the NA tree view - Take 2
'
' Revision 1.15  2008/06/24 18:08:38  joeh
' Ecosim NA indice plots are displayed in the same form where  we have the NA tree view - Take 2
'
' Revision 1.14  2007/07/09 19:44:45  joeh
' Move hard coded strings to resource file
'
' Revision 1.13  2007/07/07 00:11:03  joeh
' Decrease column width
'
' Revision 1.12  2007/06/28 19:22:09  joeh
' Switch to wait cursor when displaying data
'
' Revision 1.11  2007/06/22 19:12:45  joeh
' Modify GetInstance()
'
' Revision 1.10  2007/06/21 00:14:38  joeh
' Rename SetUpPanel() to DisplayData()
'
' Revision 1.9  2007/06/20 18:13:56  joeh
' add header to the top of the file so that CVS will log the file with every update
'
'
'==============================================================================
Option Strict On
Option Explicit On

Imports System.Windows.Forms
Imports ZedGraph

Public Class cFlowFromDetritus
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

        SetUpGridColumn()

        'Set up grid rows
        Grid.RowHeadersVisible = False
        Grid.RowCount = NetworkManager.nGroups
        Grid.Rows(0).DefaultCellStyle.WrapMode = DataGridViewTriState.True
        Grid.Rows(0).DefaultCellStyle.BackColor = Drawing.Color.MintCream
        Grid.Rows(0).Frozen = True
        Grid.Rows(0).Height = FIRST_ROW_HEIGHT

        ReDim astrRowContent(Grid.Columns.Count)
        astrRowContent(0) = ""
        astrRowContent(1) = My.Resources.COL_HDR_GRP_NAME
        astrRowContent(2) = ""
        Grid.Rows(0).SetValues(astrRowContent)
        Grid.Rows(0).Visible = True

        For i As Integer = 1 To NetworkManager.nGroups - 1
            astrRowContent(0) = CStr(i)
            astrRowContent(1) = NetworkManager.GroupName(i)
            astrRowContent(2) = Me.StyleGuide.FormatNumber(NetworkManager.FlowFromDetritus(i))
            Grid.Rows(i).SetValues(astrRowContent)
            Grid.Rows(i).Visible = True
        Next
        Grid.ClearSelection()

    End Sub

    Private Sub SetUpGridColumn()

        'DataGrid.RowCount = 1
        Grid.ColumnCount = 3

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
