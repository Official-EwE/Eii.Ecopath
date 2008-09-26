Namespace Controls

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class ucGlyphSelect
        Inherits System.Windows.Forms.UserControl

        'UserControl overrides dispose to clean up the component list.
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
            Me.m_flpGlyphs = New System.Windows.Forms.FlowLayoutPanel
            Me.SuspendLayout()
            '
            'flpGlyphs
            '
            Me.m_flpGlyphs.AutoScroll = True
            Me.m_flpGlyphs.BackColor = System.Drawing.SystemColors.Window
            Me.m_flpGlyphs.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_flpGlyphs.Location = New System.Drawing.Point(0, 0)
            Me.m_flpGlyphs.Margin = New System.Windows.Forms.Padding(0)
            Me.m_flpGlyphs.Name = "flpGlyphs"
            Me.m_flpGlyphs.Size = New System.Drawing.Size(282, 150)
            Me.m_flpGlyphs.TabIndex = 0
            '
            'ucGlyphSelect
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.m_flpGlyphs)
            Me.Name = "ucGlyphSelect"
            Me.Size = New System.Drawing.Size(282, 150)
            Me.ResumeLayout(False)

        End Sub
        Friend WithEvents m_flpGlyphs As System.Windows.Forms.FlowLayoutPanel

    End Class

End Namespace
