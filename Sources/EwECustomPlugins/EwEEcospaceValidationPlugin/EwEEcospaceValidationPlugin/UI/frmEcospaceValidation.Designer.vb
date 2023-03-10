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

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmEcospaceValidation
    Inherits frmEwE

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
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmEcospaceValidation))
        Me.m_tlpMain = New System.Windows.Forms.TableLayoutPanel()
        Me.m_plTimestep = New System.Windows.Forms.Panel()
        Me.m_slTimestep = New ScientificInterfaceShared.Controls.ucSlider()
        Me.m_nudTimeStep = New System.Windows.Forms.NumericUpDown()
        Me.m_lblTimeStep = New System.Windows.Forms.Label()
        Me.m_grid = New EwEEcospaceValidationPlugin.gridPredPreyOverlap()
        Me.m_plRegions = New System.Windows.Forms.Panel()
        Me.m_slRegion = New ScientificInterfaceShared.Controls.ucSlider()
        Me.m_nudRegion = New System.Windows.Forms.NumericUpDown()
        Me.m_lblRegion = New System.Windows.Forms.Label()
        Me.m_tlpMain.SuspendLayout()
        Me.m_plTimestep.SuspendLayout()
        CType(Me.m_nudTimeStep, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.m_plRegions.SuspendLayout()
        CType(Me.m_nudRegion, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'm_tlpMain
        '
        resources.ApplyResources(Me.m_tlpMain, "m_tlpMain")
        Me.m_tlpMain.Controls.Add(Me.m_plTimestep, 0, 0)
        Me.m_tlpMain.Controls.Add(Me.m_grid, 0, 2)
        Me.m_tlpMain.Controls.Add(Me.m_plRegions, 0, 1)
        Me.m_tlpMain.Name = "m_tlpMain"
        '
        'm_plTimestep
        '
        Me.m_plTimestep.Controls.Add(Me.m_slTimestep)
        Me.m_plTimestep.Controls.Add(Me.m_nudTimeStep)
        Me.m_plTimestep.Controls.Add(Me.m_lblTimeStep)
        resources.ApplyResources(Me.m_plTimestep, "m_plTimestep")
        Me.m_plTimestep.Name = "m_plTimestep"
        '
        'm_slTimestep
        '
        resources.ApplyResources(Me.m_slTimestep, "m_slTimestep")
        Me.m_slTimestep.CurrentKnob = 0
        Me.m_slTimestep.Maximum = 100
        Me.m_slTimestep.Minimum = 0
        Me.m_slTimestep.Name = "m_slTimestep"
        Me.m_slTimestep.NumKnobs = 1
        '
        'm_nudTimeStep
        '
        resources.ApplyResources(Me.m_nudTimeStep, "m_nudTimeStep")
        Me.m_nudTimeStep.Name = "m_nudTimeStep"
        '
        'm_lblTimeStep
        '
        resources.ApplyResources(Me.m_lblTimeStep, "m_lblTimeStep")
        Me.m_lblTimeStep.Name = "m_lblTimeStep"
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
        'm_plRegions
        '
        Me.m_plRegions.Controls.Add(Me.m_slRegion)
        Me.m_plRegions.Controls.Add(Me.m_nudRegion)
        Me.m_plRegions.Controls.Add(Me.m_lblRegion)
        resources.ApplyResources(Me.m_plRegions, "m_plRegions")
        Me.m_plRegions.Name = "m_plRegions"
        '
        'm_slRegion
        '
        resources.ApplyResources(Me.m_slRegion, "m_slRegion")
        Me.m_slRegion.CurrentKnob = 0
        Me.m_slRegion.Maximum = 100
        Me.m_slRegion.Minimum = 0
        Me.m_slRegion.Name = "m_slRegion"
        Me.m_slRegion.NumKnobs = 1
        '
        'm_nudRegion
        '
        resources.ApplyResources(Me.m_nudRegion, "m_nudRegion")
        Me.m_nudRegion.Name = "m_nudRegion"
        '
        'm_lblRegion
        '
        resources.ApplyResources(Me.m_lblRegion, "m_lblRegion")
        Me.m_lblRegion.Name = "m_lblRegion"
        '
        'frmEcospaceValidation
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.m_tlpMain)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "frmEcospaceValidation"
        Me.TabText = ""
        Me.m_tlpMain.ResumeLayout(False)
        Me.m_plTimestep.ResumeLayout(False)
        Me.m_plTimestep.PerformLayout()
        CType(Me.m_nudTimeStep, System.ComponentModel.ISupportInitialize).EndInit()
        Me.m_plRegions.ResumeLayout(False)
        Me.m_plRegions.PerformLayout()
        CType(Me.m_nudRegion, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Private WithEvents m_tlpMain As Windows.Forms.TableLayoutPanel
    Private WithEvents m_plTimestep As Windows.Forms.Panel
    Private WithEvents m_slTimestep As ScientificInterfaceShared.Controls.ucSlider
    Private WithEvents m_nudTimeStep As Windows.Forms.NumericUpDown
    Private WithEvents m_lblTimeStep As Windows.Forms.Label
    Private WithEvents m_grid As gridPredPreyOverlap
    Private WithEvents m_slRegion As ScientificInterfaceShared.Controls.ucSlider
    Private WithEvents m_nudRegion As Windows.Forms.NumericUpDown
    Private WithEvents m_lblRegion As Windows.Forms.Label
    Private WithEvents m_plRegions As Windows.Forms.Panel
End Class
