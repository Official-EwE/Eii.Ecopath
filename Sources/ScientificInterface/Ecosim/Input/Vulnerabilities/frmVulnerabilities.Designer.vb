Imports ScientificInterfaceShared.Forms

Namespace Ecosim

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class frmVulnerabilities
        Inherits frmEwEGrid

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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmVulnerabilities))
            Me.m_tsVUlnerabilities = New System.Windows.Forms.ToolStrip
            Me.m_tsbEstimateVs = New System.Windows.Forms.ToolStripButton
            Me.m_tsVUlnerabilities.SuspendLayout()
            Me.SuspendLayout()
            '
            'm_tsVUlnerabilities
            '
            Me.m_tsVUlnerabilities.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
            Me.m_tsVUlnerabilities.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tsbEstimateVs})
            resources.ApplyResources(Me.m_tsVUlnerabilities, "m_tsVUlnerabilities")
            Me.m_tsVUlnerabilities.Name = "m_tsVUlnerabilities"
            '
            'm_tsbEstimateVs
            '
            resources.ApplyResources(Me.m_tsbEstimateVs, "m_tsbEstimateVs")
            Me.m_tsbEstimateVs.Image = ScientificInterfaceShared.My.Resources.CalculatorHS
            Me.m_tsbEstimateVs.Name = "m_tsbEstimateVs"
            '
            'Vulnerabilities
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.m_tsVUlnerabilities)
            Me.Name = "Vulnerabilities"
            Me.m_tsVUlnerabilities.ResumeLayout(False)
            Me.m_tsVUlnerabilities.PerformLayout()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents m_tsVUlnerabilities As System.Windows.Forms.ToolStrip
        Private WithEvents m_tsbEstimateVs As System.Windows.Forms.ToolStripButton

    End Class

End Namespace