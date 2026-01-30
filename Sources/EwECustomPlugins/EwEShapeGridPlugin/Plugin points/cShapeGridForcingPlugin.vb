' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports ScientificInterfaceShared.Controls



Public Class cShapeGridForcingPlugin
    Inherits cShapeGridPlugin

    Public Overrides ReadOnly Property DisplayName As String
        Get
            Return My.Resources.GRID_FORCING
        End Get
    End Property

    Public Overrides ReadOnly Property ControlTooltipText As String
        Get
            Return Me.DisplayName
        End Get
    End Property

    Public Overrides ReadOnly Property Description As String
        Get
            Return My.Resources.DESC_FORCING
        End Get
    End Property

    Public Overrides ReadOnly Property Name As String
        Get
            Return "ndForcingFunctionsXGrid"
        End Get
    End Property

    Public Overrides ReadOnly Property NavigationTreeItemLocation As String
        Get
            Return "ndTimeDynamic\ndEcosimInput\ndForcingFunction"
        End Get
    End Property

    Friend Overrides Function GridType() As Type
        Return GetType(gridForcing)
    End Function

End Class
