Namespace Controls

    Partial Class ucHatchSelect
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
            Me.flpItems = New System.Windows.Forms.FlowLayoutPanel
            Me.SuspendLayout()
            '
            'flpItems
            '
            Me.flpItems.AutoScroll = True
            Me.flpItems.Dock = System.Windows.Forms.DockStyle.Fill
            Me.flpItems.Location = New System.Drawing.Point(0, 0)
            Me.flpItems.Name = "flpItems"
            Me.flpItems.Size = New System.Drawing.Size(140, 105)
            Me.flpItems.TabIndex = 0
            '
            'ucHatchSelect
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.BackColor = System.Drawing.SystemColors.Window
            Me.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.Controls.Add(Me.flpItems)
            Me.Name = "ucHatchSelect"
            Me.Size = New System.Drawing.Size(140, 105)
            Me.ResumeLayout(False)

        End Sub
        Friend WithEvents flpItems As System.Windows.Forms.FlowLayoutPanel

    End Class

End Namespace
