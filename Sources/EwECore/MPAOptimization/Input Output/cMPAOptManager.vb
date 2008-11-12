'==============================================================================
'
' $Log: cMPAOptManager.vb,v $
' Revision 1.5  2008/11/12 20:39:45  sherman
' Added Ecoseed null seed/mpa test.
'
' Revision 1.4  2008/11/12 20:20:57  joeb
' added BiomassDiversity to MPA stuff
'
' Revision 1.3  2008/11/12 19:14:14  joeb
' CellSelectedMap now contains  PercentAreaClosedFilter
'
' Revision 1.2  2008/10/31 17:25:46  jeroens
' Uses MPA opt plugins
'
' Revision 1.1  2008/09/26 07:30:25  sherman
' --== DELETED HISTORY ==--
'
'===================================================

Option Strict On

Imports EwECore.EcoSeed
Imports System.Threading
Imports EwECore.SearchObjectives
Imports EwEUtils.Core
Imports EwEPlugin

#Region "Enums"

'init
'running

Public Enum eRunStates
    ' Started
    Initializing
    Searching
    Completed
    ''' <summary>Ecoseed has selected a new cell to add to the current MPA configuration</summary>
    NewCellSelected

    ''' <summary>
    ''' New BestResult found for both EcoSeed and Random search
    ''' </summary>
    ''' <remarks></remarks>
    NewBestResultFound
End Enum

#End Region

#Region "Delegates for model to comunicate back to the manager"

''' <summary>
''' Ecoseed has computed the value of a cell
''' </summary>
''' <remarks></remarks>
Public Delegate Sub SearchIterationDelegate()

''' <summary>
''' An Ecoseed run has started
''' </summary>
''' <remarks></remarks>
Public Delegate Sub RunStateDelegate(ByVal RunState As eRunStates)


Public Delegate Sub SendMessageDelegate(ByVal message As EwECore.cMessage)

#End Region

#Region "Optimization Manager"


Public Class cMPAOptManager
    Inherits cThreadWaitBase 'for thread blocking
    Implements ICoreInterface
    Implements SearchObjectives.ISearchObjective

#Region "Private Data"

    Private m_syncObject As System.ComponentModel.ISynchronizeInvoke
    Private m_bConnected As Boolean

    'Private m_seed As cEcoSeed
    Private m_MPASearch As IMPASearchModel
    Private m_core As cCore
    Private m_searchObjectives As cSearchObjective

    Private m_SeedCellComputedCallback As SearchIterationDelegate
    Private m_SeedRunStateCallback As RunStateDelegate
    Private m_thrSeed As Threading.Thread
    Private m_curRowCol As cMPAOptOutput

    Private m_parameters As cMPAOptParameters

    Private m_orgMPAConfig(,) As Integer

    'directory for the output data
    'this can be switched to use a default data directory supplied by the core
    Private m_dataDir As String

#End Region

#Region "Construction and Initialization"


    Friend Function Init(ByRef theCore As cCore) As Boolean Implements ISearchObjective.Init

        Try
            m_core = theCore
            m_searchObjectives = m_core.SearchObjective
            m_curRowCol = New cMPAOptOutput(theCore)
            m_parameters = New cMPAOptParameters(theCore)

            Me.setDefaults()

            Me.setActiveSearch(Me.m_core.m_MPAOptData.SearchType, True)

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & " Failed to initialize EcoSeed.")
        End Try

    End Function

    Public Sub Connect(ByVal syncObject As System.ComponentModel.ISynchronizeInvoke, ByVal SeedCellCallback As SearchIterationDelegate, ByVal RunStateCallback As RunStateDelegate)

        m_syncObject = syncObject
        m_SeedCellComputedCallback = SeedCellCallback
        m_SeedRunStateCallback = RunStateCallback

        Debug.Assert(m_syncObject IsNot Nothing, Me.ToString & ".Connect() syncObject is null.")
        Debug.Assert(SeedCellCallback IsNot Nothing, Me.ToString & ".Connect() SeedCellCallback is null.")
        Debug.Assert(m_SeedRunStateCallback IsNot Nothing, Me.ToString & ".Connect() SeedCellCallback is null.")

        If m_syncObject IsNot Nothing And m_SeedCellComputedCallback IsNot Nothing And m_SeedRunStateCallback IsNot Nothing Then
            m_bConnected = True
        Else
            m_bConnected = False
            cLog.Write("EcoSeedManager is not connected to an interface.")
        End If

    End Sub


    Private Sub setDefaults()

        'set the default period to run the model to be the same as the interface
        Me.m_core.MPAOptData.EcoSpaceEndYear = Me.m_core.nEcospaceYears
        Me.m_dataDir = System.AppDomain.CurrentDomain.BaseDirectory()

    End Sub

#End Region

#Region "Private methods"

#Region "Changing search models"

    Private Function SearchModelFactory(ByVal SearchType As eMPAOptimizationModels) As IMPASearchModel

        Debug.Assert(Me.isRunning = False, Me.ToString & " Cannot change the search type while a search is running.")
        If Me.isRunning Then
            Return Me.m_MPASearch
        End If

        Dim search As IMPASearchModel
        Select Case SearchType

            Case eMPAOptimizationModels.EcoSeed
                search = New cEcoSeed

                'Data directory can be switched to come from the core (if that ever exists)
                search.OutPutFilename = System.IO.Path.Combine(Me.m_dataDir, "MPAOpt_EcoSeed_Output.csv")

            Case eMPAOptimizationModels.RandomSearch
                search = New cMPARandomSearch
                search.OutPutFilename = System.IO.Path.Combine(Me.m_dataDir, "MPAOpt_Random_Output.csv")

        End Select

        Return search

        Throw New ApplicationException(Me.ToString & ".SearchModelFactory() " & Me.m_core.MPAOptData.SearchType.ToString & " is not a supported Search Model type.")

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Toggles the active search model to use.
    ''' </summary>
    ''' <param name="newActiveSearch">New search model to use.</param>
    ''' <param name="bForceInit">Flag indicating whether the search should be
    ''' initialized, even when the active search type does not change.</param>
    ''' -----------------------------------------------------------------------
    Private Sub setActiveSearch(ByVal newActiveSearch As eMPAOptimizationModels, ByVal bForceInit As Boolean)

        'if no search has been created then make sure the factory runs
        If Me.m_MPASearch IsNot Nothing Then

            Debug.Assert(Me.isRunning = False, Me.ToString & " Can not change the search type while a search is running.")
            If Me.isRunning Then
                System.Console.WriteLine(Me.ToString & " tried to change the search model while a search is running. Sorry Dude!!!")
                Exit Sub
            End If

            'the search type being set is the same as the current one so don't do anything
            If (newActiveSearch = Me.m_core.m_MPAOptData.SearchType) And (bForceInit = False) Then
                Exit Sub
            End If

        End If

        Try
            Me.m_MPASearch = Me.SearchModelFactory(newActiveSearch)
            Me.m_MPASearch.Init(Me.m_core.m_Ecospace, Me.m_core.MPAOptData)
        Catch ex As Exception
            Debug.Assert(False, ex.Message)
            Throw New ApplicationException("Error changing search models " & ex.Message, ex)
        End Try

        Me.m_core.m_MPAOptData.SearchType = newActiveSearch

    End Sub

