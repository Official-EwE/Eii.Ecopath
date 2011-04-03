#Region " Imports "

Option Explicit On
Option Strict On

Imports System.Windows.Forms
Imports EwECore
Imports EwEUtils.Commands
Imports EwEUtils.Core
Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports ZedGraph

#End Region

Namespace Ecosim

    ''' <summary>
    ''' Form class that implements the Ecosim Monte Carlo interface.
    ''' </summary>
    Public Class MCRun

#Region " Private vars "

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
        ''' Local counter for the number of trials run
        ''' </summary>
        ''' <remarks>Zeroed when the MC completes its run MonteCarloCompletedHandler(), incremented in newRun(). 
        ''' We can not use the MC counter because it is not zeroed until the run is started by the MC. 
        ''' We need to know what run it about to happen before the run so we can store the local data.
        ''' </remarks>
        Private m_nTrials As Integer

        Private m_sYMax As Single = 1.0!

#End Region ' Private vars

#Region " Constructor "

        Public Sub New()

            Me.InitializeComponent()

        End Sub

#End Region ' Constructor

#Region " Events "

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)

            If (Me.UIContext Is Nothing) Then Return

            ' Add any initialization after the InitializeComponent() call.
            Me.m_mcmanager = Me.Core.EcosimMonteCarlo
            Me.m_mcmanager.Load()

            'set the call back delegates for the monte carlo trials and ecopath iteration
            Me.m_mcmanager.MonteCarloStepHandler = AddressOf MonteCarloStepHandler
            Me.m_mcmanager.MonteCarloEcopathStepHandler = AddressOf Me.MonteCarloEcopathStepHandler
            Me.m_mcmanager.MonteCarloCompletedHandler = AddressOf Me.MonteCarloCompletedHandler
            Me.m_mcmanager.EcosimTimeStepHandler = AddressOf Me.EcoSimTimeStepHandler
            Me.m_mcmanager.SyncObject = Me

            Me.m_fpNumTrials = New cEwEFormatProvider(Me.UIContext, Me.m_nudNumTrials, GetType(Integer))
            Me.m_fpNumTrials.Value = m_mcmanager.nTrials

            Me.m_fpTrial = New cEwEFormatProvider(Me.UIContext, Me.lblValueTrial, GetType(Integer))
            Me.m_fpTrial.Value = 0

            Me.m_fpERun = New cEwEFormatProvider(Me.UIContext, Me.lblValueERun, GetType(Integer))
            Me.m_fpERun.Value = 0

            Me.m_fpSSorg = New cEwEFormatProvider(Me.UIContext, Me.lblValueSSOrg, GetType(Single))
            Me.m_fpSSorg.Value = Me.m_mcmanager.SSorg

            Me.m_fpSS = New cEwEFormatProvider(Me.UIContext, Me.lblValueSS, GetType(Single))
            Me.m_fpSS.Value = 0.0!

            Me.m_fpSSBest = New cEwEFormatProvider(Me.UIContext, Me.lblValueSSBest, GetType(Single))
            Me.m_fpSSBest.Value = 0.0!

            Me.m_mcmanager.bShowPlot = m_cbShowBioTraj.Checked
            ' me.m_mcManager.UseFishingPattern = cbRetainCurPattern.Checked
            Me.m_mcmanager.bRetainFits = m_cbRetainEstimates.Checked

            Me.m_plothelper = New cEcosimOutputPlotHelper()
            Me.m_plothelper.Attach(Me.UIContext, Me.m_graph)
            Me.m_plothelper.ShowMultipleRuns = True

            Me.m_plothelper.ConfigurePane(SharedResources.HEADER_MCTRIALS, SharedResources.HEADER_TIME, SharedResources.HEADER_BIOMASS, False)
            Me.m_plothelper.AutoScaleYOption = cZedGraphHelper.eScaleOptionTypes.Both

            ' Configure grids
            Me.m_gridB.UIContext = Me.UIContext
            Me.m_gridBA.UIContext = Me.UIContext
            Me.m_gridEE.UIContext = Me.UIContext
            Me.m_gridPB.UIContext = Me.UIContext
            Me.m_gridBestFit.UIContext = Me.UIContext

            Me.m_cmdRunMonteCarlo = New cCommand(Me.CommandHandler, "RunMonteCarlo")
            Me.m_cmdRunMonteCarlo.AddControl(Me.m_btnRunTrials)

            Me.m_cmdStopMonteCarlo = New cCommand(Me.CommandHandler, "StopMonteCarlo")
            Me.m_cmdStopMonteCarlo.AddControl(Me.m_btnStop)

            ' Connect to ApplyTS command
            Me.m_cmdLoadTS = Me.CommandHandler.GetCommand("LoadTimeSeries")
            If Me.m_cmdLoadTS IsNot Nothing Then Me.m_cmdLoadTS.AddControl(Me.m_btnTS)

            Me.m_propNYears = New cSingleProperty(Me.Core.EcoSimModelParameters, eVarNameFlags.EcoSimNYears)
            AddHandler Me.m_propNYears.PropertyChanged, AddressOf OnPropNumYearsChanged

            Debug.Assert(Me.m_cmdLoadTS IsNot Nothing, "Command failed to load.")

            Me.m_lbGroups.Attach(Me.UIContext)
            Me.m_lbGroups.SelectedIndex = 0

            Me.CoreComponents = New eCoreComponentType() {eCoreComponentType.EcoSim, eCoreComponentType.EcoSimMonteCarlo}

        End Sub

        Protected Overrides Sub OnFormClosed(ByVal e As System.Windows.Forms.FormClosedEventArgs)

            Me.CommandHandler.Remove(Me.m_cmdRunMonteCarlo)
            Me.CommandHandler.Remove(Me.m_cmdStopMonteCarlo)

            'jb the 'WeightTimeSeries' command was not loaded during OnLoad() 
            ' Disconnect from ApplyTS command
            'Dim cmd As cCommand = Me.CommandHandler.GetCommand("WeightTimeSeries")
            'If cmd IsNot Nothing Then cmd.RemoveControl(Me.m_btnTS)

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

            MyBase.OnFormClosed(e)

        End Sub

        Private Sub btnStop_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles m_btnStop.Click
            Me.m_mcmanager.StopRun()
        End Sub

        Private Sub btApply_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_btnApply.Click
            Me.m_mcmanager.ApplyBestFits()
        End Sub

        Private Sub cbShowBioTraj_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_cbShowBioTraj.CheckedChanged
            If Not Me.m_mcmanager Is Nothing Then
                Me.m_mcmanager.bShowPlot = m_cbShowBioTraj.Checked
            End If
        End Sub

        Private Sub cbRetainCurPattern_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_cbRetainCurPattern.CheckedChanged
            If Not Me.m_mcmanager Is Nothing Then
                ' me.m_mcManager.UseFishingPattern = cbRetainCurPattern.Checked
            End If
        End Sub

        Private Sub cbRetainEstimates_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_cbRetainEstimates.CheckedChanged
            If Not Me.m_mcmanager Is Nothing Then
                Me.m_mcmanager.bRetainFits = m_cbRetainEstimates.Checked
            End If
        End Sub

        Private Sub nudNumTrials_ValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
            Handles m_nudNumTrials.ValueChanged
            If Me.m_mcmanager IsNot Nothing Then
                Try
                    Me.m_mcmanager.nTrials = CInt(Me.m_nudNumTrials.Value)
                Catch ex As Exception
                End Try
            End If
        End Sub

        Private Sub OnLoadFromPedigree(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnFromPedigree.Click
            ' Load parms from pedigree
            Me.m_mcmanager.LoadFromPedigree()
        End Sub

#End Region ' Events

#Region " MC Run callbacks "

        Private Sub MonteCarloStepHandler()

            Try
                ' Be conservative in providing status feedback
                If (Me.m_mcmanager.nTrialIterations Mod cCore.N_MONTHS = 0) Then
                    cApplicationStatusNotifier.UpdateProgress(Me.Core, _
                                                              My.Resources.STATUS_SEARCH_SEARCHING, _
                                                              Me.m_mcmanager.nTrialIterations / Me.m_mcmanager.nTrials)
                End If

                Me.m_fpTrial.Value = Me.m_mcmanager.nTrialIterations
                Me.m_fpSS.Value = Me.m_mcmanager.SS
                Me.m_fpSSBest.Value = Me.m_mcmanager.SSBestFit

                'this will draw the currently loaded data
                Me.UpdateGraphHighlights()

                'get ready for the next run if there isn't one then on big deal the data will not be used
                Me.NewIteration()

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

            cApplicationStatusNotifier.EndProgress(Me.Core)

            Me.m_nTrials = 0

            Try
                Me.m_btnApply.Enabled = True
                Me.m_cbRetainEstimates.Enabled = True
                Me.m_cbShowBioTraj.Enabled = True
                Me.m_nudNumTrials.Enabled = True
                Me.m_btnTS.Enabled = True

                'populate the grid with new values (biomass....)
                Me.m_gridBestFit.RefreshContent()

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
                For iGroup As Integer = 1 To Me.Core.nLivingGroups
                    ppl = Me.m_lpplIteration(iGroup - 1)
                    ppl.Add(New PointPair(Me.Core.EcosimFirstYear + CSng(lTime / cCore.N_MONTHS), results.Biomass(iGroup)))
                    Me.m_sYMax = Math.Max(Me.m_sYMax, results.Biomass(iGroup))
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

            Me.m_btnApply.Enabled = False
            Me.m_cbRetainEstimates.Enabled = False
            Me.m_cbShowBioTraj.Enabled = False
            Me.m_nudNumTrials.Enabled = False
            Me.m_btnTS.Enabled = False

            If Me.m_mcmanager.bShowPlot Then
                ' Select biomass plot page.
                Me.m_tcOutput.SelectedTab = Me.m_tbpBPlot
            End If

            Me.m_fpSSorg.Value = Me.m_mcmanager.SSorg
            Me.m_fpTrial.Value = 0
            Me.m_fpERun.Value = 0
            Me.m_fpSS.Value = 0.0!
            Me.m_fpSSBest.Value = 0.0!
            Me.m_sYMax = 1.0!

            cApplicationStatusNotifier.EndProgress(Me.Core)

            ' Clear out the old data
            Me.m_plothelper.Clear()

            Me.NewIteration()
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

            cmd.Enabled = Me.Core.StateMonitor.HasEcosimLoaded() And _
                          Me.Core.HasAppliedTimeSeries() And _
                          Not Me.m_mcmanager.IsRunning

            If Me.Core.HasAppliedTimeSeries() Then
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
            cmd.Enabled = Me.m_mcmanager.IsRunning
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
            'jb 14-Mar-2011 MonteCarlo manager does not need to reload if timeseries is loaded
            'Infact this will overwrite user edited Parameter Limit values
            'Me.m_mcmanager.Load()
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

            Dim pane As GraphPane = Me.m_plothelper.GetPane(1)

            With pane.XAxis.Scale
                .Min = Me.Core.EcosimFirstYear
                .Max = Me.Core.EcoSimModelParameters.NumberYears + Me.Core.EcosimFirstYear
                .MaxAuto = False
                .MinAuto = False
            End With

            Me.m_graph.AxisChange()
        End Sub

        Private Sub UpdateGraphHighlights()

            'Only Highlight if the graphs are drawing
            If Me.m_mcmanager.bShowPlot Then

                ' Start setting highlights
                Me.m_plothelper.ClearHighlights()

                For Each i As Integer In Me.m_lbGroups.SelectedIndices
                    Me.m_plothelper.Highlight(Me.m_lbGroups.GroupIndex(i), -1)
                Next

                Me.m_plothelper.YScaleMax = Me.m_sYMax
                Me.m_plothelper.Redraw()

            End If

        End Sub

#End Region ' Run Trials

#Region " Internals "

        Private Sub NewIteration()

            Dim lLines As New List(Of LineItem)

            Me.m_nTrials += 1
            Me.m_plothelper.CreateRun(String.Format(SharedResources.GENERIC_VALUE_ITERATION, Me.m_nTrials))
            Me.m_lpplIteration.Clear()

            If (Me.m_mcmanager.bShowPlot = True) Then

                For iGroup As Integer = 1 To Me.Core.nLivingGroups
                    Me.m_lpplIteration.Add(New PointPairList())
                Next

                For iGroup As Integer = 1 To Me.Core.nLivingGroups
                    Dim group As cEcoPathGroupInput = Me.Core.EcoPathGroupInputs(iGroup)
                    Dim strGroupName As String = String.Format(SharedResources.GENERIC_LABEL_INDEXED, iGroup, group.Name)
                    Dim strTrialLabel As String = String.Format(My.Resources.GENERIC_LABEL_TRIAL, Me.m_nTrials, strGroupName)
                    lLines.Add(Me.m_plothelper.CreateLine(group, Me.m_lpplIteration(iGroup - 1), strTrialLabel))
                Next iGroup

            End If
            Me.m_plothelper.YScaleMax = Me.m_sYMax
            Me.m_plothelper.PlotLines(lLines.ToArray, 1, True, False)

        End Sub

#End Region ' Internals

    End Class

End Namespace
