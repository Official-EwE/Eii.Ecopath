' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.IO
Imports System.Xml
Imports System.Xml.Serialization
Imports EwEUtils.NetUtilities
Imports EwEUtils.Utilities
Imports Microsoft.Extensions.Logging
Imports Debug = System.Diagnostics.Debug

Namespace WebServices.Ecobase

#Region " Model "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Base class for containing the data for model parameters.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cModelData

        Public Enum eSubmissionType As Integer
            [New] = 0
            Replacement = 1
            Derived = 2
        End Enum

#Region " Variables "

        ''' <summary>Ecobase ID.</summary>
        <XmlElement("model_number")>
        Public Property EcobaseCode As String = ""

        <XmlElement("model_name")>
        Public Property Name As String = ""

        <XmlElement("description")>
        Public Property Description As String = ""

        <XmlElement("author")>
        Public Property Author As String = ""

        <XmlElement("contact")>
        Public Property Contact As String = ""

        <XmlElement("num_digits")>
        Public Property NumDigits As Integer = 3

        <XmlElement("model_year")>
        Public Property FirstYear As Integer = 0

        <XmlElement("model_period")>
        Public Property NumYears As Integer = 1

        <XmlElement("country")>
        Public Property Country As String = ""

        ''' <summary>Area size.</summary>
        <XmlElement("area")>
        Public Property Area As Single = 0

        ''' <summary>Northern limit of the <see cref="Extent"/>.</summary>
        <XmlIgnore()>
        Public Property North As Single = cCore.NULL_VALUE

        ''' <summary>Eastern limit of the <see cref="Extent"/>.</summary>
        <XmlIgnore()>
        Public Property East As Single = cCore.NULL_VALUE

        ''' <summary>Western limit of the <see cref="Extent"/>.</summary>
        <XmlIgnore()>
        Public Property West As Single = cCore.NULL_VALUE

        ''' <summary>Southern limit of the <see cref="Extent"/>.</summary>
        <XmlIgnore()>
        Public Property South As Single = cCore.NULL_VALUE

        ''' <summary>Spatial bounding box.</summary>
        <XmlElement("geographic_extent")>
        Public Property Extent As String
            Get
                Return "BOX(" & cStringUtils.FormatSingle(Me.West) & " " & cStringUtils.FormatSingle(Me.North) & "," & cStringUtils.FormatSingle(Me.East) & " " & cStringUtils.FormatSingle(Me.South) & ")"
            End Get
            Set(value As String)
                Dim astrBits() As String = value.ToUpper().Replace("BOX(", "").Replace(",", " ").Replace(")", "").Trim().Split(" "c)
                If (astrBits.Length > 3) Then
                    Single.TryParse(astrBits(0), Me.West)
                    Single.TryParse(astrBits(1), Me.North)
                    Single.TryParse(astrBits(2), Me.East)
                    Single.TryParse(astrBits(3), Me.South)
                End If
            End Set
        End Property

        ''' <summary>Ecosystem type.</summary>
        <XmlElement("ecosystem_type")>
        Public Property EcosystemType As String = ""

        ''' <summary>Currency unit.</summary>
        <XmlElement("currency_units")>
        Public Property UnitCurrency As String = ""

        ''' <summary>Flag, stating if currency unit is a custom unit.</summary>
        <XmlElement("currency_units_custom")>
        Public Property UnitCurrencyIsCustom As Boolean

        ''' <summary>Flag, stating if Ecobase has the right to make model parameters available for download.</summary>
        <XmlElement("dissemination_allow")>
        Public Property AllowDissemination As Boolean = False

        ''' <summary>The digitial object identifier (doi) of the publication for this model.</summary>
        <XmlElement("doi")>
        Public Property DOI As String = ""

        ''' <summary>The URI to the publication for this model.</summary>
        <XmlElement("url")>
        Public Property URI As String = ""

        ''' <summary>The reference of the publication for this model.</summary>
        <XmlElement("reference")>
        Public Property Reference As String = ""

        ''' <summary>EwE version</summary>
        <XmlElement("ewe_version")>
        Public Property EwEVersion As String = ""

        ''' <summary>Flag, stating if the model matches the paper version.</summary>
        <XmlElement("match_paper")>
        Public Property ModelMatchesPaper As Boolean = False

        ''' <summary></summary>
        <XmlElement("temperature_mean")>
        Public Property TempMean As Single = 0
        ''' <summary></summary>
        <XmlElement("temperature_min")>
        Public Property TempMin As Single = 0
        ''' <summary></summary>
        <XmlElement("temperature_max")>
        Public Property TempMax As Single = 0

        ''' <summary></summary>
        <XmlElement("depth_mean")>
        Public Property DepthMean As Single = 0
        ''' <summary></summary>
        <XmlElement("depth_min")>
        Public Property DepthMin As Single = 0
        ''' <summary></summary>
        <XmlElement("depth_max")>
        Public Property DepthMax As Single = 0

        ''' <summary>Is Ecosim used?</summary>
        <XmlElement("ecosim")>
        Public Property EcosimUsed As Boolean = False

        ''' <summary>Is Ecospace used?</summary>
        <XmlElement("ecospace")>
        Public Property EcospaceUsed As Boolean = False

        <XmlElement("is_fitted")>
        Public Property IsFittedToTimeSeries As Boolean = False

        ''' <summary>Is the entire foodweb accounted for?</summary>
        <XmlElement("whole_food_web")>
        Public Property IsWholeFoodWeb As Boolean = False

        ''' <summary>Comments if there is difference between model used for the references and model upload</summary>
        <XmlElement("comments_difference")>
        Public Property CommentsDifference As String

        ''' <summary>Comments if model is not declared as open access.</summary>
        <XmlElement("comments_access")>
        Public Property CommentsAccess As String

        <XmlElement("fisheries")>
        Public Property ObjectiveFisheries As Boolean
        <XmlElement("aquaculture")>
        Public Property ObjectiveAquaculture As Boolean
        <XmlElement("environment_variability")>
        Public Property ObjectiveEnvironmentalVariability As Boolean
        <XmlElement("ecosyst_functioning")>
        Public Property ObjectiveEcosystemFunctioning As Boolean
        <XmlElement("pollution")>
        Public Property ObjectivePollution As Boolean
        <XmlElement("mpa")>
        Public Property ObjectiveMarineProtection As Boolean
        <XmlElement("other_impact_assessment")>
        Public Property ObjectiveOtherImpactAssessment As Boolean
        ''' <summary>Description of objectives of the model.</summary>
        <XmlElement("comments_objectives")>
        Public Property Objectives As String

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks>Set as <see cref="eSubmissionType"/></remarks>
        <XmlElement("submission_type")>
        Public Property SubmissionType As Integer

        ''' <summary>
        ''' Linked / updated EcoBase model
        ''' </summary>
        <XmlElement("modification_child")>
        Public Property SubmissionLink As String

        <XmlElement("modification_comments")>
        Public Property SubmissionComments As String

#End Region ' Variables

#Region " Construction "

        Public Sub New()
            ' NOP
        End Sub

        Public Sub New(core As cCore)

            Dim ecopathDS As cEcopathDataStructures = core.m_EcopathData
            Dim taxonDS As cTaxonDataStructures = core.m_TaxonData

            Me.Name = ecopathDS.ModelName
            Me.Description = ecopathDS.ModelDescription
            Me.EcobaseCode = ecopathDS.ModelEcobaseCode

            Me.Author = ecopathDS.ModelAuthor
            Me.Contact = ecopathDS.ModelContact

            Me.FirstYear = ecopathDS.FirstYear
            Me.NumYears = ecopathDS.NumYears

            Me.Area = ecopathDS.ModelArea

            Me.North = ecopathDS.ModelNorth
            Me.East = ecopathDS.ModelEast
            Me.West = ecopathDS.ModelWest
            Me.South = ecopathDS.ModelSouth

            Me.DOI = ecopathDS.ModelPublicationDOI
            Me.URI = ecopathDS.ModelPublicationURI
            Me.Reference = ecopathDS.ModelPublicationRef

            Me.UnitCurrencyIsCustom = Not String.IsNullOrWhiteSpace(ecopathDS.ModelUnitCurrencyCustom)
            Me.UnitCurrency = If(Me.UnitCurrencyIsCustom,
                                               ecopathDS.ModelUnitCurrencyCustom,
                                               DirectCast(ecopathDS.ModelUnitCurrency, eUnitCurrencyType).ToString())

            Me.EcosystemType = ecopathDS.ModelEcosystemType
            Me.Country = ecopathDS.ModelCountry

            Me.DepthMin = 0
            Me.DepthMean = 0
            Me.DepthMax = 0

            Me.TempMin = 0
            Me.TempMean = 0
            Me.TempMax = 0

            Me.EwEVersion = cCore.Version

        End Sub

#End Region ' Construction "

    End Class

#End Region ' Model

#Region " Groups "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Base class for containing all data for a single group.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cGroupData

#Region " Variables "

        <XmlElement("group_seq")>
        Public Property Index As Integer
        <XmlElement("group_name")>
        Public Property Name() As String
        <XmlElement("group_color")>
        Public Property Color As Integer

        ' -- Basic inputs --

        <XmlElement("habitat_area")>
        Public Property Area As Single
        <XmlElement("biomass")>
        Public Property B As Single
        ''' <summary>Biomass in habitat area</summary>
        <XmlElement("biomass_habitat_area")>
        Public Property BH As Single
        <XmlElement("b_hab_area_input")>
        Public Property BHIsInput As Boolean

        <XmlElement("pb")>
        Public Property PB As Single
        <XmlElement("pb_input")>
        Public Property PBIsInput As Boolean

        <XmlElement("qb")>
        Public Property QB As Single
        <XmlElement("qb_input")>
        Public Property QBIsInput As Boolean

        <XmlElement("ee")>
        Public Property EE As Single
        <XmlElement("ee_input")>
        Public Property EEIsInput As Boolean

        <XmlElement("ge")>
        Public Property GE As Single
        <XmlElement("ge_input")>
        Public Property GEIsInput As Boolean

        <XmlElement("detritus_import")>
        Public Property DtImp As Single
        <XmlElement("export")>
        Public Property Ex As Single
        <XmlElement("pp")>
        Public Property PP As Single
        <XmlElement("gs")>
        Public Property GS As Single
        <XmlElement("biomass_accum_rate_input")>
        Public BAIsInput As Boolean
        <XmlElement("biomass_accum")>
        Public Property BA As Single
        <XmlElement("biomass_accum_rate")>
        Public Property BaBi As Single
        <XmlElement("respiration")>
        Public Property Respiration As Single
        <XmlElement("immigration")>
        Public Property Immig As Single
        <XmlElement("emigration")>
        Public Property Emig As Single
        <XmlElement("emigration_rate")>
        Public Property EmigRate As Single
        ''' <summary>Non-market price</summary>
        <XmlElement("shadow_price")>
        Public Property Shadow As Single
        <XmlElement("other_mort")>
        Public Property OtherMort As Single
        <XmlElement("other_mort_rate")>
        Public Property MortCoOtherMort As Single
        <XmlElement("vbk")>
        Public Property vbK As Single

        ' -- Fields exclusively for the benefit of Ecotroph --
        <XmlElement("tl")>
        Public Property TL As Single
        <XmlElement("oi")>
        Public Property OmnivoryIndex As Single
        <XmlElement("flow_to_det")>
        Public Property FlowToDet As Single
        <XmlElement("net_efficiency")>
        Public Property NetEfficiency As Single
        <XmlElement("fish_mort_rate")>
        Public Property MortCoFishRate As Single
        <XmlElement("pred_mort_rate")>
        Public Property MortCoPredMort As Single
        <XmlElement("net_migration_rate")>
        Public Property MortCoNetMig As Single

        ' Diets
        <XmlArray("diet_descr")>
        <XmlArrayItem("diet")>
        Public Property Diets As New List(Of cDietData)
        <XmlElement("diet_imp")>
        Public Property ImpVar As Single

        ' Pedigree
        <XmlArray("pedigree_assignment_descr")>
        <XmlArrayItem("pedigree_assignment")>
        Public Property PedigreeAssignments As New List(Of cPedigreeAssignmentData)

        ' Taon
        <XmlArray("taxon_descr")>
        <XmlArrayItem("taxon")>
        Public Property Taxa As New List(Of cTaxonData)

#End Region ' Variables

#Region " Constructor "

        Public Sub New()
            ' NOP
        End Sub

        ''' <summary>
        ''' Constructor, initializes an instance with model data for submitting to EcoBase.
        ''' </summary>
        ''' <param name="core">The core to obtain data from.</param>
        ''' <param name="iGroup"></param>
        Public Sub New(core As cCore, iGroup As Integer)

            ' Sanity checks
            Debug.Assert(core IsNot Nothing)
            Debug.Assert(core.StateMonitor.HasEcopathRan)
            Debug.Assert(iGroup <= core.nGroups)

            Dim ecopathDS As cEcopathDataStructures = core.m_EcopathData
            Dim taxonDS As cTaxonDataStructures = core.m_TaxonData

            Me.Index = iGroup
            Me.Name = ecopathDS.GroupName(iGroup)
            Me.PP = ecopathDS.PP(iGroup)
            Me.Area = ecopathDS.Area(iGroup)
            Me.BA = ecopathDS.BAInput(iGroup)
            Me.BAIsInput = (ecopathDS.BAInput(iGroup) = ecopathDS.BA(iGroup))
            Me.BaBi = ecopathDS.BaBi(iGroup)
            Me.GS = ecopathDS.GS(iGroup)
            Me.DtImp = ecopathDS.DtImp(iGroup)
            Me.Ex = ecopathDS.Ex(iGroup)
            Me.ImpVar = ecopathDS.DC(iGroup, 0)
            'drow("GroupIsFish") = ecopathDS.GroupIsFish(iGroup)
            'drow("GroupIsInvert") = ecopathDS.GroupIsInvert(iGroup)
            Me.Shadow = ecopathDS.Shadow(iGroup)
            Me.Respiration = ecopathDS.Resp(iGroup)
            Me.OtherMort = ecopathDS.OtherMortinput(iGroup)

            Me.BHIsInput = (ecopathDS.Binput(iGroup) >= 0)
            Me.B = If(Me.BHIsInput, ecopathDS.Binput(iGroup), ecopathDS.B(iGroup))
            Me.BH = ecopathDS.Binput(iGroup) / ecopathDS.Area(iGroup)

            Me.EEIsInput = (ecopathDS.EEinput(iGroup) >= 0)
            Me.EE = If(Me.EEIsInput, ecopathDS.EEinput(iGroup), ecopathDS.EE(iGroup))

            Me.PBIsInput = (ecopathDS.PBinput(iGroup) >= 0)
            Me.PB = If(Me.PBIsInput, ecopathDS.PBinput(iGroup), ecopathDS.PB(iGroup))

            Me.QBIsInput = (ecopathDS.QBinput(iGroup) >= 0)
            Me.QB = If(Me.QBIsInput, ecopathDS.QBinput(iGroup), ecopathDS.QB(iGroup))

            Me.GEIsInput = (ecopathDS.GEinput(iGroup) >= 0)
            Me.GE = If(Me.GEIsInput, ecopathDS.GEinput(iGroup), ecopathDS.GE(iGroup))

            Me.Immig = ecopathDS.Immig(iGroup)
            Me.Emig = ecopathDS.Emigration(iGroup)
            Me.EmigRate = ecopathDS.Emig(iGroup)
            Me.Color = ecopathDS.GroupColor(iGroup)
            Me.vbK = ecopathDS.vbK(iGroup)

            Dim grpOut As cEcopathGroupOutput = core.EcopathGroupOutputs(iGroup)
            Me.TL = grpOut.TTLX
            Me.OmnivoryIndex = grpOut.OmnivoryIndex
            Me.BaBi = grpOut.BioAccumRatePerYear
            Me.FlowToDet = grpOut.FlowToDet
            Me.NetEfficiency = grpOut.NetEfficiency
            Me.MortCoFishRate = grpOut.MortCoFishRate
            Me.MortCoPredMort = grpOut.MortCoPredMort
            Me.MortCoNetMig = grpOut.MortCoNetMig
            Me.MortCoOtherMort = grpOut.MortCoOtherMort

            'PSD
            'drow("Tcatch") = psdDS.Tcatch(iGroup)
            'drow("AinLW") = psdDS.AinLWInput(iGroup)
            'drow("BinLW") = psdDS.BinLWInput(iGroup)
            'drow("Loo") = psdDS.LooInput(iGroup)
            'drow("Winf") = psdDS.WinfInput(iGroup)
            'drow("t0") = psdDS.t0Input(iGroup)
            'drow("Tmax") = psdDS.TmaxInput(iGroup)

            Me.Diets.Clear()
            For iPrey As Integer = 1 To core.nGroups
                Dim dc As Single = ecopathDS.DC(iGroup, iPrey)
                Dim df As Single = 0
                If (iPrey > core.nLivingGroups) Then df = ecopathDS.DF(iGroup, iPrey - core.nLivingGroups)
                If ((dc + df) > 0) Then
                    Dim diet As New cDietData(iPrey, dc, df)
                    Me.Diets.Add(diet)
                End If
            Next

            Me.Taxa.Clear()
            For iTaxon As Integer = 1 To taxonDS.NumTaxon
                If (taxonDS.IsTaxonStanza(iTaxon) = False) And (taxonDS.TaxonTarget(iTaxon) = iGroup) Then
                    Me.Taxa.Add(New cTaxonData(core, iTaxon))
                End If
            Next

            Me.PedigreeAssignments.Clear()
            For iVar As Integer = 1 To ecopathDS.NumPedigreeVariables
                If (ecopathDS.Pedigree(iGroup, iVar) > 0) Then
                    Me.PedigreeAssignments.Add(New cPedigreeAssignmentData(cEcopathDataStructures.PedigreeVariables(iVar), ecopathDS.Pedigree(iGroup, iVar)))
                End If
            Next

        End Sub

