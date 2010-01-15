Namespace Other

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ucAppGeneral
        Inherits System.Windows.Forms.UserControl

        'UserControl overrides dispose to clean up the component list.
        <System.Diagnostics.DebuggerNonUserCode()> _
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
            MyBase.Dispose(disposing)
        End Sub

        'Required by the Windows Form Designer
        Private components As System.ComponentModel.IContainer

        'NOTE: The following procedure is required by the Windows Form Designer
        'It can be modified using the Windows Form Designer.  
        'Do not modify it using the code editor.
        <System.Diagnostics.DebuggerStepThrough()> _
        Private Sub InitializeComponent()
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ucAppGeneral))
            Me.m_gpMRU = New System.Windows.Forms.GroupBox
            Me.m_btnClear = New System.Windows.Forms.Button
            Me.lblMDB = New System.Windows.Forms.Label
            Me.lblTitle = New System.Windows.Forms.Label
            Me.m_cbSaveLayout = New System.Windows.Forms.CheckBox
            Me.m_lblPathPrompt = New System.Windows.Forms.Label
            Me.m_txbSaveDirectory = New System.Windows.Forms.TextBox
            Me.m_btnSaveLocation = New System.Windows.Forms.Button
            Me.m_gbLayout = New System.Windows.Forms.GroupBox
            Me.m_btnRemoveAll = New System.Windows.Forms.Button
            Me.m_gpMsg = New System.Windows.Forms.GroupBox
            Me.m_nudMaxNumMessages = New System.Windows.Forms.NumericUpDown
            Me.m_lblMaxNumMessages = New System.Windows.Forms.Label
            Me.m_txbMdbNum = New System.Windows.Forms.TextBox
            Me.m_gpMRU.SuspendLayout()
            Me.m_gbLayout.SuspendLayout()
            Me.m_gpMsg.SuspendLayout()
            CType(Me.m_nudMaxNumMessages, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            'm_gpMRU
            '
            resources.ApplyResources(Me.m_gpMRU, "m_gpMRU")
            Me.m_gpMRU.Controls.Add(Me.m_btnClear)
            Me.m_gpMRU.Controls.Add(Me.m_txbMdbNum)
            Me.m_gpMRU.Controls.Add(Me.lblMDB)
            Me.m_gpMRU.Name = "m_gpMRU"
            Me.m_gpMRU.TabStop = False
            '
            'm_btnClear
            '
            resources.ApplyResources(Me.m_btnClear, "m_btnClear")
            Me.m_btnClear.Name = "m_btnClear"
            Me.m_btnClear.UseVisualStyleBackColor = True
            '
            'lblMDB
            '
            resources.ApplyResources(Me.lblMDB, "lblMDB")
            Me.lblMDB.Name = "lblMDB"
            '
            'lblTitle
            '
            Me.lblTitle.BackColor = System.Drawing.SystemColors.ButtonShadow
            resources.ApplyResources(Me.lblTitle, "lblTitle")
            Me.lblTitle.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
            Me.lblTitle.Name = "lblTitle"
            '
            'm_cbSaveLayout
            '
            resources.ApplyResources(Me.m_cbSaveLayout, "m_cbSaveLayout")
            Me.m_cbSaveLayout.Checked = True
            Me.m_cbSaveLayout.CheckState = System.Windows.Forms.CheckState.Checked
            Me.m_cbSaveLayout.Name = "m_cbSaveLayout"
            Me.m_cbSaveLayout.UseVisualStyleBackColor = True
            '
            'm_lblPathPrompt
            '
            resources.ApplyResources(Me.m_lblPathPrompt, "m_lblPathPrompt")
            Me.m_lblPathPrompt.Name = "m_lblPathPrompt"
            '
            'm_txbSaveDirectory
            '
            resources.ApplyResources(Me.m_txbSaveDirectory, "m_txbSaveDirectory")
            Me.m_txbSaveDirectory.Name = "m_txbSaveDirectory"
            '
            'm_btnSaveLocation
            '
            resources.ApplyResources(Me.m_btnSaveLocation, "m_btnSaveLocation")
            Me.m_btnSaveLocation.Name = "m_btnSaveLocation"
            Me.m_btnSaveLocation.UseVisualStyleBackColor = True
            '
            'm_gbLayout
            '
            resources.ApplyResources(Me.m_gbLayout, "m_gbLayout")
            Me.m_gbLayout.Controls.Add(Me.m_btnRemoveAll)
            Me.m_gbLayout.Controls.Add(Me.m_cbSaveLayout)
            Me.m_gbLayout.Controls.Add(Me.m_btnSaveLocation)
            Me.m_gbLayout.Controls.Add(Me.m_lblPathPrompt)
            Me.m_gbLayout.Controls.Add(Me.m_txbSaveDirectory)
            Me.m_gbLayout.Name = "m_gbLayout"
            Me.m_gbLayout.TabStop = False
            '
            'm_btnRemoveAll
            '
            resources.ApplyResources(Me.m_btnRemoveAll, "m_btnRemoveAll")
            Me.m_btnRemoveAll.Name = "m_btnRemoveAll"
            Me.m_btnRemoveAll.UseVisualStyleBackColor = True
            '
            'm_gpMsg
            '
            resources.ApplyResources(Me.m_gpMsg, "m_gpMsg")
            Me.m_gpMsg.Controls.Add(Me.m_nudMaxNumMessages)
            Me.m_gpMsg.Controls.Add(Me.m_lblMaxNumMessages)
            Me.m_gpMsg.Name = "m_gpMsg"
            Me.m_gpMsg.TabStop = False
            '
            'm_nudMaxNumMessages
            '
            resources.ApplyResources(Me.m_nudMaxNumMessages, "m_nudMaxNumMessages")
            Me.m_nudMaxNumMessages.Maximum = New Decimal(New Integer() {200, 0, 0, 0})
            Me.m_nudMaxNumMessages.Minimum = New Decimal(New Integer() {10, 0, 0, 0})
            Me.m_nudMaxNumMessages.Name = "m_nudMaxNumMessages"
            Me.m_nudMaxNumMessages.Value = New Decimal(New Integer() {10, 0, 0, 0})
            '
            'm_lblMaxNumMessages
            '
            resources.ApplyResources(Me.m_lblMaxNumMessages, "m_lblMaxNumMessages")
            Me.m_lblMaxNumMessages.Name = "m_lblMaxNumMessages"
            '
            'm_txbMdbNum
            '
            Me.m_txbMdbNum.DataBindings.Add(New System.Windows.Forms.Binding("Text", Global.ScientificInterface.Settings.Default, "MdbRecentlyUsedCount", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
            resources.ApplyResources(Me.m_txbMdbNum, "m_txbMdbNum")
            Me.m_txbMdbNum.Name = "m_txbMdbNum"
            Me.m_txbMdbNum.Text = Global.ScientificInterface.Settings.Default.MdbRecentlyUsedCount
            '
            'ucAppGeneral
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.m_gpMsg)
            Me.Controls.Add(Me.m_gbLayout)
            Me.Controls.Add(Me.lblTitle)
            Me.Controls.Add(Me.m_gpMRU)
            Me.Name = "ucAppGeneral"
            Me.m_gpMRU.ResumeLayout(False)
            Me.m_gpMRU.PerformLayout()
            Me.m_gbLayout.ResumeLayout(False)
            Me.m_gbLayout.PerformLayout()
            Me.m_gpMsg.ResumeLayout(False)
            Me.m_gpMsg.PerformLayout()
            CType(Me.m_nudMaxNumMessages, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)

        End Sub
        Friend WithEvents lblMDB As System.Windows.Forms.Label
        Friend WithEvents lblTitle As System.Windows.Forms.Label
        Private WithEvents m_nudMaxNumMessages As System.Windows.Forms.NumericUpDown
        Private WithEvents m_lblMaxNumMessages As System.Windows.Forms.Label
        Private WithEvents m_btnClear As System.Windows.Forms.Button
        Private WithEvents m_gpMRU As System.Windows.Forms.GroupBox
        Private WithEvents m_txbMdbNum As System.Windows.Forms.TextBox
        Private WithEvents m_gbLayout As System.Windows.Forms.GroupBox
        Private WithEvents m_cbSaveLayout As System.Windows.Forms.CheckBox
        Private WithEvents m_btnRemoveAll As System.Windows.Forms.Button
        Private WithEvents m_lblPathPrompt As System.Windows.Forms.Label
        Private WithEvents m_txbSaveDirectory As System.Windows.Forms.TextBox
        Private WithEvents m_btnSaveLocation As System.Windows.Forms.Button
        Private WithEvents m_gpMsg As System.Windows.Forms.GroupBox

    End Class

End Namespace

