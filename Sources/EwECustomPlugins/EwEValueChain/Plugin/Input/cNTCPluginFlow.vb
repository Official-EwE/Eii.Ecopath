Imports SharedResources = ScientificInterfaceShared.My.Resources

Public Class cNTCPluginFlow
    Inherits cNavTreeControlPlugin

    Public Overrides ReadOnly Property Name() As String
        Get
            Return "vcNode03Flow"
        End Get
    End Property

    Public Overrides ReadOnly Property ControlText() As String
        Get
            Return "Flow"
        End Get
    End Property

    Public Overrides Function FormPage() As String
        Return "ndFlow"
    End Function

    Public Overrides ReadOnly Property Description() As String
        Get
            Return "Value chain 'flow' navigation element"
        End Get
    End Property

    Public Overrides ReadOnly Property NavigationTreeItemLocation() As String
        Get
            Return Me.NavTreeNodeRoot() & "|vcNode00"
        End Get
    End Property

    Public Overrides ReadOnly Property ControlImage() As System.Drawing.Image
        Get
            Return SharedResources.nav0_application_get
        End Get
    End Property

End Class
