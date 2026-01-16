' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 3 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see https://www.gnu.org/licenses/gpl-3.0.html>. 
'
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

Imports SharedResources = ScientificInterfaceShared.My.Resources

Namespace Other

    Partial Class ucOptionsAutoRun
        Inherits System.Windows.Forms.UserControl

        'Required by the Windows Form Designer
        Private components As System.ComponentModel.IContainer

        'NOTE: The following procedure is required by the Windows Form Designer
        'It can be modified using the Windows Form Designer.  
        'Do not modify it using the code editor.
        <System.Diagnostics.DebuggerStepThrough()>
        Private Sub InitializeComponent()
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ucOptionsAutoRun))
            Me.m_hdrMain = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
            Me.m_tsQuickEdit = New ScientificInterfaceShared.Controls.cEwEToolstrip()
            Me.m_tsbnQuickHelp = New System.Windows.Forms.ToolStripButton()
            Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
            Me.m_tsbnAll = New System.Windows.Forms.ToolStripButton()
            Me.m_tsbnNone = New System.Windows.Forms.ToolStripButton()
            Me.m_grid = New ScientificInterface.gridAutoRun()
            Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
            Me.m_lblInfo = New System.Windows.Forms.Label()
            Me.m_tsQuickEdit.SuspendLayout()
            Me.TableLayoutPanel1.SuspendLayout()
            Me.SuspendLayout()
            '
            'm_hdrMain
            '
            Me.m_hdrMain.CanCollapseParent = False
            Me.m_hdrMain.CollapsedParentHeight = 0
            resources.ApplyResources(Me.m_hdrMain, "m_hdrMain")
            Me.m_hdrMain.IsCollapsed = False
            Me.m_hdrMain.Name = "m_hdrMain"
            '
            'm_tsQuickEdit
            '
            resources.ApplyResources(Me.m_tsQuickEdit, "m_tsQuickEdit")
            Me.m_tsQuickEdit.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
            Me.m_tsQuickEdit.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tsbnQuickHelp, Me.ToolStripSeparator1, Me.m_tsbnAll, Me.m_tsbnNone})
            Me.m_tsQuickEdit.Name = "m_tsQuickEdit"
            '
            'm_tsbnQuickHelp
            '
            Me.m_tsbnQuickHelp.AutoToolTip = False
            Me.m_tsbnQuickHelp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
            resources.ApplyResources(Me.m_tsbnQuickHelp, "m_tsbnQuickHelp")
            Me.m_tsbnQuickHelp.Name = "m_tsbnQuickHelp"
            '
            'ToolStripSeparator1
            '
            Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
            resources.ApplyResources(Me.ToolStripSeparator1, "ToolStripSeparator1")
            '
            'm_tsbnAll
            '
            Me.m_tsbnAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            resources.ApplyResources(Me.m_tsbnAll, "m_tsbnAll")
            Me.m_tsbnAll.Name = "m_tsbnAll"
            '
            'm_tsbnNone
            '
            Me.m_tsbnNone.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            resources.ApplyResources(Me.m_tsbnNone, "m_tsbnNone")
            Me.m_tsbnNone.Name = "m_tsbnNone"
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
            Me.m_grid.DataName = "grid content"
            resources.ApplyResources(Me.m_grid, "m_grid")
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
            'TableLayoutPanel1
            '
            resources.ApplyResources(Me.TableLayoutPanel1, "TableLayoutPanel1")
            Me.TableLayoutPanel1.Controls.Add(Me.m_lblInfo, 0, 2)
            Me.TableLayoutPanel1.Controls.Add(Me.m_hdrMain, 0, 0)
            Me.TableLayoutPanel1.Controls.Add(Me.m_grid, 0, 3)
            Me.TableLayoutPanel1.Controls.Add(Me.m_tsQuickEdit, 0, 1)
            Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
            '
            'm_lblInfo
            '
            resources.ApplyResources(Me.m_lblInfo, "m_lblInfo")
            Me.m_lblInfo.Name = "m_lblInfo"
            '
            'ucOptionsAutoRun
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
            Me.Controls.Add(Me.TableLayoutPanel1)
            Me.Name = "ucOptionsAutoRun"
            Me.m_tsQuickEdit.ResumeLayout(False)
            Me.m_tsQuickEdit.PerformLayout()
            Me.TableLayoutPanel1.ResumeLayout(False)
            Me.TableLayoutPanel1.PerformLayout()
            Me.ResumeLayout(False)

        End Sub
        Private WithEvents m_hdrMain As ScientificInterfaceShared.Controls.cEwEHeaderLabel
        Private WithEvents m_tsQuickEdit As cEwEToolstrip
        Private WithEvents m_grid As gridAutoRun
        Private WithEvents m_tsbnAll As ToolStripButton
        Private WithEvents m_tsbnNone As ToolStripButton
        Friend WithEvents TableLayoutPanel1 As TableLayoutPanel
        Friend WithEvents ToolStripSeparator1 As ToolStripSeparator
        Private WithEvents m_lblInfo As Label
        Private WithEvents m_tsbnQuickHelp As ToolStripButton
    End Class

End Namespace
