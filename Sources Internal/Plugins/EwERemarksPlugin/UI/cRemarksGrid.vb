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
' Copyright 1991-2012 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'
#Region " Imports "

Option Strict On
Imports ScientificInterfaceShared.Controls.EwEGrid
Imports ScientificInterfaceShared.Style
Imports EwEUtils.Core
Imports EwERemarksPlugin
Imports EwECore.Auxiliary
Imports EwECore
Imports SourceGrid2.VisualModels
Imports ScientificInterfaceShared.Properties
Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports System.Windows.Forms

#End Region ' Imports

<CLSCompliant(False)> _
Friend Class cRemarksGrid
    Inherits EwEGrid

#Region " Private vars "

    Private m_data As cProperty() = Nothing

    Private Enum eColumnTypes As Integer
        Source1 = 0
        Parameter
        Source2
        Remark
    End Enum

#End Region ' Private vars

#Region " Construction "

    Public Sub New()
        MyBase.New()
    End Sub

#End Region ' Construction

#Region " Public access "

    Public Sub SetData(ByVal data() As cProperty)

        Me.m_data = data
        Me.FillData()

    End Sub

#End Region ' Public access

#Region " Grid overrides "

    Protected Overrides Sub InitStyle()
        MyBase.InitStyle()

        Me.Redim(1, [Enum].GetValues(GetType(eColumnTypes)).Length)

        Me(0, eColumnTypes.Source1) = New EwEColumnHeaderCell(My.Resources.HEADER_SOURCE)
        Me(0, eColumnTypes.Parameter) = New EwEColumnHeaderCell(My.Resources.HEADER_PARAMETER)
        Me(0, eColumnTypes.Source2) = New EwEColumnHeaderCell(My.Resources.HEADER_SOURCE_SEC)

        Me(0, eColumnTypes.Remark) = New EwEColumnHeaderCell(SharedResources.HEADER_REMARK)
        Me(0, eColumnTypes.Remark).VisualModel.TextAlignment = Drawing.ContentAlignment.MiddleLeft

        Me.TrackPropertySelection = False
        Me.FixedColumnWidths = False
        Me.FixedColumns = 3

    End Sub

    Protected Overrides Sub FillData()

        If (Me.UIContext Is Nothing) Then Return
        If (Me.m_data Is Nothing) Then Return

        Me.RowsCount = 1

        Dim vfm As New cVarnameTypeFormatter()

        For i As Integer = 0 To Me.m_data.Length - 1

            Dim prop As cProperty = Me.m_data(i)
            Dim cell As EwECellBase = Nothing
            Dim iRow As Integer = Me.AddRow()

            cell = New PropertyCell(Me.PropertyManager, prop.Source, eVarNameFlags.Name)
            cell.Style = cStyleGuide.eStyleFlags.Names Or cStyleGuide.eStyleFlags.NotEditable
            Me(iRow, eColumnTypes.Source1) = cell

            cell = New EwECell(vfm.GetDescriptor(prop.VarName), GetType(String), cStyleGuide.eStyleFlags.NotEditable)
            Me(iRow, eColumnTypes.Parameter) = cell

            cell = New PropertyCell(Me.PropertyManager, prop.SourceSec, eVarNameFlags.Name)
            cell.Style = cStyleGuide.eStyleFlags.Names Or cStyleGuide.eStyleFlags.NotEditable
            Me(iRow, eColumnTypes.Source2) = cell

            Me(iRow, eColumnTypes.Remark) = New cRemarkCell(prop)

        Next i

    End Sub

    Protected Overrides Sub FinishStyle()

        Me.Columns(eColumnTypes.Source1).AutoSizeMode = SourceGrid2.AutoSizeMode.EnableAutoSize
        Me.Columns(eColumnTypes.Parameter).AutoSizeMode = SourceGrid2.AutoSizeMode.EnableAutoSize
        Me.Columns(eColumnTypes.Source2).AutoSizeMode = SourceGrid2.AutoSizeMode.EnableAutoSize
        Me.Columns(eColumnTypes.Remark).AutoSizeMode = SourceGrid2.AutoSizeMode.EnableStretch
        Me.AutoStretchColumnsToFitWidth = True

        MyBase.FinishStyle()

    End Sub

#End Region ' Grid overrides

End Class
