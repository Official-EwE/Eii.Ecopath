Option Strict On

Imports EwECore.ValueWrapper
Imports System.Threading
Imports EwECore.FitToTimeSeries
Imports EwEUtils.Core
Imports EwECore.SearchObjectives

Public Class cF2TSManager
    Inherits cCoreInputOutputBase
    Implements SearchObjectives.ISearchObjective

    'ToDo_jb Firstyear and LastYear need to be set for the time series data

#Region " Construction, Initialization and Destruction"

    Private m_PPIs As New Dictionary(Of String, cPredPreyInteraction)
    Private m_EPData As cEcopathDataStructures = Nothing
    Private m_ESData As cEcosimDatastructures = Nothing
    Private m_model As cF2TSModel = Nothing

    ' Received delegate instances to report progress to
    Private m_runstartedHandler As RunStartedDelegate = Nothing
    Private m_runstepHandler As RunStepDelegate = Nothing
    Private m_runstoppedHandler As RunStoppedDelegate = Nothing
    Private m_runModelHandler As RunModelDelegate = Nothing


    Private m_runSilent As Boolean

    'Messaging 
    'list of messages sent from the model
    Private m_lstMessages As List(Of cMessage)
    'definition of delegate used to send messages to the core
    'this is for marshalling messages across thread boundaries
    Private Delegate Sub SendMessagesToCoreDelegate()

    'Multi threading
    ''' <summary>
    ''' m_SignalState is use by an calling routine to block its thread until the model has completed
    ''' </summary>
    ''' <remarks>See Wait()</remarks>
    Private m_SignalState As ManualResetEvent '(True)

    ''' <summary>
    ''' m_semaphore is use to block the model from being called while it is running
    ''' </summary>
    Private m_semaphore As Semaphore

    Private m_searchObjective As cSearchObjective


    Friend Sub New(ByRef theCore As cCore)
        MyBase.New(theCore)


        Me.m_semaphore = New Semaphore(0, 1)
        Me.m_SignalState = New ManualResetEvent(True)

        Me.m_lstMessages = New List(Of cMessage)

        Me.m_semaphore.Release()

        Dim val As cValue = Nothing
        Dim meta As cVariableMetaData = Nothing

        Me.AllowValidation = False
        Me.m_coreComponent = eCoreComponentType.EcoSimFitToTimeSeries
        Me.m_dataType = eDataTypes.FitToTimeSeries

        Me.m_core = theCore
        Me.m_EPData = theCore.m_EcoPathData
        Me.m_ESData = theCore.m_EcoSimData

        m_searchObjective = theCore.SearchObjective

        'default OK status used for setVariable
        m_ValidationStatus = New cVariableStatus(Me, eStatusFlags.OK, "", eVarNameFlags.NotSet)

        'boolean
        ' F2TSVulnerabilitySearch
        meta = New cVariableMetaData()
        val = New cValue(New Boolean, eVarNameFlags.F2TSVulnerabilitySearch, eStatusFlags.Null, eValueTypes.Bool, meta, m_core.m_validators.getValidator(eVarNameFlags.F2TSVulnerabilitySearch))
        m_values.Add(val.varName, val)

        'boolean
        ' AnomalySearch
        meta = New cVariableMetaData()
        val = New cValue(New Boolean, eVarNameFlags.F2TSAnomalySearch, eStatusFlags.Null, eValueTypes.Bool, meta, m_core.m_validators.getValidator(eVarNameFlags.F2TSVulnerabilitySearch))
        m_values.Add(val.varName, val)


        ' F2TSCatchAnomaly
        meta = New cVariableMetaData()
        val = New cValue(New Boolean, eVarNameFlags.F2TSCatchAnomaly, eStatusFlags.Null, eValueTypes.Bool, meta, m_core.m_validators.getValidator(eVarNameFlags.F2TSCatchAnomaly))
        m_values.Add(val.varName, val)

        'singles
        ' F2TSFirstYear
        meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
        val = New cValue(New Integer, eVarNameFlags.F2TSFirstYear, eStatusFlags.Null, eValueTypes.Int, meta, m_core.m_validators.getValidator(eVarNameFlags.F2TSFirstYear))
        m_values.Add(val.varName, val)

        ' F2TSLastYear
        meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
        val = New cValue(New Integer, eVarNameFlags.F2TSLastYear, eStatusFlags.Null, eValueTypes.Int, meta, m_core.m_validators.getValidator(eVarNameFlags.F2TSLastYear))
        m_values.Add(val.varName, val)

        ' F2TSVulnerabilityVariance
        meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
        val = New cValue(New Single, eVarNameFlags.F2TSVulnerabilityVariance, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.F2TSVulnerabilityVariance))
        m_values.Add(val.varName, val)

        ' F2TSPPVariance
        meta = New cVariableMetaData(0, Single.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
        val = New cValue(New Single, eVarNameFlags.F2TSPPVariance, eStatusFlags.Null, eValueTypes.Sng, meta, m_core.m_validators.getValidator(eVarNameFlags.F2TSPPVariance))
        m_values.Add(val.varName, val)

        'integers
        ' F2TSCatchAnomalySearchShape
        meta = New cVariableMetaData(0, Integer.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
        val = New cValue(New Integer, eVarNameFlags.F2TSCatchAnomalySearchShapeNumber, eStatusFlags.Null, eValueTypes.Int, meta, m_core.m_validators.getValidator(eVarNameFlags.F2TSCatchAnomalySearchShapeNumber))
        m_values.Add(val.varName, val)

        ' F2TSNumSplinePoints
        meta = New cVariableMetaData(0, Integer.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
        val = New cValue(New Integer, eVarNameFlags.F2TSNumSplinePoints, eStatusFlags.Null, eValueTypes.Int, meta, m_core.m_validators.getValidator(eVarNameFlags.F2TSNumSplinePoints))
        m_values.Add(val.varName, val)

        'Singlearray
        meta = New cVariableMetaData(0, 1, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
        val = New cValueArray(eValueTypes.SingleArray, eVarNameFlags.F2TSAppliedWeights, eStatusFlags.Null, eCoreCounterTypes.nTimeSeriesApplied, AddressOf m_core.GetCoreCounter, meta, m_core.m_validators.getValidator(eVarNameFlags.NotSet))
        m_values.Add(val.varName, val)

 
        ' AIC N Data points
        meta = New cVariableMetaData(0, Integer.MaxValue, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo))
        val = New cValue(New Integer, eVarNameFlags.F2TSNAICData, eStatusFlags.Null, eValueTypes.Int, meta, m_core.m_validators.getValidator(eVarNameFlags.F2TSNAICData))
        m_values.Add(val.varName, val)


        Me.AllowValidation = True

        ' Create and configure model
        Me.m_model = New cF2TSModel(Me.m_core, Me.m_core.m_EcoSim, Me.m_EPData, Me.m_ESData)
        Me.m_model.Init(AddressOf RunStartedCallback, AddressOf RunStepCallback, AddressOf RunStoppedCallback, _
                        AddressOf AddMessageCallback, AddressOf RunModelCallBack, AddressOf Me.SendMessageCallback)

    End Sub

    Friend Function Init(ByRef theCore As cCore) As Boolean Implements ISearchObjective.Init

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Loads local values in the manager from the underlying data structures.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Function Load() As Boolean Implements ISearchObjective.Load

        If (Me.m_EPData.ActiveEcosimScenario <= 0) Then Return False

        Dim f2tsDS As cF2TSDataStructures = Me.m_core.m_FitToTimeSeriesData
        Dim tsDS As cTimeSeriesDataStructures = Me.m_core.m_TSData

        Me.setDefaultAICNData()

        Me.AllowValidation = False
        Me.VulnerabilitySearch = f2tsDS.bVulnerabilitySearch
        Me.CatchAnomaly = f2tsDS.bCatchAnomaly
        Me.AnomalySearchShapeNumber = f2tsDS.iCatchAnomalySearchShapeNumber
        Me.NumSplinePoints = f2tsDS.nNumSplinePoints
        Me.FirstYear = f2tsDS.FirstYear
        Me.LastYear = f2tsDS.LastYear
        Me.PPVariance = f2tsDS.PPVariance
        Me.VulnerabilityVariance = f2tsDS.VulnerabilityVariance

        Me.NAICDataPoints = f2tsDS.nAICData

        ' Use DBID from current Ecosim scenario
        Me.DBID = Me.m_EPData.EcosimScenarioDBID(Me.m_EPData.ActiveEcosimScenario)

        Me.AllowValidation = True

    End Function

    Public Overrides Sub Clear() Implements ISearchObjective.Clear
        MyBase.Clear()

        Try
            Me.m_SyncObject = Nothing
            Me.m_runstartedHandler = Nothing
            Me.m_runstepHandler = Nothing
            Me.m_runstoppedHandler = Nothing
            Me.m_runModelHandler = Nothing

            'kill the thread if it is still alive
            If Me.m_thrdRun IsNot Nothing Then
                Me.m_thrdRun.Abort()
                Me.m_thrdRun = Nothing
            End If

        Catch ex As Exception
            cLog.Write(ex)
        End Try

    End Sub

    Public Sub Connect(ByVal syncObject As System.ComponentModel.ISynchronizeInvoke, _
            ByVal runStartedCallback As RunStartedDelegate, ByVal runStepCallback As RunStepDelegate, ByVal runStoppedCallback As RunStoppedDelegate, ByVal RunModelCallBack As RunModelDelegate)

        Debug.Assert(m_runstartedHandler = Nothing)

        Try
            Me.m_SyncObject = syncObject
            Me.m_runstartedHandler = runStartedCallback
            Me.m_runstepHandler = runStepCallback
            Me.m_runstoppedHandler = runStoppedCallback
            Me.m_runModelHandler = RunModelCallBack
        Catch ex As Exception
            cLog.Write(ex)
            Throw New ApplicationException(Me.ToString & ".Connect() Error.", ex)
        End Try

    End Sub

    Public Sub Disconnect(ByVal runStartedCallback As RunStartedDelegate, ByVal runStepCallback As RunStepDelegate, ByVal runStoppedCallback As RunStoppedDelegate, ByVal RunModelCallBack As RunModelDelegate)

        Try
            Me.m_SyncObject = Nothing
            Me.m_runstartedHandler = Nothing
            Me.m_runstepHandler = Nothing
            Me.m_runstoppedHandler = Nothing
            Me.m_runModelHandler = Nothing

            'kill the thread if it is still alive
            If Me.m_thrdRun IsNot Nothing Then
                Me.m_thrdRun.Abort()
                Me.m_thrdRun = Nothing
            End If

        Catch ex As Exception
            cLog.Write(ex)
            Throw New ApplicationException(Me.ToString & ".Disconnect() Error.", ex)
        End Try


    End Sub


    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Stores the values in the manager back to the underlying data structures.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Function Update(ByVal DataType As EwEUtils.Core.eDataTypes) As Boolean Implements SearchObjectives.ISearchObjective.Update

        Dim f2tsDS As cF2TSDataStructures = Me.m_core.m_FitToTimeSeriesData
        Dim tsDS As cTimeSeriesDataStructures = Me.m_core.m_TSData

        f2tsDS.bVulnerabilitySearch = Me.VulnerabilitySearch
        f2tsDS.bCatchAnomaly = Me.CatchAnomaly
        f2tsDS.bAnomalySearch = Me.AnomalySearch

        f2tsDS.iCatchAnomalySearchShapeNumber = Me.AnomalySearchShapeNumber
        f2tsDS.nNumSplinePoints = Me.NumSplinePoints
        f2tsDS.FirstYear = Me.FirstYear
        f2tsDS.LastYear = Me.LastYear
        f2tsDS.PPVariance = Me.PPVariance
        f2tsDS.VulnerabilityVariance = Me.VulnerabilityVariance

        f2tsDS.nAICData = Me.NAICDataPoints

    End Function

    ''' <summary>
    ''' Compute the default value for nAICData number of AIC data points
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub setDefaultAICNData()

        Dim f2tsDS As cF2TSDataStructures = Me.m_core.m_FitToTimeSeriesData
        Dim tsDS As cTimeSeriesDataStructures = Me.m_core.m_TSData

        f2tsDS.nAICData = tsDS.NdatType * 3

    End Sub


#End Region

#Region " Generic variable access "

    ''' <summary>
    ''' States whether there is a link between a given predator and prey.
    ''' </summary>
    ''' <param name="iPred"></param>
    ''' <param name="iPrey"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' This code encapsulates working with ecosim inlinks, ilink and jlink variables.
    ''' </remarks>
    Public Function isPredPrey(ByVal iPred As Integer, ByVal iPrey As Integer) As Boolean
        For i As Integer = 1 To Me.m_ESData.Narena
            If Me.m_ESData.Iarena(i) = iPrey And Me.m_ESData.Jarena(i) = iPred Then Return True
        Next
        Return False
    End Function

    Public ReadOnly Property TotalTime() As Integer
        Get
            Return Me.m_ESData.NumYears ' ?
        End Get
    End Property

    Public Property VulnerabilitySearch() As Boolean
        Get
            Return CBool(Me.GetVariable(eVarNameFlags.F2TSVulnerabilitySearch))
        End Get
        Set(ByVal value As Boolean)
            Me.SetVariable(eVarNameFlags.F2TSVulnerabilitySearch, value)
        End Set
    End Property

    Public Property CatchAnomaly() As Boolean
        Get
            Return CBool(Me.GetVariable(eVarNameFlags.F2TSCatchAnomaly))
        End Get
        Set(ByVal value As Boolean)
            Me.SetVariable(eVarNameFlags.F2TSCatchAnomaly, value)
        End Set
    End Property

    Public Property AnomalySearch() As Boolean
        Get
            Return CBool(Me.GetVariable(eVarNameFlags.F2TSAnomalySearch))
        End Get
        Set(ByVal value As Boolean)
            Me.SetVariable(eVarNameFlags.F2TSAnomalySearch, value)
        End Set
    End Property

    Public Property AnomalySearchShapeNumber() As Integer
        Get
            Return CInt(GetVariable(eVarNameFlags.F2TSCatchAnomalySearchShapeNumber))
        End Get
        Set(ByVal value As Integer)
            Me.SetVariable(eVarNameFlags.F2TSCatchAnomalySearchShapeNumber, value)
        End Set
    End Property

    Public Property FirstYear() As Integer
        Get
            Return CInt(Me.GetVariable(eVarNameFlags.F2TSFirstYear))
        End Get
        Set(ByVal value As Integer)
            Me.SetVariable(eVarNameFlags.F2TSFirstYear, value)
        End Set
    End Property

    Public Property LastYear() As Integer
        Get
            Return CInt(Me.GetVariable(eVarNameFlags.F2TSLastYear))
        End Get
        Set(ByVal value As Integer)
            Me.SetVariable(eVarNameFlags.F2TSLastYear, value)
        End Set
    End Property

    Public Property VulnerabilityVariance() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.F2TSVulnerabilityVariance))
        End Get
        Set(ByVal value As Single)
            Me.SetVariable(eVarNameFlags.F2TSVulnerabilityVariance, value)
        End Set
    End Property

    Public Property PPVariance() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.F2TSPPVariance))
        End Get
        Set(ByVal value As Single)
            Me.SetVariable(eVarNameFlags.F2TSPPVariance, value)
        End Set
    End Property

    Public Property NumSplinePoints() As Integer
        Get
            Return CInt(Me.GetVariable(eVarNameFlags.F2TSNumSplinePoints))
        End Get
        Set(ByVal value As Integer)
            Me.SetVariable(eVarNameFlags.F2TSNumSplinePoints, value)
        End Set
    End Property

   
    ''' <summary>
    ''' Number of data points for the AIC indicator
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property NAICDataPoints() As Integer
        Get
            Return CInt(Me.GetVariable(eVarNameFlags.F2TSNAICData))
        End Get
        Set(ByVal value As Integer)
            Me.SetVariable(eVarNameFlags.F2TSNAICData, value)
        End Set
    End Property


    Public ReadOnly Property nTimeSeriesYears() As Integer
        Get
            Return Me.m_core.m_TSData.nMaxYears
        End Get
    End Property

    ''' <summary>
    ''' Vulnerability block to use for pred,prey
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property VulnerabilityBlocks() As Integer(,)
        Get
            ' Translate m_model.Vblockcode into pred/prey array
            Dim a2iVulnerabilityBlocks(Me.m_EPData.NumGroups, Me.m_EPData.NumGroups) As Integer
            For iLink As Integer = 1 To Me.m_ESData.Narena
                a2iVulnerabilityBlocks(Me.m_ESData.Jarena(iLink), Me.m_ESData.Iarena(iLink)) = Me.m_model.VblockCode(iLink)
            Next
            Return a2iVulnerabilityBlocks
        End Get
        Set(ByVal value(,) As Integer)
            ' Copy pred/prey array into inlinks array
            Dim aiVblockCode(Me.m_ESData.Narena) As Integer
            Dim iLink As Integer = 1

            For j As Integer = 1 To Me.m_EPData.NumGroups      'all living groups; consumers
                For i As Integer = 0 To Me.m_EPData.NumGroups 'prey
                    If (Me.isPredPrey(i, j) = True) Then
                        aiVblockCode(iLink) = value(i, j)
                        iLink += 1
                    End If
                Next i
            Next j
            Me.m_model.VblockCode = aiVblockCode
        End Set
    End Property

    Public Property nBlockCodes() As Integer
        Get
            Return m_model.nBlockCodes
        End Get
        Set(ByVal value As Integer)
            m_model.nBlockCodes = value
        End Set
    End Property



#End Region ' Generic variable access

#Region " Public access "

#Region " Model state access "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' States whether F2TS models can run.
    ''' </summary>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Public Function CanRun() As Boolean
        ' Check all pre-run states

        ' jb replaced the is running check with a semaphore
        Dim bCanRun As Boolean '= Not Me.IsRunning

        Try

            bCanRun = Me.m_core.StateMonitor.HasEcosimLoaded

            If (Me.AnomalySearch) Then bCanRun = bCanRun And (Me.AnomalySearchShapeNumber > 0)
            'isRefDataLoaded() will send a message if there is not data loaded
            bCanRun = bCanRun And isRefDataLoaded()
            bCanRun = bCanRun And Me.m_SyncObject IsNot Nothing

        Catch ex As Exception
            System.Console.WriteLine(Me.ToString & " Error not properly initialized.")
            bCanRun = False
        End Try

        If Not bCanRun Then
            m_core.Messages.SendMessage(New cMessage("Fit to Time Series not all the parameters have been set correctly.", eMessageType.ErrorEncountered, eCoreComponentType.EcoSimFitToTimeSeries, eMessageImportance.Warning))
        End If

        Return bCanRun

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' States whether a F2TS model is running.
    ''' </summary>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Public Function IsRunning() As Boolean

        If (Me.m_thrdRun Is Nothing) Then
            'the thread is not running!!!
            Return False
        End If
        'this is for robustness
        Return (Me.m_thrdRun.ThreadState = ThreadState.Running)

    End Function

    ''' <summary>
    ''' Block the calling thread until the model has finished running
    ''' </summary>
    ''' <remarks>This can be used by an interface to call the model then wait for results before continuing processing.</remarks>
    Public Sub Wait()
        System.Console.WriteLine("Fit to time series: Waiting.")

        'block until m_SignalState changes
        Me.m_SignalState.WaitOne()

        System.Console.WriteLine("Fit to time series: Finished waiting.")

    End Sub

    Private Function isRefDataLoaded() As Boolean

        If m_core.m_TSData.NdatType > 0 Then
            Return True
        End If

        'jb this should never happen but if it does we better tell the interface why this could not be run
        m_core.Messages.SendMessage(New cMessage("Fit to Time Series no time series data loaded for fitting.", eMessageType.ErrorEncountered, _
                                    eCoreComponentType.EcoSimFitToTimeSeries, eMessageImportance.Warning))

        Return False
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Stops a running F2TS model
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Function StopRun() As Boolean

        Try
            'the model will keep running until it hits the StopRun flag
            'at which point it will call the RunStoppedDelegate(eRunType)
            'this lets it die gracefully
            m_model.StopRun = True
            Return True
        Catch ex As Exception

        End Try

        'Try
        '    Me.m_thrdRun.Abort()
        'Catch e As Threading.ThreadAbortException
        '    Return True
        'Catch e As Exception
        '    Return False
        'End Try
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns the <see cref="eRunType">type of run</see> 
    ''' the current F2TS model is performing.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Function GetRunType() As eRunType
        Return Me.m_model.RunState
    End Function

