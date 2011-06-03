#Region " Imports "

Option Strict On
Imports ScientificInterfaceShared.Controls

#End Region ' Imports

Public Class cPluginPointTimeSeries
    Inherits cPluginPointBase

    Public Overrides ReadOnly Property ControlText As String
        Get
            Return "Time series grid"
        End Get
    End Property

    Public Overrides ReadOnly Property ControlTooltipText As String
        Get
            Return Me.ControlText
        End Get
    End Property

    Public Overrides ReadOnly Property Description As String
        Get
            Return "Display of all loaded time series in a grid format"
        End Get
    End Property

    Public Overrides ReadOnly Property Name As String
        Get
            Return "ndTimeSeriesGrid"
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
