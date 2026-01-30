' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Option Explicit On

Imports System.Windows.Forms
Imports ZedGraph
Imports ScientificInterfaceShared.Controls



Namespace TL1ToConsumer


    Public Class cCyclesPathways
        Inherits cContentManager

        Public Sub New()
        End Sub

        Public Overrides Function PageTitle() As String
            ' ToDo: globalize this
            Return "Cycles and pathways TL1 to consumer"
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
            Me.Toolstrip.Visible = bSucces
            Me.ToolstripShowGroupSelections(My.Resources.LBL_PATH_TO, eGroupFilterTypes.Living)
            Return bSucces
        End Function

        Public Overrides Sub DisplayData()

            Me.Grid.ReadOnly = True
            Me.Grid.ColumnCount = 2

            SetGridColumnPropertyDefault(Me.Grid)

            Me.Grid.Columns(0).Frozen = True
            Me.Grid.Columns(0).DefaultCellStyle.BackColor = Drawing.SystemColors.Control

            Me.Grid.Columns(1).Width = 660
            Me.Grid.Columns(1).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft

        End Sub

        Public Overrides Sub UpdateData(iSel1 As Integer, iSel2 As Integer)

            Dim strRowContent() As String

            Me.Grid.RowHeadersVisible = False

            ReDim strRowContent(Me.Grid.Columns.Count)
            Me.NetworkManager.FindPathwaysToConsumer(iSel1)
            If Me.NetworkManager.PathWays.Count > 0 Then
                Me.Grid.RowCount = Me.NetworkManager.PathWays.Count + 1
                Me.Grid.Rows(0).DefaultCellStyle.WrapMode = DataGridViewTriState.True
                Me.Grid.Rows(0).DefaultCellStyle.BackColor = Drawing.SystemColors.Control
                Me.Grid.Rows(0).Frozen = True
                Me.Grid.Rows(0).Height = FIRST_ROW_HEIGHT

                strRowContent(0) = My.Resources.COL_HDR_PATH_NUM
                strRowContent(1) = My.Resources.COL_HDR_PATH_CONSUM
                Me.Grid.Rows(0).SetValues(strRowContent)
                Me.Grid.Rows(0).Visible = True

                For intPathwayIndex As Integer = 0 To Me.NetworkManager.PathWays.Count - 1
                    strRowContent(0) = CStr(intPathwayIndex + 1)
                    strRowContent(1) = CStr(Me.NetworkManager.PathWays.Item(intPathwayIndex))
                    Me.Grid.Rows(intPathwayIndex + 1).SetValues(strRowContent)
                    Me.Grid.Rows(intPathwayIndex + 1).Visible = True
                Next
            Else
                Me.Grid.RowCount = 2
                Me.Grid.Rows(0).DefaultCellStyle.WrapMode = DataGridViewTriState.True
                Me.Grid.Rows(0).DefaultCellStyle.BackColor = Drawing.SystemColors.Control
                Me.Grid.Rows(0).Frozen = True
                Me.Grid.Rows(0).Height = FIRST_ROW_HEIGHT

                strRowContent(0) = My.Resources.COL_HDR_PATH_NUM
                strRowContent(1) = My.Resources.COL_HDR_PATH_CONSUM
                Me.Grid.Rows(0).SetValues(strRowContent)
                Me.Grid.Rows(0).Visible = True

                strRowContent(0) = My.Resources.ROW_HDR_NO_PATH_FOUND
                strRowContent(1) = ""
                Me.Grid.Rows(1).SetValues(strRowContent)
                Me.Grid.Rows(1).Visible = True
            End If
            Me.Grid.ClearSelection()
        End Sub

    End Class

End Namespace

