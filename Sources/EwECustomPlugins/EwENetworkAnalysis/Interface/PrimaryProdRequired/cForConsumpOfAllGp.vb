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

#Region " Imports "

Option Strict On
Option Explicit On

Imports System.Windows.Forms
Imports EwECore
Imports ZedGraph
Imports ScientificInterfaceShared.Controls

#End Region ' Imports

<CLSCompliant(False)> _
Public Class cForConsumpOfAllGp
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
        Return "Primary production required for comsumption of all groups"
    End Function

    Public Overrides Sub DisplayData()

        Dim strRowContent() As String
        Dim sngTotalPPRCons As Single

        Me.SetUpGridColumn()

        'Set up grid rows
        Me.Grid.RowHeadersVisible = False
        Me.Grid.RowCount = Me.NetworkManager.Core.nLivingGroups + 2
        Me.Grid.Rows(0).DefaultCellStyle.WrapMode = DataGridViewTriState.True
        Me.Grid.Rows(0).DefaultCellStyle.BackColor = Drawing.SystemColors.Control
        Me.Grid.Rows(0).Frozen = True
        Me.Grid.Rows(0).Height = FIRST_ROW_HEIGHT

        ReDim strRowContent(Me.Grid.Columns.Count)
        strRowContent(0) = ""
        strRowContent(1) = My.Resources.COL_HDR_GRP_NAME
        strRowContent(2) = My.Resources.COL_HDR_NUM_PATH
        strRowContent(3) = My.Resources.COL_HDR_TL
        strRowContent(4) = My.Resources.COL_HDR_PPR_PP
        strRowContent(5) = My.Resources.COL_HDR_PPR_DET
        strRowContent(6) = My.Resources.COL_HDR_PPR
        strRowContent(7) = My.Resources.COL_HDR_CONSUM
        strRowContent(8) = My.Resources.COL_HDR_PPR_COMSUM
        strRowContent(9) = My.Resources.COL_HDR_PPR_TOTAL_PP
        strRowContent(10) = My.Resources.COL_HDR_PPR_U_BIOMASS
        Me.Grid.Rows(0).SetValues(strRowContent)
        Me.Grid.Rows(0).Visible = True

        For i As Integer = 1 To Me.NetworkManager.Core.nLivingGroups
            strRowContent(0) = CStr(i)
            strRowContent(1) = Me.NetworkManager.GroupName(i)
            strRowContent(2) = CStr(Me.NetworkManager.NumOfPaths(i))
            strRowContent(3) = Me.StyleGuide.FormatNumber(Me.NetworkManager.TrophicLevel(i))
            strRowContent(4) = Me.StyleGuide.FormatNumber(Me.NetworkManager.PPRRequired(i))
            strRowContent(5) = Me.StyleGuide.FormatNumber(Me.NetworkManager.PPRRequiredDet(i))
            strRowContent(6) = Me.StyleGuide.FormatNumber(Me.NetworkManager.PPRRequiredSum(i))
            strRowContent(7) = Me.StyleGuide.FormatNumber(Me.NetworkManager.PPRCons(i))
            sngTotalPPRCons = sngTotalPPRCons + Me.NetworkManager.PPRCons(i)
            If Me.NetworkManager.PPRCons(i) > 0.0 Then
                strRowContent(8) = Me.StyleGuide.FormatNumber(Me.NetworkManager.PPROverCons(i))
            Else
                strRowContent(8) = ""
            End If
            strRowContent(9) = Me.StyleGuide.FormatNumber(Me.NetworkManager.PPRTotPP(i))
            If Me.NetworkManager.TotalPrimaryProduction > 0.0 Then
                strRowContent(10) = Me.StyleGuide.FormatNumber(Me.NetworkManager.PPRU(i))
            Else
                strRowContent(10) = ""
            End If
            Me.Grid.Rows(i).SetValues(strRowContent)
            Me.Grid.Rows(i).Visible = True

            'DataGrid.Rows(i - 1).HeaderCell.Value = CStr(i)
            'DataGrid.Rows(i - 1).HeaderCell.Style.BackColor = Drawing.Color.Beige
        Next

        'Display total
        For i As Integer = 0 To Me.Grid.Columns.Count - 1
            strRowContent(i) = ""
        Next
        strRowContent(1) = My.Resources.ROW_HDR_TOTAL
        strRowContent(2) = CStr((Me.NetworkManager.NumLivPath + Me.NetworkManager.NumDetPath))
        strRowContent(7) = Me.StyleGuide.FormatNumber(sngTotalPPRCons)
        Me.Grid.Rows(Me.Grid.Rows.Count - 1).SetValues(strRowContent)
        Me.Grid.Rows(Me.Grid.Rows.Count - 1).Visible = True

        'Hide some rows
        For i As Integer = 1 To Me.NetworkManager.Core.nLivingGroups
            If Me.NetworkManager.PPRCons(i) <= 0.0 Or _
                Me.NetworkManager.TotalPrimaryProduction <= 0.0 Then
                Me.Grid.Rows(i).Visible = False
            End If
        Next
        Me.Grid.ClearSelection()
        Cursor.Current = Cursors.Default
    End Sub

    Private Sub SetUpGridColumn()

        Me.Grid.ReadOnly = True
        'DataGrid.RowCount = 1
        Me.Grid.ColumnCount = 11

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
