Option Strict On

''' ---------------------------------------------------------------------------
''' <summary>
''' Interface for implementing a plugin point that is automatically invoked when
''' an Ecospace run is invalidated. This happens when an user input causes
''' the current Ecopath results to become invalid, or when a model is closed.
''' </summary>
''' ---------------------------------------------------------------------------
Public Interface IEcopathRunInvalidatedPlugin
    Inherits IPlugin

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Execute an Ecopath Run Invalidated plug-in.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Sub EcopathRunInvalidated()

End Interface
