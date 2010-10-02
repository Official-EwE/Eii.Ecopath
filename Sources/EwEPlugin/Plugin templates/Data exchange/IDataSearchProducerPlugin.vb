#Region " Imports "

Option Strict On
Imports System
Imports EwEUtils.Core

#End Region ' Imports

Namespace Data

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Plug-in interface for data providers that allow data to be searched.
    ''' </summary>
    ''' -------------------------------------------------------------------
    Public Interface IDataSearchProducerPlugin
        Inherits IDataProducerPlugin

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Starts an asynchronous search for data.
        ''' </summary>
        ''' <param name="data">The data providing search terms.</param>
        ''' <returns>True if started succesful.</returns>
        ''' -------------------------------------------------------------------
        Function StartSearch(ByVal data As Object) As Boolean

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Interrupt a current search.
        ''' </summary>
        ''' <returns>True if stopped succesfully.</returns>
        ''' -------------------------------------------------------------------
        Function StopSearch() As Boolean

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Queries a data producer if a search is in progress.
        ''' </summary>
        ''' <returns>True if a search is in progress.</returns>
        ''' -------------------------------------------------------------------
        Function IsSeaching() As Boolean

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns search results.
        ''' </summary>
        ''' <param name="dataTerm">The search term that was used.</param>
        ''' <param name="results">Returned search results.</param>
        ''' <returns>True if succesful.</returns>
        ''' -------------------------------------------------------------------
        Function SearchResults(ByVal dataTerm As Object, ByRef results As IDataSearchResults) As Boolean

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns a search term for an interface to substitute data into.
        ''' </summary>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Function CreateSearchTerm() As ITaxonData

    End Interface

End Namespace