#End Region ' Constructor

#Region " Public properties "

        Public ReadOnly Property NumPedigreeAssignments As Integer
            Get
                Return Me.PedigreeAssignments.Count
            End Get
        End Property

#End Region ' Public properties

    End Class

#End Region ' Groups

#Region " Diets "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Base class for containing a single diet for a predator.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cDietData

#Region " Variables "

        <XmlElement("prey_seq")>
        Public Property PreyIndex As Integer

        <XmlElement("proportion")>
        Public Property Amount As Single

        <XmlElement("detritus_fate")>
        Public Property DetritusFate As Single

#End Region ' Variables

#Region " Constructor "

        Public Sub New()
            ' NOP
        End Sub

        Public Sub New(iPrey As Integer, amount As Single, detritusfate As Single)
            Me.New()
            Me.PreyIndex = iPrey
            Me.Amount = amount
            Me.DetritusFate = detritusfate
        End Sub

#End Region ' Constructor

    End Class

#End Region ' Diets

#Region " Fleets "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Base class for containing the data for a single fleet.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cFleetData

#Region " Variables "

        ''' <summary>Sequential, one-based index of a fleet.</summary>
        <XmlElement("fleet_seq")>
        Public Property Index As Integer = 0

        ''' <summary>Name of a fleet.</summary>
        <XmlElement("fleet_name")>
        Public Property Name() As String

        ''' <summary>Nominal effort of a fleet.</summary>
        <XmlElement("fleet_nominal_effort")>
        Public Property NominalEffort() As Single

        <XmlElement("fleet_color")>
        Public Property Color() As Integer

        <XmlElement("fixed_cost")>
        Public Property FixedCost As Single

        <XmlElement("sailing_cost")>
        Public Property SailCost As Single

        <XmlElement("variable_cost")>
        Public Property VarCost As Single

        <XmlArray("catch_descr")>
        <XmlArrayItem("catch")>
        Public Property Catches As New List(Of cCatchData)

        <XmlArray("discard_fate_descr")>
        <XmlArrayItem("discard_fate")>
        Public Property DiscardFates As New List(Of cDiscardFateData)

#End Region ' Variables

#Region " Construction "

        Public Sub New()
            ' NOP
        End Sub

        Public Sub New(core As cCore, iFleet As Integer)

            Dim ecopathDS As cEcopathDataStructures = core.m_EcopathData

            Me.Index = iFleet
            Me.Name = ecopathDS.FleetName(iFleet)
            Me.NominalEffort = ecopathDS.NominalEffort(iFleet)
            Me.Color = ecopathDS.FleetColor(iFleet)
            Me.FixedCost = ecopathDS.CostPct(iFleet, eCostIndex.Fixed)
            Me.SailCost = ecopathDS.CostPct(iFleet, eCostIndex.Sail)
            Me.VarCost = ecopathDS.CostPct(iFleet, eCostIndex.CUPE)

            ' Catches and landings
            Me.Catches.Clear()
            ' Discard fate
            Me.DiscardFates.Clear()

            For iGroup As Integer = 1 To core.nGroups
                If (ecopathDS.Landing(iFleet, iGroup) > 0) Then
                    Me.Catches.Add(New cCatchData(iGroup, ecopathDS.Landing(iFleet, iGroup), cCatchData.eCatchType.Landing))
                End If
                If (ecopathDS.Discard(iFleet, iGroup) > 0) Then
                    Me.Catches.Add(New cCatchData(iGroup, ecopathDS.Discard(iFleet, iGroup), cCatchData.eCatchType.Discards))
                End If
                If (ecopathDS.Market(iFleet, iGroup) > 0) Then
                    Me.Catches.Add(New cCatchData(iGroup, ecopathDS.Market(iFleet, iGroup), cCatchData.eCatchType.Market))
                End If
                If (ecopathDS.PropDiscardMort(iFleet, iGroup) > 0) Then
                    Me.Catches.Add(New cCatchData(iGroup, ecopathDS.PropDiscardMort(iFleet, iGroup), cCatchData.eCatchType.PropDiscardMort))
                End If
                If (iGroup > core.nLivingGroups) Then
                    If (ecopathDS.DiscardFate(iFleet, iGroup - core.nLivingGroups) > 0) Then
                        Me.DiscardFates.Add(New cDiscardFateData(iGroup - core.nLivingGroups, ecopathDS.DiscardFate(iFleet, iGroup - core.nLivingGroups)))
                    End If
                End If
            Next

        End Sub

#End Region ' Construction

#Region " Public properties "

        Public ReadOnly Property NumCatches As Integer
            Get
                Return Me.Catches.Count
            End Get
        End Property

        Public ReadOnly Property NumDiscardFate As Integer
            Get
                Return Me.DiscardFates.Count
            End Get
        End Property

#End Region ' Public properties

    End Class

#End Region ' Fleets

#Region " Catches "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Base class for containing a single catch for a group.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cCatchData

        Public Enum eCatchType As Byte
            Landing
            Discards
            Market
            PropDiscardMort
        End Enum

#Region " Variables "

        ''' <summary>Name of the fleet the catch belongs to.</summary>
        <XmlElement("group_seq")>
        Public Property GroupIndex As Integer

        ''' <summary>Catch value.</summary>
        <XmlElement("catch_value")>
        Public Property Amount As Single

        <XmlElement("catch_type")>
        Public Property Type As String

        ''' <summary>Interpreted <see cref="eCatchType">value</see>.</summary>
        <XmlIgnore()>
        Public Property CatchType As eCatchType
            Get
                Select Case Me.Type.ToLower()
                    Case "total landings" : Return eCatchType.Landing
                    Case "discards" : Return eCatchType.Discards
                    Case "market" : Return eCatchType.Market
                    Case "prop mort" : Return eCatchType.PropDiscardMort
                    Case Else
                        Debug.Assert(False, "Enumerated value " & Me.Type & " not supported")
                End Select
                Return eCatchType.Discards
            End Get
            Set(value As eCatchType)
                Select Case value
                    Case eCatchType.Discards : Me.Type = "discards"
                    Case eCatchType.Landing : Me.Type = "total landings"
                    Case eCatchType.Market : Me.Type = "market"
                    Case eCatchType.PropDiscardMort : Me.Type = "prop mort"
                    Case Else
                        Debug.Assert(False, "Enumerated value " & value & " not supported")
                End Select
            End Set
        End Property

