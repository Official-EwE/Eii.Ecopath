#Region " Imports "

Option Strict On
Imports EwEUtils.Core

#End Region ' Imports

Namespace Data

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Interface for building a container for plug-in search results provided
    ''' by <see cref="IDataSearchProducerPlugin">data search plug-ins.</see>
    ''' </summary>
    ''' -------------------------------------------------------------------
    Public Interface IDataSearchResults
        Inherits IPluginData

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the term that was used to obtain these results.
        ''' </summary>
        ''' -------------------------------------------------------------------
        ReadOnly Property SearchTerm() As Object

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get an array of search results that matched the term.
        ''' </summary>
        ''' -------------------------------------------------------------------
        ReadOnly Property SearchResults() As Object()

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get an array of score results for the matches.
        ''' </summary>
        ''' -------------------------------------------------------------------
        ReadOnly Property SearchScores() As Single()

    End Interface

End Namespace
