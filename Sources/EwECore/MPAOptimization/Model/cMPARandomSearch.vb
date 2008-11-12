'==============================================================================
'
' $Log: cMPARandomSearch.vb,v $
' Revision 1.5  2008/11/12 22:21:46  joeb
' Bug fixes from adding BiomassDiversity
'
' Revision 1.4  2008/11/12 20:20:58  joeb
' added BiomassDiversity to MPA stuff
'
' Revision 1.3  2008/11/12 19:14:15  joeb
' CellSelectedMap now contains  PercentAreaClosedFilter
'
' Revision 1.2  2008/11/11 23:02:26  villyc
' Scaling of importance layers to unity average before summing up. Not sure what change in scientificinterface is.
'
' Revision 1.1  2008/09/26 07:30:26  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.36  2008/09/24 00:11:04  villyc
' f limits and others
'
' Revision 1.35  2008/08/26 19:42:09  villyc
' add trap for negative cell weightings in importance layers
'
' Revision 1.34  2008/08/19 19:23:44  joeb
' Quiting a search is quicker
'
' Revision 1.33  2008/08/19 17:07:38  joeb
' Changed TotWeightedValueBase
'
' Revision 1.32  2008/08/18 17:49:39  joeb
' Added WeightedTotal to Search data
'
' Revision 1.31  2008/08/17 16:55:22  joeb
' MPA Optimization default data directory
'
' Revision 1.30  2008/08/15 22:07:07  joeb
' Percentage in Results()
'
' Revision 1.29  2008/08/15 21:11:17  joeb
' Changed RunStates to Initializing and Searching
'
' Revision 1.28  2008/08/15 17:31:55  joeb
' Last commit was missing some code Oppsssss
'
' Revision 1.27  2008/08/15 16:47:43  joeb
' Fixed Random not selecting cells if not importance layer(s)
'
' Revision 1.26  2008/08/14 18:07:05  joeb
' Added StartYear and EndYear to MPA Optimizations
'
' Revision 1.25  2008/08/13 15:22:03  joeb
' Removed dead code
'
' Revision 1.24  2008/08/11 21:10:20  joeb
' *** empty log message ***
'
' Revision 1.23  2008/08/07 19:41:24  sherman
' Exposed LayerImportance from the Core
'
' Revision 1.22  2008/08/07 18:17:34  sherman
' Added Importance Layer to Ecospace Datastructures
'
' Revision 1.21  2008/08/06 22:35:14  villyc
' small edits
'
' Revision 1.20  2008/08/06 21:13:43  villyc
' Adding computations for 'importance layers' in random mpa
'
' Revision 1.19  2008/06/26 14:07:15  joeb
' *** empty log message ***
'
' Revision 1.18  2008/06/26 05:56:51  villyc
' Adding cost of sailing calculation, not tested yet
'
' Revision 1.17  2008/06/25 21:04:16  villyc
' *** empty log message ***
'
' Revision 1.16  2008/06/25 20:25:24  joeb
' Added results list
'
' Revision 1.15  2008/06/25 19:00:57  joeb
' Sorting of results
'
' Revision 1.14  2008/06/25 18:25:00  villyc
' Removing VC's wy of storing best runs
'
' Revision 1.13  2008/06/24 21:56:41  joeb
' Added bUseCellWeight
'
' Revision 1.12  2008/06/24 00:44:13  villyc
' VC random mpa top 1% implementation
'
' Revision 1.11  2008/06/23 16:39:08  villyc
' VC updating mpa optim
'
' Revision 1.10  2008/06/20 21:27:13  joeb
' Changing output for both Ecoseed and Random search
'
' Revision 1.9  2008/06/19 20:53:33  villyc
' VC small fixes
'
' Revision 1.8  2008/06/19 18:05:12  joeb
' Random search clears out old ecoseed results
'
' Revision 1.7  2008/06/19 16:52:33  joeb
' File output
' Added Cells(List of cMPACell)
'
' Revision 1.6  2008/06/18 18:41:26  villyc
' fixed counting of cells
'
'==============================================================================

Option Strict On
Imports System.Math

Public Class cMPARandomSearch
    Implements IMPASearchModel

