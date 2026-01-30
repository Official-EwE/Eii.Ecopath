' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.Threading
Imports EwEUtils.Utilities
Imports Microsoft.Extensions.Logging
Imports Debug = System.Diagnostics.Debug



Namespace Ecospace.Advection

    'ToDo 15-Aug-2016 added a variable for the threshold upwelling depth
    'Right now it's hardwired at 30, make that a parameter.
    '   Done 22-Aug-2016

    'ToDo 15-Aug-2016 Check the Flow(,) variable to see if it is still needed?

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Manager for user interfaces to interact with the Ecospace Advection
    ''' calculations.
    ''' </summary>
    ''' <remarks>
    ''' <para>Remote processes can <see cref="cAdvectionManager.Connect">connect</see>
    ''' to this class, providing three delegates to track the progress of advection calculations:
    ''' <list type="bullet">
    ''' <item><description><see cref="cAdvectionManager.ComputationStartedDelegate">ComputationStartedDelegate</see></description></item>
    ''' <item><description><see cref="cAdvectionManager.ComputationProgressDelegate">ComputationProgressDelegate</see></description></item>
    ''' <item><description><see cref="cAdvectionManager.ComputationCompletedDelegate">ComputationCompletedDelegate</see></description></item>
    ''' </list>
    ''' Make sure to properly <see cref="cAdvectionManager.Disconnect">Disconnect</see>
    ''' from the manager when it is no longer needed.</para>
    ''' <para>Any remote process can parameterize the advection calculations
    ''' via <see cref="cAdvectionManager.ModelParameters">ModelParameters</see>. The
    ''' computations use a series of Ecospace layers for input, please see the
    ''' internals of <see cref="cAdvection">cAdvection</see> for details.</para>
    ''' <para>Advection computations are started via <see cref="cAdvectionManager.RunPhysicsModel()">Run</see>.
    ''' Computed results are exposed by the Ecospace <see cref="cEcospaceLayerAdvection">advection layer</see>,
    ''' which can be obtained via <see cref="cEcospaceBasemap.LayerAdvection">cEcospaceBasemap.LayerAdvection</see>.
    ''' </para>
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Public Class cAdvectionManager
        Inherits cThreadWaitBase 'provides the Wait() method
        Implements ICoreInterface

        ''' -------------------------------------------------------------------
        ''' <summary>Delegate that will be called when advection computations are about to start.</summary>
        ''' -------------------------------------------------------------------
        Public Delegate Sub ComputationStartedDelegate()

        ''' -------------------------------------------------------------------
        ''' <summary>Delegate that will be called at the end of each advection iteration.</summary>
        ''' <param name="iIteration">The number of the iteration.</param>
        ''' -------------------------------------------------------------------
        Public Delegate Sub ComputationProgressDelegate(iIteration As Integer)

        ''' -------------------------------------------------------------------
        ''' <summary>Delegate that will be called when advection computations have finished.</summary>
        ''' <param name="iIteration">The number of completed iterations.</param>
        ''' <param name="bInterrupted">Flag stating whether the iterations were interrupted by the user.</param>
        ''' <param name="bBadFlow">Flag stating whether the computed flow was considered 'bad'.</param>
        ''' -------------------------------------------------------------------
        Public Delegate Sub ComputationCompletedDelegate(iIteration As Integer, bInterrupted As Boolean, bBadFlow As Boolean)

#Region " Private Variables "

        Private m_comp As cAdvection = Nothing
        Private m_core As cCore = Nothing
        Private m_parameters As cAdvectionParameters = Nothing
        Private m_data As cEcospaceDataStructures = Nothing
        Private m_lstMessages As New List(Of cMessage)

        Private m_syncObject As System.ComponentModel.ISynchronizeInvoke
        Private m_RunStartedDelegate As ComputationStartedDelegate
        Private m_RunProgressDelegate As ComputationProgressDelegate
        Private m_RunCompletedDelegate As ComputationCompletedDelegate

        Private Delegate Sub CallingThreadDelegate()

        Private ReadOnly m_logger As ILogger = LoggingContext.CreateLogger(Of cAdvectionManager)()

#End Region ' Private Variables

#Region " Construction and Initialization "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Hidden constructor; the manager should be created only once by the 
        ''' EwE core.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Friend Sub New()
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Connect to the Advection manager.
        ''' </summary>
        ''' <param name="ComputationStartedCallBack">Delegate that will be called when 
        ''' advection computations are <see cref="ComputationStartedDelegate">about to start</see>.</param>
        ''' <param name="ComputationCompletedBack">Delegate that will be called at the
        ''' end of <see cref="ComputationProgressDelegate">each iteration</see> of 
        ''' advection computations.</param>
        ''' <param name="ComputationProgressCallBack">Delegate that will be called when 
        ''' advection computations <see cref="ComputationCompletedDelegate">have completed</see>.</param>
        ''' <remarks>Make sure to properly <see cref="Disconnect">Disconnect</see>
        ''' when this manager is no longer needed.</remarks>
        ''' -------------------------------------------------------------------
        Public Sub Connect(ComputationStartedCallBack As ComputationStartedDelegate,
                           ComputationCompletedBack As ComputationCompletedDelegate,
                           ComputationProgressCallBack As ComputationProgressDelegate)

            Me.m_RunStartedDelegate = ComputationStartedCallBack
            Me.m_RunCompletedDelegate = ComputationCompletedBack
            Me.m_RunProgressDelegate = ComputationProgressCallBack

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Disconnect from the Advection manager previously connected via
        ''' <see cref="Connect">Connect</see>.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Sub Disconnect()

            Me.m_RunStartedDelegate = Nothing
            Me.m_RunProgressDelegate = Nothing
            Me.m_RunCompletedDelegate = Nothing

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Initialize the manager for operation.
        ''' </summary>
        ''' <param name="theCore">Core instance to operate upon.</param>
        ''' <param name="theEcospace">Ecospace instance to operate upon.</param>
        ''' -------------------------------------------------------------------
        Friend Function Init(theCore As cCore, theEcospace As cEcoSpace) As Boolean
            Try

                Me.m_core = theCore

                Me.m_comp = New cAdvection()
                Me.m_comp.Init(theCore, theEcospace)
                'm_comp.AddMessageCallback = AddressOf OnAddMessageHandler
                Me.m_comp.ProgressCallback = AddressOf Me.OnAdvectionCalcsProgressHandler
                Me.m_comp.RunStartedCallBack = AddressOf Me.OnAdvectionCalcsStartedHandler
                Me.m_comp.RunCompletedCallback = AddressOf Me.OnAdvectionCalcsCompletedHandler

                'get the data from the core
                Me.m_data = Me.m_core.m_EcospaceData
                Me.m_parameters = New cAdvectionParameters(Me.m_core, -1)

                Return True

            Catch ex As Exception
                m_logger.LogError(ex, "cAdvectionManager.Init Exception")
                Return False
            End Try

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Load data into existing interface objects
        ''' </summary>
        ''' <returns>True if successful.</returns>
        ''' -------------------------------------------------------------------
        Friend Function Load() As Boolean

            Try
                Me.m_parameters.AllowValidation = False

                Me.m_parameters.UpwellingThreshold = Me.m_comp.UpwellingThreshold
                Me.m_parameters.UpwellingPPMultiplier = Me.m_data.PPupWell

                Me.m_parameters.ResetStatusFlags()
                Me.m_parameters.AllowValidation = True
                Return True
            Catch ex As Exception
                Return False
            End Try

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Update the underlying data with values from the interface
        ''' </summary>
        ''' <returns>True if successful.</returns>
        ''' -------------------------------------------------------------------
        Public Function Update() As Boolean

            Me.m_comp.UpwellingThreshold = Me.m_parameters.UpwellingThreshold
            Me.m_data.PPupWell = Me.m_parameters.UpwellingPPMultiplier

            Return True

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Clear the manager data, but leaves the manager ready for future use.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Sub Clear()
            Try
                If Me.m_parameters IsNot Nothing Then
                    Me.m_parameters.Clear()
                End If
            Catch ex As Exception
                Debug.Assert(False, Me.ToString & ".Clear() Exception: " & ex.Message)
            End Try

        End Sub

