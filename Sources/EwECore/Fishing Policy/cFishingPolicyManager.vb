' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.Threading
Imports EwECore.SearchObjectives
Imports EwEUtils.Utilities
Imports Microsoft.Extensions.Logging
Imports Debug = System.Diagnostics.Debug

Namespace FishingPolicy

    Public Class cFishingPolicyManager
        Inherits cThreadWaitBase 'provides the Wait() method
        Implements ICoreInterface
        Implements ISearchObjective

#Region "Private Variables"

        Private m_FPsearch As cFishingPolicySearch
        Private m_core As cCore

        ' Private m_lstGroups As New cCoreInputOutputList(Of cSearchObjectiveGroupInput)(eDataTypes.FishingPolicyGroupInput, 1)
        Private m_lstFleets As New cCoreInputOutputList(Of cFishingPolicySearchBlock)(eDataTypes.FishingPolicySearchBlocks, 1)
        Private m_parameters As cFishingPolicyParameters
        Private m_lstMessages As New List(Of cMessage)
        Private m_results As cFPSSearchResults
        Private m_searchObjective As cSearchObjective

        Private m_syncObject As System.ComponentModel.ISynchronizeInvoke
        Private m_SearchCompletedDelegate As SearchCompletedDelegate
        Private m_RunCompletedDelegate As RunCompletedDelegate
        Private m_ProgressDelegate As ProgressDelegate
        Private m_StartRunDelegate As RunStartedDelegate

        Private ReadOnly m_logger As ILogger = LoggingContext.CreateLogger(Of cFishingPolicyManager)()

        'Private m_PluginManager As c
        Private Delegate Sub CallingThreadDelegate()

#End Region

