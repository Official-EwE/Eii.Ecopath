Imports EwEPlugin
Imports EwECore
Imports EwEUtils

Public Class cCEFASPluginPoint
    Implements EwEPlugin.IGUIPlugin
    Implements EwEPlugin.IMenuItemPlugin
    Implements EwEPlugin.ICorePlugin
    Implements EwEPlugin.IEcosimEndTimestepPlugin
    Implements EwEPlugin.IEcosimSubTimestepsPlugin
    Implements EwEPlugin.IEcosimRunCompletedPlugin
    Implements EwEPlugin.IEcosimRunInitializedPlugin

#Region " Private Variables "
    Private m_core As EwECore.cCore
    Private m_bInitOK As Boolean

    Private m_CEFASForm As frmCEFASSample

    Private m_EcopathDS As cEcopathDataStructures
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
            Return "CEFAS sample plug-in"
        End Get
    End Property

    ''' <summary>Generic <see cref="EwEPlugin.IPlugin.Description">IPlugin.Description</see> implementation.</summary>
    Public ReadOnly Property Description() As String Implements EwEPlugin.IPlugin.Description
        Get
            Return "Run Ecosim thread and variable time step"
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
            Return "CEFAS plug-in sample"
        End Get
    End Property

    ''' <summary>What shows up when hovering over the menu.</summary>
    Public ReadOnly Property ControlTooltipText() As String Implements EwEPlugin.IGUIPlugin.ControlTooltipText
        Get
            Return "Multi thread and variable time step sample"
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

    Public Sub CoreInitialized(ByRef objEcoPath As Object, ByRef objEcoSim As Object, ByRef objEcoSpace As Object) Implements EwEPlugin.ICorePlugin.CoreInitialized

        Debug.Assert(TypeOf objEcoSim Is Ecosim.cEcoSimModel, Me.ToString & _
                    ".CoreInitialized() argument objEcoSim is not a cEcoSimModel object.")

        Debug.Assert(TypeOf objEcoPath Is Ecopath.cEcoPathModel, Me.ToString & _
            ".CoreInitialized() argument objEcoPath is not a cEcoPathModel object.")

        Try
            'get the Ecosim and Ecopath data from the Models
            m_EcosimDS = DirectCast(objEcoSim, Ecosim.cEcoSimModel).EcosimData
            m_EcopathDS = DirectCast(objEcoPath, Ecopath.cEcoPathModel).EcopathData

        Catch ex As Exception
            Debug.Assert(False, ex.Message)
        End Try

    End Sub

#End Region

#Region "Timestep and RunCompleted plugin points"

    ''' <summary>
    ''' Plugin Point called when Ecosim has started a new run
    ''' </summary>
    ''' <param name="EcosimDatastructures"></param>
    ''' <remarks></remarks>
    Public Sub EcosimRunInitialized(ByVal EcosimDatastructures As Object) Implements EwEPlugin.IEcosimRunInitializedPlugin.EcosimRunInitialized
        'tell the interface a new run has started
        If Me.HasInterface(Me.m_CEFASForm) Then
            Me.m_CEFASForm.onEcosimRunStarted()
        End If
    End Sub


    ''' <summary>
    ''' Plugin Point called when Ecosim has completed a monthly time step
    ''' </summary>
    Private Sub EcosimEndTimeStep(ByRef BiomassAtTimestep() As Single, ByVal EcosimDatastructures As Object, ByVal iTime As Integer, ByVal Ecosimresults As Object) Implements EwEPlugin.IEcosimEndTimestepPlugin.EcosimEndTimeStep

        Try

            If Me.HasInterface(Me.m_CEFASForm) Then

                Me.m_CEFASForm.onEcosimMonthlyTimeStep(iTime)

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

                Me.m_CEFASForm.onEcosimRunCompleted()

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
            Return Me.m_EcopathDS
        End Get
        Set(ByVal value As cEcopathDataStructures)
            Me.m_EcopathDS = value
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

        Try
            Dim simdata As cEcosimDatastructures = DirectCast(EcosimDatastructures, cEcosimDatastructures)
            'Any changes made to BiomassAtTimestep(ngroups) will be use for the next times step
            For igrp As Integer = 1 To Me.EcosimDS.nGroups
                If Me.EcopathDS.PP(igrp) = 1 Then
                    'increase biomass of primary producers by some fixed amount per year
                    BiomassAtTimestep(igrp) = Me.EcopathDS.B(igrp) + Me.EcopathDS.B(igrp) * 0.1F * (TimeInYears + 1)
                End If
            Next

        Catch ex As Exception

        End Try

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

                Me.m_CEFASForm.onEcosimSubTimestep(TimeInYears, DeltaT, SubTimestepIndex, EcosimDatastructures)

            End If

        Catch ex As Exception
            System.Console.WriteLine("Exception in Ecosim Multithreaded sample " & ex.Message)
        End Try
    End Sub

#End Region


End Class