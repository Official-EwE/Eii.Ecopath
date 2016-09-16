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
'    UBC Fisheries Centre, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

Option Strict On
Imports System.Windows.Forms
Imports ScientificInterfaceShared.Controls.EwEGrid
Imports SourceGrid2
Imports SourceGrid2.Cells

Public Class gridUI
    Inherits EwEGrid

    Public Sub New()
        MyBase.New()
    End Sub

    Public Property Plugin As cFishMIPResultWriterPlugin

    Protected Overrides Sub InitStyle()

        MyBase.InitStyle()

        Me.Redim(1, [Enum].GetValues(GetType(cFishMIPResultWriterPlugin.eResultTypes)).Length + 3)

        Me(0, 0) = New EwEColumnHeaderCell("")
        Me(0, 1) = New EwEColumnHeaderCell("Group")
        Me(0, 2 + cFishMIPResultWriterPlugin.eResultTypes.b10cm) = New EwEColumnHeaderCell("B > 10cm")
        Me(0, 2 + cFishMIPResultWriterPlugin.eResultTypes.b30cm) = New EwEColumnHeaderCell("B > 30cm")
        Me(0, 2 + cFishMIPResultWriterPlugin.eResultTypes.tc) = New EwEColumnHeaderCell("Total catch")
        Me(0, 2 + cFishMIPResultWriterPlugin.eResultTypes.tcb) = New EwEColumnHeaderCell("Total consumer B")
        Me(0, 2 + cFishMIPResultWriterPlugin.eResultTypes.tsb) = New EwEColumnHeaderCell("Total system B")

        Me.FixedColumns = 2

    End Sub

    Protected Overrides Sub FillData()

        Me.RowsCount = 1
        For i As Integer = 1 To Me.UIContext.Core.nGroups

            Dim iRow As Integer = Me.AddRow()
            Me(iRow, 0) = New EwERowHeaderCell(CStr(i))
            Me(iRow, 1) = New EwERowHeaderCell(Me.Core.EcoPathGroupInputs(i).Name)

            For j As Integer = 0 To [Enum].GetValues(GetType(cFishMIPResultWriterPlugin.eResultTypes)).Length - 1
                Me(iRow, 2 + j) = New EwECheckboxCell(Me.Plugin.Configuration(i, j))
                Me(iRow, 2 + j).Behaviors.Add(Me.EwEEditHandler)
            Next
        Next

    End Sub

    Protected Overrides Sub FinishStyle()
        MyBase.FinishStyle()
        Me.FixedColumnWidths = False
    End Sub

    Protected Overrides Function OnCellValueChanged(p As Position, cell As ICellVirtual) As Boolean

        Me.Plugin.Configuration(p.Row, p.Column - 2) = CBool(cell.GetValue(p))
        BeginInvoke(New MethodInvoker(AddressOf Me.Plugin.ConfigChanged))

        Return True

    End Function

End Class