#End Region ' Variables

#Region " Construction "

        Public Sub New()
            'NOP
        End Sub

        Public Sub New(iGroup As Integer, amount As Single, type As eCatchType)
            Me.GroupIndex = iGroup
            Me.Amount = amount
            Me.CatchType = type
        End Sub

#End Region ' Construction

    End Class

#End Region ' Catches

#Region " Discard fate "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Base class for containing a discard fate for a fleet/group combination.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cDiscardFateData

#Region " Variables "

        ''' <summary>Name of the fleet the catch belongs to.</summary>
        <XmlElement("group_seq")>
        Public Property GroupIndex As Integer
        <XmlElement("amount")>
        Public Property Amount As Single

#End Region ' Variables

#Region " Construction "

        Public Sub New()
            'NOP
        End Sub

        Public Sub New(iGroup As Integer, amount As Single)
            Me.GroupIndex = iGroup
            Me.Amount = amount
        End Sub

#End Region ' Construction

    End Class

#End Region ' Catches

#Region " Multi-stanza "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Base class for containing the data for a single fleet.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cStanzaData

#Region " Variables "

        ''' <summary>Sequential, one-based index of a stanza.</summary>
        <XmlElement("stanza_seq")>
        Public Property Index As Integer = 0

        ''' <summary>Name of a stanza.</summary>
        <XmlElement("stanza_name")>
        Public Property Name() As String

        <XmlArray("lifestage_descr")>
        <XmlArrayItem("lifestage")>
        Public Property LifeStages As New List(Of cStanzaLifeStageData)

        <XmlElement("leading_b")>
        Public Property LeadingB As Integer
        <XmlElement("leading_qb")>
        Public Property LeadingQB As Integer
        <XmlElement("rec_power")>
        Public Property RecPower As Single
        <XmlElement("bab_split")>
        Public Property BaBSplit As Single
        <XmlElement("w_mat_w_inf")>
        Public Property WmatWinf As Single
        <XmlElement("fixed_fecundity")>
        Public Property FixedFecundity As Boolean
        <XmlElement("egg_at_spawn")>
        Public Property EggAtSpawn As Boolean

        ' Taxon
        <XmlArray("taxon_descr")>
        <XmlArrayItem("taxon")>
        Public Property Taxonomy As New List(Of cTaxonData)

#End Region ' Variables

#Region " Construction "

        Public Sub New()
            ' NOP
        End Sub

        Public Sub New(core As cCore, iStanza As Integer)

            Dim stanzads As cStanzaDatastructures = core.m_Stanza
            Dim taxonDS As cTaxonDataStructures = core.m_TaxonData

            Me.Index = iStanza
            Me.Name = stanzads.StanzaName(iStanza)

            Me.RecPower = stanzads.RecPowerSplit(iStanza)
            Me.BaBSplit = stanzads.BABsplit(iStanza)
            Me.WmatWinf = stanzads.WmatWinf(iStanza)
            Me.FixedFecundity = stanzads.FixedFecundity(iStanza)
            Me.EggAtSpawn = stanzads.EggAtSpawn(iStanza)

            Me.LeadingB = stanzads.BaseStanza(iStanza)
            Me.LeadingQB = stanzads.BaseStanzaCB(iStanza)

            Me.LifeStages.Clear()
            For iStage As Integer = 1 To stanzads.Nstanza(iStanza)
                Me.LifeStages.Add(New cStanzaLifeStageData(core, iStanza, iStage))
            Next

            Me.Taxonomy.Clear()
            For iTaxon As Integer = 1 To taxonDS.NumTaxon
                If (taxonDS.IsTaxonStanza(iTaxon) = True) And (taxonDS.TaxonTarget(iTaxon) = iStanza) Then
                    Debug.Assert(Me.Taxonomy.Count = 0)
                    Me.Taxonomy.Add(New cTaxonData(core, iTaxon))
                End If
            Next

        End Sub

#End Region ' Construction

#Region " Public properties "

        Public ReadOnly Property NumLifeStages As Integer
            Get
                Return Me.LifeStages.Count
            End Get
        End Property

#End Region ' Public properties

    End Class

#End Region ' Multi-stanza

#Region " Multi-stanza life stage "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Base class for containing the data for a single multi-stanza life stage.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cStanzaLifeStageData

#Region " Variables "

        ''' <summary>Sequential, one-based index of a stanza.</summary>
        <XmlElement("stage_seq")>
        Public Property Index As Integer = 0
        <XmlElement("group_seq")>
        Public Property GroupIndex As Integer = 0
        <XmlElement("z")>
        Public Property Z As Single
        <XmlElement("start_age")>
        Public Property Age As Integer

