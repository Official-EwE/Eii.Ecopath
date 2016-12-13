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

#Region " Imports "

Option Strict On
Imports EwECore
Imports EwEUtils.Utilities
Imports ScientificInterfaceShared.Controls
Imports ScientificInterfaceShared.Controls.EwEGrid
Imports ScientificInterfaceShared.Properties
Imports ScientificInterfaceShared.Style.cStyleGuide
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region ' Imports

Public Class gridTaxonomy
    Inherits EwEGrid

    Private Enum eColumnTypes As Integer
        Name = 0
        MergB
        Agg1B
        Agg2B
        MergC
        Agg1C
        Agg2C
    End Enum

    Private m_data As cEcopathMergeGroupsDatastructures = Nothing

    Public Sub New()
    End Sub

    Public Sub Init(uic As cUIContext, data As cEcopathMergeGroupsDatastructures)
        ' Set data before UIC; UIC setting will trigger grid refresh etc.
        Me.m_data = data
        Me.UIContext = uic
    End Sub

    Protected Overrides Sub InitStyle()
        MyBase.InitStyle()

        If (Me.UIContext Is Nothing) Then Return
        If (Me.m_data Is Nothing) Then Return

        Me.FixedColumnWidths = False
        Me.Redim(1, [Enum].GetValues(GetType(eColumnTypes)).Length)

        Me(0, eColumnTypes.Name) = New EwEColumnHeaderCell(SharedResources.HEADER_SPECIES)
        Me(0, eColumnTypes.MergB) = New EwEColumnHeaderCell(My.Resources.HEADER_PROP_B)
        Me(0, eColumnTypes.Agg1B) = New EwEColumnHeaderCell()
        Me(0, eColumnTypes.Agg2B) = New EwEColumnHeaderCell()
        Me(0, eColumnTypes.MergC) = New EwEColumnHeaderCell(My.Resources.HEADER_PROP_C)
        Me(0, eColumnTypes.Agg1C) = New EwEColumnHeaderCell()
        Me(0, eColumnTypes.Agg2C) = New EwEColumnHeaderCell()

        Me.UpdateHeader()

        Me.AllowBlockSelect = False

    End Sub

    Protected Overrides Sub FillData()

        For Each i As Integer In Me.m_data.TaxonPropBiomass.Keys
            Me.AddRow(Me.Core.Taxon(i))
        Next

    End Sub

    Protected Overrides Sub FinishStyle()
        MyBase.FinishStyle()
        Me.StretchColumnsToFitWidth()
    End Sub

    Private Overloads Sub AddRow(taxon As cTaxon)

        Dim propScName As cScientificNameProperty = New cScientificNameProperty(Me.PropertyManager, taxon)
        Me.RegisterLocalProperty(propScName)

        Dim iRow As Integer = Me.AddRow()
        Dim cell As EwECellBase = Nothing

        cell = New PropertyRowHeaderChildCell(propScName)
        Me(iRow, eColumnTypes.Name) = cell

        For i As Integer = 1 To Me.ColumnsCount - 1
            cell = New EwECell("", eStyleFlags.NotEditable)
            cell.SuppressZero() = True
            Me(iRow, i) = cell
        Next
        Me.UpdateRow(iRow, taxon)

    End Sub

    Private Sub UpdateHeader()

        Me.UpdateHeaderCell(eColumnTypes.Agg1B, Me.m_data.IndexTarget, My.Resources.HEADER_PROP_B)
        Me.UpdateHeaderCell(eColumnTypes.Agg1C, Me.m_data.IndexTarget, My.Resources.HEADER_PROP_C)
        Me.UpdateHeaderCell(eColumnTypes.Agg2B, Me.m_data.IndexMerge, My.Resources.HEADER_PROP_B)
        Me.UpdateHeaderCell(eColumnTypes.Agg2C, Me.m_data.IndexMerge, My.Resources.HEADER_PROP_C)

    End Sub

    Private Sub UpdateRow(iRow As Integer, taxon As cTaxon)

        If (taxon.iGroup = Me.m_data.IndexTarget) Then
            Me.UpdateCell(iRow, eColumnTypes.Agg1B, taxon.Proportion, eStyleFlags.NotEditable)
            Me.UpdateCell(iRow, eColumnTypes.Agg1C, taxon.ProportionCatch, eStyleFlags.NotEditable)
        Else
            Me.UpdateCell(iRow, eColumnTypes.Agg1B, 0, eStyleFlags.NotEditable Or eStyleFlags.Null)
            Me.UpdateCell(iRow, eColumnTypes.Agg1C, 0, eStyleFlags.NotEditable Or eStyleFlags.Null)
        End If

        If (taxon.iGroup = Me.m_data.IndexMerge) Then
            Me.UpdateCell(iRow, eColumnTypes.Agg2B, taxon.Proportion, eStyleFlags.NotEditable)
            Me.UpdateCell(iRow, eColumnTypes.Agg2C, taxon.ProportionCatch, eStyleFlags.NotEditable)
        Else
            Me.UpdateCell(iRow, eColumnTypes.Agg2B, 0, eStyleFlags.NotEditable Or eStyleFlags.Null)
            Me.UpdateCell(iRow, eColumnTypes.Agg2C, 0, eStyleFlags.NotEditable Or eStyleFlags.Null)
        End If

        Me.UpdateCell(iRow, eColumnTypes.MergB, Me.m_data.TaxonPropBiomass(taxon.Index), eStyleFlags.NotEditable)
        Me.UpdateCell(iRow, eColumnTypes.MergC, Me.m_data.TaxonPropCatch(taxon.Index), eStyleFlags.NotEditable)

    End Sub

    Private Sub UpdateHeaderCell(iCol As Integer, iIndex As Integer, strVal As String)

        Dim c As EwEColumnHeaderCell = DirectCast(Me(0, iCol), EwEColumnHeaderCell)
        If (iIndex > 0) Then
            c.Value = cStringUtils.Localize(ScientificInterfaceShared.My.Resources.GENERIC_LABEL_DETAILED, iIndex, strVal)
        Else
            c.Value = strVal
        End If

    End Sub

    Private Sub UpdateCell(iRow As Integer, iCol As Integer, val As Single, style As eStyleFlags)

        Dim c As EwECell = DirectCast(Me(iRow, iCol), EwECell)
        c.Value = val
        c.Style = style

    End Sub

End Class
