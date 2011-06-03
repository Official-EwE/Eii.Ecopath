#Region " Imports "

Option Strict On
Imports ScientificInterfaceShared.Controls

#End Region ' Imports

Public Class cPluginPointFishingMortality
    Inherits cPluginPointBase

    Public Overrides ReadOnly Property ControlText() As String
        Get
            Return "Fishing mortality functions grid"
        End Get
    End Property

    Public Overrides ReadOnly Property ControlTooltipText() As String
        Get
            Return Me.ControlText
        End Get
    End Property

    Public Overrides ReadOnly Property Description() As String
        Get
            Return "Display of all fishing mortality functions in a grid format"
        End Get
    End Property

    Public Overrides ReadOnly Property Name() As String
        Get
            Return "ndFishingMortalityGrid"
        End Get
    End Property

    Public Overrides ReadOnly Property NavigationTreeItemLocation() As String
        Get
            Return "ndTimeDynamic\ndEcosimInput\ndFishingMortality"
        End Get
    End Property

    Friend Overrides Function GridType() As Type
        Return GetType(gridFishingMortality)
    End Function

End Class
