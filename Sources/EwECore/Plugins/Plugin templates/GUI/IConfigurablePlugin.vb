' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.Common

Namespace Plugins.UI

    ''' ===========================================================================
    ''' <summary>
    ''' Plug-in point that provides a <see cref="IConfigurable">configurable</see>
    ''' interactions.
    ''' </summary>
    ''' ===========================================================================
    Public Interface IConfigurablePlugin
        Inherits IPlugin
        Inherits IConfigurable

    End Interface

End Namespace