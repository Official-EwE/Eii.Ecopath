<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMSE
    Inherits WeifenLuo.WinFormsUI.Docking.DockContent

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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMSE))
        Me.txtnTrials = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.btnLoadSampled = New System.Windows.Forms.Button()
        Me.txtNYearsProject = New System.Windows.Forms.TextBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.txtTolerance = New System.Windows.Forms.TextBox()
        Me.btnSample = New System.Windows.Forms.Button()
        Me.btnGamma = New System.Windows.Forms.Button()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.btShowTFMForm = New System.Windows.Forms.Button()
        Me.btnEcopathParams2 = New System.Windows.Forms.Button()
        Me.Panel3 = New System.Windows.Forms.Panel()
        Me.Panel4 = New System.Windows.Forms.Panel()
        Me.Panel6 = New System.Windows.Forms.Panel()
        Me.txtNModels2Run = New System.Windows.Forms.TextBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.btnAdvancedSettings = New System.Windows.Forms.Button()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.txtArea = New System.Windows.Forms.TextBox()
        Me.lblArea = New System.Windows.Forms.Label()
        Me.Button2 = New System.Windows.Forms.Button()
        Me.Panel3.SuspendLayout()
        Me.Panel4.SuspendLayout()
        Me.Panel6.SuspendLayout()
        Me.Panel1.SuspendLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'txtnTrials
        '
        Me.txtnTrials.Location = New System.Drawing.Point(119, 24)
        Me.txtnTrials.Name = "txtnTrials"
        Me.txtnTrials.Size = New System.Drawing.Size(70, 20)
        Me.txtnTrials.TabIndex = 0
        Me.txtnTrials.Text = "5"
        Me.txtnTrials.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(13, 27)
        Me.Label1.MaximumSize = New System.Drawing.Size(100, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(95, 13)
        Me.Label1.TabIndex = 4
        Me.Label1.Text = "Number of models:"
        '
        'btnLoadSampled
        '
        Me.btnLoadSampled.Location = New System.Drawing.Point(204, 53)
        Me.btnLoadSampled.Name = "btnLoadSampled"
        Me.btnLoadSampled.Size = New System.Drawing.Size(84, 30)
        Me.btnLoadSampled.TabIndex = 6
        Me.btnLoadSampled.Text = "Run"
        Me.btnLoadSampled.UseVisualStyleBackColor = True
        '
        'txtNYearsProject
        '
        Me.txtNYearsProject.Location = New System.Drawing.Point(119, 59)
        Me.txtNYearsProject.Name = "txtNYearsProject"
        Me.txtNYearsProject.Size = New System.Drawing.Size(70, 20)
        Me.txtNYearsProject.TabIndex = 13
        Me.txtNYearsProject.Text = "5"
        Me.txtNYearsProject.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(15, 53)
        Me.Label4.MaximumSize = New System.Drawing.Size(100, 0)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(98, 26)
        Me.Label4.TabIndex = 14
        Me.Label4.Text = "Number of Years to Project:"
        Me.Label4.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(424, 100)
        Me.Label2.MaximumSize = New System.Drawing.Size(100, 0)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(75, 26)
        Me.Label2.TabIndex = 15
        Me.Label2.Text = "Tolerance for mass-balance:"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.TopRight
        Me.Label2.Visible = False
        '
        'txtTolerance
        '
        Me.txtTolerance.Location = New System.Drawing.Point(505, 99)
        Me.txtTolerance.Name = "txtTolerance"
        Me.txtTolerance.Size = New System.Drawing.Size(70, 20)
        Me.txtTolerance.TabIndex = 1
        Me.txtTolerance.Text = "0.0005"
        Me.txtTolerance.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        Me.txtTolerance.Visible = False
        '
        'btnSample
        '
        Me.btnSample.Location = New System.Drawing.Point(204, 14)
        Me.btnSample.Name = "btnSample"
        Me.btnSample.Size = New System.Drawing.Size(84, 30)
        Me.btnSample.TabIndex = 3
        Me.btnSample.Text = "Get models"
        Me.btnSample.UseVisualStyleBackColor = True
        '
        'btnGamma
        '
        Me.btnGamma.Enabled = False
        Me.btnGamma.Location = New System.Drawing.Point(200, 6)
        Me.btnGamma.Name = "btnGamma"
        Me.btnGamma.Size = New System.Drawing.Size(121, 32)
        Me.btnGamma.TabIndex = 5
        Me.btnGamma.Text = "Generate Diet Matrix"
        Me.btnGamma.UseVisualStyleBackColor = True
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(747, 257)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(95, 13)
        Me.Label3.TabIndex = 20
        Me.Label3.Text = "SampleParameters"
        Me.Label3.Visible = False
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(80, 48)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(145, 21)
        Me.Button1.TabIndex = 22
        Me.Button1.Text = "Change Data Directory"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'btShowTFMForm
        '
        Me.btShowTFMForm.Location = New System.Drawing.Point(204, 12)
        Me.btShowTFMForm.Name = "btShowTFMForm"
        Me.btShowTFMForm.Size = New System.Drawing.Size(84, 42)
        Me.btShowTFMForm.TabIndex = 24
        Me.btShowTFMForm.Text = "Setup fishing strategies"
        Me.btShowTFMForm.UseVisualStyleBackColor = True
        '
        'btnEcopathParams2
        '
        Me.btnEcopathParams2.Location = New System.Drawing.Point(734, 48)
        Me.btnEcopathParams2.Name = "btnEcopathParams2"
        Me.btnEcopathParams2.Size = New System.Drawing.Size(108, 52)
        Me.btnEcopathParams2.TabIndex = 25
        Me.btnEcopathParams2.Text = "Test Ecopath Params2"
        Me.btnEcopathParams2.UseVisualStyleBackColor = True
        Me.btnEcopathParams2.Visible = False
        '
        'Panel3
        '
        Me.Panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Panel3.Controls.Add(Me.btnGamma)
        Me.Panel3.Location = New System.Drawing.Point(513, 254)
        Me.Panel3.Name = "Panel3"
        Me.Panel3.Size = New System.Drawing.Size(329, 48)
        Me.Panel3.TabIndex = 26
        Me.Panel3.Visible = False
        '
        'Panel4
        '
        Me.Panel4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Panel4.Controls.Add(Me.txtnTrials)
        Me.Panel4.Controls.Add(Me.btnSample)
        Me.Panel4.Controls.Add(Me.Label1)
        Me.Panel4.Location = New System.Drawing.Point(80, 85)
        Me.Panel4.Name = "Panel4"
        Me.Panel4.Size = New System.Drawing.Size(300, 58)
        Me.Panel4.TabIndex = 27
        '
        'Panel6
        '
        Me.Panel6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Panel6.Controls.Add(Me.txtNModels2Run)
        Me.Panel6.Controls.Add(Me.Label4)
        Me.Panel6.Controls.Add(Me.Label5)
        Me.Panel6.Controls.Add(Me.btnLoadSampled)
        Me.Panel6.Controls.Add(Me.txtNYearsProject)
        Me.Panel6.Location = New System.Drawing.Point(80, 200)
        Me.Panel6.Name = "Panel6"
        Me.Panel6.Size = New System.Drawing.Size(300, 93)
        Me.Panel6.TabIndex = 29
        '
        'txtNModels2Run
        '
        Me.txtNModels2Run.Location = New System.Drawing.Point(119, 20)
        Me.txtNModels2Run.Name = "txtNModels2Run"
        Me.txtNModels2Run.Size = New System.Drawing.Size(70, 20)
        Me.txtNModels2Run.TabIndex = 16
        Me.txtNModels2Run.Text = "5"
        Me.txtNModels2Run.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(13, 14)
        Me.Label5.MaximumSize = New System.Drawing.Size(100, 0)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(92, 26)
        Me.Label5.TabIndex = 17
        Me.Label5.Text = "Number of models to use:"
        Me.Label5.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(517, 238)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(124, 13)
        Me.Label6.TabIndex = 31
        Me.Label6.Text = "Generate diet matrix csv:"
        Me.Label6.Visible = False
        '
        'btnAdvancedSettings
        '
        Me.btnAdvancedSettings.Location = New System.Drawing.Point(357, 48)
        Me.btnAdvancedSettings.Name = "btnAdvancedSettings"
        Me.btnAdvancedSettings.Size = New System.Drawing.Size(23, 21)
        Me.btnAdvancedSettings.TabIndex = 36
        Me.btnAdvancedSettings.Text = "+"
        Me.btnAdvancedSettings.UseVisualStyleBackColor = True
        '
        'Panel1
        '
        Me.Panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Panel1.Controls.Add(Me.txtArea)
        Me.Panel1.Controls.Add(Me.lblArea)
        Me.Panel1.Controls.Add(Me.btShowTFMForm)
        Me.Panel1.Location = New System.Drawing.Point(80, 136)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(300, 66)
        Me.Panel1.TabIndex = 28
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(36, 91)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(38, 13)
        Me.Label7.TabIndex = 37
        Me.Label7.Text = "Step 1"
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(36, 149)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(38, 13)
        Me.Label8.TabIndex = 38
        Me.Label8.Text = "Step 2"
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Location = New System.Drawing.Point(36, 205)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(38, 13)
        Me.Label9.TabIndex = 39
        Me.Label9.Text = "Step 3"
        '
        'PictureBox1
        '
        Me.PictureBox1.BackColor = System.Drawing.Color.White
        Me.PictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.PictureBox1.Image = CType(resources.GetObject("PictureBox1.Image"), System.Drawing.Image)
        Me.PictureBox1.Location = New System.Drawing.Point(80, 309)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(300, 29)
        Me.PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.PictureBox1.TabIndex = 53
        Me.PictureBox1.TabStop = False
        '
        'txtArea
        '
        Me.txtArea.Location = New System.Drawing.Point(119, 24)
        Me.txtArea.Name = "txtArea"
        Me.txtArea.Size = New System.Drawing.Size(70, 20)
        Me.txtArea.TabIndex = 25
        Me.txtArea.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        Me.txtArea.Visible = False
        '
        'lblArea
        '
        Me.lblArea.AutoSize = True
        Me.lblArea.Location = New System.Drawing.Point(73, 27)
        Me.lblArea.MaximumSize = New System.Drawing.Size(100, 0)
        Me.lblArea.Name = "lblArea"
        Me.lblArea.Size = New System.Drawing.Size(32, 13)
        Me.lblArea.TabIndex = 26
        Me.lblArea.Text = "Area:"
        Me.lblArea.Visible = False
        '
        'Button2
        '
        Me.Button2.Location = New System.Drawing.Point(711, 137)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(99, 43)
        Me.Button2.TabIndex = 54
        Me.Button2.Text = "Button2"
        Me.Button2.UseVisualStyleBackColor = True
        '
        'frmMSE
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(879, 477)
        Me.Controls.Add(Me.Button2)
        Me.Controls.Add(Me.PictureBox1)
        Me.Controls.Add(Me.Label9)
        Me.Controls.Add(Me.Label8)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.txtTolerance)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.btnAdvancedSettings)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.Panel6)
        Me.Controls.Add(Me.Panel4)
        Me.Controls.Add(Me.Panel3)
        Me.Controls.Add(Me.btnEcopathParams2)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Panel1)
        Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Name = "frmMSE"
        Me.Text = "Cefas MSE"
        Me.Panel3.ResumeLayout(False)
        Me.Panel4.ResumeLayout(False)
        Me.Panel4.PerformLayout()
        Me.Panel6.ResumeLayout(False)
        Me.Panel6.PerformLayout()
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents txtnTrials As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents btnLoadSampled As System.Windows.Forms.Button
    Friend WithEvents txtNYearsProject As System.Windows.Forms.TextBox
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents txtTolerance As System.Windows.Forms.TextBox
    Friend WithEvents btnSample As System.Windows.Forms.Button
    Friend WithEvents btnGamma As System.Windows.Forms.Button
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Button1 As System.Windows.Forms.Button
    Friend WithEvents btShowTFMForm As System.Windows.Forms.Button
    Friend WithEvents btnEcopathParams2 As System.Windows.Forms.Button
    Friend WithEvents Panel3 As System.Windows.Forms.Panel
    Friend WithEvents Panel4 As System.Windows.Forms.Panel
    Friend WithEvents Panel6 As System.Windows.Forms.Panel
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents btnAdvancedSettings As System.Windows.Forms.Button
    Friend WithEvents txtNModels2Run As System.Windows.Forms.TextBox
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents PictureBox1 As System.Windows.Forms.PictureBox
    Friend WithEvents txtArea As System.Windows.Forms.TextBox
    Friend WithEvents lblArea As System.Windows.Forms.Label
    Friend WithEvents Button2 As System.Windows.Forms.Button
End Class
