' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace MSE

    ''' <summary>
    ''' Minimal stats-recording contract used by <see cref="cMSEStockRecruitment"/>. Implemented by <see cref="cMSESummaryStats"/>,
    ''' but can be implemented independently to use the stock-recruitment model without the full core data structures.
    ''' </summary>
    Public Interface IMSESummaryStats

        ''' <summary>Add a data point for a grouping (group/fleet) at a time step.</summary>
        ''' <param name="index">One-based grouping index (group or fleet).</param>
        ''' <param name="TimeIndex">One-based time step index.</param>
        ''' <param name="Value">Value to record.</param>
        Sub AddValue(index As Integer, TimeIndex As Integer, Value As Single)

    End Interface

End Namespace
