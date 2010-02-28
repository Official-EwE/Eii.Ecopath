''' ---------------------------------------------------------------------------
''' <summary>
''' Interface for extending the Ecospace initialzation logic. Plug-ins of this
''' type are invoked as soon as all Ecospace data is loaded in the EwE Core.
''' </summary>
''' ---------------------------------------------------------------------------
Public Interface IEcospaceInitializedPlugin
    Inherits IPlugin

    Sub EcospaceInitialized(ByVal EcospaceDatastructures As Object)

End Interface
