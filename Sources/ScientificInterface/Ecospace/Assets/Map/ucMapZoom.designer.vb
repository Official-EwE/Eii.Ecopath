Namespace Ecospace

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class ucMapZoom
        Inherits System.Windows.Forms.UserControl

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
        '<System.Diagnostics.DebuggerStepThrough()> _
        Private Sub InitializeComponent()
            Me.components = New System.ComponentModel.Container
            Me.m_plZoom = New System.Windows.Forms.Panel
            Me.m_map = New ScientificInterface.Ecospace.ucMap
            Me.m_cmsZoom = New System.Windows.Forms.ContextMenuStrip(Me.components)
            Me.PositionToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
            Me.m_tsmiViewCenter2 = New System.Windows.Forms.ToolStripMenuItem
            Me.m_tsmiViewStretch2 = New System.Windows.Forms.ToolStripMenuItem
            Me.ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator
            Me.m_tsmiZoomIn = New System.Windows.Forms.ToolStripMenuItem
            Me.m_tsmiZoomOut = New System.Windows.Forms.ToolStripMenuItem
            Me.m_tsmiZoomReset = New System.Windows.Forms.ToolStripMenuItem
            Me.m_sbHorz = New System.Windows.Forms.HScrollBar
            Me.m_sbVert = New System.Windows.Forms.VScrollBar
            Me.m_plZoom.SuspendLayout()
            Me.m_cmsZoom.SuspendLayout()
            Me.SuspendLayout()
            '
            'm_plZoom
            '
            Me.m_plZoom.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_plZoom.BackColor = System.Drawing.SystemColors.AppWorkspace
            Me.m_plZoom.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.m_plZoom.Controls.Add(Me.m_map)
            Me.m_plZoom.Location = New System.Drawing.Point(0, 0)
            Me.m_plZoom.Margin = New System.Windows.Forms.Padding(0)
            Me.m_plZoom.Name = "m_plZoom"
            Me.m_plZoom.Size = New System.Drawing.Size(427, 345)
            Me.m_plZoom.TabIndex = 0
            '
            'm_map
            '
            Me.m_map.BackColor = System.Drawing.SystemColors.Window
            Me.m_map.Basemap = Nothing
            Me.m_map.Editable = False
            Me.m_map.Location = New System.Drawing.Point(0, 0)
            Me.m_map.Margin = New System.Windows.Forms.Padding(0)
            Me.m_map.Name = "m_map"
            Me.m_map.Size = New System.Drawing.Size(200, 200)
            Me.m_map.TabIndex = 0
            '
            'm_cmsZoom
            '
            Me.m_cmsZoom.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.PositionToolStripMenuItem, Me.ToolStripSeparator2, Me.m_tsmiZoomIn, Me.m_tsmiZoomOut, Me.m_tsmiZoomReset})
            Me.m_cmsZoom.Name = "m_cmsControl"
            Me.m_cmsZoom.Size = New System.Drawing.Size(181, 98)
            '
            'PositionToolStripMenuItem
            '
            Me.PositionToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tsmiViewCenter2, Me.m_tsmiViewStretch2})
            Me.PositionToolStripMenuItem.Enabled = False
            Me.PositionToolStripMenuItem.Name = "PositionToolStripMenuItem"
            Me.PositionToolStripMenuItem.Size = New System.Drawing.Size(180, 22)
            Me.PositionToolStripMenuItem.Text = "Position"
            '
            'm_tsmiViewCenter2
            '
            Me.m_tsmiViewCenter2.Name = "m_tsmiViewCenter2"
            Me.m_tsmiViewCenter2.Size = New System.Drawing.Size(120, 22)
            Me.m_tsmiViewCenter2.Text = "Center"
            '
            'm_tsmiViewStretch2
            '
            Me.m_tsmiViewStretch2.Name = "m_tsmiViewStretch2"
            Me.m_tsmiViewStretch2.Size = New System.Drawing.Size(120, 22)
            Me.m_tsmiViewStretch2.Text = "Stretch"
            '
            'ToolStripSeparator2
            '
            Me.ToolStripSeparator2.Name = "ToolStripSeparator2"
            Me.ToolStripSeparator2.Size = New System.Drawing.Size(177, 6)
            '
            'm_tsmiZoomIn
            '
            Me.m_tsmiZoomIn.Enabled = False
            Me.m_tsmiZoomIn.Name = "m_tsmiZoomIn"
            Me.m_tsmiZoomIn.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.I), System.Windows.Forms.Keys)
            Me.m_tsmiZoomIn.Size = New System.Drawing.Size(180, 22)
            Me.m_tsmiZoomIn.Text = "Zoom in"
            '
            'm_tsmiZoomOut
            '
            Me.m_tsmiZoomOut.Enabled = False
            Me.m_tsmiZoomOut.Name = "m_tsmiZoomOut"
            Me.m_tsmiZoomOut.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.O), System.Windows.Forms.Keys)
            Me.m_tsmiZoomOut.Size = New System.Drawing.Size(180, 22)
            Me.m_tsmiZoomOut.Text = "Zoom out"
            '
            'm_tsmiZoomReset
            '
            Me.m_tsmiZoomReset.Enabled = False
            Me.m_tsmiZoomReset.Name = "m_tsmiZoomReset"
            Me.m_tsmiZoomReset.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.R), System.Windows.Forms.Keys)
            Me.m_tsmiZoomReset.Size = New System.Drawing.Size(180, 22)
            Me.m_tsmiZoomReset.Text = "Reset zoom"
            '
            'm_sbHorz
            '
            Me.m_sbHorz.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_sbHorz.Location = New System.Drawing.Point(1, 345)
            Me.m_sbHorz.Name = "m_sbHorz"
            Me.m_sbHorz.Size = New System.Drawing.Size(426, 19)
            Me.m_sbHorz.TabIndex = 1
            '
            'm_sbVert
            '
            Me.m_sbVert.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_sbVert.Location = New System.Drawing.Point(427, 1)
            Me.m_sbVert.Name = "m_sbVert"
            Me.m_sbVert.Size = New System.Drawing.Size(16, 344)
            Me.m_sbVert.TabIndex = 2
            '
            'ucMapZoom
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.m_sbVert)
            Me.Controls.Add(Me.m_sbHorz)
            Me.Controls.Add(Me.m_plZoom)
            Me.Name = "ucMapZoom"
            Me.Size = New System.Drawing.Size(443, 364)
            Me.m_plZoom.ResumeLayout(False)
            Me.m_cmsZoom.ResumeLayout(False)
            Me.ResumeLayout(False)

        End Sub
        Private WithEvents m_sbHorz As System.Windows.Forms.HScrollBar
        Private WithEvents m_sbVert As System.Windows.Forms.VScrollBar
        Private WithEvents PositionToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
        Private WithEvents m_tsmiViewCenter2 As System.Windows.Forms.ToolStripMenuItem
        Private WithEvents m_tsmiViewStretch2 As System.Windows.Forms.ToolStripMenuItem
        Private WithEvents ToolStripSeparator2 As System.Windows.Forms.ToolStripSeparator
        Private WithEvents m_tsmiZoomIn As System.Windows.Forms.ToolStripMenuItem
        Private WithEvents m_tsmiZoomOut As System.Windows.Forms.ToolStripMenuItem
        Private WithEvents m_tsmiZoomReset As System.Windows.Forms.ToolStripMenuItem
        Private WithEvents m_cmsZoom As System.Windows.Forms.ContextMenuStrip
        Private WithEvents m_plZoom As System.Windows.Forms.Panel
        Private WithEvents m_map As ScientificInterface.Ecospace.ucMap

    End Class

End Namespace