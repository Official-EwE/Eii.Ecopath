<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class dlgSensitivityOfSStoV
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(dlgSensitivityOfSStoV))
        Me.m_ucVulBlocks = New ScientificInterface.Ecosim.ucVulnerabiltyBlocks
        Me.m_pbSearch = New System.Windows.Forms.ProgressBar
        Me.m_btnSearch = New System.Windows.Forms.Button
        Me.m_rbSearchPredPrey = New System.Windows.Forms.RadioButton
        Me.m_rbSearchPred = New System.Windows.Forms.RadioButton
        Me.m_lblNumCategories = New System.Windows.Forms.Label
        Me.m_nudNumBlocks = New System.Windows.Forms.NumericUpDown
        Me.m_btnCancel = New System.Windows.Forms.Button
        Me.m_btnOk = New System.Windows.Forms.Button
        Me.Panel1 = New System.Windows.Forms.Panel
        Me.m_lblSearch = New System.Windows.Forms.Label
        Me.Panel2 = New System.Windows.Forms.Panel
        Me.m_lblTransfer = New System.Windows.Forms.Label
        CType(Me.m_nudNumBlocks, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel1.SuspendLayout()
        Me.Panel2.SuspendLayout()
        Me.SuspendLayout()
        '
        'm_ucVulBlocks
        '
        resources.ApplyResources(Me.m_ucVulBlocks, "m_ucVulBlocks")
        Me.m_ucVulBlocks.BlockColors = Nothing
        Me.m_ucVulBlocks.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.m_ucVulBlocks.Name = "m_ucVulBlocks"
        Me.m_ucVulBlocks.SelectedBlockNum = 0
        Me.m_ucVulBlocks.UIContext = Nothing
        '
        'm_pbSearch
        '
        resources.ApplyResources(Me.m_pbSearch, "m_pbSearch")
        Me.m_pbSearch.Name = "m_pbSearch"
        Me.m_pbSearch.Style = System.Windows.Forms.ProgressBarStyle.Continuous
        '
        'm_btnSearch
        '
        resources.ApplyResources(Me.m_btnSearch, "m_btnSearch")
        Me.m_btnSearch.Name = "m_btnSearch"
        Me.m_btnSearch.UseVisualStyleBackColor = True
        '
        'm_rbSearchPredPrey
        '
        resources.ApplyResources(Me.m_rbSearchPredPrey, "m_rbSearchPredPrey")
        Me.m_rbSearchPredPrey.Name = "m_rbSearchPredPrey"
        Me.m_rbSearchPredPrey.UseVisualStyleBackColor = True
        '
        'm_rbSearchPred
        '
        resources.ApplyResources(Me.m_rbSearchPred, "m_rbSearchPred")
        Me.m_rbSearchPred.Checked = True
        Me.m_rbSearchPred.Name = "m_rbSearchPred"
        Me.m_rbSearchPred.TabStop = True
        Me.m_rbSearchPred.UseVisualStyleBackColor = True
        '
        'm_lblNumCategories
        '
        resources.ApplyResources(Me.m_lblNumCategories, "m_lblNumCategories")
        Me.m_lblNumCategories.Name = "m_lblNumCategories"
        '
        'm_nudNumBlocks
        '
        resources.ApplyResources(Me.m_nudNumBlocks, "m_nudNumBlocks")
        Me.m_nudNumBlocks.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.m_nudNumBlocks.Name = "m_nudNumBlocks"
        Me.m_nudNumBlocks.Value = New Decimal(New Integer() {1, 0, 0, 0})
        '
        'm_btnCancel
        '
        resources.ApplyResources(Me.m_btnCancel, "m_btnCancel")
        Me.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.m_btnCancel.Name = "m_btnCancel"
        Me.m_btnCancel.UseVisualStyleBackColor = True
        '
        'm_btnOk
        '
        resources.ApplyResources(Me.m_btnOk, "m_btnOk")
        Me.m_btnOk.Name = "m_btnOk"
        Me.m_btnOk.UseVisualStyleBackColor = True
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.m_lblSearch)
        Me.Panel1.Controls.Add(Me.m_rbSearchPred)
        Me.Panel1.Controls.Add(Me.m_rbSearchPredPrey)
        Me.Panel1.Controls.Add(Me.m_btnSearch)
        resources.ApplyResources(Me.Panel1, "Panel1")
        Me.Panel1.Name = "Panel1"
        '
        'm_lblSearch
        '
        resources.ApplyResources(Me.m_lblSearch, "m_lblSearch")
        Me.m_lblSearch.BackColor = System.Drawing.SystemColors.ControlDark
        Me.m_lblSearch.ForeColor = System.Drawing.SystemColors.ControlLightLight
        Me.m_lblSearch.Name = "m_lblSearch"
        '
        'Panel2
        '
        Me.Panel2.Controls.Add(Me.m_nudNumBlocks)
        Me.Panel2.Controls.Add(Me.m_lblNumCategories)
        Me.Panel2.Controls.Add(Me.m_lblTransfer)
        resources.ApplyResources(Me.Panel2, "Panel2")
        Me.Panel2.Name = "Panel2"
        '
        'm_lblTransfer
        '
        resources.ApplyResources(Me.m_lblTransfer, "m_lblTransfer")
        Me.m_lblTransfer.BackColor = System.Drawing.SystemColors.ControlDark
        Me.m_lblTransfer.ForeColor = System.Drawing.SystemColors.ControlLightLight
        Me.m_lblTransfer.Name = "m_lblTransfer"
        '
        'dlgSensitivityOfSStoV
        '
        Me.AcceptButton = Me.m_btnSearch
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.m_btnCancel
        Me.Controls.Add(Me.m_pbSearch)
        Me.Controls.Add(Me.m_ucVulBlocks)
        Me.Controls.Add(Me.Panel2)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.m_btnCancel)
        Me.Controls.Add(Me.m_btnOk)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "dlgSensitivityOfSStoV"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show
        CType(Me.m_nudNumBlocks, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        Me.Panel2.ResumeLayout(False)
        Me.Panel2.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents m_rbSearchPredPrey As System.Windows.Forms.RadioButton
    Friend WithEvents m_rbSearchPred As System.Windows.Forms.RadioButton
    Friend WithEvents m_btnCancel As System.Windows.Forms.Button
    Friend WithEvents m_btnOk As System.Windows.Forms.Button
    Friend WithEvents m_nudNumBlocks As System.Windows.Forms.NumericUpDown
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Private WithEvents m_lblSearch As System.Windows.Forms.Label
    Friend WithEvents Panel2 As System.Windows.Forms.Panel
    Private WithEvents m_lblTransfer As System.Windows.Forms.Label
    Private WithEvents m_ucVulBlocks As ScientificInterface.Ecosim.ucVulnerabiltyBlocks
    Private WithEvents m_btnSearch As System.Windows.Forms.Button
    Private WithEvents m_pbSearch As System.Windows.Forms.ProgressBar
    Private WithEvents m_lblNumCategories As System.Windows.Forms.Label
End Class
