
Option Strict On

Public Class cF2TSDataStructures

    Public bVulnerabilitySearch As Boolean = True
    Public bCatchAnomaly As Boolean = False
    Public bAnomalySearch As Boolean = False
    Public FirstYear As Integer = 1
    Public LastYear As Integer = 1
    Public VulnerabilityVariance As Single = 10.0!
    Public PPVariance As Single = 0.1!
    Public iCatchAnomalySearchShapeNumber As Integer = 0
    Public nNumSplinePoints As Integer = 0

    ''' <summary>
    ''' Number of AIC parameters
    ''' </summary>
    ''' <remarks></remarks>
    Public nAICPars As Integer

    ''' <summary>
    ''' Number of AIC data points
    ''' </summary>
    ''' <remarks></remarks>
    Public nAICData As Integer

    ''' <summary>
    ''' Akaike Information Criteria for the last run
    ''' </summary>
    ''' <remarks></remarks>
    Public AIC As Single

    Public UseDefaultV As Boolean = True

End Class
