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
' Copyright 1991- UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports EwEUtils.Core
Imports EwEUtils.Utilities

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Container for transferring Taxonomy data
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cTaxonSearchData
    Implements ITaxonSearchData
    Implements ITaxonDetailsData

#Region " Privates "

    Private m_strSource As String = ""
    Private m_strSourceKey As String = ""
    Private m_searchFields As eTaxonLevelType = eTaxonLevelType.Any

    ' -- ID --
    Private m_lCodeSAUP As Long = 0
    Private m_lCodeFB As Long = 0
    Private m_lCodeSLB As Long = 0
    Private m_strCodeLSID As String = ""
    Private m_strCodeFAO As String = ""

    ' -- Data --
    Private m_strCommon As String = ""
    Private m_strPhylum As String = ""
    Private m_strClass As String = ""
    Private m_strOrder As String = ""
    Private m_strFamily As String = ""
    Private m_strGenus As String = ""
    Private m_strSpecies As String = ""
    Private m_sNorth As Single = cCore.NULL_VALUE
    Private m_sSouth As Single = cCore.NULL_VALUE
    Private m_sEast As Single = cCore.NULL_VALUE
    Private m_sWest As Single = cCore.NULL_VALUE
    Private m_bExploited As Boolean = False
    Private m_ecology As eEcologyTypes = eEcologyTypes.NotSet
    Private m_conservation As eIUCNConservationStatusTypes = eIUCNConservationStatusTypes.NotSet
    Private m_exploitation As eExploitationTypes = eExploitationTypes.NotSet
    Private m_occurrence As eOccurrenceStatusTypes = eOccurrenceStatusTypes.NotSet
    Private m_organism As eOrganismTypes = eOrganismTypes.Fishes
    Private m_sLastUpdated As Double = cDateUtils.DateToJulian(Date.Now())
    Private m_sMaxLength As Single = cCore.NULL_VALUE
    Private m_sMeanLength As Single = cCore.NULL_VALUE
    Private m_sMeanLifespan As Single = cCore.NULL_VALUE
    Private m_sMeanWeight As Single = cCore.NULL_VALUE
    Private m_iVulnerabilityIndex As Integer = cCore.NULL_VALUE
    ' Growth
    Private m_vbgkf As Single = cCore.NULL_VALUE
    Private m_sWinf As Single = cCore.NULL_VALUE

#End Region ' Privates

#Region " Constructor "

    Public Sub New(strSource As String)
        Me.m_strSource = strSource
    End Sub

#End Region ' Constructor

