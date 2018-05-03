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
'    UBC Institute for the Oceans and Fisheries, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

Imports ScientificInterfaceShared

Namespace SpatialData

    Partial Class ucMultiFileDatasetConfigPage
        Inherits System.Windows.Forms.UserControl

        'Required by the Windows Form Designer
        Private components As System.ComponentModel.IContainer

        'NOTE: The following procedure is required by the Windows Form Designer
        'It can be modified using the Windows Form Designer.  
        'Do not modify it using the code editor.
        Private Sub InitializeComponent()
            Me.m_btnBrowse = New System.Windows.Forms.Button()
            Me.m_dgvFiles = New System.Windows.Forms.DataGridView()
            Me.m_colError = New System.Windows.Forms.DataGridViewImageColumn()
            Me.m_colFileName = New System.Windows.Forms.DataGridViewTextBoxColumn()
            Me.m_colTime = New EwESpatialAssetsPlugin.cCalendarColumn()
            Me.m_lblName = New System.Windows.Forms.Label()
            Me.m_tbxName = New System.Windows.Forms.TextBox()
            Me.m_tbxDescription = New System.Windows.Forms.TextBox()
            Me.m_lblDescription = New System.Windows.Forms.Label()
            Me.CCalendarColumn1 = New EwESpatialAssetsPlugin.cCalendarColumn()
            Me.DataGridViewTextBoxColumn1 = New System.Windows.Forms.DataGridViewTextBoxColumn()
            Me.m_hdrFiles = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
            Me.m_tlpConfig = New System.Windows.Forms.TableLayoutPanel()
            Me.m_plTime = New System.Windows.Forms.Panel()
            Me.m_nudSpacing = New System.Windows.Forms.NumericUpDown()
            Me.m_lblSpace2 = New System.Windows.Forms.Label()
            Me.m_lblSpace1 = New System.Windows.Forms.Label()
            Me.m_mtbIntervalStart = New System.Windows.Forms.MaskedTextBox()
            Me.m_rbFromName = New System.Windows.Forms.RadioButton()
            Me.m_rbInterval = New System.Windows.Forms.RadioButton()
            Me.m_tbxDatePart = New System.Windows.Forms.TextBox()
            Me.m_hdrTime = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
            Me.m_btnSetTime = New System.Windows.Forms.Button()
            Me.m_plFiles = New System.Windows.Forms.Panel()
            Me.m_mtbSeasonalEnd = New System.Windows.Forms.MaskedTextBox()
            Me.m_cbSeasonal = New System.Windows.Forms.CheckBox()
            Me.m_lblLocationSample = New System.Windows.Forms.Label()
            Me.m_lblLocation = New System.Windows.Forms.Label()
            Me.m_plDescription = New System.Windows.Forms.Panel()
            Me.m_cmbVarName = New System.Windows.Forms.ComboBox()
            Me.m_hdrDescription = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
            Me.m_lblVariable = New System.Windows.Forms.Label()
            CType(Me.m_dgvFiles, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.m_tlpConfig.SuspendLayout()
            Me.m_plTime.SuspendLayout()
            CType(Me.m_nudSpacing, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.m_plFiles.SuspendLayout()
            Me.m_plDescription.SuspendLayout()
            Me.SuspendLayout()
            '
            'm_btnBrowse
            '
            Me.m_btnBrowse.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_btnBrowse.Location = New System.Drawing.Point(351, 18)
            Me.m_btnBrowse.Name = "m_btnBrowse"
            Me.m_btnBrowse.Size = New System.Drawing.Size(90, 23)
            Me.m_btnBrowse.TabIndex = 2
            Me.m_btnBrowse.Text = "&Browse..."
            Me.m_btnBrowse.UseVisualStyleBackColor = True
            '
            'm_dgvFiles
            '
            Me.m_dgvFiles.AllowUserToAddRows = False
            Me.m_dgvFiles.AllowUserToDeleteRows = False
            Me.m_dgvFiles.AllowUserToResizeColumns = False
            Me.m_dgvFiles.AllowUserToResizeRows = False
            Me.m_dgvFiles.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_dgvFiles.BorderStyle = System.Windows.Forms.BorderStyle.None
            Me.m_dgvFiles.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
            Me.m_dgvFiles.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.m_colError, Me.m_colFileName, Me.m_colTime})
            Me.m_dgvFiles.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter
            Me.m_dgvFiles.Location = New System.Drawing.Point(3, 47)
            Me.m_dgvFiles.Name = "m_dgvFiles"
            Me.m_dgvFiles.RowHeadersVisible = False
            Me.m_dgvFiles.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing
            Me.m_dgvFiles.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
            Me.m_dgvFiles.ShowCellErrors = False
            Me.m_dgvFiles.ShowCellToolTips = False
            Me.m_dgvFiles.ShowEditingIcon = False
            Me.m_dgvFiles.ShowRowErrors = False
            Me.m_dgvFiles.Size = New System.Drawing.Size(438, 209)
            Me.m_dgvFiles.TabIndex = 9
            '
            'm_colError
            '
            Me.m_colError.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None
            Me.m_colError.Frozen = True
            Me.m_colError.HeaderText = ""
            Me.m_colError.Name = "m_colError"
            Me.m_colError.ReadOnly = True
            Me.m_colError.Resizable = System.Windows.Forms.DataGridViewTriState.[False]
            Me.m_colError.Width = 20
            '
            'm_colFileName
            '
            Me.m_colFileName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None
            Me.m_colFileName.Frozen = True
            Me.m_colFileName.HeaderText = "File"
            Me.m_colFileName.Name = "m_colFileName"
            Me.m_colFileName.ReadOnly = True
            Me.m_colFileName.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
            Me.m_colFileName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
            Me.m_colFileName.Width = 317
            '
            'm_colTime
            '
            Me.m_colTime.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells
            Me.m_colTime.Frozen = True
            Me.m_colTime.HeaderText = "Time"
            Me.m_colTime.MinimumWidth = 120
            Me.m_colTime.Name = "m_colTime"
            Me.m_colTime.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
            Me.m_colTime.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic
            Me.m_colTime.Width = 120
            '
            'm_lblName
            '
            Me.m_lblName.AutoSize = True
            Me.m_lblName.Location = New System.Drawing.Point(3, 23)
            Me.m_lblName.Name = "m_lblName"
            Me.m_lblName.Size = New System.Drawing.Size(38, 13)
            Me.m_lblName.TabIndex = 1
            Me.m_lblName.Text = "&Name:"
            '
            'm_tbxName
            '
            Me.m_tbxName.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_tbxName.Location = New System.Drawing.Point(80, 20)
            Me.m_tbxName.MaxLength = 100
            Me.m_tbxName.Name = "m_tbxName"
            Me.m_tbxName.Size = New System.Drawing.Size(361, 20)
            Me.m_tbxName.TabIndex = 2
            '
            'm_tbxDescription
            '
            Me.m_tbxDescription.AcceptsReturn = True
            Me.m_tbxDescription.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_tbxDescription.Location = New System.Drawing.Point(80, 46)
            Me.m_tbxDescription.Multiline = True
            Me.m_tbxDescription.Name = "m_tbxDescription"
            Me.m_tbxDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
            Me.m_tbxDescription.Size = New System.Drawing.Size(361, 73)
            Me.m_tbxDescription.TabIndex = 4
            '
            'm_lblDescription
            '
            Me.m_lblDescription.AutoSize = True
            Me.m_lblDescription.Location = New System.Drawing.Point(3, 49)
            Me.m_lblDescription.Name = "m_lblDescription"
            Me.m_lblDescription.Size = New System.Drawing.Size(63, 13)
            Me.m_lblDescription.TabIndex = 3
            Me.m_lblDescription.Text = "&Description:"
            '
            'CCalendarColumn1
            '
            Me.CCalendarColumn1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
            Me.CCalendarColumn1.HeaderText = "File"
            Me.CCalendarColumn1.Name = "CCalendarColumn1"
            Me.CCalendarColumn1.ReadOnly = True
            '
            'DataGridViewTextBoxColumn1
            '
            Me.DataGridViewTextBoxColumn1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells
            Me.DataGridViewTextBoxColumn1.HeaderText = "Time"
            Me.DataGridViewTextBoxColumn1.Name = "DataGridViewTextBoxColumn1"
            '
            'm_hdrFiles
            '
            Me.m_hdrFiles.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_hdrFiles.CanCollapseParent = False
            Me.m_hdrFiles.CollapsedParentHeight = 71
            Me.m_hdrFiles.IsCollapsed = False
            Me.m_hdrFiles.Location = New System.Drawing.Point(0, 0)
            Me.m_hdrFiles.Name = "m_hdrFiles"
            Me.m_hdrFiles.Size = New System.Drawing.Size(441, 18)
            Me.m_hdrFiles.TabIndex = 0
            Me.m_hdrFiles.Text = "Files"
            Me.m_hdrFiles.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'm_tlpConfig
            '
            Me.m_tlpConfig.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
            Me.m_tlpConfig.ColumnCount = 1
            Me.m_tlpConfig.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
            Me.m_tlpConfig.Controls.Add(Me.m_plTime, 0, 2)
            Me.m_tlpConfig.Controls.Add(Me.m_plFiles, 0, 1)
            Me.m_tlpConfig.Controls.Add(Me.m_plDescription, 0, 0)
            Me.m_tlpConfig.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_tlpConfig.Location = New System.Drawing.Point(0, 0)
            Me.m_tlpConfig.Name = "m_tlpConfig"
            Me.m_tlpConfig.RowCount = 3
            Me.m_tlpConfig.RowStyles.Add(New System.Windows.Forms.RowStyle())
            Me.m_tlpConfig.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
            Me.m_tlpConfig.RowStyles.Add(New System.Windows.Forms.RowStyle())
            Me.m_tlpConfig.Size = New System.Drawing.Size(450, 520)
            Me.m_tlpConfig.TabIndex = 0
            '
            'm_plTime
            '
            Me.m_plTime.Controls.Add(Me.m_nudSpacing)
            Me.m_plTime.Controls.Add(Me.m_lblSpace2)
            Me.m_plTime.Controls.Add(Me.m_lblSpace1)
            Me.m_plTime.Controls.Add(Me.m_mtbIntervalStart)
            Me.m_plTime.Controls.Add(Me.m_rbFromName)
            Me.m_plTime.Controls.Add(Me.m_rbInterval)
            Me.m_plTime.Controls.Add(Me.m_tbxDatePart)
            Me.m_plTime.Controls.Add(Me.m_hdrTime)
            Me.m_plTime.Controls.Add(Me.m_btnSetTime)
            Me.m_plTime.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_plTime.Location = New System.Drawing.Point(3, 445)
            Me.m_plTime.Name = "m_plTime"
            Me.m_plTime.Size = New System.Drawing.Size(444, 72)
            Me.m_plTime.TabIndex = 0
            '
            'm_nudSpacing
            '
            Me.m_nudSpacing.Location = New System.Drawing.Point(197, 21)
            Me.m_nudSpacing.Maximum = New Decimal(New Integer() {120, 0, 0, 0})
            Me.m_nudSpacing.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
            Me.m_nudSpacing.Name = "m_nudSpacing"
            Me.m_nudSpacing.Size = New System.Drawing.Size(55, 20)
            Me.m_nudSpacing.TabIndex = 11
            Me.m_nudSpacing.Value = New Decimal(New Integer() {1, 0, 0, 0})
            '
            'm_lblSpace2
            '
            Me.m_lblSpace2.AutoSize = True
            Me.m_lblSpace2.Location = New System.Drawing.Point(254, 23)
            Me.m_lblSpace2.Name = "m_lblSpace2"
            Me.m_lblSpace2.Size = New System.Drawing.Size(47, 13)
            Me.m_lblSpace2.TabIndex = 3
            Me.m_lblSpace2.Text = "month(s)"
            '
            'm_lblSpace1
            '
            Me.m_lblSpace1.AutoSize = True
            Me.m_lblSpace1.Location = New System.Drawing.Point(136, 23)
            Me.m_lblSpace1.Name = "m_lblSpace1"
            Me.m_lblSpace1.Size = New System.Drawing.Size(51, 13)
            Me.m_lblSpace1.TabIndex = 3
            Me.m_lblSpace1.Text = " , spaced"
            '
            'm_mtbIntervalStart
            '
            Me.m_mtbIntervalStart.Location = New System.Drawing.Point(80, 20)
            Me.m_mtbIntervalStart.Mask = "0000/00"
            Me.m_mtbIntervalStart.Name = "m_mtbIntervalStart"
            Me.m_mtbIntervalStart.Size = New System.Drawing.Size(52, 20)
            Me.m_mtbIntervalStart.TabIndex = 2
            Me.m_mtbIntervalStart.Text = "195001"
            '
            'm_rbFromName
            '
            Me.m_rbFromName.AutoSize = True
            Me.m_rbFromName.Location = New System.Drawing.Point(6, 47)
            Me.m_rbFromName.Name = "m_rbFromName"
            Me.m_rbFromName.Size = New System.Drawing.Size(185, 17)
            Me.m_rbFromName.TabIndex = 7
            Me.m_rbFromName.TabStop = True
            Me.m_rbFromName.Text = "From file &name (select which part):"
            Me.m_rbFromName.UseVisualStyleBackColor = True
            '
            'm_rbInterval
            '
            Me.m_rbInterval.AutoSize = True
            Me.m_rbInterval.Checked = True
            Me.m_rbInterval.Location = New System.Drawing.Point(6, 21)
            Me.m_rbInterval.Name = "m_rbInterval"
            Me.m_rbInterval.Size = New System.Drawing.Size(73, 17)
            Me.m_rbInterval.TabIndex = 1
            Me.m_rbInterval.TabStop = True
            Me.m_rbInterval.Text = "S&tarting at"
            Me.m_rbInterval.UseVisualStyleBackColor = True
            '
            'm_tbxDatePart
            '
            Me.m_tbxDatePart.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_tbxDatePart.HideSelection = False
            Me.m_tbxDatePart.Location = New System.Drawing.Point(197, 47)
            Me.m_tbxDatePart.Name = "m_tbxDatePart"
            Me.m_tbxDatePart.ReadOnly = True
            Me.m_tbxDatePart.Size = New System.Drawing.Size(128, 20)
            Me.m_tbxDatePart.TabIndex = 8
            '
            'm_hdrTime
            '
            Me.m_hdrTime.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_hdrTime.CanCollapseParent = False
            Me.m_hdrTime.CollapsedParentHeight = 76
            Me.m_hdrTime.IsCollapsed = False
            Me.m_hdrTime.Location = New System.Drawing.Point(0, 0)
            Me.m_hdrTime.Name = "m_hdrTime"
            Me.m_hdrTime.Size = New System.Drawing.Size(441, 18)
            Me.m_hdrTime.TabIndex = 0
            Me.m_hdrTime.Text = "Time utility"
            Me.m_hdrTime.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'm_btnSetTime
            '
            Me.m_btnSetTime.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_btnSetTime.Location = New System.Drawing.Point(350, 18)
            Me.m_btnSetTime.Name = "m_btnSetTime"
            Me.m_btnSetTime.Size = New System.Drawing.Size(90, 23)
            Me.m_btnSetTime.TabIndex = 10
            Me.m_btnSetTime.Text = "Set file &times"
            Me.m_btnSetTime.UseVisualStyleBackColor = True
            '
            'm_plFiles
            '
            Me.m_plFiles.Controls.Add(Me.m_mtbSeasonalEnd)
            Me.m_plFiles.Controls.Add(Me.m_cbSeasonal)
            Me.m_plFiles.Controls.Add(Me.m_dgvFiles)
            Me.m_plFiles.Controls.Add(Me.m_hdrFiles)
            Me.m_plFiles.Controls.Add(Me.m_btnBrowse)
            Me.m_plFiles.Controls.Add(Me.m_lblLocationSample)
            Me.m_plFiles.Controls.Add(Me.m_lblLocation)
            Me.m_plFiles.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_plFiles.Location = New System.Drawing.Point(3, 155)
            Me.m_plFiles.Name = "m_plFiles"
            Me.m_plFiles.Size = New System.Drawing.Size(444, 284)
            Me.m_plFiles.TabIndex = 1
            '
            'm_mtbSeasonalEnd
            '
            Me.m_mtbSeasonalEnd.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
            Me.m_mtbSeasonalEnd.Location = New System.Drawing.Point(139, 262)
            Me.m_mtbSeasonalEnd.Mask = "0000/00"
            Me.m_mtbSeasonalEnd.Name = "m_mtbSeasonalEnd"
            Me.m_mtbSeasonalEnd.Size = New System.Drawing.Size(52, 20)
            Me.m_mtbSeasonalEnd.TabIndex = 2
            Me.m_mtbSeasonalEnd.Text = "195001"
            '
            'm_cbSeasonal
            '
            Me.m_cbSeasonal.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
            Me.m_cbSeasonal.AutoSize = True
            Me.m_cbSeasonal.Location = New System.Drawing.Point(7, 264)
            Me.m_cbSeasonal.Name = "m_cbSeasonal"
            Me.m_cbSeasonal.Size = New System.Drawing.Size(126, 17)
            Me.m_cbSeasonal.TabIndex = 11
            Me.m_cbSeasonal.Text = "&Data is seasonal until"
            Me.m_cbSeasonal.UseVisualStyleBackColor = True
            '
            'm_lblLocationSample
            '
            Me.m_lblLocationSample.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_lblLocationSample.Location = New System.Drawing.Point(82, 23)
            Me.m_lblLocationSample.Name = "m_lblLocationSample"
            Me.m_lblLocationSample.Size = New System.Drawing.Size(263, 18)
            Me.m_lblLocationSample.TabIndex = 1
            Me.m_lblLocationSample.Text = "<path>"
            '
            'm_lblLocation
            '
            Me.m_lblLocation.AutoSize = True
            Me.m_lblLocation.Location = New System.Drawing.Point(4, 23)
            Me.m_lblLocation.Name = "m_lblLocation"
            Me.m_lblLocation.Size = New System.Drawing.Size(51, 13)
            Me.m_lblLocation.TabIndex = 1
            Me.m_lblLocation.Text = "Location:"
            '
            'm_plDescription
            '
            Me.m_plDescription.Controls.Add(Me.m_cmbVarName)
            Me.m_plDescription.Controls.Add(Me.m_hdrDescription)
            Me.m_plDescription.Controls.Add(Me.m_tbxName)
            Me.m_plDescription.Controls.Add(Me.m_lblVariable)
            Me.m_plDescription.Controls.Add(Me.m_lblDescription)
            Me.m_plDescription.Controls.Add(Me.m_lblName)
            Me.m_plDescription.Controls.Add(Me.m_tbxDescription)
            Me.m_plDescription.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_plDescription.Location = New System.Drawing.Point(3, 3)
            Me.m_plDescription.Name = "m_plDescription"
            Me.m_plDescription.Size = New System.Drawing.Size(444, 146)
            Me.m_plDescription.TabIndex = 0
            '
            'm_cmbVarName
            '
            Me.m_cmbVarName.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_cmbVarName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.m_cmbVarName.FormattingEnabled = True
            Me.m_cmbVarName.Location = New System.Drawing.Point(80, 125)
            Me.m_cmbVarName.Name = "m_cmbVarName"
            Me.m_cmbVarName.Size = New System.Drawing.Size(361, 21)
            Me.m_cmbVarName.Sorted = True
            Me.m_cmbVarName.TabIndex = 5
            '
            'm_hdrDescription
            '
            Me.m_hdrDescription.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_hdrDescription.CanCollapseParent = True
            Me.m_hdrDescription.CollapsedParentHeight = 94
            Me.m_hdrDescription.IsCollapsed = False
            Me.m_hdrDescription.Location = New System.Drawing.Point(0, 0)
            Me.m_hdrDescription.Name = "m_hdrDescription"
            Me.m_hdrDescription.Size = New System.Drawing.Size(441, 18)
            Me.m_hdrDescription.TabIndex = 0
            Me.m_hdrDescription.Text = "Description"
            Me.m_hdrDescription.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'm_lblVariable
            '
            Me.m_lblVariable.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
            Me.m_lblVariable.AutoSize = True
            Me.m_lblVariable.Location = New System.Drawing.Point(4, 128)
            Me.m_lblVariable.Name = "m_lblVariable"
            Me.m_lblVariable.Size = New System.Drawing.Size(48, 13)
            Me.m_lblVariable.TabIndex = 3
            Me.m_lblVariable.Text = "&Variable:"
            '
            'ucMultiFileDatasetConfigPage
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
            Me.BackColor = System.Drawing.SystemColors.Control
            Me.Controls.Add(Me.m_tlpConfig)
            Me.MinimumSize = New System.Drawing.Size(410, 400)
            Me.Name = "ucMultiFileDatasetConfigPage"
            Me.Size = New System.Drawing.Size(450, 520)
            CType(Me.m_dgvFiles, System.ComponentModel.ISupportInitialize).EndInit()
            Me.m_tlpConfig.ResumeLayout(False)
            Me.m_plTime.ResumeLayout(False)
            Me.m_plTime.PerformLayout()
            CType(Me.m_nudSpacing, System.ComponentModel.ISupportInitialize).EndInit()
            Me.m_plFiles.ResumeLayout(False)
            Me.m_plFiles.PerformLayout()
            Me.m_plDescription.ResumeLayout(False)
            Me.m_plDescription.PerformLayout()
            Me.ResumeLayout(False)

        End Sub
        Private WithEvents m_btnBrowse As System.Windows.Forms.Button
        Private WithEvents m_dgvFiles As System.Windows.Forms.DataGridView
        Private WithEvents m_lblName As System.Windows.Forms.Label
        Private WithEvents m_tbxName As System.Windows.Forms.TextBox
        Private WithEvents m_tbxDescription As System.Windows.Forms.TextBox
        Private WithEvents m_lblDescription As System.Windows.Forms.Label
        Private WithEvents CCalendarColumn1 As cCalendarColumn
        Private WithEvents DataGridViewTextBoxColumn1 As System.Windows.Forms.DataGridViewTextBoxColumn
        Private WithEvents m_tlpConfig As System.Windows.Forms.TableLayoutPanel
        Private WithEvents m_plTime As System.Windows.Forms.Panel
        Private WithEvents m_hdrTime As Controls.cEwEHeaderLabel
        Private WithEvents m_plFiles As System.Windows.Forms.Panel
        Private WithEvents m_plDescription As System.Windows.Forms.Panel
        Private WithEvents m_hdrDescription As Controls.cEwEHeaderLabel
        Private WithEvents m_tbxDatePart As System.Windows.Forms.TextBox
        Private WithEvents m_hdrFiles As Controls.cEwEHeaderLabel
        Private WithEvents m_mtbIntervalStart As System.Windows.Forms.MaskedTextBox
        Private WithEvents m_rbInterval As System.Windows.Forms.RadioButton
        Private WithEvents m_btnSetTime As System.Windows.Forms.Button
        Private WithEvents m_rbFromName As System.Windows.Forms.RadioButton
        Private WithEvents m_lblSpace1 As System.Windows.Forms.Label
        Private WithEvents m_cmbVarName As System.Windows.Forms.ComboBox
        Private WithEvents m_lblVariable As System.Windows.Forms.Label
        Private WithEvents m_cbSeasonal As System.Windows.Forms.CheckBox
        Private WithEvents m_mtbSeasonalEnd As System.Windows.Forms.MaskedTextBox
        Private WithEvents m_lblLocation As System.Windows.Forms.Label
        Private WithEvents m_lblLocationSample As System.Windows.Forms.Label
        Private WithEvents m_colError As System.Windows.Forms.DataGridViewImageColumn
        Private WithEvents m_colFileName As System.Windows.Forms.DataGridViewTextBoxColumn
        Private WithEvents m_colTime As EwESpatialAssetsPlugin.cCalendarColumn
        Friend WithEvents m_nudSpacing As System.Windows.Forms.NumericUpDown
        Private WithEvents m_lblSpace2 As System.Windows.Forms.Label

    End Class

End Namespace