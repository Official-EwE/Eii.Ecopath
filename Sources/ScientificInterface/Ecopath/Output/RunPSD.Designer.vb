Imports WeifenLuo.WinFormsUI.Docking
Imports ScientificInterfaceShared

Namespace Ecopath.Output

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class RunPSD
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
            Me.components = New System.ComponentModel.Container
            Dim m_tsmiDummy1 As System.Windows.Forms.ToolStripMenuItem
            Dim m_tsmiDummy2 As System.Windows.Forms.ToolStripMenuItem
            Dim m_sep1 As System.Windows.Forms.ToolStripSeparator
            Dim m_sep666 As System.Windows.Forms.ToolStripSeparator
            Dim m_sep2 As System.Windows.Forms.ToolStripSeparator
            Dim m_sep3 As System.Windows.Forms.ToolStripSeparator
            Dim m_sep4 As System.Windows.Forms.ToolStripSeparator
            Dim m_sep5 As System.Windows.Forms.ToolStripSeparator
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(RunPSD))
            Me.m_tsRunPSD = New System.Windows.Forms.ToolStrip
            Me.m_tbddTotalMortality = New System.Windows.Forms.ToolStripDropDownButton
            Me.m_tsmiGroupPB = New System.Windows.Forms.ToolStripMenuItem
            Me.m_tsmiLorenzen = New System.Windows.Forms.ToolStripMenuItem
            Me.m_tbxNWLat = New System.Windows.Forms.ToolStripTextBox
            Me.m_tbxSELat = New System.Windows.Forms.ToolStripTextBox
            Me.m_tsmiDummy3 = New System.Windows.Forms.ToolStripMenuItem
            Me.m_cbxAvgLat = New System.Windows.Forms.ToolStripComboBox
            Me.m_tsbnShowHideGroups = New System.Windows.Forms.ToolStripButton
            Me.m_lblNoOfPointsPSD = New System.Windows.Forms.ToolStripLabel
            Me.m_tbxNoOfPointsPSD = New System.Windows.Forms.ToolStripTextBox
            Me.m_lblMinWeight = New System.Windows.Forms.ToolStripLabel
            Me.m_tbxMinWeight = New System.Windows.Forms.ToolStripTextBox
            Me.m_lblNoOfPointsMovAvg = New System.Windows.Forms.ToolStripLabel
            Me.m_tbxNoOfPointsMovAvg = New System.Windows.Forms.ToolStripTextBox
            Me.m_btnRun = New System.Windows.Forms.ToolStripButton
            Me.m_zedgraph = New ZedGraph.ZedGraphControl
            m_tsmiDummy1 = New System.Windows.Forms.ToolStripMenuItem
            m_tsmiDummy2 = New System.Windows.Forms.ToolStripMenuItem
            m_sep1 = New System.Windows.Forms.ToolStripSeparator
            m_sep666 = New System.Windows.Forms.ToolStripSeparator
            m_sep2 = New System.Windows.Forms.ToolStripSeparator
            m_sep3 = New System.Windows.Forms.ToolStripSeparator
            m_sep4 = New System.Windows.Forms.ToolStripSeparator
            m_sep5 = New System.Windows.Forms.ToolStripSeparator
            Me.m_tsRunPSD.SuspendLayout()
            Me.SuspendLayout()
            '
            'm_tsmiDummy1
            '
            m_tsmiDummy1.Margin = New System.Windows.Forms.Padding(10, 0, 0, 0)
            m_tsmiDummy1.Name = "m_tsmiDummy1"
            m_tsmiDummy1.Size = New System.Drawing.Size(184, 22)
            m_tsmiDummy1.Text = "NW corner latitudes:"
            '
            'm_tsmiDummy2
            '
            m_tsmiDummy2.Margin = New System.Windows.Forms.Padding(10, -1, 0, 0)
            m_tsmiDummy2.Name = "m_tsmiDummy2"
            m_tsmiDummy2.Size = New System.Drawing.Size(184, 22)
            m_tsmiDummy2.Text = "SE corner latitudes:"
            '
            'm_sep1
            '
            m_sep1.Name = "m_sep1"
            m_sep1.Size = New System.Drawing.Size(6, 25)
            '
            'm_sep666
            '
            m_sep666.Name = "m_sep666"
            m_sep666.Size = New System.Drawing.Size(6, 25)
            '
            'm_sep2
            '
            m_sep2.Name = "m_sep2"
            m_sep2.Size = New System.Drawing.Size(6, 25)
            '
            'm_sep3
            '
            m_sep3.Name = "m_sep3"
            m_sep3.Size = New System.Drawing.Size(6, 25)
            '
            'm_sep4
            '
            m_sep4.Name = "m_sep4"
            m_sep4.Size = New System.Drawing.Size(6, 25)
            '
            'm_sep5
            '
            m_sep5.Name = "m_sep5"
            m_sep5.Size = New System.Drawing.Size(6, 25)
            '
            'm_tsRunPSD
            '
            Me.m_tsRunPSD.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tbddTotalMortality, m_sep1, Me.m_tsbnShowHideGroups, m_sep666, Me.m_lblNoOfPointsPSD, Me.m_tbxNoOfPointsPSD, m_sep2, Me.m_lblMinWeight, Me.m_tbxMinWeight, m_sep3, Me.m_lblNoOfPointsMovAvg, Me.m_tbxNoOfPointsMovAvg, m_sep4, Me.m_btnRun, m_sep5})
            Me.m_tsRunPSD.Location = New System.Drawing.Point(0, 0)
            Me.m_tsRunPSD.Name = "m_tsRunPSD"
            Me.m_tsRunPSD.Size = New System.Drawing.Size(869, 25)
            Me.m_tsRunPSD.TabIndex = 0
            Me.m_tsRunPSD.Text = "ToolStrip1"
            '
            'm_tbddTotalMortality
            '
            Me.m_tbddTotalMortality.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tsmiGroupPB, Me.m_tsmiLorenzen, m_tsmiDummy1, Me.m_tbxNWLat, m_tsmiDummy2, Me.m_tbxSELat, Me.m_tsmiDummy3, Me.m_cbxAvgLat})
            Me.m_tbddTotalMortality.Image = Global.ScientificInterface.My.Resources.Resources.OptionsHS
            Me.m_tbddTotalMortality.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.m_tbddTotalMortality.Name = "m_tbddTotalMortality"
            Me.m_tbddTotalMortality.Size = New System.Drawing.Size(105, 22)
            Me.m_tbddTotalMortality.Text = "Total mortality"
            '
            'm_tsmiGroupPB
            '
            Me.m_tsmiGroupPB.CheckOnClick = True
            Me.m_tsmiGroupPB.Name = "m_tsmiGroupPB"
            Me.m_tsmiGroupPB.Size = New System.Drawing.Size(184, 22)
            Me.m_tsmiGroupPB.Text = "Group P/B "
            '
            'm_tsmiLorenzen
            '
            Me.m_tsmiLorenzen.CheckOnClick = True
            Me.m_tsmiLorenzen.Name = "m_tsmiLorenzen"
            Me.m_tsmiLorenzen.Size = New System.Drawing.Size(184, 22)
            Me.m_tsmiLorenzen.Text = "Lorenzen-variable "
            '
            'm_tbxNWLat
            '
            Me.m_tbxNWLat.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.m_tbxNWLat.Margin = New System.Windows.Forms.Padding(115, -20, 1, 1)
            Me.m_tbxNWLat.Name = "m_tbxNWLat"
            Me.m_tbxNWLat.Size = New System.Drawing.Size(35, 21)
            '
            'm_tbxSELat
            '
            Me.m_tbxSELat.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.m_tbxSELat.Margin = New System.Windows.Forms.Padding(115, -21, 1, 1)
            Me.m_tbxSELat.Name = "m_tbxSELat"
            Me.m_tbxSELat.Size = New System.Drawing.Size(35, 21)
            '
            'm_tsmiDummy3
            '
            Me.m_tsmiDummy3.Margin = New System.Windows.Forms.Padding(10, 0, 0, 0)
            Me.m_tsmiDummy3.Name = "m_tsmiDummy3"
            Me.m_tsmiDummy3.Size = New System.Drawing.Size(184, 22)
            Me.m_tsmiDummy3.Text = "Mean lat:"
            '
            'm_cbxAvgLat
            '
            Me.m_cbxAvgLat.FlatStyle = System.Windows.Forms.FlatStyle.Standard
            Me.m_cbxAvgLat.Items.AddRange(New Object() {"> 0 and < 30", "> 30 and < 60", "> 60 and < 90"})
            Me.m_cbxAvgLat.Margin = New System.Windows.Forms.Padding(60, -22, 2, 2)
            Me.m_cbxAvgLat.Name = "m_cbxAvgLat"
            Me.m_cbxAvgLat.Size = New System.Drawing.Size(90, 21)
            '
            'm_tsbnShowHideGroups
            '
            Me.m_tsbnShowHideGroups.Image = Global.ScientificInterface.My.Resources.Resources.Eye_open
            Me.m_tsbnShowHideGroups.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.m_tsbnShowHideGroups.Name = "m_tsbnShowHideGroups"
            Me.m_tsbnShowHideGroups.Size = New System.Drawing.Size(107, 22)
            Me.m_tsbnShowHideGroups.Text = "Select groups ..."
            '
            'm_lblNoOfPointsPSD
            '
            Me.m_lblNoOfPointsPSD.Name = "m_lblNoOfPointsPSD"
            Me.m_lblNoOfPointsPSD.Size = New System.Drawing.Size(108, 22)
            Me.m_lblNoOfPointsPSD.Text = "No. of points for PSD"
            '
            'm_tbxNoOfPointsPSD
            '
            Me.m_tbxNoOfPointsPSD.Name = "m_tbxNoOfPointsPSD"
            Me.m_tbxNoOfPointsPSD.Size = New System.Drawing.Size(50, 25)
            '
            'm_lblMinWeight
            '
            Me.m_lblMinWeight.Name = "m_lblMinWeight"
            Me.m_lblMinWeight.Size = New System.Drawing.Size(59, 22)
            Me.m_lblMinWeight.Text = "Min. wt (g)"
            '
            'm_tbxMinWeight
            '
            Me.m_tbxMinWeight.Name = "m_tbxMinWeight"
            Me.m_tbxMinWeight.Size = New System.Drawing.Size(50, 25)
            '
            'm_lblNoOfPointsMovAvg
            '
            Me.m_lblNoOfPointsMovAvg.Name = "m_lblNoOfPointsMovAvg"
            Me.m_lblNoOfPointsMovAvg.Size = New System.Drawing.Size(130, 22)
            Me.m_lblNoOfPointsMovAvg.Text = "No. of points for mov avg"
            Me.m_lblNoOfPointsMovAvg.Visible = False
            '
            'm_tbxNoOfPointsMovAvg
            '
            Me.m_tbxNoOfPointsMovAvg.Name = "m_tbxNoOfPointsMovAvg"
            Me.m_tbxNoOfPointsMovAvg.Size = New System.Drawing.Size(50, 25)
            Me.m_tbxNoOfPointsMovAvg.Visible = False
            '
            'm_btnRun
            '
            Me.m_btnRun.AutoSize = False
            Me.m_btnRun.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            Me.m_btnRun.Image = CType(resources.GetObject("m_btnRun.Image"), System.Drawing.Image)
            Me.m_btnRun.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.m_btnRun.Name = "m_btnRun"
            Me.m_btnRun.Size = New System.Drawing.Size(50, 22)
            Me.m_btnRun.Text = "Run"
            '
            'm_zedgraph
            '
            Me.m_zedgraph.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_zedgraph.Location = New System.Drawing.Point(0, 25)
            Me.m_zedgraph.Name = "m_zedgraph"
            Me.m_zedgraph.ScrollGrace = 0
            Me.m_zedgraph.ScrollMaxX = 0
            Me.m_zedgraph.ScrollMaxY = 0
            Me.m_zedgraph.ScrollMaxY2 = 0
            Me.m_zedgraph.ScrollMinX = 0
            Me.m_zedgraph.ScrollMinY = 0
            Me.m_zedgraph.ScrollMinY2 = 0
            Me.m_zedgraph.Size = New System.Drawing.Size(869, 241)
            Me.m_zedgraph.TabIndex = 1
            '
            'RunPSD
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(869, 266)
            Me.Controls.Add(Me.m_zedgraph)
            Me.Controls.Add(Me.m_tsRunPSD)
            Me.Name = "RunPSD"
            Me.ShowInTaskbar = False
            Me.Text = "RunParticleSizeDistribution"
            Me.m_tsRunPSD.ResumeLayout(False)
            Me.m_tsRunPSD.PerformLayout()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Friend WithEvents m_tsRunPSD As System.Windows.Forms.ToolStrip
        Private WithEvents m_tbxNoOfPointsMovAvg As System.Windows.Forms.ToolStripTextBox
        Private WithEvents m_btnRun As System.Windows.Forms.ToolStripButton
        Private WithEvents m_tbddTotalMortality As System.Windows.Forms.ToolStripDropDownButton
        Private WithEvents m_tsmiGroupPB As System.Windows.Forms.ToolStripMenuItem
        Private WithEvents m_tsmiLorenzen As System.Windows.Forms.ToolStripMenuItem
        Private WithEvents m_tbxNWLat As System.Windows.Forms.ToolStripTextBox
        Private WithEvents m_tbxSELat As System.Windows.Forms.ToolStripTextBox
        Private WithEvents m_tsbnShowHideGroups As System.Windows.Forms.ToolStripButton
        Private WithEvents m_lblNoOfPointsPSD As System.Windows.Forms.ToolStripLabel
        Private WithEvents m_tbxNoOfPointsPSD As System.Windows.Forms.ToolStripTextBox
        Private WithEvents m_lblMinWeight As System.Windows.Forms.ToolStripLabel
        Private WithEvents m_tbxMinWeight As System.Windows.Forms.ToolStripTextBox
        Private WithEvents m_lblNoOfPointsMovAvg As System.Windows.Forms.ToolStripLabel
        Private WithEvents m_zedgraph As ZedGraph.ZedGraphControl
        Friend WithEvents m_tsmiDummy3 As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents m_cbxAvgLat As System.Windows.Forms.ToolStripComboBox
    End Class

End Namespace
