' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 2 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see <http://www.gnu.org/licenses/gpl-2.0.html>. 
'
' Copyright 1991- 
'    UBC Institute for the Oceans and Fisheries, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

Imports ScientificInterfaceShared.Forms
Imports ZedGraph

Namespace Ecosim

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
            Me.components = New System.ComponentModel.Container()
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(MCRun))
            Me.m_lblNumTrials = New System.Windows.Forms.Label()
            Me.m_btnRunTrials = New System.Windows.Forms.Button()
            Me.m_btnStop = New System.Windows.Forms.Button()
            Me.m_tcMain = New System.Windows.Forms.TabControl()
            Me.m_tbpSettings = New System.Windows.Forms.TabPage()
            Me.m_cbRetainEstimates = New System.Windows.Forms.CheckBox()
            Me.m_btDefaultTol = New System.Windows.Forms.Button()
            Me.m_cbSRA = New System.Windows.Forms.CheckBox()
            Me.m_cbRetainCurPattern = New System.Windows.Forms.CheckBox()
            Me.m_lblFMratio = New System.Windows.Forms.Label()
            Me.m_lblEEtol = New System.Windows.Forms.Label()
            Me.m_cbShowBioTraj = New System.Windows.Forms.CheckBox()
            Me.m_tbxFMratio = New System.Windows.Forms.TextBox()
            Me.m_tbxEETol = New System.Windows.Forms.TextBox()
            Me.m_tbpB = New System.Windows.Forms.TabPage()
            Me.m_gridB = New ScientificInterface.Ecosim.gridMCRunInput()
            Me.m_tsB = New ScientificInterfaceShared.Controls.cEwEToolstrip()
            Me.m_tsbnLoadPedB = New System.Windows.Forms.ToolStripButton()
            Me.m_tbpBP = New System.Windows.Forms.TabPage()
            Me.m_gridPB = New ScientificInterface.Ecosim.gridMCRunInput()
            Me.m_tsPB = New ScientificInterfaceShared.Controls.cEwEToolstrip()
            Me.m_tsbnLoadPedPB = New System.Windows.Forms.ToolStripButton()
            Me.m_tbpQB = New System.Windows.Forms.TabPage()
            Me.m_gridQB = New ScientificInterface.Ecosim.gridMCRunInput()
            Me.m_tsQB = New ScientificInterfaceShared.Controls.cEwEToolstrip()
            Me.m_tsbnLoadPedQB = New System.Windows.Forms.ToolStripButton()
            Me.m_tbpEE = New System.Windows.Forms.TabPage()
            Me.m_gridEE = New ScientificInterface.Ecosim.gridMCRunInput()
            Me.m_tsEE = New ScientificInterfaceShared.Controls.cEwEToolstrip()
            Me.m_tbpBA = New System.Windows.Forms.TabPage()
            Me.m_gridBA = New ScientificInterface.Ecosim.gridMCRunInput()
            Me.m_tsBA = New ScientificInterfaceShared.Controls.cEwEToolstrip()
            Me.m_tbpBPlot = New System.Windows.Forms.TabPage()
            Me.m_spPlot = New System.Windows.Forms.SplitContainer()
            Me.m_graph = New ZedGraph.ZedGraphControl()
            Me.m_tsPlot = New ScientificInterfaceShared.Controls.cEwEToolstrip()
            Me.m_tsbnShowBestOnly = New System.Windows.Forms.ToolStripButton()
            Me.m_tsbnShowGroups = New System.Windows.Forms.ToolStripButton()
            Me.m_lbGroups = New ScientificInterfaceShared.Controls.cGroupListBox()
            Me.m_lblGroups = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
            Me.m_tbpBestTrial = New System.Windows.Forms.TabPage()
            Me.m_gridBestFit = New ScientificInterface.Ecosim.gridMCRunOutput()
            Me.m_cbSave = New System.Windows.Forms.CheckBox()
            Me.lblTrial = New System.Windows.Forms.Label()
            Me.lblERun = New System.Windows.Forms.Label()
            Me.lblSS = New System.Windows.Forms.Label()
            Me.lblBestSS = New System.Windows.Forms.Label()
            Me.m_btnApply = New System.Windows.Forms.Button()
            Me.m_nudNumTrials = New System.Windows.Forms.NumericUpDown()
            Me.m_btnTS = New System.Windows.Forms.Button()
            Me.lblValueERun = New System.Windows.Forms.Label()
            Me.lblValueSSBest = New System.Windows.Forms.Label()
            Me.lblValueSS = New System.Windows.Forms.Label()
            Me.lblValueSSOrg = New System.Windows.Forms.Label()
            Me.lblValueTrial = New System.Windows.Forms.Label()
            Me.lbSSOrg = New System.Windows.Forms.Label()
            Me.m_hdrInputOpt = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
            Me.m_hdrOutputParam = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
            Me.m_tlpOutputs = New System.Windows.Forms.TableLayoutPanel()
            Me.m_tcMain.SuspendLayout()
            Me.m_tbpSettings.SuspendLayout()
            Me.m_tbpB.SuspendLayout()
            Me.m_tsB.SuspendLayout()
            Me.m_tbpBP.SuspendLayout()
            Me.m_tsPB.SuspendLayout()
            Me.m_tbpQB.SuspendLayout()
            Me.m_tsQB.SuspendLayout()
            Me.m_tbpEE.SuspendLayout()
            Me.m_tbpBA.SuspendLayout()
            Me.m_tbpBPlot.SuspendLayout()
            CType(Me.m_spPlot, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.m_spPlot.Panel1.SuspendLayout()
            Me.m_spPlot.Panel2.SuspendLayout()
            Me.m_spPlot.SuspendLayout()
            Me.m_tsPlot.SuspendLayout()
            Me.m_tbpBestTrial.SuspendLayout()
            CType(Me.m_nudNumTrials, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.m_tlpOutputs.SuspendLayout()
            Me.SuspendLayout()
            '
            'm_lblNumTrials
            '
            resources.ApplyResources(Me.m_lblNumTrials, "m_lblNumTrials")
            Me.m_lblNumTrials.Name = "m_lblNumTrials"
            '
            'm_btnRunTrials
            '
            resources.ApplyResources(Me.m_btnRunTrials, "m_btnRunTrials")
            Me.m_btnRunTrials.Name = "m_btnRunTrials"
            Me.m_btnRunTrials.UseVisualStyleBackColor = True
            '
            'm_btnStop
            '
            resources.ApplyResources(Me.m_btnStop, "m_btnStop")
            Me.m_btnStop.DialogResult = System.Windows.Forms.DialogResult.Cancel
            Me.m_btnStop.Name = "m_btnStop"
            Me.m_btnStop.UseVisualStyleBackColor = True
            '
            'm_tcMain
            '
            resources.ApplyResources(Me.m_tcMain, "m_tcMain")
            Me.m_tcMain.Controls.Add(Me.m_tbpSettings)
            Me.m_tcMain.Controls.Add(Me.m_tbpB)
            Me.m_tcMain.Controls.Add(Me.m_tbpBP)
            Me.m_tcMain.Controls.Add(Me.m_tbpQB)
            Me.m_tcMain.Controls.Add(Me.m_tbpEE)
            Me.m_tcMain.Controls.Add(Me.m_tbpBA)
            Me.m_tcMain.Controls.Add(Me.m_tbpBPlot)
            Me.m_tcMain.Controls.Add(Me.m_tbpBestTrial)
            Me.m_tcMain.Name = "m_tcMain"
            Me.m_tcMain.SelectedIndex = 0
            '
            'm_tbpSettings
            '
            Me.m_tbpSettings.Controls.Add(Me.m_cbRetainEstimates)
            Me.m_tbpSettings.Controls.Add(Me.m_btDefaultTol)
            Me.m_tbpSettings.Controls.Add(Me.m_cbSRA)
            Me.m_tbpSettings.Controls.Add(Me.m_cbRetainCurPattern)
            Me.m_tbpSettings.Controls.Add(Me.m_lblFMratio)
            Me.m_tbpSettings.Controls.Add(Me.m_lblEEtol)
            Me.m_tbpSettings.Controls.Add(Me.m_cbShowBioTraj)
            Me.m_tbpSettings.Controls.Add(Me.m_tbxFMratio)
            Me.m_tbpSettings.Controls.Add(Me.m_tbxEETol)
            resources.ApplyResources(Me.m_tbpSettings, "m_tbpSettings")
            Me.m_tbpSettings.Name = "m_tbpSettings"
            Me.m_tbpSettings.UseVisualStyleBackColor = True
            '
            'm_cbRetainEstimates
            '
            resources.ApplyResources(Me.m_cbRetainEstimates, "m_cbRetainEstimates")
            Me.m_cbRetainEstimates.Name = "m_cbRetainEstimates"
            Me.m_cbRetainEstimates.UseVisualStyleBackColor = True
            '
            'm_btDefaultTol
            '
            resources.ApplyResources(Me.m_btDefaultTol, "m_btDefaultTol")
            Me.m_btDefaultTol.Name = "m_btDefaultTol"
            Me.m_btDefaultTol.UseVisualStyleBackColor = True
            '
            'm_cbSRA
            '
            resources.ApplyResources(Me.m_cbSRA, "m_cbSRA")
            Me.m_cbSRA.Name = "m_cbSRA"
            Me.m_cbSRA.UseVisualStyleBackColor = True
            '
            'm_cbRetainCurPattern
            '
            resources.ApplyResources(Me.m_cbRetainCurPattern, "m_cbRetainCurPattern")
            Me.m_cbRetainCurPattern.Name = "m_cbRetainCurPattern"
            Me.m_cbRetainCurPattern.UseVisualStyleBackColor = True
            '
            'm_lblFMratio
            '
            resources.ApplyResources(Me.m_lblFMratio, "m_lblFMratio")
            Me.m_lblFMratio.Name = "m_lblFMratio"
            '
            'm_lblEEtol
            '
            resources.ApplyResources(Me.m_lblEEtol, "m_lblEEtol")
            Me.m_lblEEtol.Name = "m_lblEEtol"
            '
            'm_cbShowBioTraj
            '
            resources.ApplyResources(Me.m_cbShowBioTraj, "m_cbShowBioTraj")
            Me.m_cbShowBioTraj.Checked = True
            Me.m_cbShowBioTraj.CheckState = System.Windows.Forms.CheckState.Checked
            Me.m_cbShowBioTraj.Name = "m_cbShowBioTraj"
            Me.m_cbShowBioTraj.UseVisualStyleBackColor = True
            '
            'm_tbxFMratio
            '
            resources.ApplyResources(Me.m_tbxFMratio, "m_tbxFMratio")
            Me.m_tbxFMratio.Name = "m_tbxFMratio"
            '
            'm_tbxEETol
            '
            resources.ApplyResources(Me.m_tbxEETol, "m_tbxEETol")
            Me.m_tbxEETol.Name = "m_tbxEETol"
            '
            'm_tbpB
            '
            Me.m_tbpB.Controls.Add(Me.m_gridB)
            Me.m_tbpB.Controls.Add(Me.m_tsB)
            resources.ApplyResources(Me.m_tbpB, "m_tbpB")
            Me.m_tbpB.Name = "m_tbpB"
            Me.m_tbpB.UseVisualStyleBackColor = True
            '
            'm_gridB
            '
            Me.m_gridB.AllowBlockSelect = True
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
            Me.m_gridB.UIContext = Nothing
            '
            'm_tsB
            '
            Me.m_tsB.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
            Me.m_tsB.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tsbnLoadPedB})
            resources.ApplyResources(Me.m_tsB, "m_tsB")
            Me.m_tsB.Name = "m_tsB"
            Me.m_tsB.RenderMode = System.Windows.Forms.ToolStripRenderMode.System
            '
            'm_tsbnLoadPedB
            '
            resources.ApplyResources(Me.m_tsbnLoadPedB, "m_tsbnLoadPedB")
            Me.m_tsbnLoadPedB.Name = "m_tsbnLoadPedB"
            '
            'm_tbpBP
            '
            Me.m_tbpBP.Controls.Add(Me.m_gridPB)
            Me.m_tbpBP.Controls.Add(Me.m_tsPB)
            resources.ApplyResources(Me.m_tbpBP, "m_tbpBP")
            Me.m_tbpBP.Name = "m_tbpBP"
            Me.m_tbpBP.UseVisualStyleBackColor = True
            '
            'm_gridPB
            '
            Me.m_gridPB.AllowBlockSelect = True
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
            Me.m_gridPB.UIContext = Nothing
            '
            'm_tsPB
            '
            Me.m_tsPB.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
            Me.m_tsPB.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tsbnLoadPedPB})
            resources.ApplyResources(Me.m_tsPB, "m_tsPB")
            Me.m_tsPB.Name = "m_tsPB"
            Me.m_tsPB.RenderMode = System.Windows.Forms.ToolStripRenderMode.System
            '
            'm_tsbnLoadPedPB
            '
            resources.ApplyResources(Me.m_tsbnLoadPedPB, "m_tsbnLoadPedPB")
            Me.m_tsbnLoadPedPB.Name = "m_tsbnLoadPedPB"
            '
            'm_tbpQB
            '
            Me.m_tbpQB.Controls.Add(Me.m_gridQB)
            Me.m_tbpQB.Controls.Add(Me.m_tsQB)
            resources.ApplyResources(Me.m_tbpQB, "m_tbpQB")
            Me.m_tbpQB.Name = "m_tbpQB"
            Me.m_tbpQB.UseVisualStyleBackColor = True
            '
            'm_gridQB
            '
            Me.m_gridQB.AllowBlockSelect = True
            Me.m_gridQB.AutoSizeMinHeight = 10
            Me.m_gridQB.AutoSizeMinWidth = 10
            Me.m_gridQB.AutoStretchColumnsToFitWidth = False
            Me.m_gridQB.AutoStretchRowsToFitHeight = False
            Me.m_gridQB.BackColor = System.Drawing.Color.White
            Me.m_gridQB.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.m_gridQB.ContextMenuStyle = CType((((SourceGrid2.ContextMenuStyle.ColumnResize Or SourceGrid2.ContextMenuStyle.AutoSize) _
                Or SourceGrid2.ContextMenuStyle.CopyPasteSelection) _
                Or SourceGrid2.ContextMenuStyle.CellContextMenu), SourceGrid2.ContextMenuStyle)
            Me.m_gridQB.CustomSort = False
            Me.m_gridQB.DisplayInputValue = ScientificInterfaceShared.Definitions.eMCRunDisplayInputValueTypes.QB
            resources.ApplyResources(Me.m_gridQB, "m_gridQB")
            Me.m_gridQB.FixedColumnWidths = False
            Me.m_gridQB.FocusStyle = SourceGrid2.FocusStyle.None
            Me.m_gridQB.GridToolTipActive = True
            Me.m_gridQB.Name = "m_gridQB"
            Me.m_gridQB.SpecialKeys = CType((((((((((SourceGrid2.GridSpecialKeys.Ctrl_C Or SourceGrid2.GridSpecialKeys.Ctrl_V) _
                Or SourceGrid2.GridSpecialKeys.Ctrl_X) _
                Or SourceGrid2.GridSpecialKeys.Delete) _
                Or SourceGrid2.GridSpecialKeys.Arrows) _
                Or SourceGrid2.GridSpecialKeys.Tab) _
                Or SourceGrid2.GridSpecialKeys.PageDownUp) _
                Or SourceGrid2.GridSpecialKeys.Enter) _
                Or SourceGrid2.GridSpecialKeys.Escape) _
                Or SourceGrid2.GridSpecialKeys.Backspace), SourceGrid2.GridSpecialKeys)
            Me.m_gridQB.UIContext = Nothing
            '
            'm_tsQB
            '
            Me.m_tsQB.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
            Me.m_tsQB.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tsbnLoadPedQB})
            resources.ApplyResources(Me.m_tsQB, "m_tsQB")
            Me.m_tsQB.Name = "m_tsQB"
            Me.m_tsQB.RenderMode = System.Windows.Forms.ToolStripRenderMode.System
            '
            'm_tsbnLoadPedQB
            '
            resources.ApplyResources(Me.m_tsbnLoadPedQB, "m_tsbnLoadPedQB")
            Me.m_tsbnLoadPedQB.Name = "m_tsbnLoadPedQB"
            '
            'm_tbpEE
            '
            Me.m_tbpEE.Controls.Add(Me.m_gridEE)
            Me.m_tbpEE.Controls.Add(Me.m_tsEE)
            resources.ApplyResources(Me.m_tbpEE, "m_tbpEE")
            Me.m_tbpEE.Name = "m_tbpEE"
            Me.m_tbpEE.UseVisualStyleBackColor = True
            '
            'm_gridEE
            '
            Me.m_gridEE.AllowBlockSelect = True
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
            Me.m_gridEE.UIContext = Nothing
            '
            'm_tsEE
            '
            Me.m_tsEE.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
            resources.ApplyResources(Me.m_tsEE, "m_tsEE")
            Me.m_tsEE.Name = "m_tsEE"
            Me.m_tsEE.RenderMode = System.Windows.Forms.ToolStripRenderMode.System
            '
            'm_tbpBA
            '
            Me.m_tbpBA.Controls.Add(Me.m_gridBA)
            Me.m_tbpBA.Controls.Add(Me.m_tsBA)
            resources.ApplyResources(Me.m_tbpBA, "m_tbpBA")
            Me.m_tbpBA.Name = "m_tbpBA"
            Me.m_tbpBA.UseVisualStyleBackColor = True
            '
            'm_gridBA
            '
            Me.m_gridBA.AllowBlockSelect = True
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
            Me.m_gridBA.UIContext = Nothing
            '
            'm_tsBA
            '
            Me.m_tsBA.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
            resources.ApplyResources(Me.m_tsBA, "m_tsBA")
            Me.m_tsBA.Name = "m_tsBA"
            Me.m_tsBA.RenderMode = System.Windows.Forms.ToolStripRenderMode.System
            '
            'm_tbpBPlot
            '
            Me.m_tbpBPlot.BackColor = System.Drawing.Color.Transparent
            Me.m_tbpBPlot.Controls.Add(Me.m_spPlot)
            resources.ApplyResources(Me.m_tbpBPlot, "m_tbpBPlot")
            Me.m_tbpBPlot.Name = "m_tbpBPlot"
            Me.m_tbpBPlot.UseVisualStyleBackColor = True
            '
            'm_spPlot
            '
            Me.m_spPlot.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            resources.ApplyResources(Me.m_spPlot, "m_spPlot")
            Me.m_spPlot.FixedPanel = System.Windows.Forms.FixedPanel.Panel2
            Me.m_spPlot.Name = "m_spPlot"
            '
            'm_spPlot.Panel1
            '
            Me.m_spPlot.Panel1.Controls.Add(Me.m_graph)
            Me.m_spPlot.Panel1.Controls.Add(Me.m_tsPlot)
            '
            'm_spPlot.Panel2
            '
            Me.m_spPlot.Panel2.Controls.Add(Me.m_lbGroups)
            Me.m_spPlot.Panel2.Controls.Add(Me.m_lblGroups)
            '
            'm_graph
            '
            resources.ApplyResources(Me.m_graph, "m_graph")
            Me.m_graph.Name = "m_graph"
            Me.m_graph.ScrollGrace = 0.0R
            Me.m_graph.ScrollMaxX = 0.0R
            Me.m_graph.ScrollMaxY = 0.0R
            Me.m_graph.ScrollMaxY2 = 0.0R
            Me.m_graph.ScrollMinX = 0.0R
            Me.m_graph.ScrollMinY = 0.0R
            Me.m_graph.ScrollMinY2 = 0.0R
            '
            'm_tsPlot
            '
            Me.m_tsPlot.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
            Me.m_tsPlot.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tsbnShowBestOnly, Me.m_tsbnShowGroups})
            resources.ApplyResources(Me.m_tsPlot, "m_tsPlot")
            Me.m_tsPlot.Name = "m_tsPlot"
            Me.m_tsPlot.RenderMode = System.Windows.Forms.ToolStripRenderMode.System
            '
            'm_tsbnShowBestOnly
            '
            Me.m_tsbnShowBestOnly.CheckOnClick = True
            resources.ApplyResources(Me.m_tsbnShowBestOnly, "m_tsbnShowBestOnly")
            Me.m_tsbnShowBestOnly.Name = "m_tsbnShowBestOnly"
            '
            'm_tsbnShowGroups
            '
            Me.m_tsbnShowGroups.CheckOnClick = True
            resources.ApplyResources(Me.m_tsbnShowGroups, "m_tsbnShowGroups")
            Me.m_tsbnShowGroups.Name = "m_tsbnShowGroups"
            '
            'm_lbGroups
            '
            Me.m_lbGroups.AllGroupsItemColor = System.Drawing.Color.Transparent
            Me.m_lbGroups.AllGroupsItemText = "(All)"
            resources.ApplyResources(Me.m_lbGroups, "m_lbGroups")
            Me.m_lbGroups.BorderStyle = System.Windows.Forms.BorderStyle.None
            Me.m_lbGroups.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed
            Me.m_lbGroups.FormattingEnabled = True
            Me.m_lbGroups.GroupListTracking = ScientificInterfaceShared.Controls.cGroupListBox.eGroupTrackingType.LivingGroups
            Me.m_lbGroups.IsAllGroupsItemSelected = False
            Me.m_lbGroups.Name = "m_lbGroups"
            Me.m_lbGroups.SelectedGroup = Nothing
            Me.m_lbGroups.SelectedGroupIndex = -1
            Me.m_lbGroups.SortThreshold = -9999.0!
            Me.m_lbGroups.SortType = ScientificInterfaceShared.Controls.cGroupListBox.eSortType.ValueAsc
            '
            'm_lblGroups
            '
            resources.ApplyResources(Me.m_lblGroups, "m_lblGroups")
            Me.m_lblGroups.CanCollapseParent = False
            Me.m_lblGroups.CollapsedParentHeight = 0
            Me.m_lblGroups.IsCollapsed = False
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
            Me.m_gridBestFit.AllowBlockSelect = True
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
            Me.m_gridBestFit.UIContext = Nothing
            '
            'm_cbSave
            '
            resources.ApplyResources(Me.m_cbSave, "m_cbSave")
            Me.m_cbSave.Name = "m_cbSave"
            Me.m_cbSave.UseVisualStyleBackColor = True
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
            'm_btnApply
            '
            resources.ApplyResources(Me.m_btnApply, "m_btnApply")
            Me.m_btnApply.Name = "m_btnApply"
            Me.m_btnApply.UseVisualStyleBackColor = True
            '
            'm_nudNumTrials
            '
            resources.ApplyResources(Me.m_nudNumTrials, "m_nudNumTrials")
            Me.m_nudNumTrials.Maximum = New Decimal(New Integer() {2147483647, 0, 0, 0})
            Me.m_nudNumTrials.Name = "m_nudNumTrials"
            '
            'm_btnTS
            '
            resources.ApplyResources(Me.m_btnTS, "m_btnTS")
            Me.m_btnTS.Name = "m_btnTS"
            Me.m_btnTS.UseVisualStyleBackColor = True
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
            Me.m_hdrInputOpt.CanCollapseParent = False
            Me.m_hdrInputOpt.CollapsedParentHeight = 0
            Me.m_hdrInputOpt.IsCollapsed = False
            Me.m_hdrInputOpt.Name = "m_hdrInputOpt"
            '
            'm_hdrOutputParam
            '
            resources.ApplyResources(Me.m_hdrOutputParam, "m_hdrOutputParam")
            Me.m_hdrOutputParam.CanCollapseParent = False
            Me.m_hdrOutputParam.CollapsedParentHeight = 0
            Me.m_hdrOutputParam.IsCollapsed = False
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
            Me.CancelButton = Me.m_btnStop
            Me.Controls.Add(Me.m_cbSave)
            Me.Controls.Add(Me.m_tlpOutputs)
            Me.Controls.Add(Me.m_hdrOutputParam)
            Me.Controls.Add(Me.m_hdrInputOpt)
            Me.Controls.Add(Me.m_nudNumTrials)
            Me.Controls.Add(Me.m_btnTS)
            Me.Controls.Add(Me.m_lblNumTrials)
            Me.Controls.Add(Me.m_tcMain)
            Me.Controls.Add(Me.m_btnApply)
            Me.Controls.Add(Me.m_btnStop)
            Me.Controls.Add(Me.m_btnRunTrials)
            Me.CoreExecutionState = EwEUtils.Core.eCoreExecutionState.EcosimLoaded
            Me.Name = "MCRun"
            Me.TabText = "Monte Carlo simulations"
            Me.m_tcMain.ResumeLayout(False)
            Me.m_tbpSettings.ResumeLayout(False)
            Me.m_tbpSettings.PerformLayout()
            Me.m_tbpB.ResumeLayout(False)
            Me.m_tbpB.PerformLayout()
            Me.m_tsB.ResumeLayout(False)
            Me.m_tsB.PerformLayout()
            Me.m_tbpBP.ResumeLayout(False)
            Me.m_tbpBP.PerformLayout()
            Me.m_tsPB.ResumeLayout(False)
            Me.m_tsPB.PerformLayout()
            Me.m_tbpQB.ResumeLayout(False)
            Me.m_tbpQB.PerformLayout()
            Me.m_tsQB.ResumeLayout(False)
            Me.m_tsQB.PerformLayout()
            Me.m_tbpEE.ResumeLayout(False)
            Me.m_tbpEE.PerformLayout()
            Me.m_tbpBA.ResumeLayout(False)
            Me.m_tbpBA.PerformLayout()
            Me.m_tbpBPlot.ResumeLayout(False)
            Me.m_spPlot.Panel1.ResumeLayout(False)
            Me.m_spPlot.Panel1.PerformLayout()
            Me.m_spPlot.Panel2.ResumeLayout(False)
            CType(Me.m_spPlot, System.ComponentModel.ISupportInitialize).EndInit()
            Me.m_spPlot.ResumeLayout(False)
            Me.m_tsPlot.ResumeLayout(False)
            Me.m_tsPlot.PerformLayout()
            Me.m_tbpBestTrial.ResumeLayout(False)
            CType(Me.m_nudNumTrials, System.ComponentModel.ISupportInitialize).EndInit()
            Me.m_tlpOutputs.ResumeLayout(False)
            Me.m_tlpOutputs.PerformLayout()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents m_lblNumTrials As System.Windows.Forms.Label
        Private WithEvents m_btnRunTrials As System.Windows.Forms.Button
        Private WithEvents m_btnStop As System.Windows.Forms.Button
        Private WithEvents m_btnApply As System.Windows.Forms.Button
        Private WithEvents m_btnTS As System.Windows.Forms.Button
        Private WithEvents m_nudNumTrials As System.Windows.Forms.NumericUpDown
        Private WithEvents m_hdrInputOpt As cEwEHeaderLabel
        Private WithEvents m_hdrOutputParam As cEwEHeaderLabel
        Private WithEvents m_tcMain As System.Windows.Forms.TabControl
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
        Private WithEvents m_gridBestFit As ScientificInterface.Ecosim.gridMCRunOutput
        Private WithEvents m_tbpBPlot As System.Windows.Forms.TabPage
        Private WithEvents m_tbpBestTrial As System.Windows.Forms.TabPage
        Private WithEvents m_tbpBA As System.Windows.Forms.TabPage
        Private WithEvents m_tbpEE As System.Windows.Forms.TabPage
        Private WithEvents m_tbpBP As System.Windows.Forms.TabPage
        Private WithEvents m_tbpB As System.Windows.Forms.TabPage
        Private WithEvents m_spPlot As System.Windows.Forms.SplitContainer
        Private WithEvents m_lbGroups As ScientificInterfaceShared.Controls.cGroupListBox
        Private WithEvents m_lblGroups As cEwEHeaderLabel
        Private WithEvents m_tbpQB As System.Windows.Forms.TabPage
        Private WithEvents m_btDefaultTol As System.Windows.Forms.Button
        Private WithEvents m_tbpSettings As System.Windows.Forms.TabPage
        Private WithEvents m_cbSave As System.Windows.Forms.CheckBox
        Private WithEvents m_cbRetainEstimates As System.Windows.Forms.CheckBox
        Private WithEvents m_cbRetainCurPattern As System.Windows.Forms.CheckBox
        Private WithEvents m_cbShowBioTraj As System.Windows.Forms.CheckBox
        Private WithEvents m_gridB As ScientificInterface.Ecosim.gridMCRunInput
        Private WithEvents m_tsB As cEwEToolstrip
        Private WithEvents m_tsbnLoadPedB As System.Windows.Forms.ToolStripButton
        Private WithEvents m_gridPB As ScientificInterface.Ecosim.gridMCRunInput
        Private WithEvents m_tsPB As ScientificInterfaceShared.Controls.cEwEToolstrip
        Private WithEvents m_tsbnLoadPedPB As System.Windows.Forms.ToolStripButton
        Private WithEvents m_gridQB As ScientificInterface.Ecosim.gridMCRunInput
        Private WithEvents m_tsQB As ScientificInterfaceShared.Controls.cEwEToolstrip
        Private WithEvents m_tsbnLoadPedQB As System.Windows.Forms.ToolStripButton
        Private WithEvents m_gridEE As ScientificInterface.Ecosim.gridMCRunInput
        Private WithEvents m_tsEE As ScientificInterfaceShared.Controls.cEwEToolstrip
        Private WithEvents m_gridBA As ScientificInterface.Ecosim.gridMCRunInput
        Private WithEvents m_tsBA As ScientificInterfaceShared.Controls.cEwEToolstrip
        Private WithEvents m_cbSRA As System.Windows.Forms.CheckBox
        Private WithEvents m_lblFMratio As System.Windows.Forms.Label
        Private WithEvents m_lblEEtol As System.Windows.Forms.Label
        Private WithEvents m_tbxEETol As System.Windows.Forms.TextBox
        Private WithEvents m_tbxFMratio As System.Windows.Forms.TextBox
        Private WithEvents m_tsPlot As ScientificInterfaceShared.Controls.cEwEToolstrip
        Private WithEvents m_tsbnShowBestOnly As System.Windows.Forms.ToolStripButton
        Private WithEvents m_tsbnShowGroups As System.Windows.Forms.ToolStripButton
        Private WithEvents m_graph As ZedGraph.ZedGraphControl
    End Class

End Namespace

