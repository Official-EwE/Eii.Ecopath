<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Public Class AppLauncher
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
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
        Dim m_tss As System.Windows.Forms.ToolStripSeparator
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(AppLauncher))
        Me.m_tsmiHelpContents = New System.Windows.Forms.ToolStripMenuItem
        Me.m_tsmiHelp = New System.Windows.Forms.ToolStripMenuItem
        Me.m_tsmiHelpIndex = New System.Windows.Forms.ToolStripMenuItem
        Me.m_tsmiHelpSearch = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripSeparator12 = New System.Windows.Forms.ToolStripSeparator
        Me.m_tsmiHelpBugReport = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripSeparator8 = New System.Windows.Forms.ToolStripSeparator
        Me.m_tsmiHelpAbout = New System.Windows.Forms.ToolStripMenuItem
        Me.m_tsmiWindows = New System.Windows.Forms.ToolStripMenuItem
        Me.m_tsmiWindowsClose = New System.Windows.Forms.ToolStripMenuItem
        Me.m_tsmiWindowsCloseAll = New System.Windows.Forms.ToolStripMenuItem
        Me.m_tsmiOptions = New System.Windows.Forms.ToolStripMenuItem
        Me.m_tsStatus = New System.Windows.Forms.ToolStripStatusLabel
        Me.m_ssMain = New System.Windows.Forms.StatusStrip
        Me.m_tsbProgress = New System.Windows.Forms.ToolStripProgressBar
        Me.m_tsSelection = New System.Windows.Forms.ToolStripStatusLabel
        Me.m_tsEcopathModel = New System.Windows.Forms.ToolStripStatusLabel
        Me.m_tsEcosimScenario = New System.Windows.Forms.ToolStripStatusLabel
        Me.m_tsEcospaceScenario = New System.Windows.Forms.ToolStripStatusLabel
        Me.m_tsEcotracerScenario = New System.Windows.Forms.ToolStripStatusLabel
        Me.m_tsmiFileExit = New System.Windows.Forms.ToolStripMenuItem
        Me.m_tsmiFile = New System.Windows.Forms.ToolStripMenuItem
        Me.m_tsmiFileNew = New System.Windows.Forms.ToolStripMenuItem
        Me.m_tsmiFileOpen = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripSeparator5 = New System.Windows.Forms.ToolStripSeparator
        Me.m_tsmiFileClose = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator
        Me.m_tsmiFileSave = New System.Windows.Forms.ToolStripMenuItem
        Me.m_tsmiFileSaveAs = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripSeparator9 = New System.Windows.Forms.ToolStripSeparator
        Me.m_tsmiFileRecent = New System.Windows.Forms.ToolStripMenuItem
        Me.NoneToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripSeparator4 = New System.Windows.Forms.ToolStripSeparator
        Me.m_menuMain = New System.Windows.Forms.MenuStrip
        Me.m_tsmiView = New System.Windows.Forms.ToolStripMenuItem
        Me.m_tsmiViewStartPage = New System.Windows.Forms.ToolStripMenuItem
        Me.m_tsmiViewNavigation = New System.Windows.Forms.ToolStripMenuItem
        Me.m_tsmiViewStatus = New System.Windows.Forms.ToolStripMenuItem
        Me.m_tsmiViewRemarks = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripSeparator10 = New System.Windows.Forms.ToolStripSeparator
        Me.m_tsmiViewModelBar = New System.Windows.Forms.ToolStripMenuItem
        Me.m_tsmiViewStatusBar = New System.Windows.Forms.ToolStripMenuItem
        Me.m_tsmiEcopath = New System.Windows.Forms.ToolStripMenuItem
        Me.m_tsmiEcopathEditGroups = New System.Windows.Forms.ToolStripMenuItem
        Me.m_tsmiEcopathEditMultiStanza = New System.Windows.Forms.ToolStripMenuItem
        Me.m_tsmiEcopathEditFleets = New System.Windows.Forms.ToolStripMenuItem
        Me.m_tsmiEcosim = New System.Windows.Forms.ToolStripMenuItem
        Me.m_tsmiEcosimNew = New System.Windows.Forms.ToolStripMenuItem
        Me.m_tsmiEcosimLoad = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator
        Me.m_tsmiEcosimSave = New System.Windows.Forms.ToolStripMenuItem
        Me.m_tsmiEcosimSaveAs = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripSeparator6 = New System.Windows.Forms.ToolStripSeparator
        Me.m_tsmiTimeSeriesImport = New System.Windows.Forms.ToolStripMenuItem
        Me.m_tsmiTimeSeriesLoad = New System.Windows.Forms.ToolStripMenuItem
        Me.m_tsmiTimeSeriesEditWeights = New System.Windows.Forms.ToolStripMenuItem
        Me.m_tsmiTimeSeriesReloadLast = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripSeparator3 = New System.Windows.Forms.ToolStripSeparator
        Me.m_tsmiExportBiomassToCSV = New System.Windows.Forms.ToolStripMenuItem
        Me.m_tsmiEcospace = New System.Windows.Forms.ToolStripMenuItem
        Me.m_tsmiEcospaceNew = New System.Windows.Forms.ToolStripMenuItem
        Me.m_tsmiEcospaceLoad = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripSeparator7 = New System.Windows.Forms.ToolStripSeparator
        Me.m_tsmiEcospaceSave = New System.Windows.Forms.ToolStripMenuItem
        Me.m_tsmiEcospaceSaveAs = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripSeparator11 = New System.Windows.Forms.ToolStripSeparator
        Me.m_tsmiEcospaceEditMap = New System.Windows.Forms.ToolStripMenuItem
        Me.m_tsmiEcospaceEditHabitats = New System.Windows.Forms.ToolStripMenuItem
        Me.m_tsmiEcospaceEditMPAs = New System.Windows.Forms.ToolStripMenuItem
        Me.m_tsmiEcospaceEditRegions = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripSeparator16 = New System.Windows.Forms.ToolStripSeparator
        Me.m_tsmiEcospaceEditImportanceLayers = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripSeparator17 = New System.Windows.Forms.ToolStripSeparator
        Me.m_tsmiEcospaceImportLayers = New System.Windows.Forms.ToolStripMenuItem
        Me.m_tsmiTools = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripSeparator15 = New System.Windows.Forms.ToolStripSeparator
        Me.m_tsmiEcotracer = New System.Windows.Forms.ToolStripMenuItem
        Me.m_tsmiEcotracerNew = New System.Windows.Forms.ToolStripMenuItem
        Me.m_tsmiEcotracerLoad = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripSeparator14 = New System.Windows.Forms.ToolStripSeparator
        Me.m_tsmiEcotracerSave = New System.Windows.Forms.ToolStripMenuItem
        Me.m_tsmiEcotracerSaveAs = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripSeparator13 = New System.Windows.Forms.ToolStripSeparator
        Me.ToolBarToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.m_tsModel = New System.Windows.Forms.ToolStrip
        Me.m_tsbEcopath = New System.Windows.Forms.ToolStripButton
        Me.m_tsbEcosim = New System.Windows.Forms.ToolStripSplitButton
        Me.m_tsbEcospace = New System.Windows.Forms.ToolStripSplitButton
        Me.m_tsbModel = New System.Windows.Forms.ToolStripButton
        Me.m_tsmiFileCompact = New System.Windows.Forms.ToolStripMenuItem
        m_tss = New System.Windows.Forms.ToolStripSeparator
        Me.m_ssMain.SuspendLayout()
        Me.m_menuMain.SuspendLayout()
        Me.m_tsModel.SuspendLayout()
        Me.SuspendLayout()
        '
        'm_tss
        '
        m_tss.Name = "m_tss"
        resources.ApplyResources(m_tss, "m_tss")
        '
        'm_tsmiHelpContents
        '
        Me.m_tsmiHelpContents.Name = "m_tsmiHelpContents"
        resources.ApplyResources(Me.m_tsmiHelpContents, "m_tsmiHelpContents")
        '
        'm_tsmiHelp
        '
        Me.m_tsmiHelp.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tsmiHelpContents, Me.m_tsmiHelpIndex, Me.m_tsmiHelpSearch, Me.ToolStripSeparator12, Me.m_tsmiHelpBugReport, Me.ToolStripSeparator8, Me.m_tsmiHelpAbout})
        Me.m_tsmiHelp.Name = "m_tsmiHelp"
        resources.ApplyResources(Me.m_tsmiHelp, "m_tsmiHelp")
        '
        'm_tsmiHelpIndex
        '
        resources.ApplyResources(Me.m_tsmiHelpIndex, "m_tsmiHelpIndex")
        Me.m_tsmiHelpIndex.Name = "m_tsmiHelpIndex"
        '
        'm_tsmiHelpSearch
        '
        resources.ApplyResources(Me.m_tsmiHelpSearch, "m_tsmiHelpSearch")
        Me.m_tsmiHelpSearch.Name = "m_tsmiHelpSearch"
        '
        'ToolStripSeparator12
        '
        Me.ToolStripSeparator12.Name = "ToolStripSeparator12"
        resources.ApplyResources(Me.ToolStripSeparator12, "ToolStripSeparator12")
        '
        'm_tsmiHelpBugReport
        '
        Me.m_tsmiHelpBugReport.Name = "m_tsmiHelpBugReport"
        resources.ApplyResources(Me.m_tsmiHelpBugReport, "m_tsmiHelpBugReport")
        '
        'ToolStripSeparator8
        '
        Me.ToolStripSeparator8.Name = "ToolStripSeparator8"
        resources.ApplyResources(Me.ToolStripSeparator8, "ToolStripSeparator8")
        '
        'm_tsmiHelpAbout
        '
        Me.m_tsmiHelpAbout.Name = "m_tsmiHelpAbout"
        resources.ApplyResources(Me.m_tsmiHelpAbout, "m_tsmiHelpAbout")
        '
        'm_tsmiWindows
        '
        Me.m_tsmiWindows.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tsmiWindowsClose, Me.m_tsmiWindowsCloseAll})
        Me.m_tsmiWindows.Name = "m_tsmiWindows"
        resources.ApplyResources(Me.m_tsmiWindows, "m_tsmiWindows")
        '
        'm_tsmiWindowsClose
        '
        Me.m_tsmiWindowsClose.Name = "m_tsmiWindowsClose"
        resources.ApplyResources(Me.m_tsmiWindowsClose, "m_tsmiWindowsClose")
        '
        'm_tsmiWindowsCloseAll
        '
        Me.m_tsmiWindowsCloseAll.Name = "m_tsmiWindowsCloseAll"
        resources.ApplyResources(Me.m_tsmiWindowsCloseAll, "m_tsmiWindowsCloseAll")
        '
        'm_tsmiOptions
        '
        Me.m_tsmiOptions.Image = Global.ScientificInterface.My.Resources.Resources.OptionsHS
        Me.m_tsmiOptions.Name = "m_tsmiOptions"
        resources.ApplyResources(Me.m_tsmiOptions, "m_tsmiOptions")
        '
        'm_tsStatus
        '
        resources.ApplyResources(Me.m_tsStatus, "m_tsStatus")
        Me.m_tsStatus.Name = "m_tsStatus"
        Me.m_tsStatus.Spring = True
        '
        'm_ssMain
        '
        resources.ApplyResources(Me.m_ssMain, "m_ssMain")
        Me.m_ssMain.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tsbProgress, Me.m_tsStatus, Me.m_tsSelection, Me.m_tsEcopathModel, Me.m_tsEcosimScenario, Me.m_tsEcospaceScenario, Me.m_tsEcotracerScenario})
        Me.m_ssMain.Name = "m_ssMain"
        Me.m_ssMain.ShowItemToolTips = True
        '
        'm_tsbProgress
        '
        Me.m_tsbProgress.Name = "m_tsbProgress"
        resources.ApplyResources(Me.m_tsbProgress, "m_tsbProgress")
        Me.m_tsbProgress.Step = 1
        Me.m_tsbProgress.Style = System.Windows.Forms.ProgressBarStyle.Continuous
        '
        'm_tsSelection
        '
        Me.m_tsSelection.BorderSides = CType((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left Or System.Windows.Forms.ToolStripStatusLabelBorderSides.Right), System.Windows.Forms.ToolStripStatusLabelBorderSides)
        Me.m_tsSelection.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter
        Me.m_tsSelection.Name = "m_tsSelection"
        resources.ApplyResources(Me.m_tsSelection, "m_tsSelection")
        '
        'm_tsEcopathModel
        '
        Me.m_tsEcopathModel.AutoToolTip = True
        Me.m_tsEcopathModel.BorderSides = CType((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left Or System.Windows.Forms.ToolStripStatusLabelBorderSides.Right), System.Windows.Forms.ToolStripStatusLabelBorderSides)
        Me.m_tsEcopathModel.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter
        Me.m_tsEcopathModel.Image = Global.ScientificInterface.My.Resources.Resources.Ecopath_32x32
        Me.m_tsEcopathModel.Name = "m_tsEcopathModel"
        resources.ApplyResources(Me.m_tsEcopathModel, "m_tsEcopathModel")
        '
        'm_tsEcosimScenario
        '
        Me.m_tsEcosimScenario.AutoToolTip = True
        Me.m_tsEcosimScenario.BorderSides = CType((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left Or System.Windows.Forms.ToolStripStatusLabelBorderSides.Right), System.Windows.Forms.ToolStripStatusLabelBorderSides)
        Me.m_tsEcosimScenario.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter
        Me.m_tsEcosimScenario.Image = Global.ScientificInterface.My.Resources.Resources.Ecosim_32x32
        Me.m_tsEcosimScenario.Name = "m_tsEcosimScenario"
        resources.ApplyResources(Me.m_tsEcosimScenario, "m_tsEcosimScenario")
        '
        'm_tsEcospaceScenario
        '
        Me.m_tsEcospaceScenario.AutoToolTip = True
        Me.m_tsEcospaceScenario.BorderSides = CType((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left Or System.Windows.Forms.ToolStripStatusLabelBorderSides.Right), System.Windows.Forms.ToolStripStatusLabelBorderSides)
        Me.m_tsEcospaceScenario.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter
        Me.m_tsEcospaceScenario.Image = Global.ScientificInterface.My.Resources.Resources.Ecospace_32x32
        Me.m_tsEcospaceScenario.Name = "m_tsEcospaceScenario"
        resources.ApplyResources(Me.m_tsEcospaceScenario, "m_tsEcospaceScenario")
        '
        'm_tsEcotracerScenario
        '
        Me.m_tsEcotracerScenario.AutoToolTip = True
        Me.m_tsEcotracerScenario.BorderSides = CType((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left Or System.Windows.Forms.ToolStripStatusLabelBorderSides.Right), System.Windows.Forms.ToolStripStatusLabelBorderSides)
        Me.m_tsEcotracerScenario.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter
        Me.m_tsEcotracerScenario.Image = Global.ScientificInterface.My.Resources.Resources.Ecotracer_32x32
        Me.m_tsEcotracerScenario.Name = "m_tsEcotracerScenario"
        resources.ApplyResources(Me.m_tsEcotracerScenario, "m_tsEcotracerScenario")
        '
        'm_tsmiFileExit
        '
        Me.m_tsmiFileExit.Name = "m_tsmiFileExit"
        resources.ApplyResources(Me.m_tsmiFileExit, "m_tsmiFileExit")
        '
        'm_tsmiFile
        '
        Me.m_tsmiFile.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tsmiFileNew, Me.m_tsmiFileOpen, Me.ToolStripSeparator5, Me.m_tsmiFileClose, Me.ToolStripSeparator1, Me.m_tsmiFileSave, Me.m_tsmiFileSaveAs, Me.m_tsmiFileCompact, Me.ToolStripSeparator9, Me.m_tsmiFileRecent, Me.ToolStripSeparator4, Me.m_tsmiFileExit})
        resources.ApplyResources(Me.m_tsmiFile, "m_tsmiFile")
        Me.m_tsmiFile.Name = "m_tsmiFile"
        '
        'm_tsmiFileNew
        '
        Me.m_tsmiFileNew.Image = Global.ScientificInterface.My.Resources.Resources.NewDocumentHS
        Me.m_tsmiFileNew.Name = "m_tsmiFileNew"
        resources.ApplyResources(Me.m_tsmiFileNew, "m_tsmiFileNew")
        '
        'm_tsmiFileOpen
        '
        Me.m_tsmiFileOpen.Image = Global.ScientificInterface.My.Resources.Resources.openHS
        resources.ApplyResources(Me.m_tsmiFileOpen, "m_tsmiFileOpen")
        Me.m_tsmiFileOpen.Name = "m_tsmiFileOpen"
        '
        'ToolStripSeparator5
        '
        Me.ToolStripSeparator5.Name = "ToolStripSeparator5"
        resources.ApplyResources(Me.ToolStripSeparator5, "ToolStripSeparator5")
        '
        'm_tsmiFileClose
        '
        Me.m_tsmiFileClose.Name = "m_tsmiFileClose"
        resources.ApplyResources(Me.m_tsmiFileClose, "m_tsmiFileClose")
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        resources.ApplyResources(Me.ToolStripSeparator1, "ToolStripSeparator1")
        '
        'm_tsmiFileSave
        '
        Me.m_tsmiFileSave.Image = Global.ScientificInterface.My.Resources.Resources.saveHS
        Me.m_tsmiFileSave.Name = "m_tsmiFileSave"
        resources.ApplyResources(Me.m_tsmiFileSave, "m_tsmiFileSave")
        '
        'm_tsmiFileSaveAs
        '
        Me.m_tsmiFileSaveAs.Name = "m_tsmiFileSaveAs"
        resources.ApplyResources(Me.m_tsmiFileSaveAs, "m_tsmiFileSaveAs")
        '
        'ToolStripSeparator9
        '
        Me.ToolStripSeparator9.Name = "ToolStripSeparator9"
        resources.ApplyResources(Me.ToolStripSeparator9, "ToolStripSeparator9")
        '
        'm_tsmiFileRecent
        '
        Me.m_tsmiFileRecent.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.NoneToolStripMenuItem})
        Me.m_tsmiFileRecent.Name = "m_tsmiFileRecent"
        resources.ApplyResources(Me.m_tsmiFileRecent, "m_tsmiFileRecent")
        '
        'NoneToolStripMenuItem
        '
        resources.ApplyResources(Me.NoneToolStripMenuItem, "NoneToolStripMenuItem")
        Me.NoneToolStripMenuItem.Name = "NoneToolStripMenuItem"
        '
        'ToolStripSeparator4
        '
        Me.ToolStripSeparator4.Name = "ToolStripSeparator4"
        resources.ApplyResources(Me.ToolStripSeparator4, "ToolStripSeparator4")
        '
        'm_menuMain
        '
        Me.m_menuMain.GripMargin = New System.Windows.Forms.Padding(0)
        Me.m_menuMain.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tsmiFile, Me.m_tsmiView, Me.m_tsmiEcopath, Me.m_tsmiEcosim, Me.m_tsmiEcospace, Me.m_tsmiTools, Me.m_tsmiWindows, Me.m_tsmiHelp})
        resources.ApplyResources(Me.m_menuMain, "m_menuMain")
        Me.m_menuMain.MdiWindowListItem = Me.m_tsmiWindows
        Me.m_menuMain.Name = "m_menuMain"
        '
        'm_tsmiView
        '
        Me.m_tsmiView.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tsmiViewStartPage, Me.m_tsmiViewNavigation, Me.m_tsmiViewStatus, Me.m_tsmiViewRemarks, Me.ToolStripSeparator10, Me.m_tsmiViewModelBar, Me.m_tsmiViewStatusBar})
        Me.m_tsmiView.Name = "m_tsmiView"
        resources.ApplyResources(Me.m_tsmiView, "m_tsmiView")
        '
        'm_tsmiViewStartPage
        '
        Me.m_tsmiViewStartPage.Name = "m_tsmiViewStartPage"
        resources.ApplyResources(Me.m_tsmiViewStartPage, "m_tsmiViewStartPage")
        '
        'm_tsmiViewNavigation
        '
        Me.m_tsmiViewNavigation.Name = "m_tsmiViewNavigation"
        resources.ApplyResources(Me.m_tsmiViewNavigation, "m_tsmiViewNavigation")
        '
        'm_tsmiViewStatus
        '
        Me.m_tsmiViewStatus.Name = "m_tsmiViewStatus"
        resources.ApplyResources(Me.m_tsmiViewStatus, "m_tsmiViewStatus")
        '
        'm_tsmiViewRemarks
        '
        Me.m_tsmiViewRemarks.Name = "m_tsmiViewRemarks"
        resources.ApplyResources(Me.m_tsmiViewRemarks, "m_tsmiViewRemarks")
        '
        'ToolStripSeparator10
        '
        Me.ToolStripSeparator10.Name = "ToolStripSeparator10"
        resources.ApplyResources(Me.ToolStripSeparator10, "ToolStripSeparator10")
        '
        'm_tsmiViewModelBar
        '
        Me.m_tsmiViewModelBar.Name = "m_tsmiViewModelBar"
        resources.ApplyResources(Me.m_tsmiViewModelBar, "m_tsmiViewModelBar")
        '
        'm_tsmiViewStatusBar
        '
        Me.m_tsmiViewStatusBar.Checked = True
        Me.m_tsmiViewStatusBar.CheckOnClick = True
        Me.m_tsmiViewStatusBar.CheckState = System.Windows.Forms.CheckState.Checked
        Me.m_tsmiViewStatusBar.Name = "m_tsmiViewStatusBar"
        resources.ApplyResources(Me.m_tsmiViewStatusBar, "m_tsmiViewStatusBar")
        '
        'm_tsmiEcopath
        '
        Me.m_tsmiEcopath.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tsmiEcopathEditGroups, Me.m_tsmiEcopathEditMultiStanza, Me.m_tsmiEcopathEditFleets})
        Me.m_tsmiEcopath.Name = "m_tsmiEcopath"
        resources.ApplyResources(Me.m_tsmiEcopath, "m_tsmiEcopath")
        '
        'm_tsmiEcopathEditGroups
        '
        Me.m_tsmiEcopathEditGroups.Image = Global.ScientificInterface.My.Resources.Resources.EditGroup
        Me.m_tsmiEcopathEditGroups.Name = "m_tsmiEcopathEditGroups"
        resources.ApplyResources(Me.m_tsmiEcopathEditGroups, "m_tsmiEcopathEditGroups")
        '
        'm_tsmiEcopathEditMultiStanza
        '
        Me.m_tsmiEcopathEditMultiStanza.Image = Global.ScientificInterface.My.Resources.Resources.EditMultiStanza
        Me.m_tsmiEcopathEditMultiStanza.Name = "m_tsmiEcopathEditMultiStanza"
        resources.ApplyResources(Me.m_tsmiEcopathEditMultiStanza, "m_tsmiEcopathEditMultiStanza")
        '
        'm_tsmiEcopathEditFleets
        '
        Me.m_tsmiEcopathEditFleets.Image = Global.ScientificInterface.My.Resources.Resources.EditFleets
        Me.m_tsmiEcopathEditFleets.Name = "m_tsmiEcopathEditFleets"
        resources.ApplyResources(Me.m_tsmiEcopathEditFleets, "m_tsmiEcopathEditFleets")
        '
        'm_tsmiEcosim
        '
        Me.m_tsmiEcosim.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tsmiEcosimNew, Me.m_tsmiEcosimLoad, Me.ToolStripSeparator2, Me.m_tsmiEcosimSave, Me.m_tsmiEcosimSaveAs, Me.ToolStripSeparator6, Me.m_tsmiTimeSeriesImport, Me.m_tsmiTimeSeriesLoad, Me.m_tsmiTimeSeriesEditWeights, Me.m_tsmiTimeSeriesReloadLast, Me.ToolStripSeparator3, Me.m_tsmiExportBiomassToCSV})
        Me.m_tsmiEcosim.Name = "m_tsmiEcosim"
        resources.ApplyResources(Me.m_tsmiEcosim, "m_tsmiEcosim")
        '
        'm_tsmiEcosimNew
        '
        Me.m_tsmiEcosimNew.Image = Global.ScientificInterface.My.Resources.Resources.NewDocumentHS
        Me.m_tsmiEcosimNew.Name = "m_tsmiEcosimNew"
        resources.ApplyResources(Me.m_tsmiEcosimNew, "m_tsmiEcosimNew")
        '
        'm_tsmiEcosimLoad
        '
        Me.m_tsmiEcosimLoad.Image = Global.ScientificInterface.My.Resources.Resources.openHS
        Me.m_tsmiEcosimLoad.Name = "m_tsmiEcosimLoad"
        resources.ApplyResources(Me.m_tsmiEcosimLoad, "m_tsmiEcosimLoad")
        '
        'ToolStripSeparator2
        '
        Me.ToolStripSeparator2.Name = "ToolStripSeparator2"
        resources.ApplyResources(Me.ToolStripSeparator2, "ToolStripSeparator2")
        '
        'm_tsmiEcosimSave
        '
        Me.m_tsmiEcosimSave.Image = Global.ScientificInterface.My.Resources.Resources.saveHS
        Me.m_tsmiEcosimSave.Name = "m_tsmiEcosimSave"
        resources.ApplyResources(Me.m_tsmiEcosimSave, "m_tsmiEcosimSave")
        '
        'm_tsmiEcosimSaveAs
        '
        Me.m_tsmiEcosimSaveAs.Name = "m_tsmiEcosimSaveAs"
        resources.ApplyResources(Me.m_tsmiEcosimSaveAs, "m_tsmiEcosimSaveAs")
        '
        'ToolStripSeparator6
        '
        Me.ToolStripSeparator6.Name = "ToolStripSeparator6"
        resources.ApplyResources(Me.ToolStripSeparator6, "ToolStripSeparator6")
        '
        'm_tsmiTimeSeriesImport
        '
        Me.m_tsmiTimeSeriesImport.Image = Global.ScientificInterface.My.Resources.Resources.ImportXMLHS
        Me.m_tsmiTimeSeriesImport.Name = "m_tsmiTimeSeriesImport"
        resources.ApplyResources(Me.m_tsmiTimeSeriesImport, "m_tsmiTimeSeriesImport")
        '
        'm_tsmiTimeSeriesLoad
        '
        Me.m_tsmiTimeSeriesLoad.Name = "m_tsmiTimeSeriesLoad"
        resources.ApplyResources(Me.m_tsmiTimeSeriesLoad, "m_tsmiTimeSeriesLoad")
        '
        'm_tsmiTimeSeriesEditWeights
        '
        Me.m_tsmiTimeSeriesEditWeights.Name = "m_tsmiTimeSeriesEditWeights"
        resources.ApplyResources(Me.m_tsmiTimeSeriesEditWeights, "m_tsmiTimeSeriesEditWeights")
        '
        'm_tsmiTimeSeriesReloadLast
        '
        Me.m_tsmiTimeSeriesReloadLast.Image = Global.ScientificInterface.My.Resources.Resources.NavForward
        Me.m_tsmiTimeSeriesReloadLast.Name = "m_tsmiTimeSeriesReloadLast"
        resources.ApplyResources(Me.m_tsmiTimeSeriesReloadLast, "m_tsmiTimeSeriesReloadLast")
        '
        'ToolStripSeparator3
        '
        Me.ToolStripSeparator3.Name = "ToolStripSeparator3"
        resources.ApplyResources(Me.ToolStripSeparator3, "ToolStripSeparator3")
        '
        'm_tsmiExportBiomassToCSV
        '
        Me.m_tsmiExportBiomassToCSV.Name = "m_tsmiExportBiomassToCSV"
        resources.ApplyResources(Me.m_tsmiExportBiomassToCSV, "m_tsmiExportBiomassToCSV")
        '
        'm_tsmiEcospace
        '
        Me.m_tsmiEcospace.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tsmiEcospaceNew, Me.m_tsmiEcospaceLoad, Me.ToolStripSeparator7, Me.m_tsmiEcospaceSave, Me.m_tsmiEcospaceSaveAs, Me.ToolStripSeparator11, Me.m_tsmiEcospaceEditMap, Me.m_tsmiEcospaceEditHabitats, Me.m_tsmiEcospaceEditMPAs, Me.m_tsmiEcospaceEditRegions, Me.ToolStripSeparator16, Me.m_tsmiEcospaceEditImportanceLayers, Me.ToolStripSeparator17, Me.m_tsmiEcospaceImportLayers})
        Me.m_tsmiEcospace.Name = "m_tsmiEcospace"
        resources.ApplyResources(Me.m_tsmiEcospace, "m_tsmiEcospace")
        '
        'm_tsmiEcospaceNew
        '
        Me.m_tsmiEcospaceNew.Image = Global.ScientificInterface.My.Resources.Resources.NewDocumentHS
        Me.m_tsmiEcospaceNew.Name = "m_tsmiEcospaceNew"
        resources.ApplyResources(Me.m_tsmiEcospaceNew, "m_tsmiEcospaceNew")
        '
        'm_tsmiEcospaceLoad
        '
        Me.m_tsmiEcospaceLoad.Image = Global.ScientificInterface.My.Resources.Resources.openHS
        Me.m_tsmiEcospaceLoad.Name = "m_tsmiEcospaceLoad"
        resources.ApplyResources(Me.m_tsmiEcospaceLoad, "m_tsmiEcospaceLoad")
        '
        'ToolStripSeparator7
        '
        Me.ToolStripSeparator7.Name = "ToolStripSeparator7"
        resources.ApplyResources(Me.ToolStripSeparator7, "ToolStripSeparator7")
        '
        'm_tsmiEcospaceSave
        '
        Me.m_tsmiEcospaceSave.Image = Global.ScientificInterface.My.Resources.Resources.saveHS
        Me.m_tsmiEcospaceSave.Name = "m_tsmiEcospaceSave"
        resources.ApplyResources(Me.m_tsmiEcospaceSave, "m_tsmiEcospaceSave")
        '
        'm_tsmiEcospaceSaveAs
        '
        Me.m_tsmiEcospaceSaveAs.Name = "m_tsmiEcospaceSaveAs"
        resources.ApplyResources(Me.m_tsmiEcospaceSaveAs, "m_tsmiEcospaceSaveAs")
        '
        'ToolStripSeparator11
        '
        Me.ToolStripSeparator11.Name = "ToolStripSeparator11"
        resources.ApplyResources(Me.ToolStripSeparator11, "ToolStripSeparator11")
        '
        'm_tsmiEcospaceEditMap
        '
        Me.m_tsmiEcospaceEditMap.Image = Global.ScientificInterface.My.Resources.Resources.Raster1
        Me.m_tsmiEcospaceEditMap.Name = "m_tsmiEcospaceEditMap"
        resources.ApplyResources(Me.m_tsmiEcospaceEditMap, "m_tsmiEcospaceEditMap")
        '
        'm_tsmiEcospaceEditHabitats
        '
        Me.m_tsmiEcospaceEditHabitats.Image = Global.ScientificInterface.My.Resources.Resources.Habitat1
        Me.m_tsmiEcospaceEditHabitats.Name = "m_tsmiEcospaceEditHabitats"
        resources.ApplyResources(Me.m_tsmiEcospaceEditHabitats, "m_tsmiEcospaceEditHabitats")
        '
        'm_tsmiEcospaceEditMPAs
        '
        Me.m_tsmiEcospaceEditMPAs.Image = Global.ScientificInterface.My.Resources.Resources.MPA1
        Me.m_tsmiEcospaceEditMPAs.Name = "m_tsmiEcospaceEditMPAs"
        resources.ApplyResources(Me.m_tsmiEcospaceEditMPAs, "m_tsmiEcospaceEditMPAs")
        '
        'm_tsmiEcospaceEditRegions
        '
        Me.m_tsmiEcospaceEditRegions.Image = Global.ScientificInterface.My.Resources.Resources.Regions
        Me.m_tsmiEcospaceEditRegions.Name = "m_tsmiEcospaceEditRegions"
        resources.ApplyResources(Me.m_tsmiEcospaceEditRegions, "m_tsmiEcospaceEditRegions")
        '
        'ToolStripSeparator16
        '
        Me.ToolStripSeparator16.Name = "ToolStripSeparator16"
        resources.ApplyResources(Me.ToolStripSeparator16, "ToolStripSeparator16")
        '
        'm_tsmiEcospaceEditImportanceLayers
        '
        Me.m_tsmiEcospaceEditImportanceLayers.Name = "m_tsmiEcospaceEditImportanceLayers"
        resources.ApplyResources(Me.m_tsmiEcospaceEditImportanceLayers, "m_tsmiEcospaceEditImportanceLayers")
        '
        'ToolStripSeparator17
        '
        Me.ToolStripSeparator17.Name = "ToolStripSeparator17"
        resources.ApplyResources(Me.ToolStripSeparator17, "ToolStripSeparator17")
        '
        'm_tsmiEcospaceImportLayers
        '
        Me.m_tsmiEcospaceImportLayers.Name = "m_tsmiEcospaceImportLayers"
        resources.ApplyResources(Me.m_tsmiEcospaceImportLayers, "m_tsmiEcospaceImportLayers")
        '
        'm_tsmiTools
        '
        Me.m_tsmiTools.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tsmiOptions, Me.ToolStripSeparator15, Me.m_tsmiEcotracer})
        Me.m_tsmiTools.Name = "m_tsmiTools"
        resources.ApplyResources(Me.m_tsmiTools, "m_tsmiTools")
        '
        'ToolStripSeparator15
        '
        Me.ToolStripSeparator15.Name = "ToolStripSeparator15"
        resources.ApplyResources(Me.ToolStripSeparator15, "ToolStripSeparator15")
        '
        'm_tsmiEcotracer
        '
        Me.m_tsmiEcotracer.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tsmiEcotracerNew, Me.m_tsmiEcotracerLoad, Me.ToolStripSeparator14, Me.m_tsmiEcotracerSave, Me.m_tsmiEcotracerSaveAs})
        Me.m_tsmiEcotracer.Image = Global.ScientificInterface.My.Resources.Resources.Ecotracer_32x32
        Me.m_tsmiEcotracer.Name = "m_tsmiEcotracer"
        resources.ApplyResources(Me.m_tsmiEcotracer, "m_tsmiEcotracer")
        '
        'm_tsmiEcotracerNew
        '
        Me.m_tsmiEcotracerNew.Image = Global.ScientificInterface.My.Resources.Resources.NewDocumentHS
        Me.m_tsmiEcotracerNew.Name = "m_tsmiEcotracerNew"
        resources.ApplyResources(Me.m_tsmiEcotracerNew, "m_tsmiEcotracerNew")
        '
        'm_tsmiEcotracerLoad
        '
        Me.m_tsmiEcotracerLoad.Image = Global.ScientificInterface.My.Resources.Resources.openHS
        Me.m_tsmiEcotracerLoad.Name = "m_tsmiEcotracerLoad"
        resources.ApplyResources(Me.m_tsmiEcotracerLoad, "m_tsmiEcotracerLoad")
        '
        'ToolStripSeparator14
        '
        Me.ToolStripSeparator14.Name = "ToolStripSeparator14"
        resources.ApplyResources(Me.ToolStripSeparator14, "ToolStripSeparator14")
        '
        'm_tsmiEcotracerSave
        '
        Me.m_tsmiEcotracerSave.Image = Global.ScientificInterface.My.Resources.Resources.saveHS
        Me.m_tsmiEcotracerSave.Name = "m_tsmiEcotracerSave"
        resources.ApplyResources(Me.m_tsmiEcotracerSave, "m_tsmiEcotracerSave")
        '
        'm_tsmiEcotracerSaveAs
        '
        Me.m_tsmiEcotracerSaveAs.Name = "m_tsmiEcotracerSaveAs"
        resources.ApplyResources(Me.m_tsmiEcotracerSaveAs, "m_tsmiEcotracerSaveAs")
        '
        'ToolStripSeparator13
        '
        Me.ToolStripSeparator13.Name = "ToolStripSeparator13"
        resources.ApplyResources(Me.ToolStripSeparator13, "ToolStripSeparator13")
        '
        'ToolBarToolStripMenuItem
        '
        Me.ToolBarToolStripMenuItem.Name = "ToolBarToolStripMenuItem"
        resources.ApplyResources(Me.ToolBarToolStripMenuItem, "ToolBarToolStripMenuItem")
        '
        'm_tsModel
        '
        Me.m_tsModel.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tsbEcopath, Me.m_tsbEcosim, Me.m_tsbEcospace, m_tss, Me.m_tsbModel})
        Me.m_tsModel.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow
        resources.ApplyResources(Me.m_tsModel, "m_tsModel")
        Me.m_tsModel.Name = "m_tsModel"
        '
        'm_tsbEcopath
        '
        Me.m_tsbEcopath.Image = Global.ScientificInterface.My.Resources.Resources.Ecopath_32x32
        resources.ApplyResources(Me.m_tsbEcopath, "m_tsbEcopath")
        Me.m_tsbEcopath.Name = "m_tsbEcopath"
        '
        'm_tsbEcosim
        '
        Me.m_tsbEcosim.BackColor = System.Drawing.SystemColors.Control
        Me.m_tsbEcosim.DropDownButtonWidth = 16
        Me.m_tsbEcosim.Image = Global.ScientificInterface.My.Resources.Resources.Ecosim_32x32
        resources.ApplyResources(Me.m_tsbEcosim, "m_tsbEcosim")
        Me.m_tsbEcosim.Name = "m_tsbEcosim"
        '
        'm_tsbEcospace
        '
        Me.m_tsbEcospace.DropDownButtonWidth = 16
        Me.m_tsbEcospace.Image = Global.ScientificInterface.My.Resources.Resources.Ecospace_32x32
        resources.ApplyResources(Me.m_tsbEcospace, "m_tsbEcospace")
        Me.m_tsbEcospace.Name = "m_tsbEcospace"
        '
        'm_tsbModel
        '
        Me.m_tsbModel.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.m_tsbModel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.m_tsbModel.ForeColor = System.Drawing.SystemColors.ControlDark
        resources.ApplyResources(Me.m_tsbModel, "m_tsbModel")
        Me.m_tsbModel.Name = "m_tsbModel"
        '
        'm_tsmiFileCompact
        '
        Me.m_tsmiFileCompact.Name = "m_tsmiFileCompact"
        resources.ApplyResources(Me.m_tsmiFileCompact, "m_tsmiFileCompact")
        '
        'AppLauncher
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.m_tsModel)
        Me.Controls.Add(Me.m_ssMain)
        Me.Controls.Add(Me.m_menuMain)
        Me.IsMdiContainer = True
        Me.MainMenuStrip = Me.m_menuMain
        Me.Name = "AppLauncher"
        Me.m_ssMain.ResumeLayout(False)
        Me.m_ssMain.PerformLayout()
        Me.m_menuMain.ResumeLayout(False)
        Me.m_menuMain.PerformLayout()
        Me.m_tsModel.ResumeLayout(False)
        Me.m_tsModel.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents m_tsmiHelpContents As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_tsmiHelp As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_tsmiHelpIndex As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_tsmiHelpSearch As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator8 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents m_tsmiHelpAbout As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ArrangeIconsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_tsmiWindows As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents CascadeToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents TileVerticalToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents TileHorizontalToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_tsmiOptions As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_ssMain As System.Windows.Forms.StatusStrip
    Friend WithEvents m_menuMain As System.Windows.Forms.MenuStrip
    Friend WithEvents ToolBarToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_tsmiTools As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator9 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ToolStripSeparator10 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents m_tsmiWindowsCloseAll As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_tsmiEcosim As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_tsmiEcosimLoad As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_tsmiEcosimSave As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator3 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents m_tsmiTimeSeriesImport As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator4 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents NoneToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator5 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents m_tsModel As System.Windows.Forms.ToolStrip
    Friend WithEvents m_tsmiEcospace As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_tsmiEcospaceLoad As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_tsmiEcospaceSave As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_tsmiWindowsClose As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents m_tsmiEcosimSaveAs As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_tsmiEcospaceSaveAs As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_tsmiEcosimNew As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator2 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ToolStripSeparator6 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents m_tsmiEcospaceNew As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator7 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ToolStripSeparator11 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents m_tsmiEcospaceEditHabitats As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_tsmiEcospaceEditMPAs As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_tsmiEcospaceEditRegions As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_tsmiEcospaceEditMap As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator12 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents m_tsmiTimeSeriesLoad As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator13 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents m_tsmiEcotracer As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_tsmiEcotracerLoad As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_tsmiEcotracerSave As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_tsmiEcotracerSaveAs As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_tsmiEcotracerNew As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator14 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ToolStripSeparator15 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents m_tsSelection As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents m_tsmiTimeSeriesReloadLast As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents m_tsbModel As System.Windows.Forms.ToolStripButton
    Private WithEvents m_tsbEcopath As System.Windows.Forms.ToolStripButton
    Private WithEvents m_tsbEcosim As System.Windows.Forms.ToolStripSplitButton
    Private WithEvents m_tsbEcospace As System.Windows.Forms.ToolStripSplitButton
    Friend WithEvents ToolStripSeparator16 As System.Windows.Forms.ToolStripSeparator
    Private WithEvents m_tsmiTimeSeriesEditWeights As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator17 As System.Windows.Forms.ToolStripSeparator
    Private WithEvents m_tsmiEcospaceImportLayers As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents m_tsmiEcospaceEditImportanceLayers As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents m_tsStatus As System.Windows.Forms.ToolStripStatusLabel
    Private WithEvents m_tsEcopathModel As System.Windows.Forms.ToolStripStatusLabel
    Private WithEvents m_tsEcosimScenario As System.Windows.Forms.ToolStripStatusLabel
    Private WithEvents m_tsEcospaceScenario As System.Windows.Forms.ToolStripStatusLabel
    Private WithEvents m_tsEcotracerScenario As System.Windows.Forms.ToolStripStatusLabel
    Private WithEvents m_tsbProgress As System.Windows.Forms.ToolStripProgressBar
    Private WithEvents m_tsmiHelpBugReport As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents m_tsmiExportBiomassToCSV As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents m_tsmiEcopath As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents m_tsmiEcopathEditGroups As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents m_tsmiEcopathEditMultiStanza As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents m_tsmiEcopathEditFleets As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents m_tsmiView As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents m_tsmiViewStatusBar As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents m_tsmiViewRemarks As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents m_tsmiViewStartPage As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents m_tsmiViewNavigation As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents m_tsmiViewStatus As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents m_tsmiViewModelBar As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents m_tsmiFile As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents m_tsmiFileOpen As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents m_tsmiFileClose As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents m_tsmiFileNew As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents m_tsmiFileSave As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents m_tsmiFileSaveAs As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents m_tsmiFileRecent As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents m_tsmiFileExit As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents m_tsmiFileCompact As System.Windows.Forms.ToolStripMenuItem

End Class

