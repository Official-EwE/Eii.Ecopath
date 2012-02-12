' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 2 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see <http://www.gnu.org/licenses/gpl-2.0.html>. 
'
' Copyright 1991-2012 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'
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
        Property Winf() As Single
        ''' <summary>Weight asymptotic</summary>
        Property vbgfK() As Single
        ''' <summary>Von Bertalanffy Curvature parameter, K in taxon table</summary>
        Property LastUpdated() As Double

    End Interface

End Namespace
