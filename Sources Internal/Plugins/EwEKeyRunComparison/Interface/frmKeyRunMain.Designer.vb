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
'    UBC Fisheries Centre, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

Imports ScientificInterfaceShared.Forms

Partial Class frmKeyRunMain
    Inherits frmEwE

    'Form overrides dispose to clean up the component list.
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
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmKeyRunMain))
        Me.m_btnSave = New System.Windows.Forms.Button()
        Me.m_btnLoad = New System.Windows.Forms.Button()
        Me.m_lblKeyRunFile = New System.Windows.Forms.Label()
        Me.m_lbRunStatus = New System.Windows.Forms.Label()
        Me.m_cbShowErrorsOnly = New System.Windows.Forms.CheckBox()
        Me.m_pbStatus = New System.Windows.Forms.PictureBox()
        Me.m_btnCompare = New System.Windows.Forms.Button()
        Me.m_grid = New EwEKeyRunComparisonPlugin.gridKeyRunComparison()
        Me.m_hdrDetails = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        CType(Me.m_pbStatus, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'm_btnSave
        '
        resources.ApplyResources(Me.m_btnSave, "m_btnSave")
        Me.m_btnSave.Name = "m_btnSave"
        Me.m_btnSave.UseVisualStyleBackColor = True
        '
        'm_btnLoad
        '
        resources.ApplyResources(Me.m_btnLoad, "m_btnLoad")
        Me.m_btnLoad.Name = "m_btnLoad"
        Me.m_btnLoad.UseVisualStyleBackColor = True
        '
        'm_lblKeyRunFile
        '
        resources.ApplyResources(Me.m_lblKeyRunFile, "m_lblKeyRunFile")
        Me.m_lblKeyRunFile.Name = "m_lblKeyRunFile"
        '
        'm_lbRunStatus
        '
        resources.ApplyResources(Me.m_lbRunStatus, "m_lbRunStatus")
        Me.m_lbRunStatus.Name = "m_lbRunStatus"
        '
        'm_cbShowErrorsOnly
        '
        resources.ApplyResources(Me.m_cbShowErrorsOnly, "m_cbShowErrorsOnly")
        Me.m_cbShowErrorsOnly.Name = "m_cbShowErrorsOnly"
        Me.m_cbShowErrorsOnly.UseVisualStyleBackColor = True
        '
        'm_pbStatus
        '
        resources.ApplyResources(Me.m_pbStatus, "m_pbStatus")
        Me.m_pbStatus.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.m_pbStatus.Name = "m_pbStatus"
        Me.m_pbStatus.TabStop = False
        '
        'm_btnCompare
        '
        resources.ApplyResources(Me.m_btnCompare, "m_btnCompare")
        Me.m_btnCompare.Name = "m_btnCompare"
        Me.m_btnCompare.UseVisualStyleBackColor = True
        '
        'm_grid
        '
        Me.m_grid.AllowBlockSelect = False
        resources.ApplyResources(Me.m_grid, "m_grid")
        Me.m_grid.AutoSizeMinHeight = 10
        Me.m_grid.AutoSizeMinWidth = 10
        Me.m_grid.AutoStretchColumnsToFitWidth = True
        Me.m_grid.AutoStretchRowsToFitHeight = False
        Me.m_grid.BackColor = System.Drawing.Color.White
        Me.m_grid.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.m_grid.ComparisonManager = Nothing
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
        Me.m_grid.ShowErrorsOnly = False
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
        'm_hdrDetails
        '
        resources.ApplyResources(Me.m_hdrDetails, "m_hdrDetails")
        Me.m_hdrDetails.CanCollapseParent = False
        Me.m_hdrDetails.CollapsedParentHeight = 0
        Me.m_hdrDetails.IsCollapsed = False
        Me.m_hdrDetails.Name = "m_hdrDetails"
        '
        'frmKeyRunMain
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ControlBox = False
        Me.Controls.Add(Me.m_hdrDetails)
        Me.Controls.Add(Me.m_btnCompare)
        Me.Controls.Add(Me.m_pbStatus)
        Me.Controls.Add(Me.m_cbShowErrorsOnly)
        Me.Controls.Add(Me.m_lbRunStatus)
        Me.Controls.Add(Me.m_lblKeyRunFile)
        Me.Controls.Add(Me.m_grid)
        Me.Controls.Add(Me.m_btnSave)
        Me.Controls.Add(Me.m_btnLoad)
        Me.Name = "frmKeyRunMain"
        Me.ShowInTaskbar = False
        Me.TabText = "Key Run Comparison"
        CType(Me.m_pbStatus, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Private WithEvents m_btnSave As System.Windows.Forms.Button
    Private WithEvents m_btnLoad As System.Windows.Forms.Button
    Private WithEvents m_grid As gridKeyRunComparison
    Private WithEvents m_lblKeyRunFile As System.Windows.Forms.Label
    Private WithEvents m_cbShowErrorsOnly As System.Windows.Forms.CheckBox
    Private WithEvents m_pbStatus As System.Windows.Forms.PictureBox
    Private WithEvents m_btnCompare As System.Windows.Forms.Button
    Private WithEvents m_hdrDetails As ScientificInterfaceShared.Controls.cEwEHeaderLabel
    Private WithEvents m_lbRunStatus As System.Windows.Forms.Label
End Class
