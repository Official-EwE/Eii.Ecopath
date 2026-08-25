' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace MSE

    ''' <summary>
    ''' Stock-recruitment contract used by <see cref="cMSEQuotaCalculator"/>. Implemented by <see cref="cMSEStockRecruitment"/>,
    ''' but can be implemented independently to use the quota calculator without the full core data structures.
    ''' </summary>
    Public Interface IMSEStockRecruitment

        ''' <summary>
        ''' Estimate the biomass of a group for the current year from its true and observed biomass.
        ''' </summary>
        ''' <param name="iGroup">Group index.</param>
        ''' <param name="B">True biomass calculated by Ecosim.</param>
        ''' <param name="BioEst">Observed biomass (true biomass with observation error).</param>
        ''' <param name="Blast">Biomass estimate from the previous year.</param>
        ''' <param name="iCurYear">Current MSE year index.</param>
        ''' <returns>The biomass estimate for the current year.</returns>
        Function StockRecruitment(iGroup As Integer, B As Single, BioEst As Single, Blast As Single, iCurYear As Integer) As Single

    End Interface

End Namespace
