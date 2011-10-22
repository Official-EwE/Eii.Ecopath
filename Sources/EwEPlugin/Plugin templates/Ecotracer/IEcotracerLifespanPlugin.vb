Option Strict On

''' ---------------------------------------------------------------------------
''' <summary>
''' Interface for extending the logic of closing an Ecotracer scenario.
''' </summary>
''' ---------------------------------------------------------------------------
Public Interface IEcotracerLifespanPlugin
    Inherits IEcotracerPlugin

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Ecotracer scenario is closed.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Sub ClosedEcospaceScenario()

End Interface
