' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Public Class cF2TSDataStructures

    Public bVulnerabilitySearch As Boolean = True
    Public bCatchAnomaly As Boolean = False
    Public bAnomalySearch As Boolean = False
    Public FirstYear As Integer = 1
    Public LastYear As Integer = 1
    Public VulnerabilityVariance As Single = 10.0!
    Public PPVariance As Single = 0.1!
    ''' <summary>Index of anomaly shape.</summary>
    ''' <remarks>One-based or zero-based? Shape managers are zero-based; stepwise fitting assumes one-based!</remarks>
    Public iCatchAnomalySearchShapeNumber As Integer = 0
    Public nNumSplinePoints As Integer = 0
    Public RunSilent As Boolean = False

    ''' <summary>
    ''' Number of AIC parameters
    ''' </summary>
    Public nAICPars As Integer

    ''' <summary>
    ''' Number of AIC data points
    ''' </summary>
    Public nAICData As Integer

    ''' <summary>
    ''' Akaike Information Criteria for the last run
    ''' </summary>
    Public AIC As Single

    Public UseDefaultV As Boolean = True

End Class
