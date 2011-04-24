#Region " Imports "

Option Strict On
Imports EwEUtils.Core

#End Region ' Imports

Namespace Core

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Template for exchanging Taxonomy details data.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Interface ITaxonDetailsData
        Inherits ITaxonSearchData

        ''' <summary></summary>
        Property EcologyType() As eEcologyTypes
        ''' <summary></summary>
        Property OrganismType() As eOrganismTypes
        ''' <summary></summary>
        Property IUCNConservationStatus() As eIUCNConservationStatusTypes
        ''' <summary></summary>
        Property OccurrenceStatus() As eOccurrenceStatusTypes
        ''' <summary></summary>
        Property MeanWeight() As Single
        ''' <summary></summary>
        Property MeanLifespan() As Single
        ''' <summary></summary>
        Property MeanLength() As Single
        ''' <summary></summary>
        Property MaxLength() As Single
        ''' <summary></summary>
        Property VulnerabilityIndex() As Integer
        ''' <summary></summary>
        Property LastUpdated() As Double

    End Interface

End Namespace
