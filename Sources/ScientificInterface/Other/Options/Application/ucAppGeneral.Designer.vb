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
            Me.GroupBox1 = New System.Windows.Forms.GroupBox
            Me.btnClear = New System.Windows.Forms.Button
            Me.txbMdbNum = New System.Windows.Forms.TextBox
            Me.lblMDB = New System.Windows.Forms.Label
            Me.lblTitle = New System.Windows.Forms.Label
            Me.cbSaveLayout = New System.Windows.Forms.CheckBox
            Me.Label1 = New System.Windows.Forms.Label
            Me.txbSaveDirectory = New System.Windows.Forms.TextBox
            Me.btnSaveLocation = New System.Windows.Forms.Button
            Me.GroupBox2 = New System.Windows.Forms.GroupBox
            Me.btnRemoveAll = New System.Windows.Forms.Button
            Me.gpbMsg = New System.Windows.Forms.GroupBox
            Me.m_nudMaxNumMessages = New System.Windows.Forms.NumericUpDown
            Me.m_lblMaxNumMessages = New System.Windows.Forms.Label
            Me.cbInformation = New System.Windows.Forms.ComboBox
            Me.cbWarning = New System.Windows.Forms.ComboBox
            Me.cbCritical = New System.Windows.Forms.ComboBox
            Me.lbInformation = New System.Windows.Forms.Label
            Me.lbWarning = New System.Windows.Forms.Label
            Me.lbCritical = New System.Windows.Forms.Label
            Me.GroupBox1.SuspendLayout()
            Me.GroupBox2.SuspendLayout()
            Me.gpbMsg.SuspendLayout()
            CType(Me.m_nudMaxNumMessages, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            'GroupBox1
            '
            resources.ApplyResources(Me.GroupBox1, "GroupBox1")
            Me.GroupBox1.Controls.Add(Me.btnClear)
            Me.GroupBox1.Controls.Add(Me.txbMdbNum)
            Me.GroupBox1.Controls.Add(Me.lblMDB)
            Me.GroupBox1.Name = "GroupBox1"
            Me.GroupBox1.TabStop = False
            '
            'btnClear
            '
            resources.ApplyResources(Me.btnClear, "btnClear")
            Me.btnClear.Name = "btnClear"
            Me.btnClear.UseVisualStyleBackColor = True
            '
            'txbMdbNum
            '
            Me.txbMdbNum.DataBindings.Add(New System.Windows.Forms.Binding("Text", Global.ScientificInterface.Settings.Default, "MdbRecentlyUsedCount", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
            resources.ApplyResources(Me.txbMdbNum, "txbMdbNum")
            Me.txbMdbNum.Name = "txbMdbNum"
            Me.txbMdbNum.Text = Global.ScientificInterface.Settings.Default.MdbRecentlyUsedCount
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
            'cbSaveLayout
            '
            resources.ApplyResources(Me.cbSaveLayout, "cbSaveLayout")
            Me.cbSaveLayout.Checked = True
            Me.cbSaveLayout.CheckState = System.Windows.Forms.CheckState.Checked
            Me.cbSaveLayout.Name = "cbSaveLayout"
            Me.cbSaveLayout.UseVisualStyleBackColor = True
            '
            'Label1
            '
            resources.ApplyResources(Me.Label1, "Label1")
            Me.Label1.Name = "Label1"
            '
            'txbSaveDirectory
            '
            resources.ApplyResources(Me.txbSaveDirectory, "txbSaveDirectory")
            Me.txbSaveDirectory.Name = "txbSaveDirectory"
            '
            'btnSaveLocation
            '
            resources.ApplyResources(Me.btnSaveLocation, "btnSaveLocation")
            Me.btnSaveLocation.Name = "btnSaveLocation"
            Me.btnSaveLocation.UseVisualStyleBackColor = True
            '
            'GroupBox2
            '
            resources.ApplyResources(Me.GroupBox2, "GroupBox2")
            Me.GroupBox2.Controls.Add(Me.btnRemoveAll)
            Me.GroupBox2.Controls.Add(Me.cbSaveLayout)
            Me.GroupBox2.Controls.Add(Me.btnSaveLocation)
            Me.GroupBox2.Controls.Add(Me.Label1)
            Me.GroupBox2.Controls.Add(Me.txbSaveDirectory)
            Me.GroupBox2.Name = "GroupBox2"
            Me.GroupBox2.TabStop = False
            '
            'btnRemoveAll
            '
            resources.ApplyResources(Me.btnRemoveAll, "btnRemoveAll")
            Me.btnRemoveAll.Name = "btnRemoveAll"
            Me.btnRemoveAll.UseVisualStyleBackColor = True
            '
            'gpbMsg
            '
            resources.ApplyResources(Me.gpbMsg, "gpbMsg")
            Me.gpbMsg.Controls.Add(Me.m_nudMaxNumMessages)
            Me.gpbMsg.Controls.Add(Me.m_lblMaxNumMessages)
            Me.gpbMsg.Controls.Add(Me.cbInformation)
            Me.gpbMsg.Controls.Add(Me.cbWarning)
            Me.gpbMsg.Controls.Add(Me.cbCritical)
            Me.gpbMsg.Controls.Add(Me.lbInformation)
            Me.gpbMsg.Controls.Add(Me.lbWarning)
            Me.gpbMsg.Controls.Add(Me.lbCritical)
            Me.gpbMsg.Name = "gpbMsg"
            Me.gpbMsg.TabStop = False
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
            'cbInformation
            '
            resources.ApplyResources(Me.cbInformation, "cbInformation")
            Me.cbInformation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.cbInformation.FormattingEnabled = True
            Me.cbInformation.Name = "cbInformation"
            '
            'cbWarning
            '
            resources.ApplyResources(Me.cbWarning, "cbWarning")
            Me.cbWarning.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.cbWarning.FormattingEnabled = True
            Me.cbWarning.Name = "cbWarning"
            '
            'cbCritical
            '
            resources.ApplyResources(Me.cbCritical, "cbCritical")
            Me.cbCritical.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.cbCritical.FormattingEnabled = True
            Me.cbCritical.Name = "cbCritical"
            '
            'lbInformation
            '
            resources.ApplyResources(Me.lbInformation, "lbInformation")
            Me.lbInformation.Name = "lbInformation"
            '
            'lbWarning
            '
            resources.ApplyResources(Me.lbWarning, "lbWarning")
            Me.lbWarning.Name = "lbWarning"
            '
            'lbCritical
            '
            resources.ApplyResources(Me.lbCritical, "lbCritical")
            Me.lbCritical.Name = "lbCritical"
            '
            'ucAppGeneral
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.gpbMsg)
            Me.Controls.Add(Me.GroupBox2)
            Me.Controls.Add(Me.lblTitle)
            Me.Controls.Add(Me.GroupBox1)
            Me.Name = "ucAppGeneral"
            Me.GroupBox1.ResumeLayout(False)
            Me.GroupBox1.PerformLayout()
            Me.GroupBox2.ResumeLayout(False)
            Me.GroupBox2.PerformLayout()
            Me.gpbMsg.ResumeLayout(False)
            Me.gpbMsg.PerformLayout()
            CType(Me.m_nudMaxNumMessages, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)

        End Sub
        Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
        Friend WithEvents txbMdbNum As System.Windows.Forms.TextBox
        Friend WithEvents lblMDB As System.Windows.Forms.Label
        Friend WithEvents btnClear As System.Windows.Forms.Button
        Friend WithEvents lblTitle As System.Windows.Forms.Label
        Friend WithEvents txbSaveDirectory As System.Windows.Forms.TextBox
        Friend WithEvents Label1 As System.Windows.Forms.Label
        Friend WithEvents cbSaveLayout As System.Windows.Forms.CheckBox
        Friend WithEvents btnSaveLocation As System.Windows.Forms.Button
        Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
        Friend WithEvents btnRemoveAll As System.Windows.Forms.Button
        Friend WithEvents gpbMsg As System.Windows.Forms.GroupBox
        Friend WithEvents cbInformation As System.Windows.Forms.ComboBox
        Friend WithEvents cbWarning As System.Windows.Forms.ComboBox
        Friend WithEvents cbCritical As System.Windows.Forms.ComboBox
        Friend WithEvents lbInformation As System.Windows.Forms.Label
        Friend WithEvents lbWarning As System.Windows.Forms.Label
        Friend WithEvents lbCritical As System.Windows.Forms.Label
        Private WithEvents m_nudMaxNumMessages As System.Windows.Forms.NumericUpDown
        Private WithEvents m_lblMaxNumMessages As System.Windows.Forms.Label

    End Class

End Namespace

