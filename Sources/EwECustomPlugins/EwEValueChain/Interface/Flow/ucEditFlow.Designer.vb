Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports ScientificInterfaceShared.Controls

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ucEditFlow
    Inherits System.Windows.Forms.UserControl

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ucEditFlow))
        Me.m_tsMain = New ScientificInterfaceShared.Controls.cEwEToolstrip()
        Me.m_tsbSave = New System.Windows.Forms.ToolStripSplitButton()
        Me.m_tsmiSave = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_tsmiExportToImage = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator3 = New System.Windows.Forms.ToolStripSeparator()
        Me.m_tsbAdd = New System.Windows.Forms.ToolStripDropDownButton()
        Me.m_tsbCreateProducersForFleets = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator4 = New System.Windows.Forms.ToolStripSeparator()
        Me.m_tsbCreateProducer = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_tsbCreateProcessing = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_tsbCreateDistribution = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_tsbCreateWholesaler = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_tsbCreateRetailer = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_tsbCreateConsumer = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.m_tsbMove = New System.Windows.Forms.ToolStripButton()
        Me.m_tsbLink = New System.Windows.Forms.ToolStripButton()
        Me.m_tsbDelete = New System.Windows.Forms.ToolStripButton()
        Me.m_tsSeparator = New System.Windows.Forms.ToolStripSeparator()
        Me.m_tsbShowGrid = New System.Windows.Forms.ToolStripButton()
        Me.m_tsbArrange = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSeparator5 = New System.Windows.Forms.ToolStripSeparator()
        Me.m_tsddZoom = New System.Windows.Forms.ToolStripDropDownButton()
        Me.m_tsmiZoom50 = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_tsmiZoom75 = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_tsmiZoom100 = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_tsmiZoom125 = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_tsmiZoom150 = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_tsmiZoom200 = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_split = New System.Windows.Forms.SplitContainer()
        Me.m_plFlow = New EwEValueChainPlugin.plFlow()
        Me.m_tlpDetails = New System.Windows.Forms.TableLayoutPanel()
        Me.m_pgDetails = New System.Windows.Forms.PropertyGrid()
        Me.m_selector = New EwEValueChainPlugin.ucSelector2()
        Me.m_hdrDetails = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.m_tsMain.SuspendLayout()
        CType(Me.m_split, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.m_split.Panel1.SuspendLayout()
        Me.m_split.Panel2.SuspendLayout()
        Me.m_split.SuspendLayout()
        Me.m_tlpDetails.SuspendLayout()
        Me.SuspendLayout()
        '
        'm_tsMain
        '
        Me.m_tsMain.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
        Me.m_tsMain.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tsbSave, Me.ToolStripSeparator3, Me.m_tsbAdd, Me.ToolStripSeparator1, Me.m_tsbMove, Me.m_tsbLink, Me.m_tsbDelete, Me.m_tsSeparator, Me.m_tsbShowGrid, Me.m_tsbArrange, Me.ToolStripSeparator5, Me.m_tsddZoom})
        Me.m_tsMain.Location = New System.Drawing.Point(0, 0)
        Me.m_tsMain.Name = "m_tsMain"
        Me.m_tsMain.RenderMode = System.Windows.Forms.ToolStripRenderMode.System
        Me.m_tsMain.Size = New System.Drawing.Size(756, 38)
        Me.m_tsMain.TabIndex = 1
        '
        'm_tsbSave
        '
        Me.m_tsbSave.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tsmiSave, Me.m_tsmiExportToImage})
        Me.m_tsbSave.Image = CType(resources.GetObject("m_tsbSave.Image"), System.Drawing.Image)
        Me.m_tsbSave.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.m_tsbSave.Name = "m_tsbSave"
        Me.m_tsbSave.Size = New System.Drawing.Size(47, 35)
        Me.m_tsbSave.Text = "&Save"
        Me.m_tsbSave.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText
        Me.m_tsbSave.Visible = False
        '
        'm_tsmiSave
        '
        Me.m_tsmiSave.Image = CType(resources.GetObject("m_tsmiSave.Image"), System.Drawing.Image)
        Me.m_tsmiSave.Name = "m_tsmiSave"
        Me.m_tsmiSave.Size = New System.Drawing.Size(166, 22)
        Me.m_tsmiSave.Text = "Save to database"
        '
        'm_tsmiExportToImage
        '
        Me.m_tsmiExportToImage.Image = CType(resources.GetObject("m_tsmiExportToImage.Image"), System.Drawing.Image)
        Me.m_tsmiExportToImage.Name = "m_tsmiExportToImage"
        Me.m_tsmiExportToImage.Size = New System.Drawing.Size(166, 22)
        Me.m_tsmiExportToImage.Text = "E&xport to image..."
        '
        'ToolStripSeparator3
        '
        Me.ToolStripSeparator3.Name = "ToolStripSeparator3"
        Me.ToolStripSeparator3.Size = New System.Drawing.Size(6, 38)
        '
        'm_tsbAdd
        '
        Me.m_tsbAdd.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tsbCreateProducersForFleets, Me.ToolStripSeparator4, Me.m_tsbCreateProducer, Me.m_tsbCreateProcessing, Me.m_tsbCreateDistribution, Me.m_tsbCreateWholesaler, Me.m_tsbCreateRetailer, Me.m_tsbCreateConsumer})
        Me.m_tsbAdd.Image = CType(resources.GetObject("m_tsbAdd.Image"), System.Drawing.Image)
        Me.m_tsbAdd.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.m_tsbAdd.Name = "m_tsbAdd"
        Me.m_tsbAdd.Size = New System.Drawing.Size(42, 35)
        Me.m_tsbAdd.Text = "&Add"
        Me.m_tsbAdd.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText
        Me.m_tsbAdd.ToolTipText = "Add or create units"
        '
        'm_tsbCreateProducersForFleets
        '
        Me.m_tsbCreateProducersForFleets.Name = "m_tsbCreateProducersForFleets"
        Me.m_tsbCreateProducersForFleets.Size = New System.Drawing.Size(234, 22)
        Me.m_tsbCreateProducersForFleets.Text = "Create producers for &Fleets"
        Me.m_tsbCreateProducersForFleets.ToolTipText = "Create a producer unit for every fleet defined in Ecopath"
        '
        'ToolStripSeparator4
        '
        Me.ToolStripSeparator4.Name = "ToolStripSeparator4"
        Me.ToolStripSeparator4.Size = New System.Drawing.Size(231, 6)
        '
        'm_tsbCreateProducer
        '
        Me.m_tsbCreateProducer.Name = "m_tsbCreateProducer"
        Me.m_tsbCreateProducer.ShortcutKeys = CType(((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.Shift) _
            Or System.Windows.Forms.Keys.D1), System.Windows.Forms.Keys)
        Me.m_tsbCreateProducer.Size = New System.Drawing.Size(234, 22)
        Me.m_tsbCreateProducer.Text = "New &producer"
        '
        'm_tsbCreateProcessing
        '
        Me.m_tsbCreateProcessing.Name = "m_tsbCreateProcessing"
        Me.m_tsbCreateProcessing.ShortcutKeys = CType(((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.Shift) _
            Or System.Windows.Forms.Keys.D2), System.Windows.Forms.Keys)
        Me.m_tsbCreateProcessing.Size = New System.Drawing.Size(234, 22)
        Me.m_tsbCreateProcessing.Text = "New pro&cessing"
        '
        'm_tsbCreateDistribution
        '
        Me.m_tsbCreateDistribution.Name = "m_tsbCreateDistribution"
        Me.m_tsbCreateDistribution.ShortcutKeys = CType(((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.Shift) _
            Or System.Windows.Forms.Keys.D3), System.Windows.Forms.Keys)
        Me.m_tsbCreateDistribution.Size = New System.Drawing.Size(234, 22)
        Me.m_tsbCreateDistribution.Text = "New d&istribution"
        '
        'm_tsbCreateWholesaler
        '
        Me.m_tsbCreateWholesaler.Name = "m_tsbCreateWholesaler"
        Me.m_tsbCreateWholesaler.ShortcutKeys = CType(((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.Shift) _
            Or System.Windows.Forms.Keys.D4), System.Windows.Forms.Keys)
        Me.m_tsbCreateWholesaler.Size = New System.Drawing.Size(234, 22)
        Me.m_tsbCreateWholesaler.Text = "New &wholesaler"
        '
        'm_tsbCreateRetailer
        '
        Me.m_tsbCreateRetailer.Name = "m_tsbCreateRetailer"
        Me.m_tsbCreateRetailer.ShortcutKeys = CType(((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.Shift) _
            Or System.Windows.Forms.Keys.D5), System.Windows.Forms.Keys)
        Me.m_tsbCreateRetailer.Size = New System.Drawing.Size(234, 22)
        Me.m_tsbCreateRetailer.Text = "New &retailer"
        '
        'm_tsbCreateConsumer
        '
        Me.m_tsbCreateConsumer.Name = "m_tsbCreateConsumer"
        Me.m_tsbCreateConsumer.ShortcutKeys = CType(((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.Shift) _
            Or System.Windows.Forms.Keys.D6), System.Windows.Forms.Keys)
        Me.m_tsbCreateConsumer.Size = New System.Drawing.Size(234, 22)
        Me.m_tsbCreateConsumer.Text = "New &consumer"
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        Me.ToolStripSeparator1.Size = New System.Drawing.Size(6, 38)
        '
        'm_tsbMove
        '
        Me.m_tsbMove.Checked = True
        Me.m_tsbMove.CheckState = System.Windows.Forms.CheckState.Checked
        Me.m_tsbMove.Image = CType(resources.GetObject("m_tsbMove.Image"), System.Drawing.Image)
        Me.m_tsbMove.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.m_tsbMove.Name = "m_tsbMove"
        Me.m_tsbMove.Size = New System.Drawing.Size(41, 35)
        Me.m_tsbMove.Text = "&Move"
        Me.m_tsbMove.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText
        Me.m_tsbMove.ToolTipText = "Reposition units in this diagram"
        '
        'm_tsbLink
        '
        Me.m_tsbLink.Image = CType(resources.GetObject("m_tsbLink.Image"), System.Drawing.Image)
        Me.m_tsbLink.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.m_tsbLink.Name = "m_tsbLink"
        Me.m_tsbLink.Size = New System.Drawing.Size(33, 35)
        Me.m_tsbLink.Text = "&Link"
        Me.m_tsbLink.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText
        Me.m_tsbLink.ToolTipText = "Create links between units in this diagram"
        '
        'm_tsbDelete
        '
        Me.m_tsbDelete.Image = CType(resources.GetObject("m_tsbDelete.Image"), System.Drawing.Image)
        Me.m_tsbDelete.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.m_tsbDelete.Name = "m_tsbDelete"
        Me.m_tsbDelete.Size = New System.Drawing.Size(44, 35)
        Me.m_tsbDelete.Text = "&Delete"
        Me.m_tsbDelete.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText
        Me.m_tsbDelete.ToolTipText = "Delete links and units"
        '
        'm_tsSeparator
        '
        Me.m_tsSeparator.Name = "m_tsSeparator"
        Me.m_tsSeparator.Size = New System.Drawing.Size(6, 38)
        '
        'm_tsbShowGrid
        '
        Me.m_tsbShowGrid.Image = CType(resources.GetObject("m_tsbShowGrid.Image"), System.Drawing.Image)
        Me.m_tsbShowGrid.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.m_tsbShowGrid.Name = "m_tsbShowGrid"
        Me.m_tsbShowGrid.Size = New System.Drawing.Size(64, 35)
        Me.m_tsbShowGrid.Text = "Show grid"
        Me.m_tsbShowGrid.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText
        '
        'm_tsbArrange
        '
        Me.m_tsbArrange.Image = CType(resources.GetObject("m_tsbArrange.Image"), System.Drawing.Image)
        Me.m_tsbArrange.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.m_tsbArrange.Name = "m_tsbArrange"
        Me.m_tsbArrange.Size = New System.Drawing.Size(53, 35)
        Me.m_tsbArrange.Text = "A&rrange"
        Me.m_tsbArrange.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText
        Me.m_tsbArrange.ToolTipText = "Arrange units"
        '
        'ToolStripSeparator5
        '
        Me.ToolStripSeparator5.Name = "ToolStripSeparator5"
        Me.ToolStripSeparator5.Size = New System.Drawing.Size(6, 38)
        '
        'm_tsddZoom
        '
        Me.m_tsddZoom.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tsmiZoom50, Me.m_tsmiZoom75, Me.m_tsmiZoom100, Me.m_tsmiZoom125, Me.m_tsmiZoom150, Me.m_tsmiZoom200})
        Me.m_tsddZoom.Image = CType(resources.GetObject("m_tsddZoom.Image"), System.Drawing.Image)
        Me.m_tsddZoom.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.m_tsddZoom.Name = "m_tsddZoom"
        Me.m_tsddZoom.Size = New System.Drawing.Size(52, 35)
        Me.m_tsddZoom.Text = "Zoom"
        Me.m_tsddZoom.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText
        '
        'm_tsmiZoom50
        '
        Me.m_tsmiZoom50.Name = "m_tsmiZoom50"
        Me.m_tsmiZoom50.Size = New System.Drawing.Size(102, 22)
        Me.m_tsmiZoom50.Text = "50%"
        '
        'm_tsmiZoom75
        '
        Me.m_tsmiZoom75.Name = "m_tsmiZoom75"
        Me.m_tsmiZoom75.Size = New System.Drawing.Size(102, 22)
        Me.m_tsmiZoom75.Text = "75%"
        '
        'm_tsmiZoom100
        '
        Me.m_tsmiZoom100.Name = "m_tsmiZoom100"
        Me.m_tsmiZoom100.Size = New System.Drawing.Size(102, 22)
        Me.m_tsmiZoom100.Text = "100%"
        '
        'm_tsmiZoom125
        '
        Me.m_tsmiZoom125.Name = "m_tsmiZoom125"
        Me.m_tsmiZoom125.Size = New System.Drawing.Size(102, 22)
        Me.m_tsmiZoom125.Text = "125%"
        '
        'm_tsmiZoom150
        '
        Me.m_tsmiZoom150.Name = "m_tsmiZoom150"
        Me.m_tsmiZoom150.Size = New System.Drawing.Size(102, 22)
        Me.m_tsmiZoom150.Text = "150%"
        '
        'm_tsmiZoom200
        '
        Me.m_tsmiZoom200.Name = "m_tsmiZoom200"
        Me.m_tsmiZoom200.Size = New System.Drawing.Size(102, 22)
        Me.m_tsmiZoom200.Text = "200%"
        '
        'm_split
        '
        Me.m_split.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_split.Location = New System.Drawing.Point(0, 38)
        Me.m_split.Name = "m_split"
        '
        'm_split.Panel1
        '
        Me.m_split.Panel1.Controls.Add(Me.m_plFlow)
        '
        'm_split.Panel2
        '
        Me.m_split.Panel2.Controls.Add(Me.m_tlpDetails)
        Me.m_split.Size = New System.Drawing.Size(756, 425)
        Me.m_split.SplitterDistance = 571
        Me.m_split.TabIndex = 2
        '
        'm_plFlow
        '
        Me.m_plFlow.AutoScroll = True
        Me.m_plFlow.BackColor = System.Drawing.SystemColors.Window
        Me.m_plFlow.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.m_plFlow.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_plFlow.EditMode = EwEValueChainPlugin.plFlow.eEditMode.Move
        Me.m_plFlow.FleetFilter = Nothing
        Me.m_plFlow.Location = New System.Drawing.Point(0, 0)
        Me.m_plFlow.Name = "m_plFlow"
        Me.m_plFlow.ShowGrid = False
        Me.m_plFlow.Size = New System.Drawing.Size(571, 425)
        Me.m_plFlow.TabIndex = 0
        '
        'm_tlpDetails
        '
        Me.m_tlpDetails.ColumnCount = 1
        Me.m_tlpDetails.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.m_tlpDetails.Controls.Add(Me.m_pgDetails, 0, 2)
        Me.m_tlpDetails.Controls.Add(Me.m_selector, 0, 0)
        Me.m_tlpDetails.Controls.Add(Me.m_hdrDetails, 0, 1)
        Me.m_tlpDetails.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_tlpDetails.Location = New System.Drawing.Point(0, 0)
        Me.m_tlpDetails.Margin = New System.Windows.Forms.Padding(0)
        Me.m_tlpDetails.Name = "m_tlpDetails"
        Me.m_tlpDetails.RowCount = 3
        Me.m_tlpDetails.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.m_tlpDetails.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.m_tlpDetails.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.m_tlpDetails.Size = New System.Drawing.Size(181, 425)
        Me.m_tlpDetails.TabIndex = 0
        '
        'm_pgDetails
        '
        Me.m_pgDetails.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_pgDetails.Location = New System.Drawing.Point(3, 143)
        Me.m_pgDetails.Name = "m_pgDetails"
        Me.m_pgDetails.Size = New System.Drawing.Size(175, 279)
        Me.m_pgDetails.TabIndex = 0
        '
        'm_selector
        '
        Me.m_selector.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_selector.Location = New System.Drawing.Point(0, 0)
        Me.m_selector.Margin = New System.Windows.Forms.Padding(0)
        Me.m_selector.Name = "m_selector"
        Me.m_selector.Selection = New EwEUtils.Database.cEwEDatabase.cOOPStorable(-1) {}
        Me.m_selector.Size = New System.Drawing.Size(181, 120)
        Me.m_selector.TabIndex = 1
        '
        'm_hdrDetails
        '
        Me.m_hdrDetails.CanCollapseParent = False
        Me.m_hdrDetails.CollapsedParentHeight = 0
        Me.m_hdrDetails.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_hdrDetails.IsCollapsed = False
        Me.m_hdrDetails.Location = New System.Drawing.Point(3, 120)
        Me.m_hdrDetails.Name = "m_hdrDetails"
        Me.m_hdrDetails.Size = New System.Drawing.Size(175, 20)
        Me.m_hdrDetails.TabIndex = 2
        Me.m_hdrDetails.Text = "Selection details"
        Me.m_hdrDetails.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'ucEditFlow
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.m_split)
        Me.Controls.Add(Me.m_tsMain)
        Me.DoubleBuffered = True
        Me.Name = "ucEditFlow"
        Me.Size = New System.Drawing.Size(756, 463)
        Me.m_tsMain.ResumeLayout(False)
        Me.m_tsMain.PerformLayout()
        Me.m_split.Panel1.ResumeLayout(False)
        Me.m_split.Panel2.ResumeLayout(False)
        CType(Me.m_split, System.ComponentModel.ISupportInitialize).EndInit()
        Me.m_split.ResumeLayout(False)
        Me.m_tlpDetails.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Private WithEvents m_tsMain As cEwEToolstrip
    Private WithEvents m_tsbCreateProducersForFleets As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents m_tsbCreateProducer As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents m_tsbCreateProcessing As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents m_tsbCreateDistribution As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents m_tsbCreateWholesaler As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents m_tsbCreateRetailer As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents m_tsbCreateConsumer As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
    Private WithEvents m_split As System.Windows.Forms.SplitContainer
    Private WithEvents m_plFlow As plFlow
    Private WithEvents m_pgDetails As System.Windows.Forms.PropertyGrid
    Private WithEvents ToolStripSeparator4 As System.Windows.Forms.ToolStripSeparator
    Private WithEvents m_tsbMove As System.Windows.Forms.ToolStripButton
    Private WithEvents m_tsbLink As System.Windows.Forms.ToolStripButton
    Private WithEvents m_tsbAdd As System.Windows.Forms.ToolStripDropDownButton
    Private WithEvents m_tsbDelete As System.Windows.Forms.ToolStripButton
    Private WithEvents m_tsSeparator As System.Windows.Forms.ToolStripSeparator
    Private WithEvents m_tsbArrange As System.Windows.Forms.ToolStripButton
    Private WithEvents m_tsbShowGrid As System.Windows.Forms.ToolStripButton
    Private WithEvents ToolStripSeparator3 As System.Windows.Forms.ToolStripSeparator
    Private WithEvents ToolStripSeparator5 As System.Windows.Forms.ToolStripSeparator
    Private WithEvents m_tsddZoom As System.Windows.Forms.ToolStripDropDownButton
    Private WithEvents m_tsmiZoom50 As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents m_tsmiZoom75 As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents m_tsmiZoom100 As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents m_tsmiZoom125 As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents m_tsmiZoom150 As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents m_tsmiZoom200 As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents m_tsbSave As System.Windows.Forms.ToolStripSplitButton
    Private WithEvents m_tsmiExportToImage As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents m_tsmiSave As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents m_tlpDetails As System.Windows.Forms.TableLayoutPanel
    Private WithEvents m_selector As EwEValueChainPlugin.ucSelector2
    Private WithEvents m_hdrDetails As ScientificInterfaceShared.Controls.cEwEHeaderLabel

End Class
