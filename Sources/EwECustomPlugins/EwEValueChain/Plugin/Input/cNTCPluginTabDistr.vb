Imports SharedResources = ScientificInterfaceShared.My.Resources

Public Class cNTCPluginTabDistr
    Inherits cNavTreeControlPlugin

    Public Overrides ReadOnly Property Name() As String
        Get
            Return "vcNode23Distributors"
        End Get
    End Property

    Public Overrides ReadOnly Property ControlText() As String
        Get
            Return "Distributors"
        End Get
    End Property

    Public Overrides Function FormPage() As String
        Return "ndDistribution"
    End Function

    Public Overrides ReadOnly Property Description() As String
        Get
            Return "Value chain 'Distributors table' navigation element"
        End Get
    End Property

    Public Overrides ReadOnly Property NavigationTreeItemLocation() As String
        Get
            Return Me.NavTreeNodeRoot() & "|vcNode00|vcNode10Tables"
        End Get
    End Property

    Public Overrides ReadOnly Property ControlImage() As System.Drawing.Image
        Get
            Return SharedResources.nav0_application_get
        End Get
    End Property

End Class
