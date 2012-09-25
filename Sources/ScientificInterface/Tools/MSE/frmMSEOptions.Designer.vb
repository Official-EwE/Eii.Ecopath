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
' Copyright 1991-2012 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'
Imports ScientificInterfaceShared.Forms
Imports ScientificInterfaceShared.Controls

Partial Class frmMSEOptions
    Inherits frmEwE

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMSEOptions))
        Me.m_pnlRegOpt = New System.Windows.Forms.Panel()
        Me.CEwEHeaderLabel2 = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.m_rbQuotaControls = New System.Windows.Forms.RadioButton()
        Me.m_rbEffortControls = New System.Windows.Forms.RadioButton()
        Me.pnlFTracking = New System.Windows.Forms.Panel()
        Me.txSBPower = New System.Windows.Forms.TextBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.rbExact = New System.Windows.Forms.RadioButton()
        Me.rbDirectExp = New System.Windows.Forms.RadioButton()
        Me.rbCatchEstBio = New System.Windows.Forms.RadioButton()
        Me.rbUseRegs = New System.Windows.Forms.RadioButton()
        Me.rbNoRegs = New System.Windows.Forms.RadioButton()
        Me.m_pnlRunOpt = New System.Windows.Forms.Panel()
        Me.txKalmanGain = New System.Windows.Forms.TextBox()
        Me.m_lblKalmanGain = New System.Windows.Forms.Label()
        Me.m_ckPlugin = New System.Windows.Forms.CheckBox()
        Me.m_hdrRunOptions = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.m_SplitControls = New System.Windows.Forms.SplitContainer()
        Me.m_gridFleetLPEffortBounds = New ScientificInterface.gridFleetLPEffortBounds()
        Me.CEwEHeaderLabel1 = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.CEwEHeaderLabel3 = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.m_hdrEffortRegOptions = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.txMaxEffort = New System.Windows.Forms.TextBox()
        Me.rbEffortEcosim = New System.Windows.Forms.RadioButton()
        Me.rbEffortNoCap = New System.Windows.Forms.RadioButton()
        Me.rbEffortPredicted = New System.Windows.Forms.RadioButton()
        Me.m_GrdRegOptions = New ScientificInterface.Ecosim.gridRegulatoryOptions()
        Me.m_pnlRegOpt.SuspendLayout()
        Me.pnlFTracking.SuspendLayout()
        Me.m_pnlRunOpt.SuspendLayout()
        CType(Me.m_SplitControls, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.m_SplitControls.Panel1.SuspendLayout()
        Me.m_SplitControls.Panel2.SuspendLayout()
        Me.m_SplitControls.SuspendLayout()
        Me.SuspendLayout()
        '
        'm_pnlRegOpt
        '
        resources.ApplyResources(Me.m_pnlRegOpt, "m_pnlRegOpt")
        Me.m_pnlRegOpt.Controls.Add(Me.CEwEHeaderLabel2)
        Me.m_pnlRegOpt.Controls.Add(Me.m_rbQuotaControls)
        Me.m_pnlRegOpt.Controls.Add(Me.m_rbEffortControls)
        Me.m_pnlRegOpt.Controls.Add(Me.pnlFTracking)
        Me.m_pnlRegOpt.Controls.Add(Me.rbUseRegs)
        Me.m_pnlRegOpt.Controls.Add(Me.rbNoRegs)
        Me.m_pnlRegOpt.Name = "m_pnlRegOpt"
        '
        'CEwEHeaderLabel2
        '
        Me.CEwEHeaderLabel2.CanCollapseParent = False
        Me.CEwEHeaderLabel2.CollapsedParentHeight = 0
        resources.ApplyResources(Me.CEwEHeaderLabel2, "CEwEHeaderLabel2")
        Me.CEwEHeaderLabel2.IsCollapsed = False
        Me.CEwEHeaderLabel2.Name = "CEwEHeaderLabel2"
        '
        'm_rbQuotaControls
        '
        resources.ApplyResources(Me.m_rbQuotaControls, "m_rbQuotaControls")
        Me.m_rbQuotaControls.Name = "m_rbQuotaControls"
        Me.m_rbQuotaControls.TabStop = True
        Me.m_rbQuotaControls.UseVisualStyleBackColor = True
        '
        'm_rbEffortControls
        '
        resources.ApplyResources(Me.m_rbEffortControls, "m_rbEffortControls")
        Me.m_rbEffortControls.Name = "m_rbEffortControls"
        Me.m_rbEffortControls.TabStop = True
        Me.m_rbEffortControls.UseVisualStyleBackColor = True
        '
        'pnlFTracking
        '
        Me.pnlFTracking.Controls.Add(Me.txSBPower)
        Me.pnlFTracking.Controls.Add(Me.Label6)
        Me.pnlFTracking.Controls.Add(Me.rbExact)
        Me.pnlFTracking.Controls.Add(Me.rbDirectExp)
        Me.pnlFTracking.Controls.Add(Me.rbCatchEstBio)
        resources.ApplyResources(Me.pnlFTracking, "pnlFTracking")
        Me.pnlFTracking.Name = "pnlFTracking"
        '
        'txSBPower
        '
        resources.ApplyResources(Me.txSBPower, "txSBPower")
        Me.txSBPower.Name = "txSBPower"
        '
        'Label6
        '
        resources.ApplyResources(Me.Label6, "Label6")
        Me.Label6.Name = "Label6"
        '
        'rbExact
        '
        resources.ApplyResources(Me.rbExact, "rbExact")
        Me.rbExact.Name = "rbExact"
        Me.rbExact.TabStop = True
        Me.rbExact.UseVisualStyleBackColor = True
        '
        'rbDirectExp
        '
        resources.ApplyResources(Me.rbDirectExp, "rbDirectExp")
        Me.rbDirectExp.Name = "rbDirectExp"
        Me.rbDirectExp.UseVisualStyleBackColor = True
        '
        'rbCatchEstBio
        '
        resources.ApplyResources(Me.rbCatchEstBio, "rbCatchEstBio")
        Me.rbCatchEstBio.Checked = True
        Me.rbCatchEstBio.Name = "rbCatchEstBio"
        Me.rbCatchEstBio.TabStop = True
        Me.rbCatchEstBio.UseVisualStyleBackColor = True
        '
        'rbUseRegs
        '
        resources.ApplyResources(Me.rbUseRegs, "rbUseRegs")
        Me.rbUseRegs.Checked = True
        Me.rbUseRegs.Name = "rbUseRegs"
        Me.rbUseRegs.TabStop = True
        Me.rbUseRegs.UseVisualStyleBackColor = True
        '
        'rbNoRegs
        '
        resources.ApplyResources(Me.rbNoRegs, "rbNoRegs")
        Me.rbNoRegs.Name = "rbNoRegs"
        Me.rbNoRegs.UseVisualStyleBackColor = True
        '
        'm_pnlRunOpt
        '
        resources.ApplyResources(Me.m_pnlRunOpt, "m_pnlRunOpt")
        Me.m_pnlRunOpt.Controls.Add(Me.txKalmanGain)
        Me.m_pnlRunOpt.Controls.Add(Me.m_lblKalmanGain)
        Me.m_pnlRunOpt.Controls.Add(Me.m_ckPlugin)
        Me.m_pnlRunOpt.Controls.Add(Me.m_hdrRunOptions)
        Me.m_pnlRunOpt.Name = "m_pnlRunOpt"
        '
        'txKalmanGain
        '
        resources.ApplyResources(Me.txKalmanGain, "txKalmanGain")
        Me.txKalmanGain.Name = "txKalmanGain"
        '
        'm_lblKalmanGain
        '
        resources.ApplyResources(Me.m_lblKalmanGain, "m_lblKalmanGain")
        Me.m_lblKalmanGain.Name = "m_lblKalmanGain"
        '
        'm_ckPlugin
        '
        resources.ApplyResources(Me.m_ckPlugin, "m_ckPlugin")
        Me.m_ckPlugin.Name = "m_ckPlugin"
        Me.m_ckPlugin.UseVisualStyleBackColor = True
        '
        'm_hdrRunOptions
        '
        Me.m_hdrRunOptions.CanCollapseParent = False
        Me.m_hdrRunOptions.CollapsedParentHeight = 0
        resources.ApplyResources(Me.m_hdrRunOptions, "m_hdrRunOptions")
        Me.m_hdrRunOptions.IsCollapsed = False
        Me.m_hdrRunOptions.Name = "m_hdrRunOptions"
        '
        'm_SplitControls
        '
        resources.ApplyResources(Me.m_SplitControls, "m_SplitControls")
        Me.m_SplitControls.Name = "m_SplitControls"
        '
        'm_SplitControls.Panel1
        '
        Me.m_SplitControls.Panel1.Controls.Add(Me.m_gridFleetLPEffortBounds)
        Me.m_SplitControls.Panel1.Controls.Add(Me.CEwEHeaderLabel1)
        '
        'm_SplitControls.Panel2
        '
        Me.m_SplitControls.Panel2.Controls.Add(Me.CEwEHeaderLabel3)
        Me.m_SplitControls.Panel2.Controls.Add(Me.m_hdrEffortRegOptions)
        Me.m_SplitControls.Panel2.Controls.Add(Me.txMaxEffort)
        Me.m_SplitControls.Panel2.Controls.Add(Me.rbEffortEcosim)
        Me.m_SplitControls.Panel2.Controls.Add(Me.rbEffortNoCap)
        Me.m_SplitControls.Panel2.Controls.Add(Me.rbEffortPredicted)
        Me.m_SplitControls.Panel2.Controls.Add(Me.m_GrdRegOptions)
        '
        'm_gridFleetLPEffortBounds
        '
        Me.m_gridFleetLPEffortBounds.AllowBlockSelect = True
        resources.ApplyResources(Me.m_gridFleetLPEffortBounds, "m_gridFleetLPEffortBounds")
        Me.m_gridFleetLPEffortBounds.AutoSizeMinHeight = 10
        Me.m_gridFleetLPEffortBounds.AutoSizeMinWidth = 10
        Me.m_gridFleetLPEffortBounds.AutoStretchColumnsToFitWidth = False
        Me.m_gridFleetLPEffortBounds.AutoStretchRowsToFitHeight = False
        Me.m_gridFleetLPEffortBounds.BackColor = System.Drawing.Color.White
        Me.m_gridFleetLPEffortBounds.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.m_gridFleetLPEffortBounds.ContextMenuStyle = CType((((SourceGrid2.ContextMenuStyle.ColumnResize Or SourceGrid2.ContextMenuStyle.AutoSize) _
            Or SourceGrid2.ContextMenuStyle.CopyPasteSelection) _
            Or SourceGrid2.ContextMenuStyle.CellContextMenu), SourceGrid2.ContextMenuStyle)
        Me.m_gridFleetLPEffortBounds.CustomSort = False
        Me.m_gridFleetLPEffortBounds.FixedColumnWidths = False
        Me.m_gridFleetLPEffortBounds.FocusStyle = SourceGrid2.FocusStyle.None
        Me.m_gridFleetLPEffortBounds.GridToolTipActive = True
        Me.m_gridFleetLPEffortBounds.Name = "m_gridFleetLPEffortBounds"
        Me.m_gridFleetLPEffortBounds.SpecialKeys = CType((((((((((SourceGrid2.GridSpecialKeys.Ctrl_C Or SourceGrid2.GridSpecialKeys.Ctrl_V) _
            Or SourceGrid2.GridSpecialKeys.Ctrl_X) _
            Or SourceGrid2.GridSpecialKeys.Delete) _
            Or SourceGrid2.GridSpecialKeys.Arrows) _
            Or SourceGrid2.GridSpecialKeys.Tab) _
            Or SourceGrid2.GridSpecialKeys.PageDownUp) _
            Or SourceGrid2.GridSpecialKeys.Enter) _
            Or SourceGrid2.GridSpecialKeys.Escape) _
            Or SourceGrid2.GridSpecialKeys.Backspace), SourceGrid2.GridSpecialKeys)
        Me.m_gridFleetLPEffortBounds.UIContext = Nothing
        '
        'CEwEHeaderLabel1
        '
        resources.ApplyResources(Me.CEwEHeaderLabel1, "CEwEHeaderLabel1")
        Me.CEwEHeaderLabel1.CanCollapseParent = False
        Me.CEwEHeaderLabel1.CollapsedParentHeight = 0
        Me.CEwEHeaderLabel1.IsCollapsed = False
        Me.CEwEHeaderLabel1.Name = "CEwEHeaderLabel1"
        '
        'CEwEHeaderLabel3
        '
        Me.CEwEHeaderLabel3.CanCollapseParent = False
        Me.CEwEHeaderLabel3.CollapsedParentHeight = 0
        resources.ApplyResources(Me.CEwEHeaderLabel3, "CEwEHeaderLabel3")
        Me.CEwEHeaderLabel3.IsCollapsed = False
        Me.CEwEHeaderLabel3.Name = "CEwEHeaderLabel3"
        '
        'm_hdrEffortRegOptions
        '
        Me.m_hdrEffortRegOptions.CanCollapseParent = False
        Me.m_hdrEffortRegOptions.CollapsedParentHeight = 0
        resources.ApplyResources(Me.m_hdrEffortRegOptions, "m_hdrEffortRegOptions")
        Me.m_hdrEffortRegOptions.IsCollapsed = False
        Me.m_hdrEffortRegOptions.Name = "m_hdrEffortRegOptions"
        '
        'txMaxEffort
        '
        resources.ApplyResources(Me.txMaxEffort, "txMaxEffort")
        Me.txMaxEffort.Name = "txMaxEffort"
        '
        'rbEffortEcosim
        '
        resources.ApplyResources(Me.rbEffortEcosim, "rbEffortEcosim")
        Me.rbEffortEcosim.Name = "rbEffortEcosim"
        Me.rbEffortEcosim.UseVisualStyleBackColor = True
        '
        'rbEffortNoCap
        '
        resources.ApplyResources(Me.rbEffortNoCap, "rbEffortNoCap")
        Me.rbEffortNoCap.Checked = True
        Me.rbEffortNoCap.Name = "rbEffortNoCap"
        Me.rbEffortNoCap.TabStop = True
        Me.rbEffortNoCap.UseVisualStyleBackColor = True
        '
        'rbEffortPredicted
        '
        resources.ApplyResources(Me.rbEffortPredicted, "rbEffortPredicted")
        Me.rbEffortPredicted.Name = "rbEffortPredicted"
        Me.rbEffortPredicted.UseVisualStyleBackColor = True
        '
        'm_GrdRegOptions
        '
        Me.m_GrdRegOptions.AllowBlockSelect = True
        resources.ApplyResources(Me.m_GrdRegOptions, "m_GrdRegOptions")
        Me.m_GrdRegOptions.AutoSizeMinHeight = 10
        Me.m_GrdRegOptions.AutoSizeMinWidth = 10
        Me.m_GrdRegOptions.AutoStretchColumnsToFitWidth = False
        Me.m_GrdRegOptions.AutoStretchRowsToFitHeight = False
        Me.m_GrdRegOptions.BackColor = System.Drawing.Color.White
        Me.m_GrdRegOptions.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.m_GrdRegOptions.ContextMenuStyle = CType((((SourceGrid2.ContextMenuStyle.ColumnResize Or SourceGrid2.ContextMenuStyle.AutoSize) _
            Or SourceGrid2.ContextMenuStyle.CopyPasteSelection) _
            Or SourceGrid2.ContextMenuStyle.CellContextMenu), SourceGrid2.ContextMenuStyle)
        Me.m_GrdRegOptions.CustomSort = False
        Me.m_GrdRegOptions.FixedColumnWidths = True
        Me.m_GrdRegOptions.FocusStyle = SourceGrid2.FocusStyle.None
        Me.m_GrdRegOptions.GridToolTipActive = True
        Me.m_GrdRegOptions.Name = "m_GrdRegOptions"
        Me.m_GrdRegOptions.SpecialKeys = CType((((((((((SourceGrid2.GridSpecialKeys.Ctrl_C Or SourceGrid2.GridSpecialKeys.Ctrl_V) _
            Or SourceGrid2.GridSpecialKeys.Ctrl_X) _
            Or SourceGrid2.GridSpecialKeys.Delete) _
            Or SourceGrid2.GridSpecialKeys.Arrows) _
            Or SourceGrid2.GridSpecialKeys.Tab) _
            Or SourceGrid2.GridSpecialKeys.PageDownUp) _
            Or SourceGrid2.GridSpecialKeys.Enter) _
            Or SourceGrid2.GridSpecialKeys.Escape) _
            Or SourceGrid2.GridSpecialKeys.Backspace), SourceGrid2.GridSpecialKeys)
        Me.m_GrdRegOptions.UIContext = Nothing
        '
        'frmMSEOptions
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.m_SplitControls)
        Me.Controls.Add(Me.m_pnlRunOpt)
        Me.Controls.Add(Me.m_pnlRegOpt)
        Me.Name = "frmMSEOptions"
        Me.m_pnlRegOpt.ResumeLayout(False)
        Me.m_pnlRegOpt.PerformLayout()
        Me.pnlFTracking.ResumeLayout(False)
        Me.pnlFTracking.PerformLayout()
        Me.m_pnlRunOpt.ResumeLayout(False)
        Me.m_pnlRunOpt.PerformLayout()
        Me.m_SplitControls.Panel1.ResumeLayout(False)
        Me.m_SplitControls.Panel2.ResumeLayout(False)
        Me.m_SplitControls.Panel2.PerformLayout()
        CType(Me.m_SplitControls, System.ComponentModel.ISupportInitialize).EndInit()
        Me.m_SplitControls.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Private WithEvents rbUseRegs As System.Windows.Forms.RadioButton
    Private WithEvents rbNoRegs As System.Windows.Forms.RadioButton
    Private WithEvents pnlFTracking As System.Windows.Forms.Panel
    Private WithEvents txSBPower As System.Windows.Forms.TextBox
    Private WithEvents Label6 As System.Windows.Forms.Label
    Private WithEvents rbExact As System.Windows.Forms.RadioButton
    Private WithEvents rbDirectExp As System.Windows.Forms.RadioButton
    Private WithEvents rbCatchEstBio As System.Windows.Forms.RadioButton
    Private WithEvents txKalmanGain As System.Windows.Forms.TextBox
    Private WithEvents m_lblKalmanGain As System.Windows.Forms.Label
    Private WithEvents m_ckPlugin As System.Windows.Forms.CheckBox
    Private WithEvents m_hdrRunOptions As cEwEHeaderLabel
    Private WithEvents m_pnlRegOpt As System.Windows.Forms.Panel
    Private WithEvents m_pnlRunOpt As System.Windows.Forms.Panel
    Private WithEvents CEwEHeaderLabel2 As ScientificInterfaceShared.Controls.cEwEHeaderLabel
    Friend WithEvents m_rbQuotaControls As System.Windows.Forms.RadioButton
    Friend WithEvents m_rbEffortControls As System.Windows.Forms.RadioButton
    Friend WithEvents m_SplitControls As System.Windows.Forms.SplitContainer
    Private WithEvents CEwEHeaderLabel1 As ScientificInterfaceShared.Controls.cEwEHeaderLabel
    Private WithEvents CEwEHeaderLabel3 As ScientificInterfaceShared.Controls.cEwEHeaderLabel
    Private WithEvents m_hdrEffortRegOptions As ScientificInterfaceShared.Controls.cEwEHeaderLabel
    Friend WithEvents txMaxEffort As System.Windows.Forms.TextBox
    Private WithEvents rbEffortEcosim As System.Windows.Forms.RadioButton
    Private WithEvents rbEffortNoCap As System.Windows.Forms.RadioButton
    Private WithEvents rbEffortPredicted As System.Windows.Forms.RadioButton
    Friend WithEvents m_GrdRegOptions As ScientificInterface.Ecosim.gridRegulatoryOptions
    Private WithEvents m_gridFleetLPEffortBounds As ScientificInterface.gridFleetLPEffortBounds
End Class
