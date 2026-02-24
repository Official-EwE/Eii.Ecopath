' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Plugins.UI

    ''' ===========================================================================
    ''' <summary>
    ''' Plug-in that should automatically launch its User Interface when loaded.
    ''' </summary>
    ''' ===========================================================================
    Public Interface IAutolaunchPlugin
        Inherits IGUIPlugin

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Plug-in point to state whether auto-launch is active. If set to true,
        ''' the plug-in will be launched, activating its user interface if available.
        ''' </summary>
        ''' <returns>A plug-in should return true if it desires to be auto-lanched.</returns>
        ''' -----------------------------------------------------------------------
        Function Autolaunch() As Boolean

    End Interface

End Namespace