#Region "Construction and Initialization"

        ''' <summary>
        ''' Connect an interface to the Fishing Policy Search
        ''' </summary>
        ''' <param name="RunStartedCallBack">Callback a search run is about to start. If ModelParameters.nRun > 1 this will be call at the start of each run.</param>
        ''' <param name="RunCompletedBack">Callback a search run has completed. If ModelParameters.nRun > 1 this will be call at the end of each run.</param>
        ''' <param name="ProgressCallBack">Callback reports progress of the search</param>
        ''' <param name="SearchCompletedCallBack">Calback all search runs have completed.</param>
        ''' <remarks></remarks>
        Public Sub Connect(RunStartedCallBack As RunStartedDelegate, RunCompletedBack As RunCompletedDelegate,
                            ProgressCallBack As ProgressDelegate, SearchCompletedCallBack As SearchCompletedDelegate)

            Me.m_StartRunDelegate = RunStartedCallBack
            Me.m_RunCompletedDelegate = RunCompletedBack
            Me.m_ProgressDelegate = ProgressCallBack
            Me.m_SearchCompletedDelegate = SearchCompletedCallBack

        End Sub

        Public Sub DisConnect()

            Me.m_StartRunDelegate = Nothing
            Me.m_RunCompletedDelegate = Nothing
            Me.m_ProgressDelegate = Nothing
            Me.m_SearchCompletedDelegate = Nothing

        End Sub

        Friend Sub New()

        End Sub

        ''' <summary>
        ''' Build interface objects
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Friend Function Init(ByRef theCore As cCore) As Boolean Implements ISearchObjective.Init
            Try

                Me.m_core = theCore

                'init the Fihsing Policy Search model
                Me.m_FPsearch = New cFishingPolicySearch
                Me.m_FPsearch.init(Me.m_core)
                Me.m_FPsearch.SearchCompletedCallBack = AddressOf Me.OnFPSCompletedHandler
                Me.m_FPsearch.AddMessageCallBack = AddressOf Me.OnFPSAddMessageHandler
                Me.m_FPsearch.ProgressCallBack = AddressOf Me.OnFPSProgressHandler
                Me.m_FPsearch.SearchStartedCallBack = AddressOf Me.OnFPSRunStartedHandler
                Me.m_FPsearch.RunCompletedCallBack = AddressOf Me.OnFPSRunCompletedHandler

                'init object for interface
                Me.m_parameters = New cFishingPolicyParameters(Me.m_core, cCore.NULL_VALUE)

                'get the search objective object from the core
                'this is Group, Fleet and Parameters for the shared search interface ISearchObjective
                Me.m_searchObjective = Me.m_core.SearchObjective

                'Init the search data
                Dim search As cSearchDatastructures = Me.m_core.m_SearchData

                'redims and sets frate, Jobs and TargetProfitability to default values
                'search.bInSearch = True
                search.SearchMode = eSearchModes.FishingPolicy

                ''sets BGoalValue() as a function of PB from last ecopath run
                'search.setDefaultBGoal(m_core.m_EcoPathData.PB)

                'set block codes to defaults, a code for each fleet
                search.SetDefaultFBlockCodes()

                'this will set ParNumber() and BlockNumber() based on the defaults set above
                search.SetFletchPars()

                Me.m_lstFleets.Clear()
                Dim flt As cFishingPolicySearchBlock
                For iflt As Integer = 1 To Me.m_core.nFleets
                    'use the database ID for the Fleets
                    flt = New cFishingPolicySearchBlock(Me.m_core, Me.m_core.m_EcopathData.FleetDBID(iflt))
                    Me.m_lstFleets.Add(flt)
                Next

                'set the search back to false 
                ' search.bInSearch = False
                search.SearchMode = eSearchModes.NotInSearch
                Return True

            Catch ex As Exception
                m_logger.LogError(ex, "Error initializing Fishing Policy Manager")
                Return False
            End Try

        End Function

        ''' <summary>
        ''' Load data into existing interface objects
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Friend Function Load() As Boolean Implements ISearchObjective.Load

            Try
                Dim iflt As Integer

                Dim coreData As cSearchDatastructures = Me.m_core.m_SearchData

                'Model Parameters
                Me.m_parameters.AllowValidation = False
                Me.m_parameters.InitOption = eInitOption.EcopathBaseF

                Me.m_parameters.MaxNumEval = coreData.nInterations
                Me.m_parameters.nRuns = coreData.nRuns
                Me.m_parameters.IncludeComp = coreData.IncludeCompetitiveImpact
                Me.m_parameters.MaxEffChange = coreData.MaxEffortChange
                Me.m_parameters.UseEconomicPlugin = coreData.FPSUseEconomicPlugin

                Me.m_parameters.ResetStatusFlags()

                Me.m_parameters.AllowValidation = True

                For Each flt As cFishingPolicySearchBlock In Me.m_lstFleets
                    flt.AllowValidation = False

                    iflt = Array.IndexOf(Me.m_core.m_EcopathData.FleetDBID, flt.DBID)
                    flt.Index = iflt

                    flt.Resize()
                    flt.Name = Me.m_core.m_EcopathData.FleetName(iflt)

                    For it As Integer = 1 To Me.m_core.nEcosimYears
                        flt.SearchBlocks(it) = coreData.FblockCode(iflt, it)
                    Next it

                    flt.AllowValidation = True

                Next

            Catch ex As Exception

            End Try

        End Function

        Public Sub Clear() Implements ISearchObjective.Clear

            Try

                If Me.m_lstFleets IsNot Nothing Then Me.m_lstFleets.Clear()
                If Me.m_parameters IsNot Nothing Then Me.m_parameters.Clear()

                If Me.m_FPsearch IsNot Nothing Then
                    Me.m_FPsearch.SearchCompletedCallBack = Nothing
                    Me.m_FPsearch.AddMessageCallBack = Nothing
                    Me.m_FPsearch.ProgressCallBack = Nothing
                    Me.m_FPsearch.SearchStartedCallBack = Nothing
                    Me.m_FPsearch.RunCompletedCallBack = Nothing
                End If
                Me.m_FPsearch = Nothing

            Catch ex As Exception
                m_logger.LogError(ex, "Error clearing Fishing Policy Manager")
            End Try

        End Sub

        ''' <summary>
        ''' Update the underlying data with values from the interface
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Update(DataType As eDataTypes) As Boolean Implements ISearchObjective.Update
            Dim coreData As cSearchDatastructures = Me.m_core.m_SearchData

            'this will set the number of Frate() dimensions and populate it with default values
            'updating Frates() with FblockCode() for a time step is done by the model 
            coreData.nBlocks = Me.nSearchBlocks

            'load the code blocks
            For Each flt As cFishingPolicySearchBlock In Me.m_lstFleets

                'coreData.Jobs(flt.Index) = flt.JobCatchValue
                'coreData.TargetProfitability(flt.Index) = flt.TargetProfitability

                For it As Integer = 1 To Me.m_core.nEcosimYears
                    coreData.FblockCode(flt.Index, it) = flt.SearchBlocks(it)
                Next it
            Next

            'Model Parameters
            coreData.SearchMethod = Me.m_parameters.SearchOption

            coreData.InitOption = Me.m_parameters.InitOption
            coreData.SearchMethod = Me.m_parameters.SearchOption
            coreData.IncludeCompetitiveImpact = Me.m_parameters.IncludeComp
            coreData.FPSUseEconomicPlugin = Me.m_parameters.UseEconomicPlugin

            coreData.PortFolio = Me.m_parameters.MaxPortUtil

            coreData.nInterations = CInt(Me.m_parameters.MaxNumEval)
            coreData.nRuns = Me.m_parameters.nRuns

            coreData.MaxEffortChange = Me.m_parameters.MaxEffChange
            If Me.m_parameters.MaxEffChange > 0 Then
                coreData.MinimizeEffortChange = True
            Else
                coreData.MinimizeEffortChange = False
            End If

            'strangeness 
            'if OptimizeApproach is 'System Objective' then the SearchMethod is flet or dfpmin
            'if  OptimizeApproach is 'Base profitability' then SearchMethod is eSearchOption.BaseProfitability
            'this comes from EwE5
            If Me.m_parameters.OptimizeApproach = eOptimizeApproachTypes.FleetValues Then
                coreData.SearchMethod = eSearchOptionTypes.BaseProfitability
            End If

        End Function

        '''' <summary>
        '''' Set the base year in the Searchblocks
        '''' </summary>
        '''' <remarks></remarks>
        'Friend Sub UpdateBaseYear()

        '    Try

        '        Dim coreData As cSearchDatastructures = m_core.m_SearchData

        '        'get the new base year
        '        Dim newBaseYear As Integer = Me.m_searchObjective.ObjectiveParameters.BaseYear
        '        Dim iOrgBaseYear As Integer
        '        Dim yearOffset As Integer = 1

        '        'the base year has not been set in the core data yet 
        '        'so use coreData.BaseYear to set the old value to something usefull
        '        If coreData.BaseYear = m_core.nEcosimYears Then
        '            yearOffset = -1
        '        End If

        '        iOrgBaseYear = coreData.BaseYear
        '        For iflt As Integer = 1 To m_core.nFleets

        '            Dim clearCode As Integer = coreData.FblockCode(iflt, iOrgBaseYear + yearOffset)
        '            coreData.FblockCode(iflt, iOrgBaseYear) = clearCode 'reset the original search code
        '            coreData.FblockCode(iflt, Me.m_searchObjective.ObjectiveParameters.BaseYear) = 0 'set the new base year

        '        Next iflt

        '        For Each flt As cFishingPolicySearchBlock In m_lstFleets
        '            For it As Integer = 1 To m_core.nEcosimYears
        '                flt.SearchBlocks(it) = coreData.FblockCode(flt.Index, it)
        '            Next it
        '        Next

        '        coreData.BaseYear = Me.m_searchObjective.ObjectiveParameters.BaseYear

        '    Catch ex As Exception
        '        m_logger.LogError(ex, "Error updating Fishing Policy Manager base year")
        '    End Try
        'End Sub

