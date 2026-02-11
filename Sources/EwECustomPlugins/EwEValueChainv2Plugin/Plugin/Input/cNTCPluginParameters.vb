' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports SharedResources = ScientificInterfaceShared.My.Resources

Public Class cNTCPluginParameters
    Inherits cNavTreeControlPlugin

    Public Overrides ReadOnly Property Name() As String
        Get
            Return "vcV2Node01Parameters"
        End Get
    End Property

    Public Overrides ReadOnly Property DisplayName() As String
        Get
            Return My.Resources.NAVTREE_INPUT_PARAMETERS
        End Get
    End Property

    Public Overrides Function FormPage() As eValueChainPageTypes
        Return eValueChainPageTypes.Parameters
    End Function

    Public Overrides ReadOnly Property Description() As String
        Get
            Return "Value chain 'parameters' navigation element"
        End Get
    End Property

    Public Overrides ReadOnly Property NavigationTreeItemLocation() As String
        Get
            Return Me.NavTreeNodeRoot() & "|vcV2Node00"
        End Get
    End Property

    Public Overrides ReadOnly Property ControlImage() As Object
        Get
            Return SharedResources.nav_output
        End Get
    End Property

End Class
