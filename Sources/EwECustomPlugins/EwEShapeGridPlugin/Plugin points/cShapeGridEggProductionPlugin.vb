' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports ScientificInterfaceShared.Controls



Public Class cShapeGridEggProductionPlugin
    Inherits cShapeGridPlugin

    Public Overrides ReadOnly Property DisplayName As String
        Get
            Return My.Resources.GRID_EGGPROD
        End Get
    End Property

    Public Overrides ReadOnly Property ControlTooltipText As String
        Get
            Return Me.DisplayName
        End Get
    End Property

    Public Overrides ReadOnly Property Description As String
        Get
            Return My.Resources.DESC_EGGPROD
        End Get
    End Property

    Public Overrides ReadOnly Property Name As String
        Get
            Return "ndEggProductionXGrid"
        End Get
    End Property

    Public Overrides ReadOnly Property NavigationTreeItemLocation As String
        Get
            Return "ndTimeDynamic\ndEcosimInput\ndEP"
        End Get
    End Property

    Friend Overrides Function GridType() As Type
        Return GetType(gridEggProduction)
    End Function

End Class