#End Region ' Model state access

#Region " SensitivitySS2VByPredPrey "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Public Function RunSensitivitySS2VByPredPrey() As Boolean

        ' Safety check
        If Not CanRun() Then Return False

        Try

            ' Sanity check
            Debug.Assert(Me.m_thrdRun Is Nothing)

            'block if the semaphore is already set
            Me.m_semaphore.WaitOne()
            Me.m_SignalState.Reset()

            ' Make sure model can access manager variables from the shared data structures
            Me.Update(Me.m_dataType)

            ' Launch requested analysis model 
            If Me.m_SyncObject IsNot Nothing Then
                m_thrdRun = New Thread(AddressOf Me.m_model.RunSensitivitySS2VByPredPrey)
                m_thrdRun.Start()
            Else
                Me.m_model.RunSensitivitySS2VByPredPrey()
            End If

            Return True

        Catch ex As Exception

            Me.m_SignalState.Set()
            Me.m_semaphore.Release()
            cLog.Write(ex)
            Me.SendMessageCallback(New cMessage("Fit to timeseries Error: Sensitvity to predator prey search. " & ex.Message, eMessageType.ErrorEncountered, _
                                    eCoreComponentType.EcoSimFitToTimeSeries, eMessageImportance.Critical, Me.m_dataType))

        End Try

    End Function

