Imports WeifenLuo.WinFormsUI.Docking

Namespace Ecosim

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class Vulnerabilities
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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Vulnerabilities))
            Me.tsVUlnerabilities = New System.Windows.Forms.ToolStrip
            Me.tsbEstimateVs = New System.Windows.Forms.ToolStripButton
            Me.plVulGrid = New System.Windows.Forms.Panel
            Me.tsVUlnerabilities.SuspendLayout()
            Me.SuspendLayout()
            '
            'tsVUlnerabilities
            '
            Me.tsVUlnerabilities.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsbEstimateVs})
            resources.ApplyResources(Me.tsVUlnerabilities, "tsVUlnerabilities")
            Me.tsVUlnerabilities.Name = "tsVUlnerabilities"
            '
            'tsbEstimateVs
            '
            Me.tsbEstimateVs.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            resources.ApplyResources(Me.tsbEstimateVs, "tsbEstimateVs")
            Me.tsbEstimateVs.Name = "tsbEstimateVs"
            '
            'plVulGrid
            '
            resources.ApplyResources(Me.plVulGrid, "plVulGrid")
            Me.plVulGrid.Name = "plVulGrid"
            '
            'Vulnerabilities
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.plVulGrid)
            Me.Controls.Add(Me.tsVUlnerabilities)
            Me.Name = "Vulnerabilities"
            Me.tsVUlnerabilities.ResumeLayout(False)
            Me.tsVUlnerabilities.PerformLayout()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Friend WithEvents tsVUlnerabilities As System.Windows.Forms.ToolStrip
        Friend WithEvents tsbEstimateVs As System.Windows.Forms.ToolStripButton
        Friend WithEvents plVulGrid As System.Windows.Forms.Panel

    End Class

End Namespace