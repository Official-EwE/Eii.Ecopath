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
Imports System.Security
Imports System.Windows.Forms
Imports System.Xml
Imports EwECore
Imports EwECore.DataSources
Imports EwEMSPLink
Imports EwEMSPPlugin.Emulator
Imports EwEUtils.Core
Imports EwEUtils.Utilities
Imports ScientificInterfaceShared.Commands
Imports ScientificInterfaceShared.Controls
Imports ScientificInterfaceShared.Controls.EwEGrid
Imports SharedRecources = ScientificInterfaceShared.My.Resources

#End Region ' Imports

' TODO: Crashes happen when opening MSP tools UI during an Ecospace run, when outcomes have not been calibrated
' Change: move time step control from form to MSP Tools plug-in, and handle pausing there.
' --- THIS IS JUST A UI ISSUE, AND DOES NOT AFFECT LIVE GAMES ---

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
        Private WithEvents m_fpBycatchFee As cEwEFormatProvider = Nothing

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
        ''' <param name="shell">The <see cref="cEwEMSPLink"/> that contains MSP game info.</param>
        ''' <param name="data">The <see cref="cEcospaceDataStructures"/> to work against.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(uic As cUIContext, shell As cEwEMSPLink, data As cEcospaceDataStructures)

            Me.SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.OptimizedDoubleBuffer Or ControlStyles.ResizeRedraw, True)

            Me.InitializeComponent()

            Me.UIContext = uic
            Me.m_spacedata = data

            Me.m_testdata = New cTestSetData()
            Me.MSPLink = shell
            Me.m_gridPressureMappings.Shell = Me.MSPLink
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

            Dim ge As cOperatorBase = cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo)
            Dim le As cOperatorBase = cOperatorManager.getOperator(eOperators.LessThanOrEqualTo)

            Me.m_tsbnImport.Image = SharedRecources.ImportHS
            Me.m_tsbnExport.Image = SharedRecources.ExportHS

            Me.m_ilTabIcons.Images.Add(SharedRecources.OK)
            Me.m_ilTabIcons.Images.Add(SharedRecources.Warning)
            Me.m_ilTabIcons.Images.Add(SharedRecources.Critical)

            Me.m_gridPressureMappings.UIContext = Me.UIContext
            Me.m_gridOutcome.UIContext = Me.UIContext
            Me.m_gridEmulTestset.UIContext = Me.UIContext

            Me.m_fpSpinupYears = New cEwEFormatProvider(Me.UIContext, Me.m_tbxSpinupYears, GetType(Integer))
            Me.m_fpRunYears = New cEwEFormatProvider(Me.UIContext, Me.m_tbxRunYears, GetType(Integer))
            Me.m_fpMAPCellClosure = New cEwEFormatProvider(Me.UIContext, Me.m_tbxMPACellClosure, GetType(Single), metadata:=New cVariableMetaData(0, 1, ge, le, 0.25))
            Me.m_fpBycatchFee = New cEwEFormatProvider(Me.UIContext, Me.m_tbxBycatchFee, GetType(Single), metadata:=New cVariableMetaData(0, 1000, ge, le, 10))

            Me.m_checkEcosimTimeSeries = New cEcosimTimeSeriesChecker(Me.Core)
            Me.m_checkEcosimFishing = New cEcosimFishingChecker(Me.Core)
            Me.m_checkEcosimForcing = New cEcosimForcingChecker(Me.Core)
            Me.m_checkEcospaceTimeSeries = New cEcospaceTimeSeriesChecker(Me.Core)

            Me.m_cbGameCalcIndicators.Checked = False

            Me.m_qeh = New cQuickEditHandler()
            Me.m_qeh.Attach(Me.m_gridOutcome, Me.UIContext, Me.m_tsOutcome, False)

            Me.Core.AddEcospaceTimeStepHandler(Me.m_dgtTimeStep)
            Me.FillGamesCombo()
            Me.FillPressureTypesCombo()
            Me.FillOutputTypesCombo()
            Me.FillTestsetCombo()
            Me.FillStopOptionsCombo()

            ' For the benefit of the requirement checkers. This could have been encapsulated a bit more neatly, but ok...
            Me.CoreComponents = New eCoreComponentType() {eCoreComponentType.TimeSeries, eCoreComponentType.Ecospace, eCoreComponentType.ShapesManager}
            Me.CoreExecutionState = eCoreExecutionState.EcospaceLoaded

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

            Me.m_qeh.Detach()

            Me.m_testdata.Close()

            Me.Core.RemoveEcospaceTimeStepHandler(Me.m_dgtTimeStep)
            Me.m_dgtTimeStep = Nothing

            Me.m_fpSpinupYears.Release()
            Me.m_fpRunYears.Release()
            Me.m_fpMAPCellClosure.Release()
            Me.m_fpBycatchFee.Release()

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

        Public Sub Prod()
            Me.BeginInvoke(New MethodInvoker(AddressOf UpdateControls))
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Overridden to update the state of controls in this form.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Protected Overrides Sub UpdateControls()
            MyBase.UpdateControls()

            If (Me.IsDisposed) Then Return
            If (Me.UIContext Is Nothing) Then Return
            If (Me.m_bInupdate) Then Return
            If (Me.Core.ActiveEcospaceScenarioIndex <= 0) Then Return

            Dim sm As cCoreStateMonitor = Me.Core.StateMonitor
            Dim space As cEcospaceScenario = Me.Core.EcospaceScenarios(Me.Core.ActiveEcospaceScenarioIndex)

            Dim game As cGame = Me.SelectedGame()
            Dim outcome As cOutcome = Nothing

            Dim bHasGame As Boolean = (game IsNot Nothing)
            Dim bHasGameName As Boolean = (Me.m_tstbGameName.Text.Trim.Length > 3)
            Dim bHasDuplicateGameNames As Boolean = False
            Dim bHasGameVersion As Boolean = Not String.IsNullOrWhiteSpace(Me.m_tbxInfoVersion.Text)
            Dim bHasPressureName As Boolean = Not String.IsNullOrWhiteSpace(Me.m_tbxPressureName.Text)
            Dim bHasPressureSelected As Boolean = (Me.m_gridPressureMappings.SelectedPressure IsNot Nothing)
            Dim bHasOutcomeName As Boolean = Not String.IsNullOrWhiteSpace(Me.m_tbxOutcomeName.Text)
            Dim bHasOutcomeSelected As Boolean = (Me.m_lbOutputs.SelectedIndices.Count = 1)
            Dim bHasOutcomesSelected As Boolean = (Me.m_lbOutputs.SelectedIndices.Count > 1)
            Dim bHasTestsetSelected As Boolean = (Me.m_cmbEmulTestsets.SelectedIndex > -1)
            Dim bHasTestsetName As Boolean = Not String.IsNullOrWhiteSpace(Me.m_tbxTestsetName.Text)
            Dim bHasCurrent As Boolean = False
            Dim bIsEcospaceRunning As Boolean = sm.IsEcospaceRunning

            If (game IsNot Nothing) Then
                bHasCurrent = (space.DBID = game.EcospaceID)
            End If

            If bHasOutcomeSelected And Not bHasOutcomesSelected Then
                outcome = CType(Me.m_lbOutputs.SelectedItem, cOutcome)
            End If

            Dim iImg As Integer = 0
            Dim nOK As Integer = 0

            If (bHasGame) Then
                For Each gameTest As cGame In Me.MSPLink.Data.Games
                    If (Not ReferenceEquals(game, gameTest)) Then
                        bHasDuplicateGameNames = bHasDuplicateGameNames Or (String.Compare(gameTest.Name, game.Name, True) = 0)
                    End If
                Next
            End If

            ' -- Game --
            Me.m_tsbnGameAdd.Enabled = bHasGameName And Not bIsEcospaceRunning
            Me.m_tsbnGameEdit.Enabled = bHasGameName And bHasGame And Not bIsEcospaceRunning
            Me.m_tsbnGameDelete.Enabled = bHasGame And Not bIsEcospaceRunning
            Me.m_tsddGames.Enabled = Not bIsEcospaceRunning

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
            Me.m_btnOutcomeAdd.Enabled = bHasOutcomeName
            Me.m_btnOutcomeRename.Enabled = bHasOutcomeName And bHasOutcomeSelected
            Me.m_btnOutcomeDelete.Enabled = bHasOutcomesSelected Or bHasOutcomesSelected
            Me.m_tsbnOuputRaw.Enabled = bHasOutcomeSelected
            Me.m_tsbnOuputBinned.Enabled = bHasOutcomeSelected
            If (outcome IsNot Nothing) Then
                Me.m_tsbnOuputRaw.Checked = outcome.IsRawData
                Me.m_tsbnOuputBinned.Checked = Not outcome.IsRawData
            End If

            ' -- Emulator --
            Me.m_tpEmulator.Enabled = bHasGame
            Me.m_btnEmulStep.Enabled = bIsEcospaceRunning And Me.Core.EcospacePaused
            Me.m_btnEmulStop.Enabled = bIsEcospaceRunning
            Me.m_cbSaveOutputMaps.Enabled = Not bIsEcospaceRunning
            Me.m_nudEmulOutcomeRange.Enabled = Not bIsEcospaceRunning And bHasGame
            Me.m_btnEmulViewOutputFolder.Enabled = Directory.Exists(Me.OutputPath)

            Me.m_cmbEmulTestsets.Enabled = bHasGame

            Me.m_cbEmulPauseSpace.Checked = My.Settings.PauseEcospace
            Me.m_cmbEmulPauseOptions.SelectedIndex = Math.Max(0, Math.Min(Me.m_cmbEmulPauseOptions.Items.Count - 1, My.Settings.PauseEcospaceInterval))
            Me.m_cbSaveOutputMaps.Checked = My.Settings.SaveOutputMaps

            ' Can only modify test sets when Ecospace is not running
            Me.m_btnTestsetAdd.Enabled = bHasGame And bHasTestsetName And Not bIsEcospaceRunning
            Me.m_btnTestsetRename.Enabled = bHasGame And bHasTestsetName And bHasTestsetSelected And Not bIsEcospaceRunning
            Me.m_btnTestsetDelete.Enabled = bHasGame And bHasTestsetSelected And Not bIsEcospaceRunning
            Me.m_btnTestsetApply.Enabled = bHasGame And bHasTestsetSelected
            Me.m_gridEmulTestset.Enabled = bHasGame And bHasTestsetSelected And Not bIsEcospaceRunning

            Me.ShowModelStatus(Me.m_lblCheckGame, bHasGame And Not bHasDuplicateGameNames, My.Resources.CHECK_GAME_OK, My.Resources.CHECK_GAME_FAILED)

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

                For Each out As cOutcome In game.Outcomes
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
            Return DirectCast(Me.m_tsddGames.SelectedItem, cGame)
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
            Handles m_tstbGameName.TextChanged, m_tbxPressureName.TextChanged, m_tbxOutcomeName.TextChanged, m_tbxTestsetName.TextChanged
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
            If (Me.m_bInupdate) Then Return
            Me.UpdateControls()
        End Sub

