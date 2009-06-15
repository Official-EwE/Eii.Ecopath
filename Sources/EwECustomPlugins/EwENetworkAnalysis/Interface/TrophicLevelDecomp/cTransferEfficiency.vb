'==============================================================================
'
' $Log: cTransferEfficiency.vb,v $
' Revision 1.1  2009/06/15 14:15:26  jeroens
' Flattened directory structure
'
' Revision 1.11  2009/06/04 18:04:08  jeroens
' Moved transfer efficiency computations to manager
'
' Revision 1.10  2009/06/03 19:26:27  jeroens
' Uses EwEUtils ToRoman
'
' Revision 1.9  2009/05/30 00:00:49  jeroens
' Toolstrip usage centralized
'
' Revision 1.8  2009/05/19 13:41:06  jeroens
' Content manager derived pages will take care of updating NA run state
'
' Revision 1.7  2009/05/02 01:51:31  jeroens
' Updated to cControlManager FN name change
'
' Revision 1.6  2009/05/01 17:42:53  jeroens
' Inherited from cContentManager
'
' Revision 1.5  2009/04/17 01:07:00  joeh
' Make MixedTrophicImpactUC not visible when needed
'
' Revision 1.4  2009/04/15 23:22:27  joeh
' Add "Imports System.Windows.Forms" statement
'
' Revision 1.3  2009/04/15 18:14:48  joeh
' Set m_Panel.AutoScroll = False
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

Imports EwEUtils.Utilities
Imports ZedGraph
Imports System.Windows.Forms

