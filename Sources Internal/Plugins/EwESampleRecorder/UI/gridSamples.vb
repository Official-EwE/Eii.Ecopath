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
Imports EwECore.Samples
Imports EwEUtils.Core
Imports EwEUtils.SystemUtilities
Imports ScientificInterfaceShared.Controls.EwEGrid
Imports ScientificInterfaceShared.Style
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region ' Imports


Public Class gridSamples
    Inherits EwEGrid

    Private Enum eColumnTypes As Integer
        Index
        Loaded
        Rating
        [Date]
        System
    End Enum

    Public Sub New()
        MyBase.new()
    End Sub

    Protected Overrides Sub InitStyle()
        MyBase.InitStyle()

        Me.Redim(1, [Enum].GetValues(GetType(eColumnTypes)).Length)

        ' ToDo: globalize this
        Me(0, eColumnTypes.Index) = New EwEColumnHeaderCell()
        Me(0, eColumnTypes.Loaded) = New EwEColumnHeaderCell("Loaded")
        Me(0, eColumnTypes.Rating) = New EwEColumnHeaderCell("Rating")
        Me(0, eColumnTypes.Date) = New EwEColumnHeaderCell("Date")
        Me(0, eColumnTypes.System) = New EwEColumnHeaderCell("System")

        Me.FixedColumns = 2
        Me.FixedColumnWidths = False
        ' Me.Selection.SelectionMode = SourceGrid2.GridSelectionMode.Row
        Me.AllowBlockSelect = False

    End Sub

    Protected Overrides Sub FillData()

        If (Me.UIContext Is Nothing) Then Return

        Dim man As cEcopathSampleManager = Me.Core.SampleManager

        For i As Integer = 1 To man.nSamples
            Dim iRow As Integer = Me.AddRow()
            Dim s As cEcopathSample = man.Sample(i)
            Me(iRow, eColumnTypes.Index) = New PropertyRowHeaderCell(Me.PropertyManager, s, eVarNameFlags.Index)
            Me(iRow, eColumnTypes.Rating) = New PropertyCell(Me.PropertyManager, s, eVarNameFlags.SampleRating)
            Me(iRow, eColumnTypes.Loaded) = New EwECell("", cStyleGuide.eStyleFlags.NotEditable)
            Me(iRow, eColumnTypes.Date) = New EwECell(s.Generated, cStyleGuide.eStyleFlags.NotEditable)
            Me(iRow, eColumnTypes.System) = New EwECell(s.Source, cStyleGuide.eStyleFlags.NotEditable)
            Me.Sample(iRow) = s
        Next

        Me.UpdateLoadState()

    End Sub

    Protected Overrides Sub FinishStyle()
        MyBase.FinishStyle()
    End Sub

    Public Sub UpdateLoadState()

        Dim man As cEcopathSampleManager = Me.Core.SampleManager

        For iRow As Integer = 1 To Me.RowsCount - 1
            Dim s As cEcopathSample = Me.Sample(iRow)
            Me(iRow, eColumnTypes.Loaded).Value = cSystemUtils.IIF(man.IsLoaded(s), SharedResources.GENERIC_VALUE_YES, "")
            Me(iRow, eColumnTypes.Rating).Value = s.Rating
        Next

    End Sub

    Public Property Sample(iRow As Integer) As cEcopathSample
        Get
            If (iRow < 1) Or (iRow >= Me.RowsCount) Then Return Nothing
            Return DirectCast(Me.Rows(iRow).Tag, cEcopathSample)
        End Get
        Private Set(value As cEcopathSample)
            If (iRow < 1) Or (iRow >= Me.RowsCount) Then Return
            Me.Rows(iRow).Tag = value
        End Set
    End Property

End Class
