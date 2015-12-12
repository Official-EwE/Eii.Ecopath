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
Imports ScientificInterfaceShared.Controls
Imports ScientificInterfaceShared.Forms

Partial Class dlgEcobaseImport
    Inherits frmEwE

    'Form overrides dispose to clean up the component list.
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

    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(dlgEcobaseImport))
        Me.m_scEcobaseContent = New System.Windows.Forms.SplitContainer()
        Me.m_hdrModels = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.m_lbxModels = New System.Windows.Forms.ListBox()
        Me.m_browser = New System.Windows.Forms.WebBrowser()
        Me.m_hdrDetails = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.m_btnCancel = New System.Windows.Forms.Button()
        Me.m_btnOK = New System.Windows.Forms.Button()
        Me.m_wrkGetModels = New System.ComponentModel.BackgroundWorker()
        Me.m_tsFilter = New ScientificInterfaceShared.Controls.cEwEToolstrip()
        Me.m_tstbSearch = New System.Windows.Forms.ToolStripTextBox()
        Me.m_tsddValue = New System.Windows.Forms.ToolStripDropDownButton()
        Me.m_tsmiNone = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_tsmiAuthor = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_tsmiCountry = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_tsmiRegion = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_tsmiLME = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_tsmiEcoCat = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_tsmiEcoType = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_tsmiDepth = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_tsmiTemperature = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_tcContent = New System.Windows.Forms.TabControl()
        Me.m_tpAgreement = New System.Windows.Forms.TabPage()
        Me.m_pbAgreement = New System.Windows.Forms.PictureBox()
        Me.m_rtfAgreement = New System.Windows.Forms.RichTextBox()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.m_cbEcoBaseAgreement = New System.Windows.Forms.CheckBox()
        Me.m_tpImport = New System.Windows.Forms.TabPage()
        Me.m_wrkGetAgreement = New System.ComponentModel.BackgroundWorker()
        CType(Me.m_scEcobaseContent, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.m_scEcobaseContent.Panel1.SuspendLayout()
        Me.m_scEcobaseContent.Panel2.SuspendLayout()
        Me.m_scEcobaseContent.SuspendLayout()
        Me.m_tsFilter.SuspendLayout()
        Me.m_tcContent.SuspendLayout()
        Me.m_tpAgreement.SuspendLayout()
        CType(Me.m_pbAgreement, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.m_tpImport.SuspendLayout()
        Me.SuspendLayout()
        '
        'm_scEcobaseContent
        '
        resources.ApplyResources(Me.m_scEcobaseContent, "m_scEcobaseContent")
        Me.m_scEcobaseContent.FixedPanel = System.Windows.Forms.FixedPanel.Panel1
        Me.m_scEcobaseContent.Name = "m_scEcobaseContent"
        '
        'm_scEcobaseContent.Panel1
        '
        Me.m_scEcobaseContent.Panel1.Controls.Add(Me.m_hdrModels)
        Me.m_scEcobaseContent.Panel1.Controls.Add(Me.m_lbxModels)
        '
        'm_scEcobaseContent.Panel2
        '
        Me.m_scEcobaseContent.Panel2.Controls.Add(Me.m_browser)
        Me.m_scEcobaseContent.Panel2.Controls.Add(Me.m_hdrDetails)
        '
        'm_hdrModels
        '
        Me.m_hdrModels.CanCollapseParent = False
        Me.m_hdrModels.CollapsedParentHeight = 0
        resources.ApplyResources(Me.m_hdrModels, "m_hdrModels")
        Me.m_hdrModels.IsCollapsed = False
        Me.m_hdrModels.Name = "m_hdrModels"
        '
        'm_lbxModels
        '
        resources.ApplyResources(Me.m_lbxModels, "m_lbxModels")
        Me.m_lbxModels.FormattingEnabled = True
        Me.m_lbxModels.Name = "m_lbxModels"
        Me.m_lbxModels.Sorted = True
        '
        'm_browser
        '
        resources.ApplyResources(Me.m_browser, "m_browser")
        Me.m_browser.IsWebBrowserContextMenuEnabled = False
        Me.m_browser.Name = "m_browser"
        Me.m_browser.ScriptErrorsSuppressed = True
        '
        'm_hdrDetails
        '
        Me.m_hdrDetails.CanCollapseParent = False
        Me.m_hdrDetails.CollapsedParentHeight = 0
        resources.ApplyResources(Me.m_hdrDetails, "m_hdrDetails")
        Me.m_hdrDetails.IsCollapsed = False
        Me.m_hdrDetails.Name = "m_hdrDetails"
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
        'm_wrkGetModels
        '
        '
        'm_tsFilter
        '
        Me.m_tsFilter.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
        Me.m_tsFilter.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tstbSearch, Me.m_tsddValue})
        resources.ApplyResources(Me.m_tsFilter, "m_tsFilter")
        Me.m_tsFilter.Name = "m_tsFilter"
        Me.m_tsFilter.RenderMode = System.Windows.Forms.ToolStripRenderMode.System
        '
        'm_tstbSearch
        '
        Me.m_tstbSearch.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.m_tstbSearch.Name = "m_tstbSearch"
        resources.ApplyResources(Me.m_tstbSearch, "m_tstbSearch")
        '
        'm_tsddValue
        '
        Me.m_tsddValue.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.m_tsddValue.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tsmiNone, Me.m_tsmiAuthor, Me.m_tsmiCountry, Me.m_tsmiRegion, Me.m_tsmiLME, Me.m_tsmiEcoCat, Me.m_tsmiEcoType, Me.m_tsmiDepth, Me.m_tsmiTemperature})
        resources.ApplyResources(Me.m_tsddValue, "m_tsddValue")
        Me.m_tsddValue.Name = "m_tsddValue"
        '
        'm_tsmiNone
        '
        Me.m_tsmiNone.Name = "m_tsmiNone"
        resources.ApplyResources(Me.m_tsmiNone, "m_tsmiNone")
        Me.m_tsmiNone.Tag = "0"
        '
        'm_tsmiAuthor
        '
        Me.m_tsmiAuthor.Name = "m_tsmiAuthor"
        resources.ApplyResources(Me.m_tsmiAuthor, "m_tsmiAuthor")
        Me.m_tsmiAuthor.Tag = "1"
        '
        'm_tsmiCountry
        '
        Me.m_tsmiCountry.Name = "m_tsmiCountry"
        resources.ApplyResources(Me.m_tsmiCountry, "m_tsmiCountry")
        Me.m_tsmiCountry.Tag = "2"
        '
        'm_tsmiRegion
        '
        Me.m_tsmiRegion.Name = "m_tsmiRegion"
        resources.ApplyResources(Me.m_tsmiRegion, "m_tsmiRegion")
        Me.m_tsmiRegion.Tag = "3"
        '
        'm_tsmiLME
        '
        Me.m_tsmiLME.Name = "m_tsmiLME"
        resources.ApplyResources(Me.m_tsmiLME, "m_tsmiLME")
        Me.m_tsmiLME.Tag = "4"
        '
        'm_tsmiEcoCat
        '
        Me.m_tsmiEcoCat.Name = "m_tsmiEcoCat"
        resources.ApplyResources(Me.m_tsmiEcoCat, "m_tsmiEcoCat")
        Me.m_tsmiEcoCat.Tag = "5"
        '
        'm_tsmiEcoType
        '
        Me.m_tsmiEcoType.Name = "m_tsmiEcoType"
        resources.ApplyResources(Me.m_tsmiEcoType, "m_tsmiEcoType")
        Me.m_tsmiEcoType.Tag = "6"
        '
        'm_tsmiDepth
        '
        Me.m_tsmiDepth.Name = "m_tsmiDepth"
        resources.ApplyResources(Me.m_tsmiDepth, "m_tsmiDepth")
        Me.m_tsmiDepth.Tag = "7"
        '
        'm_tsmiTemperature
        '
        Me.m_tsmiTemperature.Name = "m_tsmiTemperature"
        resources.ApplyResources(Me.m_tsmiTemperature, "m_tsmiTemperature")
        Me.m_tsmiTemperature.Tag = "8"
        '
        'm_tcContent
        '
        resources.ApplyResources(Me.m_tcContent, "m_tcContent")
        Me.m_tcContent.Controls.Add(Me.m_tpAgreement)
        Me.m_tcContent.Controls.Add(Me.m_tpImport)
        Me.m_tcContent.Name = "m_tcContent"
        Me.m_tcContent.SelectedIndex = 0
        '
        'm_tpAgreement
        '
        Me.m_tpAgreement.Controls.Add(Me.m_pbAgreement)
        Me.m_tpAgreement.Controls.Add(Me.m_rtfAgreement)
        Me.m_tpAgreement.Controls.Add(Me.PictureBox1)
        Me.m_tpAgreement.Controls.Add(Me.m_cbEcoBaseAgreement)
        resources.ApplyResources(Me.m_tpAgreement, "m_tpAgreement")
        Me.m_tpAgreement.Name = "m_tpAgreement"
        Me.m_tpAgreement.UseVisualStyleBackColor = True
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
        'm_tpImport
        '
        Me.m_tpImport.Controls.Add(Me.m_scEcobaseContent)
        Me.m_tpImport.Controls.Add(Me.m_tsFilter)
        resources.ApplyResources(Me.m_tpImport, "m_tpImport")
        Me.m_tpImport.Name = "m_tpImport"
        Me.m_tpImport.UseVisualStyleBackColor = True
        '
        'm_wrkGetAgreement
        '
        '
        'dlgEcobaseImport
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.m_btnCancel
        Me.Controls.Add(Me.m_tcContent)
        Me.Controls.Add(Me.m_btnOK)
        Me.Controls.Add(Me.m_btnCancel)
        Me.DoubleBuffered = True
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "dlgEcobaseImport"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show
        Me.TabText = ""
        Me.m_scEcobaseContent.Panel1.ResumeLayout(False)
        Me.m_scEcobaseContent.Panel2.ResumeLayout(False)
        CType(Me.m_scEcobaseContent, System.ComponentModel.ISupportInitialize).EndInit()
        Me.m_scEcobaseContent.ResumeLayout(False)
        Me.m_tsFilter.ResumeLayout(False)
        Me.m_tsFilter.PerformLayout()
        Me.m_tcContent.ResumeLayout(False)
        Me.m_tpAgreement.ResumeLayout(False)
        Me.m_tpAgreement.PerformLayout()
        CType(Me.m_pbAgreement, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.m_tpImport.ResumeLayout(False)
        Me.m_tpImport.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Private WithEvents m_scEcobaseContent As System.Windows.Forms.SplitContainer
    Private WithEvents m_lbxModels As System.Windows.Forms.ListBox
    Private WithEvents m_btnOK As System.Windows.Forms.Button
    Private WithEvents m_btnCancel As System.Windows.Forms.Button
    Private WithEvents m_wrkGetModels As System.ComponentModel.BackgroundWorker
    Private WithEvents m_hdrModels As ScientificInterfaceShared.Controls.cEwEHeaderLabel
    Private WithEvents m_hdrDetails As ScientificInterfaceShared.Controls.cEwEHeaderLabel
    Private WithEvents m_browser As System.Windows.Forms.WebBrowser
    Private WithEvents m_tsFilter As cEwEToolstrip
    Private WithEvents m_tcContent As System.Windows.Forms.TabControl
    Private WithEvents m_tpAgreement As System.Windows.Forms.TabPage
    Private WithEvents m_tpImport As System.Windows.Forms.TabPage
    Private WithEvents m_pbAgreement As System.Windows.Forms.PictureBox
    Private WithEvents m_rtfAgreement As System.Windows.Forms.RichTextBox
    Friend WithEvents PictureBox1 As System.Windows.Forms.PictureBox
    Private WithEvents m_cbEcoBaseAgreement As System.Windows.Forms.CheckBox
    Private WithEvents m_wrkGetAgreement As System.ComponentModel.BackgroundWorker
    Private WithEvents m_tstbSearch As System.Windows.Forms.ToolStripTextBox
    Private WithEvents m_tsddValue As System.Windows.Forms.ToolStripDropDownButton
    Friend WithEvents m_tsmiCountry As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_tsmiRegion As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_tsmiLME As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_tsmiEcoCat As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_tsmiEcoType As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents m_tsmiDepth As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents m_tsmiAuthor As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_tsmiTemperature As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_tsmiNone As System.Windows.Forms.ToolStripMenuItem
End Class
