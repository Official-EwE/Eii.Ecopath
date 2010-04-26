Namespace Controls

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
        Partial Class ucBioPercentToolbar
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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ucBioPercentToolbar))
            Me.tsMenus = New cEwEToolstrip
            Me.m_tsbnDefineXAxis = New System.Windows.Forms.ToolStripButton
            Me.tsMenus.SuspendLayout()
            Me.SuspendLayout()
            '
            'tsMenus
            '
            resources.ApplyResources(Me.tsMenus, "tsMenus")
            Me.tsMenus.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
            Me.tsMenus.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tsbnDefineXAxis})
            Me.tsMenus.Name = "tsMenus"
            Me.tsMenus.RenderMode = System.Windows.Forms.ToolStripRenderMode.System
            '
            'm_tsbnDefineXAxis
            '
            Me.m_tsbnDefineXAxis.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            resources.ApplyResources(Me.m_tsbnDefineXAxis, "m_tsbnDefineXAxis")
            Me.m_tsbnDefineXAxis.Name = "m_tsbnDefineXAxis"
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
        Private WithEvents m_tsbnDefineXAxis As System.Windows.Forms.ToolStripButton
        Private WithEvents tsMenus As cEwEToolstrip

    End Class

End Namespace