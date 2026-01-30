' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports SharedResources = ScientificInterfaceShared.My.Resources

Public Class cNTCPluginTabWholeseller
    Inherits cNavTreeControlPlugin

    Public Overrides ReadOnly Property Name() As String
        Get
            Return "vcNode22Wholeseller"
        End Get
    End Property

    Public Overrides ReadOnly Property DisplayName() As String
        Get
            Return My.Resources.NAVTREE_INPUT_TABLE_WHOLESALERS
        End Get
    End Property

    Public Overrides Function FormPage() As frmMain.eValueChainPageTypes
        Return frmMain.eValueChainPageTypes.TableWholesellers
    End Function

    Public Overrides ReadOnly Property Description() As String
        Get
            Return "Value chain 'wholeseller table' navigation element"
        End Get
    End Property

    Public Overrides ReadOnly Property NavigationTreeItemLocation() As String
        Get
            Return Me.NavTreeNodeRoot() & "|vcNode00|vcNode10Tables"
        End Get
    End Property

    Public Overrides ReadOnly Property ControlImage() As Object
        Get
            Return SharedResources.nav_output
        End Get
    End Property

End Class