#End Region ' SensitivitySS2VByPredPrey

#Region " SensitivitySS2VByPredator "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Public Function RunSensitivitySS2VByPredator() As Boolean

        ' Safety check
        If Not CanRun() Then Return False

        Try
            'block if the semaphore is already set
            Me.m_semaphore.WaitOne()
            Me.m_SignalState.Reset()
            ' Sanity check
            Debug.Assert(Me.m_thrdRun Is Nothing)

            ' Make sure model can access manager variables from the shared data structures
            Me.Update(Me.m_dataType)

            ' Launch requested analysis model 
            If Me.m_SyncObject IsNot Nothing Then
                m_thrdRun = New Thread(AddressOf Me.m_model.RunSensitivitySS2VByPredator)
                m_thrdRun.Start()
            Else
                Me.m_model.RunSensitivitySS2VByPredator()
            End If

            Return True

        Catch ex As Exception

            Me.m_SignalState.Set()
            cLog.Write(ex)
            Me.SendMessageCallback(New cMessage("Fit to timeseries Error: Sensitvity to predator search. " & ex.Message, eMessageType.ErrorEncountered, _
                                    eCoreComponentType.EcoSimFitToTimeSeries, eMessageImportance.Critical, Me.m_dataType))
        End Try

    End Function

#End Region ' SensitivitySS2VByPredPrey

