' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

#Region " Options "


Imports System.Drawing

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

End Class
