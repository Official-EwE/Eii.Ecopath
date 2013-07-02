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
        Me.txtnTrials = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.btnLoadSampled = New System.Windows.Forms.Button()
        Me.txtNYearsProject = New System.Windows.Forms.TextBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.txtTolerance = New System.Windows.Forms.TextBox()
        Me.btnVulnerabilities = New System.Windows.Forms.Button()
        Me.btnEcopathParams = New System.Windows.Forms.Button()
        Me.btnSample = New System.Windows.Forms.Button()
        Me.btnGamma = New System.Windows.Forms.Button()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.btShowTFMForm = New System.Windows.Forms.Button()
        Me.btnEcopathParams2 = New System.Windows.Forms.Button()
        Me.Panel3 = New System.Windows.Forms.Panel()
        Me.Panel4 = New System.Windows.Forms.Panel()
        Me.Panel5 = New System.Windows.Forms.Panel()
        Me.Panel6 = New System.Windows.Forms.Panel()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.Panel3.SuspendLayout()
        Me.Panel4.SuspendLayout()
        Me.Panel5.SuspendLayout()
        Me.Panel6.SuspendLayout()
        Me.Panel1.SuspendLayout()
        Me.Panel2.SuspendLayout()
        Me.SuspendLayout()
        '
        'txtnTrials
        '
        Me.txtnTrials.Location = New System.Drawing.Point(102, 14)
        Me.txtnTrials.Name = "txtnTrials"
        Me.txtnTrials.Size = New System.Drawing.Size(70, 20)
        Me.txtnTrials.TabIndex = 0
        Me.txtnTrials.Text = "5"
        Me.txtnTrials.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(60, 17)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(35, 13)
        Me.Label1.TabIndex = 4
        Me.Label1.Text = "Trials:"
        '
        'btnLoadSampled
        '
        Me.btnLoadSampled.Location = New System.Drawing.Point(200, 7)
        Me.btnLoadSampled.Name = "btnLoadSampled"
        Me.btnLoadSampled.Size = New System.Drawing.Size(121, 32)
        Me.btnLoadSampled.TabIndex = 6
        Me.btnLoadSampled.Text = "Run"
        Me.btnLoadSampled.UseVisualStyleBackColor = True
        '
        'txtNYearsProject
        '
        Me.txtNYearsProject.Location = New System.Drawing.Point(124, 13)
        Me.txtNYearsProject.Name = "txtNYearsProject"
        Me.txtNYearsProject.Size = New System.Drawing.Size(70, 20)
        Me.txtNYearsProject.TabIndex = 13
        Me.txtNYearsProject.Text = "5"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(20, 7)
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
        Me.Label2.Location = New System.Drawing.Point(20, 7)
        Me.Label2.MaximumSize = New System.Drawing.Size(100, 0)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(75, 26)
        Me.Label2.TabIndex = 15
        Me.Label2.Text = "Tolerance for mass-balance:"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'txtTolerance
        '
        Me.txtTolerance.Location = New System.Drawing.Point(102, 13)
        Me.txtTolerance.Name = "txtTolerance"
        Me.txtTolerance.Size = New System.Drawing.Size(70, 20)
        Me.txtTolerance.TabIndex = 1
        Me.txtTolerance.Text = "0.0005"
        Me.txtTolerance.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'btnVulnerabilities
        '
        Me.btnVulnerabilities.Location = New System.Drawing.Point(73, 4)
        Me.btnVulnerabilities.Name = "btnVulnerabilities"
        Me.btnVulnerabilities.Size = New System.Drawing.Size(121, 39)
        Me.btnVulnerabilities.TabIndex = 7
        Me.btnVulnerabilities.Text = "Get Vulnerabilities"
        Me.btnVulnerabilities.UseVisualStyleBackColor = True
        '
        'btnEcopathParams
        '
        Me.btnEcopathParams.Location = New System.Drawing.Point(200, 3)
        Me.btnEcopathParams.Name = "btnEcopathParams"
        Me.btnEcopathParams.Size = New System.Drawing.Size(121, 39)
        Me.btnEcopathParams.TabIndex = 2
        Me.btnEcopathParams.Text = "Get mass-balance Ecopath parameters"
        Me.btnEcopathParams.UseVisualStyleBackColor = True
        '
        'btnSample
        '
        Me.btnSample.Location = New System.Drawing.Point(200, 4)
        Me.btnSample.Name = "btnSample"
        Me.btnSample.Size = New System.Drawing.Size(121, 39)
        Me.btnSample.TabIndex = 3
        Me.btnSample.Text = "Get Other Ecosim Parameters"
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
        Me.Label3.Location = New System.Drawing.Point(270, 107)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(95, 13)
        Me.Label3.TabIndex = 20
        Me.Label3.Text = "SampleParameters"
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(200, 4)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(121, 39)
        Me.Button1.TabIndex = 22
        Me.Button1.Text = "Change Data Directory"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'btShowTFMForm
        '
        Me.btShowTFMForm.Location = New System.Drawing.Point(203, 7)
        Me.btShowTFMForm.Name = "btShowTFMForm"
        Me.btShowTFMForm.Size = New System.Drawing.Size(121, 32)
        Me.btShowTFMForm.TabIndex = 24
        Me.btShowTFMForm.Text = "HCRs"
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
        Me.Panel3.Location = New System.Drawing.Point(36, 104)
        Me.Panel3.Name = "Panel3"
        Me.Panel3.Size = New System.Drawing.Size(329, 48)
        Me.Panel3.TabIndex = 26
        '
        'Panel4
        '
        Me.Panel4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Panel4.Controls.Add(Me.txtTolerance)
        Me.Panel4.Controls.Add(Me.Label2)
        Me.Panel4.Controls.Add(Me.btnEcopathParams)
        Me.Panel4.Location = New System.Drawing.Point(36, 176)
        Me.Panel4.Name = "Panel4"
        Me.Panel4.Size = New System.Drawing.Size(329, 48)
        Me.Panel4.TabIndex = 27
        '
        'Panel5
        '
        Me.Panel5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Panel5.Controls.Add(Me.btnVulnerabilities)
        Me.Panel5.Controls.Add(Me.btnSample)
        Me.Panel5.Location = New System.Drawing.Point(36, 250)
        Me.Panel5.Name = "Panel5"
        Me.Panel5.Size = New System.Drawing.Size(329, 48)
        Me.Panel5.TabIndex = 28
        '
        'Panel6
        '
        Me.Panel6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Panel6.Controls.Add(Me.Label4)
        Me.Panel6.Controls.Add(Me.txtNYearsProject)
        Me.Panel6.Controls.Add(Me.btnLoadSampled)
        Me.Panel6.Location = New System.Drawing.Point(36, 402)
        Me.Panel6.Name = "Panel6"
        Me.Panel6.Size = New System.Drawing.Size(329, 48)
        Me.Panel6.TabIndex = 29
        '
        'Panel1
        '
        Me.Panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Panel1.Controls.Add(Me.btShowTFMForm)
        Me.Panel1.Location = New System.Drawing.Point(36, 326)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(329, 48)
        Me.Panel1.TabIndex = 27
        '
        'Panel2
        '
        Me.Panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Panel2.Controls.Add(Me.Button1)
        Me.Panel2.Controls.Add(Me.txtnTrials)
        Me.Panel2.Controls.Add(Me.Label1)
        Me.Panel2.Location = New System.Drawing.Point(36, 35)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(329, 48)
        Me.Panel2.TabIndex = 27
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(40, 19)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(48, 13)
        Me.Label5.TabIndex = 30
        Me.Label5.Text = "Settings:"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(40, 88)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(124, 13)
        Me.Label6.TabIndex = 31
        Me.Label6.Text = "Generate diet matrix csv:"
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(40, 160)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(106, 13)
        Me.Label7.TabIndex = 32
        Me.Label7.Text = "Ecopath Parameters:"
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(40, 234)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(100, 13)
        Me.Label8.TabIndex = 33
        Me.Label8.Text = "Ecosim Parameters:"
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Location = New System.Drawing.Point(40, 310)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(38, 13)
        Me.Label9.TabIndex = 34
        Me.Label9.Text = "HCRs:"
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Location = New System.Drawing.Point(40, 386)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(79, 13)
        Me.Label10.TabIndex = 35
        Me.Label10.Text = "Run projection:"
        '
        'frmMSE
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(879, 477)
        Me.Controls.Add(Me.Label10)
        Me.Controls.Add(Me.Label9)
        Me.Controls.Add(Me.Label8)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.Panel2)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.Panel6)
        Me.Controls.Add(Me.Panel5)
        Me.Controls.Add(Me.Panel4)
        Me.Controls.Add(Me.Panel3)
        Me.Controls.Add(Me.btnEcopathParams2)
        Me.Controls.Add(Me.Label3)
        Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Name = "frmMSE"
        Me.Text = "Cefas MSE"
        Me.Panel3.ResumeLayout(False)
        Me.Panel4.ResumeLayout(False)
        Me.Panel4.PerformLayout()
        Me.Panel5.ResumeLayout(False)
        Me.Panel6.ResumeLayout(False)
        Me.Panel6.PerformLayout()
        Me.Panel1.ResumeLayout(False)
        Me.Panel2.ResumeLayout(False)
        Me.Panel2.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents txtnTrials As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents btnLoadSampled As System.Windows.Forms.Button
    Friend WithEvents txtNYearsProject As System.Windows.Forms.TextBox
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents txtTolerance As System.Windows.Forms.TextBox
    Friend WithEvents btnEcopathParams As System.Windows.Forms.Button
    Friend WithEvents btnSample As System.Windows.Forms.Button
    Friend WithEvents btnGamma As System.Windows.Forms.Button
    Friend WithEvents btnVulnerabilities As System.Windows.Forms.Button
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Button1 As System.Windows.Forms.Button
    Friend WithEvents btShowTFMForm As System.Windows.Forms.Button
    Friend WithEvents btnEcopathParams2 As System.Windows.Forms.Button
    Friend WithEvents Panel3 As System.Windows.Forms.Panel
    Friend WithEvents Panel4 As System.Windows.Forms.Panel
    Friend WithEvents Panel5 As System.Windows.Forms.Panel
    Friend WithEvents Panel6 As System.Windows.Forms.Panel
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents Panel2 As System.Windows.Forms.Panel
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents Label10 As System.Windows.Forms.Label
End Class
