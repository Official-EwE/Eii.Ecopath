' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports ScientificInterfaceShared.Controls



Public Class cShapeGridTimeSeriesPlugin
    Inherits cShapeGridPlugin

    Public Overrides ReadOnly Property DisplayName As String
        Get
            Return My.Resources.GRID_TIMESERIES
        End Get
    End Property

    Public Overrides ReadOnly Property ControlTooltipText As String
        Get
            Return Me.DisplayName
        End Get
    End Property

    Public Overrides ReadOnly Property Description As String
        Get
            Return My.Resources.DESC_TIMESERIES
        End Get
    End Property

    Public Overrides ReadOnly Property Name As String
        Get
            Return "ndTimeSeriesXGrid"
        End Get
    End Property

    Public Overrides ReadOnly Property NavigationTreeItemLocation As String
        Get
            Return "ndTimeDynamic\ndEcosimInput\ndTimeSeries"
        End Get
    End Property

    Friend Overrides Function GridType() As Type
        Return GetType(gridTimeSeries)
    End Function

End Class
