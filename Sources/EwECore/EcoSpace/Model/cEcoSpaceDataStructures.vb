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
#Region " Imports "

Option Strict On
Imports EwEUtils.Core
Imports EwEUtils.Extensions

#End Region ' Imports

#Region "Public Class definitions"

Public Class cEcospaceDataStructures

#Region "Public Fields"

    ''' <summary>ESRI projection string of the default WGS_84 projection of Ecospace.</summary>
    Public Const DEFAULT_COORDINATESYSTEM As String = "GEOGCS[""WGS 84"", DATUM[""WGS_1984"", SPHEROID[""WGS 84"",6378137,298.257223563, AUTHORITY[""EPSG"",""7030""]], AUTHORITY[""EPSG"",""6326""]], PRIMEM[""Greenwich"",0, AUTHORITY[""EPSG"",""8901""]], UNIT[""degree"",0.01745329251994328, AUTHORITY[""EPSG"",""9122""]], AUTHORITY[""EPSG"",""4326""]]"

    ''' <summary>
    ''' Flag to indicate that a system-wide mutex should be used to safeguard spat temp data exchange. 
    ''' </summary>
    ''' <remarks>
    ''' This will be used in cEcospace.SetSpatialTemporal data. Commented:
    ''' When running multiple cores on the same computer at the same time, different processes reading the same files from disk can cause funky behaviour. 
    ''' Model that should run the same sometimes give different results. Mutex is an attempt to solve this to enforce that only ONE ecospace instance at 
    ''' the time can ask for external spat temp data.
    ''' </remarks>
    Public UseSystemMutex As Boolean = False

    Public EcosimScenarioDBID As Integer
    ''' <summary>Array of ecospace group database IDs.</summary>
    Public GroupDBID() As Integer
    ''' <summary>Array of mappings to ecopath group database IDs.</summary>
    Public EcopathGroupDBID() As Integer
    ''' <summary>Array of ecospace region database IDs.</summary>
    Public RegionDBID() As Integer
    ''' <summary>Array of ecospace habitat database IDs.</summary>
    Public HabitatDBID() As Integer
    ''' <summary>Array of ecospace MPA database IDs.</summary>
    Public MPADBID() As Integer
    ''' <summary>Array of ecospace Fleet database IDs.</summary>
    Public FleetDBID() As Integer
    ''' <summary>Array of mappings to ecopath fleet database IDs.</summary>
    Public EcopathFleetDBID() As Integer

    'number of years to run the simulation for
    Public Property TotalTime As Single

    ''' <summary>
    ''' Predict fishing effort via the Gravity attraction model
    ''' </summary>
    ''' <remarks>If = True Predict fishing effort based on Fishing Cost Map, Catch Value and Area Fished. If PredictEffort = False then use the Ecopath Effort.</remarks>
    Public PredictEffort As Boolean
    Public AdjustSpace As Boolean
    Public SpaceTime As Boolean
    Public IsFishRateSet As Boolean

    ''' <summary>
    ''' Get/set whether Ecospace will use square cells, e.g. will bypass cell width corrections.
    ''' </summary>
    Public AssumeSquareCells As Boolean = False
    ''' <summary>
    ''' Bad-ass flag, stating whether cell length can be computed from cell size and vice-versa.
    ''' This should really be properly determined from proper projections
    ''' </summary>
    Public LinkCellWidthAndSize As Boolean = True
    ''' <summary>
    ''' WKT projection string for the Ecospace coordinate system
    ''' </summary>
    Friend ProjectionString As String = DEFAULT_COORDINATESYSTEM

    Public CurrentForce As Boolean
    'jb Ecoseed may get move to an object
    'for now this will let the code function
    Public EcoseedOn As Boolean

    ''' <summary>Current model time step in years. Incremented by <see cref="TimeStep">TimeStep</see> at the end of the timestep.</summary>
    ''' <remarks>This is the time in years, not the array index.</remarks>
    Public TimeNow As Double

    ''' <summary>
    ''' Length of the time step in years 
    ''' </summary>
    ''' <remarks>1 month = 0.083333</remarks>
    Public TimeStep As Double = 1 / cCore.N_MONTHS

    ''' <summary>Current year that is being executed.</summary>
    Public YearNow As Integer = 0

    ''' <summary>Current month that is being executed.</summary>
    Public MonthNow As Integer = 0

    'jb ??? this may be temporary
    'setting of default values need to have access to Stanza and Ecosim data
    Public StanzaGroups As cStanzaDatastructures
    Public EcoPathData As cEcopathDataStructures

    ''' <summary>Number of Fishing Fleets </summary>
    ''' <remarks></remarks>
    Public nFleets As Integer

    ''' <summary>Number of Habitat types defined by the user</summary>
    Public NoHabitats As Integer

    Public nLiving As Integer

    ''' <summary>Descriptive text of habitat type (name) </summary>
    Public HabitatText() As String

    ''' <summary>The proportion to which a group prefers a habitat.</summary>
    ''' <remarks>Indexed Group,Habitat</remarks>
    Public PrefHab(,) As Single

    ''' <summary>The proportion of habitat type in a map cell.</summary>
    ''' <remarks>Sparse array (Habitat)(Row,Col)</remarks>
    Public PHabType()(,) As Single

    ''' <summary>The proportion of map cell that is fished by a fleet.</summary>
    ''' <remarks>
    ''' Computed in cEcoSpace.SetEffortParameters()
    ''' Spares Indexed (Gear)(Row,Col)
    ''' </remarks>
    Public PAreaFished()(,) As Single
    ''' <summary>Does this Fishing fleet use this habitat type </summary>
    ''' <remarks>Indexed Fleet,Habitat</remarks>
    Public GearHab(,) As Boolean

    ''' <summary>
    ''' Total number of habitat cells by habitat type
    ''' </summary>
    ''' <remarks>Caluclated in CalcHabitatArea()</remarks>
    Public HabArea() As Single

    ''' <summary>
    ''' Proportion of total area used by a habitat type
    ''' </summary>
    ''' <remarks>HabAreaProportion(iHab) = HabArea(iHab) / TotalHabitatArea </remarks>
    Public HabAreaProportion() As Single

    Public AdvectSpeed As Single
    Public jord(1000) As Integer

    Public MoveScale As Single

    ''' <summary>
    ''' Inverse of emigration response to fitness
    ''' </summary>
    ''' <remarks>In EwE5 there is no variable for this it is read from the interface when it is needed</remarks>
    Public FitnessResp As Single

    ' ''' <summary>Number of habitat time changes</summary>
    'Public NoHabChanges As Integer
    ' ''' <summary>Habitat time for NoHabChange #</summary>
    'Public HabTime() As Single
    ' ''' <summary>Habitat changes for NoHabChange #</summary>
    'Public HabChange(,) As Integer

    'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
    'Map Variables

    ''' <summary>Number of rows in the current base map</summary>
    Public InRow As Integer
    ''' <summary>Number of rows in the current base map</summary>
    Public InCol As Integer
    ''' <summary>Length in KM of a cell, used for dispersal etc.</summary>
    Public CellLength As Single
    ''' <summary>Latitude of upper left coordinate of the current basemap.</summary>
    Public Lat1 As Single
    ''' <summary>Longitude of upper left coordinate of the current basemap.</summary>
    Public Lon1 As Single
    ''' <summary>Flag, states whether the map represents the global ocean.</summary>
    Public IsGlobalMap As Boolean
    'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

    ''' <summary> Total number of stanza groups </summary>
    ''' <remarks>Sum of nStanza(isplit) for each stanza. Set in RedimMapVars()</remarks>
    Public Nvarsplit As Integer

    ''' <summary>total number of all groups </summary>
    ''' <remarks>Nvarsplit + nGroups. Set in RedimMapVars() Used for dimensioning</remarks>
    Public nvartot As Integer

    ''' <summary>Total number of cells that have water </summary>
    ''' <remarks>computed in ScaleRelativePrimaryProductivityToEcopathLevel()</remarks>
    Public nWaterCells As Integer

    Public Basebiomass() As Single
    Public Bnew() As Single
    Public der() As Single
    'Public EatEffBad() As Single
    Public MPABiomass() As Single
    ''' <summary>Movement rate?!</summary>
    Public Mrate() As Single
    Public IBMMigMovRatio() As Single
    ''' <summary>Base dispersal rate as entered by the user</summary>
    Public Mvel() As Single
    Public RelMoveBad() As Single
    Public RelVulBad() As Single
    Public IsAdvected() As Boolean

    ''' <summary> Weighting for directed movement within a migratory area (groups)</summary>
    ''' <remarks></remarks>
    Public InMigAreaMovement() As Single

    ''' <summary>Biomass by cell (row, col, group)</summary>
    Public Bcell(,,) As Single
    ''' <summary>Contaminant by cell (row, col, group)</summary>
    Public Ccell(,,) As Single
    Public Clast(,,) As Single
    Public AMmTr(,,) As Single
    Public Ftr(,,) As Single

    ''' <summary>
    ''' Total biomass loss due to Mortality Other
    ''' </summary>
    Public MOLoss()(,) As Single

    Public Blast(,,) As Single
    ''' <summary>Actual depth map as used by Ecospace, computed from <see cref="DepthInput"/> and <see cref="Excluded"/>.</summary>
    Public Depth(,) As Single
    Public DepthA(,) As Single
    Public DepthX(,) As Integer
    Public DepthY(,) As Single

    ''' <summary>Catch by Row, Col, Group.</summary>
    Public CatchMap(,,) As Single

    ''' <summary>Catch by Row, Col, Fleet.</summary>
    Public CatchFleetMap(,,) As Single

    ''' <summary>Discards (all? mortality?) by Row, Col, Group.</summary>
    ''' <remarks>This is not exposed by the interface at this time. It was included for the Biodiversity plugin and can only be accessed via code.</remarks>
    Public DiscardsMap(,,) As Single

    ''' <summary>User-entered depth map</summary>
    Public DepthInput(,) As Single
    ''' <summary>Is a cell included in modeling by Row, Col.</summary>
    Public Excluded(,) As Boolean
    ''' <summary>Modeled area, in area units^2 by Row, Col.</summary>
    Public CellArea(,) As Single
    ''' <summary>Region area, in area units^2. Region 0 represents the entire area.</summary>
    Public RegionArea() As Single
    ''' <summary>Region cells. Region 0 represents the entire area.</summary>
    Public RegionCells() As Integer

    ''' <summary>Trophic Level by Row, Col, Group.</summary>
    Public TL(,,) As Single
    ''' <summary>Trophic Level of the catch by Row, Col.</summary>
    Public TLc(,) As Single
    ''' <summary>Kemptons Q by Row, Col.</summary>
    Public KemptonsQ(,) As Single
    ''' <summary>ShannonDiversity by Row, Col.</summary>
    Public ShannonDiversity(,) As Single

    'these are all part of velmaker
    'velmaker may become its own class
    Public Xvel(,) As Single, Yvel(,) As Single
    Public Xvloc(,) As Single, Yvloc(,) As Single
    Public UpVel(,) As Single
    ''' <summary>Wind X velocity (month)(i x j)</summary>
    Public MonthlyXwind()(,) As Single
    ''' <summary>Wind Y velocity (month)(i x j)</summary>
    Public MonthlyYwind()(,) As Single

    Public MonthlyXvel()(,) As Single
    Public MonthlyYvel()(,) As Single
    Public MonthlyUpWell()(,) As Single

    Public flow(,) As Single

    Public Region(,) As Integer
    Public RelPP(,) As Single
    Public RelCin(,) As Single

    ''' <summary>
    ''' MPA layout, dimensioned as mpa x (row, col)
    ''' </summary>
    Public MPA()(,) As Integer

    ''' <summary>
    ''' Base value for relative PP (relative PP at t=0). Set after PP has been read from the database.
    ''' </summary>
    ''' <remarks>RelPP can be changed by external data this is use to restore RelPP to its original value</remarks>
    Public RelPP0(,) As Single

    ''' <summary>
    ''' Sailing cost (fleet x row x col)
    ''' </summary>
    Public Sail()(,) As Single
    Public Port()(,) As Boolean

    Public EffPower() As Single

    ''' <summary>
    ''' Ecospace base biomass gathered at the end of the first timestep after any spinup period.
    ''' </summary>
    ''' <remarks></remarks>
    Public BBase() As Single
    Public nRegions As Integer

    ''' <summary>Number of Importance layers</summary>
    Public nImportanceLayers As Integer
    Public ImportanceLayerDBID() As Integer
    Public ImportanceLayerName() As String
    Public ImportanceLayerDescription() As String
    Public ImportanceLayerWeight() As Single
    ''' <summary>Importance layer data (layer)(row, col)</summary>
    Public ImportanceLayerMap()(,) As Single

    ''' <summary>Number of environmental layers</summary>
    Public nEnvironmentalDriverLayers As Integer
    ''' <summary>Environmental layer database IDS</summary>
    Public EnvironmentalLayerDBID() As Integer
    ''' <summary>Environmental layer names</summary>
    Public EnvironmentalLayerName() As String
    ''' <summary>Environmental layer descriptions</summary>
    Public EnvironmentalLayerDescription() As String
    ''' <summary>Environmental layer units</summary>
    Public EnvironmentalLayerUnits() As String
    ''' <summary>Environmental layer data (layer)(row, col)</summary>
    Public EnvironmentalLayerMap()(,) As Single
    ''' <summary>Environmental layer capacity disabled flag (layer)</summary>
    Public EnvironmentalLayerCapacityDisabled() As Boolean

    'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
    'Capacity calculation customization flags

    ''' <summary>
    ''' If true, Ecospace is allowed to recalculate the habitat 
    ''' capacity gradient if input capacity contains values less than <see cref="MinHabCap"/> 
    ''' If false, Ecospace is only allowed to adjust 
    ''' </summary>
    Public Property UseHabCapGradientCorrections As Boolean = True

    ''' <summary>
    ''' A customizable lowest limit for acceptable habitat 
    ''' capacity values. 
    ''' </summary>
    Public Property MinHabCap As Single = 0.01F

    'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
    'Summary data

    ''' <summary>
    ''' Number of time steps for averaging summary window data
    ''' </summary>
    ''' <remarks></remarks>
    Public NumStep As Integer

    ''' <summary>Start time of the first and second summary data period. In Years </summary>
    ''' <remarks> Data is summarized over two time periods set by SumStart(0) and SumStart(1). The number of time steps to summarize over is set in NumStep.
    ''' Defaults are set in redimTimeVariables().
    ''' Used in cEcospace.summarySetTimeStep() to set the index to store the summary data in. The first or second summary period.
    ''' </remarks>
    Public SumStart(1) As Single

    ''' <summary>ResultsCatchRegionGearGroup( nRegions, nFleets, nGroups, nTimesteps)</summary>
    Public ResultsCatchRegionGearGroup(,,,) As Single
    ''' <summary>ResultsCatchRegionGearGroup( nRegions, nFleets, nGroups, nYears)</summary>
    Public ResultsCatchRegionGearGroupYear(,,,) As Single
    ''' <summary>ResultsRegionLandingsGearGroup(nRegions, nFleets, nGroups, nTimesteps)</summary>
    Public ResultsLandingsRegionGearGroup(,,,) As Single
    ''' <summary>ResultsRegionValueGearGroup(nRegions, nFleets, nGroups, nTimesteps)</summary>
    Public ResultsValueRegionGearGroup(,,,) As Single
    ''' <summary>ResultsConsumptionPredPrey(nRegions, nGroups, nGroups, nTimesteps)</summary>
    Public ResultsRegionConsumptionPredPrey(,,,) As Single

    ''' <summary>ResultsByFleet(nvars,nFleets,NumberOfTimeSteps)</summary>
    Public ResultsByFleet(,,) As Single
    ''' <summary>ResultsByFleetGroup(nvars, nFleets, nGroups, nTimesteps)</summary>
    Public ResultsByFleetGroup(,,,) As Single
    ''' <summary>ResultsRegionGroup(nRegions, nGroups, nTimesteps)</summary>
    Public ResultsRegionGroup(,,) As Single
    ''' <summary>ResultsRegionGroup(nRegions, nGroups, nYears)</summary>
    Public ResultsRegionGroupYear(,,) As Single

    ''' <summary> Summarized time step data </summary>
    ''' <remarks>populated in sumarizeTimeStepData()</remarks>
    Public ResultsByGroup(,,) As Single 'ResultsByGroup(nVars,Ngroups,  NumberOfTimeSteps)

    Public ResultsSummaryByFleet(,) As Single 'vars, fleets

    ''' <summary> Sum of landings across all cells by Group/Fleet for the current timestep </summary>
    Public Landings(,) As Single

    'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

    Public PPupWell As Single

    ''' <summary>IsMigratory flag, per group.</summary>
    Public IsMigratory() As Boolean

    ''' <summary>Migration preferred row (group, month).</summary>
    ''' <remarks>In the original version of Ecospace this value was entered as point.
    ''' In the migration update of Ecospace this value will be calculated from a monthly migration map</remarks>
    Friend PrefRow(,) As Integer
    ''' <summary>Migration preferred column (group, month).</summary>
    ''' <remarks>In the original version of Ecospace this value was entered as point.
    ''' In the migration update of Ecospace this value will be calculated from a monthly migration map</remarks>
    Friend Prefcol(,) As Integer

    ''' <summary>North-south migration concentration, per group.</summary>
    ''' <remarks>In the oringial version of Ecospace this value was entered as a fixed value per group.
    ''' In the migration update of Ecospace this value will be calculated from a monthly migration map</remarks>
    Friend MigConcRow() As Single
    ''' <summary>East-west migration concentration, per group.</summary>
    ''' <remarks>In the oringial version of Ecospace this value was entered as a fixed value per group.
    ''' In the migration update of Ecospace this value will be calculated from a monthly migration map</remarks>
    Friend MigConcCol() As Single

    ''' <summary>
    ''' Average value in the <see cref="cEcospaceDataStructures.Sail">Sail(fleet,row,col)</see> map for all water cells.
    ''' </summary>
    ''' <remarks>
    ''' Used only to distribute total effort across the cells. Does not effect the total effort.
    ''' If <see cref="cEcospaceDataStructures.UseEffortDistThreshold">bUseEffortDistThreshold</see> = True this will only include cells below the fishing effort threshold</remarks>
    Public SailScale() As Single

    ''' <summary>
    ''' Sailing Effort Multiplier by Fleet
    ''' </summary>
    ''' <remarks></remarks>
    Public SEmult() As Single

    ''' <summary>
    ''' Total fishing mortality by group,row,col
    ''' </summary>
    ''' <remarks>calculated in PredictEffortDistribution() Sum of EffortSpace() * catchability (EcoSim.relQ)</remarks>
    Public Ftot(,,) As Single

    ''' <summary>
    ''' Fishing Mortality (catchrate) by a fleet for each cell fleet,row,col
    ''' </summary>
    ''' <remarks>Computed from Ecosim.FishRateGear(fleet,time) and "gravity attraction" in PredictEffortDistribution()  </remarks>
    Public EffortSpace(,,) As Single

    ''' <summary>Number of MPAs</summary>
    Public MPAno As Integer
    Public MPAname() As String
    ''' <summary>MPA monthly open/closed state (Month x MPA), true when open for fishing.</summary>
    Public MPAmonth(,) As Boolean
    ''' <summary>MPA enforcement (fleet x MPA), true if an MPA is open to fishing for a given fleet</summary>
    Public MPAfishery(,) As Boolean

    ''' <summary>Fleet/Gear cell access (fleet, row, col).</summary>
    ''' <remarks>JS added 12 Sept 12 to determine fleet cell fishing access for the mao once 
    ''' for every time step. This is done to facilitate overlapping MPAs.</remarks>
    Friend IsFished(,,) As Boolean

    ''' <summary>
    ''' SOR weight 
    ''' </summary>
    ''' <remarks></remarks>
    Public W As Single

    ''' <summary>
    ''' Iteration tolerance for solvegrid.
    ''' </summary>
    ''' <remarks>
    ''' High values will be less accurate, but with less computing time. Reasonable values: 0.1-0.000001.
    ''' </remarks>
    Public Tol As Single

    ''' <summary>
    ''' Maximum number of iterations that solvegrid will use to find the implicit solution for the next timestep
    ''' </summary>
    ''' <remarks>
    ''' Lower numbers will be faster but less accurate. needs to be set in reasonable accord to Tol. Reasonable values: 10-100
    ''' </remarks>
    Public maxIter As Integer

    ''' <summary>
    ''' Number of threads to use for the grid solvers
    ''' </summary>
    Public nGridSolverThreads As Integer

    ''' <summary>
    ''' Number of threads to use for the IBM Movement
    ''' </summary>
    Public nIBMMovementSolverThreads As Integer

    ''' <summary>
    ''' Number of threads to run the groups biomass calculations on 
    ''' </summary>
    Public nSpaceSolverThreads As Integer

    ''' <summary>
    ''' Number of effort distribution threads
    ''' </summary>
    Public nEffortDistThreads As Integer

    'stanza groups per thread for the IBM stuff (thread x igroup)
    Public nIBMGroupsPerThread(,) As Integer

    Public nIBMPacketsPerThread As Integer

    Public SpDat As Integer
    Public SpDatYear As Integer

    Public SpName() As String
    Public SpPool() As Integer
    Public SpType() As Integer
    Public SpWt() As Single
    Public SpVal(,) As Single
    Public SpYear() As Integer
    Public SpForceBB(,) As Single
    Public SpForceCatch(,) As Single
    Public SpForceZ(,) As Single
    Public IsSpShown() As Boolean
    Public SpRegion() As Integer

    'for reference data
    Public SpaceBiomassByRegion(,,) As Single
    Public SpaceBiomassByRegionCount(,,) As Single
    Public SpaceCatchByRegion(,,) As Single
    Public SpaceCatchByRegionCount(,,) As Single
    Public SpaceEffortByRegionFleet(,,) As Single
    Public SpaceEffortByRegionFleetCount(,,) As Single

    '***************** new multistanza variables
    'Dim TotLoss() As Single, TotEatenBy() As Single, TotBiom() As Single, TotPred() As Single, IFDweight() As Single, TotIFDweight() As Single, PredCell() As Single, Blocal() As Single
    Public PredCell(,,) As Single
    Public IFDweight(,,) As Single
    Public NewMultiStanza As Boolean, IFDPower As Single
    Public ByPassIntegrate() As Boolean

    Public UseIBM As Boolean
    Public UseExact As Boolean

    'these are used to split up the species properly for threading 
    'according to # of species that are actually being integrated
    'contains the indices of ByPassIntegrate() that are FALSE
    Public integratedGroups() As Integer
    Public totalIntegratedGroups As Integer

    'these are the bounds of the water squares for each column
    'solvegrid will go from istartrow(j) to iendrow(j)
    Public iStartRow() As Integer
    Public iEndRow() As Integer
    Public jStartCol() As Integer
    Public jEndCol() As Integer


    'total number of water cells on the map
    'used by spaceSolver to split up the cells to each thread according to # of water cells
    Public iTotalWaterCells As Integer
    'for each water cell, these give the i and j coordinate of that cell
    'used by solvecell to find out which i,j to use for their current water cell
    Public iWaterCellIndex() As Integer
    Public jWaterCellIndex() As Integer

    ''' <summary>
    ''' Sum of Squares fit to reference data
    ''' </summary>
    Public SS As Single

    Public Aspace() As Single 'this is a modified Alink (from ecosim)
    Public Vspace() As Single 'this is a modified VulArena (from ecosim)

    ''' <summary>
    ''' <para>This determines how much weight is put into the pathfinding movement algorithm for migratory species.
    ''' If fish are getting caught in complex habitat, increasing this value will help the fish get "un-stuck".</para>
    ''' <para>Possible values [0-1]</para>
    ''' <para>Increasing this will increase the concentration of the fish, so the regular NS/EW concentrations should
    ''' be lowered to keep the concentration the same.</para>
    ''' </summary>
    Public barrierAvoidanceWeight() As Single

    ''' <summary>Predation rate by Row, Col, Prey/Pred link</summary>
    ''' <remarks>Added for Model coupling</remarks>
    Public MPred(,,) As Single

    ''' <summary>Detritus by Row, Col, group</summary>
    ''' <remarks>Added for Model coupling</remarks>
    Public GroupDetritus(,,) As Single

    ''' <summary>
    ''' Habitat Capacity by Row,Col,Group
    ''' </summary>
    ''' <remarks>Habitat capacity is the normalized Capacity of all inputs (maps and response functions)  <see cref="cEcoSpace.SetHabCap">Ecospace.SetHabCap</see> </remarks>
    Public HabCap()(,) As Single

    ''' <summary>
    ''' User defined input habitat capacity.
    ''' </summary>
    Public HabCapInput()(,) As Single

    ''' <summary>
    ''' Other mortality multiplier by Row,Col,Group
    ''' </summary>
    Public MOProp()(,) As Single

    ''' <summary> Sum of Capacity across the map cells by group </summary>
    Public TotHabCap() As Single

    '''<summary>max capacity by group</summary>
    ''' <remarks>Used to check that the user has set capacities for all groups </remarks>
    Public MaxHabCap() As Single

    ''' <summary>
    ''' Relative cell width due to lattitude tapering, unless <see cref="AssumeSquareCells"/> is set.
    ''' </summary>
    Public RelativeCellWidth() As Single

    Public SaveAnnual As Boolean = True
    Public SaveSelectedGroupsFleetsOnly As Boolean = False

    ''' <summary>
    ''' Use the Ecospace Output directory defined by the core. If True this path will include Model-name/Ecopath_6. Scenario-name/
    ''' If False just the core output directory.
    ''' </summary>
    ''' <remarks>
    ''' This allows you to set the Ecospace output directory from code. 
    ''' You could loop over a bunch of different cases and set the output dir for each case.
    ''' </remarks>
    Public UseCoreOutputDir As Boolean = True

    ''' Array index of the Capacity Response shape/function that gets applied to this driver
    ''' Stored by Environmental driver index (map), Group index
    ''' ishape = CapMapFunctions(iEnviroDriver,iGroup)
    Public CapacityResponseFunctions(,) As Integer

    ''' <summary>
    ''' Array index of the MO Response shape/function that gets applied to this driver
    ''' Stored by Environmental driver index (map), Group index
    ''' ishape = MortalityResposeFunctions(iEnviroDriver,iGroup)
    ''' </summary>
    Public MortalityResposeFunctions(,) As Integer

    ' Generate for each driver layer + 0 which is depth
    Public CapMaps As IEnviroInputData()

    ''' <summary>
    ''' Array of IEnviroInputData objects(in this case containing driver layers) that have been applied as MO Mortality Response functions
    ''' Initialized in cEcospaceMortalityResponseManager.Load()
    ''' </summary>
    Public MortalityResponseDrivers As IEnviroInputData()

    ''' <summary>
    ''' Capacity calculation type per group
    ''' </summary>
    Public CapCalType() As EwEUtils.Core.eEcospaceCapacityCalType

    ''' <summary>
    ''' Nearest suitable map row (iPacket) for an IBM Packet by nStanzaGroups(nSplit), MaxStanzas, row, col
    ''' </summary>
    ''' <remarks></remarks>
    Public ItoUse(,,,) As Integer

    ''' <summary>
    ''' Nearest suitable map col (jPacket) for an IBM Packet by nStanzaGroups(nSplit), MaxStanzas, row, col
    ''' </summary>
    ''' <remarks></remarks>
    Public JtoUse(,,,) As Integer

    Public MovePacketsAtStanzaEntry As Boolean

    ''' <summary>
    ''' Primary Production Scaler average value of relPP(row,col) for all water cells
    ''' </summary>
    ''' <remarks>computed by ScaleRelativePrimaryProductivityToEcopathLevel() set in InitSpatialEquilibrium. 
    ''' In EwE5 this was local to FindSpatialEquilibrium. Here it has been move up in scope so that FindSpatialEquilibrium() can be split up into components.
    ''' Init (InitSpatialEquilibrium), run (FindSpatialEquilibrium) ......
    ''' 10-May-2012 Moved to cEcoSpaceDataStructures so PPScale can be set by the External PP Spatial Temporal data 
    ''' </remarks>
    Public PPScale As Double

    Public SaveASC As Boolean = False
    Public SaveCSV As Boolean = False

    ''' <summary>
    ''' Ratio of habitat area to total habitat capacity 
    ''' </summary>
    ''' <remarks>
    ''' BRatio(group) = ThabArea / TotHabCap(group) 
    ''' [total habitat area] / [sum of habitat capacity by group] 
    ''' </remarks>
    Public BRatio() As Single


    ''' <summary>
    ''' List of map cells that have a value in the Sail(,,)array less than EffortDistThreshold
    ''' </summary>
    ''' <remarks>Populate in <see cref="PopulateFleetCells"></see></remarks>
    Public FleetSailCells() As List(Of cRowCol)

    ''' <summary>
    ''' Boolean flag is fishing effort restricted to cells with a Sail() of less than EffortDistThreshold
    ''' </summary>
    ''' <remarks>True if fishing effort is restricted. False effort is distributed over all water cells.</remarks>
    Public UseEffortDistThreshold As Boolean

    ''' <summary>
    ''' Threshold value in the Sail(fleet,row,col) [sailing cost map] for a cells inclusion in effort distribution. 
    ''' </summary>
    ''' <remarks></remarks>
    Public EffortDistThreshold As Single

    ''' <summary>
    ''' Total number of habitat area cells
    ''' Any cell with a depth > 0 of any habitat type
    ''' </summary>
    ''' <remarks>computed in CalcHabitatArea()</remarks>
    Public ThabArea As Single

    ''' <summary>
    ''' Calculate the TrophicLevel map in Ecospace. 
    ''' True Ecospace will populate the <see cref="cEcospaceDataStructures.TL">TrophicLevel</see> map in cEcospaceDataStructures.TL. 
    ''' </summary>
    ''' <remarks>This incurs significant overhead so it is Off(False) by default. At this time is can only be turned ON(True) via code.</remarks>
    Public bCalTrophicLevel As Boolean = False

    ''' <summary>
    ''' Number of fishing effort zones (LME's, EEZ...)
    ''' </summary>
    Public nEffZones As Integer

    ''' <summary>
    ''' Proportion of relative fishing effort for a fleet in an zone(LME,Region....) by nFleets, nEffZones
    ''' </summary>
    Public PropEffortFleetZone(,) As Single

    ''' <summary>
    ''' Index of the Effort Zone a cell is in by Row Col
    ''' </summary>
    Public EffZones(,) As Integer

    ''' <summary>
    ''' Sum of Effort modified by proportion of area fished in a cell.
    ''' </summary>
    ''' <remarks>
    ''' Set in <see cref="cEcospace.SetEffortParameters"> cEcospace.SetEffortParameter()
    ''' </see></remarks>
    Public TotEffort() As Single


    'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
    'Spatial effort distribution penalty variables
    Public AttractNofish() As Single
    Public NoFishWeight As Single
    Public Ftarget() As Single
    Public Pencon() As Single
    Public DoPenaltysearch As Boolean
    Public PenPow As Single
    Public FirstPenaltyMonth As Integer
    Public EffortRelaxationWeight As Single
    'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

    ''' <summary>
    ''' Use a "Spin-Up" period for Ecospace
    ''' </summary>
    Public UseSpinUp As Boolean

    ''' <summary>
    ''' List of MO layer indexs that have changed.
    ''' This will be updated(set to changed) when a layer is loaded by the spatial temporal framework
    ''' See cCapacityDataAdapter.Adapt
    ''' </summary>
    Public MOLayerChanged As List(Of Integer)

    Public RelMoveFitGroup()(,) As Single

    Public RelNutMult(,) As Single

    Public bSaveRelNutFile As Boolean

    ''' <summary>Are we in a Spin-Up period?</summary>   
    Public Property bInSpinUp As Boolean = False

    Public Property UseSpinUpPlot As Boolean

    Public Property UseSpinUpBase As Boolean

    ''' <summary>
    ''' Number of years to run the Spin-Up for
    ''' </summary>
    Public Property SpinUpYears As Single

    ''' <summary>
    ''' Ecospace base biomass before the Spin-Up period. Gathered at the end of the first timestep.
    ''' </summary>
    ''' <remarks>Only populted if UseSpin = True</remarks>
    Public SpinUpBBase() As Single

    Public BaseFishMort() As Single
    Public BaseCatch() As Single
    Public BaseConsump() As Single
    Public BasePredMort() As Single

    Public isGroupHabCapChanged() As Boolean

    ''' <summary>
    ''' The Capacity model has a "one time" initialization of  <see cref="MaxHabCap"></see> value used for normalization of inputs.
    ''' This Flag gets set to True once MaxHabCap() has been set 
    ''' </summary>
    ''' <remarks></remarks>
    Public hasCapInitialized As Boolean

    ''' <summary>
    ''' User defined output directory for Ecospace output Maps
    ''' </summary>
    ''' <remarks>
    ''' Not used by the Scientic Interface. 
    ''' This is only used if UseCoreOutputDir = False and EcospaceMapOutputDir is not null.   
    ''' </remarks>
    Public EcospaceMapOutputDir As String

    ''' <summary>
    ''' User defined output directory for Ecospace Area Averaged outputs
    ''' </summary>
    ''' <remarks>
    ''' Not used by the Scientic Interface. 
    ''' This is only used if UseCoreOutputDir = False and EcospaceAreaOutputDir is not null.
    ''' </remarks>
    Public EcospaceAreaOutputDir As String

    ''' <summary>
    ''' First model time step to being writing Ecospace output files
    ''' </summary>
    ''' <remarks></remarks>
    Public FirstOutputTimeStep As Integer = 1

    ''' <summary>
    ''' Monthly Migration maps stored in a ragged array 
    ''' Dimensioned by (group,month)(row,col)
    ''' </summary>
    ''' <remarks></remarks>
    Public MigMaps(,)(,) As Single

    ''' <summary>
    ''' Is the Ecosim biomass time series forcing enabled for this group
    ''' </summary>
    Public IsEcosimBioForcingGroup() As Boolean
    Public UseEcosimBiomassForcing As Boolean = False

    Public IsEcosimDiscardForcingGroup() As Boolean
    Public UseEcosimDiscardForcing As Boolean = False

    Public m_enaCellData As Dictionary(Of String, cENAData)

    Public bENA As Boolean

    Public Kmovefit() As Single

    Public RelFitnessBase(,,) As Single

    ''' <summary>
    ''' Save the threading run time log. By default this will be turned off, the log will not be saved. 
    ''' Used by plugins or external process to optimize run threading.
    ''' </summary>
    Public bSaveThreadingLog As Boolean

    ''' <summary>
    ''' Flow of detritus for each cell in the Ecospace map 
    ''' </summary>
    Public ConKdetSpace(,)(,,) As Single '(row,col)(ngroups, ndetritus,nfleets)

    Public ConKtrophic(,)() As Single '(row,col)(inLinks)


