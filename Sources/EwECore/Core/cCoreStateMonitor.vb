'==============================================================================
'
' $Log: cCoreStateMonitor.vb,v $
' Revision 1.2  2008/11/20 17:30:54  jeroens
' Uses initialized core exec states
'
' Revision 1.1  2008/09/26 07:30:12  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.34  2008/07/10 18:20:57  jeroens
' Fixed forceupdate bug which caused unneccesary notifications
'
' Revision 1.33  2008/07/01 19:13:09  sherman
' Merged branch - Fix_Ecopat_EcosimUpdateBug
'
' Revision 1.32.2.2  2008/07/01 18:36:25  sherman
' Merged Fix_Ecopat_EcosimUpdate...
'
' Revision 1.32  2008/06/28 03:21:02  jeroens
' State monitor maintains simple modifaction registry that allows the core to determine to what level Sim and Space need to be reinitialized
'
' Revision 1.31  2008/04/24 08:33:40  jeroens
' Core execution state recalculated when data state changes, REGARDLESS whether data state changed!
'
' Revision 1.30  2008/03/04 23:59:47  jeroens
' Core state monitor now handles run state updates; this is no longer handled by the core (who screwed up at it)
'
' Revision 1.29  2008/02/28 16:31:54  joeb
' Fixed bug in HasEcospaceRan
'
' Revision 1.28  2008/02/28 16:23:36  jeroens
' Fixed ecospace state config copy/paste bugs
'
' Revision 1.27  2008/02/27 17:02:04  jeroens
' Fixed diagnosis bug of EcotracerLoaded state
'
' Revision 1.26  2008/02/22 15:13:43  jeroens
' no message
'
' Revision 1.24  2008/01/29 15:55:49  jeroens
' Made CLS compliant
'
' Revision 1.23  2008/01/06 17:01:21  jeroens
' * Fixed bug 371
'
' Revision 1.22  2007/12/09 22:11:45  jeroens
' + Added new data entity Datasource for storing generic data
'
' Revision 1.21  2007/12/05 03:34:39  jeroens
' + Added ecotracer support
'
' Revision 1.20  2007/09/04 01:24:04  jeroens
' * Core state monitor suppressed data change notifications while core is under a batch lock
'
' Revision 1.19  2007/08/27 17:37:38  jeroens
' + Added ability to force updates on DataState changes
'
' Revision 1.18  2007/06/03 15:27:39  jeroens
' + Added Ecopath, Ecosim and Ecospace state change events
'
' Revision 1.17  2007/06/03 02:39:10  jeroens
' + Added Is**Running diagnostics
'
' Revision 1.16  2007/05/30 16:39:15  jeroens
' + Added option to force update events to be broadcasted
'
' Revision 1.15  2007/03/21 16:26:24  jeroens
' * Fixed state change detection bug in CalcCoreExecutionState
' + Only unloading a model/scenario will void higher states
'
' Revision 1.14  2007/03/15 14:10:54  jeroens
' * Moved eCoreExecutionsState to EwEUtils
'
' Revision 1.13  2007/03/08 17:02:37  jeroens
' - Removed discontinued data state config methods
' * Simplified CalcExecutionState to prevent Ecosim state does not aversely affect Ecospace state
'
' Revision 1.12  2007/03/07 18:24:16  jeroens
' * Data changed state obtained from Datasource
' * Ecopath can load when Ecosim loaded; Ecosim does no longer need to run first
'
' Revision 1.11  2007/01/30 16:59:29  jeroens
' * Fixed XML comment warning
'
' Revision 1.10  2007/01/25 16:28:41  jeroens
' + EcospaceLoaded state set correctly
'
' Revision 1.9  2007/01/25 15:38:33  jeroens
' + Properly named core monitor states
'
' Revision 1.8  2007/01/14 21:10:38  jeroens
' no message
'
' Revision 1.7  2007/01/12 14:51:58  jeroens
' + Added Ecospace support
'
' Revision 1.6  2006/10/10 15:20:09  jeroens
' * xxxLoaded states properly clear data changed states
'
' Revision 1.5  2006/10/06 15:55:42  jeroens
' + Added VoidExecutionState
'
' Revision 1.4  2006/09/27 23:38:06  jeroens
' * Fixed bug in SetEcopathModified
'
' Revision 1.3  2006/09/14 02:29:56  jeroens
' + Fixed data state update bug
'
' Revision 1.2  2006/09/10 12:57:07  jeroens
' + Added usage code example
'
' Revision 1.1  2006/09/10 03:03:55  jeroens
' Initial version
'
'==============================================================================

