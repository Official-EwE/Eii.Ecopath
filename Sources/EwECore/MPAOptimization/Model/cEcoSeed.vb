'==============================================================================
'
' $Log: cEcoSeed.vb,v $
' Revision 1.4  2008/11/13 18:40:07  joeb
' Added AreaBoundary
'
' Revision 1.3  2008/11/12 22:21:45  joeb
' Bug fixes from adding BiomassDiversity
'
' Revision 1.2  2008/11/12 19:14:15  joeb
' CellSelectedMap now contains  PercentAreaClosedFilter
'
' Revision 1.1  2008/09/26 07:30:26  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.18  2008/09/24 00:11:04  villyc
' f limits and others
'
' Revision 1.17  2008/08/19 19:23:43  joeb
' Quiting a search is quicker
'
' Revision 1.16  2008/08/19 17:07:36  joeb
' Changed TotWeightedValueBase
'
' Revision 1.15  2008/08/18 17:49:39  joeb
' Added WeightedTotal to Search data
'
' Revision 1.14  2008/08/17 16:55:22  joeb
' MPA Optimization default data directory
'
' Revision 1.13  2008/08/15 22:07:06  joeb
' Percentage in Results()
'
' Revision 1.12  2008/08/15 21:11:17  joeb
' Changed RunStates to Initializing and Searching
'
' Revision 1.11  2008/08/15 18:35:22  joeb
' Added TotalValue and PercentageClosed to cMPAOptOutPut
'
' Revision 1.10  2008/08/15 16:47:42  joeb
' Fixed Random not selecting cells if not importance layer(s)
'
' Revision 1.9  2008/08/14 18:07:04  joeb
' Added StartYear and EndYear to MPA Optimizations
'
' Revision 1.8  2008/08/11 21:10:19  joeb
' *** empty log message ***
'
' Revision 1.7  2008/06/26 14:07:13  joeb
' *** empty log message ***
'
' Revision 1.6  2008/06/25 20:25:23  joeb
' Added results list
'
' Revision 1.5  2008/06/20 21:27:11  joeb
' Changing output for both Ecoseed and Random search
'
' Revision 1.4  2008/06/19 16:52:31  joeb
' File output
' Added Cells(List of cMPACell)
'
' Revision 1.3  2008/06/18 18:19:48  joeb
' Changes for Random search file output
'
' Revision 1.2  2008/06/14 16:39:13  joeb
' Fixed a bug and Added NewCellSelected runstate
'
' Revision 1.1  2008/06/13 15:48:27  joeb
' Added MPAOptimization folder
'
' Revision 1.21  2008/06/12 19:13:32  joeb
' More changes to run random search
'
' Revision 1.20  2008/06/11 15:52:29  joeb
' Change names of Seed Files to MPAOpt
'
' Revision 1.19  2008/06/10 21:53:39  joeb
' Changes for new MPA optimization
'
' Revision 1.18  2008/05/01 15:13:26  joeb
'   m_search.setDefaultsForEcoseed() renamed to   m_search.setMinSearchBlocks()
'
' Revision 1.17  2008/04/24 20:02:22  joeb
' Removed some dead code
'
' Revision 1.16  2008/04/23 17:31:01  joeb
' Minor tweeks to SearchData
'
' Revision 1.15  2008/04/17 20:15:27  joeb
' Change  cSearchDataStructures.bDoFPSearch to cSearchDataStructures.bInSearch
'
' Revision 1.14  2008/04/04 02:00:07  jeroens
' Replacing stubbed test values with real user-configured values
'
' Revision 1.13  2008/03/29 00:12:32  jeroens
' Running state updated correctly
'
' Revision 1.12  2008/03/26 17:46:21  joeb
' Added RunStateCallback to Ecoseed
'
' Revision 1.11  2008/03/26 15:13:43  joeb
' Changed initForRun() set the EcospaceTimestepDelegate to Nothing this stops Ecospace from sending out Timestep messages
'
' Revision 1.10  2008/03/20 16:43:59  joeb
' Added some comments
'
' Revision 1.9  2008/02/26 19:12:21  joeb
' Fixed bug 432 Ecospace run stop before last time step
'
' Revision 1.8  2008/01/23 20:13:11  joeb
' Removed ecoseed debug form
'
' Revision 1.7  2008/01/23 17:30:43  joeb
' bunch of stuff
'
' Revision 1.6  2008/01/23 15:57:30  joeb
' Added Log header
'
'
'===================================================

Option Strict On

Imports EwECore
Imports EwECore.cEcoSpace

