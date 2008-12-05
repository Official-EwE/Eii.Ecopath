Imports WeifenLuo.WinFormsUI.Docking

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmNetworkAnalysis
    Inherits DockContent

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
        Dim TreeNode1 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Relative flows", 1, 1)
        Dim TreeNode2 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Absolute flows", 1, 1)
        Dim TreeNode3 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Trophic level decomposition", 5, 5, New System.Windows.Forms.TreeNode() {TreeNode1, TreeNode2})
        Dim TreeNode4 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("From primary producers", 1, 1)
        Dim TreeNode5 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("From detritus", 1, 1)
        Dim TreeNode6 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("From all combined", 1, 1)
        Dim TreeNode7 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Transfer efficiency", 1, 1)
        Dim TreeNode8 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Flow pyramid", 1, 1)
        Dim TreeNode9 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Biomass by trophic level", 1, 1)
        Dim TreeNode10 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Biomass pyramid", 1, 1)
        Dim TreeNode11 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Catch by trophic level", 1, 1)
        Dim TreeNode12 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Catch pyramid", 1, 1)
        Dim TreeNode13 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Summary of flow data", 5, 5, New System.Windows.Forms.TreeNode() {TreeNode7, TreeNode8, TreeNode9, TreeNode10, TreeNode11, TreeNode12})
        Dim TreeNode14 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Flows and biomasses", 5, 5, New System.Windows.Forms.TreeNode() {TreeNode4, TreeNode5, TreeNode6, TreeNode13})
        Dim TreeNode15 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("For harvest of all groups", 1, 1)
        Dim TreeNode16 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("For consumption of all groups", 1, 1)
        Dim TreeNode17 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Primary production required", 5, 5, New System.Windows.Forms.TreeNode() {TreeNode15, TreeNode16})
        Dim TreeNode18 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Impact data", 1, 1)
        Dim TreeNode19 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Graph of mixed trophic impact", 1, 1)
        Dim TreeNode20 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Mixed trophic impact", 5, 5, New System.Windows.Forms.TreeNode() {TreeNode18, TreeNode19})
        Dim TreeNode21 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Total", 1, 1)
        Dim TreeNode22 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("By group", 1, 1)
        Dim TreeNode23 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Ascendency", 5, 5, New System.Windows.Forms.TreeNode() {TreeNode21, TreeNode22})
        Dim TreeNode24 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Flow from detritus", 1, 1)
        Dim TreeNode25 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Pathway", 1, 1)
        Dim TreeNode26 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Summary of pathways", 1, 1)
        Dim TreeNode27 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Consumer <- TL1", 5, 5, New System.Windows.Forms.TreeNode() {TreeNode25, TreeNode26})
        Dim TreeNode28 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Pathway", 1, 1)
        Dim TreeNode29 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Summary of pathways", 1, 1)
        Dim TreeNode30 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Consumer <- prey <- TL1", 5, 5, New System.Windows.Forms.TreeNode() {TreeNode28, TreeNode29})
        Dim TreeNode31 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Pathway", 1, 1)
        Dim TreeNode32 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Summary of pathways", 1, 1)
        Dim TreeNode33 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Top predator <- prey", 5, 5, New System.Windows.Forms.TreeNode() {TreeNode31, TreeNode32})
        Dim TreeNode34 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Pathway", 1, 1)
        Dim TreeNode35 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Summary of pathways", 1, 1)
        Dim TreeNode36 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Cycles (living)", 5, 5, New System.Windows.Forms.TreeNode() {TreeNode34, TreeNode35})
        Dim TreeNode37 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Pathway", 1, 1)
        Dim TreeNode38 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Summary of pathways", 1, 1)
        Dim TreeNode39 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Cycles (all)", 5, 5, New System.Windows.Forms.TreeNode() {TreeNode37, TreeNode38})
        Dim TreeNode40 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Cycling and path length", 1, 1)
        Dim TreeNode41 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Cycles and pathways", 5, 5, New System.Windows.Forms.TreeNode() {TreeNode27, TreeNode30, TreeNode33, TreeNode36, TreeNode39, TreeNode40})
        Dim TreeNode42 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Indices without primary production required estimate", 1, 1)
        Dim TreeNode43 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Indices with primary production required estimate", 1, 1)
        Dim TreeNode44 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Ecosim network analysis ", 5, 5, New System.Windows.Forms.TreeNode() {TreeNode42, TreeNode43})
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmNetworkAnalysis))
        Me.scNetworkAnalysis = New System.Windows.Forms.SplitContainer
        Me.tvNetworkAnalysis = New System.Windows.Forms.TreeView
        Me.imglstNetworkAnalysis = New System.Windows.Forms.ImageList(Me.components)
        Me.zgcNetworkAnalysis = New ZedGraph.ZedGraphControl
        Me.tsNetworkAnalysis = New System.Windows.Forms.ToolStrip
        Me.tslblSelection1 = New System.Windows.Forms.ToolStripLabel
        Me.tscmbSelection1 = New System.Windows.Forms.ToolStripComboBox
        Me.tslblSelection2 = New System.Windows.Forms.ToolStripLabel
        Me.tscmbSelection2 = New System.Windows.Forms.ToolStripComboBox
        Me.tslblProgressBar = New System.Windows.Forms.ToolStripLabel
        Me.tspgbProgressBar = New System.Windows.Forms.ToolStripProgressBar
        Me.tsbtnCancel = New System.Windows.Forms.ToolStripButton
        Me.tsbtnOutputIndicesCSV = New System.Windows.Forms.ToolStripButton
        Me.tsbtnOutputGraphEMF = New System.Windows.Forms.ToolStripButton
        Me.tsbtnPrintGraph = New System.Windows.Forms.ToolStripButton
        Me.dgvNetworkAnalysis = New System.Windows.Forms.DataGridView
        Me.tlpNetworkAnalysis = New System.Windows.Forms.TableLayoutPanel
        Me.PictureBox1 = New System.Windows.Forms.PictureBox
        Me.lblNetworkAnalysis = New System.Windows.Forms.Label
        Me.scNetworkAnalysis.Panel1.SuspendLayout()
        Me.scNetworkAnalysis.Panel2.SuspendLayout()
        Me.scNetworkAnalysis.SuspendLayout()
        Me.tsNetworkAnalysis.SuspendLayout()
        CType(Me.dgvNetworkAnalysis, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tlpNetworkAnalysis.SuspendLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
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
        Me.scNetworkAnalysis.Panel2.BackColor = System.Drawing.Color.White
        Me.scNetworkAnalysis.Panel2.Controls.Add(Me.zgcNetworkAnalysis)
        Me.scNetworkAnalysis.Panel2.Controls.Add(Me.tsNetworkAnalysis)
        Me.scNetworkAnalysis.Panel2.Controls.Add(Me.dgvNetworkAnalysis)
        Me.scNetworkAnalysis.Panel2.Controls.Add(Me.tlpNetworkAnalysis)
        Me.scNetworkAnalysis.Size = New System.Drawing.Size(895, 467)
        Me.scNetworkAnalysis.SplitterDistance = 270
        Me.scNetworkAnalysis.TabIndex = 3
        '
        'tvNetworkAnalysis
        '
        Me.tvNetworkAnalysis.BackColor = System.Drawing.Color.MintCream
        Me.tvNetworkAnalysis.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tvNetworkAnalysis.ImageIndex = 0
        Me.tvNetworkAnalysis.ImageList = Me.imglstNetworkAnalysis
        Me.tvNetworkAnalysis.Location = New System.Drawing.Point(0, 0)
        Me.tvNetworkAnalysis.Name = "tvNetworkAnalysis"
        TreeNode1.ImageIndex = 1
        TreeNode1.Name = "ndRelativeFlows"
        TreeNode1.SelectedImageIndex = 1
        TreeNode1.Text = "Relative flows"
        TreeNode2.ImageIndex = 1
        TreeNode2.Name = "ndAbsoluteFlows"
        TreeNode2.SelectedImageIndex = 1
        TreeNode2.Text = "Absolute flows"
        TreeNode3.ImageIndex = 5
        TreeNode3.Name = "ndTrophicLlevelDdecomposition"
        TreeNode3.SelectedImageIndex = 5
        TreeNode3.Text = "Trophic level decomposition"
        TreeNode4.ImageIndex = 1
        TreeNode4.Name = "ndFromPrimaryProducers"
        TreeNode4.SelectedImageIndex = 1
        TreeNode4.Text = "From primary producers"
        TreeNode5.ImageIndex = 1
        TreeNode5.Name = "ndFromDetritus"
        TreeNode5.SelectedImageIndex = 1
        TreeNode5.Text = "From detritus"
        TreeNode6.ImageIndex = 1
        TreeNode6.Name = "ndFromAllCombined"
        TreeNode6.SelectedImageIndex = 1
        TreeNode6.Text = "From all combined"
        TreeNode7.ImageIndex = 1
        TreeNode7.Name = "ndTransferEfficiency"
        TreeNode7.SelectedImageIndex = 1
        TreeNode7.Text = "Transfer efficiency"
        TreeNode8.ImageIndex = 1
        TreeNode8.Name = "ndFlowPyramid"
        TreeNode8.SelectedImageIndex = 1
        TreeNode8.Text = "Flow pyramid"
        TreeNode9.ImageIndex = 1
        TreeNode9.Name = "ndBiomassByTrophicLevel"
        TreeNode9.SelectedImageIndex = 1
        TreeNode9.Text = "Biomass by trophic level"
        TreeNode10.ImageIndex = 1
        TreeNode10.Name = "ndBiomassPyramid"
        TreeNode10.SelectedImageIndex = 1
        TreeNode10.Text = "Biomass pyramid"
        TreeNode11.ImageIndex = 1
        TreeNode11.Name = "ndCatchByTrophicLevel"
        TreeNode11.SelectedImageIndex = 1
        TreeNode11.Text = "Catch by trophic level"
        TreeNode12.ImageIndex = 1
        TreeNode12.Name = "ndCatchPyramid"
        TreeNode12.SelectedImageIndex = 1
        TreeNode12.Text = "Catch pyramid"
        TreeNode13.ImageIndex = 5
        TreeNode13.Name = "ndSummaryOfFlowData"
        TreeNode13.SelectedImageIndex = 5
        TreeNode13.Text = "Summary of flow data"
        TreeNode14.ImageIndex = 5
        TreeNode14.Name = "ndFlowsAndBiomasses"
        TreeNode14.SelectedImageIndex = 5
        TreeNode14.Text = "Flows and biomasses"
        TreeNode15.ImageIndex = 1
        TreeNode15.Name = "ndForHarvestOfAllGroups"
        TreeNode15.SelectedImageIndex = 1
        TreeNode15.Text = "For harvest of all groups"
        TreeNode16.ImageIndex = 1
        TreeNode16.Name = "ndForConsumptionOfAllGroups"
        TreeNode16.SelectedImageIndex = 1
        TreeNode16.Text = "For consumption of all groups"
        TreeNode17.ImageIndex = 5
        TreeNode17.Name = "ndPrimaryProductionRequired"
        TreeNode17.SelectedImageIndex = 5
        TreeNode17.Text = "Primary production required"
        TreeNode18.ImageIndex = 1
        TreeNode18.Name = "ndImpactData"
        TreeNode18.SelectedImageIndex = 1
        TreeNode18.Text = "Impact data"
        TreeNode19.ImageIndex = 1
        TreeNode19.Name = "ndGraphOfMixedTrophicImpacts"
        TreeNode19.SelectedImageIndex = 1
        TreeNode19.Text = "Graph of mixed trophic impact"
        TreeNode20.ImageIndex = 5
        TreeNode20.Name = "ndMixedTrophicImpact"
        TreeNode20.SelectedImageIndex = 5
        TreeNode20.Text = "Mixed trophic impact"
        TreeNode21.ImageIndex = 1
        TreeNode21.Name = "ndTotal"
        TreeNode21.SelectedImageIndex = 1
        TreeNode21.Text = "Total"
        TreeNode22.ImageIndex = 1
        TreeNode22.Name = "ndByGroup"
        TreeNode22.SelectedImageIndex = 1
        TreeNode22.Text = "By group"
        TreeNode23.ImageIndex = 5
        TreeNode23.Name = "ndAscendency"
        TreeNode23.SelectedImageIndex = 5
        TreeNode23.Text = "Ascendency"
        TreeNode24.ImageIndex = 1
        TreeNode24.Name = "ndFlowFromDetritus"
        TreeNode24.SelectedImageIndex = 1
        TreeNode24.Text = "Flow from detritus"
        TreeNode25.ImageIndex = 1
        TreeNode25.Name = "ndPathway"
        TreeNode25.SelectedImageIndex = 1
        TreeNode25.Text = "Pathway"
        TreeNode26.ImageIndex = 1
        TreeNode26.Name = "ndSummaryOfPathways"
        TreeNode26.SelectedImageIndex = 1
        TreeNode26.Text = "Summary of pathways"
        TreeNode27.ImageIndex = 5
        TreeNode27.Name = "ndConsumer<-TL1"
        TreeNode27.SelectedImageIndex = 5
        TreeNode27.Text = "Consumer <- TL1"
        TreeNode28.ImageIndex = 1
        TreeNode28.Name = "ndPathway"
        TreeNode28.SelectedImageIndex = 1
        TreeNode28.Text = "Pathway"
        TreeNode29.ImageIndex = 1
        TreeNode29.Name = "ndSummaryOfPathways"
        TreeNode29.SelectedImageIndex = 1
        TreeNode29.Text = "Summary of pathways"
        TreeNode30.ImageIndex = 5
        TreeNode30.Name = "ndConsumer<-Prey<-TL1"
        TreeNode30.SelectedImageIndex = 5
        TreeNode30.Text = "Consumer <- prey <- TL1"
        TreeNode31.ImageIndex = 1
        TreeNode31.Name = "ndPathway"
        TreeNode31.SelectedImageIndex = 1
        TreeNode31.Text = "Pathway"
        TreeNode32.ImageIndex = 1
        TreeNode32.Name = "ndSummaryOfPathways"
        TreeNode32.SelectedImageIndex = 1
        TreeNode32.Text = "Summary of pathways"
        TreeNode33.ImageIndex = 5
        TreeNode33.Name = "ndTopPredator<-Prey"
        TreeNode33.SelectedImageIndex = 5
        TreeNode33.Text = "Top predator <- prey"
        TreeNode34.ImageIndex = 1
        TreeNode34.Name = "ndPathway"
        TreeNode34.SelectedImageIndex = 1
        TreeNode34.Text = "Pathway"
        TreeNode35.ImageIndex = 1
        TreeNode35.Name = "ndSummaryOfPathways"
        TreeNode35.SelectedImageIndex = 1
        TreeNode35.Text = "Summary of pathways"
        TreeNode36.ImageIndex = 5
        TreeNode36.Name = "ndCycles(living)"
        TreeNode36.SelectedImageIndex = 5
        TreeNode36.Text = "Cycles (living)"
        TreeNode37.ImageIndex = 1
        TreeNode37.Name = "ndPathway"
        TreeNode37.SelectedImageIndex = 1
        TreeNode37.Text = "Pathway"
        TreeNode38.ImageIndex = 1
        TreeNode38.Name = "ndSummaryOfPathways"
        TreeNode38.SelectedImageIndex = 1
        TreeNode38.Text = "Summary of pathways"
        TreeNode39.ImageIndex = 5
        TreeNode39.Name = "ndCycles(all)"
        TreeNode39.SelectedImageIndex = 5
        TreeNode39.Text = "Cycles (all)"
        TreeNode40.ImageIndex = 1
        TreeNode40.Name = "ndCyclingAndPathLength"
        TreeNode40.SelectedImageIndex = 1
        TreeNode40.Text = "Cycling and path length"
        TreeNode41.ImageIndex = 5
        TreeNode41.Name = "ndCyclesAndPathways"
        TreeNode41.SelectedImageIndex = 5
        TreeNode41.Text = "Cycles and pathways"
        TreeNode42.ImageIndex = 1
        TreeNode42.Name = "ndWithoutPrimaryProductionRequiredEstimate"
        TreeNode42.SelectedImageIndex = 1
        TreeNode42.Text = "Indices without primary production required estimate"
        TreeNode43.ImageIndex = 1
        TreeNode43.Name = "ndWithPrimaryProductionRequiredEstimate"
        TreeNode43.SelectedImageIndex = 1
        TreeNode43.Text = "Indices with primary production required estimate"
        TreeNode44.ImageIndex = 5
        TreeNode44.Name = "ndEcosim network analysis indices"
        TreeNode44.SelectedImageIndex = 5
        TreeNode44.Text = "Ecosim network analysis "
        Me.tvNetworkAnalysis.Nodes.AddRange(New System.Windows.Forms.TreeNode() {TreeNode3, TreeNode14, TreeNode17, TreeNode20, TreeNode23, TreeNode24, TreeNode41, TreeNode44})
        Me.tvNetworkAnalysis.SelectedImageIndex = 0
        Me.tvNetworkAnalysis.Size = New System.Drawing.Size(270, 467)
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
        'zgcNetworkAnalysis
        '
        Me.zgcNetworkAnalysis.Location = New System.Drawing.Point(3, 36)
        Me.zgcNetworkAnalysis.Name = "zgcNetworkAnalysis"
        Me.zgcNetworkAnalysis.ScrollGrace = 0
        Me.zgcNetworkAnalysis.ScrollMaxX = 0
        Me.zgcNetworkAnalysis.ScrollMaxY = 0
        Me.zgcNetworkAnalysis.ScrollMaxY2 = 0
        Me.zgcNetworkAnalysis.ScrollMinX = 0
        Me.zgcNetworkAnalysis.ScrollMinY = 0
        Me.zgcNetworkAnalysis.ScrollMinY2 = 0
        Me.zgcNetworkAnalysis.Size = New System.Drawing.Size(574, 425)
        Me.zgcNetworkAnalysis.TabIndex = 7
        '
        'tsNetworkAnalysis
        '
        Me.tsNetworkAnalysis.BackColor = System.Drawing.Color.DarkSeaGreen
        Me.tsNetworkAnalysis.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tslblSelection1, Me.tscmbSelection1, Me.tslblSelection2, Me.tscmbSelection2, Me.tslblProgressBar, Me.tspgbProgressBar, Me.tsbtnCancel, Me.tsbtnOutputIndicesCSV, Me.tsbtnOutputGraphEMF, Me.tsbtnPrintGraph})
        Me.tsNetworkAnalysis.Location = New System.Drawing.Point(0, 0)
        Me.tsNetworkAnalysis.Name = "tsNetworkAnalysis"
        Me.tsNetworkAnalysis.Size = New System.Drawing.Size(621, 25)
        Me.tsNetworkAnalysis.TabIndex = 4
        Me.tsNetworkAnalysis.Text = "ToolStrip1"
        '
        'tslblSelection1
        '
        Me.tslblSelection1.Name = "tslblSelection1"
        Me.tslblSelection1.Size = New System.Drawing.Size(80, 22)
        Me.tslblSelection1.Text = "ToolStripLabel1"
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
        'tslblProgressBar
        '
        Me.tslblProgressBar.Name = "tslblProgressBar"
        Me.tslblProgressBar.Size = New System.Drawing.Size(80, 22)
        Me.tslblProgressBar.Text = "ToolStripLabel3"
        '
        'tspgbProgressBar
        '
        Me.tspgbProgressBar.Name = "tspgbProgressBar"
        Me.tspgbProgressBar.Size = New System.Drawing.Size(100, 22)
        '
        'tsbtnCancel
        '
        Me.tsbtnCancel.BackColor = System.Drawing.Color.DarkSeaGreen
        Me.tsbtnCancel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.tsbtnCancel.Image = CType(resources.GetObject("tsbtnCancel.Image"), System.Drawing.Image)
        Me.tsbtnCancel.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tsbtnCancel.Name = "tsbtnCancel"
        Me.tsbtnCancel.Size = New System.Drawing.Size(43, 17)
        Me.tsbtnCancel.Text = "Cancel"
        '
        'tsbtnOutputIndicesCSV
        '
        Me.tsbtnOutputIndicesCSV.BackColor = System.Drawing.Color.DarkSeaGreen
        Me.tsbtnOutputIndicesCSV.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.tsbtnOutputIndicesCSV.Image = CType(resources.GetObject("tsbtnOutputIndicesCSV.Image"), System.Drawing.Image)
        Me.tsbtnOutputIndicesCSV.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tsbtnOutputIndicesCSV.Name = "tsbtnOutputIndicesCSV"
        Me.tsbtnOutputIndicesCSV.Size = New System.Drawing.Size(145, 17)
        Me.tsbtnOutputIndicesCSV.Text = "Output all indices to CSV file"
        '
        'tsbtnOutputGraphEMF
        '
        Me.tsbtnOutputGraphEMF.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.tsbtnOutputGraphEMF.Image = CType(resources.GetObject("tsbtnOutputGraphEMF.Image"), System.Drawing.Image)
        Me.tsbtnOutputGraphEMF.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tsbtnOutputGraphEMF.Name = "tsbtnOutputGraphEMF"
        Me.tsbtnOutputGraphEMF.Size = New System.Drawing.Size(88, 17)
        Me.tsbtnOutputGraphEMF.Text = "Save to EMF file"
        '
        'tsbtnPrintGraph
        '
        Me.tsbtnPrintGraph.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.tsbtnPrintGraph.Image = CType(resources.GetObject("tsbtnPrintGraph.Image"), System.Drawing.Image)
        Me.tsbtnPrintGraph.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tsbtnPrintGraph.Name = "tsbtnPrintGraph"
        Me.tsbtnPrintGraph.Size = New System.Drawing.Size(64, 17)
        Me.tsbtnPrintGraph.Text = "Print graph"
        '
        'dgvNetworkAnalysis
        '
        Me.dgvNetworkAnalysis.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.dgvNetworkAnalysis.BackgroundColor = System.Drawing.SystemColors.ControlLightLight
        Me.dgvNetworkAnalysis.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvNetworkAnalysis.Location = New System.Drawing.Point(0, 28)
        Me.dgvNetworkAnalysis.Name = "dgvNetworkAnalysis"
        Me.dgvNetworkAnalysis.Size = New System.Drawing.Size(565, 439)
        Me.dgvNetworkAnalysis.TabIndex = 3
        '
        'tlpNetworkAnalysis
        '
        Me.tlpNetworkAnalysis.ColumnCount = 3
        Me.tlpNetworkAnalysis.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.tlpNetworkAnalysis.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 210.0!))
        Me.tlpNetworkAnalysis.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.tlpNetworkAnalysis.Controls.Add(Me.PictureBox1, 1, 1)
        Me.tlpNetworkAnalysis.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tlpNetworkAnalysis.Location = New System.Drawing.Point(0, 0)
        Me.tlpNetworkAnalysis.Name = "tlpNetworkAnalysis"
        Me.tlpNetworkAnalysis.RowCount = 3
        Me.tlpNetworkAnalysis.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.tlpNetworkAnalysis.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 400.0!))
        Me.tlpNetworkAnalysis.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.tlpNetworkAnalysis.Size = New System.Drawing.Size(621, 467)
        Me.tlpNetworkAnalysis.TabIndex = 5
        '
        'PictureBox1
        '
        Me.PictureBox1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PictureBox1.Image = Global.EwENetworkAnalysis.My.Resources.Resources.N_Asponsors
        Me.PictureBox1.Location = New System.Drawing.Point(208, 36)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(204, 394)
        Me.PictureBox1.TabIndex = 0
        Me.PictureBox1.TabStop = False
        '
        'lblNetworkAnalysis
        '
        Me.lblNetworkAnalysis.AutoSize = True
        Me.lblNetworkAnalysis.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblNetworkAnalysis.Location = New System.Drawing.Point(3, 9)
        Me.lblNetworkAnalysis.Name = "lblNetworkAnalysis"
        Me.lblNetworkAnalysis.Size = New System.Drawing.Size(202, 20)
        Me.lblNetworkAnalysis.TabIndex = 4
        Me.lblNetworkAnalysis.Text = "Network analysis plug-in"
        '
        'frmNetworkAnalysis
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.Control
        Me.ClientSize = New System.Drawing.Size(906, 510)
        Me.Controls.Add(Me.lblNetworkAnalysis)
        Me.Controls.Add(Me.scNetworkAnalysis)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmNetworkAnalysis"
        Me.TabText = "Network analysis plug-in"
        Me.Text = "Networkanalysis plug-in"
        Me.scNetworkAnalysis.Panel1.ResumeLayout(False)
        Me.scNetworkAnalysis.Panel2.ResumeLayout(False)
        Me.scNetworkAnalysis.Panel2.PerformLayout()
        Me.scNetworkAnalysis.ResumeLayout(False)
        Me.tsNetworkAnalysis.ResumeLayout(False)
        Me.tsNetworkAnalysis.PerformLayout()
        CType(Me.dgvNetworkAnalysis, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tlpNetworkAnalysis.ResumeLayout(False)
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
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
    Friend WithEvents tslblProgressBar As System.Windows.Forms.ToolStripLabel
    Friend WithEvents tspgbProgressBar As System.Windows.Forms.ToolStripProgressBar
    Friend WithEvents tslblSelection1 As System.Windows.Forms.ToolStripLabel
    Friend WithEvents tsbtnCancel As System.Windows.Forms.ToolStripButton
    Friend WithEvents tlpNetworkAnalysis As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents PictureBox1 As System.Windows.Forms.PictureBox
    Friend WithEvents tsbtnOutputIndicesCSV As System.Windows.Forms.ToolStripButton
    Friend WithEvents zgcNetworkAnalysis As ZedGraph.ZedGraphControl
    Friend WithEvents tsbtnOutputGraphEMF As System.Windows.Forms.ToolStripButton
    Friend WithEvents tsbtnPrintGraph As System.Windows.Forms.ToolStripButton
End Class