#End Region ' Variables

#Region " Construction "

        Public Sub New()
            ' NOP
        End Sub

        Public Sub New(core As cCore, iStanza As Integer, iLifeStage As Integer)

            Dim stanzaDS As cStanzaDatastructures = core.m_Stanza

            Me.Index = iLifeStage
            Me.GroupIndex = stanzaDS.EcopathCode(iStanza, iLifeStage)
            Me.Z = stanzaDS.Stanza_Z(iStanza, iLifeStage)
            Me.Age = stanzaDS.Age1(iStanza, iLifeStage)

        End Sub

#End Region ' Construction

    End Class

#End Region ' Multi-stanza

#Region " Pedigree "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Class for containing a single pedigree level.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cPedigreeData

#Region " Variables "

        ''' <summary>Index of the pedigree level.</summary>
        <XmlElement("pedigree_seq")>
        Public Property Index As Integer
        <XmlElement("pedigree_name")>
        Public Property Name As String
        <XmlElement("description")>
        Public Property Description As String
        <XmlElement("pedigree_color")>
        Public Property Color As Integer

        <XmlElement("variable")>
        Public Property Variable As String

        <XmlIgnore()>
        Public Property VarName As eVarNameFlags
            Get
                Dim cin As cCoreEnumNamesIndex = cCoreEnumNamesIndex.GetInstance()
                Return cin.GetVarName(Me.Variable)
            End Get
            Set(value As eVarNameFlags)
                Dim cin As cCoreEnumNamesIndex = cCoreEnumNamesIndex.GetInstance()
                Me.Variable = cin.GetVarName(value)
            End Set
        End Property

        <XmlElement("index_value")>
        Public Property IndexValue As Single
        <XmlElement("conf_interv")>
        Public Property ConfidenceValue As Integer
        <XmlElement("estimated")>
        Public Property IsEstimated As Boolean

#End Region ' Variables

#Region " Construction "

        Public Sub New()
            'NOP
        End Sub

        Public Sub New(core As cCore, iLevel As Integer)

            Dim ecopathDS As cEcopathDataStructures = core.m_EcopathData

            Me.Index = iLevel
            Me.Name = ecopathDS.PedigreeLevelName(iLevel)
            Me.Description = ecopathDS.PedigreeLevelDescription(iLevel)
            Me.Color = ecopathDS.PedigreeLevelColor(iLevel)
            Me.VarName = ecopathDS.PedigreeLevelVarName(iLevel)
            Me.IndexValue = ecopathDS.PedigreeLevelIndexValue(iLevel)
            Me.ConfidenceValue = ecopathDS.PedigreeLevelConfidence(iLevel)
            Me.IsEstimated = ecopathDS.PedigreeLevelEstimated(iLevel)

        End Sub

#End Region ' Construction

    End Class

#End Region ' Pedigree

#Region " Pedigree assignments "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Class for containing a single pedigree level.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cPedigreeAssignmentData

#Region " Variables "

        ''' <summary>Index of the pedigree level.</summary>
        <XmlElement("pedigree_seq")>
        Public Property LevelIndex As Integer

        <XmlElement("variable")>
        Public Property Variable As String

        <XmlIgnore()>
        Public Property VarName As eVarNameFlags
            Get
                Dim cin As cCoreEnumNamesIndex = cCoreEnumNamesIndex.GetInstance()
                Return cin.GetVarName(Me.Variable)
            End Get
            Set(value As eVarNameFlags)
                Dim cin As cCoreEnumNamesIndex = cCoreEnumNamesIndex.GetInstance()
                Me.Variable = cin.GetVarName(value)
            End Set
        End Property

