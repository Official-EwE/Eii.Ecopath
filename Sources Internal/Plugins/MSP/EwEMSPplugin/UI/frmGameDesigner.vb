' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 2 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see <http://www.gnu.org/licenses/gpl-2.0.html>. 
'
' Copyright 2016- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'
#Region " Imports "

Option Strict On
Imports System.Drawing
Imports System.IO
Imports System.Windows.Forms
Imports EwECore
Imports EwECore.DataSources
Imports EwEMSPPlugin
Imports EwEMSPPlugin.Emulator
Imports EwEShell
Imports EwEUtils.Core
Imports EwEUtils.SystemUtilities.cSystemUtils
Imports EwEUtils.Utilities
Imports ScientificInterfaceShared.Commands
Imports ScientificInterfaceShared.Controls
Imports ScientificInterfaceShared.Controls.EwEGrid
Imports SourceGrid2
Imports SharedRecources = ScientificInterfaceShared.My.Resources

#End Region ' Imports

Namespace UI

    ''' <summary>
    ''' Main user interface for the MSP tools for EwE desktop plug-in.
    ''' </summary>
    ''' <seealso cref="ScientificInterfaceShared.Forms.frmEwE" />
    Public Class frmGameDesigner

#Region " Private vars "

        Private m_spacedata As cEcospaceDataStructures = Nothing
        Private m_testdata As cTestSetData = Nothing
        Private m_bInupdate As Boolean = True
        Private m_qeh As cQuickEditHandler = Nothing

        Private m_strOutputFolder As String = ""

        Private WithEvents m_fpSpinupYears As cEwEFormatProvider = Nothing
        Private WithEvents m_fpRunYears As cEwEFormatProvider = Nothing
        Private WithEvents m_fpMAPCellClosure As cEwEFormatProvider = Nothing

        Private m_dgtTimeStep As New cCore.EcoSpaceInterfaceDelegate(AddressOf OnEcospaceTimeStep)

        Private m_checkEcosimTimeSeries As cRequirementChecker = Nothing
        Private m_checkEcosimFishing As cRequirementChecker = Nothing
        Private m_checkEcosimForcing As cRequirementChecker = Nothing
        Private m_checkEcospaceTimeSeries As cRequirementChecker = Nothing

#End Region ' Private vars 

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Creates a new <see cref="frmGameDesigner"/>.
        ''' </summary>
        ''' <param name="uic">The <see cref="cUIContext"/> to work against.</param>
        ''' <param name="shell">The <see cref="cEwEShell"/> that contains MSP game info.</param>
        ''' <param name="data">The <see cref="cEcospaceDataStructures"/> to work against.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(uic As cUIContext, shell As cEwEShell, data As cEcospaceDataStructures)

            Me.SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.OptimizedDoubleBuffer Or ControlStyles.ResizeRedraw, True)

            Me.InitializeComponent()

            Me.UIContext = uic
            Me.m_spacedata = data

            Me.m_testdata = New cTestSetData()
            Me.Shell = shell
            Me.m_gridPressureMappings.Shell = Me.Shell
            Me.m_gridOutcome.Shell = shell

            Me.Text = My.Resources.NODE_CONFIG
            Me.TabText = Me.Text

            Me.m_lblAboutVersion.Text = cStringUtils.Localize(Me.m_lblAboutVersion.Text, My.Resources.VERSION)

        End Sub