Namespace EcoSeed

    'ToDo_jb EcoSeed YearTimeStep() sets iYear to TotalTime
    'ToDo_jb m_data.SeedBlockSize2 EwE5 m_data.SeedBlockSize2 is set by the user to 1,4,9,16 or 25 
    '   SideStep = Sqr(m_data.SeedBlockSize2) then in EvaluateSeedCell m_data.SeedBlockSize2 is set to 9 and SideStep is not reset
    ' it looks like the only value that maters here is SideStep as long as SideStep is set before m_data.SeedBlockSize2 is hardwired at 9
    'this is happening in a temp way in at the start of run

    Public Class cEcoSeed
        Implements IMPASearchModel


#Region "Private data"

        Const N_MAX_RESULTS As Integer = 100
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
        ' Private m_BestObjective As cObjectiveResult

        Private m_cellComputedCallback As SearchIterationDelegate
        Private m_StateCallback As RunStateDelegate
        Private m_nIters As Integer

        Private m_filename As String

#Region "Modeling data from EwE5"

        Private BOrig(,,) As Single
        Private FOrig(,,) As Single
        Private WOrig(,,) As Single
        Private TestedSeed(,) As Boolean

        Private TimesCalled As Long
        Private SeedLeft As Boolean
        Private MPARow() As Integer
        Private MPACol() As Integer
        Private MPAVal() As Single   ', sum As Single
        Private MPABio() As Single, MPABioInit As Single
        Private MPAcount() As Integer, Tn As Integer ', MpaCnt As Integer ' added here for the pointer to be available in ecoseed abmpa
        Private MPAstep As Integer, ChangeFontClr As Boolean
        Private EffortMPA(,) As Single 'abmpa
        Private SailTot() As Single, SailMax() As Single
        'these next are to scale the value to a Von B type curve abmpa
        Private BOrigTot() As Single
        '     Private ValScaler() As Single
        '   Private Bseed(,,) As Single
        Private Blastseed(,,) As Single
        '  Private Fseed(,,) As Single
        '  Private Wseed(,,) As Single

        Public StoreBtimeForEcoSeed() As Single

        Private TotalSearchMax As Single
        Private SeedSumMax As Single

        Private EmployBase As Single, TotValBase As Single, ManValueBase As Single, EcoValueBase As Single, BioDiversityBase As Single, areaBoundBase As Single
        Private TotWeightedValueBase As Single
        Private SideStep As Integer

#End Region

#End Region

#Region "Construction and Initialization"

        Public Function Init(ByRef EcoSpaceModel As cEcoSpace, ByRef EcoSeedData As cMPAOptDataStructures) As Boolean Implements IMPASearchModel.Init

            Try

                m_EcoSpace = EcoSpaceModel

                'set EcoSpace to use this MPA optimization model
                m_EcoSpace.MPAOptimization = Me

                m_SpaceData = m_EcoSpace.EcoSpaceParameters
                m_data = EcoSeedData
                m_data.SeedBlockSize2 = 1 'default is one cell per iteration

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
        End Sub


#End Region

#Region "Public Properties and Methods"


        Public Property EcoSeedData() As cMPAOptDataStructures Implements IMPASearchModel.MPAOptData
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
            Me.m_EcoSpace.StopRun = True
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
                Return m_lstObjectiveResults
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
                '
                ' Check seeds
                For ir As Integer = 1 To Me.m_SpaceData.Inrow
                    For ic As Integer = 1 To Me.m_SpaceData.InCol
                        If m_data.MPASeed(ir, ic) > 0 Then
                            Return True
                        End If
                    Next ic
                Next ir

                ' Check MPAs
                ' Check within MPAs
                For ir As Integer = 1 To Me.m_SpaceData.Inrow
                    For ic As Integer = 1 To Me.m_SpaceData.InCol

                        If m_SpaceData.MPA(ir, ic) > 0 Then
                            Return True
                        End If
                    Next ic
                Next ir

                Return False

            End Get
        End Property

#End Region

