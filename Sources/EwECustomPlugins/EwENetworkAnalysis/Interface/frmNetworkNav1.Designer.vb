<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmNetworkNav1
    Inherits WeifenLuo.WinFormsUI.DockContent

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container
        Dim tslblSelection1 As System.Windows.Forms.ToolStripLabel
        Dim TreeNode1 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Relative flows")
        Dim TreeNode2 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Absolute flows")
        Dim TreeNode3 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Transfer efficiency")
        Dim TreeNode4 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Flow pyramid")
        Dim TreeNode5 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Biomass by trophic level")
        Dim TreeNode6 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Biomass pyramid")
        Dim TreeNode7 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Catch by trophic level")
        Dim TreeNode8 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Catch pyramid")
        Dim TreeNode9 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Summary of flow data", New System.Windows.Forms.TreeNode() {TreeNode3, TreeNode4, TreeNode5, TreeNode6, TreeNode7, TreeNode8})
        Dim TreeNode10 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Trophic level decomposition", New System.Windows.Forms.TreeNode() {TreeNode1, TreeNode2, TreeNode9})
        Dim TreeNode11 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("From primary producers")
        Dim TreeNode12 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("From detritus")
        Dim TreeNode13 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("From all combined")
        Dim TreeNode14 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Flows and biomasses", New System.Windows.Forms.TreeNode() {TreeNode11, TreeNode12, TreeNode13})
        Dim TreeNode15 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("For harvest of all groups")
        Dim TreeNode16 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("For consumption of all groups")
        Dim TreeNode17 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Primary production required", New System.Windows.Forms.TreeNode() {TreeNode15, TreeNode16})
        Dim TreeNode18 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Impact data")
        Dim TreeNode19 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Graph of mixed trophic impacts")
        Dim TreeNode20 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Show/hide groups on mixed trophic impacts graph")
        Dim TreeNode21 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Summary of impact data", New System.Windows.Forms.TreeNode() {TreeNode19, TreeNode20})
        Dim TreeNode22 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Mixed trophic impact", New System.Windows.Forms.TreeNode() {TreeNode18, TreeNode21})
        Dim TreeNode23 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Total")
        Dim TreeNode24 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("By group")
        Dim TreeNode25 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Ascendency", New System.Windows.Forms.TreeNode() {TreeNode23, TreeNode24})
        Dim TreeNode26 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Flow from detritus")
        Dim TreeNode27 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Pathway")
        Dim TreeNode28 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Summary of pathways")
        Dim TreeNode29 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Consumer <- TL1", New System.Windows.Forms.TreeNode() {TreeNode27, TreeNode28})
        Dim TreeNode30 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Pathway")
        Dim TreeNode31 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Summary of pathways")
        Dim TreeNode32 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Consumer <- prey <- TL1", New System.Windows.Forms.TreeNode() {TreeNode30, TreeNode31})
        Dim TreeNode33 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Pathway")
        Dim TreeNode34 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Summary of pathways")
        Dim TreeNode35 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Top predator <- prey", New System.Windows.Forms.TreeNode() {TreeNode33, TreeNode34})
        Dim TreeNode36 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Pathway")
        Dim TreeNode37 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Summary of pathways")
        Dim TreeNode38 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Cycles (living)", New System.Windows.Forms.TreeNode() {TreeNode36, TreeNode37})
        Dim TreeNode39 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Pathway")
        Dim TreeNode40 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Summary of pathways")
        Dim TreeNode41 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Cycles (all)", New System.Windows.Forms.TreeNode() {TreeNode39, TreeNode40})
        Dim TreeNode42 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Cycling and path length")
        Dim TreeNode43 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Cycles and pathways", New System.Windows.Forms.TreeNode() {TreeNode29, TreeNode32, TreeNode35, TreeNode38, TreeNode41, TreeNode42})
        Dim TreeNode44 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Network analysis indices in Ecosim ")
        Dim TreeNode45 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("EwE Network Analysis Plugin", New System.Windows.Forms.TreeNode() {TreeNode10, TreeNode14, TreeNode17, TreeNode22, TreeNode25, TreeNode26, TreeNode43, TreeNode44})
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmNetworkNav1))
        Me.scNetworkAnalysis = New System.Windows.Forms.SplitContainer
        Me.tvNetworkAnalysis = New System.Windows.Forms.TreeView
        Me.imglstNetworkAnalysis = New System.Windows.Forms.ImageList(Me.components)
        Me.dgvNetworkAnalysis = New System.Windows.Forms.DataGridView
        Me.tsNetworkAnalysis = New System.Windows.Forms.ToolStrip
        Me.tscmbSelection1 = New System.Windows.Forms.ToolStripComboBox
        Me.tslblSelection2 = New System.Windows.Forms.ToolStripLabel
        Me.tscmbSelection2 = New System.Windows.Forms.ToolStripComboBox
        Me.lblNetworkAnalysis = New System.Windows.Forms.Label
        tslblSelection1 = New System.Windows.Forms.ToolStripLabel
        Me.scNetworkAnalysis.Panel1.SuspendLayout()
        Me.scNetworkAnalysis.Panel2.SuspendLayout()
        Me.scNetworkAnalysis.SuspendLayout()
        CType(Me.dgvNetworkAnalysis, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tsNetworkAnalysis.SuspendLayout()
        Me.SuspendLayout()
        '
        'tslblSelection1
        '
        tslblSelection1.Name = "tslblSelection1"
        tslblSelection1.Size = New System.Drawing.Size(80, 22)
        tslblSelection1.Text = "ToolStripLabel1"
        '
        'scNetworkAnalysis
        '
        Me.scNetworkAnalysis.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.scNetworkAnalysis.Location = New System.Drawing.Point(7, 37)
        Me.scNetworkAnalysis.Name = "scNetworkAnalysis"
        '
        'scNetworkAnalysis.Panel1
        '
        Me.scNetworkAnalysis.Panel1.Controls.Add(Me.tvNetworkAnalysis)
        '
        'scNetworkAnalysis.Panel2
        '
        Me.scNetworkAnalysis.Panel2.Controls.Add(Me.dgvNetworkAnalysis)
        Me.scNetworkAnalysis.Panel2.Controls.Add(Me.tsNetworkAnalysis)
        Me.scNetworkAnalysis.Size = New System.Drawing.Size(895, 467)
        Me.scNetworkAnalysis.SplitterDistance = 183
        Me.scNetworkAnalysis.TabIndex = 3
        '
        'tvNetworkAnalysis
        '
        Me.tvNetworkAnalysis.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.tvNetworkAnalysis.BackColor = System.Drawing.Color.MintCream
        Me.tvNetworkAnalysis.ImageIndex = 0
        Me.tvNetworkAnalysis.ImageList = Me.imglstNetworkAnalysis
        Me.tvNetworkAnalysis.Location = New System.Drawing.Point(0, 0)
        Me.tvNetworkAnalysis.Name = "tvNetworkAnalysis"
        TreeNode1.Name = "ndRelativeFlows"
        TreeNode1.Text = "Relative flows"
        TreeNode2.Name = "ndAbsoluteFlows"
        TreeNode2.Text = "Absolute flows"
        TreeNode3.Name = "ndTransferEfficiency"
        TreeNode3.Text = "Transfer efficiency"
        TreeNode4.Name = "ndFlowPyramid"
        TreeNode4.Text = "Flow pyramid"
        TreeNode5.Name = "ndBiomassByTrophicLevel"
        TreeNode5.Text = "Biomass by trophic level"
        TreeNode6.Name = "ndBiomassPyramid"
        TreeNode6.Text = "Biomass pyramid"
        TreeNode7.Name = "ndCatchByTrophicLevel"
        TreeNode7.Text = "Catch by trophic level"
        TreeNode8.Name = "ndCatchPyramid"
        TreeNode8.Text = "Catch pyramid"
        TreeNode9.Name = "ndSummaryOfFlowData"
        TreeNode9.Text = "Summary of flow data"
        TreeNode10.Name = "ndTrophicLlevelDdecomposition"
        TreeNode10.Text = "Trophic level decomposition"
        TreeNode11.Name = "ndFromPrimaryProducers"
        TreeNode11.Text = "From primary producers"
        TreeNode12.Name = "ndFromDetritus"
        TreeNode12.Text = "From detritus"
        TreeNode13.Name = "ndFromAllCombined"
        TreeNode13.Text = "From all combined"
        TreeNode14.Name = "ndFlowsAndBiomasses"
        TreeNode14.Text = "Flows and biomasses"
        TreeNode15.Name = "ndForHarvestOfAllGroups"
        TreeNode15.Text = "For harvest of all groups"
        TreeNode16.Name = "ndForConsumptionOfAllGroups"
        TreeNode16.Text = "For consumption of all groups"
        TreeNode17.Name = "ndPrimaryProductionRequired"
        TreeNode17.Text = "Primary production required"
        TreeNode18.Name = "ndImpactData"
        TreeNode18.Text = "Impact data"
        TreeNode19.Name = "ndGraphOfMixedTrophicImpacts"
        TreeNode19.Text = "Graph of mixed trophic impacts"
        TreeNode20.Name = "ndShow/hideGroupsOnGraph"
        TreeNode20.Text = "Show/hide groups on mixed trophic impacts graph"
        TreeNode21.Name = "ndSummaryOfImpactData"
        TreeNode21.Text = "Summary of impact data"
        TreeNode22.Name = "ndMixedTrophicImpact"
        TreeNode22.Text = "Mixed trophic impact"
        TreeNode23.Name = "ndTotal"
        TreeNode23.Text = "Total"
        TreeNode24.Name = "ndByGroup"
        TreeNode24.Text = "By group"
        TreeNode25.Name = "ndAscendency"
        TreeNode25.Text = "Ascendency"
        TreeNode26.Name = "ndFlowFromDetritus"
        TreeNode26.Text = "Flow from detritus"
        TreeNode27.Name = "ndPathway"
        TreeNode27.Text = "Pathway"
        TreeNode28.Name = "ndSummaryOfPathways"
        TreeNode28.Text = "Summary of pathways"
        TreeNode29.Name = "ndConsumer<-TL1"
        TreeNode29.Text = "Consumer <- TL1"
        TreeNode30.Name = "ndPathway"
        TreeNode30.Text = "Pathway"
        TreeNode31.Name = "ndSummaryOfPathways"
        TreeNode31.Text = "Summary of pathways"
        TreeNode32.Name = "ndConsumer<-Prey<-TL1"
        TreeNode32.Text = "Consumer <- prey <- TL1"
        TreeNode33.Name = "ndPathway"
        TreeNode33.Text = "Pathway"
        TreeNode34.Name = "ndSummaryOfPathways"
        TreeNode34.Text = "Summary of pathways"
        TreeNode35.Name = "ndTopPredator<-Prey"
        TreeNode35.Text = "Top predator <- prey"
        TreeNode36.Name = "ndPathway"
        TreeNode36.Text = "Pathway"
        TreeNode37.Name = "ndSummaryOfPathways"
        TreeNode37.Text = "Summary of pathways"
        TreeNode38.Name = "ndCycles(living)"
        TreeNode38.Text = "Cycles (living)"
        TreeNode39.Name = "ndPathway"
        TreeNode39.Text = "Pathway"
        TreeNode40.Name = "ndSummaryOfPathways"
        TreeNode40.Text = "Summary of pathways"
        TreeNode41.Name = "ndCycles(all)"
        TreeNode41.Text = "Cycles (all)"
        TreeNode42.Name = "ndCyclingAndPathLength"
        TreeNode42.Text = "Cycling and path length"
        TreeNode43.Name = "ndCyclesAndPathways"
        TreeNode43.Text = "Cycles and pathways"
        TreeNode44.Name = "ndNetworkAnalysisIndicesInEcosim "
        TreeNode44.Text = "Network analysis indices in Ecosim "
        TreeNode45.ImageIndex = 2
        TreeNode45.Name = "ndEwENetworkAnalysisPlugin"
        TreeNode45.SelectedImageKey = "run.bmp"
        TreeNode45.Text = "EwE Network Analysis Plugin"
        Me.tvNetworkAnalysis.Nodes.AddRange(New System.Windows.Forms.TreeNode() {TreeNode45})
        Me.tvNetworkAnalysis.SelectedImageIndex = 0
        Me.tvNetworkAnalysis.Size = New System.Drawing.Size(180, 360)
        Me.tvNetworkAnalysis.TabIndex = 2
        '
        'imglstNetworkAnalysis
        '
        Me.imglstNetworkAnalysis.ImageStream = CType(resources.GetObject("imglstNetworkAnalysis.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.imglstNetworkAnalysis.TransparentColor = System.Drawing.Color.Transparent
        Me.imglstNetworkAnalysis.Images.SetKeyName(0, "application_get.png")
        Me.imglstNetworkAnalysis.Images.SetKeyName(1, "application_put.png")
        Me.imglstNetworkAnalysis.Images.SetKeyName(2, "run.bmp")
        Me.imglstNetworkAnalysis.Images.SetKeyName(3, "tools.bmp")
        Me.imglstNetworkAnalysis.Images.SetKeyName(4, "Ecopath.bmp")
        Me.imglstNetworkAnalysis.Images.SetKeyName(5, "output_extend.png")
        Me.imglstNetworkAnalysis.Images.SetKeyName(6, "input_extend.png")
        Me.imglstNetworkAnalysis.Images.SetKeyName(7, "wi0064-16.ico")
        Me.imglstNetworkAnalysis.Images.SetKeyName(8, "wi0126-16.ico")
        Me.imglstNetworkAnalysis.Images.SetKeyName(9, "wi0122-16.ico")
        Me.imglstNetworkAnalysis.Images.SetKeyName(10, "wi0054-16.ico")
        '
        'dgvNetworkAnalysis
        '
        Me.dgvNetworkAnalysis.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.dgvNetworkAnalysis.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvNetworkAnalysis.Location = New System.Drawing.Point(0, 28)
        Me.dgvNetworkAnalysis.Name = "dgvNetworkAnalysis"
        Me.dgvNetworkAnalysis.Size = New System.Drawing.Size(708, 439)
        Me.dgvNetworkAnalysis.TabIndex = 3
        '
        'tsNetworkAnalysis
        '
        Me.tsNetworkAnalysis.BackColor = System.Drawing.Color.DarkSeaGreen
        Me.tsNetworkAnalysis.Items.AddRange(New System.Windows.Forms.ToolStripItem() {tslblSelection1, Me.tscmbSelection1, Me.tslblSelection2, Me.tscmbSelection2})
        Me.tsNetworkAnalysis.Location = New System.Drawing.Point(0, 0)
        Me.tsNetworkAnalysis.Name = "tsNetworkAnalysis"
        Me.tsNetworkAnalysis.Size = New System.Drawing.Size(708, 25)
        Me.tsNetworkAnalysis.TabIndex = 4
        Me.tsNetworkAnalysis.Text = "ToolStrip1"
        '
        'tscmbSelection1
        '
        Me.tscmbSelection1.BackColor = System.Drawing.SystemColors.Window
        Me.tscmbSelection1.Name = "tscmbSelection1"
        Me.tscmbSelection1.Size = New System.Drawing.Size(121, 25)
        '
        'tslblSelection2
        '
        Me.tslblSelection2.Name = "tslblSelection2"
        Me.tslblSelection2.Size = New System.Drawing.Size(80, 22)
        Me.tslblSelection2.Text = "ToolStripLabel2"
        '
        'tscmbSelection2
        '
        Me.tscmbSelection2.Name = "tscmbSelection2"
        Me.tscmbSelection2.Size = New System.Drawing.Size(121, 25)
        '
        'lblNetworkAnalysis
        '
        Me.lblNetworkAnalysis.AutoSize = True
        Me.lblNetworkAnalysis.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblNetworkAnalysis.Location = New System.Drawing.Point(3, 9)
        Me.lblNetworkAnalysis.Name = "lblNetworkAnalysis"
        Me.lblNetworkAnalysis.Size = New System.Drawing.Size(240, 20)
        Me.lblNetworkAnalysis.TabIndex = 4
        Me.lblNetworkAnalysis.Text = "EwE Network Analysis Plugin"
        '
        'frmNetworkNav1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.Control
        Me.ClientSize = New System.Drawing.Size(906, 510)
        Me.Controls.Add(Me.lblNetworkAnalysis)
        Me.Controls.Add(Me.scNetworkAnalysis)
        Me.Name = "frmNetworkNav1"
        Me.TabText = "EwE Network Analysis Plugin"
        Me.Text = "EwE Network Analysis Plugin"
        Me.scNetworkAnalysis.Panel1.ResumeLayout(False)
        Me.scNetworkAnalysis.Panel2.ResumeLayout(False)
        Me.scNetworkAnalysis.Panel2.PerformLayout()
        Me.scNetworkAnalysis.ResumeLayout(False)
        CType(Me.dgvNetworkAnalysis, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tsNetworkAnalysis.ResumeLayout(False)
        Me.tsNetworkAnalysis.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents scNetworkAnalysis As System.Windows.Forms.SplitContainer
    Friend WithEvents tvNetworkAnalysis As System.Windows.Forms.TreeView
    Friend WithEvents dgvNetworkAnalysis As System.Windows.Forms.DataGridView
    Friend WithEvents lblNetworkAnalysis As System.Windows.Forms.Label
    Friend WithEvents tsNetworkAnalysis As System.Windows.Forms.ToolStrip
    Friend WithEvents imglstNetworkAnalysis As System.Windows.Forms.ImageList
    Friend WithEvents tscmbSelection1 As System.Windows.Forms.ToolStripComboBox
    Friend WithEvents tslblSelection2 As System.Windows.Forms.ToolStripLabel
    Friend WithEvents tscmbSelection2 As System.Windows.Forms.ToolStripComboBox
End Class
