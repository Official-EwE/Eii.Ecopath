Imports SharedResources = ScientificInterfaceShared.My.Resources

Public Class cNTCPluginParameters
    Inherits cNavTreeControlPlugin

    Public Overrides ReadOnly Property Name() As String
        Get
            Return "nd00Parameters"
        End Get
    End Property

    Public Overrides ReadOnly Property ControlText() As String
        Get
            Return "Parameters"
        End Get
    End Property

    Public Overrides Function FormPage() As String
        Return ""
    End Function

    Public Overrides ReadOnly Property Description() As String
        Get
            Return "Value chain 'parameters' navigation element"
        End Get
    End Property

    Public Overrides ReadOnly Property NavigationTreeItemLocation() As String
        Get
            Return "ndParameterization|ndEcopathOutputTools|ndValueChain"
        End Get
    End Property

    Public Overrides ReadOnly Property ControlImage() As System.Drawing.Image
        Get
            Return SharedResources.nav0_application_get
        End Get
    End Property

End Class
