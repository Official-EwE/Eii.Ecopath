'==============================================================================
'
' $Log: cFishingPolicyManager.vb,v $
' Revision 1.4  2009/01/16 18:30:29  jeroens
' eMessageSource renamed to eCoreComponentTypes
'
' Revision 1.3  2008/12/02 19:07:21  joeb
' Added flag for computation of EcoSim timestep ouput
'
' Revision 1.2  2008/10/29 19:51:02  joeb
' Bug Fix When doing multiple runs the interface was not getting a chance to update before the next set of iterations started. Made RunCompleted delegate synchronous
'
' Revision 1.1  2008/09/26 07:30:23  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.43  2008/09/24 00:11:03  villyc
' f limits and others
'
' Revision 1.42  2008/08/11 21:09:10  joeb
' Changes for Bug Fix 459 Added Search Modes
'
' Revision 1.41  2008/06/06 15:56:04  joeb
' Moved eDataTypes to EwEUtils.Core
'
' Revision 1.40  2008/06/03 16:42:32  joeb
' Fixed Bug 326
'
' Revision 1.39  2008/05/12 18:58:35  joeb
' Restructure of search objects to use ISearchObjective interface
'
' Revision 1.38  2008/05/06 20:53:31  joeb
' oooooppppssssssssss
'
' Revision 1.37  2008/05/06 20:38:06  joeb
' Fixed Initialization bugs from the last time I comitted oppsssss
'
' Revision 1.36  2008/05/06 20:03:22  joeb
' Minor tweeks to Initialization and error handling
'
' Revision 1.35  2008/04/29 19:29:28  joeb
' Minor changes to error handling
'
' Revision 1.34  2008/04/24 20:02:58  joeb
' Now inherits from cThreadedManagerBase
'
' Revision 1.33  2008/04/23 17:37:05  joeb
' ValWeight now come from SearchDataStructures instead of FishPolicySearch
'
' Revision 1.32  2008/04/17 20:21:22  joeb
' Changed cCore.m_EcosimSearch to cCore.m_SearchData
'
' Revision 1.31  2008/04/17 20:16:59  joeb
' Change  cSearchDataStructures.bDoFPSearch to cSearchDataStructures.bInSearch
'
' Revision 1.30  2008/04/15 15:21:06  joeb
' Added Validation and Updating for BaseYear and SearchBlocks
'
' Revision 1.29  2008/04/11 15:08:57  joeb
' Added a Connect method
'
' Revision 1.28  2008/02/06 16:29:32  jeroens
' Fixed issue 404
'
' Revision 1.27  2008/02/06 00:58:39  jeroens
' Fixed shape update crash: core OnChanged call called on GUI thread in fpsCompletedHandler
'
' Revision 1.26  2007/11/21 14:39:31  jeroens
' * Fixed enums
'
' Revision 1.25  2007/10/03 17:17:31  joeb
' Bug Fixes
'
' Revision 1.24  2007/09/29 16:46:31  joeb
' reload when number of years changes.
'
' Revision 1.23  2007/09/28 18:48:31  joeb
' *** empty log message ***
'
' Revision 1.22  2007/09/24 14:18:23  joeb
' bDoFPSearch changes
'
' Revision 1.21  2007/09/17 21:07:22  joeb
' UseCostPenalty
'
' Revision 1.20  2007/09/13 17:34:07  joeb
' NBlocks no longer includes zero blocks
'
' Revision 1.19  2007/09/13 15:27:49  joeb
' Changes to Delegate/Handlers
'
' Revision 1.18  2007/09/11 20:17:35  joeb
' Hooking interface up to objects
'
' Revision 1.17  2007/09/11 14:49:44  joeb
' Dimensioning
'
' Revision 1.16  2007/09/10 22:54:48  joeb
' more more more always more
'
' Revision 1.15  2007/09/10 22:51:30  joeb
' *** empty log message ***
'
' Revision 1.14  2007/09/10 22:31:47  joeb
' Added SearchForBaseProfitability()
'
' Revision 1.13  2007/09/10 14:46:07  joeb
' still more base code
'
' Revision 1.12  2007/09/09 15:21:28  joeb
' Still adding code
'
' Revision 1.11  2007/09/07 15:28:20  joeb
' Tons O crap!
'
' Revision 1.10  2007/09/04 17:16:11  joeb
' Minor changes for Fishing Policy Search
'
' Revision 1.9  2007/08/31 14:49:50  joeb
' More more more.....
'
' Revision 1.8  2007/08/29 15:41:17  joeb
' Minor changes
'
' Revision 1.7  2007/08/29 14:59:36  joeb
' Added a bunch of computational code
'
' Revision 1.6  2007/08/27 15:24:08  joeb
' Added Log header
'
'
'==============================================================================