#Region " Overrides "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Form load event override to initialize content.
        ''' </summary>
        ''' <param name="e">Ignored</param>
        ''' -----------------------------------------------------------------------
        Protected Overrides Sub OnLoad(e As EventArgs)
            MyBase.OnLoad(e)

            If (Me.UIContext Is Nothing) Then Return

            Me.m_ilTabIcons.Images.Add(SharedRecources.OK)
            Me.m_ilTabIcons.Images.Add(SharedRecources.Warning)
            Me.m_ilTabIcons.Images.Add(SharedRecources.Critical)

            Me.m_gridPressureMappings.UIContext = Me.UIContext
            Me.m_gridOutcome.UIContext = Me.UIContext
            Me.m_gridEmulTestset.UIContext = Me.UIContext

            Me.m_fpSpinupYears = New cEwEFormatProvider(Me.UIContext, Me.m_tbxSpinupYears, GetType(Integer))
            Me.m_fpRunYears = New cEwEFormatProvider(Me.UIContext, Me.m_tbxRunYears, GetType(Integer))
            Me.m_fpMAPCellClosure = New cEwEFormatProvider(Me.UIContext, Me.m_tbxMPACellClosure, GetType(Single))

            Me.m_checkEcosimTimeSeries = New cEcosimTimeSeriesChecker(Me.Core)
            Me.m_checkEcosimFishing = New cEcosimFishingChecker(Me.Core)
            Me.m_checkEcosimForcing = New cEcosimForcingChecker(Me.Core)
            Me.m_checkEcospaceTimeSeries = New cEcospaceTimeSeriesChecker(Me.Core)

            Me.m_cbGameCalcIndicators.Checked = False

            Me.m_qeh = New cQuickEditHandler()
            Me.m_qeh.Attach(Me.m_gridOutcome, Me.UIContext, Me.m_tsOutcome, Me.m_gridOutcome.IsOutputGrid)

            Me.Core.AddEcospaceTimeStepHandler(Me.m_dgtTimeStep)
            Me.FillGameCombo()
            Me.FillPressureTypesCombo()
            Me.FillOutputTypesCombo()
            Me.FillTestsetCombo()
            Me.FillStopOptionsCombo()

            ' For the benefit of the requirement checkers. This could have been encapsulated a bit more neatly, but ok...
            Me.CoreComponents = New eCoreComponentType() {eCoreComponentType.TimeSeries, eCoreComponentType.EcoSpace, eCoreComponentType.ShapesManager}
            Me.CoreExecutionState = eCoreExecutionState.EcospaceLoaded

            Me.m_cbEmulPauseSpace.Checked = My.Settings.PauseEcospace
            Me.m_cmbEmulPauseOptions.SelectedIndex = Math.Max(0, Math.Min(Me.m_cmbEmulPauseOptions.Items.Count - 1, My.Settings.PauseEcospaceInterval))
            Me.m_cbSaveOutputMaps.Checked = My.Settings.SaveOutputMaps

            Me.m_bInupdate = False
            Me.UpdateControls()

        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Form close event override, cleans up the UI.
        ''' </summary>
        ''' <param name="e">Ignored</param>
        ''' -----------------------------------------------------------------------
        Protected Overrides Sub OnFormClosed(e As FormClosedEventArgs)

            ' To prevent any updates in response to resetting / dying controls
            Me.m_bInupdate = True

            My.Settings.PauseEcospace = Me.m_cbEmulPauseSpace.Checked
            My.Settings.PauseEcospaceInterval = Me.m_cmbEmulPauseOptions.SelectedIndex
            My.Settings.SaveOutputMaps = Me.m_cbSaveOutputMaps.Checked

            Me.m_qeh.Detach()

            Me.m_testdata.Close()

            Me.Core.RemoveEcospaceTimeStepHandler(Me.m_dgtTimeStep)
            Me.m_dgtTimeStep = Nothing

            Me.m_fpSpinupYears.Release()
            Me.m_fpRunYears.Release()
            Me.m_fpMAPCellClosure.Release()

            Me.m_gridPressureMappings.UIContext = Nothing
            Me.m_gridOutcome.UIContext = Nothing
            Me.m_gridEmulTestset.UIContext = Nothing

            MyBase.OnFormClosed(e)

        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Core message handler to make the UI respond to important EwE events.
        ''' </summary>
        ''' <param name="msg">The subscribed message to respond to.</param>
        ''' -----------------------------------------------------------------------
        Public Overrides Sub OnCoreMessage(msg As cMessage)
            MyBase.OnCoreMessage(msg)

            If (Me.IsDisposed) Then Return

            Me.m_checkEcosimTimeSeries.OnCoreMessage(msg)
            Me.m_checkEcosimFishing.OnCoreMessage(msg)
            Me.m_checkEcosimForcing.OnCoreMessage(msg)
            Me.m_checkEcospaceTimeSeries.OnCoreMessage(msg)

            Me.BeginInvoke(New MethodInvoker(AddressOf Me.UpdateControls))

        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' overridden to ensure that this form is treated as a run form. Run forms 
        ''' do not close when input data has changed.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Overrides ReadOnly Property IsRunForm As Boolean
            Get
                Return True
            End Get
        End Property

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Overridden to update the state of controls in this form.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Protected Overrides Sub UpdateControls()
            MyBase.UpdateControls()

            If (Me.UIContext Is Nothing) Then Return
            If (Me.m_bInupdate) Then Return
            If (Me.Core.ActiveEcospaceScenarioIndex <= 0) Then Return

            Dim sm As cCoreStateMonitor = Me.Core.StateMonitor
            Dim space As cEcospaceScenario = Me.Core.EcospaceScenarios(Me.Core.ActiveEcospaceScenarioIndex)

            Dim game As cGame = Me.SelectedGame()
            Dim bHasGame As Boolean = (game IsNot Nothing)
            Dim bHasGameName As Boolean = (Me.m_tbxGameName.Text.Trim.Length > 3)
            Dim bHasDuplicateGameNames As Boolean = False
            Dim bHasGameVersion As Boolean = Not String.IsNullOrWhiteSpace(Me.m_tbxInfoVersion.Text)
            Dim bHasPressureName As Boolean = Not String.IsNullOrWhiteSpace(Me.m_tbxPressureName.Text)
            Dim bHasPressureSelected As Boolean = (Me.m_gridPressureMappings.SelectedPressure IsNot Nothing)
            Dim bHasOutputName As Boolean = Not String.IsNullOrWhiteSpace(Me.m_tbxOutcomeName.Text)
            Dim bHasOutputSelected As Boolean = (Me.m_lbOutputs.SelectedIndices.Count = 1)
            Dim bHasOutputSselected As Boolean = (Me.m_lbOutputs.SelectedIndices.Count > 0)
            Dim bHasTestsetSelected As Boolean = (Me.m_cmbEmulTestsets.SelectedIndex > -1)
            Dim bHasTestsetName As Boolean = Not String.IsNullOrWhiteSpace(Me.m_tbxTestsetName.Text)
            Dim bHasCurrent As Boolean = False
            Dim bIsEcospaceRunning As Boolean = sm.IsEcospaceRunning

            If (game IsNot Nothing) Then
                bHasCurrent = (space.DBID = game.EcospaceID)
            End If

            Dim iImg As Integer = 0
            Dim nOK As Integer = 0

            If (bHasGame) Then
                For Each gameTest As cGame In Me.Shell.Data.Games
                    If (Not ReferenceEquals(game, gameTest)) Then
                        bHasDuplicateGameNames = bHasDuplicateGameNames Or (String.Compare(gameTest.Name, game.Name, True) = 0)
                    End If
                Next
            End If

            ' -- Game --
            Me.m_btnGameAdd.Enabled = bHasGameName And Not bIsEcospaceRunning
            Me.m_btnGameRename.Enabled = bHasGameName And bHasGame And Not bIsEcospaceRunning
            Me.m_btnGameDelete.Enabled = bHasGame And Not bIsEcospaceRunning
            Me.m_cmbGames.Enabled = Not bIsEcospaceRunning

            ' -- Info --
            Me.m_tpInformation.Enabled = bHasGame And Not bIsEcospaceRunning
            Me.m_btnSettingsUseCurrentScenario.Enabled = Not bHasCurrent

            ' -- Settings --
            Me.m_tpEwESettings.Enabled = bHasGame And Not bIsEcospaceRunning

            ' -- Pressures --
            Me.m_tpPressures.Enabled = bHasGame And Not bIsEcospaceRunning
            Me.m_btnPressureAdd.Enabled = bHasPressureName
            Me.m_btnPressureRename.Enabled = bHasPressureName And bHasPressureSelected
            Me.m_btnPressureDelete.Enabled = bHasPressureSelected

            ' -- Outputs --
            Me.m_tpOutcomes.Enabled = bHasGame And Not bIsEcospaceRunning
            Me.m_btnOutcomeAdd.Enabled = bHasOutputName
            Me.m_btnOutcomeRename.Enabled = bHasOutputName And bHasOutputSelected
            Me.m_btnOutcomeDelete.Enabled = bHasOutputSselected

            ' -- Emulator --
            Me.m_tpEmulator.Enabled = bHasGame
            Me.m_btnEmulStep.Enabled = bIsEcospaceRunning And Me.Core.EcospacePaused
            Me.m_btnEmulStop.Enabled = bIsEcospaceRunning
            Me.m_cbSaveOutputMaps.Enabled = Not bIsEcospaceRunning
            Me.m_nudEmulOutcomeRange.Enabled = Not bIsEcospaceRunning And bHasGame
            Me.m_btnEmulViewOutputFolder.Enabled = Directory.Exists(Me.OutputPath)

            Me.m_cmbEmulTestsets.Enabled = bHasGame

            ' Can only modify test sets when Ecospace is not running
            Me.m_btnTestsetAdd.Enabled = bHasGame And bHasTestsetName And Not bIsEcospaceRunning
            Me.m_btnTestsetRename.Enabled = bHasGame And bHasTestsetName And bHasTestsetSelected And Not bIsEcospaceRunning
            Me.m_btnTestsetDelete.Enabled = bHasGame And bHasTestsetSelected And Not bIsEcospaceRunning
            Me.m_btnTestsetApply.Enabled = bHasGame And bHasTestsetSelected
            Me.m_gridEmulTestset.Enabled = bHasGame And bHasTestsetSelected And Not bIsEcospaceRunning

            Me.ShowModelStatus(Me.m_lblCheckGame, Not bHasDuplicateGameNames, My.Resources.CHECK_GAME_OK, My.Resources.CHECK_GAME_FAILED)

            Dim bSimOK As Boolean = Not bHasDuplicateGameNames And
                Me.ShowModelStatus(Me.m_lblCheckSimTimeSeries, Not Me.HasEcosimTimeseries(), My.Resources.CHECK_SIM_TS_OK, My.Resources.CHECK_SIM_TS_FAILED) And
                Me.ShowModelStatus(Me.m_lblCheckSimForcing, Not Me.HasEcosimForcingPattern(), My.Resources.CHECK_SIM_FF_OK, My.Resources.CHECK_SIM_FF_FAILED) And
                Me.ShowModelStatus(Me.m_lblCheckSimFishing, Not Me.HasEcosimFishingPattern(), My.Resources.CHECK_SIM_FISH_OK, My.Resources.CHECK_SIM_FISH_FAILED) And
                Me.ShowModelStatus(Me.m_lblCheckSpaceTimeSeries, Not Me.HasEcospaceTimeseries(), My.Resources.CHECK_SPACE_TS_OK, My.Resources.CHECK_SPACE_TS_FAILED)

            If (game IsNot Nothing) Then

                iImg = 0
                If (Not bHasGameVersion) Then iImg = 2
                Me.SetTabStatusImage(Me.m_tpInformation, iImg)

                iImg = 0
                If Not bSimOK Then iImg = Math.Max(iImg, 1)
                Me.SetTabStatusImage(Me.m_tpEwESettings, iImg)

                Select Case game.NumConnectedDrivers
                    Case 0 : iImg = 2
                    Case 1 To 3 : iImg = 1
                    Case Else : iImg = 0
                End Select
                Me.SetTabStatusImage(Me.m_tpPressures, iImg)

                For Each out As cOutcome In game.Outputs
                    If (out.LayerType = cOutcome.eLayerType.Indicator) Then
                        nOK += 1
                    Else
                        If (out.NumUsed > 0) Then nOK += 1
                    End If
                Next
                Select Case nOK
                    Case 0 : iImg = 2
                    Case 1 To 3 : iImg = 1
                    Case Else : iImg = 0
                End Select
                Me.SetTabStatusImage(Me.m_tpOutcomes, iImg)

                iImg = 0
                If (Me.m_cbEmulPauseSpace.Checked) Then iImg = 1
                Me.SetTabStatusImage(Me.m_tpEmulator, iImg)
            Else
                Me.SetTabStatusImage(Me.m_tpEwESettings, 1)
                Me.SetTabStatusImage(Me.m_tpPressures, 1)
                Me.SetTabStatusImage(Me.m_tpOutcomes, 1)
                Me.SetTabStatusImage(Me.m_tpEmulator, 1)
            End If

        End Sub

