'==============================================================================
'
' $Log: cEcoSpaceDataStructures.vb,v $
' Revision 1.10  2009/04/20 19:43:14  joeb
' typo fix
'
' Revision 1.9  2009/04/20 19:41:54  joeb
' Bug Fix Ecospace slowed down after running half a run. This was caused by the SolverThreads using the wrong time counter (its instead of itt) and throwing an error and writting to the log. The log was choking on because of multiple threads writting to it at the same time..... it goes on and on...
'
' Revision 1.8  2009/02/02 22:29:08  joeb
' Added more output vars to EcoSpace fleets
'
' Revision 1.7  2009/01/20 22:32:01  joeb
' Renamed CatchRegionGearGroup to ResultsCatchRegionGearGroup
'
' Revision 1.6  2009/01/19 20:22:53  joeb
' Fixed bug in AverageSpatialResults() biomass does not need to be average over space. This happens at the end of the time step.
'
' Revision 1.5  2009/01/14 18:46:55  joeb
' Time series results averaged over space at the end of the run
'
' Revision 1.4  2009/01/12 22:54:57  joeb
' Ecospace now stores all results over time. Not just for the summary periods.
'
' Revision 1.3  2008/12/09 19:48:59  joeb
' Ouput objects now use core data instead of buffering data
'
' Revision 1.2  2008/11/28 16:54:10  joeb
' Cleaned up ToDo's
'
' Revision 1.1  2008/09/26 07:30:22  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.42  2008/09/23 16:06:27  joeb
' Fixed bug 432 I hope...
'
' Revision 1.41  2008/09/12 18:30:14  joeb
' Added BiomassByRegion to Ecospace output
'
' Revision 1.40  2008/09/03 08:02:29  villyc
' Adding distribution envelopes to ecospace
'
' Revision 1.39  2008/09/01 11:53:38  villyc
' Adding spatial fields (temp, salinity, ...) to Ecosim and Ecospace
'
' Revision 1.38  2008/08/14 18:07:03  joeb
' Added StartYear and EndYear to MPA Optimizations
'
' Revision 1.37  2008/08/11 02:01:10  jeroens
' Integrated cLayerImportanceData class
'
' Revision 1.36  2008/08/07 19:41:24  sherman
' Exposed LayerImportance from the Core
'
' Revision 1.35  2008/08/07 18:17:33  sherman
' Added Importance Layer to Ecospace Datastructures
'
' Revision 1.34  2008/04/17 20:16:26  joeb
' Change  cSearchDataStructures.bDoFPSearch to cSearchDataStructures.bInSearch
'
' Revision 1.33  2008/03/18 16:50:11  joeb
' Minor tweaks
'
' Revision 1.32  2008/01/13 20:01:45  joeb
' Moved Variables for EcoSeed
'
' Revision 1.31  2007/12/19 20:07:45  joeb
' Added nTimeSteps number of ecospace time steps this was calculated by the core before
'
' Revision 1.30  2007/12/11 18:14:18  joeb
' Minor edits
'
' Revision 1.29  2007/10/16 00:28:37  jeroens
' * Fixed bug 289
'
' Revision 1.28  2007/08/08 18:59:45  joeb
' Added nCellsInRegion for number of map cells in a region
'
' Revision 1.27  2007/07/26 18:40:02  jeroens
' + BarrierAvoidanceWeight moved to space data
'
' Revision 1.26  2007/07/23 21:29:05  willw
' added Aspace and Vspace variables
'
' Revision 1.25  2007/06/29 00:23:21  jeroens
' + Added UseExact variable
'
' Revision 1.24  2007/06/27 23:04:52  willw
' redimmed ismigratory to nvartot for additional use in solvegrid
'
' Revision 1.23  2007/06/22 16:04:27  joeb
' Added SS output
'
' Revision 1.22  2007/06/18 22:07:14  willw
' added new variables for solvegridrow (jstartcol and jendcol) the bounds for water
'
' Revision 1.21  2007/06/14 16:34:28  jeroens
' * Tol and maxIter moved to Ecospace datastructures
'
' Revision 1.20  2007/06/13 20:29:52  willw
' added comments for new variables
'
' Revision 1.19  2007/06/13 19:57:29  joeb
' Changed  IsAdvected() to Boolean
'
' Revision 1.18  2007/06/12 19:20:06  willw
' added some variables for better usage of thread time for space solver, and to make grid solver do less work
'
' Revision 1.17  2007/06/11 22:08:16  joeb
' Added array for reference data fitting
'
' Revision 1.16  2007/06/08 15:53:53  jeroens
' * Threads determined only when not set
'
' Revision 1.15  2007/06/04 20:21:12  willw
' changed MoveScale to 2 from 0.2
'
' Revision 1.14  2007/06/04 18:43:28  joeb
' Changed time step counters
'
' Revision 1.13  2007/05/31 19:56:39  willw
' lots of changes for IBM, fixed multithreading for it
'
' Revision 1.12  2007/05/28 23:19:57  willw
' added stuff for IBM approach
'
' Revision 1.11  2007/05/16 22:02:37  willw
' lots of stuff to commit carl's changes to stanza structures
'
' Revision 1.10  2007/05/15 20:56:10  joeb
' More contaminant tracing changes
'
' Revision 1.9  2007/05/15 15:09:03  joeb
' Added Time Series data
'
' Revision 1.8  2007/05/09 18:09:54  joeb
' Commiting a threading bug!!!!!!!
'
' Revision 1.7  2007/05/09 17:20:11  willw
' net # of threads to be # of logical cpus the computers sees
'
' Revision 1.6  2007/05/09 16:28:38  willw
' no message
'
' Revision 1.5  2007/05/08 18:39:14  willw
' no message
'
' Revision 1.4  2007/05/07 21:34:14  joeb
' Added nSpaceSolverThreads
'
' Revision 1.3  2007/05/07 20:37:59  joeb
' Added Grid Solver variables to Ecospace  input objects
'
' Revision 1.2  2007/05/04 01:25:20  jeroens
' + Ecosim and Ecospace scenario definitions moved to Ecopath data structures
'
' Revision 1.1  2007/05/01 17:12:50  joeb
' Changed directory structure
'
' Revision 1.29  2007/04/30 20:02:26  joeb
' New foraging arena code in derivtRed
'
' Revision 1.28  2007/03/05 23:47:30  joeb
' *** empty log message ***
'
' Revision 1.27  2007/03/01 18:32:41  joeb
' Update to new interface names
'
' Revision 1.26  2007/02/25 17:26:30  joeb
' Bug Fixes
'
' Revision 1.25  2007/02/19 20:03:51  joeb
' Changes to defaults and initialization
'
' Revision 1.24  2007/02/12 23:48:02  joeb
' Data Summary
'
' Revision 1.23  2007/02/08 15:49:04  joeb
' Changes to dimesioning of summary data
'
' Revision 1.22  2007/02/06 15:41:48  joeb
' Added Ecospace results
'
' Revision 1.21  2007/01/30 17:01:26  jeroens
' * MPAMonth changed to boolean
'
' Revision 1.20  2007/01/20 03:19:42  jeroens
' + Included EcopathFleetDBID in fleet ReDim
'
' Revision 1.19  2007/01/19 01:14:37  joeb
' Initialization of EcospaceGroups
'
' Revision 1.18  2007/01/18 17:49:59  jeroens
' + MPA DBID redimmed too
'
' Revision 1.17  2007/01/18 15:33:27  jeroens
' + MigratoryVars resized in SetDefaults
'
' Revision 1.16  2007/01/17 20:13:47  joeb
' Changed nGroups to Public Property
' Added dimensioning for Database ids
'
' Revision 1.15  2007/01/17 18:35:22  joeb
' Changes to ReDim code for most data
'
' Revision 1.14  2007/01/17 16:45:09  jeroens
' + Preparing for connection to datasource
'
' Revision 1.13  2007/01/16 20:46:22  joeb
' Removed Asserts
'
' Revision 1.12  2007/01/16 17:20:55  jeroens
' * Implementing more Ecospace interfaces
'
' Revision 1.11  2006/12/18 22:17:54  joeb
' First working version. This will load a default spatial grid and give the same results as EwE5 Ecospace on a default grid.
'
' Revision 1.10  2006/12/07 15:08:54  jeroens
' + Added habitat change vars
'
' Revision 1.9  2006/12/05 21:36:29  joeb
' Bug fixes to FindSpatialEquilibrium
'
' Revision 1.8  2006/12/04 20:33:00  joeb
' Added W (SOR weight)
'
' Revision 1.7  2006/12/04 14:33:00  jeroens
' + Strict ON
' + Added database variables
'
'==============================================================================

