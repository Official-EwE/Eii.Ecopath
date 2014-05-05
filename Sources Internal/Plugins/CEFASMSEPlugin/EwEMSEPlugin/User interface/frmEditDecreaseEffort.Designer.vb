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

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmEditDecreaseEffort
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmEditDecreaseEffort))
        Me.dgvMaxDecreaseEffort = New System.Windows.Forms.DataGridView()
        Me.FleetNumber = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.FleetName = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.MaxDecrease = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.m_btnCancel = New System.Windows.Forms.Button()
        Me.m_btnOK = New System.Windows.Forms.Button()
        Me.GridMaxDecreaseEffort1 = New EwEMSEPlugin.gridMaxDecreaseEffort()
        CType(Me.dgvMaxDecreaseEffort, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'dgvMaxDecreaseEffort
        '
        Me.dgvMaxDecreaseEffort.AllowUserToAddRows = False
        Me.dgvMaxDecreaseEffort.AllowUserToDeleteRows = False
        Me.dgvMaxDecreaseEffort.AllowUserToResizeRows = False
        resources.ApplyResources(Me.dgvMaxDecreaseEffort, "dgvMaxDecreaseEffort")
        Me.dgvMaxDecreaseEffort.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvMaxDecreaseEffort.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.FleetNumber, Me.FleetName, Me.MaxDecrease})
        Me.dgvMaxDecreaseEffort.Name = "dgvMaxDecreaseEffort"
        Me.dgvMaxDecreaseEffort.RowHeadersVisible = False
        '
        'FleetNumber
        '
        resources.ApplyResources(Me.FleetNumber, "FleetNumber")
        Me.FleetNumber.Name = "FleetNumber"
        '
        'FleetName
        '
        resources.ApplyResources(Me.FleetName, "FleetName")
        Me.FleetName.Name = "FleetName"
        '
        'MaxDecrease
        '
        resources.ApplyResources(Me.MaxDecrease, "MaxDecrease")
        Me.MaxDecrease.Name = "MaxDecrease"
        '
        'm_btnCancel
        '
        resources.ApplyResources(Me.m_btnCancel, "m_btnCancel")
        Me.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.m_btnCancel.Name = "m_btnCancel"
        Me.m_btnCancel.UseVisualStyleBackColor = True
        '
        'm_btnOK
        '
        resources.ApplyResources(Me.m_btnOK, "m_btnOK")
        Me.m_btnOK.Name = "m_btnOK"
        Me.m_btnOK.UseVisualStyleBackColor = True
        '
        'GridMaxDecreaseEffort1
        '
        Me.GridMaxDecreaseEffort1.AllowBlockSelect = False
        Me.GridMaxDecreaseEffort1.AutoSizeMinHeight = 10
        Me.GridMaxDecreaseEffort1.AutoSizeMinWidth = 10
        Me.GridMaxDecreaseEffort1.AutoStretchColumnsToFitWidth = True
        Me.GridMaxDecreaseEffort1.AutoStretchRowsToFitHeight = False
        Me.GridMaxDecreaseEffort1.BackColor = System.Drawing.Color.White
        Me.GridMaxDecreaseEffort1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.GridMaxDecreaseEffort1.ContextMenuStyle = CType((((SourceGrid2.ContextMenuStyle.ColumnResize Or SourceGrid2.ContextMenuStyle.AutoSize) _
            Or SourceGrid2.ContextMenuStyle.CopyPasteSelection) _
            Or SourceGrid2.ContextMenuStyle.CellContextMenu), SourceGrid2.ContextMenuStyle)
        Me.GridMaxDecreaseEffort1.CustomSort = False
        Me.GridMaxDecreaseEffort1.Data = Nothing
        Me.GridMaxDecreaseEffort1.DataName = "grid content"
        Me.GridMaxDecreaseEffort1.FixedColumnWidths = False
        Me.GridMaxDecreaseEffort1.FocusStyle = SourceGrid2.FocusStyle.None
        Me.GridMaxDecreaseEffort1.GridToolTipActive = True
        Me.GridMaxDecreaseEffort1.IsLayoutSuspended = False
        resources.ApplyResources(Me.GridMaxDecreaseEffort1, "GridMaxDecreaseEffort1")
        Me.GridMaxDecreaseEffort1.Name = "GridMaxDecreaseEffort1"
        Me.GridMaxDecreaseEffort1.SpecialKeys = CType((((((((((SourceGrid2.GridSpecialKeys.Ctrl_C Or SourceGrid2.GridSpecialKeys.Ctrl_V) _
            Or SourceGrid2.GridSpecialKeys.Ctrl_X) _
            Or SourceGrid2.GridSpecialKeys.Delete) _
            Or SourceGrid2.GridSpecialKeys.Arrows) _
            Or SourceGrid2.GridSpecialKeys.Tab) _
            Or SourceGrid2.GridSpecialKeys.PageDownUp) _
            Or SourceGrid2.GridSpecialKeys.Enter) _
            Or SourceGrid2.GridSpecialKeys.Escape) _
            Or SourceGrid2.GridSpecialKeys.Backspace), SourceGrid2.GridSpecialKeys)
        Me.GridMaxDecreaseEffort1.UIContext = Nothing
        '
        'frmEditDecreaseEffort
        '
        Me.AcceptButton = Me.m_btnOK
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.m_btnCancel
        Me.ControlBox = False
        Me.Controls.Add(Me.GridMaxDecreaseEffort1)
        Me.Controls.Add(Me.m_btnOK)
        Me.Controls.Add(Me.m_btnCancel)
        Me.Controls.Add(Me.dgvMaxDecreaseEffort)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmEditDecreaseEffort"
        Me.ShowInTaskbar = False
        CType(Me.dgvMaxDecreaseEffort, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents FleetNumber As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents FleetName As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents MaxDecrease As System.Windows.Forms.DataGridViewTextBoxColumn
    Private WithEvents m_btnCancel As System.Windows.Forms.Button
    Private WithEvents m_btnOK As System.Windows.Forms.Button
    Private WithEvents dgvMaxDecreaseEffort As System.Windows.Forms.DataGridView
    Private WithEvents m_grid As gridDistributionParameters
    Private WithEvents GridMaxDecreaseEffort1 As EwEMSEPlugin.gridMaxDecreaseEffort
End Class
