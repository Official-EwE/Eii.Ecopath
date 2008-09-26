Imports WeifenLuo.WinFormsUI.Docking

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmNetworkMain
    Inherits DockContent

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
        Me.btTest = New System.Windows.Forms.Button
        Me.Label1 = New System.Windows.Forms.Label
        Me.lbTestMessage = New System.Windows.Forms.Label
        Me.prgProgress = New System.Windows.Forms.ProgressBar
        Me.SuspendLayout()
        '
        'btTest
        '
        Me.btTest.Location = New System.Drawing.Point(12, 25)
        Me.btTest.Name = "btTest"
        Me.btTest.Size = New System.Drawing.Size(108, 24)
        Me.btTest.TabIndex = 0
        Me.btTest.Text = "Test Network"
        Me.btTest.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(12, 9)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(288, 13)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "This is a temporary interface for the Network Analysis Plugin"
        '
        'lbTestMessage
        '
        Me.lbTestMessage.AutoSize = True
        Me.lbTestMessage.Location = New System.Drawing.Point(27, 68)
        Me.lbTestMessage.Name = "lbTestMessage"
        Me.lbTestMessage.Size = New System.Drawing.Size(78, 13)
        Me.lbTestMessage.TabIndex = 2
        Me.lbTestMessage.Text = "Test messages"
        '
        'prgProgress
        '
        Me.prgProgress.Location = New System.Drawing.Point(31, 92)
        Me.prgProgress.Maximum = 1000
        Me.prgProgress.Name = "prgProgress"
        Me.prgProgress.Size = New System.Drawing.Size(222, 20)
        Me.prgProgress.Step = 1
        Me.prgProgress.TabIndex = 3
        '
        'frmNetworkMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(368, 266)
        Me.Controls.Add(Me.prgProgress)
        Me.Controls.Add(Me.lbTestMessage)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.btTest)
        Me.Name = "frmNetworkMain"
        Me.Text = "Network Analysis Plugin Interface"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents btTest As System.Windows.Forms.Button
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents lbTestMessage As System.Windows.Forms.Label
    Friend WithEvents prgProgress As System.Windows.Forms.ProgressBar
End Class
