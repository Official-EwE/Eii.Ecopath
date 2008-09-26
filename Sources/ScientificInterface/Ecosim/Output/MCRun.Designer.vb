Imports WeifenLuo.WinFormsUI.Docking

Namespace Ecosim

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class MCRun
        Inherits DockContent

        'Form overrides dispose to clean up the component list.
        <System.Diagnostics.DebuggerNonUserCode()> _
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
            MyBase.Dispose(disposing)
        End Sub

        'Required by the Windows Form Designer
        Private components As System.ComponentModel.IContainer

        'NOTE: The following procedure is required by the Windows Form Designer
        'It can be modified using the Windows Form Designer.  
        'Do not modify it using the code editor.
        <System.Diagnostics.DebuggerStepThrough()> _
        Private Sub InitializeComponent()
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(MCRun))
            Me.lblNumTrials = New System.Windows.Forms.Label
            Me.btnRunTrials = New System.Windows.Forms.Button
            Me.btnStop = New System.Windows.Forms.Button
            Me.tcMCOutput = New System.Windows.Forms.TabControl
            Me.tbpB = New System.Windows.Forms.TabPage
            Me.tbpBP = New System.Windows.Forms.TabPage
            Me.tbpEE = New System.Windows.Forms.TabPage
            Me.tbpBA = New System.Windows.Forms.TabPage
            Me.tbpBestTrial = New System.Windows.Forms.TabPage
            Me.tbpBPlot = New System.Windows.Forms.TabPage
            Me.cbPedigree = New System.Windows.Forms.CheckBox
            Me.cbRetainEstimates = New System.Windows.Forms.CheckBox
            Me.cbRetainCurPattern = New System.Windows.Forms.CheckBox
            Me.lblTrial = New System.Windows.Forms.Label
            Me.lblERun = New System.Windows.Forms.Label
            Me.lblSS = New System.Windows.Forms.Label
            Me.lblBestSS = New System.Windows.Forms.Label
            Me.cbShowBioTraj = New System.Windows.Forms.CheckBox
            Me.prgMCTrials = New System.Windows.Forms.ProgressBar
            Me.btApply = New System.Windows.Forms.Button
            Me.gpbInput = New System.Windows.Forms.GroupBox
            Me.nudNumTrials = New System.Windows.Forms.NumericUpDown
            Me.btnTS = New System.Windows.Forms.Button
            Me.gpbOutput = New System.Windows.Forms.GroupBox
            Me.lblValueERun = New System.Windows.Forms.Label
            Me.lblValueSSBest = New System.Windows.Forms.Label
            Me.lblValueSS = New System.Windows.Forms.Label
            Me.lblValueSSOrg = New System.Windows.Forms.Label
            Me.lblValueTrial = New System.Windows.Forms.Label
            Me.lbSSOrg = New System.Windows.Forms.Label
            Me.tcMCOutput.SuspendLayout()
            Me.gpbInput.SuspendLayout()
            CType(Me.nudNumTrials, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.gpbOutput.SuspendLayout()
            Me.SuspendLayout()
            '
            'lblNumTrials
            '
            resources.ApplyResources(Me.lblNumTrials, "lblNumTrials")
            Me.lblNumTrials.Name = "lblNumTrials"
            '
            'btnRunTrials
            '
            resources.ApplyResources(Me.btnRunTrials, "btnRunTrials")
            Me.btnRunTrials.Name = "btnRunTrials"
            Me.btnRunTrials.UseVisualStyleBackColor = True
            '
            'btnStop
            '
            resources.ApplyResources(Me.btnStop, "btnStop")
            Me.btnStop.Name = "btnStop"
            Me.btnStop.UseVisualStyleBackColor = True
            '
            'tcMCOutput
            '
            resources.ApplyResources(Me.tcMCOutput, "tcMCOutput")
            Me.tcMCOutput.Controls.Add(Me.tbpB)
            Me.tcMCOutput.Controls.Add(Me.tbpBP)
            Me.tcMCOutput.Controls.Add(Me.tbpEE)
            Me.tcMCOutput.Controls.Add(Me.tbpBA)
            Me.tcMCOutput.Controls.Add(Me.tbpBestTrial)
            Me.tcMCOutput.Controls.Add(Me.tbpBPlot)
            Me.tcMCOutput.Name = "tcMCOutput"
            Me.tcMCOutput.SelectedIndex = 0
            '
            'tbpB
            '
            resources.ApplyResources(Me.tbpB, "tbpB")
            Me.tbpB.Name = "tbpB"
            Me.tbpB.UseVisualStyleBackColor = True
            '
            'tbpBP
            '
            resources.ApplyResources(Me.tbpBP, "tbpBP")
            Me.tbpBP.Name = "tbpBP"
            Me.tbpBP.UseVisualStyleBackColor = True
            '
            'tbpEE
            '
            resources.ApplyResources(Me.tbpEE, "tbpEE")
            Me.tbpEE.Name = "tbpEE"
            Me.tbpEE.UseVisualStyleBackColor = True
            '
            'tbpBA
            '
            resources.ApplyResources(Me.tbpBA, "tbpBA")
            Me.tbpBA.Name = "tbpBA"
            Me.tbpBA.UseVisualStyleBackColor = True
            '
            'tbpBestTrial
            '
            resources.ApplyResources(Me.tbpBestTrial, "tbpBestTrial")
            Me.tbpBestTrial.Name = "tbpBestTrial"
            Me.tbpBestTrial.UseVisualStyleBackColor = True
            '
            'tbpBPlot
            '
            Me.tbpBPlot.BackColor = System.Drawing.SystemColors.Control
            resources.ApplyResources(Me.tbpBPlot, "tbpBPlot")
            Me.tbpBPlot.Name = "tbpBPlot"
            '
            'cbPedigree
            '
            resources.ApplyResources(Me.cbPedigree, "cbPedigree")
            Me.cbPedigree.Name = "cbPedigree"
            Me.cbPedigree.UseVisualStyleBackColor = True
            '
            'cbRetainEstimates
            '
            resources.ApplyResources(Me.cbRetainEstimates, "cbRetainEstimates")
            Me.cbRetainEstimates.Name = "cbRetainEstimates"
            Me.cbRetainEstimates.UseVisualStyleBackColor = True
            '
            'cbRetainCurPattern
            '
            resources.ApplyResources(Me.cbRetainCurPattern, "cbRetainCurPattern")
            Me.cbRetainCurPattern.Name = "cbRetainCurPattern"
            Me.cbRetainCurPattern.UseVisualStyleBackColor = True
            '
            'lblTrial
            '
            resources.ApplyResources(Me.lblTrial, "lblTrial")
            Me.lblTrial.Name = "lblTrial"
            '
            'lblERun
            '
            resources.ApplyResources(Me.lblERun, "lblERun")
            Me.lblERun.Name = "lblERun"
            '
            'lblSS
            '
            resources.ApplyResources(Me.lblSS, "lblSS")
            Me.lblSS.Name = "lblSS"
            '
            'lblBestSS
            '
            resources.ApplyResources(Me.lblBestSS, "lblBestSS")
            Me.lblBestSS.Name = "lblBestSS"
            '
            'cbShowBioTraj
            '
            resources.ApplyResources(Me.cbShowBioTraj, "cbShowBioTraj")
            Me.cbShowBioTraj.Checked = True
            Me.cbShowBioTraj.CheckState = System.Windows.Forms.CheckState.Checked
            Me.cbShowBioTraj.Name = "cbShowBioTraj"
            Me.cbShowBioTraj.UseVisualStyleBackColor = True
            '
            'prgMCTrials
            '
            resources.ApplyResources(Me.prgMCTrials, "prgMCTrials")
            Me.prgMCTrials.Name = "prgMCTrials"
            Me.prgMCTrials.Step = 1
            Me.prgMCTrials.Style = System.Windows.Forms.ProgressBarStyle.Continuous
            '
            'btApply
            '
            resources.ApplyResources(Me.btApply, "btApply")
            Me.btApply.Name = "btApply"
            Me.btApply.UseVisualStyleBackColor = True
            '
            'gpbInput
            '
            resources.ApplyResources(Me.gpbInput, "gpbInput")
            Me.gpbInput.Controls.Add(Me.nudNumTrials)
            Me.gpbInput.Controls.Add(Me.btnTS)
            Me.gpbInput.Controls.Add(Me.lblNumTrials)
            Me.gpbInput.Controls.Add(Me.cbPedigree)
            Me.gpbInput.Controls.Add(Me.cbRetainEstimates)
            Me.gpbInput.Controls.Add(Me.cbRetainCurPattern)
            Me.gpbInput.Controls.Add(Me.cbShowBioTraj)
            Me.gpbInput.Name = "gpbInput"
            Me.gpbInput.TabStop = False
            '
            'nudNumTrials
            '
            resources.ApplyResources(Me.nudNumTrials, "nudNumTrials")
            Me.nudNumTrials.Maximum = New Decimal(New Integer() {Integer.MaxValue, 0, 0, 0})
            Me.nudNumTrials.Name = "nudNumTrials"
            '
            'btnTS
            '
            resources.ApplyResources(Me.btnTS, "btnTS")
            Me.btnTS.Name = "btnTS"
            Me.btnTS.UseVisualStyleBackColor = True
            '
            'gpbOutput
            '
            resources.ApplyResources(Me.gpbOutput, "gpbOutput")
            Me.gpbOutput.Controls.Add(Me.lblValueERun)
            Me.gpbOutput.Controls.Add(Me.lblValueSSBest)
            Me.gpbOutput.Controls.Add(Me.lblValueSS)
            Me.gpbOutput.Controls.Add(Me.lblValueSSOrg)
            Me.gpbOutput.Controls.Add(Me.lblValueTrial)
            Me.gpbOutput.Controls.Add(Me.lblBestSS)
            Me.gpbOutput.Controls.Add(Me.lblSS)
            Me.gpbOutput.Controls.Add(Me.lbSSOrg)
            Me.gpbOutput.Controls.Add(Me.lblTrial)
            Me.gpbOutput.Controls.Add(Me.lblERun)
            Me.gpbOutput.Name = "gpbOutput"
            Me.gpbOutput.TabStop = False
            '
            'lblValueERun
            '
            resources.ApplyResources(Me.lblValueERun, "lblValueERun")
            Me.lblValueERun.Name = "lblValueERun"
            '
            'lblValueSSBest
            '
            resources.ApplyResources(Me.lblValueSSBest, "lblValueSSBest")
            Me.lblValueSSBest.Name = "lblValueSSBest"
            '
            'lblValueSS
            '
            resources.ApplyResources(Me.lblValueSS, "lblValueSS")
            Me.lblValueSS.Name = "lblValueSS"
            '
            'lblValueSSOrg
            '
            resources.ApplyResources(Me.lblValueSSOrg, "lblValueSSOrg")
            Me.lblValueSSOrg.Name = "lblValueSSOrg"
            '
            'lblValueTrial
            '
            resources.ApplyResources(Me.lblValueTrial, "lblValueTrial")
            Me.lblValueTrial.Name = "lblValueTrial"
            '
            'lbSSOrg
            '
            resources.ApplyResources(Me.lbSSOrg, "lbSSOrg")
            Me.lbSSOrg.Name = "lbSSOrg"
            '
            'MCRun
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.gpbOutput)
            Me.Controls.Add(Me.gpbInput)
            Me.Controls.Add(Me.tcMCOutput)
            Me.Controls.Add(Me.btApply)
            Me.Controls.Add(Me.prgMCTrials)
            Me.Controls.Add(Me.btnStop)
            Me.Controls.Add(Me.btnRunTrials)
            Me.Name = "MCRun"
            Me.tcMCOutput.ResumeLayout(False)
            Me.gpbInput.ResumeLayout(False)
            Me.gpbInput.PerformLayout()
            CType(Me.nudNumTrials, System.ComponentModel.ISupportInitialize).EndInit()
            Me.gpbOutput.ResumeLayout(False)
            Me.gpbOutput.PerformLayout()
            Me.ResumeLayout(False)

        End Sub
        Friend WithEvents lblNumTrials As System.Windows.Forms.Label
        Friend WithEvents btnRunTrials As System.Windows.Forms.Button
        Friend WithEvents btnStop As System.Windows.Forms.Button
        Friend WithEvents cbPedigree As System.Windows.Forms.CheckBox
        Friend WithEvents cbRetainEstimates As System.Windows.Forms.CheckBox
        Friend WithEvents cbRetainCurPattern As System.Windows.Forms.CheckBox
        Friend WithEvents lblTrial As System.Windows.Forms.Label
        Friend WithEvents lblERun As System.Windows.Forms.Label
        Friend WithEvents lblSS As System.Windows.Forms.Label
        Friend WithEvents lblBestSS As System.Windows.Forms.Label
        Friend WithEvents cbShowBioTraj As System.Windows.Forms.CheckBox
        Friend WithEvents tcMCOutput As System.Windows.Forms.TabControl
        Friend WithEvents tbpB As System.Windows.Forms.TabPage
        Friend WithEvents tbpBP As System.Windows.Forms.TabPage
        Friend WithEvents tbpEE As System.Windows.Forms.TabPage
        Friend WithEvents tbpBA As System.Windows.Forms.TabPage
        Friend WithEvents tbpBestTrial As System.Windows.Forms.TabPage
        Friend WithEvents prgMCTrials As System.Windows.Forms.ProgressBar
        Friend WithEvents btApply As System.Windows.Forms.Button
        Friend WithEvents tbpBPlot As System.Windows.Forms.TabPage
        Friend WithEvents gpbInput As System.Windows.Forms.GroupBox
        Friend WithEvents btnTS As System.Windows.Forms.Button
        Friend WithEvents gpbOutput As System.Windows.Forms.GroupBox
        Friend WithEvents lbSSOrg As System.Windows.Forms.Label
        Friend WithEvents nudNumTrials As System.Windows.Forms.NumericUpDown
        Friend WithEvents lblValueERun As System.Windows.Forms.Label
        Friend WithEvents lblValueSSBest As System.Windows.Forms.Label
        Friend WithEvents lblValueSS As System.Windows.Forms.Label
        Friend WithEvents lblValueSSOrg As System.Windows.Forms.Label
        Friend WithEvents lblValueTrial As System.Windows.Forms.Label
    End Class

End Namespace