#Region " Properties "

    ''' <inheritdocs cref="ITaxonSearchData.Phylum"/>
    Public Property Phylum() As String _
        Implements ITaxonSearchData.Phylum
        Get
            Return m_strPhylum
        End Get
        Set(ByVal value As String)
            Me.m_strPhylum = value
        End Set
    End Property

    ''' <inheritdocs cref="ITaxonSearchData.[Class]"/>
    Public Property [Class]() As String _
        Implements ITaxonSearchData.Class
        Get
            Return Me.m_strClass
        End Get
        Set(ByVal value As String)
            Me.m_strClass = value
        End Set
    End Property

    ''' <inheritdocs cref="ITaxonSearchData.Common"/>
    Public Property Common() As String _
        Implements ITaxonSearchData.Common
        Get
            Return Me.m_strCommon
        End Get
        Set(ByVal value As String)
            Me.m_strCommon = value
        End Set
    End Property

    ''' <inheritdocs cref="ITaxonSearchData.Family"/>
    Public Property Family() As String _
        Implements ITaxonSearchData.Family
        Get
            Return Me.m_strFamily
        End Get
        Set(ByVal value As String)
            Me.m_strFamily = value
        End Set
    End Property

    ''' <inheritdocs cref="ITaxonSearchData.Genus"/>
    Public Property Genus() As String _
        Implements ITaxonSearchData.Genus
        Get
            Return Me.m_strGenus
        End Get
        Set(ByVal value As String)
            Me.m_strGenus = value
        End Set
    End Property

    ''' <inheritdocs cref="ITaxonSearchData.Order"/>
    Public Property Order() As String _
        Implements ITaxonSearchData.Order
        Get
            Return Me.m_strOrder
        End Get
        Set(ByVal value As String)
            Me.m_strOrder = value
        End Set
    End Property

    ''' <inheritdocs cref="ITaxonSearchData.Species"/>
    Public Property Species() As String _
        Implements ITaxonSearchData.Species
        Get
            Return Me.m_strSpecies
        End Get
        Set(ByVal value As String)
            Me.m_strSpecies = value
        End Set
    End Property

    ''' <inheritdocs cref="ITaxonSearchData.CodeSAUP"/>
    Public Property CodeSAUP() As Long _
        Implements ITaxonSearchData.CodeSAUP
        Get
            Return Me.m_lCodeSAUP
        End Get
        Set(ByVal value As Long)
            Me.m_lCodeSAUP = value
        End Set
    End Property

    ''' <inheritdocs cref="ITaxonSearchData.CodeFB"/>
    Public Property CodeFB As Long _
        Implements ITaxonSearchData.CodeFB
        Get
            Return Me.m_lCodeFB
        End Get
        Set(value As Long)
            Me.m_lCodeFB = value
        End Set
    End Property

    ''' <inheritdocs cref="ITaxonSearchData.CodeSLB"/>
    Public Property CodeSLB As Long _
        Implements ITaxonSearchData.CodeSLB
        Get
            Return Me.m_lCodeSLB
        End Get
        Set(value As Long)
            Me.m_lCodeSLB = value
        End Set
    End Property

    ''' <inheritdocs cref="ITaxonSearchData.CodeLSID"/>
    Public Property CodeLSID() As String _
        Implements ITaxonSearchData.CodeLSID
        Get
            Return Me.m_strCodeLSID
        End Get
        Set(ByVal value As String)
            Me.m_strCodeLSID = value
        End Set
    End Property

    ''' <inheritdocs cref="ITaxonSearchData.CodeFAO"/>
    Public Property CodeFAO() As String _
        Implements ITaxonSearchData.CodeFAO
        Get
            ' Rerouted to source key
            Return Me.m_strCodeFAO
        End Get
        Set(ByVal value As String)
            ' Rerouted to source key
            Me.m_strCodeFAO = value
        End Set
    End Property

    ''' <inheritdocs cref="ITaxonSearchData.Source"/>
    Public Property Source() As String _
        Implements ITaxonSearchData.Source
        Get
            Return Me.m_strSource
        End Get
        Set(ByVal value As String)
            ' NOP
        End Set
    End Property

    ''' <inheritdocs cref="ITaxonSearchData.SourceKey"/>
    Public Property SourceKey() As String _
        Implements ITaxonSearchData.SourceKey
        Get
            Return Me.m_strSourceKey
        End Get
        Set(ByVal value As String)
            Me.m_strSourceKey = value
        End Set
    End Property

    Public Property SearchFields As eTaxonLevelType _
        Implements ITaxonSearchData.SearchFields
        Get
            Return Me.m_searchFields
        End Get
        Set(value As eTaxonLevelType)
            Me.m_searchFields = value
        End Set
    End Property

    ''' <inheritdocs cref="ITaxonSearchData.North"/>
    Public Property North() As Single _
        Implements ITaxonSearchData.North
        Get
            Return Me.m_sNorth
        End Get
        Set(ByVal value As Single)
            Me.m_sNorth = value
        End Set
    End Property

    ''' <inheritdocs cref="ITaxonSearchData.South"/>
    Public Property South() As Single _
        Implements ITaxonSearchData.South
        Get
            Return Me.m_sSouth
        End Get
        Set(ByVal value As Single)
            Me.m_sSouth = value
        End Set
    End Property

    ''' <inheritdocs cref="ITaxonSearchData.East"/>
    Public Property East() As Single _
        Implements ITaxonSearchData.East
        Get
            Return Me.m_sEast
        End Get
        Set(ByVal value As Single)
            Me.m_sEast = value
        End Set
    End Property

    ''' <inheritdocs cref="ITaxonSearchData.West"/>
    Public Property West() As Single _
        Implements ITaxonSearchData.West
        Get
            Return Me.m_sWest
        End Get
        Set(ByVal value As Single)
            Me.m_sWest = value
        End Set
    End Property

    ''' <inheritdocs cref="ITaxonDetailsData.EcologyType"/>
    Public Property EcologyType() As eEcologyTypes _
        Implements ITaxonDetailsData.EcologyType
        Get
            Return Me.m_ecology
        End Get
        Set(ByVal value As eEcologyTypes)
            Me.m_ecology = value
        End Set
    End Property

    ''' <inheritdocs cref="ITaxonDetailsData.IUCNConservationStatus"/>
    Public Property IUCNConservationStatus() As eIUCNConservationStatusTypes _
        Implements ITaxonDetailsData.IUCNConservationStatus
        Get
            Return Me.m_conservation
        End Get
        Set(ByVal value As eIUCNConservationStatusTypes)
            Me.m_conservation = value
        End Set
    End Property

    ''' <inheritdocs cref="ITaxonDetailsData.ExploitationStatus"/>
    Public Property ExploitationStatus() As eExploitationTypes _
        Implements ITaxonDetailsData.ExploitationStatus
        Get
            Return Me.m_exploitation
        End Get
        Set(ByVal value As eExploitationTypes)
            Me.m_exploitation = value
        End Set
    End Property

    ''' <inheritdocs cref="ITaxonDetailsData.LastUpdated"/>
    Public Property LastUpdated() As Double _
        Implements ITaxonDetailsData.LastUpdated
        Get
            Return Me.m_sLastUpdated
        End Get
        Set(ByVal value As Double)
            Me.m_sLastUpdated = value
        End Set
    End Property

    ''' <inheritdocs cref="ITaxonDetailsData.MaxLength"/>
    Public Property MaxLength() As Single _
        Implements ITaxonDetailsData.MaxLength
        Get
            Return Me.m_sMaxLength
        End Get
        Set(ByVal value As Single)
            Me.m_sMaxLength = value
        End Set
    End Property

    ''' <inheritdocs cref="ITaxonDetailsData.MeanLength"/>
    Public Property MeanLength() As Single _
        Implements ITaxonDetailsData.MeanLength
        Get
            Return Me.m_sMeanLength
        End Get
        Set(ByVal value As Single)
            Me.m_sMeanLength = value
        End Set
    End Property

    ''' <inheritdocs cref="ITaxonDetailsData.MeanLifespan"/>
    Public Property MeanLifespan() As Single _
        Implements ITaxonDetailsData.MeanLifespan
        Get
            Return Me.m_sMeanLifespan
        End Get
        Set(ByVal value As Single)
            Me.m_sMeanLifespan = value
        End Set
    End Property

    ''' <inheritdocs cref="ITaxonDetailsData.MeanWeight"/>
    Public Property MeanWeight() As Single _
        Implements ITaxonDetailsData.MeanWeight
        Get
            Return Me.m_sMeanWeight
        End Get
        Set(ByVal value As Single)
            Me.m_sMeanWeight = value
        End Set
    End Property

    ''' <inheritdocs cref="ITaxonDetailsData.OccurrenceStatus"/>
    Public Property OccurrenceStatus() As eOccurrenceStatusTypes _
        Implements ITaxonDetailsData.OccurrenceStatus
        Get
            Return Me.m_occurrence
        End Get
        Set(ByVal value As eOccurrenceStatusTypes)
            Me.m_occurrence = value
        End Set
    End Property

    ''' <inheritdocs cref="ITaxonDetailsData.OrganismType"/>
    Public Property OrganismType() As eOrganismTypes _
        Implements ITaxonDetailsData.OrganismType
        Get
            Return Me.m_organism
        End Get
        Set(ByVal value As eOrganismTypes)
            Me.m_organism = value
        End Set
    End Property

    ''' <inheritdocs cref="ITaxonDetailsData.VulnerabilityIndex"/>
    Public Property VulnerabilityIndex() As Integer _
        Implements ITaxonDetailsData.VulnerabilityIndex
        Get
            Return Me.m_iVulnerabilityIndex
        End Get
        Set(ByVal value As Integer)
            Me.m_iVulnerabilityIndex = value
        End Set
    End Property

#End Region ' Properties

    ''' <inheritdocs cref="ITaxonDetailsData.vbgfK"/>
    Public Property vbgfK As Single Implements EwEUtils.Core.ITaxonDetailsData.vbgfK
        Get
            Return Me.m_vbgkf
        End Get
        Set(value As Single)
            Me.m_vbgkf = value
        End Set
    End Property

    ''' <inheritdocs cref="ITaxonDetailsData.Winf"/>
    Public Property Winf As Single Implements EwEUtils.Core.ITaxonDetailsData.Winf
        Get
            Return Me.m_sWinf
        End Get
        Set(value As Single)
            Me.m_sWinf = value
        End Set
    End Property

End Class
