#Region " Imports "

Option Strict On
Imports ScientificInterfaceShared.Controls

#End Region ' Imports

Public Class cPluginPointLandingsMediation
    Inherits cPluginPointBase

    Public Overrides ReadOnly Property ControlText() As String
        Get
            Return "Price elasticity functions grid"
        End Get
    End Property

    Public Overrides ReadOnly Property ControlTooltipText() As String
        Get
            Return Me.ControlText
        End Get
    End Property

    Public Overrides ReadOnly Property Description() As String
        Get
            Return "Display of all price elasticity functions in a grid format"
        End Get
    End Property

    Public Overrides ReadOnly Property Name() As String
        Get
            Return "ndPriceElasticityGrid"
        End Get
    End Property

    Public Overrides ReadOnly Property NavigationTreeItemLocation() As String
        Get
            Return "ndTimeDynamic\ndEcosimInput\ndPriceElasticity"
        End Get
    End Property

    Friend Overrides Function GridType() As Type
        Return GetType(gridLandingsMediation)
    End Function

End Class
