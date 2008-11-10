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
        Me.ContentsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.HelpMenu = New System.Windows.Forms.ToolStripMenuItem
        Me.IndexToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.SearchToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripSeparator12 = New System.Windows.Forms.ToolStripSeparator
        Me.ReportBugMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripSeparator8 = New System.Windows.Forms.ToolStripSeparator
        Me.AboutToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.WindowsMenu = New System.Windows.Forms.ToolStripMenuItem
        Me.CloseToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.CloseAllToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.OptionsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.m_tsStatus = New System.Windows.Forms.ToolStripStatusLabel
        Me.m_ssMain = New System.Windows.Forms.StatusStrip
        Me.m_tsbProgress = New System.Windows.Forms.ToolStripProgressBar
        Me.m_tsSelection = New System.Windows.Forms.ToolStripStatusLabel
        Me.m_tsEcopathModel = New System.Windows.Forms.ToolStripStatusLabel
        Me.m_tsEcosimScenario = New System.Windows.Forms.ToolStripStatusLabel
        Me.m_tsEcospaceScenario = New System.Windows.Forms.ToolStripStatusLabel
        Me.m_tsEcotracerScenario = New System.Windows.Forms.ToolStripStatusLabel
        Me.ExitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.FileMenu = New System.Windows.Forms.ToolStripMenuItem
        Me.NewFileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.OpenFileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripSeparator5 = New System.Windows.Forms.ToolStripSeparator
        Me.CloseModelToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator
        Me.SaveModelToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.SaveModelAsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripSeparator9 = New System.Windows.Forms.ToolStripSeparator
        Me.RecentMDBToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.NoneToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripSeparator4 = New System.Windows.Forms.ToolStripSeparator
        Me.m_menuMain = New System.Windows.Forms.MenuStrip
        Me.ViewMenu = New System.Windows.Forms.ToolStripMenuItem
        Me.StartPageToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.NavigationPanelToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.StatusPanelToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.SelectionPanelToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripSeparator10 = New System.Windows.Forms.ToolStripSeparator
        Me.ModelBarToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.StatusBarToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.EcopathToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.EditGroupsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.EditMultiStanzaGroupsToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem
        Me.EditFleetsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.EcosimToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.NewEcosimScenarioToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.LoadEcosimScenarioToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator
        Me.SaveEcosimScenarioToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.SaveEcosimScenarioAsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripSeparator6 = New System.Windows.Forms.ToolStripSeparator
        Me.ImportTimeSeriesToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.LoadTimeSeriesToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.WeightTimeSeriesToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.LoadAndApplyLastTimeSeriesToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripSeparator3 = New System.Windows.Forms.ToolStripSeparator
        Me.ExportBiomassToFileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.EcospaceToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.NewEcospaceScenarioToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.LoadEcospaceScenarioToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripSeparator7 = New System.Windows.Forms.ToolStripSeparator
        Me.SaveEcospaceScenarioToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.SaveEcospaceScenarioAsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripSeparator11 = New System.Windows.Forms.ToolStripSeparator
        Me.EditBasemapToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.EditHabitatsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.EditMPAsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.EditRegionsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripSeparator16 = New System.Windows.Forms.ToolStripSeparator
        Me.EditImportanceLayersToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripSeparator17 = New System.Windows.Forms.ToolStripSeparator
        Me.ImportLayerDataToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolsMenu = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripSeparator15 = New System.Windows.Forms.ToolStripSeparator
        Me.EcotracerToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.NewTracerScenarioToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.LoadTracerScenarioToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripSeparator14 = New System.Windows.Forms.ToolStripSeparator
        Me.SaveTracerScenarioToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.SaveTracerScenarioAsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripSeparator13 = New System.Windows.Forms.ToolStripSeparator
        Me.ToolBarToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.m_tsModel = New System.Windows.Forms.ToolStrip
        Me.m_tsbEcopath = New System.Windows.Forms.ToolStripButton
        Me.m_tsbEcosim = New System.Windows.Forms.ToolStripSplitButton
        Me.m_tsbEcospace = New System.Windows.Forms.ToolStripSplitButton
        Me.m_tsbModel = New System.Windows.Forms.ToolStripButton
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
        'ContentsToolStripMenuItem
        '
        Me.ContentsToolStripMenuItem.Name = "ContentsToolStripMenuItem"
        resources.ApplyResources(Me.ContentsToolStripMenuItem, "ContentsToolStripMenuItem")
        '
        'HelpMenu
        '
        Me.HelpMenu.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ContentsToolStripMenuItem, Me.IndexToolStripMenuItem, Me.SearchToolStripMenuItem, Me.ToolStripSeparator12, Me.ReportBugMenuItem, Me.ToolStripSeparator8, Me.AboutToolStripMenuItem})
        Me.HelpMenu.Name = "HelpMenu"
        resources.ApplyResources(Me.HelpMenu, "HelpMenu")
        '
        'IndexToolStripMenuItem
        '
        resources.ApplyResources(Me.IndexToolStripMenuItem, "IndexToolStripMenuItem")
        Me.IndexToolStripMenuItem.Name = "IndexToolStripMenuItem"
        '
        'SearchToolStripMenuItem
        '
        resources.ApplyResources(Me.SearchToolStripMenuItem, "SearchToolStripMenuItem")
        Me.SearchToolStripMenuItem.Name = "SearchToolStripMenuItem"
        '
        'ToolStripSeparator12
        '
        Me.ToolStripSeparator12.Name = "ToolStripSeparator12"
        resources.ApplyResources(Me.ToolStripSeparator12, "ToolStripSeparator12")
        '
        'ReportBugMenuItem
        '
        Me.ReportBugMenuItem.Name = "ReportBugMenuItem"
        resources.ApplyResources(Me.ReportBugMenuItem, "ReportBugMenuItem")
        '
        'ToolStripSeparator8
        '
        Me.ToolStripSeparator8.Name = "ToolStripSeparator8"
        resources.ApplyResources(Me.ToolStripSeparator8, "ToolStripSeparator8")
        '
        'AboutToolStripMenuItem
        '
        Me.AboutToolStripMenuItem.Name = "AboutToolStripMenuItem"
        resources.ApplyResources(Me.AboutToolStripMenuItem, "AboutToolStripMenuItem")
        '
        'WindowsMenu
        '
        Me.WindowsMenu.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.CloseToolStripMenuItem, Me.CloseAllToolStripMenuItem})
        Me.WindowsMenu.Name = "WindowsMenu"
        resources.ApplyResources(Me.WindowsMenu, "WindowsMenu")
        '
        'CloseToolStripMenuItem
        '
        Me.CloseToolStripMenuItem.Name = "CloseToolStripMenuItem"
        resources.ApplyResources(Me.CloseToolStripMenuItem, "CloseToolStripMenuItem")
        '
        'CloseAllToolStripMenuItem
        '
        Me.CloseAllToolStripMenuItem.Name = "CloseAllToolStripMenuItem"
        resources.ApplyResources(Me.CloseAllToolStripMenuItem, "CloseAllToolStripMenuItem")
        '
        'OptionsToolStripMenuItem
        '
        Me.OptionsToolStripMenuItem.Image = Global.ScientificInterface.My.Resources.Resources.OptionsHS
        Me.OptionsToolStripMenuItem.Name = "OptionsToolStripMenuItem"
        resources.ApplyResources(Me.OptionsToolStripMenuItem, "OptionsToolStripMenuItem")
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
        'ExitToolStripMenuItem
        '
        Me.ExitToolStripMenuItem.Name = "ExitToolStripMenuItem"
        resources.ApplyResources(Me.ExitToolStripMenuItem, "ExitToolStripMenuItem")
        '
        'FileMenu
        '
        Me.FileMenu.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.NewFileToolStripMenuItem, Me.OpenFileToolStripMenuItem, Me.ToolStripSeparator5, Me.CloseModelToolStripMenuItem, Me.ToolStripSeparator1, Me.SaveModelToolStripMenuItem, Me.SaveModelAsToolStripMenuItem, Me.ToolStripSeparator9, Me.RecentMDBToolStripMenuItem, Me.ToolStripSeparator4, Me.ExitToolStripMenuItem})
        resources.ApplyResources(Me.FileMenu, "FileMenu")
        Me.FileMenu.Name = "FileMenu"
        '
        'NewFileToolStripMenuItem
        '
        Me.NewFileToolStripMenuItem.Image = Global.ScientificInterface.My.Resources.Resources.NewDocumentHS
        Me.NewFileToolStripMenuItem.Name = "NewFileToolStripMenuItem"
        resources.ApplyResources(Me.NewFileToolStripMenuItem, "NewFileToolStripMenuItem")
        '
        'OpenFileToolStripMenuItem
        '
        Me.OpenFileToolStripMenuItem.Image = Global.ScientificInterface.My.Resources.Resources.openHS
        resources.ApplyResources(Me.OpenFileToolStripMenuItem, "OpenFileToolStripMenuItem")
        Me.OpenFileToolStripMenuItem.Name = "OpenFileToolStripMenuItem"
        '
        'ToolStripSeparator5
        '
        Me.ToolStripSeparator5.Name = "ToolStripSeparator5"
        resources.ApplyResources(Me.ToolStripSeparator5, "ToolStripSeparator5")
        '
        'CloseModelToolStripMenuItem
        '
        Me.CloseModelToolStripMenuItem.Name = "CloseModelToolStripMenuItem"
        resources.ApplyResources(Me.CloseModelToolStripMenuItem, "CloseModelToolStripMenuItem")
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        resources.ApplyResources(Me.ToolStripSeparator1, "ToolStripSeparator1")
        '
        'SaveModelToolStripMenuItem
        '
        Me.SaveModelToolStripMenuItem.Image = Global.ScientificInterface.My.Resources.Resources.saveHS
        Me.SaveModelToolStripMenuItem.Name = "SaveModelToolStripMenuItem"
        resources.ApplyResources(Me.SaveModelToolStripMenuItem, "SaveModelToolStripMenuItem")
        '
        'SaveModelAsToolStripMenuItem
        '
        Me.SaveModelAsToolStripMenuItem.Name = "SaveModelAsToolStripMenuItem"
        resources.ApplyResources(Me.SaveModelAsToolStripMenuItem, "SaveModelAsToolStripMenuItem")
        '
        'ToolStripSeparator9
        '
        Me.ToolStripSeparator9.Name = "ToolStripSeparator9"
        resources.ApplyResources(Me.ToolStripSeparator9, "ToolStripSeparator9")
        '
        'RecentMDBToolStripMenuItem
        '
        Me.RecentMDBToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.NoneToolStripMenuItem})
        Me.RecentMDBToolStripMenuItem.Name = "RecentMDBToolStripMenuItem"
        resources.ApplyResources(Me.RecentMDBToolStripMenuItem, "RecentMDBToolStripMenuItem")
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
        Me.m_menuMain.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.FileMenu, Me.ViewMenu, Me.EcopathToolStripMenuItem, Me.EcosimToolStripMenuItem, Me.EcospaceToolStripMenuItem, Me.ToolsMenu, Me.WindowsMenu, Me.HelpMenu})
        resources.ApplyResources(Me.m_menuMain, "m_menuMain")
        Me.m_menuMain.MdiWindowListItem = Me.WindowsMenu
        Me.m_menuMain.Name = "m_menuMain"
        '
        'ViewMenu
        '
        Me.ViewMenu.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.StartPageToolStripMenuItem, Me.NavigationPanelToolStripMenuItem, Me.StatusPanelToolStripMenuItem, Me.SelectionPanelToolStripMenuItem, Me.ToolStripSeparator10, Me.ModelBarToolStripMenuItem, Me.StatusBarToolStripMenuItem})
        Me.ViewMenu.Name = "ViewMenu"
        resources.ApplyResources(Me.ViewMenu, "ViewMenu")
        '
        'StartPageToolStripMenuItem
        '
        Me.StartPageToolStripMenuItem.Name = "StartPageToolStripMenuItem"
        resources.ApplyResources(Me.StartPageToolStripMenuItem, "StartPageToolStripMenuItem")
        '
        'NavigationPanelToolStripMenuItem
        '
        Me.NavigationPanelToolStripMenuItem.Name = "NavigationPanelToolStripMenuItem"
        resources.ApplyResources(Me.NavigationPanelToolStripMenuItem, "NavigationPanelToolStripMenuItem")
        '
        'StatusPanelToolStripMenuItem
        '
        Me.StatusPanelToolStripMenuItem.Name = "StatusPanelToolStripMenuItem"
        resources.ApplyResources(Me.StatusPanelToolStripMenuItem, "StatusPanelToolStripMenuItem")
        '
        'SelectionPanelToolStripMenuItem
        '
        Me.SelectionPanelToolStripMenuItem.Name = "SelectionPanelToolStripMenuItem"
        resources.ApplyResources(Me.SelectionPanelToolStripMenuItem, "SelectionPanelToolStripMenuItem")
        '
        'ToolStripSeparator10
        '
        Me.ToolStripSeparator10.Name = "ToolStripSeparator10"
        resources.ApplyResources(Me.ToolStripSeparator10, "ToolStripSeparator10")
        '
        'ModelBarToolStripMenuItem
        '
        Me.ModelBarToolStripMenuItem.Name = "ModelBarToolStripMenuItem"
        resources.ApplyResources(Me.ModelBarToolStripMenuItem, "ModelBarToolStripMenuItem")
        '
        'StatusBarToolStripMenuItem
        '
        Me.StatusBarToolStripMenuItem.Checked = True
        Me.StatusBarToolStripMenuItem.CheckOnClick = True
        Me.StatusBarToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked
        Me.StatusBarToolStripMenuItem.Name = "StatusBarToolStripMenuItem"
        resources.ApplyResources(Me.StatusBarToolStripMenuItem, "StatusBarToolStripMenuItem")
        '
        'EcopathToolStripMenuItem
        '
        Me.EcopathToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.EditGroupsToolStripMenuItem, Me.EditMultiStanzaGroupsToolStripMenuItem1, Me.EditFleetsToolStripMenuItem})
        Me.EcopathToolStripMenuItem.Name = "EcopathToolStripMenuItem"
        resources.ApplyResources(Me.EcopathToolStripMenuItem, "EcopathToolStripMenuItem")
        '
        'EditGroupsToolStripMenuItem
        '
        Me.EditGroupsToolStripMenuItem.Image = Global.ScientificInterface.My.Resources.Resources.EditGroup
        Me.EditGroupsToolStripMenuItem.Name = "EditGroupsToolStripMenuItem"
        resources.ApplyResources(Me.EditGroupsToolStripMenuItem, "EditGroupsToolStripMenuItem")
        '
        'EditMultiStanzaGroupsToolStripMenuItem1
        '
        Me.EditMultiStanzaGroupsToolStripMenuItem1.Image = Global.ScientificInterface.My.Resources.Resources.EditMultiStanza
        Me.EditMultiStanzaGroupsToolStripMenuItem1.Name = "EditMultiStanzaGroupsToolStripMenuItem1"
        resources.ApplyResources(Me.EditMultiStanzaGroupsToolStripMenuItem1, "EditMultiStanzaGroupsToolStripMenuItem1")
        '
        'EditFleetsToolStripMenuItem
        '
        Me.EditFleetsToolStripMenuItem.Image = Global.ScientificInterface.My.Resources.Resources.EditFleets
        Me.EditFleetsToolStripMenuItem.Name = "EditFleetsToolStripMenuItem"
        resources.ApplyResources(Me.EditFleetsToolStripMenuItem, "EditFleetsToolStripMenuItem")
        '
        'EcosimToolStripMenuItem
        '
        Me.EcosimToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.NewEcosimScenarioToolStripMenuItem, Me.LoadEcosimScenarioToolStripMenuItem, Me.ToolStripSeparator2, Me.SaveEcosimScenarioToolStripMenuItem, Me.SaveEcosimScenarioAsToolStripMenuItem, Me.ToolStripSeparator6, Me.ImportTimeSeriesToolStripMenuItem, Me.LoadTimeSeriesToolStripMenuItem, Me.WeightTimeSeriesToolStripMenuItem, Me.LoadAndApplyLastTimeSeriesToolStripMenuItem, Me.ToolStripSeparator3, Me.ExportBiomassToFileToolStripMenuItem})
        Me.EcosimToolStripMenuItem.Name = "EcosimToolStripMenuItem"
        resources.ApplyResources(Me.EcosimToolStripMenuItem, "EcosimToolStripMenuItem")
        '
        'NewEcosimScenarioToolStripMenuItem
        '
        Me.NewEcosimScenarioToolStripMenuItem.Image = Global.ScientificInterface.My.Resources.Resources.NewDocumentHS
        Me.NewEcosimScenarioToolStripMenuItem.Name = "NewEcosimScenarioToolStripMenuItem"
        resources.ApplyResources(Me.NewEcosimScenarioToolStripMenuItem, "NewEcosimScenarioToolStripMenuItem")
        '
        'LoadEcosimScenarioToolStripMenuItem
        '
        Me.LoadEcosimScenarioToolStripMenuItem.Image = Global.ScientificInterface.My.Resources.Resources.openHS
        Me.LoadEcosimScenarioToolStripMenuItem.Name = "LoadEcosimScenarioToolStripMenuItem"
        resources.ApplyResources(Me.LoadEcosimScenarioToolStripMenuItem, "LoadEcosimScenarioToolStripMenuItem")
        '
        'ToolStripSeparator2
        '
        Me.ToolStripSeparator2.Name = "ToolStripSeparator2"
        resources.ApplyResources(Me.ToolStripSeparator2, "ToolStripSeparator2")
        '
        'SaveEcosimScenarioToolStripMenuItem
        '
        Me.SaveEcosimScenarioToolStripMenuItem.Image = Global.ScientificInterface.My.Resources.Resources.saveHS
        Me.SaveEcosimScenarioToolStripMenuItem.Name = "SaveEcosimScenarioToolStripMenuItem"
        resources.ApplyResources(Me.SaveEcosimScenarioToolStripMenuItem, "SaveEcosimScenarioToolStripMenuItem")
        '
        'SaveEcosimScenarioAsToolStripMenuItem
        '
        Me.SaveEcosimScenarioAsToolStripMenuItem.Name = "SaveEcosimScenarioAsToolStripMenuItem"
        resources.ApplyResources(Me.SaveEcosimScenarioAsToolStripMenuItem, "SaveEcosimScenarioAsToolStripMenuItem")
        '
        'ToolStripSeparator6
        '
        Me.ToolStripSeparator6.Name = "ToolStripSeparator6"
        resources.ApplyResources(Me.ToolStripSeparator6, "ToolStripSeparator6")
        '
        'ImportTimeSeriesToolStripMenuItem
        '
        Me.ImportTimeSeriesToolStripMenuItem.Image = Global.ScientificInterface.My.Resources.Resources.ImportXMLHS
        Me.ImportTimeSeriesToolStripMenuItem.Name = "ImportTimeSeriesToolStripMenuItem"
        resources.ApplyResources(Me.ImportTimeSeriesToolStripMenuItem, "ImportTimeSeriesToolStripMenuItem")
        '
        'LoadTimeSeriesToolStripMenuItem
        '
        Me.LoadTimeSeriesToolStripMenuItem.Name = "LoadTimeSeriesToolStripMenuItem"
        resources.ApplyResources(Me.LoadTimeSeriesToolStripMenuItem, "LoadTimeSeriesToolStripMenuItem")
        '
        'WeightTimeSeriesToolStripMenuItem
        '
        Me.WeightTimeSeriesToolStripMenuItem.Name = "WeightTimeSeriesToolStripMenuItem"
        resources.ApplyResources(Me.WeightTimeSeriesToolStripMenuItem, "WeightTimeSeriesToolStripMenuItem")
        '
        'LoadAndApplyLastTimeSeriesToolStripMenuItem
        '
        Me.LoadAndApplyLastTimeSeriesToolStripMenuItem.Image = Global.ScientificInterface.My.Resources.Resources.NavForward
        Me.LoadAndApplyLastTimeSeriesToolStripMenuItem.Name = "LoadAndApplyLastTimeSeriesToolStripMenuItem"
        resources.ApplyResources(Me.LoadAndApplyLastTimeSeriesToolStripMenuItem, "LoadAndApplyLastTimeSeriesToolStripMenuItem")
        '
        'ToolStripSeparator3
        '
        Me.ToolStripSeparator3.Name = "ToolStripSeparator3"
        resources.ApplyResources(Me.ToolStripSeparator3, "ToolStripSeparator3")
        '
        'ExportBiomassToFileToolStripMenuItem
        '
        Me.ExportBiomassToFileToolStripMenuItem.Name = "ExportBiomassToFileToolStripMenuItem"
        resources.ApplyResources(Me.ExportBiomassToFileToolStripMenuItem, "ExportBiomassToFileToolStripMenuItem")
        '
        'EcospaceToolStripMenuItem
        '
        Me.EcospaceToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.NewEcospaceScenarioToolStripMenuItem, Me.LoadEcospaceScenarioToolStripMenuItem, Me.ToolStripSeparator7, Me.SaveEcospaceScenarioToolStripMenuItem, Me.SaveEcospaceScenarioAsToolStripMenuItem, Me.ToolStripSeparator11, Me.EditBasemapToolStripMenuItem, Me.EditHabitatsToolStripMenuItem, Me.EditMPAsToolStripMenuItem, Me.EditRegionsToolStripMenuItem, Me.ToolStripSeparator16, Me.EditImportanceLayersToolStripMenuItem, Me.ToolStripSeparator17, Me.ImportLayerDataToolStripMenuItem})
        Me.EcospaceToolStripMenuItem.Name = "EcospaceToolStripMenuItem"
        resources.ApplyResources(Me.EcospaceToolStripMenuItem, "EcospaceToolStripMenuItem")
        '
        'NewEcospaceScenarioToolStripMenuItem
        '
        Me.NewEcospaceScenarioToolStripMenuItem.Image = Global.ScientificInterface.My.Resources.Resources.NewDocumentHS
        Me.NewEcospaceScenarioToolStripMenuItem.Name = "NewEcospaceScenarioToolStripMenuItem"
        resources.ApplyResources(Me.NewEcospaceScenarioToolStripMenuItem, "NewEcospaceScenarioToolStripMenuItem")
        '
        'LoadEcospaceScenarioToolStripMenuItem
        '
        Me.LoadEcospaceScenarioToolStripMenuItem.Image = Global.ScientificInterface.My.Resources.Resources.openHS
        Me.LoadEcospaceScenarioToolStripMenuItem.Name = "LoadEcospaceScenarioToolStripMenuItem"
        resources.ApplyResources(Me.LoadEcospaceScenarioToolStripMenuItem, "LoadEcospaceScenarioToolStripMenuItem")
        '
        'ToolStripSeparator7
        '
        Me.ToolStripSeparator7.Name = "ToolStripSeparator7"
        resources.ApplyResources(Me.ToolStripSeparator7, "ToolStripSeparator7")
        '
        'SaveEcospaceScenarioToolStripMenuItem
        '
        Me.SaveEcospaceScenarioToolStripMenuItem.Image = Global.ScientificInterface.My.Resources.Resources.saveHS
        Me.SaveEcospaceScenarioToolStripMenuItem.Name = "SaveEcospaceScenarioToolStripMenuItem"
        resources.ApplyResources(Me.SaveEcospaceScenarioToolStripMenuItem, "SaveEcospaceScenarioToolStripMenuItem")
        '
        'SaveEcospaceScenarioAsToolStripMenuItem
        '
        Me.SaveEcospaceScenarioAsToolStripMenuItem.Name = "SaveEcospaceScenarioAsToolStripMenuItem"
        resources.ApplyResources(Me.SaveEcospaceScenarioAsToolStripMenuItem, "SaveEcospaceScenarioAsToolStripMenuItem")
        '
        'ToolStripSeparator11
        '
        Me.ToolStripSeparator11.Name = "ToolStripSeparator11"
        resources.ApplyResources(Me.ToolStripSeparator11, "ToolStripSeparator11")
        '
        'EditBasemapToolStripMenuItem
        '
        Me.EditBasemapToolStripMenuItem.Image = Global.ScientificInterface.My.Resources.Resources.Raster1
        Me.EditBasemapToolStripMenuItem.Name = "EditBasemapToolStripMenuItem"
        resources.ApplyResources(Me.EditBasemapToolStripMenuItem, "EditBasemapToolStripMenuItem")
        '
        'EditHabitatsToolStripMenuItem
        '
        Me.EditHabitatsToolStripMenuItem.Image = Global.ScientificInterface.My.Resources.Resources.Habitat1
        Me.EditHabitatsToolStripMenuItem.Name = "EditHabitatsToolStripMenuItem"
        resources.ApplyResources(Me.EditHabitatsToolStripMenuItem, "EditHabitatsToolStripMenuItem")
        '
        'EditMPAsToolStripMenuItem
        '
        Me.EditMPAsToolStripMenuItem.Image = Global.ScientificInterface.My.Resources.Resources.MPA1
        Me.EditMPAsToolStripMenuItem.Name = "EditMPAsToolStripMenuItem"
        resources.ApplyResources(Me.EditMPAsToolStripMenuItem, "EditMPAsToolStripMenuItem")
        '
        'EditRegionsToolStripMenuItem
        '
        Me.EditRegionsToolStripMenuItem.Image = Global.ScientificInterface.My.Resources.Resources.Regions
        Me.EditRegionsToolStripMenuItem.Name = "EditRegionsToolStripMenuItem"
        resources.ApplyResources(Me.EditRegionsToolStripMenuItem, "EditRegionsToolStripMenuItem")
        '
        'ToolStripSeparator16
        '
        Me.ToolStripSeparator16.Name = "ToolStripSeparator16"
        resources.ApplyResources(Me.ToolStripSeparator16, "ToolStripSeparator16")
        '
        'EditImportanceLayersToolStripMenuItem
        '
        Me.EditImportanceLayersToolStripMenuItem.Name = "EditImportanceLayersToolStripMenuItem"
        resources.ApplyResources(Me.EditImportanceLayersToolStripMenuItem, "EditImportanceLayersToolStripMenuItem")
        '
        'ToolStripSeparator17
        '
        Me.ToolStripSeparator17.Name = "ToolStripSeparator17"
        resources.ApplyResources(Me.ToolStripSeparator17, "ToolStripSeparator17")
        '
        'ImportLayerDataToolStripMenuItem
        '
        Me.ImportLayerDataToolStripMenuItem.Name = "ImportLayerDataToolStripMenuItem"
        resources.ApplyResources(Me.ImportLayerDataToolStripMenuItem, "ImportLayerDataToolStripMenuItem")
        '
        'ToolsMenu
        '
        Me.ToolsMenu.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.OptionsToolStripMenuItem, Me.ToolStripSeparator15, Me.EcotracerToolStripMenuItem})
        Me.ToolsMenu.Name = "ToolsMenu"
        resources.ApplyResources(Me.ToolsMenu, "ToolsMenu")
        '
        'ToolStripSeparator15
        '
        Me.ToolStripSeparator15.Name = "ToolStripSeparator15"
        resources.ApplyResources(Me.ToolStripSeparator15, "ToolStripSeparator15")
        '
        'EcotracerToolStripMenuItem
        '
        Me.EcotracerToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.NewTracerScenarioToolStripMenuItem, Me.LoadTracerScenarioToolStripMenuItem, Me.ToolStripSeparator14, Me.SaveTracerScenarioToolStripMenuItem, Me.SaveTracerScenarioAsToolStripMenuItem})
        Me.EcotracerToolStripMenuItem.Image = Global.ScientificInterface.My.Resources.Resources.Ecotracer_32x32
        Me.EcotracerToolStripMenuItem.Name = "EcotracerToolStripMenuItem"
        resources.ApplyResources(Me.EcotracerToolStripMenuItem, "EcotracerToolStripMenuItem")
        '
        'NewTracerScenarioToolStripMenuItem
        '
        Me.NewTracerScenarioToolStripMenuItem.Image = Global.ScientificInterface.My.Resources.Resources.NewDocumentHS
        Me.NewTracerScenarioToolStripMenuItem.Name = "NewTracerScenarioToolStripMenuItem"
        resources.ApplyResources(Me.NewTracerScenarioToolStripMenuItem, "NewTracerScenarioToolStripMenuItem")
        '
        'LoadTracerScenarioToolStripMenuItem
        '
        Me.LoadTracerScenarioToolStripMenuItem.Image = Global.ScientificInterface.My.Resources.Resources.openHS
        Me.LoadTracerScenarioToolStripMenuItem.Name = "LoadTracerScenarioToolStripMenuItem"
        resources.ApplyResources(Me.LoadTracerScenarioToolStripMenuItem, "LoadTracerScenarioToolStripMenuItem")
        '
        'ToolStripSeparator14
        '
        Me.ToolStripSeparator14.Name = "ToolStripSeparator14"
        resources.ApplyResources(Me.ToolStripSeparator14, "ToolStripSeparator14")
        '
        'SaveTracerScenarioToolStripMenuItem
        '
        Me.SaveTracerScenarioToolStripMenuItem.Image = Global.ScientificInterface.My.Resources.Resources.saveHS
        Me.SaveTracerScenarioToolStripMenuItem.Name = "SaveTracerScenarioToolStripMenuItem"
        resources.ApplyResources(Me.SaveTracerScenarioToolStripMenuItem, "SaveTracerScenarioToolStripMenuItem")
        '
        'SaveTracerScenarioAsToolStripMenuItem
        '
        Me.SaveTracerScenarioAsToolStripMenuItem.Name = "SaveTracerScenarioAsToolStripMenuItem"
        resources.ApplyResources(Me.SaveTracerScenarioAsToolStripMenuItem, "SaveTracerScenarioAsToolStripMenuItem")
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
    Friend WithEvents ContentsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents HelpMenu As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents IndexToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents SearchToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator8 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents AboutToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ArrangeIconsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents WindowsMenu As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents CascadeToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents TileVerticalToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents TileHorizontalToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents OptionsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_tsStatus As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents m_ssMain As System.Windows.Forms.StatusStrip
    Friend WithEvents ExitToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents FileMenu As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_menuMain As System.Windows.Forms.MenuStrip
    Friend WithEvents ViewMenu As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolBarToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents StatusBarToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolsMenu As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents OpenFileToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator9 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ToolStripSeparator10 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents CloseAllToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents EcopathToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents EditGroupsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents EcosimToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents LoadEcosimScenarioToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents SaveEcosimScenarioToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents SelectionPanelToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents StartPageToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents NavigationPanelToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents StatusPanelToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator3 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ImportTimeSeriesToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ExportBiomassToFileToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents RecentMDBToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator4 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents NoneToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator5 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents CloseModelToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents NewFileToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_tsModel As System.Windows.Forms.ToolStrip
    Friend WithEvents m_tsEcopathModel As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents m_tsEcosimScenario As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents m_tsEcospaceScenario As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents EditMultiStanzaGroupsToolStripMenuItem1 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents EditFleetsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents EcospaceToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents LoadEcospaceScenarioToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents SaveEcospaceScenarioToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents CloseToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents SaveModelAsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents SaveModelToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents SaveEcosimScenarioAsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents SaveEcospaceScenarioAsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents NewEcosimScenarioToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator2 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ToolStripSeparator6 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents NewEcospaceScenarioToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator7 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ToolStripSeparator11 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents EditHabitatsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents EditMPAsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents EditRegionsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents EditBasemapToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ModelBarToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator12 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ReportBugMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents LoadTimeSeriesToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator13 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents EcotracerToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents LoadTracerScenarioToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents SaveTracerScenarioToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents SaveTracerScenarioAsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents NewTracerScenarioToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator14 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ToolStripSeparator15 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents m_tsEcotracerScenario As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents m_tsSelection As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents LoadAndApplyLastTimeSeriesToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_tsbProgress As System.Windows.Forms.ToolStripProgressBar
    Private WithEvents m_tsbModel As System.Windows.Forms.ToolStripButton
    Private WithEvents m_tsbEcopath As System.Windows.Forms.ToolStripButton
    Private WithEvents m_tsbEcosim As System.Windows.Forms.ToolStripSplitButton
    Private WithEvents m_tsbEcospace As System.Windows.Forms.ToolStripSplitButton
    Friend WithEvents ToolStripSeparator16 As System.Windows.Forms.ToolStripSeparator
    Private WithEvents WeightTimeSeriesToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator17 As System.Windows.Forms.ToolStripSeparator
    Private WithEvents ImportLayerDataToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents EditImportanceLayersToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem

End Class

