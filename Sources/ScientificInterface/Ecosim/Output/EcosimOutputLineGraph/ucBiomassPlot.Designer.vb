Namespace Ecosim

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class ucBiomassPlot
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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ucBiomassPlot))
            Me.gbAnonymous = New System.Windows.Forms.GroupBox
            Me.m_ts = New System.Windows.Forms.ToolStrip
            Me.tsbtnShowHideGroups = New System.Windows.Forms.ToolStripButton
            Me.tslblSSValue = New System.Windows.Forms.ToolStripLabel
            Me.tsblbSS = New System.Windows.Forms.ToolStripLabel
            Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator
            Me.AutoscaleToolstripButton = New System.Windows.Forms.ToolStripButton
            Me.tslblYAxisValue = New System.Windows.Forms.ToolStripLabel
            Me.tstbxYAxisValue = New System.Windows.Forms.ToolStripTextBox
            Me.m_tsbSet = New System.Windows.Forms.ToolStripButton
            Me.ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator
            Me.ToolStripDropDownButton1 = New System.Windows.Forms.ToolStripDropDownButton
            Me.AnnualOutputToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
            Me.OverlayToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
            Me.tcOutput = New System.Windows.Forms.TabControl
            Me.tbpGroup = New System.Windows.Forms.TabPage
            Me.lbGroups = New System.Windows.Forms.ListBox
            Me.tbpLayer = New System.Windows.Forms.TabPage
            Me.clbLayers = New System.Windows.Forms.CheckedListBox
            Me.plBiomassPlot = New System.Windows.Forms.Panel
            Me.gbAnonymous.SuspendLayout()
            Me.m_ts.SuspendLayout()
            Me.tcOutput.SuspendLayout()
            Me.tbpGroup.SuspendLayout()
            Me.tbpLayer.SuspendLayout()
            Me.SuspendLayout()
            '
            'gbAnonymous
            '
            Me.gbAnonymous.Controls.Add(Me.m_ts)
            Me.gbAnonymous.Controls.Add(Me.tcOutput)
            Me.gbAnonymous.Controls.Add(Me.plBiomassPlot)
            Me.gbAnonymous.Dock = System.Windows.Forms.DockStyle.Fill
            Me.gbAnonymous.Location = New System.Drawing.Point(0, 0)
            Me.gbAnonymous.Name = "gbAnonymous"
            Me.gbAnonymous.Size = New System.Drawing.Size(740, 345)
            Me.gbAnonymous.TabIndex = 0
            Me.gbAnonymous.TabStop = False
            Me.gbAnonymous.Text = "Ecosim biomass output"
            '
            'm_ts
            '
            Me.m_ts.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsbtnShowHideGroups, Me.tslblSSValue, Me.tsblbSS, Me.ToolStripSeparator1, Me.AutoscaleToolstripButton, Me.tslblYAxisValue, Me.tstbxYAxisValue, Me.m_tsbSet, Me.ToolStripSeparator2, Me.ToolStripDropDownButton1})
            Me.m_ts.Location = New System.Drawing.Point(3, 16)
            Me.m_ts.Name = "m_ts"
            Me.m_ts.Size = New System.Drawing.Size(734, 25)
            Me.m_ts.TabIndex = 10
            Me.m_ts.Text = "ToolStrip1"
            '
            'tsbtnShowHideGroups
            '
            Me.tsbtnShowHideGroups.Image = Global.ScientificInterface.My.Resources.Resources.Eye_open
            Me.tsbtnShowHideGroups.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.tsbtnShowHideGroups.Name = "tsbtnShowHideGroups"
            Me.tsbtnShowHideGroups.Size = New System.Drawing.Size(101, 22)
            Me.tsbtnShowHideGroups.Text = "Show &groups..."
            '
            'tslblSSValue
            '
            Me.tslblSSValue.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
            Me.tslblSSValue.Name = "tslblSSValue"
            Me.tslblSSValue.Size = New System.Drawing.Size(13, 22)
            Me.tslblSSValue.Text = "0"
            '
            'tsblbSS
            '
            Me.tsblbSS.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
            Me.tsblbSS.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            Me.tsblbSS.Name = "tsblbSS"
            Me.tsblbSS.Size = New System.Drawing.Size(86, 22)
            Me.tsblbSS.Text = "Sum of Squares:"
            '
            'ToolStripSeparator1
            '
            Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
            Me.ToolStripSeparator1.Size = New System.Drawing.Size(6, 25)
            '
            'AutoscaleToolstripButton
            '
            Me.AutoscaleToolstripButton.Checked = True
            Me.AutoscaleToolstripButton.CheckState = System.Windows.Forms.CheckState.Checked
            Me.AutoscaleToolstripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            Me.AutoscaleToolstripButton.Image = CType(resources.GetObject("AutoscaleToolstripButton.Image"), System.Drawing.Image)
            Me.AutoscaleToolstripButton.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.AutoscaleToolstripButton.Name = "AutoscaleToolstripButton"
            Me.AutoscaleToolstripButton.Size = New System.Drawing.Size(58, 22)
            Me.AutoscaleToolstripButton.Text = "&Autoscale"
            '
            'tslblYAxisValue
            '
            Me.tslblYAxisValue.Name = "tslblYAxisValue"
            Me.tslblYAxisValue.Size = New System.Drawing.Size(80, 22)
            Me.tslblYAxisValue.Text = "Scale &Y axis to:"
            '
            'tstbxYAxisValue
            '
            Me.tstbxYAxisValue.AcceptsReturn = True
            Me.tstbxYAxisValue.AutoSize = False
            Me.tstbxYAxisValue.Name = "tstbxYAxisValue"
            Me.tstbxYAxisValue.Size = New System.Drawing.Size(75, 25)
            '
            'm_tsbSet
            '
            Me.m_tsbSet.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
            Me.m_tsbSet.Image = Global.ScientificInterface.My.Resources.Resources.NavForward
            Me.m_tsbSet.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.m_tsbSet.Name = "m_tsbSet"
            Me.m_tsbSet.Size = New System.Drawing.Size(23, 22)
            Me.m_tsbSet.Text = "Set"
            '
            'ToolStripSeparator2
            '
            Me.ToolStripSeparator2.Name = "ToolStripSeparator2"
            Me.ToolStripSeparator2.Size = New System.Drawing.Size(6, 25)
            '
            'ToolStripDropDownButton1
            '
            Me.ToolStripDropDownButton1.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.AnnualOutputToolStripMenuItem, Me.OverlayToolStripMenuItem})
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
            Me.AnnualOutputToolStripMenuItem.Text = "Annual output"
            '
            'OverlayToolStripMenuItem
            '
            Me.OverlayToolStripMenuItem.Name = "OverlayToolStripMenuItem"
            Me.OverlayToolStripMenuItem.Size = New System.Drawing.Size(153, 22)
            Me.OverlayToolStripMenuItem.Text = "Overlay"
            '
            'tcOutput
            '
            Me.tcOutput.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.tcOutput.Controls.Add(Me.tbpGroup)
            Me.tcOutput.Controls.Add(Me.tbpLayer)
            Me.tcOutput.Location = New System.Drawing.Point(616, 45)
            Me.tcOutput.Name = "tcOutput"
            Me.tcOutput.SelectedIndex = 0
            Me.tcOutput.Size = New System.Drawing.Size(118, 295)
            Me.tcOutput.TabIndex = 9
            '
            'tbpGroup
            '
            Me.tbpGroup.Controls.Add(Me.lbGroups)
            Me.tbpGroup.Location = New System.Drawing.Point(4, 22)
            Me.tbpGroup.Name = "tbpGroup"
            Me.tbpGroup.Padding = New System.Windows.Forms.Padding(3)
            Me.tbpGroup.Size = New System.Drawing.Size(110, 269)
            Me.tbpGroup.TabIndex = 1
            Me.tbpGroup.Text = "Group"
            Me.tbpGroup.UseVisualStyleBackColor = True
            '
            'lbGroups
            '
            Me.lbGroups.Dock = System.Windows.Forms.DockStyle.Fill
            Me.lbGroups.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed
            Me.lbGroups.FormattingEnabled = True
            Me.lbGroups.Location = New System.Drawing.Point(3, 3)
            Me.lbGroups.Name = "lbGroups"
            Me.lbGroups.Size = New System.Drawing.Size(104, 251)
            Me.lbGroups.TabIndex = 2
            '
            'tbpLayer
            '
            Me.tbpLayer.Controls.Add(Me.clbLayers)
            Me.tbpLayer.Location = New System.Drawing.Point(4, 22)
            Me.tbpLayer.Name = "tbpLayer"
            Me.tbpLayer.Padding = New System.Windows.Forms.Padding(3)
            Me.tbpLayer.Size = New System.Drawing.Size(110, 269)
            Me.tbpLayer.TabIndex = 0
            Me.tbpLayer.Text = "Layer"
            Me.tbpLayer.UseVisualStyleBackColor = True
            '
            'clbLayers
            '
            Me.clbLayers.CheckOnClick = True
            Me.clbLayers.Dock = System.Windows.Forms.DockStyle.Fill
            Me.clbLayers.FormattingEnabled = True
            Me.clbLayers.Location = New System.Drawing.Point(3, 3)
            Me.clbLayers.Name = "clbLayers"
            Me.clbLayers.Size = New System.Drawing.Size(104, 259)
            Me.clbLayers.TabIndex = 0
            '
            'plBiomassPlot
            '
            Me.plBiomassPlot.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.plBiomassPlot.BackColor = System.Drawing.SystemColors.Window
            Me.plBiomassPlot.Location = New System.Drawing.Point(7, 39)
            Me.plBiomassPlot.Name = "plBiomassPlot"
            Me.plBiomassPlot.Size = New System.Drawing.Size(603, 301)
            Me.plBiomassPlot.TabIndex = 0
            '
            'ucBiomassPlot
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.gbAnonymous)
            Me.Name = "ucBiomassPlot"
            Me.Size = New System.Drawing.Size(740, 345)
            Me.gbAnonymous.ResumeLayout(False)
            Me.gbAnonymous.PerformLayout()
            Me.m_ts.ResumeLayout(False)
            Me.m_ts.PerformLayout()
            Me.tcOutput.ResumeLayout(False)
            Me.tbpGroup.ResumeLayout(False)
            Me.tbpLayer.ResumeLayout(False)
            Me.ResumeLayout(False)

        End Sub
        Friend WithEvents gbAnonymous As System.Windows.Forms.GroupBox
        Friend WithEvents tcOutput As System.Windows.Forms.TabControl
        Friend WithEvents tbpLayer As System.Windows.Forms.TabPage
        Friend WithEvents clbLayers As System.Windows.Forms.CheckedListBox
        Friend WithEvents tbpGroup As System.Windows.Forms.TabPage
        Friend WithEvents lbGroups As System.Windows.Forms.ListBox
        Friend WithEvents plBiomassPlot As System.Windows.Forms.Panel
        Friend WithEvents m_ts As System.Windows.Forms.ToolStrip
        Friend WithEvents tsbtnShowHideGroups As System.Windows.Forms.ToolStripButton
        Friend WithEvents tslblYAxisValue As System.Windows.Forms.ToolStripLabel
        Friend WithEvents tstbxYAxisValue As System.Windows.Forms.ToolStripTextBox
        Friend WithEvents ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
        Friend WithEvents ToolStripDropDownButton1 As System.Windows.Forms.ToolStripDropDownButton
        Friend WithEvents OverlayToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents AnnualOutputToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents tslblSSValue As System.Windows.Forms.ToolStripLabel
        Friend WithEvents tsblbSS As System.Windows.Forms.ToolStripLabel
        Friend WithEvents AutoscaleToolstripButton As System.Windows.Forms.ToolStripButton
        Friend WithEvents ToolStripSeparator2 As System.Windows.Forms.ToolStripSeparator
        Friend WithEvents m_tsbSet As System.Windows.Forms.ToolStripButton

    End Class

End Namespace