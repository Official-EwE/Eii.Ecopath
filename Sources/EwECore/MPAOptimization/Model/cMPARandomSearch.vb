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

Option Strict On
Imports System.Math
Imports EwEUtils.Core
Imports System.IO
Imports EwEUtils.Utilities

Public Class cMPARandomSearch
    Implements IMPASearchModel

#Region "Private data"

    Private LayerSumInMPA() As Single
    Private MaxLayerSumByLayerAndPctMPA(,) As Single
    Private LayerInclusion(,,) As Single

    Const N_MAX_RESULTS As Integer = 500
    Const RESULTS_TO_KEEP As Integer = N_MAX_RESULTS \ 2

    Private m_EcoSpace As cEcoSpace
    Friend m_SpaceData As cEcospaceDataStructures

    Private m_data As cMPAOptDataStructures
    Private m_search As cSearchDatastructures

    Private m_bRunning As Boolean
    Private m_esStartTime As Single
    Private EcoSeedOn As Boolean

    'results of each iterations
    Private m_lstObjectiveResults As New List(Of cObjectiveResult)

    Private m_cellComputedCallback As cMPAOptManager.SearchIterationDelegate
    Private m_StateCallback As cMPAOptManager.SearchRunStateDelegate
    Private m_SendMessageDelegate As cMPAOptManager.SendMessageDelegate

    '''' <summary>Best results of the current run</summary>
    'Private m_bestResults As cObjectiveResult

    Private CumulativeCellWeight() As Double
    Private CellCount As Integer
    Private m_nIters As Integer 'number of iteration completed

    ' -- Autosave settings --

    ''' <summary>Auto-save file name.</summary>
    Private Const c_FILENAME As String = "MPAOpt_random_Output.csv"
    ''' <summary>Flag, stating whether autosave is enabled.</summary>
    Private m_bAutosaveResults As Boolean = False
    ''' <summary>Auto-save folder.</summary>
    Private m_strOutputPath As String = ""
    ''' <summary>Auto-save file header.</summary>
    Private m_strHeader As String = ""

#Region "Modeling data from EwE5"

    Private BOrig(,,) As Single
    Private FOrig(,,) As Single
    Private WOrig(,,) As Single
    Private TimesCalled As Long
    Private Blastseed(,,) As Single

    Public StoreBtimeForEcoSeed() As Single

    Private TotWeightedValueBase As Single
    Private EmployBase As Single, TotValBase As Single, ManValueBase As Single, EcoValueBase As Single, KemptonsBase As Single, AreaBoundBase As Single
    Private TargetSumMax As Single

    Private AreaBoundary As Single


#End Region

#End Region

#Region "Construction and Initialization"

    Public Function Init(ByRef EcoSpaceModel As cEcoSpace, ByRef MPAOptData As cMPAOptDataStructures) As Boolean Implements IMPASearchModel.Init

        Try

            m_EcoSpace = EcoSpaceModel
            m_data = MPAOptData

            m_SpaceData = m_EcoSpace.EcoSpaceData
            m_search = m_EcoSpace.SearchData

            'set EcoSpace to use this MPA optimization model
            m_EcoSpace.MPAOptimization = Me

            'the seed array can be needed before the model is run
            ReDim m_data.MPASeed(m_SpaceData.InRow + 1, m_SpaceData.InCol + 1)

        Catch ex As Exception
            cLog.Write(ex)
            Return False
        End Try

        Return True

    End Function

    Public Sub Connect(ByVal OnSearchInteration As cMPAOptManager.SearchIterationDelegate, _
                       ByVal OnRunStateChanged As cMPAOptManager.SearchRunStateDelegate, _
                       ByVal OnSendMessage As cMPAOptManager.SendMessageDelegate) Implements IMPASearchModel.Connect
        m_cellComputedCallback = OnSearchInteration
        m_StateCallback = OnRunStateChanged
        m_SendMessageDelegate = OnSendMessage
    End Sub


#End Region

#Region "Public Properties and Methods"

    Public Property MPAOptData() As cMPAOptDataStructures Implements IMPASearchModel.MPAOptData
        Get
            Return m_data
        End Get
        Set(ByVal value As cMPAOptDataStructures)
            m_data = value
        End Set
    End Property

    Public ReadOnly Property EcospaceStartTime() As Single Implements IMPASearchModel.EcospaceStartTime
        Get

            If Not m_bRunning Then
                'this got called even though Ecoseed is not running this should NOT happen
                'Oh well return zero this should be the default start time for ecospace
                Return 0
            End If

            If TimesCalled > 1 Then
                'if Ecoseed has already run Ecospace 
                'then start the time loop at the start of the first summary time period
                'This should change to Ecoseed having its own start and end time instead of using the the summary time periods
                Return Me.m_data.EcoSpaceStartYear
            Else
                'This is the first time Ecoseed will run Ecospace
                'Ecospace needs to run for the entire time period to set the base values
                Return 0
            End If

        End Get
    End Property


    Public ReadOnly Property isRunning() As Boolean Implements IMPASearchModel.isRunning
        Get
            Return Me.m_bRunning
        End Get
    End Property

    Public Sub StopRun() Implements IMPASearchModel.StopRun
        m_data.StopRun = True
    End Sub

    Public Sub clearMPAs() Implements IMPASearchModel.clearMPAs
        For ir As Integer = 1 To m_SpaceData.InRow
            For ic As Integer = 1 To m_SpaceData.InCol
                m_SpaceData.MPA(ir, ic) = 0
            Next ic
        Next ir
    End Sub

    Public Sub clearSeedCells() Implements IMPASearchModel.clearSeedCells
        For ir As Integer = 1 To m_SpaceData.InRow
            For ic As Integer = 1 To m_SpaceData.InCol
                m_data.MPASeed(ir, ic) = 0
            Next ic
        Next ir
    End Sub


    Public Function setAllCellsToMPA(ByVal iMPA As Integer) As Boolean Implements IMPASearchModel.setAllCellsToMPA

        'make sure the MPA index supplied by the user is in bounds
        If iMPA > 0 And iMPA <= m_SpaceData.MPAno Then
            For ir As Integer = 1 To m_SpaceData.InRow
                For ic As Integer = 1 To m_SpaceData.InCol
                    m_SpaceData.MPA(ir, ic) = iMPA
                Next ic
            Next ir
            Return True
        Else
            'invalid MPA index
            Return False
        End If

    End Function

    Public Function setAllCellsToSeed(ByVal iMPA As Integer) As Boolean Implements IMPASearchModel.setAllCellsToSeed

        'make sure the MPA index supplied by the user is in bounds
        If iMPA > 0 And iMPA <= m_SpaceData.MPAno Then
            For ir As Integer = 1 To m_SpaceData.InRow
                For ic As Integer = 1 To m_SpaceData.InCol
                    m_data.MPASeed(ir, ic) = iMPA
                Next ic
            Next ir
            Return True
        Else
            'invalid MPA index
            Return False
        End If
    End Function


    Public ReadOnly Property Results() As System.Collections.Generic.List(Of cObjectiveResult) Implements IMPASearchModel.Results
        Get
            Return Me.m_lstObjectiveResults
        End Get
    End Property

    Public ReadOnly Property nInterationCompleted() As Integer Implements IMPASearchModel.nInterationsCompleted
        Get
            Return Me.m_nIters
        End Get
    End Property

    Public ReadOnly Property OKtoRun() As Boolean Implements IMPASearchModel.OKtoRun
        Get
            'the random search can always run 
            'thats quite a statment...
            Return True
        End Get
    End Property

    ''' <inheritdocs cref="IMPASearchModel.ConfigureAutosave"/>
    Public Sub ConfigureAutosave(ByVal bAutosave As Boolean, ByVal strOutputPath As String, ByVal strHeader As String) _
        Implements IMPASearchModel.ConfigureAutosave
        Me.m_bAutosaveResults = bAutosave
        Me.m_strOutputPath = strOutputPath
        Me.m_strHeader = strHeader
    End Sub

