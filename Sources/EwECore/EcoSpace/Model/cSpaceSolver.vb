' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Option Explicit On

Imports System.Threading
Imports Microsoft.Extensions.Logging
Imports Debug = System.Diagnostics.Debug

Public Class cSpaceSolver

    ''' <summary>
    ''' Wait handle 
    ''' </summary>
    ''' <remarks>
    ''' When the Solve() thread is running (SignalState in a non-signaled state SignalState.Reset()) 
    ''' calls to SignalState.WaitOne() will block until Solve() has completed (SignalState in a signaled state SignalState.Set())
    ''' </remarks>
    Public WaitHandle As ManualResetEvent
    Public Shared ThreadIncrementer As Integer

    Public RunTimeSeconds As Double
    Public CatchCPUTimeSec As Double
    Public lstCellCompTimes As New List(Of Double)

    Private m_ConTracer As cContaminantTracer

    Public iYear As Integer ' current year

    Public ThreadID As Integer

    'references
    Public m_EcospaceModel As cEcoSpace
    Public m_Data As cEcospaceDataStructures
    Public m_SimData As cEcosimDatastructures
    Public m_PathData As cEcopathDataStructures
    Public m_Stanza As cStanzaDatastructures
    Public m_Ecosim As Ecosim.cEcosimModel
    Public m_TracerData As cContaminantTracerDataStructures

    Public Search As cSearchDatastructures

    Public EcoFunctions As cEcoFunctions
    Public TLlockOb As Object

    Public Bcw(,,) As Single
    Public C(,,) As Single
    Public d(,,) As Single
    Public e(,,) As Single
    Public BEQLast(,,) As Single
    ' Public WchangeVar(,,) As Single
    Public Btime() As Single
    Public F(,,) As Single
    Public AMm(,,) As Single
    Public Ecode() As Integer
    Public HdenCell(,,) As Single
    Public RelFitness(,,) As Single
    Public FtimeCell(,,) As Single

    ''' <summary>
    ''' Consumption rate by cell, group. EatenBy/Biomass
    ''' </summary>
    Public Cper(,,) As Single
    Public PconSplit() As Single
    Public RelRepStanza() As Single
    Public Tstanza() As Single
    Public PbSpace() As Single

    'needs to be set from ecospace, but not references
    'Public Tn As Integer
    Public nvar2 As Integer
    Public itt As Integer
    Public PPScale As Double
    Public TimeStep2 As Single
    Public MinChange As Single

    'locals
    'Private ebb() As Single
    Private BB() As Single
    Private loss() As Single
    Private RelPPupwell As Single
    Private RelR As Single, RelRS As Single, Rflow As Single
    Private Flowin() As Single
    Private FlowoutRate() As Single
    Private ieco As Integer
    Private isc As Integer
    Private isp As Integer
    Private ist As Integer
    Private EatEff() As Single
    Private VulPred() As Single
    Private ig As Integer
    Private pbb() As Single
    Private TimeStepC As Single

    Private lossSpace()(,) As Single
    Private ReadOnly m_logger As ILogger = LoggingContext.CreateLogger(Of cSpaceSolver)()

    'These are total sums for every cell, so must be summed for each thread seperately, then combined after they've all run
    'Public BtimeLocal() As Single
    Public TotLossThread() As Single
    Public TotEatenByThread() As Single
    Public TotBiomThread() As Single
    Public TotPredThread() As Single
    Public TotIFDweightThread() As Single

    ''' <summary>
    ''' Sum of Effort, Sailing Effort, Catch and Value across cells by fleet
    ''' </summary>
    Public ResultsByFleet(,) As Single

    ''' <summary>
    ''' Sum of Biomass, Relative Biomass and Catch by group across cells by group
    ''' </summary>
    Public ResultsByGroup(,) As Single

    ''' <summary>Sum of Catch and Value across cells by fleet group</summary>
    Public ResultsByFleetGroup(,,) As Single

    ''' <summary>Sum of Catch (absolute) by region x fleet x group.</summary>
    ''' <remarks>Cell catches are multiplied by <see cref="cEcospaceDataStructures.CellArea"/></remarks>
    Public ResultsCatchRegionGearGroup(,,) As Single
    ''' <summary>Average landings (absolute) by region x fleet x group</summary>
    ''' <remarks>Cell landings are multiplied by <see cref="cEcospaceDataStructures.CellArea"/></remarks>
    Public ResultsLandingsRegionGearGroup(,,) As Single
    ''' <summary>Average value (absolute) by region x fleet x group</summary>
    ''' <remarks>Cell values are multiplied by <see cref="cEcospaceDataStructures.CellArea"/></remarks>
    Public ResultsValueRegionGearGroup(,,) As Single
    ''' <summary>Sum of consumption by region x pred x prey</summary>
    ''' <remarks>Cell consumptions are multiplied by <see cref="cEcospaceDataStructures.CellArea"/></remarks>
    Public ResultsConsumptionRegionPredPrey(,,) As Single

    ''' <summary>
    ''' Sum of Landings across cells by group fleet
    ''' </summary>
    Public Landings(,) As Single

    'the ip groups to solve
    Public iFrstCell As Integer
    Public iLstCell As Integer

    'variables from m_ESData, used locally
    Private Hden() As Single
    Private Ftime() As Single
    Private Fish1() As Single
    Private FishTime() As Single

    ''' <summary>
    ''' Fishing Effort multiplier by Fleet,time = 0
    ''' </summary>
    ''' <remarks>
    ''' This array has the same signature as cEcosimDataStructures.FishRateGear(fleet,time)
    ''' but never uses the time index.
    ''' This is so EcoSpace can share the same functions as Ecosim but never have to worry about the different time step indexes.
    ''' Ecospace populates the zero time index with the effort multiplier for the current timestep at the start of each timestep.
    '''  </remarks>
    Private FishRateGear(,) As Single
    Private pred() As Single
    Private Eatenof() As Single
    Private Eatenby() As Single
    Private RelaSwitch() As Single
    Private NutBiom As Single
    Private NutFree As Single
    Private MedVal() As Single

    ''' <summary>Consumption (prey, pred)</summary>
    Private Consumpt(,) As Single

    'Contaminant tracing used locally
    Dim Derivcon() As Single, Cintotal() As Single, Closs() As Single, ConCtot As Single
    'Dim ConKtrophic()(,) As Single

    Private RtoNext As Single
    Private SurvRat As Single

    ''' <summary>Detritus by Group</summary>
    ''' <remarks>Added for Atlantis coupling. Local copy passes to SimDetritusMT() by each thread(this prevents cross thread corruption) then used to update map </remarks>
    Private GroupDetritus() As Single

    Private m_stpWatch As Stopwatch

    Private BBRatio() As Single

    Private TotFisheriesDiscards As Single

    Private m_bFitnessSet(,) As Boolean

    Private m_moLoss() As Single

    Public Sub Init()
        'local spatial variables
        ReDim Me.loss(Me.m_Data.NGroups)
        ReDim Me.pbb(Me.m_Data.NGroups)
        ReDim Me.EatEff(Me.m_Data.nvartot)
        ReDim Me.VulPred(Me.m_Data.nvartot)
        ReDim Me.Flowin(Me.m_Data.nvartot)
        ReDim Me.FlowoutRate(Me.m_Data.nvartot)
        'ReDim ebb(m_Data.nvartot)
        ReDim Me.BB(Me.m_Data.nvartot)

        'local versions of ecosim variables
        ReDim Me.Hden(Me.m_Data.NGroups)
        ReDim Me.Ftime(Me.m_Data.NGroups)
        ReDim Me.Fish1(Me.m_Data.NGroups)
        ReDim Me.FishTime(Me.m_Data.NGroups)
        ReDim Me.pred(Me.m_Data.NGroups)
        ReDim Me.Eatenof(Me.m_Data.NGroups)
        ReDim Me.Eatenby(Me.m_Data.NGroups)
        ReDim Me.MedVal(Me.m_SimData.BioMedData.MediationShapes)

        ReDim Me.Consumpt(Me.m_Data.NGroups, Me.m_Data.NGroups)

        'FishRateGear(nFleets,nTime) used to pass Effort in the current cell at the current timestep to both SimDeritusMT and SetMedFunctions()
        'Effort from the current cell time step is stored in the zero index i.e. FishRateGear(fleet,0) = EffortSpace(fleet,row,col)
        'this allows cSpaceSolver.FishRateGear(fleet,time) to remain compatible with cEcosimDataStructures.FishRateGear(fleet,time)
        'and both Sim and Space can use the same methods... 
        ReDim Me.FishRateGear(Me.m_Data.nFleets, 0)

        'thread copy of global sums
        ' ReDim BtimeLocal(m_Data.NGroups)
        ReDim Me.TotLossThread(Me.m_Data.NGroups)
        ReDim Me.TotEatenByThread(Me.m_Data.NGroups)
        ReDim Me.TotBiomThread(Me.m_Data.NGroups)
        ReDim Me.TotPredThread(Me.m_Data.NGroups)
        ReDim Me.TotIFDweightThread(Me.m_Data.NGroups)
        ReDim Me.GroupDetritus(Me.m_Data.NGroups)

        ReDim Me.BBRatio(Me.m_Data.NGroups)

        ReDim Me.ResultsByGroup([Enum].GetValues(GetType(eSpaceResultsGroups)).Length, Me.m_Data.NGroups)
        ReDim Me.ResultsByFleet([Enum].GetValues(GetType(eSpaceResultsFleets)).Length, Me.m_Data.nFleets)
        ReDim Me.ResultsByFleetGroup([Enum].GetValues(GetType(eSpaceResultsFleetsGroups)).Length, Me.m_Data.nFleets, Me.m_Data.NGroups)
        ReDim Me.Landings(Me.m_Data.NGroups, Me.m_Data.nFleets)
        ReDim Me.ResultsCatchRegionGearGroup(Me.m_Data.nRegions, Me.m_Data.nFleets, Me.m_Data.NGroups)
        ReDim Me.ResultsLandingsRegionGearGroup(Me.m_Data.nRegions, Me.m_Data.nFleets, Me.m_Data.NGroups)
        ReDim Me.ResultsValueRegionGearGroup(Me.m_Data.nRegions, Me.m_Data.nFleets, Me.m_Data.NGroups)
        ReDim Me.ResultsConsumptionRegionPredPrey(Me.m_Data.nRegions, Me.m_Data.NGroups, Me.m_Data.NGroups)

        'local copies are initialized from the ecosim data
        Array.Copy(Me.m_SimData.Hden, Me.Hden, Me.m_Data.NGroups + 1)
        Array.Copy(Me.m_SimData.Ftime, Me.Ftime, Me.m_Data.NGroups + 1)
        Array.Copy(Me.m_SimData.Fish1, Me.Fish1, Me.m_Data.NGroups + 1)
        Array.Copy(Me.m_SimData.FishTime, Me.FishTime, Me.m_Data.NGroups + 1)
        Array.Copy(Me.m_SimData.pred, Me.pred, Me.m_Data.NGroups + 1)
        Array.Copy(Me.m_SimData.Eatenof, Me.Eatenof, Me.m_Data.NGroups + 1)
        Array.Copy(Me.m_SimData.Eatenby, Me.Eatenby, Me.m_Data.NGroups + 1)

        lossSpace = New Single(Me.m_Data.NGroups)(,) {}
        For igrp As Integer = 0 To Me.m_Data.NGroups
            lossSpace(igrp) = New Single(Me.m_Data.InRow, Me.m_Data.InCol) {}
        Next

        Me.m_ConTracer.Init(Me.m_TracerData, Me.m_PathData, Me.m_SimData, Me.m_Stanza)
        Me.m_ConTracer.CInitialize()

        Me.m_ConTracer.ThreadID = Me.ThreadID

        Me.m_bFitnessSet = New Boolean(Me.m_Data.InRow, Me.m_Data.InCol) {}

        Me.m_moLoss = New Single(Me.m_Data.NGroups) {}

        Me.m_stpWatch = New Stopwatch

    End Sub

    Public Sub Clear()

        Try
            'each solver get its own Contaminant Tracer data and model
            Me.m_TracerData.Clear()
            Me.m_ConTracer = Nothing
        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".Clear() Exception: " & ex.Message)
            m_logger.LogError(ex, ".Clear() Exception")
        End Try

    End Sub

    ''' <summary>
    ''' Set the groups to iterate over.
    ''' </summary>
    ''' <param name="iFirstCell"></param>
    ''' <param name="iLastCell"></param>
    ''' <remarks>Call for each thread, before the thread is started, to set the groups to solve.</remarks>
    Public Sub FirstLastCells(ByVal iFirstCell As Integer, ByVal iLastCell As Integer)
        Me.iFrstCell = iFirstCell
        Me.iLstCell = iLastCell
    End Sub

    Public Sub SetTimeStepC(ByVal timestpc As Single)
        Me.TimeStepC = timestpc
    End Sub

    ''' <summary>
    ''' Do any processing necessary at the start of a new year
    ''' </summary>
    ''' <param name="iYear"></param>
    ''' <remarks></remarks>
    Public Sub YearTimeStep(ByVal iYear As Integer)
        Try
            If Me.Search.bInSearch Then
                'Indicators need to clear out there yearly data
                ' Indic.YearTimeStep(m_EPData)

            End If
        Catch ex As Exception
            m_logger.LogError(ex, "cSpaceSolver.YearTimeStep Exception")
        End Try

    End Sub

    Public Sub resetSpinup()
        'reset the Fitness Calculated flag
        'this will force the base relative fitness movement to be re-calculated to the first year after the spinup
        'See setRelFitnessBase()
        Me.m_bFitnessSet = New Boolean(Me.m_Data.InRow, Me.m_Data.InCol) {}
        ' Also reset concentrations accumulated during spin-up
        Me.m_ConTracer.CInitialize()
    End Sub

    Private Sub InitForTimestep()

        Try

            If Me.m_TracerData.EcoSpaceConSimOn Then
                ReDim Me.Derivcon(Me.m_PathData.NumGroups), Me.Cintotal(Me.m_PathData.NumGroups), Me.Closs(Me.m_PathData.NumGroups)
            End If

            'Clear out the results
            Array.Clear(Me.Landings, 0, Me.Landings.Length)
            Array.Clear(Me.ResultsByGroup, 0, Me.ResultsByGroup.Length)
            Array.Clear(Me.ResultsByFleet, 0, Me.ResultsByFleet.Length)
            Array.Clear(Me.ResultsByFleetGroup, 0, Me.ResultsByFleetGroup.Length)
            Array.Clear(Me.ResultsCatchRegionGearGroup, 0, Me.ResultsCatchRegionGearGroup.Length)
            Array.Clear(Me.ResultsLandingsRegionGearGroup, 0, Me.ResultsLandingsRegionGearGroup.Length)
            Array.Clear(Me.ResultsValueRegionGearGroup, 0, Me.ResultsValueRegionGearGroup.Length)
            Array.Clear(Me.ResultsConsumptionRegionPredPrey, 0, Me.ResultsConsumptionRegionPredPrey.Length)

            'Array.Clear(Me.BtimeLocal, 0, m_Data.NGroups)
            Array.Clear(Me.TotLossThread, 0, Me.m_Data.NGroups)
            Array.Clear(Me.TotEatenByThread, 0, Me.m_Data.NGroups)
            Array.Clear(Me.TotBiomThread, 0, Me.m_Data.NGroups)
            Array.Clear(Me.TotPredThread, 0, Me.m_Data.NGroups)
            Array.Clear(Me.TotIFDweightThread, 0, Me.m_Data.NGroups)

            Me.TotFisheriesDiscards = 0

        Catch ex As Exception
            Debug.Assert(False, ex.Message)
            m_logger.LogError(ex, "cSpaceSolver.InitForTimestep Exception")
        End Try

    End Sub

#Region "Public 'Solve'"

    ''' <summary>
    ''' This is the method that the ThreadPool calls. 
    ''' It must have the object argument to match the Delegate signature required by ThreadPool.QueueUserWorkItem()
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub Solve(ByVal obParam As Object)
        'For our purposes here we are ignoring the obParam argument 
        'this sub signature is required by the ThreadPool.QueueUserWorkItem(...)

        Me.InitForTimestep()

        'Dim thrdID As Integer = Threading.Thread.CurrentThread.ManagedThreadId
        'Console.WriteLine("Solve Derivt OBID = " & Me.ThreadID.ToString & ", ThreadID = " & thrdID.ToString & ", Start T = " & DateTime.Now.ToLongTimeString)
        'Console.WriteLine("     N Map Cells = " & (iLstCell - iFrstCell + 1).ToString)

        Me.m_stpWatch.Reset()
        Me.m_stpWatch.Start()

        Try
            Dim iCell As Integer

            'do the processing here
            For iCell = Me.iFrstCell To Me.iLstCell

                Debug.Assert(Me.m_Data.Depth(Me.m_Data.iWaterCellIndex(iCell), Me.m_Data.jWaterCellIndex(iCell)) > 0, "Opps Ecospace iWaterCellIndex() and jWaterCellIndex() contain land cells.")
                'iCell is the linear index of the two dimensional spatial array
                'iWaterCellIndex(iCell) and jWaterCellIndex(iCell) were populted with the indexes(irow,jcol) of water cells only during initialization
                'Dim st As Double = Me.m_stpWatch.Elapsed.TotalMilliseconds
                Me.SolveCell(Me.m_Data.iWaterCellIndex(iCell), Me.m_Data.jWaterCellIndex(iCell))

                'Me.lstCellCompTimes.Add(Me.m_stpWatch.Elapsed.TotalMilliseconds - st)
            Next iCell

        Catch ex As Exception
            m_logger.LogError(ex, "cSpaceSolver.Solve Exception")
            Debug.Assert(False, ex.Message)
        End Try

        'set signal state to 'signaled' 
        'the processing has finished SignalState.WaitOne() will return immediately
        If Interlocked.Decrement(cSpaceSolver.ThreadIncrementer) = 0 Then
            Me.WaitHandle.Set()
        End If

        Me.m_stpWatch.Stop()
        Me.RunTimeSeconds = Me.m_stpWatch.Elapsed.TotalSeconds
        ' Console.WriteLine("SpaceSolver.Solve() ID " & Me.ThreadID.ToString & " Run time(sec)" & (Me.m_stpWatch.Elapsed.TotalSeconds).ToString)

    End Sub

    Public Sub SolveC(ByVal obParam As Object)
        'For our purposes here we are ignoring the obParam argument 
        'this sub signature is required by the ThreadPool.QueueUserWorkItem(...)

        ReDim Me.Derivcon(Me.m_PathData.NumGroups), Me.Cintotal(Me.m_PathData.NumGroups), Me.Closs(Me.m_PathData.NumGroups)

        'Dim thrdID As Integer = Threading.Thread.CurrentThread.ManagedThreadId
        'Console.WriteLine("Solve Derivt OBID = " & Me.ThreadID.ToString & ", ThreadID = " & thrdID.ToString & ", Start T = " & DateTime.Now.ToLongTimeString)
        'Console.WriteLine("     N Map Cells = " & (iLstCell - iFrstCell + 1).ToString)

        'Me.m_stpWatch.Reset()
        Me.m_stpWatch.Start()

        Try
            Dim iCell As Integer

            'do the processing here
            For iCell = Me.iFrstCell To Me.iLstCell

                Debug.Assert(Me.m_Data.Depth(Me.m_Data.iWaterCellIndex(iCell), Me.m_Data.jWaterCellIndex(iCell)) > 0, "Opps Ecospace iWaterCellIndex() and jWaterCellIndex() contain land cells.")
                'iCell is the linear index of the two dimensional spatial array
                'iWaterCellIndex(iCell) and jWaterCellIndex(iCell) were populted with the indexes(irow,jcol) of water cells only during initialization
                'Dim st As Double = Me.m_stpWatch.Elapsed.TotalMilliseconds
                Me.SolveCellC(Me.m_Data.iWaterCellIndex(iCell), Me.m_Data.jWaterCellIndex(iCell))

                'Me.lstCellCompTimes.Add(Me.m_stpWatch.Elapsed.TotalMilliseconds - st)
            Next iCell

        Catch ex As Exception
            m_logger.LogError(ex, "cSpaceSolver.SolveC Exception")
            Debug.Assert(False, ex.Message)
        End Try

        'set signal state to 'signaled' 
        'the processing has finished SignalState.WaitOne() will return immediately
        If Interlocked.Decrement(cSpaceSolver.ThreadIncrementer) = 0 Then
            Me.WaitHandle.Set()
        End If

        Me.m_stpWatch.Stop()
        Me.RunTimeSeconds = Me.m_stpWatch.Elapsed.TotalSeconds
        ' Console.WriteLine("SpaceSolver.Solve() ID " & Me.ThreadID.ToString & " Run time(sec)" & (Me.m_stpWatch.Elapsed.TotalSeconds).ToString)

    End Sub
