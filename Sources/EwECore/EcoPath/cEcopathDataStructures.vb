Option Strict Off ' OUCH
Imports EwEUtils.Core

''' <summary>
''' Wrapper for the underlying data structures of the EcoPath model. 
''' Provides a way to wrap all the data from EcoPath into one place
''' </summary>
Public Class cEcopathDataStructures

#Region "Private data"

    Private m_messages As cMessagePublisher

#End Region

#Region " Public Variables "

    Public bInitialized As Boolean = False

    Public ModelDBID As Integer = 0
    Public ModelName As String = ""
    Public ModelDescription As String = ""
    Public ModelArea As Single = 0
    Public ModelNumDigits As Integer = 0
    Public ModelGroupDigits As Boolean = False
    Public ModelUnitTime As eUnitTimeType = 0
    Public ModelUnitTimeCustom As String = ""
    Public ModelUnitCurrency As eUnitCurrencyType = eUnitCurrencyType.NotSet
    Public ModelUnitCurrencyCustom As String = ""
    Public ModelUnitMonetary As String = ""
    Public ModelUnitArea As eUnitAreaType = 0
    Public ModelUnitAreaCustom As String = ""
    Public ModelAuthor As String = ""
    Public ModelContact As String = ""
    Public ModelLastSaved As Double = 0
    Public ModelAreaName As String = ""
    Public ModelSouth As Single = 0
    Public ModelNorth As Single = 0
    Public ModelWest As Single = 0
    Public ModelEast As Single = 0
    Public FirstYear As Integer = Date.Now.Year
    Public NumYears As Integer = 1

    ''' <summary>Group names.</summary>
    ''' <remarks>In EwE5, group names were used to identify groups. In EwE6 this 
    ''' is done via <see cref="GroupDBID">unique IDs</see></remarks>
    Public GroupName() As String ' was Specie()
    ''' <summary>Group Database ID - uniquely identifies a group.</summary>
    Friend GroupDBID() As Integer

    ''' <summary>Number of Ecosim scenarios available in a loaded model.</summary>
    Public NumEcosimScenarios As Integer
    ''' <summary>Array of Ecosim scenario names.</summary>
    Public EcosimScenarioName() As String
    ''' <summary>Array of Ecosim scenario database IDs.</summary>
    Public EcosimScenarioDBID() As Integer
    ''' <summary>Array of Ecosim scenario descriptions.</summary>
    Public EcosimScenarioDescription() As String
    ''' <summary>Array of Ecosim scenario authors.</summary>
    Public EcosimScenarioAuthor() As String
    ''' <summary>Array of Ecosim scenario contacts.</summary>
    Public EcosimScenarioContact() As String
    ''' <summary>Array of Ecosim scenario save dates (in julian day format).</summary>
    Public EcosimScenarioLastSaved() As Double
    ''' <summary>Index of active Ecosim scenario.</summary>
    Public ActiveEcosimScenario As Integer = cCore.NULL_VALUE

    ''' <summary>Number of Ecospace scenarios available in a loaded model.</summary>
    Public NumEcospaceScenarios As Integer
    ''' <summary>Array of Ecospace scenario names.</summary>
    Public EcospaceScenarioName() As String
    ''' <summary>Array of Ecospace scenario database IDs.</summary>
    Public EcospaceScenarioDBID() As Integer
    ''' <summary>Array of Ecospace scenario descriptions.</summary>
    Public EcospaceScenarioDescription() As String
    ''' <summary>Array of Ecospace scenario authors.</summary>
    Public EcospaceScenarioAuthor() As String
    ''' <summary>Array of Ecospace scenario contacts.</summary>
    Public EcospaceScenarioContact() As String
    ''' <summary>Array of Ecospace scenario save dates (in julian day format).</summary>
    Public EcospaceScenarioLastSaved() As Double
    ''' <summary>Index of active Ecospace scenario.</summary>
    Public ActiveEcospaceScenario As Integer = cCore.NULL_VALUE

    ''' <summary>Number of Ecotracer scenarios available in a loaded model.</summary>
    Public NumEcotracerScenarios As Integer
    ''' <summary>Array of Ecotracer scenario names.</summary>
    Public EcotracerScenarioName() As String
    ''' <summary>Array of Ecotracer scenario database IDs.</summary>
    Public EcotracerScenarioDBID() As Integer
    ''' <summary>Array of Ecotracer scenario descriptions.</summary>
    Public EcotracerScenarioDescription() As String
    ''' <summary>Array of Ecotracer scenario authors.</summary>
    Public EcotracerScenarioAuthor() As String
    ''' <summary>Array of Ecotracer scenario contacts.</summary>
    Public EcotracerScenarioContact() As String
    ''' <summary>Array of Ecotracer scenario save dates (in julian day format).</summary>
    Public EcotracerScenarioLastSaved() As Double
    ''' <summary>Index of active Ecotracer scenario.</summary>
    Public ActiveEcotracerScenario As Integer = cCore.NULL_VALUE

    ''' <summary>Biomass (computed)</summary>
    Public B() As Single
    ''' <summary>Biomass in habitat area (t/km²)</summary>
    Public BH() As Single
    ''' <summary>Biomass accumulation (t/km²/year)</summary>
    Public BA() As Single
    ''' <summary>Biomass accumulation / biomass</summary>
    Public BaBi() As Single
    ''' <summary>Production / biomass (/year)</summary>
    Public PB() As Single
    ''' <summary>Consumption / biomass (/year)</summary>
    Public QB() As Single
    ''' <summary>Ecotrophic efficiency (ratio)</summary>
    Public EE() As Single
    ''' <summary>Production / consumption (ratio)</summary>
    ''' <remarks>Fraction of the production that is passed up in the food web.</remarks>
    Public GE() As Single
    ''' <summary>Unassimilation / consumption (ratio)</summary>
    ''' <remarks>Fraction of the food that is not assimilated.</remarks>
    Public GS() As Single

    'Input Values are user entered values.
    'Inputs are the values that can be edited by a user, get saved to the database and displayed as basic inputs
    'each array will have a companion used for modeling that does not have 'input' i.e. EEinput() and EE() 
    'the input values are copied into the modeling array whenever the ecopath model is run CopyInputToModelArrays(...) 
    'these values are exposed via cEcoPathGroupOutputs

    ''' <summary>Ecotrophic efficiency (ratio) - original user input value of <see cref="EE">EE</see>.</summary>
    Public EEinput() As Single
    ''' <summary>Production / biomass (/year) - original user input of <see cref="PB">PB</see>.</summary>
    Public PBinput() As Single
    ''' <summary>Consumption / biomass (/year) - original user input of <see cref="QB">QB</see>.</summary>
    Public QBinput() As Single
    ''' <summary>Production / consumption (ratio) - original user input of <see cref="GE">GE</see>.</summary>
    Public GEinput() As Single

    ''' <summary>Biomass (input value)- original user input of <see cref="B">B</see>.</summary>
    Public Binput() As Single

    ''' <summary>Biomass habitat area (input value)- original user input of <see cref="BH">BH</see>.</summary>
    Public BHinput() As Single

    Private min_B_QB As Single 'minimum B*QB

    ''' <summary>Total number of groups (living and detritus)</summary>
    Public NumGroups As Integer
    ''' <summary>Total number of living groups.</summary>
    Public NumLiving As Integer
    ''' <summary>Total number of detritus groups.</summary>
    Public NumDetrit As Integer
    ''' <summary>Total number of fleets.</summary>
    Public NumFleet As Integer
    ''' <summary>Index of current selected currency units.</summary>
    Public currUnitIndex As Integer = eUnitCurrencyType.WetWeight
    ''' <summary>User-provided name for time units.</summary>
    Public TimeUnitName As String
    ''' <summary>Index of current selected time unit.</summary>
    Public TimeUnitIndex As Integer
    ''' <summary>Flag stating whether diets have been modified since the last time Ecopath has ran.</summary>
    Public DietsModified As Boolean
    Public PProd As Single

    Public DietChanged(,) As Integer

    Public Ex() As Single

    ''' <summary>Sum (per <see cref="NumGroups">NumGroups</see>) of landings + discards.</summary>
    ''' <remarks>Computed in Catch_calculations(). was called Catch but this causes a naming conflict with Try Catch blocks</remarks>
    Public fCatch() As Single '
    ''' <summary>User input matrix for Diet composition(<see cref="NumGroups">Pred</see>, <see cref="NumGroups">Prey</see>) (ratio), a <see cref="NumGroups">NumGroups</see> * <see cref="NumGroups">NumGroups</see>
    ''' matrix of species consumption ratios.</summary>
    Public DCInput(,) As Single
    ''' <summary>Diet composition(per <see cref="NumGroups">pred</see>, <see cref="NumGroups">prey</see>) (ratio), a <see cref="NumGroups">NumGroups</see> * <see cref="NumGroups">NumGroups</see>
    ''' matrix of species consumption ratios.</summary>
    Public DC(,) As Single
    ''' <summary>Detritus fate(per <see cref="NumGroups">NumGroups</see>, <see cref="NumDetrit">NumDetrit</see>) (ratio)</summary>
    ''' <remarks>Matrix describing where to direct surplus detritus.</remarks>
    Public DF(,) As Single
    ''' <summary>Area (<see cref="NumGroups">NumGroups</see>)</summary>
    ''' <remarks>Fraction of the Area where a group occurs.</remarks>
    Public Area() As Single
    ''' <summary>Diet (<see cref="NumGroups">pred</see>, <see cref="NumGroups">prey</see>) change flags.</summary>
    Public DCChanged(,) As Boolean         'Diet composition

    Public BQB() As Single
    ''' <summary>All non-usable 'model currency' that leaves the box represented by a group.</summary>
    Public Resp() As Single
    Public PP() As Single           'TM Trophic Mode
    Public det(,) As Single '(50, 50)  
    ''' <summary>Diet Composition of Detritus  for fishery.</summary>
    Public DCDet(,) As Single
    Public DetEaten() As Single                 ' For multiple detritus
    Public DetPassedOn() As Single              ' For multiple detritus
    Public DetPassedProp() As Single              ' For multiple detritus
    Public FlowToDet() As Single
    Public InputToDet() As Single

    ''' <summary>Migration into the area covered by the model (t/km²/year)</summary>
    ''' <remarks>Note that migration is not the same as import, refer to the manual for details.</remarks>
    Public Immig() As Single
    ''' <summary>Emigration out of the area covered by the model (t/km²/year)</summary>
    Public Emigration() As Single
    ''' <summary>Emigration relative to biomass (ratio)</summary>
    Public Emig() As Single    'relative to biomass, used in Ecosim
    Public Shadow() As Single
    ''' <summary>States which groups are fishes.</summary>
    Public GroupIsFish() As Boolean
    ''' <summary>States which groups are invertebrates.</summary>
    Public GroupIsInvert() As Boolean
    ' Public GrpsToShow() As Boolean
    Public PropLanded(,) As Single
    Public TTLX() As Single    'Trophic levels in Ecopath
    'Public TLSim() As Single    'These TL's are recalculated for each time step in Ecosim
    'JS 08Jan09: LHS was a global scratch variable, changed to local scope
    'Public LHS(,) As Single
    Public NumCatchCodes As Integer = 30
    Public CatchCode(,) As Integer
    Public CVpar(,) As Single
    Public M0() As Single
    Public M2() As Single
    Public Path() As Integer
    Public LastComp() As Integer
    '  Public SpeciesCode(,) As Integer '0: Ecopath group no for this stanza, 1: Ecopath no for leading B stanza, 2: Ecopath no for leading QB stanza
    ''' <summary>Detritus import (ratio)</summary>
    Public DtImp() As Single
    Public StanzaGroup() As Boolean 'Dim: numgroups, True if this is a group with stanza's

    'fishing variables
    Public NoGearData As Boolean
    ''' <summary> cost(nFleets,3) '1 is fixed cost, 2 is cost per unit effort, 3 sailing cost </summary>
    Public cost(,) As Single
    Public CostPct(,) As Single

    ''' <summary> discarded biomass by fleet group </summary>
    Public Discard(,) As Single
    Public DiscardFate(,) As Single
    Public FleetName() As String
    Friend FleetDBID() As Integer

    ''' <summary> landings biomass by fleet group </summary>
    Public Landing(,) As Single
    Public Market(,) As Single
    Public PropDiscard(,) As Single
    ''' <summary>Proportion of regulated discards that die (by gear group)</summary>
    Public PropDiscardMort(,) As Single ' gear group 0-1

    'summary stats
    'populated after parameters have been estimated in EcoPath
    'by the routines
    'ComputeFisheriesStats()
    'Compute_M2_Resp_and_Stats()
    'ComputeMoreStats()

    Public RTZ As Single 'sum of respiration
    Public Consum As Single
    Public SumBio As Single
    Public CatchSum As Single 'sum of catch
    Public GEff As Single 'gross efficiency
    Public Totpp As Single
    Public TLcatch As Single
    Public Dt As Single 'total flow of detritus
    Public SumEx As Single 'sum of exports
    Public SumP As Single 'Sum of all production
    Public Conn As Single 'Connectance Index
    Public SysOm As Single
    Public LandingValue As Single
    Public ShadowValue As Single
    Public Fixed As Single
    Public Variab As Single

    Public vbK() As Single 'VBGF curvature parameter K (/year)
    Public Hlap(,) As Single
    Public Plap(,) As Single
    Public GroupColor() As Integer
    Public FleetColor() As Integer
    Public Host(,) As Single  'last is for fishery (combined only)

    ' -- Pedigree

    Public NumPedigreeLevels As Integer
    Public PedigreeLevelDBID() As Integer
    Public PedigreeLevelName() As String
    Public PedigreeLevelColor() As Integer
    Public PedigreeLevelDescription() As String
    Public PedigreeLevelVarName() As eVarNameFlags
    ''' <summary>Index value expressed in ratio [0, 1]</summary>
    Public PedigreeLevelIndexValue() As Single
    ''' <summary>Confidence interval expressed in rounded percentages</summary>
    Public PedigreeLevelConfidence() As Integer
    Public PedigreeLevelEstimated() As Boolean
    ''' <summary>Array [#groups, #supported vars] = pedigree index.</summary>
    Public Pedigree(,) As Integer
    ''' <summary>One-based array of variables supported by the pedigree system.</summary>
    Public PedigreeVariables As eVarNameFlags() = {eVarNameFlags.NotSet, eVarNameFlags.Biomass, eVarNameFlags.PBInput, eVarNameFlags.QBInput}
    'Public PedigreeVariables As eVarNameFlags() = {eVarNameFlags.NotSet, eVarNameFlags.Biomass, eVarNameFlags.PBInput, eVarNameFlags.QBInput, eVarNameFlags.DietComp, eVarNameFlags.TCatchInput}
    Public NumPedigreeVariables As Integer = Me.PedigreeVariables.Length - 1

    Public PedigreeStatsModelIndex As Single
    Public PedigreeStatsTStar As Single

    ''' <summary>Total number of taxonomy codes.</summary>
    Public NumTaxon As Integer = 0
    ''' <summary>Taxonomy code DBID (xNumTaxa).</summary>
    Public TaxonDBID() As Integer
    ''' <summary>Group taxon assignments (xNumTaxa) -> iGroup</summary>
    Public TaxonGroup() As Integer
    ''' <summary>Group taxon proportions (xNumTaxa)</summary>
    Public TaxonGroupProp() As Single
    ''' <summary>Taxonomy class names (xNumTaxa).</summary>
    Public TaxonClass() As String
    ''' <summary>Taxonomy order names (xNumTaxa).</summary>
    Public TaxonOrder() As String
    ''' <summary>Taxonomy family names (xNumTaxa).</summary>
    Public TaxonFamily() As String
    ''' <summary>Taxonomy genus names (xNumTaxa).</summary>
    Public TaxonGenus() As String
    ''' <summary>Taxonomy species names (xNumTaxa).</summary>
    Public TaxonSpecies() As String
    ''' <summary>Taxonomy common names (xNumTaxa).</summary>
    Public TaxonCommonName() As String
    ''' <summary>Taxonomy ISCAAP codes (xNumTaxa).</summary>
    Public TaxonCodeISCAAP() As String
    ''' <summary>Taxonomy taxon names (xNumTaxa).</summary>
    Public TaxonCodeTaxon() As String
    ''' <summary>Taxonomy 3A names (xNumTaxa).</summary>
    Public TaxonCode3A() As String
    ''' <summary>Taxonomy source names where Taxon information was derived from (xNumTaxa).</summary>
    Public TaxonSource() As String
    ''' <summary>Taxonomy source keys to access Taxon information in <see cref="TaxonSource">a source</see>(xNumTaxa).</summary>
    Public TaxonSourceKey() As String
    ''' <summary>Taxonomy last updated dates (xNumTaxa) in julian day format.</summary>
    Public TaxonLastUpdated() As Double
    ''' <summary>Northern limit of taxon occurrence bounding box</summary>
    Public TaxonNorth() As Single
    ''' <summary>Southern limit of taxon occurrence bounding box</summary>
    Public TaxonSouth() As Single
    ''' <summary>Eastern limit of taxon occurrence bounding box</summary>
    Public TaxonEast() As Single
    ''' <summary>Western limit of taxon occurrence bounding box</summary>
    Public TaxonWest() As Single

    ''' <summary>
    ''' Number of missing variables per groups
    ''' </summary>
    ''' <remarks>These are the variables that need to be computed be Ecopath</remarks>
    Public mis() As Integer

#End Region

#Region " Borrowed from EcoRanger "

    ' Borrowed from EcoRanger for Chesson calculation since this calculation is required
    ' for generating Ecopath output data.
    Public SumR() As Single
    Public Alpha(,) As Single

#End Region ' Borrowed from EcoRanger

#Region "Redimensioning"

    ''' <summary>
    ''' Redim All variables that in EcoPath that have an NGroup dimension
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>This act as a central location to change the number of groups in the EcoPath data</remarks>
    Public Function redimGroups() As Boolean

        Try

            redimGroupVariables() 'just ngroup variables
            RedimFleetVariables(True) 'fleets clear out the values
            RedimTaxon()
            Return True

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".redimGroups Error: " & ex.Message)
        End Try


    End Function

    ''' <summary>
    ''' redimension array variables 
    ''' called when a new model is loaded
    ''' </summary>
    ''' <returns></returns>
    ''' True if no error
    ''' <remarks></remarks>
    Public Function redimGroupVariables() As Boolean
        Dim i As Integer, j As Integer
        NumDetrit = NumGroups - NumLiving

        ' EstimateWhat(NumGroups)

        ReDim PB(NumGroups)
        ReDim EE(NumGroups)
        ReDim QB(NumGroups)
        ReDim GE(NumGroups)
        ReDim B(NumGroups)
        ReDim BH(NumGroups)    'habitat biomass

        ReDim GEinput(NumGroups)
        ReDim PBinput(NumGroups)
        ReDim EEinput(NumGroups)
        ReDim QBinput(NumGroups)
        ReDim Binput(NumGroups)
        ReDim BHinput(NumGroups)

        ReDim Ex(NumGroups)
        ReDim fCatch(NumGroups)
        ReDim Area(NumGroups)
        For i = 1 To NumGroups
            Area(i) = 1
        Next
        ReDim BA(NumGroups)
        ReDim BaBi(NumGroups)
        ReDim DCInput(NumGroups + 1, NumGroups + 1)
        ReDim DC(NumGroups + 1, NumGroups + 1)
        ReDim DCChanged(NumGroups + 1, NumGroups + 1) 'jb added to tell the core which diet comp values where changed
        ReDim PP(NumGroups)
        ReDim GroupName(NumGroups)
        ReDim GroupDBID(NumGroups)
        ReDim GS(NumGroups)
        ReDim TTLX(NumGroups)     'Trophic levels in Ecopath
        'JS 08Jan09: SumDC and LHS were a global scratch variable, changed to local scope
        'ReDim LHS(NumGroups, NumGroups)
        'ReDim SumDC(NumGroups)
        ReDim BQB(NumGroups)

        ReDim Resp(NumGroups)
        ReDim DF(NumGroups, NumGroups - NumLiving)

        ReDim DtImp(NumGroups)
        ReDim DetEaten(NumGroups)
        ReDim DetPassedOn(NumGroups)
        ReDim DetPassedProp(NumGroups)
        ReDim InputToDet(NumGroups)
        ReDim M0(NumGroups)
        ReDim M2(NumGroups)
        ReDim Path(2 * NumGroups + 2)
        ReDim LastComp(2 * NumGroups + 1)
        ReDim Immig(NumGroups)
        ReDim Emigration(NumGroups)
        ReDim Emig(NumGroups)
        ReDim Shadow(NumGroups)
        ReDim GroupIsFish(NumGroups)
        ReDim GroupIsInvert(NumGroups)
        ReDim PropLanded(NumFleet, NumGroups)

        ReDim Host(NumGroups, NumGroups)
        ReDim Hlap(NumGroups, NumGroups)
        ReDim Plap(NumGroups, NumGroups)
        ReDim GroupColor(NumGroups)

        ReDim SumR(NumGroups)
        ReDim Alpha(NumGroups, NumGroups)
        ReDim vbK(NumGroups)

        'ReDim GrpsToShow(NumGroups + NumFleet + 2)

        'For i = 1 To NumGroups + NumFleet
        '    GrpsToShow(i) = True
        'Next

        'For i = NumGroups + NumFleet + 1 To NumGroups + NumFleet + 2
        '    GrpsToShow(i) = False
        'Next

        NumCatchCodes = 30
        ReDim CatchCode(NumCatchCodes, NumGroups)
        ReDim CVpar(5, NumGroups)

        For i = 1 To NumGroups
            For j = 0 To 4
                CVpar(j, i) = 0.1
            Next j
            CVpar(5, i) = 0.05
        Next i

        'Stanzagroup  needed when importing eii files
        ReDim StanzaGroup(NumGroups)

        ReDim mis(NumGroups)

        ' GearVariables(True)
        '   CinfoDeclare()    'The variables for Ecotracer: all using numgroups

        Return True
    End Function


    ''' <summary>
    ''' Redimension all fishing variables
    ''' </summary>
    ''' <param name="NoPreserve">
    ''' A flag to keep the existing values in the arrays 
    ''' True means do NOT keep the original values NO preserve.
    ''' False to KEEP the values.
    ''' </param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function RedimFleetVariables(ByVal NoPreserve As Boolean) As Boolean

        Dim bNeedDefaultFleet As Boolean = False ' (NumFleet = 0)

        ' Always need one fleet
        If (bNeedDefaultFleet) Then
            NumFleet = 1
        End If

        'det() is not saved to database
        ReDim det(NumGroups + NumFleet, NumGroups + NumFleet)
        If NoPreserve Then
            ReDim DCDet(NumGroups - NumLiving, NumFleet)        'Diet composition of detritus
            ReDim FlowToDet(NumGroups + NumFleet)
        Else
            ReDim Preserve DCDet(NumGroups - NumLiving, NumFleet)       'Diet composition of detritus
            ReDim Preserve FlowToDet(NumGroups + NumFleet)
        End If
        'Next in Gear
        ReDim cost(NumFleet, 3)       '1 is fixed cost, 2 is cost per unit effort, 3 sailing cost
        ReDim CostPct(NumFleet, 3)       '1 is fixed cost, 2 is cost per unit effort, 3 sailing cost
        ReDim FleetName(NumFleet + 1)
        ReDim FleetDBID(NumFleet + 1)
        'Next in Catch
        ReDim Landing(NumFleet, NumGroups)
        ReDim Discard(NumFleet, NumGroups)
        ReDim DiscardFate(NumFleet, NumGroups - NumLiving)
        ReDim PropLanded(NumFleet, NumGroups)
        ReDim PropDiscard(NumFleet, NumGroups)
        ReDim PropDiscardMort(NumFleet, NumGroups)
        ReDim Market(NumFleet, NumGroups)
        ReDim FleetColor(NumFleet)

        If (bNeedDefaultFleet) Then
            ' Populate default fleet
            FleetName(1) = My.Resources.CoreDefaults.CORE_DEFAULT_FLEET()
            FleetDBID(1) = 1
            CostPct(1, eCostIndex.Fixed) = 0
            CostPct(1, eCostIndex.CUPE) = 100
            CostPct(1, eCostIndex.Sail) = 0
            For iGroup As Integer = 1 To NumLiving
                ' Set default landing
                Landing(1, iGroup) = fCatch(iGroup)
                ' Set default price
                Market(1, iGroup) = 1
            Next
            For nFleet As Integer = 1 To NumFleet
                ' Set the last det col values to 1.0
                DiscardFate(nFleet, NumDetrit) = 1
            Next
        Else
            ' Set default market (off-vessel) prices
            For iFleet As Integer = 1 To NumFleet
                For iGroup As Integer = 1 To NumGroups
                    Market(iFleet, iGroup) = 1.0!
                    PropDiscardMort(iFleet, iGroup) = 1.0!
                Next iGroup
            Next iFleet
        End If

        Return True

    End Function

    Public Sub RedimEcosimScenarios()

        ReDim Me.EcosimScenarioName(Me.NumEcosimScenarios)
        ReDim Me.EcosimScenarioDBID(Me.NumEcosimScenarios)
        ReDim Me.EcosimScenarioDescription(Me.NumEcosimScenarios)
        ReDim Me.EcosimScenarioAuthor(Me.NumEcosimScenarios)
        ReDim Me.EcosimScenarioContact(Me.NumEcosimScenarios)
        ReDim Me.EcosimScenarioLastSaved(Me.NumEcosimScenarios)

        Me.ActiveEcosimScenario = cCore.NULL_VALUE

    End Sub

    Public Sub RedimEcospaceScenarios()

        ReDim Me.EcospaceScenarioName(Me.NumEcospaceScenarios)
        ReDim Me.EcospaceScenarioDBID(Me.NumEcospaceScenarios)
        ReDim Me.EcospaceScenarioDescription(Me.NumEcospaceScenarios)
        ReDim Me.EcospaceScenarioAuthor(Me.NumEcospaceScenarios)
        ReDim Me.EcospaceScenarioContact(Me.NumEcospaceScenarios)
        ReDim Me.EcospaceScenarioLastSaved(Me.NumEcospaceScenarios)

        Me.ActiveEcospaceScenario = cCore.NULL_VALUE

    End Sub

    Public Sub RedimEcotracerScenarios()

        ReDim Me.EcotracerScenarioName(Me.NumEcotracerScenarios)
        ReDim Me.EcotracerScenarioDBID(Me.NumEcotracerScenarios)
        ReDim Me.EcotracerScenarioDescription(Me.NumEcotracerScenarios)
        ReDim Me.EcotracerScenarioAuthor(Me.NumEcotracerScenarios)
        ReDim Me.EcotracerScenarioContact(Me.NumEcotracerScenarios)
        ReDim Me.EcotracerScenarioLastSaved(Me.NumEcotracerScenarios)

        Me.ActiveEcotracerScenario = cCore.NULL_VALUE

    End Sub

    Public Sub RedimPedigree()

        ReDim Me.PedigreeLevelDBID(Me.NumPedigreeLevels)
        ReDim Me.PedigreeLevelName(Me.NumPedigreeLevels)
        ReDim Me.PedigreeLevelColor(Me.NumPedigreeLevels)
        ReDim Me.PedigreeLevelDescription(Me.NumPedigreeLevels)
        ReDim Me.PedigreeLevelVarName(Me.NumPedigreeLevels)
        ReDim Me.PedigreeLevelIndexValue(Me.NumPedigreeLevels)
        ReDim Me.PedigreeLevelConfidence(Me.NumPedigreeLevels)
        ReDim Me.PedigreeLevelEstimated(Me.NumPedigreeLevels)
        ReDim Me.Pedigree(Me.NumGroups, Me.NumPedigreeVariables)

    End Sub

    Public Sub RedimTaxon()

        ReDim Me.TaxonDBID(Me.NumTaxon)
        ReDim Me.TaxonGroup(Me.NumTaxon)
        ReDim Me.TaxonGroupProp(Me.NumTaxon)
        ReDim Me.TaxonClass(Me.NumTaxon)
        ReDim Me.TaxonCode3A(Me.NumTaxon)
        ReDim Me.TaxonCodeISCAAP(Me.NumTaxon)
        ReDim Me.TaxonCodeTaxon(Me.NumTaxon)
        ReDim Me.TaxonCommonName(Me.NumTaxon)
        ReDim Me.TaxonFamily(Me.NumTaxon)
        ReDim Me.TaxonGenus(Me.NumTaxon)
        ReDim Me.TaxonOrder(Me.NumTaxon)
        ReDim Me.TaxonSourceKey(Me.NumTaxon)
        ReDim Me.TaxonSource(Me.NumTaxon)
        ReDim Me.TaxonSpecies(Me.NumTaxon)
        ReDim Me.TaxonNorth(Me.NumTaxon)
        ReDim Me.TaxonSouth(Me.NumTaxon)
        ReDim Me.TaxonEast(Me.NumTaxon)
        ReDim Me.TaxonWest(Me.NumTaxon)
        ReDim Me.TaxonLastUpdated(Me.NumTaxon)

    End Sub

    Public Sub Clear()
        Me.NumGroups = 0
        Me.NumTaxon = 0
        Me.NumFleet = 0
        Me.NumLiving = 0
        Me.NumDetrit = 0
        Me.NumEcosimScenarios = 0
        Me.NumEcospaceScenarios = 0
        Me.NumEcotracerScenarios = 0
    End Sub

#End Region

#Region "Computed Variables/Stats"


    ''' <summary>
    ''' Central handler for computing anything after an Ecopath model run.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function onPostEcopathRun() As Boolean

        Try

            UpdateBH()
            Compute_M2_Resp_and_Stats()
            ComputeFisheriesStats()
            Compute_M2_Resp_and_Stats()
            ComputeMoreStats()
            ComputeProfit()
            ComputePedigree()

            Return True

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".PostEcopathUpdate() Error: " & ex.Message)
            Return False
        End Try

    End Function


    ''' <summary>
    '''     Computes 
    '''CatchSum: sum of catch.
    '''GEff: Gross efficiency catch/net p.p..
    '''TLcatch: Mean trophic level of the catch.
    '''Run after the parameters have been estimated.
    ''' </summary>
    ''' <remarks>
    ''' This code was originally at the bottom of ParamEstimate1.
    ''' </remarks>
    Private Sub ComputeFisheriesStats()
        Dim Kount As Single, Total As Single, Mean As Single, IMPT As Single, Consu As Single, TruPut As Single
        Dim prod As Single
        Dim i As Integer, ii As Integer

        'Kount = 0
        'Total = 0
        'Mean = 0
        For i = 1 To NumGroups
            If TTLX(i) <> 0 And B(i) <> 0 Then
                Total = Total + BQB(i) * B(i)
                Mean = Mean + TTLX(i) * B(i)
                Kount = Kount + B(i)
            End If
        Next i

        CatchSum = 0
        IMPT = 0
        Mean = 0
        Consu = 0
        TruPut = 0

        For i = 1 To NumGroups
            CatchSum = CatchSum + Landing(0, i) + Discard(0, i) 'Catch(i)
            If PP(i) = 2 Then              'A detritus box
                IMPT = IMPT + DtImp(i)
            Else
                IMPT = IMPT + DC(i, 0) * QB(i) * B(i)
            End If
            prod = 0
            If QB(i) >= 0 Then
                prod = B(i) * PB(i) * EE(i)
                Consu = Consu + B(i) * QB(i)
            End If
            If PP(i) = 2 Then
                Consu = Consu + Dt
                For ii = 1 To NumGroups
                    prod = prod + B(ii) * QB(ii) * DC(ii, NumGroups)
                Next ii
            End If
            'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
            '            'MOD: VC/ELI 012397
            '            If i > NumLiving And prod < 0 Then GoTo SkipTr
            '            'END MOD
            '            TruPut = TruPut + prod
            '            If QB(i) = 0 Then Mean = Mean + B(i) * PB(i)
            'SkipTr:
            'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

            'jb Modified to not use the goto statment
            'the original intent was to NOT sum "prod" for non living groups that had negative "prod"
            'so 'TruPut' is the sum of all positive 'prod'
            If (i > NumLiving And prod < 0) = False Then 'GoTo SkipTr
                TruPut = TruPut + prod
                If QB(i) = 0 Then Mean = Mean + B(i) * PB(i)
            End If

        Next i

        'If NumGroups > NumLiving And EX(NumGroups) > 0 Then TruPut = TruPut + EX(NumGroups) '+ BA(NumGroups)
        For i = NumLiving + 1 To NumGroups
            TruPut = TruPut + Ex(i)
        Next
        If Totpp > 0 Then
            GEff = CatchSum / Totpp
        ElseIf PProd > 0 Then
            GEff = CatchSum / PProd
        Else
            GEff = 0
        End If

        If GEff <> 0 Then
            ' TLcatch gives trophic level of the fishery
            Kount = 0 : Total = 0
            For i = 1 To NumGroups
                Kount = Kount + fCatch(i)
                Total = Total + TTLX(i) * fCatch(i)
            Next i
            If Kount > 0 Then
                TLcatch = Total / Kount
            Else
                TLcatch = 0
            End If
        End If

    End Sub
    '''<summary>
    '''     Computes
    '''M2(): Predator mortality for group i.
    '''Resp(i): Respiration for group i.
    '''RTZ: sum resp.  
    '''ConSum: sum of consumption.
    '''SumBio: sum of biomass.
    '''min_B_QB: minimum B*QB.
    ''' </summary>
    ''' <remarks>
    ''' Was Public Sub ParamEstimate2() in original code
    ''' </remarks>
    Private Sub Compute_M2_Resp_and_Stats()

        Dim Prod As Single = 0
        Dim M2Sum As Single = 0
        Dim strMsg As String = ""
        Dim i As Integer, j As Integer
        Dim b_resp_below_zero As Boolean = False

        'jb variable from v-5 not used here
        'Dim pt As Integer, DetC As Integer

        RTZ = 0
        Consum = 0
        SumBio = 0

        For i = 1 To NumGroups
            If i <= NumLiving Then
                SumBio = SumBio + B(i)
                For j = 1 To NumLiving
                    If DC(j, i) > 0 And B(i) > 0 Then M2Sum = M2Sum + B(j) * QB(j) * DC(j, i) / B(i)
                Next j
            End If
            M2(i) = M2Sum
            M2Sum = 0

            If i <= NumLiving Then
                If QB(i) > 0 Then

                    Consum = Consum + B(i) * QB(i)
                    Prod = EE(i) * B(i) * PB(i) + FlowToDet(i)

                    ' FlowToDet(i) is the total flow to Detritus
                    If currUnitIndex = eUnitCurrencyType.Nitrogen Or currUnitIndex = eUnitCurrencyType.Phosporous Or currUnitIndex = eUnitCurrencyType.CustomNutrient Then
                        Resp(i) = 0 'Nutrient       B(i) * QB(i) - prod
                    ElseIf PP(i) < 1 Then
                        Resp(i) = B(i) * QB(i) - (1 - PP(i)) * Prod
                    Else
                        Resp(i) = B(i) * QB(i) - Prod
                    End If
                Else
                    'vc resp of pp OK  RESP(i) = 0
                End If
            Else
                'vc resp of detritus OK RESP(i) = 0
            End If

            RTZ = RTZ + Resp(i)

            If Resp(i) < 0 Then b_resp_below_zero = True 'pt = 2

            'jb 7-dec-04 DetC never used
            'If det(0, i) < 0 Then DetC = 1

        Next i

        'jb min_B_QB was called min
        min_B_QB = 0
        For i = 1 To NumGroups
            If QB(i) > 0 Then
                If min_B_QB = 0 Then min_B_QB = B(i) * QB(i)
                If min_B_QB > B(i) * QB(i) Then min_B_QB = B(i) * QB(i)
            End If
        Next i

        If b_resp_below_zero Then
            strMsg = "WARNING : Respiration cannot be negative. Summary statistics for the system"
            strMsg = strMsg & " are suppressed. Please check parameters and rerun program."

            Me.m_messages.AddMessage(New cMessage(strMsg, eMessageType.ErrorEncountered, _
                                                    eCoreComponentType.EcoPath, eMessageImportance.Warning))
        End If
    End Sub
    ''' <summary>
    ''' Compute
    ''' Conn: Connectance Index.
    ''' SumEx: sum of export.
    ''' SumP: Sum of all production.
    ''' SysOm: System Omnivory Index.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub ComputeMoreStats()
        Dim i As Integer, j As Integer, SysOmDen As Single

        For i = 1 To NumLiving
            For j = 1 To NumGroups
                If DC(i, j) > 0 Then Conn = Conn + 1
            Next j
        Next i
        Conn = Conn / (NumLiving) ^ 2  'with detritus

        'system omnivory index
        SysOm = 0
        SysOmDen = 0
        'jb min_B_QB was min 
        'it is set in Compute_M2_Resp_and_Stats()
        For i = 1 To NumLiving
            If B(i) * QB(i) / min_B_QB > 0 Then    ' *** CONSUMERS ONLY
                SysOm = SysOm + Math.Log(B(i) * QB(i) / min_B_QB) * BQB(i)
                SysOmDen = SysOmDen + Math.Log(B(i) * QB(i) / min_B_QB)
            End If
        Next i

        If SysOmDen > 0 Then SysOm = SysOm / SysOmDen

        SumEx = 0
        SumP = 0
        For i = 1 To NumGroups
            SumEx = SumEx + Ex(i)
            If PB(i) > 0 And B(i) > 0 Then SumP = SumP + PB(i) * B(i)
        Next i

    End Sub

    Private Sub ComputeProfit()
        Dim Gear As Integer
        Dim Grp As Integer
        Dim value As Single

        LandingValue = 0
        ShadowValue = 0

        For Grp = 1 To NumGroups
            For Gear = 1 To NumFleet
                value = Landing(Gear, Grp) * Market(Gear, Grp)
                If value > 0 Then LandingValue = LandingValue + value
            Next
            value = Shadow(Grp) * B(Grp)
            If value > 0 Then ShadowValue = ShadowValue + value
        Next

        Fixed = 0
        Variab = 0
        For Gear = 1 To NumFleet
            Fixed = Fixed + cost(Gear, eCostIndex.Fixed)
            Variab = Variab + cost(Gear, eCostIndex.CUPE) + cost(Gear, eCostIndex.Sail)
        Next

    End Sub

    Private Sub ComputePedigree()

        Dim iLevel As Integer = 0
        Dim iTotal As Integer = 0
        Dim iNumLevels As Integer = 0
        Dim group As cEcoPathGroupInput = Nothing
        Dim var As eVarNameFlags = eVarNameFlags.NotSet
        Dim bPedigreeComplete As Boolean = (Me.NumPedigreeLevels > 0)

        For iGroup As Integer = 1 To Me.NumGroups
            ' For all vars
            For iVariable As Integer = 1 To Me.NumPedigreeVariables

                var = Me.PedigreeVariables(iVariable)

                If Me.PP(iGroup) = 1 And (var = eVarNameFlags.PBInput Or var = eVarNameFlags.QBInput) Then
                    'Skip qb for producers
                ElseIf Me.fCatch(iGroup) = 0 And (var = eVarNameFlags.TCatchInput) Then
                    'do nothing continue to next par
                ElseIf Me.PP(iGroup) = 2 Then
                    'do nothing
                Else
                    Try
                        iLevel = Me.Pedigree(iGroup, iVariable)
                        iTotal += Me.PedigreeLevelIndexValue(iLevel)
                        iNumLevels += 1
                        If (Me.Pedigree(iGroup, iVariable) < 0) Then
                            bPedigreeComplete = False
                        End If
                    Catch ex As Exception

                    End Try
                End If

            Next iVariable
        Next iGroup

        If (iNumLevels = 0 Or Not bPedigreeComplete) Then
            Me.PedigreeStatsModelIndex = cCore.NULL_VALUE
            Me.PedigreeStatsTStar = cCore.NULL_VALUE
        Else
            Dim sVar As Single = CSng(iTotal / iNumLevels)
            Me.PedigreeStatsModelIndex = sVar
            Me.PedigreeStatsTStar = CSng(sVar * Math.Sqrt(Me.NumLiving - 2) / Math.Sqrt(1 - sVar ^ 2))
        End If

    End Sub

    Public Sub DietWasChanged(ByVal pred As Integer, ByVal prey As Integer)
        Dim j As Integer, K As Integer
        Dim FoundPredPrey As Boolean

        j = UBound(DietChanged, 2)
        For K = 0 To j
            If DietChanged(0, K) = pred And DietChanged(1, K) = prey Then
                FoundPredPrey = True
                Exit For
            End If
        Next

        If FoundPredPrey = False Then
            ReDim Preserve DietChanged(1, j + 1)
            DietChanged(0, j + 1) = pred
            DietChanged(1, j + 1) = prey
        End If

    End Sub

    ''' <summary>
    ''' Copy the Input arrays into the arrays that are used for modeling and model output.
    ''' </summary>
    ''' <returns>True if all the values were copied successfully.</returns>
    ''' <remarks>This is call at the start of an Ecopath model run to copy the input data into the arrays that are used
    ''' for model computations and output. I.e. copies EEinput(NumGroups) into EE(NumGroups). In EwE5 this is called MakeUnknownUnknown </remarks>
    Public Function CopyInputToModelArrays() As Boolean

        'Warning EwE5 also included input variables for BA, Immig, and Emigration 
        'See modEcosSense.MakeUnknownUnknown
        Try
            Binput.CopyTo(B, 0)
            BHinput.CopyTo(BH, 0)
            EEinput.CopyTo(EE, 0)
            PBinput.CopyTo(PB, 0)
            QBinput.CopyTo(QB, 0)
            GEinput.CopyTo(GE, 0)

            ' copy dc
            For i As Integer = 0 To Me.NumGroups
                For j As Integer = 0 To Me.NumGroups
                    DC(i, j) = DCInput(i, j)
                Next
            Next
            Return True
        Catch ex As Exception
            Debug.Assert(False, ex.Message)
            Return False
        End Try

    End Function

    ''' <summary>
    ''' Compute missing <see cref="BH">BH</see> (Biomass/Area) values.
    ''' </summary>
    ''' <returns>True if successfully.</returns>
    ''' <remarks>
    ''' EwE5 performed differently here; BH() value was left at its NULL input value,
    ''' and was computed in the interface for display. I hope this doesn't mess anything up.
    ''' </remarks>
    Private Function UpdateBH() As Boolean
        For i As Integer = 1 To NumGroups
            If BH(i) < 0 And B(i) > 0 And Area(i) > 0 Then
                BH(i) = B(i) / Area(i)
            End If
        Next
        Return True
    End Function

    ''' <summary>
    ''' Sums a <see cref="DC">Diet Composition</see> matrix to one. 
    ''' </summary>
    ''' <param name="bSumDCInput">Flag, states which DC matrix should be corrected. If 
    ''' True, the user input matrix <see cref="DCInput">DCInput</see> will be altered. 
    ''' If False, the model matrix <see cref="DC">DC</see> will be altered.</param>
    Public Sub SumDCToOne(Optional ByVal bSumDCInput As Boolean = False)

        ' Pick matrix to alter
        Dim asDCref(,) As Single = CType(IIf(bSumDCInput, Me.DCInput, Me.DC), Single(,))

        ' For each potential predator
        For iPred As Integer = 1 To NumLiving
            ' Is a consumer?
            If PP(iPred) < 1 Then
                ' #Yes: calc sum
                Dim sDCSum As Single = 0.0
                ' For each of potential prey
                ' ** NOTE THAT THE LOWER BOUND USED HERE IS 0 INSTEAD OF 1! This is to include
                ' ** DC Impoprt in the calculations - which is stored at index 0.
                For iPrey As Integer = 0 To Me.NumGroups
                    ' Add consumption to sum
                    sDCSum += asDCref(iPred, iPrey)
                Next iPrey

                ' Is there predation with a need to recalc?
                If (sDCSum > 0) And (sDCSum <> 1.0) Then
                    ' For each prey
                    For iPrey As Integer = 1 To Me.NumGroups
                        ' Rescale consumption
                        asDCref(iPred, iPrey) = asDCref(iPred, iPrey) / sDCSum
                    Next iPrey
                End If
            End If ' PP < 1
        Next iPred
    End Sub