Option Strict On

Imports EwECore.DataSources
Imports EwEUtils.Core

''' ---------------------------------------------------------------------------
''' <summary>
''' Monitor that distributes Core execution state change events and Core data
''' state change events.
''' </summary>
''' <remarks>
''' <para>The following class tracks core execution state changes:</para>
''' <code>
''' Class StateTracker
''' 
'''     Public Sub New(ByRef sm as cCoreStateMonitor)
'''         ' Hook up to core state monitor
'''        AddHandler sm.CoreExecutionStateEvent, AddressOf OnCoreExecutionStateChange
'''     End Sub
''' 
'''     Private Sub OnCoreExecutionStateChange(ByVal core As cCore, ByVal iState As eCoreExecutionState)
'''        ' Handle core state changes
'''        Console.WriteLine("State tracker: core {0} state has changed to {1}", core, iState)
'''     End Sub
'''
''' End Class
''' </code>
''' </remarks>
''' ---------------------------------------------------------------------------
Public Class cCoreStateMonitor

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Constructor, initializes a new instance of this class.
    ''' </summary>
    ''' <param name="core">The <see cref="cCore">Core</see> that is monitored.</param>
    ''' -----------------------------------------------------------------------
    Public Sub New(ByRef core As cCore)
        Debug.Assert(core IsNot Nothing)
        Me.m_core = core
    End Sub

#Region " CoreExecutionState Delegates and Events "

    ''' -----------------------------------------------------------------------
    ''' <summary>Delegate, invoked to broadcast a core execution state change event.</summary>
    ''' <param name="core">A reference to the EwE <see cref="cCore">Core</see> which
    ''' execution state changed.</param>
    ''' <param name="iState">The new <see cref="eCoreExecutionState">Core execution state</see>.</param>
    ''' -----------------------------------------------------------------------
    Public Delegate Sub CoreExecutionStateDelegate(ByVal core As cCore, ByVal iState As eCoreExecutionState)

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' The core execution state change event.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Custom Event CoreExecutionStateEvent As CoreExecutionStateDelegate
        AddHandler(ByVal handler As CoreExecutionStateDelegate)
            Me.m_executionStateHandlers.Add(handler)
            handler.Invoke(Me.m_core, Me.m_iExecutionState)
        End AddHandler

        RemoveHandler(ByVal handler As CoreExecutionStateDelegate)
            Me.m_executionStateHandlers.Remove(handler)
        End RemoveHandler

        RaiseEvent(ByVal core As cCore, ByVal iState As eCoreExecutionState)
            For Each h As CoreExecutionStateDelegate In Me.m_executionStateHandlers
                h.Invoke(core, iState)
            Next
        End RaiseEvent
    End Event

    ''' <summary>List of all subscribed core execution state event listeners.</summary>
    Private m_executionStateHandlers As New List(Of CoreExecutionStateDelegate)

#End Region ' CoreExcutionState Delegates and Events

#Region " CoreDataState Delegate and Event "

    ''' -----------------------------------------------------------------------
    ''' <summary>Delegate, invoked to broadcast a core data state change event.</summary>
    ''' <param name="coreStateMonitor">THe monitor sending the event.</param>
    ''' -----------------------------------------------------------------------
    Public Delegate Sub CoreDataStateDelegate(ByVal coreStateMonitor As cCoreStateMonitor)

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' The core data state change event.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Custom Event CoreDataStateEvent As CoreDataStateDelegate
        AddHandler(ByVal handler As CoreDataStateDelegate)
            Me.m_dataStateHandlers.Add(handler)
            handler.Invoke(Me)
        End AddHandler

        RemoveHandler(ByVal handler As CoreDataStateDelegate)
            Me.m_dataStateHandlers.Remove(handler)
        End RemoveHandler

        RaiseEvent(ByVal coreStateMonitor As cCoreStateMonitor)
            For Each h As CoreDataStateDelegate In Me.m_dataStateHandlers
                h.Invoke(Me)
            Next
        End RaiseEvent
    End Event

    ''' <summary>List of all subscribed core data state event listeners.</summary>
    Private m_dataStateHandlers As New List(Of CoreDataStateDelegate)

#End Region ' CoreExcutionState Delegate and Event

#Region " Private members "

    ''' <summary>Reference to the monitored core.</summary>
    Private m_core As cCore = Nothing

    ''' <summary>Core execution state flag.</summary>
    Private m_iExecutionState As eCoreExecutionState = eCoreExecutionState.Idle
    ''' <summary>Ecopath execution state flag.</summary>
    Private m_iEcopathState As eCoreExecutionState = eCoreExecutionState.Idle
    ''' <summary>Ecosim execution state flag.</summary>
    Private m_iEcosimState As eCoreExecutionState = eCoreExecutionState.Idle
    ''' <summary>Ecospace execution state flag.</summary>
    Private m_iEcospaceState As eCoreExecutionState = eCoreExecutionState.Idle
    ''' <summary>Ecotracer execution state flag.</summary>
    Private m_iEcotracerState As eCoreExecutionState = eCoreExecutionState.Idle

    ''' <summary>Flag indicating whether the datasource contains unsaved changes that do not affect the running model and its scenarios.</summary>
    Private m_bDatasourceModified As Boolean = False
    ''' <summary>Flag indicating whether the ecopath model data contains unsaved changes.</summary>
    Private m_bEcopathModified As Boolean = False
    ''' <summary>Flag indicating whether the ecosim scenario data contains unsaved changes.</summary>
    Private m_bEcosimModified As Boolean = False
    ''' <summary>Flag indicating whether the ecospace scenario data contains unsaved changes.</summary>
    Private m_bEcospaceModified As Boolean = False
    ''' <summary>Flag indicating whether the ecotracer scenario data contains unsaved changes.</summary>
    Private m_bEcotracerModified As Boolean = False

