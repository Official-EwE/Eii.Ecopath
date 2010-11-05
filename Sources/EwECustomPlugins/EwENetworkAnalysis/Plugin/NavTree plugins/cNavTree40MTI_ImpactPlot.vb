#Region " Imports "

Option Strict On
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region ' Imports

Public Class cNavTree40MTI_ImpactPlot
    Inherits cNavTree36MTI

    Public Overrides ReadOnly Property ControlImage() As System.Drawing.Image
        Get
            Return SharedResources.nav1_application_put
        End Get
    End Property

    Public Overrides ReadOnly Property ControlText() As String
        Get
            Return My.Resources.PAGE_MTI_PLOT
        End Get
    End Property

    Public Overrides Function FormPage() As frmNetworkAnalysis.eNetworkAnalysisPageTypes
        Return frmNetworkAnalysis.eNetworkAnalysisPageTypes.GraphOfMixedTrophicImpact
    End Function

    Public Overrides ReadOnly Property Name() As String
        Get
            Return "nwa40"
        End Get
    End Property

    Public Overrides ReadOnly Property NavigationTreeItemLocation() As String
        Get
            Return MyBase.NavigationTreeItemLocation & "|" & MyBase.Name
        End Get
    End Property
End Class
