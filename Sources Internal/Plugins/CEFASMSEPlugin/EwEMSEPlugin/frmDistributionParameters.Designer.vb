<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmDistributionParameters
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmDistributionParameters))
        Dim DataGridViewCellStyle7 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle8 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle9 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle10 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle11 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle12 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Me.cboPathOrSim = New System.Windows.Forms.ComboBox()
        Me.cboParamName = New System.Windows.Forms.ComboBox()
        Me.dgvParameters = New System.Windows.Forms.DataGridView()
        Me.GroupNumber = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.GroupName = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Mean = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.CV = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Lower = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Upper = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.m_btnCancel = New System.Windows.Forms.Button()
        Me.m_btnOK = New System.Windows.Forms.Button()
        CType(Me.dgvParameters, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'cboPathOrSim
        '
        Me.cboPathOrSim.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboPathOrSim.FormattingEnabled = True
        Me.cboPathOrSim.Items.AddRange(New Object() {resources.GetString("cboPathOrSim.Items"), resources.GetString("cboPathOrSim.Items1")})
        resources.ApplyResources(Me.cboPathOrSim, "cboPathOrSim")
        Me.cboPathOrSim.Name = "cboPathOrSim"
        '
        'cboParamName
        '
        Me.cboParamName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboParamName.FormattingEnabled = True
        resources.ApplyResources(Me.cboParamName, "cboParamName")
        Me.cboParamName.Name = "cboParamName"
        '
        'dgvParameters
        '
        Me.dgvParameters.AllowUserToAddRows = False
        Me.dgvParameters.AllowUserToDeleteRows = False
        Me.dgvParameters.AllowUserToResizeRows = False
        resources.ApplyResources(Me.dgvParameters, "dgvParameters")
        Me.dgvParameters.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvParameters.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.GroupNumber, Me.GroupName, Me.Mean, Me.CV, Me.Lower, Me.Upper})
        Me.dgvParameters.MultiSelect = False
        Me.dgvParameters.Name = "dgvParameters"
        Me.dgvParameters.RowHeadersVisible = False
        Me.dgvParameters.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgvParameters.ShowRowErrors = False
        '
        'GroupNumber
        '
        DataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        Me.GroupNumber.DefaultCellStyle = DataGridViewCellStyle7
        resources.ApplyResources(Me.GroupNumber, "GroupNumber")
        Me.GroupNumber.Name = "GroupNumber"
        '
        'GroupName
        '
        DataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        Me.GroupName.DefaultCellStyle = DataGridViewCellStyle8
        resources.ApplyResources(Me.GroupName, "GroupName")
        Me.GroupName.Name = "GroupName"
        '
        'Mean
        '
        DataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        Me.Mean.DefaultCellStyle = DataGridViewCellStyle9
        resources.ApplyResources(Me.Mean, "Mean")
        Me.Mean.Name = "Mean"
        '
        'CV
        '
        DataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        Me.CV.DefaultCellStyle = DataGridViewCellStyle10
        resources.ApplyResources(Me.CV, "CV")
        Me.CV.Name = "CV"
        '
        'Lower
        '
        DataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        Me.Lower.DefaultCellStyle = DataGridViewCellStyle11
        resources.ApplyResources(Me.Lower, "Lower")
        Me.Lower.Name = "Lower"
        '
        'Upper
        '
        DataGridViewCellStyle12.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        Me.Upper.DefaultCellStyle = DataGridViewCellStyle12
        resources.ApplyResources(Me.Upper, "Upper")
        Me.Upper.Name = "Upper"
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
        'frmDistributionParameters
        '
        Me.AcceptButton = Me.m_btnOK
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.m_btnCancel
        Me.Controls.Add(Me.m_btnOK)
        Me.Controls.Add(Me.m_btnCancel)
        Me.Controls.Add(Me.dgvParameters)
        Me.Controls.Add(Me.cboParamName)
        Me.Controls.Add(Me.cboPathOrSim)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmDistributionParameters"
        CType(Me.dgvParameters, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents cboPathOrSim As System.Windows.Forms.ComboBox
    Friend WithEvents cboParamName As System.Windows.Forms.ComboBox
    Friend WithEvents dgvParameters As System.Windows.Forms.DataGridView
    Friend WithEvents GroupNumber As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents GroupName As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Mean As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents CV As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Lower As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Upper As System.Windows.Forms.DataGridViewTextBoxColumn
    Private WithEvents m_btnCancel As System.Windows.Forms.Button
    Private WithEvents m_btnOK As System.Windows.Forms.Button
End Class
