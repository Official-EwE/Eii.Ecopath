Option Strict On

''' ---------------------------------------------------------------------------
''' <summary>
''' Interface for implementing a plugin point that is invoked whenever an EwE
''' Ecopath model has been closed.
''' </summary>
''' ---------------------------------------------------------------------------
Public Interface IEcopathClosedPlugin
    Inherits IEcopathPlugin

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Execution interface for an Ecopath close model plugin point.
    ''' </summary>
    ''' <returns>True if closed succesful.</returns>
    ''' -----------------------------------------------------------------------
    Function CloseModel() As Boolean

End Interface