#End Region ' Private members

#Region " Private helpers "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Calculates and updates the Core execution state. A 
    ''' <see cref="CoreExecutionStateEvent">CoreExecutionStateEvent</see> is
    ''' broadcasted when the Core execution state changes.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub CalcExecutionState(ByVal iEcopathState As eCoreExecutionState, _
            ByVal iEcosimState As eCoreExecutionState, _
            ByVal iEcospaceState As eCoreExecutionState, _
            ByVal iEcotracerState As eCoreExecutionState, _
            Optional ByVal bForceUpdate As Boolean = False)

        Dim iState As eCoreExecutionState = eCoreExecutionState.Idle
        Dim bEcopathStateChange As Boolean = False
        Dim bEcosimStateChange As Boolean = False
        Dim bEcospaceStateChange As Boolean = False
        Dim bEcotracerStateChange As Boolean = False

        bEcopathStateChange = (iEcopathState <> Me.m_iEcopathState)
        bEcosimStateChange = (iEcosimState <> Me.m_iEcosimState)
        bEcospaceStateChange = (iEcospaceState <> Me.m_iEcospaceState)
        bEcotracerStateChange = (iEcotracerState <> Me.m_iEcotracerState)

        ' No state changes?
        If (Not bEcopathStateChange And Not bEcosimStateChange And Not bEcospaceStateChange And Not bEcotracerStateChange) And (Not bForceUpdate) Then Return

        ' Accept ecopath state
        iState = iEcopathState
        ' Has ecopath model ran?
        If iState = eCoreExecutionState.EcopathCompleted Then
            ' #Yes: is an ecosim scenario loaded?
            If iEcosimState <> eCoreExecutionState.Idle Then
                ' #Yes: accept ecosim state
                iState = iEcosimState
                ' Is an ecosim model loaded?
                If iState >= eCoreExecutionState.EcosimLoaded Then
                    ' #Yes: is an ecospace model loaded?
                    If iEcospaceState <> eCoreExecutionState.Idle Then
                        ' #Yes: accept ecospace state
                        iState = iEcospaceState
                    End If
                End If
            End If
        End If

        ' Update local execution sub-state flags
        Me.m_iEcopathState = iEcopathState
        Me.m_iEcosimState = iEcosimState
        Me.m_iEcospaceState = iEcospaceState
        Me.m_iEcotracerState = iEcotracerState

        ' Update core execution state flag
        Me.m_iExecutionState = iState

        ' Broadcast states
        RaiseEvent CoreExecutionStateEvent(Me.m_core, Me.m_iExecutionState)
    End Sub

    Friend Sub UpdateDataState(ByVal ds As IEwEDataSource, Optional ByVal tsSendUpdate As TriState = TriState.UseDefault)

        Dim bDatasourceModified As Boolean = False
        Dim bEcopathModified As Boolean = False
        Dim bEcosimModified As Boolean = False
        Dim bEcospaceModified As Boolean = False
        Dim bEcotracerModified As Boolean = False

        If (ds IsNot Nothing) Then
            bDatasourceModified = ds.IsModified()
            If (TypeOf ds Is IEcopathDataSource) Then bEcopathModified = DirectCast(ds, IEcopathDataSource).IsEcopathModified()
            If (TypeOf ds Is IEcosimDatasource) Then bEcosimModified = DirectCast(ds, IEcosimDatasource).IsEcosimModified()
            If (TypeOf ds Is IEcospaceDatasource) Then bEcospaceModified = DirectCast(ds, IEcospaceDatasource).IsEcospaceModified()
            If (TypeOf ds Is IEcotracerDatasource) Then bEcotracerModified = DirectCast(ds, IEcotracerDatasource).IsEcotracerModified()
        End If

        Me.UpdateDataState(bDatasourceModified, bEcopathModified, bEcosimModified, bEcospaceModified, bEcotracerModified, tsSendUpdate)
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Calculates and updates the Core data state. A 
    ''' <see cref="CoreDataStateEvent">CoreDataStateEvent</see> is
    ''' broadcasted when the data state of either Ecopath or Ecosim changes.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub UpdateDataState(ByVal bDatasourceModified As Boolean, _
            ByVal bEcopathModified As Boolean, _
            ByVal bEcosimModified As Boolean, _
            ByVal bEcospaceModified As Boolean, _
            ByVal bEcotracerModified As Boolean, _
            Optional ByVal tsSendUpdate As TriState = TriState.UseDefault)

        Dim bChange As Boolean = (bDatasourceModified <> Me.m_bDatasourceModified) Or _
           (bEcopathModified <> Me.m_bEcopathModified) Or _
           (bEcosimModified <> Me.m_bEcosimModified) Or _
           (bEcospaceModified <> Me.m_bEcospaceModified) Or _
           (bEcotracerModified <> Me.m_bEcotracerModified)

        ' Update flags
        Me.m_bDatasourceModified = bDatasourceModified
        Me.m_bEcopathModified = bEcopathModified
        Me.m_bEcosimModified = bEcosimModified
        Me.m_bEcospaceModified = bEcospaceModified
        Me.m_bEcotracerModified = bEcotracerModified

        ' Update core execution state
        Me.CalcExecutionState( _
            DirectCast(IIf(Me.m_bEcopathModified, eCoreExecutionState.EcopathLoaded, Me.m_iEcopathState), eCoreExecutionState), _
            DirectCast(IIf(Me.m_bEcosimModified, eCoreExecutionState.EcosimLoaded, Me.m_iEcosimState), eCoreExecutionState), _
            DirectCast(IIf(Me.m_bEcospaceModified, eCoreExecutionState.EcospaceLoaded, Me.m_iEcospaceState), eCoreExecutionState), _
            DirectCast(IIf(Me.m_bEcotracerModified, eCoreExecutionState.EcotracerLoaded, Me.m_iEcotracerState), eCoreExecutionState), _
            (tsSendUpdate = TriState.True))

        ' Broadcast data state event
        If tsSendUpdate = TriState.False Then Return
        If tsSendUpdate = TriState.UseDefault And Not bChange Then Return

        ' Broadcast changes
        RaiseEvent CoreDataStateEvent(Me)

    End Sub