#End Region ' Overrides

#Region " Public access "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Returns the selected <see cref="cGame">game</see>.
        ''' </summary>
        ''' <returns>The selected <see cref="cGame">game</see>, or Nothing if no
        ''' game is selected.</returns>
        ''' -----------------------------------------------------------------------
        Public Function SelectedGame() As cGame
            Return DirectCast(Me.m_cmbGames.SelectedItem, cGame)
        End Function

#End Region ' Public access

#Region " Requirement checking "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns whether Ecosim has (undesired) timeseries.
        ''' </summary>
        ''' <returns>True if Ecosim has timeseries.</returns>
        ''' -------------------------------------------------------------------
        Private Function HasEcosimTimeseries() As Boolean
            Return Not Me.m_checkEcosimTimeSeries.RequirementsMet
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns whether Ecosim has (undesired) temporal forcing.
        ''' </summary>
        ''' <returns>True if Ecosim has temporal forcing.</returns>
        ''' -------------------------------------------------------------------
        Private Function HasEcosimForcingPattern() As Boolean
            Return Not Me.m_checkEcosimForcing.RequirementsMet
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns whether Ecosim has (undesired) fishing patterns.
        ''' </summary>
        ''' <returns>True if Ecosim has fishing patterns.</returns>
        ''' -------------------------------------------------------------------
        Private Function HasEcosimFishingPattern() As Boolean
            Return Not Me.m_checkEcosimFishing.RequirementsMet
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns whether Ecospace has (undesired) timeseries.
        ''' </summary>
        ''' <returns>True if Ecospace has timeseries.</returns>
        ''' -------------------------------------------------------------------
        Private Function HasEcospaceTimeseries() As Boolean
            Return Not Me.m_checkEcospaceTimeSeries.RequirementsMet
        End Function

#End Region ' Requirement checking

#Region " Control events "