#End Region

    Private Function SolveCell(ByVal i As Integer, ByVal j As Integer) As Boolean
        Dim iGrp As Integer
        Dim PopWt As Single
        Dim CellAreaKM2 As Single

        Try
            ' Debug.Assert(Me.m_Data.Depth(i, j) > 0)
            ' System.Console.WriteLine("Thread ID, " & Me.ThreadID & ", " & i.ToString & ", " & j.ToString)
            'this changes the timestep for higher order numerical sceme.  the timestep isn't actually different, it's a multiplier
            Me.TimeStep2 = CSng(Me.m_Data.TimeStep * 0.66667)

            Me.TotFisheriesDiscards = 0

            'Cell area in KM2 at the equator * relative width of the cell
            CellAreaKM2 = CSng(Me.m_Data.CellLength ^ 2.0) * Me.m_Data.RelativeCellWidth(i)

            If Me.m_TracerData.EcoSpaceConSimOn Then
                'set contaminant concentration for the environment
                'before entering the group loop
                Debug.Assert(Not (Me.m_Data.Ccell(i, j, 0) < 0.0 Or Single.IsNaN(Me.m_Data.Ccell(i, j, 0)) Or Double.IsInfinity(Me.m_Data.Ccell(i, j, 0))))
                Me.m_ConTracer.ConcTr(0) = Me.m_Data.Ccell(i, j, 0)
            End If

            For iGrp = 1 To Me.m_Data.NGroups
                'abmpa: at this point (after having been in solvegrid) the BCell holds
                'the long term equilibrium biomass or at least an approx to)

                If Me.m_Data.Depth(i, j) = 0 Then Me.m_Data.Bcell(i, j, iGrp) = 0
                If Single.IsNaN(Me.m_Data.Bcell(i, j, iGrp)) = True Then Me.m_Data.Bcell(i, j, iGrp) = 1.0E-30
                Me.BB(iGrp) = Me.m_Data.Bcell(i, j, iGrp)

                If Me.m_TracerData.EcoSpaceConSimOn Then
                    If Single.IsNaN(Me.m_Data.Ccell(i, j, iGrp)) Then
                        Me.m_Data.Ccell(i, j, iGrp) = 0.0
                    End If
                    Me.m_ConTracer.ConcTr(iGrp) = Me.m_Data.Ccell(i, j, iGrp)
                End If

                'sum biomass over all the cells
                'this is now done individually for each thread, then summed outside the threads
                'Btime(ip) = Btime(ip) + BB(ip)
                'BtimeLocal(iGrp) = BB(iGrp) * m_Data.Width(i) + BtimeLocal(iGrp)

                If (Me.m_SimData.NoIntegrate(iGrp) = iGrp Or Me.m_SimData.NoIntegrate(iGrp) < 0) And Me.m_SimData.SimGE(iGrp) > 0 Then
                    If (Me.Cper(i, j, iGrp) > 0 And Me.m_SimData.FtimeAdjust(iGrp) > 0) Then
                        Me.FtimeCell(i, j, iGrp) = Me.FtimeCell(i, j, iGrp) * (0.7F + 0.3F * Me.m_SimData.Cbase(iGrp) / Me.Cper(i, j, iGrp))
                    End If
                    '  FtimeCell(i, j, ip) = Cbase(ip) / Cper(i, j, ip)
                    If Me.FtimeCell(i, j, iGrp) > Me.m_SimData.FtimeMax(iGrp) Then Me.FtimeCell(i, j, iGrp) = Me.m_SimData.FtimeMax(iGrp)
                    If Me.FtimeCell(i, j, iGrp) < 0.1 Then Me.FtimeCell(i, j, iGrp) = 0.1
                    Me.Ftime(iGrp) = Me.FtimeCell(i, j, iGrp)
                End If

                Me.Hden(iGrp) = Me.HdenCell(i, j, iGrp)

                'Set FishTime() (F, fishing mortality) for this timestep cell
                If Me.m_Data.Depth(i, j) > 0 Then
                    If Me.m_Data.PredictEffort Then
                        'F set by cEcospace.PredictEffortDistributionThreaded()
                        Me.FishTime(iGrp) = Me.m_Data.Ftot(iGrp, i, j)
                        'Debug.Assert(Single.IsNaN(Me.FishTime(iGrp)) = False)
                        '****Following lines set fishrategear for Simdetritus
                        For Me.ig = 1 To Me.m_Data.nFleets
                            Me.FishRateGear(Me.ig, 0) = Me.m_Data.EffortSpace(Me.ig, i, j)
                            'effortspace should be 1.0 for cell with "average" effort by gear type ig
                        Next
                    Else
                        'Not Predicting Effort. Use the Ecosim F Time Series
                        'F = Ecosim F at this time step
                        Me.FishTime(iGrp) = Me.m_SimData.FishRateNo(iGrp, itt)

                        For Me.ig = 1 To Me.m_Data.nFleets
                            'Effort used to calculate Catch and Value in cEcospace.accumCatchData
                            Me.m_Data.EffortSpace(Me.ig, i, j) = 1.0
                            'fishrategear for Simdetritus
                            Me.FishRateGear(Me.ig, 0) = 1 ' 1 x FishMGear(ig, ip)
                        Next
                    End If 'If m_Data.PredictEffort > 0 Then

                Else
                    'depth<=0
                    Me.FishTime(iGrp) = 0
                    '****Following line sets fishrategear for Simdetritus
                    For Me.ig = 1 To Me.m_Data.nFleets
                        Me.FishRateGear(Me.ig, 0) = 0
                    Next

                End If 'If m_Data.Depth(i, j) > 0 Then

                Me.EatEff(iGrp) = 1
                Me.VulPred(iGrp) = 1

                If Me.m_Data.HabCap(iGrp)(i, j) < 0.1 Then
                    Me.VulPred(iGrp) = Me.m_Data.RelVulBad(iGrp)
                End If

                Me.EatEff(iGrp) = Me.m_Data.HabCap(iGrp)(i, j) 'm_Data.EatEffBad(iGrp)

            Next iGrp

            Me.accumCatchData(Me.itt, Me.iYear, Me.BB, Me.FishTime, i, j)

            For Me.isc = 1 To Me.m_Data.Nvarsplit
                Me.ieco = Me.Ecode(Me.isc)
                'ebb(nvar2 + isc) = m_Data.Bcell(i, j, nvar2 + isc)

                If Me.m_Data.NewMultiStanza Or Me.m_Data.UseIBM Then
                    Me.pred(Me.ieco) = Me.m_Data.PredCell(i, j, Me.ieco)
                Else
                    Me.pred(Me.ieco) = Me.m_Data.Bcell(i, j, Me.nvar2 + Me.isc) * Me.PconSplit(Me.isc) ' Nadult(i, j, ip)
                End If
            Next Me.isc

            Me.RelPPupwell = 1 + Me.m_Data.PPupWell * Me.m_Data.UpVel(i, j) / Me.m_Data.CellLength

            If Me.RelPPupwell < 1 Then Me.RelPPupwell = 1

            Dim scaledPP As Double = (Me.m_Data.RelPP(i, j) / Me.PPScale * Me.RelPPupwell)

            'jb compute Flowin() and FlowoutRate() for all groups for this row/col
            Me.derivtRed(Me.BB, Me.Flowin, Me.FlowoutRate, Me.EatEff, Me.VulPred, scaledPP, i, j)

            If Me.m_TracerData.EcoSpaceConSimOn Then
                Me.m_ConTracer.ConKtrophic = m_Data.ConKtrophic(i, j)
                Me.m_ConTracer.ConKdet = m_Data.ConKdetSpace(i, j)
                Me.m_ConTracer.loss = Me.loss 'set loss to ecospace loss for this cell
                Me.m_ConTracer.ConDeriv(Me.BB, Me.Derivcon, Me.Cintotal, Me.Closs, Me.m_Data.RelCin(i, j), True)
            End If

            If Not Me.m_bFitnessSet(i, j) Then
                Me.setRelFitnessBase(i, j)
            End If

            'Debug.Assert(i <> 26 And j <> 26)
            Dim minMovement As Single = 1 / cEcoSpace.MAX_FITNESSMOVEMENT
            'jb now populate the spatial matrixes with the data computed by derivtRed() for this cell across all groups
            For iGrp = 1 To Me.m_Data.NGroups
                Me.HdenCell(i, j, iGrp) = Me.Hden(iGrp)

                If (Me.m_Data.Kmovefit(iGrp) > 0) And Me.m_bFitnessSet(i, j) Then
                    Dim FCatch As Single = Me.FishTime(iGrp) * Me.BB(iGrp)

                    Dim PredFitness As Single = ((Me.m_SimData.SimGE(iGrp) * Me.Eatenby(iGrp)) / (Me.loss(iGrp) - FCatch + 1.0E-10F)) / Me.m_Data.RelFitnessBase(i, j, iGrp)

                    If Single.IsNaN(PredFitness) Then PredFitness = 1.0

                    If PredFitness < minMovement Then PredFitness = minMovement
                    If PredFitness > cEcoSpace.MAX_FITNESSMOVEMENT Then PredFitness = cEcoSpace.MAX_FITNESSMOVEMENT
                    Me.RelFitness(i, j, iGrp) = CSng((1 - cEcoSpace.W_RELAX_FITNESS) * Me.RelFitness(i, j, iGrp) + cEcoSpace.W_RELAX_FITNESS * PredFitness)

                Else
                    Me.RelFitness(i, j, iGrp) = 1.0
                End If '

                Me.ResultsByGroup(eSpaceResultsGroups.FishingMort, iGrp) += Me.FishTime(iGrp)
                Me.ResultsByGroup(eSpaceResultsGroups.ConsumpRate, iGrp) += Me.Eatenby(iGrp) / (Me.BB(iGrp) + 1.0E-20F)
                Me.ResultsByGroup(eSpaceResultsGroups.PredMortRate, iGrp) += Me.Eatenof(iGrp) / (Me.BB(iGrp) + 1.0E-20F)
                'loss(group) units are KM2
                'TotalLoss sum of loss for the total area of the cell. Not just KM2
                Me.ResultsByGroup(eSpaceResultsGroups.TotalLoss, iGrp) += Me.loss(iGrp) * CellAreaKM2

                Me.ResultsByGroup(eSpaceResultsGroups.OtherMortalityLoss, iGrp) += Me.m_moLoss(iGrp)
            Next

            For iGrp = 1 To Me.m_Data.NGroups

                If Me.m_PathData.isEcospaceModelCoupled Then Me.m_Data.GroupDetritus(i, j, iGrp) = Me.GroupDetritus(iGrp)

                Me.F(i, j, iGrp) = Me.Flowin(iGrp)
                Me.AMm(i, j, iGrp) = -Me.FlowoutRate(iGrp) - Me.Bcw(i + 1, j, iGrp) - Me.C(i - 1, j, iGrp) - Me.d(i, j, iGrp) - Me.e(i, j, iGrp)
                If Me.AMm(i, j, iGrp) >= 0 Then Me.AMm(i, j, iGrp) = -1.0E+30
                'm_Data.deriv2(i, j, ip) = m_Data.deriv(i, j, ip)
                'm_Data.deriv(i, j, ip) = AMm(i, j, ip) * m_Data.Bcell(i, j, ip) + F(i, j, ip) + Bcw(i, j, ip) * m_Data.Bcell(i - 1, j, ip) + C(i, j, ip) * m_Data.Bcell(i + 1, j, ip) + d(i, j - 1, ip) * m_Data.Bcell(i, j - 1, ip) + e(i, j + 1, ip) * m_Data.Bcell(i, j + 1, ip)
                If Me.m_Data.SpaceTime Then
                    Me.AMm(i, j, iGrp) = Me.AMm(i, j, iGrp) - 1 / Me.TimeStep2
                    'this is for new 2nd order BDF numerical sceme (replacing backwards euler)
                    Me.F(i, j, iGrp) = Me.F(i, j, iGrp) + (1.3333F * Me.m_Data.Bcell(i, j, iGrp) - 0.3333F * Me.m_Data.Blast(i, j, iGrp)) / Me.TimeStep2
                    Me.m_Data.Blast(i, j, iGrp) = Me.m_Data.Bcell(i, j, iGrp)
                End If

                If Me.m_SimData.SimGE(iGrp) > 0 Then
                    Me.Cper(i, j, iGrp) = Me.Eatenby(iGrp) / (Me.m_Data.Bcell(i, j, iGrp) + 1.0E-20F)
                End If
                If Me.Cper(i, j, iGrp) < 0.001 * Me.m_SimData.Cbase(iGrp) Then
                    Me.Cper(i, j, iGrp) = 0.001F * Me.m_SimData.Cbase(iGrp)
                End If

                Me.m_Data.MOLoss(iGrp)(i, j) = Me.m_moLoss(iGrp)

            Next iGrp

            If Me.m_TracerData.EcoSpaceConSimOn Then
                For iGrp = 0 To Me.m_Data.NGroups
                    Me.m_Data.Ftr(i, j, iGrp) = Me.Cintotal(iGrp)
                    Me.m_Data.AMmTr(i, j, iGrp) = -Me.Closs(iGrp) - Me.Bcw(i + 1, j, iGrp) - Me.C(i - 1, j, iGrp) - Me.d(i, j, iGrp) - Me.e(i, j, iGrp)
                    If Me.m_Data.AMmTr(i, j, iGrp) >= 0 Then Me.m_Data.AMmTr(i, j, iGrp) = -1.0E+30
                    '   If m_Data.SpaceTime And FastIntegrate(ip) = False Then
                    '  m_data_Cder(i, j, iGrp) = Cintotal(iGrp) + m_Data.AMmTr(i, j, iGrp) + inflows from surrounding cells
                    'old backward euler
                    'm_Data.Ftr(i, j, iGrp) = m_Data.Ftr(i, j, iGrp) + m_Data.Ccell(i, j, iGrp) / m_data.TimeStep
                    'm_Data.AMmTr(i, j, iGrp) = m_Data.AMmTr(i, j, iGrp) - 1 / m_Data.TimeStep
                    'new BDF2
                    'm_Data.Ftr(i, j, iGrp) = m_Data.Ftr(i, j, iGrp) + (1.3333F * m_Data.Ccell(i, j, iGrp) - 0.3333F * m_Data.Clast(i, j, iGrp)) / TimeStep2 '/ m_Data.TimeStep
                    'm_Data.AMmTr(i, j, iGrp) = m_Data.AMmTr(i, j, iGrp) - 1 / TimeStep2 '/ m_Data.TimeStep
                    'THIS NO LONGER DOES A TIMESTEP
                    'This will no longer be used SolveGrid!
                    'This is only to calculate the expected timestep
                    'The timestep version of this is now calculated in SolveCellC
                    '  End If
                Next
            End If

            Me.isc = 0
            For Me.isp = 1 To Me.m_Stanza.Nsplit
                Me.ieco = Me.m_Stanza.EcopathCode(Me.isp, Me.m_Stanza.Nstanza(Me.isp))

                Me.RelR = Me.m_Data.Bcell(i, j, Me.ieco) * Me.RelRepStanza(Me.isp) ' * m_Data.HabCap(i, j, ieco) * m_Data.nWaterCells / m_Data.TotHabCap(ieco) ' Added HabCap correction for recruitment rate
                For Me.ist = 1 To Me.m_Stanza.Nstanza(Me.isp)
                    Me.isc = Me.isc + 1
                    Me.ieco = Me.m_Stanza.EcopathCode(Me.isp, Me.ist)
                    If Me.m_Data.NewMultiStanza Then
                        'accumulate information needed to predict mean stanza loss, feeding, IFD weights from derivtred outputs
                        'these arrays are used in the new SpaceSplitUpdate subroutine for predicting mortality
                        'rate and growth rate averages over space by age in that update routine
                        'IFDweight is used to predict proportion of biomass of ieco stanza that will be on cell i,j
                        If Me.m_Data.Depth(i, j) > 0 Then
                            PopWt = Me.m_Data.Bcell(i, j, Me.nvar2 + Me.isc)
                            Me.TotLossThread(Me.ieco) = Me.TotLossThread(Me.ieco) + Me.loss(Me.ieco) * PopWt
                            Me.TotEatenByThread(Me.ieco) = Me.TotEatenByThread(Me.ieco) + Me.Eatenby(Me.ieco) * PopWt
                            Me.TotBiomThread(Me.ieco) = Me.TotBiomThread(Me.ieco) + Me.m_Data.Bcell(i, j, Me.ieco) * PopWt
                            Me.TotPredThread(Me.ieco) = Me.TotPredThread(Me.ieco) + Me.pred(Me.ieco) * PopWt
                            'm_Data.IFDweight(i, j, ieco) = ((Eatenby(ieco) / pred(ieco)) / (loss(ieco) / m_Data.Bcell(i, j, ieco))) ^ m_Data.IFDPower
                            Me.m_Data.IFDweight(i, j, Me.ieco) = PopWt 'm_Data.Bcell(i, j, nvar2 + isc)
                            Me.TotIFDweightThread(Me.ieco) = Me.TotIFDweightThread(Me.ieco) + Me.m_Data.IFDweight(i, j, Me.ieco)
                        End If
                    ElseIf Me.m_Data.UseIBM Then
                        Me.m_Stanza.Zcell(i, j, Me.ieco) = Me.loss(Me.ieco) / (Me.m_Data.Bcell(i, j, Me.ieco) + 1.0E-30F)
                        If Me.m_Data.Bcell(i, j, Me.ieco) = 0 Then
                            Me.m_Stanza.Zcell(i, j, Me.ieco) = 0
                        End If
                        Me.Cper(i, j, Me.ieco) = Me.Eatenby(Me.ieco) / (Me.m_Data.PredCell(i, j, Me.ieco) + 1.0E-30F)
                        If Me.m_Data.PredCell(i, j, Me.ieco) = 0 Then
                            Me.Cper(i, j, Me.ieco) = Me.m_SimData.Cbase(Me.ieco)
                        End If
                    End If
                    Me.SurvRat = CSng(Math.Exp(-Me.FlowoutRate(Me.ieco) * Me.Tstanza(Me.isc)))
                    Me.RelRS = Me.RelR * Me.SurvRat 'Math.Exp(-FlowoutRate(ieco) * Tstanza(isc))
                    If Me.ist = 1 Then '< m_Stanza.Nstanza(isp) Then
                        Me.Rflow = Me.RelR - Me.RelRS
                    Else
                        Me.Rflow = Me.RtoNext
                    End If
                    Me.RtoNext = Me.m_Data.Bcell(i, j, Me.nvar2 + Me.isc) * Me.FlowoutRate(Me.ieco) / (1 / (Me.SurvRat + 1.0E-20F) - 1)
                    Me.RelR = Me.RelRS
                    If Me.m_Data.NewMultiStanza Then
                        Me.Cper(i, j, Me.ieco) = Me.Eatenby(Me.ieco) / (Me.m_Data.PredCell(i, j, Me.ieco) + 1.0E-30F)
                        If Me.ist > 1 Then Me.Rflow = Me.m_Data.Bcell(i, j, Me.m_Stanza.EcopathCode(Me.isp, Me.ist - 1))
                    ElseIf Me.m_Data.UseIBM = False And Me.m_Data.NewMultiStanza = False Then
                        Me.Cper(i, j, Me.ieco) = Me.Eatenby(Me.ieco) / (Me.m_Data.Bcell(i, j, Me.nvar2 + Me.isc) * Me.PconSplit(Me.isc) + 1.0E-30F)
                    End If

                    Me.F(i, j, Me.nvar2 + Me.isc) = Me.Rflow
                    Me.AMm(i, j, Me.nvar2 + Me.isc) = -Me.FlowoutRate(Me.ieco) - Me.Bcw(i + 1, j, Me.ieco) - Me.C(i - 1, j, Me.ieco) - Me.d(i, j, Me.ieco) - Me.e(i, j, Me.ieco)
                    If Me.AMm(i, j, Me.nvar2 + Me.isc) >= 0 Then Me.AMm(i, j, Me.nvar2 + Me.isc) = -1.0E+30

                    If Me.m_Data.SpaceTime Then
                        Me.F(i, j, Me.nvar2 + Me.isc) = Me.F(i, j, Me.nvar2 + Me.isc) + (1.3333F * Me.m_Data.Bcell(i, j, Me.nvar2 + Me.isc) - 0.3333F * Me.m_Data.Blast(i, j, Me.nvar2 + Me.isc)) / Me.TimeStep2
                        'F(i, j, nvar2 + isc) = F(i, j, nvar2 + isc) + m_Data.Bcell(i, j, nvar2 + isc) / m_Data.TimeStep
                        Me.AMm(i, j, Me.nvar2 + Me.isc) = Me.AMm(i, j, Me.nvar2 + Me.isc) - 1 / Me.TimeStep2
                        Me.m_Data.Blast(i, j, Me.nvar2 + Me.isc) = Me.m_Data.Bcell(i, j, Me.nvar2 + Me.isc)
                    End If
                Next
            Next

            '                    'For MPA Seed routine:
            '                    'At equilibrium 0 = dB = G - ZB, hence Bo = G/Z, where Bo is in    m_data.bcell() and Z=-AMm()
            '                    'For the no fishing situation: Bclose ~ Bo Z / (Z-F) or
            '                    'Bclose = -Bcell(i,j,ip) * AMm(i,j,ip) / (AMm(i,j,p) - Ftime(i,j,ip))
            '                    'This is the long-term predicted biomass in the cell from not fishing there
            '                    '   If AMm(i, j, ip) > 0 Then Bclose(i, j, ip) = -Bcell(i, j, ip) * AMm(i, j, ip) / (AMm(i, j, P) - Ftime(i, j, ip))

            Return True

        Catch ex As Exception
            m_logger.LogError(ex, ".SolveCell() Exception")
            Debug.Assert(False, ex.StackTrace)
            Throw New ApplicationException(Me.ToString & ".SolveCell() Error: " & ex.Message)
        End Try

    End Function

    Private Function SolveCellC(ByVal i As Integer, ByVal j As Integer) As Boolean
        Dim iGrp As Integer

        Try
            'Dim bio() As Single
            ' System.Console.WriteLine("Thread ID, " & Me.ThreadID & ", " & i.ToString & ", " & j.ToString)
            'this changes the timestep for higher order numerical sceme.  the timestep isn't actually different, it's a multiplier
            'jb Apr-2023 TimeStep2c is not used any more. Integration done by cEcopace.runContaminantTracerExplicit1()
            'TimeStep2c = CSng(Me.TimeStepC * 0.66667)

            'Cell area in KM2 at the equator * relative width of the cell
            'jb Apr-2023 CellAreaKM2
            'CellAreaKM2 = CSng(Me.m_Data.CellLength ^ 2.0) * Me.m_Data.Width(i)

            For iGrp = 0 To Me.m_Data.NGroups
                Me.m_ConTracer.ConcTr(iGrp) = Me.m_Data.Ccell(i, j, iGrp)
            Next

            'System.Console.WriteLine(Me.m_ConTracer.ThreadID.ToString + ", " + i.ToString + ", " + j.ToString)
            'bio = New Single(Me.m_Data.NGroups) {}
            'Me.m_ConTracer.loss = Me.loss 'set loss to ecospace loss for this cell
            If Me.m_ConTracer.loss Is Nothing Or Me.m_ConTracer.bio Is Nothing Then
                Me.m_ConTracer.loss = New Single(Me.m_Data.NGroups) {}
                Me.m_ConTracer.bio = New Single(Me.m_Data.NGroups) {}

            Else
                System.Array.Clear(Me.m_ConTracer.loss, 0, Me.m_Data.NGroups)
                System.Array.Clear(Me.m_ConTracer.bio, 0, Me.m_Data.NGroups)
            End If

            'set state variables by cell from Ecospace
            For iGrp = 0 To Me.m_Data.NGroups
                Me.m_ConTracer.loss(iGrp) = Me.lossSpace(iGrp)(i, j)
                m_ConTracer.bio(iGrp) = Me.m_Data.Bcell(i, j, iGrp)
            Next

            'Set contaminant map variable calculated by cell
            'Detritus for this cell. 
            'Computed in SimDetritusMT()
            Me.m_ConTracer.ConKdet = m_Data.ConKdetSpace(i, j)
            'Consumption by pred prey link
            Me.m_ConTracer.ConKtrophic = m_Data.ConKtrophic(i, j)
            Me.m_ConTracer.ConDeriv(m_ConTracer.bio, Me.Derivcon, Me.Cintotal, Me.Closs, Me.m_Data.RelCin(i, j), True)

            For iGrp = 0 To Me.m_Data.NGroups
                Me.m_Data.Ftr(i, j, iGrp) = Me.Cintotal(iGrp)
                Me.m_Data.AMmTr(i, j, iGrp) = -Me.Closs(iGrp) - Me.Bcw(i + 1, j, iGrp) - Me.C(i - 1, j, iGrp) - Me.d(i, j, iGrp) - Me.e(i, j, iGrp)
                If Me.m_Data.AMmTr(i, j, iGrp) >= 0 Then Me.m_Data.AMmTr(i, j, iGrp) = -1.0E+30
                'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
                'Integration done by cEcopace.runContaminantTracerExplicit1()
                '   If m_Data.SpaceTime And FastIntegrate(ip) = False Then
                '  m_data_Cder(i, j, iGrp) = Cintotal(iGrp) + m_Data.AMmTr(i, j, iGrp) + inflows from surrounding cells
                'old backward euler
                'm_Data.Ftr(i, j, iGrp) = m_Data.Ftr(i, j, iGrp) + m_Data.Ccell(i, j, iGrp) / m_data.TimeStep
                'm_Data.AMmTr(i, j, iGrp) = m_Data.AMmTr(i, j, iGrp) - 1 / m_Data.TimeStep
                'new BDF2
                'm_Data.Ftr(i, j, iGrp) = m_Data.Ftr(i, j, iGrp) + m_Data.Ccell(i, j, iGrp) / TimeStepC
                'm_Data.Ftr(i, j, iGrp) = m_Data.Ftr(i, j, iGrp) + (1.3333F * m_Data.Ccell(i, j, iGrp) - 0.3333F * m_Data.Clast(i, j, iGrp)) / TimeStep2c '/ m_Data.TimeStep
                'm_Data.AMmTr(i, j, iGrp) = m_Data.AMmTr(i, j, iGrp) - 1 / TimeStepC '/ m_Data.TimeStep
                'm_Data.AMmTr(i, j, iGrp) = m_Data.AMmTr(i, j, iGrp) - 1 / TimeStep2c '/ m_Data.TimeStep

                '  End If
                'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
            Next

            Return True

        Catch ex As Exception
            m_logger.LogError(ex, ".SolveCellC() Exception")
            Debug.Assert(False, ex.StackTrace)
            Throw New ApplicationException(Me.ToString & ".SolveCellc() Error: " & ex.Message)
        End Try

    End Function

    Public Sub derivtRed(ByVal Biomass() As Single, ByRef Flowin() As Single, ByRef FlowoutRate() As Single, ByRef EatEff() As Single, ByRef VulPred() As Single, ByVal RelProdScaler As Double, ByVal iRow As Integer, ByVal iCol As Integer)
        'reduced derivatives for MPA equilibration procedure
        Dim i As Integer, j As Integer, ii As Integer
        Dim eat As Single, Pmult As Single
        Dim SimGEt As Single
        Dim Dwe As Single
        Dim Bprey As Single

        'Imported Detritus forcing function multiplier
        Dim DtImpMult As Single

        Dim aeff() As Single, Veff() As Single
        ReDim aeff(Me.m_SimData.inlinks), Veff(Me.m_SimData.inlinks)

        Dim Hdent() As Single
        ReDim Hdent(Me.m_Data.NGroups)

        'EwE5 ToDetritus() is declared at a global level
        'in EcoSpace this is the only place it is used so its scope is local to EcoSpace
        Dim ToDetritus() As Single
        ReDim ToDetritus(Me.m_Data.NGroups)

        Try

            'populate MedVal(nMedFunctions) with the Mediation Function multiplier for this Timestep
            Me.SetMedFunctions(Biomass)

            Me.setpred(Biomass)

            Array.Clear(Me.Eatenof, 0, Me.Eatenof.Length)
            Array.Clear(Me.Eatenby, 0, Me.Eatenof.Length)
            Array.Clear(Me.Consumpt, 0, Me.Consumpt.Length)

            Dwe = 0.5

            'set ecosim nutrients
            Me.NutBiom = 0
            For i = 1 To Me.m_Data.NGroups
                Me.NutBiom = Me.NutBiom + Biomass(i)
            Next

            Me.NutFree = CSng(Me.m_SimData.NutTot * RelProdScaler - Me.NutBiom)
            If Me.NutFree < Me.m_SimData.NutMin Then Me.NutFree = Me.m_SimData.NutMin

            Me.m_Data.RelNutMult(iRow, iCol) = 0.0F
            Me.SaveRelNutFree(Me.NutFree, iRow, iCol)

            For j = Me.m_Data.nLiving + 1 To Me.m_Data.NGroups
                ToDetritus(j - Me.m_Data.nLiving) = 0
            Next j

            Me.SetRelaSwitch(Biomass)

            'get first estimate of denominators of predation rate disc equations
            Dim ia As Integer, Vbiom() As Single, Vdenom() As Single
            'this requires first estimates of vulnerable biomasses Vbiom by foraging arena
            ReDim Vbiom(Me.m_SimData.Narena), Vdenom(Me.m_SimData.Narena)
            For ii = 1 To Me.m_SimData.inlinks
                i = Me.m_SimData.ilink(ii) : j = Me.m_SimData.jlink(ii) : ia = Me.m_SimData.ArenaLink(ii)
                'Debug.Assert(VulPred(i) = 1)
                aeff(ii) = Me.m_Data.Aspace(ii) * Me.Ftime(j) * Me.RelaSwitch(ii) * VulPred(i)
                Veff(ia) = Me.m_Data.Vspace(ia) * Me.Ftime(i)
                Me.ApplyAVmodifiers(aeff(ii), Veff(ia), i, Me.m_SimData.Jarena(ia), False, iRow, iCol)  '?not sure this will work right with multiple preds in arenas
                'Debug.Assert(EatEff(j) = 1)
                Vdenom(ia) = Vdenom(ia) + aeff(ii) * Me.pred(j) / Me.Hden(j) / EatEff(j)
            Next

            'then calculate first estimate using initial Hden estimates of vulnerable biomass in each arena
            For ia = 1 To Me.m_SimData.Narena
                i = Me.m_SimData.Iarena(ia)
                If Me.m_SimData.BoutFeeding Then
                    If Vdenom(ia) > 0 Then
                        Vbiom(ia) = Veff(ia) * Biomass(i) * (1 - CSng(Math.Exp(-Vdenom(ia)))) / Vdenom(ia)
                    Else
                        Vbiom(ia) = Veff(ia) * Biomass(i)
                    End If
                Else
                    Vbiom(ia) = Veff(ia) * Biomass(i) / (Me.m_Data.Vspace(ia) + Veff(ia) + Vdenom(ia))
                End If
            Next

            'then update hden estimates based on new vulnerable biomass estimates
            For ii = 1 To Me.m_SimData.inlinks
                j = Me.m_SimData.jlink(ii)
                ia = Me.m_SimData.ArenaLink(ii)
                Hdent(j) = Hdent(j) + aeff(ii) * Vbiom(ia)
            Next

            For j = 1 To Me.m_Data.NGroups
                Me.Hden(j) = (1 - Dwe) * (1 + Me.m_SimData.Htime(j) * Hdent(j)) + Dwe * Me.Hden(j)
            Next

            'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
            'then update vulnerable biomass estimates using new Hden estimates (THIS MAY NOT BE NECESSARY?)
            ReDim Vbiom(Me.m_SimData.Narena), Vdenom(Me.m_SimData.Narena)
            For ii = 1 To Me.m_SimData.inlinks
                i = Me.m_SimData.ilink(ii) : j = Me.m_SimData.jlink(ii) : ia = Me.m_SimData.ArenaLink(ii)
                Vdenom(ia) = Vdenom(ia) + aeff(ii) * Me.pred(j) / Me.Hden(j) / EatEff(j)
            Next
            For ia = 1 To Me.m_SimData.Narena
                i = Me.m_SimData.Iarena(ia)
                If Me.m_SimData.BoutFeeding Then
                    If Vdenom(ia) > 0 Then
                        Vbiom(ia) = Veff(ia) * Biomass(i) * (1 - CSng(Math.Exp(-Vdenom(ia)))) / Vdenom(ia)
                    Else
                        Vbiom(ia) = Veff(ia) * Biomass(i)
                    End If
                Else
                    Vbiom(ia) = Veff(ia) * Biomass(i) / (Me.m_Data.Vspace(ia) + Veff(ia) + Vdenom(ia))
                End If
            Next
            'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

            'then predict consumption flows and cumulative consumptions using the new Vbiom estimates
            For ii = 1 To Me.m_SimData.inlinks
                i = Me.m_SimData.ilink(ii) : j = Me.m_SimData.jlink(ii) : ia = Me.m_SimData.ArenaLink(ii)
                If Me.m_SimData.TrophicOff Then Bprey = Me.m_SimData.StartBiomass(i) Else Bprey = Biomass(i)

                Select Case Me.m_SimData.FlowType(i, j) 'prey always first
                    Case 1 'donor controlled flow
                        eat = aeff(ii) * Bprey
                    Case 3 'limited total flow
                        eat = aeff(ii) * Bprey * Me.pred(j) / (1 + aeff(ii) * Me.pred(j) * Bprey / Me.m_SimData.maxflow(i, j))
                    Case 2 'prey limited flow
                        eat = aeff(ii) * Vbiom(ia) * Me.pred(j) / Me.Hden(j)
                    Case Else
                        eat = 0
                End Select

                Me.Eatenof(i) = Me.Eatenof(i) + eat
                Me.Eatenby(j) = Me.Eatenby(j) + eat

                If Me.m_PathData.isEcospaceModelCoupled Then
                    'predation mort by link
                    Me.m_Data.MPred(iRow, iCol, ii) = eat / (Bprey + 1.0E-20F)
                End If

                If Me.m_Data.bCalTrophicLevel Or Me.m_Data.bENA Then Me.Consumpt(i, j) = Me.Consumpt(i, j) + eat

                'jb 
                If Me.m_TracerData.EcoSpaceConSimOn = True Then
                    If Biomass(i) > 0 Then
                        'Me.m_ConTracer.ConKtrophic(ii) = eat / Biomass(i)
                        Me.m_Data.ConKtrophic(iRow, iCol)(ii) = eat / Biomass(i)
                    Else
                        'Me.m_ConTracer.ConKtrophic(ii) = 0
                        Me.m_Data.ConKtrophic(iRow, iCol)(ii) = 0
                    End If
                End If

                ' JS 19Jan24: Region averages have been updated to correctly incorporate cell areas, and need correcting to the total region area
                ' JS 19Jan24: Region 0 holds total map average, no longer just the left-over region values
                If Me.m_Data.nRegions >= 1 Then
                    Dim iRgn As Integer = Me.m_Data.Region(iRow, iCol)
                    If (iRgn > 0 And iRgn <= Me.m_Data.nRegions) Then
                        Me.ResultsConsumptionRegionPredPrey(iRgn, j, i) += eat * Me.m_Data.CellArea(iRow, iCol)
                    End If
                End If
                Me.ResultsConsumptionRegionPredPrey(0, j, i) += eat * Me.m_Data.CellArea(iRow, iCol)

            Next ii

            Me.CalcTrophicLevel(iRow, iCol, Me.Consumpt, Me.Eatenby)

            'Make the detritus calculations here:
            Me.SimDetritusSpace(Biomass, Me.FishRateGear, Me.Eatenby, ToDetritus, Me.GroupDetritus, iRow, iCol)

            Dim moMult As Single = 1
            For i = 1 To Me.m_Data.NGroups

                Me.Eatenby(i) = Me.Eatenby(i) + Me.m_SimData.QBoutside(i) * Biomass(i)

                If i <= Me.m_Data.nLiving Then      'Living group
                    Pmult = 1.0#
                    Me.ApplyAVmodifiers(Pmult, Veff(1), i, i, False, iRow, iCol)
                    'pbb becomes pbmaxs= pb times a max increase factor = pbm for consumers
                    'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
                    'Changed 3-Mar-2017
                    'Carl Walters email "fixing nutrient effects on primary production in ecosim, and bug in modifying producers with forcing functions and mediation functions"
                    'There is a bad setup in derivt that couples nutrient response effects to the biomass shading effects; these need to vary independently. 
                    '1)      There is a line that calculates pbb(i):
                    'pbb(i) = m_Data.PBmaxs(i) * m_Data.NutFree / (m_Data.NutFree + m_Data.NutFreeBase(i)) * Pmult * m_Data.pbm(i) / (1 + Biomass(i) * m_Data.pbbiomass(i))
                    'change the term m_Data.PBmaxs(i) * m_Data.NutFree / (m_Data.NutFree + m_Data.NutFreeBase(i)) in this line to just
                    '2.0* m_Data.NutFree / (m_Data.NutFree + m_Data.NutFreeBase(i))
                    '(this allows primary production rate to as much as double as nutrient concentrations increase)
                    '2)      This necessitates a change in the calculation of NutFreeBase(i) in InitialState:

                    moMult = Me.getM0Mult(i, iRow, iCol)

                    Me.pbb(i) = 2 * EatEff(i) * Me.NutFree / (Me.NutFree + Me.m_SimData.NutFreeBase(i)) * Pmult * Me.m_SimData.pbm(i) / (1 + Biomass(i) * Me.PbSpace(i))
                    Me.loss(i) = Me.Eatenof(i) + (Me.m_SimData.mo(i) * moMult * (1 - Me.m_SimData.MoPred(i) + Me.m_SimData.MoPred(i) * Me.Ftime(i)) + Me.m_PathData.Emig(i) + Me.FishTime(i)) * Biomass(i)
                    Me.lossSpace(i)(iRow, iCol) = Me.loss(i)
                    'Debug.Assert(Single.IsNaN(Me.loss(i)) = False)

                    Me.SaveMOLoss(i, Biomass(i), moMult, iRow, iCol)

                    'on the use of variable GE CJW wrote to VC on 041210: just need to modify derivt to calculate GE for each time step
                    'from GE=0.6Z/(Z+3K*), where Z=loss/B, in the last loop over groups.  That calculation will automatically be overwritten
                    '(dB/dt from it is ignored anyway) for split groups, so not worth avoiding doing it for them.
                    If Me.m_SimData.UseVarPQ And Me.m_PathData.vbK(i) > 0 Then
                        SimGEt = Me.m_SimData.AssimEff(i) * Me.loss(i) / Biomass(i) / (Me.loss(i) / Biomass(i) + 3 * Me.m_PathData.vbK(i))
                    Else
                        SimGEt = Me.m_SimData.SimGE(i)
                    End If

                    Flowin(i) = Me.m_PathData.Immig(i) + SimGEt * Me.Eatenby(i) + Me.pbb(i) * Biomass(i)

                    If Biomass(i) > 1.0E-20 Then
                        FlowoutRate(i) = Me.loss(i) / Biomass(i)
                    Else
                        FlowoutRate(i) = 100
                    End If

                Else 'i <= m_Data.nLiving
                    'Detritus(group)

                    Me.loss(i) = Me.Eatenof(i) + Me.m_PathData.Emig(i) + Me.m_SimData.DetritusOut(i) * Biomass(i)
                    Me.lossSpace(i)(iRow, iCol) = Me.Eatenof(i) + Me.m_PathData.Emig(i) + Me.m_SimData.DetritusOut(i) * Biomass(i)
                    'deriv(i) = Immig(i) + ToDetritus(i - n) - loss(i)
                    If Me.loss(i) <> 0 And Biomass(i) > 0 Then
                        'biomeq(i) = (Immig(i) + ToDetritus(i - n)) / (loss(i) / Biomass(i))
                        DtImpMult = 1
                        Me.ApplyAVmodifiers(DtImpMult, 0, i, i, True, iRow, iCol)

                        Flowin(i) = (Me.m_PathData.Immig(i) * DtImpMult + ToDetritus(i - Me.m_Data.nLiving))
                        FlowoutRate(i) = Me.loss(i) / Biomass(i)
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

            '  System.Console.WriteLine()
            If Me.m_Data.bENA Then
                Me.ENAData(iRow, iCol, Biomass, Flowin, Me.Consumpt, Me.FishTime, ToDetritus, Me.GroupDetritus)
            End If

        Catch ex As Exception
            Throw New ApplicationException(Me.ToString & ".derivtRed() Error: " & ex.Message)
        End Try
    End Sub

    '***********************
    'THIS FUNCTION IS COPIED FROM cEcoSimModel.vb
    'Changes here will NOT copy over to there
    '***********************
    ''' <summary>
    ''' Sets RelaSwitch() 
    ''' </summary>
    ''' <param name="B">Biomass at this time step for this spatial cell</param>
    ''' <remarks>Sets RelaSwitch() using local B() and  Ecosim.A(), Ecosim.SwitchPower(), Ecosim.BaseTimeSwitch()  </remarks>
    Sub SetRelaSwitch(ByVal B() As Single)     'Switching
        Dim i As Integer, j As Integer, ii As Integer
        Dim PredDen() As Double
        ReDim PredDen(Me.m_Data.NGroups)

        If Me.RelaSwitch Is Nothing Then
            ReDim Me.RelaSwitch(Me.m_SimData.inlinks)
        Else
            Array.Clear(Me.RelaSwitch, 0, Me.m_SimData.inlinks + 1)
        End If

        For ii = 1 To Me.m_SimData.inlinks
            i = Me.m_SimData.ilink(ii) : j = Me.m_SimData.jlink(ii)
            PredDen(j) = PredDen(j) + Me.m_Ecosim.A(i, j) * B(i) ^ Me.m_SimData.SwitchPower(j)
        Next
        For ii = 1 To Me.m_SimData.inlinks
            i = Me.m_SimData.ilink(ii) : j = Me.m_SimData.jlink(ii)
            If Me.m_SimData.SwitchPower(j) = 0.0# Then
                Me.RelaSwitch(ii) = 1
            Else
                Me.RelaSwitch(ii) = CSng(Me.m_Ecosim.A(i, j) * B(i) ^ Me.m_SimData.SwitchPower(j) / (PredDen(j) + 1.0E-20) / Me.m_SimData.BaseTimeSwitch(ii))
            End If
        Next

    End Sub
    '***********************
    'THIS FUNCTION IS COPIED FROM cEcoSimModel.vb
    'Changes here will NOT copy over to there
    '***********************
    Sub setpred(ByVal Biomass() As Single)
        'Routine modified 290597 VC to follow ESimII
        Dim i As Integer ', ii As Integer
        'set predator abundance measure used for predation
        'rate calculations; this is just biomass for
        'simple pools, or predator numbers for pools that
        'are split into Juv-Adult pairs
        'note below that biomass(ii) for ii>n contains
        'numbers in pools iad, iju rather than biomasses
        For i = 1 To Me.m_Data.NGroups
            'If i > N And biomass(i) = 0 Then biomass(i) = 1
            If Biomass(i) < 1.0E-20 Or Single.IsNaN(Biomass(i)) Then Biomass(i) = 1.0E-20 '0.00000001
            If Me.m_SimData.NoIntegrate(i) >= 0 Then Me.pred(i) = Biomass(i)
            If Single.IsNaN(Me.pred(i)) Then Me.pred(i) = 1.0E-20
        Next

    End Sub

    Private Function getM0Mult(igrp As Integer, irow As Integer, icol As Integer) As Single
        Dim MoMult As Single

        Try
            If Me.m_SimData.mo(igrp) = 0.0F Then
                Return 1.0F
            End If

            'MOProp(igrp)(irow, icol) Proportion of the total population that will experience mortality in this cell/time-step
            'This is the value calcualted by the response function for this cell/time-step
            Dim propMort As Single = Me.m_Data.MOProp(igrp)(irow, icol)

            If propMort = 0.0 Then Return 1.0
            'cap it at just below 1.0
            If propMort >= 1.0 Then propMort = 0.9999F

            'instantaneous motality in Ecopath annual units, this should be the same as Ecopath mo units
            Dim lnPropMort As Single = CSng(-Math.Log(1 - propMort)) * 12
            'Create a scalar value that will scale mo too the new mortality rate
            MoMult = ((lnPropMort + Me.m_SimData.mo(igrp)) / Me.m_SimData.mo(igrp))

        Catch ex As Exception
            MoMult = 1.0F
        End Try

        Return MoMult

    End Function

    ' SaveRelNutFree

    Private Sub SaveRelNutFree(NutFree As Single, irow As Integer, icol As Integer)

        If Me.m_Data.bSaveRelNutFile Then
            Me.m_Data.RelNutMult(irow, icol) += 2 * NutFree / (NutFree + Me.m_SimData.NutFreeBase(1))
        End If

    End Sub

    Private Sub SaveMOLoss(igrp As Integer, biomass As Single, MOMult As Single, irow As Integer, icol As Integer)

        Dim propMort As Single = Me.m_Data.MOProp(igrp)(irow, icol)
        'cap it at just below 1.0
        If propMort >= 1.0 Then propMort = 0.9999F
        Dim lnPropMort As Single = CSng(-Math.Log(1 - propMort))
        Me.m_moLoss(igrp) = lnPropMort * biomass

        ''xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
        ''For debugging
        'If propMort > 0.5 And igrp = 2 Then
        '    Dim moTot As Single = (m_SimData.mo(igrp) * MOMult * (1 - m_SimData.MoPred(igrp) + m_SimData.MoPred(igrp) * Ftime(igrp))) * biomass
        '    Dim noMoForce As Single = (m_SimData.mo(igrp) * (1 - m_SimData.MoPred(igrp) + m_SimData.MoPred(igrp) * Ftime(igrp))) * biomass
        '    ' System.Console.WriteLine(m_moLoss(igrp).ToString + ", " + (moTot - noMoForce).ToString + ", " + (m_moLoss(igrp) / (moTot - noMoForce)).ToString)
        'End If
        ''xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

    End Sub

    '***********************
    'THIS FUNCTION IS COPIED FROM cEcoSimModel.vb
    'Changes here will NOT copy over to there
    '***********************
    ''' <summary>
    ''' Sets MedVal() mediation value used to modify a or v. Local version for thread safety.
    ''' </summary>
    ''' <param name="Biom"></param>
    ''' <remarks>MedVal(nmediationshapes) is used in ApplyAVmodifiers()</remarks>
    Sub SetMedFunctions(ByVal Biom() As Single)
        'called from derivt, derivtred if MedIsUsed(0)=true to set
        'current Y value of each active trophic mediation function
        If Me.m_SimData.BioMedData.MedIsUsed(0) Then

            'jb Feb-2024 When the mediation value was set on the Ecopath biomass 
            'that biomass is the average across the map.
            'In Ecospace the biomass can be concentrated 
            'So to get the original averaged mediation affect
            'we need to scale the biomass's back to the original Ecopath averaged values

            'Calculate B/BRatio
            For i As Integer = 1 To Me.m_Data.NGroups
                Me.BBRatio(i) = Biom(i) / (Me.m_Data.BRatio(i) + 1.0E-20F)
            Next

            'Calculate MedVal() from the B/BRatio
            Me.m_SimData.BioMedData.SetMedFunctions(Me.BBRatio, Me.FishRateGear, 0, Me.MedVal)
        End If

    End Sub

    '***********************
    'THIS FUNCTION IS COPIED from cEcosimModel.vb
    'Changes here will NOT be copy over to there
    '***********************
    ''' <summary>
    ''' Apply the multi function mediation functions/modifiers to 'a'(searchrate) and 'v'(vulnerability)
    ''' uses MedVal(NMediationShapes) to modify A and/or V
    ''' </summary>
    ''' <param name="A">SearchRate to modify</param>
    ''' <param name="v">Vulnerability to modify</param>
    ''' <param name="i">i Index (Prey)</param>
    ''' <param name="j">j Index (Pred)</param>
    ''' <param name="UseTime">True if the modifier is over time (Ecosim), False if not (Ecospace) </param>
    ''' <remarks>
    ''' THREADING:  MedVal() is set to the mediating value based on biomass for each map cell at each time step via cSpaceSolver.SetMedFunctions().
    ''' It is unique to this thread/cell/time-step. It was moved here to make it thread safe.
    '''</remarks>
    Sub ApplyAVmodifiers(ByRef A As Single, ByRef v As Single, ByVal i As Integer, ByVal j As Integer, ByVal UseTime As Boolean, ByVal iRow As Integer, ByVal iCol As Integer)
        Dim K As Integer, Mult As Single

        For K = 1 To cMediationDataStructures.MAXFUNCTIONS

            If Me.m_SimData.BioMedData.FunctionNumber(i, j, K) = 0 Then Exit Sub

            If Me.m_SimData.BioMedData.IsMedFunction(i, j, K) Then
                Mult = Me.MedVal(Me.m_SimData.BioMedData.FunctionNumber(i, j, K))
            Else
                Mult = 1
                'If UseTime = True Then Mult = m_ESData.tval(m_ESData.FunctionNumber(i, j, K)) Else Mult = 1
            End If
            'Debug.Assert(Mult = 1)

            Select Case Me.m_SimData.BioMedData.ApplicationType(i, j, K)
                'SearchRate, Production and ImportedDetritus are all applied to the A multiplier
                Case eForcingFunctionApplication.SearchRate,
                     eForcingFunctionApplication.ProductionRate
                    A = A * Mult
                Case eForcingFunctionApplication.Vulnerability
                    v = v * Mult
                Case eForcingFunctionApplication.ArenaArea
                    A = A / (Mult + 1.0E-10F)
                Case eForcingFunctionApplication.VulAndArea
                    A = A / (Mult + 1.0E-10F)
                    v = v * Mult
                Case eForcingFunctionApplication.Import
                    A = A * Mult
            End Select

        Next

    End Sub

    ''' <summary>
    ''' Accumulate the fisheries data (catch) for a single group for this map cell. 
    ''' This is called before DerivtRed(), in the time step, so it is the condition at the start of the time step.
    ''' </summary>
    ''' <param name="Biomass">Biomass for all the groups at this time step</param>
    ''' <param name="iRow">Map row</param>
    ''' <param name="iCol">Map col</param>
    ''' <remarks></remarks>
    Public Sub accumCatchData(ByVal iCumTime As Integer, ByVal iYear As Integer, ByVal Biomass() As Single, ByVal FMortByGroup() As Single, ByVal iRow As Integer, ByVal iCol As Integer)
        Dim cellCatch As Single
        Dim cellLandings As Single
        Dim cellValue As Single
        Dim cellDiscards As Single
        Dim iFlt As Integer
        Dim iGrp As Integer
        Dim st As Double = Me.m_stpWatch.Elapsed.TotalSeconds

        Try

            For iFlt = 1 To Me.m_Data.nFleets
                'Effort
                Me.ResultsByFleet(eSpaceResultsFleets.FishingEffort, iFlt) += Me.m_Data.EffortSpace(iFlt, iRow, iCol)
                'SailingEffort: at this point SailingEffort is  sum of [fishing effort] * [effort of fishing each cell (Sail(iFlt, iRow, iCol))] /  SailScale(ifleet)
                'Effort of fishing all the cells
                Me.ResultsByFleet(eSpaceResultsFleets.SailingEffort, iFlt) += (Me.m_Data.EffortSpace(iFlt, iRow, iCol) * Me.m_Data.Sail(iFlt)(iRow, iCol) / Me.m_Data.SailScale(iFlt))

                'sum values into All Fleets 0 index 
                Me.ResultsByFleet(eSpaceResultsFleets.FishingEffort, 0) += Me.ResultsByFleet(eSpaceResultsFleets.FishingEffort, iFlt)
                Me.ResultsByFleet(eSpaceResultsFleets.SailingEffort, 0) += Me.ResultsByFleet(eSpaceResultsFleets.SailingEffort, iFlt)

            Next

            For iGrp = 1 To Me.m_Data.NGroups
                If Me.m_PathData.fCatch(iGrp) > 0 Then
                    'jb 29-Jan-12 in the multithreaded version FishTime was not updated to the F for this cell
                    'use fishing mortality rate passed in instead 
                    'Dim bCatch As Single = Biomass(igrp) * m_SimData.FishTime(igrp) 
                    Dim bCatch As Single = Biomass(iGrp) * FMortByGroup(iGrp)
                    Me.ResultsByGroup(eSpaceResultsGroups.CatchBio, iGrp) += bCatch
                    Me.m_Data.CatchMap(iRow, iCol, iGrp) += bCatch
                    'Next value of catch, depends on what gear was used:
                    For iFlt = 1 To Me.m_PathData.NumFleet
                        ' BUas Marin Hekman found that catch data was incorrectly aggregated here. Good catch.
                        If m_Data.IsFished(iFlt, iRow, iCol) Then
                            If Me.m_PathData.Landing(iFlt, iGrp) + Me.m_PathData.Discard(iFlt, iGrp) > 0 Then
                                'First get catch
                                cellCatch = Biomass(iGrp) * Me.m_Data.EffortSpace(iFlt, iRow, iCol) * Me.m_SimData.relQ(iFlt, iGrp) * (Me.m_SimData.PropLandedTime(iFlt, iGrp) + Me.m_SimData.PropDiscardTime(iFlt, iGrp))
                                Me.m_Data.CatchFleetMap(iRow, iCol, iFlt) += cellCatch
                                'Sum the total catch by gear
                                Me.ResultsByFleet(eSpaceResultsFleets.CatchBio, iFlt) += cellCatch
                                'sum all fleets
                                Me.ResultsByFleet(eSpaceResultsFleets.CatchBio, 0) += cellCatch

                                Me.ResultsByFleetGroup(eSpaceResultsFleetsGroups.CatchBio, iFlt, iGrp) += cellCatch
                                'sum all fleets into the zero fleet index
                                Me.ResultsByFleetGroup(eSpaceResultsFleetsGroups.CatchBio, 0, iGrp) += cellCatch

                                cellLandings = Biomass(iGrp) * Me.m_Data.EffortSpace(iFlt, iRow, iCol) * Me.m_SimData.relQ(iFlt, iGrp) * Me.m_SimData.PropLandedTime(iFlt, iGrp)
                                Me.Landings(iGrp, iFlt) += cellLandings

                                cellValue = cellLandings * Me.m_Ecosim.MarketValue(iGrp, iFlt, iCumTime, iYear)

                                'Next line is for adding up catch, landings and value by region

                                ' JS 19Jan24: Region averages have been updated to correctly incorporate cell areas, and need correcting to the total region area
                                ' JS 19Jan24: Region 0 holds total map average, no longer just the left-over region values
                                If Me.m_Data.nRegions >= 1 Then
                                    Dim iRgn As Integer = Me.m_Data.Region(iRow, iCol)
                                    If (iRgn > 0 And iRgn <= Me.m_Data.nRegions) Then
                                        Me.ResultsCatchRegionGearGroup(iRgn, iFlt, iGrp) += cellCatch * Me.m_Data.CellArea(iRow, iCol)
                                        Me.ResultsLandingsRegionGearGroup(iRgn, iFlt, iGrp) += cellLandings * Me.m_Data.CellArea(iRow, iCol)
                                        Me.ResultsValueRegionGearGroup(iRgn, iFlt, iGrp) += cellValue * Me.m_Data.CellArea(iRow, iCol)
                                    End If
                                End If
                                Me.ResultsCatchRegionGearGroup(0, iFlt, iGrp) += cellCatch * Me.m_Data.CellArea(iRow, iCol)
                                Me.ResultsLandingsRegionGearGroup(0, iFlt, iGrp) += cellLandings * Me.m_Data.CellArea(iRow, iCol)
                                Me.ResultsValueRegionGearGroup(0, iFlt, iGrp) += cellValue * Me.m_Data.CellArea(iRow, iCol)

                                'Discards map used by the Biodiversity plugin
                                'Include discards that survived
                                cellDiscards = Biomass(iGrp) * Me.m_Data.EffortSpace(iFlt, iRow, iCol) * Me.m_SimData.relQ(iFlt, iGrp) * (1 - Me.m_SimData.PropLandedTime(iFlt, iGrp))
                                Me.m_Data.DiscardsMap(iRow, iCol, iGrp) += cellDiscards

                                If (Me.m_Data.CatchGroupFleetMap(iFlt, iGrp) Is Nothing) Then
                                    Dim map(Me.m_Data.InRow, Me.m_Data.InCol) As Single
                                    Me.m_Data.CatchGroupFleetMap(iFlt, iGrp) = map
                                End If
                                Me.m_Data.CatchGroupFleetMap(iFlt, iGrp)(iRow, iCol) += cellCatch

                                If (Me.m_Data.DiscardMortGroupFleetMap(iFlt, iGrp) Is Nothing) Then
                                    Dim map(Me.m_Data.InRow, Me.m_Data.InCol) As Single
                                    Me.m_Data.DiscardMortGroupFleetMap(iFlt, iGrp) = map
                                End If
                                Me.m_Data.DiscardMortGroupFleetMap(iFlt, iGrp)(iRow, iCol) += cellDiscards * Me.m_SimData.PropDiscardMortTime(iFlt, iGrp)

                                If (Me.m_Data.DiscardSurviveGroupFleetMap(iFlt, iGrp) Is Nothing) Then
                                    Dim map(Me.m_Data.InRow, Me.m_Data.InCol) As Single
                                    Me.m_Data.DiscardSurviveGroupFleetMap(iFlt, iGrp) = map
                                End If
                                Me.m_Data.DiscardSurviveGroupFleetMap(iFlt, iGrp)(iRow, iCol) += cellDiscards * (1 - Me.m_SimData.PropDiscardMortTime(iFlt, iGrp))

                            End If
                        End If
                    Next iFlt
                End If 'If m_EPdata.fCatch(igrp) > 0 Then

            Next iGrp

        Catch ex As Exception
            m_logger.LogError(".accumCatchData() Error: " & ex.Message)
        End Try

        Me.CatchCPUTimeSec += Me.m_stpWatch.Elapsed.TotalSeconds - st
    End Sub

    Public Sub New(ByVal ThreadNumber As Integer)

        Me.ThreadID = ThreadNumber

        Me.m_ConTracer = New cContaminantTracer
        'create a new tracer data structure
        'this will get a copy of the data that has been initialized by the database in cEcospace.InitSpaceSolverThreads()
        Me.m_TracerData = New cContaminantTracerDataStructures

    End Sub

    ''' <summary>
    ''' Calculate trophic level and populate the trophic level map in the Ecospace data structures <see cref="cEcospaceDataStructures.TL">TL</see> map array.
    ''' </summary>
    ''' <param name="iRow">Row index </param>
    ''' <param name="iCol">Column index</param>
    ''' <param name="Consumpt">Consumption by consumer,prey  </param>
    ''' <param name="EatenBy">Total biomass consumed by a group </param>
    ''' <remarks>Trophic level is only calculated if <see cref="cEcospaceDataStructures.bCalTrophicLevel">bCalTrophicLevel</see> = True</remarks>
    Private Sub CalcTrophicLevel(iRow As Integer, iCol As Integer, Consumpt(,) As Single, EatenBy() As Single)
        Dim i As Integer
        Dim j As Integer
        Dim SumDiet As Single

        If Not Me.m_Data.bCalTrophicLevel Or Me.m_Data.bInSpinUp Then
            'Turned off or spinning up
            Return
        End If

        Debug.Assert(Me.EcoFunctions IsNot Nothing, "Space cannot run CalcTrophicLevel() because EcoFunctions has not been initialized properly.")
        Debug.Assert(Me.m_Data.TL IsNot Nothing, "Space cannot run CalcTrophicLevel() because cEcospaceDataStructures.TL(row,col,group) has not been initialized properly.")
        Debug.Assert(Me.TLlockOb IsNot Nothing, "Space cannot run CalcTrophicLevel() because TLlockOb has not been initialized properly.")

        Dim Diet(Me.m_Data.NGroups, Me.m_Data.NGroups) As Single
        Dim TLs(Me.m_Data.NGroups) As Single
        Dim totalTL As Single = 0
        Dim fCatch As Single = 0

        Try

            For i = 1 To Me.m_Data.nLiving  'consumer
                If EatenBy(i) > 0 Then
                    SumDiet = 0
                    For j = 1 To Me.m_Data.NGroups  'food
                        Diet(i, j) = Consumpt(j, i) / EatenBy(i)
                        SumDiet = SumDiet + Diet(i, j)
                    Next j
                    If SumDiet > 0 Then
                        For j = 1 To Me.m_Data.NGroups  'food
                            Diet(i, j) = Diet(i, j) / SumDiet
                        Next j
                    End If
                End If
            Next i

            SyncLock Me.TLlockOb
                Me.EcoFunctions.EstimateTrophicLevels(Me.m_Data.NGroups, Me.m_Data.nLiving, Me.m_PathData.PP, Diet, TLs)
            End SyncLock 'TLlockOb

            'populate the map for this row col
            For igrp As Integer = 1 To Me.m_Data.NGroups
                Me.m_Data.TL(iRow, iCol, igrp) = TLs(igrp)
                fCatch += Me.m_Data.CatchMap(iRow, iCol, igrp)
                totalTL += TLs(igrp) * Me.m_Data.CatchMap(iRow, iCol, igrp)
            Next

            If (fCatch > 0) Then
                Me.m_Data.TLc(iRow, iCol) = CSng(totalTL / (fCatch + 1.0E-20))
            Else
                Me.m_Data.TLc(iRow, iCol) = 0
            End If

            If (Me.m_Data.TimeStep = 0) Then

                SyncLock Me.TLlockOb
                    Me.m_Data.KemptonsQ(iRow, iCol) = Me.EcoFunctions.KemptonsQ(Me.m_PathData.NumLiving, Me.m_PathData.TTLX, Me.m_Data.BBase, 0.25)
                    Me.m_Data.ShannonDiversity(iRow, iCol) = Me.EcoFunctions.ShannonDiversityIndex(Me.m_PathData.NumLiving, Me.m_Data.BBase)
                End SyncLock 'TLlockO

            Else
                ' Recompose Biomass array
                Dim BSpace(Me.m_PathData.NumGroups) As Single

                For iGroup As Integer = 1 To Me.m_PathData.NumGroups
                    BSpace(iGroup) = Me.m_Data.Bcell(iRow, iCol, iGroup)
                Next

                SyncLock Me.TLlockOb
                    Me.m_Data.KemptonsQ(iRow, iCol) = Me.EcoFunctions.KemptonsQ(Me.m_PathData.NumLiving, TLs, BSpace, 0.25)
                    Me.m_Data.ShannonDiversity(iRow, iCol) = Me.EcoFunctions.ShannonDiversityIndex(Me.m_PathData.NumLiving, BSpace)
                End SyncLock 'TLlockOb

            End If

        Catch ex As Exception
            m_logger.LogError(".CalcTrophicLevel() Error: " & ex.Message)
            Debug.Assert(False, Me.ToString & ".CalcTrophicLevel() Error: " & ex.Message)
        End Try

    End Sub

    Public Sub SimDetritusSpace(ByVal Biomass() As Single, ByVal FishRateGear(,) As Single, ByVal Eatenby() As Single, ByRef ToDetritus() As Single, ByRef DetritusByGroup() As Single, irow As Integer, icol As Integer)
        Dim i As Integer, iFleet As Integer
        Dim ToDet As Single, DetFlowN As Single = 0
        Dim jDet As Integer ', j As Integer
        Dim moMult As Single
        Dim nDet As Integer = Me.m_Data.NGroups - Me.m_PathData.NumLiving

        'DetritusByGroup() needs to be cleared because the values are summed into it
        Array.Clear(DetritusByGroup, 0, Me.m_Data.NGroups)

        For i = 1 To Me.m_PathData.NumLiving
            For jDet = 1 To nDet
                'First take egestion
                ToDet = Me.m_PathData.GS(i) * Eatenby(i) * Me.m_PathData.DF(i, jDet)

                moMult = Me.getM0Mult(i, irow, icol)
                'Add dying organisms
                ToDet = ToDet + Me.m_SimData.mo(i) * moMult * Biomass(i) * Me.m_PathData.DF(i, jDet)

                For iFleet = 1 To Me.m_PathData.NumFleet
                    Dim PropDiscMort As Single = Me.m_SimData.PropDiscardTime(iFleet, i) / (Me.m_SimData.PropLandedTime(iFleet, i) + Me.m_SimData.PropDiscardTime(iFleet, i) + 1.0E-20F)
                    'jb 07-Jan-2010 Changed to use Propdiscardtime(fleets,groups) (% discarded for this time step) initialized to ecopath PropDiscard() or set in MSE.RegulateEffort() 
                    'discard mort is included in Propdiscardtime() by initialization and MSE 
                    DetFlowN = Me.m_PathData.DiscardFate(iFleet, jDet) * Biomass(i) * FishRateGear(iFleet, 0) * Me.m_SimData.FishMGear(iFleet, i) * PropDiscMort 'Me.m_SimData.Propdiscardtime(K, i)
                    ToDet = ToDet + DetFlowN

                    If Me.m_TracerData.EcoSpaceConSimOn = True Then
                        Me.m_Data.ConKdetSpace(irow, icol)(i, jDet, iFleet) = DetFlowN / Biomass(i)
                    End If

                    Me.TotFisheriesDiscards += DetFlowN

                Next iFleet

                ToDetritus(jDet) = ToDetritus(jDet) + ToDet
                DetritusByGroup(i) += ToDet

            Next jDet
        Next i

        'Next add flow from other detritus groups
        For i = 1 To nDet
            For jDet = 1 To nDet ' (Me.m_Data.NGroups - Me.m_PathData.NumLiving)
                If i <> jDet Then
                    ToDetritus(jDet) = ToDetritus(jDet) + Me.m_PathData.DetPassedProp(i) * Biomass(i) * Me.m_PathData.DF(i, jDet)
                End If
            Next
        Next

        Me.m_SimData.FirstTime = False

    End Sub
    'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

    Private Sub ENAData(Ir As Integer, ic As Integer, ByVal Biomass() As Single, Production() As Single, consumpt(,) As Single, FishingMort() As Single, FlowToDertitus() As Single, DetritusFlowByGroup() As Single)
        'Lookup this cell in the dictionary of cells by its Row,Col haskey
        Dim d As cENAData = Me.m_Data.m_enaCellData(cENAData.GetHash(Ir, ic))
        'Populate its data with values from this cell at this time  step
        d.Populate(Me.m_Data, Biomass, Production, consumpt, FishingMort, Me.Eatenof, FlowToDertitus, DetritusFlowByGroup, Me.TotFisheriesDiscards)
    End Sub

    Private Sub setRelFitnessBase(i As Integer, j As Integer)
        Dim relFit As Single

        If Me.itt < 13 And (Me.m_bFitnessSet(i, j) = False) Then

            Dim minMovement As Single = 1 / cEcoSpace.MAX_FITNESSMOVEMENT
            For iGrp As Integer = 1 To Me.m_Data.NGroups

                If Me.itt = 1 Then Me.m_Data.RelFitnessBase(i, j, iGrp) = 1.0

                If (Me.m_Data.Kmovefit(iGrp) > 0) Then

                    Dim FCatch As Single = Me.FishTime(iGrp) * Me.BB(iGrp)
                    relFit = (Me.m_SimData.SimGE(iGrp) * Me.Eatenby(iGrp)) / (Me.loss(iGrp) - FCatch + 1.0E-10F)
                    'If i = 1 And j = 1 And iGrp = 1 Then
                    '    Debug.WriteLine("Base " + relFit.ToString + "," + m_SimData.SimGE(iGrp).ToString + "," + Eatenby(iGrp).ToString + "," + loss(iGrp).ToString + "," + FCatch.ToString)
                    'End If
                    If Single.IsNaN(relFit) Then relFit = 1.0
                    Me.m_Data.RelFitnessBase(i, j, iGrp) += relFit
                End If '

            Next iGrp

            If Me.itt = 12 Then
                'Last time step of the year 
                'average the RelFitnessBase()
                For iGrp As Integer = 1 To Me.m_Data.NGroups
                    If (Me.m_Data.Kmovefit(iGrp) > 0) Then
                        Me.m_Data.RelFitnessBase(i, j, iGrp) /= 12.0F
                    End If '
                    Me.m_bFitnessSet(i, j) = True
                Next iGrp

            End If 'Me.itt > 5 And (Me.m_bFitnessSet(i, j) = False)

        End If 'Me.itt < 13 

    End Sub

End Class

#Region "New Local Memory"

#If NOTUSED Then

Public Class cSpaceSolver_LocalMemory

#Region "Variables"

    ''' <summary>
    ''' Wait handle 
    ''' </summary>
    ''' <remarks>
    ''' When the Solve() thread is running (SignalState in a non-signaled state SignalState.Reset()) 
    ''' calls to SignalState.WaitOne() will block until Solve() has completed (SignalState in a signaled state SignalState.Set())
    ''' </remarks>
    Public WaitHandle As ManualResetEvent
    Public Shared ThreadIncrementer As Integer

    Public SolveCPUTimeSec As Double
    Public lstCellCompTimes As New List(Of Double)

    Private m_ConTracer As cContaminantTracer

    Public iYear As Integer ' current year

    ' ''' <summary>
    ' ''' Delegate for posting error messages.
    ' ''' </summary>
    ' ''' <remarks>
    ' ''' All error handling must be done on the same thread. Errors can not be thrown from one thread to another.
    ' ''' A delegate must be used to cross the thread boundary. EcospaceErrorHandler is a delegate to a sub on the main Ecospace thread.
    ' ''' </remarks>
    'Public EcospaceErrorHandler As cEcoSpace.SolverErrorDelegate

    Public ThreadID As Integer

    'references
    Public m_EcospaceModel As cEcoSpace
    Public m_Data As cEcospaceDataStructures
    Public m_SimData As cEcosimDatastructures
    Public m_PathData As cEcopathDataStructures
    Public m_Stanza As cStanzaDatastructures
    Public m_Ecosim As Ecosim.cEcoSimModel
    Public m_TracerData As cContaminantTracerDataStructures

    Public Search As cSearchDatastructures

    Public Bcw(,,) As Single
    Public C(,,) As Single
    Public d(,,) As Single
    Public e(,,) As Single
    Public BEQLast(,,) As Single
    ' Public WchangeVar(,,) As Single
    Public Btime() As Single
    Public F(,,) As Single
    Public AMm(,,) As Single
    Public Ecode() As Integer
    Public HdenCell(,,) As Single
    Public RelFitness(,,) As Single
    Public FtimeCell(,,) As Single
    Public Cper(,,) As Single
    Public PconSplit() As Single
    Public RelRepStanza() As Single
    Public Tstanza() As Single
    Public PbSpace() As Single

    'needs to be set from ecospace, but not references
    'Public Tn As Integer
    Public nvar2 As Integer
    Public itt As Integer
    Public PPScale As Double
    Public TimeStep2 As Single
    Public MinChange As Single

    ' Public syncCopyLock As Object

    'locals
    'Private ebb() As Single
    Private BB() As Single
    Private loss() As Single
    Private RelPPupwell As Single
    Private RelR As Single, RelRS As Single, Rflow As Single
    Private Flowin() As Single
    Private FlowoutRate() As Single
    Private ieco As Integer
    Private isc As Integer
    Private isp As Integer
    Private ist As Integer
    Private EatEff() As Single
    Private VulPred() As Single
    Private ig As Integer
    Private pbb() As Single

    'These are total sums for every cell, so must be summed for each thread seperately, then combined after they've all run
    'Public BtimeLocal() As Single
    Public TotLossThread() As Single
    Public TotEatenByThread() As Single
    Public TotBiomThread() As Single
    Public TotPredThread() As Single
    Public TotIFDweightThread() As Single

    ''' <summary>
    ''' Sum of Effort, Sailing Effort, Catch and Value across cells by fleet
    ''' </summary>
    ''' <remarks></remarks>
    Public ResultsByFleet(,) As Single

    ''' <summary>
    ''' Sum of Biomass, Relative Biomass and Catch by group across cells by group
    ''' </summary>
    ''' <remarks></remarks>
    Public ResultsByGroup(,) As Single

    ''' <summary>
    ''' Sum of Catch and Value across cells by fleet group
    ''' </summary>
    ''' <remarks></remarks>
    Public ResultsByFleetGroup(,,) As Single
    Public ResultsCatchRegionGearGroup(,,) As Single

    ''' <summary>
    ''' Sum of Landings across cells by group fleet
    ''' </summary>
    ''' <remarks></remarks>
    Public Landings(,) As Single

    ''' <summary>
    ''' First map cell in a linear list of cells to compute.
    ''' </summary>
    ''' <remarks></remarks>
    Public iFrstCell As Integer

    ''' <summary>
    ''' Last map cell in a linear list of cells to compute.
    ''' </summary>
    ''' <remarks></remarks>
    Public iLstCell As Integer

    Public CatchCPUTimeSec As Double

    Public bUseLocalMemory As Boolean

    'variables from m_ESData, used locally
    Private Hden() As Single
    Private Ftime() As Single
    Private Fish1() As Single
    Private FishTime() As Single

    ''' <summary>
    ''' Fishing Effort multiplier by Fleet,time = 0
    ''' </summary>
    ''' <remarks>
    ''' This array has the same signature as cEcosimDataStructures.FishRateGear(fleet,time)
    ''' but never uses the time index.
    ''' This is so EcoSpace can share the same functions as Ecosim but never have to worry about the different time step indexes.
    ''' Ecospace populates the zero time index with the effort multiplier for the current timestep at the start of each timestep.
    '''  </remarks>
    Private EffortSpace(,) As Single
    Private pred() As Single
    Private Eatenof() As Single
    Private Eatenby() As Single
    Private RelaSwitch() As Single
    Private NutBiom As Single
    Private NutFree As Single
    Private MedVal() As Single

    'Contaminant tracing used locally
    Dim Derivcon() As Single, Cintotal() As Single, Closs() As Single, ConCtot As Single

    Private RtoNext As Single
    Private SurvRat As Single

    ''' <summary>Detritus by Group</summary>
    ''' <remarks>Added for Atlantis coupling. Local copy passes to SimDetritusMT() by each thread(this prevents cross thread corruption) then used to update map </remarks>
    Private GroupDetritus() As Single

    Private m_stpWatch As Stopwatch
    Private m_stpInit As Stopwatch

    Private BBRatio() As Single

    Private inLinks As Integer
    Private ilink() As Integer
    Private jlink() As Integer
    Private ArenaLink() As Integer

    Private Bio(,) As Single
    Private AMlin(,) As Single
    Private Flin(,) As Single

    Private SimGE() As Single
    Private FtimeMax() As Single
    Private Cbase() As Single
    Private RelVulBad() As Single

    Private A(,) As Single
    Private SwitchPower() As Single
    Private BaseTimeSwitch() As Single

    'Me.m_Data.IsFished(iFlt, iRow, iCol)
    'Private isFished(,,) As Boolean
    ' Private isCaught(,) As Boolean

#End Region

#Region "Construction Initialization"

    ''' <summary>
    ''' First time initialization run after construction. This gets run on the cores threads.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub Init()

        'local spatial variables
        Me.AllocateLocalData()
        Me.initLocalDataForRun()

        Dim nGroups As Integer = Me.m_Data.NGroups
        Dim nFleets As Integer = Me.m_Data.nFleets

        ResultsByGroup = New Single(cEcospaceDataStructures.N_RESULTS_GROUPS, nGroups) {}
        ResultsByFleet = New Single(cEcospaceDataStructures.N_RESULTS_FLEETS, nFleets) {}
        ResultsByFleetGroup = New Single(cEcospaceDataStructures.N_RESULTS_FLEETGROUPS, nFleets, nGroups) {}
        Landings = New Single(nGroups, nFleets) {}
        ResultsCatchRegionGearGroup = New Single(m_Data.nRegions, nFleets, nGroups) {}

        m_ConTracer.Init(m_TracerData, m_PathData, m_SimData, m_Stanza)
        m_ConTracer.CInitialize()

        Me.m_stpWatch = New Stopwatch
        Me.m_stpInit = New Stopwatch
    End Sub

    ''' <summary>
    ''' Update the core data arrays from the local arrays populated during the time step.
    ''' </summary>
    ''' <remarks>Only updates the data if <see cref="bUseLocalMemory"> bUseLocalMemory = True</see> </remarks>
    Private Sub UpdateCoreData()

        If Me.bUseLocalMemory Then
            'Only copy data from local to core if we are using local data
            m_stpInit.Restart()

            Try
                Me.copyLocalToCore(Bio, Me.m_Data.Bcell)
                Me.copyLocalToCore(AMlin, AMm)
                Me.copyLocalToCore(Flin, F)
            Catch ex As Exception
                System.Console.WriteLine(Me.ToString + ".UpdateCoreData() Exception: " + ex.Message)
                m_logger.LogError(Me.ToString + ".UpdateCoreData() Exception: " + ex.Message)
            End Try

            m_stpInit.Stop()
            Console.WriteLine("Solver ID = " + Me.ThreadID.ToString + " copy CPU time (sec), " + Me.m_stpInit.Elapsed.TotalSeconds.ToString)

        End If 'If Me.bUseLocalMemory Then

    End Sub

    Private Sub initLocalDataForRun()

        Dim ngrps As Integer = Me.m_Data.NGroups

        If Me.bUseLocalMemory Then
            Me.copyCoreToLocal(Me.m_Data.Bcell, Bio)
            Me.copyCoreToLocal(Me.AMm, AMlin)
            Me.copyCoreToLocal(Me.F, Flin)
        End If

        'For igrp As Integer = 1 To ngrps
        '    For iflt As Integer = 1 To Me.m_Data.nFleets
        '        isCaught(iflt, igrp) = False
        '        If Me.m_PathData.Landing(iflt, igrp) + Me.m_PathData.Discard(iflt, igrp) > 0 Then
        '            isCaught(iflt, igrp) = True
        '        End If
        '    Next
        'Next

        'FtimeCell(i, j, iGrp)
        'm_Data.Blast(i, j, iGrp)
        'HdenCell(i, j, iGrp) = Hden(iGrp)

        '    SyncLock syncCopyLock

        'For igrp As Integer = 0 To ngrps
        '    SimGE(igrp) = m_SimData.SimGE(igrp)
        '    FtimeMax(igrp) = m_SimData.FtimeMax(igrp)
        '    Cbase(igrp) = m_SimData.Cbase(igrp)

        '    Fish1(igrp) = m_SimData.Fish1(igrp)
        '    Hden(igrp) = m_SimData.Hden(igrp)
        '    Ftime(igrp) = m_SimData.Ftime(igrp)
        '    FishTime(igrp) = m_SimData.FishTime(igrp)
        '    'Array.Copy(m_SimData.Ftime, Ftime, ngrps + 1)
        '    'Array.Copy(m_SimData.Fish1, Fish1, ngrps + 1)
        '    'Array.Copy(m_SimData.FishTime, FishTime, ngrps + 1)

        '    pred(igrp) = m_SimData.pred(igrp)
        '    Eatenof(igrp) = m_SimData.Eatenof(igrp)
        '    Eatenby(igrp) = m_SimData.Eatenby(igrp)

        '    RelVulBad(igrp) = m_Data.RelVulBad(igrp)
        'Next

        'For ilnk As Integer = 1 To Me.inLinks
        '    ilink(ilnk) = m_SimData.ilink(ilnk)
        '    jlink(ilnk) = m_SimData.jlink(ilnk)
        '    ArenaLink(ilnk) = m_SimData.ArenaLink(ilnk)
        'Next

        Array.Copy(m_SimData.SimGE, SimGE, ngrps + 1)
        Array.Copy(m_SimData.FtimeMax, FtimeMax, ngrps + 1)
        Array.Copy(m_SimData.Cbase, Cbase, ngrps + 1)

        Array.Copy(m_Data.RelVulBad, RelVulBad, ngrps + 1)

        Array.Copy(m_SimData.Fish1, Fish1, ngrps + 1)

        Array.Copy(m_SimData.ilink, ilink, Me.inLinks + 1)
        Array.Copy(m_SimData.jlink, jlink, Me.inLinks + 1)
        Array.Copy(m_SimData.ArenaLink, ArenaLink, Me.inLinks + 1)

        Array.Copy(m_SimData.Hden, Hden, ngrps + 1)
        Array.Copy(m_SimData.Ftime, Ftime, ngrps + 1)
        Array.Copy(m_SimData.Fish1, Fish1, ngrps + 1)
        Array.Copy(m_SimData.FishTime, FishTime, ngrps + 1)
        Array.Copy(m_SimData.pred, pred, ngrps + 1)
        Array.Copy(m_SimData.Eatenof, Eatenof, ngrps + 1)
        Array.Copy(m_SimData.Eatenby, Eatenby, ngrps + 1)

        Array.Copy(Me.m_Ecosim.A, A, Me.m_Ecosim.A.Length)
        Array.Copy(Me.m_SimData.SwitchPower, SwitchPower, ngrps + 1)
        Array.Copy(Me.m_SimData.BaseTimeSwitch, BaseTimeSwitch, Me.inLinks + 1)
        ' Array.Copy(Me.m_Data.IsFished, isFished, Me.m_Data.IsFished.Length)

        '      End SyncLock

    End Sub

    Private Sub AllocateLocalData()

        Dim nGroups As Integer = Me.m_Data.NGroups
        Dim nvartot As Integer = Me.m_Data.nvartot
        Dim nFleets As Integer = Me.m_Data.nFleets

        If m_TracerData.EcoSpaceConSimOn Then
            ReDim Derivcon(m_PathData.NumGroups), Cintotal(m_PathData.NumGroups), Closs(m_PathData.NumGroups)
        End If

        'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
        'local spatial variables

        EatEff = New Single(nvartot) {}
        VulPred = New Single(nvartot) {}
        Flowin = New Single(nvartot) {}
        FlowoutRate = New Single(nvartot) {}
        BB = New Single(nvartot) {}

        loss = New Single(nGroups) {}
        pbb = New Single(nGroups) {}

        'local versions of ecosim variables
        Hden = New Single(nGroups) {}
        Ftime = New Single(nGroups) {}
        FishTime = New Single(nGroups) {}
        pred = New Single(nGroups) {}
        Eatenof = New Single(nGroups) {}
        Eatenby = New Single(nGroups) {}
        MedVal = New Single(nGroups) {}

        SimGE = New Single(nGroups) {}
        FtimeMax = New Single(nGroups) {}
        Cbase = New Single(nGroups) {}
        RelVulBad = New Single(nGroups) {}
        Fish1 = New Single(nGroups) {}

        'FishRateGear(nFleets,nTime) used to pass Effort in the current cell at the current timestep to both SimDeritusMT and SetMedFunctions()
        'Effort from the current cell time step is stored in the zero index i.e. FishRateGear(fleet,0) = EffortSpace(fleet,row,col)
        'this allows cSpaceSolver.FishRateGear(fleet,time) to remain compatible with cEcosimDataStructures.FishRateGear(fleet,time)
        'and both Sim and Space can use the same methods... 
        ' ReDim EffortSpace(nFleets, 0)
        EffortSpace = New Single(nFleets, 0) {}

        'thread copy of global sums
        'BtimeLocal = New Single(nGroups) {}
        TotLossThread = New Single(nGroups) {}
        TotEatenByThread = New Single(nGroups) {}
        TotBiomThread = New Single(nGroups) {}
        TotPredThread = New Single(nGroups) {}
        TotIFDweightThread = New Single(nGroups) {}
        GroupDetritus = New Single(nGroups) {}

        BBRatio = New Single(nGroups) {}

        'ResultsByGroup = New Single(cEcospaceDataStructures.N_RESULTS_GROUPS, nGroups) {}
        'ResultsByFleet = New Single(cEcospaceDataStructures.N_RESULTS_FLEETS, nFleets) {}
        'ResultsByFleetGroup = New Single(cEcospaceDataStructures.N_RESULTS_FLEETGROUPS, nFleets, nGroups) {}
        'Landings = New Single(nGroups, nFleets) {}
        'ResultsCatchRegionGearGroup = New Single(m_Data.nRegions, nFleets, nGroups) {}

        'Really... for Hungabee... so inLinks is allocated on the current cpu
        'I'm not sure about this
        Me.inLinks = New Integer
        Me.inLinks = m_SimData.inlinks
        ilink = New Integer(inLinks) {}
        jlink = New Integer(inLinks) {}
        ArenaLink = New Integer(inLinks) {}

        SwitchPower = New Single(nGroups) {}
        A = New Single(nGroups, nGroups) {}
        BaseTimeSwitch = New Single(inLinks) {}

        ''iFlt, iRow, iCol
        'isFished = New Boolean(nFleets, Me.m_Data.InRow, Me.m_Data.InCol) {}
        'isCaught = New Boolean(nFleets, nGroups) {}

        'xxxxxxxxxxxxxxxxxxxxxxxxxx
    End Sub

    Private Sub InitForTimestep()

        Try

            If Me.bUseLocalMemory Then

                m_stpInit = Stopwatch.StartNew

                Me.AllocateLocalData()
                Me.initLocalDataForRun()

                m_stpInit.Stop()
                ' Console.WriteLine("Copy first = " + etfirst.ToString + " Total runtime(sec) " + stpCopy.Elapsed.TotalSeconds.ToString)

            Else

                'Array.Copy(Me.m_Data.IsFished, isFished, Me.m_Data.IsFished.Length)

                'Clear out the results
                Array.Clear(Me.Landings, 0, Me.Landings.Length)
                Array.Clear(Me.ResultsByGroup, 0, Me.ResultsByGroup.Length)
                Array.Clear(Me.ResultsByFleet, 0, Me.ResultsByFleet.Length)
                Array.Clear(Me.ResultsByFleetGroup, 0, Me.ResultsByFleetGroup.Length)
                Array.Clear(Me.ResultsCatchRegionGearGroup, 0, Me.ResultsCatchRegionGearGroup.Length)

                'Array.Clear(Me.BtimeLocal, 0, m_Data.NGroups)
                Array.Clear(Me.TotLossThread, 0, m_Data.NGroups)
                Array.Clear(Me.TotEatenByThread, 0, m_Data.NGroups)
                Array.Clear(Me.TotBiomThread, 0, m_Data.NGroups)
                Array.Clear(Me.TotPredThread, 0, m_Data.NGroups)
                Array.Clear(Me.TotIFDweightThread, 0, m_Data.NGroups)

            End If

        Catch ex As Exception
            Debug.Assert(False, ex.Message)
            m_logger.LogError(Me.ToString & ".InitForTimestep() Exception: " & ex.Message)
        End Try

    End Sub

    Public Sub Clear()

        Try
            'each solver get it's own Contaminant Tracer data and model
            Me.m_TracerData.Clear()
            Me.m_ConTracer = Nothing
        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".Clear() Exception: " & ex.Message)
            m_logger.LogError(Me.ToString & ".Clear() Exception: " & ex.Message)
        End Try

    End Sub

    ''' <summary>
    ''' Set the groups to iterate over.
    ''' </summary>
    ''' <param name="iFirstCell"></param>
    ''' <param name="iLastCell"></param>
    ''' <remarks>Call for each thread, before the thread is started, to set the groups to solve.</remarks>
    Public Sub FirstLastCells(ByVal iFirstCell As Integer, ByVal iLastCell As Integer)
        iFrstCell = iFirstCell
        iLstCell = iLastCell
    End Sub

    ''' <summary>
    ''' Do any processing necessary at the start of a new year
    ''' </summary>
    ''' <param name="iYear"></param>
    ''' <remarks></remarks>
    Public Sub YearTimeStep(ByVal iYear As Integer)
        Try
            If Search.bInSearch Then
                'Indicators need to clear out there yearly data
                ' Indic.YearTimeStep(m_EPData)

            End If
        Catch ex As Exception
            m_logger.LogError(Me.ToString & ".YearTimeStep() Exception: " & ex.Message)
        End Try

    End Sub

