Namespace Ecospace

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
        Partial Class frmMPAOptimizations
        Inherits frmEwE

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
            Me.m_btnRun = New System.Windows.Forms.Button
            Me.m_btnStop = New System.Windows.Forms.Button
            Me.m_ucZoom = New ScientificInterface.Ecospace.ucZoomBaseMap
            Me.m_tlbLayers = New System.Windows.Forms.TableLayoutPanel
            Me.plLayers = New System.Windows.Forms.Panel
            Me.m_lblLayers = New System.Windows.Forms.Label
            Me.m_tsMap = New System.Windows.Forms.ToolStrip
            Me.m_tsbMPA = New System.Windows.Forms.ToolStripDropDownButton
            Me.m_tsmClearMPA = New System.Windows.Forms.ToolStripMenuItem
            Me.m_tsmSetAllMPA = New System.Windows.Forms.ToolStripMenuItem
            Me.m_tsbSeed = New System.Windows.Forms.ToolStripDropDownButton
            Me.m_tsmClearSeed = New System.Windows.Forms.ToolStripMenuItem
            Me.m_tsmSetAllSeed = New System.Windows.Forms.ToolStripMenuItem
            Me.m_tsbEditLayers = New System.Windows.Forms.ToolStripButton
            Me.m_scMain = New System.Windows.Forms.SplitContainer
            Me.SplitContainer1 = New System.Windows.Forms.SplitContainer
            Me.m_tcObjectives = New System.Windows.Forms.TabControl
            Me.m_tcWeights = New System.Windows.Forms.TabPage
            Me.m_tcFleet = New System.Windows.Forms.TabPage
            Me.m_tcGroup = New System.Windows.Forms.TabPage
            Me.m_cmbMPA = New System.Windows.Forms.ComboBox
            Me.m_nudBoundaryWeight = New System.Windows.Forms.NumericUpDown
            Me.m_gbSearch = New System.Windows.Forms.GroupBox
            Me.m_rbRandom = New System.Windows.Forms.RadioButton
            Me.m_rbEcoseed = New System.Windows.Forms.RadioButton
            Me.m_lblBoundaryWeight = New System.Windows.Forms.Label
            Me.m_lblParameters = New System.Windows.Forms.Label
            Me.m_lbMPA = New System.Windows.Forms.Label
            Me.m_lblIterations = New System.Windows.Forms.Label
            Me.m_lblStep = New System.Windows.Forms.Label
            Me.m_lblMaxArea = New System.Windows.Forms.Label
            Me.m_lblMinArea = New System.Windows.Forms.Label
            Me.m_lblEndYear = New System.Windows.Forms.Label
            Me.m_nudIterations = New System.Windows.Forms.NumericUpDown
            Me.m_nudStep = New System.Windows.Forms.NumericUpDown
            Me.m_nudMaxArea = New System.Windows.Forms.NumericUpDown
            Me.m_nudMinArea = New System.Windows.Forms.NumericUpDown
            Me.m_nudEndYear = New System.Windows.Forms.NumericUpDown
            Me.m_lblStartYear = New System.Windows.Forms.Label
            Me.m_nudStartYear = New System.Windows.Forms.NumericUpDown
            Me.m_tlpRunStop = New System.Windows.Forms.TableLayoutPanel
            Me.m_lblObjectives = New System.Windows.Forms.Label
            Me.m_scMap = New System.Windows.Forms.SplitContainer
            Me.m_lblMap = New System.Windows.Forms.Label
            Me.m_tcResults = New System.Windows.Forms.TabControl
            Me.m_tpProgress = New System.Windows.Forms.TabPage
            Me.m_graphProgress = New ZedGraph.ZedGraphControl
            Me.m_gridProgress = New ScientificInterface.gridMPAOptimizations
            Me.m_tpResults = New System.Windows.Forms.TabPage
            Me.m_lblBestPercentile = New System.Windows.Forms.Label
            Me.m_gridResults = New ScientificInterface.gridMPAOptimizations
            Me.m_graphResults = New ZedGraph.ZedGraphControl
            Me.m_btnResetMPAs = New System.Windows.Forms.Button
            Me.m_btnConvertToMpa = New System.Windows.Forms.Button
            Me.m_btnNewSearch = New System.Windows.Forms.Button
            Me.m_nudBestPercentile = New System.Windows.Forms.NumericUpDown
            Me.m_lblOutput = New System.Windows.Forms.Label
            Me.m_tlbLayers.SuspendLayout()
            Me.m_tsMap.SuspendLayout()
            Me.m_scMain.Panel1.SuspendLayout()
            Me.m_scMain.Panel2.SuspendLayout()
            Me.m_scMain.SuspendLayout()
            Me.SplitContainer1.Panel1.SuspendLayout()
            Me.SplitContainer1.Panel2.SuspendLayout()
            Me.SplitContainer1.SuspendLayout()
            Me.m_tcObjectives.SuspendLayout()
            CType(Me.m_nudBoundaryWeight, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.m_gbSearch.SuspendLayout()
            CType(Me.m_nudIterations, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.m_nudStep, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.m_nudMaxArea, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.m_nudMinArea, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.m_nudEndYear, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.m_nudStartYear, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.m_tlpRunStop.SuspendLayout()
            Me.m_scMap.Panel1.SuspendLayout()
            Me.m_scMap.Panel2.SuspendLayout()
            Me.m_scMap.SuspendLayout()
            Me.m_tcResults.SuspendLayout()
            Me.m_tpProgress.SuspendLayout()
            Me.m_tpResults.SuspendLayout()
            CType(Me.m_nudBestPercentile, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            'm_btnRun
            '
            Me.m_btnRun.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_btnRun.Location = New System.Drawing.Point(3, 3)
            Me.m_btnRun.Name = "m_btnRun"
            Me.m_btnRun.Size = New System.Drawing.Size(110, 23)
            Me.m_btnRun.TabIndex = 0
            Me.m_btnRun.Text = "Run"
            Me.m_btnRun.UseVisualStyleBackColor = True
            '
            'm_btnStop
            '
            Me.m_btnStop.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_btnStop.Location = New System.Drawing.Point(119, 3)
            Me.m_btnStop.Name = "m_btnStop"
            Me.m_btnStop.Size = New System.Drawing.Size(110, 23)
            Me.m_btnStop.TabIndex = 1
            Me.m_btnStop.Text = "Stop"
            Me.m_btnStop.UseVisualStyleBackColor = True
            '
            'm_ucZoom
            '
            Me.m_ucZoom.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_ucZoom.Location = New System.Drawing.Point(0, 0)
            Me.m_ucZoom.Name = "m_ucZoom"
            Me.m_ucZoom.PositionMode = ScientificInterface.Ecospace.ucZoomBaseMap.ePositionModeTypes.Center
            Me.m_ucZoom.Size = New System.Drawing.Size(457, 484)
            Me.m_ucZoom.TabIndex = 0
            '
            'm_tlbLayers
            '
            Me.m_tlbLayers.ColumnCount = 1
            Me.m_tlbLayers.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
            Me.m_tlbLayers.Controls.Add(Me.plLayers, 0, 2)
            Me.m_tlbLayers.Controls.Add(Me.m_lblLayers, 0, 1)
            Me.m_tlbLayers.Controls.Add(Me.m_tsMap, 0, 0)
            Me.m_tlbLayers.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_tlbLayers.Location = New System.Drawing.Point(0, 0)
            Me.m_tlbLayers.Name = "m_tlbLayers"
            Me.m_tlbLayers.RowCount = 3
            Me.m_tlbLayers.RowStyles.Add(New System.Windows.Forms.RowStyle)
            Me.m_tlbLayers.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
            Me.m_tlbLayers.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
            Me.m_tlbLayers.Size = New System.Drawing.Size(180, 484)
            Me.m_tlbLayers.TabIndex = 2
            '
            'plLayers
            '
            Me.plLayers.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
            Me.plLayers.Dock = System.Windows.Forms.DockStyle.Fill
            Me.plLayers.Location = New System.Drawing.Point(0, 59)
            Me.plLayers.Margin = New System.Windows.Forms.Padding(0, 3, 0, 0)
            Me.plLayers.Name = "plLayers"
            Me.plLayers.Size = New System.Drawing.Size(180, 425)
            Me.plLayers.TabIndex = 2
            '
            'm_lblLayers
            '
            Me.m_lblLayers.AutoSize = True
            Me.m_lblLayers.BackColor = System.Drawing.SystemColors.ButtonShadow
            Me.m_lblLayers.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_lblLayers.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold)
            Me.m_lblLayers.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
            Me.m_lblLayers.ImeMode = System.Windows.Forms.ImeMode.NoControl
            Me.m_lblLayers.Location = New System.Drawing.Point(0, 36)
            Me.m_lblLayers.Margin = New System.Windows.Forms.Padding(0)
            Me.m_lblLayers.Name = "m_lblLayers"
            Me.m_lblLayers.Size = New System.Drawing.Size(180, 20)
            Me.m_lblLayers.TabIndex = 1
            Me.m_lblLayers.Text = "Layers"
            Me.m_lblLayers.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'm_tsMap
            '
            Me.m_tsMap.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tsbMPA, Me.m_tsbSeed, Me.m_tsbEditLayers})
            Me.m_tsMap.Location = New System.Drawing.Point(0, 0)
            Me.m_tsMap.Name = "m_tsMap"
            Me.m_tsMap.Size = New System.Drawing.Size(180, 36)
            Me.m_tsMap.TabIndex = 0
            Me.m_tsMap.Text = "m_tsLayers"
            '
            'm_tsbMPA
            '
            Me.m_tsbMPA.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tsmClearMPA, Me.m_tsmSetAllMPA})
            Me.m_tsbMPA.Image = Global.ScientificInterface.My.Resources.Resources.MPA1
            Me.m_tsbMPA.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.m_tsbMPA.Name = "m_tsbMPA"
            Me.m_tsbMPA.Size = New System.Drawing.Size(41, 33)
            Me.m_tsbMPA.Text = "MPA"
            Me.m_tsbMPA.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText
            '
            'm_tsmClearMPA
            '
            Me.m_tsmClearMPA.Name = "m_tsmClearMPA"
            Me.m_tsmClearMPA.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.D1), System.Windows.Forms.Keys)
            Me.m_tsmClearMPA.Size = New System.Drawing.Size(255, 22)
            Me.m_tsmClearMPA.Text = "&Clear MPA cells"
            '
            'm_tsmSetAllMPA
            '
            Me.m_tsmSetAllMPA.Name = "m_tsmSetAllMPA"
            Me.m_tsmSetAllMPA.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.D2), System.Windows.Forms.Keys)
            Me.m_tsmSetAllMPA.Size = New System.Drawing.Size(255, 22)
            Me.m_tsmSetAllMPA.Text = "&Set all cells to selected MPA"
            '
            'm_tsbSeed
            '
            Me.m_tsbSeed.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tsmClearSeed, Me.m_tsmSetAllSeed})
            Me.m_tsbSeed.Image = Global.ScientificInterface.My.Resources.Resources.help
            Me.m_tsbSeed.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.m_tsbSeed.Name = "m_tsbSeed"
            Me.m_tsbSeed.Size = New System.Drawing.Size(44, 33)
            Me.m_tsbSeed.Text = "Seed"
            Me.m_tsbSeed.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText
            '
            'm_tsmClearSeed
            '
            Me.m_tsmClearSeed.Name = "m_tsmClearSeed"
            Me.m_tsmClearSeed.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.D3), System.Windows.Forms.Keys)
            Me.m_tsmClearSeed.Size = New System.Drawing.Size(214, 22)
            Me.m_tsmClearSeed.Text = "&Clear all seed cells"
            '
            'm_tsmSetAllSeed
            '
            Me.m_tsmSetAllSeed.Name = "m_tsmSetAllSeed"
            Me.m_tsmSetAllSeed.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.D4), System.Windows.Forms.Keys)
            Me.m_tsmSetAllSeed.Size = New System.Drawing.Size(214, 22)
            Me.m_tsmSetAllSeed.Text = "&Set all cells to seed"
            '
            'm_tsbEditLayers
            '
            Me.m_tsbEditLayers.Image = Global.ScientificInterface.My.Resources.Resources.WarningHS
            Me.m_tsbEditLayers.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.m_tsbEditLayers.Name = "m_tsbEditLayers"
            Me.m_tsbEditLayers.Size = New System.Drawing.Size(66, 33)
            Me.m_tsbEditLayers.Text = "Importance"
            Me.m_tsbEditLayers.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText
            '
            'm_scMain
            '
            Me.m_scMain.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_scMain.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
            Me.m_scMain.Location = New System.Drawing.Point(1, 0)
            Me.m_scMain.Name = "m_scMain"
            Me.m_scMain.Orientation = System.Windows.Forms.Orientation.Horizontal
            '
            'm_scMain.Panel1
            '
            Me.m_scMain.Panel1.Controls.Add(Me.SplitContainer1)
            Me.m_scMain.Panel1MinSize = 418
            '
            'm_scMain.Panel2
            '
            Me.m_scMain.Panel2.Controls.Add(Me.m_tcResults)
            Me.m_scMain.Panel2.Controls.Add(Me.m_lblOutput)
            Me.m_scMain.Size = New System.Drawing.Size(885, 776)
            Me.m_scMain.SplitterDistance = 506
            Me.m_scMain.TabIndex = 0
            '
            'SplitContainer1
            '
            Me.SplitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
            Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
            Me.SplitContainer1.Location = New System.Drawing.Point(0, 0)
            Me.SplitContainer1.Name = "SplitContainer1"
            '
            'SplitContainer1.Panel1
            '
            Me.SplitContainer1.Panel1.Controls.Add(Me.m_tcObjectives)
            Me.SplitContainer1.Panel1.Controls.Add(Me.m_cmbMPA)
            Me.SplitContainer1.Panel1.Controls.Add(Me.m_nudBoundaryWeight)
            Me.SplitContainer1.Panel1.Controls.Add(Me.m_gbSearch)
            Me.SplitContainer1.Panel1.Controls.Add(Me.m_lblBoundaryWeight)
            Me.SplitContainer1.Panel1.Controls.Add(Me.m_lblParameters)
            Me.SplitContainer1.Panel1.Controls.Add(Me.m_lbMPA)
            Me.SplitContainer1.Panel1.Controls.Add(Me.m_lblIterations)
            Me.SplitContainer1.Panel1.Controls.Add(Me.m_lblStep)
            Me.SplitContainer1.Panel1.Controls.Add(Me.m_lblMaxArea)
            Me.SplitContainer1.Panel1.Controls.Add(Me.m_lblMinArea)
            Me.SplitContainer1.Panel1.Controls.Add(Me.m_lblEndYear)
            Me.SplitContainer1.Panel1.Controls.Add(Me.m_nudIterations)
            Me.SplitContainer1.Panel1.Controls.Add(Me.m_nudStep)
            Me.SplitContainer1.Panel1.Controls.Add(Me.m_nudMaxArea)
            Me.SplitContainer1.Panel1.Controls.Add(Me.m_nudMinArea)
            Me.SplitContainer1.Panel1.Controls.Add(Me.m_nudEndYear)
            Me.SplitContainer1.Panel1.Controls.Add(Me.m_lblStartYear)
            Me.SplitContainer1.Panel1.Controls.Add(Me.m_nudStartYear)
            Me.SplitContainer1.Panel1.Controls.Add(Me.m_tlpRunStop)
            Me.SplitContainer1.Panel1.Controls.Add(Me.m_lblObjectives)
            Me.SplitContainer1.Panel1MinSize = 200
            '
            'SplitContainer1.Panel2
            '
            Me.SplitContainer1.Panel2.Controls.Add(Me.m_scMap)
            Me.SplitContainer1.Panel2.Controls.Add(Me.m_lblMap)
            Me.SplitContainer1.Size = New System.Drawing.Size(885, 506)
            Me.SplitContainer1.SplitterDistance = 236
            Me.SplitContainer1.TabIndex = 0
            '
            'm_tcObjectives
            '
            Me.m_tcObjectives.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_tcObjectives.Appearance = System.Windows.Forms.TabAppearance.FlatButtons
            Me.m_tcObjectives.Controls.Add(Me.m_tcWeights)
            Me.m_tcObjectives.Controls.Add(Me.m_tcFleet)
            Me.m_tcObjectives.Controls.Add(Me.m_tcGroup)
            Me.m_tcObjectives.Location = New System.Drawing.Point(0, 280)
            Me.m_tcObjectives.Name = "m_tcObjectives"
            Me.m_tcObjectives.SelectedIndex = 0
            Me.m_tcObjectives.Size = New System.Drawing.Size(232, 162)
            Me.m_tcObjectives.TabIndex = 18
            '
            'm_tcWeights
            '
            Me.m_tcWeights.Location = New System.Drawing.Point(4, 25)
            Me.m_tcWeights.Margin = New System.Windows.Forms.Padding(0)
            Me.m_tcWeights.Name = "m_tcWeights"
            Me.m_tcWeights.Size = New System.Drawing.Size(224, 133)
            Me.m_tcWeights.TabIndex = 0
            Me.m_tcWeights.Text = "Weights"
            Me.m_tcWeights.UseVisualStyleBackColor = True
            '
            'm_tcFleet
            '
            Me.m_tcFleet.Location = New System.Drawing.Point(4, 25)
            Me.m_tcFleet.Margin = New System.Windows.Forms.Padding(0)
            Me.m_tcFleet.Name = "m_tcFleet"
            Me.m_tcFleet.Size = New System.Drawing.Size(224, 133)
            Me.m_tcFleet.TabIndex = 1
            Me.m_tcFleet.Text = "Fleet"
            Me.m_tcFleet.UseVisualStyleBackColor = True
            '
            'm_tcGroup
            '
            Me.m_tcGroup.Location = New System.Drawing.Point(4, 25)
            Me.m_tcGroup.Margin = New System.Windows.Forms.Padding(0)
            Me.m_tcGroup.Name = "m_tcGroup"
            Me.m_tcGroup.Size = New System.Drawing.Size(224, 133)
            Me.m_tcGroup.TabIndex = 2
            Me.m_tcGroup.Text = "Group"
            Me.m_tcGroup.UseVisualStyleBackColor = True
            '
            'm_cmbMPA
            '
            Me.m_cmbMPA.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_cmbMPA.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.m_cmbMPA.FormattingEnabled = True
            Me.m_cmbMPA.Location = New System.Drawing.Point(101, 177)
            Me.m_cmbMPA.Name = "m_cmbMPA"
            Me.m_cmbMPA.Size = New System.Drawing.Size(128, 21)
            Me.m_cmbMPA.TabIndex = 15
            '
            'm_nudBoundaryWeight
            '
            Me.m_nudBoundaryWeight.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_nudBoundaryWeight.DecimalPlaces = 3
            Me.m_nudBoundaryWeight.Increment = New Decimal(New Integer() {1, 0, 0, 131072})
            Me.m_nudBoundaryWeight.Location = New System.Drawing.Point(101, 448)
            Me.m_nudBoundaryWeight.Name = "m_nudBoundaryWeight"
            Me.m_nudBoundaryWeight.Size = New System.Drawing.Size(128, 20)
            Me.m_nudBoundaryWeight.TabIndex = 20
            '
            'm_gbSearch
            '
            Me.m_gbSearch.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_gbSearch.Controls.Add(Me.m_rbRandom)
            Me.m_gbSearch.Controls.Add(Me.m_rbEcoseed)
            Me.m_gbSearch.Location = New System.Drawing.Point(12, 204)
            Me.m_gbSearch.Name = "m_gbSearch"
            Me.m_gbSearch.Size = New System.Drawing.Size(217, 43)
            Me.m_gbSearch.TabIndex = 16
            Me.m_gbSearch.TabStop = False
            Me.m_gbSearch.Text = "Sea&rch"
            '
            'm_rbRandom
            '
            Me.m_rbRandom.AutoSize = True
            Me.m_rbRandom.Location = New System.Drawing.Point(95, 20)
            Me.m_rbRandom.Name = "m_rbRandom"
            Me.m_rbRandom.Size = New System.Drawing.Size(65, 17)
            Me.m_rbRandom.TabIndex = 1
            Me.m_rbRandom.Text = "Random"
            Me.m_rbRandom.UseVisualStyleBackColor = True
            '
            'm_rbEcoseed
            '
            Me.m_rbEcoseed.AutoSize = True
            Me.m_rbEcoseed.Location = New System.Drawing.Point(10, 19)
            Me.m_rbEcoseed.Name = "m_rbEcoseed"
            Me.m_rbEcoseed.Size = New System.Drawing.Size(67, 17)
            Me.m_rbEcoseed.TabIndex = 0
            Me.m_rbEcoseed.Text = "Ecoseed"
            Me.m_rbEcoseed.UseVisualStyleBackColor = True
            '
            'm_lblBoundaryWeight
            '
            Me.m_lblBoundaryWeight.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
            Me.m_lblBoundaryWeight.AutoSize = True
            Me.m_lblBoundaryWeight.Location = New System.Drawing.Point(3, 450)
            Me.m_lblBoundaryWeight.Name = "m_lblBoundaryWeight"
            Me.m_lblBoundaryWeight.Size = New System.Drawing.Size(89, 13)
            Me.m_lblBoundaryWeight.TabIndex = 19
            Me.m_lblBoundaryWeight.Text = "&Boundary weight:"
            '
            'm_lblParameters
            '
            Me.m_lblParameters.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_lblParameters.BackColor = System.Drawing.SystemColors.ControlDark
            Me.m_lblParameters.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.m_lblParameters.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
            Me.m_lblParameters.Location = New System.Drawing.Point(0, 0)
            Me.m_lblParameters.Margin = New System.Windows.Forms.Padding(0)
            Me.m_lblParameters.Name = "m_lblParameters"
            Me.m_lblParameters.Size = New System.Drawing.Size(232, 18)
            Me.m_lblParameters.TabIndex = 0
            Me.m_lblParameters.Text = "Parameters"
            Me.m_lblParameters.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'm_lbMPA
            '
            Me.m_lbMPA.AutoSize = True
            Me.m_lbMPA.Location = New System.Drawing.Point(9, 180)
            Me.m_lbMPA.Name = "m_lbMPA"
            Me.m_lbMPA.Size = New System.Drawing.Size(33, 13)
            Me.m_lbMPA.TabIndex = 14
            Me.m_lbMPA.Text = "&MPA:"
            '
            'm_lblIterations
            '
            Me.m_lblIterations.AutoSize = True
            Me.m_lblIterations.Location = New System.Drawing.Point(9, 153)
            Me.m_lblIterations.Name = "m_lblIterations"
            Me.m_lblIterations.Size = New System.Drawing.Size(53, 13)
            Me.m_lblIterations.TabIndex = 10
            Me.m_lblIterations.Text = "&Iterations:"
            '
            'm_lblStep
            '
            Me.m_lblStep.AutoSize = True
            Me.m_lblStep.Location = New System.Drawing.Point(9, 127)
            Me.m_lblStep.Name = "m_lblStep"
            Me.m_lblStep.Size = New System.Drawing.Size(49, 13)
            Me.m_lblStep.TabIndex = 8
            Me.m_lblStep.Text = "S&tep (%):"
            '
            'm_lblMaxArea
            '
            Me.m_lblMaxArea.AutoSize = True
            Me.m_lblMaxArea.Location = New System.Drawing.Point(9, 101)
            Me.m_lblMaxArea.Name = "m_lblMaxArea"
            Me.m_lblMaxArea.Size = New System.Drawing.Size(71, 13)
            Me.m_lblMaxArea.TabIndex = 6
            Me.m_lblMaxArea.Text = "Ma&x area (%):"
            '
            'm_lblMinArea
            '
            Me.m_lblMinArea.AutoSize = True
            Me.m_lblMinArea.Location = New System.Drawing.Point(9, 75)
            Me.m_lblMinArea.Name = "m_lblMinArea"
            Me.m_lblMinArea.Size = New System.Drawing.Size(68, 13)
            Me.m_lblMinArea.TabIndex = 4
            Me.m_lblMinArea.Text = "Mi&n area (%):"
            '
            'm_lblEndYear
            '
            Me.m_lblEndYear.AutoSize = True
            Me.m_lblEndYear.Location = New System.Drawing.Point(9, 49)
            Me.m_lblEndYear.Name = "m_lblEndYear"
            Me.m_lblEndYear.Size = New System.Drawing.Size(52, 13)
            Me.m_lblEndYear.TabIndex = 2
            Me.m_lblEndYear.Text = "&End year:"
            '
            'm_nudIterations
            '
            Me.m_nudIterations.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_nudIterations.Location = New System.Drawing.Point(101, 151)
            Me.m_nudIterations.Name = "m_nudIterations"
            Me.m_nudIterations.Size = New System.Drawing.Size(128, 20)
            Me.m_nudIterations.TabIndex = 11
            '
            'm_nudStep
            '
            Me.m_nudStep.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_nudStep.Location = New System.Drawing.Point(101, 125)
            Me.m_nudStep.Name = "m_nudStep"
            Me.m_nudStep.Size = New System.Drawing.Size(128, 20)
            Me.m_nudStep.TabIndex = 9
            Me.m_nudStep.Value = New Decimal(New Integer() {10, 0, 0, 0})
            '
            'm_nudMaxArea
            '
            Me.m_nudMaxArea.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_nudMaxArea.Location = New System.Drawing.Point(101, 99)
            Me.m_nudMaxArea.Name = "m_nudMaxArea"
            Me.m_nudMaxArea.Size = New System.Drawing.Size(128, 20)
            Me.m_nudMaxArea.TabIndex = 7
            '
            'm_nudMinArea
            '
            Me.m_nudMinArea.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_nudMinArea.Location = New System.Drawing.Point(101, 73)
            Me.m_nudMinArea.Name = "m_nudMinArea"
            Me.m_nudMinArea.Size = New System.Drawing.Size(128, 20)
            Me.m_nudMinArea.TabIndex = 5
            '
            'm_nudEndYear
            '
            Me.m_nudEndYear.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_nudEndYear.Location = New System.Drawing.Point(101, 47)
            Me.m_nudEndYear.Name = "m_nudEndYear"
            Me.m_nudEndYear.Size = New System.Drawing.Size(128, 20)
            Me.m_nudEndYear.TabIndex = 3
            '
            'm_lblStartYear
            '
            Me.m_lblStartYear.AutoSize = True
            Me.m_lblStartYear.Location = New System.Drawing.Point(9, 23)
            Me.m_lblStartYear.Name = "m_lblStartYear"
            Me.m_lblStartYear.Size = New System.Drawing.Size(55, 13)
            Me.m_lblStartYear.TabIndex = 0
            Me.m_lblStartYear.Text = "&Start year:"
            '
            'm_nudStartYear
            '
            Me.m_nudStartYear.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_nudStartYear.Location = New System.Drawing.Point(101, 21)
            Me.m_nudStartYear.Name = "m_nudStartYear"
            Me.m_nudStartYear.Size = New System.Drawing.Size(128, 20)
            Me.m_nudStartYear.TabIndex = 1
            '
            'm_tlpRunStop
            '
            Me.m_tlpRunStop.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_tlpRunStop.ColumnCount = 2
            Me.m_tlpRunStop.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
            Me.m_tlpRunStop.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
            Me.m_tlpRunStop.Controls.Add(Me.m_btnStop, 1, 0)
            Me.m_tlpRunStop.Controls.Add(Me.m_btnRun, 0, 0)
            Me.m_tlpRunStop.Location = New System.Drawing.Point(0, 473)
            Me.m_tlpRunStop.Margin = New System.Windows.Forms.Padding(0)
            Me.m_tlpRunStop.Name = "m_tlpRunStop"
            Me.m_tlpRunStop.RowCount = 1
            Me.m_tlpRunStop.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
            Me.m_tlpRunStop.Size = New System.Drawing.Size(232, 29)
            Me.m_tlpRunStop.TabIndex = 21
            '
            'm_lblObjectives
            '
            Me.m_lblObjectives.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_lblObjectives.BackColor = System.Drawing.SystemColors.ControlDark
            Me.m_lblObjectives.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.m_lblObjectives.ForeColor = System.Drawing.SystemColors.ControlLightLight
            Me.m_lblObjectives.Location = New System.Drawing.Point(1, 256)
            Me.m_lblObjectives.Name = "m_lblObjectives"
            Me.m_lblObjectives.Size = New System.Drawing.Size(236, 18)
            Me.m_lblObjectives.TabIndex = 17
            Me.m_lblObjectives.Text = "Objectives"
            Me.m_lblObjectives.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'm_scMap
            '
            Me.m_scMap.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_scMap.Location = New System.Drawing.Point(0, 18)
            Me.m_scMap.Margin = New System.Windows.Forms.Padding(0)
            Me.m_scMap.Name = "m_scMap"
            '
            'm_scMap.Panel1
            '
            Me.m_scMap.Panel1.Controls.Add(Me.m_ucZoom)
            '
            'm_scMap.Panel2
            '
            Me.m_scMap.Panel2.Controls.Add(Me.m_tlbLayers)
            Me.m_scMap.Size = New System.Drawing.Size(641, 484)
            Me.m_scMap.SplitterDistance = 457
            Me.m_scMap.TabIndex = 1
            '
            'm_lblMap
            '
            Me.m_lblMap.BackColor = System.Drawing.SystemColors.ControlDark
            Me.m_lblMap.Dock = System.Windows.Forms.DockStyle.Top
            Me.m_lblMap.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.m_lblMap.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
            Me.m_lblMap.Location = New System.Drawing.Point(0, 0)
            Me.m_lblMap.Margin = New System.Windows.Forms.Padding(0)
            Me.m_lblMap.Name = "m_lblMap"
            Me.m_lblMap.Size = New System.Drawing.Size(641, 18)
            Me.m_lblMap.TabIndex = 0
            Me.m_lblMap.Text = "Map"
            Me.m_lblMap.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'm_tcResults
            '
            Me.m_tcResults.Controls.Add(Me.m_tpProgress)
            Me.m_tcResults.Controls.Add(Me.m_tpResults)
            Me.m_tcResults.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_tcResults.Location = New System.Drawing.Point(0, 18)
            Me.m_tcResults.Name = "m_tcResults"
            Me.m_tcResults.SelectedIndex = 0
            Me.m_tcResults.Size = New System.Drawing.Size(881, 244)
            Me.m_tcResults.TabIndex = 0
            '
            'm_tpProgress
            '
            Me.m_tpProgress.Controls.Add(Me.m_graphProgress)
            Me.m_tpProgress.Controls.Add(Me.m_gridProgress)
            Me.m_tpProgress.Location = New System.Drawing.Point(4, 22)
            Me.m_tpProgress.Name = "m_tpProgress"
            Me.m_tpProgress.Padding = New System.Windows.Forms.Padding(3)
            Me.m_tpProgress.Size = New System.Drawing.Size(873, 218)
            Me.m_tpProgress.TabIndex = 0
            Me.m_tpProgress.Text = "Progress"
            Me.m_tpProgress.UseVisualStyleBackColor = True
            '
            'm_graphProgress
            '
            Me.m_graphProgress.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_graphProgress.IsAutoScrollRange = True
            Me.m_graphProgress.Location = New System.Drawing.Point(256, 0)
            Me.m_graphProgress.Name = "m_graphProgress"
            Me.m_graphProgress.ScrollGrace = 0
            Me.m_graphProgress.ScrollMaxX = 0
            Me.m_graphProgress.ScrollMaxY = 0
            Me.m_graphProgress.ScrollMaxY2 = 0
            Me.m_graphProgress.ScrollMinX = 0
            Me.m_graphProgress.ScrollMinY = 0
            Me.m_graphProgress.ScrollMinY2 = 0
            Me.m_graphProgress.Size = New System.Drawing.Size(617, 218)
            Me.m_graphProgress.TabIndex = 2
            '
            'm_gridProgress
            '
            Me.m_gridProgress.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
            Me.m_gridProgress.AutoSizeMinHeight = 10
            Me.m_gridProgress.AutoSizeMinWidth = 10
            Me.m_gridProgress.AutoStretchColumnsToFitWidth = False
            Me.m_gridProgress.AutoStretchRowsToFitHeight = False
            Me.m_gridProgress.BackColor = System.Drawing.Color.White
            Me.m_gridProgress.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.m_gridProgress.ContextMenuStyle = CType((((SourceGrid2.ContextMenuStyle.ColumnResize Or SourceGrid2.ContextMenuStyle.AutoSize) _
                        Or SourceGrid2.ContextMenuStyle.CopyPasteSelection) _
                        Or SourceGrid2.ContextMenuStyle.CellContextMenu), SourceGrid2.ContextMenuStyle)
            Me.m_gridProgress.CustomSort = False
            Me.m_gridProgress.FixedColumnWidths = False
            Me.m_gridProgress.FocusStyle = SourceGrid2.FocusStyle.None
            Me.m_gridProgress.GridToolTipActive = True
            Me.m_gridProgress.Location = New System.Drawing.Point(0, 0)
            Me.m_gridProgress.Name = "m_gridProgress"
            Me.m_gridProgress.Size = New System.Drawing.Size(250, 218)
            Me.m_gridProgress.SpecialKeys = CType((((((((((SourceGrid2.GridSpecialKeys.Ctrl_C Or SourceGrid2.GridSpecialKeys.Ctrl_V) _
                        Or SourceGrid2.GridSpecialKeys.Ctrl_X) _
                        Or SourceGrid2.GridSpecialKeys.Delete) _
                        Or SourceGrid2.GridSpecialKeys.Arrows) _
                        Or SourceGrid2.GridSpecialKeys.Tab) _
                        Or SourceGrid2.GridSpecialKeys.PageDownUp) _
                        Or SourceGrid2.GridSpecialKeys.Enter) _
                        Or SourceGrid2.GridSpecialKeys.Escape) _
                        Or SourceGrid2.GridSpecialKeys.Backspace), SourceGrid2.GridSpecialKeys)
            Me.m_gridProgress.TabIndex = 1
            '
            'm_tpResults
            '
            Me.m_tpResults.Controls.Add(Me.m_lblBestPercentile)
            Me.m_tpResults.Controls.Add(Me.m_gridResults)
            Me.m_tpResults.Controls.Add(Me.m_graphResults)
            Me.m_tpResults.Controls.Add(Me.m_btnResetMPAs)
            Me.m_tpResults.Controls.Add(Me.m_btnConvertToMpa)
            Me.m_tpResults.Controls.Add(Me.m_btnNewSearch)
            Me.m_tpResults.Controls.Add(Me.m_nudBestPercentile)
            Me.m_tpResults.Location = New System.Drawing.Point(4, 22)
            Me.m_tpResults.Name = "m_tpResults"
            Me.m_tpResults.Size = New System.Drawing.Size(873, 218)
            Me.m_tpResults.TabIndex = 2
            Me.m_tpResults.Text = "Results"
            Me.m_tpResults.UseVisualStyleBackColor = True
            '
            'm_lblBestPercentile
            '
            Me.m_lblBestPercentile.AutoSize = True
            Me.m_lblBestPercentile.Location = New System.Drawing.Point(-1, 8)
            Me.m_lblBestPercentile.Name = "m_lblBestPercentile"
            Me.m_lblBestPercentile.Size = New System.Drawing.Size(42, 13)
            Me.m_lblBestPercentile.TabIndex = 0
            Me.m_lblBestPercentile.Text = "&Best %:"
            '
            'm_gridResults
            '
            Me.m_gridResults.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_gridResults.AutoSizeMinHeight = 10
            Me.m_gridResults.AutoSizeMinWidth = 10
            Me.m_gridResults.AutoStretchColumnsToFitWidth = False
            Me.m_gridResults.AutoStretchRowsToFitHeight = False
            Me.m_gridResults.BackColor = System.Drawing.Color.White
            Me.m_gridResults.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.m_gridResults.ContextMenuStyle = CType((((SourceGrid2.ContextMenuStyle.ColumnResize Or SourceGrid2.ContextMenuStyle.AutoSize) _
                        Or SourceGrid2.ContextMenuStyle.CopyPasteSelection) _
                        Or SourceGrid2.ContextMenuStyle.CellContextMenu), SourceGrid2.ContextMenuStyle)
            Me.m_gridResults.CustomSort = False
            Me.m_gridResults.FixedColumnWidths = False
            Me.m_gridResults.FocusStyle = SourceGrid2.FocusStyle.None
            Me.m_gridResults.GridToolTipActive = True
            Me.m_gridResults.Location = New System.Drawing.Point(674, 32)
            Me.m_gridResults.Name = "m_gridResults"
            Me.m_gridResults.Size = New System.Drawing.Size(196, 186)
            Me.m_gridResults.SpecialKeys = CType((((((((((SourceGrid2.GridSpecialKeys.Ctrl_C Or SourceGrid2.GridSpecialKeys.Ctrl_V) _
                        Or SourceGrid2.GridSpecialKeys.Ctrl_X) _
                        Or SourceGrid2.GridSpecialKeys.Delete) _
                        Or SourceGrid2.GridSpecialKeys.Arrows) _
                        Or SourceGrid2.GridSpecialKeys.Tab) _
                        Or SourceGrid2.GridSpecialKeys.PageDownUp) _
                        Or SourceGrid2.GridSpecialKeys.Enter) _
                        Or SourceGrid2.GridSpecialKeys.Escape) _
                        Or SourceGrid2.GridSpecialKeys.Backspace), SourceGrid2.GridSpecialKeys)
            Me.m_gridResults.TabIndex = 6
            '
            'm_graphResults
            '
            Me.m_graphResults.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_graphResults.IsAutoScrollRange = True
            Me.m_graphResults.Location = New System.Drawing.Point(0, 32)
            Me.m_graphResults.Name = "m_graphResults"
            Me.m_graphResults.ScrollGrace = 0
            Me.m_graphResults.ScrollMaxX = 0
            Me.m_graphResults.ScrollMaxY = 0
            Me.m_graphResults.ScrollMaxY2 = 0
            Me.m_graphResults.ScrollMinX = 0
            Me.m_graphResults.ScrollMinY = 0
            Me.m_graphResults.ScrollMinY2 = 0
            Me.m_graphResults.Size = New System.Drawing.Size(668, 186)
            Me.m_graphResults.TabIndex = 5
            '
            'm_btnResetMPAs
            '
            Me.m_btnResetMPAs.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_btnResetMPAs.Location = New System.Drawing.Point(608, 3)
            Me.m_btnResetMPAs.Name = "m_btnResetMPAs"
            Me.m_btnResetMPAs.Size = New System.Drawing.Size(128, 23)
            Me.m_btnResetMPAs.TabIndex = 3
            Me.m_btnResetMPAs.Text = "Reset MPAs"
            Me.m_btnResetMPAs.UseVisualStyleBackColor = True
            '
            'm_btnConvertToMpa
            '
            Me.m_btnConvertToMpa.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_btnConvertToMpa.Location = New System.Drawing.Point(497, 3)
            Me.m_btnConvertToMpa.Name = "m_btnConvertToMpa"
            Me.m_btnConvertToMpa.Size = New System.Drawing.Size(105, 23)
            Me.m_btnConvertToMpa.TabIndex = 2
            Me.m_btnConvertToMpa.Text = "Convert to MPA"
            Me.m_btnConvertToMpa.UseVisualStyleBackColor = True
            '
            'm_btnNewSearch
            '
            Me.m_btnNewSearch.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_btnNewSearch.Location = New System.Drawing.Point(742, 3)
            Me.m_btnNewSearch.Name = "m_btnNewSearch"
            Me.m_btnNewSearch.Size = New System.Drawing.Size(128, 23)
            Me.m_btnNewSearch.TabIndex = 4
            Me.m_btnNewSearch.Text = "&New search"
            Me.m_btnNewSearch.UseVisualStyleBackColor = True
            '
            'm_nudBestPercentile
            '
            Me.m_nudBestPercentile.Location = New System.Drawing.Point(51, 6)
            Me.m_nudBestPercentile.Name = "m_nudBestPercentile"
            Me.m_nudBestPercentile.Size = New System.Drawing.Size(94, 20)
            Me.m_nudBestPercentile.TabIndex = 1
            '
            'm_lblOutput
            '
            Me.m_lblOutput.BackColor = System.Drawing.SystemColors.ControlDark
            Me.m_lblOutput.Dock = System.Windows.Forms.DockStyle.Top
            Me.m_lblOutput.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.m_lblOutput.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
            Me.m_lblOutput.Location = New System.Drawing.Point(0, 0)
            Me.m_lblOutput.Margin = New System.Windows.Forms.Padding(0)
            Me.m_lblOutput.Name = "m_lblOutput"
            Me.m_lblOutput.Size = New System.Drawing.Size(881, 18)
            Me.m_lblOutput.TabIndex = 0
            Me.m_lblOutput.Text = "Output"
            Me.m_lblOutput.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'frmMPAOptimizations
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(885, 775)
            Me.Controls.Add(Me.m_scMain)
            Me.DoubleBuffered = True
            Me.Name = "frmMPAOptimizations"
            Me.Text = "Ecoseed"
            Me.m_tlbLayers.ResumeLayout(False)
            Me.m_tlbLayers.PerformLayout()
            Me.m_tsMap.ResumeLayout(False)
            Me.m_tsMap.PerformLayout()
            Me.m_scMain.Panel1.ResumeLayout(False)
            Me.m_scMain.Panel2.ResumeLayout(False)
            Me.m_scMain.ResumeLayout(False)
            Me.SplitContainer1.Panel1.ResumeLayout(False)
            Me.SplitContainer1.Panel1.PerformLayout()
            Me.SplitContainer1.Panel2.ResumeLayout(False)
            Me.SplitContainer1.ResumeLayout(False)
            Me.m_tcObjectives.ResumeLayout(False)
            CType(Me.m_nudBoundaryWeight, System.ComponentModel.ISupportInitialize).EndInit()
            Me.m_gbSearch.ResumeLayout(False)
            Me.m_gbSearch.PerformLayout()
            CType(Me.m_nudIterations, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.m_nudStep, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.m_nudMaxArea, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.m_nudMinArea, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.m_nudEndYear, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.m_nudStartYear, System.ComponentModel.ISupportInitialize).EndInit()
            Me.m_tlpRunStop.ResumeLayout(False)
            Me.m_scMap.Panel1.ResumeLayout(False)
            Me.m_scMap.Panel2.ResumeLayout(False)
            Me.m_scMap.ResumeLayout(False)
            Me.m_tcResults.ResumeLayout(False)
            Me.m_tpProgress.ResumeLayout(False)
            Me.m_tpResults.ResumeLayout(False)
            Me.m_tpResults.PerformLayout()
            CType(Me.m_nudBestPercentile, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)

        End Sub
        Friend WithEvents m_btnRun As System.Windows.Forms.Button
        Friend WithEvents m_btnStop As System.Windows.Forms.Button
        Friend WithEvents m_ucZoom As ucZoomBaseMap
        Friend WithEvents m_tlbLayers As System.Windows.Forms.TableLayoutPanel
        Friend WithEvents m_lblLayers As System.Windows.Forms.Label
        Friend WithEvents m_tsMap As System.Windows.Forms.ToolStrip
        Friend WithEvents m_tsbMPA As System.Windows.Forms.ToolStripDropDownButton
        Friend WithEvents m_tsmClearMPA As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents m_tsmSetAllMPA As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents m_tsbSeed As System.Windows.Forms.ToolStripDropDownButton
        Friend WithEvents m_tsmClearSeed As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents m_tsmSetAllSeed As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents plLayers As System.Windows.Forms.Panel
        Friend WithEvents m_scMain As System.Windows.Forms.SplitContainer
        Friend WithEvents m_lblParameters As System.Windows.Forms.Label
        Friend WithEvents m_lblMap As System.Windows.Forms.Label
        Friend WithEvents m_lblOutput As System.Windows.Forms.Label
        Friend WithEvents m_nudBoundaryWeight As System.Windows.Forms.NumericUpDown
        Friend WithEvents m_lblBoundaryWeight As System.Windows.Forms.Label
        Friend WithEvents m_lblEndYear As System.Windows.Forms.Label
        Friend WithEvents m_lblStartYear As System.Windows.Forms.Label
        Friend WithEvents m_nudStartYear As System.Windows.Forms.NumericUpDown
        Friend WithEvents m_nudEndYear As System.Windows.Forms.NumericUpDown
        Friend WithEvents m_tlpRunStop As System.Windows.Forms.TableLayoutPanel
        Friend WithEvents m_lblObjectives As System.Windows.Forms.Label
        Friend WithEvents SplitContainer1 As System.Windows.Forms.SplitContainer
        Friend WithEvents m_tcResults As System.Windows.Forms.TabControl
        Friend WithEvents m_tpProgress As System.Windows.Forms.TabPage
        Friend WithEvents m_scMap As System.Windows.Forms.SplitContainer
        Friend WithEvents m_lblMaxArea As System.Windows.Forms.Label
        Friend WithEvents m_lblMinArea As System.Windows.Forms.Label
        Friend WithEvents m_nudStep As System.Windows.Forms.NumericUpDown
        Friend WithEvents m_nudMaxArea As System.Windows.Forms.NumericUpDown
        Friend WithEvents m_nudMinArea As System.Windows.Forms.NumericUpDown
        Friend WithEvents m_lblStep As System.Windows.Forms.Label
        Friend WithEvents m_lblIterations As System.Windows.Forms.Label
        Friend WithEvents m_nudIterations As System.Windows.Forms.NumericUpDown
        Friend WithEvents m_gbSearch As System.Windows.Forms.GroupBox
        Friend WithEvents m_rbRandom As System.Windows.Forms.RadioButton
        Friend WithEvents m_rbEcoseed As System.Windows.Forms.RadioButton
        Friend WithEvents m_cmbMPA As System.Windows.Forms.ComboBox
        Friend WithEvents m_lbMPA As System.Windows.Forms.Label
        Friend WithEvents m_tcObjectives As System.Windows.Forms.TabControl
        Friend WithEvents m_tcWeights As System.Windows.Forms.TabPage
        Friend WithEvents m_tcFleet As System.Windows.Forms.TabPage
        Friend WithEvents m_tcGroup As System.Windows.Forms.TabPage
        Friend WithEvents m_tsbEditLayers As System.Windows.Forms.ToolStripButton
        Friend WithEvents m_gridProgress As ScientificInterface.gridMPAOptimizations
        Friend WithEvents m_tpResults As System.Windows.Forms.TabPage
        Friend WithEvents m_graphProgress As ZedGraph.ZedGraphControl
        Friend WithEvents m_btnNewSearch As System.Windows.Forms.Button
        Private WithEvents m_nudBestPercentile As System.Windows.Forms.NumericUpDown
        Private WithEvents m_btnConvertToMpa As System.Windows.Forms.Button
        Private WithEvents m_btnResetMPAs As System.Windows.Forms.Button
        Private WithEvents m_lblBestPercentile As System.Windows.Forms.Label
        Private WithEvents m_graphResults As ZedGraph.ZedGraphControl
        Private WithEvents m_gridResults As ScientificInterface.gridMPAOptimizations
    End Class
End Namespace