#End Region '  Construction and Initialization

#Region " Public Properties "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get configurable advection parameters.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property ModelParameters() As cAdvectionParameters
            Get
                Return Me.m_parameters
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Count of the Advection calculations run.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property Iteration() As Integer
            Get
                Return Me.m_comp.Iteration
            End Get
        End Property

        '''' -------------------------------------------------------------------
        '''' <summary>
        '''' Stop the Advection calculations run.
        '''' </summary>
        '''' <remarks>This will not do anything if the search is not running</remarks>
        '''' -------------------------------------------------------------------
        'Public Sub StopRun(Optional WaitTimeInMillSec As Integer = -1)

        'End Sub

#End Region ' Public Properties

#Region " Running "

        ''' <summary>
        ''' Overloaded version to run the PhysicsModel on a single thread
        ''' </summary>
        ''' <remarks></remarks>
        Public Function RunPhysicsModel() As Boolean

            Debug.Assert(Not Me.m_core.StateMonitor.IsBusy,
                         Me.ToString + ".RunPhysicsModel() The Statemonitor thinks the Advection model is already running! This might be a bug.")
            If (Me.m_core.StateMonitor.IsBusy) Then Return False

            'Make sure the sync object isn't pointing to something
            Me.m_syncObject = Nothing

            Dim bSuccess As Boolean

            If Me.IsRunning Then
                Me.m_core.Messages.SendMessage(New cMessage(My.Resources.CoreMessages.COMPUTATION_ALREADY_RUNNING,
                                                            eMessageType.ErrorEncountered,
                                                            eCoreComponentType.Ecospace,
                                                            eMessageImportance.Warning,
                                                            eDataTypes.EcospaceAdvectionManager))
                Return False
            End If


            Try
                bSuccess = Me.m_comp.RunPhysicsModel()
            Catch ex As Exception
                m_logger.LogError(ex, "cAdvectionManager.RunPhysicsModel Exception")
                Me.m_core.Messages.SendMessage(New cMessage(cStringUtils.Localize(My.Resources.CoreMessages.ADVECTION_ERROR, ex.Message),
                                                         eMessageType.ErrorEncountered,
                                                         eCoreComponentType.Ecospace,
                                                         eMessageImportance.Critical,
                                                         eDataTypes.EcospaceAdvectionManager))


                bSuccess = False
            End Try

            Me.m_core.m_SearchData.SearchMode = eSearchModes.NotInSearch
            'send any messages generated from starting the search
            Me.OnSendCoreMessages()

            Return bSuccess
        End Function


        Public Function RunPhysicsModel(SyncObject As System.ComponentModel.ISynchronizeInvoke) As Boolean

            ' Sanity check
            If (Me.m_core.StateMonitor.IsBusy) Then Return False

            Dim thrd As Thread = Nothing
            Dim bSuccess As Boolean = True

            Me.m_syncObject = SyncObject

            If Me.IsRunning Then
                Me.m_core.Messages.SendMessage(New cMessage(My.Resources.CoreMessages.COMPUTATION_ALREADY_RUNNING,
                                                            eMessageType.ErrorEncountered,
                                                            eCoreComponentType.Ecospace,
                                                            eMessageImportance.Warning,
                                                            eDataTypes.EcospaceAdvectionManager))
                Return False
            End If

            Me.SetWait()
            Try

                thrd = New Thread(AddressOf Me.RunThreaded)
                thrd.Start()

            Catch ex As Exception
                m_logger.LogError(ex, "cAdvectionManager.RunPhysicsModel(..) Exception")
                Me.m_core.Messages.SendMessage(New cMessage(cStringUtils.Localize(My.Resources.CoreMessages.ADVECTION_ERROR, ex.Message),
                                                         eMessageType.ErrorEncountered,
                                                         eCoreComponentType.Ecospace,
                                                         eMessageImportance.Critical,
                                                         eDataTypes.EcospaceAdvectionManager))

                ' If an error has been thrown make sure the OnAdvectionCalcsCompletedHandler delegate is called
                ' This way an interface can respond
                Me.OnAdvectionCalcsCompletedHandler(Me.m_comp.Iteration, Me.m_comp.Interrupted, Me.m_comp.BadFlow)

                bSuccess = False
            End Try

            'send any messages generated from starting the search
            Me.OnSendCoreMessages()

            Return bSuccess

        End Function


        Private Sub RunThreaded()

            Me.m_core.StateMonitor.SetIsSearching(eSearchModes.External)
            Me.m_core.SetStopRunDelegate(AddressOf Me.StopRun)

            Try
                Me.m_comp.RunPhysicsModel()
            Catch ex As Exception
                m_logger.LogError(ex, "cAdvectionManager.RunThreaded Exception")
            End Try

            Me.m_core.SetStopRunDelegate(Nothing)
            Me.m_core.StateMonitor.SetIsSearching(eSearchModes.NotInSearch)

        End Sub


        Public Sub ClearAdvectionResults()
            Try
                For imon As Integer = 1 To 12
                    Array.Clear(Me.m_data.MonthlyXvel(imon), 0, Me.m_data.MonthlyXvel(imon).Length)
                    Array.Clear(Me.m_data.MonthlyYvel(imon), 0, Me.m_data.MonthlyXvel(imon).Length)
                    Array.Clear(Me.m_data.MonthlyUpWell(imon), 0, Me.m_data.MonthlyXvel(imon).Length)
                Next

                Return

            Catch ex As Exception
                Debug.Assert(False, "Opps Exception in cAdvectionManager.ClearAdvectionResults(): " & ex.Message)
            End Try

        End Sub


        Public Function Revert() As Boolean

            Me.m_comp.Revert()

            Try
                ' Invalidate layers
                For Each layer As cEcospaceLayer In Me.m_core.EcospaceBasemap.LayerAdvection
                    layer.Invalidate()
                Next
                Return True
            Catch ex As Exception
                m_logger.LogError(ex, "cAdvectionManager.Revert Exception")
            End Try
            Return False

        End Function

        Public Overrides Function StopRun(Optional WaitTimeInMillSec As Integer = -1) As Boolean
            Me.m_comp.Interrupted = True
        End Function