#End Region

#Region "Events callbacks from Search Models"

    Private Sub OnSearchIteration()

        Try

            'populate the current row col
            m_curRowCol.Init(m_MPASearch.MPAOptData, Me.m_core.m_EcoSpaceData)

            If m_bConnected Then
                m_syncObject.BeginInvoke(Me.m_SeedCellComputedCallback, Nothing)
            Else
                System.Console.WriteLine("EcoSeedManager not connected to an interface.")
            End If

        Catch ex As Exception

        End Try

    End Sub


    Private Sub OnEcoSeedRunStateChanged(ByVal RunState As eRunStates)

        Try

            Dim dlgt As New InvokePluginPointDelegate(AddressOf InvokePluginPoint)

            If RunState = eRunStates.NewBestResultFound Then
                m_curRowCol.Init(m_MPASearch.MPAOptData, Me.m_core.m_EcoSpaceData)
            End If

            If RunState = eRunStates.Completed Then
                'the run has completed release any waiting threads
                Me.ReleaseWait()
            End If

            If m_bConnected Then

                'Invoke will wait for the function to return 
                'this lets the interface gather data before it has changed is response to a new best cell selected
                Me.m_syncObject.Invoke(Me.m_SeedRunStateCallback, New Object() {RunState})

                ' Invoke plugins
                Me.m_syncObject.Invoke(dlgt, New Object() {RunState})

            Else
                System.Console.WriteLine("EcoSeedManager not connected to an interface.")
            End If

        Catch ex As Exception
            cLog.Write(ex)
        End Try

    End Sub

    Private Sub OnSendMessage(ByVal Message As EwECore.cMessage)

    End Sub

    Private Delegate Sub InvokePluginPointDelegate(ByVal RunState As eRunStates)

    Private Sub InvokePluginPoint(ByVal RunState As eRunStates)

        Dim pm As cPluginManager = Me.m_core.PluginManager

        If (pm IsNot Nothing) Then

            Select Case RunState

                Case eRunStates.Initializing
                    pm.MPAOptimizationsSearchInitialized(Me.m_MPASearch, Me.m_core.m_MPAOptData.SearchType)
                Case eRunStates.Searching
                    pm.MPAOptimizationsSearchStart(Me.m_MPASearch, Me.m_core.m_MPAOptData.SearchType)
                Case eRunStates.Completed
                    pm.MPAOptimizationsSearchEnd(Me.m_MPASearch, Me.m_core.m_MPAOptData.SearchType)
                Case eRunStates.NewBestResultFound
                    pm.MPAOptimizationsSearchNewBestResultFound(Me.m_MPASearch, Me.m_core.m_MPAOptData.SearchType)
                Case eRunStates.NewCellSelected
                    Debug.Assert(Me.m_core.m_MPAOptData.SearchType = eMPAOptimizationModels.EcoSeed)
                    pm.EcoseedNewCellSelected(Me.m_MPASearch)

            End Select

        End If

    End Sub

