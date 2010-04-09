Imports System
Imports System.Math
Imports System.Threading
Imports EwEPlugin
Imports EwECore.EcoSeed
Imports EwEUtils.Core


''' <summary>
''' Definition of Time Step Delegate use for notification of an EcoSim time step
''' </summary>
Public Delegate Sub EcoSpaceTimeStepDelegate(ByVal iTime As Integer)



Public Class cEcoSpace

    'ToDo_jb Change summary values to be across all time steps

#Region "Solver threads"

    Public Delegate Sub SolverErrorDelegate(ByVal ThreadID As Integer, ByVal msg As String)
    Private m_bsolverError As Boolean
    Private m_solverErrorMsg As String
    Private m_solverErrorID As Integer

#End Region

#Region "Private data"

    ''' <summary>To call the plugins</summary>
    Private m_pluginManager As cPluginManager

    ' Private m_lstTimeStepCallBacks As New List(Of EcoSpaceTimeStepStartDelegate)

    'new multiStanza stuff
    Private TotLoss() As Single
    Private TotEatenBy() As Single
    Private TotBiom() As Single
    Private TotPred() As Single
    Private TotIFDweight() As Single
    Private Blocal() As Single
    Private Tbiom As Single, Tpred As Single, Wcell As Single

    Private m_gridSolvers As List(Of cGridSolver)
    Private m_spaceSolvers As List(Of cSpaceSolver)
    Private m_IBMSolvers As List(Of cIBMSolver)

    Private iTotalCells As Integer 'this is the total number of spatial cells i.e. rows*columns

    Private m_TimestepDelegate As EcoSpaceTimeStepDelegate

    Private m_publisher As New cMessagePublisher

    'Ecosim and EcoPath data will be via m_ESData and m_EPData references NOT through the model reference (m_Ecosim)
    'I still need to sort out how Ecosim needs to be intialized for Ecospace to access this data.
    'Ecosim must have a scenario loaded!!!! This will have to be handled by the core
    Private m_EPdata As cEcopathDataStructures
    Private m_SimData As cEcosimDatastructures
    Private m_Data As cEcospaceDataStructures
    Private m_Stanza As cStanzaDatastructures
    Private m_Ecosim As EcoSim.cEcoSimModel
    Private m_search As cSearchDatastructures
    '  Private m_indic As Ecosim.cEcosimIndicies
    Private m_tracerData As cContaminantTracerDataStructures
    Private m_OptMPA As IMPASearchModel

    Private m_refdata As cEcospaceTimeSeriesDataStructures

    Private m_StopRun As Boolean

    'habitat (preference function is habgrad, which has max value of habbest, 90% drop in movment if slope=-2 given movescale=1
    'habbest could be a constant
    Private HabBest As Integer
    'size of the window (number of cells) to compute the habitat gradients over see SetHabGrad()
    'iWindow could be a constant
    Private iWindow As Integer

    Private HabGrad(,,) As Single
    ' Private NcellsHab() As Integer

    'the analog of habgrad, but for migration, and has a monthly component
    Private MigGrad(,,,) As Single


    Private TotEffort() As Single

    Private RelMoveFit(,) As Single 'populated in SetKmove()
    Private PzoTOmove() As Single 'populated in SetKmove()
    Private Kmovefit() As Single 'populated in SetKmove()
    Private RelFitness(,,) As Single

    Friend FtimeCell(,,) As Single 'feeding time???
    Private HdenCell(,,) As Single

    ''' <summary>Sum of Biomass for all the cells in the current time step </summary>
    Private Btime() As Single

    ''' <summary>Sum of cCell (contaminant)for all the cells in the current time step </summary>
    Private ConTotal() As Single
    Private MinChange As Single

    Private MigPowi(,) As Single
    Private MigPowj(,) As Single
    Private PrefRowP(,) As Single, PrefColP(,) As Single

    'these are now in the data structure
    'Private Vspace() As Single 'vulnerabilities set to same values as ecosim during initialization
    'Private Aspace() As Single 'search rate set to same values as ecosim during initialization
    Private PbSpace() As Single ' P/B from Ecopath

    Private der() As Single

    Private Basebiomass() As Single

    Private Flowin() As Single
    Private FlowoutRate() As Single

    'A() searchrate modifer one if in prefered habitate < 1 otherwise used in derivRed() to calculate effective search rate
    'repopulated for each time step each cell
    Private EatEff() As Single
    '   'V() modifier used in the same way as EatEff() to modfy effective vulnerability in derivtRed()
    Private VulPred() As Single

    Private Tstanza() As Single
    ' Private conSplit() As Single ' pred()/NstanzaBase()
    Private NstanzaBase() As Single
    Private RecSplit() As Single
    Private PconSplit() As Single

    ''' <remarks>
    ''' EwE5 loss() is global and the same variable is used for both Ecosim and Ecospace
    ''' EwE6 loss for Ecosim in declared in cEcosimDataStructres so that it can be used to initialize Wchange() in Ecospace
    ''' loss for Ecospace is private and computed in DerivtRed()
    ''' So if you need the loss from Derivt() use cEcoSimDataStructres.loss() not cEcoSpace.loss()!!!!!!
    ''' </remarks>
    Private loss() As Single


    Private pbb() As Single

    'movement parameters use for SolveGrid()
    'computed in SetMovementParameters() 
    Private Bcw(,,) As Single
    Private C(,,) As Single
    Private d(,,) As Single
    Private e(,,) As Single

    Private AMm(,,) As Single
    Private F(,,) As Single

    Private BEQlast(,,) As Single 'equilibrium biomass at the last timestep

    Private TimeStep2 As Single

    ' Dim Tn As Integer ' summary array index

    'jb Movement parameter with no migration?????
    'Set in SetMovementParameters() to the same values as counterparts BcwNomig() = Bcw()
    Private BcwNomig(,,) As Single
    Private CNomig(,,) As Single
    Private dNomig(,,) As Single
    Private Enomig(,,) As Single


    ''' <summary>
    ''' Converts an iGroup into a cumulative stanza index Nvarsplit
    ''' </summary>
    ''' <remarks>Populated in initSpatialEqulibrium().  
    ''' Use to access stanza varaibles that are stored after the groups in biologial indexes of spatial matrixes.
    ''' </remarks>
    Private IecoCode() As Integer

    ''' <summary>
    ''' Total number of time step
    ''' </summary>
    ''' <remarks>set in redimForRun()</remarks>
    Private nEcospaceTimeSteps As Integer

    ''' <summary>
    ''' This is the index to the imonth for data arrayed by month i.e. zscale()
    ''' </summary>
    ''' <remarks>If the user has set the Ecospace time step to some value other than monthly this index will point to the first month of the time step.
    ''' For example timestep = 0.5 first loop itt = 1 second loop itt = 7</remarks>
    Private its As Integer

    ''' <summary>
    ''' Cumulative itime step at the current user selected time step.
    ''' </summary>
    ''' <remarks></remarks>
    Private itt As Integer


    Private HabAreaUsed() As Single

    ''' <summary>
    ''' Total number of habitat area cells
    ''' Any cell with a depth > 0 of any habitat type
    ''' </summary>
    ''' <remarks>computed in CalcHabitatArea()</remarks>
    Public ThabArea As Single

    'timers
    Private gridThreadWaitTimer As Single
    Private ibmThreadWaitTimer As Single
    Private ibmThreadWaitTimer2 As Single
    Private spaceThreadWaitTimer As Single

    Private totalIterThread() As Integer 'total number of solvegrid iterations for each thread

    'grid solver for contaminant the tracer
    Private grdslvConSim As cGridSolver
    Private m_ConBypassIntegrated() As Boolean

    Private nMigratory As Integer 'total number of migratory variables that are solved for
    Private migratoryIndex() As Integer 'the ip index of the migratory species
    Private nGroupsInThread() As Integer 'number of groups solved in gridsolver by each thread
    Private threadGroups(,) As Integer 'the ip indices solved by each thread

    Private threadGroupsConSim(,) As Integer

    Private m_SpaceCatchSemaphor As Semaphore

    'Private m_FleetSums As New List(Of cSpaceFleetSummary)
    'Private m_FleetSum As cSpaceFleetSummary

#End Region

