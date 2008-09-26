Imports WeifenLuo.WinFormsUI.Docking

Namespace Ecopath.Input

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class DietComp
        Inherits frmEwEGrid

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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(DietComp))
            Me.ToolStrip1 = New System.Windows.Forms.ToolStrip
            Me.tsSumtoOneBtn = New System.Windows.Forms.ToolStripButton
            Me.plDietCompGrid = New System.Windows.Forms.Panel
            Me.ToolStrip1.SuspendLayout()
            Me.SuspendLayout()
            '
            'ToolStrip1
            '
            Me.ToolStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsSumtoOneBtn})
            resources.ApplyResources(Me.ToolStrip1, "ToolStrip1")
            Me.ToolStrip1.Name = "ToolStrip1"
            '
            'tsSumtoOneBtn
            '
            Me.tsSumtoOneBtn.Image = Global.ScientificInterface.My.Resources.Resources.OptionsIconSM
            resources.ApplyResources(Me.tsSumtoOneBtn, "tsSumtoOneBtn")
            Me.tsSumtoOneBtn.Name = "tsSumtoOneBtn"
            '
            'plDietCompGrid
            '
            resources.ApplyResources(Me.plDietCompGrid, "plDietCompGrid")
            Me.plDietCompGrid.Name = "plDietCompGrid"
            '
            'DietComp
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.plDietCompGrid)
            Me.Controls.Add(Me.ToolStrip1)
            Me.Name = "DietComp"
            Me.ToolStrip1.ResumeLayout(False)
            Me.ToolStrip1.PerformLayout()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Friend WithEvents ToolStrip1 As System.Windows.Forms.ToolStrip
        Friend WithEvents plDietCompGrid As System.Windows.Forms.Panel
        Friend WithEvents tsSumtoOneBtn As System.Windows.Forms.ToolStripButton
    End Class

End Namespace
