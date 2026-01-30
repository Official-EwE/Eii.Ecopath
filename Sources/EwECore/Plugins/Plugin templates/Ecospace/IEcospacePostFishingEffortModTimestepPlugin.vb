' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Plugins.Ecospace

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Interface for extending the Ecospace fishing effort logic. Plug-ins of this
    ''' type are invoked as soon as Ecospace fishing effort has been calculated.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Interface IEcospacePostFishingEffortModTimestepPlugin
        Inherits IPlugin

        Sub EcospacePostFishingEffortModTimestep(EcospaceDatastructures As Object, iTime As Integer)

    End Interface

End Namespace