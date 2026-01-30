' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.Common

Namespace Plugins.Data

    ''' <summary>
    ''' Plug-in point allowing taxonomy <see cref="IDataSearchProducerPlugin">search
    ''' plug-ins</see> to report their search capabilities.
    ''' </summary>
    Public Interface ITaxonSearchCapabilitiesPlugin
        Inherits ITaxonSearchCapabilities
        Inherits IDataSearchProducerPlugin

    End Interface

End Namespace
