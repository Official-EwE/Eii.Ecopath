Namespace Ecospace

    Partial Class frmMPAs
        Inherits ScientificInterfaceShared.Forms.frmEwEGrid

        'Form overrides dispose to clean up the component list.
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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMPAs))
            Me.m_tsMain = New System.Windows.Forms.ToolStrip()
            Me.m_tsbnDefineMPAs = New System.Windows.Forms.ToolStripButton()
            Me.m_grid = New gridMPAs()
            Me.m_tsMain.SuspendLayout()
            Me.SuspendLayout()
            '
            'm_tsMain
            '
            Me.m_tsMain.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tsbnDefineMPAs})
            resources.ApplyResources(Me.m_tsMain, "m_tsMain")
            Me.m_tsMain.Name = "m_tsMain"
            '
            'm_tsbnDefineMPAs
            '
            resources.ApplyResources(Me.m_tsbnDefineMPAs, "m_tsbnDefineMPAs")
            Me.m_tsbnDefineMPAs.Name = "m_tsbnDefineMPAs"
            '
            'm_grid
            '
            resources.ApplyResources(Me.m_grid, "m_grid")
            Me.m_grid.Name = "m_grid"
            '
            'frmMPAs
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.m_grid)
            Me.Controls.Add(Me.m_tsMain)
            Me.Name = "frmMPAs"
            Me.m_tsMain.ResumeLayout(False)
            Me.m_tsMain.PerformLayout()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents m_tsMain As System.Windows.Forms.ToolStrip
        Private WithEvents m_tsbnDefineMPAs As System.Windows.Forms.ToolStripButton
        Private WithEvents m_grid As gridMPAs
    End Class

End Namespace
