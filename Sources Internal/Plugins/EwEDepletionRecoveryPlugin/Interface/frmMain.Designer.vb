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
' Copyright 1991- 
'    UBC Fisheries Centre, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports ScientificInterfaceShared.Controls
Imports ScientificInterfaceShared.Forms
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region ' Imports

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMain
    Inherits frmEwE

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
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
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMain))
        Me.m_btnAddModel = New System.Windows.Forms.Button()
        Me.m_bntRemoveModel = New System.Windows.Forms.Button()
        Me.m_hdrModelProperties = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.m_btnAllGroups = New System.Windows.Forms.Button()
        Me.m_btnNoneGroups = New System.Windows.Forms.Button()
        Me.m_hdrOutput = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.m_lblOutputDirectory = New System.Windows.Forms.Label()
        Me.m_tbxOutputDirectory = New System.Windows.Forms.TextBox()
        Me.m_btnBrowse = New System.Windows.Forms.Button()
        Me.m_btnRun = New System.Windows.Forms.Button()
        Me.m_tsMain = New System.Windows.Forms.ToolStrip()
        Me.m_tsbReset = New System.Windows.Forms.ToolStripButton()
        Me.m_tssbLoad = New System.Windows.Forms.ToolStripButton()
        Me.m_tsbSave = New System.Windows.Forms.ToolStripButton()
        Me.m_tlpGroupCategories = New System.Windows.Forms.TableLayoutPanel()
        Me.m_glbGroups = New ScientificInterfaceShared.Controls.cGroupListBox()
        Me.m_lvCategoriesGroup = New System.Windows.Forms.ListView()
        Me.m_chGroupCat = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.m_chGroupCount = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.m_clbModels = New System.Windows.Forms.CheckedListBox()
        Me.m_lblNumberOfYears = New System.Windows.Forms.Label()
        Me.m_nudNumberOfYears = New System.Windows.Forms.NumericUpDown()
        Me.m_tbxMask = New System.Windows.Forms.TextBox()
        Me.m_lblMask = New System.Windows.Forms.Label()
        Me.m_tcModelProperties = New System.Windows.Forms.TabControl()
        Me.m_tpEcosimScenarios = New System.Windows.Forms.TabPage()
        Me.m_clbScenarios = New System.Windows.Forms.CheckedListBox()
        Me.m_tpGroups = New System.Windows.Forms.TabPage()
        Me.m_tpFleets = New System.Windows.Forms.TabPage()
        Me.m_tlpFleets = New System.Windows.Forms.TableLayoutPanel()
        Me.m_lvCategoriesFleet = New System.Windows.Forms.ListView()
        Me.m_chFleetCat = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.m_chFleetCount = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.m_flbFleets = New ScientificInterfaceShared.Controls.cFleetListBox()
        Me.m_btnAllFleets = New System.Windows.Forms.Button()
        Me.m_btnNoneFleets = New System.Windows.Forms.Button()
        Me.m_clbEcosimResults = New System.Windows.Forms.CheckedListBox()
        Me.m_btnAllResults = New System.Windows.Forms.Button()
        Me.m_btnNoneResults = New System.Windows.Forms.Button()
        Me.m_hdrModels = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.m_btnNoModels = New System.Windows.Forms.Button()
        Me.m_btnAllModels = New System.Windows.Forms.Button()
        Me.m_tcOutput = New System.Windows.Forms.TabControl()
        Me.m_tpFiles = New System.Windows.Forms.TabPage()
        Me.m_tpEcosimResults = New System.Windows.Forms.TabPage()
        Me.m_tsMain.SuspendLayout()
        Me.m_tlpGroupCategories.SuspendLayout()
        CType(Me.m_nudNumberOfYears, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.m_tcModelProperties.SuspendLayout()
        Me.m_tpEcosimScenarios.SuspendLayout()
        Me.m_tpGroups.SuspendLayout()
        Me.m_tpFleets.SuspendLayout()
        Me.m_tlpFleets.SuspendLayout()
        Me.m_tcOutput.SuspendLayout()
        Me.m_tpFiles.SuspendLayout()
        Me.m_tpEcosimResults.SuspendLayout()
        Me.SuspendLayout()
        '
        'm_btnAddModel
        '
        resources.ApplyResources(Me.m_btnAddModel, "m_btnAddModel")
        Me.m_btnAddModel.Name = "m_btnAddModel"
        Me.m_btnAddModel.UseVisualStyleBackColor = True
        '
        'm_bntRemoveModel
        '
        resources.ApplyResources(Me.m_bntRemoveModel, "m_bntRemoveModel")
        Me.m_bntRemoveModel.Name = "m_bntRemoveModel"
        Me.m_bntRemoveModel.UseVisualStyleBackColor = True
        '
        'm_hdrModelProperties
        '
        resources.ApplyResources(Me.m_hdrModelProperties, "m_hdrModelProperties")
        Me.m_hdrModelProperties.CanCollapseParent = False
        Me.m_hdrModelProperties.CollapsedParentHeight = 0
        Me.m_hdrModelProperties.IsCollapsed = False
        Me.m_hdrModelProperties.Name = "m_hdrModelProperties"
        '
        'm_btnAllGroups
        '
        resources.ApplyResources(Me.m_btnAllGroups, "m_btnAllGroups")
        Me.m_btnAllGroups.Name = "m_btnAllGroups"
        Me.m_btnAllGroups.UseVisualStyleBackColor = True
        '
        'm_btnNoneGroups
        '
        resources.ApplyResources(Me.m_btnNoneGroups, "m_btnNoneGroups")
        Me.m_btnNoneGroups.Name = "m_btnNoneGroups"
        Me.m_btnNoneGroups.UseVisualStyleBackColor = True
        '
        'm_hdrOutput
        '
        resources.ApplyResources(Me.m_hdrOutput, "m_hdrOutput")
        Me.m_hdrOutput.CanCollapseParent = False
        Me.m_hdrOutput.CollapsedParentHeight = 0
        Me.m_hdrOutput.IsCollapsed = False
        Me.m_hdrOutput.Name = "m_hdrOutput"
        '
        'm_lblOutputDirectory
        '
        resources.ApplyResources(Me.m_lblOutputDirectory, "m_lblOutputDirectory")
        Me.m_lblOutputDirectory.Name = "m_lblOutputDirectory"
        '
        'm_tbxOutputDirectory
        '
        resources.ApplyResources(Me.m_tbxOutputDirectory, "m_tbxOutputDirectory")
        Me.m_tbxOutputDirectory.Name = "m_tbxOutputDirectory"
        '
        'm_btnBrowse
        '
        resources.ApplyResources(Me.m_btnBrowse, "m_btnBrowse")
        Me.m_btnBrowse.Name = "m_btnBrowse"
        Me.m_btnBrowse.UseVisualStyleBackColor = True
        '
        'm_btnRun
        '
        resources.ApplyResources(Me.m_btnRun, "m_btnRun")
        Me.m_btnRun.Name = "m_btnRun"
        Me.m_btnRun.UseVisualStyleBackColor = True
        '
        'm_tsMain
        '
        Me.m_tsMain.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
        Me.m_tsMain.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tsbReset, Me.m_tssbLoad, Me.m_tsbSave})
        resources.ApplyResources(Me.m_tsMain, "m_tsMain")
        Me.m_tsMain.Name = "m_tsMain"
        '
        'm_tsbReset
        '
        resources.ApplyResources(Me.m_tsbReset, "m_tsbReset")
        Me.m_tsbReset.Name = "m_tsbReset"
        '
        'm_tssbLoad
        '
        resources.ApplyResources(Me.m_tssbLoad, "m_tssbLoad")
        Me.m_tssbLoad.Name = "m_tssbLoad"
        '
        'm_tsbSave
        '
        resources.ApplyResources(Me.m_tsbSave, "m_tsbSave")
        Me.m_tsbSave.Name = "m_tsbSave"
        '
        'm_tlpGroupCategories
        '
        resources.ApplyResources(Me.m_tlpGroupCategories, "m_tlpGroupCategories")
        Me.m_tlpGroupCategories.Controls.Add(Me.m_glbGroups, 1, 0)
        Me.m_tlpGroupCategories.Controls.Add(Me.m_lvCategoriesGroup, 0, 0)
        Me.m_tlpGroupCategories.Name = "m_tlpGroupCategories"
        '
        'm_glbGroups
        '
        Me.m_glbGroups.AllGroupsItemColor = System.Drawing.Color.Transparent
        Me.m_glbGroups.AllGroupsItemText = "(All)"
        resources.ApplyResources(Me.m_glbGroups, "m_glbGroups")
        Me.m_glbGroups.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed
        Me.m_glbGroups.FormattingEnabled = True
        Me.m_glbGroups.IsAllGroupsItemSelected = False
        Me.m_glbGroups.Name = "m_glbGroups"
        Me.m_glbGroups.SelectedGroup = Nothing
        Me.m_glbGroups.SelectedGroupIndex = -1
        Me.m_glbGroups.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended
        Me.m_glbGroups.ShowAllGroupsItem = False
        Me.m_glbGroups.SortThreshold = -9999.0!
        Me.m_glbGroups.SortType = ScientificInterfaceShared.Controls.cGroupListBox.eSortType.GroupNameAsc
        '
        'm_lvCategoriesGroup
        '
        Me.m_lvCategoriesGroup.Activation = System.Windows.Forms.ItemActivation.OneClick
        Me.m_lvCategoriesGroup.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.m_chGroupCat, Me.m_chGroupCount})
        resources.ApplyResources(Me.m_lvCategoriesGroup, "m_lvCategoriesGroup")
        Me.m_lvCategoriesGroup.FullRowSelect = True
        Me.m_lvCategoriesGroup.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable
        Me.m_lvCategoriesGroup.HideSelection = False
        Me.m_lvCategoriesGroup.MultiSelect = False
        Me.m_lvCategoriesGroup.Name = "m_lvCategoriesGroup"
        Me.m_lvCategoriesGroup.UseCompatibleStateImageBehavior = False
        Me.m_lvCategoriesGroup.View = System.Windows.Forms.View.Details
        '
        'm_chGroupCat
        '
        resources.ApplyResources(Me.m_chGroupCat, "m_chGroupCat")
        '
        'm_chGroupCount
        '
        resources.ApplyResources(Me.m_chGroupCount, "m_chGroupCount")
        '
        'm_clbModels
        '
        Me.m_clbModels.AllowDrop = True
        resources.ApplyResources(Me.m_clbModels, "m_clbModels")
        Me.m_clbModels.CheckOnClick = True
        Me.m_clbModels.FormattingEnabled = True
        Me.m_clbModels.Name = "m_clbModels"
        Me.m_clbModels.Sorted = True
        '
        'm_lblNumberOfYears
        '
        resources.ApplyResources(Me.m_lblNumberOfYears, "m_lblNumberOfYears")
        Me.m_lblNumberOfYears.Name = "m_lblNumberOfYears"
        '
        'm_nudNumberOfYears
        '
        resources.ApplyResources(Me.m_nudNumberOfYears, "m_nudNumberOfYears")
        Me.m_nudNumberOfYears.Maximum = New Decimal(New Integer() {200, 0, 0, 0})
        Me.m_nudNumberOfYears.Minimum = New Decimal(New Integer() {30, 0, 0, 0})
        Me.m_nudNumberOfYears.Name = "m_nudNumberOfYears"
        Me.m_nudNumberOfYears.Value = New Decimal(New Integer() {100, 0, 0, 0})
        '
        'm_tbxMask
        '
        resources.ApplyResources(Me.m_tbxMask, "m_tbxMask")
        Me.m_tbxMask.Name = "m_tbxMask"
        '
        'm_lblMask
        '
        resources.ApplyResources(Me.m_lblMask, "m_lblMask")
        Me.m_lblMask.Name = "m_lblMask"
        '
        'm_tcModelProperties
        '
        resources.ApplyResources(Me.m_tcModelProperties, "m_tcModelProperties")
        Me.m_tcModelProperties.Controls.Add(Me.m_tpEcosimScenarios)
        Me.m_tcModelProperties.Controls.Add(Me.m_tpGroups)
        Me.m_tcModelProperties.Controls.Add(Me.m_tpFleets)
        Me.m_tcModelProperties.Multiline = True
        Me.m_tcModelProperties.Name = "m_tcModelProperties"
        Me.m_tcModelProperties.SelectedIndex = 0
        '
        'm_tpEcosimScenarios
        '
        Me.m_tpEcosimScenarios.BackColor = System.Drawing.SystemColors.Control
        Me.m_tpEcosimScenarios.Controls.Add(Me.m_clbScenarios)
        resources.ApplyResources(Me.m_tpEcosimScenarios, "m_tpEcosimScenarios")
        Me.m_tpEcosimScenarios.Name = "m_tpEcosimScenarios"
        '
        'm_clbScenarios
        '
        resources.ApplyResources(Me.m_clbScenarios, "m_clbScenarios")
        Me.m_clbScenarios.CheckOnClick = True
        Me.m_clbScenarios.FormattingEnabled = True
        Me.m_clbScenarios.Name = "m_clbScenarios"
        Me.m_clbScenarios.Sorted = True
        '
        'm_tpGroups
        '
        Me.m_tpGroups.BackColor = System.Drawing.SystemColors.Control
        Me.m_tpGroups.Controls.Add(Me.m_tlpGroupCategories)
        Me.m_tpGroups.Controls.Add(Me.m_btnAllGroups)
        Me.m_tpGroups.Controls.Add(Me.m_btnNoneGroups)
        resources.ApplyResources(Me.m_tpGroups, "m_tpGroups")
        Me.m_tpGroups.Name = "m_tpGroups"
        '
        'm_tpFleets
        '
        Me.m_tpFleets.BackColor = System.Drawing.SystemColors.Control
        Me.m_tpFleets.Controls.Add(Me.m_tlpFleets)
        Me.m_tpFleets.Controls.Add(Me.m_btnAllFleets)
        Me.m_tpFleets.Controls.Add(Me.m_btnNoneFleets)
        resources.ApplyResources(Me.m_tpFleets, "m_tpFleets")
        Me.m_tpFleets.Name = "m_tpFleets"
        '
        'm_tlpFleets
        '
        resources.ApplyResources(Me.m_tlpFleets, "m_tlpFleets")
        Me.m_tlpFleets.Controls.Add(Me.m_lvCategoriesFleet, 0, 0)
        Me.m_tlpFleets.Controls.Add(Me.m_flbFleets, 1, 0)
        Me.m_tlpFleets.Name = "m_tlpFleets"
        '
        'm_lvCategoriesFleet
        '
        Me.m_lvCategoriesFleet.Activation = System.Windows.Forms.ItemActivation.OneClick
        Me.m_lvCategoriesFleet.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.m_chFleetCat, Me.m_chFleetCount})
        resources.ApplyResources(Me.m_lvCategoriesFleet, "m_lvCategoriesFleet")
        Me.m_lvCategoriesFleet.FullRowSelect = True
        Me.m_lvCategoriesFleet.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable
        Me.m_lvCategoriesFleet.HideSelection = False
        Me.m_lvCategoriesFleet.MultiSelect = False
        Me.m_lvCategoriesFleet.Name = "m_lvCategoriesFleet"
        Me.m_lvCategoriesFleet.UseCompatibleStateImageBehavior = False
        Me.m_lvCategoriesFleet.View = System.Windows.Forms.View.Details
        '
        'm_chFleetCat
        '
        resources.ApplyResources(Me.m_chFleetCat, "m_chFleetCat")
        '
        'm_chFleetCount
        '
        resources.ApplyResources(Me.m_chFleetCount, "m_chFleetCount")
        '
        'm_flbFleets
        '
        resources.ApplyResources(Me.m_flbFleets, "m_flbFleets")
        Me.m_flbFleets.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed
        Me.m_flbFleets.FormattingEnabled = True
        Me.m_flbFleets.Name = "m_flbFleets"
        Me.m_flbFleets.SelectedFleet = Nothing
        Me.m_flbFleets.SelectedFleetIndex = -1
        Me.m_flbFleets.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended
        Me.m_flbFleets.ShowAllFleetsItem = False
        Me.m_flbFleets.SortThreshold = -9999.0!
        '
        'm_btnAllFleets
        '
        resources.ApplyResources(Me.m_btnAllFleets, "m_btnAllFleets")
        Me.m_btnAllFleets.Name = "m_btnAllFleets"
        Me.m_btnAllFleets.UseVisualStyleBackColor = True
        '
        'm_btnNoneFleets
        '
        resources.ApplyResources(Me.m_btnNoneFleets, "m_btnNoneFleets")
        Me.m_btnNoneFleets.Name = "m_btnNoneFleets"
        Me.m_btnNoneFleets.UseVisualStyleBackColor = True
        '
        'm_clbEcosimResults
        '
        resources.ApplyResources(Me.m_clbEcosimResults, "m_clbEcosimResults")
        Me.m_clbEcosimResults.CheckOnClick = True
        Me.m_clbEcosimResults.FormattingEnabled = True
        Me.m_clbEcosimResults.MultiColumn = True
        Me.m_clbEcosimResults.Name = "m_clbEcosimResults"
        Me.m_clbEcosimResults.ThreeDCheckBoxes = True
        '
        'm_btnAllResults
        '
        resources.ApplyResources(Me.m_btnAllResults, "m_btnAllResults")
        Me.m_btnAllResults.Name = "m_btnAllResults"
        Me.m_btnAllResults.UseVisualStyleBackColor = True
        '
        'm_btnNoneResults
        '
        resources.ApplyResources(Me.m_btnNoneResults, "m_btnNoneResults")
        Me.m_btnNoneResults.Name = "m_btnNoneResults"
        Me.m_btnNoneResults.UseVisualStyleBackColor = True
        '
        'm_hdrModels
        '
        resources.ApplyResources(Me.m_hdrModels, "m_hdrModels")
        Me.m_hdrModels.CanCollapseParent = False
        Me.m_hdrModels.CollapsedParentHeight = 0
        Me.m_hdrModels.IsCollapsed = False
        Me.m_hdrModels.Name = "m_hdrModels"
        '
        'm_btnNoModels
        '
        resources.ApplyResources(Me.m_btnNoModels, "m_btnNoModels")
        Me.m_btnNoModels.Name = "m_btnNoModels"
        Me.m_btnNoModels.UseVisualStyleBackColor = True
        '
        'm_btnAllModels
        '
        resources.ApplyResources(Me.m_btnAllModels, "m_btnAllModels")
        Me.m_btnAllModels.Name = "m_btnAllModels"
        Me.m_btnAllModels.UseVisualStyleBackColor = True
        '
        'm_tcOutput
        '
        resources.ApplyResources(Me.m_tcOutput, "m_tcOutput")
        Me.m_tcOutput.Controls.Add(Me.m_tpEcosimResults)
        Me.m_tcOutput.Controls.Add(Me.m_tpFiles)
        Me.m_tcOutput.Name = "m_tcOutput"
        Me.m_tcOutput.SelectedIndex = 0
        '
        'm_tpFiles
        '
        Me.m_tpFiles.BackColor = System.Drawing.SystemColors.Control
        Me.m_tpFiles.Controls.Add(Me.m_lblOutputDirectory)
        Me.m_tpFiles.Controls.Add(Me.m_tbxOutputDirectory)
        Me.m_tpFiles.Controls.Add(Me.m_btnBrowse)
        Me.m_tpFiles.Controls.Add(Me.m_lblNumberOfYears)
        Me.m_tpFiles.Controls.Add(Me.m_nudNumberOfYears)
        Me.m_tpFiles.Controls.Add(Me.m_tbxMask)
        Me.m_tpFiles.Controls.Add(Me.m_lblMask)
        resources.ApplyResources(Me.m_tpFiles, "m_tpFiles")
        Me.m_tpFiles.Name = "m_tpFiles"
        '
        'm_tpEcosimResults
        '
        Me.m_tpEcosimResults.BackColor = System.Drawing.SystemColors.Control
        Me.m_tpEcosimResults.Controls.Add(Me.m_btnAllResults)
        Me.m_tpEcosimResults.Controls.Add(Me.m_clbEcosimResults)
        Me.m_tpEcosimResults.Controls.Add(Me.m_btnNoneResults)
        resources.ApplyResources(Me.m_tpEcosimResults, "m_tpEcosimResults")
        Me.m_tpEcosimResults.Name = "m_tpEcosimResults"
        '
        'frmMain
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.m_tcModelProperties)
        Me.Controls.Add(Me.m_btnNoModels)
        Me.Controls.Add(Me.m_clbModels)
        Me.Controls.Add(Me.m_btnAllModels)
        Me.Controls.Add(Me.m_hdrModelProperties)
        Me.Controls.Add(Me.m_btnAddModel)
        Me.Controls.Add(Me.m_bntRemoveModel)
        Me.Controls.Add(Me.m_hdrModels)
        Me.Controls.Add(Me.m_tcOutput)
        Me.Controls.Add(Me.m_tsMain)
        Me.Controls.Add(Me.m_btnRun)
        Me.Controls.Add(Me.m_hdrOutput)
        Me.Icon = Global.EwEDepletionRecoveryPlugin.My.Resources.Resources.DepletionRecovery
        Me.Name = "frmMain"
        Me.ShowInTaskbar = False
        Me.TabText = ""
        Me.m_tsMain.ResumeLayout(False)
        Me.m_tsMain.PerformLayout()
        Me.m_tlpGroupCategories.ResumeLayout(False)
        CType(Me.m_nudNumberOfYears, System.ComponentModel.ISupportInitialize).EndInit()
        Me.m_tcModelProperties.ResumeLayout(False)
        Me.m_tpEcosimScenarios.ResumeLayout(False)
        Me.m_tpGroups.ResumeLayout(False)
        Me.m_tpFleets.ResumeLayout(False)
        Me.m_tlpFleets.ResumeLayout(False)
        Me.m_tcOutput.ResumeLayout(False)
        Me.m_tpFiles.ResumeLayout(False)
        Me.m_tpFiles.PerformLayout()
        Me.m_tpEcosimResults.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Private WithEvents m_btnAddModel As System.Windows.Forms.Button
    Private WithEvents m_bntRemoveModel As System.Windows.Forms.Button
    Private WithEvents m_hdrModelProperties As cEwEHeaderLabel
    Private WithEvents m_glbGroups As cGroupListBox
    Private WithEvents m_btnAllGroups As System.Windows.Forms.Button
    Private WithEvents m_btnNoneGroups As System.Windows.Forms.Button
    Private WithEvents m_hdrOutput As cEwEHeaderLabel
    Private WithEvents m_lblOutputDirectory As System.Windows.Forms.Label
    Private WithEvents m_tbxOutputDirectory As System.Windows.Forms.TextBox
    Private WithEvents m_btnBrowse As System.Windows.Forms.Button
    Private WithEvents m_btnRun As System.Windows.Forms.Button
    Private WithEvents m_tsMain As System.Windows.Forms.ToolStrip
    Private WithEvents m_tsbSave As System.Windows.Forms.ToolStripButton
    Private WithEvents m_tsbReset As System.Windows.Forms.ToolStripButton
    Private WithEvents m_tlpGroupCategories As System.Windows.Forms.TableLayoutPanel
    Private WithEvents m_lvCategoriesGroup As System.Windows.Forms.ListView
    Private WithEvents m_chGroupCat As System.Windows.Forms.ColumnHeader
    Private WithEvents m_chGroupCount As System.Windows.Forms.ColumnHeader
    Private WithEvents m_clbModels As System.Windows.Forms.CheckedListBox
    Private WithEvents m_lblNumberOfYears As System.Windows.Forms.Label
    Private WithEvents m_nudNumberOfYears As System.Windows.Forms.NumericUpDown
    Private WithEvents m_tbxMask As System.Windows.Forms.TextBox
    Private WithEvents m_lblMask As System.Windows.Forms.Label
    Private WithEvents m_tssbLoad As System.Windows.Forms.ToolStripButton
    Private WithEvents m_tcModelProperties As System.Windows.Forms.TabControl
    Private WithEvents m_tpGroups As System.Windows.Forms.TabPage
    Private WithEvents m_tpFleets As System.Windows.Forms.TabPage
    Private WithEvents m_tpEcosimScenarios As System.Windows.Forms.TabPage
    Private WithEvents m_clbScenarios As System.Windows.Forms.CheckedListBox
    Private WithEvents m_tlpFleets As System.Windows.Forms.TableLayoutPanel
    Private WithEvents m_lvCategoriesFleet As System.Windows.Forms.ListView
    Private WithEvents m_chFleetCat As System.Windows.Forms.ColumnHeader
    Private WithEvents m_chFleetCount As System.Windows.Forms.ColumnHeader
    Private WithEvents m_btnAllFleets As System.Windows.Forms.Button
    Private WithEvents m_btnNoneFleets As System.Windows.Forms.Button
    Private WithEvents m_flbFleets As cFleetListBox
    Private WithEvents m_hdrModels As cEwEHeaderLabel
    Private WithEvents m_btnAllResults As System.Windows.Forms.Button
    Private WithEvents m_btnNoneResults As System.Windows.Forms.Button
    Private WithEvents m_clbEcosimResults As System.Windows.Forms.CheckedListBox
    Private WithEvents m_btnNoModels As System.Windows.Forms.Button
    Private WithEvents m_btnAllModels As System.Windows.Forms.Button
    Private WithEvents m_tcOutput As System.Windows.Forms.TabControl
    Private WithEvents m_tpFiles As System.Windows.Forms.TabPage
    Private WithEvents m_tpEcosimResults As System.Windows.Forms.TabPage
End Class