#End Region

#Region "Internal methods"
    ''' <summary>Checks if Seeds cells exist or MPA's, only if Ecoseed is running.</summary>
    ''' <returns>True if any of the above exists.</returns>
    Private Function CheckForSeedsAndMPA() As Boolean
        Dim val As Boolean = False

        ' Check if it Ecoseed enabled
        If Me.m_parameters.SearchType <> eMPAOptimizationModels.EcoSeed Then
            Return True
        End If

        ' Check seeds
        For ir As Integer = 1 To Me.m_core.m_EcoSpaceData.Inrow
            For ic As Integer = 1 To Me.m_core.m_EcoSpaceData.InCol
                'Me.m_core.m_EcoSpaceData.s(ir, ic) = 0
            Next ic
        Next ir


        ' Check MPAs
        ' Check within MPAs
        For ir As Integer = 1 To Me.m_core.m_EcoSpaceData.Inrow
            For ic As Integer = 1 To Me.m_core.m_EcoSpaceData.InCol
                'Me.m_core.m_EcoSpaceData.MPA(ir, ic) = 0
            Next ic
        Next ir

        ' Just make it work for now.
        val = True

        Return val
    End Function
#End Region ' Internal Methods

#End Region

#Region "Public methods"

    Public Function Run() As Boolean

        Try

            m_MPASearch.Connect(AddressOf OnSearchIteration, AddressOf Me.OnEcoSeedRunStateChanged, AddressOf Me.OnSendMessage)

            If Me.isRunning Then
                Me.m_core.Messages.SendMessage(New cMessage("Optimization is already running. Only one evaluation can be run at a time.", eMessageType.ErrorEncountered, eMessageSource.EcoSpace, eMessageImportance.Critical))
                Return False
            End If

            ' Test if no seed cells nor MPA
            If Me.CheckForSeedsAndMPA() Then
                Dim msg As New cFeedbackMessage("No Seed selected nor MPA's set, optimzation may yield unknown results. Would you like to continue?", eMessageSource.MPAOptimization, eMessageImportance.Warning, cFeedbackMessage.eReplyStyle.OK_CANCEL, eDataTypes.MPAOptParameters, cFeedbackMessage.eReply.CANCEL)
                If msg.Reply = cFeedbackMessage.eReply.CANCEL Or msg.Reply = cFeedbackMessage.eReply.NO Then Return False
            End If

            Me.setWait()

            'keep a copy of the original MPA configuration
            ReDim m_orgMPAConfig(Me.m_core.m_EcoSpaceData.Inrow + 1, Me.m_core.m_EcoSpaceData.InCol + 1)
            Array.Copy(Me.m_core.m_EcoSpaceData.MPA, m_orgMPAConfig, Me.m_core.m_EcoSpaceData.MPA.Length)

            m_thrSeed = New Threading.Thread(AddressOf m_MPASearch.Run)
            m_thrSeed.Start()

        Catch ex As Exception
            cLog.Write(ex)
            Me.m_core.Messages.SendMessage(New cMessage("Ecoseed Error: " & ex.Message, eMessageType.ErrorEncountered, eMessageSource.EcoSpace, eMessageImportance.Critical))
            Me.ReleaseWait()
            Return False
        End Try

        Return True

    End Function

    Public Sub YearTimeStep(ByRef iYear As Integer, ByVal Biomass() As Single)
        m_MPASearch.YearTimeStep(iYear, Biomass)
    End Sub

    Public Sub StopRun()
        m_MPASearch.StopRun()
    End Sub

    Public Sub clearMPAs()
        Me.m_MPASearch.clearMPAs()
    End Sub

    Public Sub clearSeedCells()
        Me.m_MPASearch.clearSeedCells()
    End Sub

    Public Function setAllCellsToMPA(ByVal iMPA As Integer) As Boolean
        Return Me.m_MPASearch.setAllCellsToMPA(iMPA)
    End Function

    Public Function setAllCellsToSeed(ByVal iMPA As Integer) As Boolean
        Return Me.m_MPASearch.setAllCellsToSeed(iMPA)
    End Function

    ''' <summary>
    ''' Array of the number of times a cell was selected during the search
    ''' </summary>
    ''' <param name="TopPercentile">Top percentile of search results to include in the map</param>
    ''' <param name="NumberOfResults">Number of results in the top percentile</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CellSelectedMap(ByVal TopPercentile As Integer, ByRef NumberOfResults As Integer, ByVal PercentAreaClosedFilter As Integer) As Integer(,)
        Dim map(,) As Integer
        Dim nResults As Integer
        Dim obj As cObjectiveResult

        Try

            Dim nR As Integer = Me.m_core.m_EcoSpaceData.Inrow
            Dim nC As Integer = Me.m_core.m_EcoSpaceData.InCol
            ReDim map(nR, nC)

            'bound the percentile
            TopPercentile = Math.Min(100, Math.Max(1, TopPercentile))

            'turn the TopPercentile into the number of results
            nResults = CInt(Math.Ceiling(Me.m_MPASearch.nInterationsCompleted * TopPercentile \ 100))

            'bound nResults
            If nResults < 1 Then nResults = 1
            If nResults > Me.m_MPASearch.Results.Count Then nResults = Me.m_MPASearch.Results.Count

            Debug.Assert(nResults <= Me.m_MPASearch.Results.Count, Me.ToString & ".CellMap() Error computing number of results to use for map.")

            For ires As Integer = 0 To nResults - 1

                obj = Me.m_MPASearch.Results.Item(ires)

                If obj.PercentageClosed = PercentAreaClosedFilter Then
                    NumberOfResults += 1
                    'count the number of hits to each cell in the map
                    For Each cell As cMPACell In obj.Cells
                        map(cell.Row, cell.Col) += 1
                    Next cell
                End If
            Next ires

            Return map

        Catch ex As Exception
            cLog.Write(ex)
            Debug.Assert(False, ex.Message)
            Me.m_core.Messages.SendMessage(New cMessage("MPA Optimization Error: " & ex.Message, eMessageType.ErrorEncountered, eMessageSource.SearchObjective, eMessageImportance.Critical))
            Return Nothing

        End Try

    End Function

#End Region

#Region " Public properties "

    ''' <summary>
    ''' Output object for the current Ecoseed interation 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property CurrentRowColResults() As cMPAOptOutput

        Get
            Return m_curRowCol
        End Get

    End Property


    Public ReadOnly Property MPAOptimizationParamters() As cMPAOptParameters
        Get
            Return m_parameters
        End Get
    End Property

    '''' <summary>
    '''' Best search result up to the current search iteration
    '''' </summary>
    '''' <value></value>
    '''' <returns></returns>
    '''' <remarks></remarks>
    'Public ReadOnly Property BestResult() As cObjectiveResult
    '    Get
    '        Return Me.m_MPASearch.BestResult
    '    End Get
    'End Property


    Public ReadOnly Property Results() As List(Of cObjectiveResult)
        Get
            'make sure the results are sorted
            Me.m_MPASearch.Results.Sort()
            Return Me.m_MPASearch.Results
        End Get
    End Property

    Public ReadOnly Property OrgMPA() As Integer(,)
        Get
            Return Me.m_orgMPAConfig
        End Get
    End Property

    Public ReadOnly Property nIterationsCompleted() As Integer
        Get
            Return Me.m_MPASearch.nInterationsCompleted
        End Get
    End Property

    Public Property OutputFileName() As String
        Get
            Return Me.m_MPASearch.OutPutFilename
        End Get
        Set(ByVal value As String)
            Me.m_MPASearch.OutPutFilename = value
        End Set
    End Property

#End Region ' Public properties

#Region "ICoreInterface"

    Public ReadOnly Property DataType() As eDataTypes Implements ICoreInterface.DataType
        Get
            Return eDataTypes.MPAOptManager
        End Get
    End Property

    Public Property DBID() As Integer Implements ICoreInterface.DBID
        Get
            Return cCore.NULL_VALUE
        End Get
        Set(ByVal value As Integer)
            Debug.Assert(False, Me.ToString & ".DBID no implementation.")
        End Set
    End Property

    Public Function GetID() As String Implements ICoreInterface.GetID
        Return cCore.NULL_VALUE.ToString
    End Function

    Public Property Index() As Integer Implements ICoreInterface.Index
        Get
            Return cCore.NULL_VALUE
        End Get
        Set(ByVal value As Integer)
            Debug.Assert(False, Me.ToString & ".Index no implementation.")
        End Set
    End Property

    Public Property Name() As String Implements ICoreInterface.Name
        Get
            Return Me.ToString
        End Get
        Set(ByVal value As String)
            Debug.Assert(False, Me.ToString & ".Name no implementation.")
        End Set
    End Property

#End Region

#Region "ISearchObjective implementation"

    Public ReadOnly Property FleetObjectives(ByVal iFleet As Integer) As cSearchObjectiveFleetInput Implements ISearchObjective.FleetObjectives
        Get
            Return Me.m_searchObjectives.FleetObjectives(iFleet)
        End Get
    End Property

    Public ReadOnly Property GroupObjectives(ByVal iGroup As Integer) As cSearchObjectiveGroupInput Implements ISearchObjective.GroupObjectives
        Get
            Return Me.m_searchObjectives.GroupObjectives(iGroup)
        End Get
    End Property

    Public Function Load() As Boolean Implements ISearchObjective.Load

        Try
            Dim coreData As cMPAOptDataStructures = Me.m_core.MPAOptData
            Me.m_parameters.AllowValidation = False
            Me.m_parameters.SearchType = coreData.SearchType
            Me.m_parameters.StepSize = coreData.stepSize
            Me.m_parameters.BoundaryWeight = coreData.BoundaryWeight
            Me.m_parameters.MaxArea = coreData.MaxArea
            Me.m_parameters.MinArea = coreData.MinArea
            Me.m_parameters.nIterations = coreData.nIterations
            Me.m_parameters.iMPAToUse = coreData.iMPAtoUse
            Me.m_parameters.bUseCellWeight = coreData.bUseCellWeight

            Me.m_parameters.StartYear = coreData.EcoSpaceStartYear
            Me.m_parameters.EndYear = coreData.EcoSpaceEndYear

            Me.m_parameters.AllowValidation = True

            Me.m_parameters.ResetStatusFlags()

        Catch ex As Exception
            cLog.Write(ex)
            Debug.Assert(False, ex.Message)
            Me.m_parameters.AllowValidation = True
            Return False
        End Try

        Return True

    End Function

    Public Function Update(ByVal DataType As eDataTypes) As Boolean Implements ISearchObjective.Update

        Try

            Dim coreData As cMPAOptDataStructures = Me.m_core.MPAOptData

            coreData.stepSize = Me.m_parameters.StepSize
            coreData.BoundaryWeight = Me.m_parameters.BoundaryWeight
            coreData.MaxArea = Me.m_parameters.MaxArea
            coreData.MinArea = Me.m_parameters.MinArea
            coreData.nIterations = Me.m_parameters.nIterations
            coreData.iMPAtoUse = Me.m_parameters.iMPAToUse
            coreData.bUseCellWeight = Me.m_parameters.bUseCellWeight

            coreData.EcoSpaceStartYear = Me.m_parameters.StartYear
            coreData.EcoSpaceEndYear = Me.m_parameters.EndYear

            Me.setActiveSearch(Me.m_parameters.SearchType, False)

        Catch ex As Exception
            cLog.Write(ex)
            Debug.Assert(False, ex.Message)
            Return False
        End Try

        Return True

    End Function

    Public ReadOnly Property ValueWeights() As cSearchObjectiveWeights Implements ISearchObjective.ValueWeights
        Get
            Return Me.m_searchObjectives.ValueWeights
        End Get
    End Property

    Public ReadOnly Property ObjectiveParameters() As SearchObjectives.cSearchObjectiveParameters Implements SearchObjectives.ISearchObjective.ObjectiveParameters
        Get
            Return Me.m_searchObjectives.ObjectiveParameters
        End Get
    End Property

#End Region

End Class

#End Region

#Region "IMPASearchModel definition"

Public Interface IMPASearchModel

    Sub Run()

    Function Init(ByRef EcoSpaceModel As cEcoSpace, ByRef MPAOptData As cMPAOptDataStructures) As Boolean

    Sub Connect(ByVal OnSearchInteration As SearchIterationDelegate, ByVal OnRunStateChanged As RunStateDelegate, ByVal OnSendMessage As SendMessageDelegate)

    Sub StopRun()
    Sub clearMPAs()
    Sub clearSeedCells()
    Function setAllCellsToMPA(ByVal iMPA As Integer) As Boolean
    Function setAllCellsToSeed(ByVal iMPA As Integer) As Boolean

    Property MPAOptData() As cMPAOptDataStructures
    ReadOnly Property isRunning() As Boolean
    ReadOnly Property EcospaceStartTime() As Single
    ReadOnly Property Results() As List(Of cObjectiveResult)
    ReadOnly Property nInterationsCompleted() As Integer

    Property OutPutFilename() As String

    Sub YearTimeStep(ByRef iYear As Integer, ByVal Biomass() As Single)

End Interface

#End Region

#Region "cObjectiveResult class definition"

Public Class cObjectiveResult
    Implements IComparable(Of cObjectiveResult)

    Public Row As Integer
    Public Col As Integer
    Public objFuncEconomicValue As Single
    Public objFuncMandatedValue As Single
    Public objFuncSocialValue As Single
    Public objFuncEcologicalValue As Single
    Public objFuncAreaBorder As Single
    Public objBiomassDiversity As Single


    ''' <summary>
    ''' Includes weights
    ''' </summary>
    ''' <remarks></remarks>
    Public objFuncTotal As Single

    Public SearchType As eMPAOptimizationModels
    Public Cells As List(Of cMPACell)
    Public PercentageClosed As Integer


    Public Sub New(ByRef MPAData As cMPAOptDataStructures, ByRef SpaceData As cEcospaceDataStructures)

        Row = MPAData.bestrow
        Col = MPAData.bestcol

        objFuncEconomicValue = MPAData.objFuncEconomicValue
        objFuncMandatedValue = MPAData.objFuncMandatedValue
        objFuncSocialValue = MPAData.objFuncSocialValue
        objFuncEcologicalValue = MPAData.objFuncEcologicalValue
        objBiomassDiversity = MPAData.objBiomassDiversity

        objFuncAreaBorder = MPAData.objFuncAreaBorder

        objFuncTotal = MPAData.objFuncTotal

        SearchType = MPAData.SearchType

        'copy the list of cells into a new list 
        Cells = New List(Of cMPACell)(MPAData.Cells)

        calcPercentageClosed(MPAData, SpaceData)

    End Sub

    Public Sub Init(ByRef MPAData As cMPAOptDataStructures, ByRef SpaceData As cEcospaceDataStructures)


        Try
            objFuncEconomicValue = MPAData.objFuncEconomicValue
            objFuncMandatedValue = MPAData.objFuncMandatedValue
            objFuncSocialValue = MPAData.objFuncSocialValue
            objFuncEcologicalValue = MPAData.objFuncEcologicalValue
            objFuncAreaBorder = MPAData.objFuncAreaBorder
            objBiomassDiversity = MPAData.objBiomassDiversity
            objFuncTotal = MPAData.objFuncTotal

            Select Case MPAData.SearchType

                Case eMPAOptimizationModels.EcoSeed

                    Debug.Assert(SpaceData IsNot Nothing, Me.ToString & ".Init() SpaceData must be passed in!")
                    Cells.Clear()
                    For ir As Integer = 1 To SpaceData.Inrow
                        For ic As Integer = 1 To SpaceData.InCol
                            If SpaceData.MPA(ir, ic) <> 0 Then
                                Cells.Add(New cMPACell(ir, ic, SpaceData.MPA(ir, ic)))
                            End If
                        Next
                    Next

                Case eMPAOptimizationModels.RandomSearch
                    Cells = New List(Of cMPACell)(MPAData.Cells)

            End Select

            calcPercentageClosed(MPAData, SpaceData)

        Catch ex As Exception
            cLog.Write(ex)
            Throw New ApplicationException(Me.ToString & ".Init() Error: " & ex.Message, ex)
        End Try

    End Sub


    Private Sub calcPercentageClosed(ByRef MPAData As cMPAOptDataStructures, ByRef SpaceData As cEcospaceDataStructures)
        'what percentage of the area is closed
        Dim nTotCells As Integer = SpaceData.nWaterCells
        Dim nMPACells As Integer
        For ir As Integer = 1 To SpaceData.Inrow
            For ic As Integer = 1 To SpaceData.InCol
                If SpaceData.MPA(ir, ic) = MPAData.iMPAtoUse Then
                    nMPACells += 1
                End If
            Next
        Next
        Me.PercentageClosed = CInt(nMPACells / nTotCells * 100)
    End Sub

    Public Overrides Function ToString() As String

        Return "Total weighted value = " & objFuncTotal.ToString & ", Economic = " & objFuncEconomicValue.ToString & ", Mandated = " & objFuncMandatedValue.ToString _
                & ", Social = " & objFuncSocialValue.ToString & ", Ecological = " & objFuncEcologicalValue.ToString
    End Function

    Public Function CompareTo(ByVal other As cObjectiveResult) As Integer Implements System.IComparable(Of cObjectiveResult).CompareTo

        'Sort in reverse order
        'Biggest first
        If Me.objFuncTotal < other.objFuncTotal Then
            Return 1
        ElseIf Me.objFuncTotal = other.objFuncTotal Then
            Return 0
        ElseIf Me.objFuncTotal > other.objFuncTotal Then
            Return -1
        End If

    End Function
End Class


#End Region


