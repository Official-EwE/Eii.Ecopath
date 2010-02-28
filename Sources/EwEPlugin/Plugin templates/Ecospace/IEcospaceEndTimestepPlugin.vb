''' ---------------------------------------------------------------------------
''' <summary>
''' Interface for extending the Ecospace end time step logic. Plug-ins of this
''' type are invoked as soon as the EwE Core has finished its calculatios
''' of an Ecospace time step.
''' </summary>
''' ---------------------------------------------------------------------------
Public Interface IEcospaceEndTimestepPlugin
    Inherits IPlugin

    Sub EcospaceEndTimeStep(ByVal EcospaceDatastructures As Object, ByVal iTime As Integer)

End Interface


