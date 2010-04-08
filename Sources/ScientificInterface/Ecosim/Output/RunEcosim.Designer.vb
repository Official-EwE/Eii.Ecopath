Imports WeifenLuo.WinFormsUI.Docking
Imports ScientificInterfaceShared

Namespace Ecosim

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class RunEcosim
        Inherits frmEwE

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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(RunEcosim))
            Dim ToolStripSeparator2 As System.Windows.Forms.ToolStripSeparator
            Dim ToolStripSeparator4 As System.Windows.Forms.ToolStripSeparator
            Me.btnRunOrStop = New System.Windows.Forms.Button
            Me.m_tsMain = New System.Windows.Forms.ToolStrip
            Me.tslTarget = New System.Windows.Forms.ToolStripLabel
            Me.tscbTarget = New ScientificInterfaceShared.Controls.cCustomToolstripComboBox
            Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator
            Me.tsbSetTo0 = New System.Windows.Forms.ToolStripButton
            Me.tsbSetToValue = New System.Windows.Forms.ToolStripButton
            Me.tsbResetFs = New System.Windows.Forms.ToolStripButton
            Me.m_sketchPad = New ScientificInterfaceShared.Controls.ucForcingSketchPad
            Me.m_spContainer = New System.Windows.Forms.SplitContainer
            Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel
            Me.m_ts = New System.Windows.Forms.ToolStrip
            Me.m_tsbtnShowHideGroups = New System.Windows.Forms.ToolStripButton
            Me.tslblSSValue = New System.Windows.Forms.ToolStripLabel
            Me.tsblbSS = New System.Windows.Forms.ToolStripLabel
            Me.m_tsdrpdnbtnContent = New System.Windows.Forms.ToolStripDropDownButton
            Me.m_tsmiBiomass = New System.Windows.Forms.ToolStripMenuItem
            Me.m_tsmiCatch = New System.Windows.Forms.ToolStripMenuItem
            Me.m_tsddPlotType = New System.Windows.Forms.ToolStripDropDownButton
            Me.m_tsmiCumulative = New System.Windows.Forms.ToolStripMenuItem
            Me.m_tsmiRelative = New System.Windows.Forms.ToolStripMenuItem
            Me.m_tssbExplore = New System.Windows.Forms.ToolStripSplitButton
            Me.m_tsmiSortMostChanged = New System.Windows.Forms.ToolStripMenuItem
            Me.ChangeAmountToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
            Me.m_tstbChangeAmount = New System.Windows.Forms.ToolStripTextBox
            Me.ToolStripSeparator3 = New System.Windows.Forms.ToolStripSeparator
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
            Me.m_scGraph = New System.Windows.Forms.SplitContainer
            Me.m_graph = New ZedGraph.ZedGraphControl
            Me.m_scOptions = New System.Windows.Forms.SplitContainer
            Me.m_lblRuns = New System.Windows.Forms.Label
            Me.m_lbRuns = New System.Windows.Forms.ListBox
            Me.m_lblGroups = New System.Windows.Forms.Label
            Me.m_lbGroups = New ScientificInterfaceShared.Controls.cGroupListBox
            ToolStripSeparator5 = New System.Windows.Forms.ToolStripSeparator
            ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator
            ToolStripSeparator4 = New System.Windows.Forms.ToolStripSeparator
            Me.m_tsMain.SuspendLayout()
            Me.m_spContainer.Panel1.SuspendLayout()
            Me.m_spContainer.Panel2.SuspendLayout()
            Me.m_spContainer.SuspendLayout()
            Me.TableLayoutPanel1.SuspendLayout()
            Me.m_ts.SuspendLayout()
            Me.m_scGraph.Panel1.SuspendLayout()
            Me.m_scGraph.Panel2.SuspendLayout()
            Me.m_scGraph.SuspendLayout()
            Me.m_scOptions.Panel1.SuspendLayout()
            Me.m_scOptions.Panel2.SuspendLayout()
            Me.m_scOptions.SuspendLayout()
            Me.SuspendLayout()
            '
            'ToolStripSeparator5
            '
            ToolStripSeparator5.Name = "ToolStripSeparator5"
            resources.ApplyResources(ToolStripSeparator5, "ToolStripSeparator5")
            '
            'ToolStripSeparator2
            '
            ToolStripSeparator2.Name = "ToolStripSeparator2"
            resources.ApplyResources(ToolStripSeparator2, "ToolStripSeparator2")
            '
            'ToolStripSeparator4
            '
            ToolStripSeparator4.Name = "ToolStripSeparator4"
            resources.ApplyResources(ToolStripSeparator4, "ToolStripSeparator4")
            '
            'btnRunOrStop
            '
            resources.ApplyResources(Me.btnRunOrStop, "btnRunOrStop")
            Me.btnRunOrStop.DialogResult = System.Windows.Forms.DialogResult.Cancel
            Me.btnRunOrStop.Name = "btnRunOrStop"
            Me.btnRunOrStop.UseVisualStyleBackColor = True
            '
            'm_tsMain
            '
            Me.m_tsMain.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tslTarget, Me.tscbTarget, Me.ToolStripSeparator1, Me.tsbSetTo0, Me.tsbSetToValue, Me.tsbResetFs})
            resources.ApplyResources(Me.m_tsMain, "m_tsMain")
            Me.m_tsMain.Name = "m_tsMain"
            '
            'tslTarget
            '
            Me.tslTarget.Name = "tslTarget"
            resources.ApplyResources(Me.tslTarget, "tslTarget")
            '
            'tscbTarget
            '
            resources.ApplyResources(Me.tscbTarget, "tscbTarget")
            Me.tscbTarget.DropDownHeight = 1
            Me.tscbTarget.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.tscbTarget.Name = "tscbTarget"
            '
            'ToolStripSeparator1
            '
            Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
            resources.ApplyResources(Me.ToolStripSeparator1, "ToolStripSeparator1")
            '
            'tsbSetTo0
            '
            Me.tsbSetTo0.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            resources.ApplyResources(Me.tsbSetTo0, "tsbSetTo0")
            Me.tsbSetTo0.Name = "tsbSetTo0"
            '
            'tsbSetToValue
            '
            Me.tsbSetToValue.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            resources.ApplyResources(Me.tsbSetToValue, "tsbSetToValue")
            Me.tsbSetToValue.Name = "tsbSetToValue"
            '
            'tsbResetFs
            '
            Me.tsbResetFs.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            resources.ApplyResources(Me.tsbResetFs, "tsbResetFs")
            Me.tsbResetFs.Name = "tsbResetFs"
            '
            'm_sketchPad
            '
            resources.ApplyResources(Me.m_sketchPad, "m_sketchPad")
            Me.m_sketchPad.BackColor = System.Drawing.Color.FromArgb(CType(CType(231, Byte), Integer), CType(CType(235, Byte), Integer), CType(CType(250, Byte), Integer))
            Me.m_sketchPad.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.m_sketchPad.Cursor = System.Windows.Forms.Cursors.Hand
            Me.m_sketchPad.DisplayAxis = True
            Me.m_sketchPad.Editable = True
            Me.m_sketchPad.Handler = Nothing
            Me.m_sketchPad.IsSeasonal = False
            Me.m_sketchPad.Name = "m_sketchPad"
            Me.m_sketchPad.Shape = Nothing
            Me.m_sketchPad.ShapeColor = System.Drawing.Color.AliceBlue
            Me.m_sketchPad.ShowXMark = False
            Me.m_sketchPad.SketchDrawMode = ScientificInterfaceShared.Definitions.eSketchDrawModeTypes.Fill
            Me.m_sketchPad.UIContext = Nothing
            Me.m_sketchPad.XMarkLabel = ""
            Me.m_sketchPad.XMarkValue = -9999.0!
            Me.m_sketchPad.YAxisAutoScaleMode = ScientificInterfaceShared.Definitions.eAxisAutoScaleModeTypes.[Auto]
            Me.m_sketchPad.YAxisMaxValue = 0.0!
            Me.m_sketchPad.YAxisMinValue = 1.0!
            Me.m_sketchPad.YMarkLabel = ""
            Me.m_sketchPad.YMarkValue = -9999.0!
            '
            'm_spContainer
            '
            resources.ApplyResources(Me.m_spContainer, "m_spContainer")
            Me.m_spContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel2
            Me.m_spContainer.Name = "m_spContainer"
            '
            'm_spContainer.Panel1
            '
            Me.m_spContainer.Panel1.Controls.Add(Me.TableLayoutPanel1)
            '
            'm_spContainer.Panel2
            '
            Me.m_spContainer.Panel2.Controls.Add(Me.m_tsMain)
            Me.m_spContainer.Panel2.Controls.Add(Me.btnRunOrStop)
            Me.m_spContainer.Panel2.Controls.Add(Me.m_sketchPad)
            '
            'TableLayoutPanel1
            '
            resources.ApplyResources(Me.TableLayoutPanel1, "TableLayoutPanel1")
            Me.TableLayoutPanel1.Controls.Add(Me.m_ts, 0, 0)
            Me.TableLayoutPanel1.Controls.Add(Me.m_scGraph, 0, 1)
            Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
            '
            'm_ts
            '
            Me.m_ts.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tsbtnShowHideGroups, Me.tslblSSValue, Me.tsblbSS, ToolStripSeparator5, Me.m_tsdrpdnbtnContent, Me.m_tsddPlotType, ToolStripSeparator2, Me.m_tssbExplore, Me.ToolStripSeparator3, Me.m_tsddGraphOptions})
            resources.ApplyResources(Me.m_ts, "m_ts")
            Me.m_ts.Name = "m_ts"
            '
            'm_tsbtnShowHideGroups
            '
            Me.m_tsbtnShowHideGroups.Image = Global.ScientificInterface.My.Resources.Resources.DisplayGroups
            resources.ApplyResources(Me.m_tsbtnShowHideGroups, "m_tsbtnShowHideGroups")
            Me.m_tsbtnShowHideGroups.Name = "m_tsbtnShowHideGroups"
            '
            'tslblSSValue
            '
            Me.tslblSSValue.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
            Me.tslblSSValue.Name = "tslblSSValue"
            resources.ApplyResources(Me.tslblSSValue, "tslblSSValue")
            '
            'tsblbSS
            '
            Me.tsblbSS.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
            Me.tsblbSS.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            Me.tsblbSS.Name = "tsblbSS"
            resources.ApplyResources(Me.tsblbSS, "tsblbSS")
            '
            'm_tsdrpdnbtnContent
            '
            Me.m_tsdrpdnbtnContent.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tsmiBiomass, Me.m_tsmiCatch})
            Me.m_tsdrpdnbtnContent.Image = Global.ScientificInterface.My.Resources.Resources.Importance
            resources.ApplyResources(Me.m_tsdrpdnbtnContent, "m_tsdrpdnbtnContent")
            Me.m_tsdrpdnbtnContent.Name = "m_tsdrpdnbtnContent"
            '
            'm_tsmiBiomass
            '
            Me.m_tsmiBiomass.Checked = True
            Me.m_tsmiBiomass.CheckOnClick = True
            Me.m_tsmiBiomass.CheckState = System.Windows.Forms.CheckState.Checked
            Me.m_tsmiBiomass.Name = "m_tsmiBiomass"
            resources.ApplyResources(Me.m_tsmiBiomass, "m_tsmiBiomass")
            '
            'm_tsmiCatch
            '
            Me.m_tsmiCatch.CheckOnClick = True
            Me.m_tsmiCatch.Name = "m_tsmiCatch"
            resources.ApplyResources(Me.m_tsmiCatch, "m_tsmiCatch")
            '
            'm_tsddPlotType
            '
            Me.m_tsddPlotType.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tsmiCumulative, Me.m_tsmiRelative})
            resources.ApplyResources(Me.m_tsddPlotType, "m_tsddPlotType")
            Me.m_tsddPlotType.Name = "m_tsddPlotType"
            '
            'm_tsmiCumulative
            '
            Me.m_tsmiCumulative.CheckOnClick = True
            Me.m_tsmiCumulative.Name = "m_tsmiCumulative"
            resources.ApplyResources(Me.m_tsmiCumulative, "m_tsmiCumulative")
            '
            'm_tsmiRelative
            '
            Me.m_tsmiRelative.Checked = True
            Me.m_tsmiRelative.CheckOnClick = True
            Me.m_tsmiRelative.CheckState = System.Windows.Forms.CheckState.Checked
            Me.m_tsmiRelative.Name = "m_tsmiRelative"
            resources.ApplyResources(Me.m_tsmiRelative, "m_tsmiRelative")
            '
            'm_tssbExplore
            '
            Me.m_tssbExplore.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tsmiSortMostChanged, Me.ChangeAmountToolStripMenuItem, Me.m_tstbChangeAmount})
            Me.m_tssbExplore.Image = Global.ScientificInterface.My.Resources.Resources.ZoomHS
            resources.ApplyResources(Me.m_tssbExplore, "m_tssbExplore")
            Me.m_tssbExplore.Name = "m_tssbExplore"
            '
            'm_tsmiSortMostChanged
            '
            Me.m_tsmiSortMostChanged.Name = "m_tsmiSortMostChanged"
            resources.ApplyResources(Me.m_tsmiSortMostChanged, "m_tsmiSortMostChanged")
            '
            'ChangeAmountToolStripMenuItem
            '
            Me.ChangeAmountToolStripMenuItem.Margin = New System.Windows.Forms.Padding(15, 0, 0, 0)
            Me.ChangeAmountToolStripMenuItem.Name = "ChangeAmountToolStripMenuItem"
            resources.ApplyResources(Me.ChangeAmountToolStripMenuItem, "ChangeAmountToolStripMenuItem")
            '
            'm_tstbChangeAmount
            '
            Me.m_tstbChangeAmount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.m_tstbChangeAmount.Margin = New System.Windows.Forms.Padding(110, -21, 1, 1)
            resources.ApplyResources(Me.m_tstbChangeAmount, "m_tstbChangeAmount")
            Me.m_tstbChangeAmount.Name = "m_tstbChangeAmount"
            '
            'ToolStripSeparator3
            '
            Me.ToolStripSeparator3.Name = "ToolStripSeparator3"
            resources.ApplyResources(Me.ToolStripSeparator3, "ToolStripSeparator3")
            '
            'm_tsddGraphOptions
            '
            Me.m_tsddGraphOptions.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tsmiAutoscale, Me.m_tsmiCustomScaleLabel, Me.m_tsmiMax, Me.m_tstbMax, Me.m_tsmiMin, Me.m_tstbMin, ToolStripSeparator4, Me.m_tsmiShowAnnualOutput, Me.m_tsmShowMultipleRuns, Me.m_tsmiShowLegend})
            Me.m_tsddGraphOptions.Image = Global.ScientificInterface.My.Resources.Resources.OptionsHS
            resources.ApplyResources(Me.m_tsddGraphOptions, "m_tsddGraphOptions")
            Me.m_tsddGraphOptions.Name = "m_tsddGraphOptions"
            '
            'm_tsmiAutoscale
            '
            Me.m_tsmiAutoscale.Checked = True
            Me.m_tsmiAutoscale.CheckOnClick = True
            Me.m_tsmiAutoscale.CheckState = System.Windows.Forms.CheckState.Checked
            Me.m_tsmiAutoscale.Name = "m_tsmiAutoscale"
            resources.ApplyResources(Me.m_tsmiAutoscale, "m_tsmiAutoscale")
            '
            'm_tsmiCustomScaleLabel
            '
            Me.m_tsmiCustomScaleLabel.CheckOnClick = True
            Me.m_tsmiCustomScaleLabel.Name = "m_tsmiCustomScaleLabel"
            resources.ApplyResources(Me.m_tsmiCustomScaleLabel, "m_tsmiCustomScaleLabel")
            '
            'm_tsmiMax
            '
            Me.m_tsmiMax.Margin = New System.Windows.Forms.Padding(15, 0, 0, 0)
            Me.m_tsmiMax.Name = "m_tsmiMax"
            resources.ApplyResources(Me.m_tsmiMax, "m_tsmiMax")
            '
            'm_tstbMax
            '
            Me.m_tstbMax.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.m_tstbMax.Margin = New System.Windows.Forms.Padding(50, -21, 1, 1)
            resources.ApplyResources(Me.m_tstbMax, "m_tstbMax")
            Me.m_tstbMax.Name = "m_tstbMax"
            '
            'm_tsmiMin
            '
            Me.m_tsmiMin.Margin = New System.Windows.Forms.Padding(15, 0, 0, 0)
            Me.m_tsmiMin.Name = "m_tsmiMin"
            resources.ApplyResources(Me.m_tsmiMin, "m_tsmiMin")
            '
            'm_tstbMin
            '
            Me.m_tstbMin.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.m_tstbMin.Margin = New System.Windows.Forms.Padding(50, -21, 1, 1)
            resources.ApplyResources(Me.m_tstbMin, "m_tstbMin")
            Me.m_tstbMin.Name = "m_tstbMin"
            '
            'm_tsmiShowAnnualOutput
            '
            Me.m_tsmiShowAnnualOutput.Name = "m_tsmiShowAnnualOutput"
            resources.ApplyResources(Me.m_tsmiShowAnnualOutput, "m_tsmiShowAnnualOutput")
            '
            'm_tsmShowMultipleRuns
            '
            Me.m_tsmShowMultipleRuns.Name = "m_tsmShowMultipleRuns"
            resources.ApplyResources(Me.m_tsmShowMultipleRuns, "m_tsmShowMultipleRuns")
            '
            'm_tsmiShowLegend
            '
            Me.m_tsmiShowLegend.Name = "m_tsmiShowLegend"
            resources.ApplyResources(Me.m_tsmiShowLegend, "m_tsmiShowLegend")
            '
            'm_scGraph
            '
            resources.ApplyResources(Me.m_scGraph, "m_scGraph")
            Me.m_scGraph.FixedPanel = System.Windows.Forms.FixedPanel.Panel2
            Me.m_scGraph.Name = "m_scGraph"
            '
            'm_scGraph.Panel1
            '
            Me.m_scGraph.Panel1.Controls.Add(Me.m_graph)
            '
            'm_scGraph.Panel2
            '
            Me.m_scGraph.Panel2.Controls.Add(Me.m_scOptions)
            '
            'm_graph
            '
            resources.ApplyResources(Me.m_graph, "m_graph")
            Me.m_graph.Name = "m_graph"
            Me.m_graph.ScrollGrace = 0
            Me.m_graph.ScrollMaxX = 0
            Me.m_graph.ScrollMaxY = 0
            Me.m_graph.ScrollMaxY2 = 0
            Me.m_graph.ScrollMinX = 0
            Me.m_graph.ScrollMinY = 0
            Me.m_graph.ScrollMinY2 = 0
            '
            'm_scOptions
            '
            resources.ApplyResources(Me.m_scOptions, "m_scOptions")
            Me.m_scOptions.Name = "m_scOptions"
            '
            'm_scOptions.Panel1
            '
            Me.m_scOptions.Panel1.Controls.Add(Me.m_lblRuns)
            Me.m_scOptions.Panel1.Controls.Add(Me.m_lbRuns)
            '
            'm_scOptions.Panel2
            '
            Me.m_scOptions.Panel2.Controls.Add(Me.m_lblGroups)
            Me.m_scOptions.Panel2.Controls.Add(Me.m_lbGroups)
            '
            'm_lblRuns
            '
            resources.ApplyResources(Me.m_lblRuns, "m_lblRuns")
            Me.m_lblRuns.BackColor = System.Drawing.SystemColors.ControlDark
            Me.m_lblRuns.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
            Me.m_lblRuns.Name = "m_lblRuns"
            '
            'm_lbRuns
            '
            resources.ApplyResources(Me.m_lbRuns, "m_lbRuns")
            Me.m_lbRuns.BackColor = System.Drawing.SystemColors.Window
            Me.m_lbRuns.FormattingEnabled = True
            Me.m_lbRuns.Name = "m_lbRuns"
            Me.m_lbRuns.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended
            '
            'm_lblGroups
            '
            resources.ApplyResources(Me.m_lblGroups, "m_lblGroups")
            Me.m_lblGroups.BackColor = System.Drawing.SystemColors.ControlDark
            Me.m_lblGroups.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
            Me.m_lblGroups.Name = "m_lblGroups"
            '
            'm_lbGroups
            '
            resources.ApplyResources(Me.m_lbGroups, "m_lbGroups")
            Me.m_lbGroups.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed
            Me.m_lbGroups.FormattingEnabled = True
            Me.m_lbGroups.GroupDisplayStyle = ScientificInterfaceShared.Controls.cGroupListBox.eGroupDisplayStyleTypes.DisplayAsHidden
            Me.m_lbGroups.GroupListTracking = ScientificInterfaceShared.Controls.cGroupListBox.eGroupTrackingType.AllGroups
            Me.m_lbGroups.Name = "m_lbGroups"
            Me.m_lbGroups.SelectedGroup = Nothing
            Me.m_lbGroups.SelectedGroupIndex = -1
            Me.m_lbGroups.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended
            Me.m_lbGroups.SortThreshold = -9999.0!
            Me.m_lbGroups.SortType = ScientificInterfaceShared.Controls.cGroupListBox.eSortType.GroupIndexAsc
            '
            'RunEcosim
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.m_spContainer)
            Me.Name = "RunEcosim"
            Me.m_tsMain.ResumeLayout(False)
            Me.m_tsMain.PerformLayout()
            Me.m_spContainer.Panel1.ResumeLayout(False)
            Me.m_spContainer.Panel2.ResumeLayout(False)
            Me.m_spContainer.Panel2.PerformLayout()
            Me.m_spContainer.ResumeLayout(False)
            Me.TableLayoutPanel1.ResumeLayout(False)
            Me.TableLayoutPanel1.PerformLayout()
            Me.m_ts.ResumeLayout(False)
            Me.m_ts.PerformLayout()
            Me.m_scGraph.Panel1.ResumeLayout(False)
            Me.m_scGraph.Panel2.ResumeLayout(False)
            Me.m_scGraph.ResumeLayout(False)
            Me.m_scOptions.Panel1.ResumeLayout(False)
            Me.m_scOptions.Panel2.ResumeLayout(False)
            Me.m_scOptions.ResumeLayout(False)
            Me.ResumeLayout(False)

        End Sub

        Private WithEvents btnRunOrStop As System.Windows.Forms.Button
        Private WithEvents m_sketchPad As ucForcingSketchPad
        Private WithEvents m_tsMain As System.Windows.Forms.ToolStrip
        Private WithEvents tslTarget As System.Windows.Forms.ToolStripLabel
        Private WithEvents tscbTarget As cCustomToolstripComboBox
        Private WithEvents ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
        Private WithEvents tsbResetFs As System.Windows.Forms.ToolStripButton
        Private WithEvents tsbSetTo0 As System.Windows.Forms.ToolStripButton
        Private WithEvents tsbSetToValue As System.Windows.Forms.ToolStripButton
        Private WithEvents m_spContainer As System.Windows.Forms.SplitContainer
        Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
        Private WithEvents m_ts As System.Windows.Forms.ToolStrip
        Private WithEvents m_tsbtnShowHideGroups As System.Windows.Forms.ToolStripButton
        Private WithEvents tslblSSValue As System.Windows.Forms.ToolStripLabel
        Private WithEvents tsblbSS As System.Windows.Forms.ToolStripLabel
        Private WithEvents m_tsddPlotType As System.Windows.Forms.ToolStripDropDownButton
        Private WithEvents m_tsmiCumulative As System.Windows.Forms.ToolStripMenuItem
        Private WithEvents m_tsmiRelative As System.Windows.Forms.ToolStripMenuItem
        Private WithEvents m_tsddGraphOptions As System.Windows.Forms.ToolStripDropDownButton
        Private WithEvents m_tsmiAutoscale As System.Windows.Forms.ToolStripMenuItem
        Private WithEvents m_tsmiCustomScaleLabel As System.Windows.Forms.ToolStripMenuItem
        Private WithEvents m_tsmiMax As System.Windows.Forms.ToolStripMenuItem
        Private WithEvents m_tstbMax As System.Windows.Forms.ToolStripTextBox
        Private WithEvents m_tsmiMin As System.Windows.Forms.ToolStripMenuItem
        Private WithEvents m_tstbMin As System.Windows.Forms.ToolStripTextBox
        Private WithEvents m_tsmiShowAnnualOutput As System.Windows.Forms.ToolStripMenuItem
        Private WithEvents m_tsmShowMultipleRuns As System.Windows.Forms.ToolStripMenuItem
        Private WithEvents m_tsmiShowLegend As System.Windows.Forms.ToolStripMenuItem
        Private WithEvents m_graph As ZedGraph.ZedGraphControl
        Private WithEvents m_scOptions As System.Windows.Forms.SplitContainer
        Private WithEvents m_lbRuns As System.Windows.Forms.ListBox
        Private WithEvents m_lbGroups As ScientificInterfaceShared.Controls.cGroupListBox
        Private WithEvents m_scGraph As System.Windows.Forms.SplitContainer
        Friend WithEvents ToolStripSeparator3 As System.Windows.Forms.ToolStripSeparator
        Private WithEvents m_lblGroups As System.Windows.Forms.Label
        Private WithEvents m_lblRuns As System.Windows.Forms.Label
        Private WithEvents m_tssbExplore As System.Windows.Forms.ToolStripSplitButton
        Private WithEvents m_tsmiSortMostChanged As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents ChangeAmountToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
        Private WithEvents m_tstbChangeAmount As System.Windows.Forms.ToolStripTextBox
        Private WithEvents m_tsdrpdnbtnContent As System.Windows.Forms.ToolStripDropDownButton
        Private WithEvents m_tsmiBiomass As System.Windows.Forms.ToolStripMenuItem
        Private WithEvents m_tsmiCatch As System.Windows.Forms.ToolStripMenuItem

    End Class
End Namespace

