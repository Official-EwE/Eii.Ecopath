'==============================================================================
'
' $Log: cCyclingAndPathLen.vb,v $
' Revision 1.8  2009/05/19 13:41:13  jeroens
' Content manager derived pages will take care of updating NA run state
'
' Revision 1.7  2009/05/02 01:51:21  jeroens
' Updated to cControlManager FN name change
'
' Revision 1.6  2009/05/01 17:42:55  jeroens
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
' Revision 1.2  2008/11/25 23:44:07  joeh
' Copy and paste in cells of data grid view
'
' Revision 1.1  2008/09/26 07:30:50  sherman
' --== DELETED HISTORY ==--
'
'==============================================================================
Option Strict On
Option Explicit On

Imports System.Windows.Forms
Imports ZedGraph

Public Class cCyclingAndPathLen
    Inherits cContentManager

    Public Sub New()
        '
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

        SetUpGridColumn()

        'Set up grid rows
        Grid.RowHeadersVisible = False
        Grid.RowCount = 8
        Grid.Rows(0).DefaultCellStyle.WrapMode = DataGridViewTriState.True
        Grid.Rows(0).DefaultCellStyle.BackColor = Drawing.Color.MintCream
        Grid.Rows(0).Frozen = True
        Grid.Rows(0).Height = FIRST_ROW_HEIGHT

        ReDim strRowContent(Grid.Columns.Count)
        strRowContent(0) = My.Resources.COL_HDR_PARAM
        strRowContent(1) = My.Resources.COL_HDR_VALUE
        strRowContent(2) = My.Resources.COL_HDR_UNIT
        Grid.Rows(0).SetValues(strRowContent)
        Grid.Rows(0).Visible = True

        'SetCellText(Grid, 1, 1, "Throughput cycled (excluding detritus)")
        'SetCellValue(Grid, 2, 1, Format(Tc, "0.00"))
        'SetCellText(Grid, 3, 1, GetUnits(2, 2))
        strRowContent(0) = My.Resources.ROW_HDR_THROUGHPUT_CYC_LIV
        strRowContent(1) = NetworkManager.ThroughputCycledLiving.ToString("F2")
        strRowContent(2) = My.Resources.STR_T_KM2_YR
        Grid.Rows(1).SetValues(strRowContent)
        Grid.Rows(1).Visible = True

        'g_Recordset.Fields("TrputCyclExlDet").value = Tc
        'SetCellText(Grid, 1, 2, "Predatory cycling index")
        'SetCellValue(Grid, 2, 2, IIf(Abs(TCyc) > 0, Format(100 * Tc / TCyc, "0.00"), ""))
        'SetCellText(Grid, 3, 2, "% of throughput w/o detritus")
        'g_Recordset.Fields("PredatorCyclingIndex").value = 100 * Tc / TCyc
        strRowContent(0) = My.Resources.ROW_HDR_PRED_CYC_INDX
        If Math.Abs(NetworkManager.ThroughputCycledPredatory) > 0.0 Then
            strRowContent(1) = (100.0 * NetworkManager.ThroughputCycledLiving / _
                NetworkManager.ThroughputCycledPredatory).ToString("F2")
        Else
            strRowContent(1) = ""
        End If
        strRowContent(2) = My.Resources.STR_PCT_THROUGHPUT_LIV
        Grid.Rows(2).SetValues(strRowContent)
        Grid.Rows(2).Visible = True

        'SetCellText(Grid, 1, 3, "Throughput cycled (including detritus)")
        'SetCellValue(Grid, 2, 3, IIf(Abs(TcD) > 0, Format(TcD, "0.00"), ""))  'Format(100 * Tc / TcD, "0.00"), "")
        'SetCellText(Grid, 3, 3, GetUnits(2, 2))
        'g_Recordset.Fields("TrputCyclInclDet").value = TcD
        strRowContent(0) = My.Resources.ROW_HDR_THROUGHPUT_CYC_TOTAL
        If Math.Abs(NetworkManager.ThroughputCycledAll) > 0.0 Then
            strRowContent(1) = NetworkManager.ThroughputCycledAll.ToString("F2")
        Else
            strRowContent(1) = ""
        End If
        strRowContent(2) = My.Resources.STR_T_KM2_YR
        Grid.Rows(3).SetValues(strRowContent)
        Grid.Rows(3).Visible = True

        'SetCellText(Grid, 1, 4, "Finn's cycling index")
        'SetCellValue(Grid, 2, 4, Format(100 * TcD / TruPut, "0.00"))
        'SetCellText(Grid, 3, 4, "% of total throughput")
        'g_Recordset.Fields("FinnCyclingIndex").value = 100 * TcD / TruPut
        strRowContent(0) = My.Resources.ROW_HDR_FINN_CYC_INDX
        strRowContent(1) = (100.0 * NetworkManager.ThroughputCycledAll / _
            NetworkManager.ThroughputTotal).ToString("F2")
        strRowContent(2) = My.Resources.STR_PCT_TOTAL_THROUGHPUT
        Grid.Rows(4).SetValues(strRowContent)
        Grid.Rows(4).Visible = True

        'Mean path length is truput/(export+respiration)
        'SetCellText(Grid, 1, 5, "Finn's mean path length")
        'If SumEx + SumResp > 0 Then
        '    SetCellValue(Grid, 2, 5, Format(TruPut / (SumEx + SumResp), GenNum))
        '    'Print #fnum, Chr(9); Format(IIf(Abs((TruPut / (SumEx + SumResp))) > 0.001, TruPut / (SumEx + SumResp), 0), "0.00") &
        '    g_Recordset.Fields("PathLength").value = TruPut / (SumEx + SumResp)
        'End If
        'SetCellText(Grid, 3, 5, "-")
        strRowContent(0) = My.Resources.ROW_HDR_FINN_MEAN_PATH_LEN
        If NetworkManager.ThroughputExport + NetworkManager.ThroughputResp > 0.0 Then
            strRowContent(1) = (NetworkManager.ThroughputTotal / _
                (NetworkManager.ThroughputExport + NetworkManager.ThroughputResp)).ToString("F4")
        Else
            strRowContent(1) = ""
        End If
        strRowContent(2) = My.Resources.STR_NONE
        Grid.Rows(5).SetValues(strRowContent)
        Grid.Rows(5).Visible = True

        'SetCellText(Grid, 1, 6, "Finn's straight-through path length")
        'If (SumEx - Ex(NumGroups) + SumResp) > 0 Then
        '    SetCellValue(Grid, 2, 6, Format((TCyc - Tc) / (SumEx - Ex(NumGroups) + SumResp), GenNum))
        '    g_Recordset.Fields("StraightPathLength").value = IIf(Abs(((TCyc - Tc) / (SumEx - Ex(NumGroups) + SumResp))) > 0.001, ((TCyc - Tc) / (SumEx - Ex(NumGroups) + SumResp)), 0)
        'End If
        'SetCellText(Grid, 3, 6, "without detritus")
        'g_Recordset.Update()
        strRowContent(0) = My.Resources.ROW_HDR_FINN_STR_THRU_PATH_LEN
        If NetworkManager.ThroughputExport - NetworkManager.ThroughputExportByGroup(NetworkManager.nGroups) + _
            NetworkManager.ThroughputResp > 0.0 Then
            strRowContent(1) = ((NetworkManager.ThroughputCycledPredatory - NetworkManager.ThroughputCycledLiving) / _
                (NetworkManager.ThroughputExport - NetworkManager.ThroughputExportByGroup(NetworkManager.nGroups) + _
                NetworkManager.ThroughputResp)).ToString("F4")
        Else
            strRowContent(1) = ""
        End If
        strRowContent(2) = My.Resources.STR_WO_DET
        Grid.Rows(6).SetValues(strRowContent)
        Grid.Rows(6).Visible = True

        'SetCellText(Grid, 1, 7, "Finn's straight-through path length") '7
        'If SumEx + SumResp > 0 Then
        '    SetCellValue(Grid, 2, 7, Format((TruPut - TcD) / (SumEx + SumResp), GenNum))
        'End If
        'SetCellText(Grid, 3, 7, "with detritus")
        strRowContent(0) = My.Resources.ROW_HDR_FINN_STR_THRU_PATH_LEN
        If NetworkManager.ThroughputExport + NetworkManager.ThroughputResp > 0.0 Then
            strRowContent(1) = ((NetworkManager.ThroughputTotal - NetworkManager.ThroughputCycledAll) / _
                (NetworkManager.ThroughputExport + NetworkManager.ThroughputResp)).ToString("F4")
        Else
            strRowContent(1) = ""
        End If
        strRowContent(2) = My.Resources.STR_W_DET
        Grid.Rows(7).SetValues(strRowContent)
        Grid.Rows(7).Visible = True

        Grid.ClearSelection()
    End Sub

    Private Sub SetUpGridColumn()

        Grid.ReadOnly = True
        Grid.ColumnCount = 3

        SetGridColumnPropertyDefault(Grid)

        Grid.Columns(0).Frozen = True
        Grid.Columns(0).DefaultCellStyle.BackColor = Drawing.Color.MintCream
        Grid.Columns(0).Width = 220
        Grid.Columns(0).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft

        Grid.Columns(2).Width = 165
        Grid.Columns(2).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft

    End Sub

End Class
