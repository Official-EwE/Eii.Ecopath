Option Strict On

''' ---------------------------------------------------------------------------
''' <summary>
''' Interface for implementing a plugin point that is automatically invoked when
''' an Ecospace run is invalidated. This happens when an user input causes
''' the current Ecospace results to become invalid, or when an Ecospace scenario
''' is closed.
''' </summary>
''' ---------------------------------------------------------------------------
Public Interface IEcospaceRunInvalidatedPlugin
    Inherits IPlugin

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Execute an Ecospace Run Invalidated plug-in.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Sub EcospaceRunInvalidated()

End Interface
