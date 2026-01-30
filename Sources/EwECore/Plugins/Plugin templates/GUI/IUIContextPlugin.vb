' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Plugins.UI

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Plugin interface that defines all functionality required to receive a user
    ''' interface UI context.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Interface IUIContextPlugin
        Inherits IPlugin

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Implement this plug-in point to receive a user interface context. See
        ''' ScientificInterfaceShared > Controls > cUIContext for a full description
        ''' of this object.
        ''' </summary>
        ''' <param name="uic"></param>
        ''' -----------------------------------------------------------------------
        Sub UIContext(uic As Object)

    End Interface

End Namespace