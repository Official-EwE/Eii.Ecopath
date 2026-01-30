' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Plugins.Search

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Interface for implementing plugin points that are invoked from the EwE
    ''' searches
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Interface ISearchPlugin

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Search has been initialized by the core.
        ''' </summary>
        ''' <param name="SearchDatastructures">cSearchDataStructures</param>
        ''' -----------------------------------------------------------------------
        Sub SearchInitialized(SearchDatastructures As Object)

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' The user selected minimization routine has made a call to the function 
        ''' being minimized.
        ''' </summary>
        ''' <param name="SearchDatastructures">cSearchDataStructures</param>
        ''' -----------------------------------------------------------------------
        Sub PostRunSearchResults(SearchDatastructures As Object)

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

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Search is completed.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Sub SearchCompleted(SearchDatastructures As Object)

    End Interface

End Namespace