#Region " Imports "

Option Strict On

#End Region ' Imports

Namespace Core

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Template for exchanging Taxonomy data.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Interface ITaxonData

        ''' <summary>Class name.</summary>
        Property [Class]() As String
        Property Order() As String
        Property Family() As String
        Property Genus() As String
        Property Species() As String
        Property Common() As String
        Property CodeISSCAAP() As String
        Property CodeTaxon() As String
        Property Code3A() As String
        Property Source() As String
        Property SourceKey() As String
        ''' <summary>Julian date when this Taxon was last updated.</summary>
        Property LastUpdated() As Single

    End Interface

End Namespace
