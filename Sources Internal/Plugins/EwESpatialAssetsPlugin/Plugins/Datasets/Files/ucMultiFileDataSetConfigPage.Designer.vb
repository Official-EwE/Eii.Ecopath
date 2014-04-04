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
' Copyright 1991- UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'
Imports ScientificInterfaceShared

Namespace SpatialData

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class ucMultiFileDatasetConfigPage
        Inherits System.Windows.Forms.UserControl

        'Required by the Windows Form Designer
        Private components As System.ComponentModel.IContainer

        'NOTE: The following procedure is required by the Windows Form Designer
        'It can be modified using the Windows Form Designer.  
        'Do not modify it using the code editor.
        Private Sub InitializeComponent()
            Me.m_lblPath = New System.Windows.Forms.Label()
            Me.m_tbxPath = New System.Windows.Forms.TextBox()
            Me.m_btnBrowse = New System.Windows.Forms.Button()
            Me.m_dgvFiles = New System.Windows.Forms.DataGridView()
            Me.m_colFileName = New System.Windows.Forms.DataGridViewTextBoxColumn()
            Me.m_colTime = New EwESpatialAssetsPlugin.cCalendarColumn()
            Me.m_btnSearch = New System.Windows.Forms.Button()
            Me.m_lblName = New System.Windows.Forms.Label()
            Me.m_tbxName = New System.Windows.Forms.TextBox()
            Me.m_tbxDescription = New System.Windows.Forms.TextBox()
            Me.m_lblDescription = New System.Windows.Forms.Label()
            Me.CCalendarColumn1 = New EwESpatialAssetsPlugin.cCalendarColumn()
            Me.DataGridViewTextBoxColumn1 = New System.Windows.Forms.DataGridViewTextBoxColumn()
            Me.m_hdrFiles = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
            Me.m_tlpConfig = New System.Windows.Forms.TableLayoutPanel()
            Me.m_plTime = New System.Windows.Forms.Panel()
            Me.m_cbSeasonal = New System.Windows.Forms.CheckBox()
            Me.m_cmbInterval = New System.Windows.Forms.ComboBox()
            Me.m_lblIntervalWith = New System.Windows.Forms.Label()
            Me.m_mtbSeasonalEnd = New System.Windows.Forms.MaskedTextBox()
            Me.m_mtbIntervalStart = New System.Windows.Forms.MaskedTextBox()
            Me.m_rbFromName = New System.Windows.Forms.RadioButton()
            Me.m_rbFromDate = New System.Windows.Forms.RadioButton()
            Me.m_rbInterval = New System.Windows.Forms.RadioButton()
            Me.m_tbxDatePart = New System.Windows.Forms.TextBox()
            Me.m_hdrTime = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
            Me.m_btnSetTime = New System.Windows.Forms.Button()
            Me.m_plFiles = New System.Windows.Forms.Panel()
            Me.m_tbxFileNamePattern = New System.Windows.Forms.TextBox()
            Me.m_lblFileType = New System.Windows.Forms.Label()
            Me.m_cmbExtensions = New System.Windows.Forms.ComboBox()
            Me.m_plDescription = New System.Windows.Forms.Panel()
            Me.m_cmbVarName = New System.Windows.Forms.ComboBox()
            Me.m_hdrDescription = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
            Me.m_lblVariable = New System.Windows.Forms.Label()
            CType(Me.m_dgvFiles, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.m_tlpConfig.SuspendLayout()
            Me.m_plTime.SuspendLayout()
            Me.m_plFiles.SuspendLayout()
            Me.m_plDescription.SuspendLayout()
            Me.SuspendLayout()
            '
            'm_lblPath
            '
            Me.m_lblPath.AutoSize = True
            Me.m_lblPath.Location = New System.Drawing.Point(4, 28)
            Me.m_lblPath.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
            Me.m_lblPath.Name = "m_lblPath"
            Me.m_lblPath.Size = New System.Drawing.Size(52, 17)
            Me.m_lblPath.TabIndex = 0
            Me.m_lblPath.Text = "&Folder:"
            '
            'm_tbxPath
            '
            Me.m_tbxPath.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_tbxPath.Location = New System.Drawing.Point(107, 25)
            Me.m_tbxPath.Margin = New System.Windows.Forms.Padding(4)
            Me.m_tbxPath.Name = "m_tbxPath"
            Me.m_tbxPath.Size = New System.Drawing.Size(352, 22)
            Me.m_tbxPath.TabIndex = 1
            '
            'm_btnBrowse
            '
            Me.m_btnBrowse.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_btnBrowse.Location = New System.Drawing.Point(468, 22)
            Me.m_btnBrowse.Margin = New System.Windows.Forms.Padding(4)
            Me.m_btnBrowse.Name = "m_btnBrowse"
            Me.m_btnBrowse.Size = New System.Drawing.Size(120, 28)
            Me.m_btnBrowse.TabIndex = 2
            Me.m_btnBrowse.Text = "&Choose..."
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
            Me.m_dgvFiles.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.m_colFileName, Me.m_colTime})
            Me.m_dgvFiles.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter
            Me.m_dgvFiles.Location = New System.Drawing.Point(4, 95)
            Me.m_dgvFiles.Margin = New System.Windows.Forms.Padding(4)
            Me.m_dgvFiles.MultiSelect = False
            Me.m_dgvFiles.Name = "m_dgvFiles"
            Me.m_dgvFiles.RowHeadersVisible = False
            Me.m_dgvFiles.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing
            Me.m_dgvFiles.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
            Me.m_dgvFiles.ShowCellErrors = False
            Me.m_dgvFiles.ShowCellToolTips = False
            Me.m_dgvFiles.ShowEditingIcon = False
            Me.m_dgvFiles.ShowRowErrors = False
            Me.m_dgvFiles.Size = New System.Drawing.Size(584, 185)
            Me.m_dgvFiles.TabIndex = 9
            '
            'm_colFileName
            '
            Me.m_colFileName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
            Me.m_colFileName.HeaderText = "File"
            Me.m_colFileName.Name = "m_colFileName"
            Me.m_colFileName.ReadOnly = True
            Me.m_colFileName.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
            Me.m_colFileName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
            '
            'm_colTime
            '
            Me.m_colTime.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells
            Me.m_colTime.HeaderText = "Time"
            Me.m_colTime.MinimumWidth = 120
            Me.m_colTime.Name = "m_colTime"
            Me.m_colTime.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
            Me.m_colTime.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic
            Me.m_colTime.Width = 120
            '
            'm_btnSearch
            '
            Me.m_btnSearch.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_btnSearch.Location = New System.Drawing.Point(468, 54)
            Me.m_btnSearch.Margin = New System.Windows.Forms.Padding(4)
            Me.m_btnSearch.Name = "m_btnSearch"
            Me.m_btnSearch.Size = New System.Drawing.Size(120, 28)
            Me.m_btnSearch.TabIndex = 8
            Me.m_btnSearch.Text = "S&earch files"
            Me.m_btnSearch.UseVisualStyleBackColor = True
            '
            'm_lblName
            '
            Me.m_lblName.AutoSize = True
            Me.m_lblName.Location = New System.Drawing.Point(4, 28)
            Me.m_lblName.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
            Me.m_lblName.Name = "m_lblName"
            Me.m_lblName.Size = New System.Drawing.Size(49, 17)
            Me.m_lblName.TabIndex = 1
            Me.m_lblName.Text = "&Name:"
            '
            'm_tbxName
            '
            Me.m_tbxName.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_tbxName.Location = New System.Drawing.Point(107, 25)
            Me.m_tbxName.Margin = New System.Windows.Forms.Padding(4)
            Me.m_tbxName.MaxLength = 100
            Me.m_tbxName.Name = "m_tbxName"
            Me.m_tbxName.Size = New System.Drawing.Size(480, 22)
            Me.m_tbxName.TabIndex = 2
            '
            'm_tbxDescription
            '
            Me.m_tbxDescription.AcceptsReturn = True
            Me.m_tbxDescription.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_tbxDescription.Location = New System.Drawing.Point(107, 57)
            Me.m_tbxDescription.Margin = New System.Windows.Forms.Padding(4)
            Me.m_tbxDescription.Multiline = True
            Me.m_tbxDescription.Name = "m_tbxDescription"
            Me.m_tbxDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
            Me.m_tbxDescription.Size = New System.Drawing.Size(480, 89)
            Me.m_tbxDescription.TabIndex = 4
            '
            'm_lblDescription
            '
            Me.m_lblDescription.AutoSize = True
            Me.m_lblDescription.Location = New System.Drawing.Point(4, 60)
            Me.m_lblDescription.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
            Me.m_lblDescription.Name = "m_lblDescription"
            Me.m_lblDescription.Size = New System.Drawing.Size(83, 17)
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
            Me.m_hdrFiles.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
            Me.m_hdrFiles.Name = "m_hdrFiles"
            Me.m_hdrFiles.Size = New System.Drawing.Size(588, 22)
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
            Me.m_tlpConfig.Margin = New System.Windows.Forms.Padding(4)
            Me.m_tlpConfig.Name = "m_tlpConfig"
            Me.m_tlpConfig.RowCount = 3
            Me.m_tlpConfig.RowStyles.Add(New System.Windows.Forms.RowStyle())
            Me.m_tlpConfig.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
            Me.m_tlpConfig.RowStyles.Add(New System.Windows.Forms.RowStyle())
            Me.m_tlpConfig.Size = New System.Drawing.Size(600, 640)
            Me.m_tlpConfig.TabIndex = 0
            '
            'm_plTime
            '
            Me.m_plTime.Controls.Add(Me.m_cbSeasonal)
            Me.m_plTime.Controls.Add(Me.m_cmbInterval)
            Me.m_plTime.Controls.Add(Me.m_lblIntervalWith)
            Me.m_plTime.Controls.Add(Me.m_mtbSeasonalEnd)
            Me.m_plTime.Controls.Add(Me.m_mtbIntervalStart)
            Me.m_plTime.Controls.Add(Me.m_rbFromName)
            Me.m_plTime.Controls.Add(Me.m_rbFromDate)
            Me.m_plTime.Controls.Add(Me.m_rbInterval)
            Me.m_plTime.Controls.Add(Me.m_tbxDatePart)
            Me.m_plTime.Controls.Add(Me.m_hdrTime)
            Me.m_plTime.Controls.Add(Me.m_btnSetTime)
            Me.m_plTime.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_plTime.Location = New System.Drawing.Point(4, 483)
            Me.m_plTime.Margin = New System.Windows.Forms.Padding(4)
            Me.m_plTime.Name = "m_plTime"
            Me.m_plTime.Size = New System.Drawing.Size(592, 153)
            Me.m_plTime.TabIndex = 0
            '
            'm_cbSeasonal
            '
            Me.m_cbSeasonal.AutoSize = True
            Me.m_cbSeasonal.Location = New System.Drawing.Point(9, 31)
            Me.m_cbSeasonal.Margin = New System.Windows.Forms.Padding(4)
            Me.m_cbSeasonal.Name = "m_cbSeasonal"
            Me.m_cbSeasonal.Size = New System.Drawing.Size(165, 21)
            Me.m_cbSeasonal.TabIndex = 11
            Me.m_cbSeasonal.Text = "&Data is seasonal until"
            Me.m_cbSeasonal.UseVisualStyleBackColor = True
            '
            'm_cmbInterval
            '
            Me.m_cmbInterval.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.m_cmbInterval.FormattingEnabled = True
            Me.m_cmbInterval.Items.AddRange(New Object() {"month", "3 months", "6 months", "year", "decade"})
            Me.m_cmbInterval.Location = New System.Drawing.Point(341, 64)
            Me.m_cmbInterval.Margin = New System.Windows.Forms.Padding(4)
            Me.m_cmbInterval.Name = "m_cmbInterval"
            Me.m_cmbInterval.Size = New System.Drawing.Size(115, 24)
            Me.m_cmbInterval.TabIndex = 4
            '
            'm_lblIntervalWith
            '
            Me.m_lblIntervalWith.AutoSize = True
            Me.m_lblIntervalWith.Location = New System.Drawing.Point(185, 68)
            Me.m_lblIntervalWith.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
            Me.m_lblIntervalWith.Name = "m_lblIntervalWith"
            Me.m_lblIntervalWith.Size = New System.Drawing.Size(146, 17)
            Me.m_lblIntervalWith.TabIndex = 3
            Me.m_lblIntervalWith.Text = " spaced evenly every "
            '
            'm_mtbSeasonalEnd
            '
            Me.m_mtbSeasonalEnd.Location = New System.Drawing.Point(185, 28)
            Me.m_mtbSeasonalEnd.Margin = New System.Windows.Forms.Padding(4)
            Me.m_mtbSeasonalEnd.Mask = "0000/00"
            Me.m_mtbSeasonalEnd.Name = "m_mtbSeasonalEnd"
            Me.m_mtbSeasonalEnd.Size = New System.Drawing.Size(68, 22)
            Me.m_mtbSeasonalEnd.TabIndex = 2
            Me.m_mtbSeasonalEnd.Text = "195001"
            '
            'm_mtbIntervalStart
            '
            Me.m_mtbIntervalStart.Location = New System.Drawing.Point(108, 64)
            Me.m_mtbIntervalStart.Margin = New System.Windows.Forms.Padding(4)
            Me.m_mtbIntervalStart.Mask = "0000/00"
            Me.m_mtbIntervalStart.Name = "m_mtbIntervalStart"
            Me.m_mtbIntervalStart.Size = New System.Drawing.Size(68, 22)
            Me.m_mtbIntervalStart.TabIndex = 2
            Me.m_mtbIntervalStart.Text = "195001"
            '
            'm_rbFromName
            '
            Me.m_rbFromName.AutoSize = True
            Me.m_rbFromName.Location = New System.Drawing.Point(9, 122)
            Me.m_rbFromName.Margin = New System.Windows.Forms.Padding(4)
            Me.m_rbFromName.Name = "m_rbFromName"
            Me.m_rbFromName.Size = New System.Drawing.Size(245, 21)
            Me.m_rbFromName.TabIndex = 7
            Me.m_rbFromName.TabStop = True
            Me.m_rbFromName.Text = "From file &name (select which part):"
            Me.m_rbFromName.UseVisualStyleBackColor = True
            '
            'm_rbFromDate
            '
            Me.m_rbFromDate.AutoSize = True
            Me.m_rbFromDate.Location = New System.Drawing.Point(9, 94)
            Me.m_rbFromDate.Margin = New System.Windows.Forms.Padding(4)
            Me.m_rbFromDate.Name = "m_rbFromDate"
            Me.m_rbFromDate.Size = New System.Drawing.Size(115, 21)
            Me.m_rbFromDate.TabIndex = 6
            Me.m_rbFromDate.TabStop = True
            Me.m_rbFromDate.Text = "From file &date"
            Me.m_rbFromDate.UseVisualStyleBackColor = True
            '
            'm_rbInterval
            '
            Me.m_rbInterval.AutoSize = True
            Me.m_rbInterval.Checked = True
            Me.m_rbInterval.Location = New System.Drawing.Point(9, 65)
            Me.m_rbInterval.Margin = New System.Windows.Forms.Padding(4)
            Me.m_rbInterval.Name = "m_rbInterval"
            Me.m_rbInterval.Size = New System.Drawing.Size(94, 21)
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
            Me.m_tbxDatePart.Location = New System.Drawing.Point(264, 122)
            Me.m_tbxDatePart.Margin = New System.Windows.Forms.Padding(4)
            Me.m_tbxDatePart.Name = "m_tbxDatePart"
            Me.m_tbxDatePart.ReadOnly = True
            Me.m_tbxDatePart.Size = New System.Drawing.Size(169, 22)
            Me.m_tbxDatePart.TabIndex = 8
            '
            'm_hdrTime
            '
            Me.m_hdrTime.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_hdrTime.CanCollapseParent = True
            Me.m_hdrTime.CollapsedParentHeight = 76
            Me.m_hdrTime.IsCollapsed = False
            Me.m_hdrTime.Location = New System.Drawing.Point(0, 0)
            Me.m_hdrTime.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
            Me.m_hdrTime.Name = "m_hdrTime"
            Me.m_hdrTime.Size = New System.Drawing.Size(588, 22)
            Me.m_hdrTime.TabIndex = 0
            Me.m_hdrTime.Text = "Time utility"
            Me.m_hdrTime.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'm_btnSetTime
            '
            Me.m_btnSetTime.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_btnSetTime.Location = New System.Drawing.Point(468, 62)
            Me.m_btnSetTime.Margin = New System.Windows.Forms.Padding(4)
            Me.m_btnSetTime.Name = "m_btnSetTime"
            Me.m_btnSetTime.Size = New System.Drawing.Size(120, 28)
            Me.m_btnSetTime.TabIndex = 10
            Me.m_btnSetTime.Text = "Set file &times"
            Me.m_btnSetTime.UseVisualStyleBackColor = True
            '
            'm_plFiles
            '
            Me.m_plFiles.Controls.Add(Me.m_tbxFileNamePattern)
            Me.m_plFiles.Controls.Add(Me.m_lblFileType)
            Me.m_plFiles.Controls.Add(Me.m_cmbExtensions)
            Me.m_plFiles.Controls.Add(Me.m_dgvFiles)
            Me.m_plFiles.Controls.Add(Me.m_hdrFiles)
            Me.m_plFiles.Controls.Add(Me.m_tbxPath)
            Me.m_plFiles.Controls.Add(Me.m_lblPath)
            Me.m_plFiles.Controls.Add(Me.m_btnBrowse)
            Me.m_plFiles.Controls.Add(Me.m_btnSearch)
            Me.m_plFiles.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_plFiles.Location = New System.Drawing.Point(4, 192)
            Me.m_plFiles.Margin = New System.Windows.Forms.Padding(4)
            Me.m_plFiles.Name = "m_plFiles"
            Me.m_plFiles.Size = New System.Drawing.Size(592, 283)
            Me.m_plFiles.TabIndex = 1
            '
            'm_tbxFileNamePattern
            '
            Me.m_tbxFileNamePattern.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_tbxFileNamePattern.Location = New System.Drawing.Point(108, 57)
            Me.m_tbxFileNamePattern.Margin = New System.Windows.Forms.Padding(4)
            Me.m_tbxFileNamePattern.Name = "m_tbxFileNamePattern"
            Me.m_tbxFileNamePattern.Size = New System.Drawing.Size(97, 22)
            Me.m_tbxFileNamePattern.TabIndex = 7
            Me.m_tbxFileNamePattern.Text = "*"
            '
            'm_lblFileType
            '
            Me.m_lblFileType.AutoSize = True
            Me.m_lblFileType.Location = New System.Drawing.Point(4, 60)
            Me.m_lblFileType.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
            Me.m_lblFileType.Name = "m_lblFileType"
            Me.m_lblFileType.Size = New System.Drawing.Size(65, 17)
            Me.m_lblFileType.TabIndex = 3
            Me.m_lblFileType.Text = "&File filter:"
            '
            'm_cmbExtensions
            '
            Me.m_cmbExtensions.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_cmbExtensions.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.m_cmbExtensions.FormattingEnabled = True
            Me.m_cmbExtensions.Location = New System.Drawing.Point(251, 57)
            Me.m_cmbExtensions.Margin = New System.Windows.Forms.Padding(4)
            Me.m_cmbExtensions.Name = "m_cmbExtensions"
            Me.m_cmbExtensions.Size = New System.Drawing.Size(208, 24)
            Me.m_cmbExtensions.TabIndex = 4
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
            Me.m_plDescription.Location = New System.Drawing.Point(4, 4)
            Me.m_plDescription.Margin = New System.Windows.Forms.Padding(4)
            Me.m_plDescription.Name = "m_plDescription"
            Me.m_plDescription.Size = New System.Drawing.Size(592, 180)
            Me.m_plDescription.TabIndex = 0
            '
            'm_cmbVarName
            '
            Me.m_cmbVarName.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_cmbVarName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.m_cmbVarName.FormattingEnabled = True
            Me.m_cmbVarName.Location = New System.Drawing.Point(107, 154)
            Me.m_cmbVarName.Margin = New System.Windows.Forms.Padding(4)
            Me.m_cmbVarName.Name = "m_cmbVarName"
            Me.m_cmbVarName.Size = New System.Drawing.Size(480, 24)
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
            Me.m_hdrDescription.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
            Me.m_hdrDescription.Name = "m_hdrDescription"
            Me.m_hdrDescription.Size = New System.Drawing.Size(588, 22)
            Me.m_hdrDescription.TabIndex = 0
            Me.m_hdrDescription.Text = "Description"
            Me.m_hdrDescription.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'm_lblVariable
            '
            Me.m_lblVariable.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
            Me.m_lblVariable.AutoSize = True
            Me.m_lblVariable.Location = New System.Drawing.Point(5, 158)
            Me.m_lblVariable.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
            Me.m_lblVariable.Name = "m_lblVariable"
            Me.m_lblVariable.Size = New System.Drawing.Size(64, 17)
            Me.m_lblVariable.TabIndex = 3
            Me.m_lblVariable.Text = "&Variable:"
            '
            'ucMultiFileDatasetConfigPage
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
            Me.BackColor = System.Drawing.SystemColors.Control
            Me.Controls.Add(Me.m_tlpConfig)
            Me.Margin = New System.Windows.Forms.Padding(4)
            Me.MinimumSize = New System.Drawing.Size(547, 492)
            Me.Name = "ucMultiFileDatasetConfigPage"
            Me.Size = New System.Drawing.Size(600, 640)
            CType(Me.m_dgvFiles, System.ComponentModel.ISupportInitialize).EndInit()
            Me.m_tlpConfig.ResumeLayout(False)
            Me.m_plTime.ResumeLayout(False)
            Me.m_plTime.PerformLayout()
            Me.m_plFiles.ResumeLayout(False)
            Me.m_plFiles.PerformLayout()
            Me.m_plDescription.ResumeLayout(False)
            Me.m_plDescription.PerformLayout()
            Me.ResumeLayout(False)

        End Sub
        Private WithEvents m_lblPath As System.Windows.Forms.Label
        Private WithEvents m_tbxPath As System.Windows.Forms.TextBox
        Private WithEvents m_btnBrowse As System.Windows.Forms.Button
        Private WithEvents m_dgvFiles As System.Windows.Forms.DataGridView
        Private WithEvents m_btnSearch As System.Windows.Forms.Button
        Private WithEvents m_lblName As System.Windows.Forms.Label
        Private WithEvents m_tbxName As System.Windows.Forms.TextBox
        Private WithEvents m_tbxDescription As System.Windows.Forms.TextBox
        Private WithEvents m_lblDescription As System.Windows.Forms.Label
        Private WithEvents CCalendarColumn1 As cCalendarColumn
        Private WithEvents DataGridViewTextBoxColumn1 As System.Windows.Forms.DataGridViewTextBoxColumn
        Private WithEvents m_colFileName As System.Windows.Forms.DataGridViewTextBoxColumn
        Private WithEvents m_colTime As cCalendarColumn
        Private WithEvents m_tlpConfig As System.Windows.Forms.TableLayoutPanel
        Private WithEvents m_plTime As System.Windows.Forms.Panel
        Private WithEvents m_hdrTime As Controls.cEwEHeaderLabel
        Private WithEvents m_plFiles As System.Windows.Forms.Panel
        Private WithEvents m_plDescription As System.Windows.Forms.Panel
        Private WithEvents m_hdrDescription As Controls.cEwEHeaderLabel
        Private WithEvents m_tbxDatePart As System.Windows.Forms.TextBox
        Private WithEvents m_hdrFiles As Controls.cEwEHeaderLabel
        Private WithEvents m_cmbInterval As System.Windows.Forms.ComboBox
        Private WithEvents m_mtbIntervalStart As System.Windows.Forms.MaskedTextBox
        Private WithEvents m_rbInterval As System.Windows.Forms.RadioButton
        Private WithEvents m_btnSetTime As System.Windows.Forms.Button
        Private WithEvents m_rbFromDate As System.Windows.Forms.RadioButton
        Private WithEvents m_rbFromName As System.Windows.Forms.RadioButton
        Private WithEvents m_cmbExtensions As System.Windows.Forms.ComboBox
        Private WithEvents m_tbxFileNamePattern As System.Windows.Forms.TextBox
        Private WithEvents m_lblFileType As System.Windows.Forms.Label
        Private WithEvents m_lblIntervalWith As System.Windows.Forms.Label
        Private WithEvents m_cmbVarName As System.Windows.Forms.ComboBox
        Private WithEvents m_lblVariable As System.Windows.Forms.Label
        Private WithEvents m_cbSeasonal As System.Windows.Forms.CheckBox
        Private WithEvents m_mtbSeasonalEnd As System.Windows.Forms.MaskedTextBox

    End Class

End Namespace