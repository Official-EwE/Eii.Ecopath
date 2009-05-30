'==============================================================================
'
' $Log: cForHarvestOfAllGp.vb,v $
' Revision 1.9  2009/05/30 00:00:51  jeroens
' Toolstrip usage centralized
'
' Revision 1.8  2009/05/19 13:41:13  jeroens
' Content manager derived pages will take care of updating NA run state
'
' Revision 1.7  2009/05/02 01:51:29  jeroens
' Updated to cControlManager FN name change
'
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
'==============================================================================
Option Strict On
Option Explicit On

Imports System.Windows.Forms
Imports EwECore
Imports ZedGraph

Public Class cForHarvestOfAllGp
    Inherits cContentManager

    Public Sub New()
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
        Dim lngSumPath As Long

        ' Load pre-requesites
        Me.NetworkManager.RunRequiredPrimaryProd()

        ' Init
        SetUpGridColumn()

        'Set up grid rows
        Grid.RowHeadersVisible = False
        Grid.RowCount = Me.NetworkManager.Core.nLivingGroups + 2
        Grid.Rows(0).DefaultCellStyle.WrapMode = DataGridViewTriState.True
        Grid.Rows(0).DefaultCellStyle.BackColor = Drawing.Color.MintCream
        Grid.Rows(0).Frozen = True
        Grid.Rows(0).Height = FIRST_ROW_HEIGHT

        ReDim strRowContent(Grid.Columns.Count)
        strRowContent(0) = ""
        strRowContent(1) = My.Resources.COL_HDR_GRP_NAME
        strRowContent(2) = My.Resources.COL_HDR_NUM_PATH
        strRowContent(3) = My.Resources.COL_HDR_TL
        strRowContent(4) = My.Resources.COL_HDR_PPR_PP
        strRowContent(5) = My.Resources.COL_HDR_PPR_DET
        strRowContent(6) = My.Resources.COL_HDR_PPR
        strRowContent(7) = My.Resources.COL_HDR_CATCH
        strRowContent(8) = My.Resources.COL_HDR_PPR_CATCH
        strRowContent(9) = My.Resources.COL_HDR_PPR_TOTAL_PP
        strRowContent(10) = My.Resources.COL_HDR_PPR_U_CATCH
        Grid.Rows(0).SetValues(strRowContent)
        Grid.Rows(0).Visible = True

        For i As Integer = 1 To Me.NetworkManager.Core.nLivingGroups
            strRowContent(0) = CStr(i)
            strRowContent(1) = NetworkManager.GroupName(i)
            strRowContent(2) = CStr(NetworkManager.NumerPaths(i))
            If NetworkManager.PPRCatchHarvest(i) > 0.0 Then lngSumPath = lngSumPath + NetworkManager.NumerPaths(i)
            strRowContent(3) = NetworkManager.TrophicLevel(i).ToString("F2")
            strRowContent(4) = NetworkManager.PPRRequiredHarvest(i).ToString("F2")
            strRowContent(5) = NetworkManager.PPRRequiredDetHarvest(i).ToString("F2")
            strRowContent(6) = NetworkManager.PPRRequiredSumHarvest(i).ToString("F2")
            strRowContent(7) = NetworkManager.PPRCatchHarvest(i).ToString("F2")
            If NetworkManager.PPRCatchHarvest(i) > 0.0 Then
                strRowContent(8) = NetworkManager.PPROverCatchHarvest(i).ToString("F2")
            Else
                strRowContent(8) = ""
            End If
            strRowContent(9) = NetworkManager.PPRTotPPHarvest(i).ToString("F2")
            If NetworkManager.PPRCatchHarvest(i) > 0.0 And NetworkManager.TotalPrimaryProduction > 0.0 Then
                strRowContent(10) = NetworkManager.PPRUHarvest(i).ToString("F2")
            Else
                strRowContent(10) = ""
            End If
            Grid.Rows(i).SetValues(strRowContent)
            Grid.Rows(i).Visible = True
        Next

        'Display total
        For i As Integer = 0 To Grid.Columns.Count - 1
            strRowContent(i) = ""
        Next
        strRowContent(1) = My.Resources.ROW_HDR_TOTAL
        strRowContent(2) = CStr(lngSumPath)
        strRowContent(3) = NetworkManager.TotalTL.ToString("F2")
        strRowContent(4) = NetworkManager.TotalPPRPP.ToString("F2")
        strRowContent(5) = NetworkManager.TotalPPRDet.ToString("F2")
        strRowContent(6) = (NetworkManager.TotalPPRPP + NetworkManager.TotalPPRDet).ToString("F2")
        strRowContent(7) = NetworkManager.TotalCatch.ToString("F2")
        If NetworkManager.TotalCatch > 0.0 Then
            strRowContent(8) = ((NetworkManager.TotalPPRPP + NetworkManager.TotalPPRDet) / _
                NetworkManager.TotalCatch).ToString("F2")
        Else
            strRowContent(8) = ""
        End If
        strRowContent(9) = (100 * (NetworkManager.TotalPPRPP + NetworkManager.TotalPPRDet) / _
            (NetworkManager.TotalPrimaryProduction + NetworkManager.DetThroughtput(1))).ToString("F2")
        If NetworkManager.TotalCatch > 0.0 Then
            strRowContent(10) = ((NetworkManager.TotalPPRPP + NetworkManager.TotalPPRDet) / _
                (NetworkManager.TotalPrimaryProduction + NetworkManager.DetThroughtput(1)) / _
                NetworkManager.TotalCatch).ToString("F2")
        Else
            strRowContent(10) = ""
        End If
        Grid.Rows(Grid.RowCount - 1).SetValues(strRowContent)
        Grid.Rows(Grid.RowCount - 1).Visible = True

        'Hide some rows
        For i As Integer = 1 To Me.NetworkManager.Core.nLivingGroups
            If NetworkManager.PPRCatchHarvest(i) <= 0.0 Or _
                NetworkManager.PPRCatchHarvest(i) <= 0.0 And NetworkManager.TotalPrimaryProduction <= 0.0 Then
                Grid.Rows(i).Visible = False
            End If
        Next
        Grid.ClearSelection()
        Cursor.Current = Cursors.Default
    End Sub

    Private Sub SetUpGridColumn()

        Grid.ReadOnly = True
        'DataGrid.RowCount = 1
        Grid.ColumnCount = 11

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
