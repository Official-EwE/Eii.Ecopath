'==============================================================================
'
' $Log: cByGroup.vb,v $
' Revision 1.7  2009/05/02 01:51:20  jeroens
' Updated to cControlManager FN name change
'
' Revision 1.6  2009/05/01 17:42:54  jeroens
' Inherited from cContentManager
'
' Revision 1.5  2009/04/17 01:07:01  joeh
' Make MixedTrophicImpactUC not visible when needed
'
' Revision 1.4  2009/04/16 00:29:47  joeh
' Add "Import.System.Windows.Forms" statement
'
' Revision 1.3  2009/04/15 18:14:49  joeh
' Set m_Panel.AutoScroll = False
'
' Revision 1.2  2008/11/25 20:55:41  joeh
' Copy and paste in cells of data grid view
'
' Revision 1.1  2008/09/26 07:30:48  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.19  2008/06/25 01:53:39  joeh
' Ecosim NA indice plots are displayed in the same form where we have the NA tree view - Take 2
'
' Revision 1.18  2008/06/24 18:08:37  joeh
' Ecosim NA indice plots are displayed in the same form where  we have the NA tree view - Take 2
'
' Revision 1.17  2007/07/09 23:05:48  joeh
' Move hard coded strings to resource file
'
' Revision 1.16  2007/07/07 00:11:03  joeh
' Decrease column width
'
' Revision 1.15  2007/07/06 00:44:59  joeh
' Move hard coded strings to resource file
'
' Revision 1.14  2007/06/28 19:22:51  joeh
' Switch to wait cursor when displaying data
'
' Revision 1.13  2007/06/22 19:12:45  joeh
' Modify GetInstance()
'
' Revision 1.12  2007/06/22 00:35:28  joeh
' Add Option Strict On and Option Explicit On
'
' Revision 1.11  2007/06/21 23:49:35  joeh
' Move hard coded strings into the resource file
'
' Revision 1.10  2007/06/21 18:08:45  joeh
' Make the 2 in km2 to superscript
'
' Revision 1.9  2007/06/21 00:14:38  joeh
' Rename SetUpPanel() to DisplayData()
'
' Revision 1.8  2007/06/20 18:13:55  joeh
' add header to the top of the file so that CVS will log the file with every update
'
'
'==============================================================================
Option Strict On
Option Explicit On

Imports System.Windows.Forms
Imports ZedGraph

