Imports ScientificInterfaceShared.Forms
Imports ScientificInterfaceShared.Controls

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
        Me.m_pnlRegOpt = New System.Windows.Forms.Panel
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
        Me.m_hdrEffortRegOptions = New cEwEHeaderLabel
        Me.m_pnlRunOpt = New System.Windows.Forms.Panel
        Me.txKalmanGain = New System.Windows.Forms.TextBox
        Me.m_lblKalmanGain = New System.Windows.Forms.Label
        Me.m_ckPlugin = New System.Windows.Forms.CheckBox
        Me.m_hdrRunOptions = New cEwEHeaderLabel
        Me.m_pnlRegOpt.SuspendLayout()
        Me.pnlUseReg.SuspendLayout()
        Me.pnlFTracking.SuspendLayout()
        Me.m_pnlRunOpt.SuspendLayout()
        Me.SuspendLayout()
        '
        'm_pnlRegOpt
        '
        resources.ApplyResources(Me.m_pnlRegOpt, "m_pnlRegOpt")
        Me.m_pnlRegOpt.Controls.Add(Me.pnlUseReg)
        Me.m_pnlRegOpt.Controls.Add(Me.pnlFTracking)
        Me.m_pnlRegOpt.Controls.Add(Me.rbUseRegs)
        Me.m_pnlRegOpt.Controls.Add(Me.rbNoRegs)
        Me.m_pnlRegOpt.Controls.Add(Me.m_hdrEffortRegOptions)
        Me.m_pnlRegOpt.Name = "m_pnlRegOpt"
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
        'm_hdrEffortRegOptions
        '
        Me.m_hdrEffortRegOptions.BackColor = System.Drawing.SystemColors.ButtonShadow
        resources.ApplyResources(Me.m_hdrEffortRegOptions, "m_hdrEffortRegOptions")
        Me.m_hdrEffortRegOptions.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.m_hdrEffortRegOptions.Name = "m_hdrEffortRegOptions"
        '
        'm_pnlRunOpt
        '
        resources.ApplyResources(Me.m_pnlRunOpt, "m_pnlRunOpt")
        Me.m_pnlRunOpt.Controls.Add(Me.txKalmanGain)
        Me.m_pnlRunOpt.Controls.Add(Me.m_lblKalmanGain)
        Me.m_pnlRunOpt.Controls.Add(Me.m_ckPlugin)
        Me.m_pnlRunOpt.Controls.Add(Me.m_hdrRunOptions)
        Me.m_pnlRunOpt.Name = "m_pnlRunOpt"
        '
        'txKalmanGain
        '
        resources.ApplyResources(Me.txKalmanGain, "txKalmanGain")
        Me.txKalmanGain.Name = "txKalmanGain"
        '
        'm_lblKalmanGain
        '
        resources.ApplyResources(Me.m_lblKalmanGain, "m_lblKalmanGain")
        Me.m_lblKalmanGain.Name = "m_lblKalmanGain"
        '
        'm_ckPlugin
        '
        resources.ApplyResources(Me.m_ckPlugin, "m_ckPlugin")
        Me.m_ckPlugin.Name = "m_ckPlugin"
        Me.m_ckPlugin.UseVisualStyleBackColor = True
        '
        'm_hdrRunOptions
        '
        Me.m_hdrRunOptions.BackColor = System.Drawing.SystemColors.ButtonShadow
        resources.ApplyResources(Me.m_hdrRunOptions, "m_hdrRunOptions")
        Me.m_hdrRunOptions.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.m_hdrRunOptions.Name = "m_hdrRunOptions"
        '
        'frmOptions
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.m_pnlRunOpt)
        Me.Controls.Add(Me.m_pnlRegOpt)
        Me.Name = "frmOptions"
        Me.m_pnlRegOpt.ResumeLayout(False)
        Me.m_pnlRegOpt.PerformLayout()
        Me.pnlUseReg.ResumeLayout(False)
        Me.pnlUseReg.PerformLayout()
        Me.pnlFTracking.ResumeLayout(False)
        Me.pnlFTracking.PerformLayout()
        Me.m_pnlRunOpt.ResumeLayout(False)
        Me.m_pnlRunOpt.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Private WithEvents rbUseRegs As System.Windows.Forms.RadioButton
    Private WithEvents rbEffortPredicted As System.Windows.Forms.RadioButton
    Private WithEvents rbNoRegs As System.Windows.Forms.RadioButton
    Private WithEvents m_hdrEffortRegOptions As cEwEHeaderLabel
    Private WithEvents pnlUseReg As System.Windows.Forms.Panel
    Private WithEvents pnlFTracking As System.Windows.Forms.Panel
    Private WithEvents txSBPower As System.Windows.Forms.TextBox
    Private WithEvents Label6 As System.Windows.Forms.Label
    Private WithEvents rbExact As System.Windows.Forms.RadioButton
    Private WithEvents rbDirectExp As System.Windows.Forms.RadioButton
    Private WithEvents rbCatchEstBio As System.Windows.Forms.RadioButton
    Private WithEvents rbEffortEcosim As System.Windows.Forms.RadioButton
    Private WithEvents rbEffortNoCap As System.Windows.Forms.RadioButton
    Private WithEvents txKalmanGain As System.Windows.Forms.TextBox
    Private WithEvents m_lblKalmanGain As System.Windows.Forms.Label
    Private WithEvents m_ckPlugin As System.Windows.Forms.CheckBox
    Private WithEvents m_hdrRunOptions As cEwEHeaderLabel
    Private WithEvents m_pnlRegOpt As System.Windows.Forms.Panel
    Private WithEvents m_pnlRunOpt As System.Windows.Forms.Panel
End Class
