<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ucGridView
    Inherits System.Windows.Forms.UserControl

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.m_ts = New ScientificInterfaceShared.Controls.cEwEToolstrip()
        Me.m_plGrid = New System.Windows.Forms.Panel()
        Me.SuspendLayout()
        '
        'm_ts
        '
        Me.m_ts.Location = New System.Drawing.Point(0, 0)
        Me.m_ts.Name = "m_ts"
        Me.m_ts.Size = New System.Drawing.Size(695, 25)
        Me.m_ts.TabIndex = 0
        '
        'm_plGrid
        '
        Me.m_plGrid.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_plGrid.Location = New System.Drawing.Point(0, 25)
        Me.m_plGrid.Name = "m_plGrid"
        Me.m_plGrid.Size = New System.Drawing.Size(695, 342)
        Me.m_plGrid.TabIndex = 1
        '
        'ucGridView
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.m_plGrid)
        Me.Controls.Add(Me.m_ts)
        Me.Name = "ucGridView"
        Me.Size = New System.Drawing.Size(695, 367)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Private WithEvents m_ts As ScientificInterfaceShared.Controls.cEwEToolstrip
    Private WithEvents m_plGrid As System.Windows.Forms.Panel

End Class
