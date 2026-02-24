' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Plugins.Ecospace

    Public Interface IEcospaceRunCompletedPlugin
        Inherits IPlugin

        Sub EcospaceRunCompleted(EcoSpaceDatastructures As Object)

    End Interface

End Namespace