<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ucSingleFileDataSetConfigPage
    Inherits System.Windows.Forms.UserControl

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.m_tbxName = New System.Windows.Forms.TextBox()
        Me.m_lblDescription = New System.Windows.Forms.Label()
        Me.m_lblName = New System.Windows.Forms.Label()
        Me.m_tbxDescription = New System.Windows.Forms.TextBox()
        Me.m_lblFile = New System.Windows.Forms.Label()
        Me.m_tbxFile = New System.Windows.Forms.TextBox()
        Me.m_btnBrowse = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.m_rbFirstTimeStep = New System.Windows.Forms.RadioButton()
        Me.m_rbMonth = New System.Windows.Forms.RadioButton()
        Me.m_date = New System.Windows.Forms.DateTimePicker()
        Me.SuspendLayout()
        '
        'm_tbxName
        '
        Me.m_tbxName.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_tbxName.Location = New System.Drawing.Point(72, 3)
        Me.m_tbxName.MaxLength = 100
        Me.m_tbxName.Name = "m_tbxName"
        Me.m_tbxName.Size = New System.Drawing.Size(286, 20)
        Me.m_tbxName.TabIndex = 1
        '
        'm_lblDescription
        '
        Me.m_lblDescription.AutoSize = True
        Me.m_lblDescription.Location = New System.Drawing.Point(3, 32)
        Me.m_lblDescription.Name = "m_lblDescription"
        Me.m_lblDescription.Size = New System.Drawing.Size(63, 13)
        Me.m_lblDescription.TabIndex = 2
        Me.m_lblDescription.Text = "&Description:"
        '
        'm_lblName
        '
        Me.m_lblName.AutoSize = True
        Me.m_lblName.Location = New System.Drawing.Point(3, 6)
        Me.m_lblName.Name = "m_lblName"
        Me.m_lblName.Size = New System.Drawing.Size(38, 13)
        Me.m_lblName.TabIndex = 0
        Me.m_lblName.Text = "&Name:"
        '
        'm_tbxDescription
        '
        Me.m_tbxDescription.AcceptsReturn = True
        Me.m_tbxDescription.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_tbxDescription.Location = New System.Drawing.Point(72, 29)
        Me.m_tbxDescription.Multiline = True
        Me.m_tbxDescription.Name = "m_tbxDescription"
        Me.m_tbxDescription.Size = New System.Drawing.Size(286, 73)
        Me.m_tbxDescription.TabIndex = 3
        '
        'm_lblFile
        '
        Me.m_lblFile.AutoSize = True
        Me.m_lblFile.Location = New System.Drawing.Point(3, 111)
        Me.m_lblFile.Name = "m_lblFile"
        Me.m_lblFile.Size = New System.Drawing.Size(26, 13)
        Me.m_lblFile.TabIndex = 4
        Me.m_lblFile.Text = "&File:"
        '
        'm_tbxFile
        '
        Me.m_tbxFile.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_tbxFile.Location = New System.Drawing.Point(72, 108)
        Me.m_tbxFile.MaxLength = 100
        Me.m_tbxFile.Name = "m_tbxFile"
        Me.m_tbxFile.Size = New System.Drawing.Size(205, 20)
        Me.m_tbxFile.TabIndex = 5
        '
        'm_btnBrowse
        '
        Me.m_btnBrowse.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_btnBrowse.Location = New System.Drawing.Point(283, 106)
        Me.m_btnBrowse.Name = "m_btnBrowse"
        Me.m_btnBrowse.Size = New System.Drawing.Size(75, 23)
        Me.m_btnBrowse.TabIndex = 6
        Me.m_btnBrowse.Text = "&Browse"
        Me.m_btnBrowse.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(3, 136)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(33, 13)
        Me.Label1.TabIndex = 7
        Me.Label1.Text = "&Date:"
        '
        'm_rbFirstTimeStep
        '
        Me.m_rbFirstTimeStep.AutoSize = True
        Me.m_rbFirstTimeStep.Location = New System.Drawing.Point(72, 134)
        Me.m_rbFirstTimeStep.Name = "m_rbFirstTimeStep"
        Me.m_rbFirstTimeStep.Size = New System.Drawing.Size(89, 17)
        Me.m_rbFirstTimeStep.TabIndex = 8
        Me.m_rbFirstTimeStep.TabStop = True
        Me.m_rbFirstTimeStep.Text = "&First time step"
        Me.m_rbFirstTimeStep.UseVisualStyleBackColor = True
        '
        'm_rbMonth
        '
        Me.m_rbMonth.AutoSize = True
        Me.m_rbMonth.Location = New System.Drawing.Point(72, 157)
        Me.m_rbMonth.Name = "m_rbMonth"
        Me.m_rbMonth.Size = New System.Drawing.Size(92, 17)
        Me.m_rbMonth.TabIndex = 9
        Me.m_rbMonth.TabStop = True
        Me.m_rbMonth.Text = "&A fixed month:"
        Me.m_rbMonth.UseVisualStyleBackColor = True
        '
        'm_date
        '
        Me.m_date.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_date.CustomFormat = "MM/yyyy"
        Me.m_date.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.m_date.Location = New System.Drawing.Point(170, 156)
        Me.m_date.Name = "m_date"
        Me.m_date.Size = New System.Drawing.Size(107, 20)
        Me.m_date.TabIndex = 10
        '
        'ucSingleFileDataSetConfigPage
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.m_date)
        Me.Controls.Add(Me.m_rbMonth)
        Me.Controls.Add(Me.m_rbFirstTimeStep)
        Me.Controls.Add(Me.m_btnBrowse)
        Me.Controls.Add(Me.m_tbxFile)
        Me.Controls.Add(Me.m_tbxName)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.m_lblFile)
        Me.Controls.Add(Me.m_lblDescription)
        Me.Controls.Add(Me.m_lblName)
        Me.Controls.Add(Me.m_tbxDescription)
        Me.Name = "ucSingleFileDataSetConfigPage"
        Me.Size = New System.Drawing.Size(361, 182)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Private WithEvents m_tbxName As System.Windows.Forms.TextBox
    Private WithEvents m_lblDescription As System.Windows.Forms.Label
    Private WithEvents m_lblName As System.Windows.Forms.Label
    Private WithEvents m_tbxDescription As System.Windows.Forms.TextBox
    Private WithEvents m_lblFile As System.Windows.Forms.Label
    Private WithEvents m_tbxFile As System.Windows.Forms.TextBox
    Private WithEvents m_btnBrowse As System.Windows.Forms.Button
    Private WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents m_rbFirstTimeStep As System.Windows.Forms.RadioButton
    Friend WithEvents m_rbMonth As System.Windows.Forms.RadioButton
    Private WithEvents m_date As System.Windows.Forms.DateTimePicker

End Class
