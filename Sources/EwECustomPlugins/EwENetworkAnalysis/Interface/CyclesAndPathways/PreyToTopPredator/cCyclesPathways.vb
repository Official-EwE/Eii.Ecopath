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

 

Namespace PreyToPredator

    
    Public Class cCyclesPathways
        Inherits cContentManager

        Public Sub New()
        End Sub

        Public Overrides Function PageTitle() As String
            ' ToDo: globalize this
            Return "Cycles and pathways prey to predator"
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
            Me.ToolstripShowGroupSelections(My.Resources.LBL_PATH_FROM, eGroupFilterTypes.All)

            Return bSucces
        End Function

        Public Overrides Sub DisplayData()

            Me.Grid.ColumnCount = 2

            SetGridColumnPropertyDefault(Me.Grid)

            Me.Grid.Columns(0).Frozen = True
            Me.Grid.Columns(0).DefaultCellStyle.BackColor = Drawing.SystemColors.Control

            Me.Grid.Columns(1).Width = 660
            Me.Grid.Columns(1).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft

        End Sub
        Public Overrides Sub UpdateData(iSel1 As Integer, iSel2 As Integer)

            Dim astrRowContent() As String

            Me.Grid.RowHeadersVisible = False

            ReDim astrRowContent(Me.Grid.Columns.Count)
            Me.NetworkManager.FindPathwaysFromPrey(iSel1)
            If Me.NetworkManager.PathWays.Count > 0 Then
                Me.Grid.RowCount = Me.NetworkManager.PathWays.Count + 1
                Me.Grid.Rows(0).DefaultCellStyle.WrapMode = DataGridViewTriState.True
                Me.Grid.Rows(0).DefaultCellStyle.BackColor = Drawing.SystemColors.Control
                Me.Grid.Rows(0).Frozen = True
                Me.Grid.Rows(0).Height = FIRST_ROW_HEIGHT

                astrRowContent(0) = My.Resources.COL_HDR_PATH_NUM
                astrRowContent(1) = My.Resources.COL_HDR_PATH
                Me.Grid.Rows(0).SetValues(astrRowContent)
                Me.Grid.Rows(0).Visible = True

                For intPathwayIndex As Integer = 0 To Me.NetworkManager.PathWays.Count - 1
                    astrRowContent(0) = CStr(intPathwayIndex + 1)
                    astrRowContent(1) = CStr(Me.NetworkManager.PathWays.Item(intPathwayIndex))
                    Me.Grid.Rows(intPathwayIndex + 1).SetValues(astrRowContent)
                    Me.Grid.Rows(intPathwayIndex + 1).Visible = True
                Next
            Else
                Me.Grid.RowCount = 2
                Me.Grid.Rows(0).DefaultCellStyle.WrapMode = DataGridViewTriState.True
                Me.Grid.Rows(0).DefaultCellStyle.BackColor = Drawing.SystemColors.Control
                Me.Grid.Rows(0).Frozen = True
                Me.Grid.Rows(0).Height = FIRST_ROW_HEIGHT

                astrRowContent(0) = My.Resources.COL_HDR_PATH_NUM
                astrRowContent(1) = My.Resources.COL_HDR_PATH
                Me.Grid.Rows(0).SetValues(astrRowContent)
                Me.Grid.Rows(0).Visible = True

                astrRowContent(0) = My.Resources.ROW_HDR_NO_PATH_FOUND
                astrRowContent(1) = ""
                Me.Grid.Rows(1).SetValues(astrRowContent)
                Me.Grid.Rows(1).Visible = True
            End If
            Me.Grid.ClearSelection()
        End Sub

    End Class

End Namespace
