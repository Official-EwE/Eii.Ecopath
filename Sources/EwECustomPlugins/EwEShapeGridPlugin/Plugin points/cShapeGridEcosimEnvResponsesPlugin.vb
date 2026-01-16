' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 3 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see https://www.gnu.org/licenses/gpl-3.0.html>. 
'
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'




Imports EwECore



Public Class cShapeGridEcosimEnvResponsesPlugin
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
            Return "ndXForagingResponseGridSim"
        End Get
    End Property

    Public Overrides ReadOnly Property NavigationTreeItemLocation() As String
        Get
            Return "ndTimeDynamic\ndEcosimInput\ndEcosimEnvironmentalResponse"
        End Get
    End Property

    Friend Overrides Function GridType() As Type
        Return GetType(gridFunctionalResponses)
    End Function

    Public Overrides ReadOnly Property EnabledState As eCoreExecutionState
        Get
            Return eCoreExecutionState.EcosimLoaded
        End Get
    End Property

End Class