#Region " Search "

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="RunSilent">Optional parameter to run without sending any messages or requesting any feedback</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function RunSearch(Optional ByVal RunSilent As Boolean = False) As Boolean

        Dim iPPYear1 As Integer = 0
        Dim iPPYear2 As Integer = 0
        Dim bret As Boolean

        ' Safety check
        If Not CanRun() Then Return False

        Me.m_runSilent = RunSilent

        Try

            'block if the semaphore is already set
            Me.m_semaphore.WaitOne()
            Me.m_SignalState.Reset()

            ' Make sure model can access manager variables from the shared data structures
            Me.Update(Me.m_dataType)

            ' Launch requested analysis model 
            If Me.m_SyncObject IsNot Nothing Then
                m_thrdRun = New Thread(AddressOf Me.m_model.RunSearch)
                m_thrdRun.Start()
                bret = True
            Else
                bret = False
            End If

            Return bret

        Catch ex As Exception
            Me.m_SignalState.Set()
            Me.m_runSilent = False
            cLog.Write(ex)
            Me.SendMessageCallback(New cMessage("Fit to timeseries Error: " & ex.Message, eMessageType.ErrorEncountered, _
                         eCoreComponentType.EcoSimFitToTimeSeries, eMessageImportance.Critical, Me.m_dataType))

            Return False

        End Try


    End Function