#End Region ' Variables

#Region " Construction "

        Public Sub New()
            'NOP
        End Sub

        Public Sub New(var As eVarNameFlags, iLevel As Integer)

            Me.VarName = var
            Me.LevelIndex = iLevel

        End Sub

#End Region ' Construction

    End Class

#End Region ' Pedigree assignments

#Region " Taxa "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Class for containing a single species.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cTaxonData

#Region " Variables "

        ''' <summary>Index of the taxon.</summary>
        <XmlElement("taxon_seq")>
        Public Property TaxonIndex As Integer
        <XmlElement("taxon_name")>
        Public Property CommonName As String
        ''' <summary>Reserved for future use.</summary>
        <XmlElement("taxon_kingdom")>
        Public Property Kingdom As String
        ''' <summary>Reserved for future use.</summary>
        <XmlElement("taxon_phylum")>
        Public Property Phylum As String
        <XmlElement("taxon_class")>
        Public Property [Class] As String
        <XmlElement("taxon_order")>
        Public Property Order As String
        <XmlElement("taxon_family")>
        Public Property Family As String
        <XmlElement("taxon_genus")>
        Public Property Genus As String
        <XmlElement("taxon_species")>
        Public Property Species As String

        ''' <summary>See <see cref="eVarNameFlags.CodeSAUP"></see></summary>
        <XmlElement("code_saup")>
        Public Property CodeSAUP As Long
        ''' <summary>See <see cref="eVarNameFlags.CodeFB"></see></summary>
        <XmlElement("code_fishbase")>
        Public Property CodeFB As Long
        ''' <summary>See <see cref="eVarNameFlags.CodeSLB"></see></summary>
        <XmlElement("code_sealifebase")>
        Public Property CodeSLB As Long
        ''' <summary>See <see cref="eVarNameFlags.CodeFAO"></see></summary>
        <XmlElement("code_fao")>
        Public Property CodeFAO As String
        ''' <summary>See <see cref="eVarNameFlags.CodeLSID"></see></summary>
        <XmlElement("code_lsid")>
        Public Property CodeLSID As String

        <XmlElement("source")>
        Public Property Source As String
        <XmlElement("source_key")>
        Public Property SourceKey As String

        ''' <summary>Northern limit of the <see cref="Extent"/>.</summary>
        <XmlIgnore()>
        Public Property North As Single
        ''' <summary>Eastern limit of the <see cref="Extent"/>.</summary>
        <XmlIgnore()>
        Public Property East As Single
        ''' <summary>Western limit of the <see cref="Extent"/>.</summary>
        <XmlIgnore()>
        Public Property West As Single
        ''' <summary>Southern limit of the <see cref="Extent"/>.</summary>
        <XmlIgnore()>
        Public Property South As Single

        ''' <summary>Spatial bounding box.</summary>
        <XmlElement("geographic_extent")>
        Public Property Extent As String
            Get
                Return "BOX(" & cStringUtils.FormatSingle(Me.West) & " " & cStringUtils.FormatSingle(Me.North) & "," & cStringUtils.FormatSingle(Me.East) & " " & cStringUtils.FormatSingle(Me.South) & ")"
            End Get
            Set(value As String)
                Dim strBits() As String = value.ToUpper().Replace("BOX(", "").Replace(",", " ").Replace(")", "").Trim().Split(" "c)
                Me.West = Single.Parse(strBits(0))
                Me.North = Single.Parse(strBits(1))
                Me.East = Single.Parse(strBits(2))
                Me.South = Single.Parse(strBits(3))
            End Set
        End Property

        <XmlElement("prop_biomass")>
        Public Property PropBiomass As Single

        <XmlElement("prop_catch")>
        Public Property PropCatch As Single

        ' -- Ecology type --

        <XmlElement("type_ecology")>
        Public Property Ecology As String

        <XmlIgnore()>
        Public Property EcologyType As eEcologyTypes
            Get
                Dim t As eEcologyTypes = eEcologyTypes.NotSet
                [Enum].TryParse(Me.Ecology, t)
                Return t
            End Get
            Set(value As eEcologyTypes)
                Me.Ecology = value.ToString
            End Set
        End Property

        ' -- Organism type --

        <XmlElement("type_organism")>
        Public Property Organism As String

        <XmlIgnore()>
        Public Property OrganismType As eOrganismTypes
            Get
                Dim t As eOrganismTypes = eOrganismTypes.NotSet
                [Enum].TryParse(Me.Organism, t)
                Return t
            End Get
            Set(value As eOrganismTypes)
                Me.Organism = value.ToString
            End Set
        End Property

        ' -- IUCN status --

        <XmlElement("iucn_status")>
        Public Property IUCNConservationStatus As String

        <XmlIgnore()>
        Public Property IUCNConservationStatusType As eIUCNConservationStatusTypes
            Get
                Dim t As eIUCNConservationStatusTypes = eIUCNConservationStatusTypes.NotSet
                [Enum].TryParse(Me.IUCNConservationStatus, t)
                Return t
            End Get
            Set(value As eIUCNConservationStatusTypes)
                Me.IUCNConservationStatus = value.ToString
            End Set
        End Property

        ' -- Exploitation type --

        <XmlElement("type_exploitation")>
        Public Property ExploitationStatus As String

        <XmlIgnore()>
        Public Property ExploitationStatusType As eExploitationTypes
            Get
                Dim t As eExploitationTypes = eExploitationTypes.NotSet
                [Enum].TryParse(Me.ExploitationStatus, t)
                Return t
            End Get
            Set(value As eExploitationTypes)
                Me.ExploitationStatus = value.ToString
            End Set
        End Property

        ' -- Occurrence --

        <XmlElement("type_occurrence")>
        Public Property OccurrenceStatus As String

        <XmlIgnore()>
        Public Property OccurrenceStatusType As eOccurrenceStatusTypes
            Get
                Dim t As eOccurrenceStatusTypes = eOccurrenceStatusTypes.NotSet
                [Enum].TryParse(Me.OccurrenceStatus, t)
                Return t
            End Get
            Set(value As eOccurrenceStatusTypes)
                Me.OccurrenceStatus = value.ToString
            End Set
        End Property

        <XmlElement("vulnerability_index")>
        Public Property VulnerabilityIndex As Integer
        <XmlElement("weight_mean")>
        Public Property MeanWeight As Single
        <XmlElement("length_mean")>
        Public Property MeanLength As Single
        <XmlElement("length_max")>
        Public Property MaxLength As Single
        <XmlElement("lifespan_mean")>
        Public Property MeanLifeSpan As Single
        <XmlElement("weight_at_inf")>
        Public Property Winf As Single
        <XmlElement("vbk")>
        Public Property vbk As Single

#End Region ' Variables

#Region " Construction "

        Public Sub New()
            'NOP
        End Sub

        Public Sub New(core As cCore, iTaxon As Integer)

            Dim taxonDS As cTaxonDataStructures = core.m_TaxonData

            Me.TaxonIndex = iTaxon
            Me.CommonName = taxonDS.TaxonName(iTaxon)

            Me.Kingdom = ""
            Me.Phylum = ""
            Me.Class = taxonDS.TaxonClass(iTaxon)
            Me.Order = taxonDS.TaxonOrder(iTaxon)
            Me.Family = taxonDS.TaxonFamily(iTaxon)
            Me.Genus = taxonDS.TaxonGenus(iTaxon)
            Me.Species = taxonDS.TaxonSpecies(iTaxon)

            Me.CodeSAUP = taxonDS.TaxonCodeSAUP(iTaxon)
            Me.CodeFB = taxonDS.TaxonCodeFB(iTaxon)
            Me.CodeSLB = taxonDS.TaxonCodeSLB(iTaxon)
            Me.CodeFAO = taxonDS.TaxonCodeFAO(iTaxon)
            Me.CodeLSID = taxonDS.TaxonCodeLSID(iTaxon)

            Me.Source = taxonDS.TaxonSource(iTaxon)
            Me.SourceKey = taxonDS.TaxonSourceKey(iTaxon)
            Me.North = taxonDS.TaxonNorth(iTaxon)
            Me.West = taxonDS.TaxonWest(iTaxon)
            Me.South = taxonDS.TaxonSouth(iTaxon)
            Me.East = taxonDS.TaxonEast(iTaxon)

            Me.PropBiomass = taxonDS.TaxonPropBiomass(iTaxon)
            Me.PropCatch = taxonDS.TaxonPropCatch(iTaxon)

            Me.EcologyType = taxonDS.TaxonEcologyType(iTaxon)
            Me.OrganismType = taxonDS.TaxonOrganismType(iTaxon)
            Me.IUCNConservationStatusType = taxonDS.TaxonIUCNConservationStatus(iTaxon)
            Me.ExploitationStatusType = taxonDS.TaxonExploitationStatus(iTaxon)
            Me.OccurrenceStatusType = taxonDS.TaxonOccurrenceStatus(iTaxon)

            Me.VulnerabilityIndex = taxonDS.TaxonVulnerabilityIndex(iTaxon)
            Me.MeanWeight = taxonDS.TaxonMeanWeight(iTaxon)
            Me.MeanLength = taxonDS.TaxonMeanLength(iTaxon)
            Me.MaxLength = taxonDS.TaxonMaxLength(iTaxon)
            Me.MeanLifeSpan = taxonDS.TaxonMeanLifeSpan(iTaxon)
            Me.Winf = taxonDS.TaxonWinf(iTaxon)
            Me.vbk = taxonDS.TaxonK(iTaxon)

        End Sub

#End Region ' Construction

    End Class

#End Region ' Taxa

#Region " cEcobaseModelParameters "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Base class for containing the data for a single model, as received from
    ''' EcoBase
    ''' </summary>
    ''' -----------------------------------------------------------------------
    <XmlRoot("EcoBaseModel")>
    Public Class cEcobaseModelParameters
        Private Shared ReadOnly m_logger As ILogger = LoggingContext.CreateLogger(Of cEcobaseModelParameters)()

