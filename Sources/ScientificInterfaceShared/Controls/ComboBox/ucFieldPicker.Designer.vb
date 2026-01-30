' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Controls

    Partial Class ucFieldPicker
        Inherits System.Windows.Forms.UserControl

        'UserControl overrides dispose to clean up the component list.
        <System.Diagnostics.DebuggerNonUserCode()>
        Protected Overrides Sub Dispose(disposing As Boolean)
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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ucFieldPicker))
            Me.m_tsBogus = New ScientificInterfaceShared.Controls.cEwEToolstrip()
            Me.m_tsddFields = New System.Windows.Forms.ToolStripSplitButton()
            Me.m_tsBogus.SuspendLayout()
            Me.SuspendLayout()
            '
            'm_tsBogus
            '
            Me.m_tsBogus.AutoSize = False
            Me.m_tsBogus.BackColor = System.Drawing.Color.Transparent
            Me.m_tsBogus.CanOverflow = False
            Me.m_tsBogus.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_tsBogus.GripMargin = New System.Windows.Forms.Padding(0)
            Me.m_tsBogus.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
            Me.m_tsBogus.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tsddFields})
            Me.m_tsBogus.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Table
            Me.m_tsBogus.Location = New System.Drawing.Point(0, 0)
            Me.m_tsBogus.Name = "m_tsBogus"
            Me.m_tsBogus.RenderMode = System.Windows.Forms.ToolStripRenderMode.System
            Me.m_tsBogus.Size = New System.Drawing.Size(62, 21)
            Me.m_tsBogus.TabIndex = 6
            '
            'm_tsddFields
            '
            Me.m_tsddFields.BackColor = System.Drawing.Color.Transparent
            Me.m_tsddFields.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            Me.m_tsddFields.Image = CType(resources.GetObject("m_tsddFields.Image"), System.Drawing.Image)
            Me.m_tsddFields.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.m_tsddFields.Name = "m_tsddFields"
            Me.m_tsddFields.Size = New System.Drawing.Size(54, 19)
            Me.m_tsddFields.Text = "{field}"
            '
            'ucFieldPicker
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
            Me.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
            Me.Controls.Add(Me.m_tsBogus)
            Me.Name = "ucFieldPicker"
            Me.Size = New System.Drawing.Size(62, 21)
            Me.m_tsBogus.ResumeLayout(False)
            Me.m_tsBogus.PerformLayout()
            Me.ResumeLayout(False)

        End Sub
        Private WithEvents m_tsBogus As cEwEToolstrip
        Private WithEvents m_tsddFields As System.Windows.Forms.ToolStripSplitButton

    End Class

End Namespace
