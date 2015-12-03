Imports ScientificInterfaceShared.Controls

Partial Class frmImportShapes
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmImportShapes))
        Me.m_hdrSource = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.m_tbImportSeparator = New ScientificInterfaceShared.Controls.ucCharacterTextBox()
        Me.m_tbImportDelimiter = New ScientificInterfaceShared.Controls.ucCharacterTextBox()
        Me.m_tbImportFileName = New System.Windows.Forms.TextBox()
        Me.m_lblImportDecimalSeparator = New System.Windows.Forms.Label()
        Me.m_lblImportDelimiter = New System.Windows.Forms.Label()
        Me.m_rbImportSourceClipboard = New System.Windows.Forms.RadioButton()
        Me.m_btnImportBrowse = New System.Windows.Forms.Button()
        Me.m_rbImportSourceTextFile = New System.Windows.Forms.RadioButton()
        Me.m_dgvImportPreview = New System.Windows.Forms.DataGridView()
        Me.m_hdrTarget = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.m_hdrPreview = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.m_btnOk = New System.Windows.Forms.Button()
        Me.m_btnCancel = New System.Windows.Forms.Button()
        Me.m_cmbTarget = New System.Windows.Forms.ComboBox()
        Me.m_lblImportAs = New System.Windows.Forms.Label()
        CType(Me.m_dgvImportPreview, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'm_hdrSource
        '
        resources.ApplyResources(Me.m_hdrSource, "m_hdrSource")
        Me.m_hdrSource.CanCollapseParent = False
        Me.m_hdrSource.CollapsedParentHeight = 0
        Me.m_hdrSource.IsCollapsed = False
        Me.m_hdrSource.Name = "m_hdrSource"
        '
        'm_tbImportSeparator
        '
        Me.m_tbImportSeparator.AcceptsReturn = True
        Me.m_tbImportSeparator.AcceptsTab = True
        resources.ApplyResources(Me.m_tbImportSeparator, "m_tbImportSeparator")
        Me.m_tbImportSeparator.Character = Global.Microsoft.VisualBasic.ChrW(46)
        Me.m_tbImportSeparator.CharacterMask = ""
        Me.m_tbImportSeparator.CharCode = 46
        Me.m_tbImportSeparator.MaskInclusive = False
        Me.m_tbImportSeparator.Name = "m_tbImportSeparator"
        Me.m_tbImportSeparator.ShortcutsEnabled = False
        '
        'm_tbImportDelimiter
        '
        Me.m_tbImportDelimiter.AcceptsReturn = True
        Me.m_tbImportDelimiter.AcceptsTab = True
        Me.m_tbImportDelimiter.Character = Global.Microsoft.VisualBasic.ChrW(44)
        Me.m_tbImportDelimiter.CharacterMask = ""
        Me.m_tbImportDelimiter.CharCode = 44
        resources.ApplyResources(Me.m_tbImportDelimiter, "m_tbImportDelimiter")
        Me.m_tbImportDelimiter.MaskInclusive = False
        Me.m_tbImportDelimiter.Name = "m_tbImportDelimiter"
        Me.m_tbImportDelimiter.ShortcutsEnabled = False
        '
        'm_tbImportFileName
        '
        resources.ApplyResources(Me.m_tbImportFileName, "m_tbImportFileName")
        Me.m_tbImportFileName.Name = "m_tbImportFileName"
        '
        'm_lblImportDecimalSeparator
        '
        resources.ApplyResources(Me.m_lblImportDecimalSeparator, "m_lblImportDecimalSeparator")
        Me.m_lblImportDecimalSeparator.Name = "m_lblImportDecimalSeparator"
        '
        'm_lblImportDelimiter
        '
        resources.ApplyResources(Me.m_lblImportDelimiter, "m_lblImportDelimiter")
        Me.m_lblImportDelimiter.Name = "m_lblImportDelimiter"
        '
        'm_rbImportSourceClipboard
        '
        resources.ApplyResources(Me.m_rbImportSourceClipboard, "m_rbImportSourceClipboard")
        Me.m_rbImportSourceClipboard.Name = "m_rbImportSourceClipboard"
        Me.m_rbImportSourceClipboard.UseVisualStyleBackColor = True
        '
        'm_btnImportBrowse
        '
        resources.ApplyResources(Me.m_btnImportBrowse, "m_btnImportBrowse")
        Me.m_btnImportBrowse.Name = "m_btnImportBrowse"
        Me.m_btnImportBrowse.UseVisualStyleBackColor = True
        '
        'm_rbImportSourceTextFile
        '
        resources.ApplyResources(Me.m_rbImportSourceTextFile, "m_rbImportSourceTextFile")
        Me.m_rbImportSourceTextFile.Checked = True
        Me.m_rbImportSourceTextFile.Name = "m_rbImportSourceTextFile"
        Me.m_rbImportSourceTextFile.TabStop = True
        Me.m_rbImportSourceTextFile.UseVisualStyleBackColor = True
        '
        'm_dgvImportPreview
        '
        Me.m_dgvImportPreview.AllowUserToAddRows = False
        Me.m_dgvImportPreview.AllowUserToDeleteRows = False
        resources.ApplyResources(Me.m_dgvImportPreview, "m_dgvImportPreview")
        Me.m_dgvImportPreview.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells
        Me.m_dgvImportPreview.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.m_dgvImportPreview.ColumnHeadersVisible = False
        Me.m_dgvImportPreview.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically
        Me.m_dgvImportPreview.Name = "m_dgvImportPreview"
        Me.m_dgvImportPreview.ReadOnly = True
        Me.m_dgvImportPreview.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders
        '
        'm_hdrTarget
        '
        resources.ApplyResources(Me.m_hdrTarget, "m_hdrTarget")
        Me.m_hdrTarget.CanCollapseParent = False
        Me.m_hdrTarget.CollapsedParentHeight = 0
        Me.m_hdrTarget.IsCollapsed = False
        Me.m_hdrTarget.Name = "m_hdrTarget"
        '
        'm_hdrPreview
        '
        resources.ApplyResources(Me.m_hdrPreview, "m_hdrPreview")
        Me.m_hdrPreview.CanCollapseParent = False
        Me.m_hdrPreview.CollapsedParentHeight = 0
        Me.m_hdrPreview.IsCollapsed = False
        Me.m_hdrPreview.Name = "m_hdrPreview"
        '
        'm_btnOk
        '
        resources.ApplyResources(Me.m_btnOk, "m_btnOk")
        Me.m_btnOk.Name = "m_btnOk"
        '
        'm_btnCancel
        '
        resources.ApplyResources(Me.m_btnCancel, "m_btnCancel")
        Me.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.m_btnCancel.Name = "m_btnCancel"
        '
        'm_cmbTarget
        '
        resources.ApplyResources(Me.m_cmbTarget, "m_cmbTarget")
        Me.m_cmbTarget.FormattingEnabled = True
        Me.m_cmbTarget.Name = "m_cmbTarget"
        '
        'm_lblImportAs
        '
        resources.ApplyResources(Me.m_lblImportAs, "m_lblImportAs")
        Me.m_lblImportAs.Name = "m_lblImportAs"
        '
        'frmImportShapes
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.m_btnCancel
        Me.ControlBox = False
        Me.Controls.Add(Me.m_lblImportAs)
        Me.Controls.Add(Me.m_cmbTarget)
        Me.Controls.Add(Me.m_btnOk)
        Me.Controls.Add(Me.m_btnCancel)
        Me.Controls.Add(Me.m_dgvImportPreview)
        Me.Controls.Add(Me.m_hdrTarget)
        Me.Controls.Add(Me.m_hdrPreview)
        Me.Controls.Add(Me.m_hdrSource)
        Me.Controls.Add(Me.m_tbImportSeparator)
        Me.Controls.Add(Me.m_tbImportDelimiter)
        Me.Controls.Add(Me.m_tbImportFileName)
        Me.Controls.Add(Me.m_lblImportDecimalSeparator)
        Me.Controls.Add(Me.m_lblImportDelimiter)
        Me.Controls.Add(Me.m_rbImportSourceClipboard)
        Me.Controls.Add(Me.m_btnImportBrowse)
        Me.Controls.Add(Me.m_rbImportSourceTextFile)
        Me.Name = "frmImportShapes"
        Me.ShowInTaskbar = False
        CType(Me.m_dgvImportPreview, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Private WithEvents m_hdrSource As ScientificInterfaceShared.Controls.cEwEHeaderLabel
    Private WithEvents m_tbImportSeparator As ScientificInterfaceShared.Controls.ucCharacterTextBox
    Private WithEvents m_tbImportDelimiter As ScientificInterfaceShared.Controls.ucCharacterTextBox
    Private WithEvents m_tbImportFileName As System.Windows.Forms.TextBox
    Private WithEvents m_lblImportDecimalSeparator As System.Windows.Forms.Label
    Private WithEvents m_lblImportDelimiter As System.Windows.Forms.Label
    Private WithEvents m_rbImportSourceClipboard As System.Windows.Forms.RadioButton
    Private WithEvents m_btnImportBrowse As System.Windows.Forms.Button
    Private WithEvents m_rbImportSourceTextFile As System.Windows.Forms.RadioButton
    Private WithEvents m_dgvImportPreview As System.Windows.Forms.DataGridView
    Private WithEvents m_hdrTarget As ScientificInterfaceShared.Controls.cEwEHeaderLabel
    Private WithEvents m_hdrPreview As ScientificInterfaceShared.Controls.cEwEHeaderLabel
    Private WithEvents m_btnOk As System.Windows.Forms.Button
    Private WithEvents m_btnCancel As System.Windows.Forms.Button
    Private WithEvents m_cmbTarget As System.Windows.Forms.ComboBox
    Private WithEvents m_lblImportAs As System.Windows.Forms.Label
End Class
