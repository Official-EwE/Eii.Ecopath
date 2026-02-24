' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Plugins.UI

    ''' ===========================================================================
    ''' <summary>
    ''' Plugin point that allows a GUI plugin to state its desired dock location.
    ''' </summary>
    ''' ===========================================================================
    Public Interface IDockStatePlugin
        Inherits IGUIPlugin

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' The dockstate for the form of this plugin.
        ''' </summary>
        ''' <remarks>
        ''' Values are interpreted as
        ''' WeifenLuo DockState enumerated values. This project is not linked to
        ''' WeifenLuo's DockPanel suite, but implementing plug-ins can include
        ''' such a reference and return actual DockState enumerated values here.
        ''' </remarks>
        ''' -----------------------------------------------------------------------
        Function DockState() As Integer

    End Interface

End Namespace