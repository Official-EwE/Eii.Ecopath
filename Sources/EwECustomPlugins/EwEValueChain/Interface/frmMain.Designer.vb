Imports WeifenLuo.WinFormsUI.Docking

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMain
    Inherits DockContent

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
        Dim TreeNode1 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Parameters", 2, 2)
        Dim TreeNode2 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Defaults")
        Dim TreeNode3 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Flow", 0, 0)
        Dim TreeNode4 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Producer", 0, 0)
        Dim TreeNode5 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Processing", 0, 0)
        Dim TreeNode6 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Distribution", 0, 0)
        Dim TreeNode7 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Market", 0, 0)
        Dim TreeNode8 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Consumer")
        Dim TreeNode9 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Tables", 6, 6, New System.Windows.Forms.TreeNode() {TreeNode4, TreeNode5, TreeNode6, TreeNode7, TreeNode8})
        Dim TreeNode10 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Input", 6, 6, New System.Windows.Forms.TreeNode() {TreeNode2, TreeNode3, TreeNode9})
        Dim TreeNode11 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Run value chain", 1, 1)
        Dim TreeNode12 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Output", 5, 5, New System.Windows.Forms.TreeNode() {TreeNode11})
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMain))
        Me.scMain = New System.Windows.Forms.SplitContainer
        Me.m_tvNav = New System.Windows.Forms.TreeView
        Me.m_ilNavigation = New System.Windows.Forms.ImageList(Me.components)
        Me.scMain.Panel1.SuspendLayout()
        Me.scMain.SuspendLayout()
        Me.SuspendLayout()
        '
        'scMain
        '
        Me.scMain.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.scMain.Location = New System.Drawing.Point(9, 9)
        Me.scMain.Margin = New System.Windows.Forms.Padding(0)
        Me.scMain.Name = "scMain"
        '
        'scMain.Panel1
        '
        Me.scMain.Panel1.Controls.Add(Me.m_tvNav)
        Me.scMain.Size = New System.Drawing.Size(981, 565)
        Me.scMain.SplitterDistance = 196
        Me.scMain.TabIndex = 6
        '
        'm_tvNav
        '
        Me.m_tvNav.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_tvNav.BackColor = System.Drawing.SystemColors.Window
        Me.m_tvNav.HideSelection = False
        Me.m_tvNav.ImageIndex = 0
        Me.m_tvNav.ImageList = Me.m_ilNavigation
        Me.m_tvNav.Location = New System.Drawing.Point(0, 0)
        Me.m_tvNav.Margin = New System.Windows.Forms.Padding(0)
        Me.m_tvNav.Name = "m_tvNav"
        TreeNode1.ImageIndex = 2
        TreeNode1.Name = "ndParameters"
        TreeNode1.SelectedImageIndex = 2
        TreeNode1.Text = "Parameters"
        TreeNode2.Name = "ndDefaults"
        TreeNode2.Text = "Defaults"
        TreeNode3.ImageIndex = 0
        TreeNode3.Name = "ndFlow"
        TreeNode3.SelectedImageIndex = 0
        TreeNode3.Text = "Flow"
        TreeNode4.ImageIndex = 0
        TreeNode4.Name = "ndProducer"
        TreeNode4.SelectedImageIndex = 0
        TreeNode4.Text = "Producer"
        TreeNode5.ImageIndex = 0
        TreeNode5.Name = "ndProcessing"
        TreeNode5.SelectedImageIndex = 0
        TreeNode5.Text = "Processing"
        TreeNode6.ImageIndex = 0
        TreeNode6.Name = "ndDistribution"
        TreeNode6.SelectedImageIndex = 0
        TreeNode6.Text = "Distribution"
        TreeNode7.ImageIndex = 0
        TreeNode7.Name = "ndMarket"
        TreeNode7.SelectedImageIndex = 0
        TreeNode7.Text = "Market"
        TreeNode8.Name = "ndConsumer"
        TreeNode8.Text = "Consumer"
        TreeNode9.ImageIndex = 6
        TreeNode9.Name = "ndUnits"
        TreeNode9.SelectedImageIndex = 6
        TreeNode9.Text = "Tables"
        TreeNode10.ImageIndex = 6
        TreeNode10.Name = "ndInput"
        TreeNode10.SelectedImageIndex = 6
        TreeNode10.Text = "Input"
        TreeNode11.ImageIndex = 1
        TreeNode11.Name = "ndRun"
        TreeNode11.SelectedImageIndex = 1
        TreeNode11.Text = "Run value chain"
        TreeNode12.ImageIndex = 5
        TreeNode12.Name = "ndOutput"
        TreeNode12.SelectedImageIndex = 5
        TreeNode12.Text = "Output"
        Me.m_tvNav.Nodes.AddRange(New System.Windows.Forms.TreeNode() {TreeNode1, TreeNode10, TreeNode12})
        Me.m_tvNav.SelectedImageIndex = 0
        Me.m_tvNav.Size = New System.Drawing.Size(196, 565)
        Me.m_tvNav.TabIndex = 3
        '
        'm_ilNavigation
        '
        Me.m_ilNavigation.ImageStream = CType(resources.GetObject("m_ilNavigation.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.m_ilNavigation.TransparentColor = System.Drawing.Color.Transparent
        Me.m_ilNavigation.Images.SetKeyName(0, "application_get.png")
        Me.m_ilNavigation.Images.SetKeyName(1, "application_put.png")
        Me.m_ilNavigation.Images.SetKeyName(2, "run.bmp")
        Me.m_ilNavigation.Images.SetKeyName(3, "tools.bmp")
        Me.m_ilNavigation.Images.SetKeyName(4, "Ecopath.bmp")
        Me.m_ilNavigation.Images.SetKeyName(5, "output_extend.png")
        Me.m_ilNavigation.Images.SetKeyName(6, "input_extend.png")
        Me.m_ilNavigation.Images.SetKeyName(7, "wi0064-16.ico")
        Me.m_ilNavigation.Images.SetKeyName(8, "wi0126-16.ico")
        Me.m_ilNavigation.Images.SetKeyName(9, "wi0122-16.ico")
        Me.m_ilNavigation.Images.SetKeyName(10, "wi0054-16.ico")
        '
        'frmMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(999, 583)
        Me.Controls.Add(Me.scMain)
        Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmMain"
        Me.TabText = "<title>"
        Me.Text = "<title>"
        Me.scMain.Panel1.ResumeLayout(False)
        Me.scMain.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents scMain As System.Windows.Forms.SplitContainer
    Friend WithEvents m_tvNav As System.Windows.Forms.TreeView
    Friend WithEvents m_ilNavigation As System.Windows.Forms.ImageList
End Class
