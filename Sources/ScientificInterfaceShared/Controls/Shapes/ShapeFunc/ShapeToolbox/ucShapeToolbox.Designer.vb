
Namespace Controls

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class ucShapeToolbox
        Inherits System.Windows.Forms.UserControl

        'Required by the Windows Form Designer
        Private components As System.ComponentModel.IContainer

        'NOTE: The following procedure is required by the Windows Form Designer
        'It can be modified using the Windows Form Designer.  
        'Do not modify it using the code editor.
        <System.Diagnostics.DebuggerStepThrough()> _
        Private Sub InitializeComponent()
            Me.components = New System.ComponentModel.Container
            Me.m_lvContextMenuStrip = New System.Windows.Forms.ContextMenuStrip(Me.components)
            Me.AddToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
            Me.ApplyToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
            Me.DuplicateToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
            Me.ImportToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
            Me.RemoveToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
            Me.RenameToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
            Me.m_lvShapes = New ScientificInterfaceShared.Controls.cSmoothListView
            Me.ExportToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
            Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator
            Me.ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator
            Me.m_lvContextMenuStrip.SuspendLayout()
            Me.SuspendLayout()
            '
            'lvContextMenuStrip
            '
            Me.m_lvContextMenuStrip.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.AddToolStripMenuItem, Me.ApplyToolStripMenuItem, Me.DuplicateToolStripMenuItem, Me.ToolStripSeparator1, Me.ImportToolStripMenuItem, Me.ExportToolStripMenuItem, Me.ToolStripSeparator2, Me.RemoveToolStripMenuItem, Me.RenameToolStripMenuItem})
            Me.m_lvContextMenuStrip.Name = "lvContextMenuStrip"
            Me.m_lvContextMenuStrip.Size = New System.Drawing.Size(153, 192)
            '
            'AddToolStripMenuItem
            '
            Me.AddToolStripMenuItem.Name = "AddToolStripMenuItem"
            Me.AddToolStripMenuItem.Size = New System.Drawing.Size(152, 22)
            Me.AddToolStripMenuItem.Text = "A&dd..."
            '
            'ApplyToolStripMenuItem
            '
            Me.ApplyToolStripMenuItem.Name = "ApplyToolStripMenuItem"
            Me.ApplyToolStripMenuItem.Size = New System.Drawing.Size(152, 22)
            Me.ApplyToolStripMenuItem.Text = "A&pply..."
            '
            'DuplicateToolStripMenuItem
            '
            Me.DuplicateToolStripMenuItem.Name = "DuplicateToolStripMenuItem"
            Me.DuplicateToolStripMenuItem.Size = New System.Drawing.Size(152, 22)
            Me.DuplicateToolStripMenuItem.Text = "&Duplicate"
            '
            'ImportToolStripMenuItem
            '
            Me.ImportToolStripMenuItem.Name = "ImportToolStripMenuItem"
            Me.ImportToolStripMenuItem.Size = New System.Drawing.Size(152, 22)
            Me.ImportToolStripMenuItem.Text = "&Import..."
            '
            'RemoveToolStripMenuItem
            '
            Me.RemoveToolStripMenuItem.Name = "RemoveToolStripMenuItem"
            Me.RemoveToolStripMenuItem.Size = New System.Drawing.Size(152, 22)
            Me.RemoveToolStripMenuItem.Text = "Re&move"
            '
            'RenameToolStripMenuItem
            '
            Me.RenameToolStripMenuItem.Name = "RenameToolStripMenuItem"
            Me.RenameToolStripMenuItem.Size = New System.Drawing.Size(152, 22)
            Me.RenameToolStripMenuItem.Text = "Re&name"
            '
            'lvShapes
            '
            Me.m_lvShapes.BackColor = System.Drawing.SystemColors.Window
            Me.m_lvShapes.BorderStyle = System.Windows.Forms.BorderStyle.None
            Me.m_lvShapes.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_lvShapes.HideSelection = False
            Me.m_lvShapes.Location = New System.Drawing.Point(0, 0)
            Me.m_lvShapes.Name = "lvShapes"
            Me.m_lvShapes.Size = New System.Drawing.Size(690, 150)
            Me.m_lvShapes.TabIndex = 0
            Me.m_lvShapes.UseCompatibleStateImageBehavior = False
            '
            'ExportToolStripMenuItem
            '
            Me.ExportToolStripMenuItem.Name = "ExportToolStripMenuItem"
            Me.ExportToolStripMenuItem.Size = New System.Drawing.Size(152, 22)
            Me.ExportToolStripMenuItem.Text = "E&xport..."
            '
            'ToolStripSeparator1
            '
            Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
            Me.ToolStripSeparator1.Size = New System.Drawing.Size(149, 6)
            '
            'ToolStripSeparator2
            '
            Me.ToolStripSeparator2.Name = "ToolStripSeparator2"
            Me.ToolStripSeparator2.Size = New System.Drawing.Size(149, 6)
            '
            'ucShapeToolbox
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ContextMenuStrip = Me.m_lvContextMenuStrip
            Me.Controls.Add(Me.m_lvShapes)
            Me.Name = "ucShapeToolbox"
            Me.Size = New System.Drawing.Size(690, 150)
            Me.m_lvContextMenuStrip.ResumeLayout(False)
            Me.ResumeLayout(False)

        End Sub
        Private WithEvents m_lvShapes As cSmoothListView
        Private WithEvents m_lvContextMenuStrip As System.Windows.Forms.ContextMenuStrip
        Private WithEvents AddToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
        Private WithEvents DuplicateToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
        Private WithEvents RemoveToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
        Private WithEvents RenameToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
        Private WithEvents ApplyToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
        Private WithEvents ImportToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
        Friend WithEvents ExportToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents ToolStripSeparator2 As System.Windows.Forms.ToolStripSeparator

    End Class

End Namespace

