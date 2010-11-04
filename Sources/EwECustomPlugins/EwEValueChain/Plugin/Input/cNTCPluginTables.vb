Imports SharedResources = ScientificInterfaceShared.My.Resources

Public Class cNTCPluginTables
    Inherits cNavTreeControlPlugin

    Public Overrides ReadOnly Property Name() As String
        Get
            Return "vcNode10Tables"
        End Get
    End Property

    Public Overrides ReadOnly Property ControlText() As String
        Get
            Return "Tables"
        End Get
    End Property

    Public Overrides Function FormPage() As String
        Return "ndTables"
    End Function

    Public Overrides ReadOnly Property Description() As String
        Get
            Return "Value chain 'tables' navigation element"
        End Get
    End Property

    Public Overrides ReadOnly Property NavigationTreeItemLocation() As String
        Get
            Return Me.NavTreeNodeRoot() & "|vcNode00"
        End Get
    End Property

    Public Overrides ReadOnly Property ControlImage() As System.Drawing.Image
        Get
            Return SharedResources.nav4_output_extend
        End Get
    End Property

End Class
