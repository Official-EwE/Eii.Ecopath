#Region " Imports "

Option Strict On
Imports EwEUtils.Core

#End Region ' Imports

Namespace Core

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Template for exchanging Taxonomy detailed data.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Interface ITaxonDetailsData
        Inherits ITaxonSearchData

        ' ToDo: add diet, biomass, etc

        ''' <summary>Julian date when this taxon was last updated.</summary>
        Property LastUpdated() As Double

    End Interface

End Namespace