Public Class cTransferEfficiency
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

        Dim strRowContent() As String
        Dim TRavgP(4) As Single
        Dim TRavgD(4) As Single
        Dim TRavgT(4) As Single

        SetUpGridColumn(NetworkManager.nTrophicLevels)

        'Set up grid rows
        Grid.RowHeadersVisible = False
        Grid.RowCount = 9
        Grid.Rows(0).DefaultCellStyle.WrapMode = DataGridViewTriState.True
        Grid.Rows(0).DefaultCellStyle.BackColor = Drawing.Color.MintCream
        Grid.Rows(0).Frozen = True
        Grid.Rows(0).Height = FIRST_ROW_HEIGHT

        ReDim strRowContent(Grid.Columns.Count)
        strRowContent(0) = My.Resources.COL_HDR_SOURCE_TRP_LVL
        For i As Integer = 2 To NetworkManager.nTrophicLevels
            strRowContent(i - 1) = StringUtils.ToRoman(i)
        Next
        Grid.Rows(0).SetValues(strRowContent)
        Grid.Rows(0).Visible = True

        strRowContent(0) = My.Resources.ROW_HDR_PRODUCER
        For i As Integer = 2 To NetworkManager.nTrophicLevels
            strRowContent(i - 1) = ""
            Dim sngTemp As Single = Me.NetworkManager.PPTransferEfficiency(i)
            If (100.0 * sngTemp) > 0 Then
                strRowContent(i - 1) = (100.0 * sngTemp).ToString("F1")
                If i <= 4 Then TRavgP(i) = sngTemp
            End If
        Next
        Grid.Rows(1).SetValues(strRowContent)
        Grid.Rows(1).Visible = True

        strRowContent(0) = My.Resources.ROW_HDR_DET
        For i As Integer = 2 To NetworkManager.nTrophicLevels
            strRowContent(i - 1) = ""
            Dim sngTemp As Single = NetworkManager.DetTransferEfficiency(i)
            If (100.0 * sngTemp) > 0 Then
                strRowContent(i - 1) = (100.0 * sngTemp).ToString("F1")
                If i <= 4 Then TRavgD(i) = sngTemp
            End If
        Next
        Grid.Rows(2).SetValues(strRowContent)
        Grid.Rows(2).Visible = True

        strRowContent(0) = My.Resources.ROW_HDR_ALL_FLOWS
        For i As Integer = 2 To NetworkManager.nTrophicLevels
            Dim sngTr1 As Single = NetworkManager.PPConsByPred(i) + NetworkManager.DetConsByPred(i)
            If sngTr1 > 0 Then
                If NetworkManager.PPThroughtput(i) + NetworkManager.DetThroughtput(i) > 0 Then
                    NetworkManager.TrEm1(i) = sngTr1 / (NetworkManager.PPThroughtput(i) + NetworkManager.DetThroughtput(i))
                End If
            End If
            Dim sngTotTr As Single = NetworkManager.TotTransferEfficiency(i)
            If (sngTotTr > 0) Then
                strRowContent(i - 1) = (100.0 * sngTotTr).ToString("F1")
                If i <= 4 Then TRavgT(i) = sngTotTr
            Else
                NetworkManager.TrEm1(i) = 0
                strRowContent(i - 1) = ""
            End If
        Next i
        Grid.Rows(3).SetValues(strRowContent)
        Grid.Rows(3).Visible = True

        strRowContent(0) = My.Resources.STR_PROP_TOTAL_FLOW + NetworkManager.FlowFromDetritus.ToString("F2")
        For i As Integer = 1 To Grid.Columns.Count - 1
            strRowContent(i) = ""
        Next
        Grid.Rows(4).SetValues(strRowContent)
        Grid.Rows(4).Visible = True

        strRowContent(0) = My.Resources.STR_TRANSFER_EFF
        For i As Integer = 1 To Grid.Columns.Count - 1
            strRowContent(i) = ""
        Next
        Grid.Rows(5).SetValues(strRowContent)
        Grid.Rows(5).Visible = True

        If TRavgP(2) > 0 And TRavgP(3) > 0 And TRavgP(4) > 0 Then
            TRavgP(0) = CSng((TRavgP(2) * TRavgP(3) * TRavgP(4)) ^ (1 / 3))
            strRowContent(0) = My.Resources.STR_FROM_PRIM_PRODUCER + (100.0 * TRavgP(0)).ToString("F1") + "%"
        End If
        For i As Integer = 1 To Grid.Columns.Count - 1
            strRowContent(i) = ""
        Next
        Grid.Rows(6).SetValues(strRowContent)
        Grid.Rows(6).Visible = True

        If TRavgD(2) > 0 And TRavgD(3) > 0 And TRavgD(4) > 0 Then
            TRavgD(0) = CSng((TRavgD(2) * TRavgD(3) * TRavgD(4)) ^ (1 / 3))
            strRowContent(0) = My.Resources.STR_FROM_DET + (100.0 * TRavgD(0)).ToString("F1") + "%"
        End If
        For i As Integer = 1 To Grid.Columns.Count - 1
            strRowContent(i) = ""
        Next
        Grid.Rows(7).SetValues(strRowContent)
        Grid.Rows(7).Visible = True

        If TRavgT(2) > 0 And TRavgT(3) > 0 And TRavgT(4) > 0 Then
            TRavgT(0) = CSng((TRavgT(2) * TRavgT(3) * TRavgT(4)) ^ (1 / 3))
            strRowContent(0) = My.Resources.STR_TOTAL + (100.0 * TRavgT(0)).ToString("F1") + "%"
        End If
        For i As Integer = 1 To Grid.Columns.Count - 1
            strRowContent(i) = ""
        Next
        Grid.Rows(8).SetValues(strRowContent)
        Grid.Rows(8).Visible = True
        Grid.ClearSelection()
    End Sub

    Private Sub SetUpGridColumn(ByVal iNumTrophicLevel As Integer)

        Grid.ColumnCount = iNumTrophicLevel

        SetGridColumnPropertyDefault(Grid)

        Grid.Columns(0).Width = 330
        Grid.Columns(0).Frozen = True
        Grid.Columns(0).DefaultCellStyle.BackColor = Drawing.Color.MintCream
        Grid.Columns(0).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft

    End Sub

End Class
