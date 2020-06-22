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
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'
Imports ScientificInterfaceShared.Forms

Partial Class frmApplyResponses
    Inherits frmEwEGrid

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmApplyResponses))
        Me.m_tsFilter = New System.Windows.Forms.ToolStrip()
        Me.m_tslFilter = New System.Windows.Forms.ToolStripLabel()
        Me.m_tstbFilter = New System.Windows.Forms.ToolStripTextBox()
        Me.m_tsbnCaseSensitive = New System.Windows.Forms.ToolStripButton()
        Me.m_tsMain = New System.Windows.Forms.SplitContainer()
        Me.m_lvShapes = New System.Windows.Forms.ListView()
        Me.m_tsDrivers = New System.Windows.Forms.SplitContainer()
        Me.m_gridDrivers = New EwEApplyEnvResponsesPlugin.gridDrivers()
        Me.m_gridApply = New EwEApplyEnvResponsesPlugin.gridApplyShape()
        Me.m_hdrDrivers = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.m_hdrApplied = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.m_tsFilter.SuspendLayout()
        CType(Me.m_tsMain, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.m_tsMain.Panel1.SuspendLayout()
        Me.m_tsMain.Panel2.SuspendLayout()
        Me.m_tsMain.SuspendLayout()
        CType(Me.m_tsDrivers, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.m_tsDrivers.Panel1.SuspendLayout()
        Me.m_tsDrivers.Panel2.SuspendLayout()
        Me.m_tsDrivers.SuspendLayout()
        Me.SuspendLayout()
        '
        'm_tsFilter
        '
        Me.m_tsFilter.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tslFilter, Me.m_tstbFilter, Me.m_tsbnCaseSensitive})
        Me.m_tsFilter.Location = New System.Drawing.Point(0, 0)
        Me.m_tsFilter.Name = "m_tsFilter"
        Me.m_tsFilter.Size = New System.Drawing.Size(800, 25)
        Me.m_tsFilter.TabIndex = 0
        Me.m_tsFilter.Text = "ToolStrip1"
        '
        'm_tslFilter
        '
        Me.m_tslFilter.Image = CType(resources.GetObject("m_tslFilter.Image"), System.Drawing.Image)
        Me.m_tslFilter.Name = "m_tslFilter"
        Me.m_tslFilter.Size = New System.Drawing.Size(26, 22)
        Me.m_tslFilter.Text = ":"
        '
        'm_tstbFilter
        '
        Me.m_tstbFilter.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.m_tstbFilter.Name = "m_tstbFilter"
        Me.m_tstbFilter.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never
        Me.m_tstbFilter.Size = New System.Drawing.Size(100, 25)
        Me.m_tstbFilter.ToolTipText = "Filter by name"
        '
        'm_tsbnCaseSensitive
        '
        Me.m_tsbnCaseSensitive.CheckOnClick = True
        Me.m_tsbnCaseSensitive.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.m_tsbnCaseSensitive.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.m_tsbnCaseSensitive.Name = "m_tsbnCaseSensitive"
        Me.m_tsbnCaseSensitive.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never
        Me.m_tsbnCaseSensitive.Size = New System.Drawing.Size(25, 22)
        Me.m_tsbnCaseSensitive.Text = "Aa"
        Me.m_tsbnCaseSensitive.ToolTipText = "Name filter is case-sensitive"
        '
        'm_tsMain
        '
        Me.m_tsMain.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_tsMain.Location = New System.Drawing.Point(0, 25)
        Me.m_tsMain.Name = "m_tsMain"
        '
        'm_tsMain.Panel1
        '
        Me.m_tsMain.Panel1.Controls.Add(Me.m_lvShapes)
        '
        'm_tsMain.Panel2
        '
        Me.m_tsMain.Panel2.Controls.Add(Me.m_tsDrivers)
        Me.m_tsMain.Size = New System.Drawing.Size(800, 425)
        Me.m_tsMain.SplitterDistance = 198
        Me.m_tsMain.TabIndex = 2
        '
        'm_lvShapes
        '
        Me.m_lvShapes.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_lvShapes.FullRowSelect = True
        Me.m_lvShapes.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None
        Me.m_lvShapes.HideSelection = False
        Me.m_lvShapes.HoverSelection = True
        Me.m_lvShapes.Location = New System.Drawing.Point(0, 0)
        Me.m_lvShapes.MultiSelect = False
        Me.m_lvShapes.Name = "m_lvShapes"
        Me.m_lvShapes.Size = New System.Drawing.Size(198, 425)
        Me.m_lvShapes.TabIndex = 0
        Me.m_lvShapes.UseCompatibleStateImageBehavior = False
        Me.m_lvShapes.View = System.Windows.Forms.View.SmallIcon
        '
        'm_tsDrivers
        '
        Me.m_tsDrivers.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_tsDrivers.Location = New System.Drawing.Point(0, 0)
        Me.m_tsDrivers.Name = "m_tsDrivers"
        Me.m_tsDrivers.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'm_tsDrivers.Panel1
        '
        Me.m_tsDrivers.Panel1.Controls.Add(Me.m_gridDrivers)
        Me.m_tsDrivers.Panel1.Controls.Add(Me.m_hdrDrivers)
        '
        'm_tsDrivers.Panel2
        '
        Me.m_tsDrivers.Panel2.Controls.Add(Me.m_gridApply)
        Me.m_tsDrivers.Panel2.Controls.Add(Me.m_hdrApplied)
        Me.m_tsDrivers.Size = New System.Drawing.Size(598, 425)
        Me.m_tsDrivers.SplitterDistance = 117
        Me.m_tsDrivers.TabIndex = 1
        '
        'm_gridDrivers
        '
        Me.m_gridDrivers.AllowBlockSelect = True
        Me.m_gridDrivers.AutoSizeMinHeight = 10
        Me.m_gridDrivers.AutoSizeMinWidth = 10
        Me.m_gridDrivers.AutoStretchColumnsToFitWidth = False
        Me.m_gridDrivers.AutoStretchRowsToFitHeight = False
        Me.m_gridDrivers.BackColor = System.Drawing.Color.White
        Me.m_gridDrivers.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.m_gridDrivers.ContextMenuStyle = CType((((SourceGrid2.ContextMenuStyle.ColumnResize Or SourceGrid2.ContextMenuStyle.AutoSize) _
            Or SourceGrid2.ContextMenuStyle.CopyPasteSelection) _
            Or SourceGrid2.ContextMenuStyle.CellContextMenu), SourceGrid2.ContextMenuStyle)
        Me.m_gridDrivers.CustomSort = False
        Me.m_gridDrivers.DataName = "grid content"
        Me.m_gridDrivers.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_gridDrivers.FixedColumnWidths = False
        Me.m_gridDrivers.FocusStyle = SourceGrid2.FocusStyle.None
        Me.m_gridDrivers.GridToolTipActive = True
        Me.m_gridDrivers.IsLayoutSuspended = False
        Me.m_gridDrivers.IsOutputGrid = False
        Me.m_gridDrivers.Location = New System.Drawing.Point(0, 18)
        Me.m_gridDrivers.Name = "m_gridDrivers"
        Me.m_gridDrivers.Size = New System.Drawing.Size(598, 99)
        Me.m_gridDrivers.SpecialKeys = CType((((((((((SourceGrid2.GridSpecialKeys.Ctrl_C Or SourceGrid2.GridSpecialKeys.Ctrl_V) _
            Or SourceGrid2.GridSpecialKeys.Ctrl_X) _
            Or SourceGrid2.GridSpecialKeys.Delete) _
            Or SourceGrid2.GridSpecialKeys.Arrows) _
            Or SourceGrid2.GridSpecialKeys.Tab) _
            Or SourceGrid2.GridSpecialKeys.PageDownUp) _
            Or SourceGrid2.GridSpecialKeys.Enter) _
            Or SourceGrid2.GridSpecialKeys.Escape) _
            Or SourceGrid2.GridSpecialKeys.Backspace), SourceGrid2.GridSpecialKeys)
        Me.m_gridDrivers.TabIndex = 0
        Me.m_gridDrivers.UIContext = Nothing
        '
        'm_gridApply
        '
        Me.m_gridApply.AllowBlockSelect = True
        Me.m_gridApply.AllowDrop = True
        Me.m_gridApply.AutoSizeMinHeight = 10
        Me.m_gridApply.AutoSizeMinWidth = 10
        Me.m_gridApply.AutoStretchColumnsToFitWidth = False
        Me.m_gridApply.AutoStretchRowsToFitHeight = False
        Me.m_gridApply.BackColor = System.Drawing.Color.White
        Me.m_gridApply.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.m_gridApply.ContextMenuStyle = CType((((SourceGrid2.ContextMenuStyle.ColumnResize Or SourceGrid2.ContextMenuStyle.AutoSize) _
            Or SourceGrid2.ContextMenuStyle.CopyPasteSelection) _
            Or SourceGrid2.ContextMenuStyle.CellContextMenu), SourceGrid2.ContextMenuStyle)
        Me.m_gridApply.CustomSort = False
        Me.m_gridApply.DataName = "grid content"
        Me.m_gridApply.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_gridApply.FixedColumnWidths = False
        Me.m_gridApply.FocusStyle = SourceGrid2.FocusStyle.None
        Me.m_gridApply.GridToolTipActive = True
        Me.m_gridApply.IsLayoutSuspended = False
        Me.m_gridApply.IsOutputGrid = False
        Me.m_gridApply.Location = New System.Drawing.Point(0, 18)
        Me.m_gridApply.Name = "m_gridApply"
        Me.m_gridApply.SelectedDriver = Nothing
        Me.m_gridApply.Size = New System.Drawing.Size(598, 286)
        Me.m_gridApply.SpecialKeys = CType((((((((((SourceGrid2.GridSpecialKeys.Ctrl_C Or SourceGrid2.GridSpecialKeys.Ctrl_V) _
            Or SourceGrid2.GridSpecialKeys.Ctrl_X) _
            Or SourceGrid2.GridSpecialKeys.Delete) _
            Or SourceGrid2.GridSpecialKeys.Arrows) _
            Or SourceGrid2.GridSpecialKeys.Tab) _
            Or SourceGrid2.GridSpecialKeys.PageDownUp) _
            Or SourceGrid2.GridSpecialKeys.Enter) _
            Or SourceGrid2.GridSpecialKeys.Escape) _
            Or SourceGrid2.GridSpecialKeys.Backspace), SourceGrid2.GridSpecialKeys)
        Me.m_gridApply.TabIndex = 0
        Me.m_gridApply.UIContext = Nothing
        '
        'm_hdrDrivers
        '
        Me.m_hdrDrivers.CanCollapseParent = False
        Me.m_hdrDrivers.CollapsedParentHeight = 0
        Me.m_hdrDrivers.Dock = System.Windows.Forms.DockStyle.Top
        Me.m_hdrDrivers.IsCollapsed = False
        Me.m_hdrDrivers.Location = New System.Drawing.Point(0, 0)
        Me.m_hdrDrivers.Name = "m_hdrDrivers"
        Me.m_hdrDrivers.Size = New System.Drawing.Size(598, 18)
        Me.m_hdrDrivers.TabIndex = 0
        Me.m_hdrDrivers.Text = "Drivers"
        Me.m_hdrDrivers.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'm_hdrApplied
        '
        Me.m_hdrApplied.CanCollapseParent = False
        Me.m_hdrApplied.CollapsedParentHeight = 0
        Me.m_hdrApplied.Dock = System.Windows.Forms.DockStyle.Top
        Me.m_hdrApplied.IsCollapsed = False
        Me.m_hdrApplied.Location = New System.Drawing.Point(0, 0)
        Me.m_hdrApplied.Name = "m_hdrApplied"
        Me.m_hdrApplied.Size = New System.Drawing.Size(598, 18)
        Me.m_hdrApplied.TabIndex = 0
        Me.m_hdrApplied.Text = "Applied responses"
        Me.m_hdrApplied.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'frmApplyResponses
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(800, 450)
        Me.Controls.Add(Me.m_tsMain)
        Me.Controls.Add(Me.m_tsFilter)
        Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmApplyResponses"
        Me.TabText = ""
        Me.Text = "Apply functional responses tryout)"
        Me.m_tsFilter.ResumeLayout(False)
        Me.m_tsFilter.PerformLayout()
        Me.m_tsMain.Panel1.ResumeLayout(False)
        Me.m_tsMain.Panel2.ResumeLayout(False)
        CType(Me.m_tsMain, System.ComponentModel.ISupportInitialize).EndInit()
        Me.m_tsMain.ResumeLayout(False)
        Me.m_tsDrivers.Panel1.ResumeLayout(False)
        Me.m_tsDrivers.Panel2.ResumeLayout(False)
        CType(Me.m_tsDrivers, System.ComponentModel.ISupportInitialize).EndInit()
        Me.m_tsDrivers.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Private WithEvents m_tsFilter As Windows.Forms.ToolStrip
    Private WithEvents m_tslFilter As Windows.Forms.ToolStripLabel
    Private WithEvents m_tstbFilter As Windows.Forms.ToolStripTextBox
    Private WithEvents m_tsbnCaseSensitive As Windows.Forms.ToolStripButton
    Private WithEvents m_tsMain As Windows.Forms.SplitContainer
    Private WithEvents m_lvShapes As Windows.Forms.ListView
    Private WithEvents m_tsDrivers As Windows.Forms.SplitContainer
    Private WithEvents m_gridDrivers As gridDrivers
    Private WithEvents m_gridApply As gridApplyShape
    Private WithEvents m_hdrDrivers As ScientificInterfaceShared.Controls.cEwEHeaderLabel
    Private WithEvents m_hdrApplied As ScientificInterfaceShared.Controls.cEwEHeaderLabel
End Class
