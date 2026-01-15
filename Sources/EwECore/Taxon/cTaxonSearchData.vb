' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 3 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see https://www.gnu.org/licenses/gpl-3.0.html>. 
'
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

Imports EwECore.Common
Imports EwEUtils.Utilities

''' ---------------------------------------------------------------------------
''' <summary>
''' Container for transferring Taxonomy data
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cTaxonSearchData
    Implements ITaxonSearchData
    Implements ITaxonDetailsData

#Region " Constructor "

    Public Sub New(strSource As String)
        Me.Source = strSource
        Me.Phylum = ""
        Me.Class = ""
        Me.Family = ""
        Me.Genus = ""
        Me.Order = ""
        Me.Species = ""
        Me.Common = ""
        Me.CodeAphia = ""
        Me.CodeAquaMaps = ""
        Me.CodeFAO = ""
        Me.CodeLSID = ""
    End Sub

    Public Sub New(iTaxon As Integer, data As cTaxonDataStructures)
        Me.New(data.TaxonSource(iTaxon))
        Me.Class = data.TaxonClass(iTaxon)
        Me.CodeAphia = data.TaxonCodeAphia(iTaxon)
        Me.CodeAquaMaps = data.TaxonCodeAquaMaps(iTaxon)
        Me.CodeFAO = data.TaxonCodeFAO(iTaxon)
        Me.CodeFishBase = data.TaxonCodeFB(iTaxon)
        Me.CodeLSID = data.TaxonCodeLSID(iTaxon)
        Me.CodeOBIS = data.TaxonCodeOBIS(iTaxon)
        Me.CodeSAUP = data.TaxonCodeSAUP(iTaxon)
        Me.CodeSeaLifeBase = data.TaxonCodeSLB(iTaxon)
        Me.Common = data.TaxonName(iTaxon)
        Me.East = data.TaxonEast(iTaxon)
        Me.EcologyType = data.TaxonEcologyType(iTaxon)
        Me.ExploitationStatus = data.TaxonExploitationStatus(iTaxon)
        Me.Family = data.TaxonFamily(iTaxon)
        Me.Genus = data.TaxonGenus(iTaxon)
        Me.IUCNConservationStatus = data.TaxonIUCNConservationStatus(iTaxon)
        Me.LastUpdated = data.TaxonLastUpdated(iTaxon)
        Me.MaxLength = data.TaxonMaxLength(iTaxon)
        Me.MeanLength = data.TaxonMeanLength(iTaxon)
        Me.MeanLifespan = data.TaxonMeanLifeSpan(iTaxon)
        Me.MeanWeight = data.TaxonMeanWeight(iTaxon)
        Me.North = data.TaxonNorth(iTaxon)
        Me.OccurrenceStatus = data.TaxonOccurrenceStatus(iTaxon)
        Me.Order = data.TaxonOrder(iTaxon)
        Me.OrganismType = data.TaxonOrganismType(iTaxon)
        Me.SourceKey = data.TaxonSourceKey(iTaxon)
        Me.South = data.TaxonSouth(iTaxon)
        Me.Species = data.TaxonSpecies(iTaxon)
        Me.vbgfK = 0
        Me.VulnerabilityIndex = data.TaxonVulnerabilityIndex(iTaxon)
        Me.West = data.TaxonWest(iTaxon)
        Me.Winf = data.TaxonWinf(iTaxon)
    End Sub

#End Region ' Constructor

#Region " Properties "

    ''' <inheritdocs cref="ITaxonSearchData.Phylum"/>
    Public Property Phylum() As String Implements ITaxonSearchData.Phylum

    ''' <inheritdocs cref="ITaxonSearchData.[Class]"/>
    Public Property [Class]() As String Implements ITaxonSearchData.Class

    ''' <inheritdocs cref="ITaxonSearchData.Common"/>
    Public Property Common() As String Implements ITaxonSearchData.Common

    ''' <inheritdocs cref="ITaxonSearchData.Family"/>
    Public Property Family() As String Implements ITaxonSearchData.Family

    ''' <inheritdocs cref="ITaxonSearchData.Genus"/>
    Public Property Genus() As String Implements ITaxonSearchData.Genus

    ''' <inheritdocs cref="ITaxonSearchData.Order"/>
    Public Property Order() As String Implements ITaxonSearchData.Order

    ''' <inheritdocs cref="ITaxonSearchData.Species"/>
    Public Property Species() As String Implements ITaxonSearchData.Species

    ''' <inheritdocs cref="ITaxonSearchData.CodeSAUP"/>
    Public Property CodeSAUP() As Long Implements ITaxonSearchData.CodeSAUP

    ''' <inheritdocs cref="ITaxonSearchData.CodeFishBase"/>
    Public Property CodeFishBase As Long Implements ITaxonSearchData.CodeFishBase

    ''' <inheritdocs cref="ITaxonSearchData.CodeSeaLifeBase"/>
    Public Property CodeSeaLifeBase As Long Implements ITaxonSearchData.CodeSeaLifeBase

    ''' <inheritdocs cref="ITaxonSearchData.CodeAquaMaps"/>
    Public Property CodeAquaMaps As String Implements ITaxonSearchData.CodeAquaMaps

    ''' <inheritdocs cref="ITaxonSearchData.CodeAphia"/>
    Public Property CodeAphia As String Implements ITaxonSearchData.CodeAphia

    ''' <inheritdocs cref="ITaxonSearchData.CodeOBIS"/>
    Public Property CodeOBIS As Long Implements ITaxonSearchData.CodeOBIS

    ''' <inheritdocs cref="ITaxonSearchData.CodeLSID"/>
    Public Property CodeLSID() As String Implements ITaxonSearchData.CodeLSID

    ''' <inheritdocs cref="ITaxonSearchData.CodeFAO"/>
    Public Property CodeFAO() As String Implements ITaxonSearchData.CodeFAO

    ''' <inheritdocs cref="ITaxonSearchData.Source"/>
    Public Property Source() As String Implements ITaxonSearchData.Source

    ''' <inheritdocs cref="ITaxonSearchData.SourceKey"/>
    Public Property SourceKey() As String Implements ITaxonSearchData.SourceKey

    Public Property SearchFields As eTaxonClassificationType Implements ITaxonSearchData.SearchFields

    ''' <inheritdocs cref="ITaxonSearchData.North"/>
    Public Property North() As Single = cCore.NULL_VALUE Implements ITaxonSearchData.North

    ''' <inheritdocs cref="ITaxonSearchData.South"/>
    Public Property South() As Single = cCore.NULL_VALUE Implements ITaxonSearchData.South

    ''' <inheritdocs cref="ITaxonSearchData.East"/>
    Public Property East() As Single = cCore.NULL_VALUE Implements ITaxonSearchData.East

    ''' <inheritdocs cref="ITaxonSearchData.West"/>
    Public Property West() As Single = cCore.NULL_VALUE Implements ITaxonSearchData.West

    ''' <inheritdocs cref="ITaxonDetailsData.EcologyType"/>
    Public Property EcologyType() As eEcologyTypes = eEcologyTypes.NotSet Implements ITaxonDetailsData.EcologyType

    ''' <inheritdocs cref="ITaxonDetailsData.IUCNConservationStatus"/>
    Public Property IUCNConservationStatus() As eIUCNConservationStatusTypes = eIUCNConservationStatusTypes.NotSet Implements ITaxonDetailsData.IUCNConservationStatus

    ''' <inheritdocs cref="ITaxonDetailsData.ExploitationStatus"/>
    Public Property ExploitationStatus() As eExploitationTypes = eExploitationTypes.NotSet Implements ITaxonDetailsData.ExploitationStatus

    ''' <inheritdocs cref="ITaxonDetailsData.LastUpdated"/>
    Public Property LastUpdated() As Double = cDateUtils.DateToJulian(Date.Now()) Implements ITaxonDetailsData.LastUpdated

    ''' <inheritdocs cref="ITaxonDetailsData.MaxLength"/>
    Public Property MaxLength() As Single = cCore.NULL_VALUE Implements ITaxonDetailsData.MaxLength

    ''' <inheritdocs cref="ITaxonDetailsData.MeanLength"/>
    Public Property MeanLength() As Single = cCore.NULL_VALUE Implements ITaxonDetailsData.MeanLength

    ''' <inheritdocs cref="ITaxonDetailsData.MeanLifespan"/>
    Public Property MeanLifespan() As Single = cCore.NULL_VALUE Implements ITaxonDetailsData.MeanLifespan

    ''' <inheritdocs cref="ITaxonDetailsData.MeanWeight"/>
    Public Property MeanWeight() As Single = cCore.NULL_VALUE Implements ITaxonDetailsData.MeanWeight

    ''' <inheritdocs cref="ITaxonDetailsData.OccurrenceStatus"/>
    Public Property OccurrenceStatus() As eOccurrenceStatusTypes = eOccurrenceStatusTypes.NotSet Implements ITaxonDetailsData.OccurrenceStatus

    ''' <inheritdocs cref="ITaxonDetailsData.OrganismType"/>
    Public Property OrganismType() As eOrganismTypes = eOrganismTypes.NotSet Implements ITaxonDetailsData.OrganismType

    ''' <inheritdocs cref="ITaxonDetailsData.VulnerabilityIndex"/>
    Public Property VulnerabilityIndex() As Integer = cCore.NULL_VALUE Implements ITaxonDetailsData.VulnerabilityIndex

    ''' <inheritdocs cref="ITaxonDetailsData.vbgfK"/>
    Public Property vbgfK As Single = cCore.NULL_VALUE Implements ITaxonDetailsData.vbgfK

    ''' <inheritdocs cref="ITaxonDetailsData.Winf"/>
    Public Property Winf As Single = cCore.NULL_VALUE Implements ITaxonDetailsData.Winf

#End Region ' Properties

End Class