#Region "Private data"

    Private Port(,) As Boolean 'VC port dimensioning should be row, col, fleets, but I'll leave out fleets now
    'VC we need the below from basemap
    Private IDH_UL As Long       'Upper left coordinate of basemap
    Private IDH_SS As Integer    'Basemap stepsize in no of steps per degree
    Private Lat1 As Single   'Long
    Private Lat2 As Single   'Long
    Private Lon1 As Single   'Long
    Private Lon2 As Single   'Long

    Const N_MAX_RESULTS As Integer = 500
    Const RESULTS_TO_KEEP As Integer = N_MAX_RESULTS \ 2

    Private m_EcoSpace As cEcoSpace
    Friend m_SpaceData As cEcospaceDataStructures

    Private m_data As cMPAOptDataStructures
    Private m_search As cSearchDatastructures

    Private m_bRunning As Boolean
    Private m_esStartTime As Single
    Private EcoSeedOn As Boolean
    Private m_bWriteFile As Boolean

    'results of each iterations
    Private m_lstObjectiveResults As New List(Of cObjectiveResult)

    Private m_cellComputedCallback As SearchIterationDelegate
    Private m_StateCallback As RunStateDelegate
    Private m_SendMessageCallback As SendMessageDelegate

    '''' <summary>Best results of the current run</summary>
    'Private m_bestResults As cObjectiveResult

    Private CumulativeCellWeight() As Double
    Private CellCount As Integer
    Private m_nIters As Integer 'number of iteration completed

    Private m_filename As String

#Region "Modeling data from EwE5"

    Private BOrig(,,) As Single
    Private FOrig(,,) As Single
    Private WOrig(,,) As Single
    Private TimesCalled As Long
    Private Blastseed(,,) As Single

    Public StoreBtimeForEcoSeed() As Single

    Private TotWeightedValueBase As Single
    Private EmployBase As Single, TotValBase As Single, ManValueBase As Single, EcoValueBase As Single, KemptonsBase As Single
    Private TargetSumMax As Single

#End Region

#End Region

#Region "Construction and Initialization"

    Public Function Init(ByRef EcoSpaceModel As cEcoSpace, ByRef MPAOptData As cMPAOptDataStructures) As Boolean Implements IMPASearchModel.Init

        Try

            m_EcoSpace = EcoSpaceModel

            'set EcoSpace to use this MPA optimization model
            m_EcoSpace.MPAOptimization = Me

            m_SpaceData = m_EcoSpace.EcoSpaceParameters
            m_data = MPAOptData

            'the seed array can be needed before the model is run
            ReDim m_data.MPASeed(m_SpaceData.Inrow + 1, m_SpaceData.InCol + 1)

        Catch ex As Exception
            cLog.Write(ex)
            Return False
        End Try

        Return True

    End Function

    Public Sub Connect(ByVal OnSearchInteration As SearchIterationDelegate, ByVal OnRunStateChanged As RunStateDelegate, ByVal OnSendMessage As SendMessageDelegate) Implements IMPASearchModel.Connect
        m_cellComputedCallback = OnSearchInteration
        m_StateCallback = OnRunStateChanged
        m_SendMessageCallback = OnSendMessage
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
        For ir As Integer = 1 To m_SpaceData.Inrow
            For ic As Integer = 1 To m_SpaceData.InCol
                m_SpaceData.MPA(ir, ic) = 0
            Next ic
        Next ir
    End Sub

    Public Sub clearSeedCells() Implements IMPASearchModel.clearSeedCells
        For ir As Integer = 1 To m_SpaceData.Inrow
            For ic As Integer = 1 To m_SpaceData.InCol
                m_data.MPASeed(ir, ic) = 0
            Next ic
        Next ir
    End Sub


    Public Function setAllCellsToMPA(ByVal iMPA As Integer) As Boolean Implements IMPASearchModel.setAllCellsToMPA

        'make sure the MPA index supplied by the user is in bounds
        If iMPA > 0 And iMPA <= m_SpaceData.MPAno Then
            For ir As Integer = 1 To m_SpaceData.Inrow
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
            For ir As Integer = 1 To m_SpaceData.Inrow
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
            Return m_nIters
        End Get
    End Property

    Public Property OutPutFilename() As String Implements IMPASearchModel.OutPutFilename
        Get
            Return Me.m_filename
        End Get
        Set(ByVal value As String)
            Me.m_filename = value
        End Set
    End Property


    Public ReadOnly Property OKtoRun() As Boolean Implements IMPASearchModel.OKtoRun
        Get
            'the random search can always run 
            'thats quite a statment...
            Return True
        End Get
    End Property

#End Region

#Region "Running the model"

    Private Sub initForRun()

        Try

            'Ecoseed does not listen to the Ecospace time steps
            Me.m_EcoSpace.TimeStepDelegate = Nothing

            m_bWriteFile = False
            If Not String.IsNullOrEmpty(Me.m_filename) Then
                'if there is a filename then write the output file
                m_bWriteFile = True
            End If

            'create a new list to store the results
            m_lstObjectiveResults = New List(Of cObjectiveResult)
            TargetSumMax = 0

            'Clear out any values from a previous ecoseed run
            m_data.Clear()

            m_search = m_EcoSpace.SearchData
            RedimSeedVariables()

        Catch ex As Exception
            Me.WriteError(ex)
            Throw New ApplicationException(Me.ToString & ".initForRun() Error: " & ex.Message, ex)
        End Try

    End Sub


    Public Sub Run() Implements IMPASearchModel.Run

        Try

            Me.m_bRunning = True
            Me.setRunState(eRunStates.Initializing)

            Me.m_data.StopRun = False

            Me.runSearch()

        Catch ex As Exception
            Me.SendErrorMessage("MPA Optimizatoin Random Search Error")
            Debug.Assert(False, ex.StackTrace)
        End Try

        Me.m_bRunning = False
        Me.setRunState(eRunStates.Completed)

    End Sub


    Friend Sub runSearch()
        'VC changes
        'Main loop for running the Random MPA optimization

        Dim StoreOptimalPct As Single = 1 'from GUI
        Dim MinimalEvaluationValue As Single = 0

        Try
            Debug.Assert(m_data IsNot Nothing, "Ecoseed: data not initialized")
            Debug.Assert(m_EcoSpace IsNot Nothing, "Ecoseed: Ecospace not initialized")
            System.Console.WriteLine("-----------MPA Random Search --------------")

            Me.initForRun()
            m_search.SearchMode = eSearchModes.SpatialOpt
            m_search.setMinSearchBlocks()
            Me.getBaseValues()

            Me.WriteOutputFileHeader()

            CalculateCellWeightings()

            Dim iR As Integer = m_SpaceData.Inrow
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

            Dim StoreNo As Integer = CInt(StoreOptimalPct * m_data.nIterations / 100)

            'Step from Min area(%) (= integer) to Max area(%) (= integer) stepsize = Step (%) (=integer)
            Dim iStep As Integer = CInt((-m_data.MinArea + m_data.MaxArea) / m_data.stepSize)

            Debug.Assert(m_data.iMPAtoUse > 0, "Current MPA not set!!!.")

            Dim nStep As Integer = 0

            Me.setRunState(eRunStates.Searching)

            m_nIters = 0
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

                    'Save to csv file
                    Me.WriteOutputData()
                    m_nIters += 1

                Next
                If m_data.StopRun Then Exit For
                nStep += 1
            Next

            Me.m_lstObjectiveResults.Sort()

            cleanUp()

        Catch ex As Exception
            Me.WriteError(ex)
            m_bRunning = False
            Debug.Assert(False, ex.StackTrace)
            Throw New ApplicationException("MPARandom Search Error: " & ex.Message, ex)
        End Try

    End Sub


    Private Sub selectRandomCells(ByVal NumberMPA As Integer, ByVal curMPA As Integer)
        'VC changes
        Dim generator As New Random()   '

        Try

            'clear out the last set of cells
            m_data.ClearCells()

            'VC presume its quicker to load to local value than stepping out many times to get these:
            Dim inRow As Integer = Me.m_SpaceData.Inrow '+ 1
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
        Dim AreaBorder As Single

        Try

            curSum = m_search.ValWeight(1) * m_search.totval / TotValBase + _
                     m_search.ValWeight(2) * m_search.Employ / EmployBase + _
                     m_search.ValWeight(3) * m_search.manvalue / ManValueBase + _
                     m_search.ValWeight(4) * m_search.ecovalue / EcoValueBase + _
                      m_search.ValWeight(5) * m_search.KemptonQ / KemptonsBase


            'Calculate boundary length/area ratio
            'If m_data.BoundaryWeight > 0 Then
            AreaBorder = CalculateAreaOverBondaryLength()
            curSum = curSum + AreaBorder * m_data.BoundaryWeight
            'End If
            m_data.objFuncTotal = (m_search.WeightedTotal + AreaBorder) / Me.TotWeightedValueBase

            'calculate the relative values in to data structures 
            'so they can be use to populate the Input/Output object for the interface
            m_data.objFuncEcologicalValue = m_search.ecovalue / EcoValueBase
            m_data.objFuncMandatedValue = m_search.manvalue / ManValueBase
            m_data.objFuncSocialValue = m_search.Employ / EmployBase
            m_data.objFuncEconomicValue = m_search.totval / TotValBase
            m_data.objBiomassDiversity = m_search.KemptonQ / KemptonsBase
            m_data.objFuncAreaBorder = AreaBorder

            If curSum > TargetSumMax Then
                'save the best results 
                TargetSumMax = curSum

                Me.setRunState(eRunStates.NewBestResultFound)

            End If

            'keep the results of every search
            Me.m_lstObjectiveResults.Add(New cObjectiveResult(m_data, Me.m_SpaceData))

            'Memory management for results
            If Me.m_lstObjectiveResults.Count >= N_MAX_RESULTS Then
                'sorts in decending order (biggest objFuncTotal first)
                Me.m_lstObjectiveResults.Sort()
                'remove lowest results from the end of the list
                Me.m_lstObjectiveResults.RemoveRange(RESULTS_TO_KEEP - 1, Me.m_lstObjectiveResults.Count - RESULTS_TO_KEEP)
            End If

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
            iYear = CInt(m_EcoSpace.EcoSpaceParameters.TotalTime)
            m_EcoSpace.StopRun = True
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

                For i = 1 To m_SpaceData.Inrow
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
                For i = 1 To m_SpaceData.Inrow
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


    Private Sub setRunState(ByVal RunState As eRunStates)

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

        For i As Integer = 1 To m_SpaceData.Inrow
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
        For ir = 1 To m_SpaceData.Inrow
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
        If Border > 0 Then Return Area / Border
    End Function



    Private Sub getBaseValues()

        'on the first call to ecospace ecoseed makes a copy of Biomass(), FTime()... See KeepOrReloadCellValues() at the user defined start time-step
        'then on subsequient call it starts ecospace at the user defined start time-step and copies the values from the original call back to ecospace
        TimesCalled = 1
        m_EcoSpace.Run()

        If Me.m_data.StopRun Then Exit Sub

        'this will start ecospace at the user defined timestep and copy the values from the first call into this timestep
        TimesCalled = 2
        m_EcoSpace.Run()

        'values were set in the search object by EcoSpace.Run()
        EmployBase = m_search.Employ
        TotValBase = m_search.totval
        ManValueBase = m_search.manvalue
        EcoValueBase = m_search.ecovalue
        KemptonsBase = m_search.KemptonQ

        If TotValBase = 0 Then TotValBase = 1
        If TotValBase < 0 Then TotValBase = -TotValBase
        If EmployBase = 0 Then EmployBase = 1
        If EmployBase < 0 Then EmployBase = -EmployBase
        If ManValueBase = 0 Then ManValueBase = 1
        If EcoValueBase = 0 Then EcoValueBase = 1

        TotWeightedValueBase = 0 + m_search.ValWeight(1) * TotValBase + m_search.ValWeight(2) * EmployBase + _
                        m_search.ValWeight(3) * ManValueBase + m_search.ValWeight(4) * EcoValueBase + m_search.ValWeight(5) * KemptonsBase

        TotWeightedValueBase += CalculateAreaOverBondaryLength() * m_data.BoundaryWeight

        System.Console.WriteLine("Random weighted base value = " & TotWeightedValueBase.ToString)

    End Sub

    Private Sub CalculateCellWeightings()
        'VC added this sub
        Dim iC As Integer       'used to count the cells

        Try

            Dim inRow As Integer = m_SpaceData.Inrow
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

            Debug.Assert(Me.m_SpaceData.nImportanceLayers = Me.m_SpaceData.ImportanceLayers.Count, "Number of Importance Layers does not match the list of layers in the core")
            Dim data(,) As Single, weight As Double

            Dim AverageLayer(Me.m_SpaceData.nImportanceLayers - 1) As Double

            'VC2008Nov11, scaling each of the importance layers to have average 1
            For iL As Integer = 0 To Me.m_SpaceData.nImportanceLayers - 1
                data = Me.m_SpaceData.ImportanceLayers(iL).Data
                'weight = Me.m_SpaceData.ImportanceLayers(iL).sWeight
                Dim Count As Integer = 0
                For i As Integer = 1 To inRow
                    For j As Integer = 1 To inCol
                        If data(i, j) > 0 Then
                            Count += 1
                            AverageLayer(iL) += data(i, j)
                        End If
                    Next j
                Next i
                If Count > 0 Then AverageLayer(iL) /= Count
            Next iL


            For iL As Integer = 0 To Me.m_SpaceData.nImportanceLayers - 1
                data = Me.m_SpaceData.ImportanceLayers(iL).Data
                weight = Me.m_SpaceData.ImportanceLayers(iL).sWeight
                For i As Integer = 1 To inRow
                    For j As Integer = 1 To inCol
                        CellWeight(i, j) += weight * data(i, j) / AverageLayer(iL)
                    Next j
                Next i
            Next iL

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

#End Region

#Region "Saving Ouput CSV file and memory"

    ''' <summary>
    ''' Store the best row and col for this search interation
    ''' </summary>
    ''' <remarks>Right now this is writting the results file and memory</remarks>
    Private Sub StoreObjectiveFunctionResults()

        Try

            'write the data to file
            WriteOutputData()

            'keep the resutls in memory
            '  m_lstObjectiveResults.Add(New cObjectiveResult(m_data))

        Catch ex As Exception
            Debug.Assert(False, "Ecoseed Error in StoreObjectiveFunctionResults(). " & ex.Message)
            cLog.Write(ex)
            'Just Blunder On????????????????????

        End Try

    End Sub

    ''' <summary>
    ''' Create a new ouput csv file and write the header
    ''' </summary>
    ''' <remarks>This will destroy any existing file with the same name as OutputCVSFileName  </remarks>
    Private Sub WriteOutputFileHeader()

        If Not m_bWriteFile Then
            'not writing to file
            Return
        End If
        'EwE5
        'Write #fnum, "row", "col", "econ", "social", "mandated", "ecosystem", "Area/Border"
        'Write #fnum, "", "", ValWeight(1), ValWeight(2), ValWeight(3), ValWeight(4), BoundaryWeight

        Dim sb As New Text.StringBuilder
        sb.AppendLine("MPA Optimization output")
        sb.AppendLine("Date = " & Date.Today.ToLongDateString)
        sb.AppendLine("<Objective weights for run>")
        sb.AppendLine("Economic, Social, Mandated, Ecosystem, Area/Border")

        sb.AppendLine(String.Format("{0:F}, {1:F}, {2:F}, {3:F}, {4:F}", _
                m_search.ValWeight(1), m_search.ValWeight(2), m_search.ValWeight(3), m_search.ValWeight(4), m_data.BoundaryWeight))

        sb.AppendLine("<Base Values>")
        sb.AppendLine("Economic, Social, Mandated, Ecosystem")
        sb.AppendLine(String.Format("{0:F}, {1:F}, {2:F}, {3:F}", _
                TotValBase, EmployBase, ManValueBase, EcoValueBase))

        sb.AppendLine("<Data Format>")

        sb.AppendLine("Number of Rows and Columns")
        sb.AppendLine("Row, Column, MPAIndex")
        sb.AppendLine("Economic, Social, Mandated, Ecosystem, Area/Border")

        'this will create a new file each time
        cLog.WriteTextToFile(Me.m_filename, sb, False)

    End Sub

    ''' <summary>
    ''' Write the objective function values to file
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub WriteOutputData()

        Try

            If Not m_bWriteFile Then
                'not writing to file
                Return
            End If
            'EwE5
            'Write #fnum, bestrow, bestcol, ObjF(0), ObjF(1), ObjF(2), ObjF(3), ObjF(4)

            Dim sb As New Text.StringBuilder

            sb.AppendLine("<Data>")
            sb.AppendLine(m_data.Cells.Count.ToString)
            For Each cell As cMPACell In m_data.Cells
                sb.Append(String.Format("{0:D}, {1:D}, {2:D}, ", cell.Row, cell.Col, cell.iMPA))
            Next
            sb.Append(ControlChars.NewLine)

            sb.AppendLine(String.Format("{0:r}, {1:r}, {2:r}, {3:r}, {4:r}", _
                             m_data.objFuncEconomicValue, m_data.objFuncSocialValue, m_data.objFuncMandatedValue, m_data.objFuncEcologicalValue, m_data.objFuncAreaBorder))

            cLog.WriteTextToFile(Me.m_filename, sb, True)

        Catch ex As Exception
            cLog.Write(ex)
            Throw New ApplicationException("WriteOutputData() Error. " & ex.Message)
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

        ReDim BOrig(m_SpaceData.Inrow + 1, m_SpaceData.InCol + 1, nvartot)
        ReDim FOrig(m_SpaceData.Inrow + 1, m_SpaceData.InCol + 1, nvartot)
        ReDim WOrig(m_SpaceData.Inrow + 1, m_SpaceData.InCol + 1, nvartot)
        ReDim Blastseed(m_SpaceData.Inrow + 1, m_SpaceData.InCol + 1, nvartot)
        ReDim StoreBtimeForEcoSeed(m_SpaceData.NGroups)

    End Sub


#End Region

#Region "Message Handling"

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
            System.Console.WriteLine(message)
            WriteError(ex)
        Catch newEx As Exception
            Debug.Assert(False, newEx.Message)
        End Try

    End Sub

    Private Sub SendErrorMessage(ByVal message As String)

        Try

            If Me.m_SendMessageCallback IsNot Nothing Then
                Dim msg As New cMessage(message, eMessageType.ErrorEncountered, eMessageSource.MPAOptimization, eMessageImportance.Critical)
                Me.m_SendMessageCallback.Invoke(msg)
            End If

        Catch ex As Exception
            cLog.Write(ex)
            Debug.Assert(False, Me.ToString & ".setRunState() " & ex.Message)
        End Try

    End Sub


#End Region

#Region "VC sailing distance "

    Private Sub SetAllCoastsToPorts()
        Dim i As Integer
        Dim j As Integer
        Dim k As Integer
        Dim l As Integer
        Dim inRow As Integer = m_SpaceData.Inrow
        Dim inCol As Integer = m_SpaceData.InCol
        ReDim Port(inRow, inCol)

        For i = 1 To inRow
            For j = 1 To inCol
                'Check if there is a neighboring cell which is in water
                If m_EcoSpace.EcoSpaceParameters.Depth(i, j) <= 0 Then    'it is a land cell
                    For k = i - 1 To i + 1
                        For l = j - 1 To j + 1
                            'Don't check the cell being clicked
                            If (k <> i Or l <> j) And k > 0 And k <= inRow And l > 0 And l <= inCol And m_EcoSpace.EcoSpaceParameters.Depth(k, l) > 0 Then
                                'For Gear = 0 To NumGear
                                Port(i, j) = True
                                'Next
                            End If
                        Next
                    Next
                End If
            Next
        Next
    End Sub


    Private Sub CalculateCostOfSailing()
        Dim i As Integer
        Dim ix As Integer
        Dim iy As Integer
        Dim j As Integer
        Dim K As Integer
        Dim Ports As Integer
        'Dim vis() As Boolean
        Dim minD(,) As Single
        Dim Dist As Single
        Dim Lati As Single
        Dim Longi As Single
        Dim LatPort As Single
        Dim LonPort As Single
        Dim inRow As Integer = m_SpaceData.Inrow
        Dim inCol As Integer = m_SpaceData.InCol
        Dim PortX() As Integer
        Dim PortY() As Integer
        If IDH_SS <= 0 Then IDH_SS = 2

        'Dim SA As Single    'SailingCost per unit distance for this gear
        '                    'Unit distances are calculated here
        'If Lat1 = 0 And Lon1 = 0 Then MsgBox("Enter latitude and longitude", vbOKOnly) : Exit Sub
        'SA = cost(AF, 3)      'SailingCost(AF)
        'Erase PortX()
        'Erase PortY()
        Ports = 0
        For i = 1 To inRow
            For j = 1 To inCol
                If Port(i, j) = True Then
                    Ports = Ports + 1
                End If
            Next
        Next
        ReDim PortX(Ports)
        ReDim PortY(Ports)
        Ports = 0
        For i = 1 To inRow
            For j = 1 To inCol
                If Port(i, j) = True Then
                    Ports = Ports + 1
                    PortX(Ports) = i
                    PortY(Ports) = j
                End If
            Next
        Next        'If Ports = 0 Then
        '    MsgBox("No ports/landingsplaces entered for " + frmSpace.listFleet.Text, vbInformation + vbOKOnly, "Ecospace, no ports")
        '    Exit Sub
        'End If
        'OK now, there are ports
        Dist = 0
        'ReDim Vis(Inrow, Incol)
        ReDim minD(inRow, inCol)
        For i = 1 To inRow : For j = 1 To inCol
                minD(i, j) = 200000
            Next : Next
        For K = 1 To Ports      'go port by port
            ix = PortX(K)
            iy = PortY(K)
            LonPort = CSng(Lon1 + (ix / IDH_SS) / 2)
            LatPort = CSng(Lat1 - (iy / IDH_SS) / 2)
            'Sail(AF, ix, iy) = 0
            For i = 1 To inRow : For j = 1 To inCol
                    If m_EcoSpace.EcoSpaceParameters.Depth(i, j) > 0 Then 'water cell
                        Longi = CSng(Lon1 + (i / IDH_SS) / 2)
                        Lati = CSng(Lat1 - (j / IDH_SS) / 2)
                        Dist = CalDistance(LonPort, LatPort, Longi, Lati, 0)
                        If Dist < minD(i, j) Then minD(i, j) = Dist
                    End If
                Next : Next
            'test the neighboring cells
            'Calc8Dist i, j
            'FindMinDistFor8Neighbors i, j
        Next
        Dim Disti As Single
        For i = 1 To inRow : For j = 1 To inCol
                'If ActiveFleet = 0 Then    'Same for all fleets
                For K = 0 To m_EcoSpace.EcoPathParameters.NumFleet
                    'Port(K, i, j) = Port(0, i, j)
                    If Min(i, j) < 200000 Then Disti = Min(i, j) Else Disti = 0
                    m_EcoSpace.EcoSpaceParameters.Sail(K, i, j) = Disti
                Next
                'Else
                'Sail(ActiveFleet, i, j) = IIf(Min(i, j) < 200000, Min(i, j), 0)
                ''Next place zero sailing cost for non-coastal ports
                'If Sail(ActiveFleet, i, j) < 0 And Depth(i, j) > 0 Then Sail(ActiveFleet, i, j) = 0
                'End If
            Next : Next
    End Sub

    Private Function CalDistance(ByVal Lon1 As Single, ByVal Lat1 As Single, ByVal Lon2 As Single, ByVal Lat2 As Single, ByVal DistType As Integer) As Single  ', Dist As Single, XDist As Single, YDist As Single) As Single
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
        Dim Xdist As Double
        Dim Dist As Double

        Dim TwoPie As Double = 3.14159265359 * 2.0#
        Dim DR As Double = TwoPie / 360 'for converting degrees to radians for functions

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

        If DistType = 0 Then
            Dist = DistNM
            Ydist = Ydist * 60.0#
        ElseIf DistType = 1 Then
            'in km
            Dist = DistNM * 1.85325
            Ydist = Ydist * 60.0# * 1.85325
        ElseIf DistType = 2 Then
            Dist = AngDisDeg
        End If

        Return CSng(Dist)
        Xdist = Sqrt(Dist ^ 2 - Ydist ^ 2) * Sign(Lon1 - Lon2)
        Exit Function

errCalDistance:
        Xdist = -1
        CalDistance = 0 '-1 vc changed this from -1 to 0
        Exit Function

    End Function

#End Region

End Class
