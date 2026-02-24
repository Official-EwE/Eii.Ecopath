' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Plugins.Ecosim

    Public Interface IEcosimRunInitializedPlugin
        Inherits IPlugin

        ''' <summary>
        ''' Ecosim has initialized and is about to start the time loop
        ''' </summary>
        ''' <param name="EcosimDatastructures"></param>
        ''' <remarks></remarks>
        Sub EcosimRunInitialized(EcosimDatastructures As Object)

    End Interface

End Namespace