#Region "Varaibles from FindSpatialEqulibrium()"

    'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
    'Variables that where local to FindSpatialEqulibrium() in EwE5
    'moved to the level of the class so that FindSpatialEqulibrium() could be split up into smaller pieces
    'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
    ''' <summary>
    ''' Primary Production Scaler
    ''' </summary>
    ''' <remarks>computed by ScaleRelativePrimaryProductivityToEcopathLevel() set in InitSpatialEquilibrium. 
    ''' In EwE5 this was local to FindSpatialEquilibrium. Here it has been move up in scope so that FindSpatialEquilibrium() can be split up into components.
    ''' Init (InitSpatialEquilibrium), run (FindSpatialEquilibrium) ......
    ''' </remarks>
    Private PPScale As Single

    Private jord(1000) As Integer

    Private Cper(,,) As Single

    ''' <summary>
    ''' Converts a cumulative stanza index(Nvarsplit) into an iGroup index
    ''' </summary>
    ''' <remarks>Computed in initSpatialEquilibrium(). 
    ''' This is the opposite of IecoCode().
    ''' </remarks>
    Private Ecode() As Integer

    ''' <summary>
    ''' 1/StartBiomass of oldest stanza for this split
    ''' </summary>
    ''' <remarks>RelRepStanza(Nsplit)</remarks>
    Dim RelRepStanza() As Single

    ''' <summary>
    ''' Index of the first element after the end of the groups
    ''' </summary>
    ''' <remarks>This is used for the split group indexes that are stored after the end groups for arrays that are dimensioned by nTotVars</remarks>
    Dim nvar2 As Integer

#End Region

#Region "Public Properties"

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Exposes the MessagePublisher instance so that the core can add message handlers
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Messages() As cMessagePublisher
        Get
            Return m_publisher
        End Get
    End Property

    Public Property PluginManager() As cPluginManager
        Get
            Return Me.m_pluginManager
        End Get
        Set(ByVal pm As cPluginManager)
            Me.m_pluginManager = pm
        End Set
    End Property

    ''' <summary>
    ''' Ecopath data used for initial state
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property EcoPathData() As cEcopathDataStructures
        Get
            Return m_EPdata
        End Get
        Set(ByVal value As cEcopathDataStructures)
            m_EPdata = value
        End Set
    End Property


    ''' <summary>
    ''' Ecosim data used for initial state
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property EcoSimData() As cEcosimDatastructures
        Get
            Return m_SimData
        End Get
        Set(ByVal value As cEcosimDatastructures)
            m_SimData = value
        End Set
    End Property

    Public Property EcoSpaceData() As cEcospaceDataStructures
        Get
            Return m_Data
        End Get
        Set(ByVal value As cEcospaceDataStructures)
            m_Data = value
        End Set
    End Property

    Public Property StanzaData() As cStanzaDatastructures
        Get
            Return m_Stanza
        End Get
        Set(ByVal value As cStanzaDatastructures)
            m_Stanza = value
        End Set
    End Property

    Public Property EcoSim() As Ecosim.cEcoSimModel
        Get
            Return m_Ecosim
        End Get
        Set(ByVal value As Ecosim.cEcoSimModel)
            m_Ecosim = value
        End Set
    End Property

    Public Property ContaiminantTracerData() As cContaminantTracerDataStructures
        Get
            Return m_tracerData
        End Get
        Set(ByVal value As cContaminantTracerDataStructures)
            m_tracerData = value
        End Set
    End Property

    Public Property TimeSeriesData() As cEcospaceTimeSeriesDataStructures
        Get
            Return m_refdata
        End Get
        Set(ByVal newValue As cEcospaceTimeSeriesDataStructures)
            m_refdata = newValue
        End Set
    End Property


    Public Property TimeStepDelegate() As EcoSpaceTimeStepDelegate
        Get
            Return Me.m_TimestepDelegate
        End Get
        Set(ByVal value As EcoSpaceTimeStepDelegate)
            m_TimestepDelegate = value
        End Set
    End Property


    Public Property StopRun() As Boolean
        Get
            Return m_StopRun
        End Get
        Set(ByVal value As Boolean)
            m_StopRun = value
        End Set
    End Property

    Public Property SearchData() As cSearchDatastructures
        Get
            Return m_search
        End Get
        Set(ByVal value As cSearchDatastructures)
            m_search = value
        End Set
    End Property

    Public Property MPAOptimization() As IMPASearchModel
        Get
            Return m_OptMPA
        End Get
        Set(ByVal value As IMPASearchModel)
            m_OptMPA = value
        End Set
    End Property


#End Region

#Region "Initialization"

    ''' <summary>
    ''' Initialize base varaibles with the default values
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>Was MPAMoveLoadFormTasks() in EwE5. Most of this was moved to cEcoSpaceDataStructures.SetDefaults() </remarks>
    Public Function InitToDefaults() As Boolean
        Dim i As Integer

        Try

            Debug.Assert(m_Data IsNot Nothing, Me.ToString & ".Init() Data not initialized.")
            Debug.Assert(m_EPdata IsNot Nothing, Me.ToString & ".Init() Data not initialized.")
            Debug.Assert(m_SimData IsNot Nothing, Me.ToString & ".Init() Data not initialized.")
            Debug.Assert(m_Ecosim IsNot Nothing, Me.ToString & ".Init() Data not initialized.")
            Debug.Assert(m_Stanza IsNot Nothing, Me.ToString & ".Init() Data not initialized.")

            'set parameters used to define habitat gradient functions and strength of response to gradients toward desired
            'habitat (preference function is habgrad, which has max value of habbest, 90% drop in movment if slope=-2 given movescale=1
            HabBest = 10
            iWindow = 5

            'was in FindSpatialEquilibrium()
            MinChange = 0.3

            m_Data.W = 1.2
            m_Data.Tol = 0.0001
            m_Data.maxIter = 40

            m_Stanza.NPacketsMultiplier = 0.5
            'm_Data.NewMultiStanza = True
            m_Data.UseIBM = True
            m_Data.TimeStep = CSng(1 / 12) 'one month

            'this should be available to users in interface, higher values typically cause
            'instability in spatial allocation (IFD) model for multistanza biomass distributions
            m_Data.IFDPower = 0.5 'this isn't actually used anymore

            'nvartot
            ReDim IecoCode(m_Data.NGroups)

            'compute the IecoCode() index
            'this index pointer is unique to Ecospace
            'this will need to be re-computed if the number of groups or stanzas change
            Dim ir As Integer, igrp As Integer
            For i = 1 To m_Stanza.Nsplit
                For j As Integer = 1 To m_Stanza.Nstanza(i)
                    ir = ir + 1
                    igrp = m_Stanza.EcopathCode(i, j)
                    IecoCode(igrp) = ir
                Next
            Next

            Me.m_SpaceCatchSemaphor = New System.Threading.Semaphore(1, 1, "EcoSpaceMontlyCatch")

            Return True

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".Init() Error: " & ex.Message)
            Return False
        End Try


    End Function

#End Region

#Region "Public methods"

    Public Function Run() As Boolean
        Dim bsuccess As Boolean = True
        Try

            'redim all 
            If redimForRun() Then

                Me.initSpatialEquilibrium()
                Me.FindSpatialEquilibrium()

            Else
                bsuccess = False
            End If

        Catch ex As Exception
            Debug.Assert(False, ex.Message)
            m_publisher.AddMessage(New cMessage("Ecospace Error: " & ex.Message, eMessageType.ErrorEncountered, _
                                        eCoreComponentType.EcoSpace, eMessageImportance.Critical, eDataTypes.NotSet))
            bsuccess = False
        End Try

        m_publisher.sendAllMessages()

        Return bsuccess

    End Function

#End Region

#Region "Private modeling code"

    Private Sub FindSpatialEquilibrium()
        'this routine attempts to seek spatial equilibrium in ecosim biomasses, given mpa pattern
        'and start density map based on no movement
        Dim iYear As Integer
        Dim imonth As Integer
        Dim i As Integer
        Dim j As Integer
        Dim ip As Integer
        Dim BB() As Single
        Dim ebb() As Single 'abmpa
        Dim Wtr As Single

        Dim steps_per_year As Integer = 1 / m_Data.TimeStep
        Dim bAccumulateData As Boolean

        Dim RelFopt() As Single
        Dim Fgear() As Single

        Dim slvrTimer As Single
        Dim spaceTimer As Single
        Dim timeStepTimer As Single
        Dim IBMTimer As Single
        gridThreadWaitTimer = 0
        ibmThreadWaitTimer = 0
        ibmThreadWaitTimer2 = 0
        spaceThreadWaitTimer = 0

        'used for timing threaded code
        Dim slvET2 As Single
        Dim slvET As Single

        Dim FtimeTotal(m_Data.NGroups) As Single

        Dim ExtraTime As Integer = m_search.ExtraYearsForSearch

        Try

            ReDim Fgear(m_EPdata.NumFleet)
            ReDim RelFopt(1)
            'stanza counters
            'nvar2 is an index that counts from the end of the groups up to cEcoSpaceDataStructures.nvartot = nGroups + NSplit(nvartot = [total number of groups] + [sum of all split groups])
            'it is used for stanza data that is stored after groups (any variable that is dimed by nvartot)
            nvar2 = m_Data.NGroups

            iTotalCells = m_Data.InCol * m_Data.InRow

            ReDim ebb(m_Data.nvartot)
            ReDim BB(m_Data.nvartot)

            Dim tTimeLoop As Double
            Dim bdump(,) As Single
            ReDim bdump(m_Data.InRow, m_Data.InCol)

            If m_Data.UseIBM Then InitPackets()
            m_Data.nIBMPacketsPerThread = (m_Stanza.Npackets + m_Data.nGridSolverThreads - 1) \ m_Data.nGridSolverThreads

            tTimeLoop = Microsoft.VisualBasic.Timer '* 1000
            '  System.Console.WriteLine("Ecospace Start Temporal Spatial loop")
            itt = 0

            If m_search.bInSearch Then
                m_search.initForRun(Me.m_EPdata, Me.m_SimData)
                m_search.setBaseYearEffort(Me.m_SimData)
            End If

            Dim StartTime As Single = 0
            If m_OptMPA IsNot Nothing Then
                If m_OptMPA.isRunning Then
                    StartTime = m_OptMPA.EcospaceStartTime
                End If
            End If
            'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
            'START OF TIME LOOP
            'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
            For m_Data.TimeNow = StartTime To (m_Data.TotalTime - m_Data.TimeStep) Step m_Data.TimeStep
                '            'SetBoundaryB

                'set time step counters
                its = CInt(m_Data.TimeNow * 12) + 1 ' i time assuming a monthly time step used for data array by month i.e. zscale()
                itt += 1 'cumulative i time at the curent time step 

                Debug.Assert(itt <= nEcospaceTimeSteps, "itt > nEcospaceTimeSteps")

                'make sure the time loop indexes do not get larger then the data they reference
                If itt > nEcospaceTimeSteps Then itt = nEcospaceTimeSteps
                If its > m_SimData.ForcePoints Then its = m_SimData.ForcePoints 'HACK  bump back the index

                imonth = 1 + (its - 1) Mod 12
                iYear = 1 + Math.Truncate(m_Data.TimeNow + 0.001)  'iYear will be truncated to the integer part of timenow
                If imonth = 1 Then
                    bAccumulateData = True 'new year collect the model fitting data after the six month
                End If

                'Tell Ecoseed that we are at the start of a timestep
                Me.EcoseedBeginTimeStep(imonth, iYear, Btime)

                'Ecospace has been stopped
                If Me.m_StopRun Then
                    Exit For
                End If

                'do any external processing at the start of the time step
                BeginTimeStep(Fgear, its, imonth, iYear, Btime, RelFopt, m_Data.TimeNow)

                If m_search.bInSearch Then
                    For i = 1 To m_EPdata.NumFleet
                        If m_search.FblockCode(i, iYear) > 0 Then
                            m_SimData.FishRateGear(i, its) = Fgear(i)
                        End If
                        m_SimData.FishRateGear(i, 0) = Fgear(i) 'm_Data.FishRateGear(i, itime)
                    Next
                End If

                '********************Martell******************
                'This is for monthy current vectors.
                If m_Data.CurrentForce Then
                    For i = 0 To m_Data.InRow + 1
                        For j = 0 To m_Data.InCol + 1
                            'jb Xv() are dimmed when the current field are read in
                            'which is not happening yet so if this crashes that is probable the problem
                            m_Data.Xvel(i, j) = m_Data.Xv(i, j, imonth)
                            m_Data.Yvel(i, j) = m_Data.Yv(i, j, imonth)
                        Next j
                    Next i

                    'ToDo_jb FindSpatial....... velmaker
                    'Calculate Upwelling indicies
                    SM_MapApparentUpwell(m_Data.Xvel, m_Data.Yvel)
                    SetMovementParameters()
                End If
                '****************END of Martell*****************
                'CJW moved imonth line 2/10/03 to here, before varymovementparameters call
                'ToDo_jb FindSpatial... Saving Time series data
                '            'Save Timeseries data when at half a year
                '            'VC060519: Trying to save at first month to use when not monthly time step
                'jb storing of the time series data still  needs to be implemented
                '            StoreTimeSeriesData = IIf(imonth = 1 And SpDatYear > 0, True, False)

                VaryMovementParameters2(imonth)
                'If useMigratoryGrad Then
                'VaryMigMovementParameters(imonth)
                'Else
                'For ip = 1 To m_Data.NGroups
                '    If m_Data.IsMigratory(ip) Then
                '        VaryMovementParameters(imonth, ip, IecoCode(ip))
                '    End If
                'Next
                'End If

                'set tval() (time step forcing value) to the value for this time step for each forcing shape
                'Time forcing function are disable in EcoSpace via ApplyAVmodifiers() "UseTime" flag
                'If ApplyAVmodifiers() is called with the UseTime = True then the time forcing function will be used
                For i = 0 To m_SimData.ForcingShapes
                    m_SimData.tval(i) = m_SimData.zscale(its, i)
                Next

                'ToDo_jb EggProdShapeSplit() make sure this is correct
                'set current relative reproductive rates for stanzas groups
                For i = 1 To m_Stanza.Nsplit
                    If m_Stanza.EggProdShapeSplit(i) > 0 Then
                        RelRepStanza(i) = m_SimData.tval(m_Stanza.EggProdShapeSplit(i)) * m_Stanza.RscaleSplit(i) / m_SimData.StartBiomass(m_Stanza.EcopathCode(i, m_Stanza.Nstanza(i)))
                    End If
                Next

                ''TN is a pointer being used to decide which sum to work with
                ' SetSummaryTimeStep(Tn)

                If m_Data.PredictEffort Then PredictEffortDistribution(imonth, its)

                If m_pluginManager IsNot Nothing Then m_pluginManager.EcospacePostFishingEffortModTimestep(m_Data, m_Data.TimeNow)

                ReDim Btime(m_Data.NGroups) 'this clears out btime
                ReDim ConTotal(m_Data.NGroups)

                '*************
                'UPDATE SOLVERS WITH NON REFERENCED TIMESTEP DATA (itt, etc)
                '*************
                UpdateSpaceSolverThreads(iYear)

                slvET2 = Microsoft.VisualBasic.Timer
                'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
                'Run the biomass calculation for each spatial cell at this time step
                'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
                'System.Console.WriteLine(m_Data.TimeNow.ToString)
                runSpaceSolverThreads()
                spaceTimer = spaceTimer + (Microsoft.VisualBasic.Timer - slvET2)

                slvET = Microsoft.VisualBasic.Timer
                'now solve the spatial grid
                runGridSolverThreads()
                slvrTimer = slvrTimer + (Microsoft.VisualBasic.Timer - slvET)

                'make sure none of the biomass cells are zero
                For ip = 1 To m_Data.nvartot
                    For i = 1 To m_Data.InRow
                        For j = 1 To m_Data.InCol
                            If m_Data.Bcell(i, j, ip) < 1.0E-30 Then m_Data.Bcell(i, j, ip) = 1.0E-30
                        Next j
                    Next i
                Next 'For ip = 1 To m_Data.nvartot


                'summarizeTimeStepData(itt, imonth, Tn)

                'Dim slvET3 As Single = Microsoft.VisualBasic.Timer

                ''post notification that a time step has been completed
                'processTimeStep(itt)
                'timeStepTimer = timeStepTimer + (Microsoft.VisualBasic.Timer - slvET3)

                'For ip = 1 To m_Data.nvartot
                '    '********following will bypass unneeded solvegrid calls when NewMultiStanza=true
                '    If m_Data.ByPassIntegrate(ip) = False Then
                '        SolveGrid(ip, AMm, F, m_Data.Bcell, m_Data.Inrow, m_Data.InCol, Tol, jord, m_Data.W)
                '        SolveGridRow(ip, AMm, F, m_Data.Bcell, m_Data.Inrow, m_Data.InCol, Tol, jord, m_Data.W)
                '        For i = 1 To m_Data.Inrow : For j = 1 To m_Data.InCol
                '                If m_Data.Bcell(i, j, ip) < 1.0E-30 Then m_Data.Bcell(i, j, ip) = 1.0E-30
                '            Next : Next
                '    End If
                'Next

                'update total age structure over space for multistanza groups if new method is used
                If m_Data.NewMultiStanza Then
                    SpaceSplitUpdate()  'update overall population age structure using total loss, consumption added over grid cells
                    'then distribute updated biomasses over the spatial grid
                    'The following code (for isp...next isp) is real Rambo shit, really should be put
                    'in its own subroutine called 'DistributeMultiStanzaBiomass' so we
                    'can improve it later with more complex spatial redistribution
                    'rules eg running an IBM to predict movement among cells
                    Dim ieco As Integer
                    For isp As Integer = 1 To m_Stanza.Nsplit
                        For ist As Integer = 1 To m_Stanza.Nstanza(isp)
                            ieco = m_Stanza.EcopathCode(isp, ist)
                            '***WARNING**** FOLLOWING CALCULATION WILL FAIL IF ADJUSTSPACEPARS HAS NOT BEEN CALLED
                            'SINCE CALCTOTAREA WILL NOT HAVE BEEN CALLED AND NEITHER THABAREA OR HABAREAUSED WILL HAVE BEEN
                            'SET
                            Tbiom = (ThabArea) * Blocal(ieco)  'B has been updated in spacesplitupdate at this point
                            Tpred = (ThabArea) * m_SimData.pred(ieco)  'pred has been updated by call to splitsetpred in spacesplitupdate
                            For i = 1 To m_Data.InRow : For j = 1 To m_Data.InCol
                                    If (m_Data.PrefHab(ieco, m_Data.HabType(i, j)) = True _
                                        Or m_Data.PrefHab(ieco, 0) = True) _
                                        And m_Data.DistributionEnvelope(i, j, ieco) = True _
                                        And m_Data.Depth(i, j) > 0 Then
                                        Wcell = m_Data.IFDweight(i, j, ieco) / TotIFDweight(ieco)
                                        m_Data.Bcell(i, j, ieco) = Tbiom * Wcell
                                        m_Data.PredCell(i, j, ieco) = Tpred * Wcell
                                    End If
                                Next : Next
                        Next
                    Next
                ElseIf m_Data.UseIBM Then
                    slvET2 = Microsoft.VisualBasic.Timer
                    runIBMSolverThreads()
                    IBMTimer = IBMTimer + (Microsoft.VisualBasic.Timer - slvET2)
                End If 'end of section to overwrite PDE biomasses with multistanza distributed biomasses if newmultistanza=true

                'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
                'contaminant tracing
                If m_tracerData.EcoSpaceConSimOn Then
                    Me.grdslvConSim.FirstLastGroups(0, m_EPdata.NumGroups)
                    'the grid solver has already been initialized with a reference to the contaminant tracing data
                    Me.grdslvConSim.Solve(Nothing)

                    ReDim m_tracerData.ConcMax(m_EPdata.NumGroups)

                    For i = 1 To m_Data.InRow
                        For j = 1 To m_Data.InCol
                            For ip = 0 To m_Data.NGroups
                                'If SpaceTime = False Then Wtr = Exp(AMmTr(i, j, ip) * TimeStep) Else Wtr = 0
                                Wtr = Math.Exp(m_Data.AMmTr(i, j, ip) * m_Data.TimeStep)
                                m_Data.Ccell(i, j, ip) = Wtr * m_Data.Clast(i, j, ip) + (1 - Wtr) * m_Data.Ccell(i, j, ip)
                                m_Data.Clast(i, j, ip) = m_Data.Ccell(i, j, ip)

                                If m_Data.Ccell(i, j, ip) > m_tracerData.ConcMax(ip) Then m_tracerData.ConcMax(ip) = m_Data.Ccell(i, j, ip)

                                m_tracerData.TracerConcByRegion(m_Data.Region(i, j), ip, itt) = m_tracerData.TracerConcByRegion(m_Data.Region(i, j), ip, itt) + m_Data.Ccell(i, j, ip)
                                m_tracerData.TracerCBRegion(m_Data.Region(i, j), ip, itt) = m_tracerData.TracerCBRegion(m_Data.Region(i, j), ip, itt) + m_Data.Ccell(i, j, ip) / m_Data.Bcell(i, j, ip)
                                'sum of concentration by region
                                'ewe5
                                'If StoreTimeSeriesData Then
                                '    SpaceTraceByRegion(iYear, ip, 0) = SpaceTraceByRegion(iYear, ip, 0) + Ccell(i, j, ip)
                                '    SpaceTraceByRegion(iYear, ip, Region(i, j)) = SpaceTraceByRegion(iYear, ip, Region(i, j)) + Ccell(i, j, ip)
                                '    SpaceTraceByRegionCount(iYear, ip, 0) = SpaceTraceByRegionCount(iYear, ip, 0) + 1
                                '    SpaceTraceByRegionCount(iYear, ip, Region(i, j)) = SpaceTraceByRegionCount(iYear, ip, Region(i, j)) + 1
                                'End If

                            Next
                        Next
                    Next
                End If
                'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

                If imonth >= 6 And bAccumulateData Then
                    'make sure AccumulateDataInfo only gets called once a year
                    'if the user has set the time step to a value other the one month imonth may never = 6 or it may = 6 for multiple time steps

                    'jb loss needs to change to average loss over all the water cells
                    m_Ecosim.AccumulateDataInfo(iYear, Btime, loss)
                    bAccumulateData = False
                End If

                summarizeTimeStepData(itt, imonth)

                If m_search.bInSearch And iYear = m_search.BaseYear And imonth = 12 Then
                    m_search.calcBaseYearCost(iYear, m_Data.nWaterCells)
                End If

                Dim slvET3 As Single = Microsoft.VisualBasic.Timer

                'post notification that a time step has been completed
                onTimeStep(itt)
                timeStepTimer = timeStepTimer + (Microsoft.VisualBasic.Timer - slvET3)

                If m_pluginManager IsNot Nothing Then m_pluginManager.EcospaceEndTimeStep(m_Data, m_Data.TimeNow)

            Next m_Data.TimeNow
            'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
            'END OF TIME LOOP
            'xxxxxxxxxxxxxxxxxxxxxxxxxxxx

            Me.m_Data.AverageSpatialResults()
            Me.m_Data.SummarizeResults(Me.m_EPdata.cost, Me.m_search.Jobs)

            If m_search.bInSearch Then
                Dim runTime As Integer = CInt(itt * m_Data.TimeStep)
                Dim RuntimePB As Integer = runTime
                If m_search.BaseYear > m_OptMPA.EcospaceStartTime Then RuntimePB = m_OptMPA.MPAOptData.EcoSpaceEndYear - m_search.BaseYear
                m_search.EcoSpaceSummarizeIndicators(Fgear, runTime, RuntimePB, m_Data.nWaterCells)
            End If

            Dim SpaceSS As Single
            'If SpDatYear > 0 Then 'there is time series data so calculate SS SpSS
            SpaceSS = CalculateSpaceSS()
            'End If

            m_Ecosim.PlotDataInfo(False, m_Data.SS)

            Dim totalIter As Single
            For i = 1 To m_Data.nGridSolverThreads
                totalIter = totalIter + totalIterThread(i)
            Next

            'System.Console.WriteLine("FindSpatialEquilibrium() Number of Time Steps " & itt.ToString)
            'System.Console.WriteLine("FindSpatialEquilibrium() Run Time = " & CStr(Microsoft.VisualBasic.Timer - tTimeLoop))
            'System.Console.WriteLine("FindSpatialEquilibrium() GridSolver Run Time = " & slvrTimer.ToString)
            'System.Console.WriteLine("FindSpatialEquilibrium() SpaceSolver Run Time = " & spaceTimer.ToString)
            'System.Console.WriteLine("FindSpatialEquilibrium() processTimeStep Run Time = " & timeStepTimer.ToString)
            'System.Console.WriteLine("FindSpatialEquilibrium() IBMsolver Run Time = " & IBMTimer.ToString)
            'System.Console.WriteLine("FindSpatialEquilibrium() Grid Thread waiting Time = " & gridThreadWaitTimer.ToString)
            'System.Console.WriteLine("FindSpatialEquilibrium() Space Thread waiting Time = " & spaceThreadWaitTimer.ToString)
            'System.Console.WriteLine("FindSpatialEquilibrium() IBM1 Thread waiting Time = " & ibmThreadWaitTimer.ToString)
            'System.Console.WriteLine("FindSpatialEquilibrium() IBM2 Thread waiting Time = " & ibmThreadWaitTimer2.ToString)
            'System.Console.WriteLine("FindSpatialEquilibrium() SolveGrid total iterations = " & totalIter.ToString)
            'Dim solver As cGridSolver
            'For Each solver In m_gridSolvers
            '    System.Console.WriteLine("Gridsolver Thread " + solver.ThreadID.ToString + " = " + solver.threadTime.ToString)
            'Next
            'If m_Data.UseIBM Then
            '    Dim ibmsolver As cIBMSolver
            '    For Each ibmsolver In m_IBMSolvers
            '        System.Console.WriteLine("IBM1 Thread " + ibmsolver.ThreadID.ToString + " = " + ibmsolver.threadTime1.ToString)
            '    Next
            '    For Each ibmsolver In m_IBMSolvers
            '        System.Console.WriteLine("IBM2 Thread " + ibmsolver.ThreadID.ToString + " = " + ibmsolver.threadTime2.ToString)
            '    Next
            '    For Each ibmsolver In m_IBMSolvers
            '        System.Console.WriteLine("IBM MovePackets Thread " + ibmsolver.ThreadID.ToString + " = " + ibmsolver.threadTimeMove.ToString)
            '    Next
            'End If
        Catch ex As Exception
            Debug.Assert(False, ex.Message)
            Throw New ApplicationException("FindSpatialEquilibrium() Error: " & ex.Message, ex)
        End Try

    End Sub


    Private Sub BeginTimeStep(ByRef Fgear() As Single, ByVal its As Integer, ByVal imonth As Integer, ByRef iYear As Integer, ByRef BiomassCellAvg() As Single, ByVal relfopt() As Single, ByVal TimeStep As Single)
        Try
            Dim nYears As Integer = CInt(m_Data.TotalTime)

            If m_pluginManager IsNot Nothing Then m_pluginManager.EcospaceBeginTimeStep(m_Data, m_Data.TimeNow)

            If imonth = 1 Then
                'if we are in the first month then this is a new year
                If m_search.bInSearch Then
                    'YearTimeStepEcoSpace() will compute DF, Fgear(), NetCost(), and FishYear() for this year step

                    m_search.YearTimeStepEcoSpace(BiomassCellAvg, Fgear, iYear, m_Data.nWaterCells, relfopt)
                    m_search.calcNetCost(Fgear, iYear)
                    m_search.calcYearlySummaryValues(BiomassCellAvg)

                End If

                'tell all the space solver threads that a new year has started
                InitSolversForYear(iYear)

            End If

        Catch ex As Exception
            Debug.Assert(False, ex.StackTrace)
            cLog.Write(ex)
            Throw New ApplicationException("EcoSpace.BeginTimeStep() error: " & ex.Message, ex)
        End Try
    End Sub

    Private Sub EcoseedBeginTimeStep(ByVal imonth As Integer, ByRef iYear As Integer, ByRef BiomassCellAvg() As Single)

        If m_OptMPA IsNot Nothing Then

            If m_OptMPA.isRunning Then
                'if we are in the first month then this is a new year
                If imonth = 1 Then
                    'Call Ecoseed at the start of each year
                    'On the first call EcoSeed will
                    'set iYear to the user defined Start year 
                    'populate BiomassCellAvg() with biomass values for the start year calculated by Ecospace during Ecoseed initialization
                    'If iYear = Ecoseed end year then it will set Ecospace.StopRun to true 
                    'this will cause the time loop in Ecospace to exit
                    m_OptMPA.YearTimeStep(iYear, Btime)
                End If
            End If

        End If

    End Sub


    Private Sub runIBMSolverThreads()
        Dim solver As cIBMSolver
        Dim iFstGrp As Integer
        Dim iLstgrp As Integer
        Dim iFirstPacket As Integer
        Dim iLastPacket As Integer

        iFstGrp = 1
        iLstgrp = 0
        iFirstPacket = 1
        iLastPacket = 0

        Dim solvCtr As Integer = 1
        Dim timerTemp As Single

        For Each solver In m_IBMSolvers
            ReDim solver.BcellThread(m_Data.InRow, m_Data.InCol, m_Data.nvartot)
            ReDim solver.PredCellThread(m_Data.InRow, m_Data.InCol, m_Data.nvartot)
        Next
        ReDim m_Stanza.EggCell(m_Data.InRow, m_Data.InCol, m_Stanza.Nsplit)

        Try


            'this loop should only excecute once
            Do While iLstgrp < m_Stanza.Nsplit
                'loop through each solver object, make sure it's okay to run, and run it
                'each thread will do several groups at a time
                For Each solver In m_IBMSolvers

                    If solver.isOkToRun Then

                        iLstgrp = iFstGrp + m_Data.nIBMGroupsPerThread - 1
                        If iLstgrp > m_Stanza.Nsplit Then iLstgrp = m_Stanza.Nsplit

                        solver.FirstLastGroups(iFstGrp, iLstgrp)
                        solver.SignalState.Reset()

                        solver.isOkToRun = False
                        ThreadPool.QueueUserWorkItem(AddressOf solver.SolveFirst)

                        iFstGrp += m_Data.nIBMGroupsPerThread
                    Else
                        'System.Console.WriteLine("Solver thread blocked ID:" & solver.ThreadID & " Group:" & solver.iFirstIndex & " time:" & m_Data.TimeNow)
                    End If

                    If iLstgrp >= m_Stanza.Nsplit Then
                        Exit For
                    End If
                Next solver
            Loop

            ' wait for all the threads to finish before starting the next time step
            For Each solver In m_IBMSolvers
                If solvCtr = 2 Then timerTemp = Microsoft.VisualBasic.Timer
                solver.SignalState.WaitOne()
                solvCtr = solvCtr + 1
            Next

            If solvCtr = 2 Then timerTemp = Microsoft.VisualBasic.Timer
            solvCtr = 1
            ibmThreadWaitTimer = ibmThreadWaitTimer + (Microsoft.VisualBasic.Timer - timerTemp)

            Dim ieco As Integer
            For isp As Integer = 1 To m_Stanza.Nsplit
                For ist As Integer = 1 To m_Stanza.Nstanza(isp)

                    ieco = m_Stanza.EcopathCode(isp, ist)
                    For i As Integer = 1 To m_Data.InRow : For j As Integer = 1 To m_Data.InCol
                            m_Data.Bcell(i, j, ieco) = 0
                            m_Data.PredCell(i, j, ieco) = 0
                        Next : Next
                Next
            Next

            'this loop should only excecute once
            Do While iLastPacket < m_Stanza.Npackets
                'loop through each solver object, make sure it's okay to run, and run it
                'each thread will do several groups at a time


                For Each solver In m_IBMSolvers

                    If solver.isOkToRun Then

                        iLastPacket = iFirstPacket + m_Data.nIBMPacketsPerThread - 1
                        If iLastPacket > m_Stanza.Npackets Then iLastPacket = m_Stanza.Npackets

                        'solver.FirstLastGroups(iFstGrp, iLstgrp)
                        solver.iFirstPacket = iFirstPacket
                        solver.iLastPacket = iLastPacket
                        solver.SignalState.Reset()

                        solver.isOkToRun = False
                        ThreadPool.QueueUserWorkItem(AddressOf solver.Solve)

                        iFirstPacket += m_Data.nIBMPacketsPerThread
                    Else
                        'System.Console.WriteLine("Solver thread blocked ID:" & solver.ThreadID & " Group:" & solver.iFirstIndex & " time:" & m_Data.TimeNow)
                    End If

                    If iLastPacket >= m_Stanza.Npackets Then
                        Exit For
                    End If
                Next solver
            Loop

            ' wait for all the threads to finish before starting the next time step
            For Each solver In m_IBMSolvers
                If solvCtr = 2 Then timerTemp = Microsoft.VisualBasic.Timer
                solver.SignalState.WaitOne()
                solvCtr = solvCtr + 1
            Next
            If solvCtr = 2 Then timerTemp = Microsoft.VisualBasic.Timer
            ibmThreadWaitTimer2 = ibmThreadWaitTimer2 + (Microsoft.VisualBasic.Timer - timerTemp)

            For Each solver In m_IBMSolvers
                For isp As Integer = 1 To m_Stanza.Nsplit
                    For ist As Integer = 1 To m_Stanza.Nstanza(isp)
                        ieco = m_Stanza.EcopathCode(isp, ist)
                        For i As Integer = 1 To m_Data.InRow : For j As Integer = 1 To m_Data.InCol
                                m_Data.Bcell(i, j, ieco) = m_Data.Bcell(i, j, ieco) + solver.BcellThread(i, j, ieco)
                                m_Data.PredCell(i, j, ieco) = m_Data.PredCell(i, j, ieco) + solver.PredCellThread(i, j, ieco)
                            Next : Next
                    Next
                Next
            Next

        Catch ex As Exception
            cLog.Write(ex)
            Debug.Assert(False, ex.Message)
            Throw New ApplicationException("Error in runIBMSolverThreads()", ex)
        End Try


    End Sub


    Private Sub runGridSolverThreads()
        Dim solver As cGridSolver
        Dim iFstGrp As Integer
        Dim iLstgrp As Integer

        iFstGrp = 1
        iLstgrp = 0

        Try
            'this loop should only excecute once
            'Do While iLstgrp < m_Data.totalIntegratedGroups
            'loop through each solver object, make sure it's okay to run, and run it
            'each thread will do several groups at a time
            For Each solver In m_gridSolvers

                If solver.isOkToRunning Then

                    'iLstgrp = iFstGrp + m_Data.nGroupsPerThread - 1
                    'If iLstgrp > m_Data.totalIntegratedGroups Then iLstgrp = m_Data.totalIntegratedGroups 'm_Data.nvartot Then iLstgrp = m_Data.nvartot

                    'solver.FirstLastGroups(m_Data.integratedGroups(iFstGrp), m_Data.integratedGroups(iLstgrp))
                    solver.FirstLastGroups(1, nGroupsInThread(solver.ThreadID))
                    solver.SignalState.Reset()

                    solver.isOkToRunning = False
                    ThreadPool.QueueUserWorkItem(AddressOf solver.Solve)

                    'iFstGrp += m_Data.nGroupsPerThread
                Else
                    'System.Console.WriteLine("Solver thread blocked ID:" & solver.ThreadID & " Group:" & solver.iFirstIndex & " time:" & m_Data.TimeNow)
                End If

                'If iLstgrp >= m_Data.totalIntegratedGroups Then
                'Exit For
                'End If
            Next solver
            'Loop

            ' wait for all the threads to finish before starting the next time step
            Dim solvCtr As Integer = 1
            Dim timerTemp As Single
            Dim iterTime As Single
            For Each solver In m_gridSolvers
                If solvCtr = 2 Then timerTemp = Microsoft.VisualBasic.Timer
                solver.SignalState.WaitOne()
                totalIterThread(solvCtr) = totalIterThread(solvCtr) + solver.iterThread
                iterTime = iterTime + solver.iterThread
                solvCtr = solvCtr + 1
            Next
            If solvCtr = 2 Then timerTemp = Microsoft.VisualBasic.Timer
            gridThreadWaitTimer = gridThreadWaitTimer + (Microsoft.VisualBasic.Timer - timerTemp)


            '  System.Console.WriteLine("Solvegrid iterations = " & iterTime.ToString)
        Catch ex As Exception
            cLog.Write(ex)
            Debug.Assert(False, ex.Message)
            Throw New ApplicationException("Error in runSolverThreads()", ex)
        End Try


    End Sub
    Private Sub runSpaceSolverThreads()
        Dim solver As cSpaceSolver
        Dim iFrstCell As Integer
        Dim iLstCell As Integer
        Dim ieco As Integer
        ReDim Btime(m_Data.NGroups)
        ReDim TotLoss(m_Data.NGroups)
        ReDim TotEatenBy(m_Data.NGroups)
        ReDim TotBiom(m_Data.NGroups)
        ReDim TotPred(m_Data.NGroups)
        ReDim TotIFDweight(m_Data.NGroups)

        'redims the Btime of each thread
        'this could be done inside the thread, but if solve gets run on the 
        'same thread, (which it shouldn't), it will be deleted
        For Each solver In m_spaceSolvers
            ReDim solver.BtimeLocal(m_Data.NGroups)
            ReDim solver.TotLossThread(m_Data.NGroups)
            ReDim solver.TotEatenByThread(m_Data.NGroups)
            ReDim solver.TotBiomThread(m_Data.NGroups)
            ReDim solver.TotPredThread(m_Data.NGroups)
            ReDim solver.TotIFDweightThread(m_Data.NGroups)
        Next

        iFrstCell = 1
        iLstCell = 0

        Try
            'this do loop should only execute once
            Do While iLstCell < m_Data.iTotalWaterCells 'm_data.iTotalCells
                'loop through each solver, create a thread for it, and run it
                For Each solver In m_spaceSolvers

                    If solver.isOkToRun Then

                        iLstCell = iFrstCell + m_Data.nCellsPerThread - 1
                        If iLstCell > m_Data.iTotalWaterCells Then iLstCell = m_Data.iTotalWaterCells 'iTotalCells Then iLstCell = iTotalCells

                        solver.FirstLastCells(iFrstCell, iLstCell)
                        solver.SignalState.Reset()

                        solver.isOkToRun = False
                        ThreadPool.QueueUserWorkItem(AddressOf solver.Solve)

                        iFrstCell += m_Data.nCellsPerThread

                    Else
                        'this shouldn't happen
                        Debug.Assert(False, "Attempting to use an already running thread")
                    End If ' If solver.isOkToRunning Then

                    If iLstCell >= m_Data.iTotalWaterCells Then 'iTotalCells Then
                        Exit For
                    End If

                    If m_bsolverError Then
                        'one of the solver threads has thrown an error that was handled in the thread
                        're-throw the error here
                        Throw New ApplicationException("Ecospace error " & m_solverErrorMsg & " ThreadID = " & m_solverErrorID.ToString)
                    End If

                Next solver

            Loop ' Do While iLstCell < iTotalCells

            Dim solvCtr As Integer = 1
            Dim timerTemp As Single
            ' wait for all the threads to finish before starting the next time step
            For Each solver In m_spaceSolvers
                If solvCtr = 2 Then timerTemp = Microsoft.VisualBasic.Timer
                solver.SignalState.WaitOne()
                solvCtr = solvCtr + 1
            Next
            If solvCtr = 2 Then timerTemp = Microsoft.VisualBasic.Timer
            spaceThreadWaitTimer = spaceThreadWaitTimer + (Microsoft.VisualBasic.Timer - timerTemp)

            'this sums variables from all sums local to each thread
            For Each solver In m_spaceSolvers
                For ip As Integer = 1 To m_Data.NGroups
                    Btime(ip) = solver.BtimeLocal(ip) + Btime(ip)
                Next

                'If m_search.bInSearch Then
                '    'sum search data from threads
                '    For iflt As Integer = 1 To m_EPdata.NumFleet
                '        For igrp As Integer = 1 To m_EPdata.NumLiving
                '            Me.m_search.ValCatch(iflt, igrp) = m_search.ValCatch(iflt, igrp) + solver.Search.ValCatch(iflt, igrp)
                '            Me.m_search.CatchYear(iflt, igrp) = m_search.CatchYear(iflt, igrp) + solver.Search.CatchYear(iflt, igrp)
                '        Next
                '    Next
                'End If

                If m_Data.NewMultiStanza Then
                    For isp As Integer = 1 To m_Stanza.Nsplit
                        'ieco = m_Stanza.EcopathCode(isp, m_Stanza.Nstanza(isp))
                        For ist As Integer = 1 To m_Stanza.Nstanza(isp)
                            ieco = m_Stanza.EcopathCode(isp, ist)

                            'accumulate information needed to predict mean stanza loss, feeding, IFD weights from derivtred outputs
                            'these arrays are used in the new SpaceSplitUpdate subroutine for predicting mortality
                            'rate and growth rate averages over space by age in that update routine
                            'IFDweight is used to predict proportion of biomass of ieco stanza that will be on cell i,j
                            TotLoss(ieco) = TotLoss(ieco) + solver.TotLossThread(ieco)
                            TotEatenBy(ieco) = TotEatenBy(ieco) + solver.TotEatenByThread(ieco)
                            TotBiom(ieco) = TotBiom(ieco) + solver.TotBiomThread(ieco)
                            TotPred(ieco) = TotPred(ieco) + solver.TotPredThread(ieco)
                            TotIFDweight(ieco) = TotIFDweight(ieco) + solver.TotIFDweightThread(ieco)

                        Next
                    Next
                End If
            Next


        Catch ex As Exception
            cLog.Write(ex)
            Debug.Assert(False, ex.Message)
            Throw New ApplicationException("Error in runSpaceSolverThreads() " & ex.Message, ex)
        End Try


    End Sub

    Public Function initSpatialEquilibrium() As Boolean
        Dim ip As Integer, i As Integer, j As Integer
        Dim ig As Integer
        Dim isp As Integer, ist As Integer

        Try


            ReDim totalIterThread(m_Data.nGridSolverThreads + 1)

            'redim MPred at the start of each run because we have no way of knowing when EcoSimDataStructures.inlinks has changed
            'inlinks is the number of prey/pred linkages
            ReDim Me.m_Data.MPred(Me.m_Data.InRow + 1, Me.m_Data.InCol + 1, Me.m_SimData.inlinks)

            'm_Data.Depth(10, 10) = 0
            m_bsolverError = False

            m_StopRun = False
            nvar2 = m_Data.NGroups

            '*******************
            'readAdvectFile()
            '*****************

            'jb Moved default values to InitToDefaults
            ' m_Data.NewMultiStanza = True
            '  m_Data.UseIBM = True
            'm_Data.TimeStep = 1 / 12

            If m_Data.NewMultiStanza Then
                '    m_Data.IFDPower = 0.5 'this should be available to users in interface, higher values typically cause
                'instability in spatial allocation (IFD) model for multistanza biomass distributions

                ReDim Blocal(m_Data.NGroups)
            End If

            Dim Wchange() As Single
            ReDim Wchange(m_Data.nvartot)

            ReDim Cper(m_Data.InRow + 1, m_Data.InCol + 1, m_Data.NGroups)
            ReDim Ecode(m_Data.Nvarsplit)

            If m_tracerData.EcoSpaceConSimOn Then
                m_Data.RedimConSimVars()
                m_tracerData.redimForEcospaceRun(m_Data.NoRegions, m_Data.NGroups, m_Data.nTimeSteps)
            End If

            m_SimData.FirstTime = True

            'In EwE5 this was part of InitialState changed here
            'compute the IecoCode() index
            'this index pointer is unique to Ecospace
            ReDim IecoCode(m_Data.NGroups)
            Dim ir As Integer, igrp As Integer
            For i = 1 To m_Stanza.Nsplit
                For j = 1 To m_Stanza.Nstanza(i)
                    ir = ir + 1
                    igrp = m_Stanza.EcopathCode(i, j)
                    IecoCode(igrp) = ir
                Next
            Next

            'populates Kmovefit() and PzoTOmove()
            SetKmove() 'test set for movement in relation to fitness 

            '    If NoRegions > 0 Then ShowReg = True

            'jb this is all handled by the cContaminant object(s)
            ' If m_ESData.ConSimOn Then ReDim m_Ecosim.ConTrace.ConcTr(m_Data.NGroups + 1)
            '    If ConSimOn Then
            '        Dim Derivcon() As Single, Cintotal() As Single, Closs() As Single, ConCtot As Single
            '       ReDim Derivcon(NumGroups) As Single, Cintotal(NumGroups) As Single, Closs(NumGroups) As Single
            '    End If


            '    'The following hab parameters are used for recording habitatchanges when space is paused:
            '        If NoHabChanges > 0 And chkReplayHabitatChanges Then NextHabitatRecord = 1 : NextHabitatTime = HabTime(1) 'first time
            '        If chkRecordHabitatChanges Then
            '            If NoHabChanges > 0 Then
            '                RetVal = MsgBox("Delete existing habitat change scenario", vbInformation + vbYesNo)
            '                If RetVal = vbYes Then
            '                    NoHabChanges = 0
            '                    ReDim HabChange(3, NoHabChanges)
            '                End If
            '            Else
            '                NoHabChanges = 0
            '                ReDim HabChange(3, NoHabChanges)
            '            End If
            '        End If


            SetBoundaryDepths()

            'check to see if user wants to have some groups advected/migratory
            ReDim MigPowi(m_Data.NGroups, m_Data.InRow + 1), MigPowj(m_Data.NGroups, m_Data.InCol + 1)
            ReDim PrefRowP(m_Data.NGroups, 12), PrefColP(m_Data.NGroups, 12)
            For ip = 1 To m_Data.NGroups

                'jb comment out for now
                'ToDo_jb  initSpatialEquilibrium() FeedBackMessage
                'If M - Data.RelMoveBad(ip) = 1 And Mvel(ip) > 1 And IsAdvected(ip) = 0 Then
                '    If MsgBox("Is group " + Specie$(ip) + " advected?", vbYesNo) = vbYes Then IsAdvected(ip) = 1
                'End If

                If m_Data.IsMigratory(ip) Then
                    For i = 1 To m_Data.InRow : MigPowi(ip, i) = i ^ m_Data.MigConcRow(ip) : Next
                    For j = 1 To m_Data.InCol : MigPowj(ip, j) = j ^ m_Data.MigConcCol(ip) : Next
                    For i = 1 To 12
                        PrefRowP(ip, i) = m_Data.PrefRow(ip, i) ^ m_Data.MigConcRow(ip)
                        PrefColP(ip, i) = m_Data.Prefcol(ip, i) ^ m_Data.MigConcCol(ip)
                    Next
                End If

            Next

            'VC Hobart Sep 2008 
            ReDim m_Data.SpatialField(m_Data.InRow, m_Data.InCol, m_Data.nSpatialFields)
            ReDim m_Data.SpatialFieldOptimum(m_Data.nLiving, m_Data.nSpatialFields)
            ReDim m_Data.SpatialFieldStdLeft(m_Data.nLiving, m_Data.nSpatialFields)
            ReDim m_Data.SpatialFieldStdRight(m_Data.nLiving, m_Data.nSpatialFields)

            'VC Hobart Sep 2008 next is for reading of distribution envelopes, 
            ReDim m_Data.DistributionEnvelope(m_Data.InRow + 1, m_Data.InCol + 1, m_Data.NGroups)  'dimensioning detritus as well

            For iRo As Integer = 0 To m_Data.InRow + 1
                For iCo As Integer = 0 To m_Data.InCol + 1
                    For iGr As Integer = 1 To m_Data.NGroups
                        m_Data.DistributionEnvelope(iRo, iCo, iGr) = True
                    Next
                Next
            Next


            SetHabGrad()

            m_Data.PredictEffort = True 'from EwE5
            'mData.PredictEffort set in InitToDefaults()
            '        'If chkPredictEffort.Value = Checked Then PredictEffort = 1 Else PredictEffort = 0

            'first set density map for all pools to no movement equilibrium
            SetBiomassesEcospace()
            PPScale = ScaleRelativePrimaryProductivityToEcopathLevel()
            ScaleSailingToUnity()

            'calculate exponential weights for time step updating
            m_Ecosim.Derivt(0, m_SimData.StartBiomass, der)
            For ip = 1 To m_Data.NGroups
                '****Following line corrects bug where Mrate was set later in the routine
                'CJW modified Mrate calculation next line 2/2003 for migratory species
                If m_Data.IsMigratory(ip) = False Then
                    m_Data.Mrate(ip) = m_Data.Mvel(ip) / (3.14159 * m_Data.CellLength)
                Else
                    m_Data.Mrate(ip) = m_Data.Mvel(ip) / Math.Sqrt(m_Data.CellLength)
                End If

            Next ip

            For ig = 1 To m_Data.nFleets
                m_SimData.FishRateGear(ig, 0) = 1
            Next ig

            If m_Data.IsFishRateSet = False Then
                For ig = 1 To m_Data.nFleets
                    'For i = 0 To TotalTime * 12
                    For i = 0 To m_SimData.NTimes
                        m_SimData.FishRateGear(ig, i) = 1
                    Next
                Next
                'ToDo_jb IsFishRateSet in EwE5 see when this gets reset to false
                m_Data.IsFishRateSet = True
            End If

            If m_Data.PredictEffort Then SetEffortParameters()

            If m_tracerData.EcoSpaceConSimOn Then
                'If m_ConTracer Is Nothing Then
                '    m_ConTracer = New cContaminantTracer
                'End If
                'm_ConTracer.Init(m_tracerData, m_EPdata, m_ESData, m_Stanza)
                'm_ConTracer.CInitialize()
                Basebiomass(0) = 1
                m_Data.IsAdvected(0) = True
            End If

            For ip = 0 To m_Data.NGroups
                Btime(ip) = 0
                For i = 0 To m_Data.InRow + 1
                    For j = 0 To m_Data.InCol + 1
                        If m_Data.IsAdvected(ip) Then
                            m_Data.Bcell(i, j, ip) = Basebiomass(ip)
                        Else : m_Data.Bcell(i, j, ip) = 0
                        End If

                        If m_Data.Depth(i, j) > 0 Then
                            m_Data.Bcell(i, j, ip) = Basebiomass(ip)
                            If m_Data.PrefHab(ip, m_Data.HabType(i, j)) = False And m_Data.PrefHab(ip, 0) = False Then
                                m_Data.Bcell(i, j, ip) = 0.1 * m_SimData.StartBiomass(ip)
                            End If
                            'VC Hobart Sep 2008: only assign biomass if it is within distribution envelope
                            If m_Data.DistributionEnvelope(i, j, ip) = False Then
                                m_Data.Bcell(i, j, ip) = 0.0001 * m_SimData.StartBiomass(ip)
                            End If


                            If m_Data.IsMigratory(ip) Then
                                If i = 0 Or i = m_Data.InRow + 1 Or j = 0 Or j = m_Data.InCol + 1 Then m_Data.Bcell(i, j, ip) = 0
                            End If
                        Else
                            AMm(i, j, ip) = -1.0 'E+30
                        End If ' If m_Data.Depth(i, j) > 0 Then

                        If ip = 0 Then m_Data.Bcell(i, j, ip) = 1

                        m_Data.Blast(i, j, ip) = m_Data.Bcell(i, j, ip)
                        If i > 0 And j > 0 And i <= m_Data.InRow And j <= m_Data.InCol Then Btime(ip) = Btime(ip) + m_Data.Bcell(i, j, ip)

                        If m_tracerData.EcoSpaceConSimOn And ip <= m_Data.NGroups Then
                            'Debug.Assert(False, "EcoSpace Contaminant Tracer not Initialized properly.")
                            'jb in EwE5 Ccell() is initialized using ConcTr()
                            'in EwE5 CInitialize() was called right before this setting ConcTr() to Czero()
                            m_Data.Ccell(i, j, ip) = m_Data.Bcell(i, j, ip) / Basebiomass(ip) * Me.m_tracerData.Czero(ip)
                            m_Data.Clast(i, j, ip) = m_Data.Ccell(i, j, ip)
                        End If

                        Cper(i, j, ip) = m_SimData.Cbase(ip)
                        FtimeCell(i, j, ip) = 1
                        HdenCell(i, j, ip) = m_SimData.Hden(ip)
                    Next j
                Next i
                Btime(ip) = Btime(ip) / m_Data.nWaterCells
            Next ip

            Dim isc As Integer, ieco As Integer
            isc = 0


            For isp = 1 To m_Stanza.Nsplit
                For ist = 1 To m_Stanza.Nstanza(isp)
                    isc = isc + 1
                    '  If ist = m_Stanza.Nstanza(isp) Then iadultS(isp) = isc
                    ieco = m_Stanza.EcopathCode(isp, ist)
                    If m_Data.NewMultiStanza Or m_Data.UseIBM Then
                        'these flags turn off implicit integration for multistanza biomasses when newmultistanza=true
                        m_Data.ByPassIntegrate(ieco) = True
                        If m_Data.UseIBM Then m_Data.ByPassIntegrate(nvar2 + isc) = True
                    End If
                    Ecode(isc) = ieco
                    If m_Data.IsMigratory(ieco) = True Then m_Data.IsMigratory(nvar2 + isc) = True
                    For i = 0 To m_Data.InRow + 1
                        For j = 0 To m_Data.InCol + 1
                            If m_Data.Depth(i, j) > 0 Then
                                'VC Hobart Sep 2008: adding distribution envelope 
                                If (m_Data.PrefHab(ieco, m_Data.HabType(i, j)) = True Or m_Data.PrefHab(ieco, 0) = True) _
                                And m_Data.DistributionEnvelope(i, j, ieco) = True Then
                                    m_Data.Bcell(i, j, nvar2 + isc) = NstanzaBase(isc) * Basebiomass(ieco) / m_SimData.StartBiomass(ieco)
                                    If m_Data.NewMultiStanza Then m_Data.PredCell(i, j, ieco) = m_SimData.pred(ieco)
                                Else
                                    m_Data.Bcell(i, j, nvar2 + isc) = NstanzaBase(isc) / 10
                                    If m_Data.NewMultiStanza Then m_Data.PredCell(i, j, ieco) = m_SimData.pred(ieco) / 1000
                                End If
                            Else ': Print()
                                m_Data.Bcell(i, j, nvar2 + isc) = 1.0E-20
                            End If
                            m_Data.Blast(i, j, nvar2 + isc) = m_Data.Bcell(i, j, nvar2 + isc)
                        Next
                    Next
                Next
            Next

            'set dispersal rate arrays for solvegrid
            SetMovementParameters()

            ' If CurrentForce Then velmaker.SetBoundaryDepths() 
            'Need to call this to initialize DepthY and DepthX arrays.  
            If m_Data.CurrentForce Then SetXYBoundaryDepths()

            'set some solvegrid solution parameters
            Dim ihalf As Integer
            Dim iter As Double
            Dim TimeStep2 As Single
            'm_Data.W = 0.9
            'm_Data.Tol = 0.0001
            'm_Data.maxIter = 40

            ihalf = Int(m_Data.InCol / 2)
            j = 0
            For i = ihalf To 1 Step -1
                j = j + 1
                jord(i) = j
            Next
            For i = ihalf + 1 To m_Data.InCol
                j = j + 1
                jord(i) = j
            Next

            iter = 0
            'If m_Data.NumStep < 1 Then m_Data.NumStep = 5
            'm_Data.NumStep0 = 0 : m_Data.NumStep1 = 0
            TimeStep2 = m_Data.TimeStep '/ 2

            '   Dim RelRepStanza() As Single
            ReDim RelRepStanza(m_Stanza.Nsplit)
            For i = 1 To m_Stanza.Nsplit
                RelRepStanza(i) = 1 / m_SimData.StartBiomass(m_Stanza.EcopathCode(i, m_Stanza.Nstanza(i)))
            Next i

            Dim waterCtr As Integer = 0
            Dim foundRow As Boolean
            ReDim m_Data.iWaterCellIndex(m_Data.InCol * m_Data.InRow)
            ReDim m_Data.jWaterCellIndex(m_Data.InCol * m_Data.InRow)
            ReDim m_Data.iStartRow(m_Data.InCol)
            ReDim m_Data.iEndRow(m_Data.InCol)
            ReDim m_Data.jStartCol(m_Data.InRow)
            ReDim m_Data.jEndCol(m_Data.InRow)


            'this finds the start and end rows and columns so that solvegrid doesn't go through every one
            For j = 1 To m_Data.InCol
                foundRow = False
                m_Data.iStartRow(j) = m_Data.InRow + 1
                m_Data.iEndRow(j) = 0
                For i = 1 To m_Data.InRow
                    If m_Data.Depth(i, j) > 0 Then
                        waterCtr = waterCtr + 1
                        m_Data.iWaterCellIndex(waterCtr) = i
                        m_Data.jWaterCellIndex(waterCtr) = j
                        If m_Data.iStartRow(j) = m_Data.InRow + 1 Then
                            m_Data.iStartRow(j) = i
                            foundRow = True
                        End If
                        m_Data.iEndRow(j) = i
                    End If
                Next
                'm_Data.iStartRow(j) = 1
                'm_Data.iEndRow(j) = m_Data.Inrow
            Next
            m_Data.iTotalWaterCells = waterCtr

            For i = 1 To m_Data.InRow
                m_Data.jStartCol(i) = m_Data.InCol + 1
                m_Data.jEndCol(i) = 0
                For j = 1 To m_Data.InCol
                    If m_Data.Depth(i, j) > 0 Then
                        If m_Data.jStartCol(i) = m_Data.InCol + 1 Then
                            m_Data.jStartCol(i) = j
                        End If
                        m_Data.jEndCol(i) = j
                    End If
                Next
            Next

            ReDim BEQlast(m_Data.InRow + 1, m_Data.InCol + 1, m_Data.nvartot)

            '**** this functionality has been moved below **** 
            'this finds which groups are being integrated, so they can be adeq
            'ReDim m_Data.integratedGroups(m_Data.nvartot)
            'Dim integrateIndex As Integer = 0
            'For i = 1 To m_Data.nvartot
            '    If m_Data.ByPassIntegrate(i) = False Then
            '        integrateIndex = integrateIndex + 1
            '        m_Data.integratedGroups(integrateIndex) = i
            '    End If
            'Next
            'm_Data.totalIntegratedGroups = integrateIndex

            'ww set up thread alocation for gridsolver, since migratory takes much longer
            nMigratory = 0
            ReDim migratoryIndex(m_Data.nvartot)
            For i = 1 To m_Data.nvartot
                'find all the migratory species
                If m_Data.IsMigratory(i) And m_Data.ByPassIntegrate(i) = False Then
                    nMigratory += 1
                    migratoryIndex(nMigratory) = i
                End If
            Next
            Dim thread As Integer
            ReDim nGroupsInThread(m_Data.nGridSolverThreads)
            ReDim threadGroups(m_Data.nGridSolverThreads, m_Data.nvartot)
            For i = 1 To nMigratory
                'allocate the migratory species to threads
                thread = (i - 1) Mod m_Data.nGridSolverThreads + 1
                nGroupsInThread(thread) += 1
                threadGroups(thread, nGroupsInThread(thread)) = migratoryIndex(i)
            Next
            Dim nNonMigThreads As Integer = (m_Data.nGridSolverThreads - nMigratory Mod m_Data.nGridSolverThreads)
            Dim numNonMig As Integer
            For i = 1 To m_Data.nvartot
                'assign the nonmigratory integrated variables to the least used threads
                If m_Data.IsMigratory(i) = False And m_Data.ByPassIntegrate(i) = False Then
                    numNonMig += 1
                    thread = m_Data.nGridSolverThreads - (numNonMig - 1) Mod nNonMigThreads
                    nGroupsInThread(thread) += 1
                    threadGroups(thread, nGroupsInThread(thread)) = i
                End If
            Next

            InitGridSolverThreads() 'init the solver objects one for each thread
            InitSpaceSolverThreads()
            If m_Data.UseIBM Then InitIBMSolverThreads()

            If nMigratory > 0 Then 'And useMigratoryGrad Then
                SetMigGrad() 'ww
            End If

            'Dim solver As cGridSolver = m_gridSolvers(0)


            '*** this below is garbage ***
            ''test structure for initializing spatial distribution of migratory species
            'For ip = 1 To m_Data.NGroups
            '    If m_Data.IsMigratory(ip) = True And (m_Data.Inrow > 1 And m_Data.InCol > 1) And (m_Data.MigConcRow(ip) > 0 Or m_Data.MigConcCol(ip) > 0) Then
            '        'If IsMigratory(ip) = True Then

            '        'jb EwE5 
            '        'VaryMovementParameters(1, ip, IadCode(ip), IjuCode(ip), IecoCode(ip))
            '        VaryMovementParameters(1, ip, IecoCode(ip))

            '        For i = 1 To m_Data.Inrow
            '            For j = 1 To m_Data.InCol
            '                If i = m_Data.PrefRow(ip, 1) And j = m_Data.Prefcol(ip, 1) Then
            '                    Flowin(ip) = 1 : FlowoutRate(ip) = 1 / m_Data.Bcell(i, j, ip)
            '                Else
            '                    Flowin(ip) = 0 : FlowoutRate(ip) = 0
            '                End If
            '                F(i, j, ip) = Flowin(ip)
            '                AMm(i, j, ip) = -FlowoutRate(ip) - Bcw(i + 1, j, ip) - C(i - 1, j, ip) - d(i, j, ip) - e(i, j, ip)
            '                If AMm(i, j, ip) >= 0 Then AMm(i, j, ip) = -1.0 'E+30
            '            Next
            '        Next

            '        solver.FirstLastGroups(ip, ip)
            '        'solver.Solve(Nothing)

            '    End If 'If m_Data.IsMigratory(ip) = True And (m_Data.Inrow > 1 And m_Data.InCol > 1) And (m_Data.MigConcRow(ip) > 0 Or m_Data.MigConcCol(ip) > 0) Then
            'Next ip


            If m_tracerData.EcoSpaceConSimOn Then
                'initialize the contaminant tracing
                Try
                    'contaminant tracer grid solver runs on one thread
                    'process all groups on the single thread
                    ReDim Me.threadGroupsConSim(1, m_Data.NGroups)
                    For igrp = 1 To m_Data.NGroups
                        threadGroupsConSim(1, igrp) = igrp
                    Next

                    'bypass integrated for contaminants should be false for all groups
                    ReDim m_ConBypassIntegrated(m_Data.NGroups)

                    If grdslvConSim Is Nothing Then
                        'grid solver object for the contaminant tracer
                        grdslvConSim = New cGridSolver(1)
                    End If

                    'init the grid solver object
                    grdslvConSim.Init(m_Data.AMmTr, m_Data.Ftr, m_Data.Ccell, m_Data.InRow, m_Data.InCol, m_Data.Tol, jord, m_Data.W, Bcw, C, d, e, _
                                       m_Data.Depth, m_ConBypassIntegrated, m_Data.iStartRow, m_Data.iEndRow, m_Data.TimeStep, m_Data.maxIter, m_Data.jStartCol, _
                                       m_Data.jEndCol, m_Data.IsMigratory, threadGroupsConSim, m_Data.UseExact)

                Catch ex As Exception
                    'something went very wrong with the initialization
                    m_tracerData.EcoSpaceConSimOn = False
                    Debug.Assert(False, ex.StackTrace)
                    cLog.Write(ex)
                End Try

            End If

            'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
            'ToDo_jb initSpatialEquilibrium() Still need to  implement reading of the nutrient maps
            'and 
            '        ReDim SimPlot(NumGroups, 7, TotalTime / TimeStep + 1)
            '        ReadNutrientMaps()

            m_Ecosim.InitializeDataInfo()

            'If m_search.bInSearch Then
            '    'NEEDS TO RECALCULATE BASECOST ETC
            '    m_search.redimForRun()
            '    m_search.bBaseYearSet = False
            'End If


            'TotalTime = m_ESData.NumYears
            'jb maxtime is set by the size of the time step in Ecosim one month max time steps per year
            'MaxTime = m_Data.TotalTime * (1 / m_Data.TimeStep) '* 12 '1200  'TotalTime
            'MaxTime = m_Data.TotalTime * 12 '1200  'TotalTime


            '        If SpDatYear > 0 Then  'there are timeseries
            '            ReDim SpaceBiomassByRegion(TotalTime, m_data.nGroups, NoRegions)
            '            ReDim SpaceBiomassByRegionCount(TotalTime, m_data.nGroups, NoRegions)
            '            ReDim SpaceCatchByRegion(TotalTime, m_data.nGroups, NoRegions)
            '            ReDim SpaceCatchByRegionCount(TotalTime, m_data.nGroups, NoRegions)
            '            ReDim SpaceEffortByRegionFleet(TotalTime, NumGear, NoRegions)
            '            ReDim SpaceEffortByRegionFleetCount(TotalTime, NumGear, NoRegions)
            '            If ConSimOn Then 'only if there are tracer data
            '                ReDim SpaceTraceByRegion(TotalTime, m_data.nGroups, NoRegions)
            '                ReDim SpaceTraceByRegionCount(TotalTime, m_data.nGroups, NoRegions)
            '            End If
            '        End If




            'm_Data.nGroupsPerThread = (m_Data.totalIntegratedGroups + m_Data.nGridSolverThreads - 1) \ m_Data.nGridSolverThreads 'm_Data.nvartot \ m_Data.nGridSolverThreads + 1
            m_Data.nCellsPerThread = (m_Data.iTotalWaterCells + m_Data.nSpaceSolverThreads - 1) \ m_Data.nSpaceSolverThreads
            m_Data.nIBMGroupsPerThread = (m_Stanza.Nsplit + m_Data.nGridSolverThreads - 1) \ m_Data.nGridSolverThreads


            'Me.m_FleetSum = New cSpaceFleetSummary(Me.m_Data, 0)


            'waterCtr = 0
            'Dim iList As Single()
            'Dim iCellsList As New List(Of Integer())
            'For i = 1 To m_Data.nSpaceSolverThreads
            '    ReDim iList(m_Data.nCellsPerThread)
            '    For j = 1 To m_Data.nCellsPerThread
            '        waterCtr = waterCtr + 1
            '        iList(j) = m_Data.iWaterCellIndex(waterCtr)

            '    Next
            'Next

            Return True

        Catch ex As Exception
            Debug.Assert(False, ex.StackTrace)
            Throw New Exception("InitSpatialEquilibrium() Error: " & ex.Message, ex)
        End Try


    End Function




    ''' <summary>
    ''' Redim all non local variables before running FindSpatialEquilibrium()
    ''' </summary>
    ''' <remarks>In EwE5 this was handled inside FindSpatialEquilibrium. 
    ''' These are variables that will be populated by the Ecospace initialization initSpatialEquilibrium().
    ''' This should not contain any data that was populated by the database.
    ''' </remarks>
    Friend Function redimForRun() As Boolean
        ' EwE5
        'ReDim ebb(NumGroups + 3 * npairs + Nvarsplit) As Single 'abmpa
        'ReDim BB(NumGroups + 3 * npairs + Nvarsplit) As Single

        Dim success As Boolean = True
        Dim message As cMessage
        Try

            'redim new stanza stuff
            ReDim m_Data.PredCell(m_Data.InRow + 1, m_Data.InCol + 1, m_Data.NGroups)
            ReDim m_Data.IFDweight(m_Data.InRow, m_Data.InCol, m_Data.NGroups)
            ReDim m_Data.ByPassIntegrate(m_Data.nvartot)

            ReDim m_Data.BBase(m_Data.NGroups)

            ReDim RelFitness(m_Data.InRow + 1, m_Data.InCol + 1, m_Data.NGroups)
            ReDim Basebiomass(m_Data.nvartot)
            ReDim der(m_Data.NGroups)
            ReDim loss(m_Data.NGroups)
            ReDim pbb(m_Data.NGroups)

            ReDim EatEff(m_Data.nvartot)
            ReDim VulPred(m_Data.nvartot)

            ReDim Flowin(m_Data.nvartot)
            ReDim FlowoutRate(m_Data.nvartot)

            ReDim F(m_Data.InRow + 1, m_Data.InCol + 1, m_Data.nvartot)
            ReDim AMm(m_Data.InRow + 1, m_Data.InCol + 1, m_Data.nvartot)

            ReDim BcwNomig(m_Data.InRow + 1, m_Data.InCol + 1, m_Data.nvartot)
            ReDim CNomig(m_Data.InRow + 1, m_Data.InCol + 1, m_Data.nvartot)
            ReDim dNomig(m_Data.InRow + 1, m_Data.InCol + 1, m_Data.nvartot)
            ReDim Enomig(m_Data.InRow + 1, m_Data.InCol + 1, m_Data.nvartot)

            ' ReDim conSplit(m_Data.Nvarsplit)
            ReDim TotEffort(m_Data.nFleets)
            ReDim RecSplit(m_Data.Nvarsplit)
            ReDim PconSplit(m_Data.Nvarsplit)
            ReDim Tstanza(m_Data.Nvarsplit)
            ReDim NstanzaBase(m_Data.Nvarsplit)

            nEcospaceTimeSteps = CInt(m_Data.TotalTime * (1.0 / m_Data.TimeStep))
            success = success And m_Data.redimTimeStepResults(nEcospaceTimeSteps)

        Catch ex As Exception
            message = New cMessage(My.Resources.CoreMessages.ECOSPACE_INIT_ERROR, _
                                   eMessageType.ErrorEncountered, eCoreComponentType.EcoSpace, eMessageImportance.Critical)
        End Try

        If message IsNot Nothing Then
            Me.Messages.AddMessage(message)
            success = False
        End If

        Return success

    End Function


    Sub SetKmove()
        Dim i As Integer

        ReDim RelMoveFit(m_Data.InRow + 1, m_Data.InCol + 1)
        ReDim PzoTOmove(m_Data.NGroups)
        ReDim Kmovefit(m_Data.NGroups)

        For i = 1 To m_Data.NGroups
            PzoTOmove(i) = m_Data.FitnessResp
            If m_EPdata.PB(i) > 0 Then Kmovefit(i) = 2.197225 / (PzoTOmove(i) * m_EPdata.PB(i))
        Next

        ReDim m_Data.Blast(m_Data.InRow + 1, m_Data.InCol + 1, m_Data.nvartot)
        ReDim FtimeCell(m_Data.InRow + 1, m_Data.InCol + 1, m_Data.NGroups)
        ReDim HdenCell(m_Data.InRow + 1, m_Data.InCol + 1, m_Data.NGroups)
        'ReDim LastB(m_Data.nGroups)
        ReDim Btime(m_Data.NGroups)

    End Sub


    Sub SetBoundaryDepths()
        'set cells around system boundary to depth 1 so as to allow flow across them and proper
        'tests for critters that are advected
        Dim i As Integer
        Dim j As Integer

        For j = 0 To m_Data.InCol + 1
            m_Data.Depth(0, j) = 1
            m_Data.Depth(m_Data.InRow + 1, j) = 1
        Next

        For i = 0 To m_Data.InRow + 1
            m_Data.Depth(i, 0) = 1
            m_Data.Depth(i, m_Data.InCol + 1) = 1
        Next

    End Sub


    Private Sub SetHabGrad()
        'set habitat quality gradient maps for all habitat types, for use in biased movement assessments
        Dim i As Integer, j As Integer, ii As Integer, jj As Integer, ihab As Integer, Thab As Single, Nobs As Single
        Dim i1 As Integer, i2 As Integer, j1 As Integer, j2 As Integer, Sweep As Integer, Habadd As Single
        Dim nsweep As Integer
        ReDim HabGrad(m_Data.InRow + 1, m_Data.InCol + 1, m_Data.NGroups)

        If m_Data.InRow > m_Data.InCol Then nsweep = m_Data.InRow Else nsweep = m_Data.InCol
        'nsweep = nsweep * 2
        iWindow = 1
        For ihab = 1 To m_Data.NGroups
            For Sweep = 1 To nsweep
                'If NcellsHab(ihab) > 0 Then
                For i = 1 To m_Data.InRow : For j = 1 To m_Data.InCol
                        'If i = 1 And j = 11 And ihab = 4 Then Stop
                        Thab = 0 : Nobs = 0
                        i1 = i - iWindow : If i1 < 0 Then i1 = 0
                        i2 = i + iWindow : If i2 > m_Data.InRow + 1 Then i2 = m_Data.InRow + 1
                        j1 = j - iWindow : If j1 < 0 Then j1 = 0
                        j2 = j + iWindow : If j2 > m_Data.InCol + 1 Then j2 = m_Data.InCol + 1
                        For ii = i1 To i2 : For jj = j1 To j2
                                Habadd = 0
                                If m_Data.PrefHab(ihab, m_Data.HabType(ii, jj)) And m_Data.Depth(ii, jj) > 0 _
                                And m_Data.DistributionEnvelope(ii, jj, ihab) Then
                                    Habadd = HabBest
                                ElseIf Sweep > 1 Then
                                    Habadd = HabGrad(ii, jj, ihab)
                                End If
                                Thab = Thab + Habadd
                                If m_Data.Depth(ii, jj) > 0 Then
                                    Nobs = Nobs + 1
                                End If
                            Next : Next
                        HabGrad(i, j, ihab) = Thab / Nobs
                        'hack
                        'If HabGrad(i, j, ihab) > HabBest Then Stop 'HabGrad(i, j, ihab) = HabBest
                        If m_Data.PrefHab(ihab, m_Data.HabType(i, j)) _
                        And m_Data.DistributionEnvelope(i, j, ihab) Then HabGrad(i, j, ihab) = HabBest

                        If m_Data.PrefHab(ihab, 0) And m_Data.DistributionEnvelope(i, j, ihab) _
                        Then HabGrad(i, j, ihab) = 1

                        If m_Data.Depth(i, j) = 0 Then HabGrad(i, j, ihab) = 0.0
                    Next : Next
                'If m_Data.PrefHab(ihab, 0) Then Exit For
                'End If
            Next
        Next

        'Dim tempstr As String
        'For ihab = 4 To 4 'm_Data.NGroups
        '    Debug.Print("ihab = " + ihab.ToString)
        '    For i = 0 To m_Data.Inrow + 1
        '        For j = 0 To m_Data.InCol + 1
        '            If Math.Round(HabGrad(i, j, ihab) * 10) < 100 Then
        '                tempstr = tempstr + " "
        '                If Math.Round(HabGrad(i, j, ihab) * 10) < 10 Then
        '                    tempstr = tempstr + " "
        '                End If
        '            End If
        '            If HabGrad(i, j, 1) > 0 Then
        '                tempstr = tempstr + Math.Round(HabGrad(i, j, ihab) * 10).ToString + " "
        '            Else
        '                tempstr = tempstr + "  "
        '            End If
        '        Next
        '        Debug.Print(tempstr)
        '        tempstr = ""
        '    Next
        'Next
        'For Sweep = 1 To 3
        '    For ihab = 1 To m_Data.NGroups
        '        For i = 0 To m_Data.Inrow + 1
        '            For j = 0 To m_Data.InCol + 1
        '                Thab = 0 : Nobs = 0
        '                i1 = i - iWindow : If i1 < 0 Then i1 = 0
        '                i2 = i + iWindow : If i2 > m_Data.Inrow + 1 Then i2 = m_Data.Inrow + 1
        '                j1 = j - iWindow : If j1 < 0 Then j1 = 0
        '                j2 = j + iWindow : If j2 > m_Data.InCol + 1 Then j2 = m_Data.InCol + 1

        '                For ii = i1 To i2
        '                    For jj = j1 To j2
        '                        Habadd = 0

        '                        If m_Data.PrefHab(ihab, m_Data.HabType(ii, jj)) Then
        '                            Habadd = HabBest
        '                        ElseIf Sweep > 1 Then
        '                            Habadd = HabGrad(ii, jj, ihab)
        '                        End If

        '                        Thab = Thab + Habadd
        '                        Nobs = Nobs + 1
        '                    Next jj
        '                Next ii

        '                HabGrad(i, j, ihab) = Thab / Nobs
        '                If m_Data.PrefHab(ihab, m_Data.HabType(i, j)) Then HabGrad(i, j, ihab) = HabBest
        '                If m_Data.PrefHab(ihab, 0) Then HabGrad(i, j, ihab) = 1
        '            Next j
        '        Next i
        '    Next
        'Next
    End Sub


    Private Sub SetBiomassesEcospace()
        Dim i As Integer, j As Integer, ii As Integer ' , IterMax As Integer
        ReDim m_Data.Vspace(m_SimData.inlinks), m_Data.Aspace(m_SimData.inlinks), PbSpace(m_Data.NGroups)

        'calculate pbbiomass parameter from pbbase and pbm
        m_Ecosim.Set_pbm_pbbiomass()
        'get initial derivative to define runge-kutta time step deltat
        m_Ecosim.SetFishTimetoFish1()
        '****ADDED BY CJW SEPT 2001
        m_Ecosim.InitialState()
        m_Ecosim.SetTimeSteps()
        m_Ecosim.CalcStartEatenOfBy()

        m_Ecosim.SetBBtoStartBiomass(m_Data.NGroups)
        ' ReDim der(m_Data.NGroups)

        m_Ecosim.Derivt(0, m_SimData.StartBiomass, der)

        'set up initial biomass density for fished areas
        For i = 1 To m_Data.NGroups
            m_SimData.FishTime(i) = m_SimData.Fish1(i)
            Basebiomass(i) = m_SimData.StartBiomass(i)
            PbSpace(i) = m_SimData.pbbiomass(i)
        Next

        'For ii = 1 To m_ESData.inlinks
        '    i = m_ESData.ilink(ii)
        '    j = m_ESData.jlink(ii)
        '    m_Data.Aspace(ii) = m_Ecosim.A(i, j)
        '    m_Data.Vspace(ii) = m_ESData.vulrate(i, j)
        'Next

        For ii = 1 To m_SimData.inlinks
            i = m_SimData.ilink(ii) : j = m_SimData.jlink(ii) ' : ia = ArenaLink(ii)
            m_Data.Aspace(ii) = m_SimData.Alink(ii)
        Next
        For ia As Integer = 1 To m_SimData.Narena
            m_Data.Vspace(ia) = m_SimData.VulArena(ia)
        Next


        'calculate correction factors for numbers dynamics going back from delay difference
        'to continuous case
        'If m_Data.AdjustSpace = True Then AdjustSpacePars()
        AdjustSpaceParsNew()

        If m_Stanza.Nsplit > 0 Then
            For i = 1 To m_Data.NGroups
                EatEff(i) = 1
                VulPred(i) = 1
            Next

            'jb EwE5 called  derivtRed with BB() this is the biomass at the current time step defined in Ecosim
            'I have changed to to call derivtRed with StartBiomass() which sould have the same effect and keep Ecosim.BB() out of this code
            derivtRed(m_SimData.StartBiomass, Flowin, FlowoutRate, EatEff, VulPred, 1)

        End If

        Dim isp As Integer, ist As Integer, St As Single, Sn As Single, ieco As Integer
        i = 0
        For isp = 1 To m_Stanza.Nsplit
            St = 1
            For ist = 1 To m_Stanza.Nstanza(isp)
                ieco = m_Stanza.EcopathCode(isp, ist)
                i = i + 1
                Tstanza(i) = (m_Stanza.Age2(isp, ist) - m_Stanza.Age1(isp, ist)) / 12.0#
                Sn = St * Math.Exp(-Tstanza(i) * FlowoutRate(ieco))

                If ist < m_Stanza.Nstanza(isp) Then
                    RecSplit(i) = St - Sn
                Else
                    RecSplit(i) = St
                End If

                St = Sn
                NstanzaBase(i) = RecSplit(i) / FlowoutRate(ieco)
                PconSplit(i) = m_SimData.pred(ieco) / NstanzaBase(i)
            Next
        Next
    End Sub


    '''' <summary>
    '''' this sub wasn't being called, so i'll remark it out
    '''' VC Hobart Sep 2008
    '''' </summary>
    '''' <remarks></remarks>
    'Private Sub AdjustSpacePars()
    '    'set ecospace basebiomass using proportions of usable habitat for each pool, and adjust
    '    'vulnerability, search parameters to mean biomasses in habitats used
    '    Dim i As Integer, j As Integer, K As Integer, ii As Integer, Bpred As Single, Bprey As Single, ia As Integer
    '    Dim Temp As Single

    '    'just set v and a to base values, do not change basebiomass unless adjustspace=true
    '    'get habitat areas

    '    CalcHabitatArea()
    '    'calculate habitat area used by each biomass type
    '    ReDim HabAreaUsed(m_Data.NGroups)
    '    For i = 1 To m_Data.NGroups
    '        For j = 1 To m_Data.NoHabitats
    '            If m_Data.PrefHab(i, j) Or m_Data.PrefHab(i, 0) Then HabAreaUsed(i) = HabAreaUsed(i) + m_Data.HabArea(j)
    '        Next
    '        If HabAreaUsed(i) > 0 Then
    '            Basebiomass(i) = ThabArea * m_ESData.StartBiomass(i) / HabAreaUsed(i)
    '        Else
    '            Basebiomass(i) = m_ESData.StartBiomass(i) 'don't really need this; set before routine called
    '        End If
    '    Next

    '    'adjust vulnerability and search parameters for these basebiomass values in preferred habitats
    '    For ii = 1 To m_ESData.inlinks
    '        i = m_ESData.ilink(ii) : j = m_ESData.jlink(ii)
    '        ia = m_ESData.ArenaLink(ii)
    '        Bpred = Basebiomass(j)
    '        Bprey = 0 'Aprey = 0
    '        For K = 1 To m_Data.NoHabitats
    '            If m_Data.PrefHab(j, K) Or m_Data.PrefHab(j, 0) Then
    '                If m_Data.PrefHab(i, K) Or m_Data.PrefHab(i, 0) Then
    '                    Bprey = Bprey + Basebiomass(i) * m_Data.HabArea(K)
    '                Else
    '                    Bprey = Bprey + 0.5 * m_ESData.StartBiomass(i) * m_Data.HabArea(K)
    '                End If
    '                ' Aprey = Aprey + HabArea(k)
    '            End If
    '        Next

    '        If HabAreaUsed(j) > 0 Then
    '            Bprey = Bprey / HabAreaUsed(j)
    '        Else
    '            Bprey = m_ESData.StartBiomass(i)
    '        End If

    '        m_Data.Vspace(ia) = m_ESData.vulrate(i, j) * Bpred / Bprey * m_ESData.StartBiomass(i) / m_ESData.StartBiomass(j)

    '        If m_ESData.Consumption(i, j) > 0.0000000001 Then
    '            'Below had Consumption(i,j)=10E-43 and caused overflow; placed trap above,VC05Feb01
    '            Temp = (m_Data.Vspace(ia) * Bprey * (m_ESData.pred(j) / m_ESData.Hden(j)) / m_ESData.Consumption(i, j) - (m_ESData.pred(j) / m_ESData.Hden(j)) * Bpred / m_ESData.StartBiomass(j))
    '        End If
    '        If Temp > 0 Then
    '            m_Data.Aspace(ii) = 2 * m_Data.Vspace(ia) / Temp
    '        Else
    '            m_Data.Aspace(ii) = m_Ecosim.A(i, j) '0
    '        End If
    '        '060608CJW: The frmspace.adjustspacepars routine does not calculate rates of effective search Aspace(ii) correctly
    '        'for some critters.  I’m not sure exactly what all got screwed up in the routine, but one place is just below
    '        'where you placed a “trap” for low consumption(i,j) cases.  For the if statement involving Temp, Aspace(ii)
    '        'is set to zero if Temp<0.  That should be changed to set Aspace(ii)=A(i,j) if Temp<0.
    '        'Setting aspace to zero can actually cause some groups not to feed at all after the correction is applied.
    '        If m_Data.Aspace(ii) < 0 Then m_Data.Aspace(ii) = m_Ecosim.A(i, j)
    '    Next

    '    'adjust pbbiomass for primary producers
    '    For i = 1 To m_Data.NGroups
    '        If m_ESData.pbm(i) > 0 Then 'primary producer
    '            PbSpace(i) = m_ESData.pbbiomass(i) * m_ESData.StartBiomass(i) / Basebiomass(i)
    '        End If
    '    Next

    'End Sub


    Public Sub CalcHabitatArea()
        Dim i As Integer, j As Integer ', Cnt As Object

        ReDim m_Data.HabArea(m_Data.NoHabitats)
        ReDim m_Data.HabAreaProportion(m_Data.NoHabitats)
        ThabArea = 0

        If m_Data.NoHabitats = 0 Then Exit Sub

        For i = 1 To m_Data.InRow
            For j = 1 To m_Data.InCol
                If m_Data.Depth(i, j) > 0 Then
                    ThabArea = ThabArea + 1
                    m_Data.HabArea(m_Data.HabType(i, j)) = m_Data.HabArea(m_Data.HabType(i, j)) + 1
                End If
            Next
        Next

        '    Cnt = 0
        If ThabArea = 0 Then Exit Sub
        For i = 1 To m_Data.NoHabitats
            m_Data.HabAreaProportion(i) = m_Data.HabArea(i) / ThabArea
            ' If Trim(m_Data.HabitatText(i)) <> "" Then Cnt = Cnt + 1
        Next
        'If Cnt > 0 Then
        '    ReDim habColr(Cnt)
        '    TF = ColorGrad(habColr)
        'End If
        m_Data.HabAreaProportion(0) = 1
    End Sub




    Sub derivtRed(ByVal Biomass() As Single, ByRef Flowin() As Single, ByRef FlowoutRate() As Single, ByRef EatEff() As Single, ByRef VulPred() As Single, ByVal RelProd As Single)
        'reduced derivatives for MPA equilibration procedure
        Dim i As Integer, j As Integer, ii As Integer
        Dim eat As Single, Pmult As Single
        'Dim Vprey As Single
        'Dim Shown As Boolean
        Dim SimGEt As Single
        Dim Dwe As Single
        Dim Bprey As Single

        'Detritus by group is ignored by this version of deritRed(). Each thread has its own version that it uses.
        'So we can declare it localy and never us it to update the detritus map
        Dim GrpDet() As Single
        ReDim GrpDet(m_Data.NGroups)

        Dim aeff() As Single, Veff() As Single
        ReDim aeff(m_SimData.inlinks), Veff(m_SimData.inlinks)

        Dim Hdent() As Single
        ReDim Hdent(m_Data.NGroups)

        'EwE5 ToDetritus() is declared at a global level
        'in EcoSpace this is the only place it is used so its scope is local to EcoSpace
        Dim ToDetritus() As Single
        ReDim ToDetritus(m_Data.NGroups)


        If m_SimData.MedIsUsed(0) Then m_Ecosim.SetMedFunctions(Biomass)

        m_Ecosim.setpred(Biomass)
        ReDim m_SimData.Eatenof(m_Data.NGroups)
        ReDim m_SimData.Eatenby(m_Data.NGroups)

        Dwe = 0.5

        'set ecosim nutrients
        m_SimData.NutBiom = 0
        For i = 1 To m_Data.NGroups
            m_SimData.NutBiom = m_SimData.NutBiom + Biomass(i)
        Next
        m_SimData.NutFree = m_SimData.NutTot * RelProd - m_SimData.NutBiom
        If m_SimData.NutFree < m_SimData.NutMin Then m_SimData.NutFree = m_SimData.NutMin

        If m_SimData.IndicesOn Then
            ReDim m_SimData.Consumpt(m_Data.NGroups, m_Data.NGroups)
        End If

        For j = m_Data.nLiving + 1 To m_Data.NGroups
            ToDetritus(j - m_Data.nLiving) = 0
            'jb DetPassedOn() is not used anywhere
            ' DetPassedOn(j) = 0
        Next j

        m_Ecosim.SetRelaSwitch(Biomass)

        'get first estimate of denominators of predation rate disc equations
        Dim ia As Integer, Vbiom() As Single, Vdenom() As Single
        'this requires first estimates of vulnerable biomasses Vbiom by foraging arena
        ReDim Vbiom(m_SimData.Narena), Vdenom(m_SimData.Narena)
        For ii = 1 To m_SimData.inlinks
            i = m_SimData.ilink(ii) : j = m_SimData.jlink(ii) : ia = m_SimData.ArenaLink(ii)
            aeff(ii) = m_SimData.Alink(ii) * m_SimData.Ftime(j) * m_SimData.RelaSwitch(ii)
            Veff(ia) = m_SimData.VulArena(ia) * m_SimData.Ftime(i)
            m_Ecosim.ApplyAVmodifiers(aeff(ii), Veff(ia), i, m_SimData.Jarena(ia), False)  '?not sure this will work right with multiple preds in arenas
            Vdenom(ia) = Vdenom(ia) + aeff(ii) * m_SimData.pred(j) / m_SimData.Hden(j)
        Next

        'then calculate first estimate using initial Hden estimates of vulnerable biomass in each arena
        For ia = 1 To m_SimData.Narena
            i = m_SimData.Iarena(ia)
            If m_SimData.BoutFeeding Then
                If Vdenom(ia) > 0 Then
                    Vbiom(ia) = Veff(ia) * Biomass(i) * (1 - Math.Exp(-Vdenom(ia))) / Vdenom(ia)
                Else
                    Vbiom(ia) = Veff(ia) * Biomass(i)
                End If
            Else
                Vbiom(ia) = Veff(ia) * Biomass(i) / (m_SimData.VulArena(ia) + Veff(ia) + Vdenom(ia))
            End If
        Next

        'then update hden estimates based on new vulnerable biomass estimates
        For ii = 1 To m_SimData.inlinks
            j = m_SimData.jlink(ii)
            ia = m_SimData.ArenaLink(ii)
            Hdent(j) = Hdent(j) + aeff(ii) * Vbiom(ia)
        Next

        For j = 1 To m_Data.NGroups
            m_SimData.Hden(j) = (1 - Dwe) * (1 + m_SimData.Htime(j) * Hdent(j)) + Dwe * m_SimData.Hden(j)
        Next

        'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
        'then update vulnerable biomass estimates using new Hden estimates (THIS MAY NOT BE NECESSARY?)
        ReDim Vbiom(m_SimData.Narena), Vdenom(m_SimData.Narena)
        For ii = 1 To m_SimData.inlinks
            i = m_SimData.ilink(ii) : j = m_SimData.jlink(ii) : ia = m_SimData.ArenaLink(ii)
            aeff(ii) = aeff(ii) * m_SimData.Ftime(j) * m_SimData.RelaSwitch(ii)
            'see ecosim derivt
            'aeff(ii) = m_ESData.Alink(ii) * m_ESData.Ftime(j) * m_ESData.RelaSwitch(ii)
            Vdenom(ia) = Vdenom(ia) + aeff(ii) * m_SimData.pred(j) / m_SimData.Hden(j)
        Next
        For ia = 1 To m_SimData.Narena
            i = m_SimData.Iarena(ia)
            If m_SimData.BoutFeeding Then
                If Vdenom(ia) > 0 Then
                    Vbiom(ia) = Veff(ia) * Biomass(i) * (1 - Math.Exp(-Vdenom(ia))) / Vdenom(ia)
                Else
                    Vbiom(ia) = Veff(ia) * Biomass(i)
                End If
            Else
                Vbiom(ia) = Veff(ia) * Biomass(i) / (m_SimData.VulArena(ia) + Veff(ia) + Vdenom(ia))
            End If
        Next
        'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

        'then predict consumption flows and cumulative consumptions using the new Vbiom estimates
        For ii = 1 To m_SimData.inlinks
            i = m_SimData.ilink(ii) : j = m_SimData.jlink(ii) : ia = m_SimData.ArenaLink(ii)
            If m_SimData.TrophicOff Then Bprey = m_SimData.StartBiomass(i) Else Bprey = Biomass(i)

            'prey
            ' For j = 1 To N  'VC ignore detritus; CJW had NumGroups 'predator
            '    aeff = A(i, j) * tval(SeasonType(i, j)) * Ftime(j)
            '    Veff = vulrate(i, j) * Ftime(i) * MedVal(MF(i, j))
            Select Case m_SimData.FlowType(i, j) 'prey always first
                Case 1 'donor controlled flow
                    eat = aeff(ii) * Bprey
                Case 3 'limited total flow
                    'MsgBox ("invalid flow control type setting; edit your mdb")
                    eat = aeff(ii) * Bprey * m_SimData.pred(j) / (1 + aeff(ii) * m_SimData.pred(j) * Bprey / m_SimData.maxflow(i, j))
                Case 2 'prey limited flow
                    'Vprey = Veff(ii) * Bprey / (vulrate(i, j) + Veff(ii) + aeff(ii) * pred(j) / Hden(j))
                    eat = aeff(ii) * Vbiom(ia) * m_SimData.pred(j) / m_SimData.Hden(j)
                Case Else
                    eat = 0
            End Select
            m_SimData.Eatenof(i) = m_SimData.Eatenof(i) + eat
            m_SimData.Eatenby(j) = m_SimData.Eatenby(j) + eat
            If m_SimData.IndicesOn Then m_SimData.Consumpt(i, j) = m_SimData.Consumpt(i, j) + eat

            'If frmSim1.IndicesOn Then Consumption(i, j) = Consumption(i, j) + eat
            'ToDetritus = ToDetritus + GS(j) * eat       'DF should be considered

            'jb 
            'If m_ESData.ConSimOn = True Then
            '    If Biomass(i) > 0 Then m_ESData.ConKtrophic(ii) = eat / Biomass(i) Else m_ESData.ConKtrophic(ii) = 0
            'End If

        Next

        'Make the detritus calculations here:
        m_Ecosim.SimDetritusMT(Biomass, m_SimData.FishRateGear, m_SimData.Eatenby, m_SimData.Eatenof, ToDetritus, GrpDet)

        For i = 1 To m_Data.NGroups

            m_SimData.Eatenby(i) = m_SimData.Eatenby(i) + m_SimData.QBoutside(i) * Biomass(i)

            If i <= m_Data.nLiving Then      'Living group
                Pmult = 1.0#
                m_Ecosim.ApplyAVmodifiers(Pmult, Veff(1), i, i, False)
                pbb(i) = Pmult * EatEff(i) * m_SimData.PBmaxs(i) * m_SimData.NutFree / (m_SimData.NutFree + m_SimData.NutFreeBase(i)) * m_SimData.pbm(i) / (1 + Biomass(i) * PbSpace(i))
                'pbb becomes pbmaxs= pb times a max increase factor = pbm for consumers
                loss(i) = m_SimData.Eatenof(i) + (m_SimData.mo(i) * (1 - m_SimData.MoPred(i) + m_SimData.MoPred(i) * m_SimData.Ftime(i)) + m_EPdata.Emig(i) + m_SimData.FishTime(i)) * Biomass(i)
                'deriv(i) = Immig(i) + Biomass(i) * pbb(i) + simGE(i) * Eatenby(i) - loss(i)
                'biomeq(i) = (Immig(i) + simGE(i) * Eatenby(i) + pbb(i) * Biomass(i)) / (loss(i) / Biomass(i))

                'jb change layout so I could read it
                'SimGEt = IIf(m_ESData.UseVarPQ And m_EPdata.vbK(i) > 0, m_ESData.AssimEff(i) * loss(i) / Biomass(i) / (loss(i) / Biomass(i) + 3 * m_EPdata.vbK(i)), m_ESData.SimGE(i))
                If m_SimData.UseVarPQ And m_EPdata.vbK(i) > 0 Then
                    SimGEt = m_SimData.AssimEff(i) * loss(i) / Biomass(i) / (loss(i) / Biomass(i) + 3 * m_EPdata.vbK(i))
                Else
                    SimGEt = m_SimData.SimGE(i)
                End If

                Flowin(i) = m_EPdata.Immig(i) + SimGEt * m_SimData.Eatenby(i) + pbb(i) * Biomass(i)

                If Biomass(i) > 1.0E-20 Then
                    FlowoutRate(i) = loss(i) / Biomass(i)
                Else
                    FlowoutRate(i) = 100
                End If
                'If Abs(Flowin(i) - loss(i)) > 0.1 * loss(i) Then Stop
            Else                'Detritus group
                loss(i) = m_SimData.Eatenof(i) + m_EPdata.Emig(i) + m_SimData.DetritusOut(i) * Biomass(i)
                'deriv(i) = Immig(i) + ToDetritus(i - n) - loss(i)
                If loss(i) <> 0 And Biomass(i) > 0 Then
                    'biomeq(i) = (Immig(i) + ToDetritus(i - n)) / (loss(i) / Biomass(i))
                    Flowin(i) = (m_EPdata.Immig(i) + ToDetritus(i - m_Data.nLiving))
                    FlowoutRate(i) = loss(i) / Biomass(i)
                Else
                    Flowin(i) = 1.0E-20
                    'VC160398 below FlowoutRate(i) was set to 100 before
                    If Biomass(i) > 0 Then
                        FlowoutRate(i) = Flowin(i) / Biomass(i)
                    Else
                        FlowoutRate(i) = 0.0000000001
                    End If
                End If
            End If
        Next

    End Sub

    ''' <summary>
    ''' This function is used to scale the relative primary productivity _
    ''' so that the total primary productivity is the same in Ecospace and Ecopath
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ScaleRelativePrimaryProductivityToEcopathLevel() As Single
        Dim Factor As Single
        Dim i As Integer
        Dim j As Integer

        'This function is used to scale the relative primary productivity _
        'so that the total primary productivity is the same in Ecospace and Ecopath

        m_Data.nWaterCells = 0
        For i = 1 To m_Data.InRow
            For j = 1 To m_Data.InCol
                If m_Data.Depth(i, j) > 0 Then 'Water
                    Factor = Factor + m_Data.RelPP(i, j)
                    m_Data.nWaterCells = m_Data.nWaterCells + 1
                End If
            Next
        Next

        If m_Data.nWaterCells > 0 And Factor > 0 Then
            Return Factor / m_Data.nWaterCells
        Else
            Return 1
        End If

    End Function

    ''' <summary>
    ''' This function is used to scale the sailing cost so that _
    ''' it is the same in Ecospace and Ecopath
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub ScaleSailingToUnity()
        Dim Factor As Single
        Dim i As Integer
        Dim j As Integer
        Dim Count As Long
        Dim GearNo As Integer

        ReDim m_Data.SailScale(m_Data.nFleets)
        Factor = 0
        m_Data.SailScale(0) = 1

        For GearNo = 1 To m_Data.nFleets
            For i = 1 To m_Data.InRow
                For j = 1 To m_Data.InCol
                    If m_Data.Depth(i, j) > 0 Then 'Water
                        Factor = Factor + m_Data.Sail(GearNo, i, j)
                        Count = Count + 1
                    End If
                Next
            Next

            If Count > 0 And Factor > 0 Then
                m_Data.SailScale(GearNo) = Factor / Count
            Else
                m_Data.SailScale(GearNo) = 1
            End If

        Next GearNo

    End Sub


    Private Sub SetEffortParameters()
        'this predicts total effort by gear type over model cells
        'accounting for habitat type restriction of each gear (gearhab(geartype,habitat))
        Dim i As Integer, j As Integer, ig As Integer

        For ig = 1 To m_Data.nFleets
            TotEffort(ig) = 0
            For i = 1 To m_Data.InRow
                For j = 1 To m_Data.InCol
                    'below changed following CJW's email of 20 Jan 98:
                    'I found one bad error in ecospace: subroutine that calculates total
                    'effort (seteffortparameters) has wrong conditions for summing total
                    'effort (should be over all cells where depth>0, remove other conditions),
                    'causing ecospace to reduce effort whenever MPA cells added (should just
                    'redistribute ecopath total, not reduce it at same time).

                    If (m_Data.GearHab(ig, m_Data.HabType(i, j)) Or m_Data.GearHab(ig, 0)) And m_Data.Depth(i, j) > 0 Then
                        TotEffort(ig) = TotEffort(ig) + 1
                    End If

                    ' If m_data.Depth(i, j) > 0 Then TotEffort(ig) = TotEffort(ig) + 1
                Next
            Next
            'TotEffort(ig) = TotEffort(ig) * SEmult(ig)
        Next

    End Sub



    Sub SetMovementParameters()
        'sets solvegrid movement arrays based on depth map
        Dim i As Integer, j As Integer, ip As Integer, AdScale As Single ', iad As Integer, iju As Integer
        Dim isp As Integer, ist As Integer, nvar2 As Integer, ir As Integer, ieco As Integer
        '   Erase Bcw, C, d, e

        ReDim Bcw(m_Data.InRow + 1, m_Data.InCol + 1, m_Data.nvartot)
        ReDim C(m_Data.InRow + 1, m_Data.InCol + 1, m_Data.nvartot)
        ReDim d(m_Data.InRow + 1, m_Data.InCol + 1, m_Data.nvartot)
        ReDim e(m_Data.InRow + 1, m_Data.InCol + 1, m_Data.nvartot)


        AdScale = 1 / m_Data.CellLength '/ (2 * 3.14159 * CellLength)
        For i = 0 To m_Data.InRow
            For j = 0 To m_Data.InCol
                'check depth on right face of this cell
                If m_Data.Depth(i, j) > 0 Then
                    If m_Data.Depth(i, j + 1) > 0 Then

                        For ip = 1 To m_Data.NGroups
                            If j > 0 And j < m_Data.InCol Then
                                e(i, j + 1, ip) = m_Data.Mrate(ip) * RelMove(ip, i, j + 1) * RelHabMove(i, j + 1, i, j, Me.HabGrad, m_Data.MoveScale, ip)
                                d(i, j, ip) = m_Data.Mrate(ip) * RelMove(ip, i, j) * RelHabMove(i, j, i, j + 1, Me.HabGrad, m_Data.MoveScale, ip)
                                If m_Data.IsAdvected(ip) Then
                                    If m_Data.Xvel(i, j) > 0 Then
                                        d(i, j, ip) = d(i, j, ip) + m_Data.Xvel(i, j) * AdScale 'from j to the right
                                    Else
                                        e(i, j + 1, ip) = e(i, j + 1, ip) - m_Data.Xvel(i, j) * AdScale 'into j from right
                                    End If

                                End If
                            Else
                                If m_Data.IsAdvected(ip) Then
                                    If m_Data.Xvel(i, j) > 0 Then
                                        e(i, j + 1, ip) = m_Data.Mrate(ip) 'into j from right
                                        d(i, j, ip) = m_Data.Mrate(ip) + m_Data.Xvel(i, j) * AdScale 'from j to the right
                                    Else
                                        e(i, j + 1, ip) = m_Data.Mrate(ip) - m_Data.Xvel(i, j) * AdScale 'into j from right
                                        d(i, j, ip) = m_Data.Mrate(ip) 'from j to the right

                                    End If
                                Else
                                    e(i, j + 1, ip) = 0
                                    d(i, j, ip) = 0
                                End If
                            End If
                            Enomig(i, j + 1, ip) = e(i, j + 1, ip)
                            dNomig(i, j, ip) = d(i, j, ip)
                        Next
                        'If npairs > 0 Then
                        '    For ip = 1 To npairs : iad = iadult(ip) : iju = ijuv(ip)
                        '        e(i, j + 1, nvar + ip) = e(i, j + 1, iad)
                        '        d(i, j, nvar + ip) = d(i, j, iad)
                        '        e(i, j + 1, nvar + npairs + ip) = e(i, j + 1, iju)
                        '        d(i, j, nvar + npairs + ip) = d(i, j, iju)
                        '        Enomig(i, j + 1, nvar + ip) = e(i, j + 1, iad)
                        '        dNomig(i, j, nvar + ip) = d(i, j, iad)
                        '        Enomig(i, j + 1, nvar + npairs + ip) = e(i, j + 1, iju)
                        '        dNomig(i, j, nvar + npairs + ip) = d(i, j, iju)
                        '    Next
                        'End If

                        'EwE5
                        ' nvar2 = nvar + 2 * npairs
                        nvar2 = m_Data.NGroups
                        ir = 0
                        For isp = 1 To m_Stanza.Nsplit
                            For ist = 1 To m_Stanza.Nstanza(isp)
                                ieco = m_Stanza.EcopathCode(isp, ist)
                                ir = ir + 1
                                e(i, j + 1, nvar2 + ir) = e(i, j + 1, ieco)
                                d(i, j, nvar2 + ir) = d(i, j, ieco)
                                Enomig(i, j + 1, nvar2 + ir) = e(i, j + 1, ieco)
                                dNomig(i, j, nvar2 + ir) = d(i, j, ieco)
                            Next
                        Next
                    End If
                    'then check depths on bottom face of this cell
                    If m_Data.Depth(i + 1, j) > 0 Then
                        For ip = 1 To m_Data.NGroups
                            If i > 0 And i < m_Data.InRow Then
                                C(i, j, ip) = m_Data.Mrate(ip) * RelMove(ip, i + 1, j) * RelHabMove(i + 1, j, i, j, HabGrad, m_Data.MoveScale, ip)
                                Bcw(i + 1, j, ip) = m_Data.Mrate(ip) * RelMove(ip, i, j) * RelHabMove(i, j, i + 1, j, HabGrad, m_Data.MoveScale, ip)
                                If m_Data.IsAdvected(ip) Then
                                    If m_Data.Yvel(i, j) > 0 Then
                                        Bcw(i + 1, j, ip) = Bcw(i + 1, j, ip) + m_Data.Yvel(i, j) * AdScale 'from j to the right
                                    Else
                                        C(i, j, ip) = C(i, j, ip) - m_Data.Yvel(i, j) * AdScale   'into j from right
                                    End If

                                End If
                            Else
                                If m_Data.IsAdvected(ip) Then
                                    If m_Data.Yvel(i, j) > 0 Then
                                        C(i, j, ip) = m_Data.Mrate(ip) 'from row i+1 to i
                                        Bcw(i + 1, j, ip) = m_Data.Mrate(ip) + m_Data.Yvel(i, j) * AdScale ' + AdvectSouth 'from i to i+1
                                    Else
                                        C(i, j, ip) = m_Data.Mrate(ip) - m_Data.Yvel(i, j) * AdScale 'from row i+1 to i
                                        Bcw(i + 1, j, ip) = m_Data.Mrate(ip)
                                    End If
                                Else
                                    C(i, j, ip) = 0
                                    Bcw(i + 1, j, ip) = 0
                                End If
                            End If
                            CNomig(i, j, ip) = C(i, j, ip)
                            BcwNomig(i + 1, j, ip) = Bcw(i + 1, j, ip)
                        Next
                        'If npairs > 0 Then
                        '    For ip = 1 To npairs : iad = iadult(ip) : iju = ijuv(ip)
                        '        Bcw(i + 1, j, nvar + ip) = Bcw(i + 1, j, iad)
                        '        C(i, j, nvar + ip) = C(i, j, iad)
                        '        Bcw(i + 1, j, nvar + npairs + ip) = Bcw(i + 1, j, iju)
                        '        C(i, j, nvar + npairs + ip) = C(i, j, iju)
                        '        BcwNomig(i + 1, j, nvar + ip) = Bcw(i + 1, j, iad)
                        '        CNomig(i, j, nvar + ip) = C(i, j, iad)
                        '        BcwNomig(i + 1, j, nvar + npairs + ip) = Bcw(i + 1, j, iju)
                        '        CNomig(i, j, nvar + npairs + ip) = C(i, j, iju)
                        '    Next
                        'End If

                        'EwE5
                        ' nvar2 = nvar + 2 * npairs
                        nvar2 = m_Data.NGroups
                        ir = 0
                        For isp = 1 To m_Stanza.Nsplit
                            For ist = 1 To m_Stanza.Nstanza(isp)
                                ieco = m_Stanza.EcopathCode(isp, ist)
                                ir = ir + 1
                                Bcw(i + 1, j, nvar2 + ir) = Bcw(i + 1, j, ieco)
                                C(i, j, nvar2 + ir) = C(i, j, ieco)
                                BcwNomig(i + 1, j, nvar2 + ir) = Bcw(i + 1, j, ieco)
                                CNomig(i, j, nvar2 + ir) = C(i, j, ieco)
                            Next
                        Next
                    End If
                End If

            Next j
        Next i

        If m_tracerData.EcoSpaceConSimOn Then
            'set movement rates for physical contaminant concentration to
            'rates for first detritus pool
            For i = 0 To m_Data.InRow + 1
                For j = 0 To m_Data.InCol + 1
                    Bcw(i, j, 0) = Bcw(i, j, m_EPdata.NumLiving + 1)
                    C(i, j, 0) = C(i, j, m_EPdata.NumLiving + 1)
                    d(i, j, 0) = d(i, j, m_EPdata.NumLiving + 1)
                    e(i, j, 0) = e(i, j, m_EPdata.NumLiving + 1)
                    BcwNomig(i, j, 0) = Bcw(i, j, m_EPdata.NumLiving + 1)
                    CNomig(i, j, 0) = C(i, j, m_EPdata.NumLiving + 1)
                    dNomig(i, j, 0) = d(i, j, m_EPdata.NumLiving + 1)
                    Enomig(i, j, 0) = e(i, j, m_EPdata.NumLiving + 1)
                Next
            Next
        End If
    End Sub



    Function RelMove(ByVal ip As Integer, ByVal i As Integer, ByVal j As Integer) As Single
        'calculates relative movemement rate out of cell i,j for pool/species ip, as function of
        'habitat state in cell i,j
        If m_Data.PrefHab(ip, m_Data.HabType(i, j)) Or m_Data.PrefHab(ip, 0) Then
            RelMove = 1
        Else
            RelMove = m_Data.RelMoveBad(ip)
        End If

    End Function


    Function RelHabMove(ByVal i1 As Integer, ByVal j1 As Integer, ByVal i2 As Integer, ByVal j2 As Integer, ByVal G(,,) As Single, ByVal gk As Single, ByVal ihab As Integer) As Single
        'sets relative movement rate using slope of g() function between origin (i1,j1) and destination (i2,j2) cells
        'function is 1 when slope ss is zero
        Dim Ss As Single
        Ss = G(i2, j2, ihab) - G(i1, j1, ihab)
        Select Case Ss
            Case 0
                RelHabMove = 1
            Case Is > 0
                RelHabMove = 2 / (1 + Math.Exp(-gk * Ss))
            Case Is < 0
                RelHabMove = 0.01
            Case Else
                Stop
        End Select
    End Function






    Sub VaryMovementParameters(ByVal imonth As Integer, ByVal ip As Integer, ByVal ieco As Integer)
        'EwE5 definition IsIad and IsIju indexes remove these are iAdult and iJuvenial indexes for the split pool code
        'Sub VaryMovementParameters(ByVal imonth As Integer, ByVal ip As Integer, ByVal IsIad As Integer, ByVal IsIju As Integer, ByVal ieco As Integer)

        'sets solvegrid movement arrays based on depth map
        Dim i As Integer, j As Integer, AdScale As Single
        Dim nvar2 As Integer, ir As Integer, Distort As Single
        Dim Ep As Single
        Dim MaxCh As Single
        Dim FitRatio As Single
        AdScale = 1 '/ (2 * 3.14159 * CellLength)
        MaxCh = 1

        'calculate relative emigration rate from each cell as function
        'of fitness, scaling parameter KmoveFit(ip) set in setKmove routine
        For i = 0 To m_Data.InRow + 1
            For j = 0 To m_Data.InCol + 1
                If m_Data.FitRespType > 0 Then
                    Ep = -Kmovefit(ip) * RelFitness(i, j, ip)
                    If Ep < -MaxCh Then Ep = -MaxCh
                    If Ep > MaxCh Then Ep = MaxCh
                    Ep = Math.Exp(Ep)
                    RelMoveFit(i, j) = 2.0# * Ep / (1 + Ep)
                    '        If ip = 18 And imonth > 1 And i > 30 And i <  m_data.inrow + 1 And j > 1 And j < 5 Then Stop
                Else
                    RelMoveFit(i, j) = 1
                End If
            Next
        Next

        For i = 0 To m_Data.InRow
            For j = 0 To m_Data.InCol
                If m_Data.Depth(i, j) > 0 Then

                    'check depth on right face of this cell
                    If m_Data.Depth(i, j + 1) > 0 Then

                        If MigPowj(ip, j) > 0 Then
                            Distort = 2 * MigPowj(ip, j) / (PrefColP(ip, imonth) + MigPowj(ip, j))
                        Else
                            Distort = 1
                        End If

                        If m_Data.FitRespType < 2 Then
                            e(i, j + 1, ip) = Enomig(i, j + 1, ip) * RelMoveFit(i, j + 1) * (Distort)
                            d(i, j, ip) = dNomig(i, j, ip) * RelMoveFit(i, j) * (2 - Distort)
                        Else
                            FitRatio = RelMoveFit(i, j + 1) / RelMoveFit(i, j)
                            e(i, j + 1, ip) = Enomig(i, j + 1, ip) * FitRatio * (Distort)
                            d(i, j, ip) = dNomig(i, j, ip) / FitRatio * (2 - Distort)
                        End If

                        If j = 0 Or j = m_Data.InCol Then
                            e(i, j + 1, ip) = 0
                            d(i, j, ip) = 0
                        End If

                        'jb split pool code removed
                        'If IsIad > 0 Then
                        '    e(i, j + 1, nvar + IsIad) = e(i, j + 1, ip)
                        '    d(i, j, nvar + IsIad) = d(i, j, ip)
                        'End If
                        'If IsIju > 0 Then
                        '    e(i, j + 1, nvar + npairs + IsIju) = e(i, j + 1, ip)
                        '    d(i, j, nvar + npairs + IsIju) = d(i, j, ip)
                        'End If

                        nvar2 = m_Data.NGroups
                        If ieco > 0 Then
                            ir = IecoCode(ip)
                            e(i, j + 1, nvar2 + ir) = e(i, j + 1, ip)
                            d(i, j, nvar2 + ir) = d(i, j, ip)
                            'Enomig(i, j + 1, nvar2 + ir) = E(i, j + 1, ieco)
                            'dNomig(i, j, nvar2 + ir) = d(i, j, ieco)
                        End If
                    End If ' If m_Data.Depth(i, j + 1) > 0 Then check depth on right face of this cell

                    'then check depths on bottom face of this cell
                    If m_Data.Depth(i + 1, j) > 0 Then
                        If MigPowi(ip, i) > 0 Then
                            Distort = 2 * MigPowi(ip, i) / (PrefRowP(ip, imonth) + MigPowi(ip, i))
                        Else
                            Distort = 1
                        End If

                        If m_Data.FitRespType < 2 Then
                            C(i, j, ip) = CNomig(i, j, ip) * Distort * RelMoveFit(i + 1, j)
                            Bcw(i + 1, j, ip) = BcwNomig(i + 1, j, ip) * RelMoveFit(i, j) * (2 - Distort)
                        Else
                            FitRatio = RelMoveFit(i + 1, j) / RelMoveFit(i, j)
                            C(i, j, ip) = CNomig(i, j, ip) * Distort * FitRatio
                            Bcw(i + 1, j, ip) = BcwNomig(i + 1, j, ip) / FitRatio * (2 - Distort)
                        End If

                        If i = 0 Or i = m_Data.InRow Then
                            C(i, j, ip) = 0
                            Bcw(i + 1, j, ip) = 0
                        End If

                        ''jb split pool code removed
                        'If IsIad > 0 Then
                        '    Bcw(i + 1, j, nvar + IsIad) = Bcw(i + 1, j, ip)
                        '    C(i, j, nvar + IsIad) = C(i, j, ip)
                        'End If
                        'If IsIju > 0 Then
                        '    Bcw(i + 1, j, nvar + npairs + IsIju) = Bcw(i + 1, j, ip)
                        '    C(i, j, nvar + npairs + IsIju) = C(i, j, ip)
                        'End If

                        If ieco > 0 Then
                            ir = ieco
                            Bcw(i + 1, j, nvar2 + ir) = Bcw(i + 1, j, ip)
                            C(i, j, nvar2 + ir) = C(i, j, ip)
                        End If
                    End If 'If m_Data.Depth(i + 1, j) > 0 Then then check depths on bottom face of this cell

                End If 'If m_Data.Depth(i, j) > 0 Then

            Next j
        Next i

    End Sub


    Sub SolveGrid(ByVal ip As Integer, ByVal Aloc(,,) As Single, ByVal Floc(,,) As Single, ByVal X(,,) As Single, ByVal M As Integer, ByVal NomCols As Integer, ByVal Tol As Single, ByVal jord() As Integer, ByVal W As Single)
        'this routine solves for equilibrium field of concentrations x over a grid
        ' x(i,j) is equilibrium concentration of x in grid cell i,j
        'am(i,j) is total loss rate of x from cell i,j...NB:am(i,j)<0 !!!!!!
        'b(i,j) is loss rate from element i-1 to i in column j of grid
        'c(i,j) is loss rate from element i+1 to i in column j of grid
        'd(i,j) is loss rate from element j to element j+1 in row i of grid
        'e(i,j) is loss rate from element j to element j-1 in row i of grid
        'f(i,j) is forcing input to element i,j from sources outside the grid
        'm is number of rows (i) in grid
        'NomCols is number of columns (j) in grid
        'tol is tolerance limit for change in iterative solution
        'jord(k) is which column j to do as k=1, k=2,...,k=n (iteration order)
        'w is SOR overrelaxation parameter-found 1.25 to be good for typical problems
        Dim iter As Integer, j As Integer, i As Integer, jj As Integer, ic As Integer

        Dim alfa(,) As Single
        Dim gam(,) As Single
        Dim rhs(,) As Single
        Dim G() As Single
        Dim Xold(,) As Single
        ReDim alfa(M + 1, NomCols + 1)
        ReDim gam(M + 1, NomCols + 1)
        ReDim rhs(M + 1, NomCols + 1)
        ReDim G(M + 1)
        ReDim Xold(M + 1, NomCols + 1)

        'first compute LU decomposition elements for each column j
        'If StopRun = 1 Then Exit Sub
        For j = 1 To NomCols
            Xold(1, j) = X(1, j, ip)
            alfa(1, j) = Aloc(1, j, ip)
            gam(1, j) = C(1, j, ip) / alfa(1, j)
            For i = 2 To M
                Xold(i, j) = X(i, j, ip)
                alfa(i, j) = Aloc(i, j, ip) - Bcw(i, j, ip) * gam(i - 1, j)
                gam(i, j) = C(i, j, ip) / alfa(i, j)
            Next
        Next
        'now begin block Gauss-Seidel/SOR iteration over columns of grid
        'at each iteration, solve explicitly for values in each column given
        'current estimates of "forcing" input from other columns based on their
        'current estimates
        iter = 0
iterate:
        For jj = 1 To NomCols

            j = jord(jj)
            For i = 1 To M
                rhs(i, j) = -Floc(i, j, ip) - d(i, j - 1, ip) * X(i, j - 1, ip) - e(i, j + 1, ip) * X(i, j + 1, ip)
            Next
            rhs(1, j) = rhs(1, j) - Bcw(1, j, ip) * X(0, j, ip)
            rhs(M, j) = rhs(M, j) - C(M, j, ip) * X(M + 1, j, ip)
            'now solve for x(i,j) over i using these forcing inputs to one dimensional
            'tridiagonal solver
            G(1) = rhs(1, j) / alfa(1, j)
            'IF iflag > 0 THEN FOR i = 1 TO m: PRINT x(i, j), xold(i, j): NEXT: STOP
            For i = 2 To M
                G(i) = (rhs(i, j) - Bcw(i, j, ip) * G(i - 1)) / alfa(i, j)
            Next
            X(M, j, ip) = G(M)
            For i = M - 1 To 1 Step -1
                X(i, j, ip) = G(i) - gam(i, j) * X(i + 1, j, ip)
            Next
            'IF iflag > 0 THEN
            '        FOR i = 1 TO m: PRINT x(i, j), xold(i, j): NEXT
            '        PRINT FRE(-1), FRE(-2)
            '        : STOP
            'END IF
            For i = 1 To M
                X(i, j, ip) = (1 - W) * Xold(i, j) + W * X(i, j, ip)
            Next
        Next

        ic = 0
        For i = 1 To M
            For j = 1 To NomCols
                If m_Data.Depth(i, j) > 0 Then

                    If Math.Abs(X(i, j, ip) - Xold(i, j)) > Tol Then
                        ic = ic + 1
                    End If
                    Xold(i, j) = X(i, j, ip)
                    If Math.Abs(Xold(i, j)) < 1.0E-20 Then
                        Xold(i, j) = 0
                    End If

                End If
            Next
        Next
        'LOCATE 1, 1: Print "SOR it="; iter;: LOCATE 2, 1: Print "    nc="; ic;
        ' Label12.Caption = iter : Label13.Caption = ic 'DoEvents
        iter = iter + 1
        If ic > 0 And iter < 20 Then GoTo iterate
        'CLS
        'LOCATE 1, 1
        'FOR i = 1 TO 20: PRINT USING "## "; i; : FOR j = 1 TO nomcols: PRINT USING " .##"; x(i, j); : NEXT: PRINT : NEXT
        'WHILE INKEY$ = "": WEND
exitline:
        Erase alfa, gam, rhs, G, Xold

    End Sub

    Sub SolveGridRow(ByVal ip As Integer, ByVal Aloc(,,) As Single, ByVal Floc(,,) As Single, ByVal X(,,) As Single, ByVal M As Integer, ByVal NomCols As Integer, ByVal Tol As Single, ByVal jord() As Integer, ByVal W As Single)
        'this routine solves for equilibrium field of concentrations x over a grid
        ' x(i,j) is equilibrium concentration of x in grid cell i,j
        'am(i,j) is total loss rate of x from cell i,j...NB:am(i,j)<0 !!!!!!
        'b(i,j) is loss rate from element i-1 to i in column j of grid
        'c(i,j) is loss rate from element i+1 to i in column j of grid
        'd(i,j) is loss rate from element j to element j+1 in row i of grid
        'e(i,j) is loss rate from element j to element j-1 in row i of grid
        'f(i,j) is forcing input to element i,j from sources outside the grid
        'm is number of rows (i) in grid
        'NomCols is number of columns (j) in grid
        'tol is tolerance limit for change in iterative solution
        'jord(k) is which column j to do as k=1, k=2,...,k=n (iteration order)
        'w is SOR overrelaxation parameter-found 1.25 to be good for typical problems
        Dim iter As Integer, j As Integer, i As Integer, ic As Integer ', ii As Integer

        Dim alfa(,) As Single
        Dim gam(,) As Single
        Dim rhs(,) As Single
        Dim G() As Single
        Dim Xold(,) As Single
        ReDim alfa(M + 1, NomCols + 1)
        ReDim gam(M + 1, NomCols + 1)
        ReDim rhs(M + 1, NomCols + 1)
        ReDim G(NomCols + 1)
        ReDim Xold(M + 1, NomCols + 1)

        'first compute LU decomposition elements for each column j
        'If StopRun = 1 Then Exit Sub

        For i = 1 To M
            Xold(i, 1) = X(i, 1, ip)
            alfa(i, 1) = Aloc(i, 1, ip) : gam(i, 1) = e(i, 2, ip) / alfa(i, 1)
            For j = 2 To NomCols
                Xold(i, j) = X(i, j, ip)
                alfa(i, j) = Aloc(i, j, ip) - d(i, j - 1, ip) * gam(i, j - 1)
                gam(i, j) = e(i, j + 1, ip) / alfa(i, j)
            Next
        Next
        'now begin block Gauss-Seidel/SOR iteration over columns of grid
        'at each iteration, solve explicitly for values in each column given
        'current estimates of "forcing" input from other columns based on their
        'current estimates
        iter = 0
iterate:
        For i = 1 To M
            ' If StopRun = 1 Then Exit Sub
            'j = jord(jj)
            For j = 1 To NomCols
                rhs(i, j) = -Floc(i, j, ip) - Bcw(i, j, ip) * X(i - 1, j, ip) - C(i, j, ip) * X(i + 1, j, ip)
            Next
            rhs(i, 1) = rhs(i, 1) - d(i, 0, ip) * X(i, 0, ip)
            rhs(i, NomCols) = rhs(i, NomCols) - e(i, NomCols + 1, ip) * X(i, NomCols + 1, ip)
            'now solve for x(i,j) over i using these forcing inputs to one dimensional
            'tridiagonal solver
            G(1) = rhs(i, 1) / alfa(i, 1)
            'IF iflag > 0 THEN FOR i = 1 TO m: PRINT x(i, j), xold(i, j): NEXT: STOP
            For j = 2 To NomCols
                G(j) = (rhs(i, j) - d(i, j - 1, ip) * G(j - 1)) / alfa(i, j)
            Next
            X(i, NomCols, ip) = G(NomCols)
            For j = NomCols - 1 To 1 Step -1
                X(i, j, ip) = G(j) - gam(i, j) * X(i, j + 1, ip)
            Next
            'IF iflag > 0 THEN
            '        FOR i = 1 TO m: PRINT x(i, j), xold(i, j): NEXT
            '        PRINT FRE(-1), FRE(-2)
            '        : STOP
            'END IF
            For j = 1 To NomCols
                X(i, j, ip) = (1 - W) * Xold(i, j) + W * X(i, j, ip)
            Next
        Next

        ic = 0
        For i = 1 To M
            For j = 1 To NomCols
                If m_Data.Depth(i, j) > 0 Then

                    If Math.Abs(X(i, j, ip) - Xold(i, j)) > Tol Then
                        ic = ic + 1
                    End If
                    Xold(i, j) = X(i, j, ip)
                    If Math.Abs(Xold(i, j)) < 1.0E-20 Then
                        Xold(i, j) = 0 ': Stop
                    End If

                End If
            Next j
        Next i
        'LOCATE 1, 1: Print "SOR it="; iter;: LOCATE 2, 1: Print "    nc="; ic;
        ' Label12.Caption = iter: Label13.Caption = ic: 'DoEvents
        iter = iter + 1
        If ic > 0 And iter < 20 Then GoTo iterate
        'CLS
        'LOCATE 1, 1
        'FOR i = 1 TO 20: PRINT USING "## "; i; : FOR j = 1 TO nomcols: PRINT USING " .##"; x(i, j); : NEXT: PRINT : NEXT
        'WHILE INKEY$ = "": WEND
exitline:
        Erase alfa, gam, rhs, G, Xold

    End Sub


    Sub PredictEffortDistribution(ByVal iMonth As Integer, ByVal iCumMonth As Integer)

        'ToDo_jb PredictEffortDistribution in EwE5 the cumulative month counter (iCumMonth) starts at zero
        'on the first call Month = Zero not one!!!! 
        'this means that the values retrieved from FishRateGear(igear, month) are indexed from zero

        'this routine predicts spatial effort and fishing mortality rate
        'distribution by gear type; called at each iteration
        'step in finding biomass spatial equilibrium
        'model below is a gravity attraction model, distributing
        'total efforts TotEffort(gear) over all cells where each gear can fish
        'in proportion to relative profitability (catch rate x price sum) for that cell for the gear
        Dim ig As Integer, i As Integer, j As Integer, TotAttract As Single
        Dim Valt As Single, isp As Integer
        '  Dim PresentHabitat As Integer
        Dim Effort() As Single
        Dim EffortCost As Single
        Dim SailCost As Single
        Static NoSailing As Boolean, TotE As Single


        ReDim Effort(m_Data.nFleets)
        ReDim m_Data.Ftot(m_Data.NGroups, m_Data.InRow, m_Data.InCol)
        ReDim m_Data.EffortSpace(m_Data.nFleets, m_Data.InRow, m_Data.InCol)

        'replaced by iMonth
        '   MM = (Month()) Mod 12 + 1 'used for mpaseason business
        For ig = 1 To m_Data.nFleets
            TotE = TotEffort(ig) * m_Data.SEmult(ig)
            Effort(ig) = 0
            'jb Attract() gets cleared out for each fleet
            ReDim m_Data.Attract(m_Data.InRow, m_Data.InCol)
            TotAttract = 0.0000000001

            'Introduce a factor which balances fixed and sailingcost: (up to 02Jan02 the next if then was in the loop over spatial cells below, no need for this)
            If m_EPdata.cost(ig, eCostIndex.CUPE) + m_EPdata.cost(ig, eCostIndex.Sail) = 0 Then
                EffortCost = 0
                SailCost = 1
                If NoSailing = False Then
                    ''ToDo_jb Feedback Message?? or some other type of message that gets posted immediately
                    'MsgBox("No variable or sailing cost has been specified for " + GearName(ig), vbInformation + vbOKOnly, "Check cost of fishing in Ecopath")
                    NoSailing = True
                End If
            Else
                EffortCost = m_EPdata.cost(ig, eCostIndex.CUPE) / (m_EPdata.cost(ig, eCostIndex.Fixed) + m_EPdata.cost(ig, eCostIndex.CUPE) + m_EPdata.cost(ig, eCostIndex.Sail))
                SailCost = m_EPdata.cost(ig, eCostIndex.Sail) / (m_EPdata.cost(ig, eCostIndex.Fixed) + m_EPdata.cost(ig, eCostIndex.CUPE) + m_EPdata.cost(ig, eCostIndex.Sail))
            End If

            For i = 1 To m_Data.InRow
                For j = 1 To m_Data.InCol
                    If m_Data.MPA(i, j) > m_Data.MPAno Then m_Data.MPA(i, j) = 0 'This type of MPA may have been deleted
                    If m_Data.Depth(i, j) > 0 And _
                        (m_Data.MPA(i, j) = 0 Or m_Data.MPAfishery(ig, m_Data.MPA(i, j)) Or m_Data.MPAmonth(iMonth, m_Data.MPA(i, j))) _
                        And (m_Data.GearHab(ig, m_Data.HabType(i, j)) Or m_Data.GearHab(ig, 0)) Then
                        'If Depth(i, j) > 0 And MPA(i, j) = 0 And (PresentHabitat = GearHab(ig, PresentHabitat) Or GearHab(ig, PresentHabitat) = 0) Then
                        'mpamonth(mpatype, month) is false if closed, True if open.
                        Valt = 0
                        For isp = 1 To m_Data.NGroups
                            Valt = Valt + m_EPdata.Market(ig, isp) * m_Data.Bcell(i, j, isp) * m_SimData.relQ(ig, isp)
                        Next
                        If m_Data.Sail(ig, i, j) = 0 Then m_Data.Sail(ig, i, j) = 0.000001
                        'VC Sail() above: to avoid dividing with zero
                        Valt = (Valt ^ m_Data.EffPower(ig)) / (EffortCost + SailCost * m_Data.Sail(ig, i, j) / m_Data.SailScale(ig))
                        m_Data.Attract(i, j) = Valt 'may want to modify this by dividing by a site cost factor for cell i,j
                        TotAttract = TotAttract + Valt
                    End If
                Next
            Next

            For i = 1 To m_Data.InRow
                For j = 1 To m_Data.InCol
                    'VC19Aug98: Fishing in water, not in MPA unless the MPA is fished, and only if this gear operate in this habitat or in all habitats
                    If m_Data.Depth(i, j) > 0 And _
                        (m_Data.MPA(i, j) = 0 Or m_Data.MPAfishery(ig, m_Data.MPA(i, j)) Or m_Data.MPAmonth(iMonth, m_Data.MPA(i, j))) _
                        And (m_Data.GearHab(ig, m_Data.HabType(i, j)) Or m_Data.GearHab(ig, 0)) Then
                        '  water               (not MPA       or fished MPA)     and ( habitat       =  fish here or  this gear doen not fish here??)
                        'EffortSpace(ig, i, j) = TotEffort(ig) * Attract(i, j) / TotAttract
                        'VC/080499 Above changed per CJWs advice to reflect effort change over time in Ecospace
                        m_Data.EffortSpace(ig, i, j) = m_SimData.FishRateGear(ig, iCumMonth) * TotE * m_Data.Attract(i, j) / TotAttract
                        Effort(ig) = Effort(ig) + m_Data.EffortSpace(ig, i, j)
                        For isp = 1 To m_Data.NGroups
                            m_Data.Ftot(isp, i, j) = m_Data.Ftot(isp, i, j) + m_Data.EffortSpace(ig, i, j) * m_SimData.relQ(ig, isp)
                        Next
                    End If
                Next
            Next
        Next
    End Sub

    ''' <summary>
    ''' solvetime() is not called at this time. It has been left in for reference
    ''' </summary>
    ''' <param name="ip"></param>
    ''' <param name="Aloc"></param>
    ''' <param name="Floc"></param>
    ''' <param name="X"></param>
    ''' <param name="M"></param>
    ''' <param name="NomCols"></param>
    ''' <param name="Tol"></param>
    ''' <param name="jord"></param>
    ''' <param name="Dt"></param>
    ''' <remarks></remarks>
    Sub solvetime(ByVal ip As Integer, ByVal Aloc(,,) As Single, ByVal Floc(,,) As Single, ByVal X(,,) As Single, ByVal M As Integer, ByVal NomCols As Integer, ByVal Tol As Single, ByVal jord() As Integer, ByVal Dt As Single)
        Dim i As Integer, j As Integer, Xold(,) As Single
        ReDim Xold(m_Data.InRow + 1, m_Data.InCol + 1)
        For i = 0 To M + 1
            For j = 0 To NomCols + 1
                Xold(i, j) = X(i, j, ip)
            Next
        Next
        For i = 1 To M
            For j = 1 To NomCols
                X(i, j, ip) = (1 / (1 - Aloc(i, j, ip) * Dt)) * (Xold(i, j) + Dt * (Floc(i, j, ip) + Bcw(i, j, ip) * Xold(i - 1, j) + C(i, j, ip) * Xold(i + 1, j) + d(i, j - 1, ip) * Xold(i, j - 1) + e(i, j + 1, ip) * Xold(i, j + 1)))
            Next
        Next
    End Sub