#End Region ' Search

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns the most recent received results.
    ''' </summary>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Public Function Results() As cF2TSResults
        Return Me.m_model.Results
    End Function


    Public Sub setNBlocksFromSensitivity(ByVal nBlocks As Integer)
        Me.m_model.setNBlocksFromSensitivity(nBlocks)
    End Sub

#End Region ' Public access

#Region " Internal model callback handlers "

    Private m_thrdRun As Threading.Thread = Nothing
    Private m_SyncObject As System.ComponentModel.ISynchronizeInvoke
    Private m_messages As New List(Of cMessage)
    Private m_results As cF2TSResults = Nothing

    ''' <summary>
    ''' Delegate handler called by the model when the run has started
    ''' </summary>
    ''' <param name="runType"></param>
    ''' <remarks>This handler is passed to the model during the contruction of the manager via cF2TSModel.Init()</remarks>
    Private Sub RunStartedCallback(ByVal runType As eRunType, ByVal nSteps As Integer)

        Dim parms(1) As Object
        parms(0) = runType
        parms(1) = nSteps

        ' Clear previous results
        Me.m_results = Nothing

        System.Console.WriteLine("F2TS: Run Started. " & runType.ToString)

        Try
            ' Call delegate
            m_SyncObject.BeginInvoke(Me.m_runstartedHandler, parms)
        Catch ex As Exception
            cLog.Write(ex)
            Debug.Assert(False, ex.StackTrace)
        End Try

    End Sub

    ''' <summary>
    ''' Delegate handler called by the model when the run has completed a step
    ''' </summary>
    ''' <remarks>This handler is passed to the model during the contruction of the manager via cF2TSModel.Init()</remarks>
    Private Sub RunStepCallback()

        Try

            'keep a reference
            m_results = m_model.Results

            If m_model.RunState = eRunType.Search Then
                Try 'incase m_results is not a cSearchResults object
                    System.Console.WriteLine("F2TS: Run Step. SS = " & DirectCast(m_results, cSearchResults).IterSS)
                Catch ex As Exception
                    'dont need to do anything this is just for debugging
                End Try
            End If


            ' Call delegate
            m_SyncObject.BeginInvoke(Me.m_runstepHandler, Nothing)

        Catch ex As Exception
            cLog.Write(ex)
            Debug.Assert(False, "The interface has thrown an exception that was handled by " & Me.ToString)
        End Try

    End Sub


    ''' <summary>
    ''' Delegate handler called by the model when the run has stopped
    ''' </summary>
    ''' <param name="runType"></param>
    ''' <remarks>This handler is passed to the model during the contruction of the manager via cF2TSModel.Init()</remarks>
    Private Sub RunStoppedCallback(ByVal runType As eRunType)

        Try
            'keep a reference
            m_results = m_model.Results
            Dim objs(0) As Object
            objs(0) = runType

            System.Console.WriteLine("F2TS: Run Stopped.")

            'call anything that needs to be called at the end of a model run via the m_SyncObject 
            'so that it will be marshalled to the interfaces thread
            Dim dlgRunStopped As RunStoppedDelegate = AddressOf Me.ThreadSafeRunStopped
            m_SyncObject.BeginInvoke(dlgRunStopped, objs)

            'once all the processing has completed clear the semaphore and set the signal
            Me.m_semaphore.Release()
            m_SignalState.Set()
            Me.m_runSilent = False

            Me.m_thrdRun = Nothing

        Catch ex As Exception

            Me.m_semaphore.Release()
            m_SignalState.Set()

            cLog.Write(ex)
            Debug.Assert(False, ex.Message)
        End Try


    End Sub

    Private Sub ThreadSafeRunStopped(ByVal runType As eRunType)

        'THIS MUST BE CALLED ON THE INTERFACES THREAD VIA m_SyncObject
        'do anything at the end of a model run that may interact with the interface thread here
        Try
            Dim objs(0) As Object
            objs(0) = runType

            'the core may send messages that are handled by the interface

            m_core.VulnerabilitiesChanged()
            m_core.LoadEcosimStats()

            If Not Me.m_runSilent Then

                'send any messages created by the fit to time series
                For Each msg As cMessage In m_lstMessages
                    m_core.Messages.AddMessage(msg)
                Next
                m_core.Messages.sendAllMessages()
                m_lstMessages.Clear()
            End If

            ' Call delegate on the interfaces thread
            m_SyncObject.BeginInvoke(Me.m_runstoppedHandler, objs)

        Catch ex As Exception
            cLog.Write(ex)
            'this should work because this routine MUST be on the interface thread!!!!
            m_core.Messages.SendMessage(New cMessage("Fit to Time Series error: " & ex.Message, eMessageType.ErrorEncountered, eCoreComponentType.EcoSimFitToTimeSeries, eMessageImportance.Warning))
        End Try

    End Sub

    Private Sub RunModelCallBack(ByVal runType As eRunType, ByVal iCurrentIterationStep As Integer, ByVal nTotalInterationSteps As Integer)
        Dim parms(2) As Object

        Try
            System.Console.WriteLine("F2TS: Ecosim called.")
            parms(0) = runType
            parms(1) = iCurrentIterationStep
            parms(2) = nTotalInterationSteps

            ' Call delegate
            m_SyncObject.BeginInvoke(Me.m_runModelHandler, parms)

        Catch ex As Exception
            cLog.Write(ex)

        End Try
    End Sub


    ''' <summary>
    ''' Delegate handler for Model to add a message to the managers list of messages
    ''' </summary>
    ''' <param name="msg"></param>
    ''' <remarks> </remarks>
    Private Sub AddMessageCallback(ByVal msg As cMessage)
        Try
            If Me.m_runSilent Then
                System.Console.WriteLine(Me.ToString & " Tried to send message while running in Silent Mode.")
                Exit Sub
            End If
            'add the message to the list of messages
            m_lstMessages.Add(msg)
        Catch ex As Exception
            cLog.Write(ex)
        End Try

    End Sub

    ''' <summary>
    ''' Delegate handler for Model to add a message to the managers list of messages
    ''' </summary>
    ''' <param name="msg"></param>
    ''' <remarks> </remarks>
    Private Sub SendMessageCallback(ByVal msg As cMessage)
        Try
            Dim objs(0) As Object

            If Me.m_runSilent Then
                System.Console.WriteLine(Me.ToString & " Tried to send message while running in Silent Mode.")
                Exit Sub
            End If

            objs(0) = msg

            'call ThreadSafeSendMessage() via the m_SyncObject 
            'this will put ThreadSafeSendMessage() on the interface thread
            Dim dlgSenMessage As RunMessageDelegate = AddressOf Me.ThreadSafeSendMessage
            m_SyncObject.BeginInvoke(dlgSenMessage, objs)

        Catch ex As Exception
            cLog.Write(ex)
        End Try

    End Sub

    Private Sub ThreadSafeSendMessage(ByVal msg As cMessage)
        Try
            m_core.Messages.SendMessage(msg)
        Catch ex As Exception
            cLog.Write(ex)
        End Try

    End Sub

#End Region ' Internal model handling

    Protected Overrides Sub Finalize()
        MyBase.Finalize()

        If Me.m_thrdRun IsNot Nothing Then
            'I dont think this can happen
            'the thread is kill when the form is unloaded but just in case
            Me.m_thrdRun.Abort()
            Me.m_thrdRun = Nothing
        End If

    End Sub

#Region "ISearchObjective implementation"

    Public ReadOnly Property FleetObjectives(ByVal iFleet As Integer) As cSearchObjectiveFleetInput Implements ISearchObjective.FleetObjectives
        Get
            Return Me.m_searchObjective.FleetObjectives(iFleet)
        End Get
    End Property

    Public ReadOnly Property GroupObjectives(ByVal iGroup As Integer) As cSearchObjectiveGroupInput Implements ISearchObjective.GroupObjectives
        Get
            Return Me.m_searchObjective.GroupObjectives(iGroup)
        End Get
    End Property

    Public ReadOnly Property ObjectiveParameters() As SearchObjectives.cSearchObjectiveParameters Implements SearchObjectives.ISearchObjective.ObjectiveParameters
        Get
            Return Me.m_searchObjective.ObjectiveParameters
        End Get
    End Property

    Public ReadOnly Property ValueWeights() As cSearchObjectiveWeights Implements ISearchObjective.ValueWeights
        Get
            Return Me.m_searchObjective.ValueWeights
        End Get
    End Property

#End Region

End Class
