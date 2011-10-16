''' ---------------------------------------------------------------------------
''' <summary>
''' Ecotracer post-initialization plug-in
''' </summary>
''' ---------------------------------------------------------------------------
Public Interface IEcotracerInitializedPlugin
    Inherits IPlugin

    Sub EcotracerInitialized(ByVal ContaminantTracerDatastructures As Object)

End Interface