Imports EwECore.Ecosim
Imports System.Threading
Imports EwECore.SearchObjectives
Imports EwEUtils.Core


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
        Public Sub Connect(ByVal RunStartedCallBack As RunStartedDelegate, ByVal RunCompletedBack As RunCompletedDelegate, _
                            ByVal ProgressCallBack As ProgressDelegate, ByVal SearchCompletedCallBack As SearchCompletedDelegate)

            m_StartRunDelegate = RunStartedCallBack
            m_RunCompletedDelegate = RunCompletedBack
            m_ProgressDelegate = ProgressCallBack
            m_SearchCompletedDelegate = SearchCompletedCallBack

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

                m_core = theCore

                'init the Fihsing Policy Search model
                m_FPsearch = New cFishingPolicySearch
                m_FPsearch.init(m_core)
                m_FPsearch.SearchCompletedCallBack = AddressOf Me.OnFPSCompletedHandler
                m_FPsearch.AddMessageCallBack = AddressOf Me.OnFPSAddMessageHandler
                m_FPsearch.ProgressCallBack = AddressOf OnFPSProgressHandler
                m_FPsearch.SearchStartedCallBack = AddressOf OnFPSRunStartedHandler
                m_FPsearch.RunCompletedCallBack = AddressOf OnFPSRunCompletedHandler

                'init object for interface
                m_parameters = New cFishingPolicyParameters(m_core, cCore.NULL_VALUE)

                'get the search objective object from the core
                'this is Group, Fleet and Parameters for the shared search interface ISearchObjective
                m_searchObjective = m_core.SearchObjective

                'Init the search data
                Dim search As cSearchDatastructures = m_core.m_SearchData

                'redims and sets frate, Jobs and TargetProfitability to default values
                'search.bInSearch = True
                search.SearchMode = eSearchModes.FishingPolicy

                ''sets BGoalValue() as a function of PB from last ecopath run
                'search.setDefaultBGoal(m_core.m_EcoPathData.PB)

                'set block codes to defaults, a code for each fleet
                search.setDefaultFBlockCodes()

                'this will set ParNumber() and BlockNumber() based on the defaults set above
                search.SetFletchPars()

                m_lstFleets.Clear()
                Dim flt As cFishingPolicySearchBlock
                For iflt As Integer = 1 To m_core.nFleets
                    'use the database ID for the Fleets
                    flt = New cFishingPolicySearchBlock(m_core, m_core.m_EcoPathData.FleetDBID(iflt))
                    m_lstFleets.Add(flt)
                Next

                'set the search back to false 
                ' search.bInSearch = False
                search.SearchMode = eSearchModes.NotInSearch
                Return True

            Catch ex As Exception
                cLog.Write(ex)
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

                Dim coreData As cSearchDatastructures = m_core.m_SearchData

                'Model Parameters
                m_parameters.AllowValidation = False
                m_parameters.InitOption = eInitOption.EcopathBaseF

                m_parameters.MaxNumEval = coreData.nInterations
                m_parameters.nRuns = coreData.nRuns
                m_parameters.IncludeComp = coreData.IncludeCompetitiveImpact
                m_parameters.MaxEffChange = coreData.MaxEffortChange

                m_parameters.ResetStatusFlags()

                m_parameters.AllowValidation = True

                For Each flt As cFishingPolicySearchBlock In m_lstFleets
                    flt.AllowValidation = False

                    iflt = Array.IndexOf(m_core.m_EcoPathData.FleetDBID, flt.DBID)
                    flt.Index = iflt

                    flt.Resize()
                    flt.Name = m_core.m_EcoPathData.FleetName(iflt)

                    For it As Integer = 1 To m_core.nEcosimYears
                        flt.SearchBlocks(it) = coreData.FblockCode(iflt, it)
                    Next it

                    flt.AllowValidation = True

                Next

            Catch ex As Exception

            End Try

        End Function

        ''' <summary>
        ''' Update the underlying data with values from the interface
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Update(ByVal DataType As eDataTypes) As Boolean Implements ISearchObjective.Update
            Dim coreData As cSearchDatastructures = m_core.m_SearchData

            'this will set the number of Frate() dimensions and populate it with default values
            'updating Frates() with FblockCode() for a time step is done by the model 
            coreData.nBlocks = Me.nSearchBlocks

            'load the code blocks
            For Each flt As cFishingPolicySearchBlock In m_lstFleets

                'coreData.Jobs(flt.Index) = flt.JobCatchValue
                'coreData.TargetProfitability(flt.Index) = flt.TargetProfitability

                For it As Integer = 1 To m_core.nEcosimYears
                    coreData.FblockCode(flt.Index, it) = flt.SearchBlocks(it)
                Next it
            Next

            'Model Parameters
            coreData.SearchMethod = Me.m_parameters.SearchOption

            coreData.InitOption = Me.m_parameters.InitOption
            coreData.SearchMethod = Me.m_parameters.SearchOption
            coreData.IncludeCompetitiveImpact = Me.m_parameters.IncludeComp

            coreData.PortFolio = Me.m_parameters.MaxPortUtil

            coreData.nInterations = m_parameters.MaxNumEval
            coreData.nRuns = m_parameters.nRuns


            coreData.MaxEffortChange = m_parameters.MaxEffChange
            If m_parameters.MaxEffChange > 0 Then
                coreData.MinimizeEffortChange = True
            Else
                coreData.MinimizeEffortChange = False
            End If

            'strangeness 
            'if OptimizeApproach is 'System Objective' then the SearchMethod is flet or dfpmin
            'if  OptimizeApproach is 'Base profitability' then SearchMethod is eSearchOption.BaseProfitability
            'this comes from EwE5
            If m_parameters.OptimizeApproach = eOptimizeApproachTypes.FleetValues Then
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
        '        cLog.Write(ex)
        '    End Try
        'End Sub


