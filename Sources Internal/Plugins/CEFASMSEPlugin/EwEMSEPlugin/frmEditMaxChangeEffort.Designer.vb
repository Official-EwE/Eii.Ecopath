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
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.btnOK = New System.Windows.Forms.Button()
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
        'btnCancel
        '
        resources.ApplyResources(Me.btnCancel, "btnCancel")
        Me.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'btnOK
        '
        resources.ApplyResources(Me.btnOK, "btnOK")
        Me.btnOK.Name = "btnOK"
        Me.btnOK.UseVisualStyleBackColor = True
        '
        'frmEditDecreaseEffort
        '
        Me.AcceptButton = Me.btnOK
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.btnCancel
        Me.Controls.Add(Me.btnOK)
        Me.Controls.Add(Me.btnCancel)
        Me.Controls.Add(Me.dgvMaxDecreaseEffort)
        Me.Name = "frmEditDecreaseEffort"
        CType(Me.dgvMaxDecreaseEffort, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents FleetNumber As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents FleetName As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents MaxDecrease As System.Windows.Forms.DataGridViewTextBoxColumn
    Private WithEvents btnCancel As System.Windows.Forms.Button
    Private WithEvents btnOK As System.Windows.Forms.Button
    Private WithEvents dgvMaxDecreaseEffort As System.Windows.Forms.DataGridView
End Class