#End Region ' Common

#Region " Game "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Event handler, called when the user has selected a <see cref="cGame">game</see>.
        ''' </summary>
        ''' <param name="sender">Ignored.</param>
        ''' <param name="e">Ignored</param>
        ''' -----------------------------------------------------------------------
        Private Sub OnGameSelected(sender As Object, e As EventArgs) Handles m_tsddGames.SelectedIndexChanged
            Try
                Dim game As cGame = Me.SelectedGame()

                Me.m_bInupdate = True
                If (game IsNot Nothing) Then
                    Me.m_fpRunYears.Value = game.RunYears
                    Me.m_fpSpinupYears.Value = game.SpinupYears
                    Me.m_fpMAPCellClosure.Value = game.MPACellClosureRatio
                    Me.m_fpBycatchFee.Value = game.BycatchCostMultiplier
                    Me.m_tstbGameName.Text = game.Name
                    Me.m_tbxInfoVersion.Text = game.Version
                    Me.m_tbxInfoAuthor.Text = game.Author
                    Me.m_tbxInfoContact.Text = game.Contact
                    Me.m_tbxInfoDescription.Text = game.Description
                    Me.m_cbGameCalcIndicators.Checked = game.CalculateIndicators
                    Me.m_nudEmulOutcomeRange.Value = CDec(game.OutcomeRange)
                Else
                    Me.m_fpRunYears.Value = 100
                    Me.m_fpSpinupYears.Value = 10
                    Me.m_fpMAPCellClosure.Value = 0.25
                    Me.m_fpBycatchFee.Value = 10
                    Me.m_tstbGameName.Text = ""
                    Me.m_tbxInfoVersion.Text = ""
                    Me.m_tbxInfoAuthor.Text = ""
                    Me.m_tbxInfoContact.Text = ""
                    Me.m_tbxInfoDescription.Text = ""
                    Me.m_cbGameCalcIndicators.Checked = False
                End If
                Me.m_bInupdate = False

                Me.m_gridPressureMappings.Game = game
                Me.m_gridEmulTestset.Game = game
                Me.FillOutcomeListbox()

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
        ''' Event handler, called when the user wishes to add a <see cref="CGame">game</see>.
        ''' </summary>
        ''' <param name="sender">Ignored.</param>
        ''' <param name="e">Ignored</param>
        ''' -----------------------------------------------------------------------
        Private Sub OnAddGame(sender As Object, e As EventArgs) _
            Handles m_tsbnGameAdd.Click

            Try
                Dim g As New cGame(Me.Core)
                g.Name = Me.m_tstbGameName.Text
                g.Author = Me.Core.DefaultAuthor
                g.Contact = Me.Core.DefaultContact
                g.EcosimID = Me.Core.EcosimScenarios(Me.Core.ActiveEcosimScenarioIndex).DBID
                g.EcospaceID = Me.Core.EcospaceScenarios(Me.Core.ActiveEcospaceScenarioIndex).DBID

                g.AddDefaultPressures()

                Me.MSPLink.Data.Add(g)
                Me.MSPLink.OnChanged()
                Me.FillGamesCombo(g)
                Me.UpdateControls()

            Catch ex As Exception

            End Try
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Event handler, called when the user wishes to rename the selected <see cref="CGame">game</see>.
        ''' </summary>
        ''' <param name="sender">Ignored.</param>
        ''' <param name="e">Ignored</param>
        ''' -----------------------------------------------------------------------
        Private Sub OnRenameGame(sender As Object, e As EventArgs) _
            Handles m_tsbnGameEdit.Click
            Try
                Dim g As cGame = Me.SelectedGame()
                If (g IsNot Nothing) Then
                    g.Name = Me.m_tstbGameName.Text
                    Me.MSPLink.OnChanged()
                    Me.FillGamesCombo(g)
                End If
            Catch ex As Exception

            End Try
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Event handler, called when the user wishes to delete the selected <see cref="CGame">game</see>.
        ''' </summary>
        ''' <param name="sender">Ignored.</param>
        ''' <param name="e">Ignored</param>
        ''' -----------------------------------------------------------------------
        Private Sub OnDeleteGame(sender As Object, e As EventArgs) _
            Handles m_tsbnGameDelete.Click
            Try
                Dim sel As cGame = Me.SelectedGame
                If Me.PromptDelete(sel) Then
                    Me.MSPLink.Data.Remove(Me.SelectedGame())
                    Me.MSPLink.OnChanged()
                    Me.FillGamesCombo()
                    Me.UpdateControls()
                End If
            Catch ex As Exception

            End Try
        End Sub

#End Region ' Game

#Region " Game info and game settings "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Event handler, called when the user whas modified the Ecospace run
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
                    Me.MSPLink.OnChanged()
                End If
            Catch ex As Exception
                cLog.Write(ex)
            End Try
            Me.UpdateControls()
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Event handler, called when the user wishes to review Ecosim time series.
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
                    Me.MSPLink.OnChanged()
                    Me.FillGamesCombo(g)
                End If
            Catch ex As Exception

            End Try
            Me.UpdateControls()
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Event handler, called when the user imports games.
        ''' </summary>
        ''' <param name="sender">Ignored.</param>
        ''' <param name="e">Ignored</param>
        ''' -----------------------------------------------------------------------
        Private Sub OnImportGames(sender As Object, e As EventArgs) Handles m_tsbnImport.Click

            Try
                Dim cmd As cFileOpenCommand = CType(Me.CommandHandler.GetCommand(cFileOpenCommand.COMMAND_NAME), cFileOpenCommand)
                cmd.Invoke("MSP games file|*.xml", 0, "Select MSP game file to load")

                If (cmd.Result = DialogResult.OK) Then
                    Using sr As New StreamReader(cmd.FileName)
                        Me.MSPLink.Data.FromXML(sr.ReadToEnd)
                    End Using
                End If
            Catch ex As Exception
                ' Plop
            End Try
            Me.FillGamesCombo()
            Me.UpdateControls()

        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Event handler, called when the user exports games.
        ''' </summary>
        ''' <param name="sender">Ignored.</param>
        ''' <param name="e">Ignored</param>
        ''' -----------------------------------------------------------------------
        Private Sub OnExportGames(sender As Object, e As EventArgs) Handles m_tsbnExport.Click

            Try
                Dim cmd As cFileSaveCommand = CType(Me.CommandHandler.GetCommand(cFileSaveCommand.COMMAND_NAME), cFileSaveCommand)
                cmd.Invoke("MSP games file|*.xml", 0, "Select MSP game file to save")

                If (cmd.Result = DialogResult.OK) Then
                    Using sw As New StreamWriter(cmd.FileName)
                        sw.Write(Me.MSPLink.Data.ToXML())
                        sw.Flush()
                    End Using
                End If
            Catch ex As Exception

            End Try

        End Sub

#End Region ' Game info and game settings "

#Region " Pressures "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Event handler, called when the user wishes to add a <see cref="cPressure">pressure</see>.
        ''' </summary>
        ''' <param name="sender">Ignored.</param>
        ''' <param name="e">Ignored</param>
        ''' -----------------------------------------------------------------------
        Private Sub OnAddPressure(sender As Object, e As EventArgs) _
            Handles m_btnPressureAdd.Click
            Try
                Dim g As cGame = Me.SelectedGame()
                Dim p As cPressure = Nothing
                Dim n As String = Me.m_tbxPressureName.Text

                Select Case Me.m_cmbPressureTypes.SelectedIndex
                    Case 0
                        p = New cEnvironmentalPressure(n)
                    Case 1
                        p = New cFishingEffortPressure(n)
                    Case 2
                        p = New cFishingEcoPressure(n)
                    Case Else
                        Debug.Assert(False, "Whoopsie")
                End Select

                g.Add(p)

                Me.MSPLink.OnChanged()
                Me.m_gridPressureMappings.RefreshContent()
            Catch ex As Exception

            End Try
            Me.UpdateControls()
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Event handler, called when the user wishes to rename the selected 
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
                Me.MSPLink.OnChanged()
            Catch ex As Exception

            End Try
            Me.UpdateControls()
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Event handler, called when the user wishes to delete the selected 
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

                If Me.PromptDelete(pressure) Then
                    ' Reroute mappings
                    g.Driver(pressure.Name) = Nothing
                    g.Remove(pressure)

                    Me.m_gridPressureMappings.RefreshContent()
                    Me.MSPLink.OnChanged()
                End If
            Catch ex As Exception

            End Try
            Me.UpdateControls()
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Event handler, called when the user wishes to create default 
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
            Me.MSPLink.OnChanged()

            Me.UpdateControls()

        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Event handler, called when the user has selected a <see cref="cPressure">pressure</see>.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Private Sub OnPressureSelected() _
            Handles m_gridPressureMappings.OnSelectionChanged

            If (Me.m_bInupdate) Then Return
            Me.m_bInupdate = True

            Try
                Dim p As cPressure = Me.m_gridPressureMappings.SelectedPressure
                If (p IsNot Nothing) Then
                    Me.m_tbxPressureName.Text = p.Name
                    'Dim iSel As Integer = -1
                    'If TypeOf p Is cEnvironmentalPressure Then
                    '    iSel = 0
                    'ElseIf TypeOf p Is cFishingEffortPressure Then
                    '    iSel = 1
                    'ElseIf TypeOf p Is cFishingEcoPressure Then
                    '    iSel = 2
                    'End If
                    'Me.m_cmbPressureTypes.SelectedIndex = iSel
                End If
            Catch ex As Exception

            End Try
            Me.m_bInupdate = False

            Me.UpdateControls()

        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Event handler, called when the user has connected changed  a 
        ''' <see cref="cPressure">pressure</see> to <see cref="cDriver">driver</see> 
        ''' mapping.
        ''' </summary>
        ''' <param name="sender">Ignored.</param>
        ''' -----------------------------------------------------------------------
        Private Sub OnMappingsChanged(sender As gridPressureDriverMappings) _
            Handles m_gridPressureMappings.OnMappingsChanged

            Me.m_gridEmulTestset.RefreshContent()
            Me.UpdateControls()

        End Sub