#Region "Running the model"

        Private Sub initForRun()

            Try

                'Ecoseed does not listen to the Ecospace time steps
                Me.m_EcoSpace.TimeStepDelegate = Nothing

                If Not String.IsNullOrEmpty(Me.m_filename) Then
                    'if there is a filename then write the output file
                    m_bWriteFile = True
                End If

                'create a new list to store the results
                m_lstObjectiveResults = New List(Of cObjectiveResult)

            Catch ex As Exception
                cLog.Write(ex)
                Throw New ApplicationException(Me.ToString & ".initForRun() Error: " & ex.Message, ex)
            End Try

        End Sub


        Public Sub Run() Implements IMPASearchModel.Run

            Me.m_bRunning = True
            Me.setRunState(eRunStates.Initializing)

            Me.m_data.StopRun = False
            Try
                Me.runSeed()
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
                cLog.Write(ex)
            End Try

            Me.m_bRunning = False
            Me.setRunState(eRunStates.Completed)

        End Sub


        Friend Sub runSeed()
            Dim NotAllCellsAreMPAs As Boolean
            Dim AreaBordary As Single
            'Dim bExitRun As Boolean

            'total objective sum of the current search 
            Dim CurSum As Single

            Try
                Debug.Assert(m_data IsNot Nothing, "Ecoseed: data not initialized")
                Debug.Assert(m_EcoSpace IsNot Nothing, "Ecoseed: Ecospace not initialized")

                m_search = m_EcoSpace.SearchData

                initForRun()
                m_data.SeedBlockSize2 = 1

                'ToDo_jb SideStep EwE5 there is no explict cast of SideStep to int figure out if it is rounded or truncated
                SideStep = CInt(Math.Sqrt(m_data.SeedBlockSize2))

                SeedSumMax = Single.MinValue
                TotalSearchMax = Single.MinValue
                m_data.bestrow = -1
                m_data.bestcol = -1
                RedimSeedVariables()

                m_search.SearchMode = eSearchModes.SpatialOpt
                m_search.setMinSearchBlocks()

                getBaseValues()
                System.Console.WriteLine("------------Ecoseed----------------")

                WriteOutputFileHeader()

                Me.setRunState(eRunStates.Searching)

                Me.m_nIters = 0
                NotAllCellsAreMPAs = True
                Do While NotAllCellsAreMPAs

                    'check if all cells are MPAs
                    NotAllCellsAreMPAs = CellsNotMPA()
                    If NotAllCellsAreMPAs = False Then
                        EcoSeedOn = False
                    Else
                        EcoSeedOn = True
                        ReDim TestedSeed(m_SpaceData.Inrow, m_SpaceData.InCol)
                    End If

                    TimesCalled = TimesCalled + 1

                    'Loop over all the Seed cells and find the one with the highest weighted value
                    Do While EcoSeedOn
                        If m_data.StopRun Then Exit Do

                        'Set the next seed cell to be a MPA cell:
                        'SelectNextSeedCell will set EcoSeedOn 
                        '   True if there is a valid cell to test
                        '   False if all the cells have been evaluated and it is time to set the next batch of SeedCells
                        SelectNextSeedCell()

                        If EcoSeedOn Then

                            Output()
                            m_EcoSpace.Run()
                            If m_data.StopRun Then Exit Do

                            CurSum = 0 + m_search.ValWeight(1) * m_search.totval / TotValBase + _
                            m_search.ValWeight(2) * m_search.Employ / EmployBase + _
                            m_search.ValWeight(3) * m_search.manvalue / ManValueBase + _
                            m_search.ValWeight(4) * m_search.ecovalue / EcoValueBase + _
                             m_search.ValWeight(5) * m_search.KemptonQ / BioDiversityBase

                            'Calculate boundary length/area ratio
                            AreaBordary = CalculateAreaOverBondaryLength()
                            CurSum = CurSum + AreaBordary * m_data.BoundaryWeight

                            m_data.objFuncEcologicalValue = m_search.ecovalue / EcoValueBase
                            m_data.objFuncMandatedValue = m_search.manvalue / ManValueBase
                            m_data.objFuncSocialValue = m_search.Employ / EmployBase
                            m_data.objFuncEconomicValue = m_search.totval / TotValBase
                            m_data.objFuncBiomassDiv = m_search.KemptonQ / BioDiversityBase
                            m_data.objFuncAreaBorder = AreaBordary / areaBoundBase
                            m_data.objFuncTotal = (m_search.WeightedTotal + AreaBordary * m_data.BoundaryWeight) / Me.TotWeightedValueBase

                            If CurSum > SeedSumMax Then

                                m_data.bestrow = m_data.CurRow
                                m_data.bestcol = m_data.CurCol

                                SeedSumMax = CurSum

                                If SeedSumMax > TotalSearchMax Then
                                    'new highest score across all the model runs
                                    Me.setRunState(eRunStates.NewBestResultFound)
                                End If

                                'System.Console.WriteLine("m_data.bestrow = " & m_data.bestrow.ToString & ", m_data.bestcol= " & m_data.bestrow.ToString & ", TargetSum = " & TargetSum.ToString)
                            End If

                            'turn the current MPA cell off SelectNextSeedCell() will set the next cell
                            clearCurrentMPATestCells()
                            Me.m_nIters += 1

                        Else
                            'EcoSeedOn = False
                            SelectNewMPAcell()
                        End If

                    Loop ' Do While EcoSeedOn
                    If m_data.StopRun Then Exit Do

                    'All the current seed cells have been tested for the highest weighted value
                    'Add the best row col to the MPA configuration
                    'Select the next set of seed cells to test
                    If m_data.bestrow > 0 And m_data.bestcol > 0 Then

                        StoreObjectiveFunctionResults()

                        'Tell the delegate that a new best cell has been selected. 
                        'this needs to be synchronous because the best row/col are set back to -1 (not selected) right after
                        'synchronization is handled by the manager
                        Me.setRunState(eRunStates.NewCellSelected)

                        'set the MPA cell to the selected Seed cell
                        m_SpaceData.MPA(m_data.bestrow, m_data.bestcol) = m_data.MPASeed(m_data.bestrow, m_data.bestcol)
                        m_data.MPASeed(m_data.bestrow, m_data.bestcol) = 0

                        SeedSumMax = Single.MinValue
                        m_data.bestrow = -1
                        m_data.bestcol = -1

                        SetSeedCellsAdjacentToMPAs()

                    Else
                        EcoSeedOn = False
                    End If

                Loop ' Do While NotAllCellsAreMPAs

                Output()

                m_EcoSpace.SearchData.SearchMode = eSearchModes.NotInSearch
                cleanUp()

            Catch ex As Exception
                cLog.Write(ex)
                m_bRunning = False
                Debug.Assert(False, ex.StackTrace)
                Throw New ApplicationException("EcoSeed Error: " & ex.Message, ex)
            End Try

        End Sub

        ''' <summary>
        ''' Public interfaced called by Ecospace at the start of each Year
        ''' </summary>
        ''' <param name="Biomass"></param>
        ''' <param name="iYear"></param>
        ''' <remarks>This is used by Ecoseed to control the length of the Ecospace run</remarks>
        Public Sub YearTimeStep(ByRef iYear As Integer, ByVal Biomass() As Single) Implements IMPASearchModel.YearTimeStep

            If Not Me.m_bRunning Then
                'Ecoseed is not running so don't do anything
                Exit Sub
            End If

            'jb for now 
            If iYear = Me.m_data.EcoSpaceStartYear Then
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
                                m_SpaceData.Blast(i, j, ip) = Blastseed(i, j, ip)
                                m_SpaceData.Bcell(i, j, ip) = BOrig(i, j, ip)
                                m_EcoSpace.FtimeCell(i, j, ip) = FOrig(i, j, ip)
                            Next
                        Next
                    Next
                    For i = 1 To m_SpaceData.NGroups
                        biomass(i) = StoreBtimeForEcoSeed(i)
                    Next
                End If

            Catch ex As Exception
                cLog.Write(ex)
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
                Debug.Assert(False, ex.Message)
                cLog.Write(ex)
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

        ''' <summary>
        ''' Find the next set of MPA cells to evaluate
        ''' </summary>
        ''' <remarks>In EwE5 this was called EvaluateSeedCell() the original code is at the bottom inactivated by a compiler directive </remarks>
        Private Sub SelectNextSeedCell()

            Dim i As Integer, ir As Integer, ic As Integer, j As Integer

            'If EcoSeedOn Then

            'EcoSeedOn controls the evaluation loop in RunSeed()
            'it tells the loop that we have found the next seed cell/block
            EcoSeedOn = False

            For ir = 1 To m_SpaceData.Inrow
                For ic = 1 To m_SpaceData.InCol

                    If m_data.MPASeed(ir, ic) > 0 And TestedSeed(ir, ic) = False Then 'Found one

                        'EcoSeedOn controls the evaluation loop in RunSeed()
                        EcoSeedOn = True

                        m_data.CurRow = SideStep * ((ir - 1) \ SideStep) + 1
                        m_data.CurCol = SideStep * ((ic - 1) \ SideStep) + 1

                        For i = m_data.CurRow To m_data.CurRow + SideStep - 1
                            For j = m_data.CurCol To m_data.CurCol + SideStep - 1
                                If i >= 0 And i <= m_SpaceData.Inrow And j >= 0 And j <= m_SpaceData.InCol Then
                                    'has to split the next in two as i or j may exceed dimensioning
                                    If m_SpaceData.Depth(i, j) > 0 Then
                                        TestedSeed(i, j) = True

                                        ' m_data.MPASeed(i, j) make sure MPASeed() is set to an MPA index for this row and col
                                        Debug.Assert(m_data.MPASeed(i, j) <> 0, "Ecoseed MPASeed() not set correctly.")

                                        'set the MPA's to use the MPASeed for this row col
                                        'MPASeed(row,col) was set in SetSeedCellsAdjacentToMPAs()
                                        'MPA() will need to be cleared at the end of this iteration
                                        m_SpaceData.MPA(i, j) = m_data.MPASeed(ir, ic)

                                    End If ' If m_esData.Depth(i, j) > 0 Then
                                End If ' If i >= 0 And i <= m_esData.Inrow And j >= 0 And j <= m_esData.InCol Then
                            Next j
                        Next i

                        'done we have found the next seed cell/block
                        Exit Sub

                    End If ' If m_data.MPASeed(ir, ic) > 0 And TestedSeed(ir, ic) = False Then 'Found one
                Next ic
            Next ir
            'End If

            Exit Sub

