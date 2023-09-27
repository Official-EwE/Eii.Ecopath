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
' This plug-in was developed under the Safenet project, and has been contributed
' to the EwE approach by the Safenet project.
' 
' Copyright 1991- 
'    UBC Institute for the Oceans and Fisheries, Vancouver BC, Canada, and
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'
#Region " Imports "

Option Strict On
Imports ScientificInterfaceShared.Controls
Imports ScientificInterfaceShared.Forms

#End Region ' Imports

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
        Me.m_pbCSIC = New System.Windows.Forms.PictureBox()
        Me.m_pbEII = New System.Windows.Forms.PictureBox()
        Me.m_hdrCredits = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.m_tcTrends = New System.Windows.Forms.TabControl()
        Me.m_tabMPA = New System.Windows.Forms.TabPage()
        Me.m_dgvRuleSettings = New System.Windows.Forms.DataGridView()
        Me.m_colSettingsProt = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.m_colSettingsMaxEffect = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.m_hdrRuleSettings = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.m_lblHasTrends = New System.Windows.Forms.Label()
        Me.m_lblHasMetadata = New System.Windows.Forms.Label()
        Me.m_pbHasTrends = New System.Windows.Forms.PictureBox()
        Me.m_pbHasMetadata = New System.Windows.Forms.PictureBox()
        Me.m_lblVersion = New System.Windows.Forms.Label()
        Me.m_cbEnabled = New System.Windows.Forms.CheckBox()
        Me.m_tabTrends = New System.Windows.Forms.TabPage()
        Me.m_plApplication = New System.Windows.Forms.Panel()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.m_rbApplyIsAbsolute = New System.Windows.Forms.RadioButton()
        Me.m_rbApplyIsRelative = New System.Windows.Forms.RadioButton()
        Me.m_plApplyTo = New System.Windows.Forms.Panel()
        Me.m_lblApplyTo = New System.Windows.Forms.Label()
        Me.m_rbApplyToMPAs = New System.Windows.Forms.RadioButton()
        Me.m_rbApplyToRegions = New System.Windows.Forms.RadioButton()
        Me.m_btnTrendFished = New System.Windows.Forms.Button()
        Me.m_btnTrendAll = New System.Windows.Forms.Button()
        Me.m_btnTrendNone = New System.Windows.Forms.Button()
        Me.m_tbxTrendFile = New System.Windows.Forms.TextBox()
        Me.m_btnTrendLoad = New System.Windows.Forms.Button()
        Me.m_dgvTrends = New System.Windows.Forms.DataGridView()
        Me.m_colTrendGroup = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.m_colTrendTarget = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.m_colTrendSummary = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.m_colTrendValid = New System.Windows.Forms.DataGridViewImageColumn()
        Me.m_colTrendEnable = New System.Windows.Forms.DataGridViewCheckBoxColumn()
        Me.m_lblTrendFile = New System.Windows.Forms.Label()
        Me.m_btnTrendReset = New System.Windows.Forms.Button()
        Me.m_btnTrendMagic = New System.Windows.Forms.Button()
        Me.m_tabMetadata = New System.Windows.Forms.TabPage()
        Me.m_dgvRuleData = New System.Windows.Forms.DataGridView()
        Me.m_colMPAIndex = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.m_colMPAName = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.m_colMPAUseEmitter = New System.Windows.Forms.DataGridViewCheckBoxColumn()
        Me.m_colMPAProtection = New System.Windows.Forms.DataGridViewComboBoxColumn()
        Me.m_rbApplyIsCumulative = New System.Windows.Forms.RadioButton()
        Me.m_rbApplyToHabitats = New System.Windows.Forms.RadioButton()
        Me.m_ltpCredits.SuspendLayout()
        CType(Me.m_pbSafenet, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.m_pbCSIC, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.m_pbEII, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.m_tcTrends.SuspendLayout()
        Me.m_tabMPA.SuspendLayout()
        CType(Me.m_dgvRuleSettings, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.m_pbHasTrends, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.m_pbHasMetadata, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.m_tabTrends.SuspendLayout()
        Me.m_plApplication.SuspendLayout()
        Me.m_plApplyTo.SuspendLayout()
        CType(Me.m_dgvTrends, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.m_tabMetadata.SuspendLayout()
        CType(Me.m_dgvRuleData, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'm_ltpCredits
        '
        Me.m_ltpCredits.BackColor = System.Drawing.Color.White
        resources.ApplyResources(Me.m_ltpCredits, "m_ltpCredits")
        Me.m_ltpCredits.Controls.Add(Me.m_pbSafenet, 1, 1)
        Me.m_ltpCredits.Controls.Add(Me.m_pbCSIC, 3, 1)
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
        'm_pbCSIC
        '
        Me.m_pbCSIC.BackgroundImage = Global.EwEBiomassEmitterPlugin.My.Resources.Resources.icm_transparent
        resources.ApplyResources(Me.m_pbCSIC, "m_pbCSIC")
        Me.m_pbCSIC.Name = "m_pbCSIC"
        Me.m_pbCSIC.TabStop = False
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
        Me.m_tcTrends.Controls.Add(Me.m_tabTrends)
        Me.m_tcTrends.Controls.Add(Me.m_tabMetadata)
        resources.ApplyResources(Me.m_tcTrends, "m_tcTrends")
        Me.m_tcTrends.Name = "m_tcTrends"
        Me.m_tcTrends.SelectedIndex = 0
        '
        'm_tabMPA
        '
        Me.m_tabMPA.Controls.Add(Me.m_dgvRuleSettings)
        Me.m_tabMPA.Controls.Add(Me.m_hdrRuleSettings)
        Me.m_tabMPA.Controls.Add(Me.m_lblHasTrends)
        Me.m_tabMPA.Controls.Add(Me.m_lblHasMetadata)
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
        'm_lblHasTrends
        '
        resources.ApplyResources(Me.m_lblHasTrends, "m_lblHasTrends")
        Me.m_lblHasTrends.Name = "m_lblHasTrends"
        '
        'm_lblHasMetadata
        '
        resources.ApplyResources(Me.m_lblHasMetadata, "m_lblHasMetadata")
        Me.m_lblHasMetadata.Name = "m_lblHasMetadata"
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
        'm_tabTrends
        '
        Me.m_tabTrends.Controls.Add(Me.m_plApplication)
        Me.m_tabTrends.Controls.Add(Me.m_plApplyTo)
        Me.m_tabTrends.Controls.Add(Me.m_btnTrendFished)
        Me.m_tabTrends.Controls.Add(Me.m_btnTrendAll)
        Me.m_tabTrends.Controls.Add(Me.m_btnTrendNone)
        Me.m_tabTrends.Controls.Add(Me.m_tbxTrendFile)
        Me.m_tabTrends.Controls.Add(Me.m_btnTrendLoad)
        Me.m_tabTrends.Controls.Add(Me.m_dgvTrends)
        Me.m_tabTrends.Controls.Add(Me.m_lblTrendFile)
        Me.m_tabTrends.Controls.Add(Me.m_btnTrendReset)
        Me.m_tabTrends.Controls.Add(Me.m_btnTrendMagic)
        resources.ApplyResources(Me.m_tabTrends, "m_tabTrends")
        Me.m_tabTrends.Name = "m_tabTrends"
        Me.m_tabTrends.UseVisualStyleBackColor = True
        '
        'm_plApplication
        '
        Me.m_plApplication.Controls.Add(Me.m_rbApplyIsCumulative)
        Me.m_plApplication.Controls.Add(Me.Label1)
        Me.m_plApplication.Controls.Add(Me.m_rbApplyIsAbsolute)
        Me.m_plApplication.Controls.Add(Me.m_rbApplyIsRelative)
        resources.ApplyResources(Me.m_plApplication, "m_plApplication")
        Me.m_plApplication.Name = "m_plApplication"
        '
        'Label1
        '
        resources.ApplyResources(Me.Label1, "Label1")
        Me.Label1.Name = "Label1"
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
        'm_plApplyTo
        '
        Me.m_plApplyTo.Controls.Add(Me.m_lblApplyTo)
        Me.m_plApplyTo.Controls.Add(Me.m_rbApplyToHabitats)
        Me.m_plApplyTo.Controls.Add(Me.m_rbApplyToMPAs)
        Me.m_plApplyTo.Controls.Add(Me.m_rbApplyToRegions)
        resources.ApplyResources(Me.m_plApplyTo, "m_plApplyTo")
        Me.m_plApplyTo.Name = "m_plApplyTo"
        '
        'm_lblApplyTo
        '
        resources.ApplyResources(Me.m_lblApplyTo, "m_lblApplyTo")
        Me.m_lblApplyTo.Name = "m_lblApplyTo"
        '
        'm_rbApplyToMPAs
        '
        resources.ApplyResources(Me.m_rbApplyToMPAs, "m_rbApplyToMPAs")
        Me.m_rbApplyToMPAs.Checked = True
        Me.m_rbApplyToMPAs.Name = "m_rbApplyToMPAs"
        Me.m_rbApplyToMPAs.UseVisualStyleBackColor = True
        '
        'm_rbApplyToRegions
        '
        resources.ApplyResources(Me.m_rbApplyToRegions, "m_rbApplyToRegions")
        Me.m_rbApplyToRegions.Name = "m_rbApplyToRegions"
        Me.m_rbApplyToRegions.UseVisualStyleBackColor = True
        '
        'm_btnTrendFished
        '
        resources.ApplyResources(Me.m_btnTrendFished, "m_btnTrendFished")
        Me.m_btnTrendFished.Name = "m_btnTrendFished"
        Me.m_btnTrendFished.UseVisualStyleBackColor = True
        '
        'm_btnTrendAll
        '
        resources.ApplyResources(Me.m_btnTrendAll, "m_btnTrendAll")
        Me.m_btnTrendAll.Name = "m_btnTrendAll"
        Me.m_btnTrendAll.UseVisualStyleBackColor = True
        '
        'm_btnTrendNone
        '
        resources.ApplyResources(Me.m_btnTrendNone, "m_btnTrendNone")
        Me.m_btnTrendNone.Name = "m_btnTrendNone"
        Me.m_btnTrendNone.UseVisualStyleBackColor = True
        '
        'm_tbxTrendFile
        '
        resources.ApplyResources(Me.m_tbxTrendFile, "m_tbxTrendFile")
        Me.m_tbxTrendFile.Name = "m_tbxTrendFile"
        Me.m_tbxTrendFile.ReadOnly = True
        '
        'm_btnTrendLoad
        '
        resources.ApplyResources(Me.m_btnTrendLoad, "m_btnTrendLoad")
        Me.m_btnTrendLoad.Name = "m_btnTrendLoad"
        Me.m_btnTrendLoad.UseVisualStyleBackColor = True
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
        'm_btnTrendReset
        '
        resources.ApplyResources(Me.m_btnTrendReset, "m_btnTrendReset")
        Me.m_btnTrendReset.Name = "m_btnTrendReset"
        Me.m_btnTrendReset.UseVisualStyleBackColor = True
        '
        'm_btnTrendMagic
        '
        resources.ApplyResources(Me.m_btnTrendMagic, "m_btnTrendMagic")
        Me.m_btnTrendMagic.Image = Global.EwEBiomassEmitterPlugin.My.Resources.Resources.pure_magic
        Me.m_btnTrendMagic.Name = "m_btnTrendMagic"
        Me.m_btnTrendMagic.UseVisualStyleBackColor = True
        '
        'm_tabMetadata
        '
        Me.m_tabMetadata.Controls.Add(Me.m_dgvRuleData)
        resources.ApplyResources(Me.m_tabMetadata, "m_tabMetadata")
        Me.m_tabMetadata.Name = "m_tabMetadata"
        Me.m_tabMetadata.UseVisualStyleBackColor = True
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
        'm_rbApplyIsCumulative
        '
        resources.ApplyResources(Me.m_rbApplyIsCumulative, "m_rbApplyIsCumulative")
        Me.m_rbApplyIsCumulative.Name = "m_rbApplyIsCumulative"
        Me.m_rbApplyIsCumulative.UseVisualStyleBackColor = True
        '
        'm_rbApplyToHabitats
        '
        resources.ApplyResources(Me.m_rbApplyToHabitats, "m_rbApplyToHabitats")
        Me.m_rbApplyToHabitats.Name = "m_rbApplyToHabitats"
        Me.m_rbApplyToHabitats.UseVisualStyleBackColor = True
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
        CType(Me.m_pbCSIC, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.m_pbEII, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.m_tcTrends.ResumeLayout(False)
        Me.m_tabMPA.ResumeLayout(False)
        Me.m_tabMPA.PerformLayout()
        CType(Me.m_dgvRuleSettings, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.m_pbHasTrends, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.m_pbHasMetadata, System.ComponentModel.ISupportInitialize).EndInit()
        Me.m_tabTrends.ResumeLayout(False)
        Me.m_tabTrends.PerformLayout()
        Me.m_plApplication.ResumeLayout(False)
        Me.m_plApplication.PerformLayout()
        Me.m_plApplyTo.ResumeLayout(False)
        Me.m_plApplyTo.PerformLayout()
        CType(Me.m_dgvTrends, System.ComponentModel.ISupportInitialize).EndInit()
        Me.m_tabMetadata.ResumeLayout(False)
        CType(Me.m_dgvRuleData, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Private WithEvents m_ltpCredits As Windows.Forms.TableLayoutPanel
    Private WithEvents m_pbSafenet As Windows.Forms.PictureBox
    Private WithEvents m_pbCSIC As Windows.Forms.PictureBox
    Private WithEvents m_pbEII As Windows.Forms.PictureBox
    Private WithEvents m_hdrCredits As ScientificInterfaceShared.Controls.cEwEHeaderLabel
    Friend WithEvents TableLayoutPanel1 As Windows.Forms.TableLayoutPanel
    Private WithEvents m_tcTrends As Windows.Forms.TabControl
    Private WithEvents m_tabMPA As Windows.Forms.TabPage
    Private WithEvents m_dgvRuleSettings As Windows.Forms.DataGridView
    Friend WithEvents m_colSettingsProt As Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents m_colSettingsMaxEffect As Windows.Forms.DataGridViewTextBoxColumn
    Private WithEvents m_hdrRuleSettings As cEwEHeaderLabel
    Private WithEvents m_lblHasTrends As Windows.Forms.Label
    Private WithEvents m_lblHasMetadata As Windows.Forms.Label
    Private WithEvents m_pbHasTrends As Windows.Forms.PictureBox
    Private WithEvents m_pbHasMetadata As Windows.Forms.PictureBox
    Private WithEvents m_lblVersion As Windows.Forms.Label
    Private WithEvents m_cbEnabled As Windows.Forms.CheckBox
    Private WithEvents m_tabTrends As Windows.Forms.TabPage
    Private WithEvents m_tbxTrendFile As Windows.Forms.TextBox
    Private WithEvents m_btnTrendLoad As Windows.Forms.Button
    Private WithEvents m_lblApplyTo As Windows.Forms.Label
    Private WithEvents m_rbApplyToRegions As Windows.Forms.RadioButton
    Private WithEvents m_dgvTrends As Windows.Forms.DataGridView
    Private WithEvents m_rbApplyToMPAs As Windows.Forms.RadioButton
    Private WithEvents m_lblTrendFile As Windows.Forms.Label
    Private WithEvents m_btnTrendReset As Windows.Forms.Button
    Private WithEvents m_btnTrendMagic As Windows.Forms.Button
    Private WithEvents m_tabMetadata As Windows.Forms.TabPage
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
    Friend WithEvents m_btnTrendFished As Windows.Forms.Button
    Friend WithEvents m_btnTrendAll As Windows.Forms.Button
    Friend WithEvents m_btnTrendNone As Windows.Forms.Button
    Private WithEvents m_plApplication As Windows.Forms.Panel
    Private WithEvents Label1 As Windows.Forms.Label
    Private WithEvents m_rbApplyIsAbsolute As Windows.Forms.RadioButton
    Private WithEvents m_rbApplyIsRelative As Windows.Forms.RadioButton
    Private WithEvents m_plApplyTo As Windows.Forms.Panel
    Private WithEvents m_rbApplyIsCumulative As Windows.Forms.RadioButton
    Private WithEvents m_rbApplyToHabitats As Windows.Forms.RadioButton
End Class
