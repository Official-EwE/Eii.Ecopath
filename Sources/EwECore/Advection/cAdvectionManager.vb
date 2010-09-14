Option Strict On
Imports EwECore.Ecosim
Imports System.Threading
Imports EwEUtils.Core

Namespace Ecospace.Advection

    Public Class cAdvectionManager
        Inherits cThreadWaitBase 'provides the Wait() method
        Implements ICoreInterface

        Friend Delegate Sub EcospaceAdvectionAddMessageHandler(ByVal message As cMessage)
        Public Delegate Sub EcoSpaceAdvectionStartedDelegate()
        Public Delegate Sub EcoSpaceAdvectionProgressDelegate(ByVal iIteration As Integer)
        Public Delegate Sub EcoSpaceAdvectionCompletedDelegate(ByVal iIteration As Integer, ByVal bInterrupted As Boolean, ByVal bBadAdvection As Boolean)

#Region "Private Variables"

        Private m_comp As cAdvection = Nothing
        Private m_core As cCore = Nothing
        Private m_parameters As cAdvectionParameters = Nothing
        Private m_data As cEcospaceDataStructures = Nothing
        Private m_lstMessages As New List(Of cMessage)

        Private m_syncObject As System.ComponentModel.ISynchronizeInvoke
        Private m_RunStartedDelegate As EcoSpaceAdvectionStartedDelegate
        Private m_RunProgressDelegate As EcoSpaceAdvectionProgressDelegate
        Private m_RunCompletedDelegate As EcoSpaceAdvectionCompletedDelegate

        Private Delegate Sub CallingThreadDelegate()

#End Region

#Region "Construction and Initialization"

        ''' <summary>
        ''' Secretive constructor.
        ''' </summary>
        Friend Sub New()

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Connect an interface to the Advection calculations
        ''' </summary>
        ''' <param name="RunStartedCallBack">Callback a search run is about to start. If ModelParameters.nRun > 1 this will be call at the start of each run.</param>
        ''' <param name="RunCompletedBack">Callback a search run has completed. If ModelParameters.nRun > 1 this will be call at the end of each run.</param>
        ''' <param name="ProgressCallBack">Callback reports progress of the search</param>
        ''' -------------------------------------------------------------------
        Public Sub Connect(ByVal RunStartedCallBack As EcoSpaceAdvectionStartedDelegate, _
                           ByVal RunCompletedBack As EcoSpaceAdvectionCompletedDelegate, _
                           ByVal ProgressCallBack As EcoSpaceAdvectionProgressDelegate)

            Me.m_RunStartedDelegate = RunStartedCallBack
            Me.m_RunCompletedDelegate = RunCompletedBack
            Me.m_RunProgressDelegate = ProgressCallBack

        End Sub

        Public Sub Disconnect()

            Me.m_comp.Interrupted = True
            Me.m_RunStartedDelegate = Nothing
            Me.m_RunProgressDelegate = Nothing
            Me.m_RunCompletedDelegate = Nothing

        End Sub

        ''' <summary>
        ''' Build interface objects
        ''' </summary>
        Friend Function Init(ByVal theCore As cCore, ByVal theEcospace As cEcoSpace) As Boolean
            Try

                Me.m_core = theCore

                'init the Fihsing Policy Search model
                m_comp = New cAdvection()
                m_comp.Init(theCore, theEcospace)
                m_comp.AddMessageCallback = AddressOf OnAddMessageHandler
                m_comp.ProgressCallback = AddressOf OnAdvectionCalcsProgressHandler
                m_comp.RunStartedCallBack = AddressOf OnAdvectionCalcsStartedHandler
                m_comp.RunCompletedCallback = AddressOf OnAdvectionCalcsCompletedHandler

                'get the data from the core
                m_data = m_core.m_EcoSpaceData
                m_parameters = m_core.AdvectionParameters

                Return True

            Catch ex As Exception
                cLog.Write(ex)
                Return False
            End Try

        End Function

        ''' <summary>
        ''' Load data into existing interface objects
        ''' </summary>
        ''' <returns>True if successful.</returns>
        Friend Function Load() As Boolean

            Try
                m_parameters.AllowValidation = False

                m_parameters.Coriolis = Me.m_data.Coriolis
                m_parameters.XVelocity = Me.m_data.XVelocity
                m_parameters.YVelocity = Me.m_data.YVelocity

                m_parameters.ResetStatusFlags()
                m_parameters.AllowValidation = True
                Return True
            Catch ex As Exception
                Return False
            End Try

        End Function

        ''' <summary>
        ''' Update the underlying data with values from the interface
        ''' </summary>
        ''' <returns>True if successful.</returns>
        Public Function Update() As Boolean

            Me.m_data.Coriolis = Me.m_parameters.Coriolis
            Me.m_data.XVelocity = Me.m_parameters.XVelocity
            Me.m_data.YVelocity = Me.m_parameters.YVelocity

            Return True

        End Function

#End Region

#Region "Public Properties"

        ''' <summary>
        ''' Get configurable advection parameters.
        ''' </summary>
        Public ReadOnly Property ModelParameters() As cAdvectionParameters
            Get
                Return m_parameters
            End Get
        End Property

        ''' <summary>
        ''' Count of the Advection calculations run
        ''' </summary>
        ''' <remarks>if isRunning = True then iRun will be the count of the current run out of ModelParameters.nRuns</remarks>
        Public ReadOnly Property Iteration() As Integer
            Get
                Return Me.m_comp.Iteration
            End Get
        End Property

        ''' <summary>
        ''' Stop the Advection calculations run
        ''' </summary>
        ''' <remarks>This will not do anything if the search is not running</remarks>
        Public Sub StopRun()
            Me.m_comp.Interrupted = True
        End Sub

