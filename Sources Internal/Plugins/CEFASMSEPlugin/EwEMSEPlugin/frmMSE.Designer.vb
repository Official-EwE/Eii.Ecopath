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
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.btnVulnerabilities = New System.Windows.Forms.Button()
        Me.btnEcopathParams = New System.Windows.Forms.Button()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.txtTolerance = New System.Windows.Forms.TextBox()
        Me.btnSample = New System.Windows.Forms.Button()
        Me.btnGamma = New System.Windows.Forms.Button()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.txtOptimIterations = New System.Windows.Forms.TextBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.Button2 = New System.Windows.Forms.Button()
        Me.btShowTFMForm = New System.Windows.Forms.Button()
        Me.Panel1.SuspendLayout()
        Me.Panel2.SuspendLayout()
        Me.SuspendLayout()
        '
        'txtnTrials
        '
        Me.txtnTrials.Location = New System.Drawing.Point(549, 29)
        Me.txtnTrials.Name = "txtnTrials"
        Me.txtnTrials.Size = New System.Drawing.Size(50, 20)
        Me.txtnTrials.TabIndex = 0
        Me.txtnTrials.Text = "5"
        Me.txtnTrials.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(490, 32)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(35, 13)
        Me.Label1.TabIndex = 4
        Me.Label1.Text = "Trials:"
        '
        'btnLoadSampled
        '
        Me.btnLoadSampled.Location = New System.Drawing.Point(181, 72)
        Me.btnLoadSampled.Name = "btnLoadSampled"
        Me.btnLoadSampled.Size = New System.Drawing.Size(118, 46)
        Me.btnLoadSampled.TabIndex = 6
        Me.btnLoadSampled.Text = "Generate Results"
        Me.btnLoadSampled.UseVisualStyleBackColor = True
        '
        'txtNYearsProject
        '
        Me.txtNYearsProject.Location = New System.Drawing.Point(229, 10)
        Me.txtNYearsProject.Name = "txtNYearsProject"
        Me.txtNYearsProject.Size = New System.Drawing.Size(70, 20)
        Me.txtNYearsProject.TabIndex = 13
        Me.txtNYearsProject.Text = "5"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(89, 13)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(137, 13)
        Me.Label4.TabIndex = 14
        Me.Label4.Text = "Number of Years to Project:"
        Me.Label4.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'Panel1
        '
        Me.Panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Panel1.Controls.Add(Me.btnVulnerabilities)
        Me.Panel1.Controls.Add(Me.btnEcopathParams)
        Me.Panel1.Controls.Add(Me.Label2)
        Me.Panel1.Controls.Add(Me.txtTolerance)
        Me.Panel1.Controls.Add(Me.btnSample)
        Me.Panel1.Location = New System.Drawing.Point(269, 82)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(329, 177)
        Me.Panel1.TabIndex = 18
        '
        'btnVulnerabilities
        '
        Me.btnVulnerabilities.Location = New System.Drawing.Point(54, 78)
        Me.btnVulnerabilities.Name = "btnVulnerabilities"
        Me.btnVulnerabilities.Size = New System.Drawing.Size(121, 45)
        Me.btnVulnerabilities.TabIndex = 7
        Me.btnVulnerabilities.Text = "Get Vulnerabilities"
        Me.btnVulnerabilities.UseVisualStyleBackColor = True
        '
        'btnEcopathParams
        '
        Me.btnEcopathParams.Location = New System.Drawing.Point(181, 14)
        Me.btnEcopathParams.Name = "btnEcopathParams"
        Me.btnEcopathParams.Size = New System.Drawing.Size(118, 46)
        Me.btnEcopathParams.TabIndex = 2
        Me.btnEcopathParams.Text = "Get mass-balance Ecopath parameters"
        Me.btnEcopathParams.UseVisualStyleBackColor = True
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(82, 144)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(141, 13)
        Me.Label2.TabIndex = 15
        Me.Label2.Text = "Tolerance for mass-balance:"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'txtTolerance
        '
        Me.txtTolerance.Location = New System.Drawing.Point(229, 141)
        Me.txtTolerance.Name = "txtTolerance"
        Me.txtTolerance.Size = New System.Drawing.Size(70, 20)
        Me.txtTolerance.TabIndex = 1
        Me.txtTolerance.Text = "0.0005"
        Me.txtTolerance.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'btnSample
        '
        Me.btnSample.Location = New System.Drawing.Point(181, 78)
        Me.btnSample.Name = "btnSample"
        Me.btnSample.Size = New System.Drawing.Size(118, 45)
        Me.btnSample.TabIndex = 3
        Me.btnSample.Text = "Get Other Ecosim Parameters"
        Me.btnSample.UseVisualStyleBackColor = True
        '
        'btnGamma
        '
        Me.btnGamma.Enabled = False
        Me.btnGamma.Location = New System.Drawing.Point(325, 98)
        Me.btnGamma.Name = "btnGamma"
        Me.btnGamma.Size = New System.Drawing.Size(121, 45)
        Me.btnGamma.TabIndex = 5
        Me.btnGamma.Text = "Generate Diet Matrix"
        Me.btnGamma.UseVisualStyleBackColor = True
        '
        'Panel2
        '
        Me.Panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Panel2.Controls.Add(Me.txtOptimIterations)
        Me.Panel2.Controls.Add(Me.Label6)
        Me.Panel2.Controls.Add(Me.txtNYearsProject)
        Me.Panel2.Controls.Add(Me.Label4)
        Me.Panel2.Controls.Add(Me.btnLoadSampled)
        Me.Panel2.Location = New System.Drawing.Point(269, 296)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(329, 133)
        Me.Panel2.TabIndex = 19
        '
        'txtOptimIterations
        '
        Me.txtOptimIterations.Location = New System.Drawing.Point(229, 36)
        Me.txtOptimIterations.Name = "txtOptimIterations"
        Me.txtOptimIterations.Size = New System.Drawing.Size(70, 20)
        Me.txtOptimIterations.TabIndex = 15
        Me.txtOptimIterations.Text = "1"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(70, 39)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(156, 13)
        Me.Label6.TabIndex = 16
        Me.Label6.Text = "Iterations for Effort Optimisation:"
        Me.Label6.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(268, 66)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(95, 13)
        Me.Label3.TabIndex = 20
        Me.Label3.Text = "SampleParameters"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(267, 280)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(89, 13)
        Me.Label5.TabIndex = 21
        Me.Label5.Text = "Generate Results"
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(270, 12)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(124, 37)
        Me.Button1.TabIndex = 22
        Me.Button1.Text = "Change Data Directory"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'Button2
        '
        Me.Button2.Location = New System.Drawing.Point(646, 122)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(129, 48)
        Me.Button2.TabIndex = 23
        Me.Button2.Text = "Test"
        Me.Button2.UseVisualStyleBackColor = True
        '
        'btShowTFMForm
        '
        Me.btShowTFMForm.Location = New System.Drawing.Point(646, 210)
        Me.btShowTFMForm.Name = "btShowTFMForm"
        Me.btShowTFMForm.Size = New System.Drawing.Size(205, 30)
        Me.btShowTFMForm.TabIndex = 24
        Me.btShowTFMForm.Text = "Show Target Fishing Mortalites..."
        Me.btShowTFMForm.UseVisualStyleBackColor = True
        '
        'frmMSE
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(938, 508)
        Me.Controls.Add(Me.btShowTFMForm)
        Me.Controls.Add(Me.Button2)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.btnGamma)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.txtnTrials)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.Panel2)
        Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Name = "frmMSE"
        Me.Text = "Cefas MSE"
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
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
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents txtTolerance As System.Windows.Forms.TextBox
    Friend WithEvents btnEcopathParams As System.Windows.Forms.Button
    Friend WithEvents btnSample As System.Windows.Forms.Button
    Friend WithEvents btnGamma As System.Windows.Forms.Button
    Friend WithEvents btnVulnerabilities As System.Windows.Forms.Button
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Panel2 As System.Windows.Forms.Panel
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents Button1 As System.Windows.Forms.Button
    Friend WithEvents txtOptimIterations As System.Windows.Forms.TextBox
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents Button2 As System.Windows.Forms.Button
    Friend WithEvents btShowTFMForm As System.Windows.Forms.Button
End Class