#Region " Variables "

        ''' <summary>The <see cref="cModelData"/>.</summary>
        <XmlElement("model_descr")>
        Public Property Model As cModelData

        ''' <summary>The list of <see cref="cGroupData">groups</see>.</summary>
        <XmlArray("group_descr")>
        <XmlArrayItem("group")>
        Public Groups As New List(Of cGroupData)

        ''' <summary>The list of <see cref="cFleetData">fleets</see>.</summary>
        <XmlArray("fleet_descr")>
        <XmlArrayItem("fleet")>
        Public Fleets As New List(Of cFleetData)

        ''' <summary>The list of <see cref="cStanzaData">multi-stanza groups</see>.</summary>
        <XmlArray("stanza_descr")>
        <XmlArrayItem("stanza")>
        Public Stanzas As New List(Of cStanzaData)

        ''' <summary>The list of <see cref="cPedigreeData">pedigree levels</see>.</summary>
        <XmlArray("pedigree_descr")>
        <XmlArrayItem("pedigree")>
        Public PedigreeLevels As New List(Of cPedigreeData)

#End Region ' Variables

#Region " Construction "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Default contructor
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Sub New()
            ' NOP
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor, loads an instance from the currently loaded model.
        ''' </summary>
        ''' <param name="core">The core that has the loaded model.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(core As cCore)

            ' Sanity checks
            Debug.Assert(core.StateMonitor.HasEcopathLoaded(), "Ecopath not loaded, cannot continue")
            Debug.Assert(core.IsModelBalanced(), "Ecopath not balanced, cannot continue")

            Me.Model = New cModelData(core)

            For iGroup As Integer = 1 To core.nGroups
                Me.Groups.Add(New cGroupData(core, iGroup))
            Next

            For iFleet As Integer = 1 To core.nFleets
                Me.Fleets.Add(New cFleetData(core, iFleet))
            Next

            For iStanza As Integer = 1 To core.nStanzas
                Me.Stanzas.Add(New cStanzaData(core, iStanza))
            Next

            For iPedigree As Integer = 1 To core.m_EcopathData.NumPedigreeLevels
                Me.PedigreeLevels.Add(New cPedigreeData(core, iPedigree))
            Next

        End Sub

#End Region ' Construction

#Region " Shared access "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Factory method, create a cEcobaseData instance from WSDL output.
        ''' </summary>
        ''' <param name="strModel"></param>
        ''' <returns>A cEcobaseData instance, or nothing if an error occurred.</returns>
        ''' -------------------------------------------------------------------
        Public Shared Function FromXML(strModel As String) As cEcobaseModelParameters

            ' Clean up
            If (String.IsNullOrWhiteSpace(strModel)) Then Return Nothing

#If DEBUG Then
            ' Store original XML for debugging purposes
            Using w As New StreamWriter("EcobaseModelParameters_org.xml")
                w.Write(strModel)
                w.Close()
            End Using
#End If

            'strModel = strModel.Replace(""" & vbLF && """, "")
            strModel = strModel.Replace("ETinputtot", "cEcobaseModelParameters")

            Dim reader As New StringReader(strModel)
            Dim serializer As New XmlSerializer(GetType(cEcobaseModelParameters))
            Dim selfie As cEcobaseModelParameters = Nothing

            Try
                selfie = CType(serializer.Deserialize(reader), cEcobaseModelParameters)
            Catch ex As Exception
                ' Hmm
                m_logger.LogError(ex, "cEcobaseModelParameters.FromXML")
            End Try

            If (selfie.Model Is Nothing) Then Return Nothing

#If DEBUG Then
            ' Store cleaned XML for debugging purposes
            Dim doc As New Xml.XmlDocument()
            doc.LoadXml(strModel)
            doc.Save("EcobaseModelParameters_in.xml")
            ' Store processed XML for debugging purposes
            doc.LoadXml(cEcobaseModelParameters.ToXML(selfie))
            doc.Save("EcobaseModelParameters_processed.xml")
#End If
            Return selfie

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Convert a cEcobaseData instance to a chunk of XML for submission to EcoBase
        ''' </summary>
        ''' <param name="data"></param>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Public Shared Function ToXML(data As cEcobaseModelParameters) As String

            Dim writerText As New cFlexibleEncodingStringWriter()
            writerText.CustomEncoding = System.Text.Encoding.UTF8
            Dim writerXML As XmlWriter = XmlWriter.Create(writerText)
            Dim serializer As New XmlSerializer(GetType(cEcobaseModelParameters))
            serializer.Serialize(writerXML, data)
            Return writerText.ToString()

        End Function

#End Region ' Shared access

#Region " Public properties "

        Public ReadOnly Property NumGroups As Integer
            Get
                Return Me.Groups.Count
            End Get
        End Property

        Public ReadOnly Property NumLiving As Integer
            Get
                Dim iNumLiving As Integer = 0
                For Each gd As cGroupData In Me.Groups
                    If gd.PP < 2 Then iNumLiving += 1
                Next
                Return iNumLiving
            End Get
        End Property

        Public ReadOnly Property NumFleets As Integer
            Get
                Return Me.Fleets.Count
            End Get
        End Property

        Public ReadOnly Property NumStanza As Integer
            Get
                Return Me.Stanzas.Count
            End Get
        End Property

        Public ReadOnly Property NumPedigree As Integer
            Get
                Return Me.PedigreeLevels.Count
            End Get
        End Property

#End Region ' Public properties

    End Class

#End Region ' cEcobaseModelParameters

#Region " cEcobaseModelList "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Base class for containing a list of models received from EcoBase.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    <XmlRoot("EcoBaseModels")>
    Public Class cEcobaseModelList
        Private Shared ReadOnly m_logger As ILogger = LoggingContext.CreateLogger(Of cEcobaseModelList)()

#Region " Variables "

        ''' <summary>The list of <see cref="cModelData"/> for all models in EcoBase.</summary>
        <XmlArray("model_descr")>
        <XmlArrayItem("model")>
        Public Property Models As New List(Of cModelData)

