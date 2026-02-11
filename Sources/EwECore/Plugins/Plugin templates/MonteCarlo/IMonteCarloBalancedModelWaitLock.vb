' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.Threading

Namespace Plugins.MonteCarlo

    Public Interface IMonteCarloBalancedModelWaitLock
        Inherits IPlugin

        Sub MonteCarloEcopathModelBalancedWaitLock(MonteCarloThread As Thread, WaitEvent As ManualResetEvent)

    End Interface

End Namespace
