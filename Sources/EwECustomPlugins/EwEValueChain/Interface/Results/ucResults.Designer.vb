Imports ScientificInterfaceShared.Controls

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ucResults
    Inherits System.Windows.Forms.UserControl

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ucResults))
        Me.m_btnRunEcopath = New System.Windows.Forms.Button
        Me.m_tsResults = New System.Windows.Forms.ToolStrip
        Me.m_tslFleets = New System.Windows.Forms.ToolStripLabel
        Me.m_tscmbFleets = New System.Windows.Forms.ToolStripComboBox
        Me.m_tssep1 = New System.Windows.Forms.ToolStripSeparator
        Me.m_tsbShowFlow = New System.Windows.Forms.ToolStripButton
        Me.m_tssep2 = New System.Windows.Forms.ToolStripSeparator
        Me.m_tsbEcopath = New System.Windows.Forms.ToolStripButton
        Me.m_tsbEcosim = New System.Windows.Forms.ToolStripButton
        Me.m_tsbEquilibrium = New System.Windows.Forms.ToolStripButton
        Me.m_btnRunEcosim = New System.Windows.Forms.Button
        Me.m_scResults = New System.Windows.Forms.SplitContainer
        Me.m_plFlow = New EwEValueChainPlugin.plFlow
        Me.m_btnRunEquilibrium = New System.Windows.Forms.Button
        Me.m_tscmbGraphData = New System.Windows.Forms.ToolStripComboBox
        Me.m_tslblData = New System.Windows.Forms.ToolStripLabel
        Me.m_tssSep3 = New System.Windows.Forms.ToolStripSeparator
        Me.m_tsResults.SuspendLayout()
        Me.m_scResults.Panel1.SuspendLayout()
        Me.m_scResults.SuspendLayout()
        Me.SuspendLayout()
        '
        'm_btnRunEcopath
        '
        Me.m_btnRunEcopath.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_btnRunEcopath.FlatStyle = System.Windows.Forms.FlatStyle.Popup
        Me.m_btnRunEcopath.Location = New System.Drawing.Point(437, 529)
        Me.m_btnRunEcopath.Margin = New System.Windows.Forms.Padding(0)
        Me.m_btnRunEcopath.Name = "m_btnRunEcopath"
        Me.m_btnRunEcopath.Size = New System.Drawing.Size(100, 23)
        Me.m_btnRunEcopath.TabIndex = 0
        Me.m_btnRunEcopath.Text = "&Run Ecopath"
        Me.m_btnRunEcopath.UseVisualStyleBackColor = True
        '
        'm_tsResults
        '
        Me.m_tsResults.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tslblData, Me.m_tscmbGraphData, Me.m_tssSep3, Me.m_tslFleets, Me.m_tscmbFleets, Me.m_tssep1, Me.m_tsbShowFlow, Me.m_tssep2, Me.m_tsbEcopath, Me.m_tsbEcosim, Me.m_tsbEquilibrium})
        Me.m_tsResults.Location = New System.Drawing.Point(0, 0)
        Me.m_tsResults.Name = "m_tsResults"
        Me.m_tsResults.Size = New System.Drawing.Size(751, 25)
        Me.m_tsResults.TabIndex = 2
        '
        'm_tslFleets
        '
        Me.m_tslFleets.Name = "m_tslFleets"
        Me.m_tslFleets.Size = New System.Drawing.Size(35, 22)
        Me.m_tslFleets.Text = "&Fleet:"
        '
        'm_tscmbFleets
        '
        Me.m_tscmbFleets.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.m_tscmbFleets.Name = "m_tscmbFleets"
        Me.m_tscmbFleets.Size = New System.Drawing.Size(150, 25)
        '
        'm_tssep1
        '
        Me.m_tssep1.Name = "m_tssep1"
        Me.m_tssep1.Size = New System.Drawing.Size(6, 25)
        '
        'm_tsbShowFlow
        '
        Me.m_tsbShowFlow.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.m_tsbShowFlow.Image = CType(resources.GetObject("m_tsbShowFlow.Image"), System.Drawing.Image)
        Me.m_tsbShowFlow.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.m_tsbShowFlow.Name = "m_tsbShowFlow"
        Me.m_tsbShowFlow.Size = New System.Drawing.Size(60, 22)
        Me.m_tsbShowFlow.Text = "&Show flow"
        '
        'm_tssep2
        '
        Me.m_tssep2.Name = "m_tssep2"
        Me.m_tssep2.Size = New System.Drawing.Size(6, 25)
        '
        'm_tsbEcopath
        '
        Me.m_tsbEcopath.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.m_tsbEcopath.Image = CType(resources.GetObject("m_tsbEcopath.Image"), System.Drawing.Image)
        Me.m_tsbEcopath.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.m_tsbEcopath.Name = "m_tsbEcopath"
        Me.m_tsbEcopath.Size = New System.Drawing.Size(23, 22)
        Me.m_tsbEcopath.Text = "Table"
        Me.m_tsbEcopath.ToolTipText = "Show Ecopath results table"
        '
        'm_tsbEcosim
        '
        Me.m_tsbEcosim.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.m_tsbEcosim.Image = CType(resources.GetObject("m_tsbEcosim.Image"), System.Drawing.Image)
        Me.m_tsbEcosim.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.m_tsbEcosim.Name = "m_tsbEcosim"
        Me.m_tsbEcosim.Size = New System.Drawing.Size(23, 22)
        Me.m_tsbEcosim.Text = "Graph"
        Me.m_tsbEcosim.ToolTipText = "Show Ecosim results graph"
        '
        'm_tsbEquilibrium
        '
        Me.m_tsbEquilibrium.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.m_tsbEquilibrium.Image = Global.EwEValueChainPlugin.My.Resources.Resources.eqgraphhs
        Me.m_tsbEquilibrium.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.m_tsbEquilibrium.Name = "m_tsbEquilibrium"
        Me.m_tsbEquilibrium.Size = New System.Drawing.Size(23, 22)
        Me.m_tsbEquilibrium.Text = "Equilibrium"
        Me.m_tsbEquilibrium.ToolTipText = "Show equilibrium results graph"
        '
        'm_btnRunEcosim
        '
        Me.m_btnRunEcosim.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_btnRunEcosim.FlatStyle = System.Windows.Forms.FlatStyle.Popup
        Me.m_btnRunEcosim.Location = New System.Drawing.Point(543, 529)
        Me.m_btnRunEcosim.Margin = New System.Windows.Forms.Padding(0)
        Me.m_btnRunEcosim.Name = "m_btnRunEcosim"
        Me.m_btnRunEcosim.Size = New System.Drawing.Size(100, 23)
        Me.m_btnRunEcosim.TabIndex = 0
        Me.m_btnRunEcosim.Text = "Run &Ecosim"
        Me.m_btnRunEcosim.UseVisualStyleBackColor = True
        '
        'm_scResults
        '
        Me.m_scResults.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_scResults.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.m_scResults.Location = New System.Drawing.Point(0, 25)
        Me.m_scResults.Margin = New System.Windows.Forms.Padding(0)
        Me.m_scResults.Name = "m_scResults"
        Me.m_scResults.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'm_scResults.Panel1
        '
        Me.m_scResults.Panel1.Controls.Add(Me.m_plFlow)
        Me.m_scResults.Size = New System.Drawing.Size(751, 500)
        Me.m_scResults.SplitterDistance = 74
        Me.m_scResults.TabIndex = 3
        '
        'm_plFlow
        '
        Me.m_plFlow.AutoScroll = True
        Me.m_plFlow.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_plFlow.EditMode = EwEValueChainPlugin.plFlow.eEditMode.[ReadOnly]
        Me.m_plFlow.FleetFilter = Nothing
        Me.m_plFlow.Location = New System.Drawing.Point(0, 0)
        Me.m_plFlow.Margin = New System.Windows.Forms.Padding(0)
        Me.m_plFlow.Name = "m_plFlow"
        Me.m_plFlow.ShowGrid = False
        Me.m_plFlow.Size = New System.Drawing.Size(747, 70)
        Me.m_plFlow.TabIndex = 0
        Me.m_plFlow.ZoomFactor = 1.0!
        '
        'm_btnRunEquilibrium
        '
        Me.m_btnRunEquilibrium.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_btnRunEquilibrium.FlatStyle = System.Windows.Forms.FlatStyle.Popup
        Me.m_btnRunEquilibrium.Location = New System.Drawing.Point(649, 529)
        Me.m_btnRunEquilibrium.Margin = New System.Windows.Forms.Padding(0)
        Me.m_btnRunEquilibrium.Name = "m_btnRunEquilibrium"
        Me.m_btnRunEquilibrium.Size = New System.Drawing.Size(100, 23)
        Me.m_btnRunEquilibrium.TabIndex = 0
        Me.m_btnRunEquilibrium.Text = "Run E&quilibrium"
        Me.m_btnRunEquilibrium.UseVisualStyleBackColor = True
        '
        'm_tscmbGraphData
        '
        Me.m_tscmbGraphData.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.m_tscmbGraphData.Name = "m_tscmbGraphData"
        Me.m_tscmbGraphData.Size = New System.Drawing.Size(121, 25)
        '
        'm_tslblData
        '
        Me.m_tslblData.Name = "m_tslblData"
        Me.m_tslblData.Size = New System.Drawing.Size(34, 22)
        Me.m_tslblData.Text = "&Data:"
        '
        'm_tssSep3
        '
        Me.m_tssSep3.Name = "m_tssSep3"
        Me.m_tssSep3.Size = New System.Drawing.Size(6, 25)
        '
        'ucResults
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.m_scResults)
        Me.Controls.Add(Me.m_btnRunEquilibrium)
        Me.Controls.Add(Me.m_btnRunEcosim)
        Me.Controls.Add(Me.m_btnRunEcopath)
        Me.Controls.Add(Me.m_tsResults)
        Me.Name = "ucResults"
        Me.Size = New System.Drawing.Size(751, 552)
        Me.m_tsResults.ResumeLayout(False)
        Me.m_tsResults.PerformLayout()
        Me.m_scResults.Panel1.ResumeLayout(False)
        Me.m_scResults.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Private WithEvents m_btnRunEcopath As System.Windows.Forms.Button
    Private WithEvents m_tsResults As System.Windows.Forms.ToolStrip
    Private WithEvents m_tslFleets As System.Windows.Forms.ToolStripLabel
    Private WithEvents m_tscmbFleets As System.Windows.Forms.ToolStripComboBox
    Private WithEvents m_btnRunEcosim As System.Windows.Forms.Button
    Private WithEvents m_scResults As System.Windows.Forms.SplitContainer
    Private WithEvents m_tssep1 As System.Windows.Forms.ToolStripSeparator
    Private WithEvents m_tsbShowFlow As System.Windows.Forms.ToolStripButton
    Private WithEvents m_plFlow As plFlow
    Private WithEvents m_tssep2 As System.Windows.Forms.ToolStripSeparator
    Private WithEvents m_tsbEcopath As System.Windows.Forms.ToolStripButton
    Private WithEvents m_btnRunEquilibrium As System.Windows.Forms.Button
    Private WithEvents m_tsbEcosim As System.Windows.Forms.ToolStripButton
    Private WithEvents m_tsbEquilibrium As System.Windows.Forms.ToolStripButton
    Private WithEvents m_tscmbGraphData As System.Windows.Forms.ToolStripComboBox
    Private WithEvents m_tslblData As System.Windows.Forms.ToolStripLabel
    Private WithEvents m_tssSep3 As System.Windows.Forms.ToolStripSeparator

End Class
