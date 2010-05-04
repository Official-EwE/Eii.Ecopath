Namespace Controls

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class ucZedGraphHoverMenu
        Inherits Form

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
            Me.m_btnIn = New System.Windows.Forms.Button
            Me.m_btnOut = New System.Windows.Forms.Button
            Me.SuspendLayout()
            '
            'm_btnIn
            '
            Me.m_btnIn.Image = Global.ScientificInterfaceShared.My.Resources.Resources.ZoomInHS
            Me.m_btnIn.Location = New System.Drawing.Point(0, 0)
            Me.m_btnIn.Margin = New System.Windows.Forms.Padding(0)
            Me.m_btnIn.Name = "m_btnIn"
            Me.m_btnIn.Size = New System.Drawing.Size(23, 23)
            Me.m_btnIn.TabIndex = 0
            Me.m_btnIn.UseVisualStyleBackColor = True
            '
            'm_btnOut
            '
            Me.m_btnOut.Image = Global.ScientificInterfaceShared.My.Resources.Resources.ZoomOutHS
            Me.m_btnOut.Location = New System.Drawing.Point(23, 0)
            Me.m_btnOut.Margin = New System.Windows.Forms.Padding(0)
            Me.m_btnOut.Name = "m_btnOut"
            Me.m_btnOut.Size = New System.Drawing.Size(23, 23)
            Me.m_btnOut.TabIndex = 0
            Me.m_btnOut.UseVisualStyleBackColor = True
            '
            'ucZedGraphHoverMenu
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
            Me.BackColor = System.Drawing.SystemColors.ButtonFace
            Me.ClientSize = New System.Drawing.Size(46, 23)
            Me.ControlBox = False
            Me.Controls.Add(Me.m_btnOut)
            Me.Controls.Add(Me.m_btnIn)
            Me.DoubleBuffered = True
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
            Me.MaximumSize = New System.Drawing.Size(46, 23)
            Me.MinimumSize = New System.Drawing.Size(46, 23)
            Me.Name = "ucZedGraphHoverMenu"
            Me.ShowIcon = False
            Me.ShowInTaskbar = False
            Me.ResumeLayout(False)

        End Sub
        Private WithEvents m_btnIn As System.Windows.Forms.Button
        Private WithEvents m_btnOut As System.Windows.Forms.Button

    End Class

End Namespace ' Controls