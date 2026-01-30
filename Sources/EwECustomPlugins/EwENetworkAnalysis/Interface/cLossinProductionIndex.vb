' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Option Explicit On

Imports System.Windows.Forms
Imports EwECore
Imports ZedGraph
Imports ScientificInterfaceShared.Controls




Public Class cLossinProductionIndex
    Inherits cContentManager

    Public Sub New()
        '
    End Sub

    Public Overrides Function PageTitle() As String
        ' ToDo: globalize this
        Return "Loss of Production index"
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
        Dim LindexTot As Single = 0

        Me.NetworkManager.RunRequiredPrimaryProd()

        Me.SetUpGridColumn()

        'Set up grid rows
        Me.Grid.RowHeadersVisible = False
        Me.Grid.RowCount = Me.NetworkManager.nLivingGroups + 2
        Me.Grid.Rows(0).DefaultCellStyle.WrapMode = DataGridViewTriState.True
        Me.Grid.Rows(0).DefaultCellStyle.BackColor = Drawing.SystemColors.Control
        Me.Grid.Rows(0).Frozen = True
        Me.Grid.Rows(0).Height = FIRST_ROW_HEIGHT

        ReDim astrRowContent(Me.Grid.Columns.Count)
        astrRowContent(0) = ""
        astrRowContent(1) = My.Resources.COL_HDR_GRP_NAME
        astrRowContent(2) = My.Resources.COL_HDR_LINDEX
        astrRowContent(3) = My.Resources.COL_HDR_PSUST
        'astrRowContent(4) = My.Resources.COL_HDR_PSUST_SDLOWER
        'astrRowContent(5) = My.Resources.COL_HDR_PSUST_SDUPPER

        Me.Grid.Rows(0).SetValues(astrRowContent)
        Me.Grid.Rows(0).Visible = True

        For i As Integer = 1 To Me.NetworkManager.nLivingGroups
            astrRowContent(0) = CStr(i)
            astrRowContent(1) = Me.NetworkManager.GroupName(i)
            astrRowContent(2) = Me.StyleGuide.FormatNumber(Me.NetworkManager.Lindex(i))
            astrRowContent(3) = Me.StyleGuide.FormatNumber(Me.NetworkManager.Psust(i))
            'astrRowContent(4) = Me.StyleGuide.FormatNumber(NetworkManager.PsustSDlower(i))
            'astrRowContent(5) = Me.StyleGuide.FormatNumber(NetworkManager.PsustSDupper(i))
            LindexTot += Me.NetworkManager.Lindex(i)
            Me.Grid.Rows(i).SetValues(astrRowContent)
            Me.Grid.Rows(i).Visible = True
        Next

        astrRowContent(0) = ""
        astrRowContent(1) = My.Resources.ROW_HDR_TOTAL
        astrRowContent(2) = Me.StyleGuide.FormatNumber(LindexTot)
        astrRowContent(3) = Me.StyleGuide.FormatNumber(Me.NetworkManager.CalcPsust(LindexTot))
        Me.Grid.Rows(Me.NetworkManager.nLivingGroups + 1).SetValues(astrRowContent)

        For i As Integer = 1 To Me.NetworkManager.Core.nLivingGroups
            If Me.NetworkManager.PPRCatchHarvest(i) <= 0.0 Or
                Me.NetworkManager.PPRCatchHarvest(i) <= 0.0 And Me.NetworkManager.TotalPrimaryProduction <= 0.0 Then
                Me.Grid.Rows(i).Visible = False
            End If
        Next


        Me.Grid.ClearSelection()

    End Sub

    Private Sub SetUpGridColumn()

        'DataGrid.RowCount = 1
        Me.Grid.ColumnCount = 4

        SetGridColumnPropertyDefault(Me.Grid)

        Me.Grid.Columns(0).DefaultCellStyle.BackColor = Drawing.SystemColors.Control
        Me.Grid.Columns(0).Frozen = True
        Me.Grid.Columns(0).Width = ID_COL_WIDTH

        Me.Grid.Columns(1).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
        Me.Grid.Columns(1).DefaultCellStyle.BackColor = Drawing.SystemColors.Control
        Me.Grid.Columns(1).Frozen = True
        Me.Grid.Columns(1).Width = GRP_NAME_COL_WIDTH

    End Sub

End Class
