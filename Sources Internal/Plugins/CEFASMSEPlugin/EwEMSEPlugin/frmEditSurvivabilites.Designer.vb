<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmEditSurvivabilites
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
        Me.dgvSurvivabilities = New System.Windows.Forms.DataGridView()
        Me.FleetNumber = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Fleet = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.GroupNumber = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Group = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Alpha = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Beta = New System.Windows.Forms.DataGridViewTextBoxColumn()
        CType(Me.dgvSurvivabilities, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'dgvSurvivabilities
        '
        Me.dgvSurvivabilities.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvSurvivabilities.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.FleetNumber, Me.Fleet, Me.GroupNumber, Me.Group, Me.Alpha, Me.Beta})
        Me.dgvSurvivabilities.Location = New System.Drawing.Point(23, 187)
        Me.dgvSurvivabilities.Name = "dgvSurvivabilities"
        Me.dgvSurvivabilities.Size = New System.Drawing.Size(628, 135)
        Me.dgvSurvivabilities.TabIndex = 0
        '
        'FleetNumber
        '
        Me.FleetNumber.HeaderText = "Fleet No."
        Me.FleetNumber.Name = "FleetNumber"
        Me.FleetNumber.Width = 40
        '
        'Fleet
        '
        Me.Fleet.HeaderText = "Fleet"
        Me.Fleet.Name = "Fleet"
        Me.Fleet.Width = 200
        '
        'GroupNumber
        '
        Me.GroupNumber.HeaderText = "Group No."
        Me.GroupNumber.Name = "GroupNumber"
        Me.GroupNumber.Width = 40
        '
        'Group
        '
        Me.Group.HeaderText = "Group"
        Me.Group.Name = "Group"
        Me.Group.Width = 200
        '
        'Alpha
        '
        Me.Alpha.HeaderText = "Alpha"
        Me.Alpha.Name = "Alpha"
        Me.Alpha.Width = 50
        '
        'Beta
        '
        Me.Beta.HeaderText = "Beta"
        Me.Beta.Name = "Beta"
        Me.Beta.Width = 50
        '
        'frmEditSurvivabilites
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(900, 334)
        Me.Controls.Add(Me.dgvSurvivabilities)
        Me.Name = "frmEditSurvivabilites"
        Me.Text = "frmEditSurvivabilites"
        CType(Me.dgvSurvivabilities, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents dgvSurvivabilities As System.Windows.Forms.DataGridView
    Friend WithEvents FleetNumber As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Fleet As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents GroupNumber As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Group As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Alpha As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Beta As System.Windows.Forms.DataGridViewTextBoxColumn
End Class
