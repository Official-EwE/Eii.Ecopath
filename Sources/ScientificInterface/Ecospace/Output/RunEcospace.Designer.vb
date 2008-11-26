Imports WeifenLuo.WinFormsUI.Docking

Namespace Ecospace

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class RunEcospace
        Inherits frmEwE


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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(RunEcospace))
            Me.m_btnRun = New System.Windows.Forms.Button
            Me.m_cbDisplayGroup = New System.Windows.Forms.ComboBox
            Me.m_rbShowSingle = New System.Windows.Forms.RadioButton
            Me.m_rbShowNonHidden = New System.Windows.Forms.RadioButton
            Me.m_rbShowAll = New System.Windows.Forms.RadioButton
            Me.m_cbOverlay = New System.Windows.Forms.CheckBox
            Me.m_rbDisplayCoverB = New System.Windows.Forms.RadioButton
            Me.m_rbDisplayContaminantC = New System.Windows.Forms.RadioButton
            Me.m_rbDisplayFishingEffort = New System.Windows.Forms.RadioButton
            Me.m_rbDisplayRelBiomass = New System.Windows.Forms.RadioButton
            Me.m_btnStop = New System.Windows.Forms.Button
            Me.m_pbSmallPlot = New System.Windows.Forms.PictureBox
            Me.m_pbMap = New System.Windows.Forms.PictureBox
            Me.m_pbColors = New System.Windows.Forms.PictureBox
            Me.m_lblHigh = New System.Windows.Forms.Label
            Me.m_lblLow = New System.Windows.Forms.Label
            Me.m_lbPlotTime = New System.Windows.Forms.Label
            Me.m_lblLargePoolName = New System.Windows.Forms.Label
            Me.m_pbLargePlot = New System.Windows.Forms.PictureBox
            Me.m_lblPoolName = New System.Windows.Forms.Label
            Me.m_scMain = New System.Windows.Forms.SplitContainer
            Me.m_tlpRun = New System.Windows.Forms.TableLayoutPanel
            Me.m_tcOutputs = New System.Windows.Forms.TabControl
            Me.m_tabSmallMultiples = New System.Windows.Forms.TabPage
            Me.m_tabPlot = New System.Windows.Forms.TabPage
            Me.m_lblDist = New System.Windows.Forms.Label
            Me.m_lblDispOpt = New System.Windows.Forms.Label
            CType(Me.m_pbSmallPlot, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.m_pbMap, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.m_pbColors, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.m_pbLargePlot, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.m_scMain.Panel1.SuspendLayout()
            Me.m_scMain.Panel2.SuspendLayout()
            Me.m_scMain.SuspendLayout()
            Me.m_tlpRun.SuspendLayout()
            Me.m_tcOutputs.SuspendLayout()
            Me.m_tabSmallMultiples.SuspendLayout()
            Me.m_tabPlot.SuspendLayout()
            Me.SuspendLayout()
            '
            'm_btnRun
            '
            resources.ApplyResources(Me.m_btnRun, "m_btnRun")
            Me.m_btnRun.Name = "m_btnRun"
            Me.m_btnRun.UseVisualStyleBackColor = True
            '
            'm_cbDisplayGroup
            '
            resources.ApplyResources(Me.m_cbDisplayGroup, "m_cbDisplayGroup")
            Me.m_cbDisplayGroup.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.m_cbDisplayGroup.FormattingEnabled = True
            Me.m_cbDisplayGroup.Name = "m_cbDisplayGroup"
            '
            'm_rbShowSingle
            '
            resources.ApplyResources(Me.m_rbShowSingle, "m_rbShowSingle")
            Me.m_rbShowSingle.Name = "m_rbShowSingle"
            Me.m_rbShowSingle.UseVisualStyleBackColor = True
            '
            'm_rbShowNonHidden
            '
            resources.ApplyResources(Me.m_rbShowNonHidden, "m_rbShowNonHidden")
            Me.m_rbShowNonHidden.Name = "m_rbShowNonHidden"
            Me.m_rbShowNonHidden.UseVisualStyleBackColor = True
            '
            'm_rbShowAll
            '
            resources.ApplyResources(Me.m_rbShowAll, "m_rbShowAll")
            Me.m_rbShowAll.Checked = True
            Me.m_rbShowAll.Name = "m_rbShowAll"
            Me.m_rbShowAll.TabStop = True
            Me.m_rbShowAll.UseVisualStyleBackColor = True
            '
            'm_cbOverlay
            '
            resources.ApplyResources(Me.m_cbOverlay, "m_cbOverlay")
            Me.m_cbOverlay.Name = "m_cbOverlay"
            Me.m_cbOverlay.UseVisualStyleBackColor = True
            '
            'm_rbDisplayCoverB
            '
            resources.ApplyResources(Me.m_rbDisplayCoverB, "m_rbDisplayCoverB")
            Me.m_rbDisplayCoverB.Name = "m_rbDisplayCoverB"
            Me.m_rbDisplayCoverB.UseVisualStyleBackColor = True
            '
            'm_rbDisplayContaminantC
            '
            resources.ApplyResources(Me.m_rbDisplayContaminantC, "m_rbDisplayContaminantC")
            Me.m_rbDisplayContaminantC.Name = "m_rbDisplayContaminantC"
            Me.m_rbDisplayContaminantC.UseVisualStyleBackColor = True
            '
            'm_rbDisplayFishingEffort
            '
            resources.ApplyResources(Me.m_rbDisplayFishingEffort, "m_rbDisplayFishingEffort")
            Me.m_rbDisplayFishingEffort.Name = "m_rbDisplayFishingEffort"
            Me.m_rbDisplayFishingEffort.UseVisualStyleBackColor = True
            '
            'm_rbDisplayRelBiomass
            '
            resources.ApplyResources(Me.m_rbDisplayRelBiomass, "m_rbDisplayRelBiomass")
            Me.m_rbDisplayRelBiomass.Checked = True
            Me.m_rbDisplayRelBiomass.Name = "m_rbDisplayRelBiomass"
            Me.m_rbDisplayRelBiomass.TabStop = True
            Me.m_rbDisplayRelBiomass.UseVisualStyleBackColor = True
            '
            'm_btnStop
            '
            resources.ApplyResources(Me.m_btnStop, "m_btnStop")
            Me.m_btnStop.Name = "m_btnStop"
            Me.m_btnStop.UseVisualStyleBackColor = True
            '
            'm_pbSmallPlot
            '
            resources.ApplyResources(Me.m_pbSmallPlot, "m_pbSmallPlot")
            Me.m_pbSmallPlot.BackColor = System.Drawing.Color.White
            Me.m_pbSmallPlot.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
            Me.m_pbSmallPlot.Name = "m_pbSmallPlot"
            Me.m_pbSmallPlot.TabStop = False
            '
            'm_pbMap
            '
            resources.ApplyResources(Me.m_pbMap, "m_pbMap")
            Me.m_pbMap.BackColor = System.Drawing.SystemColors.ControlDark
            Me.m_pbMap.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
            Me.m_pbMap.Name = "m_pbMap"
            Me.m_pbMap.TabStop = False
            '
            'm_pbColors
            '
            resources.ApplyResources(Me.m_pbColors, "m_pbColors")
            Me.m_pbColors.BackColor = System.Drawing.Color.White
            Me.m_pbColors.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
            Me.m_pbColors.Name = "m_pbColors"
            Me.m_pbColors.TabStop = False
            '
            'm_lblHigh
            '
            resources.ApplyResources(Me.m_lblHigh, "m_lblHigh")
            Me.m_lblHigh.ForeColor = System.Drawing.Color.Red
            Me.m_lblHigh.Name = "m_lblHigh"
            '
            'm_lblLow
            '
            resources.ApplyResources(Me.m_lblLow, "m_lblLow")
            Me.m_lblLow.ForeColor = System.Drawing.Color.Blue
            Me.m_lblLow.Name = "m_lblLow"
            '
            'm_lbPlotTime
            '
            resources.ApplyResources(Me.m_lbPlotTime, "m_lbPlotTime")
            Me.m_lbPlotTime.Name = "m_lbPlotTime"
            '
            'm_lblLargePoolName
            '
            resources.ApplyResources(Me.m_lblLargePoolName, "m_lblLargePoolName")
            Me.m_lblLargePoolName.BackColor = System.Drawing.Color.White
            Me.m_lblLargePoolName.Name = "m_lblLargePoolName"
            '
            'm_pbLargePlot
            '
            resources.ApplyResources(Me.m_pbLargePlot, "m_pbLargePlot")
            Me.m_pbLargePlot.BackColor = System.Drawing.Color.White
            Me.m_pbLargePlot.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
            Me.m_pbLargePlot.Name = "m_pbLargePlot"
            Me.m_pbLargePlot.TabStop = False
            '
            'm_lblPoolName
            '
            resources.ApplyResources(Me.m_lblPoolName, "m_lblPoolName")
            Me.m_lblPoolName.BackColor = System.Drawing.Color.White
            Me.m_lblPoolName.Name = "m_lblPoolName"
            '
            'm_scMain
            '
            Me.m_scMain.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            resources.ApplyResources(Me.m_scMain, "m_scMain")
            Me.m_scMain.Name = "m_scMain"
            '
            'm_scMain.Panel1
            '
            Me.m_scMain.Panel1.Controls.Add(Me.m_cbDisplayGroup)
            Me.m_scMain.Panel1.Controls.Add(Me.m_lblDispOpt)
            Me.m_scMain.Panel1.Controls.Add(Me.m_rbShowSingle)
            Me.m_scMain.Panel1.Controls.Add(Me.m_rbDisplayCoverB)
            Me.m_scMain.Panel1.Controls.Add(Me.m_rbShowNonHidden)
            Me.m_scMain.Panel1.Controls.Add(Me.m_lblDist)
            Me.m_scMain.Panel1.Controls.Add(Me.m_rbShowAll)
            Me.m_scMain.Panel1.Controls.Add(Me.m_rbDisplayContaminantC)
            Me.m_scMain.Panel1.Controls.Add(Me.m_tlpRun)
            Me.m_scMain.Panel1.Controls.Add(Me.m_rbDisplayFishingEffort)
            Me.m_scMain.Panel1.Controls.Add(Me.m_lblPoolName)
            Me.m_scMain.Panel1.Controls.Add(Me.m_rbDisplayRelBiomass)
            Me.m_scMain.Panel1.Controls.Add(Me.m_cbOverlay)
            Me.m_scMain.Panel1.Controls.Add(Me.m_lbPlotTime)
            Me.m_scMain.Panel1.Controls.Add(Me.m_pbSmallPlot)
            '
            'm_scMain.Panel2
            '
            Me.m_scMain.Panel2.Controls.Add(Me.m_tcOutputs)
            '
            'm_tlpRun
            '
            resources.ApplyResources(Me.m_tlpRun, "m_tlpRun")
            Me.m_tlpRun.Controls.Add(Me.m_btnRun, 0, 0)
            Me.m_tlpRun.Controls.Add(Me.m_btnStop, 1, 0)
            Me.m_tlpRun.Name = "m_tlpRun"
            '
            'm_tcOutputs
            '
            resources.ApplyResources(Me.m_tcOutputs, "m_tcOutputs")
            Me.m_tcOutputs.Controls.Add(Me.m_tabSmallMultiples)
            Me.m_tcOutputs.Controls.Add(Me.m_tabPlot)
            Me.m_tcOutputs.Name = "m_tcOutputs"
            Me.m_tcOutputs.SelectedIndex = 0
            '
            'm_tabSmallMultiples
            '
            Me.m_tabSmallMultiples.Controls.Add(Me.m_pbMap)
            Me.m_tabSmallMultiples.Controls.Add(Me.m_pbColors)
            Me.m_tabSmallMultiples.Controls.Add(Me.m_lblLow)
            Me.m_tabSmallMultiples.Controls.Add(Me.m_lblHigh)
            resources.ApplyResources(Me.m_tabSmallMultiples, "m_tabSmallMultiples")
            Me.m_tabSmallMultiples.Name = "m_tabSmallMultiples"
            Me.m_tabSmallMultiples.UseVisualStyleBackColor = True
            '
            'm_tabPlot
            '
            Me.m_tabPlot.Controls.Add(Me.m_lblLargePoolName)
            Me.m_tabPlot.Controls.Add(Me.m_pbLargePlot)
            resources.ApplyResources(Me.m_tabPlot, "m_tabPlot")
            Me.m_tabPlot.Name = "m_tabPlot"
            Me.m_tabPlot.UseVisualStyleBackColor = True
            '
            'm_lblDist
            '
            resources.ApplyResources(Me.m_lblDist, "m_lblDist")
            Me.m_lblDist.BackColor = System.Drawing.SystemColors.ButtonShadow
            Me.m_lblDist.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
            Me.m_lblDist.Name = "m_lblDist"
            '
            'm_lblDispOpt
            '
            resources.ApplyResources(Me.m_lblDispOpt, "m_lblDispOpt")
            Me.m_lblDispOpt.BackColor = System.Drawing.SystemColors.ButtonShadow
            Me.m_lblDispOpt.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
            Me.m_lblDispOpt.Name = "m_lblDispOpt"
            '
            'RunEcospace
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.m_scMain)
            Me.Name = "RunEcospace"
            CType(Me.m_pbSmallPlot, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.m_pbMap, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.m_pbColors, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.m_pbLargePlot, System.ComponentModel.ISupportInitialize).EndInit()
            Me.m_scMain.Panel1.ResumeLayout(False)
            Me.m_scMain.Panel1.PerformLayout()
            Me.m_scMain.Panel2.ResumeLayout(False)
            Me.m_scMain.ResumeLayout(False)
            Me.m_tlpRun.ResumeLayout(False)
            Me.m_tcOutputs.ResumeLayout(False)
            Me.m_tabSmallMultiples.ResumeLayout(False)
            Me.m_tabPlot.ResumeLayout(False)
            Me.m_tabPlot.PerformLayout()
            Me.ResumeLayout(False)

        End Sub
        Friend WithEvents m_btnRun As System.Windows.Forms.Button
        Friend WithEvents m_cbDisplayGroup As System.Windows.Forms.ComboBox
        Friend WithEvents m_rbShowSingle As System.Windows.Forms.RadioButton
        Friend WithEvents m_rbShowNonHidden As System.Windows.Forms.RadioButton
        Friend WithEvents m_rbShowAll As System.Windows.Forms.RadioButton
        Friend WithEvents m_rbDisplayFishingEffort As System.Windows.Forms.RadioButton
        Friend WithEvents m_rbDisplayRelBiomass As System.Windows.Forms.RadioButton
        Friend WithEvents m_btnStop As System.Windows.Forms.Button
        Friend WithEvents m_pbSmallPlot As System.Windows.Forms.PictureBox
        Friend WithEvents m_pbMap As System.Windows.Forms.PictureBox
        Friend WithEvents m_pbColors As System.Windows.Forms.PictureBox
        Friend WithEvents m_lblHigh As System.Windows.Forms.Label
        Friend WithEvents m_lblLow As System.Windows.Forms.Label
        Friend WithEvents m_lbPlotTime As System.Windows.Forms.Label
        Friend WithEvents m_pbLargePlot As System.Windows.Forms.PictureBox
        Friend WithEvents m_lblPoolName As System.Windows.Forms.Label
        Friend WithEvents m_lblLargePoolName As System.Windows.Forms.Label
        Friend WithEvents m_cbOverlay As System.Windows.Forms.CheckBox
        Friend WithEvents m_scMain As System.Windows.Forms.SplitContainer
        Friend WithEvents m_tcOutputs As System.Windows.Forms.TabControl
        Friend WithEvents m_tabSmallMultiples As System.Windows.Forms.TabPage
        Friend WithEvents m_tabPlot As System.Windows.Forms.TabPage
        Friend WithEvents m_rbDisplayContaminantC As System.Windows.Forms.RadioButton
        Friend WithEvents m_rbDisplayCoverB As System.Windows.Forms.RadioButton
        Friend WithEvents m_tlpRun As System.Windows.Forms.TableLayoutPanel
        Friend WithEvents m_lblDispOpt As System.Windows.Forms.Label
        Friend WithEvents m_lblDist As System.Windows.Forms.Label

 
    End Class

End Namespace

