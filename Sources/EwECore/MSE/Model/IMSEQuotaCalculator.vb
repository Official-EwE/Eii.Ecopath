' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace MSE

    ''' <summary>
    ''' Quota calculation contract for the MSE. Implemented by <see cref="cMSEQuotaCalculator"/>,
    ''' but can be implemented independently to use the MSE without the full core data structures.
    ''' </summary>
    Public Interface IMSEQuotaCalculator

        ''' <summary>
        ''' Estimate biomass per living group via the stock-recruitment model and store it in the quota data.
        ''' </summary>
        ''' <param name="Biomass">Biomass by group calculated by Ecosim.</param>
        ''' <param name="curYear">Current MSE year index.</param>
        ''' <param name="randomNormal">Supplies a normally distributed random number (mean 0, std 1).</param>
        Sub DoAssessment(Biomass() As Single, curYear As Integer, randomNormal As Func(Of Single))

        ''' <summary>
        ''' Set the quota, apply uncertainty and share it between the fleets. Returns the quota by group.
        ''' </summary>
        ''' <param name="randomNormal">Supplies a normally distributed random number (mean 0, std 1).</param>
        Function UpdateQuotas(randomNormal As Func(Of Single)) As Single()

    End Interface

End Namespace
