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

Imports EwECore
Imports ScientificInterfaceShared.Controls.EwEGrid
Imports EwEUtils.Utilities
Imports SharedResources = ScientificInterfaceShared.My.Resources

Public Class gridApplyShape
    Inherits EwEGrid

    Private Enum eColumnTypes
        Index
        Group
        Response
        Thumbnail
        Type
        Min
        Max
    End Enum

    Public Sub New()

    End Sub

    Protected Overrides Sub InitStyle()
        MyBase.InitStyle()

        ' ToDo: globalized this
        Me.Redim(1, [Enum].GetValues(GetType(eColumnTypes)).Length)
        Me(0, eColumnTypes.Index) = New EwEColumnHeaderCell()
        Me(0, eColumnTypes.Group) = New EwEColumnHeaderCell(SharedResources.HEADER_GROUP)
        Me(0, eColumnTypes.Response) = New EwEColumnHeaderCell("Response")
        Me(0, eColumnTypes.Thumbnail) = New EwEColumnHeaderCell("Preview")
        Me(0, eColumnTypes.Type) = New EwEColumnHeaderCell("Type")
        Me(0, eColumnTypes.Min) = New EwEColumnHeaderCell("Min")
        Me(0, eColumnTypes.Max) = New EwEColumnHeaderCell("Max")

    End Sub

    Private m_driver As cEcospaceLayer = Nothing

    Public Property SelectedDriver As cEcospaceLayer
        Get
            Return Me.m_driver
        End Get
        Set(value As cEcospaceLayer)
            If (Object.ReferenceEquals(value, Me.m_driver)) Then Return
            Me.m_driver = value
            Me.RefreshContent()
        End Set
    End Property

    Protected Overrides Sub FillData()

        If (Me.m_driver Is Nothing) Then Return

        Me(0, eColumnTypes.Response).Value = cStringUtils.Localize("Response to {0}", Me.m_driver.Name)

        For i As Integer = 1 To Me.Core.nGroups
            Dim group As cEcoPathGroupInput = Me.Core.EcoPathGroupInputs(i)
            Dim iRow As Integer = Me.AddRow()
            Me(iRow, eColumnTypes.Index) = New EwERowHeaderCell(CStr(i))
            Me(iRow, eColumnTypes.Group) = New PropertyRowHeaderCell(Me.PropertyManager, group, EwEUtils.Core.eVarNameFlags.Name)
            Me(iRow, eColumnTypes.Response) = New EwECell("<function name>")
            Me(iRow, eColumnTypes.Thumbnail) = New EwECell("<thmb>")
            Me(iRow, eColumnTypes.Type) = New EwECell("<type>")
            Me(iRow, eColumnTypes.Min) = New EwECell("0")
            Me(iRow, eColumnTypes.Max) = New EwECell("42")
        Next
    End Sub

End Class
