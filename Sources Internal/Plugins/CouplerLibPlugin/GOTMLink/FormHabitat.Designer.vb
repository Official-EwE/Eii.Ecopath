<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FormHabitat
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
        Me.DataGridView1 = New System.Windows.Forms.DataGridView()
        Me.Habitat = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.mindepth = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.maxdepth = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.tempdate = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.mintemp = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.maxtemp = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.custname = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.cust1date = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.cus1max = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.cust1max = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.Button2 = New System.Windows.Forms.Button()
        Me.DateTimePicker1 = New System.Windows.Forms.DateTimePicker()
        Me.Button3 = New System.Windows.Forms.Button()
        Me.TextBox2 = New System.Windows.Forms.TextBox()
        Me.TextBox3 = New System.Windows.Forms.TextBox()
        Me.TextBox4 = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.CheckBox1 = New System.Windows.Forms.CheckBox()
        Me.TextBox1 = New System.Windows.Forms.TextBox()
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'DataGridView1
        '
        Me.DataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DataGridView1.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.Habitat, Me.mindepth, Me.maxdepth, Me.tempdate, Me.mintemp, Me.maxtemp, Me.custname, Me.cust1date, Me.cus1max, Me.cust1max})
        Me.DataGridView1.Location = New System.Drawing.Point(12, 58)
        Me.DataGridView1.Name = "DataGridView1"
        Me.DataGridView1.RowHeadersWidth = 30
        Me.DataGridView1.Size = New System.Drawing.Size(893, 296)
        Me.DataGridView1.TabIndex = 0
        '
        'Habitat
        '
        Me.Habitat.HeaderText = "Habitat Number"
        Me.Habitat.Name = "Habitat"
        Me.Habitat.Width = 80
        '
        'mindepth
        '
        Me.mindepth.HeaderText = "Minimum Depth"
        Me.mindepth.Name = "mindepth"
        Me.mindepth.Width = 80
        '
        'maxdepth
        '
        Me.maxdepth.HeaderText = "Maximum Depth"
        Me.maxdepth.Name = "maxdepth"
        Me.maxdepth.Width = 80
        '
        'tempdate
        '
        Me.tempdate.HeaderText = "Temperature Date"
        Me.tempdate.Name = "tempdate"
        Me.tempdate.Width = 80
        '
        'mintemp
        '
        Me.mintemp.HeaderText = "Minimum Temperature"
        Me.mintemp.Name = "mintemp"
        Me.mintemp.Width = 80
        '
        'maxtemp
        '
        Me.maxtemp.HeaderText = "Maximum Temperature"
        Me.maxtemp.Name = "maxtemp"
        Me.maxtemp.Width = 80
        '
        'custname
        '
        Me.custname.HeaderText = "Custom 1 Name"
        Me.custname.Name = "custname"
        Me.custname.Width = 80
        '
        'cust1date
        '
        Me.cust1date.HeaderText = "Custom 1 Date"
        Me.cust1date.Name = "cust1date"
        Me.cust1date.Width = 80
        '
        'cus1max
        '
        Me.cus1max.HeaderText = "Custom 1 Minimum"
        Me.cus1max.Name = "cus1max"
        Me.cus1max.Width = 80
        '
        'cust1max
        '
        Me.cust1max.HeaderText = "Custom 1 Maximum"
        Me.cust1max.Name = "cust1max"
        Me.cust1max.Width = 80
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(596, 421)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(75, 23)
        Me.Button1.TabIndex = 1
        Me.Button1.Text = "Finish"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'Button2
        '
        Me.Button2.Location = New System.Drawing.Point(677, 421)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(75, 23)
        Me.Button2.TabIndex = 2
        Me.Button2.Text = "Cancel"
        Me.Button2.UseVisualStyleBackColor = True
        '
        'DateTimePicker1
        '
        Me.DateTimePicker1.Location = New System.Drawing.Point(295, 405)
        Me.DateTimePicker1.Name = "DateTimePicker1"
        Me.DateTimePicker1.Size = New System.Drawing.Size(152, 20)
        Me.DateTimePicker1.TabIndex = 3
        '
        'Button3
        '
        Me.Button3.Location = New System.Drawing.Point(462, 402)
        Me.Button3.Name = "Button3"
        Me.Button3.Size = New System.Drawing.Size(75, 23)
        Me.Button3.TabIndex = 4
        Me.Button3.Text = "Paste"
        Me.Button3.UseVisualStyleBackColor = True
        '
        'TextBox2
        '
        Me.TextBox2.CausesValidation = False
        Me.TextBox2.Location = New System.Drawing.Point(240, 373)
        Me.TextBox2.Name = "TextBox2"
        Me.TextBox2.Size = New System.Drawing.Size(100, 20)
        Me.TextBox2.TabIndex = 6
        '
        'TextBox3
        '
        Me.TextBox3.Location = New System.Drawing.Point(419, 373)
        Me.TextBox3.Name = "TextBox3"
        Me.TextBox3.Size = New System.Drawing.Size(100, 20)
        Me.TextBox3.TabIndex = 7
        '
        'TextBox4
        '
        Me.TextBox4.Location = New System.Drawing.Point(596, 373)
        Me.TextBox4.Name = "TextBox4"
        Me.TextBox4.Size = New System.Drawing.Size(100, 20)
        Me.TextBox4.TabIndex = 8
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(26, 380)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(34, 13)
        Me.Label1.TabIndex = 9
        Me.Label1.Text = "x_min"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(191, 376)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(34, 13)
        Me.Label2.TabIndex = 10
        Me.Label2.Text = "x-max"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(363, 376)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(31, 13)
        Me.Label3.TabIndex = 11
        Me.Label3.Text = "y-min"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(541, 376)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(34, 13)
        Me.Label4.TabIndex = 12
        Me.Label4.Text = "y-max"
        '
        'CheckBox1
        '
        Me.CheckBox1.AutoSize = True
        Me.CheckBox1.Location = New System.Drawing.Point(749, 376)
        Me.CheckBox1.Name = "CheckBox1"
        Me.CheckBox1.Size = New System.Drawing.Size(43, 17)
        Me.CheckBox1.TabIndex = 13
        Me.CheckBox1.Text = "Clip"
        Me.CheckBox1.UseVisualStyleBackColor = True
        '
        'TextBox1
        '
        Me.TextBox1.Location = New System.Drawing.Point(66, 374)
        Me.TextBox1.Name = "TextBox1"
        Me.TextBox1.Size = New System.Drawing.Size(100, 20)
        Me.TextBox1.TabIndex = 14
        '
        'FormHabitat
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(925, 451)
        Me.Controls.Add(Me.TextBox1)
        Me.Controls.Add(Me.CheckBox1)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.TextBox4)
        Me.Controls.Add(Me.TextBox3)
        Me.Controls.Add(Me.TextBox2)
        Me.Controls.Add(Me.Button3)
        Me.Controls.Add(Me.DateTimePicker1)
        Me.Controls.Add(Me.Button2)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.DataGridView1)
        Me.Name = "FormHabitat"
        Me.Text = "Form1"
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents DataGridView1 As System.Windows.Forms.DataGridView
    Friend WithEvents Button1 As System.Windows.Forms.Button
    Friend WithEvents Button2 As System.Windows.Forms.Button
    Friend WithEvents Habitat As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents mindepth As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents maxdepth As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents tempdate As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents mintemp As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents maxtemp As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents custname As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents cust1date As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents cus1max As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents cust1max As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents DateTimePicker1 As System.Windows.Forms.DateTimePicker
    Friend WithEvents Button3 As System.Windows.Forms.Button
    Friend WithEvents TextBox2 As System.Windows.Forms.TextBox
    Friend WithEvents TextBox3 As System.Windows.Forms.TextBox
    Friend WithEvents TextBox4 As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents CheckBox1 As System.Windows.Forms.CheckBox
    Friend WithEvents TextBox1 As System.Windows.Forms.TextBox
End Class