#End Region ' Pressures

#Region " Outcomes "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Event handler, called when the user wishes to add a <see cref="cOutcome">output</see>.
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

                Me.FillOutcomeListbox()
                Me.MSPLink.OnChanged()
                Me.m_lbOutputs.SelectedItem = output
            Catch ex As Exception

            End Try
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Event handler, called when the user wishes to rename the selected 
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

                Me.FillOutcomeListbox()
                Me.MSPLink.OnChanged()
                Me.m_lbOutputs.SelectedItem = output
            Catch ex As Exception

            End Try
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Event handler, called when the user wishes to delete the selected 
        ''' <see cref="cOutcome">output</see>.
        ''' </summary>
        ''' <param name="sender">Ignored.</param>
        ''' <param name="e">Ignored</param>
        ''' -----------------------------------------------------------------------
        Private Sub OnDeleteOutput(sender As Object, e As EventArgs) Handles m_btnOutcomeDelete.Click
            Try

                Dim g As cGame = Me.SelectedGame()
                Dim test As IMELItem = Nothing

                If Me.m_lbOutputs.SelectedItems.Count = 1 Then
                    test = DirectCast(Me.m_lbOutputs.SelectedItem, IMELItem)
                End If

                If (Me.PromptDelete(test)) Then
                    For Each item As Object In Me.m_lbOutputs.SelectedItems
                        g.Remove(DirectCast(item, cOutcome))
                    Next
                    Me.FillOutcomeListbox()
                    Me.OnOutputSelected(Nothing, Nothing)
                    Me.MSPLink.OnChanged()
                End If

            Catch ex As Exception

            End Try
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Event handler, called when the user has selected an <see cref="cOutcome">output</see>
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

        Private Sub OnOutcomeRawSelected(sender As Object, e As EventArgs) _
            Handles m_tsbnOuputRaw.Click

            If Me.m_bInupdate Then Return

            If (Me.m_lbOutputs.SelectedItems.Count = 1) Then
                Dim out As cOutcome = CType(Me.m_lbOutputs.SelectedItem, cOutcome)
                out.IsRawData = True
                Me.OnOutputChanged(Nothing)
            End If

        End Sub

        Private Sub OnOutcomeBinnedSelected(sender As Object, e As EventArgs) _
            Handles m_tsbnOuputBinned.Click

            If Me.m_bInupdate Then Return

            If (Me.m_lbOutputs.SelectedItems.Count = 1) Then
                Dim out As cOutcome = CType(Me.m_lbOutputs.SelectedItem, cOutcome)
                out.IsRawData = False
                Me.OnOutputChanged(Nothing)
            End If

        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Event handler, called when the user has changed the configuration of an
        ''' <see cref="cOutcome">output</see>.
        ''' </summary>
        ''' <param name="sender">Ignored.</param>
        ''' -----------------------------------------------------------------------
        Private Sub OnOutputChanged(sender As gridOutcomes) _
            Handles m_gridOutcome.OnMappingsChanged

            Dim out As cOutcome = CType(Me.m_lbOutputs.SelectedItem, cOutcome)
            Me.m_bInupdate = True
            Me.m_lbOutputs.Items(Me.m_lbOutputs.SelectedIndex) = Me.m_lbOutputs.SelectedItem
            Me.m_bInupdate = False
            Me.MSPLink.OnChanged()
            Me.UpdateControls()

        End Sub

#End Region ' Outcomes

#Region " Emulator "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Event handler, called when the user wishes to add a <see cref="cTestset">test set</see>.
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
        ''' Event handler, called when the user wishes to rename the selected 
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
        ''' Event handler, called when the user wishes to delete the selected 
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
        ''' Event handler, called when the user has selected a <see cref="cTestset">test set</see>
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
        ''' Event handler, called when the user has changed the configuratio of a
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
        ''' Event handler, called when the user wishes to load a <see cref="cTestset">test set</see>
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
        ''' Event handler, called when the user has toggled the Ecospace pause option.
        ''' <see cref="cTestset">test set</see>.
        ''' </summary>
        ''' <param name="sender">Ignored.</param>
        ''' <param name="e">Ignored</param>
        ''' -----------------------------------------------------------------------
        Private Sub OnPauseEcospaceCheckChanged(sender As Object, e As EventArgs) _
            Handles m_cbEmulPauseSpace.CheckedChanged, m_cmbEmulPauseOptions.SelectedIndexChanged,
                    m_cbSaveOutputMaps.CheckedChanged

            My.Settings.PauseEcospace = Me.m_cbEmulPauseSpace.Checked
            My.Settings.PauseEcospaceInterval = Me.m_cmbEmulPauseOptions.SelectedIndex
            My.Settings.SaveOutputMaps = Me.m_cbSaveOutputMaps.Checked
            My.Settings.Save()

            Me.UpdateControls()

        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Event handler, called when the user wishes to advance the current 
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
        ''' Event handler, called when the user wishes to stop the current Ecospace 
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
        ''' Event handler, called when the user wishes to view to folder where
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
        ''' Event handler, called when the user wishes to view to folder where
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
        ''' Event handler, called when the user wishes to visit the MSP site.
        ''' </summary>
        ''' <param name="sender">Ignored.</param>
        ''' <param name="e">Ignored</param>
        ''' -----------------------------------------------------------------------
        Private Sub OnVisitMSPChallenge(sender As Object, e As EventArgs) Handles m_pbMSPChallenge.Click
            Me.Visit("https://www.mspchallenge.info/")
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Event handler, called when the user wishes to visit the RWS site.
        ''' </summary>
        ''' <param name="sender">Ignored.</param>
        ''' <param name="e">Ignored</param>
        ''' -----------------------------------------------------------------------
        Private Sub OnVisitRWS(sender As Object, e As EventArgs) Handles m_pbRWS.Click
            Me.Visit("https://www.rws.nl/")
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Event handler, called when the user wishes to visit the EII site.
        ''' </summary>
        ''' <param name="sender">Ignored.</param>
        ''' <param name="e">Ignored</param>
        ''' -----------------------------------------------------------------------
        Private Sub OnVisitEII(sender As Object, e As EventArgs) Handles m_pbEII.Click
            Me.Visit("https://www.ecopathinternational.org")
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Event handler, called when the user wishes to visit the BUAS site.
        ''' </summary>
        ''' <param name="sender">Ignored.</param>
        ''' <param name="e">Ignored</param>
        ''' -----------------------------------------------------------------------
        Private Sub OnVisitBUAS(sender As Object, e As EventArgs) Handles m_pbBUAS.Click
            Me.Visit("https://www.buas.nl")
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Event handler, called when the user wishes to visit the Ecoscope site.
        ''' </summary>
        ''' <param name="sender">Ignored.</param>
        ''' <param name="e">Ignored</param>
        ''' -----------------------------------------------------------------------
        Private Sub OnVisitEcoscope(sender As Object, e As EventArgs) Handles m_pbEcoscope.Click
            Me.Visit("https://ecoscopium.eu/")
        End Sub