#End Region ' Private helpers

#Region " State configuration "

#Region " Ecopath "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' State change entry point; to be called when an Ecopath model is loaded
    ''' or unloaded.
    ''' </summary>
    ''' <param name="bHasModel">Flag indicating whether an Ecopath model is
    ''' loaded (True) or unloaded (False).</param>
    ''' -----------------------------------------------------------------------
    Friend Sub SetEcopathLoaded(ByVal bHasModel As Boolean, Optional ByVal bForceUpdate As Boolean = False)
        ' Update execution state
        If bHasModel Then
            ' Switch to ecopath loaded. All other model states must be reset to either idle or loaded
            Me.CalcExecutionState(eCoreExecutionState.EcopathLoaded, _
                DirectCast(Math.Min(Me.m_iEcosimState, eCoreExecutionState.EcosimLoaded), eCoreExecutionState), _
                DirectCast(Math.Min(Me.m_iEcospaceState, eCoreExecutionState.EcospaceLoaded), eCoreExecutionState), _
                DirectCast(Math.Min(Me.m_iEcotracerState, eCoreExecutionState.EcotracerLoaded), eCoreExecutionState), _
                bForceUpdate)
        Else
            Me.CalcExecutionState(eCoreExecutionState.Idle, eCoreExecutionState.Idle, eCoreExecutionState.Idle, eCoreExecutionState.Idle, bForceUpdate)
        End If
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' State change entry point; to be called when an Ecopath model is initialized.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Friend Sub SetEcopathInitialized()
        ' Check for invalid state transitions
        If (Me.m_iEcopathState = eCoreExecutionState.Idle) Then Return
        ' Update execution state
        Me.CalcExecutionState(eCoreExecutionState.EcopathInitialized, _
            DirectCast(Math.Min(Me.m_iEcosimState, eCoreExecutionState.EcosimLoaded), eCoreExecutionState), _
            DirectCast(Math.Min(Me.m_iEcospaceState, eCoreExecutionState.EcospaceLoaded), eCoreExecutionState), _
            DirectCast(Math.Min(Me.m_iEcotracerState, eCoreExecutionState.EcotracerLoaded), eCoreExecutionState))
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' State change entry point; to be called when an Ecopath model is started.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Friend Sub SetEcopathRun()
        ' Check for invalid state transitions
        If (Me.m_iEcopathState = eCoreExecutionState.Idle) Then Return
        ' Update execution state
        Me.CalcExecutionState(eCoreExecutionState.EcopathRunning, _
                DirectCast(Math.Min(Me.m_iEcosimState, eCoreExecutionState.EcosimLoaded), eCoreExecutionState), _
                DirectCast(Math.Min(Me.m_iEcospaceState, eCoreExecutionState.EcospaceLoaded), eCoreExecutionState), _
                DirectCast(Math.Min(Me.m_iEcotracerState, eCoreExecutionState.EcotracerLoaded), eCoreExecutionState))
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' State change entry point; to be called when an Ecopath model has
    ''' completed its parameter estimation.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Friend Sub SetEcopathCompleted()
        ' Check for invalid state transitions
        If (Me.m_iEcopathState <> eCoreExecutionState.EcopathRunning) Then Return
        ' Update execution state
        Me.CalcExecutionState(eCoreExecutionState.EcopathCompleted, _
                DirectCast(Math.Min(Me.m_iEcosimState, eCoreExecutionState.EcosimLoaded), eCoreExecutionState), _
                DirectCast(Math.Min(Me.m_iEcospaceState, eCoreExecutionState.EcospaceLoaded), eCoreExecutionState), _
                DirectCast(Math.Min(Me.m_iEcotracerState, eCoreExecutionState.EcotracerLoaded), eCoreExecutionState))
    End Sub