#End Region ' Running

#Region " Events "

        Private Sub OnAdvectionCalcsStartedHandler()
            Dim ctd As CallingThreadDelegate = Nothing

            Try

                If Me.m_RunStartedDelegate IsNot Nothing Then
                    'call the delegate supplied by the interface
                    If Me.m_syncObject IsNot Nothing Then
                        Me.m_syncObject.BeginInvoke(Me.m_RunStartedDelegate, Nothing)
                    Else
                        Me.m_RunStartedDelegate.Invoke()
                    End If
                End If

            Catch ex As Exception
                m_logger.LogError(ex, "cAdvectionManager.OnAdvectionCalcsStartedHandler Exception")
            End Try

        End Sub

        Private Sub OnAdvectionCalcsProgressHandler(iInteration As Integer)

            Try
                If Me.m_RunProgressDelegate IsNot Nothing Then
                    ' Invalidate layers
                    For Each layer As cEcospaceLayer In Me.m_core.EcospaceBasemap.LayerAdvection
                        layer.Invalidate()
                    Next
                    ' Call the delegate supplied by the interface
                    If Me.m_syncObject IsNot Nothing Then
                        Me.m_syncObject.BeginInvoke(Me.m_RunProgressDelegate, New Object() {Me.m_comp.Iteration})
                    Else
                        Me.m_RunProgressDelegate.Invoke(Me.m_comp.Iteration)
                    End If
                End If

            Catch ex As Exception
                m_logger.LogError(ex, "cAdvectionManager.OnAdvectionCalcsProgressHandler Exception. iInteration:{iInteration}", iInteration)
            End Try

        End Sub

        Private Sub OnAdvectionCalcsCompletedHandler(iIteration As Integer, bInterrupted As Boolean, bBadAdvection As Boolean)

            Try
                Me.m_core.m_SearchData.SearchMode = eSearchModes.NotInSearch

                ' Release any waiting threads
                Me.ReleaseWait()

                'send any messages that the model added to the managers list of messages
                'by using the m_syncObject the messages will be sent on the Interfaces thread not the FPS thread
                If Me.m_syncObject IsNot Nothing Then
                    Dim ctd As CallingThreadDelegate = AddressOf Me.OnSendCoreMessages
                    Me.m_syncObject.BeginInvoke(ctd, Nothing)

                    ctd = AddressOf Me.OnChanged
                    Me.m_syncObject.BeginInvoke(ctd, Nothing)
                Else
                    Me.OnSendCoreMessages()
                    Me.OnChanged()
                End If

                If Me.m_RunCompletedDelegate IsNot Nothing Then
                    'call the delegate supplied by the interface
                    If Me.m_syncObject IsNot Nothing Then
                        Me.m_syncObject.BeginInvoke(Me.m_RunCompletedDelegate, New Object() {iIteration, bInterrupted, bBadAdvection})
                    Else
                        Me.m_RunCompletedDelegate(iIteration, bInterrupted, bBadAdvection)
                    End If
                End If

            Catch ex As Exception
                m_logger.LogError(ex, "cAdvectionManager.OnAdvectionCalcsCompletedHandler Exception. iIteration:{iIteration}, bInterrupted:{bInterrupted}, bBadAdvection:{bBadAdvection}", iIteration, bInterrupted, bBadAdvection)
                Me.m_core.m_SearchData.SearchMode = eSearchModes.NotInSearch
            End Try

        End Sub

        Private Sub OnAddMessageHandler(message As cMessage)
            'add the message to the managers list of mesasges
            'these messages will be sent at the end of the run
            Me.m_lstMessages.Add(message)

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
                m_logger.LogError(ex, "cAdvectionManager.OnSendCoreMessages Exception")
            End Try
        End Sub

        Private Sub OnChanged()
            Try
                Me.m_core.onChanged(Me)
            Catch ex As Exception
                'this should never happen!!!!! ehhhh
                m_logger.LogError(ex, "cAdvectionManager.OnChanged Exception")
            End Try
        End Sub



