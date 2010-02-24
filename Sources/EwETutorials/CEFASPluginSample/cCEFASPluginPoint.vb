Imports EwEPlugin
Imports EwECore
Imports EwEUtils

Public Class cCEFASPluginPoint
    Implements EwEPlugin.IGUIPlugin
    Implements EwEPlugin.IMenuItemPlugin
    Implements EwEPlugin.IEcosimInitializedPlugin
    Implements EwEPlugin.IEcosimEndTimestepPlugin
    Implements EwEPlugin.IEcosimSubTimestepsPlugin
    Implements EwEPlugin.IEcosimRunCompletedPlugin

    ''' <summary>
    ''' Delegate for marshalling data onto the UI thread. Called by the monthly timestep plugin point.
    ''' </summary>
    ''' <param name="TimeStep"></param>
    ''' <remarks>UI elements must be called from the same thread as they are created on. Forms provide the .Invoke method that will marshall calls. </remarks>
    Delegate Sub MarshallOnMonthlyTimeStep(ByVal TimeStep As Integer)

    ''' <summary>
    ''' Delegate for marshalling data onto the UI thread. Called by the sub timestep plugin point.
    ''' </summary>
    ''' <param name="TimeInYears"></param>
    ''' <param name="DeltaT"></param>
    ''' <param name="SubTimestepIndex"></param>
    ''' <param name="EcosimDatastructures"></param>
    ''' <remarks>UI elements must be called from the same thread as they are created on. Forms provide the .Invoke method that will marshall calls. </remarks>
    Delegate Sub MarshallOnSubTimeStep(ByVal TimeInYears As Single, ByVal DeltaT As Single, ByVal SubTimestepIndex As Integer, ByVal EcosimDatastructures As Object)

    ''' <summary>
    '''  Delegate for marshalling calls onto the UI thread. Called by the IEcosimRunCompletedPlugin plugin point.
    ''' </summary>
    ''' <remarks></remarks>
    Delegate Sub MarshallOnRunCompleted()


#Region " Private Variables "
    Private m_core As EwECore.cCore
    Private m_bInitOK As Boolean

    Private m_CEFASForm As frmCEFASSample

    Private m_EcopathDs As cEcopathDataStructures
    Private m_EcosimDS As cEcosimDatastructures
    Private m_EcospaceDS As cEcospaceDataStructures

#End Region

#Region "Plugin Requirements"
    ''' <summary>
    ''' Initialize the Plugin. This is called when the core loads the Plugin. It will only be called once.
    ''' </summary>
    Public Sub Initialize(ByVal core As Object) Implements EwEPlugin.IPlugin.Initialize
        Debug.Assert(TypeOf core Is EwECore.cCore, Me.ToString & ".Initialize() argument core is not a cCore object.")
        m_bInitOK = False
        Try
            If TypeOf core Is EwECore.cCore Then
                m_core = DirectCast(core, EwECore.cCore)

                m_bInitOK = True
                System.Console.WriteLine(Me.ToString & ".Initialize() Successfull.")
            Else
                'some kind of a message
                System.Console.WriteLine(Me.ToString & ".Initialize() Failed.")
                Return
            End If
        Catch ex As Exception
            cLog.Write(ex)
            System.Console.WriteLine(Me.ToString & ".Initialize() Error: " & ex.Message)
            Debug.Assert(False, ex.Message)
            Return
        End Try
    End Sub

    ''' <summary>Generic <see cref="EwEPlugin.IPlugin.Name">IPlugin.Name</see> implementation.</summary>
    Public ReadOnly Property Name() As String Implements EwEPlugin.IPlugin.Name
        Get
            Return "Ecosim on thread Plugin Example"
        End Get
    End Property

    ''' <summary>Generic <see cref="EwEPlugin.IPlugin.Description">IPlugin.Description</see> implementation.</summary>
    Public ReadOnly Property Description() As String Implements EwEPlugin.IPlugin.Description
        Get
            Return "Run Ecosim in on a seperate thread."
        End Get
    End Property

    ''' <summary>Generic <see cref="EwEPlugin.IPlugin.Author">IPlugin.Author</see> implementation.</summary>
    Public ReadOnly Property Author() As String Implements EwEPlugin.IPlugin.Author
        Get
            Return "UBC Fisheries Centre"
        End Get
    End Property

    ''' <summary>Generic <see cref="EwEPlugin.IPlugin.Contact">IPlugin.Contact</see> implementation.</summary>
    Public ReadOnly Property Contact() As String Implements EwEPlugin.IPlugin.Contact
        Get
            Return "mailto:support@ecopath.org"
        End Get
    End Property

