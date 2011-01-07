Imports ScientificInterfaceShared.Forms

Namespace Ecosim
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    <CLSCompliant(False)> _
    Partial Class frmApplyMedPrimaryProducer
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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmApplyMedPrimaryProducer))
            Me.plApplyFFGrid = New System.Windows.Forms.Panel
            Me.m_ts = New System.Windows.Forms.ToolStrip
            Me.tsBtnClearAll = New System.Windows.Forms.ToolStripButton
            Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator
            Me.tsBtnSetAll = New System.Windows.Forms.ToolStripButton
            Me.m_ts.SuspendLayout()
            Me.SuspendLayout()
            '
            'plApplyFFGrid
            '
            resources.ApplyResources(Me.plApplyFFGrid, "plApplyFFGrid")
            Me.plApplyFFGrid.Name = "plApplyFFGrid"
            '
            'm_ts
            '
            Me.m_ts.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
            Me.m_ts.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsBtnClearAll, Me.ToolStripSeparator1, Me.tsBtnSetAll})
            resources.ApplyResources(Me.m_ts, "m_ts")
            Me.m_ts.Name = "m_ts"
            Me.m_ts.RenderMode = System.Windows.Forms.ToolStripRenderMode.System
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
            'frmApplyMedPrimaryProducer
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.m_ts)
            Me.Controls.Add(Me.plApplyFFGrid)
            Me.Name = "frmApplyMedPrimaryProducer"
            Me.TabText = "Apply shapes"
            Me.Controls.SetChildIndex(Me.plApplyFFGrid, 0)
            Me.Controls.SetChildIndex(Me.m_ts, 0)
            Me.m_ts.ResumeLayout(False)
            Me.m_ts.PerformLayout()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Friend WithEvents plApplyFFGrid As System.Windows.Forms.Panel
        Friend WithEvents tsBtnClearAll As System.Windows.Forms.ToolStripButton
        Friend WithEvents tsBtnSetAll As System.Windows.Forms.ToolStripButton
        Friend WithEvents ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
        Private WithEvents m_ts As System.Windows.Forms.ToolStrip

    End Class
End Namespace

