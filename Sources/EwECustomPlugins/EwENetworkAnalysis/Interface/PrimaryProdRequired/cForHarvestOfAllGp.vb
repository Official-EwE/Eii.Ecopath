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
Imports ScientificInterfaceShared.Controls




Public Class cForHarvestOfAllGp
    Inherits cContentManager

    Public Sub New()
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
        Return "Primary production required for harvest of all groups"
    End Function

    Public Overrides Sub DisplayData()

        Dim strRowContent() As String
        Dim lngSumPath As Long

        ' Load pre-requesites
        Me.NetworkManager.RunRequiredPrimaryProd()

        ' Init
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
        strRowContent(7) = My.Resources.COL_HDR_CATCH
        strRowContent(8) = My.Resources.COL_HDR_PPR_CATCH
        strRowContent(9) = My.Resources.COL_HDR_PPR_TOTAL_PP
        strRowContent(10) = My.Resources.COL_HDR_PPR_U_CATCH
        Me.Grid.Rows(0).SetValues(strRowContent)
        Me.Grid.Rows(0).Visible = True

        For i As Integer = 1 To Me.NetworkManager.Core.nLivingGroups
            strRowContent(0) = CStr(i)
            strRowContent(1) = Me.NetworkManager.GroupName(i)
            strRowContent(2) = CStr(Me.NetworkManager.NumOfPaths(i))
            If Me.NetworkManager.PPRCatchHarvest(i) > 0.0 Then lngSumPath = lngSumPath + Me.NetworkManager.NumOfPaths(i)
            strRowContent(3) = Me.StyleGuide.FormatNumber(Me.NetworkManager.TrophicLevel(i))
            strRowContent(4) = Me.StyleGuide.FormatNumber(Me.NetworkManager.PPRRequiredHarvest(i))
            strRowContent(5) = Me.StyleGuide.FormatNumber(Me.NetworkManager.PPRRequiredDetHarvest(i))
            strRowContent(6) = Me.StyleGuide.FormatNumber(Me.NetworkManager.PPRRequiredSumHarvest(i))
            strRowContent(7) = Me.StyleGuide.FormatNumber(Me.NetworkManager.PPRCatchHarvest(i))
            If Me.NetworkManager.PPRCatchHarvest(i) > 0.0 Then
                strRowContent(8) = Me.StyleGuide.FormatNumber(Me.NetworkManager.PPROverCatchHarvest(i))
            Else
                strRowContent(8) = ""
            End If
            strRowContent(9) = Me.StyleGuide.FormatNumber(Me.NetworkManager.PPRTotPPHarvest(i))
            If Me.NetworkManager.PPRCatchHarvest(i) > 0.0 And Me.NetworkManager.TotalPrimaryProduction > 0.0 Then
                strRowContent(10) = Me.StyleGuide.FormatNumber(Me.NetworkManager.PPRUHarvest(i))
            Else
                strRowContent(10) = ""
            End If
            Me.Grid.Rows(i).SetValues(strRowContent)
            Me.Grid.Rows(i).Visible = True
        Next

        'Display total
        For i As Integer = 0 To Me.Grid.Columns.Count - 1
            strRowContent(i) = ""
        Next
        strRowContent(1) = My.Resources.ROW_HDR_TOTAL
        strRowContent(2) = CStr(lngSumPath)
        strRowContent(3) = Me.StyleGuide.FormatNumber(Me.NetworkManager.TotalTL)
        strRowContent(4) = Me.StyleGuide.FormatNumber(Me.NetworkManager.TotalPPRPP)
        strRowContent(5) = Me.StyleGuide.FormatNumber(Me.NetworkManager.TotalPPRDet)
        strRowContent(6) = Me.StyleGuide.FormatNumber(Me.NetworkManager.TotalPPRPP + Me.NetworkManager.TotalPPRDet)
        strRowContent(7) = Me.StyleGuide.FormatNumber(Me.NetworkManager.TotalCatch)
        If Me.NetworkManager.TotalCatch > 0.0 Then
            strRowContent(8) = Me.StyleGuide.FormatNumber((Me.NetworkManager.TotalPPRPP + Me.NetworkManager.TotalPPRDet) / _
                Me.NetworkManager.TotalCatch)
        Else
            strRowContent(8) = ""
        End If
        strRowContent(9) = Me.StyleGuide.FormatNumber(100 * (Me.NetworkManager.TotalPPRPP + Me.NetworkManager.TotalPPRDet) / _
            (Me.NetworkManager.TotalPrimaryProduction + Me.NetworkManager.DetThroughtput(1)))
        If Me.NetworkManager.TotalCatch > 0.0 Then
            strRowContent(10) = Me.StyleGuide.FormatNumber((Me.NetworkManager.TotalPPRPP + Me.NetworkManager.TotalPPRDet) / _
                (Me.NetworkManager.TotalPrimaryProduction + Me.NetworkManager.DetThroughtput(1)) / _
                Me.NetworkManager.TotalCatch)
        Else
            strRowContent(10) = ""
        End If
        Me.Grid.Rows(Me.Grid.RowCount - 1).SetValues(strRowContent)
        Me.Grid.Rows(Me.Grid.RowCount - 1).Visible = True

        'Hide some rows
        For i As Integer = 1 To Me.NetworkManager.Core.nLivingGroups
            If Me.NetworkManager.PPRCatchHarvest(i) <= 0.0 Or _
                Me.NetworkManager.PPRCatchHarvest(i) <= 0.0 And Me.NetworkManager.TotalPrimaryProduction <= 0.0 Then
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
