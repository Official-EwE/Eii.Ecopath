' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.Common

Namespace Plugins.MonteCarlo

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Interface for providing an Monte Carlo result writer as a plug-in.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Interface IMonteCarloResultWriterPlugin
        Inherits IPlugin
        Inherits IMonteCarloResultsWriter

    End Interface

End Namespace