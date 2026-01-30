' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Option Explicit On

Imports System.Windows.Forms
Imports ZedGraph
Imports EwEUtils.Utilities
Imports ScientificInterfaceShared.Controls




Public Class cFromPrimaryProd
    Inherits cContentManager

    Public Sub New()
    End Sub

    Public Overrides Function PageTitle() As String
        ' Todo: globalize this
        Return "Flows and biomasses from primary producers"
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

        Dim strRowContent() As String
        Dim sSum() As Single

        Me.SetUpGridColumn()

        'Set up grid rows
        Me.Grid.RowHeadersVisible = False
        Me.Grid.RowCount = Me.NetworkManager.nTrophicLevels + 2
        Me.Grid.Rows(0).DefaultCellStyle.WrapMode = DataGridViewTriState.True
        Me.Grid.Rows(0).DefaultCellStyle.BackColor = Drawing.SystemColors.Control
        Me.Grid.Rows(0).Frozen = True
        Me.Grid.Rows(0).Height = FIRST_ROW_HEIGHT

        ReDim strRowContent(Me.Grid.Columns.Count)
        ReDim sSum(Me.Grid.Columns.Count)
        strRowContent(0) = My.Resources.COL_HDR_TRP_LVL_FLOW
        strRowContent(1) = My.Resources.COL_HDR_IMPORT
        strRowContent(2) = My.Resources.COL_HDR_CONSUM_PREDAT
        strRowContent(3) = My.Resources.COL_HDR_EXPORT
        strRowContent(4) = My.Resources.COL_HDR_FLOW_DET
        strRowContent(5) = My.Resources.COL_HDR_RESP
        strRowContent(6) = My.Resources.COL_HDR_THROUGHPUT
        Me.Grid.Rows(0).SetValues(strRowContent)
        Me.Grid.Visible = True

        For i As Integer = Me.NetworkManager.nTrophicLevels To 1 Step -1
            'strRowContent(0) = CStr(i)
            strRowContent(0) = cStringUtils.ToRoman(i)
            If i = 1 Then
                strRowContent(1) = Me.StyleGuide.FormatNumber(Me.NetworkManager.PPImport(i))
                sSum(1) = sSum(1) + Me.NetworkManager.PPImport(i)
            Else
                strRowContent(1) = ""
            End If
            strRowContent(2) = Me.StyleGuide.FormatNumber(Me.NetworkManager.PPConsByPred(i))
            sSum(2) = sSum(2) + Me.NetworkManager.PPConsByPred(i)
            strRowContent(3) = Me.StyleGuide.FormatNumber(Me.NetworkManager.PPExport(i))
            sSum(3) = sSum(3) + Me.NetworkManager.PPExport(i)
            strRowContent(4) = Me.StyleGuide.FormatNumber(Me.NetworkManager.PPToDetritus(i))
            sSum(4) = sSum(4) + Me.NetworkManager.PPToDetritus(i)
            strRowContent(5) = Me.StyleGuide.FormatNumber(Me.NetworkManager.PPRespiration(i))
            sSum(5) = sSum(5) + Me.NetworkManager.PPRespiration(i)
            strRowContent(6) = Me.StyleGuide.FormatNumber(Me.NetworkManager.PPThroughtput(i))
            sSum(6) = sSum(6) + Me.NetworkManager.PPThroughtput(i)
            Me.Grid.Rows(Me.NetworkManager.nTrophicLevels - i + 1).SetValues(strRowContent)
            Me.Grid.Rows(Me.NetworkManager.nTrophicLevels - i + 1).Visible = True
        Next

        strRowContent(0) = My.Resources.ROW_HDR_SUM
        For i As Integer = 1 To Me.Grid.Columns.Count - 1
            strRowContent(i) = Me.StyleGuide.FormatNumber(sSum(i))
        Next
        Me.Grid.Rows(Me.Grid.RowCount - 1).SetValues(strRowContent)
        Me.Grid.Rows(Me.Grid.RowCount - 1).Visible = True
        Me.Grid.ClearSelection()
    End Sub

    Private Sub SetUpGridColumn()

        Me.Grid.ReadOnly = True
        'DataGrid.RowCount = 1
        Me.Grid.ColumnCount = 7

        SetGridColumnPropertyDefault(Me.Grid)

        Me.Grid.Columns(0).Frozen = True
        Me.Grid.Columns(0).DefaultCellStyle.BackColor = Drawing.SystemColors.Control

    End Sub

End Class