#End Region

#Region "Public Properties"

  

        Public ReadOnly Property SearchBlocks(ByVal iFleet As Integer) As cFishingPolicySearchBlock
            Get
                Return m_lstFleets(iFleet)
            End Get
        End Property

        Public ReadOnly Property ModelParameters() As cFishingPolicyParameters
            Get
                Return m_parameters
            End Get
        End Property

   

        ''' <summary>
        ''' Number of unique search blocks across all the fleets
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property nSearchBlocks() As Integer
            Get

                Dim nblocks As New List(Of Integer)
                Dim bindex As Integer
                Dim value As Integer
                For Each flt As cFishingPolicySearchBlock In m_lstFleets

                    For i As Integer = 1 To m_core.nEcosimYears
                        value = flt.SearchBlocks(i)
                        If value > 0 Then 'don't count zero blocks these do not contain a search block
                            bindex = nblocks.IndexOf(value)
                            If bindex < 0 Then
                                nblocks.Add(value)
                            End If
                        End If
                    Next i
                Next
                Return nblocks.Count
            End Get

        End Property

        'All Callbacks have been moved to the Connect(...) method
        '''' <summary>
        '''' A Fishing Policy search has completed all it's runs.
        '''' </summary>
        '''' <remarks>If there are multiple runs they have all been completed or an Error has occured and the runs could not be completed.</remarks>
        'Public WriteOnly Property SearchCompletedHandler() As SearchCompletedDelegate
        '    Set(ByVal value As SearchCompletedDelegate)
        '        m_SearchCompletedDelegate = value
        '    End Set
        'End Property

        '''' <summary>
        '''' Progress of the current Fishing Policy run
        '''' </summary>
        '''' <remarks>The Results object will contian the results of the current iteration</remarks>
        'Public WriteOnly Property ProgressHandler() As ProgressDelegate
        '    Set(ByVal value As ProgressDelegate)
        '        m_ProgressDelegate = value
        '    End Set
        'End Property

        '''' <summary>
        '''' A Fishing Policy Search run has started.
        '''' </summary>
        '''' <remarks>When this is called the SearchResults object will be initialized and dimensioned but it will not contain any values. 
        '''' The number of the run about the be started will be in the iRun poperty </remarks>
        'Public WriteOnly Property RunStartedHandler() As RunStartedDelegate
        '    Set(ByVal value As RunStartedDelegate)
        '        Me.m_StartRunDelegate = value
        '    End Set
        'End Property


        '''' <summary>
        '''' A run of the Fishing Policy search has completed.
        '''' </summary>
        '''' <remarks></remarks>
        'Public WriteOnly Property RunCompletedHandler() As RunCompletedDelegate
        '    Set(ByVal value As RunCompletedDelegate)
        '        Me.m_RunCompletedDelegate = value
        '    End Set
        'End Property




        ''' <summary>
        ''' Progress results of the search
        ''' </summary>
        ''' <remarks>This object will be populated at for each call to the ProgressHandler() delegate</remarks>
        Public ReadOnly Property SearchResults() As cFPSSearchResults
            Get
                Return m_FPsearch.Results
            End Get
        End Property


        ''' <summary>
        ''' Count of the current search run
        ''' </summary>
        ''' <remarks>if isRunning = True then iRun will be the count of the current run out of ModelParameters.nRuns</remarks>
        Public ReadOnly Property iRun() As Boolean
            Get
                Return Me.m_FPsearch.iRun
            End Get
        End Property

        ''' <summary>
        ''' Stop the Fishing Policy Search run
        ''' </summary>
        ''' <remarks>This will not do anything if the search is not running</remarks>
        Public Sub StopRun()
            Me.m_FPsearch.SearchFailed = True
            Me.m_FPsearch.StopEstimation = True
        End Sub

#End Region

#Region "private handlers for search callbacks/delegates"

        Private Sub OnFPSCompletedHandler()

            Try

                m_core.m_SearchData.SearchMode = eSearchModes.NotInSearch

                'release any waiting threads
                Me.ReleaseWait()

                'send any messages that the model added to the managers list of messages
                'by using the m_syncObject the messages will be sent on the Interfaces thread not the FPS thread
                Dim ctd As CallingThreadDelegate = AddressOf Me.OnSendCoreMessages
                m_syncObject.BeginInvoke(ctd, Nothing)

                ctd = AddressOf Me.OnChanged
                m_syncObject.BeginInvoke(ctd, Nothing)

                If m_SearchCompletedDelegate IsNot Nothing Then
                    'call the delegate supplied by the interface
                    m_syncObject.BeginInvoke(m_SearchCompletedDelegate, Nothing)
                End If

            Catch ex As Exception
                cLog.Write(ex)
                m_core.m_SearchData.SearchMode = eSearchModes.NotInSearch
                Me.ReleaseWait()
            End Try

        End Sub


        Private Sub OnFPSProgressHandler()

            Try

                m_results = Me.m_FPsearch.Results

                If m_ProgressDelegate IsNot Nothing Then
                    'call the delegate supplied by the interface
                    m_syncObject.BeginInvoke(Me.m_ProgressDelegate, Nothing)
                End If

            Catch ex As Exception
                cLog.Write(ex)
            End Try

        End Sub

        Private Sub OnFPSRunCompletedHandler()

            Try

                m_results = Me.m_FPsearch.Results

                If m_RunCompletedDelegate IsNot Nothing Then
                    'call the delegate supplied by the interface
                    m_syncObject.Invoke(Me.m_RunCompletedDelegate, Nothing)
                End If

            Catch ex As Exception
                cLog.Write(ex)
            End Try

        End Sub


        Private Sub OnFPSAddMessageHandler(ByRef message As cMessage)
            'add the message to the managers list of mesasges
            'these messages will be sent at the end of the run
            m_lstMessages.Add(message)

        End Sub

        Private Sub OnFPSRunStartedHandler()
            Dim ctd As CallingThreadDelegate = Nothing

            Try

                ' Debug.Assert(Me.m_StartRunDelegate IsNot Nothing, "Fishing Policy Manager SearchStarted() has not been set.")
                If m_StartRunDelegate IsNot Nothing Then
                    'call the delegate supplied by the interface
                    m_syncObject.BeginInvoke(Me.m_StartRunDelegate, Nothing)
                End If

            Catch ex As Exception
                cLog.Write(ex)
            End Try

        End Sub

        Private Sub OnSendCoreMessages()
            Try
                For Each msg As cMessage In m_lstMessages
                    m_core.Messages.AddMessage(msg)
                Next
                m_core.Messages.sendAllMessages()
                m_lstMessages.Clear()
            Catch ex As Exception
                'this should never happen!!!!! ehhhh
                cLog.Write(ex)
            End Try
        End Sub

        Private Sub OnChanged()
            Try
                m_core.onChanged(Me)
            Catch ex As Exception
                'this should never happen!!!!! ehhhh
                cLog.Write(ex)
            End Try
        End Sub

#End Region

#Region "Running the model"

        Public Function Run(ByVal SyncObject As System.ComponentModel.ISynchronizeInvoke) As Boolean

            m_syncObject = SyncObject
            Dim thrdMC As Thread
            Dim search As cSearchDatastructures = m_core.m_SearchData
            Dim bsuccess As Boolean

            Try

                If Me.isRunning Then
                    m_core.Messages.SendMessage(New cMessage("A Fishing Policy Search is already running. Only one search can be run at a time.", eMessageType.ErrorEncountered, _
                                                eCoreComponentType.EcoSimMonteCarlo, eMessageImportance.Critical, eDataTypes.MonteCarlo))
                    Return False
                End If

                bsuccess = True

                Me.setWait()

                search.SearchMode = eSearchModes.FishingPolicy
                Me.m_core.m_EcoSimData.bTimestepOutput = False
                Me.Update(Me.DataType)

                thrdMC = New Thread(AddressOf Me.m_FPsearch.Run)
                thrdMC.Start()

            Catch ex As Exception
                cLog.Write(ex)
                'unblock the thread before doing anything incase something has called Wait()

                search.SearchMode = eSearchModes.NotInSearch
                m_core.Messages.SendMessage(New cMessage("Error running the Fishing Policy Search.", eMessageType.ErrorEncountered, _
                                            eCoreComponentType.FishingPolicySearch, eMessageImportance.Critical, eDataTypes.FishingPolicyManager))

                'if an error has been thrown make sure the SearchCompletedCallBack delegate is called
                'this way an interface can responded 
                OnFPSCompletedHandler()
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
                Return eCoreComponentType.EcoSim
            End Get
        End Property

        Public Property DBID() As Integer Implements ICoreInterface.DBID
            Get
                Return cCore.NULL_VALUE
            End Get
            Set(ByVal value As Integer)

            End Set
        End Property

        Public Function GetID() As String Implements ICoreInterface.GetID
            Return Me.ToString
        End Function

        Public Property Index() As Integer Implements ICoreInterface.Index
            Get

            End Get
            Set(ByVal value As Integer)
                Debug.Assert(False, "Can not set the Index of " & Me.ToString)
            End Set
        End Property

        Public Property Name() As String Implements ICoreInterface.Name
            Get
                Return Me.ToString
            End Get
            Set(ByVal value As String)
                Debug.Assert(False, "Can not set the Name of " & Me.ToString)
            End Set
        End Property

#End Region

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

End Namespace