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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmNetworkAnalysis))
        Me.scNetworkAnalysis = New System.Windows.Forms.SplitContainer
        Me.tvNetworkAnalysis = New System.Windows.Forms.TreeView
        Me.imglstNetworkAnalysis = New System.Windows.Forms.ImageList(Me.components)
        Me.m_graph = New ZedGraph.ZedGraphControl
        Me.m_plot = New EwENetworkAnalysis.ucPlot
        Me.m_datagrid = New System.Windows.Forms.DataGridView
        Me.m_tlpInfo = New System.Windows.Forms.TableLayoutPanel
        Me.PictureBox1 = New System.Windows.Forms.PictureBox
        Me.m_toolstrip = New System.Windows.Forms.ToolStrip
        Me.tslblSelection1 = New System.Windows.Forms.ToolStripLabel
        Me.tscmbSelection1 = New System.Windows.Forms.ToolStripComboBox
        Me.tslblSelection2 = New System.Windows.Forms.ToolStripLabel
        Me.tscmbSelection2 = New System.Windows.Forms.ToolStripComboBox
        Me.tsbtnOutputIndicesCSV = New System.Windows.Forms.ToolStripButton
        Me.tsbtnOutputGraphEMF = New System.Windows.Forms.ToolStripButton
        Me.lblNetworkAnalysis = New System.Windows.Forms.Label
        Me.scNetworkAnalysis.Panel1.SuspendLayout()
        Me.scNetworkAnalysis.Panel2.SuspendLayout()
        Me.scNetworkAnalysis.SuspendLayout()
        CType(Me.m_datagrid, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.m_tlpInfo.SuspendLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.m_toolstrip.SuspendLayout()
        Me.SuspendLayout()
        '
        'scNetworkAnalysis
        '
        resources.ApplyResources(Me.scNetworkAnalysis, "scNetworkAnalysis")
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
        Me.scNetworkAnalysis.Panel2.Controls.Add(Me.m_datagrid)
        Me.scNetworkAnalysis.Panel2.Controls.Add(Me.m_tlpInfo)
        Me.scNetworkAnalysis.Panel2.Controls.Add(Me.m_toolstrip)
        '
        'tvNetworkAnalysis
        '
        Me.tvNetworkAnalysis.BackColor = System.Drawing.Color.MintCream
        resources.ApplyResources(Me.tvNetworkAnalysis, "tvNetworkAnalysis")
        Me.tvNetworkAnalysis.FullRowSelect = True
        Me.tvNetworkAnalysis.HideSelection = False
        Me.tvNetworkAnalysis.ImageList = Me.imglstNetworkAnalysis
        Me.tvNetworkAnalysis.Name = "tvNetworkAnalysis"
        Me.tvNetworkAnalysis.Nodes.AddRange(New System.Windows.Forms.TreeNode() {CType(resources.GetObject("tvNetworkAnalysis.Nodes"), System.Windows.Forms.TreeNode), CType(resources.GetObject("tvNetworkAnalysis.Nodes1"), System.Windows.Forms.TreeNode), CType(resources.GetObject("tvNetworkAnalysis.Nodes2"), System.Windows.Forms.TreeNode), CType(resources.GetObject("tvNetworkAnalysis.Nodes3"), System.Windows.Forms.TreeNode), CType(resources.GetObject("tvNetworkAnalysis.Nodes4"), System.Windows.Forms.TreeNode), CType(resources.GetObject("tvNetworkAnalysis.Nodes5"), System.Windows.Forms.TreeNode), CType(resources.GetObject("tvNetworkAnalysis.Nodes6"), System.Windows.Forms.TreeNode), CType(resources.GetObject("tvNetworkAnalysis.Nodes7"), System.Windows.Forms.TreeNode), CType(resources.GetObject("tvNetworkAnalysis.Nodes8"), System.Windows.Forms.TreeNode)})
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
        'm_plot
        '
        Me.m_plot.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        resources.ApplyResources(Me.m_plot, "m_plot")
        Me.m_plot.Name = "m_plot"
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
        'm_toolstrip
        '
        Me.m_toolstrip.BackColor = System.Drawing.Color.DarkSeaGreen
        Me.m_toolstrip.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tslblSelection1, Me.tscmbSelection1, Me.tslblSelection2, Me.tscmbSelection2, Me.tsbtnOutputIndicesCSV, Me.tsbtnOutputGraphEMF})
        resources.ApplyResources(Me.m_toolstrip, "m_toolstrip")
        Me.m_toolstrip.Name = "m_toolstrip"
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
        Me.tsbtnOutputIndicesCSV.BackColor = System.Drawing.Color.DarkSeaGreen
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
        'lblNetworkAnalysis
        '
        resources.ApplyResources(Me.lblNetworkAnalysis, "lblNetworkAnalysis")
        Me.lblNetworkAnalysis.Name = "lblNetworkAnalysis"
        '
        'frmNetworkAnalysis
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.Control
        Me.Controls.Add(Me.lblNetworkAnalysis)
        Me.Controls.Add(Me.scNetworkAnalysis)
        Me.Name = "frmNetworkAnalysis"
        Me.ShowInTaskbar = False
        Me.TabText = "Network analysis plug-in"
        Me.scNetworkAnalysis.Panel1.ResumeLayout(False)
        Me.scNetworkAnalysis.Panel2.ResumeLayout(False)
        Me.scNetworkAnalysis.Panel2.PerformLayout()
        Me.scNetworkAnalysis.ResumeLayout(False)
        CType(Me.m_datagrid, System.ComponentModel.ISupportInitialize).EndInit()
        Me.m_tlpInfo.ResumeLayout(False)
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.m_toolstrip.ResumeLayout(False)
        Me.m_toolstrip.PerformLayout()
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
    Private WithEvents tslblSelection1 As System.Windows.Forms.ToolStripLabel
    Private WithEvents PictureBox1 As System.Windows.Forms.PictureBox
    Private WithEvents tsbtnOutputIndicesCSV As System.Windows.Forms.ToolStripButton
    Private WithEvents tsbtnOutputGraphEMF As System.Windows.Forms.ToolStripButton
    Private WithEvents m_toolstrip As System.Windows.Forms.ToolStrip
    Private WithEvents m_datagrid As System.Windows.Forms.DataGridView
    Private WithEvents m_graph As ZedGraph.ZedGraphControl
    Private WithEvents m_tlpInfo As System.Windows.Forms.TableLayoutPanel
    Private WithEvents m_plot As ucPlot
End Class
