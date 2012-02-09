Imports ScientificInterfaceShared.Forms
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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(RunEcospace))
            Me.m_btnRun = New System.Windows.Forms.Button()
            Me.m_cmbDisplayGroup = New System.Windows.Forms.ComboBox()
            Me.m_rbShowSingle = New System.Windows.Forms.RadioButton()
            Me.m_rbShowNonHidden = New System.Windows.Forms.RadioButton()
            Me.m_rbShowAll = New System.Windows.Forms.RadioButton()
            Me.m_cbOverlay = New System.Windows.Forms.CheckBox()
            Me.m_btnStop = New System.Windows.Forms.Button()
            Me.m_pbMap = New System.Windows.Forms.PictureBox()
            Me.m_pbColors = New System.Windows.Forms.PictureBox()
            Me.m_lblHigh = New System.Windows.Forms.Label()
            Me.m_lblLow = New System.Windows.Forms.Label()
            Me.m_scMain = New System.Windows.Forms.SplitContainer()
            Me.m_tlpOptions = New System.Windows.Forms.TableLayoutPanel()
            Me.m_plLabelOptions = New System.Windows.Forms.Panel()
            Me.m_cbInvertColor = New System.Windows.Forms.CheckBox()
            Me.m_cmbLabelPos = New System.Windows.Forms.ComboBox()
            Me.m_cbShowLabels = New System.Windows.Forms.CheckBox()
            Me.m_hdrLabelOptions = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
            Me.m_plDistribution = New System.Windows.Forms.Panel()
            Me.m_txFMax = New System.Windows.Forms.TextBox()
            Me.m_ckSelFleets = New System.Windows.Forms.CheckBox()
            Me.m_rbDisplayF = New System.Windows.Forms.RadioButton()
            Me.m_rbDisplayRelBiomass = New System.Windows.Forms.RadioButton()
            Me.m_rbDisplayFOverB = New System.Windows.Forms.RadioButton()
            Me.m_rbDisplayFishingEffort = New System.Windows.Forms.RadioButton()
            Me.m_rbDisplayContaminantC = New System.Windows.Forms.RadioButton()
            Me.m_hdrDist = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
            Me.m_rbDisplayCoverB = New System.Windows.Forms.RadioButton()
            Me.m_plDisplayOptions = New System.Windows.Forms.Panel()
            Me.m_btnDisplayGroups = New System.Windows.Forms.Button()
            Me.m_cbShowIBMPackets = New System.Windows.Forms.CheckBox()
            Me.m_hdrDispOpt = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
            Me.m_cbMPA = New System.Windows.Forms.CheckBox()
            Me.m_plRun = New System.Windows.Forms.Panel()
            Me.m_hdrRunning = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
            Me.m_tlpRun = New System.Windows.Forms.TableLayoutPanel()
            Me.m_btnPause = New System.Windows.Forms.Button()
            Me.m_cmbRunType = New System.Windows.Forms.ComboBox()
            Me.m_tcOutputs = New System.Windows.Forms.TabControl()
            Me.m_tabMap = New System.Windows.Forms.TabPage()
            Me.m_tabPlot = New System.Windows.Forms.TabPage()
            Me.m_zgPlotLarge = New ZedGraph.ZedGraphControl()
            Me.Label1 = New System.Windows.Forms.Label()
            CType(Me.m_pbMap, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.m_pbColors, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.m_scMain, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.m_scMain.Panel1.SuspendLayout()
            Me.m_scMain.Panel2.SuspendLayout()
            Me.m_scMain.SuspendLayout()
            Me.m_tlpOptions.SuspendLayout()
            Me.m_plLabelOptions.SuspendLayout()
            Me.m_plDistribution.SuspendLayout()
            Me.m_plDisplayOptions.SuspendLayout()
            Me.m_plRun.SuspendLayout()
            Me.m_tlpRun.SuspendLayout()
            Me.m_tcOutputs.SuspendLayout()
            Me.m_tabMap.SuspendLayout()
            Me.m_tabPlot.SuspendLayout()
            Me.SuspendLayout()
            '
            'm_btnRun
            '
            resources.ApplyResources(Me.m_btnRun, "m_btnRun")
            Me.m_btnRun.Name = "m_btnRun"
            Me.m_btnRun.UseVisualStyleBackColor = True
            '
            'm_cmbDisplayGroup
            '
            resources.ApplyResources(Me.m_cmbDisplayGroup, "m_cmbDisplayGroup")
            Me.m_cmbDisplayGroup.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.m_cmbDisplayGroup.FormattingEnabled = True
            Me.m_cmbDisplayGroup.Name = "m_cmbDisplayGroup"
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
            Me.m_scMain.FixedPanel = System.Windows.Forms.FixedPanel.Panel1
            Me.m_scMain.Name = "m_scMain"
            '
            'm_scMain.Panel1
            '
            Me.m_scMain.Panel1.Controls.Add(Me.m_tlpOptions)
            '
            'm_scMain.Panel2
            '
            Me.m_scMain.Panel2.Controls.Add(Me.m_tcOutputs)
            '
            'm_tlpOptions
            '
            resources.ApplyResources(Me.m_tlpOptions, "m_tlpOptions")
            Me.m_tlpOptions.Controls.Add(Me.m_plLabelOptions, 0, 2)
            Me.m_tlpOptions.Controls.Add(Me.m_plDistribution, 0, 0)
            Me.m_tlpOptions.Controls.Add(Me.m_plDisplayOptions, 0, 1)
            Me.m_tlpOptions.Controls.Add(Me.m_plRun, 0, 3)
            Me.m_tlpOptions.Name = "m_tlpOptions"
            '
            'm_plLabelOptions
            '
            Me.m_plLabelOptions.Controls.Add(Me.m_cbInvertColor)
            Me.m_plLabelOptions.Controls.Add(Me.m_cmbLabelPos)
            Me.m_plLabelOptions.Controls.Add(Me.m_cbShowLabels)
            Me.m_plLabelOptions.Controls.Add(Me.m_hdrLabelOptions)
            resources.ApplyResources(Me.m_plLabelOptions, "m_plLabelOptions")
            Me.m_plLabelOptions.Name = "m_plLabelOptions"
            '
            'm_cbInvertColor
            '
            resources.ApplyResources(Me.m_cbInvertColor, "m_cbInvertColor")
            Me.m_cbInvertColor.Name = "m_cbInvertColor"
            Me.m_cbInvertColor.UseVisualStyleBackColor = True
            '
            'm_cmbLabelPos
            '
            resources.ApplyResources(Me.m_cmbLabelPos, "m_cmbLabelPos")
            Me.m_cmbLabelPos.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.m_cmbLabelPos.FormattingEnabled = True
            Me.m_cmbLabelPos.Items.AddRange(New Object() {resources.GetString("m_cmbLabelPos.Items"), resources.GetString("m_cmbLabelPos.Items1"), resources.GetString("m_cmbLabelPos.Items2"), resources.GetString("m_cmbLabelPos.Items3"), resources.GetString("m_cmbLabelPos.Items4"), resources.GetString("m_cmbLabelPos.Items5"), resources.GetString("m_cmbLabelPos.Items6"), resources.GetString("m_cmbLabelPos.Items7"), resources.GetString("m_cmbLabelPos.Items8")})
            Me.m_cmbLabelPos.Name = "m_cmbLabelPos"
            '
            'm_cbShowLabels
            '
            resources.ApplyResources(Me.m_cbShowLabels, "m_cbShowLabels")
            Me.m_cbShowLabels.Checked = True
            Me.m_cbShowLabels.CheckState = System.Windows.Forms.CheckState.Checked
            Me.m_cbShowLabels.Name = "m_cbShowLabels"
            Me.m_cbShowLabels.UseVisualStyleBackColor = True
            '
            'm_hdrLabelOptions
            '
            resources.ApplyResources(Me.m_hdrLabelOptions, "m_hdrLabelOptions")
            Me.m_hdrLabelOptions.CanCollapseParent = True
            Me.m_hdrLabelOptions.CollapsedParentHeight = 0
            Me.m_hdrLabelOptions.IsCollapsed = False
            Me.m_hdrLabelOptions.Name = "m_hdrLabelOptions"
            '
            'm_plDistribution
            '
            Me.m_plDistribution.Controls.Add(Me.Label1)
            Me.m_plDistribution.Controls.Add(Me.m_txFMax)
            Me.m_plDistribution.Controls.Add(Me.m_ckSelFleets)
            Me.m_plDistribution.Controls.Add(Me.m_rbDisplayF)
            Me.m_plDistribution.Controls.Add(Me.m_rbDisplayRelBiomass)
            Me.m_plDistribution.Controls.Add(Me.m_rbDisplayFOverB)
            Me.m_plDistribution.Controls.Add(Me.m_rbDisplayFishingEffort)
            Me.m_plDistribution.Controls.Add(Me.m_rbDisplayContaminantC)
            Me.m_plDistribution.Controls.Add(Me.m_hdrDist)
            Me.m_plDistribution.Controls.Add(Me.m_rbDisplayCoverB)
            resources.ApplyResources(Me.m_plDistribution, "m_plDistribution")
            Me.m_plDistribution.Name = "m_plDistribution"
            '
            'm_txFMax
            '
            resources.ApplyResources(Me.m_txFMax, "m_txFMax")
            Me.m_txFMax.Name = "m_txFMax"
            '
            'm_ckSelFleets
            '
            resources.ApplyResources(Me.m_ckSelFleets, "m_ckSelFleets")
            Me.m_ckSelFleets.Name = "m_ckSelFleets"
            Me.m_ckSelFleets.UseVisualStyleBackColor = True
            '
            'm_rbDisplayF
            '
            resources.ApplyResources(Me.m_rbDisplayF, "m_rbDisplayF")
            Me.m_rbDisplayF.Name = "m_rbDisplayF"
            Me.m_rbDisplayF.UseVisualStyleBackColor = True
            '
            'm_rbDisplayRelBiomass
            '
            resources.ApplyResources(Me.m_rbDisplayRelBiomass, "m_rbDisplayRelBiomass")
            Me.m_rbDisplayRelBiomass.Checked = True
            Me.m_rbDisplayRelBiomass.Name = "m_rbDisplayRelBiomass"
            Me.m_rbDisplayRelBiomass.TabStop = True
            Me.m_rbDisplayRelBiomass.UseVisualStyleBackColor = True
            '
            'm_rbDisplayFOverB
            '
            resources.ApplyResources(Me.m_rbDisplayFOverB, "m_rbDisplayFOverB")
            Me.m_rbDisplayFOverB.Name = "m_rbDisplayFOverB"
            Me.m_rbDisplayFOverB.UseVisualStyleBackColor = True
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
            'm_hdrDist
            '
            resources.ApplyResources(Me.m_hdrDist, "m_hdrDist")
            Me.m_hdrDist.CanCollapseParent = False
            Me.m_hdrDist.CollapsedParentHeight = 0
            Me.m_hdrDist.IsCollapsed = False
            Me.m_hdrDist.Name = "m_hdrDist"
            '
            'm_rbDisplayCoverB
            '
            resources.ApplyResources(Me.m_rbDisplayCoverB, "m_rbDisplayCoverB")
            Me.m_rbDisplayCoverB.Name = "m_rbDisplayCoverB"
            Me.m_rbDisplayCoverB.UseVisualStyleBackColor = True
            '
            'm_plDisplayOptions
            '
            Me.m_plDisplayOptions.Controls.Add(Me.m_btnDisplayGroups)
            Me.m_plDisplayOptions.Controls.Add(Me.m_rbShowAll)
            Me.m_plDisplayOptions.Controls.Add(Me.m_cbShowIBMPackets)
            Me.m_plDisplayOptions.Controls.Add(Me.m_hdrDispOpt)
            Me.m_plDisplayOptions.Controls.Add(Me.m_cbMPA)
            Me.m_plDisplayOptions.Controls.Add(Me.m_cbOverlay)
            Me.m_plDisplayOptions.Controls.Add(Me.m_rbShowNonHidden)
            Me.m_plDisplayOptions.Controls.Add(Me.m_rbShowSingle)
            Me.m_plDisplayOptions.Controls.Add(Me.m_cmbDisplayGroup)
            resources.ApplyResources(Me.m_plDisplayOptions, "m_plDisplayOptions")
            Me.m_plDisplayOptions.Name = "m_plDisplayOptions"
            '
            'm_btnDisplayGroups
            '
            resources.ApplyResources(Me.m_btnDisplayGroups, "m_btnDisplayGroups")
            Me.m_btnDisplayGroups.Name = "m_btnDisplayGroups"
            Me.m_btnDisplayGroups.UseVisualStyleBackColor = True
            '
            'm_cbShowIBMPackets
            '
            resources.ApplyResources(Me.m_cbShowIBMPackets, "m_cbShowIBMPackets")
            Me.m_cbShowIBMPackets.Checked = True
            Me.m_cbShowIBMPackets.CheckState = System.Windows.Forms.CheckState.Checked
            Me.m_cbShowIBMPackets.Name = "m_cbShowIBMPackets"
            Me.m_cbShowIBMPackets.UseVisualStyleBackColor = True
            '
            'm_hdrDispOpt
            '
            resources.ApplyResources(Me.m_hdrDispOpt, "m_hdrDispOpt")
            Me.m_hdrDispOpt.CanCollapseParent = False
            Me.m_hdrDispOpt.CollapsedParentHeight = 0
            Me.m_hdrDispOpt.IsCollapsed = False
            Me.m_hdrDispOpt.Name = "m_hdrDispOpt"
            '
            'm_cbMPA
            '
            resources.ApplyResources(Me.m_cbMPA, "m_cbMPA")
            Me.m_cbMPA.Checked = True
            Me.m_cbMPA.CheckState = System.Windows.Forms.CheckState.Checked
            Me.m_cbMPA.Name = "m_cbMPA"
            Me.m_cbMPA.UseVisualStyleBackColor = True
            '
            'm_plRun
            '
            resources.ApplyResources(Me.m_plRun, "m_plRun")
            Me.m_plRun.Controls.Add(Me.m_hdrRunning)
            Me.m_plRun.Controls.Add(Me.m_tlpRun)
            Me.m_plRun.Name = "m_plRun"
            '
            'm_hdrRunning
            '
            resources.ApplyResources(Me.m_hdrRunning, "m_hdrRunning")
            Me.m_hdrRunning.CanCollapseParent = False
            Me.m_hdrRunning.CollapsedParentHeight = 0
            Me.m_hdrRunning.IsCollapsed = False
            Me.m_hdrRunning.Name = "m_hdrRunning"
            '
            'm_tlpRun
            '
            resources.ApplyResources(Me.m_tlpRun, "m_tlpRun")
            Me.m_tlpRun.Controls.Add(Me.m_btnRun, 0, 0)
            Me.m_tlpRun.Controls.Add(Me.m_btnPause, 0, 1)
            Me.m_tlpRun.Controls.Add(Me.m_btnStop, 1, 1)
            Me.m_tlpRun.Controls.Add(Me.m_cmbRunType, 1, 0)
            Me.m_tlpRun.Name = "m_tlpRun"
            '
            'm_btnPause
            '
            resources.ApplyResources(Me.m_btnPause, "m_btnPause")
            Me.m_btnPause.Name = "m_btnPause"
            Me.m_btnPause.UseVisualStyleBackColor = True
            '
            'm_cmbRunType
            '
            resources.ApplyResources(Me.m_cmbRunType, "m_cmbRunType")
            Me.m_cmbRunType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.m_cmbRunType.FormattingEnabled = True
            Me.m_cmbRunType.Items.AddRange(New Object() {resources.GetString("m_cmbRunType.Items"), resources.GetString("m_cmbRunType.Items1"), resources.GetString("m_cmbRunType.Items2")})
            Me.m_cmbRunType.Name = "m_cmbRunType"
            '
            'm_tcOutputs
            '
            resources.ApplyResources(Me.m_tcOutputs, "m_tcOutputs")
            Me.m_tcOutputs.Controls.Add(Me.m_tabMap)
            Me.m_tcOutputs.Controls.Add(Me.m_tabPlot)
            Me.m_tcOutputs.Name = "m_tcOutputs"
            Me.m_tcOutputs.SelectedIndex = 0
            '
            'm_tabMap
            '
            Me.m_tabMap.Controls.Add(Me.m_pbMap)
            Me.m_tabMap.Controls.Add(Me.m_pbColors)
            Me.m_tabMap.Controls.Add(Me.m_lblLow)
            Me.m_tabMap.Controls.Add(Me.m_lblHigh)
            resources.ApplyResources(Me.m_tabMap, "m_tabMap")
            Me.m_tabMap.Name = "m_tabMap"
            Me.m_tabMap.UseVisualStyleBackColor = True
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
            Me.m_zgPlotLarge.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
            resources.ApplyResources(Me.m_zgPlotLarge, "m_zgPlotLarge")
            Me.m_zgPlotLarge.Name = "m_zgPlotLarge"
            Me.m_zgPlotLarge.ScrollGrace = 0.0R
            Me.m_zgPlotLarge.ScrollMaxX = 0.0R
            Me.m_zgPlotLarge.ScrollMaxY = 0.0R
            Me.m_zgPlotLarge.ScrollMaxY2 = 0.0R
            Me.m_zgPlotLarge.ScrollMinX = 0.0R
            Me.m_zgPlotLarge.ScrollMinY = 0.0R
            Me.m_zgPlotLarge.ScrollMinY2 = 0.0R
            '
            'Label1
            '
            resources.ApplyResources(Me.Label1, "Label1")
            Me.Label1.Name = "Label1"
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
            CType(Me.m_scMain, System.ComponentModel.ISupportInitialize).EndInit()
            Me.m_scMain.ResumeLayout(False)
            Me.m_tlpOptions.ResumeLayout(False)
            Me.m_plLabelOptions.ResumeLayout(False)
            Me.m_plLabelOptions.PerformLayout()
            Me.m_plDistribution.ResumeLayout(False)
            Me.m_plDistribution.PerformLayout()
            Me.m_plDisplayOptions.ResumeLayout(False)
            Me.m_plDisplayOptions.PerformLayout()
            Me.m_plRun.ResumeLayout(False)
            Me.m_tlpRun.ResumeLayout(False)
            Me.m_tcOutputs.ResumeLayout(False)
            Me.m_tabMap.ResumeLayout(False)
            Me.m_tabPlot.ResumeLayout(False)
            Me.ResumeLayout(False)

        End Sub
        Private WithEvents m_btnRun As System.Windows.Forms.Button
        Private WithEvents m_cmbDisplayGroup As System.Windows.Forms.ComboBox
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
        Private WithEvents m_tabMap As System.Windows.Forms.TabPage
        Private WithEvents m_tabPlot As System.Windows.Forms.TabPage
        Private WithEvents m_tlpRun As System.Windows.Forms.TableLayoutPanel
        Private WithEvents m_hdrDispOpt As cEwEHeaderLabel
        Private WithEvents m_hdrDist As cEwEHeaderLabel
        Private WithEvents m_plDisplayOptions As System.Windows.Forms.Panel
        Private WithEvents m_zgPlotLarge As ZedGraphControl
        Private WithEvents m_btnDisplayGroups As System.Windows.Forms.Button
        Private WithEvents m_plDistribution As System.Windows.Forms.Panel
        Private WithEvents m_rbDisplayRelBiomass As System.Windows.Forms.RadioButton
        Private WithEvents m_rbDisplayFishingEffort As System.Windows.Forms.RadioButton
        Private WithEvents m_rbDisplayContaminantC As System.Windows.Forms.RadioButton
        Private WithEvents m_rbDisplayCoverB As System.Windows.Forms.RadioButton
        Private WithEvents m_cbMPA As System.Windows.Forms.CheckBox
        Private WithEvents m_hdrLabelOptions As cEwEHeaderLabel
        Private WithEvents m_cmbLabelPos As System.Windows.Forms.ComboBox
        Private WithEvents m_cbShowLabels As System.Windows.Forms.CheckBox
        Private WithEvents m_plLabelOptions As System.Windows.Forms.Panel
        Private WithEvents m_cbInvertColor As System.Windows.Forms.CheckBox
        Private WithEvents m_hdrRunning As ScientificInterfaceShared.Controls.cEwEHeaderLabel
        Private WithEvents m_btnPause As System.Windows.Forms.Button
        Private WithEvents m_cmbRunType As System.Windows.Forms.ComboBox
        Private WithEvents m_cbShowIBMPackets As System.Windows.Forms.CheckBox
        Private WithEvents m_rbDisplayF As System.Windows.Forms.RadioButton
        Private WithEvents m_plRun As System.Windows.Forms.Panel
        Private WithEvents m_rbDisplayFOverB As System.Windows.Forms.RadioButton
        Private WithEvents m_tlpOptions As System.Windows.Forms.TableLayoutPanel
        Friend WithEvents m_ckSelFleets As System.Windows.Forms.CheckBox
        Friend WithEvents m_txFMax As System.Windows.Forms.TextBox
        Friend WithEvents Label1 As System.Windows.Forms.Label

 
    End Class

End Namespace

