' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Plugins.UI
    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Plugin interface that defines all functionality required to add a menu
    ''' item to the EwE main menu. The menu item can be checked or unchecked.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Interface IMenuItemTogglePlugin
        Inherits IMenuItemPlugin

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Implement this to specify whether a menu item should be checked or 
        ''' unchecked at a given moment. This options should always have been part of 
        ''' <see cref="IMenuItemPlugin"/>.
        ''' </summary>
        ''' <remarks>
        ''' Note that the checked state may not show in the Windows UI if a plug-in 
        ''' has been given a <see cref="ControlImage"/>.
        ''' </remarks>
        ''' -----------------------------------------------------------------------
        ReadOnly Property IsChecked() As Boolean

    End Interface

End Namespace