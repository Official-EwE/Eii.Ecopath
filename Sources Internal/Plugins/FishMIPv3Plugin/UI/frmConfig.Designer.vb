Imports ScientificInterfaceShared.Forms
Imports System.Windows.Forms

Partial Class frmConfig
    Inherits frmEwEGrid

    'Form overrides dispose to clean up the component list.
    <Diagnostics.DebuggerNonUserCode()>
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
    <Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Dim sep2 As System.Windows.Forms.ToolStripSeparator
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmConfig))
        Dim sep0 As System.Windows.Forms.ToolStripSeparator
        Me.m_lblESM = New System.Windows.Forms.Label()
        Me.m_cmbESM = New System.Windows.Forms.ComboBox()
        Me.m_tlpMain = New System.Windows.Forms.TableLayoutPanel()
        Me.m_ts = New System.Windows.Forms.ToolStrip()
        Me.m_tsbnLoadProtocol = New System.Windows.Forms.ToolStripButton()
        Me.m_tslbProtocol = New System.Windows.Forms.ToolStripLabel()
        Me.m_tslbArea = New System.Windows.Forms.ToolStripLabel()
        Me.m_tscmbArea = New System.Windows.Forms.ToolStripComboBox()
        Me.m_tsbnCalculateScaling = New System.Windows.Forms.ToolStripButton()
        Me.m_tsbnSaveEcosim = New System.Windows.Forms.ToolStripButton()
        Me.m_tsbnSaveEcospace = New System.Windows.Forms.ToolStripButton()
        Me.m_tsbnSaveEcoInd = New System.Windows.Forms.ToolStripButton()
        Me.m_btnApply = New System.Windows.Forms.Button()
        Me.m_tcMain = New System.Windows.Forms.TabControl()
        Me.m_tabProtocol = New System.Windows.Forms.TabPage()
        Me.m_plProtocol = New System.Windows.Forms.Panel()
        Me.m_scConfig = New System.Windows.Forms.SplitContainer()
        Me.m_tlpDrivers = New System.Windows.Forms.TableLayoutPanel()
        Me.m_dgvFishing = New System.Windows.Forms.DataGridView()
        Me.DataGridViewTextBoxColumn1 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.DataGridViewTextBoxColumn2 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.DataGridViewTextBoxColumn3 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.m_dgvDriverScaling = New System.Windows.Forms.DataGridView()
        Me.m_colDSNo = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.m_colDSESM = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.m_colDSVar = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.m_colDSScaling = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.m_hdrIndex = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.m_hdrScaling = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.m_dgvDriversPos = New System.Windows.Forms.DataGridView()
        Me.m_colDINo = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.m_colDIDriver = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.m_colDIIndex = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.m_hdrFishing = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.m_tlpExperiment = New System.Windows.Forms.TableLayoutPanel()
        Me.m_hdrExperiments = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.m_dgvExperimentDrivers = New System.Windows.Forms.DataGridView()
        Me.m_colPeriod = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.m_colStartYear = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.m_colEnd = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.m_colGCM = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.m_colSoc = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.m_tlpConfig = New System.Windows.Forms.TableLayoutPanel()
        Me.m_lblExperiment = New System.Windows.Forms.Label()
        Me.m_cmbExperiment = New System.Windows.Forms.ComboBox()
        Me.m_tabOutput = New System.Windows.Forms.TabPage()
        Me.m_scOutput = New System.Windows.Forms.SplitContainer()
        Me.m_dgvIndicators = New System.Windows.Forms.DataGridView()
        Me.m_colIndicatorEnabled = New System.Windows.Forms.DataGridViewCheckBoxColumn()
        Me.m_colIndicator = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.m_colEcoINDAbbr = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.m_tsOutput = New System.Windows.Forms.ToolStrip()
        Me.m_tslbFIshMIP = New System.Windows.Forms.ToolStripLabel()
        Me.m_tsbnEcoOceanDefaults = New System.Windows.Forms.ToolStripButton()
        Me.m_tsbnTaxonDefaults = New System.Windows.Forms.ToolStripButton()
        Me.m_tsspOutput = New System.Windows.Forms.ToolStripSeparator()
        Me.ToolStripLabel1 = New System.Windows.Forms.ToolStripLabel()
        Me.m_tsbnEcoIndTriatlas = New System.Windows.Forms.ToolStripButton()
        Me.m_tsbnEcoIndNone = New System.Windows.Forms.ToolStripButton()
        Me.m_tsbnEcoIndAll = New System.Windows.Forms.ToolStripButton()
        Me.m_tlpCredits = New System.Windows.Forms.TableLayoutPanel()
        Me.m_pbTriatlas = New System.Windows.Forms.PictureBox()
        Me.m_pbFishMIP = New System.Windows.Forms.PictureBox()
        Me.m_grid = New FishMIPv3Plugin.gridConfig()
        sep2 = New System.Windows.Forms.ToolStripSeparator()
        sep0 = New System.Windows.Forms.ToolStripSeparator()
        Me.m_tlpMain.SuspendLayout()
        Me.m_ts.SuspendLayout()
        Me.m_tcMain.SuspendLayout()
        Me.m_tabProtocol.SuspendLayout()
        Me.m_plProtocol.SuspendLayout()
        CType(Me.m_scConfig, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.m_scConfig.Panel1.SuspendLayout()
        Me.m_scConfig.Panel2.SuspendLayout()
        Me.m_scConfig.SuspendLayout()
        Me.m_tlpDrivers.SuspendLayout()
        CType(Me.m_dgvFishing, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.m_dgvDriverScaling, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.m_dgvDriversPos, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.m_tlpExperiment.SuspendLayout()
        CType(Me.m_dgvExperimentDrivers, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.m_tlpConfig.SuspendLayout()
        Me.m_tabOutput.SuspendLayout()
        CType(Me.m_scOutput, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.m_scOutput.Panel1.SuspendLayout()
        Me.m_scOutput.Panel2.SuspendLayout()
        Me.m_scOutput.SuspendLayout()
        CType(Me.m_dgvIndicators, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.m_tsOutput.SuspendLayout()
        Me.m_tlpCredits.SuspendLayout()
        CType(Me.m_pbTriatlas, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.m_pbFishMIP, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'sep2
        '
        sep2.Name = "sep2"
        resources.ApplyResources(sep2, "sep2")
        '
        'sep0
        '
        sep0.Name = "sep0"
        resources.ApplyResources(sep0, "sep0")
        '
        'm_lblESM
        '
        resources.ApplyResources(Me.m_lblESM, "m_lblESM")
        Me.m_lblESM.Name = "m_lblESM"
        '
        'm_cmbESM
        '
        resources.ApplyResources(Me.m_cmbESM, "m_cmbESM")
        Me.m_cmbESM.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.m_cmbESM.FormattingEnabled = True
        Me.m_cmbESM.Name = "m_cmbESM"
        '
        'm_tlpMain
        '
        resources.ApplyResources(Me.m_tlpMain, "m_tlpMain")
        Me.m_tlpMain.Controls.Add(Me.m_ts, 0, 0)
        Me.m_tlpMain.Controls.Add(Me.m_btnApply, 0, 2)
        Me.m_tlpMain.Controls.Add(Me.m_tcMain, 0, 1)
        Me.m_tlpMain.Controls.Add(Me.m_tlpCredits, 0, 3)
        Me.m_tlpMain.Name = "m_tlpMain"
        '
        'm_ts
        '
        resources.ApplyResources(Me.m_ts, "m_ts")
        Me.m_ts.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
        Me.m_ts.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tsbnLoadProtocol, Me.m_tslbProtocol, sep0, Me.m_tslbArea, Me.m_tscmbArea, Me.m_tsbnCalculateScaling, sep2, Me.m_tsbnSaveEcosim, Me.m_tsbnSaveEcospace, Me.m_tsbnSaveEcoInd})
        Me.m_ts.Name = "m_ts"
        Me.m_ts.RenderMode = System.Windows.Forms.ToolStripRenderMode.System
        '
        'm_tsbnLoadProtocol
        '
        resources.ApplyResources(Me.m_tsbnLoadProtocol, "m_tsbnLoadProtocol")
        Me.m_tsbnLoadProtocol.Name = "m_tsbnLoadProtocol"
        '
        'm_tslbProtocol
        '
        Me.m_tslbProtocol.Name = "m_tslbProtocol"
        resources.ApplyResources(Me.m_tslbProtocol, "m_tslbProtocol")
        '
        'm_tslbArea
        '
        Me.m_tslbArea.Name = "m_tslbArea"
        resources.ApplyResources(Me.m_tslbArea, "m_tslbArea")
        '
        'm_tscmbArea
        '
        Me.m_tscmbArea.Name = "m_tscmbArea"
        resources.ApplyResources(Me.m_tscmbArea, "m_tscmbArea")
        '
        'm_tsbnCalculateScaling
        '
        Me.m_tsbnCalculateScaling.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        resources.ApplyResources(Me.m_tsbnCalculateScaling, "m_tsbnCalculateScaling")
        Me.m_tsbnCalculateScaling.Name = "m_tsbnCalculateScaling"
        '
        'm_tsbnSaveEcosim
        '
        Me.m_tsbnSaveEcosim.CheckOnClick = True
        resources.ApplyResources(Me.m_tsbnSaveEcosim, "m_tsbnSaveEcosim")
        Me.m_tsbnSaveEcosim.Name = "m_tsbnSaveEcosim"
        '
        'm_tsbnSaveEcospace
        '
        Me.m_tsbnSaveEcospace.CheckOnClick = True
        resources.ApplyResources(Me.m_tsbnSaveEcospace, "m_tsbnSaveEcospace")
        Me.m_tsbnSaveEcospace.Name = "m_tsbnSaveEcospace"
        '
        'm_tsbnSaveEcoInd
        '
        Me.m_tsbnSaveEcoInd.CheckOnClick = True
        resources.ApplyResources(Me.m_tsbnSaveEcoInd, "m_tsbnSaveEcoInd")
        Me.m_tsbnSaveEcoInd.Name = "m_tsbnSaveEcoInd"
        '
        'm_btnApply
        '
        resources.ApplyResources(Me.m_btnApply, "m_btnApply")
        Me.m_btnApply.Name = "m_btnApply"
        Me.m_btnApply.UseVisualStyleBackColor = True
        '
        'm_tcMain
        '
        Me.m_tcMain.Controls.Add(Me.m_tabProtocol)
        Me.m_tcMain.Controls.Add(Me.m_tabOutput)
        resources.ApplyResources(Me.m_tcMain, "m_tcMain")
        Me.m_tcMain.Name = "m_tcMain"
        Me.m_tcMain.SelectedIndex = 0
        '
        'm_tabProtocol
        '
        Me.m_tabProtocol.Controls.Add(Me.m_plProtocol)
        resources.ApplyResources(Me.m_tabProtocol, "m_tabProtocol")
        Me.m_tabProtocol.Name = "m_tabProtocol"
        Me.m_tabProtocol.UseVisualStyleBackColor = True
        '
        'm_plProtocol
        '
        Me.m_plProtocol.Controls.Add(Me.m_scConfig)
        resources.ApplyResources(Me.m_plProtocol, "m_plProtocol")
        Me.m_plProtocol.Name = "m_plProtocol"
        '
        'm_scConfig
        '
        resources.ApplyResources(Me.m_scConfig, "m_scConfig")
        Me.m_scConfig.Name = "m_scConfig"
        '
        'm_scConfig.Panel1
        '
        Me.m_scConfig.Panel1.Controls.Add(Me.m_tlpDrivers)
        '
        'm_scConfig.Panel2
        '
        Me.m_scConfig.Panel2.Controls.Add(Me.m_tlpExperiment)
        '
        'm_tlpDrivers
        '
        resources.ApplyResources(Me.m_tlpDrivers, "m_tlpDrivers")
        Me.m_tlpDrivers.Controls.Add(Me.m_dgvFishing, 2, 1)
        Me.m_tlpDrivers.Controls.Add(Me.m_dgvDriverScaling, 1, 1)
        Me.m_tlpDrivers.Controls.Add(Me.m_hdrIndex, 0, 0)
        Me.m_tlpDrivers.Controls.Add(Me.m_hdrScaling, 1, 0)
        Me.m_tlpDrivers.Controls.Add(Me.m_dgvDriversPos, 0, 1)
        Me.m_tlpDrivers.Controls.Add(Me.m_hdrFishing, 2, 0)
        Me.m_tlpDrivers.Name = "m_tlpDrivers"
        '
        'm_dgvFishing
        '
        Me.m_dgvFishing.AllowUserToAddRows = False
        Me.m_dgvFishing.AllowUserToDeleteRows = False
        Me.m_dgvFishing.AllowUserToResizeRows = False
        Me.m_dgvFishing.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.m_dgvFishing.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.DataGridViewTextBoxColumn1, Me.DataGridViewTextBoxColumn2, Me.DataGridViewTextBoxColumn3})
        resources.ApplyResources(Me.m_dgvFishing, "m_dgvFishing")
        Me.m_dgvFishing.MultiSelect = False
        Me.m_dgvFishing.Name = "m_dgvFishing"
        Me.m_dgvFishing.RowHeadersVisible = False
        Me.m_dgvFishing.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        '
        'DataGridViewTextBoxColumn1
        '
        Me.DataGridViewTextBoxColumn1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader
        Me.DataGridViewTextBoxColumn1.Frozen = True
        resources.ApplyResources(Me.DataGridViewTextBoxColumn1, "DataGridViewTextBoxColumn1")
        Me.DataGridViewTextBoxColumn1.Name = "DataGridViewTextBoxColumn1"
        Me.DataGridViewTextBoxColumn1.ReadOnly = True
        '
        'DataGridViewTextBoxColumn2
        '
        Me.DataGridViewTextBoxColumn2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells
        Me.DataGridViewTextBoxColumn2.Frozen = True
        resources.ApplyResources(Me.DataGridViewTextBoxColumn2, "DataGridViewTextBoxColumn2")
        Me.DataGridViewTextBoxColumn2.Name = "DataGridViewTextBoxColumn2"
        Me.DataGridViewTextBoxColumn2.ReadOnly = True
        '
        'DataGridViewTextBoxColumn3
        '
        Me.DataGridViewTextBoxColumn3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        resources.ApplyResources(Me.DataGridViewTextBoxColumn3, "DataGridViewTextBoxColumn3")
        Me.DataGridViewTextBoxColumn3.Name = "DataGridViewTextBoxColumn3"
        Me.DataGridViewTextBoxColumn3.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        '
        'm_dgvDriverScaling
        '
        Me.m_dgvDriverScaling.AllowUserToAddRows = False
        Me.m_dgvDriverScaling.AllowUserToDeleteRows = False
        Me.m_dgvDriverScaling.AllowUserToResizeRows = False
        Me.m_dgvDriverScaling.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.m_dgvDriverScaling.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.m_colDSNo, Me.m_colDSESM, Me.m_colDSVar, Me.m_colDSScaling})
        resources.ApplyResources(Me.m_dgvDriverScaling, "m_dgvDriverScaling")
        Me.m_dgvDriverScaling.MultiSelect = False
        Me.m_dgvDriverScaling.Name = "m_dgvDriverScaling"
        Me.m_dgvDriverScaling.RowHeadersVisible = False
        Me.m_dgvDriverScaling.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        '
        'm_colDSNo
        '
        Me.m_colDSNo.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader
        Me.m_colDSNo.Frozen = True
        resources.ApplyResources(Me.m_colDSNo, "m_colDSNo")
        Me.m_colDSNo.Name = "m_colDSNo"
        Me.m_colDSNo.ReadOnly = True
        '
        'm_colDSESM
        '
        Me.m_colDSESM.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells
        Me.m_colDSESM.Frozen = True
        resources.ApplyResources(Me.m_colDSESM, "m_colDSESM")
        Me.m_colDSESM.Name = "m_colDSESM"
        Me.m_colDSESM.ReadOnly = True
        '
        'm_colDSVar
        '
        Me.m_colDSVar.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells
        resources.ApplyResources(Me.m_colDSVar, "m_colDSVar")
        Me.m_colDSVar.Name = "m_colDSVar"
        Me.m_colDSVar.ReadOnly = True
        '
        'm_colDSScaling
        '
        Me.m_colDSScaling.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        resources.ApplyResources(Me.m_colDSScaling, "m_colDSScaling")
        Me.m_colDSScaling.Name = "m_colDSScaling"
        '
        'm_hdrIndex
        '
        Me.m_hdrIndex.CanCollapseParent = False
        Me.m_hdrIndex.CollapsedParentHeight = 0
        resources.ApplyResources(Me.m_hdrIndex, "m_hdrIndex")
        Me.m_hdrIndex.IsCollapsed = False
        Me.m_hdrIndex.Name = "m_hdrIndex"
        '
        'm_hdrScaling
        '
        Me.m_hdrScaling.CanCollapseParent = False
        Me.m_hdrScaling.CollapsedParentHeight = 0
        resources.ApplyResources(Me.m_hdrScaling, "m_hdrScaling")
        Me.m_hdrScaling.IsCollapsed = False
        Me.m_hdrScaling.Name = "m_hdrScaling"
        '
        'm_dgvDriversPos
        '
        Me.m_dgvDriversPos.AllowUserToAddRows = False
        Me.m_dgvDriversPos.AllowUserToDeleteRows = False
        Me.m_dgvDriversPos.AllowUserToResizeRows = False
        Me.m_dgvDriversPos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.m_dgvDriversPos.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.m_colDINo, Me.m_colDIDriver, Me.m_colDIIndex})
        resources.ApplyResources(Me.m_dgvDriversPos, "m_dgvDriversPos")
        Me.m_dgvDriversPos.MultiSelect = False
        Me.m_dgvDriversPos.Name = "m_dgvDriversPos"
        Me.m_dgvDriversPos.RowHeadersVisible = False
        Me.m_dgvDriversPos.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        '
        'm_colDINo
        '
        Me.m_colDINo.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader
        resources.ApplyResources(Me.m_colDINo, "m_colDINo")
        Me.m_colDINo.Name = "m_colDINo"
        Me.m_colDINo.ReadOnly = True
        '
        'm_colDIDriver
        '
        Me.m_colDIDriver.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells
        resources.ApplyResources(Me.m_colDIDriver, "m_colDIDriver")
        Me.m_colDIDriver.Name = "m_colDIDriver"
        Me.m_colDIDriver.ReadOnly = True
        '
        'm_colDIIndex
        '
        Me.m_colDIIndex.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        resources.ApplyResources(Me.m_colDIIndex, "m_colDIIndex")
        Me.m_colDIIndex.Name = "m_colDIIndex"
        Me.m_colDIIndex.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        '
        'm_hdrFishing
        '
        Me.m_hdrFishing.CanCollapseParent = False
        Me.m_hdrFishing.CollapsedParentHeight = 0
        resources.ApplyResources(Me.m_hdrFishing, "m_hdrFishing")
        Me.m_hdrFishing.IsCollapsed = False
        Me.m_hdrFishing.Name = "m_hdrFishing"
        '
        'm_tlpExperiment
        '
        resources.ApplyResources(Me.m_tlpExperiment, "m_tlpExperiment")
        Me.m_tlpExperiment.Controls.Add(Me.m_hdrExperiments, 0, 0)
        Me.m_tlpExperiment.Controls.Add(Me.m_dgvExperimentDrivers, 0, 2)
        Me.m_tlpExperiment.Controls.Add(Me.m_tlpConfig, 0, 1)
        Me.m_tlpExperiment.Name = "m_tlpExperiment"
        '
        'm_hdrExperiments
        '
        Me.m_hdrExperiments.CanCollapseParent = False
        Me.m_hdrExperiments.CollapsedParentHeight = 0
        resources.ApplyResources(Me.m_hdrExperiments, "m_hdrExperiments")
        Me.m_hdrExperiments.IsCollapsed = False
        Me.m_hdrExperiments.Name = "m_hdrExperiments"
        '
        'm_dgvExperimentDrivers
        '
        Me.m_dgvExperimentDrivers.AllowUserToAddRows = False
        Me.m_dgvExperimentDrivers.AllowUserToDeleteRows = False
        Me.m_dgvExperimentDrivers.AllowUserToResizeRows = False
        Me.m_dgvExperimentDrivers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.m_dgvExperimentDrivers.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.m_colPeriod, Me.m_colStartYear, Me.m_colEnd, Me.m_colGCM, Me.m_colSoc})
        resources.ApplyResources(Me.m_dgvExperimentDrivers, "m_dgvExperimentDrivers")
        Me.m_dgvExperimentDrivers.MultiSelect = False
        Me.m_dgvExperimentDrivers.Name = "m_dgvExperimentDrivers"
        Me.m_dgvExperimentDrivers.RowHeadersVisible = False
        Me.m_dgvExperimentDrivers.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        '
        'm_colPeriod
        '
        Me.m_colPeriod.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCellsExceptHeader
        resources.ApplyResources(Me.m_colPeriod, "m_colPeriod")
        Me.m_colPeriod.Name = "m_colPeriod"
        Me.m_colPeriod.ReadOnly = True
        '
        'm_colStartYear
        '
        Me.m_colStartYear.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None
        resources.ApplyResources(Me.m_colStartYear, "m_colStartYear")
        Me.m_colStartYear.Name = "m_colStartYear"
        Me.m_colStartYear.ReadOnly = True
        '
        'm_colEnd
        '
        Me.m_colEnd.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None
        resources.ApplyResources(Me.m_colEnd, "m_colEnd")
        Me.m_colEnd.Name = "m_colEnd"
        Me.m_colEnd.ReadOnly = True
        '
        'm_colGCM
        '
        Me.m_colGCM.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        Me.m_colGCM.FillWeight = 50.0!
        resources.ApplyResources(Me.m_colGCM, "m_colGCM")
        Me.m_colGCM.Name = "m_colGCM"
        Me.m_colGCM.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.m_colGCM.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        '
        'm_colSoc
        '
        Me.m_colSoc.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        Me.m_colSoc.FillWeight = 50.0!
        resources.ApplyResources(Me.m_colSoc, "m_colSoc")
        Me.m_colSoc.Name = "m_colSoc"
        Me.m_colSoc.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.m_colSoc.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        '
        'm_tlpConfig
        '
        resources.ApplyResources(Me.m_tlpConfig, "m_tlpConfig")
        Me.m_tlpConfig.Controls.Add(Me.m_lblExperiment, 0, 0)
        Me.m_tlpConfig.Controls.Add(Me.m_cmbExperiment, 1, 0)
        Me.m_tlpConfig.Controls.Add(Me.m_lblESM, 3, 0)
        Me.m_tlpConfig.Controls.Add(Me.m_cmbESM, 4, 0)
        Me.m_tlpConfig.Name = "m_tlpConfig"
        '
        'm_lblExperiment
        '
        resources.ApplyResources(Me.m_lblExperiment, "m_lblExperiment")
        Me.m_lblExperiment.Name = "m_lblExperiment"
        '
        'm_cmbExperiment
        '
        resources.ApplyResources(Me.m_cmbExperiment, "m_cmbExperiment")
        Me.m_cmbExperiment.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.m_cmbExperiment.FormattingEnabled = True
        Me.m_cmbExperiment.Items.AddRange(New Object() {resources.GetString("m_cmbExperiment.Items"), resources.GetString("m_cmbExperiment.Items1"), resources.GetString("m_cmbExperiment.Items2"), resources.GetString("m_cmbExperiment.Items3"), resources.GetString("m_cmbExperiment.Items4"), resources.GetString("m_cmbExperiment.Items5"), resources.GetString("m_cmbExperiment.Items6"), resources.GetString("m_cmbExperiment.Items7")})
        Me.m_cmbExperiment.Name = "m_cmbExperiment"
        '
        'm_tabOutput
        '
        Me.m_tabOutput.Controls.Add(Me.m_scOutput)
        Me.m_tabOutput.Controls.Add(Me.m_tsOutput)
        resources.ApplyResources(Me.m_tabOutput, "m_tabOutput")
        Me.m_tabOutput.Name = "m_tabOutput"
        Me.m_tabOutput.UseVisualStyleBackColor = True
        '
        'm_scOutput
        '
        resources.ApplyResources(Me.m_scOutput, "m_scOutput")
        Me.m_scOutput.Name = "m_scOutput"
        '
        'm_scOutput.Panel1
        '
        Me.m_scOutput.Panel1.Controls.Add(Me.m_grid)
        '
        'm_scOutput.Panel2
        '
        Me.m_scOutput.Panel2.Controls.Add(Me.m_dgvIndicators)
        '
        'm_dgvIndicators
        '
        Me.m_dgvIndicators.AllowUserToAddRows = False
        Me.m_dgvIndicators.AllowUserToDeleteRows = False
        Me.m_dgvIndicators.AllowUserToResizeRows = False
        Me.m_dgvIndicators.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.m_dgvIndicators.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.m_colIndicatorEnabled, Me.m_colIndicator, Me.m_colEcoINDAbbr})
        resources.ApplyResources(Me.m_dgvIndicators, "m_dgvIndicators")
        Me.m_dgvIndicators.MultiSelect = False
        Me.m_dgvIndicators.Name = "m_dgvIndicators"
        Me.m_dgvIndicators.RowHeadersVisible = False
        Me.m_dgvIndicators.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        '
        'm_colIndicatorEnabled
        '
        Me.m_colIndicatorEnabled.Frozen = True
        resources.ApplyResources(Me.m_colIndicatorEnabled, "m_colIndicatorEnabled")
        Me.m_colIndicatorEnabled.Name = "m_colIndicatorEnabled"
        Me.m_colIndicatorEnabled.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.m_colIndicatorEnabled.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic
        '
        'm_colIndicator
        '
        Me.m_colIndicator.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        resources.ApplyResources(Me.m_colIndicator, "m_colIndicator")
        Me.m_colIndicator.Name = "m_colIndicator"
        Me.m_colIndicator.ReadOnly = True
        '
        'm_colEcoINDAbbr
        '
        Me.m_colEcoINDAbbr.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader
        resources.ApplyResources(Me.m_colEcoINDAbbr, "m_colEcoINDAbbr")
        Me.m_colEcoINDAbbr.Name = "m_colEcoINDAbbr"
        Me.m_colEcoINDAbbr.ReadOnly = True
        '
        'm_tsOutput
        '
        Me.m_tsOutput.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tslbFIshMIP, Me.m_tsbnEcoOceanDefaults, Me.m_tsbnTaxonDefaults, Me.m_tsspOutput, Me.ToolStripLabel1, Me.m_tsbnEcoIndTriatlas, Me.m_tsbnEcoIndNone, Me.m_tsbnEcoIndAll})
        resources.ApplyResources(Me.m_tsOutput, "m_tsOutput")
        Me.m_tsOutput.Name = "m_tsOutput"
        '
        'm_tslbFIshMIP
        '
        Me.m_tslbFIshMIP.Name = "m_tslbFIshMIP"
        resources.ApplyResources(Me.m_tslbFIshMIP, "m_tslbFIshMIP")
        '
        'm_tsbnEcoOceanDefaults
        '
        Me.m_tsbnEcoOceanDefaults.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        resources.ApplyResources(Me.m_tsbnEcoOceanDefaults, "m_tsbnEcoOceanDefaults")
        Me.m_tsbnEcoOceanDefaults.Name = "m_tsbnEcoOceanDefaults"
        '
        'm_tsbnTaxonDefaults
        '
        Me.m_tsbnTaxonDefaults.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        resources.ApplyResources(Me.m_tsbnTaxonDefaults, "m_tsbnTaxonDefaults")
        Me.m_tsbnTaxonDefaults.Name = "m_tsbnTaxonDefaults"
        '
        'm_tsspOutput
        '
        Me.m_tsspOutput.Name = "m_tsspOutput"
        resources.ApplyResources(Me.m_tsspOutput, "m_tsspOutput")
        '
        'ToolStripLabel1
        '
        Me.ToolStripLabel1.Name = "ToolStripLabel1"
        resources.ApplyResources(Me.ToolStripLabel1, "ToolStripLabel1")
        '
        'm_tsbnEcoIndTriatlas
        '
        resources.ApplyResources(Me.m_tsbnEcoIndTriatlas, "m_tsbnEcoIndTriatlas")
        Me.m_tsbnEcoIndTriatlas.Name = "m_tsbnEcoIndTriatlas"
        '
        'm_tsbnEcoIndNone
        '
        Me.m_tsbnEcoIndNone.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        resources.ApplyResources(Me.m_tsbnEcoIndNone, "m_tsbnEcoIndNone")
        Me.m_tsbnEcoIndNone.Name = "m_tsbnEcoIndNone"
        '
        'm_tsbnEcoIndAll
        '
        Me.m_tsbnEcoIndAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        resources.ApplyResources(Me.m_tsbnEcoIndAll, "m_tsbnEcoIndAll")
        Me.m_tsbnEcoIndAll.Name = "m_tsbnEcoIndAll"
        '
        'm_tlpCredits
        '
        Me.m_tlpCredits.BackColor = System.Drawing.Color.White
        resources.ApplyResources(Me.m_tlpCredits, "m_tlpCredits")
        Me.m_tlpCredits.Controls.Add(Me.m_pbTriatlas, 3, 0)
        Me.m_tlpCredits.Controls.Add(Me.m_pbFishMIP, 1, 0)
        Me.m_tlpCredits.Name = "m_tlpCredits"
        '
        'm_pbTriatlas
        '
        Me.m_pbTriatlas.BackColor = System.Drawing.Color.White
        Me.m_pbTriatlas.BackgroundImage = Global.FishMIPv3Plugin.My.Resources.Resources.triatlas
        resources.ApplyResources(Me.m_pbTriatlas, "m_pbTriatlas")
        Me.m_pbTriatlas.Name = "m_pbTriatlas"
        Me.m_pbTriatlas.TabStop = False
        '
        'm_pbFishMIP
        '
        Me.m_pbFishMIP.BackgroundImage = Global.FishMIPv3Plugin.My.Resources.Resources.FishMIP_logo_plain
        resources.ApplyResources(Me.m_pbFishMIP, "m_pbFishMIP")
        Me.m_pbFishMIP.Name = "m_pbFishMIP"
        Me.m_pbFishMIP.TabStop = False
        '
        'm_grid
        '
        Me.m_grid.AllowBlockSelect = True
        Me.m_grid.AutoSizeMinHeight = 10
        Me.m_grid.AutoSizeMinWidth = 10
        Me.m_grid.AutoStretchColumnsToFitWidth = False
        Me.m_grid.AutoStretchRowsToFitHeight = False
        Me.m_grid.BackColor = System.Drawing.Color.White
        Me.m_grid.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.m_grid.Configuration = Nothing
        Me.m_grid.ContextMenuStyle = CType((((SourceGrid2.ContextMenuStyle.ColumnResize Or SourceGrid2.ContextMenuStyle.AutoSize) _
            Or SourceGrid2.ContextMenuStyle.CopyPasteSelection) _
            Or SourceGrid2.ContextMenuStyle.CellContextMenu), SourceGrid2.ContextMenuStyle)
        Me.m_grid.CustomSort = False
        Me.m_grid.DataName = "grid content"
        resources.ApplyResources(Me.m_grid, "m_grid")
        Me.m_grid.FixedColumnWidths = False
        Me.m_grid.FocusStyle = SourceGrid2.FocusStyle.None
        Me.m_grid.GridToolTipActive = True
        Me.m_grid.IsLayoutSuspended = False
        Me.m_grid.IsOutputGrid = False
        Me.m_grid.Name = "m_grid"
        Me.m_grid.SpecialKeys = CType((((((((((SourceGrid2.GridSpecialKeys.Ctrl_C Or SourceGrid2.GridSpecialKeys.Ctrl_V) _
            Or SourceGrid2.GridSpecialKeys.Ctrl_X) _
            Or SourceGrid2.GridSpecialKeys.Delete) _
            Or SourceGrid2.GridSpecialKeys.Arrows) _
            Or SourceGrid2.GridSpecialKeys.Tab) _
            Or SourceGrid2.GridSpecialKeys.PageDownUp) _
            Or SourceGrid2.GridSpecialKeys.Enter) _
            Or SourceGrid2.GridSpecialKeys.Escape) _
            Or SourceGrid2.GridSpecialKeys.Backspace), SourceGrid2.GridSpecialKeys)
        Me.m_grid.UIContext = Nothing
        '
        'frmConfig
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.Controls.Add(Me.m_tlpMain)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmConfig"
        Me.ShowInTaskbar = False
        Me.TabText = ""
        Me.m_tlpMain.ResumeLayout(False)
        Me.m_tlpMain.PerformLayout()
        Me.m_ts.ResumeLayout(False)
        Me.m_ts.PerformLayout()
        Me.m_tcMain.ResumeLayout(False)
        Me.m_tabProtocol.ResumeLayout(False)
        Me.m_plProtocol.ResumeLayout(False)
        Me.m_scConfig.Panel1.ResumeLayout(False)
        Me.m_scConfig.Panel2.ResumeLayout(False)
        CType(Me.m_scConfig, System.ComponentModel.ISupportInitialize).EndInit()
        Me.m_scConfig.ResumeLayout(False)
        Me.m_tlpDrivers.ResumeLayout(False)
        CType(Me.m_dgvFishing, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.m_dgvDriverScaling, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.m_dgvDriversPos, System.ComponentModel.ISupportInitialize).EndInit()
        Me.m_tlpExperiment.ResumeLayout(False)
        CType(Me.m_dgvExperimentDrivers, System.ComponentModel.ISupportInitialize).EndInit()
        Me.m_tlpConfig.ResumeLayout(False)
        Me.m_tabOutput.ResumeLayout(False)
        Me.m_tabOutput.PerformLayout()
        Me.m_scOutput.Panel1.ResumeLayout(False)
        Me.m_scOutput.Panel2.ResumeLayout(False)
        CType(Me.m_scOutput, System.ComponentModel.ISupportInitialize).EndInit()
        Me.m_scOutput.ResumeLayout(False)
        CType(Me.m_dgvIndicators, System.ComponentModel.ISupportInitialize).EndInit()
        Me.m_tsOutput.ResumeLayout(False)
        Me.m_tsOutput.PerformLayout()
        Me.m_tlpCredits.ResumeLayout(False)
        CType(Me.m_pbTriatlas, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.m_pbFishMIP, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Private WithEvents m_lblESM As Label
    Private WithEvents m_cmbESM As ComboBox
    Private WithEvents m_tlpMain As TableLayoutPanel
    Private WithEvents m_ts As ToolStrip
    Private WithEvents m_tlpConfig As TableLayoutPanel
    Private WithEvents m_tsbnSaveEcosim As ToolStripButton
    Private WithEvents m_tsbnSaveEcospace As ToolStripButton
    Private WithEvents m_lblExperiment As Label
    Private WithEvents m_cmbExperiment As ComboBox
    Private WithEvents m_btnApply As Button
    Private WithEvents m_dgvDriversPos As DataGridView
    Private WithEvents m_dgvDriverScaling As DataGridView
    Private WithEvents m_tslbArea As ToolStripLabel
    Private WithEvents m_tscmbArea As ToolStripComboBox
    Private WithEvents m_hdrIndex As ScientificInterfaceShared.Controls.cEwEHeaderLabel
    Private WithEvents m_hdrScaling As ScientificInterfaceShared.Controls.cEwEHeaderLabel
    Private WithEvents m_tsbnSaveEcoInd As ToolStripButton
    Private WithEvents m_pbTriatlas As PictureBox
    Private WithEvents m_tlpDrivers As TableLayoutPanel
    Private WithEvents m_tsbnLoadProtocol As ToolStripButton
    Private WithEvents m_tslbProtocol As ToolStripLabel
    Private WithEvents m_dgvExperimentDrivers As DataGridView
    Private WithEvents m_hdrExperiments As ScientificInterfaceShared.Controls.cEwEHeaderLabel
    Private WithEvents m_colPeriod As DataGridViewTextBoxColumn
    Private WithEvents m_colStartYear As DataGridViewTextBoxColumn
    Private WithEvents m_colEnd As DataGridViewTextBoxColumn
    Private WithEvents m_colGCM As DataGridViewTextBoxColumn
    Private WithEvents m_colSoc As DataGridViewTextBoxColumn
    Private WithEvents m_tcMain As TabControl
    Private WithEvents m_tabProtocol As TabPage
    Private WithEvents m_plProtocol As Panel
    Private WithEvents m_tabOutput As TabPage
    Private WithEvents m_grid As gridConfig
    Private WithEvents m_dgvIndicators As DataGridView
    Private WithEvents m_tsOutput As ToolStrip
    Private WithEvents m_tslbFIshMIP As ToolStripLabel
    Private WithEvents m_tsbnEcoOceanDefaults As ToolStripButton
    Private WithEvents m_tsbnTaxonDefaults As ToolStripButton
    Private WithEvents m_tsspOutput As ToolStripSeparator
    Private WithEvents ToolStripLabel1 As ToolStripLabel
    Private WithEvents m_tsbnEcoIndNone As ToolStripButton
    Private WithEvents m_tsbnEcoIndAll As ToolStripButton
    Private WithEvents m_tsbnEcoIndTriatlas As ToolStripButton
    Private WithEvents m_scOutput As SplitContainer
    Private WithEvents m_colIndicatorEnabled As DataGridViewCheckBoxColumn
    Private WithEvents m_colIndicator As DataGridViewTextBoxColumn
    Private WithEvents m_colEcoINDAbbr As DataGridViewTextBoxColumn
    Private WithEvents m_dgvFishing As DataGridView
    Private WithEvents m_hdrFishing As ScientificInterfaceShared.Controls.cEwEHeaderLabel
    Private WithEvents m_colDSNo As DataGridViewTextBoxColumn
    Private WithEvents m_colDSESM As DataGridViewTextBoxColumn
    Private WithEvents m_colDSVar As DataGridViewTextBoxColumn
    Private WithEvents m_colDSScaling As DataGridViewTextBoxColumn
    Private WithEvents m_colDINo As DataGridViewTextBoxColumn
    Private WithEvents m_colDIDriver As DataGridViewTextBoxColumn
    Private WithEvents m_colDIIndex As DataGridViewTextBoxColumn
    Private WithEvents DataGridViewTextBoxColumn1 As DataGridViewTextBoxColumn
    Private WithEvents DataGridViewTextBoxColumn2 As DataGridViewTextBoxColumn
    Private WithEvents DataGridViewTextBoxColumn3 As DataGridViewTextBoxColumn
    Private WithEvents m_scConfig As SplitContainer
    Private WithEvents m_tlpExperiment As TableLayoutPanel
    Private WithEvents m_tsbnCalculateScaling As ToolStripButton
    Private WithEvents m_tlpCredits As TableLayoutPanel
    Private WithEvents m_pbFishMIP As PictureBox
End Class
