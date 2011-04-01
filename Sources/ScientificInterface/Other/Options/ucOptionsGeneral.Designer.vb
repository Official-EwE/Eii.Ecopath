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
            Me.m_gpMRU = New System.Windows.Forms.GroupBox
            Me.m_fieldpickOutput = New ScientificInterfaceShared.Controls.ucFieldPicker
            Me.m_fieldpickBackup = New ScientificInterfaceShared.Controls.ucFieldPicker
            Me.m_lblSample = New System.Windows.Forms.Label
            Me.m_tbOutput = New System.Windows.Forms.TextBox
            Me.m_tbBackupMask = New System.Windows.Forms.TextBox
            Me.m_lblExample = New System.Windows.Forms.Label
            Me.m_lblOutput = New System.Windows.Forms.Label
            Me.m_lblBackup = New System.Windows.Forms.Label
            Me.m_nudMRU = New System.Windows.Forms.NumericUpDown
            Me.m_btnClearMRU = New System.Windows.Forms.Button
            Me.m_lblMRU = New System.Windows.Forms.Label
            Me.m_gpMsg = New System.Windows.Forms.GroupBox
            Me.m_cbShowTime = New System.Windows.Forms.CheckBox
            Me.m_nudMaxNumMessages = New System.Windows.Forms.NumericUpDown
            Me.m_lblMaxNumMessages = New System.Windows.Forms.Label
            Me.m_gbStartup = New System.Windows.Forms.GroupBox
            Me.m_lblResetOverwritePrompts = New System.Windows.Forms.Label
            Me.m_btnResetOverwritePrompts = New System.Windows.Forms.Button
            Me.m_cbCheckEwE6 = New System.Windows.Forms.CheckBox
            Me.m_cbDownloadUpdates = New System.Windows.Forms.CheckBox
            Me.m_hdrCaption = New ScientificInterfaceShared.Controls.cEwEHeaderLabel
            Me.m_gpMRU.SuspendLayout()
            CType(Me.m_nudMRU, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.m_gpMsg.SuspendLayout()
            CType(Me.m_nudMaxNumMessages, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.m_gbStartup.SuspendLayout()
            Me.SuspendLayout()
            '
            'm_gpMRU
            '
            resources.ApplyResources(Me.m_gpMRU, "m_gpMRU")
            Me.m_gpMRU.Controls.Add(Me.m_fieldpickOutput)
            Me.m_gpMRU.Controls.Add(Me.m_fieldpickBackup)
            Me.m_gpMRU.Controls.Add(Me.m_lblSample)
            Me.m_gpMRU.Controls.Add(Me.m_tbOutput)
            Me.m_gpMRU.Controls.Add(Me.m_tbBackupMask)
            Me.m_gpMRU.Controls.Add(Me.m_lblExample)
            Me.m_gpMRU.Controls.Add(Me.m_lblOutput)
            Me.m_gpMRU.Controls.Add(Me.m_lblBackup)
            Me.m_gpMRU.Controls.Add(Me.m_nudMRU)
            Me.m_gpMRU.Controls.Add(Me.m_btnClearMRU)
            Me.m_gpMRU.Controls.Add(Me.m_lblMRU)
            Me.m_gpMRU.Name = "m_gpMRU"
            Me.m_gpMRU.TabStop = False
            '
            'm_fieldpickOutput
            '
            resources.ApplyResources(Me.m_fieldpickOutput, "m_fieldpickOutput")
            Me.m_fieldpickOutput.Fields = Nothing
            Me.m_fieldpickOutput.Label = "Folder"
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
            'm_lblSample
            '
            resources.ApplyResources(Me.m_lblSample, "m_lblSample")
            Me.m_lblSample.Name = "m_lblSample"
            '
            'm_tbOutput
            '
            resources.ApplyResources(Me.m_tbOutput, "m_tbOutput")
            Me.m_tbOutput.Name = "m_tbOutput"
            '
            'm_tbBackupMask
            '
            resources.ApplyResources(Me.m_tbBackupMask, "m_tbBackupMask")
            Me.m_tbBackupMask.Name = "m_tbBackupMask"
            '
            'm_lblExample
            '
            resources.ApplyResources(Me.m_lblExample, "m_lblExample")
            Me.m_lblExample.Name = "m_lblExample"
            '
            'm_lblOutput
            '
            resources.ApplyResources(Me.m_lblOutput, "m_lblOutput")
            Me.m_lblOutput.Name = "m_lblOutput"
            '
            'm_lblBackup
            '
            resources.ApplyResources(Me.m_lblBackup, "m_lblBackup")
            Me.m_lblBackup.Name = "m_lblBackup"
            '
            'm_nudMRU
            '
            resources.ApplyResources(Me.m_nudMRU, "m_nudMRU")
            Me.m_nudMRU.Maximum = New Decimal(New Integer() {42, 0, 0, 0})
            Me.m_nudMRU.Name = "m_nudMRU"
            Me.m_nudMRU.Value = New Decimal(New Integer() {10, 0, 0, 0})
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
            Me.m_gbStartup.Controls.Add(Me.m_lblResetOverwritePrompts)
            Me.m_gbStartup.Controls.Add(Me.m_btnResetOverwritePrompts)
            Me.m_gbStartup.Controls.Add(Me.m_cbCheckEwE6)
            Me.m_gbStartup.Controls.Add(Me.m_cbDownloadUpdates)
            Me.m_gbStartup.Name = "m_gbStartup"
            Me.m_gbStartup.TabStop = False
            '
            'm_lblResetOverwritePrompts
            '
            resources.ApplyResources(Me.m_lblResetOverwritePrompts, "m_lblResetOverwritePrompts")
            Me.m_lblResetOverwritePrompts.Name = "m_lblResetOverwritePrompts"
            '
            'm_btnResetOverwritePrompts
            '
            resources.ApplyResources(Me.m_btnResetOverwritePrompts, "m_btnResetOverwritePrompts")
            Me.m_btnResetOverwritePrompts.Name = "m_btnResetOverwritePrompts"
            Me.m_btnResetOverwritePrompts.UseVisualStyleBackColor = True
            '
            'm_cbCheckEwE6
            '
            resources.ApplyResources(Me.m_cbCheckEwE6, "m_cbCheckEwE6")
            Me.m_cbCheckEwE6.Name = "m_cbCheckEwE6"
            Me.m_cbCheckEwE6.UseVisualStyleBackColor = True
            '
            'm_cbDownloadUpdates
            '
            resources.ApplyResources(Me.m_cbDownloadUpdates, "m_cbDownloadUpdates")
            Me.m_cbDownloadUpdates.Name = "m_cbDownloadUpdates"
            Me.m_cbDownloadUpdates.UseVisualStyleBackColor = True
            '
            'm_hdrCaption
            '
            resources.ApplyResources(Me.m_hdrCaption, "m_hdrCaption")
            Me.m_hdrCaption.Name = "m_hdrCaption"
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
            Me.ResumeLayout(False)

        End Sub
        Private WithEvents m_lblMRU As System.Windows.Forms.Label
        Private WithEvents m_nudMaxNumMessages As System.Windows.Forms.NumericUpDown
        Private WithEvents m_lblMaxNumMessages As System.Windows.Forms.Label
        Private WithEvents m_btnClearMRU As System.Windows.Forms.Button
        Private WithEvents m_gpMRU As System.Windows.Forms.GroupBox
        Private WithEvents m_gpMsg As System.Windows.Forms.GroupBox
        Private WithEvents m_nudMRU As System.Windows.Forms.NumericUpDown
        Private WithEvents m_gbStartup As System.Windows.Forms.GroupBox
        Private WithEvents m_cbCheckEwE6 As System.Windows.Forms.CheckBox
        Private WithEvents m_cbDownloadUpdates As System.Windows.Forms.CheckBox
        Private WithEvents m_lblResetOverwritePrompts As System.Windows.Forms.Label
        Private WithEvents m_btnResetOverwritePrompts As System.Windows.Forms.Button
        Private WithEvents m_hdrCaption As cEwEHeaderLabel
        Private WithEvents m_cbShowTime As System.Windows.Forms.CheckBox
        Private WithEvents m_lblBackup As System.Windows.Forms.Label
        Private WithEvents m_tbBackupMask As System.Windows.Forms.TextBox
        Private WithEvents m_lblSample As System.Windows.Forms.Label
        Private WithEvents m_lblExample As System.Windows.Forms.Label
        Private WithEvents m_tbOutput As System.Windows.Forms.TextBox
        Private WithEvents m_lblOutput As System.Windows.Forms.Label
        Private WithEvents m_fieldpickBackup As ScientificInterfaceShared.Controls.ucFieldPicker
        Private WithEvents m_fieldpickOutput As ScientificInterfaceShared.Controls.ucFieldPicker

    End Class

End Namespace