#Region " Common "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Event handler, called when the user has changed a text in the UI for the 
        ''' benefit of renaming / declaring items. These changes do NOT dirty the model.
        ''' </summary>
        ''' <param name="sender">Ignored.</param>
        ''' <param name="e">Ignored</param>
        ''' -----------------------------------------------------------------------
        Private Sub OnEditorTextChanged(sender As Object, e As EventArgs) _
            Handles m_tbxGameName.TextChanged, m_tbxPressureName.TextChanged, m_tbxOutcomeName.TextChanged, m_tbxTestsetName.TextChanged
            Me.UpdateControls()
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Event handler, called when the user has selected a type in the UI.
        ''' </summary>
        ''' <param name="sender">Ignored.</param>
        ''' <param name="e">Ignored</param>
        ''' -----------------------------------------------------------------------
        Private Sub OnAnyTypeSelected(sender As Object, e As EventArgs) _
        Handles m_cmbOutputTypes.SelectedIndexChanged, m_cmbPressureTypes.SelectedIndexChanged
            Me.UpdateControls()
        End Sub

#End Region ' Common

#Region " Game "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Event handler, called when the users has selected a <see cref="cGame">game</see>.
        ''' </summary>
        ''' <param name="sender">Ignored.</param>
        ''' <param name="e">Ignored</param>
        ''' -----------------------------------------------------------------------
        Private Sub OnGameSelected(sender As Object, e As EventArgs) Handles m_cmbGames.SelectedIndexChanged
            Try
                Dim game As cGame = Me.SelectedGame()

                Me.m_bInupdate = True
                If (game IsNot Nothing) Then
                    Me.m_fpRunYears.Value = game.RunYears
                    Me.m_fpSpinupYears.Value = game.SpinupYears
                    Me.m_fpMAPCellClosure.Value = game.MPACellClosureRatio
                    Me.m_tbxGameName.Text = game.Name
                    Me.m_tbxInfoVersion.Text = game.Version
                    Me.m_tbxInfoAuthor.Text = game.Author
                    Me.m_tbxInfoContact.Text = game.Contact
                    Me.m_tbxInfoDescription.Text = game.Description
                    Me.m_cbGameCalcIndicators.Checked = game.CalculateIndicators
                    Me.m_nudEmulOutcomeRange.Value = CDec(game.OutcomeRange)
                Else
                    Me.m_fpRunYears.Value = 0
                    Me.m_fpSpinupYears.Value = 0
                    Me.m_fpMAPCellClosure.Value = 0
                    Me.m_tbxGameName.Text = ""
                    Me.m_tbxInfoVersion.Text = ""
                    Me.m_tbxInfoAuthor.Text = ""
                    Me.m_tbxInfoContact.Text = ""
                    Me.m_tbxInfoDescription.Text = ""
                    Me.m_cbGameCalcIndicators.Checked = False
                End If
                Me.m_bInupdate = False

                Me.m_gridPressureMappings.Game = game
                Me.m_gridEmulTestset.Game = game
                Me.FillOutputListbox()

                Dim model As cEwEModel = Me.Core.EwEModel
                Dim space As cEcospaceScenario = Me.Core.EcospaceScenarios(Me.Core.ActiveEcospaceScenarioIndex)

                Me.m_testdata.Load(Me.Core.EwEModel.Name & "_" & space.DBID, game)

                Me.UpdateControls()

            Catch ex As Exception
                Me.m_bInupdate = False
            End Try

        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Event handler, called when a <see cref="CGame">game</see> definition needs 
        ''' to be loaded from the MEL JSON file.
        ''' </summary>
        ''' <param name="sender">Ignored.</param>
        ''' <param name="e">Ignored</param>
        ''' -----------------------------------------------------------------------
        Private Sub OnAddGameFromJSONFile(sender As Object, e As EventArgs) _
            Handles m_btnGameAddFromJSON.Click

            Try
                Dim cmd As cFileOpenCommand = CType(Me.CommandHandler.GetCommand(cFileOpenCommand.COMMAND_NAME), cFileOpenCommand)
                cmd.Invoke("MSP JSON files|*.json", 0, "Select MSP JSON file to load")

                If (cmd.Result = DialogResult.OK) Then
                    Try
                        Dim cfg As New cJSONGameConfig()
                        cfg.LoadFile(cmd.FileName)
                        ' Validate Ecospace params
                        Dim g As New cGame(Me.Core)
                        g.Name = cfg.Mode
                        g.Author = Me.Core.DefaultAuthor
                        g.Contact = Me.Core.DefaultContact
                        g.EcosimID = Me.Core.EcosimScenarios(Me.Core.ActiveEcosimScenarioIndex).DBID
                        g.EcospaceID = Me.Core.EcospaceScenarios(Me.Core.ActiveEcospaceScenarioIndex).DBID
                        g.OutcomeRange = cfg.OutcomeRange

                        For Each p As cPressure In cfg.Pressures
                            g.Add(p)
                        Next

                        For Each outcome As cGrid In cfg.Outcomes
                            Dim t As cOutcome.eLayerType = cOutcome.eLayerType.Biomass
                            For Each test As cOutcome.eLayerType In [Enum].GetValues(GetType(cOutcome.eLayerType))
                                If (outcome.Name.ToLower.Contains(test.ToString.ToLower)) Then
                                    t = test
                                End If
                            Next
                            g.Add(New cOutcome(Me.Core, outcome.Name, t))
                        Next

                        Me.Shell.Data.Add(g)
                        Me.Shell.OnChanged()
                        Me.FillGameCombo(g)
                        Me.UpdateControls()

                    Catch ex As Exception

                    End Try

                End If
            Catch ex As Exception

            End Try
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Event handler, called when the users wishes to add a <see cref="CGame">game</see>.
        ''' </summary>
        ''' <param name="sender">Ignored.</param>
        ''' <param name="e">Ignored</param>
        ''' -----------------------------------------------------------------------
        Private Sub OnAddGame(sender As Object, e As EventArgs) _
            Handles m_btnGameAdd.Click

            Try
                Dim g As New cGame(Me.Core)
                g.Name = Me.m_tbxGameName.Text
                g.Author = Me.Core.DefaultAuthor
                g.Contact = Me.Core.DefaultContact
                g.EcosimID = Me.Core.EcosimScenarios(Me.Core.ActiveEcosimScenarioIndex).DBID
                g.EcospaceID = Me.Core.EcospaceScenarios(Me.Core.ActiveEcospaceScenarioIndex).DBID

                g.AddDefaultPressures()

                Me.Shell.Data.Add(g)
                Me.Shell.OnChanged()
                Me.FillGameCombo(g)
                Me.UpdateControls()

            Catch ex As Exception

            End Try
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Event handler, called when the users wishes to rename the selected <see cref="CGame">game</see>.
        ''' </summary>
        ''' <param name="sender">Ignored.</param>
        ''' <param name="e">Ignored</param>
        ''' -----------------------------------------------------------------------
        Private Sub OnRenameGame(sender As Object, e As EventArgs) _
            Handles m_btnGameRename.Click
            Try
                Dim g As cGame = Me.SelectedGame()
                If (g IsNot Nothing) Then
                    g.Name = Me.m_tbxGameName.Text
                    Me.Shell.OnChanged()
                    Me.FillGameCombo(g)
                End If
            Catch ex As Exception

            End Try
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Event handler, called when the users wishes to delete the selected <see cref="CGame">game</see>.
        ''' </summary>
        ''' <param name="sender">Ignored.</param>
        ''' <param name="e">Ignored</param>
        ''' -----------------------------------------------------------------------
        Private Sub OnDeleteGame(sender As Object, e As EventArgs) _
            Handles m_btnGameDelete.Click
            Try
                Me.Shell.Data.Remove(Me.SelectedGame())
                Me.Shell.OnChanged()
                Me.FillGameCombo()
                Me.UpdateControls()
            Catch ex As Exception

            End Try
        End Sub

#End Region ' Game

