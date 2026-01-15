' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 3 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see https://www.gnu.org/licenses/gpl-3.0.html>. 
'
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'




Option Explicit On

Imports System.Windows.Forms
Imports ZedGraph
Imports EwEUtils.Utilities
Imports ScientificInterfaceShared.Controls




Public Class cFromAllCombined
    Inherits cContentManager

    Public Sub New()
    End Sub

    Public Overrides Function PageTitle() As String
        ' Todo: globalize this
        Return "Flows and biomasses from all, combined"
    End Function

    Public Overrides Function Attach(manager As cNetworkManager,
                                     datagrid As DataGridView,
                                     graph As ZedGraphControl,
                                     plot As ucPlot,
                                     toolstrip As ToolStrip,
                                     info As Control,
                                     uic As cUIContext) As Boolean
        Dim bSucces As Boolean = MyBase.Attach(manager, datagrid, graph, plot, toolstrip, info, uic)
        Me.Grid.Visible = bSucces
        Return bSucces
    End Function

    Public Overrides Sub DisplayData()

        Dim astrRowContent() As String
        Dim asSum() As Single

        Me.SetUpGridColumn()

        'Set up grid rows
        Me.Grid.RowHeadersVisible = False
        Me.Grid.RowCount = Me.NetworkManager.nTrophicLevels + 5
        Me.Grid.Rows(0).DefaultCellStyle.WrapMode = DataGridViewTriState.True
        Me.Grid.Rows(0).DefaultCellStyle.BackColor = Drawing.SystemColors.Control
        Me.Grid.Rows(0).Frozen = True
        Me.Grid.Rows(0).Height = FIRST_ROW_HEIGHT

        ReDim astrRowContent(Me.Grid.Columns.Count)
        ReDim asSum(Me.Grid.Columns.Count)
        astrRowContent(0) = My.Resources.COL_HDR_TRP_LVL_FLOW
        astrRowContent(1) = My.Resources.COL_HDR_IMPORT
        astrRowContent(2) = My.Resources.COL_HDR_CONSUM_PREDAT
        astrRowContent(3) = My.Resources.COL_HDR_EXPORT
        astrRowContent(4) = My.Resources.COL_HDR_FLOW_DET
        astrRowContent(5) = My.Resources.COL_HDR_RESP
        astrRowContent(6) = My.Resources.COL_HDR_THROUGHPUT
        Me.Grid.Rows(0).SetValues(astrRowContent)
        Me.Grid.Rows(0).Visible = True

        For i As Integer = Me.NetworkManager.nTrophicLevels To 1 Step -1
            astrRowContent(0) = cStringUtils.ToRoman(i)
            If i = 1 Then
                astrRowContent(1) = Me.StyleGuide.FormatNumber(Me.NetworkManager.DetImport(i) + Me.NetworkManager.PPImport(i))
                asSum(1) = asSum(1) + Me.NetworkManager.DetImport(i) + Me.NetworkManager.PPImport(i)
            Else
                astrRowContent(1) = ""
            End If
            astrRowContent(2) = Me.StyleGuide.FormatNumber(Me.NetworkManager.DetConsByPred(i) + Me.NetworkManager.PPConsByPred(i))
            asSum(2) = asSum(2) + Me.NetworkManager.DetConsByPred(i) + Me.NetworkManager.PPConsByPred(i)
            astrRowContent(3) = Me.StyleGuide.FormatNumber(Me.NetworkManager.DetExport(i) + Me.NetworkManager.PPExport(i))
            asSum(3) = asSum(3) + Me.NetworkManager.DetExport(i) + Me.NetworkManager.PPExport(i)
            astrRowContent(4) = Me.StyleGuide.FormatNumber(Me.NetworkManager.DetToDetritus(i) + Me.NetworkManager.PPToDetritus(i))
            asSum(4) = asSum(4) + Me.NetworkManager.DetToDetritus(i) + Me.NetworkManager.PPToDetritus(i)
            astrRowContent(5) = Me.StyleGuide.FormatNumber(Me.NetworkManager.DetRespiration(i) + Me.NetworkManager.PPRespiration(i))
            asSum(5) = asSum(5) + Me.NetworkManager.DetRespiration(i) + Me.NetworkManager.PPRespiration(i)
            astrRowContent(6) = Me.StyleGuide.FormatNumber(Me.NetworkManager.DetThroughtput(i) + Me.NetworkManager.PPThroughtput(i))
            asSum(6) = asSum(6) + Me.NetworkManager.DetThroughtput(i) + Me.NetworkManager.PPThroughtput(i)
            Me.Grid.Rows(Me.NetworkManager.nTrophicLevels - i + 1).SetValues(astrRowContent)
            Me.Grid.Rows(Me.NetworkManager.nTrophicLevels - i + 1).Visible = True
        Next

        astrRowContent(0) = My.Resources.ROW_HDR_SUM
        For i As Integer = 1 To Me.Grid.Columns.Count - 1
            astrRowContent(i) = Me.StyleGuide.FormatNumber(asSum(i))
        Next
        Me.Grid.Rows(Me.Grid.RowCount - 4).SetValues(astrRowContent)
        Me.Grid.Rows(Me.Grid.RowCount - 4).Visible = True

        astrRowContent(0) = My.Resources.ROW_HDR_EXTRACT_BREAK_CYC
        For i As Integer = 1 To Me.Grid.Columns.Count - 2
            astrRowContent(i) = ""
        Next
        astrRowContent(Me.Grid.Columns.Count - 1) = Me.StyleGuide.FormatNumber(Me.NetworkManager.ExtractedToBreakCycles)
        Me.Grid.Rows(Me.Grid.RowCount - 3).SetValues(astrRowContent)
        Me.Grid.Rows(Me.Grid.RowCount - 3).Visible = True

        astrRowContent(0) = My.Resources.ROW_HDR_INPUT_TRP_LVL_II_PLUS
        For i As Integer = 1 To Me.Grid.Columns.Count - 2
            astrRowContent(i) = ""
        Next
        astrRowContent(Me.Grid.Columns.Count - 1) = Me.StyleGuide.FormatNumber(Me.NetworkManager.InputTLIIPlus)
        Me.Grid.Rows(Me.Grid.RowCount - 2).SetValues(astrRowContent)
        Me.Grid.Rows(Me.Grid.RowCount - 2).Visible = True

        astrRowContent(0) = My.Resources.ROW_HDR_TOTAL_THROUGHPUT
        For i As Integer = 1 To Me.Grid.Columns.Count - 2
            astrRowContent(i) = ""
        Next
        astrRowContent(Me.Grid.Columns.Count - 1) = Me.StyleGuide.FormatNumber(Me.NetworkManager.TotalThroughput + _
            Me.NetworkManager.ExtractedToBreakCycles + Me.NetworkManager.InputTLIIPlus)
        Me.Grid.Rows(Me.Grid.RowCount - 1).SetValues(astrRowContent)
        Me.Grid.Rows(Me.Grid.RowCount - 1).Visible = True
        Me.Grid.ClearSelection()
        Cursor.Current = Cursors.Default

    End Sub

    Private Sub SetUpGridColumn()

        Me.Grid.ReadOnly = True
        'DataGrid.RowCount = 1
        Me.Grid.ColumnCount = 7

        SetGridColumnPropertyDefault(Me.Grid)

        Me.Grid.Columns(0).Width = 160
        Me.Grid.Columns(0).Frozen = True
        Me.Grid.Columns(0).DefaultCellStyle.BackColor = Drawing.SystemColors.Control

    End Sub

End Class
