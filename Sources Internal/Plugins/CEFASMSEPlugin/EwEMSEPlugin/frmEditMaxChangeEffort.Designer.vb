<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmEditDecreaseEffort
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
        Me.dgvMaxDecreaseEffort = New System.Windows.Forms.DataGridView()
        Me.FleetNumber = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.FleetName = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.MaxDecrease = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.btnOK = New System.Windows.Forms.Button()
        CType(Me.dgvMaxDecreaseEffort, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'dgvMaxDecreaseEffort
        '
        Me.dgvMaxDecreaseEffort.AllowUserToAddRows = False
        Me.dgvMaxDecreaseEffort.AllowUserToDeleteRows = False
        Me.dgvMaxDecreaseEffort.AllowUserToResizeRows = False
        Me.dgvMaxDecreaseEffort.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvMaxDecreaseEffort.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.FleetNumber, Me.FleetName, Me.MaxDecrease})
        Me.dgvMaxDecreaseEffort.Location = New System.Drawing.Point(12, 12)
        Me.dgvMaxDecreaseEffort.Name = "dgvMaxDecreaseEffort"
        Me.dgvMaxDecreaseEffort.Size = New System.Drawing.Size(502, 359)
        Me.dgvMaxDecreaseEffort.TabIndex = 0
        '
        'FleetNumber
        '
        Me.FleetNumber.HeaderText = "Fleet Number"
        Me.FleetNumber.Name = "FleetNumber"
        '
        'FleetName
        '
        Me.FleetName.HeaderText = "Fleet Name"
        Me.FleetName.Name = "FleetName"
        '
        'MaxDecrease
        '
        Me.MaxDecrease.HeaderText = "Maximum percentage decrease in effort"
        Me.MaxDecrease.Name = "MaxDecrease"
        Me.MaxDecrease.Width = 220
        '
        'btnCancel
        '
        Me.btnCancel.Location = New System.Drawing.Point(274, 388)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(117, 26)
        Me.btnCancel.TabIndex = 1
        Me.btnCancel.Text = "Cancel"
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'btnOK
        '
        Me.btnOK.Location = New System.Drawing.Point(397, 388)
        Me.btnOK.Name = "btnOK"
        Me.btnOK.Size = New System.Drawing.Size(117, 26)
        Me.btnOK.TabIndex = 2
        Me.btnOK.Text = "OK"
        Me.btnOK.UseVisualStyleBackColor = True
        '
        'frmEditDecreaseEffort
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(535, 427)
        Me.Controls.Add(Me.btnOK)
        Me.Controls.Add(Me.btnCancel)
        Me.Controls.Add(Me.dgvMaxDecreaseEffort)
        Me.Name = "frmEditDecreaseEffort"
        Me.Text = "Edit maxim decrease in fishing effort"
        CType(Me.dgvMaxDecreaseEffort, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents dgvMaxDecreaseEffort As System.Windows.Forms.DataGridView
    Friend WithEvents FleetNumber As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents FleetName As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents MaxDecrease As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents btnCancel As System.Windows.Forms.Button
    Friend WithEvents btnOK As System.Windows.Forms.Button
End Class