#End Region

#Region "Private Data"

    'not much
    Private m_ngroups As Integer
    Private m_publisher As cMessagePublisher

#End Region

#Region "Construction"

    Public Sub New(ByVal MessagePublisher As cMessagePublisher)
        Me.m_publisher = MessagePublisher
    End Sub

#End Region

#Region "Public Properties"

    '''<summary>
    ''' Have any of the capacity input layers changed
    ''' </summary>
    ''' <remarks>Capacity Inputs, Habitats, Environmental layers, depth....</remarks>
    Public Property isCapacityChanged() As Boolean = False

    ''' <summary>
    ''' Set the isGroupHabCapChanged() flag to True or False for all the groups
    ''' </summary>
    ''' <param name="Value"></param>
    ''' <remarks></remarks>
    Public Sub setHabCapGroupIsChanged(ByVal Value As Boolean)
        For igrp As Integer = 1 To Me.NGroups
            Me.isGroupHabCapChanged(igrp) = Value
        Next
    End Sub

    '''<summary>
    ''' Habitat input layers have changed, which could affect fishing
    ''' </summary>
    Public Property isFishingHabitatChanged() As Boolean = False

    ''' <summary>Number of Base Groups (Ecopath) </summary>
    ''' <remarks>This was nvar in EwE5</remarks>
    Public Property NGroups() As Integer
        Get
            Return Me.m_ngroups
        End Get
        Set(ByVal value As Integer)
            Me.m_ngroups = value
            Me.RedimGroups() 'implicit ??????
            'this is different then the other counters (nFleets....) 
            'which delay the dimensioning until the data is loaded
            'this may not be a good idea
        End Set
    End Property

    Public ReadOnly Property nTimeSteps() As Integer

        Get
            Return CInt(Me.TotalTime * (1 / Me.TimeStep))
        End Get

    End Property

    ''' <summary>
    ''' Number of Ecospace time steps per year at the current <see cref="TimeStep">time step</see>
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property nTimeStepsPerYear As Integer
        Get
            Return CInt(1 / Me.TimeStep)
        End Get
    End Property

    ''' <summary>
    ''' Returns whether any group is Advected
    ''' </summary>
    Public ReadOnly Property isAdvectionActive As Boolean
        Get
            For igrp As Integer = 1 To Me.m_ngroups
                If Me.IsAdvected(igrp) Then Return True
            Next
            Return False
        End Get
    End Property

    ''' <summary>
    ''' Get/set whether advection is forced with external data. If so,
    ''' monthly advection patterns will not be used, and external data is 
    ''' relied on instead.
    ''' </summary>
    Public Property isAdvectionForced As Boolean = False

    ''' <summary>
    ''' Returns whether any group is forced through biomass timeseries in Ecosim.
    ''' </summary>
    Public ReadOnly Property isEcosimBiomassForcingLoaded As Boolean
        Get
            For igrp As Integer = 1 To Me.NGroups
                If Me.IsEcosimBioForcingGroup(igrp) Then
                    Return True
                End If
            Next
            Return False
        End Get
    End Property

    ''' <summary>
    ''' Returns whether any group is forced through discards timeseries in Ecosim.
    ''' </summary>
    Public ReadOnly Property isEcosimDiscardForcingLoaded As Boolean
        Get
            For igrp As Integer = 1 To Me.NGroups
                If Me.IsEcosimDiscardForcingGroup(igrp) Then
                    Return True
                End If
            Next
            Return False
        End Get
    End Property

