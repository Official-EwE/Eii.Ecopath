'==============================================================================
'
' $Log: cFromAllCombined.vb,v $
' Revision 1.9  2009/05/30 00:00:52  jeroens
' Toolstrip usage centralized
'
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
                                     ByVal plot As ucPlot, _
                                     ByVal toolstrip As ToolStrip) As Boolean
        Dim bSucces As Boolean = MyBase.Attach(manager, datagrid, graph, plot, toolstrip)
        Me.Grid.Visible = bSucces
        Return bSucces
    End Function

    Public Overrides Sub DisplayData()

        Dim astrRowContent() As String
        Dim asSum() As Single

        SetUpGridColumn()

        'Set up grid rows
        Grid.RowHeadersVisible = False
        Grid.RowCount = NetworkManager.nTrophicLevels + 5
        Grid.Rows(0).DefaultCellStyle.WrapMode = DataGridViewTriState.True
        Grid.Rows(0).DefaultCellStyle.BackColor = Drawing.Color.MintCream
        Grid.Rows(0).Frozen = True
        Grid.Rows(0).Height = FIRST_ROW_HEIGHT

        ReDim astrRowContent(Grid.Columns.Count)
        ReDim asSum(Grid.Columns.Count)
        astrRowContent(0) = My.Resources.COL_HDR_TRP_LVL_FLOW
        astrRowContent(1) = My.Resources.COL_HDR_IMPORT
        astrRowContent(2) = My.Resources.COL_HDR_CONSUM_PREDAT
        astrRowContent(3) = My.Resources.COL_HDR_EXPORT
        astrRowContent(4) = My.Resources.COL_HDR_FLOW_DET
        astrRowContent(5) = My.Resources.COL_HDR_RESP
        astrRowContent(6) = My.Resources.COL_HDR_THROUGHPUT
        Grid.Rows(0).SetValues(astrRowContent)
        Grid.Rows(0).Visible = True

        For i As Integer = NetworkManager.nTrophicLevels To 1 Step -1
            astrRowContent(0) = CRoman(i)
            If i = 1 Then
                astrRowContent(1) = Me.StyleGuide.FormatNumber(NetworkManager.DetImport(i) + NetworkManager.PPImport(i))
                asSum(1) = asSum(1) + NetworkManager.DetImport(i) + NetworkManager.PPImport(i)
            Else
                astrRowContent(1) = ""
            End If
            astrRowContent(2) = Me.StyleGuide.FormatNumber(NetworkManager.DetConsByPred(i) + NetworkManager.PPConsByPred(i))
            asSum(2) = asSum(2) + NetworkManager.DetConsByPred(i) + NetworkManager.PPConsByPred(i)
            astrRowContent(3) = Me.StyleGuide.FormatNumber(NetworkManager.DetExport(i) + NetworkManager.PPExport(i))
            asSum(3) = asSum(3) + NetworkManager.DetExport(i) + NetworkManager.PPExport(i)
            astrRowContent(4) = Me.StyleGuide.FormatNumber(NetworkManager.DetToDetritus(i) + NetworkManager.PPToDetritus(i))
            asSum(4) = asSum(4) + NetworkManager.DetToDetritus(i) + NetworkManager.PPToDetritus(i)
            astrRowContent(5) = Me.StyleGuide.FormatNumber(NetworkManager.DetRespiration(i) + NetworkManager.PPRespiration(i))
            asSum(5) = asSum(5) + NetworkManager.DetRespiration(i) + NetworkManager.PPRespiration(i)
            astrRowContent(6) = Me.StyleGuide.FormatNumber(NetworkManager.DetThroughtput(i) + NetworkManager.PPThroughtput(i))
            asSum(6) = asSum(6) + NetworkManager.DetThroughtput(i) + NetworkManager.PPThroughtput(i)
            Grid.Rows(NetworkManager.nTrophicLevels - i + 1).SetValues(astrRowContent)
            Grid.Rows(NetworkManager.nTrophicLevels - i + 1).Visible = True
        Next

        astrRowContent(0) = My.Resources.ROW_HDR_SUM
        For i As Integer = 1 To Grid.Columns.Count - 1
            astrRowContent(i) = Me.StyleGuide.FormatNumber(asSum(i))
        Next
        Grid.Rows(Grid.RowCount - 4).SetValues(astrRowContent)
        Grid.Rows(Grid.RowCount - 4).Visible = True

        astrRowContent(0) = My.Resources.ROW_HDR_EXTRACT_BREAK_CYC
        For i As Integer = 1 To Grid.Columns.Count - 2
            astrRowContent(i) = ""
        Next
        astrRowContent(Grid.Columns.Count - 1) = Me.StyleGuide.FormatNumber(NetworkManager.ExtractedToBreakCycles)
        Grid.Rows(Grid.RowCount - 3).SetValues(astrRowContent)
        Grid.Rows(Grid.RowCount - 3).Visible = True

        astrRowContent(0) = My.Resources.ROW_HDR_INPUT_TRP_LVL_II_PLUS
        For i As Integer = 1 To Grid.Columns.Count - 2
            astrRowContent(i) = ""
        Next
        astrRowContent(Grid.Columns.Count - 1) = Me.StyleGuide.FormatNumber(NetworkManager.InputTLIIPlus)
        Grid.Rows(Grid.RowCount - 2).SetValues(astrRowContent)
        Grid.Rows(Grid.RowCount - 2).Visible = True

        astrRowContent(0) = My.Resources.ROW_HDR_TOTAL_THROUGHPUT
        For i As Integer = 1 To Grid.Columns.Count - 2
            astrRowContent(i) = ""
        Next
        astrRowContent(Grid.Columns.Count - 1) = Me.StyleGuide.FormatNumber(NetworkManager.TotalThroughput + _
            NetworkManager.ExtractedToBreakCycles + NetworkManager.InputTLIIPlus)
        Grid.Rows(Grid.RowCount - 1).SetValues(astrRowContent)
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
