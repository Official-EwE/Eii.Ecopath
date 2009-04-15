'==============================================================================
'
' $Log: cCyclingAndPathLen.vb,v $
' Revision 1.3  2009/04/15 18:14:49  joeh
' Set m_Panel.AutoScroll = False
'
' Revision 1.2  2008/11/25 23:44:07  joeh
' Copy and paste in cells of data grid view
'
' Revision 1.1  2008/09/26 07:30:50  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.17  2008/06/25 01:53:39  joeh
' Ecosim NA indice plots are displayed in the same form where we have the NA tree view - Take 2
'
' Revision 1.16  2008/06/24 18:08:37  joeh
' Ecosim NA indice plots are displayed in the same form where  we have the NA tree view - Take 2
'
' Revision 1.15  2007/06/28 19:22:52  joeh
' Switch to wait cursor when displaying data
'
' Revision 1.14  2007/06/22 19:12:45  joeh
' Modify GetInstance()
'
' Revision 1.13  2007/06/22 16:37:27  joeh
' Move hard coded strings to the resource file
'
' Revision 1.12  2007/06/22 00:35:28  joeh
' Add Option Strict On and Option Explicit On
'
' Revision 1.11  2007/06/21 18:08:46  joeh
' Make the 2 in km2 to superscript
'
' Revision 1.10  2007/06/21 00:14:38  joeh
' Rename SetUpPanel() to DisplayData()
'
' Revision 1.9  2007/06/20 18:13:55  joeh
' add header to the top of the file so that CVS will log the file with every update
'
'
'==============================================================================
Option Strict On
Option Explicit On

Imports System.Windows.Forms
Imports ZedGraph

