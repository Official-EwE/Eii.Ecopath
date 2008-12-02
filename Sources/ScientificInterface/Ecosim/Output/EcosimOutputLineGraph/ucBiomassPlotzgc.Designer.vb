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
            Me.SplitContainer1 = New System.Windows.Forms.SplitContainer
            Me.lbOverlay = New System.Windows.Forms.ListBox
            Me.lbGroups = New ScientificInterfaceShared.Controls.LegendListBox
            Me.m_zgc = New ZedGraph.ZedGraphControl
            Me.m_ts = New System.Windows.Forms.ToolStrip
            Me.tslblSSValue = New System.Windows.Forms.ToolStripLabel
            Me.tsblbSS = New System.Windows.Forms.ToolStripLabel
            Me.m_tsdrpdnbtnBiomassCatch = New System.Windows.Forms.ToolStripDropDownButton
            Me.BiomassToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
            Me.CatchToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
            Me.ToolStripSeparator3 = New System.Windows.Forms.ToolStripSeparator
            Me.m_tsdrpdnbtnPlotType = New System.Windows.Forms.ToolStripDropDownButton
            Me.CumulativeToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
            Me.RelativeToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
            Me.ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator
            Me.m_tsdrpdnbtnGraphOptions = New System.Windows.Forms.ToolStripDropDownButton
            Me.m_tlsAutoScaleToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
            Me.m_tlsCustomScaleToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
            Me.SetMaxToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
            Me.m_tstbxSetMax = New System.Windows.Forms.ToolStripTextBox
            Me.SetMinToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
            Me.m_tstbxSetMin = New System.Windows.Forms.ToolStripTextBox
            Me.ToolStripSeparator4 = New System.Windows.Forms.ToolStripSeparator
            Me.AnnualOutputToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
            Me.OverlayToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
            Me.ShowLegendToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
            Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator
            Me.SplitContainer1.Panel1.SuspendLayout()
            Me.SplitContainer1.Panel2.SuspendLayout()
            Me.SplitContainer1.SuspendLayout()
            Me.m_ts.SuspendLayout()
            Me.SuspendLayout()
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
            Me.SplitContainer1.Panel1.Controls.Add(Me.lbOverlay)
            '
            'SplitContainer1.Panel2
            '
            Me.SplitContainer1.Panel2.Controls.Add(Me.lbGroups)
            Me.SplitContainer1.Size = New System.Drawing.Size(119, 429)
            Me.SplitContainer1.SplitterDistance = 164
            Me.SplitContainer1.TabIndex = 12
            '
            'lbOverlay
            '
            Me.lbOverlay.Dock = System.Windows.Forms.DockStyle.Fill
            Me.lbOverlay.FormattingEnabled = True
            Me.lbOverlay.IntegralHeight = False
            Me.lbOverlay.Location = New System.Drawing.Point(0, 0)
            Me.lbOverlay.Name = "lbOverlay"
            Me.lbOverlay.Size = New System.Drawing.Size(119, 164)
            Me.lbOverlay.TabIndex = 0
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
            Me.lbGroups.Size = New System.Drawing.Size(119, 261)
            Me.lbGroups.TabIndex = 1
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
            Me.m_ts.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tslblSSValue, Me.tsblbSS, Me.m_tsdrpdnbtnBiomassCatch, Me.ToolStripSeparator3, Me.m_tsdrpdnbtnPlotType, Me.ToolStripSeparator2, Me.m_tsdrpdnbtnGraphOptions, Me.ToolStripSeparator1})
            Me.m_ts.Location = New System.Drawing.Point(0, 0)
            Me.m_ts.Name = "m_ts"
            Me.m_ts.Size = New System.Drawing.Size(860, 25)
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
            'm_tsdrpdnbtnBiomassCatch
            '
            Me.m_tsdrpdnbtnBiomassCatch.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.BiomassToolStripMenuItem, Me.CatchToolStripMenuItem})
            Me.m_tsdrpdnbtnBiomassCatch.Image = Global.ScientificInterface.My.Resources.Resources.OptionsHS
            Me.m_tsdrpdnbtnBiomassCatch.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.m_tsdrpdnbtnBiomassCatch.Name = "m_tsdrpdnbtnBiomassCatch"
            Me.m_tsdrpdnbtnBiomassCatch.Size = New System.Drawing.Size(116, 22)
            Me.m_tsdrpdnbtnBiomassCatch.Text = "&Biomass or catch"
            '
            'BiomassToolStripMenuItem
            '
            Me.BiomassToolStripMenuItem.Checked = True
            Me.BiomassToolStripMenuItem.CheckOnClick = True
            Me.BiomassToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked
            Me.BiomassToolStripMenuItem.Name = "BiomassToolStripMenuItem"
            Me.BiomassToolStripMenuItem.Size = New System.Drawing.Size(123, 22)
            Me.BiomassToolStripMenuItem.Text = "&Biomass"
            '
            'CatchToolStripMenuItem
            '
            Me.CatchToolStripMenuItem.CheckOnClick = True
            Me.CatchToolStripMenuItem.Name = "CatchToolStripMenuItem"
            Me.CatchToolStripMenuItem.Size = New System.Drawing.Size(123, 22)
            Me.CatchToolStripMenuItem.Text = "&Catch"
            '
            'ToolStripSeparator3
            '
            Me.ToolStripSeparator3.Name = "ToolStripSeparator3"
            Me.ToolStripSeparator3.Size = New System.Drawing.Size(6, 25)
            '
            'm_tsdrpdnbtnPlotType
            '
            Me.m_tsdrpdnbtnPlotType.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.CumulativeToolStripMenuItem, Me.RelativeToolStripMenuItem})
            Me.m_tsdrpdnbtnPlotType.Image = Global.ScientificInterface.My.Resources.Resources.OptionsHS
            Me.m_tsdrpdnbtnPlotType.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.m_tsdrpdnbtnPlotType.Name = "m_tsdrpdnbtnPlotType"
            Me.m_tsdrpdnbtnPlotType.Size = New System.Drawing.Size(84, 22)
            Me.m_tsdrpdnbtnPlotType.Text = "&Plot types"
            '
            'CumulativeToolStripMenuItem
            '
            Me.CumulativeToolStripMenuItem.CheckOnClick = True
            Me.CumulativeToolStripMenuItem.Name = "CumulativeToolStripMenuItem"
            Me.CumulativeToolStripMenuItem.Size = New System.Drawing.Size(152, 22)
            Me.CumulativeToolStripMenuItem.Text = "&Cumulative"
            '
            'RelativeToolStripMenuItem
            '
            Me.RelativeToolStripMenuItem.Checked = True
            Me.RelativeToolStripMenuItem.CheckOnClick = True
            Me.RelativeToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked
            Me.RelativeToolStripMenuItem.Name = "RelativeToolStripMenuItem"
            Me.RelativeToolStripMenuItem.Size = New System.Drawing.Size(152, 22)
            Me.RelativeToolStripMenuItem.Text = "&Relative"
            '
            'ToolStripSeparator2
            '
            Me.ToolStripSeparator2.Name = "ToolStripSeparator2"
            Me.ToolStripSeparator2.Size = New System.Drawing.Size(6, 25)
            '
            'm_tsdrpdnbtnGraphOptions
            '
            Me.m_tsdrpdnbtnGraphOptions.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tlsAutoScaleToolStripMenuItem, Me.m_tlsCustomScaleToolStripMenuItem, Me.SetMaxToolStripMenuItem, Me.m_tstbxSetMax, Me.SetMinToolStripMenuItem, Me.m_tstbxSetMin, Me.ToolStripSeparator4, Me.AnnualOutputToolStripMenuItem, Me.OverlayToolStripMenuItem, Me.ShowLegendToolStripMenuItem})
            Me.m_tsdrpdnbtnGraphOptions.Image = Global.ScientificInterface.My.Resources.Resources.OptionsHS
            Me.m_tsdrpdnbtnGraphOptions.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.m_tsdrpdnbtnGraphOptions.Name = "m_tsdrpdnbtnGraphOptions"
            Me.m_tsdrpdnbtnGraphOptions.Size = New System.Drawing.Size(103, 22)
            Me.m_tsdrpdnbtnGraphOptions.Text = "Graph &options"
            '
            'm_tlsAutoScaleToolStripMenuItem
            '
            Me.m_tlsAutoScaleToolStripMenuItem.Checked = True
            Me.m_tlsAutoScaleToolStripMenuItem.CheckOnClick = True
            Me.m_tlsAutoScaleToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked
            Me.m_tlsAutoScaleToolStripMenuItem.Name = "m_tlsAutoScaleToolStripMenuItem"
            Me.m_tlsAutoScaleToolStripMenuItem.Size = New System.Drawing.Size(153, 22)
            Me.m_tlsAutoScaleToolStripMenuItem.Text = "&Auto scale"
            '
            'm_tlsCustomScaleToolStripMenuItem
            '
            Me.m_tlsCustomScaleToolStripMenuItem.CheckOnClick = True
            Me.m_tlsCustomScaleToolStripMenuItem.Name = "m_tlsCustomScaleToolStripMenuItem"
            Me.m_tlsCustomScaleToolStripMenuItem.Size = New System.Drawing.Size(153, 22)
            Me.m_tlsCustomScaleToolStripMenuItem.Text = "Custom &scale"
            '
            'SetMaxToolStripMenuItem
            '
            Me.SetMaxToolStripMenuItem.Margin = New System.Windows.Forms.Padding(15, 0, 0, 0)
            Me.SetMaxToolStripMenuItem.Name = "SetMaxToolStripMenuItem"
            Me.SetMaxToolStripMenuItem.Size = New System.Drawing.Size(153, 22)
            Me.SetMaxToolStripMenuItem.Text = "M&ax:"
            '
            'm_tstbxSetMax
            '
            Me.m_tstbxSetMax.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.m_tstbxSetMax.Margin = New System.Windows.Forms.Padding(50, -21, 1, 1)
            Me.m_tstbxSetMax.Name = "m_tstbxSetMax"
            Me.m_tstbxSetMax.Size = New System.Drawing.Size(50, 21)
            '
            'SetMinToolStripMenuItem
            '
            Me.SetMinToolStripMenuItem.Margin = New System.Windows.Forms.Padding(15, 0, 0, 0)
            Me.SetMinToolStripMenuItem.Name = "SetMinToolStripMenuItem"
            Me.SetMinToolStripMenuItem.Size = New System.Drawing.Size(153, 22)
            Me.SetMinToolStripMenuItem.Text = "M&in:"
            '
            'm_tstbxSetMin
            '
            Me.m_tstbxSetMin.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.m_tstbxSetMin.Margin = New System.Windows.Forms.Padding(50, -21, 1, 1)
            Me.m_tstbxSetMin.Name = "m_tstbxSetMin"
            Me.m_tstbxSetMin.Size = New System.Drawing.Size(50, 21)
            '
            'ToolStripSeparator4
            '
            Me.ToolStripSeparator4.Name = "ToolStripSeparator4"
            Me.ToolStripSeparator4.Size = New System.Drawing.Size(150, 6)
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
        Private WithEvents lbOverlay As System.Windows.Forms.ListBox
        Private WithEvents lbGroups As ScientificInterfaceShared.Controls.LegendListBox
        Private WithEvents tslblSSValue As System.Windows.Forms.ToolStripLabel
        Private WithEvents m_tsdrpdnbtnGraphOptions As System.Windows.Forms.ToolStripDropDownButton
        Private WithEvents AnnualOutputToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
        Private WithEvents OverlayToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
        Private WithEvents ShowLegendToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
        Friend WithEvents m_tsdrpdnbtnPlotType As System.Windows.Forms.ToolStripDropDownButton
        Friend WithEvents CumulativeToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents RelativeToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents ToolStripSeparator2 As System.Windows.Forms.ToolStripSeparator
        Friend WithEvents m_tsdrpdnbtnBiomassCatch As System.Windows.Forms.ToolStripDropDownButton
        Friend WithEvents BiomassToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents CatchToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents ToolStripSeparator3 As System.Windows.Forms.ToolStripSeparator
        Friend WithEvents m_tlsAutoScaleToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents m_tlsCustomScaleToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents m_tstbxSetMax As System.Windows.Forms.ToolStripTextBox
        Friend WithEvents m_tstbxSetMin As System.Windows.Forms.ToolStripTextBox
        Friend WithEvents SetMaxToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents SetMinToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents ToolStripSeparator4 As System.Windows.Forms.ToolStripSeparator

    End Class

End Namespace