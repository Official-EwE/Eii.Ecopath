Namespace Ecosim

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class ucBiomassPlotzgc
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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ucBiomassPlotzgc))
            Me.gbAnonymous = New System.Windows.Forms.GroupBox
            Me.SplitContainer1 = New System.Windows.Forms.SplitContainer
            Me.lbOverlay = New System.Windows.Forms.ListBox
            Me.m_zgc = New ZedGraph.ZedGraphControl
            Me.m_ts = New System.Windows.Forms.ToolStrip
            Me.tslblSSValue = New System.Windows.Forms.ToolStripLabel
            Me.tsblbSS = New System.Windows.Forms.ToolStripLabel
            Me.ToolStripDropDownButton2 = New System.Windows.Forms.ToolStripDropDownButton
            Me.CulmulativeToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
            Me.RelativeToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
            Me.ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator
            Me.ToolStripDropDownButton1 = New System.Windows.Forms.ToolStripDropDownButton
            Me.AnnualOutputToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
            Me.OverlayToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
            Me.ShowLegendToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
            Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator
            Me.m_tsbAutoscale = New System.Windows.Forms.ToolStripButton
            Me.m_tsbCustomScale = New System.Windows.Forms.ToolStripButton
            Me.m_tlsMax = New System.Windows.Forms.ToolStripLabel
            Me.m_tstbScaleMax = New System.Windows.Forms.ToolStripTextBox
            Me.m_tslMin = New System.Windows.Forms.ToolStripLabel
            Me.m_tstbScaleMin = New System.Windows.Forms.ToolStripTextBox
            Me.lbGroups = New ScientificInterfaceShared.Controls.LegendListBox
            Me.gbAnonymous.SuspendLayout()
            Me.SplitContainer1.Panel1.SuspendLayout()
            Me.SplitContainer1.Panel2.SuspendLayout()
            Me.SplitContainer1.SuspendLayout()
            Me.m_ts.SuspendLayout()
            Me.SuspendLayout()
            '
            'gbAnonymous
            '
            Me.gbAnonymous.Controls.Add(Me.SplitContainer1)
            Me.gbAnonymous.Controls.Add(Me.m_zgc)
            Me.gbAnonymous.Controls.Add(Me.m_ts)
            Me.gbAnonymous.Dock = System.Windows.Forms.DockStyle.Fill
            Me.gbAnonymous.Location = New System.Drawing.Point(0, 0)
            Me.gbAnonymous.Name = "gbAnonymous"
            Me.gbAnonymous.Size = New System.Drawing.Size(860, 460)
            Me.gbAnonymous.TabIndex = 0
            Me.gbAnonymous.TabStop = False
            Me.gbAnonymous.Text = "Ecosim biomass output"
            '
            'SplitContainer1
            '
            Me.SplitContainer1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.SplitContainer1.Location = New System.Drawing.Point(740, 44)
            Me.SplitContainer1.Margin = New System.Windows.Forms.Padding(0)
            Me.SplitContainer1.Name = "SplitContainer1"
            Me.SplitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal
            '
            'SplitContainer1.Panel1
            '
            Me.SplitContainer1.Panel1.Controls.Add(Me.lbOverlay)
            '
            'SplitContainer1.Panel2
            '
            Me.SplitContainer1.Panel2.Controls.Add(Me.lbGroups)
            Me.SplitContainer1.Size = New System.Drawing.Size(119, 410)
            Me.SplitContainer1.SplitterDistance = 141
            Me.SplitContainer1.TabIndex = 12
            '
            'lbOverlay
            '
            Me.lbOverlay.Dock = System.Windows.Forms.DockStyle.Fill
            Me.lbOverlay.FormattingEnabled = True
            Me.lbOverlay.IntegralHeight = False
            Me.lbOverlay.Location = New System.Drawing.Point(0, 0)
            Me.lbOverlay.Name = "lbOverlay"
            Me.lbOverlay.Size = New System.Drawing.Size(119, 141)
            Me.lbOverlay.TabIndex = 0
            '
            'm_zgc
            '
            Me.m_zgc.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_zgc.Location = New System.Drawing.Point(3, 44)
            Me.m_zgc.Name = "m_zgc"
            Me.m_zgc.ScrollGrace = 0
            Me.m_zgc.ScrollMaxX = 0
            Me.m_zgc.ScrollMaxY = 0
            Me.m_zgc.ScrollMaxY2 = 0
            Me.m_zgc.ScrollMinX = 0
            Me.m_zgc.ScrollMinY = 0
            Me.m_zgc.ScrollMinY2 = 0
            Me.m_zgc.Size = New System.Drawing.Size(731, 411)
            Me.m_zgc.TabIndex = 11
            '
            'm_ts
            '
            Me.m_ts.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tslblSSValue, Me.tsblbSS, Me.ToolStripDropDownButton2, Me.ToolStripSeparator2, Me.ToolStripDropDownButton1, Me.ToolStripSeparator1, Me.m_tsbAutoscale, Me.m_tsbCustomScale, Me.m_tlsMax, Me.m_tstbScaleMax, Me.m_tslMin, Me.m_tstbScaleMin})
            Me.m_ts.Location = New System.Drawing.Point(3, 16)
            Me.m_ts.Name = "m_ts"
            Me.m_ts.Size = New System.Drawing.Size(854, 25)
            Me.m_ts.TabIndex = 10
            Me.m_ts.Text = "ToolStrip1"
            '
            'tslblSSValue
            '
            Me.tslblSSValue.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
            Me.tslblSSValue.Name = "tslblSSValue"
            Me.tslblSSValue.Size = New System.Drawing.Size(0, 22)
            '
            'tsblbSS
            '
            Me.tsblbSS.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
            Me.tsblbSS.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            Me.tsblbSS.Name = "tsblbSS"
            Me.tsblbSS.Size = New System.Drawing.Size(86, 22)
            Me.tsblbSS.Text = "Sum of Squares:"
            '
            'ToolStripDropDownButton2
            '
            Me.ToolStripDropDownButton2.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.CulmulativeToolStripMenuItem, Me.RelativeToolStripMenuItem})
            Me.ToolStripDropDownButton2.Image = Global.ScientificInterface.My.Resources.Resources.OptionsHS
            Me.ToolStripDropDownButton2.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.ToolStripDropDownButton2.Name = "ToolStripDropDownButton2"
            Me.ToolStripDropDownButton2.Size = New System.Drawing.Size(79, 22)
            Me.ToolStripDropDownButton2.Text = "&Plot type"
            '
            'CulmulativeToolStripMenuItem
            '
            Me.CulmulativeToolStripMenuItem.Checked = True
            Me.CulmulativeToolStripMenuItem.CheckOnClick = True
            Me.CulmulativeToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked
            Me.CulmulativeToolStripMenuItem.Name = "CulmulativeToolStripMenuItem"
            Me.CulmulativeToolStripMenuItem.Size = New System.Drawing.Size(140, 22)
            Me.CulmulativeToolStripMenuItem.Text = "Culmulative"
            '
            'RelativeToolStripMenuItem
            '
            Me.RelativeToolStripMenuItem.CheckOnClick = True
            Me.RelativeToolStripMenuItem.Name = "RelativeToolStripMenuItem"
            Me.RelativeToolStripMenuItem.Size = New System.Drawing.Size(140, 22)
            Me.RelativeToolStripMenuItem.Text = "Relative"
            '
            'ToolStripSeparator2
            '
            Me.ToolStripSeparator2.Name = "ToolStripSeparator2"
            Me.ToolStripSeparator2.Size = New System.Drawing.Size(6, 25)
            '
            'ToolStripDropDownButton1
            '
            Me.ToolStripDropDownButton1.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.AnnualOutputToolStripMenuItem, Me.OverlayToolStripMenuItem, Me.ShowLegendToolStripMenuItem})
            Me.ToolStripDropDownButton1.Image = Global.ScientificInterface.My.Resources.Resources.OptionsHS
            Me.ToolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.ToolStripDropDownButton1.Name = "ToolStripDropDownButton1"
            Me.ToolStripDropDownButton1.Size = New System.Drawing.Size(103, 22)
            Me.ToolStripDropDownButton1.Text = "Graph &options"
            '
            'AnnualOutputToolStripMenuItem
            '
            Me.AnnualOutputToolStripMenuItem.Name = "AnnualOutputToolStripMenuItem"
            Me.AnnualOutputToolStripMenuItem.Size = New System.Drawing.Size(153, 22)
            Me.AnnualOutputToolStripMenuItem.Text = "&Annual output"
            '
            'OverlayToolStripMenuItem
            '
            Me.OverlayToolStripMenuItem.Name = "OverlayToolStripMenuItem"
            Me.OverlayToolStripMenuItem.Size = New System.Drawing.Size(153, 22)
            Me.OverlayToolStripMenuItem.Text = "O&verlay"
            '
            'ShowLegendToolStripMenuItem
            '
            Me.ShowLegendToolStripMenuItem.Name = "ShowLegendToolStripMenuItem"
            Me.ShowLegendToolStripMenuItem.Size = New System.Drawing.Size(153, 22)
            Me.ShowLegendToolStripMenuItem.Text = "Show &legend"
            '
            'ToolStripSeparator1
            '
            Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
            Me.ToolStripSeparator1.Size = New System.Drawing.Size(6, 25)
            '
            'm_tsbAutoscale
            '
            Me.m_tsbAutoscale.Checked = True
            Me.m_tsbAutoscale.CheckState = System.Windows.Forms.CheckState.Checked
            Me.m_tsbAutoscale.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            Me.m_tsbAutoscale.Image = CType(resources.GetObject("m_tsbAutoscale.Image"), System.Drawing.Image)
            Me.m_tsbAutoscale.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.m_tsbAutoscale.Name = "m_tsbAutoscale"
            Me.m_tsbAutoscale.Size = New System.Drawing.Size(62, 22)
            Me.m_tsbAutoscale.Text = "Auto-scale"
            '
            'm_tsbCustomScale
            '
            Me.m_tsbCustomScale.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            Me.m_tsbCustomScale.Image = CType(resources.GetObject("m_tsbCustomScale.Image"), System.Drawing.Image)
            Me.m_tsbCustomScale.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.m_tsbCustomScale.Name = "m_tsbCustomScale"
            Me.m_tsbCustomScale.Size = New System.Drawing.Size(74, 22)
            Me.m_tsbCustomScale.Text = "Custom scale"
            '
            'm_tlsMax
            '
            Me.m_tlsMax.Name = "m_tlsMax"
            Me.m_tlsMax.Size = New System.Drawing.Size(31, 22)
            Me.m_tlsMax.Text = "Max:"
            '
            'm_tstbScaleMax
            '
            Me.m_tstbScaleMax.Name = "m_tstbScaleMax"
            Me.m_tstbScaleMax.Size = New System.Drawing.Size(50, 25)
            '
            'm_tslMin
            '
            Me.m_tslMin.Name = "m_tslMin"
            Me.m_tslMin.Size = New System.Drawing.Size(27, 22)
            Me.m_tslMin.Text = "Min:"
            '
            'm_tstbScaleMin
            '
            Me.m_tstbScaleMin.Name = "m_tstbScaleMin"
            Me.m_tstbScaleMin.Size = New System.Drawing.Size(50, 25)
            '
            'lbGroups
            '
            Me.lbGroups.Dock = System.Windows.Forms.DockStyle.Fill
            Me.lbGroups.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed
            Me.lbGroups.FormattingEnabled = True
            Me.lbGroups.IntegralHeight = False
            Me.lbGroups.Location = New System.Drawing.Point(0, 0)
            Me.lbGroups.Name = "lbGroups"
            Me.lbGroups.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended
            Me.lbGroups.Size = New System.Drawing.Size(119, 265)
            Me.lbGroups.TabIndex = 1
            '
            'ucBiomassPlotzgc
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.gbAnonymous)
            Me.Name = "ucBiomassPlotzgc"
            Me.Size = New System.Drawing.Size(860, 460)
            Me.gbAnonymous.ResumeLayout(False)
            Me.gbAnonymous.PerformLayout()
            Me.SplitContainer1.Panel1.ResumeLayout(False)
            Me.SplitContainer1.Panel2.ResumeLayout(False)
            Me.SplitContainer1.ResumeLayout(False)
            Me.m_ts.ResumeLayout(False)
            Me.m_ts.PerformLayout()
            Me.ResumeLayout(False)

        End Sub

        Private WithEvents gbAnonymous As System.Windows.Forms.GroupBox
        Private WithEvents m_ts As System.Windows.Forms.ToolStrip
        Private WithEvents tsblbSS As System.Windows.Forms.ToolStripLabel
        Private WithEvents m_zgc As ZedGraph.ZedGraphControl
        Private WithEvents SplitContainer1 As System.Windows.Forms.SplitContainer
        Private WithEvents lbOverlay As System.Windows.Forms.ListBox
        Private WithEvents lbGroups As ScientificInterfaceShared.Controls.LegendListBox
        Private WithEvents tslblSSValue As System.Windows.Forms.ToolStripLabel
        Private WithEvents ToolStripDropDownButton1 As System.Windows.Forms.ToolStripDropDownButton
        Private WithEvents AnnualOutputToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
        Private WithEvents OverlayToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
        Private WithEvents ShowLegendToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
        Private WithEvents m_tsbAutoscale As System.Windows.Forms.ToolStripButton
        Private WithEvents m_tsbCustomScale As System.Windows.Forms.ToolStripButton
        Private WithEvents m_tstbScaleMin As System.Windows.Forms.ToolStripTextBox
        Friend WithEvents m_tslMin As System.Windows.Forms.ToolStripLabel
        Private WithEvents m_tlsMax As System.Windows.Forms.ToolStripLabel
        Friend WithEvents m_tstbScaleMax As System.Windows.Forms.ToolStripTextBox
        Friend WithEvents ToolStripDropDownButton2 As System.Windows.Forms.ToolStripDropDownButton
        Friend WithEvents CulmulativeToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents RelativeToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents ToolStripSeparator2 As System.Windows.Forms.ToolStripSeparator

    End Class

End Namespace