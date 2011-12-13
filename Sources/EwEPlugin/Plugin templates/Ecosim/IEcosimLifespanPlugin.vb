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
    ''' Datasource load ecosim scenario plugin point.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Sub CloseEcosimScenario()

End Interface
