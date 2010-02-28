''' ---------------------------------------------------------------------------
''' <summary>
''' Interface for extending the Ecospace end time step logic. Plug-ins of this
''' type are invoked as soon as the EwE Core has finished its calculatios
''' of an Ecospace time step, and after all IEcospaceBeginTimestepPlugin points
''' have been called.
''' </summary>
''' ---------------------------------------------------------------------------
Public Interface IEcospaceBeginTimestepPostPlugin
    Inherits IPlugin

    Sub EcospaceBeginTimeStepPost(ByVal EcospaceDatastructures As Object, ByVal iTime As Integer)

End Interface
