' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Common

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Template for exchanging Taxonomy details data.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Interface ITaxonDetailsData
        Inherits ITaxonSearchData

        ''' <summary>
        ''' Get/set the <see cref="eEcologyTypes"/> for a taxon.
        ''' </summary>
        Property EcologyType() As eEcologyTypes
        ''' <summary>
        ''' Get/set the <see cref="eOrganismTypes"/> for a taxon.
        ''' </summary>
        Property OrganismType() As eOrganismTypes
        ''' <summary>
        ''' Get/set the <see cref="eIUCNConservationStatusTypes"/> for a taxon.
        ''' </summary>
        Property IUCNConservationStatus() As eIUCNConservationStatusTypes
        ''' <summary>
        ''' Get/set the <see cref="eExploitationTypes"/> for a taxon.
        ''' </summary>
        Property ExploitationStatus() As eExploitationTypes
        ''' <summary>
        ''' Get/set the <see cref="eOccurrenceStatusTypes"/> for a taxon.
        ''' </summary>
        Property OccurrenceStatus() As eOccurrenceStatusTypes
        ''' <summary>
        ''' Get/set the mean weight for a taxon.
        ''' </summary>
        Property MeanWeight() As Single
        ''' <summary>
        ''' Get/set the mean life span for a taxon.
        ''' </summary>
        Property MeanLifespan() As Single
        ''' <summary>
        ''' Get/set the mean length for a taxon.
        ''' </summary>
        Property MeanLength() As Single
        ''' <summary>
        ''' Get/set the max length for a taxon.
        ''' </summary>
        Property MaxLength() As Single
        ''' <summary>
        ''' Get/set the vulnerability index for a taxon.
        ''' </summary>
        Property VulnerabilityIndex() As Integer
        ''' <summary>
        ''' Get/set the asymptotic weight for a taxon.
        ''' </summary>
        Property Winf() As Single
        ''' <summary>
        ''' Get/set the asymptotic weight for a taxon.
        ''' </summary>
        Property vbgfK() As Single
        ''' <summary>Julian date when record was last updated.</summary>
        Property LastUpdated() As Double

    End Interface

End Namespace
