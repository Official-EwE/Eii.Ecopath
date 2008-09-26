Imports WeifenLuo

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmNetworkNav
    Inherits WeifenLuo.WinFormsUI.DockContent

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
        Dim TreeNode24 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Relative flows")
        Dim TreeNode25 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Absolute flows")
        Dim TreeNode26 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Trophic level decomposition", New System.Windows.Forms.TreeNode() {TreeNode24, TreeNode25})
        Dim TreeNode27 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("From primary producers")
        Dim TreeNode28 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("From detritus")
        Dim TreeNode29 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("From all combined")
        Dim TreeNode30 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Flows and biomasses", New System.Windows.Forms.TreeNode() {TreeNode27, TreeNode28, TreeNode29})
        Dim TreeNode31 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("For harvest of all groups")
        Dim TreeNode32 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("For consumption of all groups")
        Dim TreeNode33 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Primary production required", New System.Windows.Forms.TreeNode() {TreeNode31, TreeNode32})
        Dim TreeNode34 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Mixed trophic impact")
        Dim TreeNode35 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Total")
        Dim TreeNode36 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("By group")
        Dim TreeNode37 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Ascendency", New System.Windows.Forms.TreeNode() {TreeNode35, TreeNode36})
        Dim TreeNode38 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Flow from detritus")
        Dim TreeNode39 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Consumer <- TL1")
        Dim TreeNode40 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Consumer <- prey <- TL1")
        Dim TreeNode41 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Top predator <- prey")
        Dim TreeNode42 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Cycles (living)")
        Dim TreeNode43 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Cycles (all)")
        Dim TreeNode44 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Cycling and path length")
        Dim TreeNode45 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Cycles and pathways", New System.Windows.Forms.TreeNode() {TreeNode39, TreeNode40, TreeNode41, TreeNode42, TreeNode43, TreeNode44})
        Dim TreeNode46 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("EwE Network Analysis Plugin", New System.Windows.Forms.TreeNode() {TreeNode26, TreeNode30, TreeNode33, TreeNode34, TreeNode37, TreeNode38, TreeNode45})
        Me.tvNavigation = New System.Windows.Forms.TreeView
        Me.Panel1 = New System.Windows.Forms.Panel
        Me.SuspendLayout()
        '
        'tvNavigation
        '
        Me.tvNavigation.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.tvNavigation.Location = New System.Drawing.Point(12, 12)
        Me.tvNavigation.Name = "tvNavigation"
        TreeNode24.Name = "ndRelativeFlows"
        TreeNode24.Text = "Relative flows"
        TreeNode25.Name = "ndAbsoluteFlows"
        TreeNode25.Text = "Absolute flows"
        TreeNode26.Name = "ndTrophicLlevelDdecomposition"
        TreeNode26.Text = "Trophic level decomposition"
        TreeNode27.Name = "ndFromPrimaryProducers"
        TreeNode27.Text = "From primary producers"
        TreeNode28.Name = "ndFromDetritus"
        TreeNode28.Text = "From detritus"
        TreeNode29.Name = "ndFromAllCombined"
        TreeNode29.Text = "From all combined"
        TreeNode30.Name = "ndFlowsAndBiomasses"
        TreeNode30.Text = "Flows and biomasses"
        TreeNode31.Name = "ndForHarvestOfAllGroups"
        TreeNode31.Text = "For harvest of all groups"
        TreeNode32.Name = "ndForConsumptionOfAllGroups"
        TreeNode32.Text = "For consumption of all groups"
        TreeNode33.Name = "ndPrimaryProductionRequired"
        TreeNode33.Text = "Primary production required"
        TreeNode34.Name = "ndMixedTrophicImpact"
        TreeNode34.Text = "Mixed trophic impact"
        TreeNode35.Name = "ndTotal"
        TreeNode35.Text = "Total"
        TreeNode36.Name = "ndByGroup"
        TreeNode36.Text = "By group"
        TreeNode37.Name = "ndAscendency"
        TreeNode37.Text = "Ascendency"
        TreeNode38.Name = "ndFlowFromDetritus"
        TreeNode38.Text = "Flow from detritus"
        TreeNode39.Name = "ndConsumer<-TL1"
        TreeNode39.Text = "Consumer <- TL1"
        TreeNode40.Name = "ndConsumer<-Prey<-TL1"
        TreeNode40.Text = "Consumer <- prey <- TL1"
        TreeNode41.Name = "ndTopPredator<-Prey"
        TreeNode41.Text = "Top predator <- prey"
        TreeNode42.Name = "ndCycles(living)"
        TreeNode42.Text = "Cycles (living)"
        TreeNode43.Name = "ndCycles(all)"
        TreeNode43.Text = "Cycles (all)"
        TreeNode44.Name = "ndCyclingAndPathLength"
        TreeNode44.Text = "Cycling and path length"
        TreeNode45.Name = "ndCyclesAndPathways"
        TreeNode45.Text = "Cycles and pathways"
        TreeNode46.Name = "ndEwENetworkAnalysisPlugin"
        TreeNode46.Text = "EwE Network Analysis Plugin"
        Me.tvNavigation.Nodes.AddRange(New System.Windows.Forms.TreeNode() {TreeNode46})
        Me.tvNavigation.Size = New System.Drawing.Size(224, 258)
        Me.tvNavigation.TabIndex = 0
        '
        'Panel1
        '
        Me.Panel1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Panel1.Location = New System.Drawing.Point(242, 12)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(409, 258)
        Me.Panel1.TabIndex = 1
        '
        'frmNetworkNav
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(663, 282)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.tvNavigation)
        Me.Name = "frmNetworkNav"
        Me.TabText = "EwE Network Analysis Plugin"
        Me.Text = "EwE Network Analysis Plugin"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents tvNavigation As System.Windows.Forms.TreeView
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
End Class
