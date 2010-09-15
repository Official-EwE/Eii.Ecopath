#Region " Imports "

Option Strict On
Imports EwECore
Imports EwEUtils.Core
Imports EwEUtils.Utilities
Imports ScientificInterface.Ecospace.Basemap.Layers
Imports EwECore.Ecospace.Advection

#End Region ' Imports

Namespace Ecospace.Advection

    Public Class frmAdvection

#Region " Private vars "

        Private m_manager As cAdvectionManager = Nothing
 
        Private m_fpVX As cEwEFormatProvider = Nothing
        Private m_fpVY As cEwEFormatProvider = Nothing
        Private m_fpCoriolis As cEwEFormatProvider = Nothing
        Private m_fpWind As cEwEFormatProvider = Nothing
        Private m_fpMLD As cEwEFormatProvider = Nothing

        Private m_dlgtStarted As cAdvectionManager.ComputationStartedDelegate = Nothing
        Private m_dlgtProgress As cAdvectionManager.ComputationProgressDelegate = Nothing
        Private m_dlgtStopped As cAdvectionManager.ComputationCompletedDelegate = Nothing

        ''' <summary>Flag stating whether this form started a search.</summary>
        Private m_bSearching As Boolean = False
        ''' <summary>Flag stating whether a search was completed from this form.</summary>
        Private m_bHasRun As Boolean = False

#End Region ' Private vars

        Public Sub New()
            Me.InitializeComponent()
        End Sub

#Region " Form overrides "

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)

            ' Design time bypass
            If Me.UIContext Is Nothing Then Return

            Me.m_manager = Me.Core.AdvectionManager

            ' Set up format providers
            Me.m_fpCoriolis = New cPropertyFormatProvider(Me.UIContext, Me.m_nudCoriolis, Me.Core.AdvectionParameters, eVarNameFlags.Coriolis)
            Me.m_fpVX = New cPropertyFormatProvider(Me.UIContext, Me.m_nudXVelocity, Me.Core.AdvectionParameters, eVarNameFlags.XVelocity)
            Me.m_fpVY = New cPropertyFormatProvider(Me.UIContext, Me.m_nudYVelocity, Me.Core.AdvectionParameters, eVarNameFlags.YVelocity)
            Me.m_fpWind = New cEwEFormatProvider(Me.UIContext, Me.m_nudWind, GetType(Single))
            Me.m_fpMLD = New cEwEFormatProvider(Me.UIContext, Me.m_nudDepth, GetType(Integer))

            ' Connect all layers to the zoom toolbar
            For Each uc As ucAdvectionMap In Me.Maps
                Me.m_ucZoomToolbar.AddZoomContainer(uc.ZoomCtrl)
            Next
            Me.m_ucZoomToolbar.PositionMode = ucMapZoom.ePositionModeTypes.Center

            ' Populate month dropdown
            Me.m_tscmMonth.Items.Clear()
            For i As Integer = 1 To cCore.N_MONTHS
                Me.m_tscmMonth.Items.Add(cDateUtils.GetMonthName(i))
            Next
            Me.m_tscmMonth.SelectedIndex = 0

            ' Initialize control values
            Me.m_nudWind.Value = CDec(DirectCast(Me.m_ucWind.DataLayer.Editor, cLayerEditorVector).ScaleFactor)
            Me.m_nudDepth.Value = CDec(Me.m_ucMLD.DataLayer.Editor.CellValue)

            ' Config EwEForm
            Me.CoreComponents = New eCoreComponentType() {eCoreComponentType.EcoSpace}

            ' Kick off
            Me.UpdateControls()

            Me.m_dlgtStarted = New cAdvectionManager.ComputationStartedDelegate(AddressOf OnCalcStarted)
            Me.m_dlgtProgress = New cAdvectionManager.ComputationProgressDelegate(AddressOf OnCalcProgress)
            Me.m_dlgtStopped = New cAdvectionManager.ComputationCompletedDelegate(AddressOf OnCalcStopped)
            Me.m_manager.Connect(Me.m_dlgtStarted, Me.m_dlgtStopped, Me.m_dlgtProgress)

            If Me.m_manager.isRunning Then Me.StartRun()

        End Sub

        Protected Overrides Sub OnFormClosed(ByVal e As FormClosedEventArgs)

            ' Stop any pending run, just in case
            Me.StopRun()

            For Each uc As ucAdvectionMap In Me.Maps
                Me.m_ucZoomToolbar.RemoveZoomContainer(uc.ZoomCtrl)
            Next

            Me.m_fpVX.Release()
            Me.m_fpVY.Release()
            Me.m_fpCoriolis.Release()
            Me.m_fpWind.Release()
            Me.m_fpMLD.Release()

            MyBase.OnFormClosed(e)

        End Sub

#End Region ' Form overrides

#Region " Public bits "

        Public Overrides Property UIContext() As ScientificInterfaceShared.Controls.cUIContext
            Get
                Return MyBase.UIContext
            End Get
            Set(ByVal value As ScientificInterfaceShared.Controls.cUIContext)
                MyBase.UIContext = value
                Me.m_ucZoomToolbar.UIContext = Me.UIContext
                For Each uc As ucAdvectionMap In Me.Maps
                    uc.UIContext = value
                Next
            End Set
        End Property

#End Region ' Public bits

