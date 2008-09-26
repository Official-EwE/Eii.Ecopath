<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ucBrushPicker
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
        Me.lbSize = New System.Windows.Forms.Label
        Me.lbValue = New System.Windows.Forms.Label
        Me.tbValue = New System.Windows.Forms.TrackBar
        Me.nudValue = New System.Windows.Forms.NumericUpDown
        CType(Me.tbValue, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.nudValue, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'lbSize
        '
        Me.lbSize.AutoSize = True
        Me.lbSize.Location = New System.Drawing.Point(0, 8)
        Me.lbSize.Name = "lbSize"
        Me.lbSize.Size = New System.Drawing.Size(30, 13)
        Me.lbSize.TabIndex = 0
        Me.lbSize.Text = "&Size:"
        '
        'lbValue
        '
        Me.lbValue.AutoSize = True
        Me.lbValue.Location = New System.Drawing.Point(0, 34)
        Me.lbValue.Name = "lbValue"
        Me.lbValue.Size = New System.Drawing.Size(37, 13)
        Me.lbValue.TabIndex = 2
        Me.lbValue.Text = "&Value:"
        '
        'tbValue
        '
        Me.tbValue.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.tbValue.LargeChange = 1
        Me.tbValue.Location = New System.Drawing.Point(36, -2)
        Me.tbValue.Maximum = 5
        Me.tbValue.Minimum = 1
        Me.tbValue.Name = "tbValue"
        Me.tbValue.Size = New System.Drawing.Size(118, 42)
        Me.tbValue.TabIndex = 1
        Me.tbValue.TickStyle = System.Windows.Forms.TickStyle.TopLeft
        Me.tbValue.Value = 1
        '
        'nudValue
        '
        Me.nudValue.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.nudValue.DecimalPlaces = 3
        Me.nudValue.Location = New System.Drawing.Point(43, 32)
        Me.nudValue.Name = "nudValue"
        Me.nudValue.Size = New System.Drawing.Size(104, 20)
        Me.nudValue.TabIndex = 3
        '
        'ucBrushPicker
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.nudValue)
        Me.Controls.Add(Me.lbValue)
        Me.Controls.Add(Me.lbSize)
        Me.Controls.Add(Me.tbValue)
        Me.Name = "ucBrushPicker"
        Me.Size = New System.Drawing.Size(150, 62)
        CType(Me.tbValue, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.nudValue, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents lbSize As System.Windows.Forms.Label
    Friend WithEvents lbValue As System.Windows.Forms.Label
    Friend WithEvents tbValue As System.Windows.Forms.TrackBar
    Friend WithEvents nudValue As System.Windows.Forms.NumericUpDown

End Class
