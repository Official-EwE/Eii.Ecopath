
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmIndicesWithPPREst
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
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
        Me.ZedGraphControl = New ZedGraph.ZedGraphControl
        Me.btnOK = New System.Windows.Forms.Button
        Me.SuspendLayout()
        '
        'ZedGraphControl
        '
        Me.ZedGraphControl.Location = New System.Drawing.Point(29, 26)
        Me.ZedGraphControl.Name = "ZedGraphControl"
        Me.ZedGraphControl.ScrollGrace = 0
        Me.ZedGraphControl.ScrollMaxX = 0
        Me.ZedGraphControl.ScrollMaxY = 0
        Me.ZedGraphControl.ScrollMaxY2 = 0
        Me.ZedGraphControl.ScrollMinX = 0
        Me.ZedGraphControl.ScrollMinY = 0
        Me.ZedGraphControl.ScrollMinY2 = 0
        Me.ZedGraphControl.Size = New System.Drawing.Size(601, 558)
        Me.ZedGraphControl.TabIndex = 0
        '
        'btnOK
        '
        Me.btnOK.Location = New System.Drawing.Point(555, 601)
        Me.btnOK.Name = "btnOK"
        Me.btnOK.Size = New System.Drawing.Size(75, 23)
        Me.btnOK.TabIndex = 1
        Me.btnOK.Text = "&OK"
        Me.btnOK.UseVisualStyleBackColor = True
        '
        'frmIndicesWithPPREst
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(654, 636)
        Me.Controls.Add(Me.btnOK)
        Me.Controls.Add(Me.ZedGraphControl)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.Name = "frmIndicesWithPPREst"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Ecosim network analysis indices (with primary production required estimate)"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents ZedGraphControl As ZedGraph.ZedGraphControl
    Friend WithEvents btnOK As System.Windows.Forms.Button
End Class