#End Region

#Region "Data summary"

    ''' <summary>
    ''' Accumulate the fisheries data (catch) for a single group for this map cell. 
    ''' This is called before DerivtRed(), in the time step, so it is the condition at the start of the time step.
    ''' </summary>
    ''' <param name="Biomass">Biomass for all the groups at this time step</param>
    ''' <param name="iRow">Map row</param>
    ''' <param name="iCol">Map col</param>
    ''' <remarks></remarks>
    Public Sub accumCatchData(ByVal iCumTime As Integer, ByVal Biomass() As Single, ByVal iRow As Integer, ByVal iCol As Integer)
        Dim sum As Single, iFlt As Integer

        'Only one thread can use this code at a time
        'block all others
        Me.m_SpaceCatchSemaphor.WaitOne()

        Try
            '     iSumIndex = 0
            'summarize the data if this timestep is part of the start or end time period
            'iSumIndex will = -1 if this timestep is not being summarized
            '     If iSumIndex >= 0 Then

            For iFlt = 1 To m_Data.nFleets
                'Effort
                m_Data.ResultsByFleet(eSpaceResultsFleets.FishingEffort, iFlt, iCumTime) += m_Data.EffortSpace(iFlt, iRow, iCol)
                'SailingEffort: at this point SailingEffort is  sum of [fishing effort] * [effort of fishing each cell (Sail(iFlt, iRow, iCol))] /  SailScale(ifleet)
                'Effort of fishing all the cells
                m_Data.ResultsByFleet(eSpaceResultsFleets.SailingEffort, iFlt, iCumTime) += (m_Data.EffortSpace(iFlt, iRow, iCol) * m_Data.Sail(iFlt, iRow, iCol) / m_Data.SailScale(iFlt))

                'sum values into All Fleets 0 index 
                m_Data.ResultsByFleet(eSpaceResultsFleets.FishingEffort, 0, iCumTime) += m_Data.ResultsByFleet(eSpaceResultsFleets.FishingEffort, iFlt, iCumTime)
                m_Data.ResultsByFleet(eSpaceResultsFleets.SailingEffort, 0, iCumTime) += m_Data.ResultsByFleet(eSpaceResultsFleets.SailingEffort, iFlt, iCumTime)

                ''To get the original effort the effortspace is divided by the fishrategear for the month
                'If m_ESData.FishRateGear(iFlt, iCumTime) > 0 Then
                '    m_Data.SumCostInit(iSumIndex, iFlt) = m_Data.SumCostInit(iSumIndex, iFlt) + m_Data.EffortSpace(iFlt, iRow, iCol) / m_ESData.FishRateGear(iFlt, iCumTime) * m_Data.Sail(iFlt, iRow, iCol) / m_Data.SailScale(iFlt)
                'End If

            Next

            For igrp As Integer = 1 To Me.m_Data.NGroups

                'If m_Data.NoRegions > 0 Then
                '    m_Data.ResultsRegionGroup(m_Data.Region(iRow, iCol), igrp, iCumTime) += Biomass(igrp)
                'End If

                If m_EPdata.fCatch(igrp) > 0 Then
                    m_Data.ResultsByGroup(eSpaceResultsGroups.CatchBio, igrp, iCumTime) = m_Data.ResultsByGroup(eSpaceResultsGroups.CatchBio, igrp, iCumTime) + Biomass(igrp) * m_SimData.FishTime(igrp)
                    'Next value of catch, depends on what gear was used:
                    For iFlt = 1 To m_EPdata.NumFleet
                        If m_EPdata.Landing(iFlt, igrp) + m_EPdata.Discard(iFlt, igrp) > 0 Then
                            'First get catch
                            sum = Biomass(igrp) * m_Data.EffortSpace(iFlt, iRow, iCol) * m_SimData.relQ(iFlt, igrp)
                            'Sum the total catch by gear
                            m_Data.ResultsByFleet(eSpaceResultsFleets.CatchBio, iFlt, iCumTime) += sum
                            'sum all fleets
                            m_Data.ResultsByFleet(eSpaceResultsFleets.CatchBio, 0, iCumTime) += sum

                            m_Data.ResultsByFleetGroup(eSpaceResultsFleetsGroups.CatchBio, iFlt, igrp, iCumTime) += sum
                            'sum all fleets into the zero fleet index
                            m_Data.ResultsByFleetGroup(eSpaceResultsFleetsGroups.CatchBio, 0, igrp, iCumTime) += sum

                            'Next line is for adding up catch by region etc
                            If m_Data.NoRegions > 0 Then
                                m_Data.ResultsCatchRegionGearGroup(m_Data.Region(iRow, iCol), iFlt, igrp, iCumTime) += sum
                            End If
                            'Then multily with marketvalue * prop landed
                            sum = sum * m_EPdata.Market(iFlt, igrp) * m_EPdata.Landing(iFlt, igrp) / (m_EPdata.Landing(iFlt, igrp) + m_EPdata.Discard(iFlt, igrp))

                            'And add to group and to gear sums
                            m_Data.ResultsByFleetGroup(eSpaceResultsFleetsGroups.Value, iFlt, igrp, iCumTime) += sum
                            'sum of all fleets
                            m_Data.ResultsByFleetGroup(eSpaceResultsFleetsGroups.Value, 0, igrp, iCumTime) += sum

                            m_Data.ResultsByFleet(eSpaceResultsFleets.Value, iFlt, iCumTime) += sum
                            m_Data.ResultsByFleet(eSpaceResultsFleets.Value, 0, iCumTime) += sum
                        End If
                    Next iFlt
                End If 'If m_EPdata.fCatch(igrp) > 0 Then
            Next igrp
            '        End If

        Catch ex As Exception
            cLog.Write(ex)
        End Try

        Me.m_SpaceCatchSemaphor.Release()



        '                        '060109VC: Adding Time Series Reference Data to Ecospace
        '                        'Will only save data for once per year, (at half year)
        '                        'save at iYear = int(timenow)
        '                        'ReDim SpaceBiomassByRegion(totalTime, m_data.nGroups, NoRegions)
        '                        'ReDim SpaceCatchByRegion(totalTime, m_data.nGroups, NoRegions)
        '                        If StoreTimeSeriesData Then
        '                            SpaceBiomassByRegion(iYear, iGrp, 0) = SpaceBiomassByRegion(iYear, iGrp, 0) + Biomass(iGrp)
        '                            SpaceBiomassByRegion(iYear, iGrp, Region(iRow, iCol)) = SpaceBiomassByRegion(iYear, iGrp, Region(iRow, iCol)) + Biomass(iGrp)
        '                            SpaceBiomassByRegionCount(iYear, iGrp, 0) = SpaceBiomassByRegionCount(iYear, iGrp, 0) + 1
        '                            SpaceBiomassByRegionCount(iYear, iGrp, Region(iRow, iCol)) = SpaceBiomassByRegionCount(iYear, iGrp, Region(iRow, iCol)) + 1
        '                    If Catch(iGrp) > 0 Then
        '                                SpaceCatchByRegion(iYear, iGrp, 0) = SpaceCatchByRegion(iYear, iGrp, 0) + Biomass(iGrp) * FishTime(iGrp)
        '                                SpaceCatchByRegion(iYear, iGrp, Region(iRow, iCol)) = SpaceCatchByRegion(iYear, iGrp, Region(iRow, iCol)) + Biomass(iGrp) * FishTime(iGrp)
        '                                SpaceCatchByRegionCount(iYear, iGrp, 0) = SpaceCatchByRegionCount(iYear, iGrp, 0) + 1
        '                                SpaceCatchByRegionCount(iYear, iGrp, Region(iRow, iCol)) = SpaceCatchByRegionCount(iYear, iGrp, Region(iRow, iCol)) + 1
        '                            End If
        '                            If iGrp = 1 Then
        '                                ForiFlt= 1 To NumGear
        '                                    SpaceEffortByRegionFleet(iYear, ig, 0) = SpaceEffortByRegionFleet(iYear, ig, 0) + EffortSpace(ig, iRow, iCol)
        '                                    SpaceEffortByRegionFleet(iYear, ig, Region(iRow, iCol)) = SpaceEffortByRegionFleet(iYear, ig, Region(iRow, iCol)) + EffortSpace(ig, iRow, iCol)
        '                                    SpaceEffortByRegionFleetCount(iYear, ig, 0) = SpaceEffortByRegionFleetCount(iYear, ig, 0) + 1
        '                                    SpaceEffortByRegionFleetCount(iYear, ig, Region(iRow, iCol)) = SpaceEffortByRegionFleetCount(iYear, ig, Region(iRow, iCol)) + 1
        '                                Next
        '                            End If
        '                        End If

        '                        'abmpa: use this routine for ecoseed abmpa
        '                        If En1 >= 0 And chkMPA.value = Checked Then
        '                            If Shadow(iGrp) > 0 Then Ecoseed.CalcBioValSeed(iRow, iCol, ebb(), iGrp, En1)
        '                    If Catch(iGrp) > 0 Then Ecoseed.CalcGearValSeed iRow, iCol, ebb(), iGrp, En1
        '                        End If

    End Sub



    ''' <summary>
    ''' Sumarize output data for at the end of a time step
    ''' </summary>
    ''' <param name="iTimeStep"></param>
    ''' <param name="iMonth"></param>
    ''' <remarks>Called at the end of a time step to populate data for a time step output/results.</remarks>
    Private Sub summarizeTimeStepData(ByVal iTimeStep As Integer, ByVal iMonth As Integer)

        Dim igrp As Integer
        Try

            'increment the number of timestep the model ran for
            m_Data.nSumTimeSteps += 1

            'if this is the first time step
            'then BBase() needs to be set to the base value calculated be Ecospace this may not be the same as the starting Ecopath biomass
            If iTimeStep = 1 Then
                For igrp = 1 To m_Data.NGroups
                    If Btime(igrp) = 0 Then Btime(igrp) = Single.Epsilon
                    m_Data.BBase(igrp) = Btime(igrp) / m_Data.nWaterCells
                Next igrp
            End If

            Dim irgn As Integer
            For ir As Integer = 1 To Me.m_Data.InRow
                For ic As Integer = 1 To Me.m_Data.InCol
                    irgn = Me.m_Data.Region(ir, ic)
                    For igrp = 1 To Me.m_Data.NGroups
                        Me.m_Data.ResultsRegionGroup(irgn, igrp, iTimeStep) += Me.m_Data.Bcell(ir, ic, igrp)
                    Next
                Next ic
            Next ir

            For igrp = 1 To m_Data.NGroups

                'biomass averaged across all the cells for this time step
                Btime(igrp) = Btime(igrp) / m_Data.nWaterCells

                'save for each time step
                'biomass
                m_Data.ResultsByGroup(eSpaceResultsGroups.Biomass, igrp, iTimeStep) = Btime(igrp)
                'relative biomass
                m_Data.ResultsByGroup(eSpaceResultsGroups.RelativeBiomass, igrp, iTimeStep) = Btime(igrp) / m_Data.BBase(igrp)

            Next igrp

            If m_tracerData.EcoSpaceConSimOn Then

                'average contamintant by region for each time step
                For irgn = 0 To m_Data.NoRegions

                    Dim nInRgn As Integer = m_Data.nCellsInRegion(irgn)
                    If nInRgn = 0 Then nInRgn = 1 'there can be regions with zero cells(no area) this avoids a /0 

                    For igrp = 0 To m_Data.NGroups
                        m_tracerData.TracerConcByRegion(irgn, igrp, iTimeStep) = m_tracerData.TracerConcByRegion(irgn, igrp, iTimeStep) / nInRgn
                        m_tracerData.TracerCBRegion(irgn, igrp, iTimeStep) = m_tracerData.TracerCBRegion(irgn, igrp, iTimeStep) / nInRgn
                    Next igrp

                Next irgn

            End If 'If m_tracerData.EcoSpaceConSimOn Then

        Catch ex As Exception
            Debug.Assert(False)
            Throw New Exception("summarizeTimeStepData() Error: " & ex.Message, ex)
        End Try

        '  Debug.Assert(iSumIndex <> -1)
        Exit Sub


        '   If imonth = 6 Then AccumulateDataInfo(1 + Int(TimeNow), Btime, False)



        'EwE5 code from FindSpatialEquilibrium
        '            If EcoSeedOn = False And MPAstep <= 1 Or MPAstep >= CLng(Inrow) * CLng(Incol) + 1 Then
        '                ShowTransect(Blast)
        '                If StopRun > 0 Then GoTo exitSub
        '                If itt > TotalTime / TimeStep Then itt = TotalTime / TimeStep
        '                'itt = 12 * TimeNow + 1
        '                'VC020130: Carl, you suggested that the below should be used to constrain itt. MaxTime is however
        '                'a constant 1200, while simplot is dyn. dimensioned. hence I think the above constraint is better
        '                'If itt > MaxTime Then itt = MaxTime
        '                For ip = 1 To m_data.nGroups : Btime(ip) = Btime(ip) / Water : SimPlot(ip, 0, itt) = Btime(ip) : Next
        '                If imonth = 6 Then AccumulateDataInfo(1 + Int(TimeNow), Btime, False)

        '                'plot time results
        '                If TimeNow > 0.0# Then
        '                    For ip = 1 To nvar
        '                        If GrpsToShow(ip) Then        'Only display non-hidden groups
        '                            If Tn >= 0 Then SumBiomass(Tn, ip) = SumBiomass(Tn, ip) + Btime(ip)
        '                            'If TimeNow >= SumStart(1) Then SumBiomass(ip) = SumBiomass(ip) + Btime(ip)
        '                            'SumCatch(ip) = SumCatch(ip) + Btime(ip) * ?(ip)
        '                            'End If
        '                            If Btime(ip) < 0.1 * BBase(ip) Then Btime(ip) = 0.1 * BBase(ip)
        '                            If Btime(ip) > 10.0# * BBase(ip) Then Btime(ip) = 10.0# * BBase(ip)
        '                            Btime(ip) = Log(Btime(ip) / BBase(ip))
        '                        BioPlot.Line (LastT, LastB(ip))-(TimeNow, Btime(ip)), PoolColor(ip)
        '                        BioPlot2.Line (LastT, LastB(ip))-(TimeNow, Btime(ip)), PoolColor(ip)
        '                            LastB(ip) = Btime(ip)
        '                        End If
        '                    Next
        '                Else   'First entry
        '                    For ip = 1 To nvar
        '                        BBase(ip) = Btime(ip)
        '                        LastB(ip) = 0
        '                        If Tn >= 0 Then
        '                            SumBiomass(Tn, ip) = SumBiomass(Tn, ip) + Btime(ip)
        '                            If NoRegions > 0 Then SumBiomassRegion(Tn, ip, Region(i, j)) = SumBiomassRegion(Tn, ip, Region(i, j)) + Btime(ip)
        '                        End If
        '                    Next
        '                End If
        '                LastT = TimeNow
        '                'Update the time counter
        '                timeshow.Caption = Format$(TimeNow, "0.00") + " of " + CStr(TotalTime) + " years"
        '                If chkEnlargePlot.value = Checked Then
        '                    BioPlot2.Visible = True
        '                    BioPlot2.ZOrder()
        '                    MapD.Visible = False
        '                Else
        '                    BioPlot2.Visible = False
        '                    MapD.Visible = True
        '                End If
        '                DoEvents()
        '            End If
        '            If EcoSeedOn = True Then
        '                LastT = TimeNow
        '                'Update the time counter
        '                timeshow.Caption = Format$(TimeNow, "0.00") + " of " + CStr(TotalTime) + " years"
        '                If chkEnlargePlot.value = Checked Then
        '                    BioPlot2.Visible = True
        '                    BioPlot2.ZOrder()
        '                    MapD.Visible = False
        '                Else
        '                    BioPlot2.Visible = False
        '                    MapD.Visible = True
        '                End If
        '                DoEvents()
        '            End If

    End Sub

    Private Sub onTimeStep(ByVal iTime As Integer)

        Try
            If Me.m_TimestepDelegate IsNot Nothing Then
                m_TimestepDelegate(iTime)
            End If
        Catch ex As Exception
            Debug.Assert(False, ex.Message)
            Throw New Exception("processTimeStep() Error: " & ex.Message)
        End Try

    End Sub

