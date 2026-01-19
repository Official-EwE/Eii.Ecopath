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
Imports EwECore.ValueWrapper

''' <summary>
''' Taxonomy definition that contributes to a functional group or stanza configuration.
''' </summary>
Public Class cTaxon
    Inherits cCoreInputOutputBase
    Implements ITaxonSearchData
    Implements ITaxonDetailsData

#Region " Construction and Intialization "

    Friend Sub New(core As cCore, DBID As Integer)
        MyBase.New(core)

        Dim val As cValue = Nothing
        Dim cbuf() As Char

        Me.AllowValidation = False

        Me.m_coreComponent = eCoreComponentType.Ecopath
        Me.m_dataType = eDataTypes.Taxon
        Me.DBID = DBID
        Me.m_ValidationStatus = New cVariableStatus(Me, eStatusFlags.OK, "", eVarNameFlags.NotSet)

        ' Taxon group
        val = New cValue(core, New Integer, eVarNameFlags.TaxonGroup, eStatusFlags.Null, eValueTypes.Int)
        Me.m_values.Add(val.varName, val)

        val = New cValue(core, New Integer, eVarNameFlags.TaxonStanza, eStatusFlags.Null, eValueTypes.Int)
        Me.m_values.Add(val.varName, val)

        val = New cValue(core, New String(cbuf), eVarNameFlags.Class, eStatusFlags.OK, eValueTypes.Str)
        Me.m_values.Add(val.varName, val)

        val = New cValue(core, New String(cbuf), eVarNameFlags.Phylum, eStatusFlags.OK, eValueTypes.Str)
        Me.m_values.Add(val.varName, val)

        val = New cValue(core, New String(cbuf), eVarNameFlags.Order, eStatusFlags.OK, eValueTypes.Str)
        Me.m_values.Add(val.varName, val)

        val = New cValue(core, New String(cbuf), eVarNameFlags.Family, eStatusFlags.OK, eValueTypes.Str)
        Me.m_values.Add(val.varName, val)

        val = New cValue(core, New String(cbuf), eVarNameFlags.Genus, eStatusFlags.OK, eValueTypes.Str)
        Me.m_values.Add(val.varName, val)

        val = New cValue(core, New String(cbuf), eVarNameFlags.Species, eStatusFlags.OK, eValueTypes.Str)
        Me.m_values.Add(val.varName, val)

        val = New cValue(core, New Integer, eVarNameFlags.CodeSAUP, eStatusFlags.OK, eValueTypes.Int)
        Me.m_values.Add(val.varName, val)

        val = New cValue(core, New Integer, eVarNameFlags.CodeFB, eStatusFlags.OK, eValueTypes.Int)
        Me.m_values.Add(val.varName, val)

        val = New cValue(core, New Integer, eVarNameFlags.CodeSLB, eStatusFlags.OK, eValueTypes.Int)
        Me.m_values.Add(val.varName, val)

        val = New cValue(core, New String(cbuf), eVarNameFlags.CodeAquaMaps, eStatusFlags.OK, eValueTypes.Str)
        Me.m_values.Add(val.varName, val)

        val = New cValue(core, New String(cbuf), eVarNameFlags.CodeAphia, eStatusFlags.OK, eValueTypes.Str)
        Me.m_values.Add(val.varName, val)

        val = New cValue(core, New Integer, eVarNameFlags.CodeOBIS, eStatusFlags.OK, eValueTypes.Int)
        Me.m_values.Add(val.varName, val)

        val = New cValue(core, New String(cbuf), eVarNameFlags.CodeLSID, eStatusFlags.OK, eValueTypes.Str)
        Me.m_values.Add(val.varName, val)

        val = New cValue(core, New String(cbuf), eVarNameFlags.CodeFAO, eStatusFlags.OK, eValueTypes.Str)
        Me.m_values.Add(val.varName, val)

        val = New cValue(core, New String(cbuf), eVarNameFlags.Source, eStatusFlags.OK, eValueTypes.Str)
        Me.m_values.Add(val.varName, val)

        val = New cValue(core, New String(cbuf), eVarNameFlags.SourceKey, eStatusFlags.OK, eValueTypes.Str)
        Me.m_values.Add(val.varName, val)

        val = New cValue(core, New Single, eVarNameFlags.North, eStatusFlags.OK, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)

        val = New cValue(core, New Single, eVarNameFlags.South, eStatusFlags.OK, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)

        val = New cValue(core, New Single, eVarNameFlags.East, eStatusFlags.OK, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)

        val = New cValue(core, New Single, eVarNameFlags.West, eStatusFlags.OK, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)

        ' Search fields
        val = New cValue(core, New Long, eVarNameFlags.TaxonSearchFields, eStatusFlags.OK, eValueTypes.Int)
        Me.m_values.Add(val.varName, val)

        ' Proportion of biomass
        val = New cValue(core, New Single, eVarNameFlags.TaxonPropBiomass, eStatusFlags.Null, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)

        ' Proportion of catch
        val = New cValue(core, New Single, eVarNameFlags.TaxonPropCatch, eStatusFlags.Null, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)

        ' EcologyType
        val = New cValue(core, New Integer, eVarNameFlags.EcologyType, eStatusFlags.Null, eValueTypes.Int)
        Me.m_values.Add(val.varName, val)

        ' OrganismType
        val = New cValue(core, New Integer, eVarNameFlags.OrganismType, eStatusFlags.Null, eValueTypes.Int)
        Me.m_values.Add(val.varName, val)

        ' IUCNConservationStatus
        val = New cValue(core, New Integer, eVarNameFlags.IUCNConservationStatus, eStatusFlags.Null, eValueTypes.Int)
        Me.m_values.Add(val.varName, val)

        ' ExploitationStatus
        val = New cValue(core, New Integer, eVarNameFlags.ExploitationStatus, eStatusFlags.Null, eValueTypes.Int)
        Me.m_values.Add(val.varName, val)

        ' OccurrenceStatus
        val = New cValue(core, New Integer, eVarNameFlags.OccurrenceStatus, eStatusFlags.Null, eValueTypes.Int)
        Me.m_values.Add(val.varName, val)

        ' TaxonMeanWeight
        val = New cValue(core, New Single, eVarNameFlags.TaxonMeanWeight, eStatusFlags.Null, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)

        ' TaxonMeanLength
        val = New cValue(core, New Single, eVarNameFlags.TaxonMeanLength, eStatusFlags.Null, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)

        ' TaxonMaxLength
        val = New cValue(core, New Single, eVarNameFlags.TaxonMaxLength, eStatusFlags.Null, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)

        ' TaxonMeanLifespan
        val = New cValue(core, New Single, eVarNameFlags.TaxonMeanLifespan, eStatusFlags.Null, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)

        ' TaxonVulnerabiltyIndex
        val = New cValue(core, New Integer, eVarNameFlags.TaxonVulnerabilityIndex, eStatusFlags.Null, eValueTypes.Int)
        Me.m_values.Add(val.varName, val)

        ' TaxonWinf
        val = New cValue(core, New Single, eVarNameFlags.TaxonWinf, eStatusFlags.Null, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)

        ' TaxonvbgfK
        val = New cValue(core, New Single, eVarNameFlags.TaxonvbgfK, eStatusFlags.Null, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)

        ' Last updated julian date
        val = New cValue(core, New Single, eVarNameFlags.LastUpdated, eStatusFlags.OK, eValueTypes.Sng)
        Me.m_values.Add(val.varName, val)

        Me.AllowValidation = True

    End Sub

#End Region

#Region " Overrides "

    Friend Overrides Function ResetStatusFlags(Optional bForceReset As Boolean = False) As Boolean

        MyBase.ResetStatusFlags(bForceReset)
        Me.m_core.Set_Taxon_Flags(Me, False)
        Return True

    End Function

    Public Overrides Function ToString() As String
        Return String.Format("{0} {1}", Me.Genus, Me.Species)
    End Function

#End Region ' Overrides

#Region " Variables via dot (.) operator "

    ''' <summary>
    ''' Get/set the index of the Ecopath group that a taxonomy definition contributes to.
    ''' </summary>
    Public Property iGroup() As Integer
        Get
            Return CInt(Me.GetVariable(eVarNameFlags.TaxonGroup))
        End Get
        Set(value As Integer)
            Me.SetVariable(eVarNameFlags.TaxonGroup, value)
        End Set
    End Property

    ''' <summary>
    ''' Get/set the index of the Stanza configuration that a taxonomy definition contributes to.
    ''' </summary>
    Public Property iStanza() As Integer
        Get
            Return CInt(Me.GetVariable(eVarNameFlags.TaxonStanza))
        End Get
        Set(value As Integer)
            Me.SetVariable(eVarNameFlags.TaxonStanza, value)
        End Set
    End Property

    ''' <summary>
    ''' Get/set the proportion that a taxonomy definition contributes to a <see cref="Group">group</see>.
    ''' </summary>
    Public Property PropB() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.TaxonPropBiomass))
        End Get
        Set(value As Single)
            Me.SetVariable(eVarNameFlags.TaxonPropBiomass, value)
        End Set
    End Property

    ''' <summary>
    ''' Get/set the phylum of a taxonomy definition.
    ''' </summary>
    Public Property Phylum() As String _
        Implements ITaxonDetailsData.Phylum
        Get
            Return CStr(Me.GetVariable(eVarNameFlags.Phylum))
        End Get
        Set(value As String)
            Me.SetVariable(eVarNameFlags.Phylum, value)
        End Set
    End Property

    ''' <summary>
    ''' Get/set the class of a taxonomy definition.
    ''' </summary>
    Public Property [Class]() As String _
        Implements ITaxonDetailsData.Class
        Get
            Return CStr(Me.GetVariable(eVarNameFlags.Class))
        End Get
        Set(value As String)
            Me.SetVariable(eVarNameFlags.Class, value)
        End Set
    End Property

    ''' <summary>
    ''' Get/set the order of a taxonomy definition.
    ''' </summary>
    Public Property Order() As String _
        Implements ITaxonDetailsData.Order
        Get
            Return CStr(Me.GetVariable(eVarNameFlags.Order))
        End Get
        Set(value As String)
            Me.SetVariable(eVarNameFlags.Order, value)
        End Set
    End Property

    ''' <summary>
    ''' Get/set the family of a taxonomy definition.
    ''' </summary>
    Public Property Family() As String _
        Implements ITaxonDetailsData.Family
        Get
            Return CStr(Me.GetVariable(eVarNameFlags.Family))
        End Get
        Set(value As String)
            Me.SetVariable(eVarNameFlags.Family, value)
        End Set
    End Property

    ''' <summary>
    ''' Get/set the genus of a taxonomy definition.
    ''' </summary>
    Public Property Genus() As String _
        Implements ITaxonDetailsData.Genus
        Get
            Return CStr(Me.GetVariable(eVarNameFlags.Genus))
        End Get
        Set(value As String)
            Me.SetVariable(eVarNameFlags.Genus, value)
        End Set
    End Property

    ''' <summary>
    ''' Get/set the species of a taxonomy definition.
    ''' </summary>
    Public Property Species() As String _
        Implements ITaxonDetailsData.Species
        Get
            Return CStr(Me.GetVariable(eVarNameFlags.Species))
        End Get
        Set(value As String)
            Me.SetVariable(eVarNameFlags.Species, value)
        End Set
    End Property

    ''' <summary>
    ''' Get/set the species common name.
    ''' </summary>
    Public Property Common() As String _
        Implements ITaxonDetailsData.Common
        Get
            Return Me.Name
        End Get
        Set(value As String)
            Me.Name = value
        End Set
    End Property

    ''' <summary>
    ''' Get/set flags last used to search the taxon.
    ''' </summary>
    Public Property SearchFields() As eTaxonClassificationType _
        Implements ITaxonDetailsData.SearchFields
        Get
            Return DirectCast(Me.GetVariable(eVarNameFlags.TaxonSearchFields), eTaxonClassificationType)
        End Get
        Set(value As eTaxonClassificationType)
            Me.SetVariable(eVarNameFlags.TaxonSearchFields, value)
        End Set
    End Property

    ''' <inheritdocs cref=" ITaxonDetailsData.CodeSAUP"/>
    Public Property CodeSAUP() As Long _
        Implements ITaxonDetailsData.CodeSAUP
        Get
            Return CLng(Me.GetVariable(eVarNameFlags.CodeSAUP))
        End Get
        Set(value As Long)
            Me.SetVariable(eVarNameFlags.CodeSAUP, value)
        End Set
    End Property

    ''' <inheritdocs cref=" ITaxonDetailsData.CodeFishBase"/>
    Public Property CodeFishBase() As Long _
        Implements ITaxonDetailsData.CodeFishBase
        Get
            Return CLng(Me.GetVariable(eVarNameFlags.CodeFB))
        End Get
        Set(value As Long)
            Me.SetVariable(eVarNameFlags.CodeFB, value)
        End Set
    End Property

    ''' <inheritdocs cref=" ITaxonDetailsData.CodeSeaLifeBase"/>
    Public Property CodeSeaLifeBase() As Long _
        Implements ITaxonDetailsData.CodeSeaLifeBase
        Get
            Return CLng(Me.GetVariable(eVarNameFlags.CodeSLB))
        End Get
        Set(value As Long)
            Me.SetVariable(eVarNameFlags.CodeSLB, value)
        End Set
    End Property

    ''' <inheritdocs cref=" ITaxonDetailsData.CodeAquaMaps"/>
    Public Property CodeAquaMaps As String Implements ITaxonSearchData.CodeAquaMaps
        Get
            Return CStr(Me.GetVariable(eVarNameFlags.CodeAquaMaps))
        End Get
        Set(value As String)
            Me.SetVariable(eVarNameFlags.CodeAquaMaps, value)
        End Set
    End Property

    ''' <inheritdocs cref=" ITaxonDetailsData.CodeAphia"/>
    Public Property CodeAphia As String Implements ITaxonSearchData.CodeAphia
        Get
            Return CStr(Me.GetVariable(eVarNameFlags.CodeAphia))
        End Get
        Set(value As String)
            Me.SetVariable(eVarNameFlags.CodeAphia, value)
        End Set
    End Property

    ''' <inheritdocs cref=" ITaxonDetailsData.CodeOBIS"/>
    Public Property CodeOBIS As Long Implements ITaxonSearchData.CodeOBIS
        Get
            Return CLng(Me.GetVariable(eVarNameFlags.CodeOBIS))
        End Get
        Set(value As Long)
            Me.SetVariable(eVarNameFlags.CodeOBIS, value)
        End Set
    End Property

    ''' <inheritdocs cref=" ITaxonDetailsData.CodeFAO"/>
    Public Property CodeFAO() As String _
        Implements ITaxonDetailsData.CodeFAO
        Get
            Return CStr(Me.GetVariable(eVarNameFlags.CodeFAO))
        End Get
        Set(value As String)
            Me.SetVariable(eVarNameFlags.CodeFAO, value)
        End Set
    End Property

    ''' <inheritdocs cref=" ITaxonDetailsData.CodeLSID"/>
    Public Property CodeLSID() As String _
        Implements ITaxonDetailsData.CodeLSID
        Get
            Return CStr(Me.GetVariable(eVarNameFlags.CodeLSID))
        End Get
        Set(value As String)
            Me.SetVariable(eVarNameFlags.CodeLSID, value)
        End Set
    End Property

    ''' <summary>
    ''' Get/set the name of the source that a taxonomy definition was obtained from.
    ''' </summary>
    Public Property Source() As String _
        Implements ITaxonDetailsData.Source
        Get
            Return CStr(Me.GetVariable(eVarNameFlags.Source))
        End Get
        Set(value As String)
            Me.SetVariable(eVarNameFlags.Source, value)
        End Set
    End Property

    ''' <summary>
    ''' Get/set the key to refresh a taxonomy definition from the <see cref="Source">source</see>.
    ''' </summary>
    Public Property SourceKey() As String _
        Implements ITaxonDetailsData.SourceKey
        Get
            Return CStr(Me.GetVariable(eVarNameFlags.SourceKey))
        End Get
        Set(value As String)
            Me.SetVariable(eVarNameFlags.SourceKey, value)
        End Set
    End Property

    ''' <summary>
    ''' Get/set the southern extent of the model bounding box.
    ''' </summary>
    Public Property South() As Single _
        Implements ITaxonDetailsData.South
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.South))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.South, value)
        End Set
    End Property

    ''' <summary>
    ''' Get/set the northern extent of the model bounding box.
    ''' </summary>
    Public Property North() As Single _
        Implements ITaxonDetailsData.North
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.North))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.North, value)
        End Set
    End Property

    ''' <summary>
    ''' Get/set the western extent of the model bounding box.
    ''' </summary>
    Public Property West() As Single _
        Implements ITaxonDetailsData.West
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.West))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.West, value)
        End Set
    End Property

    ''' <summary>
    ''' Get/set the eastern extent of the model bounding box.
    ''' </summary>
    Public Property East() As Single _
        Implements ITaxonDetailsData.East
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.East))
        End Get

        Set(value As Single)
            Me.SetVariable(eVarNameFlags.East, value)
        End Set
    End Property

    ''' <summary>
    ''' Get/set the <see cref="eEcologyTypes"/> for a taxon.
    ''' </summary>
    Public Property EcologyType() As eEcologyTypes _
        Implements ITaxonDetailsData.EcologyType
        Get
            Return DirectCast(Me.GetVariable(eVarNameFlags.EcologyType), eEcologyTypes)
        End Get
        Set(value As eEcologyTypes)
            Me.SetVariable(eVarNameFlags.EcologyType, value)
        End Set
    End Property

    ''' <summary>
    ''' Get/set the <see cref="eOrganismTypes"/> for a taxon.
    ''' </summary>
    Public Property OrganismType() As eOrganismTypes _
        Implements ITaxonDetailsData.OrganismType
        Get
            Return DirectCast(Me.GetVariable(eVarNameFlags.OrganismType), eOrganismTypes)
        End Get
        Set(value As eOrganismTypes)
            Me.SetVariable(eVarNameFlags.OrganismType, value)
        End Set
    End Property

    ''' <summary>
    ''' Get/set the proportion of the catch of this taxon.
    ''' </summary>
    Public Property PropC() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.TaxonPropCatch))
        End Get
        Set(value As Single)
            Me.SetVariable(eVarNameFlags.TaxonPropCatch, value)
        End Set
    End Property

    ''' <summary>
    ''' Get/set the <see cref="eIUCNConservationStatusTypes"/> for a taxon.
    ''' </summary>
    Public Property IUCNConservationStatus() As eIUCNConservationStatusTypes _
        Implements ITaxonDetailsData.IUCNConservationStatus
        Get
            Return DirectCast(Me.GetVariable(eVarNameFlags.IUCNConservationStatus), eIUCNConservationStatusTypes)
        End Get
        Set(value As eIUCNConservationStatusTypes)
            Me.SetVariable(eVarNameFlags.IUCNConservationStatus, value)
        End Set
    End Property

    ''' <summary>
    ''' Get/set the <see cref="eExploitationTypes"/> for a taxon.
    ''' </summary>
    Public Property ExploitationStatus() As eExploitationTypes _
        Implements ITaxonDetailsData.ExploitationStatus
        Get
            Return DirectCast(Me.GetVariable(eVarNameFlags.ExploitationStatus), eExploitationTypes)
        End Get
        Set(value As eExploitationTypes)
            Me.SetVariable(eVarNameFlags.ExploitationStatus, value)
        End Set
    End Property

    ''' <summary>
    ''' Get/set the <see cref="eOccurrenceStatusTypes"/> for a taxon.
    ''' </summary>
    Public Property OccurrenceStatus() As eOccurrenceStatusTypes _
        Implements ITaxonDetailsData.OccurrenceStatus
        Get
            Return DirectCast(Me.GetVariable(eVarNameFlags.OccurrenceStatus), eOccurrenceStatusTypes)
        End Get
        Set(value As eOccurrenceStatusTypes)
            Me.SetVariable(eVarNameFlags.OccurrenceStatus, value)
        End Set
    End Property

    ''' <summary>
    ''' Get/set the mean weight for a taxon.
    ''' </summary>
    Public Property MeanWeight() As Single _
        Implements ITaxonDetailsData.MeanWeight
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.TaxonMeanWeight))
        End Get
        Set(value As Single)
            Me.SetVariable(eVarNameFlags.TaxonMeanWeight, value)
        End Set
    End Property

    ''' <summary>
    ''' Get/set the mean weight status for a taxon.
    ''' </summary>
    Public Property MeanWeightStatus() As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.TaxonMeanWeight)
        End Get
        Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.TaxonMeanWeight, value)
        End Set
    End Property

    ''' <summary>
    ''' Get/set the mean length for a taxon.
    ''' </summary>
    Public Property MeanLength() As Single _
        Implements ITaxonDetailsData.MeanLength
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.TaxonMeanLength))
        End Get
        Set(value As Single)
            Me.SetVariable(eVarNameFlags.TaxonMeanLength, value)
        End Set
    End Property

    ''' <summary>
    ''' Get/set the mean length status for a taxon.
    ''' </summary>
    Public Property MeanLengthStatus() As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.TaxonMeanLength)
        End Get
        Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.TaxonMeanLength, value)
        End Set
    End Property

    ''' <summary>
    ''' Get/set the max length for a taxon.
    ''' </summary>
    Public Property MaxLength() As Single _
        Implements ITaxonDetailsData.MaxLength
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.TaxonMaxLength))
        End Get
        Set(value As Single)
            Me.SetVariable(eVarNameFlags.TaxonMaxLength, value)
        End Set
    End Property

    ''' <summary>
    ''' Get/set the max length status for a taxon.
    ''' </summary>
    Public Property MaxLengthStatus() As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.TaxonMaxLength)
        End Get
        Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.TaxonMaxLength, value)
        End Set
    End Property

    ''' <summary>
    ''' Get/set the mean life span for a taxon.
    ''' </summary>
    Public Property MeanLifespan() As Single _
        Implements ITaxonDetailsData.MeanLifespan
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.TaxonMeanLifespan))
        End Get
        Set(value As Single)
            Me.SetVariable(eVarNameFlags.TaxonMeanLifespan, value)
        End Set
    End Property

    ''' <summary>
    ''' Get/set the mean life span status for a taxon.
    ''' </summary>
    Public Property MeanLifespanStatus() As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.TaxonMeanLifespan)
        End Get
        Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.TaxonMeanLifespan, value)
        End Set
    End Property

    ''' <summary>
    ''' Get/set the vulnerability index for a taxon.
    ''' </summary>
    Public Property VulnerabilityIndex() As Integer _
        Implements ITaxonDetailsData.VulnerabilityIndex
        Get
            Return CInt(Me.GetVariable(eVarNameFlags.TaxonVulnerabilityIndex))
        End Get
        Set(value As Integer)
            Me.SetVariable(eVarNameFlags.TaxonVulnerabilityIndex, value)
        End Set
    End Property

    ''' <summary>
    ''' Get/set the vulnerability index status for a taxon.
    ''' </summary>
    Public Property VulnerabilityIndexStatus() As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.TaxonVulnerabilityIndex)
        End Get
        Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.TaxonVulnerabilityIndex, value)
        End Set
    End Property

    ''' <summary>
    ''' Get/set the asymptotic weight for a taxon.
    ''' </summary>
    Public Property Winf() As Single _
        Implements ITaxonDetailsData.Winf
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.TaxonWinf))
        End Get
        Set(value As Single)
            Me.SetVariable(eVarNameFlags.TaxonWinf, value)
        End Set
    End Property

    ''' <summary>
    ''' Get/set the asymptotic weight for a taxon.
    ''' </summary>
    Public Property vbgfK() As Single _
        Implements ITaxonDetailsData.vbgfK
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.TaxonvbgfK))
        End Get
        Set(value As Single)
            Me.SetVariable(eVarNameFlags.TaxonvbgfK, value)
        End Set
    End Property

    ''' <summary>
    ''' Get/set the Julian date the taxonomy definition was last updated.
    ''' </summary>
    Public Property LastUpdated() As Double _
        Implements ITaxonDetailsData.LastUpdated
        Get
            Return CDbl(Me.GetVariable(eVarNameFlags.LastUpdated))
        End Get

        Set(value As Double)
            Me.SetVariable(eVarNameFlags.LastUpdated, value)
        End Set
    End Property

#Region " Obsolete "

    ''' <summary>
    ''' Get/set the index of the Ecopath group that a taxonomy definition contributes to.
    ''' </summary>
    <Obsolete("Use iGroup instead")>
    Public Property Group() As Integer
        Get
            Return Me.iGroup
        End Get
        Set(value As Integer)
            Me.iGroup = value
        End Set
    End Property

    ''' <summary>
    ''' Get/set the index of the Stanza configuration that a taxonomy definition contributes to.
    ''' </summary>
    <Obsolete("Use iStanza instead")>
    Public Property Stanza() As Integer
        Get
            Return Me.iStanza
        End Get
        Set(value As Integer)
            Me.iStanza = value
        End Set
    End Property

#End Region ' Obsolete

#End Region

End Class
