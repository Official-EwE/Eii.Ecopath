' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 3 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see https://www.gnu.org/licenses/gpl-3.0.html>. 
'
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

Namespace Controls

    Partial Class ucEditGradient
        Inherits UserControl

        'UserControl overrides dispose to clean up the component list.
        <System.Diagnostics.DebuggerNonUserCode()>
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
        <System.Diagnostics.DebuggerStepThrough()>
        Private Sub InitializeComponent()
            Me.m_nudAlpha = New ScientificInterfaceShared.Controls.cEwENumericUpDown()
            Me.m_nudBlue = New ScientificInterfaceShared.Controls.cEwENumericUpDown()
            Me.m_nudGreen = New ScientificInterfaceShared.Controls.cEwENumericUpDown()
            Me.m_nudRed = New ScientificInterfaceShared.Controls.cEwENumericUpDown()
            Me.m_slAlpha = New ScientificInterfaceShared.Controls.ucSlider()
            Me.m_slBlue = New ScientificInterfaceShared.Controls.ucSlider()
            Me.m_slGreen = New ScientificInterfaceShared.Controls.ucSlider()
            Me.m_slRed = New ScientificInterfaceShared.Controls.ucSlider()
            Me.m_plGradient = New System.Windows.Forms.Panel()
            Me.m_pbCurrentColor = New System.Windows.Forms.PictureBox()
            Me.m_lbAlpha = New System.Windows.Forms.Label()
            Me.m_lbBlue = New System.Windows.Forms.Label()
            Me.m_lbGreen = New System.Windows.Forms.Label()
            Me.m_lbRed = New System.Windows.Forms.Label()
            Me.m_btnAdd = New System.Windows.Forms.Button()
            Me.m_btnRemove = New System.Windows.Forms.Button()
            Me.m_slGradient = New ScientificInterfaceShared.Controls.ucSlider()
            Me.m_btnFlip = New System.Windows.Forms.Button()
            Me.m_tbxName = New System.Windows.Forms.TextBox()
            CType(Me.m_nudAlpha, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.m_nudBlue, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.m_nudGreen, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.m_nudRed, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.m_pbCurrentColor, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            'm_nudAlpha
            '
            Me.m_nudAlpha.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_nudAlpha.InterceptMouseWheel = ScientificInterfaceShared.Controls.cEwENumericUpDown.eInterceptMouseWheelType.WhenMouseOver
            Me.m_nudAlpha.Location = New System.Drawing.Point(283, 156)
            Me.m_nudAlpha.Maximum = New Decimal(New Integer() {255, 0, 0, 0})
            Me.m_nudAlpha.Name = "m_nudAlpha"
            Me.m_nudAlpha.Size = New System.Drawing.Size(54, 20)
            Me.m_nudAlpha.TabIndex = 19
            '
            'm_nudBlue
            '
            Me.m_nudBlue.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_nudBlue.InterceptMouseWheel = ScientificInterfaceShared.Controls.cEwENumericUpDown.eInterceptMouseWheelType.WhenMouseOver
            Me.m_nudBlue.Location = New System.Drawing.Point(283, 133)
            Me.m_nudBlue.Maximum = New Decimal(New Integer() {255, 0, 0, 0})
            Me.m_nudBlue.Name = "m_nudBlue"
            Me.m_nudBlue.Size = New System.Drawing.Size(54, 20)
            Me.m_nudBlue.TabIndex = 16
            '
            'm_nudGreen
            '
            Me.m_nudGreen.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_nudGreen.InterceptMouseWheel = ScientificInterfaceShared.Controls.cEwENumericUpDown.eInterceptMouseWheelType.WhenMouseOver
            Me.m_nudGreen.Location = New System.Drawing.Point(283, 110)
            Me.m_nudGreen.Maximum = New Decimal(New Integer() {255, 0, 0, 0})
            Me.m_nudGreen.Name = "m_nudGreen"
            Me.m_nudGreen.Size = New System.Drawing.Size(54, 20)
            Me.m_nudGreen.TabIndex = 13
            '
            'm_nudRed
            '
            Me.m_nudRed.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_nudRed.InterceptMouseWheel = ScientificInterfaceShared.Controls.cEwENumericUpDown.eInterceptMouseWheelType.WhenMouseOver
            Me.m_nudRed.Location = New System.Drawing.Point(283, 86)
            Me.m_nudRed.Maximum = New Decimal(New Integer() {255, 0, 0, 0})
            Me.m_nudRed.Name = "m_nudRed"
            Me.m_nudRed.Size = New System.Drawing.Size(54, 20)
            Me.m_nudRed.TabIndex = 10
            '
            'm_slAlpha
            '
            Me.m_slAlpha.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_slAlpha.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
            Me.m_slAlpha.CurrentKnob = 0
            Me.m_slAlpha.Location = New System.Drawing.Point(88, 156)
            Me.m_slAlpha.Maximum = 255
            Me.m_slAlpha.Minimum = 0
            Me.m_slAlpha.Name = "m_slAlpha"
            Me.m_slAlpha.NumKnobs = 1
            Me.m_slAlpha.Size = New System.Drawing.Size(189, 20)
            Me.m_slAlpha.TabIndex = 18
            '
            'm_slBlue
            '
            Me.m_slBlue.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_slBlue.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
            Me.m_slBlue.CurrentKnob = 0
            Me.m_slBlue.Location = New System.Drawing.Point(88, 133)
            Me.m_slBlue.Maximum = 255
            Me.m_slBlue.Minimum = 0
            Me.m_slBlue.Name = "m_slBlue"
            Me.m_slBlue.NumKnobs = 1
            Me.m_slBlue.Size = New System.Drawing.Size(189, 20)
            Me.m_slBlue.TabIndex = 15
            '
            'm_slGreen
            '
            Me.m_slGreen.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_slGreen.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
            Me.m_slGreen.CurrentKnob = 0
            Me.m_slGreen.Location = New System.Drawing.Point(88, 110)
            Me.m_slGreen.Maximum = 255
            Me.m_slGreen.Minimum = 0
            Me.m_slGreen.Name = "m_slGreen"
            Me.m_slGreen.NumKnobs = 1
            Me.m_slGreen.Size = New System.Drawing.Size(189, 20)
            Me.m_slGreen.TabIndex = 12
            '
            'm_slRed
            '
            Me.m_slRed.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_slRed.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
            Me.m_slRed.CurrentKnob = 0
            Me.m_slRed.Location = New System.Drawing.Point(88, 86)
            Me.m_slRed.Maximum = 255
            Me.m_slRed.Minimum = 0
            Me.m_slRed.Name = "m_slRed"
            Me.m_slRed.NumKnobs = 1
            Me.m_slRed.Size = New System.Drawing.Size(189, 23)
            Me.m_slRed.TabIndex = 9
            '
            'm_plGradient
            '
            Me.m_plGradient.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_plGradient.BackColor = System.Drawing.SystemColors.Control
            Me.m_plGradient.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
            Me.m_plGradient.Cursor = System.Windows.Forms.Cursors.Hand
            Me.m_plGradient.Location = New System.Drawing.Point(4, 29)
            Me.m_plGradient.Name = "m_plGradient"
            Me.m_plGradient.Size = New System.Drawing.Size(274, 22)
            Me.m_plGradient.TabIndex = 3
            Me.m_plGradient.TabStop = True
            '
            'm_pbCurrentColor
            '
            Me.m_pbCurrentColor.BackColor = System.Drawing.Color.PaleGreen
            Me.m_pbCurrentColor.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
            Me.m_pbCurrentColor.Cursor = System.Windows.Forms.Cursors.Hand
            Me.m_pbCurrentColor.Location = New System.Drawing.Point(7, 86)
            Me.m_pbCurrentColor.Name = "m_pbCurrentColor"
            Me.m_pbCurrentColor.Size = New System.Drawing.Size(23, 23)
            Me.m_pbCurrentColor.TabIndex = 7
            Me.m_pbCurrentColor.TabStop = False
            '
            'm_lbAlpha
            '
            Me.m_lbAlpha.AutoSize = True
            Me.m_lbAlpha.Location = New System.Drawing.Point(42, 158)
            Me.m_lbAlpha.Name = "m_lbAlpha"
            Me.m_lbAlpha.Size = New System.Drawing.Size(37, 13)
            Me.m_lbAlpha.TabIndex = 17
            Me.m_lbAlpha.Text = "&Alpha:"
            '
            'm_lbBlue
            '
            Me.m_lbBlue.AutoSize = True
            Me.m_lbBlue.Location = New System.Drawing.Point(43, 135)
            Me.m_lbBlue.Name = "m_lbBlue"
            Me.m_lbBlue.Size = New System.Drawing.Size(31, 13)
            Me.m_lbBlue.TabIndex = 14
            Me.m_lbBlue.Text = "&Blue:"
            '
            'm_lbGreen
            '
            Me.m_lbGreen.AutoSize = True
            Me.m_lbGreen.Location = New System.Drawing.Point(43, 112)
            Me.m_lbGreen.Name = "m_lbGreen"
            Me.m_lbGreen.Size = New System.Drawing.Size(39, 13)
            Me.m_lbGreen.TabIndex = 11
            Me.m_lbGreen.Text = "&Green:"
            '
            'm_lbRed
            '
            Me.m_lbRed.AutoSize = True
            Me.m_lbRed.Location = New System.Drawing.Point(42, 89)
            Me.m_lbRed.Name = "m_lbRed"
            Me.m_lbRed.Size = New System.Drawing.Size(30, 13)
            Me.m_lbRed.TabIndex = 8
            Me.m_lbRed.Text = "&Red:"
            '
            'm_btnAdd
            '
            Me.m_btnAdd.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_btnAdd.Location = New System.Drawing.Point(284, 57)
            Me.m_btnAdd.Name = "m_btnAdd"
            Me.m_btnAdd.Size = New System.Drawing.Size(23, 23)
            Me.m_btnAdd.TabIndex = 4
            Me.m_btnAdd.Text = "+"
            Me.m_btnAdd.UseVisualStyleBackColor = True
            '
            'm_btnRemove
            '
            Me.m_btnRemove.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_btnRemove.Location = New System.Drawing.Point(313, 57)
            Me.m_btnRemove.Name = "m_btnRemove"
            Me.m_btnRemove.Size = New System.Drawing.Size(23, 23)
            Me.m_btnRemove.TabIndex = 5
            Me.m_btnRemove.Text = "-"
            Me.m_btnRemove.UseVisualStyleBackColor = True
            '
            'm_slGradient
            '
            Me.m_slGradient.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_slGradient.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
            Me.m_slGradient.CurrentKnob = 0
            Me.m_slGradient.Location = New System.Drawing.Point(4, 57)
            Me.m_slGradient.Maximum = 100
            Me.m_slGradient.Minimum = 0
            Me.m_slGradient.Name = "m_slGradient"
            Me.m_slGradient.NumKnobs = 1
            Me.m_slGradient.Size = New System.Drawing.Size(274, 23)
            Me.m_slGradient.TabIndex = 6
            '
            'm_btnFlip
            '
            Me.m_btnFlip.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_btnFlip.Location = New System.Drawing.Point(284, 28)
            Me.m_btnFlip.Name = "m_btnFlip"
            Me.m_btnFlip.Size = New System.Drawing.Size(52, 23)
            Me.m_btnFlip.TabIndex = 20
            Me.m_btnFlip.Text = "Flip"
            Me.m_btnFlip.UseVisualStyleBackColor = True
            '
            'm_tbxName
            '
            Me.m_tbxName.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_tbxName.Location = New System.Drawing.Point(4, 3)
            Me.m_tbxName.Name = "m_tbxName"
            Me.m_tbxName.Size = New System.Drawing.Size(332, 20)
            Me.m_tbxName.TabIndex = 22
            '
            'ucEditGradient
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
            Me.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
            Me.Controls.Add(Me.m_tbxName)
            Me.Controls.Add(Me.m_btnFlip)
            Me.Controls.Add(Me.m_btnRemove)
            Me.Controls.Add(Me.m_btnAdd)
            Me.Controls.Add(Me.m_nudAlpha)
            Me.Controls.Add(Me.m_nudBlue)
            Me.Controls.Add(Me.m_nudGreen)
            Me.Controls.Add(Me.m_nudRed)
            Me.Controls.Add(Me.m_slAlpha)
            Me.Controls.Add(Me.m_slBlue)
            Me.Controls.Add(Me.m_slGreen)
            Me.Controls.Add(Me.m_slGradient)
            Me.Controls.Add(Me.m_slRed)
            Me.Controls.Add(Me.m_plGradient)
            Me.Controls.Add(Me.m_pbCurrentColor)
            Me.Controls.Add(Me.m_lbAlpha)
            Me.Controls.Add(Me.m_lbBlue)
            Me.Controls.Add(Me.m_lbGreen)
            Me.Controls.Add(Me.m_lbRed)
            Me.Name = "ucEditGradient"
            Me.Size = New System.Drawing.Size(340, 183)
            CType(Me.m_nudAlpha, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.m_nudBlue, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.m_nudGreen, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.m_nudRed, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.m_pbCurrentColor, System.ComponentModel.ISupportInitialize).EndInit()
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
        Private WithEvents m_nudAlpha As ScientificInterfaceShared.Controls.cEwENumericUpDown
        Private WithEvents m_nudBlue As ScientificInterfaceShared.Controls.cEwENumericUpDown
        Private WithEvents m_nudGreen As ScientificInterfaceShared.Controls.cEwENumericUpDown
        Private WithEvents m_nudRed As ScientificInterfaceShared.Controls.cEwENumericUpDown
        Private WithEvents m_pbCurrentColor As System.Windows.Forms.PictureBox
        Private WithEvents m_plGradient As System.Windows.Forms.Panel
        Private WithEvents m_btnAdd As System.Windows.Forms.Button
        Private WithEvents m_btnRemove As System.Windows.Forms.Button
        Private WithEvents m_slGradient As ScientificInterfaceShared.Controls.ucSlider
        Private WithEvents m_btnFlip As System.Windows.Forms.Button
        Private WithEvents m_tbxName As TextBox
    End Class

End Namespace
