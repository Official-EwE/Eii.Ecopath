<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ucPlot
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
        Me.m_plContent = New ScientificInterfaceShared.Controls.ucSmoothPanel
        Me.SuspendLayout()
        '
        'm_plContent
        '
        Me.m_plContent.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_plContent.Location = New System.Drawing.Point(0, 0)
        Me.m_plContent.Margin = New System.Windows.Forms.Padding(0)
        Me.m_plContent.Name = "m_plContent"
        Me.m_plContent.Size = New System.Drawing.Size(311, 231)
        Me.m_plContent.TabIndex = 0
        '
        'ucPlot
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoScroll = True
        Me.Controls.Add(Me.m_plContent)
        Me.Name = "ucPlot"
        Me.Size = New System.Drawing.Size(311, 231)
        Me.ResumeLayout(False)

    End Sub
    Private WithEvents m_plContent As ScientificInterfaceShared.Controls.ucSmoothPanel

End Class
