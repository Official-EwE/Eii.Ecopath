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
            Me.tsMenus.SuspendLayout()
            Me.SuspendLayout()
            '
            'tsMenus
            '
            resources.ApplyResources(Me.tsMenus, "tsMenus")
            Me.tsMenus.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
            Me.tsMenus.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tsbnDefineMediatingItems})
            Me.tsMenus.Name = "tsMenus"
            Me.tsMenus.RenderMode = System.Windows.Forms.ToolStripRenderMode.System
            '
            'm_tsbnDefineMediatingItems
            '
            Me.m_tsbnDefineMediatingItems.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            resources.ApplyResources(Me.m_tsbnDefineMediatingItems, "m_tsbnDefineMediatingItems")
            Me.m_tsbnDefineMediatingItems.Name = "m_tsbnDefineMediatingItems"
            '
            'ucBioPercentToolbar
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.BackColor = System.Drawing.SystemColors.Control
            Me.Controls.Add(Me.tsMenus)
            Me.Name = "ucBioPercentToolbar"
            Me.tsMenus.ResumeLayout(False)
            Me.tsMenus.PerformLayout()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents m_tsbnDefineMediatingItems As System.Windows.Forms.ToolStripButton
        Private WithEvents tsMenus As cEwEToolstrip

    End Class

End Namespace