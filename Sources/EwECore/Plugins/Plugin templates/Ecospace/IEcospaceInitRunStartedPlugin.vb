' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Plugins.Ecospace

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Ecospace plugin point called when run initialization is about to start.
    ''' </summary>
    ''' <seealso cref="IEcospaceInitRunCompletedPlugin"/>
    ''' ---------------------------------------------------------------------------
    Public Interface IEcospaceInitRunStartedPlugin
        Inherits IPlugin

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Plug-in point that is called when Ecospace is about to initialize for running.
        ''' </summary>
        ''' <param name="EcospaceDatastructures">The ecospace datastructures.</param>
        ''' -----------------------------------------------------------------------
        Sub EcospaceInitRunStarted(EcospaceDatastructures As Object)

    End Interface

End Namespace