#End Region ' Ecopath

#Region " Ecosim "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' State change entry point; to be called when an Ecosim scenario is loaded
    ''' or unloaded.
    ''' </summary>
    ''' <param name="bHasScenario">Flag indicating whether an Ecosim scenario is
    ''' loaded (True) or unloaded (False).</param>
    ''' -----------------------------------------------------------------------
    Friend Sub SetEcoSimLoaded(ByVal bHasScenario As Boolean, Optional ByVal bForceUpdate As Boolean = False)
        ' Update execution state
        If bHasScenario Then
            ' Switch to ecosim loaded state. Space and Tracer states must be reset to either idle or loaded
            Me.CalcExecutionState(Me.m_iEcopathState, eCoreExecutionState.EcosimLoaded, _
                DirectCast(Math.Min(Me.m_iEcospaceState, eCoreExecutionState.EcospaceLoaded), eCoreExecutionState), _
                DirectCast(Math.Min(Me.m_iEcotracerState, eCoreExecutionState.EcotracerLoaded), eCoreExecutionState), _
                bForceUpdate)
        Else
            Me.CalcExecutionState(Me.m_iEcopathState, eCoreExecutionState.Idle, eCoreExecutionState.Idle, eCoreExecutionState.Idle, bForceUpdate)
        End If
        ' Clear scenario changed flags
        Me.UpdateDataState(Me.m_bDatasourceModified, Me.m_bEcopathModified, False, False, Me.m_bEcotracerModified)
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' State change entry point; to be called when an Ecosim scenario is initialized.
    ''' or unloaded.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Friend Sub SetEcoSimInitialized()
        ' Check for invalid state transitions
        If (Me.m_iEcosimState = eCoreExecutionState.Idle) Then Return
        ' Update execution state
        Me.CalcExecutionState(Me.m_iEcopathState, eCoreExecutionState.EcosimInitialized, _
                DirectCast(Math.Min(Me.m_iEcospaceState, eCoreExecutionState.EcospaceLoaded), eCoreExecutionState), _
                DirectCast(Math.Min(Me.m_iEcotracerState, eCoreExecutionState.EcotracerLoaded), eCoreExecutionState))
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' State change entry point; to be called when an Ecosim scenario is started.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Friend Sub SetEcosimRun()
        ' Check for invalid state transitions
        If (Me.m_iEcosimState = eCoreExecutionState.Idle) Then Return
        ' Update execution state
        Me.CalcExecutionState(Me.m_iEcopathState, eCoreExecutionState.EcosimRunning, _
                DirectCast(Math.Min(Me.m_iEcospaceState, eCoreExecutionState.EcospaceLoaded), eCoreExecutionState), _
                DirectCast(Math.Min(Me.m_iEcotracerState, eCoreExecutionState.EcotracerLoaded), eCoreExecutionState))
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' State change entry point; to be called when an Ecosim scenario has 
    ''' completed its timesteps.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Friend Sub SetEcosimCompleted()
        ' Check for invalid state transitions
        If (Me.m_iEcosimState <> eCoreExecutionState.EcosimRunning) Then Return

        Me.m_bRequiresEcosimFullInit = False

        ' Update execution state
        Me.CalcExecutionState(Me.m_iEcopathState, eCoreExecutionState.EcosimCompleted, _
                DirectCast(Math.Min(Me.m_iEcospaceState, eCoreExecutionState.EcospaceLoaded), eCoreExecutionState), _
                DirectCast(Math.Min(Me.m_iEcotracerState, eCoreExecutionState.EcotracerLoaded), eCoreExecutionState))
    End Sub

#End Region ' Ecosim

#Region " Ecospace "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' State change entry point; to be called when an Ecospace scenario is loaded
    ''' or unloaded.
    ''' </summary>
    ''' <param name="bHasScenario">Flag indicating whether an Ecospace scenario is
    ''' loaded (True) or unloaded (False).</param>
    ''' -----------------------------------------------------------------------
    Friend Sub SetEcospaceLoaded(ByVal bHasScenario As Boolean, Optional ByVal bForceUpdate As Boolean = False)
        ' Update execution state
        If bHasScenario Then
            ' Switch to ecospace loaded state. Tracer state must be reset to either idle or loaded
            Me.CalcExecutionState(Me.m_iEcopathState, Me.m_iEcosimState, eCoreExecutionState.EcospaceLoaded, _
                DirectCast(Math.Min(Me.m_iEcotracerState, eCoreExecutionState.EcotracerLoaded), eCoreExecutionState), _
                bForceUpdate)
        Else
            Me.CalcExecutionState(Me.m_iEcopathState, Me.m_iEcosimState, eCoreExecutionState.Idle, Me.m_iEcotracerState, bForceUpdate)
        End If
        ' Clear scenario changed flags
        Me.UpdateDataState(Me.m_bDatasourceModified, Me.m_bEcopathModified, Me.m_bEcosimModified, False, Me.m_bEcotracerModified)
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' State change entry point; to be called when an Ecospace scenario is initialized.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Friend Sub SetEcospaceInitialized()
        ' Check for invalid state transitions
        If (Me.m_iEcosimState = eCoreExecutionState.Idle) Then Return
        ' Update execution state
        Me.CalcExecutionState(Me.m_iEcopathState, Me.m_iEcosimState, eCoreExecutionState.EcospaceInitialized, _
                DirectCast(Math.Min(Me.m_iEcotracerState, eCoreExecutionState.EcotracerLoaded), eCoreExecutionState))
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' State change entry point; to be called when an Ecospace scenario is started.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Friend Sub SetEcospaceRun()
        ' Check for invalid state transitions
        If (Me.m_iEcosimState = eCoreExecutionState.Idle) Then Return
        ' Update execution state
        Me.CalcExecutionState(Me.m_iEcopathState, Me.m_iEcosimState, eCoreExecutionState.EcospaceRunning, _
                DirectCast(Math.Min(Me.m_iEcotracerState, eCoreExecutionState.EcotracerLoaded), eCoreExecutionState))
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' State change entry point; to be called when an Ecosim scenario has 
    ''' completed its timesteps.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Friend Sub SetEcospaceCompleted()
        ' Check for invalid state transitions
        If (Me.m_iEcospaceState <> eCoreExecutionState.EcospaceRunning) Then Return
        ' Update execution state
        Me.CalcExecutionState(Me.m_iEcopathState, Me.m_iEcosimState, eCoreExecutionState.EcospaceCompleted, _
                DirectCast(Math.Min(Me.m_iEcotracerState, eCoreExecutionState.EcotracerLoaded), eCoreExecutionState))
    End Sub

#End Region ' Ecospace

#Region " Ecotracer "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' State change entry point; to be called when an Ecotracer scenario is loaded
    ''' or unloaded.
    ''' </summary>
    ''' <param name="bHasScenario">Flag indicating whether an Ecotracer scenario is
    ''' loaded (True) or unloaded (False).</param>
    ''' -----------------------------------------------------------------------
    Friend Sub SetEcotracerLoaded(ByVal bHasScenario As Boolean, Optional ByVal bForceUpdate As Boolean = False)
        ' Update execution state
        If bHasScenario Then
            Me.CalcExecutionState(Me.m_iEcopathState, Me.m_iEcosimState, Me.m_iEcospaceState, eCoreExecutionState.EcotracerLoaded, bForceUpdate)
        Else
            Me.CalcExecutionState(Me.m_iEcopathState, Me.m_iEcosimState, Me.m_iEcospaceState, eCoreExecutionState.Idle, bForceUpdate)
        End If
        ' Clear scenario changed flags
        Me.UpdateDataState(Me.m_bDatasourceModified, Me.m_bEcopathModified, Me.m_bEcosimModified, Me.m_bEcospaceModified, False)
    End Sub