Option Strict On

Public Class cEcospaceDataStructures

#Region "Public Fields"

#Region " Storage classes "

    Public Class cLayerImportanceData

        Public DBID As Integer
        Public Data(,) As Single
        Public strName As String
        Public strDescription As String
        Public sWeight As Single

        Public Sub New(ByVal inRow As Integer, ByVal inCol As Integer)
            ReDim Data(inRow, inCol)
        End Sub

    End Class
#End Region

    Public EcosimScenarioDBID As Integer
    ''' <summary>Array of ecospace group database IDs.</summary>
    Public GroupDBID() As Integer
    Public EcopathGroupDBID() As Integer
    ''' <summary>Array of ecospace region database IDs.</summary>
    Public RegionDBID() As Integer
    ''' <summary>Array of ecospace habitat database IDs.</summary>
    Public HabitatDBID() As Integer
    ''' <summary>Array of ecospace MPA database IDs.</summary>
    Public MPADBID() As Integer
    ''' <summary>Array of ecospace Fleet database IDs.</summary>
    Public FleetDBID() As Integer
    Public EcopathFleetDBID() As Integer


    'number of years to run the simulation for
    Public TotalTime As Single

    'flags
    'jb PredictEffort was an integer in EwE5 
    Public PredictEffort As Boolean
    Public AdjustSpace As Boolean
    Public SpaceTime As Boolean

    Public IsFishRateSet As Boolean

    Public CurrentForce As Boolean
    'jb Ecoseed may get move to an object
    'for now this will let the code function
    Public EcoseedOn As Boolean

    Public chkMPA As Boolean

  
    ''' <summary>
    ''' Current Model time step
    ''' </summary>
    ''' <remarks>This is the time in years not the array index</remarks>
    Public TimeNow As Single
    Public TimeStep As Single

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

    ''' <summary>Number of Importance layers</summary>
    Public nImportanceLayers As Integer

    ''' <summary>Descriptive text of habitat type (name) </summary>
    Public HabitatText() As String

    ''' <summary>States whether a group prefers a habitat.</summary>
    Public PrefHab(,) As Boolean

    ''' <summary> Does this Fishing fleet use this habitat type </summary>
    Public GearHab(,) As Boolean

    ''' <summary>
    ''' Total number of habitat cells by habitat type
    ''' </summary>
    ''' <remarks>Caluclated in CalcHabitatArea()</remarks>
    Public HabArea() As Single

    ''' <summary>
    ''' Proportion of total habitat area by Habitat type
    ''' </summary>
    ''' <remarks>HabAreaProportion(iHab) = HabArea(iHab) / TotalHabitatArea </remarks>
    Public HabAreaProportion() As Single

    Public AdvectSpeed As Single

    Public MoveScale As Single

    ''' <summary>
    ''' Inverse of emigration response to fitness
    ''' </summary>
    ''' <remarks>In EwE5 there is no variable for this it is read from the interface when it is needed</remarks>
    Public FitnessResp As Single

    ''' <summary>Number of habitat time changes</summary>
    Public NoHabChanges As Integer
    ''' <summary>Habitat time for NoHabChange #</summary>
    Public HabTime() As Single
    ''' <summary>Habitat changes for NoHabChange #</summary>
    Public HabChange(,) As Integer

    'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
    'Map Variables

    ''' <summary>Number of rows in the current base map</summary>
    Public Inrow As Integer
    ''' <summary>Number of rows in the current base map</summary>
    Public InCol As Integer
    ''' <summary>Length in KM of a cell </summary>
    ''' <remarks>Not area</remarks>
    Public CellLength As Single
    ''' <summary>Current basemap stepsize, in number of steps per degree</summary>
    Public IDH_SS As Single
    ''' <summary>Upper left coordinate of the current basemap</summary>
    Public IDH_UL As Single
    ''' <summary>Latitude of upper left coordinate of the current basemap, calculated from <see cref="IDH_UL">IDH_UL</see></summary>
    Public Lat1 As Single
    ''' <summary>Longitude of upper left coordinate of the current basemap, calculated from <see cref="IDH_UL">IDH_UL</see></summary>
    Public Lon1 As Single
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

    'Public RecSplit() As Single
    'Public PconSplit() As Single
    'Public Tstanza() As Single
    'Public NstanzaBase() As Single
    Public Basebiomass() As Single
    Public Bnew() As Single
    Public der() As Single
    'Public EatEff() As Single
    Public EatEffBad() As Single
    'Public Flowin() As Single
    'Public FlowoutRate() As Single
    Public MPABiomass() As Single
    Public Mrate() As Single
    Public Mvel() As Single
    Public RelMoveBad() As Single
    Public RelVulBad() As Single
    ' Public VulPred() As Single
    Public IsAdvected() As Boolean

    Public AMm(,,) As Single
    Public Bcell(,,) As Single
    ' If ConSimOn Then
    Public Ccell(,,) As Single
    Public Clast(,,) As Single
    Public AMmTr(,,) As Single
    Public Ftr(,,) As Single
    ' End If
    'public Bclose(,,) As Single
    Public Bcw(,,) As Single
    Public Blast(,,) As Single
    Public C(,,) As Single
    Public d(,,) As Single
    Public Depth(,) As Integer
    Public DepthA(,) As Single
    Public DepthX(,) As Integer
    Public DepthY(,) As Single

    'these are all part of velmaker
    'velmaker may become its own class
    Public Xvel(,) As Single, Yvel(,) As Single
    Public Xvloc(,) As Single, Yvloc(,) As Single
    Public UpVel(,) As Single
    Public Xv(,,) As Single, Yv(,,) As Single 'SM, 3d arrays for time varying current velocities.
    Public flow(,) As Single

    Public E(,,) As Single
    Public BcwNomig(,,) As Single
    Public CNomig(,,) As Single
    Public dNomig(,,) As Single
    Public Enomig(,,) As Single
    Public F(,,) As Single
    Public HabType(,) As Integer
    Public RegionName() As String
    Public Region(,) As Integer
    Public MPA(,) As Integer
    Public RelPP(,) As Single
    Public RelCin(,) As Single
    Public DepthOrig(,) As Integer    'for use with habitat change
    Public HabTypeorig(,) As Integer  'for use with habitat change
    Public MPAorig(,) As Integer      'for use with habitat change
    Public RelPPorig(,) As Single     'for use with habitat change
    Public RelCinorig(,) As Single    'for use with habitat change
    Public Sail(,,) As Single 'effort to fish a map cell, used as a multiplier with effort, Scaled to Ecopath ScaleSailingToUnity() in InitSpatialEqulibrium()
    Public Port(,,) As Boolean
    Public ImportanceLayers As New List(Of cLayerImportanceData)

    Public EffPower() As Single

    Public BBase() As Single
    Public NoRegions As Integer

    'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
    'Summary data

    ''' <summary>Number of timesteps the model ran for. Used to average data over the run.</summary>
    Public nSumTimeSteps As Integer

    Public NumStep As Integer       'Number of time steps for averaging summary window data

    ''' <summary>Start time of the first and second summary data period. In Years </summary>
    ''' <remarks> Data is summarized over two time periods set by SumStart(0) and SumStart(1). The number of time steps to summarize over is set in NumStep.
    ''' Defaults are set in redimTimeVaraibles().
    ''' Used in cEcospace.summarySetTimeStep() to set the index to store the summary data in. The first or second summary period.
    ''' </remarks>
    Public SumStart(1) As Single

    Public ResultsCatchRegionGearGroup(,,,) As Single 'ResultsCatchRegionGearGroup( NoRegions, nFleets, NGroups, ntimesteps)
    Public ResultsByFleet(,,) As Single 'ResultsByFleet(nvars,nFleets,NumberOfTimeSteps)
    Public ResultsByFleetGroup(,,,) As Single 'ResultsByFleetGroup(nvars,nFleets,nGroups,NumberOfTimeSteps)
    Public ResultsRegionGroup(,,) As Single 'ResultsRegionGroup(region, group, timestep)

    ''' <summary> Summarized time step data </summary>
    ''' <remarks>populated in sumarizeTimeStepData()</remarks>
    Public ResultsByGroup(,,) As Single 'ResultsByGroup(nVars,Ngroups,  NumberOfTimeSteps)

    Public ResultsSummaryByFleet(,) As Single 'vars, fleets

    ''' <summary>Number of variables in ResultsXXX arrays </summary>
    Public Const N_RESULTS_GROUPS As Integer = 2
    Public Const N_RESULTS_FLEETS As Integer = 3
    Public Const N_RESULTS_FLEETGROUPS As Integer = 1

    'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

    Public PPupWell As Single

    Public PrefRow(,) As Integer
    Public Prefcol(,) As Integer
    Public IsMigratory() As Boolean
    Public MigConcRow() As Single
    Public MigConcCol() As Single

    Public SailScale() As Single

    Public FitRespType As Integer

    Public SEmult() As Single

    Public Attract(,) As Single

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
    Public MPAmonth(,) As Boolean
    Public MPAfishery(,) As Boolean


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
    ''' Number of groups for each solver thread
    ''' </summary>
    Public nGroupsPerThread As Integer

    ''' <summary>
    ''' Number of threads to run the groups biomass calculations on 
    ''' </summary>
    Public nSpaceSolverThreads As Integer

    ''' <summary>
    ''' Number of cells per biomass thread
    ''' </summary>
    Public nCellsPerThread As Integer

    'number of species per thread for the IBM stuff
    Public nIBMGroupsPerThread As Integer

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
    ''' If fish are getting cought in complex habitat, increasing this value will help the fish get "un-stuck".</para>
    ''' <para>Possible values [0-1]</para>
    ''' <para>Increasing this will increase the concentration of the fish, so the regular NS/EW concentrations should
    ''' be lowered to keep the concentration the same.</para>
    ''' </summary>
    Public barrierAvoidanceWeight() As Single


    'VC Hobart Sep 2008: We need a data structure for handling salinity, temperature, etc.
    'it should eventually (perhaps) be dimensioned with time steps as well. 
    Public SpatialField(,,) As Single               'row, col, index
    Public SpatialFieldOptimum(,) As Boolean   ' group, index
    Public SpatialFieldStdLeft(,) As Boolean   ' group, index   = left side of normal distribution
    Public SpatialFieldStdRight(,) As Boolean  ' group, index   = right side of normal dist
    Public nSpatialFields As Integer      'this is to be read from the init file when connecting to other
    'models, or when we have interface for this it will be read from Ecosim info
    Public SpatialFieldsInUse As Boolean   'Use this to turn on the processing of spatial fields in SpaceSolver


    'VC Hobart Sep 2008: we need a way to handle species distribution envelopes. 
    'we now assign groups/species to habitats, but to address climate change issues, it would make more sense
    'to use a distribution envelope, so that we can limit the species to their actual occurrence area, rather
    'than everywhere on a basemap where the right habitat is available
    Public DistributionEnvelope(,,) As Boolean 'format: row, col, group


