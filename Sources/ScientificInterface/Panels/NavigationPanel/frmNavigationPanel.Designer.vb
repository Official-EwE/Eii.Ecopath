Imports ScientificInterfaceShared.Forms

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmNavigationPanel
    Inherits frmEwEDockContent

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmNavigationPanel))
        Me.m_tvNavigation = New System.Windows.Forms.TreeView
        Me.m_ilTreeIcons = New System.Windows.Forms.ImageList(Me.components)
        Me.SuspendLayout()
        '
        'm_tvNavigation
        '
        resources.ApplyResources(Me.m_tvNavigation, "m_tvNavigation")
        Me.m_tvNavigation.FullRowSelect = True
        Me.m_tvNavigation.HideSelection = False
        Me.m_tvNavigation.HotTracking = True
        Me.m_tvNavigation.ImageList = Me.m_ilTreeIcons
        Me.m_tvNavigation.Name = "m_tvNavigation"
        Me.m_tvNavigation.Nodes.AddRange(New System.Windows.Forms.TreeNode() {CType(resources.GetObject("m_tvNavigation.Nodes"), System.Windows.Forms.TreeNode), CType(resources.GetObject("m_tvNavigation.Nodes1"), System.Windows.Forms.TreeNode), CType(resources.GetObject("m_tvNavigation.Nodes2"), System.Windows.Forms.TreeNode), CType(resources.GetObject("m_tvNavigation.Nodes3"), System.Windows.Forms.TreeNode), CType(resources.GetObject("m_tvNavigation.Nodes4"), System.Windows.Forms.TreeNode)})
        '
        'm_ilTreeIcons
        '
        Me.m_ilTreeIcons.ImageStream = CType(resources.GetObject("m_ilTreeIcons.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.m_ilTreeIcons.TransparentColor = System.Drawing.Color.Transparent
        Me.m_ilTreeIcons.Images.SetKeyName(0, "application_get.png")
        Me.m_ilTreeIcons.Images.SetKeyName(1, "application_put.png")
        Me.m_ilTreeIcons.Images.SetKeyName(2, "run.bmp")
        Me.m_ilTreeIcons.Images.SetKeyName(3, "tools.bmp")
        Me.m_ilTreeIcons.Images.SetKeyName(4, "output_extend.png")
        Me.m_ilTreeIcons.Images.SetKeyName(5, "input_extend.png")
        Me.m_ilTreeIcons.Images.SetKeyName(6, "Ecospace_32x32.png")
        Me.m_ilTreeIcons.Images.SetKeyName(7, "Ecosim_32x32.png")
        Me.m_ilTreeIcons.Images.SetKeyName(8, "Ecopath_32x32.png")
        '
        'NavigationPanel
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.ControlLightLight
        Me.Controls.Add(Me.m_tvNavigation)
        Me.HideOnClose = True
        Me.Name = "NavigationPanel"
        Me.TabText = "Navigator"
        Me.ResumeLayout(False)

    End Sub

    Private WithEvents m_tvNavigation As System.Windows.Forms.TreeView
    Private WithEvents m_ilTreeIcons As System.Windows.Forms.ImageList
End Class
