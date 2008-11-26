Imports WeifenLuo.WinFormsUI.Docking

Namespace Ecosim
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class frmFishingPolicySearch
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
            Me.SplitContainer1 = New System.Windows.Forms.SplitContainer
            Me.btnSearch = New System.Windows.Forms.Button
            Me.btnStop = New System.Windows.Forms.Button
            Me.plRunParams = New System.Windows.Forms.Panel
            Me.Label2 = New System.Windows.Forms.Label
            Me.txGenDiscRate = New System.Windows.Forms.TextBox
            Me.Label1 = New System.Windows.Forms.Label
            Me.txDiscountRate = New System.Windows.Forms.TextBox
            Me.plMaxSO = New System.Windows.Forms.Panel
            Me.cbPrevCE = New System.Windows.Forms.CheckBox
            Me.cbMaxPortUl = New System.Windows.Forms.CheckBox
            Me.cbIncludeCCosts = New System.Windows.Forms.CheckBox
            Me.nupMaxEffChg = New System.Windows.Forms.NumericUpDown
            Me.nudBaseYear = New System.Windows.Forms.NumericUpDown
            Me.lblBaseYear = New System.Windows.Forms.Label
            Me.cmbOptmApproach = New System.Windows.Forms.ComboBox
            Me.lblMaxEffChg = New System.Windows.Forms.Label
            Me.lblOptmApproach = New System.Windows.Forms.Label
            Me.cmbSearchUsing = New System.Windows.Forms.ComboBox
            Me.lblSearchUsing = New System.Windows.Forms.Label
            Me.cmbInitUsing = New System.Windows.Forms.ComboBox
            Me.lblInitUsing = New System.Windows.Forms.Label
            Me.nupMaxNumEval = New System.Windows.Forms.NumericUpDown
            Me.nupNumOfRuns = New System.Windows.Forms.NumericUpDown
            Me.lblMaxNumEval = New System.Windows.Forms.Label
            Me.lblNumOfRuns = New System.Windows.Forms.Label
            Me.lblInitializationHeader = New System.Windows.Forms.Label
            Me.plBlocks = New System.Windows.Forms.Panel
            Me.tcMain = New System.Windows.Forms.TabControl
            Me.tbpObjv = New System.Windows.Forms.TabPage
            Me.SplitContainer2 = New System.Windows.Forms.SplitContainer
            Me.SplitContainer3 = New System.Windows.Forms.SplitContainer
            Me.tbpResultTable = New System.Windows.Forms.TabPage
            Me.scIterResult = New System.Windows.Forms.SplitContainer
            Me.scIterResultMultiRun = New System.Windows.Forms.SplitContainer
            Me.m_tpPlots = New System.Windows.Forms.TabPage
            Me.m_splcPlotResults = New System.Windows.Forms.SplitContainer
            Me.m_graphResults = New ZedGraph.ZedGraphControl
            Me.m_zgcKiteDiag = New ZedGraph.ZedGraphControl
            Me.SplitContainer1.Panel1.SuspendLayout()
            Me.SplitContainer1.Panel2.SuspendLayout()
            Me.SplitContainer1.SuspendLayout()
            Me.plRunParams.SuspendLayout()
            Me.plMaxSO.SuspendLayout()
            CType(Me.nupMaxEffChg, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.nudBaseYear, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.nupMaxNumEval, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.nupNumOfRuns, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.tcMain.SuspendLayout()
            Me.tbpObjv.SuspendLayout()
            Me.SplitContainer2.Panel2.SuspendLayout()
            Me.SplitContainer2.SuspendLayout()
            Me.SplitContainer3.SuspendLayout()
            Me.tbpResultTable.SuspendLayout()
            Me.scIterResult.Panel1.SuspendLayout()
            Me.scIterResult.SuspendLayout()
            Me.scIterResultMultiRun.SuspendLayout()
            Me.m_tpPlots.SuspendLayout()
            Me.m_splcPlotResults.Panel1.SuspendLayout()
            Me.m_splcPlotResults.Panel2.SuspendLayout()
            Me.m_splcPlotResults.SuspendLayout()
            Me.SuspendLayout()
            '
            'SplitContainer1
            '
            Me.SplitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
            Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
            Me.SplitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1
            Me.SplitContainer1.Location = New System.Drawing.Point(0, 0)
            Me.SplitContainer1.Margin = New System.Windows.Forms.Padding(0)
            Me.SplitContainer1.Name = "SplitContainer1"
            Me.SplitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal
            '
            'SplitContainer1.Panel1
            '
            Me.SplitContainer1.Panel1.Controls.Add(Me.btnSearch)
            Me.SplitContainer1.Panel1.Controls.Add(Me.btnStop)
            Me.SplitContainer1.Panel1.Controls.Add(Me.plRunParams)
            Me.SplitContainer1.Panel1.Controls.Add(Me.plBlocks)
            Me.SplitContainer1.Panel1MinSize = 338
            '
            'SplitContainer1.Panel2
            '
            Me.SplitContainer1.Panel2.Controls.Add(Me.tcMain)
            Me.SplitContainer1.Size = New System.Drawing.Size(897, 621)
            Me.SplitContainer1.SplitterDistance = 338
            Me.SplitContainer1.TabIndex = 0
            '
            'btnSearch
            '
            Me.btnSearch.Location = New System.Drawing.Point(48, 307)
            Me.btnSearch.Name = "btnSearch"
            Me.btnSearch.Size = New System.Drawing.Size(75, 23)
            Me.btnSearch.TabIndex = 22
            Me.btnSearch.Text = "&Search"
            Me.btnSearch.UseVisualStyleBackColor = True
            '
            'btnStop
            '
            Me.btnStop.Location = New System.Drawing.Point(129, 307)
            Me.btnStop.Name = "btnStop"
            Me.btnStop.Size = New System.Drawing.Size(75, 23)
            Me.btnStop.TabIndex = 23
            Me.btnStop.Text = "Sto&p"
            Me.btnStop.UseVisualStyleBackColor = True
            '
            'plRunParams
            '
            Me.plRunParams.Controls.Add(Me.Label2)
            Me.plRunParams.Controls.Add(Me.txGenDiscRate)
            Me.plRunParams.Controls.Add(Me.Label1)
            Me.plRunParams.Controls.Add(Me.txDiscountRate)
            Me.plRunParams.Controls.Add(Me.plMaxSO)
            Me.plRunParams.Controls.Add(Me.nupMaxEffChg)
            Me.plRunParams.Controls.Add(Me.nudBaseYear)
            Me.plRunParams.Controls.Add(Me.lblBaseYear)
            Me.plRunParams.Controls.Add(Me.cmbOptmApproach)
            Me.plRunParams.Controls.Add(Me.lblMaxEffChg)
            Me.plRunParams.Controls.Add(Me.lblOptmApproach)
            Me.plRunParams.Controls.Add(Me.cmbSearchUsing)
            Me.plRunParams.Controls.Add(Me.lblSearchUsing)
            Me.plRunParams.Controls.Add(Me.cmbInitUsing)
            Me.plRunParams.Controls.Add(Me.lblInitUsing)
            Me.plRunParams.Controls.Add(Me.nupMaxNumEval)
            Me.plRunParams.Controls.Add(Me.nupNumOfRuns)
            Me.plRunParams.Controls.Add(Me.lblMaxNumEval)
            Me.plRunParams.Controls.Add(Me.lblNumOfRuns)
            Me.plRunParams.Controls.Add(Me.lblInitializationHeader)
            Me.plRunParams.Location = New System.Drawing.Point(-1, -1)
            Me.plRunParams.Margin = New System.Windows.Forms.Padding(0)
            Me.plRunParams.Name = "plRunParams"
            Me.plRunParams.Size = New System.Drawing.Size(262, 305)
            Me.plRunParams.TabIndex = 0
            '
            'Label2
            '
            Me.Label2.AutoSize = True
            Me.Label2.Location = New System.Drawing.Point(5, 48)
            Me.Label2.Name = "Label2"
            Me.Label2.Size = New System.Drawing.Size(79, 13)
            Me.Label2.TabIndex = 3
            Me.Label2.Text = "Gen. disc. rate:"
            '
            'txGenDiscRate
            '
            Me.txGenDiscRate.Location = New System.Drawing.Point(116, 45)
            Me.txGenDiscRate.Name = "txGenDiscRate"
            Me.txGenDiscRate.Size = New System.Drawing.Size(60, 20)
            Me.txGenDiscRate.TabIndex = 4
            '
            'Label1
            '
            Me.Label1.AutoSize = True
            Me.Label1.Location = New System.Drawing.Point(5, 22)
            Me.Label1.Name = "Label1"
            Me.Label1.Size = New System.Drawing.Size(73, 13)
            Me.Label1.TabIndex = 1
            Me.Label1.Text = "Discount rate:"
            '
            'txDiscountRate
            '
            Me.txDiscountRate.Location = New System.Drawing.Point(116, 19)
            Me.txDiscountRate.Name = "txDiscountRate"
            Me.txDiscountRate.Size = New System.Drawing.Size(60, 20)
            Me.txDiscountRate.TabIndex = 2
            '
            'plMaxSO
            '
            Me.plMaxSO.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.plMaxSO.Controls.Add(Me.cbPrevCE)
            Me.plMaxSO.Controls.Add(Me.cbMaxPortUl)
            Me.plMaxSO.Controls.Add(Me.cbIncludeCCosts)
            Me.plMaxSO.Location = New System.Drawing.Point(7, 252)
            Me.plMaxSO.Margin = New System.Windows.Forms.Padding(0)
            Me.plMaxSO.Name = "plMaxSO"
            Me.plMaxSO.Size = New System.Drawing.Size(255, 42)
            Me.plMaxSO.TabIndex = 19
            '
            'cbPrevCE
            '
            Me.cbPrevCE.AutoSize = True
            Me.cbPrevCE.Location = New System.Drawing.Point(0, 1)
            Me.cbPrevCE.Name = "cbPrevCE"
            Me.cbPrevCE.Size = New System.Drawing.Size(138, 17)
            Me.cbPrevCE.TabIndex = 0
            Me.cbPrevCE.Text = "Prevent cost > earnings"
            Me.cbPrevCE.UseVisualStyleBackColor = True
            '
            'cbMaxPortUl
            '
            Me.cbMaxPortUl.AutoSize = True
            Me.cbMaxPortUl.Location = New System.Drawing.Point(0, 24)
            Me.cbMaxPortUl.Name = "cbMaxPortUl"
            Me.cbMaxPortUl.Size = New System.Drawing.Size(135, 17)
            Me.cbMaxPortUl.TabIndex = 1
            Me.cbMaxPortUl.Text = "Maximize portfolio utility"
            Me.cbMaxPortUl.UseVisualStyleBackColor = True
            '
            'cbIncludeCCosts
            '
            Me.cbIncludeCCosts.AutoSize = True
            Me.cbIncludeCCosts.Location = New System.Drawing.Point(6, 3)
            Me.cbIncludeCCosts.Name = "cbIncludeCCosts"
            Me.cbIncludeCCosts.Size = New System.Drawing.Size(133, 17)
            Me.cbIncludeCCosts.TabIndex = 0
            Me.cbIncludeCCosts.Text = "Include compete costs"
            Me.cbIncludeCCosts.UseVisualStyleBackColor = True
            '
            'nupMaxEffChg
            '
            Me.nupMaxEffChg.Location = New System.Drawing.Point(116, 150)
            Me.nupMaxEffChg.Name = "nupMaxEffChg"
            Me.nupMaxEffChg.Size = New System.Drawing.Size(60, 20)
            Me.nupMaxEffChg.TabIndex = 12
            '
            'nudBaseYear
            '
            Me.nudBaseYear.Location = New System.Drawing.Point(116, 123)
            Me.nudBaseYear.Maximum = New Decimal(New Integer() {9999, 0, 0, 0})
            Me.nudBaseYear.Name = "nudBaseYear"
            Me.nudBaseYear.Size = New System.Drawing.Size(60, 20)
            Me.nudBaseYear.TabIndex = 10
            '
            'lblBaseYear
            '
            Me.lblBaseYear.AutoSize = True
            Me.lblBaseYear.Location = New System.Drawing.Point(5, 125)
            Me.lblBaseYear.Name = "lblBaseYear"
            Me.lblBaseYear.Size = New System.Drawing.Size(57, 13)
            Me.lblBaseYear.TabIndex = 9
            Me.lblBaseYear.Text = "Base year:"
            '
            'cmbOptmApproach
            '
            Me.cmbOptmApproach.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.cmbOptmApproach.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.cmbOptmApproach.FormattingEnabled = True
            Me.cmbOptmApproach.Items.AddRange(New Object() {"Maximize system objective", "Maximize by fleet values"})
            Me.cmbOptmApproach.Location = New System.Drawing.Point(116, 226)
            Me.cmbOptmApproach.Name = "cmbOptmApproach"
            Me.cmbOptmApproach.Size = New System.Drawing.Size(143, 21)
            Me.cmbOptmApproach.TabIndex = 18
            '
            'lblMaxEffChg
            '
            Me.lblMaxEffChg.AutoSize = True
            Me.lblMaxEffChg.Location = New System.Drawing.Point(5, 152)
            Me.lblMaxEffChg.Name = "lblMaxEffChg"
            Me.lblMaxEffChg.Size = New System.Drawing.Size(96, 13)
            Me.lblMaxEffChg.TabIndex = 11
            Me.lblMaxEffChg.Text = "Max effort change:"
            '
            'lblOptmApproach
            '
            Me.lblOptmApproach.Location = New System.Drawing.Point(5, 221)
            Me.lblOptmApproach.Name = "lblOptmApproach"
            Me.lblOptmApproach.Size = New System.Drawing.Size(72, 32)
            Me.lblOptmApproach.TabIndex = 17
            Me.lblOptmApproach.Text = "Optimization approach:"
            '
            'cmbSearchUsing
            '
            Me.cmbSearchUsing.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.cmbSearchUsing.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.cmbSearchUsing.FormattingEnabled = True
            Me.cmbSearchUsing.Items.AddRange(New Object() {"Fletch", "DFPmin"})
            Me.cmbSearchUsing.Location = New System.Drawing.Point(116, 199)
            Me.cmbSearchUsing.Name = "cmbSearchUsing"
            Me.cmbSearchUsing.Size = New System.Drawing.Size(143, 21)
            Me.cmbSearchUsing.TabIndex = 16
            '
            'lblSearchUsing
            '
            Me.lblSearchUsing.AutoSize = True
            Me.lblSearchUsing.Location = New System.Drawing.Point(5, 202)
            Me.lblSearchUsing.Name = "lblSearchUsing"
            Me.lblSearchUsing.Size = New System.Drawing.Size(72, 13)
            Me.lblSearchUsing.TabIndex = 15
            Me.lblSearchUsing.Text = "Search using:"
            '
            'cmbInitUsing
            '
            Me.cmbInitUsing.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.cmbInitUsing.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.cmbInitUsing.FormattingEnabled = True
            Me.cmbInitUsing.Items.AddRange(New Object() {"Ecopath base F's", "Current F's", "Random F's"})
            Me.cmbInitUsing.Location = New System.Drawing.Point(116, 176)
            Me.cmbInitUsing.Name = "cmbInitUsing"
            Me.cmbInitUsing.Size = New System.Drawing.Size(143, 21)
            Me.cmbInitUsing.TabIndex = 14
            '
            'lblInitUsing
            '
            Me.lblInitUsing.AutoSize = True
            Me.lblInitUsing.Location = New System.Drawing.Point(5, 179)
            Me.lblInitUsing.Name = "lblInitUsing"
            Me.lblInitUsing.Size = New System.Drawing.Size(75, 13)
            Me.lblInitUsing.TabIndex = 13
            Me.lblInitUsing.Text = "Initialize using:"
            '
            'nupMaxNumEval
            '
            Me.nupMaxNumEval.Location = New System.Drawing.Point(116, 97)
            Me.nupMaxNumEval.Maximum = New Decimal(New Integer() {1000000, 0, 0, 0})
            Me.nupMaxNumEval.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
            Me.nupMaxNumEval.Name = "nupMaxNumEval"
            Me.nupMaxNumEval.Size = New System.Drawing.Size(60, 20)
            Me.nupMaxNumEval.TabIndex = 8
            Me.nupMaxNumEval.Value = New Decimal(New Integer() {2000, 0, 0, 0})
            '
            'nupNumOfRuns
            '
            Me.nupNumOfRuns.Location = New System.Drawing.Point(116, 71)
            Me.nupNumOfRuns.Maximum = New Decimal(New Integer() {10000, 0, 0, 0})
            Me.nupNumOfRuns.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
            Me.nupNumOfRuns.Name = "nupNumOfRuns"
            Me.nupNumOfRuns.Size = New System.Drawing.Size(60, 20)
            Me.nupNumOfRuns.TabIndex = 6
            Me.nupNumOfRuns.Value = New Decimal(New Integer() {1, 0, 0, 0})
            '
            'lblMaxNumEval
            '
            Me.lblMaxNumEval.AutoSize = True
            Me.lblMaxNumEval.Location = New System.Drawing.Point(5, 99)
            Me.lblMaxNumEval.Name = "lblMaxNumEval"
            Me.lblMaxNumEval.Size = New System.Drawing.Size(63, 13)
            Me.lblMaxNumEval.TabIndex = 7
            Me.lblMaxNumEval.Text = "Max # eval:"
            '
            'lblNumOfRuns
            '
            Me.lblNumOfRuns.AutoSize = True
            Me.lblNumOfRuns.Location = New System.Drawing.Point(5, 73)
            Me.lblNumOfRuns.Name = "lblNumOfRuns"
            Me.lblNumOfRuns.Size = New System.Drawing.Size(64, 13)
            Me.lblNumOfRuns.TabIndex = 5
            Me.lblNumOfRuns.Text = "No of Runs:"
            '
            'lblInitializationHeader
            '
            Me.lblInitializationHeader.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.lblInitializationHeader.BackColor = System.Drawing.SystemColors.ButtonShadow
            Me.lblInitializationHeader.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold)
            Me.lblInitializationHeader.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
            Me.lblInitializationHeader.ImeMode = System.Windows.Forms.ImeMode.NoControl
            Me.lblInitializationHeader.Location = New System.Drawing.Point(0, 0)
            Me.lblInitializationHeader.Name = "lblInitializationHeader"
            Me.lblInitializationHeader.Size = New System.Drawing.Size(262, 18)
            Me.lblInitializationHeader.TabIndex = 0
            Me.lblInitializationHeader.Text = "Parameters"
            Me.lblInitializationHeader.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'plBlocks
            '
            Me.plBlocks.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.plBlocks.Location = New System.Drawing.Point(267, -1)
            Me.plBlocks.Name = "plBlocks"
            Me.plBlocks.Size = New System.Drawing.Size(854, 349)
            Me.plBlocks.TabIndex = 0
            '
            'tcMain
            '
            Me.tcMain.Controls.Add(Me.tbpObjv)
            Me.tcMain.Controls.Add(Me.tbpResultTable)
            Me.tcMain.Controls.Add(Me.m_tpPlots)
            Me.tcMain.Dock = System.Windows.Forms.DockStyle.Fill
            Me.tcMain.Location = New System.Drawing.Point(0, 0)
            Me.tcMain.Name = "tcMain"
            Me.tcMain.SelectedIndex = 0
            Me.tcMain.Size = New System.Drawing.Size(893, 275)
            Me.tcMain.TabIndex = 0
            '
            'tbpObjv
            '
            Me.tbpObjv.Controls.Add(Me.SplitContainer2)
            Me.tbpObjv.Location = New System.Drawing.Point(4, 22)
            Me.tbpObjv.Name = "tbpObjv"
            Me.tbpObjv.Padding = New System.Windows.Forms.Padding(3)
            Me.tbpObjv.Size = New System.Drawing.Size(885, 249)
            Me.tbpObjv.TabIndex = 0
            Me.tbpObjv.Text = "Objectives"
            Me.tbpObjv.UseVisualStyleBackColor = True
            '
            'SplitContainer2
            '
            Me.SplitContainer2.Dock = System.Windows.Forms.DockStyle.Fill
            Me.SplitContainer2.Location = New System.Drawing.Point(3, 3)
            Me.SplitContainer2.Name = "SplitContainer2"
            '
            'SplitContainer2.Panel2
            '
            Me.SplitContainer2.Panel2.Controls.Add(Me.SplitContainer3)
            Me.SplitContainer2.Size = New System.Drawing.Size(879, 243)
            Me.SplitContainer2.SplitterDistance = 288
            Me.SplitContainer2.TabIndex = 0
            '
            'SplitContainer3
            '
            Me.SplitContainer3.Dock = System.Windows.Forms.DockStyle.Fill
            Me.SplitContainer3.Location = New System.Drawing.Point(0, 0)
            Me.SplitContainer3.Name = "SplitContainer3"
            Me.SplitContainer3.Size = New System.Drawing.Size(587, 243)
            Me.SplitContainer3.SplitterDistance = 191
            Me.SplitContainer3.TabIndex = 0
            '
            'tbpResultTable
            '
            Me.tbpResultTable.Controls.Add(Me.scIterResult)
            Me.tbpResultTable.Location = New System.Drawing.Point(4, 22)
            Me.tbpResultTable.Name = "tbpResultTable"
            Me.tbpResultTable.Padding = New System.Windows.Forms.Padding(3)
            Me.tbpResultTable.Size = New System.Drawing.Size(885, 249)
            Me.tbpResultTable.TabIndex = 1
            Me.tbpResultTable.Text = "Iteration results"
            Me.tbpResultTable.UseVisualStyleBackColor = True
            '
            'scIterResult
            '
            Me.scIterResult.Dock = System.Windows.Forms.DockStyle.Fill
            Me.scIterResult.Location = New System.Drawing.Point(3, 3)
            Me.scIterResult.Name = "scIterResult"
            '
            'scIterResult.Panel1
            '
            Me.scIterResult.Panel1.Controls.Add(Me.scIterResultMultiRun)
            Me.scIterResult.Size = New System.Drawing.Size(879, 243)
            Me.scIterResult.SplitterDistance = 488
            Me.scIterResult.TabIndex = 0
            '
            'scIterResultMultiRun
            '
            Me.scIterResultMultiRun.Dock = System.Windows.Forms.DockStyle.Fill
            Me.scIterResultMultiRun.Location = New System.Drawing.Point(0, 0)
            Me.scIterResultMultiRun.Name = "scIterResultMultiRun"
            Me.scIterResultMultiRun.Orientation = System.Windows.Forms.Orientation.Horizontal
            Me.scIterResultMultiRun.Size = New System.Drawing.Size(488, 243)
            Me.scIterResultMultiRun.SplitterDistance = 124
            Me.scIterResultMultiRun.TabIndex = 0
            '
            'm_tpPlots
            '
            Me.m_tpPlots.Controls.Add(Me.m_splcPlotResults)
            Me.m_tpPlots.Location = New System.Drawing.Point(4, 22)
            Me.m_tpPlots.Name = "m_tpPlots"
            Me.m_tpPlots.Padding = New System.Windows.Forms.Padding(3)
            Me.m_tpPlots.Size = New System.Drawing.Size(885, 249)
            Me.m_tpPlots.TabIndex = 2
            Me.m_tpPlots.Text = "Plot results"
            Me.m_tpPlots.UseVisualStyleBackColor = True
            '
            'm_splcPlotResults
            '
            Me.m_splcPlotResults.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_splcPlotResults.Location = New System.Drawing.Point(3, 3)
            Me.m_splcPlotResults.Name = "m_splcPlotResults"
            '
            'm_splcPlotResults.Panel1
            '
            Me.m_splcPlotResults.Panel1.Controls.Add(Me.m_graphResults)
            '
            'm_splcPlotResults.Panel2
            '
            Me.m_splcPlotResults.Panel2.Controls.Add(Me.m_zgcKiteDiag)
            Me.m_splcPlotResults.Panel2Collapsed = True
            Me.m_splcPlotResults.Size = New System.Drawing.Size(879, 243)
            Me.m_splcPlotResults.SplitterDistance = 626
            Me.m_splcPlotResults.TabIndex = 10
            '
            'm_graphResults
            '
            Me.m_graphResults.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_graphResults.IsAutoScrollRange = True
            Me.m_graphResults.Location = New System.Drawing.Point(0, 0)
            Me.m_graphResults.Name = "m_graphResults"
            Me.m_graphResults.ScrollGrace = 0
            Me.m_graphResults.ScrollMaxX = 0
            Me.m_graphResults.ScrollMaxY = 0
            Me.m_graphResults.ScrollMaxY2 = 0
            Me.m_graphResults.ScrollMinX = 0
            Me.m_graphResults.ScrollMinY = 0
            Me.m_graphResults.ScrollMinY2 = 0
            Me.m_graphResults.Size = New System.Drawing.Size(879, 243)
            Me.m_graphResults.TabIndex = 8
            '
            'm_zgcKiteDiag
            '
            Me.m_zgcKiteDiag.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_zgcKiteDiag.IsAutoScrollRange = True
            Me.m_zgcKiteDiag.Location = New System.Drawing.Point(0, 0)
            Me.m_zgcKiteDiag.Name = "m_zgcKiteDiag"
            Me.m_zgcKiteDiag.ScrollGrace = 0
            Me.m_zgcKiteDiag.ScrollMaxX = 0
            Me.m_zgcKiteDiag.ScrollMaxY = 0
            Me.m_zgcKiteDiag.ScrollMaxY2 = 0
            Me.m_zgcKiteDiag.ScrollMinX = 0
            Me.m_zgcKiteDiag.ScrollMinY = 0
            Me.m_zgcKiteDiag.ScrollMinY2 = 0
            Me.m_zgcKiteDiag.Size = New System.Drawing.Size(249, 243)
            Me.m_zgcKiteDiag.TabIndex = 9
            '
            'frmFishingPolicySearch
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(897, 621)
            Me.Controls.Add(Me.SplitContainer1)
            Me.Name = "frmFishingPolicySearch"
            Me.TabText = "Fishing policy search"
            Me.Text = "Fishing policy search"
            Me.SplitContainer1.Panel1.ResumeLayout(False)
            Me.SplitContainer1.Panel2.ResumeLayout(False)
            Me.SplitContainer1.ResumeLayout(False)
            Me.plRunParams.ResumeLayout(False)
            Me.plRunParams.PerformLayout()
            Me.plMaxSO.ResumeLayout(False)
            Me.plMaxSO.PerformLayout()
            CType(Me.nupMaxEffChg, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.nudBaseYear, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.nupMaxNumEval, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.nupNumOfRuns, System.ComponentModel.ISupportInitialize).EndInit()
            Me.tcMain.ResumeLayout(False)
            Me.tbpObjv.ResumeLayout(False)
            Me.SplitContainer2.Panel2.ResumeLayout(False)
            Me.SplitContainer2.ResumeLayout(False)
            Me.SplitContainer3.ResumeLayout(False)
            Me.tbpResultTable.ResumeLayout(False)
            Me.scIterResult.Panel1.ResumeLayout(False)
            Me.scIterResult.ResumeLayout(False)
            Me.scIterResultMultiRun.ResumeLayout(False)
            Me.m_tpPlots.ResumeLayout(False)
            Me.m_splcPlotResults.Panel1.ResumeLayout(False)
            Me.m_splcPlotResults.Panel2.ResumeLayout(False)
            Me.m_splcPlotResults.ResumeLayout(False)
            Me.ResumeLayout(False)

        End Sub
        Friend WithEvents SplitContainer1 As System.Windows.Forms.SplitContainer
        Friend WithEvents plBlocks As System.Windows.Forms.Panel
        Friend WithEvents plRunParams As System.Windows.Forms.Panel
        Friend WithEvents lblInitializationHeader As System.Windows.Forms.Label
        Friend WithEvents lblInitUsing As System.Windows.Forms.Label
        Friend WithEvents nupMaxNumEval As System.Windows.Forms.NumericUpDown
        Friend WithEvents nupNumOfRuns As System.Windows.Forms.NumericUpDown
        Friend WithEvents lblMaxNumEval As System.Windows.Forms.Label
        Friend WithEvents lblNumOfRuns As System.Windows.Forms.Label
        Friend WithEvents cmbSearchUsing As System.Windows.Forms.ComboBox
        Friend WithEvents cmbInitUsing As System.Windows.Forms.ComboBox
        Friend WithEvents cmbOptmApproach As System.Windows.Forms.ComboBox
        Friend WithEvents lblOptmApproach As System.Windows.Forms.Label
        Friend WithEvents tcMain As System.Windows.Forms.TabControl
        Friend WithEvents tbpObjv As System.Windows.Forms.TabPage
        Friend WithEvents tbpResultTable As System.Windows.Forms.TabPage
        Friend WithEvents nupMaxEffChg As System.Windows.Forms.NumericUpDown
        Friend WithEvents lblBaseYear As System.Windows.Forms.Label
        Friend WithEvents lblMaxEffChg As System.Windows.Forms.Label
        Friend WithEvents nudBaseYear As System.Windows.Forms.NumericUpDown
        Friend WithEvents SplitContainer2 As System.Windows.Forms.SplitContainer
        Friend WithEvents SplitContainer3 As System.Windows.Forms.SplitContainer
        Friend WithEvents scIterResult As System.Windows.Forms.SplitContainer
        Friend WithEvents scIterResultMultiRun As System.Windows.Forms.SplitContainer
        Friend WithEvents Label1 As System.Windows.Forms.Label
        Friend WithEvents txDiscountRate As System.Windows.Forms.TextBox
        Friend WithEvents Label2 As System.Windows.Forms.Label
        Friend WithEvents txGenDiscRate As System.Windows.Forms.TextBox
        Friend WithEvents lblSearchUsing As System.Windows.Forms.Label
        Friend WithEvents plMaxSO As System.Windows.Forms.Panel
        Friend WithEvents cbPrevCE As System.Windows.Forms.CheckBox
        Friend WithEvents cbMaxPortUl As System.Windows.Forms.CheckBox
        Friend WithEvents cbIncludeCCosts As System.Windows.Forms.CheckBox
        Friend WithEvents btnStop As System.Windows.Forms.Button
        Friend WithEvents btnSearch As System.Windows.Forms.Button
        Friend WithEvents m_tpPlots As System.Windows.Forms.TabPage
        Private WithEvents m_zgcKiteDiag As ZedGraph.ZedGraphControl
        Private WithEvents m_graphResults As ZedGraph.ZedGraphControl
        Friend WithEvents m_splcPlotResults As System.Windows.Forms.SplitContainer

    End Class
End Namespace