#Region " Control events "

        Private Sub OnToggleOptions(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_tsmiToggleOptions.Click

            ' Sanity check
            If Me.UIContext Is Nothing Then Return

            Me.m_scMain.Panel1Collapsed = Not Me.m_scMain.Panel1Collapsed
            Me.UpdateControls()

        End Sub

        Private Sub OnShowMonth(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_tscmMonth.SelectedIndexChanged

            ' Sanity check
            If Me.UIContext Is Nothing Then Return

            Dim layer As cLayer = Me.m_ucWind.DataLayer
            DirectCast(layer.Data, cEcospaceLayerWind).Month = (1 + Me.m_tscmMonth.SelectedIndex)
            layer.Update(cLayer.eChangeFlags.Map, False)

        End Sub

        Private Sub OnCursorSizeChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
            Handles m_sliderCursor.ValueChanged

            ' Sanity check
            If Me.UIContext Is Nothing Then Return

            ' Get cursor size
            Dim iCursorSize As Integer = CInt(Me.m_sliderCursor.Value)

            ' Distribute cursor size
            For Each uc As ucAdvectionMap In Me.Maps
                If uc.DataLayer IsNot Nothing Then
                    uc.DataLayer.Editor.CursorSize = iCursorSize
                    uc.Map.UpdateCursorFeedback()
                End If
            Next

        End Sub

        Private Sub OnWindValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
            Handles m_nudWind.ValueChanged

            ' Sanity check
            If Me.UIContext Is Nothing Then Return

            With DirectCast(Me.m_ucWind.DataLayer.Editor, cLayerEditorVector)
                .ScaleFactor = CSng(Me.m_nudWind.Value)
            End With

        End Sub

        Private Sub OnMLDValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
            Handles m_nudDepth.ValueChanged

            ' Sanity check
            If Me.UIContext Is Nothing Then Return

            Me.m_ucMLD.DataLayer.Editor.CellValue = CSng(Me.m_nudDepth.Value)

        End Sub

        Private Sub OnComputeVels(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnStart.Click
            Me.StartRun()
        End Sub

        Private Sub OnStopComputing(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnStop.Click
            Me.m_manager.StopRun()
        End Sub

        Private Sub OnApplyVels(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnApplyVels.Click

        End Sub

#End Region ' Control events

#Region " Event handlers "

        'Public Overrides Sub OnCoreMessage(ByVal msg As EwECore.cMessage)
        '    '' Refresh basemap on ANY data added or removed message from Ecospace
        '    'If ((msg.Source = eCoreComponentType.EcoSpace) And (msg.Type = eMessageType.DataAddedOrRemoved)) Then
        '    '    ' Refresh it all
        '    '    Me.Basemap = Me.Core.EcospaceBasemap
        '    'End If
        'End Sub

        'Private Sub OnCoreExecutionStateChanged(ByVal csm As cCoreStateMonitor)
        '    Me.UpdateControls()
        'End Sub

        Private Sub OnCalcStarted()
            Me.m_bHasRun = False
            Me.UpdateControls()
        End Sub

        Private Sub OnCalcProgress(ByVal iIter As Integer)

            ' Update app status
            cApplicationStatusNotifier.SetStatusText("Advection running iteration " & iIter, TriState.UseDefault, -1)
            ' Update data layer
            Dim layer As cLayer = Me.m_ucMap.DataLayer
            layer.Update(cLayer.eChangeFlags.Map, False)

            'Dim iLeft As Integer = 0
            'Math.DivRem(iIter, 10, iLeft)
            'If iLeft = 0 Then
            '    Me.m_ucMap.Refresh()
            'End If

        End Sub

        Private Sub OnCalcStopped(ByVal iIter As Integer, ByVal bInterrupted As Boolean, ByVal bBadFlow As Boolean)
            Me.StopRun()
            Me.m_ucMap.Invalidate()
            Me.m_bHasRun = Not bBadFlow
        End Sub

#End Region ' Event handlers

#Region " Internals "

        Private Sub UpdateControls()

            ' Gather stats
            Dim bBusy As Boolean = Me.m_manager.isRunning

            Me.m_btnStart.Enabled = Not bBusy And Not Me.m_bSearching
            Me.m_btnStop.Enabled = Me.m_bSearching
            Me.m_btnApplyVels.Enabled = Me.m_bHasRun

            Me.m_tsmiToggleOptions.Checked = Not Me.m_scMain.Panel1Collapsed

        End Sub

        Private Function Maps() As ucAdvectionMap()
            Return New ucAdvectionMap() {Me.m_ucMap, Me.m_ucMLD, Me.m_ucUpwelling, Me.m_ucWind}
        End Function

        Private Sub StartRun()

            ' Already running? Abort
            If Me.m_bSearching Then Return

            If Not Me.m_manager.isRunning Then Me.m_manager.Run(Me)
            Me.m_bSearching = Me.m_manager.isRunning

            If m_bSearching Then
                cApplicationStatusNotifier.SetStatusText("Starting Advection computations...", TriState.True, -1)
                Me.UpdateControls()
            End If

        End Sub

        Private Sub StopRun()

            If Not Me.m_bSearching Then Return

            Me.m_bSearching = False
            cApplicationStatusNotifier.SetStatusText("", TriState.False)
            Me.m_manager.StopRun()
            Me.UpdateControls()

        End Sub

#End Region ' Internals

    End Class

End Namespace
