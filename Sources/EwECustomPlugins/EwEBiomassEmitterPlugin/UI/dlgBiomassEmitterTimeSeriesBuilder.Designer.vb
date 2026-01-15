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
' This plug-in was developed under the Safenet project, and has been contributed
' to the EwE approach by the Safenet project.
' 
' Copyright 1991- 
'    UBC Institute for the Oceans and Fisheries, Vancouver BC, Canada, and
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class dlgBiomassEmitterTimeSeriesBuilder
    Inherits System.Windows.Forms.Form

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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(dlgBiomassEmitterTimeSeriesBuilder))
        Me.m_droppie = New ScientificInterfaceShared.Controls.cFileDropLabel()
        Me.m_dgvSettings = New System.Windows.Forms.DataGridView()
        Me.m_colGroupNo = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.m_colName = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.m_btnSave = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.m_tbxFile = New System.Windows.Forms.TextBox()
        Me.m_btnBrowseOutput = New System.Windows.Forms.Button()
        Me.m_cbApplyOnSave = New System.Windows.Forms.CheckBox()
        Me.m_btnCancel = New System.Windows.Forms.Button()
        Me.m_dgvMappings = New System.Windows.Forms.DataGridView()
        Me.m_scGrids = New System.Windows.Forms.SplitContainer()
        Me.ToolStrip1 = New System.Windows.Forms.ToolStrip()
        Me.m_tsbnRelative = New System.Windows.Forms.ToolStripButton()
        Me.m_tsbnAbsolute = New System.Windows.Forms.ToolStripButton()
        Me.m_colMappingFrom = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.m_colMappingDest = New System.Windows.Forms.DataGridViewTextBoxColumn()
        CType(Me.m_dgvSettings, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.m_dgvMappings, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.m_scGrids, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.m_scGrids.Panel1.SuspendLayout()
        Me.m_scGrids.Panel2.SuspendLayout()
        Me.m_scGrids.SuspendLayout()
        Me.ToolStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'm_droppie
        '
        Me.m_droppie.AllowDrop = True
        Me.m_droppie.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_droppie.BackColor = System.Drawing.Color.Transparent
        Me.m_droppie.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.m_droppie.FileExtensions = ""
        Me.m_droppie.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Bold)
        Me.m_droppie.ForeColor = System.Drawing.SystemColors.ButtonShadow
        Me.m_droppie.Location = New System.Drawing.Point(12, 25)
        Me.m_droppie.MaxFiles = 0
        Me.m_droppie.Name = "m_droppie"
        Me.m_droppie.Size = New System.Drawing.Size(606, 89)
        Me.m_droppie.TabIndex = 0
        Me.m_droppie.Text = "Drop region average files here" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "(both before*.csv and after*.csv)"
        Me.m_droppie.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'm_dgvSettings
        '
        Me.m_dgvSettings.AllowUserToAddRows = False
        Me.m_dgvSettings.AllowUserToDeleteRows = False
        Me.m_dgvSettings.AllowUserToResizeRows = False
        Me.m_dgvSettings.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.m_dgvSettings.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.m_colGroupNo, Me.m_colName})
        Me.m_dgvSettings.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_dgvSettings.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter
        Me.m_dgvSettings.Location = New System.Drawing.Point(0, 0)
        Me.m_dgvSettings.Margin = New System.Windows.Forms.Padding(0)
        Me.m_dgvSettings.MultiSelect = False
        Me.m_dgvSettings.Name = "m_dgvSettings"
        Me.m_dgvSettings.ReadOnly = True
        Me.m_dgvSettings.RowHeadersVisible = False
        Me.m_dgvSettings.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.m_dgvSettings.Size = New System.Drawing.Size(606, 174)
        Me.m_dgvSettings.TabIndex = 0
        '
        'm_colGroupNo
        '
        Me.m_colGroupNo.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells
        Me.m_colGroupNo.Frozen = True
        Me.m_colGroupNo.HeaderText = "#"
        Me.m_colGroupNo.Name = "m_colGroupNo"
        Me.m_colGroupNo.ReadOnly = True
        Me.m_colGroupNo.Width = 39
        '
        'm_colName
        '
        Me.m_colName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells
        Me.m_colName.Frozen = True
        Me.m_colName.HeaderText = "Group"
        Me.m_colName.Name = "m_colName"
        Me.m_colName.ReadOnly = True
        Me.m_colName.Width = 61
        '
        'm_btnSave
        '
        Me.m_btnSave.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_btnSave.Location = New System.Drawing.Point(543, 414)
        Me.m_btnSave.Name = "m_btnSave"
        Me.m_btnSave.Size = New System.Drawing.Size(75, 23)
        Me.m_btnSave.TabIndex = 7
        Me.m_btnSave.Text = "Save"
        '
        'Label1
        '
        Me.Label1.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(14, 390)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(47, 13)
        Me.Label1.TabIndex = 2
        Me.Label1.Text = "Save to:"
        '
        'm_tbxFile
        '
        Me.m_tbxFile.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_tbxFile.Location = New System.Drawing.Point(67, 387)
        Me.m_tbxFile.Name = "m_tbxFile"
        Me.m_tbxFile.Size = New System.Drawing.Size(472, 20)
        Me.m_tbxFile.TabIndex = 3
        '
        'm_btnBrowseOutput
        '
        Me.m_btnBrowseOutput.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_btnBrowseOutput.Location = New System.Drawing.Point(545, 385)
        Me.m_btnBrowseOutput.Name = "m_btnBrowseOutput"
        Me.m_btnBrowseOutput.Size = New System.Drawing.Size(75, 23)
        Me.m_btnBrowseOutput.TabIndex = 4
        Me.m_btnBrowseOutput.Text = "&Choose..."
        Me.m_btnBrowseOutput.UseVisualStyleBackColor = True
        '
        'm_cbApplyOnSave
        '
        Me.m_cbApplyOnSave.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.m_cbApplyOnSave.AutoSize = True
        Me.m_cbApplyOnSave.Location = New System.Drawing.Point(12, 418)
        Me.m_cbApplyOnSave.Name = "m_cbApplyOnSave"
        Me.m_cbApplyOnSave.Size = New System.Drawing.Size(177, 17)
        Me.m_cbApplyOnSave.TabIndex = 5
        Me.m_cbApplyOnSave.Text = "&Load emission file when created"
        Me.m_cbApplyOnSave.UseVisualStyleBackColor = True
        '
        'm_btnCancel
        '
        Me.m_btnCancel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_btnCancel.Location = New System.Drawing.Point(462, 414)
        Me.m_btnCancel.Name = "m_btnCancel"
        Me.m_btnCancel.Size = New System.Drawing.Size(75, 23)
        Me.m_btnCancel.TabIndex = 6
        Me.m_btnCancel.Text = "Cancel"
        '
        'm_dgvMappings
        '
        Me.m_dgvMappings.AllowUserToAddRows = False
        Me.m_dgvMappings.AllowUserToDeleteRows = False
        Me.m_dgvMappings.AllowUserToResizeRows = False
        Me.m_dgvMappings.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.m_dgvMappings.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.m_colMappingFrom, Me.m_colMappingDest})
        Me.m_dgvMappings.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_dgvMappings.Location = New System.Drawing.Point(0, 0)
        Me.m_dgvMappings.Name = "m_dgvMappings"
        Me.m_dgvMappings.RowHeadersVisible = False
        Me.m_dgvMappings.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.m_dgvMappings.Size = New System.Drawing.Size(606, 84)
        Me.m_dgvMappings.TabIndex = 0
        '
        'm_scGrids
        '
        Me.m_scGrids.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_scGrids.Location = New System.Drawing.Point(12, 117)
        Me.m_scGrids.Name = "m_scGrids"
        Me.m_scGrids.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'm_scGrids.Panel1
        '
        Me.m_scGrids.Panel1.Controls.Add(Me.m_dgvSettings)
        '
        'm_scGrids.Panel2
        '
        Me.m_scGrids.Panel2.Controls.Add(Me.m_dgvMappings)
        Me.m_scGrids.Size = New System.Drawing.Size(606, 262)
        Me.m_scGrids.SplitterDistance = 174
        Me.m_scGrids.TabIndex = 1
        '
        'ToolStrip1
        '
        Me.ToolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
        Me.ToolStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tsbnRelative, Me.m_tsbnAbsolute})
        Me.ToolStrip1.Location = New System.Drawing.Point(0, 0)
        Me.ToolStrip1.Name = "ToolStrip1"
        Me.ToolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System
        Me.ToolStrip1.Size = New System.Drawing.Size(632, 25)
        Me.ToolStrip1.TabIndex = 8
        Me.ToolStrip1.Text = "ToolStrip1"
        '
        'm_tsbnRelative
        '
        Me.m_tsbnRelative.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.m_tsbnRelative.Image = CType(resources.GetObject("m_tsbnRelative.Image"), System.Drawing.Image)
        Me.m_tsbnRelative.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.m_tsbnRelative.Name = "m_tsbnRelative"
        Me.m_tsbnRelative.Size = New System.Drawing.Size(145, 22)
        Me.m_tsbnRelative.Text = "Create relative time series"
        '
        'm_tsbnAbsolute
        '
        Me.m_tsbnAbsolute.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.m_tsbnAbsolute.Image = CType(resources.GetObject("m_tsbnAbsolute.Image"), System.Drawing.Image)
        Me.m_tsbnAbsolute.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.m_tsbnAbsolute.Name = "m_tsbnAbsolute"
        Me.m_tsbnAbsolute.Size = New System.Drawing.Size(152, 22)
        Me.m_tsbnAbsolute.Text = "Create absolute time series"
        '
        'm_colMappingFrom
        '
        Me.m_colMappingFrom.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        Me.m_colMappingFrom.HeaderText = "Region in source model"
        Me.m_colMappingFrom.Name = "m_colMappingFrom"
        Me.m_colMappingFrom.ReadOnly = True
        '
        'm_colMappingDest
        '
        Me.m_colMappingDest.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        Me.m_colMappingDest.HeaderText = "Emit to region/MPA in this model"
        Me.m_colMappingDest.MaxInputLength = 2
        Me.m_colMappingDest.Name = "m_colMappingDest"
        '
        'dlgBiomassEmitterTimeSeriesBuilder
        '
        Me.AcceptButton = Me.m_btnSave
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(632, 449)
        Me.Controls.Add(Me.ToolStrip1)
        Me.Controls.Add(Me.m_scGrids)
        Me.Controls.Add(Me.m_btnCancel)
        Me.Controls.Add(Me.m_btnSave)
        Me.Controls.Add(Me.m_cbApplyOnSave)
        Me.Controls.Add(Me.m_btnBrowseOutput)
        Me.Controls.Add(Me.m_tbxFile)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.m_droppie)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "dlgBiomassEmitterTimeSeriesBuilder"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Biomass emitter CSV generation utility"
        CType(Me.m_dgvSettings, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.m_dgvMappings, System.ComponentModel.ISupportInitialize).EndInit()
        Me.m_scGrids.Panel1.ResumeLayout(False)
        Me.m_scGrids.Panel2.ResumeLayout(False)
        CType(Me.m_scGrids, System.ComponentModel.ISupportInitialize).EndInit()
        Me.m_scGrids.ResumeLayout(False)
        Me.ToolStrip1.ResumeLayout(False)
        Me.ToolStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Private WithEvents m_droppie As ScientificInterfaceShared.Controls.cFileDropLabel
    Private WithEvents m_dgvSettings As Windows.Forms.DataGridView
    Private WithEvents m_btnSave As Windows.Forms.Button
    Private WithEvents Label1 As Windows.Forms.Label
    Private WithEvents m_tbxFile As Windows.Forms.TextBox
    Private WithEvents m_btnBrowseOutput As Windows.Forms.Button
    Private WithEvents m_cbApplyOnSave As Windows.Forms.CheckBox
    Private WithEvents m_btnCancel As Windows.Forms.Button
    Friend WithEvents m_colGroupNo As Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents m_colName As Windows.Forms.DataGridViewTextBoxColumn
    Private WithEvents m_dgvMappings As Windows.Forms.DataGridView
    Friend WithEvents m_scGrids As Windows.Forms.SplitContainer
    Friend WithEvents ToolStrip1 As Windows.Forms.ToolStrip
    Private WithEvents m_tsbnRelative As Windows.Forms.ToolStripButton
    Private WithEvents m_tsbnAbsolute As Windows.Forms.ToolStripButton
    Friend WithEvents m_colMappingFrom As Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents m_colMappingDest As Windows.Forms.DataGridViewTextBoxColumn
End Class
