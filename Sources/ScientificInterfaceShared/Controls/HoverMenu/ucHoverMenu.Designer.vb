Namespace Controls

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class ucHoverMenu
        Inherits UserControl

        'Required by the Windows Form Designer
        Private components As System.ComponentModel.IContainer

        'NOTE: The following procedure is required by the Windows Form Designer
        'It can be modified using the Windows Form Designer.  
        'Do not modify it using the code editor.
        <System.Diagnostics.DebuggerStepThrough()> _
        Private Sub InitializeComponent()
            Me.m_ts = New System.Windows.Forms.ToolStrip
            Me.m_tsbnZoomIn = New System.Windows.Forms.ToolStripButton
            Me.m_tsbnZoomOut = New System.Windows.Forms.ToolStripButton
            Me.m_tsbnZoomReset = New System.Windows.Forms.ToolStripButton
            Me.m_ts.SuspendLayout()
            Me.SuspendLayout()
            '
            'm_ts
            '
            Me.m_ts.CanOverflow = False
            Me.m_ts.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
            Me.m_ts.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tsbnZoomIn, Me.m_tsbnZoomOut, Me.m_tsbnZoomReset})
            Me.m_ts.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow
            Me.m_ts.Location = New System.Drawing.Point(0, 0)
            Me.m_ts.Name = "m_ts"
            Me.m_ts.Size = New System.Drawing.Size(217, 23)
            Me.m_ts.TabIndex = 0
            '
            'm_tsbnZoomIn
            '
            Me.m_tsbnZoomIn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
            Me.m_tsbnZoomIn.Image = Global.ScientificInterfaceShared.My.Resources.Resources.ZoomInHS
            Me.m_tsbnZoomIn.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.m_tsbnZoomIn.Name = "m_tsbnZoomIn"
            Me.m_tsbnZoomIn.Size = New System.Drawing.Size(23, 20)
            Me.m_tsbnZoomIn.Text = "Zoom in"
            '
            'm_tsbnZoomOut
            '
            Me.m_tsbnZoomOut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
            Me.m_tsbnZoomOut.Image = Global.ScientificInterfaceShared.My.Resources.Resources.ZoomOutHS
            Me.m_tsbnZoomOut.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.m_tsbnZoomOut.Name = "m_tsbnZoomOut"
            Me.m_tsbnZoomOut.Size = New System.Drawing.Size(23, 20)
            Me.m_tsbnZoomOut.Text = "Zoom out"
            '
            'm_tsbnZoomReset
            '
            Me.m_tsbnZoomReset.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
            Me.m_tsbnZoomReset.Image = Global.ScientificInterfaceShared.My.Resources.Resources.ZoomHS
            Me.m_tsbnZoomReset.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.m_tsbnZoomReset.Name = "m_tsbnZoomReset"
            Me.m_tsbnZoomReset.Size = New System.Drawing.Size(23, 20)
            Me.m_tsbnZoomReset.Text = "Reset all zoom"
            '
            'ucHoverMenu
            '
            Me.AutoSize = True
            Me.BackColor = System.Drawing.SystemColors.ButtonFace
            Me.Controls.Add(Me.m_ts)
            Me.Name = "ucHoverMenu"
            Me.Size = New System.Drawing.Size(217, 23)
            Me.m_ts.ResumeLayout(False)
            Me.m_ts.PerformLayout()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents m_ts As System.Windows.Forms.ToolStrip
        Private WithEvents m_tsbnZoomIn As System.Windows.Forms.ToolStripButton
        Private WithEvents m_tsbnZoomOut As System.Windows.Forms.ToolStripButton
        Private WithEvents m_tsbnZoomReset As System.Windows.Forms.ToolStripButton
    End Class

End Namespace ' Controls