#End Region

#Region "Map Velocities"

    Sub SetXYBoundaryDepths()
        'calculates flow depths at cell faces:depthX is bottom face, depthY is right face of each cell
        Dim i As Integer, j As Integer

        ReDim m_Data.DepthX(m_Data.InRow, m_Data.InCol)
        ReDim m_Data.DepthY(m_Data.InRow, m_Data.InCol)

        For i = 0 To m_Data.InRow
            For j = 0 To m_Data.InCol
                If m_Data.Depth(i, j) > 0 Then
                    If m_Data.Depth(i + 1, j) > 0 Then
                        If m_Data.DepthA(i + 1, j) > m_Data.DepthA(i, j) Then
                            m_Data.DepthY(i, j) = m_Data.DepthA(i, j)
                        Else
                            m_Data.DepthY(i, j) = m_Data.DepthA(i + 1, j)
                        End If
                    End If 'If m_Data.Depth(i + 1, j) > 0 Then

                    If m_Data.Depth(i, j + 1) > 0 Then
                        If m_Data.DepthA(i, j + 1) > m_Data.DepthA(i, j) Then
                            m_Data.DepthX(i, j) = m_Data.DepthA(i, j)
                        Else
                            m_Data.DepthX(i, j) = m_Data.DepthA(i, j + 1)
                        End If
                    End If ' If m_Data.Depth(i, j + 1) > 0 Then

                End If ' If m_Data.Depth(i, j) > 0 Then
            Next j
        Next i

        ReDim m_Data.Xvel(m_Data.InRow + 1, m_Data.InCol + 1)
        ReDim m_Data.Yvel(m_Data.InRow + 1, m_Data.InCol + 1)
        For i = 0 To m_Data.InRow + 1
            For j = 0 To m_Data.InCol + 1
                If m_Data.Depth(i, j) > 0 Then
                    m_Data.Xvel(i, j) = m_Data.Xvloc(i, j)
                    m_Data.Yvel(i, j) = m_Data.Yvloc(i, j)
                End If
            Next j
        Next i

    End Sub


    Public Sub SM_MapApparentUpwell(ByVal Xvloc(,) As Single, ByVal Yvloc(,) As Single)
        'sets apparent upwelling/downwelling rates based only on flow forcing field
        'sketched by model user
        Dim Fl As Single, i As Integer, j As Integer, UpMax As Single, UpLoc As Single, Cl2 As Single
        ReDim m_Data.flow(m_Data.InRow + 1, m_Data.InCol + 1)
        Cl2 = 0.01 / m_Data.CellLength ' ^ 2

        For i = 0 To m_Data.InRow
            For j = 0 To m_Data.InCol
                If m_Data.Depth(i, j) > 0 Then
                    If m_Data.Depth(i + 1, j) > 0 Then
                        Fl = Yvloc(i, j) * m_Data.DepthY(i, j)
                        'Yvel(i, j) = Yvloc(i, j) '?????????????????????
                        m_Data.flow(i, j) = m_Data.flow(i, j) - Fl
                        m_Data.flow(i + 1, j) = m_Data.flow(i + 1, j) + Fl
                    End If
                    If m_Data.Depth(i, j + 1) > 0 Then
                        Fl = Xvloc(i, j) * m_Data.DepthX(i, j)
                        'Xvel(i, j) = Xvloc(i, j) '??????????????????????????
                        m_Data.flow(i, j) = m_Data.flow(i, j) - Fl
                        m_Data.flow(i, j + 1) = m_Data.flow(i, j + 1) + Fl
                    End If
                End If
            Next
        Next
        UpMax = 0
        For i = 1 To m_Data.InRow
            For j = 1 To m_Data.InCol
                If m_Data.Depth(i, j) > 0 Then
                    If Math.Abs(m_Data.flow(i, j)) > UpMax Then UpMax = Math.Abs(m_Data.flow(i, j))
                End If
            Next
        Next
        UpMax = UpMax * Cl2
        '  Up.Cls()
        For i = 1 To m_Data.InRow
            For j = 1 To m_Data.InCol
                If m_Data.Depth(i, j) > 0 Then
                    UpLoc = -m_Data.flow(i, j) * Cl2
                    m_Data.UpVel(i, j) = UpLoc  'Added for this model  SM.
                    'Up.Circle (j + 0.5, i + 0.5 - UpLoc / UpMax), 0.1
                    'Up.Line (j + 0.5, i + 0.5)-Step(0, -UpLoc / UpMax)
                End If
            Next
        Next
        'UpCap.Caption = "Upwelling velocities, max=" + Format$(UpMax / CellLength, "###.##") + "km/yr"
    End Sub


    Private Sub readAdvectFile()
        ''Read in Advection Field data.  SM, Jan 7, 2003
        ''Used for reading in Advection field data.
        'Dim i As Integer, j As Integer
        'Dim F$
        ''F$ = DispCommonDlg(7, frmMdiEcopath4.dlgFileAccess, F$)
        'If DispCommonDlg(7, frmMdiEcopath4.dlgFileAccess, F$) Then
        '    'Stop
        '    m_Data.CurrentForce = True
        '    'velmaker.ReadVelFields F$
        '    NewReadVelFields(F$)
        'End If
        Try
            Dim d As New System.Windows.Forms.OpenFileDialog
            Dim sr As System.IO.TextReader

            If d.ShowDialog() = System.Windows.Forms.DialogResult.OK Then
                sr = New System.IO.StreamReader(d.FileName)

                Dim i As Integer, j As Integer, InrowRead As Integer, IncolRead As Integer
                Dim Xvl As Single, Yvl As Single, Xvv As Single, Yvv As Single, Vxp As Single, Vyp As Single, Upv As Single, Dep As Single

                InrowRead = ReadNumber(sr)
                IncolRead = ReadNumber(sr)

                If InrowRead <> m_Data.InRow Or IncolRead <> m_Data.InCol Then
                    If MsgBox("Number of rows and columns in this advection file are not the same as your current map; try to read anyway?", vbYesNo) = vbNo Then Exit Sub
                End If

                Vxp = ReadNumber(sr)
                Vyp = ReadNumber(sr)

                For i = 0 To InrowRead + 1
                    For j = 0 To IncolRead + 1
                        Xvl = ReadNumber(sr)
                        Yvl = ReadNumber(sr)
                        Xvv = ReadNumber(sr)
                        Yvv = ReadNumber(sr)
                        Upv = ReadNumber(sr)
                        Dep = ReadNumber(sr)
                        If i <= m_Data.InRow + 1 And j <= m_Data.InCol + 1 Then
                            m_Data.Xvloc(i, j) = Xvl
                            m_Data.Yvloc(i, j) = Yvl
                            m_Data.Xvel(i, j) = Xvv
                            m_Data.Yvel(i, j) = Yvv
                            m_Data.UpVel(i, j) = Upv
                            m_Data.DepthA(i, j) = Dep
                        End If
                    Next
                Next

            End If
        Catch ex As Exception
            Debug.Assert(False, "Reading advection file failed - " + ex.Message)
        End Try

    End Sub

    Private Const cCHARS_NUMBER As String = "-0123456789E."
    Private Const cCHARS_STRING As String = "-0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ_$."
    Private cSeparator As Char = CChar(" ")

    Protected Function ReadNumber(ByRef sr As System.IO.TextReader) As Single
        Dim ch(255) As Char ' Should be enough to hold one single number
        Dim readCh(1) As Char
        Dim nChar As Integer = 0

        ' Read leading spaces
        Do
            sr.Read(readCh, 0, 1)
        Loop Until (cCHARS_NUMBER.IndexOfAny(readCh) > -1) Or (sr.Peek() < 0)

        If (sr.Peek() = -1) Then Throw New Exception("Unexpected end of file found while reading body")

        ' Read digits
        Do
            ch(nChar) = readCh(0)
            nChar += 1
            sr.Read(readCh, 0, 1)
        Loop Until (cCHARS_NUMBER.IndexOfAny(readCh) = -1) Or (sr.Peek() < 0)

        Return Single.Parse(ch)

    End Function


