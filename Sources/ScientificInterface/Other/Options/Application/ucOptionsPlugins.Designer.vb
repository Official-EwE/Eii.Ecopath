Imports ScientificInterfaceShared

Namespace Other

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class ucOptionsPlugins
        Inherits System.Windows.Forms.UserControl

        'Required by the Windows Form Designer
        Private components As System.ComponentModel.IContainer

        'NOTE: The following procedure is required by the Windows Form Designer
        'It can be modified using the Windows Form Designer.  
        'Do not modify it using the code editor.
        <System.Diagnostics.DebuggerStepThrough()> _
        Private Sub InitializeComponent()
            Me.components = New System.ComponentModel.Container
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ucOptionsPlugins))
            Me.m_hdrCaption = New ScientificInterfaceShared.Controls.cEwEHeaderLabel
            Me.m_tvPlugins = New System.Windows.Forms.TreeView
            Me.m_ilPlugins = New System.Windows.Forms.ImageList(Me.components)
            Me.m_split = New System.Windows.Forms.SplitContainer
            Me.m_cbEnablePlugin = New System.Windows.Forms.CheckBox
            Me.m_split.Panel1.SuspendLayout()
            Me.m_split.SuspendLayout()
            Me.SuspendLayout()
            '
            'm_hdrCaption
            '
            Me.m_hdrCaption.Dock = System.Windows.Forms.DockStyle.Top
            Me.m_hdrCaption.Location = New System.Drawing.Point(0, 0)
            Me.m_hdrCaption.Name = "m_hdrCaption"
            Me.m_hdrCaption.Size = New System.Drawing.Size(414, 18)
            Me.m_hdrCaption.TabIndex = 0
            Me.m_hdrCaption.Text = "Plugins"
            Me.m_hdrCaption.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'm_tvPlugins
            '
            Me.m_tvPlugins.BorderStyle = System.Windows.Forms.BorderStyle.None
            Me.m_tvPlugins.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_tvPlugins.FullRowSelect = True
            Me.m_tvPlugins.HideSelection = False
            Me.m_tvPlugins.ImageIndex = 0
            Me.m_tvPlugins.ImageList = Me.m_ilPlugins
            Me.m_tvPlugins.Location = New System.Drawing.Point(0, 0)
            Me.m_tvPlugins.Margin = New System.Windows.Forms.Padding(0)
            Me.m_tvPlugins.Name = "m_tvPlugins"
            Me.m_tvPlugins.SelectedImageIndex = 0
            Me.m_tvPlugins.Size = New System.Drawing.Size(134, 302)
            Me.m_tvPlugins.TabIndex = 0
            '
            'm_ilPlugins
            '
            Me.m_ilPlugins.ImageStream = CType(resources.GetObject("m_ilPlugins.ImageStream"), System.Windows.Forms.ImageListStreamer)
            Me.m_ilPlugins.TransparentColor = System.Drawing.Color.Transparent
            Me.m_ilPlugins.Images.SetKeyName(0, "Ecopath.ico")
            Me.m_ilPlugins.Images.SetKeyName(1, "pluginicon.png")
            Me.m_ilPlugins.Images.SetKeyName(2, "BreakpointHS.png")
            Me.m_ilPlugins.Images.SetKeyName(3, "ConflictHS.png")
            '
            'm_split
            '
            Me.m_split.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_split.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
            Me.m_split.Location = New System.Drawing.Point(0, 24)
            Me.m_split.Margin = New System.Windows.Forms.Padding(0)
            Me.m_split.Name = "m_split"
            '
            'm_split.Panel1
            '
            Me.m_split.Panel1.Controls.Add(Me.m_tvPlugins)
            Me.m_split.Size = New System.Drawing.Size(414, 306)
            Me.m_split.SplitterDistance = 138
            Me.m_split.TabIndex = 1
            '
            'm_cbEnablePlugin
            '
            Me.m_cbEnablePlugin.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
            Me.m_cbEnablePlugin.AutoSize = True
            Me.m_cbEnablePlugin.Location = New System.Drawing.Point(0, 333)
            Me.m_cbEnablePlugin.Name = "m_cbEnablePlugin"
            Me.m_cbEnablePlugin.Size = New System.Drawing.Size(262, 17)
            Me.m_cbEnablePlugin.TabIndex = 2
            Me.m_cbEnablePlugin.Text = "&Load selected plug-in module next time EwE starts"
            Me.m_cbEnablePlugin.UseVisualStyleBackColor = True
            '
            'ucOptionsPlugins
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.m_cbEnablePlugin)
            Me.Controls.Add(Me.m_split)
            Me.Controls.Add(Me.m_hdrCaption)
            Me.Margin = New System.Windows.Forms.Padding(0)
            Me.Name = "ucOptionsPlugins"
            Me.Size = New System.Drawing.Size(414, 353)
            Me.m_split.Panel1.ResumeLayout(False)
            Me.m_split.ResumeLayout(False)
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents m_tvPlugins As System.Windows.Forms.TreeView
        Private WithEvents m_split As System.Windows.Forms.SplitContainer
        Private WithEvents m_ilPlugins As System.Windows.Forms.ImageList
        Private WithEvents m_hdrCaption As cEwEHeaderLabel
        Private WithEvents m_cbEnablePlugin As System.Windows.Forms.CheckBox

    End Class

End Namespace
