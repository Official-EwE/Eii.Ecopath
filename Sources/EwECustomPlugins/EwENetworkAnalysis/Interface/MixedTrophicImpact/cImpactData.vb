'==============================================================================
'
' $Log: cImpactData.vb,v $
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
' Revision 1.15  2008/06/25 01:53:41  joeh
' Ecosim NA indice plots are displayed in the same form where we have the NA tree view - Take 2
'
' Revision 1.14  2008/06/24 18:08:38  joeh
' Ecosim NA indice plots are displayed in the same form where  we have the NA tree view - Take 2
'
' Revision 1.13  2007/07/07 00:11:04  joeh
' Decrease column width
'
' Revision 1.12  2007/07/06 00:44:59  joeh
' Move hard coded strings to resource file
'
' Revision 1.11  2007/06/28 19:22:10  joeh
' Switch to wait cursor when displaying data
'
' Revision 1.10  2007/06/22 19:12:46  joeh
' Modify GetInstance()
'
' Revision 1.9  2007/06/22 00:35:30  joeh
' Add Option Strict On and Option Explicit On
'
' Revision 1.8  2007/06/21 00:14:39  joeh
' Rename SetUpPanel() to DisplayData()
'
' Revision 1.7  2007/06/20 18:13:58  joeh
' add header to the top of the file so that CVS will log the file with every update
'
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

    Public Overrides Sub Attach(ByVal manager As cNetworkManager, _
                                  ByVal datagrid As DataGridView, _
                                  ByVal graph As ZedGraphControl, _
                                  ByVal plot As ucPlot)
        MyBase.Attach(manager, datagrid, graph, plot)
        Me.Grid.Visible = True
    End Sub

    Public Overrides Sub DisplayData()

        Dim strRowContent() As String

        SetUpGridColumn(NetworkManager.nGroups, NetworkManager.nFleets)

        'Set up grid rows
        Grid.RowHeadersVisible = False
        Grid.RowCount = NetworkManager.nGroups + NetworkManager.nFleets + 1
        Grid.Rows(0).DefaultCellStyle.WrapMode = DataGridViewTriState.True
        Grid.Rows(0).DefaultCellStyle.BackColor = Drawing.Color.MintCream
        Grid.Rows(0).Frozen = True
        Grid.Rows(0).Height = FIRST_ROW_HEIGHT

        ReDim strRowContent(Grid.Columns.Count)
        strRowContent(0) = ""
        strRowContent(1) = My.Resources.COL_HDR_IMPACTING_IMPACTED
        For intIndex As Integer = 1 To NetworkManager.nGroups
            strRowContent(intIndex + 1) = NetworkManager.GroupName(intIndex)
        Next
        For intIndex As Integer = 1 To NetworkManager.nFleets
            strRowContent(NetworkManager.nGroups + intIndex + 1) = NetworkManager.FleetName(intIndex)
        Next
        Grid.Rows(0).SetValues(strRowContent)
        Grid.Rows(0).Visible = True

        For i As Integer = 1 To NetworkManager.nGroups + NetworkManager.nFleets
            strRowContent(0) = CStr(i)
            If i <= NetworkManager.nGroups Then
                strRowContent(1) = NetworkManager.GroupName(i)
            Else
                strRowContent(1) = NetworkManager.FleetName(i - NetworkManager.nGroups)
            End If
            For j As Integer = 1 To NetworkManager.nGroups + NetworkManager.nFleets
                strRowContent(j + 1) = (NetworkManager.MixedTrophicImpacts(i, j)).ToString("F4")
            Next
            Grid.Rows(i).SetValues(strRowContent)
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
