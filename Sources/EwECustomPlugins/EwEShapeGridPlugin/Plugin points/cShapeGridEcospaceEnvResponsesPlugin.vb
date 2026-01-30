' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore



Public Class cShapeGridEcospaceEnvResponsesPlugin
    Inherits cShapeGridPlugin

    Public Overrides ReadOnly Property DisplayName() As String
        Get
            Return My.Resources.GRID_FNRESPONSES
        End Get
    End Property

    Public Overrides ReadOnly Property ControlTooltipText() As String
        Get
            Return Me.DisplayName
        End Get
    End Property

    Public Overrides ReadOnly Property Description() As String
        Get
            Return My.Resources.DESC_HABCAP
        End Get
    End Property

    Public Overrides ReadOnly Property Name() As String
        Get
            ' Sort at the end of it all
            Return "ndXForagingResponseGridSpace"
        End Get
    End Property

    Public Overrides ReadOnly Property NavigationTreeItemLocation() As String
        Get
            Return "ndSpatialDynamic\ndEcospaceInput\ndEcospaceEnvironmentalResponse"
        End Get
    End Property

    Friend Overrides Function GridType() As Type
        Return GetType(gridFunctionalResponses)
    End Function

    Public Overrides ReadOnly Property EnabledState As eCoreExecutionState
        Get
            Return eCoreExecutionState.EcospaceLoaded
        End Get
    End Property

End Class