#Region " Navigation Requirements "
    Public ReadOnly Property ControlImage() As System.Drawing.Image Implements EwEPlugin.IGUIPlugin.ControlImage
        Get
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property ControlText() As String Implements EwEPlugin.IGUIPlugin.ControlText
        Get
            Return "Ecosim threaded plug-in"
        End Get
    End Property

    ''' <summary>What shows up when hovering over the menu.</summary>
    Public ReadOnly Property ControlTooltipText() As String Implements EwEPlugin.IGUIPlugin.ControlTooltipText
        Get
            Return "Ecosim on thread"
        End Get
    End Property

    ''' <summary>Tells the core to load itself to the specified state.
    ''' This is a short cut, but we currently use Idle and manually load the state.</summary>
    Public ReadOnly Property EnabledState() As EwEUtils.Core.eCoreExecutionState Implements EwEPlugin.IGUIPlugin.EnabledState
        Get
            Return EwEUtils.Core.eCoreExecutionState.Idle
        End Get
    End Property

    ''' <summary>Menu Item or Tree node clicked</summary>
    ''' <remarks>This will handle click events from all interface controls</remarks>
    Public Sub OnControlClick(ByVal sender As Object, ByVal e As System.EventArgs, ByRef frmPlugin As System.Windows.Forms.Form) Implements EwEPlugin.IGUIPlugin.OnControlClick
        ' Load the forms please

        ' Test if form still exists
        If Not Me.HasInterface(DirectCast(Me.m_CEFASForm, System.Windows.Forms.Form)) Then
            m_CEFASForm = New frmCEFASSample(Me)
        End If

        ' Pass form reference back to calling app
        frmPlugin = m_CEFASForm
    End Sub

    Private Function HasInterface(ByVal theForm As System.Windows.Forms.Form) As Boolean
        If theForm Is Nothing Then Return False
        If theForm.IsDisposed Then Return False
        Return True
    End Function


    Public ReadOnly Property MenuItemLocation() As String Implements EwEPlugin.IMenuItemPlugin.MenuItemLocation
        Get
            Return "MenuTools"
        End Get
    End Property

#End Region ' Navigation Requirements
#End Region ' Plugin Properties

#Region "Storage of Datastructures"

    ''' <summary>
    ''' Called by the core when Ecosim is initalized properly
    ''' </summary>
    Public Sub EcosimInitialized(ByVal EcosimDatastructures As Object) Implements EwEPlugin.IEcosimInitializedPlugin.EcosimInitialized

        Debug.Assert(TypeOf EcosimDatastructures Is EwECore.cEcosimDatastructures, Me.ToString & _
                            ".EcosimInitialized() argument EcosimDatastructures is not a cEcosimDatastructures object.")
        Try
            If TypeOf EcosimDatastructures Is EwECore.cEcosimDatastructures Then
                m_EcosimDS = DirectCast(EcosimDatastructures, cEcosimDatastructures)
                System.Console.WriteLine(Me.ToString & ".EcosimInitialized() Successfull.")
            Else
                Debug.Assert(False, "Accepted the wrong kind of Ecopath Datastructures")
            End If
        Catch ex As Exception
            Debug.Assert(False, ex.Message)
        End Try
    End Sub


#End Region

#Region "Timestep and RunCompleted plugin points"


    ''' <summary>
    ''' Plugin Point called when Ecosim has completed a time step
    ''' </summary>
    Private Sub EcosimEndTimeStep(ByRef BiomassAtTimestep() As Single, ByVal EcosimDatastructures As Object, ByVal iTime As Integer, ByVal Ecosimresults As Object) Implements EwEPlugin.IEcosimEndTimestepPlugin.EcosimEndTimeStep

        Try

            If Me.HasInterface(Me.m_CEFASForm) Then

                If Me.EcosimDS.bMultiThreaded Then
                    Me.m_CEFASForm.Invoke(New MarshallOnMonthlyTimeStep(AddressOf Me.m_CEFASForm.onEcosimMonthlyTimeStep), New Object() {iTime})
                Else
                    Me.m_CEFASForm.onEcosimMonthlyTimeStep(iTime)
                End If

            End If

        Catch ex As Exception
            System.Console.WriteLine("Exception in EcosimEndTimeStep " & ex.Message)
        End Try

    End Sub

    ''' <summary>
    ''' Plugin point that gets call when an Ecosim run has completed
    ''' </summary>
    ''' <param name="EcosimDatastructures"></param>
    ''' <remarks></remarks>
    Public Sub EcosimRunCompleted(ByVal EcosimDatastructures As Object) Implements EwEPlugin.IEcosimRunCompletedPlugin.EcosimRunCompleted
        Try

            If Me.HasInterface(Me.m_CEFASForm) Then
                'when the EcosimRunCompleted plugin point is called Ecosim will have reset bMultiThreaded=False and StepsPerMonth=1 (default values)
                Me.m_CEFASForm.Invoke(New MarshallOnRunCompleted(AddressOf Me.m_CEFASForm.onEcosimRunCompleted), Nothing)

            End If

        Catch ex As Exception
            System.Console.WriteLine("Exception in Ecosim EcosimRunCompleted " & ex.Message)
        End Try
    End Sub

