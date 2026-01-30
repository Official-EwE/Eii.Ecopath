' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.Common

Namespace Plugins.Ecospace

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Interface for providing an Ecospace result writer as a plug-in.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Interface IEcospaceResultWriterPlugin
        Inherits IPlugin
        Inherits IEcospaceResultsWriter

    End Interface

End Namespace