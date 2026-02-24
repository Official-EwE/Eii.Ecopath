' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Plugins

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Interface for implementing a plug-in that is explicitly de-initialized.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Interface IDisposedPlugin
        Inherits IPlugin

        Sub Dispose()

    End Interface

End Namespace