#End Region

#Region "Running the model"

    Private Sub initForRun()

        Try

            'Ecoseed does not listen to the Ecospace time steps
            Me.m_EcoSpace.TimeStepDelegate = Nothing

            'create a new list to store the results
            m_lstObjectiveResults = New List(Of cObjectiveResult)
            TargetSumMax = 0

            'Clear out any values from a previous ecoseed run
            m_data.Clear()

            RedimSeedVariables()

        Catch ex As Exception
            Me.WriteError(ex)
            Throw New ApplicationException(Me.ToString & ".initForRun() Error: " & ex.Message, ex)
        End Try

    End Sub


    Public Sub Run() Implements IMPASearchModel.Run

        Try

            Me.m_bRunning = True
            Me.setRunState(cMPAOptManager.eRunStates.Initializing)

            Me.m_data.StopRun = False

            Me.runSearch()

        Catch ex As Exception
            Me.WriteError("MPA Optimizatoin Random Search Error")
            Debug.Assert(False, ex.StackTrace)
        End Try

        Me.m_bRunning = False
        Me.setRunState(cMPAOptManager.eRunStates.Completed)

    End Sub


    Friend Sub runSearch()
        'VC changes
        'Main loop for running the Random MPA optimization

        Dim StoreOptimalPct As Single = 1 'from GUI
        Dim MinimalEvaluationValue As Single = 0
        Dim writer As StreamWriter = Nothing

        If Me.m_bAutosaveResults Then
            If cFileUtils.IsDirectoryAvailable(Me.m_strOutputPath, True) Then
                Try
                    writer = New StreamWriter(Path.Combine(Me.m_strOutputPath, c_FILENAME))
                Catch ex As Exception

                End Try
            End If
        End If

        Try
            Debug.Assert(m_data IsNot Nothing, "Ecoseed: data not initialized")
            Debug.Assert(m_EcoSpace IsNot Nothing, "Ecoseed: Ecospace not initialized")
            System.Console.WriteLine("-----------MPA Random Search --------------")

            Me.initForRun()
            m_search.SearchMode = eSearchModes.SpatialOpt
            m_search.setMinSearchBlocks()
            Me.getBaseValues()

            Me.WriteOutputFileHeader(writer)

            CalculateCellWeightings()

            Dim iR As Integer = m_SpaceData.InRow
            Dim iC As Integer = m_SpaceData.InCol
            'we don't want to clear all data cells, only the one with the currently selected MPA
            'Array.Clear(Me.m_SpaceData.MPA, 0, Me.m_SpaceData.MPA.Length)
            For i As Integer = 1 To iR
                For j As Integer = 1 To iC
                    If m_SpaceData.MPA(i, j) = m_data.iMPAtoUse Then m_SpaceData.MPA(i, j) = 0
                Next
            Next

            'We need number of potential MPA cells, this is watercells 
            '  - (cells which are either not an MPA 
            '    or which already are the same kind of MPA.)

            Dim CellCount As Integer
            For i As Integer = 1 To iR
                For j As Integer = 1 To iC
                    If m_SpaceData.Depth(i, j) > 0 And (m_SpaceData.MPA(i, j) = m_data.iMPAtoUse Or m_SpaceData.MPA(i, j) = 0) Then CellCount += 1
                Next
            Next

            'Get the layer weights by percentage MPA coverage
            sortLayersByCellWeight(CellCount)
            'vc hack:
            'Using sw As StreamWriter = New StreamWriter("c:\RandomMPA.csv", False)  'true makes it append
            '    sw.WriteLine("Protected, obj.function, layers1, layer 2, etc")
            '    sw.Close()
            'End Using

            Dim StoreNo As Integer = CInt(StoreOptimalPct * m_data.nIterations / 100)

            'Step from Min area(%) (= integer) to Max area(%) (= integer) stepsize = Step (%) (=integer)
            Dim iStep As Integer = CInt((-m_data.MinArea + m_data.MaxArea) / m_data.stepSize)

            Debug.Assert(m_data.iMPAtoUse > 0, "Current MPA not set!!!.")

            Dim nStep As Integer = 0

            Me.setRunState(cMPAOptManager.eRunStates.Searching)

            m_nIters = 0

            'Dim NoRuns As Integer = CInt((m_data.MaxArea - m_data.MinArea) / m_data.stepSize * m_data.nIterations)
            'ReDim LayerInclusion (me.m_SpaceData.nImportanceLayers,  

            For iPropMPA As Integer = m_data.MinArea To m_data.MaxArea Step m_data.stepSize
                'keep track of how may times we've stepped: 
                'calculate how many cells that should be closed:
                'this is calculated based on number of water cells - number of other mpsa cells, not total number of cells:
                Dim NumberMPA As Integer = CInt(iPropMPA * CellCount / 100)
                'Dim NumberMPA As Integer = CInt(iPropMPA * m_SpaceData.iTotalWaterCells / 100)

                'Step through and do iterations:
                For iIter As Integer = 1 To m_data.nIterations
                    'select the MPA cells that are to be evaluated in this run
                    Me.selectRandomCells(NumberMPA, m_data.iMPAtoUse)

                    Me.Output()

                    'Run EcoSpace
                    Me.m_EcoSpace.Run()
                    If m_data.StopRun Then Exit For

                    'Evaluate the current MPA cell selection
                    Me.EvaluateRun()

                    'Store LayerSumInMPA
                    calcImportanceLayersCoverageInRun()
                    'vc hack
                    Dim sLayer As String = iPropMPA.ToString & "," & m_data.objFuncTotal.ToString & ","
                    For iL As Integer = 0 To m_SpaceData.nImportanceLayers - 1
                        If MaxLayerSumByLayerAndPctMPA(iL, iPropMPA) > 0 Then
                            sLayer += (LayerSumInMPA(iL) / MaxLayerSumByLayerAndPctMPA(iL, iPropMPA)).ToString & ","
                        End If
                    Next

                    ' Process results
                    Me.StoreObjectiveFunctionResults(writer)
                    ' Next
                    Me.m_nIters += 1

                Next
                If Me.m_data.StopRun Then Exit For
                nStep += 1
            Next

            '  Me.m_lstObjectiveResults.Sort()

            Me.cleanUp()

        Catch ex As Exception
            Me.WriteError(ex)
            Me.m_bRunning = False
            Debug.Assert(False, ex.StackTrace)
        End Try

        If (writer IsNot Nothing) Then
            writer.Flush()
            writer.Close()
            writer.Dispose()
        End If

    End Sub


    Private Sub selectRandomCells(ByVal NumberMPA As Integer, ByVal curMPA As Integer)
        'VC changes
        Dim generator As New Random()   '

        Try

            'clear out the last set of cells
            m_data.ClearCells()

            'VC presume its quicker to load to local value than stepping out many times to get these:
            Dim inRow As Integer = Me.m_SpaceData.InRow '+ 1
            Dim inCol As Integer = Me.m_SpaceData.InCol '+ 1

            'we don't want to clear all data cells, only the one with the currently selected MPA
            'Array.Clear(Me.m_SpaceData.MPA, 0, Me.m_SpaceData.MPA.Length)
            For i As Integer = 1 To inRow
                For j As Integer = 1 To inCol
                    If m_SpaceData.MPA(i, j) = curMPA Then m_SpaceData.MPA(i, j) = 0
                Next
            Next

            'Now start selecting the ones to make MPAs
            Dim iThisCell As Integer
            Dim iC As Integer = 0
            Dim GetOut As Integer = 0

            Dim Rand As New Random() '  Double = generator.NextDouble

            Do While iC < NumberMPA And GetOut < 100 * NumberMPA
                Dim RanVal As Double = Rand.NextDouble
                For i As Integer = 1 To CellCount
                    If CumulativeCellWeight(i) >= RanVal Then iThisCell = i : Exit For
                Next

                'Dim GetRow As Integer = (iThisCell - 1) \ inRow + 1' jb changed
                Dim GetRow As Integer = (iThisCell - 1) \ inCol + 1
                Dim GetCol As Integer = (iThisCell - 1) Mod inCol + 1

                'now we know which cell to close
                'but check that the cell hasn't been made into an mpa already\
                If m_SpaceData.Depth(GetRow, GetCol) > 0 And m_SpaceData.MPA(GetRow, GetCol) = 0 Then
                    m_SpaceData.MPA(GetRow, GetCol) = curMPA
                    System.Console.WriteLine(GetRow.ToString & "  " & GetCol.ToString)
                    m_data.AddCell(GetRow, GetCol, curMPA)
                    iC += 1
                    GetOut = 0
                Else
                    GetOut += 1
                End If
            Loop

        Catch ex As Exception
            Me.WriteError(ex)
            Debug.Assert(False, Me.ToString & ".selectRandomCells() Error: " & ex.Message)
            Throw New ApplicationException(Me.ToString & ".selectRandomCells() Error:", ex)
        End Try

    End Sub


    Private Function EvaluateRun() As Single
        Dim curSum As Single 'results of the search run

        Try

            curSum = m_search.ValWeight(eSearchCriteriaResultTypes.TotalValue) * m_search.totval / TotValBase + _
                     m_search.ValWeight(eSearchCriteriaResultTypes.Employment) * m_search.Employ / EmployBase + _
                     m_search.ValWeight(eSearchCriteriaResultTypes.MandateReb) * m_search.manvalue / ManValueBase + _
                     m_search.ValWeight(eSearchCriteriaResultTypes.Ecological) * m_search.ecovalue / EcoValueBase + _
                     m_search.ValWeight(eSearchCriteriaResultTypes.BioDiversity) * m_search.KemptonQ / KemptonsBase


            'Calculate boundary length/area ratio
            AreaBoundary = CalculateAreaOverBondaryLength()
            curSum = curSum + AreaBoundary * m_data.BoundaryWeight
            m_data.objFuncTotal = (m_search.WeightedTotal + AreaBoundary * m_data.BoundaryWeight) / Me.TotWeightedValueBase

            'calculate the relative values in to data structures 
            'so they can be use to populate the Input/Output object for the interface
            m_data.objFuncEcologicalValue = m_search.ecovalue / EcoValueBase
            m_data.objFuncMandatedValue = m_search.manvalue / ManValueBase
            m_data.objFuncSocialValue = m_search.Employ / EmployBase
            m_data.objFuncEconomicValue = m_search.totval / TotValBase
            m_data.objFuncBiodiversity = m_search.KemptonQ / KemptonsBase
            m_data.objFuncAreaBorder = AreaBoundary / AreaBoundBase

            If curSum > TargetSumMax Then
                'save the best results 
                TargetSumMax = curSum

                Me.setRunState(cMPAOptManager.eRunStates.NewBestResultFound)

            End If

            'keep the results of every search
            Me.m_lstObjectiveResults.Add(New cObjectiveResult(m_data, Me.m_SpaceData))

            ''Memory management for results
            'If Me.m_lstObjectiveResults.Count >= N_MAX_RESULTS Then
            '    'sorts in decending order (biggest objFuncTotal first)
            '    Me.m_lstObjectiveResults.Sort()
            '    'remove lowest results from the end of the list
            '    Me.m_lstObjectiveResults.RemoveRange(RESULTS_TO_KEEP - 1, Me.m_lstObjectiveResults.Count - RESULTS_TO_KEEP)
            'End If

            Return curSum

        Catch ex As Exception
            cLog.Write(ex)
            Debug.Assert(False, Me.ToString & ".EvaluateRun() Error: " & ex.Message)
            Throw New ApplicationException(Me.ToString & ".EvaluateRun() Error:", ex)
        End Try

    End Function



    ''' <summary>
    ''' Public interfaced called by Ecospace at the start of each Year
    ''' </summary>
    ''' <param name="Biomass"></param>
    ''' <param name="iYear"></param>
    ''' <remarks>This is used by Ecoseed to control the length of the Ecospace run</remarks>
    Friend Sub YearTimeStep(ByRef iYear As Integer, ByVal Biomass() As Single) Implements IMPASearchModel.YearTimeStep

        If Not Me.m_bRunning Then
            'Ecoseed is not running so don't do anything
            Exit Sub
        End If

        'jb for now 
        If iYear = Me.m_data.EcoSpaceEndYear Then
            KeepOrReloadCellValues(Biomass)
        ElseIf iYear = Me.m_data.EcoSpaceEndYear Then
            iYear = CInt(m_EcoSpace.EcoSpaceData.TotalTime)
            m_EcoSpace.StopRun() ' = True
        End If


    End Sub


    Private Sub Output()

        Try
            If Me.m_cellComputedCallback IsNot Nothing Then
                m_cellComputedCallback.Invoke()
            End If
        Catch ex As Exception
            Me.WriteError(ex)
            Debug.Assert(False, Me.ToString & ".setRunState() " & ex.Message)
        End Try

    End Sub

    Private Sub dumpSearchValues(ByVal search As cSearchDatastructures)

        'totval = m_search.totval
        'Employ = m_search.Employ
        'manvalue = m_search.manvalue
        'ecovalue = m_search.ecovalue

        System.Console.WriteLine("Total Value = " & search.totval / TotValBase & _
                                    ", Employ Value = " & search.Employ / EmployBase & _
                                    ", Mandated Value = " & search.manvalue / ManValueBase & _
                                    ", Eco Value = " & search.ecovalue / EcoValueBase)
    End Sub


    Public Sub KeepOrReloadCellValues(ByVal biomass() As Single)
        Dim i As Integer, j As Integer, ip As Integer
        'these are not being kept properly ab02182000
        'TimesCalled is reinitialized for each timestep

        'ToDo_jb KeepOrReloadCellValues WchangeVar() is only in the ecospace threads 
        'If this really needs to happen it needs to get copied out of the threads then copied back in?????
        Try
            If TimesCalled = 1 Then 'First time keep the original bcell values

                For i = 1 To m_SpaceData.InRow
                    For j = 1 To m_SpaceData.InCol
                        For ip = 1 To m_SpaceData.NGroups
                            BOrig(i, j, ip) = m_SpaceData.Bcell(i, j, ip)
                            FOrig(i, j, ip) = m_EcoSpace.FtimeCell(i, j, ip)
                            '   WOrig(i, j, ip) = m_esData.WchangeVar(i, j, ip)
                            Blastseed(i, j, ip) = m_SpaceData.Blast(i, j, ip)
                        Next
                    Next
                Next
                'Btime is needed when running Ecoseed
                For i = 1 To m_SpaceData.NGroups
                    StoreBtimeForEcoSeed(i) = biomass(i)
                Next
            End If

            If TimesCalled >= 2 Then 'second time recalls the original bcell values for each timestep
                For i = 1 To m_SpaceData.InRow
                    For j = 1 To m_SpaceData.InCol
                        For ip = 1 To m_SpaceData.NGroups
                            '  Bseed(i, j, ip) = BOrig(i, j, ip)
                            ' Fseed(i, j, ip) = FOrig(i, j, ip)
                            '    Wseed(i, j, ip) = WOrig(i, j, ip)
                            m_SpaceData.Blast(i, j, ip) = Blastseed(i, j, ip)
                            m_SpaceData.Bcell(i, j, ip) = BOrig(i, j, ip) 'Bseed(i, j, ip)
                            m_EcoSpace.FtimeCell(i, j, ip) = FOrig(i, j, ip)
                            ' WchangeVar(i, j, ip) = Wseed(i, j, ip)
                            '    LastT = m_esData.SumStart(0) - TimeStep
                        Next
                    Next
                Next
                For i = 1 To m_SpaceData.NGroups
                    biomass(i) = StoreBtimeForEcoSeed(i)
                Next
            End If

        Catch ex As Exception
            Me.WriteError(ex)
            Debug.Assert(False, ex.StackTrace)
            Throw New ApplicationException("EcoSeed.KeepOrReloadCellValues() error: " & ex.Message, ex)
        End Try

    End Sub


    Private Sub setRunState(ByVal RunState As cMPAOptManager.eRunStates)

        Try

            If Me.m_StateCallback IsNot Nothing Then

                Me.m_StateCallback.Invoke(RunState)

            End If

        Catch ex As Exception
            cLog.Write(ex)
            Debug.Assert(False, Me.ToString & ".setRunState() " & ex.Message)
        End Try

    End Sub


    Private Function CellsNotMPA() As Boolean

        For i As Integer = 1 To m_SpaceData.InRow
            For j As Integer = 1 To m_SpaceData.InCol
                If m_SpaceData.MPA(i, j) = 0 And m_SpaceData.Depth(i, j) > 0 Then
                    Return True
                End If
            Next
        Next

        Return False

    End Function


    Private Function CalculateAreaOverBondaryLength() As Single
        Dim ir As Integer
        Dim ic As Integer
        Dim Area As Single
        Dim Border As Integer
        CalculateAreaOverBondaryLength = 0
        For ir = 1 To m_SpaceData.InRow
            For ic = 1 To m_SpaceData.InCol
                If m_SpaceData.MPA(ir, ic) > 0 Then
                    Area = Area + 1
                    If m_SpaceData.MPA(ir - 1, ic) = 0 And m_SpaceData.Depth(ir - 1, ic) > 0 Then Border = Border + 1 'cell above is not mpa
                    If m_SpaceData.MPA(ir + 1, ic) = 0 And m_SpaceData.Depth(ir + 1, ic) > 0 Then Border = Border + 1 'cell below is not mpa
                    If m_SpaceData.MPA(ir, ic - 1) = 0 And m_SpaceData.Depth(ir, ic - 1) > 0 Then Border = Border + 1 'cell left is not mpa
                    If m_SpaceData.MPA(ir, ic + 1) = 0 And m_SpaceData.Depth(ir, ic + 1) > 0 Then Border = Border + 1 'cell right is not mpa
                End If
            Next
        Next
        If Border > 0 Then
            Return Area / Border
        Else
            'baserun no mpa, so return 1?
            Return 0.25
        End If
    End Function

    Private Sub getBaseValues()

        m_search.redimForRun()

        'on the first call to ecospace ecoseed makes a copy of Biomass(), FTime()... See KeepOrReloadCellValues() at the user defined start time-step
        'then on subsequient calls it starts ecospace at the user defined start time-step and copies the values from the original call back to ecospace
        TimesCalled = 1
        'Get economic values for the base year BaseYearCost and BaseYearEffort
        Me.m_search.bBaseYearSet = False
        m_EcoSpace.Run()

        If Me.m_data.StopRun Then Exit Sub

        ''this will start ecospace at the user defined timestep and copy the values from the first call into this timestep
        TimesCalled = 2
        m_EcoSpace.Run()

        'values were set in the search object by EcoSpace.Run()
        EmployBase = m_search.Employ
        TotValBase = m_search.totval
        ManValueBase = m_search.manvalue
        EcoValueBase = m_search.ecovalue
        KemptonsBase = m_search.KemptonQ
        AreaBoundBase = CalculateAreaOverBondaryLength()

        If TotValBase = 0 Then TotValBase = 1
        If TotValBase < 0 Then TotValBase = -TotValBase
        If EmployBase = 0 Then EmployBase = 1
        If EmployBase < 0 Then EmployBase = -EmployBase
        If ManValueBase = 0 Then ManValueBase = 1
        If EcoValueBase = 0 Then EcoValueBase = 1
        If AreaBoundBase = 0 Then AreaBoundBase = 1
        If KemptonsBase = 0 Then KemptonsBase = 1

        TotWeightedValueBase = 0 + m_search.ValWeight(eSearchCriteriaResultTypes.TotalValue) * TotValBase + _
                        m_search.ValWeight(eSearchCriteriaResultTypes.Employment) * EmployBase + _
                        m_search.ValWeight(eSearchCriteriaResultTypes.MandateReb) * ManValueBase + _
                        m_search.ValWeight(eSearchCriteriaResultTypes.Ecological) * EcoValueBase + _
                        m_search.ValWeight(eSearchCriteriaResultTypes.BioDiversity) * KemptonsBase + _
                        m_data.BoundaryWeight * AreaBoundBase

    End Sub

    Private Sub CalculateCellWeightings()
        'VC added this sub
        Dim iC As Integer       'used to count the cells

        Try

            Dim inRow As Integer = m_SpaceData.InRow
            Dim inCol As Integer = m_SpaceData.InCol
            CellCount = inRow * inCol

            ReDim CumulativeCellWeight(CellCount)
            Dim CellWeight(inRow, inCol) As Double

            'If on the GUI the "Group weighting" is checked then calculate cellweight, otherwise, set to 1
            'use guidance function
            'cell contribution to objectivity function at the ecopath base case 
            '1. equal prob
            '2. biomass or habitat proportional
            '3. inverse objectivity function 
            'evt 4 mcmc search, start with a given number of closed cells, replace a cell (based on probability), evaluate, 

            'develop a measure including
            '1. spatial cost of fishing (distance from port): this becomes and "importance" layer, we can just cut and paste it in
            '2. depth factor (deeper  = more costly): this also becomes an importance layer
            '3. Any "importance" layer, i.e. Jeroen, we need to be able to store "importance" layers, which for now can be cut and pasted into ecospace. 
            '   The "importance" layers will need to have a title and description, plus a value for each cell. 
            '4. How much does the cell contribute to fishing pressure for the cells to be protected


            'Scan through the spreadsheet with the importance layers, and set up the likelihood function.

            'If Me.m_data.bUseCellWeight Then
            '    ''Get the ecosystem structure weightings from the GUI (needs to be added)
            '    ''for now hard code to 1
            '    'Dim GroupWeight(m_SpaceData.NGroups) As Single
            '    'For ip As Integer = 1 To m_SpaceData.NGroups
            '    '    GroupWeight(ip) = 1
            '    'Next

            '    For i As Integer = 1 To inRow
            '        For j As Integer = 1 To inCol
            '            For ip As Integer = 1 To m_SpaceData.NGroups
            '                '    CellWeight(i, j) += GroupWeight(ip) * BOrig(i, j, ip)
            '                CellWeight(i, j) += Me.m_search.BGoalValue(ip) * BOrig(i, j, ip)
            '            Next
            '        Next
            '    Next
            'Else
            'iC = 0

            Dim data(,,) As Single = Me.m_SpaceData.ImportanceLayerMap
            Dim weight As Double
            Dim LayerSum(Me.m_SpaceData.nImportanceLayers) As Double

            'VC2008Nov11, scaling each of the importance layers to have average 1
            For iL As Integer = 1 To Me.m_SpaceData.nImportanceLayers
                'weight = Me.m_SpaceData.ImportanceLayers(iL).sWeight
                Dim Count As Integer = 0
                For i As Integer = 1 To inRow
                    For j As Integer = 1 To inCol
                        If data(iL, i, j) > 0 Then
                            Count += 1
                            LayerSum(iL) += data(iL, i, j)
                        End If
                    Next j
                Next i
                'This will make the average for each layer 1, but then a layer that only has values 
                'in a few cells will count much less, than one with values in many cells
                'If Count > 0 Then AverageLayer(iL) /= Count
                'So insteat making the layers SUM to 1
                If LayerSum(iL) = 0 Then LayerSum(iL) = 1 'just to avoid division with 0, if a layer is empty
            Next iL

            Dim minCellWeight As Double = 1000000000000000
            For iL As Integer = 1 To Me.m_SpaceData.nImportanceLayers
                weight = Me.m_SpaceData.ImportanceLayerWeight(iL)
                For i As Integer = 1 To inRow
                    For j As Integer = 1 To inCol
                        CellWeight(i, j) += weight * data(iL, i, j) / LayerSum(iL)
                        If CellWeight(i, j) < minCellWeight And CellWeight(i, j) > 0 Then minCellWeight = CellWeight(i, j)
                    Next j
                Next i
            Next iL

            'now make sure all cells can be selected:
            For i As Integer = 1 To inRow
                For j As Integer = 1 To inCol
                    If CellWeight(i, j) = 0 Then 'give it a value
                        CellWeight(i, j) = 0.001 * minCellWeight
                    End If
                Next j
            Next i


            'Now calculate cumulative weighted importance over all cells:
            iC = 0
            Dim Sum As Double = 0
            For i As Integer = 1 To inRow
                For j As Integer = 1 To inCol
                    iC += 1
                    If CellWeight(i, j) < 0 Then CellWeight(i, j) = 0
                    Sum += CellWeight(i, j)
                    CumulativeCellWeight(iC) = Sum
                Next
            Next

            'Finally scalse the cellweights so that they sum to 1
            If Sum > 0 Then
                For i As Integer = 1 To CellCount
                    CumulativeCellWeight(i) /= Sum
                Next
            Else
                'if there are no values in any of the importance layer
                'set CumulativeCellWeight() to an even gradient so that the cell selection will not be weighted
                Dim g As Single = CSng(1 / CellCount)
                For i As Integer = 1 To CellCount
                    CumulativeCellWeight(i) += g * i
                Next
            End If

        Catch ex As Exception
            Me.WriteError(ex)
            Debug.Assert(False, ex.StackTrace)
            Throw New ApplicationException(Me.ToString & ".CalculateCellWeightings() " & ex.Message, ex)
        End Try

    End Sub

    Private Sub sortLayersByCellWeight(ByVal CellCount As Integer)
        Dim NoCells As Integer = m_SpaceData.InRow * m_SpaceData.InCol
        ReDim MaxLayerSumByLayerAndPctMPA(m_SpaceData.nImportanceLayers, 100)

        For iL As Integer = 1 To Me.m_SpaceData.nImportanceLayers
            Dim Cnt As Integer = 0
            Dim ArrayVal(NoCells) As Single

            For i As Integer = 1 To m_SpaceData.InRow
                For j As Integer = 1 To m_SpaceData.InCol
                    Cnt = Cnt + 1
                    'Make a copy of the data
                    ArrayVal(Cnt) = m_SpaceData.ImportanceLayerMap(iL, i, j)
                Next j
            Next i
            'now we have all the layer values in ArrayVal, so sort them:
            System.Array.Sort(ArrayVal)
            System.Array.Reverse(ArrayVal)
            'We can now store the layerweight for each percentage coverage:
            For iMPA As Integer = 1 To 100
                'we want to store this for 100 levels (%) of protection
                For iC As Integer = 0 To CInt(CellCount * iMPA / 100) - 1
                    MaxLayerSumByLayerAndPctMPA(iL, iMPA) += ArrayVal(iC)
                Next
            Next
        Next iL
    End Sub

    Private Sub calcImportanceLayersCoverageInRun()
        Dim Data(,,) As Single = Me.m_SpaceData.ImportanceLayerMap
        ReDim LayerSumInMPA(Me.m_SpaceData.nImportanceLayers)

        For iL As Integer = 1 To Me.m_SpaceData.nImportanceLayers
            For iR As Integer = 1 To m_SpaceData.InRow
                For iC As Integer = 1 To m_SpaceData.InCol
                    If m_SpaceData.MPA(iR, iC) = m_data.iMPAtoUse Then 'this is a protected cell, so check what 
                        LayerSumInMPA(iL) += Data(iL, iR, iC)
                    End If
                Next iC
            Next iR
        Next iL
    End Sub

