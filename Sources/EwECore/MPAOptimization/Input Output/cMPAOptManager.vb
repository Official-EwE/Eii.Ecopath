' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.EcoSeed
Imports EwECore.SearchObjectives
Imports Microsoft.Extensions.Logging
Imports Debug = System.Diagnostics.Debug



#Region "Optimization Manager"

Public Class cMPAOptManager
    Inherits cThreadWaitBase 'for thread blocking
    Implements ICoreInterface
    Implements SearchObjectives.ISearchObjective

#Region "Enums"

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' ENumerated type, defines MPA search run states.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Enum eRunStates As Integer
        ''' <summary>Search is not active.</summary>
        Idle
        ''' <summary>Search is initializing.</summary>
        Initializing
        ''' <summary>Search is in progress.</summary>
        Searching
        ''' <summary>Search is completed.</summary>
        Completed
        ''' <summary>A new cell is selected to add to the current MPA configuration</summary>
        NewCellSelected
        ''' <summary>A new best result has been found.</summary>
        NewBestResultFound
    End Enum

#End Region

#Region "Delegates for model to comunicate back to the manager"

    ''' <summary>
    ''' MPA Search has computed the value of a cell
    ''' </summary>
    Public Delegate Sub SearchIterationDelegate()

    ''' <summary>
    ''' MPA Search run state delegate.
    ''' </summary>
    Public Delegate Sub SearchRunStateDelegate(RunState As eRunStates)

    ''' <summary>
    ''' Message sending delegate
    ''' </summary>
    ''' <param name="message"></param>
    Public Delegate Sub SendMessageDelegate(message As EwECore.cMessage)

#End Region

#Region "Private Data"

    Private m_syncObject As System.ComponentModel.ISynchronizeInvoke
    Private m_bConnected As Boolean

    Private m_MPASearch As IMPASearchModel
    Private m_core As cCore
    Private m_searchObjectives As cSearchObjective

    Private m_SeedCellComputedCallback As SearchIterationDelegate
    Private m_SeedRunStateCallback As SearchRunStateDelegate
    Private m_thrSeed As Threading.Thread
    Private m_curRowCol As cMPAOptOutput

    Private m_parameters As cMPAOptParameters

    ''' <summary>
    ''' Original MPA configuration (row x col x mpa)
    ''' </summary>
    Private m_orgMPAConfig()(,) As Integer

    ''' <summary>directory for the output data</summary>
    Private m_dataDir As String = ""
    Private ReadOnly m_logger As ILogger = LoggingContext.CreateLogger(Of cMPAOptManager)()

#End Region

#Region "Construction and Initialization"


    Friend Function Init(ByRef theCore As cCore) As Boolean Implements ISearchObjective.Init

        Try
            Me.m_core = theCore
            Me.m_searchObjectives = Me.m_core.SearchObjective
            Me.m_curRowCol = New cMPAOptOutput(theCore)
            Me.m_parameters = New cMPAOptParameters(theCore)

            Me.setDefaults()

            Me.setActiveSearch(Me.m_core.m_MPAOptData.SearchType, True)

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & " Failed to initialize EcoSeed.")
        End Try

    End Function

    Public Sub Connect(syncObject As System.ComponentModel.ISynchronizeInvoke, SeedCellCallback As SearchIterationDelegate, RunStateCallback As SearchRunStateDelegate)

        Me.m_syncObject = syncObject
        Me.m_SeedCellComputedCallback = SeedCellCallback
        Me.m_SeedRunStateCallback = RunStateCallback

        Debug.Assert(Me.m_syncObject IsNot Nothing, Me.ToString & ".Connect() syncObject is null.")
        Debug.Assert(SeedCellCallback IsNot Nothing, Me.ToString & ".Connect() SeedCellCallback is null.")
        Debug.Assert(Me.m_SeedRunStateCallback IsNot Nothing, Me.ToString & ".Connect() SeedCellCallback is null.")

        If Me.m_syncObject IsNot Nothing And Me.m_SeedCellComputedCallback IsNot Nothing And Me.m_SeedRunStateCallback IsNot Nothing Then
            Me.m_bConnected = True
        Else
            Me.m_bConnected = False
            m_logger.LogError("EcoSeedManager is not connected to an interface.")
        End If

    End Sub

    Public Sub Disconnect()
        Me.m_syncObject = Nothing
        Me.m_SeedCellComputedCallback = Nothing
        Me.m_SeedRunStateCallback = Nothing
        Me.m_bConnected = False
    End Sub

    Private Sub setDefaults()
        'set the default period to run the model to be the same as the interface
        Me.m_core.MPAOptData.EcoSpaceEndYear = Me.m_core.nEcospaceYears
    End Sub

