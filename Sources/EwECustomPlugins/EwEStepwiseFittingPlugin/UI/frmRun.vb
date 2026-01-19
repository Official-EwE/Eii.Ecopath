' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 3 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see https://www.gnu.org/licenses/gpl-3.0.html>. 
'
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
'    Scottish Association for Marine Science, Oban, Scotland
'
' Stepwise Fitting Procedure by Sheila Heymans, Erin Scott, Jeroen Steenbeek
' Copyright 2015- Scottish Association for Marine Science, Oban, Scotland
'
' Erin Scott was funded by the Scottish Informatics and Computer Science
' Alliance (SICSA) Postgraduate Industry Internship Programme.
' ===============================================================================
'



Imports System.Windows.Forms
Imports EwECore
Imports EwECore.Common
Imports ScientificInterfaceShared.Commands
Imports ScientificInterfaceShared.Controls
Imports ScientificInterfaceShared.Style
Imports EwEUtils.Logging
Imports Microsoft.Extensions.Logging
Imports Debug = System.Diagnostics.Debug



Public Class frmRun

#Region " Private vars "

    Private Enum eRunMode As Integer
        NotSet = 0
        CommandLine
        StandAlone
        Plugin
    End Enum

    Private m_engine As cSFPManager = Nothing
    Private m_plugin As cSFPPluginPoint = Nothing
    Private m_runmode As eRunMode = eRunMode.NotSet
    Private m_bInUpdate As Boolean = False

    Private WithEvents m_cmdLoadTS As cCommand = Nothing

    Private m_fpK As cEwEFormatProvider = Nothing
    Private m_fpVukCap As cEwEFormatProvider = Nothing
    Private ReadOnly m_logger As ILogger = LoggingContext.CreateLogger(Of frmRun)()

#End Region ' Private vars

#Region " Construction "

    Public Sub New(uic As cUIContext, engine As cSFPManager)

        MyBase.New()

        Me.InitializeComponent()
        Me.UIContext = uic
        Me.m_engine = engine

        Me.Text = My.Resources.DISPLAYNAME
        Me.TabText = Me.Text

        Me.m_runmode = eRunMode.StandAlone

    End Sub

    Public Sub New(uic As cUIContext, engine As cSFPManager, plugin As cSFPPluginPoint)

        Me.New(uic, engine)

        Me.m_plugin = plugin
        Me.m_runmode = eRunMode.Plugin

    End Sub

#End Region ' Construction

