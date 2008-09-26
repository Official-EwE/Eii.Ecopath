'==============================================================================
'
' $Log: cF2TSDataStructures.vb,v $
' Revision 1.1  2008/09/26 07:30:25  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.8  2007/11/13 23:38:41  jeroens
' * Set defaults
'
' Revision 1.7  2007/11/01 18:07:47  joeb
' Added bAnomalySearch
'
' Revision 1.6  2007/10/29 15:21:54  jeroens
' - Discontinued dead logic
'
' Revision 1.5  2007/09/24 14:20:00  joeb
' Implementation of fitting
'
' Revision 1.4  2007/09/07 19:57:46  jeroens
' * Split Variance var in 2
'
' Revision 1.3  2007/08/25 15:01:53  jeroens
' * Morphing, morphing
'
' Revision 1.2  2007/08/24 19:16:32  jeroens
' * Working
'
'==============================================================================

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

End Class
