Namespace Controls

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class ucShapeToolboxToolbar
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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ucShapeToolboxToolbar))
            Me.m_ts = New System.Windows.Forms.ToolStrip
            Me.tsbImport = New System.Windows.Forms.ToolStripButton
            Me.tsbLoad = New System.Windows.Forms.ToolStripButton
            Me.tsbWeight = New System.Windows.Forms.ToolStripButton
            Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator
            Me.tsbAdd = New System.Windows.Forms.ToolStripButton
            Me.tsbDuplicate = New System.Windows.Forms.ToolStripButton
            Me.tsbRemove = New System.Windows.Forms.ToolStripButton
            Me.ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator
            Me.tsbSetTo0 = New System.Windows.Forms.ToolStripButton
            Me.tsbSetToValue = New System.Windows.Forms.ToolStripButton
            Me.tsbResetFs = New System.Windows.Forms.ToolStripButton
            Me.m_ts.SuspendLayout()
            Me.SuspendLayout()
            '
            'm_ts
            '
            Me.m_ts.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
            Me.m_ts.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsbImport, Me.tsbLoad, Me.tsbWeight, Me.ToolStripSeparator1, Me.tsbAdd, Me.tsbDuplicate, Me.tsbRemove, Me.ToolStripSeparator2, Me.tsbSetTo0, Me.tsbSetToValue, Me.tsbResetFs})
            Me.m_ts.Location = New System.Drawing.Point(0, 0)
            Me.m_ts.Name = "m_ts"
            Me.m_ts.RenderMode = System.Windows.Forms.ToolStripRenderMode.System
            Me.m_ts.Size = New System.Drawing.Size(600, 25)
            Me.m_ts.TabIndex = 0
            Me.m_ts.Text = "ToolStrip1"
            '
            'tsbImport
            '
            Me.tsbImport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            Me.tsbImport.Image = CType(resources.GetObject("tsbImport.Image"), System.Drawing.Image)
            Me.tsbImport.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.tsbImport.Name = "tsbImport"
            Me.tsbImport.Size = New System.Drawing.Size(55, 22)
            Me.tsbImport.Text = "&Import..."
            '
            'tsbLoad
            '
            Me.tsbLoad.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            Me.tsbLoad.Image = CType(resources.GetObject("tsbLoad.Image"), System.Drawing.Image)
            Me.tsbLoad.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.tsbLoad.Name = "tsbLoad"
            Me.tsbLoad.Size = New System.Drawing.Size(46, 22)
            Me.tsbLoad.Text = "&Load..."
            '
            'tsbWeight
            '
            Me.tsbWeight.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            Me.tsbWeight.Image = CType(resources.GetObject("tsbWeight.Image"), System.Drawing.Image)
            Me.tsbWeight.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.tsbWeight.Name = "tsbWeight"
            Me.tsbWeight.Size = New System.Drawing.Size(57, 22)
            Me.tsbWeight.Text = "&Weight..."
            '
            'ToolStripSeparator1
            '
            Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
            Me.ToolStripSeparator1.Size = New System.Drawing.Size(6, 25)
            '
            'tsbAdd
            '
            Me.tsbAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            Me.tsbAdd.Image = CType(resources.GetObject("tsbAdd.Image"), System.Drawing.Image)
            Me.tsbAdd.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.tsbAdd.Name = "tsbAdd"
            Me.tsbAdd.Size = New System.Drawing.Size(42, 22)
            Me.tsbAdd.Text = "&Add..."
            '
            'tsbDuplicate
            '
            Me.tsbDuplicate.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            Me.tsbDuplicate.Image = CType(resources.GetObject("tsbDuplicate.Image"), System.Drawing.Image)
            Me.tsbDuplicate.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.tsbDuplicate.Name = "tsbDuplicate"
            Me.tsbDuplicate.Size = New System.Drawing.Size(55, 22)
            Me.tsbDuplicate.Text = "&Duplicate"
            '
            'tsbRemove
            '
            Me.tsbRemove.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            Me.tsbRemove.Image = CType(resources.GetObject("tsbRemove.Image"), System.Drawing.Image)
            Me.tsbRemove.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.tsbRemove.Name = "tsbRemove"
            Me.tsbRemove.Size = New System.Drawing.Size(62, 22)
            Me.tsbRemove.Text = "&Remove..."
            '
            'ToolStripSeparator2
            '
            Me.ToolStripSeparator2.Name = "ToolStripSeparator2"
            Me.ToolStripSeparator2.Size = New System.Drawing.Size(6, 25)
            '
            'tsbSetTo0
            '
            Me.tsbSetTo0.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            Me.tsbSetTo0.Image = CType(resources.GetObject("tsbSetTo0.Image"), System.Drawing.Image)
            Me.tsbSetTo0.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.tsbSetTo0.Name = "tsbSetTo0"
            Me.tsbSetTo0.Size = New System.Drawing.Size(49, 22)
            Me.tsbSetTo0.Text = "&Set to 0"
            '
            'tsbSetToValue
            '
            Me.tsbSetToValue.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            Me.tsbSetToValue.Image = CType(resources.GetObject("tsbSetToValue.Image"), System.Drawing.Image)
            Me.tsbSetToValue.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.tsbSetToValue.Name = "tsbSetToValue"
            Me.tsbSetToValue.Size = New System.Drawing.Size(81, 22)
            Me.tsbSetToValue.Text = "Set to &value..."
            '
            'tsbResetFs
            '
            Me.tsbResetFs.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            Me.tsbResetFs.Image = CType(resources.GetObject("tsbResetFs.Image"), System.Drawing.Image)
            Me.tsbResetFs.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.tsbResetFs.Name = "tsbResetFs"
            Me.tsbResetFs.Size = New System.Drawing.Size(53, 22)
            Me.tsbResetFs.Text = "Reset &All"
            '
            'ucShapeToolboxToolbar
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.m_ts)
            Me.Name = "ucShapeToolboxToolbar"
            Me.Size = New System.Drawing.Size(600, 25)
            Me.m_ts.ResumeLayout(False)
            Me.m_ts.PerformLayout()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents m_ts As System.Windows.Forms.ToolStrip
        Private WithEvents tsbImport As System.Windows.Forms.ToolStripButton
        Private WithEvents tsbLoad As System.Windows.Forms.ToolStripButton
        Private WithEvents ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
        Private WithEvents tsbAdd As System.Windows.Forms.ToolStripButton
        Private WithEvents tsbDuplicate As System.Windows.Forms.ToolStripButton
        Private WithEvents tsbRemove As System.Windows.Forms.ToolStripButton
        Private WithEvents ToolStripSeparator2 As System.Windows.Forms.ToolStripSeparator
        Private WithEvents tsbWeight As System.Windows.Forms.ToolStripButton
        Private WithEvents tsbSetTo0 As System.Windows.Forms.ToolStripButton
        Private WithEvents tsbSetToValue As System.Windows.Forms.ToolStripButton
        Private WithEvents tsbResetFs As System.Windows.Forms.ToolStripButton

    End Class

End Namespace
