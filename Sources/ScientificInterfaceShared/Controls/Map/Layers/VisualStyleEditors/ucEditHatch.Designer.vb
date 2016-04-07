' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 2 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see <http://www.gnu.org/licenses/gpl-2.0.html>. 
'
' Copyright 1991- 
'    UBC Fisheries Centre, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

Namespace Controls

    Partial Class ucEditHatch
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
            Me.nudAlpha = New cEwENumericUpDown
            Me.nudBlue = New cEwENumericUpDown
            Me.nudGreen = New cEwENumericUpDown
            Me.nudRed = New cEwENumericUpDown
            Me.tbAlpha = New ScientificInterfaceShared.Controls.ucSlider
            Me.tbBlue = New ScientificInterfaceShared.Controls.ucSlider
            Me.tbGreen = New ScientificInterfaceShared.Controls.ucSlider
            Me.tbRed = New ScientificInterfaceShared.Controls.ucSlider
            Me.pbBrush = New System.Windows.Forms.Panel
            Me.plForeColor = New System.Windows.Forms.Panel
            Me.lbAlpha = New System.Windows.Forms.Label
            Me.lbBlue = New System.Windows.Forms.Label
            Me.lbGreen = New System.Windows.Forms.Label
            Me.lbRed = New System.Windows.Forms.Label
            Me.plBackColor = New System.Windows.Forms.Panel
            CType(Me.nudAlpha, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.nudBlue, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.nudGreen, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.nudRed, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            'nudAlpha
            '
            Me.nudAlpha.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.nudAlpha.Location = New System.Drawing.Point(289, 70)
            Me.nudAlpha.Maximum = New Decimal(New Integer() {255, 0, 0, 0})
            Me.nudAlpha.Name = "nudAlpha"
            Me.nudAlpha.Size = New System.Drawing.Size(54, 20)
            Me.nudAlpha.TabIndex = 29
            '
            'nudBlue
            '
            Me.nudBlue.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.nudBlue.Location = New System.Drawing.Point(289, 47)
            Me.nudBlue.Maximum = New Decimal(New Integer() {255, 0, 0, 0})
            Me.nudBlue.Name = "nudBlue"
            Me.nudBlue.Size = New System.Drawing.Size(54, 20)
            Me.nudBlue.TabIndex = 26
            '
            'nudGreen
            '
            Me.nudGreen.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.nudGreen.Location = New System.Drawing.Point(289, 24)
            Me.nudGreen.Maximum = New Decimal(New Integer() {255, 0, 0, 0})
            Me.nudGreen.Name = "nudGreen"
            Me.nudGreen.Size = New System.Drawing.Size(54, 20)
            Me.nudGreen.TabIndex = 23
            '
            'nudRed
            '
            Me.nudRed.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.nudRed.Location = New System.Drawing.Point(289, 0)
            Me.nudRed.Maximum = New Decimal(New Integer() {255, 0, 0, 0})
            Me.nudRed.Name = "nudRed"
            Me.nudRed.Size = New System.Drawing.Size(54, 20)
            Me.nudRed.TabIndex = 20
            '
            'tbAlpha
            '
            Me.tbAlpha.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.tbAlpha.Location = New System.Drawing.Point(120, 70)
            Me.tbAlpha.Maximum = 255
            Me.tbAlpha.Minimum = 0
            Me.tbAlpha.Name = "tbAlpha"
            Me.tbAlpha.Size = New System.Drawing.Size(163, 20)
            Me.tbAlpha.TabIndex = 28
            Me.tbAlpha.Value = 50
            '
            'tbBlue
            '
            Me.tbBlue.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.tbBlue.Location = New System.Drawing.Point(120, 47)
            Me.tbBlue.Maximum = 255
            Me.tbBlue.Minimum = 0
            Me.tbBlue.Name = "tbBlue"
            Me.tbBlue.Size = New System.Drawing.Size(163, 20)
            Me.tbBlue.TabIndex = 25
            Me.tbBlue.Value = 50
            '
            'tbGreen
            '
            Me.tbGreen.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.tbGreen.Location = New System.Drawing.Point(120, 24)
            Me.tbGreen.Maximum = 255
            Me.tbGreen.Minimum = 0
            Me.tbGreen.Name = "tbGreen"
            Me.tbGreen.Size = New System.Drawing.Size(163, 20)
            Me.tbGreen.TabIndex = 22
            Me.tbGreen.Value = 50
            '
            'tbRed
            '
            Me.tbRed.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.tbRed.Location = New System.Drawing.Point(120, 0)
            Me.tbRed.Maximum = 255
            Me.tbRed.Minimum = 0
            Me.tbRed.Name = "tbRed"
            Me.tbRed.Size = New System.Drawing.Size(163, 23)
            Me.tbRed.TabIndex = 19
            Me.tbRed.Value = 50
            '
            'pbBrush
            '
            Me.pbBrush.BackColor = System.Drawing.SystemColors.Control
            Me.pbBrush.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
            Me.pbBrush.Cursor = System.Windows.Forms.Cursors.Hand
            Me.pbBrush.Location = New System.Drawing.Point(0, 0)
            Me.pbBrush.Name = "pbBrush"
            Me.pbBrush.Size = New System.Drawing.Size(36, 36)
            Me.pbBrush.TabIndex = 15
            Me.pbBrush.TabStop = True
            '
            'plForeColor
            '
            Me.plForeColor.BackColor = System.Drawing.SystemColors.Control
            Me.plForeColor.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
            Me.plForeColor.Cursor = System.Windows.Forms.Cursors.Hand
            Me.plForeColor.Location = New System.Drawing.Point(42, 0)
            Me.plForeColor.Name = "plForeColor"
            Me.plForeColor.Size = New System.Drawing.Size(22, 22)
            Me.plForeColor.TabIndex = 16
            Me.plForeColor.TabStop = True
            '
            'lbAlpha
            '
            Me.lbAlpha.AutoSize = True
            Me.lbAlpha.Location = New System.Drawing.Point(83, 72)
            Me.lbAlpha.Name = "lbAlpha"
            Me.lbAlpha.Size = New System.Drawing.Size(37, 13)
            Me.lbAlpha.TabIndex = 27
            Me.lbAlpha.Text = "&Alpha:"
            '
            'lbBlue
            '
            Me.lbBlue.AutoSize = True
            Me.lbBlue.Location = New System.Drawing.Point(84, 49)
            Me.lbBlue.Name = "lbBlue"
            Me.lbBlue.Size = New System.Drawing.Size(31, 13)
            Me.lbBlue.TabIndex = 24
            Me.lbBlue.Text = "&Blue:"
            '
            'lbGreen
            '
            Me.lbGreen.AutoSize = True
            Me.lbGreen.Location = New System.Drawing.Point(84, 26)
            Me.lbGreen.Name = "lbGreen"
            Me.lbGreen.Size = New System.Drawing.Size(39, 13)
            Me.lbGreen.TabIndex = 21
            Me.lbGreen.Text = "&Green:"
            '
            'lbRed
            '
            Me.lbRed.AutoSize = True
            Me.lbRed.Location = New System.Drawing.Point(83, 3)
            Me.lbRed.Name = "lbRed"
            Me.lbRed.Size = New System.Drawing.Size(30, 13)
            Me.lbRed.TabIndex = 18
            Me.lbRed.Text = "&Red:"
            '
            'plBackColor
            '
            Me.plBackColor.BackColor = System.Drawing.SystemColors.Control
            Me.plBackColor.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
            Me.plBackColor.Cursor = System.Windows.Forms.Cursors.Hand
            Me.plBackColor.Location = New System.Drawing.Point(56, 14)
            Me.plBackColor.Name = "plBackColor"
            Me.plBackColor.Size = New System.Drawing.Size(22, 22)
            Me.plBackColor.TabIndex = 17
            Me.plBackColor.TabStop = True
            '
            'ucEditHatch
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.nudAlpha)
            Me.Controls.Add(Me.nudBlue)
            Me.Controls.Add(Me.nudGreen)
            Me.Controls.Add(Me.nudRed)
            Me.Controls.Add(Me.tbAlpha)
            Me.Controls.Add(Me.tbBlue)
            Me.Controls.Add(Me.tbGreen)
            Me.Controls.Add(Me.tbRed)
            Me.Controls.Add(Me.pbBrush)
            Me.Controls.Add(Me.plForeColor)
            Me.Controls.Add(Me.lbAlpha)
            Me.Controls.Add(Me.lbBlue)
            Me.Controls.Add(Me.lbGreen)
            Me.Controls.Add(Me.lbRed)
            Me.Controls.Add(Me.plBackColor)
            Me.Name = "ucEditHatch"
            Me.Size = New System.Drawing.Size(343, 92)
            CType(Me.nudAlpha, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.nudBlue, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.nudGreen, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.nudRed, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Friend WithEvents nudAlpha As System.Windows.Forms.NumericUpDown
        Friend WithEvents nudBlue As System.Windows.Forms.NumericUpDown
        Friend WithEvents nudGreen As System.Windows.Forms.NumericUpDown
        Friend WithEvents nudRed As System.Windows.Forms.NumericUpDown
        Friend WithEvents tbAlpha As ucSlider
        Friend WithEvents tbBlue As ucSlider
        Friend WithEvents tbGreen As ucSlider
        Friend WithEvents tbRed As ucSlider
        Friend WithEvents pbBrush As System.Windows.Forms.Panel
        Friend WithEvents plForeColor As System.Windows.Forms.Panel
        Friend WithEvents lbAlpha As System.Windows.Forms.Label
        Friend WithEvents lbBlue As System.Windows.Forms.Label
        Friend WithEvents lbGreen As System.Windows.Forms.Label
        Friend WithEvents lbRed As System.Windows.Forms.Label
        Friend WithEvents plBackColor As System.Windows.Forms.Panel

    End Class

End Namespace