#End Region

#Region "Multi Threading stuff"
    ' this creates a solver object for each thread and initialises them with ecospace data
    Private Function InitGridSolverThreads() As Boolean
        Dim solver As cGridSolver

        Try
            If m_gridSolvers Is Nothing Then
                m_gridSolvers = New List(Of cGridSolver)
            Else
                m_gridSolvers.Clear()
            End If

            For i As Integer = 1 To m_Data.nGridSolverThreads
                solver = New cGridSolver(i)
                solver.Init(AMm, F, m_Data.Bcell, m_Data.InRow, m_Data.InCol, m_Data.Tol, jord, m_Data.W, Bcw, C, d, e, m_Data.Depth, m_Data.ByPassIntegrate, m_Data.iStartRow, m_Data.iEndRow, m_Data.TimeStep, m_Data.maxIter, m_Data.jStartCol, m_Data.jEndCol, m_Data.IsMigratory, threadGroups, m_Data.UseExact)
                m_gridSolvers.Add(solver)
            Next i

            Return True

        Catch ex As Exception

            cLog.Write(ex)
            Debug.Assert(False, ex.Message)
            Throw New ApplicationException(Me.ToString & ".InitGridSolverThreads() Error:  " & ex.Message, ex)

        End Try


    End Function

    ''' <summary>
    ''' Creates a spacesolver object for each thread, and initialises them with references to ecospace variables
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function InitSpaceSolverThreads() As Boolean
        Dim solver As cSpaceSolver

        Try
            If m_spaceSolvers Is Nothing Then
                m_spaceSolvers = New List(Of cSpaceSolver)
            Else
                m_spaceSolvers.Clear()
            End If

            For i As Integer = 1 To m_Data.nSpaceSolverThreads
                solver = New cSpaceSolver(i)

                'set reference variables
                solver.m_EcospaceModel = Me
                solver.m_Data = m_Data
                solver.m_SimData = m_SimData
                solver.m_PathData = m_EPdata
                solver.m_Stanza = m_Stanza
                solver.m_Ecosim = m_Ecosim

                'copy tracer data into each thread
                'this way each thread gets its own copy of the data that has been initialized by the database
                m_tracerData.CopyTo(solver.m_TracerData)

                solver.Search = m_search
                solver.Bcw = Bcw
                solver.C = C
                solver.d = d
                solver.e = e
                solver.BEQLast = BEQlast
                solver.Btime = Btime
                solver.F = F
                solver.AMm = AMm
                solver.Ecode = Ecode
                solver.HdenCell = HdenCell
                solver.RelFitness = RelFitness
                solver.FtimeCell = FtimeCell
                solver.Cper = Cper
                solver.PconSplit = PconSplit
                solver.RelRepStanza = RelRepStanza
                solver.Tstanza = Tstanza
                solver.PbSpace = PbSpace

                'needs to be set from ecospace, but not references
                ' solver.Tn = Tn
                solver.nvar2 = nvar2
                solver.itt = itt 'itimestep index to data stored by month
                solver.PPScale = PPScale
                solver.TimeStep2 = m_Data.TimeStep / 2
                solver.MinChange = MinChange
                solver.Init()

                solver.EcospaceErrorHandler = AddressOf Me.SolverErrorHandler

                m_spaceSolvers.Add(solver)
            Next i

            Return True

        Catch ex As Exception

            cLog.Write(ex)
            Debug.Assert(False, ex.Message)
            Throw New ApplicationException(Me.ToString & ".InitSpaceSolverThreads() Error:  " & ex.Message, ex)

        End Try


    End Function


    Private Sub InitSolversForYear(ByVal iYear As Integer)

        Try
            For Each solver As cSpaceSolver In Me.m_spaceSolvers

                solver.YearTimeStep(iYear)
                'discount factor was computed in the main time loop

            Next
        Catch ex As Exception
            cLog.Write(ex)
            Throw New ApplicationException(Me.ToString & ".InitForYear() Error:  " & ex.Message, ex)
        End Try
    End Sub
    ''' <summary>
    ''' Creates a spacesolver object for each thread, and initialises them with references to ecospace variables
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function InitIBMSolverThreads() As Boolean
        Dim solver As cIBMSolver

        Try
            If m_IBMSolvers Is Nothing Then
                m_IBMSolvers = New List(Of cIBMSolver)
            Else
                m_IBMSolvers.Clear()
            End If

            For i As Integer = 1 To m_Data.nGridSolverThreads
                solver = New cIBMSolver(i)

                'set reference variables
                solver.m_EcospaceModel = Me
                solver.m_Data = m_Data
                solver.m_ESData = m_SimData
                solver.m_Stanza = m_Stanza
                solver.m_Ecosim = m_Ecosim
                solver.Bcw = Bcw
                solver.C = C
                solver.d = d
                solver.e = e
                solver.Cper = Cper

                solver.Init()

                solver.EcospaceErrorHandler = AddressOf Me.SolverErrorHandler

                m_IBMSolvers.Add(solver)
            Next i

            Return True

        Catch ex As Exception

            cLog.Write(ex)
            Debug.Assert(False, ex.Message)
            Throw New ApplicationException(Me.ToString & ".InitIBMSolverThreads() Error:  " & ex.Message, ex)

        End Try


    End Function


    Private Sub SolverErrorHandler(ByVal ThreadID As Integer, ByVal msg As String)
        m_solverErrorMsg = msg
        m_solverErrorID = ThreadID
        Me.m_bsolverError = True
        System.Console.WriteLine("Error in Ecospace solver thread ID " & ThreadID.ToString & " " & msg)
    End Sub

    ''' <summary>
    ''' This iterates over the list of space solvers, and initializes them with variables calculated by Ecospace, at each time step
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function UpdateSpaceSolverThreads(ByVal Year As Integer) As Boolean

        Dim solver As cSpaceSolver
        Try

            For Each solver In m_spaceSolvers
                solver.nvar2 = nvar2
                solver.itt = itt 'itimestep index to data stored by month
                solver.PPScale = PPScale
                solver.TimeStep2 = m_Data.TimeStep / 2
                solver.MinChange = MinChange
                solver.Btime = Btime
                solver.iYear = Year
            Next

            Return True

        Catch ex As Exception

            cLog.Write(ex)
            Debug.Assert(False, ex.Message)
            Throw New ApplicationException(Me.ToString & ".UpdateGridSolverThreads() Error:  " & ex.Message, ex)

        End Try


    End Function

    Private Function Get2DArray(ByVal startIndex As Integer, ByVal endIndex As Integer, ByRef X As Single(,), ByVal nRows As Integer, ByVal nCols As Integer) As Single(,)
        Dim iStart As Integer
        Dim jStart As Integer
        Dim iEnd As Integer
        Dim jNew As Integer
        Dim iNew As Integer
        Dim iOld As Integer
        Dim jOld As Integer
        Dim newX(,) As Single

        iStart = (startIndex - 1) \ nCols + 1
        jStart = (startIndex - 1) Mod nCols + 1
        iEnd = (endIndex - 1) \ nCols + 1

        ReDim newX(iEnd - iStart + 1, nCols)

        For k As Integer = 1 To endIndex - startIndex + 1

            jNew = (k + jStart - 2) Mod nCols + 1
            iNew = (k + jStart - 2) \ nCols + 1
            iOld = (k + jStart - 2) \ nCols + iStart
            jOld = jNew

            newX(iNew, jNew) = X(iOld, jOld)

        Next

        Return newX

    End Function

    Private Function Get3DArray(ByVal startIndex As Integer, ByVal endIndex As Integer, ByRef X As Single(,,), ByVal nRows As Integer, ByVal nCols As Integer, ByVal nGroups As Integer) As Single(,,)
        Dim iStart As Integer
        Dim jStart As Integer
        Dim iEnd As Integer
        Dim jNew As Integer
        Dim iNew As Integer
        Dim iOld As Integer
        Dim jOld As Integer
        Dim newX(,,) As Single

        iStart = (startIndex - 1) \ nCols + 1
        jStart = (startIndex - 1) Mod nCols + 1
        iEnd = (endIndex - 1) \ nCols + 1

        ReDim newX(iEnd - iStart + 1, nCols, nGroups)

        For k As Integer = 1 To endIndex - startIndex + 1

            jNew = (k + jStart - 2) Mod nCols + 1
            iNew = (k + jStart - 2) \ nCols + 1
            iOld = (k + jStart - 2) \ nCols + iStart
            jOld = jNew

            For ip As Integer = 1 To nGroups
                newX(iNew, jNew, ip) = X(iOld, jOld, ip)
            Next
        Next

        Return newX

    End Function
    Private Function Get2DArray(ByVal startIndex As Integer, ByVal endIndex As Integer, ByRef X As Integer(,), ByVal nRows As Integer, ByVal nCols As Integer) As Integer(,)
        Dim iStart As Integer
        Dim jStart As Integer
        Dim iEnd As Integer
        Dim jNew As Integer
        Dim iNew As Integer
        Dim iOld As Integer
        Dim jOld As Integer
        Dim newX(,) As Integer

        iStart = (startIndex - 1) \ nCols + 1
        jStart = (startIndex - 1) Mod nCols + 1
        iEnd = (endIndex - 1) \ nCols + 1

        ReDim newX(iEnd - iStart + 1, nCols)

        For k As Integer = 1 To endIndex - startIndex + 1

            jNew = (k + jStart - 2) Mod nCols + 1
            iNew = (k + jStart - 2) \ nCols + 1
            iOld = (k + jStart - 2) \ nCols + iStart
            jOld = jNew

            newX(iNew, jNew) = X(iOld, jOld)

        Next

        Return newX

    End Function

    Private Function Get3DArray(ByVal startIndex As Integer, ByVal endIndex As Integer, ByRef X As Integer(,,), ByVal nRows As Integer, ByVal nCols As Integer, ByVal nGroups As Integer) As Integer(,,)
        Dim iStart As Integer
        Dim jStart As Integer
        Dim iEnd As Integer
        Dim jNew As Integer
        Dim iNew As Integer
        Dim iOld As Integer
        Dim jOld As Integer
        Dim newX(,,) As Integer

        iStart = (startIndex - 1) \ nCols + 1
        jStart = (startIndex - 1) Mod nCols + 1
        iEnd = (endIndex - 1) \ nCols + 1

        ReDim newX(iEnd - iStart + 1, nCols, nGroups)

        For k As Integer = 1 To endIndex - startIndex + 1

            jNew = (k + jStart - 2) Mod nCols + 1
            iNew = (k + jStart - 2) \ nCols + 1
            iOld = (k + jStart - 2) \ nCols + iStart
            jOld = jNew

            For ip As Integer = 1 To nGroups
                newX(iNew, jNew, ip) = X(iOld, jOld, ip)
            Next
        Next

        Return newX

    End Function