#Region " Game info and game settings "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Event handler, called when the users whas modified the Ecospace run
        ''' settings.
        ''' </summary>
        ''' <param name="sender">Ignored.</param>
        ''' <param name="e">Ignored</param>
        ''' -----------------------------------------------------------------------
        Private Sub OnInfoOrSettingsChanged(sender As Object, e As EventArgs) _
            Handles m_tbxInfoVersion.TextChanged, m_tbxInfoAuthor.TextChanged, m_tbxInfoContact.TextChanged, m_tbxInfoDescription.TextChanged,
                    m_fpSpinupYears.OnValueChanged, m_fpRunYears.OnValueChanged, m_fpMAPCellClosure.OnValueChanged, m_cbGameCalcIndicators.CheckedChanged, m_tbxInfoDescription.TextChanged
            Try
                If (Me.m_bInupdate) Then Return
                Dim g As cGame = Me.SelectedGame()
                If (g IsNot Nothing) Then
                    g.Version = Me.m_tbxInfoVersion.Text
                    g.Author = Me.m_tbxInfoAuthor.Text
                    g.Contact = Me.m_tbxInfoContact.Text
                    g.Description = Me.m_tbxInfoDescription.Text
                    g.SpinupYears = CInt(Me.m_fpSpinupYears.Value)
                    g.RunYears = CInt(Me.m_fpRunYears.Value)
                    g.MPACellClosureRatio = CSng(Me.m_fpMAPCellClosure.Value)
                    g.CalculateIndicators = Me.m_cbGameCalcIndicators.Checked
                    Me.Shell.OnChanged()
                End If
            Catch ex As Exception
                cLog.Write(ex)
            End Try
            Me.UpdateControls()
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Event handler, called when the users wishes to review Ecosim time series.
        ''' </summary>
        ''' <param name="sender">Ignored.</param>
        ''' <param name="e">Ignored</param>
        ''' -----------------------------------------------------------------------
        Private Sub OnChangeTimeSeries(sender As Object, e As EventArgs) _
            Handles m_lblCheckSimTimeSeries.Click
            Try
                Dim cmd As cCommand = Me.CommandHandler.GetCommand("LoadTimeSeries")
                If (cmd IsNot Nothing) Then cmd.Invoke()
            Catch ex As Exception
                cLog.Write(ex)
            End Try
        End Sub

        Private Sub OnUseCurrentScenario_Click(sender As Object, e As EventArgs) _
            Handles m_btnSettingsUseCurrentScenario.Click
            Try
                Dim g As cGame = Me.SelectedGame()
                If (g IsNot Nothing) Then
                    Dim space As cEcospaceScenario = Me.Core.EcospaceScenarios(Me.Core.ActiveEcospaceScenarioIndex)
                    g.EcospaceID = space.DBID
                    Me.Shell.OnChanged()
                    Me.FillGameCombo(g)
                End If
            Catch ex As Exception

            End Try
            Me.UpdateControls()
        End Sub

        ' ToDo: add label click handlers for sim and space too. Even better: make the labels handle this themselves

#End Region ' Game info and game settings "

#Region " Pressures "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Event handler, called when the users wishes to add a <see cref="cPressure">pressure</see>.
        ''' </summary>
        ''' <param name="sender">Ignored.</param>
        ''' <param name="e">Ignored</param>
        ''' -----------------------------------------------------------------------
        Private Sub OnAddPressure(sender As Object, e As EventArgs) _
            Handles m_btnPressureAdd.Click
            Try
                Dim type As cPressure.eDataTypes = DirectCast(Me.m_cmbPressureTypes.SelectedItem, cPressure.eDataTypes)
                Dim g As cGame = Me.SelectedGame()
                Dim p As New cPressure(Me.m_tbxPressureName.Text, type)

                g.Add(p)

                Me.Shell.OnChanged()
                Me.m_gridPressureMappings.RefreshContent()
            Catch ex As Exception

            End Try
            Me.UpdateControls()
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Event handler, called when the users wishes to rename the selected 
        ''' <see cref="cPressure">pressure</see>.
        ''' </summary>
        ''' <param name="sender">Ignored.</param>
        ''' <param name="e">Ignored</param>
        ''' -----------------------------------------------------------------------
        Private Sub OnRenamePressure(sender As Object, e As EventArgs) _
            Handles m_btnPressureRename.Click
            Try
                Dim pressure As cPressure = Me.m_gridPressureMappings.SelectedPressure
                Dim g As cGame = Me.SelectedGame()

                If (g Is Nothing) Then Return
                If (pressure Is Nothing) Then Return

                Dim strOldName As String = pressure.Name
                Dim strNewName As String = Me.m_tbxPressureName.Text

                If (String.Compare(strOldName, strNewName, False) = 0) Then Return

                ' Reroute mappings
                g.Driver(strNewName) = g.Driver(strOldName)
                g.Driver(strOldName) = Nothing

                pressure.Name = strNewName

                Me.m_gridPressureMappings.RefreshContent()
                Me.Shell.OnChanged()
            Catch ex As Exception

            End Try
            Me.UpdateControls()
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Event handler, called when the users wishes to delete the selected 
        ''' <see cref="cPressure">pressure</see>.
        ''' </summary>
        ''' <param name="sender">Ignored.</param>
        ''' <param name="e">Ignored</param>
        ''' -----------------------------------------------------------------------
        Private Sub OnDeletePressure(sender As Object, e As EventArgs) Handles m_btnPressureDelete.Click
            Try
                Dim pressure As cPressure = Me.m_gridPressureMappings.SelectedPressure
                Dim g As cGame = Me.SelectedGame()

                If (g Is Nothing) Then Return
                If (pressure Is Nothing) Then Return

                ' ToDo: add deletion confirmation message

                ' Reroute mappings
                g.Driver(pressure.Name) = Nothing
                g.Remove(pressure)

                Me.m_gridPressureMappings.RefreshContent()
                Me.Shell.OnChanged()
            Catch ex As Exception

            End Try
            Me.UpdateControls()
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Event handler, called when the users wishes to create default 
        ''' <see cref="cPressure">pressures</see>.
        ''' </summary>
        ''' <param name="sender">Ignored.</param>
        ''' <param name="e">Ignored</param>
        ''' -----------------------------------------------------------------------
        Private Sub OnCreateDefaultPressures(sender As Object, e As EventArgs) Handles m_btnPressureDefaults.Click

            Dim g As cGame = Me.SelectedGame()

            If (g Is Nothing) Then Return
            g.AddDefaultPressures()

            Me.m_gridPressureMappings.RefreshContent()
            Me.Shell.OnChanged()

        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Event handler, called when the users has selected a <see cref="cPressure">pressure</see>.
        ''' </summary>
        ''' <param name="selection">Ignored.</param>
        ''' -----------------------------------------------------------------------
        Private Sub OnPressureSelected(selection As CellVirtualCollection) _
            Handles m_gridPressureMappings.OnSelectionChanged

            Dim p As cPressure = Me.m_gridPressureMappings.SelectedPressure
            If (p IsNot Nothing) Then
                Me.m_tbxPressureName.Text = p.Name
            End If
            Me.UpdateControls()

        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Event handler, called when the users has connected changed  a 
        ''' <see cref="cPressure">pressure</see> to <see cref="cDriver">driver</see> 
        ''' mapping.
        ''' </summary>
        ''' <param name="sender">Ignored.</param>
        ''' -----------------------------------------------------------------------
        Private Sub OnMappingsChanged(sender As gridDrivers) _
            Handles m_gridPressureMappings.OnMappingsChanged

            Me.m_gridEmulTestset.RefreshContent()
            Me.UpdateControls()

        End Sub

#End Region ' Pressures