Public Class cByGroup
    Inherits cContentManager

    Public Sub New()
        '
    End Sub

    Public Overrides Sub DisplayData()
        Dim strRowContent() As String

        SetUpGridColumn()

        'Set up grid rows
        Me.Grid.RowHeadersVisible = False
        Me.Grid.RowCount = Me.NetworkManager.nGroups + 4
        Me.Grid.Rows(0).DefaultCellStyle.WrapMode = DataGridViewTriState.True
        Me.Grid.Rows(0).DefaultCellStyle.BackColor = Drawing.Color.MintCream
        Me.Grid.Rows(0).Frozen = True
        Me.Grid.Rows(0).Height = FIRST_ROW_HEIGHT

        ReDim strRowContent(Grid.Columns.Count)
        strRowContent(0) = ""
        strRowContent(1) = My.Resources.COL_HDR_GRP_NAME
        strRowContent(2) = My.Resources.COL_HDR_ASCEND
        strRowContent(3) = My.Resources.COL_HDR_OVERHEAD
        strRowContent(4) = My.Resources.COL_HDR_CAPACITY
        strRowContent(5) = My.Resources.COL_HDR_INFO
        strRowContent(6) = My.Resources.COL_HDR_THROUGHPUT_UNIT
        Me.Grid.Rows(0).SetValues(strRowContent)
        Me.Grid.Rows(0).Visible = True

        For i As Integer = 1 To Me.NetworkManager.nGroups
            strRowContent(0) = CStr(i)
            strRowContent(1) = Me.NetworkManager.GroupName(i)
            strRowContent(2) = Me.NetworkManager.AscendancyByGroup(i).ToString("F4")
            strRowContent(3) = Me.NetworkManager.OverheadByGroup(i).ToString("F4")
            strRowContent(4) = Me.NetworkManager.CapacityByGroup(i).ToString("F4")
            strRowContent(5) = Me.NetworkManager.InformationByGroup(i).ToString("F4")
            strRowContent(6) = Me.NetworkManager.ThroughputByGroup(i).ToString("F4")
            Me.Grid.Rows(i).SetValues(strRowContent)
            Me.Grid.Rows(i).Visible = True
        Next

        strRowContent(0) = ""
        strRowContent(1) = My.Resources.ROW_HDR_IMPORT
        strRowContent(2) = ""
        strRowContent(3) = ""
        strRowContent(4) = ""
        strRowContent(5) = ""
        strRowContent(6) = NetworkManager.ThroughputByGroup(Me.NetworkManager.nGroups + 1).ToString("F4")
        Me.Grid.Rows(NetworkManager.nGroups + 1).SetValues(strRowContent)
        Me.Grid.Rows(NetworkManager.nGroups + 1).Visible = True

        strRowContent(0) = ""
        strRowContent(1) = My.Resources.ROW_HDR_TOTAL
        strRowContent(2) = Me.NetworkManager.AscendencyTotal.ToString("F4")
        strRowContent(3) = Me.NetworkManager.OverheadTotal.ToString("F4")
        strRowContent(4) = Me.NetworkManager.CapacityTotal.ToString("F4")
        If Me.NetworkManager.ThroughputTotal > 0 Then
            strRowContent(5) = (Me.NetworkManager.AscendencyTotal / Me.NetworkManager.ThroughputTotal).ToString("F4")
        Else
            strRowContent(5) = ""
        End If
        strRowContent(6) = Me.NetworkManager.ThroughputTotal.ToString("F4")
        Me.Grid.Rows(Me.NetworkManager.nGroups + 2).SetValues(strRowContent)
        Me.Grid.Rows(Me.NetworkManager.nGroups + 2).Visible = True

        strRowContent(0) = ""
        strRowContent(1) = My.Resources.ROW_HDR_PCT
        strRowContent(2) = (Me.NetworkManager.AscendencyTotal / Me.NetworkManager.CapacityTotal * 100.0).ToString("F4")
        strRowContent(3) = (Me.NetworkManager.OverheadTotal / Me.NetworkManager.CapacityTotal * 100.0).ToString("F4")
        strRowContent(4) = (Me.NetworkManager.CapacityTotal / Me.NetworkManager.CapacityTotal * 100.0).ToString("F4")
        strRowContent(5) = ""
        strRowContent(6) = ""
        Me.Grid.Rows(Me.NetworkManager.nGroups + 3).SetValues(strRowContent)
        Me.Grid.Rows(Me.NetworkManager.nGroups + 3).Visible = True

        Me.Grid.ClearSelection()
    End Sub

    Private Sub SetUpGridColumn()

        Me.Graph.Visible = False
        Me.Grid.ReadOnly = True
        Me.Grid.Visible = True
        'Me.DataGrid.RowCount = 1
        Me.Grid.ColumnCount = 7

        SetGridColumnPropertyDefault(Me.Grid)

        Me.Grid.Columns(0).DefaultCellStyle.BackColor = Drawing.Color.MintCream
        Me.Grid.Columns(0).Frozen = True
        Me.Grid.Columns(0).Width = ID_COL_WIDTH

        Grid.Columns(1).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
        Grid.Columns(1).DefaultCellStyle.BackColor = Drawing.Color.MintCream
        Grid.Columns(1).Frozen = True
        Grid.Columns(1).Width = GRP_NAME_COL_WIDTH

        'For intIndex As Integer = 2 To 4
        '    DataGrid.Columns(intIndex).Width = 120
        'Next
    End Sub

End Class
