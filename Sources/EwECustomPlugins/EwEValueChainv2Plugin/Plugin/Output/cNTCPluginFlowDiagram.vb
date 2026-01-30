' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports SharedResources = ScientificInterfaceShared.My.Resources

Public Class cNTCPluginFD
    Inherits cNavTreeControlPlugin

    Public Overrides ReadOnly Property Name() As String
        Get
            Return "vcV2Node54FlowDiagram"
        End Get
    End Property

    Public Overrides ReadOnly Property DisplayName() As String
        Get
            Return My.Resources.NAVTREE_OUTPUT_FLOWDIAGRAM
        End Get
    End Property

    Public Overrides Function FormPage() As eValueChainPageTypes
        Return eValueChainPageTypes.FlowDiagram
    End Function

    Public Overrides ReadOnly Property Description() As String
        Get
            Return "Value chain flow diagram"
        End Get
    End Property

    Public Overrides ReadOnly Property NavigationTreeItemLocation() As String
        Get
            Return Me.NavTreeNodeRoot() & "|vcV2Node00"
        End Get
    End Property

    Public Overrides ReadOnly Property ControlImage() As Object
        Get
            Return SharedResources.nav_input
        End Get
    End Property

End Class
