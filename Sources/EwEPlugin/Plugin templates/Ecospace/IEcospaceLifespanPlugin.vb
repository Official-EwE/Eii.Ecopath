Option Strict On

''' ---------------------------------------------------------------------------
''' <summary>
''' Interface for extending the logic of closing an Ecospace scenario.
''' </summary>
''' ---------------------------------------------------------------------------
Public Interface IEcospaceLifespanPlugin
    Inherits IEcospacePlugin

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Ecospace scenario is closed.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Sub CloseEcospaceScenario()

End Interface
