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
            Me.m_gpMsg = New System.Windows.Forms.GroupBox
            Me.m_nudMaxNumMessages = New System.Windows.Forms.NumericUpDown
            Me.m_lblMaxNumMessages = New System.Windows.Forms.Label
            Me.m_nudMRU = New System.Windows.Forms.NumericUpDown
            Me.m_gpMRU.SuspendLayout()
            Me.m_gpMsg.SuspendLayout()
            CType(Me.m_nudMaxNumMessages, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.m_nudMRU, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            'm_gpMRU
            '
            resources.ApplyResources(Me.m_gpMRU, "m_gpMRU")
            Me.m_gpMRU.Controls.Add(Me.m_nudMRU)
            Me.m_gpMRU.Controls.Add(Me.m_btnClear)
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
            'm_nudMRU
            '
            resources.ApplyResources(Me.m_nudMRU, "m_nudMRU")
            Me.m_nudMRU.Maximum = New Decimal(New Integer() {24, 0, 0, 0})
            Me.m_nudMRU.Name = "m_nudMRU"
            '
            'ucAppGeneral
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.m_gpMsg)
            Me.Controls.Add(Me.lblTitle)
            Me.Controls.Add(Me.m_gpMRU)
            Me.Name = "ucAppGeneral"
            Me.m_gpMRU.ResumeLayout(False)
            Me.m_gpMRU.PerformLayout()
            Me.m_gpMsg.ResumeLayout(False)
            Me.m_gpMsg.PerformLayout()
            CType(Me.m_nudMaxNumMessages, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.m_nudMRU, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)

        End Sub
        Friend WithEvents lblMDB As System.Windows.Forms.Label
        Friend WithEvents lblTitle As System.Windows.Forms.Label
        Private WithEvents m_nudMaxNumMessages As System.Windows.Forms.NumericUpDown
        Private WithEvents m_lblMaxNumMessages As System.Windows.Forms.Label
        Private WithEvents m_btnClear As System.Windows.Forms.Button
        Private WithEvents m_gpMRU As System.Windows.Forms.GroupBox
        Private WithEvents m_gpMsg As System.Windows.Forms.GroupBox
        Private WithEvents m_nudMRU As System.Windows.Forms.NumericUpDown

    End Class

End Namespace

