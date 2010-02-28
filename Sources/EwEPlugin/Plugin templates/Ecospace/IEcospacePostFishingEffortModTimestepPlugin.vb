''' ---------------------------------------------------------------------------
''' <summary>
''' Interface for extending the Ecospace fishing effort logic. Plug-ins of this
''' type are invoked as soon as Ecospace fishing effort has been calculated.
''' </summary>
''' ---------------------------------------------------------------------------
Public Interface IEcospacePostFishingEffortModTimestepPlugin
    Inherits IPlugin

    Sub EcospacePostFishingEffortModTimestep(ByVal EcospaceDatastructures As Object, ByVal iTime As Integer)

End Interface
