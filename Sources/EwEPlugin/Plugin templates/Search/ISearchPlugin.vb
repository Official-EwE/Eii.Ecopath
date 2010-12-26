Option Strict On

''' ---------------------------------------------------------------------------
''' <summary>
''' Interface for implementing plugin points that are invoked from the EwE
''' searches
''' </summary>
''' ---------------------------------------------------------------------------
Public Interface ISearchPlugin

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Fishing Policy search has been initialized by the core.
    ''' </summary>
    ''' <param name="SearchDatastructures">cSearchDataStructures</param>
    ''' -----------------------------------------------------------------------
    Sub SearchInitialized(ByVal SearchDatastructures As Object)

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' The user selected minimization routine has made a call to the function 
    ''' being minimized.
    ''' </summary>
    ''' <param name="SearchDatastructures">cSearchDataStructures</param>
    ''' -----------------------------------------------------------------------
    Sub PostRunSearchResults(ByVal SearchDatastructures As Object)

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Search iteration are about to start.
    ''' </summary>
    ''' <remarks>
    ''' The minimization is about to run for the user selected number of 
    ''' iteration. 
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Sub SearchIterationsStarting()

End Interface
