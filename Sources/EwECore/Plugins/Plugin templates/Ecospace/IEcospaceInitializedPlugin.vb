' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Plugins.Ecospace

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Interface for extending the Ecospace initialzation logic. Plug-ins of this
    ''' type are invoked as soon as all Ecospace data is loaded in the EwE Core.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Interface IEcospaceInitializedPlugin
        Inherits IPlugin

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Plug-in point that is called when Ecospace has loaded a new scenario, is
        ''' initialized, and is ready to be used.
        ''' </summary>
        ''' <param name="EcospaceDatastructures">The ecospace datastructures that 
        ''' just received new scenario data.</param>
        ''' -----------------------------------------------------------------------
        Sub EcospaceInitialized(EcospaceDatastructures As Object)

    End Interface

End Namespace