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
        Me.Panel5 = New System.Windows.Forms.Panel()
        Me.Panel4 = New System.Windows.Forms.Panel()
        Me.m_rbQuotaControls = New System.Windows.Forms.RadioButton()
        Me.m_rbEffortControls = New System.Windows.Forms.RadioButton()
        Me.rbUseRegs = New System.Windows.Forms.RadioButton()
        Me.rbNoRegs = New System.Windows.Forms.RadioButton()
        Me.CEwEHeaderLabel2 = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.m_TableLayout = New System.Windows.Forms.TableLayoutPanel()
        Me.m_panelEffortControls = New System.Windows.Forms.Panel()
        Me.CEwEHeaderLabel6 = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.m_panelQuotaControls = New System.Windows.Forms.Panel()
        Me.CEwEHeaderLabel3 = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.m_hdrEffortRegOptions = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.txMaxEffort = New System.Windows.Forms.TextBox()
        Me.rbEffortEcosim = New System.Windows.Forms.RadioButton()
        Me.rbEffortNoCap = New System.Windows.Forms.RadioButton()
        Me.rbEffortPredicted = New System.Windows.Forms.RadioButton()
        Me.m_panelNoReg = New System.Windows.Forms.Panel()
        Me.CEwEHeaderLabel4 = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.pnlFTracking = New System.Windows.Forms.Panel()
        Me.txSBPower = New System.Windows.Forms.TextBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.rbExact = New System.Windows.Forms.RadioButton()
        Me.rbDirectExp = New System.Windows.Forms.RadioButton()
        Me.rbCatchEstBio = New System.Windows.Forms.RadioButton()
        Me.m_gridFleetLPEffortBounds = New ScientificInterface.gridFleetLPEffortBounds()
        Me.m_gridRegOptions = New ScientificInterface.Ecosim.gridRegulatoryOptions()
        Me.m_pnlRegOpt.SuspendLayout()
        Me.Panel5.SuspendLayout()
        Me.Panel4.SuspendLayout()
        Me.m_TableLayout.SuspendLayout()
        Me.m_panelEffortControls.SuspendLayout()
        Me.m_panelQuotaControls.SuspendLayout()
        Me.m_panelNoReg.SuspendLayout()
        Me.pnlFTracking.SuspendLayout()
        Me.SuspendLayout()
        '
        'm_pnlRegOpt
        '
        resources.ApplyResources(Me.m_pnlRegOpt, "m_pnlRegOpt")
        Me.m_pnlRegOpt.Controls.Add(Me.Panel5)
        Me.m_pnlRegOpt.Controls.Add(Me.CEwEHeaderLabel2)
        Me.m_pnlRegOpt.Name = "m_pnlRegOpt"
        '
        'Panel5
        '
        Me.Panel5.Controls.Add(Me.Panel4)
        Me.Panel5.Controls.Add(Me.rbUseRegs)
        Me.Panel5.Controls.Add(Me.rbNoRegs)
        resources.ApplyResources(Me.Panel5, "Panel5")
        Me.Panel5.Name = "Panel5"
        '
        'Panel4
        '
        Me.Panel4.Controls.Add(Me.m_rbQuotaControls)
        Me.Panel4.Controls.Add(Me.m_rbEffortControls)
        resources.ApplyResources(Me.Panel4, "Panel4")
        Me.Panel4.Name = "Panel4"
        '
        'm_rbQuotaControls
        '
        resources.ApplyResources(Me.m_rbQuotaControls, "m_rbQuotaControls")
        Me.m_rbQuotaControls.Name = "m_rbQuotaControls"
        Me.m_rbQuotaControls.UseVisualStyleBackColor = True
        '
        'm_rbEffortControls
        '
        resources.ApplyResources(Me.m_rbEffortControls, "m_rbEffortControls")
        Me.m_rbEffortControls.Checked = True
        Me.m_rbEffortControls.Name = "m_rbEffortControls"
        Me.m_rbEffortControls.TabStop = True
        Me.m_rbEffortControls.UseVisualStyleBackColor = True
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
        'CEwEHeaderLabel2
        '
        Me.CEwEHeaderLabel2.CanCollapseParent = False
        Me.CEwEHeaderLabel2.CollapsedParentHeight = 0
        resources.ApplyResources(Me.CEwEHeaderLabel2, "CEwEHeaderLabel2")
        Me.CEwEHeaderLabel2.IsCollapsed = False
        Me.CEwEHeaderLabel2.Name = "CEwEHeaderLabel2"
        '
        'm_TableLayout
        '
        resources.ApplyResources(Me.m_TableLayout, "m_TableLayout")
        Me.m_TableLayout.Controls.Add(Me.m_panelEffortControls, 0, 0)
        Me.m_TableLayout.Controls.Add(Me.m_panelQuotaControls, 0, 1)
        Me.m_TableLayout.Controls.Add(Me.m_panelNoReg, 0, 2)
        Me.m_TableLayout.Name = "m_TableLayout"
        '
        'm_panelEffortControls
        '
        Me.m_panelEffortControls.Controls.Add(Me.CEwEHeaderLabel6)
        Me.m_panelEffortControls.Controls.Add(Me.m_gridFleetLPEffortBounds)
        resources.ApplyResources(Me.m_panelEffortControls, "m_panelEffortControls")
        Me.m_panelEffortControls.Name = "m_panelEffortControls"
        '
        'CEwEHeaderLabel6
        '
        resources.ApplyResources(Me.CEwEHeaderLabel6, "CEwEHeaderLabel6")
        Me.CEwEHeaderLabel6.CanCollapseParent = False
        Me.CEwEHeaderLabel6.CollapsedParentHeight = 0
        Me.CEwEHeaderLabel6.IsCollapsed = False
        Me.CEwEHeaderLabel6.Name = "CEwEHeaderLabel6"
        '
        'm_panelQuotaControls
        '
        Me.m_panelQuotaControls.Controls.Add(Me.CEwEHeaderLabel3)
        Me.m_panelQuotaControls.Controls.Add(Me.m_hdrEffortRegOptions)
        Me.m_panelQuotaControls.Controls.Add(Me.txMaxEffort)
        Me.m_panelQuotaControls.Controls.Add(Me.rbEffortEcosim)
        Me.m_panelQuotaControls.Controls.Add(Me.rbEffortNoCap)
        Me.m_panelQuotaControls.Controls.Add(Me.rbEffortPredicted)
        Me.m_panelQuotaControls.Controls.Add(Me.m_gridRegOptions)
        resources.ApplyResources(Me.m_panelQuotaControls, "m_panelQuotaControls")
        Me.m_panelQuotaControls.Name = "m_panelQuotaControls"
        '
        'CEwEHeaderLabel3
        '
        resources.ApplyResources(Me.CEwEHeaderLabel3, "CEwEHeaderLabel3")
        Me.CEwEHeaderLabel3.CanCollapseParent = False
        Me.CEwEHeaderLabel3.CollapsedParentHeight = 0
        Me.CEwEHeaderLabel3.IsCollapsed = False
        Me.CEwEHeaderLabel3.Name = "CEwEHeaderLabel3"
        '
        'm_hdrEffortRegOptions
        '
        resources.ApplyResources(Me.m_hdrEffortRegOptions, "m_hdrEffortRegOptions")
        Me.m_hdrEffortRegOptions.CanCollapseParent = False
        Me.m_hdrEffortRegOptions.CollapsedParentHeight = 0
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
        'm_panelNoReg
        '
        Me.m_panelNoReg.Controls.Add(Me.CEwEHeaderLabel4)
        Me.m_panelNoReg.Controls.Add(Me.pnlFTracking)
        resources.ApplyResources(Me.m_panelNoReg, "m_panelNoReg")
        Me.m_panelNoReg.Name = "m_panelNoReg"
        '
        'CEwEHeaderLabel4
        '
        resources.ApplyResources(Me.CEwEHeaderLabel4, "CEwEHeaderLabel4")
        Me.CEwEHeaderLabel4.CanCollapseParent = False
        Me.CEwEHeaderLabel4.CollapsedParentHeight = 0
        Me.CEwEHeaderLabel4.IsCollapsed = False
        Me.CEwEHeaderLabel4.Name = "CEwEHeaderLabel4"
        '
        'pnlFTracking
        '
        resources.ApplyResources(Me.pnlFTracking, "pnlFTracking")
        Me.pnlFTracking.Controls.Add(Me.txSBPower)
        Me.pnlFTracking.Controls.Add(Me.Label6)
        Me.pnlFTracking.Controls.Add(Me.rbExact)
        Me.pnlFTracking.Controls.Add(Me.rbDirectExp)
        Me.pnlFTracking.Controls.Add(Me.rbCatchEstBio)
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
        'm_gridRegOptions
        '
        Me.m_gridRegOptions.AllowBlockSelect = True
        resources.ApplyResources(Me.m_gridRegOptions, "m_gridRegOptions")
        Me.m_gridRegOptions.AutoSizeMinHeight = 10
        Me.m_gridRegOptions.AutoSizeMinWidth = 10
        Me.m_gridRegOptions.AutoStretchColumnsToFitWidth = False
        Me.m_gridRegOptions.AutoStretchRowsToFitHeight = False
        Me.m_gridRegOptions.BackColor = System.Drawing.Color.White
        Me.m_gridRegOptions.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.m_gridRegOptions.ContextMenuStyle = CType((((SourceGrid2.ContextMenuStyle.ColumnResize Or SourceGrid2.ContextMenuStyle.AutoSize) _
            Or SourceGrid2.ContextMenuStyle.CopyPasteSelection) _
            Or SourceGrid2.ContextMenuStyle.CellContextMenu), SourceGrid2.ContextMenuStyle)
        Me.m_gridRegOptions.CustomSort = False
        Me.m_gridRegOptions.FixedColumnWidths = True
        Me.m_gridRegOptions.FocusStyle = SourceGrid2.FocusStyle.None
        Me.m_gridRegOptions.GridToolTipActive = True
        Me.m_gridRegOptions.Name = "m_gridRegOptions"
        Me.m_gridRegOptions.SpecialKeys = CType((((((((((SourceGrid2.GridSpecialKeys.Ctrl_C Or SourceGrid2.GridSpecialKeys.Ctrl_V) _
            Or SourceGrid2.GridSpecialKeys.Ctrl_X) _
            Or SourceGrid2.GridSpecialKeys.Delete) _
            Or SourceGrid2.GridSpecialKeys.Arrows) _
            Or SourceGrid2.GridSpecialKeys.Tab) _
            Or SourceGrid2.GridSpecialKeys.PageDownUp) _
            Or SourceGrid2.GridSpecialKeys.Enter) _
            Or SourceGrid2.GridSpecialKeys.Escape) _
            Or SourceGrid2.GridSpecialKeys.Backspace), SourceGrid2.GridSpecialKeys)
        Me.m_gridRegOptions.UIContext = Nothing
        '
        'frmMSEOptions
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.m_pnlRegOpt)
        Me.Controls.Add(Me.m_TableLayout)
        Me.Name = "frmMSEOptions"
        Me.m_pnlRegOpt.ResumeLayout(False)
        Me.Panel5.ResumeLayout(False)
        Me.Panel5.PerformLayout()
        Me.Panel4.ResumeLayout(False)
        Me.Panel4.PerformLayout()
        Me.m_TableLayout.ResumeLayout(False)
        Me.m_panelEffortControls.ResumeLayout(False)
        Me.m_panelQuotaControls.ResumeLayout(False)
        Me.m_panelQuotaControls.PerformLayout()
        Me.m_panelNoReg.ResumeLayout(False)
        Me.pnlFTracking.ResumeLayout(False)
        Me.pnlFTracking.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Private WithEvents m_pnlRegOpt As System.Windows.Forms.Panel
    Private WithEvents CEwEHeaderLabel2 As ScientificInterfaceShared.Controls.cEwEHeaderLabel
    Friend WithEvents m_TableLayout As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents m_panelEffortControls As System.Windows.Forms.Panel
    Private WithEvents m_gridFleetLPEffortBounds As ScientificInterface.gridFleetLPEffortBounds
    Friend WithEvents m_panelQuotaControls As System.Windows.Forms.Panel
    Private WithEvents CEwEHeaderLabel3 As ScientificInterfaceShared.Controls.cEwEHeaderLabel
    Private WithEvents m_hdrEffortRegOptions As ScientificInterfaceShared.Controls.cEwEHeaderLabel
    Friend WithEvents txMaxEffort As System.Windows.Forms.TextBox
    Private WithEvents rbEffortEcosim As System.Windows.Forms.RadioButton
    Private WithEvents rbEffortNoCap As System.Windows.Forms.RadioButton
    Private WithEvents rbEffortPredicted As System.Windows.Forms.RadioButton
    Friend WithEvents m_gridRegOptions As ScientificInterface.Ecosim.gridRegulatoryOptions
    Friend WithEvents m_panelNoReg As System.Windows.Forms.Panel
    Private WithEvents pnlFTracking As System.Windows.Forms.Panel
    Private WithEvents txSBPower As System.Windows.Forms.TextBox
    Private WithEvents Label6 As System.Windows.Forms.Label
    Private WithEvents rbExact As System.Windows.Forms.RadioButton
    Private WithEvents rbDirectExp As System.Windows.Forms.RadioButton
    Private WithEvents rbCatchEstBio As System.Windows.Forms.RadioButton
    Friend WithEvents Panel5 As System.Windows.Forms.Panel
    Friend WithEvents Panel4 As System.Windows.Forms.Panel
    Friend WithEvents m_rbQuotaControls As System.Windows.Forms.RadioButton
    Friend WithEvents m_rbEffortControls As System.Windows.Forms.RadioButton
    Private WithEvents rbUseRegs As System.Windows.Forms.RadioButton
    Private WithEvents rbNoRegs As System.Windows.Forms.RadioButton
    Private WithEvents CEwEHeaderLabel6 As ScientificInterfaceShared.Controls.cEwEHeaderLabel
    Private WithEvents CEwEHeaderLabel4 As ScientificInterfaceShared.Controls.cEwEHeaderLabel
End Class
