' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Controls.Map.Layers

    Partial Class ucLayerEditorEffortZone
        Inherits ucLayerEditorDefault

        'Required by the Windows Form Designer
        Private components As System.ComponentModel.IContainer

        'NOTE: The following procedure is required by the Windows Form Designer
        'It can be modified using the Windows Form Designer.  
        'Do not modify it using the code editor.
        <System.Diagnostics.DebuggerStepThrough()>
        Private Sub InitializeComponent()
            Me.m_lblZone = New System.Windows.Forms.Label()
            Me.m_nudZone = New System.Windows.Forms.NumericUpDown()
            CType(Me.m_nudZone, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            'm_lblZone
            '
            Me.m_lblZone.AutoSize = True
            Me.m_lblZone.Location = New System.Drawing.Point(3, 153)
            Me.m_lblZone.Name = "m_lblZone"
            Me.m_lblZone.Size = New System.Drawing.Size(35, 13)
            Me.m_lblZone.TabIndex = 11
            Me.m_lblZone.Text = "&Zone:"
            '
            'm_nudZone
            '
            Me.m_nudZone.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_nudZone.Location = New System.Drawing.Point(68, 151)
            Me.m_nudZone.Name = "m_nudZone"
            Me.m_nudZone.Size = New System.Drawing.Size(129, 20)
            Me.m_nudZone.TabIndex = 12
            '
            'ucLayerEditorEffortZone
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
            Me.Controls.Add(Me.m_lblZone)
            Me.Controls.Add(Me.m_nudZone)
            Me.Name = "ucLayerEditorEffortZone"
            Me.Size = New System.Drawing.Size(200, 178)
            Me.Controls.SetChildIndex(Me.m_nudZone, 0)
            Me.Controls.SetChildIndex(Me.m_lblZone, 0)
            CType(Me.m_nudZone, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub

        Private WithEvents m_lblZone As System.Windows.Forms.Label
        Private WithEvents m_nudZone As System.Windows.Forms.NumericUpDown

    End Class

End Namespace
