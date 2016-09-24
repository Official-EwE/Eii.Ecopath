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

Public Class gridConfig
    Inherits EwEGrid

    Public Sub New()
        MyBase.New()
    End Sub

    Private m_config As cConfiguration = cFishMIPcore.GetInstance().Configuration

    Protected Overrides Sub InitStyle()

        MyBase.InitStyle()

        Me.Redim(1, [Enum].GetValues(GetType(cConfiguration.eResultTypes)).Length + 3)

        Me(0, 0) = New EwEColumnHeaderCell("")
        Me(0, 1) = New EwEColumnHeaderCell("Group")
        Me(0, 2 + cConfiguration.eResultTypes.tsb) = New EwEColumnHeaderCell("Total system B")
        Me(0, 2 + cConfiguration.eResultTypes.tcb) = New EwEColumnHeaderCell("Total consumer B")
        Me(0, 2 + cConfiguration.eResultTypes.b10cm) = New EwEColumnHeaderCell("B > 10cm")
        Me(0, 2 + cConfiguration.eResultTypes.b30cm) = New EwEColumnHeaderCell("B > 30cm")
        Me(0, 2 + cConfiguration.eResultTypes.tc) = New EwEColumnHeaderCell("Total catch")
        Me(0, 2 + cConfiguration.eResultTypes.tc10cm) = New EwEColumnHeaderCell("Catch  B > 10cm")
        Me(0, 2 + cConfiguration.eResultTypes.tc30cm) = New EwEColumnHeaderCell("Catch  B > 30cm")
        Me(0, 2 + cConfiguration.eResultTypes.bcom) = New EwEColumnHeaderCell("B commercial")

        Me.FixedColumns = 2

    End Sub

    Protected Overrides Sub FillData()

        Me.RowsCount = 1
        For i As Integer = 1 To Me.UIContext.Core.nGroups

            Dim iRow As Integer = Me.AddRow()
            Me(iRow, 0) = New EwERowHeaderCell(CStr(i))
            Me(iRow, 1) = New EwERowHeaderCell(Me.Core.EcoPathGroupInputs(i).Name)

            For Each j As cConfiguration.eResultTypes In [Enum].GetValues(GetType(cConfiguration.eResultTypes))
                Me(iRow, 2 + j) = New EwECheckboxCell(Me.m_config(i, j))
                Me(iRow, 2 + j).Behaviors.Add(Me.EwEEditHandler)
            Next
        Next

    End Sub

    Protected Overrides Sub FinishStyle()
        MyBase.FinishStyle()
        Me.FixedColumnWidths = False
    End Sub

    Protected Overrides Function OnCellValueChanged(p As Position, cell As ICellVirtual) As Boolean

        Me.m_config(p.Row, CType(p.Column - 2, cConfiguration.eResultTypes)) = CBool(cell.GetValue(p))
        BeginInvoke(New MethodInvoker(AddressOf Me.m_config.Save))

        Return True

    End Function

End Class
