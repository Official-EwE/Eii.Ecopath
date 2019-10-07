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
Imports EwECore
Imports ScientificInterfaceShared.Controls.EwEGrid
Imports ScientificInterfaceShared.Style
Imports EwEUtils.Core
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region  ' Imports

Public Class gridDrivers
    Inherits EwEGrid

    Private Enum eColumnTypes
        Index
        Name
        Units
        Min
        Max
        Mean
    End Enum

    Public Sub New()

    End Sub

    Protected Overrides Sub InitStyle()
        MyBase.InitStyle()

        ' ToDo: globalized this
        Me.Redim(1, [Enum].GetValues(GetType(eColumnTypes)).Length)
        Me(0, eColumnTypes.Index) = New EwEColumnHeaderCell()
        Me(0, eColumnTypes.Name) = New EwEColumnHeaderCell(SharedResources.HEADER_NAME)
        Me(0, eColumnTypes.Units) = New EwEColumnHeaderCell(SharedResources.HEADER_UNITS)
        Me(0, eColumnTypes.Min) = New EwEColumnHeaderCell("Min")
        Me(0, eColumnTypes.Max) = New EwEColumnHeaderCell("Max")
        Me(0, eColumnTypes.Mean) = New EwEColumnHeaderCell("Mean")

    End Sub

    Protected Overrides Sub FillData()

        Dim bm As cEcospaceBasemap = Me.Core.EcospaceBasemap

        Me.AddDriver(bm.LayerDepth)
        For i As Integer = 1 To Me.Core.nEnvironmentalDriverLayers
            Me.AddDriver(bm.LayerDriver(i))
        Next

    End Sub

    Private Sub AddDriver(driver As cEcospaceLayerSingle)
        Dim iRow As Integer = Me.AddRow()
        Me(iRow, eColumnTypes.Index) = New EwERowHeaderCell(CStr(driver.Index))
        ' Hmm, how does the Depth layer get its units?
        Me(iRow, eColumnTypes.Name) = New PropertyRowHeaderCell(Me.PropertyManager, driver, eVarNameFlags.Name)
        Me(iRow, eColumnTypes.Units) = New EwECell(driver.Units, cStyleGuide.eStyleFlags.NotEditable)
        Me(iRow, eColumnTypes.Min) = New EwECell(driver.MinValue, cStyleGuide.eStyleFlags.NotEditable)
        Me(iRow, eColumnTypes.Max) = New EwECell(driver.MaxValue, cStyleGuide.eStyleFlags.NotEditable)
        Me(iRow, eColumnTypes.Mean) = New EwECell(driver.MeanValue, cStyleGuide.eStyleFlags.NotEditable)
        Me.Rows(iRow).Tag = driver
    End Sub

    Public ReadOnly Property SelectedDriver As cEcospaceLayer
        Get
            If Me.SelectedRow < 1 Then Return Nothing
            Return DirectCast(Me.Rows(Me.SelectedRow).Tag, cEcospaceLayer)
        End Get
    End Property

End Class
