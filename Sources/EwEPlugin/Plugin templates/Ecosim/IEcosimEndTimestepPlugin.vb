Option Strict On

''' ===========================================================================
''' <summary>
''' Plugin points for the end of an Ecosim time step.
''' </summary>
''' ===========================================================================
Public Interface IEcosimEndTimestepPlugin
    Inherits IPlugin

    Sub EcosimEndTimeStep(ByRef BiomassAtTimestep() As Single, ByVal EcosimDatastructures As Object, ByVal iTime As Integer, ByVal Ecosimresults As Object)

End Interface