#End Region

#Region "private handlers for search callbacks/delegates"

        Private Sub OnAdvectionCalcsStartedHandler()
            Dim ctd As CallingThreadDelegate = Nothing

            Try

                If m_RunStartedDelegate IsNot Nothing Then
                    'call the delegate supplied by the interface
                    m_syncObject.BeginInvoke(Me.m_RunStartedDelegate, Nothing)
                End If

            Catch ex As Exception
                cLog.Write(ex)
            End Try

        End Sub

        Private Sub OnAdvectionCalcsCompletedHandler(ByVal iIteration As Integer, ByVal bInterrupted As Boolean, ByVal bBadAdvection As Boolean)

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

                If Me.m_RunCompletedDelegate IsNot Nothing Then
                    'call the delegate supplied by the interface
                    m_syncObject.BeginInvoke(m_RunCompletedDelegate, New Object() {iIteration, bInterrupted, bBadAdvection})
                End If

            Catch ex As Exception
                cLog.Write(ex)
                m_core.m_SearchData.SearchMode = eSearchModes.NotInSearch
                Me.ReleaseWait()
            End Try

        End Sub


        Private Sub OnAdvectionCalcsProgressHandler(ByVal iInteration As Integer)

            Try

                If m_RunProgressDelegate IsNot Nothing Then
                    'call the delegate supplied by the interface
                    m_syncObject.BeginInvoke(Me.m_RunProgressDelegate, New Object() {Me.m_comp.Iteration})
                End If

            Catch ex As Exception
                cLog.Write(ex)
            End Try

        End Sub


        Private Sub OnAddMessageHandler(ByVal message As cMessage)
            'add the message to the managers list of mesasges
            'these messages will be sent at the end of the run
            m_lstMessages.Add(message)

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
            Dim thrd As Thread
            Dim bsuccess As Boolean

            Try

                If Me.IsRunning Then
                    m_core.Messages.SendMessage(New cMessage("An advection computation is already running. Only one search can be run at a time.", eMessageType.ErrorEncountered, _
                                                eCoreComponentType.FishingPolicySearch, eMessageImportance.Critical, eDataTypes.MonteCarlo))
                    Return False
                End If

                bsuccess = True

                Me.setWait()

                Me.Update()

                thrd = New Thread(AddressOf Me.m_comp.Run)
                thrd.Start()

            Catch ex As Exception
                cLog.Write(ex)
                'unblock the thread before doing anything incase something has called Wait()

                m_core.Messages.SendMessage(New cMessage("Error running the Advection calculations.", eMessageType.ErrorEncountered, _
                                            eCoreComponentType.FishingPolicySearch, eMessageImportance.Critical, eDataTypes.FishingPolicyManager))

                'if an error has been thrown make sure the SearchCompletedCallBack delegate is called
                'this way an interface can responded 
                Me.OnAdvectionCalcsCompletedHandler(Me.m_comp.Iteration, Me.m_comp.Interrupted, Me.m_comp.BadFlow)

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
                Return eDataTypes.EcospaceAdvectionManager
            End Get
        End Property

        Public ReadOnly Property CoreComponent() As eCoreComponentType Implements ICoreInterface.CoreComponent
            Get
                Return eCoreComponentType.EcoSpace
            End Get
        End Property

        Public Property DBID() As Integer Implements ICoreInterface.DBID
            Get
                Return cCore.NULL_VALUE
            End Get
            Set(ByVal value As Integer)
                ' NOP
            End Set
        End Property

        Public Function GetID() As String Implements ICoreInterface.GetID
            Return Me.ToString
        End Function

        Public Property Index() As Integer Implements ICoreInterface.Index
            Get
                Return cCore.NULL_VALUE
            End Get
            Set(ByVal value As Integer)
                ' NOP
            End Set
        End Property

        Public Property Name() As String Implements ICoreInterface.Name
            Get
                Return Me.ToString
            End Get
            Set(ByVal value As String)
                ' NOP
            End Set
        End Property

#End Region

    End Class

End Namespace
