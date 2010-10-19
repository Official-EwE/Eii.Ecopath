Imports ScientificInterfaceShared.Forms
Imports WeifenLuo.WinFormsUI.Docking

Namespace Ecosim
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    <CLSCompliant(False)> _
    Partial Class frmApplyFFConsumer
        Inherits frmApplyShapeBase

        'Form overrides dispose to clean up the component list.
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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmApplyFFConsumer))
            Me.plApplyFFGrid = New System.Windows.Forms.Panel
            Me.ToolStrip1 = New System.Windows.Forms.ToolStrip
            Me.tsBtnClearAll = New System.Windows.Forms.ToolStripButton
            Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator
            Me.tsBtnSetAll = New System.Windows.Forms.ToolStripButton
            Me.ToolStrip1.SuspendLayout()
            Me.SuspendLayout()
            '
            'plApplyFFGrid
            '
            resources.ApplyResources(Me.plApplyFFGrid, "plApplyFFGrid")
            Me.plApplyFFGrid.Name = "plApplyFFGrid"
            '
            'ToolStrip1
            '
            Me.ToolStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsBtnClearAll, Me.ToolStripSeparator1, Me.tsBtnSetAll})
            resources.ApplyResources(Me.ToolStrip1, "ToolStrip1")
            Me.ToolStrip1.Name = "ToolStrip1"
            Me.ToolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System
            '
            'tsBtnClearAll
            '
            Me.tsBtnClearAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            resources.ApplyResources(Me.tsBtnClearAll, "tsBtnClearAll")
            Me.tsBtnClearAll.Name = "tsBtnClearAll"
            '
            'ToolStripSeparator1
            '
            Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
            resources.ApplyResources(Me.ToolStripSeparator1, "ToolStripSeparator1")
            '
            'tsBtnSetAll
            '
            Me.tsBtnSetAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            resources.ApplyResources(Me.tsBtnSetAll, "tsBtnSetAll")
            Me.tsBtnSetAll.Name = "tsBtnSetAll"
            '
            'frmApplyFFConsumer
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.ToolStrip1)
            Me.Controls.Add(Me.plApplyFFGrid)
            Me.Name = "frmApplyFFConsumer"
            Me.TabText = "Apply FF (cons)"
            Me.Controls.SetChildIndex(Me.plApplyFFGrid, 0)
            Me.Controls.SetChildIndex(Me.ToolStrip1, 0)
            Me.ToolStrip1.ResumeLayout(False)
            Me.ToolStrip1.PerformLayout()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Friend WithEvents plApplyFFGrid As System.Windows.Forms.Panel
        Friend WithEvents ToolStrip1 As System.Windows.Forms.ToolStrip
        Friend WithEvents tsBtnClearAll As System.Windows.Forms.ToolStripButton
        Friend WithEvents tsBtnSetAll As System.Windows.Forms.ToolStripButton
        Friend WithEvents ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator

    End Class
End Namespace

