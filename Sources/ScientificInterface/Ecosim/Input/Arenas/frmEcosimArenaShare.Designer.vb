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
#Region " Imports "

Option Strict On

#End Region ' Imports

Namespace Ecosim

    Partial Class frmEcosimArenaShare
        Inherits frmEwEGrid

        'Form overrides dispose to clean up the component list.
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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmEcosimArenaShare))
            Me.m_tsMain = New ScientificInterfaceShared.Controls.cEwEToolstrip()
            Me.m_tsbnSumToOne = New System.Windows.Forms.ToolStripButton()
            Me.m_tsbnReset = New System.Windows.Forms.ToolStripButton()
            Me.m_tlpContent = New System.Windows.Forms.TableLayoutPanel()
            Me.m_groups = New ScientificInterfaceShared.Controls.cGroupListBox()
            Me.m_grid = New ScientificInterface.Ecosim.gridEcosimArenaShare()
            Me.m_hdrPrey = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
            Me.m_hdrArenas = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
            Me.m_tsMain.SuspendLayout()
            Me.m_tlpContent.SuspendLayout()
            Me.SuspendLayout()
            '
            'm_tsMain
            '
            Me.m_tsMain.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
            Me.m_tsMain.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tsbnSumToOne, Me.m_tsbnReset})
            resources.ApplyResources(Me.m_tsMain, "m_tsMain")
            Me.m_tsMain.Name = "m_tsMain"
            '
            'm_tsbnSumToOne
            '
            resources.ApplyResources(Me.m_tsbnSumToOne, "m_tsbnSumToOne")
            Me.m_tsbnSumToOne.Name = "m_tsbnSumToOne"
            '
            'm_tsbnReset
            '
            resources.ApplyResources(Me.m_tsbnReset, "m_tsbnReset")
            Me.m_tsbnReset.Name = "m_tsbnReset"
            '
            'm_tlpContent
            '
            resources.ApplyResources(Me.m_tlpContent, "m_tlpContent")
            Me.m_tlpContent.Controls.Add(Me.m_groups, 0, 1)
            Me.m_tlpContent.Controls.Add(Me.m_grid, 1, 1)
            Me.m_tlpContent.Controls.Add(Me.m_hdrPrey, 0, 0)
            Me.m_tlpContent.Controls.Add(Me.m_hdrArenas, 1, 0)
            Me.m_tlpContent.Name = "m_tlpContent"
            '
            'm_groups
            '
            Me.m_groups.AllGroupsItemColor = System.Drawing.Color.Transparent
            Me.m_groups.AllGroupsItemText = "(all)"
            resources.ApplyResources(Me.m_groups, "m_groups")
            Me.m_groups.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed
            Me.m_groups.FormattingEnabled = True
            Me.m_groups.GroupDisplayStyle = ScientificInterfaceShared.Controls.cGroupListBox.eGroupDisplayStyleTypes.DisplayVisibleOnly
            Me.m_groups.GroupListTracking = ScientificInterfaceShared.Controls.cGroupListBox.eGroupTrackingType.Manual
            Me.m_groups.IsAllGroupsItemSelected = False
            Me.m_groups.Name = "m_groups"
            Me.m_groups.SelectedGroup = Nothing
            Me.m_groups.SelectedGroupIndex = -1
            Me.m_groups.ShowAllGroupsItem = False
            Me.m_groups.SortThreshold = -9999.0!
            Me.m_groups.VisibleGroups = Nothing
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
            Me.m_grid.IsOutputGrid = False
            Me.m_grid.Name = "m_grid"
            Me.m_grid.SelectedGroup = Nothing
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
            'm_hdrPrey
            '
            Me.m_hdrPrey.CanCollapseParent = False
            Me.m_hdrPrey.CollapsedParentHeight = 0
            resources.ApplyResources(Me.m_hdrPrey, "m_hdrPrey")
            Me.m_hdrPrey.IsCollapsed = False
            Me.m_hdrPrey.Name = "m_hdrPrey"
            '
            'm_hdrArenas
            '
            Me.m_hdrArenas.CanCollapseParent = False
            Me.m_hdrArenas.CollapsedParentHeight = 0
            resources.ApplyResources(Me.m_hdrArenas, "m_hdrArenas")
            Me.m_hdrArenas.IsCollapsed = False
            Me.m_hdrArenas.Name = "m_hdrArenas"
            '
            'frmEcosimArenaShare
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.m_tlpContent)
            Me.Controls.Add(Me.m_tsMain)
            Me.Name = "frmEcosimArenaShare"
            Me.TabText = ""
            Me.m_tsMain.ResumeLayout(False)
            Me.m_tsMain.PerformLayout()
            Me.m_tlpContent.ResumeLayout(False)
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub

        Private WithEvents m_tsMain As cEwEToolstrip
        Private WithEvents m_tsbnReset As ToolStripButton
        Private WithEvents m_tlpContent As TableLayoutPanel
        Private WithEvents m_groups As cGroupListBox
        Private WithEvents m_grid As gridEcosimArenaShare
        Private WithEvents m_hdrPrey As cEwEHeaderLabel
        Private WithEvents m_hdrArenas As cEwEHeaderLabel
        Private WithEvents m_tsbnSumToOne As ToolStripButton
    End Class

End Namespace
