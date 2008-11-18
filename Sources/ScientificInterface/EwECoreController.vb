'==============================================================================
'
' $Log: EwECoreController.vb,v $
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

#Region " Public access "

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Constructor, initializes a new instance of a EwECoreController.
    ''' </summary>
    ''' <param name="core">The instance of the core to control.</param>
    ''' ---------------------------------------------------------------------------
    Public Sub New(ByRef core As cCore, ByRef appl As AppLauncher)
        ' Remember the core to control
        Me.m_Core = core
        ' Remember applauncher to ask for additional information
        Me.m_AppLauncher = appl
        ' No Ecospace scenario active
        Me.m_EcospaceScenario = Nothing
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
    <CLSCompliant(False)> _
    Public Function LoadState(ByVal iState As eCoreExecutionState, _
            Optional ByVal bForceState As Boolean = False) As Boolean

        ' State already superceded or active?
        If (m_Core.StateMonitor.IsExecutionStateSuperceded(iState)) And (bForceState = False) Then
            Return True
        End If

        Select Case iState

            Case eCoreExecutionState.EcopathLoaded
                Return TryLoadEcopathModel()

            Case eCoreExecutionState.EcopathCompleted
                Return TryRunEcoPathModel()

            Case eCoreExecutionState.EcosimLoaded
                Return TryLoadEcosimScenario(False)

            Case eCoreExecutionState.EcosimCompleted
                ' FG 070214: Ecosim allowed to run implicitly
                Return TryRunEcosimModel()

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

    <CLSCompliant(False)> _
    Public Function LoadPersistState(ByVal iState As eCoreExecutionState, _
            Optional ByVal bForceState As Boolean = False) As Boolean

        ' State already superceded or active?
        If (m_Core.StateMonitor.IsExecutionStateSuperceded(iState)) And (bForceState = False) Then
            Return True
        End If

        Select Case iState

            Case eCoreExecutionState.EcopathLoaded
                Return TryLoadEcopathModel()

            Case eCoreExecutionState.EcopathCompleted
                Return TryRunEcoPathModel()

            Case eCoreExecutionState.EcosimLoaded
                Return TryLoadEcosimScenario(True)

            Case eCoreExecutionState.EcosimCompleted
                ' Ecosim is only run from a dedicated interface.
                ' Cannot launch this interface from here; fall through.
                'Feb 14, 2007: After discussion, we allow implicit Ecosim 
                ' running
                Return TryRunEcosimModel()

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

    ''' <summary>The core, the core.</summary>
    Private m_Core As cCore = Nothing
    ''' <summary>AppLauncher instance to ask for additional information.</summary>
    Private m_AppLauncher As AppLauncher = Nothing
    ''' <summary>Current Ecospace scenario.</summary>
    Private m_EcospaceScenario As cEcospaceScenario = Nothing

    ' *************************************************************************
    ' VERIFY_JS: conceptual issue - this information should not be stored in this class!
    ' Instead, this class should be handed a time step delegate, or it should
    ' be able to ask the AppLauncher for such a delegate.
    Private m_BiomassResults(,) As Single
    Private m_CurrentStep As Integer
    ' *************************************************************************

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
    ''' Attempt to run an Ecopath model.
    ''' </summary>
    ''' <returns>True if succesful.</returns>
    ''' ---------------------------------------------------------------------------
    Private Function TryRunEcoPathModel() As Boolean

        ' Is ecopath model loaded?
        If LoadState(eCoreExecutionState.EcopathLoaded) Then
            ' #Yes: run ecopath
            Return m_Core.RunEcoPath()
        End If

        Return False
    End Function

    Private Function TryRunEcosimModel() As Boolean

        ' Is Ecosim scenario loaded?
        If LoadState(eCoreExecutionState.EcosimLoaded) Then
            ReDim m_BiomassResults(m_Core.nGroups, m_Core.EcoSimModelParameters().NumberYears * 12)
            m_CurrentStep = 0
            Return m_Core.RunEcoSim(AddressOf TimeStepFromEcoSim_handler)
        End If

        Return False

    End Function


    Private Sub TimeStepFromEcoSim_handler(ByVal iTime As Long, ByVal results As cEcoSimResults)

        m_CurrentStep = CInt(iTime)

        For groupIndex As Integer = 1 To results.nGroups
            m_BiomassResults(groupIndex, CInt(iTime)) = results.Biomass(groupIndex)
        Next

    End Sub

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Attempt to load an Ecosim scenario.
    ''' </summary>
    ''' <returns>True if succesful.</returns>
    ''' ---------------------------------------------------------------------------
    Private Function TryLoadEcosimScenario(ByVal bPersist As Boolean) As Boolean

        Dim bSuccess As Boolean = False

        'FG: Bug fix June 14, 2007
        If bPersist Then
            If Me.LoadPersistState(eCoreExecutionState.EcopathCompleted) Then
                ' Let AppLauncher perform the load as it sees fit
                bSuccess = Me.m_AppLauncher.LoadEcosimScenario(bPersist, True)
            End If
        Else
            If Me.LoadState(eCoreExecutionState.EcopathCompleted) Then
                ' Let AppLauncher perform the load as it sees fit
                bSuccess = Me.m_AppLauncher.LoadEcosimScenario(bPersist, True)
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

        'FG: Bug fix June 14, 2007
        If bPersist Then
            ' JS 07mar07: Ecosim model needs to be loaded, not run, for an ecospace model to load.
            If Me.LoadPersistState(eCoreExecutionState.EcosimLoaded) Then
                ' Let AppLauncher perform the load as it sees fit
                bSuccess = Me.m_AppLauncher.LoadEcospaceScenario(bPersist, True)
            End If
        Else
            ' JS 07mar07: Ecosim model needs to be loaded, not run, for an ecospace model to load.
            If Me.LoadState(eCoreExecutionState.EcosimLoaded) Then
                ' Let AppLauncher perform the load as it sees fit
                bSuccess = Me.m_AppLauncher.LoadEcospaceScenario(bPersist, True)
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

        If bPersist Then
            ' JS 07mar07: Ecosim model needs to be loaded, not run, for an ecotracer model to load.
            If Me.LoadPersistState(eCoreExecutionState.EcosimLoaded) Then
                ' Let AppLauncher perform the load as it sees fit
                bSuccess = Me.m_AppLauncher.LoadEcotracerScenario(bPersist, True)
            End If
        Else
            ' JS 07mar07: Ecosim model needs to be loaded, not run, for an ecotracer model to load.
            If Me.LoadState(eCoreExecutionState.EcosimLoaded) Then
                ' Let AppLauncher perform the load as it sees fit
                bSuccess = Me.m_AppLauncher.LoadEcotracerScenario(bPersist, True)
            End If
        End If

        Return bSuccess
    End Function

#End Region ' Private members 

End Class
