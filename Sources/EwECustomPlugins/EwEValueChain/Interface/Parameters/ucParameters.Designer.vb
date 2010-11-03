Imports ScientificInterfaceShared.Controls

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ucParameters
    Inherits System.Windows.Forms.UserControl

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.m_tlpSponsors = New System.Windows.Forms.TableLayoutPanel
        Me.m_pbLenfest = New System.Windows.Forms.PictureBox
        Me.m_pbSAUP = New System.Windows.Forms.PictureBox
        Me.m_pbEU = New System.Windows.Forms.PictureBox
        Me.m_lblSponsors = New ScientificInterfaceShared.Controls.cEwEHeaderLabel
        Me.m_lblBaseYear = New System.Windows.Forms.Label
        Me.m_nudBaseYear = New System.Windows.Forms.NumericUpDown
        Me.m_hdrEcosimSettings = New cEwEHeaderLabel
        Me.m_hdrIntegration = New cEwEHeaderLabel
        Me.m_chkRunWithEcopath = New System.Windows.Forms.CheckBox
        Me.m_chkRunWithEcosim = New System.Windows.Forms.CheckBox
        Me.m_chkRunWithSearches = New System.Windows.Forms.CheckBox
        Me.m_hdrEQ = New cEwEHeaderLabel
        Me.m_clbFleets = New System.Windows.Forms.CheckedListBox
        Me.m_lblFleets = New System.Windows.Forms.Label
        Me.m_lblEffortMin = New System.Windows.Forms.Label
        Me.m_lblEffortMax = New System.Windows.Forms.Label
        Me.m_lbEffortIncr = New System.Windows.Forms.Label
        Me.m_nudEffortMin = New System.Windows.Forms.NumericUpDown
        Me.m_nudEffortMax = New System.Windows.Forms.NumericUpDown
        Me.m_nudEffortIncr = New System.Windows.Forms.NumericUpDown
        Me.m_chkResultsByFleet = New System.Windows.Forms.CheckBox
        Me.m_tlpSponsors.SuspendLayout()
        CType(Me.m_pbLenfest, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.m_pbSAUP, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.m_pbEU, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.m_nudBaseYear, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.m_nudEffortMin, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.m_nudEffortMax, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.m_nudEffortIncr, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'm_tlpSponsors
        '
        Me.m_tlpSponsors.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_tlpSponsors.BackColor = System.Drawing.Color.White
        Me.m_tlpSponsors.ColumnCount = 3
        Me.m_tlpSponsors.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25.0!))
        Me.m_tlpSponsors.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25.0!))
        Me.m_tlpSponsors.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25.0!))
        Me.m_tlpSponsors.Controls.Add(Me.m_pbLenfest, 0, 0)
        Me.m_tlpSponsors.Controls.Add(Me.m_pbSAUP, 1, 0)
        Me.m_tlpSponsors.Controls.Add(Me.m_pbEU, 2, 0)
        Me.m_tlpSponsors.Location = New System.Drawing.Point(3, 397)
        Me.m_tlpSponsors.Margin = New System.Windows.Forms.Padding(0)
        Me.m_tlpSponsors.Name = "m_tlpSponsors"
        Me.m_tlpSponsors.RowCount = 1
        Me.m_tlpSponsors.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.m_tlpSponsors.Size = New System.Drawing.Size(697, 76)
        Me.m_tlpSponsors.TabIndex = 18
        '
        'm_pbLenfest
        '
        Me.m_pbLenfest.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_pbLenfest.Image = Global.EwEValueChainPlugin.My.Resources.Resources.Lenfest_Logo_50px
        Me.m_pbLenfest.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.m_pbLenfest.Location = New System.Drawing.Point(3, 3)
        Me.m_pbLenfest.Name = "m_pbLenfest"
        Me.m_pbLenfest.Size = New System.Drawing.Size(226, 70)
        Me.m_pbLenfest.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.m_pbLenfest.TabIndex = 0
        Me.m_pbLenfest.TabStop = False
        '
        'm_pbSAUP
        '
        Me.m_pbSAUP.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_pbSAUP.Image = Global.EwEValueChainPlugin.My.Resources.Resources.sautxt_50px
        Me.m_pbSAUP.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.m_pbSAUP.Location = New System.Drawing.Point(235, 3)
        Me.m_pbSAUP.Name = "m_pbSAUP"
        Me.m_pbSAUP.Size = New System.Drawing.Size(226, 70)
        Me.m_pbSAUP.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.m_pbSAUP.TabIndex = 1
        Me.m_pbSAUP.TabStop = False
        '
        'm_pbEU
        '
        Me.m_pbEU.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_pbEU.Image = Global.EwEValueChainPlugin.My.Resources.Resources.EU_50px
        Me.m_pbEU.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.m_pbEU.Location = New System.Drawing.Point(467, 3)
        Me.m_pbEU.Name = "m_pbEU"
        Me.m_pbEU.Size = New System.Drawing.Size(227, 70)
        Me.m_pbEU.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.m_pbEU.TabIndex = 2
        Me.m_pbEU.TabStop = False
        '
        'm_lblSponsors
        '
        Me.m_lblSponsors.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_lblSponsors.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.m_lblSponsors.Location = New System.Drawing.Point(0, 374)
        Me.m_lblSponsors.Name = "m_lblSponsors"
        Me.m_lblSponsors.Size = New System.Drawing.Size(703, 18)
        Me.m_lblSponsors.TabIndex = 17
        Me.m_lblSponsors.Text = "Sponsors"
        Me.m_lblSponsors.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'm_lblBaseYear
        '
        Me.m_lblBaseYear.AutoSize = True
        Me.m_lblBaseYear.Location = New System.Drawing.Point(3, 126)
        Me.m_lblBaseYear.Name = "m_lblBaseYear"
        Me.m_lblBaseYear.Size = New System.Drawing.Size(57, 13)
        Me.m_lblBaseYear.TabIndex = 6
        Me.m_lblBaseYear.Text = "&Base year:"
        '
        'm_nudBaseYear
        '
        Me.m_nudBaseYear.Location = New System.Drawing.Point(93, 124)
        Me.m_nudBaseYear.Name = "m_nudBaseYear"
        Me.m_nudBaseYear.Size = New System.Drawing.Size(106, 20)
        Me.m_nudBaseYear.TabIndex = 7
        '
        'm_hdrEcosimSettings
        '
        Me.m_hdrEcosimSettings.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_hdrEcosimSettings.BackColor = System.Drawing.SystemColors.ButtonShadow
        Me.m_hdrEcosimSettings.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold)
        Me.m_hdrEcosimSettings.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.m_hdrEcosimSettings.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.m_hdrEcosimSettings.Location = New System.Drawing.Point(0, 97)
        Me.m_hdrEcosimSettings.Name = "m_hdrEcosimSettings"
        Me.m_hdrEcosimSettings.Size = New System.Drawing.Size(703, 18)
        Me.m_hdrEcosimSettings.TabIndex = 5
        Me.m_hdrEcosimSettings.Text = "Ecosim-dependent settings"
        Me.m_hdrEcosimSettings.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'm_hdrIntegration
        '
        Me.m_hdrIntegration.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_hdrIntegration.BackColor = System.Drawing.SystemColors.ButtonShadow
        Me.m_hdrIntegration.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold)
        Me.m_hdrIntegration.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.m_hdrIntegration.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.m_hdrIntegration.Location = New System.Drawing.Point(0, 0)
        Me.m_hdrIntegration.Name = "m_hdrIntegration"
        Me.m_hdrIntegration.Size = New System.Drawing.Size(703, 18)
        Me.m_hdrIntegration.TabIndex = 0
        Me.m_hdrIntegration.Text = "Integration"
        Me.m_hdrIntegration.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'm_chkRunWithEcopath
        '
        Me.m_chkRunWithEcopath.AutoSize = True
        Me.m_chkRunWithEcopath.Location = New System.Drawing.Point(6, 24)
        Me.m_chkRunWithEcopath.Name = "m_chkRunWithEcopath"
        Me.m_chkRunWithEcopath.Size = New System.Drawing.Size(111, 17)
        Me.m_chkRunWithEcopath.TabIndex = 1
        Me.m_chkRunWithEcopath.Text = "Run with Eco&path"
        Me.m_chkRunWithEcopath.UseVisualStyleBackColor = True
        '
        'm_chkRunWithEcosim
        '
        Me.m_chkRunWithEcosim.AutoSize = True
        Me.m_chkRunWithEcosim.Location = New System.Drawing.Point(6, 47)
        Me.m_chkRunWithEcosim.Name = "m_chkRunWithEcosim"
        Me.m_chkRunWithEcosim.Size = New System.Drawing.Size(105, 17)
        Me.m_chkRunWithEcosim.TabIndex = 2
        Me.m_chkRunWithEcosim.Text = "Run with Eco&sim"
        Me.m_chkRunWithEcosim.UseVisualStyleBackColor = True
        '
        'm_chkRunWithSearches
        '
        Me.m_chkRunWithSearches.AutoSize = True
        Me.m_chkRunWithSearches.Location = New System.Drawing.Point(6, 70)
        Me.m_chkRunWithSearches.Name = "m_chkRunWithSearches"
        Me.m_chkRunWithSearches.Size = New System.Drawing.Size(114, 17)
        Me.m_chkRunWithSearches.TabIndex = 3
        Me.m_chkRunWithSearches.Text = "Run with &searches"
        Me.m_chkRunWithSearches.UseVisualStyleBackColor = True
        '
        'm_hdrEQ
        '
        Me.m_hdrEQ.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_hdrEQ.BackColor = System.Drawing.SystemColors.ButtonShadow
        Me.m_hdrEQ.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold)
        Me.m_hdrEQ.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.m_hdrEQ.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.m_hdrEQ.Location = New System.Drawing.Point(0, 156)
        Me.m_hdrEQ.Name = "m_hdrEQ"
        Me.m_hdrEQ.Size = New System.Drawing.Size(703, 18)
        Me.m_hdrEQ.TabIndex = 8
        Me.m_hdrEQ.Text = "Equilibrium search"
        Me.m_hdrEQ.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'm_clbFleets
        '
        Me.m_clbFleets.CheckOnClick = True
        Me.m_clbFleets.FormattingEnabled = True
        Me.m_clbFleets.IntegralHeight = False
        Me.m_clbFleets.Location = New System.Drawing.Point(356, 182)
        Me.m_clbFleets.Name = "m_clbFleets"
        Me.m_clbFleets.Size = New System.Drawing.Size(137, 72)
        Me.m_clbFleets.TabIndex = 16
        '
        'm_lblFleets
        '
        Me.m_lblFleets.AutoSize = True
        Me.m_lblFleets.Location = New System.Drawing.Point(235, 184)
        Me.m_lblFleets.Name = "m_lblFleets"
        Me.m_lblFleets.Size = New System.Drawing.Size(115, 13)
        Me.m_lblFleets.TabIndex = 15
        Me.m_lblFleets.Text = "&Fleets to vary effort for:"
        '
        'm_lblEffortMin
        '
        Me.m_lblEffortMin.AutoSize = True
        Me.m_lblEffortMin.Location = New System.Drawing.Point(3, 184)
        Me.m_lblEffortMin.Name = "m_lblEffortMin"
        Me.m_lblEffortMin.Size = New System.Drawing.Size(78, 13)
        Me.m_lblEffortMin.TabIndex = 9
        Me.m_lblEffortMin.Text = "M&inimum effort:"
        '
        'm_lblEffortMax
        '
        Me.m_lblEffortMax.AutoSize = True
        Me.m_lblEffortMax.Location = New System.Drawing.Point(3, 210)
        Me.m_lblEffortMax.Name = "m_lblEffortMax"
        Me.m_lblEffortMax.Size = New System.Drawing.Size(78, 13)
        Me.m_lblEffortMax.TabIndex = 11
        Me.m_lblEffortMax.Text = "M&aximum effort"
        '
        'm_lbEffortIncr
        '
        Me.m_lbEffortIncr.AutoSize = True
        Me.m_lbEffortIncr.Location = New System.Drawing.Point(3, 236)
        Me.m_lbEffortIncr.Name = "m_lbEffortIncr"
        Me.m_lbEffortIncr.Size = New System.Drawing.Size(84, 13)
        Me.m_lbEffortIncr.TabIndex = 13
        Me.m_lbEffortIncr.Text = "Effort i&ncrement:"
        '
        'm_nudEffortMin
        '
        Me.m_nudEffortMin.DecimalPlaces = 2
        Me.m_nudEffortMin.Location = New System.Drawing.Point(93, 182)
        Me.m_nudEffortMin.Name = "m_nudEffortMin"
        Me.m_nudEffortMin.Size = New System.Drawing.Size(106, 20)
        Me.m_nudEffortMin.TabIndex = 10
        '
        'm_nudEffortMax
        '
        Me.m_nudEffortMax.DecimalPlaces = 2
        Me.m_nudEffortMax.Location = New System.Drawing.Point(93, 208)
        Me.m_nudEffortMax.Name = "m_nudEffortMax"
        Me.m_nudEffortMax.Size = New System.Drawing.Size(106, 20)
        Me.m_nudEffortMax.TabIndex = 12
        '
        'm_nudEffortIncr
        '
        Me.m_nudEffortIncr.DecimalPlaces = 2
        Me.m_nudEffortIncr.Location = New System.Drawing.Point(93, 234)
        Me.m_nudEffortIncr.Minimum = New Decimal(New Integer() {1, 0, 0, 131072})
        Me.m_nudEffortIncr.Name = "m_nudEffortIncr"
        Me.m_nudEffortIncr.Size = New System.Drawing.Size(106, 20)
        Me.m_nudEffortIncr.TabIndex = 14
        Me.m_nudEffortIncr.Value = New Decimal(New Integer() {25, 0, 0, 131072})
        '
        'm_chkResultsByFleet
        '
        Me.m_chkResultsByFleet.AutoSize = True
        Me.m_chkResultsByFleet.Location = New System.Drawing.Point(238, 21)
        Me.m_chkResultsByFleet.Name = "m_chkResultsByFleet"
        Me.m_chkResultsByFleet.Size = New System.Drawing.Size(136, 17)
        Me.m_chkResultsByFleet.TabIndex = 4
        Me.m_chkResultsByFleet.Text = "Produce results by &fleet"
        Me.m_chkResultsByFleet.UseVisualStyleBackColor = True
        '
        'ucParameters
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.m_nudEffortIncr)
        Me.Controls.Add(Me.m_nudEffortMax)
        Me.Controls.Add(Me.m_nudEffortMin)
        Me.Controls.Add(Me.m_lbEffortIncr)
        Me.Controls.Add(Me.m_lblEffortMax)
        Me.Controls.Add(Me.m_lblEffortMin)
        Me.Controls.Add(Me.m_lblFleets)
        Me.Controls.Add(Me.m_clbFleets)
        Me.Controls.Add(Me.m_chkRunWithSearches)
        Me.Controls.Add(Me.m_chkRunWithEcosim)
        Me.Controls.Add(Me.m_chkResultsByFleet)
        Me.Controls.Add(Me.m_chkRunWithEcopath)
        Me.Controls.Add(Me.m_tlpSponsors)
        Me.Controls.Add(Me.m_nudBaseYear)
        Me.Controls.Add(Me.m_lblBaseYear)
        Me.Controls.Add(Me.m_hdrEQ)
        Me.Controls.Add(Me.m_hdrIntegration)
        Me.Controls.Add(Me.m_hdrEcosimSettings)
        Me.Controls.Add(Me.m_lblSponsors)
        Me.MinimumSize = New System.Drawing.Size(400, 400)
        Me.Name = "ucParameters"
        Me.Size = New System.Drawing.Size(703, 476)
        Me.m_tlpSponsors.ResumeLayout(False)
        CType(Me.m_pbLenfest, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.m_pbSAUP, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.m_pbEU, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.m_nudBaseYear, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.m_nudEffortMin, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.m_nudEffortMax, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.m_nudEffortIncr, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents m_pbLenfest As System.Windows.Forms.PictureBox
    Friend WithEvents m_pbSAUP As System.Windows.Forms.PictureBox
    Friend WithEvents m_pbEU As System.Windows.Forms.PictureBox
    Private WithEvents m_lblBaseYear As System.Windows.Forms.Label
    Private WithEvents m_nudBaseYear As System.Windows.Forms.NumericUpDown
    Private WithEvents m_hdrEcosimSettings As cEwEHeaderLabel
    Private WithEvents m_hdrIntegration As cEwEHeaderLabel
    Private WithEvents m_chkRunWithEcopath As System.Windows.Forms.CheckBox
    Private WithEvents m_chkRunWithEcosim As System.Windows.Forms.CheckBox
    Private WithEvents m_chkRunWithSearches As System.Windows.Forms.CheckBox
    Private WithEvents m_tlpSponsors As System.Windows.Forms.TableLayoutPanel
    Private WithEvents m_lblSponsors As cEwEHeaderLabel
    Private WithEvents m_hdrEQ As cEwEHeaderLabel
    Private WithEvents m_clbFleets As System.Windows.Forms.CheckedListBox
    Private WithEvents m_lblFleets As System.Windows.Forms.Label
    Private WithEvents m_lblEffortMin As System.Windows.Forms.Label
    Private WithEvents m_lbEffortIncr As System.Windows.Forms.Label
    Private WithEvents m_lblEffortMax As System.Windows.Forms.Label
    Private WithEvents m_nudEffortMin As System.Windows.Forms.NumericUpDown
    Private WithEvents m_nudEffortMax As System.Windows.Forms.NumericUpDown
    Private WithEvents m_nudEffortIncr As System.Windows.Forms.NumericUpDown
    Private WithEvents m_chkResultsByFleet As System.Windows.Forms.CheckBox

End Class
