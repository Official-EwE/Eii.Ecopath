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
' Copyright 1991-2013 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'
#Region " Imports "

Option Strict On
Imports ScientificInterfaceShared.Controls

#End Region ' Imports

Public Class cPluginPointPredPreyMediation
    Inherits cPluginPointBase

    Public Overrides ReadOnly Property ControlText() As String
        Get
            Return "Mediation functions grid"
        End Get
    End Property

    Public Overrides ReadOnly Property ControlTooltipText() As String
        Get
            Return Me.ControlText
        End Get
    End Property

    Public Overrides ReadOnly Property Description() As String
        Get
            Return "Display of all mediation functions in a grid format"
        End Get
    End Property

    Public Overrides ReadOnly Property Name() As String
        Get
            Return "ndMediationGrid"
        End Get
    End Property

    Public Overrides ReadOnly Property NavigationTreeItemLocation() As String
        Get
            Return "ndTimeDynamic\ndEcosimInput\ndMediation"
        End Get
    End Property

    Friend Overrides Function GridType() As Type
        Return GetType(gridPredPreyMediation)
    End Function

End Class
