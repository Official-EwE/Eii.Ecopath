#Region " Imports "

Option Explicit On
Option Strict On

Imports EwECore
Imports EwEUtils.Core
Imports ScientificInterface.Ecosim
Imports ScientificInterface.Ecospace

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Helper class that tries to make sure the interface correctly loads a particular
''' scenario.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cCoreController

#Region " Private vars "

    ''' <summary>Core state monitor to query.</summary>
    Private m_monitor As cCoreStateMonitor = Nothing
    ''' <summary>Manager to use for bringing the core up to date.</summary>
    Private m_manager As cCoreStateManager = Nothing

#End Region ' Private vars

#Region " Public access "

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Constructor, initializes a new instance of a EwECoreController.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Sub New(ByVal monitor As cCoreStateMonitor, ByVal manager As cCoreStateManager)
        Me.m_manager = manager
        Me.m_monitor = monitor
    End Sub

    Private m_bInUpdate As Boolean = False

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Check whether the EwE Core running state matches a given state, and if
    ''' this is not the case, try to bring the Core up to par.
    ''' </summary>
    ''' <param name="iState">The <see cref="eCoreExecutionState">Core execution state</see>
    ''' to check.</param>
    ''' <param name="bForceState">Tells this method to force loading the state, 
    ''' regardless what state the EwE6 core is at. Handle this parameter with 
    ''' care because recklessly overriding core states may have unpredictable 
    ''' results.</param>
    ''' <returns>True if succesful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function LoadState(ByVal iState As eCoreExecutionState, _
            Optional ByVal bForceState As Boolean = False) As Boolean

        Dim bSucces As Boolean = True

        ' State already superceded or active?
        If (Me.m_monitor.IsExecutionStateSuperceded(iState)) And (bForceState = False) Then
            Return bSucces
        End If

        If Me.m_bInUpdate Then Return bSucces
        Me.m_bInUpdate = True

        Select Case iState

            Case eCoreExecutionState.EcopathLoaded, _
                 eCoreExecutionState.EcopathInitialized
                bSucces = TryLoadEcopathModel()

            Case eCoreExecutionState.EcopathCompleted
                bSucces = TryCompleteEcopath()

            Case eCoreExecutionState.EcosimLoaded
                bSucces = TryLoadEcosimScenario()

            Case eCoreExecutionState.EcosimInitialized
                bSucces = TryInitializeEcosim()

            Case eCoreExecutionState.EcosimCompleted
                bSucces = TryCompleteEcosim()

            Case eCoreExecutionState.EcotracerLoaded
                bSucces = TryLoadEcotracerScenario()

            Case eCoreExecutionState.EcospaceLoaded, _
                 eCoreExecutionState.EcospaceInitialized
                bSucces = TryLoadEcospaceScenario()

            Case Else
                bSucces = False

        End Select

        Me.m_bInUpdate = False

        Return bSucces

    End Function

    Public Sub LoadEcosimScenario()
        ' Force this core state
        LoadState(eCoreExecutionState.EcosimLoaded, True)
    End Sub

    Public Sub LoadEcospaceScenario()
        ' Force this core state
        LoadState(eCoreExecutionState.EcospaceLoaded, True)
    End Sub

    Public Sub LoadEcotracerScenario()
        ' Force this core state
        LoadState(eCoreExecutionState.EcotracerLoaded, True)
    End Sub

#End Region ' Public access

#Region " Private members "

    ' TODO_JS: nov18o8 Use core mechanism to auto-update to desired run state. Do not run core models from this class, because plug-ins will need this too
    ' TODO_JB: nov18o8 Buid core mechanism to auto-update to desired run state

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Attempt to load an Ecopath model.
    ''' </summary>
    ''' <returns>True if succesful.</returns>
    ''' ---------------------------------------------------------------------------
    Private Function TryLoadEcopathModel() As Boolean
        ' The navigation tree is only visible when an Ecopath model is loaded.
        ' This is an assumption that may get us into trouble if the form behaviour
        ' were to change.
        Return True
    End Function

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Attempt get Ecopath to produce outputs.
    ''' </summary>
    ''' <returns>True if succesful.</returns>
    ''' ---------------------------------------------------------------------------
    Private Function TryCompleteEcopath() As Boolean

        ' Is ecopath model loaded?
        If LoadState(eCoreExecutionState.EcopathLoaded) Then
            ' #Yes: get ecopath up to par
            Return Me.m_manager.LoadState(eCoreExecutionState.EcopathCompleted)
        End If

        Return False
    End Function

    Private Function TryInitializeEcosim() As Boolean

        ' Is Ecosim scenario loaded?
        If LoadState(eCoreExecutionState.EcosimLoaded) Then
            Return Me.m_manager.LoadState(eCoreExecutionState.EcosimInitialized)
        End If

        Return False

    End Function

    Private Function TryCompleteEcosim() As Boolean

        ' Is Ecosim scenario loaded?
        If LoadState(eCoreExecutionState.EcosimLoaded) Then
            Return Me.m_manager.LoadState(eCoreExecutionState.EcosimCompleted)
        End If

        Return False

    End Function

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Attempt to load an Ecosim scenario.
    ''' </summary>
    ''' <returns>True if succesful.</returns>
    ''' ---------------------------------------------------------------------------
    Private Function TryLoadEcosimScenario() As Boolean

        Dim bSuccess As Boolean = False
        Dim appl As AppLauncher = AppLauncher.GetInstance()

        If Me.LoadState(eCoreExecutionState.EcopathCompleted) Then
            ' Let AppLauncher perform the load as it sees fit
            bSuccess = appl.LoadEcosimScenario(True)
        End If

        Return bSuccess
    End Function

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Attempt to load an Ecospace scenario.
    ''' </summary>
    ''' <returns>True if succesful.</returns>
    ''' ---------------------------------------------------------------------------
    Private Function TryLoadEcospaceScenario() As Boolean

        Dim bSuccess As Boolean = False
        Dim appl As AppLauncher = AppLauncher.GetInstance()

        ' JS 07mar07: Ecosim model needs to be loaded, not run, for an ecospace model to load.
        If LoadState(eCoreExecutionState.EcosimInitialized) Then
            ' Let AppLauncher perform the load as it sees fit
            bSuccess = appl.LoadEcospaceScenario(True)
        End If
 
        Return bSuccess
    End Function


    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Attempt to load an Ecotracer scenario.
    ''' </summary>
    ''' <returns>True if succesful.</returns>
    ''' ---------------------------------------------------------------------------
    Private Function TryLoadEcotracerScenario() As Boolean

        Dim bSuccess As Boolean = False
        Dim appl As AppLauncher = AppLauncher.GetInstance()

        ' JS 07mar07: Ecosim model needs to be loaded, not run, for an ecotracer model to load.
        If Me.LoadState(eCoreExecutionState.EcosimLoaded) Then
            ' Let AppLauncher perform the load as it sees fit
            bSuccess = appl.LoadEcotracerScenario(True)
        End If

        Return bSuccess
    End Function

#End Region ' Private members 

End Class
