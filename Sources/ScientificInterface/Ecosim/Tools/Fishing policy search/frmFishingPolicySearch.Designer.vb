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
            Me.m_scTopBits = New System.Windows.Forms.SplitContainer
            Me.m_tlpRunStop = New System.Windows.Forms.TableLayoutPanel
            Me.btnStop = New System.Windows.Forms.Button
            Me.btnSearch = New System.Windows.Forms.Button
            Me.m_plRunParams = New System.Windows.Forms.Panel
            Me.m_chkIncludeCCosts = New System.Windows.Forms.CheckBox
            Me.m_chkMaxPortUl = New System.Windows.Forms.CheckBox
            Me.m_chkUsePlugin = New System.Windows.Forms.CheckBox
            Me.m_chkPrevCE = New System.Windows.Forms.CheckBox
            Me.m_lblGenDiscRate = New System.Windows.Forms.Label
            Me.m_txtGenDiscRate = New System.Windows.Forms.TextBox
            Me.m_lblDiscRate = New System.Windows.Forms.Label
            Me.m_txtDiscountRate = New System.Windows.Forms.TextBox
            Me.m_nudMaxEffChg = New System.Windows.Forms.NumericUpDown
            Me.m_nudBaseYear = New System.Windows.Forms.NumericUpDown
            Me.m_lblBaseYear = New System.Windows.Forms.Label
            Me.m_cmbOptmApproach = New System.Windows.Forms.ComboBox
            Me.m_lblMaxEffChg = New System.Windows.Forms.Label
            Me.m_lblOptmApproach = New System.Windows.Forms.Label
            Me.m_cmbSearchUsing = New System.Windows.Forms.ComboBox
            Me.m_lblSearchUsing = New System.Windows.Forms.Label
            Me.m_cmbInitUsing = New System.Windows.Forms.ComboBox
            Me.m_lblInitUsing = New System.Windows.Forms.Label
            Me.m_nudMaxNumEval = New System.Windows.Forms.NumericUpDown
            Me.m_nudNumberOfRuns = New System.Windows.Forms.NumericUpDown
            Me.m_lblMaxNumEval = New System.Windows.Forms.Label
            Me.m_lblNumOfRuns = New System.Windows.Forms.Label
            Me.lblInitializationHeader = New System.Windows.Forms.Label
            Me.m_plBlocks = New System.Windows.Forms.Panel
            Me.m_tcMain = New System.Windows.Forms.TabControl
            Me.m_tabObjectives = New System.Windows.Forms.TabPage
            Me.m_scObjectives = New System.Windows.Forms.SplitContainer
            Me.m_scAarghArghAaargh = New System.Windows.Forms.SplitContainer
            Me.m_tabResultTable = New System.Windows.Forms.TabPage
            Me.m_scIterResult = New System.Windows.Forms.SplitContainer
            Me.m_scIterResultMultiRun = New System.Windows.Forms.SplitContainer
            Me.m_tpPlots = New System.Windows.Forms.TabPage
            Me.m_splcPlotResults = New System.Windows.Forms.SplitContainer
            Me.m_graphResults = New ZedGraph.ZedGraphControl
            Me.m_zgcKiteDiag = New ZedGraph.ZedGraphControl
            Me.m_scTopBits.Panel1.SuspendLayout()
            Me.m_scTopBits.Panel2.SuspendLayout()
            Me.m_scTopBits.SuspendLayout()
            Me.m_tlpRunStop.SuspendLayout()
            Me.m_plRunParams.SuspendLayout()
            CType(Me.m_nudMaxEffChg, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.m_nudBaseYear, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.m_nudMaxNumEval, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.m_nudNumberOfRuns, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.m_tcMain.SuspendLayout()
            Me.m_tabObjectives.SuspendLayout()
            Me.m_scObjectives.Panel2.SuspendLayout()
            Me.m_scObjectives.SuspendLayout()
            Me.m_scAarghArghAaargh.SuspendLayout()
            Me.m_tabResultTable.SuspendLayout()
            Me.m_scIterResult.Panel1.SuspendLayout()
            Me.m_scIterResult.SuspendLayout()
            Me.m_scIterResultMultiRun.SuspendLayout()
            Me.m_tpPlots.SuspendLayout()
            Me.m_splcPlotResults.Panel1.SuspendLayout()
            Me.m_splcPlotResults.Panel2.SuspendLayout()
            Me.m_splcPlotResults.SuspendLayout()
            Me.SuspendLayout()
            '
            'm_scTopBits
            '
            Me.m_scTopBits.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
            Me.m_scTopBits.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_scTopBits.FixedPanel = System.Windows.Forms.FixedPanel.Panel1
            Me.m_scTopBits.Location = New System.Drawing.Point(0, 0)
            Me.m_scTopBits.Margin = New System.Windows.Forms.Padding(0)
            Me.m_scTopBits.Name = "m_scTopBits"
            Me.m_scTopBits.Orientation = System.Windows.Forms.Orientation.Horizontal
            '
            'm_scTopBits.Panel1
            '
            Me.m_scTopBits.Panel1.Controls.Add(Me.m_tlpRunStop)
            Me.m_scTopBits.Panel1.Controls.Add(Me.m_plRunParams)
            Me.m_scTopBits.Panel1.Controls.Add(Me.m_plBlocks)
            Me.m_scTopBits.Panel1MinSize = 338
            '
            'm_scTopBits.Panel2
            '
            Me.m_scTopBits.Panel2.Controls.Add(Me.m_tcMain)
            Me.m_scTopBits.Size = New System.Drawing.Size(993, 687)
            Me.m_scTopBits.SplitterDistance = 381
            Me.m_scTopBits.TabIndex = 0
            '
            'm_tlpRunStop
            '
            Me.m_tlpRunStop.ColumnCount = 5
            Me.m_tlpRunStop.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
            Me.m_tlpRunStop.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 75.0!))
            Me.m_tlpRunStop.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 3.0!))
            Me.m_tlpRunStop.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 75.0!))
            Me.m_tlpRunStop.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
            Me.m_tlpRunStop.Controls.Add(Me.btnStop, 3, 0)
            Me.m_tlpRunStop.Controls.Add(Me.btnSearch, 1, 0)
            Me.m_tlpRunStop.Location = New System.Drawing.Point(0, 350)
            Me.m_tlpRunStop.Margin = New System.Windows.Forms.Padding(0)
            Me.m_tlpRunStop.Name = "m_tlpRunStop"
            Me.m_tlpRunStop.RowCount = 1
            Me.m_tlpRunStop.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
            Me.m_tlpRunStop.Size = New System.Drawing.Size(265, 23)
            Me.m_tlpRunStop.TabIndex = 0
            '
            'btnStop
            '
            Me.btnStop.Location = New System.Drawing.Point(134, 0)
            Me.btnStop.Margin = New System.Windows.Forms.Padding(0)
            Me.btnStop.Name = "btnStop"
            Me.btnStop.Size = New System.Drawing.Size(75, 23)
            Me.btnStop.TabIndex = 1
            Me.btnStop.Text = "Sto&p"
            Me.btnStop.UseVisualStyleBackColor = True
            '
            'btnSearch
            '
            Me.btnSearch.Location = New System.Drawing.Point(56, 0)
            Me.btnSearch.Margin = New System.Windows.Forms.Padding(0)
            Me.btnSearch.Name = "btnSearch"
            Me.btnSearch.Size = New System.Drawing.Size(75, 23)
            Me.btnSearch.TabIndex = 0
            Me.btnSearch.Text = "&Search"
            Me.btnSearch.UseVisualStyleBackColor = True
            '
            'm_plRunParams
            '
            Me.m_plRunParams.Controls.Add(Me.m_chkIncludeCCosts)
            Me.m_plRunParams.Controls.Add(Me.m_chkMaxPortUl)
            Me.m_plRunParams.Controls.Add(Me.m_chkUsePlugin)
            Me.m_plRunParams.Controls.Add(Me.m_chkPrevCE)
            Me.m_plRunParams.Controls.Add(Me.m_lblGenDiscRate)
            Me.m_plRunParams.Controls.Add(Me.m_txtGenDiscRate)
            Me.m_plRunParams.Controls.Add(Me.m_lblDiscRate)
            Me.m_plRunParams.Controls.Add(Me.m_txtDiscountRate)
            Me.m_plRunParams.Controls.Add(Me.m_nudMaxEffChg)
            Me.m_plRunParams.Controls.Add(Me.m_nudBaseYear)
            Me.m_plRunParams.Controls.Add(Me.m_lblBaseYear)
            Me.m_plRunParams.Controls.Add(Me.m_cmbOptmApproach)
            Me.m_plRunParams.Controls.Add(Me.m_lblMaxEffChg)
            Me.m_plRunParams.Controls.Add(Me.m_lblOptmApproach)
            Me.m_plRunParams.Controls.Add(Me.m_cmbSearchUsing)
            Me.m_plRunParams.Controls.Add(Me.m_lblSearchUsing)
            Me.m_plRunParams.Controls.Add(Me.m_cmbInitUsing)
            Me.m_plRunParams.Controls.Add(Me.m_lblInitUsing)
            Me.m_plRunParams.Controls.Add(Me.m_nudMaxNumEval)
            Me.m_plRunParams.Controls.Add(Me.m_nudNumberOfRuns)
            Me.m_plRunParams.Controls.Add(Me.m_lblMaxNumEval)
            Me.m_plRunParams.Controls.Add(Me.m_lblNumOfRuns)
            Me.m_plRunParams.Controls.Add(Me.lblInitializationHeader)
            Me.m_plRunParams.Location = New System.Drawing.Point(0, 0)
            Me.m_plRunParams.Margin = New System.Windows.Forms.Padding(0)
            Me.m_plRunParams.Name = "m_plRunParams"
            Me.m_plRunParams.Size = New System.Drawing.Size(265, 344)
            Me.m_plRunParams.TabIndex = 0
            '
            'm_chkIncludeCCosts
            '
            Me.m_chkIncludeCCosts.AutoSize = True
            Me.m_chkIncludeCCosts.Location = New System.Drawing.Point(8, 325)
            Me.m_chkIncludeCCosts.Name = "m_chkIncludeCCosts"
            Me.m_chkIncludeCCosts.Size = New System.Drawing.Size(133, 17)
            Me.m_chkIncludeCCosts.TabIndex = 21
            Me.m_chkIncludeCCosts.Text = "Include &compete costs"
            Me.m_chkIncludeCCosts.UseVisualStyleBackColor = True
            '
            'm_chkMaxPortUl
            '
            Me.m_chkMaxPortUl.AutoSize = True
            Me.m_chkMaxPortUl.Location = New System.Drawing.Point(8, 302)
            Me.m_chkMaxPortUl.Name = "m_chkMaxPortUl"
            Me.m_chkMaxPortUl.Size = New System.Drawing.Size(135, 17)
            Me.m_chkMaxPortUl.TabIndex = 20
            Me.m_chkMaxPortUl.Text = "Maximize portfolio &utility"
            Me.m_chkMaxPortUl.UseVisualStyleBackColor = True
            '
            'm_chkUsePlugin
            '
            Me.m_chkUsePlugin.AutoSize = True
            Me.m_chkUsePlugin.Location = New System.Drawing.Point(8, 256)
            Me.m_chkUsePlugin.Name = "m_chkUsePlugin"
            Me.m_chkUsePlugin.Size = New System.Drawing.Size(152, 17)
            Me.m_chkUsePlugin.TabIndex = 18
            Me.m_chkUsePlugin.Text = "Use p&lug-in economic data"
            Me.m_chkUsePlugin.UseVisualStyleBackColor = True
            '
            'm_chkPrevCE
            '
            Me.m_chkPrevCE.AutoSize = True
            Me.m_chkPrevCE.Location = New System.Drawing.Point(8, 279)
            Me.m_chkPrevCE.Name = "m_chkPrevCE"
            Me.m_chkPrevCE.Size = New System.Drawing.Size(138, 17)
            Me.m_chkPrevCE.TabIndex = 19
            Me.m_chkPrevCE.Text = "&Prevent cost > earnings"
            Me.m_chkPrevCE.UseVisualStyleBackColor = True
            '
            'm_lblGenDiscRate
            '
            Me.m_lblGenDiscRate.AutoSize = True
            Me.m_lblGenDiscRate.Location = New System.Drawing.Point(5, 51)
            Me.m_lblGenDiscRate.Name = "m_lblGenDiscRate"
            Me.m_lblGenDiscRate.Size = New System.Drawing.Size(111, 13)
            Me.m_lblGenDiscRate.TabIndex = 2
            Me.m_lblGenDiscRate.Text = "&Generic discount rate:"
            '
            'm_txtGenDiscRate
            '
            Me.m_txtGenDiscRate.Location = New System.Drawing.Point(122, 48)
            Me.m_txtGenDiscRate.Name = "m_txtGenDiscRate"
            Me.m_txtGenDiscRate.Size = New System.Drawing.Size(60, 20)
            Me.m_txtGenDiscRate.TabIndex = 3
            '
            'm_lblDiscRate
            '
            Me.m_lblDiscRate.AutoSize = True
            Me.m_lblDiscRate.Location = New System.Drawing.Point(5, 25)
            Me.m_lblDiscRate.Name = "m_lblDiscRate"
            Me.m_lblDiscRate.Size = New System.Drawing.Size(73, 13)
            Me.m_lblDiscRate.TabIndex = 0
            Me.m_lblDiscRate.Text = "&Discount rate:"
            '
            'm_txtDiscountRate
            '
            Me.m_txtDiscountRate.Location = New System.Drawing.Point(122, 22)
            Me.m_txtDiscountRate.Name = "m_txtDiscountRate"
            Me.m_txtDiscountRate.Size = New System.Drawing.Size(60, 20)
            Me.m_txtDiscountRate.TabIndex = 1
            '
            'm_nudMaxEffChg
            '
            Me.m_nudMaxEffChg.Location = New System.Drawing.Point(122, 153)
            Me.m_nudMaxEffChg.Name = "m_nudMaxEffChg"
            Me.m_nudMaxEffChg.Size = New System.Drawing.Size(60, 20)
            Me.m_nudMaxEffChg.TabIndex = 11
            '
            'm_nudBaseYear
            '
            Me.m_nudBaseYear.Location = New System.Drawing.Point(122, 126)
            Me.m_nudBaseYear.Maximum = New Decimal(New Integer() {9999, 0, 0, 0})
            Me.m_nudBaseYear.Name = "m_nudBaseYear"
            Me.m_nudBaseYear.Size = New System.Drawing.Size(60, 20)
            Me.m_nudBaseYear.TabIndex = 9
            '
            'm_lblBaseYear
            '
            Me.m_lblBaseYear.AutoSize = True
            Me.m_lblBaseYear.Location = New System.Drawing.Point(5, 128)
            Me.m_lblBaseYear.Name = "m_lblBaseYear"
            Me.m_lblBaseYear.Size = New System.Drawing.Size(57, 13)
            Me.m_lblBaseYear.TabIndex = 8
            Me.m_lblBaseYear.Text = "&Base year:"
            '
            'm_cmbOptmApproach
            '
            Me.m_cmbOptmApproach.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_cmbOptmApproach.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.m_cmbOptmApproach.FormattingEnabled = True
            Me.m_cmbOptmApproach.Items.AddRange(New Object() {"Maximize system objective", "Maximize by fleet values"})
            Me.m_cmbOptmApproach.Location = New System.Drawing.Point(122, 229)
            Me.m_cmbOptmApproach.Name = "m_cmbOptmApproach"
            Me.m_cmbOptmApproach.Size = New System.Drawing.Size(140, 21)
            Me.m_cmbOptmApproach.TabIndex = 17
            '
            'm_lblMaxEffChg
            '
            Me.m_lblMaxEffChg.AutoSize = True
            Me.m_lblMaxEffChg.Location = New System.Drawing.Point(5, 155)
            Me.m_lblMaxEffChg.Name = "m_lblMaxEffChg"
            Me.m_lblMaxEffChg.Size = New System.Drawing.Size(96, 13)
            Me.m_lblMaxEffChg.TabIndex = 10
            Me.m_lblMaxEffChg.Text = "Max e&ffort change:"
            '
            'm_lblOptmApproach
            '
            Me.m_lblOptmApproach.Location = New System.Drawing.Point(5, 224)
            Me.m_lblOptmApproach.Name = "m_lblOptmApproach"
            Me.m_lblOptmApproach.Size = New System.Drawing.Size(105, 32)
            Me.m_lblOptmApproach.TabIndex = 16
            Me.m_lblOptmApproach.Text = "&Optimization approach:"
            '
            'm_cmbSearchUsing
            '
            Me.m_cmbSearchUsing.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_cmbSearchUsing.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.m_cmbSearchUsing.FormattingEnabled = True
            Me.m_cmbSearchUsing.Items.AddRange(New Object() {"Fletch", "DFPmin"})
            Me.m_cmbSearchUsing.Location = New System.Drawing.Point(122, 202)
            Me.m_cmbSearchUsing.Name = "m_cmbSearchUsing"
            Me.m_cmbSearchUsing.Size = New System.Drawing.Size(140, 21)
            Me.m_cmbSearchUsing.TabIndex = 15
            '
            'm_lblSearchUsing
            '
            Me.m_lblSearchUsing.AutoSize = True
            Me.m_lblSearchUsing.Location = New System.Drawing.Point(5, 205)
            Me.m_lblSearchUsing.Name = "m_lblSearchUsing"
            Me.m_lblSearchUsing.Size = New System.Drawing.Size(72, 13)
            Me.m_lblSearchUsing.TabIndex = 14
            Me.m_lblSearchUsing.Text = "&Search using:"
            '
            'm_cmbInitUsing
            '
            Me.m_cmbInitUsing.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_cmbInitUsing.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.m_cmbInitUsing.FormattingEnabled = True
            Me.m_cmbInitUsing.Items.AddRange(New Object() {"Ecopath base F's", "Current F's", "Random F's"})
            Me.m_cmbInitUsing.Location = New System.Drawing.Point(122, 179)
            Me.m_cmbInitUsing.Name = "m_cmbInitUsing"
            Me.m_cmbInitUsing.Size = New System.Drawing.Size(140, 21)
            Me.m_cmbInitUsing.TabIndex = 13
            '
            'm_lblInitUsing
            '
            Me.m_lblInitUsing.AutoSize = True
            Me.m_lblInitUsing.Location = New System.Drawing.Point(5, 182)
            Me.m_lblInitUsing.Name = "m_lblInitUsing"
            Me.m_lblInitUsing.Size = New System.Drawing.Size(75, 13)
            Me.m_lblInitUsing.TabIndex = 12
            Me.m_lblInitUsing.Text = "&Initialize using:"
            '
            'm_nudMaxNumEval
            '
            Me.m_nudMaxNumEval.Location = New System.Drawing.Point(122, 100)
            Me.m_nudMaxNumEval.Maximum = New Decimal(New Integer() {1000000, 0, 0, 0})
            Me.m_nudMaxNumEval.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
            Me.m_nudMaxNumEval.Name = "m_nudMaxNumEval"
            Me.m_nudMaxNumEval.Size = New System.Drawing.Size(60, 20)
            Me.m_nudMaxNumEval.TabIndex = 7
            Me.m_nudMaxNumEval.Value = New Decimal(New Integer() {2000, 0, 0, 0})
            '
            'm_nudNumberOfRuns
            '
            Me.m_nudNumberOfRuns.Location = New System.Drawing.Point(122, 74)
            Me.m_nudNumberOfRuns.Maximum = New Decimal(New Integer() {10000, 0, 0, 0})
            Me.m_nudNumberOfRuns.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
            Me.m_nudNumberOfRuns.Name = "m_nudNumberOfRuns"
            Me.m_nudNumberOfRuns.Size = New System.Drawing.Size(60, 20)
            Me.m_nudNumberOfRuns.TabIndex = 5
            Me.m_nudNumberOfRuns.Value = New Decimal(New Integer() {1, 0, 0, 0})
            '
            'm_lblMaxNumEval
            '
            Me.m_lblMaxNumEval.AutoSize = True
            Me.m_lblMaxNumEval.Location = New System.Drawing.Point(5, 102)
            Me.m_lblMaxNumEval.Name = "m_lblMaxNumEval"
            Me.m_lblMaxNumEval.Size = New System.Drawing.Size(85, 13)
            Me.m_lblMaxNumEval.TabIndex = 6
            Me.m_lblMaxNumEval.Text = "Max no of &evals:"
            '
            'm_lblNumOfRuns
            '
            Me.m_lblNumOfRuns.AutoSize = True
            Me.m_lblNumOfRuns.Location = New System.Drawing.Point(5, 76)
            Me.m_lblNumOfRuns.Name = "m_lblNumOfRuns"
            Me.m_lblNumOfRuns.Size = New System.Drawing.Size(82, 13)
            Me.m_lblNumOfRuns.TabIndex = 4
            Me.m_lblNumOfRuns.Text = "&Number of runs:"
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
            Me.lblInitializationHeader.Size = New System.Drawing.Size(265, 18)
            Me.lblInitializationHeader.TabIndex = 0
            Me.lblInitializationHeader.Text = "Parameters"
            Me.lblInitializationHeader.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'm_plBlocks
            '
            Me.m_plBlocks.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_plBlocks.Location = New System.Drawing.Point(271, 0)
            Me.m_plBlocks.Margin = New System.Windows.Forms.Padding(0)
            Me.m_plBlocks.Name = "m_plBlocks"
            Me.m_plBlocks.Size = New System.Drawing.Size(718, 377)
            Me.m_plBlocks.TabIndex = 2
            '
            'm_tcMain
            '
            Me.m_tcMain.Controls.Add(Me.m_tabObjectives)
            Me.m_tcMain.Controls.Add(Me.m_tabResultTable)
            Me.m_tcMain.Controls.Add(Me.m_tpPlots)
            Me.m_tcMain.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_tcMain.Location = New System.Drawing.Point(0, 0)
            Me.m_tcMain.Name = "m_tcMain"
            Me.m_tcMain.SelectedIndex = 0
            Me.m_tcMain.Size = New System.Drawing.Size(989, 298)
            Me.m_tcMain.TabIndex = 0
            '
            'm_tabObjectives
            '
            Me.m_tabObjectives.Controls.Add(Me.m_scObjectives)
            Me.m_tabObjectives.Location = New System.Drawing.Point(4, 22)
            Me.m_tabObjectives.Name = "m_tabObjectives"
            Me.m_tabObjectives.Padding = New System.Windows.Forms.Padding(3)
            Me.m_tabObjectives.Size = New System.Drawing.Size(981, 272)
            Me.m_tabObjectives.TabIndex = 0
            Me.m_tabObjectives.Text = "Objectives"
            Me.m_tabObjectives.UseVisualStyleBackColor = True
            '
            'm_scObjectives
            '
            Me.m_scObjectives.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_scObjectives.Location = New System.Drawing.Point(3, 3)
            Me.m_scObjectives.Name = "m_scObjectives"
            '
            'm_scObjectives.Panel2
            '
            Me.m_scObjectives.Panel2.Controls.Add(Me.m_scAarghArghAaargh)
            Me.m_scObjectives.Size = New System.Drawing.Size(975, 266)
            Me.m_scObjectives.SplitterDistance = 319
            Me.m_scObjectives.TabIndex = 0
            '
            'm_scAarghArghAaargh
            '
            Me.m_scAarghArghAaargh.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_scAarghArghAaargh.Location = New System.Drawing.Point(0, 0)
            Me.m_scAarghArghAaargh.Name = "m_scAarghArghAaargh"
            Me.m_scAarghArghAaargh.Size = New System.Drawing.Size(652, 266)
            Me.m_scAarghArghAaargh.SplitterDistance = 212
            Me.m_scAarghArghAaargh.TabIndex = 0
            '
            'm_tabResultTable
            '
            Me.m_tabResultTable.Controls.Add(Me.m_scIterResult)
            Me.m_tabResultTable.Location = New System.Drawing.Point(4, 22)
            Me.m_tabResultTable.Name = "m_tabResultTable"
            Me.m_tabResultTable.Padding = New System.Windows.Forms.Padding(3)
            Me.m_tabResultTable.Size = New System.Drawing.Size(981, 272)
            Me.m_tabResultTable.TabIndex = 1
            Me.m_tabResultTable.Text = "Iteration results"
            Me.m_tabResultTable.UseVisualStyleBackColor = True
            '
            'm_scIterResult
            '
            Me.m_scIterResult.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_scIterResult.Location = New System.Drawing.Point(3, 3)
            Me.m_scIterResult.Name = "m_scIterResult"
            '
            'm_scIterResult.Panel1
            '
            Me.m_scIterResult.Panel1.Controls.Add(Me.m_scIterResultMultiRun)
            Me.m_scIterResult.Size = New System.Drawing.Size(975, 266)
            Me.m_scIterResult.SplitterDistance = 541
            Me.m_scIterResult.TabIndex = 0
            '
            'm_scIterResultMultiRun
            '
            Me.m_scIterResultMultiRun.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_scIterResultMultiRun.Location = New System.Drawing.Point(0, 0)
            Me.m_scIterResultMultiRun.Name = "m_scIterResultMultiRun"
            Me.m_scIterResultMultiRun.Orientation = System.Windows.Forms.Orientation.Horizontal
            Me.m_scIterResultMultiRun.Size = New System.Drawing.Size(541, 266)
            Me.m_scIterResultMultiRun.SplitterDistance = 135
            Me.m_scIterResultMultiRun.TabIndex = 0
            '
            'm_tpPlots
            '
            Me.m_tpPlots.Controls.Add(Me.m_splcPlotResults)
            Me.m_tpPlots.Location = New System.Drawing.Point(4, 22)
            Me.m_tpPlots.Name = "m_tpPlots"
            Me.m_tpPlots.Padding = New System.Windows.Forms.Padding(3)
            Me.m_tpPlots.Size = New System.Drawing.Size(981, 272)
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
            Me.m_splcPlotResults.Size = New System.Drawing.Size(975, 266)
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
            Me.m_graphResults.Size = New System.Drawing.Size(975, 266)
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
            Me.m_zgcKiteDiag.Size = New System.Drawing.Size(96, 100)
            Me.m_zgcKiteDiag.TabIndex = 9
            '
            'frmFishingPolicySearch
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(993, 687)
            Me.Controls.Add(Me.m_scTopBits)
            Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.Name = "frmFishingPolicySearch"
            Me.TabText = "Fishing policy search"
            Me.Text = "Fishing policy search"
            Me.m_scTopBits.Panel1.ResumeLayout(False)
            Me.m_scTopBits.Panel2.ResumeLayout(False)
            Me.m_scTopBits.ResumeLayout(False)
            Me.m_tlpRunStop.ResumeLayout(False)
            Me.m_plRunParams.ResumeLayout(False)
            Me.m_plRunParams.PerformLayout()
            CType(Me.m_nudMaxEffChg, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.m_nudBaseYear, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.m_nudMaxNumEval, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.m_nudNumberOfRuns, System.ComponentModel.ISupportInitialize).EndInit()
            Me.m_tcMain.ResumeLayout(False)
            Me.m_tabObjectives.ResumeLayout(False)
            Me.m_scObjectives.Panel2.ResumeLayout(False)
            Me.m_scObjectives.ResumeLayout(False)
            Me.m_scAarghArghAaargh.ResumeLayout(False)
            Me.m_tabResultTable.ResumeLayout(False)
            Me.m_scIterResult.Panel1.ResumeLayout(False)
            Me.m_scIterResult.ResumeLayout(False)
            Me.m_scIterResultMultiRun.ResumeLayout(False)
            Me.m_tpPlots.ResumeLayout(False)
            Me.m_splcPlotResults.Panel1.ResumeLayout(False)
            Me.m_splcPlotResults.Panel2.ResumeLayout(False)
            Me.m_splcPlotResults.ResumeLayout(False)
            Me.ResumeLayout(False)

        End Sub
        Friend WithEvents m_scTopBits As System.Windows.Forms.SplitContainer
        Friend WithEvents lblInitializationHeader As System.Windows.Forms.Label
        Friend WithEvents m_lblSearchUsing As System.Windows.Forms.Label
        Friend WithEvents btnStop As System.Windows.Forms.Button
        Friend WithEvents btnSearch As System.Windows.Forms.Button
        Private WithEvents m_zgcKiteDiag As ZedGraph.ZedGraphControl
        Private WithEvents m_graphResults As ZedGraph.ZedGraphControl
        Friend WithEvents m_splcPlotResults As System.Windows.Forms.SplitContainer
        Private WithEvents m_tlpRunStop As System.Windows.Forms.TableLayoutPanel
        Private WithEvents m_lblOptmApproach As System.Windows.Forms.Label
        Private WithEvents m_lblInitUsing As System.Windows.Forms.Label
        Private WithEvents m_lblMaxEffChg As System.Windows.Forms.Label
        Private WithEvents m_lblBaseYear As System.Windows.Forms.Label
        Private WithEvents m_lblMaxNumEval As System.Windows.Forms.Label
        Private WithEvents m_lblNumOfRuns As System.Windows.Forms.Label
        Private WithEvents m_lblGenDiscRate As System.Windows.Forms.Label
        Private WithEvents m_lblDiscRate As System.Windows.Forms.Label
        Private WithEvents m_txtDiscountRate As System.Windows.Forms.TextBox
        Private WithEvents m_txtGenDiscRate As System.Windows.Forms.TextBox
        Private WithEvents m_nudNumberOfRuns As System.Windows.Forms.NumericUpDown
        Private WithEvents m_nudMaxNumEval As System.Windows.Forms.NumericUpDown
        Private WithEvents m_nudBaseYear As System.Windows.Forms.NumericUpDown
        Private WithEvents m_nudMaxEffChg As System.Windows.Forms.NumericUpDown
        Private WithEvents m_cmbInitUsing As System.Windows.Forms.ComboBox
        Private WithEvents m_cmbSearchUsing As System.Windows.Forms.ComboBox
        Private WithEvents m_cmbOptmApproach As System.Windows.Forms.ComboBox
        Private WithEvents m_chkUsePlugin As System.Windows.Forms.CheckBox
        Private WithEvents m_chkPrevCE As System.Windows.Forms.CheckBox
        Private WithEvents m_chkMaxPortUl As System.Windows.Forms.CheckBox
        Private WithEvents m_chkIncludeCCosts As System.Windows.Forms.CheckBox
        Private WithEvents m_plRunParams As System.Windows.Forms.Panel
        Private WithEvents m_plBlocks As System.Windows.Forms.Panel
        Private WithEvents m_tcMain As System.Windows.Forms.TabControl
        Private WithEvents m_tabObjectives As System.Windows.Forms.TabPage
        Private WithEvents m_scObjectives As System.Windows.Forms.SplitContainer
        Private WithEvents m_scAarghArghAaargh As System.Windows.Forms.SplitContainer
        Private WithEvents m_tabResultTable As System.Windows.Forms.TabPage
        Private WithEvents m_scIterResult As System.Windows.Forms.SplitContainer
        Private WithEvents m_scIterResultMultiRun As System.Windows.Forms.SplitContainer
        Private WithEvents m_tpPlots As System.Windows.Forms.TabPage

    End Class
End Namespace