#End Region

#Region "Saving Ouput CSV file and memory"

    ''' <summary>
    ''' Store the best row and col for this search interation
    ''' </summary>
    ''' <remarks>Right now this is writting the results file and memory</remarks>
    Private Sub StoreObjectiveFunctionResults(ByVal writer As StreamWriter)

        Try

            'write the data to file
            Me.WriteOutputData(writer)

            'keep the results in memory
            '  m_lstObjectiveResults.Add(New cObjectiveResult(m_data))

        Catch ex As Exception
            Debug.Assert(False, "Ecoseed Error in StoreObjectiveFunctionResults(). " & ex.Message)
            cLog.Write(ex)
            'Just Blunder On????????????????????

        End Try

    End Sub

    ''' <summary>
    ''' Write header information to an output writer.
    ''' </summary>
    ''' <param name="writer">The writer to write to.</param>
    Private Sub WriteOutputFileHeader(ByVal writer As StreamWriter)

        If (writer Is Nothing) Then Return

        'EwE5
        'Write #fnum, "row", "col", "econ", "social", "mandated", "ecosystem", "Area/Border"
        'Write #fnum, "", "", ValWeight(1), ValWeight(2), ValWeight(3), ValWeight(4), BoundaryWeight

        writer.WriteLine("MPA Optimization output")
        writer.WriteLine(Me.m_strHeader)
        writer.WriteLine("Objective weights for run")
        writer.WriteLine("Economic,Social,Mandated,Ecosystem,Biodiversity,Area/Boundary")
        writer.WriteLine()
        writer.WriteLine(cStringUtils.Localize("{0},{1},{2},{3},{4}", _
                cStringUtils.FormatNumber(Me.m_search.ValWeight(eSearchCriteriaResultTypes.TotalValue)), _
                cStringUtils.FormatNumber(Me.m_search.ValWeight(eSearchCriteriaResultTypes.Employment)), _
                cStringUtils.FormatNumber(Me.m_search.ValWeight(eSearchCriteriaResultTypes.MandateReb)), _
                cStringUtils.FormatNumber(Me.m_search.ValWeight(eSearchCriteriaResultTypes.Ecological)), _
                cStringUtils.FormatNumber(Me.m_search.ValWeight(eSearchCriteriaResultTypes.BioDiversity)), _
                cStringUtils.FormatNumber(Me.m_data.BoundaryWeight)))
        writer.WriteLine()
        writer.WriteLine("Base Values")
        writer.WriteLine("Economic, Social, Mandated, Ecosystem, Biomass Diversity, Area/Boundary")
        writer.WriteLine(cStringUtils.Localize("{0},{1},{2},{3},{4},{5}", _
                cStringUtils.FormatNumber(TotValBase), _
                cStringUtils.FormatNumber(EmployBase), _
                cStringUtils.FormatNumber(ManValueBase), _
                cStringUtils.FormatNumber(EcoValueBase), _
                cStringUtils.FormatNumber(KemptonsBase), _
                cStringUtils.FormatNumber(AreaBoundBase)))
        writer.WriteLine()
        'writer.WriteLine("Data Format")
        'writer.WriteLine("Number of Rows and Columns")
        'writer.WriteLine("Row, Column, MPAIndex")
        'writer.WriteLine("Economic,Social,Mandated,Ecosystem,Biodiversity,Area/Border")

        ' ToDo: globalize this
        ' ToDo: send at end of autosave, include result
        Dim msg As New cMessage(cStringUtils.Localize("MPA search output saved to '{0}", Path.Combine(Me.m_strOutputPath, c_FILENAME)), _
                                eMessageType.DataExport, eCoreComponentType.External, eMessageImportance.Information)
        msg.Hyperlink = Me.m_strOutputPath
        Me.SendMessage(msg)

    End Sub

    ''' <summary>
    ''' Write the objective function values to file
    ''' </summary>
    ''' <param name="writer">The writer to write to.</param>
    Private Sub WriteOutputData(ByVal writer As StreamWriter)

        If (writer Is Nothing) Then Return

        Try
            'EwE5
            'Write #fnum, bestrow, bestcol, ObjF(0), ObjF(1), ObjF(2), ObjF(3), ObjF(4)
            writer.WriteLine("Iteration," & Me.m_data.nIterations)
            writer.WriteLine("MPA cells," & cStringUtils.FormatNumber(Me.m_data.Cells.Count))
            For Each cell As cMPACell In m_data.Cells
                writer.WriteLine("{0},{1},{2}", cell.Row, cell.Col, cell.iMPA)
            Next
            writer.WriteLine("Economic,Social,Mandated,Ecosystem,Biodiversity,Area/Border")
            writer.WriteLine(cStringUtils.Localize("{0},{1},{2},{3},{4}", _
                   cStringUtils.FormatNumber(Me.m_data.objFuncEconomicValue), _
                   cStringUtils.FormatNumber(Me.m_data.objFuncSocialValue), _
                   cStringUtils.FormatNumber(Me.m_data.objFuncMandatedValue), _
                   cStringUtils.FormatNumber(Me.m_data.objFuncEcologicalValue), _
                   cStringUtils.FormatNumber(Me.m_data.objFuncBiodiversity), _
                   cStringUtils.FormatNumber(Me.m_data.objFuncAreaBorder)))

        Catch ex As Exception
            cLog.Write(ex, "cMPARandomSearch::WriteOutputData")
        End Try

    End Sub

