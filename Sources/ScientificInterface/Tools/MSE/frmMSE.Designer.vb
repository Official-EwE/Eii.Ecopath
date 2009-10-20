Imports WeifenLuo.WinFormsUI.Docking

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMSE
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
        Me.btRun = New System.Windows.Forms.Button
        Me.prgProgress = New System.Windows.Forms.ProgressBar
        Me.txNTrials = New System.Windows.Forms.TextBox
        Me.Label1 = New System.Windows.Forms.Label
        Me.lbRun = New System.Windows.Forms.Label
        Me.spInputOutput = New System.Windows.Forms.SplitContainer
        Me.Label2 = New System.Windows.Forms.Label
        Me.tbObjectives = New System.Windows.Forms.TabControl
        Me.pgObjective = New System.Windows.Forms.TabPage
        Me.pgEcoObjectives = New System.Windows.Forms.TabPage
        Me.pgFleetWeight = New System.Windows.Forms.TabPage
        Me.PanelFleetWeight = New ScientificInterface.gridFishingWeights
        Me.pgCatchabiltiy = New System.Windows.Forms.TabPage
        Me.GridCatchabilityIncrease1 = New ScientificInterface.gridCatchabilityIncrease
        Me.pgRiskBounds = New System.Windows.Forms.TabPage
        Me.GridRiskBounds1 = New ScientificInterface.gridRiskBounds
        Me.pgCV = New System.Windows.Forms.TabPage
        Me.panelCV = New ScientificInterface.gridBioCV
        Me.Label3 = New System.Windows.Forms.Label
        Me.tbOutput = New System.Windows.Forms.TabControl
        Me.pgGraphs = New System.Windows.Forms.TabPage
        Me.zdGraph = New ZedGraph.ZedGraphControl
        Me.pgRisk = New System.Windows.Forms.TabPage
        Me.grdRiskResults = New ScientificInterface.gridRiskResults
        Me.ckPlugin = New System.Windows.Forms.CheckBox
        Me.rbCatchEstBio = New System.Windows.Forms.RadioButton
        Me.rbDirectExp = New System.Windows.Forms.RadioButton
        Me.txKalman = New System.Windows.Forms.TextBox
        Me.txForecast = New System.Windows.Forms.TextBox
        Me.Label4 = New System.Windows.Forms.Label
        Me.Label5 = New System.Windows.Forms.Label
        Me.Label6 = New System.Windows.Forms.Label
        Me.txSBPower = New System.Windows.Forms.TextBox
        Me.Label7 = New System.Windows.Forms.Label
        Me.pgPerformance = New System.Windows.Forms.TabPage
        Me.gridPerformanceResults = New ScientificInterface.gridPerformanceResults
        Me.spInputOutput.Panel1.SuspendLayout()
        Me.spInputOutput.Panel2.SuspendLayout()
        Me.spInputOutput.SuspendLayout()
        Me.tbObjectives.SuspendLayout()
        Me.pgFleetWeight.SuspendLayout()
        Me.pgCatchabiltiy.SuspendLayout()
        Me.pgRiskBounds.SuspendLayout()
        Me.pgCV.SuspendLayout()
        Me.tbOutput.SuspendLayout()
        Me.pgGraphs.SuspendLayout()
        Me.pgRisk.SuspendLayout()
        Me.pgPerformance.SuspendLayout()
        Me.SuspendLayout()
        '
        'btRun
        '
        Me.btRun.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btRun.Location = New System.Drawing.Point(4, 51)
        Me.btRun.Margin = New System.Windows.Forms.Padding(0)
        Me.btRun.Name = "btRun"
        Me.btRun.Size = New System.Drawing.Size(153, 23)
        Me.btRun.TabIndex = 0
        Me.btRun.Text = "&Run"
        Me.btRun.UseVisualStyleBackColor = True
        '
        'prgProgress
        '
        Me.prgProgress.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.prgProgress.Location = New System.Drawing.Point(4, 82)
        Me.prgProgress.Name = "prgProgress"
        Me.prgProgress.Size = New System.Drawing.Size(931, 15)
        Me.prgProgress.TabIndex = 1
        '
        'txNTrials
        '
        Me.txNTrials.Location = New System.Drawing.Point(94, 26)
        Me.txNTrials.Name = "txNTrials"
        Me.txNTrials.Size = New System.Drawing.Size(63, 20)
        Me.txNTrials.TabIndex = 2
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(5, 29)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(83, 13)
        Me.Label1.TabIndex = 3
        Me.Label1.Text = "Number of trials:"
        '
        'lbRun
        '
        Me.lbRun.BackColor = System.Drawing.SystemColors.ButtonShadow
        Me.lbRun.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lbRun.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lbRun.Location = New System.Drawing.Point(4, 0)
        Me.lbRun.Name = "lbRun"
        Me.lbRun.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.lbRun.Size = New System.Drawing.Size(153, 21)
        Me.lbRun.TabIndex = 4
        Me.lbRun.Text = "Run"
        Me.lbRun.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'spInputOutput
        '
        Me.spInputOutput.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.spInputOutput.Location = New System.Drawing.Point(4, 103)
        Me.spInputOutput.Name = "spInputOutput"
        '
        'spInputOutput.Panel1
        '
        Me.spInputOutput.Panel1.Controls.Add(Me.Label2)
        Me.spInputOutput.Panel1.Controls.Add(Me.tbObjectives)
        '
        'spInputOutput.Panel2
        '
        Me.spInputOutput.Panel2.Controls.Add(Me.Label3)
        Me.spInputOutput.Panel2.Controls.Add(Me.tbOutput)
        Me.spInputOutput.Size = New System.Drawing.Size(931, 547)
        Me.spInputOutput.SplitterDistance = 480
        Me.spInputOutput.TabIndex = 7
        '
        'Label2
        '
        Me.Label2.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label2.BackColor = System.Drawing.SystemColors.ButtonShadow
        Me.Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.Label2.Location = New System.Drawing.Point(0, 0)
        Me.Label2.Name = "Label2"
        Me.Label2.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Label2.Size = New System.Drawing.Size(482, 21)
        Me.Label2.TabIndex = 9
        Me.Label2.Text = "Inputs"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'tbObjectives
        '
        Me.tbObjectives.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.tbObjectives.Controls.Add(Me.pgObjective)
        Me.tbObjectives.Controls.Add(Me.pgEcoObjectives)
        Me.tbObjectives.Controls.Add(Me.pgFleetWeight)
        Me.tbObjectives.Controls.Add(Me.pgCatchabiltiy)
        Me.tbObjectives.Controls.Add(Me.pgRiskBounds)
        Me.tbObjectives.Controls.Add(Me.pgCV)
        Me.tbObjectives.Location = New System.Drawing.Point(0, 24)
        Me.tbObjectives.MinimumSize = New System.Drawing.Size(10, 0)
        Me.tbObjectives.Name = "tbObjectives"
        Me.tbObjectives.SelectedIndex = 0
        Me.tbObjectives.Size = New System.Drawing.Size(477, 523)
        Me.tbObjectives.TabIndex = 7
        '
        'pgObjective
        '
        Me.pgObjective.Location = New System.Drawing.Point(4, 22)
        Me.pgObjective.Name = "pgObjective"
        Me.pgObjective.Size = New System.Drawing.Size(469, 497)
        Me.pgObjective.TabIndex = 0
        Me.pgObjective.Text = "Objectives"
        Me.pgObjective.UseVisualStyleBackColor = True
        '
        'pgEcoObjectives
        '
        Me.pgEcoObjectives.Location = New System.Drawing.Point(4, 22)
        Me.pgEcoObjectives.Name = "pgEcoObjectives"
        Me.pgEcoObjectives.Size = New System.Drawing.Size(469, 497)
        Me.pgEcoObjectives.TabIndex = 1
        Me.pgEcoObjectives.Text = "Eco Objectives"
        Me.pgEcoObjectives.UseVisualStyleBackColor = True
        '
        'pgFleetWeight
        '
        Me.pgFleetWeight.Controls.Add(Me.PanelFleetWeight)
        Me.pgFleetWeight.Location = New System.Drawing.Point(4, 22)
        Me.pgFleetWeight.Name = "pgFleetWeight"
        Me.pgFleetWeight.Size = New System.Drawing.Size(469, 497)
        Me.pgFleetWeight.TabIndex = 3
        Me.pgFleetWeight.Text = "Fleet Weight"
        Me.pgFleetWeight.UseVisualStyleBackColor = True
        '
        'PanelFleetWeight
        '
        Me.PanelFleetWeight.AutoSizeMinHeight = 10
        Me.PanelFleetWeight.AutoSizeMinWidth = 10
        Me.PanelFleetWeight.AutoStretchColumnsToFitWidth = False
        Me.PanelFleetWeight.AutoStretchRowsToFitHeight = False
        Me.PanelFleetWeight.BackColor = System.Drawing.Color.White
        Me.PanelFleetWeight.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.PanelFleetWeight.ContextMenuStyle = CType((((SourceGrid2.ContextMenuStyle.ColumnResize Or SourceGrid2.ContextMenuStyle.AutoSize) _
                    Or SourceGrid2.ContextMenuStyle.CopyPasteSelection) _
                    Or SourceGrid2.ContextMenuStyle.CellContextMenu), SourceGrid2.ContextMenuStyle)
        Me.PanelFleetWeight.CustomSort = False
        Me.PanelFleetWeight.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PanelFleetWeight.FixedColumnWidths = False
        Me.PanelFleetWeight.FocusStyle = SourceGrid2.FocusStyle.None
        Me.PanelFleetWeight.GridToolTipActive = True
        Me.PanelFleetWeight.Location = New System.Drawing.Point(0, 0)
        Me.PanelFleetWeight.Name = "PanelFleetWeight"
        Me.PanelFleetWeight.Size = New System.Drawing.Size(469, 497)
        Me.PanelFleetWeight.SpecialKeys = CType((((((((((SourceGrid2.GridSpecialKeys.Ctrl_C Or SourceGrid2.GridSpecialKeys.Ctrl_V) _
                    Or SourceGrid2.GridSpecialKeys.Ctrl_X) _
                    Or SourceGrid2.GridSpecialKeys.Delete) _
                    Or SourceGrid2.GridSpecialKeys.Arrows) _
                    Or SourceGrid2.GridSpecialKeys.Tab) _
                    Or SourceGrid2.GridSpecialKeys.PageDownUp) _
                    Or SourceGrid2.GridSpecialKeys.Enter) _
                    Or SourceGrid2.GridSpecialKeys.Escape) _
                    Or SourceGrid2.GridSpecialKeys.Backspace), SourceGrid2.GridSpecialKeys)
        Me.PanelFleetWeight.TabIndex = 0
        '
        'pgCatchabiltiy
        '
        Me.pgCatchabiltiy.Controls.Add(Me.GridCatchabilityIncrease1)
        Me.pgCatchabiltiy.Location = New System.Drawing.Point(4, 22)
        Me.pgCatchabiltiy.Name = "pgCatchabiltiy"
        Me.pgCatchabiltiy.Size = New System.Drawing.Size(469, 497)
        Me.pgCatchabiltiy.TabIndex = 4
        Me.pgCatchabiltiy.Text = "Catchability Increase"
        Me.pgCatchabiltiy.UseVisualStyleBackColor = True
        '
        'GridCatchabilityIncrease1
        '
        Me.GridCatchabilityIncrease1.AutoSizeMinHeight = 10
        Me.GridCatchabilityIncrease1.AutoSizeMinWidth = 10
        Me.GridCatchabilityIncrease1.AutoStretchColumnsToFitWidth = False
        Me.GridCatchabilityIncrease1.AutoStretchRowsToFitHeight = False
        Me.GridCatchabilityIncrease1.BackColor = System.Drawing.Color.White
        Me.GridCatchabilityIncrease1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.GridCatchabilityIncrease1.ContextMenuStyle = CType((((SourceGrid2.ContextMenuStyle.ColumnResize Or SourceGrid2.ContextMenuStyle.AutoSize) _
                    Or SourceGrid2.ContextMenuStyle.CopyPasteSelection) _
                    Or SourceGrid2.ContextMenuStyle.CellContextMenu), SourceGrid2.ContextMenuStyle)
        Me.GridCatchabilityIncrease1.CustomSort = False
        Me.GridCatchabilityIncrease1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GridCatchabilityIncrease1.FixedColumnWidths = False
        Me.GridCatchabilityIncrease1.FocusStyle = SourceGrid2.FocusStyle.None
        Me.GridCatchabilityIncrease1.GridToolTipActive = True
        Me.GridCatchabilityIncrease1.Location = New System.Drawing.Point(0, 0)
        Me.GridCatchabilityIncrease1.Name = "GridCatchabilityIncrease1"
        Me.GridCatchabilityIncrease1.Size = New System.Drawing.Size(469, 497)
        Me.GridCatchabilityIncrease1.SpecialKeys = CType((((((((((SourceGrid2.GridSpecialKeys.Ctrl_C Or SourceGrid2.GridSpecialKeys.Ctrl_V) _
                    Or SourceGrid2.GridSpecialKeys.Ctrl_X) _
                    Or SourceGrid2.GridSpecialKeys.Delete) _
                    Or SourceGrid2.GridSpecialKeys.Arrows) _
                    Or SourceGrid2.GridSpecialKeys.Tab) _
                    Or SourceGrid2.GridSpecialKeys.PageDownUp) _
                    Or SourceGrid2.GridSpecialKeys.Enter) _
                    Or SourceGrid2.GridSpecialKeys.Escape) _
                    Or SourceGrid2.GridSpecialKeys.Backspace), SourceGrid2.GridSpecialKeys)
        Me.GridCatchabilityIncrease1.TabIndex = 0
        '
        'pgRiskBounds
        '
        Me.pgRiskBounds.Controls.Add(Me.GridRiskBounds1)
        Me.pgRiskBounds.Location = New System.Drawing.Point(4, 22)
        Me.pgRiskBounds.Name = "pgRiskBounds"
        Me.pgRiskBounds.Size = New System.Drawing.Size(469, 497)
        Me.pgRiskBounds.TabIndex = 5
        Me.pgRiskBounds.Text = "Risk Bounds"
        Me.pgRiskBounds.UseVisualStyleBackColor = True
        '
        'GridRiskBounds1
        '
        Me.GridRiskBounds1.AutoSizeMinHeight = 10
        Me.GridRiskBounds1.AutoSizeMinWidth = 10
        Me.GridRiskBounds1.AutoStretchColumnsToFitWidth = False
        Me.GridRiskBounds1.AutoStretchRowsToFitHeight = False
        Me.GridRiskBounds1.BackColor = System.Drawing.Color.White
        Me.GridRiskBounds1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.GridRiskBounds1.ContextMenuStyle = CType((((SourceGrid2.ContextMenuStyle.ColumnResize Or SourceGrid2.ContextMenuStyle.AutoSize) _
                    Or SourceGrid2.ContextMenuStyle.CopyPasteSelection) _
                    Or SourceGrid2.ContextMenuStyle.CellContextMenu), SourceGrid2.ContextMenuStyle)
        Me.GridRiskBounds1.CustomSort = False
        Me.GridRiskBounds1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GridRiskBounds1.FixedColumnWidths = False
        Me.GridRiskBounds1.FocusStyle = SourceGrid2.FocusStyle.None
        Me.GridRiskBounds1.GridToolTipActive = True
        Me.GridRiskBounds1.Location = New System.Drawing.Point(0, 0)
        Me.GridRiskBounds1.Name = "GridRiskBounds1"
        Me.GridRiskBounds1.Size = New System.Drawing.Size(469, 497)
        Me.GridRiskBounds1.SpecialKeys = CType((((((((((SourceGrid2.GridSpecialKeys.Ctrl_C Or SourceGrid2.GridSpecialKeys.Ctrl_V) _
                    Or SourceGrid2.GridSpecialKeys.Ctrl_X) _
                    Or SourceGrid2.GridSpecialKeys.Delete) _
                    Or SourceGrid2.GridSpecialKeys.Arrows) _
                    Or SourceGrid2.GridSpecialKeys.Tab) _
                    Or SourceGrid2.GridSpecialKeys.PageDownUp) _
                    Or SourceGrid2.GridSpecialKeys.Enter) _
                    Or SourceGrid2.GridSpecialKeys.Escape) _
                    Or SourceGrid2.GridSpecialKeys.Backspace), SourceGrid2.GridSpecialKeys)
        Me.GridRiskBounds1.TabIndex = 0
        '
        'pgCV
        '
        Me.pgCV.Controls.Add(Me.panelCV)
        Me.pgCV.Location = New System.Drawing.Point(4, 22)
        Me.pgCV.Name = "pgCV"
        Me.pgCV.Size = New System.Drawing.Size(469, 497)
        Me.pgCV.TabIndex = 2
        Me.pgCV.Text = "C.V. Fishing Rate"
        Me.pgCV.UseVisualStyleBackColor = True
        '
        'panelCV
        '
        Me.panelCV.AutoSizeMinHeight = 10
        Me.panelCV.AutoSizeMinWidth = 10
        Me.panelCV.AutoStretchColumnsToFitWidth = False
        Me.panelCV.AutoStretchRowsToFitHeight = False
        Me.panelCV.BackColor = System.Drawing.Color.White
        Me.panelCV.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.panelCV.ContextMenuStyle = CType((((SourceGrid2.ContextMenuStyle.ColumnResize Or SourceGrid2.ContextMenuStyle.AutoSize) _
                    Or SourceGrid2.ContextMenuStyle.CopyPasteSelection) _
                    Or SourceGrid2.ContextMenuStyle.CellContextMenu), SourceGrid2.ContextMenuStyle)
        Me.panelCV.CustomSort = False
        Me.panelCV.Dock = System.Windows.Forms.DockStyle.Fill
        Me.panelCV.FixedColumnWidths = False
        Me.panelCV.FocusStyle = SourceGrid2.FocusStyle.None
        Me.panelCV.GridToolTipActive = True
        Me.panelCV.Location = New System.Drawing.Point(0, 0)
        Me.panelCV.Name = "panelCV"
        Me.panelCV.Size = New System.Drawing.Size(469, 497)
        Me.panelCV.SpecialKeys = CType((((((((((SourceGrid2.GridSpecialKeys.Ctrl_C Or SourceGrid2.GridSpecialKeys.Ctrl_V) _
                    Or SourceGrid2.GridSpecialKeys.Ctrl_X) _
                    Or SourceGrid2.GridSpecialKeys.Delete) _
                    Or SourceGrid2.GridSpecialKeys.Arrows) _
                    Or SourceGrid2.GridSpecialKeys.Tab) _
                    Or SourceGrid2.GridSpecialKeys.PageDownUp) _
                    Or SourceGrid2.GridSpecialKeys.Enter) _
                    Or SourceGrid2.GridSpecialKeys.Escape) _
                    Or SourceGrid2.GridSpecialKeys.Backspace), SourceGrid2.GridSpecialKeys)
        Me.panelCV.TabIndex = 0
        '
        'Label3
        '
        Me.Label3.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label3.BackColor = System.Drawing.SystemColors.ButtonShadow
        Me.Label3.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.Label3.Location = New System.Drawing.Point(0, 0)
        Me.Label3.Name = "Label3"
        Me.Label3.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Label3.Size = New System.Drawing.Size(447, 21)
        Me.Label3.TabIndex = 9
        Me.Label3.Text = "Outputs"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'tbOutput
        '
        Me.tbOutput.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.tbOutput.Controls.Add(Me.pgGraphs)
        Me.tbOutput.Controls.Add(Me.pgRisk)
        Me.tbOutput.Controls.Add(Me.pgPerformance)
        Me.tbOutput.Location = New System.Drawing.Point(0, 24)
        Me.tbOutput.MinimumSize = New System.Drawing.Size(10, 0)
        Me.tbOutput.Name = "tbOutput"
        Me.tbOutput.SelectedIndex = 0
        Me.tbOutput.Size = New System.Drawing.Size(447, 523)
        Me.tbOutput.TabIndex = 6
        '
        'pgGraphs
        '
        Me.pgGraphs.Controls.Add(Me.zdGraph)
        Me.pgGraphs.Location = New System.Drawing.Point(4, 22)
        Me.pgGraphs.Name = "pgGraphs"
        Me.pgGraphs.Size = New System.Drawing.Size(439, 497)
        Me.pgGraphs.TabIndex = 0
        Me.pgGraphs.Text = "Graphs"
        Me.pgGraphs.UseVisualStyleBackColor = True
        '
        'zdGraph
        '
        Me.zdGraph.Dock = System.Windows.Forms.DockStyle.Fill
        Me.zdGraph.Location = New System.Drawing.Point(0, 0)
        Me.zdGraph.Margin = New System.Windows.Forms.Padding(0)
        Me.zdGraph.Name = "zdGraph"
        Me.zdGraph.ScrollGrace = 0
        Me.zdGraph.ScrollMaxX = 0
        Me.zdGraph.ScrollMaxY = 0
        Me.zdGraph.ScrollMaxY2 = 0
        Me.zdGraph.ScrollMinX = 0
        Me.zdGraph.ScrollMinY = 0
        Me.zdGraph.ScrollMinY2 = 0
        Me.zdGraph.Size = New System.Drawing.Size(439, 497)
        Me.zdGraph.TabIndex = 0
        '
        'pgRisk
        '
        Me.pgRisk.Controls.Add(Me.grdRiskResults)
        Me.pgRisk.Location = New System.Drawing.Point(4, 22)
        Me.pgRisk.Name = "pgRisk"
        Me.pgRisk.Size = New System.Drawing.Size(439, 497)
        Me.pgRisk.TabIndex = 1
        Me.pgRisk.Text = "Risk"
        Me.pgRisk.UseVisualStyleBackColor = True
        '
        'grdRiskResults
        '
        Me.grdRiskResults.AutoSizeMinHeight = 10
        Me.grdRiskResults.AutoSizeMinWidth = 10
        Me.grdRiskResults.AutoStretchColumnsToFitWidth = False
        Me.grdRiskResults.AutoStretchRowsToFitHeight = False
        Me.grdRiskResults.BackColor = System.Drawing.Color.White
        Me.grdRiskResults.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.grdRiskResults.ContextMenuStyle = CType((((SourceGrid2.ContextMenuStyle.ColumnResize Or SourceGrid2.ContextMenuStyle.AutoSize) _
                    Or SourceGrid2.ContextMenuStyle.CopyPasteSelection) _
                    Or SourceGrid2.ContextMenuStyle.CellContextMenu), SourceGrid2.ContextMenuStyle)
        Me.grdRiskResults.CustomSort = False
        Me.grdRiskResults.Dock = System.Windows.Forms.DockStyle.Fill
        Me.grdRiskResults.FixedColumnWidths = False
        Me.grdRiskResults.FocusStyle = SourceGrid2.FocusStyle.None
        Me.grdRiskResults.GridToolTipActive = True
        Me.grdRiskResults.Location = New System.Drawing.Point(0, 0)
        Me.grdRiskResults.Name = "grdRiskResults"
        Me.grdRiskResults.Size = New System.Drawing.Size(439, 497)
        Me.grdRiskResults.SpecialKeys = CType((((((((((SourceGrid2.GridSpecialKeys.Ctrl_C Or SourceGrid2.GridSpecialKeys.Ctrl_V) _
                    Or SourceGrid2.GridSpecialKeys.Ctrl_X) _
                    Or SourceGrid2.GridSpecialKeys.Delete) _
                    Or SourceGrid2.GridSpecialKeys.Arrows) _
                    Or SourceGrid2.GridSpecialKeys.Tab) _
                    Or SourceGrid2.GridSpecialKeys.PageDownUp) _
                    Or SourceGrid2.GridSpecialKeys.Enter) _
                    Or SourceGrid2.GridSpecialKeys.Escape) _
                    Or SourceGrid2.GridSpecialKeys.Backspace), SourceGrid2.GridSpecialKeys)
        Me.grdRiskResults.TabIndex = 0
        '
        'ckPlugin
        '
        Me.ckPlugin.AutoSize = True
        Me.ckPlugin.Location = New System.Drawing.Point(578, 29)
        Me.ckPlugin.Name = "ckPlugin"
        Me.ckPlugin.Size = New System.Drawing.Size(149, 17)
        Me.ckPlugin.TabIndex = 9
        Me.ckPlugin.Text = "Use plugin economic data"
        Me.ckPlugin.UseVisualStyleBackColor = True
        '
        'rbCatchEstBio
        '
        Me.rbCatchEstBio.AutoSize = True
        Me.rbCatchEstBio.Checked = True
        Me.rbCatchEstBio.Location = New System.Drawing.Point(169, 28)
        Me.rbCatchEstBio.Name = "rbCatchEstBio"
        Me.rbCatchEstBio.Size = New System.Drawing.Size(144, 17)
        Me.rbCatchEstBio.TabIndex = 10
        Me.rbCatchEstBio.TabStop = True
        Me.rbCatchEstBio.Text = "Catch/estimated biomass"
        Me.rbCatchEstBio.UseVisualStyleBackColor = True
        '
        'rbDirectExp
        '
        Me.rbDirectExp.AutoSize = True
        Me.rbDirectExp.Location = New System.Drawing.Point(169, 57)
        Me.rbDirectExp.Name = "rbDirectExp"
        Me.rbDirectExp.Size = New System.Drawing.Size(130, 17)
        Me.rbDirectExp.TabIndex = 11
        Me.rbDirectExp.Text = "Direct exploitation rate"
        Me.rbDirectExp.UseVisualStyleBackColor = True
        '
        'txKalman
        '
        Me.txKalman.Location = New System.Drawing.Point(465, 27)
        Me.txKalman.Name = "txKalman"
        Me.txKalman.Size = New System.Drawing.Size(56, 20)
        Me.txKalman.TabIndex = 12
        '
        'txForecast
        '
        Me.txForecast.Location = New System.Drawing.Point(465, 56)
        Me.txForecast.Name = "txForecast"
        Me.txForecast.Size = New System.Drawing.Size(56, 20)
        Me.txForecast.TabIndex = 13
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(356, 30)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(68, 13)
        Me.Label4.TabIndex = 14
        Me.Label4.Text = "Kalman gain:"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(356, 59)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(103, 13)
        Me.Label5.TabIndex = 15
        Me.Label5.Text = "Forecast stock gain:"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(575, 59)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(133, 13)
        Me.Label6.TabIndex = 16
        Me.Label6.Text = "Survey vs. biomass power:"
        '
        'txSBPower
        '
        Me.txSBPower.Location = New System.Drawing.Point(714, 56)
        Me.txSBPower.Name = "txSBPower"
        Me.txSBPower.Size = New System.Drawing.Size(48, 20)
        Me.txSBPower.TabIndex = 17
        '
        'Label7
        '
        Me.Label7.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label7.BackColor = System.Drawing.SystemColors.ButtonShadow
        Me.Label7.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label7.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.Label7.Location = New System.Drawing.Point(166, 0)
        Me.Label7.Name = "Label7"
        Me.Label7.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Label7.Size = New System.Drawing.Size(769, 21)
        Me.Label7.TabIndex = 18
        Me.Label7.Text = "Model parameters"
        Me.Label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'pgPerformance
        '
        Me.pgPerformance.Controls.Add(Me.gridPerformanceResults)
        Me.pgPerformance.Location = New System.Drawing.Point(4, 22)
        Me.pgPerformance.Name = "pgPerformance"
        Me.pgPerformance.Size = New System.Drawing.Size(439, 497)
        Me.pgPerformance.TabIndex = 2
        Me.pgPerformance.Text = "Performance"
        Me.pgPerformance.UseVisualStyleBackColor = True
        '
        'gridPerformanceResults
        '
        Me.gridPerformanceResults.AutoSizeMinHeight = 10
        Me.gridPerformanceResults.AutoSizeMinWidth = 10
        Me.gridPerformanceResults.AutoStretchColumnsToFitWidth = False
        Me.gridPerformanceResults.AutoStretchRowsToFitHeight = False
        Me.gridPerformanceResults.BackColor = System.Drawing.Color.White
        Me.gridPerformanceResults.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.gridPerformanceResults.ContextMenuStyle = CType((((SourceGrid2.ContextMenuStyle.ColumnResize Or SourceGrid2.ContextMenuStyle.AutoSize) _
                    Or SourceGrid2.ContextMenuStyle.CopyPasteSelection) _
                    Or SourceGrid2.ContextMenuStyle.CellContextMenu), SourceGrid2.ContextMenuStyle)
        Me.gridPerformanceResults.CustomSort = False
        Me.gridPerformanceResults.Dock = System.Windows.Forms.DockStyle.Fill
        Me.gridPerformanceResults.FixedColumnWidths = False
        Me.gridPerformanceResults.FocusStyle = SourceGrid2.FocusStyle.None
        Me.gridPerformanceResults.GridToolTipActive = True
        Me.gridPerformanceResults.Location = New System.Drawing.Point(0, 0)
        Me.gridPerformanceResults.Name = "gridPerformanceResults"
        Me.gridPerformanceResults.Size = New System.Drawing.Size(439, 497)
        Me.gridPerformanceResults.SpecialKeys = CType((((((((((SourceGrid2.GridSpecialKeys.Ctrl_C Or SourceGrid2.GridSpecialKeys.Ctrl_V) _
                    Or SourceGrid2.GridSpecialKeys.Ctrl_X) _
                    Or SourceGrid2.GridSpecialKeys.Delete) _
                    Or SourceGrid2.GridSpecialKeys.Arrows) _
                    Or SourceGrid2.GridSpecialKeys.Tab) _
                    Or SourceGrid2.GridSpecialKeys.PageDownUp) _
                    Or SourceGrid2.GridSpecialKeys.Enter) _
                    Or SourceGrid2.GridSpecialKeys.Escape) _
                    Or SourceGrid2.GridSpecialKeys.Backspace), SourceGrid2.GridSpecialKeys)
        Me.gridPerformanceResults.TabIndex = 1
        '
        'frmMSE
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(938, 653)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.txSBPower)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.txForecast)
        Me.Controls.Add(Me.txKalman)
        Me.Controls.Add(Me.rbDirectExp)
        Me.Controls.Add(Me.rbCatchEstBio)
        Me.Controls.Add(Me.ckPlugin)
        Me.Controls.Add(Me.spInputOutput)
        Me.Controls.Add(Me.lbRun)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.txNTrials)
        Me.Controls.Add(Me.prgProgress)
        Me.Controls.Add(Me.btRun)
        Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Name = "frmMSE"
        Me.Text = "frmMSE"
        Me.spInputOutput.Panel1.ResumeLayout(False)
        Me.spInputOutput.Panel2.ResumeLayout(False)
        Me.spInputOutput.ResumeLayout(False)
        Me.tbObjectives.ResumeLayout(False)
        Me.pgFleetWeight.ResumeLayout(False)
        Me.pgCatchabiltiy.ResumeLayout(False)
        Me.pgRiskBounds.ResumeLayout(False)
        Me.pgCV.ResumeLayout(False)
        Me.tbOutput.ResumeLayout(False)
        Me.pgGraphs.ResumeLayout(False)
        Me.pgRisk.ResumeLayout(False)
        Me.pgPerformance.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents lbRun As System.Windows.Forms.Label
    Friend WithEvents spInputOutput As System.Windows.Forms.SplitContainer
    Friend WithEvents pgEcoObjectives As System.Windows.Forms.TabPage
    Friend WithEvents pgGraphs As System.Windows.Forms.TabPage
    Friend WithEvents pgRisk As System.Windows.Forms.TabPage
    Friend WithEvents pgCV As System.Windows.Forms.TabPage
    Friend WithEvents pgFleetWeight As System.Windows.Forms.TabPage
    Friend WithEvents pgCatchabiltiy As System.Windows.Forms.TabPage
    Friend WithEvents pgRiskBounds As System.Windows.Forms.TabPage
    Friend WithEvents panelCV As gridBioCV
    Friend WithEvents PanelFleetWeight As gridFishingWeights
    Friend WithEvents GridRiskBounds1 As ScientificInterface.gridRiskBounds
    Friend WithEvents GridCatchabilityIncrease1 As ScientificInterface.gridCatchabilityIncrease
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Private WithEvents btRun As System.Windows.Forms.Button
    Private WithEvents prgProgress As System.Windows.Forms.ProgressBar
    Private WithEvents txNTrials As System.Windows.Forms.TextBox
    Private WithEvents Label1 As System.Windows.Forms.Label
    Private WithEvents tbObjectives As System.Windows.Forms.TabControl
    Private WithEvents pgObjective As System.Windows.Forms.TabPage
    Private WithEvents tbOutput As System.Windows.Forms.TabControl
    Private WithEvents zdGraph As ZedGraph.ZedGraphControl
    Private WithEvents ckPlugin As System.Windows.Forms.CheckBox
    Private WithEvents grdRiskResults As ScientificInterface.gridRiskResults
    Private WithEvents Label3 As System.Windows.Forms.Label
    Private WithEvents Label2 As System.Windows.Forms.Label
    Private WithEvents rbCatchEstBio As System.Windows.Forms.RadioButton
    Private WithEvents rbDirectExp As System.Windows.Forms.RadioButton
    Private WithEvents txKalman As System.Windows.Forms.TextBox
    Private WithEvents txForecast As System.Windows.Forms.TextBox
    Private WithEvents Label4 As System.Windows.Forms.Label
    Private WithEvents Label5 As System.Windows.Forms.Label
    Private WithEvents Label6 As System.Windows.Forms.Label
    Private WithEvents txSBPower As System.Windows.Forms.TextBox
    Private WithEvents pgPerformance As System.Windows.Forms.TabPage
    Friend WithEvents gridPerformanceResults As ScientificInterface.gridPerformanceResults
End Class
