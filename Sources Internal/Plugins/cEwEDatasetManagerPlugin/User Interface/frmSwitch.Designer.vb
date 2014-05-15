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
' Copyright 1991- UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'
Imports ScientificInterfaceShared.Forms

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmSwitch
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmSwitch))
        Me.m_cmbDatasets = New System.Windows.Forms.ComboBox()
        Me.m_btnCancel = New System.Windows.Forms.Button()
        Me.m_btnOK = New System.Windows.Forms.Button()
        Me.m_lblDrop = New ScientificInterfaceShared.Controls.cFileDropLabel()
        Me.SuspendLayout()
        '
        'm_cmbDatasets
        '
        resources.ApplyResources(Me.m_cmbDatasets, "m_cmbDatasets")
        Me.m_cmbDatasets.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.m_cmbDatasets.FormattingEnabled = True
        Me.m_cmbDatasets.Name = "m_cmbDatasets"
        Me.m_cmbDatasets.Sorted = True
        '
        'm_btnCancel
        '
        resources.ApplyResources(Me.m_btnCancel, "m_btnCancel")
        Me.m_btnCancel.Name = "m_btnCancel"
        Me.m_btnCancel.UseVisualStyleBackColor = True
        '
        'm_btnOK
        '
        resources.ApplyResources(Me.m_btnOK, "m_btnOK")
        Me.m_btnOK.Name = "m_btnOK"
        Me.m_btnOK.UseVisualStyleBackColor = True
        '
        'm_lblDrop
        '
        Me.m_lblDrop.AllowDrop = True
        resources.ApplyResources(Me.m_lblDrop, "m_lblDrop")
        Me.m_lblDrop.BackColor = System.Drawing.Color.Transparent
        Me.m_lblDrop.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.m_lblDrop.FileExtensions = ".xml"
        Me.m_lblDrop.ForeColor = System.Drawing.SystemColors.ButtonShadow
        Me.m_lblDrop.MaxFiles = 1
        Me.m_lblDrop.Name = "m_lblDrop"
        '
        'frmSwitch
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.m_lblDrop)
        Me.Controls.Add(Me.m_btnOK)
        Me.Controls.Add(Me.m_btnCancel)
        Me.Controls.Add(Me.m_cmbDatasets)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmSwitch"
        Me.ShowInTaskbar = False
        Me.ResumeLayout(False)

    End Sub
    Private WithEvents m_btnCancel As System.Windows.Forms.Button
    Private WithEvents m_btnOK As System.Windows.Forms.Button
    Private WithEvents m_cmbDatasets As System.Windows.Forms.ComboBox
    Private WithEvents m_lblDrop As ScientificInterfaceShared.Controls.cFileDropLabel
End Class
