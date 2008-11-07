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
            Me.m_plLayers = New System.Windows.Forms.Panel
            Me.m_lblLayers = New System.Windows.Forms.Label
            Me.m_tsMap = New System.Windows.Forms.ToolStrip
            Me.m_tsbMPA = New System.Windows.Forms.ToolStripDropDownButton
            Me.m_tsmClearMPA = New System.Windows.Forms.ToolStripMenuItem
            Me.m_tsmSetAllMPA = New System.Windows.Forms.ToolStripMenuItem
            Me.m_tsbSeed = New System.Windows.Forms.ToolStripDropDownButton
            Me.m_tsmClearSeed = New System.Windows.Forms.ToolStripMenuItem
            Me.m_tsmSetAllSeed = New System.Windows.Forms.ToolStripMenuItem
            Me.m_tsbEditLayers = New System.Windows.Forms.ToolStripButton
            Me.m_cmbMPA = New System.Windows.Forms.ComboBox
            Me.m_rbRandom = New System.Windows.Forms.RadioButton
            Me.m_rbEcoseed = New System.Windows.Forms.RadioButton
            Me.m_lblParameters = New System.Windows.Forms.Label
            Me.m_lbMPA = New System.Windows.Forms.Label
            Me.m_lblEndYear = New System.Windows.Forms.Label
            Me.m_nudIterations = New System.Windows.Forms.NumericUpDown
            Me.m_nudStep = New System.Windows.Forms.NumericUpDown
            Me.m_nudEndYear = New System.Windows.Forms.NumericUpDown
            Me.m_lblStartYear = New System.Windows.Forms.Label
            Me.m_nudStartYear = New System.Windows.Forms.NumericUpDown
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
            Me.m_nudBestPercentile = New System.Windows.Forms.NumericUpDown
            Me.m_btnNewSearch = New System.Windows.Forms.Button
            Me.m_lblOutput = New System.Windows.Forms.Label
            Me.m_lblSearchType = New System.Windows.Forms.Label
            Me.m_tlbParameters = New System.Windows.Forms.TableLayoutPanel
            Me.m_lblMinArea = New System.Windows.Forms.Label
            Me.m_lblMaxArea = New System.Windows.Forms.Label
            Me.m_nudMinArea = New System.Windows.Forms.NumericUpDown
            Me.m_nudMaxArea = New System.Windows.Forms.NumericUpDown
            Me.m_lblStep = New System.Windows.Forms.Label
            Me.m_lblIterations = New System.Windows.Forms.Label
            Me.m_lblBoundaryWeight = New System.Windows.Forms.Label
            Me.m_nudBoundaryWeight = New System.Windows.Forms.NumericUpDown
            Me.m_scMap = New System.Windows.Forms.SplitContainer
            Me.m_tcConfiguration = New System.Windows.Forms.TabControl
            Me.m_tabParameters = New System.Windows.Forms.TabPage
            Me.m_tlpObjectives = New System.Windows.Forms.TableLayoutPanel
            Me.m_lblWeight = New System.Windows.Forms.Label
            Me.m_plGroup = New System.Windows.Forms.Panel
            Me.m_plFleet = New System.Windows.Forms.Panel
            Me.m_plWeights = New System.Windows.Forms.Panel
            Me.m_lbFleet = New System.Windows.Forms.Label
            Me.m_lblGroup = New System.Windows.Forms.Label
            Me.m_tabMap = New System.Windows.Forms.TabPage
            Me.Label1 = New System.Windows.Forms.Label
            Me.m_tabCredits = New System.Windows.Forms.TabPage
            Me.m_tlbLayers.SuspendLayout()
            Me.m_tsMap.SuspendLayout()
            CType(Me.m_nudIterations, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.m_nudStep, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.m_nudEndYear, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.m_nudStartYear, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.m_tcResults.SuspendLayout()
            Me.m_tpProgress.SuspendLayout()
            Me.m_tpResults.SuspendLayout()
            CType(Me.m_nudBestPercentile, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.m_tlbParameters.SuspendLayout()
            CType(Me.m_nudMinArea, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.m_nudMaxArea, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.m_nudBoundaryWeight, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.m_scMap.Panel1.SuspendLayout()
            Me.m_scMap.Panel2.SuspendLayout()
            Me.m_scMap.SuspendLayout()
            Me.m_tcConfiguration.SuspendLayout()
            Me.m_tabParameters.SuspendLayout()
            Me.m_tlpObjectives.SuspendLayout()
            Me.m_tabMap.SuspendLayout()
            Me.SuspendLayout()
            '
            'm_btnRun
            '
            Me.m_btnRun.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_btnRun.Location = New System.Drawing.Point(487, 23)
            Me.m_btnRun.Name = "m_btnRun"
            Me.m_btnRun.Size = New System.Drawing.Size(96, 23)
            Me.m_btnRun.TabIndex = 4
            Me.m_btnRun.Text = "&Run"
            Me.m_btnRun.UseVisualStyleBackColor = True
            '
            'm_btnStop
            '
            Me.m_btnStop.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_btnStop.DialogResult = System.Windows.Forms.DialogResult.Cancel
            Me.m_btnStop.Location = New System.Drawing.Point(589, 23)
            Me.m_btnStop.Name = "m_btnStop"
            Me.m_btnStop.Size = New System.Drawing.Size(96, 23)
            Me.m_btnStop.TabIndex = 5
            Me.m_btnStop.Text = "&Stop"
            Me.m_btnStop.UseVisualStyleBackColor = True
            '
            'm_ucZoom
            '
            Me.m_ucZoom.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_ucZoom.Location = New System.Drawing.Point(0, 0)
            Me.m_ucZoom.Name = "m_ucZoom"
            Me.m_ucZoom.PositionMode = ScientificInterface.Ecospace.ucZoomBaseMap.ePositionModeTypes.Center
            Me.m_ucZoom.Size = New System.Drawing.Size(609, 445)
            Me.m_ucZoom.TabIndex = 0
            '
            'm_tlbLayers
            '
            Me.m_tlbLayers.ColumnCount = 1
            Me.m_tlbLayers.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
            Me.m_tlbLayers.Controls.Add(Me.m_plLayers, 0, 2)
            Me.m_tlbLayers.Controls.Add(Me.m_lblLayers, 0, 1)
            Me.m_tlbLayers.Controls.Add(Me.m_tsMap, 0, 0)
            Me.m_tlbLayers.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_tlbLayers.Location = New System.Drawing.Point(0, 0)
            Me.m_tlbLayers.Name = "m_tlbLayers"
            Me.m_tlbLayers.RowCount = 3
            Me.m_tlbLayers.RowStyles.Add(New System.Windows.Forms.RowStyle)
            Me.m_tlbLayers.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
            Me.m_tlbLayers.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
            Me.m_tlbLayers.Size = New System.Drawing.Size(161, 445)
            Me.m_tlbLayers.TabIndex = 2
            '
            'm_plLayers
            '
            Me.m_plLayers.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
            Me.m_plLayers.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_plLayers.Location = New System.Drawing.Point(0, 59)
            Me.m_plLayers.Margin = New System.Windows.Forms.Padding(0, 3, 0, 0)
            Me.m_plLayers.Name = "m_plLayers"
            Me.m_plLayers.Size = New System.Drawing.Size(161, 386)
            Me.m_plLayers.TabIndex = 2
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
            Me.m_lblLayers.Size = New System.Drawing.Size(161, 20)
            Me.m_lblLayers.TabIndex = 1
            Me.m_lblLayers.Text = "Layers"
            Me.m_lblLayers.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'm_tsMap
            '
            Me.m_tsMap.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tsbMPA, Me.m_tsbSeed, Me.m_tsbEditLayers})
            Me.m_tsMap.Location = New System.Drawing.Point(0, 0)
            Me.m_tsMap.Name = "m_tsMap"
            Me.m_tsMap.Size = New System.Drawing.Size(161, 36)
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
            'm_cmbMPA
            '
            Me.m_cmbMPA.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_cmbMPA.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.m_cmbMPA.FormattingEnabled = True
            Me.m_cmbMPA.Location = New System.Drawing.Point(613, 3)
            Me.m_cmbMPA.Name = "m_cmbMPA"
            Me.m_cmbMPA.Size = New System.Drawing.Size(164, 21)
            Me.m_cmbMPA.TabIndex = 13
            '
            'm_rbRandom
            '
            Me.m_rbRandom.AutoSize = True
            Me.m_rbRandom.Location = New System.Drawing.Point(158, 26)
            Me.m_rbRandom.Name = "m_rbRandom"
            Me.m_rbRandom.Size = New System.Drawing.Size(65, 17)
            Me.m_rbRandom.TabIndex = 3
            Me.m_rbRandom.Text = "Random"
            Me.m_rbRandom.UseVisualStyleBackColor = True
            '
            'm_rbEcoseed
            '
            Me.m_rbEcoseed.AutoSize = True
            Me.m_rbEcoseed.Location = New System.Drawing.Point(85, 26)
            Me.m_rbEcoseed.Name = "m_rbEcoseed"
            Me.m_rbEcoseed.Size = New System.Drawing.Size(67, 17)
            Me.m_rbEcoseed.TabIndex = 2
            Me.m_rbEcoseed.Text = "Ecoseed"
            Me.m_rbEcoseed.UseVisualStyleBackColor = True
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
            Me.m_lblParameters.Size = New System.Drawing.Size(793, 18)
            Me.m_lblParameters.TabIndex = 0
            Me.m_lblParameters.Text = "Search types"
            Me.m_lblParameters.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'm_lbMPA
            '
            Me.m_lbMPA.AutoSize = True
            Me.m_lbMPA.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_lbMPA.Location = New System.Drawing.Point(518, 0)
            Me.m_lbMPA.Name = "m_lbMPA"
            Me.m_lbMPA.Size = New System.Drawing.Size(89, 27)
            Me.m_lbMPA.TabIndex = 12
            Me.m_lbMPA.Text = "&MPA:"
            Me.m_lbMPA.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'm_lblEndYear
            '
            Me.m_lblEndYear.AutoSize = True
            Me.m_lblEndYear.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_lblEndYear.Location = New System.Drawing.Point(3, 27)
            Me.m_lblEndYear.Name = "m_lblEndYear"
            Me.m_lblEndYear.Size = New System.Drawing.Size(55, 26)
            Me.m_lblEndYear.TabIndex = 2
            Me.m_lblEndYear.Text = "&End year:"
            Me.m_lblEndYear.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'm_nudIterations
            '
            Me.m_nudIterations.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_nudIterations.Location = New System.Drawing.Point(64, 56)
            Me.m_nudIterations.Name = "m_nudIterations"
            Me.m_nudIterations.Size = New System.Drawing.Size(163, 20)
            Me.m_nudIterations.TabIndex = 5
            '
            'm_nudStep
            '
            Me.m_nudStep.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_nudStep.Location = New System.Drawing.Point(330, 56)
            Me.m_nudStep.Name = "m_nudStep"
            Me.m_nudStep.Size = New System.Drawing.Size(162, 20)
            Me.m_nudStep.TabIndex = 11
            Me.m_nudStep.Value = New Decimal(New Integer() {10, 0, 0, 0})
            '
            'm_nudEndYear
            '
            Me.m_nudEndYear.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_nudEndYear.Location = New System.Drawing.Point(64, 30)
            Me.m_nudEndYear.Name = "m_nudEndYear"
            Me.m_nudEndYear.Size = New System.Drawing.Size(163, 20)
            Me.m_nudEndYear.TabIndex = 3
            '
            'm_lblStartYear
            '
            Me.m_lblStartYear.AutoSize = True
            Me.m_lblStartYear.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_lblStartYear.Location = New System.Drawing.Point(3, 0)
            Me.m_lblStartYear.Name = "m_lblStartYear"
            Me.m_lblStartYear.Size = New System.Drawing.Size(55, 27)
            Me.m_lblStartYear.TabIndex = 0
            Me.m_lblStartYear.Text = "&Start year:"
            Me.m_lblStartYear.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'm_nudStartYear
            '
            Me.m_nudStartYear.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_nudStartYear.Location = New System.Drawing.Point(64, 3)
            Me.m_nudStartYear.Name = "m_nudStartYear"
            Me.m_nudStartYear.Size = New System.Drawing.Size(163, 20)
            Me.m_nudStartYear.TabIndex = 1
            '
            'm_tcResults
            '
            Me.m_tcResults.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_tcResults.Controls.Add(Me.m_tpProgress)
            Me.m_tcResults.Controls.Add(Me.m_tpResults)
            Me.m_tcResults.Location = New System.Drawing.Point(-1, 576)
            Me.m_tcResults.Name = "m_tcResults"
            Me.m_tcResults.SelectedIndex = 0
            Me.m_tcResults.Size = New System.Drawing.Size(792, 218)
            Me.m_tcResults.TabIndex = 10
            '
            'm_tpProgress
            '
            Me.m_tpProgress.Controls.Add(Me.m_graphProgress)
            Me.m_tpProgress.Controls.Add(Me.m_gridProgress)
            Me.m_tpProgress.Location = New System.Drawing.Point(4, 22)
            Me.m_tpProgress.Name = "m_tpProgress"
            Me.m_tpProgress.Padding = New System.Windows.Forms.Padding(3)
            Me.m_tpProgress.Size = New System.Drawing.Size(784, 192)
            Me.m_tpProgress.TabIndex = 0
            Me.m_tpProgress.Text = "Progress"
            Me.m_tpProgress.UseVisualStyleBackColor = True
            '
            'm_graphProgress
            '
            Me.m_graphProgress.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
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
            Me.m_graphProgress.Size = New System.Drawing.Size(528, 192)
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
            Me.m_tpResults.Controls.Add(Me.m_nudBestPercentile)
            Me.m_tpResults.Location = New System.Drawing.Point(4, 22)
            Me.m_tpResults.Name = "m_tpResults"
            Me.m_tpResults.Size = New System.Drawing.Size(784, 192)
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
            Me.m_gridResults.Location = New System.Drawing.Point(577, 32)
            Me.m_gridResults.Name = "m_gridResults"
            Me.m_gridResults.Size = New System.Drawing.Size(207, 160)
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
            Me.m_graphResults.Size = New System.Drawing.Size(571, 160)
            Me.m_graphResults.TabIndex = 5
            '
            'm_btnResetMPAs
            '
            Me.m_btnResetMPAs.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_btnResetMPAs.Location = New System.Drawing.Point(688, 3)
            Me.m_btnResetMPAs.Name = "m_btnResetMPAs"
            Me.m_btnResetMPAs.Size = New System.Drawing.Size(96, 23)
            Me.m_btnResetMPAs.TabIndex = 3
            Me.m_btnResetMPAs.Text = "Reset MPAs"
            Me.m_btnResetMPAs.UseVisualStyleBackColor = True
            '
            'm_btnConvertToMpa
            '
            Me.m_btnConvertToMpa.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_btnConvertToMpa.Location = New System.Drawing.Point(577, 3)
            Me.m_btnConvertToMpa.Name = "m_btnConvertToMpa"
            Me.m_btnConvertToMpa.Size = New System.Drawing.Size(105, 23)
            Me.m_btnConvertToMpa.TabIndex = 2
            Me.m_btnConvertToMpa.Text = "Convert to MPA"
            Me.m_btnConvertToMpa.UseVisualStyleBackColor = True
            '
            'm_nudBestPercentile
            '
            Me.m_nudBestPercentile.Location = New System.Drawing.Point(51, 6)
            Me.m_nudBestPercentile.Name = "m_nudBestPercentile"
            Me.m_nudBestPercentile.Size = New System.Drawing.Size(94, 20)
            Me.m_nudBestPercentile.TabIndex = 1
            '
            'm_btnNewSearch
            '
            Me.m_btnNewSearch.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_btnNewSearch.Location = New System.Drawing.Point(691, 23)
            Me.m_btnNewSearch.Name = "m_btnNewSearch"
            Me.m_btnNewSearch.Size = New System.Drawing.Size(96, 23)
            Me.m_btnNewSearch.TabIndex = 6
            Me.m_btnNewSearch.Text = "&New search"
            Me.m_btnNewSearch.UseVisualStyleBackColor = True
            '
            'm_lblOutput
            '
            Me.m_lblOutput.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_lblOutput.BackColor = System.Drawing.SystemColors.ControlDark
            Me.m_lblOutput.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.m_lblOutput.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
            Me.m_lblOutput.Location = New System.Drawing.Point(0, 555)
            Me.m_lblOutput.Margin = New System.Windows.Forms.Padding(0)
            Me.m_lblOutput.Name = "m_lblOutput"
            Me.m_lblOutput.Size = New System.Drawing.Size(791, 18)
            Me.m_lblOutput.TabIndex = 9
            Me.m_lblOutput.Text = "Output"
            Me.m_lblOutput.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'm_lblSearchType
            '
            Me.m_lblSearchType.AutoSize = True
            Me.m_lblSearchType.Location = New System.Drawing.Point(4, 28)
            Me.m_lblSearchType.Name = "m_lblSearchType"
            Me.m_lblSearchType.Size = New System.Drawing.Size(67, 13)
            Me.m_lblSearchType.TabIndex = 1
            Me.m_lblSearchType.Text = "&Search type:"
            Me.m_lblSearchType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'm_tlbParameters
            '
            Me.m_tlbParameters.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_tlbParameters.ColumnCount = 8
            Me.m_tlbParameters.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle)
            Me.m_tlbParameters.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33434!))
            Me.m_tlbParameters.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
            Me.m_tlbParameters.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle)
            Me.m_tlbParameters.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33283!))
            Me.m_tlbParameters.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
            Me.m_tlbParameters.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle)
            Me.m_tlbParameters.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33283!))
            Me.m_tlbParameters.Controls.Add(Me.m_lblStartYear, 0, 0)
            Me.m_tlbParameters.Controls.Add(Me.m_lblEndYear, 0, 1)
            Me.m_tlbParameters.Controls.Add(Me.m_nudStartYear, 1, 0)
            Me.m_tlbParameters.Controls.Add(Me.m_nudEndYear, 1, 1)
            Me.m_tlbParameters.Controls.Add(Me.m_lblMinArea, 3, 0)
            Me.m_tlbParameters.Controls.Add(Me.m_lblMaxArea, 3, 1)
            Me.m_tlbParameters.Controls.Add(Me.m_nudMinArea, 4, 0)
            Me.m_tlbParameters.Controls.Add(Me.m_nudMaxArea, 4, 1)
            Me.m_tlbParameters.Controls.Add(Me.m_lblBoundaryWeight, 6, 2)
            Me.m_tlbParameters.Controls.Add(Me.m_lblStep, 3, 2)
            Me.m_tlbParameters.Controls.Add(Me.m_nudStep, 4, 2)
            Me.m_tlbParameters.Controls.Add(Me.m_cmbMPA, 7, 0)
            Me.m_tlbParameters.Controls.Add(Me.m_lbMPA, 6, 0)
            Me.m_tlbParameters.Controls.Add(Me.m_lblIterations, 0, 2)
            Me.m_tlbParameters.Controls.Add(Me.m_nudIterations, 1, 2)
            Me.m_tlbParameters.Controls.Add(Me.m_nudBoundaryWeight, 7, 2)
            Me.m_tlbParameters.Location = New System.Drawing.Point(0, 0)
            Me.m_tlbParameters.Name = "m_tlbParameters"
            Me.m_tlbParameters.RowCount = 3
            Me.m_tlbParameters.RowStyles.Add(New System.Windows.Forms.RowStyle)
            Me.m_tlbParameters.RowStyles.Add(New System.Windows.Forms.RowStyle)
            Me.m_tlbParameters.RowStyles.Add(New System.Windows.Forms.RowStyle)
            Me.m_tlbParameters.Size = New System.Drawing.Size(780, 78)
            Me.m_tlbParameters.TabIndex = 0
            '
            'm_lblMinArea
            '
            Me.m_lblMinArea.AutoSize = True
            Me.m_lblMinArea.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_lblMinArea.Location = New System.Drawing.Point(253, 0)
            Me.m_lblMinArea.Name = "m_lblMinArea"
            Me.m_lblMinArea.Size = New System.Drawing.Size(71, 27)
            Me.m_lblMinArea.TabIndex = 6
            Me.m_lblMinArea.Text = "Mi&n area (%):"
            Me.m_lblMinArea.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'm_lblMaxArea
            '
            Me.m_lblMaxArea.AutoSize = True
            Me.m_lblMaxArea.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_lblMaxArea.Location = New System.Drawing.Point(253, 27)
            Me.m_lblMaxArea.Name = "m_lblMaxArea"
            Me.m_lblMaxArea.Size = New System.Drawing.Size(71, 26)
            Me.m_lblMaxArea.TabIndex = 8
            Me.m_lblMaxArea.Text = "Ma&x area (%):"
            Me.m_lblMaxArea.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'm_nudMinArea
            '
            Me.m_nudMinArea.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_nudMinArea.Location = New System.Drawing.Point(330, 3)
            Me.m_nudMinArea.Name = "m_nudMinArea"
            Me.m_nudMinArea.Size = New System.Drawing.Size(162, 20)
            Me.m_nudMinArea.TabIndex = 7
            '
            'm_nudMaxArea
            '
            Me.m_nudMaxArea.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_nudMaxArea.Location = New System.Drawing.Point(330, 30)
            Me.m_nudMaxArea.Name = "m_nudMaxArea"
            Me.m_nudMaxArea.Size = New System.Drawing.Size(162, 20)
            Me.m_nudMaxArea.TabIndex = 9
            '
            'm_lblStep
            '
            Me.m_lblStep.AutoSize = True
            Me.m_lblStep.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_lblStep.Location = New System.Drawing.Point(253, 53)
            Me.m_lblStep.Name = "m_lblStep"
            Me.m_lblStep.Size = New System.Drawing.Size(71, 26)
            Me.m_lblStep.TabIndex = 10
            Me.m_lblStep.Text = "S&tep (%):"
            Me.m_lblStep.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'm_lblIterations
            '
            Me.m_lblIterations.AutoSize = True
            Me.m_lblIterations.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_lblIterations.Location = New System.Drawing.Point(3, 53)
            Me.m_lblIterations.Name = "m_lblIterations"
            Me.m_lblIterations.Size = New System.Drawing.Size(55, 26)
            Me.m_lblIterations.TabIndex = 4
            Me.m_lblIterations.Text = "&Iterations:"
            Me.m_lblIterations.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'm_lblBoundaryWeight
            '
            Me.m_lblBoundaryWeight.AutoSize = True
            Me.m_lblBoundaryWeight.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_lblBoundaryWeight.Location = New System.Drawing.Point(518, 53)
            Me.m_lblBoundaryWeight.Name = "m_lblBoundaryWeight"
            Me.m_lblBoundaryWeight.Size = New System.Drawing.Size(89, 26)
            Me.m_lblBoundaryWeight.TabIndex = 14
            Me.m_lblBoundaryWeight.Text = "&Boundary weight:"
            Me.m_lblBoundaryWeight.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'm_nudBoundaryWeight
            '
            Me.m_nudBoundaryWeight.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_nudBoundaryWeight.Location = New System.Drawing.Point(613, 56)
            Me.m_nudBoundaryWeight.Name = "m_nudBoundaryWeight"
            Me.m_nudBoundaryWeight.Size = New System.Drawing.Size(164, 20)
            Me.m_nudBoundaryWeight.TabIndex = 15
            '
            'm_scMap
            '
            Me.m_scMap.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_scMap.FixedPanel = System.Windows.Forms.FixedPanel.Panel2
            Me.m_scMap.Location = New System.Drawing.Point(3, 3)
            Me.m_scMap.Name = "m_scMap"
            '
            'm_scMap.Panel1
            '
            Me.m_scMap.Panel1.Controls.Add(Me.m_ucZoom)
            '
            'm_scMap.Panel2
            '
            Me.m_scMap.Panel2.Controls.Add(Me.m_tlbLayers)
            Me.m_scMap.Size = New System.Drawing.Size(774, 445)
            Me.m_scMap.SplitterDistance = 609
            Me.m_scMap.TabIndex = 6
            '
            'm_tcConfiguration
            '
            Me.m_tcConfiguration.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_tcConfiguration.Controls.Add(Me.m_tabParameters)
            Me.m_tcConfiguration.Controls.Add(Me.m_tabMap)
            Me.m_tcConfiguration.Controls.Add(Me.m_tabCredits)
            Me.m_tcConfiguration.Location = New System.Drawing.Point(3, 73)
            Me.m_tcConfiguration.Name = "m_tcConfiguration"
            Me.m_tcConfiguration.SelectedIndex = 0
            Me.m_tcConfiguration.Size = New System.Drawing.Size(788, 477)
            Me.m_tcConfiguration.TabIndex = 8
            '
            'm_tabParameters
            '
            Me.m_tabParameters.Controls.Add(Me.m_tlpObjectives)
            Me.m_tabParameters.Controls.Add(Me.m_tlbParameters)
            Me.m_tabParameters.Location = New System.Drawing.Point(4, 22)
            Me.m_tabParameters.Name = "m_tabParameters"
            Me.m_tabParameters.Padding = New System.Windows.Forms.Padding(3)
            Me.m_tabParameters.Size = New System.Drawing.Size(780, 451)
            Me.m_tabParameters.TabIndex = 0
            Me.m_tabParameters.Text = "Parameters and objectives"
            Me.m_tabParameters.UseVisualStyleBackColor = True
            '
            'm_tlpObjectives
            '
            Me.m_tlpObjectives.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_tlpObjectives.ColumnCount = 5
            Me.m_tlpObjectives.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 28.57143!))
            Me.m_tlpObjectives.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 6.0!))
            Me.m_tlpObjectives.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 28.57143!))
            Me.m_tlpObjectives.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 6.0!))
            Me.m_tlpObjectives.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 42.85714!))
            Me.m_tlpObjectives.Controls.Add(Me.m_lblWeight, 0, 0)
            Me.m_tlpObjectives.Controls.Add(Me.m_plGroup, 4, 1)
            Me.m_tlpObjectives.Controls.Add(Me.m_plFleet, 2, 1)
            Me.m_tlpObjectives.Controls.Add(Me.m_plWeights, 0, 1)
            Me.m_tlpObjectives.Controls.Add(Me.m_lbFleet, 2, 0)
            Me.m_tlpObjectives.Controls.Add(Me.m_lblGroup, 4, 0)
            Me.m_tlpObjectives.Location = New System.Drawing.Point(0, 85)
            Me.m_tlpObjectives.Name = "m_tlpObjectives"
            Me.m_tlpObjectives.RowCount = 2
            Me.m_tlpObjectives.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 18.0!))
            Me.m_tlpObjectives.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
            Me.m_tlpObjectives.Size = New System.Drawing.Size(780, 366)
            Me.m_tlpObjectives.TabIndex = 6
            '
            'm_lblWeight
            '
            Me.m_lblWeight.AutoSize = True
            Me.m_lblWeight.BackColor = System.Drawing.SystemColors.ControlDark
            Me.m_lblWeight.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_lblWeight.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.m_lblWeight.ForeColor = System.Drawing.SystemColors.ControlLightLight
            Me.m_lblWeight.Location = New System.Drawing.Point(0, 0)
            Me.m_lblWeight.Margin = New System.Windows.Forms.Padding(0)
            Me.m_lblWeight.Name = "m_lblWeight"
            Me.m_lblWeight.Size = New System.Drawing.Size(219, 18)
            Me.m_lblWeight.TabIndex = 0
            Me.m_lblWeight.Text = "Weight"
            Me.m_lblWeight.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'm_plGroup
            '
            Me.m_plGroup.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_plGroup.Location = New System.Drawing.Point(453, 21)
            Me.m_plGroup.Name = "m_plGroup"
            Me.m_plGroup.Size = New System.Drawing.Size(324, 342)
            Me.m_plGroup.TabIndex = 5
            '
            'm_plFleet
            '
            Me.m_plFleet.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_plFleet.Location = New System.Drawing.Point(228, 21)
            Me.m_plFleet.Name = "m_plFleet"
            Me.m_plFleet.Size = New System.Drawing.Size(213, 342)
            Me.m_plFleet.TabIndex = 4
            '
            'm_plWeights
            '
            Me.m_plWeights.Location = New System.Drawing.Point(3, 21)
            Me.m_plWeights.Name = "m_plWeights"
            Me.m_plWeights.Size = New System.Drawing.Size(213, 342)
            Me.m_plWeights.TabIndex = 3
            '
            'm_lbFleet
            '
            Me.m_lbFleet.AutoSize = True
            Me.m_lbFleet.BackColor = System.Drawing.SystemColors.ControlDark
            Me.m_lbFleet.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_lbFleet.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.m_lbFleet.ForeColor = System.Drawing.SystemColors.ControlLightLight
            Me.m_lbFleet.Location = New System.Drawing.Point(225, 0)
            Me.m_lbFleet.Margin = New System.Windows.Forms.Padding(0)
            Me.m_lbFleet.Name = "m_lbFleet"
            Me.m_lbFleet.Size = New System.Drawing.Size(219, 18)
            Me.m_lbFleet.TabIndex = 1
            Me.m_lbFleet.Text = "Fleet"
            Me.m_lbFleet.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'm_lblGroup
            '
            Me.m_lblGroup.AutoSize = True
            Me.m_lblGroup.BackColor = System.Drawing.SystemColors.ControlDark
            Me.m_lblGroup.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_lblGroup.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.m_lblGroup.ForeColor = System.Drawing.SystemColors.ControlLightLight
            Me.m_lblGroup.Location = New System.Drawing.Point(450, 0)
            Me.m_lblGroup.Margin = New System.Windows.Forms.Padding(0)
            Me.m_lblGroup.Name = "m_lblGroup"
            Me.m_lblGroup.Size = New System.Drawing.Size(330, 18)
            Me.m_lblGroup.TabIndex = 2
            Me.m_lblGroup.Text = "Group"
            Me.m_lblGroup.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'm_tabMap
            '
            Me.m_tabMap.Controls.Add(Me.m_scMap)
            Me.m_tabMap.Location = New System.Drawing.Point(4, 22)
            Me.m_tabMap.Name = "m_tabMap"
            Me.m_tabMap.Padding = New System.Windows.Forms.Padding(3)
            Me.m_tabMap.Size = New System.Drawing.Size(780, 451)
            Me.m_tabMap.TabIndex = 1
            Me.m_tabMap.Text = "Map"
            Me.m_tabMap.UseVisualStyleBackColor = True
            '
            'Label1
            '
            Me.Label1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.Label1.BackColor = System.Drawing.SystemColors.ControlDark
            Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.Label1.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
            Me.Label1.Location = New System.Drawing.Point(0, 49)
            Me.Label1.Margin = New System.Windows.Forms.Padding(0)
            Me.Label1.Name = "Label1"
            Me.Label1.Size = New System.Drawing.Size(793, 18)
            Me.Label1.TabIndex = 7
            Me.Label1.Text = "Configuration"
            Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'm_tabCredits
            '
            Me.m_tabCredits.Location = New System.Drawing.Point(4, 22)
            Me.m_tabCredits.Name = "m_tabCredits"
            Me.m_tabCredits.Size = New System.Drawing.Size(780, 451)
            Me.m_tabCredits.TabIndex = 2
            Me.m_tabCredits.Text = "Credits"
            Me.m_tabCredits.UseVisualStyleBackColor = True
            '
            'frmMPAOptimizations
            '
            Me.AcceptButton = Me.m_btnRun
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.AutoScroll = True
            Me.CancelButton = Me.m_btnStop
            Me.ClientSize = New System.Drawing.Size(792, 796)
            Me.Controls.Add(Me.m_tcConfiguration)
            Me.Controls.Add(Me.m_tcResults)
            Me.Controls.Add(Me.m_btnRun)
            Me.Controls.Add(Me.m_lblOutput)
            Me.Controls.Add(Me.m_btnStop)
            Me.Controls.Add(Me.m_btnNewSearch)
            Me.Controls.Add(Me.m_rbRandom)
            Me.Controls.Add(Me.Label1)
            Me.Controls.Add(Me.m_lblParameters)
            Me.Controls.Add(Me.m_lblSearchType)
            Me.Controls.Add(Me.m_rbEcoseed)
            Me.CoreExecutionState = EwEUtils.Core.eCoreExecutionState.EcospaceLoaded
            Me.DoubleBuffered = True
            Me.Name = "frmMPAOptimizations"
            Me.Text = "Ecoseed"
            Me.m_tlbLayers.ResumeLayout(False)
            Me.m_tlbLayers.PerformLayout()
            Me.m_tsMap.ResumeLayout(False)
            Me.m_tsMap.PerformLayout()
            CType(Me.m_nudIterations, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.m_nudStep, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.m_nudEndYear, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.m_nudStartYear, System.ComponentModel.ISupportInitialize).EndInit()
            Me.m_tcResults.ResumeLayout(False)
            Me.m_tpProgress.ResumeLayout(False)
            Me.m_tpResults.ResumeLayout(False)
            Me.m_tpResults.PerformLayout()
            CType(Me.m_nudBestPercentile, System.ComponentModel.ISupportInitialize).EndInit()
            Me.m_tlbParameters.ResumeLayout(False)
            Me.m_tlbParameters.PerformLayout()
            CType(Me.m_nudMinArea, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.m_nudMaxArea, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.m_nudBoundaryWeight, System.ComponentModel.ISupportInitialize).EndInit()
            Me.m_scMap.Panel1.ResumeLayout(False)
            Me.m_scMap.Panel2.ResumeLayout(False)
            Me.m_scMap.ResumeLayout(False)
            Me.m_tcConfiguration.ResumeLayout(False)
            Me.m_tabParameters.ResumeLayout(False)
            Me.m_tlpObjectives.ResumeLayout(False)
            Me.m_tlpObjectives.PerformLayout()
            Me.m_tabMap.ResumeLayout(False)
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents m_tlbLayers As System.Windows.Forms.TableLayoutPanel
        Private WithEvents m_lblLayers As System.Windows.Forms.Label
        Private WithEvents m_tsMap As System.Windows.Forms.ToolStrip
        Private WithEvents m_tsbMPA As System.Windows.Forms.ToolStripDropDownButton
        Private WithEvents m_tsmClearMPA As System.Windows.Forms.ToolStripMenuItem
        Private WithEvents m_tsmSetAllMPA As System.Windows.Forms.ToolStripMenuItem
        Private WithEvents m_tsbSeed As System.Windows.Forms.ToolStripDropDownButton
        Private WithEvents m_tsmClearSeed As System.Windows.Forms.ToolStripMenuItem
        Private WithEvents m_tsmSetAllSeed As System.Windows.Forms.ToolStripMenuItem
        Private WithEvents m_nudStartYear As System.Windows.Forms.NumericUpDown
        Private WithEvents m_nudEndYear As System.Windows.Forms.NumericUpDown
        Private WithEvents m_tcResults As System.Windows.Forms.TabControl
        Private WithEvents m_tpProgress As System.Windows.Forms.TabPage
        Private WithEvents m_nudStep As System.Windows.Forms.NumericUpDown
        Private WithEvents m_nudIterations As System.Windows.Forms.NumericUpDown
        Private WithEvents m_rbRandom As System.Windows.Forms.RadioButton
        Private WithEvents m_rbEcoseed As System.Windows.Forms.RadioButton
        Private WithEvents m_cmbMPA As System.Windows.Forms.ComboBox
        Private WithEvents m_tsbEditLayers As System.Windows.Forms.ToolStripButton
        Private WithEvents m_gridProgress As ScientificInterface.gridMPAOptimizations
        Private WithEvents m_tpResults As System.Windows.Forms.TabPage
        Private WithEvents m_btnNewSearch As System.Windows.Forms.Button
        Private WithEvents m_nudBestPercentile As System.Windows.Forms.NumericUpDown
        Private WithEvents m_btnConvertToMpa As System.Windows.Forms.Button
        Private WithEvents m_btnResetMPAs As System.Windows.Forms.Button
        Private WithEvents m_lblBestPercentile As System.Windows.Forms.Label
        Private WithEvents m_graphResults As ZedGraph.ZedGraphControl
        Private WithEvents m_gridResults As ScientificInterface.gridMPAOptimizations
        Private WithEvents m_lblSearchType As System.Windows.Forms.Label
        Private WithEvents m_lblParameters As System.Windows.Forms.Label
        Private WithEvents m_lblEndYear As System.Windows.Forms.Label
        Private WithEvents m_lblStartYear As System.Windows.Forms.Label
        Private WithEvents m_tlbParameters As System.Windows.Forms.TableLayoutPanel
        Private WithEvents m_lbMPA As System.Windows.Forms.Label
        Private WithEvents m_scMap As System.Windows.Forms.SplitContainer
        Private WithEvents m_ucZoom As ScientificInterface.Ecospace.ucZoomBaseMap
        Private WithEvents m_plLayers As System.Windows.Forms.Panel
        Private WithEvents m_btnRun As System.Windows.Forms.Button
        Private WithEvents m_btnStop As System.Windows.Forms.Button
        Private WithEvents m_graphProgress As ZedGraph.ZedGraphControl
        Private WithEvents m_lblOutput As System.Windows.Forms.Label
        Private WithEvents m_tabParameters As System.Windows.Forms.TabPage
        Private WithEvents m_tabMap As System.Windows.Forms.TabPage
        Private WithEvents Label1 As System.Windows.Forms.Label
        Private WithEvents m_tcConfiguration As System.Windows.Forms.TabControl
        Private WithEvents m_plGroup As System.Windows.Forms.Panel
        Private WithEvents m_lblMinArea As System.Windows.Forms.Label
        Private WithEvents m_lblMaxArea As System.Windows.Forms.Label
        Private WithEvents m_nudMinArea As System.Windows.Forms.NumericUpDown
        Private WithEvents m_nudMaxArea As System.Windows.Forms.NumericUpDown
        Private WithEvents m_lblStep As System.Windows.Forms.Label
        Private WithEvents m_lblIterations As System.Windows.Forms.Label
        Private WithEvents m_lblBoundaryWeight As System.Windows.Forms.Label
        Private WithEvents m_nudBoundaryWeight As System.Windows.Forms.NumericUpDown
        Private WithEvents m_tlpObjectives As System.Windows.Forms.TableLayoutPanel
        Private WithEvents m_lblWeight As System.Windows.Forms.Label
        Private WithEvents m_lbFleet As System.Windows.Forms.Label
        Private WithEvents m_lblGroup As System.Windows.Forms.Label
        Private WithEvents m_plFleet As System.Windows.Forms.Panel
        Private WithEvents m_plWeights As System.Windows.Forms.Panel
        Friend WithEvents m_tabCredits As System.Windows.Forms.TabPage
    End Class
End Namespace
