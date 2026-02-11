' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Plugins.Ecospace

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Ecospace plugin point called when all run initialization has completed and 
    ''' time steps are about to begin.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Interface IEcospaceInitRunCompletedPlugin
        Inherits IPlugin

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Plug-in point that is called when Ecospace is about to start running.
        ''' </summary>
        ''' <param name="EcospaceDatastructures">The ecospace datastructures.</param>
        ''' -----------------------------------------------------------------------
        Sub EcospaceInitRunCompleted(EcospaceDatastructures As Object)

    End Interface

End Namespace