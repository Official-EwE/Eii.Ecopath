' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Plugins.UI

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Plug-in interface that defines all functionality required to add a custom
    ''' item to the EwE options tree.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Interface IEwEOptionsPlugin
        Inherits IConfigurablePlugin

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Override this to specify the options tree node name for this plug-in.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        ReadOnly Property Label() As String

    End Interface

End Namespace