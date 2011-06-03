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
