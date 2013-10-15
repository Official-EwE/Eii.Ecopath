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
' The Cefas MSE plug-in was developed by the Centre for Environment, Fisheries and 
' Aquaculture Science (Cefas). 
'
' EwE copyright: 1991- UBC Fisheries Centre, Vancouver BC, Canada.
' Cefas MSE plug-in copyright: 2013- Cefas, Lowestoft, UK.
' ===============================================================================
'

Imports ScientificInterfaceShared.Forms
Imports ScientificInterfaceShared.Controls



Partial Class frmTFMpolicy2
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
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmTFMpolicy2))
        Me.m_scMain = New System.Windows.Forms.SplitContainer()
        Me.m_tsStrategy = New cEwEToolstrip()
        Me.ToolStripLabel1 = New System.Windows.Forms.ToolStripLabel()
        Me.m_tscmStrategies = New System.Windows.Forms.ToolStripComboBox()
        Me.m_tsbnAddStrategy = New System.Windows.Forms.ToolStripButton()
        Me.m_tsbnDeleteStrategy = New System.Windows.Forms.ToolStripButton()
        Me.m_tsbnSaveToCSV = New System.Windows.Forms.ToolStripButton()
        Me.m_graph = New ZedGraph.ZedGraphControl()
        Me.m_grid = New EwEMSEPlugin.gridTargetFishingMortalityPolicy()
        Me.m_tsHCR = New ScientificInterfaceShared.Controls.cEwEToolstrip()
        Me.tsbDefaultTFM = New System.Windows.Forms.ToolStripButton()
        Me.m_sep1 = New System.Windows.Forms.ToolStripSeparator()
        Me.m_tsbnAddHCR = New System.Windows.Forms.ToolStripButton()
        Me.m_tsbnRemoveHCR = New System.Windows.Forms.ToolStripButton()
        Me.m_btnOK = New System.Windows.Forms.Button()
        Me.m_btnCancel = New System.Windows.Forms.Button()
        CType(Me.m_scMain, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.m_scMain.Panel1.SuspendLayout()
        Me.m_scMain.Panel2.SuspendLayout()
        Me.m_scMain.SuspendLayout()
        Me.m_tsStrategy.SuspendLayout()
        Me.m_tsHCR.SuspendLayout()
        Me.SuspendLayout()
        '
        'm_scMain
        '
        resources.ApplyResources(Me.m_scMain, "m_scMain")
        Me.m_scMain.Name = "m_scMain"
        '
        'm_scMain.Panel1
        '
        Me.m_scMain.Panel1.Controls.Add(Me.m_tsStrategy)
        Me.m_scMain.Panel1.Controls.Add(Me.m_graph)
        '
        'm_scMain.Panel2
        '
        Me.m_scMain.Panel2.Controls.Add(Me.m_grid)
        Me.m_scMain.Panel2.Controls.Add(Me.m_tsHCR)
        '
        'm_tsStrategy
        '
        Me.m_tsStrategy.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripLabel1, Me.m_tscmStrategies, Me.m_tsbnAddStrategy, Me.m_tsbnDeleteStrategy, Me.m_tsbnSaveToCSV})
        resources.ApplyResources(Me.m_tsStrategy, "m_tsStrategy")
        Me.m_tsStrategy.Name = "m_tsStrategy"
        '
        'ToolStripLabel1
        '
        Me.ToolStripLabel1.Name = "ToolStripLabel1"
        resources.ApplyResources(Me.ToolStripLabel1, "ToolStripLabel1")
        '
        'm_tscmStrategies
        '
        Me.m_tscmStrategies.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.m_tscmStrategies.Name = "m_tscmStrategies"
        resources.ApplyResources(Me.m_tscmStrategies, "m_tscmStrategies")
        '
        'm_tsbnAddStrategy
        '
        Me.m_tsbnAddStrategy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        resources.ApplyResources(Me.m_tsbnAddStrategy, "m_tsbnAddStrategy")
        Me.m_tsbnAddStrategy.Name = "m_tsbnAddStrategy"
        '
        'm_tsbnDeleteStrategy
        '
        Me.m_tsbnDeleteStrategy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        resources.ApplyResources(Me.m_tsbnDeleteStrategy, "m_tsbnDeleteStrategy")
        Me.m_tsbnDeleteStrategy.Name = "m_tsbnDeleteStrategy"
        '
        'm_tsbnSaveToCSV
        '
        Me.m_tsbnSaveToCSV.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        resources.ApplyResources(Me.m_tsbnSaveToCSV, "m_tsbnSaveToCSV")
        Me.m_tsbnSaveToCSV.Name = "m_tsbnSaveToCSV"
        '
        'm_graph
        '
        resources.ApplyResources(Me.m_graph, "m_graph")
        Me.m_graph.EditModifierKeys = System.Windows.Forms.Keys.None
        Me.m_graph.Name = "m_graph"
        Me.m_graph.ScrollGrace = 0.0R
        Me.m_graph.ScrollMaxX = 0.0R
        Me.m_graph.ScrollMaxY = 0.0R
        Me.m_graph.ScrollMaxY2 = 0.0R
        Me.m_graph.ScrollMinX = 0.0R
        Me.m_graph.ScrollMinY = 0.0R
        Me.m_graph.ScrollMinY2 = 0.0R
        Me.m_graph.ZoomButtons = System.Windows.Forms.MouseButtons.None
        '
        'm_grid
        '
        Me.m_grid.AllowBlockSelect = True
        Me.m_grid.AutoSizeMinHeight = 10
        Me.m_grid.AutoSizeMinWidth = 10
        resources.ApplyResources(Me.m_grid, "m_grid")
        Me.m_grid.AutoStretchColumnsToFitWidth = False
        Me.m_grid.AutoStretchRowsToFitHeight = False
        Me.m_grid.BackColor = System.Drawing.Color.White
        Me.m_grid.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.m_grid.ContextMenuStyle = CType((((SourceGrid2.ContextMenuStyle.ColumnResize Or SourceGrid2.ContextMenuStyle.AutoSize) _
            Or SourceGrid2.ContextMenuStyle.CopyPasteSelection) _
            Or SourceGrid2.ContextMenuStyle.CellContextMenu), SourceGrid2.ContextMenuStyle)
        Me.m_grid.CustomSort = False
        Me.m_grid.DataName = "grid content"
        Me.m_grid.FixedColumnWidths = False
        Me.m_grid.FocusStyle = SourceGrid2.FocusStyle.None
        Me.m_grid.GridToolTipActive = True
        Me.m_grid.IsLayoutSuspended = False
        Me.m_grid.Name = "m_grid"
        Me.m_grid.SpecialKeys = CType((((((((((SourceGrid2.GridSpecialKeys.Ctrl_C Or SourceGrid2.GridSpecialKeys.Ctrl_V) _
            Or SourceGrid2.GridSpecialKeys.Ctrl_X) _
            Or SourceGrid2.GridSpecialKeys.Delete) _
            Or SourceGrid2.GridSpecialKeys.Arrows) _
            Or SourceGrid2.GridSpecialKeys.Tab) _
            Or SourceGrid2.GridSpecialKeys.PageDownUp) _
            Or SourceGrid2.GridSpecialKeys.Enter) _
            Or SourceGrid2.GridSpecialKeys.Escape) _
            Or SourceGrid2.GridSpecialKeys.Backspace), SourceGrid2.GridSpecialKeys)
        Me.m_grid.UIContext = Nothing
        '
        'm_tsHCR
        '
        Me.m_tsHCR.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
        Me.m_tsHCR.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsbDefaultTFM, Me.m_sep1, Me.m_tsbnAddHCR, Me.m_tsbnRemoveHCR})
        resources.ApplyResources(Me.m_tsHCR, "m_tsHCR")
        Me.m_tsHCR.Name = "m_tsHCR"
        Me.m_tsHCR.RenderMode = System.Windows.Forms.ToolStripRenderMode.System
        '
        'tsbDefaultTFM
        '
        Me.tsbDefaultTFM.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        resources.ApplyResources(Me.tsbDefaultTFM, "tsbDefaultTFM")
        Me.tsbDefaultTFM.Name = "tsbDefaultTFM"
        '
        'm_sep1
        '
        Me.m_sep1.Name = "m_sep1"
        resources.ApplyResources(Me.m_sep1, "m_sep1")
        '
        'm_tsbnAddHCR
        '
        Me.m_tsbnAddHCR.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        resources.ApplyResources(Me.m_tsbnAddHCR, "m_tsbnAddHCR")
        Me.m_tsbnAddHCR.Name = "m_tsbnAddHCR"
        '
        'm_tsbnRemoveHCR
        '
        Me.m_tsbnRemoveHCR.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        resources.ApplyResources(Me.m_tsbnRemoveHCR, "m_tsbnRemoveHCR")
        Me.m_tsbnRemoveHCR.Name = "m_tsbnRemoveHCR"
        '
        'm_btnOK
        '
        resources.ApplyResources(Me.m_btnOK, "m_btnOK")
        Me.m_btnOK.Name = "m_btnOK"
        Me.m_btnOK.UseVisualStyleBackColor = True
        '
        'm_btnCancel
        '
        resources.ApplyResources(Me.m_btnCancel, "m_btnCancel")
        Me.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.m_btnCancel.Name = "m_btnCancel"
        Me.m_btnCancel.UseVisualStyleBackColor = True
        '
        'frmTFMpolicy2
        '
        Me.AcceptButton = Me.m_btnOK
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.m_btnCancel
        Me.Controls.Add(Me.m_btnCancel)
        Me.Controls.Add(Me.m_btnOK)
        Me.Controls.Add(Me.m_scMain)
        Me.MinimizeBox = False
        Me.Name = "frmTFMpolicy2"
        Me.ShowIcon = False
        Me.m_scMain.Panel1.ResumeLayout(False)
        Me.m_scMain.Panel1.PerformLayout()
        Me.m_scMain.Panel2.ResumeLayout(False)
        Me.m_scMain.Panel2.PerformLayout()
        CType(Me.m_scMain, System.ComponentModel.ISupportInitialize).EndInit()
        Me.m_scMain.ResumeLayout(False)
        Me.m_tsStrategy.ResumeLayout(False)
        Me.m_tsStrategy.PerformLayout()
        Me.m_tsHCR.ResumeLayout(False)
        Me.m_tsHCR.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    'Ecosim.gridTargetFishingMortalityPolicy
    Private WithEvents m_scMain As System.Windows.Forms.SplitContainer
    Private WithEvents m_graph As ZedGraph.ZedGraphControl
    Private WithEvents m_tsHCR As cEwEToolstrip
    Private WithEvents tsbDefaultTFM As System.Windows.Forms.ToolStripButton
    Private WithEvents m_grid As EwEMSEPlugin.gridTargetFishingMortalityPolicy
    Private WithEvents m_tsStrategy As cEwEToolstrip
    Private WithEvents ToolStripLabel1 As System.Windows.Forms.ToolStripLabel
    Private WithEvents m_tscmStrategies As System.Windows.Forms.ToolStripComboBox
    Private WithEvents m_tsbnAddStrategy As System.Windows.Forms.ToolStripButton
    Private WithEvents m_tsbnDeleteStrategy As System.Windows.Forms.ToolStripButton
    Private WithEvents m_tsbnSaveToCSV As System.Windows.Forms.ToolStripButton
    Private WithEvents m_sep1 As System.Windows.Forms.ToolStripSeparator
    Private WithEvents m_tsbnAddHCR As System.Windows.Forms.ToolStripButton
    Private WithEvents m_tsbnRemoveHCR As System.Windows.Forms.ToolStripButton
    Private WithEvents m_btnOK As System.Windows.Forms.Button
    Private WithEvents m_btnCancel As System.Windows.Forms.Button

End Class