#If 0 Then
            'EWE5 original code 
            'this was called EvaluateSeedCell()

            m_data.SeedBlockSize2 = 9
            ''next loop finds seed blocks, sets them according to user input seed block size abmpa
            ''If StartMPA And RunningMPA = False Then
            If EcoSeedOn Then
                EcoSeedOn = False
                For ir = 1 To m_esData.Inrow
                    For ic = 1 To m_esData.InCol

                        If EcoSeedOn Then
                            ir = m_esData.Inrow
                            ic = m_esData.InCol
                            Exit For
                        End If

                        If m_data.MPASeed(ir, ic) > 0 And TestedSeed(ir, ic) = False Then 'Found one
                            EcoSeedOn = True
                            Select Case m_data.SeedBlockSize2
                                Case 1
                                    TestedSeed(ir, ic) = True
                                    m_esData.MPA(ir, ic) = m_data.MPASeed(ir, ic)
                                    m_data.CurRow = ir
                                    m_data.CurCol = ic
                                Case 4, 9, 16, 25

                                    m_data.CurRow = SideStep * ((ir - 1) \ SideStep) + 1
                                    m_data.CurCol = SideStep * ((ic - 1) \ SideStep) + 1

                                    For i = m_data.CurRow To m_data.CurRow + SideStep - 1
                                        For j = m_data.CurCol To m_data.CurCol + SideStep - 1
                                            If i >= 0 And i <= m_esData.Inrow And j >= 0 And j <= m_esData.InCol Then
                                                'has to split the next in two as i or j may exceed dimensioning
                                                If m_esData.Depth(i, j) > 0 Then
                                                    TestedSeed(i, j) = True
                                                End If
                                            End If
                                        Next
                                    Next
                            End Select ' Select Case m_data.SeedBlockSize2

                            Exit For

                        End If ' If m_data.MPASeed(ir, ic) > 0 And TestedSeed(ir, ic) = False Then 'Found one
                    Next
                Next
            End If

            Exit Sub


           If 2 = 3 And StartMPA And RunningMPA Then
                Select Case m_data.SeedBlockSize2
                    Case 1
                        m_esData.MPA(m_data.CurRow, m_data.CurCol) = m_data.MPASeed(m_data.CurRow, m_data.CurCol)   ' 1
                        ''m_data.MPASeed(m_data.CurRow, m_data.CurCol) = 0
                        'frmSpace.MapDepth(frmSeed.MPAmap)
                        ''GetFactor1
                        'DoEvents()
                    Case 4, 9, 16, 25
                        For ir = m_data.CurRow To m_data.CurRow + SideStep - 1 : For ic = m_data.CurCol To m_data.CurCol + SideStep - 1
                                If ir <= m_esData.Inrow And ic <= m_esData.InCol Then
                                    If m_data.MPASeed(ir, ic) > 0 Then
                                        m_esData.MPA(ir, ic) = m_data.MPASeed(ir, ic)
                                        m_data.MPASeed(ir, ic) = 0
                                        'frmSpace.MapDepth(frmSeed.MPAmap)
                                        'GetFactor1
                                        m_data.MPASeed(ir, ic) = -1
                                        'DoEvents
                                    End If
                                End If
                            Next : Next
                        For ir = m_data.CurRow To m_data.CurRow + SideStep - 1 : For ic = m_data.CurCol To m_data.CurCol + SideStep - 1
                                If ir <= m_esData.Inrow And ic <= m_esData.InCol Then
                                    If m_data.MPASeed(ir, ic) = -1 Then m_data.MPASeed(ir, ic) = MPA(ir, ic) '1
                                End If
                            Next : Next
                End Select
            End If