#Region " Overrides "

    Protected Overrides Sub OnLoad(e As System.EventArgs)
        MyBase.OnLoad(e)

        Me.m_bInUpdate = True

        Dim parms As cSFPParameters = Me.m_engine.Parameters

        If Me.m_runmode = eRunMode.Plugin Then
            Me.MinimumSize = New Drawing.Size(Me.MinimumSize.Width, Me.MinimumSize.Height - m_plModel.Height)
        End If

        Me.m_grid.UIContext = Me.UIContext
        Me.m_grid.Initialize(Me.m_engine)

        Me.m_btnResetFolder.Image = ScientificInterfaceShared.My.Resources.ResetHS
        Me.m_btnResetFolder.Text = ""

        ' Populate controls
        Me.m_rbPredator.Checked = (parms.VulSearchMode = ISFPIteration.eVulSearchMode.Predator)
        Me.m_rbPredPrey.Checked = (parms.VulSearchMode = ISFPIteration.eVulSearchMode.PredPrey)
        Me.m_nudStepSize.Value = parms.AnomalySearchSplineStepSize
        Me.m_cmbAutoSave.SelectedIndex = parms.AutosaveMode
        Me.m_nudNoThreads.Maximum = parms.MaxThreads
        Me.m_nudNoThreads.Value = parms.NumThreads
        Me.m_cbResetVs.Checked = parms.ResetVsOnRun

        Me.m_fpK = New cEwEFormatProvider(Me.UIContext, Me.m_nudK, GetType(Integer))
        Me.m_fpVukCap = New cPropertyFormatProvider(Me.UIContext, Me.m_tbxVUlCap, Me.Core.EcosimModelParameters, eVarNameFlags.VulnerabilityCap)

        ' -- Handle run modes
        Select Case Me.m_runmode

            Case eRunMode.NotSet
                Debug.Assert(False)

            Case eRunMode.StandAlone
                Me.PopulateModelControls()
                Me.PopulateModelDropdowns()
                If Me.Core.ActiveEcosimScenarioIndex > 0 Then Me.SelectedEcosimScenario = Me.Core.EcosimScenarios(Me.Core.ActiveEcosimScenarioIndex)
                If Me.Core.ActiveTimeSeriesDatasetIndex > 0 Then Me.SelectedTimeSeries = Me.Core.TimeSeriesDataset(Me.Core.ActiveTimeSeriesDatasetIndex)
                'Hide the time series and apply button in standalone mode
                Me.m_btnTS.Hide()
                Me.m_btnApply.Hide()

            Case eRunMode.Plugin

                Me.PopulateAnomalyDropdown()

                ' Connect to ApplyTS command to time series button
                Me.m_cmdLoadTS = Me.CommandHandler.GetCommand("LoadTimeSeries")
                If Me.m_cmdLoadTS IsNot Nothing Then Me.m_cmdLoadTS.AddControl(Me.m_btnTS)

                ' Bring SFP manager up to date to running core
                Me.m_engine.UpdateToCore()
                Me.m_grid.RefreshContent()
                Me.m_engine.Parameters.CustomOutputFolder = ""

        End Select

        AddHandler Me.m_engine.OnIterationUpdated, AddressOf Me.OnIterationUpdated

        Me.m_bInUpdate = False

        Me.UpdateControls()

        Me.CoreComponents = New eCoreComponentType() {eCoreComponentType.MediatedInteractionManager, eCoreComponentType.TimeSeries}

    End Sub

    Protected Overrides Sub OnFormClosed(e As System.Windows.Forms.FormClosedEventArgs)

        ' Cleanup
        RemoveHandler Me.m_engine.OnIterationUpdated, AddressOf Me.OnIterationUpdated

        'Detach time series load command from time series button
        If Me.m_cmdLoadTS IsNot Nothing Then Me.m_cmdLoadTS.RemoveControl(Me.m_btnTS)

        Me.m_fpK.Release()
        Me.m_fpVukCap.Release()

        ' Done
        My.Settings.Save()
        MyBase.OnFormClosed(e)

    End Sub

    Public Overrides ReadOnly Property IsRunForm As Boolean
        Get
            Return True
        End Get
    End Property

    Public Overrides Sub OnCoreMessage(msg As EwECore.cMessage)
        MyBase.OnCoreMessage(msg)

        If Me.m_bKeepResults Then Return

        Try
            Select Case msg.Source
                Case eCoreComponentType.MediatedInteractionManager
                    Me.PopulateAnomalyDropdown()
                Case eCoreComponentType.TimeSeries
                    Dim parms As cSFPParameters = Me.m_engine.Parameters
                    Me.m_engine.Refresh(0)
                    Me.m_grid.RefreshContent()
                    Me.UpdateControls()
            End Select
        Catch ex As Exception
            m_logger.LogError(ex, "OnCoreMessage. Error processing core message in SFP run form")
        End Try

    End Sub

    Private m_bWasRunning As Boolean = False

    Public Overloads Sub UpdateControls()
        MyBase.UpdateControls()

        Dim bHasTimeSeries As Boolean = (Me.m_engine.TSIndex >= 1)
        Dim bHasEnabledIterations As Boolean = False
        Dim bHasEnabledIterationSelected As Boolean = False
        Dim bHasCompletedIterationSelected As Boolean = False
        Dim bNeedsAnomalyShape As Boolean = False
        Dim bHasAnomalyShape As Boolean = (Me.SelectedShapeIndex >= 0)
        Dim bContainsAnomaly As Boolean = False
        Dim bContainsVul As Boolean = False
        Dim bIsRunning As Boolean = Me.m_engine.IsRunning
        Dim bIsDefaultPath As Boolean = Me.m_engine.IsDefaultOutputFolder
        Dim parms As cSFPParameters = Me.m_engine.Parameters

        ' Running state has not changed? Skip this to reduce flashing in the UI
        If bIsRunning And Me.m_bWasRunning Then Return
        Me.m_bWasRunning = bIsRunning

        If (Me.m_bInUpdate) Then Return
        Me.m_bInUpdate = True

        Try ' Just to make sure that m_bInUpdate is reset at any cost

            Dim it As ISFPIteration = Nothing
            For Each it In Me.m_engine.Iterations
                bContainsAnomaly = bContainsAnomaly Or (it.GetType Is GetType(EwEStepwiseFittingPlugin.cSFPAnomalySearch)) Or
                                               (it.GetType Is GetType(EwEStepwiseFittingPlugin.cSFPVandASearch))
                bContainsVul = bContainsVul Or (it.GetType Is GetType(EwEStepwiseFittingPlugin.cSFPVulnerabilitySearch)) Or
                                               (it.GetType Is GetType(EwEStepwiseFittingPlugin.cSFPVandASearch))
            Next

            it = Me.SelectedIteration
            If (it IsNot Nothing) Then
                bHasCompletedIterationSelected = (it.RunState = ISFPIteration.eRunState.Completed)
                bHasEnabledIterationSelected = it.Enabled
                bNeedsAnomalyShape = bNeedsAnomalyShape Or (it.GetType Is GetType(EwEStepwiseFittingPlugin.cSFPAnomalySearch)) Or
                                               (it.GetType Is GetType(EwEStepwiseFittingPlugin.cSFPVandASearch))
            End If

            Dim bAnomalyOk As Boolean = Not bNeedsAnomalyShape Or bHasAnomalyShape

            For Each it In Me.m_engine.Iterations
                bHasEnabledIterations = bHasEnabledIterations Or it.Enabled
            Next

            Try
                Me.m_nudK.Enabled = (parms.MaxK > parms.MinK)
                Me.m_nudK.Minimum = parms.MinK
                Me.m_nudK.Maximum = parms.MaxK
                Me.m_nudK.Value = parms.K
                Me.m_fpK.Style = If(parms.K <= parms.CorrectK, cStyleGuide.eStyleFlags.OK, cStyleGuide.eStyleFlags.InvalidModelResult)

            Catch ex As Exception

            End Try

            ' -- Model panel --
            Me.m_plModel.Visible = (Me.m_runmode = eRunMode.StandAlone)
            Me.m_plModel.Enabled = (Me.m_runmode = eRunMode.StandAlone) And Not bIsRunning
            Me.m_cmbScenario.Enabled = Me.Core.StateMonitor.HasEcopathLoaded
            Me.m_cmbTimeSeries.Enabled = Me.Core.StateMonitor.HasEcosimLoaded

            ' -- Parameters panel --
            Me.m_plSettings.Enabled = Not bIsRunning
            Me.m_cbEnableAbsBioforBaseline.Enabled = (Not bIsRunning) And parms.HasAbsoluteBiomassTimeSeries

            ' -- Iterations panel --
            Me.m_btnSelectAll.Enabled = Not bIsRunning
            Me.m_btnSelectNone.Enabled = Not bIsRunning
            Me.m_btnSelectA.Enabled = (Not bIsRunning) And bContainsAnomaly
            Me.m_btnSelectBaseline.Enabled = Not bIsRunning
            Me.m_btnSelectFishing.Enabled = Not bIsRunning
            Me.m_btnSelectV.Enabled = (Not bIsRunning) And bContainsVul
            Me.m_btnSelectVandA.Enabled = (Not bIsRunning) And bContainsVul And bContainsAnomaly
            Me.m_btnSelectFandVandA.Enabled = (Not bIsRunning) And bContainsVul And bContainsAnomaly
            Me.m_btnApply.Enabled = bHasCompletedIterationSelected And bHasEnabledIterationSelected And (Not bIsRunning)
            Me.m_grid.UpdateRunState()

            ' -- Run panel --
            Me.m_cmbAutoSave.SelectedIndex = Me.m_engine.Parameters.AutosaveMode
            Me.m_cmbAutoSave.Enabled = Not bIsRunning

            ' Update output path entirely to resolve path placeholders
            Me.m_tbxOutputFolder.Text = Me.m_engine.OutputFolder
            Me.m_btnResetFolder.Enabled = (Not bIsRunning) And (Not bIsDefaultPath)
            Me.m_btnChooseFolder.Enabled = Not bIsRunning

            'Run button enabled when at least one iteration is enabled, time series are loaded, and anomaly search is set up ok
            Me.m_btnRun.Enabled = bHasEnabledIterations And bHasTimeSeries And bAnomalyOk And (Not bIsRunning)
            Me.m_btnStop.Enabled = bIsRunning

        Catch ex As Exception

        End Try

        Me.m_bInUpdate = False

    End Sub

    ''' <summary>
    ''' Apply the contents of the UI to the engine.
    ''' </summary>
    Private Sub CommitControls(bRefreshIterationsList As Boolean)

        Dim parms As cSFPParameters = Me.m_engine.Parameters

        Try
            ' The harmless ones
            parms.VulSearchMode = If(Me.m_rbPredator.Checked, ISFPIteration.eVulSearchMode.Predator, ISFPIteration.eVulSearchMode.PredPrey)
            parms.NumThreads = CInt(Me.m_nudNoThreads.Value)
            parms.AutosaveMode = CType(Me.m_cmbAutoSave.SelectedIndex, cSFPParameters.eAutosaveMode)
            parms.ResetVsOnRun = Me.m_cbResetVs.Checked

            ' And the ones that affect the list of iterations (grid refrensh can be slow)
            If bRefreshIterationsList Then

                parms.AnomalyShapeIndex = Me.SelectedShapeIndex
                parms.AnomalySearchSplineStepSize = CInt(Me.m_nudStepSize.Value)
                parms.EnableAbsoluteBiomassTimeSeries = Me.m_cbEnableAbsBioforBaseline.Checked
                parms.K = CInt(Me.m_nudK.Value)

                Me.m_engine.Refresh(parms.K)
                Me.m_grid.RefreshContent()

            End If
        Catch ex As Exception

        End Try
        Me.UpdateControls()

    End Sub

#End Region ' Overrides

#Region " Plug-in triggers "

    Public Sub OnTimeSeriesLoaded(tsd As cTimeSeriesDataset)

        ' This could very well be called while running
        If (Me.m_engine.IsRunning) Then
            Return
        End If

        Try
            Me.m_engine.UpdateToCore()
            Me.m_grid.RefreshContent()
            Me.UpdateControls()
        Catch ex As Exception

        End Try

    End Sub

#End Region ' Plug-in triggers

#Region " Control events "

    Private Sub OnSelectModel(sender As System.Object, e As System.EventArgs) _
        Handles m_btnSelectModel.Click

        If (Me.m_bInUpdate) Then Return
        If (Me.m_runmode <> eRunMode.StandAlone) Then Return

        Try
            ' Get a model file name from the user
            Dim strFileName As String = Me.ShowSelectModelDialogue()
            Me.m_engine.LoadModel(strFileName)

        Catch ex As Exception

        End Try

        Me.PopulateModelControls()
        Me.PopulateModelDropdowns()

        Me.UpdateControls()

    End Sub

    Private Sub OnFormatScenario(sender As Object, e As System.Windows.Forms.ListControlConvertEventArgs) _
        Handles m_cmbScenario.Format

        Try
            e.Value = DirectCast(e.ListItem, cEcoSimScenario).Name
        Catch ex As Exception
            Debug.Assert(False, ex.Message)
        End Try

    End Sub

    Private Sub OnFormatShape(sender As Object, e As System.Windows.Forms.ListControlConvertEventArgs) _
        Handles m_cmbAnomalyShape.Format

        Try
            Dim fmt As New ScientificInterfaceShared.Style.cShapeDataFormatter()
            e.Value = fmt.ToString(e.ListItem)
        Catch ex As Exception
            Debug.Assert(False, ex.Message)
        End Try

    End Sub

    Private Sub OnFormatTimeSeries(sender As Object, e As System.Windows.Forms.ListControlConvertEventArgs) _
        Handles m_cmbTimeSeries.Format

        Try
            e.Value = DirectCast(e.ListItem, cTimeSeriesDataset).Name
        Catch ex As Exception
            Debug.Assert(False, ex.Message)
        End Try

    End Sub

    Private Sub OnSelectedScenario(sender As Object, e As System.EventArgs) _
        Handles m_cmbScenario.SelectedIndexChanged

        If (Me.m_bInUpdate) Then Return
        If (Me.m_runmode <> eRunMode.StandAlone) Then Return

        Try
            Dim scenario As cEcoSimScenario = Me.SelectedEcosimScenario
            Dim isc As Integer = 0
            If (scenario IsNot Nothing) Then isc = scenario.Index
            Me.m_engine.LoadEcoSimScenario(isc)
        Catch ex As Exception
            Debug.Assert(False, ex.Message)
        End Try

        Me.UpdateControls()

    End Sub

    Private Sub OnSelectedTimeseries(sender As Object, e As System.EventArgs) _
        Handles m_cmbTimeSeries.SelectedIndexChanged

        If (Me.m_bInUpdate) Then Return
        If (Me.m_runmode <> eRunMode.StandAlone) Then Return

        Try
            Dim ts As cTimeSeriesDataset = Me.SelectedTimeSeries
            Dim its As Integer = 0
            If (ts IsNot Nothing) Then its = ts.Index
            Me.m_engine.LoadTimeSeries(its)

            Me.CommitControls(True)
        Catch ex As Exception
            Debug.Assert(False, ex.Message)
        End Try

    End Sub

    Private Sub OnShapeSelected(sender As System.Object, e As System.EventArgs) _
        Handles m_cmbAnomalyShape.SelectedIndexChanged

        If (Me.m_engine Is Nothing) Then Return
        If (Me.m_bInUpdate) Then Return

        Me.CommitControls(True)

    End Sub

    Private Sub OnThreadCountChanged(sender As Object, e As System.EventArgs) _
        Handles m_nudNoThreads.ValueChanged

        ' Safety catch: Numeric updown controls throw events on creation. Aargh.
        If (Me.m_engine Is Nothing) Then Return
        If (Me.m_bInUpdate) Then Return

        Me.CommitControls(False)

    End Sub

    Private Sub OnSplinePointStepSizeChanged(sender As Object, e As System.EventArgs) _
        Handles m_nudStepSize.ValueChanged

        ' Safety catch: Numeric updown controls throw events on creation. Aargh.
        If (Me.m_engine Is Nothing) Then Return
        If (Me.m_bInUpdate) Then Return

        Me.CommitControls(True)

    End Sub

    Private Sub OnKChanged(sender As System.Object, e As System.EventArgs) _
        Handles m_nudK.ValueChanged

        ' Safety catch: Numeric updown controls throw events on creation. Aargh.
        If (Me.m_engine Is Nothing) Then Return
        If (Me.m_bInUpdate) Then Return

        Me.CommitControls(True)

    End Sub

    Private Sub OnSearchCheckedChanged(sender As System.Object, e As System.EventArgs) _
        Handles m_rbPredator.CheckedChanged,
                m_rbPredPrey.CheckedChanged

        If (Me.m_engine Is Nothing) Then Return
        If (Me.m_bInUpdate) Then Return

        Me.CommitControls(False)

    End Sub

    'Private Sub OnExport(sender As Object, e As EventArgs)

    '    Try
    '        Dim cmd As cFileSaveCommand = CType(Me.UIContext.CommandHandler.GetCommand(cFileSaveCommand.COMMAND_NAME), cFileSaveCommand)
    '        cmd.Invoke(ScientificInterfaceShared.My.Resources.FILEFILTER_XML, 0, "Select file save location")
    '        If cmd.Result = DialogResult.OK Then
    '            Dim IO As New cSFPio(Me.m_engine)
    '            Dim msg As cMessage = Nothing
    '            If IO.ToXML(cmd.FileName) Then
    '                msg = New cMessage(String.Format("Stepwise fitting results saved to {0}", cmd.FileName), eMessageType.DataExport, eCoreComponentType.External, eMessageImportance.Information)
    '                msg.Hyperlink = System.IO.Path.GetDirectoryName(cmd.FileName)
    '            Else
    '                msg = New cMessage(String.Format("Stepwise fitting results failed to save to {0}", cmd.FileName), eMessageType.DataExport, eCoreComponentType.External, eMessageImportance.Critical)
    '            End If
    '            Me.Core.Messages.SendMessage(msg)
    '        End If
    '    Catch ex As Exception

    '    End Try
    'End Sub

    'Private Sub OnReloadIterations(sender As Object, e As EventArgs)
    '    Try
    '        Me.m_engine.LoadIterationsConfiguration()
    '        Me.m_grid.UpdateContent()
    '    Catch ex As Exception

    '    End Try
    'End Sub

    Private Sub OnSelectAll(sender As System.Object, e As System.EventArgs) _
        Handles m_btnSelectAll.Click

        Try
            ' Enable all iterations for running
            For Each it As ISFPIteration In Me.m_engine.Iterations
                it.Enabled = True
            Next
        Catch ex As Exception
            Debug.Assert(False, ex.Message)
        End Try
        Me.m_grid.UpdateContent()

    End Sub

    Private Sub OnSelectNone(sender As System.Object, e As System.EventArgs) _
        Handles m_btnSelectNone.Click

        Try
            ' Disable all iterations for running
            For Each it As ISFPIteration In Me.m_engine.Iterations
                it.Enabled = False
            Next
        Catch ex As Exception
            Debug.Assert(False, ex.Message)
        End Try
        Me.m_grid.UpdateContent()

    End Sub

    Private Sub OnSelectVuls(sender As System.Object, e As System.EventArgs) _
        Handles m_btnSelectV.Click
        Try
            ' Enable all Vulnerability iterations for running
            For Each it As ISFPIteration In Me.m_engine.Iterations
                If it.GetType Is GetType(EwEStepwiseFittingPlugin.cSFPVulnerabilitySearch) Then
                    it.Enabled = True
                End If
            Next
        Catch ex As Exception
            Debug.Assert(False, ex.Message)
        End Try
        Me.m_grid.UpdateContent()
    End Sub

    Private Sub OnSelectAnomaly(sender As System.Object, e As System.EventArgs) _
        Handles m_btnSelectA.Click
        Try
            ' Enable all Anomaly iterations for running
            For Each it As ISFPIteration In Me.m_engine.Iterations
                If it.GetType Is GetType(EwEStepwiseFittingPlugin.cSFPAnomalySearch) Then
                    it.Enabled = True
                End If
            Next
        Catch ex As Exception
            Debug.Assert(False, ex.Message)
        End Try
        Me.m_grid.UpdateContent()
    End Sub

    Private Sub OnSelectVandA(sender As System.Object, e As System.EventArgs) _
        Handles m_btnSelectVandA.Click
        Try
            ' Enable all V and A iterations for running
            For Each it As ISFPIteration In Me.m_engine.Iterations
                If it.GetType Is GetType(EwEStepwiseFittingPlugin.cSFPVandASearch) Then
                    it.Enabled = True
                End If
            Next
        Catch ex As Exception
            Debug.Assert(False, ex.Message)
        End Try
        Me.m_grid.UpdateContent()
    End Sub

    Private Sub OnSelectFandVandA(sender As System.Object, e As System.EventArgs) _
        Handles m_btnSelectFandVandA.Click
        Try
            ' Enable all V and A iterations for running
            For Each it As ISFPIteration In Me.m_engine.Iterations
                If it.GetType Is GetType(EwEStepwiseFittingPlugin.cSFPVandASearch) And (it.BaseSearchMode = ISFPIteration.eBaseSearchMode.Fishing) Then
                    it.Enabled = True
                End If
            Next
        Catch ex As Exception
            Debug.Assert(False, ex.Message)
        End Try
        Me.m_grid.UpdateContent()
    End Sub

    Private Sub OnSelectBaseline(sender As System.Object, e As System.EventArgs) _
        Handles m_btnSelectBaseline.Click
        Try
            ' Enable Baseline iterations for running
            For Each it As ISFPIteration In Me.m_engine.Iterations
                If (it.BaseSearchMode = ISFPIteration.eBaseSearchMode.Baseline) Then
                    it.Enabled = True
                End If
            Next
        Catch ex As Exception
            Debug.Assert(False, ex.Message)
        End Try
        Me.m_grid.UpdateContent()
    End Sub

    Private Sub OnSelectTSOnly(sender As Object, e As EventArgs) _
        Handles m_btnSelectTS.Click
        Try
            ' Enable Baseline iterations for running
            For Each it As ISFPIteration In Me.m_engine.Iterations
                If (it.IsGroupsWithTimeSeriesOnly) Then
                    it.Enabled = True
                End If
            Next
        Catch ex As Exception
            Debug.Assert(False, ex.Message)
        End Try
        Me.m_grid.UpdateContent()

    End Sub

    Private Sub OnSelectFishin(sender As System.Object, e As System.EventArgs) _
        Handles m_btnSelectFishing.Click
        Try
            ' Enable Fishing iterations for running
            For Each it As ISFPIteration In Me.m_engine.Iterations
                If it.BaseSearchMode = ISFPIteration.eBaseSearchMode.Fishing Then
                    it.Enabled = True
                End If
            Next
        Catch ex As Exception
            Debug.Assert(False, ex.Message)
        End Try
        Me.m_grid.UpdateContent()
    End Sub

    Private Sub OnIterationSelected() _
        Handles m_grid.OnSelectionChanged

        Try
            Me.BeginInvoke(New MethodInvoker(AddressOf Me.UpdateControls))
        Catch ex As Exception
            Debug.Assert(False, ex.Message)
        End Try

    End Sub

    Private m_bKeepResults As Boolean = False

    ''' <summary>
    ''' Load selected interation into the EwE user interface
    ''' </summary>
    Private Sub OnApplyIteration(sender As System.Object, e As System.EventArgs) _
        Handles m_btnApply.Click

        Try
            Dim iteration As ISFPIteration = Me.SelectedIteration
            If (iteration IsNot Nothing) Then
                Me.m_bKeepResults = True
                iteration.Apply(Me.Core)
                Me.m_bKeepResults = False
            End If
        Catch ex As Exception
            Debug.Assert(False, ex.Message)
        End Try

    End Sub

    Private Sub OnChooseOutputFolder(sender As Object, e As System.EventArgs) _
        Handles m_btnChooseFolder.Click

        Try
            Dim dlg As New FolderBrowserDialog()
            dlg.SelectedPath = Me.m_engine.Parameters.CustomOutputFolder
            dlg.ShowNewFolderButton = True
            If (dlg.ShowDialog() = System.Windows.Forms.DialogResult.OK) Then
                Me.m_engine.Parameters.CustomOutputFolder = dlg.SelectedPath
                Me.UpdateControls()
            End If
        Catch ex As Exception

        End Try
    End Sub

    Private Sub OnResetOutputFolder(sender As Object, e As System.EventArgs) _
        Handles m_btnResetFolder.Click
        Try
            Me.m_engine.Parameters.CustomOutputFolder = ""
            Me.UpdateControls()
        Catch ex As Exception

        End Try
    End Sub

    Private Sub OnAutosaveOptionsChanged(sender As Object, e As System.EventArgs) _
        Handles m_cmbAutoSave.SelectedIndexChanged

        If (Me.m_engine Is Nothing) Then Return
        If (Me.m_bInUpdate) Then Return

        Me.CommitControls(False)

    End Sub

    Private Sub OnEnableAbBioforBaselineChanged(sender As Object, e As System.EventArgs) _
        Handles m_cbEnableAbsBioforBaseline.CheckedChanged

        If (Me.m_engine Is Nothing) Then Return
        If (Me.m_bInUpdate) Then Return

        Me.CommitControls(True)

    End Sub

    Private Sub OnRun(sender As System.Object, e As System.EventArgs) _
        Handles m_btnRun.Click

        Try
            ' Run the Stepwise Fitting Procedure
            Me.m_engine.Run()
            Me.m_grid.UpdateContent()
        Catch ex As Exception
            Debug.Assert(False, ex.Message)
        End Try

    End Sub

    Private Sub OnStop(sender As System.Object, e As System.EventArgs) _
        Handles m_btnStop.Click

        Try
            Me.m_engine.StopRun()
        Catch ex As Exception
            Debug.Assert(False, ex.Message)
        End Try

    End Sub

    Private Delegate Sub OnIterationUpdatedDelegate(sender As cSFPManager, iteration As ISFPIteration)

    Private Sub OnIterationUpdated(sender As cSFPManager, iteration As ISFPIteration)

        If Me.InvokeRequired Then
            Me.Invoke(New OnIterationUpdatedDelegate(AddressOf Me.OnIterationUpdated), New Object() {sender, iteration})
        Else
            ' Lazy update
            Me.BeginInvoke(New MethodInvoker(AddressOf Me.m_grid.UpdateContent))
        End If

    End Sub

