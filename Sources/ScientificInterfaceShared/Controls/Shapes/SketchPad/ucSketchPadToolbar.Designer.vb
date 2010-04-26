Namespace Controls
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class ucSketchPadToolbar
        Inherits System.Windows.Forms.UserControl

        'UserControl overrides dispose to clean up the component list.
        <System.Diagnostics.DebuggerNonUserCode()> _
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
            MyBase.Dispose(disposing)
        End Sub

        'Required by the Windows Form Designer
        Private components As System.ComponentModel.IContainer

        'NOTE: The following procedure is required by the Windows Form Designer
        'It can be modified using the Windows Form Designer.  
        'Do not modify it using the code editor.
        <System.Diagnostics.DebuggerStepThrough()> _
        Private Sub InitializeComponent()
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ucSketchPadToolbar))
            Me.tsMenus = New cEwEToolstrip
            Me.tsBtnSave = New System.Windows.Forms.ToolStripButton
            Me.ToolStripSeparator4 = New System.Windows.Forms.ToolStripSeparator
            Me.tslbShapeView = New System.Windows.Forms.ToolStripLabel
            Me.tscbbShapeView = New System.Windows.Forms.ToolStripComboBox
            Me.m_tslWeight = New System.Windows.Forms.ToolStripLabel
            Me.m_tstbWeight = New System.Windows.Forms.ToolStripTextBox
            Me.tsBtnValue = New System.Windows.Forms.ToolStripButton
            Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator
            Me.tsbChangeShape = New System.Windows.Forms.ToolStripButton
            Me.tsBtnReset = New System.Windows.Forms.ToolStripButton
            Me.ToolStripSeparator5 = New System.Windows.Forms.ToolStripSeparator
            Me.tsBtnOptions = New System.Windows.Forms.ToolStripButton
            Me.tsMenus.SuspendLayout()
            Me.SuspendLayout()
            '
            'tsMenus
            '
            resources.ApplyResources(Me.tsMenus, "tsMenus")
            Me.tsMenus.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
            Me.tsMenus.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsBtnSave, Me.ToolStripSeparator4, Me.tslbShapeView, Me.tscbbShapeView, Me.m_tslWeight, Me.m_tstbWeight, Me.tsBtnValue, Me.ToolStripSeparator1, Me.tsbChangeShape, Me.tsBtnReset, Me.ToolStripSeparator5, Me.tsBtnOptions})
            Me.tsMenus.Name = "tsMenus"
            Me.tsMenus.RenderMode = System.Windows.Forms.ToolStripRenderMode.System
            '
            'tsBtnSave
            '
            Me.tsBtnSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            resources.ApplyResources(Me.tsBtnSave, "tsBtnSave")
            Me.tsBtnSave.Name = "tsBtnSave"
            '
            'ToolStripSeparator4
            '
            Me.ToolStripSeparator4.Name = "ToolStripSeparator4"
            resources.ApplyResources(Me.ToolStripSeparator4, "ToolStripSeparator4")
            '
            'tslbShapeView
            '
            Me.tslbShapeView.Name = "tslbShapeView"
            resources.ApplyResources(Me.tslbShapeView, "tslbShapeView")
            '
            'tscbbShapeView
            '
            Me.tscbbShapeView.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.tscbbShapeView.Items.AddRange(New Object() {resources.GetString("tscbbShapeView.Items"), resources.GetString("tscbbShapeView.Items1")})
            Me.tscbbShapeView.Name = "tscbbShapeView"
            resources.ApplyResources(Me.tscbbShapeView, "tscbbShapeView")
            '
            'm_tslWeight
            '
            Me.m_tslWeight.Name = "m_tslWeight"
            resources.ApplyResources(Me.m_tslWeight, "m_tslWeight")
            '
            'm_tstbWeight
            '
            Me.m_tstbWeight.AcceptsReturn = True
            resources.ApplyResources(Me.m_tstbWeight, "m_tstbWeight")
            Me.m_tstbWeight.Name = "m_tstbWeight"
            '
            'tsBtnValue
            '
            Me.tsBtnValue.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            resources.ApplyResources(Me.tsBtnValue, "tsBtnValue")
            Me.tsBtnValue.Name = "tsBtnValue"
            '
            'ToolStripSeparator1
            '
            Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
            resources.ApplyResources(Me.ToolStripSeparator1, "ToolStripSeparator1")
            '
            'tsbChangeShape
            '
            Me.tsbChangeShape.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            resources.ApplyResources(Me.tsbChangeShape, "tsbChangeShape")
            Me.tsbChangeShape.Name = "tsbChangeShape"
            '
            'tsBtnReset
            '
            Me.tsBtnReset.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            resources.ApplyResources(Me.tsBtnReset, "tsBtnReset")
            Me.tsBtnReset.Name = "tsBtnReset"
            '
            'ToolStripSeparator5
            '
            Me.ToolStripSeparator5.Name = "ToolStripSeparator5"
            resources.ApplyResources(Me.ToolStripSeparator5, "ToolStripSeparator5")
            '
            'tsBtnOptions
            '
            Me.tsBtnOptions.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            resources.ApplyResources(Me.tsBtnOptions, "tsBtnOptions")
            Me.tsBtnOptions.Name = "tsBtnOptions"
            '
            'ucSketchPadToolbar
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.BackColor = System.Drawing.SystemColors.Control
            Me.Controls.Add(Me.tsMenus)
            Me.Name = "ucSketchPadToolbar"
            Me.tsMenus.ResumeLayout(False)
            Me.tsMenus.PerformLayout()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents tsMenus As cEwEToolstrip
        Private WithEvents tsBtnReset As System.Windows.Forms.ToolStripButton
        Private WithEvents tsBtnValue As System.Windows.Forms.ToolStripButton
        Private WithEvents tsBtnSave As System.Windows.Forms.ToolStripButton
        Private WithEvents ToolStripSeparator4 As System.Windows.Forms.ToolStripSeparator
        Private WithEvents tsBtnOptions As System.Windows.Forms.ToolStripButton
        Private WithEvents ToolStripSeparator5 As System.Windows.Forms.ToolStripSeparator
        Private WithEvents tscbbShapeView As System.Windows.Forms.ToolStripComboBox
        Private WithEvents tslbShapeView As System.Windows.Forms.ToolStripLabel
        Private WithEvents ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
        Private WithEvents tsbChangeShape As System.Windows.Forms.ToolStripButton
        Private WithEvents m_tslWeight As System.Windows.Forms.ToolStripLabel
        Private WithEvents m_tstbWeight As System.Windows.Forms.ToolStripTextBox

    End Class

End Namespace