#End Region

#Region "Properties"

    Friend ReadOnly Property Core() As cCore
        Get
            Return Me.m_core
        End Get
    End Property

#Region "Datastructures property"

    Public Property EcopathDS() As cEcopathDataStructures
        Get
            Return Me.m_EcopathDs
        End Get
        Set(ByVal value As cEcopathDataStructures)
            Me.m_EcopathDs = value
        End Set
    End Property

    ''' <summary>Direct access to the core Ecosim values</summary>
    Public Property EcosimDS() As cEcosimDatastructures
        Get
            Return Me.m_EcosimDS
        End Get
        Set(ByVal value As cEcosimDatastructures)
            Me.m_EcosimDS = value
        End Set
    End Property

    ''' <summary>Direct access to the core Ecospace values</summary>
    Public Property EcospaceDS() As cEcospaceDataStructures
        Get
            Return Me.m_EcospaceDS
        End Get
        Set(ByVal value As cEcospaceDataStructures)
            Me.m_EcospaceDS = value
        End Set
    End Property
#End Region

#End Region

#Region "Sub Timestep Plugin points"
    ''' <summary>
    ''' Plugin point call at the start of an Ecosim sub timestep
    ''' </summary>
    ''' <param name="BiomassAtTimestep">Biomass array alteration to biomass values will be used for the current timestep </param>
    ''' <param name="TimeInYears">Time of the timestep in years</param>
    ''' <param name="DeltaT">Length of the timestep in years</param>
    ''' <param name="SubTimestepIndex">Index of the sub timestep</param>
    ''' <param name="EcosimDatastructures">EcosimDatastructures object </param>
    ''' <remarks>Any changes made to BiomassAtTimestep() or EcosimDatastructures will be used by Ecosim during the timestep</remarks>
    Public Sub EcosimSubTimeStepBegin(ByRef BiomassAtTimestep() As Single, ByVal TimeInYears As Single, ByVal DeltaT As Single, ByVal SubTimestepIndex As Integer, ByVal EcosimDatastructures As Object) Implements EwEPlugin.IEcosimSubTimestepsPlugin.EcosimSubTimeStepBegin

    End Sub

    ''' <summary>
    ''' Plugin point call at the end of an Ecosim sub timestep
    ''' </summary>
    ''' <param name="BiomassAtTimestep">Biomass array contains the results of the sub timestep </param>
    ''' <param name="TimeInYears">Time of the timestep in years</param>
    ''' <param name="DeltaT">Length of the timestep in years</param>
    ''' <param name="SubTimestepIndex">Index of the sub timestep</param>
    ''' <param name="EcosimDatastructures">EcosimDatastructures object </param>
    ''' <remarks>
    ''' BiomassAtTimestep() and EcosimDatastructures will contain the results of the sub timestep. 
    ''' cCore interface objects will not be updated until all the sub timesteps have been completed and EwEPlugin.IEcosimEndTimestepPlugin.EcosimEndTimeStep plugin point is called.
    ''' </remarks>
    Public Sub EcosimSubTimeStepEnd(ByRef BiomassAtTimestep() As Single, ByVal TimeInYears As Single, ByVal DeltaT As Single, ByVal SubTimestepIndex As Integer, ByVal EcosimDatastructures As Object) Implements EwEPlugin.IEcosimSubTimestepsPlugin.EcosimSubTimeStepEnd
        Try

            If Me.HasInterface(Me.m_CEFASForm) Then

                If Me.EcosimDS.bMultiThreaded Then
                    Me.m_CEFASForm.Invoke(New MarshallOnSubTimeStep(AddressOf Me.m_CEFASForm.onEcosimSubTimestep), New Object() {TimeInYears, DeltaT, SubTimestepIndex, EcosimDatastructures})
                Else
                    Me.m_CEFASForm.onEcosimSubTimestep(TimeInYears, DeltaT, SubTimestepIndex, EcosimDatastructures)
                End If

            End If

        Catch ex As Exception
            System.Console.WriteLine("Exception in Ecosim Multithreaded sample " & ex.Message)
        End Try
    End Sub

#End Region

End Class