#End Region

#Region "Debugging stuff"


    ''' <summary>
    ''' Dump the estimated parameters to a csv file.
    ''' </summary>
    ''' <param name="FileName">
    ''' Name of the dump file.
    ''' </param>
    ''' <returns>
    ''' True if no error Encountered.
    ''' False if an error.
    ''' </returns>
    ''' <remarks></remarks>
    Public Function DumpResults(ByVal FileName As String) As Boolean
        Dim stream As System.IO.StreamWriter
        Dim i As Integer, returnvalue As Boolean

        Try
            stream = New System.IO.StreamWriter(FileName)
        Catch ex As Exception
            DumpResults = False
            cLog.Write(Me.ToString + ".DumpResults() failed to open file.")
            Exit Function
        End Try

        Try
            stream.WriteLine("GroupName,Biomass(B),Prod/Biomass(PB),Cons/Biomass(QB),Ecotrophic eff.(EE),Prod/Consum(GE)")
            For i = 1 To NumGroups
                stream.Write(GroupName(i))
                stream.Write(",")
                stream.Write(B(i))
                stream.Write(",")
                stream.Write(PB(i))
                stream.Write(",")
                stream.Write(QB(i))
                stream.Write(",")
                stream.Write(EE(i))
                stream.Write(",")
                stream.Write(GE(i))

                stream.Write(vbCrLf)
            Next

            stream.Close()

        Catch ex As Exception
            stream.Close()
            returnvalue = False
        End Try

        returnvalue = True

        DumpResults = returnvalue

    End Function

    ''' <summary>
    ''' Run any post initialization validation
    ''' </summary>
    ''' <remarks>This should only be called from the datasouce once it has populated the Ecopath variables.
    ''' It should not be called by the core in response to an edit because it can alter values in an unknown number of places. 
    ''' The core would need to reload all it's Ecopath data after the call.
    ''' If other logic is need that the core can have access to it should be put in a separate routine and called here. 
    ''' The core can then access the logic via a different interface.
    '''  </remarks>
    Public Sub onPostInitialization()
        'not much at this time

        'GS = zero if group is Primary producer
        For iGroup As Integer = 1 To NumGroups
            If PP(iGroup) = 1 Then
                GS(iGroup) = 0
            End If
        Next

    End Sub