#End Region ' Credits

#End Region ' Control events

#Region " Internals "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the <see cref="cEwEMSPLink">EwE MSP shell</see> to operate onto.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Friend Property MSPLink As cEwEMSPLink = Nothing

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns the currently selected <see cref="CTestset">test set</see>.
        ''' </summary>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Private Function SelectedTestset() As cTestset
            Return DirectCast(Me.m_cmbEmulTestsets.SelectedItem, cTestset)
        End Function

        Private Sub FillGamesCombo(Optional sel As cGame = Nothing)

            Me.m_tsddGames.Items.Clear()
            For Each cfg As cGame In Me.MSPLink.Data.Games
                Me.m_tsddGames.Items.Add(cfg)
            Next
            If sel IsNot Nothing Then
                Me.m_tsddGames.SelectedItem = sel
            ElseIf (Me.m_tsddGames.Items.Count > 0) Then
                Me.m_tsddGames.SelectedIndex = 0
            End If

            Me.OnGameSelected(Me, Nothing)
        End Sub

        Private Sub FillPressureTypesCombo()
            Me.m_cmbPressureTypes.Items.Clear()
            Me.m_cmbPressureTypes.Items.Add(My.Resources.CHOICE_GRID)
            Me.m_cmbPressureTypes.Items.Add(My.Resources.CHOICE_FISHING_EFFORT)
            Me.m_cmbPressureTypes.Items.Add(My.Resources.CHOICE_FISHING_ECO)
        End Sub

        Private Sub FillOutputTypesCombo()

            Me.m_cmbOutputTypes.Items.Clear()
            For Each t As cOutcome.eLayerType In [Enum].GetValues(GetType(cOutcome.eLayerType))
                Me.m_cmbOutputTypes.Items.Add(t)
            Next
            Me.m_cmbOutputTypes.SelectedIndex = 0

        End Sub

        Private Sub FillOutcomeListbox()

            If (Me.m_bInupdate) Then Return

            Me.m_bInupdate = True
            Try
                Dim g As cGame = Me.SelectedGame()
                Me.m_lbOutputs.Items.Clear()
                If (g IsNot Nothing) Then
                    For Each out As cOutcome In g.Outcomes
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

                            ' XXX REWRITE THIS XXX
                            'Select Case p.DataType
                            '    Case cPressure.eDataTypes.Grid
                            '        Dim psim As New cPressure(p.Name, bm.InCol, bm.InRow)
                            '        If psim.Grid.Load(data, Me.UIContext.Core) Then
                            '            pressures.Add(psim)
                            '        Else
                            '            msg.Message = cStringUtils.Localize(My.Resources.STATUS_TESTSET_LOAD_FAILED, testset.Name)
                            '            vs = New cVariableStatus(eStatusFlags.ErrorEncountered, cStringUtils.Localize(My.Resources.STATUS_TESTDATA_MAP_REJECTED, p.Name, data),
                            '                                     eVarNameFlags.NotSet, eDataTypes.NotSet, eCoreComponentType.Ecospace, 0)
                            '            msg.AddVariable(vs)
                            '        End If
                            '    Case cPressure.eDataTypes.Scalar
                            '        If Double.TryParse(data, p.Scalar) Then
                            '            pressures.Add(p)
                            '        End If
                            '    Case Else
                            '        Debug.Assert(False)
                            'End Select
                        End If

                    Next
                End If

                ' Pass pressures on
                g.ApplyPressures(pressures.ToArray(), False)

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
            If (g Is Nothing) Then Return

            Dim outcomes As New List(Of cGrid)
            Dim bm As cEcospaceBasemap = Me.UIContext.Core.EcospaceBasemap
            Dim parms As cEcospaceModelParameters = Me.UIContext.Core.EcospaceModelParameters

            For Each o As cOutcome In g.Outcomes
                Dim grid As New cGrid(o.Name, bm.InCol, bm.InRow)
                grid.IsValid = False
                outcomes.Add(grid)
            Next
            g.LoadOutcomes(outcomes.ToArray(), data)

            ' Save outputs
            If (Me.m_cbSaveOutputMaps.Checked) Then
                ' Prep msg
                Dim msg As New cMessage("MSP outcomes saved to disk for Ecospace timestep " & data.iTimeStep, eMessageType.DataExport, eCoreComponentType.Ecospace, eMessageImportance.Information)

                For Each grid As cGrid In outcomes

                    If (grid.IsValid) Then

                        Dim strFile As String = cFileUtils.ToValidFileName("outcome_" & grid.Name & "_" & data.iTimeStep.ToString("D4") & ".asc", False)
                        Dim vs As cVariableStatus = Nothing

                        Try
                            If (grid.Save(Path.Combine(Me.OutputPath, strFile), Me.Core)) Then
                                vs = New cVariableStatus(eStatusFlags.OK, g.Name, eVarNameFlags.NotSet, eDataTypes.NotSet, eCoreComponentType.Ecospace, 0)
                                msg.Hyperlink = Me.OutputPath
                            End If
                        Catch ex As Exception
                            vs = New cVariableStatus(eStatusFlags.ErrorEncountered, ex.Message, eVarNameFlags.NotSet, eDataTypes.NotSet, eCoreComponentType.Ecospace, 0)
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
                    Case 1 : bPause = (data.iTimeStep Mod Math.Round(parms.NumberOfTimeStepsPerYear)) = 0
                    Case 2 : bPause = (data.iTimeStep Mod Math.Round(5 * parms.NumberOfTimeStepsPerYear)) = 0
                End Select
                If (bPause = True) Then
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

        Private Sub OnExportGameData(sender As Object, e As EventArgs)

            Dim ds As IEwEDataSource = Me.Core.DataSource
            Dim file As String = Path.Combine(ds.Directory, ds.FileName) & "_MSPgames.xml"
            Dim msg As cMessage = Nothing

            If Me.MSPLink.SaveConfiguration(file) Then
                msg = New cMessage(cStringUtils.Localize(My.Resources.STATUS_GAME_EXPORT_SUCCESS, file), eMessageType.DataExport, eCoreComponentType.External, eMessageImportance.Information)
                msg.Hyperlink = Path.GetDirectoryName(file)
            Else
                msg = New cMessage(cStringUtils.Localize(My.Resources.STATUS_GAME_EXPORT_FAILED, file), eMessageType.DataExport, eCoreComponentType.External, eMessageImportance.Critical)
            End If

            Me.Core.Messages.SendMessage(msg)

        End Sub

        Private Function PromptDelete(data As IMELItem) As Boolean

            Dim msg As String = ""

            If (data IsNot Nothing) Then
                Dim datatype As String = ""
                If (TypeOf (data) Is cGame) Then
                    datatype = My.Resources.TYPE_GAME
                ElseIf (TypeOf (data) Is cPressure) Then
                    datatype = My.Resources.TYPE_PRESSURE
                ElseIf (TypeOf (data) Is cOutcome) Then
                    datatype = My.Resources.TYPE_OUTCOME
                End If
                msg = cStringUtils.Localize(My.Resources.PROMPT_DELETE_SINGLE, datatype, data.Name)
            Else
                msg = My.Resources.PROMPT_DELETE_MULTIPLE
            End If

            Dim fmsg As New cFeedbackMessage(msg, eCoreComponentType.External, eMessageType.DataModified, eMessageImportance.Question, eMessageReplyStyle.YES_NO)
            Me.Core.Messages.SendMessage(fmsg)

            Return fmsg.Reply = eMessageReply.YES

        End Function

#End Region ' Internals

    End Class

End Namespace
