Imports WeifenLuo.WinFormsUI.Docking

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmEcotrophGrid
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
        Dim tslblTerminalTL As System.Windows.Forms.ToolStripLabel
        Dim TreeNode1 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Automatic smooth", 1, 1)
        Dim TreeNode2 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Omnivory index", 1, 1)
        Dim TreeNode3 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("User defined sigma", 1, 1)
        Dim TreeNode4 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Transpose", 5, 5, New System.Windows.Forms.TreeNode() {TreeNode1, TreeNode2, TreeNode3})
        Dim TreeNode5 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Basic parameters", 1, 1)
        Dim TreeNode6 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Forward calculation", 1, 1)
        Dim TreeNode7 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Backward calculation", 1, 1)
        Dim TreeNode8 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("CTSA", 5, 5, New System.Windows.Forms.TreeNode() {TreeNode5, TreeNode6, TreeNode7})
        Dim TreeNode9 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Basic parameters", 1, 1)
        Dim TreeNode10 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Evenly spaced effort multipliers", 1, 1)
        Dim TreeNode11 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Unevenly spaced effort multipliers", 1, 1)
        Dim TreeNode12 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("User defined effort multipliers", 1, 1)
        Dim TreeNode13 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Diagnosis", 5, 5, New System.Windows.Forms.TreeNode() {TreeNode9, TreeNode10, TreeNode11, TreeNode12})
        Dim TreeNode14 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Basic parameters", 1, 1)
        Dim TreeNode15 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Catch forecast", 1, 1)
        Dim TreeNode16 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Catch past analysis", 1, 1)
        Dim TreeNode17 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Dynamics", 5, 5, New System.Windows.Forms.TreeNode() {TreeNode14, TreeNode15, TreeNode16})
        Dim TreeNode18 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("EcoTroph plug-in", 5, 5, New System.Windows.Forms.TreeNode() {TreeNode4, TreeNode8, TreeNode13, TreeNode17})
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmEcotrophGrid))
        Me.tpEcotroph8 = New System.Windows.Forms.TabPage
        Me.dgvEcotroph8 = New System.Windows.Forms.DataGridView
        Me.lblEcotroph = New System.Windows.Forms.Label
        Me.scEcotroph1 = New System.Windows.Forms.SplitContainer
        Me.tvEcotroph = New System.Windows.Forms.TreeView
        Me.imglstEcotroph = New System.Windows.Forms.ImageList(Me.components)
        Me.scEcotroph2 = New System.Windows.Forms.SplitContainer
        Me.tcEcotroph = New System.Windows.Forms.TabControl
        Me.tpEcotroph1 = New System.Windows.Forms.TabPage
        Me.dgvEcotroph1 = New System.Windows.Forms.DataGridView
        Me.tpEcotroph2 = New System.Windows.Forms.TabPage
        Me.dgvEcotroph2 = New System.Windows.Forms.DataGridView
        Me.tpEcotroph3 = New System.Windows.Forms.TabPage
        Me.dgvEcotroph3 = New System.Windows.Forms.DataGridView
        Me.tpEcotroph4 = New System.Windows.Forms.TabPage
        Me.dgvEcotroph4 = New System.Windows.Forms.DataGridView
        Me.tpEcotroph5 = New System.Windows.Forms.TabPage
        Me.dgvEcotroph5 = New System.Windows.Forms.DataGridView
        Me.tpEcotroph6 = New System.Windows.Forms.TabPage
        Me.dgvEcotroph6 = New System.Windows.Forms.DataGridView
        Me.tpEcotroph7 = New System.Windows.Forms.TabPage
        Me.dgvEcotroph7 = New System.Windows.Forms.DataGridView
        Me.tpEcotroph9 = New System.Windows.Forms.TabPage
        Me.dgvEcotroph9 = New System.Windows.Forms.DataGridView
        Me.tpEcotroph10 = New System.Windows.Forms.TabPage
        Me.dgvEcotroph10 = New System.Windows.Forms.DataGridView
        Me.tpEcotroph11 = New System.Windows.Forms.TabPage
        Me.dgvEcotroph11 = New System.Windows.Forms.DataGridView
        Me.tpEcotroph12 = New System.Windows.Forms.TabPage
        Me.dgvEcotroph12 = New System.Windows.Forms.DataGridView
        Me.tpEcotroph13 = New System.Windows.Forms.TabPage
        Me.dgvEcotroph13 = New System.Windows.Forms.DataGridView
        Me.zgEcotroph = New ZedGraph.ZedGraphControl
        Me.tsEcotroph = New System.Windows.Forms.ToolStrip
        Me.tsbtnCalculate = New System.Windows.Forms.ToolStripButton
        Me.tsbtnPlot = New System.Windows.Forms.ToolStripButton
        Me.tsbtnImportCatches = New System.Windows.Forms.ToolStripButton
        Me.tsbtnSetDefault = New System.Windows.Forms.ToolStripButton
        Me.tssepSeparator = New System.Windows.Forms.ToolStripSeparator
        Me.tslblSmoothFactor = New System.Windows.Forms.ToolStripLabel
        Me.tstbxSmoothFactor = New System.Windows.Forms.ToolStripTextBox
        Me.tslblProgressBar = New System.Windows.Forms.ToolStripLabel
        Me.tspgbProgressBar = New System.Windows.Forms.ToolStripProgressBar
        Me.tslblWaterTemp = New System.Windows.Forms.ToolStripLabel
        Me.tstbxWaterTemp = New System.Windows.Forms.ToolStripTextBox
        Me.tslblTETL12 = New System.Windows.Forms.ToolStripLabel
        Me.tstbxTETL12 = New System.Windows.Forms.ToolStripTextBox
        Me.tslblTETL2 = New System.Windows.Forms.ToolStripLabel
        Me.tstbxTETL2 = New System.Windows.Forms.ToolStripTextBox
        Me.tslblMain = New System.Windows.Forms.ToolStripLabel
        Me.tscbxMainDiagnosis = New System.Windows.Forms.ToolStripComboBox
        Me.tscbxMainDynamics = New System.Windows.Forms.ToolStripComboBox
        Me.tslblTopD = New System.Windows.Forms.ToolStripLabel
        Me.tstbxTopD = New System.Windows.Forms.ToolStripTextBox
        Me.tslblFormD = New System.Windows.Forms.ToolStripLabel
        Me.tstbxFormD = New System.Windows.Forms.ToolStripTextBox
        Me.tslblAsymptote = New System.Windows.Forms.ToolStripLabel
        Me.tstbxAsymptote = New System.Windows.Forms.ToolStripTextBox
        Me.tslblTL50 = New System.Windows.Forms.ToolStripLabel
        Me.tstbxTL50 = New System.Windows.Forms.ToolStripTextBox
        Me.tslblSlope = New System.Windows.Forms.ToolStripLabel
        Me.tstbxSlope = New System.Windows.Forms.ToolStripTextBox
        Me.tscbxTerminalTL = New System.Windows.Forms.ToolStripComboBox
        Me.tslblInitializationBwdCal = New System.Windows.Forms.ToolStripLabel
        Me.tscbxInitializationBwdCal = New System.Windows.Forms.ToolStripComboBox
        Me.tslblSlopeSelectivityTTL = New System.Windows.Forms.ToolStripLabel
        Me.tstbxSlopeSelectivityTTL = New System.Windows.Forms.ToolStripTextBox
        Me.tslblBeta = New System.Windows.Forms.ToolStripLabel
        Me.tstbxBeta = New System.Windows.Forms.ToolStripTextBox
        Me.tslblInitializationFwdCal = New System.Windows.Forms.ToolStripLabel
        Me.tscbxInitializationFwdCal = New System.Windows.Forms.ToolStripComboBox
        Me.tslblRefYear = New System.Windows.Forms.ToolStripLabel
        Me.tstbxRefYear = New System.Windows.Forms.ToolStripTextBox
        Me.tslblNumYear = New System.Windows.Forms.ToolStripLabel
        Me.tstbxNumYear = New System.Windows.Forms.ToolStripTextBox
        tslblTerminalTL = New System.Windows.Forms.ToolStripLabel
        Me.tpEcotroph8.SuspendLayout()
        CType(Me.dgvEcotroph8, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.scEcotroph1.Panel1.SuspendLayout()
        Me.scEcotroph1.Panel2.SuspendLayout()
        Me.scEcotroph1.SuspendLayout()
        Me.scEcotroph2.Panel1.SuspendLayout()
        Me.scEcotroph2.Panel2.SuspendLayout()
        Me.scEcotroph2.SuspendLayout()
        Me.tcEcotroph.SuspendLayout()
        Me.tpEcotroph1.SuspendLayout()
        CType(Me.dgvEcotroph1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tpEcotroph2.SuspendLayout()
        CType(Me.dgvEcotroph2, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tpEcotroph3.SuspendLayout()
        CType(Me.dgvEcotroph3, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tpEcotroph4.SuspendLayout()
        CType(Me.dgvEcotroph4, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tpEcotroph5.SuspendLayout()
        CType(Me.dgvEcotroph5, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tpEcotroph6.SuspendLayout()
        CType(Me.dgvEcotroph6, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tpEcotroph7.SuspendLayout()
        CType(Me.dgvEcotroph7, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tpEcotroph9.SuspendLayout()
        CType(Me.dgvEcotroph9, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tpEcotroph10.SuspendLayout()
        CType(Me.dgvEcotroph10, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tpEcotroph11.SuspendLayout()
        CType(Me.dgvEcotroph11, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tpEcotroph12.SuspendLayout()
        CType(Me.dgvEcotroph12, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tpEcotroph13.SuspendLayout()
        CType(Me.dgvEcotroph13, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tsEcotroph.SuspendLayout()
        Me.SuspendLayout()
        '
        'tslblTerminalTL
        '
        tslblTerminalTL.Name = "tslblTerminalTL"
        tslblTerminalTL.Size = New System.Drawing.Size(61, 22)
        tslblTerminalTL.Text = "Terminal TL"
        '
        'tpEcotroph8
        '
        Me.tpEcotroph8.Controls.Add(Me.dgvEcotroph8)
        Me.tpEcotroph8.Location = New System.Drawing.Point(4, 22)
        Me.tpEcotroph8.Name = "tpEcotroph8"
        Me.tpEcotroph8.Size = New System.Drawing.Size(642, 410)
        Me.tpEcotroph8.TabIndex = 7
        Me.tpEcotroph8.Text = "TabPage8"
        Me.tpEcotroph8.UseVisualStyleBackColor = True
        '
        'dgvEcotroph8
        '
        Me.dgvEcotroph8.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.dgvEcotroph8.BackgroundColor = System.Drawing.SystemColors.Window
        Me.dgvEcotroph8.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvEcotroph8.Location = New System.Drawing.Point(0, 0)
        Me.dgvEcotroph8.Name = "dgvEcotroph8"
        Me.dgvEcotroph8.Size = New System.Drawing.Size(642, 410)
        Me.dgvEcotroph8.TabIndex = 0
        '
        'lblEcotroph
        '
        Me.lblEcotroph.AutoSize = True
        Me.lblEcotroph.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblEcotroph.Location = New System.Drawing.Point(3, 9)
        Me.lblEcotroph.Name = "lblEcotroph"
        Me.lblEcotroph.Size = New System.Drawing.Size(145, 20)
        Me.lblEcotroph.TabIndex = 0
        Me.lblEcotroph.Text = "EcoTroph plug-in"
        '
        'scEcotroph1
        '
        Me.scEcotroph1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.scEcotroph1.Location = New System.Drawing.Point(7, 37)
        Me.scEcotroph1.Name = "scEcotroph1"
        '
        'scEcotroph1.Panel1
        '
        Me.scEcotroph1.Panel1.Controls.Add(Me.tvEcotroph)
        '
        'scEcotroph1.Panel2
        '
        Me.scEcotroph1.Panel2.Controls.Add(Me.scEcotroph2)
        Me.scEcotroph1.Panel2.Controls.Add(Me.tsEcotroph)
        Me.scEcotroph1.Size = New System.Drawing.Size(895, 467)
        Me.scEcotroph1.SplitterDistance = 224
        Me.scEcotroph1.TabIndex = 1
        '
        'tvEcotroph
        '
        Me.tvEcotroph.BackColor = System.Drawing.Color.LemonChiffon
        Me.tvEcotroph.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tvEcotroph.ImageIndex = 0
        Me.tvEcotroph.ImageList = Me.imglstEcotroph
        Me.tvEcotroph.Location = New System.Drawing.Point(0, 0)
        Me.tvEcotroph.Name = "tvEcotroph"
        TreeNode1.ImageIndex = 1
        TreeNode1.Name = "ndAutoSmooth"
        TreeNode1.SelectedImageIndex = 1
        TreeNode1.Text = "Automatic smooth"
        TreeNode2.ImageIndex = 1
        TreeNode2.Name = "ndOminIdx"
        TreeNode2.SelectedImageIndex = 1
        TreeNode2.Text = "Omnivory index"
        TreeNode3.ImageIndex = 1
        TreeNode3.Name = "ndUserDefSigma"
        TreeNode3.SelectedImageIndex = 1
        TreeNode3.Text = "User defined sigma"
        TreeNode4.ImageIndex = 5
        TreeNode4.Name = "ndTranspose"
        TreeNode4.SelectedImageIndex = 5
        TreeNode4.Text = "Transpose"
        TreeNode5.ImageIndex = 1
        TreeNode5.Name = "ndBasicParam"
        TreeNode5.SelectedImageIndex = 1
        TreeNode5.Text = "Basic parameters"
        TreeNode6.ImageIndex = 1
        TreeNode6.Name = "ndForwardCal"
        TreeNode6.SelectedImageIndex = 1
        TreeNode6.Text = "Forward calculation"
        TreeNode7.ImageIndex = 1
        TreeNode7.Name = "ndBackwardCal"
        TreeNode7.SelectedImageIndex = 1
        TreeNode7.Text = "Backward calculation"
        TreeNode8.ImageIndex = 5
        TreeNode8.Name = "ndCTSA"
        TreeNode8.SelectedImageIndex = 5
        TreeNode8.Text = "CTSA"
        TreeNode9.ImageIndex = 1
        TreeNode9.Name = "ndBasicParam"
        TreeNode9.SelectedImageIndex = 1
        TreeNode9.Text = "Basic parameters"
        TreeNode10.ImageIndex = 1
        TreeNode10.Name = "ndEvenlySpacedEM"
        TreeNode10.SelectedImageIndex = 1
        TreeNode10.Text = "Evenly spaced effort multipliers"
        TreeNode11.ImageIndex = 1
        TreeNode11.Name = "ndUnevenlySpacedEM"
        TreeNode11.SelectedImageIndex = 1
        TreeNode11.Text = "Unevenly spaced effort multipliers"
        TreeNode12.ImageIndex = 1
        TreeNode12.Name = "ndUserDefinedEM"
        TreeNode12.SelectedImageIndex = 1
        TreeNode12.Text = "User defined effort multipliers"
        TreeNode13.ImageIndex = 5
        TreeNode13.Name = "ndDiagnosis"
        TreeNode13.SelectedImageIndex = 5
        TreeNode13.Text = "Diagnosis"
        TreeNode14.ImageIndex = 1
        TreeNode14.Name = "ndBasicParam"
        TreeNode14.SelectedImageIndex = 1
        TreeNode14.Text = "Basic parameters"
        TreeNode15.ImageIndex = 1
        TreeNode15.Name = "ndCatchForecast"
        TreeNode15.SelectedImageIndex = 1
        TreeNode15.Text = "Catch forecast"
        TreeNode16.ImageIndex = 1
        TreeNode16.Name = "ndCatchPastAnalysis"
        TreeNode16.SelectedImageIndex = 1
        TreeNode16.Text = "Catch past analysis"
        TreeNode17.ImageIndex = 5
        TreeNode17.Name = "ndDynamics"
        TreeNode17.SelectedImageIndex = 5
        TreeNode17.Text = "Dynamics"
        TreeNode18.ImageIndex = 5
        TreeNode18.Name = "ndEwEEcotrophPlugin"
        TreeNode18.SelectedImageIndex = 5
        TreeNode18.Text = "EcoTroph plug-in"
        Me.tvEcotroph.Nodes.AddRange(New System.Windows.Forms.TreeNode() {TreeNode18})
        Me.tvEcotroph.SelectedImageIndex = 0
        Me.tvEcotroph.Size = New System.Drawing.Size(224, 467)
        Me.tvEcotroph.TabIndex = 0
        '
        'imglstEcotroph
        '
        Me.imglstEcotroph.ImageStream = CType(resources.GetObject("imglstEcotroph.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.imglstEcotroph.TransparentColor = System.Drawing.Color.Transparent
        Me.imglstEcotroph.Images.SetKeyName(0, "")
        Me.imglstEcotroph.Images.SetKeyName(1, "")
        Me.imglstEcotroph.Images.SetKeyName(2, "")
        Me.imglstEcotroph.Images.SetKeyName(3, "")
        Me.imglstEcotroph.Images.SetKeyName(4, "")
        Me.imglstEcotroph.Images.SetKeyName(5, "")
        Me.imglstEcotroph.Images.SetKeyName(6, "")
        Me.imglstEcotroph.Images.SetKeyName(7, "")
        Me.imglstEcotroph.Images.SetKeyName(8, "")
        Me.imglstEcotroph.Images.SetKeyName(9, "")
        Me.imglstEcotroph.Images.SetKeyName(10, "")
        '
        'scEcotroph2
        '
        Me.scEcotroph2.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.scEcotroph2.Location = New System.Drawing.Point(0, 28)
        Me.scEcotroph2.Name = "scEcotroph2"
        '
        'scEcotroph2.Panel1
        '
        Me.scEcotroph2.Panel1.Controls.Add(Me.tcEcotroph)
        '
        'scEcotroph2.Panel2
        '
        Me.scEcotroph2.Panel2.Controls.Add(Me.zgEcotroph)
        Me.scEcotroph2.Panel2MinSize = 5
        Me.scEcotroph2.Size = New System.Drawing.Size(664, 436)
        Me.scEcotroph2.SplitterDistance = 650
        Me.scEcotroph2.TabIndex = 1
        '
        'tcEcotroph
        '
        Me.tcEcotroph.Controls.Add(Me.tpEcotroph1)
        Me.tcEcotroph.Controls.Add(Me.tpEcotroph2)
        Me.tcEcotroph.Controls.Add(Me.tpEcotroph3)
        Me.tcEcotroph.Controls.Add(Me.tpEcotroph4)
        Me.tcEcotroph.Controls.Add(Me.tpEcotroph5)
        Me.tcEcotroph.Controls.Add(Me.tpEcotroph6)
        Me.tcEcotroph.Controls.Add(Me.tpEcotroph7)
        Me.tcEcotroph.Controls.Add(Me.tpEcotroph8)
        Me.tcEcotroph.Controls.Add(Me.tpEcotroph9)
        Me.tcEcotroph.Controls.Add(Me.tpEcotroph10)
        Me.tcEcotroph.Controls.Add(Me.tpEcotroph11)
        Me.tcEcotroph.Controls.Add(Me.tpEcotroph12)
        Me.tcEcotroph.Controls.Add(Me.tpEcotroph13)
        Me.tcEcotroph.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tcEcotroph.Location = New System.Drawing.Point(0, 0)
        Me.tcEcotroph.Name = "tcEcotroph"
        Me.tcEcotroph.SelectedIndex = 0
        Me.tcEcotroph.Size = New System.Drawing.Size(650, 436)
        Me.tcEcotroph.TabIndex = 0
        '
        'tpEcotroph1
        '
        Me.tpEcotroph1.Controls.Add(Me.dgvEcotroph1)
        Me.tpEcotroph1.Location = New System.Drawing.Point(4, 22)
        Me.tpEcotroph1.Name = "tpEcotroph1"
        Me.tpEcotroph1.Padding = New System.Windows.Forms.Padding(3)
        Me.tpEcotroph1.Size = New System.Drawing.Size(642, 410)
        Me.tpEcotroph1.TabIndex = 0
        Me.tpEcotroph1.Text = "TabPage1"
        Me.tpEcotroph1.UseVisualStyleBackColor = True
        '
        'dgvEcotroph1
        '
        Me.dgvEcotroph1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.dgvEcotroph1.BackgroundColor = System.Drawing.SystemColors.Window
        Me.dgvEcotroph1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvEcotroph1.Location = New System.Drawing.Point(0, 0)
        Me.dgvEcotroph1.Name = "dgvEcotroph1"
        Me.dgvEcotroph1.Size = New System.Drawing.Size(642, 410)
        Me.dgvEcotroph1.TabIndex = 0
        '
        'tpEcotroph2
        '
        Me.tpEcotroph2.Controls.Add(Me.dgvEcotroph2)
        Me.tpEcotroph2.Location = New System.Drawing.Point(4, 22)
        Me.tpEcotroph2.Name = "tpEcotroph2"
        Me.tpEcotroph2.Padding = New System.Windows.Forms.Padding(3)
        Me.tpEcotroph2.Size = New System.Drawing.Size(642, 410)
        Me.tpEcotroph2.TabIndex = 1
        Me.tpEcotroph2.Text = "TabPage2"
        Me.tpEcotroph2.UseVisualStyleBackColor = True
        '
        'dgvEcotroph2
        '
        Me.dgvEcotroph2.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.dgvEcotroph2.BackgroundColor = System.Drawing.SystemColors.Window
        Me.dgvEcotroph2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvEcotroph2.Location = New System.Drawing.Point(0, 0)
        Me.dgvEcotroph2.Name = "dgvEcotroph2"
        Me.dgvEcotroph2.Size = New System.Drawing.Size(642, 410)
        Me.dgvEcotroph2.TabIndex = 0
        '
        'tpEcotroph3
        '
        Me.tpEcotroph3.Controls.Add(Me.dgvEcotroph3)
        Me.tpEcotroph3.Location = New System.Drawing.Point(4, 22)
        Me.tpEcotroph3.Name = "tpEcotroph3"
        Me.tpEcotroph3.Size = New System.Drawing.Size(642, 410)
        Me.tpEcotroph3.TabIndex = 2
        Me.tpEcotroph3.Text = "TabPage3"
        Me.tpEcotroph3.UseVisualStyleBackColor = True
        '
        'dgvEcotroph3
        '
        Me.dgvEcotroph3.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.dgvEcotroph3.BackgroundColor = System.Drawing.SystemColors.Window
        Me.dgvEcotroph3.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvEcotroph3.Location = New System.Drawing.Point(0, 0)
        Me.dgvEcotroph3.Name = "dgvEcotroph3"
        Me.dgvEcotroph3.Size = New System.Drawing.Size(642, 410)
        Me.dgvEcotroph3.TabIndex = 0
        '
        'tpEcotroph4
        '
        Me.tpEcotroph4.Controls.Add(Me.dgvEcotroph4)
        Me.tpEcotroph4.Location = New System.Drawing.Point(4, 22)
        Me.tpEcotroph4.Name = "tpEcotroph4"
        Me.tpEcotroph4.Size = New System.Drawing.Size(642, 410)
        Me.tpEcotroph4.TabIndex = 3
        Me.tpEcotroph4.Text = "TabPage4"
        Me.tpEcotroph4.UseVisualStyleBackColor = True
        '
        'dgvEcotroph4
        '
        Me.dgvEcotroph4.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.dgvEcotroph4.BackgroundColor = System.Drawing.SystemColors.Window
        Me.dgvEcotroph4.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvEcotroph4.Location = New System.Drawing.Point(0, 0)
        Me.dgvEcotroph4.Name = "dgvEcotroph4"
        Me.dgvEcotroph4.Size = New System.Drawing.Size(642, 410)
        Me.dgvEcotroph4.TabIndex = 0
        '
        'tpEcotroph5
        '
        Me.tpEcotroph5.Controls.Add(Me.dgvEcotroph5)
        Me.tpEcotroph5.Location = New System.Drawing.Point(4, 22)
        Me.tpEcotroph5.Name = "tpEcotroph5"
        Me.tpEcotroph5.Size = New System.Drawing.Size(642, 410)
        Me.tpEcotroph5.TabIndex = 4
        Me.tpEcotroph5.Text = "TabPage5"
        Me.tpEcotroph5.UseVisualStyleBackColor = True
        '
        'dgvEcotroph5
        '
        Me.dgvEcotroph5.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.dgvEcotroph5.BackgroundColor = System.Drawing.SystemColors.Window
        Me.dgvEcotroph5.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvEcotroph5.Location = New System.Drawing.Point(0, 0)
        Me.dgvEcotroph5.Name = "dgvEcotroph5"
        Me.dgvEcotroph5.Size = New System.Drawing.Size(642, 410)
        Me.dgvEcotroph5.TabIndex = 0
        '
        'tpEcotroph6
        '
        Me.tpEcotroph6.Controls.Add(Me.dgvEcotroph6)
        Me.tpEcotroph6.Location = New System.Drawing.Point(4, 22)
        Me.tpEcotroph6.Name = "tpEcotroph6"
        Me.tpEcotroph6.Size = New System.Drawing.Size(642, 410)
        Me.tpEcotroph6.TabIndex = 5
        Me.tpEcotroph6.Text = "TabPage6"
        Me.tpEcotroph6.UseVisualStyleBackColor = True
        '
        'dgvEcotroph6
        '
        Me.dgvEcotroph6.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.dgvEcotroph6.BackgroundColor = System.Drawing.SystemColors.Window
        Me.dgvEcotroph6.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvEcotroph6.Location = New System.Drawing.Point(0, 0)
        Me.dgvEcotroph6.Name = "dgvEcotroph6"
        Me.dgvEcotroph6.Size = New System.Drawing.Size(642, 410)
        Me.dgvEcotroph6.TabIndex = 0
        '
        'tpEcotroph7
        '
        Me.tpEcotroph7.Controls.Add(Me.dgvEcotroph7)
        Me.tpEcotroph7.Location = New System.Drawing.Point(4, 22)
        Me.tpEcotroph7.Name = "tpEcotroph7"
        Me.tpEcotroph7.Size = New System.Drawing.Size(642, 410)
        Me.tpEcotroph7.TabIndex = 6
        Me.tpEcotroph7.Text = "TabPage7"
        Me.tpEcotroph7.UseVisualStyleBackColor = True
        '
        'dgvEcotroph7
        '
        Me.dgvEcotroph7.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.dgvEcotroph7.BackgroundColor = System.Drawing.SystemColors.Window
        Me.dgvEcotroph7.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvEcotroph7.Location = New System.Drawing.Point(0, 0)
        Me.dgvEcotroph7.Name = "dgvEcotroph7"
        Me.dgvEcotroph7.Size = New System.Drawing.Size(642, 410)
        Me.dgvEcotroph7.TabIndex = 0
        '
        'tpEcotroph9
        '
        Me.tpEcotroph9.Controls.Add(Me.dgvEcotroph9)
        Me.tpEcotroph9.Location = New System.Drawing.Point(4, 22)
        Me.tpEcotroph9.Name = "tpEcotroph9"
        Me.tpEcotroph9.Size = New System.Drawing.Size(642, 410)
        Me.tpEcotroph9.TabIndex = 8
        Me.tpEcotroph9.Text = "TabPage9"
        Me.tpEcotroph9.UseVisualStyleBackColor = True
        '
        'dgvEcotroph9
        '
        Me.dgvEcotroph9.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.dgvEcotroph9.BackgroundColor = System.Drawing.SystemColors.Window
        Me.dgvEcotroph9.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvEcotroph9.Location = New System.Drawing.Point(0, 0)
        Me.dgvEcotroph9.Name = "dgvEcotroph9"
        Me.dgvEcotroph9.Size = New System.Drawing.Size(642, 410)
        Me.dgvEcotroph9.TabIndex = 0
        '
        'tpEcotroph10
        '
        Me.tpEcotroph10.Controls.Add(Me.dgvEcotroph10)
        Me.tpEcotroph10.Location = New System.Drawing.Point(4, 22)
        Me.tpEcotroph10.Name = "tpEcotroph10"
        Me.tpEcotroph10.Size = New System.Drawing.Size(642, 410)
        Me.tpEcotroph10.TabIndex = 9
        Me.tpEcotroph10.Text = "TabPage10"
        Me.tpEcotroph10.UseVisualStyleBackColor = True
        '
        'dgvEcotroph10
        '
        Me.dgvEcotroph10.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.dgvEcotroph10.BackgroundColor = System.Drawing.SystemColors.Window
        Me.dgvEcotroph10.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvEcotroph10.Location = New System.Drawing.Point(0, 0)
        Me.dgvEcotroph10.Name = "dgvEcotroph10"
        Me.dgvEcotroph10.Size = New System.Drawing.Size(642, 410)
        Me.dgvEcotroph10.TabIndex = 0
        '
        'tpEcotroph11
        '
        Me.tpEcotroph11.Controls.Add(Me.dgvEcotroph11)
        Me.tpEcotroph11.Location = New System.Drawing.Point(4, 22)
        Me.tpEcotroph11.Name = "tpEcotroph11"
        Me.tpEcotroph11.Size = New System.Drawing.Size(642, 410)
        Me.tpEcotroph11.TabIndex = 10
        Me.tpEcotroph11.Text = "TabPage11"
        Me.tpEcotroph11.UseVisualStyleBackColor = True
        '
        'dgvEcotroph11
        '
        Me.dgvEcotroph11.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.dgvEcotroph11.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvEcotroph11.Location = New System.Drawing.Point(0, 0)
        Me.dgvEcotroph11.Name = "dgvEcotroph11"
        Me.dgvEcotroph11.Size = New System.Drawing.Size(642, 410)
        Me.dgvEcotroph11.TabIndex = 0
        '
        'tpEcotroph12
        '
        Me.tpEcotroph12.Controls.Add(Me.dgvEcotroph12)
        Me.tpEcotroph12.Location = New System.Drawing.Point(4, 22)
        Me.tpEcotroph12.Name = "tpEcotroph12"
        Me.tpEcotroph12.Size = New System.Drawing.Size(642, 410)
        Me.tpEcotroph12.TabIndex = 11
        Me.tpEcotroph12.Text = "TabPage12"
        Me.tpEcotroph12.UseVisualStyleBackColor = True
        '
        'dgvEcotroph12
        '
        Me.dgvEcotroph12.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.dgvEcotroph12.BackgroundColor = System.Drawing.SystemColors.Window
        Me.dgvEcotroph12.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvEcotroph12.Location = New System.Drawing.Point(0, 0)
        Me.dgvEcotroph12.Name = "dgvEcotroph12"
        Me.dgvEcotroph12.Size = New System.Drawing.Size(642, 410)
        Me.dgvEcotroph12.TabIndex = 0
        '
        'tpEcotroph13
        '
        Me.tpEcotroph13.Controls.Add(Me.dgvEcotroph13)
        Me.tpEcotroph13.Location = New System.Drawing.Point(4, 22)
        Me.tpEcotroph13.Name = "tpEcotroph13"
        Me.tpEcotroph13.Size = New System.Drawing.Size(642, 410)
        Me.tpEcotroph13.TabIndex = 12
        Me.tpEcotroph13.Text = "TabPage13"
        Me.tpEcotroph13.UseVisualStyleBackColor = True
        '
        'dgvEcotroph13
        '
        Me.dgvEcotroph13.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.dgvEcotroph13.BackgroundColor = System.Drawing.SystemColors.Window
        Me.dgvEcotroph13.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvEcotroph13.Location = New System.Drawing.Point(0, 0)
        Me.dgvEcotroph13.Name = "dgvEcotroph13"
        Me.dgvEcotroph13.Size = New System.Drawing.Size(642, 410)
        Me.dgvEcotroph13.TabIndex = 0
        '
        'zgEcotroph
        '
        Me.zgEcotroph.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.zgEcotroph.Location = New System.Drawing.Point(3, 25)
        Me.zgEcotroph.Name = "zgEcotroph"
        Me.zgEcotroph.ScrollGrace = 0
        Me.zgEcotroph.ScrollMaxX = 0
        Me.zgEcotroph.ScrollMaxY = 0
        Me.zgEcotroph.ScrollMaxY2 = 0
        Me.zgEcotroph.ScrollMinX = 0
        Me.zgEcotroph.ScrollMinY = 0
        Me.zgEcotroph.ScrollMinY2 = 0
        Me.zgEcotroph.Size = New System.Drawing.Size(4, 404)
        Me.zgEcotroph.TabIndex = 0
        '
        'tsEcotroph
        '
        Me.tsEcotroph.BackColor = System.Drawing.Color.Goldenrod
        Me.tsEcotroph.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsbtnCalculate, Me.tsbtnPlot, Me.tsbtnImportCatches, Me.tsbtnSetDefault, Me.tssepSeparator, Me.tslblSmoothFactor, Me.tstbxSmoothFactor, Me.tslblProgressBar, Me.tspgbProgressBar, Me.tslblWaterTemp, Me.tstbxWaterTemp, Me.tslblTETL12, Me.tstbxTETL12, Me.tslblTETL2, Me.tstbxTETL2, Me.tslblMain, Me.tscbxMainDiagnosis, Me.tscbxMainDynamics, Me.tslblTopD, Me.tstbxTopD, Me.tslblFormD, Me.tstbxFormD, Me.tslblAsymptote, Me.tstbxAsymptote, Me.tslblTL50, Me.tstbxTL50, Me.tslblSlope, Me.tstbxSlope, tslblTerminalTL, Me.tscbxTerminalTL, Me.tslblInitializationBwdCal, Me.tscbxInitializationBwdCal, Me.tslblSlopeSelectivityTTL, Me.tstbxSlopeSelectivityTTL, Me.tslblBeta, Me.tstbxBeta, Me.tslblInitializationFwdCal, Me.tscbxInitializationFwdCal, Me.tslblRefYear, Me.tstbxRefYear, Me.tslblNumYear, Me.tstbxNumYear})
        Me.tsEcotroph.Location = New System.Drawing.Point(0, 0)
        Me.tsEcotroph.Name = "tsEcotroph"
        Me.tsEcotroph.Size = New System.Drawing.Size(667, 25)
        Me.tsEcotroph.TabIndex = 0
        Me.tsEcotroph.Text = "ToolStrip1"
        '
        'tsbtnCalculate
        '
        Me.tsbtnCalculate.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.tsbtnCalculate.Image = CType(resources.GetObject("tsbtnCalculate.Image"), System.Drawing.Image)
        Me.tsbtnCalculate.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tsbtnCalculate.Name = "tsbtnCalculate"
        Me.tsbtnCalculate.Size = New System.Drawing.Size(55, 22)
        Me.tsbtnCalculate.Text = "Calculate"
        Me.tsbtnCalculate.Visible = False
        '
        'tsbtnPlot
        '
        Me.tsbtnPlot.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.tsbtnPlot.Image = CType(resources.GetObject("tsbtnPlot.Image"), System.Drawing.Image)
        Me.tsbtnPlot.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tsbtnPlot.Name = "tsbtnPlot"
        Me.tsbtnPlot.Size = New System.Drawing.Size(29, 22)
        Me.tsbtnPlot.Text = "Plot"
        '
        'tsbtnImportCatches
        '
        Me.tsbtnImportCatches.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.tsbtnImportCatches.Image = CType(resources.GetObject("tsbtnImportCatches.Image"), System.Drawing.Image)
        Me.tsbtnImportCatches.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tsbtnImportCatches.Name = "tsbtnImportCatches"
        Me.tsbtnImportCatches.Size = New System.Drawing.Size(43, 22)
        Me.tsbtnImportCatches.Text = "Import"
        Me.tsbtnImportCatches.ToolTipText = "Import catches from file"
        '
        'tsbtnSetDefault
        '
        Me.tsbtnSetDefault.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.tsbtnSetDefault.Image = CType(resources.GetObject("tsbtnSetDefault.Image"), System.Drawing.Image)
        Me.tsbtnSetDefault.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tsbtnSetDefault.Name = "tsbtnSetDefault"
        Me.tsbtnSetDefault.Size = New System.Drawing.Size(64, 22)
        Me.tsbtnSetDefault.Text = "Set default"
        Me.tsbtnSetDefault.ToolTipText = "Set default values"
        '
        'tssepSeparator
        '
        Me.tssepSeparator.Name = "tssepSeparator"
        Me.tssepSeparator.Size = New System.Drawing.Size(6, 25)
        '
        'tslblSmoothFactor
        '
        Me.tslblSmoothFactor.Name = "tslblSmoothFactor"
        Me.tslblSmoothFactor.Size = New System.Drawing.Size(75, 22)
        Me.tslblSmoothFactor.Text = "Smooth factor"
        Me.tslblSmoothFactor.Visible = False
        '
        'tstbxSmoothFactor
        '
        Me.tstbxSmoothFactor.BackColor = System.Drawing.Color.LightGreen
        Me.tstbxSmoothFactor.Name = "tstbxSmoothFactor"
        Me.tstbxSmoothFactor.Size = New System.Drawing.Size(35, 25)
        Me.tstbxSmoothFactor.Visible = False
        '
        'tslblProgressBar
        '
        Me.tslblProgressBar.Name = "tslblProgressBar"
        Me.tslblProgressBar.Size = New System.Drawing.Size(49, 22)
        Me.tslblProgressBar.Text = "Progress"
        Me.tslblProgressBar.Visible = False
        '
        'tspgbProgressBar
        '
        Me.tspgbProgressBar.Name = "tspgbProgressBar"
        Me.tspgbProgressBar.Size = New System.Drawing.Size(100, 22)
        Me.tspgbProgressBar.Visible = False
        '
        'tslblWaterTemp
        '
        Me.tslblWaterTemp.AutoSize = False
        Me.tslblWaterTemp.Font = New System.Drawing.Font("Tahoma", 8.25!)
        Me.tslblWaterTemp.Name = "tslblWaterTemp"
        Me.tslblWaterTemp.Size = New System.Drawing.Size(50, 22)
        Me.tslblWaterTemp.Text = "Water temp"
        Me.tslblWaterTemp.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.tslblWaterTemp.Visible = False
        '
        'tstbxWaterTemp
        '
        Me.tstbxWaterTemp.BackColor = System.Drawing.Color.LightGreen
        Me.tstbxWaterTemp.Name = "tstbxWaterTemp"
        Me.tstbxWaterTemp.Size = New System.Drawing.Size(30, 25)
        Me.tstbxWaterTemp.Visible = False
        '
        'tslblTETL12
        '
        Me.tslblTETL12.AutoSize = False
        Me.tslblTETL12.Name = "tslblTETL12"
        Me.tslblTETL12.Size = New System.Drawing.Size(50, 22)
        Me.tslblTETL12.Text = "TE for 1<TL<2"
        Me.tslblTETL12.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.tslblTETL12.Visible = False
        '
        'tstbxTETL12
        '
        Me.tstbxTETL12.BackColor = System.Drawing.Color.LightGreen
        Me.tstbxTETL12.Name = "tstbxTETL12"
        Me.tstbxTETL12.Size = New System.Drawing.Size(30, 25)
        Me.tstbxTETL12.Visible = False
        '
        'tslblTETL2
        '
        Me.tslblTETL2.AutoSize = False
        Me.tslblTETL2.Name = "tslblTETL2"
        Me.tslblTETL2.Size = New System.Drawing.Size(50, 22)
        Me.tslblTETL2.Text = "TE for TL>=2"
        Me.tslblTETL2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.tslblTETL2.Visible = False
        '
        'tstbxTETL2
        '
        Me.tstbxTETL2.BackColor = System.Drawing.Color.LightGreen
        Me.tstbxTETL2.Name = "tstbxTETL2"
        Me.tstbxTETL2.Size = New System.Drawing.Size(30, 25)
        Me.tstbxTETL2.Visible = False
        '
        'tslblMain
        '
        Me.tslblMain.Name = "tslblMain"
        Me.tslblMain.Size = New System.Drawing.Size(29, 22)
        Me.tslblMain.Text = "Main"
        '
        'tscbxMainDiagnosis
        '
        Me.tscbxMainDiagnosis.BackColor = System.Drawing.Color.LightGreen
        Me.tscbxMainDiagnosis.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.tscbxMainDiagnosis.Items.AddRange(New Object() {"Please select", "Transpose-Automatic smooth", "Transpose-Omnivory index", "Transpose-User defined sigma", "CTSA-Forward calculation", "CTSA-Backward calculation"})
        Me.tscbxMainDiagnosis.Name = "tscbxMainDiagnosis"
        Me.tscbxMainDiagnosis.Size = New System.Drawing.Size(121, 25)
        '
        'tscbxMainDynamics
        '
        Me.tscbxMainDynamics.BackColor = System.Drawing.Color.LightGreen
        Me.tscbxMainDynamics.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.tscbxMainDynamics.Items.AddRange(New Object() {"Please select", "Transpose-Automatic smooth", "Transpose-Omnivory index", "Transpose-User defined sigma", "CTSA-Forward calculation", "CTSA-Backward calculation"})
        Me.tscbxMainDynamics.Name = "tscbxMainDynamics"
        Me.tscbxMainDynamics.Size = New System.Drawing.Size(121, 25)
        '
        'tslblTopD
        '
        Me.tslblTopD.Name = "tslblTopD"
        Me.tslblTopD.Size = New System.Drawing.Size(35, 22)
        Me.tslblTopD.Text = "Top D"
        Me.tslblTopD.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.tslblTopD.Visible = False
        '
        'tstbxTopD
        '
        Me.tstbxTopD.BackColor = System.Drawing.Color.LightGreen
        Me.tstbxTopD.Name = "tstbxTopD"
        Me.tstbxTopD.Size = New System.Drawing.Size(30, 25)
        Me.tstbxTopD.Visible = False
        '
        'tslblFormD
        '
        Me.tslblFormD.Name = "tslblFormD"
        Me.tslblFormD.Size = New System.Drawing.Size(41, 22)
        Me.tslblFormD.Text = "Form D"
        Me.tslblFormD.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.tslblFormD.Visible = False
        '
        'tstbxFormD
        '
        Me.tstbxFormD.BackColor = System.Drawing.Color.LightGreen
        Me.tstbxFormD.Name = "tstbxFormD"
        Me.tstbxFormD.Size = New System.Drawing.Size(30, 25)
        Me.tstbxFormD.Visible = False
        '
        'tslblAsymptote
        '
        Me.tslblAsymptote.AutoSize = False
        Me.tslblAsymptote.Name = "tslblAsymptote"
        Me.tslblAsymptote.Size = New System.Drawing.Size(50, 13)
        Me.tslblAsymptote.Text = "Asymptote"
        Me.tslblAsymptote.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.tslblAsymptote.Visible = False
        '
        'tstbxAsymptote
        '
        Me.tstbxAsymptote.BackColor = System.Drawing.Color.LightGreen
        Me.tstbxAsymptote.Name = "tstbxAsymptote"
        Me.tstbxAsymptote.Size = New System.Drawing.Size(30, 25)
        Me.tstbxAsymptote.Visible = False
        '
        'tslblTL50
        '
        Me.tslblTL50.Name = "tslblTL50"
        Me.tslblTL50.Size = New System.Drawing.Size(30, 22)
        Me.tslblTL50.Text = "TL50"
        Me.tslblTL50.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.tslblTL50.Visible = False
        '
        'tstbxTL50
        '
        Me.tstbxTL50.BackColor = System.Drawing.Color.LightGreen
        Me.tstbxTL50.Name = "tstbxTL50"
        Me.tstbxTL50.Size = New System.Drawing.Size(30, 25)
        Me.tstbxTL50.Visible = False
        '
        'tslblSlope
        '
        Me.tslblSlope.Name = "tslblSlope"
        Me.tslblSlope.Size = New System.Drawing.Size(33, 22)
        Me.tslblSlope.Text = "Slope"
        Me.tslblSlope.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.tslblSlope.Visible = False
        '
        'tstbxSlope
        '
        Me.tstbxSlope.BackColor = System.Drawing.Color.LightGreen
        Me.tstbxSlope.Name = "tstbxSlope"
        Me.tstbxSlope.Size = New System.Drawing.Size(30, 25)
        Me.tstbxSlope.Visible = False
        '
        'tscbxTerminalTL
        '
        Me.tscbxTerminalTL.BackColor = System.Drawing.Color.LightGreen
        Me.tscbxTerminalTL.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.tscbxTerminalTL.DropDownWidth = 75
        Me.tscbxTerminalTL.Items.AddRange(New Object() {"4", "4.1", "4.2", "4.3", "4.4", "4.5", "4.6", "4.7", "4.8", "4.9", "5", "5.1", "5.2", "5.3", "5.4", "5.5", "5.6", "5.7", "5.8", "5.9", "6"})
        Me.tscbxTerminalTL.Name = "tscbxTerminalTL"
        Me.tscbxTerminalTL.Size = New System.Drawing.Size(75, 25)
        '
        'tslblInitializationBwdCal
        '
        Me.tslblInitializationBwdCal.Name = "tslblInitializationBwdCal"
        Me.tslblInitializationBwdCal.Size = New System.Drawing.Size(103, 13)
        Me.tslblInitializationBwdCal.Text = "Initialization bwd cal"
        '
        'tscbxInitializationBwdCal
        '
        Me.tscbxInitializationBwdCal.BackColor = System.Drawing.Color.LightGreen
        Me.tscbxInitializationBwdCal.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.tscbxInitializationBwdCal.Items.AddRange(New Object() {"Accessible fishing mortality at TTL", "Fishing loss rate at TTL"})
        Me.tscbxInitializationBwdCal.Name = "tscbxInitializationBwdCal"
        Me.tscbxInitializationBwdCal.Size = New System.Drawing.Size(165, 21)
        '
        'tslblSlopeSelectivityTTL
        '
        Me.tslblSlopeSelectivityTTL.Name = "tslblSlopeSelectivityTTL"
        Me.tslblSlopeSelectivityTTL.Size = New System.Drawing.Size(105, 13)
        Me.tslblSlopeSelectivityTTL.Tag = ""
        Me.tslblSlopeSelectivityTTL.Text = "Slope Selectivity TTL"
        '
        'tstbxSlopeSelectivityTTL
        '
        Me.tstbxSlopeSelectivityTTL.BackColor = System.Drawing.Color.LightGreen
        Me.tstbxSlopeSelectivityTTL.Name = "tstbxSlopeSelectivityTTL"
        Me.tstbxSlopeSelectivityTTL.Size = New System.Drawing.Size(30, 21)
        '
        'tslblBeta
        '
        Me.tslblBeta.Name = "tslblBeta"
        Me.tslblBeta.Size = New System.Drawing.Size(29, 13)
        Me.tslblBeta.Text = "Beta"
        '
        'tstbxBeta
        '
        Me.tstbxBeta.BackColor = System.Drawing.Color.LightGreen
        Me.tstbxBeta.Name = "tstbxBeta"
        Me.tstbxBeta.Size = New System.Drawing.Size(30, 21)
        '
        'tslblInitializationFwdCal
        '
        Me.tslblInitializationFwdCal.Name = "tslblInitializationFwdCal"
        Me.tslblInitializationFwdCal.Size = New System.Drawing.Size(101, 13)
        Me.tslblInitializationFwdCal.Text = "Initialization fwd cal"
        '
        'tscbxInitializationFwdCal
        '
        Me.tscbxInitializationFwdCal.BackColor = System.Drawing.Color.LightGreen
        Me.tscbxInitializationFwdCal.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.tscbxInitializationFwdCal.Items.AddRange(New Object() {"Biomass at TL=1", "Biomass at TL=2", "Production at TL=1", "Production at TL=2"})
        Me.tscbxInitializationFwdCal.Name = "tscbxInitializationFwdCal"
        Me.tscbxInitializationFwdCal.Size = New System.Drawing.Size(121, 21)
        '
        'tslblRefYear
        '
        Me.tslblRefYear.Name = "tslblRefYear"
        Me.tslblRefYear.Size = New System.Drawing.Size(82, 13)
        Me.tslblRefYear.Text = "Reference year"
        '
        'tstbxRefYear
        '
        Me.tstbxRefYear.BackColor = System.Drawing.Color.LightGreen
        Me.tstbxRefYear.Name = "tstbxRefYear"
        Me.tstbxRefYear.Size = New System.Drawing.Size(30, 21)
        '
        'tslblNumYear
        '
        Me.tslblNumYear.Name = "tslblNumYear"
        Me.tslblNumYear.Size = New System.Drawing.Size(82, 13)
        Me.tslblNumYear.Text = "Number of year"
        '
        'tstbxNumYear
        '
        Me.tstbxNumYear.BackColor = System.Drawing.Color.LightGreen
        Me.tstbxNumYear.Name = "tstbxNumYear"
        Me.tstbxNumYear.Size = New System.Drawing.Size(30, 21)
        '
        'frmEcotrophGrid
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(906, 510)
        Me.Controls.Add(Me.scEcotroph1)
        Me.Controls.Add(Me.lblEcotroph)
        Me.Name = "frmEcotrophGrid"
        Me.TabText = "EcoTroph plug-in"
        Me.Text = "EcoTroph plug-in"
        Me.tpEcotroph8.ResumeLayout(False)
        CType(Me.dgvEcotroph8, System.ComponentModel.ISupportInitialize).EndInit()
        Me.scEcotroph1.Panel1.ResumeLayout(False)
        Me.scEcotroph1.Panel2.ResumeLayout(False)
        Me.scEcotroph1.Panel2.PerformLayout()
        Me.scEcotroph1.ResumeLayout(False)
        Me.scEcotroph2.Panel1.ResumeLayout(False)
        Me.scEcotroph2.Panel2.ResumeLayout(False)
        Me.scEcotroph2.ResumeLayout(False)
        Me.tcEcotroph.ResumeLayout(False)
        Me.tpEcotroph1.ResumeLayout(False)
        CType(Me.dgvEcotroph1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tpEcotroph2.ResumeLayout(False)
        CType(Me.dgvEcotroph2, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tpEcotroph3.ResumeLayout(False)
        CType(Me.dgvEcotroph3, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tpEcotroph4.ResumeLayout(False)
        CType(Me.dgvEcotroph4, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tpEcotroph5.ResumeLayout(False)
        CType(Me.dgvEcotroph5, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tpEcotroph6.ResumeLayout(False)
        CType(Me.dgvEcotroph6, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tpEcotroph7.ResumeLayout(False)
        CType(Me.dgvEcotroph7, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tpEcotroph9.ResumeLayout(False)
        CType(Me.dgvEcotroph9, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tpEcotroph10.ResumeLayout(False)
        CType(Me.dgvEcotroph10, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tpEcotroph11.ResumeLayout(False)
        CType(Me.dgvEcotroph11, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tpEcotroph12.ResumeLayout(False)
        CType(Me.dgvEcotroph12, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tpEcotroph13.ResumeLayout(False)
        CType(Me.dgvEcotroph13, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tsEcotroph.ResumeLayout(False)
        Me.tsEcotroph.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents lblEcotroph As System.Windows.Forms.Label
    Friend WithEvents scEcotroph1 As System.Windows.Forms.SplitContainer
    Friend WithEvents tvEcotroph As System.Windows.Forms.TreeView
    Friend WithEvents tsEcotroph As System.Windows.Forms.ToolStrip
    Friend WithEvents imglstEcotroph As System.Windows.Forms.ImageList
    Friend WithEvents scEcotroph2 As System.Windows.Forms.SplitContainer
    Friend WithEvents tcEcotroph As System.Windows.Forms.TabControl
    Friend WithEvents tpEcotroph1 As System.Windows.Forms.TabPage
    Friend WithEvents tpEcotroph2 As System.Windows.Forms.TabPage
    Friend WithEvents zgEcotroph As ZedGraph.ZedGraphControl
    Friend WithEvents tslblSmoothFactor As System.Windows.Forms.ToolStripLabel
    Friend WithEvents tstbxSmoothFactor As System.Windows.Forms.ToolStripTextBox
    Friend WithEvents tspgbProgressBar As System.Windows.Forms.ToolStripProgressBar
    Friend WithEvents tslblProgressBar As System.Windows.Forms.ToolStripLabel
    Friend WithEvents dgvEcotroph1 As System.Windows.Forms.DataGridView
    Friend WithEvents tpEcotroph3 As System.Windows.Forms.TabPage
    Friend WithEvents dgvEcotroph2 As System.Windows.Forms.DataGridView
    Friend WithEvents dgvEcotroph3 As System.Windows.Forms.DataGridView
    Friend WithEvents tsbtnCalculate As System.Windows.Forms.ToolStripButton
    Friend WithEvents tpEcotroph4 As System.Windows.Forms.TabPage
    Friend WithEvents tpEcotroph5 As System.Windows.Forms.TabPage
    Friend WithEvents tpEcotroph6 As System.Windows.Forms.TabPage
    Friend WithEvents tpEcotroph7 As System.Windows.Forms.TabPage
    Friend WithEvents dgvEcotroph4 As System.Windows.Forms.DataGridView
    Friend WithEvents dgvEcotroph5 As System.Windows.Forms.DataGridView
    Friend WithEvents dgvEcotroph6 As System.Windows.Forms.DataGridView
    Friend WithEvents dgvEcotroph7 As System.Windows.Forms.DataGridView
    Friend WithEvents dgvEcotroph8 As System.Windows.Forms.DataGridView
    Friend WithEvents tpEcotroph9 As System.Windows.Forms.TabPage
    Friend WithEvents tpEcotroph10 As System.Windows.Forms.TabPage
    Friend WithEvents tpEcotroph11 As System.Windows.Forms.TabPage
    Friend WithEvents tpEcotroph12 As System.Windows.Forms.TabPage
    Friend WithEvents tpEcotroph13 As System.Windows.Forms.TabPage
    Friend WithEvents dgvEcotroph9 As System.Windows.Forms.DataGridView
    Friend WithEvents dgvEcotroph10 As System.Windows.Forms.DataGridView
    Friend WithEvents dgvEcotroph11 As System.Windows.Forms.DataGridView
    Friend WithEvents dgvEcotroph12 As System.Windows.Forms.DataGridView
    Friend WithEvents dgvEcotroph13 As System.Windows.Forms.DataGridView
    Friend WithEvents tslblWaterTemp As System.Windows.Forms.ToolStripLabel
    Friend WithEvents tstbxWaterTemp As System.Windows.Forms.ToolStripTextBox
    Friend WithEvents tpEcotroph8 As System.Windows.Forms.TabPage
    Friend WithEvents tslblTETL12 As System.Windows.Forms.ToolStripLabel
    Friend WithEvents tstbxTETL12 As System.Windows.Forms.ToolStripTextBox
    Friend WithEvents tslblTETL2 As System.Windows.Forms.ToolStripLabel
    Friend WithEvents tstbxTETL2 As System.Windows.Forms.ToolStripTextBox
    Friend WithEvents tslblTopD As System.Windows.Forms.ToolStripLabel
    Friend WithEvents tstbxTopD As System.Windows.Forms.ToolStripTextBox
    Friend WithEvents tslblFormD As System.Windows.Forms.ToolStripLabel
    Friend WithEvents tstbxFormD As System.Windows.Forms.ToolStripTextBox
    Friend WithEvents tslblAsymptote As System.Windows.Forms.ToolStripLabel
    Friend WithEvents tstbxAsymptote As System.Windows.Forms.ToolStripTextBox
    Friend WithEvents tslblTL50 As System.Windows.Forms.ToolStripLabel
    Friend WithEvents tstbxTL50 As System.Windows.Forms.ToolStripTextBox
    Friend WithEvents tslblSlope As System.Windows.Forms.ToolStripLabel
    Friend WithEvents tstbxSlope As System.Windows.Forms.ToolStripTextBox
    Friend WithEvents tslblSlopeSelectivityTTL As System.Windows.Forms.ToolStripLabel
    Friend WithEvents tstbxSlopeSelectivityTTL As System.Windows.Forms.ToolStripTextBox
    Friend WithEvents tssepSeparator As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents tslblBeta As System.Windows.Forms.ToolStripLabel
    Friend WithEvents tstbxBeta As System.Windows.Forms.ToolStripTextBox
    Friend WithEvents tslblMain As System.Windows.Forms.ToolStripLabel
    Friend WithEvents tscbxMainDiagnosis As System.Windows.Forms.ToolStripComboBox
    Friend WithEvents tslblInitializationFwdCal As System.Windows.Forms.ToolStripLabel
    Friend WithEvents tscbxInitializationFwdCal As System.Windows.Forms.ToolStripComboBox
    Friend WithEvents tscbxMainDynamics As System.Windows.Forms.ToolStripComboBox
    Friend WithEvents tslblRefYear As System.Windows.Forms.ToolStripLabel
    Friend WithEvents tstbxRefYear As System.Windows.Forms.ToolStripTextBox
    Friend WithEvents tslblNumYear As System.Windows.Forms.ToolStripLabel
    Friend WithEvents tstbxNumYear As System.Windows.Forms.ToolStripTextBox
    Friend WithEvents tsbtnPlot As System.Windows.Forms.ToolStripButton
    Friend WithEvents tslblInitializationBwdCal As System.Windows.Forms.ToolStripLabel
    Friend WithEvents tscbxInitializationBwdCal As System.Windows.Forms.ToolStripComboBox
    Friend WithEvents tscbxTerminalTL As System.Windows.Forms.ToolStripComboBox
    Friend WithEvents tsbtnSetDefault As System.Windows.Forms.ToolStripButton
    Friend WithEvents tsbtnImportCatches As System.Windows.Forms.ToolStripButton
End Class
