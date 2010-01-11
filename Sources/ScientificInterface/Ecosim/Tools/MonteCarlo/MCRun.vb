#Region " Imports "

Option Explicit On
Option Strict On

Imports System.Windows.Forms
Imports EwECore
Imports EwEUtils.Commands
Imports EwEUtils.Core
Imports ZedGraph

#End Region

Namespace Ecosim

    ''' <summary>
    ''' Form class that implements the Ecosim Monte Carlo interface.
    ''' </summary>
    Public Class MCRun

#Region " Private vars "

        Private m_core As EwECore.cCore = Nothing
        Private m_mcmanager As cMonteCarloManager = Nothing
        Private m_plothelper As cEcosimOutputPlotHelper = Nothing

        Private WithEvents m_cmdRunMonteCarlo As cCommand = Nothing
        Private WithEvents m_cmdStopMonteCarlo As cCommand = Nothing
        Private WithEvents m_cmdLoadTS As cCommand = Nothing

        ''' <summary>Live monitoring of Ecosim NYears</summary>
        Private m_propNYears As cSingleProperty = Nothing

        Private m_fpNumTrials As cEwEFormatProvider = Nothing
        Private m_fpTrial As cEwEFormatProvider = Nothing
        Private m_fpERun As cEwEFormatProvider = Nothing
        Private m_fpSSorg As cEwEFormatProvider = Nothing
        Private m_fpSS As cEwEFormatProvider = Nothing
        Private m_fpSSBest As cEwEFormatProvider = Nothing

        Private m_lpplIteration As New List(Of PointPairList)

        ''' <summary>
        '''  Local counter for the number of trials run
        ''' </summary>
        ''' <remarks>Zeroed when the MC completes its run MonteCarloCompletedHandler(), incremented in newRun(). 
        ''' We can not use the MC counter because it is not zeroed until the run is started by the MC. 
        ''' We need to know what run it about to happen before the run so we can store the local data.
        ''' </remarks>
        Private m_nTrials As Integer

#End Region ' Private vars

#Region " Constructor "

        Public Sub New()

            ' This call is required by the Windows Form Designer.
            Me.InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.
            Me.m_core = cCore.GetInstance
            Me.m_mcmanager = Me.m_core.EcosimMonteCarlo
            Me.m_mcmanager.Load()

            'set the call back delegates for the monte carlo trials and ecopath iteration
            Me.m_mcmanager.MonteCarloStepHandler = AddressOf MonteCarloStepHandler
            Me.m_mcmanager.MonteCarloEcopathStepHandler = AddressOf Me.MonteCarloEcopathStepHandler
            Me.m_mcmanager.MonteCarloCompletedHandler = AddressOf Me.MonteCarloCompletedHandler
            Me.m_mcmanager.EcosimTimeStepHandler = AddressOf Me.EcoSimTimeStepHandler
            Me.m_mcmanager.SyncObject = Me

        End Sub

#End Region ' Constructor

#Region " Events "

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)

            ' Set via designer
            'm_gridB.DisplayInputValue = eMCRunDisplayInputValueTypes.B
            'm_gridPB.DisplayInputValue = eMCRunDisplayInputValueTypes.PB
            'm_gridEE.DisplayInputValue = eMCRunDisplayInputValueTypes.EE
            'm_gridBA.DisplayInputValue = eMCRunDisplayInputValueTypes.BA

            Me.m_fpNumTrials = New cEwEFormatProvider(Me.nudNumTrials, GetType(Integer))
            Me.m_fpNumTrials.Value = m_mcmanager.nTrials

            Me.m_fpTrial = New cEwEFormatProvider(Me.lblValueTrial, GetType(Integer))
            Me.m_fpTrial.Value = 0

            Me.m_fpERun = New cEwEFormatProvider(Me.lblValueERun, GetType(Integer))
            Me.m_fpERun.Value = 0

            Me.m_fpSSorg = New cEwEFormatProvider(Me.lblValueSSOrg, GetType(Single))
            Me.m_fpSSorg.Value = Me.m_mcmanager.SSorg

            Me.m_fpSS = New cEwEFormatProvider(Me.lblValueSS, GetType(Single))
            Me.m_fpSS.Value = 0.0!

            Me.m_fpSSBest = New cEwEFormatProvider(Me.lblValueSSBest, GetType(Single))
            Me.m_fpSSBest.Value = 0.0!

            Me.m_mcmanager.bShowPlot = cbShowBioTraj.Checked
            ' me.m_mcManager.UseFishingPattern = cbRetainCurPattern.Checked
            Me.m_mcmanager.bRetainFits = cbRetainEstimates.Checked

            Me.m_plothelper = New cEcosimOutputPlotHelper()
            Me.m_plothelper.Attach(Me.m_core, Me.m_graph)
            Me.m_plothelper.ShowMultipleRuns = True
            ' ToDo_JS: localize this
            Me.m_plothelper.ConfigurePane("Monte carlo trials", "Time", "Biomass", False)
            Me.m_plothelper.AutoScaleOption() = cZedGraphHelper.ScaleOptions.Both

            Me.m_cmdRunMonteCarlo = New cCommand("RunMonteCarlo")
            Me.m_cmdRunMonteCarlo.AddControl(Me.btnRunTrials)
            cCommandHandler.GetInstance().Add(Me.m_cmdRunMonteCarlo)

            Me.m_cmdStopMonteCarlo = New cCommand("StopMonteCarlo")
            Me.m_cmdStopMonteCarlo.AddControl(Me.btnStop)
            cCommandHandler.GetInstance().Add(Me.m_cmdStopMonteCarlo)

            ' Connect to ApplyTS command
            Me.m_cmdLoadTS = cCommandHandler.GetInstance().GetCommand("LoadTimeSeries")
            If Me.m_cmdLoadTS IsNot Nothing Then Me.m_cmdLoadTS.AddControl(Me.btnTS)

            Me.m_propNYears = New cSingleProperty(m_core.EcoSimModelParameters, eVarNameFlags.EcoSimNYears)
            AddHandler Me.m_propNYears.PropertyChanged, AddressOf OnPropNumYearsChanged

            Debug.Assert(Me.m_cmdLoadTS IsNot Nothing, "Command failed to load.")

            Me.m_lbGroups.Attach(Me.m_core, cStyleGuide.GetInstance())

            Me.CoreComponents = New eCoreComponentType() {eCoreComponentType.EcoSim}
            Me.PopulateGroupBox()

        End Sub

        Protected Overrides Sub OnFormClosed(ByVal e As System.Windows.Forms.FormClosedEventArgs)

            cCommandHandler.GetInstance().Remove(Me.m_cmdRunMonteCarlo)
            cCommandHandler.GetInstance().Remove(Me.m_cmdStopMonteCarlo)

            ' Disconnect from ApplyTS command
            Dim cmd As cCommand = cCommandHandler.GetInstance().GetCommand("WeightTimeSeries")
            If cmd IsNot Nothing Then cmd.RemoveControl(Me.btnTS)

            Me.m_lbGroups.Detach()

            ' Disconnect from property
            RemoveHandler Me.m_propNYears.PropertyChanged, AddressOf OnPropNumYearsChanged
            Me.m_propNYears = Nothing

            Me.m_plothelper.Detach()

            Me.m_fpERun.Release()
            Me.m_fpNumTrials.Release()
            Me.m_fpSS.Release()
            Me.m_fpSSBest.Release()
            Me.m_fpSSorg.Release()
            Me.m_fpTrial.Release()

            Me.CoreComponents = Nothing

            MyBase.OnFormClosed(e)

        End Sub

        Private Sub MCRun_Activated(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Activated
            Me.m_mcmanager.EcosimTimeStepHandler = AddressOf Me.EcoSimTimeStepHandler
        End Sub

        Private Sub MCRun_Deactivate(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Deactivate
            Me.m_mcmanager.EcosimTimeStepHandler = Nothing
        End Sub

        Private Sub btnStop_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnStop.Click
            Me.m_mcmanager.StopRun()
        End Sub

        Private Sub btApply_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btApply.Click
            Me.m_mcmanager.ApplyBestFits()
        End Sub

        Private Sub cbShowBioTraj_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles cbShowBioTraj.CheckedChanged
            If Not Me.m_mcmanager Is Nothing Then
                Me.m_mcmanager.bShowPlot = cbShowBioTraj.Checked
            End If
        End Sub

        Private Sub cbRetainCurPattern_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles cbRetainCurPattern.CheckedChanged
            If Not Me.m_mcmanager Is Nothing Then
                ' me.m_mcManager.UseFishingPattern = cbRetainCurPattern.Checked
            End If
        End Sub

        Private Sub cbRetainEstimates_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles cbRetainEstimates.CheckedChanged
            If Not Me.m_mcmanager Is Nothing Then
                Me.m_mcmanager.bRetainFits = cbRetainEstimates.Checked
            End If
        End Sub

        Private Sub nudNumTrials_ValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
            Handles nudNumTrials.ValueChanged
            If Me.m_mcmanager IsNot Nothing Then
                Try
                    Me.m_mcmanager.nTrials = CInt(Me.nudNumTrials.Value)
                Catch ex As Exception
                End Try
            End If
        End Sub

#End Region ' Events

#Region " MC Run callbacks "

        Private Sub MonteCarloStepHandler()

            Try
                cApplicationStatusNotifier.SetStatusText(My.Resources.STATUS_SEARCH_SEARCHING, TriState.UseDefault, _
                                  Me.m_mcmanager.nTrialIterations / Me.m_mcmanager.nTrials)

                Me.m_fpTrial.Value = Me.m_mcmanager.nTrialIterations
                Me.m_fpSS.Value = Me.m_mcmanager.SS
                Me.m_fpSSBest.Value = Me.m_mcmanager.SSBestFit

                'this will draw the currently loaded data
                Me.UpdateGraphHighlights()

                'get ready for the next run if there isn't one then on big deal the data will not be used
                Me.newRun()

            Catch ex As Exception
                Debug.Assert(False, ex.StackTrace)
            End Try

        End Sub

        Private Sub MonteCarloEcopathStepHandler()

            Try
                Me.m_fpERun.Value = Me.m_mcmanager.nEcopathIterations
            Catch ex As Exception
                Debug.Assert(False, ex.StackTrace)
            End Try

        End Sub

        Private Sub MonteCarloCompletedHandler()

            cApplicationStatusNotifier.SetStatusText("", TriState.False)

            Me.m_nTrials = 0

            Try
                Me.btApply.Enabled = True
                Me.cbRetainEstimates.Enabled = True
                Me.cbShowBioTraj.Enabled = True
                Me.nudNumTrials.Enabled = True
                Me.btnTS.Enabled = True

                'populate the grid with new values (biomass....)
                Me.m_gridBestFit.RefreshData()

                ' Select outputs
                Me.m_tcOutput.SelectedTab = m_tbpBestTrial

            Catch ex As Exception
                Debug.Assert(False, ex.StackTrace)
            End Try

        End Sub

        ''' <summary>
        ''' Time Step handler for Ecosim results
        ''' </summary>
        ''' <remarks>This will be called at each ecosim timestep for plotting the data</remarks>
        Private Sub EcoSimTimeStepHandler(ByVal lTime As Long, ByVal results As cEcoSimResults)

            Dim ppl As PointPairList = Nothing

            If (Me.m_lpplIteration.Count = 0) Then Return

            Try

                ' Store results
                For iGroup As Integer = 1 To Me.m_core.nLivingGroups
                    ppl = Me.m_lpplIteration(iGroup - 1)
                    ppl.Add(New PointPair(Me.m_core.EcosimFirstYear + CInt(lTime), results.Biomass(iGroup)))
                Next

            Catch ex As Exception

            End Try
        End Sub

#End Region ' MC Run callbacks

#Region " Run Trials "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Command handler; executes the 
        ''' <see cref="m_cmdRunMonteCarlo">Run Monte Carlo command</see>.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub m_cmdRunMonteCarlo_OnInvoke(ByVal cmd As EwEUtils.Commands.cCommand) _
            Handles m_cmdRunMonteCarlo.OnInvoke

            Me.btApply.Enabled = False
            Me.cbRetainEstimates.Enabled = False
            Me.cbShowBioTraj.Enabled = False
            Me.nudNumTrials.Enabled = False
            Me.btnTS.Enabled = False

            If Me.m_mcmanager.bShowPlot Then
                ' Select biomass plot page.
                Me.m_tcOutput.SelectedTab = Me.m_tbpBPlot
            End If

            Me.m_fpSSorg.Value = Me.m_mcmanager.SSorg
            Me.m_fpTrial.Value = 0
            Me.m_fpERun.Value = 0
            Me.m_fpSS.Value = 0.0!
            Me.m_fpSSBest.Value = 0.0!

            cApplicationStatusNotifier.SetStatusText(My.Resources.STATUS_SEARCH_INITIALIZING, TriState.True)

            'clear out the old data
            Me.m_plothelper.Clear()

            Me.newRun()
            Me.m_mcmanager.Run()

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Command update handler; enables and disables the 
        ''' <see cref="m_cmdRunMonteCarlo">Run Monte Carlo command</see>.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub m_cmdRunMonteCarlo_OnUpdate(ByVal cmd As EwEUtils.Commands.cCommand) _
            Handles m_cmdRunMonteCarlo.OnUpdate

            cmd.Enabled = Me.m_core.StateMonitor.HasEcosimLoaded() And _
                          Me.m_core.HasAppliedTimeSeries() And _
                          Not Me.m_mcmanager.isRunning

            If m_core.HasAppliedTimeSeries() Then
                ' JS 11dec07: is this necessary?
                Me.m_fpSSorg.Value = Me.m_mcmanager.SSorg
            End If

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Command handler; executes the 
        ''' <see cref="m_cmdStopMonteCarlo">Stop Monte Carlo command</see>.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub m_cmdStopMonteCarlo_OnInvoke(ByVal cmd As EwEUtils.Commands.cCommand) Handles m_cmdStopMonteCarlo.OnInvoke
            m_mcmanager.StopRun()
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Command update handler; enables and disables the 
        ''' <see cref="m_cmdStopMonteCarlo">Stop Monte Carlo command</see>.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub m_cmdStopMonteCarlo_OnUpdate(ByVal cmd As EwEUtils.Commands.cCommand) Handles m_cmdStopMonteCarlo.OnUpdate
            cmd.Enabled = Me.m_mcmanager.isRunning
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' The Apply time series Command/button has been invoked
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub m_cmdApplyTS_OnPostInvoke(ByVal cmd As EwEUtils.Commands.cCommand) _
            Handles m_cmdLoadTS.OnPostInvoke
            'this means the time series data could have changed
            'reload the data into the manager
            Me.m_mcmanager.Load()
            Me.UpdateGraphXAxis()
        End Sub

        Private Sub OnPropNumYearsChanged(ByVal prop As cProperty, ByVal changeFlags As cProperty.eChangeFlags)
            Me.UpdateGraphXAxis()
        End Sub

        Private Sub m_lbGroups_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_lbGroups.SelectedIndexChanged
            Me.UpdateGraphHighlights()
        End Sub

        Private Sub UpdateGraphXAxis()
            Me.m_graph.GraphPane.XAxis.Scale.Min = Me.m_core.EcosimFirstYear
            Me.m_graph.GraphPane.XAxis.Scale.Max = Me.m_core.EcoSimModelParameters.NumberYears + Me.m_core.EcosimFirstYear
            Me.m_graph.AxisChange()
        End Sub

        Private Sub UpdateGraphHighlights()

            'Only Highlight if the graphs are drawing
            If Me.m_mcmanager.bShowPlot Then


                ' Start setting highlights
                Me.m_plothelper.ClearHighlights()

                'Dim gi As cGroupListBox.cGroupItem = Nothing
                'For Each item As Object In Me.m_lbGroups.SelectedItems
                '    If TypeOf (item) Is cGroupListBox.cGroupItem Then
                '        gi = DirectCast(item, cGroupListBox.cGroupItem)
                '        Me.m_plothelper.Highlight(gi.Group.Index, -1)
                '    End If
                'Next item
                For Each i As Integer In Me.m_lbGroups.SelectedIndices
                    Me.m_plothelper.Highlight(Me.m_lbGroups.GroupIndex(i), -1)
                Next

                Me.m_graph.Invalidate()

            End If

        End Sub

        Private Sub PopulateGroupBox()

            Me.m_lbGroups.GroupListTracking = cGroupListBox.eGroupTrackingType.AllGroups
            Me.m_lbGroups.Populate()

            'Dim group As cCoreGroupBase = Nothing
            'Dim gi As cGroupListBox.cGroupItem = Nothing

            'Me.m_lbGroups.SuspendLayout()
            'Me.m_lbGroups.Items.Clear()

            'For iGroup As Integer = 1 To m_core.nLivingGroups

            '    ' #Yes: add group to the list of options
            '    group = Me.m_core.EcoPathGroupInputs(iGroup)
            '    gi = New cGroupListBox.cGroupItem(group)
            '    Me.m_lbGroups.Items.Add(gi)

            'Next

            Me.m_lbGroups.SelectedIndex = 0
            'Me.m_lbGroups.ResumeLayout()

        End Sub

#End Region ' Run Trials

#Region " Internals "

        Private Sub newRun()

            Me.m_nTrials += 1
            Me.m_plothelper.CreateRun(String.Format("Iteration {0}", Me.m_nTrials))
            Me.m_lpplIteration.Clear()

            If (Me.m_mcmanager.bShowPlot = True) Then

                For iGroup As Integer = 1 To Me.m_core.nLivingGroups
                    Me.m_lpplIteration.Add(New PointPairList())
                Next


                Dim group As cEcoPathGroupInput = Nothing
                For iGroup As Integer = 1 To Me.m_core.nLivingGroups
                    ' Get the ecopath group
                    group = Me.m_core.EcoPathGroupInputs(iGroup)

                    Me.m_plothelper.AddLine(group.Name, iGroup, _
                                            cEcosimOutputPlotHelper.eLineType.RelativeBiomass, _
                                            Me.m_lpplIteration(iGroup - 1))

                Next iGroup

            End If

        End Sub

#End Region ' Internals

    End Class

End Namespace
