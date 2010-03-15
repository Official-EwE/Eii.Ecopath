Imports WeifenLuo.WinFormsUI.Docking

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMSE
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
        Me.components = New System.ComponentModel.Container
        Me.btRun = New System.Windows.Forms.Button
        Me.txNTrials = New System.Windows.Forms.TextBox
        Me.Label1 = New System.Windows.Forms.Label
        Me.lbRun = New System.Windows.Forms.Label
        Me.Label7 = New System.Windows.Forms.Label
        Me.Label3 = New System.Windows.Forms.Label
        Me.pnlRegOpt = New System.Windows.Forms.Panel
        Me.rbTrackUseQuota = New System.Windows.Forms.RadioButton
        Me.rbPredictEffort = New System.Windows.Forms.RadioButton
        Me.rbFTracking = New System.Windows.Forms.RadioButton
        Me.pnlFTracking = New System.Windows.Forms.Panel
        Me.Label4 = New System.Windows.Forms.Label
        Me.txSBPower = New System.Windows.Forms.TextBox
        Me.Label6 = New System.Windows.Forms.Label
        Me.rbExact = New System.Windows.Forms.RadioButton
        Me.rbDirectExp = New System.Windows.Forms.RadioButton
        Me.rbCatchEstBio = New System.Windows.Forms.RadioButton
        Me.btStop = New System.Windows.Forms.Button
        Me.zdGraph = New ZedGraph.ZedGraphControl
        Me.btShowHide = New System.Windows.Forms.Button
        Me.pnlRunOpt = New System.Windows.Forms.Panel
        Me.txKalmanGain = New System.Windows.Forms.TextBox
        Me.Label8 = New System.Windows.Forms.Label
        Me.ckSave = New System.Windows.Forms.CheckBox
        Me.Label5 = New System.Windows.Forms.Label
        Me.txForecast = New System.Windows.Forms.TextBox
        Me.ckPlugin = New System.Windows.Forms.CheckBox
        Me.Label2 = New System.Windows.Forms.Label
        Me.pnlRegOpt.SuspendLayout()
        Me.pnlFTracking.SuspendLayout()
        Me.pnlRunOpt.SuspendLayout()
        Me.SuspendLayout()
        '
        'btRun
        '
        Me.btRun.FlatStyle = System.Windows.Forms.FlatStyle.Popup
        Me.btRun.Location = New System.Drawing.Point(11, 52)
        Me.btRun.Margin = New System.Windows.Forms.Padding(0)
        Me.btRun.Name = "btRun"
        Me.btRun.Size = New System.Drawing.Size(146, 22)
        Me.btRun.TabIndex = 0
        Me.btRun.Text = "&Run"
        Me.btRun.UseVisualStyleBackColor = True
        '
        'txNTrials
        '
        Me.txNTrials.Location = New System.Drawing.Point(94, 24)
        Me.txNTrials.Name = "txNTrials"
        Me.txNTrials.Size = New System.Drawing.Size(63, 20)
        Me.txNTrials.TabIndex = 2
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(8, 28)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(83, 13)
        Me.Label1.TabIndex = 3
        Me.Label1.Text = "Number of trials:"
        '
        'lbRun
        '
        Me.lbRun.BackColor = System.Drawing.SystemColors.ButtonShadow
        Me.lbRun.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lbRun.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lbRun.Location = New System.Drawing.Point(8, 0)
        Me.lbRun.Name = "lbRun"
        Me.lbRun.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.lbRun.Size = New System.Drawing.Size(153, 21)
        Me.lbRun.TabIndex = 4
        Me.lbRun.Text = "Run"
        Me.lbRun.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label7
        '
        Me.Label7.BackColor = System.Drawing.SystemColors.ButtonShadow
        Me.Label7.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label7.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.Label7.Location = New System.Drawing.Point(3, 0)
        Me.Label7.Name = "Label7"
        Me.Label7.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Label7.Size = New System.Drawing.Size(314, 21)
        Me.Label7.TabIndex = 18
        Me.Label7.Text = "Effort and regulatory options"
        Me.Label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label3
        '
        Me.Label3.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label3.BackColor = System.Drawing.SystemColors.ButtonShadow
        Me.Label3.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.Label3.Location = New System.Drawing.Point(8, 143)
        Me.Label3.Name = "Label3"
        Me.Label3.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Label3.Size = New System.Drawing.Size(889, 22)
        Me.Label3.TabIndex = 9
        Me.Label3.Text = "Outputs"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'pnlRegOpt
        '
        Me.pnlRegOpt.Controls.Add(Me.rbTrackUseQuota)
        Me.pnlRegOpt.Controls.Add(Me.rbPredictEffort)
        Me.pnlRegOpt.Controls.Add(Me.rbFTracking)
        Me.pnlRegOpt.Controls.Add(Me.Label7)
        Me.pnlRegOpt.Location = New System.Drawing.Point(169, 0)
        Me.pnlRegOpt.Name = "pnlRegOpt"
        Me.pnlRegOpt.Size = New System.Drawing.Size(311, 137)
        Me.pnlRegOpt.TabIndex = 24
        '
        'rbTrackUseQuota
        '
        Me.rbTrackUseQuota.AutoSize = True
        Me.rbTrackUseQuota.Checked = True
        Me.rbTrackUseQuota.Location = New System.Drawing.Point(6, 26)
        Me.rbTrackUseQuota.Name = "rbTrackUseQuota"
        Me.rbTrackUseQuota.Size = New System.Drawing.Size(192, 17)
        Me.rbTrackUseQuota.TabIndex = 26
        Me.rbTrackUseQuota.TabStop = True
        Me.rbTrackUseQuota.Text = "Ecosim effort use regulatory options"
        Me.rbTrackUseQuota.UseVisualStyleBackColor = True
        '
        'rbPredictEffort
        '
        Me.rbPredictEffort.AutoSize = True
        Me.rbPredictEffort.Location = New System.Drawing.Point(6, 55)
        Me.rbPredictEffort.Name = "rbPredictEffort"
        Me.rbPredictEffort.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.rbPredictEffort.Size = New System.Drawing.Size(191, 17)
        Me.rbPredictEffort.TabIndex = 25
        Me.rbPredictEffort.Text = "Predict effort use regulatory options"
        Me.rbPredictEffort.UseVisualStyleBackColor = True
        '
        'rbFTracking
        '
        Me.rbFTracking.AutoSize = True
        Me.rbFTracking.Location = New System.Drawing.Point(6, 83)
        Me.rbFTracking.Name = "rbFTracking"
        Me.rbFTracking.Size = New System.Drawing.Size(284, 17)
        Me.rbFTracking.TabIndex = 24
        Me.rbFTracking.Text = "Ecosim effort no regulations (evaluate current scenario)"
        Me.rbFTracking.UseVisualStyleBackColor = True
        '
        'pnlFTracking
        '
        Me.pnlFTracking.Controls.Add(Me.Label4)
        Me.pnlFTracking.Controls.Add(Me.txSBPower)
        Me.pnlFTracking.Controls.Add(Me.Label6)
        Me.pnlFTracking.Controls.Add(Me.rbExact)
        Me.pnlFTracking.Controls.Add(Me.rbDirectExp)
        Me.pnlFTracking.Controls.Add(Me.rbCatchEstBio)
        Me.pnlFTracking.Location = New System.Drawing.Point(692, 0)
        Me.pnlFTracking.Name = "pnlFTracking"
        Me.pnlFTracking.Size = New System.Drawing.Size(205, 137)
        Me.pnlFTracking.TabIndex = 25
        '
        'Label4
        '
        Me.Label4.BackColor = System.Drawing.SystemColors.ButtonShadow
        Me.Label4.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label4.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.Label4.Location = New System.Drawing.Point(0, 0)
        Me.Label4.Name = "Label4"
        Me.Label4.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Label4.Size = New System.Drawing.Size(211, 21)
        Me.Label4.TabIndex = 29
        Me.Label4.Text = "Ecosim scenario options"
        Me.Label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'txSBPower
        '
        Me.txSBPower.Location = New System.Drawing.Point(154, 110)
        Me.txSBPower.Name = "txSBPower"
        Me.txSBPower.Size = New System.Drawing.Size(48, 20)
        Me.txSBPower.TabIndex = 25
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(3, 114)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(133, 13)
        Me.Label6.TabIndex = 24
        Me.Label6.Text = "Survey vs. biomass power:"
        '
        'rbExact
        '
        Me.rbExact.AutoSize = True
        Me.rbExact.Location = New System.Drawing.Point(6, 83)
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
        Me.rbDirectExp.Location = New System.Drawing.Point(6, 55)
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
        Me.rbCatchEstBio.Location = New System.Drawing.Point(6, 26)
        Me.rbCatchEstBio.Name = "rbCatchEstBio"
        Me.rbCatchEstBio.Size = New System.Drawing.Size(144, 17)
        Me.rbCatchEstBio.TabIndex = 21
        Me.rbCatchEstBio.TabStop = True
        Me.rbCatchEstBio.Text = "Catch/estimated biomass"
        Me.rbCatchEstBio.UseVisualStyleBackColor = True
        '
        'btStop
        '
        Me.btStop.FlatStyle = System.Windows.Forms.FlatStyle.Popup
        Me.btStop.Location = New System.Drawing.Point(11, 80)
        Me.btStop.Name = "btStop"
        Me.btStop.Size = New System.Drawing.Size(146, 22)
        Me.btStop.TabIndex = 26
        Me.btStop.Text = "Stop"
        Me.btStop.UseVisualStyleBackColor = True
        '
        'zdGraph
        '
        Me.zdGraph.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.zdGraph.Location = New System.Drawing.Point(8, 171)
        Me.zdGraph.Margin = New System.Windows.Forms.Padding(0)
        Me.zdGraph.Name = "zdGraph"
        Me.zdGraph.ScrollGrace = 0
        Me.zdGraph.ScrollMaxX = 0
        Me.zdGraph.ScrollMaxY = 0
        Me.zdGraph.ScrollMaxY2 = 0
        Me.zdGraph.ScrollMinX = 0
        Me.zdGraph.ScrollMinY = 0
        Me.zdGraph.ScrollMinY2 = 0
        Me.zdGraph.Size = New System.Drawing.Size(889, 596)
        Me.zdGraph.TabIndex = 27
        '
        'btShowHide
        '
        Me.btShowHide.FlatStyle = System.Windows.Forms.FlatStyle.Popup
        Me.btShowHide.Location = New System.Drawing.Point(12, 108)
        Me.btShowHide.Name = "btShowHide"
        Me.btShowHide.Size = New System.Drawing.Size(145, 25)
        Me.btShowHide.TabIndex = 32
        Me.btShowHide.Text = "Show/hide items..."
        Me.btShowHide.UseVisualStyleBackColor = True
        '
        'pnlRunOpt
        '
        Me.pnlRunOpt.Controls.Add(Me.txKalmanGain)
        Me.pnlRunOpt.Controls.Add(Me.Label8)
        Me.pnlRunOpt.Controls.Add(Me.ckSave)
        Me.pnlRunOpt.Controls.Add(Me.Label5)
        Me.pnlRunOpt.Controls.Add(Me.txForecast)
        Me.pnlRunOpt.Controls.Add(Me.ckPlugin)
        Me.pnlRunOpt.Controls.Add(Me.Label2)
        Me.pnlRunOpt.Location = New System.Drawing.Point(492, 0)
        Me.pnlRunOpt.Name = "pnlRunOpt"
        Me.pnlRunOpt.Size = New System.Drawing.Size(185, 137)
        Me.pnlRunOpt.TabIndex = 33
        '
        'txKalmanGain
        '
        Me.txKalmanGain.Location = New System.Drawing.Point(134, 110)
        Me.txKalmanGain.Name = "txKalmanGain"
        Me.txKalmanGain.Size = New System.Drawing.Size(48, 20)
        Me.txKalmanGain.TabIndex = 38
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(3, 114)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(68, 13)
        Me.Label8.TabIndex = 37
        Me.Label8.Text = "Kalman gain:"
        '
        'ckSave
        '
        Me.ckSave.AutoSize = True
        Me.ckSave.Enabled = False
        Me.ckSave.Location = New System.Drawing.Point(3, 55)
        Me.ckSave.Name = "ckSave"
        Me.ckSave.Size = New System.Drawing.Size(84, 17)
        Me.ckSave.TabIndex = 36
        Me.ckSave.Text = "Save output"
        Me.ckSave.UseVisualStyleBackColor = True
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(3, 85)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(103, 13)
        Me.Label5.TabIndex = 35
        Me.Label5.Text = "Forecast stock gain:"
        '
        'txForecast
        '
        Me.txForecast.Location = New System.Drawing.Point(134, 81)
        Me.txForecast.Name = "txForecast"
        Me.txForecast.Size = New System.Drawing.Size(48, 20)
        Me.txForecast.TabIndex = 34
        '
        'ckPlugin
        '
        Me.ckPlugin.AutoSize = True
        Me.ckPlugin.Enabled = False
        Me.ckPlugin.Location = New System.Drawing.Point(3, 26)
        Me.ckPlugin.Name = "ckPlugin"
        Me.ckPlugin.Size = New System.Drawing.Size(149, 17)
        Me.ckPlugin.TabIndex = 33
        Me.ckPlugin.Text = "Use plugin economic data"
        Me.ckPlugin.UseVisualStyleBackColor = True
        '
        'Label2
        '
        Me.Label2.BackColor = System.Drawing.SystemColors.ButtonShadow
        Me.Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.Label2.Location = New System.Drawing.Point(0, 0)
        Me.Label2.Name = "Label2"
        Me.Label2.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Label2.Size = New System.Drawing.Size(185, 21)
        Me.Label2.TabIndex = 32
        Me.Label2.Text = "Model run options"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'frmMSE
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(906, 776)
        Me.Controls.Add(Me.pnlRunOpt)
        Me.Controls.Add(Me.btShowHide)
        Me.Controls.Add(Me.zdGraph)
        Me.Controls.Add(Me.btStop)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.lbRun)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.txNTrials)
        Me.Controls.Add(Me.btRun)
        Me.Controls.Add(Me.pnlRegOpt)
        Me.Controls.Add(Me.pnlFTracking)
        Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Name = "frmMSE"
        Me.Text = "frmMSE"
        Me.pnlRegOpt.ResumeLayout(False)
        Me.pnlRegOpt.PerformLayout()
        Me.pnlFTracking.ResumeLayout(False)
        Me.pnlFTracking.PerformLayout()
        Me.pnlRunOpt.ResumeLayout(False)
        Me.pnlRunOpt.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents lbRun As System.Windows.Forms.Label
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Private WithEvents btRun As System.Windows.Forms.Button
    Private WithEvents txNTrials As System.Windows.Forms.TextBox
    Private WithEvents Label1 As System.Windows.Forms.Label
    Private WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents pnlRegOpt As System.Windows.Forms.Panel
    Friend WithEvents rbPredictEffort As System.Windows.Forms.RadioButton
    Friend WithEvents rbFTracking As System.Windows.Forms.RadioButton
    Friend WithEvents pnlFTracking As System.Windows.Forms.Panel
    Friend WithEvents rbExact As System.Windows.Forms.RadioButton
    Private WithEvents rbDirectExp As System.Windows.Forms.RadioButton
    Private WithEvents rbCatchEstBio As System.Windows.Forms.RadioButton
    Friend WithEvents rbTrackUseQuota As System.Windows.Forms.RadioButton
    Friend WithEvents btStop As System.Windows.Forms.Button
    Private WithEvents zdGraph As ZedGraph.ZedGraphControl
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Private WithEvents txSBPower As System.Windows.Forms.TextBox
    Private WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents btShowHide As System.Windows.Forms.Button
    Friend WithEvents pnlRunOpt As System.Windows.Forms.Panel
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents ckSave As System.Windows.Forms.CheckBox
    Private WithEvents Label5 As System.Windows.Forms.Label
    Private WithEvents txForecast As System.Windows.Forms.TextBox
    Private WithEvents ckPlugin As System.Windows.Forms.CheckBox
    Friend WithEvents txKalmanGain As System.Windows.Forms.TextBox
    Friend WithEvents Label8 As System.Windows.Forms.Label
End Class
