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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmOptions))
        Me.pnlRegOpt = New System.Windows.Forms.Panel
        Me.pnlUseReg = New System.Windows.Forms.Panel
        Me.rbEffortEcosim = New System.Windows.Forms.RadioButton
        Me.rbEffortNoCap = New System.Windows.Forms.RadioButton
        Me.rbEffortPredicted = New System.Windows.Forms.RadioButton
        Me.pnlFTracking = New System.Windows.Forms.Panel
        Me.txSBPower = New System.Windows.Forms.TextBox
        Me.Label6 = New System.Windows.Forms.Label
        Me.rbExact = New System.Windows.Forms.RadioButton
        Me.rbDirectExp = New System.Windows.Forms.RadioButton
        Me.rbCatchEstBio = New System.Windows.Forms.RadioButton
        Me.rbUseRegs = New System.Windows.Forms.RadioButton
        Me.rbNoRegs = New System.Windows.Forms.RadioButton
        Me.Label7 = New System.Windows.Forms.Label
        Me.pnlRunOpt = New System.Windows.Forms.Panel
        Me.txKalmanGain = New System.Windows.Forms.TextBox
        Me.Label8 = New System.Windows.Forms.Label
        Me.ckPlugin = New System.Windows.Forms.CheckBox
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
        Me.pnlRegOpt.Controls.Add(Me.rbUseRegs)
        Me.pnlRegOpt.Controls.Add(Me.rbNoRegs)
        Me.pnlRegOpt.Controls.Add(Me.Label7)
        resources.ApplyResources(Me.pnlRegOpt, "pnlRegOpt")
        Me.pnlRegOpt.Name = "pnlRegOpt"
        '
        'pnlUseReg
        '
        Me.pnlUseReg.Controls.Add(Me.rbEffortEcosim)
        Me.pnlUseReg.Controls.Add(Me.rbEffortNoCap)
        Me.pnlUseReg.Controls.Add(Me.rbEffortPredicted)
        resources.ApplyResources(Me.pnlUseReg, "pnlUseReg")
        Me.pnlUseReg.Name = "pnlUseReg"
        '
        'rbEffortEcosim
        '
        resources.ApplyResources(Me.rbEffortEcosim, "rbEffortEcosim")
        Me.rbEffortEcosim.Name = "rbEffortEcosim"
        Me.rbEffortEcosim.UseVisualStyleBackColor = True
        '
        'rbEffortNoCap
        '
        resources.ApplyResources(Me.rbEffortNoCap, "rbEffortNoCap")
        Me.rbEffortNoCap.Checked = True
        Me.rbEffortNoCap.Name = "rbEffortNoCap"
        Me.rbEffortNoCap.TabStop = True
        Me.rbEffortNoCap.UseVisualStyleBackColor = True
        '
        'rbEffortPredicted
        '
        resources.ApplyResources(Me.rbEffortPredicted, "rbEffortPredicted")
        Me.rbEffortPredicted.Name = "rbEffortPredicted"
        Me.rbEffortPredicted.UseVisualStyleBackColor = True
        '
        'pnlFTracking
        '
        Me.pnlFTracking.Controls.Add(Me.txSBPower)
        Me.pnlFTracking.Controls.Add(Me.Label6)
        Me.pnlFTracking.Controls.Add(Me.rbExact)
        Me.pnlFTracking.Controls.Add(Me.rbDirectExp)
        Me.pnlFTracking.Controls.Add(Me.rbCatchEstBio)
        resources.ApplyResources(Me.pnlFTracking, "pnlFTracking")
        Me.pnlFTracking.Name = "pnlFTracking"
        '
        'txSBPower
        '
        resources.ApplyResources(Me.txSBPower, "txSBPower")
        Me.txSBPower.Name = "txSBPower"
        '
        'Label6
        '
        resources.ApplyResources(Me.Label6, "Label6")
        Me.Label6.Name = "Label6"
        '
        'rbExact
        '
        resources.ApplyResources(Me.rbExact, "rbExact")
        Me.rbExact.Name = "rbExact"
        Me.rbExact.TabStop = True
        Me.rbExact.UseVisualStyleBackColor = True
        '
        'rbDirectExp
        '
        resources.ApplyResources(Me.rbDirectExp, "rbDirectExp")
        Me.rbDirectExp.Name = "rbDirectExp"
        Me.rbDirectExp.UseVisualStyleBackColor = True
        '
        'rbCatchEstBio
        '
        resources.ApplyResources(Me.rbCatchEstBio, "rbCatchEstBio")
        Me.rbCatchEstBio.Checked = True
        Me.rbCatchEstBio.Name = "rbCatchEstBio"
        Me.rbCatchEstBio.TabStop = True
        Me.rbCatchEstBio.UseVisualStyleBackColor = True
        '
        'rbUseRegs
        '
        resources.ApplyResources(Me.rbUseRegs, "rbUseRegs")
        Me.rbUseRegs.Checked = True
        Me.rbUseRegs.Name = "rbUseRegs"
        Me.rbUseRegs.TabStop = True
        Me.rbUseRegs.UseVisualStyleBackColor = True
        '
        'rbNoRegs
        '
        resources.ApplyResources(Me.rbNoRegs, "rbNoRegs")
        Me.rbNoRegs.Name = "rbNoRegs"
        Me.rbNoRegs.UseVisualStyleBackColor = True
        '
        'Label7
        '
        Me.Label7.BackColor = System.Drawing.SystemColors.ButtonShadow
        resources.ApplyResources(Me.Label7, "Label7")
        Me.Label7.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.Label7.Name = "Label7"
        '
        'pnlRunOpt
        '
        Me.pnlRunOpt.Controls.Add(Me.txKalmanGain)
        Me.pnlRunOpt.Controls.Add(Me.Label8)
        Me.pnlRunOpt.Controls.Add(Me.ckPlugin)
        Me.pnlRunOpt.Controls.Add(Me.Label2)
        resources.ApplyResources(Me.pnlRunOpt, "pnlRunOpt")
        Me.pnlRunOpt.Name = "pnlRunOpt"
        '
        'txKalmanGain
        '
        resources.ApplyResources(Me.txKalmanGain, "txKalmanGain")
        Me.txKalmanGain.Name = "txKalmanGain"
        '
        'Label8
        '
        resources.ApplyResources(Me.Label8, "Label8")
        Me.Label8.Name = "Label8"
        '
        'ckPlugin
        '
        resources.ApplyResources(Me.ckPlugin, "ckPlugin")
        Me.ckPlugin.Name = "ckPlugin"
        Me.ckPlugin.UseVisualStyleBackColor = True
        '
        'Label2
        '
        Me.Label2.BackColor = System.Drawing.SystemColors.ButtonShadow
        resources.ApplyResources(Me.Label2, "Label2")
        Me.Label2.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.Label2.Name = "Label2"
        '
        'frmOptions
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.pnlRunOpt)
        Me.Controls.Add(Me.pnlRegOpt)
        Me.Name = "frmOptions"
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
    Friend WithEvents rbUseRegs As System.Windows.Forms.RadioButton
    Friend WithEvents rbEffortPredicted As System.Windows.Forms.RadioButton
    Friend WithEvents rbNoRegs As System.Windows.Forms.RadioButton
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents pnlUseReg As System.Windows.Forms.Panel
    Friend WithEvents pnlFTracking As System.Windows.Forms.Panel
    Private WithEvents txSBPower As System.Windows.Forms.TextBox
    Private WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents rbExact As System.Windows.Forms.RadioButton
    Private WithEvents rbDirectExp As System.Windows.Forms.RadioButton
    Private WithEvents rbCatchEstBio As System.Windows.Forms.RadioButton
    Friend WithEvents rbEffortEcosim As System.Windows.Forms.RadioButton
    Friend WithEvents rbEffortNoCap As System.Windows.Forms.RadioButton
    Friend WithEvents pnlRunOpt As System.Windows.Forms.Panel
    Friend WithEvents txKalmanGain As System.Windows.Forms.TextBox
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Private WithEvents ckPlugin As System.Windows.Forms.CheckBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
End Class
