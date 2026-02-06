Imports System.Windows.Forms
Imports ScientificInterfaceShared.Forms

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmSURIMIChecker
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmSURIMIChecker))
        Me.OK_Button = New System.Windows.Forms.Button()
        Me.Cancel_Button = New System.Windows.Forms.Button()
        Me.m_tcMain = New System.Windows.Forms.TabControl()
        Me.m_tabSpecies = New System.Windows.Forms.TabPage()
        Me.m_tlpSpecies = New System.Windows.Forms.TableLayoutPanel()
        Me.m_tsSpecies = New System.Windows.Forms.ToolStrip()
        Me.m_tslSpeciesVocab = New System.Windows.Forms.ToolStripLabel()
        Me.m_tscmbSpeciesVoc = New System.Windows.Forms.ToolStripComboBox()
        Me.m_tabFleets = New System.Windows.Forms.TabPage()
        Me.m_dgvSpecies = New System.Windows.Forms.DataGridView()
        Me.m_colFG = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.m_colTaxon = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.m_colScName = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.m_colFAO = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.m_colMLK = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.m_tslLifestageVoc = New System.Windows.Forms.ToolStripLabel()
        Me.m_tscmbLifestageVoc = New System.Windows.Forms.ToolStripComboBox()
        Me.m_tsbnCalculateSpeciues = New System.Windows.Forms.ToolStripButton()
        Me.m_tcMain.SuspendLayout()
        Me.m_tabSpecies.SuspendLayout()
        Me.m_tlpSpecies.SuspendLayout()
        Me.m_tsSpecies.SuspendLayout()
        CType(Me.m_dgvSpecies, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'OK_Button
        '
        Me.OK_Button.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.OK_Button.Location = New System.Drawing.Point(3, 3)
        Me.OK_Button.Name = "OK_Button"
        Me.OK_Button.Size = New System.Drawing.Size(67, 23)
        Me.OK_Button.TabIndex = 0
        Me.OK_Button.Text = "OK"
        '
        'Cancel_Button
        '
        Me.Cancel_Button.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.Cancel_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Cancel_Button.Location = New System.Drawing.Point(76, 3)
        Me.Cancel_Button.Name = "Cancel_Button"
        Me.Cancel_Button.Size = New System.Drawing.Size(67, 23)
        Me.Cancel_Button.TabIndex = 1
        Me.Cancel_Button.Text = "Cancel"
        '
        'm_tcMain
        '
        Me.m_tcMain.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_tcMain.Controls.Add(Me.m_tabSpecies)
        Me.m_tcMain.Controls.Add(Me.m_tabFleets)
        Me.m_tcMain.Location = New System.Drawing.Point(12, 12)
        Me.m_tcMain.Name = "m_tcMain"
        Me.m_tcMain.SelectedIndex = 0
        Me.m_tcMain.Size = New System.Drawing.Size(829, 342)
        Me.m_tcMain.TabIndex = 1
        '
        'm_tabSpecies
        '
        Me.m_tabSpecies.Controls.Add(Me.m_tlpSpecies)
        Me.m_tabSpecies.Location = New System.Drawing.Point(4, 22)
        Me.m_tabSpecies.Name = "m_tabSpecies"
        Me.m_tabSpecies.Padding = New System.Windows.Forms.Padding(3)
        Me.m_tabSpecies.Size = New System.Drawing.Size(821, 316)
        Me.m_tabSpecies.TabIndex = 0
        Me.m_tabSpecies.Text = "Species"
        Me.m_tabSpecies.UseVisualStyleBackColor = True
        '
        'm_tlpSpecies
        '
        Me.m_tlpSpecies.ColumnCount = 1
        Me.m_tlpSpecies.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.m_tlpSpecies.Controls.Add(Me.m_tsSpecies, 0, 0)
        Me.m_tlpSpecies.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_tlpSpecies.Location = New System.Drawing.Point(3, 3)
        Me.m_tlpSpecies.Name = "m_tlpSpecies"
        Me.m_tlpSpecies.RowCount = 2
        Me.m_tlpSpecies.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.m_tlpSpecies.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.m_tlpSpecies.Size = New System.Drawing.Size(815, 310)
        Me.m_tlpSpecies.TabIndex = 0
        '
        'm_tsSpecies
        '
        Me.m_tsSpecies.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tslSpeciesVocab, Me.m_tscmbSpeciesVoc, Me.m_tslLifestageVoc, Me.m_tscmbLifestageVoc, Me.m_tsbnCalculateSpeciues})
        Me.m_tsSpecies.Location = New System.Drawing.Point(0, 0)
        Me.m_tsSpecies.Name = "m_tsSpecies"
        Me.m_tsSpecies.Size = New System.Drawing.Size(815, 25)
        Me.m_tsSpecies.TabIndex = 0
        Me.m_tsSpecies.Text = ""
        '
        'm_tslSpeciesVocab
        '
        Me.m_tslSpeciesVocab.Name = "m_tslSpeciesVocab"
        Me.m_tslSpeciesVocab.Size = New System.Drawing.Size(71, 22)
        Me.m_tslSpeciesVocab.Text = "Species voc:"
        '
        'm_tscmbSpeciesVoc
        '
        Me.m_tscmbSpeciesVoc.Name = "m_tscmbSpeciesVoc"
        Me.m_tscmbSpeciesVoc.Size = New System.Drawing.Size(121, 25)
        '
        'm_tabFleets
        '
        Me.m_tabFleets.Location = New System.Drawing.Point(4, 22)
        Me.m_tabFleets.Name = "m_tabFleets"
        Me.m_tabFleets.Padding = New System.Windows.Forms.Padding(3)
        Me.m_tabFleets.Size = New System.Drawing.Size(821, 316)
        Me.m_tabFleets.TabIndex = 1
        Me.m_tabFleets.Text = "Fleets"
        Me.m_tabFleets.UseVisualStyleBackColor = True
        '
        'm_dgvSpecies
        '
        Me.m_dgvSpecies.AllowUserToAddRows = False
        Me.m_dgvSpecies.AllowUserToDeleteRows = False
        Me.m_dgvSpecies.AllowUserToOrderColumns = True
        Me.m_dgvSpecies.AllowUserToResizeRows = False
        Me.m_dgvSpecies.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.m_dgvSpecies.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.m_colFG, Me.m_colTaxon, Me.m_colScName, Me.m_colFAO, Me.m_colMLK})
        Me.m_dgvSpecies.Location = New System.Drawing.Point(39, 360)
        Me.m_dgvSpecies.Margin = New System.Windows.Forms.Padding(0)
        Me.m_dgvSpecies.Name = "m_dgvSpecies"
        Me.m_dgvSpecies.RowHeadersVisible = False
        Me.m_dgvSpecies.ShowCellErrors = False
        Me.m_dgvSpecies.ShowEditingIcon = False
        Me.m_dgvSpecies.ShowRowErrors = False
        Me.m_dgvSpecies.Size = New System.Drawing.Size(357, 137)
        Me.m_dgvSpecies.TabIndex = 0
        Me.m_dgvSpecies.Dock = DockStyle.Fill
        '
        'm_colFG
        '
        Me.m_colFG.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells
        Me.m_colFG.HeaderText = "FG"
        Me.m_colFG.Name = "m_colFG"
        Me.m_colFG.ReadOnly = True
        Me.m_colFG.Width = 46
        '
        'm_colTaxon
        '
        Me.m_colTaxon.HeaderText = "Taxon"
        Me.m_colTaxon.Name = "m_colTaxon"
        Me.m_colTaxon.ReadOnly = True
        '
        'm_colScName
        '
        Me.m_colScName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader
        Me.m_colScName.HeaderText = "Scientific name"
        Me.m_colScName.Name = "m_colScName"
        Me.m_colScName.ReadOnly = True
        Me.m_colScName.Width = 96
        '
        'm_colFAO
        '
        Me.m_colFAO.HeaderText = "ASFIS code"
        Me.m_colFAO.Name = "m_colFAO"
        '
        'm_colMLK
        '
        Me.m_colMLK.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        Me.m_colMLK.HeaderText = "MultiLevel Key"
        Me.m_colMLK.Name = "m_colMLK"
        Me.m_colMLK.ReadOnly = True
        '
        'm_tslLifestageVoc
        '
        Me.m_tslLifestageVoc.Name = "m_tslLifestageVoc"
        Me.m_tslLifestageVoc.Size = New System.Drawing.Size(79, 22)
        Me.m_tslLifestageVoc.Text = "Lifestage voc:"
        '
        'm_tscmbLifestageVoc
        '
        Me.m_tscmbLifestageVoc.Name = "m_tscmbLifestageVoc"
        Me.m_tscmbLifestageVoc.Size = New System.Drawing.Size(121, 25)
        '
        'm_tsbnCalculateSpeciues
        '
        Me.m_tsbnCalculateSpeciues.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.m_tsbnCalculateSpeciues.Image = CType(resources.GetObject("m_tsbnCalculateSpeciues.Image"), System.Drawing.Image)
        Me.m_tsbnCalculateSpeciues.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.m_tsbnCalculateSpeciues.Name = "m_tsbnCalculateSpeciues"
        Me.m_tsbnCalculateSpeciues.Size = New System.Drawing.Size(60, 22)
        Me.m_tsbnCalculateSpeciues.Text = "Calculate"
        '
        'frmRun
        '
        Me.AcceptButton = Me.OK_Button
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.Cancel_Button
        Me.ClientSize = New System.Drawing.Size(853, 398)
        Me.Controls.Add(Me.m_dgvSpecies)
        Me.Controls.Add(Me.m_tcMain)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmRun"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "SURIMI config checker"
        Me.m_tcMain.ResumeLayout(False)
        Me.m_tabSpecies.ResumeLayout(False)
        Me.m_tlpSpecies.ResumeLayout(False)
        Me.m_tlpSpecies.PerformLayout()
        Me.m_tsSpecies.ResumeLayout(False)
        Me.m_tsSpecies.PerformLayout()
        CType(Me.m_dgvSpecies, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Private WithEvents OK_Button As System.Windows.Forms.Button
    Private WithEvents Cancel_Button As System.Windows.Forms.Button
    Private WithEvents m_tcMain As Windows.Forms.TabControl
    Private WithEvents m_tabSpecies As Windows.Forms.TabPage
    Private WithEvents m_tabFleets As Windows.Forms.TabPage
    Private WithEvents m_dgvSpecies As Windows.Forms.DataGridView
    Private WithEvents m_colFG As Windows.Forms.DataGridViewTextBoxColumn
    Private WithEvents m_colTaxon As Windows.Forms.DataGridViewTextBoxColumn
    Private WithEvents m_colScName As Windows.Forms.DataGridViewTextBoxColumn
    Private WithEvents m_colFAO As Windows.Forms.DataGridViewTextBoxColumn
    Private WithEvents m_colMLK As Windows.Forms.DataGridViewTextBoxColumn
    Private WithEvents m_tlpSpecies As Windows.Forms.TableLayoutPanel
    Private WithEvents m_tsSpecies As Windows.Forms.ToolStrip
    Private WithEvents m_tslSpeciesVocab As Windows.Forms.ToolStripLabel
    Private WithEvents m_tscmbSpeciesVoc As Windows.Forms.ToolStripComboBox
    Private WithEvents m_tslLifestageVoc As Windows.Forms.ToolStripLabel
    Private WithEvents m_tscmbLifestageVoc As Windows.Forms.ToolStripComboBox
    Private WithEvents m_tsbnCalculateSpeciues As Windows.Forms.ToolStripButton
End Class
