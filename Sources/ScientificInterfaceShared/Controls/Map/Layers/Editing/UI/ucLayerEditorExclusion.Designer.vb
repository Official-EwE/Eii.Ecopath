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
' Copyright 1991- UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'

Namespace Controls.Map.Layers

    Partial Class ucLayerEditorExclusion
        Inherits ucLayerEditorDefault

        'Required by the Windows Form Designer
        Private components As System.ComponentModel.IContainer

        'NOTE: The following procedure is required by the Windows Form Designer
        'It can be modified using the Windows Form Designer.  
        'Do not modify it using the code editor.
        <System.Diagnostics.DebuggerStepThrough()> _
        Private Sub InitializeComponent()
            Me.m_lblDepth = New System.Windows.Forms.Label()
            Me.m_nudDepth = New ScientificInterfaceShared.Controls.cEwENumericUpDown()
            Me.m_btnClear = New System.Windows.Forms.Button()
            Me.m_btnSet = New System.Windows.Forms.Button()
            Me.m_cbAlwaysShowExcluded = New System.Windows.Forms.CheckBox()
            Me.m_tlpButtons = New System.Windows.Forms.TableLayoutPanel()
            Me.m_btnInvert = New System.Windows.Forms.Button()
            CType(Me.m_nudDepth, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.m_tlpButtons.SuspendLayout()
            Me.SuspendLayout()
            '
            'm_lblDepth
            '
            Me.m_lblDepth.AutoSize = True
            Me.m_lblDepth.Location = New System.Drawing.Point(0, 128)
            Me.m_lblDepth.Name = "m_lblDepth"
            Me.m_lblDepth.Size = New System.Drawing.Size(51, 13)
            Me.m_lblDepth.TabIndex = 2
            Me.m_lblDepth.Text = "&Depth >="
            '
            'm_nudDepth
            '
            Me.m_nudDepth.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_nudDepth.InterceptMouseWheel = ScientificInterfaceShared.Controls.cEwENumericUpDown.eInterceptMouseWheelType.WhenMouseOver
            Me.m_nudDepth.Location = New System.Drawing.Point(68, 126)
            Me.m_nudDepth.Maximum = New Decimal(New Integer() {10000, 0, 0, 0})
            Me.m_nudDepth.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
            Me.m_nudDepth.Name = "m_nudDepth"
            Me.m_nudDepth.Size = New System.Drawing.Size(73, 20)
            Me.m_nudDepth.TabIndex = 3
            Me.m_nudDepth.Value = New Decimal(New Integer() {1, 0, 0, 0})
            '
            'm_btnClear
            '
            Me.m_btnClear.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_btnClear.Location = New System.Drawing.Point(0, 0)
            Me.m_btnClear.Margin = New System.Windows.Forms.Padding(0, 0, 3, 0)
            Me.m_btnClear.Name = "m_btnClear"
            Me.m_btnClear.Size = New System.Drawing.Size(94, 23)
            Me.m_btnClear.TabIndex = 0
            Me.m_btnClear.Text = "&Clear"
            Me.m_btnClear.UseVisualStyleBackColor = True
            '
            'm_btnSet
            '
            Me.m_btnSet.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_btnSet.Location = New System.Drawing.Point(147, 123)
            Me.m_btnSet.Name = "m_btnSet"
            Me.m_btnSet.Size = New System.Drawing.Size(50, 23)
            Me.m_btnSet.TabIndex = 4
            Me.m_btnSet.Text = "&Set"
            Me.m_btnSet.UseVisualStyleBackColor = True
            '
            'm_cbAlwaysShowExcluded
            '
            Me.m_cbAlwaysShowExcluded.AutoSize = True
            Me.m_cbAlwaysShowExcluded.Location = New System.Drawing.Point(3, 181)
            Me.m_cbAlwaysShowExcluded.Name = "m_cbAlwaysShowExcluded"
            Me.m_cbAlwaysShowExcluded.Size = New System.Drawing.Size(157, 17)
            Me.m_cbAlwaysShowExcluded.TabIndex = 6
            Me.m_cbAlwaysShowExcluded.Text = "&Always show excluded cells"
            Me.m_cbAlwaysShowExcluded.UseVisualStyleBackColor = True
            '
            'm_tlpButtons
            '
            Me.m_tlpButtons.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_tlpButtons.ColumnCount = 2
            Me.m_tlpButtons.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
            Me.m_tlpButtons.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
            Me.m_tlpButtons.Controls.Add(Me.m_btnClear, 0, 0)
            Me.m_tlpButtons.Controls.Add(Me.m_btnInvert, 1, 0)
            Me.m_tlpButtons.Location = New System.Drawing.Point(3, 152)
            Me.m_tlpButtons.Name = "m_tlpButtons"
            Me.m_tlpButtons.RowCount = 1
            Me.m_tlpButtons.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
            Me.m_tlpButtons.Size = New System.Drawing.Size(194, 23)
            Me.m_tlpButtons.TabIndex = 5
            '
            'm_btnInvert
            '
            Me.m_btnInvert.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_btnInvert.Location = New System.Drawing.Point(100, 0)
            Me.m_btnInvert.Margin = New System.Windows.Forms.Padding(3, 0, 0, 0)
            Me.m_btnInvert.Name = "m_btnInvert"
            Me.m_btnInvert.Size = New System.Drawing.Size(94, 23)
            Me.m_btnInvert.TabIndex = 1
            Me.m_btnInvert.Text = "&Invert"
            Me.m_btnInvert.UseVisualStyleBackColor = True
            '
            'ucLayerEditorExclusion
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.m_tlpButtons)
            Me.Controls.Add(Me.m_cbAlwaysShowExcluded)
            Me.Controls.Add(Me.m_lblDepth)
            Me.Controls.Add(Me.m_btnSet)
            Me.Controls.Add(Me.m_nudDepth)
            Me.Name = "ucLayerEditorExclusion"
            Me.Size = New System.Drawing.Size(200, 203)
            Me.Controls.SetChildIndex(Me.m_nudDepth, 0)
            Me.Controls.SetChildIndex(Me.m_btnSet, 0)
            Me.Controls.SetChildIndex(Me.m_lblDepth, 0)
            Me.Controls.SetChildIndex(Me.m_cbAlwaysShowExcluded, 0)
            Me.Controls.SetChildIndex(Me.m_tlpButtons, 0)
            CType(Me.m_nudDepth, System.ComponentModel.ISupportInitialize).EndInit()
            Me.m_tlpButtons.ResumeLayout(False)
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents m_nudDepth As ScientificInterfaceShared.Controls.cEwENumericUpDown
        Private WithEvents m_btnClear As System.Windows.Forms.Button
        Private WithEvents m_btnSet As System.Windows.Forms.Button
        Private WithEvents m_lblDepth As System.Windows.Forms.Label
        Private WithEvents m_cbAlwaysShowExcluded As System.Windows.Forms.CheckBox
        Private WithEvents m_tlpButtons As System.Windows.Forms.TableLayoutPanel
        Private WithEvents m_btnInvert As System.Windows.Forms.Button

    End Class

End Namespace