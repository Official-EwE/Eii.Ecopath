Namespace Ecospace

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class ucMapZoomToolbar
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
        <System.Diagnostics.DebuggerStepThrough()> _
        Private Sub InitializeComponent()
            Me.components = New System.ComponentModel.Container
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ucMapZoomToolbar))
            Me.m_tsZoom = New System.Windows.Forms.ToolStrip
            Me.m_tsbSaveImage = New System.Windows.Forms.ToolStripButton
            Me.m_ts1 = New System.Windows.Forms.ToolStripSeparator
            Me.m_tsddbPosition = New System.Windows.Forms.ToolStripDropDownButton
            Me.m_tsmiViewCenter1 = New System.Windows.Forms.ToolStripMenuItem
            Me.m_tsmiViewStretch1 = New System.Windows.Forms.ToolStripMenuItem
            Me.m_ts2 = New System.Windows.Forms.ToolStripSeparator
            Me.m_tsbZoomIn = New System.Windows.Forms.ToolStripButton
            Me.m_tsbZoomOut = New System.Windows.Forms.ToolStripButton
            Me.m_tscbZoomPercent = New System.Windows.Forms.ToolStripComboBox
            Me.m_tsbZoomReset = New System.Windows.Forms.ToolStripButton
            Me.m_cmsZoom = New System.Windows.Forms.ContextMenuStrip(Me.components)
            Me.m_tsmiViewCenter2 = New System.Windows.Forms.ToolStripMenuItem
            Me.m_tsmiViewStretch2 = New System.Windows.Forms.ToolStripMenuItem
            Me.m_ts3 = New System.Windows.Forms.ToolStripSeparator
            Me.m_tsmiZoomIn = New System.Windows.Forms.ToolStripMenuItem
            Me.m_tsmiZoomOut = New System.Windows.Forms.ToolStripMenuItem
            Me.m_tsmiZoomReset = New System.Windows.Forms.ToolStripMenuItem
            Me.m_tsZoom.SuspendLayout()
            Me.m_cmsZoom.SuspendLayout()
            Me.SuspendLayout()
            '
            'm_tsZoom
            '
            Me.m_tsZoom.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tsbSaveImage, Me.m_ts1, Me.m_tsddbPosition, Me.m_ts2, Me.m_tsbZoomIn, Me.m_tsbZoomOut, Me.m_tscbZoomPercent, Me.m_tsbZoomReset})
            Me.m_tsZoom.Location = New System.Drawing.Point(0, 0)
            Me.m_tsZoom.Name = "m_tsZoom"
            Me.m_tsZoom.Size = New System.Drawing.Size(377, 25)
            Me.m_tsZoom.TabIndex = 1
            Me.m_tsZoom.Text = "m_tzZoom"
            '
            'm_tsbSaveImage
            '
            Me.m_tsbSaveImage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
            Me.m_tsbSaveImage.Image = Global.ScientificInterface.My.Resources.Resources.InsertPictureHS
            Me.m_tsbSaveImage.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.m_tsbSaveImage.Name = "m_tsbSaveImage"
            Me.m_tsbSaveImage.Size = New System.Drawing.Size(23, 22)
            Me.m_tsbSaveImage.Text = "Save image"
            '
            'm_ts1
            '
            Me.m_ts1.Name = "m_ts1"
            Me.m_ts1.Size = New System.Drawing.Size(6, 25)
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
            Me.m_tsmiViewCenter1.Size = New System.Drawing.Size(120, 22)
            Me.m_tsmiViewCenter1.Text = "Center"
            '
            'm_tsmiViewStretch1
            '
            Me.m_tsmiViewStretch1.Name = "m_tsmiViewStretch1"
            Me.m_tsmiViewStretch1.Size = New System.Drawing.Size(120, 22)
            Me.m_tsmiViewStretch1.Text = "Stretch"
            '
            'm_ts2
            '
            Me.m_ts2.Name = "m_ts2"
            Me.m_ts2.Size = New System.Drawing.Size(6, 25)
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
            '
            'm_tsbZoomReset
            '
            Me.m_tsbZoomReset.Image = Global.ScientificInterface.My.Resources.Resources.ZoomHS
            Me.m_tsbZoomReset.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.m_tsbZoomReset.Name = "m_tsbZoomReset"
            Me.m_tsbZoomReset.Size = New System.Drawing.Size(55, 22)
            Me.m_tsbZoomReset.Text = "Reset"
            '
            'm_cmsZoom
            '
            Me.m_cmsZoom.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tsmiViewCenter2, Me.m_tsmiViewStretch2, Me.m_ts3, Me.m_tsmiZoomIn, Me.m_tsmiZoomOut, Me.m_tsmiZoomReset})
            Me.m_cmsZoom.Name = "m_cmsControl"
            Me.m_cmsZoom.Size = New System.Drawing.Size(181, 120)
            '
            'm_tsmiViewCenter2
            '
            Me.m_tsmiViewCenter2.Name = "m_tsmiViewCenter2"
            Me.m_tsmiViewCenter2.Size = New System.Drawing.Size(180, 22)
            Me.m_tsmiViewCenter2.Text = "Center"
            '
            'm_tsmiViewStretch2
            '
            Me.m_tsmiViewStretch2.Name = "m_tsmiViewStretch2"
            Me.m_tsmiViewStretch2.Size = New System.Drawing.Size(180, 22)
            Me.m_tsmiViewStretch2.Text = "Stretch"
            '
            'm_ts3
            '
            Me.m_ts3.Name = "m_ts3"
            Me.m_ts3.Size = New System.Drawing.Size(177, 6)
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
            'ucMapZoomToolbar
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.AutoSize = True
            Me.Controls.Add(Me.m_tsZoom)
            Me.MinimumSize = New System.Drawing.Size(100, 25)
            Me.Name = "ucMapZoomToolbar"
            Me.Size = New System.Drawing.Size(377, 49)
            Me.m_tsZoom.ResumeLayout(False)
            Me.m_tsZoom.PerformLayout()
            Me.m_cmsZoom.ResumeLayout(False)
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents m_tsZoom As System.Windows.Forms.ToolStrip
        Private WithEvents m_cmsZoom As System.Windows.Forms.ContextMenuStrip
        Private WithEvents m_tsbSaveImage As System.Windows.Forms.ToolStripButton
        Private WithEvents m_ts1 As System.Windows.Forms.ToolStripSeparator
        Private WithEvents m_ts2 As System.Windows.Forms.ToolStripSeparator
        Private WithEvents m_ts3 As System.Windows.Forms.ToolStripSeparator
        Private WithEvents m_tsddbPosition As System.Windows.Forms.ToolStripDropDownButton
        Private WithEvents m_tsmiViewCenter1 As System.Windows.Forms.ToolStripMenuItem
        Private WithEvents m_tsmiViewStretch1 As System.Windows.Forms.ToolStripMenuItem
        Private WithEvents m_tsbZoomIn As System.Windows.Forms.ToolStripButton
        Private WithEvents m_tsbZoomOut As System.Windows.Forms.ToolStripButton
        Private WithEvents m_tscbZoomPercent As System.Windows.Forms.ToolStripComboBox
        Private WithEvents m_tsbZoomReset As System.Windows.Forms.ToolStripButton
        Private WithEvents m_tsmiZoomIn As System.Windows.Forms.ToolStripMenuItem
        Private WithEvents m_tsmiZoomOut As System.Windows.Forms.ToolStripMenuItem
        Private WithEvents m_tsmiZoomReset As System.Windows.Forms.ToolStripMenuItem
        Private WithEvents m_tsmiViewCenter2 As System.Windows.Forms.ToolStripMenuItem
        Private WithEvents m_tsmiViewStretch2 As System.Windows.Forms.ToolStripMenuItem

    End Class

End Namespace