#End Region

    Public Sub New()

    End Sub

#Region "Summary stats"

    Public Function CalculateSpaceSS() As Single
        '        'accumulates statistical information for comparing model to data
        '        'for simulation years 0=first simulation year)
        '        'assumes first simulation year is first calendar year in data csv file
        'On Local Error GoTo exitSub
        Dim i As Long, j As Long, iDyear As Integer, Zstat As Single
        Dim Erpred() As Single
        Dim Ss As Single
        'Dim Cnt As Long
        Dim bCountStat As Boolean

        Dim SpNObs() As Integer
        Dim SpSumZ() As Single
        Dim SpSumZ2() As Single


        ReDim Erpred(m_refdata.NdatType * m_refdata.NdatYear)
        '  ReDim ErTrace(m_refdata.NdatType * m_refdata.NdatYear)
        'ReDim SpTraceObs(SpDat)
        'ReDim SpTraceZ(SpDat)
        'ReDim SpTraceZ2(SpDat)

        ReDim SpNObs(m_refdata.NdatType)
        ReDim SpSumZ(m_refdata.NdatType)
        ReDim SpSumZ2(m_refdata.NdatType)

        'ReDim SpeDatq(SpDat)
        'Dim m_refdata.Iobs As Long
        ''SS Calculations for Ecospace
        ''Timeseries values are stored for each year in variables below; 0 is used for the first year:
        ''ReDim SpaceBiomassByRegion(totalTime, NumGroups, NoRegions)
        ''ReDim SpaceCatchByRegion(totalTime, NumGroups, NoRegions)
        'ReDim ObsPred(2, 0)
        'ReDim ObsName(0)
        'ReDim ObsUse(0)
        'm_refdata.Iobs = 0
        ''SpTraceObs = 0
        'Accumulate z statistics for observations:
        For j = 1 To m_refdata.NdatType  'This time series is for region: m_refdata.spregion(j)
            For iDyear = 1 To m_Data.TotalTime '- 1
                If m_refdata.DatVal(iDyear, j) > 0 And _
                    (m_refdata.DatType(j) = eTimeSeriesType.BiomassAbs Or m_refdata.DatType(j) = eTimeSeriesType.BiomassRel Or _
                     m_refdata.DatType(j) = eTimeSeriesType.Catches Or m_refdata.DatType(j) = eTimeSeriesType.CatchesForcing Or _
                     m_refdata.DatType(j) = eTimeSeriesType.FishingEffort) Then

                    bCountStat = False

                    'SWt(m_refdata.Iobs) = SpWt(j)
                    Select Case m_refdata.DatType(j)
                        Case eTimeSeriesType.BiomassAbs, eTimeSeriesType.BiomassRel 'Abundance Data
                            If m_Data.SpaceBiomassByRegionCount(iDyear, m_refdata.DatPool(j), m_refdata.SPRegion(j)) > 0 Then
                                Zstat = Math.Log(m_refdata.DatVal(iDyear, j) / m_Data.SpaceBiomassByRegion(iDyear, m_refdata.DatPool(j), m_refdata.SPRegion(j)) * m_Data.SpaceBiomassByRegionCount(iDyear, m_refdata.DatPool(j), m_refdata.SPRegion(j)))          'BB(DatPool(j)))
                                bCountStat = True
                            End If

                        Case eTimeSeriesType.FishingEffort '3  'Effort data, In Ecospace only used for comparison, not to drive effort
                            If m_Data.SpaceEffortByRegionFleetCount(iDyear, m_refdata.DatPool(j), m_refdata.SPRegion(j)) > 0 Then
                                Zstat = Math.Log(m_refdata.DatVal(iDyear, j) / m_Data.SpaceEffortByRegionFleet(iDyear, m_refdata.DatPool(j), m_refdata.SPRegion(j)) * m_Data.SpaceEffortByRegionFleetCount(iDyear, m_refdata.DatPool(j), m_refdata.SPRegion(j)))          'BB(DatPool(j)))
                                bCountStat = True
                            End If

                        Case eTimeSeriesType.Catches, eTimeSeriesType.CatchesForcing '-6, 6     'Absolute Catch Data, Martell, Jan 02
                            If m_Data.SpaceCatchByRegion(iDyear, m_refdata.DatPool(j), m_refdata.SPRegion(j)) > 0 Then
                                Zstat = Math.Log(m_refdata.DatVal(iDyear, j) / m_Data.SpaceCatchByRegion(iDyear, m_refdata.DatPool(j), m_refdata.SPRegion(j)) * m_Data.SpaceCatchByRegionCount(iDyear, m_refdata.DatPool(j), m_refdata.SPRegion(j)))
                                bCountStat = True
                            End If

                        Case Else
                    End Select

                    If bCountStat Then
                        m_refdata.Iobs = m_refdata.Iobs + 1

                        Erpred(m_refdata.Iobs) = Zstat
                        SpNObs(j) = SpNObs(j) + 1
                        SpSumZ(j) = SpSumZ(j) + Zstat
                        SpSumZ2(j) = SpSumZ2(j) + Zstat * Zstat
                    End If

                    'ElseIf ConSimOn And m_refdata.datVal(iDyear, j) > 0 And (m_refdata.datType(j) = 8 Or m_refdata.datType(j) = 9) Then
                    '    'This is to add reference time series for Ecotracer runs
                    '    If ConcTr(m_refdata.datpool(j)) > 0 Then
                    '        'NobsTime(iDyear) = NobsTime(iDyear) + 1
                    '        'NobsTime is for testing significance, don't need a similar one for tracer, for now at least
                    '        'Wt(m_refdata.Iobs) = WtType(j)
                    '        If SpaceTraceByRegionCount(iDyear, m_refdata.datpool(j), m_refdata.spregion(j)) > 0 Then
                    '            Zstat = Log(m_refdata.datVal(iDyear, j) / SpaceTraceByRegion(iDyear, m_refdata.datpool(j), m_refdata.spregion(j)) * SpaceTraceByRegionCount(iDyear, m_refdata.datpool(j), m_refdata.spregion(j)))          'BB(DatPool(j)))
                    '            Cnt = Cnt + 1
                    '            ReDim Preserve ObsPred(2, Cnt)
                    '            ReDim Preserve ObsName(Cnt)
                    '            ReDim Preserve ObsUse(Cnt)
                    '            ObsPred(0, Cnt) = m_refdata.datVal(iDyear, j)
                    '            ObsName(Cnt) = SpName(j)   'name of the series
                    '            ObsUse(Cnt) = True
                    '            ObsPred(1, Cnt) = SpaceTraceByRegion(iDyear, m_refdata.datpool(j), m_refdata.spregion(j)) / SpaceTraceByRegionCount(iDyear, m_refdata.datpool(j), m_refdata.spregion(j))
                    '            ObsPred(2, Cnt) = m_refdata.spregion(j)
                    '            'ObsPred(1, Cnt) = ObsPred(0, Cnt) * (1 + Rnd())
                    '            'save the biggest values for scaling
                    '            If ObsPred(0, Cnt) > ObsPred(0, 0) Then ObsPred(0, 0) = ObsPred(0, Cnt)
                    '            If ObsPred(1, Cnt) > ObsPred(1, 0) Then ObsPred(1, 0) = ObsPred(1, Cnt)
                    '        End If
                    '        'YTraceHat(m_refdata.Iobs) = Log(ConcTr(DatPool(j)))
                    '        ErTrace(TraceObs) = Zstat
                    '        SpTraceObs(j) = SpTraceObs(j) + 1
                    '        SpTraceZ(j) = SpTraceZ(j) + Zstat
                    '        SpTraceZ2(j) = SpTraceZ2(j) + Zstat * Zstat
                    '    End If
                End If
            Next
        Next j

        '        '-----------------------------------
        'ReDim m_refdata.DatSS(SpDat) As Single
        'ReDim m_refdata.DatQ(SpDat) As Single

        For j = 1 To m_refdata.NdatType
            If SpNObs(j) > 0 Then
                If m_refdata.DatType(j) = eTimeSeriesType.BiomassAbs Then
                    m_refdata.DatSS(j) = SpSumZ2(j)
                    m_refdata.DatQ(j) = 0

                ElseIf m_refdata.DatType(j) = eTimeSeriesType.BiomassRel _
                    Or m_refdata.DatType(j) = eTimeSeriesType.FishingEffort Or _
                       m_refdata.DatType(j) = eTimeSeriesType.TotalMortality Or _
                       m_refdata.DatType(j) = eTimeSeriesType.Catches Or _
                       m_refdata.DatType(j) = eTimeSeriesType.CatchesForcing Or _
                       m_refdata.DatType(j) = eTimeSeriesType.AverageWeight Then 'added mean body wieght here

                    m_refdata.DatSS(j) = SpSumZ2(j) - SpSumZ(j) ^ 2 / SpNObs(j)
                    m_refdata.DatQ(j) = SpSumZ(j) / SpNObs(j)
                    ' SpeDatq(j) = Exp(m_refdata.DatQ(j))
                End If
            End If
        Next

        m_refdata.Iobs = 0
        Ss = 0

        For i = 1 To m_refdata.NdatYear
            iDyear = m_refdata.DatYear(i) - m_refdata.DatYear(1)
            For j = 1 To m_refdata.NdatType
                If m_refdata.DatVal(i, j) > 0 And iDyear < m_Data.TotalTime + 1 And _
                    (m_refdata.DatType(j) = eTimeSeriesType.BiomassRel Or _
                     m_refdata.DatType(j) = eTimeSeriesType.BiomassAbs Or _
                     m_refdata.DatType(j) = eTimeSeriesType.FishingEffort Or _
                     m_refdata.DatType(j) = eTimeSeriesType.TotalMortality Or _
                     m_refdata.DatType(j) = eTimeSeriesType.Catches Or _
                     m_refdata.DatType(j) = eTimeSeriesType.CatchesForcing Or _
                     m_refdata.DatType(j) = eTimeSeriesType.AverageWeight) Then

                    m_refdata.Iobs = m_refdata.Iobs + 1
                    Erpred(m_refdata.Iobs) = Erpred(m_refdata.Iobs) - m_refdata.DatQ(j)
                    Ss = Ss + Erpred(m_refdata.Iobs) ^ 2 ' * Wt(m_refdata.Iobs)
                End If

            Next
        Next


        '        '--------------------------------------------------
        '        'Trace SS Spatial:
        '        TraceSS = 0
        '        If ConSimOn Then
        '    ReDim tDatSS(SpDat) As Single, tDatq(SpDat) As Single, teDatq(SpDat) As Single
        '            'Dim i As Integer, j As Integer, iYear As Integer, bplot As Single

        '            For j = 1 To SpDat
        '                If SpTraceObs(j) > 0 Then
        '                    If m_refdata.datType(j) = 9 Then      'absolute concentration
        '                        tDatSS(j) = SpTraceZ2(j)
        '                        tDatq(j) = 0
        '                    ElseIf m_refdata.datType(j) = 8 Then  'relative concentration
        '                        tDatSS(j) = SpTraceZ2(j) - SpTraceZ(j) ^ 2 / SpTraceObs(j)
        '                        tDatq(j) = SpTraceZ(j) / SpTraceObs(j)
        '                        teDatq(j) = Exp(tDatq(j))
        '                    End If
        '                End If
        '            Next

        '            TraceObs = 0
        '            'Ss = 0

        '            For i = 1 To SpDatYear
        '                iDyear = SpYear(i) - SpYear(1)
        '                For j = 1 To SpDat
        '                    If (m_refdata.datType(j) = 8 Or m_refdata.datType(j) = 9) And Abs(m_refdata.datVal(i, j)) > 0 And iDyear < TotalTime + 1 Then
        '                        TraceObs = TraceObs + 1
        '                        ErTrace(TraceObs) = ErTrace(TraceObs) - tDatq(j)
        '                        'DatDev(j, i) = ErTrace(TraceObs)
        '                        TraceSS = TraceSS + ErTrace(TraceObs) ^ 2 '* Wt(m_refdata.Iobs) 'No weight on trace
        '                        'YTraceHat(TraceObs) = YTraceHat(TraceObs) + Datq(j)
        '                    End If

        '                Next
        '            Next
        '        End If
        '        '===================================================

        '        'As a start using SS not LL
        '        Dim LogL As Single
        '        For j = 1 To SpDat
        '            If m_refdata.DatSS(j) > 0 Then LogL = LogL + SpWt(j) * (SpNObs(j) - 1) * Log(m_refdata.DatSS(j))
        '        Next
        '        LogL = LogL / 2
        '        If SetToLike = True Then Ss = LogL

        Return Ss

        'exitSub:
        '        CalculateSpaceSS = Ss
    End Function


    Public Sub RedimSpaceCSVvariables()
        'Ecosim name    Ecospace name

        'NdatType       SpDat
        'DatName        SpName()
        'DatPool        SpPool()
        'DatType        SpType()
        'WtType         SpWt()
        'DatVal         SpVal()
        'DatYear        SpYear()
        'PoolForceBB    SpForceBB()
        'PoolForceCatch SpForceCatch()
        'PoolForceZ     SpForceZ()
        'IsDatShown     IsSpShown()
        '               SpRegion()
        '(the next ones are dimensioned elsewhere)
        'DatSumZ()      SpSumZ()
        'DatSumZ2()     SpSumZ2()
        'DatNobs()      SpNobs()
        'DatSS()        SpSS()
        'DatTraceObs()  SpTraceObc()
        'DatTraceZ()    SpTraceZ()
        'DatTraceZ2()   SpTraceZ2()
        'Datq()         SpDatq()
        'eDatq()        SpeDatq()

        ReDim m_Data.SpName(m_Data.SpDat)
        ReDim m_Data.SpPool(m_Data.SpDat)
        ReDim m_Data.SpType(m_Data.SpDat)
        ReDim m_Data.SpWt(m_Data.SpDat)
        ReDim m_Data.SpVal(m_Data.SpDatYear + 1, m_Data.SpDat)
        ReDim m_Data.SpYear(m_Data.SpDatYear)
        ReDim m_Data.SpForceBB(m_Data.NGroups, m_Data.SpDatYear)
        ReDim m_Data.SpForceCatch(m_Data.NGroups, m_Data.SpDatYear)
        ReDim m_Data.SpForceZ(m_Data.NGroups, m_Data.SpDatYear)
        ReDim m_Data.IsSpShown(m_Data.SpDat)
        ReDim m_Data.SpRegion(m_Data.SpDat)
    End Sub



