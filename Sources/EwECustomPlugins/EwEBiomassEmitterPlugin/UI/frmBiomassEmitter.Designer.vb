' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports ScientificInterfaceShared.Controls
Imports ScientificInterfaceShared.Forms



Partial Class frmBiomassEmitter
    Inherits frmEwE

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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmBiomassEmitter))
        Me.m_ltpCredits = New System.Windows.Forms.TableLayoutPanel()
        Me.m_pbSafenet = New System.Windows.Forms.PictureBox()
        Me.m_pbICM = New System.Windows.Forms.PictureBox()
        Me.m_pbEII = New System.Windows.Forms.PictureBox()
        Me.m_hdrCredits = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.m_tcTrends = New System.Windows.Forms.TabControl()
        Me.m_tabMPA = New System.Windows.Forms.TabPage()
        Me.m_dgvRuleSettings = New System.Windows.Forms.DataGridView()
        Me.m_colSettingsProt = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.m_colSettingsMaxEffect = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.m_hdrRuleSettings = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.m_lblHasTimeSeries = New System.Windows.Forms.Label()
        Me.m_lblHasRules = New System.Windows.Forms.Label()
        Me.m_pbHasTrends = New System.Windows.Forms.PictureBox()
        Me.m_pbHasMetadata = New System.Windows.Forms.PictureBox()
        Me.m_lblVersion = New System.Windows.Forms.Label()
        Me.m_cbEnabled = New System.Windows.Forms.CheckBox()
        Me.m_tabTimeseries = New System.Windows.Forms.TabPage()
        Me.m_plTimeSeriesApplication = New System.Windows.Forms.Panel()
        Me.m_rbApplyIsAdditive = New System.Windows.Forms.RadioButton()
        Me.m_lblTimeSeriesApplication = New System.Windows.Forms.Label()
        Me.m_rbApplyIsAbsolute = New System.Windows.Forms.RadioButton()
        Me.m_rbApplyIsRelative = New System.Windows.Forms.RadioButton()
        Me.m_plTimeSeriesTarget = New System.Windows.Forms.Panel()
        Me.m_lblTimeSeriesTarget = New System.Windows.Forms.Label()
        Me.m_rbApplyToHabitats = New System.Windows.Forms.RadioButton()
        Me.m_rbApplyToMPAs = New System.Windows.Forms.RadioButton()
        Me.m_rbApplyToRegions = New System.Windows.Forms.RadioButton()
        Me.m_btnEnableFishedGroupTimeSeries = New System.Windows.Forms.Button()
        Me.m_btnEnableAllTimeSeries = New System.Windows.Forms.Button()
        Me.m_btnDisableAllTimeSeries = New System.Windows.Forms.Button()
        Me.m_tbxTimeSeriesFile = New System.Windows.Forms.TextBox()
        Me.m_btnBrowseTimeSeries = New System.Windows.Forms.Button()
        Me.m_dgvTrends = New System.Windows.Forms.DataGridView()
        Me.m_colTrendGroup = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.m_colTrendTarget = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.m_colTrendSummary = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.m_colTrendValid = New System.Windows.Forms.DataGridViewImageColumn()
        Me.m_colTrendEnable = New System.Windows.Forms.DataGridViewCheckBoxColumn()
        Me.m_lblTrendFile = New System.Windows.Forms.Label()
        Me.m_btnResetTimeSeriesFile = New System.Windows.Forms.Button()
        Me.m_btnBuildTrend = New System.Windows.Forms.Button()
        Me.m_tabRules = New System.Windows.Forms.TabPage()
        Me.m_dgvRuleData = New System.Windows.Forms.DataGridView()
        Me.m_colMPAIndex = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.m_colMPAName = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.m_colMPAUseEmitter = New System.Windows.Forms.DataGridViewCheckBoxColumn()
        Me.m_colMPAProtection = New System.Windows.Forms.DataGridViewComboBoxColumn()
        Me.m_ltpCredits.SuspendLayout()
        CType(Me.m_pbSafenet, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.m_pbICM, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.m_pbEII, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.m_tcTrends.SuspendLayout()
        Me.m_tabMPA.SuspendLayout()
        CType(Me.m_dgvRuleSettings, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.m_pbHasTrends, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.m_pbHasMetadata, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.m_tabTimeseries.SuspendLayout()
        Me.m_plTimeSeriesApplication.SuspendLayout()
        Me.m_plTimeSeriesTarget.SuspendLayout()
        CType(Me.m_dgvTrends, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.m_tabRules.SuspendLayout()
        CType(Me.m_dgvRuleData, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'm_ltpCredits
        '
        Me.m_ltpCredits.BackColor = System.Drawing.Color.White
        resources.ApplyResources(Me.m_ltpCredits, "m_ltpCredits")
        Me.m_ltpCredits.Controls.Add(Me.m_pbSafenet, 1, 1)
        Me.m_ltpCredits.Controls.Add(Me.m_pbICM, 3, 1)
        Me.m_ltpCredits.Controls.Add(Me.m_pbEII, 5, 1)
        Me.m_ltpCredits.Name = "m_ltpCredits"
        '
        'm_pbSafenet
        '
        Me.m_pbSafenet.BackgroundImage = Global.EwEBiomassEmitterPlugin.My.Resources.Resources.Safenet_logo
        resources.ApplyResources(Me.m_pbSafenet, "m_pbSafenet")
        Me.m_pbSafenet.Name = "m_pbSafenet"
        Me.m_pbSafenet.TabStop = False
        '
        'm_pbICM
        '
        Me.m_pbICM.BackgroundImage = Global.EwEBiomassEmitterPlugin.My.Resources.Resources.ICM_logo_blue
        resources.ApplyResources(Me.m_pbICM, "m_pbICM")
        Me.m_pbICM.Name = "m_pbICM"
        Me.m_pbICM.TabStop = False
        '
        'm_pbEII
        '
        Me.m_pbEII.BackgroundImage = Global.EwEBiomassEmitterPlugin.My.Resources.Resources.EII_transparent
        resources.ApplyResources(Me.m_pbEII, "m_pbEII")
        Me.m_pbEII.Name = "m_pbEII"
        Me.m_pbEII.TabStop = False
        '
        'm_hdrCredits
        '
        Me.m_hdrCredits.CanCollapseParent = False
        Me.m_hdrCredits.CollapsedParentHeight = 0
        resources.ApplyResources(Me.m_hdrCredits, "m_hdrCredits")
        Me.m_hdrCredits.IsCollapsed = False
        Me.m_hdrCredits.Name = "m_hdrCredits"
        '
        'TableLayoutPanel1
        '
        resources.ApplyResources(Me.TableLayoutPanel1, "TableLayoutPanel1")
        Me.TableLayoutPanel1.Controls.Add(Me.m_ltpCredits, 0, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.m_hdrCredits, 0, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.m_tcTrends, 0, 0)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        '
        'm_tcTrends
        '
        Me.m_tcTrends.Controls.Add(Me.m_tabMPA)
        Me.m_tcTrends.Controls.Add(Me.m_tabTimeseries)
        Me.m_tcTrends.Controls.Add(Me.m_tabRules)
        resources.ApplyResources(Me.m_tcTrends, "m_tcTrends")
        Me.m_tcTrends.Name = "m_tcTrends"
        Me.m_tcTrends.SelectedIndex = 0
        '
        'm_tabMPA
        '
        Me.m_tabMPA.Controls.Add(Me.m_dgvRuleSettings)
        Me.m_tabMPA.Controls.Add(Me.m_hdrRuleSettings)
        Me.m_tabMPA.Controls.Add(Me.m_lblHasTimeSeries)
        Me.m_tabMPA.Controls.Add(Me.m_lblHasRules)
        Me.m_tabMPA.Controls.Add(Me.m_pbHasTrends)
        Me.m_tabMPA.Controls.Add(Me.m_pbHasMetadata)
        Me.m_tabMPA.Controls.Add(Me.m_lblVersion)
        Me.m_tabMPA.Controls.Add(Me.m_cbEnabled)
        resources.ApplyResources(Me.m_tabMPA, "m_tabMPA")
        Me.m_tabMPA.Name = "m_tabMPA"
        Me.m_tabMPA.UseVisualStyleBackColor = True
        '
        'm_dgvRuleSettings
        '
        Me.m_dgvRuleSettings.AllowUserToAddRows = False
        Me.m_dgvRuleSettings.AllowUserToDeleteRows = False
        resources.ApplyResources(Me.m_dgvRuleSettings, "m_dgvRuleSettings")
        Me.m_dgvRuleSettings.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.m_dgvRuleSettings.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.m_colSettingsProt, Me.m_colSettingsMaxEffect})
        Me.m_dgvRuleSettings.Name = "m_dgvRuleSettings"
        Me.m_dgvRuleSettings.RowHeadersVisible = False
        '
        'm_colSettingsProt
        '
        Me.m_colSettingsProt.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells
        resources.ApplyResources(Me.m_colSettingsProt, "m_colSettingsProt")
        Me.m_colSettingsProt.Name = "m_colSettingsProt"
        Me.m_colSettingsProt.ReadOnly = True
        '
        'm_colSettingsMaxEffect
        '
        Me.m_colSettingsMaxEffect.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        resources.ApplyResources(Me.m_colSettingsMaxEffect, "m_colSettingsMaxEffect")
        Me.m_colSettingsMaxEffect.Name = "m_colSettingsMaxEffect"
        '
        'm_hdrRuleSettings
        '
        resources.ApplyResources(Me.m_hdrRuleSettings, "m_hdrRuleSettings")
        Me.m_hdrRuleSettings.CanCollapseParent = False
        Me.m_hdrRuleSettings.CollapsedParentHeight = 0
        Me.m_hdrRuleSettings.IsCollapsed = False
        Me.m_hdrRuleSettings.Name = "m_hdrRuleSettings"
        '
        'm_lblHasTimeSeries
        '
        resources.ApplyResources(Me.m_lblHasTimeSeries, "m_lblHasTimeSeries")
        Me.m_lblHasTimeSeries.Name = "m_lblHasTimeSeries"
        '
        'm_lblHasRules
        '
        resources.ApplyResources(Me.m_lblHasRules, "m_lblHasRules")
        Me.m_lblHasRules.Name = "m_lblHasRules"
        '
        'm_pbHasTrends
        '
        resources.ApplyResources(Me.m_pbHasTrends, "m_pbHasTrends")
        Me.m_pbHasTrends.Name = "m_pbHasTrends"
        Me.m_pbHasTrends.TabStop = False
        '
        'm_pbHasMetadata
        '
        resources.ApplyResources(Me.m_pbHasMetadata, "m_pbHasMetadata")
        Me.m_pbHasMetadata.Name = "m_pbHasMetadata"
        Me.m_pbHasMetadata.TabStop = False
        '
        'm_lblVersion
        '
        resources.ApplyResources(Me.m_lblVersion, "m_lblVersion")
        Me.m_lblVersion.Name = "m_lblVersion"
        '
        'm_cbEnabled
        '
        resources.ApplyResources(Me.m_cbEnabled, "m_cbEnabled")
        Me.m_cbEnabled.Name = "m_cbEnabled"
        Me.m_cbEnabled.UseVisualStyleBackColor = True
        '
        'm_tabTimeseries
        '
        Me.m_tabTimeseries.Controls.Add(Me.m_plTimeSeriesApplication)
        Me.m_tabTimeseries.Controls.Add(Me.m_plTimeSeriesTarget)
        Me.m_tabTimeseries.Controls.Add(Me.m_btnEnableFishedGroupTimeSeries)
        Me.m_tabTimeseries.Controls.Add(Me.m_btnEnableAllTimeSeries)
        Me.m_tabTimeseries.Controls.Add(Me.m_btnDisableAllTimeSeries)
        Me.m_tabTimeseries.Controls.Add(Me.m_tbxTimeSeriesFile)
        Me.m_tabTimeseries.Controls.Add(Me.m_btnBrowseTimeSeries)
        Me.m_tabTimeseries.Controls.Add(Me.m_dgvTrends)
        Me.m_tabTimeseries.Controls.Add(Me.m_lblTrendFile)
        Me.m_tabTimeseries.Controls.Add(Me.m_btnResetTimeSeriesFile)
        Me.m_tabTimeseries.Controls.Add(Me.m_btnBuildTrend)
        resources.ApplyResources(Me.m_tabTimeseries, "m_tabTimeseries")
        Me.m_tabTimeseries.Name = "m_tabTimeseries"
        Me.m_tabTimeseries.UseVisualStyleBackColor = True
        '
        'm_plTimeSeriesApplication
        '
        Me.m_plTimeSeriesApplication.Controls.Add(Me.m_rbApplyIsAdditive)
        Me.m_plTimeSeriesApplication.Controls.Add(Me.m_lblTimeSeriesApplication)
        Me.m_plTimeSeriesApplication.Controls.Add(Me.m_rbApplyIsAbsolute)
        Me.m_plTimeSeriesApplication.Controls.Add(Me.m_rbApplyIsRelative)
        resources.ApplyResources(Me.m_plTimeSeriesApplication, "m_plTimeSeriesApplication")
        Me.m_plTimeSeriesApplication.Name = "m_plTimeSeriesApplication"
        '
        'm_rbApplyIsAdditive
        '
        resources.ApplyResources(Me.m_rbApplyIsAdditive, "m_rbApplyIsAdditive")
        Me.m_rbApplyIsAdditive.Name = "m_rbApplyIsAdditive"
        Me.m_rbApplyIsAdditive.UseVisualStyleBackColor = True
        '
        'm_lblTimeSeriesApplication
        '
        resources.ApplyResources(Me.m_lblTimeSeriesApplication, "m_lblTimeSeriesApplication")
        Me.m_lblTimeSeriesApplication.Name = "m_lblTimeSeriesApplication"
        '
        'm_rbApplyIsAbsolute
        '
        resources.ApplyResources(Me.m_rbApplyIsAbsolute, "m_rbApplyIsAbsolute")
        Me.m_rbApplyIsAbsolute.Name = "m_rbApplyIsAbsolute"
        Me.m_rbApplyIsAbsolute.UseVisualStyleBackColor = True
        '
        'm_rbApplyIsRelative
        '
        resources.ApplyResources(Me.m_rbApplyIsRelative, "m_rbApplyIsRelative")
        Me.m_rbApplyIsRelative.Checked = True
        Me.m_rbApplyIsRelative.Name = "m_rbApplyIsRelative"
        Me.m_rbApplyIsRelative.TabStop = True
        Me.m_rbApplyIsRelative.UseVisualStyleBackColor = True
        '
        'm_plTimeSeriesTarget
        '
        Me.m_plTimeSeriesTarget.Controls.Add(Me.m_lblTimeSeriesTarget)
        Me.m_plTimeSeriesTarget.Controls.Add(Me.m_rbApplyToHabitats)
        Me.m_plTimeSeriesTarget.Controls.Add(Me.m_rbApplyToMPAs)
        Me.m_plTimeSeriesTarget.Controls.Add(Me.m_rbApplyToRegions)
        resources.ApplyResources(Me.m_plTimeSeriesTarget, "m_plTimeSeriesTarget")
        Me.m_plTimeSeriesTarget.Name = "m_plTimeSeriesTarget"
        '
        'm_lblTimeSeriesTarget
        '
        resources.ApplyResources(Me.m_lblTimeSeriesTarget, "m_lblTimeSeriesTarget")
        Me.m_lblTimeSeriesTarget.Name = "m_lblTimeSeriesTarget"
        '
        'm_rbApplyToHabitats
        '
        resources.ApplyResources(Me.m_rbApplyToHabitats, "m_rbApplyToHabitats")
        Me.m_rbApplyToHabitats.Name = "m_rbApplyToHabitats"
        Me.m_rbApplyToHabitats.UseVisualStyleBackColor = True
        '
        'm_rbApplyToMPAs
        '
        resources.ApplyResources(Me.m_rbApplyToMPAs, "m_rbApplyToMPAs")
        Me.m_rbApplyToMPAs.Checked = True
        Me.m_rbApplyToMPAs.Name = "m_rbApplyToMPAs"
        Me.m_rbApplyToMPAs.TabStop = True
        Me.m_rbApplyToMPAs.UseVisualStyleBackColor = True
        '
        'm_rbApplyToRegions
        '
        resources.ApplyResources(Me.m_rbApplyToRegions, "m_rbApplyToRegions")
        Me.m_rbApplyToRegions.Name = "m_rbApplyToRegions"
        Me.m_rbApplyToRegions.UseVisualStyleBackColor = True
        '
        'm_btnEnableFishedGroupTimeSeries
        '
        resources.ApplyResources(Me.m_btnEnableFishedGroupTimeSeries, "m_btnEnableFishedGroupTimeSeries")
        Me.m_btnEnableFishedGroupTimeSeries.Name = "m_btnEnableFishedGroupTimeSeries"
        Me.m_btnEnableFishedGroupTimeSeries.UseVisualStyleBackColor = True
        '
        'm_btnEnableAllTimeSeries
        '
        resources.ApplyResources(Me.m_btnEnableAllTimeSeries, "m_btnEnableAllTimeSeries")
        Me.m_btnEnableAllTimeSeries.Name = "m_btnEnableAllTimeSeries"
        Me.m_btnEnableAllTimeSeries.UseVisualStyleBackColor = True
        '
        'm_btnDisableAllTimeSeries
        '
        resources.ApplyResources(Me.m_btnDisableAllTimeSeries, "m_btnDisableAllTimeSeries")
        Me.m_btnDisableAllTimeSeries.Name = "m_btnDisableAllTimeSeries"
        Me.m_btnDisableAllTimeSeries.UseVisualStyleBackColor = True
        '
        'm_tbxTimeSeriesFile
        '
        resources.ApplyResources(Me.m_tbxTimeSeriesFile, "m_tbxTimeSeriesFile")
        Me.m_tbxTimeSeriesFile.Name = "m_tbxTimeSeriesFile"
        Me.m_tbxTimeSeriesFile.ReadOnly = True
        '
        'm_btnBrowseTimeSeries
        '
        resources.ApplyResources(Me.m_btnBrowseTimeSeries, "m_btnBrowseTimeSeries")
        Me.m_btnBrowseTimeSeries.Name = "m_btnBrowseTimeSeries"
        Me.m_btnBrowseTimeSeries.UseVisualStyleBackColor = True
        '
        'm_dgvTrends
        '
        Me.m_dgvTrends.AllowUserToAddRows = False
        Me.m_dgvTrends.AllowUserToDeleteRows = False
        resources.ApplyResources(Me.m_dgvTrends, "m_dgvTrends")
        Me.m_dgvTrends.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.m_dgvTrends.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.m_colTrendGroup, Me.m_colTrendTarget, Me.m_colTrendSummary, Me.m_colTrendValid, Me.m_colTrendEnable})
        Me.m_dgvTrends.Name = "m_dgvTrends"
        Me.m_dgvTrends.ReadOnly = True
        Me.m_dgvTrends.RowHeadersVisible = False
        '
        'm_colTrendGroup
        '
        Me.m_colTrendGroup.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader
        resources.ApplyResources(Me.m_colTrendGroup, "m_colTrendGroup")
        Me.m_colTrendGroup.Name = "m_colTrendGroup"
        Me.m_colTrendGroup.ReadOnly = True
        '
        'm_colTrendTarget
        '
        Me.m_colTrendTarget.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader
        resources.ApplyResources(Me.m_colTrendTarget, "m_colTrendTarget")
        Me.m_colTrendTarget.Name = "m_colTrendTarget"
        Me.m_colTrendTarget.ReadOnly = True
        '
        'm_colTrendSummary
        '
        Me.m_colTrendSummary.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        resources.ApplyResources(Me.m_colTrendSummary, "m_colTrendSummary")
        Me.m_colTrendSummary.Name = "m_colTrendSummary"
        Me.m_colTrendSummary.ReadOnly = True
        '
        'm_colTrendValid
        '
        Me.m_colTrendValid.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader
        resources.ApplyResources(Me.m_colTrendValid, "m_colTrendValid")
        Me.m_colTrendValid.Name = "m_colTrendValid"
        Me.m_colTrendValid.ReadOnly = True
        '
        'm_colTrendEnable
        '
        resources.ApplyResources(Me.m_colTrendEnable, "m_colTrendEnable")
        Me.m_colTrendEnable.Name = "m_colTrendEnable"
        Me.m_colTrendEnable.ReadOnly = True
        '
        'm_lblTrendFile
        '
        resources.ApplyResources(Me.m_lblTrendFile, "m_lblTrendFile")
        Me.m_lblTrendFile.Name = "m_lblTrendFile"
        '
        'm_btnResetTimeSeriesFile
        '
        resources.ApplyResources(Me.m_btnResetTimeSeriesFile, "m_btnResetTimeSeriesFile")
        Me.m_btnResetTimeSeriesFile.Name = "m_btnResetTimeSeriesFile"
        Me.m_btnResetTimeSeriesFile.UseVisualStyleBackColor = True
        '
        'm_btnBuildTrend
        '
        resources.ApplyResources(Me.m_btnBuildTrend, "m_btnBuildTrend")
        Me.m_btnBuildTrend.Image = Global.EwEBiomassEmitterPlugin.My.Resources.Resources.pure_magic
        Me.m_btnBuildTrend.Name = "m_btnBuildTrend"
        Me.m_btnBuildTrend.UseVisualStyleBackColor = True
        '
        'm_tabRules
        '
        Me.m_tabRules.Controls.Add(Me.m_dgvRuleData)
        resources.ApplyResources(Me.m_tabRules, "m_tabRules")
        Me.m_tabRules.Name = "m_tabRules"
        Me.m_tabRules.UseVisualStyleBackColor = True
        '
        'm_dgvRuleData
        '
        Me.m_dgvRuleData.AllowUserToAddRows = False
        Me.m_dgvRuleData.AllowUserToDeleteRows = False
        Me.m_dgvRuleData.AllowUserToResizeRows = False
        Me.m_dgvRuleData.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.m_dgvRuleData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.m_dgvRuleData.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.m_colMPAIndex, Me.m_colMPAName, Me.m_colMPAUseEmitter, Me.m_colMPAProtection})
        resources.ApplyResources(Me.m_dgvRuleData, "m_dgvRuleData")
        Me.m_dgvRuleData.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter
        Me.m_dgvRuleData.MultiSelect = False
        Me.m_dgvRuleData.Name = "m_dgvRuleData"
        Me.m_dgvRuleData.RowHeadersVisible = False
        Me.m_dgvRuleData.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        '
        'm_colMPAIndex
        '
        resources.ApplyResources(Me.m_colMPAIndex, "m_colMPAIndex")
        Me.m_colMPAIndex.Name = "m_colMPAIndex"
        Me.m_colMPAIndex.ReadOnly = True
        '
        'm_colMPAName
        '
        Me.m_colMPAName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells
        resources.ApplyResources(Me.m_colMPAName, "m_colMPAName")
        Me.m_colMPAName.Name = "m_colMPAName"
        Me.m_colMPAName.ReadOnly = True
        '
        'm_colMPAUseEmitter
        '
        Me.m_colMPAUseEmitter.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader
        resources.ApplyResources(Me.m_colMPAUseEmitter, "m_colMPAUseEmitter")
        Me.m_colMPAUseEmitter.Name = "m_colMPAUseEmitter"
        Me.m_colMPAUseEmitter.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.m_colMPAUseEmitter.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic
        '
        'm_colMPAProtection
        '
        Me.m_colMPAProtection.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells
        resources.ApplyResources(Me.m_colMPAProtection, "m_colMPAProtection")
        Me.m_colMPAProtection.Name = "m_colMPAProtection"
        '
        'frmBiomassEmitter
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "frmBiomassEmitter"
        Me.ShowInTaskbar = False
        Me.TabText = ""
        Me.m_ltpCredits.ResumeLayout(False)
        CType(Me.m_pbSafenet, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.m_pbICM, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.m_pbEII, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.m_tcTrends.ResumeLayout(False)
        Me.m_tabMPA.ResumeLayout(False)
        Me.m_tabMPA.PerformLayout()
        CType(Me.m_dgvRuleSettings, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.m_pbHasTrends, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.m_pbHasMetadata, System.ComponentModel.ISupportInitialize).EndInit()
        Me.m_tabTimeseries.ResumeLayout(False)
        Me.m_tabTimeseries.PerformLayout()
        Me.m_plTimeSeriesApplication.ResumeLayout(False)
        Me.m_plTimeSeriesApplication.PerformLayout()
        Me.m_plTimeSeriesTarget.ResumeLayout(False)
        Me.m_plTimeSeriesTarget.PerformLayout()
        CType(Me.m_dgvTrends, System.ComponentModel.ISupportInitialize).EndInit()
        Me.m_tabRules.ResumeLayout(False)
        CType(Me.m_dgvRuleData, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Private WithEvents m_ltpCredits As Windows.Forms.TableLayoutPanel
    Private WithEvents m_pbSafenet As Windows.Forms.PictureBox
    Private WithEvents m_pbICM As Windows.Forms.PictureBox
    Private WithEvents m_pbEII As Windows.Forms.PictureBox
    Private WithEvents m_hdrCredits As ScientificInterfaceShared.Controls.cEwEHeaderLabel
    Friend WithEvents TableLayoutPanel1 As Windows.Forms.TableLayoutPanel
    Private WithEvents m_tcTrends As Windows.Forms.TabControl
    Private WithEvents m_tabMPA As Windows.Forms.TabPage
    Private WithEvents m_dgvRuleSettings As Windows.Forms.DataGridView
    Friend WithEvents m_colSettingsProt As Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents m_colSettingsMaxEffect As Windows.Forms.DataGridViewTextBoxColumn
    Private WithEvents m_hdrRuleSettings As cEwEHeaderLabel
    Private WithEvents m_lblHasTimeSeries As Windows.Forms.Label
    Private WithEvents m_lblHasRules As Windows.Forms.Label
    Private WithEvents m_pbHasTrends As Windows.Forms.PictureBox
    Private WithEvents m_pbHasMetadata As Windows.Forms.PictureBox
    Private WithEvents m_lblVersion As Windows.Forms.Label
    Private WithEvents m_cbEnabled As Windows.Forms.CheckBox
    Private WithEvents m_tabTimeseries As Windows.Forms.TabPage
    Private WithEvents m_tbxTimeSeriesFile As Windows.Forms.TextBox
    Private WithEvents m_btnBrowseTimeSeries As Windows.Forms.Button
    Private WithEvents m_lblTimeSeriesTarget As Windows.Forms.Label
    Private WithEvents m_rbApplyToRegions As Windows.Forms.RadioButton
    Private WithEvents m_dgvTrends As Windows.Forms.DataGridView
    Private WithEvents m_rbApplyToMPAs As Windows.Forms.RadioButton
    Private WithEvents m_lblTrendFile As Windows.Forms.Label
    Private WithEvents m_btnResetTimeSeriesFile As Windows.Forms.Button
    Private WithEvents m_btnBuildTrend As Windows.Forms.Button
    Private WithEvents m_tabRules As Windows.Forms.TabPage
    Private WithEvents m_dgvRuleData As Windows.Forms.DataGridView
    Friend WithEvents m_colMPAIndex As Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents m_colMPAName As Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents m_colMPAUseEmitter As Windows.Forms.DataGridViewCheckBoxColumn
    Friend WithEvents m_colMPAProtection As Windows.Forms.DataGridViewComboBoxColumn
    Friend WithEvents m_colTrendGroup As Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents m_colTrendTarget As Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents m_colTrendSummary As Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents m_colTrendValid As Windows.Forms.DataGridViewImageColumn
    Friend WithEvents m_colTrendEnable As Windows.Forms.DataGridViewCheckBoxColumn
    Private WithEvents m_plTimeSeriesApplication As Windows.Forms.Panel
    Private WithEvents m_lblTimeSeriesApplication As Windows.Forms.Label
    Private WithEvents m_rbApplyIsAbsolute As Windows.Forms.RadioButton
    Private WithEvents m_rbApplyIsRelative As Windows.Forms.RadioButton
    Private WithEvents m_plTimeSeriesTarget As Windows.Forms.Panel
    Private WithEvents m_rbApplyIsAdditive As Windows.Forms.RadioButton
    Private WithEvents m_rbApplyToHabitats As Windows.Forms.RadioButton
    Private WithEvents m_btnDisableAllTimeSeries As Windows.Forms.Button
    Private WithEvents m_btnEnableAllTimeSeries As Windows.Forms.Button
    Private WithEvents m_btnEnableFishedGroupTimeSeries As Windows.Forms.Button
End Class