#End Region

#Region "Public Methods"

    Public Sub Clear()
        Me.m_ngroups = 0
        Me.nFleets = 0
        Me.TotalTime = 0
        Me.TotalTime = 0
        Me.nRegions = 0
        Me.InCol = 0
        Me.InRow = 0
        Me.nvartot = 0
        Me.NoHabitats = 0
        Me.UseEcosimBiomassForcing = False

        Try

            Me.Depth = Nothing
            Me.DepthA = Nothing
            Me.DepthX = Nothing
            Me.DepthY = Nothing
            Me.Xvel = Nothing
            Me.Yvel = Nothing
            Me.Xvloc = Nothing
            Me.Yvloc = Nothing
            Me.UpVel = Nothing
            Me.MonthlyXwind = Nothing
            Me.MonthlyYwind = Nothing
            Me.flow = Nothing
            Me.Region = Nothing
            Me.MPA = Nothing
            Me.RelPP = Nothing
            Me.RelCin = Nothing
            Me.Sail = Nothing
            Me.GroupDetritus = Nothing

            Me.Basebiomass = Nothing
            Me.Bnew = Nothing
            Me.der = Nothing
            'EatEffBad = Nothing
            Me.MPABiomass = Nothing
            Me.Mrate = Nothing
            Me.Mvel = Nothing
            Me.RelMoveBad = Nothing
            Me.RelVulBad = Nothing
            Me.IsAdvected = Nothing

            Me.PrefRow = Nothing
            Me.Prefcol = Nothing
            Me.IsMigratory = Nothing
            Me.MigConcRow = Nothing
            Me.MigConcCol = Nothing
            Me.barrierAvoidanceWeight = Nothing
            Me.MigMaps = Nothing

            Me.MPADBID = Nothing '(Me.MPAno)
            Me.MPAname = Nothing '(Me.MPAno)
            Me.MPAmonth = Nothing '(12, Me.MPAno)
            Me.MPAfishery = Nothing '(Me.nFleets, Me.MPAno)

            Me.ResultsByGroup = Nothing ', N_RESULTS_GROUPS, m_ngroups, NumberOfTimeSteps)
            Me.ResultsByFleet = Nothing ', N_RESULTS_FLEETS, nFleets, NumberOfTimeSteps)
            Me.ResultsByFleetGroup = Nothing ', N_RESULTS_FLEETGROUPS, nFleets, NGroups, NumberOfTimeSteps)
            Me.ResultsRegionGroup = Nothing ', NoRegions, NGroups, NumberOfTimeSteps)
            Me.ResultsCatchRegionGearGroup = Nothing ', NoRegions, nFleets, NGroups, NumberOfTimeSteps)
            Me.ResultsLandingsRegionGearGroup = Nothing ', NoRegions, nFleets, NGroups, NumberOfTimeSteps)
            Me.ResultsValueRegionGearGroup = Nothing ', NoRegions, nFleets, NGroups, NumberOfTimeSteps)
            Me.ResultsRegionConsumptionPredPrey = Nothing

            Me.MPred = Nothing
            Me.EffortSpace = Nothing
            Me.PredCell = Nothing
            Me.IFDweight = Nothing
            Me.Ftot = Nothing
            Me.EffPower = Nothing
            Me.SEmult = Nothing
            Me.HabAreaProportion = Nothing
            Me.HabArea = Nothing
            Me.PHabType = Nothing
            Me.FleetSailCells = Nothing

            Me.MigMaps = Nothing

            If Me.m_enaCellData IsNot Nothing Then Me.m_enaCellData.Clear()
            Me.m_enaCellData = Nothing

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".Clear() Exception: " & ex.Message)
        End Try

    End Sub

    ''' <summary>
    ''' Set default values and dimemsion basic arrays
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>This should be called before reading values from database. I think..... I hope!!!!!!!!!!</remarks>
    Public Function SetDefaults() As Boolean
        Dim i As Integer

        Try

            'EwE5 default value hardwired into the interface
            Me.FitnessResp = 100
            Me.PPupWell = 0.01
            Me.PredictEffort = True

            'SOR weight from EwE5 interface frmSpace.text3
            Me.W = 0.9

            Me.TimeStep = 1 / 12 'monthly time steps. In EwE5 this is set all over the place 

            'EwE5 set to True in frmSpace.Form_Activate()
            'its value is then changed from an option radio button SpaceInit() on the run tab
            Me.AdjustSpace = True

            'jb SpaceTime and CurrentForce defaults from EwE5 frmSpace.Load()
            Me.SpaceTime = True 'in EwE5 the check box that controls this is labled 'Integrate' on the run tab
            Me.CurrentForce = False

            Me.InRow = 0
            Me.InCol = 0

            Me.AdvectSpeed = 0.1

            Me.CellLength = 100 'this is from the EwE5 database

            Me.MoveScale = 2.0 '0.2
            If Me.TotalTime = 0 Then Me.TotalTime = 50 'default of 50 year simulation

            'redimTimeVariables()
            Me.setDefaultSummaryPeriod()

            Me.NoHabitats = 1
            'requires NoHabitats, nGroups, nFleets, NoHabChanges
            Me.RedimHabitatVariables()

            'dimension arrays to current problem size
            'DefaultBasemapDimensions()
            Me.ReDimMapVars()

            Me.RedimMigratoryVariables()

            Me.SetDefaultMeanVelocityMvel()


            Me.DoPenaltysearch = True
            Me.NoFishWeight = 0.3
            Me.PenPow = 10.0
            Me.FirstPenaltyMonth = 5 * 12 ' five years
            EffortRelaxationWeight = 0.9
            ReDim Me.Ftarget(Me.NGroups)
            For i = 1 To Me.NGroups                            'CJW had nvar not n1
                Me.PrefHab(i, 0) = 1.0! ' True
                Me.InMigAreaMovement(i) = 0.1F
                Me.Kmovefit(i) = 0
                Ftarget(i) = 1000.0
            Next 'set preferred habitat to 1 (pelagic) by default
            'Debug.Assert(False, "Ftarget(3) = 0.05")
            'If NGroups > 0 Then Ftarget(3) = 0.05


            Me.ReDimFleets()

            Me.UseEffortDistThreshold = False
            Me.EffortDistThreshold = 10000

            'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
            'Spin Up
            Me.UseSpinUp = False
            Me.UseSpinUpPlot = False
            Me.SpinUpYears = 10
            'xxxxxxxxxxxxxxxxxxxxxxxxx

            Me.EcospaceAreaOutputDir = ""
            Me.EcospaceMapOutputDir = ""

            Me.bSaveRelNutFile = False
            Me.bSaveThreadingLog = False

            Return True
        Catch ex As Exception
            Debug.Assert(False, ex.Message)
            Return False
        End Try

    End Function


    Public Sub SetDefaultThreads()
        'multi threading defaults
        ' JS 08jun07: added 0 check since the datasource may have provided these values
        If (Me.nSpaceSolverThreads <= 0) Then
            Me.nGridSolverThreads = System.Environment.ProcessorCount
            Me.nSpaceSolverThreads = System.Environment.ProcessorCount
            Me.nEffortDistThreads = System.Environment.ProcessorCount
        End If

        'Yeah do the IBM Movement threads separately
        'Because it's not save in the database at this time
        If (Me.nIBMMovementSolverThreads <= 0) Then Me.nIBMMovementSolverThreads = System.Environment.ProcessorCount

    End Sub

    Private Sub SetDefaultMeanVelocityMvel()
        Dim i As Integer
        Dim j As Integer

        Try

            Debug.Assert(Me.EcoPathData IsNot Nothing, "Ecospace must have a reference to Ecopath data to initialize.")

            'Dim MaxTL As Single
            'MaxTL = 0
            'For j = 1 To NumLiving
            '    If TTLX(j) > MaxTL Then MaxTL = TTLX(j)
            'Next
            'MaxTL = MaxTL - 1
            'Set max average velocity movement to 100 km/year and the others linearly scaled after trophic level
            For j = 1 To Me.NGroups  'NumLiving
                Me.Mvel(j) = 300   'CInt(99 * (1 - (MaxTL - (TTLX(j) - 1)) / MaxTL)) + 1
            Next
            'For j = NumLiving + 1 To NumGroups
            '    Mvel(j) = 1
            'Next
            'How about discards they should have a lower dispersal rate:
            'check the discard fate
            'DiscardFate(NumGear, NumGroups - NumLiving)
            For j = Me.nLiving + 1 To Me.NGroups
                For i = 1 To Me.nFleets
                    If Me.EcoPathData.DiscardFate(i, j - Me.nLiving) > 0 Then
                        Me.Mvel(j) = 10
                        Exit For
                    End If
                Next
            Next

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".SetDefaultMeanVelocityMvel() Error: " & ex.Message)
            Throw New System.Exception(Me.ToString & ".SetDefaultMeanVelocityMvel() Error: " & ex.Message)
        End Try


    End Sub

    ''' <summary>
    ''' Redim variables for MPAs
    ''' </summary>
    ''' <remarks>In EwE5 this was handled when Ecosim loaded</remarks>
    Public Sub RedimMPAVariables()
        Try
            ReDim Me.MPADBID(Me.MPAno)
            ReDim Me.MPAname(Me.MPAno)
            ReDim Me.MPAmonth(12, Me.MPAno)
            ReDim Me.MPAfishery(Me.nFleets, Me.MPAno)
            Me.allocate(Me.MPA, Me.MPAno, Me.InRow + 1, Me.InCol + 1)
        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".RedimMPAVariables() Error: " & ex.Message)
            Throw New System.Exception(Me.ToString & ".RedimMPAVariables() Error: " & ex.Message)
        End Try


    End Sub

    ''' <summary>
    ''' Redim variables for migratory preferences
    ''' </summary>
    ''' <remarks>In EwE5 this was handled when Ecosim loaded</remarks>
    Public Sub RedimMigratoryVariables()
        Try

            ReDim Me.IsMigratory(Me.nvartot)
            ReDim Me.PrefRow(Me.NGroups, 12)
            ReDim Me.Prefcol(Me.NGroups, 12)
            ReDim Me.MigConcRow(Me.NGroups)
            ReDim Me.MigConcCol(Me.NGroups)
            ReDim Me.barrierAvoidanceWeight(Me.NGroups)

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".RedimMigratoryVariables() Error: " & ex.Message)
            Throw New System.Exception(Me.ToString & ".RedimMigratoryVariables() Error: " & ex.Message)
        End Try


    End Sub

    ''' <summary>
    '''  Re-dimension the habitat variables
    ''' </summary>
    ''' <param name="PreserveHabitat">True to preserve the existing data in the habitat array. False to clear out this data (load a new model)</param>
    ''' <remarks>
    ''' This is called when ever the number of groups or habitat types changes.
    ''' Called when a new model is loaded (PreserveHabitat = False) or the user has changed the number of habitat types (PreserveHabitat = True).
    ''' If only the number of habitats has changed then it will keep the existing data (PreserveHabitat = True). 
    ''' If the number of groups has changed then all the data must be re-initialized (from the datasource).
    '''</remarks>
    Public Sub RedimHabitatVariables(Optional ByVal PreserveHabitat As Boolean = False)

        Try

            If Not PreserveHabitat Then
                'new model is being read
                'clear out the exiting data
                ReDim Me.PrefHab(Me.NGroups, Me.NoHabitats)
                ReDim Me.GearHab(Me.nFleets, Me.NoHabitats)
                ReDim Me.HabitatText(Me.NoHabitats)
                ReDim Me.HabArea(Me.NoHabitats)
                ReDim Me.HabAreaProportion(Me.NoHabitats)
                ReDim Me.HabitatDBID(Me.NoHabitats)

                Me.allocate(Me.PHabType, Me.NoHabitats, Me.InRow, Me.InCol)

                ' JS 15oct07: fix for bug 289 - By default, GearHab and PrefHab are True for 'All' habitat
                For iGroup As Integer = 0 To Me.NGroups
                    Me.PrefHab(iGroup, 0) = 1.0! ' True
                Next

                For iFleet As Integer = 0 To Me.nFleets
                    Me.GearHab(iFleet, 0) = True
                Next

            Else
                'only the number of habitats has changed 
                'keep the existing data
                ReDim Preserve Me.PrefHab(Me.NGroups, Me.NoHabitats)
                ReDim Preserve Me.GearHab(Me.nFleets, Me.NoHabitats)
                ReDim Preserve Me.HabitatText(Me.NoHabitats)
                ReDim Preserve Me.HabArea(Me.NoHabitats)
                ReDim Preserve Me.HabAreaProportion(Me.NoHabitats)
                ReDim Preserve Me.HabitatDBID(Me.NoHabitats)

            End If

            Me.allocate(Me.PHabType, Me.NoHabitats, Me.InRow, Me.InCol)

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".RedimHabitatVariables() Error: " & ex.Message)
            Throw New System.Exception(Me.ToString & ".RedimHabitatVariables() Error: " & ex.Message)
        End Try


    End Sub

    ''' <summary>
    ''' Set the Map to its default size
    ''' </summary>
    Public Sub DefaultBasemapDimensions()

        If Me.InRow = 0 Then Me.InRow = 20 'number of map cell rows
        If Me.InCol = 0 Then Me.InCol = 20 'number of map cell columns
        If Me.CellLength = 0 Then Me.CellLength = 5 'map cell size, in degrees

    End Sub

    Sub ReDimMapVars()
        Dim i As Integer, j As Integer

        Try

            Debug.Assert(Me.StanzaGroups IsNot Nothing, Me.ToString & ".ReDimMapVars() Stanzagroups needs to be set.")

            'count up the total number of stanza groups
            Me.Nvarsplit = 0
            For i = 1 To Me.StanzaGroups.Nsplit
                For j = 1 To Me.StanzaGroups.Nstanza(i)
                    Me.Nvarsplit = Me.Nvarsplit + 1
                Next
            Next

            'jb EwE5 EwE6 does not have Pairs (split pools)
            'nvartot = NumGroups + 2 * npairs + Nvarsplit
            Me.nvartot = Me.NGroups + Me.Nvarsplit

            ReDim Me.Basebiomass(Me.nvartot)
            ReDim Me.Bnew(Me.nvartot)
            ReDim Me.der(Me.nvartot)
            'ReDim EatEff(nvartot)
            'ReDim EatEffBad(nvartot)
            'ReDim Flowin(nvartot)
            'ReDim FlowoutRate(nvartot)
            ReDim Me.MPABiomass(Me.nvartot)
            ReDim Me.Mrate(Me.nvartot)
            ReDim Me.Mvel(Me.nvartot)
            ReDim Me.RelMoveBad(Me.nvartot)
            ReDim Me.RelVulBad(Me.nvartot)
            ReDim Me.IsAdvected(Me.NGroups)
            ReDim Me.TotHabCap(Me.NGroups)
            ReDim Me.MaxHabCap(Me.NGroups)

            ReDim Me.IBMMigMovRatio(Me.NGroups)

            ReDim Me.InMigAreaMovement(Me.NGroups)

            ' Allocate room for Depth map
            ReDim Me.CapacityResponseFunctions(Me.nEnvironmentalDriverLayers + 1, Me.NGroups)

            ReDim Me.MortalityResposeFunctions(Me.nEnvironmentalDriverLayers + 1, Me.NGroups)

            ReDim Me.ImportanceLayerDBID(Me.nImportanceLayers)
            ReDim Me.ImportanceLayerName(Me.nImportanceLayers)
            ReDim Me.ImportanceLayerDescription(Me.nImportanceLayers)
            ReDim Me.ImportanceLayerWeight(Me.nImportanceLayers)

            ReDim Me.EnvironmentalLayerDBID(Me.nEnvironmentalDriverLayers)
            ReDim Me.EnvironmentalLayerName(Me.nEnvironmentalDriverLayers)
            ReDim Me.EnvironmentalLayerDescription(Me.nEnvironmentalDriverLayers)
            ReDim Me.EnvironmentalLayerUnits(Me.nEnvironmentalDriverLayers)
            ReDim Me.EnvironmentalLayerCapacityDisabled(Me.nEnvironmentalDriverLayers)

            Me.MOLayerChanged = New List(Of Integer)

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".ReDimMapVars() Error: " & ex.Message)
            Throw New System.Exception(Me.ToString & ".ReDimMapVars() Error: " & ex.Message)
        End Try

    End Sub

    Public Sub ReDimFleets()
        Try

            ReDim Me.FleetDBID(Me.nFleets)
            ReDim Me.EcopathFleetDBID(Me.nFleets)
            ReDim Me.SEmult(Me.nFleets)
            ReDim Me.EffPower(Me.nFleets)

            Me.setFleetDefaults()

            ReDim Me.FleetSailCells(Me.nFleets)
            For iflt As Integer = 1 To Me.nFleets
                Me.FleetSailCells(iflt) = New List(Of cRowCol)
            Next

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".ReDimFleets() Error: " & ex.Message)
            Throw New System.Exception(Me.ToString & ".ReDimFleets() Error: " & ex.Message)
        End Try

    End Sub

    ''' <summary>
    ''' Dimensions and sets the number of Effort Zones
    ''' </summary>
    ''' <remarks>Sets PropEffortFleetArea(nFleets,nAreas) to a default of one</remarks>
    Public Sub ReDimEffortZones()
        Debug.Assert(Me.nEffZones > 0, "ReDimPropEffortArea(nAreas) NumberOfAreas must be greater than 0.")

        Me.PropEffortFleetZone = New Single(Me.nFleets, Me.nEffZones) {}

        For iflt As Integer = 1 To Me.nFleets
            'Default proportion of effort in an area should sum to  = 1
            For iarea As Integer = 0 To Me.nEffZones
                Me.PropEffortFleetZone(iflt, iarea) = CSng(1 / (nEffZones + 1.0E-20))
            Next iarea
        Next

    End Sub


    Private Sub setFleetDefaults()

        'jb just set to default of one
        For i As Integer = 1 To Me.nFleets
            Me.EffPower(i) = 1
            Me.SEmult(i) = 1
        Next

    End Sub

    ''' <summary>
    ''' Make sure there are migration maps for all migrating groups.
    ''' </summary>
    ''' <remarks>
    ''' On the first load new maps should be allocated for all the groups. 
    ''' Once a model has loaded all the existing maps should be preserved and only new maps should get new blank maps.
    ''' This should preserve the users configuration if the turn a migrating group on or off. 
    ''' </remarks>
    Friend Sub RedimMigrationMaps(bClearExisting As Boolean)

        If bClearExisting Then
            Me.MigMaps = Nothing
        End If

        If (Me.MigMaps Is Nothing) Then
            Me.MigMaps = New Single(Me.NGroups, 12)(,) {}
        End If

        '  Me.MigMaps = New Single(NGroups, 12)(,) {}
        For iGrp As Integer = 1 To Me.NGroups
            If Me.IsMigratory(iGrp) Then
                If (Me.MigMaps(iGrp, 1) Is Nothing) Then
                    For iMonth As Integer = 1 To 12
                        Me.MigMaps(iGrp, iMonth) = New Single(Me.InRow + 1, Me.InCol + 1) {}
                    Next
                End If
            End If
        Next
    End Sub

    Friend Sub DebugTestEffortZones()

        Return

        'Warning
        Debug.Assert(False, "Effort Zones have been set for debugging.")

        Me.nEffZones = 4
        Me.ReDimEffortZones()

        For iflt As Integer = 1 To Me.nFleets
            For iz As Integer = 1 To Me.nEffZones
                'Effort by zone
                Me.PropEffortFleetZone(iflt, iz) = CSng(iz ^ 2.0) 'CSng(iz / Me.nEffZones)
            Next
        Next

        Dim iiz As Integer
        For ir As Integer = 1 To Me.InRow
            For ic As Integer = 1 To Me.InCol
                iiz = 1
                If ic > (Me.InCol / 2) Then
                    iiz = 2
                End If
                Me.EffZones(ir, ic) = iiz
            Next
        Next




        'Dim iseq As Integer
        'For ir As Integer = 1 To Me.InRow
        '    For ic As Integer = 1 To Me.InCol
        '        iseq += 1
        '        Me.EffZones(ir, ic) = 1 + CInt((Me.nEffZones - 1) * (iseq / (Me.InRow * Me.InCol)))
        '    Next
        'Next

    End Sub


    'Friend Sub debugSetAdvectionVectors()
    '    Debug.Assert(False, "Warning Advection Vectors have been hardcoded for debuging...")
    '    ReDim Me.Xvel(Me.InRow + 1, Me.InCol + 1)
    '    ReDim Me.Yvel(Me.InRow + 1, Me.InCol + 1)
    '    Dim vel As Single = 0
    '    For i As Integer = 0 To Me.InRow + 1
    '        For j As Integer = 0 To Me.InCol + 1
    '            '  If Me.Depth(i, j) > 0 Then
    '            Me.Xvel(i, j) = vel
    '            Me.Yvel(i, j) = vel
    '            vel += 1
    '            '  End If
    '        Next j
    '    Next i
    'End Sub

    'Friend Sub debugSetMigMapsFromPrefRowCol()

    '    Debug.Assert(False, "Warning debugSetMigrationMaps() Setting Migration Maps with values in PrefRow() and PrefCol()")
    '    Dim OffSet As Integer = 1
    '    Dim i1 As Integer, i2 As Integer, j1 As Integer, j2 As Integer
    '    For igrp As Integer = 1 To NGroups
    '        For imon As Integer = 1 To 12
    '            If IsMigratory(igrp) Then
    '                For irow As Integer = 1 To InRow
    '                    For icol As Integer = 1 To InCol

    '                        If PrefRow(igrp, imon) = irow And icol = Prefcol(igrp, imon) Then

    '                            i1 = irow - OffSet : If i1 < 1 Then i1 = 1
    '                            i2 = irow + OffSet : If i2 > InRow Then i2 = InRow
    '                            j1 = icol - OffSet : If j1 < 1 Then j1 = 1
    '                            j2 = icol + OffSet : If j2 > InCol Then j2 = InCol
    '                            For ii As Integer = i1 To i2
    '                                For jj As Integer = j1 To j2
    '                                    Me.MigMaps(igrp, imon)(ii, jj) = True
    '                                Next
    '                            Next


    '                        End If

    '                    Next icol
    '                Next irow
    '            End If 'If IsMigratory(igrp) Then
    '        Next imon
    '    Next igrp
    'End Sub

    Friend Sub calcPrefRowColFromMigrationMap()

        Debug.Assert(False, "Warning debugCalcPrefRowColFromMap() Calculating PrefRow() PrefCol() from Migration Maps")
        Dim minRow As Integer, maxRow As Integer, minCol As Integer, maxCol As Integer
        For igrp As Integer = 1 To Me.NGroups
            If Me.IsMigratory(igrp) Then
                For imon As Integer = 1 To 12
                    minRow = Me.InRow + 1
                    minCol = Me.InCol + 1
                    maxRow = 0
                    maxCol = 0

                    For irow As Integer = 1 To Me.InRow
                        For icol As Integer = 1 To Me.InCol

                            If (Me.MigMaps(igrp, imon)(irow, icol) > cEcoSpace.MIN_MIG_PROB) Then
                                minRow = Math.Min(irow, minRow)
                                minCol = Math.Min(icol, minCol)
                                maxRow = Math.Max(irow, maxRow)
                                maxCol = Math.Max(icol, maxCol)
                            End If
                        Next icol
                    Next irow
                    Me.PrefRow(igrp, imon) = (minRow + maxRow) \ 2
                    Me.Prefcol(igrp, imon) = (minCol + maxCol) \ 2

                Next imon
            End If 'If IsMigratory(igrp) Then
        Next igrp
    End Sub


    Friend Sub debugTestDiscardsMaps()
        Dim sumDiscards As Single
        Dim n As Integer

        System.Console.WriteLine("---------------Discards Dump-------------------")

        For igrp As Integer = 1 To Me.NGroups
            sumDiscards = 0
            n = 0
            For ir As Integer = 1 To Me.InRow
                For ic As Integer = 1 To Me.InCol
                    If Me.DiscardsMap(ir, ic, igrp) > 0 Then
                        sumDiscards += Me.DiscardsMap(ir, ic, igrp)
                        n += 1
                    End If
                Next ic
            Next ir

            If sumDiscards > 0 Then
                System.Console.WriteLine("Discards for group " + igrp.ToString + " = " + (sumDiscards / n).ToString)
            End If

        Next igrp

        System.Console.WriteLine("---------------END Discards Dump-------------------")


    End Sub


    Friend Sub debugDumpContaminantMap(foriGroup As Integer)
        System.Console.WriteLine("-------------Contaminants for " + foriGroup.ToString + "----------")
        Dim sumC As Single
        For ir As Integer = 1 To Me.InRow
            For ic As Integer = 1 To Me.InCol
                System.Console.Write(Me.Ccell(ir, ic, foriGroup).ToString)
                If ic <> Me.InCol Then
                    System.Console.Write(", ")
                Else
                    System.Console.WriteLine()
                End If

                sumC += (Me.Ccell(ir, ic, foriGroup))
            Next
        Next


        System.Console.WriteLine("Sum, " + sumC.ToString)
        System.Console.WriteLine("-----------------------")
    End Sub



    ''' <summary>
    ''' Populate <see cref="cEcospaceDataStructures.FleetSailCells"></see> with a list of map cells in <see cref="cEcospaceDataStructures.Sail"></see> that are less than <see cref="cEcospaceDataStructures.EffortDistThreshold"></see>
    ''' </summary>
    ''' <remarks> FleetSailCells is used by <see cref="cEcoSpace.PredictEffortDistributionThreadedLoadShared"></see> to calculate effort distribution only on cells in the list</remarks>
    Public Sub PopulateFleetCells()

        If Not Me.UseEffortDistThreshold Then Return

        System.Console.WriteLine("Calculating map cells per fleet.")
        System.Console.WriteLine("Number of water cells " + Me.nWaterCells.ToString)
        For iflt As Integer = 1 To Me.nFleets
            Me.FleetSailCells(iflt).Clear()
            For ir As Integer = 1 To Me.InRow
                For ic As Integer = 1 To Me.InCol
                    If Me.Depth(ir, ic) > 0 Then
                        If Me.Sail(iflt)(ir, ic) < Me.EffortDistThreshold Then
                            Me.FleetSailCells(iflt).Add(New cRowCol(ir, ic))
                        End If 'Me.Sail(iflt, ir, ic) < Me.FleetSailThreshold 
                    End If 'Depth(ir, ic) > 0
                Next ic
            Next ir

            System.Console.WriteLine("  Fleet " + iflt.ToString + " n cells, " + Me.FleetSailCells(iflt).Count.ToString)

        Next iflt

    End Sub

    ''' <summary>
    ''' Allocate memory for an array with 4 dimensions
    ''' </summary>
    ''' <remarks>Do garbage collection on the discarded memory so memory in never allocated twice.</remarks>
    Friend Sub allocate(ByRef array(,,,) As Single, ByVal d1 As Integer, ByVal d2 As Integer, ByVal d3 As Integer, ByVal d4 As Integer)

        If array IsNot Nothing Then
            If array.Length = (d1 + 1) * (d2 + 1) * (d3 + 1) * (d4 + 1) Then
                System.Array.Clear(array, 0, array.Length)
                Return
            End If
        End If

        Erase array
        array = Nothing
        'Dim mgs As Single = CSng(d1 * d2 * d3 * d4 * 4 / 1048576)
        'System.Console.WriteLine("Allocating=" & mgs.ToString & " Memory=" & (GC.GetTotalMemory(True) / 1048576).ToString)
        'GC.Collect()

        ReDim array(d1, d2, d3, d4)

    End Sub

    ''' <summary>
    ''' Allocate memory for an array with 3 dimensions
    ''' </summary>
    ''' <remarks>Do garbage collection on the discarded memory so memory in never allocated twice.</remarks>
    Friend Sub allocate(ByRef array(,,) As Single, ByVal d1 As Integer, ByVal d2 As Integer, ByVal d3 As Integer)

        If array IsNot Nothing Then
            If array.Length = (d1 + 1) * (d2 + 1) * (d3 + 1) Then
                System.Array.Clear(array, 0, array.Length)
                Return
            End If
        End If

        Erase array
        array = Nothing

        'GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced)
        'Dim mgs As Single = CSng(d1 * d2 * d3 * 4 / 1048576)
        'System.Console.WriteLine("Allocating=" & mgs.ToString & " Memory=" & (GC.GetTotalMemory(True) / 1048576).ToString)

        GC.Collect()
        ReDim array(d1, d2, d3)

    End Sub

    ''' <summary>
    ''' Allocate memory for an array with 3 dimensions
    ''' </summary>
    ''' <remarks>Do garbage collection on the discarded memory so memory in never allocated twice.</remarks>
    Friend Sub allocate(ByRef array()(,) As Single, ByVal nGroupsFleets As Integer, ByVal d2 As Integer, ByVal d3 As Integer)
        Dim bCleared As Boolean = True

        If array IsNot Nothing Then
            If array.Length = (nGroupsFleets + 1) Then

                For i As Integer = 0 To nGroupsFleets
                    If array(i).Length = (d2 + 1) * (d3 + 1) Then
                        System.Array.Clear(array(i), 0, array(i).Length)
                    Else
                        bCleared = False
                        Exit For
                    End If
                Next

                'If we managed to clear the array then Return
                'If NOT the allocate a new array
                If bCleared Then
                    Return
                End If

            End If
        End If

        Erase array
        array = Nothing

        array = New Single(nGroupsFleets)(,) {}
        For i As Integer = 0 To nGroupsFleets
            array(i) = New Single(d2, d3) {}
        Next
        GC.Collect()

    End Sub

    ''' <summary>
    ''' Allocate memory for an array with 3 dimensions
    ''' </summary>
    ''' <remarks>Do garbage collection on the discarded memory so memory in never allocated twice.</remarks>
    Friend Sub allocate(ByRef array()(,) As Boolean, ByVal d1 As Integer, ByVal d2 As Integer, ByVal d3 As Integer)
        Dim bCleared As Boolean = True

        If array IsNot Nothing Then
            If array.Length = (d1 + 1) Then

                For i As Integer = 0 To d1
                    If array(i).Length = (d2 + 1) * (d3 + 1) Then
                        System.Array.Clear(array(i), 0, array(i).Length)
                    Else
                        bCleared = False
                        Exit For
                    End If
                Next

                'If we managed to clear the array then Return
                'If NOT the allocate a new array
                If bCleared Then
                    Return
                End If

            End If
        End If

        Erase array
        array = Nothing

        array = New Boolean(d1)(,) {}
        For i As Integer = 0 To d1
            array(i) = New Boolean(d2, d3) {}
        Next
        GC.Collect()

    End Sub



    ''' <summary>
    ''' Allocate memory for an array with 3 dimensions
    ''' </summary>
    ''' <remarks>Do garbage collection on the discarded memory so memory in never allocated twice.</remarks>
    Friend Sub allocate(ByRef array(,,) As Boolean, ByVal d1 As Integer, ByVal d2 As Integer, ByVal d3 As Integer)

        If array IsNot Nothing Then
            If array.Length = (d1 + 1) * (d2 + 1) * (d3 + 1) Then
                System.Array.Clear(array, 0, array.Length)
                Return
            End If
        End If

        Erase array
        array = Nothing

        'GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced)
        'Dim mgs As Single = CSng(d1 * d2 * d3 * 4 / 1048576)
        'System.Console.WriteLine("Allocating=" & mgs.ToString & " Memory=" & (GC.GetTotalMemory(True) / 1048576).ToString)

        GC.Collect()
        ReDim array(d1, d2, d3)

    End Sub


    ''' <summary>
    ''' Allocate memory for an array of singles with 2 dimensions
    ''' </summary>
    ''' <remarks>Do garbage collection on the discarded memory so memory in never allocated twice.</remarks>
    Friend Sub allocate(ByRef array(,) As Single, ByVal d1 As Integer, ByVal d2 As Integer)

        If array IsNot Nothing Then
            If array.Length = (d1 + 1) * (d2 + 1) Then
                System.Array.Clear(array, 0, array.Length)
                Return
            End If
        End If

        Erase array
        array = Nothing
        GC.Collect()

        ReDim array(d1, d2)

    End Sub

    ''' <summary>
    ''' Allocate memory for an array with 2 dimensions
    ''' </summary>
    ''' <remarks>Do garbage collection on the discarded memory so memory in never allocated twice.</remarks>
    Friend Sub allocate(ByRef array(,) As Integer, ByVal d1 As Integer, ByVal d2 As Integer)

        If array IsNot Nothing Then
            If array.Length = (d1 + 1) * (d2 + 1) Then
                System.Array.Clear(array, 0, array.Length)
                Return
            End If
        End If

        'Erase array
        'array = Nothing
        'GC.Collect()

        ReDim array(d1, d2)

    End Sub

    ''' <summary>
    ''' Allocate memory for an array with 3 dimensions
    ''' </summary>
    ''' <remarks>Do garbage collection on the discarded memory so memory in never allocated twice.</remarks>
    Friend Sub allocate(ByRef array(,,) As Integer, ByVal d1 As Integer, ByVal d2 As Integer, d3 As Integer)

        If array IsNot Nothing Then
            If array.Length = (d1 + 1) * (d2 + 1) * (d3 + 1) Then
                System.Array.Clear(array, 0, array.Length)
                Return
            End If
        End If

        Erase array
        array = Nothing

        'GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced)
        'Dim mgs As Single = CSng(d1 * d2 * d3 * 4 / 1048576)
        'System.Console.WriteLine("Allocating=" & mgs.ToString & " Memory=" & (GC.GetTotalMemory(True) / 1048576).ToString)

        GC.Collect()
        ReDim array(d1, d2, d3)

    End Sub

    ''' <summary>
    ''' Allocate memory for an array of boolean values with 2 dimensions
    ''' </summary>
    ''' <remarks>Do garbage collection on the discarded memory so memory in never allocated twice.</remarks>
    Friend Sub allocate(ByRef array(,) As Boolean, ByVal d1 As Integer, ByVal d2 As Integer)

        If array IsNot Nothing Then
            If array.Length = (d1 + 1) * (d2 + 1) Then
                System.Array.Clear(array, 0, array.Length)
                Return
            End If
        End If

        'Erase array
        'array = Nothing
        'GC.Collect()

        ReDim array(d1, d2)

    End Sub

    ' == JS added to move overlapping MPA logic to sparse arrays ==

    ''' <summary>
    ''' Allocate memory for an array with 3 dimensions
    ''' </summary>
    ''' <remarks>Do garbage collection on the discarded memory so memory in never allocated twice.</remarks>
    Friend Sub allocate(ByRef array()(,) As Integer, ByVal d1 As Integer, ByVal d2 As Integer, ByVal d3 As Integer)
        Dim bCleared As Boolean = True

        If array IsNot Nothing Then
            If array.Length = (d1 + 1) Then

                For i As Integer = 0 To d1
                    If array(i).Length = (d2 + 1) * (d3 + 1) Then
                        System.Array.Clear(array(i), 0, array(i).Length)
                    Else
                        bCleared = False
                        Exit For
                    End If
                Next

                'If we managed to clear the array then Return
                'If NOT the allocate a new array
                If bCleared Then
                    Return
                End If

            End If
        End If

        Erase array
        array = Nothing

        array = New Integer(d1)(,) {}
        For i As Integer = 0 To d1
            array(i) = New Integer(d2, d3) {}
        Next
        GC.Collect()

    End Sub

    Public Sub ReDimMapDims()
        'NvarTot = nvar + 2 * npairs
        Dim i As Integer, j As Integer, k As Integer

        Debug.Assert(Me.StanzaGroups IsNot Nothing, Me.ToString & ".ReDimMapDims() Stanzagroups needs to be set.")

        Try

            'jb this is also set in ReDimMapVars()
            Me.Nvarsplit = 0
            For i = 1 To Me.StanzaGroups.Nsplit
                For j = 1 To Me.StanzaGroups.Nstanza(i)
                    Me.Nvarsplit = Me.Nvarsplit + 1
                Next
            Next
            Me.nvartot = Me.NGroups + Me.Nvarsplit

            'force the garbage collection
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced)

            Me.allocate(Me.Bcell, Me.InRow + 1, Me.InCol + 1, Me.nvartot)
            Me.allocate(Me.Blast, Me.InRow + 1, Me.InCol + 1, Me.nvartot)

            Me.allocate(Me.CatchMap, Me.InRow, Me.InCol, Me.NGroups)
            Me.allocate(Me.DiscardsMap, Me.InRow, Me.InCol, Me.NGroups)
            Me.allocate(Me.CatchFleetMap, Me.InRow, Me.InCol, Me.nFleets)

            'For Nereus EcoOcean there are more fleets than groups
            'so dimension the fleets first
            Me.allocate(Me.Port, Me.nFleets, Me.InRow, Me.InCol)
            Me.allocate(Me.PAreaFished, Me.nFleets, Me.InRow, Me.InCol)
            Me.allocate(Me.Sail, Me.nFleets, Me.InRow + 1, Me.InCol + 1)

            'MOLoss
            Me.allocate(Me.MOLoss, Me.NGroups, Me.InRow, Me.InCol)

            Me.allocate(Me.HabCapInput, Me.NGroups, Me.InRow + 1, Me.InCol + 1)
            For i = 1 To Me.InRow : For j = 1 To Me.InCol : For k = 1 To Me.NGroups : Me.HabCapInput(k)(i, j) = 1 : Next : Next : Next
            Me.allocate(Me.HabCap, Me.NGroups, Me.InRow + 1, Me.InCol + 1)

            Me.allocate(Me.MOProp, Me.NGroups, Me.InRow + 1, Me.InCol + 1)

            Me.allocate(Me.PHabType, Me.NoHabitats, Me.InRow, Me.InCol)

            Me.allocate(Me.MonthlyXwind, cCore.N_MONTHS, Me.InRow + 1, Me.InCol + 1)
            Me.allocate(Me.MonthlyYwind, cCore.N_MONTHS, Me.InRow + 1, Me.InCol + 1)
            '  For i = 1 To InRow : For j = 1 To InCol : For k = 1 To cCore.N_MONTHS : Xv(i, j, k) = 1 : Yv(i, j, k) = 1 : Next : Next : Next

            Me.allocate(Me.DepthInput, Me.InRow + 1, Me.InCol + 1)
            'Resized basemap should have water everywhere
            Me.DepthInput.Fill(1)
            Me.allocate(Me.Excluded, Me.InRow + 1, Me.InCol + 1)
            Me.allocate(Me.CellArea, Me.InRow + 1, Me.InCol + 1)
            ' Cell area default 1
            Me.CellArea.Fill(1)

            Me.allocate(Me.Depth, Me.InRow + 1, Me.InCol + 1)
            Me.allocate(Me.DepthA, Me.InRow + 1, Me.InCol + 1)
            Me.allocate(Me.DepthX, Me.InRow + 1, Me.InCol + 1)
            Me.allocate(Me.DepthY, Me.InRow + 1, Me.InCol + 1)
            Me.allocate(Me.Xvel, Me.InRow + 1, Me.InCol + 1)
            Me.allocate(Me.Yvel, Me.InRow + 1, Me.InCol + 1)
            Me.allocate(Me.Xvloc, Me.InRow + 1, Me.InCol + 1)
            Me.allocate(Me.Yvloc, Me.InRow + 1, Me.InCol + 1)
            Me.allocate(Me.UpVel, Me.InRow + 1, Me.InCol + 1)
            Me.allocate(Me.flow, Me.InRow + 1, Me.InCol + 1)

            Me.allocate(Me.Region, Me.InRow + 1, Me.InCol + 1)
            Me.allocate(Me.RelPP, Me.InRow + 1, Me.InCol + 1)
            Me.allocate(Me.RelCin, Me.InRow + 1, Me.InCol + 1)

            Me.allocate(Me.RelNutMult, Me.InRow, Me.InCol)

            ' JS 14May16: Only allocate this temporary array when a relPP backup is made
            'Me.allocate(relPP0, InRow + 1, InCol + 1)
            Me.RelPP0 = Nothing

            Me.allocate(Me.TL, Me.InRow, Me.InCol, Me.NGroups)
            Me.allocate(Me.TLc, Me.InRow, Me.InCol)
            Me.allocate(Me.KemptonsQ, Me.InRow, Me.InCol)
            Me.allocate(Me.ShannonDiversity, Me.InRow, Me.InCol)

            Me.allocate(Me.ImportanceLayerMap, Me.nImportanceLayers, Me.InRow + 1, Me.InCol + 1)
            Me.allocate(Me.EnvironmentalLayerMap, Me.nEnvironmentalDriverLayers, Me.InRow + 1, Me.InCol + 1)

            ReDim Me.MPAfishery(Me.nFleets, 1)
            ReDim Me.MPAmonth(12, 1)
            ReDim Me.IsFished(Me.nFleets, Me.InRow, Me.InCol)
            ReDim Me.EffZones(Me.InRow, Me.InCol)
            ReDim Me.RelativeCellWidth(Me.InRow)

            Me.allocate(Me.MonthlyXvel, 12, Me.InRow + 1, Me.InCol + 1)
            Me.allocate(Me.MonthlyYvel, 12, Me.InRow + 1, Me.InCol + 1)
            Me.allocate(Me.MonthlyUpWell, 12, Me.InRow + 1, Me.InCol + 1)

            'jb move this here to set a few defaults this will have to change
            For i = 1 To Me.NGroups                            'CJW had nvar not n1
                Me.PrefHab(i, 0) = 1.0! ' True
            Next 'set preferred habitat to 1 (pelagic) by default

            Me.ReDimEffortZones()

            'Calculate the relative cell widths due to latitude tapering, if applicable
            Me.CalculateRelCellWidths()

            For i = 1 To Me.InRow
                For j = 1 To Me.InCol      'Default Values for new maps
                    Me.Depth(i, j) = 1
                    Me.DepthA(i, j) = Me.Depth(i, j)
                    ' HabType(i, j) = 1
                    Me.RelPP(i, j) = 1
                    Me.RelCin(i, j) = 1
                    For k = 1 To Me.nFleets
                        Me.Sail(k)(i, j) = 1
                    Next

                    'Use all habitats
                    Me.PHabType(0)(i, j) = 1.0F

                    'Default Areas=1
                    Me.EffZones(i, j) = 1

                Next j
            Next i

            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced)

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".ReDimMapDims() Error: " & ex.Message)
            Throw New System.Exception(Me.ToString & ".ReDimMapDims() Error: " & ex.Message)
        End Try

    End Sub

    '''' <summary>
    '''' This method assumes nautical miles and gave wonky results.
    '''' JS 04Mar25 replaced this method by CalculateRelCellWidths.
    '''' </summary>
    'Friend Sub CalculateCellWidth()

    '    Dim halfcell As Single = Me.CellLength / 2 / (60 * 1.852F)
    '    Dim dtLat As Single
    '    For i As Integer = 1 To Me.InRow

    '        dtLat = Me.CellLength * (i - 1) / (60 * 1.852F) - halfcell
    '        'System.Console.WriteLine((Lat1 - dtLat).ToString + ", ")

    '        'jb 28-Nov-2013 find width for the center of the cell
    '        If (Me.AssumeSquareCells) Then
    '            Me.RelativeCellWidth(i) = 1
    '        Else
    '            'half a cell height in degrees 
    '            Dim Lat As Single = Me.Lat1 - dtLat
    '            Me.RelativeCellWidth(i) = CSng(Math.Cos(Lat / 90.0 * Math.PI / 2.0))
    '        End If

    '    Next i

    'End Sub

    ''' <summary>
    ''' Simplified algorithm to calculate the relative cell widths in
    ''' the assumed WGS84 projection. This code assumes that the model 
    ''' uses km
    ''' </summary>
    Friend Sub CalculateRelCellWidths()

        ' Approximate km per degree of latitude
        Const KM_PER_DEGREE As Single = 111.12F
        'Const MN_PER_DEGREE As Single = KM_PER_DEGREE / 1.852F

        ' Approximate cell size in decimal degree
        Dim CellSizeDD As Single = Me.CellLength / KM_PER_DEGREE
        ' Starting latitude cell centroid
        Dim toplatCtr As Single = Me.Lat1 - CellSizeDD / 2

        For i As Integer = 1 To Me.InRow

            If (Me.AssumeSquareCells) Then
                Me.RelativeCellWidth(i) = 1
            Else
                ' Calculate latitude of the cell center
                Dim Lat As Single = toplatCtr - (i - 1) * CellSizeDD
                ' Convert latitude to radians
                Dim latRad As Double = Lat * (Math.PI / 180.0)
                ' Adjust width using the cosine of latitude
                Me.RelativeCellWidth(i) = CSng(Math.Abs(Math.Cos(latRad)))
            End If
        Next i

        If (Me.AssumeSquareCells) Then
            Me.IsGlobalMap = False
        Else
            Me.IsGlobalMap = ((Me.InCol * CellSizeDD) > 359)
        End If

    End Sub




    Public Sub RedimConSimVars()

        'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
        'HACK Warning
        'Contaminant Tracing arrays are only initialized if Contaminant Tracing is turned on 
        'and get re-allocated  for each run.
        'This causes a problem for the Spatial Temporal forcing 
        'because it uses the Ecospace Basemap Layers which only get initialized once with a reference to the current data in memory
        'which will be different for each run
        If Me.Ccell Is Nothing Then
            'not allocated yet so create it
            ReDim Me.Ccell(Me.InRow + 1, Me.InCol + 1, Me.NGroups)
        End If

        'check the size incase this is a new model/basemap
        Dim size As Integer = (Me.InRow + 2) * (Me.InCol + 2) * (Me.NGroups + 1)
        If Me.Ccell.Length <> size Then
            ReDim Me.Ccell(Me.InRow + 1, Me.InCol + 1, Me.NGroups)
        End If
        'Clear out any old data
        Array.Clear(Me.Ccell, 0, size)
        'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

        ReDim Me.Clast(Me.InRow + 1, Me.InCol + 1, Me.NGroups)
        ReDim Me.AMmTr(Me.InRow + 1, Me.InCol + 1, Me.NGroups)
        ReDim Me.Ftr(Me.InRow + 1, Me.InCol + 1, Me.NGroups)

    End Sub

    Public Sub RedimGroups()
        Try
            ReDim Me.GroupDBID(Me.m_ngroups)
            ReDim Me.EcopathGroupDBID(Me.m_ngroups)
            ReDim Me.CapCalType(Me.m_ngroups)

            ReDim Me.IsEcosimBioForcingGroup(Me.m_ngroups)
            ReDim Me.IsEcosimDiscardForcingGroup(Me.m_ngroups)
            ReDim Me.Kmovefit(Me.m_ngroups)

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".redimGroupDBID() Error: " & ex.Message)
        End Try
    End Sub

    Friend Sub RedimRegionAdminForRun()

        ReDim Me.RegionArea(Me.nRegions)
        ReDim Me.RegionCells(Me.nRegions)
        For iRow As Integer = 1 To Me.InRow
            For iCol As Integer = 1 To Me.InCol
                If (Me.Depth(iRow, iCol) > 0) Then
                    Dim iReg As Integer = Me.Region(iRow, iCol)
                    If (iReg > 0 And iReg <= Me.nRegions) Then
                        Me.RegionArea(iReg) += Me.CellArea(iRow, iCol)
                        Me.RegionCells(iReg) += 1
                    End If
                    ' Total water area
                    Me.RegionArea(0) += Me.CellArea(iRow, iCol)
                    Me.RegionCells(0) += 1
                End If
            Next
        Next

    End Sub

    ''' <summary>
    ''' Redim the data that saves the Ecospace results over time
    ''' </summary>
    ''' <remarks>This must be called by Ecospace at the start of a run to clear out any existing data.</remarks>
    Public Function redimTimeStepResults(ByVal NumberOfTimeSteps As Integer) As Boolean

        'Debug.Assert(TimeStep > 0 And TotalTime > 0)
        Dim success As Boolean = True

        'reset the number of time steps the model ran for
        'nSumTimeSteps = 0
        Dim message As cMessage

        Try

            Me.allocate(Me.ResultsByGroup, [Enum].GetValues(GetType(eSpaceResultsGroups)).Length, Me.NGroups, NumberOfTimeSteps)
            Me.allocate(Me.ResultsByFleet, [Enum].GetValues(GetType(eSpaceResultsFleets)).Length, Me.nFleets, NumberOfTimeSteps)
            Me.allocate(Me.ResultsByFleetGroup, [Enum].GetValues(GetType(eSpaceResultsFleetsGroups)).Length, Me.nFleets, Me.NGroups, NumberOfTimeSteps)
            Me.allocate(Me.ResultsRegionConsumptionPredPrey, Me.nRegions, Me.NGroups, Me.NGroups, NumberOfTimeSteps)

            Me.allocate(Me.ResultsRegionGroup, Me.nRegions, Me.NGroups, NumberOfTimeSteps)
            Me.allocate(Me.ResultsRegionGroupYear, Me.nRegions, Me.NGroups, CInt(NumberOfTimeSteps / Math.Max(Me.NumStep, 1) + 1))
            Me.allocate(Me.ResultsCatchRegionGearGroup, Me.nRegions, Me.nFleets, Me.NGroups, NumberOfTimeSteps)
            Me.allocate(Me.ResultsCatchRegionGearGroupYear, Me.nRegions, Me.nFleets, Me.NGroups, CInt(NumberOfTimeSteps / Math.Max(Me.NumStep, 1) + 1))
            Me.allocate(Me.ResultsLandingsRegionGearGroup, Me.nRegions, Me.nFleets, Me.NGroups, NumberOfTimeSteps)
            Me.allocate(Me.ResultsValueRegionGearGroup, Me.nRegions, Me.nFleets, Me.NGroups, NumberOfTimeSteps)

        Catch exmem As OutOfMemoryException
            System.Console.WriteLine(Me.ToString & ".redimTimeStepResults() Out of memory: " & exmem.Message)
            message = New cMessage(My.Resources.CoreMessages.ECOSPACE_OUT_OF_MEMORY,
                                   eMessageType.Any, EwEUtils.Core.eCoreComponentType.Ecospace, eMessageImportance.Critical)
        Catch ex As Exception
            System.Console.WriteLine(Me.ToString & ".redimTimeStepResults(): " & ex.Message)
            message = New cMessage(ex.Message, eMessageType.Any, EwEUtils.Core.eCoreComponentType.Ecospace, eMessageImportance.Critical)
        End Try

        If message IsNot Nothing Then
            Me.m_publisher.AddMessage(message)
            success = False
        End If

        Return success

    End Function


    Public Sub setDefaultSummaryPeriod()
        Try
            Debug.Assert(Me.TimeStep > 0)
            'set the summary data to be over the total time
            Me.SumStart(0) = 0 'start of first summary period
            Me.SumStart(1) = Me.TotalTime - 1 'start of last summary perion
            Me.NumStep = Math.Max(1, CInt(1.0 / Me.TimeStep)) 'number of time steps to summarize over one year for the default summary
        Catch ex As Exception
            Me.SumStart(0) = 0 'start of first summary period
            Me.SumStart(1) = Me.TotalTime - 1 'start of last summary period
            Me.NumStep = 1 'number of time steps to summarize over one year for the default summary
            Debug.Assert(False)
        End Try
    End Sub

    ''' <summary>
    ''' Ecospace spatial reference data not used but left in place for legacy reasons 
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub redimForReferenceData()

        Debug.Assert(False, "Ecospace spatial reference data has not been implemented yet!")

        Dim ttYears As Integer = CInt(Me.TotalTime)
        ReDim Me.SpaceBiomassByRegion(ttYears, Me.NGroups, Me.nRegions)
        ReDim Me.SpaceBiomassByRegionCount(ttYears, Me.NGroups, Me.nRegions)
        ReDim Me.SpaceCatchByRegion(ttYears, Me.NGroups, Me.nRegions)
        ReDim Me.SpaceCatchByRegionCount(ttYears, Me.NGroups, Me.nRegions)
        ReDim Me.SpaceEffortByRegionFleet(ttYears, Me.nFleets, Me.nRegions)
        ReDim Me.SpaceEffortByRegionFleetCount(ttYears, Me.nFleets, Me.nRegions)

    End Sub


    ''' <summary>
    ''' Get sum of Biomass by Region Group for the Start and End summary period
    ''' </summary>
    ''' <remarks>Summary time windows are defined by the user</remarks>
    Public Sub getSumBiomByRegion(ByVal iRegion As Integer, ByVal iGroup As Integer, ByRef startBio As Single, ByRef endBio As Single)
        Dim st As Integer, et As Integer, nts As Integer
        startBio = 0
        endBio = 0

        'get the start and end time indexes and number of time steps to sum over
        'getStartEndSumIndex() will figure out the one based indexes
        Me.getStartEndSumIndex(st, et, nts)

        For it As Integer = st To st + nts - 1
            startBio = startBio + Me.ResultsRegionGroup(iRegion, iGroup, it)
        Next
        startBio = startBio / nts

        For it As Integer = et To et + nts - 1
            endBio = endBio + Me.ResultsRegionGroup(iRegion, iGroup, it)
        Next
        endBio = endBio / nts

    End Sub
    ''' <summary>
    ''' Get Biomass for summary periods
    ''' </summary>
    Public Sub getSumBiom(ByVal iGroup As Integer, ByRef startBio As Single, ByRef endBio As Single)
        Dim st As Integer, et As Integer, nts As Integer
        startBio = 0
        endBio = 0

        'get the start and end time indexes and number of time steps to sum over
        'getStartEndSumIndex() will figure out the one based indexes
        Me.getStartEndSumIndex(st, et, nts)

        For it As Integer = st To st + nts - 1
            startBio = startBio + Me.ResultsByGroup(eSpaceResultsGroups.Biomass, iGroup, it)
        Next
        startBio = startBio / nts

        For it As Integer = et To et + nts - 1
            endBio = endBio + Me.ResultsByGroup(eSpaceResultsGroups.Biomass, iGroup, it)
        Next
        endBio = endBio / nts

    End Sub

    ''' <summary>
    ''' Get Catch by Fleet Group for summary periods
    ''' </summary>
    Public Sub getSumCatchFleetGroup(ByVal iFleet As Integer, ByVal iGroup As Integer, ByRef startCatch As Single, ByRef endCatch As Single)
        Dim st As Integer, et As Integer, nts As Integer
        startCatch = 0
        endCatch = 0

        'get the start and end time indexes and number of time steps to sum over
        'getStartEndSumIndex() will figure out the one based indexes
        Me.getStartEndSumIndex(st, et, nts)

        For it As Integer = st To st + nts - 1
            startCatch = startCatch + Me.ResultsByFleetGroup(eSpaceResultsFleetsGroups.CatchBio, iFleet, iGroup, it)
        Next
        startCatch = startCatch / nts

        For it As Integer = et To et + nts - 1
            endCatch = endCatch + Me.ResultsByFleetGroup(eSpaceResultsFleetsGroups.CatchBio, iFleet, iGroup, it)
        Next
        endCatch = endCatch / nts

    End Sub

    ''' <summary>
    ''' Get Value by Fleet Group for summary periods
    ''' </summary>
    Public Sub getSumValueFleetGroup(ByVal iFleet As Integer, ByVal iGroup As Integer, ByRef startCatch As Single, ByRef endCatch As Single)
        Dim st As Integer, et As Integer, nts As Integer
        startCatch = 0
        endCatch = 0

        'get the start and end time indexes and number of time steps to sum over
        'getStartEndSumIndex() will figure out the one based indexes
        Me.getStartEndSumIndex(st, et, nts)

        For it As Integer = st To st + nts - 1
            startCatch = startCatch + Me.ResultsByFleetGroup(eSpaceResultsFleetsGroups.Value, iFleet, iGroup, it)
        Next
        startCatch = startCatch / nts

        For it As Integer = et To et + nts - 1
            endCatch = endCatch + Me.ResultsByFleetGroup(eSpaceResultsFleetsGroups.Value, iFleet, iGroup, it)
        Next
        endCatch = endCatch / nts

    End Sub

    ''' <summary>
    ''' Get Catch by Fleet for summary periods
    ''' </summary>
    Public Sub getSumCatchFleet(ByVal iFleet As Integer, ByRef startCatch As Single, ByRef endCatch As Single)
        Dim st As Integer, et As Integer, nts As Integer
        startCatch = 0
        endCatch = 0

        'get the start and end time indexes and number of time steps to sum over
        'getStartEndSumIndex() will figure out the one based indexes
        Me.getStartEndSumIndex(st, et, nts)

        For it As Integer = st To st + nts - 1
            startCatch = startCatch + Me.ResultsByFleet(eSpaceResultsFleets.CatchBio, iFleet, it)
        Next
        startCatch = startCatch / nts

        For it As Integer = et To et + nts - 1
            endCatch = endCatch + Me.ResultsByFleet(eSpaceResultsFleets.CatchBio, iFleet, it)
        Next
        endCatch = endCatch / nts

    End Sub


    ''' <summary>
    ''' Get Cost by Fleet for summary periods
    ''' </summary>
    ''' <param name="EcopathCost">Cost from Ecopath actual cost in Ecopath dollars for one unit of Ecopath fishing</param>
    ''' <remarks>Cost is computed from values saved over time because of the was it's calculated</remarks>
    Public Sub getSumCostFleet(ByVal EcopathCost(,) As Single, ByVal iFleet As Integer, ByRef startCost As Single, ByRef endCost As Single)
        Dim st As Integer, et As Integer, nts As Integer
        Dim sSailEffort As Single, eSailEffort As Single
        Dim sFishEffort As Single, eFishEffort As Single
        startCost = 0
        endCost = 0

        'get the start and end time indexes and number of time steps to sum over
        'getStartEndSumIndex() will figure out the one based indexes
        Me.getStartEndSumIndex(st, et, nts)

        'eSpaceResultsFleets.SailingEffort and FishingEffort are spatially averaged cEcospace.accumCatchData() and me.AverageSpatialResults()
        For it As Integer = st To st + nts - 1
            sSailEffort += Me.ResultsByFleet(eSpaceResultsFleets.SailingEffort, iFleet, it)
            sFishEffort += Me.ResultsByFleet(eSpaceResultsFleets.FishingEffort, iFleet, it)
        Next
        'in EwE5 Effort is averaged over time steps
        'sailing effort is not
        sFishEffort = sFishEffort / nts

        For it As Integer = et To et + nts - 1
            eSailEffort += Me.ResultsByFleet(eSpaceResultsFleets.SailingEffort, iFleet, it)
            eFishEffort += Me.ResultsByFleet(eSpaceResultsFleets.FishingEffort, iFleet, it)
        Next
        eFishEffort = eFishEffort / nts

        'cost = [fixed cost] + ([fishing effort] * [ecopath effort cost] + [sailing effort] * [ecopath sailing cost])
        startCost = EcopathCost(iFleet, 1) + (sFishEffort * EcopathCost(iFleet, 2) + sSailEffort * EcopathCost(iFleet, 3))
        endCost = EcopathCost(iFleet, 1) + (eFishEffort * EcopathCost(iFleet, 2) + eSailEffort * EcopathCost(iFleet, 3))

        'Console.WriteLine("Effort Fleet = " & iFleet.ToString & ", Start = " & sFishEffort.ToString & ", End = " & eFishEffort.ToString)
        'Console.WriteLine("Sail Fleet = " & iFleet.ToString & ", Start = " & sSailEffort.ToString & ", End = " & eSailEffort.ToString)

    End Sub



    ''' <summary>
    ''' Get Value by Fleet for summary periods
    ''' </summary>
    Public Sub getSumValueFleet(ByVal iFleet As Integer, ByRef startValue As Single, ByRef endValue As Single)
        Dim st As Integer, et As Integer, nts As Integer
        startValue = 0
        endValue = 0

        'get the start and end time indexes and number of time steps to sum over
        'getStartEndSumIndex() will figure out the one based indexes
        Me.getStartEndSumIndex(st, et, nts)

        For it As Integer = st To st + nts - 1
            startValue = startValue + Me.ResultsByFleet(eSpaceResultsFleets.Value, iFleet, it)
        Next
        startValue = startValue / nts

        For it As Integer = et To et + nts - 1
            endValue = endValue + Me.ResultsByFleet(eSpaceResultsFleets.Value, iFleet, it)
        Next
        endValue = endValue / nts

    End Sub


    ''' <summary>
    ''' Get Value by Fleet for summary periods
    ''' </summary>
    Public Sub getSumEffortES(ByVal iFleet As Integer, ByRef EndoverStart As Single)
        Dim st As Integer, et As Integer, nts As Integer
        Dim s As Single, e As Single
        'get the start and end time indexes and number of time steps to sum over
        'getStartEndSumIndex() will figure out the one based indexes
        Me.getStartEndSumIndex(st, et, nts)

        For it As Integer = st To st + nts - 1
            s = s + Me.ResultsByFleet(eSpaceResultsFleets.FishingEffort, iFleet, it)
        Next
        s = s / nts

        For it As Integer = et To et + nts - 1
            e = e + Me.ResultsByFleet(eSpaceResultsFleets.FishingEffort, iFleet, it)
        Next
        e = e / nts

        If s = 0 Then s = 1
        EndoverStart = e / s

    End Sub


    ''' <summary>
    ''' Get Catch by REgion, Fleet, Group for summary periods
    ''' </summary>
    Public Sub getSumCatchRegionGearGroup(ByVal iRegion As Integer, ByVal iFleet As Integer, ByVal iGroup As Integer, ByRef startCatch As Single, ByRef endCatch As Single)
        Dim st As Integer, et As Integer, nts As Integer
        startCatch = 0
        endCatch = 0

        'get the start and end time indexes and number of time steps to sum over
        'getStartEndSumIndex() will figure out the one based indexes
        Me.getStartEndSumIndex(st, et, nts)

        For it As Integer = st To st + nts - 1
            startCatch = startCatch + Me.ResultsCatchRegionGearGroup(iRegion, iFleet, iGroup, it)
        Next
        startCatch = startCatch / nts

        For it As Integer = et To et + nts - 1
            endCatch = endCatch + Me.ResultsCatchRegionGearGroup(iRegion, iFleet, iGroup, it)
        Next
        endCatch = endCatch / nts

    End Sub


    ''' <summary>
    ''' Average the results values over number of water cells
    ''' </summary>
    Public Sub AverageSpatialResults()
        Dim iflt As Integer, igrp As Integer, it As Integer, ivar As Integer, irgn As Integer
        Dim ncells As Integer
        Try

            For ivar = 0 To [Enum].GetValues(GetType(eSpaceResultsFleets)).Length
                For iflt = 0 To Me.nFleets
                    For it = 1 To Me.nTimeSteps
                        Me.ResultsByFleet(ivar, iflt, it) /= Me.RegionCells(0)
                    Next it
                Next iflt
            Next ivar

            For ivar = 0 To [Enum].GetValues(GetType(eSpaceResultsFleetsGroups)).Length
                For iflt = 0 To Me.nFleets
                    For igrp = 1 To Me.NGroups
                        For it = 1 To Me.nTimeSteps
                            Me.ResultsByFleetGroup(ivar, iflt, igrp, it) /= Me.RegionCells(0)
                        Next it
                    Next igrp
                Next iflt
            Next ivar

            For irgn = 0 To Me.nRegions
                ncells = Me.RegionCells(irgn)
                If ncells = 0 Then ncells = 1
                For igrp = 1 To Me.NGroups
                    For it = 1 To Me.nTimeSteps
                        Me.ResultsRegionGroup(irgn, igrp, it) /= ncells
                    Next it
                Next igrp
            Next irgn

            For irgn = 0 To Me.nRegions
                ncells = Me.RegionCells(irgn)
                If ncells = 0 Then ncells = 1
                For iflt = 0 To Me.nFleets
                    For igrp = 1 To Me.NGroups
                        For it = 1 To Me.nTimeSteps
                            Me.ResultsCatchRegionGearGroup(irgn, iflt, igrp, it) /= ncells
                            Me.ResultsLandingsRegionGearGroup(irgn, iflt, igrp, it) /= ncells
                            Me.ResultsValueRegionGearGroup(irgn, iflt, igrp, it) /= ncells
                        Next it
                    Next igrp
                Next iflt
            Next irgn

        Catch ex As Exception
            Debug.Assert(False, ex.Message)
            cLog.Write(ex)
        End Try

    End Sub


    Public Sub SummarizeResultsByFleet(nTimeSteps As Integer, ByVal EcopathCost(,) As Single, ByVal JobMultiplier() As Single)
        Dim SailEffort As Single, FishEffort As Single
        Dim cost As Single, value As Single

        'Me.nSumTimeSteps = 0
        Debug.Assert(nTimeSteps <= Me.ResultsByFleet.GetUpperBound(2), "EcoSpace summary data time step counter not set correctly!")

        'number of years the model actually ran for, computed in case the model run was stopped by the user
        Dim nYears As Single = CSng(nTimeSteps / (1 / Me.TimeStep))

        ReDim Me.ResultsSummaryByFleet(1, Me.nFleets)

        'All values in ResultsByFleet() have been averaged over space
        For iflt As Integer = 0 To Me.nFleets
            SailEffort = 0
            FishEffort = 0
            value = 0
            For it As Integer = 1 To nTimeSteps
                SailEffort += Me.ResultsByFleet(eSpaceResultsFleets.SailingEffort, iflt, it)
                FishEffort += Me.ResultsByFleet(eSpaceResultsFleets.FishingEffort, iflt, it)
                value += Me.ResultsByFleet(eSpaceResultsFleets.Value, iflt, it)
            Next

            cost = EcopathCost(iflt, 1) + (FishEffort * EcopathCost(iflt, 2) + SailEffort * EcopathCost(iflt, 3))

            'profit average yearly
            Me.ResultsSummaryByFleet(0, iflt) = (value - cost) / nYears
            'jobs average yearly
            Me.ResultsSummaryByFleet(1, iflt) = value * JobMultiplier(iflt) / nYears

        Next

    End Sub


    ''' <summary>
    ''' Get the indexes for the user defined time windows that the results data is summarized over
    ''' </summary>
    ''' <param name="startIndex">Index for the first time window</param>
    ''' <param name="endIndex">Index for the end/last time window</param>
    ''' <param name="nIndexes">Number of time steps the user defined to summarize the data over</param>
    ''' <remarks></remarks>
    Private Sub getStartEndSumIndex(ByRef startIndex As Integer, ByRef endIndex As Integer, ByRef nIndexes As Integer)

        Dim nSteps As Integer = CInt(1.0 / Me.TimeStep)
        startIndex = CInt(Me.SumStart(0) * nSteps) + 1
        endIndex = CInt(Me.SumStart(1) * nSteps) + 1
        If startIndex > Me.nTimeSteps Then startIndex = 1
        If endIndex > Me.nTimeSteps Then endIndex = Me.nTimeSteps - Me.NumStep
        nIndexes = Me.NumStep
    End Sub

    ''' <summary>
    ''' Preserve RelPP map in the <see cref="relPP0"/> temporary array.
    ''' </summary>
    Public Sub setBaseRelPP()
        Me.allocate(Me.RelPP0, Me.InRow + 1, Me.InCol + 1)
        Array.Copy(Me.RelPP, Me.RelPP0, Me.RelPP.Length)
    End Sub

    ''' <summary>
    ''' Restore RelPP map from the <see cref="relPP0"/> temporary array.
    ''' </summary>
    ''' <remarks>
    ''' This will clear the relPP0 temporary array.
    ''' </remarks>
    Public Sub restoreBaseRelPP()
        If (Me.RelPP0 IsNot Nothing) Then
            Array.Copy(Me.RelPP0, Me.RelPP, Me.RelPP.Length)
            Me.RelPP0 = Nothing
        End If
    End Sub

    ''' <summary>
    ''' Hardwire some Capacity map values
    ''' </summary>
    ''' <remarks>FOR DEBUGGING ONLY</remarks>
    Public Sub setDebugCapMaps(ByVal CapEnvResData As cMediationDataStructures)

        Try
            ''set PHabType(,,) to 100% for cells that are loaded as a HabType from the database 
            'For irow As Integer = 1 To InRow
            '    For icol As Integer = 1 To InCol
            '        PHabType(irow, icol, HabType(irow, icol)) = 1
            '    Next icol
            'Next irow

            ''set Input habitat capacity to 1 for all groups 
            'For irow As Integer = 1 To InRow
            '    For icol As Integer = 1 To InCol
            '        For igrp As Integer = 1 To Me.NGroups
            '            Me.HabCapInput(irow, icol, igrp) = 1
            '        Next
            '    Next icol
            'Next irow

        Catch ex As Exception
            Debug.Assert(False, "Failed to init debug capacity map")
        End Try


    End Sub

    ''' <summary>
    ''' Count the number of water cells and sets public property nWaterCells
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function setNWaterCells() As Integer
        Me.nWaterCells = 0
        For i As Integer = 1 To Me.InRow
            For j As Integer = 1 To Me.InCol
                If Me.Depth(i, j) > 0 Then 'Water
                    Me.nWaterCells += 1
                End If
            Next
        Next

    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Return the <see cref="cCoreInputOutputBase.DBID">unique database ID</see>
    ''' for any Ecospace map layer.
    ''' </summary>
    ''' <param name="varname">The <see cref="eVarNameFlags"/> of the layer to find the database ID for.</param>
    ''' <param name="iIndex">The <see cref="cCoreInputOutputBase.Index"/> of the layer to find the database ID for.</param>
    ''' <returns>An integer, or <see cref="cCore.NULL_VALUE"/> if the requested
    ''' layer was not found.</returns>
    ''' <remarks>
    ''' This method is robust to any type of abuse; non-registered <paramref name="varname">variables</paramref>
    ''' and <paramref name="iIndex">indexes</paramref> are dealt with properly.
    ''' </remarks>
    ''' -------------------------------------------------------------------
    Public Function GetLayerID(varname As eVarNameFlags, iIndex As Integer) As Integer
        Dim arr As Integer() = Me.GetLayerIDs(varname)
        If ((iIndex < 0) Or (iIndex >= arr.Length)) Then Return cCore.NULL_VALUE
        Return arr(iIndex)
    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Obtain a layer DBID for any varname and index.
    ''' </summary>
    ''' <param name="varname"></param>
    ''' <remarks>
    ''' This method is robust to any type of abuse; non-registered <paramref name="varname">variables</paramref>
    ''' are dealt with properly.
    ''' </remarks>
    ''' -------------------------------------------------------------------
    Public ReadOnly Property GetLayerIDs(varname As eVarNameFlags) As Integer()
        Get
            Select Case varname
                Case eVarNameFlags.LayerBiomassForcing : Return Me.GroupDBID
                Case eVarNameFlags.LayerBiomassRelativeForcing : Return Me.GroupDBID
                Case eVarNameFlags.LayerDriver : Return Me.EnvironmentalLayerDBID
                Case eVarNameFlags.LayerHabitat : Return Me.HabitatDBID
                Case eVarNameFlags.LayerHabitatCapacity : Return Me.GroupDBID
                Case eVarNameFlags.LayerHabitatCapacityInput : Return Me.GroupDBID
                Case eVarNameFlags.LayerImportance : Return Me.ImportanceLayerDBID
                Case eVarNameFlags.LayerMigration : Return Me.GroupDBID
                Case eVarNameFlags.LayerMPA : Return Me.MPADBID
                Case eVarNameFlags.LayerPort : Return Me.FleetDBID
                Case eVarNameFlags.LayerSail : Return Me.FleetDBID
                Case eVarNameFlags.LayerAdvection : Return New Integer() {0, 1, 2}
            End Select
            Return New Integer() {0, 1}
        End Get
    End Property

    Public ReadOnly Property GetLayerDataType(varname As eVarNameFlags) As eDataTypes
        Get
            Select Case varname
                Case eVarNameFlags.LayerBiomassForcing : Return eDataTypes.EcospaceGroup
                Case eVarNameFlags.LayerBiomassRelativeForcing : Return eDataTypes.EcospaceGroup
                Case eVarNameFlags.LayerDriver : Return eDataTypes.EcospaceLayerDriver
                Case eVarNameFlags.LayerHabitat : Return eDataTypes.EcospaceHabitat
                Case eVarNameFlags.LayerHabitatCapacity : Return eDataTypes.EcospaceGroup
                Case eVarNameFlags.LayerHabitatCapacityInput : Return eDataTypes.EcospaceGroup
                Case eVarNameFlags.LayerImportance : Return eDataTypes.EcospaceLayerImportance
                Case eVarNameFlags.LayerMigration : Return eDataTypes.EcospaceGroup
                Case eVarNameFlags.LayerMPA : Return eDataTypes.EcospaceMPA
                Case eVarNameFlags.LayerPort : Return eDataTypes.EcospaceFleet
                Case eVarNameFlags.LayerSail : Return eDataTypes.EcospaceFleet
                Case eVarNameFlags.LayerAdvection : Return eDataTypes.EcospaceLayerAdvection
            End Select
            Return eDataTypes.NotSet
        End Get
    End Property

    Public Function IsMPAActive(iMPA As Integer) As Boolean
        If (iMPA < 1 Or iMPA > Me.MPAno) Then Return False
        Dim bHasFleet As Boolean = False
        Dim bHasMonth As Boolean = False
        For imonth As Integer = 1 To 12 : bHasMonth = bHasMonth Or (Me.MPAmonth(imonth, iMPA) = False) : Next
        For ifleet As Integer = 1 To Me.nFleets : bHasFleet = bHasFleet Or (Me.MPAfishery(ifleet, iMPA) = False) : Next
        Return bHasFleet And bHasMonth
    End Function

    Public Sub InitContaminantMaps(nLinks As Integer)

        'Detritus
        Me.ConKdetSpace = New Single(Me.InRow, Me.InCol)(,,) {}
        For irow As Integer = 0 To Me.InRow
            For icol As Integer = 0 To Me.InCol
                Me.ConKdetSpace(irow, icol) = New Single(Me.NGroups, Me.NGroups - Me.nLiving, Me.nFleets) {}
            Next
        Next

        'Consumption by cell
        ConKtrophic = New Single(Me.InRow, Me.InCol)() {}
        For irow As Integer = 0 To Me.InRow
            For icol As Integer = 0 To Me.InCol
                Me.ConKtrophic(irow, icol) = New Single(nLinks) {}
            Next
        Next

    End Sub

#End Region

    ''' <summary>Equator length in km.</summary>
    ''' <remarks>http://en.wikipedia.org/wiki/Equator#Exact_length_of_the_Equator</remarks>
    Friend Shared c_sEquatorLength As Single = 40007.862917

    Friend Shared ReadOnly Property DegreeToKm() As Single
        Get
            Return c_sEquatorLength / 360.0!
        End Get
    End Property

    Public Shared Function ToCellSize(ByVal sCellLength As Single, ByVal bAssumeSquareCells As Boolean) As Single
        If (bAssumeSquareCells) Then
            Return sCellLength * 1000.0!
        End If
        Return sCellLength / DegreeToKm
    End Function

    Public Shared Function ToCellLength(ByVal sCellSize As Single, ByVal bAssumeSquareCells As Boolean) As Single
        If (bAssumeSquareCells) Then
            Return sCellSize / 1000.0!
        End If
        Return sCellSize * DegreeToKm
    End Function

End Class

#End Region


