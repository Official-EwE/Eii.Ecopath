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

#End Region ' Imports

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
        Me.RefreshContent()

    End Sub

#End Region ' Public access

#Region " Grid overrides "

    Protected Overrides Sub InitStyle()
        MyBase.InitStyle()

        Dim cell As EwECell = Nothing

        Me.Redim(1, [Enum].GetValues(GetType(eColumnTypes)).Length)

        cell = New EwEColumnHeaderCell(My.Resources.HEADER_SOURCE)
        Me(0, eColumnTypes.Source1) = cell

        cell = New EwEColumnHeaderCell(My.Resources.HEADER_PARAMETER)
        Me(0, eColumnTypes.Parameter) = cell

        cell = New EwEColumnHeaderCell(My.Resources.HEADER_SOURCE_SEC)
        Me(0, eColumnTypes.Source2) = cell

        cell = New EwEColumnHeaderCell(SharedResources.HEADER_REMARK)
        cell.VisualModel.TextAlignment = Drawing.ContentAlignment.MiddleLeft
        Me(0, eColumnTypes.Remark) = cell

        Me.FixedColumnWidths = False
        Me.FixedColumns = 0
        Me.AutoStretchColumnsToFitWidth = True

        ' Make sure this grid does NOT screw up the selection of properties!
        Me.TrackPropertySelection = False

    End Sub

    Protected Overrides Sub FillData()

        If (Me.UIContext Is Nothing) Then Return
        If (Me.m_data Is Nothing) Then Return

        Dim vfm As New cVarnameTypeFormatter()
        Dim cfm As New cCoreInterfaceFormatter("")
        Dim styleRO As cStyleGuide.eStyleFlags = cStyleGuide.eStyleFlags.NotEditable
        Dim cell As EwECellBase = Nothing
        Dim prop As cProperty = Nothing

        For i As Integer = 0 To Me.m_data.Length - 1

            prop = Me.m_data(i)
            Me.AddRow()

            cell = New PropertyRowHeaderCell(Me.PropertyManager, prop.Source, eVarNameFlags.Name)
            ' Prevent 'Remarks' from popping up
            cell.Style = cStyleGuide.eStyleFlags.Names Or cStyleGuide.eStyleFlags.NotEditable
            Me(Me.RowsCount - 1, eColumnTypes.Source1) = cell

            cell = New EwERowHeaderCell(vfm.GetDescriptor(prop.VarName))
            Me(Me.RowsCount - 1, eColumnTypes.Parameter) = cell

            cell = New PropertyRowHeaderCell(Me.PropertyManager, prop.SourceSec, eVarNameFlags.Name)
            ' Prevent 'Remarks' from popping up
            cell.Style = cStyleGuide.eStyleFlags.Names Or cStyleGuide.eStyleFlags.NotEditable
            Me(Me.RowsCount - 1, eColumnTypes.Source2) = cell

            Me(Me.RowsCount - 1, eColumnTypes.Remark) = New cRemarkCell(prop)

        Next i

    End Sub

#End Region ' Grid overrides

End Class
