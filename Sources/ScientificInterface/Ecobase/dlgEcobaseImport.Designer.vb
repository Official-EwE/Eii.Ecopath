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
        Me.m_cbAccept = New System.Windows.Forms.CheckBox()
        Me.m_llViewEcobaseDataAgreement = New System.Windows.Forms.LinkLabel()
        CType(Me.m_scEcobaseContent, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.m_scEcobaseContent.Panel1.SuspendLayout()
        Me.m_scEcobaseContent.Panel2.SuspendLayout()
        Me.m_scEcobaseContent.SuspendLayout()
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
        'm_cbAccept
        '
        resources.ApplyResources(Me.m_cbAccept, "m_cbAccept")
        Me.m_cbAccept.Name = "m_cbAccept"
        Me.m_cbAccept.UseVisualStyleBackColor = True
        '
        'm_llViewEcobaseDataAgreement
        '
        resources.ApplyResources(Me.m_llViewEcobaseDataAgreement, "m_llViewEcobaseDataAgreement")
        Me.m_llViewEcobaseDataAgreement.Name = "m_llViewEcobaseDataAgreement"
        Me.m_llViewEcobaseDataAgreement.TabStop = True
        '
        'dlgEcobaseImport
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.m_btnCancel
        Me.Controls.Add(Me.m_llViewEcobaseDataAgreement)
        Me.Controls.Add(Me.m_cbAccept)
        Me.Controls.Add(Me.m_btnOK)
        Me.Controls.Add(Me.m_btnCancel)
        Me.Controls.Add(Me.m_scEcobaseContent)
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
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Private WithEvents m_scEcobaseContent As System.Windows.Forms.SplitContainer
    Private WithEvents m_lbxModels As System.Windows.Forms.ListBox
    Private WithEvents m_btnOK As System.Windows.Forms.Button
    Private WithEvents m_btnCancel As System.Windows.Forms.Button
    Private WithEvents m_wrkGetModels As System.ComponentModel.BackgroundWorker
    Private WithEvents m_hdrModels As ScientificInterfaceShared.Controls.cEwEHeaderLabel
    Private WithEvents m_hdrDetails As ScientificInterfaceShared.Controls.cEwEHeaderLabel
    Private WithEvents m_cbAccept As System.Windows.Forms.CheckBox
    Private WithEvents m_llViewEcobaseDataAgreement As System.Windows.Forms.LinkLabel
    Private WithEvents m_browser As System.Windows.Forms.WebBrowser
End Class
