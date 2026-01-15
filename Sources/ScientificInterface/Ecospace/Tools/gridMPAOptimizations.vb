' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 3 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see https://www.gnu.org/licenses/gpl-3.0.html>. 
'
' Copyright 1991- 
'    UBC Institute for the Oceans and Fisheries, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'




Imports SharedResources = ScientificInterfaceShared.My.Resources



''' ===========================================================================
''' <summary>
''' Grid class for showing MPA optimizations progress information.
''' </summary>
''' ===========================================================================

Public Class gridMPAOptimizations
    : Inherits cEwEGrid

    Public Enum eColumnTypes As Byte
        Variable = 0
        Value
    End Enum

    Protected Overrides Sub InitStyle()
        MyBase.InitStyle()

        If Me.UIContext Is Nothing Then Return

        Dim c As cEwECell = Nothing

        Me.FixedColumnWidths = False

        Me.Redim(9, [Enum].GetValues(GetType(eColumnTypes)).Length)

        Me(0, eColumnTypes.Variable) = New cEwEColumnHeaderCell(SharedResources.HEADER_INDICATOR)
        Me(0, eColumnTypes.Value) = New cEwEColumnHeaderCell(SharedResources.HEADER_VALUE)

        Me(1, eColumnTypes.Variable) = New cEwERowHeaderCell(SharedResources.HEADER_NET_ECONOMIC_VALUE)
        c = New cEwECell(0.0!, GetType(Single))
        c.Style = cStyleGuide.eStyleFlags.OK Or cStyleGuide.eStyleFlags.NotEditable
        Me(1, eColumnTypes.Value) = c

        Me(2, eColumnTypes.Variable) = New cEwERowHeaderCell(SharedResources.HEADER_SOCIAL_VALUE_EMPLOYMENT)
        c = New cEwECell(0.0!, GetType(Single))
        c.Style = cStyleGuide.eStyleFlags.OK Or cStyleGuide.eStyleFlags.NotEditable
        Me(2, eColumnTypes.Value) = c

        Me(3, eColumnTypes.Variable) = New cEwERowHeaderCell(SharedResources.HEADER_MANDATED_REBUILDING)
        c = New cEwECell(0.0!, GetType(Single))
        c.Style = cStyleGuide.eStyleFlags.OK Or cStyleGuide.eStyleFlags.NotEditable
        Me(3, eColumnTypes.Value) = c

        Me(4, eColumnTypes.Variable) = New cEwERowHeaderCell(SharedResources.HEADER_ECOSYSTEM_STRUCTURE)
        c = New cEwECell(0.0!, GetType(Single))
        c.Style = cStyleGuide.eStyleFlags.OK Or cStyleGuide.eStyleFlags.NotEditable
        Me(4, eColumnTypes.Value) = c

        Me(5, eColumnTypes.Variable) = New cEwERowHeaderCell(SharedResources.HEADER_BIODIVERSITY)
        c = New cEwECell(0.0!, GetType(Single))
        c.Style = cStyleGuide.eStyleFlags.OK Or cStyleGuide.eStyleFlags.NotEditable
        Me(5, eColumnTypes.Value) = c

        Me(6, eColumnTypes.Variable) = New cEwERowHeaderCell(SharedResources.HEADER_BOUNDARYWEIGHT)
        c = New cEwECell(0.0!, GetType(Single))
        c.Style = cStyleGuide.eStyleFlags.OK Or cStyleGuide.eStyleFlags.NotEditable
        Me(6, eColumnTypes.Value) = c

        Me(7, eColumnTypes.Variable) = New cEwERowHeaderCell(SharedResources.HEADER_TOTAL)
        c = New cEwECell(0.0!, GetType(Single))
        c.Style = cStyleGuide.eStyleFlags.OK Or cStyleGuide.eStyleFlags.NotEditable
        Me(7, eColumnTypes.Value) = c

        Me(8, eColumnTypes.Variable) = New cEwERowHeaderCell(SharedResources.HEADER_AREA_CLOSED)
        c = New cEwECell(0.0!, GetType(Single))
        c.Style = cStyleGuide.eStyleFlags.OK Or cStyleGuide.eStyleFlags.NotEditable
        Me(8, eColumnTypes.Value) = c

    End Sub

    Public Overrides ReadOnly Property SuppressQuickEdits As Boolean
        Get
            Return True
        End Get
    End Property

    Protected Overrides Sub FillData()

    End Sub

    Public Sub LogResult(sEconomicValue As Single, sSocialValue As Single, _
        sMandatedValue As Single, sEcologicalValue As Single, _
        sBiomassDiversityValue As Single, sBoundaryWeightValue As Single, _
        sTotalWeighted As Single, sPercClosed As Single)

        Me(1, eColumnTypes.Value).Value = sEconomicValue
        Me(2, eColumnTypes.Value).Value = sSocialValue
        Me(3, eColumnTypes.Value).Value = sMandatedValue
        Me(4, eColumnTypes.Value).Value = sEcologicalValue
        Me(5, eColumnTypes.Value).Value = sBiomassDiversityValue
        Me(6, eColumnTypes.Value).Value = sBoundaryWeightValue
        Me(7, eColumnTypes.Value).Value = sTotalWeighted
        Me(8, eColumnTypes.Value).Value = sPercClosed

        Me.InvalidateCells()

    End Sub

End Class