#End Region ' Ecotracer

#End Region ' State configuration

#Region " State diagnostics "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns whether there are ANY unsaved changes.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Function IsModified() As Boolean
        ' OMG
        Return (Me.IsDatasourceModified Or Me.IsEcopathModified Or Me.IsEcosimModified Or Me.IsEcospaceModified Or Me.IsEcotracerModified)
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns whether an Ecopath model has been loaded.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Function HasEcopathLoaded() As Boolean
        Return Me.m_iEcopathState <> eCoreExecutionState.Idle
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns whether an Ecopath model has been initialized.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Function HasEcopathInitialized() As Boolean
        Return Me.m_iEcopathState = eCoreExecutionState.EcopathInitialized
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns whether an Ecopath model is running.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Function IsEcopathRunning() As Boolean
        Return Me.m_iEcopathState = eCoreExecutionState.EcopathRunning
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns whether an Ecopath model has completed succesfully.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Function HasEcopathRan() As Boolean
        Return Me.m_iEcopathState = eCoreExecutionState.EcopathCompleted
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns whether an Ecosim scenario has been loaded.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Function HasEcosimLoaded() As Boolean
        Return Me.m_iEcosimState <> eCoreExecutionState.Idle
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns whether an Ecosim scenario has been initialized.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Function HasEcosimInitialized() As Boolean
        Return Me.m_iEcosimState = eCoreExecutionState.EcosimInitialized
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns whether an Ecosim scenario is running.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Function IsEcosimRunning() As Boolean
        Return Me.m_iEcosimState = eCoreExecutionState.EcosimRunning
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns whether an Ecosim scenario has completed its timesteps succesfully.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Function HasEcosimRan() As Boolean
        Return Me.m_iEcosimState = eCoreExecutionState.EcosimCompleted
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns whether an Ecospace scenario has been loaded.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Function HasEcospaceLoaded() As Boolean
        Return Me.m_iEcospaceState <> eCoreExecutionState.Idle
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns whether an Ecospace scenario has been initialized.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Function HasEcospaceInitialized() As Boolean
        Return Me.m_iEcospaceState = eCoreExecutionState.EcospaceInitialized
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns whether an Ecospace scenario is running.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Function IsEcospaceRunning() As Boolean
        Return Me.m_iEcospaceState = eCoreExecutionState.EcospaceRunning
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns whether an Ecospace scenario has completed its timesteps succesfully.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Function HasEcospaceRan() As Boolean
        Return Me.m_iEcospaceState = eCoreExecutionState.EcospaceCompleted
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns whether the current core execution state equals or exceeds
    ''' a given state.
    ''' </summary>
    ''' <param name="iState">The <see cref="eCoreExecutionState">core execution state</see> to check.</param>
    ''' <remarks>
    ''' <para>The core execution states system is described by a sequence of 
    ''' states that supercede earlier states.</para>
    ''' <para>For instance, the eCoreExecutionState EcosimReady can only 
    ''' occur when an underlying Ecopath model has been loaded and when
    ''' the Ecopath model as completed a succesful run: the state
    ''' <see cref="eCoreExecutionState.EcosimLoaded">EcosimLoaded</see> thus supercedes
    ''' <see cref="eCoreExecutionState.EcopathLoaded">EcopathLoaded</see> and
    ''' <see cref="eCoreExecutionState.EcopathCompleted">EcopathCompleted</see>.</para>
    ''' <para>Please be careful when interpreting results from this method; do
    ''' not confuse superceding with implying! In the aforementioned example, 
    ''' the EcosimReady state also supercedes the state that descibes that an 
    ''' Ecopath model has been modified by the user, and the state that describes
    ''' that an Ecopath model is not loaded yet.</para>
    ''' <para>In some case, assuming that superceded states are also current 
    ''' states may lead to serious nonsense.</para>
    ''' </remarks>
    ''' <note_to_self>
    ''' Wow, that's a lot of talking for a one-line function implementation...
    ''' </note_to_self>
    ''' -----------------------------------------------------------------------
    Public Function IsExecutionStateSuperceded(ByVal iState As eCoreExecutionState) As Boolean
        ' Exception for Ecotracer load state since it does not fit the incremental state tree well;
        ' If Ecospace is loaded, the ecotracer loaded state is assumed true, ugh...
        If iState = eCoreExecutionState.EcotracerLoaded Then Return Me.HasEcotracerLoaded()

        Return (iState <= Me.m_iExecutionState)
    End Function

    Public Function CanEcopathLoad() As Boolean
        Return True
    End Function

    Public Function CanEcosimLoad() As Boolean
        Return Me.HasEcopathRan()
    End Function

    Public Function CanEcospaceLoad() As Boolean
        Return Me.HasEcosimLoaded()
    End Function

    Public Function CanEcotracerLoad() As Boolean
        Return Me.HasEcopathLoaded()
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns whether an Ecotracer scenario has been loaded.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Function HasEcotracerLoaded() As Boolean
        Return Me.m_iEcotracerState <> eCoreExecutionState.Idle
    End Function

