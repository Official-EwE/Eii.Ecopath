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

Partial Class frmMSEResults
    Inherits frmEwE

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(disposing As Boolean)
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMSEResults))
        Me.m_grid = New ScientificInterface.gridRiskResults()
        Me.m_tlpAll = New System.Windows.Forms.TableLayoutPanel()
        Me.m_tsOptions = New cEwEToolstrip()
        Me.m_tslViewAs = New System.Windows.Forms.ToolStripLabel()
        Me.m_tsbnGroup = New System.Windows.Forms.ToolStripButton()
        Me.m_tsbnFleet = New System.Windows.Forms.ToolStripButton()
        Me.m_tlpAll.SuspendLayout()
        Me.m_tsOptions.SuspendLayout()
        Me.SuspendLayout()
        '
        'm_grid
        '
        Me.m_grid.AllowBlockSelect = True
        Me.m_grid.AutoSizeMinHeight = 10
        Me.m_grid.AutoSizeMinWidth = 10
        Me.m_grid.AutoStretchColumnsToFitWidth = False
        Me.m_grid.AutoStretchRowsToFitHeight = False
        Me.m_grid.BackColor = System.Drawing.Color.White
        Me.m_grid.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.m_grid.ContextMenuStyle = CType((((SourceGrid2.ContextMenuStyle.ColumnResize Or SourceGrid2.ContextMenuStyle.AutoSize) _
            Or SourceGrid2.ContextMenuStyle.CopyPasteSelection) _
            Or SourceGrid2.ContextMenuStyle.CellContextMenu), SourceGrid2.ContextMenuStyle)
        Me.m_grid.CustomSort = False
        Me.m_grid.DataName = "MSE_results"
        Me.m_grid.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_grid.FixedColumnWidths = True
        Me.m_grid.FocusStyle = SourceGrid2.FocusStyle.None
        Me.m_grid.GridToolTipActive = True
        Me.m_grid.GridType = ScientificInterface.gridRiskResults.eGridType.Group
        Me.m_grid.IsLayoutSuspended = False
        
        Me.m_grid.Location = New System.Drawing.Point(3, 28)
        Me.m_grid.Name = "m_grid"
        Me.m_grid.Size = New System.Drawing.Size(633, 394)
        Me.m_grid.SpecialKeys = CType((((((((((SourceGrid2.GridSpecialKeys.Ctrl_C Or SourceGrid2.GridSpecialKeys.Ctrl_V) _
            Or SourceGrid2.GridSpecialKeys.Ctrl_X) _
            Or SourceGrid2.GridSpecialKeys.Delete) _
            Or SourceGrid2.GridSpecialKeys.Arrows) _
            Or SourceGrid2.GridSpecialKeys.Tab) _
            Or SourceGrid2.GridSpecialKeys.PageDownUp) _
            Or SourceGrid2.GridSpecialKeys.Enter) _
            Or SourceGrid2.GridSpecialKeys.Escape) _
            Or SourceGrid2.GridSpecialKeys.Backspace), SourceGrid2.GridSpecialKeys)
        Me.m_grid.TabIndex = 4
        Me.m_grid.UIContext = Nothing
        '
        'm_tlpAll
        '
        Me.m_tlpAll.ColumnCount = 1
        Me.m_tlpAll.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.m_tlpAll.Controls.Add(Me.m_grid, 0, 1)
        Me.m_tlpAll.Controls.Add(Me.m_tsOptions, 0, 0)
        Me.m_tlpAll.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_tlpAll.Location = New System.Drawing.Point(0, 0)
        Me.m_tlpAll.Name = "m_tlpAll"
        Me.m_tlpAll.RowCount = 2
        Me.m_tlpAll.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.m_tlpAll.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.m_tlpAll.Size = New System.Drawing.Size(639, 425)
        Me.m_tlpAll.TabIndex = 6
        '
        'm_tsOptions
        '
        Me.m_tsOptions.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tslViewAs, Me.m_tsbnGroup, Me.m_tsbnFleet})
        Me.m_tsOptions.Location = New System.Drawing.Point(0, 0)
        Me.m_tsOptions.Name = "m_tsOptions"
        Me.m_tsOptions.Size = New System.Drawing.Size(639, 25)
        Me.m_tsOptions.TabIndex = 5
        Me.m_tsOptions.Text = "ToolStrip1"
        '
        'm_tslViewAs
        '
        Me.m_tslViewAs.Name = "m_tslViewAs"
        Me.m_tslViewAs.Size = New System.Drawing.Size(35, 22)
        Me.m_tslViewAs.Text = "&View:"
        '
        'm_tsbnGroup
        '
        Me.m_tsbnGroup.CheckOnClick = True
        Me.m_tsbnGroup.Image = CType(resources.GetObject("m_tsbnGroup.Image"), System.Drawing.Image)
        Me.m_tsbnGroup.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.m_tsbnGroup.Name = "m_tsbnGroup"
        Me.m_tsbnGroup.Size = New System.Drawing.Size(60, 22)
        Me.m_tsbnGroup.Text = "Group"
        '
        'm_tsbnFleet
        '
        Me.m_tsbnFleet.CheckOnClick = True
        Me.m_tsbnFleet.Image = CType(resources.GetObject("m_tsbnFleet.Image"), System.Drawing.Image)
        Me.m_tsbnFleet.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.m_tsbnFleet.Name = "m_tsbnFleet"
        Me.m_tsbnFleet.Size = New System.Drawing.Size(52, 22)
        Me.m_tsbnFleet.Text = "Fleet"
        '
        'frmMSEResults
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.ClientSize = New System.Drawing.Size(639, 425)
        Me.Controls.Add(Me.m_tlpAll)
        Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmMSEResults"
        Me.TabText = ""
        Me.Text = "MSE results"
        Me.m_tlpAll.ResumeLayout(False)
        Me.m_tlpAll.PerformLayout()
        Me.m_tsOptions.ResumeLayout(False)
        Me.m_tsOptions.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Private WithEvents m_grid As ScientificInterface.gridRiskResults
    Private WithEvents m_tlpAll As TableLayoutPanel
    Private WithEvents m_tsOptions As cEwEToolstrip
    Private WithEvents m_tslViewAs As ToolStripLabel
    Private WithEvents m_tsbnGroup As ToolStripButton
    Private WithEvents m_tsbnFleet As ToolStripButton
End Class
