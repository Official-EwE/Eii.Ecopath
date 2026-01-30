' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Common

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Template for searching Taxonomy data from external data sources.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Interface ITaxonSearchData

        ' -- Reference --

        ''' <summary>Data source that a taxon was obtained for.</summary>
        Property Source() As String
        ''' <summary>Key to update this taxonomy from the source.</summary>
        Property SourceKey() As String

        ''' <summary>Bitwise flag pattern indicating which fields to search, and
        ''' which fields have been searched.
        ''' </summary>
        Property SearchFields As eTaxonClassificationType

        ' -- Data fields --

        ''' <summary>Class name of a taxon.</summary>
        Property [Class]() As String
        ''' <summary>Order name of a taxon.</summary>
        Property Order() As String
        ''' <summary>Family name of a taxon.</summary>
        Property Family() As String
        ''' <summary>Genus name of a taxon.</summary>
        Property Genus() As String
        ''' <summary>Species name of a taxon.</summary>
        Property Species() As String
        ''' <summary>Common name of a taxon.</summary>
        Property Common() As String
        ''' <summary>Phylum of a taxon.</summary>
        Property Phylum() As String

        ' -- Identification --

        ''' <summary>FAO taxon code (http://www.fao.org/fishery/collection/asfis/en).</summary>
        Property CodeFAO() As String
        ''' <summary>FishBase SpecCode.</summary>
        Property CodeFishBase() As Long
        ''' <summary>Sea Life Base SpecCode.</summary>
        Property CodeSeaLifeBase As Long
        ''' <summary>Sea Around Us project Taxon ID</summary>
        Property CodeSAUP As Long
        ''' <summary>Taxonomy Databases Working Group Life Catalogue ID (http://lsid.tdwg.org/)</summary>
        Property CodeLSID As String
        ''' <summary>AquaMaps SpeciesID.</summary>
        Property CodeAquaMaps As String
        ''' <summary>WoRMS AphiaID.</summary>
        Property CodeAphia As String
        ''' <summary>OBIS taxonomy number.</summary>
        Property CodeOBIS As Long

        ' -- Spatial extent --

        ''' <summary>Northern limit of the bounding box where this taxon occurs.</summary>
        Property North() As Single
        ''' <summary>Southern limit of the bounding box where this taxon occurs.</summary>
        Property South() As Single
        ''' <summary>Eastern limit of the bounding box where this taxon occurs.</summary>
        Property East() As Single
        ''' <summary>Western limit of the bounding box where this taxon occurs.</summary>
        Property West() As Single

    End Interface

End Namespace
