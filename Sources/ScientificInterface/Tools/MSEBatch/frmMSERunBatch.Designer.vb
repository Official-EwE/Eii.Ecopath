
Imports ScientificInterfaceShared.Controls
Imports ScientificInterfaceShared.Forms


<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMSERunBatch
    Inherits frmEwE

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
        Me.btRunBatch = New System.Windows.Forms.Button()
        Me.txNTFMIters = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.lstMsgs = New System.Windows.Forms.ListBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'btRunBatch
        '
        Me.btRunBatch.Location = New System.Drawing.Point(12, 53)
        Me.btRunBatch.Name = "btRunBatch"
        Me.btRunBatch.Size = New System.Drawing.Size(99, 25)
        Me.btRunBatch.TabIndex = 0
        Me.btRunBatch.Text = "Run Batch"
        Me.btRunBatch.UseVisualStyleBackColor = True
        '
        'txNTFMIters
        '
        Me.txNTFMIters.Location = New System.Drawing.Point(143, 21)
        Me.txNTFMIters.Name = "txNTFMIters"
        Me.txNTFMIters.Size = New System.Drawing.Size(82, 20)
        Me.txNTFMIters.TabIndex = 1
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(11, 24)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(126, 13)
        Me.Label1.TabIndex = 2
        Me.Label1.Text = "Number of TFM iterations"
        '
        'lstMsgs
        '
        Me.lstMsgs.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lstMsgs.FormattingEnabled = True
        Me.lstMsgs.Location = New System.Drawing.Point(11, 110)
        Me.lstMsgs.Name = "lstMsgs"
        Me.lstMsgs.Size = New System.Drawing.Size(546, 160)
        Me.lstMsgs.TabIndex = 3
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(11, 94)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(55, 13)
        Me.Label2.TabIndex = 4
        Me.Label2.Text = "Messages"
        '
        'frmMSERunBatch
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(569, 288)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.lstMsgs)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.txNTFMIters)
        Me.Controls.Add(Me.btRunBatch)
        Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Name = "frmMSERunBatch"
        Me.Text = "frmMSERunBatch"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents btRunBatch As System.Windows.Forms.Button
    Friend WithEvents txNTFMIters As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents lstMsgs As System.Windows.Forms.ListBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
End Class