#End Region

#Region "Memory Managment"

    Private Sub cleanUp()

        Erase BOrig
        Erase FOrig
        Erase WOrig
        Erase Blastseed

    End Sub

    Private Sub RedimSeedVariables()
        Dim nvartot As Integer = m_SpaceData.NGroups + 2

        ReDim BOrig(m_SpaceData.InRow + 1, m_SpaceData.InCol + 1, nvartot)
        ReDim FOrig(m_SpaceData.InRow + 1, m_SpaceData.InCol + 1, nvartot)
        ReDim WOrig(m_SpaceData.InRow + 1, m_SpaceData.InCol + 1, nvartot)
        ReDim Blastseed(m_SpaceData.InRow + 1, m_SpaceData.InCol + 1, nvartot)
        ReDim StoreBtimeForEcoSeed(m_SpaceData.NGroups)

    End Sub


#End Region

#Region " Message handling "

    Private Sub WriteError(ByVal ex As Exception)
        Try
            cLog.Write(ex)
            System.Console.WriteLine(Me.ToString & " Error: " & ex.Message)
            System.Console.WriteLine("Stack trace " & ex.StackTrace)
        Catch newEx As Exception
            Debug.Assert(False, newEx.Message)
        End Try
    End Sub

    Private Sub WriteError(ByVal message As String, ByVal ex As Exception)
        Try
            cLog.Write(message)
            WriteError(ex)
        Catch newEx As Exception
            Debug.Assert(False, newEx.Message)
        End Try
    End Sub

    Private Sub WriteError(ByVal message As String)
        Dim msg As New cMessage(message, eMessageType.ErrorEncountered, eCoreComponentType.MPAOptimization, eMessageImportance.Critical)
        Me.SendMessage(msg)
    End Sub

    Private Sub SendMessage(ByVal msg As cMessage)
        Try
            If (Me.m_SendMessageDelegate IsNot Nothing) Then Me.m_SendMessageDelegate.Invoke(msg)
        Catch ex As Exception
            cLog.Write(ex)
            Debug.Assert(False, Me.ToString & ".setRunState() " & ex.Message)
        End Try
    End Sub

#End Region ' Message handling

End Class
