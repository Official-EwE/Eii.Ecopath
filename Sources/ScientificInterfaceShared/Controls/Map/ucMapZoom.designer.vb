' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Controls.Map

    Partial Class ucMapZoom
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
        '<System.Diagnostics.DebuggerStepThrough()> _
        Private Sub InitializeComponent()
            Me.components = New System.ComponentModel.Container()
            Me.m_map = New ScientificInterfaceShared.Controls.Map.ucMap()
            Me.m_cmsZoom = New System.Windows.Forms.ContextMenuStrip(Me.components)
            Me.PositionToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
            Me.m_tsmiViewCenter2 = New System.Windows.Forms.ToolStripMenuItem()
            Me.m_tsmiViewStretch2 = New System.Windows.Forms.ToolStripMenuItem()
            Me.ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator()
            Me.m_tsmiZoomIn = New System.Windows.Forms.ToolStripMenuItem()
            Me.m_tsmiZoomOut = New System.Windows.Forms.ToolStripMenuItem()
            Me.m_tsmiZoomReset = New System.Windows.Forms.ToolStripMenuItem()
            Me.m_tlpMap = New System.Windows.Forms.TableLayoutPanel()
            Me.m_sbVert = New System.Windows.Forms.VScrollBar()
            Me.m_sbHorz = New System.Windows.Forms.HScrollBar()
            Me.m_cmsZoom.SuspendLayout()
            Me.m_tlpMap.SuspendLayout()
            Me.SuspendLayout()
            '
            'm_map
            '
            Me.m_map.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
            Me.m_map.BackColor = System.Drawing.SystemColors.Window
            Me.m_map.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.m_map.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_map.Editable = False
            Me.m_map.Location = New System.Drawing.Point(0, 0)
            Me.m_map.Margin = New System.Windows.Forms.Padding(0)
            Me.m_map.Name = "m_map"
            Me.m_map.ScrollPos = New System.Drawing.Point(0, 0)
            Me.m_map.Size = New System.Drawing.Size(426, 347)
            Me.m_map.TabIndex = 0
            Me.m_map.Title = ""
            Me.m_map.UIContext = Nothing
            '
            'm_cmsZoom
            '
            Me.m_cmsZoom.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.PositionToolStripMenuItem, Me.ToolStripSeparator2, Me.m_tsmiZoomIn, Me.m_tsmiZoomOut, Me.m_tsmiZoomReset})
            Me.m_cmsZoom.Name = "m_cmsControl"
            Me.m_cmsZoom.Size = New System.Drawing.Size(177, 98)
            '
            'PositionToolStripMenuItem
            '
            Me.PositionToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tsmiViewCenter2, Me.m_tsmiViewStretch2})
            Me.PositionToolStripMenuItem.Enabled = False
            Me.PositionToolStripMenuItem.Name = "PositionToolStripMenuItem"
            Me.PositionToolStripMenuItem.Size = New System.Drawing.Size(176, 22)
            Me.PositionToolStripMenuItem.Text = "Position"
            '
            'm_tsmiViewCenter2
            '
            Me.m_tsmiViewCenter2.Name = "m_tsmiViewCenter2"
            Me.m_tsmiViewCenter2.Size = New System.Drawing.Size(111, 22)
            Me.m_tsmiViewCenter2.Text = "Center"
            '
            'm_tsmiViewStretch2
            '
            Me.m_tsmiViewStretch2.Name = "m_tsmiViewStretch2"
            Me.m_tsmiViewStretch2.Size = New System.Drawing.Size(111, 22)
            Me.m_tsmiViewStretch2.Text = "Stretch"
            '
            'ToolStripSeparator2
            '
            Me.ToolStripSeparator2.Name = "ToolStripSeparator2"
            Me.ToolStripSeparator2.Size = New System.Drawing.Size(173, 6)
            '
            'm_tsmiZoomIn
            '
            Me.m_tsmiZoomIn.Enabled = False
            Me.m_tsmiZoomIn.Name = "m_tsmiZoomIn"
            Me.m_tsmiZoomIn.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.I), System.Windows.Forms.Keys)
            Me.m_tsmiZoomIn.Size = New System.Drawing.Size(176, 22)
            Me.m_tsmiZoomIn.Text = "Zoom in"
            '
            'm_tsmiZoomOut
            '
            Me.m_tsmiZoomOut.Enabled = False
            Me.m_tsmiZoomOut.Name = "m_tsmiZoomOut"
            Me.m_tsmiZoomOut.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.O), System.Windows.Forms.Keys)
            Me.m_tsmiZoomOut.Size = New System.Drawing.Size(176, 22)
            Me.m_tsmiZoomOut.Text = "Zoom out"
            '
            'm_tsmiZoomReset
            '
            Me.m_tsmiZoomReset.Enabled = False
            Me.m_tsmiZoomReset.Name = "m_tsmiZoomReset"
            Me.m_tsmiZoomReset.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.R), System.Windows.Forms.Keys)
            Me.m_tsmiZoomReset.Size = New System.Drawing.Size(176, 22)
            Me.m_tsmiZoomReset.Text = "Reset zoom"
            '
            'm_tlpMap
            '
            Me.m_tlpMap.ColumnCount = 2
            Me.m_tlpMap.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
            Me.m_tlpMap.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
            Me.m_tlpMap.Controls.Add(Me.m_map, 0, 0)
            Me.m_tlpMap.Controls.Add(Me.m_sbVert, 1, 0)
            Me.m_tlpMap.Controls.Add(Me.m_sbHorz, 0, 1)
            Me.m_tlpMap.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_tlpMap.Location = New System.Drawing.Point(0, 0)
            Me.m_tlpMap.Name = "m_tlpMap"
            Me.m_tlpMap.RowCount = 2
            Me.m_tlpMap.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
            Me.m_tlpMap.RowStyles.Add(New System.Windows.Forms.RowStyle())
            Me.m_tlpMap.Size = New System.Drawing.Size(443, 364)
            Me.m_tlpMap.TabIndex = 1
            '
            'm_sbVert
            '
            Me.m_sbVert.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_sbVert.Location = New System.Drawing.Point(426, 0)
            Me.m_sbVert.Name = "m_sbVert"
            Me.m_sbVert.Size = New System.Drawing.Size(17, 347)
            Me.m_sbVert.TabIndex = 1
            '
            'm_sbHorz
            '
            Me.m_sbHorz.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_sbHorz.Location = New System.Drawing.Point(0, 347)
            Me.m_sbHorz.Name = "m_sbHorz"
            Me.m_sbHorz.Size = New System.Drawing.Size(426, 17)
            Me.m_sbHorz.TabIndex = 2
            '
            'ucMapZoom
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.AutoScroll = True
            Me.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
            Me.Controls.Add(Me.m_tlpMap)
            Me.Name = "ucMapZoom"
            Me.Size = New System.Drawing.Size(443, 364)
            Me.m_cmsZoom.ResumeLayout(False)
            Me.m_tlpMap.ResumeLayout(False)
            Me.ResumeLayout(False)

        End Sub
        Private WithEvents PositionToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
        Private WithEvents m_tsmiViewCenter2 As System.Windows.Forms.ToolStripMenuItem
        Private WithEvents m_tsmiViewStretch2 As System.Windows.Forms.ToolStripMenuItem
        Private WithEvents ToolStripSeparator2 As System.Windows.Forms.ToolStripSeparator
        Private WithEvents m_tsmiZoomIn As System.Windows.Forms.ToolStripMenuItem
        Private WithEvents m_tsmiZoomOut As System.Windows.Forms.ToolStripMenuItem
        Private WithEvents m_tsmiZoomReset As System.Windows.Forms.ToolStripMenuItem
        Private WithEvents m_cmsZoom As System.Windows.Forms.ContextMenuStrip
        Private WithEvents m_map As ucMap
        Private WithEvents m_tlpMap As TableLayoutPanel
        Private WithEvents m_sbVert As VScrollBar
        Private WithEvents m_sbHorz As HScrollBar
    End Class

End Namespace