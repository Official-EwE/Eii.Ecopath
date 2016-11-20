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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(dlgMergeGroups))
        Me.m_lblAgg1 = New System.Windows.Forms.Label()
        Me.m_cmbTarget = New System.Windows.Forms.ComboBox()
        Me.m_lblAgg2 = New System.Windows.Forms.Label()
        Me.m_lblNew = New System.Windows.Forms.Label()
        Me.m_tbxNewName = New System.Windows.Forms.TextBox()
        Me.m_btnOK = New System.Windows.Forms.Button()
        Me.m_btnCancel = New System.Windows.Forms.Button()
        Me.m_clbGroups = New System.Windows.Forms.CheckedListBox()
        Me.m_tlpLogo = New System.Windows.Forms.TableLayoutPanel()
        Me.m_pbLogo = New System.Windows.Forms.PictureBox()
        Me.m_hdrSponsor = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.m_tlpLogo.SuspendLayout()
        CType(Me.m_pbLogo, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'm_lblAgg1
        '
        resources.ApplyResources(Me.m_lblAgg1, "m_lblAgg1")
        Me.m_lblAgg1.Name = "m_lblAgg1"
        '
        'm_cmbTarget
        '
        resources.ApplyResources(Me.m_cmbTarget, "m_cmbTarget")
        Me.m_cmbTarget.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.m_cmbTarget.FormattingEnabled = True
        Me.m_cmbTarget.Name = "m_cmbTarget"
        '
        'm_lblAgg2
        '
        resources.ApplyResources(Me.m_lblAgg2, "m_lblAgg2")
        Me.m_lblAgg2.Name = "m_lblAgg2"
        '
        'm_lblNew
        '
        resources.ApplyResources(Me.m_lblNew, "m_lblNew")
        Me.m_lblNew.Name = "m_lblNew"
        '
        'm_tbxNewName
        '
        resources.ApplyResources(Me.m_tbxNewName, "m_tbxNewName")
        Me.m_tbxNewName.Name = "m_tbxNewName"
        '
        'm_btnOK
        '
        resources.ApplyResources(Me.m_btnOK, "m_btnOK")
        Me.m_btnOK.Name = "m_btnOK"
        Me.m_btnOK.UseVisualStyleBackColor = True
        '
        'm_btnCancel
        '
        resources.ApplyResources(Me.m_btnCancel, "m_btnCancel")
        Me.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.m_btnCancel.Name = "m_btnCancel"
        Me.m_btnCancel.UseVisualStyleBackColor = True
        '
        'm_clbGroups
        '
        resources.ApplyResources(Me.m_clbGroups, "m_clbGroups")
        Me.m_clbGroups.CheckOnClick = True
        Me.m_clbGroups.FormattingEnabled = True
        Me.m_clbGroups.Name = "m_clbGroups"
        '
        'm_tlpLogo
        '
        resources.ApplyResources(Me.m_tlpLogo, "m_tlpLogo")
        Me.m_tlpLogo.BackColor = System.Drawing.Color.White
        Me.m_tlpLogo.Controls.Add(Me.m_pbLogo, 1, 1)
        Me.m_tlpLogo.Name = "m_tlpLogo"
        '
        'm_pbLogo
        '
        Me.m_pbLogo.BackgroundImage = Global.EwEMergeSplitGroupsPlugin.My.Resources.Resources.geomar_logo_en_print
        resources.ApplyResources(Me.m_pbLogo, "m_pbLogo")
        Me.m_pbLogo.Name = "m_pbLogo"
        Me.m_pbLogo.TabStop = False
        '
        'm_hdrSponsor
        '
        resources.ApplyResources(Me.m_hdrSponsor, "m_hdrSponsor")
        Me.m_hdrSponsor.CanCollapseParent = False
        Me.m_hdrSponsor.CollapsedParentHeight = 0
        Me.m_hdrSponsor.IsCollapsed = False
        Me.m_hdrSponsor.Name = "m_hdrSponsor"
        '
        'dlgMergeGroups
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.CancelButton = Me.m_btnCancel
        Me.ControlBox = False
        Me.Controls.Add(Me.m_hdrSponsor)
        Me.Controls.Add(Me.m_tlpLogo)
        Me.Controls.Add(Me.m_clbGroups)
        Me.Controls.Add(Me.m_btnCancel)
        Me.Controls.Add(Me.m_btnOK)
        Me.Controls.Add(Me.m_tbxNewName)
        Me.Controls.Add(Me.m_lblNew)
        Me.Controls.Add(Me.m_lblAgg2)
        Me.Controls.Add(Me.m_cmbTarget)
        Me.Controls.Add(Me.m_lblAgg1)
        Me.Name = "dlgMergeGroups"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.m_tlpLogo.ResumeLayout(False)
        CType(Me.m_pbLogo, System.ComponentModel.ISupportInitialize).EndInit()
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
    Private WithEvents m_tlpLogo As Windows.Forms.TableLayoutPanel
    Private WithEvents m_pbLogo As Windows.Forms.PictureBox
    Private WithEvents m_hdrSponsor As cEwEHeaderLabel
End Class
