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

        ReadOnly Property [Class]() As String
        ReadOnly Property Order() As String
        ReadOnly Property Family() As String
        ReadOnly Property Genus() As String
        ReadOnly Property Species() As String
        ReadOnly Property Common() As String
        ReadOnly Property CodeISSCAAP() As String
        ReadOnly Property CodeTaxon() As String
        ReadOnly Property Code3A() As String
        ReadOnly Property Source() As String
        ReadOnly Property SourceKey() As String
        ''' <summary>Julian date when this Taxon was last updated.</summary>
        ReadOnly Property LastUpdated() As Single

    End Interface

End Namespace
