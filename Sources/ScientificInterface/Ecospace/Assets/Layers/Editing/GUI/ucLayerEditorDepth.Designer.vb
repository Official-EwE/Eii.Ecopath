Namespace Ecospace.Basemap.Layers

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class ucLayerEditorDepth
        Inherits ucLayerEditorDefault

        'Required by the Windows Form Designer
        Private components As System.ComponentModel.IContainer

        'NOTE: The following procedure is required by the Windows Form Designer
        'It can be modified using the Windows Form Designer.  
        'Do not modify it using the code editor.
        <System.Diagnostics.DebuggerStepThrough()> _
        Private Sub InitializeComponent()
            Me.m_rbWater = New System.Windows.Forms.RadioButton
            Me.m_rbLand = New System.Windows.Forms.RadioButton
            Me.m_nudDepth = New System.Windows.Forms.NumericUpDown
            Me.m_pbPreviewWater = New System.Windows.Forms.PictureBox
            Me.m_pbPreviewLand = New System.Windows.Forms.PictureBox
            Me.m_cbProtectCoastline = New System.Windows.Forms.CheckBox
            CType(Me.m_nudDepth, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.m_pbPreviewWater, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.m_pbPreviewLand, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            'm_rbWater
            '
            Me.m_rbWater.AutoSize = True
            Me.m_rbWater.Location = New System.Drawing.Point(3, 42)
            Me.m_rbWater.Name = "m_rbWater"
            Me.m_rbWater.Size = New System.Drawing.Size(54, 17)
            Me.m_rbWater.TabIndex = 2
            Me.m_rbWater.TabStop = True
            Me.m_rbWater.Text = "&Water"
            Me.m_rbWater.UseVisualStyleBackColor = True
            '
            'm_rbLand
            '
            Me.m_rbLand.AutoSize = True
            Me.m_rbLand.Location = New System.Drawing.Point(3, 67)
            Me.m_rbLand.Name = "m_rbLand"
            Me.m_rbLand.Size = New System.Drawing.Size(49, 17)
            Me.m_rbLand.TabIndex = 4
            Me.m_rbLand.TabStop = True
            Me.m_rbLand.Text = "&Land"
            Me.m_rbLand.UseVisualStyleBackColor = True
            '
            'm_nudDepth
            '
            Me.m_nudDepth.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_nudDepth.Location = New System.Drawing.Point(65, 42)
            Me.m_nudDepth.Maximum = New Decimal(New Integer() {10000, 0, 0, 0})
            Me.m_nudDepth.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
            Me.m_nudDepth.Name = "m_nudDepth"
            Me.m_nudDepth.Size = New System.Drawing.Size(99, 20)
            Me.m_nudDepth.TabIndex = 3
            Me.m_nudDepth.Value = New Decimal(New Integer() {1, 0, 0, 0})
            '
            'm_pbPreviewWater
            '
            Me.m_pbPreviewWater.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_pbPreviewWater.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
            Me.m_pbPreviewWater.ImeMode = System.Windows.Forms.ImeMode.NoControl
            Me.m_pbPreviewWater.Location = New System.Drawing.Point(170, 42)
            Me.m_pbPreviewWater.Name = "m_pbPreviewWater"
            Me.m_pbPreviewWater.Size = New System.Drawing.Size(27, 20)
            Me.m_pbPreviewWater.TabIndex = 5
            Me.m_pbPreviewWater.TabStop = False
            '
            'm_pbPreviewLand
            '
            Me.m_pbPreviewLand.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_pbPreviewLand.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
            Me.m_pbPreviewLand.ImeMode = System.Windows.Forms.ImeMode.NoControl
            Me.m_pbPreviewLand.Location = New System.Drawing.Point(170, 68)
            Me.m_pbPreviewLand.Name = "m_pbPreviewLand"
            Me.m_pbPreviewLand.Size = New System.Drawing.Size(27, 21)
            Me.m_pbPreviewLand.TabIndex = 5
            Me.m_pbPreviewLand.TabStop = False
            '
            'm_cbProtectCoastline
            '
            Me.m_cbProtectCoastline.AutoSize = True
            Me.m_cbProtectCoastline.Location = New System.Drawing.Point(65, 93)
            Me.m_cbProtectCoastline.Name = "m_cbProtectCoastline"
            Me.m_cbProtectCoastline.Size = New System.Drawing.Size(126, 17)
            Me.m_cbProtectCoastline.TabIndex = 1
            Me.m_cbProtectCoastline.Text = "Do not edit coast line"
            Me.m_cbProtectCoastline.UseVisualStyleBackColor = True
            '
            'ucLayerEditorDepth
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.m_rbWater)
            Me.Controls.Add(Me.m_pbPreviewWater)
            Me.Controls.Add(Me.m_pbPreviewLand)
            Me.Controls.Add(Me.m_rbLand)
            Me.Controls.Add(Me.m_nudDepth)
            Me.Controls.Add(Me.m_cbProtectCoastline)
            Me.Name = "ucLayerEditorDepth"
            Me.Size = New System.Drawing.Size(200, 113)
            Me.Controls.SetChildIndex(Me.m_cbProtectCoastline, 0)
            Me.Controls.SetChildIndex(Me.m_nudDepth, 0)
            Me.Controls.SetChildIndex(Me.m_rbLand, 0)
            Me.Controls.SetChildIndex(Me.m_pbPreviewLand, 0)
            Me.Controls.SetChildIndex(Me.m_pbPreviewWater, 0)
            Me.Controls.SetChildIndex(Me.m_rbWater, 0)
            CType(Me.m_nudDepth, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.m_pbPreviewWater, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.m_pbPreviewLand, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents m_rbWater As System.Windows.Forms.RadioButton
        Private WithEvents m_rbLand As System.Windows.Forms.RadioButton
        Private WithEvents m_nudDepth As System.Windows.Forms.NumericUpDown
        Protected WithEvents m_pbPreviewWater As System.Windows.Forms.PictureBox
        Protected WithEvents m_pbPreviewLand As System.Windows.Forms.PictureBox
        Private WithEvents m_cbProtectCoastline As System.Windows.Forms.CheckBox

    End Class

End Namespace