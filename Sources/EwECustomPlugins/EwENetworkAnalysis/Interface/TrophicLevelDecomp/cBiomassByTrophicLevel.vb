' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Option Explicit On

Imports System.Windows.Forms
Imports EwECore
Imports ZedGraph
Imports EwEUtils.Utilities
Imports ScientificInterfaceShared.Controls




Public Class cBiomassByTrophicLevel
    Inherits cContentManager

    Public Sub New()
        '
    End Sub

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
        Me.ToolstripShowDisplayGroups(bSucces)
        Return bSucces
    End Function

    Public Overrides Function PageTitle() As String
        ' ToDo: globalize this
        Return "Biomass by tropic level"
    End Function

    Public Overrides Sub DisplayData()

        Dim astrRowContent() As String

        Dim core As cCore = Me.UIContext.Core
        Dim bShowItem As Boolean = True
        Dim asBiomassGroupsShown() As Single
        Dim asMassDetritusShown() As Single

        Me.SetUpGridColumn()

        'Set up grid rows
        Me.Grid.RowHeadersVisible = False
        Me.Grid.RowCount = Me.NetworkManager.nTrophicLevels + 1
        Me.Grid.Rows(0).DefaultCellStyle.WrapMode = DataGridViewTriState.True
        Me.Grid.Rows(0).DefaultCellStyle.BackColor = Drawing.SystemColors.Control
        Me.Grid.Rows(0).Frozen = True
        Me.Grid.Rows(0).Height = FIRST_ROW_HEIGHT

        'Calculate non-hidden data
        ReDim asBiomassGroupsShown(Me.NetworkManager.nGroups)
        ReDim asMassDetritusShown(Me.NetworkManager.nGroups)

        For i As Integer = 1 To Me.NetworkManager.nGroups

            If Me.StyleGuide.GroupVisible(i) Then
                For j As Integer = 1 To Me.NetworkManager.nTrophicLevels
                    If Me.NetworkManager.RelativeFlow(i, j) = 0 Then
                    Else
                        If i <= core.nLivingGroups Then
                            asBiomassGroupsShown(j) += Me.NetworkManager.RelativeFlow(i, j) * Me.NetworkManager.BiomassByGroup(i)
                        Else
                            asMassDetritusShown(j) += Me.NetworkManager.RelativeFlow(i, j) * Me.NetworkManager.BiomassByGroup(i)
                        End If
                    End If
                Next
            End If
        Next

        ReDim astrRowContent(Me.Grid.Columns.Count)
        astrRowContent(0) = My.Resources.COL_HDR_TRP_LVL
        astrRowContent(1) = My.Resources.COL_HDR_LIVING_TKM2
        astrRowContent(2) = My.Resources.COL_HDR_DETRITUS_TKM2
        astrRowContent(3) = My.Resources.COL_HDR_TOTAL_TKM2
        astrRowContent(4) = My.Resources.COL_HDR_NONHIDDEN
        Me.Grid.Rows(0).SetValues(astrRowContent)
        Me.Grid.Rows(0).Visible = True

        For i As Integer = Me.NetworkManager.nTrophicLevels To 1 Step -1
            astrRowContent(0) = cStringUtils.ToRoman(i)
            astrRowContent(1) = Me.StyleGuide.FormatNumber(asBiomassGroupsShown(i))
            If i = 1 Then
                astrRowContent(2) = Me.StyleGuide.FormatNumber(asMassDetritusShown(i))
            Else
                astrRowContent(2) = ""
            End If
            astrRowContent(3) = Me.StyleGuide.FormatNumber(asBiomassGroupsShown(i) + asMassDetritusShown(i))
            astrRowContent(4) = Me.StyleGuide.FormatNumber(Me.NetworkManager.BiomassByTrophicLevel(i) + Me.NetworkManager.DetritusByTrophicLevel(i))
            Me.Grid.Rows(Me.NetworkManager.nTrophicLevels - i + 1).SetValues(astrRowContent)
            Me.Grid.Rows(Me.NetworkManager.nTrophicLevels - i + 1).Visible = True
        Next

        Me.Grid.ClearSelection()
    End Sub

    Private Sub SetUpGridColumn()

        ' JS: add columns Living, detritus

        Me.Grid.ReadOnly = True
        'DataGrid.RowCount = 1
        Me.Grid.ColumnCount = 5

        SetGridColumnPropertyDefault(Me.Grid)

        Me.Grid.Columns(0).Frozen = True
        Me.Grid.Columns(0).DefaultCellStyle.BackColor = Drawing.SystemColors.Control

    End Sub

End Class
