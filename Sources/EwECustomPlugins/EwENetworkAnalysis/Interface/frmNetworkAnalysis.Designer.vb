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
        Dim TreeNode46 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Relative flows", 1, 1)
        Dim TreeNode47 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Absolute flows", 1, 1)
        Dim TreeNode48 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Trophic level decomposition", 5, 5, New System.Windows.Forms.TreeNode() {TreeNode46, TreeNode47})
        Dim TreeNode49 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("From primary producers", 1, 1)
        Dim TreeNode50 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("From detritus", 1, 1)
        Dim TreeNode51 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("From all combined", 1, 1)
        Dim TreeNode52 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Transfer efficiency", 1, 1)
        Dim TreeNode53 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Flow pyramid", 1, 1)
        Dim TreeNode54 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Biomass by trophic level", 1, 1)
        Dim TreeNode55 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Biomass pyramid", 1, 1)
        Dim TreeNode56 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Catch by trophic level", 1, 1)
        Dim TreeNode57 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Catch pyramid", 1, 1)
        Dim TreeNode58 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Summary of flow data", 5, 5, New System.Windows.Forms.TreeNode() {TreeNode52, TreeNode53, TreeNode54, TreeNode55, TreeNode56, TreeNode57})
        Dim TreeNode59 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Flows and biomasses", 5, 5, New System.Windows.Forms.TreeNode() {TreeNode49, TreeNode50, TreeNode51, TreeNode58})
        Dim TreeNode60 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("For harvest of all groups", 1, 1)
        Dim TreeNode61 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("For consumption of all groups", 1, 1)
        Dim TreeNode62 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Primary production required", 5, 5, New System.Windows.Forms.TreeNode() {TreeNode60, TreeNode61})
        Dim TreeNode63 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Impact data", 1, 1)
        Dim TreeNode64 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Graph of mixed trophic impact", 1, 1)
        Dim TreeNode65 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Mixed trophic impact", 5, 5, New System.Windows.Forms.TreeNode() {TreeNode63, TreeNode64})
        Dim TreeNode66 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Total", 1, 1)
        Dim TreeNode67 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("By group", 1, 1)
        Dim TreeNode68 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Ascendency", 5, 5, New System.Windows.Forms.TreeNode() {TreeNode66, TreeNode67})
        Dim TreeNode69 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Flow from detritus", 1, 1)
        Dim TreeNode70 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Pathway", 1, 1)
        Dim TreeNode71 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Summary of pathways", 1, 1)
        Dim TreeNode72 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Consumer <- TL1", 5, 5, New System.Windows.Forms.TreeNode() {TreeNode70, TreeNode71})
        Dim TreeNode73 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Pathway", 1, 1)
        Dim TreeNode74 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Summary of pathways", 1, 1)
        Dim TreeNode75 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Consumer <- prey <- TL1", 5, 5, New System.Windows.Forms.TreeNode() {TreeNode73, TreeNode74})
        Dim TreeNode76 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Pathway", 1, 1)
        Dim TreeNode77 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Summary of pathways", 1, 1)
        Dim TreeNode78 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Top predator <- prey", 5, 5, New System.Windows.Forms.TreeNode() {TreeNode76, TreeNode77})
        Dim TreeNode79 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Pathway", 1, 1)
        Dim TreeNode80 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Summary of pathways", 1, 1)
        Dim TreeNode81 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Cycles (living)", 5, 5, New System.Windows.Forms.TreeNode() {TreeNode79, TreeNode80})
        Dim TreeNode82 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Pathway", 1, 1)
        Dim TreeNode83 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Summary of pathways", 1, 1)
        Dim TreeNode84 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Cycles (all)", 5, 5, New System.Windows.Forms.TreeNode() {TreeNode82, TreeNode83})
        Dim TreeNode85 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Cycling and path length", 1, 1)
        Dim TreeNode86 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Cycles and pathways", 5, 5, New System.Windows.Forms.TreeNode() {TreeNode72, TreeNode75, TreeNode78, TreeNode81, TreeNode84, TreeNode85})
        Dim TreeNode87 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Without primary production required estimate", 1, 1)
        Dim TreeNode88 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("With primary production required estimate", 1, 1)
        Dim TreeNode89 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Ecosim network analysis indices ", 5, 5, New System.Windows.Forms.TreeNode() {TreeNode87, TreeNode88})
        Dim TreeNode90 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Functional response", 1, 1)
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
        TreeNode46.ImageIndex = 1
        TreeNode46.Name = "ndRelativeFlows"
        TreeNode46.SelectedImageIndex = 1
        TreeNode46.Text = "Relative flows"
        TreeNode47.ImageIndex = 1
        TreeNode47.Name = "ndAbsoluteFlows"
        TreeNode47.SelectedImageIndex = 1
        TreeNode47.Text = "Absolute flows"
        TreeNode48.ImageIndex = 5
        TreeNode48.Name = "ndTrophicLlevelDdecomposition"
        TreeNode48.SelectedImageIndex = 5
        TreeNode48.Text = "Trophic level decomposition"
        TreeNode49.ImageIndex = 1
        TreeNode49.Name = "ndFromPrimaryProducers"
        TreeNode49.SelectedImageIndex = 1
        TreeNode49.Text = "From primary producers"
        TreeNode50.ImageIndex = 1
        TreeNode50.Name = "ndFromDetritus"
        TreeNode50.SelectedImageIndex = 1
        TreeNode50.Text = "From detritus"
        TreeNode51.ImageIndex = 1
        TreeNode51.Name = "ndFromAllCombined"
        TreeNode51.SelectedImageIndex = 1
        TreeNode51.Text = "From all combined"
        TreeNode52.ImageIndex = 1
        TreeNode52.Name = "ndTransferEfficiency"
        TreeNode52.SelectedImageIndex = 1
        TreeNode52.Text = "Transfer efficiency"
        TreeNode53.ImageIndex = 1
        TreeNode53.Name = "ndFlowPyramid"
        TreeNode53.SelectedImageIndex = 1
        TreeNode53.Text = "Flow pyramid"
        TreeNode54.ImageIndex = 1
        TreeNode54.Name = "ndBiomassByTrophicLevel"
        TreeNode54.SelectedImageIndex = 1
        TreeNode54.Text = "Biomass by trophic level"
        TreeNode55.ImageIndex = 1
        TreeNode55.Name = "ndBiomassPyramid"
        TreeNode55.SelectedImageIndex = 1
        TreeNode55.Text = "Biomass pyramid"
        TreeNode56.ImageIndex = 1
        TreeNode56.Name = "ndCatchByTrophicLevel"
        TreeNode56.SelectedImageIndex = 1
        TreeNode56.Text = "Catch by trophic level"
        TreeNode57.ImageIndex = 1
        TreeNode57.Name = "ndCatchPyramid"
        TreeNode57.SelectedImageIndex = 1
        TreeNode57.Text = "Catch pyramid"
        TreeNode58.ImageIndex = 5
        TreeNode58.Name = "ndSummaryOfFlowData"
        TreeNode58.SelectedImageIndex = 5
        TreeNode58.Text = "Summary of flow data"
        TreeNode59.ImageIndex = 5
        TreeNode59.Name = "ndFlowsAndBiomasses"
        TreeNode59.SelectedImageIndex = 5
        TreeNode59.Text = "Flows and biomasses"
        TreeNode60.ImageIndex = 1
        TreeNode60.Name = "ndForHarvestOfAllGroups"
        TreeNode60.SelectedImageIndex = 1
        TreeNode60.Text = "For harvest of all groups"
        TreeNode61.ImageIndex = 1
        TreeNode61.Name = "ndForConsumptionOfAllGroups"
        TreeNode61.SelectedImageIndex = 1
        TreeNode61.Text = "For consumption of all groups"
        TreeNode62.ImageIndex = 5
        TreeNode62.Name = "ndPrimaryProductionRequired"
        TreeNode62.SelectedImageIndex = 5
        TreeNode62.Text = "Primary production required"
        TreeNode63.ImageIndex = 1
        TreeNode63.Name = "ndImpactData"
        TreeNode63.SelectedImageIndex = 1
        TreeNode63.Text = "Impact data"
        TreeNode64.ImageIndex = 1
        TreeNode64.Name = "ndGraphOfMixedTrophicImpacts"
        TreeNode64.SelectedImageIndex = 1
        TreeNode64.Text = "Graph of mixed trophic impact"
        TreeNode65.ImageIndex = 5
        TreeNode65.Name = "ndMixedTrophicImpact"
        TreeNode65.SelectedImageIndex = 5
        TreeNode65.Text = "Mixed trophic impact"
        TreeNode66.ImageIndex = 1
        TreeNode66.Name = "ndTotal"
        TreeNode66.SelectedImageIndex = 1
        TreeNode66.Text = "Total"
        TreeNode67.ImageIndex = 1
        TreeNode67.Name = "ndByGroup"
        TreeNode67.SelectedImageIndex = 1
        TreeNode67.Text = "By group"
        TreeNode68.ImageIndex = 5
        TreeNode68.Name = "ndAscendency"
        TreeNode68.SelectedImageIndex = 5
        TreeNode68.Text = "Ascendency"
        TreeNode69.ImageIndex = 1
        TreeNode69.Name = "ndFlowFromDetritus"
        TreeNode69.SelectedImageIndex = 1
        TreeNode69.Text = "Flow from detritus"
        TreeNode70.ImageIndex = 1
        TreeNode70.Name = "ndPathway"
        TreeNode70.SelectedImageIndex = 1
        TreeNode70.Text = "Pathway"
        TreeNode71.ImageIndex = 1
        TreeNode71.Name = "ndSummaryOfPathways"
        TreeNode71.SelectedImageIndex = 1
        TreeNode71.Text = "Summary of pathways"
        TreeNode72.ImageIndex = 5
        TreeNode72.Name = "ndConsumer<-TL1"
        TreeNode72.SelectedImageIndex = 5
        TreeNode72.Text = "Consumer <- TL1"
        TreeNode73.ImageIndex = 1
        TreeNode73.Name = "ndPathway"
        TreeNode73.SelectedImageIndex = 1
        TreeNode73.Text = "Pathway"
        TreeNode74.ImageIndex = 1
        TreeNode74.Name = "ndSummaryOfPathways"
        TreeNode74.SelectedImageIndex = 1
        TreeNode74.Text = "Summary of pathways"
        TreeNode75.ImageIndex = 5
        TreeNode75.Name = "ndConsumer<-Prey<-TL1"
        TreeNode75.SelectedImageIndex = 5
        TreeNode75.Text = "Consumer <- prey <- TL1"
        TreeNode76.ImageIndex = 1
        TreeNode76.Name = "ndPathway"
        TreeNode76.SelectedImageIndex = 1
        TreeNode76.Text = "Pathway"
        TreeNode77.ImageIndex = 1
        TreeNode77.Name = "ndSummaryOfPathways"
        TreeNode77.SelectedImageIndex = 1
        TreeNode77.Text = "Summary of pathways"
        TreeNode78.ImageIndex = 5
        TreeNode78.Name = "ndTopPredator<-Prey"
        TreeNode78.SelectedImageIndex = 5
        TreeNode78.Text = "Top predator <- prey"
        TreeNode79.ImageIndex = 1
        TreeNode79.Name = "ndPathway"
        TreeNode79.SelectedImageIndex = 1
        TreeNode79.Text = "Pathway"
        TreeNode80.ImageIndex = 1
        TreeNode80.Name = "ndSummaryOfPathways"
        TreeNode80.SelectedImageIndex = 1
        TreeNode80.Text = "Summary of pathways"
        TreeNode81.ImageIndex = 5
        TreeNode81.Name = "ndCycles(living)"
        TreeNode81.SelectedImageIndex = 5
        TreeNode81.Text = "Cycles (living)"
        TreeNode82.ImageIndex = 1
        TreeNode82.Name = "ndPathway"
        TreeNode82.SelectedImageIndex = 1
        TreeNode82.Text = "Pathway"
        TreeNode83.ImageIndex = 1
        TreeNode83.Name = "ndSummaryOfPathways"
        TreeNode83.SelectedImageIndex = 1
        TreeNode83.Text = "Summary of pathways"
        TreeNode84.ImageIndex = 5
        TreeNode84.Name = "ndCycles(all)"
        TreeNode84.SelectedImageIndex = 5
        TreeNode84.Text = "Cycles (all)"
        TreeNode85.ImageIndex = 1
        TreeNode85.Name = "ndCyclingAndPathLength"
        TreeNode85.SelectedImageIndex = 1
        TreeNode85.Text = "Cycling and path length"
        TreeNode86.ImageIndex = 5
        TreeNode86.Name = "ndCyclesAndPathways"
        TreeNode86.SelectedImageIndex = 5
        TreeNode86.Text = "Cycles and pathways"
        TreeNode87.ImageIndex = 1
        TreeNode87.Name = "ndWithoutPrimaryProductionRequiredEstimate"
        TreeNode87.SelectedImageIndex = 1
        TreeNode87.Text = "Without primary production required estimate"
        TreeNode88.ImageIndex = 1
        TreeNode88.Name = "ndWithPrimaryProductionRequiredEstimate"
        TreeNode88.SelectedImageIndex = 1
        TreeNode88.Text = "With primary production required estimate"
        TreeNode89.ImageIndex = 5
        TreeNode89.Name = "ndEcosim network analysis indices"
        TreeNode89.SelectedImageIndex = 5
        TreeNode89.Text = "Ecosim network analysis indices "
        TreeNode90.ImageIndex = 1
        TreeNode90.Name = "ndFunctionalResponse"
        TreeNode90.SelectedImageIndex = 1
        TreeNode90.Text = "Functional response"
        Me.tvNetworkAnalysis.Nodes.AddRange(New System.Windows.Forms.TreeNode() {TreeNode48, TreeNode59, TreeNode62, TreeNode65, TreeNode68, TreeNode69, TreeNode86, TreeNode89, TreeNode90})
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