#End Region

#Region "Public Properties"

        Public ReadOnly Property SearchBlocks(iFleet As Integer) As cFishingPolicySearchBlock
            Get
                Return Me.m_lstFleets(iFleet)
            End Get
        End Property

        Public ReadOnly Property ModelParameters() As cFishingPolicyParameters
            Get
                Return Me.m_parameters
            End Get
        End Property

        ''' <summary>
        ''' Number of unique search blocks across all the fleets
        ''' </summary>
        Public ReadOnly Property nSearchBlocks() As Integer
            Get
                Dim nblocks As New List(Of Integer)
                Dim bindex As Integer
                Dim value As Integer
                For Each flt As cFishingPolicySearchBlock In Me.m_lstFleets

                    For i As Integer = 1 To Me.m_core.nEcosimYears
                        value = flt.SearchBlocks(i)
                        ' Only count search (non-zero) blocks
                        If (value > 0) Then
                            bindex = nblocks.IndexOf(value)
                            If (bindex < 0) Then
                                nblocks.Add(value)
                            End If
                        End If
                    Next i
                Next
                Return nblocks.Count

            End Get
        End Property

        ''' <summary>
        ''' Progress results of the search
        ''' </summary>
        ''' <remarks>This object will be populated at for each call to the ProgressHandler() delegate</remarks>
        Public ReadOnly Property SearchResults() As cFPSSearchResults
            Get
                Return Me.m_FPsearch.Results
            End Get
        End Property

        ''' <summary>
        ''' Count of the current search run
        ''' </summary>
        ''' <remarks>if isRunning = True then iRun will be the count of the current run out of ModelParameters.nRuns</remarks>
        Public ReadOnly Property iRun() As Integer
            Get
                Return Me.m_FPsearch.iRun
            End Get
        End Property

        ''' <summary>
        ''' Stop the Fishing Policy Search run
        ''' </summary>
        ''' <remarks>This will not do anything if the search is not running</remarks>
        Public Overrides Function StopRun(Optional WaitTimeInMillSec As Integer = -1) As Boolean 'Implements SearchObjectives.ISearchObjective.StopRun

            Dim result As Boolean = True
            Try
                If Me.m_FPsearch IsNot Nothing Then
                    Me.m_FPsearch.SearchFailed = True
                    Me.m_FPsearch.StopEstimation = True

                    result = Me.Wait(WaitTimeInMillSec)

                End If

            Catch ex As Exception
                result = False
            End Try
            Return result

        End Function

#End Region

#Region "private handlers for search callbacks/delegates"

        Private Sub OnFPSCompletedHandler()

            Try

                Me.m_core.m_SearchData.SearchMode = eSearchModes.NotInSearch

                'release any waiting threads
                Me.ReleaseWait()

                'send any messages that the model added to the managers list of messages
                'by using the m_syncObject the messages will be sent on the Interfaces thread not the FPS thread
                Dim ctd As CallingThreadDelegate = AddressOf Me.OnSendCoreMessages
                Me.m_syncObject.BeginInvoke(ctd, Nothing)

                ctd = AddressOf Me.OnChanged
                Me.m_syncObject.BeginInvoke(ctd, Nothing)

                If Me.m_SearchCompletedDelegate IsNot Nothing Then
                    'call the delegate supplied by the interface
                    Me.m_syncObject.BeginInvoke(Me.m_SearchCompletedDelegate, Nothing)
                End If

                ctd = Nothing
                Me.m_syncObject = Nothing

            Catch ex As Exception
                m_logger.LogError(ex, "Error in Fishing Policy Manager completion handler")
                Me.m_core.m_SearchData.SearchMode = eSearchModes.NotInSearch
                Me.ReleaseWait()
            End Try

        End Sub

        Private Sub OnFPSProgressHandler()

            Try

                Me.m_results = Me.m_FPsearch.Results

                If Me.m_ProgressDelegate IsNot Nothing Then
                    'call the delegate supplied by the interface
                    Me.m_syncObject.BeginInvoke(Me.m_ProgressDelegate, Nothing)
                End If

            Catch ex As Exception
                m_logger.LogError(ex, "OnFPSProgressHandler. Error in Fishing Policy Manager progress handler")
            End Try

        End Sub

        Private Sub OnFPSRunCompletedHandler()

            Try

                Me.m_results = Me.m_FPsearch.Results

                If Me.m_RunCompletedDelegate IsNot Nothing Then
                    'call the delegate supplied by the interface
                    Me.m_syncObject.Invoke(Me.m_RunCompletedDelegate, Nothing)
                End If

            Catch ex As Exception
                m_logger.LogError(ex, "OnFPSRunCompletedHandler. Error in Fishing Policy Manager run completed handler")
            End Try

        End Sub

        Private Sub OnFPSAddMessageHandler(ByRef message As cMessage)
            'add the message to the managers list of mesasges
            'these messages will be sent at the end of the run
            Me.m_lstMessages.Add(message)

        End Sub

        Private Sub OnFPSRunStartedHandler()
            Dim ctd As CallingThreadDelegate = Nothing

            Try

                ' Debug.Assert(Me.m_StartRunDelegate IsNot Nothing, "Fishing Policy Manager SearchStarted() has not been set.")
                If Me.m_StartRunDelegate IsNot Nothing Then
                    'call the delegate supplied by the interface
                    Me.m_syncObject.BeginInvoke(Me.m_StartRunDelegate, Nothing)
                End If

            Catch ex As Exception
                m_logger.LogError(ex, "OnFPSRunStartedHandler. Error in Fishing Policy Manager run started handler")
            End Try

        End Sub

        Private Sub OnSendCoreMessages()
            Try
                For Each msg As cMessage In Me.m_lstMessages
                    Me.m_core.Messages.AddMessage(msg)
                Next
                Me.m_core.Messages.sendAllMessages()
                Me.m_lstMessages.Clear()
            Catch ex As Exception
                'this should never happen!!!!! ehhhh
                m_logger.LogError(ex, "OnSendCoreMessages. Error sending core messages from Fishing Policy Manager")
            End Try
        End Sub

        Private Sub OnChanged()
            Try
                Me.m_core.onChanged(Me)
            Catch ex As Exception
                'this should never happen!!!!! ehhhh
                m_logger.LogError(ex, "OnChanged. Error sending change notification from Fishing Policy Manager")
            End Try
        End Sub

#End Region

#Region "Running the model"

        Public Function Run(SyncObject As System.ComponentModel.ISynchronizeInvoke) As Boolean

            ' This should not happen
            If Me.IsRunning Then Return False

            Me.m_syncObject = SyncObject
            Dim FPSthread As Thread
            Dim search As cSearchDatastructures = Me.m_core.m_SearchData
            Dim bsuccess As Boolean = True

            Try
                Me.SetWait()

                search.SearchMode = eSearchModes.FishingPolicy
                Me.m_core.m_EcoSimData.bTimestepOutput = True
                Me.Update(Me.DataType)

                FPSthread = New Thread(AddressOf Me.m_FPsearch.Run)
                FPSthread.Start()

            Catch ex As Exception
                m_logger.LogError(ex, "Run. Error starting Fishing Policy Search run")
                'unblock the thread before doing anything incase something has called Wait()

                search.SearchMode = eSearchModes.NotInSearch
                Me.m_core.Messages.SendMessage(New cMessage(cStringUtils.Localize(My.Resources.CoreMessages.FPS_RUN_ERROR, ex.Message),
                                                            eMessageType.ErrorEncountered, eCoreComponentType.FishingPolicySearch, eMessageImportance.Critical, eDataTypes.FishingPolicyManager))

                'if an error has been thrown make sure the SearchCompletedCallBack delegate is called
                'this way an interface can responded 
                Me.OnFPSCompletedHandler()
                bsuccess = False

            End Try

            'send any messages generated from starting the search
            Me.OnSendCoreMessages()
            Return bsuccess

        End Function

#End Region

#Region "ICoreInterface implementation"

        Public ReadOnly Property DataType() As eDataTypes Implements ICoreInterface.DataType
            Get
                Return eDataTypes.FishingPolicyManager
            End Get
        End Property

        Public ReadOnly Property CoreComponent() As eCoreComponentType Implements ICoreInterface.CoreComponent
            Get
                Return eCoreComponentType.Ecosim
            End Get
        End Property

        Public Property DBID() As Integer Implements ICoreInterface.DBID
            Get
                Return cCore.NULL_VALUE
            End Get
            Set(value As Integer)

            End Set
        End Property

        Public Function GetID() As String Implements ICoreInterface.GetID
            Return Me.ToString
        End Function

        Public Property Index() As Integer Implements ICoreInterface.Index
            Get

            End Get
            Set(value As Integer)
                Debug.Assert(False, "Can not set the Index of " & Me.ToString)
            End Set
        End Property

        Public Property Name() As String Implements ICoreInterface.Name
            Get
                Return Me.ToString
            End Get
            Set(value As String)
                Debug.Assert(False, "Can not set the Name of " & Me.ToString)
            End Set
        End Property

#End Region

#Region "ISearchObjective implementation"

        Public ReadOnly Property FleetObjectives(iFleet As Integer) As cSearchObjectiveFleetInput Implements ISearchObjective.FleetObjectives
            Get
                Return Me.m_searchObjective.FleetObjectives(iFleet)
            End Get
        End Property

        Public ReadOnly Property GroupObjectives(iGroup As Integer) As cSearchObjectiveGroupInput Implements ISearchObjective.GroupObjectives
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

End Namespace