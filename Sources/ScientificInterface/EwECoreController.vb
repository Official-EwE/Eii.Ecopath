'==============================================================================
'
' $Log: EwECoreController.vb,v $
' Revision 1.3  2008/11/20 18:42:17  jeroens
' CoreController uses cCoreStateManager
'
' Revision 1.2  2008/11/18 16:35:35  jeroens
' Left ToDo
'
' Revision 1.1  2008/09/26 07:31:24  sherman
' --== DELETED HISTORY ==--
'
'==============================================================================

#Region " Imports directive "

Option Explicit On
Option Strict On

Imports EwECore
Imports EwEUtils.Core
Imports ScientificInterface.Ecosim
Imports ScientificInterface.Ecospace

#End Region ' Imports directive

''' ---------------------------------------------------------------------------
''' <summary>
''' Helper class that tries to set the EwE core in a particular running state.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class EwECoreController

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

        ' State already superceded or active?
        If (Me.m_monitor.IsExecutionStateSuperceded(iState)) And (bForceState = False) Then
            Return True
        End If

        Select Case iState

            Case eCoreExecutionState.EcopathLoaded
                Return TryLoadEcopathModel()

            Case eCoreExecutionState.EcopathCompleted
                Return TryCompleteEcopath()

            Case eCoreExecutionState.EcosimLoaded
                Return TryLoadEcosimScenario(False)

            Case eCoreExecutionState.EcosimCompleted
                Return TryCompleteEcosim()

            Case eCoreExecutionState.EcotracerLoaded
                Return TryLoadEcotracerScenario(False)

            Case eCoreExecutionState.EcospaceLoaded
                Return TryLoadEcospaceScenario(False)

        End Select

        Return False
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

    Public Function LoadPersistState(ByVal iState As eCoreExecutionState, _
            Optional ByVal bForceState As Boolean = False) As Boolean

        ' State already superceded or active?
        If (Me.m_monitor.IsExecutionStateSuperceded(iState)) And (bForceState = False) Then
            Return True
        End If

        Select Case iState

            Case eCoreExecutionState.EcopathLoaded
                Return TryLoadEcopathModel()

            Case eCoreExecutionState.EcopathCompleted
                Return TryCompleteEcopath()

            Case eCoreExecutionState.EcosimLoaded
                Return TryLoadEcosimScenario(True)

            Case eCoreExecutionState.EcosimInitialized
                Return TryInitializeEcosim()

            Case eCoreExecutionState.EcosimCompleted
                Return TryCompleteEcosim()

            Case eCoreExecutionState.EcospaceLoaded
                Return TryLoadEcospaceScenario(True)

            Case eCoreExecutionState.EcotracerLoaded
                Return TryLoadEcotracerScenario(True)

        End Select

        Return False

    End Function

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
    Private Function TryLoadEcosimScenario(ByVal bPersist As Boolean) As Boolean

        Dim bSuccess As Boolean = False
        Dim appl As AppLauncher = AppLauncher.GetInstance()

        'FG: Bug fix June 14, 2007
        If bPersist Then
            If Me.LoadPersistState(eCoreExecutionState.EcopathCompleted) Then
                ' Let AppLauncher perform the load as it sees fit
                bSuccess = appl.LoadEcosimScenario(bPersist, True)
            End If
        Else
            If Me.LoadState(eCoreExecutionState.EcopathCompleted) Then
                ' Let AppLauncher perform the load as it sees fit
                bSuccess = appl.LoadEcosimScenario(bPersist, True)
            End If
        End If
        ' Ecopath has completed?

        Return bSuccess
    End Function

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Attempt to load an Ecospace scenario.
    ''' </summary>
    ''' <returns>True if succesful.</returns>
    ''' ---------------------------------------------------------------------------
    Private Function TryLoadEcospaceScenario(ByVal bPersist As Boolean) As Boolean

        Dim bSuccess As Boolean = False
        Dim appl As AppLauncher = AppLauncher.GetInstance()

        'FG: Bug fix June 14, 2007
        If bPersist Then
            ' JS 07mar07: Ecosim model needs to be loaded, not run, for an ecospace model to load.
            If Me.LoadPersistState(eCoreExecutionState.EcosimInitialized) Then
                ' Let AppLauncher perform the load as it sees fit
                bSuccess = appl.LoadEcospaceScenario(bPersist, True)
            End If
        Else
            ' JS 07mar07: Ecosim model needs to be loaded, not run, for an ecospace model to load.
            If Me.LoadState(eCoreExecutionState.EcosimInitialized) Then
                ' Let AppLauncher perform the load as it sees fit
                bSuccess = appl.LoadEcospaceScenario(bPersist, True)
            End If
        End If

        Return bSuccess
    End Function


    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Attempt to load an Ecotracer scenario.
    ''' </summary>
    ''' <returns>True if succesful.</returns>
    ''' ---------------------------------------------------------------------------
    Private Function TryLoadEcotracerScenario(ByVal bPersist As Boolean) As Boolean

        Dim bSuccess As Boolean = False
        Dim appl As AppLauncher = AppLauncher.GetInstance()

        If bPersist Then
            ' JS 07mar07: Ecosim model needs to be loaded, not run, for an ecotracer model to load.
            If Me.LoadPersistState(eCoreExecutionState.EcosimLoaded) Then
                ' Let AppLauncher perform the load as it sees fit
                bSuccess = appl.LoadEcotracerScenario(bPersist, True)
            End If
        Else
            ' JS 07mar07: Ecosim model needs to be loaded, not run, for an ecotracer model to load.
            If Me.LoadState(eCoreExecutionState.EcosimLoaded) Then
                ' Let AppLauncher perform the load as it sees fit
                bSuccess = appl.LoadEcotracerScenario(bPersist, True)
            End If
        End If

        Return bSuccess
    End Function

#End Region ' Private members 

End Class
