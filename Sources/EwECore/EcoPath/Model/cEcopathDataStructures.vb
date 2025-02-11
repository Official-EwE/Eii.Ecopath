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
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

Option Strict Off ' OUCH
Imports EwEUtils.Extensions
Imports EwEUtils.Core
Imports OfficeOpenXml.FormulaParsing.Excel.Functions.Logical

''' <summary>
''' Wrapper for the underlying data structures of the EcoPath model. 
''' Provides a way to wrap all the data from EcoPath into one place
''' </summary>
Public Class cEcopathDataStructures

#Region " Private data "

    Private m_messages As cMessagePublisher

#End Region ' Private data

    Public Sub New(CoreMessagePublisher As cMessagePublisher)
        Me.m_messages = CoreMessagePublisher

        'No External coupling of Ecospace by default
        Me.isEcospaceModelCoupled = False

    End Sub

#Region " Public Variables "

    Public bInitialized As Boolean = False

    Public ModelDBID As Integer = 0
    Public ModelName As String = ""
    Public ModelDescription As String = ""
    Public ModelArea As Single = 0
    Public ModelNumDigits As Integer = 0
    Public ModelGroupDigits As Boolean = False
    Public ModelUnitTime As eUnitTimeType = eUnitTimeType.Year
    Public ModelUnitTimeCustom As String = ""
    ''' <summary>Index of current selected currency units.</summary>
    Public ModelUnitCurrency As Integer = eUnitCurrencyType.WetWeight
    Public ModelUnitCurrencyCustom As String = ""
    Public ModelUnitMonetary As String = ""
    Public ModelUnitArea As eUnitAreaType = eUnitAreaType.Km2
    Public ModelUnitAreaCustom As String = ""
    Public ModelAuthor As String = ""
    Public ModelContact As String = ""
    Public ModelLastSaved As Double = 0
    Public ModelSouth As Single = 0
    Public ModelNorth As Single = 0
    Public ModelWest As Single = 0
    Public ModelEast As Single = 0
    Public FirstYear As Integer = Date.Now.Year
    Public NumYears As Integer = 1
    Public ModelCountry As String = ""
    Public ModelEcosystemType As String = ""
    ''' <summary>Code of a model in the Ecobase repository, if any.</summary>
    Public ModelEcobaseCode As String = ""
    Public ModelPublicationDOI As String = ""
    Public ModelPublicationURI As String = ""
    Public ModelPublicationRef As String = ""

    ''' <summary>Group names.</summary>
    Public GroupName() As String
    ''' <summary>Group Database ID - uniquely identifies a group.</summary>
    Public GroupDBID() As Integer

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
    ''' <summary>Biomass accumulation (t/km²/year) as entered by the user</summary>
    Public BAInput() As Single
    ''' <summary>Biomass accumulation / biomass</summary>
    Public BaBi() As Single
    ''' <summary>Biomass accumulation (t/km²/year)</summary>
    Public BA() As Single
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

    ''' <summary>Unassimilation / consumption (ratio) for Energy Currency units ONLY</summary>
    ''' <remarks>Fraction of the food that is not assimilated.</remarks>
    Public GSEng() As Single

    'Input Values are user entered values.
    'Inputs are the values that can be edited by a user, get saved to the database and displayed as basic inputs
    'each array will have a companion used for modeling that does not have 'input' i.e. EEinput() and EE() 
    'the input values are copied into the modeling array whenever the ecopath model is run CopyInputToModelArrays(...) 
    'these values are exposed via cEcoPathGroupOutputs

    ''' <summary>Ecotrophic efficiency (ratio) - original user input value of <see cref="EE">EE</see>.</summary>
    Public EEinput() As Single
    ''' <summary>Other mortaility (ratio) - defined as 1-<see cref="EE">EE</see>.</summary>
    Public OtherMortinput() As Single
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
    ''' <summary>User-provided name for time units.</summary>
    Public TimeUnitName As String
    ''' <summary>Index of current selected time unit.</summary>
    Public TimeUnitIndex As Integer
    ''' <summary>Flag stating whether diets have been modified since the last time Ecopath has ran.</summary>
    Public DietsModified As Boolean
    Public PProd As Single
    Public Energy() As Single

    Public DietChanged(,) As Integer

    Public Ex() As Single

    ''' <summary>Sum (per <see cref="NumGroups">NumGroups</see>) of landings + discards.</summary>
    ''' <remarks>Computed in Catch_calculations(). was called Catch but this causes a naming conflict with Try Catch blocks</remarks>
    Public fCatch() As Single '
    ''' <summary>Diet composition(per pred, prey) (ratio), a <see cref="NumGroups">NumGroups</see> * <see cref="NumGroups">NumGroups</see>
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
    ''' <summary>Detritus flow (#groups + #fleet,#groups + #fleet)</summary>
    Public det(,) As Single
    ''' <summary>Diet Composition of Detritus  for fishery.</summary>
    Public DCDet(,) As Single
    Public DetEaten() As Single                 ' For multiple detritus
    Public DetPassedOn() As Single              ' For multiple detritus
    Public DetPassedProp() As Single              ' For multiple detritus
    ''' <summary>Flow to detritus (x (group + fleet)).</summary>
    Public FlowToDet() As Single
    ''' <summary>Input to detritus (x group).</summary>
    Public InputToDet() As Single

    ''' <summary>Migration into the area covered by the model (t/km²/year)</summary>
    ''' <remarks>Note that migration is not the same as import, refer to the manual for details.</remarks>
    Public Immig() As Single
    ''' <summary>Emigration (per group) out of the area covered by the model (t/km²/year)</summary>
    Public Emigration() As Single
    ''' <summary>Emigration (per group) relative to biomass (ratio)</summary>
    Public Emig() As Single    'relative to biomass, used in Ecosim
    Public Shadow() As Single
    ''' <summary>States which groups are fishes. There is no interface in EwE for this flag, and its function should be replaced by the taxonomy logic</summary>
    Public GroupIsFish() As Boolean
    ''' <summary>States which groups are invertebrates. There is no interface in EwE for this flag, and its function should be replaced by the taxonomy logic</summary>
    Public GroupIsInvert() As Boolean
    Public PropLanded(,) As Single
    ''' <summary>Trophic levels in Ecopath.</summary>
    Public TTLX() As Single
    'Public TLSim() As Single    'These TL's are recalculated for each time step in Ecosim
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
    ''' <summary>Names of fleets.</summary>
    Public FleetName() As String
    ''' <summary>Database IDs per fleet.</summary>
    Public FleetDBID() As Integer
    Public NoGearData As Boolean
    ''' <summary> cost(nFleets,3) '1 is fixed cost, 2 is cost per unit effort, 3 sailing cost </summary>
    Public cost(,) As Single
    ''' <summary>Actual, real-world effort represented by the fleet</summary>
    Public NominalEffort() As Single
    Public CostPct(,) As Single
    ''' <summary>Discarded biomass by fleet group </summary>
    ''' <remarks>Includes survival!</remarks>
    Public Discard(,) As Single
    ''' <summary>Fate of discards (by fleet, #detritus)</summary>
    Public DiscardFate(,) As Single


    ''' <summary>Landinged biomass (by fleet,group)</summary>
    Public Landing(,) As Single
    ''' <summary>Market value of landings (by fleet,group)</summary>
    Public Market(,) As Single
    ''' <summary>Proportion of total catch that are discards (by fleet, group)</summary>
    ''' <remarks>This is proportion of the total catch that are discarded. Including mortality and survivals.</remarks>
    Public PropDiscard(,) As Single
    ''' <summary>Proportion of regulated discards that die (by fleet, group)</summary>
    Public PropDiscardMort(,) As Single ' gear group 0-1


    Public RTZ As Single 'sum of respiration
    Public Consum As Single
    Public SumBio As Single
    ''' <summary>Sum of catch.</summary>
    Public CatchSum As Single
    ''' <summary>Gross efficiency.</summary>
    Public GEff As Single
    Public Totpp As Single
    ''' <summary>Tropic level of the catch.</summary>
    Public TLcatch As Single
    ''' <summary>Total flow of detritus</summary>
    Public Dt As Single
    ''' <summary>Sum of exports.</summary>
    Public SumEx As Single
    ''' <summary>Sum of all production.</summary>
    Public SumP As Single
    ''' <summary>Connectance Index.</summary>
    Public Conn As Single
    Public SysOm As Single
    Public LandingValue As Single
    Public ShadowValue As Single
    Public Fixed As Single
    Public Variab As Single

    ''' <summary>VBGF curvature parameter K (/year).</summary>
    Public vbK() As Single
    Public Hlap(,) As Single
    Public Plap(,) As Single
    ''' <summary>Colours for groups in an interface (x group).</summary>
    Public GroupColor() As Integer
    ''' <summary>Colours for fleets in an interface (x fleet).</summary>
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

    ''' <summary>Array [#groups, #supported vars] = Level index.</summary>
    Public Pedigree(,) As Integer
    ''' <summary>One-based array of variables supported by the pedigree system.</summary>
    Public Shared PedigreeVariables() As eVarNameFlags = {eVarNameFlags.NotSet, eVarNameFlags.BiomassAreaInput, eVarNameFlags.PBInput, eVarNameFlags.QBInput, eVarNameFlags.DietComp, eVarNameFlags.TCatchInput}
    ''' <summary>Number of <see cref="PedigreeVariables"/></summary>
    Public NumPedigreeVariables As Integer = PedigreeVariables.Length - 1

    Public PedigreeStatsModel As Single
    Public PedigreeStatsTStar As Single

    ''' <summary>
    ''' Number of missing variables per groups
    ''' </summary>
    ''' <remarks>These are the variables that need to be computed be Ecopath</remarks>
    Public mis() As Integer

    ''' <summary>
    ''' Is the currently loaded Ecospace model setup for coupling with an external model.
    ''' </summary>
    ''' <remarks>
    ''' Coupling joins Ecospace to an external model that is used to dynamically compute PP or other lower trophic level values. 
    ''' This flag is used to dimension variables during the load of an Ecospace model.
    ''' Stored with the Ecopath data because this needs to be set before an Ecospace scenario is loaded so it can be used for dimensioning.
    ''' </remarks>
    Public isEcospaceModelCoupled As Boolean

    Public isGroupLeadingB() As Boolean
    Public isGroupLeadingCB() As Boolean

    Public DiversityIndexType As eDiversityIndexType = eDiversityIndexType.Shannon
    Public KemptonsQ As Single
    Public Shannon As Single

    ''' <summary>
    ''' Returns the computed diversity index that is selected in <see cref="DiversityIndexType"/>
    ''' </summary>
    Public ReadOnly Property DiversityIndex As Single
        Get
            Select Case Me.DiversityIndexType
                Case eDiversityIndexType.KemptonsQ
                    Return Me.KemptonsQ
                Case eDiversityIndexType.Shannon
                    Return Me.Shannon
                Case Else
                    Debug.Assert(False, "Diversity index type not supported")
            End Select
            Return cCore.NULL_VALUE
        End Get
    End Property

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

            Me.redimGroupVariables() 'just ngroup variables
            Me.RedimFleetVariables(True) 'fleets clear out the values
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
        Me.NumDetrit = Me.NumGroups - Me.NumLiving

        ' EstimateWhat(NumGroups)

        ReDim Me.PB(Me.NumGroups)
        ReDim Me.EE(Me.NumGroups)
        ReDim Me.QB(Me.NumGroups)
        ReDim Me.GE(Me.NumGroups)
        ReDim Me.B(Me.NumGroups)
        ReDim Me.BH(Me.NumGroups)    'habitat biomass

        ReDim Me.GEinput(Me.NumGroups)
        ReDim Me.PBinput(Me.NumGroups)
        ReDim Me.EEinput(Me.NumGroups)
        ReDim Me.OtherMortinput(Me.NumGroups)
        ReDim Me.QBinput(Me.NumGroups)
        ReDim Me.Binput(Me.NumGroups)
        ReDim Me.BHinput(Me.NumGroups)

        ReDim Me.Ex(Me.NumGroups)
        ReDim Me.fCatch(Me.NumGroups)
        ReDim Me.Area(Me.NumGroups)
        For i = 1 To Me.NumGroups
            Me.Area(i) = 1
        Next
        ReDim Me.BAInput(Me.NumGroups)
        ReDim Me.BaBi(Me.NumGroups)
        ReDim Me.BA(Me.NumGroups)
        ReDim Me.DC(Me.NumGroups + 1, Me.NumGroups + 1)
        ReDim Me.DCChanged(Me.NumGroups + 1, Me.NumGroups + 1) 'jb added to tell the core which diet comp values where changed
        ReDim Me.PP(Me.NumGroups)
        ReDim Me.GroupName(Me.NumGroups)
        ReDim Me.GroupDBID(Me.NumGroups)
        ReDim Me.GS(Me.NumGroups)
        ReDim Me.TTLX(Me.NumGroups)     'Trophic levels in Ecopath
        'JS 08Jan09: SumDC and LHS were a global scratch variable, changed to local scope
        'ReDim LHS(NumGroups, NumGroups)
        'ReDim SumDC(NumGroups)
        ReDim Me.BQB(Me.NumGroups)

        ReDim Me.Resp(Me.NumGroups)
        ReDim Me.DF(Me.NumGroups, Me.NumGroups - Me.NumLiving)

        ReDim Me.DtImp(Me.NumGroups)
        ReDim Me.DetEaten(Me.NumGroups)
        ReDim Me.DetPassedOn(Me.NumGroups)
        ReDim Me.DetPassedProp(Me.NumGroups)
        ReDim Me.InputToDet(Me.NumGroups)
        ReDim Me.M0(Me.NumGroups)
        ReDim Me.M2(Me.NumGroups)
        ReDim Me.Path(2 * Me.NumGroups + 2)
        ReDim Me.LastComp(2 * Me.NumGroups + 1)
        ReDim Me.Immig(Me.NumGroups)
        ReDim Me.Emigration(Me.NumGroups)
        ReDim Me.Emig(Me.NumGroups)
        ReDim Me.Shadow(Me.NumGroups)
        ReDim Me.Energy(Me.NumGroups)
        ReDim Me.GroupIsFish(Me.NumGroups)
        ReDim Me.GroupIsInvert(Me.NumGroups)
        ReDim Me.PropLanded(Me.NumFleet, Me.NumGroups)

        ReDim Me.Host(Me.NumGroups, Me.NumGroups)
        ReDim Me.Hlap(Me.NumGroups, Me.NumGroups)
        ReDim Me.Plap(Me.NumGroups, Me.NumGroups)
        ReDim Me.GroupColor(Me.NumGroups)

        ReDim Me.SumR(Me.NumGroups)
        ReDim Me.Alpha(Me.NumGroups, Me.NumGroups)
        ReDim Me.vbK(Me.NumGroups)

        'ReDim GrpsToShow(NumGroups + NumFleet + 2)

        'For i = 1 To NumGroups + NumFleet
        '    GrpsToShow(i) = True
        'Next

        'For i = NumGroups + NumFleet + 1 To NumGroups + NumFleet + 2
        '    GrpsToShow(i) = False
        'Next

        Me.NumCatchCodes = 30
        ReDim Me.CatchCode(Me.NumCatchCodes, Me.NumGroups)
        ReDim Me.CVpar(5, Me.NumGroups)

        For i = 1 To Me.NumGroups
            For j = 0 To 4
                Me.CVpar(j, i) = 0.1
            Next j
            Me.CVpar(5, i) = 0.05
            Me.Energy(i) = 1
        Next i

        'Stanzagroup  needed when importing eii files
        ReDim Me.StanzaGroup(Me.NumGroups)

        ReDim Me.mis(Me.NumGroups)

        'is the Ecopath group the leading B or QB for a MultiStanza group
        ReDim Me.isGroupLeadingB(Me.NumGroups)
        ReDim Me.isGroupLeadingCB(Me.NumGroups)

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
    Public Function RedimFleetVariables(NoPreserve As Boolean) As Boolean

        'det() is not saved to database
        ReDim Me.det(Me.NumGroups + Me.NumFleet, Me.NumGroups + Me.NumFleet)
        If NoPreserve Then
            ReDim Me.DCDet(Me.NumGroups - Me.NumLiving, Me.NumFleet)        'Diet composition of detritus
            ReDim Me.FlowToDet(Me.NumGroups + Me.NumFleet)
        Else
            ReDim Preserve Me.DCDet(Me.NumGroups - Me.NumLiving, Me.NumFleet)       'Diet composition of detritus
            ReDim Preserve Me.FlowToDet(Me.NumGroups + Me.NumFleet)
        End If
        'Next in Gear
        ReDim Me.cost(Me.NumFleet, 3)       '1 is fixed cost, 2 is cost per unit effort, 3 sailing cost
        ReDim Me.CostPct(Me.NumFleet, 3)       '1 is fixed cost, 2 is cost per unit effort, 3 sailing cost
        ReDim Me.FleetName(Me.NumFleet + 1)
        ReDim Me.FleetDBID(Me.NumFleet + 1)
        ReDim Me.NominalEffort(Me.NumFleet)
        'Next in Catch
        ReDim Me.Landing(Me.NumFleet, Me.NumGroups)
        ReDim Me.Discard(Me.NumFleet, Me.NumGroups)
        ReDim Me.DiscardFate(Me.NumFleet, Me.NumGroups - Me.NumLiving)
        ReDim Me.PropLanded(Me.NumFleet, Me.NumGroups)
        ReDim Me.PropDiscard(Me.NumFleet, Me.NumGroups)
        ReDim Me.PropDiscardMort(Me.NumFleet, Me.NumGroups)
        ReDim Me.Market(Me.NumFleet, Me.NumGroups)
        ReDim Me.FleetColor(Me.NumFleet)

        ' Set default market (off-vessel) prices
        For iFleet As Integer = 1 To Me.NumFleet
            For iGroup As Integer = 1 To Me.NumGroups
                Me.Market(iFleet, iGroup) = 1.0!
                Me.PropDiscardMort(iFleet, iGroup) = 1.0!
            Next iGroup
        Next iFleet

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

    Public Sub Clear()

        Me.NumGroups = 0
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
    Public Function onPostEcopathRun(fn As cEcoFunctions) As Boolean

        Try

            Me.UpdateBH()
            Me.Compute_M2_Resp_and_Stats(fn)
            Me.ComputeFisheriesStats()
            Me.Compute_M2_Resp_and_Stats(fn)
            Me.ComputeMoreStats(fn)
            Me.ComputeProfit()
            Me.ComputePedigree()

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
        For i = 1 To Me.NumGroups
            If Me.TTLX(i) <> 0 And Me.B(i) <> 0 Then
                Total = Total + Me.BQB(i) * Me.B(i)
                Mean = Mean + Me.TTLX(i) * Me.B(i)
                Kount = Kount + Me.B(i)
            End If
        Next i

        Me.CatchSum = 0
        IMPT = 0
        Mean = 0
        Consu = 0
        TruPut = 0

        For i = 1 To Me.NumGroups
            Me.CatchSum = Me.CatchSum + Me.Landing(0, i) + Me.Discard(0, i) 'Catch(i)
            If Me.PP(i) = 2 Then              'A detritus box
                IMPT = IMPT + Me.DtImp(i)
            Else
                IMPT = IMPT + Me.DC(i, 0) * Me.QB(i) * Me.B(i)
            End If
            prod = 0
            If Me.QB(i) >= 0 Then
                prod = Me.B(i) * Me.PB(i) * Me.EE(i)
                Consu = Consu + Me.B(i) * Me.QB(i)
            End If
            If Me.PP(i) = 2 Then
                Consu = Consu + Me.Dt
                For ii = 1 To Me.NumGroups
                    prod = prod + Me.B(ii) * Me.QB(ii) * Me.DC(ii, Me.NumGroups)
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
            If (i > Me.NumLiving And prod < 0) = False Then 'GoTo SkipTr
                TruPut = TruPut + prod
                If Me.QB(i) = 0 Then Mean = Mean + Me.B(i) * Me.PB(i)
            End If

        Next i

        'If NumGroups > NumLiving And EX(NumGroups) > 0 Then TruPut = TruPut + EX(NumGroups) '+ BA(NumGroups)
        For i = Me.NumLiving + 1 To Me.NumGroups
            TruPut = TruPut + Me.Ex(i)
        Next
        If Me.Totpp > 0 Then
            Me.GEff = Me.CatchSum / Me.Totpp
        ElseIf Me.PProd > 0 Then
            Me.GEff = Me.CatchSum / Me.PProd
        Else
            Me.GEff = 0
        End If

        If Me.GEff <> 0 Then
            ' TLcatch gives trophic level of the fishery
            Kount = 0 : Total = 0
            For i = 1 To Me.NumGroups
                Kount = Kount + Me.fCatch(i)
                Total = Total + Me.TTLX(i) * Me.fCatch(i)
            Next i
            If Kount > 0 Then
                Me.TLcatch = Total / Kount
            Else
                Me.TLcatch = 0
            End If
        End If

    End Sub

	''' -----------------------------------------------------------------------
    '''<summary>
    '''Computes the following:
    '''M2(): Predator mortality for group i.
    '''Resp(i): Respiration for group i.
    '''RTZ: sum resp.  
    '''ConSum: sum of consumption.
    '''SumBio: sum of biomass.
    '''min_B_QB: minimum B*QB.
    ''' </summary>
    ''' <remarks>
    ''' Was Public Sub ParamEstimate2() in original EwE5 code. 
	''' Sept 2023: this method no longer checks for negative Respiration; this check is
	''' now better integrated in the Ecopath parameter checks
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Friend Sub Compute_M2_Resp_and_Stats(Functions As cEcoFunctions)
        Dim Prod As Single
        Dim Consump As Single, UnAssimConsump As Single
        Dim M2Sum As Single
        Dim i As Integer, j As Integer
        Dim bRespOK As Boolean = True

        Me.RTZ = 0
        Me.Consum = 0
        Me.SumBio = 0

        For i = 1 To Me.NumGroups
            If i <= Me.NumLiving Then
                Me.SumBio = Me.SumBio + Me.B(i)
                For j = 1 To Me.NumLiving
                    If Me.DC(j, i) > 0 And Me.B(i) > 0 Then M2Sum = M2Sum + Me.B(j) * Me.QB(j) * Me.DC(j, i) / Me.B(i)
                Next j
            End If
            Me.M2(i) = M2Sum
            M2Sum = 0

            If i <= Me.NumLiving Then
                If Me.QB(i) > 0 Then

                    Prod = Me.B(i) * Me.PB(i)
                    Consump = Me.B(i) * Me.QB(i)
                    UnAssimConsump = Me.GS(i) * Consump

                    'sum consumption across all the groups for Ecopath Stats
                    Me.Consum += Consump

                    Me.Resp(i) = Consump - Prod - UnAssimConsump
                    'Respiration = zero if the units are nutrients
                    If Me.areUnitCurrencyNutrients() Then
                        Me.Resp(i) = 0.0F 'Nutrient    
                    End If

                    'xxxxxxxxxxxxxxxxxxxxxxxxxxxxx
                    'jb 4-Apr-2013 Change to account for Unassimilated consumption
                    'Consum = Consum + B(i) * QB(i)
                    'Prod = EE(i) * B(i) * PB(i) + FlowToDet(i)

                    '' FlowToDet(i) is the total flow to Detritus
                    'If Me.areUnitCurrencyNutrients() Then
                    '    Resp(i) = 0 'Nutrient       B(i) * QB(i) - prod
                    'ElseIf PP(i) < 1 Then
                    '    Resp(i) = B(i) * QB(i) - (1 - PP(i)) * Prod
                    'Else
                    '    Resp(i) = B(i) * QB(i) - Prod
                    'End If
                    'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

                Else
                    'Primary producers
                    'vc resp of pp OK  RESP(i) = 0
                    Me.Resp(i) = 0.0F
                End If
            Else
                'Detritus
                'vc resp of detritus OK RESP(i) = 0
                Me.Resp(i) = 0.0F
            End If

            'Sum of respiration across all the groups
            Me.RTZ += Me.Resp(i)

        Next i

        'jb min_B_QB was called min
        Me.min_B_QB = 0
        For i = 1 To Me.NumGroups
            If Me.QB(i) > 0 Then
                If Me.min_B_QB = 0 Then Me.min_B_QB = Me.B(i) * Me.QB(i)
                If Me.min_B_QB > Me.B(i) * Me.QB(i) Then Me.min_B_QB = Me.B(i) * Me.QB(i)
            End If
        Next i

    End Sub

    ''' <summary>
    ''' Compute
    ''' Conn: Connectance Index.
    ''' SumEx: sum of export.
    ''' SumP: Sum of all production.
    ''' SysOm: System Omnivory Index.
    ''' Shannon: ShannonDiversity  ---- either Shannon
    ''' Kempton: KemptonsQ         ---- or Kempton will be shown
    ''' </summary>
    Private Sub ComputeMoreStats(fn As cEcoFunctions)
        Dim i As Integer, j As Integer, SysOmDen As Single

        For i = 1 To Me.NumLiving
            For j = 1 To Me.NumGroups
                If Me.DC(i, j) > 0 Then Me.Conn = Me.Conn + 1
            Next j
        Next i
        Me.Conn = Me.Conn / (Me.NumLiving) ^ 2  'with detritus

        'system omnivory index
        Me.SysOm = 0
        SysOmDen = 0
        'jb min_B_QB was min 
        'it is set in Compute_M2_Resp_and_Stats()
        For i = 1 To Me.NumLiving
            If Me.B(i) * Me.QB(i) / Me.min_B_QB > 0 Then    ' *** CONSUMERS ONLY
                Me.SysOm = Me.SysOm + Math.Log(Me.B(i) * Me.QB(i) / Me.min_B_QB) * Me.BQB(i)
                SysOmDen = SysOmDen + Math.Log(Me.B(i) * Me.QB(i) / Me.min_B_QB)
            End If
        Next i

        If SysOmDen > 0 Then Me.SysOm = Me.SysOm / SysOmDen

        Me.SumEx = 0
        Me.SumP = 0
        For i = 1 To Me.NumGroups
            Me.SumEx = Me.SumEx + Me.Ex(i)
            If Me.PB(i) > 0 And Me.B(i) > 0 Then Me.SumP = Me.SumP + Me.PB(i) * Me.B(i)
        Next i

        If (fn IsNot Nothing) Then
            Me.KemptonsQ = fn.KemptonsQ(Me.NumLiving, Me.TTLX, Me.B, 0.25)
            Me.Shannon = fn.ShannonDiversityIndex(Me.NumLiving, Me.B)
        End If

    End Sub

    Private Sub ComputeProfit()
        Dim Gear As Integer
        Dim Grp As Integer
        Dim value As Single

        Me.LandingValue = 0
        Me.ShadowValue = 0

        For Grp = 1 To Me.NumGroups
            For Gear = 1 To Me.NumFleet
                value = Me.Landing(Gear, Grp) * Me.Market(Gear, Grp)
                If value > 0 Then Me.LandingValue = Me.LandingValue + value
            Next
            value = Me.Shadow(Grp) * Me.B(Grp)
            If value > 0 Then Me.ShadowValue = Me.ShadowValue + value
        Next

        Me.Fixed = 0
        Me.Variab = 0
        For Gear = 1 To Me.NumFleet
            Me.Fixed = Me.Fixed + Me.cost(Gear, eCostIndex.Fixed)
            Me.Variab = Me.Variab + Me.cost(Gear, eCostIndex.CUPE) + Me.cost(Gear, eCostIndex.Sail)
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

                var = cEcopathDataStructures.PedigreeVariables(iVariable)

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
            Me.PedigreeStatsModel = cCore.NULL_VALUE
            Me.PedigreeStatsTStar = cCore.NULL_VALUE
        Else
            Dim sVar As Single = CSng(iTotal / iNumLevels)
            Me.PedigreeStatsModel = sVar
            Me.PedigreeStatsTStar = CSng(sVar * Math.Sqrt(Me.NumLiving - 2) / Math.Sqrt(1 - sVar ^ 2))
        End If

    End Sub

    Public Sub DietWasChanged(pred As Integer, prey As Integer)
        Dim j As Integer, K As Integer
        Dim FoundPredPrey As Boolean

        If (Me.DietChanged Is Nothing) Then ReDim Me.DietChanged(1, 0)

        ' j = UBound(DietChanged, 2)
        j = Me.DietChanged.GetUpperBound(1)
        For K = 0 To j
            If Me.DietChanged(0, K) = pred And Me.DietChanged(1, K) = prey Then
                FoundPredPrey = True
                Exit For
            End If
        Next

        If FoundPredPrey = False Then
            ReDim Preserve Me.DietChanged(1, j + 1)
            Me.DietChanged(0, j + 1) = pred
            Me.DietChanged(1, j + 1) = prey
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
            Me.Binput.CopyTo(Me.B, 0)
            Me.BHinput.CopyTo(Me.BH, 0)
            Me.PBinput.CopyTo(Me.PB, 0)
            Me.QBinput.CopyTo(Me.QB, 0)
            Me.GEinput.CopyTo(Me.GE, 0)
            Me.BAInput.CopyTo(Me.BA, 0)

            ' deal with EE and other mort (1-EE)
            'EEinput.CopyTo(EE, 0)
            For i As Integer = 0 To Me.NumGroups
                If Me.OtherMortinput(i) > 0 Then
                    Me.EE(i) = 1 - Me.OtherMortinput(i)
                Else
                    Me.EE(i) = Me.EEinput(i)
                End If
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
    ''' <remarks>
    ''' EwE5 performed differently here; BH() value was left at its NULL input value,
    ''' and was computed in the interface for display. I hope this doesn't mess anything up.
    ''' </remarks>
    Private Sub UpdateBH()
        For i As Integer = 1 To Me.NumGroups
            If Me.BH(i) < 0 And Me.B(i) > 0 And Me.Area(i) > 0 Then
                Me.BH(i) = Me.B(i) / Me.Area(i)
            End If
        Next
    End Sub

    ''' <summary>
    ''' Sums a <see cref="DC">Diet Composition</see> matrix to one. 
    ''' </summary>
    Public Sub SumDCToOne()

        ' For each potential predator
        For iPred As Integer = 1 To Me.NumLiving
            ' Is a consumer?
            If Me.PP(iPred) < 1 Then
                ' #Yes: calc sum
                Dim sDCSum As Single = 0.0
                ' For each of potential prey
                ' ** NOTE THAT THE LOWER BOUND USED HERE IS 0 INSTEAD OF 1! This is to include
                ' ** DC Impoprt in the calculations - which is stored at index 0.
                For iPrey As Integer = 0 To Me.NumGroups
                    ' Add consumption to sum
                    sDCSum += Me.DC(iPred, iPrey)
                Next iPrey

                ' Is there predation with a need to recalc?
                If (sDCSum > 0) And (sDCSum <> 1.0) Then
                    ' For each prey
                    ' JS 28Aug15: Rescale imports too!!!
                    For iPrey As Integer = 0 To Me.NumGroups
                        ' Rescale consumption
                        Me.DC(iPred, iPrey) = Me.DC(iPred, iPrey) / sDCSum
                    Next iPrey
                End If
            End If ' PP < 1
        Next iPred

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns whether Ecopath is set to use Nutrient-based currency units.
    ''' </summary>
    ''' <returns>True if Ecopath is set to use Nutrient-based currency units.</returns>
    ''' -----------------------------------------------------------------------
    Public Function areUnitCurrencyNutrients() As Boolean
        Return Me.ModelUnitCurrency = eUnitCurrencyType.Nitrogen Or
               Me.ModelUnitCurrency = eUnitCurrencyType.Phosporous Or
               Me.ModelUnitCurrency = eUnitCurrencyType.CustomNutrient
    End Function

#End Region

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
        Dim igrp As Integer
        Dim bGSWarning As Boolean = False
        'GS = zero if group is Primary producer
        For igrp = 1 To Me.NumGroups
            If Me.PP(igrp) = 1 Then
                Me.GS(igrp) = 0
            End If

            'Constrain GS to a percentage
            'This should not happen in EwE6 
            'This check was in a bunch of places in EwE5 it is centralized here
            'In EwE6 GS() should be constrained by the interface or the importer
            If Me.GS(igrp) > 1 Then
                Me.GS(igrp) = Me.GS(igrp) / 100
                bGSWarning = True
            End If

        Next 'For iGroup As Integer = 1 To NumGroups

        'Make backup copy of GS() 
        'that can be used to swap GS if currUnitIndex is changed
        ReDim Me.GSEng(Me.NumGroups)
        If Me.areUnitCurrencyNutrients Then
            'Model Currency is Nurtient
            'set GSEng(0) to default values
            For igrp = 1 To Me.NumLiving
                Me.GSEng(igrp) = 0.2
                If Me.PP(igrp) = 1 Then
                    Me.GS(igrp) = 0
                End If
            Next

        Else
            'Energy Currency Units
            'Make a copy of the original GS values 
            Array.Copy(Me.GS, Me.GSEng, Me.GS.Length)

        End If

        Try
            If bGSWarning Then
                Dim strmsg As String = My.Resources.CoreMessages.ECOPATH_GS_WARNING
                Me.m_messages.AddMessage(New cMessage(strmsg, eMessageType.ErrorEncountered,
                                                eCoreComponentType.Ecopath, eMessageImportance.Warning))
            End If
        Catch ex As Exception

        End Try

    End Sub

    Friend Sub copyTo(ByRef dest As cEcopathDataStructures, Optional bRedim As Boolean = True)
        Try
            'variables needed to redim
            dest.NumGroups = Me.NumGroups
            dest.NumFleet = Me.NumFleet
            dest.NumDetrit = Me.NumDetrit
            dest.NumLiving = Me.NumLiving

            If bRedim Then
                dest.redimGroups()
            End If

            dest.bInitialized = Me.bInitialized

            Me.GroupName.CopyTo(dest.GroupName, 0)    'was Specie()
            'GroupDBID.CopyTo(dest.GroupDBID, 0)        'Do not copy IDs!

            dest.NumEcosimScenarios = Me.NumEcosimScenarios
            'EcosimScenarioName.CopyTo(dest.EcosimScenarioName, 0)
            'EcosimScenarioDBID.CopyTo(dest.EcosimScenarioDBID, 0)
            'EcosimScenarioDescription.CopyTo(dest.EcosimScenarioDescription, 0)
            dest.ActiveEcosimScenario = Me.ActiveEcosimScenario

            Me.NumEcospaceScenarios = dest.NumEcospaceScenarios
            'EcospaceScenarioName.CopyTo(dest.EcospaceScenarioName, 0)
            'EcospaceScenarioDBID.CopyTo(dest.EcospaceScenarioDBID, 0)
            'EcospaceScenarioDescription.CopyTo(dest.EcospaceScenarioDescription, 0)
            'ActiveEcospaceScenario = cCore.NULL_VALUE

            Me.B.CopyTo(dest.B, 0)
            Me.BH.CopyTo(dest.BH, 0)
            Me.BA.CopyTo(dest.BA, 0)
            Me.BAInput.CopyTo(dest.BAInput, 0)
            Me.BaBi.CopyTo(dest.BaBi, 0)
            Me.PB.CopyTo(dest.PB, 0)
            Me.QB.CopyTo(dest.QB, 0)
            Me.EE.CopyTo(dest.EE, 0)
            Me.GE.CopyTo(dest.GE, 0)
            Me.GS.CopyTo(dest.GS, 0)
            Me.EEinput.CopyTo(dest.EEinput, 0)
            Me.OtherMortinput.CopyTo(dest.OtherMortinput, 0)
            Me.PBinput.CopyTo(dest.PBinput, 0)
            Me.QBinput.CopyTo(dest.QBinput, 0)
            Me.GEinput.CopyTo(dest.GEinput, 0)

            Me.Binput.CopyTo(dest.Binput, 0)

            Me.BHinput.CopyTo(dest.BHinput, 0)

            'min_B_QB = dest.min_B_QB 'minimum B*QB
            dest.DC = Me.DC.Clone
            dest.DC = Me.DC.Clone

            'dest.currUnitName = currUnitName
            dest.ModelUnitCurrency = Me.ModelUnitCurrency
            dest.TimeUnitName = Me.TimeUnitName
            dest.TimeUnitIndex = Me.TimeUnitIndex
            dest.DietsModified = Me.DietsModified
            dest.PProd = Me.PProd

            ''''DietChanged.CopyTo(dest.DietChanged, 0)

            Me.Ex.CopyTo(dest.Ex, 0)

            Me.fCatch.CopyTo(dest.fCatch, 0) 'was called Catch but this causes a naming conflict with Try Catch blocks
            Array.Copy(Me.DC, dest.DC, Me.DC.Length)
            dest.DC = Me.DC.Clone
            dest.DC = Me.DC.Clone
            dest.DF = Me.DF.Clone
            Me.Area.CopyTo(dest.Area, 0)
            dest.DCChanged = Me.DCChanged.Clone

            Me.BQB.CopyTo(dest.BQB, 0)
            Me.Resp.CopyTo(dest.Resp, 0)
            Me.PP.CopyTo(dest.PP, 0)           'TM Trophic Mode
            dest.det = Me.det.Clone
            dest.DCDet = Me.DCDet.Clone                 'Diet Composition of Detritus  for fishery            DetEaten.CopyTo(dest.DetEaten, 0)                 ' For multiple detritus
            Me.DetPassedOn.CopyTo(dest.DetPassedOn, 0)              ' For multiple detritus
            Me.DetPassedProp.CopyTo(dest.DetPassedProp, 0)              ' For multiple detritus
            Me.FlowToDet.CopyTo(dest.FlowToDet, 0)
            Me.InputToDet.CopyTo(dest.InputToDet, 0)
            'JS 08Jan09: SumDC was a global scratch variable, changed to local scope
            'SumDC.CopyTo(dest.SumDC, 0)

            Me.Immig.CopyTo(dest.Immig, 0)
            Me.Emigration.CopyTo(dest.Emigration, 0)
            Me.Emig.CopyTo(dest.Emig, 0)    'relative to biomass, used in Ecosim
            Me.Shadow.CopyTo(dest.Shadow, 0)
            Me.GroupIsFish.CopyTo(dest.GroupIsFish, 0)
            Me.GroupIsInvert.CopyTo(dest.GroupIsInvert, 0)

            dest.NumCatchCodes = Me.NumCatchCodes
            dest.PropLanded = Me.PropLanded.Clone
            Me.TTLX.CopyTo(dest.TTLX, 0)
            'JS 08Jan09: LHS was a global scratch variable, changed to local scope
            'dest.LHS = LHS.Clone
            Me.StanzaGroup.CopyTo(dest.StanzaGroup, 0)
            dest.CatchCode = Me.CatchCode.Clone
            dest.CVpar = Me.CVpar.Clone
            Me.M0.CopyTo(dest.M0, 0)
            Me.M2.CopyTo(dest.M2, 0)
            dest.Path = Me.Path.Clone
            dest.LastComp = Me.LastComp.Clone
            Me.DtImp.CopyTo(dest.DtImp, 0)

            ''fishing(variables)
            dest.NoGearData = Me.NoGearData
            dest.cost = Me.cost.Clone
            dest.CostPct = Me.CostPct.Clone
            dest.Discard = Me.Discard.Clone
            dest.DiscardFate = Me.DiscardFate.Clone
            Me.FleetName.CopyTo(dest.FleetName, 0)
            dest.NominalEffort = Me.NominalEffort.Clone
            dest.Landing = Me.Landing.Clone
            dest.Market = Me.Market.Clone
            dest.PropDiscard = Me.PropDiscard.Clone

            dest.RTZ = Me.RTZ
            dest.Consum = Me.Consum
            dest.SumBio = Me.SumBio
            dest.CatchSum = Me.CatchSum
            dest.GEff = Me.GEff
            dest.Totpp = Me.Totpp
            dest.TLcatch = Me.TLcatch
            dest.Dt = Me.Dt
            dest.SumEx = Me.SumEx
            dest.SumP = Me.SumP
            dest.Conn = Me.Conn
            dest.SysOm = Me.SysOm

            Me.vbK.CopyTo(dest.vbK, 0)
            dest.Hlap = Me.Hlap.Clone
            dest.Plap = Me.Plap.Clone
            Me.GroupColor.CopyTo(dest.GroupColor, 0)
            Me.FleetColor.CopyTo(dest.FleetColor, 0)
            dest.Host = Me.Host.Clone
            Me.mis.CopyTo(dest.mis, 0)

            ' Copy model data
            dest.ModelArea = Me.ModelArea
            dest.ModelAuthor = Me.ModelAuthor
            dest.ModelContact = Me.ModelContact
            dest.ModelDescription = Me.ModelDescription
            dest.ModelEast = Me.ModelEast
            dest.ModelGroupDigits = Me.ModelGroupDigits
            dest.ModelName = Me.ModelName
            dest.ModelNorth = Me.ModelNorth
            dest.ModelNumDigits = Me.ModelNumDigits
            dest.ModelSouth = Me.ModelSouth
            dest.ModelUnitCurrency = Me.ModelUnitCurrency
            dest.ModelUnitCurrencyCustom = Me.ModelUnitCurrencyCustom
            dest.ModelUnitMonetary = Me.ModelUnitMonetary
            dest.ModelUnitTime = Me.ModelUnitTime
            dest.ModelUnitTimeCustom = Me.ModelUnitTimeCustom
            dest.ModelWest = Me.ModelWest
            dest.FirstYear = Me.FirstYear

        Catch ex2 As Exception
            Debug.Assert(False, ex2.Message)
        End Try

    End Sub

End Class
