Imports WeifenLuo.WinFormsUI.Docking
Imports ZedGraph

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
            Me.components = New System.ComponentModel.Container
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(RunEcospace))
            Me.m_btnRun = New System.Windows.Forms.Button
            Me.m_cbDisplayGroup = New System.Windows.Forms.ComboBox
            Me.m_rbShowSingle = New System.Windows.Forms.RadioButton
            Me.m_rbShowNonHidden = New System.Windows.Forms.RadioButton
            Me.m_rbShowAll = New System.Windows.Forms.RadioButton
            Me.m_cbOverlay = New System.Windows.Forms.CheckBox
            Me.m_btnStop = New System.Windows.Forms.Button
            Me.m_pbMap = New System.Windows.Forms.PictureBox
            Me.m_pbColors = New System.Windows.Forms.PictureBox
            Me.m_lblHigh = New System.Windows.Forms.Label
            Me.m_lblLow = New System.Windows.Forms.Label
            Me.m_scMain = New System.Windows.Forms.SplitContainer
            Me.m_plDistribution = New System.Windows.Forms.Panel
            Me.m_rbDisplayRelBiomass = New System.Windows.Forms.RadioButton
            Me.m_rbDisplayFishingEffort = New System.Windows.Forms.RadioButton
            Me.m_rbDisplayContaminantC = New System.Windows.Forms.RadioButton
            Me.m_lblDist = New System.Windows.Forms.Label
            Me.m_rbDisplayCoverB = New System.Windows.Forms.RadioButton
            Me.m_plDisplayOptions = New System.Windows.Forms.Panel
            Me.m_btnDisplayGroups = New System.Windows.Forms.Button
            Me.m_lblDispOpt = New System.Windows.Forms.Label
            Me.m_cbMPA = New System.Windows.Forms.CheckBox
            Me.m_tlpRun = New System.Windows.Forms.TableLayoutPanel
            Me.m_tcOutputs = New System.Windows.Forms.TabControl
            Me.m_tabSmallMultiples = New System.Windows.Forms.TabPage
            Me.m_tabPlot = New System.Windows.Forms.TabPage
            Me.m_zgPlotLarge = New ZedGraph.ZedGraphControl
            CType(Me.m_pbMap, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.m_pbColors, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.m_scMain.Panel1.SuspendLayout()
            Me.m_scMain.Panel2.SuspendLayout()
            Me.m_scMain.SuspendLayout()
            Me.m_plDistribution.SuspendLayout()
            Me.m_plDisplayOptions.SuspendLayout()
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
            'm_btnStop
            '
            resources.ApplyResources(Me.m_btnStop, "m_btnStop")
            Me.m_btnStop.Name = "m_btnStop"
            Me.m_btnStop.UseVisualStyleBackColor = True
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
            'm_scMain
            '
            Me.m_scMain.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            resources.ApplyResources(Me.m_scMain, "m_scMain")
            Me.m_scMain.Name = "m_scMain"
            '
            'm_scMain.Panel1
            '
            Me.m_scMain.Panel1.Controls.Add(Me.m_plDistribution)
            Me.m_scMain.Panel1.Controls.Add(Me.m_plDisplayOptions)
            Me.m_scMain.Panel1.Controls.Add(Me.m_tlpRun)
            '
            'm_scMain.Panel2
            '
            Me.m_scMain.Panel2.Controls.Add(Me.m_tcOutputs)
            '
            'm_plDistribution
            '
            resources.ApplyResources(Me.m_plDistribution, "m_plDistribution")
            Me.m_plDistribution.Controls.Add(Me.m_rbDisplayRelBiomass)
            Me.m_plDistribution.Controls.Add(Me.m_rbDisplayFishingEffort)
            Me.m_plDistribution.Controls.Add(Me.m_rbDisplayContaminantC)
            Me.m_plDistribution.Controls.Add(Me.m_lblDist)
            Me.m_plDistribution.Controls.Add(Me.m_rbDisplayCoverB)
            Me.m_plDistribution.Name = "m_plDistribution"
            '
            'm_rbDisplayRelBiomass
            '
            resources.ApplyResources(Me.m_rbDisplayRelBiomass, "m_rbDisplayRelBiomass")
            Me.m_rbDisplayRelBiomass.Checked = True
            Me.m_rbDisplayRelBiomass.Name = "m_rbDisplayRelBiomass"
            Me.m_rbDisplayRelBiomass.TabStop = True
            Me.m_rbDisplayRelBiomass.UseVisualStyleBackColor = True
            '
            'm_rbDisplayFishingEffort
            '
            resources.ApplyResources(Me.m_rbDisplayFishingEffort, "m_rbDisplayFishingEffort")
            Me.m_rbDisplayFishingEffort.Name = "m_rbDisplayFishingEffort"
            Me.m_rbDisplayFishingEffort.UseVisualStyleBackColor = True
            '
            'm_rbDisplayContaminantC
            '
            resources.ApplyResources(Me.m_rbDisplayContaminantC, "m_rbDisplayContaminantC")
            Me.m_rbDisplayContaminantC.Name = "m_rbDisplayContaminantC"
            Me.m_rbDisplayContaminantC.UseVisualStyleBackColor = True
            '
            'm_lblDist
            '
            resources.ApplyResources(Me.m_lblDist, "m_lblDist")
            Me.m_lblDist.BackColor = System.Drawing.SystemColors.ButtonShadow
            Me.m_lblDist.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
            Me.m_lblDist.Name = "m_lblDist"
            '
            'm_rbDisplayCoverB
            '
            resources.ApplyResources(Me.m_rbDisplayCoverB, "m_rbDisplayCoverB")
            Me.m_rbDisplayCoverB.Name = "m_rbDisplayCoverB"
            Me.m_rbDisplayCoverB.UseVisualStyleBackColor = True
            '
            'm_plDisplayOptions
            '
            resources.ApplyResources(Me.m_plDisplayOptions, "m_plDisplayOptions")
            Me.m_plDisplayOptions.Controls.Add(Me.m_btnDisplayGroups)
            Me.m_plDisplayOptions.Controls.Add(Me.m_rbShowAll)
            Me.m_plDisplayOptions.Controls.Add(Me.m_lblDispOpt)
            Me.m_plDisplayOptions.Controls.Add(Me.m_cbMPA)
            Me.m_plDisplayOptions.Controls.Add(Me.m_cbOverlay)
            Me.m_plDisplayOptions.Controls.Add(Me.m_rbShowNonHidden)
            Me.m_plDisplayOptions.Controls.Add(Me.m_rbShowSingle)
            Me.m_plDisplayOptions.Controls.Add(Me.m_cbDisplayGroup)
            Me.m_plDisplayOptions.Name = "m_plDisplayOptions"
            '
            'm_btnDisplayGroups
            '
            resources.ApplyResources(Me.m_btnDisplayGroups, "m_btnDisplayGroups")
            Me.m_btnDisplayGroups.Image = Global.ScientificInterface.My.Resources.Resources.Eye_open
            Me.m_btnDisplayGroups.Name = "m_btnDisplayGroups"
            Me.m_btnDisplayGroups.UseVisualStyleBackColor = True
            '
            'm_lblDispOpt
            '
            resources.ApplyResources(Me.m_lblDispOpt, "m_lblDispOpt")
            Me.m_lblDispOpt.BackColor = System.Drawing.SystemColors.ButtonShadow
            Me.m_lblDispOpt.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
            Me.m_lblDispOpt.Name = "m_lblDispOpt"
            '
            'm_cbMPA
            '
            resources.ApplyResources(Me.m_cbMPA, "m_cbMPA")
            Me.m_cbMPA.Name = "m_cbMPA"
            Me.m_cbMPA.UseVisualStyleBackColor = True
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
            Me.m_tabPlot.Controls.Add(Me.m_zgPlotLarge)
            resources.ApplyResources(Me.m_tabPlot, "m_tabPlot")
            Me.m_tabPlot.Name = "m_tabPlot"
            Me.m_tabPlot.UseVisualStyleBackColor = True
            '
            'm_zgPlotLarge
            '
            resources.ApplyResources(Me.m_zgPlotLarge, "m_zgPlotLarge")
            Me.m_zgPlotLarge.Name = "m_zgPlotLarge"
            Me.m_zgPlotLarge.ScrollGrace = 0
            Me.m_zgPlotLarge.ScrollMaxX = 0
            Me.m_zgPlotLarge.ScrollMaxY = 0
            Me.m_zgPlotLarge.ScrollMaxY2 = 0
            Me.m_zgPlotLarge.ScrollMinX = 0
            Me.m_zgPlotLarge.ScrollMinY = 0
            Me.m_zgPlotLarge.ScrollMinY2 = 0
            '
            'RunEcospace
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.m_scMain)
            Me.Name = "RunEcospace"
            Me.TabText = "Run Ecospace"
            CType(Me.m_pbMap, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.m_pbColors, System.ComponentModel.ISupportInitialize).EndInit()
            Me.m_scMain.Panel1.ResumeLayout(False)
            Me.m_scMain.Panel2.ResumeLayout(False)
            Me.m_scMain.ResumeLayout(False)
            Me.m_plDistribution.ResumeLayout(False)
            Me.m_plDistribution.PerformLayout()
            Me.m_plDisplayOptions.ResumeLayout(False)
            Me.m_plDisplayOptions.PerformLayout()
            Me.m_tlpRun.ResumeLayout(False)
            Me.m_tcOutputs.ResumeLayout(False)
            Me.m_tabSmallMultiples.ResumeLayout(False)
            Me.m_tabPlot.ResumeLayout(False)
            Me.ResumeLayout(False)

        End Sub
        Private WithEvents m_btnRun As System.Windows.Forms.Button
        Private WithEvents m_cbDisplayGroup As System.Windows.Forms.ComboBox
        Private WithEvents m_rbShowSingle As System.Windows.Forms.RadioButton
        Private WithEvents m_rbShowNonHidden As System.Windows.Forms.RadioButton
        Private WithEvents m_rbShowAll As System.Windows.Forms.RadioButton
        Private WithEvents m_btnStop As System.Windows.Forms.Button
        Private WithEvents m_pbMap As System.Windows.Forms.PictureBox
        Private WithEvents m_pbColors As System.Windows.Forms.PictureBox
        Private WithEvents m_lblHigh As System.Windows.Forms.Label
        Private WithEvents m_lblLow As System.Windows.Forms.Label
        Private WithEvents m_cbOverlay As System.Windows.Forms.CheckBox
        Private WithEvents m_scMain As System.Windows.Forms.SplitContainer
        Private WithEvents m_tcOutputs As System.Windows.Forms.TabControl
        Private WithEvents m_tabSmallMultiples As System.Windows.Forms.TabPage
        Private WithEvents m_tabPlot As System.Windows.Forms.TabPage
        Private WithEvents m_tlpRun As System.Windows.Forms.TableLayoutPanel
        Private WithEvents m_lblDispOpt As System.Windows.Forms.Label
        Private WithEvents m_lblDist As System.Windows.Forms.Label
        Private WithEvents m_plDisplayOptions As System.Windows.Forms.Panel
        Private WithEvents m_zgPlotLarge As ZedGraphControl
        Private WithEvents m_btnDisplayGroups As System.Windows.Forms.Button
        Private WithEvents m_plDistribution As System.Windows.Forms.Panel
        Private WithEvents m_rbDisplayRelBiomass As System.Windows.Forms.RadioButton
        Private WithEvents m_rbDisplayFishingEffort As System.Windows.Forms.RadioButton
        Private WithEvents m_rbDisplayContaminantC As System.Windows.Forms.RadioButton
        Private WithEvents m_rbDisplayCoverB As System.Windows.Forms.RadioButton
        Private WithEvents m_cbMPA As System.Windows.Forms.CheckBox

 
    End Class

End Namespace

