'==============================================================================
'
' $Log: MCRun.vb,v $
' Revision 1.9  2009/06/24 18:31:16  jeroens
' Enabled to be maintained in the designer
'
' Revision 1.8  2009/06/23 18:17:30  jeroens
' Fixed layout: SS no longer drops off of screen
' Fixed panel padding
' Uses central progress feedback structure
'
' Revision 1.7  2009/05/13 13:59:53  jeroens
' Renamed ecosim plot classes
'
' Revision 1.6  2009/05/11 01:50:59  jeroens
' Renamed command classes
'
' Revision 1.5  2009/03/19 16:02:26  jeroens
' Added FormatProvider.Release
'
' Revision 1.4  2009/01/16 23:46:21  jeroens
' Fixed ApplyTimeSeries outdated name bug
'
' Revision 1.3  2008/12/15 15:55:37  jeroens
' no message
'
' Revision 1.2  2008/11/25 02:16:26  sherman
' Added feedback onto monte carlo progress bar.
'
' Revision 1.1  2008/09/26 07:31:47  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.32  2008/07/18 17:51:19  jeroens
' Updated to new ZedGraphHelper interface
'
' Revision 1.31  2008/06/02 00:01:33  jeroens
' Added ScientificInterfaceShared
'
' Revision 1.30  2008/05/29 22:22:54  jeroens
' Moved eVarNameFlags to EwEUtils
'
' Revision 1.29  2008/05/05 22:21:26  jeroens
' Shared progress bar
'
' Revision 1.28  2007/12/14 15:48:39  jeroens
' * Updated to new way of controlling output graph
'
' Revision 1.27  2007/12/11 17:04:49  jeroens
' * Uses format providers to present output, resolves pending globalization issues
' * Time series btn links to load, not just apply
'
' Revision 1.26  2007/12/10 04:09:17  jeroens
' * Cleaned-up GUI
' * Uses EwEformatprovider
' * Numbers formatted via StyleGuide
' * Balanced dynamically assigned handlers
' + Identified pending globalization issues
'
' Revision 1.25  2007/10/09 18:58:56  joeb
' Progress bar
'
' Revision 1.24  2007/10/05 19:09:55  joeb
' Graph resizes in response to number of years changing
'
' Revision 1.23  2007/10/05 18:15:14  joeb
' Added BatchMode to biomass graph
'
' Revision 1.22  2007/09/29 01:17:00  joeb
' Bug Fixes
'
' Revision 1.21  2007/09/28 18:55:17  joeb
' changed number of years
'
' Revision 1.20  2007/08/24 22:54:43  fgao
' Add a progress bar ... Temporary test for incremental drawing..
'
' Revision 1.19  2007/08/10 23:23:41  fgao
' Finish ucBiomassPlot, make them work for both MCRun and RunEcosim UI,
' Add annual plot options etc.
'
' Revision 1.18  2007/08/10 00:38:48  fgao
' Keep on adding and debugging more function
' s
'
' Revision 1.17  2007/08/09 21:22:01  fgao
' Add More MCRun functions..
'
' Revision 1.16  2007/08/08 23:22:44  fgao
' Add refresh data public interface. We need to populate data again after
' MCRun ends..
'
' Revision 1.15  2007/08/07 17:52:19  jeroens
' * Fully connected Run, Stop button
' + Connected ApplyTS button to global command
'
' Revision 1.14  2007/08/03 23:46:49  fgao
' Improved a lot in biomass rendering speed.
'
' Revision 1.13  2007/08/01 23:42:44  fgao
' Add MC Run plot now....
'
' Revision 1.12  2007/08/01 23:14:44  fgao
' Removed conflicting code
'
' Revision 1.11  2007/07/31 16:03:42  jeroens
' * Run and Stop handled via central commands
'
'==============================================================================

#Region " Imports "

Option Explicit On
Option Strict On

Imports System.Windows.Forms
Imports EwECore
Imports EwEUtils.Commands
Imports EwEUtils.Core

#End Region

Namespace Ecosim

    ''' <summary>
    ''' Form class that implements the Ecosim Monte Carlo interface.
    ''' </summary>
    Public Class MCRun

#Region " Private vars "

        Private m_core As EwECore.cCore
        Private m_mcmanager As cMonteCarloManager
        Private m_as2BiomassResults(,) As Single

        Private WithEvents m_cmdRunMonteCarlo As cCommand = Nothing
        Private WithEvents m_cmdStopMonteCarlo As cCommand = Nothing
        Private WithEvents m_cmdLoadTS As cCommand = Nothing

        ''' <summary>Live monitoring of Ecosim NYears</summary>
        Private m_pTS As cSingleProperty = Nothing

        Private m_ucBPlots As New ucEcosimOutputPlotOLD
        Private m_fpNumTrials As cEwEFormatProvider = Nothing
        Private m_fpTrial As cEwEFormatProvider = Nothing
        Private m_fpERun As cEwEFormatProvider = Nothing
        Private m_fpSSorg As cEwEFormatProvider = Nothing
        Private m_fpSS As cEwEFormatProvider = Nothing
        Private m_fpSSBest As cEwEFormatProvider = Nothing

