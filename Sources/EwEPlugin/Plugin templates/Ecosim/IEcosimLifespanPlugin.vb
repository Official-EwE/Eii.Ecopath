Option Strict On

''' ---------------------------------------------------------------------------
''' <summary>
''' Interface for extending the logic of closing an Ecosim scenario.
''' </summary>
''' ---------------------------------------------------------------------------
Public Interface IEcosimLifespanPlugin
    Inherits IEcosimPlugin

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Ecosim is closed.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Sub ClosedEcosimScenario()

End Interface
