' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports ScientificInterfaceShared.Controls

Partial Class dlgMergeGroups
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(disposing As Boolean)
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(dlgMergeGroups))
        Me.m_lblTarget = New System.Windows.Forms.Label()
        Me.m_cmbTarget = New System.Windows.Forms.ComboBox()
        Me.m_lblMerge = New System.Windows.Forms.Label()
        Me.m_lblNew = New System.Windows.Forms.Label()
        Me.m_tbxNewName = New System.Windows.Forms.TextBox()
        Me.m_btnOK = New System.Windows.Forms.Button()
        Me.m_btnCancel = New System.Windows.Forms.Button()
        Me.m_tlpLogo = New System.Windows.Forms.TableLayoutPanel()
        Me.m_pbLogo = New System.Windows.Forms.PictureBox()
        Me.m_rbEE = New System.Windows.Forms.RadioButton()
        Me.m_rbQB = New System.Windows.Forms.RadioButton()
        Me.m_rbPB = New System.Windows.Forms.RadioButton()
        Me.m_rbB = New System.Windows.Forms.RadioButton()
        Me.m_lblEstimate = New System.Windows.Forms.Label()
        Me.m_cmbMerge = New System.Windows.Forms.ComboBox()
        Me.m_tcInputs = New System.Windows.Forms.TabControl()
        Me.m_tabBasicInput = New System.Windows.Forms.TabPage()
        Me.m_grid = New EwEMergeSplitGroupsPlugin.gridGroupInput()
        Me.m_tabDiets = New System.Windows.Forms.TabPage()
        Me.m_gridDietComp = New EwEMergeSplitGroupsPlugin.gridDietComposition()
        Me.m_tabTaxonomy = New System.Windows.Forms.TabPage()
        Me.m_gridTaxa = New EwEMergeSplitGroupsPlugin.gridTaxonomy()
        Me.m_tlpBasicInput = New System.Windows.Forms.TableLayoutPanel()
        Me.m_plEstimate = New System.Windows.Forms.Panel()
        Me.m_tlpLogo.SuspendLayout()
        CType(Me.m_pbLogo, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.m_tcInputs.SuspendLayout()
        Me.m_tabBasicInput.SuspendLayout()
        Me.m_tabDiets.SuspendLayout()
        Me.m_tabTaxonomy.SuspendLayout()
        Me.m_tlpBasicInput.SuspendLayout()
        Me.m_plEstimate.SuspendLayout()
        Me.SuspendLayout()
        '
        'm_lblTarget
        '
        resources.ApplyResources(Me.m_lblTarget, "m_lblTarget")
        Me.m_lblTarget.Name = "m_lblTarget"
        '
        'm_cmbTarget
        '
        resources.ApplyResources(Me.m_cmbTarget, "m_cmbTarget")
        Me.m_cmbTarget.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.m_cmbTarget.FormattingEnabled = True
        Me.m_cmbTarget.Name = "m_cmbTarget"
        '
        'm_lblMerge
        '
        resources.ApplyResources(Me.m_lblMerge, "m_lblMerge")
        Me.m_lblMerge.Name = "m_lblMerge"
        '
        'm_lblNew
        '
        resources.ApplyResources(Me.m_lblNew, "m_lblNew")
        Me.m_lblNew.Name = "m_lblNew"
        '
        'm_tbxNewName
        '
        resources.ApplyResources(Me.m_tbxNewName, "m_tbxNewName")
        Me.m_tbxNewName.Name = "m_tbxNewName"
        '
        'm_btnOK
        '
        resources.ApplyResources(Me.m_btnOK, "m_btnOK")
        Me.m_btnOK.Name = "m_btnOK"
        Me.m_btnOK.UseVisualStyleBackColor = True
        '
        'm_btnCancel
        '
        resources.ApplyResources(Me.m_btnCancel, "m_btnCancel")
        Me.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.m_btnCancel.Name = "m_btnCancel"
        Me.m_btnCancel.UseVisualStyleBackColor = True
        '
        'm_tlpLogo
        '
        Me.m_tlpLogo.BackColor = System.Drawing.Color.White
        resources.ApplyResources(Me.m_tlpLogo, "m_tlpLogo")
        Me.m_tlpLogo.Controls.Add(Me.m_pbLogo, 1, 1)
        Me.m_tlpLogo.Name = "m_tlpLogo"
        '
        'm_pbLogo
        '
        Me.m_pbLogo.BackgroundImage = Global.EwEMergeSplitGroupsPlugin.My.Resources.Resources.geomar_logo_en_print
        resources.ApplyResources(Me.m_pbLogo, "m_pbLogo")
        Me.m_pbLogo.Name = "m_pbLogo"
        Me.m_pbLogo.TabStop = False
        '
        'm_rbEE
        '
        resources.ApplyResources(Me.m_rbEE, "m_rbEE")
        Me.m_rbEE.Name = "m_rbEE"
        Me.m_rbEE.UseVisualStyleBackColor = True
        '
        'm_rbQB
        '
        resources.ApplyResources(Me.m_rbQB, "m_rbQB")
        Me.m_rbQB.Name = "m_rbQB"
        Me.m_rbQB.UseVisualStyleBackColor = True
        '
        'm_rbPB
        '
        resources.ApplyResources(Me.m_rbPB, "m_rbPB")
        Me.m_rbPB.Name = "m_rbPB"
        Me.m_rbPB.UseVisualStyleBackColor = True
        '
        'm_rbB
        '
        resources.ApplyResources(Me.m_rbB, "m_rbB")
        Me.m_rbB.Checked = True
        Me.m_rbB.Name = "m_rbB"
        Me.m_rbB.TabStop = True
        Me.m_rbB.UseVisualStyleBackColor = True
        '
        'm_lblEstimate
        '
        resources.ApplyResources(Me.m_lblEstimate, "m_lblEstimate")
        Me.m_lblEstimate.Name = "m_lblEstimate"
        '
        'm_cmbMerge
        '
        resources.ApplyResources(Me.m_cmbMerge, "m_cmbMerge")
        Me.m_cmbMerge.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.m_cmbMerge.FormattingEnabled = True
        Me.m_cmbMerge.Name = "m_cmbMerge"
        '
        'm_tcInputs
        '
        resources.ApplyResources(Me.m_tcInputs, "m_tcInputs")
        Me.m_tcInputs.Controls.Add(Me.m_tabBasicInput)
        Me.m_tcInputs.Controls.Add(Me.m_tabDiets)
        Me.m_tcInputs.Controls.Add(Me.m_tabTaxonomy)
        Me.m_tcInputs.Name = "m_tcInputs"
        Me.m_tcInputs.SelectedIndex = 0
        '
        'm_tabBasicInput
        '
        Me.m_tabBasicInput.Controls.Add(Me.m_tlpBasicInput)
        resources.ApplyResources(Me.m_tabBasicInput, "m_tabBasicInput")
        Me.m_tabBasicInput.Name = "m_tabBasicInput"
        Me.m_tabBasicInput.UseVisualStyleBackColor = True
        '
        'm_grid
        '
        Me.m_grid.AllowBlockSelect = True
        Me.m_grid.AutoSizeMinHeight = 10
        Me.m_grid.AutoSizeMinWidth = 10
        Me.m_grid.AutoStretchColumnsToFitWidth = True
        Me.m_grid.AutoStretchRowsToFitHeight = False
        Me.m_grid.BackColor = System.Drawing.Color.White
        Me.m_grid.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.m_grid.ContextMenuStyle = CType((((SourceGrid2.ContextMenuStyle.ColumnResize Or SourceGrid2.ContextMenuStyle.AutoSize) _
            Or SourceGrid2.ContextMenuStyle.CopyPasteSelection) _
            Or SourceGrid2.ContextMenuStyle.CellContextMenu), SourceGrid2.ContextMenuStyle)
        Me.m_grid.CustomSort = False
        Me.m_grid.DataName = "grid content"
        resources.ApplyResources(Me.m_grid, "m_grid")
        Me.m_grid.FixedColumnWidths = True
        Me.m_grid.FocusStyle = SourceGrid2.FocusStyle.None
        Me.m_grid.GridToolTipActive = True
        Me.m_grid.IsLayoutSuspended = False
        Me.m_grid.Name = "m_grid"
        Me.m_grid.SpecialKeys = CType((((((((((SourceGrid2.GridSpecialKeys.Ctrl_C Or SourceGrid2.GridSpecialKeys.Ctrl_V) _
            Or SourceGrid2.GridSpecialKeys.Ctrl_X) _
            Or SourceGrid2.GridSpecialKeys.Delete) _
            Or SourceGrid2.GridSpecialKeys.Arrows) _
            Or SourceGrid2.GridSpecialKeys.Tab) _
            Or SourceGrid2.GridSpecialKeys.PageDownUp) _
            Or SourceGrid2.GridSpecialKeys.Enter) _
            Or SourceGrid2.GridSpecialKeys.Escape) _
            Or SourceGrid2.GridSpecialKeys.Backspace), SourceGrid2.GridSpecialKeys)
        Me.m_grid.UIContext = Nothing
        '
        'm_tabDiets
        '
        Me.m_tabDiets.Controls.Add(Me.m_gridDietComp)
        resources.ApplyResources(Me.m_tabDiets, "m_tabDiets")
        Me.m_tabDiets.Name = "m_tabDiets"
        Me.m_tabDiets.UseVisualStyleBackColor = True
        '
        'm_gridDietComp
        '
        Me.m_gridDietComp.AllowBlockSelect = True
        Me.m_gridDietComp.AutoSizeMinHeight = 10
        Me.m_gridDietComp.AutoSizeMinWidth = 10
        Me.m_gridDietComp.AutoStretchColumnsToFitWidth = False
        Me.m_gridDietComp.AutoStretchRowsToFitHeight = False
        Me.m_gridDietComp.BackColor = System.Drawing.Color.White
        Me.m_gridDietComp.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.m_gridDietComp.ContextMenuStyle = CType((((SourceGrid2.ContextMenuStyle.ColumnResize Or SourceGrid2.ContextMenuStyle.AutoSize) _
            Or SourceGrid2.ContextMenuStyle.CopyPasteSelection) _
            Or SourceGrid2.ContextMenuStyle.CellContextMenu), SourceGrid2.ContextMenuStyle)
        Me.m_gridDietComp.CustomSort = False
        Me.m_gridDietComp.DataName = "grid content"
        resources.ApplyResources(Me.m_gridDietComp, "m_gridDietComp")
        Me.m_gridDietComp.FixedColumnWidths = True
        Me.m_gridDietComp.FocusStyle = SourceGrid2.FocusStyle.None
        Me.m_gridDietComp.GridToolTipActive = True
        Me.m_gridDietComp.IsLayoutSuspended = False
        Me.m_gridDietComp.Name = "m_gridDietComp"
        Me.m_gridDietComp.SpecialKeys = CType((((((((((SourceGrid2.GridSpecialKeys.Ctrl_C Or SourceGrid2.GridSpecialKeys.Ctrl_V) _
            Or SourceGrid2.GridSpecialKeys.Ctrl_X) _
            Or SourceGrid2.GridSpecialKeys.Delete) _
            Or SourceGrid2.GridSpecialKeys.Arrows) _
            Or SourceGrid2.GridSpecialKeys.Tab) _
            Or SourceGrid2.GridSpecialKeys.PageDownUp) _
            Or SourceGrid2.GridSpecialKeys.Enter) _
            Or SourceGrid2.GridSpecialKeys.Escape) _
            Or SourceGrid2.GridSpecialKeys.Backspace), SourceGrid2.GridSpecialKeys)
        Me.m_gridDietComp.UIContext = Nothing
        '
        'm_tabTaxonomy
        '
        Me.m_tabTaxonomy.Controls.Add(Me.m_gridTaxa)
        resources.ApplyResources(Me.m_tabTaxonomy, "m_tabTaxonomy")
        Me.m_tabTaxonomy.Name = "m_tabTaxonomy"
        Me.m_tabTaxonomy.UseVisualStyleBackColor = True
        '
        'm_gridTaxa
        '
        Me.m_gridTaxa.AllowBlockSelect = True
        Me.m_gridTaxa.AutoSizeMinHeight = 10
        Me.m_gridTaxa.AutoSizeMinWidth = 10
        Me.m_gridTaxa.AutoStretchColumnsToFitWidth = False
        Me.m_gridTaxa.AutoStretchRowsToFitHeight = False
        Me.m_gridTaxa.BackColor = System.Drawing.Color.White
        Me.m_gridTaxa.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.m_gridTaxa.ContextMenuStyle = CType((((SourceGrid2.ContextMenuStyle.ColumnResize Or SourceGrid2.ContextMenuStyle.AutoSize) _
            Or SourceGrid2.ContextMenuStyle.CopyPasteSelection) _
            Or SourceGrid2.ContextMenuStyle.CellContextMenu), SourceGrid2.ContextMenuStyle)
        Me.m_gridTaxa.CustomSort = False
        Me.m_gridTaxa.DataName = "grid content"
        resources.ApplyResources(Me.m_gridTaxa, "m_gridTaxa")
        Me.m_gridTaxa.FixedColumnWidths = True
        Me.m_gridTaxa.FocusStyle = SourceGrid2.FocusStyle.None
        Me.m_gridTaxa.GridToolTipActive = True
        Me.m_gridTaxa.IsLayoutSuspended = False
        Me.m_gridTaxa.Name = "m_gridTaxa"
        Me.m_gridTaxa.SpecialKeys = CType((((((((((SourceGrid2.GridSpecialKeys.Ctrl_C Or SourceGrid2.GridSpecialKeys.Ctrl_V) _
            Or SourceGrid2.GridSpecialKeys.Ctrl_X) _
            Or SourceGrid2.GridSpecialKeys.Delete) _
            Or SourceGrid2.GridSpecialKeys.Arrows) _
            Or SourceGrid2.GridSpecialKeys.Tab) _
            Or SourceGrid2.GridSpecialKeys.PageDownUp) _
            Or SourceGrid2.GridSpecialKeys.Enter) _
            Or SourceGrid2.GridSpecialKeys.Escape) _
            Or SourceGrid2.GridSpecialKeys.Backspace), SourceGrid2.GridSpecialKeys)
        Me.m_gridTaxa.UIContext = Nothing
        '
        'm_tlpBasicInput
        '
        resources.ApplyResources(Me.m_tlpBasicInput, "m_tlpBasicInput")
        Me.m_tlpBasicInput.Controls.Add(Me.m_grid, 0, 1)
        Me.m_tlpBasicInput.Controls.Add(Me.m_plEstimate, 0, 0)
        Me.m_tlpBasicInput.Name = "m_tlpBasicInput"
        '
        'm_plEstimate
        '
        Me.m_plEstimate.Controls.Add(Me.m_lblEstimate)
        Me.m_plEstimate.Controls.Add(Me.m_rbQB)
        Me.m_plEstimate.Controls.Add(Me.m_rbB)
        Me.m_plEstimate.Controls.Add(Me.m_rbPB)
        Me.m_plEstimate.Controls.Add(Me.m_rbEE)
        resources.ApplyResources(Me.m_plEstimate, "m_plEstimate")
        Me.m_plEstimate.Name = "m_plEstimate"
        '
        'dlgMergeGroups
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.CancelButton = Me.m_btnCancel
        Me.Controls.Add(Me.m_tcInputs)
        Me.Controls.Add(Me.m_cmbMerge)
        Me.Controls.Add(Me.m_cmbTarget)
        Me.Controls.Add(Me.m_lblTarget)
        Me.Controls.Add(Me.m_lblMerge)
        Me.Controls.Add(Me.m_tlpLogo)
        Me.Controls.Add(Me.m_tbxNewName)
        Me.Controls.Add(Me.m_btnCancel)
        Me.Controls.Add(Me.m_lblNew)
        Me.Controls.Add(Me.m_btnOK)
        Me.Name = "dlgMergeGroups"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.m_tlpLogo.ResumeLayout(False)
        CType(Me.m_pbLogo, System.ComponentModel.ISupportInitialize).EndInit()
        Me.m_tcInputs.ResumeLayout(False)
        Me.m_tabBasicInput.ResumeLayout(False)
        Me.m_tabDiets.ResumeLayout(False)
        Me.m_tabTaxonomy.ResumeLayout(False)
        Me.m_tlpBasicInput.ResumeLayout(False)
        Me.m_plEstimate.ResumeLayout(False)
        Me.m_plEstimate.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Private WithEvents m_lblTarget As System.Windows.Forms.Label
    Private WithEvents m_cmbTarget As System.Windows.Forms.ComboBox
    Private WithEvents m_lblMerge As System.Windows.Forms.Label
    Private WithEvents m_lblNew As System.Windows.Forms.Label
    Private WithEvents m_tbxNewName As System.Windows.Forms.TextBox
    Private WithEvents m_btnOK As System.Windows.Forms.Button
    Private WithEvents m_btnCancel As System.Windows.Forms.Button
    Private WithEvents m_tlpLogo As System.Windows.Forms.TableLayoutPanel
    Private WithEvents m_pbLogo As System.Windows.Forms.PictureBox
    Private WithEvents m_rbEE As System.Windows.Forms.RadioButton
    Private WithEvents m_rbQB As System.Windows.Forms.RadioButton
    Private WithEvents m_rbPB As System.Windows.Forms.RadioButton
    Private WithEvents m_rbB As System.Windows.Forms.RadioButton
    Private WithEvents m_lblEstimate As System.Windows.Forms.Label
    Private WithEvents m_cmbMerge As System.Windows.Forms.ComboBox
    Private WithEvents m_grid As gridGroupInput
    Private WithEvents m_tcInputs As System.Windows.Forms.TabControl
    Private WithEvents m_tabBasicInput As System.Windows.Forms.TabPage
    Private WithEvents m_tabDiets As System.Windows.Forms.TabPage
    Private WithEvents m_gridDietComp As gridDietComposition
    Private WithEvents m_tabTaxonomy As System.Windows.Forms.TabPage
    Private WithEvents m_gridTaxa As gridTaxonomy
    Private WithEvents m_tlpBasicInput As Windows.Forms.TableLayoutPanel
    Private WithEvents m_plEstimate As Windows.Forms.Panel
End Class
