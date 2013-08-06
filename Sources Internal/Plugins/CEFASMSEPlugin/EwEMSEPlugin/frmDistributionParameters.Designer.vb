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
        Dim ChartArea4 As System.Windows.Forms.DataVisualization.Charting.ChartArea = New System.Windows.Forms.DataVisualization.Charting.ChartArea()
        Dim Legend4 As System.Windows.Forms.DataVisualization.Charting.Legend = New System.Windows.Forms.DataVisualization.Charting.Legend()
        Dim Series4 As System.Windows.Forms.DataVisualization.Charting.Series = New System.Windows.Forms.DataVisualization.Charting.Series()
        Me.cboPathOrSim = New System.Windows.Forms.ComboBox()
        Me.cboParamName = New System.Windows.Forms.ComboBox()
        Me.DataGridView1 = New System.Windows.Forms.DataGridView()
        Me.GroupNumber = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.GroupName = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Mean = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.CV = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Lower = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Upper = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Chart1 = New System.Windows.Forms.DataVisualization.Charting.Chart()
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Chart1, System.ComponentModel.ISupportInitialize).BeginInit()
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
        'DataGridView1
        '
        Me.DataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DataGridView1.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.GroupNumber, Me.GroupName, Me.Mean, Me.CV, Me.Lower, Me.Upper})
        Me.DataGridView1.Location = New System.Drawing.Point(46, 138)
        Me.DataGridView1.Name = "DataGridView1"
        Me.DataGridView1.Size = New System.Drawing.Size(646, 349)
        Me.DataGridView1.TabIndex = 2
        '
        'GroupNumber
        '
        Me.GroupNumber.HeaderText = "Group Number"
        Me.GroupNumber.Name = "GroupNumber"
        '
        'GroupName
        '
        Me.GroupName.HeaderText = "Group Name"
        Me.GroupName.Name = "GroupName"
        '
        'Mean
        '
        Me.Mean.HeaderText = "Mean"
        Me.Mean.Name = "Mean"
        '
        'CV
        '
        Me.CV.HeaderText = "CV"
        Me.CV.Name = "CV"
        '
        'Lower
        '
        Me.Lower.HeaderText = "Lower"
        Me.Lower.Name = "Lower"
        '
        'Upper
        '
        Me.Upper.HeaderText = "Upper"
        Me.Upper.Name = "Upper"
        '
        'Chart1
        '
        ChartArea4.Name = "ChartArea1"
        Me.Chart1.ChartAreas.Add(ChartArea4)
        Legend4.Name = "Legend1"
        Me.Chart1.Legends.Add(Legend4)
        Me.Chart1.Location = New System.Drawing.Point(712, 147)
        Me.Chart1.Name = "Chart1"
        Series4.ChartArea = "ChartArea1"
        Series4.Legend = "Legend1"
        Series4.Name = "Series1"
        Me.Chart1.Series.Add(Series4)
        Me.Chart1.Size = New System.Drawing.Size(264, 212)
        Me.Chart1.TabIndex = 3
        Me.Chart1.Text = "Chart1"
        '
        'frmDistributionParameters
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(992, 519)
        Me.Controls.Add(Me.Chart1)
        Me.Controls.Add(Me.DataGridView1)
        Me.Controls.Add(Me.cboParamName)
        Me.Controls.Add(Me.cboPathOrSim)
        Me.Name = "frmDistributionParameters"
        Me.Text = "Distribution Parameters"
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Chart1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents cboPathOrSim As System.Windows.Forms.ComboBox
    Friend WithEvents cboParamName As System.Windows.Forms.ComboBox
    Friend WithEvents DataGridView1 As System.Windows.Forms.DataGridView
    Friend WithEvents GroupNumber As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents GroupName As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Mean As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents CV As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Lower As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Upper As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Chart1 As System.Windows.Forms.DataVisualization.Charting.Chart
End Class