#End Region

#Region "Private methods"

#Region "Changing search models"

    Private Function SearchModelFactory(SearchType As eMPAOptimizationModels) As IMPASearchModel

        Debug.Assert(Me.IsRunning = False, Me.ToString & " Cannot change the search type while a search is running.")
        If Me.IsRunning Then
            Return Me.m_MPASearch
        End If

        Dim search As IMPASearchModel = Nothing

        Select Case SearchType

            Case eMPAOptimizationModels.EcoSeed
                search = New cEcoSeed()

            Case eMPAOptimizationModels.RandomSearch
                search = New cMPARandomSearch()

        End Select

        Return search

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Toggles the active search model to use.
    ''' </summary>
    ''' <param name="newActiveSearch">New search model to use.</param>
    ''' <param name="bForceInit">Flag indicating whether the search should be
    ''' initialized, even when the active search type does not change.</param>
    ''' -----------------------------------------------------------------------
    Private Sub setActiveSearch(newActiveSearch As eMPAOptimizationModels, bForceInit As Boolean)

        'if no search has been created then make sure the factory runs
        If Me.m_MPASearch IsNot Nothing Then

            Debug.Assert(Me.IsRunning = False, Me.ToString & " Can not change the search type while a search is running.")
            If Me.IsRunning Then
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
            Me.m_curRowCol.Init(Me.m_MPASearch.MPAOptData, Me.m_core.m_EcospaceData)

            If Me.m_bConnected Then
                Me.m_syncObject.BeginInvoke(Me.m_SeedCellComputedCallback, Nothing)
            Else
                System.Console.WriteLine("EcoSeedManager not connected to an interface.")
            End If

        Catch ex As Exception

        End Try

    End Sub

    Private Sub OnRunStateChanged(RunState As eRunStates)

        Try

            If RunState = eRunStates.NewBestResultFound Then
                Me.m_curRowCol.Init(Me.m_MPASearch.MPAOptData, Me.m_core.m_EcospaceData)
            End If

            If RunState = eRunStates.Completed Then
                'the run has completed release any waiting threads
                Me.ReleaseWait()
                Me.m_core.m_SearchData.SearchMode = eSearchModes.NotInSearch
            End If

            If Me.m_bConnected Then

                'Invoke will wait for the function to return 
                'this lets the interface gather data before it has changed is response to a new best cell selected
                Me.m_syncObject.Invoke(Me.m_SeedRunStateCallback, New Object() {RunState})

            Else
                System.Console.WriteLine("EcoSeedManager not connected to an interface.")
            End If

        Catch ex As Exception
            m_logger.LogError(ex, "Error in OnRunStateChanged")
        End Try

    End Sub

    Private Sub OnSendMessage(Message As EwECore.cMessage)
        Debug.Assert(False)
    End Sub

#End Region

#End Region

#Region "Public methods"

    Public Function Run() As Boolean

        ' Get real
        If Me.IsRunning Then Return False

        Try
            Me.m_MPASearch.Connect(AddressOf Me.OnSearchIteration, AddressOf Me.OnRunStateChanged, AddressOf Me.OnSendMessage)

            ' Test if no seed cells nor MPA
            If Not Me.m_MPASearch.OKtoRun Then
                Dim msg As New cFeedbackMessage(My.Resources.CoreMessages.MPAOPT_NODATA_RESUME,
                                                eCoreComponentType.MPAOptimization, eMessageType.Any,
                                                eMessageImportance.Warning, eMessageReplyStyle.YES_NO,
                                                eDataTypes.MPAOptParameters, eMessageReply.NO)
                Me.m_core.Messages.SendMessage(msg)
                If msg.Reply = eMessageReply.NO Then Return False
            End If

            Dim strBaseDir As String = Me.m_dataDir
            If String.IsNullOrWhiteSpace(strBaseDir) Then
                strBaseDir = Me.m_core.DefaultOutputPath(eAutosaveTypes.MPAOpt)
            End If

            Dim strHeader As String = ""
            If Me.m_core.SaveWithFileHeader Then Me.m_core.DefaultFileHeader(eAutosaveTypes.MPAOpt)

            ' Configure autosave behaviour
            Me.m_MPASearch.ConfigureAutosave(Me.m_core.Autosave(eAutosaveTypes.MPAOpt), strBaseDir, strHeader)

            Me.SetWait()

            ' Keep a copy of the original MPA configuration
            Dim map(,) As Integer
            Me.m_orgMPAConfig = New Integer(Me.m_core.nMPAs)(,) {}
            For i As Integer = 1 To Me.m_core.nMPAs
                ReDim map(Me.m_core.EcospaceBasemap.InRow + 1, Me.m_core.EcospaceBasemap.InCol + 1)
                Me.m_orgMPAConfig(i) = map
                Array.Copy(Me.m_core.m_EcospaceData.MPA(i), Me.m_orgMPAConfig(i), Me.m_core.m_EcospaceData.MPA(i).Length)
            Next

            Me.m_core.m_SearchData.SearchMode = eSearchModes.SpatialOpt

            Me.m_thrSeed = New Threading.Thread(AddressOf Me.m_MPASearch.Run)
            Me.m_thrSeed.Start()

        Catch ex As Exception
            m_logger.LogError(ex, "Error starting MPA optimization run")
            Me.m_core.m_SearchData.SearchMode = eSearchModes.NotInSearch
            Me.m_core.Messages.SendMessage(New cMessage(String.Format(My.Resources.CoreMessages.MPAOPT_ERROR, ex.Message),
                                                        eMessageType.ErrorEncountered, eCoreComponentType.Ecospace,
                                                        eMessageImportance.Critical))
            Me.ReleaseWait()
            Return False
        End Try

        Return True

    End Function

    Public Sub YearTimeStep(ByRef iYear As Integer, Biomass() As Single)
        Me.m_MPASearch.YearTimeStep(iYear, Biomass)
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns the run state of the current active <see cref="IMPASearchModel">MPA search</see>, if any.
    ''' </summary>
    ''' <returns>The run state of the current active <see cref="IMPASearchModel">MPA search</see>, if any.</returns>
    ''' <remarks>If no search is defined yet, the state <see cref="eRunStates.Idle"/> is returned.</remarks>
    ''' -----------------------------------------------------------------------
    Public Function RunState() As eRunStates

        If (Me.m_MPASearch Is Nothing) Then Return eRunStates.Idle
        Return (Me.m_MPASearch.RunState)

    End Function

    Public Overrides Function StopRun(Optional WaitTimeInMillSec As Integer = -1) As Boolean ' Implements SearchObjectives.ISearchObjective.StopRun
        Dim result As Boolean = True
        Try
            If Me.m_MPASearch IsNot Nothing Then
                Me.m_MPASearch.StopRun()
                result = Me.Wait(WaitTimeInMillSec)
            End If
        Catch ex As Exception
            result = False
        End Try

        Return result

    End Function

    Public Sub clearMPAs()
        Me.m_MPASearch.clearMPAs()
    End Sub

    Public Sub clearSeedCells()
        Me.m_MPASearch.clearSeedCells()
    End Sub

    Public Function setAllCellsToMPA(iMPA As Integer) As Boolean
        Return Me.m_MPASearch.setAllCellsToMPA(iMPA)
    End Function

    Public Function setAllCellsToSeed(iMPA As Integer) As Boolean
        Return Me.m_MPASearch.setAllCellsToSeed(iMPA)
    End Function

    ''' <summary>
    ''' Array of the number of times a cell was selected during the search
    ''' </summary>
    ''' <param name="TopPercentile">Top percentile of search results to include in the map</param>
    ''' <param name="NumberOfResults">Number of results in the top percentile</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CellSelectedMap(TopPercentile As Single, PercentAreaClosedFilter As Integer, ByRef NumberOfResults As Integer) As Integer(,)
        Dim map(,) As Integer
        Dim nResults As Integer
        Dim obj As cObjectiveResult
        Dim lstTemp As List(Of cObjectiveResult)

        Try

            Dim nR As Integer = Me.m_core.m_EcospaceData.InRow
            Dim nC As Integer = Me.m_core.m_EcospaceData.InCol
            ReDim map(nR, nC)

            'get a list of maps where the percentage of cells closed is what the user asked for
            lstTemp = Me.getPercentageList(PercentAreaClosedFilter)
            'sort the maps according the the objective function
            lstTemp.Sort()

            'bound the percentile
            TopPercentile = Math.Min(100, Math.Max(1, TopPercentile))

            'turn the TopPercentile into the number of results
            nResults = CInt(Math.Ceiling(lstTemp.Count * TopPercentile / 100.0!))

            'bound nResults
            If nResults < 1 Then nResults = 1
            If nResults > Me.m_MPASearch.Results.Count Then nResults = Me.m_MPASearch.Results.Count

            'populate the results map (an integer matrix) with the number of times a cell was included in the search
            For ires As Integer = 0 To nResults - 1

                obj = lstTemp.Item(ires)

                If obj.PercentageClosed = PercentAreaClosedFilter Then
                    NumberOfResults += 1
                    'count the number of hits to each cell in the map
                    For Each cell As cMPACell In obj.Cells
                        map(cell.Row, cell.Col) += 1
                    Next cell
                End If
            Next ires

            'return the hit count map
            Return map

        Catch ex As Exception
            m_logger.LogError(ex, "Error generating CellSelectedMap")
            Debug.Assert(False, ex.Message)
            Me.m_core.Messages.SendMessage(New cMessage("MPA Optimization Error: " & ex.Message, eMessageType.ErrorEncountered, eCoreComponentType.SearchObjective, eMessageImportance.Critical))
            Return Nothing

        End Try

    End Function

    Public Function ConvertResultsToMPA(TopPercentile As Single, PercentAreaClosedFilter As Integer) As Boolean

        Dim data As cMPAOptDataStructures = Me.m_core.MPAOptData
        Select Case data.SearchType
            Case eMPAOptimizationModels.RandomSearch
                Dim i As Integer
                Dim map As Integer(,) = Me.CellSelectedMap(TopPercentile, PercentAreaClosedFilter, i)
                DirectCast(Me.m_MPASearch, cMPARandomSearch).ConvertToMPA(map, PercentAreaClosedFilter, data.iMPAtoUse)

                Dim layer As cEcospaceLayerMPA = Me.m_core.EcospaceBasemap.LayerMPA(data.iMPAtoUse)
                layer.Invalidate()
            Case Else
                ' NOP
        End Select
        Return False

    End Function

    ''' <summary>
    ''' Return a list of result maps where the area closed = PercentAreaClosedFilter
    ''' </summary>
    ''' <param name="PercentAreaClosedFilter">Area of the map that is Closed (has an MPA set)</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function getPercentageList(PercentAreaClosedFilter As Integer) As List(Of cObjectiveResult)
        Dim lstOut As New List(Of cObjectiveResult)

        For iResult As Integer = 0 To Me.Results.Count - 1
            If Me.Results(iResult).PercentageClosed = PercentAreaClosedFilter Then
                lstOut.Add(Me.Results(iResult))
            End If
        Next

        Return lstOut

    End Function