#Region " Outputs "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Event handler, called when the users wishes to add a <see cref="cOutcome">output</see>.
        ''' </summary>
        ''' <param name="sender">Ignored.</param>
        ''' <param name="e">Ignored</param>
        ''' -----------------------------------------------------------------------
        Private Sub OnAddOutput(sender As Object, e As EventArgs) _
            Handles m_btnOutcomeAdd.Click
            Try
                Dim type As cOutcome.eLayerType = DirectCast(Me.m_cmbOutputTypes.SelectedItem, cOutcome.eLayerType)
                Dim output As New cOutcome(Me.UIContext.Core, Me.m_tbxOutcomeName.Text, type)
                Dim game As cGame = Me.SelectedGame()

                game.Add(output)

                Me.FillOutputListbox()
                Me.Shell.OnChanged()
                Me.m_lbOutputs.SelectedItem = output
            Catch ex As Exception

            End Try
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Event handler, called when the users wishes to rename the selected 
        ''' <see cref="cOutcome">output</see>.
        ''' </summary>
        ''' <param name="sender">Ignored.</param>
        ''' <param name="e">Ignored</param>
        ''' -----------------------------------------------------------------------
        Private Sub OnRenameOutput(sender As Object, e As EventArgs) Handles m_btnOutcomeRename.Click
            Try
                Dim output As cOutcome = DirectCast(Me.m_lbOutputs.SelectedItem, cOutcome)
                If (output Is Nothing) Then Return

                Dim type As cOutcome.eLayerType = DirectCast(Me.m_cmbOutputTypes.SelectedItem, cOutcome.eLayerType)

                output.Name = Me.m_tbxOutcomeName.Text
                output.LayerType = type

                Me.FillOutputListbox()
                Me.Shell.OnChanged()
                Me.m_lbOutputs.SelectedItem = output
            Catch ex As Exception

            End Try
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Event handler, called when the users wishes to delete the selected 
        ''' <see cref="cOutcome">output</see>.
        ''' </summary>
        ''' <param name="sender">Ignored.</param>
        ''' <param name="e">Ignored</param>
        ''' -----------------------------------------------------------------------
        Private Sub OnDeleteOutput(sender As Object, e As EventArgs) Handles m_btnOutcomeDelete.Click
            Try

                Dim g As cGame = Me.SelectedGame()
                For Each item As Object In Me.m_lbOutputs.SelectedItems
                    g.Remove(DirectCast(item, cOutcome))
                Next

                Me.FillOutputListbox()
                Me.OnOutputSelected(Nothing, Nothing)
                Me.Shell.OnChanged()

            Catch ex As Exception

            End Try
        End Sub


        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Event handler, called when the users has selected an <see cref="cOutcome">output</see>
        ''' for configuration.
        ''' </summary>
        ''' <param name="sender">Ignored.</param>
        ''' <param name="e">Ignored</param>
        ''' -----------------------------------------------------------------------
        Private Sub OnOutputSelected(sender As Object, e As EventArgs) _
            Handles m_lbOutputs.SelectedIndexChanged

            If (Me.m_bInupdate) Then Return ' To prevent the items checked listbox from refilling

            If (Me.m_lbOutputs.SelectedItems.Count = 1) Then

                Dim out As cOutcome = CType(Me.m_lbOutputs.SelectedItem, cOutcome)
                If (out Is Nothing) Then Return

                Me.m_tbxOutcomeName.Text = out.Name
                Me.m_cmbOutputTypes.SelectedItem = out.LayerType

            End If

            Me.FillOutputOptionsGrid()
            Me.UpdateControls()

        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Event handler, called when the users has changed the configuration of an
        ''' <see cref="cOutcome">output</see>.
        ''' </summary>
        ''' <param name="sender">Ignored.</param>
        ''' -----------------------------------------------------------------------
        Private Sub OnOutputChanged(sender As gridOutcomes) Handles m_gridOutcome.OnMappingsChanged

            Dim out As cOutcome = CType(Me.m_lbOutputs.SelectedItem, cOutcome)
            Me.m_bInupdate = True
            Me.m_lbOutputs.Items(Me.m_lbOutputs.SelectedIndex) = Me.m_lbOutputs.SelectedItem
            Me.m_bInupdate = False
            Me.Shell.OnChanged()
            Me.UpdateControls()

        End Sub

#End Region ' Outputs

#Region " Emulator "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Event handler, called when the users wishes to add a <see cref="cTestset">test set</see>.
        ''' </summary>
        ''' <param name="sender">Ignored.</param>
        ''' <param name="e">Ignored</param>
        ''' -----------------------------------------------------------------------
        Private Sub OnAddTestset(sender As Object, e As EventArgs) Handles m_btnTestsetAdd.Click

            Dim g As cGame = Me.SelectedGame()
            If (g Is Nothing) Then Return

            Dim t As New cTestset(Me.m_tbxTestsetName.Text, g)
            Me.m_testdata.Testsets.Add(t)

            Me.m_cmbEmulTestsets.Items.Add(t)
            Me.m_cmbEmulTestsets.SelectedItem = t

            Me.m_testdata.Save()
            Me.UpdateControls()

        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Event handler, called when the users wishes to rename the selected 
        ''' <see cref="cTestset">test set</see>.
        ''' </summary>
        ''' <param name="sender">Ignored.</param>
        ''' <param name="e">Ignored</param>
        ''' -----------------------------------------------------------------------
        Private Sub OnRenameTestset(sender As Object, e As EventArgs) Handles m_btnTestsetRename.Click

            Dim g As cGame = Me.SelectedGame()
            If (g Is Nothing) Then Return

            Dim tsel As cTestset = Me.SelectedTestset
            If (tsel Is Nothing) Then Return

            tsel.Name = Me.m_tbxTestsetName.Text
            Me.m_cmbEmulTestsets.SelectedItem = tsel

            Me.m_testdata.Save()
            Me.UpdateControls()

        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Event handler, called when the users wishes to delete the selected 
        ''' <see cref="cTestset">test set</see>.
        ''' </summary>
        ''' <param name="sender">Ignored.</param>
        ''' <param name="e">Ignored</param>
        ''' -----------------------------------------------------------------------
        Private Sub OnDeleteTestset(sender As Object, e As EventArgs) Handles m_btnTestsetDelete.Click

            Dim g As cGame = Me.SelectedGame()
            If (g Is Nothing) Then Return

            Dim tsel As cTestset = Me.SelectedTestset
            If (tsel Is Nothing) Then Return

            Me.m_cmbEmulTestsets.Items.Remove(tsel)
            Me.m_testdata.Testsets.Remove(tsel)

            Me.m_testdata.Save()
            Me.UpdateControls()

        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Event handler, called when the users has selected a <see cref="cTestset">test set</see>
        ''' for configuration.
        ''' </summary>
        ''' <param name="sender">Ignored.</param>
        ''' <param name="e">Ignored</param>
        ''' -----------------------------------------------------------------------
        Private Sub OnTestsetSelected(sender As Object, e As EventArgs) _
            Handles m_cmbEmulTestsets.SelectedIndexChanged

            Me.m_gridEmulTestset.Testset = Me.SelectedTestset
            Me.UpdateControls()

        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Event handler, called when the users has changed the configuratio of a
        ''' <see cref="cTestset">test set</see>.
        ''' </summary>
        ''' <param name="sender">Ignored.</param>
        ''' <param name="t">Ignored</param>
        ''' -----------------------------------------------------------------------
        Private Sub OnTestsetChanged(sender As Object, t As cTestset) _
            Handles m_gridEmulTestset.OnTestsetChanged
            Me.m_testdata.Save()
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Event handler, called when the users wishes to load a <see cref="cTestset">test set</see>
        ''' into connected Ecospace <see cref="cDriver">drivers</see>.
        ''' </summary>
        ''' <param name="sender">Ignored.</param>
        ''' <param name="e">Ignored</param>
        ''' -----------------------------------------------------------------------
        Private Sub OnEmulApply(sender As Object, e As EventArgs) Handles m_btnTestsetApply.Click
            Me.ApplyTestset()
            Me.UpdateControls()
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Event handler, called when the users has toggled the Ecospace pause option.
        ''' <see cref="cTestset">test set</see>.
        ''' </summary>
        ''' <param name="sender">Ignored.</param>
        ''' <param name="e">Ignored</param>
        ''' -----------------------------------------------------------------------
        Private Sub OnPauseEcospaceCheckChanged(sender As Object, e As EventArgs) _
            Handles m_cbEmulPauseSpace.CheckedChanged
            Me.UpdateControls()
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Event handler, called when the users wishes to advance the current 
        ''' Ecospace run to the next pause point.
        ''' </summary>
        ''' <param name="sender">Ignored.</param>
        ''' <param name="e">Ignored</param>
        ''' -----------------------------------------------------------------------
        Private Sub OnEmulStep(sender As Object, e As EventArgs) _
            Handles m_btnEmulStep.Click

            Dim sm As cCoreStateMonitor = Me.Core.StateMonitor
            If (Not sm.IsEcospaceRunning) Then Return

            ' Make Ecospace move on
            Me.Core.EcospacePaused = False
            Me.UpdateControls()

        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Event handler, called when the users wishes to stop the current Ecospace 
        ''' run.
        ''' </summary>
        ''' <param name="sender">Ignored.</param>
        ''' <param name="e">Ignored</param>
        ''' -----------------------------------------------------------------------
        Private Sub OnEmulStop(sender As Object, e As EventArgs) Handles m_btnEmulStop.Click

            Me.Core.StopEcospace()
            Me.UpdateControls()

        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Event handler, called when the users wishes to view to folder where
        ''' MSP output files have been stored.
        ''' </summary>
        ''' <param name="sender">Ignored.</param>
        ''' <param name="e">Ignored</param>
        ''' -----------------------------------------------------------------------
        Private Sub OnEmulSetOutcomeRange(sender As Object, e As EventArgs) Handles m_nudEmulOutcomeRange.ValueChanged

            Dim g As cGame = Me.SelectedGame()
            If (g Is Nothing) Then Return
            g.OutcomeRange = Me.m_nudEmulOutcomeRange.Value

        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Event handler, called when the users wishes to view to folder where
        ''' MSP output files have been stored.
        ''' </summary>
        ''' <param name="sender">Ignored.</param>
        ''' <param name="e">Ignored</param>
        ''' -----------------------------------------------------------------------
        Private Sub OnEmulViewOutputFolder(sender As Object, e As EventArgs) _
            Handles m_btnEmulViewOutputFolder.Click

            Try
                Dim cmd As cBrowserCommand = CType(Me.UIContext.CommandHandler.GetCommand(cBrowserCommand.COMMAND_NAME), cBrowserCommand)
                cmd.Invoke(Me.OutputPath)
            Catch ex As Exception

            End Try

        End Sub

        Private Sub OnSaveOutputMapsChanged(sender As Object, e As EventArgs) _
            Handles m_cbSaveOutputMaps.CheckedChanged

            Try
                Me.UpdateControls()
            Catch ex As Exception

            End Try

        End Sub



#End Region ' Emulator

#Region " Credits "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Event handler, called when the users wishes to visit the RWS site.
        ''' </summary>
        ''' <param name="sender">Ignored.</param>
        ''' <param name="e">Ignored</param>
        ''' -----------------------------------------------------------------------
        Private Sub OnVisitRWS(sender As Object, e As EventArgs) Handles m_pbRWS.Click
            Me.Visit("http://www.rws.nl/")
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Event handler, called when the users wishes to visit the EII site.
        ''' </summary>
        ''' <param name="sender">Ignored.</param>
        ''' <param name="e">Ignored</param>
        ''' -----------------------------------------------------------------------
        Private Sub OnVisitEII(sender As Object, e As EventArgs) Handles m_pbEII.Click
            Me.Visit("http://www.ecopathinternational.org")
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Event handler, called when the users wishes to visit the NHTV site.
        ''' </summary>
        ''' <param name="sender">Ignored.</param>
        ''' <param name="e">Ignored</param>
        ''' -----------------------------------------------------------------------
        Private Sub OnVisitNHTV(sender As Object, e As EventArgs) Handles m_pbNHTV.Click
            Me.Visit("http://www.nhtv.nl/")
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Event handler, called when the users wishes to visit the MSP site.
        ''' </summary>
        ''' <param name="sender">Ignored.</param>
        ''' <param name="e">Ignored</param>
        ''' -----------------------------------------------------------------------
        Private Sub OnVisitMSP2050(sender As Object, e As EventArgs) Handles m_pbMSPC2050.Click
            Me.Visit("http://www.mspchallenge.info/")
        End Sub

#End Region ' Credits

#End Region ' Control events

#Region " Internals "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the <see cref="cEwEShell">EwE MSP shell</see> to operate onto.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Friend Property Shell As cEwEShell = Nothing

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns the currently selected <see cref="CTestset">test set</see>.
        ''' </summary>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Private Function SelectedTestset() As cTestset
            Return DirectCast(Me.m_cmbEmulTestsets.SelectedItem, cTestset)
        End Function

        Private Sub FillGameCombo(Optional sel As cGame = Nothing)

            Me.m_cmbGames.Items.Clear()
            For Each cfg As cGame In Me.Shell.Data.Games
                Me.m_cmbGames.Items.Add(cfg)
            Next
            If sel IsNot Nothing Then
                Me.m_cmbGames.SelectedItem = sel
            ElseIf (Me.m_cmbGames.Items.Count > 0) Then
                Me.m_cmbGames.SelectedIndex = 0
            End If

            Me.OnGameSelected(Me, Nothing)
        End Sub

        Private Sub FillPressureTypesCombo()

            Me.m_cmbPressureTypes.Items.Clear()
            For Each t As cPressure.eDataTypes In [Enum].GetValues(GetType(cPressure.eDataTypes))
                If (t <> cPressure.eDataTypes.NotSet) Then
                    Me.m_cmbPressureTypes.Items.Add(t)
                End If
            Next
            Me.m_cmbPressureTypes.SelectedIndex = 0

        End Sub

        Private Sub FillOutputTypesCombo()

            Me.m_cmbOutputTypes.Items.Clear()
            For Each t As cOutcome.eLayerType In [Enum].GetValues(GetType(cOutcome.eLayerType))
                Me.m_cmbOutputTypes.Items.Add(t)
            Next
            Me.m_cmbOutputTypes.SelectedIndex = 0

        End Sub

        Private Sub FillOutputListbox()

            If (Me.m_bInupdate) Then Return

            Me.m_bInupdate = True
            Try
                Dim g As cGame = Me.SelectedGame()
                Me.m_lbOutputs.Items.Clear()
                If (g IsNot Nothing) Then
                    For Each out As cOutcome In g.Outputs
                        Me.m_lbOutputs.Items.Add(out)
                    Next
                End If
            Catch ex As Exception

            End Try
            Me.m_bInupdate = False

        End Sub

        Private Sub FillOutputOptionsGrid()

            Me.m_gridOutcome.Output = Nothing
            Me.m_gridOutcome.RefreshContent()

            If (Me.m_lbOutputs.SelectedItems.Count <> 1) Then Return

            Dim out As cOutcome = CType(Me.m_lbOutputs.SelectedItem, cOutcome)
            Me.m_gridOutcome.Output = out
            Me.m_gridOutcome.RefreshContent()

        End Sub

        Private Sub FillTestsetCombo(Optional sel As cTestset = Nothing)
            Me.m_cmbEmulTestsets.Items.Clear()
            For Each [set] As cTestset In Me.m_testdata.Testsets
                Me.m_cmbEmulTestsets.Items.Add([set])
            Next
            If (sel IsNot Nothing) Then
                Me.m_cmbEmulTestsets.SelectedItem = sel
            ElseIf (Me.m_cmbEmulTestsets.Items.Count > 0) Then
                Me.m_cmbEmulTestsets.SelectedIndex = 0
            End If
        End Sub

        Private Sub ApplyTestset()

            Dim g As cGame = Me.SelectedGame()
            If (g Is Nothing) Then Return

            Dim pressures As New List(Of cPressure)
            Dim testset As cTestset = Me.SelectedTestset()
            Dim bm As cEcospaceBasemap = Me.Core.EcospaceBasemap
            Dim msg As New cMessage(cStringUtils.Localize(My.Resources.STATUS_TESTSET_LOAD_SUCCESS, testset.Name),
                                    eMessageType.DataImport, eCoreComponentType.External, eMessageImportance.Information)

            Try
                If (testset IsNot Nothing) Then
                    For Each p As cPressure In testset.Pressures
                        Dim data As String = testset.Testdata(p)
                        If (Not String.IsNullOrWhiteSpace(data)) Then
                            Dim vs As cVariableStatus = Nothing
                            Select Case p.DataType
                                Case cPressure.eDataTypes.Grid
                                    Dim psim As New cPressure(p.Name, bm.InCol, bm.InRow)
                                    If psim.Grid.Load(data, Me.UIContext.Core) Then
                                        pressures.Add(psim)
                                    Else
                                        msg.Message = cStringUtils.Localize(My.Resources.STATUS_TESTSET_LOAD_FAILED, testset.Name)
                                        vs = New cVariableStatus(eStatusFlags.ErrorEncountered, cStringUtils.Localize(My.Resources.STATUS_TESTDATA_MAP_REJECTED, p.Name, data),
                                                                 eVarNameFlags.NotSet, eDataTypes.NotSet, eCoreComponentType.EcoSpace, 0)
                                    End If
                                    msg.AddVariable(vs)
                                Case cPressure.eDataTypes.Scalar
                                    If Double.TryParse(data, p.Scalar) Then
                                        pressures.Add(p)
                                    End If
                                Case Else
                                    Debug.Assert(False)
                            End Select
                        End If

                    Next
                End If

                ' Pass pressures on
                g.ApplyPressures(pressures.ToArray())

            Catch ex As Exception
                ' Eek!
            End Try

            Me.Core.Messages.SendMessage(msg)

        End Sub

        Private Sub FillStopOptionsCombo()
            ' Populated in form designer. Just select first item
            Me.m_cmbEmulPauseOptions.SelectedIndex = 0
        End Sub

        Private Function ShowModelStatus(lbl As cImageLabel, bOK As Boolean, strTextOK As String, strTextNotOk As String) As Boolean
            lbl.Image = If(bOK, SharedRecources.OK, SharedRecources.Warning)
            lbl.ForeColor = If(bOK, SystemColors.ControlText, Color.Red)
            lbl.Text = If(bOK, strTextOK, strTextNotOk)
            Return bOK
        End Function

        Private Sub SetTabStatusImage(tc As TabPage, iStatusImageIndex As Integer)
            tc.ImageIndex = iStatusImageIndex
        End Sub

        Private Sub OnEcospaceTimeStep(ByRef data As cEcospaceTimestep)

            ' Populate outputs
            Dim g As cGame = Me.SelectedGame()
            Dim outcomes As New List(Of cGrid)
            Dim bm As cEcospaceBasemap = Me.UIContext.Core.EcospaceBasemap
            Dim parms As cEcospaceModelParameters = Me.UIContext.Core.EcospaceModelParameters

            For Each o As cOutcome In g.Outputs
                Dim grid As New cGrid(o.Name, bm.InCol, bm.InRow)
                grid.IsValid = False
                outcomes.Add(grid)
            Next
            g.LoadOutcomes(outcomes.ToArray(), data)

            ' Save outputs
            If (Me.m_cbSaveOutputMaps.Checked) Then
                ' Prep msg
                Dim msg As New cMessage("MSP outcomes saved to disk for Ecospace timestep " & data.iTimeStep, eMessageType.DataExport, eCoreComponentType.EcoSpace, eMessageImportance.Information)

                For Each grid As cGrid In outcomes

                    If (grid.IsValid) Then

                        Dim strFile As String = cFileUtils.ToValidFileName("outcome_" & grid.Name & "_" & data.iTimeStep.ToString("D4") & ".asc", False)
                        Dim vs As cVariableStatus = Nothing

                        Try
                            If (grid.Save(Path.Combine(Me.OutputPath, strFile), Me.Core)) Then
                                vs = New cVariableStatus(eStatusFlags.OK, g.Name, eVarNameFlags.NotSet, eDataTypes.NotSet, eCoreComponentType.EcoSpace, 0)
                                msg.Hyperlink = Me.OutputPath
                            End If
                        Catch ex As Exception
                            vs = New cVariableStatus(eStatusFlags.ErrorEncountered, ex.Message, eVarNameFlags.NotSet, eDataTypes.NotSet, eCoreComponentType.EcoSpace, 0)
                        End Try

                        If (vs IsNot Nothing) Then
                            msg.AddVariable(vs)
                        End If
                    End If
                Next

                If (msg.Variables.Count > 0) Then
                    Me.Core.Messages.SendMessage(msg)
                End If

            End If

            If (Me.m_cbEmulPauseSpace.Checked) Then
                Dim bPause As Boolean = False
                Select Case Me.m_cmbEmulPauseOptions.SelectedIndex
                    Case 0 : bPause = True
                    Case 1 : bPause = (data.iTimeStep Mod parms.NumberOfTimeStepsPerYear) = 0
                    Case 2 : bPause = (data.iTimeStep Mod (5 * parms.NumberOfTimeStepsPerYear)) = 0
                End Select
                If (bPause = True) Then
                    cSoundUtilities.PlaySound(My.Resources.block)
                    Me.Pulse(eMessageImportance.Information, 5)
                End If
                Me.Core.EcospacePaused = bPause
            End If

            BeginInvoke(New MethodInvoker(AddressOf UpdateControls))

        End Sub

        Private Function OutputPath() As String
            Dim g As cGame = Me.SelectedGame()
            If (g Is Nothing) Then Return ""
            Dim strPath As String = Path.Combine(Core.DefaultOutputPath(eAutosaveTypes.EcospaceResults), "MSP")
            Return Path.Combine(strPath, cFileUtils.ToValidFileName(g.Name, False))
        End Function

        Private Sub Visit(strURL As String)

            Try
                Dim cmd As cBrowserCommand = CType(Me.UIContext.CommandHandler.GetCommand(cBrowserCommand.COMMAND_NAME), cBrowserCommand)
                cmd.Invoke(strURL)
            Catch ex As Exception

            End Try

        End Sub

        Private Sub OnExportGameData(sender As Object, e As EventArgs) Handles m_btnExport.Click

            Dim ds As IEwEDataSource = Me.Core.DataSource
            Dim file As String = Path.Combine(ds.Directory, ds.FileName) & "_MSPgames.xml"
            Dim msg As cMessage = Nothing

            If Me.Shell.SaveConfiguration(file) Then
                msg = New cMessage(cStringUtils.Localize(My.Resources.STATUS_GAME_EXPORT_SUCCESS, file), eMessageType.DataExport, eCoreComponentType.External, eMessageImportance.Information)
                msg.Hyperlink = Path.GetDirectoryName(file)
            Else
                msg = New cMessage(cStringUtils.Localize(My.Resources.STATUS_GAME_EXPORT_FAILED, file), eMessageType.DataExport, eCoreComponentType.External, eMessageImportance.Critical)
            End If

            Me.Core.Messages.SendMessage(msg)

        End Sub

#End Region ' Internals

    End Class

End Namespace
