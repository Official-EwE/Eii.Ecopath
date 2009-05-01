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
        Dim TreeNode19 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Plot of mixed trophic impact")
        Dim TreeNode20 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Ecopath5 plot of mixed trophic impact", 1, 1)
        Dim TreeNode21 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Mixed trophic impact", 5, 5, New System.Windows.Forms.TreeNode() {TreeNode18, TreeNode19, TreeNode20})
        Dim TreeNode22 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Total", 1, 1)
        Dim TreeNode23 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("By group", 1, 1)
        Dim TreeNode24 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Ascendency", 5, 5, New System.Windows.Forms.TreeNode() {TreeNode22, TreeNode23})
        Dim TreeNode25 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Flow from detritus", 1, 1)
        Dim TreeNode26 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Pathway", 1, 1)
        Dim TreeNode27 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Summary of pathways", 1, 1)
        Dim TreeNode28 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Consumer <- TL1", 5, 5, New System.Windows.Forms.TreeNode() {TreeNode26, TreeNode27})
        Dim TreeNode29 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Pathway", 1, 1)
        Dim TreeNode30 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Summary of pathways", 1, 1)
        Dim TreeNode31 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Consumer <- prey <- TL1", 5, 5, New System.Windows.Forms.TreeNode() {TreeNode29, TreeNode30})
        Dim TreeNode32 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Pathway", 1, 1)
        Dim TreeNode33 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Summary of pathways", 1, 1)
        Dim TreeNode34 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Top predator <- prey", 5, 5, New System.Windows.Forms.TreeNode() {TreeNode32, TreeNode33})
        Dim TreeNode35 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Pathway", 1, 1)
        Dim TreeNode36 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Summary of pathways", 1, 1)
        Dim TreeNode37 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Cycles (living)", 5, 5, New System.Windows.Forms.TreeNode() {TreeNode35, TreeNode36})
        Dim TreeNode38 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Pathway", 1, 1)
        Dim TreeNode39 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Summary of pathways", 1, 1)
        Dim TreeNode40 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Cycles (all)", 5, 5, New System.Windows.Forms.TreeNode() {TreeNode38, TreeNode39})
        Dim TreeNode41 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Cycling and path length", 1, 1)
        Dim TreeNode42 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Cycles and pathways", 5, 5, New System.Windows.Forms.TreeNode() {TreeNode28, TreeNode31, TreeNode34, TreeNode37, TreeNode40, TreeNode41})
        Dim TreeNode43 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Indices without primary production required estimate", 1, 1)
        Dim TreeNode44 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Indices with primary production required estimate", 1, 1)
        Dim TreeNode45 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Ecosim network analysis ", 5, 5, New System.Windows.Forms.TreeNode() {TreeNode43, TreeNode44})
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmNetworkAnalysis))
        Me.scNetworkAnalysis = New System.Windows.Forms.SplitContainer
        Me.tvNetworkAnalysis = New System.Windows.Forms.TreeView
        Me.imglstNetworkAnalysis = New System.Windows.Forms.ImageList(Me.components)
        Me.m_graph = New ZedGraph.ZedGraphControl
        Me.m_plot = New EwENetworkAnalysis.ucPlot
        Me.m_tsNetworkAnalysis = New System.Windows.Forms.ToolStrip
        Me.tslblSelection1 = New System.Windows.Forms.ToolStripLabel
        Me.tscmbSelection1 = New System.Windows.Forms.ToolStripComboBox
        Me.tslblSelection2 = New System.Windows.Forms.ToolStripLabel
        Me.tscmbSelection2 = New System.Windows.Forms.ToolStripComboBox
        Me.tslblProgressBar = New System.Windows.Forms.ToolStripLabel
        Me.tsbtnCancel = New System.Windows.Forms.ToolStripButton
        Me.tsbtnOutputIndicesCSV = New System.Windows.Forms.ToolStripButton
        Me.tsbtnOutputGraphEMF = New System.Windows.Forms.ToolStripButton
        Me.tsbtnPrintGraph = New System.Windows.Forms.ToolStripButton
        Me.tssepGraphMTI = New System.Windows.Forms.ToolStripSeparator
        Me.tsbtnGraphMTI = New System.Windows.Forms.ToolStripButton
        Me.m_datagrid = New System.Windows.Forms.DataGridView
        Me.m_tlpInfo = New System.Windows.Forms.TableLayoutPanel
        Me.PictureBox1 = New System.Windows.Forms.PictureBox
        Me.lblNetworkAnalysis = New System.Windows.Forms.Label
        Me.scNetworkAnalysis.Panel1.SuspendLayout()
        Me.scNetworkAnalysis.Panel2.SuspendLayout()
        Me.scNetworkAnalysis.SuspendLayout()
        Me.m_tsNetworkAnalysis.SuspendLayout()
        CType(Me.m_datagrid, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.m_tlpInfo.SuspendLayout()
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
        Me.scNetworkAnalysis.Panel2.Controls.Add(Me.m_graph)
        Me.scNetworkAnalysis.Panel2.Controls.Add(Me.m_plot)
        Me.scNetworkAnalysis.Panel2.Controls.Add(Me.m_tsNetworkAnalysis)
        Me.scNetworkAnalysis.Panel2.Controls.Add(Me.m_datagrid)
        Me.scNetworkAnalysis.Panel2.Controls.Add(Me.m_tlpInfo)
        Me.scNetworkAnalysis.Size = New System.Drawing.Size(895, 467)
        Me.scNetworkAnalysis.SplitterDistance = 226
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
        TreeNode19.ImageKey = "application_put.png"
        TreeNode19.Name = "ndGraphOfMixedTrophicImpact"
        TreeNode19.NodeFont = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        TreeNode19.SelectedImageKey = "application_put.png"
        TreeNode19.Text = "Plot of mixed trophic impact"
        TreeNode20.ImageIndex = 1
        TreeNode20.Name = "ndGraphOfMixedTrophicImpactEwE5"
        TreeNode20.NodeFont = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        TreeNode20.SelectedImageIndex = 1
        TreeNode20.Text = "Ecopath5 plot of mixed trophic impact"
        TreeNode21.ImageIndex = 5
        TreeNode21.Name = "ndMixedTrophicImpact"
        TreeNode21.SelectedImageIndex = 5
        TreeNode21.Text = "Mixed trophic impact"
        TreeNode22.ImageIndex = 1
        TreeNode22.Name = "ndTotal"
        TreeNode22.SelectedImageIndex = 1
        TreeNode22.Text = "Total"
        TreeNode23.ImageIndex = 1
        TreeNode23.Name = "ndByGroup"
        TreeNode23.SelectedImageIndex = 1
        TreeNode23.Text = "By group"
        TreeNode24.ImageIndex = 5
        TreeNode24.Name = "ndAscendency"
        TreeNode24.SelectedImageIndex = 5
        TreeNode24.Text = "Ascendency"
        TreeNode25.ImageIndex = 1
        TreeNode25.Name = "ndFlowFromDetritus"
        TreeNode25.SelectedImageIndex = 1
        TreeNode25.Text = "Flow from detritus"
        TreeNode26.ImageIndex = 1
        TreeNode26.Name = "ndPathway_cons_tl1"
        TreeNode26.SelectedImageIndex = 1
        TreeNode26.Text = "Pathway"
        TreeNode27.ImageIndex = 1
        TreeNode27.Name = "ndSummaryOfPathways_cons_tl1"
        TreeNode27.SelectedImageIndex = 1
        TreeNode27.Text = "Summary of pathways"
        TreeNode28.ImageIndex = 5
        TreeNode28.Name = "ndConsumer<-TL1"
        TreeNode28.SelectedImageIndex = 5
        TreeNode28.Text = "Consumer <- TL1"
        TreeNode29.ImageIndex = 1
        TreeNode29.Name = "ndPathway_cons_prey_tl1"
        TreeNode29.SelectedImageIndex = 1
        TreeNode29.Text = "Pathway"
        TreeNode30.ImageIndex = 1
        TreeNode30.Name = "ndSummaryOfPathways_cons_prey_tl1"
        TreeNode30.SelectedImageIndex = 1
        TreeNode30.Text = "Summary of pathways"
        TreeNode31.ImageIndex = 5
        TreeNode31.Name = "ndConsumer<-Prey<-TL1"
        TreeNode31.SelectedImageIndex = 5
        TreeNode31.Text = "Consumer <- prey <- TL1"
        TreeNode32.ImageIndex = 1
        TreeNode32.Name = "ndPathway_pred_prey"
        TreeNode32.SelectedImageIndex = 1
        TreeNode32.Text = "Pathway"
        TreeNode33.ImageIndex = 1
        TreeNode33.Name = "ndSummaryOfPathways_pred_prey"
        TreeNode33.SelectedImageIndex = 1
        TreeNode33.Text = "Summary of pathways"
        TreeNode34.ImageIndex = 5
        TreeNode34.Name = "ndTopPredator<-Prey"
        TreeNode34.SelectedImageIndex = 5
        TreeNode34.Text = "Top predator <- prey"
        TreeNode35.ImageIndex = 1
        TreeNode35.Name = "ndPathway_living"
        TreeNode35.SelectedImageIndex = 1
        TreeNode35.Text = "Pathway"
        TreeNode36.ImageIndex = 1
        TreeNode36.Name = "ndSummaryOfPathways_living"
        TreeNode36.SelectedImageIndex = 1
        TreeNode36.Text = "Summary of pathways"
        TreeNode37.ImageIndex = 5
        TreeNode37.Name = "ndCycles(living)"
        TreeNode37.SelectedImageIndex = 5
        TreeNode37.Text = "Cycles (living)"
        TreeNode38.ImageIndex = 1
        TreeNode38.Name = "ndPathway_all"
        TreeNode38.SelectedImageIndex = 1
        TreeNode38.Text = "Pathway"
        TreeNode39.ImageIndex = 1
        TreeNode39.Name = "ndSummaryOfPathways_all"
        TreeNode39.SelectedImageIndex = 1
        TreeNode39.Text = "Summary of pathways"
        TreeNode40.ImageIndex = 5
        TreeNode40.Name = "ndCycles(all)"
        TreeNode40.SelectedImageIndex = 5
        TreeNode40.Text = "Cycles (all)"
        TreeNode41.ImageIndex = 1
        TreeNode41.Name = "ndCyclingAndPathLength"
        TreeNode41.SelectedImageIndex = 1
        TreeNode41.Text = "Cycling and path length"
        TreeNode42.ImageIndex = 5
        TreeNode42.Name = "ndCyclesAndPathways"
        TreeNode42.SelectedImageIndex = 5
        TreeNode42.Text = "Cycles and pathways"
        TreeNode43.ImageIndex = 1
        TreeNode43.Name = "ndWithoutPrimaryProductionRequiredEstimate"
        TreeNode43.SelectedImageIndex = 1
        TreeNode43.Text = "Indices without primary production required estimate"
        TreeNode44.ImageIndex = 1
        TreeNode44.Name = "ndWithPrimaryProductionRequiredEstimate"
        TreeNode44.SelectedImageIndex = 1
        TreeNode44.Text = "Indices with primary production required estimate"
        TreeNode45.ImageIndex = 5
        TreeNode45.Name = "ndEcosim network analysis indices"
        TreeNode45.SelectedImageIndex = 5
        TreeNode45.Text = "Ecosim network analysis "
        Me.tvNetworkAnalysis.Nodes.AddRange(New System.Windows.Forms.TreeNode() {TreeNode3, TreeNode14, TreeNode17, TreeNode21, TreeNode24, TreeNode25, TreeNode42, TreeNode45})
        Me.tvNetworkAnalysis.SelectedImageIndex = 0
        Me.tvNetworkAnalysis.Size = New System.Drawing.Size(226, 467)
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
        'm_graph
        '
        Me.m_graph.Location = New System.Drawing.Point(114, 28)
        Me.m_graph.Name = "m_graph"
        Me.m_graph.ScrollGrace = 0
        Me.m_graph.ScrollMaxX = 0
        Me.m_graph.ScrollMaxY = 0
        Me.m_graph.ScrollMaxY2 = 0
        Me.m_graph.ScrollMinX = 0
        Me.m_graph.ScrollMinY = 0
        Me.m_graph.ScrollMinY2 = 0
        Me.m_graph.Size = New System.Drawing.Size(164, 433)
        Me.m_graph.TabIndex = 7
        '
        'm_plot
        '
        Me.m_plot.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.m_plot.Location = New System.Drawing.Point(3, 28)
        Me.m_plot.Name = "m_plot"
        Me.m_plot.Size = New System.Drawing.Size(105, 167)
        Me.m_plot.TabIndex = 1
        '
        'm_tsNetworkAnalysis
        '
        Me.m_tsNetworkAnalysis.BackColor = System.Drawing.Color.DarkSeaGreen
        Me.m_tsNetworkAnalysis.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tslblSelection1, Me.tscmbSelection1, Me.tslblSelection2, Me.tscmbSelection2, Me.tslblProgressBar, Me.tsbtnCancel, Me.tsbtnOutputIndicesCSV, Me.tsbtnOutputGraphEMF, Me.tsbtnPrintGraph, Me.tssepGraphMTI, Me.tsbtnGraphMTI})
        Me.m_tsNetworkAnalysis.Location = New System.Drawing.Point(0, 0)
        Me.m_tsNetworkAnalysis.Name = "m_tsNetworkAnalysis"
        Me.m_tsNetworkAnalysis.Size = New System.Drawing.Size(665, 25)
        Me.m_tsNetworkAnalysis.TabIndex = 4
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
        Me.tscmbSelection1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
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
        Me.tscmbSelection2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.tscmbSelection2.Name = "tscmbSelection2"
        Me.tscmbSelection2.Size = New System.Drawing.Size(121, 25)
        '
        'tslblProgressBar
        '
        Me.tslblProgressBar.Name = "tslblProgressBar"
        Me.tslblProgressBar.Size = New System.Drawing.Size(80, 22)
        Me.tslblProgressBar.Text = "ToolStripLabel3"
        '
        'tsbtnCancel
        '
        Me.tsbtnCancel.BackColor = System.Drawing.Color.DarkSeaGreen
        Me.tsbtnCancel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.tsbtnCancel.Image = CType(resources.GetObject("tsbtnCancel.Image"), System.Drawing.Image)
        Me.tsbtnCancel.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tsbtnCancel.Name = "tsbtnCancel"
        Me.tsbtnCancel.Size = New System.Drawing.Size(43, 22)
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
        'tssepGraphMTI
        '
        Me.tssepGraphMTI.Name = "tssepGraphMTI"
        Me.tssepGraphMTI.Size = New System.Drawing.Size(6, 25)
        '
        'tsbtnGraphMTI
        '
        Me.tsbtnGraphMTI.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.tsbtnGraphMTI.Image = CType(resources.GetObject("tsbtnGraphMTI.Image"), System.Drawing.Image)
        Me.tsbtnGraphMTI.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tsbtnGraphMTI.Name = "tsbtnGraphMTI"
        Me.tsbtnGraphMTI.Size = New System.Drawing.Size(58, 17)
        Me.tsbtnGraphMTI.Text = "Bar graph"
        '
        'm_datagrid
        '
        Me.m_datagrid.BackgroundColor = System.Drawing.SystemColors.ControlLightLight
        Me.m_datagrid.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.m_datagrid.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable
        Me.m_datagrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.m_datagrid.Location = New System.Drawing.Point(3, 201)
        Me.m_datagrid.Name = "m_datagrid"
        Me.m_datagrid.ReadOnly = True
        Me.m_datagrid.Size = New System.Drawing.Size(105, 260)
        Me.m_datagrid.TabIndex = 3
        '
        'm_tlpInfo
        '
        Me.m_tlpInfo.ColumnCount = 3
        Me.m_tlpInfo.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.m_tlpInfo.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 210.0!))
        Me.m_tlpInfo.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.m_tlpInfo.Controls.Add(Me.PictureBox1, 1, 1)
        Me.m_tlpInfo.Location = New System.Drawing.Point(284, 28)
        Me.m_tlpInfo.Name = "m_tlpInfo"
        Me.m_tlpInfo.RowCount = 3
        Me.m_tlpInfo.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.m_tlpInfo.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 400.0!))
        Me.m_tlpInfo.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.m_tlpInfo.Size = New System.Drawing.Size(373, 433)
        Me.m_tlpInfo.TabIndex = 5
        '
        'PictureBox1
        '
        Me.PictureBox1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PictureBox1.Image = Global.EwENetworkAnalysis.My.Resources.Resources.N_Asponsors
        Me.PictureBox1.Location = New System.Drawing.Point(84, 19)
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
        Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmNetworkAnalysis"
        Me.TabText = "Network analysis plug-in"
        Me.Text = "Networkanalysis plug-in"
        Me.scNetworkAnalysis.Panel1.ResumeLayout(False)
        Me.scNetworkAnalysis.Panel2.ResumeLayout(False)
        Me.scNetworkAnalysis.Panel2.PerformLayout()
        Me.scNetworkAnalysis.ResumeLayout(False)
        Me.m_tsNetworkAnalysis.ResumeLayout(False)
        Me.m_tsNetworkAnalysis.PerformLayout()
        CType(Me.m_datagrid, System.ComponentModel.ISupportInitialize).EndInit()
        Me.m_tlpInfo.ResumeLayout(False)
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Private WithEvents scNetworkAnalysis As System.Windows.Forms.SplitContainer
    Private WithEvents tvNetworkAnalysis As System.Windows.Forms.TreeView
    Private WithEvents lblNetworkAnalysis As System.Windows.Forms.Label
    Private WithEvents imglstNetworkAnalysis As System.Windows.Forms.ImageList
    Private WithEvents tscmbSelection1 As System.Windows.Forms.ToolStripComboBox
    Private WithEvents tslblSelection2 As System.Windows.Forms.ToolStripLabel
    Private WithEvents tscmbSelection2 As System.Windows.Forms.ToolStripComboBox
    Private WithEvents tslblProgressBar As System.Windows.Forms.ToolStripLabel
    Private WithEvents tslblSelection1 As System.Windows.Forms.ToolStripLabel
    Private WithEvents tsbtnCancel As System.Windows.Forms.ToolStripButton
    Private WithEvents PictureBox1 As System.Windows.Forms.PictureBox
    Private WithEvents tsbtnOutputIndicesCSV As System.Windows.Forms.ToolStripButton
    Private WithEvents tsbtnOutputGraphEMF As System.Windows.Forms.ToolStripButton
    Private WithEvents tsbtnPrintGraph As System.Windows.Forms.ToolStripButton
    Private WithEvents tsbtnGraphMTI As System.Windows.Forms.ToolStripButton
    Private WithEvents tssepGraphMTI As System.Windows.Forms.ToolStripSeparator
    Private WithEvents m_tsNetworkAnalysis As System.Windows.Forms.ToolStrip
    Private WithEvents m_datagrid As System.Windows.Forms.DataGridView
    Private WithEvents m_graph As ZedGraph.ZedGraphControl
    Private WithEvents m_tlpInfo As System.Windows.Forms.TableLayoutPanel
    Private WithEvents m_plot As ucPlot
End Class
