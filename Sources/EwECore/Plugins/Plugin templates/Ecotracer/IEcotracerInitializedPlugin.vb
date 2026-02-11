' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Plugins.ContaminantTracing

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Ecotracer post-initialization plug-in
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Interface IEcotracerInitializedPlugin
        Inherits IPlugin

        Sub EcotracerInitialized(ContaminantTracerDatastructures As Object)

    End Interface

End Namespace
