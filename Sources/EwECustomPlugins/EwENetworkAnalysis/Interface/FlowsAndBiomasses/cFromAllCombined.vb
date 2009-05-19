'==============================================================================
'
' $Log: cFromAllCombined.vb,v $
' Revision 1.8  2009/05/19 13:41:12  jeroens
' Content manager derived pages will take care of updating NA run state
'
' Revision 1.7  2009/05/02 01:51:27  jeroens
' Updated to cControlManager FN name change
'
' Revision 1.6  2009/05/01 17:42:57  jeroens
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
' Revision 1.1  2008/09/26 07:30:52  sherman
' --== DELETED HISTORY ==--
'
'==============================================================================
Option Strict On
Option Explicit On

Imports System.Windows.Forms
Imports ZedGraph

Public Class cFromAllCombined
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
        Dim sngSumVariable() As Single

        SetUpGridColumn()

        'Set up grid rows
        Grid.RowHeadersVisible = False
        Grid.RowCount = NetworkManager.nTrophicLevels + 5
        Grid.Rows(0).DefaultCellStyle.WrapMode = DataGridViewTriState.True
        Grid.Rows(0).DefaultCellStyle.BackColor = Drawing.Color.MintCream
        Grid.Rows(0).Frozen = True
        Grid.Rows(0).Height = FIRST_ROW_HEIGHT

        ReDim strRowContent(Grid.Columns.Count)
        ReDim sngSumVariable(Grid.Columns.Count)
        strRowContent(0) = My.Resources.COL_HDR_TRP_LVL_FLOW
        strRowContent(1) = My.Resources.COL_HDR_IMPORT
        strRowContent(2) = My.Resources.COL_HDR_CONSUM_PREDAT
        strRowContent(3) = My.Resources.COL_HDR_EXPORT
        strRowContent(4) = My.Resources.COL_HDR_FLOW_DET
        strRowContent(5) = My.Resources.COL_HDR_RESP
        strRowContent(6) = My.Resources.COL_HDR_THROUGHPUT
        Grid.Rows(0).SetValues(strRowContent)
        Grid.Rows(0).Visible = True

        For i As Integer = NetworkManager.nTrophicLevels To 1 Step -1
            strRowContent(0) = CRoman(i)
            If i = 1 Then
                strRowContent(1) = (NetworkManager.DetImport(i) + NetworkManager.PPImport(i)).ToString("F4")
                sngSumVariable(1) = sngSumVariable(1) + NetworkManager.DetImport(i) + NetworkManager.PPImport(i)
            Else
                strRowContent(1) = ""
            End If
            strRowContent(2) = (NetworkManager.DetConsByPred(i) + NetworkManager.PPConsByPred(i)).ToString("F4")
            sngSumVariable(2) = sngSumVariable(2) + NetworkManager.DetConsByPred(i) + NetworkManager.PPConsByPred(i)
            strRowContent(3) = (NetworkManager.DetExport(i) + NetworkManager.PPExport(i)).ToString("F4")
            sngSumVariable(3) = sngSumVariable(3) + NetworkManager.DetExport(i) + NetworkManager.PPExport(i)
            strRowContent(4) = (NetworkManager.DetToDetritus(i) + NetworkManager.PPToDetritus(i)).ToString("F4")
            sngSumVariable(4) = sngSumVariable(4) + NetworkManager.DetToDetritus(i) + NetworkManager.PPToDetritus(i)
            strRowContent(5) = (NetworkManager.DetRespiration(i) + NetworkManager.PPRespiration(i)).ToString("F4")
            sngSumVariable(5) = sngSumVariable(5) + NetworkManager.DetRespiration(i) + NetworkManager.PPRespiration(i)
            strRowContent(6) = (NetworkManager.DetThroughtput(i) + NetworkManager.PPThroughtput(i)).ToString("F4")
            sngSumVariable(6) = sngSumVariable(6) + NetworkManager.DetThroughtput(i) + NetworkManager.PPThroughtput(i)
            Grid.Rows(NetworkManager.nTrophicLevels - i + 1).SetValues(strRowContent)
            Grid.Rows(NetworkManager.nTrophicLevels - i + 1).Visible = True
        Next

        strRowContent(0) = My.Resources.ROW_HDR_SUM
        For i As Integer = 1 To Grid.Columns.Count - 1
            strRowContent(i) = sngSumVariable(i).ToString("F4")
        Next
        Grid.Rows(Grid.RowCount - 4).SetValues(strRowContent)
        Grid.Rows(Grid.RowCount - 4).Visible = True

        strRowContent(0) = My.Resources.ROW_HDR_EXTRACT_BREAK_CYC
        For i As Integer = 1 To Grid.Columns.Count - 2
            strRowContent(i) = ""
        Next
        strRowContent(Grid.Columns.Count - 1) = NetworkManager.ExtractedToBreakCycles.ToString("F4")
        Grid.Rows(Grid.RowCount - 3).SetValues(strRowContent)
        Grid.Rows(Grid.RowCount - 3).Visible = True

        strRowContent(0) = My.Resources.ROW_HDR_INPUT_TRP_LVL_II_PLUS
        For i As Integer = 1 To Grid.Columns.Count - 2
            strRowContent(i) = ""
        Next
        strRowContent(Grid.Columns.Count - 1) = NetworkManager.InputTLIIPlus.ToString("F4")
        Grid.Rows(Grid.RowCount - 2).SetValues(strRowContent)
        Grid.Rows(Grid.RowCount - 2).Visible = True

        strRowContent(0) = My.Resources.ROW_HDR_TOTAL_THROUGHPUT
        For i As Integer = 1 To Grid.Columns.Count - 2
            strRowContent(i) = ""
        Next
        strRowContent(Grid.Columns.Count - 1) = (NetworkManager.TotalThroughput + _
            NetworkManager.ExtractedToBreakCycles + NetworkManager.InputTLIIPlus).ToString("F4")
        Grid.Rows(Grid.RowCount - 1).SetValues(strRowContent)
        Grid.Rows(Grid.RowCount - 1).Visible = True
        Grid.ClearSelection()
        Cursor.Current = Cursors.Default

    End Sub

    Private Sub SetUpGridColumn()

        Grid.ReadOnly = True
        'DataGrid.RowCount = 1
        Grid.ColumnCount = 7

        SetGridColumnPropertyDefault(Grid)

        Grid.Columns(0).Width = 160
        Grid.Columns(0).Frozen = True
        Grid.Columns(0).DefaultCellStyle.BackColor = Drawing.Color.MintCream

    End Sub

End Class