#End Region

#Region " Public properties "

    ''' <summary>
    ''' Output object for the current Ecoseed interation 
    ''' </summary>
    Public ReadOnly Property CurrentRowColResults() As cMPAOptOutput
        Get
            Return Me.m_curRowCol
        End Get
    End Property

    Public ReadOnly Property MPAOptimizationParameters() As cMPAOptParameters
        Get
            Return Me.m_parameters
        End Get
    End Property

    Public ReadOnly Property Results() As List(Of cObjectiveResult)
        Get
            Return Me.m_MPASearch.Results
        End Get
    End Property

    Public ReadOnly Property OrgMPA() As Integer()(,)
        Get
            Return Me.m_orgMPAConfig
        End Get
    End Property

    Public ReadOnly Property nIterationsCompleted() As Integer
        Get
            Return Me.m_MPASearch.nInterationsCompleted
        End Get
    End Property

#End Region ' Public properties

#Region "ICoreInterface"

    Public ReadOnly Property DataType() As eDataTypes Implements ICoreInterface.DataType
        Get
            Return eDataTypes.MPAOptManager
        End Get
    End Property

    Public ReadOnly Property CoreComponent() As eCoreComponentType Implements ICoreInterface.CoreComponent
        Get
            Return eCoreComponentType.Ecospace
        End Get
    End Property

    Public Property DBID() As Integer Implements ICoreInterface.DBID
        Get
            Return cCore.NULL_VALUE
        End Get
        Set(value As Integer)
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
        Set(value As Integer)
            Debug.Assert(False, Me.ToString & ".Index no implementation.")
        End Set
    End Property

    Public Property Name() As String Implements ICoreInterface.Name
        Get
            Return Me.ToString
        End Get
        Set(value As String)
            Debug.Assert(False, Me.ToString & ".Name no implementation.")
        End Set
    End Property

#End Region

#Region "ISearchObjective implementation"

    Public ReadOnly Property FleetObjectives(iFleet As Integer) As cSearchObjectiveFleetInput Implements ISearchObjective.FleetObjectives
        Get
            Return Me.m_searchObjectives.FleetObjectives(iFleet)
        End Get
    End Property

    Public ReadOnly Property GroupObjectives(iGroup As Integer) As cSearchObjectiveGroupInput Implements ISearchObjective.GroupObjectives
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
            Me.m_parameters.UseCellWeight = coreData.bUseCellWeight
            Me.m_parameters.UseRegions = coreData.bUseRegions

            Me.m_parameters.StartYear = coreData.EcoSpaceStartYear
            Me.m_parameters.EndYear = coreData.EcoSpaceEndYear

            Me.m_parameters.AllowValidation = True

            Me.m_parameters.ResetStatusFlags()

        Catch ex As Exception
            m_logger.LogError(ex, "Error loading MPA optimization parameters")
            Debug.Assert(False, ex.Message)
            Me.m_parameters.AllowValidation = True
            Return False
        End Try

        Return True

    End Function

    Public Sub Clear() Implements ISearchObjective.Clear
        ' NOP
        Try
            If Me.m_parameters IsNot Nothing Then Me.m_parameters.Clear()
            If Me.m_curRowCol IsNot Nothing Then Me.m_curRowCol.Clear()
        Catch ex As Exception

        End Try

    End Sub

    Public Function Update(DataType As eDataTypes) As Boolean Implements ISearchObjective.Update

        Try

            Dim coreData As cMPAOptDataStructures = Me.m_core.MPAOptData

            coreData.stepSize = Me.m_parameters.StepSize
            coreData.BoundaryWeight = Me.m_parameters.BoundaryWeight
            coreData.MaxArea = Me.m_parameters.MaxArea
            coreData.MinArea = Me.m_parameters.MinArea
            coreData.nIterations = Me.m_parameters.nIterations
            coreData.iMPAtoUse = Me.m_parameters.iMPAToUse
            coreData.bUseCellWeight = Me.m_parameters.UseCellWeight
            coreData.bUseRegions = Me.m_parameters.UseRegions

            coreData.EcoSpaceStartYear = Me.m_parameters.StartYear
            coreData.EcoSpaceEndYear = Me.m_parameters.EndYear

            Me.setActiveSearch(Me.m_parameters.SearchType, False)

        Catch ex As Exception
            m_logger.LogError(ex, "Error updating MPA optimization parameters")
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
    ReadOnly Property OKtoRun() As Boolean


    Function Init(ByRef EcoSpaceModel As cEcoSpace, ByRef MPAOptData As cMPAOptDataStructures) As Boolean

    Sub Connect(OnSearchInteration As cMPAOptManager.SearchIterationDelegate,
                OnRunStateChanged As cMPAOptManager.SearchRunStateDelegate,
                OnSendMessage As cMPAOptManager.SendMessageDelegate)

    Sub StopRun()
    Sub clearMPAs()
    Sub clearSeedCells()
    Function setAllCellsToMPA(iMPA As Integer) As Boolean
    Function setAllCellsToSeed(iMPA As Integer) As Boolean

    Property MPAOptData() As cMPAOptDataStructures
    ReadOnly Property isRunning() As Boolean
    ReadOnly Property EcospaceStartTime() As Single
    ReadOnly Property Results() As List(Of cObjectiveResult)
    ReadOnly Property nInterationsCompleted() As Integer

    Sub YearTimeStep(ByRef iYear As Integer, Biomass() As Single)

    ''' <summary>
    ''' Configure the search for auto-saving.
    ''' </summary>
    ''' <param name="bAutosave">Turn auto-saving on or off.</param>
    ''' <param name="strOutputPath">Path to auto-save to.</param>
    ''' <param name="strHeader">Header information to report when auto-saving.</param>
    Sub ConfigureAutosave(bAutosave As Boolean, strOutputPath As String, strHeader As String)

    Function RunState() As cMPAOptManager.eRunStates

End Interface

#End Region

#Region "cObjectiveResult class definition"

Public Class cObjectiveResult
    Implements IComparable(Of cObjectiveResult)

    Public Sub New(MPAData As cMPAOptDataStructures, SpaceData As cEcospaceDataStructures)

        Me.Row = MPAData.bestrow
        Me.Col = MPAData.bestcol

        Me.objFuncEconomicValue = MPAData.objFuncEconomicValue
        Me.objFuncMandatedValue = MPAData.objFuncMandatedValue
        Me.objFuncSocialValue = MPAData.objFuncSocialValue
        Me.objFuncEcologicalValue = MPAData.objFuncEcologicalValue
        Me.objBiomassDiversity = MPAData.objFuncBiodiversity

        Me.objFuncAreaBorder = MPAData.objFuncAreaBorder

        Me.objFuncTotal = MPAData.objFuncTotal

        Me.SearchType = MPAData.SearchType

        'copy the list of cells into a new list 
        Me.Cells = MPAData.Cells.ToArray()

        Me.calcPercentageClosed(MPAData, SpaceData)

    End Sub

    'Public Sub Init(ByRef MPAData As cMPAOptDataStructures, ByRef SpaceData As cEcospaceDataStructures)


    '    Try
    '        objFuncEconomicValue = MPAData.objFuncEconomicValue
    '        objFuncMandatedValue = MPAData.objFuncMandatedValue
    '        objFuncSocialValue = MPAData.objFuncSocialValue
    '        objFuncEcologicalValue = MPAData.objFuncEcologicalValue
    '        objFuncAreaBorder = MPAData.objFuncAreaBorder
    '        objBiomassDiversity = MPAData.objFuncBiodiversity
    '        objFuncTotal = MPAData.objFuncTotal

    '        Select Case MPAData.SearchType

    '            Case eMPAOptimizationModels.EcoSeed

    '                Debug.Assert(SpaceData IsNot Nothing, Me.ToString & ".Init() SpaceData must be passed in!")
    '                Dim tmp As New List(Of cMPACell)
    '                For ir As Integer = 1 To SpaceData.InRow
    '                    For ic As Integer = 1 To SpaceData.InCol
    '                        'If SpaceData.MPA(ir, ic) <> 0 Then
    '                        '    Cells.Add(New cMPACell(ir, ic, SpaceData.MPA(ir, ic)))
    '                        'End If
    '                        For impa As Integer = 1 To SpaceData.MPAno
    '                            If SpaceData.MPA(impa)(ir, ic) > 0 Then
    '                                tmp.Add(New cMPACell(ir, ic, impa))
    '                            End If
    '                        Next
    '                    Next
    '                Next
    '                Me.Cells = tmp.ToArray()

    '            Case eMPAOptimizationModels.RandomSearch
    '                Me.Cells = MPAData.Cells.ToArray()

    '        End Select

    '        calcPercentageClosed(MPAData, SpaceData)

    '    Catch ex As Exception
    '        m_logger.LogError(ex, "Error initializing cObjectiveResult")
    '        Throw New ApplicationException(Me.ToString & ".Init() Error: " & ex.Message, ex)
    '    End Try

    'End Sub

    Public ReadOnly Property Row As Integer
    Public ReadOnly Property Col As Integer
    Public ReadOnly Property objFuncEconomicValue As Single
    Public ReadOnly Property objFuncMandatedValue As Single
    Public ReadOnly Property objFuncSocialValue As Single
    Public ReadOnly Property objFuncEcologicalValue As Single
    Public ReadOnly Property objFuncAreaBorder As Single
    Public ReadOnly Property objBiomassDiversity As Single

    ''' <summary>Includes weights</summary>
    Public ReadOnly Property objFuncTotal As Single = 0

    Public ReadOnly Property SearchType As eMPAOptimizationModels
    Public ReadOnly Property Cells As cMPACell()
    Public Property PercentageClosed As Integer


    Private Sub calcPercentageClosed(ByRef MPAData As cMPAOptDataStructures, ByRef SpaceData As cEcospaceDataStructures)
        'what percentage of the area is closed
        Dim nTotCells As Integer = SpaceData.nWaterCells
        Dim nMPACells As Integer
        For ir As Integer = 1 To SpaceData.InRow
            For ic As Integer = 1 To SpaceData.InCol
                'If SpaceData.MPA(ir, ic) = MPAData.iMPAtoUse Then
                If SpaceData.MPA(MPAData.iMPAtoUse)(ir, ic) > 0 Then
                    nMPACells += 1
                End If
            Next
        Next
        Me.PercentageClosed = CInt(nMPACells / nTotCells * 100)
    End Sub

    Public Overrides Function ToString() As String
        Return "Total weighted value = " & Me.objFuncTotal.ToString & ", Economic = " & Me.objFuncEconomicValue.ToString & ", Mandated = " & Me.objFuncMandatedValue.ToString _
                & ", Social = " & Me.objFuncSocialValue.ToString & ", Ecological = " & Me.objFuncEcologicalValue.ToString
    End Function

    Public Function CompareTo(other As cObjectiveResult) As Integer Implements System.IComparable(Of cObjectiveResult).CompareTo

        'Sort in reverse order, largest first
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


