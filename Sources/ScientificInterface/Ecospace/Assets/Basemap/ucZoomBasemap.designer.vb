Namespace Ecospace

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class ucZoomBaseMap
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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ucZoomBaseMap))
            Me.m_plZoom = New System.Windows.Forms.Panel
            Me.m_cmsZoom = New System.Windows.Forms.ContextMenuStrip(Me.components)
            Me.PositionToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
            Me.m_tsmiViewCenter2 = New System.Windows.Forms.ToolStripMenuItem
            Me.m_tsmiViewStretch2 = New System.Windows.Forms.ToolStripMenuItem
            Me.ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator
            Me.m_tsmiZoomIn = New System.Windows.Forms.ToolStripMenuItem
            Me.m_tsmiZoomOut = New System.Windows.Forms.ToolStripMenuItem
            Me.m_tsmiZoomReset = New System.Windows.Forms.ToolStripMenuItem
            Me.m_map = New ScientificInterface.Ecospace.ucBaseMap
            Me.m_tsZoom = New System.Windows.Forms.ToolStrip
            Me.m_tsddbPosition = New System.Windows.Forms.ToolStripDropDownButton
            Me.m_tsmiViewCenter1 = New System.Windows.Forms.ToolStripMenuItem
            Me.m_tsmiViewStretch1 = New System.Windows.Forms.ToolStripMenuItem
            Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator
            Me.m_tsbZoomIn = New System.Windows.Forms.ToolStripButton
            Me.m_tsbZoomOut = New System.Windows.Forms.ToolStripButton
            Me.m_tscbZoomPercent = New System.Windows.Forms.ToolStripComboBox
            Me.m_tsbZoomReset = New System.Windows.Forms.ToolStripButton
            Me.m_sbHorz = New System.Windows.Forms.HScrollBar
            Me.m_sbVert = New System.Windows.Forms.VScrollBar
            Me.ToolStripSeparator3 = New System.Windows.Forms.ToolStripSeparator
            Me.m_tsbSaveImage = New System.Windows.Forms.ToolStripButton
            Me.m_plZoom.SuspendLayout()
            Me.m_cmsZoom.SuspendLayout()
            Me.m_tsZoom.SuspendLayout()
            Me.SuspendLayout()
            '
            'm_plZoom
            '
            Me.m_plZoom.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_plZoom.BackColor = System.Drawing.SystemColors.AppWorkspace
            Me.m_plZoom.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.m_plZoom.ContextMenuStrip = Me.m_cmsZoom
            Me.m_plZoom.Controls.Add(Me.m_map)
            Me.m_plZoom.Location = New System.Drawing.Point(0, 25)
            Me.m_plZoom.Margin = New System.Windows.Forms.Padding(0)
            Me.m_plZoom.Name = "m_plZoom"
            Me.m_plZoom.Size = New System.Drawing.Size(427, 320)
            Me.m_plZoom.TabIndex = 0
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
            Me.m_tsmiZoomIn.Name = "m_tsmiZoomIn"
            Me.m_tsmiZoomIn.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.I), System.Windows.Forms.Keys)
            Me.m_tsmiZoomIn.Size = New System.Drawing.Size(180, 22)
            Me.m_tsmiZoomIn.Text = "Zoom in"
            '
            'm_tsmiZoomOut
            '
            Me.m_tsmiZoomOut.Name = "m_tsmiZoomOut"
            Me.m_tsmiZoomOut.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.O), System.Windows.Forms.Keys)
            Me.m_tsmiZoomOut.Size = New System.Drawing.Size(180, 22)
            Me.m_tsmiZoomOut.Text = "Zoom out"
            '
            'm_tsmiZoomReset
            '
            Me.m_tsmiZoomReset.Name = "m_tsmiZoomReset"
            Me.m_tsmiZoomReset.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.R), System.Windows.Forms.Keys)
            Me.m_tsmiZoomReset.Size = New System.Drawing.Size(180, 22)
            Me.m_tsmiZoomReset.Text = "Reset zoom"
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
            'm_tsZoom
            '
            Me.m_tsZoom.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tsbSaveImage, Me.ToolStripSeparator3, Me.m_tsddbPosition, Me.ToolStripSeparator1, Me.m_tsbZoomIn, Me.m_tsbZoomOut, Me.m_tscbZoomPercent, Me.m_tsbZoomReset})
            Me.m_tsZoom.Location = New System.Drawing.Point(0, 0)
            Me.m_tsZoom.Name = "m_tsZoom"
            Me.m_tsZoom.Size = New System.Drawing.Size(443, 25)
            Me.m_tsZoom.TabIndex = 0
            Me.m_tsZoom.Text = "ToolStrip1"
            '
            'm_tsddbPosition
            '
            Me.m_tsddbPosition.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            Me.m_tsddbPosition.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tsmiViewCenter1, Me.m_tsmiViewStretch1})
            Me.m_tsddbPosition.Image = CType(resources.GetObject("m_tsddbPosition.Image"), System.Drawing.Image)
            Me.m_tsddbPosition.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.m_tsddbPosition.Name = "m_tsddbPosition"
            Me.m_tsddbPosition.Size = New System.Drawing.Size(57, 22)
            Me.m_tsddbPosition.Text = "Position"
            '
            'm_tsmiViewCenter1
            '
            Me.m_tsmiViewCenter1.Name = "m_tsmiViewCenter1"
            Me.m_tsmiViewCenter1.Size = New System.Drawing.Size(152, 22)
            Me.m_tsmiViewCenter1.Text = "Center"
            '
            'm_tsmiViewStretch1
            '
            Me.m_tsmiViewStretch1.Name = "m_tsmiViewStretch1"
            Me.m_tsmiViewStretch1.Size = New System.Drawing.Size(152, 22)
            Me.m_tsmiViewStretch1.Text = "Stretch"
            '
            'ToolStripSeparator1
            '
            Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
            Me.ToolStripSeparator1.Size = New System.Drawing.Size(6, 25)
            '
            'm_tsbZoomIn
            '
            Me.m_tsbZoomIn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
            Me.m_tsbZoomIn.Image = Global.ScientificInterface.My.Resources.Resources.ZoomInHS
            Me.m_tsbZoomIn.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.m_tsbZoomIn.Name = "m_tsbZoomIn"
            Me.m_tsbZoomIn.Size = New System.Drawing.Size(23, 22)
            Me.m_tsbZoomIn.ToolTipText = "Zoom in"
            '
            'm_tsbZoomOut
            '
            Me.m_tsbZoomOut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
            Me.m_tsbZoomOut.Image = Global.ScientificInterface.My.Resources.Resources.ZoomOutHS
            Me.m_tsbZoomOut.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.m_tsbZoomOut.Name = "m_tsbZoomOut"
            Me.m_tsbZoomOut.Size = New System.Drawing.Size(23, 22)
            Me.m_tsbZoomOut.ToolTipText = "Zoom out"
            '
            'm_tscbZoomPercent
            '
            Me.m_tscbZoomPercent.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.m_tscbZoomPercent.Name = "m_tscbZoomPercent"
            Me.m_tscbZoomPercent.Size = New System.Drawing.Size(100, 25)
            Me.m_tscbZoomPercent.Visible = False
            '
            'm_tsbZoomReset
            '
            Me.m_tsbZoomReset.Image = Global.ScientificInterface.My.Resources.Resources.ZoomHS
            Me.m_tsbZoomReset.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.m_tsbZoomReset.Name = "m_tsbZoomReset"
            Me.m_tsbZoomReset.Size = New System.Drawing.Size(55, 22)
            Me.m_tsbZoomReset.Text = "Reset"
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
            Me.m_sbVert.Location = New System.Drawing.Point(427, 26)
            Me.m_sbVert.Name = "m_sbVert"
            Me.m_sbVert.Size = New System.Drawing.Size(16, 319)
            Me.m_sbVert.TabIndex = 2
            '
            'ToolStripSeparator3
            '
            Me.ToolStripSeparator3.Name = "ToolStripSeparator3"
            Me.ToolStripSeparator3.Size = New System.Drawing.Size(6, 25)
            '
            'm_tsbSaveImage
            '
            Me.m_tsbSaveImage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
            Me.m_tsbSaveImage.Image = Global.ScientificInterface.My.Resources.Resources.saveHS
            Me.m_tsbSaveImage.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.m_tsbSaveImage.Name = "m_tsbSaveImage"
            Me.m_tsbSaveImage.Size = New System.Drawing.Size(23, 22)
            Me.m_tsbSaveImage.Text = "Save image"
            '
            'ucZoomBaseMap
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.m_sbVert)
            Me.Controls.Add(Me.m_sbHorz)
            Me.Controls.Add(Me.m_tsZoom)
            Me.Controls.Add(Me.m_plZoom)
            Me.Name = "ucZoomBaseMap"
            Me.Size = New System.Drawing.Size(443, 364)
            Me.m_plZoom.ResumeLayout(False)
            Me.m_cmsZoom.ResumeLayout(False)
            Me.m_tsZoom.ResumeLayout(False)
            Me.m_tsZoom.PerformLayout()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Friend WithEvents m_plZoom As System.Windows.Forms.Panel
        Friend WithEvents m_tsZoom As System.Windows.Forms.ToolStrip
        Friend WithEvents m_tsbZoomIn As System.Windows.Forms.ToolStripButton
        Friend WithEvents m_tsbZoomOut As System.Windows.Forms.ToolStripButton
        Friend WithEvents m_tsbZoomReset As System.Windows.Forms.ToolStripButton
        Friend WithEvents m_tscbZoomPercent As System.Windows.Forms.ToolStripComboBox
        Friend WithEvents ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
        Friend WithEvents m_map As Ecospace.ucBaseMap
        Friend WithEvents m_tsddbPosition As System.Windows.Forms.ToolStripDropDownButton
        Friend WithEvents m_tsmiViewStretch1 As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents m_tsmiViewCenter1 As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents m_sbHorz As System.Windows.Forms.HScrollBar
        Friend WithEvents m_sbVert As System.Windows.Forms.VScrollBar
        Friend WithEvents m_cmsZoom As System.Windows.Forms.ContextMenuStrip
        Friend WithEvents PositionToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents m_tsmiViewCenter2 As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents m_tsmiViewStretch2 As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents ToolStripSeparator2 As System.Windows.Forms.ToolStripSeparator
        Friend WithEvents m_tsmiZoomIn As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents m_tsmiZoomOut As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents m_tsmiZoomReset As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents m_tsbSaveImage As System.Windows.Forms.ToolStripButton
        Friend WithEvents ToolStripSeparator3 As System.Windows.Forms.ToolStripSeparator

    End Class

End Namespace