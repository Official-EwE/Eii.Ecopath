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

Imports ScientificInterfaceShared.Controls

Partial Class dlgMergeGroups
    Inherits System.Windows.Forms.Form

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
        Me.m_lblAgg1 = New System.Windows.Forms.Label()
        Me.m_cmbTarget = New System.Windows.Forms.ComboBox()
        Me.m_lblAgg2 = New System.Windows.Forms.Label()
        Me.m_lblNew = New System.Windows.Forms.Label()
        Me.m_tbxNewName = New System.Windows.Forms.TextBox()
        Me.m_btnOK = New System.Windows.Forms.Button()
        Me.m_btnCancel = New System.Windows.Forms.Button()
        Me.m_clbGroups = New System.Windows.Forms.CheckedListBox()
        Me.SuspendLayout()
        '
        'm_lblAgg1
        '
        Me.m_lblAgg1.AutoSize = True
        Me.m_lblAgg1.Location = New System.Drawing.Point(12, 15)
        Me.m_lblAgg1.Name = "m_lblAgg1"
        Me.m_lblAgg1.Size = New System.Drawing.Size(41, 13)
        Me.m_lblAgg1.TabIndex = 0
        Me.m_lblAgg1.Text = "&Target:"
        '
        'm_cmbTarget
        '
        Me.m_cmbTarget.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_cmbTarget.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.m_cmbTarget.FormattingEnabled = True
        Me.m_cmbTarget.Location = New System.Drawing.Point(80, 12)
        Me.m_cmbTarget.Name = "m_cmbTarget"
        Me.m_cmbTarget.Size = New System.Drawing.Size(290, 21)
        Me.m_cmbTarget.TabIndex = 1
        '
        'm_lblAgg2
        '
        Me.m_lblAgg2.AutoSize = True
        Me.m_lblAgg2.Location = New System.Drawing.Point(12, 42)
        Me.m_lblAgg2.Name = "m_lblAgg2"
        Me.m_lblAgg2.Size = New System.Drawing.Size(44, 13)
        Me.m_lblAgg2.TabIndex = 0
        Me.m_lblAgg2.Text = "&Groups:"
        '
        'm_lblNew
        '
        Me.m_lblNew.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.m_lblNew.AutoSize = True
        Me.m_lblNew.Location = New System.Drawing.Point(12, 204)
        Me.m_lblNew.Name = "m_lblNew"
        Me.m_lblNew.Size = New System.Drawing.Size(61, 13)
        Me.m_lblNew.TabIndex = 0
        Me.m_lblNew.Text = "&New name:"
        '
        'm_tbxNewName
        '
        Me.m_tbxNewName.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_tbxNewName.Location = New System.Drawing.Point(80, 201)
        Me.m_tbxNewName.Name = "m_tbxNewName"
        Me.m_tbxNewName.Size = New System.Drawing.Size(290, 20)
        Me.m_tbxNewName.TabIndex = 2
        '
        'm_btnOK
        '
        Me.m_btnOK.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_btnOK.Location = New System.Drawing.Point(214, 227)
        Me.m_btnOK.Name = "m_btnOK"
        Me.m_btnOK.Size = New System.Drawing.Size(75, 23)
        Me.m_btnOK.TabIndex = 3
        Me.m_btnOK.Text = "OK"
        Me.m_btnOK.UseVisualStyleBackColor = True
        '
        'm_btnCancel
        '
        Me.m_btnCancel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.m_btnCancel.Location = New System.Drawing.Point(295, 227)
        Me.m_btnCancel.Name = "m_btnCancel"
        Me.m_btnCancel.Size = New System.Drawing.Size(75, 23)
        Me.m_btnCancel.TabIndex = 4
        Me.m_btnCancel.Text = "Cancel"
        Me.m_btnCancel.UseVisualStyleBackColor = True
        '
        'm_clbGroups
        '
        Me.m_clbGroups.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_clbGroups.CheckOnClick = True
        Me.m_clbGroups.FormattingEnabled = True
        Me.m_clbGroups.IntegralHeight = False
        Me.m_clbGroups.Location = New System.Drawing.Point(80, 42)
        Me.m_clbGroups.Name = "m_clbGroups"
        Me.m_clbGroups.Size = New System.Drawing.Size(290, 153)
        Me.m_clbGroups.TabIndex = 5
        '
        'dlgMergeGroups
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.m_btnCancel
        Me.ClientSize = New System.Drawing.Size(382, 259)
        Me.ControlBox = False
        Me.Controls.Add(Me.m_clbGroups)
        Me.Controls.Add(Me.m_btnCancel)
        Me.Controls.Add(Me.m_btnOK)
        Me.Controls.Add(Me.m_tbxNewName)
        Me.Controls.Add(Me.m_lblNew)
        Me.Controls.Add(Me.m_lblAgg2)
        Me.Controls.Add(Me.m_cmbTarget)
        Me.Controls.Add(Me.m_lblAgg1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Name = "dlgMergeGroups"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Merge groups"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Private WithEvents m_lblAgg1 As System.Windows.Forms.Label
    Private WithEvents m_cmbTarget As System.Windows.Forms.ComboBox
    Private WithEvents m_lblAgg2 As System.Windows.Forms.Label
    Private WithEvents m_lblNew As System.Windows.Forms.Label
    Private WithEvents m_tbxNewName As System.Windows.Forms.TextBox
    Private WithEvents m_btnOK As System.Windows.Forms.Button
    Private WithEvents m_btnCancel As System.Windows.Forms.Button
    Private WithEvents m_clbGroups As Windows.Forms.CheckedListBox
End Class