#End Region ' Control events

#Region " Internals "

    ''' <summary>
    ''' Presents the user with a standard Windows interface for selecting a file
    ''' </summary>
    ''' <returns>A user-selected file, or an empty string if the user did not select a file.</returns>
    ''' <remarks>
    ''' ToDo: use EwE file open command instead when ported to a plug-in
    ''' </remarks>
    Private Function ShowSelectModelDialogue() As String

        'Create a new open file dialogue
        Dim openFileDialogue As New OpenFileDialog()

        'Set the file filters
        openFileDialogue.Filter = ScientificInterfaceShared.My.Resources.FILEFILTER_MODEL_OPEN

        'Show the dialogue box and get the user-selected filename
        If (openFileDialogue.ShowDialog() = DialogResult.OK) Then
            Return openFileDialogue.FileName
        End If

        'The user did not select a file. Return an empty string
        Return String.Empty

    End Function

    Private Sub PopulateModelDropdowns()

        ' Scenarios
        Me.m_cmbScenario.Items.Clear()
        For i As Integer = 1 To Me.Core.nEcosimScenarios
            Me.m_cmbScenario.Items.Add(Me.Core.EcosimScenarios(i))
        Next

        ' Time series
        Me.m_cmbTimeSeries.Items.Clear()
        For i As Integer = 1 To Me.Core.nTimeSeriesDatasets
            Me.m_cmbTimeSeries.Items.Add(Me.Core.TimeSeriesDataset(i))
        Next

    End Sub

    Private Sub PopulateAnomalyDropdown()

        ' Anomaly shapes
        Me.m_cmbAnomalyShape.Items.Clear()

        Dim iSel As Integer = -1
        Dim parms As cSFPParameters = Me.m_engine.Parameters

        For Each shape As cShapeData In Me.m_engine.GetAvailableAnomalyShapes()
            Dim iItem As Integer = Me.m_cmbAnomalyShape.Items.Add(shape)
            If (shape.Index = parms.AnomalyShapeIndex) Then iSel = iItem
        Next

        If (Me.m_cmbAnomalyShape.Items.Count > 0) Then
            Me.m_cmbAnomalyShape.SelectedIndex = Math.Max(0, iSel)
        End If

    End Sub

    Private Sub PopulateModelControls()

        Me.m_tbxModel.Text = ""
        Dim model As cEwEModel = Me.Core.EwEModel
        If (model IsNot Nothing) Then
            Me.m_tbxModel.Text = model.Name
        End If

    End Sub

    Private Property SelectedEcosimScenario As cEcoSimScenario
        Get
            Return DirectCast(Me.m_cmbScenario.SelectedItem, cEcoSimScenario)
        End Get
        Set(scenario As cEcoSimScenario)
            Me.m_cmbScenario.SelectedItem = scenario
        End Set
    End Property

    Private Property SelectedTimeSeries As cTimeSeriesDataset
        Get
            Return DirectCast(Me.m_cmbTimeSeries.SelectedItem, cTimeSeriesDataset)
        End Get
        Set(dataset As cTimeSeriesDataset)
            Me.m_cmbTimeSeries.SelectedItem = dataset
        End Set
    End Property

    Private Property SelectedShapeIndex As Integer
        Get
            Dim item As cShapeData = DirectCast(Me.m_cmbAnomalyShape.SelectedItem, cShapeData)
            If item IsNot Nothing Then Return item.Index
            Return -1
        End Get
        Set(index As Integer)
            Dim iSel As Integer = -1
            For i As Integer = 0 To Me.m_cmbAnomalyShape.Items.Count - 1
                Dim item As cShapeData = DirectCast(Me.m_cmbAnomalyShape.Items(i), cShapeData)
                If item.Index = index Then iSel = i
            Next
            Me.m_cmbAnomalyShape.SelectedIndex = iSel
        End Set
    End Property

    Private ReadOnly Property SelectedIteration As ISFPIteration
        Get
            Return Me.m_grid.SelectedIteration
        End Get
    End Property

#End Region ' Internals

End Class