#End Region ' Events

#Region " ICoreInterface implementation "

        ''' -------------------------------------------------------------------
        ''' <inheritdoc cref="ICoreInterface.DataType"/>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property DataType() As eDataTypes _
            Implements ICoreInterface.DataType
            Get
                Return eDataTypes.EcospaceAdvectionManager
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <inheritdoc cref="ICoreInterface.CoreComponent"/>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property CoreComponent() As eCoreComponentType _
            Implements ICoreInterface.CoreComponent
            Get
                Return eCoreComponentType.Ecospace
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <inheritdoc cref="ICoreInterface.DBID"/>
        ''' -------------------------------------------------------------------
        Public Property DBID() As Integer _
            Implements ICoreInterface.DBID
            Get
                Return cCore.NULL_VALUE
            End Get
            Set(value As Integer)
                ' NOP
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <inheritdoc cref="ICoreInterface.GetID"/>
        ''' -------------------------------------------------------------------
        Public Function GetID() As String _
            Implements ICoreInterface.GetID
            Return Me.ToString
        End Function

        ''' -------------------------------------------------------------------
        ''' <inheritdoc cref="ICoreInterface.Index"/>
        ''' -------------------------------------------------------------------
        Public Property Index() As Integer _
            Implements ICoreInterface.Index
            Get
                Return cCore.NULL_VALUE
            End Get
            Set(value As Integer)
                ' NOP
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <inheritdoc cref="ICoreInterface.Name"/>
        ''' -------------------------------------------------------------------
        Public Property Name() As String _
            Implements ICoreInterface.Name
            Get
                Return Me.ToString
            End Get
            Set(value As String)
                ' NOP
            End Set
        End Property

