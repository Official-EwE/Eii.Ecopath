'==============================================================================
'
' $Log: cFromPrimaryProd.vb,v $
' Revision 1.6  2009/05/01 17:42:58  jeroens
' Inherited from cContentManager
'
' Revision 1.5  2009/04/17 01:07:04  joeh
' Make MixedTrophicImpactUC not visible when needed
'
' Revision 1.4  2009/04/15 23:37:38  joeh
' Add "Imports System.Windows.Forms" statement
'
' Revision 1.3  2009/04/15 18:14:53  joeh
' Set m_Panel.AutoScroll = False
'
' Revision 1.2  2008/11/25 20:55:41  joeh
' Copy and paste in cells of data grid view
'
' Revision 1.1  2008/09/26 07:30:53  sherman
' --== DELETED HISTORY ==--
'
'==============================================================================

Option Strict On
Option Explicit On

Imports System.Windows.Forms
Imports ZedGraph

Public Class cFromPrimaryProd
    Inherits cContentManager

    Public Sub New()
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
        Dim sngSumVariable() As Single

        SetUpGridColumn()

        'Set up grid rows
        DataGrid.RowHeadersVisible = False
        DataGrid.RowCount = NetworkManager.nTrophicLevels + 2
        DataGrid.Rows(0).DefaultCellStyle.WrapMode = DataGridViewTriState.True
        DataGrid.Rows(0).DefaultCellStyle.BackColor = Drawing.Color.MintCream
        DataGrid.Rows(0).Frozen = True
        DataGrid.Rows(0).Height = FIRST_ROW_HEIGHT

        ReDim strRowContent(DataGrid.Columns.Count)
        ReDim sngSumVariable(DataGrid.Columns.Count)
        strRowContent(0) = My.Resources.COL_HDR_TRP_LVL_FLOW
        strRowContent(1) = My.Resources.COL_HDR_IMPORT
        strRowContent(2) = My.Resources.COL_HDR_CONSUM_PREDAT
        strRowContent(3) = My.Resources.COL_HDR_EXPORT
        strRowContent(4) = My.Resources.COL_HDR_FLOW_DET
        strRowContent(5) = My.Resources.COL_HDR_RESP
        strRowContent(6) = My.Resources.COL_HDR_THROUGHPUT
        DataGrid.Rows(0).SetValues(strRowContent)
        DataGrid.Visible = True

        For i As Integer = NetworkManager.nTrophicLevels To 1 Step -1
            'strRowContent(0) = CStr(i)
            strRowContent(0) = CRoman(i)
            If i = 1 Then
                strRowContent(1) = NetworkManager.PPImport(i).ToString("F4")
                sngSumVariable(1) = sngSumVariable(1) + NetworkManager.PPImport(i)
            Else
                strRowContent(1) = ""
            End If
            strRowContent(2) = NetworkManager.PPConsByPred(i).ToString("F4")
            sngSumVariable(2) = sngSumVariable(2) + NetworkManager.PPConsByPred(i)
            strRowContent(3) = NetworkManager.PPExport(i).ToString("F4")
            sngSumVariable(3) = sngSumVariable(3) + NetworkManager.PPExport(i)
            strRowContent(4) = NetworkManager.PPToDetritus(i).ToString("F4")
            sngSumVariable(4) = sngSumVariable(4) + NetworkManager.PPToDetritus(i)
            strRowContent(5) = NetworkManager.PPRespiration(i).ToString("F4")
            sngSumVariable(5) = sngSumVariable(5) + NetworkManager.PPRespiration(i)
            strRowContent(6) = NetworkManager.PPThroughtput(i).ToString("F4")
            sngSumVariable(6) = sngSumVariable(6) + NetworkManager.PPThroughtput(i)
            DataGrid.Rows(NetworkManager.nTrophicLevels - i + 1).SetValues(strRowContent)
            DataGrid.Rows(NetworkManager.nTrophicLevels - i + 1).Visible = True
        Next

        strRowContent(0) = My.Resources.ROW_HDR_SUM
        For i As Integer = 1 To DataGrid.Columns.Count - 1
            strRowContent(i) = sngSumVariable(i).ToString("F4")
        Next
        DataGrid.Rows(DataGrid.RowCount - 1).SetValues(strRowContent)
        DataGrid.Rows(DataGrid.RowCount - 1).Visible = True
        DataGrid.ClearSelection()
    End Sub

    Private Sub SetUpGridColumn()

        DataGrid.ReadOnly = True
        'DataGrid.RowCount = 1
        DataGrid.ColumnCount = 7

        SetGridColumnPropertyDefault(DataGrid)

        DataGrid.Columns(0).Frozen = True
        DataGrid.Columns(0).DefaultCellStyle.BackColor = Drawing.Color.MintCream

    End Sub

End Class
