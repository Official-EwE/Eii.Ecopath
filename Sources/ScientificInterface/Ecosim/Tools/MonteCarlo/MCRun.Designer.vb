Imports WeifenLuo.WinFormsUI.Docking
Imports ZedGraph

Namespace Ecosim

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class MCRun
        Inherits frmEwE

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
            Me.components = New System.ComponentModel.Container
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(MCRun))
            Me.lblNumTrials = New System.Windows.Forms.Label
            Me.btnRunTrials = New System.Windows.Forms.Button
            Me.btnStop = New System.Windows.Forms.Button
            Me.m_tcOutput = New System.Windows.Forms.TabControl
            Me.m_tbpB = New System.Windows.Forms.TabPage
            Me.m_gridB = New ScientificInterface.Ecosim.MCRunInputGrid
            Me.m_tbpBP = New System.Windows.Forms.TabPage
            Me.m_gridPB = New ScientificInterface.Ecosim.MCRunInputGrid
            Me.m_tbpEE = New System.Windows.Forms.TabPage
            Me.m_gridEE = New ScientificInterface.Ecosim.MCRunInputGrid
            Me.m_tbpBA = New System.Windows.Forms.TabPage
            Me.m_gridBA = New ScientificInterface.Ecosim.MCRunInputGrid
            Me.m_tbpBPlot = New System.Windows.Forms.TabPage
            Me.m_spPlot = New System.Windows.Forms.SplitContainer
            Me.m_graph = New ZedGraph.ZedGraphControl
            Me.m_lbGroups = New ScientificInterfaceShared.Controls.cGroupListBox
            Me.m_lblGroups = New System.Windows.Forms.Label
            Me.m_tbpBestTrial = New System.Windows.Forms.TabPage
            Me.m_gridBestFit = New ScientificInterface.Ecosim.MCRunOutputGrid
            Me.cbPedigree = New System.Windows.Forms.CheckBox
            Me.cbRetainEstimates = New System.Windows.Forms.CheckBox
            Me.cbRetainCurPattern = New System.Windows.Forms.CheckBox
            Me.lblTrial = New System.Windows.Forms.Label
            Me.lblERun = New System.Windows.Forms.Label
            Me.lblSS = New System.Windows.Forms.Label
            Me.lblBestSS = New System.Windows.Forms.Label
            Me.cbShowBioTraj = New System.Windows.Forms.CheckBox
            Me.btApply = New System.Windows.Forms.Button
            Me.nudNumTrials = New System.Windows.Forms.NumericUpDown
            Me.btnTS = New System.Windows.Forms.Button
            Me.lblValueERun = New System.Windows.Forms.Label
            Me.lblValueSSBest = New System.Windows.Forms.Label
            Me.lblValueSS = New System.Windows.Forms.Label
            Me.lblValueSSOrg = New System.Windows.Forms.Label
            Me.lblValueTrial = New System.Windows.Forms.Label
            Me.lbSSOrg = New System.Windows.Forms.Label
            Me.m_hdrInputOpt = New cEwEHeaderLabel
            Me.m_hdrOutputParam = New cEwEHeaderLabel
            Me.m_tlpOutputs = New System.Windows.Forms.TableLayoutPanel
            Me.m_tcOutput.SuspendLayout()
            Me.m_tbpB.SuspendLayout()
            Me.m_tbpBP.SuspendLayout()
            Me.m_tbpEE.SuspendLayout()
            Me.m_tbpBA.SuspendLayout()
            Me.m_tbpBPlot.SuspendLayout()
            Me.m_spPlot.Panel1.SuspendLayout()
            Me.m_spPlot.Panel2.SuspendLayout()
            Me.m_spPlot.SuspendLayout()
            Me.m_tbpBestTrial.SuspendLayout()
            CType(Me.nudNumTrials, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.m_tlpOutputs.SuspendLayout()
            Me.SuspendLayout()
            '
            'lblNumTrials
            '
            resources.ApplyResources(Me.lblNumTrials, "lblNumTrials")
            Me.lblNumTrials.Name = "lblNumTrials"
            '
            'btnRunTrials
            '
            resources.ApplyResources(Me.btnRunTrials, "btnRunTrials")
            Me.btnRunTrials.Name = "btnRunTrials"
            Me.btnRunTrials.UseVisualStyleBackColor = True
            '
            'btnStop
            '
            resources.ApplyResources(Me.btnStop, "btnStop")
            Me.btnStop.Name = "btnStop"
            Me.btnStop.UseVisualStyleBackColor = True
            '
            'm_tcOutput
            '
            resources.ApplyResources(Me.m_tcOutput, "m_tcOutput")
            Me.m_tcOutput.Controls.Add(Me.m_tbpB)
            Me.m_tcOutput.Controls.Add(Me.m_tbpBP)
            Me.m_tcOutput.Controls.Add(Me.m_tbpEE)
            Me.m_tcOutput.Controls.Add(Me.m_tbpBA)
            Me.m_tcOutput.Controls.Add(Me.m_tbpBPlot)
            Me.m_tcOutput.Controls.Add(Me.m_tbpBestTrial)
            Me.m_tcOutput.Name = "m_tcOutput"
            Me.m_tcOutput.SelectedIndex = 0
            '
            'm_tbpB
            '
            Me.m_tbpB.Controls.Add(Me.m_gridB)
            resources.ApplyResources(Me.m_tbpB, "m_tbpB")
            Me.m_tbpB.Name = "m_tbpB"
            Me.m_tbpB.UseVisualStyleBackColor = True
            '
            'm_gridB
            '
            Me.m_gridB.AutoSizeMinHeight = 10
            Me.m_gridB.AutoSizeMinWidth = 10
            Me.m_gridB.AutoStretchColumnsToFitWidth = False
            Me.m_gridB.AutoStretchRowsToFitHeight = False
            Me.m_gridB.BackColor = System.Drawing.Color.White
            Me.m_gridB.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.m_gridB.ContextMenuStyle = CType((((SourceGrid2.ContextMenuStyle.ColumnResize Or SourceGrid2.ContextMenuStyle.AutoSize) _
                        Or SourceGrid2.ContextMenuStyle.CopyPasteSelection) _
                        Or SourceGrid2.ContextMenuStyle.CellContextMenu), SourceGrid2.ContextMenuStyle)
            Me.m_gridB.CustomSort = False
            Me.m_gridB.DisplayInputValue = ScientificInterfaceShared.Definitions.eMCRunDisplayInputValueTypes.B
            resources.ApplyResources(Me.m_gridB, "m_gridB")
            Me.m_gridB.FixedColumnWidths = False
            Me.m_gridB.FocusStyle = SourceGrid2.FocusStyle.None
            Me.m_gridB.GridToolTipActive = True
            Me.m_gridB.Name = "m_gridB"
            Me.m_gridB.SpecialKeys = CType((((((((((SourceGrid2.GridSpecialKeys.Ctrl_C Or SourceGrid2.GridSpecialKeys.Ctrl_V) _
                        Or SourceGrid2.GridSpecialKeys.Ctrl_X) _
                        Or SourceGrid2.GridSpecialKeys.Delete) _
                        Or SourceGrid2.GridSpecialKeys.Arrows) _
                        Or SourceGrid2.GridSpecialKeys.Tab) _
                        Or SourceGrid2.GridSpecialKeys.PageDownUp) _
                        Or SourceGrid2.GridSpecialKeys.Enter) _
                        Or SourceGrid2.GridSpecialKeys.Escape) _
                        Or SourceGrid2.GridSpecialKeys.Backspace), SourceGrid2.GridSpecialKeys)
            Me.m_gridB.TrackPropertySelection = True
            Me.m_gridB.UIContext = Nothing
            '
            'm_tbpBP
            '
            Me.m_tbpBP.Controls.Add(Me.m_gridPB)
            resources.ApplyResources(Me.m_tbpBP, "m_tbpBP")
            Me.m_tbpBP.Name = "m_tbpBP"
            Me.m_tbpBP.UseVisualStyleBackColor = True
            '
            'm_gridPB
            '
            Me.m_gridPB.AutoSizeMinHeight = 10
            Me.m_gridPB.AutoSizeMinWidth = 10
            Me.m_gridPB.AutoStretchColumnsToFitWidth = False
            Me.m_gridPB.AutoStretchRowsToFitHeight = False
            Me.m_gridPB.BackColor = System.Drawing.Color.White
            Me.m_gridPB.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.m_gridPB.ContextMenuStyle = CType((((SourceGrid2.ContextMenuStyle.ColumnResize Or SourceGrid2.ContextMenuStyle.AutoSize) _
                        Or SourceGrid2.ContextMenuStyle.CopyPasteSelection) _
                        Or SourceGrid2.ContextMenuStyle.CellContextMenu), SourceGrid2.ContextMenuStyle)
            Me.m_gridPB.CustomSort = False
            Me.m_gridPB.DisplayInputValue = ScientificInterfaceShared.Definitions.eMCRunDisplayInputValueTypes.PB
            resources.ApplyResources(Me.m_gridPB, "m_gridPB")
            Me.m_gridPB.FixedColumnWidths = False
            Me.m_gridPB.FocusStyle = SourceGrid2.FocusStyle.None
            Me.m_gridPB.GridToolTipActive = True
            Me.m_gridPB.Name = "m_gridPB"
            Me.m_gridPB.SpecialKeys = CType((((((((((SourceGrid2.GridSpecialKeys.Ctrl_C Or SourceGrid2.GridSpecialKeys.Ctrl_V) _
                        Or SourceGrid2.GridSpecialKeys.Ctrl_X) _
                        Or SourceGrid2.GridSpecialKeys.Delete) _
                        Or SourceGrid2.GridSpecialKeys.Arrows) _
                        Or SourceGrid2.GridSpecialKeys.Tab) _
                        Or SourceGrid2.GridSpecialKeys.PageDownUp) _
                        Or SourceGrid2.GridSpecialKeys.Enter) _
                        Or SourceGrid2.GridSpecialKeys.Escape) _
                        Or SourceGrid2.GridSpecialKeys.Backspace), SourceGrid2.GridSpecialKeys)
            Me.m_gridPB.TrackPropertySelection = True
            Me.m_gridPB.UIContext = Nothing
            '
            'm_tbpEE
            '
            Me.m_tbpEE.Controls.Add(Me.m_gridEE)
            resources.ApplyResources(Me.m_tbpEE, "m_tbpEE")
            Me.m_tbpEE.Name = "m_tbpEE"
            Me.m_tbpEE.UseVisualStyleBackColor = True
            '
            'm_gridEE
            '
            Me.m_gridEE.AutoSizeMinHeight = 10
            Me.m_gridEE.AutoSizeMinWidth = 10
            Me.m_gridEE.AutoStretchColumnsToFitWidth = False
            Me.m_gridEE.AutoStretchRowsToFitHeight = False
            Me.m_gridEE.BackColor = System.Drawing.Color.White
            Me.m_gridEE.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.m_gridEE.ContextMenuStyle = CType((((SourceGrid2.ContextMenuStyle.ColumnResize Or SourceGrid2.ContextMenuStyle.AutoSize) _
                        Or SourceGrid2.ContextMenuStyle.CopyPasteSelection) _
                        Or SourceGrid2.ContextMenuStyle.CellContextMenu), SourceGrid2.ContextMenuStyle)
            Me.m_gridEE.CustomSort = False
            Me.m_gridEE.DisplayInputValue = ScientificInterfaceShared.Definitions.eMCRunDisplayInputValueTypes.EE
            resources.ApplyResources(Me.m_gridEE, "m_gridEE")
            Me.m_gridEE.FixedColumnWidths = False
            Me.m_gridEE.FocusStyle = SourceGrid2.FocusStyle.None
            Me.m_gridEE.GridToolTipActive = True
            Me.m_gridEE.Name = "m_gridEE"
            Me.m_gridEE.SpecialKeys = CType((((((((((SourceGrid2.GridSpecialKeys.Ctrl_C Or SourceGrid2.GridSpecialKeys.Ctrl_V) _
                        Or SourceGrid2.GridSpecialKeys.Ctrl_X) _
                        Or SourceGrid2.GridSpecialKeys.Delete) _
                        Or SourceGrid2.GridSpecialKeys.Arrows) _
                        Or SourceGrid2.GridSpecialKeys.Tab) _
                        Or SourceGrid2.GridSpecialKeys.PageDownUp) _
                        Or SourceGrid2.GridSpecialKeys.Enter) _
                        Or SourceGrid2.GridSpecialKeys.Escape) _
                        Or SourceGrid2.GridSpecialKeys.Backspace), SourceGrid2.GridSpecialKeys)
            Me.m_gridEE.TrackPropertySelection = True
            Me.m_gridEE.UIContext = Nothing
            '
            'm_tbpBA
            '
            Me.m_tbpBA.Controls.Add(Me.m_gridBA)
            resources.ApplyResources(Me.m_tbpBA, "m_tbpBA")
            Me.m_tbpBA.Name = "m_tbpBA"
            Me.m_tbpBA.UseVisualStyleBackColor = True
            '
            'm_gridBA
            '
            Me.m_gridBA.AutoSizeMinHeight = 10
            Me.m_gridBA.AutoSizeMinWidth = 10
            Me.m_gridBA.AutoStretchColumnsToFitWidth = False
            Me.m_gridBA.AutoStretchRowsToFitHeight = False
            Me.m_gridBA.BackColor = System.Drawing.Color.White
            Me.m_gridBA.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.m_gridBA.ContextMenuStyle = CType((((SourceGrid2.ContextMenuStyle.ColumnResize Or SourceGrid2.ContextMenuStyle.AutoSize) _
                        Or SourceGrid2.ContextMenuStyle.CopyPasteSelection) _
                        Or SourceGrid2.ContextMenuStyle.CellContextMenu), SourceGrid2.ContextMenuStyle)
            Me.m_gridBA.CustomSort = False
            Me.m_gridBA.DisplayInputValue = ScientificInterfaceShared.Definitions.eMCRunDisplayInputValueTypes.BA
            resources.ApplyResources(Me.m_gridBA, "m_gridBA")
            Me.m_gridBA.FixedColumnWidths = False
            Me.m_gridBA.FocusStyle = SourceGrid2.FocusStyle.None
            Me.m_gridBA.GridToolTipActive = True
            Me.m_gridBA.Name = "m_gridBA"
            Me.m_gridBA.SpecialKeys = CType((((((((((SourceGrid2.GridSpecialKeys.Ctrl_C Or SourceGrid2.GridSpecialKeys.Ctrl_V) _
                        Or SourceGrid2.GridSpecialKeys.Ctrl_X) _
                        Or SourceGrid2.GridSpecialKeys.Delete) _
                        Or SourceGrid2.GridSpecialKeys.Arrows) _
                        Or SourceGrid2.GridSpecialKeys.Tab) _
                        Or SourceGrid2.GridSpecialKeys.PageDownUp) _
                        Or SourceGrid2.GridSpecialKeys.Enter) _
                        Or SourceGrid2.GridSpecialKeys.Escape) _
                        Or SourceGrid2.GridSpecialKeys.Backspace), SourceGrid2.GridSpecialKeys)
            Me.m_gridBA.TrackPropertySelection = True
            Me.m_gridBA.UIContext = Nothing
            '
            'm_tbpBPlot
            '
            Me.m_tbpBPlot.BackColor = System.Drawing.SystemColors.Control
            Me.m_tbpBPlot.Controls.Add(Me.m_spPlot)
            resources.ApplyResources(Me.m_tbpBPlot, "m_tbpBPlot")
            Me.m_tbpBPlot.Name = "m_tbpBPlot"
            Me.m_tbpBPlot.UseVisualStyleBackColor = True
            '
            'm_spPlot
            '
            resources.ApplyResources(Me.m_spPlot, "m_spPlot")
            Me.m_spPlot.Name = "m_spPlot"
            '
            'm_spPlot.Panel1
            '
            Me.m_spPlot.Panel1.Controls.Add(Me.m_graph)
            '
            'm_spPlot.Panel2
            '
            Me.m_spPlot.Panel2.Controls.Add(Me.m_lbGroups)
            Me.m_spPlot.Panel2.Controls.Add(Me.m_lblGroups)
            '
            'm_graph
            '
            Me.m_graph.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
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
            'm_lbGroups
            '
            Me.m_lbGroups.AllGroupsItemColor = System.Drawing.Color.Transparent
            Me.m_lbGroups.AllGroupsItemText = "(All)"
            resources.ApplyResources(Me.m_lbGroups, "m_lbGroups")
            Me.m_lbGroups.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed
            Me.m_lbGroups.FormattingEnabled = True
            Me.m_lbGroups.GroupDisplayStyle = ScientificInterfaceShared.Controls.cGroupListBox.eGroupDisplayStyleTypes.DisplayAlways
            Me.m_lbGroups.GroupListTracking = ScientificInterfaceShared.Controls.cGroupListBox.eGroupTrackingType.LivingGroups
            Me.m_lbGroups.Name = "m_lbGroups"
            Me.m_lbGroups.SelectedGroup = Nothing
            Me.m_lbGroups.SelectedGroupIndex = -1
            Me.m_lbGroups.SortThreshold = -9999.0!
            Me.m_lbGroups.SortType = ScientificInterfaceShared.Controls.cGroupListBox.eSortType.ValueAsc
            '
            'm_lblGroups
            '
            resources.ApplyResources(Me.m_lblGroups, "m_lblGroups")
            Me.m_lblGroups.BackColor = System.Drawing.SystemColors.ControlDark
            Me.m_lblGroups.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
            Me.m_lblGroups.Name = "m_lblGroups"
            '
            'm_tbpBestTrial
            '
            Me.m_tbpBestTrial.Controls.Add(Me.m_gridBestFit)
            resources.ApplyResources(Me.m_tbpBestTrial, "m_tbpBestTrial")
            Me.m_tbpBestTrial.Name = "m_tbpBestTrial"
            Me.m_tbpBestTrial.UseVisualStyleBackColor = True
            '
            'm_gridBestFit
            '
            Me.m_gridBestFit.AutoSizeMinHeight = 10
            Me.m_gridBestFit.AutoSizeMinWidth = 10
            Me.m_gridBestFit.AutoStretchColumnsToFitWidth = False
            Me.m_gridBestFit.AutoStretchRowsToFitHeight = False
            Me.m_gridBestFit.BackColor = System.Drawing.Color.White
            Me.m_gridBestFit.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.m_gridBestFit.ContextMenuStyle = CType((((SourceGrid2.ContextMenuStyle.ColumnResize Or SourceGrid2.ContextMenuStyle.AutoSize) _
                        Or SourceGrid2.ContextMenuStyle.CopyPasteSelection) _
                        Or SourceGrid2.ContextMenuStyle.CellContextMenu), SourceGrid2.ContextMenuStyle)
            Me.m_gridBestFit.CustomSort = False
            resources.ApplyResources(Me.m_gridBestFit, "m_gridBestFit")
            Me.m_gridBestFit.FixedColumnWidths = True
            Me.m_gridBestFit.FocusStyle = SourceGrid2.FocusStyle.None
            Me.m_gridBestFit.GridToolTipActive = True
            Me.m_gridBestFit.Name = "m_gridBestFit"
            Me.m_gridBestFit.SpecialKeys = CType((((((((((SourceGrid2.GridSpecialKeys.Ctrl_C Or SourceGrid2.GridSpecialKeys.Ctrl_V) _
                        Or SourceGrid2.GridSpecialKeys.Ctrl_X) _
                        Or SourceGrid2.GridSpecialKeys.Delete) _
                        Or SourceGrid2.GridSpecialKeys.Arrows) _
                        Or SourceGrid2.GridSpecialKeys.Tab) _
                        Or SourceGrid2.GridSpecialKeys.PageDownUp) _
                        Or SourceGrid2.GridSpecialKeys.Enter) _
                        Or SourceGrid2.GridSpecialKeys.Escape) _
                        Or SourceGrid2.GridSpecialKeys.Backspace), SourceGrid2.GridSpecialKeys)
            Me.m_gridBestFit.TrackPropertySelection = True
            Me.m_gridBestFit.UIContext = Nothing
            '
            'cbPedigree
            '
            resources.ApplyResources(Me.cbPedigree, "cbPedigree")
            Me.cbPedigree.Name = "cbPedigree"
            Me.cbPedigree.UseVisualStyleBackColor = True
            '
            'cbRetainEstimates
            '
            resources.ApplyResources(Me.cbRetainEstimates, "cbRetainEstimates")
            Me.cbRetainEstimates.Name = "cbRetainEstimates"
            Me.cbRetainEstimates.UseVisualStyleBackColor = True
            '
            'cbRetainCurPattern
            '
            resources.ApplyResources(Me.cbRetainCurPattern, "cbRetainCurPattern")
            Me.cbRetainCurPattern.Name = "cbRetainCurPattern"
            Me.cbRetainCurPattern.UseVisualStyleBackColor = True
            '
            'lblTrial
            '
            resources.ApplyResources(Me.lblTrial, "lblTrial")
            Me.lblTrial.Name = "lblTrial"
            '
            'lblERun
            '
            resources.ApplyResources(Me.lblERun, "lblERun")
            Me.lblERun.Name = "lblERun"
            '
            'lblSS
            '
            resources.ApplyResources(Me.lblSS, "lblSS")
            Me.lblSS.Name = "lblSS"
            '
            'lblBestSS
            '
            resources.ApplyResources(Me.lblBestSS, "lblBestSS")
            Me.lblBestSS.Name = "lblBestSS"
            '
            'cbShowBioTraj
            '
            resources.ApplyResources(Me.cbShowBioTraj, "cbShowBioTraj")
            Me.cbShowBioTraj.Checked = True
            Me.cbShowBioTraj.CheckState = System.Windows.Forms.CheckState.Checked
            Me.cbShowBioTraj.Name = "cbShowBioTraj"
            Me.cbShowBioTraj.UseVisualStyleBackColor = True
            '
            'btApply
            '
            resources.ApplyResources(Me.btApply, "btApply")
            Me.btApply.Name = "btApply"
            Me.btApply.UseVisualStyleBackColor = True
            '
            'nudNumTrials
            '
            resources.ApplyResources(Me.nudNumTrials, "nudNumTrials")
            Me.nudNumTrials.Maximum = New Decimal(New Integer() {2147483647, 0, 0, 0})
            Me.nudNumTrials.Name = "nudNumTrials"
            '
            'btnTS
            '
            resources.ApplyResources(Me.btnTS, "btnTS")
            Me.btnTS.Name = "btnTS"
            Me.btnTS.UseVisualStyleBackColor = True
            '
            'lblValueERun
            '
            resources.ApplyResources(Me.lblValueERun, "lblValueERun")
            Me.lblValueERun.Name = "lblValueERun"
            '
            'lblValueSSBest
            '
            resources.ApplyResources(Me.lblValueSSBest, "lblValueSSBest")
            Me.lblValueSSBest.Name = "lblValueSSBest"
            '
            'lblValueSS
            '
            resources.ApplyResources(Me.lblValueSS, "lblValueSS")
            Me.lblValueSS.Name = "lblValueSS"
            '
            'lblValueSSOrg
            '
            resources.ApplyResources(Me.lblValueSSOrg, "lblValueSSOrg")
            Me.lblValueSSOrg.Name = "lblValueSSOrg"
            '
            'lblValueTrial
            '
            resources.ApplyResources(Me.lblValueTrial, "lblValueTrial")
            Me.lblValueTrial.Name = "lblValueTrial"
            '
            'lbSSOrg
            '
            resources.ApplyResources(Me.lbSSOrg, "lbSSOrg")
            Me.lbSSOrg.Name = "lbSSOrg"
            '
            'm_hdrInputOpt
            '
            resources.ApplyResources(Me.m_hdrInputOpt, "m_hdrInputOpt")
            Me.m_hdrInputOpt.Name = "m_hdrInputOpt"
            '
            'm_hdrOutputParam
            '
            resources.ApplyResources(Me.m_hdrOutputParam, "m_hdrOutputParam")
            Me.m_hdrOutputParam.Name = "m_hdrOutputParam"
            '
            'm_tlpOutputs
            '
            resources.ApplyResources(Me.m_tlpOutputs, "m_tlpOutputs")
            Me.m_tlpOutputs.Controls.Add(Me.lblTrial, 0, 0)
            Me.m_tlpOutputs.Controls.Add(Me.lblERun, 0, 1)
            Me.m_tlpOutputs.Controls.Add(Me.lblValueTrial, 1, 0)
            Me.m_tlpOutputs.Controls.Add(Me.lblValueERun, 1, 1)
            Me.m_tlpOutputs.Controls.Add(Me.lblValueSSBest, 4, 2)
            Me.m_tlpOutputs.Controls.Add(Me.lbSSOrg, 3, 0)
            Me.m_tlpOutputs.Controls.Add(Me.lblValueSS, 4, 1)
            Me.m_tlpOutputs.Controls.Add(Me.lblSS, 3, 1)
            Me.m_tlpOutputs.Controls.Add(Me.lblBestSS, 3, 2)
            Me.m_tlpOutputs.Controls.Add(Me.lblValueSSOrg, 4, 0)
            Me.m_tlpOutputs.Name = "m_tlpOutputs"
            '
            'MCRun
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.m_tlpOutputs)
            Me.Controls.Add(Me.m_hdrOutputParam)
            Me.Controls.Add(Me.m_hdrInputOpt)
            Me.Controls.Add(Me.nudNumTrials)
            Me.Controls.Add(Me.btnTS)
            Me.Controls.Add(Me.lblNumTrials)
            Me.Controls.Add(Me.cbPedigree)
            Me.Controls.Add(Me.m_tcOutput)
            Me.Controls.Add(Me.cbRetainEstimates)
            Me.Controls.Add(Me.btApply)
            Me.Controls.Add(Me.cbRetainCurPattern)
            Me.Controls.Add(Me.cbShowBioTraj)
            Me.Controls.Add(Me.btnStop)
            Me.Controls.Add(Me.btnRunTrials)
            Me.Name = "MCRun"
            Me.TabText = "Monte Carlo simulation of varying Ecopath basic parameters"
            Me.m_tcOutput.ResumeLayout(False)
            Me.m_tbpB.ResumeLayout(False)
            Me.m_tbpBP.ResumeLayout(False)
            Me.m_tbpEE.ResumeLayout(False)
            Me.m_tbpBA.ResumeLayout(False)
            Me.m_tbpBPlot.ResumeLayout(False)
            Me.m_spPlot.Panel1.ResumeLayout(False)
            Me.m_spPlot.Panel2.ResumeLayout(False)
            Me.m_spPlot.ResumeLayout(False)
            Me.m_tbpBestTrial.ResumeLayout(False)
            CType(Me.nudNumTrials, System.ComponentModel.ISupportInitialize).EndInit()
            Me.m_tlpOutputs.ResumeLayout(False)
            Me.m_tlpOutputs.PerformLayout()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents lblNumTrials As System.Windows.Forms.Label
        Private WithEvents btnRunTrials As System.Windows.Forms.Button
        Private WithEvents btnStop As System.Windows.Forms.Button
        Private WithEvents cbPedigree As System.Windows.Forms.CheckBox
        Private WithEvents cbRetainEstimates As System.Windows.Forms.CheckBox
        Private WithEvents cbRetainCurPattern As System.Windows.Forms.CheckBox
        Private WithEvents cbShowBioTraj As System.Windows.Forms.CheckBox
        Private WithEvents btApply As System.Windows.Forms.Button
        Private WithEvents btnTS As System.Windows.Forms.Button
        Private WithEvents nudNumTrials As System.Windows.Forms.NumericUpDown
        Private WithEvents m_hdrInputOpt As cEwEHeaderLabel
        Private WithEvents m_hdrOutputParam As cEwEHeaderLabel
        Private WithEvents m_tcOutput As System.Windows.Forms.TabControl
        Private WithEvents lblValueERun As System.Windows.Forms.Label
        Private WithEvents lblValueSSBest As System.Windows.Forms.Label
        Private WithEvents lblValueSS As System.Windows.Forms.Label
        Private WithEvents lblValueSSOrg As System.Windows.Forms.Label
        Private WithEvents lblValueTrial As System.Windows.Forms.Label
        Private WithEvents lblTrial As System.Windows.Forms.Label
        Private WithEvents lblERun As System.Windows.Forms.Label
        Private WithEvents lblSS As System.Windows.Forms.Label
        Private WithEvents lblBestSS As System.Windows.Forms.Label
        Private WithEvents lbSSOrg As System.Windows.Forms.Label
        Private WithEvents m_tlpOutputs As System.Windows.Forms.TableLayoutPanel
        Private WithEvents m_gridB As ScientificInterface.Ecosim.MCRunInputGrid
        Private WithEvents m_gridPB As ScientificInterface.Ecosim.MCRunInputGrid
        Private WithEvents m_gridEE As ScientificInterface.Ecosim.MCRunInputGrid
        Private WithEvents m_gridBA As ScientificInterface.Ecosim.MCRunInputGrid
        Private WithEvents m_gridBestFit As ScientificInterface.Ecosim.MCRunOutputGrid
        Private WithEvents m_tbpBPlot As System.Windows.Forms.TabPage
        Private WithEvents m_tbpBestTrial As System.Windows.Forms.TabPage
        Private WithEvents m_tbpBA As System.Windows.Forms.TabPage
        Private WithEvents m_tbpEE As System.Windows.Forms.TabPage
        Private WithEvents m_tbpBP As System.Windows.Forms.TabPage
        Private WithEvents m_tbpB As System.Windows.Forms.TabPage
        Private WithEvents m_spPlot As System.Windows.Forms.SplitContainer
        Private WithEvents m_graph As ZedGraphControl
        Private WithEvents m_lbGroups As ScientificInterfaceShared.Controls.cGroupListBox
        Private WithEvents m_lblGroups As System.Windows.Forms.Label
    End Class

End Namespace

