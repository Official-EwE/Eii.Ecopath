''' ---------------------------------------------------------------------------
''' <summary>
''' Interface for extending the Ecospace begin time step logic. Plug-ins of this
''' type are invoked as soon as the EwE Core is about to begin its calculatios
''' of an Ecospace time step.
''' </summary>
''' ---------------------------------------------------------------------------
Public Interface IEcospaceBeginTimestepPlugin
    Inherits IPlugin

    Sub EcospaceBeginTimeStep(ByVal EcospaceDatastructures As Object, ByVal iTime As Integer)

End Interface
