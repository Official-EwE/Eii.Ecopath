' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore
Imports SharedResources = ScientificInterfaceShared.My.Resources



Public Class cNavTree88Sim_with_PPR
    Inherits cNavTree84Sim

    Public Overrides ReadOnly Property ControlImage() As Object
        Get
            Return SharedResources.nav_input
        End Get
    End Property

    Public Overrides ReadOnly Property DisplayName() As String
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

    Public Overrides ReadOnly Property EnabledState() As eCoreExecutionState
        Get
            Return eCoreExecutionState.EcosimLoaded
        End Get
    End Property

End Class
