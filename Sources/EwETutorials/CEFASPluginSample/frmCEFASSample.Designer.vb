Imports WeifenLuo.WinFormsUI.Docking

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmCEFASSample
    Inherits DockContent
    ' Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
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
        Me.btRunEcosim = New System.Windows.Forms.Button
        Me.txStepsPerMonth = New System.Windows.Forms.TextBox
        Me.GroupBox1 = New System.Windows.Forms.GroupBox
        Me.ckMultiThread = New System.Windows.Forms.CheckBox
        Me.GroupBox2 = New System.Windows.Forms.GroupBox
        Me.Label1 = New System.Windows.Forms.Label
        Me.lstTimesteps = New System.Windows.Forms.ListBox
        Me.Label2 = New System.Windows.Forms.Label
        Me.GroupBox1.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.SuspendLayout()
        '
        'btRunEcosim
        '
        Me.btRunEcosim.Location = New System.Drawing.Point(12, 121)
        Me.btRunEcosim.Name = "btRunEcosim"
        Me.btRunEcosim.Size = New System.Drawing.Size(214, 24)
        Me.btRunEcosim.TabIndex = 3
        Me.btRunEcosim.Text = "Run Ecosim "
        Me.btRunEcosim.UseVisualStyleBackColor = True
        '
        'txStepsPerMonth
        '
        Me.txStepsPerMonth.Location = New System.Drawing.Point(9, 47)
        Me.txStepsPerMonth.Name = "txStepsPerMonth"
        Me.txStepsPerMonth.Size = New System.Drawing.Size(83, 20)
        Me.txStepsPerMonth.TabIndex = 4
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.ckMultiThread)
        Me.GroupBox1.Location = New System.Drawing.Point(12, 12)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(214, 80)
        Me.GroupBox1.TabIndex = 5
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Run Ecosim on a thread"
        '
        'ckMultiThread
        '
        Me.ckMultiThread.AutoSize = True
        Me.ckMultiThread.Location = New System.Drawing.Point(19, 28)
        Me.ckMultiThread.Name = "ckMultiThread"
        Me.ckMultiThread.Size = New System.Drawing.Size(135, 17)
        Me.ckMultiThread.TabIndex = 1
        Me.ckMultiThread.Text = "Run Ecosim on Thread"
        Me.ckMultiThread.UseVisualStyleBackColor = True
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.Label1)
        Me.GroupBox2.Controls.Add(Me.txStepsPerMonth)
        Me.GroupBox2.Location = New System.Drawing.Point(252, 12)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(214, 80)
        Me.GroupBox2.TabIndex = 6
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Ecosim variable timestep"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(6, 28)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(200, 13)
        Me.Label1.TabIndex = 5
        Me.Label1.Text = "Set number of timesteps to run per month"
        '
        'lstTimesteps
        '
        Me.lstTimesteps.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.lstTimesteps.FormattingEnabled = True
        Me.lstTimesteps.Location = New System.Drawing.Point(12, 158)
        Me.lstTimesteps.Name = "lstTimesteps"
        Me.lstTimesteps.Size = New System.Drawing.Size(214, 212)
        Me.lstTimesteps.TabIndex = 8
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(12, 105)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(232, 13)
        Me.Label2.TabIndex = 9
        Me.Label2.Text = "Run Ecosim here or from the Scientific Interface"
        '
        'frmCEFASSample
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(582, 387)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.lstTimesteps)
        Me.Controls.Add(Me.GroupBox2)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.btRunEcosim)
        Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Name = "frmCEFASSample"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.WindowsDefaultBounds
        Me.Text = "CEFAS Sample Threaded and Varaible timesteps"
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents btRunEcosim As System.Windows.Forms.Button
    Friend WithEvents txStepsPerMonth As System.Windows.Forms.TextBox
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents ckMultiThread As System.Windows.Forms.CheckBox
    Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents lstTimesteps As System.Windows.Forms.ListBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
End Class
