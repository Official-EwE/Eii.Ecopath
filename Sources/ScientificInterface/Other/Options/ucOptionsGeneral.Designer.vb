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
            Me.m_lblSample = New System.Windows.Forms.Label
            Me.m_tsBogus = New System.Windows.Forms.ToolStrip
            Me.m_tsddFields = New System.Windows.Forms.ToolStripSplitButton
            Me.m_tbBackupMask = New System.Windows.Forms.TextBox
            Me.m_lblBackup = New System.Windows.Forms.Label
            Me.m_nudMRU = New System.Windows.Forms.NumericUpDown
            Me.m_btnClearMRU = New System.Windows.Forms.Button
            Me.m_lblMRU = New System.Windows.Forms.Label
            Me.m_hdrCaption = New ScientificInterfaceShared.Controls.cEwEHeaderLabel
            Me.m_gpMsg = New System.Windows.Forms.GroupBox
            Me.m_cbShowTime = New System.Windows.Forms.CheckBox
            Me.m_nudMaxNumMessages = New System.Windows.Forms.NumericUpDown
            Me.m_lblMaxNumMessages = New System.Windows.Forms.Label
            Me.m_gbStartup = New System.Windows.Forms.GroupBox
            Me.m_lblResetOverwritePrompts = New System.Windows.Forms.Label
            Me.m_btnResetOverwritePrompts = New System.Windows.Forms.Button
            Me.m_cbCheckEwE6 = New System.Windows.Forms.CheckBox
            Me.m_cbDownloadUpdates = New System.Windows.Forms.CheckBox
            Me.Label1 = New System.Windows.Forms.Label
            Me.m_gpMRU.SuspendLayout()
            Me.m_tsBogus.SuspendLayout()
            CType(Me.m_nudMRU, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.m_gpMsg.SuspendLayout()
            CType(Me.m_nudMaxNumMessages, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.m_gbStartup.SuspendLayout()
            Me.SuspendLayout()
            '
            'm_gpMRU
            '
            resources.ApplyResources(Me.m_gpMRU, "m_gpMRU")
            Me.m_gpMRU.Controls.Add(Me.m_lblSample)
            Me.m_gpMRU.Controls.Add(Me.m_tsBogus)
            Me.m_gpMRU.Controls.Add(Me.m_tbBackupMask)
            Me.m_gpMRU.Controls.Add(Me.Label1)
            Me.m_gpMRU.Controls.Add(Me.m_lblBackup)
            Me.m_gpMRU.Controls.Add(Me.m_nudMRU)
            Me.m_gpMRU.Controls.Add(Me.m_btnClearMRU)
            Me.m_gpMRU.Controls.Add(Me.m_lblMRU)
            Me.m_gpMRU.Name = "m_gpMRU"
            Me.m_gpMRU.TabStop = False
            '
            'm_lblSample
            '
            resources.ApplyResources(Me.m_lblSample, "m_lblSample")
            Me.m_lblSample.Name = "m_lblSample"
            '
            'm_tsBogus
            '
            resources.ApplyResources(Me.m_tsBogus, "m_tsBogus")
            Me.m_tsBogus.BackColor = System.Drawing.Color.Transparent
            Me.m_tsBogus.CanOverflow = False
            Me.m_tsBogus.GripMargin = New System.Windows.Forms.Padding(0)
            Me.m_tsBogus.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
            Me.m_tsBogus.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tsddFields})
            Me.m_tsBogus.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Table
            Me.m_tsBogus.Name = "m_tsBogus"
            Me.m_tsBogus.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional
            '
            'm_tsddFields
            '
            Me.m_tsddFields.BackColor = System.Drawing.Color.Transparent
            Me.m_tsddFields.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            resources.ApplyResources(Me.m_tsddFields, "m_tsddFields")
            Me.m_tsddFields.Name = "m_tsddFields"
            '
            'm_tbBackupMask
            '
            resources.ApplyResources(Me.m_tbBackupMask, "m_tbBackupMask")
            Me.m_tbBackupMask.Name = "m_tbBackupMask"
            '
            'm_lblBackup
            '
            resources.ApplyResources(Me.m_lblBackup, "m_lblBackup")
            Me.m_lblBackup.Name = "m_lblBackup"
            '
            'm_nudMRU
            '
            resources.ApplyResources(Me.m_nudMRU, "m_nudMRU")
            Me.m_nudMRU.Maximum = New Decimal(New Integer() {24, 0, 0, 0})
            Me.m_nudMRU.Name = "m_nudMRU"
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
            'm_hdrCaption
            '
            resources.ApplyResources(Me.m_hdrCaption, "m_hdrCaption")
            Me.m_hdrCaption.Name = "m_hdrCaption"
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
            'Label1
            '
            resources.ApplyResources(Me.Label1, "Label1")
            Me.Label1.Name = "Label1"
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
            Me.m_tsBogus.ResumeLayout(False)
            Me.m_tsBogus.PerformLayout()
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
        Private WithEvents m_tsBogus As System.Windows.Forms.ToolStrip
        Private WithEvents m_tsddFields As System.Windows.Forms.ToolStripSplitButton
        Private WithEvents m_tbBackupMask As System.Windows.Forms.TextBox
        Private WithEvents m_lblSample As System.Windows.Forms.Label
        Private WithEvents Label1 As System.Windows.Forms.Label

    End Class

End Namespace

