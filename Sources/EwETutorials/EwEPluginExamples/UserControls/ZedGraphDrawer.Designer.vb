<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ZedGraphDrawer
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container
        Me.m_zgc = New ZedGraph.ZedGraphControl
        Me.SuspendLayout()
        Me.Dock = DockStyle.Fill
        '
        'm_zgc
        '
        Me.m_zgc.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_zgc.Location = New System.Drawing.Point(0, 0)
        Me.m_zgc.Name = "m_zgc"
        Me.m_zgc.ScrollGrace = 0
        Me.m_zgc.ScrollMaxX = 0
        Me.m_zgc.ScrollMaxY = 0
        Me.m_zgc.ScrollMaxY2 = 0
        Me.m_zgc.ScrollMinX = 0
        Me.m_zgc.ScrollMinY = 0
        Me.m_zgc.ScrollMinY2 = 0
        Me.m_zgc.Size = New System.Drawing.Size(1116, 698)
        Me.m_zgc.TabIndex = 1
        '
        'ZedGraphDrawer
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.m_zgc)
        Me.Name = "ZedGraphDrawer"
        Me.Size = New System.Drawing.Size(1116, 698)
        Me.ResumeLayout(False)

    End Sub

    Public WithEvents m_zgc As ZedGraph.ZedGraphControl

End Class
