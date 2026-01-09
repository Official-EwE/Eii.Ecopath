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
Imports ZedGraph
Imports ScientificInterfaceShared.Controls




Public Class cImpactData
    Inherits cContentManager

    Public Sub New()
        '
    End Sub

    Public Overrides Function PageTitle() As String
        ' ToDo: globalize this
        Return "Mixed tropic level impacts"
    End Function

    Public Overrides Function Attach(manager As cNetworkManager,
                                    datagrid As DataGridView,
                                    graph As ZedGraphControl,
                                    plot As ucPlot,
                                    toolstrip As ToolStrip,
                                    info As Control,
                                    uic As cUIContext) As Boolean
        Dim bSucces As Boolean = MyBase.Attach(manager, datagrid, graph, plot, toolstrip, info, uic)
        Me.Toolstrip.Visible = bSucces
        Me.Grid.Visible = bSucces
        Me.ToolstripShowOptionCSV(False)

        Return bSucces
    End Function

    Public Overrides Sub DisplayData()

        Dim astrRowContent() As String

        Me.SetUpGridColumn(Me.NetworkManager.nGroups, Me.NetworkManager.nFleets)

        'Set up grid rows
        Me.Grid.RowHeadersVisible = False
        Me.Grid.RowCount = Me.NetworkManager.nGroups + Me.NetworkManager.nFleets + 1
        Me.Grid.Rows(0).DefaultCellStyle.WrapMode = DataGridViewTriState.True
        Me.Grid.Rows(0).DefaultCellStyle.BackColor = Drawing.SystemColors.Control
        Me.Grid.Rows(0).Frozen = True
        Me.Grid.Rows(0).Height = FIRST_ROW_HEIGHT

        ReDim astrRowContent(Me.Grid.Columns.Count)
        astrRowContent(0) = ""
        astrRowContent(1) = My.Resources.COL_HDR_IMPACTING_IMPACTED
        For intIndex As Integer = 1 To Me.NetworkManager.nGroups
            astrRowContent(intIndex + 1) = Me.NetworkManager.GroupName(intIndex)
        Next
        For intIndex As Integer = 1 To Me.NetworkManager.nFleets
            astrRowContent(Me.NetworkManager.nGroups + intIndex + 1) = Me.NetworkManager.FleetName(intIndex)
        Next
        Me.Grid.Rows(0).SetValues(astrRowContent)
        Me.Grid.Rows(0).Visible = True

        For i As Integer = 1 To Me.NetworkManager.nGroups + Me.NetworkManager.nFleets
            astrRowContent(0) = CStr(i)
            If i <= Me.NetworkManager.nGroups Then
                astrRowContent(1) = Me.NetworkManager.GroupName(i)
            Else
                astrRowContent(1) = Me.NetworkManager.FleetName(i - Me.NetworkManager.nGroups)
            End If
            For j As Integer = 1 To Me.NetworkManager.nGroups + Me.NetworkManager.nFleets
                astrRowContent(j + 1) = Me.StyleGuide.FormatNumber(Me.NetworkManager.MixedTrophicImpacts(i, j))
            Next
            Me.Grid.Rows(i).SetValues(astrRowContent)
            Me.Grid.Rows(i).Visible = True
        Next
        Me.Grid.ClearSelection()
    End Sub

    Private Sub SetUpGridColumn(iNumGroups As Integer, iNumFleets As Integer)

        Me.Grid.ReadOnly = True
        'DataGrid.RowCount = 1
        Me.Grid.ColumnCount = iNumGroups + iNumFleets + 2

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
