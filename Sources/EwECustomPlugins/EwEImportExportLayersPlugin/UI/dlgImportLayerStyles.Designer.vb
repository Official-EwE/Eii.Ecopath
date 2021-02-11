' ===============================================================================
' This file is part of the Safenet toolkit, contributed to Ecopath with Ecosim
' as part of Safenet project.
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
' Safenet Copyright 2017-, EwE copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

Imports ScientificInterfaceShared.Controls

Partial Class dlgImportLayerStyles
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
        Me.m_cbCreateMissingLayers = New System.Windows.Forms.CheckBox()
        Me.m_btnCancel = New System.Windows.Forms.Button()
        Me.m_btnOK = New System.Windows.Forms.Button()
        Me.m_lblPrompt = New System.Windows.Forms.Label()
        Me.m_dgLayers = New System.Windows.Forms.DataGridView()
        Me.m_colUsed = New System.Windows.Forms.DataGridViewCheckBoxColumn()
        Me.m_colIndex = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.m_colName = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.m_colStatus = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.m_pbCredits = New System.Windows.Forms.PictureBox()
        Me.m_hdrCredits = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        CType(Me.m_dgLayers, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.m_pbCredits, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'm_cbCreateMissingLayers
        '
        Me.m_cbCreateMissingLayers.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.m_cbCreateMissingLayers.AutoSize = True
        Me.m_cbCreateMissingLayers.Location = New System.Drawing.Point(12, 354)
        Me.m_cbCreateMissingLayers.Name = "m_cbCreateMissingLayers"
        Me.m_cbCreateMissingLayers.Size = New System.Drawing.Size(124, 17)
        Me.m_cbCreateMissingLayers.TabIndex = 1
        Me.m_cbCreateMissingLayers.Text = "&Create missing layers"
        Me.m_cbCreateMissingLayers.UseVisualStyleBackColor = True
        '
        'm_btnCancel
        '
        Me.m_btnCancel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_btnCancel.Location = New System.Drawing.Point(516, 350)
        Me.m_btnCancel.Name = "m_btnCancel"
        Me.m_btnCancel.Size = New System.Drawing.Size(75, 23)
        Me.m_btnCancel.TabIndex = 2
        Me.m_btnCancel.Text = "Cancel"
        Me.m_btnCancel.UseVisualStyleBackColor = True
        '
        'm_btnOK
        '
        Me.m_btnOK.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_btnOK.Location = New System.Drawing.Point(435, 350)
        Me.m_btnOK.Name = "m_btnOK"
        Me.m_btnOK.Size = New System.Drawing.Size(75, 23)
        Me.m_btnOK.TabIndex = 3
        Me.m_btnOK.Text = "OK"
        Me.m_btnOK.UseVisualStyleBackColor = True
        '
        'm_lblPrompt
        '
        Me.m_lblPrompt.AutoSize = True
        Me.m_lblPrompt.Location = New System.Drawing.Point(12, 9)
        Me.m_lblPrompt.Name = "m_lblPrompt"
        Me.m_lblPrompt.Size = New System.Drawing.Size(162, 13)
        Me.m_lblPrompt.TabIndex = 4
        Me.m_lblPrompt.Text = "&Select which layer styles to apply"
        '
        'm_dgLayers
        '
        Me.m_dgLayers.AllowUserToAddRows = False
        Me.m_dgLayers.AllowUserToDeleteRows = False
        Me.m_dgLayers.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_dgLayers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.m_dgLayers.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.m_colUsed, Me.m_colIndex, Me.m_colName, Me.m_colStatus})
        Me.m_dgLayers.Location = New System.Drawing.Point(12, 25)
        Me.m_dgLayers.MultiSelect = False
        Me.m_dgLayers.Name = "m_dgLayers"
        Me.m_dgLayers.RowHeadersVisible = False
        Me.m_dgLayers.ShowCellErrors = False
        Me.m_dgLayers.ShowCellToolTips = False
        Me.m_dgLayers.ShowEditingIcon = False
        Me.m_dgLayers.ShowRowErrors = False
        Me.m_dgLayers.Size = New System.Drawing.Size(579, 319)
        Me.m_dgLayers.TabIndex = 5
        '
        'm_colUsed
        '
        Me.m_colUsed.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader
        Me.m_colUsed.HeaderText = "Import"
        Me.m_colUsed.Name = "m_colUsed"
        Me.m_colUsed.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.m_colUsed.ThreeState = True
        Me.m_colUsed.Width = 42
        '
        'm_colIndex
        '
        Me.m_colIndex.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader
        Me.m_colIndex.HeaderText = "#"
        Me.m_colIndex.Name = "m_colIndex"
        Me.m_colIndex.ReadOnly = True
        Me.m_colIndex.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.m_colIndex.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.m_colIndex.Width = 20
        '
        'm_colName
        '
        Me.m_colName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        Me.m_colName.HeaderText = "Name"
        Me.m_colName.Name = "m_colName"
        Me.m_colName.ReadOnly = True
        Me.m_colName.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.m_colName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        '
        'm_colStatus
        '
        Me.m_colStatus.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None
        Me.m_colStatus.HeaderText = "Status"
        Me.m_colStatus.Name = "m_colStatus"
        Me.m_colStatus.ReadOnly = True
        Me.m_colStatus.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.m_colStatus.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.m_colStatus.Width = 120
        '
        'm_pbCredits
        '
        Me.m_pbCredits.BackColor = System.Drawing.Color.White
        Me.m_pbCredits.BackgroundImage = Global.EwEImportExportLayerDefinitionsPlugin.My.Resources.Resources.safenet_full
        Me.m_pbCredits.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.m_pbCredits.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.m_pbCredits.Location = New System.Drawing.Point(0, 406)
        Me.m_pbCredits.Name = "m_pbCredits"
        Me.m_pbCredits.Size = New System.Drawing.Size(603, 100)
        Me.m_pbCredits.TabIndex = 6
        Me.m_pbCredits.TabStop = False
        '
        'm_hdrCredits
        '
        Me.m_hdrCredits.CanCollapseParent = False
        Me.m_hdrCredits.CollapsedParentHeight = 0
        Me.m_hdrCredits.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.m_hdrCredits.IsCollapsed = False
        Me.m_hdrCredits.Location = New System.Drawing.Point(0, 388)
        Me.m_hdrCredits.Name = "m_hdrCredits"
        Me.m_hdrCredits.Size = New System.Drawing.Size(603, 18)
        Me.m_hdrCredits.TabIndex = 7
        Me.m_hdrCredits.Text = "Credits"
        Me.m_hdrCredits.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'dlgImportLayerStyles
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.ClientSize = New System.Drawing.Size(603, 506)
        Me.ControlBox = False
        Me.Controls.Add(Me.m_hdrCredits)
        Me.Controls.Add(Me.m_pbCredits)
        Me.Controls.Add(Me.m_dgLayers)
        Me.Controls.Add(Me.m_lblPrompt)
        Me.Controls.Add(Me.m_btnOK)
        Me.Controls.Add(Me.m_btnCancel)
        Me.Controls.Add(Me.m_cbCreateMissingLayers)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(200, 200)
        Me.Name = "dlgImportLayerStyles"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.Text = "Apply imported layer styles"
        CType(Me.m_dgLayers, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.m_pbCredits, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Private WithEvents m_cbCreateMissingLayers As System.Windows.Forms.CheckBox
    Private WithEvents m_btnCancel As System.Windows.Forms.Button
    Private WithEvents m_btnOK As System.Windows.Forms.Button
    Private WithEvents m_lblPrompt As System.Windows.Forms.Label
    Private WithEvents m_dgLayers As System.Windows.Forms.DataGridView
    Private WithEvents m_colUsed As System.Windows.Forms.DataGridViewCheckBoxColumn
    Private WithEvents m_colIndex As System.Windows.Forms.DataGridViewTextBoxColumn
    Private WithEvents m_colName As System.Windows.Forms.DataGridViewTextBoxColumn
    Private WithEvents m_colStatus As System.Windows.Forms.DataGridViewTextBoxColumn
    Private WithEvents m_pbCredits As System.Windows.Forms.PictureBox
    Private WithEvents m_hdrCredits As cEwEHeaderLabel
End Class
