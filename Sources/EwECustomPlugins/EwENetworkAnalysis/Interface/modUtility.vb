'==============================================================================
'
' $Log: modUtility.vb,v $
' Revision 1.4  2009/06/03 19:26:47  jeroens
' Moved ToRoman to EwEUtils
'
' Revision 1.3  2009/04/28 16:20:55  jeroens
' Fixed graph max axis
' Graph styling done with ZedGraphHelper
' Uses true roman number converter
'
' Revision 1.2  2008/11/25 05:47:34  joeh
' Copy and paste in cells of data grid view
'
' Revision 1.1  2008/09/26 07:30:58  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.12  2008/08/25 19:57:05  sherman
' Converted temp file to app temp files.
'
' Revision 1.11  2008/08/10 01:03:42  jeroens
' Updated to ds smarter open structure
'
' Revision 1.10  2008/08/02 03:04:20  jeroens
' Renamed resources
'
' Revision 1.9  2008/06/18 20:16:03  joeh
' Plot Ascendency on flow in a second pane
'
' Revision 1.8  2007/07/13 17:29:35  joeh
' Change variables to constant
'
' Revision 1.7  2007/07/07 00:11:03  joeh
' Decrease column width
'
' Revision 1.6  2007/06/26 21:14:28  joeh
' Diable sorting by column
'
' Revision 1.5  2007/06/22 00:35:32  joeh
' Add Option Strict On and Option Explicit On
'
' Revision 1.4  2007/06/20 18:13:57  joeh
' add header to the top of the file so that CVS will log the file with every update
'
'
'==============================================================================

Option Strict On
Option Explicit On

Imports ZedGraph
Imports System.Drawing
Imports System.IO
Imports System.Reflection
Imports System.Text

Module modUtility
    Public Const DEFAULT_COL_WIDTH As Integer = 70
    Public Const ID_COL_WIDTH As Integer = 25
    Public Const GRP_NAME_COL_WIDTH As Integer = 110
    Public Const FIRST_ROW_HEIGHT As Integer = 45

    Public Sub SetGridColumnPropertyDefault(ByVal DataGrid As Windows.Forms.DataGridView)
        DataGrid.ColumnHeadersVisible = False
        For intColIndex As Integer = 0 To DataGrid.ColumnCount - 1
            'DataGrid.Columns(intColIndex).HeaderCell.Style.Alignment = Windows.Forms.DataGridViewContentAlignment.MiddleCenter
            DataGrid.Columns(intColIndex).DefaultCellStyle.Alignment = Windows.Forms.DataGridViewContentAlignment.MiddleCenter
            DataGrid.Columns(intColIndex).DefaultCellStyle.BackColor = Drawing.Color.White
            DataGrid.Columns(intColIndex).Width = DEFAULT_COL_WIDTH '110
            DataGrid.Columns(intColIndex).Frozen = False
            DataGrid.Columns(intColIndex).SortMode = Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Next
    End Sub

    Public Sub AddCurve(ByVal strName As String, ByVal CurveVar() As Single, ByVal Pane As GraphPane, _
      ByVal MyColor As Color, Optional ByVal Symbol As SymbolType = SymbolType.None)
        Dim List As PointPairList
        Dim iNumPoints As Integer = CurveVar.GetUpperBound(0)

        List = New PointPairList()
        For iTime As Integer = 1 To iNumPoints
            List.Add(iTime, CurveVar(iTime))
        Next
        Pane.AddCurve(strName, List, MyColor, Symbol)

        Pane.XAxis.Scale.Max = iNumPoints
    End Sub

End Module