#End Region ' Variables

#Region " Construction "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Deafult hidden constructor.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub New()
            ' NOP
        End Sub

#End Region ' Construction

#Region " Shared access "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Factory method, create a cEcobaseData instance from WSDL output.
        ''' </summary>
        ''' <param name="strModelsList">Models list XML from EcoBase.</param>
        ''' <returns>A cEcobaseData instance, or nothing if an error occurred.</returns>
        ''' -------------------------------------------------------------------
        Public Shared Function FromXML(strModelsList As String) As cEcobaseModelList

            ' Clean up
            If (String.IsNullOrWhiteSpace(strModelsList)) Then Return Nothing

            'strModelsList = strModelsList.Replace("<ETinputtot>", "<?xml version=""1.0"" encoding=""utf-8""?><ETinputtot xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">")
            'strModelsList = strModelsList.Replace("ETinputtot", "EcoBaseModels")
            'strModelsList = strModelsList.Replace(""" & vbLF && """, "")

#If DEBUG Then
            ' Store original XML for debugging purposes
            Using w As New StreamWriter("EcobaseModelList_org.xml")
                w.Write(strModelsList)
                w.Close()
            End Using
#End If

            Dim reader As New StringReader(strModelsList)
            Dim serializer As New XmlSerializer(GetType(cEcobaseModelList))
            Dim selfie As cEcobaseModelList = Nothing

            Try
                selfie = CType(serializer.Deserialize(reader), cEcobaseModelList)
            Catch ex As Exception
                ' Hmm
                m_logger.LogError(ex, "cEcobaseModelList.FromXML")
            End Try

#If DEBUG Then
            ' Store cleaned XML for debugging purposes
            Dim doc As New Xml.XmlDocument()
            doc.LoadXml(strModelsList)
            doc.Save("EcobaseModelList_in.xml")
#End If
            Return selfie

        End Function

#End Region ' Shared access

#Region " Public properties "

        Public ReadOnly Property NumModels As Integer
            Get
                Return Me.Models.Count
            End Get
        End Property

#End Region ' Public properties

    End Class

#End Region ' cEcobaseModelList

End Namespace
