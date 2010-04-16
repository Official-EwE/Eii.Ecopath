Imports WeifenLuo.WinFormsUI.Docking

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmOptions
    Inherits frmEwE

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
        Me.pnlRegOpt = New System.Windows.Forms.Panel
        Me.pnlUseReg = New System.Windows.Forms.Panel
        Me.rbEcosimEffort = New System.Windows.Forms.RadioButton
        Me.rbNoCap = New System.Windows.Forms.RadioButton
        Me.pnlFTracking = New System.Windows.Forms.Panel
        Me.txSBPower = New System.Windows.Forms.TextBox
        Me.Label6 = New System.Windows.Forms.Label
        Me.rbExact = New System.Windows.Forms.RadioButton
        Me.rbDirectExp = New System.Windows.Forms.RadioButton
        Me.rbCatchEstBio = New System.Windows.Forms.RadioButton
        Me.rbTrackUseQuota = New System.Windows.Forms.RadioButton
        Me.rbPredictEffort = New System.Windows.Forms.RadioButton
        Me.rbFTracking = New System.Windows.Forms.RadioButton
        Me.Label7 = New System.Windows.Forms.Label
        Me.pnlRunOpt = New System.Windows.Forms.Panel
        Me.txKalmanGain = New System.Windows.Forms.TextBox
        Me.Label8 = New System.Windows.Forms.Label
        Me.ckPlugin = New System.Windows.Forms.CheckBox
        Me.Label5 = New System.Windows.Forms.Label
        Me.txForecast = New System.Windows.Forms.TextBox
        Me.Label2 = New System.Windows.Forms.Label
        Me.pnlRegOpt.SuspendLayout()
        Me.pnlUseReg.SuspendLayout()
        Me.pnlFTracking.SuspendLayout()
        Me.pnlRunOpt.SuspendLayout()
        Me.SuspendLayout()
        '
        'pnlRegOpt
        '
        Me.pnlRegOpt.Controls.Add(Me.pnlUseReg)
        Me.pnlRegOpt.Controls.Add(Me.pnlFTracking)
        Me.pnlRegOpt.Controls.Add(Me.rbTrackUseQuota)
        Me.pnlRegOpt.Controls.Add(Me.rbFTracking)
        Me.pnlRegOpt.Controls.Add(Me.Label7)
        Me.pnlRegOpt.Location = New System.Drawing.Point(1, 2)
        Me.pnlRegOpt.Name = "pnlRegOpt"
        Me.pnlRegOpt.Size = New System.Drawing.Size(325, 339)
        Me.pnlRegOpt.TabIndex = 25
        '
        'pnlUseReg
        '
        Me.pnlUseReg.Controls.Add(Me.rbEcosimEffort)
        Me.pnlUseReg.Controls.Add(Me.rbNoCap)
        Me.pnlUseReg.Controls.Add(Me.rbPredictEffort)
        Me.pnlUseReg.Location = New System.Drawing.Point(16, 45)
        Me.pnlUseReg.Name = "pnlUseReg"
        Me.pnlUseReg.Size = New System.Drawing.Size(249, 81)
        Me.pnlUseReg.TabIndex = 27
        '
        'rbEcosimEffort
        '
        Me.rbEcosimEffort.AutoSize = True
        Me.rbEcosimEffort.Location = New System.Drawing.Point(19, 26)
        Me.rbEcosimEffort.Name = "rbEcosimEffort"
        Me.rbEcosimEffort.Size = New System.Drawing.Size(172, 17)
        Me.rbEcosimEffort.TabIndex = 1
        Me.rbEcosimEffort.Text = "Cap effort at the Ecosim level   "
        Me.rbEcosimEffort.UseVisualStyleBackColor = True
        '
        'rbNoCap
        '
        Me.rbNoCap.AutoSize = True
        Me.rbNoCap.Checked = True
        Me.rbNoCap.Location = New System.Drawing.Point(19, 3)
        Me.rbNoCap.Name = "rbNoCap"
        Me.rbNoCap.Size = New System.Drawing.Size(132, 17)
        Me.rbNoCap.TabIndex = 0
        Me.rbNoCap.TabStop = True
        Me.rbNoCap.Text = "No upper cap on effort"
        Me.rbNoCap.UseVisualStyleBackColor = True
        '
        'pnlFTracking
        '
        Me.pnlFTracking.Controls.Add(Me.txSBPower)
        Me.pnlFTracking.Controls.Add(Me.Label6)
        Me.pnlFTracking.Controls.Add(Me.rbExact)
        Me.pnlFTracking.Controls.Add(Me.rbDirectExp)
        Me.pnlFTracking.Controls.Add(Me.rbCatchEstBio)
        Me.pnlFTracking.Location = New System.Drawing.Point(16, 155)
        Me.pnlFTracking.Name = "pnlFTracking"
        Me.pnlFTracking.Size = New System.Drawing.Size(235, 119)
        Me.pnlFTracking.TabIndex = 26
        '
        'txSBPower
        '
        Me.txSBPower.Location = New System.Drawing.Point(167, 87)
        Me.txSBPower.Name = "txSBPower"
        Me.txSBPower.Size = New System.Drawing.Size(48, 20)
        Me.txSBPower.TabIndex = 25
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(16, 91)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(133, 13)
        Me.Label6.TabIndex = 24
        Me.Label6.Text = "Survey vs. biomass power:"
        '
        'rbExact
        '
        Me.rbExact.AutoSize = True
        Me.rbExact.Location = New System.Drawing.Point(19, 60)
        Me.rbExact.Name = "rbExact"
        Me.rbExact.Size = New System.Drawing.Size(128, 17)
        Me.rbExact.TabIndex = 23
        Me.rbExact.TabStop = True
        Me.rbExact.Text = "Exact biomass known"
        Me.rbExact.UseVisualStyleBackColor = True
        '
        'rbDirectExp
        '
        Me.rbDirectExp.AutoSize = True
        Me.rbDirectExp.Location = New System.Drawing.Point(19, 32)
        Me.rbDirectExp.Name = "rbDirectExp"
        Me.rbDirectExp.Size = New System.Drawing.Size(130, 17)
        Me.rbDirectExp.TabIndex = 22
        Me.rbDirectExp.Text = "Direct exploitation rate"
        Me.rbDirectExp.UseVisualStyleBackColor = True
        '
        'rbCatchEstBio
        '
        Me.rbCatchEstBio.AutoSize = True
        Me.rbCatchEstBio.Checked = True
        Me.rbCatchEstBio.Location = New System.Drawing.Point(19, 3)
        Me.rbCatchEstBio.Name = "rbCatchEstBio"
        Me.rbCatchEstBio.Size = New System.Drawing.Size(144, 17)
        Me.rbCatchEstBio.TabIndex = 21
        Me.rbCatchEstBio.TabStop = True
        Me.rbCatchEstBio.Text = "Catch/estimated biomass"
        Me.rbCatchEstBio.UseVisualStyleBackColor = True
        '
        'rbTrackUseQuota
        '
        Me.rbTrackUseQuota.AutoSize = True
        Me.rbTrackUseQuota.Checked = True
        Me.rbTrackUseQuota.Location = New System.Drawing.Point(6, 26)
        Me.rbTrackUseQuota.Name = "rbTrackUseQuota"
        Me.rbTrackUseQuota.Size = New System.Drawing.Size(133, 17)
        Me.rbTrackUseQuota.TabIndex = 26
        Me.rbTrackUseQuota.TabStop = True
        Me.rbTrackUseQuota.Text = "Use regulatory controls"
        Me.rbTrackUseQuota.UseVisualStyleBackColor = True
        '
        'rbPredictEffort
        '
        Me.rbPredictEffort.AutoSize = True
        Me.rbPredictEffort.Location = New System.Drawing.Point(19, 49)
        Me.rbPredictEffort.Name = "rbPredictEffort"
        Me.rbPredictEffort.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.rbPredictEffort.Size = New System.Drawing.Size(85, 17)
        Me.rbPredictEffort.TabIndex = 25
        Me.rbPredictEffort.Text = "Predict effort"
        Me.rbPredictEffort.UseVisualStyleBackColor = True
        '
        'rbFTracking
        '
        Me.rbFTracking.AutoSize = True
        Me.rbFTracking.Location = New System.Drawing.Point(6, 132)
        Me.rbFTracking.Name = "rbFTracking"
        Me.rbFTracking.Size = New System.Drawing.Size(294, 17)
        Me.rbFTracking.TabIndex = 24
        Me.rbFTracking.Text = "No regulatory controls (evaluate current Ecosim scenario)"
        Me.rbFTracking.UseVisualStyleBackColor = True
        '
        'Label7
        '
        Me.Label7.BackColor = System.Drawing.SystemColors.ButtonShadow
        Me.Label7.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label7.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.Label7.Location = New System.Drawing.Point(2, 0)
        Me.Label7.Name = "Label7"
        Me.Label7.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Label7.Size = New System.Drawing.Size(314, 18)
        Me.Label7.TabIndex = 18
        Me.Label7.Text = "Effort and regulatory options"
        Me.Label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'pnlRunOpt
        '
        Me.pnlRunOpt.Controls.Add(Me.txKalmanGain)
        Me.pnlRunOpt.Controls.Add(Me.Label8)
        Me.pnlRunOpt.Controls.Add(Me.ckPlugin)
        Me.pnlRunOpt.Controls.Add(Me.Label5)
        Me.pnlRunOpt.Controls.Add(Me.txForecast)
        Me.pnlRunOpt.Controls.Add(Me.Label2)
        Me.pnlRunOpt.Location = New System.Drawing.Point(332, 2)
        Me.pnlRunOpt.Name = "pnlRunOpt"
        Me.pnlRunOpt.Size = New System.Drawing.Size(211, 141)
        Me.pnlRunOpt.TabIndex = 34
        '
        'txKalmanGain
        '
        Me.txKalmanGain.Location = New System.Drawing.Point(134, 55)
        Me.txKalmanGain.Name = "txKalmanGain"
        Me.txKalmanGain.Size = New System.Drawing.Size(48, 20)
        Me.txKalmanGain.TabIndex = 38
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(3, 59)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(68, 13)
        Me.Label8.TabIndex = 37
        Me.Label8.Text = "Kalman gain:"
        '
        'ckPlugin
        '
        Me.ckPlugin.AutoSize = True
        Me.ckPlugin.Enabled = False
        Me.ckPlugin.Location = New System.Drawing.Point(6, 88)
        Me.ckPlugin.Name = "ckPlugin"
        Me.ckPlugin.Size = New System.Drawing.Size(149, 17)
        Me.ckPlugin.TabIndex = 33
        Me.ckPlugin.Text = "Use plugin economic data"
        Me.ckPlugin.UseVisualStyleBackColor = True
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(3, 30)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(103, 13)
        Me.Label5.TabIndex = 35
        Me.Label5.Text = "Forecast stock gain:"
        '
        'txForecast
        '
        Me.txForecast.Location = New System.Drawing.Point(134, 26)
        Me.txForecast.Name = "txForecast"
        Me.txForecast.Size = New System.Drawing.Size(48, 20)
        Me.txForecast.TabIndex = 34
        '
        'Label2
        '
        Me.Label2.BackColor = System.Drawing.SystemColors.ButtonShadow
        Me.Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.Label2.Location = New System.Drawing.Point(1, 0)
        Me.Label2.Name = "Label2"
        Me.Label2.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Label2.Size = New System.Drawing.Size(185, 18)
        Me.Label2.TabIndex = 32
        Me.Label2.Text = "Model run options"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'frmOptions
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1012, 620)
        Me.Controls.Add(Me.pnlRunOpt)
        Me.Controls.Add(Me.pnlRegOpt)
        Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Name = "frmOptions"
        Me.Text = "frmOptions"
        Me.pnlRegOpt.ResumeLayout(False)
        Me.pnlRegOpt.PerformLayout()
        Me.pnlUseReg.ResumeLayout(False)
        Me.pnlUseReg.PerformLayout()
        Me.pnlFTracking.ResumeLayout(False)
        Me.pnlFTracking.PerformLayout()
        Me.pnlRunOpt.ResumeLayout(False)
        Me.pnlRunOpt.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents pnlRegOpt As System.Windows.Forms.Panel
    Friend WithEvents rbTrackUseQuota As System.Windows.Forms.RadioButton
    Friend WithEvents rbPredictEffort As System.Windows.Forms.RadioButton
    Friend WithEvents rbFTracking As System.Windows.Forms.RadioButton
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents pnlUseReg As System.Windows.Forms.Panel
    Friend WithEvents pnlFTracking As System.Windows.Forms.Panel
    Private WithEvents txSBPower As System.Windows.Forms.TextBox
    Private WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents rbExact As System.Windows.Forms.RadioButton
    Private WithEvents rbDirectExp As System.Windows.Forms.RadioButton
    Private WithEvents rbCatchEstBio As System.Windows.Forms.RadioButton
    Friend WithEvents rbEcosimEffort As System.Windows.Forms.RadioButton
    Friend WithEvents rbNoCap As System.Windows.Forms.RadioButton
    Friend WithEvents pnlRunOpt As System.Windows.Forms.Panel
    Friend WithEvents txKalmanGain As System.Windows.Forms.TextBox
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Private WithEvents Label5 As System.Windows.Forms.Label
    Private WithEvents txForecast As System.Windows.Forms.TextBox
    Private WithEvents ckPlugin As System.Windows.Forms.CheckBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
End Class