#End Region ' ICoreInterface implementation


#Region "Code from the original advection model"

#If 0 Then 'Hide the old code behind compiler directives

        Private Sub RunThreaded_OldModel()

            Me.m_core.StateMonitor.SetIsSearching(eSearchModes.External)
            Me.m_core.SetStopRunDelegate(AddressOf Me.StopRun)

            Try
                Me.m_comp.Run()
            Catch ex As Exception
                m_logger.LogError(ex, "cAdvectionManager.RunThreaded")
            End Try

            Me.m_core.SetStopRunDelegate(Nothing)
            Me.m_core.StateMonitor.SetIsSearching(eSearchModes.NotInSearch)

        End Sub



        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Run the Advection computations.
        ''' </summary>
        ''' <param name="SyncObject"></param>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Public Function Run(SyncObject As System.ComponentModel.ISynchronizeInvoke) As Boolean

            ' Sanity check
            If (Me.m_core.StateMonitor.IsBusy) Then Return False

            Dim thrd As Thread = Nothing
            Dim bSuccess As Boolean = True

            Me.m_syncObject = SyncObject

            If Me.IsRunning Then
                Me.m_core.Messages.SendMessage(New cMessage(My.Resources.CoreMessages.COMPUTATION_ALREADY_RUNNING, _
                                                            eMessageType.ErrorEncountered, _
                                                            eCoreComponentType.EcoSpace, _
                                                            eMessageImportance.Warning, _
                                                            eDataTypes.MonteCarlo))
                Return False
            End If

            Me.SetWait()
            Try
                Me.Update()

                thrd = New Thread(AddressOf Me.RunThreaded_OldModel)
                thrd.Start()

            Catch ex As Exception
                m_logger.LogError(ex, "cAdvectionManager.Run(..) Exception")
                m_core.Messages.SendMessage(New cMessage(cStringUtils.Localize(My.Resources.CoreMessages.ADVECTION_ERROR, ex.Message), _
                                                         eMessageType.ErrorEncountered, _
                                                         eCoreComponentType.EcoSpace, _
                                                         eMessageImportance.Critical, _
                                                         eDataTypes.FishingPolicyManager))

                ' If an error has been thrown make sure the OnAdvectionCalcsCompletedHandler delegate is called
                ' This way an interface can respond
                Me.OnAdvectionCalcsCompletedHandler(Me.m_comp.Iteration, Me.m_comp.Interrupted, Me.m_comp.BadFlow)

                bSuccess = False
            End Try

            'send any messages generated from starting the search
            Me.OnSendCoreMessages()

            Return bSuccess

        End Function

#End If
#End Region

    End Class

End Namespace
