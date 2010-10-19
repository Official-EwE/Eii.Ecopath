Imports WeifenLuo.WinFormsUI.Docking
Imports ScientificInterfaceShared.Forms

Namespace Ecosim

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class frmGroupRefLevels
        Inherits frmEwEGrid

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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmGroupRefLevels))
            Me.m_ts = New System.Windows.Forms.ToolStrip
            Me.m_tsbnReset = New System.Windows.Forms.ToolStripButton
            Me.m_ts.SuspendLayout()
            Me.SuspendLayout()
            '
            'm_ts
            '
            Me.m_ts.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tsbnReset})
            resources.ApplyResources(Me.m_ts, "m_ts")
            Me.m_ts.Name = "m_ts"
            '
            'm_tsbnReset
            '
            Me.m_tsbnReset.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            resources.ApplyResources(Me.m_tsbnReset, "m_tsbnReset")
            Me.m_tsbnReset.Name = "m_tsbnReset"
            '
            'frmGroupRefLevels
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.m_ts)
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "frmGroupRefLevels"
            Me.ShowInTaskbar = False
            Me.m_ts.ResumeLayout(False)
            Me.m_ts.PerformLayout()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents m_ts As System.Windows.Forms.ToolStrip
        Private WithEvents m_tsbnReset As System.Windows.Forms.ToolStripButton

    End Class

End Namespace