#End Region

#Region "Private Data"

    'not much
    Private m_ngroups As Integer

#End Region

#Region "Public Properties"
    ''' <summary>Number of Base Groups (Ecopath) </summary>
    ''' <remarks>This was nvar in EwE5</remarks>
    Public Property NGroups() As Integer
        Get
            Return m_ngroups
        End Get
        Set(ByVal value As Integer)
            m_ngroups = value
            redimGroupDBID() 'implicit ??????
            'this is different then the other counters (nFleets....) 
            'which delay the dimensioning until the data is loaded
            'this may not be a good idea
        End Set
    End Property

    Public ReadOnly Property nCellsInRegion(ByVal iRegion As Integer) As Integer

        Get
            Try
                Dim n As Integer = 0
                For irow As Integer = 1 To Inrow
                    For icol As Integer = 1 To InCol
                        If Region(irow, icol) = iRegion Then
                            n += 1
                        End If
                    Next
                Next
                Return n

            Catch ex As Exception
                cLog.Write(ex)
                Return 0
            End Try
        End Get

    End Property

    Public ReadOnly Property nTimeSteps() As Integer

        Get
            Return CInt(TotalTime * (1 / TimeStep))
        End Get

    End Property


#End Region

#Region "Public Methods"

    ''' <summary>
    ''' Set default values and dimemsion basic arrays
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>This should be called before reading values from database. I think..... I hope!!!!!!!!!!</remarks>
    Public Function SetDefaults() As Boolean
        Dim i As Integer

        Try

            'EwE5 default value hardwired into the interface
            FitnessResp = 100
            PPupWell = 0.01
            PredictEffort = True

            'SOR weight from EwE5 interface frmSpace.text3
            W = 0.9

            TimeStep = 1 / 12 'monthly time steps. In EwE5 this is set all over the place 

            'EwE5 set to True in frmSpace.Form_Activate()
            'its value is then changed from an option radio button SpaceInit() on the run tab
            AdjustSpace = True


            'jb SpaceTime and CurrentForce defaults from EwE5 frmSpace.Load()
            SpaceTime = True 'in EwE5 the check box that controls this is labled 'Integrate' on the run tab
            CurrentForce = False

            Inrow = 0
            InCol = 0

            AdvectSpeed = 0.1

            CellLength = 100 'this is from the EwE5 database

            MoveScale = 2 '0.2
            If TotalTime = 0 Then TotalTime = 50 'default of 50 year simulation

            'redimTimeVaraibles()
            setDefaultSummaryPeriod()

            NoHabitats = 1
            'requires NoHabitats, nGroups, nFleets, NoHabChanges
            RedimHabitatVariables()

            'dimension arrays to current problem size
            DefaultBasemapDimensions()
            ReDimMapVars()

            'requires nGroups, calculates nvartot and Nvarsplit
            ReDimMapDims()

            RedimMigratoryVariables()

            SetDefaultMeanVelocityMvel()


            For i = 1 To nGroups                            'CJW had nvar not n1
                PrefHab(i, 0) = True
            Next 'set preferred habitat to 1 (pelagic) by default

            For i = 1 To Inrow
                For j As Integer = 1 To InCol      'Default Values for new maps
                    Depth(i, j) = 1
                    HabType(i, j) = 1
                    RelPP(i, j) = 1
                    RelCin(i, j) = 1
                    For K As Integer = 1 To nFleets
                        Sail(K, i, j) = 1
                    Next
                Next
            Next

            ReDimFleets()

            ' setDefaultThreads()

            Return True
        Catch ex As Exception
            Debug.Assert(False, ex.Message)
            Return False
        End Try

    End Function


    Public Sub SetDefaultThreads()
        'multi threading defaults
        ' JS 08jun07: added 0 check since the datasource may have provided these values
        If (Me.nGridSolverThreads = 0) Then
            Me.nGridSolverThreads = System.Environment.ProcessorCount
            Me.nSpaceSolverThreads = System.Environment.ProcessorCount
        End If
        Me.nGroupsPerThread = Me.nvartot \ Me.nGridSolverThreads + 1
        Me.nCellsPerThread = Me.Inrow * Me.InCol \ Me.nGridSolverThreads + 1 '?????

    End Sub

    Private Sub SetDefaultMeanVelocityMvel()
        Dim i As Integer
        Dim j As Integer

        Try

            Debug.Assert(EcoPathData IsNot Nothing, "Ecospace must have a reference to Ecopath data to initialize.")

            'Dim MaxTL As Single
            'MaxTL = 0
            'For j = 1 To NumLiving
            '    If TTLX(j) > MaxTL Then MaxTL = TTLX(j)
            'Next
            'MaxTL = MaxTL - 1
            'Set max average velocity movement to 100 km/year and the others linearly scaled after trophic level
            For j = 1 To NGroups  'NumLiving
                Mvel(j) = 300   'CInt(99 * (1 - (MaxTL - (TTLX(j) - 1)) / MaxTL)) + 1
            Next
            'For j = NumLiving + 1 To NumGroups
            '    Mvel(j) = 1
            'Next
            'How about discards they should have a lower dispersal rate:
            'check the discard fate
            'DiscardFate(NumGear, NumGroups - NumLiving)
            For j = nLiving + 1 To NGroups
                For i = 1 To nFleets
                    If EcoPathData.DiscardFate(i, j - nLiving) > 0 Then
                        Mvel(j) = 10
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
            ReDim MPAname(Me.MPAno)
            ReDim MPAmonth(12, Me.MPAno)
            ReDim MPAfishery(Me.nFleets, Me.MPAno)
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

            ReDim PrefRow(nGroups, 12)
            ReDim Prefcol(nGroups, 12)
            ReDim IsMigratory(nvartot)
            ReDim MigConcRow(NGroups)
            ReDim MigConcCol(NGroups)
            ReDim barrierAvoidanceWeight(NGroups)

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
    ''' This is called when ever the number of groups or number of habitat types changes.
    ''' Called when a new model is loaded (PreserveHabitat = False) or the user has changed the number of habitat types (PreserveHabitat = True).
    ''' If only the number of habitats has changed then it will keep the existing data (PreserveHabitat = True). 
    ''' If the number of groups has changed then all the data must be re-initialized (from the datasource).
    '''</remarks>
    Public Sub RedimHabitatVariables(Optional ByVal PreserveHabitat As Boolean = False)

        Try

            If Not PreserveHabitat Then
                'new model is being read
                'clear out the exiting data
                ReDim PrefHab(NGroups, NoHabitats)
                ReDim GearHab(nFleets, NoHabitats)
                ReDim HabitatText(NoHabitats)
                ReDim HabArea(NoHabitats)
                ReDim HabAreaProportion(NoHabitats)
                ReDim HabitatDBID(NoHabitats)

                ' JS 15oct07: fix for bug 289 - By default, GearHab and PrefHab are True for 'All' habitat
                For iGroup As Integer = 0 To NGroups
                    PrefHab(iGroup, 0) = True
                Next

                For iFleet As Integer = 0 To nFleets
                    GearHab(iFleet, 0) = True
                Next

            Else
                'only the number of habitats has changed 
                'keep the existing data
                ReDim Preserve PrefHab(NGroups, NoHabitats)
                ReDim Preserve GearHab(nFleets, NoHabitats)
                ReDim Preserve HabitatText(NoHabitats)
                ReDim Preserve HabArea(NoHabitats)
                ReDim Preserve HabAreaProportion(NoHabitats)
                ReDim Preserve HabitatDBID(NoHabitats)

            End If

            ReDim HabTime(NoHabChanges)
            ReDim HabChange(3, NoHabChanges)


            'jb From EwE5
            'If NoHabitats = 1 Then 'this is first entry
            '    ReDim PrefHab(nGroups, NoHabitats)  'new model is being read
            '    ReDim GearHab(nFleets, NoHabitats)
            'Else
            '    If UBound(PrefHab, 1) = nGroups Then
            '        ReDim Preserve PrefHab(nGroups, NoHabitats)   'CJW had nvar not n1
            '    Else
            '        ReDim PrefHab(nGroups, NoHabitats)   'new model is being read
            '    End If
            '    If UBound(GearHab, 1) = nFleets Then
            '        ReDim Preserve GearHab(nFleets, NoHabitats)
            '    Else
            '        ReDim GearHab(nFleets, NoHabitats)
            '    End If
            'End If

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".RedimHabitatVariables() Error: " & ex.Message)
            Throw New System.Exception(Me.ToString & ".RedimHabitatVariables() Error: " & ex.Message)
        End Try


    End Sub

    ''' <summary>
    ''' Set the Map to its default size
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub DefaultBasemapDimensions()

        If Inrow = 0 Then Inrow = 20 'number of map cell rows
        If InCol = 0 Then InCol = 20 'number of map cell columns

        If CellLength = 0 Then CellLength = 5 'map cell side length (km)

        If IDH_SS = 0 Then IDH_SS = 2
        If IDH_UL > 0 Then
            ' JS 061204: Replaced '\' operator by Math.Truncate(# / #)
            Lat1 = CSng((Math.Truncate(IDH_UL / 10000) - 900) / 10)
            Lon1 = CSng((IDH_UL - Math.Truncate(IDH_UL / 10000) * 10000) / 10 - 180)
        End If

    End Sub

    Sub ReDimMapVars()
        Dim i As Integer, j As Integer

        'jb EwE5 variables not used here
        ''NvarTot = nvar + 2 * npairs
        'ReDim regColr(NoRegions)
        'X = ColorGrad(regColr)
        'ReDim habColr(NoHabitats)
        'X = ColorGrad(habColr)
        Try

            Debug.Assert(StanzaGroups IsNot Nothing, Me.ToString & ".ReDimMapVars() Stanzagroups needs to be set.")

            'count up the total number of stanza groups
            Nvarsplit = 0
            For i = 1 To StanzaGroups.Nsplit
                For j = 1 To StanzaGroups.Nstanza(i)
                    Nvarsplit = Nvarsplit + 1
                Next
            Next

            'jb EwE5 EwE6 does not have Pairs (split pools)
            'nvartot = NumGroups + 2 * npairs + Nvarsplit
            nvartot = nGroups + Nvarsplit

            ReDim Basebiomass(nvartot)
            ReDim Bnew(nvartot)
            ReDim der(nvartot)
            'ReDim EatEff(nvartot)
            ReDim EatEffBad(nvartot)
            'ReDim Flowin(nvartot)
            'ReDim FlowoutRate(nvartot)
            ReDim MPABiomass(nvartot)
            ReDim Mrate(nvartot)
            ReDim Mvel(nvartot)
            ReDim RelMoveBad(nvartot)
            ReDim RelVulBad(nvartot)
            '   ReDim VulPred(nvartot)
            ReDim IsAdvected(nGroups)

            'jb PrefHab() was redimed here and redimHabitatVariables()
            '        ReDim PrefHab(nGroups, NoHabitats)

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".ReDimMapVars() Error: " & ex.Message)
            Throw New System.Exception(Me.ToString & ".ReDimMapVars() Error: " & ex.Message)
        End Try



    End Sub


    Public Sub ReDimFleets()
        Try

            ReDim Me.FleetDBID(nFleets)
            ReDim Me.EcopathFleetDBID(nFleets)
            ReDim Me.SEmult(nFleets)
            ReDim Me.EffPower(nFleets)

            setFleetDefaults()

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".ReDimFleets() Error: " & ex.Message)
            Throw New System.Exception(Me.ToString & ".ReDimFleets() Error: " & ex.Message)
        End Try

    End Sub

    Private Sub setFleetDefaults()
        'calculate relative catchabilities by gear and species
        'jb just set to default of one
        For i As Integer = 1 To nFleets
            EffPower(i) = 1
            SEmult(i) = 1
        Next 'initially set all gears to fish everywhere

    End Sub

    Public Sub ReDimRegionVars()
        ReDim Me.RegionDBID(NoRegions)
        ReDim Me.RegionName(NoRegions)
        ReDim ResultsRegionGroup(NoRegions, NGroups, nTimeSteps)
    End Sub

    Public Sub ReDimMapDims()
        'NvarTot = nvar + 2 * npairs
        Dim i As Integer, j As Integer

        Debug.Assert(StanzaGroups IsNot Nothing, Me.ToString & ".ReDimMapDims() Stanzagroups needs to be set.")

        Try

            'jb this is also set in ReDimMapVars()
            Nvarsplit = 0
            For i = 1 To StanzaGroups.Nsplit
                For j = 1 To StanzaGroups.Nstanza(i)
                    Nvarsplit = Nvarsplit + 1
                Next
            Next
            nvartot = nGroups + Nvarsplit

            ReDim AMm(Inrow + 1, InCol + 1, nvartot)
            ReDim Bcell(Inrow + 1, InCol + 1, nvartot)

            'If ConSimOn Then
            'ReDim Ccell(Inrow + 1, InCol + 1, NGroups)
            'ReDim Clast(Inrow + 1, InCol + 1, NGroups)
            'ReDim AMmTr(Inrow + 1, InCol + 1, NGroups)
            'ReDim Ftr(Inrow + 1, InCol + 1, NGroups)
            ' End If

            'ReDim Bclose(Inrow + 1, Incol + 1, NvarTot) 
            ReDim Bcw(Inrow + 1, InCol + 1, nvartot)
            ReDim Blast(Inrow + 1, InCol + 1, nvartot)
            ReDim C(Inrow + 1, InCol + 1, nvartot)
            ReDim d(Inrow + 1, InCol + 1, nvartot)
            ReDim Depth(Inrow + 1, InCol + 1)
            ReDim DepthA(Inrow + 1, InCol + 1)
            ReDim Xvel(Inrow + 1, InCol + 1), Yvel(Inrow + 1, InCol + 1)
            ReDim Xvloc(Inrow + 1, InCol + 1), Yvloc(Inrow + 1, InCol + 1)
            ReDim UpVel(Inrow + 1, InCol + 1)
            ReDim E(Inrow + 1, InCol + 1, nvartot)
            ReDim BcwNomig(Inrow + 1, InCol + 1, nvartot)
            ReDim CNomig(Inrow + 1, InCol + 1, nvartot)
            ReDim dNomig(Inrow + 1, InCol + 1, nvartot)
            ReDim Enomig(Inrow + 1, InCol + 1, nvartot)
            ReDim F(Inrow + 1, InCol + 1, nvartot)
            ReDim HabType(Inrow + 1, InCol + 1)
            ReDim Region(Inrow + 1, InCol + 1)
            ReDim MPA(Inrow + 1, InCol + 1)
            ReDim RelPP(Inrow + 1, InCol + 1)
            ReDim RelCin(Inrow + 1, InCol + 1)
            ReDim DepthOrig(Inrow + 1, InCol + 1)   'for use with habitat change
            ReDim HabTypeorig(Inrow + 1, InCol + 1)  'for use with habitat change
            ReDim MPAorig(Inrow + 1, InCol + 1)      'for use with habitat change
            ReDim RelPPorig(Inrow + 1, InCol + 1)      'for use with habitat change
            ReDim RelCinorig(Inrow + 1, InCol + 1)     'for use with habitat change
            ReDim Sail(nFleets, Inrow + 1, InCol + 1)
            ReDim Port(nFleets, Inrow + 1, InCol + 1)


            'must redim the seed var here too abmpa
            ' ReDim MPASeed(Inrow + 1, InCol + 1)

            ImportanceLayers.Clear()
            For i = 0 To nImportanceLayers - 1
                ImportanceLayers.Add(New cLayerImportanceData(Inrow, InCol))
            Next

            ReDim MPAfishery(nFleets, 1)
            ReDim MPAmonth(12, 1)


            ''jb move this here to set a few defaults this will have to change
            For i = 1 To NGroups                            'CJW had nvar not n1
                PrefHab(i, 0) = True
            Next 'set preferred habitat to 1 (pelagic) by default

            For i = 1 To Inrow
                For j = 1 To InCol      'Default Values for new maps
                    Depth(i, j) = 1
                    HabType(i, j) = 1
                    RelPP(i, j) = 1
                    RelCin(i, j) = 1
                    For K As Integer = 1 To nFleets
                        Sail(K, i, j) = 1
                    Next
                Next
            Next


        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".ReDimMapDims() Error: " & ex.Message)
            Throw New System.Exception(Me.ToString & ".ReDimMapDims() Error: " & ex.Message)
        End Try

    End Sub


    Public Sub RedimConSimVars()

        ReDim Ccell(Inrow + 1, InCol + 1, NGroups)
        ReDim Clast(Inrow + 1, InCol + 1, NGroups)
        ReDim AMmTr(Inrow + 1, InCol + 1, NGroups)
        ReDim Ftr(Inrow + 1, InCol + 1, NGroups)

    End Sub

    Public Sub redimGroupDBID()
        Try
            'called for NGroups Public property
            ReDim GroupDBID(m_ngroups)
            ReDim EcopathGroupDBID(m_ngroups)
        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".redimGroupDBID() Error: " & ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Redim the data that saves the Ecospace results over time
    ''' </summary>
    ''' <remarks>This must be called by Ecospace at the start of a run to clear out any existing data.</remarks>
    Public Sub redimTimeStepResults(ByVal NumberOfTimeSteps As Integer)

        Debug.Assert(TimeStep > 0 And TotalTime > 0)

        'reset the number of time steps the model ran for
        nSumTimeSteps = 0

        ReDim Me.ResultsByGroup(N_RESULTS_GROUPS, m_ngroups, NumberOfTimeSteps)
        ReDim Me.ResultsByFleet(N_RESULTS_FLEETS, nFleets, NumberOfTimeSteps)
        ReDim Me.ResultsByFleetGroup(N_RESULTS_FLEETGROUPS, nFleets, NGroups, NumberOfTimeSteps)

        ReDim Me.ResultsRegionGroup(NoRegions, NGroups, NumberOfTimeSteps)
        ReDim Me.ResultsCatchRegionGearGroup(NoRegions, nFleets, NGroups, NumberOfTimeSteps)

    End Sub


    Public Sub setDefaultSummaryPeriod()
        Try
            Debug.Assert(TimeStep > 0)
            'set the summary data to be over the total time
            SumStart(0) = 0 'start of first summary period
            SumStart(1) = TotalTime - 1 'start of last summary perion
            NumStep = CInt(1.0 / TimeStep) 'number of time steps to summarize over one year for the default summary
        Catch ex As Exception
            SumStart(0) = 0 'start of first summary period
            SumStart(1) = TotalTime - 1 'start of last summary period
            NumStep = 1 'number of time steps to summarize over one year for the default summary
            Debug.Assert(False)
        End Try
    End Sub

    Public Sub redimForReferenceData()

        '       If SpDatYear > 0 Then  'there are timeseries
        Dim ttYears As Integer = CInt(TotalTime)
        ReDim SpaceBiomassByRegion(ttYears, NGroups, NoRegions)
        ReDim SpaceBiomassByRegionCount(ttYears, NGroups, NoRegions)
        ReDim SpaceCatchByRegion(ttYears, NGroups, NoRegions)
        ReDim SpaceCatchByRegionCount(ttYears, NGroups, NoRegions)
        ReDim SpaceEffortByRegionFleet(ttYears, nFleets, NoRegions)
        ReDim SpaceEffortByRegionFleetCount(ttYears, nFleets, NoRegions)

        'If ConSimOn Then 'only if there are tracer data
        '    ReDim SpaceTraceByRegion(TotalTime, NumGroups, NoRegions)
        '    ReDim SpaceTraceByRegionCount(TotalTime, NumGroups, NoRegions)
        'End If

        '     End If

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
            eSailEffort = eSailEffort + Me.ResultsByFleet(eSpaceResultsFleets.SailingEffort, iFleet, it)
            eFishEffort += Me.ResultsByFleet(eSpaceResultsFleets.FishingEffort, iFleet, it)
        Next
        eFishEffort = eFishEffort / nts

        'cost = [fixed cost] + ([fishing effort] * [ecopath effort cost] + [sailing effort] * [ecopath sailing cost])
        startCost = EcopathCost(iFleet, 1) + (sFishEffort * EcopathCost(iFleet, 2) + sSailEffort * EcopathCost(iFleet, 3))
        endCost = EcopathCost(iFleet, 1) + (eFishEffort * EcopathCost(iFleet, 2) + eSailEffort * EcopathCost(iFleet, 3))

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

            For ivar = 0 To N_RESULTS_FLEETS
                For iflt = 0 To Me.nFleets
                    For it = 1 To nTimeSteps
                        Me.ResultsByFleet(ivar, iflt, it) /= Me.nWaterCells
                    Next it
                Next iflt
            Next ivar

            For ivar = 0 To N_RESULTS_FLEETGROUPS
                For iflt = 0 To Me.nFleets
                    For igrp = 1 To Me.NGroups
                        For it = 1 To nTimeSteps
                            Me.ResultsByFleetGroup(ivar, iflt, igrp, it) /= Me.nWaterCells
                        Next it
                    Next igrp
                Next iflt
            Next ivar

            For irgn = 0 To Me.NoRegions
                ncells = Me.nCellsInRegion(irgn)
                If ncells = 0 Then ncells = 1
                For igrp = 1 To Me.NGroups
                    For it = 1 To nTimeSteps
                        Me.ResultsRegionGroup(irgn, igrp, it) /= ncells
                    Next it
                Next igrp
            Next irgn

            For irgn = 0 To Me.NoRegions
                ncells = Me.nCellsInRegion(irgn)
                If ncells = 0 Then ncells = 1
                For iflt = 0 To Me.nFleets
                    For igrp = 1 To Me.NGroups
                        For it = 1 To nTimeSteps
                            Me.ResultsCatchRegionGearGroup(irgn, iflt, igrp, it) /= ncells
                        Next it
                    Next igrp
                Next iflt
            Next irgn

        Catch ex As Exception
            Debug.Assert(False, ex.Message)
            cLog.Write(ex)
        End Try

    End Sub


    Public Sub SummarizeResults(ByVal EcopathCost(,) As Single, ByVal JobMultiplier() As Single)
        Dim SailEffort As Single, FishEffort As Single
        Dim cost As Single, value As Single

        Debug.Assert(nSumTimeSteps <= ResultsByFleet.GetUpperBound(2), "EcoSpace summary data time step counter not set correctly!")

        'number of years the model actually ran for, computed in case the model run was stopped by the user
        Dim nYears As Single = Me.nSumTimeSteps / (1 / TimeStep)

        ReDim Me.ResultsSummaryByFleet(1, Me.nFleets)

        'All values in ResultsByFleet() have been averaged over space
        For iflt As Integer = 0 To Me.nFleets
            SailEffort = 0
            FishEffort = 0
            value = 0
            For it As Integer = 1 To Me.nSumTimeSteps
                SailEffort += Me.ResultsByFleet(eSpaceResultsFleets.SailingEffort, iflt, it)
                FishEffort += Me.ResultsByFleet(eSpaceResultsFleets.FishingEffort, iflt, it)
                value += Me.ResultsByFleet(eSpaceResultsFleets.Value, iflt, it)
            Next

            cost = EcopathCost(iflt, 1) + (FishEffort * EcopathCost(iflt, 2) + SailEffort * EcopathCost(iflt, 3))

            'profit average yearly
            ResultsSummaryByFleet(0, iflt) = (value - cost) / nYears
            'jobs average yearly
            ResultsSummaryByFleet(1, iflt) = value * JobMultiplier(iflt) / nYears

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
        startIndex = CInt(Me.SumStart(0) * Me.NumStep) + 1
        endIndex = CInt(Me.SumStart(1) * Me.NumStep) + 1
        nIndexes = Me.NumStep
    End Sub

#End Region

End Class



