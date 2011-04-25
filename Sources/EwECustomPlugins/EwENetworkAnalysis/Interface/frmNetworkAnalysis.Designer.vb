Imports WeifenLuo.WinFormsUI.Docking
Imports ScientificInterfaceShared
Imports ScientificInterfaceShared.Controls

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
        Dim ts1 As System.Windows.Forms.ToolStripSeparator
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmNetworkAnalysis))
        Dim ts2 As System.Windows.Forms.ToolStripSeparator
        Dim ts3 As System.Windows.Forms.ToolStripSeparator
        Me.m_scNetworkAnalysis = New System.Windows.Forms.SplitContainer
        Me.m_tvNetworkAnalysis = New System.Windows.Forms.TreeView
        Me.m_scMain = New System.Windows.Forms.SplitContainer
        Me.m_tlpInfo = New System.Windows.Forms.TableLayoutPanel
        Me.PictureBox1 = New System.Windows.Forms.PictureBox
        Me.m_plot = New EwENetworkAnalysis.ucPlot
        Me.m_graph = New ZedGraph.ZedGraphControl
        Me.m_datagrid = New System.Windows.Forms.DataGridView
        Me.m_toolstrip = New ScientificInterfaceShared.Controls.cEwEToolstrip
        Me.tsmiRun = New System.Windows.Forms.ToolStripButton
        Me.tsbtnOptions = New System.Windows.Forms.ToolStripButton
        Me.tsmiDisplayGroups = New System.Windows.Forms.ToolStripButton
        Me.tslblSelection1 = New System.Windows.Forms.ToolStripLabel
        Me.tscmbSelection1 = New System.Windows.Forms.ToolStripComboBox
        Me.tslblSelection2 = New System.Windows.Forms.ToolStripLabel
        Me.tscmbSelection2 = New System.Windows.Forms.ToolStripComboBox
        Me.tsbtnOutputIndicesCSV = New System.Windows.Forms.ToolStripButton
        Me.tsbtnOutputGraphEMF = New System.Windows.Forms.ToolStripButton
        Me.m_hdrPage = New ScientificInterfaceShared.Controls.cEwEHeaderLabel
        ts1 = New System.Windows.Forms.ToolStripSeparator
        ts2 = New System.Windows.Forms.ToolStripSeparator
        ts3 = New System.Windows.Forms.ToolStripSeparator
        Me.m_scNetworkAnalysis.Panel1.SuspendLayout()
        Me.m_scNetworkAnalysis.Panel2.SuspendLayout()
        Me.m_scNetworkAnalysis.SuspendLayout()
        Me.m_scMain.Panel1.SuspendLayout()
        Me.m_scMain.SuspendLayout()
        Me.m_tlpInfo.SuspendLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.m_datagrid, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.m_toolstrip.SuspendLayout()
        Me.SuspendLayout()
        '
        'ts1
        '
        ts1.Name = "ts1"
        resources.ApplyResources(ts1, "ts1")
        '
        'ts2
        '
        ts2.Name = "ts2"
        resources.ApplyResources(ts2, "ts2")
        '
        'ts3
        '
        ts3.Name = "ts3"
        resources.ApplyResources(ts3, "ts3")
        '
        'm_scNetworkAnalysis
        '
        resources.ApplyResources(Me.m_scNetworkAnalysis, "m_scNetworkAnalysis")
        Me.m_scNetworkAnalysis.Name = "m_scNetworkAnalysis"
        '
        'm_scNetworkAnalysis.Panel1
        '
        Me.m_scNetworkAnalysis.Panel1.Controls.Add(Me.m_tvNetworkAnalysis)
        Me.m_scNetworkAnalysis.Panel1Collapsed = True
        '
        'm_scNetworkAnalysis.Panel2
        '
        Me.m_scNetworkAnalysis.Panel2.BackColor = System.Drawing.Color.White
        Me.m_scNetworkAnalysis.Panel2.Controls.Add(Me.m_scMain)
        Me.m_scNetworkAnalysis.Panel2.Controls.Add(Me.m_toolstrip)
        '
        'm_tvNetworkAnalysis
        '
        resources.ApplyResources(Me.m_tvNetworkAnalysis, "m_tvNetworkAnalysis")
        Me.m_tvNetworkAnalysis.FullRowSelect = True
        Me.m_tvNetworkAnalysis.HideSelection = False
        Me.m_tvNetworkAnalysis.Name = "m_tvNetworkAnalysis"
        Me.m_tvNetworkAnalysis.Nodes.AddRange(New System.Windows.Forms.TreeNode() {CType(resources.GetObject("m_tvNetworkAnalysis.Nodes"), System.Windows.Forms.TreeNode), CType(resources.GetObject("m_tvNetworkAnalysis.Nodes1"), System.Windows.Forms.TreeNode), CType(resources.GetObject("m_tvNetworkAnalysis.Nodes2"), System.Windows.Forms.TreeNode), CType(resources.GetObject("m_tvNetworkAnalysis.Nodes3"), System.Windows.Forms.TreeNode), CType(resources.GetObject("m_tvNetworkAnalysis.Nodes4"), System.Windows.Forms.TreeNode), CType(resources.GetObject("m_tvNetworkAnalysis.Nodes5"), System.Windows.Forms.TreeNode), CType(resources.GetObject("m_tvNetworkAnalysis.Nodes6"), System.Windows.Forms.TreeNode), CType(resources.GetObject("m_tvNetworkAnalysis.Nodes7"), System.Windows.Forms.TreeNode), CType(resources.GetObject("m_tvNetworkAnalysis.Nodes8"), System.Windows.Forms.TreeNode)})
        '
        'm_scMain
        '
        resources.ApplyResources(Me.m_scMain, "m_scMain")
        Me.m_scMain.FixedPanel = System.Windows.Forms.FixedPanel.Panel2
        Me.m_scMain.Name = "m_scMain"
        '
        'm_scMain.Panel1
        '
        Me.m_scMain.Panel1.Controls.Add(Me.m_tlpInfo)
        Me.m_scMain.Panel1.Controls.Add(Me.m_plot)
        Me.m_scMain.Panel1.Controls.Add(Me.m_graph)
        Me.m_scMain.Panel1.Controls.Add(Me.m_datagrid)
        '
        'm_tlpInfo
        '
        resources.ApplyResources(Me.m_tlpInfo, "m_tlpInfo")
        Me.m_tlpInfo.Controls.Add(Me.PictureBox1, 1, 1)
        Me.m_tlpInfo.Name = "m_tlpInfo"
        '
        'PictureBox1
        '
        resources.ApplyResources(Me.PictureBox1, "PictureBox1")
        Me.PictureBox1.Image = Global.EwENetworkAnalysis.My.Resources.Resources.N_Asponsors
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.TabStop = False
        '
        'm_plot
        '
        Me.m_plot.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.m_plot, "m_plot")
        Me.m_plot.Name = "m_plot"
        '
        'm_graph
        '
        resources.ApplyResources(Me.m_graph, "m_graph")
        Me.m_graph.Name = "m_graph"
        Me.m_graph.ScrollGrace = 0
        Me.m_graph.ScrollMaxX = 0
        Me.m_graph.ScrollMaxY = 0
        Me.m_graph.ScrollMaxY2 = 0
        Me.m_graph.ScrollMinX = 0
        Me.m_graph.ScrollMinY = 0
        Me.m_graph.ScrollMinY2 = 0
        '
        'm_datagrid
        '
        Me.m_datagrid.BackgroundColor = System.Drawing.SystemColors.ControlLightLight
        Me.m_datagrid.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.m_datagrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        resources.ApplyResources(Me.m_datagrid, "m_datagrid")
        Me.m_datagrid.Name = "m_datagrid"
        Me.m_datagrid.ReadOnly = True
        '
        'm_toolstrip
        '
        Me.m_toolstrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
        Me.m_toolstrip.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsmiRun, ts3, Me.tsbtnOptions, Me.tsmiDisplayGroups, ts1, Me.tslblSelection1, Me.tscmbSelection1, Me.tslblSelection2, Me.tscmbSelection2, ts2, Me.tsbtnOutputIndicesCSV, Me.tsbtnOutputGraphEMF})
        resources.ApplyResources(Me.m_toolstrip, "m_toolstrip")
        Me.m_toolstrip.Name = "m_toolstrip"
        '
        'tsmiRun
        '
        Me.tsmiRun.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        resources.ApplyResources(Me.tsmiRun, "tsmiRun")
        Me.tsmiRun.Name = "tsmiRun"
        '
        'tsbtnOptions
        '
        Me.tsbtnOptions.CheckOnClick = True
        Me.tsbtnOptions.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        resources.ApplyResources(Me.tsbtnOptions, "tsbtnOptions")
        Me.tsbtnOptions.Name = "tsbtnOptions"
        '
        'tsmiDisplayGroups
        '
        Me.tsmiDisplayGroups.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        resources.ApplyResources(Me.tsmiDisplayGroups, "tsmiDisplayGroups")
        Me.tsmiDisplayGroups.Name = "tsmiDisplayGroups"
        '
        'tslblSelection1
        '
        Me.tslblSelection1.Name = "tslblSelection1"
        resources.ApplyResources(Me.tslblSelection1, "tslblSelection1")
        '
        'tscmbSelection1
        '
        Me.tscmbSelection1.BackColor = System.Drawing.SystemColors.Window
        Me.tscmbSelection1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.tscmbSelection1.Name = "tscmbSelection1"
        resources.ApplyResources(Me.tscmbSelection1, "tscmbSelection1")
        '
        'tslblSelection2
        '
        Me.tslblSelection2.Name = "tslblSelection2"
        resources.ApplyResources(Me.tslblSelection2, "tslblSelection2")
        '
        'tscmbSelection2
        '
        Me.tscmbSelection2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.tscmbSelection2.Name = "tscmbSelection2"
        resources.ApplyResources(Me.tscmbSelection2, "tscmbSelection2")
        '
        'tsbtnOutputIndicesCSV
        '
        Me.tsbtnOutputIndicesCSV.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        resources.ApplyResources(Me.tsbtnOutputIndicesCSV, "tsbtnOutputIndicesCSV")
        Me.tsbtnOutputIndicesCSV.Name = "tsbtnOutputIndicesCSV"
        '
        'tsbtnOutputGraphEMF
        '
        Me.tsbtnOutputGraphEMF.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        resources.ApplyResources(Me.tsbtnOutputGraphEMF, "tsbtnOutputGraphEMF")
        Me.tsbtnOutputGraphEMF.Name = "tsbtnOutputGraphEMF"
        '
        'm_hdrPage
        '
        resources.ApplyResources(Me.m_hdrPage, "m_hdrPage")
        Me.m_hdrPage.Name = "m_hdrPage"
        '
        'frmNetworkAnalysis
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.Control
        Me.Controls.Add(Me.m_hdrPage)
        Me.Controls.Add(Me.m_scNetworkAnalysis)
        Me.Name = "frmNetworkAnalysis"
        Me.ShowInTaskbar = False
        Me.TabText = "Network analysis plug-in"
        Me.m_scNetworkAnalysis.Panel1.ResumeLayout(False)
        Me.m_scNetworkAnalysis.Panel2.ResumeLayout(False)
        Me.m_scNetworkAnalysis.Panel2.PerformLayout()
        Me.m_scNetworkAnalysis.ResumeLayout(False)
        Me.m_scMain.Panel1.ResumeLayout(False)
        Me.m_scMain.ResumeLayout(False)
        Me.m_tlpInfo.ResumeLayout(False)
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.m_datagrid, System.ComponentModel.ISupportInitialize).EndInit()
        Me.m_toolstrip.ResumeLayout(False)
        Me.m_toolstrip.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Private WithEvents m_scNetworkAnalysis As System.Windows.Forms.SplitContainer
    Private WithEvents m_tvNetworkAnalysis As System.Windows.Forms.TreeView
    Private WithEvents tscmbSelection1 As System.Windows.Forms.ToolStripComboBox
    Private WithEvents tslblSelection2 As System.Windows.Forms.ToolStripLabel
    Private WithEvents tscmbSelection2 As System.Windows.Forms.ToolStripComboBox
    Private WithEvents tslblSelection1 As System.Windows.Forms.ToolStripLabel
    Private WithEvents PictureBox1 As System.Windows.Forms.PictureBox
    Private WithEvents tsbtnOutputIndicesCSV As System.Windows.Forms.ToolStripButton
    Private WithEvents tsbtnOutputGraphEMF As System.Windows.Forms.ToolStripButton
    Private WithEvents m_toolstrip As cEwEToolstrip
    Private WithEvents m_datagrid As System.Windows.Forms.DataGridView
    Private WithEvents m_graph As ZedGraph.ZedGraphControl
    Private WithEvents m_tlpInfo As System.Windows.Forms.TableLayoutPanel
    Private WithEvents m_plot As ucPlot
    Private WithEvents tsmiDisplayGroups As System.Windows.Forms.ToolStripButton
    Private WithEvents tsmiRun As System.Windows.Forms.ToolStripButton
    Private WithEvents m_hdrPage As ScientificInterfaceShared.Controls.cEwEHeaderLabel
    Private WithEvents tsbtnOptions As System.Windows.Forms.ToolStripButton
    Private WithEvents m_scMain As System.Windows.Forms.SplitContainer
End Class
