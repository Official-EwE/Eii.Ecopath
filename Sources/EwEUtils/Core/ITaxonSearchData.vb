#Region " Imports "

Option Strict On
Imports EwEUtils.Core
Imports System

#End Region ' Imports

Namespace Core

    ''' <summary>Enumerated type, identifying taxonomy searchable fields.</summary>
    Public Enum eTaxonLevelType As Long
        Any = Common Or Species Or Genus Or Family Or Order Or [Class] Or Phylum
        Common = &H1
        Species = &H10
        Genus = &H100
        Family = &H1000
        Order = &H10000
        [Class] = &H100000
        Phylum = &H1000000
        <Obsolete("Kingdom not supported yet but added for future use")> _
        Kingdom = &H10000000
    End Enum

    ''' <summary>Type 
    ''' 
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum eTaxonCodeType As Integer
        FAOTaxon
        FishBase
        SeaLifeBase
        SeaAroundUs
        TDWG_LCID
    End Enum
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
        Property SearchFields As eTaxonLevelType

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
        Property CodeFB() As Long
        ''' <summary>Sea Life Base SpecCode.</summary>
        Property CodeSLB As Long
        ''' <summary>Sea Around Us project Taxon ID</summary>
        Property CodeSAUP As Long
        ''' <summary>Taxonomy Databases Working Group Life Catalogue ID (http://lsid.tdwg.org/)</summary>
        Property CodeLSID As String

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