#End If
            'If EndMPA And RunningMPA Then
            'SumValSeed = True
            'End If
            'ReDim bbTOT(NumGroups)
            'If MPAstep = 0 Then MPAstep = 1
        End Sub


        Private Sub clearCurrentMPATestCells()
            Dim ir As Integer, ic As Integer

            For ir = m_data.CurRow To m_data.CurRow + SideStep - 1
                For ic = m_data.CurCol To m_data.CurCol + SideStep - 1

                    If ir <= m_SpaceData.Inrow And ic <= m_SpaceData.InCol Then

                        If m_SpaceData.Depth(ir, ic) > 0 And m_data.MPASeed(ir, ic) > 0 Then
                            m_SpaceData.MPA(ir, ic) = 0
                        End If

                    End If ' If ir <= m_esData.Inrow And ic <= m_esData.InCol Then

                Next ic
            Next ir

        End Sub

        Private Sub SelectNewMPAcell() 'this occurs just before before start of new timestep

            Dim ir As Integer, ic As Integer, i As Integer ', j As Integer
            Dim fnum As Integer

            'jb from EwE5 m_data.SeedBlockSize2 is hardwired at 9 at the start of each run
            'so only Case 4, 9, 16, 25 can run I'm not sure what the other case is for
            Select Case m_data.SeedBlockSize2
                Case 1
                    If SeedLeft = True Then
                        fnum = m_SpaceData.MPA(m_data.CurRow, m_data.CurCol)
                        m_data.MPASeed(m_data.CurRow, m_data.CurCol) = m_SpaceData.MPA(m_data.CurRow, m_data.CurCol)   '1
                        m_SpaceData.MPA(m_data.CurRow, m_data.CurCol) = 0 ' fnum
                    End If
                Case 4, 9, 16, 25
                    For ir = m_data.CurRow To m_data.CurRow + SideStep - 1
                        For ic = m_data.CurCol To m_data.CurCol + SideStep - 1
                            If ir <= m_SpaceData.Inrow And ic <= m_SpaceData.InCol Then
                                If m_SpaceData.Depth(ir, ic) > 0 And m_data.MPASeed(ir, ic) > 0 Then
                                    m_SpaceData.MPA(ir, ic) = 0
                                End If

                                'm_data.MPASeed(IR, ic) = 1
                            End If
                        Next ic
                    Next ir
            End Select

            MPARow(MPAstep) = m_data.bestrow
            MPACol(MPAstep) = m_data.bestcol
            MPAstep = MPAstep + 1
            'Count how many MPA cells we have now
            i = 0
            For ir = 1 To m_SpaceData.Inrow
                For ic = 1 To m_SpaceData.InCol
                    If m_SpaceData.MPA(ir, ic) > 0 Then i = i + 1
                Next
            Next
            MPAcount(MPAstep - 1) = i
            ir = 0

            If ir = m_SpaceData.nFleets Then 'NO MORE FISHING GOING ON
                MPAstep = m_SpaceData.Inrow * m_SpaceData.InCol + 1
                'Ecoseed.StartEvaluateSeedCell
            End If
            If SeedLeft = False Then
                'And TargetSumMax <= 0 Then 'VESSELS CANT MAKE ANY MONEY
                'MsgBox "The Ecoseed routine can no longer add MPA cells:" _
                '& vbNewLine + "the rent(s) for all fishery(ies) are =< 0." _
                '& vbNewLine + "The Ecospace routine will now continue without Ecoseed.", vbInformation + vbOKOnly
                EcoSeedOn = False
                MPAstep = m_SpaceData.Inrow * m_SpaceData.InCol + 1
            End If

            'villy: this next section only allows 'adjacent' cells to become seed cells _
            '- time saver ordered by daniel AB02242000
            SetSeedCellsAdjacentToMPAs() 's

            Erase TestedSeed
            ReDim TestedSeed(m_SpaceData.Inrow, m_SpaceData.InCol)

        End Sub


        Private Sub SetSeedCellsAdjacentToMPAs()
            Dim ir As Integer
            Dim ic As Integer
            Dim iro As Integer
            Dim ico As Integer
            Dim iTemp As Integer

            For iro = 1 To m_SpaceData.Inrow
                For ico = 1 To m_SpaceData.InCol
                    If m_SpaceData.MPA(iro, ico) > 0 Then
                        'get the MPA index of the current row col
                        'this index will be used to set the neighbouring cells
                        iTemp = m_SpaceData.MPA(iro, ico)
                        Select Case m_data.SeedBlockSize2
                            Case 1
                                If m_SpaceData.MPA(iro - 1, ico) = 0 And m_SpaceData.Depth(iro - 1, ico) > 0 Then m_data.MPASeed(iro - 1, ico) = iTemp '1  'cell above is m_esdata.m_data.MPASeed
                                If m_SpaceData.MPA(iro + 1, ico) = 0 And m_SpaceData.Depth(iro + 1, ico) > 0 Then m_data.MPASeed(iro + 1, ico) = iTemp '1 'cell below is m_esdata.m_data.MPASeed
                                If m_SpaceData.MPA(iro, ico - 1) = 0 And m_SpaceData.Depth(iro, ico - 1) > 0 Then m_data.MPASeed(iro, ico - 1) = iTemp '1 'cell left is m_esdata.m_data.MPASeed
                                If m_SpaceData.MPA(iro, ico + 1) = 0 And m_SpaceData.Depth(iro, ico + 1) > 0 Then m_data.MPASeed(iro, ico + 1) = iTemp '1 'cell right is m_esdata.m_data.MPASeed
                            Case 4, 9, 16, 25
                                'Cells above:
                                For ir = iro - SideStep To iro - 1
                                    For ic = ico To ico + SideStep - 1
                                        If ir >= 0 And ir <= m_SpaceData.Inrow And ic >= 0 And ic <= m_SpaceData.InCol Then
                                            If m_SpaceData.MPA(ir, ic) = 0 And m_SpaceData.Depth(ir, ic) > 0 Then
                                                m_data.MPASeed(ir, ic) = iTemp '1 'cell above is m_esdata.m_data.MPASeed
                                            End If
                                        End If
                                    Next
                                Next
                                'cells below:
                                For ir = iro + SideStep To iro + 2 * SideStep - 1
                                    For ic = ico To ico + SideStep - 1
                                        If ir >= 0 And ir <= m_SpaceData.Inrow And ic >= 0 And ic <= m_SpaceData.InCol Then
                                            If m_SpaceData.MPA(ir, ic) = 0 And m_SpaceData.Depth(ir, ic) > 0 Then
                                                m_data.MPASeed(ir, ic) = iTemp '1 'cell above is m_esdata.m_data.MPASeed
                                            End If
                                        End If
                                    Next
                                Next
                                'cells to the left:
                                For ir = iro To iro + SideStep - 1 : For ic = ico - SideStep To ico - 1
                                        If ir >= 0 And ir <= m_SpaceData.Inrow And ic >= 0 And ic <= m_SpaceData.InCol Then
                                            If m_SpaceData.MPA(ir, ic) = 0 And m_SpaceData.Depth(ir, ic) > 0 Then
                                                m_data.MPASeed(ir, ic) = iTemp  '1 'cell above is m_esdata.m_data.MPASeed
                                            End If
                                        End If
                                    Next : Next
                                'cells to the right:
                                For ir = iro To iro + SideStep - 1
                                    For ic = ico + SideStep To ico + 2 * SideStep - 1
                                        If ir >= 0 And ir <= m_SpaceData.Inrow And ic >= 0 And ic <= m_SpaceData.InCol Then
                                            If m_SpaceData.MPA(ir, ic) = 0 And m_SpaceData.Depth(ir, ic) > 0 Then
                                                m_data.MPASeed(ir, ic) = iTemp  '1 'cell above is m_esdata.m_data.MPASeed
                                            End If
                                        End If 'If ir >= 0 And ir <= m_esData.Inrow And ic >= 0 And ic <= m_esData.InCol Then
                                    Next ic
                                Next ir
                        End Select
                    End If ' If m_esData.MPA(iro, ico) > 0 Then
                Next ico
            Next iro
        End Sub


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
            BioDiversityBase = m_search.KemptonQ

            areaBoundBase = CalculateAreaOverBondaryLength()

            If TotValBase = 0 Then TotValBase = 1
            If TotValBase < 0 Then TotValBase = -TotValBase
            If EmployBase = 0 Then EmployBase = 1
            If EmployBase < 0 Then EmployBase = -EmployBase
            If ManValueBase = 0 Then ManValueBase = 1
            If EcoValueBase = 0 Then EcoValueBase = 1
            If BioDiversityBase = 0 Then BioDiversityBase = 1

            TotWeightedValueBase = 0 + m_search.ValWeight(1) * TotValBase + m_search.ValWeight(2) * EmployBase + _
                                    m_search.ValWeight(3) * ManValueBase + m_search.ValWeight(4) * EcoValueBase + _
                                    m_search.ValWeight(5) * BioDiversityBase + m_data.BoundaryWeight * areaBoundBase

            '   System.Console.WriteLine("EcoSeed weighted base value = " & TotWeightedValueBase.ToString)

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

                'keep the results in memory
                m_lstObjectiveResults.Add(New cObjectiveResult(m_data, m_SpaceData))

                'Memory management for results
                If Me.m_lstObjectiveResults.Count >= N_MAX_RESULTS Then
                    'sorts in decending order (biggest objFuncTotal first)
                    Me.m_lstObjectiveResults.Sort()
                    'remove lowest results from the end of the list
                    Me.m_lstObjectiveResults.RemoveRange(RESULTS_TO_KEEP - 1, Me.m_lstObjectiveResults.Count - RESULTS_TO_KEEP)
                End If

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
            sb.AppendLine("Row, Col, Economic, Social, Mandated, Ecosystem, Area/Border")
            sb.AppendLine(" , , " & String.Format("{0:F}, {1:F}, {2:F}, {3:F}, {4:F}", _
                            m_search.ValWeight(1), m_search.ValWeight(2), m_search.ValWeight(3), m_search.ValWeight(4), m_data.BoundaryWeight))

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
                sb.Append(String.Format("{0:N}, {1:N}, {2:F}, {3:N}, {4:N}, {5:N}, {6:N}", _
                                m_data.bestrow, m_data.bestcol, m_data.objFuncEconomicValue, m_data.objFuncSocialValue, m_data.objFuncMandatedValue, m_data.objFuncEcologicalValue, m_data.objFuncAreaBorder))

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
            '  Erase Bseed
            Erase Blastseed
            '   Erase Fseed
            '   Erase Wseed
            '   Erase m_data.MPASeed
            'Erase MPARow
            'Erase MPACol
            'Erase EffortMPA
            'Erase MPAcount

        End Sub

        Private Sub RedimSeedVariables()
            Dim nvartot As Integer = m_SpaceData.NGroups + 2


            'ReDim Blast(m_esdata.Inrow + 1, m_esdata.Incol + 1, NvarTot) As Single
            '  ReDim Port(NumGear, m_esdata.Inrow + 1, m_esdata.Incol + 1)
            ReDim BOrig(m_SpaceData.Inrow + 1, m_SpaceData.InCol + 1, nvartot)
            ReDim FOrig(m_SpaceData.Inrow + 1, m_SpaceData.InCol + 1, nvartot)
            ReDim WOrig(m_SpaceData.Inrow + 1, m_SpaceData.InCol + 1, nvartot)
            '     ReDim Bseed(m_SpaceData.Inrow + 1, m_SpaceData.InCol + 1, nvartot)
            ReDim Blastseed(m_SpaceData.Inrow + 1, m_SpaceData.InCol + 1, nvartot)
            '   ReDim Fseed(m_SpaceData.Inrow + 1, m_SpaceData.InCol + 1, nvartot)
            '    ReDim Wseed(m_SpaceData.Inrow + 1, m_SpaceData.InCol + 1, nvartot)
            'ReDim m_data.MPASeed(m_esData.Inrow + 1, m_esData.InCol + 1)
            'ReDim EffortYear(NumGear)
            'ReDim EffortYearSeed(NumGear)
            'ReDim ValGear(NumGear)
            'ReDim SailTot(TotalTime * 12, NumGear), ValRatio(NumGear), SailMax(NumGear)
            'ReDim ValratioSeed(NumGear), ValGearSeed(NumGear), SailTotSeed(NumGear)
            'ReDim MPAVal(m_esData.Inrow + 1, m_esData.InCol + 1)
            'ReDim MpaBioValStore(m_esData.Inrow + 1, m_esData.InCol + 1)
            'ReDim MpaGearProfitsStore(m_esData.Inrow + 1, m_esData.InCol + 1)
            'ReDim MPABio(m_esData.Inrow + 1, m_esData.InCol + 1)
            'ReDim GearRent(NumGear, m_esData.Inrow + 1, m_esData.InCol + 1)
            'ReDim bbTOT(NumGroups)
            ReDim MPAcount(m_SpaceData.Inrow * m_SpaceData.InCol + 1)
            ReDim MPARow(m_SpaceData.Inrow * m_SpaceData.InCol + 1)
            ReDim MPACol(m_SpaceData.Inrow * m_SpaceData.InCol + 1)
            'ReDim EffortMPA(m_SpaceData.nFleets, m_SpaceData.Inrow * m_SpaceData.InCol + 1)
            ReDim StoreBtimeForEcoSeed(m_SpaceData.NGroups)
            'ReDim MPAVal2a(m_esData.Inrow * m_esData.InCol + 1), MPAVal2b(m_esData.Inrow * m_esData.InCol + 1), MPABio2(m_esData.Inrow * m_esData.InCol + 1)
            'ReDim GearRentStore(NumGear, m_esData.Inrow * m_esData.InCol + 1)
        End Sub
#End Region

    End Class


End Namespace