#End Region

    Friend Sub copyTo(ByRef dest As cEcopathDataStructures, Optional ByVal bRedim As Boolean = True)
        Try
            'variables needed to redim
            dest.NumGroups = NumGroups
            dest.NumFleet = NumFleet
            dest.NumDetrit = NumDetrit
            dest.NumLiving = NumLiving

            If bRedim Then
                dest.redimGroups()
            End If

            dest.bInitialized = bInitialized


            GroupName.CopyTo(dest.GroupName, 0)    'was Specie()
            'GroupDBID.CopyTo(dest.GroupDBID, 0)        'Do not copy IDs!

            dest.NumEcosimScenarios = NumEcosimScenarios
            'EcosimScenarioName.CopyTo(dest.EcosimScenarioName, 0)
            'EcosimScenarioDBID.CopyTo(dest.EcosimScenarioDBID, 0)
            'EcosimScenarioDescription.CopyTo(dest.EcosimScenarioDescription, 0)
            dest.ActiveEcosimScenario = ActiveEcosimScenario

            NumEcospaceScenarios = dest.NumEcospaceScenarios
            'EcospaceScenarioName.CopyTo(dest.EcospaceScenarioName, 0)
            'EcospaceScenarioDBID.CopyTo(dest.EcospaceScenarioDBID, 0)
            'EcospaceScenarioDescription.CopyTo(dest.EcospaceScenarioDescription, 0)
            'ActiveEcospaceScenario = cCore.NULL_VALUE

            B.CopyTo(dest.B, 0)
            BH.CopyTo(dest.BH, 0)
            BA.CopyTo(dest.BA, 0)
            BaBi.CopyTo(dest.BaBi, 0)
            PB.CopyTo(dest.PB, 0)
            QB.CopyTo(dest.QB, 0)
            EE.CopyTo(dest.EE, 0)
            GE.CopyTo(dest.GE, 0)
            GS.CopyTo(dest.GS, 0)
            EEinput.CopyTo(dest.EEinput, 0)
            PBinput.CopyTo(dest.PBinput, 0)
            QBinput.CopyTo(dest.QBinput, 0)
            GEinput.CopyTo(dest.GEinput, 0)

            Binput.CopyTo(dest.Binput, 0)

            BHinput.CopyTo(dest.BHinput, 0)

            'min_B_QB = dest.min_B_QB 'minimum B*QB
            dest.DCInput = DCInput.Clone
            dest.DC = DC.Clone

            'dest.currUnitName = currUnitName
            dest.currUnitIndex = currUnitIndex
            dest.TimeUnitName = TimeUnitName
            dest.TimeUnitIndex = TimeUnitIndex
            dest.DietsModified = DietsModified
            dest.PProd = PProd

            ''''DietChanged.CopyTo(dest.DietChanged, 0)

            Ex.CopyTo(dest.Ex, 0)

            fCatch.CopyTo(dest.fCatch, 0) 'was called Catch but this causes a naming conflict with Try Catch blocks
            Array.Copy(DCInput, dest.DCInput, DCInput.Length)
            dest.DCInput = DCInput.Clone
            dest.DC = DC.Clone
            dest.DF = DF.Clone
            Area.CopyTo(dest.Area, 0)
            dest.DCChanged = DCChanged.Clone

            BQB.CopyTo(dest.BQB, 0)
            Resp.CopyTo(dest.Resp, 0)
            PP.CopyTo(dest.PP, 0)           'TM Trophic Mode
            dest.det = det.Clone
            dest.DCDet = DCDet.Clone                 'Diet Composition of Detritus  for fishery            DetEaten.CopyTo(dest.DetEaten, 0)                 ' For multiple detritus
            DetPassedOn.CopyTo(dest.DetPassedOn, 0)              ' For multiple detritus
            DetPassedProp.CopyTo(dest.DetPassedProp, 0)              ' For multiple detritus
            FlowToDet.CopyTo(dest.FlowToDet, 0)
            InputToDet.CopyTo(dest.InputToDet, 0)
            'JS 08Jan09: SumDC was a global scratch variable, changed to local scope
            'SumDC.CopyTo(dest.SumDC, 0)

            Immig.CopyTo(dest.Immig, 0)
            Emigration.CopyTo(dest.Emigration, 0)
            Emig.CopyTo(dest.Emig, 0)    'relative to biomass, used in Ecosim
            Shadow.CopyTo(dest.Shadow, 0)
            GroupIsFish.CopyTo(dest.GroupIsFish, 0)
            GroupIsInvert.CopyTo(dest.GroupIsInvert, 0)

            dest.NumCatchCodes = NumCatchCodes
            dest.PropLanded = PropLanded.Clone
            TTLX.CopyTo(dest.TTLX, 0)
            'JS 08Jan09: LHS was a global scratch variable, changed to local scope
            'dest.LHS = LHS.Clone
            StanzaGroup.CopyTo(dest.StanzaGroup, 0)
            dest.CatchCode = CatchCode.Clone
            dest.CVpar = CVpar.Clone
            M0.CopyTo(dest.M0, 0)
            M2.CopyTo(dest.M2, 0)
            dest.Path = Path.Clone
            dest.LastComp = LastComp.Clone
            DtImp.CopyTo(dest.DtImp, 0)

            ''fishing(variables)
            dest.NoGearData = NoGearData
            dest.cost = cost.Clone
            dest.CostPct = CostPct.Clone
            dest.Discard = Discard.Clone
            dest.DiscardFate = DiscardFate.Clone
            FleetName.CopyTo(dest.FleetName, 0)
            'FleetDBID.CopyTo(dest.FleetDBID, 0) ' Do NOT copy DBIDs
            dest.Landing = Landing.Clone
            dest.Market = Market.Clone
            dest.PropDiscard = PropDiscard.Clone

            dest.RTZ = RTZ
            dest.Consum = Consum
            dest.SumBio = SumBio
            dest.CatchSum = CatchSum
            dest.GEff = GEff
            dest.Totpp = Totpp
            dest.TLcatch = TLcatch
            dest.Dt = Dt
            dest.SumEx = SumEx
            dest.SumP = SumP
            dest.Conn = Conn
            dest.SysOm = SysOm

            vbK.CopyTo(dest.vbK, 0)
            dest.Hlap = Hlap.Clone
            dest.Plap = Plap.Clone
            GroupColor.CopyTo(dest.GroupColor, 0)
            FleetColor.CopyTo(dest.FleetColor, 0)
            dest.Host = Host.Clone
            mis.CopyTo(dest.mis, 0)

            ' Copy model data
            dest.ModelArea = Me.ModelArea
            dest.ModelAreaName = Me.ModelAreaName
            dest.ModelAuthor = Me.ModelAuthor
            dest.ModelContact = Me.ModelContact
            dest.ModelDescription = Me.ModelDescription
            dest.ModelEast = Me.ModelEast
            dest.ModelGroupDigits = Me.ModelGroupDigits
            dest.ModelName = ModelName
            dest.ModelNorth = ModelNorth
            dest.ModelNumDigits = Me.ModelNumDigits
            dest.ModelSouth = Me.ModelSouth
            dest.ModelUnitCurrency = Me.ModelUnitCurrency
            dest.ModelUnitCurrencyCustom = Me.ModelUnitCurrencyCustom
            dest.ModelUnitMonetary = Me.ModelUnitMonetary
            dest.ModelUnitTime = Me.ModelUnitTime
            dest.ModelUnitTimeCustom = Me.ModelUnitTimeCustom
            dest.ModelWest = Me.ModelWest
            dest.FirstYear = Me.FirstYear
            dest.NumYears = Me.NumYears

        Catch ex2 As Exception
            Debug.Assert(False, ex2.Message)
        End Try

    End Sub

    Public Sub New(ByVal CoreMessagePublisher As cMessagePublisher)
        Me.m_messages = CoreMessagePublisher
    End Sub
End Class
