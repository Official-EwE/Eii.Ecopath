#Region " Imports "

Option Strict On
Imports EwEUtils.Core

#End Region ' Imports

Namespace Core

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Template for exchanging Taxonomy data.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Interface ITaxonData

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
        ''' <summary>ISSCAAP code of a taxon.</summary>
        ''' <remarks>http://www.fao.org/docrep/w7283e/W7283E07.htm</remarks>
        Property CodeISSCAAP() As String
        ''' <summary>Taxonomy code of a taxon.</summary>
        ''' <remarks>http://www.fao.org/fishery/collection/asfis/en</remarks>
        Property CodeTaxon() As String
        ''' <summary>3A code of a taxon.</summary>
        Property Code3A() As String
        ''' <summary>Data source that a taxon was obtained for.</summary>
        Property Source() As String
        ''' <summary>Key to update this taxonomy from the source.</summary>
        Property SourceKey() As String
        ''' <summary>Northern limit of the bounding box where this taxon occurs.</summary>
        Property North() As Single
        ''' <summary>Southern limit of the bounding box where this taxon occurs.</summary>
        Property South() As Single
        ''' <summary>Eastern limit of the bounding box where this taxon occurs.</summary>
        Property East() As Single
        ''' <summary>Western limit of the bounding box where this taxon occurs.</summary>
        Property West() As Single
        ''' <summary>Julian date when this taxon was last updated.</summary>
        Property LastUpdated() As Double

    End Interface

End Namespace
