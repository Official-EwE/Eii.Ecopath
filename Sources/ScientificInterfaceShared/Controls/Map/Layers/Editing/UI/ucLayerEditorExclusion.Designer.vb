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
' Copyright 1991-2013 UBC Fisheries Centre, Vancouver BC, Canada.
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
            Me.Label1 = New System.Windows.Forms.Label()
            Me.m_nudDepth = New ScientificInterfaceShared.Controls.cEwENumericUpDown()
            Me.m_btnClear = New System.Windows.Forms.Button()
            Me.m_btnSet = New System.Windows.Forms.Button()
            CType(Me.m_nudDepth, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            'Label1
            '
            Me.Label1.AutoSize = True
            Me.Label1.Location = New System.Drawing.Point(3, 47)
            Me.Label1.Name = "Label1"
            Me.Label1.Size = New System.Drawing.Size(45, 13)
            Me.Label1.TabIndex = 0
            Me.Label1.Text = "&Depth >"
            '
            'm_nudDepth
            '
            Me.m_nudDepth.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_nudDepth.Location = New System.Drawing.Point(65, 45)
            Me.m_nudDepth.Maximum = New Decimal(New Integer() {10000, 0, 0, 0})
            Me.m_nudDepth.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
            Me.m_nudDepth.Name = "m_nudDepth"
            Me.m_nudDepth.Size = New System.Drawing.Size(75, 20)
            Me.m_nudDepth.TabIndex = 1
            Me.m_nudDepth.Value = New Decimal(New Integer() {1, 0, 0, 0})
            '
            'm_btnClear
            '
            Me.m_btnClear.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_btnClear.Location = New System.Drawing.Point(66, 71)
            Me.m_btnClear.Name = "m_btnClear"
            Me.m_btnClear.Size = New System.Drawing.Size(131, 23)
            Me.m_btnClear.TabIndex = 3
            Me.m_btnClear.Text = "&Reset excluded cells"
            Me.m_btnClear.UseVisualStyleBackColor = True
            '
            'm_btnSet
            '
            Me.m_btnSet.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_btnSet.Location = New System.Drawing.Point(146, 42)
            Me.m_btnSet.Name = "m_btnSet"
            Me.m_btnSet.Size = New System.Drawing.Size(50, 23)
            Me.m_btnSet.TabIndex = 2
            Me.m_btnSet.Text = "&Set"
            Me.m_btnSet.UseVisualStyleBackColor = True
            '
            'ucLayerEditorExclusion
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.Label1)
            Me.Controls.Add(Me.m_btnSet)
            Me.Controls.Add(Me.m_btnClear)
            Me.Controls.Add(Me.m_nudDepth)
            Me.Name = "ucLayerEditorExclusion"
            Me.Size = New System.Drawing.Size(200, 100)
            Me.Controls.SetChildIndex(Me.m_nudDepth, 0)
            Me.Controls.SetChildIndex(Me.m_btnClear, 0)
            Me.Controls.SetChildIndex(Me.m_btnSet, 0)
            Me.Controls.SetChildIndex(Me.Label1, 0)
            CType(Me.m_nudDepth, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Friend WithEvents Label1 As System.Windows.Forms.Label
        Private WithEvents m_nudDepth As ScientificInterfaceShared.Controls.cEwENumericUpDown
        Private WithEvents m_btnClear As System.Windows.Forms.Button
        Private WithEvents m_btnSet As System.Windows.Forms.Button

    End Class

End Namespace