Public Class cCyclingAndPathLen
    Private Shared m_CyclingAndPathLenInstance As cCyclingAndPathLen

    Private m_NetworkManager As cNetworkManager
    'Private m_Panel As Windows.Forms.Panel
    Private Shared m_Panel As Windows.Forms.Panel

    Public Shared Function GetInstance(ByVal NetworkManager As cNetworkManager, ByVal Panel As Windows.Forms.Panel) As cCyclingAndPathLen
        m_Panel = Panel

        If m_CyclingAndPathLenInstance Is Nothing Then m_CyclingAndPathLenInstance = New cCyclingAndPathLen(NetworkManager, Panel)
        Return m_CyclingAndPathLenInstance
    End Function

    Private Sub New()
        '
    End Sub

    Private Sub New(ByVal NetworkManager As cNetworkManager, ByVal Panel As Windows.Forms.Panel)
        Me.New()

        m_NetworkManager = NetworkManager
        m_Panel = Panel
    End Sub

    Public Sub DisplayData()
        Dim DataGrid As Windows.Forms.DataGridView = _
            CType(m_Panel.Controls("dgvNetworkAnalysis"), Windows.Forms.DataGridView)
        Dim strRowContent() As String

        Cursor.Current = Cursors.WaitCursor
        RemoveToolStrip()

        SetUpGridColumn()

        'Set up grid rows
        DataGrid.RowHeadersVisible = False
        DataGrid.RowCount = 8
        DataGrid.Rows(0).DefaultCellStyle.WrapMode = DataGridViewTriState.True
        DataGrid.Rows(0).DefaultCellStyle.BackColor = Drawing.Color.MintCream
        DataGrid.Rows(0).Frozen = True
        DataGrid.Rows(0).Height = FIRST_ROW_HEIGHT

        ReDim strRowContent(DataGrid.Columns.Count)
        strRowContent(0) = My.Resources.COL_HDR_PARAM
        strRowContent(1) = My.Resources.COL_HDR_VALUE
        strRowContent(2) = My.Resources.COL_HDR_UNIT
        DataGrid.Rows(0).SetValues(strRowContent)
        DataGrid.Rows(0).Visible = True

        'SetCellText(Grid, 1, 1, "Throughput cycled (excluding detritus)")
        'SetCellValue(Grid, 2, 1, Format(Tc, "0.00"))
        'SetCellText(Grid, 3, 1, GetUnits(2, 2))
        strRowContent(0) = My.Resources.ROW_HDR_THROUGHPUT_CYC_LIV
        strRowContent(1) = m_NetworkManager.ThroughputCycledLiving.ToString("F2")
        strRowContent(2) = My.Resources.STR_T_KM2_YR
        DataGrid.Rows(1).SetValues(strRowContent)
        DataGrid.Rows(1).Visible = True

        'g_Recordset.Fields("TrputCyclExlDet").value = Tc
        'SetCellText(Grid, 1, 2, "Predatory cycling index")
        'SetCellValue(Grid, 2, 2, IIf(Abs(TCyc) > 0, Format(100 * Tc / TCyc, "0.00"), ""))
        'SetCellText(Grid, 3, 2, "% of throughput w/o detritus")
        'g_Recordset.Fields("PredatorCyclingIndex").value = 100 * Tc / TCyc
        strRowContent(0) = My.Resources.ROW_HDR_PRED_CYC_INDX
        If Math.Abs(m_NetworkManager.ThroughputCycledPredatory) > 0.0 Then
            strRowContent(1) = (100.0 * m_NetworkManager.ThroughputCycledLiving / _
                m_NetworkManager.ThroughputCycledPredatory).ToString("F2")
        Else
            strRowContent(1) = ""
        End If
        strRowContent(2) = My.Resources.STR_PCT_THROUGHPUT_LIV
        DataGrid.Rows(2).SetValues(strRowContent)
        DataGrid.Rows(2).Visible = True

        'SetCellText(Grid, 1, 3, "Throughput cycled (including detritus)")
        'SetCellValue(Grid, 2, 3, IIf(Abs(TcD) > 0, Format(TcD, "0.00"), ""))  'Format(100 * Tc / TcD, "0.00"), "")
        'SetCellText(Grid, 3, 3, GetUnits(2, 2))
        'g_Recordset.Fields("TrputCyclInclDet").value = TcD
        strRowContent(0) = My.Resources.ROW_HDR_THROUGHPUT_CYC_TOTAL
        If Math.Abs(m_NetworkManager.ThroughputCycledAll) > 0.0 Then
            strRowContent(1) = m_NetworkManager.ThroughputCycledAll.ToString("F2")
        Else
            strRowContent(1) = ""
        End If
        strRowContent(2) = My.Resources.STR_T_KM2_YR
        DataGrid.Rows(3).SetValues(strRowContent)
        DataGrid.Rows(3).Visible = True

        'SetCellText(Grid, 1, 4, "Finn's cycling index")
        'SetCellValue(Grid, 2, 4, Format(100 * TcD / TruPut, "0.00"))
        'SetCellText(Grid, 3, 4, "% of total throughput")
        'g_Recordset.Fields("FinnCyclingIndex").value = 100 * TcD / TruPut
        strRowContent(0) = My.Resources.ROW_HDR_FINN_CYC_INDX
        strRowContent(1) = (100.0 * m_NetworkManager.ThroughputCycledAll / _
            m_NetworkManager.ThroughputTotal).ToString("F2")
        strRowContent(2) = My.Resources.STR_PCT_TOTAL_THROUGHPUT
        DataGrid.Rows(4).SetValues(strRowContent)
        DataGrid.Rows(4).Visible = True

        'Mean path length is truput/(export+respiration)
        'SetCellText(Grid, 1, 5, "Finn's mean path length")
        'If SumEx + SumResp > 0 Then
        '    SetCellValue(Grid, 2, 5, Format(TruPut / (SumEx + SumResp), GenNum))
        '    'Print #fnum, Chr(9); Format(IIf(Abs((TruPut / (SumEx + SumResp))) > 0.001, TruPut / (SumEx + SumResp), 0), "0.00") &
        '    g_Recordset.Fields("PathLength").value = TruPut / (SumEx + SumResp)
        'End If
        'SetCellText(Grid, 3, 5, "-")
        strRowContent(0) = My.Resources.ROW_HDR_FINN_MEAN_PATH_LEN
        If m_NetworkManager.ThroughputExport + m_NetworkManager.ThroughputResp > 0.0 Then
            strRowContent(1) = (m_NetworkManager.ThroughputTotal / _
                (m_NetworkManager.ThroughputExport + m_NetworkManager.ThroughputResp)).ToString("F4")
        Else
            strRowContent(1) = ""
        End If
        strRowContent(2) = My.Resources.STR_NONE
        DataGrid.Rows(5).SetValues(strRowContent)
        DataGrid.Rows(5).Visible = True

        'SetCellText(Grid, 1, 6, "Finn's straight-through path length")
        'If (SumEx - Ex(NumGroups) + SumResp) > 0 Then
        '    SetCellValue(Grid, 2, 6, Format((TCyc - Tc) / (SumEx - Ex(NumGroups) + SumResp), GenNum))
        '    g_Recordset.Fields("StraightPathLength").value = IIf(Abs(((TCyc - Tc) / (SumEx - Ex(NumGroups) + SumResp))) > 0.001, ((TCyc - Tc) / (SumEx - Ex(NumGroups) + SumResp)), 0)
        'End If
        'SetCellText(Grid, 3, 6, "without detritus")
        'g_Recordset.Update()
        strRowContent(0) = My.Resources.ROW_HDR_FINN_STR_THRU_PATH_LEN
        If m_NetworkManager.ThroughputExport - m_NetworkManager.ThroughputExportByGroup(m_NetworkManager.nGroups) + _
            m_NetworkManager.ThroughputResp > 0.0 Then
            strRowContent(1) = ((m_NetworkManager.ThroughputCycledPredatory - m_NetworkManager.ThroughputCycledLiving) / _
                (m_NetworkManager.ThroughputExport - m_NetworkManager.ThroughputExportByGroup(m_NetworkManager.nGroups) + _
                m_NetworkManager.ThroughputResp)).ToString("F4")
        Else
            strRowContent(1) = ""
        End If
        strRowContent(2) = My.Resources.STR_WO_DET
        DataGrid.Rows(6).SetValues(strRowContent)
        DataGrid.Rows(6).Visible = True

        'SetCellText(Grid, 1, 7, "Finn's straight-through path length") '7
        'If SumEx + SumResp > 0 Then
        '    SetCellValue(Grid, 2, 7, Format((TruPut - TcD) / (SumEx + SumResp), GenNum))
        'End If
        'SetCellText(Grid, 3, 7, "with detritus")
        strRowContent(0) = My.Resources.ROW_HDR_FINN_STR_THRU_PATH_LEN
        If m_NetworkManager.ThroughputExport + m_NetworkManager.ThroughputResp > 0.0 Then
            strRowContent(1) = ((m_NetworkManager.ThroughputTotal - m_NetworkManager.ThroughputCycledAll) / _
                (m_NetworkManager.ThroughputExport + m_NetworkManager.ThroughputResp)).ToString("F4")
        Else
            strRowContent(1) = ""
        End If
        strRowContent(2) = My.Resources.STR_W_DET
        DataGrid.Rows(7).SetValues(strRowContent)
        DataGrid.Rows(7).Visible = True

        DataGrid.ClearSelection()
        Cursor.Current = Cursors.Default
    End Sub

    Private Sub SetUpGridColumn()
        Dim DataGrid As Windows.Forms.DataGridView = _
            CType(m_Panel.Controls("dgvNetworkAnalysis"), Windows.Forms.DataGridView)
        Dim GraphPane As ZedGraphControl = _
            CType(m_Panel.Controls("zgcNetworkAnalysis"), ZedGraphControl)
        Dim LogoPanel As Windows.Forms.TableLayoutPanel = _
            CType(m_Panel.Controls("tlpNetworkAnalysis"), Windows.Forms.TableLayoutPanel)

        m_Panel.AutoScroll = False
        LogoPanel.Visible = False
        GraphPane.Visible = False
        DataGrid.ReadOnly = True
        DataGrid.Visible = True
        DataGrid.ColumnCount = 3

        SetGridColumnPropertyDefault(DataGrid)

        DataGrid.Columns(0).Frozen = True
        DataGrid.Columns(0).DefaultCellStyle.BackColor = Drawing.Color.MintCream
        DataGrid.Columns(0).Width = 220
        DataGrid.Columns(0).DefaultCellStyle.Alignment = Windows.Forms.DataGridViewContentAlignment.MiddleLeft

        DataGrid.Columns(2).Width = 165
        DataGrid.Columns(2).DefaultCellStyle.Alignment = Windows.Forms.DataGridViewContentAlignment.MiddleLeft
    End Sub

    Private Sub RemoveToolStrip()
        Dim ToolStrip As Windows.Forms.ToolStrip = _
            CType(m_Panel.Controls("tsNetworkAnalysis"), Windows.Forms.ToolStrip)
        Dim DataGrid As Windows.Forms.DataGridView = _
            CType(m_Panel.Controls("dgvNetworkAnalysis"), Windows.Forms.DataGridView)

        If Not ToolStrip Is Nothing Then
            m_Panel.Controls.RemoveByKey("tsNetworkAnalysis")
            DataGrid.Dock = Windows.Forms.DockStyle.Fill
        End If
    End Sub

End Class
