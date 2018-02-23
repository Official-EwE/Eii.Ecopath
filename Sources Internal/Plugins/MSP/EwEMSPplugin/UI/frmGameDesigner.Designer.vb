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

Imports ScientificInterfaceShared.Controls
Imports SharedResources = ScientificInterfaceShared.My.Resources

Namespace UI

    Partial Class frmGameDesigner
        Inherits ScientificInterfaceShared.Forms.frmEwE

        'Form overrides dispose to clean up the component list.
        <System.Diagnostics.DebuggerNonUserCode()>
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            Try
                If disposing AndAlso components IsNot Nothing Then
                    components.Dispose()
                End If
            Finally
                MyBase.Dispose(disposing)
            End Try
        End Sub

        'Required by the Windows Form Designer
        Private components As System.ComponentModel.IContainer

        'NOTE: The following procedure is required by the Windows Form Designer
        'It can be modified using the Windows Form Designer.  
        'Do not modify it using the code editor.
        <System.Diagnostics.DebuggerStepThrough()>
        Private Sub InitializeComponent()
            Me.components = New System.ComponentModel.Container()
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmGameDesigner))
            Me.m_tabConfig = New System.Windows.Forms.TabControl()
            Me.m_tpEwESettings = New System.Windows.Forms.TabPage()
            Me.m_btnSettingsUseCurrentScenario = New System.Windows.Forms.Button()
            Me.m_hdrValidation = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
            Me.m_cbGameCalcIndicators = New System.Windows.Forms.CheckBox()
            Me.m_lblMPACellClosure2 = New System.Windows.Forms.Label()
            Me.m_lblMPACellClosure1 = New System.Windows.Forms.Label()
            Me.m_tbxMPACellClosure = New System.Windows.Forms.TextBox()
            Me.m_tbxRunYears = New System.Windows.Forms.TextBox()
            Me.m_tbxSpinupYears = New System.Windows.Forms.TextBox()
            Me.m_lblRunYears = New System.Windows.Forms.Label()
            Me.m_lblSpinupYears = New System.Windows.Forms.Label()
            Me.m_lblCheckSimFishing = New EwEMSPPlugin.UI.cImageLabel()
            Me.m_lblCheckSimForcing = New EwEMSPPlugin.UI.cImageLabel()
            Me.m_lblCheckGame = New EwEMSPPlugin.UI.cImageLabel()
            Me.m_lblCheckSpaceTimeSeries = New EwEMSPPlugin.UI.cImageLabel()
            Me.m_lblCheckSimTimeSeries = New EwEMSPPlugin.UI.cImageLabel()
            Me.m_tpInformation = New System.Windows.Forms.TabPage()
            Me.m_tbxInfoDescription = New System.Windows.Forms.TextBox()
            Me.m_tbxInfoContact = New System.Windows.Forms.TextBox()
            Me.m_tbxInfoAuthor = New System.Windows.Forms.TextBox()
            Me.m_tbxInfoVersion = New System.Windows.Forms.TextBox()
            Me.m_lblInfoDescription = New System.Windows.Forms.Label()
            Me.m_lblInfoContact = New System.Windows.Forms.Label()
            Me.m_lblInfoAuthor = New System.Windows.Forms.Label()
            Me.m_lblInfoVersion = New System.Windows.Forms.Label()
            Me.m_tpPressures = New System.Windows.Forms.TabPage()
            Me.m_cmbPressureTypes = New System.Windows.Forms.ComboBox()
            Me.m_gridPressureMappings = New EwEMSPPlugin.UI.gridDrivers()
            Me.m_btnPressureDefaults = New System.Windows.Forms.Button()
            Me.m_btnPressureDelete = New System.Windows.Forms.Button()
            Me.m_btnPressureRename = New System.Windows.Forms.Button()
            Me.m_btnPressureAdd = New System.Windows.Forms.Button()
            Me.m_tbxPressureName = New System.Windows.Forms.TextBox()
            Me.m_lblPressuresPressure = New System.Windows.Forms.Label()
            Me.m_tpOutcomes = New System.Windows.Forms.TabPage()
            Me.m_scOutputs = New System.Windows.Forms.SplitContainer()
            Me.m_lbOutputs = New System.Windows.Forms.ListBox()
            Me.m_gridOutcome = New EwEMSPPlugin.UI.gridOutcomes()
            Me.m_tsOutcome = New ScientificInterfaceShared.Controls.cEwEToolstrip()
            Me.m_cmbOutputTypes = New System.Windows.Forms.ComboBox()
            Me.m_btnOutcomeDelete = New System.Windows.Forms.Button()
            Me.m_btnOutcomeRename = New System.Windows.Forms.Button()
            Me.m_btnOutcomeAdd = New System.Windows.Forms.Button()
            Me.m_tbxOutcomeName = New System.Windows.Forms.TextBox()
            Me.m_lblOutcome = New System.Windows.Forms.Label()
            Me.m_tpEmulator = New System.Windows.Forms.TabPage()
            Me.m_nudEmulOutcomeRange = New System.Windows.Forms.NumericUpDown()
            Me.m_btnTestsetDelete = New System.Windows.Forms.Button()
            Me.m_btnTestsetRename = New System.Windows.Forms.Button()
            Me.m_lblEmulatorTestset = New System.Windows.Forms.Label()
            Me.m_btnTestsetAdd = New System.Windows.Forms.Button()
            Me.m_tbxTestsetName = New System.Windows.Forms.TextBox()
            Me.m_btnTestsetApply = New System.Windows.Forms.Button()
            Me.m_btnEmulStep = New System.Windows.Forms.Button()
            Me.m_hdrEmulRun = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
            Me.m_cmbEmulPauseOptions = New System.Windows.Forms.ComboBox()
            Me.m_btnEmulViewOutputFolder = New System.Windows.Forms.Button()
            Me.m_btnEmulStop = New System.Windows.Forms.Button()
            Me.m_lblEmulTestSets = New System.Windows.Forms.Label()
            Me.m_gridEmulTestset = New EwEMSPPlugin.UI.gridEmulator()
            Me.m_cmbEmulTestsets = New System.Windows.Forms.ComboBox()
            Me.m_cbSaveOutputMaps = New System.Windows.Forms.CheckBox()
            Me.m_cbEmulPauseSpace = New System.Windows.Forms.CheckBox()
            Me.m_tpAbout = New System.Windows.Forms.TabPage()
            Me.m_tlpAbout = New System.Windows.Forms.TableLayoutPanel()
            Me.m_lblAboutDescription = New System.Windows.Forms.Label()
            Me.m_tlpLogos = New System.Windows.Forms.TableLayoutPanel()
            Me.m_pbEII = New System.Windows.Forms.PictureBox()
            Me.m_pbNHTV = New System.Windows.Forms.PictureBox()
            Me.m_pbRWS = New System.Windows.Forms.PictureBox()
            Me.m_pbMSPC2050 = New System.Windows.Forms.PictureBox()
            Me.m_lblAboutCredits = New System.Windows.Forms.Label()
            Me.m_lblAboutVersion = New System.Windows.Forms.Label()
            Me.m_hdrCredits = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
            Me.m_ilTabIcons = New System.Windows.Forms.ImageList(Me.components)
            Me.m_cmbGames = New System.Windows.Forms.ComboBox()
            Me.m_lblGames = New System.Windows.Forms.Label()
            Me.m_btnGameAdd = New System.Windows.Forms.Button()
            Me.m_lblGame = New System.Windows.Forms.Label()
            Me.m_tbxGameName = New System.Windows.Forms.TextBox()
            Me.m_btnGameRename = New System.Windows.Forms.Button()
            Me.m_btnGameDelete = New System.Windows.Forms.Button()
            Me.m_btnGameAddFromJSON = New System.Windows.Forms.Button()
            Me.m_btnExport = New System.Windows.Forms.Button()
            Me.m_tabConfig.SuspendLayout()
            Me.m_tpEwESettings.SuspendLayout()
            Me.m_tpInformation.SuspendLayout()
            Me.m_tpPressures.SuspendLayout()
            Me.m_tpOutcomes.SuspendLayout()
            CType(Me.m_scOutputs, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.m_scOutputs.Panel1.SuspendLayout()
            Me.m_scOutputs.Panel2.SuspendLayout()
            Me.m_scOutputs.SuspendLayout()
            Me.m_tpEmulator.SuspendLayout()
            CType(Me.m_nudEmulOutcomeRange, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.m_tpAbout.SuspendLayout()
            Me.m_tlpAbout.SuspendLayout()
            Me.m_tlpLogos.SuspendLayout()
            CType(Me.m_pbEII, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.m_pbNHTV, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.m_pbRWS, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.m_pbMSPC2050, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            'm_tabConfig
            '
            resources.ApplyResources(Me.m_tabConfig, "m_tabConfig")
            Me.m_tabConfig.Controls.Add(Me.m_tpEwESettings)
            Me.m_tabConfig.Controls.Add(Me.m_tpInformation)
            Me.m_tabConfig.Controls.Add(Me.m_tpPressures)
            Me.m_tabConfig.Controls.Add(Me.m_tpOutcomes)
            Me.m_tabConfig.Controls.Add(Me.m_tpEmulator)
            Me.m_tabConfig.Controls.Add(Me.m_tpAbout)
            Me.m_tabConfig.ImageList = Me.m_ilTabIcons
            Me.m_tabConfig.Name = "m_tabConfig"
            Me.m_tabConfig.SelectedIndex = 0
            '
            'm_tpEwESettings
            '
            Me.m_tpEwESettings.BackColor = System.Drawing.Color.White
            Me.m_tpEwESettings.Controls.Add(Me.m_btnSettingsUseCurrentScenario)
            Me.m_tpEwESettings.Controls.Add(Me.m_hdrValidation)
            Me.m_tpEwESettings.Controls.Add(Me.m_cbGameCalcIndicators)
            Me.m_tpEwESettings.Controls.Add(Me.m_lblMPACellClosure2)
            Me.m_tpEwESettings.Controls.Add(Me.m_lblMPACellClosure1)
            Me.m_tpEwESettings.Controls.Add(Me.m_tbxMPACellClosure)
            Me.m_tpEwESettings.Controls.Add(Me.m_tbxRunYears)
            Me.m_tpEwESettings.Controls.Add(Me.m_tbxSpinupYears)
            Me.m_tpEwESettings.Controls.Add(Me.m_lblRunYears)
            Me.m_tpEwESettings.Controls.Add(Me.m_lblSpinupYears)
            Me.m_tpEwESettings.Controls.Add(Me.m_lblCheckSimFishing)
            Me.m_tpEwESettings.Controls.Add(Me.m_lblCheckSimForcing)
            Me.m_tpEwESettings.Controls.Add(Me.m_lblCheckGame)
            Me.m_tpEwESettings.Controls.Add(Me.m_lblCheckSpaceTimeSeries)
            Me.m_tpEwESettings.Controls.Add(Me.m_lblCheckSimTimeSeries)
            resources.ApplyResources(Me.m_tpEwESettings, "m_tpEwESettings")
            Me.m_tpEwESettings.Name = "m_tpEwESettings"
            '
            'm_btnSettingsUseCurrentScenario
            '
            resources.ApplyResources(Me.m_btnSettingsUseCurrentScenario, "m_btnSettingsUseCurrentScenario")
            Me.m_btnSettingsUseCurrentScenario.Name = "m_btnSettingsUseCurrentScenario"
            Me.m_btnSettingsUseCurrentScenario.UseVisualStyleBackColor = True
            '
            'm_hdrValidation
            '
            resources.ApplyResources(Me.m_hdrValidation, "m_hdrValidation")
            Me.m_hdrValidation.CanCollapseParent = False
            Me.m_hdrValidation.CollapsedParentHeight = 0
            Me.m_hdrValidation.IsCollapsed = False
            Me.m_hdrValidation.Name = "m_hdrValidation"
            '
            'm_cbGameCalcIndicators
            '
            resources.ApplyResources(Me.m_cbGameCalcIndicators, "m_cbGameCalcIndicators")
            Me.m_cbGameCalcIndicators.Name = "m_cbGameCalcIndicators"
            Me.m_cbGameCalcIndicators.UseVisualStyleBackColor = True
            '
            'm_lblMPACellClosure2
            '
            resources.ApplyResources(Me.m_lblMPACellClosure2, "m_lblMPACellClosure2")
            Me.m_lblMPACellClosure2.Name = "m_lblMPACellClosure2"
            '
            'm_lblMPACellClosure1
            '
            resources.ApplyResources(Me.m_lblMPACellClosure1, "m_lblMPACellClosure1")
            Me.m_lblMPACellClosure1.Name = "m_lblMPACellClosure1"
            '
            'm_tbxMPACellClosure
            '
            resources.ApplyResources(Me.m_tbxMPACellClosure, "m_tbxMPACellClosure")
            Me.m_tbxMPACellClosure.Name = "m_tbxMPACellClosure"
            '
            'm_tbxRunYears
            '
            resources.ApplyResources(Me.m_tbxRunYears, "m_tbxRunYears")
            Me.m_tbxRunYears.Name = "m_tbxRunYears"
            '
            'm_tbxSpinupYears
            '
            resources.ApplyResources(Me.m_tbxSpinupYears, "m_tbxSpinupYears")
            Me.m_tbxSpinupYears.Name = "m_tbxSpinupYears"
            '
            'm_lblRunYears
            '
            resources.ApplyResources(Me.m_lblRunYears, "m_lblRunYears")
            Me.m_lblRunYears.Name = "m_lblRunYears"
            '
            'm_lblSpinupYears
            '
            resources.ApplyResources(Me.m_lblSpinupYears, "m_lblSpinupYears")
            Me.m_lblSpinupYears.Name = "m_lblSpinupYears"
            '
            'm_lblCheckSimFishing
            '
            resources.ApplyResources(Me.m_lblCheckSimFishing, "m_lblCheckSimFishing")
            Me.m_lblCheckSimFishing.Name = "m_lblCheckSimFishing"
            '
            'm_lblCheckSimForcing
            '
            resources.ApplyResources(Me.m_lblCheckSimForcing, "m_lblCheckSimForcing")
            Me.m_lblCheckSimForcing.Name = "m_lblCheckSimForcing"
            '
            'm_lblCheckGame
            '
            resources.ApplyResources(Me.m_lblCheckGame, "m_lblCheckGame")
            Me.m_lblCheckGame.Name = "m_lblCheckGame"
            '
            'm_lblCheckSpaceTimeSeries
            '
            resources.ApplyResources(Me.m_lblCheckSpaceTimeSeries, "m_lblCheckSpaceTimeSeries")
            Me.m_lblCheckSpaceTimeSeries.Name = "m_lblCheckSpaceTimeSeries"
            '
            'm_lblCheckSimTimeSeries
            '
            resources.ApplyResources(Me.m_lblCheckSimTimeSeries, "m_lblCheckSimTimeSeries")
            Me.m_lblCheckSimTimeSeries.Name = "m_lblCheckSimTimeSeries"
            '
            'm_tpInformation
            '
            Me.m_tpInformation.Controls.Add(Me.m_tbxInfoDescription)
            Me.m_tpInformation.Controls.Add(Me.m_tbxInfoContact)
            Me.m_tpInformation.Controls.Add(Me.m_tbxInfoAuthor)
            Me.m_tpInformation.Controls.Add(Me.m_tbxInfoVersion)
            Me.m_tpInformation.Controls.Add(Me.m_lblInfoDescription)
            Me.m_tpInformation.Controls.Add(Me.m_lblInfoContact)
            Me.m_tpInformation.Controls.Add(Me.m_lblInfoAuthor)
            Me.m_tpInformation.Controls.Add(Me.m_lblInfoVersion)
            resources.ApplyResources(Me.m_tpInformation, "m_tpInformation")
            Me.m_tpInformation.Name = "m_tpInformation"
            Me.m_tpInformation.UseVisualStyleBackColor = True
            '
            'm_tbxInfoDescription
            '
            resources.ApplyResources(Me.m_tbxInfoDescription, "m_tbxInfoDescription")
            Me.m_tbxInfoDescription.Name = "m_tbxInfoDescription"
            '
            'm_tbxInfoContact
            '
            resources.ApplyResources(Me.m_tbxInfoContact, "m_tbxInfoContact")
            Me.m_tbxInfoContact.Name = "m_tbxInfoContact"
            '
            'm_tbxInfoAuthor
            '
            resources.ApplyResources(Me.m_tbxInfoAuthor, "m_tbxInfoAuthor")
            Me.m_tbxInfoAuthor.Name = "m_tbxInfoAuthor"
            '
            'm_tbxInfoVersion
            '
            resources.ApplyResources(Me.m_tbxInfoVersion, "m_tbxInfoVersion")
            Me.m_tbxInfoVersion.Name = "m_tbxInfoVersion"
            '
            'm_lblInfoDescription
            '
            resources.ApplyResources(Me.m_lblInfoDescription, "m_lblInfoDescription")
            Me.m_lblInfoDescription.Name = "m_lblInfoDescription"
            '
            'm_lblInfoContact
            '
            resources.ApplyResources(Me.m_lblInfoContact, "m_lblInfoContact")
            Me.m_lblInfoContact.Name = "m_lblInfoContact"
            '
            'm_lblInfoAuthor
            '
            resources.ApplyResources(Me.m_lblInfoAuthor, "m_lblInfoAuthor")
            Me.m_lblInfoAuthor.Name = "m_lblInfoAuthor"
            '
            'm_lblInfoVersion
            '
            resources.ApplyResources(Me.m_lblInfoVersion, "m_lblInfoVersion")
            Me.m_lblInfoVersion.Name = "m_lblInfoVersion"
            '
            'm_tpPressures
            '
            Me.m_tpPressures.Controls.Add(Me.m_cmbPressureTypes)
            Me.m_tpPressures.Controls.Add(Me.m_gridPressureMappings)
            Me.m_tpPressures.Controls.Add(Me.m_btnPressureDefaults)
            Me.m_tpPressures.Controls.Add(Me.m_btnPressureDelete)
            Me.m_tpPressures.Controls.Add(Me.m_btnPressureRename)
            Me.m_tpPressures.Controls.Add(Me.m_btnPressureAdd)
            Me.m_tpPressures.Controls.Add(Me.m_tbxPressureName)
            Me.m_tpPressures.Controls.Add(Me.m_lblPressuresPressure)
            resources.ApplyResources(Me.m_tpPressures, "m_tpPressures")
            Me.m_tpPressures.Name = "m_tpPressures"
            Me.m_tpPressures.UseVisualStyleBackColor = True
            '
            'm_cmbPressureTypes
            '
            resources.ApplyResources(Me.m_cmbPressureTypes, "m_cmbPressureTypes")
            Me.m_cmbPressureTypes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.m_cmbPressureTypes.FormattingEnabled = True
            Me.m_cmbPressureTypes.Name = "m_cmbPressureTypes"
            Me.m_cmbPressureTypes.Sorted = True
            '
            'm_gridPressureMappings
            '
            Me.m_gridPressureMappings.AllowBlockSelect = False
            resources.ApplyResources(Me.m_gridPressureMappings, "m_gridPressureMappings")
            Me.m_gridPressureMappings.AutoSizeMinHeight = 10
            Me.m_gridPressureMappings.AutoSizeMinWidth = 10
            Me.m_gridPressureMappings.AutoStretchColumnsToFitWidth = True
            Me.m_gridPressureMappings.AutoStretchRowsToFitHeight = False
            Me.m_gridPressureMappings.BackColor = System.Drawing.Color.White
            Me.m_gridPressureMappings.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.m_gridPressureMappings.ContextMenuStyle = CType((((SourceGrid2.ContextMenuStyle.ColumnResize Or SourceGrid2.ContextMenuStyle.AutoSize) _
            Or SourceGrid2.ContextMenuStyle.CopyPasteSelection) _
            Or SourceGrid2.ContextMenuStyle.CellContextMenu), SourceGrid2.ContextMenuStyle)
            Me.m_gridPressureMappings.CustomSort = False
            Me.m_gridPressureMappings.DataName = "MEL pressure mappings"
            Me.m_gridPressureMappings.FixedColumnWidths = False
            Me.m_gridPressureMappings.FocusStyle = SourceGrid2.FocusStyle.None
            Me.m_gridPressureMappings.Game = Nothing
            Me.m_gridPressureMappings.GridToolTipActive = True
            Me.m_gridPressureMappings.IsLayoutSuspended = False
            Me.m_gridPressureMappings.IsOutputGrid = False
            Me.m_gridPressureMappings.Name = "m_gridPressureMappings"
            Me.m_gridPressureMappings.Shell = Nothing
            Me.m_gridPressureMappings.SpecialKeys = CType((((((((((SourceGrid2.GridSpecialKeys.Ctrl_C Or SourceGrid2.GridSpecialKeys.Ctrl_V) _
            Or SourceGrid2.GridSpecialKeys.Ctrl_X) _
            Or SourceGrid2.GridSpecialKeys.Delete) _
            Or SourceGrid2.GridSpecialKeys.Arrows) _
            Or SourceGrid2.GridSpecialKeys.Tab) _
            Or SourceGrid2.GridSpecialKeys.PageDownUp) _
            Or SourceGrid2.GridSpecialKeys.Enter) _
            Or SourceGrid2.GridSpecialKeys.Escape) _
            Or SourceGrid2.GridSpecialKeys.Backspace), SourceGrid2.GridSpecialKeys)
            Me.m_gridPressureMappings.UIContext = Nothing
            '
            'm_btnPressureDefaults
            '
            resources.ApplyResources(Me.m_btnPressureDefaults, "m_btnPressureDefaults")
            Me.m_btnPressureDefaults.Image = Global.EwEMSPPlugin.My.Resources.Resources.defaults
            Me.m_btnPressureDefaults.Name = "m_btnPressureDefaults"
            Me.m_btnPressureDefaults.UseVisualStyleBackColor = True
            '
            'm_btnPressureDelete
            '
            resources.ApplyResources(Me.m_btnPressureDelete, "m_btnPressureDelete")
            Me.m_btnPressureDelete.Image = Global.EwEMSPPlugin.My.Resources.Resources.delete
            Me.m_btnPressureDelete.Name = "m_btnPressureDelete"
            Me.m_btnPressureDelete.UseVisualStyleBackColor = True
            '
            'm_btnPressureRename
            '
            resources.ApplyResources(Me.m_btnPressureRename, "m_btnPressureRename")
            Me.m_btnPressureRename.Image = Global.EwEMSPPlugin.My.Resources.Resources.change
            Me.m_btnPressureRename.Name = "m_btnPressureRename"
            Me.m_btnPressureRename.UseVisualStyleBackColor = True
            '
            'm_btnPressureAdd
            '
            resources.ApplyResources(Me.m_btnPressureAdd, "m_btnPressureAdd")
            Me.m_btnPressureAdd.Image = Global.EwEMSPPlugin.My.Resources.Resources.add
            Me.m_btnPressureAdd.Name = "m_btnPressureAdd"
            Me.m_btnPressureAdd.UseVisualStyleBackColor = True
            '
            'm_tbxPressureName
            '
            resources.ApplyResources(Me.m_tbxPressureName, "m_tbxPressureName")
            Me.m_tbxPressureName.Name = "m_tbxPressureName"
            '
            'm_lblPressuresPressure
            '
            resources.ApplyResources(Me.m_lblPressuresPressure, "m_lblPressuresPressure")
            Me.m_lblPressuresPressure.Name = "m_lblPressuresPressure"
            '
            'm_tpOutcomes
            '
            Me.m_tpOutcomes.Controls.Add(Me.m_scOutputs)
            Me.m_tpOutcomes.Controls.Add(Me.m_cmbOutputTypes)
            Me.m_tpOutcomes.Controls.Add(Me.m_btnOutcomeDelete)
            Me.m_tpOutcomes.Controls.Add(Me.m_btnOutcomeRename)
            Me.m_tpOutcomes.Controls.Add(Me.m_btnOutcomeAdd)
            Me.m_tpOutcomes.Controls.Add(Me.m_tbxOutcomeName)
            Me.m_tpOutcomes.Controls.Add(Me.m_lblOutcome)
            resources.ApplyResources(Me.m_tpOutcomes, "m_tpOutcomes")
            Me.m_tpOutcomes.Name = "m_tpOutcomes"
            Me.m_tpOutcomes.UseVisualStyleBackColor = True
            '
            'm_scOutputs
            '
            resources.ApplyResources(Me.m_scOutputs, "m_scOutputs")
            Me.m_scOutputs.Name = "m_scOutputs"
            '
            'm_scOutputs.Panel1
            '
            Me.m_scOutputs.Panel1.Controls.Add(Me.m_lbOutputs)
            '
            'm_scOutputs.Panel2
            '
            Me.m_scOutputs.Panel2.Controls.Add(Me.m_gridOutcome)
            Me.m_scOutputs.Panel2.Controls.Add(Me.m_tsOutcome)
            '
            'm_lbOutputs
            '
            resources.ApplyResources(Me.m_lbOutputs, "m_lbOutputs")
            Me.m_lbOutputs.FormattingEnabled = True
            Me.m_lbOutputs.Name = "m_lbOutputs"
            Me.m_lbOutputs.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended
            Me.m_lbOutputs.Sorted = True
            '
            'm_gridOutcome
            '
            Me.m_gridOutcome.AllowBlockSelect = False
            Me.m_gridOutcome.AutoSizeMinHeight = 10
            Me.m_gridOutcome.AutoSizeMinWidth = 10
            Me.m_gridOutcome.AutoStretchColumnsToFitWidth = True
            Me.m_gridOutcome.AutoStretchRowsToFitHeight = False
            Me.m_gridOutcome.BackColor = System.Drawing.Color.White
            Me.m_gridOutcome.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.m_gridOutcome.ContextMenuStyle = CType((((SourceGrid2.ContextMenuStyle.ColumnResize Or SourceGrid2.ContextMenuStyle.AutoSize) _
            Or SourceGrid2.ContextMenuStyle.CopyPasteSelection) _
            Or SourceGrid2.ContextMenuStyle.CellContextMenu), SourceGrid2.ContextMenuStyle)
            Me.m_gridOutcome.CustomSort = False
            Me.m_gridOutcome.DataName = "grid outcomes"
            resources.ApplyResources(Me.m_gridOutcome, "m_gridOutcome")
            Me.m_gridOutcome.FixedColumnWidths = False
            Me.m_gridOutcome.FocusStyle = SourceGrid2.FocusStyle.None
            Me.m_gridOutcome.GridToolTipActive = True
            Me.m_gridOutcome.IsLayoutSuspended = False
            Me.m_gridOutcome.IsOutputGrid = False
            Me.m_gridOutcome.Name = "m_gridOutcome"
            Me.m_gridOutcome.Output = Nothing
            Me.m_gridOutcome.Shell = Nothing
            Me.m_gridOutcome.SpecialKeys = CType((((((((((SourceGrid2.GridSpecialKeys.Ctrl_C Or SourceGrid2.GridSpecialKeys.Ctrl_V) _
            Or SourceGrid2.GridSpecialKeys.Ctrl_X) _
            Or SourceGrid2.GridSpecialKeys.Delete) _
            Or SourceGrid2.GridSpecialKeys.Arrows) _
            Or SourceGrid2.GridSpecialKeys.Tab) _
            Or SourceGrid2.GridSpecialKeys.PageDownUp) _
            Or SourceGrid2.GridSpecialKeys.Enter) _
            Or SourceGrid2.GridSpecialKeys.Escape) _
            Or SourceGrid2.GridSpecialKeys.Backspace), SourceGrid2.GridSpecialKeys)
            Me.m_gridOutcome.UIContext = Nothing
            '
            'm_tsOutcome
            '
            Me.m_tsOutcome.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
            resources.ApplyResources(Me.m_tsOutcome, "m_tsOutcome")
            Me.m_tsOutcome.Name = "m_tsOutcome"
            '
            'm_cmbOutputTypes
            '
            resources.ApplyResources(Me.m_cmbOutputTypes, "m_cmbOutputTypes")
            Me.m_cmbOutputTypes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.m_cmbOutputTypes.FormattingEnabled = True
            Me.m_cmbOutputTypes.Name = "m_cmbOutputTypes"
            Me.m_cmbOutputTypes.Sorted = True
            '
            'm_btnOutcomeDelete
            '
            resources.ApplyResources(Me.m_btnOutcomeDelete, "m_btnOutcomeDelete")
            Me.m_btnOutcomeDelete.Image = Global.EwEMSPPlugin.My.Resources.Resources.delete
            Me.m_btnOutcomeDelete.Name = "m_btnOutcomeDelete"
            Me.m_btnOutcomeDelete.UseVisualStyleBackColor = True
            '
            'm_btnOutcomeRename
            '
            resources.ApplyResources(Me.m_btnOutcomeRename, "m_btnOutcomeRename")
            Me.m_btnOutcomeRename.Image = Global.EwEMSPPlugin.My.Resources.Resources.change
            Me.m_btnOutcomeRename.Name = "m_btnOutcomeRename"
            Me.m_btnOutcomeRename.UseVisualStyleBackColor = True
            '
            'm_btnOutcomeAdd
            '
            resources.ApplyResources(Me.m_btnOutcomeAdd, "m_btnOutcomeAdd")
            Me.m_btnOutcomeAdd.Image = Global.EwEMSPPlugin.My.Resources.Resources.add
            Me.m_btnOutcomeAdd.Name = "m_btnOutcomeAdd"
            Me.m_btnOutcomeAdd.UseVisualStyleBackColor = True
            '
            'm_tbxOutcomeName
            '
            resources.ApplyResources(Me.m_tbxOutcomeName, "m_tbxOutcomeName")
            Me.m_tbxOutcomeName.Name = "m_tbxOutcomeName"
            '
            'm_lblOutcome
            '
            resources.ApplyResources(Me.m_lblOutcome, "m_lblOutcome")
            Me.m_lblOutcome.Name = "m_lblOutcome"
            '
            'm_tpEmulator
            '
            Me.m_tpEmulator.Controls.Add(Me.m_nudEmulOutcomeRange)
            Me.m_tpEmulator.Controls.Add(Me.m_btnTestsetDelete)
            Me.m_tpEmulator.Controls.Add(Me.m_btnTestsetRename)
            Me.m_tpEmulator.Controls.Add(Me.m_lblEmulatorTestset)
            Me.m_tpEmulator.Controls.Add(Me.m_btnTestsetAdd)
            Me.m_tpEmulator.Controls.Add(Me.m_tbxTestsetName)
            Me.m_tpEmulator.Controls.Add(Me.m_btnTestsetApply)
            Me.m_tpEmulator.Controls.Add(Me.m_btnEmulStep)
            Me.m_tpEmulator.Controls.Add(Me.m_hdrEmulRun)
            Me.m_tpEmulator.Controls.Add(Me.m_cmbEmulPauseOptions)
            Me.m_tpEmulator.Controls.Add(Me.m_btnEmulViewOutputFolder)
            Me.m_tpEmulator.Controls.Add(Me.m_btnEmulStop)
            Me.m_tpEmulator.Controls.Add(Me.m_lblEmulTestSets)
            Me.m_tpEmulator.Controls.Add(Me.m_gridEmulTestset)
            Me.m_tpEmulator.Controls.Add(Me.m_cmbEmulTestsets)
            Me.m_tpEmulator.Controls.Add(Me.m_cbSaveOutputMaps)
            Me.m_tpEmulator.Controls.Add(Me.m_cbEmulPauseSpace)
            resources.ApplyResources(Me.m_tpEmulator, "m_tpEmulator")
            Me.m_tpEmulator.Name = "m_tpEmulator"
            Me.m_tpEmulator.UseVisualStyleBackColor = True
            '
            'm_nudEmulOutcomeRange
            '
            resources.ApplyResources(Me.m_nudEmulOutcomeRange, "m_nudEmulOutcomeRange")
            Me.m_nudEmulOutcomeRange.Minimum = New Decimal(New Integer() {2, 0, 0, 0})
            Me.m_nudEmulOutcomeRange.Name = "m_nudEmulOutcomeRange"
            Me.m_nudEmulOutcomeRange.Value = New Decimal(New Integer() {2, 0, 0, 0})
            '
            'm_btnTestsetDelete
            '
            resources.ApplyResources(Me.m_btnTestsetDelete, "m_btnTestsetDelete")
            Me.m_btnTestsetDelete.Image = Global.EwEMSPPlugin.My.Resources.Resources.delete
            Me.m_btnTestsetDelete.Name = "m_btnTestsetDelete"
            Me.m_btnTestsetDelete.UseVisualStyleBackColor = True
            '
            'm_btnTestsetRename
            '
            resources.ApplyResources(Me.m_btnTestsetRename, "m_btnTestsetRename")
            Me.m_btnTestsetRename.Image = Global.EwEMSPPlugin.My.Resources.Resources.change
            Me.m_btnTestsetRename.Name = "m_btnTestsetRename"
            Me.m_btnTestsetRename.UseVisualStyleBackColor = True
            '
            'm_lblEmulatorTestset
            '
            resources.ApplyResources(Me.m_lblEmulatorTestset, "m_lblEmulatorTestset")
            Me.m_lblEmulatorTestset.Name = "m_lblEmulatorTestset"
            '
            'm_btnTestsetAdd
            '
            resources.ApplyResources(Me.m_btnTestsetAdd, "m_btnTestsetAdd")
            Me.m_btnTestsetAdd.Image = Global.EwEMSPPlugin.My.Resources.Resources.add
            Me.m_btnTestsetAdd.Name = "m_btnTestsetAdd"
            Me.m_btnTestsetAdd.UseVisualStyleBackColor = True
            '
            'm_tbxTestsetName
            '
            resources.ApplyResources(Me.m_tbxTestsetName, "m_tbxTestsetName")
            Me.m_tbxTestsetName.Name = "m_tbxTestsetName"
            '
            'm_btnTestsetApply
            '
            resources.ApplyResources(Me.m_btnTestsetApply, "m_btnTestsetApply")
            Me.m_btnTestsetApply.Name = "m_btnTestsetApply"
            Me.m_btnTestsetApply.UseVisualStyleBackColor = True
            '
            'm_btnEmulStep
            '
            resources.ApplyResources(Me.m_btnEmulStep, "m_btnEmulStep")
            Me.m_btnEmulStep.Name = "m_btnEmulStep"
            Me.m_btnEmulStep.UseVisualStyleBackColor = True
            '
            'm_hdrEmulRun
            '
            resources.ApplyResources(Me.m_hdrEmulRun, "m_hdrEmulRun")
            Me.m_hdrEmulRun.CanCollapseParent = False
            Me.m_hdrEmulRun.CollapsedParentHeight = 0
            Me.m_hdrEmulRun.IsCollapsed = False
            Me.m_hdrEmulRun.Name = "m_hdrEmulRun"
            '
            'm_cmbEmulPauseOptions
            '
            resources.ApplyResources(Me.m_cmbEmulPauseOptions, "m_cmbEmulPauseOptions")
            Me.m_cmbEmulPauseOptions.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.m_cmbEmulPauseOptions.FormattingEnabled = True
            Me.m_cmbEmulPauseOptions.Items.AddRange(New Object() {resources.GetString("m_cmbEmulPauseOptions.Items"), resources.GetString("m_cmbEmulPauseOptions.Items1"), resources.GetString("m_cmbEmulPauseOptions.Items2")})
            Me.m_cmbEmulPauseOptions.Name = "m_cmbEmulPauseOptions"
            '
            'm_btnEmulViewOutputFolder
            '
            resources.ApplyResources(Me.m_btnEmulViewOutputFolder, "m_btnEmulViewOutputFolder")
            Me.m_btnEmulViewOutputFolder.Name = "m_btnEmulViewOutputFolder"
            Me.m_btnEmulViewOutputFolder.UseVisualStyleBackColor = True
            '
            'm_btnEmulStop
            '
            resources.ApplyResources(Me.m_btnEmulStop, "m_btnEmulStop")
            Me.m_btnEmulStop.Name = "m_btnEmulStop"
            Me.m_btnEmulStop.UseVisualStyleBackColor = True
            '
            'm_lblEmulTestSets
            '
            resources.ApplyResources(Me.m_lblEmulTestSets, "m_lblEmulTestSets")
            Me.m_lblEmulTestSets.Name = "m_lblEmulTestSets"
            '
            'm_gridEmulTestset
            '
            Me.m_gridEmulTestset.AllowBlockSelect = False
            resources.ApplyResources(Me.m_gridEmulTestset, "m_gridEmulTestset")
            Me.m_gridEmulTestset.AutoSizeMinHeight = 10
            Me.m_gridEmulTestset.AutoSizeMinWidth = 10
            Me.m_gridEmulTestset.AutoStretchColumnsToFitWidth = True
            Me.m_gridEmulTestset.AutoStretchRowsToFitHeight = False
            Me.m_gridEmulTestset.BackColor = System.Drawing.Color.White
            Me.m_gridEmulTestset.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.m_gridEmulTestset.ContextMenuStyle = CType((((SourceGrid2.ContextMenuStyle.ColumnResize Or SourceGrid2.ContextMenuStyle.AutoSize) _
            Or SourceGrid2.ContextMenuStyle.CopyPasteSelection) _
            Or SourceGrid2.ContextMenuStyle.CellContextMenu), SourceGrid2.ContextMenuStyle)
            Me.m_gridEmulTestset.CustomSort = False
            Me.m_gridEmulTestset.DataName = "MEL pressure testdata"
            Me.m_gridEmulTestset.FixedColumnWidths = False
            Me.m_gridEmulTestset.FocusStyle = SourceGrid2.FocusStyle.None
            Me.m_gridEmulTestset.Game = Nothing
            Me.m_gridEmulTestset.GridToolTipActive = True
            Me.m_gridEmulTestset.IsLayoutSuspended = False
            Me.m_gridEmulTestset.IsOutputGrid = False
            Me.m_gridEmulTestset.Name = "m_gridEmulTestset"
            Me.m_gridEmulTestset.SpecialKeys = CType((((((((((SourceGrid2.GridSpecialKeys.Ctrl_C Or SourceGrid2.GridSpecialKeys.Ctrl_V) _
            Or SourceGrid2.GridSpecialKeys.Ctrl_X) _
            Or SourceGrid2.GridSpecialKeys.Delete) _
            Or SourceGrid2.GridSpecialKeys.Arrows) _
            Or SourceGrid2.GridSpecialKeys.Tab) _
            Or SourceGrid2.GridSpecialKeys.PageDownUp) _
            Or SourceGrid2.GridSpecialKeys.Enter) _
            Or SourceGrid2.GridSpecialKeys.Escape) _
            Or SourceGrid2.GridSpecialKeys.Backspace), SourceGrid2.GridSpecialKeys)
            Me.m_gridEmulTestset.Testset = Nothing
            Me.m_gridEmulTestset.UIContext = Nothing
            '
            'm_cmbEmulTestsets
            '
            resources.ApplyResources(Me.m_cmbEmulTestsets, "m_cmbEmulTestsets")
            Me.m_cmbEmulTestsets.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.m_cmbEmulTestsets.FormattingEnabled = True
            Me.m_cmbEmulTestsets.Name = "m_cmbEmulTestsets"
            Me.m_cmbEmulTestsets.Sorted = True
            '
            'm_cbSaveOutputMaps
            '
            resources.ApplyResources(Me.m_cbSaveOutputMaps, "m_cbSaveOutputMaps")
            Me.m_cbSaveOutputMaps.Name = "m_cbSaveOutputMaps"
            Me.m_cbSaveOutputMaps.UseVisualStyleBackColor = True
            '
            'm_cbEmulPauseSpace
            '
            resources.ApplyResources(Me.m_cbEmulPauseSpace, "m_cbEmulPauseSpace")
            Me.m_cbEmulPauseSpace.Name = "m_cbEmulPauseSpace"
            Me.m_cbEmulPauseSpace.UseVisualStyleBackColor = True
            '
            'm_tpAbout
            '
            Me.m_tpAbout.Controls.Add(Me.m_tlpAbout)
            resources.ApplyResources(Me.m_tpAbout, "m_tpAbout")
            Me.m_tpAbout.Name = "m_tpAbout"
            Me.m_tpAbout.UseVisualStyleBackColor = True
            '
            'm_tlpAbout
            '
            resources.ApplyResources(Me.m_tlpAbout, "m_tlpAbout")
            Me.m_tlpAbout.Controls.Add(Me.m_lblAboutDescription, 0, 1)
            Me.m_tlpAbout.Controls.Add(Me.m_tlpLogos, 0, 5)
            Me.m_tlpAbout.Controls.Add(Me.m_lblAboutCredits, 0, 2)
            Me.m_tlpAbout.Controls.Add(Me.m_lblAboutVersion, 0, 0)
            Me.m_tlpAbout.Controls.Add(Me.m_hdrCredits, 0, 4)
            Me.m_tlpAbout.Name = "m_tlpAbout"
            '
            'm_lblAboutDescription
            '
            resources.ApplyResources(Me.m_lblAboutDescription, "m_lblAboutDescription")
            Me.m_lblAboutDescription.Name = "m_lblAboutDescription"
            '
            'm_tlpLogos
            '
            resources.ApplyResources(Me.m_tlpLogos, "m_tlpLogos")
            Me.m_tlpLogos.Controls.Add(Me.m_pbEII, 7, 0)
            Me.m_tlpLogos.Controls.Add(Me.m_pbNHTV, 5, 0)
            Me.m_tlpLogos.Controls.Add(Me.m_pbRWS, 3, 0)
            Me.m_tlpLogos.Controls.Add(Me.m_pbMSPC2050, 1, 0)
            Me.m_tlpLogos.Name = "m_tlpLogos"
            '
            'm_pbEII
            '
            resources.ApplyResources(Me.m_pbEII, "m_pbEII")
            Me.m_pbEII.Name = "m_pbEII"
            Me.m_pbEII.TabStop = False
            '
            'm_pbNHTV
            '
            Me.m_pbNHTV.BackgroundImage = Global.EwEMSPPlugin.My.Resources.Resources.NHTV
            resources.ApplyResources(Me.m_pbNHTV, "m_pbNHTV")
            Me.m_pbNHTV.Name = "m_pbNHTV"
            Me.m_pbNHTV.TabStop = False
            '
            'm_pbRWS
            '
            Me.m_pbRWS.BackgroundImage = Global.EwEMSPPlugin.My.Resources.Resources.rijkswaterstaat
            resources.ApplyResources(Me.m_pbRWS, "m_pbRWS")
            Me.m_pbRWS.Name = "m_pbRWS"
            Me.m_pbRWS.TabStop = False
            '
            'm_pbMSPC2050
            '
            Me.m_pbMSPC2050.BackgroundImage = Global.EwEMSPPlugin.My.Resources.Resources.MSPChallenge2050
            resources.ApplyResources(Me.m_pbMSPC2050, "m_pbMSPC2050")
            Me.m_pbMSPC2050.Name = "m_pbMSPC2050"
            Me.m_pbMSPC2050.TabStop = False
            '
            'm_lblAboutCredits
            '
            resources.ApplyResources(Me.m_lblAboutCredits, "m_lblAboutCredits")
            Me.m_lblAboutCredits.Name = "m_lblAboutCredits"
            '
            'm_lblAboutVersion
            '
            resources.ApplyResources(Me.m_lblAboutVersion, "m_lblAboutVersion")
            Me.m_lblAboutVersion.Name = "m_lblAboutVersion"
            '
            'm_hdrCredits
            '
            Me.m_hdrCredits.CanCollapseParent = False
            Me.m_hdrCredits.CollapsedParentHeight = 0
            resources.ApplyResources(Me.m_hdrCredits, "m_hdrCredits")
            Me.m_hdrCredits.IsCollapsed = False
            Me.m_hdrCredits.Name = "m_hdrCredits"
            '
            'm_ilTabIcons
            '
            Me.m_ilTabIcons.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit
            resources.ApplyResources(Me.m_ilTabIcons, "m_ilTabIcons")
            Me.m_ilTabIcons.TransparentColor = System.Drawing.Color.Transparent
            '
            'm_cmbGames
            '
            resources.ApplyResources(Me.m_cmbGames, "m_cmbGames")
            Me.m_cmbGames.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.m_cmbGames.FormattingEnabled = True
            Me.m_cmbGames.Name = "m_cmbGames"
            Me.m_cmbGames.Sorted = True
            '
            'm_lblGames
            '
            resources.ApplyResources(Me.m_lblGames, "m_lblGames")
            Me.m_lblGames.Name = "m_lblGames"
            '
            'm_btnGameAdd
            '
            resources.ApplyResources(Me.m_btnGameAdd, "m_btnGameAdd")
            Me.m_btnGameAdd.Image = Global.EwEMSPPlugin.My.Resources.Resources.add
            Me.m_btnGameAdd.Name = "m_btnGameAdd"
            Me.m_btnGameAdd.UseVisualStyleBackColor = True
            '
            'm_lblGame
            '
            resources.ApplyResources(Me.m_lblGame, "m_lblGame")
            Me.m_lblGame.Name = "m_lblGame"
            '
            'm_tbxGameName
            '
            resources.ApplyResources(Me.m_tbxGameName, "m_tbxGameName")
            Me.m_tbxGameName.Name = "m_tbxGameName"
            '
            'm_btnGameRename
            '
            resources.ApplyResources(Me.m_btnGameRename, "m_btnGameRename")
            Me.m_btnGameRename.Image = Global.EwEMSPPlugin.My.Resources.Resources.change
            Me.m_btnGameRename.Name = "m_btnGameRename"
            Me.m_btnGameRename.UseVisualStyleBackColor = True
            '
            'm_btnGameDelete
            '
            resources.ApplyResources(Me.m_btnGameDelete, "m_btnGameDelete")
            Me.m_btnGameDelete.Image = Global.EwEMSPPlugin.My.Resources.Resources.delete
            Me.m_btnGameDelete.Name = "m_btnGameDelete"
            Me.m_btnGameDelete.UseVisualStyleBackColor = True
            '
            'm_btnGameAddFromJSON
            '
            resources.ApplyResources(Me.m_btnGameAddFromJSON, "m_btnGameAddFromJSON")
            Me.m_btnGameAddFromJSON.Image = Global.EwEMSPPlugin.My.Resources.Resources.logo_json
            Me.m_btnGameAddFromJSON.Name = "m_btnGameAddFromJSON"
            Me.m_btnGameAddFromJSON.UseVisualStyleBackColor = True
            '
            'm_btnExport
            '
            resources.ApplyResources(Me.m_btnExport, "m_btnExport")
            Me.m_btnExport.Name = "m_btnExport"
            Me.m_btnExport.UseVisualStyleBackColor = True
            '
            'frmGameDesigner
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
            Me.Controls.Add(Me.m_tbxGameName)
            Me.Controls.Add(Me.m_lblGame)
            Me.Controls.Add(Me.m_btnGameAddFromJSON)
            Me.Controls.Add(Me.m_btnGameDelete)
            Me.Controls.Add(Me.m_btnGameRename)
            Me.Controls.Add(Me.m_btnExport)
            Me.Controls.Add(Me.m_btnGameAdd)
            Me.Controls.Add(Me.m_lblGames)
            Me.Controls.Add(Me.m_cmbGames)
            Me.Controls.Add(Me.m_tabConfig)
            Me.Name = "frmGameDesigner"
            Me.ShowInTaskbar = False
            Me.TabText = ""
            Me.m_tabConfig.ResumeLayout(False)
            Me.m_tpEwESettings.ResumeLayout(False)
            Me.m_tpEwESettings.PerformLayout()
            Me.m_tpInformation.ResumeLayout(False)
            Me.m_tpInformation.PerformLayout()
            Me.m_tpPressures.ResumeLayout(False)
            Me.m_tpPressures.PerformLayout()
            Me.m_tpOutcomes.ResumeLayout(False)
            Me.m_tpOutcomes.PerformLayout()
            Me.m_scOutputs.Panel1.ResumeLayout(False)
            Me.m_scOutputs.Panel2.ResumeLayout(False)
            Me.m_scOutputs.Panel2.PerformLayout()
            CType(Me.m_scOutputs, System.ComponentModel.ISupportInitialize).EndInit()
            Me.m_scOutputs.ResumeLayout(False)
            Me.m_tpEmulator.ResumeLayout(False)
            Me.m_tpEmulator.PerformLayout()
            CType(Me.m_nudEmulOutcomeRange, System.ComponentModel.ISupportInitialize).EndInit()
            Me.m_tpAbout.ResumeLayout(False)
            Me.m_tlpAbout.ResumeLayout(False)
            Me.m_tlpAbout.PerformLayout()
            Me.m_tlpLogos.ResumeLayout(False)
            CType(Me.m_pbEII, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.m_pbNHTV, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.m_pbRWS, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.m_pbMSPC2050, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub

        Private WithEvents m_tabConfig As Windows.Forms.TabControl
        Private WithEvents m_tpPressures As Windows.Forms.TabPage
        Private WithEvents m_tpOutcomes As Windows.Forms.TabPage
        Private WithEvents m_cmbGames As Windows.Forms.ComboBox
        Private WithEvents m_lblGames As Windows.Forms.Label
        Private WithEvents m_tpEwESettings As Windows.Forms.TabPage
        Private WithEvents m_tbxRunYears As Windows.Forms.TextBox
        Private WithEvents m_tbxSpinupYears As Windows.Forms.TextBox
        Private WithEvents m_lblRunYears As Windows.Forms.Label
        Private WithEvents m_lblSpinupYears As Windows.Forms.Label
        Private WithEvents m_tpEmulator As Windows.Forms.TabPage
        Private WithEvents m_btnEmulStep As Windows.Forms.Button
        Private WithEvents m_btnEmulStop As Windows.Forms.Button
        Private WithEvents m_lblEmulTestSets As Windows.Forms.Label
        Private WithEvents m_cmbEmulTestsets As Windows.Forms.ComboBox
        Private WithEvents m_gridEmulTestset As gridEmulator
        Private WithEvents m_cbSaveOutputMaps As Windows.Forms.CheckBox
        Private WithEvents m_tpAbout As Windows.Forms.TabPage
        Private WithEvents m_tlpLogos As Windows.Forms.TableLayoutPanel
        Private WithEvents m_pbEII As Windows.Forms.PictureBox
        Private WithEvents m_pbNHTV As Windows.Forms.PictureBox
        Private WithEvents m_pbRWS As Windows.Forms.PictureBox
        Private WithEvents m_pbMSPC2050 As Windows.Forms.PictureBox
        Private WithEvents m_gridPressureMappings As gridDrivers
        Private WithEvents m_lbOutputs As Windows.Forms.ListBox
        Private WithEvents m_plEmulator As Windows.Forms.Panel
        Private WithEvents m_hdrEmulRun As ScientificInterfaceShared.Controls.cEwEHeaderLabel
        Private WithEvents m_cmbEmulPauseOptions As Windows.Forms.ComboBox
        Private WithEvents m_cbEmulPauseSpace As Windows.Forms.CheckBox
        Private WithEvents m_ilTabIcons As Windows.Forms.ImageList
        Private WithEvents m_btnGameAdd As Windows.Forms.Button
        Private WithEvents m_lblGame As Windows.Forms.Label
        Private WithEvents m_btnGameRename As Windows.Forms.Button
        Private WithEvents m_btnGameDelete As Windows.Forms.Button
        Private WithEvents m_btnPressureDefaults As Windows.Forms.Button
        Private WithEvents m_btnPressureDelete As Windows.Forms.Button
        Private WithEvents m_btnPressureRename As Windows.Forms.Button
        Private WithEvents m_btnPressureAdd As Windows.Forms.Button
        Private WithEvents m_lblPressuresPressure As Windows.Forms.Label
        Private WithEvents m_tbxPressureName As Windows.Forms.TextBox
        Private WithEvents m_cmbPressureTypes As Windows.Forms.ComboBox
        Private WithEvents m_btnOutcomeDelete As Windows.Forms.Button
        Private WithEvents m_btnOutcomeRename As Windows.Forms.Button
        Private WithEvents m_btnOutcomeAdd As Windows.Forms.Button
        Private WithEvents m_tbxOutcomeName As Windows.Forms.TextBox
        Private WithEvents m_lblOutcome As Windows.Forms.Label
        Private WithEvents m_btnTestsetDelete As Windows.Forms.Button
        Private WithEvents m_btnTestsetRename As Windows.Forms.Button
        Private WithEvents m_lblEmulatorTestset As Windows.Forms.Label
        Private WithEvents m_btnTestsetAdd As Windows.Forms.Button
        Private WithEvents m_tbxTestsetName As Windows.Forms.TextBox
        Private WithEvents m_cmbOutputTypes As Windows.Forms.ComboBox
        Private WithEvents m_tbxGameName As Windows.Forms.TextBox
        Private WithEvents m_lblCheckSimTimeSeries As cImageLabel
        Private WithEvents m_lblCheckSimFishing As cImageLabel
        Private WithEvents m_lblCheckSimForcing As cImageLabel
        Private WithEvents m_lblCheckSpaceTimeSeries As cImageLabel
        Private WithEvents m_btnGameAddFromJSON As Windows.Forms.Button
        Private WithEvents m_gridOutcome As gridOutcomes
        Private WithEvents m_lblMPACellClosure2 As Windows.Forms.Label
        Private WithEvents m_lblMPACellClosure1 As Windows.Forms.Label
        Private WithEvents m_tbxMPACellClosure As Windows.Forms.TextBox
        Private WithEvents m_btnTestsetApply As Windows.Forms.Button
        Friend WithEvents m_tlpAbout As Windows.Forms.TableLayoutPanel
        Private WithEvents m_lblAboutDescription As Windows.Forms.Label
        Private WithEvents m_btnEmulViewOutputFolder As Windows.Forms.Button
        Private WithEvents m_scOutputs As Windows.Forms.SplitContainer
        Private WithEvents m_tsOutcome As cEwEToolstrip
        Private WithEvents m_cbGameCalcIndicators As Windows.Forms.CheckBox
        Private WithEvents m_lblCheckGame As cImageLabel
        Private WithEvents m_hdrValidation As cEwEHeaderLabel
        Private WithEvents m_tpInformation As Windows.Forms.TabPage
        Private WithEvents m_tbxInfoContact As Windows.Forms.TextBox
        Private WithEvents m_tbxInfoAuthor As Windows.Forms.TextBox
        Private WithEvents m_tbxInfoVersion As Windows.Forms.TextBox
        Private WithEvents m_lblInfoContact As Windows.Forms.Label
        Private WithEvents m_lblInfoAuthor As Windows.Forms.Label
        Private WithEvents m_lblInfoVersion As Windows.Forms.Label
        Private WithEvents m_lblAboutCredits As Windows.Forms.Label
        Private WithEvents m_lblAboutVersion As Windows.Forms.Label
        Private WithEvents m_tbxInfoDescription As Windows.Forms.TextBox
        Private WithEvents m_lblInfoDescription As Windows.Forms.Label
        Friend WithEvents m_btnSettingsUseCurrentScenario As Windows.Forms.Button
        Private WithEvents m_hdrCredits As cEwEHeaderLabel
        Private WithEvents m_btnExport As Windows.Forms.Button
        Private WithEvents m_nudEmulOutcomeRange As Windows.Forms.NumericUpDown
    End Class

End Namespace
