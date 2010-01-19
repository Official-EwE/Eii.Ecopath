#Region " Imports "

Option Strict On
Imports System.Collections.Generic

#End Region ' Imports

Public Interface IResultView

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Show results for all fleets or for an inidividual fleet.
    ''' </summary>
    ''' <param name="iFleet"></param>
    ''' <param name="lUnits"></param>
    ''' <param name="result"></param>
    ''' -----------------------------------------------------------------------
    Sub ShowResults(ByVal iFleet As Integer, ByVal lUnits As List(Of cUnit), ByVal result As cResults)

End Interface
