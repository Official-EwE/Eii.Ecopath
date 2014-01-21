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

Partial Class frmRemarkPanel
    Inherits frmEwEDockContent

    Private components As System.ComponentModel.IContainer

    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmRemarkPanel))
        Me.m_tsRemarks = New ScientificInterfaceShared.Controls.cEwEToolstrip()
        Me.m_lblVarName = New System.Windows.Forms.ToolStripLabel()
        Me.m_btnApply = New System.Windows.Forms.ToolStripButton()
        Me.m_tbxRemark = New System.Windows.Forms.TextBox()
        Me.m_tsRemarks.SuspendLayout()
        Me.SuspendLayout()
        '
        'm_tsRemarks
        '
        Me.m_tsRemarks.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
        Me.m_tsRemarks.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_lblVarName, Me.m_btnApply})
        resources.ApplyResources(Me.m_tsRemarks, "m_tsRemarks")
        Me.m_tsRemarks.Name = "m_tsRemarks"
        Me.m_tsRemarks.RenderMode = System.Windows.Forms.ToolStripRenderMode.System
        '
        'm_lblVarName
        '
        Me.m_lblVarName.Name = "m_lblVarName"
        resources.ApplyResources(Me.m_lblVarName, "m_lblVarName")
        '
        'm_btnApply
        '
        Me.m_btnApply.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.m_btnApply.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        resources.ApplyResources(Me.m_btnApply, "m_btnApply")
        Me.m_btnApply.Name = "m_btnApply"
        '
        'm_tbxRemark
        '
        Me.m_tbxRemark.AcceptsReturn = True
        Me.m_tbxRemark.AcceptsTab = True
        resources.ApplyResources(Me.m_tbxRemark, "m_tbxRemark")
        Me.m_tbxRemark.Name = "m_tbxRemark"
        '
        'frmRemarkPanel
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CloseButtonVisible = False
        Me.Controls.Add(Me.m_tbxRemark)
        Me.Controls.Add(Me.m_tsRemarks)
        Me.DockAreas = CType((((WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft Or WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight) _
            Or WeifenLuo.WinFormsUI.Docking.DockAreas.DockTop) _
            Or WeifenLuo.WinFormsUI.Docking.DockAreas.DockBottom), WeifenLuo.WinFormsUI.Docking.DockAreas)
        Me.HideOnClose = True
        Me.Name = "frmRemarkPanel"
        Me.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockRightAutoHide
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.TabText = "Remarks"
        Me.m_tsRemarks.ResumeLayout(False)
        Me.m_tsRemarks.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Private WithEvents m_tsRemarks As cEwEToolstrip
    Private WithEvents m_lblVarName As System.Windows.Forms.ToolStripLabel
    Private WithEvents m_btnApply As System.Windows.Forms.ToolStripButton
    Private WithEvents m_tbxRemark As System.Windows.Forms.TextBox
End Class
