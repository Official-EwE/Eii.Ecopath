Namespace Controls

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class ucEditGradient
        Inherits ucEditVisualStyle

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
            Me.m_nudAlpha = New ScientificInterfaceShared.Controls.cEwENumericUpDown()
            Me.m_nudBlue = New ScientificInterfaceShared.Controls.cEwENumericUpDown()
            Me.m_nudGreen = New ScientificInterfaceShared.Controls.cEwENumericUpDown()
            Me.m_nudRed = New ScientificInterfaceShared.Controls.cEwENumericUpDown()
            Me.m_slAlpha = New ScientificInterfaceShared.Controls.ucSlider()
            Me.m_slBlue = New ScientificInterfaceShared.Controls.ucSlider()
            Me.m_slGreen = New ScientificInterfaceShared.Controls.ucSlider()
            Me.m_slRed = New ScientificInterfaceShared.Controls.ucSlider()
            Me.m_plPreview = New System.Windows.Forms.Panel()
            Me.m_plStart = New System.Windows.Forms.Panel()
            Me.m_lbAlpha = New System.Windows.Forms.Label()
            Me.m_lbBlue = New System.Windows.Forms.Label()
            Me.m_lbGreen = New System.Windows.Forms.Label()
            Me.m_lbRed = New System.Windows.Forms.Label()
            Me.m_plEnd = New System.Windows.Forms.Panel()
            CType(Me.m_nudAlpha, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.m_nudBlue, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.m_nudGreen, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.m_nudRed, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            'm_nudAlpha
            '
            Me.m_nudAlpha.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_nudAlpha.Location = New System.Drawing.Point(283, 101)
            Me.m_nudAlpha.Maximum = New Decimal(New Integer() {255, 0, 0, 0})
            Me.m_nudAlpha.Name = "m_nudAlpha"
            Me.m_nudAlpha.Size = New System.Drawing.Size(54, 20)
            Me.m_nudAlpha.TabIndex = 29
            '
            'm_nudBlue
            '
            Me.m_nudBlue.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_nudBlue.Location = New System.Drawing.Point(283, 78)
            Me.m_nudBlue.Maximum = New Decimal(New Integer() {255, 0, 0, 0})
            Me.m_nudBlue.Name = "m_nudBlue"
            Me.m_nudBlue.Size = New System.Drawing.Size(54, 20)
            Me.m_nudBlue.TabIndex = 26
            '
            'm_nudGreen
            '
            Me.m_nudGreen.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_nudGreen.Location = New System.Drawing.Point(283, 55)
            Me.m_nudGreen.Maximum = New Decimal(New Integer() {255, 0, 0, 0})
            Me.m_nudGreen.Name = "m_nudGreen"
            Me.m_nudGreen.Size = New System.Drawing.Size(54, 20)
            Me.m_nudGreen.TabIndex = 23
            '
            'm_nudRed
            '
            Me.m_nudRed.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_nudRed.Location = New System.Drawing.Point(283, 31)
            Me.m_nudRed.Maximum = New Decimal(New Integer() {255, 0, 0, 0})
            Me.m_nudRed.Name = "m_nudRed"
            Me.m_nudRed.Size = New System.Drawing.Size(54, 20)
            Me.m_nudRed.TabIndex = 20
            '
            'm_slAlpha
            '
            Me.m_slAlpha.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_slAlpha.Location = New System.Drawing.Point(49, 101)
            Me.m_slAlpha.Maximum = 255
            Me.m_slAlpha.Minimum = 0
            Me.m_slAlpha.Name = "m_slAlpha"
            Me.m_slAlpha.Size = New System.Drawing.Size(228, 20)
            Me.m_slAlpha.TabIndex = 28
            Me.m_slAlpha.Value = 50
            '
            'm_slBlue
            '
            Me.m_slBlue.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_slBlue.Location = New System.Drawing.Point(49, 78)
            Me.m_slBlue.Maximum = 255
            Me.m_slBlue.Minimum = 0
            Me.m_slBlue.Name = "m_slBlue"
            Me.m_slBlue.Size = New System.Drawing.Size(228, 20)
            Me.m_slBlue.TabIndex = 25
            Me.m_slBlue.Value = 50
            '
            'm_slGreen
            '
            Me.m_slGreen.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_slGreen.Location = New System.Drawing.Point(49, 55)
            Me.m_slGreen.Maximum = 255
            Me.m_slGreen.Minimum = 0
            Me.m_slGreen.Name = "m_slGreen"
            Me.m_slGreen.Size = New System.Drawing.Size(228, 20)
            Me.m_slGreen.TabIndex = 22
            Me.m_slGreen.Value = 50
            '
            'm_slRed
            '
            Me.m_slRed.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_slRed.Location = New System.Drawing.Point(49, 31)
            Me.m_slRed.Maximum = 255
            Me.m_slRed.Minimum = 0
            Me.m_slRed.Name = "m_slRed"
            Me.m_slRed.Size = New System.Drawing.Size(228, 23)
            Me.m_slRed.TabIndex = 19
            Me.m_slRed.Value = 50
            '
            'm_plPreview
            '
            Me.m_plPreview.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_plPreview.BackColor = System.Drawing.SystemColors.Control
            Me.m_plPreview.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
            Me.m_plPreview.Cursor = System.Windows.Forms.Cursors.Hand
            Me.m_plPreview.Location = New System.Drawing.Point(31, 3)
            Me.m_plPreview.Name = "m_plPreview"
            Me.m_plPreview.Size = New System.Drawing.Size(278, 22)
            Me.m_plPreview.TabIndex = 15
            Me.m_plPreview.TabStop = True
            '
            'm_plStart
            '
            Me.m_plStart.BackColor = System.Drawing.SystemColors.Control
            Me.m_plStart.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
            Me.m_plStart.Cursor = System.Windows.Forms.Cursors.Hand
            Me.m_plStart.Location = New System.Drawing.Point(3, 3)
            Me.m_plStart.Name = "m_plStart"
            Me.m_plStart.Size = New System.Drawing.Size(22, 22)
            Me.m_plStart.TabIndex = 16
            Me.m_plStart.TabStop = True
            '
            'm_lbAlpha
            '
            Me.m_lbAlpha.AutoSize = True
            Me.m_lbAlpha.Location = New System.Drawing.Point(3, 103)
            Me.m_lbAlpha.Name = "m_lbAlpha"
            Me.m_lbAlpha.Size = New System.Drawing.Size(37, 13)
            Me.m_lbAlpha.TabIndex = 27
            Me.m_lbAlpha.Text = "&Alpha:"
            '
            'm_lbBlue
            '
            Me.m_lbBlue.AutoSize = True
            Me.m_lbBlue.Location = New System.Drawing.Point(4, 80)
            Me.m_lbBlue.Name = "m_lbBlue"
            Me.m_lbBlue.Size = New System.Drawing.Size(31, 13)
            Me.m_lbBlue.TabIndex = 24
            Me.m_lbBlue.Text = "&Blue:"
            '
            'm_lbGreen
            '
            Me.m_lbGreen.AutoSize = True
            Me.m_lbGreen.Location = New System.Drawing.Point(4, 57)
            Me.m_lbGreen.Name = "m_lbGreen"
            Me.m_lbGreen.Size = New System.Drawing.Size(39, 13)
            Me.m_lbGreen.TabIndex = 21
            Me.m_lbGreen.Text = "&Green:"
            '
            'm_lbRed
            '
            Me.m_lbRed.AutoSize = True
            Me.m_lbRed.Location = New System.Drawing.Point(3, 34)
            Me.m_lbRed.Name = "m_lbRed"
            Me.m_lbRed.Size = New System.Drawing.Size(30, 13)
            Me.m_lbRed.TabIndex = 18
            Me.m_lbRed.Text = "&Red:"
            '
            'm_plEnd
            '
            Me.m_plEnd.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_plEnd.BackColor = System.Drawing.SystemColors.Control
            Me.m_plEnd.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
            Me.m_plEnd.Cursor = System.Windows.Forms.Cursors.Hand
            Me.m_plEnd.Location = New System.Drawing.Point(315, 3)
            Me.m_plEnd.Name = "m_plEnd"
            Me.m_plEnd.Size = New System.Drawing.Size(22, 22)
            Me.m_plEnd.TabIndex = 17
            Me.m_plEnd.TabStop = True
            '
            'ucEditGradient
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.m_nudAlpha)
            Me.Controls.Add(Me.m_nudBlue)
            Me.Controls.Add(Me.m_nudGreen)
            Me.Controls.Add(Me.m_nudRed)
            Me.Controls.Add(Me.m_slAlpha)
            Me.Controls.Add(Me.m_slBlue)
            Me.Controls.Add(Me.m_slGreen)
            Me.Controls.Add(Me.m_slRed)
            Me.Controls.Add(Me.m_plPreview)
            Me.Controls.Add(Me.m_plStart)
            Me.Controls.Add(Me.m_lbAlpha)
            Me.Controls.Add(Me.m_lbBlue)
            Me.Controls.Add(Me.m_lbGreen)
            Me.Controls.Add(Me.m_lbRed)
            Me.Controls.Add(Me.m_plEnd)
            Me.Name = "ucEditGradient"
            Me.Size = New System.Drawing.Size(340, 130)
            CType(Me.m_nudAlpha, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.m_nudBlue, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.m_nudGreen, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.m_nudRed, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents m_slAlpha As ucSlider
        Private WithEvents m_slBlue As ucSlider
        Private WithEvents m_slGreen As ucSlider
        Private WithEvents m_slRed As ucSlider
        Private WithEvents m_lbAlpha As System.Windows.Forms.Label
        Private WithEvents m_lbBlue As System.Windows.Forms.Label
        Private WithEvents m_lbGreen As System.Windows.Forms.Label
        Private WithEvents m_lbRed As System.Windows.Forms.Label
        Private WithEvents m_plEnd As System.Windows.Forms.Panel
        Private WithEvents m_nudAlpha As ScientificInterfaceShared.Controls.cEwENumericUpDown
        Private WithEvents m_nudBlue As ScientificInterfaceShared.Controls.cEwENumericUpDown
        Private WithEvents m_nudGreen As ScientificInterfaceShared.Controls.cEwENumericUpDown
        Private WithEvents m_nudRed As ScientificInterfaceShared.Controls.cEwENumericUpDown
        Private WithEvents m_plStart As System.Windows.Forms.Panel
        Private WithEvents m_plPreview As System.Windows.Forms.Panel

    End Class

End Namespace
