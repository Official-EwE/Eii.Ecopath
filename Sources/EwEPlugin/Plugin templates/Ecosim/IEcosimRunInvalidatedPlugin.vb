Option Strict On

''' ---------------------------------------------------------------------------
''' <summary>
''' Interface for implementing a plugin point that is automatically invoked when
''' an Ecosim run is invalidated. This happens when an user input causes
''' the current Ecosim results to become invalid, or when an Ecospace scenario
''' is closed.
''' </summary>
''' ---------------------------------------------------------------------------
Public Interface IEcosimRunInvalidatedPlugin
    Inherits IPlugin

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Execute an Ecosim Run Invalidated plug-in.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Sub EcosimRunInvalidated()

End Interface
