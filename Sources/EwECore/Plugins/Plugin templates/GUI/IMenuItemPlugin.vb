' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Plugins.UI

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Plugin interface that defines all functionality required to add a menu
    ''' item to the EwE main menu.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Interface IMenuItemPlugin
        Inherits IGUIPlugin

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' <para>
        ''' Implement this point to specify the menu item location for this plugin.
        ''' </para>
        ''' <para>A location is a '\' separated series of menu item names, starting 
        ''' at the root node of the menu that the plug-in is nested into.</para>
        ''' <para>Use of the '|' character to separate menu item names is deprecated.</para>
        ''' </summary>
        ''' -----------------------------------------------------------------------
        ReadOnly Property MenuItemLocation() As String

    End Interface

End Namespace