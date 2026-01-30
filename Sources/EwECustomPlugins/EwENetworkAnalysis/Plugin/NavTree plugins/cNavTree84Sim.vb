' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore
Imports SharedResources = ScientificInterfaceShared.My.Resources

Public Class cNavTree84Sim
    Inherits cNavTreeControlPlugin

    Public Overrides ReadOnly Property ControlImage() As Object
        Get
            Return SharedResources.nav_output
        End Get
    End Property

    Public Overrides ReadOnly Property DisplayName() As String
        Get
            Return My.Resources.PAGE_ECOSIM_NA
        End Get
    End Property

    Public Overrides Function FormPage() As frmNetworkAnalysis.eNetworkAnalysisPageTypes
        Return frmNetworkAnalysis.eNetworkAnalysisPageTypes.WithoutPrimaryProductionRequiredEstimate
    End Function

    Public Overrides ReadOnly Property Name() As String
        Get
            Return "nwa84"
        End Get
    End Property

    Public Overrides ReadOnly Property NavigationTreeItemLocation() As String
        Get
            Return Me.NavTreeNodeRoot & "|nwa00"
        End Get
    End Property

    Public Overrides ReadOnly Property EnabledState() As eCoreExecutionState
        Get
            Return eCoreExecutionState.EcosimLoaded
        End Get
    End Property

End Class
