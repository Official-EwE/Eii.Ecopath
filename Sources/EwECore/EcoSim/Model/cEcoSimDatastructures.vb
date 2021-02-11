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

'ToDo: Enable Option Strict On
'Option Strict On

Imports EwEUtils.Core


''' <summary>
''' This class wraps the underlying EcoSim data structures
''' </summary>
''' <remarks>
''' 
''' </remarks>
Public Class cEcosimDatastructures

    Public Const DEFAULT_N_FORCINGPOINTS As Integer = 1200 'min number of forcing point 100 years * FORCING_POINTS_PER_YEAR
    Public Const FORCING_POINTS_PER_YEAR As Integer = 12

    ''' <summary>
    ''' Enumerated index to the type of Ecosim Data saved over time
    ''' </summary>
    ''' <remarks>This is the index to the first element in ResultsOverTime(eEcosimResults, igroup, itime) that specifies the data being saved at each time step across groups</remarks>
    Public Enum eEcosimResults
        Biomass
        BiomassRel
        Yield
        YieldRel
        FeedingTime
        ConsumpBiomass
        TotalMort
        PredMort
        FishMort
        ProdConsump
        AvgWeight
        MortVPred
        MortVFishing
        EcoSysStructure
        TL
    End Enum

    Public Enum eEcosimPreyPredResults
        Prey
        Pred
        Consumption
    End Enum

    ''' <summary>
    ''' Mediation data for Biomass Mediation
    ''' </summary>
    ''' <remarks></remarks>
    Public BioMedData As New cMediationDataStructures

    ''' <summary>
    ''' Mediation data for Price Elasticity (mediation function)
    ''' </summary>
    ''' <remarks></remarks>
    Public PriceMedData As New cMediationDataStructures

    ''' <summary>
    ''' Capacity Environmental Response functions (mediation functions). 
    ''' Shape to convert input value to capacity value e.g. CapacityMap(irow,icol) = F(InputMap(irow,icol)) (capacity as a function of X)
    ''' </summary>
    ''' <remarks>
    ''' Generical Environmental Response functions are used to convert an enviromental input value to a value used in either Ecosim(time series input data) or Ecospace(spatial/temporal data)
    ''' For Capacity Environmental Response functions are used in conjunction with a cEnviroInputMap(of T) to populate the Capacity map
    ''' </remarks>
    Public CapEnvResData As New cMediationDataStructures

    ''' <summary>
    ''' Boolean flag set by the calling routine to tell Ecosim if it should process the output timestep data cEcoSImModel.ProcessTimeStep()
    ''' </summary>
    ''' <remarks>If true then Ecosim will compute all output data including data over time and call the timestep delegate. If false then it will run in a silent mode and not compute output data  </remarks>
    Public bTimestepOutput As Boolean

    ''' <summary>Array of Ecosim group database IDs.</summary>
    Public GroupDBID() As Integer
    ''' <summary>Array of Ecosim fleet database IDs.</summary>
    Public FleetDBID() As Integer

    ''' <summary>Total number of groups in the model.</summary>
    Public nGroups As Integer
    ''' <summary>Total number of fleets in the model.</summary>
    Public nGear As Integer

    Public FirstTime As Boolean

    ' Public ConSimOn As Boolean
    Public TrophicOff As Boolean
    Public IndicesOn As Boolean
    Public UseVarPQ As Boolean
    Public NudgeChecked As Boolean
    Public Integrate As Boolean
    Public AbortRun As Boolean
    Public EvolveIsOn As Boolean
    Public BiomassOn As Boolean
    Public ActivePair As Integer
    Public ForagingTimeLowerLimit As Single = 0.01!

    ''In the original code
    ''CperFlag is set to true when the EcoSim main form is loaded it is never set to false 
    ''this means it has no effect as EcoSim cannot be run without loading the form
    'Public CperFlag As Boolean

    ''' <summary>Duration of simulation (years).</summary>
    Public NumYears As Integer
    ''' <summary>Number of steps per year.</summary>
    Public NumStepsPerYear As Integer = cCore.N_MONTHS
    ''' <summary>Integration steps (per year).</summary>
    Public StepSize As Single

    ''' <summary>Relaxation parameter [0,1].</summary>
    Public SorWt As Single
    ''' <summary>Discount rate (% per year).</summary>
    Public Discount As Single
    ''' <summary>Equilibrium step size.</summary>
    Public EquilibriumStepSize As Single
    ''' <summary>Equilibrium max. fishing rate (relative).</summary>
    Public EquilScaleMax As Single
    ''' <summary>Base proportion of free nutrients.</summary>
    Public NutBaseFreeProp As Single

    Public ReadOnly Property NumEnvResponseFunctions As Integer
        Get
            Return Me.NumForcingShapes
        End Get
    End Property

    ''' <summary>
    ''' Index of the Response function that has been applied to this EnviromentalDrive and Group (driver,group)
    ''' </summary>
    ''' <remarks>EnvRespFuncIndex(1,2) = 10 means that the tenth response function has been applied to the first environmental driver and second group</remarks>
    Public EnvRespFuncIndex(,) As Integer

    ''' <summary>
    ''' Index of the Mortality Response function that has been applied to this EnviromentalDrive and Group (driver,group)
    ''' </summary>
    ''' <remarks>MortalityRespFuncIndex(1,2) = 10 means that the tenth response function has been applied to the first environmental driver and second group</remarks>
    Public MortalityRespFuncIndex(,) As Integer

    'dimensions for nutrient calculation
    Public NutMin As Single

    ''' <summary>Sum of biomass across all groups </summary>
    Public NutBiom As Single

    ''' <summary>Total nutrient bound in system </summary>
    ''' <remarks>NutTot = NutBiom / (1 - NutBaseFreeProp)</remarks>
    Public NutTot As Single

    ''' <summary>Nutrient free in the enviroment  </summary>
    ''' <remarks>NutFree = NutTot - NutBiom</remarks>
    Public NutFree As Single
    Public NutFreeBase() As Single

    Public VulMultAll As Single

    ''' <summary>
    ''' Vulnerability multiplier of a prey to a predator
    ''' </summary>
    ''' <remarks>VulMult(iPrey,iPred) User entered value to increase the vulnerability of a prey</remarks>
    Public VulMult(,) As Single
    Public vulrate(,) As Single

    Public Epower() As Single
    Public PcapBase() As Single
    Public CapDepreciate() As Single
    Public CapBaseGrowth() As Single

    ''' <summary>Toggle to enable TL calculations during searches. Normally this 
    ''' is not enabled because TL info is not used by searches.</summary>
    Public bAlwaysCalcTLc As Boolean = False

    ''' <summary>TL of catch (x time)</summary>
    Public TLC() As Single
    ''' <summary>FIB index (x time)</summary>
    Public FIB() As Single
    ''' <summary>TL based on Ecosim diets (x group)</summary>
    Public TLSim() As Single
    ''' <summary>Total catch per timestep</summary>
    Public CatchSim() As Single
    ''' <summary>Kemptons's Q</summary>
    Public Kemptons() As Single
    ''' <summary>Shannon Diversity Index</summary>
    Public ShannonDiversity() As Single


    ''' <summary> Max vulnerability across all prey for this predator VulnerabilityPredator(pred) = max(VulMult(prey,pred))</summary>
    Public VulnerabilityPredator() As Single

    Public maxflow(,) As Single

    Public FlowType(,) As Single

    Public Eatenof() As Single
    Public Eatenby() As Single
    Public simDCAtT(,) As Single

    ''' <summary>Nutrient loading forcing function number. This is an index in the tval() array.</summary>
    Public NutForceNumber As Integer
    ''' <summary>Max PB/(Base PB) due to nutrient concent.</summary>
    Public NutPBmax As Single
    ''' <summary>System recovery (+/- %).</summary>
    Public SystemRecovery As Single

    'dimensioned by nGroups
    ''' <summary>Max relative feeding time.</summary>
    Public FtimeMax() As Single
    ''' <summary>Feeding time adjustment rate (0-1).</summary>
    Public FtimeAdjust() As Single
    ''' <summary>Fraction of other mortality.</summary>
    Public MoPred() As Single

    ''' <summary>
    ''' Mortality other computed as (1-ee)*pb
    ''' </summary>
    ''' <remarks></remarks>
    Public mo() As Single

    ''' <summary>Predation effect on feeding time (0-1).</summary>
    Public RiskTime() As Single
    ''' <summary>Density-dependant catchability QMax/Qo.</summary> 
    Public QmQo() As Single
    ''' <summary>QBmax/QBo for handling time > 1.</summary> 
    Public CmCo() As Single
    ''' <summary>Switching power parameter (0-2).</summary>
    Public SwitchPower() As Single

    Public BaseTimeSwitch() As Single

    'jb moved vbK to Ecopath
    'Public vbK() As Single 'VBGF curvature parameter K (/year)
    Public PBmaxs() As Single 'max relative P/B

    Public RecPower() As Single

    Public Emig() As Single    'relative to biomass,

    ''' <summary>
    ''' Base consumption on a prey by a predator 
    ''' </summary>
    ''' <remarks>
    ''' Consumption(iPrey,iPred) 
    ''' computed in <see cref="Ecosim.cEcoSimModel.CalcEatenOfBy">Ecosim.CalcEatenOfBy</see>
    ''' </remarks>
    Public Consumption(,) As Single

    Public Htime() As Single

    Public SimDC(,) As Single 'diet composition for EcoSim
    'dimensioned by nPairs holds the iGroup (index) of the adult for this pair
    'i.e. GroupName(iadult(1)) is the group name of the adult for the first pair (assuming there is at least one pair)
    Public iadult() As Single
    Public ijuv() As Single 'same as iadult() but for the juvenile

    Public TimeJuv() As Single
    Public maxtimejuv() As Single
    Public mintimejuv() As Single

    ''' <summary>
    ''' Flag for doing integration in rk4 for each group
    ''' </summary>
    ''' <remarks>This can be set to one of several value:
    ''' value = i "NoIntegrate(1) = 1" do the normal integration,
    ''' value = 0 "NoIntegrate(1) = 0" no integration,
    ''' value less than 0 "NoIntegrate(2)= -2" this is a stanza group the final integration is handled by SplitUpdate()
    ''' this was also used to tell if a group is part of a splitpool 
    ''' </remarks>
    Public NoIntegrate() As Integer

    ''' <summary>Mortality due to fishing FCatch(group) / EcopathBiomass(group) by group </summary>
    ''' <remarks>Initialized in SetupSimVariables() </remarks>
    Public Fish1() As Single

    ''' <summary>
    ''' Mortality due to fishing at the current time step
    ''' </summary>
    Public FishTime() As Single

    ''' <summary>Max catch rate ??? </summary>
    ''' <remarks>stored in database defaults set in SetUpSimVariables(). This may not be neccessary in EwE6 it was only used for scaling in EwE5</remarks>
    Public FishRateMax() As Single

    ''' <summary>
    ''' Fishing mortality by Fleet, Group
    ''' </summary>
    ''' <remarks> Array element FishMGear(nFleets + 1,iGroup) will contain the sum across all fleets and should be the same as Fish1()</remarks>
    Public FishMGear(,) As Single

    ''' <summary>
    ''' Fishing mortality over time for each group.
    ''' </summary>
    ''' <remarks>Fishing mortality by group-time. It's default value is Fish1() Catch/Biomass set in DefaultFishMortalityRates(). 
    ''' It is used as a forcing/driving value over time. It is used to compute FishTime() </remarks>
    Public FishRateNo(,) As Single
    Public FishRateNoDBID() As Integer
    Public FishRateNoTitle() As String
    ''' <summary>Fish rate no shape DBID per group</summary>
    Friend GroupFishRateNoDBID() As Integer

    ''' <summary>
    ''' Fishing Effort multiplier relative to Ecopath base, by Fleet, Time.
    ''' </summary>
    ''' <remarks>Zero removes all fishing, one sets fishing effort to Ecopath value, two would double the fishing mortality for all groups fished by this fleet.
    '''  Used to scale the FishRateNo() for all the groups fished by a fleet.</remarks>
    Public FishRateGear(,) As Single
    Public FishRateGearBasis() As Single
    Public FishRateGearDBID() As Integer
    Public FishRateGearTitle() As String

    ''' <summary>
    ''' Feeding Time scaling value
    ''' </summary>
    ''' <remarks>Default value = one set in InitialState. Computed at the end of each time step in rk4()</remarks>
    Public Ftime() As Single
    Public Hden() As Single

    Public QBoutside() As Single

    ''' <summary>
    ''' Base rate of Detritus accumulation ([accumulated detritus biomass]/[biomass t=0]) calculated in <see cref="EwECore.Ecosim.cEcoSimModel.SimDetritusMT">SimDetritusMT</see>.
    ''' </summary>
    ''' <remarks>
    ''' Calculated during the initialization of Ecosim by <see cref="EwECore.Ecosim.cEcoSimModel.Init">Init()</see> and <see cref="EwECore.Ecosim.cEcoSimModel.SetTimeSteps">SetTimeSteps()</see>. 
    ''' Used by both Ecosim and Ecospace. When Ecospace initializes it calls Ecosim.Init() this sets DetritusOut() to the base Ecosim values. 
    ''' This avoids any issues with setting the initial biomass or threading races. 
    ''' </remarks>
    Public DetritusOut() As Single
    Public AssimEff() As Single
    Public SimGE() As Single

    Public StartBiomass() As Single

    ''' <summary>pbbiomass = (PB * MaxPB / PB - 1) / B </summary>
    ''' <remarks>For primary producers only. Will be zero for all other groups</remarks>
    Public pbbiomass() As Single
    Public loss() As Single

    Public Cbase() As Single

    ''' <summary>
    '''  Catch Rate at Ecopath base. Include all catch even discards that survive. set in SetRelativeCatchabilities
    ''' </summary>
    ''' <remarks>EcopathCatch / StartBiomass</remarks>
    Public relQ(,) As Single

    Public relQt(,,) As Single ' ToDo: Init to cCore.NULL when redimensioning

    'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
    'Forcing & Mediation Functions
    'Added or Changed Varaibles
    'jb April-06-2006 added to keep track of the type of a forcing shape (time or Egg)this does not include mediation shapes as they are stored seperately
    Public ForcingShapeType() As eDataTypes

    Public Elect(,,) As Single
    ''' <summary>
    '''  Mortalily rate due to predation by Link
    ''' </summary>
    Public MPred() As Single

    ''' <summary>
    '''  Detritus from all sources by group
    ''' </summary>
    Public GroupDetritus() As Single

    'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
    'Variables for non additive mortality rates
    ''' <summary>
    '''  PaddP is proportion of predation rate that is additive
    ''' </summary>
    Public PaddP() As Single
    ''' <summary>
    ''' Predation mortality rate at base
    ''' </summary>
    Public MoPredBase() As Single

    'Public startEatenOf() As Single

    ''' <summary>
    ''' 1 / PaddP - 1
    ''' </summary>
    Public PhHalf() As Single



    Public moTot() As Single

    Public moMax() As Single

    Public Qh() As Single

    'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx


    ''' <summary>
    ''' Structure to contain all settings that wrap the primitive defining the contents of a forcing or time shape
    ''' </summary>
    ''' <remarks>
    ''' jb April-07-2006 replaced Shapes() with ShapeParameters
    ''' </remarks>
    Public Structure ShapeParameters

        ''' <summary>
        ''' The <seealso cref="eShapeFunctionType">primite function</seealso> 
        ''' that defined the content of a forcing or time shape.
        ''' </summary>
        ''' <remarks>
        ''' 0 must be supplied if there is no underlying primitive function. 
        ''' Note that plug-ins can add their own ShapeFunctionType, values will
        ''' not be restricted to <see cref="eShapeFunctionType"/>.
        ''' </remarks>
        Public ShapeFunctionType As Long

        ''' <summary>
        ''' The parameters for <see cref="ShapeFunctionType"/>.
        ''' </summary>
        Public ShapeFunctionParams As Single()

        ''' <summary>
        ''' The number of parameters for <see cref="ShapeFunctionType"/>.
        ''' </summary>
        Public ReadOnly Property nShapeFunctionParams As Integer
            Get
                If (Me.ShapeFunctionParams Is Nothing) Then Return 0
                Return Me.ShapeFunctionParams.Length
            End Get
        End Property

        ''' <summary>
        ''' Get/set the value of a <see cref="ShapeFunctionParams"/>.
        ''' </summary>
        ''' <param name="iParam">Zero-based param index</param>
        Public Property ShapeFunctionParam(iParam As Integer) As Single
            Get
                If (iParam < 0 Or iParam >= Me.nShapeFunctionParams) Then Return 0
                Return Me.ShapeFunctionParams(iParam)
            End Get
            Set(value As Single)
                If (iParam < 0 Or iParam >= Me.nShapeFunctionParams) Then Return
                Me.ShapeFunctionParams(iParam) = value
            End Set
        End Property

    End Structure

    'there is one ShapeParameters array for each type of shape that has parameters Mediation and Forcing
    'Public MediationShapeParams() As ShapeParameters 'parameters that where used to create a curve from the Database Table and Fields i.e. EcoSimShapes.YZero
    Public ForcingShapeParams() As ShapeParameters 'Time and EggProd

    ''' <summary>
    ''' Unique database IDs for forcing shapes.
    ''' </summary>
    ''' <remarks>
    ''' Because Time(Forcing) and EggProd shapes are stored in the same arrays and this is for both shape types
    ''' </remarks>
    Public ForcingDBIDs() As Integer
    'is this shape a seasonal forcing shape
    Public isSeasonal() As Boolean

    ''' <summary>Total number of links/flow between groups</summary>
    Public inlinks As Integer
    ''' <summary>iPrey for inlinks</summary>
    Public ilink() As Integer '
    ''' <summary>iPred for inlinks</summary>
    Public jlink() As Integer

    'Forcing
    Public NumForcingShapes As Integer
    Public ForcingTitles() As String

    Public ForcePoints As Integer = DEFAULT_N_FORCINGPOINTS 'number of points per forcing function
    Public ZmaxScale As Single
    Public zscale(,) As Single 'ReDim Preserve zscale(ForcePoints, ForcingShapes + 3) 
    Public tval() As Single

    Public EggProdShape() As Single
    'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

    ''' <summary> Max PB = PBmax*PB </summary>
    ''' <remarks></remarks>
    Public pbm() As Single
    Public pred() As Single
    Public Qmain() As Single
    Public Qrisk() As Single

    Public Consumpt(,) As Single

    Public DCPct(,) As Single

    'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
    'Model Results Over Time for selected variable
    '
    ''' <summary>Number of timesteps in the summary data</summary>
    Public nSumTimeSteps As Integer

    ''' <summary>
    ''' Model results over time (number of variables x groups x time)
    ''' </summary>
    Public ResultsOverTime(,,) As Single
    ''' <summary>pred/prey(2) x groups x groups x time</summary>
    Public PredPreyResultsOverTime(,,,) As Single
    ''' <summary>pred/prey(2) x groups x groups</summary>
    Public ResultsAvgByPreyPred(,,) As Single
    ''' <summary>Group x fleets x time.</summary>
    Public ResultsSumCatchByGroupGear(,,) As Single
    ''' <summary>fleets x time</summary>
    Public ResultsSumCatchByGear(,) As Single

    ''' <summary>Landings by Group, Fleet</summary>
    Public ResultsLandings(,) As Single

    Public ResultsDiscardsMort(,) As Single
    Public ResultsDiscardsSurvived(,) As Single

    ''' <summary>Total discards by Group, fleets, time.</summary>
    Public ResultsTimeDiscardsGroupGear(,,) As Single
    ''' <summary>Discards that suffered mortality by Group, fleets, time.</summary>
    Public ResultsTimeDiscardsMortGroupGear(,,) As Single
    ''' <summary>Discards that survived by Group, fleets, time.</summary>
    Public ResultsTimeDiscardsSurvivedGroupGear(,,) As Single

    ''' <summary>
    ''' Landing by Group, Fleet, Time.
    ''' </summary>
    Public ResultsTimeLandingsGroupGear(,,) As Single

    ''' <summary>
    ''' Fishing mortality by time
    ''' </summary>
    Public ResultsSumFMortByGroupGear(,,) As Single ' groups,fleets,time

    Public ResultsSumValueByGroupGear(,,) As Single
    Public ResultsSumValueByGear(,) As Single
    Public ResultsEffort(,) As Single

    Public ResultsSumRelValueByGroup(,) As Single

    ''' <summary>Summarized Profit from results </summary>
    Public ProfitByFleet() As Single
    ''' <summary>Summarized Jobs from results </summary>
    Public EmploymentValueByFleet() As Single

    'xxxxxxxxxxxxxxxxxxxxxxxx

    ''' <summary>Number of time steps for averaging results.</summary>
    Public NumStep As Integer
    Public NumStep0 As Integer      'Actual number of steps for the zero element of the summary arrays Start summary time period
    Public NumStep1 As Integer      'Actual number of steps for the one element of the summary arrays end summary time peroid

    ''' <summary>Start time of the first and second summary data period. In Years </summary>
    ''' <remarks> Data is summarized over two time periods set by SumStart(0) and SumStart(1). The number of time steps to summarize over is set in NumStep.
    ''' Defaults are set in redimTimeVaraibles().
    ''' Used in cEcospace.summarySetTimeStep() to set the index to store the summary data in. The first or second summary period.
    ''' </remarks>
    Public SumStart(1) As Single

    'storage for summary time period data
    Public SumBiomass(,) As Single 'SumBiomass(iTimePeriod,iGroup)
    'catch by group
    Public SumCatch(,) As Single

    'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
    'new foraging arena variables
    Public Narena As Integer, Iarena() As Integer, Jarena() As Integer
    ''' <summary>The arena number (prey, pred)</summary>
    ''' <example> [proportion of consumption by pred in arena] = PeatArena(ArenaNo(iprey,ipred),ipred) </example>
    Public ArenaNo(,) As Integer
    Public VulArena() As Single
    Public Alink() As Single
    ''' <summary> i (prey) index of foraging arena with positive feeding on prey i by predator k </summary>
    Public IlinkSet() As Integer
    ''' <summary> j (pred) index of foraging arena having positive feeding on i by predator k </summary>
    Public JlinkSet() As Integer
    ''' <summary> index of predator whose peatarea for arena i,j is stored in list element </summary>
    Public KlinkSet() As Integer ' 
    ''' <summary> diet proportions by foraging arena from/to database </summary>
    Public PeatArena(,) As Single '
    ''' <summary> Arena Number for a pred, prey link when PeatArena(ArenaNo(iprey,jpred),jpred)>0. This pred eats from this arena.  </summary>
    Public ArenaLink() As Integer
    ''' <summary> total ecopath base consumption by trophic link </summary>
    Public Qlink() As Single '
    ''' <summary> note number of arena foraging links set from or to database </summary>
    Public NlinksSet As Integer '
    Public BoutFeeding As Boolean 'this needs an interface
    'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

    Public RelaSwitch() As Single

    'moved here for Network Analysis plugin
    Public ToDetritus() As Single

    Public FisForced() As Boolean

    ''' <summary>
    ''' Sum of squares fit to reference data
    ''' </summary>
    Public SS As Single

    ''' <summary>
    ''' Sum of squares by group
    ''' </summary>
    ''' <remarks></remarks>
    Public SSGroup() As Single

    Public PredictSimEffort As Boolean

    ''' <summary>
    ''' Boolean flag that tells Ecosim to run on it's own thread.
    ''' </summary>
    ''' <remarks>
    ''' True if Ecosim is running on a seperate thread. False otherwise. 
    ''' This flag can only be set from code and it not available from the scientific interface. 
    ''' The scientific interface is suppose to be thread safe but plugin interfaces may NOT be. 
    ''' Once the Ecosim run has completed bMultiThreaded will be set to False. bMultiThreaded will only be true for one run.
    ''' </remarks>
    Public bMultiThreaded As Boolean

    ''' <summary>
    ''' Number of sub timesteps Ecosim will run per month 
    ''' </summary>
    ''' <remarks>
    ''' Monthly timesteps in Ecosim can be divided into multiple sub timesteps. The number of sub timesteps in set via the cEcosimDatastructures.StepsPerMonth which has a default of one.  
    ''' This allows a plugin to run Ecosim with more then 12 timesteps per year. Once Ecosim has run it will reset cEcosimDatastructures.StepsPerMonth to its default value of one 
    ''' all subsequent runs of Ecosim will be on a monthly timestep unless cEcosimDatastructures.StepsPerMonth has been reset before the next run. 
    ''' This funtionality is only available via code and has no user interface. 
    ''' User interface objects e.g. cCore.EcosimGroupOutputs are NOT update for sub timesteps and will not be updated until the end of the monthly timestep. 
    ''' </remarks>
    Public StepsPerMonth As Integer

    ''' <summary>Proportion of regulated landings (by gear group) for the current time step</summary>
    Public PropLandedTime(,) As Single

    ''' <summary>
    ''' Proportion of the total catch that suffered mortality for the current time step (by gear group). Does not include discards that survived.
    ''' Initialized in cEcosim.InitPropLanded() Propdiscardtime(iflt, igrp) = PropDiscard(iflt, igrp) * PropDiscardMort(iflt, igrp)
    ''' </summary>
    Public Propdiscardtime(,) As Single

    Public PropDiscardMortTime(,) As Single


    ''' <summary>
    ''' Unit conversion factor for fishing effort 
    ''' </summary>
    ''' <remarks>Used to sum effort into a single output map</remarks>
    Public EffortConversionFactor() As Single


    Public lstEnviroInputData As List(Of IEnviroInputData)

    Public EcosimEnvResFunctions As New cMediationDataStructures

    Public Sub RedimVars()

        'jb I don't know why these where split up there may be some kind of a reason
        Me.RedimVariabs1()
        Me.RedimVariabs2()
        'jb added this was in ReadStanza
        '  redimStanza()

    End Sub

    ''' <summary>
    ''' Redims the arenas.
    ''' </summary>
    ''' <remarks>
    ''' Make sure both <see cref="Narena"/> and <see cref="NlinksSet"/> are properly initialized prior to redimensioning arenas
    ''' </remarks>
    Public Sub RedimArenaLinks()

        ReDim Me.IlinkSet(Me.NlinksSet)
        ReDim Me.JlinkSet(Me.NlinksSet)
        ReDim Me.KlinkSet(Me.NlinksSet)
        ReDim Me.PeatArena(Me.Narena, Me.nGroups)

    End Sub

    ''' <summary>
    ''' Validate and patch up shared arena use.
    ''' </summary>
    ''' <remarks>
    ''' This stage is needed to ensure that shared arenas, as read by a datasource,
    ''' are in sync with the consumption links in Ecosim.
    ''' </remarks>
    Public Sub ValidateSharedArenas()

        ' Indices of ilinksset that refer to an invalid i, j or k
        Dim iPred, iPrey, iPredShared, nPending As Integer
        Dim pending(Me.nGroups, Me.nGroups) As Boolean
        Dim lInvalidLinks As New List(Of Integer)

        ' Count up all the dietary links for which one or more arena links are needed
        For iPrey = 1 To Me.nGroups
            For iPred = 1 To Me.nGroups
                If (Me.Consumption(iPrey, iPred) > 0.0F) Then
                    pending(iPrey, iPred) = True
                    nPending += 1
                End If
            Next iPred
        Next iPrey

        'Debug.WriteLine("ShArenas: ValidateSharedArenas {0} diets, {1} NLinksSet", nComps, NlinksSet)

        ' Tick off all shared arenas that match a dietary link
        For i As Integer = 1 To Me.NlinksSet
            ' Is link invalid?
            If (Me.IlinkSet(i) = 0) Or (Me.JlinkSet(i) = 0) Or (Me.KlinkSet(i) = 0) Then
                ' #Yes: remember bad link, will be removed below
                lInvalidLinks.Add(i)
            ElseIf pending(Me.IlinkSet(i), Me.KlinkSet(i)) Then
                ' #No: tick off a good link: this pred/prey link has been presented in at least one arena
                pending(Me.IlinkSet(i), Me.JlinkSet(i)) = False
                nPending -= 1
            End If
        Next

        'Debug.WriteLine("ShArenas: ValidateSharedArenas {0} links miss defaults, {1} links are gone", nComps, lInvalidLinks.Count)

        ' Need to resize arena links?
        If (nPending > 0 Or lInvalidLinks.Count > 0) Then
            ' Make backup of current link data as set by the datasource. We'll reuse the useful bits
            Dim iLinksOld As Integer = Me.NlinksSet
            Dim ilinkcopy As Integer() = Me.IlinkSet
            Dim jlinkcopy As Integer() = Me.JlinkSet
            Dim klinkcopy As Integer() = Me.KlinkSet
            Dim peatcopy As Single(,) = Me.PeatArena
            Dim n As Integer = 1

            ' Resize
            Me.NlinksSet = Me.NlinksSet - lInvalidLinks.Count + nPending
            Me.RedimArenaLinks()

            ' Step 1: copy useful bits 
            For i As Integer = 1 To iLinksOld
                ' Not a bad link?
                If Not lInvalidLinks.Contains(i) Then
                    ' #OK: map old arena record to new arena record
                    iPrey = ilinkcopy(i)
                    iPred = jlinkcopy(i)
                    iPredShared = klinkcopy(i)
                    Dim iArena As Integer = Me.ArenaNo(iPrey, iPred)

                    Me.IlinkSet(n) = iPrey
                    Me.JlinkSet(n) = iPred
                    Me.KlinkSet(n) = iPredShared
                    Me.PeatArena(iArena, iPredShared) = peatcopy(iArena, iPredShared)
                    n += 1
                End If
            Next

            ' Step 2: Complement missing defaults (and yes, this entire routine can be seriously optimized with dictionaries)
            For iPrey = 1 To Me.nGroups
                For iPred = 1 To Me.nGroups
                    ' Is this a diet link that does not yet have an arena link?
                    If pending(iPrey, iPred) Then
                        ' #Yes: add a default
                        Me.IlinkSet(n) = iPrey
                        Me.JlinkSet(n) = iPred
                        Me.KlinkSet(n) = iPred
                        Dim iArenaTo As Integer = Me.ArenaNo(iPrey, iPred)
                        Me.PeatArena(iArenaTo, iPred) = 1

                        ' Next
                        n += 1

                        ' Not necessary, but useful for accounting and debugging:
                        ' - Flag as done
                        pending(iPrey, iPred) = False
                        nPending -= 1

                    End If
                Next iPred
            Next iPrey

            ' Are you done, Donald? (hey, how about MEGA: Make Ecosystems Great Again?)
            Debug.Assert(nPending = 0)

            'Debug.WriteLine("ShArenas: ValidateSharedArenas resized to {0} ", NlinksSet)

        End If

    End Sub

    Private Sub RedimVariabs2()

        ReDim Me.Consumption(Me.nGroups, Me.nGroups)
        ReDim Me.Consumpt(Me.nGroups, Me.nGroups)
        ReDim Me.Eatenby(Me.nGroups)
        ReDim Me.Eatenof(Me.nGroups)
        ReDim Me.pred(Me.nGroups)
        ReDim Me.simDCAtT(Me.nGroups, Me.nGroups)
        ReDim Me.DCPct(Me.nGroups, 3) 'used for B1Round, B2Round, QB, derivt (BA)

    End Sub


    Private Sub RedimVariabs1()
        Dim i, j As Integer

        ReDim Me.GroupDBID(Me.nGroups)

        'ReDim BaseTimeSwitch(nGroups)
        ReDim Me.SwitchPower(Me.nGroups)
        Me.NutBaseFreeProp = 0.9999
        Me.NutForceNumber = 0
        Me.NutPBmax = 1.5

        ReDim Me.Emig(Me.nGroups)
        ReDim Me.QmQo(Me.nGroups), Me.Htime(Me.nGroups) ', Hden(nGroups)
        ReDim Me.CmCo(Me.nGroups)

        ReDim Me.Qmain(Me.nGroups)
        ReDim Me.Qrisk(Me.nGroups)
        ReDim Me.RiskTime(Me.nGroups)
        ReDim Me.Consumption(Me.nGroups, Me.nGroups)
        ReDim Me.StartBiomass(Me.nGroups)

        ReDim Me.Eatenby(Me.nGroups)
        ReDim Me.Eatenof(Me.nGroups)
        ReDim Me.EggProdShape(CInt(Me.nGroups / 2))

        ReDim Me.FtimeAdjust(Me.nGroups)
        ReDim Me.MoPred(Me.nGroups)

        ReDim Me.iadult(CInt(Me.nGroups / 2))
        ReDim Me.ijuv(CInt(Me.nGroups / 2))

        ReDim Me.TimeJuv(CInt(Me.nGroups / 2)) 'Time spent in juv stage
        ReDim Me.maxtimejuv(CInt(Me.nGroups / 2))
        ReDim Me.mintimejuv(CInt(Me.nGroups / 2))
        ReDim Me.NoIntegrate(Me.nGroups)
        ReDim Me.pbm(Me.nGroups)
        ReDim Me.PBmaxs(Me.nGroups)
        ReDim Me.pred(Me.nGroups)
        ReDim Me.RecPower(CInt(Me.nGroups / 2))

        ReDim Me.ilink(Me.nGroups * Me.nGroups)
        ReDim Me.jlink(Me.nGroups * Me.nGroups)
        ReDim Me.SimDC(Me.nGroups, Me.nGroups)
        ReDim Me.MPred(Me.nGroups * Me.nGroups)

        ReDim Me.IlinkSet(Me.nGroups * Me.nGroups)
        ReDim Me.JlinkSet(Me.nGroups * Me.nGroups)
        ReDim Me.KlinkSet(Me.nGroups * Me.nGroups)
        ReDim Me.PeatArena(Me.nGroups * Me.nGroups, Me.nGroups)

        ReDim Me.vulrate(Me.nGroups, Me.nGroups)
        ReDim Me.VulMult(Me.nGroups, Me.nGroups)
        For i = 1 To Me.nGroups : For j = 1 To Me.nGroups : Me.vulrate(i, j) = 1.0! : Me.VulMult(i, j) = 2.0! : Next j : Next i
        ReDim Me.VulnerabilityPredator(Me.nGroups)

        ReDim Me.Fish1(Me.nGroups)
        ReDim Me.FishRateNoDBID(Me.nGroups)
        ReDim Me.FishRateNoTitle(Me.nGroups)
        ReDim Me.GroupFishRateNoDBID(Me.nGroups)

        'the plus one is for combined fleets
        ReDim Me.FleetDBID(Me.nGear + 1)
        ReDim Me.FishRateGearDBID(Me.nGear + 1)
        ReDim Me.FishRateGearBasis(Me.nGear + 1)
        ReDim Me.FishRateGearTitle(Me.nGear + 1)
        ReDim Me.FishMGear(Me.nGear + 1, Me.nGroups)

        ReDim Me.FishRateMax(Me.nGroups)

        ReDim Me.FisForced(Me.nGroups)

        ReDim Me.relQ(Me.nGear, Me.nGroups)

        ReDim Me.SSGroup(Me.nGroups)

        ReDim Me.TLSim(Me.nGroups)

        ReDim Me.GroupDetritus(Me.nGroups)

        ReDim Me.Epower(Me.nGear)
        ReDim Me.PcapBase(Me.nGear)
        ReDim Me.CapDepreciate(Me.nGear)
        ReDim Me.CapBaseGrowth(Me.nGear)

        ReDim Me.PropLandedTime(Me.nGear, Me.nGroups)
        ReDim Me.Propdiscardtime(Me.nGear, Me.nGroups)

        ReDim Me.PropDiscardMortTime(Me.nGear, Me.nGroups)

        ReDim Me.EffortConversionFactor(Me.nGear)

        ReDim Me.moTot(Me.nGroups)


        ' JS 3May16: make sure there is no overhang from past scenarios
        'Me.lstEnviroInputData.Clear()

    End Sub

    Public Sub RedimOutputsByTime(nTimesteps As Integer)
        ReDim Me.FIB(nTimesteps)
        ReDim Me.TLC(nTimesteps)     'TL of catch in Ecosim
        ReDim Me.Kemptons(nTimesteps)
        ReDim Me.ShannonDiversity(nTimesteps)
        ReDim Me.CatchSim(nTimesteps)

    End Sub

    ''' <summary>
    ''' Set the FisForced() array to False of all groups
    ''' </summary>
    ''' <remarks>This is called before loading forcing data (DoDatValCalulations())to clear out the old flags. 
    '''  EwE5 never clears this flag once set to True when forcing data is loaded this stays set and FishRateNo() is reset via a the interface, strange?</remarks>
    Public Sub clearFishForced()
        For igrp As Integer = 1 To Me.nGroups
            Me.FisForced(igrp) = False
        Next
    End Sub


    Public Sub Clear()
        Me.nGroups = 0
        Me.nGear = 0

        Me.eraseResults()

        'NTimes is the number of time step for the current number of years
        Me.FishRateNo = Nothing ' (nGroups, nTimeSteps))  'was 1200
        Me.FishRateGear = Nothing '  (nGear + 1, nTimeSteps))  'was 1200

        Me.FIB = Nothing ' (nTimesteps)
        Me.TLC = Nothing ' (nTimesteps)     'TL of catch in Ecosim
        Me.Kemptons = Nothing ' (nTimesteps)
        Me.ShannonDiversity = Nothing
        Me.CatchSim = Nothing ' (nTimesteps)

        Me.GroupDBID = Nothing ' (nGroups)

        'me.BaseTimeSwitch = nothing ' (nGroups)
        Me.SwitchPower = Nothing ' (nGroups)

        Me.Emig = Nothing ' (nGroups)
        Me.QmQo = Nothing ' (nGroups), Htime = nothing ' (nGroups) ', Hden = nothing ' (nGroups)
        Me.CmCo = Nothing ' (nGroups)

        Me.Qmain = Nothing ' (nGroups)
        Me.Qrisk = Nothing ' (nGroups)
        Me.RiskTime = Nothing ' (nGroups)
        Me.Consumption = Nothing ' (nGroups, nGroups)

        Me.Eatenby = Nothing ' (nGroups)
        Me.Eatenof = Nothing ' (nGroups)
        Me.EggProdShape = Nothing ' (CInt = nothing ' (nGroups / 2))
        Me.FtimeAdjust = Nothing ' (nGroups)
        Me.MoPred = Nothing ' (nGroups)

        Me.iadult = Nothing ' (CInt = nothing ' (nGroups / 2))
        Me.ijuv = Nothing ' (CInt = nothing ' (nGroups / 2))

        Me.TimeJuv = Nothing ' (CInt = nothing ' (nGroups / 2)) 'Time spent in juv stage
        Me.maxtimejuv = Nothing ' (CInt = nothing ' (nGroups / 2))
        Me.mintimejuv = Nothing ' (CInt = nothing ' (nGroups / 2))
        Me.NoIntegrate = Nothing ' (nGroups)
        Me.pbm = Nothing ' (nGroups)
        Me.PBmaxs = Nothing ' (nGroups)
        Me.pred = Nothing ' (nGroups)
        Me.RecPower = Nothing ' (CInt = nothing ' (nGroups / 2))

        Me.ilink = Nothing ' (nGroups * nGroups)
        Me.jlink = Nothing ' (nGroups * nGroups)
        Me.SimDC = Nothing ' (nGroups, nGroups)
        Me.MPred = Nothing ' (nGroups * nGroups)

        Me.vulrate = Nothing ' (nGroups, nGroups)
        Me.VulMult = Nothing ' (nGroups, nGroups)
        Me.VulnerabilityPredator = Nothing ' (nGroups)

        Me.Fish1 = Nothing ' (nGroups)
        Me.FishRateNoDBID = Nothing ' (nGroups)
        Me.FishRateNoTitle = Nothing ' (nGroups)
        Me.GroupFishRateNoDBID = Nothing ' (nGroups)

        'the plus one is for combined fleets
        Me.FleetDBID = Nothing ' (nGear + 1)
        Me.FishRateGearDBID = Nothing ' (nGear + 1)
        Me.FishRateGearBasis = Nothing ' (nGear + 1)
        Me.FishRateGearTitle = Nothing ' (nGear + 1)
        Me.FishMGear = Nothing ' (nGear + 1, nGroups)

        Me.FishRateMax = Nothing ' (nGroups)

        Me.FisForced = Nothing ' (nGroups)

        Me.relQ = Nothing ' (nGear, nGroups)

        Me.SSGroup = Nothing ' (nGroups)

        Me.TLSim = Nothing ' (nGroups)

        Me.GroupDetritus = Nothing ' (nGroups)

        Me.Epower = Nothing ' (nGear)
        Me.PcapBase = Nothing ' (nGear)
        Me.CapDepreciate = Nothing ' (nGear)
        Me.CapBaseGrowth = Nothing ' (nGear)

        Me.PropLandedTime = Nothing ' (nGear, nGroups)
        Me.Propdiscardtime = Nothing ' (nGear, nGroups)
        Me.PropDiscardMortTime = Nothing
        Me.Consumption = Nothing ' (nGroups, nGroups)
        Me.Consumpt = Nothing ' (nGroups, nGroups)
        Me.Eatenby = Nothing ' (nGroups)
        Me.Eatenof = Nothing ' (nGroups)
        Me.pred = Nothing ' (nGroups)
        Me.simDCAtT = Nothing ' (nGroups, nGroups)
        Me.DCPct = Nothing ' (nGroups, 3) 'used for B1Round, B2Round, QB, derivt  = nothing ' (BA)
        Me.zscale = Nothing
        Me.PeatArena = Nothing

        'Me.lstEnviroInputData.Clear()

    End Sub

    ''' <summary>
    ''' Initialize the forcing shapes to a value of one. This will overwrite  an existing values
    ''' </summary>
    ''' <remarks>In EwE5 this is called RedimZMax(). It gets called before the shapes are populated. </remarks>
    Public Function InitForcingShapes() As Boolean
        Dim i As Integer
        Dim j As Integer

        'I have altered this to just populate the arrays with some default values
        'the dimensioning happens in redimForcingShapes()
        'this separates the dimensioning from setting of default values
        'so that Forcing functions with valid values can be added from an interface and not get over written
        Try

            Me.ZmaxScale = 2

            'this will over write any values already in the shape arrays
            'so after this they must be repopulated
            For i = 0 To Me.NumForcingShapes

                Me.tval(i) = 1      'For forcing functions
                Me.ForcingDBIDs(i) = cCore.NULL_VALUE 'default un-initialized database ID

                For j = 0 To Me.ForcePoints
                    'this will make it so that a forcing function that has not had any values set will have no effect on the model
                    Me.zscale(j, i) = 1   'Default value is half the max
                Next

            Next

            Return True

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".InitForcingShapes() Error: " & ex.Message)
            cLog.Write(Me.ToString & ".InitForcingShapes() Error: " & ex.Message)
            Return False
        End Try

    End Function


    ''' <summary>
    ''' Resize the Forcing Shape Data to the new size this can be bigger or smaller then the existing number of elements
    ''' </summary>
    ''' <param name="newNumberOfShapes">The new size of the arrays</param>
    ''' <param name="newEcoSimIndex">optional Index of the last array element this is used for an AddShape functionality</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ResizeForcingShapes(newNumberOfShapes As Integer, Optional ByRef newEcoSimIndex As Integer = cCore.NULL_VALUE) As Boolean

        'this is still call by cEIIDataSource which is not used!!!! Hack hack hhhhhhhaaaa
        Debug.Assert(False, "ResizeForcingShapes() no longer implemented!")

        'Try

        '    'set the new number of shapes this was decided by the database and passed in for robustness
        '    'this way the number of shapes is controlled by the datasource
        '    ForcingShapes = newNumberOfShapes
        '    redimForcingShapes()
        '    InitForcingShapes()
        '    newEcoSimIndex = ForcingShapes
        '    Return True

        'Catch ex As Exception
        '    'ToDo_jb  cEcoSimDataStructures.AddShape() Error message
        '    Debug.Assert(False, Me.ToString & ".AddForcingShape() Error: " & ex.Message)
        '    Return False
        'End Try


    End Function

    ''' <summary>
    ''' Resize the Forcing Shape Data to the new size this can be bigger or smaller then the existing number of elements
    ''' </summary>
    ''' <param name="newNumberOfShapes">The new size of the arrays</param>
    ''' <param name="newEcoSimIndex">optional Index of the last array element this is used for an AddShape functionality</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ResizeMediationShapes(newNumberOfShapes As Integer, Optional ByRef newEcoSimIndex As Integer = cCore.NULL_VALUE) As Boolean

        Try

            'set the new number of shapes this was decided by the database and passed in for robustness
            'this way the number of shapes is controlled by the datasource
            Me.BioMedData.MediationShapes = newNumberOfShapes
            Me.BioMedData.ReDimMediation(Me.nGroups, Me.nGear)
            Return True

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".ResizeMediationShapes() Error: " & ex.Message)
            Return False
        End Try


    End Function



    ''' <summary>
    ''' Dimension all forcing function variables by ForcingPoints (number of forcing points/simulation years) and or ForcingShapes (number of forcing shapes)
    ''' </summary>
    ''' <remarks>Call this any time the number of ForcingPoints(this is the number of simulation years * 12 months) or ForcingShapes changes.
    ''' This gets called to added or remove a forcing function from the EcoSim data. The data will have to be repopulated after this has been run. 
    ''' Core.CoreForcingFunctionUpdater() will update all EcoSim Forcing and Mediation function data with the data held currently in memory by the Shape Managers. 
    ''' </remarks>
    Public Function DimForcingShapes() As Boolean

        Try
            Debug.Assert(Me.NumYears > 0, Me.ToString & ".redimForcingShapes() TotalTime must be set to redim Forcing Shapes.")
            ReDim Me.EnvRespFuncIndex(Me.NumEnvResponseFunctions, Me.nGroups)
            ReDim Me.MortalityRespFuncIndex(Me.NumEnvResponseFunctions, Me.nGroups)

            ReDim Me.zscale(Me.ForcePoints, Me.NumForcingShapes)
            ReDim Me.tval(Me.NumForcingShapes)
            ReDim Me.ForcingTitles(Me.NumForcingShapes)

            'variable added for EwE6
            ReDim Me.ForcingShapeType(Me.NumForcingShapes) 'Time or Egg Prod
            ReDim Me.ForcingShapeParams(Me.NumForcingShapes)
            ReDim Me.ForcingDBIDs(Me.NumForcingShapes)

            ReDim Me.isSeasonal(Me.NumForcingShapes)

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".redimForcingShapes() Error: " & ex.Message)
            cLog.Write(Me.ToString & ".redimForcingShapes() Error: " & ex.Message)
            Return False
        End Try

        Return True

    End Function

    ''' <summary>
    ''' Resize and set time data to the new number of years set by a user
    ''' </summary>
    ''' <remarks></remarks>
    Friend Sub redimTime(newNumberOfYears As Integer, nRefDataYears As Integer, bCopyLastShapePoint As Boolean)
        Dim ipt As Integer, ishape As Integer
        Dim orgNYears As Integer, orgNTimes As Integer

        'get the original number of years and time steps
        orgNYears = Me.NumYears
        orgNTimes = Me.NTimes

        'set number of years to the new value this will also set NTimes (number of time steps)
        Me.NumYears = newNumberOfYears

        'Fishing rate shapes need to be big enough to hold reference data see DoDataValCalculations()
        Dim ntimesteps As Integer = Me.NTimes
        If Me.NumYears < nRefDataYears Then
            ntimesteps = nRefDataYears * Me.NumStepsPerYear
        End If

        'redim preserve the fishing rate and fishmortality data
        Me.redimFishingRates(ntimesteps)

        'set the summary periods to the first and last year
        Me.DefaultSummaryPeriods()

        Me.RedimOutputsByTime(ntimesteps)

        'Only resize the forcing data if the new run length is greater then the existing run length
        'on the database load ForcePoints was set to a min of DEFAULT_N_FORCINGPOINTS (100 years, 1200 points) or the number of years in database * 12 see DimForcingShapes
        'this preserves the originally loaded forcing data if the new number of years is less the data that is already loaded
        If Me.NTimes > Me.ForcePoints Then

            'this means ForcePoints is >=  DEFAULT_N_FORCINGPOINTS and can only grow
            Dim orgPts As Single = Me.ForcePoints
            Me.ForcePoints = Me.NTimes

            'Can't Redim Preserve the first dimension
            'so we need to copy the values back into the new zscale()
            Dim orgZscale(,) As Single
            ReDim orgZscale(orgPts, Me.NumForcingShapes)
            Array.Copy(Me.zscale, orgZscale, orgZscale.Length)

            ReDim Me.zscale(Me.ForcePoints, Me.NumForcingShapes)
            ReDim Me.tval(Me.NumForcingShapes)

            For ishape = 0 To Me.NumForcingShapes
                Me.tval(0) = 1      'For forcing functions
                Me.ZmaxScale = 2
                'copy the values from the original zscale() into the new zscale()
                Dim sLast As Single = 1.0
                For ipt = 0 To orgPts
                    sLast = orgZscale(ipt, ishape)
                    Me.zscale(ipt, ishape) = sLast
                Next
                ' populate extra time (fixes #1427, #1557)
                If Me.isSeasonal(ishape) Then
                    For ipt = orgPts + 1 To Me.ForcePoints
                        Me.zscale(ipt, ishape) = orgZscale(1 + ((ipt - 1) Mod cCore.N_MONTHS), ishape)
                    Next
                Else
                    For ipt = orgPts + 1 To Me.ForcePoints
                        Me.zscale(ipt, ishape) = sLast
                    Next
                End If
            Next

        End If

        'copy the last point from the original data to the end of the new data
        If bCopyLastShapePoint Then
            If Me.NumYears > orgNYears Then

                'for the fishing rate and fish mort data copy the last point into the new points
                For igrp As Integer = 1 To Me.nGroups
                    For ipt = orgNTimes + 1 To Me.NTimes
                        Me.FishRateNo(igrp, ipt) = Me.FishRateNo(igrp, orgNTimes)
                    Next
                Next

                For iflt As Integer = 1 To Me.nGear
                    For ipt = orgNTimes + 1 To Me.NTimes
                        Me.FishRateGear(iflt, ipt) = Me.FishRateGear(iflt, orgNTimes)
                    Next
                Next

            End If
        End If

    End Sub


    '''' <summary>
    '''' Redimension all variables by NMedPoints and/or MediationShapes
    '''' </summary>
    '''' <remarks>Call this any time the number of MediationShapes has changed. This will clear out any data that was in memory.
    ''''  Core.CoreForcingFunctionUpdater() will update all EcoSim Forcing and Mediation function data with the data held currently in memory by the Shape Managers.</remarks>
    'Public Sub ReDimMediation()
    '    Dim i, j As Integer
    '    'following is for Mediation:
    '    NMedPoints = 1200
    '    ' JS18apr09: spawning 9 dummy mediation shapes without any valid database IDS screws up the database
    '    '            I tested Ecosim without mediation shapes and both core and GUI behave well
    '    'If MediationShapes <= 0 Then MediationShapes = 9
    '    ReDim Medpoints(NMedPoints, MediationShapes)
    '    ReDim MedWeights(nGroups + nGear, MediationShapes)
    '    ReDim NMedXused(MediationShapes)
    '    ReDim IMedUsed(nGroups + nGear, MediationShapes)
    '    ReDim MedXbase(MediationShapes)
    '    ReDim MedYbase(MediationShapes)
    '    ReDim MedIsUsed(MediationShapes)
    '    ReDim MedVal(MediationShapes)
    '    ReDim IMedBase(MediationShapes)

    '    'jb added
    '    ReDim MediationTitles(MediationShapes)
    '    ReDim MediationShapeParams(MediationShapes)
    '    ReDim MediationDBIDs(MediationShapes)

    '    'jb this is now handled by MedShapeParams() above
    '    'If ForcingShapes > MediationShapes Then
    '    '    ReDim Preserve Shapes(5, ForcingShapes)
    '    'Else
    '    '    ReDim Preserve Shapes(5, MediationShapes)
    '    'End If

    '    'ToDo: Sort out XBaseLine()what is it used for
    '    'ReDim XBaseLine(MediationShapes)
    '    For i = 0 To MediationShapes
    '        IMedBase(i) = NMedPoints \ 3
    '        For j = 0 To NMedPoints
    '            Medpoints(j, i) = 0.5
    '        Next
    '    Next

    'End Sub




    ''' <summary>
    ''' Hardwire some default values
    ''' </summary>
    ''' <remarks>
    ''' In the original code this was called "EcoSimFileOpen()"
    ''' </remarks>
    Friend Sub SetDefaultParameters()

        'jb
        'in the original code SetupParametersDefault1() was called before reading the ini file default values
        'that doesn't make sense to me as SetupParametersDefault1() uses values that are set from the ini read 
        'so I have switched it to SetupParametersDefault1 after the defaults are read (ini)
        'SetupParametersDefault1()


        'read ini file stored defaults here 
        'at this time there is no mechanisim for storing defaults 
        'so I have just hardwired the same values as are in the default ini file 
        'see original code "SetupParametersRead()"

        'the commented out variables where in the original code but are not declared at this time
        Me.VulMultAll = 0.3
        'StepsPerYear = 12
        Me.TimeJuv(0) = 1
        Me.mintimejuv(0) = 1
        Me.maxtimejuv(0) = 1.0001
        Me.RecPower(0) = 1
        Me.FtimeMax(0) = 2
        Me.FtimeAdjust(0) = 0.5
        Me.MoPred(0) = 0
        'Next other parameters
        Me.Discount = 5
        Me.NumYears = 20
        Me.StepSize = 100
        Me.SystemRecovery = 1
        Me.SorWt = 0.5
        Me.EquilibriumStepSize = 0.003
        Me.StepsPerMonth = 1
        'StepsPerMonth = 30
        'MsgBox("Warning daily time step.")

        'Hack warning temp hard wire of summary time periods
        Me.SumStart(0) = 0
        Me.SumStart(1) = Me.NumYears - 1
        Me.NumStep = Me.NumStepsPerYear

        'DoIntegrate=1 in the ini file 
        Me.Integrate = True

        Me.VulMultAll = 2

        Dim i As Integer

        For i = 1 To Me.nGroups     'prey
            Me.QmQo(i) = 1
            Me.CmCo(i) = 1000
            Me.SwitchPower(i) = 0
            Me.PBmaxs(i) = 2
            'jb price(nGroups) is not used anywhere
            ' price(i) = 1

            Me.NoIntegrate(i) = i
            Me.FtimeMax(i) = Me.FtimeMax(0)
            Me.FtimeAdjust(i) = Me.FtimeAdjust(0)
            Me.MoPred(i) = Me.MoPred(0)
            Me.RiskTime(i) = 0

            Me.PaddP(i) = 1

        Next

        For iflt As Integer = 1 To Me.nGear
            Me.EffortConversionFactor(iflt) = 1
        Next

        'Next from CJW's TemporaryRead
        If Me.FtimeMax(0) <= 0 Then Me.FtimeMax(0) = 2
        If Me.FtimeAdjust(0) < 0 Then Me.FtimeAdjust(0) = 0.5
        If Me.MoPred(0) <= 0 Then Me.MoPred(0) = 1


    End Sub

    Public Sub setRelQToT(iTimestep As Integer, Optional bUseNullValues As Integer = True)
        ' Debug.Assert(iTimestep <> 25)

        If iTimestep > Me.NTimes Then
            'The fishing Policy Search runs Ecosim for an extra 20 years
            'in this case driving data i.e. Fishing Effort and RelQ 
            'are set to the last year of the simulation
            iTimestep = Me.NTimes

        End If

        For iflt As Integer = 1 To Me.nGear
            For igrp As Integer = 1 To Me.nGroups
                If bUseNullValues Then
                    Me.relQ(iflt, igrp) = Me.relQt(iflt, igrp, iTimestep)
                Else
                    If Me.relQt(iflt, igrp, iTimestep) <> cCore.NULL_VALUE Then
                        Me.relQ(iflt, igrp) = Me.relQt(iflt, igrp, iTimestep)
                    Else
                        Me.relQ(iflt, igrp) = 0
                    End If

                End If
            Next
        Next

    End Sub


    ''' <summary>
    ''' Set the summary time periods to using the Ecoism run length (NTime)
    ''' </summary>
    Public Sub DefaultSummaryPeriods()
        Try
            Debug.Assert(Me.NumYears <> 0 And Me.NumStep <> 0 And Me.NumStepsPerYear <> 0, "DefaultSummaryPeriods() could not be set!")
            Me.SumStart(0) = 0
            Me.SumStart(1) = Me.NumYears - Me.NumStep / Me.NumStepsPerYear
        Catch ex As Exception
            cLog.Write(ex)
            'the model can still run if the summary time periods are messed up
        End Try

    End Sub


    Public Sub RedimTime()
        'Dim MaxTime As Integer

        Debug.Assert(Me.NumYears <> 0, Me.ToString & ".RedimTotalTimeVariables() TotalTime = 0 Something is very wrong......")
        ReDim Me.FishRateNo(Me.nGroups, Me.NTimes)  'was 1200
        ReDim Me.FishRateGear(Me.nGear + 1, Me.NTimes)  'was 1200

        Me.RedimOutputsByTime(Me.NTimes)

        'reset some default values before the data is populated by an interface or a datasource
        'this worked differently in EwE5
        Me.DefaultFishMortalityRates()
        Me.DefaultFishingRates()

        ' DefaultCatchabilities()

    End Sub


    ''' <summary>
    ''' Redim preserve the Fishing Rate and Fish Mort arrays to the number of time steps the model will run for.
    ''' </summary>
    Private Sub redimFishingRates(nTimeSteps As Integer)

        'NTimes is the number of time step for the current number of years
        ReDim Preserve Me.FishRateNo(Me.nGroups, nTimeSteps)  'was 1200
        ReDim Preserve Me.FishRateGear(Me.nGear + 1, nTimeSteps)  'was 1200

    End Sub

    ''' <summary>
    ''' Dimension the results over time arrays i.e. ResultsOverTime(),ResultsSumCatchByGroupGear()
    ''' </summary>
    ''' <remarks>This only gets called if/when the model is actually run <see>cEcoSimModel.RunModelValue</see>
    ''' This reduces the memory needs of ecosim so that it can be initialized but not run. 
    ''' Ecosim is initialized but not run when Ecospace is loaded.
    ''' This would also allow for a flag to turn of the saving of results over time.
    ''' </remarks>
    Public Sub dimResults(NumberOfYears As Integer)

        'reset the number of time steps in the summary data
        Me.nSumTimeSteps = 0

        Dim nt As Integer = NumberOfYears * Me.NumStepsPerYear

        'jb 15-Nov-2010 force garbage collection on large blocks of memory
        Erase Me.ResultsOverTime
        Erase Me.PredPreyResultsOverTime
        Erase Me.ResultsAvgByPreyPred
        Erase Me.ResultsSumCatchByGroupGear
        Erase Me.ResultsSumFMortByGroupGear
        Erase Me.ResultsSumValueByGroupGear
        Erase Me.ResultsTimeLandingsGroupGear
        Erase Me.ResultsEffort
        Erase Me.Elect

        Erase Me.ResultsSumRelValueByGroup
        Erase Me.ResultsTimeDiscardsGroupGear
        Erase Me.ResultsTimeDiscardsMortGroupGear
        Erase Me.ResultsTimeDiscardsSurvivedGroupGear

        GC.Collect()

        ReDim Me.ResultsOverTime([Enum].GetValues(GetType(eEcosimResults)).Length - 1, Me.nGroups, nt)
        ReDim Me.PredPreyResultsOverTime(2, Me.nGroups, Me.nGroups, nt)
        ReDim Me.ResultsAvgByPreyPred(1, Me.nGroups, Me.nGroups)


        'fisheries data
        ReDim Me.ResultsSumCatchByGroupGear(Me.nGroups, Me.nGear, nt) ' groups,fleets,time
        ReDim Me.ResultsSumFMortByGroupGear(Me.nGroups, Me.nGear, nt)

        ReDim Me.ResultsTimeLandingsGroupGear(Me.nGroups, Me.nGear, nt)
        ReDim Me.ResultsTimeDiscardsGroupGear(Me.nGroups, Me.nGear, nt)
        ReDim Me.ResultsTimeDiscardsMortGroupGear(Me.nGroups, Me.nGear, nt)
        ReDim Me.ResultsTimeDiscardsSurvivedGroupGear(Me.nGroups, Me.nGear, nt)

        ReDim Me.ResultsSumCatchByGear(Me.nGear, nt)
        ReDim Me.ResultsSumValueByGroupGear(Me.nGroups, Me.nGear, nt)
        ReDim Me.ResultsSumValueByGear(Me.nGear, nt)
        ReDim Me.ResultsEffort(Me.nGear, nt)
        ReDim Me.Elect(Me.nGroups, Me.nGroups, nt)

        ReDim Me.ProfitByFleet(Me.nGear)
        ReDim Me.EmploymentValueByFleet(Me.nGear)

        ReDim Me.ResultsLandings(Me.nGroups, Me.nGear)
        ReDim Me.ResultsSumRelValueByGroup(Me.nGroups, nt)

        ReDim Me.ResultsDiscardsMort(Me.nGroups, Me.nGear)
        ReDim Me.ResultsDiscardsSurvived(Me.nGroups, Me.nGear)

    End Sub


    ''' <summary>
    ''' Erase all the results arrays 
    ''' </summary>
    Public Sub eraseResults()

        Erase Me.ResultsOverTime
        Erase Me.PredPreyResultsOverTime
        Erase Me.ResultsAvgByPreyPred

        'fisheries data
        Erase Me.ResultsSumCatchByGroupGear ' groups,fleets,time
        Erase Me.ResultsSumCatchByGear
        Erase Me.ResultsSumValueByGroupGear
        Erase Me.ResultsSumValueByGear
        Erase Me.ResultsEffort
        Erase Me.Elect
        Erase Me.ResultsSumFMortByGroupGear

    End Sub


    ''' <summary>
    ''' Number of time steps to run the model for
    ''' </summary>
    ''' <remarks>[number of years]*[number of time steps per year]</remarks>
    Public ReadOnly Property NTimes() As Integer
        Get
            Return Me.NumYears * Me.NumStepsPerYear
        End Get
    End Property


    ''' <summary>
    ''' Set default fish rate values
    ''' </summary>
    ''' <remarks>
    ''' <para>This gets called after the data has been dimensioned and before it is populated by the database</para>
    ''' <para>The interface may call this as well to reset fish rate values.</para>
    ''' </remarks>
    Public Sub DefaultFishingRates()

        Dim i As Integer
        Dim j As Integer

        For i = 1 To Me.nGear + 1
            Me.FishRateGearBasis(i) = 1
            Me.FishRateGear(i, 0) = 1

            For j = 0 To Me.NTimes
                Me.FishRateGear(i, j) = 1
            Next
        Next

    End Sub

    'Public Sub DefaultCatchabilities()
    '    Dim i As Integer
    '    Dim j As Integer
    '    'set relative catchabilities by gear type, treating effort for each gear as starting at base
    '    'value of 1.0 so that F for the gear (F=qE=C/B) is 1.0xq where q is relative catchability
    '    'this avoids measuring effort in some unnecessary data units

    '    For i = 1 To nGear
    '        For j = 1 To nGroups
    '            'total catch rate 
    '            'Includes discards that survive
    '            'relQ(i, j) = (m_EPData.Landing(i, j) + m_EPData.Discard(i, j)) / m_Data.StartBiomass(j)

    '            For it As Integer = 1 To NTimes
    '                relQt(i, j, it) = (PropLandedTime(i, j) + Propdiscardtime(i, j)) / StartBiomass(j)
    '            Next

    '        Next
    '    Next

    'End Sub




    ''' <summary>
    ''' Set effort to default value for all the fleets in list
    ''' </summary>
    ''' <param name="lstFleetsIndexesToSet">List for fleets to set to default</param>
    ''' <remarks>Call when an effort timeseries has been unloaded to reset effort to default values</remarks>
    Sub setEffortToDefault(lstFleetsIndexesToSet As List(Of Integer))
        Try
            'reset effort to 1 for all fleets that where disabled
            For Each flt As Integer In lstFleetsIndexesToSet
                For it As Integer = 1 To Me.NTimes
                    Me.FishRateGear(flt, it) = 1
                Next
            Next
        Catch ex As Exception
            System.Console.WriteLine(Me.ToString & ".setEffortToDefault() Exception: " & ex.Message)
            cLog.Write(ex)
        End Try
    End Sub

    ''' <summary>
    ''' Set default fish mortality values
    ''' </summary>
    ''' <remarks>
    ''' <para>This gets called after the data has been dimensioned and before it is populated by the database</para>
    ''' <para>The interface may call this as well to reset fish rate values.</para>
    ''' </remarks>
    Public Sub DefaultFishMortalityRates()

        Dim i As Integer
        Dim j As Integer

        'xxxxxxxxxxxxxxxxxxxxxxxxxxxx
        'ToDo JB Forced F'
        'Check if this is correct once we changed to F forced for partial run 
        'and 
        'relQ() has a time component
        'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

        For i = 1 To Me.nGroups
            For j = 1 To Me.NTimes
                'set FishRateNo(group,time) to CatchRate
                'for the full run time
                Me.FishRateNo(i, j) = Me.Fish1(i)
            Next
        Next

    End Sub

    ''' <summary>
    ''' Deep-copy Ecosim data structures to another instance.
    ''' </summary>
    ''' <param name="d">The instance to copy to.</param>
    Public Sub CopyTo(ByRef d As cEcosimDatastructures)
        Try

            d.GroupDBID = Me.GroupDBID.Clone
            d.nGroups = Me.nGroups
            d.nGear = Me.nGear

            'now we can redim
            d.RedimVars()

            d.FirstTime = Me.FirstTime
            '    d.ConSimOn = ConSimOn
            d.TrophicOff = Me.TrophicOff
            d.IndicesOn = Me.IndicesOn
            d.UseVarPQ = Me.UseVarPQ
            d.NudgeChecked = Me.NudgeChecked
            d.Integrate = Me.Integrate
            d.AbortRun = Me.AbortRun
            d.EvolveIsOn = Me.EvolveIsOn
            d.BiomassOn = Me.BiomassOn
            d.ActivePair = Me.ActivePair
            'd.CperFlag = CperFlag
            d.NumYears = Me.NumYears
            d.NumStepsPerYear = Me.NumStepsPerYear
            d.StepSize = Me.StepSize
            d.SorWt = Me.SorWt
            d.Discount = Me.Discount
            d.EquilibriumStepSize = Me.EquilibriumStepSize
            d.EquilScaleMax = Me.EquilScaleMax
            d.NutBaseFreeProp = Me.NutBaseFreeProp
            d.NutMin = Me.NutMin
            d.NutBiom = Me.NutBiom
            d.NutTot = Me.NutTot
            d.NutFree = Me.NutFree
            d.NutFreeBase = Me.NutFreeBase.Clone
            d.VulMultAll = Me.VulMultAll
            d.VulMult = Me.VulMult.Clone
            d.vulrate = Me.vulrate.Clone
            d.maxflow = Me.maxflow.Clone
            d.FlowType = Me.FlowType.Clone
            'd.PoolForceCatch = PoolForceCatch.Clone
            d.Eatenof = Me.Eatenof.Clone
            d.Eatenby = Me.Eatenby.Clone
            d.NutForceNumber = Me.NutForceNumber
            d.NutPBmax = Me.NutPBmax
            d.SystemRecovery = Me.SystemRecovery
            d.FtimeMax = Me.FtimeMax.Clone
            d.FtimeAdjust = Me.FtimeAdjust.Clone
            d.MoPred = Me.MoPred.Clone
            d.mo = Me.mo.Clone
            d.RiskTime = Me.RiskTime.Clone
            d.QmQo = Me.QmQo.Clone
            d.CmCo = Me.CmCo.Clone
            d.SwitchPower = Me.SwitchPower.Clone
            d.BaseTimeSwitch = Me.BaseTimeSwitch.Clone
            d.PBmaxs = Me.PBmaxs.Clone
            d.RecPower = Me.RecPower.Clone
            d.Emig = Me.Emig.Clone
            d.Consumption = Me.Consumption.Clone
            d.Htime = Me.Htime.Clone
            d.SimDC = Me.SimDC.Clone
            d.iadult = Me.iadult.Clone
            d.ijuv = Me.ijuv.Clone
            d.TimeJuv = Me.TimeJuv.Clone
            d.maxtimejuv = Me.maxtimejuv.Clone
            d.mintimejuv = Me.mintimejuv.Clone
            d.NoIntegrate = Me.NoIntegrate.Clone
            d.Fish1 = Me.Fish1.Clone
            d.FishTime = Me.FishTime.Clone
            d.FishRateMax = Me.FishRateMax.Clone
            d.FishMGear = Me.FishMGear.Clone
            d.FishRateNo = Me.FishRateNo.Clone
            d.FishRateNoDBID = Me.FishRateNoDBID.Clone
            d.FishRateNoTitle = Me.FishRateNoTitle.Clone
            d.GroupFishRateNoDBID = Me.GroupFishRateNoDBID.Clone
            d.FishRateGear = Me.FishRateGear.Clone
            d.FishRateGearBasis = Me.FishRateGearBasis.Clone
            d.FishRateGearDBID = Me.FishRateGearDBID.Clone
            d.FishRateGearTitle = Me.FishRateGearTitle.Clone
            d.Ftime = Me.Ftime.Clone
            d.Hden = Me.Hden.Clone
            d.QBoutside = Me.QBoutside.Clone
            d.DetritusOut = Me.DetritusOut.Clone
            d.AssimEff = Me.AssimEff.Clone
            d.SimGE = Me.SimGE.Clone
            d.StartBiomass = Me.StartBiomass.Clone
            d.pbbiomass = Me.pbbiomass.Clone
            d.loss = Me.loss.Clone
            d.Cbase = Me.Cbase.Clone
            d.relQ = Me.relQ.Clone

            d.ForcingShapeType = Me.ForcingShapeType.Clone
            'ShapeParameters = ShapeParameters.clone

            '    d.MediationShapeParams = MediationShapeParams.Clone
            d.ForcingShapeParams = Me.ForcingShapeParams.Clone
            '   d.MediationTitles = MediationTitles.Clone
            '   d.MediationDBIDs = MediationDBIDs.Clone
            d.ForcingDBIDs = Me.ForcingDBIDs.Clone
            d.isSeasonal = Me.isSeasonal.Clone

            ''Mediation vars 
            'd.MediationShapes = MediationShapes
            'd.NMedPoints = NMedPoints
            'd.Medpoints = Medpoints.Clone
            'd.MedWeights = MedWeights.Clone
            'd.NMedXused = NMedXused.Clone
            'd.IMedUsed = IMedUsed.Clone
            'd.MedXbase = MedXbase.Clone
            'd.MedYbase = MedYbase.Clone
            'd.MedIsUsed = MedIsUsed.Clone  '
            'd.MedVal = MedVal.Clone
            'd.IMedBase = IMedBase.Clone
            d.inlinks = Me.inlinks
            d.ilink = Me.ilink.Clone
            d.jlink = Me.jlink.Clone

            'Forcing
            d.NumForcingShapes = Me.NumForcingShapes
            d.ForcingTitles = Me.ForcingTitles.Clone
            d.ForcePoints = Me.ForcePoints
            d.ZmaxScale = Me.ZmaxScale
            d.zscale = Me.zscale.Clone
            d.tval = Me.tval.Clone
            d.EggProdShape = Me.EggProdShape.Clone
            d.pbm = Me.pbm.Clone
            d.pred = Me.pred.Clone
            d.Qmain = Me.Qmain.Clone
            d.Qrisk = Me.Qrisk.Clone
            d.Consumpt = Me.Consumpt.Clone
            'd.DCPct = DCPct.Clone
            d.ResultsOverTime = Me.ResultsOverTime.Clone
            d.PredPreyResultsOverTime = Me.PredPreyResultsOverTime.Clone
            d.ResultsAvgByPreyPred = Me.ResultsAvgByPreyPred.Clone
            d.NumStep = Me.NumStep
            d.NumStep0 = Me.NumStep0
            d.NumStep1 = Me.NumStep1
            d.SumStart = Me.SumStart.Clone
            d.Narena = Me.Narena
            d.Iarena = Me.Iarena.Clone
            d.Jarena = Me.Jarena.Clone
            d.ArenaNo = Me.ArenaNo.Clone
            d.VulArena = Me.VulArena.Clone
            d.Alink = Me.Alink.Clone
            d.IlinkSet = Me.IlinkSet.Clone
            d.JlinkSet = Me.JlinkSet.Clone
            d.KlinkSet = Me.KlinkSet.Clone
            d.PeatArena = Me.PeatArena.Clone
            d.ArenaLink = Me.ArenaLink.Clone
            d.Qlink = Me.Qlink.Clone
            d.NlinksSet = Me.NlinksSet
            d.BoutFeeding = Me.BoutFeeding
            d.RelaSwitch = Me.RelaSwitch.Clone
            d.ToDetritus = Me.ToDetritus.Clone
            d.FisForced = Me.FisForced.Clone
            d.SS = Me.SS

            d.Epower = Me.Epower
            d.PcapBase = Me.PcapBase
            d.CapDepreciate = Me.CapDepreciate
            d.CapBaseGrowth = Me.CapBaseGrowth

        Catch ex As Exception
            Debug.Assert(False, ex.Message)
        End Try
    End Sub


    ''' <summary>
    ''' Get the time index for the Summary Start and End Time
    ''' </summary>
    ''' <param name="iSummary">0 = Start Summary Time Period. 1 = End Summary Time Period</param>
    ''' <returns>iTime Index</returns>
    ''' <remarks></remarks>
    Private Function summaryTimeIndex(iSummary As Integer) As Integer
        Dim itime As Integer
        itime = CInt(Me.SumStart(iSummary) * Me.NumStepsPerYear) + 1
        If itime > Me.NumYears * Me.NumStepsPerYear Then itime = Me.NumYears * Me.NumStepsPerYear - Me.NumStep
        Return itime
    End Function

    Public Function getSummaryBioForGroup(iGroup As Integer, ByRef startBio As Single, ByRef endBio As Single) As Single
        Dim bsum As Single, nbsum As Integer, stime As Integer, itime As Integer
        Dim bio(1) As Single

        Try

            For isum As Integer = 0 To 1
                bsum = 0
                nbsum = 0

                stime = Me.summaryTimeIndex(isum)

                For itime = stime To stime + Me.NumStep - 1
                    bsum = bsum + Me.ResultsOverTime(cEcosimDatastructures.eEcosimResults.Biomass, iGroup, itime)
                    nbsum += 1
                Next itime

                bio(isum) = bsum / nbsum

            Next isum

            startBio = bio(0)
            endBio = bio(1)

            Return True

        Catch ex As Exception
            cLog.Write(ex)

            'if there is an error startBio and endBio will be zero
            startBio = 0
            endBio = 0
            Return False
        End Try


    End Function

    Public Function getSummaryValueByGroup(iGroup As Integer, iFleet As Integer, ByRef startCatch As Single, ByRef endCatch As Single) As Boolean
        Return Me.getSummarybyGroupFleet(Me.ResultsSumValueByGroupGear, iGroup, iFleet, startCatch, endCatch)
    End Function


    Public Function getSummaryCostByCatch(iFleet As Integer, ByRef startCost As Single, ByRef endcost As Single) As Boolean
        Me.getSummaryByFleet(Me.ResultsEffort, iFleet, startCost, endcost)
    End Function


    Public Function getSummaryCatchByGroup(iGroup As Integer, iFleet As Integer, ByRef startCatch As Single, ByRef endCatch As Single) As Boolean
        Return Me.getSummarybyGroupFleet(Me.ResultsSumCatchByGroupGear, iGroup, iFleet, startCatch, endCatch)
    End Function

    Public Function getSummaryBioOfCatch(iFleet As Integer, ByRef startCatch As Single, ByRef endCatch As Single) As Boolean
        Return Me.getSummaryByFleet(Me.ResultsSumCatchByGear, iFleet, startCatch, endCatch)
    End Function

    Public Function getSummaryValueOfCatch(iFleet As Integer, ByRef startCatch As Single, ByRef endCatch As Single) As Boolean
        Return Me.getSummaryByFleet(Me.ResultsSumValueByGear, iFleet, startCatch, endCatch)
    End Function

    Private Function getSummarybyGroupFleet(ByRef values(,,) As Single, iGroup As Integer, iFleet As Integer, ByRef startVal As Single, ByRef endVal As Single) As Boolean

        Dim bsum As Single, nbsum As Integer, stime As Integer, itime As Integer
        Dim sumValues(1) As Single

        Try
            For isum As Integer = 0 To 1
                bsum = 0
                nbsum = 0

                stime = Me.summaryTimeIndex(isum)

                For itime = stime To stime + Me.NumStep - 1
                    bsum = bsum + values(iGroup, iFleet, itime)
                    nbsum += 1
                Next itime

                sumValues(isum) = bsum / nbsum

            Next isum

            startVal = sumValues(0)
            endVal = sumValues(1)

            Return True

        Catch ex As Exception
            cLog.Write(ex)

            'if there is an error startBio and endBio will be zero
            startVal = 0
            endVal = 0
            Return False
        End Try

    End Function


    Private Function getSummaryByFleet(ByRef values(,) As Single, iFleet As Integer, ByRef startVal As Single, ByRef endVal As Single) As Boolean
        Dim bsum As Single, nbsum As Integer, stime As Integer, itime As Integer
        Dim sumValues(1) As Single

        Try
            For isum As Integer = 0 To 1
                bsum = 0
                nbsum = 0

                stime = Me.summaryTimeIndex(isum)

                For itime = stime To stime + Me.NumStep - 1
                    bsum = bsum + values(iFleet, itime)
                    nbsum += 1
                Next itime

                sumValues(isum) = bsum / Me.NumStep

            Next isum

            startVal = sumValues(0)
            endVal = sumValues(1)

            Return True

        Catch ex As Exception
            cLog.Write(ex)

            'if there is an error startVal and endVal will be zero
            startVal = 0
            endVal = 0
            Return False
        End Try

    End Function


    ''' <summary>
    ''' Computed summarized results for Ecosim
    ''' </summary>
    ''' <param name="EcopathCost">Ecopath precentage of Cost CostPct(3,nfleets)</param>
    ''' <param name="JobMultiplier">Jobs multiplier from the Search data</param>
    ''' <remarks>Computes ProfitByFleet(nFleets), JobsByFleet(nfleets), Prey Pred consumption</remarks>
    Public Sub SummarizeResults(EcopathCost(,) As Single, JobMultiplier() As Single)

        For iPrey As Integer = 1 To Me.nGroups
            For iPred As Integer = 1 To Me.nGroups
                Me.ResultsAvgByPreyPred(0, iPrey, iPred) = Me.ResultsAvgByPreyPred(0, iPrey, iPred) / Me.nSumTimeSteps
                Me.ResultsAvgByPreyPred(1, iPrey, iPred) = Me.ResultsAvgByPreyPred(1, iPrey, iPred) / Me.nSumTimeSteps
            Next
        Next

        ReDim Me.ProfitByFleet(Me.nGear)
        ReDim Me.EmploymentValueByFleet(Me.nGear)

        Dim sumValue As Single, sumEffort As Single
        'number of years the data was summarized over
        Dim nYears As Single = Me.nSumTimeSteps / 12
        For iflt As Integer = 0 To Me.nGear
            sumValue = 0
            For it As Integer = 1 To Me.nSumTimeSteps
                sumValue += Me.ResultsSumValueByGear(iflt, it)
            Next

            sumEffort = 0
            For it As Integer = 1 To Me.nSumTimeSteps
                sumEffort += Me.ResultsEffort(iflt, it)
            Next

            'average profit
            '[sum of value] * [ecopath profit (percentage of catch value that is profit /per unit of effort)]
            Me.ProfitByFleet(iflt) = sumValue * (EcopathCost(iflt, eCostIndex.Profit) / 100) * sumEffort / nYears

            'TEMP just for something to work with until we have ECost up and running
            '[value of catch] * [Jobs(fleet) from the search forms]
            Me.EmploymentValueByFleet(iflt) = sumValue * JobMultiplier(iflt) / nYears 'Jobs(Fleet) percentage of value that goes to Jobs default=1

        Next iflt

    End Sub

    Public Sub ClearSummaryResults()

        Me.NumStep0 = 0     'Actual number of steps for the zero element of the summary arrays Start summary time period
        Me.NumStep1 = 0  'Actual number of steps for the one element of the summary arrays end summary time peroid
        'storage for summary time period data
        ReDim Me.SumBiomass(2, Me.nGroups) 'SumBiomass(iTimePeriod,iGroup)
        'catch by group
        ReDim Me.SumCatch(2, Me.nGroups)

    End Sub

    ''' <summary>
    ''' An Ecosim run has completed
    ''' </summary>
    ''' <remarks>Sets StepsPerMonth and bMultiThreaded to default values</remarks>
    Public Sub onEcosimRunCompleted()
        Me.StepsPerMonth = 1
        Me.bMultiThreaded = False
    End Sub

End Class