#End Region ' Private vars

#Region " Constructor "

        Public Sub New()

            ' This call is required by the Windows Form Designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.
            Me.m_core = cCore.GetInstance
            Me.m_mcmanager = Me.m_core.EcosimMonteCarlo
            Me.m_mcmanager.Load()

            Me.m_ucBPlots = New ucEcosimOutputPlotOLD()
            Me.m_ucBPlots.Dock = DockStyle.Fill
            Me.m_ucBPlots.Plot.IsSummaryLinesShown = False
            Me.m_ucBPlots.BatchMode = True

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
            Me.m_tbpBPlot.Controls.Add(Me.m_ucBPlots)

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

            m_mcmanager.bShowPlot = cbShowBioTraj.Checked
            ' m_mcManager.UseFishingPattern = cbRetainCurPattern.Checked
            m_mcmanager.bRetainFits = cbRetainEstimates.Checked

            Me.m_cmdRunMonteCarlo = New cCommand("RunMonteCarlo")
            Me.m_cmdRunMonteCarlo.AddControl(Me.btnRunTrials)
            cCommandHandler.GetInstance().Add(Me.m_cmdRunMonteCarlo)

            Me.m_cmdStopMonteCarlo = New cCommand("StopMonteCarlo")
            Me.m_cmdStopMonteCarlo.AddControl(Me.btnStop)
            cCommandHandler.GetInstance().Add(Me.m_cmdStopMonteCarlo)

            ' Connect to ApplyTS command
            m_cmdLoadTS = cCommandHandler.GetInstance().GetCommand("LoadTimeSeries")
            If m_cmdLoadTS IsNot Nothing Then m_cmdLoadTS.AddControl(Me.btnTS)

            Me.m_pTS = New cSingleProperty(m_core.EcoSimModelParameters, eVarNameFlags.EcoSimNYears)
            AddHandler Me.m_pTS.PropertyChanged, AddressOf m_pTS_PropertyChanged

            Debug.Assert(m_cmdLoadTS IsNot Nothing, "Command failed to load.")

            Me.CoreComponents = New eCoreComponentType() {eCoreComponentType.EcoSim}
        End Sub

        Protected Overrides Sub OnFormClosing(ByVal e As System.Windows.Forms.FormClosingEventArgs)

            cCommandHandler.GetInstance().Remove(Me.m_cmdRunMonteCarlo)
            cCommandHandler.GetInstance().Remove(Me.m_cmdStopMonteCarlo)

            ' Disconnect from ApplyTS command
            Dim cmd As cCommand = cCommandHandler.GetInstance().GetCommand("WeightTimeSeries")
            If cmd IsNot Nothing Then cmd.RemoveControl(Me.btnTS)

            ' Disconnect from property
            RemoveHandler Me.m_pTS.PropertyChanged, AddressOf m_pTS_PropertyChanged
            Me.m_pTS = Nothing

            Me.m_fpERun.Release()
            Me.m_fpNumTrials.Release()
            Me.m_fpSS.Release()
            Me.m_fpSSBest.Release()
            Me.m_fpSSorg.Release()
            Me.m_fpTrial.Release()

            Me.CoreComponents = Nothing
        End Sub

        Private Sub MCRun_Activated(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Activated
            Me.m_mcmanager.EcosimTimeStepHandler = AddressOf Me.EcoSimTimeStepHandler
        End Sub

        Private Sub MCRun_Deactivate(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Deactivate
            Me.m_mcmanager.EcosimTimeStepHandler = Nothing
        End Sub

        Private Sub btnStop_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnStop.Click
            m_mcmanager.StopRun()
        End Sub

        Private Sub btApply_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btApply.Click
            m_mcmanager.ApplyBestFits()
        End Sub

        Private Sub cbShowBioTraj_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbShowBioTraj.CheckedChanged
            If Not m_mcmanager Is Nothing Then
                m_mcmanager.bShowPlot = cbShowBioTraj.Checked
            End If
        End Sub

        Private Sub cbRetainCurPattern_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbRetainCurPattern.CheckedChanged
            If Not m_mcmanager Is Nothing Then
                ' m_mcManager.UseFishingPattern = cbRetainCurPattern.Checked
            End If
        End Sub

        Private Sub cbRetainEstimates_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbRetainEstimates.CheckedChanged
            If Not m_mcmanager Is Nothing Then
                m_mcmanager.bRetainFits = cbRetainEstimates.Checked
            End If
        End Sub

        Private Sub txbNumTrials_Validated(ByVal sender As System.Object, ByVal e As System.EventArgs)
            m_mcmanager.nTrials = CInt(Me.m_fpNumTrials.Value)
        End Sub

#End Region ' Events

#Region " MC Run callbacks "

        Private Sub MonteCarloStepHandler()

            Dim asn As cApplicationStatusNotifier = cApplicationStatusNotifier.GetInstance()

            Try
                ' ToDo: localize this
                asn.SetStatusText(My.Resources.STATUS_SEARCH_SEARCHING, TriState.UseDefault, _
                                  Me.m_mcmanager.nTrialIterations / Me.m_mcmanager.nTrials)
            Catch ex As Exception
                Debug.Assert(False, ex.StackTrace)
            End Try

            Me.m_fpTrial.Value = Me.m_mcmanager.nTrialIterations
            Me.m_fpSS.Value = Me.m_mcmanager.SS
            Me.m_fpSSBest.Value = Me.m_mcmanager.SSBestFit

            'Plot the graph 
            If ((Me.m_mcmanager.bShowPlot = True) And (Me.m_as2BiomassResults IsNot Nothing)) Then
                If (Me.m_mcmanager.nTrialIterations = Me.m_mcmanager.nTrials) Then
                    Me.m_ucBPlots.Plot.IsTSDataShown = True
                Else
                    Me.m_ucBPlots.Plot.IsTSDataShown = False
                End If
                Me.m_ucBPlots.AddValues(Me.m_as2BiomassResults)
            End If

        End Sub

        Private Sub MonteCarloEcopathStepHandler()

            Try
                Me.m_fpERun.Value = Me.m_mcmanager.nEcopathIterations
            Catch ex As Exception
                Debug.Assert(False, ex.StackTrace)
            End Try

        End Sub

        Private Sub MonteCarloCompletedHandler()

            Dim asn As cApplicationStatusNotifier = cApplicationStatusNotifier.GetInstance()

            Try
                Me.btApply.Enabled = True
                ' Select outputs
                Me.tcMCOutput.SelectedTab = m_tbpBestTrial
                'populate the grid with new values (biomass....)
                Me.m_gridBestFit.RefreshData()
                Me.m_ucBPlots.EnableControls(True)
                asn.SetStatusText("", TriState.False)
            Catch ex As Exception
                Debug.Assert(False, ex.StackTrace)
            End Try

        End Sub

        ''' <summary>
        ''' Time Step handler for Ecosim results
        ''' </summary>
        ''' <remarks>This will be called at each ecosim timestep for plotting the data</remarks>
        Private Sub EcoSimTimeStepHandler(ByVal iTime As Long, ByVal results As cEcoSimResults)

            If m_mcmanager.bShowPlot Then
                If iTime = 1 Then
                    ReDim m_as2BiomassResults(m_core.nGroups, m_core.nEcosimTimeSteps)
                End If
                For groupIndex As Integer = 1 To results.nGroups
                    m_as2BiomassResults(groupIndex, CInt(iTime)) = results.Biomass(groupIndex)
                Next
            End If

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
            ' Select biomass plot page.
            Me.tcMCOutput.SelectedTab = Me.m_tbpBPlot

            Me.m_fpSSorg.Value = Me.m_mcmanager.SSorg
            Me.m_fpTrial.Value = 0
            Me.m_fpERun.Value = 0
            Me.m_fpSS.Value = 0.0!
            Me.m_fpSSBest.Value = 0.0!

            cApplicationStatusNotifier.GetInstance().SetStatusText(My.Resources.STATUS_SEARCH_INITIALIZING, _
                                                                   TriState.True)

            Me.m_ucBPlots.Plot.Reset()
            Me.m_ucBPlots.EnableControls(False)

            Me.m_mcmanager.Run()

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Command update handler; enables and disables the 
        ''' <see cref="m_cmdRunMonteCarlo">Run Monte Carlo command</see>.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub m_cmdRunMonteCarlo_OnUpdate(ByVal cmd As EwEUtils.Commands.cCommand) Handles m_cmdRunMonteCarlo.OnUpdate

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
        Private Sub m_cmdApplyTS_OnPostInvoke(ByVal cmd As EwEUtils.Commands.cCommand) Handles m_cmdLoadTS.OnPostInvoke
            'this means the time series data could have changed
            'reload the data into the manager
            Me.m_mcmanager.Load()
            Me.UpdateGraphXAxis()
        End Sub

        Private Sub m_pTS_PropertyChanged(ByVal prop As cProperty, ByVal changeFlags As cProperty.eChangeFlags)
            Me.UpdateGraphXAxis()
        End Sub

        Private Sub UpdateGraphXAxis()
            Try
                'set the xaxis this is the number of years the model ran for
                Me.m_ucBPlots.Plot.XAxis = m_core.nEcosimTimeSteps
                'now what..... hope it draws right next time!
                Me.m_ucBPlots.Plot.GenerateOutputImage()
            Catch ex As Exception
                cLog.Write(ex)
            End Try

        End Sub

#End Region ' Run Trials

#Region " Internals "

#End Region ' Internals

    End Class

End Namespace
