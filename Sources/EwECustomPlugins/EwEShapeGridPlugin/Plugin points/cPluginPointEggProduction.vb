#Region " Imports "

Option Strict On
Imports ScientificInterfaceShared.Controls

#End Region ' Imports

Public Class cPluginPointEggProduction
    Inherits cPluginPointBase

    Public Overrides ReadOnly Property ControlText As String
        Get
            Return "Egg production functions grid"
        End Get
    End Property

    Public Overrides ReadOnly Property ControlTooltipText As String
        Get
            Return Me.ControlText
        End Get
    End Property

    Public Overrides ReadOnly Property Description As String
        Get
            Return "Display of all egg produciton functions in a grid format"
        End Get
    End Property

    Public Overrides ReadOnly Property Name As String
        Get
            Return "ndEggProductionGrid"
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
