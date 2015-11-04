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

Partial Class dlgEcobaseExport
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(dlgEcobaseExport))
        Me.m_lblModel = New System.Windows.Forms.Label()
        Me.m_tbxModel = New System.Windows.Forms.TextBox()
        Me.m_lblDOI = New System.Windows.Forms.Label()
        Me.m_lblAuthor = New System.Windows.Forms.Label()
        Me.m_tbxDOI = New System.Windows.Forms.TextBox()
        Me.m_tbxAuthor = New System.Windows.Forms.TextBox()
        Me.m_cbConfirmAuthor = New System.Windows.Forms.CheckBox()
        Me.m_cbConfirmDessiminate = New System.Windows.Forms.CheckBox()
        Me.m_btnCancel = New System.Windows.Forms.Button()
        Me.m_btnSubmit = New System.Windows.Forms.Button()
        Me.m_llViewEcobaseDataAgreement = New System.Windows.Forms.LinkLabel()
        Me.m_lblEmail = New System.Windows.Forms.Label()
        Me.m_tbxEmail = New System.Windows.Forms.TextBox()
        Me.m_llViewPublication = New System.Windows.Forms.LinkLabel()
        Me.m_pbModel = New System.Windows.Forms.PictureBox()
        Me.m_pbAuthor = New System.Windows.Forms.PictureBox()
        Me.m_pbPublication = New System.Windows.Forms.PictureBox()
        Me.m_pbConfirmAuthor = New System.Windows.Forms.PictureBox()
        Me.m_lbLDescription = New System.Windows.Forms.Label()
        Me.m_tbxDescription = New System.Windows.Forms.TextBox()
        Me.m_pbDescription = New System.Windows.Forms.PictureBox()
        Me.m_tbxHyperlink = New System.Windows.Forms.TextBox()
        Me.m_lblHyperlink = New System.Windows.Forms.Label()
        Me.CEwEHeaderLabel1 = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.CEwEHeaderLabel2 = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.m_hdrPublication = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.m_hdrPermission = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.m_cbIsUpdate = New System.Windows.Forms.CheckBox()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.CEwEHeaderLabel3 = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.m_lblEcoType = New System.Windows.Forms.Label()
        Me.m_lblEcoCat = New System.Windows.Forms.Label()
        Me.m_lblCountry = New System.Windows.Forms.Label()
        Me.m_lblRegion = New System.Windows.Forms.Label()
        Me.m_tbxLME = New System.Windows.Forms.TextBox()
        Me.m_lblLME = New System.Windows.Forms.Label()
        Me.m_cmbEcoCat = New System.Windows.Forms.ComboBox()
        Me.m_cmbEcoType = New System.Windows.Forms.ComboBox()
        Me.ComboBox1 = New System.Windows.Forms.ComboBox()
        Me.ComboBox2 = New System.Windows.Forms.ComboBox()
        CType(Me.m_pbModel, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.m_pbAuthor, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.m_pbPublication, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.m_pbConfirmAuthor, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.m_pbDescription, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'm_lblModel
        '
        resources.ApplyResources(Me.m_lblModel, "m_lblModel")
        Me.m_lblModel.Name = "m_lblModel"
        '
        'm_tbxModel
        '
        resources.ApplyResources(Me.m_tbxModel, "m_tbxModel")
        Me.m_tbxModel.Name = "m_tbxModel"
        '
        'm_lblDOI
        '
        resources.ApplyResources(Me.m_lblDOI, "m_lblDOI")
        Me.m_lblDOI.Name = "m_lblDOI"
        '
        'm_lblAuthor
        '
        resources.ApplyResources(Me.m_lblAuthor, "m_lblAuthor")
        Me.m_lblAuthor.Name = "m_lblAuthor"
        '
        'm_tbxDOI
        '
        resources.ApplyResources(Me.m_tbxDOI, "m_tbxDOI")
        Me.m_tbxDOI.Name = "m_tbxDOI"
        '
        'm_tbxAuthor
        '
        resources.ApplyResources(Me.m_tbxAuthor, "m_tbxAuthor")
        Me.m_tbxAuthor.Name = "m_tbxAuthor"
        '
        'm_cbConfirmAuthor
        '
        resources.ApplyResources(Me.m_cbConfirmAuthor, "m_cbConfirmAuthor")
        Me.m_cbConfirmAuthor.Name = "m_cbConfirmAuthor"
        Me.m_cbConfirmAuthor.UseVisualStyleBackColor = True
        '
        'm_cbConfirmDessiminate
        '
        resources.ApplyResources(Me.m_cbConfirmDessiminate, "m_cbConfirmDessiminate")
        Me.m_cbConfirmDessiminate.Name = "m_cbConfirmDessiminate"
        Me.m_cbConfirmDessiminate.UseVisualStyleBackColor = True
        '
        'm_btnCancel
        '
        resources.ApplyResources(Me.m_btnCancel, "m_btnCancel")
        Me.m_btnCancel.Name = "m_btnCancel"
        Me.m_btnCancel.UseVisualStyleBackColor = True
        '
        'm_btnSubmit
        '
        resources.ApplyResources(Me.m_btnSubmit, "m_btnSubmit")
        Me.m_btnSubmit.Name = "m_btnSubmit"
        Me.m_btnSubmit.UseVisualStyleBackColor = True
        '
        'm_llViewEcobaseDataAgreement
        '
        resources.ApplyResources(Me.m_llViewEcobaseDataAgreement, "m_llViewEcobaseDataAgreement")
        Me.m_llViewEcobaseDataAgreement.Name = "m_llViewEcobaseDataAgreement"
        Me.m_llViewEcobaseDataAgreement.TabStop = True
        '
        'm_lblEmail
        '
        resources.ApplyResources(Me.m_lblEmail, "m_lblEmail")
        Me.m_lblEmail.Name = "m_lblEmail"
        '
        'm_tbxEmail
        '
        resources.ApplyResources(Me.m_tbxEmail, "m_tbxEmail")
        Me.m_tbxEmail.Name = "m_tbxEmail"
        '
        'm_llViewPublication
        '
        resources.ApplyResources(Me.m_llViewPublication, "m_llViewPublication")
        Me.m_llViewPublication.Name = "m_llViewPublication"
        Me.m_llViewPublication.TabStop = True
        '
        'm_pbModel
        '
        resources.ApplyResources(Me.m_pbModel, "m_pbModel")
        Me.m_pbModel.Name = "m_pbModel"
        Me.m_pbModel.TabStop = False
        '
        'm_pbAuthor
        '
        resources.ApplyResources(Me.m_pbAuthor, "m_pbAuthor")
        Me.m_pbAuthor.Name = "m_pbAuthor"
        Me.m_pbAuthor.TabStop = False
        '
        'm_pbPublication
        '
        resources.ApplyResources(Me.m_pbPublication, "m_pbPublication")
        Me.m_pbPublication.Name = "m_pbPublication"
        Me.m_pbPublication.TabStop = False
        '
        'm_pbConfirmAuthor
        '
        resources.ApplyResources(Me.m_pbConfirmAuthor, "m_pbConfirmAuthor")
        Me.m_pbConfirmAuthor.Name = "m_pbConfirmAuthor"
        Me.m_pbConfirmAuthor.TabStop = False
        '
        'm_lbLDescription
        '
        resources.ApplyResources(Me.m_lbLDescription, "m_lbLDescription")
        Me.m_lbLDescription.Name = "m_lbLDescription"
        '
        'm_tbxDescription
        '
        resources.ApplyResources(Me.m_tbxDescription, "m_tbxDescription")
        Me.m_tbxDescription.Name = "m_tbxDescription"
        '
        'm_pbDescription
        '
        resources.ApplyResources(Me.m_pbDescription, "m_pbDescription")
        Me.m_pbDescription.Name = "m_pbDescription"
        Me.m_pbDescription.TabStop = False
        '
        'm_tbxHyperlink
        '
        resources.ApplyResources(Me.m_tbxHyperlink, "m_tbxHyperlink")
        Me.m_tbxHyperlink.Name = "m_tbxHyperlink"
        '
        'm_lblHyperlink
        '
        resources.ApplyResources(Me.m_lblHyperlink, "m_lblHyperlink")
        Me.m_lblHyperlink.Name = "m_lblHyperlink"
        '
        'CEwEHeaderLabel1
        '
        resources.ApplyResources(Me.CEwEHeaderLabel1, "CEwEHeaderLabel1")
        Me.CEwEHeaderLabel1.CanCollapseParent = False
        Me.CEwEHeaderLabel1.CollapsedParentHeight = 0
        Me.CEwEHeaderLabel1.IsCollapsed = False
        Me.CEwEHeaderLabel1.Name = "CEwEHeaderLabel1"
        '
        'CEwEHeaderLabel2
        '
        resources.ApplyResources(Me.CEwEHeaderLabel2, "CEwEHeaderLabel2")
        Me.CEwEHeaderLabel2.CanCollapseParent = False
        Me.CEwEHeaderLabel2.CollapsedParentHeight = 0
        Me.CEwEHeaderLabel2.IsCollapsed = False
        Me.CEwEHeaderLabel2.Name = "CEwEHeaderLabel2"
        '
        'm_hdrPublication
        '
        resources.ApplyResources(Me.m_hdrPublication, "m_hdrPublication")
        Me.m_hdrPublication.CanCollapseParent = False
        Me.m_hdrPublication.CollapsedParentHeight = 0
        Me.m_hdrPublication.IsCollapsed = False
        Me.m_hdrPublication.Name = "m_hdrPublication"
        '
        'm_hdrPermission
        '
        resources.ApplyResources(Me.m_hdrPermission, "m_hdrPermission")
        Me.m_hdrPermission.CanCollapseParent = False
        Me.m_hdrPermission.CollapsedParentHeight = 0
        Me.m_hdrPermission.IsCollapsed = False
        Me.m_hdrPermission.Name = "m_hdrPermission"
        '
        'm_cbIsUpdate
        '
        resources.ApplyResources(Me.m_cbIsUpdate, "m_cbIsUpdate")
        Me.m_cbIsUpdate.Name = "m_cbIsUpdate"
        Me.m_cbIsUpdate.UseVisualStyleBackColor = True
        '
        'PictureBox1
        '
        resources.ApplyResources(Me.PictureBox1, "PictureBox1")
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.TabStop = False
        '
        'CEwEHeaderLabel3
        '
        resources.ApplyResources(Me.CEwEHeaderLabel3, "CEwEHeaderLabel3")
        Me.CEwEHeaderLabel3.CanCollapseParent = False
        Me.CEwEHeaderLabel3.CollapsedParentHeight = 0
        Me.CEwEHeaderLabel3.IsCollapsed = False
        Me.CEwEHeaderLabel3.Name = "CEwEHeaderLabel3"
        '
        'm_lblEcoType
        '
        resources.ApplyResources(Me.m_lblEcoType, "m_lblEcoType")
        Me.m_lblEcoType.Name = "m_lblEcoType"
        '
        'm_lblEcoCat
        '
        resources.ApplyResources(Me.m_lblEcoCat, "m_lblEcoCat")
        Me.m_lblEcoCat.Name = "m_lblEcoCat"
        '
        'm_lblCountry
        '
        resources.ApplyResources(Me.m_lblCountry, "m_lblCountry")
        Me.m_lblCountry.Name = "m_lblCountry"
        '
        'm_lblRegion
        '
        resources.ApplyResources(Me.m_lblRegion, "m_lblRegion")
        Me.m_lblRegion.Name = "m_lblRegion"
        '
        'm_tbxLME
        '
        resources.ApplyResources(Me.m_tbxLME, "m_tbxLME")
        Me.m_tbxLME.Name = "m_tbxLME"
        '
        'm_lblLME
        '
        resources.ApplyResources(Me.m_lblLME, "m_lblLME")
        Me.m_lblLME.Name = "m_lblLME"
        '
        'm_cmbEcoCat
        '
        Me.m_cmbEcoCat.FormattingEnabled = True
        resources.ApplyResources(Me.m_cmbEcoCat, "m_cmbEcoCat")
        Me.m_cmbEcoCat.Name = "m_cmbEcoCat"
        '
        'm_cmbEcoType
        '
        Me.m_cmbEcoType.FormattingEnabled = True
        resources.ApplyResources(Me.m_cmbEcoType, "m_cmbEcoType")
        Me.m_cmbEcoType.Name = "m_cmbEcoType"
        '
        'ComboBox1
        '
        Me.ComboBox1.FormattingEnabled = True
        resources.ApplyResources(Me.ComboBox1, "ComboBox1")
        Me.ComboBox1.Name = "ComboBox1"
        '
        'ComboBox2
        '
        Me.ComboBox2.FormattingEnabled = True
        resources.ApplyResources(Me.ComboBox2, "ComboBox2")
        Me.ComboBox2.Name = "ComboBox2"
        '
        'dlgEcobaseExport
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ControlBox = False
        Me.Controls.Add(Me.m_cmbEcoCat)
        Me.Controls.Add(Me.ComboBox2)
        Me.Controls.Add(Me.ComboBox1)
        Me.Controls.Add(Me.m_cmbEcoType)
        Me.Controls.Add(Me.m_tbxLME)
        Me.Controls.Add(Me.m_lblLME)
        Me.Controls.Add(Me.m_lblRegion)
        Me.Controls.Add(Me.m_lblCountry)
        Me.Controls.Add(Me.m_lblEcoCat)
        Me.Controls.Add(Me.m_lblEcoType)
        Me.Controls.Add(Me.PictureBox1)
        Me.Controls.Add(Me.CEwEHeaderLabel3)
        Me.Controls.Add(Me.m_cbIsUpdate)
        Me.Controls.Add(Me.m_tbxHyperlink)
        Me.Controls.Add(Me.m_lblHyperlink)
        Me.Controls.Add(Me.m_pbConfirmAuthor)
        Me.Controls.Add(Me.m_pbPublication)
        Me.Controls.Add(Me.m_pbAuthor)
        Me.Controls.Add(Me.m_pbDescription)
        Me.Controls.Add(Me.m_pbModel)
        Me.Controls.Add(Me.m_llViewPublication)
        Me.Controls.Add(Me.m_llViewEcobaseDataAgreement)
        Me.Controls.Add(Me.m_btnSubmit)
        Me.Controls.Add(Me.m_btnCancel)
        Me.Controls.Add(Me.CEwEHeaderLabel1)
        Me.Controls.Add(Me.CEwEHeaderLabel2)
        Me.Controls.Add(Me.m_hdrPublication)
        Me.Controls.Add(Me.m_hdrPermission)
        Me.Controls.Add(Me.m_cbConfirmDessiminate)
        Me.Controls.Add(Me.m_cbConfirmAuthor)
        Me.Controls.Add(Me.m_tbxEmail)
        Me.Controls.Add(Me.m_tbxAuthor)
        Me.Controls.Add(Me.m_tbxDOI)
        Me.Controls.Add(Me.m_lblEmail)
        Me.Controls.Add(Me.m_tbxDescription)
        Me.Controls.Add(Me.m_tbxModel)
        Me.Controls.Add(Me.m_lblAuthor)
        Me.Controls.Add(Me.m_lbLDescription)
        Me.Controls.Add(Me.m_lblDOI)
        Me.Controls.Add(Me.m_lblModel)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "dlgEcobaseExport"
        Me.ShowInTaskbar = False
        CType(Me.m_pbModel, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.m_pbAuthor, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.m_pbPublication, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.m_pbConfirmAuthor, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.m_pbDescription, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Private WithEvents m_lblModel As System.Windows.Forms.Label
    Private WithEvents m_tbxModel As System.Windows.Forms.TextBox
    Private WithEvents m_lblDOI As System.Windows.Forms.Label
    Private WithEvents m_lblAuthor As System.Windows.Forms.Label
    Private WithEvents m_tbxDOI As System.Windows.Forms.TextBox
    Private WithEvents m_tbxAuthor As System.Windows.Forms.TextBox
    Private WithEvents m_cbConfirmAuthor As System.Windows.Forms.CheckBox
    Private WithEvents m_cbConfirmDessiminate As System.Windows.Forms.CheckBox
    Private WithEvents m_hdrPermission As ScientificInterfaceShared.Controls.cEwEHeaderLabel
    Private WithEvents CEwEHeaderLabel1 As ScientificInterfaceShared.Controls.cEwEHeaderLabel
    Private WithEvents CEwEHeaderLabel2 As ScientificInterfaceShared.Controls.cEwEHeaderLabel
    Private WithEvents m_btnCancel As System.Windows.Forms.Button
    Private WithEvents m_btnSubmit As System.Windows.Forms.Button
    Private WithEvents m_llViewEcobaseDataAgreement As System.Windows.Forms.LinkLabel
    Private WithEvents m_lblEmail As System.Windows.Forms.Label
    Private WithEvents m_tbxEmail As System.Windows.Forms.TextBox
    Private WithEvents m_llViewPublication As System.Windows.Forms.LinkLabel
    Private WithEvents m_pbModel As System.Windows.Forms.PictureBox
    Private WithEvents m_pbAuthor As System.Windows.Forms.PictureBox
    Private WithEvents m_pbPublication As System.Windows.Forms.PictureBox
    Private WithEvents m_pbConfirmAuthor As System.Windows.Forms.PictureBox
    Private WithEvents m_lbLDescription As System.Windows.Forms.Label
    Private WithEvents m_tbxDescription As System.Windows.Forms.TextBox
    Private WithEvents m_pbDescription As System.Windows.Forms.PictureBox
    Private WithEvents m_tbxHyperlink As System.Windows.Forms.TextBox
    Private WithEvents m_lblHyperlink As System.Windows.Forms.Label
    Private WithEvents m_hdrPublication As ScientificInterfaceShared.Controls.cEwEHeaderLabel
    Private WithEvents m_cbIsUpdate As System.Windows.Forms.CheckBox
    Private WithEvents PictureBox1 As System.Windows.Forms.PictureBox
    Private WithEvents CEwEHeaderLabel3 As ScientificInterfaceShared.Controls.cEwEHeaderLabel
    Private WithEvents m_lblEcoType As System.Windows.Forms.Label
    Private WithEvents m_lblEcoCat As System.Windows.Forms.Label
    Private WithEvents m_lblCountry As System.Windows.Forms.Label
    Private WithEvents m_lblRegion As System.Windows.Forms.Label
    Friend WithEvents m_tbxLME As System.Windows.Forms.TextBox
    Private WithEvents m_lblLME As System.Windows.Forms.Label
    Private WithEvents m_cmbEcoCat As System.Windows.Forms.ComboBox
    Private WithEvents m_cmbEcoType As System.Windows.Forms.ComboBox
    Private WithEvents ComboBox1 As System.Windows.Forms.ComboBox
    Private WithEvents ComboBox2 As System.Windows.Forms.ComboBox
End Class
