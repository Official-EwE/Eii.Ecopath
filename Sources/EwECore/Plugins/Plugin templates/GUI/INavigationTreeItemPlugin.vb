' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Plugins.UI

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Plug-in interface that defines all functionality required to add a custom
    ''' item to the EwE navigation tree.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Interface INavigationTreeItemPlugin
        Inherits IGUIPlugin

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Override this to specify the navigation tree item location for this plug-in.
        ''' </summary>
        ''' <remarks>
        ''' <para>A location is a '\' separated series of TreeNode names, starting 
        ''' at the root node of the navigation tree that the plug-in is nested into.</para>
        ''' <para>Use of the '|' character to separate node names is deprecated.</para>
        ''' </remarks>
        ''' -----------------------------------------------------------------------
        ReadOnly Property NavigationTreeItemLocation() As String

    End Interface

End Namespace