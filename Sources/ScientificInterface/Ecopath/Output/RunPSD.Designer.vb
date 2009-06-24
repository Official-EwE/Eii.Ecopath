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
            Dim m_sep1 As System.Windows.Forms.ToolStripSeparator
            Dim m_sep666 As System.Windows.Forms.ToolStripSeparator
            Dim m_sep2 As System.Windows.Forms.ToolStripSeparator
            Dim m_sep3 As System.Windows.Forms.ToolStripSeparator
            Dim m_sep4 As System.Windows.Forms.ToolStripSeparator
            Dim m_sep5 As System.Windows.Forms.ToolStripSeparator
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(RunPSD))
            Me.m_tsRunPSD = New System.Windows.Forms.ToolStrip
            Me.m_tsddTotalMortality = New System.Windows.Forms.ToolStripDropDownButton
            Me.m_tsmiGroupPB = New System.Windows.Forms.ToolStripMenuItem
            Me.m_tsmiLorenzen = New System.Windows.Forms.ToolStripMenuItem
            Me.m_tsmiMeanLat = New System.Windows.Forms.ToolStripMenuItem
            Me.m_tscbxMeanLat = New System.Windows.Forms.ToolStripComboBox
            Me.m_tsbnShowHideGroups = New System.Windows.Forms.ToolStripButton
            Me.m_tslblNoOfPointsPSD = New System.Windows.Forms.ToolStripLabel
            Me.m_tstbxNoOfPointsPSD = New System.Windows.Forms.ToolStripTextBox
            Me.m_tslblMinWeight = New System.Windows.Forms.ToolStripLabel
            Me.m_tstbxMinWeight = New System.Windows.Forms.ToolStripTextBox
            Me.m_tslblNoOfPointsMovAvg = New System.Windows.Forms.ToolStripLabel
            Me.m_tstbxNoOfPointsMovAvg = New System.Windows.Forms.ToolStripTextBox
            Me.m_tsbtnRun = New System.Windows.Forms.ToolStripButton
            Me.m_zedgraph = New ZedGraph.ZedGraphControl
            m_sep1 = New System.Windows.Forms.ToolStripSeparator
            m_sep666 = New System.Windows.Forms.ToolStripSeparator
            m_sep2 = New System.Windows.Forms.ToolStripSeparator
            m_sep3 = New System.Windows.Forms.ToolStripSeparator
            m_sep4 = New System.Windows.Forms.ToolStripSeparator
            m_sep5 = New System.Windows.Forms.ToolStripSeparator
            Me.m_tsRunPSD.SuspendLayout()
            Me.SuspendLayout()
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
            Me.m_tsRunPSD.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tsddTotalMortality, m_sep1, Me.m_tsbnShowHideGroups, m_sep666, Me.m_tslblNoOfPointsPSD, Me.m_tstbxNoOfPointsPSD, m_sep2, Me.m_tslblMinWeight, Me.m_tstbxMinWeight, m_sep3, Me.m_tslblNoOfPointsMovAvg, Me.m_tstbxNoOfPointsMovAvg, m_sep4, Me.m_tsbtnRun, m_sep5})
            Me.m_tsRunPSD.Location = New System.Drawing.Point(0, 0)
            Me.m_tsRunPSD.Name = "m_tsRunPSD"
            Me.m_tsRunPSD.Size = New System.Drawing.Size(869, 25)
            Me.m_tsRunPSD.TabIndex = 0
            Me.m_tsRunPSD.Text = "ToolStrip1"
            '
            'm_tsddTotalMortality
            '
            Me.m_tsddTotalMortality.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tsmiGroupPB, Me.m_tsmiLorenzen, Me.m_tsmiMeanLat, Me.m_tscbxMeanLat})
            Me.m_tsddTotalMortality.Image = Global.ScientificInterface.My.Resources.Resources.OptionsHS
            Me.m_tsddTotalMortality.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.m_tsddTotalMortality.Name = "m_tsddTotalMortality"
            Me.m_tsddTotalMortality.Size = New System.Drawing.Size(105, 22)
            Me.m_tsddTotalMortality.Text = "Total mortality"
            '
            'm_tsmiGroupPB
            '
            Me.m_tsmiGroupPB.CheckOnClick = True
            Me.m_tsmiGroupPB.Name = "m_tsmiGroupPB"
            Me.m_tsmiGroupPB.Size = New System.Drawing.Size(174, 22)
            Me.m_tsmiGroupPB.Text = "Group P/B "
            '
            'm_tsmiLorenzen
            '
            Me.m_tsmiLorenzen.CheckOnClick = True
            Me.m_tsmiLorenzen.Name = "m_tsmiLorenzen"
            Me.m_tsmiLorenzen.Size = New System.Drawing.Size(174, 22)
            Me.m_tsmiLorenzen.Text = "Lorenzen-variable "
            '
            'm_tsmiMeanLat
            '
            Me.m_tsmiMeanLat.Margin = New System.Windows.Forms.Padding(5, 0, 0, 0)
            Me.m_tsmiMeanLat.Name = "m_tsmiMeanLat"
            Me.m_tsmiMeanLat.Size = New System.Drawing.Size(174, 22)
            Me.m_tsmiMeanLat.Text = "Mean lat:"
            '
            'm_tscbxMeanLat
            '
            Me.m_tscbxMeanLat.Enabled = False
            Me.m_tscbxMeanLat.FlatStyle = System.Windows.Forms.FlatStyle.Standard
            Me.m_tscbxMeanLat.Items.AddRange(New Object() {">0 and <30", ">30 and <60", ">60 and <90"})
            Me.m_tscbxMeanLat.Margin = New System.Windows.Forms.Padding(55, -22, 2, 2)
            Me.m_tscbxMeanLat.Name = "m_tscbxMeanLat"
            Me.m_tscbxMeanLat.Size = New System.Drawing.Size(86, 21)
            '
            'm_tsbnShowHideGroups
            '
            Me.m_tsbnShowHideGroups.Image = Global.ScientificInterface.My.Resources.Resources.Eye_open
            Me.m_tsbnShowHideGroups.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.m_tsbnShowHideGroups.Name = "m_tsbnShowHideGroups"
            Me.m_tsbnShowHideGroups.Size = New System.Drawing.Size(107, 22)
            Me.m_tsbnShowHideGroups.Text = "Select groups ..."
            '
            'm_tslblNoOfPointsPSD
            '
            Me.m_tslblNoOfPointsPSD.Name = "m_tslblNoOfPointsPSD"
            Me.m_tslblNoOfPointsPSD.Size = New System.Drawing.Size(97, 22)
            Me.m_tslblNoOfPointsPSD.Text = "No. of wt. classes:"
            '
            'm_tstbxNoOfPointsPSD
            '
            Me.m_tstbxNoOfPointsPSD.Name = "m_tstbxNoOfPointsPSD"
            Me.m_tstbxNoOfPointsPSD.Size = New System.Drawing.Size(30, 25)
            '
            'm_tslblMinWeight
            '
            Me.m_tslblMinWeight.Name = "m_tslblMinWeight"
            Me.m_tslblMinWeight.Size = New System.Drawing.Size(107, 22)
            Me.m_tslblMinWeight.Text = "Lowest wt. class (g):"
            '
            'm_tstbxMinWeight
            '
            Me.m_tstbxMinWeight.Name = "m_tstbxMinWeight"
            Me.m_tstbxMinWeight.Size = New System.Drawing.Size(40, 25)
            '
            'm_tslblNoOfPointsMovAvg
            '
            Me.m_tslblNoOfPointsMovAvg.Name = "m_tslblNoOfPointsMovAvg"
            Me.m_tslblNoOfPointsMovAvg.Size = New System.Drawing.Size(132, 22)
            Me.m_tslblNoOfPointsMovAvg.Text = "No. of pts. for mov. avg.:"
            '
            'm_tstbxNoOfPointsMovAvg
            '
            Me.m_tstbxNoOfPointsMovAvg.Name = "m_tstbxNoOfPointsMovAvg"
            Me.m_tstbxNoOfPointsMovAvg.Size = New System.Drawing.Size(30, 25)
            '
            'm_tsbtnRun
            '
            Me.m_tsbtnRun.AutoSize = False
            Me.m_tsbtnRun.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            Me.m_tsbtnRun.Image = CType(resources.GetObject("m_tsbtnRun.Image"), System.Drawing.Image)
            Me.m_tsbtnRun.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.m_tsbtnRun.Name = "m_tsbtnRun"
            Me.m_tsbtnRun.Size = New System.Drawing.Size(50, 22)
            Me.m_tsbtnRun.Text = "Run"
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
            Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.Name = "RunPSD"
            Me.ShowInTaskbar = False
            Me.Text = "RunParticleSizeDistribution"
            Me.m_tsRunPSD.ResumeLayout(False)
            Me.m_tsRunPSD.PerformLayout()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents m_tsRunPSD As System.Windows.Forms.ToolStrip
        Private WithEvents m_tstbxNoOfPointsMovAvg As System.Windows.Forms.ToolStripTextBox
        Private WithEvents m_tsbtnRun As System.Windows.Forms.ToolStripButton
        Private WithEvents m_tsddTotalMortality As System.Windows.Forms.ToolStripDropDownButton
        Private WithEvents m_tsmiGroupPB As System.Windows.Forms.ToolStripMenuItem
        Private WithEvents m_tsmiLorenzen As System.Windows.Forms.ToolStripMenuItem
        Private WithEvents m_tsbnShowHideGroups As System.Windows.Forms.ToolStripButton
        Private WithEvents m_tslblNoOfPointsPSD As System.Windows.Forms.ToolStripLabel
        Private WithEvents m_tstbxNoOfPointsPSD As System.Windows.Forms.ToolStripTextBox
        Private WithEvents m_tslblMinWeight As System.Windows.Forms.ToolStripLabel
        Private WithEvents m_tstbxMinWeight As System.Windows.Forms.ToolStripTextBox
        Private WithEvents m_tslblNoOfPointsMovAvg As System.Windows.Forms.ToolStripLabel
        Private WithEvents m_zedgraph As ZedGraph.ZedGraphControl
        Private WithEvents m_tsmiMeanLat As System.Windows.Forms.ToolStripMenuItem
        Private WithEvents m_tscbxMeanLat As System.Windows.Forms.ToolStripComboBox
    End Class

End Namespace
