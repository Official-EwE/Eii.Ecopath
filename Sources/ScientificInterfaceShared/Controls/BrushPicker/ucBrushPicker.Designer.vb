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
        Me.nudValue = New System.Windows.Forms.NumericUpDown
        Me.UcSlider1 = New ScientificInterfaceShared.Controls.ucSlider
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
        'UcSlider1
        '
        Me.UcSlider1.Location = New System.Drawing.Point(43, 3)
        Me.UcSlider1.Margin = New System.Windows.Forms.Padding(0)
        Me.UcSlider1.Maximum = 100
        Me.UcSlider1.Minimum = 0
        Me.UcSlider1.Name = "UcSlider1"
        Me.UcSlider1.Size = New System.Drawing.Size(104, 20)
        Me.UcSlider1.TabIndex = 4
        Me.UcSlider1.Value = 50
        '
        'ucBrushPicker
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.UcSlider1)
        Me.Controls.Add(Me.nudValue)
        Me.Controls.Add(Me.lbValue)
        Me.Controls.Add(Me.lbSize)
        Me.Name = "ucBrushPicker"
        Me.Size = New System.Drawing.Size(150, 62)
        CType(Me.nudValue, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents lbSize As System.Windows.Forms.Label
    Friend WithEvents lbValue As System.Windows.Forms.Label
    Friend WithEvents nudValue As System.Windows.Forms.NumericUpDown
    Friend WithEvents UcSlider1 As ScientificInterfaceShared.Controls.ucSlider

End Class