#End Region

#Region " New Multistanza Stuff"
    Sub SpaceSplitUpdate()
        'updates numbers, weight, and biomass for multiple stanza species using information
        'on average performance (eatenby, loss) over ecospace grid cells used by the species
        Dim isp As Integer, ist As Integer, ieco As Integer, ia As Integer
        Dim Su As Single, Gf As Single, Nt As Single
        Dim Agemax As Integer, AgeMin As Integer, Be As Single

        For isp = 1 To m_Stanza.Nsplit
            'update numbers and body weights
            ieco = m_Stanza.EcopathCode(isp, m_Stanza.Nstanza(isp))
            If m_Ecosim.ResetPred(ieco) = False Then

                Be = 0
                For ist = 1 To m_Stanza.Nstanza(isp)
                    ieco = m_Stanza.EcopathCode(isp, ist)
                    Su = Math.Exp(-TotLoss(ieco) / 12.0# / TotBiom(ieco))
                    Gf = TotEatenBy(ieco) / TotPred(ieco)  '(month factor here included in splitalpha scaling setup)
                    For ia = m_Stanza.Age1(isp, ist) To m_Stanza.Age2(isp, ist)
                        m_Stanza.NageS(isp, ia) = m_Stanza.NageS(isp, ia) * Su
                        m_Stanza.WageS(isp, ia) = m_Stanza.vBM(isp) * m_Stanza.WageS(isp, ia) + Gf * m_Stanza.SplitAlpha(isp, ia)
                        If m_Stanza.FixedFecundity(isp) Then
                            Be = Be + m_Stanza.NageS(isp, ia) * m_Stanza.EggsSplit(isp, ia)
                        Else
                            If m_Stanza.WageS(isp, ia) > m_Stanza.WmatWinf(isp) Then Be = Be + m_Stanza.NageS(isp, ia) * (m_Stanza.WageS(isp, ia) - m_Stanza.WmatWinf(isp))
                        End If
                    Next
                Next
                m_Stanza.WageS(isp, m_Stanza.Age2(isp, m_Stanza.Nstanza(isp))) = (Su * m_Ecosim.AhatStanza(isp) + (1 - Su) * m_Stanza.WageS(isp, m_Stanza.Age2(isp, m_Stanza.Nstanza(isp)) - 1)) / (1 - m_Ecosim.RhatStanza(isp) * Su)
                m_Stanza.EggsStanza(isp) = Be
                'WageS(iSp, 0) = 0
                'update ages looping backward over age
                For ist = m_Stanza.Nstanza(isp) To 1 Step -1
                    Agemax = m_Stanza.Age2(isp, ist)
                    If ist > 1 Then AgeMin = m_Stanza.Age1(isp, ist) Else AgeMin = 1
                    If ist = m_Stanza.Nstanza(isp) Then
                        Nt = m_Stanza.NageS(isp, Agemax) + m_Stanza.NageS(isp, Agemax - 1)
                        If Nt = 0 Then Nt = 1.0E-30 'watch for zero numbers of older animals
                        'WageS(isp, Agemax) = (WageS(isp, Agemax) * NageS(isp, Agemax) + WageS(isp, Agemax - 1) * NageS(isp, Agemax - 1)) / Nt
                        m_Stanza.NageS(isp, Agemax) = Nt
                        Agemax = Agemax - 1
                    End If
                    For ia = Agemax To AgeMin Step -1
                        m_Stanza.NageS(isp, ia) = m_Stanza.NageS(isp, ia - 1)
                        m_Stanza.WageS(isp, ia) = m_Stanza.WageS(isp, ia - 1)
                    Next
                    ieco = m_Stanza.EcopathCode(isp, ist)
                    If ist < m_Stanza.Nstanza(isp) Then m_Ecosim.Brec(ieco) = m_Stanza.NageS(isp, m_Stanza.Age2(isp, ist) + 1) * m_Stanza.WageS(isp, m_Stanza.Age2(isp, ist) + 1)
                Next
                'finally set abundance at youngest age to recruitment rate
                ieco = m_Stanza.EcopathCode(isp, m_Stanza.Nstanza(isp)) 'code for adult biomass for sp isp
                'VILLY: note following assumes we extend pair list for egg prod and recpower to add multistanza options  at end of pair lists
                m_Ecosim.Srec(ieco) = m_EPdata.B(ieco)
                If m_Stanza.BaseEggsStanza(isp) > 0 Then
                    m_Stanza.NageS(isp, m_Stanza.Age1(isp, 1)) = m_Stanza.RscaleSplit(isp) * m_SimData.tval(m_Stanza.EggProdShapeSplit(isp)) * m_Stanza.RzeroS(isp) * m_SimData.tval(m_Stanza.HatchCode(isp))
                End If
                If m_Stanza.HatchCode(isp) = 0 Then m_Stanza.NageS(isp, m_Stanza.Age1(isp, 1)) = m_Stanza.NageS(isp, m_Stanza.Age1(isp, 1)) * (m_Stanza.EggsStanza(isp) / m_Stanza.BaseEggsStanza(isp)) ^ m_Stanza.RecPowerSplit(isp)
                m_Stanza.WageS(isp, m_Stanza.Age1(isp, 1)) = 0
            End If
        Next
        ' finally update bioamss and pred index information for all species
        m_Ecosim.SplitSetPred(Blocal)
        'this changes Blocal

    End Sub

    Sub InitPackets()
        'initialize numbers, weights, and positions ipacket,jpacket for IBM representation
        'note must be called in findspatialequilibrium after calls to initialize ecospace
        'variables

        Dim ia As Integer, isp As Integer, ist As Integer, iaa As Integer
        Dim ip As Integer, i As Integer, j As Integer, Nused As Integer, i1 As Integer
        Dim iList() As Integer, Jlist() As Integer, ieco As Integer, isc As Integer
        ReDim iList(ThabArea), Jlist(ThabArea), m_Stanza.iNursery(m_Stanza.Nsplit, ThabArea), m_Stanza.jNursery(m_Stanza.Nsplit, ThabArea)
        ReDim m_Stanza.IBMMovesPerMonth(m_Data.NGroups)
        ReDim m_Stanza.IBMdistmove(m_Stanza.Nsplit, m_Stanza.MaxAgeSplit)
        ReDim m_Data.PredCell(m_Data.InRow, m_Data.InCol, m_Data.NGroups)
        ReDim m_Stanza.Nnursery(m_Stanza.Nsplit), m_Stanza.StanzaNo(m_Stanza.Nsplit, m_Stanza.MaxAgeSplit)
        ReDim m_Stanza.MaxAgeSpecies(m_Stanza.Nsplit), m_Stanza.AgeIndex1(m_Stanza.Nsplit)
        'ReDim Cper(m_Data.Inrow, m_Data.InCol, m_Data.NGroups)

        'set number of packets per age **** to interface?****
        m_Stanza.Npackets = m_Data.InRow * m_Data.InCol * m_Stanza.NPacketsMultiplier

        ReDim m_Stanza.Npacket(m_Stanza.Nsplit, m_Stanza.MaxAgeSplit, m_Stanza.Npackets)
        ReDim m_Stanza.Wpacket(m_Stanza.Nsplit, m_Stanza.MaxAgeSplit, m_Stanza.Npackets)
        ReDim m_Stanza.iPacket(m_Stanza.Nsplit, m_Stanza.MaxAgeSplit, m_Stanza.Npackets)
        ReDim m_Stanza.jPacket(m_Stanza.Nsplit, m_Stanza.MaxAgeSplit, m_Stanza.Npackets)

        Dim cellsPerMonth As Single, Dmove As Single

        'set up pointer array to stanza number for each fish age
        'and set up initial numbers and weights by packet and age
        'and set initial packet positions on map grid
        'note assumes calls to initialize multistanza Nages,Wages,pred(ieco) have been made already
        isc = 0
        For isp = 1 To m_Stanza.Nsplit
            ia = -1
            m_Stanza.AgeIndex1(isp) = 0
            For ist = 1 To m_Stanza.Nstanza(isp)
                ieco = m_Stanza.EcopathCode(isp, ist)
                cellsPerMonth = m_Data.Mvel(ieco) / (12 * m_Data.CellLength)
                If cellsPerMonth >= 1 Then
                    m_Stanza.IBMMovesPerMonth(ieco) = cellsPerMonth + 1
                    Dmove = 1
                Else
                    m_Stanza.IBMMovesPerMonth(ieco) = 1
                    Dmove = cellsPerMonth / 1
                End If
                'm_Stanza.IBMMovesPerMonth(ieco) = Int(2 * m_Data.Mvel(ieco) / (12 * m_Data.CellLength) + 1) 'allows movement of roughly 1/2 cellwidth per move

                isc = isc + 1
                'make up temporary list of suitable cells for this stanza
                Nused = 0
                For i = 1 To m_Data.InRow : For j = 1 To m_Data.InCol
                        m_Data.Bcell(i, j, ieco) = 0 ' NOTE call to initpackets must be after any other Bcell initialization for multistanza biomasses
                        m_Data.PredCell(i, j, ieco) = 0
                        If (m_Data.PrefHab(ieco, m_Data.HabType(i, j)) = True Or m_Data.PrefHab(ieco, 0) = True) And m_Data.Depth(i, j) > 0 Then
                            Nused = Nused + 1
                            iList(Nused) = i : Jlist(Nused) = j
                            If ist = 1 Then m_Stanza.iNursery(isp, Nused) = i : m_Stanza.jNursery(isp, Nused) = j
                        End If
                    Next : Next
                If ist = 1 Then m_Stanza.Nnursery(isp) = Nused
                'then loop over ages to initialize numbers by age in stanza and distribute spatially
                For iaa = m_Stanza.Age1(isp, ist) To m_Stanza.Age2(isp, ist)
                    ia = ia + 1
                    m_Stanza.StanzaNo(isp, ia) = ist  'this table stores stanza number for fish of age ia, species isp
                    'following loop distributes total numbers at age over packets, sets initial weights
                    'note must be called after thabarea (number of active cells) has been calculated
                    For ip = 1 To m_Stanza.Npackets
                        m_Stanza.Npacket(isp, ia, ip) = m_Stanza.NageS(isp, ia) / m_Stanza.Npackets * ThabArea
                        m_Stanza.Wpacket(isp, ia, ip) = m_Stanza.WageS(isp, ia) + 0.0000000001
                    Next
                    For ip = 1 To m_Stanza.Npackets
                        'distribute packets uniformly over suitable cells for this stanza, using list set above
                        i1 = 1 + Rnd() * (Nused - 1)
                        m_Stanza.iPacket(isp, ia, ip) = iList(i1) + 0.5
                        m_Stanza.jPacket(isp, ia, ip) = Jlist(i1) + 0.5
                        'DEBUG: reenable following two lines
                        m_Data.Bcell(iList(i1), Jlist(i1), ieco) = m_Data.Bcell(iList(i1), Jlist(i1), ieco) + m_Stanza.Npacket(isp, ia, ip) * m_Stanza.Wpacket(isp, ia, ip)
                        m_Data.PredCell(iList(i1), Jlist(i1), ieco) = m_Data.PredCell(iList(i1), Jlist(i1), ieco) + m_Stanza.Npacket(isp, ia, ip) * m_Stanza.WWa(isp, ia)
                    Next

                    'calculate distance per move for this group
                    m_Stanza.IBMdistmove(isp, ia) = Dmove 'm_Data.Mvel(ieco) / (12 * m_Data.CellLength) / m_Stanza.IBMMovesPerMonth(ieco) 'movement distance (cell widths) per movement step
                    'note dependence here is only on ieco so far; could be more closely related to age ia
                Next
            Next
            m_Stanza.MaxAgeSpecies(isp) = ia
        Next
        ReDim m_Stanza.Zcell(m_Data.InRow, m_Data.InCol, m_Data.NGroups)  'this variable used to store spatial field of total mortality rates for survival updates
        ' For i = 1 To 6: Debug.Print Bcell(1, 1, i), StartBiomass(i): Next
        'For i = 1 To 6: Debug.Print PredCell(1, 1, i), pred(i): Next
        'Stop
    End Sub
#End Region


    Private Sub SetMigGrad()
        'set habitat quality gradient maps for all habitat types, for use in biased movement assessments
        Dim i As Integer, j As Integer, ii As Integer, jj As Integer, ihab As Integer
        'Dim Thab As Single, Nobs As Single, Habadd As Single
        Dim i1 As Integer, i2 As Integer, j1 As Integer, j2 As Integer, Sweep As Integer, imonth As Integer
        Dim nsweep As Integer
        Dim smallestDist As Single
        'Dim smallestI As Integer
        'Dim smallestJ As Integer
        Dim pathFound As Integer

        Dim nMig As Integer
        Dim migIndex() As Integer
        ReDim migIndex(m_Data.NGroups)
        Dim diagAdjust As Single
        'Dim diagAdjustFinal As Single

        Try
            For i = 1 To m_Data.NGroups
                If m_Data.IsMigratory(i) Then
                    nMig = nMig + 1
                    migIndex(nMig) = i
                End If
            Next
            ReDim MigGrad(m_Data.InRow + 1, m_Data.InCol + 1, nMig, 12)

            If m_Data.InRow > m_Data.InCol Then nsweep = m_Data.InRow Else nsweep = m_Data.InCol
            nsweep = nsweep * 2
            iWindow = 1
            For ihab = 1 To nMig
                For imonth = 1 To 12
                    For Sweep = 1 To nsweep
                        'If NcellsHab(ihab) > 0 Then
                        For i = 0 To m_Data.InRow + 1 : For j = 0 To m_Data.InCol + 1
                                If Sweep = 1 Then
                                    MigGrad(i, j, ihab, imonth) = 1000
                                ElseIf MigGrad(i, j, ihab, imonth) <> 0 Then
                                    smallestDist = 2000
                                    diagAdjust = 0
                                    'smallesti = -1
                                    'smallestJ = -1
                                    pathFound = False
                                    i1 = i - iWindow : If i1 < 0 Then i1 = 0
                                    i2 = i + iWindow : If i2 > m_Data.InRow + 1 Then i2 = m_Data.InRow + 1
                                    j1 = j - iWindow : If j1 < 0 Then j1 = 0
                                    j2 = j + iWindow : If j2 > m_Data.InCol + 1 Then j2 = m_Data.InCol + 1
                                    For ii = i1 To i2 : For jj = j1 To j2
                                            If ii = i Or jj = j Then
                                                diagAdjust = 0
                                            Else
                                                diagAdjust = 0.4142 'sqrt(2)-1
                                            End If
                                            If MigGrad(ii, jj, ihab, imonth) + diagAdjust < smallestDist And ((m_Data.Depth(i, j) <> 0 And m_Data.PrefHab(migIndex(ihab), m_Data.HabType(i, j)) Or i = 0 Or i = m_Data.InRow + 1 Or j = 0 Or j = m_Data.InCol + 1)) Then
                                                smallestDist = MigGrad(ii, jj, ihab, imonth) + diagAdjust
                                                pathFound = True
                                            End If
                                        Next : Next
                                    If pathFound Then MigGrad(i, j, ihab, imonth) = smallestDist + 1
                                End If
                                If m_Data.Depth(i, j) = 0 Or Not m_Data.PrefHab(migIndex(ihab), m_Data.HabType(i, j)) Then MigGrad(i, j, ihab, imonth) = 2000
                            Next : Next
                        If Sweep = 1 Then
                            MigGrad(m_Data.PrefRow(migIndex(ihab), imonth), m_Data.Prefcol(migIndex(ihab), imonth), ihab, imonth) = 0
                        End If
                        'If m_Data.PrefHab(ihab, 0) Then Exit For
                        'End If
                    Next
                Next
            Next

            'Dim tempstr As String
            'For ihab = 1 To 1 'nMig 'm_Data.NGroups
            '    For imonth = 1 To 12
            '        Debug.Print("")
            '        'Debug.Print("imonth = " + imonth.ToString)
            '        For i = 0 To m_Data.Inrow + 1
            '            For j = 0 To m_Data.InCol + 1
            '                If Math.Round(MigGrad(i, j, ihab, imonth)) < 100 Or MigGrad(i, j, ihab, imonth) = 2000 Then
            '                    tempstr = tempstr + " "
            '                    If Math.Round(MigGrad(i, j, ihab, imonth)) < 10 Or MigGrad(i, j, ihab, imonth) = 2000 Then
            '                        tempstr = tempstr + " "
            '                    End If
            '                End If
            '                If MigGrad(i, j, ihab, imonth) >= 0 And MigGrad(i, j, ihab, imonth) < 2000 Then
            '                    tempstr = tempstr + Math.Round(MigGrad(i, j, ihab, imonth)).ToString + " "
            '                    'ElseIf MigGrad(i, j, ihab, imonth) = 0 Then
            '                    '    tempstr = tempstr + "X "
            '                Else
            '                    tempstr = tempstr + "  "
            '                End If
            '            Next
            '            'System.Console.WriteLine(tempstr)
            '            tempstr = ""
            '        Next
            '    Next
            'Next
        Catch ex As Exception
            Debug.Assert(False, ex.Message)
        End Try
    End Sub

    Sub VaryMigMovementParameters(ByVal imonth As Integer)
        'sets solvegrid movement arrays based on depth map
        Dim i As Integer, j As Integer, ip As Integer, AdScale As Single ', iad As Integer, iju As Integer
        Dim isp As Integer, ist As Integer, nvar2 As Integer, ir As Integer, ieco As Integer
        '   Erase Bcw, C, d, e

        'ReDim Bcw(m_Data.Inrow + 1, m_Data.InCol + 1, m_Data.nvartot)
        'ReDim C(m_Data.Inrow + 1, m_Data.InCol + 1, m_Data.nvartot)
        'ReDim d(m_Data.Inrow + 1, m_Data.InCol + 1, m_Data.nvartot)
        'ReDim e(m_Data.Inrow + 1, m_Data.InCol + 1, m_Data.nvartot)
        Dim imig As Integer
        Dim nMig As Integer
        Dim migIndex() As Integer
        'Dim distortNS As Single
        'Dim distortEW As Single
        Dim distort As Single
        Try

            ReDim migIndex(m_Data.NGroups)
            For i = 1 To m_Data.NGroups
                If m_Data.IsMigratory(i) Then
                    nMig = nMig + 1
                    migIndex(nMig) = i
                End If
            Next

            AdScale = 1 / m_Data.CellLength '/ (2 * 3.14159 * CellLength)
            For i = 0 To m_Data.InRow
                For j = 0 To m_Data.InCol
                    'check depth on right face of this cell
                    If m_Data.Depth(i, j) > 0 Then
                        If m_Data.Depth(i, j + 1) > 0 Then
                            For imig = 1 To nMig
                                ip = migIndex(imig)
                                If MigPowj(ip, j) > 0 Then
                                    distort = 1 * MigPowj(ip, j) / (PrefColP(ip, imonth) + MigPowj(ip, j))
                                Else
                                    distort = 0.5
                                End If
                                If j > 0 And j < m_Data.InCol Then
                                    e(i, j + 1, ip) = Enomig(i, j + 1, ip) * RelMove(ip, i, j + 1) * RelMigMove(i, j + 1, i, j, MigGrad, m_Data.MoveScale, imig, imonth, ip) * distort
                                    d(i, j, ip) = dNomig(i, j, ip) * RelMove(ip, i, j) * RelMigMove(i, j, i, j + 1, MigGrad, m_Data.MoveScale, imig, imonth, ip) * (1 - distort)
                                    If m_Data.IsAdvected(ip) Then
                                        If m_Data.Xvel(i, j) > 0 Then
                                            d(i, j, ip) = d(i, j, ip) + m_Data.Xvel(i, j) * AdScale 'from j to the right
                                        Else
                                            e(i, j + 1, ip) = e(i, j + 1, ip) - m_Data.Xvel(i, j) * AdScale 'into j from right
                                        End If

                                    End If
                                Else
                                    If m_Data.IsAdvected(ip) Then
                                        If m_Data.Xvel(i, j) > 0 Then
                                            e(i, j + 1, ip) = m_Data.Mrate(ip) 'into j from right
                                            d(i, j, ip) = m_Data.Mrate(ip) + m_Data.Xvel(i, j) * AdScale 'from j to the right
                                        Else
                                            e(i, j + 1, ip) = m_Data.Mrate(ip) - m_Data.Xvel(i, j) * AdScale 'into j from right
                                            d(i, j, ip) = m_Data.Mrate(ip) 'from j to the right

                                        End If
                                    Else
                                        e(i, j + 1, ip) = 0
                                        d(i, j, ip) = 0
                                    End If
                                End If
                                'Enomig(i, j + 1, ip) = e(i, j + 1, ip)
                                'dNomig(i, j, ip) = d(i, j, ip)
                            Next

                            nvar2 = m_Data.NGroups
                            ir = 0
                            For isp = 1 To m_Stanza.Nsplit
                                For ist = 1 To m_Stanza.Nstanza(isp)
                                    ieco = m_Stanza.EcopathCode(isp, ist)
                                    ir = ir + 1
                                    e(i, j + 1, nvar2 + ir) = e(i, j + 1, ieco)
                                    d(i, j, nvar2 + ir) = d(i, j, ieco)
                                    'Enomig(i, j + 1, nvar2 + ir) = e(i, j + 1, ieco)
                                    'dNomig(i, j, nvar2 + ir) = d(i, j, ieco)
                                Next
                            Next
                        End If
                        'then check depths on bottom face of this cell
                        If m_Data.Depth(i + 1, j) > 0 Then
                            For imig = 1 To nMig
                                If i > 0 And i < m_Data.InRow Then
                                    ip = migIndex(imig)
                                    C(i, j, ip) = CNomig(i, j, ip) * RelMove(ip, i + 1, j) * RelMigMove(i + 1, j, i, j, MigGrad, m_Data.MoveScale, imig, imonth, ip) * distort
                                    Bcw(i + 1, j, ip) = BcwNomig(i + 1, j, ip) * RelMove(ip, i, j) * RelMigMove(i, j, i + 1, j, MigGrad, m_Data.MoveScale, imig, imonth, ip) * (1 - distort)
                                    If m_Data.IsAdvected(ip) Then
                                        If m_Data.Yvel(i, j) > 0 Then
                                            Bcw(i + 1, j, ip) = Bcw(i + 1, j, ip) + m_Data.Yvel(i, j) * AdScale 'from j to the right
                                        Else
                                            C(i, j, ip) = C(i, j, ip) - m_Data.Yvel(i, j) * AdScale   'into j from right
                                        End If

                                    End If
                                Else
                                    If m_Data.IsAdvected(ip) Then
                                        If m_Data.Yvel(i, j) > 0 Then
                                            C(i, j, ip) = m_Data.Mrate(ip) 'from row i+1 to i
                                            Bcw(i + 1, j, ip) = m_Data.Mrate(ip) + m_Data.Yvel(i, j) * AdScale ' + AdvectSouth 'from i to i+1
                                        Else
                                            C(i, j, ip) = m_Data.Mrate(ip) - m_Data.Yvel(i, j) * AdScale 'from row i+1 to i
                                            Bcw(i + 1, j, ip) = m_Data.Mrate(ip)
                                        End If
                                    Else
                                        C(i, j, ip) = 0
                                        Bcw(i + 1, j, ip) = 0
                                    End If
                                End If
                                'CNomig(i, j, ip) = C(i, j, ip)
                                'BcwNomig(i + 1, j, ip) = Bcw(i + 1, j, ip)
                            Next
                            'If npairs > 0 Then
                            '    For ip = 1 To npairs : iad = iadult(ip) : iju = ijuv(ip)
                            '        Bcw(i + 1, j, nvar + ip) = Bcw(i + 1, j, iad)
                            '        C(i, j, nvar + ip) = C(i, j, iad)
                            '        Bcw(i + 1, j, nvar + npairs + ip) = Bcw(i + 1, j, iju)
                            '        C(i, j, nvar + npairs + ip) = C(i, j, iju)
                            '        BcwNomig(i + 1, j, nvar + ip) = Bcw(i + 1, j, iad)
                            '        CNomig(i, j, nvar + ip) = C(i, j, iad)
                            '        BcwNomig(i + 1, j, nvar + npairs + ip) = Bcw(i + 1, j, iju)
                            '        CNomig(i, j, nvar + npairs + ip) = C(i, j, iju)
                            '    Next
                            'End If

                            'EwE5
                            ' nvar2 = nvar + 2 * npairs
                            nvar2 = m_Data.NGroups
                            ir = 0
                            For isp = 1 To m_Stanza.Nsplit
                                For ist = 1 To m_Stanza.Nstanza(isp)
                                    ieco = m_Stanza.EcopathCode(isp, ist)
                                    ir = ir + 1
                                    Bcw(i + 1, j, nvar2 + ir) = Bcw(i + 1, j, ieco)
                                    C(i, j, nvar2 + ir) = C(i, j, ieco)
                                    BcwNomig(i + 1, j, nvar2 + ir) = Bcw(i + 1, j, ieco)
                                    CNomig(i, j, nvar2 + ir) = C(i, j, ieco)
                                Next
                            Next
                        End If
                    End If

                Next j
            Next i

            If m_tracerData.EcoSpaceConSimOn Then
                'set movement rates for physical contaminant concentration to
                'rates for first detritus pool
                For i = 0 To m_Data.InRow + 1
                    For j = 0 To m_Data.InCol + 1
                        Bcw(i, j, 0) = Bcw(i, j, m_EPdata.NumLiving + 1)
                        C(i, j, 0) = C(i, j, m_EPdata.NumLiving + 1)
                        d(i, j, 0) = d(i, j, m_EPdata.NumLiving + 1)
                        e(i, j, 0) = e(i, j, m_EPdata.NumLiving + 1)
                        BcwNomig(i, j, 0) = Bcw(i, j, m_EPdata.NumLiving + 1)
                        CNomig(i, j, 0) = C(i, j, m_EPdata.NumLiving + 1)
                        dNomig(i, j, 0) = d(i, j, m_EPdata.NumLiving + 1)
                        Enomig(i, j, 0) = e(i, j, m_EPdata.NumLiving + 1)
                    Next
                Next
            End If
        Catch ex As Exception
            Debug.Assert(False, ex.Message)
        End Try
    End Sub

    Sub VaryMovementParameters2(ByVal imonth As Integer) ', ByVal ip As Integer, ByVal ieco As Integer)
        'EwE5 definition IsIad and IsIju indexes remove these are iAdult and iJuvenial indexes for the split pool code
        'Sub VaryMovementParameters(ByVal imonth As Integer, ByVal ip As Integer, ByVal IsIad As Integer, ByVal IsIju As Integer, ByVal ieco As Integer)

        Dim ip As Integer
        'sets solvegrid movement arrays based on depth map
        Dim i As Integer, j As Integer, AdScale As Single
        Dim nvar2 As Integer, ir As Integer, Distort As Single
        Dim Ep As Single
        Dim MaxCh As Single
        Dim FitRatio As Single
        AdScale = 1 '/ (2 * 3.14159 * CellLength)
        MaxCh = 1
        Dim ieco As Integer
        Dim imig As Integer
        Dim nMig As Integer
        Dim migIndex() As Integer
        Dim migGradWeight As Single = 0.05

        ReDim migIndex(m_Data.NGroups)
        For i = 1 To m_Data.NGroups
            If m_Data.IsMigratory(i) Then
                nMig = nMig + 1
                migIndex(nMig) = i
            End If
        Next

        For imig = 1 To nMig
            ip = migIndex(imig)
            ieco = IecoCode(ip)
            'calculate relative emigration rate from each cell as function
            'of fitness, scaling parameter KmoveFit(ip) set in setKmove routine
            For i = 0 To m_Data.InRow + 1
                For j = 0 To m_Data.InCol + 1
                    If m_Data.FitRespType > 0 Then
                        Ep = -Kmovefit(ip) * RelFitness(i, j, ip)
                        If Ep < -MaxCh Then Ep = -MaxCh
                        If Ep > MaxCh Then Ep = MaxCh
                        Ep = Math.Exp(Ep)
                        RelMoveFit(i, j) = 2.0# * Ep / (1 + Ep)
                        '        If ip = 18 And imonth > 1 And i > 30 And i <  m_data.inrow + 1 And j > 1 And j < 5 Then Stop
                    Else
                        RelMoveFit(i, j) = 1
                    End If
                Next
            Next

            For i = 0 To m_Data.InRow
                For j = 0 To m_Data.InCol
                    If m_Data.Depth(i, j) > 0 Then

                        'check depth on right face of this cell
                        If m_Data.Depth(i, j + 1) > 0 Then

                            If MigPowj(ip, j) > 0 Then
                                Distort = 2 * MigPowj(ip, j) / (PrefColP(ip, imonth) + MigPowj(ip, j))
                            Else
                                Distort = 1
                            End If
                            'Distort = 1 + Distort / 4

                            If m_Data.FitRespType < 2 Then
                                e(i, j + 1, ip) = Enomig(i, j + 1, ip) * RelMoveFit(i, j + 1) * RelMigMove(i, j + 1, i, j, MigGrad, m_Data.MoveScale, imig, imonth, ip) * (Distort) '* (MigGrad(i, j, imig, imonth) + 1) / m_Data.InCol
                                d(i, j, ip) = dNomig(i, j, ip) * RelMoveFit(i, j) * RelMigMove(i, j, i, j + 1, MigGrad, m_Data.MoveScale, imig, imonth, ip) * (2 - Distort) '* (MigGrad(i, j, imig, imonth) + 1) / m_Data.InCol
                                'e(i, j + 1, ip) = Enomig(i, j + 1, ip) * ((1 - migGradWeight) * RelMoveFit(i, j + 1) * (Distort) + migGradWeight * RelMigMove(i, j + 1, i, j, MigGrad, m_Data.MoveScale, imig, imonth))
                                'd(i, j, ip) = dNomig(i, j, ip) * ((1 - migGradWeight) * RelMoveFit(i, j) * (2 - Distort) + migGradWeight * RelMigMove(i, j, i + 1, j, MigGrad, m_Data.MoveScale, imig, imonth))
                            Else
                                FitRatio = RelMoveFit(i, j + 1) / RelMoveFit(i, j)
                                e(i, j + 1, ip) = Enomig(i, j + 1, ip) * FitRatio * (Distort) * RelMigMove(i, j + 1, i, j, MigGrad, m_Data.MoveScale, imig, imonth, ip)
                                d(i, j, ip) = dNomig(i, j, ip) / FitRatio * (2 - Distort) * RelMigMove(i, j, i, j + 1, MigGrad, m_Data.MoveScale, imig, imonth, ip)
                            End If

                            If j = 0 Or j = m_Data.InCol Then
                                e(i, j + 1, ip) = 0
                                d(i, j, ip) = 0
                            End If

                            'jb split pool code removed
                            'If IsIad > 0 Then
                            '    e(i, j + 1, nvar + IsIad) = e(i, j + 1, ip)
                            '    d(i, j, nvar + IsIad) = d(i, j, ip)
                            'End If
                            'If IsIju > 0 Then
                            '    e(i, j + 1, nvar + npairs + IsIju) = e(i, j + 1, ip)
                            '    d(i, j, nvar + npairs + IsIju) = d(i, j, ip)
                            'End If

                            nvar2 = m_Data.NGroups
                            If ieco > 0 Then
                                ir = IecoCode(ip)
                                e(i, j + 1, nvar2 + ir) = e(i, j + 1, ip)
                                d(i, j, nvar2 + ir) = d(i, j, ip)
                                'Enomig(i, j + 1, nvar2 + ir) = E(i, j + 1, ieco)
                                'dNomig(i, j, nvar2 + ir) = d(i, j, ieco)
                            End If
                        End If ' If m_Data.Depth(i, j + 1) > 0 Then check depth on right face of this cell

                        'then check depths on bottom face of this cell
                        If m_Data.Depth(i + 1, j) > 0 Then
                            If MigPowi(ip, i) > 0 Then
                                Distort = 2 * MigPowi(ip, i) / (PrefRowP(ip, imonth) + MigPowi(ip, i))
                            Else
                                Distort = 1
                            End If
                            'Distort = 1 + Distort / 2

                            If m_Data.FitRespType < 2 Then
                                'If Distort <> 1 Then Stop
                                C(i, j, ip) = CNomig(i, j, ip) * RelMoveFit(i + 1, j) * RelMigMove(i + 1, j, i, j, MigGrad, m_Data.MoveScale, imig, imonth, ip) * Distort '* (MigGrad(i, j, imig, imonth) + 1) / m_Data.Inrow
                                Bcw(i + 1, j, ip) = BcwNomig(i + 1, j, ip) * RelMoveFit(i, j) * RelMigMove(i, j, i + 1, j, MigGrad, m_Data.MoveScale, imig, imonth, ip) * (2 - Distort) '* (MigGrad(i, j, imig, imonth) + 1) / m_Data.Inrow
                                'C(i, j, ip) = CNomig(i, j, ip) * ((1 - migGradWeight) * Distort * RelMoveFit(i + 1, j) + migGradWeight * RelMigMove(i + 1, j, i, j, MigGrad, m_Data.MoveScale, imig, imonth))
                                'Bcw(i + 1, j, ip) = BcwNomig(i + 1, j, ip) * ((1 - migGradWeight) * RelMoveFit(i, j) * (2 - Distort) + migGradWeight * RelMigMove(i, j, i + 1, j, MigGrad, m_Data.MoveScale, imig, imonth))
                            Else
                                FitRatio = RelMoveFit(i + 1, j) / RelMoveFit(i, j)
                                C(i, j, ip) = CNomig(i, j, ip) * Distort * FitRatio * RelMigMove(i + 1, j, i, j, MigGrad, m_Data.MoveScale, imig, imonth, ip)
                                Bcw(i + 1, j, ip) = BcwNomig(i + 1, j, ip) / FitRatio * (2 - Distort) * RelMigMove(i, j, i + 1, j, MigGrad, m_Data.MoveScale, imig, imonth, ip)
                            End If

                            If i = 0 Or i = m_Data.InRow Then
                                C(i, j, ip) = 0
                                Bcw(i + 1, j, ip) = 0
                            End If

                            ''jb split pool code removed
                            'If IsIad > 0 Then
                            '    Bcw(i + 1, j, nvar + IsIad) = Bcw(i + 1, j, ip)
                            '    C(i, j, nvar + IsIad) = C(i, j, ip)
                            'End If
                            'If IsIju > 0 Then
                            '    Bcw(i + 1, j, nvar + npairs + IsIju) = Bcw(i + 1, j, ip)
                            '    C(i, j, nvar + npairs + IsIju) = C(i, j, ip)
                            'End If

                            If ieco > 0 Then
                                ir = ieco
                                Bcw(i + 1, j, nvar2 + ir) = Bcw(i + 1, j, ip)
                                C(i, j, nvar2 + ir) = C(i, j, ip)
                            End If
                        End If 'If m_Data.Depth(i + 1, j) > 0 Then then check depths on bottom face of this cell

                    End If 'If m_Data.Depth(i, j) > 0 Then

                Next j
            Next i
        Next imig
    End Sub


    Function RelMigMove(ByVal i1 As Integer, ByVal j1 As Integer, ByVal i2 As Integer, ByVal j2 As Integer, ByVal G(,,,) As Single, ByVal gk As Single, ByVal ihab As Integer, ByVal imonth As Integer, ByVal ip As Integer) As Single
        'sets relative movement rate using slope of g() function between origin (i1,j1) and destination (i2,j2) cells
        'function is 1 when slope ss is zero
        RelMigMove = 1
        If m_Data.barrierAvoidanceWeight(ip) > 0 Then
            Dim Ss As Single
            Dim multDir As Single
            Dim numDir As Single
            Try
                multDir = 1
                If i1 > 0 And j1 > 0 And i1 <= m_Data.InRow And j1 <= m_Data.InCol Then
                    If (G(i1 + 1, j1, ihab, imonth) - G(i1, j1, ihab, imonth)) < 0 Then numDir += 1.0
                    If (G(i1 - 1, j1, ihab, imonth) - G(i1, j1, ihab, imonth)) < 0 Then numDir += 1.0
                    If (G(i1, j1 - 1, ihab, imonth) - G(i1, j1, ihab, imonth)) < 0 Then numDir += 1.0
                    If (G(i1, j1 + 1, ihab, imonth) - G(i1, j1, ihab, imonth)) < 0 Then numDir += 1.0
                    multDir = 1 / numDir
                    'If multDir = 1 Then multDir = 2
                End If
                Ss = G(i2, j2, ihab, imonth) - G(i1, j1, ihab, imonth)
                Select Case Ss
                    'Case 0
                    'RelMigMove = 1
                    Case Is < 0
                        RelMigMove = 1 + m_Data.barrierAvoidanceWeight(ip) * multDir * G(i1, j1, ihab, imonth) / (0.5 * m_Data.InRow + G(i1, j1, ihab, imonth)) '2 / (2 - Math.Exp(-G(i1, j1, ihab, imonth)))
                    Case Is > 0
                        RelMigMove = 1 - m_Data.barrierAvoidanceWeight(ip) * multDir * G(i1, j1, ihab, imonth) / (0.5 * m_Data.InRow + G(i1, j1, ihab, imonth))
                    Case Else
                        Stop
                End Select

                If G(i1, j1, ihab, imonth) = 0 Then
                    RelMigMove = 1
                End If
                'RelMigMove = 1
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
            End Try
        End If
    End Function

    Sub AdjustSpaceParsNew()
        'set ecospace basebiomass using proportions of usable habitat for each pool, and adjust
        'vulnerability, search parameters to mean biomasses in habitats used
        Dim i As Integer, j As Integer, ii As Integer, ia As Integer
        'Dim K As Integer
        'Dim Temp As Single, Bpred As Single, Bprey As Single
        Dim BRatio() As Single
        Dim Qarena() As Single, VulBiom() As Single

        'just set v and a to base values, do not change basebiomass unless adjustspace=true
        'get habitat areas
        ReDim BRatio(m_Data.NGroups)


        CalcHabitatArea()
        'calculate habitat area used by each biomass type
        ReDim HabAreaUsed(m_Data.NGroups)
        'VC Hobart Sep 2008; Adding distribution envelopes by functional group makes it necessary 
        'to change how Habareaused is calculated. 
        ThabArea = 0
        For iRo As Integer = 1 To m_Data.InRow
            For iCo As Integer = 1 To m_Data.InCol
                If m_Data.Depth(iRo, iCo) > 0 Then
                    ThabArea = ThabArea + 1
                    'm_Data.HabArea(m_Data.HabType(i, j)) = m_Data.HabArea(m_Data.HabType(i, j)) + 1
                    For iGr As Integer = 1 To m_Data.NGroups
                        If (m_Data.PrefHab(iGr, m_Data.HabType(iRo, iCo)) Or m_Data.PrefHab(iGr, 0)) And _
                        m_Data.DistributionEnvelope(iRo, iCo, iGr) Then HabAreaUsed(iGr) += 1
                    Next
                End If
            Next
        Next

        For i = 1 To m_Data.NGroups
            'VC Hobart Sep 2008: next replaced by calculation above with Distribution Envelope
            'For j = 1 To m_Data.NoHabitats
            '    If m_Data.PrefHab(i, j) Or m_Data.PrefHab(i, 0) Then HabAreaUsed(i) = HabAreaUsed(i) + m_Data.HabArea(j)
            'Next
            If HabAreaUsed(i) > 0 Then
                Basebiomass(i) = ThabArea * m_SimData.StartBiomass(i) / HabAreaUsed(i)
                BRatio(i) = ThabArea / HabAreaUsed(i)
                If m_Data.AdjustSpace = False Then BRatio(i) = 1
            Else
                Basebiomass(i) = m_SimData.StartBiomass(i) 'don't really need this; set before routine called
                BRatio(i) = 1
            End If
        Next
        'adjust vulnerability and search parameters for these basebiomass values in preferred habitats

        ReDim m_Data.Vspace(m_SimData.Narena), m_Data.Aspace(m_SimData.inlinks)

        'find total consumptions of prey type for each arena, added over predators
        ReDim Qarena(m_SimData.Narena), VulBiom(m_SimData.Narena)
        For ii = 1 To m_SimData.inlinks
            i = m_SimData.ilink(ii)
            j = m_SimData.jlink(ii)
            ia = m_SimData.ArenaLink(ii)
            Qarena(ia) = Qarena(ia) + m_SimData.Qlink(ii) * BRatio(j)
        Next
        'then set initial vulnerable biomasses (V) by arena
        For ii = 1 To m_SimData.Narena
            i = m_SimData.Iarena(ii)
            j = m_SimData.Jarena(ii)
            If m_SimData.VulMult(i, j) > 10000000000.0# Then m_SimData.VulMult(i, j) = 10000000000.0#
            m_Data.Vspace(ii) = (m_SimData.VulMult(i, j) + 0.0000000001) * Qarena(ii) / (m_SimData.StartBiomass(i) * BRatio(i))
            If m_Data.Vspace(ii) = 0 Then m_Data.Vspace(ii) = 1
            If m_SimData.BoutFeeding Then
                VulBiom(ii) = -Qarena(ii) / Math.Log(1 - 1 / (m_SimData.VulMult(i, j) + 0.0000000001))
            Else
                VulBiom(ii) = (m_SimData.VulMult(i, j) + 0.0000000001 - 1.0#) * Qarena(ii) / (2 * m_Data.Vspace(ii))
            End If
            If VulBiom(ii) = 0 Then VulBiom(ii) = 1

            'note above calculation will give wrong result if vulmult(i,j)=1, i.e. vulmult must be strictly
            'greater than 1.0
            'set nonzero value for vularena to avoid divides by zero if no feeding in it
        Next
        'then set predator search rates (a) by trophic link
        Dim Dzero As Single
        For ii = 1 To m_SimData.inlinks
            ia = m_SimData.ArenaLink(ii)
            j = m_SimData.jlink(ii)
            If VulBiom(ia) > 0 Then
                Dzero = m_SimData.CmCo(j) / (m_SimData.CmCo(j) - 1)
                m_Data.Aspace(ii) = Dzero * m_SimData.Qlink(ii) / (VulBiom(ia) * m_SimData.pred(j))
            Else
                m_Data.Aspace(ii) = 0
            End If
        Next

        'adjust pbbiomass for primary producers
        For i = 1 To m_Data.NGroups
            If m_SimData.pbm(i) > 0 Then 'primary producer
                PbSpace(i) = m_SimData.pbbiomass(i) * m_SimData.StartBiomass(i) / Basebiomass(i)
            End If
        Next

    End Sub

    Public Sub ClearPorts(ByVal iFleet As Integer)

        Dim iStart As Integer = iFleet
        Dim iEnd As Integer = iFleet
        Dim inRow As Integer = m_Data.InRow
        Dim inCol As Integer = m_Data.InCol
        Dim i As Integer
        Dim j As Integer

        If iStart <= 0 Then iStart = 0 : iEnd = Me.EcoSpaceData.nFleets

        For i = 1 To inRow
            For j = 1 To inCol
                For iFleet = iStart To iEnd
                    m_Data.Port(iFleet, i, j) = False
                Next iFleet
            Next
        Next

    End Sub

    Public Sub SetAllCoastsToPorts(ByVal iFleet As Integer)

        Dim i As Integer
        Dim j As Integer
        Dim k As Integer
        Dim l As Integer
        Dim inRow As Integer = m_Data.InRow
        Dim inCol As Integer = m_Data.InCol
        Dim iStart As Integer = iFleet
        Dim iEnd As Integer = iFleet

        If iStart <= 0 Then iStart = 0 : iEnd = Me.EcoSpaceData.nFleets

        For i = 1 To inRow
            For j = 1 To inCol
                'Check if there is a neighboring cell which is in water
                If Me.EcoSpaceData.Depth(i, j) <= 0 Then    'it is a land cell
                    For k = i - 1 To i + 1 Step 2
                        For l = j - 1 To j + 1 Step 2
                            If k > 0 And k <= inRow And l > 0 And l <= inCol And Me.EcoSpaceData.Depth(k, l) > 0 Then
                                For iFleet = iStart To iEnd
                                    m_Data.Port(iFleet, i, j) = True
                                Next iFleet
                            End If
                        Next
                    Next
                End If
            Next
        Next

    End Sub

    Public Sub CalculateCostOfSailing()

        If (Me.m_pluginManager IsNot Nothing) Then
            If Me.m_pluginManager.EcospaceCalculateCostOfSailing(Me.m_Data, Me.m_Data.Depth, Me.m_Data.Port, Me.m_Data.Sail) Then
                ' Done, overruled
                Return
            End If
        End If

        Dim i As Integer
        Dim ix As Integer
        Dim iy As Integer
        Dim j As Integer
        Dim iPort As Integer
        Dim iFleet As Integer
        Dim Ports As Integer
        Dim minD(,,) As Single
        Dim Dist As Single
        Dim Lati As Single
        Dim Longi As Single
        Dim LatPort As Single
        Dim LonPort As Single
        Dim PortX() As Integer
        Dim PortY() As Integer
        Dim Disti As Single

        ' This calculation does NOT take the shape of land into account

        If m_Data.IDH_SS <= 0 Then m_Data.IDH_SS = 2

        Ports = 0
        For i = 1 To m_Data.InRow
            For j = 1 To m_Data.InCol
                Me.m_Data.Port(0, i, j) = False
                For iFleet = 1 To Me.m_Data.nFleets
                    If Me.m_Data.Port(iFleet, i, j) = True Then
                        Ports += 1
                        Me.m_Data.Port(0, i, j) = True
                        Exit For
                    End If
                Next
            Next
        Next
        ReDim PortX(Ports)
        ReDim PortY(Ports)
        Ports = 0
        For i = 1 To m_Data.InRow
            For j = 1 To m_Data.InCol
                If Me.m_Data.Port(0, i, j) = True Then
                    Ports += 1
                    PortX(Ports) = i
                    PortY(Ports) = j
                End If
            Next
        Next

        ReDim minD(Me.m_Data.nFleets, Me.m_Data.InRow, Me.m_Data.InCol)
        For iFleet = 0 To Me.m_Data.nFleets
            For i = 1 To m_Data.InRow
                For j = 1 To m_Data.InCol
                    minD(iFleet, i, j) = Single.MaxValue
                Next j
            Next i
        Next iFleet

        For iPort = 1 To Ports      'go port by port
            ix = PortX(iPort)
            iy = PortY(iPort)
            LonPort = CSng(m_Data.Lon1 + (ix / m_Data.IDH_SS))
            LatPort = CSng(m_Data.Lat1 - (iy / m_Data.IDH_SS))
            For iFleet = 0 To Me.m_Data.nFleets
                ' Is this fleet based in a this port?
                If Me.m_Data.Port(iFleet, ix, iy) Then
                    'LonPort = CSng(m_Data.Lon1 + (ix / m_Data.IDH_SS) / 2.0!)
                    'LatPort = CSng(m_Data.Lat1 - (iy / m_Data.IDH_SS) / 2.0!)
                    'Sail(AF, ix, iy) = 0
                    For i = 1 To m_Data.InRow
                        For j = 1 To m_Data.InCol
                            If Me.EcoSpaceData.Depth(i, j) > 0 Then 'water cell
                                Longi = CSng(m_Data.Lon1 + (i / m_Data.IDH_SS))
                                Lati = CSng(m_Data.Lat1 - (j / m_Data.IDH_SS))
                                'Longi = CSng(m_Data.Lon1 + (i / m_Data.IDH_SS) / 2.0!)
                                'Lati = CSng(m_Data.Lat1 - (j / m_Data.IDH_SS) / 2.0!)
                                Dist = CalDistance(LonPort, LatPort, Longi, Lati, eDistanceType.NauticalMiles)
                                minD(iFleet, i, j) = Math.Min(Dist, minD(iFleet, i, j))
                            Else
                                minD(iFleet, i, j) = 0
                            End If
                        Next j
                    Next i
                    'test the neighboring cells
                    'Calc8Dist i, j
                    'FindMinDistFor8Neighbors i, j
                End If
            Next iFleet
        Next iPort

        For iFleet = 0 To Me.m_Data.nFleets
            For i = 1 To m_Data.InRow
                For j = 1 To m_Data.InCol
                    If minD(iFleet, i, j) < Single.MaxValue Then Disti = minD(iFleet, i, j) Else Disti = 0.0!
                    Me.m_Data.Sail(iFleet, i, j) = Disti
                Next j
            Next i
        Next iFleet

    End Sub

    Private Enum eDistanceType As Integer
        NauticalMiles
        Kilometers
        Degrees
    End Enum

    Private Function CalDistance(ByVal Lon1 As Single, ByVal Lat1 As Single, ByVal Lon2 As Single, ByVal Lat2 As Single, _
                                 ByVal DistType As eDistanceType) As Single  ', Dist As Single, XDist As Single, YDist As Single) As Single
        'On Local Error GoTo errCalDistance
        'Villy C received this sub is from Reg Watson 04 May 2001, modified to function and dropped last terms, also made types explicit
        'Calculates the distance between two map points Lon1,Lat1 and Lon2,Lat2
        'Points are measured decimal degrees
        'Returns Dist, Long Dist(X), Lat Dist(Y) in either NatMiles or km (DistType)
        'DistType 0=NatMiles, 1=km, 2=degrees
        'Provided by Laura Wing lwing@clausent.demon.co.uk to Ken White
        'Uses a spherical triangle to the pole
        '3 variations:
        '   Same Hemisphere
        '   Different Hemisphere
        '   Spans Greenwich meridian or anti-meridian

        'Expects - (Neg) for South Latitudes
        'Note: always goes the shortest way... not over pole or wrong way around the world
        'Dist does not have a sign but XDist and YDist do

        Dim CoLatA As Double
        Dim CoLatB As Double
        Dim DifLong As Double
        Dim PartA As Double
        Dim PartB As Double
        Dim XXD As Double
        Dim AngDisDeg As Double
        Dim DistNM As Double
        Dim Ydist As Double
        Dim Dist As Double
        Dim TwoPie As Double = Math.PI * 2.0#
        Dim DR As Double = TwoPie / 360.0# 'for converting degrees to radians for functions

        CoLatA = 90 + Sign(Lat1) * Abs(Lat1)
        CoLatB = 90 + Sign(Lat2) * Abs(Lat2)

        DifLong = Abs(Lon1 - Lon2)

        If DifLong > 180 Then
            DifLong = 360 - DifLong
        End If

        Ydist = Lat1 - Lat2

        PartA = Cos(CoLatA * DR) * Cos(CoLatB * DR)
        PartB = Sin(CoLatA * DR) * Sin(CoLatB * DR) * Cos(DifLong * DR)
        XXD = PartA + PartB

        If XXD = 1.0# Then XXD = 1.000001
        'There is no arccos so it is atn(-X/sqr(-X*X+1))+1.5708
        AngDisDeg = (Atan(-XXD / Sqrt(-XXD * XXD + 1.0#)) + 1.5708) / TwoPie * 360.0#
        DistNM = AngDisDeg * 60.0#

        Select Case DistType
            Case eDistanceType.NauticalMiles
                Dist = DistNM
                Ydist = Ydist * 60.0#
            Case eDistanceType.Kilometers
                Dist = DistNM * 1.85325
                Ydist = Ydist * 60.0# * 1.85325
            Case eDistanceType.Degrees
                Dist = AngDisDeg
        End Select
        Return CSng(Dist)

        'This code can not be reached
        'Dist is returned before this can execute
        '        Xdist = Sqrt(Dist ^ 2 - Ydist ^ 2) * Sign(Lon1 - Lon2)
        '        Exit Function

        'errCalDistance:
        '        Xdist = -1
        '        CalDistance = 0 '-1 vc changed this from -1 to 0
        '        Exit Function

    End Function

End Class

