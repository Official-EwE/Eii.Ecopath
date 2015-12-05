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
' Copyright 1991- UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'

#Region " Imports "

Option Strict On

Imports ScientificInterfaceShared.Controls
Imports ScientificInterfaceShared.Controls.EwEGrid
Imports ScientificInterfaceShared.Style
Imports EwEUtils.SystemUtilities

#End Region ' Imports

Namespace Controls.Shapes

    <CLSCompliant(False)> _
    Public Class gridImportShapes
        Inherits EwEGrid

        Private m_defs As cShapeImportData.cFunctionDefinition()

        Public Property Functions As cShapeImportData.cFunctionDefinition()
            Get
                Return Me.m_defs
            End Get
            Set(defs As cShapeImportData.cFunctionDefinition())
                Me.m_defs = defs
                Me.FillData()
            End Set
        End Property

        Protected Overrides Sub InitStyle()

            ' ToDo: Globalize this

            MyBase.InitStyle()

            Me.Redim(1, 7)

            Me(0, 0) = New EwEColumnHeaderCell(EwEUtils.Core.eVarNameFlags.Name)
            Me(0, 1) = New EwEColumnHeaderCell("Function type")
            Me(0, 2) = New EwEColumnHeaderCell("Parameter 1")
            Me(0, 3) = New EwEColumnHeaderCell("Parameter 2")
            Me(0, 4) = New EwEColumnHeaderCell("Parameter 3")
            Me(0, 5) = New EwEColumnHeaderCell("Parameter 4")
            Me(0, 6) = New EwEColumnHeaderCell("Parameter 5")

            Me.FixedColumnWidths = False
            Me.FixedColumns = 1
            Me.AllowBlockSelect = False

            Me.AutoStretchColumnsToFitWidth = True
            Me.SizeColumnsEqually()

        End Sub

        Protected Overrides Sub FillData()

            ' Remove existing rows
            Me.RowsCount = 1

            If (Me.m_defs Is Nothing) Then Return

            Dim fmt As New cShapeFunctionFormatter()

            For Each fn As cShapeImportData.cFunctionDefinition In Me.m_defs
                Dim iRow As Integer = Me.AddRow()
                Me(iRow, 0) = New EwERowHeaderCell(fn.Name)
                Me(iRow, 1) = New EwECell(fmt.GetDescriptor(fn.ShapeFunction), cStyleGuide.eStyleFlags.NotEditable)
                For i As Integer = 1 To 5
                    Dim style As cStyleGuide.eStyleFlags = cSystemUtils.IIF(fn.Parms(i) >= 0, cStyleGuide.eStyleFlags.OK, cStyleGuide.eStyleFlags.Null) Or cStyleGuide.eStyleFlags.NotEditable
                    Me(iRow, 1 + i) = New EwECell(fn.Parms(i), style)
                Next
            Next

        End Sub

        Protected Overrides Sub FinishStyle()
            MyBase.FinishStyle()
            Me.Selection.SelectionMode = SourceGrid2.GridSelectionMode.Row
        End Sub

    End Class

End Namespace
