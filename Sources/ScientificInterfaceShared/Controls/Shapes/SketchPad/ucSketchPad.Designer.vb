Namespace Controls
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class ucSketchPad
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
            Me.components = New System.ComponentModel.Container
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ucSketchPad))
            Me.spContextMenuStrip = New System.Windows.Forms.ContextMenuStrip(Me.components)
            Me.DrawModeToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
            Me.FillToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
            Me.LineToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
            Me.AxisToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
            Me.ScaleYAxisToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
            Me.AutoScaleToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
            Me.RightMouseButtonToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
            Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator
            Me.ResetToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
            Me.ValueToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
            Me.LoadToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
            Me.SaveToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
            Me.ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator
            Me.OptionsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
            Me.DotsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
            Me.spContextMenuStrip.SuspendLayout()
            Me.SuspendLayout()
            '
            'spContextMenuStrip
            '
            Me.spContextMenuStrip.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.DrawModeToolStripMenuItem, Me.AxisToolStripMenuItem, Me.ScaleYAxisToolStripMenuItem, Me.ToolStripSeparator1, Me.ResetToolStripMenuItem, Me.ValueToolStripMenuItem, Me.LoadToolStripMenuItem, Me.SaveToolStripMenuItem, Me.ToolStripSeparator2, Me.OptionsToolStripMenuItem})
            Me.spContextMenuStrip.Name = "ContextMenuStrip1"
            resources.ApplyResources(Me.spContextMenuStrip, "spContextMenuStrip")
            '
            'DrawModeToolStripMenuItem
            '
            Me.DrawModeToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.FillToolStripMenuItem, Me.LineToolStripMenuItem, Me.DotsToolStripMenuItem})
            Me.DrawModeToolStripMenuItem.Name = "DrawModeToolStripMenuItem"
            resources.ApplyResources(Me.DrawModeToolStripMenuItem, "DrawModeToolStripMenuItem")
            '
            'FillToolStripMenuItem
            '
            Me.FillToolStripMenuItem.Name = "FillToolStripMenuItem"
            resources.ApplyResources(Me.FillToolStripMenuItem, "FillToolStripMenuItem")
            '
            'LineToolStripMenuItem
            '
            Me.LineToolStripMenuItem.Name = "LineToolStripMenuItem"
            resources.ApplyResources(Me.LineToolStripMenuItem, "LineToolStripMenuItem")
            '
            'AxisToolStripMenuItem
            '
            Me.AxisToolStripMenuItem.Name = "AxisToolStripMenuItem"
            resources.ApplyResources(Me.AxisToolStripMenuItem, "AxisToolStripMenuItem")
            '
            'ScaleYAxisToolStripMenuItem
            '
            Me.ScaleYAxisToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.AutoScaleToolStripMenuItem, Me.RightMouseButtonToolStripMenuItem})
            Me.ScaleYAxisToolStripMenuItem.Name = "ScaleYAxisToolStripMenuItem"
            resources.ApplyResources(Me.ScaleYAxisToolStripMenuItem, "ScaleYAxisToolStripMenuItem")
            '
            'AutoScaleToolStripMenuItem
            '
            Me.AutoScaleToolStripMenuItem.Name = "AutoScaleToolStripMenuItem"
            resources.ApplyResources(Me.AutoScaleToolStripMenuItem, "AutoScaleToolStripMenuItem")
            '
            'RightMouseButtonToolStripMenuItem
            '
            Me.RightMouseButtonToolStripMenuItem.Name = "RightMouseButtonToolStripMenuItem"
            resources.ApplyResources(Me.RightMouseButtonToolStripMenuItem, "RightMouseButtonToolStripMenuItem")
            '
            'ToolStripSeparator1
            '
            Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
            resources.ApplyResources(Me.ToolStripSeparator1, "ToolStripSeparator1")
            '
            'ResetToolStripMenuItem
            '
            Me.ResetToolStripMenuItem.Name = "ResetToolStripMenuItem"
            resources.ApplyResources(Me.ResetToolStripMenuItem, "ResetToolStripMenuItem")
            '
            'ValueToolStripMenuItem
            '
            Me.ValueToolStripMenuItem.Name = "ValueToolStripMenuItem"
            resources.ApplyResources(Me.ValueToolStripMenuItem, "ValueToolStripMenuItem")
            '
            'LoadToolStripMenuItem
            '
            resources.ApplyResources(Me.LoadToolStripMenuItem, "LoadToolStripMenuItem")
            Me.LoadToolStripMenuItem.Name = "LoadToolStripMenuItem"
            '
            'SaveToolStripMenuItem
            '
            Me.SaveToolStripMenuItem.Name = "SaveToolStripMenuItem"
            resources.ApplyResources(Me.SaveToolStripMenuItem, "SaveToolStripMenuItem")
            '
            'ToolStripSeparator2
            '
            Me.ToolStripSeparator2.Name = "ToolStripSeparator2"
            resources.ApplyResources(Me.ToolStripSeparator2, "ToolStripSeparator2")
            '
            'OptionsToolStripMenuItem
            '
            Me.OptionsToolStripMenuItem.Name = "OptionsToolStripMenuItem"
            resources.ApplyResources(Me.OptionsToolStripMenuItem, "OptionsToolStripMenuItem")
            '
            'DotsToolStripMenuItem
            '
            Me.DotsToolStripMenuItem.Name = "DotsToolStripMenuItem"
            resources.ApplyResources(Me.DotsToolStripMenuItem, "DotsToolStripMenuItem")
            '
            'SketchPad
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.BackColor = System.Drawing.SystemColors.Window
            Me.ContextMenuStrip = Me.spContextMenuStrip
            Me.Name = "SketchPad"
            Me.spContextMenuStrip.ResumeLayout(False)
            Me.ResumeLayout(False)

        End Sub
        Friend WithEvents spContextMenuStrip As System.Windows.Forms.ContextMenuStrip
        Friend WithEvents DrawModeToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents FillToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents LineToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents AxisToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents ScaleYAxisToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents AutoScaleToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents RightMouseButtonToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents ResetToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents ValueToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents LoadToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents SaveToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents OptionsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
        Friend WithEvents ToolStripSeparator2 As System.Windows.Forms.ToolStripSeparator
        Friend WithEvents DotsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem

    End Class

End Namespace
