' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports ScientificInterfaceShared.Forms

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmDefineTransects
    Inherits frmEwE

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(disposing As Boolean)
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmDefineTransects))
        Me.m_scMain = New System.Windows.Forms.SplitContainer()
        Me.m_mapzoom = New ScientificInterfaceShared.Controls.Map.ucMapZoom()
        Me.m_toolstrip = New ScientificInterfaceShared.Controls.Map.ucMapZoomToolbar()
        Me.m_tlpContent = New System.Windows.Forms.TableLayoutPanel()
        Me.m_lbxTransects = New System.Windows.Forms.ListBox()
        Me.m_btnDeleteTransect = New System.Windows.Forms.Button()
        Me.m_hdrCredits = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.m_pbCEFAS = New System.Windows.Forms.PictureBox()
        Me.m_pbINSITE = New System.Windows.Forms.PictureBox()
        CType(Me.m_scMain, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.m_scMain.Panel1.SuspendLayout()
        Me.m_scMain.Panel2.SuspendLayout()
        Me.m_scMain.SuspendLayout()
        Me.m_tlpContent.SuspendLayout()
        CType(Me.m_pbCEFAS, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.m_pbINSITE, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'm_scMain
        '
        resources.ApplyResources(Me.m_scMain, "m_scMain")
        Me.m_scMain.FixedPanel = System.Windows.Forms.FixedPanel.Panel2
        Me.m_scMain.Name = "m_scMain"
        '
        'm_scMain.Panel1
        '
        Me.m_scMain.Panel1.Controls.Add(Me.m_mapzoom)
        Me.m_scMain.Panel1.Controls.Add(Me.m_toolstrip)
        '
        'm_scMain.Panel2
        '
        Me.m_scMain.Panel2.Controls.Add(Me.m_tlpContent)
        '
        'm_mapzoom
        '
        resources.ApplyResources(Me.m_mapzoom, "m_mapzoom")
        Me.m_mapzoom.Name = "m_mapzoom"
        Me.m_mapzoom.UIContext = Nothing
        '
        'm_toolstrip
        '
        resources.ApplyResources(Me.m_toolstrip, "m_toolstrip")
        Me.m_toolstrip.Name = "m_toolstrip"
        Me.m_toolstrip.UIContext = Nothing
        '
        'm_tlpContent
        '
        resources.ApplyResources(Me.m_tlpContent, "m_tlpContent")
        Me.m_tlpContent.Controls.Add(Me.m_lbxTransects, 0, 0)
        Me.m_tlpContent.Controls.Add(Me.m_btnDeleteTransect, 0, 1)
        Me.m_tlpContent.Controls.Add(Me.m_hdrCredits, 0, 2)
        Me.m_tlpContent.Controls.Add(Me.m_pbCEFAS, 0, 3)
        Me.m_tlpContent.Controls.Add(Me.m_pbINSITE, 0, 4)
        Me.m_tlpContent.Name = "m_tlpContent"
        '
        'm_lbxTransects
        '
        resources.ApplyResources(Me.m_lbxTransects, "m_lbxTransects")
        Me.m_lbxTransects.FormattingEnabled = True
        Me.m_lbxTransects.Name = "m_lbxTransects"
        '
        'm_btnDeleteTransect
        '
        resources.ApplyResources(Me.m_btnDeleteTransect, "m_btnDeleteTransect")
        Me.m_btnDeleteTransect.Name = "m_btnDeleteTransect"
        Me.m_btnDeleteTransect.UseVisualStyleBackColor = True
        '
        'm_hdrCredits
        '
        Me.m_hdrCredits.CanCollapseParent = False
        Me.m_hdrCredits.CollapsedParentHeight = 0
        resources.ApplyResources(Me.m_hdrCredits, "m_hdrCredits")
        Me.m_hdrCredits.IsCollapsed = False
        Me.m_hdrCredits.Name = "m_hdrCredits"
        '
        'm_pbCEFAS
        '
        Me.m_pbCEFAS.BackgroundImage = Global.EwETransectExtractionPlugin.My.Resources.Resources.cefas_logo
        resources.ApplyResources(Me.m_pbCEFAS, "m_pbCEFAS")
        Me.m_pbCEFAS.Name = "m_pbCEFAS"
        Me.m_pbCEFAS.TabStop = False
        '
        'm_pbINSITE
        '
        Me.m_pbINSITE.BackgroundImage = Global.EwETransectExtractionPlugin.My.Resources.Resources.insite
        resources.ApplyResources(Me.m_pbINSITE, "m_pbINSITE")
        Me.m_pbINSITE.Name = "m_pbINSITE"
        Me.m_pbINSITE.TabStop = False
        '
        'frmDefineTransects
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.m_scMain)
        Me.Name = "frmDefineTransects"
        Me.TabText = ""
        Me.m_scMain.Panel1.ResumeLayout(False)
        Me.m_scMain.Panel1.PerformLayout()
        Me.m_scMain.Panel2.ResumeLayout(False)
        CType(Me.m_scMain, System.ComponentModel.ISupportInitialize).EndInit()
        Me.m_scMain.ResumeLayout(False)
        Me.m_tlpContent.ResumeLayout(False)
        CType(Me.m_pbCEFAS, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.m_pbINSITE, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Private WithEvents m_scMain As System.Windows.Forms.SplitContainer
    Private WithEvents m_mapzoom As ScientificInterfaceShared.Controls.Map.ucMapZoom
    Private WithEvents m_toolstrip As ScientificInterfaceShared.Controls.Map.ucMapZoomToolbar
    Friend WithEvents m_lbxTransects As System.Windows.Forms.ListBox
    Private WithEvents m_btnDeleteTransect As System.Windows.Forms.Button
    Private WithEvents m_tlpContent As System.Windows.Forms.TableLayoutPanel
    Private WithEvents m_hdrCredits As ScientificInterfaceShared.Controls.cEwEHeaderLabel
    Private WithEvents m_pbCEFAS As System.Windows.Forms.PictureBox
    Private WithEvents m_pbINSITE As System.Windows.Forms.PictureBox
End Class
