
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
            Me.lvContextMenuStrip = New System.Windows.Forms.ContextMenuStrip(Me.components)
            Me.AddToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
            Me.ApplyToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
            Me.DuplicateToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
            Me.ImportToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
            Me.RemoveToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
            Me.RenameToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
            Me.lvShapes = New ScientificInterfaceShared.Controls.cSmoothListView
            Me.ExportToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
            Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator
            Me.ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator
            Me.lvContextMenuStrip.SuspendLayout()
            Me.SuspendLayout()
            '
            'lvContextMenuStrip
            '
            Me.lvContextMenuStrip.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.AddToolStripMenuItem, Me.ApplyToolStripMenuItem, Me.DuplicateToolStripMenuItem, Me.ToolStripSeparator1, Me.ImportToolStripMenuItem, Me.ExportToolStripMenuItem, Me.ToolStripSeparator2, Me.RemoveToolStripMenuItem, Me.RenameToolStripMenuItem})
            Me.lvContextMenuStrip.Name = "lvContextMenuStrip"
            Me.lvContextMenuStrip.Size = New System.Drawing.Size(153, 192)
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
            Me.lvShapes.BackColor = System.Drawing.SystemColors.Window
            Me.lvShapes.BorderStyle = System.Windows.Forms.BorderStyle.None
            Me.lvShapes.Dock = System.Windows.Forms.DockStyle.Fill
            Me.lvShapes.HideSelection = False
            Me.lvShapes.Location = New System.Drawing.Point(0, 0)
            Me.lvShapes.Name = "lvShapes"
            Me.lvShapes.Size = New System.Drawing.Size(690, 150)
            Me.lvShapes.TabIndex = 0
            Me.lvShapes.UseCompatibleStateImageBehavior = False
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
            Me.ContextMenuStrip = Me.lvContextMenuStrip
            Me.Controls.Add(Me.lvShapes)
            Me.Name = "ucShapeToolbox"
            Me.Size = New System.Drawing.Size(690, 150)
            Me.lvContextMenuStrip.ResumeLayout(False)
            Me.ResumeLayout(False)

        End Sub
        Private WithEvents lvShapes As cSmoothListView
        Private WithEvents lvContextMenuStrip As System.Windows.Forms.ContextMenuStrip
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

