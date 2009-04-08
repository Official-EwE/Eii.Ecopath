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
            Dim ToolStripSeparator5 As System.Windows.Forms.ToolStripSeparator
            Dim ToolStripSeparator2 As System.Windows.Forms.ToolStripSeparator
            Dim ToolStripSeparator4 As System.Windows.Forms.ToolStripSeparator
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ucBiomassPlotzgc))
            Me.SplitContainer1 = New System.Windows.Forms.SplitContainer
            Me.m_lbRuns = New System.Windows.Forms.ListBox
            Me.m_lbGroups = New ScientificInterfaceShared.Controls.cGroupListBox
            Me.m_zgc = New ZedGraph.ZedGraphControl
            Me.m_ts = New System.Windows.Forms.ToolStrip
            Me.m_tsbtnShowHideGroups = New System.Windows.Forms.ToolStripButton
            Me.tslblSSValue = New System.Windows.Forms.ToolStripLabel
            Me.tsblbSS = New System.Windows.Forms.ToolStripLabel
            Me.m_tsdrpdnbtnBiomassCatch = New System.Windows.Forms.ToolStripDropDownButton
            Me.m_tsmiBiomass = New System.Windows.Forms.ToolStripMenuItem
            Me.m_tsmiCatch = New System.Windows.Forms.ToolStripMenuItem
            Me.m_tsddPlotType = New System.Windows.Forms.ToolStripDropDownButton
            Me.m_tsmiCumulative = New System.Windows.Forms.ToolStripMenuItem
            Me.m_tsmiRelative = New System.Windows.Forms.ToolStripMenuItem
            Me.m_tsddGraphOptions = New System.Windows.Forms.ToolStripDropDownButton
            Me.m_tsmiAutoscale = New System.Windows.Forms.ToolStripMenuItem
            Me.m_tsmiCustomScaleLabel = New System.Windows.Forms.ToolStripMenuItem
            Me.m_tsmiMax = New System.Windows.Forms.ToolStripMenuItem
            Me.m_tstbMax = New System.Windows.Forms.ToolStripTextBox
            Me.m_tsmiMin = New System.Windows.Forms.ToolStripMenuItem
            Me.m_tstbMin = New System.Windows.Forms.ToolStripTextBox
            Me.m_tsmiShowAnnualOutput = New System.Windows.Forms.ToolStripMenuItem
            Me.m_tsmShowMultipleRuns = New System.Windows.Forms.ToolStripMenuItem
            Me.m_tsmiShowLegend = New System.Windows.Forms.ToolStripMenuItem
            Me.m_tsbExplore = New System.Windows.Forms.ToolStripButton
            ToolStripSeparator5 = New System.Windows.Forms.ToolStripSeparator
            ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator
            ToolStripSeparator4 = New System.Windows.Forms.ToolStripSeparator
            Me.SplitContainer1.Panel1.SuspendLayout()
            Me.SplitContainer1.Panel2.SuspendLayout()
            Me.SplitContainer1.SuspendLayout()
            Me.m_ts.SuspendLayout()
            Me.SuspendLayout()
            '
            'ToolStripSeparator5
            '
            ToolStripSeparator5.Name = "ToolStripSeparator5"
            ToolStripSeparator5.Size = New System.Drawing.Size(6, 25)
            '
            'ToolStripSeparator2
            '
            ToolStripSeparator2.Name = "ToolStripSeparator2"
            ToolStripSeparator2.Size = New System.Drawing.Size(6, 25)
            '
            'ToolStripSeparator4
            '
            ToolStripSeparator4.Name = "ToolStripSeparator4"
            ToolStripSeparator4.Size = New System.Drawing.Size(171, 6)
            '
            'SplitContainer1
            '
            Me.SplitContainer1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.SplitContainer1.Location = New System.Drawing.Point(740, 25)
            Me.SplitContainer1.Margin = New System.Windows.Forms.Padding(0)
            Me.SplitContainer1.Name = "SplitContainer1"
            Me.SplitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal
            '
            'SplitContainer1.Panel1
            '
            Me.SplitContainer1.Panel1.Controls.Add(Me.m_lbRuns)
            '
            'SplitContainer1.Panel2
            '
            Me.SplitContainer1.Panel2.Controls.Add(Me.m_lbGroups)
            Me.SplitContainer1.Size = New System.Drawing.Size(119, 429)
            Me.SplitContainer1.SplitterDistance = 164
            Me.SplitContainer1.TabIndex = 12
            '
            'm_lbRuns
            '
            Me.m_lbRuns.Cursor = System.Windows.Forms.Cursors.Hand
            Me.m_lbRuns.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_lbRuns.FormattingEnabled = True
            Me.m_lbRuns.IntegralHeight = False
            Me.m_lbRuns.Location = New System.Drawing.Point(0, 0)
            Me.m_lbRuns.Name = "m_lbRuns"
            Me.m_lbRuns.Size = New System.Drawing.Size(119, 164)
            Me.m_lbRuns.TabIndex = 0
            '
            'm_lbGroups
            '
            Me.m_lbGroups.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_lbGroups.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed
            Me.m_lbGroups.FormattingEnabled = True
            Me.m_lbGroups.IntegralHeight = False
            Me.m_lbGroups.Location = New System.Drawing.Point(0, 0)
            Me.m_lbGroups.Name = "m_lbGroups"
            Me.m_lbGroups.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended
            Me.m_lbGroups.Size = New System.Drawing.Size(119, 261)
            Me.m_lbGroups.Sorted = True
            Me.m_lbGroups.SortThreshold = -9999.0!
            Me.m_lbGroups.SortType = ScientificInterfaceShared.Controls.cGroupListBox.eSortType.[Default]
            Me.m_lbGroups.TabIndex = 1
            '
            'm_zgc
            '
            Me.m_zgc.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_zgc.Location = New System.Drawing.Point(3, 25)
            Me.m_zgc.Name = "m_zgc"
            Me.m_zgc.ScrollGrace = 0
            Me.m_zgc.ScrollMaxX = 0
            Me.m_zgc.ScrollMaxY = 0
            Me.m_zgc.ScrollMaxY2 = 0
            Me.m_zgc.ScrollMinX = 0
            Me.m_zgc.ScrollMinY = 0
            Me.m_zgc.ScrollMinY2 = 0
            Me.m_zgc.Size = New System.Drawing.Size(731, 430)
            Me.m_zgc.TabIndex = 11
            '
            'm_ts
            '
            Me.m_ts.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tsbtnShowHideGroups, Me.tslblSSValue, Me.tsblbSS, ToolStripSeparator5, Me.m_tsdrpdnbtnBiomassCatch, Me.m_tsddPlotType, ToolStripSeparator2, Me.m_tsddGraphOptions, Me.m_tsbExplore})
            Me.m_ts.Location = New System.Drawing.Point(0, 0)
            Me.m_ts.Name = "m_ts"
            Me.m_ts.Size = New System.Drawing.Size(860, 25)
            Me.m_ts.TabIndex = 10
            Me.m_ts.Text = "ToolStrip1"
            '
            'm_tsbtnShowHideGroups
            '
            Me.m_tsbtnShowHideGroups.Image = Global.ScientificInterface.My.Resources.Resources.Eye_open
            Me.m_tsbtnShowHideGroups.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.m_tsbtnShowHideGroups.Name = "m_tsbtnShowHideGroups"
            Me.m_tsbtnShowHideGroups.Size = New System.Drawing.Size(101, 22)
            Me.m_tsbtnShowHideGroups.Text = "Show &groups..."
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
            'm_tsdrpdnbtnBiomassCatch
            '
            Me.m_tsdrpdnbtnBiomassCatch.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tsmiBiomass, Me.m_tsmiCatch})
            Me.m_tsdrpdnbtnBiomassCatch.Image = Global.ScientificInterface.My.Resources.Resources.OptionsHS
            Me.m_tsdrpdnbtnBiomassCatch.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.m_tsdrpdnbtnBiomassCatch.Name = "m_tsdrpdnbtnBiomassCatch"
            Me.m_tsdrpdnbtnBiomassCatch.Size = New System.Drawing.Size(116, 22)
            Me.m_tsdrpdnbtnBiomassCatch.Text = "&Biomass or catch"
            '
            'm_tsmiBiomass
            '
            Me.m_tsmiBiomass.Checked = True
            Me.m_tsmiBiomass.CheckOnClick = True
            Me.m_tsmiBiomass.CheckState = System.Windows.Forms.CheckState.Checked
            Me.m_tsmiBiomass.Name = "m_tsmiBiomass"
            Me.m_tsmiBiomass.Size = New System.Drawing.Size(123, 22)
            Me.m_tsmiBiomass.Text = "&Biomass"
            '
            'm_tsmiCatch
            '
            Me.m_tsmiCatch.CheckOnClick = True
            Me.m_tsmiCatch.Name = "m_tsmiCatch"
            Me.m_tsmiCatch.Size = New System.Drawing.Size(123, 22)
            Me.m_tsmiCatch.Text = "&Catch"
            '
            'm_tsddPlotType
            '
            Me.m_tsddPlotType.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tsmiCumulative, Me.m_tsmiRelative})
            Me.m_tsddPlotType.Image = Global.ScientificInterface.My.Resources.Resources.OptionsHS
            Me.m_tsddPlotType.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.m_tsddPlotType.Name = "m_tsddPlotType"
            Me.m_tsddPlotType.Size = New System.Drawing.Size(84, 22)
            Me.m_tsddPlotType.Text = "&Plot types"
            '
            'm_tsmiCumulative
            '
            Me.m_tsmiCumulative.CheckOnClick = True
            Me.m_tsmiCumulative.Name = "m_tsmiCumulative"
            Me.m_tsmiCumulative.Size = New System.Drawing.Size(138, 22)
            Me.m_tsmiCumulative.Text = "&Cumulative"
            '
            'm_tsmiRelative
            '
            Me.m_tsmiRelative.Checked = True
            Me.m_tsmiRelative.CheckOnClick = True
            Me.m_tsmiRelative.CheckState = System.Windows.Forms.CheckState.Checked
            Me.m_tsmiRelative.Name = "m_tsmiRelative"
            Me.m_tsmiRelative.Size = New System.Drawing.Size(138, 22)
            Me.m_tsmiRelative.Text = "&Relative"
            '
            'm_tsddGraphOptions
            '
            Me.m_tsddGraphOptions.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tsmiAutoscale, Me.m_tsmiCustomScaleLabel, Me.m_tsmiMax, Me.m_tstbMax, Me.m_tsmiMin, Me.m_tstbMin, ToolStripSeparator4, Me.m_tsmiShowAnnualOutput, Me.m_tsmShowMultipleRuns, Me.m_tsmiShowLegend})
            Me.m_tsddGraphOptions.Image = Global.ScientificInterface.My.Resources.Resources.OptionsHS
            Me.m_tsddGraphOptions.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.m_tsddGraphOptions.Name = "m_tsddGraphOptions"
            Me.m_tsddGraphOptions.Size = New System.Drawing.Size(103, 22)
            Me.m_tsddGraphOptions.Text = "Graph &options"
            '
            'm_tsmiAutoscale
            '
            Me.m_tsmiAutoscale.Checked = True
            Me.m_tsmiAutoscale.CheckOnClick = True
            Me.m_tsmiAutoscale.CheckState = System.Windows.Forms.CheckState.Checked
            Me.m_tsmiAutoscale.Name = "m_tsmiAutoscale"
            Me.m_tsmiAutoscale.Size = New System.Drawing.Size(174, 22)
            Me.m_tsmiAutoscale.Text = "&Auto scale"
            '
            'm_tsmiCustomScaleLabel
            '
            Me.m_tsmiCustomScaleLabel.CheckOnClick = True
            Me.m_tsmiCustomScaleLabel.Name = "m_tsmiCustomScaleLabel"
            Me.m_tsmiCustomScaleLabel.Size = New System.Drawing.Size(174, 22)
            Me.m_tsmiCustomScaleLabel.Text = "Custom &scale"
            '
            'm_tsmiMax
            '
            Me.m_tsmiMax.Margin = New System.Windows.Forms.Padding(15, 0, 0, 0)
            Me.m_tsmiMax.Name = "m_tsmiMax"
            Me.m_tsmiMax.Size = New System.Drawing.Size(174, 22)
            Me.m_tsmiMax.Text = "M&ax:"
            '
            'm_tstbMax
            '
            Me.m_tstbMax.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.m_tstbMax.Margin = New System.Windows.Forms.Padding(50, -21, 1, 1)
            Me.m_tstbMax.Name = "m_tstbMax"
            Me.m_tstbMax.Size = New System.Drawing.Size(50, 21)
            '
            'm_tsmiMin
            '
            Me.m_tsmiMin.Margin = New System.Windows.Forms.Padding(15, 0, 0, 0)
            Me.m_tsmiMin.Name = "m_tsmiMin"
            Me.m_tsmiMin.Size = New System.Drawing.Size(174, 22)
            Me.m_tsmiMin.Text = "M&in:"
            '
            'm_tstbMin
            '
            Me.m_tstbMin.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.m_tstbMin.Margin = New System.Windows.Forms.Padding(50, -21, 1, 1)
            Me.m_tstbMin.Name = "m_tstbMin"
            Me.m_tstbMin.Size = New System.Drawing.Size(50, 21)
            '
            'm_tsmiShowAnnualOutput
            '
            Me.m_tsmiShowAnnualOutput.Name = "m_tsmiShowAnnualOutput"
            Me.m_tsmiShowAnnualOutput.Size = New System.Drawing.Size(174, 22)
            Me.m_tsmiShowAnnualOutput.Text = "&Annual output"
            '
            'm_tsmShowMultipleRuns
            '
            Me.m_tsmShowMultipleRuns.Name = "m_tsmShowMultipleRuns"
            Me.m_tsmShowMultipleRuns.Size = New System.Drawing.Size(174, 22)
            Me.m_tsmShowMultipleRuns.Text = "Show &multiple runs"
            '
            'm_tsmiShowLegend
            '
            Me.m_tsmiShowLegend.Name = "m_tsmiShowLegend"
            Me.m_tsmiShowLegend.Size = New System.Drawing.Size(174, 22)
            Me.m_tsmiShowLegend.Text = "Show &legend"
            '
            'm_tsbExplore
            '
            Me.m_tsbExplore.CheckOnClick = True
            Me.m_tsbExplore.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            Me.m_tsbExplore.Image = CType(resources.GetObject("m_tsbExplore.Image"), System.Drawing.Image)
            Me.m_tsbExplore.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.m_tsbExplore.Name = "m_tsbExplore"
            Me.m_tsbExplore.Size = New System.Drawing.Size(82, 22)
            Me.m_tsbExplore.Text = "Explore results"
            Me.m_tsbExplore.ToolTipText = "Check to explore the Ecosim results graph with a cursor"
            '
            'ucBiomassPlotzgc
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.SplitContainer1)
            Me.Controls.Add(Me.m_zgc)
            Me.Controls.Add(Me.m_ts)
            Me.Name = "ucBiomassPlotzgc"
            Me.Size = New System.Drawing.Size(860, 460)
            Me.SplitContainer1.Panel1.ResumeLayout(False)
            Me.SplitContainer1.Panel2.ResumeLayout(False)
            Me.SplitContainer1.ResumeLayout(False)
            Me.m_ts.ResumeLayout(False)
            Me.m_ts.PerformLayout()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub

        Private WithEvents m_ts As System.Windows.Forms.ToolStrip
        Private WithEvents tsblbSS As System.Windows.Forms.ToolStripLabel
        Private WithEvents m_zgc As ZedGraph.ZedGraphControl
        Private WithEvents SplitContainer1 As System.Windows.Forms.SplitContainer
        Private WithEvents m_lbRuns As System.Windows.Forms.ListBox
        Private WithEvents m_lbGroups As ScientificInterfaceShared.Controls.cGroupListBox
        Private WithEvents tslblSSValue As System.Windows.Forms.ToolStripLabel
        Private WithEvents m_tsddGraphOptions As System.Windows.Forms.ToolStripDropDownButton
        Private WithEvents m_tsmiShowAnnualOutput As System.Windows.Forms.ToolStripMenuItem
        Private WithEvents m_tsmShowMultipleRuns As System.Windows.Forms.ToolStripMenuItem
        Private WithEvents m_tsmiShowLegend As System.Windows.Forms.ToolStripMenuItem
        Private WithEvents m_tsbExplore As System.Windows.Forms.ToolStripButton
        Private WithEvents m_tsdrpdnbtnBiomassCatch As System.Windows.Forms.ToolStripDropDownButton
        Private WithEvents m_tsbtnShowHideGroups As System.Windows.Forms.ToolStripButton
        Private WithEvents m_tsmiBiomass As System.Windows.Forms.ToolStripMenuItem
        Private WithEvents m_tsddPlotType As System.Windows.Forms.ToolStripDropDownButton
        Private WithEvents m_tsmiCatch As System.Windows.Forms.ToolStripMenuItem
        Private WithEvents m_tsmiCumulative As System.Windows.Forms.ToolStripMenuItem
        Private WithEvents m_tsmiRelative As System.Windows.Forms.ToolStripMenuItem
        Private WithEvents m_tsmiAutoscale As System.Windows.Forms.ToolStripMenuItem
        Private WithEvents m_tsmiCustomScaleLabel As System.Windows.Forms.ToolStripMenuItem
        Private WithEvents m_tstbMax As System.Windows.Forms.ToolStripTextBox
        Private WithEvents m_tsmiMax As System.Windows.Forms.ToolStripMenuItem
        Private WithEvents m_tsmiMin As System.Windows.Forms.ToolStripMenuItem
        Private WithEvents m_tstbMin As System.Windows.Forms.ToolStripTextBox

    End Class

End Namespace