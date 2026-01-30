' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.Collections.Generic
Imports ScientificInterfaceShared.Controls
Imports ValueChain



Public Interface IResultView

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Show results for all fleets or for an inidividual fleet.
    ''' </summary>
    ''' <param name="iFleet"></param>
    ''' <param name="lUnits"></param>
    ''' <param name="iYear">Year to show.</param>
    ''' <param name="result"></param>
    ''' -----------------------------------------------------------------------
    Sub ShowResults(iFleet As Integer, lUnits As cUnit(), result As cValueChainResults, iYear As Integer)

End Interface
