' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Option Explicit On

Imports System.Windows.Forms
Imports EwECore
Imports ZedGraph
Imports EwEUtils.Utilities
Imports ScientificInterfaceShared.Controls


Public Class cRelativeFlows
    Inherits cContentManager

    Public Sub New()
    End Sub

    Public Overrides Function PageTitle() As String
        ' ToDo: globalize this
        Return "Relative flows"
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

        Me.SetUpGridColumn(Me.NetworkManager.nTrophicLevels)

        'Set up grid rows
        Me.Grid.RowHeadersVisible = False
        Me.Grid.RowCount = Me.NetworkManager.nGroups + 1
        Me.Grid.Rows(0).DefaultCellStyle.WrapMode = DataGridViewTriState.True
        Me.Grid.Rows(0).DefaultCellStyle.BackColor = Drawing.SystemColors.Control
        Me.Grid.Rows(0).Frozen = True
        Me.Grid.Rows(0).Height = FIRST_ROW_HEIGHT

        ReDim astrRowContent(Me.Grid.Columns.Count)
        astrRowContent(0) = ""
        astrRowContent(1) = My.Resources.COL_HDR_GRP_NAME_TRP_LVL
        For j As Integer = 1 To Me.NetworkManager.nTrophicLevels
            astrRowContent(j + 1) = cStringUtils.ToRoman(j)
        Next
        Me.Grid.Rows(0).SetValues(astrRowContent)
        Me.Grid.Rows(0).Visible = True

        For i As Integer = 1 To Me.NetworkManager.nGroups
            astrRowContent(0) = CStr(i)
            astrRowContent(1) = Me.NetworkManager.GroupName(i)
            For j As Integer = 1 To Me.NetworkManager.nTrophicLevels
                astrRowContent(j + 1) = Me.StyleGuide.FormatNumber(Me.NetworkManager.RelativeFlow(i, j))
            Next
            'DataGrid.Rows.Add(strary)
            Me.Grid.Rows(i).SetValues(astrRowContent)
            Me.Grid.Rows(i).Visible = True

            'DataGrid.Rows(i - 1).HeaderCell.Value = CStr(i)
            'DataGrid.Rows(i - 1).HeaderCell.Style.BackColor = Drawing.Color.Beige
        Next
        Me.Grid.ClearSelection()

    End Sub

    Private Sub SetUpGridColumn(iNumTrophicLevels As Integer)

        'DataGrid.RowCount = 1
        Me.Grid.ColumnCount = iNumTrophicLevels + 2

        SetGridColumnPropertyDefault(Me.Grid)

        Me.Grid.Columns(0).DefaultCellStyle.BackColor = Drawing.SystemColors.Control
        Me.Grid.Columns(0).Frozen = True
        Me.Grid.Columns(0).Width = ID_COL_WIDTH '55

        Me.Grid.Columns(1).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
        Me.Grid.Columns(1).DefaultCellStyle.BackColor = Drawing.SystemColors.Control
        Me.Grid.Columns(1).Frozen = True
        Me.Grid.Columns(1).Width = GRP_NAME_COL_WIDTH

        'DataGrid.Rows(i - 1).HeaderCell.Value = CStr(i)
        'DataGrid.Rows(i - 1).HeaderCell.Style.BackColor = Drawing.Color.Beige

    End Sub

End Class
