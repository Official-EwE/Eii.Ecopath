' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Controls

    Partial Class ucChooseEditGradient
        Inherits ucEditVisualStyle

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
            Me.m_cmbGradient = New System.Windows.Forms.ComboBox()
            Me.m_editor = New ScientificInterfaceShared.Controls.ucEditGradient()
            Me.m_tlpContent = New System.Windows.Forms.TableLayoutPanel()
            Me.m_hdr = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
            Me.m_tlpContent.SuspendLayout()
            Me.SuspendLayout()
            '
            'm_cmbGradient
            '
            Me.m_cmbGradient.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_cmbGradient.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed
            Me.m_cmbGradient.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.m_cmbGradient.FormattingEnabled = True
            Me.m_cmbGradient.Location = New System.Drawing.Point(3, 10)
            Me.m_cmbGradient.Margin = New System.Windows.Forms.Padding(3, 10, 3, 10)
            Me.m_cmbGradient.Name = "m_cmbGradient"
            Me.m_cmbGradient.Size = New System.Drawing.Size(334, 21)
            Me.m_cmbGradient.TabIndex = 2
            '
            'm_editor
            '
            Me.m_editor.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_editor.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
            Me.m_editor.ColorRamp = Nothing
            Me.m_editor.Location = New System.Drawing.Point(3, 67)
            Me.m_editor.Name = "m_editor"
            Me.m_editor.Size = New System.Drawing.Size(334, 186)
            Me.m_editor.TabIndex = 3
            '
            'm_tlpContent
            '
            Me.m_tlpContent.ColumnCount = 1
            Me.m_tlpContent.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
            Me.m_tlpContent.Controls.Add(Me.m_cmbGradient, 0, 0)
            Me.m_tlpContent.Controls.Add(Me.m_editor, 0, 2)
            Me.m_tlpContent.Controls.Add(Me.m_hdr, 0, 1)
            Me.m_tlpContent.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_tlpContent.Location = New System.Drawing.Point(0, 0)
            Me.m_tlpContent.Name = "m_tlpContent"
            Me.m_tlpContent.RowCount = 3
            Me.m_tlpContent.RowStyles.Add(New System.Windows.Forms.RowStyle())
            Me.m_tlpContent.RowStyles.Add(New System.Windows.Forms.RowStyle())
            Me.m_tlpContent.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
            Me.m_tlpContent.Size = New System.Drawing.Size(340, 256)
            Me.m_tlpContent.TabIndex = 4
            '
            'm_hdr
            '
            Me.m_hdr.CanCollapseParent = False
            Me.m_hdr.CollapsedParentHeight = 0
            Me.m_hdr.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_hdr.IsCollapsed = False
            Me.m_hdr.Location = New System.Drawing.Point(3, 41)
            Me.m_hdr.Name = "m_hdr"
            Me.m_hdr.Size = New System.Drawing.Size(334, 23)
            Me.m_hdr.TabIndex = 3
            Me.m_hdr.Text = "Edit gradient"
            Me.m_hdr.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'ucChooseEditGradient
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
            Me.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
            Me.Controls.Add(Me.m_tlpContent)
            Me.Name = "ucChooseEditGradient"
            Me.Size = New System.Drawing.Size(340, 256)
            Me.m_tlpContent.ResumeLayout(False)
            Me.ResumeLayout(False)

        End Sub
        Private WithEvents m_cmbGradient As System.Windows.Forms.ComboBox
        Private WithEvents m_editor As ucEditGradient
        Private WithEvents m_tlpContent As TableLayoutPanel
        Private WithEvents m_hdr As cEwEHeaderLabel
    End Class

End Namespace
