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
' Copyright 1991-2012 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'
Imports ScientificInterfaceShared

Namespace Other

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ucOptionsGeneral
        Inherits System.Windows.Forms.UserControl

        'Required by the Windows Form Designer
        Private components As System.ComponentModel.IContainer

        'NOTE: The following procedure is required by the Windows Form Designer
        'It can be modified using the Windows Form Designer.  
        'Do not modify it using the code editor.
        <System.Diagnostics.DebuggerStepThrough()> _
        Private Sub InitializeComponent()
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ucOptionsGeneral))
            Me.m_gpMRU = New System.Windows.Forms.GroupBox()
            Me.m_fieldpickOutput = New ScientificInterfaceShared.Controls.ucFieldPicker()
            Me.m_fieldpickBackup = New ScientificInterfaceShared.Controls.ucFieldPicker()
            Me.m_lblSampleOutput = New System.Windows.Forms.Label()
            Me.m_lblSampleBackup = New System.Windows.Forms.Label()
            Me.m_tbOutputMask = New System.Windows.Forms.TextBox()
            Me.m_tbBackupMask = New System.Windows.Forms.TextBox()
            Me.m_lblOutput = New System.Windows.Forms.Label()
            Me.m_lblBackupFolder = New System.Windows.Forms.Label()
            Me.m_nudMRU = New ScientificInterfaceShared.Controls.cEwENumericUpDown()
            Me.m_btnDefaults = New System.Windows.Forms.Button()
            Me.m_btnClearMRU = New System.Windows.Forms.Button()
            Me.m_lblMRU = New System.Windows.Forms.Label()
            Me.m_gpMsg = New System.Windows.Forms.GroupBox()
            Me.m_cbShowTime = New System.Windows.Forms.CheckBox()
            Me.m_nudMaxNumMessages = New ScientificInterfaceShared.Controls.cEwENumericUpDown()
            Me.m_lblMaxNumMessages = New System.Windows.Forms.Label()
            Me.m_gbStartup = New System.Windows.Forms.GroupBox()
            Me.m_cbShowHost = New System.Windows.Forms.CheckBox()
            Me.m_cbDownloadUpdates = New System.Windows.Forms.CheckBox()
            Me.m_hdrCaption = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
            Me.m_lblStopTryingPre = New System.Windows.Forms.Label()
            Me.m_nudTimeOut = New ScientificInterfaceShared.Controls.cEwENumericUpDown()
            Me.m_lblStopTryingPost = New System.Windows.Forms.Label()
            Me.m_btnClearOVerwritePrompts = New System.Windows.Forms.Button()
            Me.m_gpMRU.SuspendLayout()
            CType(Me.m_nudMRU, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.m_gpMsg.SuspendLayout()
            CType(Me.m_nudMaxNumMessages, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.m_gbStartup.SuspendLayout()
            CType(Me.m_nudTimeOut, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            'm_gpMRU
            '
            resources.ApplyResources(Me.m_gpMRU, "m_gpMRU")
            Me.m_gpMRU.Controls.Add(Me.m_fieldpickOutput)
            Me.m_gpMRU.Controls.Add(Me.m_fieldpickBackup)
            Me.m_gpMRU.Controls.Add(Me.m_lblSampleOutput)
            Me.m_gpMRU.Controls.Add(Me.m_lblSampleBackup)
            Me.m_gpMRU.Controls.Add(Me.m_tbOutputMask)
            Me.m_gpMRU.Controls.Add(Me.m_tbBackupMask)
            Me.m_gpMRU.Controls.Add(Me.m_lblOutput)
            Me.m_gpMRU.Controls.Add(Me.m_lblBackupFolder)
            Me.m_gpMRU.Controls.Add(Me.m_nudMRU)
            Me.m_gpMRU.Controls.Add(Me.m_btnDefaults)
            Me.m_gpMRU.Controls.Add(Me.m_btnClearMRU)
            Me.m_gpMRU.Controls.Add(Me.m_lblMRU)
            Me.m_gpMRU.Name = "m_gpMRU"
            Me.m_gpMRU.TabStop = False
            '
            'm_fieldpickOutput
            '
            resources.ApplyResources(Me.m_fieldpickOutput, "m_fieldpickOutput")
            Me.m_fieldpickOutput.Fields = Nothing
            Me.m_fieldpickOutput.Label = "Fields"
            Me.m_fieldpickOutput.Name = "m_fieldpickOutput"
            Me.m_fieldpickOutput.ShowDirectoryPicker = True
            Me.m_fieldpickOutput.TypeFormatter = Nothing
            Me.m_fieldpickOutput.UIContext = Nothing
            '
            'm_fieldpickBackup
            '
            resources.ApplyResources(Me.m_fieldpickBackup, "m_fieldpickBackup")
            Me.m_fieldpickBackup.Fields = Nothing
            Me.m_fieldpickBackup.Label = "Fields"
            Me.m_fieldpickBackup.Name = "m_fieldpickBackup"
            Me.m_fieldpickBackup.ShowDirectoryPicker = True
            Me.m_fieldpickBackup.TypeFormatter = Nothing
            Me.m_fieldpickBackup.UIContext = Nothing
            '
            'm_lblSampleOutput
            '
            resources.ApplyResources(Me.m_lblSampleOutput, "m_lblSampleOutput")
            Me.m_lblSampleOutput.BackColor = System.Drawing.SystemColors.Control
            Me.m_lblSampleOutput.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
            Me.m_lblSampleOutput.ForeColor = System.Drawing.SystemColors.GrayText
            Me.m_lblSampleOutput.Name = "m_lblSampleOutput"
            '
            'm_lblSampleBackup
            '
            resources.ApplyResources(Me.m_lblSampleBackup, "m_lblSampleBackup")
            Me.m_lblSampleBackup.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
            Me.m_lblSampleBackup.ForeColor = System.Drawing.SystemColors.GrayText
            Me.m_lblSampleBackup.Name = "m_lblSampleBackup"
            '
            'm_tbOutputMask
            '
            resources.ApplyResources(Me.m_tbOutputMask, "m_tbOutputMask")
            Me.m_tbOutputMask.Name = "m_tbOutputMask"
            '
            'm_tbBackupMask
            '
            resources.ApplyResources(Me.m_tbBackupMask, "m_tbBackupMask")
            Me.m_tbBackupMask.Name = "m_tbBackupMask"
            '
            'm_lblOutput
            '
            resources.ApplyResources(Me.m_lblOutput, "m_lblOutput")
            Me.m_lblOutput.Name = "m_lblOutput"
            '
            'm_lblBackupFolder
            '
            resources.ApplyResources(Me.m_lblBackupFolder, "m_lblBackupFolder")
            Me.m_lblBackupFolder.Name = "m_lblBackupFolder"
            '
            'm_nudMRU
            '
            resources.ApplyResources(Me.m_nudMRU, "m_nudMRU")
            Me.m_nudMRU.Maximum = New Decimal(New Integer() {42, 0, 0, 0})
            Me.m_nudMRU.Name = "m_nudMRU"
            Me.m_nudMRU.Value = New Decimal(New Integer() {10, 0, 0, 0})
            '
            'm_btnDefaults
            '
            resources.ApplyResources(Me.m_btnDefaults, "m_btnDefaults")
            Me.m_btnDefaults.Name = "m_btnDefaults"
            Me.m_btnDefaults.UseVisualStyleBackColor = True
            '
            'm_btnClearMRU
            '
            resources.ApplyResources(Me.m_btnClearMRU, "m_btnClearMRU")
            Me.m_btnClearMRU.Name = "m_btnClearMRU"
            Me.m_btnClearMRU.UseVisualStyleBackColor = True
            '
            'm_lblMRU
            '
            resources.ApplyResources(Me.m_lblMRU, "m_lblMRU")
            Me.m_lblMRU.Name = "m_lblMRU"
            '
            'm_gpMsg
            '
            resources.ApplyResources(Me.m_gpMsg, "m_gpMsg")
            Me.m_gpMsg.Controls.Add(Me.m_cbShowTime)
            Me.m_gpMsg.Controls.Add(Me.m_nudMaxNumMessages)
            Me.m_gpMsg.Controls.Add(Me.m_lblMaxNumMessages)
            Me.m_gpMsg.Name = "m_gpMsg"
            Me.m_gpMsg.TabStop = False
            '
            'm_cbShowTime
            '
            resources.ApplyResources(Me.m_cbShowTime, "m_cbShowTime")
            Me.m_cbShowTime.Name = "m_cbShowTime"
            Me.m_cbShowTime.UseVisualStyleBackColor = True
            '
            'm_nudMaxNumMessages
            '
            resources.ApplyResources(Me.m_nudMaxNumMessages, "m_nudMaxNumMessages")
            Me.m_nudMaxNumMessages.Maximum = New Decimal(New Integer() {2000, 0, 0, 0})
            Me.m_nudMaxNumMessages.Name = "m_nudMaxNumMessages"
            Me.m_nudMaxNumMessages.Value = New Decimal(New Integer() {10, 0, 0, 0})
            '
            'm_lblMaxNumMessages
            '
            resources.ApplyResources(Me.m_lblMaxNumMessages, "m_lblMaxNumMessages")
            Me.m_lblMaxNumMessages.Name = "m_lblMaxNumMessages"
            '
            'm_gbStartup
            '
            resources.ApplyResources(Me.m_gbStartup, "m_gbStartup")
            Me.m_gbStartup.Controls.Add(Me.m_lblStopTryingPost)
            Me.m_gbStartup.Controls.Add(Me.m_lblStopTryingPre)
            Me.m_gbStartup.Controls.Add(Me.m_cbShowHost)
            Me.m_gbStartup.Controls.Add(Me.m_cbDownloadUpdates)
            Me.m_gbStartup.Controls.Add(Me.m_nudTimeOut)
            Me.m_gbStartup.Controls.Add(Me.m_btnClearOVerwritePrompts)
            Me.m_gbStartup.Name = "m_gbStartup"
            Me.m_gbStartup.TabStop = False
            '
            'm_cbShowHost
            '
            resources.ApplyResources(Me.m_cbShowHost, "m_cbShowHost")
            Me.m_cbShowHost.Name = "m_cbShowHost"
            Me.m_cbShowHost.UseVisualStyleBackColor = True
            '
            'm_cbDownloadUpdates
            '
            resources.ApplyResources(Me.m_cbDownloadUpdates, "m_cbDownloadUpdates")
            Me.m_cbDownloadUpdates.Name = "m_cbDownloadUpdates"
            Me.m_cbDownloadUpdates.UseVisualStyleBackColor = True
            '
            'm_hdrCaption
            '
            Me.m_hdrCaption.CanCollapseParent = False
            Me.m_hdrCaption.CollapsedParentHeight = 0
            resources.ApplyResources(Me.m_hdrCaption, "m_hdrCaption")
            Me.m_hdrCaption.IsCollapsed = False
            Me.m_hdrCaption.Name = "m_hdrCaption"
            '
            'm_lblStopTryingPre
            '
            resources.ApplyResources(Me.m_lblStopTryingPre, "m_lblStopTryingPre")
            Me.m_lblStopTryingPre.Name = "m_lblStopTryingPre"
            '
            'm_nudTimeOut
            '
            resources.ApplyResources(Me.m_nudTimeOut, "m_nudTimeOut")
            Me.m_nudTimeOut.Maximum = New Decimal(New Integer() {60, 0, 0, 0})
            Me.m_nudTimeOut.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
            Me.m_nudTimeOut.Name = "m_nudTimeOut"
            Me.m_nudTimeOut.Value = New Decimal(New Integer() {10, 0, 0, 0})
            '
            'm_lblStopTryingPost
            '
            resources.ApplyResources(Me.m_lblStopTryingPost, "m_lblStopTryingPost")
            Me.m_lblStopTryingPost.Name = "m_lblStopTryingPost"
            '
            'm_btnClearOVerwritePrompts
            '
            resources.ApplyResources(Me.m_btnClearOVerwritePrompts, "m_btnClearOVerwritePrompts")
            Me.m_btnClearOVerwritePrompts.Name = "m_btnClearOVerwritePrompts"
            Me.m_btnClearOVerwritePrompts.UseVisualStyleBackColor = True
            '
            'ucOptionsGeneral
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.m_gbStartup)
            Me.Controls.Add(Me.m_gpMsg)
            Me.Controls.Add(Me.m_hdrCaption)
            Me.Controls.Add(Me.m_gpMRU)
            Me.Name = "ucOptionsGeneral"
            Me.m_gpMRU.ResumeLayout(False)
            Me.m_gpMRU.PerformLayout()
            CType(Me.m_nudMRU, System.ComponentModel.ISupportInitialize).EndInit()
            Me.m_gpMsg.ResumeLayout(False)
            Me.m_gpMsg.PerformLayout()
            CType(Me.m_nudMaxNumMessages, System.ComponentModel.ISupportInitialize).EndInit()
            Me.m_gbStartup.ResumeLayout(False)
            Me.m_gbStartup.PerformLayout()
            CType(Me.m_nudTimeOut, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)

        End Sub
        Private WithEvents m_lblMRU As System.Windows.Forms.Label
        Private WithEvents m_lblMaxNumMessages As System.Windows.Forms.Label
        Private WithEvents m_btnClearMRU As System.Windows.Forms.Button
        Private WithEvents m_gpMRU As System.Windows.Forms.GroupBox
        Private WithEvents m_gpMsg As System.Windows.Forms.GroupBox
        Private WithEvents m_gbStartup As System.Windows.Forms.GroupBox
        Private WithEvents m_cbDownloadUpdates As System.Windows.Forms.CheckBox
        Private WithEvents m_hdrCaption As cEwEHeaderLabel
        Private WithEvents m_cbShowTime As System.Windows.Forms.CheckBox
        Private WithEvents m_lblBackupFolder As System.Windows.Forms.Label
        Private WithEvents m_tbBackupMask As System.Windows.Forms.TextBox
        Private WithEvents m_lblSampleBackup As System.Windows.Forms.Label
        Private WithEvents m_tbOutputMask As System.Windows.Forms.TextBox
        Private WithEvents m_lblOutput As System.Windows.Forms.Label
        Private WithEvents m_fieldpickBackup As ScientificInterfaceShared.Controls.ucFieldPicker
        Private WithEvents m_fieldpickOutput As ScientificInterfaceShared.Controls.ucFieldPicker
        Private WithEvents m_btnDefaults As System.Windows.Forms.Button
        Private WithEvents m_lblSampleOutput As System.Windows.Forms.Label
        Private WithEvents m_nudMaxNumMessages As ScientificInterfaceShared.Controls.cEwENumericUpDown
        Private WithEvents m_nudMRU As ScientificInterfaceShared.Controls.cEwENumericUpDown
        Private WithEvents m_cbShowHost As System.Windows.Forms.CheckBox
        Private WithEvents m_lblStopTryingPost As System.Windows.Forms.Label
        Private WithEvents m_lblStopTryingPre As System.Windows.Forms.Label
        Private WithEvents m_nudTimeOut As ScientificInterfaceShared.Controls.cEwENumericUpDown
        Private WithEvents m_btnClearOVerwritePrompts As System.Windows.Forms.Button

    End Class

End Namespace

