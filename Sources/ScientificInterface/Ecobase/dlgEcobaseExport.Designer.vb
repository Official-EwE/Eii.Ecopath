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
        Me.m_cbConfirmDessiminate = New System.Windows.Forms.CheckBox()
        Me.m_btnCancel = New System.Windows.Forms.Button()
        Me.m_btnSubmit = New System.Windows.Forms.Button()
        Me.m_lblEmail = New System.Windows.Forms.Label()
        Me.m_tbxEmail = New System.Windows.Forms.TextBox()
        Me.m_llViewPublication = New System.Windows.Forms.LinkLabel()
        Me.m_pbModel = New System.Windows.Forms.PictureBox()
        Me.m_pbAuthor = New System.Windows.Forms.PictureBox()
        Me.m_pbPublication = New System.Windows.Forms.PictureBox()
        Me.m_lbLDescription = New System.Windows.Forms.Label()
        Me.m_tbxDescription = New System.Windows.Forms.TextBox()
        Me.m_pbDescription = New System.Windows.Forms.PictureBox()
        Me.m_tbxHyperlink = New System.Windows.Forms.TextBox()
        Me.m_lblHyperlink = New System.Windows.Forms.Label()
        Me.m_pbAreaName = New System.Windows.Forms.PictureBox()
        Me.m_lblEcoType = New System.Windows.Forms.Label()
        Me.m_lblEcoCat = New System.Windows.Forms.Label()
        Me.m_lblCountry = New System.Windows.Forms.Label()
        Me.m_lblRegion = New System.Windows.Forms.Label()
        Me.m_tbxLME = New System.Windows.Forms.TextBox()
        Me.m_lblLME = New System.Windows.Forms.Label()
        Me.m_cmbEcoCat = New System.Windows.Forms.ComboBox()
        Me.m_cmbEcoType = New System.Windows.Forms.ComboBox()
        Me.m_cmbCountry = New System.Windows.Forms.ComboBox()
        Me.m_cmbRegion = New System.Windows.Forms.ComboBox()
        Me.m_tcExport = New System.Windows.Forms.TabControl()
        Me.m_tpEcoBase = New System.Windows.Forms.TabPage()
        Me.m_pbAgreement = New System.Windows.Forms.PictureBox()
        Me.m_rtfAgreement = New System.Windows.Forms.RichTextBox()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.m_cbEcoBaseAgreement = New System.Windows.Forms.CheckBox()
        Me.m_tpModel = New System.Windows.Forms.TabPage()
        Me.m_cbEcospaceUsed = New System.Windows.Forms.CheckBox()
        Me.m_cbIsUpdate = New System.Windows.Forms.CheckBox()
        Me.m_cbFittedToTimeSeries = New System.Windows.Forms.CheckBox()
        Me.m_cbEcosimUsed = New System.Windows.Forms.CheckBox()
        Me.m_cbConfirmAuthor = New System.Windows.Forms.CheckBox()
        Me.m_lblObjectives = New System.Windows.Forms.Label()
        Me.m_pbIsAuthor = New System.Windows.Forms.PictureBox()
        Me.m_pbObjectives = New System.Windows.Forms.PictureBox()
        Me.m_tbxObjectives = New System.Windows.Forms.TextBox()
        Me.m_hdrAuthor = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.m_tpPublication = New System.Windows.Forms.TabPage()
        Me.m_cbDifferentFromPaper = New System.Windows.Forms.CheckBox()
        Me.m_lblExplanation = New System.Windows.Forms.Label()
        Me.m_lblReference = New System.Windows.Forms.Label()
        Me.m_pbDifference = New System.Windows.Forms.PictureBox()
        Me.m_pbRef = New System.Windows.Forms.PictureBox()
        Me.m_tbxDifference = New System.Windows.Forms.TextBox()
        Me.m_tbxReference = New System.Windows.Forms.TextBox()
        Me.m_tpClassification = New System.Windows.Forms.TabPage()
        Me.m_lblTempMax = New System.Windows.Forms.Label()
        Me.m_lblDepthMax = New System.Windows.Forms.Label()
        Me.m_lblTempMean = New System.Windows.Forms.Label()
        Me.m_lblDepthMean = New System.Windows.Forms.Label()
        Me.m_lblTempMin = New System.Windows.Forms.Label()
        Me.m_lblDepthMin = New System.Windows.Forms.Label()
        Me.m_lblSouth = New System.Windows.Forms.Label()
        Me.m_lblEast = New System.Windows.Forms.Label()
        Me.m_lblWest = New System.Windows.Forms.Label()
        Me.m_lblNorth = New System.Windows.Forms.Label()
        Me.m_pbEnvVars = New System.Windows.Forms.PictureBox()
        Me.m_pbEcosystem = New System.Windows.Forms.PictureBox()
        Me.m_pbBoundingBox = New System.Windows.Forms.PictureBox()
        Me.m_tbxTempMax = New System.Windows.Forms.TextBox()
        Me.m_tbxDepthMax = New System.Windows.Forms.TextBox()
        Me.m_tbxTempMean = New System.Windows.Forms.TextBox()
        Me.m_tbxDepthMean = New System.Windows.Forms.TextBox()
        Me.m_tbxTempMin = New System.Windows.Forms.TextBox()
        Me.m_tbxDepthMin = New System.Windows.Forms.TextBox()
        Me.m_hdrEcosystem = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.m_nudEast = New ScientificInterfaceShared.Controls.cEwENumericUpDown()
        Me.m_nudSouth = New ScientificInterfaceShared.Controls.cEwENumericUpDown()
        Me.m_nudWest = New ScientificInterfaceShared.Controls.cEwENumericUpDown()
        Me.m_hdrClassification = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.m_nudNorth = New ScientificInterfaceShared.Controls.cEwENumericUpDown()
        Me.m_hdrArea = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.m_tpAccess = New System.Windows.Forms.TabPage()
        Me.m_hdrAccess = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.m_pbPermissionComment = New System.Windows.Forms.PictureBox()
        Me.m_lblPermissionComments = New System.Windows.Forms.Label()
        Me.m_tbxPermissionComments = New System.Windows.Forms.TextBox()
        Me.m_wrkGetAgreement = New System.ComponentModel.BackgroundWorker()
        Me.m_tbxNumYears = New System.Windows.Forms.TextBox()
        Me.m_tbxFirstYear = New System.Windows.Forms.TextBox()
        Me.m_lblNoYears = New System.Windows.Forms.Label()
        Me.m_tbxArea = New System.Windows.Forms.TextBox()
        Me.m_lblFirstYear = New System.Windows.Forms.Label()
        Me.m_lblArea = New System.Windows.Forms.Label()
        Me.m_pbYear = New System.Windows.Forms.PictureBox()
        Me.m_pbArea = New System.Windows.Forms.PictureBox()
        CType(Me.m_pbModel, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.m_pbAuthor, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.m_pbPublication, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.m_pbDescription, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.m_pbAreaName, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.m_tcExport.SuspendLayout()
        Me.m_tpEcoBase.SuspendLayout()
        CType(Me.m_pbAgreement, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.m_tpModel.SuspendLayout()
        CType(Me.m_pbIsAuthor, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.m_pbObjectives, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.m_tpPublication.SuspendLayout()
        CType(Me.m_pbDifference, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.m_pbRef, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.m_tpClassification.SuspendLayout()
        CType(Me.m_pbEnvVars, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.m_pbEcosystem, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.m_pbBoundingBox, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.m_nudEast, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.m_nudSouth, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.m_nudWest, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.m_nudNorth, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.m_tpAccess.SuspendLayout()
        CType(Me.m_pbPermissionComment, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.m_pbYear, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.m_pbArea, System.ComponentModel.ISupportInitialize).BeginInit()
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
        'm_pbAreaName
        '
        resources.ApplyResources(Me.m_pbAreaName, "m_pbAreaName")
        Me.m_pbAreaName.Name = "m_pbAreaName"
        Me.m_pbAreaName.TabStop = False
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
        'm_cmbCountry
        '
        Me.m_cmbCountry.FormattingEnabled = True
        resources.ApplyResources(Me.m_cmbCountry, "m_cmbCountry")
        Me.m_cmbCountry.Name = "m_cmbCountry"
        '
        'm_cmbRegion
        '
        Me.m_cmbRegion.FormattingEnabled = True
        resources.ApplyResources(Me.m_cmbRegion, "m_cmbRegion")
        Me.m_cmbRegion.Name = "m_cmbRegion"
        '
        'm_tcExport
        '
        resources.ApplyResources(Me.m_tcExport, "m_tcExport")
        Me.m_tcExport.Controls.Add(Me.m_tpEcoBase)
        Me.m_tcExport.Controls.Add(Me.m_tpModel)
        Me.m_tcExport.Controls.Add(Me.m_tpPublication)
        Me.m_tcExport.Controls.Add(Me.m_tpClassification)
        Me.m_tcExport.Controls.Add(Me.m_tpAccess)
        Me.m_tcExport.Name = "m_tcExport"
        Me.m_tcExport.SelectedIndex = 0
        '
        'm_tpEcoBase
        '
        Me.m_tpEcoBase.Controls.Add(Me.m_pbAgreement)
        Me.m_tpEcoBase.Controls.Add(Me.m_rtfAgreement)
        Me.m_tpEcoBase.Controls.Add(Me.PictureBox1)
        Me.m_tpEcoBase.Controls.Add(Me.m_cbEcoBaseAgreement)
        resources.ApplyResources(Me.m_tpEcoBase, "m_tpEcoBase")
        Me.m_tpEcoBase.Name = "m_tpEcoBase"
        Me.m_tpEcoBase.UseVisualStyleBackColor = True
        '
        'm_pbAgreement
        '
        resources.ApplyResources(Me.m_pbAgreement, "m_pbAgreement")
        Me.m_pbAgreement.Name = "m_pbAgreement"
        Me.m_pbAgreement.TabStop = False
        '
        'm_rtfAgreement
        '
        resources.ApplyResources(Me.m_rtfAgreement, "m_rtfAgreement")
        Me.m_rtfAgreement.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.m_rtfAgreement.Name = "m_rtfAgreement"
        Me.m_rtfAgreement.ReadOnly = True
        '
        'PictureBox1
        '
        Me.PictureBox1.BackgroundImage = Global.ScientificInterface.My.Resources.Resources.EcoBase1
        resources.ApplyResources(Me.PictureBox1, "PictureBox1")
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.TabStop = False
        '
        'm_cbEcoBaseAgreement
        '
        resources.ApplyResources(Me.m_cbEcoBaseAgreement, "m_cbEcoBaseAgreement")
        Me.m_cbEcoBaseAgreement.Name = "m_cbEcoBaseAgreement"
        Me.m_cbEcoBaseAgreement.UseVisualStyleBackColor = True
        '
        'm_tpModel
        '
        Me.m_tpModel.Controls.Add(Me.m_tbxNumYears)
        Me.m_tpModel.Controls.Add(Me.m_tbxFirstYear)
        Me.m_tpModel.Controls.Add(Me.m_lblNoYears)
        Me.m_tpModel.Controls.Add(Me.m_tbxArea)
        Me.m_tpModel.Controls.Add(Me.m_lblFirstYear)
        Me.m_tpModel.Controls.Add(Me.m_lblArea)
        Me.m_tpModel.Controls.Add(Me.m_cbEcospaceUsed)
        Me.m_tpModel.Controls.Add(Me.m_cbIsUpdate)
        Me.m_tpModel.Controls.Add(Me.m_cbFittedToTimeSeries)
        Me.m_tpModel.Controls.Add(Me.m_cbEcosimUsed)
        Me.m_tpModel.Controls.Add(Me.m_cbConfirmAuthor)
        Me.m_tpModel.Controls.Add(Me.m_lblModel)
        Me.m_tpModel.Controls.Add(Me.m_lblObjectives)
        Me.m_tpModel.Controls.Add(Me.m_lbLDescription)
        Me.m_tpModel.Controls.Add(Me.m_lblAuthor)
        Me.m_tpModel.Controls.Add(Me.m_tbxModel)
        Me.m_tpModel.Controls.Add(Me.m_pbIsAuthor)
        Me.m_tpModel.Controls.Add(Me.m_pbArea)
        Me.m_tpModel.Controls.Add(Me.m_pbYear)
        Me.m_tpModel.Controls.Add(Me.m_pbObjectives)
        Me.m_tpModel.Controls.Add(Me.m_pbModel)
        Me.m_tpModel.Controls.Add(Me.m_tbxObjectives)
        Me.m_tpModel.Controls.Add(Me.m_tbxDescription)
        Me.m_tpModel.Controls.Add(Me.m_pbDescription)
        Me.m_tpModel.Controls.Add(Me.m_tbxAuthor)
        Me.m_tpModel.Controls.Add(Me.m_lblEmail)
        Me.m_tpModel.Controls.Add(Me.m_tbxEmail)
        Me.m_tpModel.Controls.Add(Me.m_pbAuthor)
        Me.m_tpModel.Controls.Add(Me.m_hdrAuthor)
        resources.ApplyResources(Me.m_tpModel, "m_tpModel")
        Me.m_tpModel.Name = "m_tpModel"
        Me.m_tpModel.UseVisualStyleBackColor = True
        '
        'm_cbEcospaceUsed
        '
        resources.ApplyResources(Me.m_cbEcospaceUsed, "m_cbEcospaceUsed")
        Me.m_cbEcospaceUsed.Name = "m_cbEcospaceUsed"
        Me.m_cbEcospaceUsed.UseVisualStyleBackColor = True
        '
        'm_cbIsUpdate
        '
        resources.ApplyResources(Me.m_cbIsUpdate, "m_cbIsUpdate")
        Me.m_cbIsUpdate.Name = "m_cbIsUpdate"
        Me.m_cbIsUpdate.UseVisualStyleBackColor = True
        '
        'm_cbFittedToTimeSeries
        '
        resources.ApplyResources(Me.m_cbFittedToTimeSeries, "m_cbFittedToTimeSeries")
        Me.m_cbFittedToTimeSeries.Name = "m_cbFittedToTimeSeries"
        Me.m_cbFittedToTimeSeries.UseVisualStyleBackColor = True
        '
        'm_cbEcosimUsed
        '
        resources.ApplyResources(Me.m_cbEcosimUsed, "m_cbEcosimUsed")
        Me.m_cbEcosimUsed.Name = "m_cbEcosimUsed"
        Me.m_cbEcosimUsed.UseVisualStyleBackColor = True
        '
        'm_cbConfirmAuthor
        '
        resources.ApplyResources(Me.m_cbConfirmAuthor, "m_cbConfirmAuthor")
        Me.m_cbConfirmAuthor.Name = "m_cbConfirmAuthor"
        Me.m_cbConfirmAuthor.UseVisualStyleBackColor = True
        '
        'm_lblObjectives
        '
        resources.ApplyResources(Me.m_lblObjectives, "m_lblObjectives")
        Me.m_lblObjectives.Name = "m_lblObjectives"
        '
        'm_pbIsAuthor
        '
        resources.ApplyResources(Me.m_pbIsAuthor, "m_pbIsAuthor")
        Me.m_pbIsAuthor.Name = "m_pbIsAuthor"
        Me.m_pbIsAuthor.TabStop = False
        '
        'm_pbObjectives
        '
        resources.ApplyResources(Me.m_pbObjectives, "m_pbObjectives")
        Me.m_pbObjectives.Name = "m_pbObjectives"
        Me.m_pbObjectives.TabStop = False
        '
        'm_tbxObjectives
        '
        resources.ApplyResources(Me.m_tbxObjectives, "m_tbxObjectives")
        Me.m_tbxObjectives.Name = "m_tbxObjectives"
        '
        'm_hdrAuthor
        '
        resources.ApplyResources(Me.m_hdrAuthor, "m_hdrAuthor")
        Me.m_hdrAuthor.CanCollapseParent = False
        Me.m_hdrAuthor.CollapsedParentHeight = 0
        Me.m_hdrAuthor.IsCollapsed = False
        Me.m_hdrAuthor.Name = "m_hdrAuthor"
        '
        'm_tpPublication
        '
        Me.m_tpPublication.Controls.Add(Me.m_cbDifferentFromPaper)
        Me.m_tpPublication.Controls.Add(Me.m_lblExplanation)
        Me.m_tpPublication.Controls.Add(Me.m_lblReference)
        Me.m_tpPublication.Controls.Add(Me.m_lblHyperlink)
        Me.m_tpPublication.Controls.Add(Me.m_tbxHyperlink)
        Me.m_tpPublication.Controls.Add(Me.m_pbDifference)
        Me.m_tpPublication.Controls.Add(Me.m_pbRef)
        Me.m_tpPublication.Controls.Add(Me.m_pbPublication)
        Me.m_tpPublication.Controls.Add(Me.m_lblDOI)
        Me.m_tpPublication.Controls.Add(Me.m_llViewPublication)
        Me.m_tpPublication.Controls.Add(Me.m_tbxDifference)
        Me.m_tpPublication.Controls.Add(Me.m_tbxReference)
        Me.m_tpPublication.Controls.Add(Me.m_tbxDOI)
        resources.ApplyResources(Me.m_tpPublication, "m_tpPublication")
        Me.m_tpPublication.Name = "m_tpPublication"
        Me.m_tpPublication.UseVisualStyleBackColor = True
        '
        'm_cbDifferentFromPaper
        '
        resources.ApplyResources(Me.m_cbDifferentFromPaper, "m_cbDifferentFromPaper")
        Me.m_cbDifferentFromPaper.Name = "m_cbDifferentFromPaper"
        Me.m_cbDifferentFromPaper.UseVisualStyleBackColor = True
        '
        'm_lblExplanation
        '
        resources.ApplyResources(Me.m_lblExplanation, "m_lblExplanation")
        Me.m_lblExplanation.Name = "m_lblExplanation"
        '
        'm_lblReference
        '
        resources.ApplyResources(Me.m_lblReference, "m_lblReference")
        Me.m_lblReference.Name = "m_lblReference"
        '
        'm_pbDifference
        '
        resources.ApplyResources(Me.m_pbDifference, "m_pbDifference")
        Me.m_pbDifference.Name = "m_pbDifference"
        Me.m_pbDifference.TabStop = False
        '
        'm_pbRef
        '
        resources.ApplyResources(Me.m_pbRef, "m_pbRef")
        Me.m_pbRef.Name = "m_pbRef"
        Me.m_pbRef.TabStop = False
        '
        'm_tbxDifference
        '
        resources.ApplyResources(Me.m_tbxDifference, "m_tbxDifference")
        Me.m_tbxDifference.Name = "m_tbxDifference"
        '
        'm_tbxReference
        '
        resources.ApplyResources(Me.m_tbxReference, "m_tbxReference")
        Me.m_tbxReference.Name = "m_tbxReference"
        '
        'm_tpClassification
        '
        Me.m_tpClassification.Controls.Add(Me.m_lblTempMax)
        Me.m_tpClassification.Controls.Add(Me.m_lblDepthMax)
        Me.m_tpClassification.Controls.Add(Me.m_lblTempMean)
        Me.m_tpClassification.Controls.Add(Me.m_lblDepthMean)
        Me.m_tpClassification.Controls.Add(Me.m_lblTempMin)
        Me.m_tpClassification.Controls.Add(Me.m_lblDepthMin)
        Me.m_tpClassification.Controls.Add(Me.m_lblSouth)
        Me.m_tpClassification.Controls.Add(Me.m_lblEast)
        Me.m_tpClassification.Controls.Add(Me.m_lblWest)
        Me.m_tpClassification.Controls.Add(Me.m_lblNorth)
        Me.m_tpClassification.Controls.Add(Me.m_lblEcoType)
        Me.m_tpClassification.Controls.Add(Me.m_pbEnvVars)
        Me.m_tpClassification.Controls.Add(Me.m_pbEcosystem)
        Me.m_tpClassification.Controls.Add(Me.m_pbBoundingBox)
        Me.m_tpClassification.Controls.Add(Me.m_pbAreaName)
        Me.m_tpClassification.Controls.Add(Me.m_tbxTempMax)
        Me.m_tpClassification.Controls.Add(Me.m_tbxDepthMax)
        Me.m_tpClassification.Controls.Add(Me.m_tbxTempMean)
        Me.m_tpClassification.Controls.Add(Me.m_tbxDepthMean)
        Me.m_tpClassification.Controls.Add(Me.m_tbxTempMin)
        Me.m_tpClassification.Controls.Add(Me.m_tbxDepthMin)
        Me.m_tpClassification.Controls.Add(Me.m_tbxLME)
        Me.m_tpClassification.Controls.Add(Me.m_lblLME)
        Me.m_tpClassification.Controls.Add(Me.m_cmbRegion)
        Me.m_tpClassification.Controls.Add(Me.m_cmbCountry)
        Me.m_tpClassification.Controls.Add(Me.m_cmbEcoCat)
        Me.m_tpClassification.Controls.Add(Me.m_lblEcoCat)
        Me.m_tpClassification.Controls.Add(Me.m_cmbEcoType)
        Me.m_tpClassification.Controls.Add(Me.m_lblRegion)
        Me.m_tpClassification.Controls.Add(Me.m_lblCountry)
        Me.m_tpClassification.Controls.Add(Me.m_hdrEcosystem)
        Me.m_tpClassification.Controls.Add(Me.m_nudEast)
        Me.m_tpClassification.Controls.Add(Me.m_nudSouth)
        Me.m_tpClassification.Controls.Add(Me.m_nudWest)
        Me.m_tpClassification.Controls.Add(Me.m_hdrClassification)
        Me.m_tpClassification.Controls.Add(Me.m_nudNorth)
        Me.m_tpClassification.Controls.Add(Me.m_hdrArea)
        resources.ApplyResources(Me.m_tpClassification, "m_tpClassification")
        Me.m_tpClassification.Name = "m_tpClassification"
        Me.m_tpClassification.UseVisualStyleBackColor = True
        '
        'm_lblTempMax
        '
        resources.ApplyResources(Me.m_lblTempMax, "m_lblTempMax")
        Me.m_lblTempMax.Name = "m_lblTempMax"
        '
        'm_lblDepthMax
        '
        resources.ApplyResources(Me.m_lblDepthMax, "m_lblDepthMax")
        Me.m_lblDepthMax.Name = "m_lblDepthMax"
        '
        'm_lblTempMean
        '
        resources.ApplyResources(Me.m_lblTempMean, "m_lblTempMean")
        Me.m_lblTempMean.Name = "m_lblTempMean"
        '
        'm_lblDepthMean
        '
        resources.ApplyResources(Me.m_lblDepthMean, "m_lblDepthMean")
        Me.m_lblDepthMean.Name = "m_lblDepthMean"
        '
        'm_lblTempMin
        '
        resources.ApplyResources(Me.m_lblTempMin, "m_lblTempMin")
        Me.m_lblTempMin.Name = "m_lblTempMin"
        '
        'm_lblDepthMin
        '
        resources.ApplyResources(Me.m_lblDepthMin, "m_lblDepthMin")
        Me.m_lblDepthMin.Name = "m_lblDepthMin"
        '
        'm_lblSouth
        '
        resources.ApplyResources(Me.m_lblSouth, "m_lblSouth")
        Me.m_lblSouth.Name = "m_lblSouth"
        '
        'm_lblEast
        '
        resources.ApplyResources(Me.m_lblEast, "m_lblEast")
        Me.m_lblEast.Name = "m_lblEast"
        '
        'm_lblWest
        '
        resources.ApplyResources(Me.m_lblWest, "m_lblWest")
        Me.m_lblWest.Name = "m_lblWest"
        '
        'm_lblNorth
        '
        resources.ApplyResources(Me.m_lblNorth, "m_lblNorth")
        Me.m_lblNorth.Name = "m_lblNorth"
        '
        'm_pbEnvVars
        '
        resources.ApplyResources(Me.m_pbEnvVars, "m_pbEnvVars")
        Me.m_pbEnvVars.Name = "m_pbEnvVars"
        Me.m_pbEnvVars.TabStop = False
        '
        'm_pbEcosystem
        '
        resources.ApplyResources(Me.m_pbEcosystem, "m_pbEcosystem")
        Me.m_pbEcosystem.Name = "m_pbEcosystem"
        Me.m_pbEcosystem.TabStop = False
        '
        'm_pbBoundingBox
        '
        resources.ApplyResources(Me.m_pbBoundingBox, "m_pbBoundingBox")
        Me.m_pbBoundingBox.Name = "m_pbBoundingBox"
        Me.m_pbBoundingBox.TabStop = False
        '
        'm_tbxTempMax
        '
        resources.ApplyResources(Me.m_tbxTempMax, "m_tbxTempMax")
        Me.m_tbxTempMax.Name = "m_tbxTempMax"
        '
        'm_tbxDepthMax
        '
        resources.ApplyResources(Me.m_tbxDepthMax, "m_tbxDepthMax")
        Me.m_tbxDepthMax.Name = "m_tbxDepthMax"
        '
        'm_tbxTempMean
        '
        resources.ApplyResources(Me.m_tbxTempMean, "m_tbxTempMean")
        Me.m_tbxTempMean.Name = "m_tbxTempMean"
        '
        'm_tbxDepthMean
        '
        resources.ApplyResources(Me.m_tbxDepthMean, "m_tbxDepthMean")
        Me.m_tbxDepthMean.Name = "m_tbxDepthMean"
        '
        'm_tbxTempMin
        '
        resources.ApplyResources(Me.m_tbxTempMin, "m_tbxTempMin")
        Me.m_tbxTempMin.Name = "m_tbxTempMin"
        '
        'm_tbxDepthMin
        '
        resources.ApplyResources(Me.m_tbxDepthMin, "m_tbxDepthMin")
        Me.m_tbxDepthMin.Name = "m_tbxDepthMin"
        '
        'm_hdrEcosystem
        '
        Me.m_hdrEcosystem.CanCollapseParent = False
        Me.m_hdrEcosystem.CollapsedParentHeight = 0
        resources.ApplyResources(Me.m_hdrEcosystem, "m_hdrEcosystem")
        Me.m_hdrEcosystem.IsCollapsed = False
        Me.m_hdrEcosystem.Name = "m_hdrEcosystem"
        '
        'm_nudEast
        '
        resources.ApplyResources(Me.m_nudEast, "m_nudEast")
        Me.m_nudEast.InterceptMouseWheel = ScientificInterfaceShared.Controls.cEwENumericUpDown.eInterceptMouseWheelType.WhenMouseOver
        Me.m_nudEast.Name = "m_nudEast"
        '
        'm_nudSouth
        '
        resources.ApplyResources(Me.m_nudSouth, "m_nudSouth")
        Me.m_nudSouth.InterceptMouseWheel = ScientificInterfaceShared.Controls.cEwENumericUpDown.eInterceptMouseWheelType.WhenMouseOver
        Me.m_nudSouth.Name = "m_nudSouth"
        '
        'm_nudWest
        '
        Me.m_nudWest.InterceptMouseWheel = ScientificInterfaceShared.Controls.cEwENumericUpDown.eInterceptMouseWheelType.WhenMouseOver
        resources.ApplyResources(Me.m_nudWest, "m_nudWest")
        Me.m_nudWest.Name = "m_nudWest"
        '
        'm_hdrClassification
        '
        Me.m_hdrClassification.CanCollapseParent = False
        Me.m_hdrClassification.CollapsedParentHeight = 0
        resources.ApplyResources(Me.m_hdrClassification, "m_hdrClassification")
        Me.m_hdrClassification.IsCollapsed = False
        Me.m_hdrClassification.Name = "m_hdrClassification"
        '
        'm_nudNorth
        '
        resources.ApplyResources(Me.m_nudNorth, "m_nudNorth")
        Me.m_nudNorth.InterceptMouseWheel = ScientificInterfaceShared.Controls.cEwENumericUpDown.eInterceptMouseWheelType.WhenMouseOver
        Me.m_nudNorth.Name = "m_nudNorth"
        '
        'm_hdrArea
        '
        Me.m_hdrArea.CanCollapseParent = False
        Me.m_hdrArea.CollapsedParentHeight = 0
        resources.ApplyResources(Me.m_hdrArea, "m_hdrArea")
        Me.m_hdrArea.IsCollapsed = False
        Me.m_hdrArea.Name = "m_hdrArea"
        '
        'm_tpAccess
        '
        Me.m_tpAccess.Controls.Add(Me.m_hdrAccess)
        Me.m_tpAccess.Controls.Add(Me.m_pbPermissionComment)
        Me.m_tpAccess.Controls.Add(Me.m_lblPermissionComments)
        Me.m_tpAccess.Controls.Add(Me.m_tbxPermissionComments)
        Me.m_tpAccess.Controls.Add(Me.m_cbConfirmDessiminate)
        resources.ApplyResources(Me.m_tpAccess, "m_tpAccess")
        Me.m_tpAccess.Name = "m_tpAccess"
        Me.m_tpAccess.UseVisualStyleBackColor = True
        '
        'm_hdrAccess
        '
        Me.m_hdrAccess.CanCollapseParent = False
        Me.m_hdrAccess.CollapsedParentHeight = 0
        resources.ApplyResources(Me.m_hdrAccess, "m_hdrAccess")
        Me.m_hdrAccess.IsCollapsed = False
        Me.m_hdrAccess.Name = "m_hdrAccess"
        '
        'm_pbPermissionComment
        '
        resources.ApplyResources(Me.m_pbPermissionComment, "m_pbPermissionComment")
        Me.m_pbPermissionComment.Name = "m_pbPermissionComment"
        Me.m_pbPermissionComment.TabStop = False
        '
        'm_lblPermissionComments
        '
        resources.ApplyResources(Me.m_lblPermissionComments, "m_lblPermissionComments")
        Me.m_lblPermissionComments.Name = "m_lblPermissionComments"
        '
        'm_tbxPermissionComments
        '
        resources.ApplyResources(Me.m_tbxPermissionComments, "m_tbxPermissionComments")
        Me.m_tbxPermissionComments.Name = "m_tbxPermissionComments"
        '
        'm_wrkGetAgreement
        '
        '
        'm_tbxNumYears
        '
        resources.ApplyResources(Me.m_tbxNumYears, "m_tbxNumYears")
        Me.m_tbxNumYears.Name = "m_tbxNumYears"
        '
        'm_tbxFirstYear
        '
        resources.ApplyResources(Me.m_tbxFirstYear, "m_tbxFirstYear")
        Me.m_tbxFirstYear.Name = "m_tbxFirstYear"
        '
        'm_lblNoYears
        '
        resources.ApplyResources(Me.m_lblNoYears, "m_lblNoYears")
        Me.m_lblNoYears.Name = "m_lblNoYears"
        '
        'm_tbxArea
        '
        resources.ApplyResources(Me.m_tbxArea, "m_tbxArea")
        Me.m_tbxArea.Name = "m_tbxArea"
        '
        'm_lblFirstYear
        '
        resources.ApplyResources(Me.m_lblFirstYear, "m_lblFirstYear")
        Me.m_lblFirstYear.Name = "m_lblFirstYear"
        '
        'm_lblArea
        '
        resources.ApplyResources(Me.m_lblArea, "m_lblArea")
        Me.m_lblArea.Name = "m_lblArea"
        '
        'm_pbYear
        '
        resources.ApplyResources(Me.m_pbYear, "m_pbYear")
        Me.m_pbYear.Name = "m_pbYear"
        Me.m_pbYear.TabStop = False
        '
        'm_pbArea
        '
        resources.ApplyResources(Me.m_pbArea, "m_pbArea")
        Me.m_pbArea.Name = "m_pbArea"
        Me.m_pbArea.TabStop = False
        '
        'dlgEcobaseExport
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ControlBox = False
        Me.Controls.Add(Me.m_tcExport)
        Me.Controls.Add(Me.m_btnSubmit)
        Me.Controls.Add(Me.m_btnCancel)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "dlgEcobaseExport"
        Me.ShowInTaskbar = False
        CType(Me.m_pbModel, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.m_pbAuthor, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.m_pbPublication, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.m_pbDescription, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.m_pbAreaName, System.ComponentModel.ISupportInitialize).EndInit()
        Me.m_tcExport.ResumeLayout(False)
        Me.m_tpEcoBase.ResumeLayout(False)
        Me.m_tpEcoBase.PerformLayout()
        CType(Me.m_pbAgreement, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.m_tpModel.ResumeLayout(False)
        Me.m_tpModel.PerformLayout()
        CType(Me.m_pbIsAuthor, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.m_pbObjectives, System.ComponentModel.ISupportInitialize).EndInit()
        Me.m_tpPublication.ResumeLayout(False)
        Me.m_tpPublication.PerformLayout()
        CType(Me.m_pbDifference, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.m_pbRef, System.ComponentModel.ISupportInitialize).EndInit()
        Me.m_tpClassification.ResumeLayout(False)
        Me.m_tpClassification.PerformLayout()
        CType(Me.m_pbEnvVars, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.m_pbEcosystem, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.m_pbBoundingBox, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.m_nudEast, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.m_nudSouth, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.m_nudWest, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.m_nudNorth, System.ComponentModel.ISupportInitialize).EndInit()
        Me.m_tpAccess.ResumeLayout(False)
        Me.m_tpAccess.PerformLayout()
        CType(Me.m_pbPermissionComment, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.m_pbYear, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.m_pbArea, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Private WithEvents m_lblModel As System.Windows.Forms.Label
    Private WithEvents m_tbxModel As System.Windows.Forms.TextBox
    Private WithEvents m_lblDOI As System.Windows.Forms.Label
    Private WithEvents m_lblAuthor As System.Windows.Forms.Label
    Private WithEvents m_tbxDOI As System.Windows.Forms.TextBox
    Private WithEvents m_tbxAuthor As System.Windows.Forms.TextBox
    Private WithEvents m_cbConfirmDessiminate As System.Windows.Forms.CheckBox
    Private WithEvents m_btnCancel As System.Windows.Forms.Button
    Private WithEvents m_btnSubmit As System.Windows.Forms.Button
    Private WithEvents m_lblEmail As System.Windows.Forms.Label
    Private WithEvents m_tbxEmail As System.Windows.Forms.TextBox
    Private WithEvents m_llViewPublication As System.Windows.Forms.LinkLabel
    Private WithEvents m_pbModel As System.Windows.Forms.PictureBox
    Private WithEvents m_pbAuthor As System.Windows.Forms.PictureBox
    Private WithEvents m_pbPublication As System.Windows.Forms.PictureBox
    Private WithEvents m_lbLDescription As System.Windows.Forms.Label
    Private WithEvents m_tbxDescription As System.Windows.Forms.TextBox
    Private WithEvents m_pbDescription As System.Windows.Forms.PictureBox
    Private WithEvents m_tbxHyperlink As System.Windows.Forms.TextBox
    Private WithEvents m_lblHyperlink As System.Windows.Forms.Label
    Private WithEvents m_pbAreaName As System.Windows.Forms.PictureBox
    Private WithEvents m_lblEcoType As System.Windows.Forms.Label
    Private WithEvents m_lblEcoCat As System.Windows.Forms.Label
    Private WithEvents m_lblCountry As System.Windows.Forms.Label
    Private WithEvents m_lblRegion As System.Windows.Forms.Label
    Private WithEvents m_lblLME As System.Windows.Forms.Label
    Private WithEvents m_cmbEcoCat As System.Windows.Forms.ComboBox
    Private WithEvents m_cmbEcoType As System.Windows.Forms.ComboBox
    Private WithEvents m_cmbCountry As System.Windows.Forms.ComboBox
    Private WithEvents m_cmbRegion As System.Windows.Forms.ComboBox
    Private WithEvents m_tcExport As System.Windows.Forms.TabControl
    Private WithEvents m_tpModel As System.Windows.Forms.TabPage
    Private WithEvents m_tpClassification As System.Windows.Forms.TabPage
    Private WithEvents m_tpPublication As System.Windows.Forms.TabPage
    Private WithEvents m_tpAccess As System.Windows.Forms.TabPage
    Private WithEvents m_cbConfirmAuthor As System.Windows.Forms.CheckBox
    Private WithEvents m_lblReference As System.Windows.Forms.Label
    Private WithEvents m_tbxReference As System.Windows.Forms.TextBox
    Private WithEvents m_hdrArea As ScientificInterfaceShared.Controls.cEwEHeaderLabel
    Private WithEvents m_nudEast As ScientificInterfaceShared.Controls.cEwENumericUpDown
    Private WithEvents m_nudSouth As ScientificInterfaceShared.Controls.cEwENumericUpDown
    Private WithEvents m_nudWest As ScientificInterfaceShared.Controls.cEwENumericUpDown
    Private WithEvents m_hdrClassification As ScientificInterfaceShared.Controls.cEwEHeaderLabel
    Private WithEvents m_nudNorth As ScientificInterfaceShared.Controls.cEwENumericUpDown
    Private WithEvents m_lblSouth As System.Windows.Forms.Label
    Private WithEvents m_lblEast As System.Windows.Forms.Label
    Private WithEvents m_lblWest As System.Windows.Forms.Label
    Private WithEvents m_lblNorth As System.Windows.Forms.Label
    Private WithEvents m_pbIsAuthor As System.Windows.Forms.PictureBox
    Private WithEvents m_pbRef As System.Windows.Forms.PictureBox
    Private WithEvents m_tbxLME As System.Windows.Forms.TextBox
    Private WithEvents m_lblPermissionComments As System.Windows.Forms.Label
    Private WithEvents m_tbxPermissionComments As System.Windows.Forms.TextBox
    Private WithEvents m_lblTempMax As System.Windows.Forms.Label
    Private WithEvents m_lblDepthMax As System.Windows.Forms.Label
    Private WithEvents m_lblTempMean As System.Windows.Forms.Label
    Private WithEvents m_lblDepthMean As System.Windows.Forms.Label
    Private WithEvents m_lblTempMin As System.Windows.Forms.Label
    Private WithEvents m_lblDepthMin As System.Windows.Forms.Label
    Private WithEvents m_tbxTempMax As System.Windows.Forms.TextBox
    Private WithEvents m_tbxDepthMax As System.Windows.Forms.TextBox
    Private WithEvents m_tbxTempMean As System.Windows.Forms.TextBox
    Private WithEvents m_tbxDepthMean As System.Windows.Forms.TextBox
    Private WithEvents m_tbxTempMin As System.Windows.Forms.TextBox
    Private WithEvents m_tbxDepthMin As System.Windows.Forms.TextBox
    Private WithEvents m_hdrEcosystem As ScientificInterfaceShared.Controls.cEwEHeaderLabel
    Private WithEvents m_pbEnvVars As System.Windows.Forms.PictureBox
    Private WithEvents m_pbEcosystem As System.Windows.Forms.PictureBox
    Private WithEvents m_pbBoundingBox As System.Windows.Forms.PictureBox
    Private WithEvents m_lblObjectives As System.Windows.Forms.Label
    Private WithEvents m_pbObjectives As System.Windows.Forms.PictureBox
    Private WithEvents m_tbxObjectives As System.Windows.Forms.TextBox
    Private WithEvents m_pbPermissionComment As System.Windows.Forms.PictureBox
    Private WithEvents m_hdrAuthor As ScientificInterfaceShared.Controls.cEwEHeaderLabel
    Private WithEvents m_cbEcospaceUsed As System.Windows.Forms.CheckBox
    Private WithEvents m_cbFittedToTimeSeries As System.Windows.Forms.CheckBox
    Private WithEvents m_cbEcosimUsed As System.Windows.Forms.CheckBox
    Private WithEvents m_cbDifferentFromPaper As System.Windows.Forms.CheckBox
    Private WithEvents m_pbDifference As System.Windows.Forms.PictureBox
    Private WithEvents m_tbxDifference As System.Windows.Forms.TextBox
    Private WithEvents m_cbIsUpdate As System.Windows.Forms.CheckBox
    Private WithEvents m_lblExplanation As System.Windows.Forms.Label
    Private WithEvents m_hdrAccess As ScientificInterfaceShared.Controls.cEwEHeaderLabel
    Friend WithEvents PictureBox1 As System.Windows.Forms.PictureBox
    Private WithEvents m_cbEcoBaseAgreement As System.Windows.Forms.CheckBox
    Private WithEvents m_tpEcoBase As System.Windows.Forms.TabPage
    Private WithEvents m_pbAgreement As System.Windows.Forms.PictureBox
    Private WithEvents m_wrkGetAgreement As System.ComponentModel.BackgroundWorker
    Private WithEvents m_rtfAgreement As System.Windows.Forms.RichTextBox
    Private WithEvents m_tbxNumYears As System.Windows.Forms.TextBox
    Private WithEvents m_tbxFirstYear As System.Windows.Forms.TextBox
    Private WithEvents m_lblNoYears As System.Windows.Forms.Label
    Private WithEvents m_tbxArea As System.Windows.Forms.TextBox
    Private WithEvents m_lblFirstYear As System.Windows.Forms.Label
    Private WithEvents m_lblArea As System.Windows.Forms.Label
    Private WithEvents m_pbArea As System.Windows.Forms.PictureBox
    Private WithEvents m_pbYear As System.Windows.Forms.PictureBox
End Class
