'==============================================================================
'
' $Log: cForConsumpOfAllGp.vb,v $
' Revision 1.6  2009/05/01 17:42:56  jeroens
' Inherited from cContentManager
'
' Revision 1.5  2009/04/17 01:07:03  joeh
' Make MixedTrophicImpactUC not visible when needed
'
' Revision 1.4  2009/04/15 23:37:37  joeh
' Add "Imports System.Windows.Forms" statement
'
' Revision 1.3  2009/04/15 18:14:52  joeh
' Set m_Panel.AutoScroll = False
'
' Revision 1.2  2008/11/25 20:55:41  joeh
' Copy and paste in cells of data grid view
'
' Revision 1.1  2008/09/26 07:30:54  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.17  2008/06/25 01:53:40  joeh
' Ecosim NA indice plots are displayed in the same form where we have the NA tree view - Take 2
'
' Revision 1.16  2008/06/24 18:08:38  joeh
' Ecosim NA indice plots are displayed in the same form where  we have the NA tree view - Take 2
'
' Revision 1.15  2007/07/09 19:44:45  joeh
' Move hard coded strings to resource file
'
' Revision 1.14  2007/07/07 00:11:04  joeh
' Decrease column width
'
' Revision 1.13  2007/07/06 00:44:59  joeh
' Move hard coded strings to resource file
'
' Revision 1.12  2007/06/28 19:22:09  joeh
' Switch to wait cursor when displaying data
'
' Revision 1.11  2007/06/22 19:12:46  joeh
' Modify GetInstance()
'
' Revision 1.10  2007/06/22 00:35:29  joeh
' Add Option Strict On and Option Explicit On
'
' Revision 1.9  2007/06/21 00:14:38  joeh
' Rename SetUpPanel() to DisplayData()
'
' Revision 1.8  2007/06/20 18:13:57  joeh
' add header to the top of the file so that CVS will log the file with every update
'
'
'==============================================================================
Option Strict On
Option Explicit On

Imports System.Windows.Forms
Imports EwECore
Imports ZedGraph

Public Class cForConsumpOfAllGp
    Inherits cContentManager

    Public Sub New()
        '
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
        Dim sngTotalPPRCons As Single

        SetUpGridColumn()

        'Set up grid rows
        DataGrid.RowHeadersVisible = False
        DataGrid.RowCount = Me.NetworkManager.Core.nLivingGroups + 2
        DataGrid.Rows(0).DefaultCellStyle.WrapMode = DataGridViewTriState.True
        DataGrid.Rows(0).DefaultCellStyle.BackColor = Drawing.Color.MintCream
        DataGrid.Rows(0).Frozen = True
        DataGrid.Rows(0).Height = FIRST_ROW_HEIGHT

        ReDim strRowContent(DataGrid.Columns.Count)
        strRowContent(0) = ""
        strRowContent(1) = My.Resources.COL_HDR_GRP_NAME
        strRowContent(2) = My.Resources.COL_HDR_NUM_PATH
        strRowContent(3) = My.Resources.COL_HDR_TL
        strRowContent(4) = My.Resources.COL_HDR_PPR_PP
        strRowContent(5) = My.Resources.COL_HDR_PPR_DET
        strRowContent(6) = My.Resources.COL_HDR_PPR
        strRowContent(7) = My.Resources.COL_HDR_CONSUM
        strRowContent(8) = My.Resources.COL_HDR_PPR_COMSUM
        strRowContent(9) = My.Resources.COL_HDR_PPR_TOTAL_PP
        strRowContent(10) = My.Resources.COL_HDR_PPR_U_BIOMASS
        DataGrid.Rows(0).SetValues(strRowContent)
        DataGrid.Rows(0).Visible = True

        For i As Integer = 1 To Me.NetworkManager.Core.nLivingGroups
            strRowContent(0) = CStr(i)
            strRowContent(1) = NetworkManager.GroupName(i)
            strRowContent(2) = CStr(NetworkManager.NumerPaths(i))
            strRowContent(3) = NetworkManager.TrophicLevel(i).ToString("F2")
            strRowContent(4) = NetworkManager.PPRRequired(i).ToString("F2")
            strRowContent(5) = NetworkManager.PPRRequiredDet(i).ToString("F2")
            strRowContent(6) = NetworkManager.PPRRequiredSum(i).ToString("F2")
            strRowContent(7) = NetworkManager.PPRCons(i).ToString("F2")
            sngTotalPPRCons = sngTotalPPRCons + NetworkManager.PPRCons(i)
            If NetworkManager.PPRCons(i) > 0.0 Then
                strRowContent(8) = NetworkManager.PPROverCons(i).ToString("F2")
            Else
                strRowContent(8) = ""
            End If
            strRowContent(9) = NetworkManager.PPRTotPP(i).ToString("F2")
            If NetworkManager.TotalPrimaryProduction > 0.0 Then
                strRowContent(10) = NetworkManager.PPRU(i).ToString("F2")
            Else
                strRowContent(10) = ""
            End If
            DataGrid.Rows(i).SetValues(strRowContent)
            DataGrid.Rows(i).Visible = True

            'DataGrid.Rows(i - 1).HeaderCell.Value = CStr(i)
            'DataGrid.Rows(i - 1).HeaderCell.Style.BackColor = Drawing.Color.Beige
        Next

        'Display total
        For i As Integer = 0 To DataGrid.Columns.Count - 1
            strRowContent(i) = ""
        Next
        strRowContent(1) = My.Resources.ROW_HDR_TOTAL
        strRowContent(2) = CStr((NetworkManager.NumLivPath + NetworkManager.NumDetPath))
        strRowContent(7) = sngTotalPPRCons.ToString("F2")
        DataGrid.Rows(DataGrid.Rows.Count - 1).SetValues(strRowContent)
        DataGrid.Rows(DataGrid.Rows.Count - 1).Visible = True

        'Hide some rows
        For i As Integer = 1 To Me.NetworkManager.Core.nLivingGroups
            If NetworkManager.PPRCons(i) <= 0.0 Or _
                NetworkManager.TotalPrimaryProduction <= 0.0 Then
                DataGrid.Rows(i).Visible = False
            End If
        Next
        DataGrid.ClearSelection()
        Cursor.Current = Cursors.Default
    End Sub

    Private Sub SetUpGridColumn()

        DataGrid.ReadOnly = True
        'DataGrid.RowCount = 1
        DataGrid.ColumnCount = 11

        SetGridColumnPropertyDefault(DataGrid)

        DataGrid.Columns(0).DefaultCellStyle.BackColor = Drawing.Color.MintCream
        DataGrid.Columns(0).Frozen = True
        DataGrid.Columns(0).Width = ID_COL_WIDTH

        DataGrid.Columns(1).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
        DataGrid.Columns(1).DefaultCellStyle.BackColor = Drawing.Color.MintCream
        DataGrid.Columns(1).Frozen = True
        DataGrid.Columns(1).Width = GRP_NAME_COL_WIDTH

    End Sub

End Class
