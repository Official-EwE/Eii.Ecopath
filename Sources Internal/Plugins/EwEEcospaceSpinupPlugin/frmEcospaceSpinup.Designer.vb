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
'    UBC Institute for the Oceans and Fisheries, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

Partial Class frmEcospaceSpinup
    Inherits ScientificInterfaceShared.Forms.frmEwEGrid

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmEcospaceSpinup))
        Me.m_chkUseSpinup = New System.Windows.Forms.CheckBox()
        Me.m_tbxSpinUpYears = New System.Windows.Forms.TextBox()
        Me.m_lblSpinUpYears = New System.Windows.Forms.Label()
        Me.m_chkUseBaseBio = New System.Windows.Forms.CheckBox()
        Me.m_gridSpinUpDif = New EwEEcospaceSpinupPlugin.gridSpinupDiff(Me.components)
        Me.m_tlpContent = New System.Windows.Forms.TableLayoutPanel()
        Me.m_plControls = New System.Windows.Forms.Panel()
        Me.m_hdr = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.m_tlpContent.SuspendLayout()
        Me.m_plControls.SuspendLayout()
        Me.SuspendLayout()
        '
        'm_chkUseSpinup
        '
        resources.ApplyResources(Me.m_chkUseSpinup, "m_chkUseSpinup")
        Me.m_chkUseSpinup.Name = "m_chkUseSpinup"
        Me.m_chkUseSpinup.UseVisualStyleBackColor = True
        '
        'm_tbxSpinUpYears
        '
        resources.ApplyResources(Me.m_tbxSpinUpYears, "m_tbxSpinUpYears")
        Me.m_tbxSpinUpYears.Name = "m_tbxSpinUpYears"
        '
        'm_lblSpinUpYears
        '
        resources.ApplyResources(Me.m_lblSpinUpYears, "m_lblSpinUpYears")
        Me.m_lblSpinUpYears.Name = "m_lblSpinUpYears"
        '
        'm_chkUseBaseBio
        '
        resources.ApplyResources(Me.m_chkUseBaseBio, "m_chkUseBaseBio")
        Me.m_chkUseBaseBio.Name = "m_chkUseBaseBio"
        Me.m_chkUseBaseBio.TabStop = False
        Me.m_chkUseBaseBio.UseVisualStyleBackColor = True
        '
        'm_gridSpinUpDif
        '
        Me.m_gridSpinUpDif.AllowBlockSelect = True
        resources.ApplyResources(Me.m_gridSpinUpDif, "m_gridSpinUpDif")
        Me.m_gridSpinUpDif.AutoSizeMinHeight = 10
        Me.m_gridSpinUpDif.AutoSizeMinWidth = 10
        Me.m_gridSpinUpDif.AutoStretchColumnsToFitWidth = False
        Me.m_gridSpinUpDif.AutoStretchRowsToFitHeight = False
        Me.m_gridSpinUpDif.BackColor = System.Drawing.Color.White
        Me.m_gridSpinUpDif.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.m_gridSpinUpDif.ContextMenuStyle = CType((((SourceGrid2.ContextMenuStyle.ColumnResize Or SourceGrid2.ContextMenuStyle.AutoSize) _
            Or SourceGrid2.ContextMenuStyle.CopyPasteSelection) _
            Or SourceGrid2.ContextMenuStyle.CellContextMenu), SourceGrid2.ContextMenuStyle)
        Me.m_gridSpinUpDif.CustomSort = False
        Me.m_gridSpinUpDif.DataName = "EcospaceSpinUp"
        Me.m_gridSpinUpDif.FixedColumnWidths = False
        Me.m_gridSpinUpDif.FocusStyle = SourceGrid2.FocusStyle.None
        Me.m_gridSpinUpDif.GridToolTipActive = True
        Me.m_gridSpinUpDif.IsLayoutSuspended = False
        Me.m_gridSpinUpDif.IsOutputGrid = True
        Me.m_gridSpinUpDif.Name = "m_gridSpinUpDif"
        Me.m_gridSpinUpDif.SpecialKeys = CType((((((((((SourceGrid2.GridSpecialKeys.Ctrl_C Or SourceGrid2.GridSpecialKeys.Ctrl_V) _
            Or SourceGrid2.GridSpecialKeys.Ctrl_X) _
            Or SourceGrid2.GridSpecialKeys.Delete) _
            Or SourceGrid2.GridSpecialKeys.Arrows) _
            Or SourceGrid2.GridSpecialKeys.Tab) _
            Or SourceGrid2.GridSpecialKeys.PageDownUp) _
            Or SourceGrid2.GridSpecialKeys.Enter) _
            Or SourceGrid2.GridSpecialKeys.Escape) _
            Or SourceGrid2.GridSpecialKeys.Backspace), SourceGrid2.GridSpecialKeys)
        Me.m_gridSpinUpDif.TrackPropertySelection = False
        Me.m_gridSpinUpDif.UIContext = Nothing
        '
        'm_tlpContent
        '
        resources.ApplyResources(Me.m_tlpContent, "m_tlpContent")
        Me.m_tlpContent.Controls.Add(Me.m_gridSpinUpDif, 0, 1)
        Me.m_tlpContent.Controls.Add(Me.m_plControls, 0, 0)
        Me.m_tlpContent.Name = "m_tlpContent"
        '
        'm_plControls
        '
        Me.m_plControls.Controls.Add(Me.m_hdr)
        Me.m_plControls.Controls.Add(Me.m_chkUseSpinup)
        Me.m_plControls.Controls.Add(Me.m_tbxSpinUpYears)
        Me.m_plControls.Controls.Add(Me.m_chkUseBaseBio)
        Me.m_plControls.Controls.Add(Me.m_lblSpinUpYears)
        resources.ApplyResources(Me.m_plControls, "m_plControls")
        Me.m_plControls.Name = "m_plControls"
        '
        'm_hdr
        '
        Me.m_hdr.CanCollapseParent = False
        Me.m_hdr.CollapsedParentHeight = 0
        resources.ApplyResources(Me.m_hdr, "m_hdr")
        Me.m_hdr.IsCollapsed = False
        Me.m_hdr.Name = "m_hdr"
        '
        'frmEcospaceSpinup
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.ControlBox = False
        Me.Controls.Add(Me.m_tlpContent)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "frmEcospaceSpinup"
        Me.ShowInTaskbar = False
        Me.TabText = "Ecospace spin-up"
        Me.m_tlpContent.ResumeLayout(False)
        Me.m_plControls.ResumeLayout(False)
        Me.m_plControls.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Private WithEvents m_chkUseSpinup As System.Windows.Forms.CheckBox
    Private WithEvents m_tbxSpinUpYears As Windows.Forms.TextBox
    Private WithEvents m_lblSpinUpYears As Windows.Forms.Label
    Private WithEvents m_gridSpinUpDif As gridSpinupDiff
    Private WithEvents m_tlpContent As Windows.Forms.TableLayoutPanel
    Private WithEvents m_plControls As Windows.Forms.Panel
    Private WithEvents m_chkUseBaseBio As Windows.Forms.CheckBox
    Friend WithEvents m_hdr As ScientificInterfaceShared.Controls.cEwEHeaderLabel
End Class