#End Region

#Region "Public 'Solve'"

    ''' <summary>
    ''' Compute the trophic for all groups on a list of map cells defined by <see cref="iFrstCell"> First </see> and  <see cref="iLstCell"> Last </see> cell in the list. 
    ''' </summary>
    ''' <param name="obParam"> Object parameter needed for ThreadPool.QueueUserWorkItem(...) it is ignored here.</param>
    ''' <remarks></remarks>
    Public Sub Solve(ByVal obParam As Object)
        'For our purposes here we are ignoring the obParam argument 
        'this sub signature is required by the ThreadPool.QueueUserWorkItem(...)

        'Dim thrdID As Integer = Threading.Thread.CurrentThread.ManagedThreadId
        'Console.WriteLine("Solve Derivt OBID = " & Me.ThreadID.ToString & ", ThreadID = " & thrdID.ToString & ", Start T = " & DateTime.Now.ToLongTimeString)
        'Console.WriteLine("     N Map Cells = " & (iLstCell - iFrstCell + 1).ToString)

        Me.m_stpWatch.Reset()
        Me.m_stpWatch.Start()
        Me.CatchCPUTimeSec = 0

        Me.InitForTimestep()

        Try
            Dim iCell As Integer
            Dim iLinearCells As Integer

            For iCell = iFrstCell To iLstCell
                'iCell linear index to cell in spatial map, converts to row col via iWaterCellIndex(iCell) and jWaterCellIndex(iCell)
                'iLinearCells linear index to spatial data stored locally 0 to iLstCell-iFrstCell+1 (number of cells to compute)
                ' If Me.bUseLocalMemory Then
                Me.SolveCell_LocalMemory(iCell, iLinearCells)
                ' Else
                ' Me.SolveCell_SharedMemory(m_Data.iWaterCellIndex(iCell), m_Data.jWaterCellIndex(iCell))
                'End If
                iLinearCells += 1

            Next iCell

        Catch ex As Exception
            m_logger.LogError(Me.ToString & ".Solve() Exception: " & ex.Message)
            Debug.Assert(False, ex.Message)
        End Try

        Me.UpdateCoreData()

        'set signal state to 'signaled' 
        'the processing has finished SignalState.WaitOne() will return immediately
        If Interlocked.Decrement(cSpaceSolver.ThreadIncrementer) = 0 Then
            WaitHandle.Set()
        End If

        Me.m_stpWatch.Stop()
        Me.SolveCPUTimeSec = Me.m_stpWatch.Elapsed.TotalSeconds
        ' Console.WriteLine("SpaceSolver.Solve() ID " & Me.ThreadID.ToString & " Run time(sec)" & (Me.m_stpWatch.Elapsed.TotalSeconds).ToString)

    End Sub


#Region "SolveCell Local and Shared memory"

    ''' <summary>
    ''' Calculate trophic interaction for a single map cell directly on the Core Ecospace data. 
    ''' </summary>
    ''' <param name="i">Row index of the cell in the Core Maps arrays.</param>
    ''' <param name="j">Column index of the cell in the Core Maps arrays.</param>
    ''' <returns>True if successful. False otherwise.</returns>
    ''' <remarks>
    ''' There are two version of SolveCell see <see cref="SolveCell_LocalMemory">SolveCell_LocalMemory</see> for the other version.
    ''' </remarks>
    Private Function SolveCell_SharedMemory(ByVal i As Integer, ByVal j As Integer) As Boolean
        Dim iGrp As Integer
        Dim PopWt As Single

        Try
            'this changes the timestep for higher order numerical sceme.  the timestep isn't actually different, it's a multiplier
            TimeStep2 = m_Data.TimeStep * 0.66667F

            'Init Contaminant tracing data
            If m_TracerData.EcoSpaceConSimOn Then
                For iGrp = 0 To m_Data.NGroups
                    m_ConTracer.ConcTr(iGrp) = m_Data.Ccell(i, j, iGrp)
                Next
            End If

            'Init Fishing Effort data outside the main group loop
            If m_Data.PredictEffort Then
                '****Following lines set fishrategear for Simdetritus
                For ig = 1 To m_Data.nFleets
                    'Copy effort from the cell into the zero time element of a local array with the same structure as EcoSim.FishRateGear(fleet,time)
                    EffortSpace(ig, 0) = m_Data.EffortSpace(ig, i, j)
                    'effortspace should be 1.0 for cell with "average" effort by gear type ig
                Next
            Else
                For ig = 1 To m_Data.nFleets
                    'Copy effort from the cell into the zero time element of a local array with the same structure as EcoSim.FishRateGear(fleet,time)
                    m_Data.EffortSpace(ig, i, j) = 1.0
                    'fishrategear for Simdetritus
                    EffortSpace(ig, 0) = 1 ' 1 x FishMGear(ig, ip)
                Next
            End If 'If m_Data.PredictEffort > 0 Then

            For iGrp = 1 To m_Data.NGroups
                'abmpa: at this point (after having been in solvegrid) the BCell holds
                'the long term equilibrium biomass or at least an approx to)

                If m_Data.Depth(i, j) = 0 Then m_Data.Bcell(i, j, iGrp) = 0
                BB(iGrp) = m_Data.Bcell(i, j, iGrp)

                'sum biomass over all the cells
                'this is now done individually for each thread, then summed outside the threads
                'Btime(ip) = Btime(ip) + BB(ip)
                'BtimeLocal(iGrp) = BB(iGrp) * m_Data.Width(i) + BtimeLocal(iGrp)

                If (m_SimData.NoIntegrate(iGrp) = iGrp Or m_SimData.NoIntegrate(iGrp) < 0) And m_SimData.SimGE(iGrp) > 0 Then
                    If (Cper(i, j, iGrp) > 0 And m_SimData.FtimeAdjust(iGrp) > 0) Then
                        FtimeCell(i, j, iGrp) = FtimeCell(i, j, iGrp) * (0.7F + 0.3F * m_SimData.Cbase(iGrp) / Cper(i, j, iGrp))
                    End If
                    '  FtimeCell(i, j, ip) = Cbase(ip) / Cper(i, j, ip)
                    If FtimeCell(i, j, iGrp) > m_SimData.FtimeMax(iGrp) Then FtimeCell(i, j, iGrp) = m_SimData.FtimeMax(iGrp)
                    If FtimeCell(i, j, iGrp) < 0.1 Then FtimeCell(i, j, iGrp) = 0.1
                    Ftime(iGrp) = FtimeCell(i, j, iGrp)
                End If

                Hden(iGrp) = HdenCell(i, j, iGrp)

                'Set FishTime() (F, fishing mortality) for this timestep/cell
                If m_Data.PredictEffort Then
                    'F set by cEcospace.PredictEffortDistributionThreaded()
                    FishTime(iGrp) = m_Data.Ftot(iGrp, i, j)
                Else
                    'Not Predicting Effort
                    'F = Ecopath base F
                    FishTime(iGrp) = Fish1(iGrp)

                End If 'If m_Data.PredictEffort > 0 Then

                ''Set FishTime() (F, fishing mortality) for this timestep cell
                'If m_Data.Depth(i, j) > 0 Then
                '    If m_Data.PredictEffort Then
                '        'F set by cEcospace.PredictEffortDistributionThreaded()
                '        FishTime(iGrp) = m_Data.Ftot(iGrp, i, j)
                '        '****Following lines set fishrategear for Simdetritus
                '        For ig = 1 To m_Data.nFleets
                '            EffortSpace(ig, 0) = m_Data.EffortSpace(ig, i, j)
                '            'effortspace should be 1.0 for cell with "average" effort by gear type ig
                '        Next
                '    Else
                '        'Not Predicting Effort
                '        'F = Ecopath base F
                '        FishTime(iGrp) = Fish1(iGrp)

                '        For ig = 1 To m_Data.nFleets
                '            'Effort used to calculate Catch and Value in cEcospace.accumCatchData
                '            m_Data.EffortSpace(ig, i, j) = 1.0
                '            'fishrategear for Simdetritus
                '            EffortSpace(ig, 0) = 1 ' 1 x FishMGear(ig, ip)
                '        Next
                '    End If 'If m_Data.PredictEffort > 0 Then

                'Else
                '    'depth<=0
                '    FishTime(iGrp) = 0
                '    '****Following line sets fishrategear for Simdetritus
                '    For ig = 1 To m_Data.nFleets
                '        EffortSpace(ig, 0) = 0
                '    Next

                'End If 'If m_Data.Depth(i, j) > 0 Then

                EatEff(iGrp) = 1
                VulPred(iGrp) = 1

                If m_Data.HabCap(i, j, iGrp) < 0.1 Then
                    VulPred(iGrp) = m_Data.RelVulBad(iGrp)
                End If

                EatEff(iGrp) = m_Data.HabCap(i, j, iGrp) 'm_Data.EatEffBad(iGrp)

            Next iGrp

            Me.accumCatchData(itt, iYear, BB, FishTime, i, j)

            For isc = 1 To m_Data.Nvarsplit
                ieco = Ecode(isc)
                'ebb(nvar2 + isc) = m_Data.Bcell(i, j, nvar2 + isc)

                If m_Data.NewMultiStanza Or m_Data.UseIBM Then
                    pred(ieco) = m_Data.PredCell(i, j, ieco)
                Else
                    pred(ieco) = m_Data.Bcell(i, j, nvar2 + isc) * PconSplit(isc) ' Nadult(i, j, ip)
                End If
            Next isc

            RelPPupwell = 1 + m_Data.PPupWell * m_Data.UpVel(i, j) / m_Data.CellLength

            If RelPPupwell < 1 Then RelPPupwell = 1

            Dim scaledPP As Double = (m_Data.RelPP(i, j) / PPScale)

            'jb compute Flowin() and FlowoutRate() for all groups for this row/col
            derivtRed(BB, Flowin, FlowoutRate, EatEff, VulPred, scaledPP, i, j)

            If m_TracerData.EcoSpaceConSimOn Then
                m_ConTracer.loss = loss 'set loss to ecospace loss for this cell
                m_ConTracer.ConDeriv(BB, Derivcon, Cintotal, Closs, m_Data.RelCin(i, j), True)
            End If

            'jb now populate the spatial matrixes with the data computed by derivtRed() for this cell across all groups
            For iGrp = 1 To m_Data.NGroups
                HdenCell(i, j, iGrp) = Hden(iGrp)
                If pred(iGrp) > 1.0E-30 Then
                    RelFitness(i, j, iGrp) = (m_SimData.SimGE(iGrp) * Eatenby(iGrp) - loss(iGrp)) / pred(iGrp) + FishTime(iGrp)
                Else
                    RelFitness(i, j, iGrp) = -2.0F * m_PathData.PB(iGrp)
                End If
            Next

            For iGrp = 1 To m_Data.NGroups

                If Me.m_PathData.isEcospaceModelCoupled Then Me.m_Data.GroupDetritus(i, j, iGrp) = GroupDetritus(iGrp)

                F(i, j, iGrp) = Flowin(iGrp)
                AMm(i, j, iGrp) = -FlowoutRate(iGrp) - Bcw(i + 1, j, iGrp) - C(i - 1, j, iGrp) - d(i, j, iGrp) - e(i, j, iGrp)
                If AMm(i, j, iGrp) >= 0 Then AMm(i, j, iGrp) = -1.0E+30
                'm_Data.deriv2(i, j, ip) = m_Data.deriv(i, j, ip)
                'm_Data.deriv(i, j, ip) = AMm(i, j, ip) * m_Data.Bcell(i, j, ip) + F(i, j, ip) + Bcw(i, j, ip) * m_Data.Bcell(i - 1, j, ip) + C(i, j, ip) * m_Data.Bcell(i + 1, j, ip) + d(i, j - 1, ip) * m_Data.Bcell(i, j - 1, ip) + e(i, j + 1, ip) * m_Data.Bcell(i, j + 1, ip)
                If m_Data.SpaceTime Then
                    AMm(i, j, iGrp) = AMm(i, j, iGrp) - 1 / TimeStep2
                    'this is for new 2nd order BDF numerical sceme (replacing backwards euler)
                    F(i, j, iGrp) = F(i, j, iGrp) + (1.3333F * m_Data.Bcell(i, j, iGrp) - 0.3333F * m_Data.Blast(i, j, iGrp)) / TimeStep2
                    m_Data.Blast(i, j, iGrp) = m_Data.Bcell(i, j, iGrp)
                End If

                If m_SimData.SimGE(iGrp) > 0 Then
                    Cper(i, j, iGrp) = Eatenby(iGrp) / (m_Data.Bcell(i, j, iGrp) + 1.0E-20F)
                End If
                If Cper(i, j, iGrp) < 0.001 * m_SimData.Cbase(iGrp) Then
                    Cper(i, j, iGrp) = 0.001F * m_SimData.Cbase(iGrp)
                End If

            Next iGrp

            If m_TracerData.EcoSpaceConSimOn Then
                For iGrp = 0 To m_Data.NGroups
                    m_Data.Ftr(i, j, iGrp) = Cintotal(iGrp)
                    m_Data.AMmTr(i, j, iGrp) = -Closs(iGrp) - Bcw(i + 1, j, iGrp) - C(i - 1, j, iGrp) - d(i, j, iGrp) - e(i, j, iGrp)
                    If m_Data.AMmTr(i, j, iGrp) >= 0 Then m_Data.AMmTr(i, j, iGrp) = -1.0E+30
                    '   If m_Data.SpaceTime And FastIntegrate(ip) = False Then
                    m_Data.Ftr(i, j, iGrp) = m_Data.Ftr(i, j, iGrp) + m_Data.Ccell(i, j, iGrp) / TimeStep2 '/ m_Data.TimeStep
                    m_Data.AMmTr(i, j, iGrp) = m_Data.AMmTr(i, j, iGrp) - 1 / TimeStep2 '/ m_Data.TimeStep
                    '  End If
                Next
            End If

            isc = 0
            For isp = 1 To m_Stanza.Nsplit
                ieco = m_Stanza.EcopathCode(isp, m_Stanza.Nstanza(isp))

                RelR = m_Data.Bcell(i, j, ieco) * RelRepStanza(isp) ' * m_Data.HabCap(i, j, ieco) * m_Data.nWaterCells / m_Data.TotHabCap(ieco) ' Added HabCap correction for recruitment rate
                For ist = 1 To m_Stanza.Nstanza(isp)
                    isc = isc + 1
                    ieco = m_Stanza.EcopathCode(isp, ist)
                    If m_Data.NewMultiStanza Then
                        'accumulate information needed to predict mean stanza loss, feeding, IFD weights from derivtred outputs
                        'these arrays are used in the new SpaceSplitUpdate subroutine for predicting mortality
                        'rate and growth rate averages over space by age in that update routine
                        'IFDweight is used to predict proportion of biomass of ieco stanza that will be on cell i,j
                        'If (m_Data.PrefHab(ieco, m_Data.HabType(i, j)) = True Or m_Data.PrefHab(ieco, 0) = True) And m_Data.Depth(i, j) > 0 Then
                        If m_Data.Depth(i, j) > 0 And m_Data.HabCap(i, j, ieco) > 0.1 Then
                            PopWt = m_Data.Bcell(i, j, nvar2 + isc)
                            TotLossThread(ieco) = TotLossThread(ieco) + loss(ieco) * PopWt
                            TotEatenByThread(ieco) = TotEatenByThread(ieco) + Eatenby(ieco) * PopWt
                            TotBiomThread(ieco) = TotBiomThread(ieco) + m_Data.Bcell(i, j, ieco) * PopWt
                            TotPredThread(ieco) = TotPredThread(ieco) + pred(ieco) * PopWt
                            'm_Data.IFDweight(i, j, ieco) = ((Eatenby(ieco) / pred(ieco)) / (loss(ieco) / m_Data.Bcell(i, j, ieco))) ^ m_Data.IFDPower
                            m_Data.IFDweight(i, j, ieco) = PopWt 'm_Data.Bcell(i, j, nvar2 + isc)
                            TotIFDweightThread(ieco) = TotIFDweightThread(ieco) + m_Data.IFDweight(i, j, ieco)
                        End If
                    ElseIf m_Data.UseIBM Then
                        m_Stanza.Zcell(i, j, ieco) = loss(ieco) / (m_Data.Bcell(i, j, ieco) + 1.0E-30F)
                        If m_Data.Bcell(i, j, ieco) = 0 Then
                            m_Stanza.Zcell(i, j, ieco) = 0
                        End If
                        Cper(i, j, ieco) = Eatenby(ieco) / (m_Data.PredCell(i, j, ieco) + 1.0E-30F)
                        If m_Data.PredCell(i, j, ieco) = 0 Then
                            Cper(i, j, ieco) = m_SimData.Cbase(ieco)
                        End If
                    End If
                    SurvRat = CSng(Math.Exp(-FlowoutRate(ieco) * Tstanza(isc)))
                    RelRS = RelR * SurvRat 'Math.Exp(-FlowoutRate(ieco) * Tstanza(isc))
                    If ist = 1 Then '< m_Stanza.Nstanza(isp) Then
                        Rflow = RelR - RelRS
                    Else
                        Rflow = RtoNext
                    End If
                    RtoNext = m_Data.Bcell(i, j, nvar2 + isc) * FlowoutRate(ieco) / (1 / (SurvRat + 1.0E-20F) - 1)
                    RelR = RelRS
                    If m_Data.NewMultiStanza Then
                        Cper(i, j, ieco) = Eatenby(ieco) / (m_Data.PredCell(i, j, ieco) + 1.0E-30F)
                        If ist > 1 Then Rflow = m_Data.Bcell(i, j, m_Stanza.EcopathCode(isp, ist - 1))
                    ElseIf m_Data.UseIBM = False And m_Data.NewMultiStanza = False Then
                        Cper(i, j, ieco) = Eatenby(ieco) / (m_Data.Bcell(i, j, nvar2 + isc) * PconSplit(isc) + 1.0E-30F)
                    End If

                    F(i, j, nvar2 + isc) = Rflow
                    AMm(i, j, nvar2 + isc) = -FlowoutRate(ieco) - Bcw(i + 1, j, ieco) - C(i - 1, j, ieco) - d(i, j, ieco) - e(i, j, ieco)
                    If AMm(i, j, nvar2 + isc) >= 0 Then AMm(i, j, nvar2 + isc) = -1.0E+30

                    'm_Data.deriv2(i, j, nvar2 + isc) = m_Data.deriv(i, j, nvar2 + isc)
                    'm_Data.deriv(i, j, nvar2 + isc) = AMm(i, j, nvar2 + isc) * m_Data.Bcell(i, j, nvar2 + isc) + F(i, j, nvar2 + isc) + Bcw(i, j, nvar2 + isc) * m_Data.Bcell(i - 1, j, nvar2 + isc) + C(i, j, nvar2 + isc) * m_Data.Bcell(i + 1, j, nvar2 + isc) + d(i, j - 1, nvar2 + isc) * m_Data.Bcell(i, j - 1, nvar2 + isc) + e(i, j + 1, nvar2 + isc) * m_Data.Bcell(i, j + 1, nvar2 + isc)

                    If m_Data.SpaceTime Then
                        F(i, j, nvar2 + isc) = F(i, j, nvar2 + isc) + (1.3333F * m_Data.Bcell(i, j, nvar2 + isc) - 0.3333F * m_Data.Blast(i, j, nvar2 + isc)) / TimeStep2
                        'F(i, j, nvar2 + isc) = F(i, j, nvar2 + isc) + m_Data.Bcell(i, j, nvar2 + isc) / m_Data.TimeStep
                        AMm(i, j, nvar2 + isc) = AMm(i, j, nvar2 + isc) - 1 / TimeStep2
                        m_Data.Blast(i, j, nvar2 + isc) = m_Data.Bcell(i, j, nvar2 + isc)
                    End If
                Next
            Next

            Return True

        Catch ex As Exception
            m_logger.LogError(Me.ToString & ".SolveCell() Error: " & ex.Message)
            Debug.Assert(False, ex.StackTrace)
            Throw New ApplicationException(Me.ToString & ".SolveCell() Error: " & ex.Message)
        End Try

    End Function

