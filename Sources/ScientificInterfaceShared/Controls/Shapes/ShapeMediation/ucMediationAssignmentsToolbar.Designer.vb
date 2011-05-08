Namespace Controls

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
        Partial Class ucMediationAssignmentsToolbar
        Inherits System.Windows.Forms.UserControl

        'Required by the Windows Form Designer
        Private components As System.ComponentModel.IContainer

        'NOTE: The following procedure is required by the Windows Form Designer
        'It can be modified using the Windows Form Designer.  
        'Do not modify it using the code editor.
        <System.Diagnostics.DebuggerStepThrough()> _
        Private Sub InitializeComponent()
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ucMediationAssignmentsToolbar))
            Me.tsMenus = New ScientificInterfaceShared.Controls.cEwEToolstrip
            Me.m_tsbnDefineMediatingItems = New System.Windows.Forms.ToolStripButton
            Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator
            Me.m_tslShowAs = New System.Windows.Forms.ToolStripLabel
            Me.m_tscmbShowAs = New System.Windows.Forms.ToolStripComboBox
            Me.tsMenus.SuspendLayout()
            Me.SuspendLayout()
            '
            'tsMenus
            '
            resources.ApplyResources(Me.tsMenus, "tsMenus")
            Me.tsMenus.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
            Me.tsMenus.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tsbnDefineMediatingItems, Me.ToolStripSeparator1, Me.m_tslShowAs, Me.m_tscmbShowAs})
            Me.tsMenus.Name = "tsMenus"
            Me.tsMenus.RenderMode = System.Windows.Forms.ToolStripRenderMode.System
            '
            'm_tsbnDefineMediatingItems
            '
            Me.m_tsbnDefineMediatingItems.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            resources.ApplyResources(Me.m_tsbnDefineMediatingItems, "m_tsbnDefineMediatingItems")
            Me.m_tsbnDefineMediatingItems.Name = "m_tsbnDefineMediatingItems"
            '
            'ToolStripSeparator1
            '
            Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
            resources.ApplyResources(Me.ToolStripSeparator1, "ToolStripSeparator1")
            '
            'm_tslShowAs
            '
            Me.m_tslShowAs.Name = "m_tslShowAs"
            resources.ApplyResources(Me.m_tslShowAs, "m_tslShowAs")
            '
            'm_tscmbShowAs
            '
            Me.m_tscmbShowAs.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.m_tscmbShowAs.Items.AddRange(New Object() {resources.GetString("m_tscmbShowAs.Items"), resources.GetString("m_tscmbShowAs.Items1")})
            Me.m_tscmbShowAs.Name = "m_tscmbShowAs"
            resources.ApplyResources(Me.m_tscmbShowAs, "m_tscmbShowAs")
            '
            'ucMediationAssignmentsToolbar
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.BackColor = System.Drawing.SystemColors.Control
            Me.Controls.Add(Me.tsMenus)
            Me.Name = "ucMediationAssignmentsToolbar"
            Me.tsMenus.ResumeLayout(False)
            Me.tsMenus.PerformLayout()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents m_tsbnDefineMediatingItems As System.Windows.Forms.ToolStripButton
        Private WithEvents tsMenus As cEwEToolstrip
        Friend WithEvents ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
        Private WithEvents m_tslShowAs As System.Windows.Forms.ToolStripLabel
        Private WithEvents m_tscmbShowAs As System.Windows.Forms.ToolStripComboBox

    End Class

End Namespace