#End Region ' State diagnostics

#Region " State variable access "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns the current EwE <see cref="cCore">Core</see> Execution state.
    ''' </summary>
    ''' <returns>A <see cref="eCoreExecutionState">eCoreExecutionState</see>
    ''' value indicating the Core execution state.</returns>
    ''' -----------------------------------------------------------------------
    Public Function CoreExecutionState() As eCoreExecutionState
        Return Me.m_iExecutionState
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns whether the Datasource contains changes that have not 
    ''' been saved, which will not influence the running model and its scenarios.
    ''' </summary>
    ''' <returns>True if there are unsaved changes, False otherwise.</returns>
    ''' -----------------------------------------------------------------------
    Public Function IsDatasourceModified() As Boolean
        Return Me.m_bDatasourceModified
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns whether the Ecopath model data contains changes that have not 
    ''' been saved.</summary>
    ''' <returns>True if there are unsaved changes, False otherwise.</returns>
    ''' -----------------------------------------------------------------------
    Public Function IsEcopathModified() As Boolean
        Return Me.m_bEcopathModified
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns whether the Ecosim scenario data contains changes that have not 
    ''' been saved.</summary>
    ''' <returns>True if there are unsaved changes, False otherwise.</returns>
    ''' -----------------------------------------------------------------------
    Public Function IsEcosimModified() As Boolean
        Return Me.m_bEcosimModified
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns whether the Ecospace scenario data contains changes that have not 
    ''' been saved.</summary>
    ''' <returns>True if there are unsaved changes, False otherwise.</returns>
    ''' -----------------------------------------------------------------------
    Public Function IsEcospaceModified() As Boolean
        Return Me.m_bEcospaceModified
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns whether the Ecotracer scenario data contains changes that have not 
    ''' been saved.</summary>
    ''' <returns>True if there are unsaved changes, False otherwise.</returns>
    ''' -----------------------------------------------------------------------
    Public Function IsEcotracerModified() As Boolean
        Return Me.m_bEcotracerModified
    End Function

#End Region ' ..for if you don't like events

#Region " Registry of modifications "

    Private m_bRequiresEcosimFullInit As Boolean = False

    Friend Sub RegisterModification(ByVal messageSource As eMessageSource)
        If messageSource = eMessageSource.EcoPath Then Me.m_bRequiresEcosimFullInit = True
    End Sub

    Friend Function RequiresEcosimFullInit() As Boolean
        Return Me.m_bRequiresEcosimFullInit
    End Function

#End Region ' Ministry of silly walks

End Class
