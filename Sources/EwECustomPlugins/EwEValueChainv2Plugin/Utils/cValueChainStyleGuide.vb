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

#Region " Options "

Option Strict On
Imports System.Drawing
Imports ScientificInterfaceShared.Style
Imports ValueChain

#End Region ' Options

Public Class cValueChainStyleGuide

    Public Shared Function GetImage(unitType As cUnitFactory.eUnitType) As Image

        Select Case unitType
            Case cUnitFactory.eUnitType.Producer
                Return My.Resources.icons8_fishing_32
            Case cUnitFactory.eUnitType.Processing
                Return My.Resources.icons8_factory_32
            Case cUnitFactory.eUnitType.Distribution
                Return My.Resources.icons8_shipped_32
            Case cUnitFactory.eUnitType.Wholesaler
                Return My.Resources.icons8_depot_32
            Case cUnitFactory.eUnitType.Retailer
                Return My.Resources.icons8_shopping_cart_32
            Case cUnitFactory.eUnitType.Consumer
                Return My.Resources.icons8_meal_32
        End Select
        Return Nothing
    End Function

    Public Shared Function GetColor(unittype As cUnitFactory.eUnitType) As Color
        Select Case unittype
            Case cUnitFactory.eUnitType.Producer
                Return Color.FromArgb(255, 0, 162, 255)
            Case cUnitFactory.eUnitType.Processing
                Return Color.FromArgb(255, 0, 168, 157)
            Case cUnitFactory.eUnitType.Distribution
                Return Color.FromArgb(255, 255, 100, 78)
            Case cUnitFactory.eUnitType.Wholesaler
                Return Color.FromArgb(255, 0, 118, 168)
            Case cUnitFactory.eUnitType.Retailer
                Return Color.FromArgb(255, 248, 186, 0)
            Case cUnitFactory.eUnitType.Consumer
                Return Color.FromArgb(255, 146, 146, 146)
        End Select
        Return Color.FromArgb(255, 224, 224, 224)
    End Function

    Public Shared Function GetStyle(unit As cUnit) As cStyleGuide.eStyleFlags

        Dim st As cStyleGuide.eStyleFlags = cStyleGuide.eStyleFlags.OK

        If (unit Is Nothing) Then
            st = cStyleGuide.eStyleFlags.MissingParameter
        Else
            If (unit.HasError) Then st = st Or cStyleGuide.eStyleFlags.ErrorEncountered

            If (TypeOf unit Is cProducerUnit) Then
                Dim pu As cProducerUnit = DirectCast(unit, cProducerUnit)
                If (Not String.IsNullOrWhiteSpace(pu.GearCode)) Then st = st Or cStyleGuide.eStyleFlags.ValueComputed
            End If
        End If

        Return st

    End Function

End Class
