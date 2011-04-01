#Region " Imports "

Option Strict On
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region ' Imports

Public Class cNavTree70CP_CycLiv_P
    Inherits cNavTree68CP_CycLiv

    Public Overrides ReadOnly Property ControlImage() As System.Drawing.Image
        Get
            Return SharedResources.nav1_application_put
        End Get
    End Property

    Public Overrides ReadOnly Property ControlText() As String
        Get
            Return My.Resources.PAGE_CP_PATHWAYS
        End Get
    End Property

    Public Overrides Function FormPage() As frmNetworkAnalysis.eNetworkAnalysisPageTypes
        Return frmNetworkAnalysis.eNetworkAnalysisPageTypes.SummaryOfPathways_living
    End Function

    Public Overrides ReadOnly Property Name() As String
        Get
            Return "nwa70"
        End Get
    End Property

    Public Overrides ReadOnly Property NavigationTreeItemLocation() As String
        Get
            Return MyBase.NavigationTreeItemLocation & "|" & MyBase.Name
        End Get
    End Property
End Class
