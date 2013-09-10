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
        Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle2 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle3 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle4 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle5 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle6 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Me.cboPathOrSim = New System.Windows.Forms.ComboBox()
        Me.cboParamName = New System.Windows.Forms.ComboBox()
        Me.dgvParameters = New System.Windows.Forms.DataGridView()
        Me.GroupNumber = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.GroupName = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Mean = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.CV = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Lower = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Upper = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.btnClose = New System.Windows.Forms.Button()
        Me.btnSaveAndClose = New System.Windows.Forms.Button()
        CType(Me.dgvParameters, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'cboPathOrSim
        '
        Me.cboPathOrSim.FormattingEnabled = True
        Me.cboPathOrSim.Items.AddRange(New Object() {"Ecopath Parameters", "Ecosim Parameters"})
        Me.cboPathOrSim.Location = New System.Drawing.Point(46, 47)
        Me.cboPathOrSim.Name = "cboPathOrSim"
        Me.cboPathOrSim.Size = New System.Drawing.Size(187, 21)
        Me.cboPathOrSim.TabIndex = 0
        '
        'cboParamName
        '
        Me.cboParamName.FormattingEnabled = True
        Me.cboParamName.Location = New System.Drawing.Point(46, 87)
        Me.cboParamName.Name = "cboParamName"
        Me.cboParamName.Size = New System.Drawing.Size(187, 21)
        Me.cboParamName.TabIndex = 1
        '
        'dgvParameters
        '
        Me.dgvParameters.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvParameters.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.GroupNumber, Me.GroupName, Me.Mean, Me.CV, Me.Lower, Me.Upper})
        Me.dgvParameters.Location = New System.Drawing.Point(46, 138)
        Me.dgvParameters.Name = "dgvParameters"
        Me.dgvParameters.Size = New System.Drawing.Size(672, 349)
        Me.dgvParameters.TabIndex = 2
        '
        'GroupNumber
        '
        DataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        Me.GroupNumber.DefaultCellStyle = DataGridViewCellStyle1
        Me.GroupNumber.HeaderText = "Group Number"
        Me.GroupNumber.Name = "GroupNumber"
        '
        'GroupName
        '
        DataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        Me.GroupName.DefaultCellStyle = DataGridViewCellStyle2
        Me.GroupName.HeaderText = "Group Name"
        Me.GroupName.Name = "GroupName"
        '
        'Mean
        '
        DataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        Me.Mean.DefaultCellStyle = DataGridViewCellStyle3
        Me.Mean.HeaderText = "Mean"
        Me.Mean.Name = "Mean"
        '
        'CV
        '
        DataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        Me.CV.DefaultCellStyle = DataGridViewCellStyle4
        Me.CV.HeaderText = "CV"
        Me.CV.Name = "CV"
        Me.CV.Width = 50
        '
        'Lower
        '
        DataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        Me.Lower.DefaultCellStyle = DataGridViewCellStyle5
        Me.Lower.HeaderText = "Lower"
        Me.Lower.Name = "Lower"
        Me.Lower.Width = 50
        '
        'Upper
        '
        DataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        Me.Upper.DefaultCellStyle = DataGridViewCellStyle6
        Me.Upper.HeaderText = "Upper"
        Me.Upper.Name = "Upper"
        Me.Upper.Width = 50
        '
        'btnClose
        '
        Me.btnClose.Location = New System.Drawing.Point(352, 47)
        Me.btnClose.Name = "btnClose"
        Me.btnClose.Size = New System.Drawing.Size(138, 22)
        Me.btnClose.TabIndex = 4
        Me.btnClose.Text = "Close"
        Me.btnClose.UseVisualStyleBackColor = True
        '
        'btnSaveAndClose
        '
        Me.btnSaveAndClose.Location = New System.Drawing.Point(352, 87)
        Me.btnSaveAndClose.Name = "btnSaveAndClose"
        Me.btnSaveAndClose.Size = New System.Drawing.Size(138, 22)
        Me.btnSaveAndClose.TabIndex = 5
        Me.btnSaveAndClose.Text = "Save and Close"
        Me.btnSaveAndClose.UseVisualStyleBackColor = True
        '
        'frmDistributionParameters
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(992, 519)
        Me.Controls.Add(Me.btnSaveAndClose)
        Me.Controls.Add(Me.btnClose)
        Me.Controls.Add(Me.dgvParameters)
        Me.Controls.Add(Me.cboParamName)
        Me.Controls.Add(Me.cboPathOrSim)
        Me.Name = "frmDistributionParameters"
        Me.Text = "Distribution Parameters"
        CType(Me.dgvParameters, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents cboPathOrSim As System.Windows.Forms.ComboBox
    Friend WithEvents cboParamName As System.Windows.Forms.ComboBox
    Friend WithEvents dgvParameters As System.Windows.Forms.DataGridView
    Friend WithEvents btnClose As System.Windows.Forms.Button
    Friend WithEvents btnSaveAndClose As System.Windows.Forms.Button
    Friend WithEvents GroupNumber As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents GroupName As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Mean As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents CV As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Lower As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Upper As System.Windows.Forms.DataGridViewTextBoxColumn
End Class
