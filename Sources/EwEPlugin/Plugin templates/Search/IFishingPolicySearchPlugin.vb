'==============================================================================
'
' $Log: IFishingPolicySearchPlugin.vb,v $
' Revision 1.2  2009/01/31 00:57:45  joeb
' Added Plugin points to FPS
'
' Revision 1.1  2009/01/30 23:11:45  joeb
' Rename ISearchPlugin to IFishingPolicySearchPlugin
'
' Revision 1.1  2008/12/08 16:43:20  jeroens
' Initial version
'
'==============================================================================

Option Strict On

''' ---------------------------------------------------------------------------
''' <summary>
''' Interface for implementing plugin points that are invoked from the EwE
''' searches
''' </summary>
''' ---------------------------------------------------------------------------
Public Interface IFishingPolicySearchPlugin

    ''' <summary>
    ''' Fishing Policy search has been initialized by the core
    ''' </summary>
    ''' <param name="SearchDatastructures"></param>
    ''' <remarks></remarks>
    Sub SearchInitialized(ByVal SearchDatastructures As Object)

    ''' <summary>
    ''' The user selected minimization routine has made a call to the function being minimized (Ecosim.Run())<see>cFishingPolicySearch.FUNC()</see>
    ''' </summary>
    ''' <param name="SearchDatastructures">cSearchDataStructures as Object</param>
    ''' <remarks></remarks>
    Sub SearchFunctionCall(ByVal SearchDatastructures As Object)

    ''' <summary>
    ''' Search iteration are about to start
    ''' </summary>
    ''' <remarks>The minimization is about to run for the user selected number of iteration. <see>cFishingPolicySearch.RunSearch()</see> </remarks>
    Sub SearchIterationsStarting()

End Interface
