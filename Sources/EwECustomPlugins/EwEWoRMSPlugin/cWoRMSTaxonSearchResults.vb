' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.Common
Imports EwECore.Plugins.Data



''' ---------------------------------------------------------------------------
''' <summary>
''' Implementation of the SAUP Taxon search results class.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cWoRMSTaxonSearchResults
    Implements IDataSearchResults

#Region " Private bits "

    ''' <summary>Term used to start the search.</summary>
    Private m_term As ITaxonSearchData = Nothing
    ''' <summary>
    ''' Taxa returned in response to the search.
    ''' </summary>
    Private m_taxa As ITaxonSearchData() = Nothing
    ''' <summary>Mandatory bit: plug-in name that returned the search results.</summary>
    Private m_strPluginName As String = ""

#End Region ' Private bits

    Public Sub New(term As ITaxonSearchData,
                   results As ITaxonSearchData(),
                   strPluginName As String)
        Me.m_term = term
        Me.m_taxa = results
        Me.m_strPluginName = strPluginName
    End Sub

    ''' <inheritdoc cref="IDataSearchResults.SearchResults"/>
    Public ReadOnly Property SearchResults() As Object() _
        Implements IDataSearchResults.SearchResults
        Get
            Return Me.m_taxa
        End Get
    End Property

    ''' <inheritdoc cref="IDataSearchResults.SearchScores"/>
    Public ReadOnly Property SearchScores() As Single() _
        Implements IDataSearchResults.SearchScores
        Get
            Dim asScores(Me.m_taxa.Length) As Single
            Return asScores
        End Get
    End Property

    ''' <inheritdoc cref="IDataSearchResults.SearchTerm"/>
    Public ReadOnly Property SearchTerm() As Object _
        Implements IDataSearchResults.SearchTerm
        Get
            Return Me.m_term
        End Get
    End Property

    ''' <inheritdoc cref="IDataSearchResults.PluginName"/>
    Public ReadOnly Property PluginName() As String _
        Implements IPluginData.PluginName
        Get
            Return Me.m_strPluginName
        End Get
    End Property

    ''' <inheritdoc cref="IDataSearchResults.RunType"/>
    Public ReadOnly Property RunType() As IRunType _
        Implements IPluginData.RunType
        Get
            Return Nothing
        End Get
    End Property

End Class
