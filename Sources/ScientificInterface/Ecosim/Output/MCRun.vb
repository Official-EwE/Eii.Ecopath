'==============================================================================
'
' $Log: MCRun.vb,v $
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
        Private m_mcManager As cMonteCarloManager
        Private m_BRG As New MCRunInputGrid
        Private m_BARG As New MCRunInputGrid
        Private m_EERG As New MCRunInputGrid
        Private m_PBRG As New MCRunInputGrid
        Private m_BestFitRG As New MCRunOutputGrid
        Private m_BiomassResults(,) As Single

        Private WithEvents m_cmdRunMonteCarlo As Command = Nothing
        Private WithEvents m_cmdStopMonteCarlo As Command = Nothing
        Private WithEvents m_cmdLoadTS As Command = Nothing

        ''' <summary>Live monitoring of Ecosim NYears</summary>
        Private WithEvents m_pTS As cSingleProperty = Nothing

        Private m_ucBPlots As New ucBiomassPlot
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
            m_core = cCore.GetInstance
            m_mcManager = m_core.EcosimMonteCarlo
            m_mcManager.Load()

            'set the call back delegates for the monte carlo trials and ecopath iteration
            m_mcManager.MonteCarloStepHandler = AddressOf MonteCarloStepHandler
            m_mcManager.MonteCarloEcopathStepHandler = AddressOf Me.MonteCarloEcopathStepHandler
            m_mcManager.MonteCarloCompletedHandler = AddressOf Me.MonteCarloCompletedHandler
            m_mcManager.EcosimTimeStepHandler = AddressOf Me.EcoSimTimeStepHandler
            m_mcManager.SyncObject = Me

            m_pTS = New cSingleProperty(m_core.EcoSimModelParameters, eVarNameFlags.EcoSimNYears)

        End Sub

#End Region ' Constructor

#Region " Events "

        Private Sub MCRun_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            m_BRG.DisplayInputValue = MCRunDisplayInputValue.B
            m_PBRG.DisplayInputValue = MCRunDisplayInputValue.PB
            m_EERG.DisplayInputValue = MCRunDisplayInputValue.EE
            m_BARG.DisplayInputValue = MCRunDisplayInputValue.BA

            tbpB.Controls.Add(m_BRG)
            tbpBA.Controls.Add(m_BARG)
            tbpEE.Controls.Add(m_EERG)
            tbpBP.Controls.Add(m_PBRG)
            tbpBestTrial.Controls.Add(m_BestFitRG)

            m_ucBPlots.Plot.IsSummaryLinesShown = False
            m_ucBPlots.BatchMode = True

            tbpBPlot.Controls.Add(m_ucBPlots)

            Me.m_fpNumTrials = New cEwEFormatProvider(Me.nudNumTrials, GetType(Integer))
            Me.m_fpNumTrials.Value = m_mcManager.nTrials

            Me.m_fpTrial = New cEwEFormatProvider(Me.lblValueTrial, GetType(Integer))
            Me.m_fpTrial.Value = 0

            Me.m_fpERun = New cEwEFormatProvider(Me.lblValueERun, GetType(Integer))
            Me.m_fpERun.Value = 0

            Me.m_fpSSorg = New cEwEFormatProvider(Me.lblValueSSOrg, GetType(Single))
            Me.m_fpSSorg.Value = Me.m_mcManager.SSorg

            Me.m_fpSS = New cEwEFormatProvider(Me.lblValueSS, GetType(Single))
            Me.m_fpSS.Value = 0.0!

            Me.m_fpSSBest = New cEwEFormatProvider(Me.lblValueSSBest, GetType(Single))
            Me.m_fpSSBest.Value = 0.0!

            m_mcManager.bShowPlot = cbShowBioTraj.Checked
            ' m_mcManager.UseFishingPattern = cbRetainCurPattern.Checked
            m_mcManager.bRetainFits = cbRetainEstimates.Checked

            Me.m_cmdRunMonteCarlo = New Command("RunMonteCarlo")
            Me.m_cmdRunMonteCarlo.AddControl(Me.btnRunTrials)
            CommandHandler.GetInstance().Add(Me.m_cmdRunMonteCarlo)

            Me.m_cmdStopMonteCarlo = New Command("StopMonteCarlo")
            Me.m_cmdStopMonteCarlo.AddControl(Me.btnStop)
            CommandHandler.GetInstance().Add(Me.m_cmdStopMonteCarlo)

            ' Connect to ApplyTS command
            m_cmdLoadTS = CommandHandler.GetInstance().GetCommand("LoadTimeSeries")
            If m_cmdLoadTS IsNot Nothing Then m_cmdLoadTS.AddControl(Me.btnTS)

            Debug.Assert(m_cmdLoadTS IsNot Nothing, "Command failed to load.")

        End Sub

        Private Sub MCRun_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
            CommandHandler.GetInstance().Remove(Me.m_cmdRunMonteCarlo)
            CommandHandler.GetInstance().Remove(Me.m_cmdStopMonteCarlo)

            ' Disconnect from ApplyTS command
            Dim cmd As Command = CommandHandler.GetInstance().GetCommand("ApplyTimeSeries")
            If cmd IsNot Nothing Then cmd.RemoveControl(Me.btnTS)

            ' Disconnect from property
            Me.m_pTS = Nothing
        End Sub

        Private Sub MCRun_Activated(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Activated
            Me.m_mcManager.EcosimTimeStepHandler = AddressOf Me.EcoSimTimeStepHandler
        End Sub

        Private Sub MCRun_Deactivate(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Deactivate
            Me.m_mcManager.EcosimTimeStepHandler = Nothing
        End Sub

        Private Sub btnStop_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnStop.Click
            m_mcManager.StopRun()
        End Sub

        Private Sub btApply_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btApply.Click
            m_mcManager.ApplyBestFits()
        End Sub

        Private Sub cbShowBioTraj_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbShowBioTraj.CheckedChanged
            If Not m_mcManager Is Nothing Then
                m_mcManager.bShowPlot = cbShowBioTraj.Checked
            End If
        End Sub

        Private Sub cbRetainCurPattern_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbRetainCurPattern.CheckedChanged
            If Not m_mcManager Is Nothing Then
                ' m_mcManager.UseFishingPattern = cbRetainCurPattern.Checked
            End If
        End Sub

        Private Sub cbRetainEstimates_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbRetainEstimates.CheckedChanged
            If Not m_mcManager Is Nothing Then
                m_mcManager.bRetainFits = cbRetainEstimates.Checked
            End If
        End Sub

        Private Sub txbNumTrials_Validated(ByVal sender As System.Object, ByVal e As System.EventArgs)
            m_mcManager.nTrials = CInt(Me.m_fpNumTrials.Value)
        End Sub

#End Region ' Events

#Region " MC Run callbacks "

        Private Sub MonteCarloStepHandler()

            Try
                prgMCTrials.PerformStep()
                If Me.prgMCTrials.Value = Me.prgMCTrials.Maximum Then Me.lblTrialsComplete.Visible = True
            Catch ex As Exception
                Debug.Assert(False, ex.StackTrace)
            End Try

            Me.m_fpTrial.Value = Me.m_mcManager.nTrialIterations
            Me.m_fpSS.Value = Me.m_mcManager.SS
            Me.m_fpSSBest.Value = Me.m_mcManager.SSBestFit

            'Plot the graph 
            If ((m_mcManager.bShowPlot = True) And (m_BiomassResults IsNot Nothing)) Then
                If (m_mcManager.nTrialIterations = m_mcManager.nTrials) Then
                    m_ucBPlots.Plot.IsTSDataShown = True
                Else
                    m_ucBPlots.Plot.IsTSDataShown = False
                End If
                m_ucBPlots.AddValues(m_BiomassResults)

            End If

        End Sub

        Private Sub MonteCarloEcopathStepHandler()

            Try
                Me.m_fpERun.Value = Me.m_mcManager.nEcopathIterations
            Catch ex As Exception
                Debug.Assert(False, ex.StackTrace)
            End Try

        End Sub

        Private Sub MonteCarloCompletedHandler()

            Try
                Me.btApply.Enabled = True
                Me.tcMCOutput.SelectedIndex = 4 ' For best fit page
                'populate the grid with new values (biomass....)
                m_BestFitRG.RefreshData()
                ' m_ucBPlots.tcOutput.Enabled = True
                Me.m_ucBPlots.EnableControls(True)
            Catch ex As Exception
                Debug.Assert(False, ex.StackTrace)
            End Try

        End Sub

        ''' <summary>
        ''' Time Step handler for Ecosim results
        ''' </summary>
        ''' <remarks>This will be called at each ecosim timestep for plotting the data</remarks>
        Private Sub EcoSimTimeStepHandler(ByVal iTime As Long, ByVal results As cEcoSimResults)

            If m_mcManager.bShowPlot Then
                If iTime = 1 Then
                    ReDim m_BiomassResults(m_core.nGroups, m_core.nEcosimTimeSteps)
                End If
                For groupIndex As Integer = 1 To results.nGroups
                    m_BiomassResults(groupIndex, CInt(iTime)) = results.Biomass(groupIndex)
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
        Private Sub m_cmdRunMonteCarlo_OnInvoke(ByVal cmd As EwEUtils.Commands.Command) Handles m_cmdRunMonteCarlo.OnInvoke
            ' m_mcManager.bRetainFits = True

            Me.prgMCTrials.Maximum = m_mcManager.nTrials
            Me.prgMCTrials.Value = 0
            Me.lblTrialsComplete.Visible = False

            Me.btApply.Enabled = False
            Me.tcMCOutput.SelectedIndex = 5 ' For biomass plot page.

            Me.m_fpSSorg.Value = Me.m_mcManager.SSorg
            Me.m_fpTrial.Value = 0
            Me.m_fpERun.Value = 0
            Me.m_fpSS.Value = 0.0!
            Me.m_fpSSBest.Value = 0.0!

            Me.m_ucBPlots.EnableControls(False)
            m_ucBPlots.Plot.Reset()
            m_mcManager.Run()

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Command update handler; enables and disables the 
        ''' <see cref="m_cmdRunMonteCarlo">Run Monte Carlo command</see>.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub m_cmdRunMonteCarlo_OnUpdate(ByVal cmd As EwEUtils.Commands.Command) Handles m_cmdRunMonteCarlo.OnUpdate

            cmd.Enabled = Me.m_core.StateMonitor.HasEcosimLoaded() And _
                          Me.m_core.HasAppliedTimeSeries() And _
                          Not Me.m_mcManager.isRunning

            If m_core.HasAppliedTimeSeries() Then
                ' JS 11dec07: is this necessary?
                Me.m_fpSSorg.Value = Me.m_mcManager.SSorg
            End If

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Command handler; executes the 
        ''' <see cref="m_cmdStopMonteCarlo">Stop Monte Carlo command</see>.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub m_cmdStopMonteCarlo_OnInvoke(ByVal cmd As EwEUtils.Commands.Command) Handles m_cmdStopMonteCarlo.OnInvoke
            m_mcManager.StopRun()
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Command update handler; enables and disables the 
        ''' <see cref="m_cmdStopMonteCarlo">Stop Monte Carlo command</see>.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub m_cmdStopMonteCarlo_OnUpdate(ByVal cmd As EwEUtils.Commands.Command) Handles m_cmdStopMonteCarlo.OnUpdate
            cmd.Enabled = Me.m_mcManager.isRunning
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' The Apply time series Command/button has been invoked
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub m_cmdApplyTS_OnPostInvoke(ByVal cmd As EwEUtils.Commands.Command) Handles m_cmdLoadTS.OnPostInvoke
            'this means the time series data could have changed
            'reload the data into the manager
            Me.m_mcManager.Load()
            Me.changeGraphXAxis()
        End Sub

        Private Sub m_pTS_PropertyChanged(ByVal prop As cProperty, ByVal changeFlags As cProperty.eChangeFlags) Handles m_pTS.PropertyChanged
            Me.changeGraphXAxis()
        End Sub

        Private Sub changeGraphXAxis()
            Try
                'set the xaxis this is the number of years the model ran for
                m_ucBPlots.Plot.XAxis = m_core.nEcosimTimeSteps
                'now what..... hope it draws right next time!
                m_ucBPlots.Plot.GenerateOutputImage()
            Catch ex As Exception
                cLog.Write(ex)
            End Try

        End Sub

#End Region ' Run Trials

#Region " Internals "

#End Region ' Internals

    End Class

End Namespace
