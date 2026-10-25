' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace MSE

    ''' <summary>
    ''' Random number generation contract for the MSE. Implemented by <see cref="cRandomService"/>,
    ''' but can be implemented independently for dependency injection (DI) and testing.
    ''' </summary>
    Public Interface IRandomService

        ''' <summary>
        ''' Normally distributed random number (mean 0, std 1) via an inverse sigmoid transformation.
        ''' </summary>
        Function Normal2() As Single

        ''' <summary>
        ''' Normally distributed random number with the given standard deviation and mean.
        ''' </summary>
        ''' <param name="stdev">Standard deviation of the distribution.</param>
        ''' <param name="mean">Mean of the distribution.</param>
        Function RandNormDist(stdev As Single, mean As Single) As Single

        ''' <summary>
        ''' Box-Muller normally distributed random number with a standard deviation of one.
        ''' </summary>
        Function Normal() As Single

        ''' <summary>
        ''' Normally distributed random number where mean = 0 and std = 1.
        ''' </summary>
        Function RandomNormal() As Single

        ''' <summary>
        ''' Uniformly distributed random number greater than or equal to 0.0, and less than 1.0.
        ''' </summary>
        Function NextDouble() As Double

    End Interface

End Namespace
