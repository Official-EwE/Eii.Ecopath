#Region " Imports "

Option Strict On
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region ' Imports

Public Class cNavTree88Sim_with_PPR
    Inherits cNavTree84Sim

    Public Overrides ReadOnly Property ControlImage() As System.Drawing.Image
        Get
            Return SharedResources.nav1_application_put
        End Get
    End Property

    Public Overrides ReadOnly Property ControlText() As String
        Get
            Return My.Resources.PAGE_ECOSIM_NA_WITH_PPR
        End Get
    End Property

    Public Overrides Function FormPage() As frmNetworkAnalysis.eNetworkAnalysisPageTypes
        Return frmNetworkAnalysis.eNetworkAnalysisPageTypes.WithPrimaryProductionRequiredEstimate
    End Function

    Public Overrides ReadOnly Property Name() As String
        Get
            Return "nwa88"
        End Get
    End Property

    Public Overrides ReadOnly Property NavigationTreeItemLocation() As String
        Get
            Return MyBase.NavigationTreeItemLocation & "|" & MyBase.Name
        End Get
    End Property

    Public Overrides ReadOnly Property EnabledState() As EwEUtils.Core.eCoreExecutionState
        Get
            Return EwEUtils.Core.eCoreExecutionState.EcosimLoaded
        End Get
    End Property

End Class
