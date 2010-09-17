#Region " Imports "

Option Strict On
Imports EwECore.Ecosim
Imports System.Threading
Imports EwEUtils.Core

#End Region ' Imports

Namespace Ecospace.Advection

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
    ''' <para>Advection computations are started via <see cref="cAdvectionManager.Run">Run</see>.
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
        Public Delegate Sub ComputationProgressDelegate(ByVal iIteration As Integer)

        ''' -------------------------------------------------------------------
        ''' <summary>Delegate that will be called when advection computations have finished.</summary>
        ''' <param name="iIteration">The number of completed iterations.</param>
        ''' <param name="bInterrupted">Flag stating whether the iterations were interrupted by the user.</param>
        ''' <param name="bBadFlow">Flag stating whether the computed flow was considered 'bad'.</param>
        ''' -------------------------------------------------------------------
        Public Delegate Sub ComputationCompletedDelegate(ByVal iIteration As Integer, ByVal bInterrupted As Boolean, ByVal bBadFlow As Boolean)

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
        Public Sub Connect(ByVal ComputationStartedCallBack As ComputationStartedDelegate, _
                           ByVal ComputationCompletedBack As ComputationCompletedDelegate, _
                           ByVal ComputationProgressCallBack As ComputationProgressDelegate)

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
        Friend Function Init(ByVal theCore As cCore, ByVal theEcospace As cEcoSpace) As Boolean
            Try

                Me.m_core = theCore

                'init the Fihsing Policy Search model
                m_comp = New cAdvection()
                m_comp.Init(theCore, theEcospace)
                'm_comp.AddMessageCallback = AddressOf OnAddMessageHandler
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

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Load data into existing interface objects
        ''' </summary>
        ''' <returns>True if successful.</returns>
        ''' -------------------------------------------------------------------
        Friend Function Load() As Boolean

            Try
                m_parameters.AllowValidation = False

                m_parameters.XVelocity = Me.m_data.XVelocity
                m_parameters.YVelocity = Me.m_data.YVelocity
                m_parameters.Coriolis = Me.m_data.Coriolis
                m_parameters.SorWv = Me.m_data.SorWv

                m_parameters.ResetStatusFlags()
                m_parameters.AllowValidation = True
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

            Me.m_data.XVelocity = Me.m_parameters.XVelocity
            Me.m_data.YVelocity = Me.m_parameters.YVelocity
            Me.m_data.Coriolis = Me.m_parameters.Coriolis
            Me.m_data.SorWv = Me.m_parameters.SorWv

            Return True

        End Function

#End Region '  Construction and Initialization

#Region " Public Properties "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get configurable advection parameters.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property ModelParameters() As cAdvectionParameters
            Get
                Return m_parameters
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

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Stop the Advection calculations run.
        ''' </summary>
        ''' <remarks>This will not do anything if the search is not running</remarks>
        ''' -------------------------------------------------------------------
        Public Sub StopRun()
            Me.m_comp.Interrupted = True
        End Sub

#End Region ' Public Properties

#Region " Running "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Run the Advection computations.
        ''' </summary>
        ''' <param name="SyncObject"></param>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Public Function Run(ByVal SyncObject As System.ComponentModel.ISynchronizeInvoke) As Boolean

            Dim thrd As Thread = Nothing
            Dim bSuccess As Boolean = True

            Me.m_syncObject = SyncObject


            If Me.IsRunning Then
                Me.m_core.Messages.SendMessage(New cMessage(My.Resources.CoreMessages.ADVECTION_ALREADY_RUNNING, _
                                                            eMessageType.ErrorEncountered, _
                                                            eCoreComponentType.EcoSpace, _
                                                            eMessageImportance.Critical, _
                                                            eDataTypes.MonteCarlo))
                Return False
            End If

            Me.SetWait()
            Try
                Me.Update()

                thrd = New Thread(AddressOf Me.m_comp.Run)
                thrd.Start()

            Catch ex As Exception
                cLog.Write(ex)
                m_core.Messages.SendMessage(New cMessage(String.Format(My.Resources.CoreMessages.ADVECTION_ERROR, ex.Message), _
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

#End Region ' Running

#Region " Events "

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

                ' Release any waiting threads
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
            End Try

        End Sub

        Private Sub OnAdvectionCalcsProgressHandler(ByVal iInteration As Integer)

            Try
                Dim layer As cEcospaceLayer = Me.m_core.EcospaceBasemap.LayerAdvection

                If m_RunProgressDelegate IsNot Nothing Then
                    ' Invalidate layer
                    layer.Invalidate()
                    ' Call the delegate supplied by the interface
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
                Return eCoreComponentType.EcoSpace
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
            Set(ByVal value As Integer)
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
            Set(ByVal value As Integer)
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
            Set(ByVal value As String)
                ' NOP
            End Set
        End Property

#End Region ' ICoreInterface implementation

    End Class

End Namespace
