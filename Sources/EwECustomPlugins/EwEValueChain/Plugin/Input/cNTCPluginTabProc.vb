Imports SharedResources = ScientificInterfaceShared.My.Resources

Public Class cNTCPluginTabProc
    Inherits cNavTreeControlPlugin

    Public Overrides ReadOnly Property Name() As String
        Get
            Return "nd01Processing"
        End Get
    End Property

    Public Overrides ReadOnly Property ControlText() As String
        Get
            Return "Processing"
        End Get
    End Property

    Public Overrides Function FormPage() As String
        Return "ndProcessing"
    End Function

    Public Overrides ReadOnly Property Description() As String
        Get
            Return "Value chain 'Processing table' navigation element"
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
