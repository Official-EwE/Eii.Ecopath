' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 2 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see <http://www.gnu.org/licenses/gpl-2.0.html>. 
'
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'




Option Explicit On

Imports System.Windows.Forms
Imports EwECore
Imports ZedGraph
Imports EwEUtils.Utilities
Imports ScientificInterfaceShared.Controls




Public Class cCatchByTrophicLevel
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
        Return bSucces
    End Function

    Public Overrides Function PageTitle() As String
        ' ToDo: globalize this
        Return "Catch by tropic level"
    End Function

    Public Overrides Sub DisplayData()

        Dim strRowContent() As String
        Dim core As cCore = Me.UIContext.Core
        Dim bShowItem As Boolean = True
        Dim CatchGroupsShown() As Single

        Me.SetUpGridColumn()

        'Set up grid rows
        Me.Grid.RowHeadersVisible = False
        Me.Grid.RowCount = Me.NetworkManager.nTrophicLevels + 1
        Me.Grid.Rows(0).DefaultCellStyle.WrapMode = DataGridViewTriState.True
        Me.Grid.Rows(0).DefaultCellStyle.BackColor = Drawing.SystemColors.Control
        Me.Grid.Rows(0).Frozen = True
        Me.Grid.Rows(0).Height = FIRST_ROW_HEIGHT

        'Calculate non-hidden data
        ReDim CatchGroupsShown(Me.NetworkManager.nGroups)
        For i As Integer = 1 To Me.NetworkManager.nGroups
            ' bShowItem = sg.GroupVisible(i)
            If bShowItem Then
                For j As Integer = 1 To Me.NetworkManager.nTrophicLevels
                    If Me.NetworkManager.RelativeFlow(i, j) = 0 Then
                    Else
                        If i <= core.nLivingGroups Then
                            CatchGroupsShown(j) = CatchGroupsShown(j) + Me.NetworkManager.RelativeFlow(i, j) * Me.NetworkManager.CatchByGroup(i)
                        End If
                    End If
                Next
            End If
        Next

        ReDim strRowContent(Me.Grid.Columns.Count)
        strRowContent(0) = My.Resources.COL_HDR_TRP_LVL
        strRowContent(1) = My.Resources.COL_HDR_TOTAL_TKM2YR
        strRowContent(2) = My.Resources.COL_HDR_NONHIDDEN
        Me.Grid.Rows(0).SetValues(strRowContent)
        Me.Grid.Rows(0).Visible = True

        For i As Integer = Me.NetworkManager.nTrophicLevels To 1 Step -1
            strRowContent(0) = cStringUtils.ToRoman(i)
            strRowContent(1) = Me.StyleGuide.FormatNumber(Me.NetworkManager.CatchByTrophicLevel(i))
            strRowContent(2) = Me.StyleGuide.FormatNumber(CatchGroupsShown(i))
            Me.Grid.Rows(Me.NetworkManager.nTrophicLevels - i + 1).SetValues(strRowContent)
            Me.Grid.Rows(Me.NetworkManager.nTrophicLevels - i + 1).Visible = True
        Next

        Me.Grid.ClearSelection()
    End Sub

    Private Sub SetUpGridColumn()

        'DataGrid.RowCount = 1
        Me.Grid.ColumnCount = 3

        SetGridColumnPropertyDefault(Me.Grid)

        Me.Grid.Columns(0).Frozen = True
        Me.Grid.Columns(0).DefaultCellStyle.BackColor = Drawing.SystemColors.Control

    End Sub

End Class
