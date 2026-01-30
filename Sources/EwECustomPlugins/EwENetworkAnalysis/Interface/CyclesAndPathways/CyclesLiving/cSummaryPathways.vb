' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Option Explicit On

Imports System.Windows.Forms
Imports ZedGraph
Imports ScientificInterfaceShared.Controls



Namespace CyclesLiving


    Public Class cSummaryPathways
        Inherits cContentManager

        Public Sub New()
            '
        End Sub

        Public Overrides Function PageTitle() As String
            ' ToDo: globalize this
            Return "Summary of cycles and pathways of all living groups"
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

            Me.SetUpGridColumn()

            'Set up grid rows
            Me.Grid.RowHeadersVisible = False
            Me.Grid.RowCount = 3
            Me.Grid.Rows(0).DefaultCellStyle.WrapMode = DataGridViewTriState.True
            Me.Grid.Rows(0).DefaultCellStyle.BackColor = Drawing.SystemColors.Control
            Me.Grid.Rows(0).Frozen = True
            Me.Grid.Rows(0).Height = FIRST_ROW_HEIGHT

            ReDim strRowContent(Me.Grid.Columns.Count)
            strRowContent(0) = My.Resources.COL_HDR_PARAM
            strRowContent(1) = My.Resources.COL_HDR_VALUE
            Me.Grid.Rows(0).SetValues(strRowContent)
            Me.Grid.Rows(0).Visible = True

            strRowContent(0) = My.Resources.ROW_HDR_TOTAL_NUM_PATH
            strRowContent(1) = CStr(Me.NetworkManager.PathWays.Count)
            Me.Grid.Rows(1).SetValues(strRowContent)
            Me.Grid.Rows(1).Visible = True

            strRowContent(0) = My.Resources.ROW_HDR_MEAN_PATH_LEN
            If Me.NetworkManager.PathWays.Count = 0 Then
                strRowContent(1) = My.Resources.ROW_HDR_NOT_APP
            Else
                strRowContent(1) = Me.StyleGuide.FormatNumber(Me.NetworkManager.NumArrows / Me.NetworkManager.PathWays.Count)
            End If
            Me.Grid.Rows(2).SetValues(strRowContent)
            Me.Grid.Rows(2).Visible = True

            Me.Grid.ClearSelection()

        End Sub

        Private Sub SetUpGridColumn()

            Me.Grid.ReadOnly = True
            Me.Grid.ColumnCount = 2

            SetGridColumnPropertyDefault(Me.Grid)

            Me.Grid.Columns(0).Frozen = True
            Me.Grid.Columns(0).DefaultCellStyle.BackColor = Drawing.SystemColors.Control
            Me.Grid.Columns(0).Width = 400

        End Sub

    End Class

End Namespace

