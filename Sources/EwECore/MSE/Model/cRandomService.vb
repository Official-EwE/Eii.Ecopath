' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace MSE

    ''' <summary>
    ''' Default <see cref="IRandomService"/> implementation. Owns its <see cref="Random"/> generator,
    ''' which can be seeded for reproducible sequences. Extracted from <see cref="cMSE"/>.
    ''' </summary>
    Public Class cRandomService
        Implements IRandomService

        Private ReadOnly m_rndGen As Random

        ''' <summary>
        ''' Create a new random service. When a seed is provided the generated sequence is reproducible.
        ''' </summary>
        ''' <param name="seed">Optional seed for the underlying random number generator.</param>
        Public Sub New(Optional seed As Integer? = Nothing)
            If seed.HasValue Then
                Me.m_rndGen = New Random(seed.Value)
            Else
                Me.m_rndGen = New Random()
            End If
        End Sub

        ''' <inheritdoc cref="IRandomService.Normal2"/>
        Public Function Normal2() As Single Implements IRandomService.Normal2
            Dim R As Single
            R = CSng(2 * Me.m_rndGen.NextDouble - 1)
            Return CSng(Math.Log((1 + R) / (1 - R)) / 1.82)
        End Function

        ''' <inheritdoc cref="IRandomService.RandNormDist"/>
        Public Function RandNormDist(stdev As Single, mean As Single) As Single Implements IRandomService.RandNormDist
            Return Me.Normal() * stdev + mean
        End Function

        ''' <inheritdoc cref="IRandomService.Normal"/>
        Public Function Normal() As Single Implements IRandomService.Normal
            Dim V1 As Double, V2 As Double
            Do
                V1 = Me.m_rndGen.NextDouble
                V2 = Me.m_rndGen.NextDouble
            Loop Until V1 > 0
            Return CSng(Math.Sqrt(-2 * Math.Log(V1)) * Math.Cos(2 * 3.14159 * V2))
        End Function

        ''' <inheritdoc cref="IRandomService.RandomNormal"/>
        Public Function RandomNormal() As Single Implements IRandomService.RandomNormal
            Dim X As Double
            X = -6
            For i As Integer = 1 To 12
                X = X + Me.m_rndGen.NextDouble
            Next
            Return CSng(X)
        End Function

        ''' <inheritdoc cref="IRandomService.NextDouble"/>
        Public Function NextDouble() As Double Implements IRandomService.NextDouble
            Return Me.m_rndGen.NextDouble
        End Function

    End Class

End Namespace