#End Region


    ''' <summary>
    ''' Calculate trophic interaction for a single map cell
    ''' </summary>
    ''' <param name="iCell">Linear index of the two dimensional spatial array 1 to nWaterCells(number of water cells in the base map). Converts cell index to map row and col via <see cref="cEcospaceDataStructures.iWaterCellIndex"> iWaterCellIndex(iCell)</see>.</param>
    ''' <param name="iLoc">Index to the local spatial data stored in a linear array by cell. Local data initialized in <see cref="initLocalDataForRun">initLocalDataForRun()</see>.  </param>
    ''' <returns>True if successful. False otherwise.</returns>
    ''' <remarks></remarks>
    Private Function SolveCell_LocalMemory(iCell As Integer, iLoc As Integer) As Boolean
        Dim iGrp As Integer
        Dim PopWt As Single
        Dim i As Integer, j As Integer

        'convert the iCell index to Row and Col in the map
        i = Me.m_Data.iWaterCellIndex(iCell)
        j = Me.m_Data.jWaterCellIndex(iCell)

        Try

            Debug.Assert(Me.m_Data.Depth(i, j) > 0, Me.ToString + ".SolveCell() should not be run on cells with a zero depth.")

            'this changes the timestep for higher order numerical sceme.  the timestep isn't actually different, it's a multiplier
            TimeStep2 = m_Data.TimeStep * 0.66667F

            'Init Contaminant tracing data
            If m_TracerData.EcoSpaceConSimOn Then
                For iGrp = 0 To m_Data.NGroups
                    m_ConTracer.ConcTr(iGrp) = m_Data.Ccell(i, j, iGrp)
                Next
            End If

            'Init Fishing Effort data outside the main group loop
            If m_Data.PredictEffort Then
                '****Following lines set fishrategear for Simdetritus
                For ig = 1 To m_Data.nFleets
                    'Copy effort from the cell into the zero time element of a local array with the same structure as EcoSim.FishRateGear(fleet,time)
                    EffortSpace(ig, 0) = m_Data.EffortSpace(ig, i, j)
                    'effortspace should be 1.0 for cell with "average" effort by gear type ig
                Next
            Else
                For ig = 1 To m_Data.nFleets
                    'Copy effort from the cell into the zero time element of a local array with the same structure as EcoSim.FishRateGear(fleet,time)
                    m_Data.EffortSpace(ig, i, j) = 1.0
                    'fishrategear for Simdetritus
                    EffortSpace(ig, 0) = 1 ' 1 x FishMGear(ig, ip)
                Next
            End If 'If m_Data.PredictEffort > 0 Then

            'Init group data 
            For iGrp = 1 To m_Data.NGroups
                'abmpa: at this point (after having been in solvegrid) the BCell holds
                'the long term equilibrium biomass or at least an approx to)

                If m_Data.Depth(i, j) = 0 Then Me.Bio(iGrp, iLoc) = 0
                'Me.Bio(iGrp, iLin) = 0
                BB(iGrp) = Me.Bio(iGrp, iLoc)

                'sum biomass over all the cells
                'this is now done individually for each thread, then summed outside the threads
                'Btime(ip) = Btime(ip) + BB(ip)
                'BtimeLocal(iGrp) = BB(iGrp) * m_Data.Width(i) + BtimeLocal(iGrp)

                If (m_SimData.NoIntegrate(iGrp) = iGrp Or m_SimData.NoIntegrate(iGrp) < 0) And SimGE(iGrp) > 0 Then
                    If (Cper(i, j, iGrp) > 0 And m_SimData.FtimeAdjust(iGrp) > 0) Then
                        FtimeCell(i, j, iGrp) = FtimeCell(i, j, iGrp) * (0.7F + 0.3F * Cbase(iGrp) / Cper(i, j, iGrp))
                    End If
                    '  FtimeCell(i, j, ip) = Cbase(ip) / Cper(i, j, ip)
                    If FtimeCell(i, j, iGrp) > FtimeMax(iGrp) Then FtimeCell(i, j, iGrp) = FtimeMax(iGrp)
                    If FtimeCell(i, j, iGrp) < 0.1 Then FtimeCell(i, j, iGrp) = 0.1
                    Ftime(iGrp) = FtimeCell(i, j, iGrp)
                End If

                Hden(iGrp) = HdenCell(i, j, iGrp)

                'Set FishTime() (F, fishing mortality) for this timestep/cell
                If m_Data.PredictEffort Then
                    'F set by cEcospace.PredictEffortDistributionThreaded()
                    FishTime(iGrp) = m_Data.Ftot(iGrp, i, j)
                Else
                    'Not Predicting Effort
                    'F = Ecopath base F
                    FishTime(iGrp) = Fish1(iGrp)

                End If 'If m_Data.PredictEffort > 0 Then

                EatEff(iGrp) = 1
                VulPred(iGrp) = 1

                EatEff(iGrp) = m_Data.HabCap(i, j, iGrp)
                If EatEff(iGrp) < 0.1 Then
                    VulPred(iGrp) = m_Data.RelVulBad(iGrp)
                End If

            Next iGrp

            Me.accumCatchData(itt, iYear, BB, FishTime, i, j)

            For isc = 1 To m_Data.Nvarsplit
                ieco = Ecode(isc)
                'ebb(nvar2 + isc) = m_Data.Bcell(i, j, nvar2 + isc)

                If m_Data.NewMultiStanza Or m_Data.UseIBM Then
                    pred(ieco) = m_Data.PredCell(i, j, ieco)
                Else
                    'pred(ieco) = m_Data.Bcell(i, j, nvar2 + isc) * PconSplit(isc) ' Nadult(i, j, ip)
                    pred(ieco) = Me.Bio(nvar2 + isc, iLoc) * PconSplit(isc) ' Nadult(i, j, ip)
                End If
            Next isc

            RelPPupwell = 1 + m_Data.PPupWell * m_Data.UpVel(i, j) / m_Data.CellLength

            If RelPPupwell < 1 Then RelPPupwell = 1

            Dim scaledPP As Double = m_Data.RelPP(i, j) / PPScale

            'jb compute Flowin() and FlowoutRate() for all groups for this row/col
            derivtRed(BB, Flowin, FlowoutRate, EatEff, VulPred, scaledPP, i, j)

            If m_TracerData.EcoSpaceConSimOn Then
                m_ConTracer.loss = loss 'set loss to ecospace loss for this cell
                m_ConTracer.ConDeriv(BB, Derivcon, Cintotal, Closs, m_Data.RelCin(i, j), True)
            End If

            'jb now populate the spatial matrixes with the data computed by derivtRed() for this cell across all groups
            For iGrp = 1 To m_Data.NGroups
                HdenCell(i, j, iGrp) = Hden(iGrp)
                If pred(iGrp) > 1.0E-30 Then
                    RelFitness(i, j, iGrp) = (SimGE(iGrp) * Eatenby(iGrp) - loss(iGrp)) / pred(iGrp) + FishTime(iGrp)
                Else
                    RelFitness(i, j, iGrp) = -2.0F * m_PathData.PB(iGrp)
                End If
            Next

            For iGrp = 1 To m_Data.NGroups

                If Me.m_PathData.isEcospaceModelCoupled Then Me.m_Data.GroupDetritus(i, j, iGrp) = GroupDetritus(iGrp)

                'F(i, j, iGrp) = Flowin(iGrp)
                ' AMm(i, j, iGrp) = -FlowoutRate(iGrp) - Bcw(i + 1, j, iGrp) - C(i - 1, j, iGrp) - d(i, j, iGrp) - e(i, j, iGrp)

                Flin(iGrp, iLoc) = Flowin(iGrp)
                AMlin(iGrp, iLoc) = -FlowoutRate(iGrp) - Bcw(i + 1, j, iGrp) - C(i - 1, j, iGrp) - d(i, j, iGrp) - e(i, j, iGrp)
                If AMlin(iGrp, iLoc) >= 0 Then AMlin(iGrp, iLoc) = -1.0E+30

                If m_Data.SpaceTime Then
                    AMlin(iGrp, iLoc) = AMlin(iGrp, iLoc) - 1 / TimeStep2
                    'this is for new 2nd order BDF numerical sceme (replacing backwards euler)
                    ' F(i, j, iGrp) = F(i, j, iGrp) + (1.3333F * m_Data.Bcell(i, j, iGrp) - 0.3333F * m_Data.Blast(i, j, iGrp)) / TimeStep2
                    Flin(iGrp, iLoc) = Flin(iGrp, iLoc) + (1.3333F * Bio(iGrp, iLoc) - 0.3333F * m_Data.Blast(i, j, iGrp)) / TimeStep2
                    'm_Data.Blast(i, j, iGrp) = m_Data.Bcell(i, j, iGrp)
                    m_Data.Blast(i, j, iGrp) = Me.Bio(iGrp, iLoc)
                End If

                If m_SimData.SimGE(iGrp) > 0 Then
                    ' Cper(i, j, iGrp) = Eatenby(iGrp) / (m_Data.Bcell(i, j, iGrp) + 1.0E-20F)
                    Cper(i, j, iGrp) = Eatenby(iGrp) / (Bio(iGrp, iLoc) + 1.0E-20F)
                End If
                If Cper(i, j, iGrp) < 0.001 * m_SimData.Cbase(iGrp) Then
                    Cper(i, j, iGrp) = 0.001F * m_SimData.Cbase(iGrp)
                End If

            Next iGrp

            If m_TracerData.EcoSpaceConSimOn Then
                For iGrp = 0 To m_Data.NGroups
                    m_Data.Ftr(i, j, iGrp) = Cintotal(iGrp)
                    m_Data.AMmTr(i, j, iGrp) = -Closs(iGrp) - Bcw(i + 1, j, iGrp) - C(i - 1, j, iGrp) - d(i, j, iGrp) - e(i, j, iGrp)
                    If m_Data.AMmTr(i, j, iGrp) >= 0 Then m_Data.AMmTr(i, j, iGrp) = -1.0E+30
                    '   If m_Data.SpaceTime And FastIntegrate(ip) = False Then
                    m_Data.Ftr(i, j, iGrp) = m_Data.Ftr(i, j, iGrp) + m_Data.Ccell(i, j, iGrp) / TimeStep2
                    m_Data.AMmTr(i, j, iGrp) = m_Data.AMmTr(i, j, iGrp) - 1 / TimeStep2
                    '  End If
                Next
            End If

            isc = 0
            For isp = 1 To m_Stanza.Nsplit
                ieco = m_Stanza.EcopathCode(isp, m_Stanza.Nstanza(isp))

                RelR = m_Data.Bcell(i, j, ieco) * RelRepStanza(isp) ' * m_Data.HabCap(i, j, ieco) * m_Data.nWaterCells / m_Data.TotHabCap(ieco) ' Added HabCap correction for recruitment rate
                For ist = 1 To m_Stanza.Nstanza(isp)
                    isc = isc + 1
                    ieco = m_Stanza.EcopathCode(isp, ist)
                    If m_Data.NewMultiStanza Then
                        'accumulate information needed to predict mean stanza loss, feeding, IFD weights from derivtred outputs
                        'these arrays are used in the new SpaceSplitUpdate subroutine for predicting mortality
                        'rate and growth rate averages over space by age in that update routine
                        'IFDweight is used to predict proportion of biomass of ieco stanza that will be on cell i,j
                        'If (m_Data.PrefHab(ieco, m_Data.HabType(i, j)) = True Or m_Data.PrefHab(ieco, 0) = True) And m_Data.Depth(i, j) > 0 Then
                        If m_Data.Depth(i, j) > 0 And m_Data.HabCap(i, j, ieco) > 0.1 Then
                            PopWt = m_Data.Bcell(i, j, nvar2 + isc)
                            TotLossThread(ieco) = TotLossThread(ieco) + loss(ieco) * PopWt
                            TotEatenByThread(ieco) = TotEatenByThread(ieco) + Eatenby(ieco) * PopWt
                            TotBiomThread(ieco) = TotBiomThread(ieco) + m_Data.Bcell(i, j, ieco) * PopWt
                            TotPredThread(ieco) = TotPredThread(ieco) + pred(ieco) * PopWt
                            'm_Data.IFDweight(i, j, ieco) = ((Eatenby(ieco) / pred(ieco)) / (loss(ieco) / m_Data.Bcell(i, j, ieco))) ^ m_Data.IFDPower
                            m_Data.IFDweight(i, j, ieco) = PopWt 'm_Data.Bcell(i, j, nvar2 + isc)
                            TotIFDweightThread(ieco) = TotIFDweightThread(ieco) + m_Data.IFDweight(i, j, ieco)
                        End If
                    ElseIf m_Data.UseIBM Then
                        m_Stanza.Zcell(i, j, ieco) = loss(ieco) / (m_Data.Bcell(i, j, ieco) + 1.0E-30F)
                        If m_Data.Bcell(i, j, ieco) = 0 Then
                            m_Stanza.Zcell(i, j, ieco) = 0
                        End If
                        Cper(i, j, ieco) = Eatenby(ieco) / (m_Data.PredCell(i, j, ieco) + 1.0E-30F)
                        If m_Data.PredCell(i, j, ieco) = 0 Then
                            Cper(i, j, ieco) = m_SimData.Cbase(ieco)
                        End If
                    End If
                    SurvRat = CSng(Math.Exp(-FlowoutRate(ieco) * Tstanza(isc)))
                    RelRS = RelR * SurvRat 'Math.Exp(-FlowoutRate(ieco) * Tstanza(isc))
                    If ist = 1 Then '< m_Stanza.Nstanza(isp) Then
                        Rflow = RelR - RelRS
                    Else
                        Rflow = RtoNext
                    End If
                    RtoNext = m_Data.Bcell(i, j, nvar2 + isc) * FlowoutRate(ieco) / (1 / (SurvRat + 1.0E-20F) - 1)
                    RelR = RelRS
                    If m_Data.NewMultiStanza Then
                        Cper(i, j, ieco) = Eatenby(ieco) / (m_Data.PredCell(i, j, ieco) + 1.0E-30F)
                        If ist > 1 Then Rflow = m_Data.Bcell(i, j, m_Stanza.EcopathCode(isp, ist - 1))
                    ElseIf m_Data.UseIBM = False And m_Data.NewMultiStanza = False Then
                        Cper(i, j, ieco) = Eatenby(ieco) / (m_Data.Bcell(i, j, nvar2 + isc) * PconSplit(isc) + 1.0E-30F)
                    End If

                    F(i, j, nvar2 + isc) = Rflow
                    AMm(i, j, nvar2 + isc) = -FlowoutRate(ieco) - Bcw(i + 1, j, ieco) - C(i - 1, j, ieco) - d(i, j, ieco) - e(i, j, ieco)
                    If AMm(i, j, nvar2 + isc) >= 0 Then AMm(i, j, nvar2 + isc) = -1.0E+30

                    'm_Data.deriv2(i, j, nvar2 + isc) = m_Data.deriv(i, j, nvar2 + isc)
                    'm_Data.deriv(i, j, nvar2 + isc) = AMm(i, j, nvar2 + isc) * m_Data.Bcell(i, j, nvar2 + isc) + F(i, j, nvar2 + isc) + Bcw(i, j, nvar2 + isc) * m_Data.Bcell(i - 1, j, nvar2 + isc) + C(i, j, nvar2 + isc) * m_Data.Bcell(i + 1, j, nvar2 + isc) + d(i, j - 1, nvar2 + isc) * m_Data.Bcell(i, j - 1, nvar2 + isc) + e(i, j + 1, nvar2 + isc) * m_Data.Bcell(i, j + 1, nvar2 + isc)

                    If m_Data.SpaceTime Then
                        F(i, j, nvar2 + isc) = F(i, j, nvar2 + isc) + (1.3333F * m_Data.Bcell(i, j, nvar2 + isc) - 0.3333F * m_Data.Blast(i, j, nvar2 + isc)) / TimeStep2
                        'F(i, j, nvar2 + isc) = F(i, j, nvar2 + isc) + m_Data.Bcell(i, j, nvar2 + isc) / m_Data.TimeStep
                        AMm(i, j, nvar2 + isc) = AMm(i, j, nvar2 + isc) - 1 / TimeStep2
                        m_Data.Blast(i, j, nvar2 + isc) = m_Data.Bcell(i, j, nvar2 + isc)
                    End If
                Next
            Next

            '                    'For MPA Seed routine:
            '                    'At equilibrium 0 = dB = G - ZB, hence Bo = G/Z, where Bo is in    m_data.bcell() and Z=-AMm()
            '                    'For the no fishing situation: Bclose ~ Bo Z / (Z-F) or
            '                    'Bclose = -Bcell(i,j,ip) * AMm(i,j,ip) / (AMm(i,j,p) - Ftime(i,j,ip))
            '                    'This is the long-term predicted biomass in the cell from not fishing there
            '                    '   If AMm(i, j, ip) > 0 Then Bclose(i, j, ip) = -Bcell(i, j, ip) * AMm(i, j, ip) / (AMm(i, j, P) - Ftime(i, j, ip))

            Return True

        Catch ex As Exception
            m_logger.LogError(Me.ToString & ".SolveCell() Error: " & ex.Message)
            Debug.Assert(False, ex.StackTrace)
            Throw New ApplicationException(Me.ToString & ".SolveCell() Error: " & ex.Message)
        End Try

    End Function

#Region "Solve Cell"

#End Region

#End Region

#Region "Private Methods"

    Private Sub derivtRed(ByVal Biomass() As Single, ByRef Flowin() As Single, ByRef FlowoutRate() As Single, ByRef EatEff() As Single, ByRef VulPred() As Single, ByVal RelProdScaler As Double, ByVal iRow As Integer, ByVal iCol As Integer)
        'reduced derivatives for MPA equilibration procedure
        Dim i As Integer, j As Integer, ii As Integer
        Dim eat As Single, Pmult As Single
        Dim SimGEt As Single
        Dim Dwe As Single
        Dim Bprey As Single

        'Imported Detritus forcing function multiplier
        Dim DtImpMult As Single

        Dim aeff() As Single = New Single(inLinks) {}
        Dim Veff() As Single = New Single(inLinks) {}
        'ReDim aeff(inLinks), Veff(inLinks)

        Dim Hdent() As Single = New Single(m_Data.NGroups) {}
        ' ReDim Hdent(m_Data.NGroups)

        'EwE5 ToDetritus() is declared at a global level
        'in EcoSpace this is the only place it is used so its scope is local to EcoSpace
        Dim ToDetritus() As Single = New Single(m_Data.NGroups) {}
        'ReDim ToDetritus(m_Data.NGroups)

        Try

            'populate MedVal(nMedFunctions) with the Mediation Function multiplier for this Timestep
            SetMedFunctions(Biomass)

            setpred(Biomass)

            Array.Clear(Eatenof, 0, Eatenof.Length)
            Array.Clear(Eatenby, 0, Eatenof.Length)

            Dwe = 0.5

            'set ecosim nutrients
            NutBiom = 0
            For i = 1 To m_Data.NGroups
                NutBiom = NutBiom + Biomass(i)
            Next

            NutFree = CSng(m_SimData.NutTot * RelProdScaler - NutBiom)
            If NutFree < m_SimData.NutMin Then NutFree = m_SimData.NutMin

            '*************
            'Consumpt is NOT threadsafe
            '***********
            'If m_SimData.IndicesOn Then
            '    ReDim m_SimData.Consumpt(m_Data.NGroups, m_Data.NGroups)
            'End If

            'ToDetritus() was just created so no need to clear it out
            'For j = m_Data.nLiving + 1 To m_Data.NGroups
            '    ToDetritus(j - m_Data.nLiving) = 0
            'Next j

            SetRelaSwitch(Biomass)

            'get first estimate of denominators of predation rate disc equations
            Dim ia As Integer
            Dim Vbiom() As Single = New Single(m_SimData.Narena) {}
            Dim Vdenom() As Single = New Single(m_SimData.Narena) {}
            'ReDim Vbiom(m_SimData.Narena), Vdenom(m_SimData.Narena)

            'this requires first estimates of vulnerable biomasses Vbiom by foraging arena
            For ii = 1 To inLinks
                i = ilink(ii) : j = jlink(ii) : ia = ArenaLink(ii)

                'aeff(ii) = m_Data.Aspace(ii) * Ftime(j) * RelaSwitch(ii) * EatEff(j) * VulPred(i)
                aeff(ii) = m_Data.Aspace(ii) * Ftime(j) * RelaSwitch(ii) * VulPred(i)
                Veff(ia) = m_Data.Vspace(ia) * Ftime(i)
                ApplyAVmodifiers(aeff(ii), Veff(ia), i, m_SimData.Jarena(ia), False, iRow, iCol)  '?not sure this will work right with multiple preds in arenas
                Vdenom(ia) = Vdenom(ia) + aeff(ii) * pred(j) / Hden(j) / EatEff(j)
            Next

            'then calculate first estimate using initial Hden estimates of vulnerable biomass in each arena
            For ia = 1 To m_SimData.Narena
                i = m_SimData.Iarena(ia)
                If m_SimData.BoutFeeding Then
                    If Vdenom(ia) > 0 Then
                        Vbiom(ia) = Veff(ia) * Biomass(i) * (1 - CSng(Math.Exp(-Vdenom(ia)))) / Vdenom(ia)
                    Else
                        Vbiom(ia) = Veff(ia) * Biomass(i)
                    End If
                Else
                    Vbiom(ia) = Veff(ia) * Biomass(i) / (m_Data.Vspace(ia) + Veff(ia) + Vdenom(ia))
                End If
            Next

            'then update hden estimates based on new vulnerable biomass estimates
            For ii = 1 To inLinks
                j = jlink(ii)
                ia = ArenaLink(ii)
                Hdent(j) = Hdent(j) + aeff(ii) * Vbiom(ia)
            Next

            For j = 1 To m_Data.NGroups
                Hden(j) = (1 - Dwe) * (1 + m_SimData.Htime(j) * Hdent(j)) + Dwe * Hden(j)
            Next

            'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
            'then update vulnerable biomass estimates using new Hden estimates (THIS MAY NOT BE NECESSARY?)
            ReDim Vbiom(m_SimData.Narena), Vdenom(m_SimData.Narena)
            For ii = 1 To inLinks
                i = ilink(ii) : j = jlink(ii) : ia = ArenaLink(ii)
                Vdenom(ia) = Vdenom(ia) + aeff(ii) * pred(j) / Hden(j) / EatEff(j)
            Next
            For ia = 1 To m_SimData.Narena
                i = m_SimData.Iarena(ia)
                If m_SimData.BoutFeeding Then
                    If Vdenom(ia) > 0 Then
                        Vbiom(ia) = Veff(ia) * Biomass(i) * (1 - CSng(Math.Exp(-Vdenom(ia)))) / Vdenom(ia)
                    Else
                        Vbiom(ia) = Veff(ia) * Biomass(i)
                    End If
                Else
                    Vbiom(ia) = Veff(ia) * Biomass(i) / (m_Data.Vspace(ia) + Veff(ia) + Vdenom(ia))
                End If
            Next
            'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

            'then predict consumption flows and cumulative consumptions using the new Vbiom estimates
            For ii = 1 To inLinks
                i = ilink(ii) : j = jlink(ii) : ia = ArenaLink(ii)
                If m_SimData.TrophicOff Then Bprey = m_SimData.StartBiomass(i) Else Bprey = Biomass(i)

                Select Case m_SimData.FlowType(i, j) 'prey always first
                    Case 1 'donor controlled flow
                        eat = aeff(ii) * Bprey
                    Case 3 'limited total flow
                        eat = aeff(ii) * Bprey * pred(j) / (1 + aeff(ii) * pred(j) * Bprey / m_SimData.maxflow(i, j))
                    Case 2 'prey limited flow
                        eat = aeff(ii) * Vbiom(ia) * pred(j) / Hden(j)
                    Case Else
                        eat = 0
                End Select

                Eatenof(i) = Eatenof(i) + eat
                Eatenby(j) = Eatenby(j) + eat

                If Me.m_PathData.isEcospaceModelCoupled Then
                    'predation mort by link
                    m_Data.MPred(iRow, iCol, ii) = eat / (Bprey + 1.0E-20F)
                End If

                '******** 
                'THIS NEEDS TO CHANGE FOR THREADED STUFF
                '**********
                'If m_SimData.IndicesOn Then m_SimData.Consumpt(i, j) = m_SimData.Consumpt(i, j) + eat

                'jb 
                If m_TracerData.EcoSpaceConSimOn = True Then
                    ' Debug.Assert(False, "Contaminant tracing not implemented in Ecospace")
                    'jb ConKtrophic will need to be local it is the rate of comsumption per unit of prey
                    If Biomass(i) > 0 Then m_ConTracer.ConKtrophic(ii) = eat / Biomass(i) Else m_ConTracer.ConKtrophic(ii) = 0
                End If

            Next

            'Make the detritus calculations here:
            Me.SimDetritusMT(Biomass, EffortSpace, Eatenby, ToDetritus, GroupDetritus)

            For i = 1 To m_Data.NGroups

                Eatenby(i) = Eatenby(i) + m_SimData.QBoutside(i) * Biomass(i)

                If i <= m_Data.nLiving Then      'Living group
                    Pmult = 1.0#
                    ApplyAVmodifiers(Pmult, Veff(1), i, i, False, iRow, iCol)
                    'pbb becomes pbmaxs= pb times a max increase factor = pbm for consumers
                    pbb(i) = Pmult * EatEff(i) * m_SimData.PBmaxs(i) * NutFree / (NutFree + m_SimData.NutFreeBase(i)) * m_SimData.pbm(i) / (1 + Biomass(i) * PbSpace(i)) * EatEff(i)

                    loss(i) = Eatenof(i) + (m_SimData.mo(i) * (1 - m_SimData.MoPred(i) + m_SimData.MoPred(i) * Ftime(i)) + m_PathData.Emig(i) + FishTime(i)) * Biomass(i)

                    'on the use of variable GE CJW wrote to VC on 041210: just need to modify derivt to calculate GE for each time step
                    'from GE=0.6Z/(Z+3K*), where Z=loss/B, in the last loop over groups.  That calculation will automatically be overwritten
                    '(dB/dt from it is ignored anyway) for split groups, so not worth avoiding doing it for them.
                    If m_SimData.UseVarPQ And m_PathData.vbK(i) > 0 Then
                        SimGEt = m_SimData.AssimEff(i) * loss(i) / Biomass(i) / (loss(i) / Biomass(i) + 3 * m_PathData.vbK(i))
                    Else
                        SimGEt = SimGE(i) 'm_SimData.SimGE(i)
                    End If

                    Flowin(i) = m_PathData.Immig(i) + SimGEt * Eatenby(i) + pbb(i) * Biomass(i)

                    If Biomass(i) > 1.0E-20 Then
                        FlowoutRate(i) = loss(i) / Biomass(i)
                    Else
                        FlowoutRate(i) = 100
                    End If

                Else 'i <= m_Data.nLiving
                    'Detritus(group)

                    loss(i) = Eatenof(i) + m_PathData.Emig(i) + m_SimData.DetritusOut(i) * Biomass(i)
                    'deriv(i) = Immig(i) + ToDetritus(i - n) - loss(i)
                    If loss(i) <> 0 And Biomass(i) > 0 Then
                        'biomeq(i) = (Immig(i) + ToDetritus(i - n)) / (loss(i) / Biomass(i))
                        DtImpMult = 1
                        ApplyAVmodifiers(DtImpMult, 0, i, i, True, iRow, iCol)

                        Flowin(i) = (m_PathData.Immig(i) * DtImpMult + ToDetritus(i - m_Data.nLiving))
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

        Catch ex As Exception
            Throw New ApplicationException(Me.ToString & ".derivtRed() Error: " & ex.Message)
        End Try
    End Sub
    '***********************
    'THIS FUNCTION IS COPIED FROM cEcoSimModel.vb
    'Changes here will NOT copy over to there
    '***********************
    ''' <summary>
    ''' Sets RelaSwitch() 
    ''' </summary>
    ''' <param name="B">Biomass at this time step for this spatial cell</param>
    ''' <remarks>Sets RelaSwitch() using local B() and  Ecosim.A(), Ecosim.SwitchPower(), Ecosim.BaseTimeSwitch()  </remarks>
    Sub SetRelaSwitch(ByVal B() As Single)     'Switching
        Dim i As Integer, j As Integer, ii As Integer

        Dim PredDen() As Double = New Double(m_Data.NGroups) {}
        RelaSwitch = New Single(inLinks) {}

        For ii = 1 To inLinks
            i = ilink(ii) : j = jlink(ii)
            PredDen(j) = PredDen(j) + A(i, j) * B(i) ^ SwitchPower(j)
        Next
        For ii = 1 To inLinks
            i = ilink(ii) : j = jlink(ii)
            If m_SimData.SwitchPower(j) = 0.0# Then
                RelaSwitch(ii) = 1
            Else
                RelaSwitch(ii) = CSng(A(i, j) * B(i) ^ SwitchPower(j) / (PredDen(j) + 1.0E-20) / m_SimData.BaseTimeSwitch(ii))
            End If
        Next

    End Sub
    '***********************
    'THIS FUNCTION IS COPIED FROM cEcoSimModel.vb
    'Changes here will NOT copy over to there
    '***********************
    Sub setpred(ByVal Biomass() As Single)
        'Routine modified 290597 VC to follow ESimII
        Dim i As Integer ', ii As Integer
        'set predator abundance measure used for predation
        'rate calculations; this is just biomass for
        'simple pools, or predator numbers for pools that
        'are split into Juv-Adult pairs
        'note below that biomass(ii) for ii>n contains
        'numbers in pools iad, iju rather than biomasses
        For i = 1 To m_Data.NGroups
            'If i > N And biomass(i) = 0 Then biomass(i) = 1
            If Biomass(i) < 1.0E-20 Or Single.IsNaN(Biomass(i)) Then Biomass(i) = 1.0E-20 '0.00000001
            If m_SimData.NoIntegrate(i) >= 0 Then pred(i) = Biomass(i)
            If Single.IsNaN(pred(i)) Then pred(i) = 1.0E-20
        Next

    End Sub

    '***********************
    'THIS FUNCTION IS COPIED FROM cEcoSimModel.vb
    'Changes here will NOT copy over to there
    '***********************
    ''' <summary>
    ''' Sets MedVal() mediation value used to modify a or v. Local version for thread safety.
    ''' </summary>
    ''' <param name="Biom"></param>
    ''' <remarks>MedVal(nmediationshapes) is used in ApplyAVmodifiers()</remarks>
    Sub SetMedFunctions(ByVal Biom() As Single)
        'called from derivt, derivtred if MedIsUsed(0)=true to set
        'current Y value of each active trophic mediation function
        If m_SimData.BioMedData.MedIsUsed(0) Then

            'Calculate B/BRatio
            For i As Integer = 1 To m_Data.NGroups
                BBRatio(i) = Biom(i) / (m_Data.BRatio(i) + 1.0E-20F)
            Next

            'Calculate MedVal() from the B/BRatio
            Me.m_SimData.BioMedData.SetMedFunctions(BBRatio, Me.EffortSpace, 0, Me.MedVal)
        End If

    End Sub

    '***********************
    'THIS FUNCTION IS COPIED from cEcosimModel.vb
    'Changes here will NOT be copy over to there
    '***********************
    ''' <summary>
    ''' Apply the multi function mediation functions/modifiers to 'a'(searchrate) and 'v'(vulnerability)
    ''' uses MedVal(NMediationShapes) to modify A and/or V
    ''' </summary>
    ''' <param name="A">SearchRate to modify</param>
    ''' <param name="v">Vulnerability to modify</param>
    ''' <param name="i">i Index (Prey)</param>
    ''' <param name="j">j Index (Pred)</param>
    ''' <param name="UseTime">True if the modifier is over time (Ecosim), False if not (Ecospace) </param>
    ''' <remarks>
    ''' THREADING:  MedVal() is set to the mediating value based on biomass for each map cell at each time step via cSpaceSolver.SetMedFunctions().
    ''' It is unique to this thread/cell/time-step. It was moved here to make it thread safe.
    '''</remarks>
    Sub ApplyAVmodifiers(ByRef A As Single, ByRef v As Single, ByVal i As Integer, ByVal j As Integer, ByVal UseTime As Boolean, ByVal iRow As Integer, ByVal iCol As Integer)
        Dim K As Integer, Mult As Single
        'VC Hobart Sep 2008. Added row and col numbers to the call to this routine, as they are needed for spatial fields

        'VC Hobart Sep 2008. Adding temperature and salinity fields to Ecospace,
        'for now it's just readable in code, we'll need interface and database handling as well
        If m_Data.SpatialFieldsInUse Then
            For iSF As Integer = 1 To m_Data.nSpatialFields
                m_Ecosim.ApplySalinityModifier(A, m_Data.SpatialField(iRow, iCol, j), _
                                               m_Data.SpatialFieldOptimum(j, iSF), _
                                               m_Data.SpatialFieldStdLeft(j, iSF), _
                                               m_Data.SpatialFieldStdRight(j, iSF))
            Next
        End If

        For K = 1 To cMediationDataStructures.MAXFUNCTIONS

            If m_SimData.BioMedData.FunctionNumber(i, j, K) = 0 Then Exit Sub

            If m_SimData.BioMedData.IsMedFunction(i, j, K) Then
                Mult = MedVal(m_SimData.BioMedData.FunctionNumber(i, j, K))
            Else
                Mult = 1
                'If UseTime = True Then Mult = m_ESData.tval(m_ESData.FunctionNumber(i, j, K)) Else Mult = 1
            End If

            Select Case m_SimData.BioMedData.FunctionType(i, j, K)
                'SearchRate, Production and ImportedDetritus are all applied to the A multiplier
                Case eForcingFunctionApplication.SearchRate, _
                     eForcingFunctionApplication.ProductionRate
                    A = A * Mult
                Case eForcingFunctionApplication.Vulnerability
                    v = v * Mult
                Case eForcingFunctionApplication.ArenaArea
                    A = A / (Mult + 1.0E-10F)
                Case eForcingFunctionApplication.VulAndArea
                    A = A / (Mult + 1.0E-10F)
                    v = v * Mult
                Case eForcingFunctionApplication.Import
                    A = A * Mult
            End Select

        Next

    End Sub

    ''' <summary>
    ''' Accumulate the fisheries data (catch) for a single group for this map cell. 
    ''' This is called before DerivtRed(), in the time step, so it is the condition at the start of the time step.
    ''' </summary>
    ''' <param name="Biomass">Biomass for all the groups at this time step</param>
    ''' <param name="iRow">Map row</param>
    ''' <param name="iCol">Map col</param>
    ''' <remarks></remarks>
    Public Sub accumCatchData(ByVal iCumTime As Integer, ByVal iYear As Integer, ByVal Biomass() As Single, ByVal FMortByGroup() As Single, ByVal iRow As Integer, ByVal iCol As Integer)
        Dim cellCatch As Single, iFlt As Integer, iGrp As Integer
        Dim st As Double = Me.m_stpWatch.Elapsed.TotalSeconds

        Try
            'FishRateGear(iFlt, 0) contains the effort for this cell at this timestep
            For iFlt = 1 To m_Data.nFleets

                'Effort
                Me.ResultsByFleet(eSpaceResultsFleets.FishingEffort, iFlt) += Me.EffortSpace(iFlt, 0)
                'SailingEffort: at this point SailingEffort is  sum of [fishing effort] * [effort of fishing each cell (Sail(iFlt, iRow, iCol))] /  SailScale(ifleet)
                'Effort of fishing all the cells
                Me.ResultsByFleet(eSpaceResultsFleets.SailingEffort, iFlt) += (Me.EffortSpace(iFlt, 0) * m_Data.Sail(iFlt, iRow, iCol) / m_Data.SailScale(iFlt))

                'sum values into All Fleets 0 index 
                Me.ResultsByFleet(eSpaceResultsFleets.FishingEffort, 0) += Me.ResultsByFleet(eSpaceResultsFleets.FishingEffort, iFlt)
                Me.ResultsByFleet(eSpaceResultsFleets.SailingEffort, 0) += Me.ResultsByFleet(eSpaceResultsFleets.SailingEffort, iFlt)

            Next

            For iGrp = 1 To Me.m_Data.NGroups
                If Me.m_PathData.fCatch(iGrp) > 0 Then
                    'jb 29-Jan-12 in the multithreaded version FishTime was not updated to the F for this cell
                    'use fishing mortality rate passed in instead 
                    'Dim bCatch As Single = Biomass(igrp) * m_SimData.FishTime(igrp) * m_Data.Width(iRow)
                    Dim bCatch As Single = Biomass(iGrp) * FMortByGroup(iGrp) ' * m_Data.Width(iRow)
                    Me.ResultsByGroup(eSpaceResultsGroups.CatchBio, iGrp) += bCatch '= m_Data.ResultsByGroup(eSpaceResultsGroups.CatchBio, igrp, iCumTime) + bCatch
                    m_Data.CatchMap(iRow, iCol, iGrp) += bCatch
                    'Next value of catch, depends on what gear was used:
                    For iFlt = 1 To Me.m_PathData.NumFleet

                        'Debug.Assert(isFished(iFlt, iRow, iCol) = Me.m_Data.IsFished(iFlt, iRow, iCol), "isFished() != isFished() Really!")
                        'Is this cell fished by this fleet
                        'If isFished(iFlt, iRow, iCol) Then
                        If Me.m_Data.IsFished(iFlt, iRow, iCol) Then

                            'Is this group caught by this fleet (Landing(iFlt, iGrp) + Discard(iFlt, iGrp) > 0)
                            'If isCaught(iFlt, iGrp) Then
                            If Me.m_PathData.Landing(iFlt, iGrp) + Me.m_PathData.Discard(iFlt, iGrp) > 0 Then
                                'First get catch
                                cellCatch = Biomass(iGrp) * Me.EffortSpace(iFlt, 0) * m_SimData.relQ(iFlt, iGrp) ' * m_Data.Width(iRow)

                                'Sum the total catch by gear
                                Me.ResultsByFleet(eSpaceResultsFleets.CatchBio, iFlt) += cellCatch
                                'sum all fleets
                                Me.ResultsByFleet(eSpaceResultsFleets.CatchBio, 0) += cellCatch

                                Me.ResultsByFleetGroup(eSpaceResultsFleetsGroups.CatchBio, iFlt, iGrp) += cellCatch
                                'sum all fleets into the zero fleet index
                                Me.ResultsByFleetGroup(eSpaceResultsFleetsGroups.CatchBio, 0, iGrp) += cellCatch

                                'Next line is for adding up catch by region etc
                                If m_Data.nRegions >= 1 Then
                                    Dim iRgn As Integer = m_Data.Region(iRow, iCol)
                                    If (iRgn > m_Data.nRegions) Then iRgn = 0
                                    Me.ResultsCatchRegionGearGroup(iRgn, iFlt, iGrp) += cellCatch
                                End If

                                Me.Landings(iGrp, iFlt) += cellCatch * Me.m_PathData.PropLanded(iFlt, iGrp)
                                Me.m_Data.DiscardsMap(iRow, iCol, iGrp) += cellCatch * (1 - Me.m_PathData.PropLanded(iFlt, iGrp))
                            End If 'Me.m_PathData.Landing(iFlt, iGrp) + Me.m_PathData.Discard(iFlt, iGrp)
                        End If 'Me.m_Data.IsFished(iFlt, iRow, iCol)
                    Next iFlt
                End If 'If m_EPdata.fCatch(igrp) > 0 Then

            Next iGrp

        Catch ex As Exception
            m_logger.LogError(Me.ToString & ".accumCatchData() Error: " & ex.Message)
        End Try
        Me.CatchCPUTimeSec += Me.m_stpWatch.Elapsed.TotalSeconds - st

    End Sub

    Public Sub New(ByVal ThreadNumber As Integer)

        ThreadID = ThreadNumber

        m_ConTracer = New cContaminantTracer
        'create a new tracer data structure
        'this will get a copy of the data that has been initialized by the database in cEcospace.InitSpaceSolverThreads()
        m_TracerData = New cContaminantTracerDataStructures

    End Sub

    'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
    'FOR DEGUGGING ONLY HAS NOT BEEN TESTED
    'SimDetritus from Ecosim moved here to see if calling Ecosim is slowing down the threading
    Public Sub SimDetritusMT(ByVal Biomass() As Single, ByVal FishRateGear(,) As Single, ByVal Eatenby() As Single, ByRef ToDetritus() As Single, ByRef DetritusByGroup() As Single)
        ' Dim Surplus As Single
        Dim i As Integer, j As Integer, K As Integer
        Dim ToDet As Single, DetFlowN As Single
        DetFlowN = 0

        'DetritusByGroup() needs to be cleared because the values are summed into it
        Array.Clear(DetritusByGroup, 0, Me.m_Data.NGroups)

        For i = 1 To m_PathData.NumLiving
            For j = m_PathData.NumLiving + 1 To Me.m_Data.NGroups
                'First take egestion
                ToDet = m_PathData.GS(i) * Eatenby(i) * m_PathData.DF(i, j - m_PathData.NumLiving)
                'Add dying organisms
                ToDet = ToDet + m_SimData.mo(i) * Biomass(i) * m_PathData.DF(i, j - m_PathData.NumLiving)

                'If m_PathData.NumFleet > 0 Then     'Only if there is fishery
                For K = 1 To m_PathData.NumFleet
                    'jb 07-Jan-2010 Changed to use Propdiscardtime(fleets,groups) (% discarded for this time step) initialized to ecopath PropDiscard() or set in MSE.RegulateEffort() 
                    'discard mort is included in Propdiscardtime() by initialization and MSE 
                    DetFlowN = m_PathData.DiscardFate(K, j - m_PathData.NumLiving) * Biomass(i) * FishRateGear(K, 0) * m_SimData.FishMGear(K, i) * Me.m_SimData.Propdiscardtime(K, i)
                    ToDet = ToDet + DetFlowN

                    If m_TracerData.EcoSimConSimOn = True Then
                        m_ConTracer.ConKdet(i, j, K) = DetFlowN / Biomass(i)
                    End If

                Next K
                'End If 'If m_PathData.NumFleet > 0 Then    

                ToDetritus(j - m_PathData.NumLiving) = ToDetritus(j - m_PathData.NumLiving) + ToDet

                DetritusByGroup(i) += ToDet

            Next j
        Next i

        'Next add flow from other detritus groups
        For i = m_PathData.NumLiving + 1 To Me.m_Data.NGroups
            For j = m_PathData.NumLiving + 1 To Me.m_Data.NGroups
                If i <> j Then ToDetritus(j - m_PathData.NumLiving) = ToDetritus(j - m_PathData.NumLiving) + m_PathData.DetPassedProp(i) * Biomass(i) * m_PathData.DF(i, j - m_PathData.NumLiving)
            Next
        Next

        'If m_SimData.FirstTime = True Then
        '    For i = m_PathData.NumLiving + 1 To Me.m_Data.NGroups
        '        m_SimData.DetritusOut(i) = (ToDetritus(i - m_PathData.NumLiving) - m_PathData.BA(i) + m_PathData.Immig(i) - EatenOf(i)) / Biomass(i) - m_SimData.Emig(i)
        '        'DetritusOut(i) = (ToDetritus(i - mEPData.NumLiving) - BA(i) + DetPassedOn(i) + EX(i) + Immig(i) - Eatenof(i)) / Biomass(i) - Emig(i)
        '    Next i
        'End If
        m_SimData.FirstTime = False

    End Sub
    'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

    Private Sub copyCoreToLocal(SourceMap(,,) As Single, ByRef DestinationLinear(,) As Single)
        Dim i As Integer
        Dim n As Integer = iLstCell - iFrstCell + 1
        Dim ngrps As Integer = Me.m_Data.NGroups

        DestinationLinear = New Single(ngrps, n) {}
        '  ReDim DestinationLinear(ngrps, n)

        For iCell As Integer = iFrstCell To iLstCell
            For igrp As Integer = 1 To ngrps
                DestinationLinear(igrp, i) = SourceMap(m_Data.iWaterCellIndex(iCell), m_Data.jWaterCellIndex(iCell), igrp)
            Next
            i += 1
        Next iCell

    End Sub

    Private Sub copyLocalToCore(SourceLinear(,) As Single, ByRef DestinationMap(,,) As Single)
        Dim i As Integer
        Dim n As Integer = iLstCell - iFrstCell + 1
        Dim ngrps As Integer = Me.m_Data.NGroups

        For iCell As Integer = iFrstCell To iLstCell
            For igrp As Integer = 1 To ngrps
                DestinationMap(m_Data.iWaterCellIndex(iCell), m_Data.jWaterCellIndex(iCell), igrp) = SourceLinear(igrp, i)
            Next
            i += 1
        Next iCell

    End Sub

#End Region

End Class

#End If

#End Region
