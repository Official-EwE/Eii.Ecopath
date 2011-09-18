#Region " Imports "

Option Strict On
Imports ScientificInterfaceShared.Controls

#End Region ' Imports

Public Class cPluginPointHabitatCapacity
    Inherits cPluginPointBase

    Public Overrides ReadOnly Property ControlText() As String
        Get
            Return "Habitat Capacity functions grid"
        End Get
    End Property

    Public Overrides ReadOnly Property ControlTooltipText() As String
        Get
            Return Me.ControlText
        End Get
    End Property

    Public Overrides ReadOnly Property Description() As String
        Get
            Return "Display of all habitat capacity functions in a grid format"
        End Get
    End Property

    Public Overrides ReadOnly Property Name() As String
        Get
            Return "ndHabCapGrid"
        End Get
    End Property

    Public Overrides ReadOnly Property NavigationTreeItemLocation() As String
        Get
            Return "ndSpatialDynamic\ndEcospaceInput\ndHabCap"
        End Get
    End Property

    Friend Overrides Function GridType() As Type
        Return GetType(gridHabitatCapacity)
    End Function

    Public Overrides ReadOnly Property EnabledState As EwEUtils.Core.eCoreExecutionState
        Get
            Return EwEUtils.Core.eCoreExecutionState.EcospaceLoaded
        End Get
    End Property

End Class
