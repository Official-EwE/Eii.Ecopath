'==============================================================================
'
' $Log: cFromDetritus.vb,v $
' Revision 1.6  2009/05/01 17:42:57  jeroens
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
' Revision 1.14  2008/06/25 01:53:41  joeh
' Ecosim NA indice plots are displayed in the same form where we have the NA tree view - Take 2
'
' Revision 1.13  2008/06/24 18:08:38  joeh
' Ecosim NA indice plots are displayed in the same form where  we have the NA tree view - Take 2
'
' Revision 1.12  2007/07/06 00:44:59  joeh
' Move hard coded strings to resource file
'
' Revision 1.11  2007/06/28 19:22:09  joeh
' Switch to wait cursor when displaying data
'
' Revision 1.10  2007/06/22 19:12:46  joeh
' Modify GetInstance()
'
' Revision 1.9  2007/06/22 00:35:29  joeh
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

Public Class cFromDetritus
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
            strRowContent(0) = CRoman(i)
            If i = 1 Then
                strRowContent(1) = NetworkManager.DetImport(i).ToString("F4")
                sngSumVariable(1) = sngSumVariable(1) + NetworkManager.DetImport(i)
            Else
                strRowContent(1) = ""
            End If
            strRowContent(2) = NetworkManager.DetConsByPred(i).ToString("F4")
            sngSumVariable(2) = sngSumVariable(2) + NetworkManager.DetConsByPred(i)
            strRowContent(3) = NetworkManager.DetExport(i).ToString("F4")
            sngSumVariable(3) = sngSumVariable(3) + NetworkManager.DetExport(i)
            strRowContent(4) = NetworkManager.DetToDetritus(i).ToString("F4")
            sngSumVariable(4) = sngSumVariable(4) + NetworkManager.DetToDetritus(i)
            strRowContent(5) = NetworkManager.DetRespiration(i).ToString("F4")
            sngSumVariable(5) = sngSumVariable(5) + NetworkManager.DetRespiration(i)
            strRowContent(6) = NetworkManager.DetThroughtput(i).ToString("F4")
            sngSumVariable(6) = sngSumVariable(6) + NetworkManager.DetThroughtput(i)
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
