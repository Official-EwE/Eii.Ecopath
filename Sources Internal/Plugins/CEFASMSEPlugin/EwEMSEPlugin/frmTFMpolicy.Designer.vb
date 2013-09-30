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
' Copyright 1991-2013 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'

Imports ScientificInterfaceShared.Forms
Imports ScientificInterfaceShared.Controls



Partial Class frmTFMpolicy
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmTFMpolicy))
        Me.m_scMain = New System.Windows.Forms.SplitContainer()
        Me.btnClose = New System.Windows.Forms.Button()
        Me.btDeleteHCR = New System.Windows.Forms.Button()
        Me.btDeleteStrategy = New System.Windows.Forms.Button()
        Me.btnSaveStrategies = New System.Windows.Forms.Button()
        Me.btAddHCR = New System.Windows.Forms.Button()
        Me.btAddStrategy = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.cbStrategies = New System.Windows.Forms.ComboBox()
        Me.m_graph = New ZedGraph.ZedGraphControl()
        Me.m_grid = New EwEMSEPlugin.gridTargetFishingMortalityPolicy()
        Me.ToolStrip1 = New ScientificInterfaceShared.Controls.cEwEToolstrip()
        Me.tsbDefaultTFM = New System.Windows.Forms.ToolStripButton()
        CType(Me.m_scMain, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.m_scMain.Panel1.SuspendLayout()
        Me.m_scMain.Panel2.SuspendLayout()
        Me.m_scMain.SuspendLayout()
        Me.ToolStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'm_scMain
        '
        resources.ApplyResources(Me.m_scMain, "m_scMain")
        Me.m_scMain.Name = "m_scMain"
        '
        'm_scMain.Panel1
        '
        Me.m_scMain.Panel1.Controls.Add(Me.btnClose)
        Me.m_scMain.Panel1.Controls.Add(Me.btDeleteHCR)
        Me.m_scMain.Panel1.Controls.Add(Me.btDeleteStrategy)
        Me.m_scMain.Panel1.Controls.Add(Me.btnSaveStrategies)
        Me.m_scMain.Panel1.Controls.Add(Me.btAddHCR)
        Me.m_scMain.Panel1.Controls.Add(Me.btAddStrategy)
        Me.m_scMain.Panel1.Controls.Add(Me.Label1)
        Me.m_scMain.Panel1.Controls.Add(Me.cbStrategies)
        Me.m_scMain.Panel1.Controls.Add(Me.m_graph)
        '
        'm_scMain.Panel2
        '
        Me.m_scMain.Panel2.Controls.Add(Me.m_grid)
        Me.m_scMain.Panel2.Controls.Add(Me.ToolStrip1)
        '
        'btnClose
        '
        resources.ApplyResources(Me.btnClose, "btnClose")
        Me.btnClose.Name = "btnClose"
        Me.btnClose.UseVisualStyleBackColor = True
        '
        'btDeleteHCR
        '
        resources.ApplyResources(Me.btDeleteHCR, "btDeleteHCR")
        Me.btDeleteHCR.Name = "btDeleteHCR"
        Me.btDeleteHCR.UseVisualStyleBackColor = True
        '
        'btDeleteStrategy
        '
        resources.ApplyResources(Me.btDeleteStrategy, "btDeleteStrategy")
        Me.btDeleteStrategy.Name = "btDeleteStrategy"
        Me.btDeleteStrategy.UseVisualStyleBackColor = True
        '
        'btnSaveStrategies
        '
        resources.ApplyResources(Me.btnSaveStrategies, "btnSaveStrategies")
        Me.btnSaveStrategies.Name = "btnSaveStrategies"
        Me.btnSaveStrategies.UseVisualStyleBackColor = True
        '
        'btAddHCR
        '
        resources.ApplyResources(Me.btAddHCR, "btAddHCR")
        Me.btAddHCR.Name = "btAddHCR"
        Me.btAddHCR.UseVisualStyleBackColor = True
        '
        'btAddStrategy
        '
        resources.ApplyResources(Me.btAddStrategy, "btAddStrategy")
        Me.btAddStrategy.Name = "btAddStrategy"
        Me.btAddStrategy.UseVisualStyleBackColor = True
        '
        'Label1
        '
        resources.ApplyResources(Me.Label1, "Label1")
        Me.Label1.Name = "Label1"
        '
        'cbStrategies
        '
        Me.cbStrategies.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbStrategies.FormattingEnabled = True
        resources.ApplyResources(Me.cbStrategies, "cbStrategies")
        Me.cbStrategies.Name = "cbStrategies"
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
        'ToolStrip1
        '
        Me.ToolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
        Me.ToolStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsbDefaultTFM})
        resources.ApplyResources(Me.ToolStrip1, "ToolStrip1")
        Me.ToolStrip1.Name = "ToolStrip1"
        Me.ToolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System
        '
        'tsbDefaultTFM
        '
        Me.tsbDefaultTFM.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        resources.ApplyResources(Me.tsbDefaultTFM, "tsbDefaultTFM")
        Me.tsbDefaultTFM.Name = "tsbDefaultTFM"
        '
        'frmTFMpolicy
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.m_scMain)
        Me.Name = "frmTFMpolicy"
        Me.m_scMain.Panel1.ResumeLayout(False)
        Me.m_scMain.Panel1.PerformLayout()
        Me.m_scMain.Panel2.ResumeLayout(False)
        Me.m_scMain.Panel2.PerformLayout()
        CType(Me.m_scMain, System.ComponentModel.ISupportInitialize).EndInit()
        Me.m_scMain.ResumeLayout(False)
        Me.ToolStrip1.ResumeLayout(False)
        Me.ToolStrip1.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    'Ecosim.gridTargetFishingMortalityPolicy
    Private WithEvents m_scMain As System.Windows.Forms.SplitContainer
    Private WithEvents m_graph As ZedGraph.ZedGraphControl
    Private WithEvents ToolStrip1 As cEwEToolstrip
    Private WithEvents tsbDefaultTFM As System.Windows.Forms.ToolStripButton
    Private WithEvents Label1 As System.Windows.Forms.Label
    Private WithEvents cbStrategies As System.Windows.Forms.ComboBox
    Private WithEvents btAddHCR As System.Windows.Forms.Button
    Private WithEvents btAddStrategy As System.Windows.Forms.Button
    Private WithEvents btnSaveStrategies As System.Windows.Forms.Button
    Private WithEvents btDeleteHCR As System.Windows.Forms.Button
    Private WithEvents btDeleteStrategy As System.Windows.Forms.Button
    Private WithEvents btnClose As System.Windows.Forms.Button
    Private WithEvents m_grid As EwEMSEPlugin.gridTargetFishingMortalityPolicy

End Class


