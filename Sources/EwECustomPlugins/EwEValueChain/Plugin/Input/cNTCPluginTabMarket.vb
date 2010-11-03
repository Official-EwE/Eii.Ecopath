Imports SharedResources = ScientificInterfaceShared.My.Resources

Public Class cNTCPluginTabMarket
    Inherits cNavTreeControlPlugin

    Public Overrides ReadOnly Property Name() As String
        Get
            Return "nd01Market"
        End Get
    End Property

    Public Overrides ReadOnly Property ControlText() As String
        Get
            Return "Market"
        End Get
    End Property

    Public Overrides Function FormPage() As String
        Return "ndMarket"
    End Function

    Public Overrides ReadOnly Property Description() As String
        Get
            Return "Value chain 'Market table' navigation element"
        End Get
    End Property

    Public Overrides ReadOnly Property NavigationTreeItemLocation() As String
        Get
            Return "ndParameterization|ndEcopathOutputTools|ndValueChain|nd10Tables"
        End Get
    End Property

    Public Overrides ReadOnly Property ControlImage() As System.Drawing.Image
        Get
            Return SharedResources.nav0_application_get
        End Get
    End Property

End Class
