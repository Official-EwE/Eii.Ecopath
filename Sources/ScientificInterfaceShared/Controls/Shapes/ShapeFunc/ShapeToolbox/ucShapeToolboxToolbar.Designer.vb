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
            Me.m_tsbImport = New System.Windows.Forms.ToolStripButton
            Me.m_tsbLoad = New System.Windows.Forms.ToolStripButton
            Me.m_tsbWeight = New System.Windows.Forms.ToolStripButton
            Me.m_ts1 = New System.Windows.Forms.ToolStripSeparator
            Me.m_tsbAdd = New System.Windows.Forms.ToolStripButton
            Me.m_tsbRemove = New System.Windows.Forms.ToolStripButton
            Me.m_tsbDuplicate = New System.Windows.Forms.ToolStripButton
            Me.m_ts2 = New System.Windows.Forms.ToolStripSeparator
            Me.m_tsbSetTo0 = New System.Windows.Forms.ToolStripButton
            Me.m_tsbSetToValue = New System.Windows.Forms.ToolStripButton
            Me.m_tsbResetAll = New System.Windows.Forms.ToolStripButton
            Me.m_ts.SuspendLayout()
            Me.SuspendLayout()
            '
            'm_ts
            '
            Me.m_ts.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
            Me.m_ts.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tsbImport, Me.m_tsbLoad, Me.m_tsbWeight, Me.m_ts1, Me.m_tsbAdd, Me.m_tsbRemove, Me.m_tsbDuplicate, Me.m_ts2, Me.m_tsbSetTo0, Me.m_tsbSetToValue, Me.m_tsbResetAll})
            Me.m_ts.Location = New System.Drawing.Point(0, 0)
            Me.m_ts.Name = "m_ts"
            Me.m_ts.RenderMode = System.Windows.Forms.ToolStripRenderMode.System
            Me.m_ts.Size = New System.Drawing.Size(600, 25)
            Me.m_ts.TabIndex = 0
            Me.m_ts.Text = "ToolStrip1"
            '
            'm_tsbImport
            '
            Me.m_tsbImport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            Me.m_tsbImport.Image = CType(resources.GetObject("m_tsbImport.Image"), System.Drawing.Image)
            Me.m_tsbImport.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.m_tsbImport.Name = "m_tsbImport"
            Me.m_tsbImport.Size = New System.Drawing.Size(55, 22)
            Me.m_tsbImport.Text = "&Import..."
            '
            'm_tsbLoad
            '
            Me.m_tsbLoad.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            Me.m_tsbLoad.Image = CType(resources.GetObject("m_tsbLoad.Image"), System.Drawing.Image)
            Me.m_tsbLoad.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.m_tsbLoad.Name = "m_tsbLoad"
            Me.m_tsbLoad.Size = New System.Drawing.Size(46, 22)
            Me.m_tsbLoad.Text = "&Load..."
            '
            'm_tsbWeight
            '
            Me.m_tsbWeight.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            Me.m_tsbWeight.Image = CType(resources.GetObject("m_tsbWeight.Image"), System.Drawing.Image)
            Me.m_tsbWeight.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.m_tsbWeight.Name = "m_tsbWeight"
            Me.m_tsbWeight.Size = New System.Drawing.Size(57, 22)
            Me.m_tsbWeight.Text = "&Weight..."
            '
            'm_ts1
            '
            Me.m_ts1.Name = "m_ts1"
            Me.m_ts1.Size = New System.Drawing.Size(6, 25)
            '
            'm_tsbAdd
            '
            Me.m_tsbAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            Me.m_tsbAdd.Image = CType(resources.GetObject("m_tsbAdd.Image"), System.Drawing.Image)
            Me.m_tsbAdd.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.m_tsbAdd.Name = "m_tsbAdd"
            Me.m_tsbAdd.Size = New System.Drawing.Size(42, 22)
            Me.m_tsbAdd.Text = "&Add..."
            '
            'm_tsbRemove
            '
            Me.m_tsbRemove.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            Me.m_tsbRemove.Image = CType(resources.GetObject("m_tsbRemove.Image"), System.Drawing.Image)
            Me.m_tsbRemove.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.m_tsbRemove.Name = "m_tsbRemove"
            Me.m_tsbRemove.Size = New System.Drawing.Size(62, 22)
            Me.m_tsbRemove.Text = "&Remove..."
            '
            'm_tsbDuplicate
            '
            Me.m_tsbDuplicate.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            Me.m_tsbDuplicate.Image = CType(resources.GetObject("m_tsbDuplicate.Image"), System.Drawing.Image)
            Me.m_tsbDuplicate.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.m_tsbDuplicate.Name = "m_tsbDuplicate"
            Me.m_tsbDuplicate.Size = New System.Drawing.Size(55, 22)
            Me.m_tsbDuplicate.Text = "&Duplicate"
            '
            'm_ts2
            '
            Me.m_ts2.Name = "m_ts2"
            Me.m_ts2.Size = New System.Drawing.Size(6, 25)
            '
            'm_tsbSetTo0
            '
            Me.m_tsbSetTo0.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            Me.m_tsbSetTo0.Image = CType(resources.GetObject("m_tsbSetTo0.Image"), System.Drawing.Image)
            Me.m_tsbSetTo0.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.m_tsbSetTo0.Name = "m_tsbSetTo0"
            Me.m_tsbSetTo0.Size = New System.Drawing.Size(49, 22)
            Me.m_tsbSetTo0.Text = "&Set to 0"
            '
            'm_tsbSetToValue
            '
            Me.m_tsbSetToValue.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            Me.m_tsbSetToValue.Image = CType(resources.GetObject("m_tsbSetToValue.Image"), System.Drawing.Image)
            Me.m_tsbSetToValue.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.m_tsbSetToValue.Name = "m_tsbSetToValue"
            Me.m_tsbSetToValue.Size = New System.Drawing.Size(81, 22)
            Me.m_tsbSetToValue.Text = "Set to &value..."
            '
            'm_tsbResetAll
            '
            Me.m_tsbResetAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            Me.m_tsbResetAll.Image = CType(resources.GetObject("m_tsbResetAll.Image"), System.Drawing.Image)
            Me.m_tsbResetAll.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.m_tsbResetAll.Name = "m_tsbResetAll"
            Me.m_tsbResetAll.Size = New System.Drawing.Size(53, 22)
            Me.m_tsbResetAll.Text = "Reset &All"
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
        Private WithEvents m_tsbImport As System.Windows.Forms.ToolStripButton
        Private WithEvents m_tsbLoad As System.Windows.Forms.ToolStripButton
        Private WithEvents m_ts1 As System.Windows.Forms.ToolStripSeparator
        Private WithEvents m_tsbAdd As System.Windows.Forms.ToolStripButton
        Private WithEvents m_tsbDuplicate As System.Windows.Forms.ToolStripButton
        Private WithEvents m_tsbRemove As System.Windows.Forms.ToolStripButton
        Private WithEvents m_ts2 As System.Windows.Forms.ToolStripSeparator
        Private WithEvents m_tsbWeight As System.Windows.Forms.ToolStripButton
        Private WithEvents m_tsbSetTo0 As System.Windows.Forms.ToolStripButton
        Private WithEvents m_tsbSetToValue As System.Windows.Forms.ToolStripButton
        Private WithEvents m_tsbResetAll As System.Windows.Forms.ToolStripButton

    End Class

End Namespace
