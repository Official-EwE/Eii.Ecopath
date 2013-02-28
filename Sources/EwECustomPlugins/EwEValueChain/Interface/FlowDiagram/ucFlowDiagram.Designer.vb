Partial Class ucFlowDiagram
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
        Me.m_tsMain = New System.Windows.Forms.ToolStrip()
        Me.m_pbFD = New System.Windows.Forms.PictureBox()
        CType(Me.m_pbFD, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'm_tsMain
        '
        Me.m_tsMain.Location = New System.Drawing.Point(0, 0)
        Me.m_tsMain.Name = "m_tsMain"
        Me.m_tsMain.Size = New System.Drawing.Size(559, 25)
        Me.m_tsMain.TabIndex = 0
        '
        'm_pbFD
        '
        Me.m_pbFD.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_pbFD.Location = New System.Drawing.Point(0, 25)
        Me.m_pbFD.Name = "m_pbFD"
        Me.m_pbFD.Size = New System.Drawing.Size(559, 336)
        Me.m_pbFD.TabIndex = 1
        Me.m_pbFD.TabStop = False
        '
        'ucFlowDiagram
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.m_pbFD)
        Me.Controls.Add(Me.m_tsMain)
        Me.Name = "ucFlowDiagram"
        Me.Size = New System.Drawing.Size(559, 361)
        CType(Me.m_pbFD, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Private WithEvents m_tsMain As System.Windows.Forms.ToolStrip
    Private WithEvents m_pbFD As System.Windows.Forms.PictureBox

End Class
