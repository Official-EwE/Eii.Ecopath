#Region " Imports "

Option Explicit On
Option Strict On

Imports System.IO
Imports System.Resources
Imports System.Reflection
Imports EwEPlugin
Imports EwECore
Imports EwECore.DataSources
Imports EwECore.Database
Imports ScientificInterface.Ecopath
Imports ScientificInterface.Ecosim
Imports ScientificInterface.Ecospace
Imports ScientificInterface.Ecospace.Basemap
Imports ScientificInterface.Ecospace.Basemap.Layers
Imports ScientificInterface.Ecotracer
Imports ScientificInterface.Wizard
Imports ScientificInterface.Other
Imports ScientificInterfaceShared
Imports ScientificInterfaceShared.Forms
Imports ScientificInterfaceShared.Controls
Imports ScientificInterfaceShared.Controls.EwEGrid
Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports WeifenLuo.WinFormsUI.Docking
Imports EwEUtils.Commands
Imports EwEUtils.Core
Imports EwEUtils.Database
Imports EwEUtils.Utilities
Imports Microsoft.VisualBasic
Imports System.Threading

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' The main form of the EwE6 Scientific Interface
''' </summary>
''' ---------------------------------------------------------------------------
Public Class AppLauncher
    Implements IApplicationStatusDispatcher
    Implements IUIElement

#Region " Variables "

    Private m_uic As cUIContext = Nothing
    Private m_mhEcosim As cMessageHandler = Nothing
    Private m_mhEcospace As cMessageHandler = Nothing
    Private m_mhEcotracer As cMessageHandler = Nothing
    Private m_mhTimeseries As cMessageHandler = Nothing

    Private m_pluginManager As cPluginManager = Nothing
    Private m_pluginMenuHandler As cPluginMenuHandler = Nothing
    Private m_coreController As cCoreController = Nothing
    Private m_FormStateHelper As cEwEFormStateHelper = Nothing
    ''' <summary>Style guide updater.</summary>
    Private m_styleguideupdater As StyleGuideUpdater = Nothing
    Private m_applictionStatusNotifier As cApplicationStatusNotifier = Nothing

    Private m_strLastSelectedPath As String = ""

    ''' <summary>Status messages stack.</summary>
    Private m_lstrStatus As New List(Of String)

#Region " Panels "

    Private m_DockPanel As DockPanel = Nothing
    Private m_NavPanel As NavigationPanel = Nothing
    Private m_StatusPanel As StatusPanel = Nothing
    Private m_RemarkPanel As RemarkPanel = Nothing
    Private m_StartPage As frmWebBrowser = Nothing
    Private m_lstrProtectedPanelNames As New List(Of String)

#End Region ' Panels

#Region " Presentation mode "

    Private Structure sFormStatePrevious
        Public ShowMenu As Boolean
        Public ShowModelBar As Boolean
        Public ShowStatusBar As Boolean
        Public ShowNavPanel As Boolean
        Public FormState As FormWindowState
        Public BorderStyle As FormBorderStyle
    End Structure

    Private m_fspPresentationMode As New sFormStatePrevious()

#End Region ' Presentation mode

#Region " Commands "

    Private WithEvents m_cmdFileOpen As cFileOpenCommand = Nothing
    Private WithEvents m_cmdFileSave As cFileSaveCommand = Nothing
    Private WithEvents m_cmdDirectoryOpen As cDirectoryOpenCommand = Nothing
    Private WithEvents m_cmdExecute As cExecuteCommand = Nothing
    Private WithEvents m_cmdNewModel As cCommand = Nothing
    Private WithEvents m_cmdLoadModel As cCommand = Nothing
    Private WithEvents m_cmdSave As cCommand = Nothing
    Private WithEvents m_cmdSaveModelAs As cCommand = Nothing
    Private WithEvents m_cmdCloseModel As cCommand = Nothing
    Private WithEvents m_cmdCompactModel As cCommand = Nothing
    Private WithEvents m_cmdCloseDocument As cCommand = Nothing
    Private WithEvents m_cmdNewEcosimScenario As cCommand = Nothing
    Private WithEvents m_cmdLoadEcosimScenario As cCommand = Nothing
    'Private WithEvents m_cmdSaveEcosimScenario As cCommand = Nothing
    Private WithEvents m_cmdSaveEcosimScenarioAs As cCommand = Nothing
    Private WithEvents m_cmdDeleteEcosimScenario As cCommand = Nothing
    Private WithEvents m_cmdNewEcospaceScenario As cCommand = Nothing
    Private WithEvents m_cmdLoadEcospaceScenario As cCommand = Nothing
    'Private WithEvents m_cmdSaveEcospaceScenario As cCommand = Nothing
    Private WithEvents m_cmdSaveEcospaceScenarioAS As cCommand = Nothing
    Private WithEvents m_cmdDeleteEcospaceScenario As cCommand = Nothing
    Private WithEvents m_cmdNewEcotracerScenario As cCommand = Nothing
    Private WithEvents m_cmdLoadEcotracerScenario As cCommand = Nothing
    'Private WithEvents m_cmdSaveEcotracerScenario As cCommand = Nothing
    Private WithEvents m_cmdSaveEcotracerScenarioAS As cCommand = Nothing
    Private WithEvents m_cmdDeleteEcotracerScenario As cCommand = Nothing
    Private WithEvents m_cmdCloseAllForms As cCommand = Nothing
    Private WithEvents m_cmdNavigate As cNavigationCommand = Nothing
    Private WithEvents m_cmdViewNavPane As cCommand = Nothing
    Private WithEvents m_cmdViewStatusPane As cCommand = Nothing
    Private WithEvents m_cmdViewStartPanel As cCommand = Nothing
    Private WithEvents m_cmdViewRemarkPane As cCommand = Nothing
    Private WithEvents m_cmdViewMenu As cCommand = Nothing
    Private WithEvents m_cmdViewModelBar As cCommand = Nothing
    Private WithEvents m_cmdViewStatusbar As cCommand = Nothing
    Private WithEvents m_cmdViewPresentationMode As cCommand = Nothing
    Private WithEvents m_cmdEditGroups As cCommand = Nothing
    Private WithEvents m_cmdEditMultiStanza As cCommand = Nothing
    Private WithEvents m_cmdEditFleets As cCommand = Nothing
    Private WithEvents m_cmdEditTaxa As cCommand = Nothing
    Private WithEvents m_cmdEditPedigree As cCommand = Nothing
    Private WithEvents m_cmdEditBasemap As cCommand = Nothing
    Private WithEvents m_cmdEditHabitats As cCommand = Nothing
    Private WithEvents m_cmdEditRegions As cCommand = Nothing
    Private WithEvents m_cmdEditMPAs As cCommand = Nothing
    Private WithEvents m_cmdEditImportanceLayers As cCommand = Nothing
    Private WithEvents m_cmdImportLayerData As cCommand = Nothing
    Private WithEvents m_cmdExportLayerData As cCommand = Nothing
    Private WithEvents m_cmdImportTimeSeries As cCommand = Nothing
    Private WithEvents m_cmdLoadTimeSeries As cCommand = Nothing
    Private WithEvents m_cmdWeightTimeSeries As cCommand = Nothing
    Private WithEvents m_cmdExportTimeSeries As cCommand = Nothing
    Private WithEvents m_cmdPluginGUICommand As cPluginGUICommand = Nothing
    Private WithEvents m_cmdHelpAbout As cCommand = Nothing
    Private WithEvents m_cmdPropertySelection As cPropertySelectionCommand = Nothing
    Private WithEvents m_cmdShowHideItems As cDisplayGroupsCommand = Nothing
    Private WithEvents m_cmdEnableEcotracer As cCommand = Nothing
    Private WithEvents m_cmdEstimateVs As cCommand = Nothing
    ' ToDo_JS: Discontinue, move to Ecosim UI
    Private WithEvents m_cmdExportEcosimResultsToCSV As cCommand = Nothing

#End Region ' Commands

    ''' <summary>
    ''' Enumerated type, states how a database was loaded.
    ''' </summary>
    Private Enum eLoadSourceType As Integer
        ''' <summary>Database open attempt originated from the internal API.</summary>
        API = 0
        ''' <summary>Database open attempt originated from the command line.</summary>
        CommandLine
        ''' <summary>Database open attempt originated from the MRU list.</summary>
        MRU
        ''' <summary>Database open attempt originated from the user interface.</summary>
        User
    End Enum

#End Region ' Variables

#Region " Singleton "

    Private Shared __inst__ As AppLauncher = Nothing

    Public Shared Function GetInstance() As AppLauncher
        Return AppLauncher.__inst__
    End Function

#End Region ' Singleton

#Region " Constructors "

    Public Sub New()

        Me.InitializeComponent()

        Debug.Assert(AppLauncher.__inst__ Is Nothing, "Only one instance of AppLauncher allowed")
        AppLauncher.__inst__ = Me

        Me.m_applictionStatusNotifier = New cApplicationStatusNotifier(Me)

#If Not Debug Then
        ' Remove estimate V's from release version while under development
        Me.m_tsmiEcosimEstimateVs.Visible = False
#End If

    End Sub

#End Region ' Constructors

#Region " IUIElement implementation "

    Public Property UIContext() As cUIContext _
        Implements IUIElement.UIContext
        Get
            Return Me.m_uic
        End Get
        Private Set(ByVal value As cUIContext)
            Me.m_uic = value
        End Set
    End Property

    Public ReadOnly Property Core() As cCore
        Get
            Return Me.m_uic.Core
        End Get
    End Property

    Public ReadOnly Property CoreController() As cCoreController
        Get
            Return Me.m_coreController
        End Get
    End Property

    Public ReadOnly Property StyleGuide() As cStyleGuide
        Get
            Return Me.m_uic.StyleGuide
        End Get
    End Property

    Public ReadOnly Property Help() As cHelp
        Get
            Return Me.m_uic.Help
        End Get
    End Property

#End Region ' IUIElement implementation

#Region " Initialization "

    Private Sub ProcessCommandLine()

        Dim astrCmd As String() = cStringUtils.SplitQualified(Microsoft.VisualBasic.Command(), " ")

        If (astrCmd.Length > 0) Then
            If Not String.IsNullOrEmpty(astrCmd(0)) Then
                ' Open the model
                Me.LoadEcopathModel(astrCmd(0).Replace("""", ""), eLoadSourceType.CommandLine)
            End If
        End If

    End Sub

    Private Sub InitCommands()

        Dim cmdh As cCommandHandler = Me.m_uic.CommandHandler

        ' Create and configure File Open command
        Me.m_cmdFileOpen = New cFileOpenCommand(cmdh)

        ' Create and configure File Save command
        Me.m_cmdFileSave = New cFileSaveCommand(cmdh)

        ' Create and configure Directory Open command
        Me.m_cmdDirectoryOpen = New cDirectoryOpenCommand(cmdh)

        ' Create and configure Execute command
        Me.m_cmdExecute = New cExecuteCommand(cmdh)

        ' Create and configure new command
        Me.m_cmdNewModel = New cCommand(cmdh, "NewEcopathModel")
        Me.m_cmdNewModel.AddControl(Me.m_tsmiFileNew)

        ' Create and configure open command
        Me.m_cmdLoadModel = New cCommand(cmdh, "LoadEcopathModel")
        Me.m_cmdLoadModel.AddControl(Me.m_tsmiFileOpen)
        Me.m_cmdLoadModel.AddControl(Me.m_tsbEcopath)

        Me.m_cmdSave = New cCommand(cmdh, "SaveModel")
        Me.m_cmdSave.AddControl(Me.m_tsmiFileSave)
        Me.m_cmdSave.AddControl(Me.m_tsbSave)

        ' Create and configure save commands
        Me.m_cmdSaveModelAs = New cCommand(cmdh, "SaveModelAs")
        Me.m_cmdSaveModelAs.AddControl(Me.m_tsmiFileSaveAs)

        ' Create and configure 'close model' command
        Me.m_cmdCloseModel = New cCommand(cmdh, "CloseModel")
        Me.m_cmdCloseModel.AddControl(Me.m_tsmiFileClose)

        ' Create and configure 'compact model' command
        Me.m_cmdCompactModel = New cCommand(cmdh, "CompactModel")
        Me.m_cmdCompactModel.AddControl(Me.m_tsmiFileCompact)

        ' Create and configure 'close document' command
        Me.m_cmdCloseDocument = New cCommand(cmdh, "CloseDocument")
        Me.m_cmdCloseDocument.AddControl(Me.m_tsmiWindowsClose)

        ' Create and configure navigate command
        Me.m_cmdNavigate = New cNavigationCommand(cmdh)

        ' Create and configure 'close all forms' command
        Me.m_cmdCloseAllForms = New cCommand(cmdh, "CloseAllForms")
        Me.m_cmdCloseAllForms.AddControl(Me.m_tsmiWindowsCloseAll)

        'Create and configure 'new ecosim scenario' command
        Me.m_cmdNewEcosimScenario = New cCommand(cmdh, "NewEcosimScenario")
        Me.m_cmdNewEcosimScenario.AddControl(Me.m_tsmiEcosimNew)

        'Create and configure 'load ecosim scenario' command
        Me.m_cmdLoadEcosimScenario = New cCommand(cmdh, "LoadEcosimScenario")
        Me.m_cmdLoadEcosimScenario.AddControl(Me.m_tsmiEcosimLoad)
        Me.m_cmdLoadEcosimScenario.AddControl(Me.m_tsbEcosim)

        ''Create and configure 'save ecosim scenario' command
        'Me.m_cmdSaveEcosimScenario = New cCommand(cmdh, "SaveEcosimScenario")
        'Me.m_cmdSaveEcosimScenario.AddControl(Me.m_tsmiEcosimSave)

        'Create and configure 'save ecosim scenario as' command
        Me.m_cmdSaveEcosimScenarioAs = New cCommand(cmdh, "SaveEcosimScenarioAs")
        Me.m_cmdSaveEcosimScenarioAs.AddControl(Me.m_tsmiEcosimSaveAs)

        'Create and configure 'delete ecosim scenario' command
        Me.m_cmdDeleteEcosimScenario = New cCommand(cmdh, "DeleteEcosimScenarioAs")
        Me.m_cmdDeleteEcosimScenario.AddControl(Me.m_tsmiEcosimDelete)

        'Create and configure 'new ecospace scenario' command
        Me.m_cmdNewEcospaceScenario = New cCommand(cmdh, "NewEcospaceScenario")
        Me.m_cmdNewEcospaceScenario.AddControl(Me.m_tsmiEcospaceNew)

        'Create and configure 'load ecospace scenario' command
        Me.m_cmdLoadEcospaceScenario = New cCommand(cmdh, "LoadEcospaceScenario")
        Me.m_cmdLoadEcospaceScenario.AddControl(Me.m_tsmiEcospaceLoad)
        Me.m_cmdLoadEcospaceScenario.AddControl(Me.m_tsbEcospace)

        ''Create and configure 'save ecospace scenario' command
        'Me.m_cmdSaveEcospaceScenario = New cCommand(cmdh, "SaveEcospaceScenario")
        'Me.m_cmdSaveEcospaceScenario.AddControl(Me.m_tsmiEcospaceSave)

        'Create and configure 'save ecospace scenario as' command
        Me.m_cmdSaveEcospaceScenarioAS = New cCommand(cmdh, "SaveEcospaceScenarioAs")
        Me.m_cmdSaveEcospaceScenarioAS.AddControl(Me.m_tsmiEcospaceSaveAs)

        'Create and configure 'delete ecospace scenario' command
        Me.m_cmdDeleteEcospaceScenario = New cCommand(cmdh, "DeleteEcospaceScenario")
        Me.m_cmdDeleteEcospaceScenario.AddControl(Me.m_tsmiEcospaceDelete)

        'Create and configure 'new ecotracer scenario' command
        Me.m_cmdNewEcotracerScenario = New cCommand(cmdh, "NewEcotracerScenario")
        Me.m_cmdNewEcotracerScenario.AddControl(Me.m_tsmiEcotracerNew)

        'Create and configure 'load ecotracer scenario' command
        Me.m_cmdLoadEcotracerScenario = New cCommand(cmdh, "LoadEcotracerScenario")
        Me.m_cmdLoadEcotracerScenario.AddControl(Me.m_tsmiEcotracerLoad)
        Me.m_cmdLoadEcotracerScenario.AddControl(Me.m_tsbEcotracer)

        ''Create and configure 'save ecotracer scenario' command
        'Me.m_cmdSaveEcotracerScenario = New cCommand(cmdh, "SaveEcotracerScenario")
        'Me.m_cmdSaveEcotracerScenario.AddControl(Me.m_tsmiEcotracerSave)

        'Create and configure 'save ecotracer scenario as' command
        Me.m_cmdSaveEcotracerScenarioAS = New cCommand(cmdh, "SaveEcotracerScenarioAs")
        Me.m_cmdSaveEcotracerScenarioAS.AddControl(Me.m_tsmiEcotracerSaveAs)

        'Create and configure 'delete ecospace scenario' command
        Me.m_cmdDeleteEcotracerScenario = New cCommand(cmdh, "DeleteEcotracerScenario")
        Me.m_cmdDeleteEcotracerScenario.AddControl(Me.m_tsmiEcotracerDelete)

        'Create and configure 'view Navtree' command
        Me.m_cmdViewNavPane = New cCommand(cmdh, "ViewNavPane")
        Me.m_cmdViewNavPane.AddControl(Me.m_tsmiViewNavigation)

        'Create and configure 'view start page' command
        Me.m_cmdViewStartPanel = New cCommand(cmdh, "ViewStartPage")
        Me.m_cmdViewStartPanel.AddControl(Me.m_tsmiViewStartPage)

        'Create and configure 'view status pane' command
        Me.m_cmdViewStatusPane = New cCommand(cmdh, "ViewStatusPane")
        Me.m_cmdViewStatusPane.AddControl(Me.m_tsmiViewStatus)

        'Create and configure 'view properties pane' command
        Me.m_cmdViewRemarkPane = New cCommand(cmdh, "ViewPropertiesPane")
        Me.m_cmdViewRemarkPane.AddControl(Me.m_tsmiViewRemarks)

        'Create and configure 'view menu' command
        Me.m_cmdViewMenu = New cCommand(cmdh, "ViewMenu")
        Me.m_cmdViewMenu.AddControl(Me.m_tsmiViewMenu)

        'Create and configure 'view Buttonbar' command
        Me.m_cmdViewModelBar = New cCommand(cmdh, "ViewModelBar")
        Me.m_cmdViewModelBar.AddControl(Me.m_tsmiViewModelBar)

        'Create and configure 'view statusbar' command
        Me.m_cmdViewStatusbar = New cCommand(cmdh, "ViewStatusbar")
        Me.m_cmdViewStatusbar.AddControl(Me.m_tsmiViewStatusBar)

        'Create and configure 'presentation mode' command
        Me.m_cmdViewPresentationMode = New cCommand(cmdh, "ViewPresentationMode")
        Me.m_cmdViewPresentationMode.AddControl(Me.m_tsmiPresentation)

        'Create and configure EditGroups command
        Me.m_cmdEditGroups = New cCommand(cmdh, "EditGroups")
        Me.m_cmdEditGroups.AddControl(Me.m_tsmiEcopathEditGroups)

        'Create and configure EditMultiStanza cammand
        Me.m_cmdEditMultiStanza = New cCommand(cmdh, "EditMultiStanza")
        Me.m_cmdEditMultiStanza.AddControl(Me.m_tsmiEcopathEditMultiStanza)

        'Create and configure EditFleets command
        Me.m_cmdEditFleets = New cCommand(cmdh, "EditFleets")
        Me.m_cmdEditFleets.AddControl(Me.m_tsmiEcopathEditFleets)

        Me.m_cmdEditPedigree = New cCommand(cmdh, "EditPedigree")
        Me.m_cmdEditPedigree.AddControl(Me.m_tsmiEcopathEditPedigree)

        Me.m_cmdEditTaxa = New cCommand(cmdh, "EditTaxa")
        Me.m_cmdEditTaxa.AddControl(Me.m_tsmiEcopathEditTaxa)

        Me.m_cmdEditBasemap = New cCommand(cmdh, "EditBasemap")
        Me.m_cmdEditBasemap.AddControl(Me.m_tsmiEcospaceEditMap)

        Me.m_cmdEditHabitats = New cCommand(cmdh, "EditHabitats")
        Me.m_cmdEditHabitats.AddControl(Me.m_tsmiEcospaceEditHabitats)

        Me.m_cmdEditRegions = New cCommand(cmdh, "EditRegions")
        Me.m_cmdEditRegions.AddControl(Me.m_tsmiEcospaceEditRegions)

        Me.m_cmdEditMPAs = New cCommand(cmdh, "EditMPAs")
        Me.m_cmdEditMPAs.AddControl(Me.m_tsmiEcospaceEditMPAs)

        Me.m_cmdEditImportanceLayers = New cCommand(cmdh, "EditImportanceLayers")
        Me.m_cmdEditImportanceLayers.AddControl(Me.m_tsmiEcospaceEditImportanceLayers)

        Me.m_cmdImportLayerData = New cCommand(cmdh, "ImportLayerData")
        Me.m_cmdImportLayerData.AddControl(Me.m_tsmiEcospaceImportLayers)

        Me.m_cmdExportLayerData = New cCommand(cmdh, "ExportLayerData")

        'Create and configure ImportTimeSeries command
        Me.m_cmdImportTimeSeries = New cCommand(cmdh, "ImportTimeSeries")
        Me.m_cmdImportTimeSeries.AddControl(Me.m_tsmiTimeSeriesImport)

        'Create and configure LoadTimeSeries command
        Me.m_cmdLoadTimeSeries = New cCommand(cmdh, "LoadTimeSeries")
        Me.m_cmdLoadTimeSeries.AddControl(Me.m_tsmiTimeSeriesLoad)

        'Create and configure WeightTimeSeries command
        Me.m_cmdWeightTimeSeries = New cCommand(cmdh, "WeightTimeSeries")
        Me.m_cmdWeightTimeSeries.AddControl(Me.m_tsmiTimeSeriesEditWeights)

        'Create and configure ExportTimeSeries command
        Me.m_cmdExportTimeSeries = New cCommand(cmdh, "ExportTimeSeries")
        Me.m_cmdExportTimeSeries.AddControl(Me.m_tsmiTimeSeriesExport)

        'Create and configure Help>About command
        Me.m_cmdHelpAbout = New cCommand(cmdh, "HelpAbout")
        Me.m_cmdHelpAbout.AddControl(Me.m_tsmiHelpAbout)

        ' Create plugin gui command for GUI plugins to use
        Me.m_cmdPluginGUICommand = New cPluginGUICommand(cmdh)

        ' Create the one and only selection command
        Me.m_cmdPropertySelection = New cPropertySelectionCommand(cmdh)

        Me.m_cmdShowHideItems = New cDisplayGroupsCommand(cmdh)
        Me.m_cmdShowHideItems.AddControl(Me.m_tsmiViewItems)

        Me.m_cmdEnableEcotracer = New cCommand(cmdh, "EnableEcotracer")

        Me.m_cmdEstimateVs = New cCommand(cmdh, "EstimateVs")
        Me.m_cmdEstimateVs.AddControl(Me.m_tsmiEcosimEstimateVs)

        Me.m_cmdExportEcosimResultsToCSV = New cCommand(cmdh, "ExportEcosimResultsToCSV")
        Me.m_cmdExportEcosimResultsToCSV.AddControl(Me.m_tsmiExportBiomassToCSV)

        ' Listen to application Idle events to update command states
        AddHandler Application.Idle, AddressOf cmdh.OnIdle

    End Sub

    Private Sub InitPanels()

        ' Init panels
        m_NavPanel = New NavigationPanel(Me.UIContext, Me.m_pluginManager)
        m_StatusPanel = New StatusPanel(Me.UIContext)
        m_RemarkPanel = New RemarkPanel(Me.UIContext)
        m_StartPage = New frmWebBrowser(Me.UIContext)

        ' Add panels
        m_lstrProtectedPanelNames.Add(m_NavPanel.Name)
        m_lstrProtectedPanelNames.Add(m_StatusPanel.Name)
        m_lstrProtectedPanelNames.Add(m_RemarkPanel.Name)
        m_lstrProtectedPanelNames.Add(m_StartPage.Name)

    End Sub

    Private Sub InitDockPanelPositions()

        Me.m_NavPanel.Show(m_DockPanel, DockState.DockLeft)
        Me.m_StatusPanel.Show(m_DockPanel, DockState.DockBottomAutoHide)
        Me.m_RemarkPanel.Show(m_DockPanel, DockState.DockBottomAutoHide)

    End Sub

    Private Sub InitCoreParams()

        Dim so As SynchronizationContext = SynchronizationContext.Current

        If so Is Nothing Then
            'create the sync object on the same thread that created the AppLauncher
            so = New SynchronizationContext()
        End If

        Dim core As New cCore()
        Dim sg As New cStyleGuide()
        Dim cmdh As New cCommandHandler()
        Dim pm As New cPropertyManager(core, sg, so)
        Dim fps As New cFormPositionSettings()
        Dim help As New cHelp(Me, "UserGuide\EwE6_userguide.chm", "User Interface.htm", "EWE_UsersGuide")

        core.InitCore()

        Me.UIContext = New cUIContext(core, sg, pm, cmdh, Me, fps, help, so)

        ' Config state monitor
        Me.Core.StateMonitor.SyncObject = Me
        Me.m_mhEcosim = New cMessageHandler(AddressOf OnCoreMessage, eCoreComponentType.EcoSim, eMessageType.DataAddedOrRemoved, Me.m_uic.SyncObject)
        Me.m_mhEcospace = New cMessageHandler(AddressOf OnCoreMessage, eCoreComponentType.EcoSpace, eMessageType.DataAddedOrRemoved, Me.m_uic.SyncObject)
        Me.m_mhEcotracer = New cMessageHandler(AddressOf OnCoreMessage, eCoreComponentType.Ecotracer, eMessageType.DataAddedOrRemoved, Me.m_uic.SyncObject)
        Me.m_mhTimeseries = New cMessageHandler(AddressOf OnCoreMessage, eCoreComponentType.TimeSeries, eMessageType.DataAddedOrRemoved, Me.m_uic.SyncObject)

#If DEBUG Then
        Me.m_mhEcosim.Name = "ApplSim"
        Me.m_mhEcospace.Name = "ApplSpace"
        Me.m_mhEcotracer.Name = "ApplTracer"
        Me.m_mhTimeseries.Name = "ApplTS"
#End If

        Me.Core.Messages.AddMessageHandler(Me.m_mhEcosim)
        Me.Core.Messages.AddMessageHandler(Me.m_mhEcospace)
        Me.Core.Messages.AddMessageHandler(Me.m_mhEcotracer)
        Me.Core.Messages.AddMessageHandler(Me.m_mhTimeseries)

        ' Create plugin manager for this GUI
        Me.m_pluginManager = New cPluginManager()
        Me.m_pluginManager.UIContext = Me.UIContext
        Me.m_pluginManager.SyncObject = Me.UIContext.SyncObject
        Try
            Me.m_pluginManager.Settings = My.Settings.PluginConfiguration
        Catch ex As Exception
            Me.m_pluginManager.Settings = Nothing
        End Try

        ' Config plugin manager
        Me.m_pluginManager.Core = Me.Core
        Me.m_pluginManager.UIContext = Me.UIContext

        ' Distribute plugin manager
        Me.Core.PluginManager = Me.m_pluginManager

        ' Create plugin menu handler to position plugin menu items in the main menu from this form
        Me.m_pluginMenuHandler = New cPluginMenuHandler(Me.MainMenuStrip, Me.m_pluginManager, Me.UIContext.CommandHandler)

        ' Initialize core controller
        Me.m_coreController = New cCoreController(Me.Core.StateMonitor, Me.Core.StateManager)

        ' Initialize style guide updater
        Me.m_styleguideupdater = New StyleGuideUpdater(Me.UIContext)
        Me.m_styleguideupdater.Load()

    End Sub

    Private Sub InitEventHandlers()

        AddHandler My.Settings.SettingsLoaded, AddressOf OnDefaultSettingLoaded
        ' JS 27Apr10: ActiveContent seems to track much more accurately than ActiveDocument
        AddHandler Me.m_DockPanel.ActiveContentChanged, AddressOf OnTabFocusChanged

    End Sub

#End Region ' Initialization

#Region " Properties "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns the file name of the current loaded model.
    ''' </summary>
    ''' <param name="bFullPath">Flag stating thether the full path needs to be 
    ''' returned.</param>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property SelectedFileName(Optional ByVal bFullPath As Boolean = True) As String
        Get
            Dim ds As IEwEDataSource = Me.Core.DataSource
            If Object.ReferenceEquals(ds, Nothing) Then
                Return ""
            Else
                If bFullPath Then
                    Return ds.ToString()
                Else
                    Return Path.GetFileName(ds.ToString())
                End If
            End If
        End Get
    End Property

#End Region ' Properties

#Region " Messages "

    Private Delegate Sub SendMessageDelegate(ByVal strMsg As String, ByVal importance As eMessageImportance, ByVal component As eCoreComponentType)

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Send a message via the core.
    ''' </summary>
    ''' <param name="strMsg">Message text to send.</param>
    ''' <param name="importance">Message importance.</param>
    ''' <param name="component">Core component to represent as message origin.</param>
    ''' -----------------------------------------------------------------------
    Public Sub SendMessage(ByVal strMsg As String, _
                           Optional ByVal importance As eMessageImportance = eMessageImportance.Warning, _
                           Optional ByVal component As eCoreComponentType = eCoreComponentType.Core)

        If Me.InvokeRequired() Then
            Me.Invoke(New SendMessageDelegate(AddressOf Me.SendMessage), _
                                              New Object() {strMsg, importance, component})
            Return
        End If

        Dim msg As New cMessage(strMsg, eMessageType.Any, component, importance)
        Me.Core.Messages.SendMessage(msg)

    End Sub

    Private Delegate Function AskFeedbackDelegate(ByVal strMsg As String, ByVal importance As eMessageImportance, ByVal component As eCoreComponentType, ByVal replies As cFeedbackMessage.eReplyStyle, ByVal defaultReply As cFeedbackMessage.eReply) As cFeedbackMessage.eReply

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Ask for user feedback via the core feedback messaging system.
    ''' </summary>
    ''' <param name="strMsg">Message text to send.</param>
    ''' <param name="importance">Message importance.</param>
    ''' <param name="component">Core component to represent as message origin.</param>
    ''' -----------------------------------------------------------------------
    Public Function AskFeedback(ByVal strMsg As String, _
                             Optional ByVal importance As eMessageImportance = eMessageImportance.Warning, _
                             Optional ByVal component As eCoreComponentType = eCoreComponentType.Core, _
                             Optional ByVal replystyle As cFeedbackMessage.eReplyStyle = cFeedbackMessage.eReplyStyle.YES_NO_CANCEL, _
                             Optional ByVal defaultreply As cFeedbackMessage.eReply = cFeedbackMessage.eReply.YES) As cFeedbackMessage.eReply

        If Me.InvokeRequired() Then
            Dim dlgt As New AskFeedbackDelegate(AddressOf Me.AskFeedback)
            Dim aparms() As Object = New Object() {strMsg, importance, component, replystyle, defaultreply}
            Return DirectCast(Me.Invoke(dlgt, aparms), cFeedbackMessage.eReply)
        End If

        Dim fmsg As New cFeedbackMessage(strMsg, component, eMessageType.Any, importance, replystyle, eDataTypes.NotSet, defaultreply)
        Me.Core.Messages.SendMessage(fmsg)
        Return fmsg.Reply

    End Function

#End Region ' Messages

#Region " Form overrides "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Overridden to initialize the app launcer form.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)

        ' Add the dock panel 
        Me.SuspendLayout()
        m_DockPanel = New DockPanel
        With m_DockPanel
            .Parent = Me
            .Dock = DockStyle.Fill
            .BringToFront()
        End With

        Me.Icon = My.Resources.Ecopath
        Me.ResumeLayout()
        My.Settings.Reload()

        Dim al As ArrayList = My.Settings.MdbRecentlyUsedList
        My.Settings.MdbRecentlyUsedList = al

        ' Peeks at key but does not consume it
        Me.KeyPreview = True

        Me.InitCoreParams()
        Me.InitCommands()
        Me.InitPanels()
        Me.InitEventHandlers()

        Me.InitDockPanelPositions()

#If Not Debug Then
        ' Show start page (but not in DEBUG mode)
        Me.m_StartPage.Show(Me.m_DockPanel, DockState.Document)
#End If

        ' Start controlling the status strip
        Me.m_ssMain.Attach(Me.UIContext)
        ' Start controlling forms
        Me.m_FormStateHelper = New cEwEFormStateHelper(Me.Core.StateMonitor, Me.m_DockPanel)

        ' Update plug-ins first, if required
        If My.Settings.AutoUpdatePlugins Then
            Dim frm As New frmUpdateComponents(Me.m_pluginManager)
            frm.ShowDialog()
        End If

        ' Load plugins once GUI has been created.
        Me.LoadPlugins()
        ' Auto-launch plugins
        Me.AutolaunchPlugins()

        Me.ProcessCommandLine()
        Me.OnDefaultSettingLoaded(Nothing, Nothing) ' Ugh!
        Me.UpdateModelControls()

        Me.Help.HelpTopic(Me.m_StartPage) = "Ecopath with Ecosim 6 Getting started.htm"

        AddHandler Me.Core.StateMonitor.CoreExecutionStateEvent, AddressOf OnCoreExecutionStateChanged
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Event handler, catches the form closing event to make sure the core is finalized.
    ''' Application shut-down is cancelled if the core does not finalize correctly.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Protected Overrides Sub OnFormClosing(ByVal e As FormClosingEventArgs)

        ' Cancel application shut down if the core does not terminate succesfully.
        e.Cancel = Not Me.CloseEcopathModel()

        ' Abort if Ecopath model did not close sucessfully
        If e.Cancel Then Return

        Try

            ' Cleanup: disconnect command handler from idle event
            Dim cmdh As cCommandHandler = Me.m_uic.CommandHandler
            RemoveHandler Application.Idle, AddressOf cmdh.OnIdle

            Me.m_uic.Core.Messages.RemoveMessageHandler(Me.m_mhEcosim)
            Me.m_uic.Core.Messages.RemoveMessageHandler(Me.m_mhEcospace)
            Me.m_uic.Core.Messages.RemoveMessageHandler(Me.m_mhEcotracer)
            Me.m_uic.Core.Messages.RemoveMessageHandler(Me.m_mhTimeseries)

            RemoveHandler Me.Core.StateMonitor.CoreExecutionStateEvent, AddressOf OnCoreExecutionStateChanged
            RemoveHandler Me.m_DockPanel.ActiveContentChanged, AddressOf OnTabFocusChanged

            ' Terminate all model-independent UI components
            Me.CloseAllContents()
            Me.ClearMRUDropdown()

        Catch ex As Exception

        End Try

        MyBase.OnFormClosing(e)

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Cluck?
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Protected Overrides Sub OnKeyDown(ByVal e As KeyEventArgs)

        Try
            ' Restore menu and full screen mode on 'Escape'
            If (e.KeyCode = Keys.Escape) Then
                If (Me.m_cmdViewPresentationMode.Checked) Then
                    Me.m_cmdViewPresentationMode.Invoke()
                End If
                If (Me.m_cmdViewMenu.Checked = False) Then
                    Me.m_cmdViewMenu.Invoke()
                End If
            End If


            ' Egg!
            If (e.KeyCode = Keys.F12) Then
                MsgBox("Bite me", MsgBoxStyle.Exclamation)
            End If

            ' Egg!
            If e.Alt And e.Control And e.Shift Then
                Dim strURL As String = ""
                Select Case e.KeyCode
                    Case Keys.Oemtilde : strURL = "http://farm1.static.flickr.com/160/374820104_5ec655655c.jpg"
                    Case Keys.D1 : strURL = "http://farm1.static.flickr.com/82/261884734_01ad1712a6.jpg"
                    Case Keys.D2 : strURL = "http://farm2.static.flickr.com/1218/536646225_09f93a0b8c.jpg"
                    Case Keys.D3 : strURL = "http://farm1.static.flickr.com/112/261883295_1cab2a9714.jpg"
                    Case Keys.D4 : strURL = "http://farm1.static.flickr.com/87/261883288_06e5599f56.jpg"
                    Case Keys.D5 : strURL = "http://farm1.static.flickr.com/89/261883279_6c8b139ed9.jpg"
                    Case Keys.D6 : strURL = "http://farm1.static.flickr.com/121/261883269_cf6fd5f287.jpg"
                    Case Keys.D7 : strURL = "http://farm2.static.flickr.com/1312/1400452382_47306892c0.jpg"
                    Case Keys.D8 : strURL = "http://farm2.static.flickr.com/1012/1400449350_7dfad8dd60.jpg"
                    Case Keys.D9 : strURL = "http://farm3.static.flickr.com/2344/1536185215_fe4d413654.jpg"
                    Case Keys.D0 : strURL = "http://farm1.static.flickr.com/143/377851455_28924928b1.jpg"
                End Select

                If Not String.IsNullOrEmpty(strURL) Then
                    Me.m_StartPage.URL = strURL
                    Me.m_StartPage.Show(Me.m_DockPanel, DockState.Document)
                End If

            End If
        Catch ex As Exception

        End Try
    End Sub

#End Region ' Form overrides

#Region " Status feedback "

    Private Delegate Sub SetStatusTextDelegate(ByVal strText As String, ByVal tsUseWaitCursor As TriState, ByVal sProgress As Single)

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Set the application status strip text and wait cursor.
    ''' </summary>
    ''' <param name="strText">Status text to display, if any.</param>
    ''' <param name="tsUseWaitCursor">
    ''' <para>Tri-state flag stating whether a wait cursor should be shown.
    ''' Values are interpreted as follows:</para>
    ''' <list type="bullet">
    ''' <item><description>True: the wait cursor must be set.</description></item>
    ''' <item><description>False: the wait cursor must be cleared.</description></item>
    ''' <item><description>UseDefault: the wait cursor state should not change.</description></item>
    ''' </list>
    ''' </param>
    ''' <param name="sProgress">Ratio [0, 1] of progress to display. 0 to hide progress.</param>
    ''' <remarks>
    ''' Note that the wait cursor state is maintained via an internal counter. Setting
    ''' the wait cursor state will increment this counter, clearing the wait cursor state
    ''' decrements the counter. The actual wait cursor will be set when this counter is non-zero,
    ''' and is cleared when this counter reaches zero.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Private Sub SetStatusText(Optional ByVal strText As String = "", _
        Optional ByVal tsUseWaitCursor As TriState = TriState.UseDefault, _
        Optional ByVal sProgress As Single = 0.0) _
        Implements IApplicationStatusDispatcher.SetStatusText

        If Me.InvokeRequired() Then
            Me.Invoke(New SetStatusTextDelegate(AddressOf Me.SetStatusText), _
                      New Object() {strText, tsUseWaitCursor, sProgress})
            Return
        End If

        ' ToDo_JS: Consider using a timer to clear any status text after a certain interval

        ' Update wait cursor
        Select Case tsUseWaitCursor

            Case TriState.True ' Set wait cursor

                ' Push text to the status text stack
                Me.m_lstrStatus.Insert(0, strText)
                ' Set wait cursor
                Me.Cursor = Cursors.WaitCursor

            Case TriState.False ' Clear wait cursor

                ' Has wait cursors pending?
                If Me.m_lstrStatus.Count > 0 Then
                    ' #Yes: no text specified?
                    If String.IsNullOrEmpty(strText) Then
                        ' #Yes: obtain text from the status text stack
                        strText = Me.m_lstrStatus(0)
                    End If
                    ' Pop text from the status text stack
                    Me.m_lstrStatus.RemoveAt(0)
                End If

                ' Status stack empty?
                If Me.m_lstrStatus.Count = 0 Then
                    ' #Yes: restore default cursor
                    Me.Cursor = Cursors.Default
                    strText = ""
                End If

            Case TriState.UseDefault
                ' Don't do anything. Really.

        End Select

        ' JS 12oct07: disabled total refresh to minimize screen flickering
        '' Redraw!
        'Me.Refresh()

        ' Update status text
        Me.m_ssMain.SetStatusText(strText, sProgress)

    End Sub

#End Region ' Status feedback

#Region " Plug-ins "

    Private Sub AutolaunchPlugins()
        Using pl As New cPluginAutolaunchHandler(Me.m_pluginManager, Me.UIContext.CommandHandler)
            ' Hah! The 'using' construction here will deal with proper disposal
        End Using
    End Sub

    Private Sub LoadPlugins()

        Dim strMessage As String = ""
        Dim reply As cFeedbackMessage.eReply = cFeedbackMessage.eReply.OK
        Dim bNeedReply As Boolean = False

        Try
            Me.m_pluginManager.LoadPlugins(My.Settings.DisabledPlugins)
        Catch ex As Exception
            ' Ouch!
        End Try

        My.Settings.DisabledPlugins = Me.m_pluginManager.DisabledPlugins
        Me.SaveSettings()

    End Sub

#End Region ' Plug-ins

#Region " Database utils "

    Private Function CompactModel() As Boolean

        Dim ds As IEwEDataSource = Me.Core.DataSource
        Dim result As eDatasourceAccessType = eDatasourceAccessType.Success
        Dim strFileName As String = Me.SelectedFileName()
        Dim strMessage As String = ""
        Dim bSucces As Boolean = True

        If (Me.AskFeedback(My.Resources.PROMPT_MODEL_COMPACT) <> cFeedbackMessage.eReply.YES) Then
            Return False
        End If

        If Me.CloseEcopathModel() = False Then Return False

        Me.SetStatusText(My.Resources.STATUS_MODEL_COMPACTING, TriState.True)
        result = ds.Compact(strFileName)
        Me.SetStatusText("", TriState.False)

        If result = eDatasourceAccessType.Success Then
            bSucces = Me.LoadEcopathModel(strFileName, eLoadSourceType.API)
            If bSucces Then
                strMessage = My.Resources.STATUS_MODEL_COMPACT_SUCCESS
            Else
                strMessage = My.Resources.STATUS_MODEL_COMPACT_RELOADFAIL
            End If
        Else
            ' Report error
            Select Case result
                Case eDatasourceAccessType.Failed_OSUnsupported
                    strMessage = My.Resources.STATUS_MODEL_COMPACTING_OS
                Case eDatasourceAccessType.Failed_CannotSave
                    strMessage = My.Resources.STATUS_MODEL_COMPACTING_TEMPFILE
                Case eDatasourceAccessType.Failed_FileNotFound, _
                     eDatasourceAccessType.Failed_Unknown
                    strMessage = My.Resources.STATUS_MODEL_COMPACTING_FAILED
                Case eDatasourceAccessType.Failed_ReadOnly
                    strMessage = My.Resources.STATUS_MODEL_ACCESS_READONLY
            End Select
            bSucces = False
        End If

        If (bSucces) Then
            Me.SendMessage(strMessage, eMessageImportance.Information, eCoreComponentType.DataSource)
        Else
            Me.SendMessage(strMessage, eMessageImportance.Critical, eCoreComponentType.DataSource)
        End If

        Return bSucces

    End Function

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' To test if it is an EwE5 Access database, if it is, convert it using the 
    ''' database conversion wizard.
    ''' </summary>
    ''' <param name="strFileName">File name of the Access database to convert. If a
    ''' conversion is necessary this parameter will receive the file name of the
    ''' converted file.</param>
    ''' <returns>True if the database specified by <paramref name="fileName">filename</paramref>
    ''' is already an EwE6 database, or if the conversion was succesful.</returns>
    ''' ---------------------------------------------------------------------------
    Private Function CovertToEwE6(ByRef strFileName As String) As Boolean

        Dim dst As eDataSourceTypes = eDataSourceTypes.NotSet
        Dim comp As cEwEDatabase.eCompatibilityTypes = cEwEDatabase.eCompatibilityTypes.Unknown
        Dim bSucces As Boolean = True

        ' Detect file type
        dst = cDataSourceFactory.GetSupportedType(strFileName)
        Select Case dst

            Case eDataSourceTypes.ACCDB, eDataSourceTypes.MDB
                ' Is database, whoohoo
                Dim db As New cEwEAccessDatabase()
                If db.Open(strFileName) = eDatasourceAccessType.Opened Then
                    comp = db.Compatibility
                    db.Close()
                End If

            Case eDataSourceTypes.EII
                ' Is EII
                comp = cEwEDatabase.eCompatibilityTypes.EwE5Supported

            Case eDataSourceTypes.NotSet
                ' ?Que?
                Return False

        End Select

        Select Case comp

            Case cEwEDatabase.eCompatibilityTypes.EwE5TooOld
                Me.SendMessage(My.Resources.PROMPT_ERROR_IMPORT_EWE5_TOO_OLD)
                bSucces = False

            Case cEwEDatabase.eCompatibilityTypes.EwE5Supported
                AddRecentFilesSetting(strFileName)

                Dim dlg As New Import.dlgImportDatabase(Me.UIContext, strFileName)
                If dlg.ShowDialog(Me) = DialogResult.OK Then
                    ' Update file name
                    strFileName = dlg.ImportedFileName
                    ' Report succes
                    bSucces = True
                Else
                    bSucces = False
                End If

            Case cEwEDatabase.eCompatibilityTypes.EwE5TooNew
                Me.SendMessage(My.Resources.PROMPT_ERROR_IMPORT_EWE5_TOO_NEW)
                bSucces = False

            Case cEwEDatabase.eCompatibilityTypes.EwE6
                ' Moved to core

            Case cEwEDatabase.eCompatibilityTypes.UnknownFuture
                Me.SendMessage(My.Resources.PROMPT_ERROR_IMPORT_EWE7_OR_NEWER)
                bSucces = False

            Case cEwEDatabase.eCompatibilityTypes.Unknown
                Me.SendMessage(My.Resources.PROMPT_ERROR_IMPORT_INVALIDDB)
                bSucces = False

            Case Else
                ' Unsupported enum value?!
                Debug.Assert(False)
                bSucces = False

        End Select

        Return bSucces

    End Function

    Private Sub ReportFileAccessError(ByVal atResult As eDatasourceAccessType, ByVal strFileName As String)

        Dim strMessage As String = ""

        Select Case atResult
            Case eDatasourceAccessType.Failed_ReadOnly
                strMessage = String.Format(My.Resources.STATUS_MODEL_ACCESS_READONLY, strFileName)
            Case eDatasourceAccessType.Failed_OSUnsupported
                strMessage = String.Format(My.Resources.STATUS_MODEL_ACCESS_OS, strFileName)
            Case eDatasourceAccessType.Failed_FileNotFound
                strMessage = String.Format(My.Resources.STATUS_MODEL_ACCESS_404, strFileName)
            Case eDatasourceAccessType.Failed_CannotSave
                strMessage = String.Format(My.Resources.STATUS_MODEL_SAVE_404, strFileName)
            Case Else
                strMessage = String.Format(My.Resources.STATUS_MODEL_ACCESS_FAILED, strFileName)
        End Select

        Me.SendMessage(strMessage, eMessageImportance.Warning, eCoreComponentType.DataSource)

    End Sub

#End Region ' Database utils

#Region " UI updates "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Helper method, updates the state of controls reflecting the current model. 
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub UpdateModelControls()

        Me.m_tsModel.Path = Me.SelectedFileName
        If String.IsNullOrEmpty(Me.SelectedFileName) Then
            Me.Text = String.Format(My.Resources.GENERIC_CAPTION)
        Else
            Me.Text = String.Format(My.Resources.GENERIC_CAPTION_OPENMODEL, Me.SelectedFileName(False))
        End If

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Helper method, populate the content of the scenario drop-down controls
    ''' with lists of scenarios available in the current model. 
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub PopulateScenarioDropdowns()

        Dim tsmi As ToolStripMenuItem = Nothing

        Me.ClearScenarioDropdowns()

        ' Has a model loaded?
        If Me.Core.StateMonitor.HasEcopathLoaded() Then

            ' #Yes: add scenario lists

            ' VERIFY_JS: Should scenarios be sorted in the most recent load order, or is that going to be highly confusing?

            ' List available Ecosim scenarios.
            For i As Integer = 1 To Me.Core.EcosimScenarioCount
                tsmi = New ToolStripMenuItem()
                tsmi.Text = Me.Core.EcosimScenarios(i).Name
                tsmi.Tag = Me.Core.EcosimScenarios(i)
                tsmi.Checked = (Me.Core.ActiveEcosimScenarioIndex = i)
                AddHandler tsmi.Click, AddressOf OnLoadEcosimScenarioOrDataset
                Me.m_tsbEcosim.DropDownItems.Add(tsmi)
            Next

            ' List available Ecosim time series datasets
            For i As Integer = 1 To Me.Core.nTimeSeriesDatasets

                ' Is first dataset?
                If (i = 1) Then
                    ' #Yes: add a separtor
                    Me.m_tsbEcosim.DropDownItems.Add(New ToolStripSeparator())
                End If

                tsmi = New ToolStripMenuItem()
                tsmi.Text = Me.Core.TimeSeriesDataset(i).Name
                tsmi.Tag = Me.Core.TimeSeriesDataset(i)
                tsmi.Checked = (Me.Core.ActiveTimeSeriesDatasetIndex = i)

                AddHandler tsmi.Click, AddressOf OnLoadEcosimScenarioOrDataset
                Me.m_tsbEcosim.DropDownItems.Add(tsmi)

            Next i

            ' List available Ecospace scenarios
            For i As Integer = 1 To Me.Core.EcospaceScenarioCount
                tsmi = New ToolStripMenuItem()
                tsmi.Text = Me.Core.EcospaceScenarios(i).Name
                tsmi.Tag = Me.Core.EcospaceScenarios(i)
                tsmi.Checked = (Me.Core.ActiveEcospaceScenarioIndex = i)
                AddHandler tsmi.Click, AddressOf OnLoadEcospaceScenario
                Me.m_tsbEcospace.DropDownItems.Add(tsmi)
            Next

            ' List available Ecotracer scenarios
            For i As Integer = 1 To Me.Core.EcotracerScenarioCount
                tsmi = New ToolStripMenuItem()
                tsmi.Text = Me.Core.EcotracerScenarios(i).Name
                tsmi.Tag = Me.Core.EcotracerScenarios(i)
                tsmi.Checked = (Me.Core.ActiveEcotracerScenarioIndex = i)
                AddHandler tsmi.Click, AddressOf OnLoadEcotracerScenario
                Me.m_tsbEcotracer.DropDownItems.Add(tsmi)
            Next

        End If

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Helper method, clear the content of the scenario drop-down controls. 
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub ClearScenarioDropdowns()

        Dim tsi As ToolStripItem = Nothing

        ' Properly release sim menu items
        For Each tsi In Me.m_tsbEcosim.DropDownItems
            If Not (TypeOf tsi Is ToolStripSeparator) Then
                RemoveHandler tsi.Click, AddressOf OnLoadEcosimScenarioOrDataset
            End If
        Next
        Me.m_tsbEcosim.DropDownItems.Clear()

        ' Properly release space menu items
        For Each tsi In Me.m_tsbEcospace.DropDownItems
            RemoveHandler tsi.Click, AddressOf OnLoadEcospaceScenario
        Next
        Me.m_tsbEcospace.DropDownItems.Clear()

        ' Properly release tracer menu items
        For Each tsi In Me.m_tsbEcotracer.DropDownItems
            RemoveHandler tsi.Click, AddressOf OnLoadEcotracerScenario
        Next
        Me.m_tsbEcotracer.DropDownItems.Clear()

    End Sub

#End Region ' UI updates

#Region " Settings "

    Private Sub SaveMainFormSettings()

        ' Save the user settings when EwE exits
        My.Settings.LastSelectedDirectory = Me.m_strLastSelectedPath

        Me.m_uic.FormPositionSettings.Store(Me, False)
        Me.m_styleguideupdater.Save()
        My.Settings.FormPositions = Me.m_uic.FormPositionSettings.Setting
        Me.SaveSettings()

    End Sub

    Private Sub SaveSettings()

        If (Me.m_pluginManager IsNot Nothing) Then
            My.Settings.PluginConfiguration = Me.m_pluginManager.Settings
        End If
        My.Settings.Save()

    End Sub

#End Region ' Settings

#Region " MRU "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Add a file name to the top of the MRU list.
    ''' </summary>
    ''' <param name="strFileName">Name of the file to add.</param>
    ''' -----------------------------------------------------------------------
    Private Sub AddRecentFilesSetting(ByVal strFileName As String)

        Dim alMDBmru As ArrayList = My.Settings.MdbRecentlyUsedList

        If (alMDBmru Is Nothing) Then Return

        ' Insert at head
        alMDBmru.Insert(0, strFileName)
        My.Settings.MdbRecentlyUsedList = alMDBmru

        ' Remove any occurrences further down the list
        Me.RemoveRecentFilesSetting(strFileName, 1)

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Remove a file name from the MRU list, if possible.
    ''' </summary>
    ''' <param name="strFileName">Name of the file to remove.</param>
    ''' <param name="iStartPos">Index in the MRU list to start searching for
    ''' the item to remove. If not provided, the search will start at the 
    ''' beginning of the list.</param>
    ''' -----------------------------------------------------------------------
    Private Sub RemoveRecentFilesSetting(ByVal strFileName As String, _
                                         Optional ByVal iStartPos As Integer = 0)

        Dim alMDBmru As ArrayList = My.Settings.MdbRecentlyUsedList

        If (alMDBmru Is Nothing) Then Return

        ' Remove first occurrence from down the list
        For iEntry As Integer = iStartPos To alMDBmru.Count - 2
            ' Valid entry?
            If (TypeOf alMDBmru(iEntry) Is String) Then
                ' Get entry
                Dim strEntry As String = CStr(alMDBmru(iEntry))
                ' Is same file?
                If strEntry.StartsWith(strFileName) Then
                    ' #Yes: remove 
                    alMDBmru.RemoveAt(iEntry)
                    ' Done
                    Exit For
                End If
            End If
        Next iEntry

        ' Update system settings
        My.Settings.MdbRecentlyUsedList = alMDBmru
        Me.SaveSettings()

        ' Reflect!
        Me.PopulateMRUDropdown()

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Show the list of MRU items in the menu structure.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub PopulateMRUDropdown()

        Dim alMRU As ArrayList = My.Settings.MdbRecentlyUsedList
        Dim iNumItems As Integer = Math.Min(alMRU.Count - 1, My.Settings.MdbRecentlyUsedCount)
        Dim item As ToolStripMenuItem = Nothing
        Dim bHasMRU As Boolean = False

        ' Clear MRU list
        Me.ClearMRUDropdown()

        If (alMRU IsNot Nothing) Then
            bHasMRU = (alMRU.Count > 1)
        End If

        ' No recently accessed files yet?
        If (bHasMRU = False) Then
            ' Always have 'None' item
            item = New ToolStripMenuItem()
            item.Text = SharedResources.GENERIC_VALUE_NONE
            item.Enabled = False
            Me.m_tsmiFileRecent.DropDownItems.Add(item)
            Return
        End If

        For i As Integer = 0 To iNumItems - 1

            Dim str As String() = CStr(alMRU.Item(i)).Split(New Char() {";"c})

            item = New ToolStripMenuItem()
            item.Text = String.Format(SharedResources.GENERIC_LABEL_INDEXED, i + 1, str(0))
            item.Tag = str(0)

            'Add event handler to invoke the model
            AddHandler item.Click, AddressOf OnMRUItemClicked

            Me.m_tsmiFileRecent.DropDownItems.Add(item)

            item = New ToolStripMenuItem()
            item.Text = str(0)
            item.Tag = str(0)
            item.Checked = (String.Compare(str(0), Me.SelectedFileName, True) = 0)

            'Add event handler to invoke the model
            AddHandler item.Click, AddressOf OnMRUItemClicked

            Me.m_tsbEcopath.DropDownItems.Add(item)
        Next

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Clear the list of MRU items from the menu structure.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub ClearMRUDropdown()

        Dim item As ToolStripMenuItem = Nothing

        For Each item In Me.m_tsmiFileRecent.DropDownItems
            If (item.Tag IsNot Nothing) Then
                ' Remove dangling event handler
                RemoveHandler item.Click, AddressOf OnMRUItemClicked
            End If
        Next
        ' Eradicate menu items
        Me.m_tsmiFileRecent.DropDownItems.Clear()


        For Each item In Me.m_tsbEcopath.DropDownItems
            If (item.Tag IsNot Nothing) Then
                ' Remove dangling event handler
                RemoveHandler item.Click, AddressOf OnMRUItemClicked
            End If
        Next
        Me.m_tsbEcopath.DropDownItems.Clear()

    End Sub

#End Region ' MRU

#Region " Content navigation "

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Create a form or dock panel for a given type.
    ''' </summary>
    ''' <param name="strNavLink">Navigation descriptor that created the form.</param>
    ''' <param name="t"><see cref="Type">Type</see> of the form to create.</param>
    ''' <returns>A <see cref="Form">Form</see>-derived instance, or Nothing if the
    ''' form could not be created.
    ''' </returns>
    ''' ---------------------------------------------------------------------------
    Private Function LoadFormFromType(ByVal strNavLink As String, _
                                      ByVal t As Type, _
                                      ByVal state As eCoreExecutionState) As Form

        Dim classObject As Object
        Dim frmNew As Form = Nothing
        Dim strCaption As String = ""

        If Object.ReferenceEquals(t, Nothing) Then Return Nothing

        Try
            classObject = Activator.CreateInstance(t)

            If TypeOf classObject Is DockContent Then
                ' Is dock content
                frmNew = DirectCast(classObject, DockContent)
            ElseIf TypeOf classObject Is EwEGrid Then
                ' Is a grid
                Dim grid As EwEGrid = DirectCast(classObject, EwEGrid)
                ' Fill the form with griddibits
                grid.Dock = DockStyle.Fill
                frmNew = New frmEwEGrid(grid)
                ' Use grid text as form caption
                frmNew.Text = grid.Text
            ElseIf TypeOf classObject Is Form Then
                ' Is a generic form
                frmNew = DirectCast(classObject, Form)
                frmNew.Text = strNavLink
            End If

            If TypeOf frmNew Is frmEwE Then
                ' Provide form with state
                DirectCast(frmNew, frmEwE).CoreExecutionState = state
            End If

            If (TypeOf (frmNew) Is IUIElement) Then
                ' Configure new object with UI context
                DirectCast(frmNew, IUIElement).UIContext = Me.UIContext
            End If

            ' Fix form caption
            strCaption = frmNew.Text
            ' Use a default if necessary
            If String.IsNullOrEmpty(strCaption) Then strCaption = strNavLink
            ' Stick caption back into the form
            frmNew.Text = strCaption
            If (TypeOf frmNew Is DockContent) Then
                Dim cnt As DockContent = DirectCast(frmNew, DockContent)
                ' Use caption also for tab text
                cnt.TabText = strCaption
            End If

            ' Store nav link
            frmNew.Tag = strNavLink

            ' Set form icon based on core state
            Select Case state
                Case eCoreExecutionState.EcopathLoaded, eCoreExecutionState.EcopathCompleted, eCoreExecutionState.EcopathRunning
                    frmNew.Icon = My.Resources.Ecopath
                Case eCoreExecutionState.EcosimLoaded, eCoreExecutionState.EcosimRunning, eCoreExecutionState.EcosimCompleted
                    frmNew.Icon = My.Resources.Ecosim3
                Case eCoreExecutionState.EcospaceLoaded, eCoreExecutionState.EcospaceRunning, eCoreExecutionState.EcospaceCompleted
                    frmNew.Icon = My.Resources.Ecospace3
                Case eCoreExecutionState.EcotracerLoaded
                    frmNew.Icon = My.Resources.Ecotracer
            End Select

        Catch ex As Exception
            Debug.Assert(False, "Creation of Form was not successful.  Please contact help: '" & strNavLink & "' threw exception " & ex.ToString)
        End Try

        Return frmNew
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Helper method, tries to activate an opened dock panel or MDI child 
    ''' window.
    ''' </summary>
    ''' <param name="strNavLink">Navigation descriptor to find the panel with.</param>
    ''' <returns>True if an existing panel was found.</returns>
    ''' -----------------------------------------------------------------------
    Private Function ActivateForm(ByVal strNavLink As String) As Boolean

        Dim bFound As Boolean = False

        ' Dock settings, loop through current opened 
        For Each cnt As DockContent In m_DockPanel.Contents

            If (TypeOf cnt.Tag Is String) Then
                bFound = String.Compare(CStr(cnt.Tag), strNavLink, True) = 0
            End If

            If Not bFound Then
                bFound = (String.Compare(cnt.Text, strNavLink, True) = 0)
            End If

            If bFound Then
                ' JS 08aug07: work-around for bug 133 (http://www.ecopath.org/developers/bugtracker/view.php?id=133)
                ' Source:   Weifen Luo dock content xml section for "Document" state panel is improperly written or missing
                ' Effect:   Forms that are supposed to be docked in that panel are constructed with Unknown dock properties
                '           but are not docked into any panel. Upon Activation, this logic restores damaged dock styles to
                '           reveal forms affected by this bug.
                ' Solution: Fix imcomplete XML issues in the dock panel engine.
                '           Hahaha!
                With cnt
                    .IsHidden = False
                    If .DockState = DockState.Unknown Then .DockState = DockState.Document
                    If .VisibleState = DockState.Unknown Then .VisibleState = DockState.Document
                    If .WindowState = FormWindowState.Minimized Then .WindowState = FormWindowState.Normal
                    .BringToFront()
                    .Focus()
                End With

                Return True
            End If
        Next
        ' Failed to find an existing panel with this tab text.
        Return False
    End Function

    ''' <summary>Flag to prevent looped navigation chaos.</summary>
    Private m_bNavigating As Boolean = False
    Private m_strLastActiveContent As String = ""

    Private Sub UpdateSelectedNode(ByVal strNodeName As String, _
                                   Optional ByVal bAllowDefault As Boolean = False)

        If Me.m_bNavigating Then Return
        Me.m_bNavigating = True
        ' Remember this
        Me.m_strLastActiveContent = strNodeName
        ' Kick nav panel
        Me.m_NavPanel.SelectedNodeName(bAllowDefault) = strNodeName
        Me.m_bNavigating = False

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Private method to close all open child forms PLUS all panels on the parent form.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub CloseAllContents()
        ' Forget panels
        Me.m_NavPanel.DockPanel = Nothing
        Me.m_RemarkPanel.DockPanel = Nothing
        Me.m_StatusPanel.DockPanel = Nothing
        ' Close all other documents
        Me.CloseAllDocuments()
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Private method to close all open child forms of the parent form.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub CloseAllDocuments()

        Dim lForms As New List(Of Form)

        ' Make temp list of all documents that may be closed. This cannot
        ' be performed in a for..ech loop because that affects the iterator
        ' used in the loop.
        For Each f As Form In Me.m_DockPanel.Contents
            If Not Me.m_lstrProtectedPanelNames.Contains(f.Name) Then
                lForms.Add(f)
            End If
        Next
        ' Now close the forms
        For Each f As Form In lForms
            f.Close()
        Next
        ' Let's explicitly clean-up for once.
        lForms = Nothing

        Me.UpdateSelectedNode("", False)

    End Sub

#End Region ' Content navigation

#Region " Ecopath "

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Open Ecopath model from given location.
    ''' </summary>
    ''' <param name="strFileName">Location of the model to open.</param>
    ''' <param name="loadsource">Flag indicating where the load request came from.</param>
    ''' <remarks>This code is designed for strFileName to indicate a path. It should 
    ''' be possible to indicate a database as well. One day...</remarks>
    ''' ---------------------------------------------------------------------------
    Private Function LoadEcopathModel(ByVal strFileName As String, _
                                      ByVal loadsource As eLoadSourceType) As Boolean

        Dim ds As IEwEDataSource = Nothing
        Dim atResult As eDatasourceAccessType = eDatasourceAccessType.Failed_Unknown

        ' Check if target file exists at all before affecting anything
        If Not File.Exists(strFileName) Then

            ' Handle failure
            Select Case loadsource

                Case eLoadSourceType.MRU
                    If Me.AskFeedback(String.Format(My.Resources.PROMPT_MODELNOTFOUND_REMOVEMRU, strFileName), _
                                      replystyle:=cFeedbackMessage.eReplyStyle.YES_NO) = cFeedbackMessage.eReply.YES Then
                        Me.RemoveRecentFilesSetting(strFileName)
                    End If

                Case eLoadSourceType.User, _
                     eLoadSourceType.CommandLine
                    ' Unable to load model, show generic error
                    Me.SendMessage(String.Format(My.Resources.PROMPT_MODELNOTFOUND, strFileName), _
                                   eMessageImportance.Warning, eCoreComponentType.DataSource)

                Case eLoadSourceType.API
                    ' Do not provide user feedback in response to an API call

            End Select
            Return False
        End If

        ' Can close the current open model, if any?
        If Not CloseEcopathModel() Then
            ' #No: cannot close - abort
            Return False
        End If

        If Not CovertToEwE6(strFileName) Then
            ' #No: EwE6 database? abort
            Return False
        End If

        ' Abort if no new file name given
        If String.IsNullOrEmpty(strFileName) Then Return True

        ' Create datasource on the selected file
        ds = cDataSourceFactory.Create(strFileName)

        If (ds Is Nothing) Then
            Select Case loadsource

                Case eLoadSourceType.MRU
                    ' Should not occur

                Case eLoadSourceType.User, eLoadSourceType.CommandLine
                    ' Unable to load model, show generic error
                    Me.SendMessage(String.Format(My.Resources.PROMPT_INVALIDMODEL, strFileName), _
                                   eMessageImportance.Warning, eCoreComponentType.DataSource)

                Case eLoadSourceType.API
                    ' Ok then

            End Select
            Return False
        End If

        ' Update MRU
        Me.AddRecentFilesSetting(strFileName)

        ' Open the datasource
        atResult = ds.Open(strFileName, Me.Core)

        If (atResult <> eDatasourceAccessType.Success) Then
            Me.ReportFileAccessError(atResult, strFileName)
            Return False
        End If

        ' Ok, now let's see if the core can work with this
        If Me.Core.LoadModel(ds) Then

            ' Set core output path
            Me.Core.OutputPath = Path.GetDirectoryName(strFileName)

            '' JS 08Aug07: Whatever happened, at least the default node needs to be visible.
            ''             This also overcomes bug 133 (see bug description in ActivateForm). The Dock engine
            ''             may create forms from crippled XML settings where a doc parent section is missing.
            ''             Such forms get instantiated but GetContentFromPersistentString never gets called
            ''             because the forms are not content as far as the dock engine is concerned. Nice!
            ''             This logic makes sure that at least the default form is properly selected (and indirectly activated)
            'Me.EnsureDefaultNodeSelected()
            Me.UpdateSelectedNode("", True)
            ' Keep at it, Maurice
            Me.UpdateModelControls()
            Me.PopulateMRUDropdown()
            Me.PopulateScenarioDropdowns()

            Return True
        Else
            Dim message As String = String.Format(My.Resources.GENERIC_ERROR_FILEOPEN, strFileName)
            MessageBox.Show(Me, message, Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return False
        End If

    End Function

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Save model to a different datasource and switch to that new datasource. 
    ''' </summary>
    ''' <param name="strFileName">Full path + extension of the file to save.</param>
    ''' ---------------------------------------------------------------------------
    Private Function SaveEcopathModelAs(ByVal strFileName As String) As Boolean

        If (Me.Core.Save(strFileName)) Then
            Me.AddRecentFilesSetting(strFileName)
            Me.UpdateModelControls()
            Return True
        End If
        Return False
    End Function

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Create a new Ecopath model at a requested location.
    ''' </summary>
    ''' <param name="strFileName">The name of the file to create.</param>
    ''' <param name="strModelName">The name of the model to create.</param>
    ''' <param name="format">The file format to create.</param>
    ''' <returns>An Ecopath database, if succesful.</returns>
    ''' <remarks>
    ''' Note that this will NOT load the new model! For this, 
    ''' <see cref="LoadEcopathModel">cAppLauncher.LoadEcopathModel</see> will need
    ''' to be called.
    ''' </remarks>
    ''' ---------------------------------------------------------------------------
    Friend Function CreateEcopathModel(ByVal strFileName As String, _
                                        ByVal strModelName As String, _
                                        ByVal format As eDataSourceTypes) As cEwEDatabase

        Dim db As cEwEDatabase = Nothing
        Dim atResult As eDatasourceAccessType = eDatasourceAccessType.Failed_Unknown
        Dim strPrompt As String = ""
        Dim importance As eMessageImportance = eMessageImportance.Warning

        Select Case format
            Case eDataSourceTypes.MDB, eDataSourceTypes.ACCDB
                db = New cEwEAccessDatabase()
                atResult = db.Create(strFileName, strModelName, True, format)

            Case eDataSourceTypes.EII
                atResult = eDatasourceAccessType.Failed_DeprecatedOperation

            Case eDataSourceTypes.NotSet
                atResult = eDatasourceAccessType.Failed_UnknownType
        End Select

        ' Provide status feedback
        Select Case atResult

            Case eDatasourceAccessType.Success, eDatasourceAccessType.Opened
                strPrompt = String.Format(My.Resources.PROMPT_MODELCREATED, strFileName)
                importance = eMessageImportance.Information

            Case eDatasourceAccessType.Failed_CannotSave
                strPrompt = String.Format(My.Resources.PROMPT_INVALIDTARGETPATH, strFileName)
                importance = eMessageImportance.Critical

                ' Should not occur
                'Case eDatasourceAccessType.Failed_ReadOnly 

            Case eDatasourceAccessType.Failed_OSUnsupported
                strPrompt = My.Resources.PROMPT_DRIVERERROR
                importance = eMessageImportance.Critical

            Case eDatasourceAccessType.Failed_UnknownType
                strPrompt = My.Resources.PROMPT_INVALIDFILE
                importance = eMessageImportance.Critical

            Case eDatasourceAccessType.Failed_DeprecatedOperation
                strPrompt = My.Resources.PROMPT_FILETYPEDEPRECATED
                importance = eMessageImportance.Critical

            Case eDatasourceAccessType.Failed_Unknown
                strPrompt = String.Format(My.Resources.PROMPT_CREATE_GENERICERROR, strFileName)
                importance = eMessageImportance.Critical

        End Select

        If Not String.IsNullOrEmpty(strPrompt) Then
            Me.SendMessage(strPrompt, importance, eCoreComponentType.DataSource)
        End If

        If importance = eMessageImportance.Critical Then
            db = Nothing
        End If

        Return db

    End Function

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Create a new Ecopath model at a requested location.
    ''' </summary>
    ''' <param name="strFileName">The name of the file to create.</param>
    ''' <param name="strModelName">The name of the model to create.</param>
    ''' <returns>An Ecopath database, if succesful.</returns>
    ''' <remarks>
    ''' <para>Note that this will NOT load the new model! For this, 
    ''' <see cref="LoadEcopathModel">cAppLauncher.LoadEcopathModel</see> will need
    ''' to be called.</para>
    ''' <para>This method distills the database type from the provided file name.</para>
    ''' </remarks>
    ''' ---------------------------------------------------------------------------
    Friend Function CreateEcopathModel(ByVal strFileName As String, _
                                        ByVal strModelName As String) As cEwEDatabase
        Return Me.CreateEcopathModel(strFileName, _
                                     strModelName, _
                                     cDataSourceFactory.GetSupportedType(strFileName))
    End Function

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Close the current open Ecopath Model
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Private Function CloseEcopathModel() As Boolean

        If Not String.IsNullOrEmpty(Me.SelectedFileName) Then

            ' Not allowed to terminate core?
            If (Not Me.Core.CloseModel()) Then
                ' #Not allowed: abort
                Return False
            End If

            ' Store last directory
            My.Settings.LastSelectedDirectory = Me.m_strLastSelectedPath
            ' Save form settings
            Me.SaveMainFormSettings()
            ' Close all open documents
            Me.CloseAllDocuments()
            Me.ClearScenarioDropdowns()
            Me.m_uic.Help.Clear()

            ' Reset components
            Me.m_NavPanel.Reset()
            Me.m_StatusPanel.Reset()

            ' Clear the properties cache
            Me.m_uic.PropertyManager.Clear(eCoreComponentType.EcoPath)

            ' Clean up UI bits
            Me.UpdateModelControls()
            Me.ClearScenarioDropdowns()

            ' Take out the trash
            GC.Collect()

            ' Redraw everything immediately
            Me.Refresh()
        End If

        ' Report succes
        Return True

    End Function

#End Region ' Ecopath

#Region " Ecosim "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Load or reload an Ecosim scenario.
    ''' </summary>
    ''' <param name="bTryReuse">Flag indicating whether current scenario should reused, not reloaded, if possible.</param>
    ''' <returns>True if succesful.</returns>
    ''' -----------------------------------------------------------------------
    Friend Function LoadEcosimScenario(Optional ByVal bTryReuse As Boolean = False) As Boolean

        Dim dlg As EcosimScenarioDlg = Nothing
        Dim bSucces As Boolean = False
        Dim es As cEcoSimScenario = Nothing

        ' Try to obtain ecosim scenario to load

        ' Invoked from a command?
        If (Me.m_cmdLoadEcosimScenario.IsInvoking()) Then
            ' #Yes: try to obtain scenario from command
            es = DirectCast(Me.m_cmdLoadEcosimScenario.Tag, cEcoSimScenario)
            ' #No: Are we reloading and an active scenario is present?
        ElseIf (bTryReuse = True) And (Me.Core.ActiveEcosimScenarioIndex >= 0) Then
            Return True
        End If

        ' No scenario found yet?
        If (es Is Nothing) Then
            ' #No scenario: invoke ecosim scenario selection dialog
            dlg = New EcosimScenarioDlg(Me.UIContext, EcosimScenarioDlg.eDialogModeType.LoadScenario)
            If (dlg.ShowDialog() = Windows.Forms.DialogResult.OK) Then

                Select Case dlg.Mode
                    Case EcosimScenarioDlg.eDialogModeType.CreateScenario
                        ' User wants to create a scenario instead
                        Return Me.CreateEcosimScenario(dlg.ScenarioName, dlg.ScenarioDescription, dlg.ScenarioAuthor, dlg.ScenarioContact)
                    Case EcosimScenarioDlg.eDialogModeType.LoadScenario
                        ' User wants to load a scenario
                        es = DirectCast(dlg.Scenario, cEcoSimScenario)
                    Case Else
                        Debug.Assert(False)
                End Select

            End If
        End If

        Return LoadEcosimScenario(es)

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Load an Ecosim scenario.
    ''' </summary>
    ''' <param name="es">The <see cref="cEcoSimScenario">Scenario</see> to load.</param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Private Function LoadEcosimScenario(ByVal es As cEcoSimScenario) As Boolean

        Dim bSucces As Boolean = False

        If (es IsNot Nothing) Then
            ' #Yes: Load it
            Me.SetStatusText(String.Format(My.Resources.STATUS_ECOSIM_LOADING, es.Name), TriState.True)
            bSucces = Me.Core.LoadEcosimScenario(es)
            Me.SetStatusText("", TriState.False)
        End If
        Return bSucces

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="strName"></param>
    ''' <param name="strDescription"></param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Private Function CreateEcosimScenario(ByVal strName As String, ByVal strDescription As String, ByVal strAuthor As String, ByVal strContact As String) As Boolean

        Dim bSucces As Boolean = False

        Me.SetStatusText(String.Format(My.Resources.STATUS_ECOSIM_CREATING, strName), TriState.True)
        bSucces = Me.Core.NewEcosimScenario(strName, strDescription, strAuthor, strContact)
        Me.SetStatusText("", TriState.False)
        Return bSucces

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Invoke the manage time series interface.
    ''' </summary>
    ''' <param name="mode"><see cref="dlgManageTimeSeries.eModeType">Mode</see>
    ''' specifying how to open the interface.</param>
    ''' -----------------------------------------------------------------------
    Private Sub ManageTimeSeries(ByVal mode As dlgManageTimeSeries.eModeType)

        Dim dlg As New dlgManageTimeSeries(Me.UIContext, mode)

        ' Hmm
        dlg.StartPosition = FormStartPosition.CenterParent
        dlg.ShowInTaskbar = False
        dlg.ShowDialog()

    End Sub

#End Region ' Ecosim

#Region " Ecospace "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Load or reload an Ecospace scenario.
    ''' </summary>
    ''' <param name="bTryReuse">Flag indicating whether current scenario should reused, not reloaded, if possible.</param>
    ''' <returns>True if succesful.</returns>
    ''' -----------------------------------------------------------------------
    Friend Function LoadEcospaceScenario(Optional ByVal bTryReuse As Boolean = False) As Boolean

        Dim dlg As EcospaceScenarioDlg = Nothing
        Dim bSucces As Boolean = False
        Dim es As cEcospaceScenario = Nothing

        ' Try to obtain ecospace scenario to load

        ' Invoked from a command?
        If (Me.m_cmdLoadEcospaceScenario.IsInvoking()) Then
            ' #Yes: try to obtain scenario from command
            es = CType(Me.m_cmdLoadEcospaceScenario.Tag, cEcospaceScenario)
            ' #No: Are we reloading and an active scenario is present?
        ElseIf (bTryReuse = True) And (Me.Core.ActiveEcospaceScenarioIndex >= 0) Then
            Return True
        End If

        ' No scenario found yet?
        If (es Is Nothing) Then
            ' #No scenario: invoke ecospace scenario selection dialog
            dlg = New EcospaceScenarioDlg(Me.UIContext, EcospaceScenarioDlg.eDialogModeType.LoadScenario)
            If (dlg.ShowDialog() = Windows.Forms.DialogResult.OK) Then

                Select Case dlg.Mode
                    Case EcospaceScenarioDlg.eDialogModeType.CreateScenario
                        ' User wants to create a scenario instead
                        Return Me.CreateEcospaceScenario(dlg.ScenarioName, dlg.ScenarioDescription, _
                                dlg.ScenarioAuthor, dlg.ScenarioContact, _
                                10, 10, 0, 0, 0.5)
                    Case EcospaceScenarioDlg.eDialogModeType.LoadScenario
                        ' User wants to load a scenario
                        es = DirectCast(dlg.Scenario, cEcospaceScenario)
                    Case Else
                        Debug.Assert(False)
                End Select

            End If
        End If

        Return Me.LoadEcospaceScenario(es)
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="strName"></param>
    ''' <param name="strDescription"></param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Private Function CreateEcospaceScenario(ByVal strName As String, ByVal strDescription As String, _
            ByVal strAuthor As String, ByVal strContact As String, _
            ByVal iNumRows As Integer, ByVal iNumCols As Integer, _
            ByVal sLatTL As Single, ByVal sLonTL As Single, ByVal sCellSize As Single) As Boolean

        Dim bSucces As Boolean = False

        Me.SetStatusText(String.Format(My.Resources.STATUS_ECOSPACE_CREATING, strName), TriState.True)
        bSucces = Me.Core.NewEcospaceScenario(strName, strDescription, _
            strAuthor, strContact, iNumRows, iNumCols, sLatTL, sLonTL, sCellSize)
        Me.SetStatusText("", TriState.False)
        Return bSucces

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="es"></param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Private Function LoadEcospaceScenario(ByVal es As cEcospaceScenario) As Boolean

        Dim bSucces As Boolean = False

        If (es IsNot Nothing) Then
            ' #Yes: Load it
            Me.SetStatusText(String.Format(My.Resources.STATUS_ECOSPACE_LOADING, es.Name), TriState.True)
            bSucces = Me.Core.LoadEcospaceScenario(es)
            Me.SetStatusText("", TriState.False)
        End If
        Return bSucces

    End Function

#End Region ' Ecospace

#Region " Ecotracer "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Load or reload an Ecotracer scenario.
    ''' </summary>
    ''' <param name="bTryReuse">Flag indicating whether current scenario should reused, not reloaded, if possible.</param>
    ''' <returns>True if succesful.</returns>
    ''' -----------------------------------------------------------------------
    Friend Function LoadEcotracerScenario(Optional ByVal bTryReuse As Boolean = False) As Boolean

        Dim dlg As EcotracerScenarioDlg = Nothing
        Dim bSucces As Boolean = False
        Dim es As cEcotracerScenario = Nothing

        ' Prerequesite: Ecosim needs to be loaded
        Me.CoreController.LoadState(eCoreExecutionState.EcosimLoaded)
        ' Not succesful? abort
        If Not Me.Core.StateMonitor.HasEcosimLoaded Then Return False

        ' Try to obtain ecotracer scenario to load

        ' Invoked from a command?
        If (Me.m_cmdLoadEcotracerScenario.IsInvoking()) Then
            ' #Yes: try to obtain scenario from command
            es = CType(Me.m_cmdLoadEcotracerScenario.Tag, cEcotracerScenario)
            ' #No: Are we reloading and an active scenario is present?
        ElseIf (bTryReuse = True) And (Me.Core.ActiveEcotracerScenarioIndex >= 0) Then
            Return True
        End If

        ' No scenario found yet?
        If (es Is Nothing) Then
            ' #No scenario: invoke ecotracer scenario selection dialog
            dlg = New EcotracerScenarioDlg(Me.UIContext, EcotracerScenarioDlg.eDialogModeType.LoadScenario)
            If (dlg.ShowDialog() = Windows.Forms.DialogResult.OK) Then

                Select Case dlg.Mode
                    Case EcotracerScenarioDlg.eDialogModeType.CreateScenario
                        ' User wants to create a scenario instead
                        Return Me.CreateEcotracerScenario(dlg.ScenarioName, dlg.ScenarioDescription, dlg.ScenarioAuthor, dlg.ScenarioContact)
                    Case EcotracerScenarioDlg.eDialogModeType.LoadScenario
                        ' User wants to load a scenario
                        es = DirectCast(dlg.Scenario, cEcotracerScenario)
                    Case Else
                        Debug.Assert(False)
                End Select

            End If
        End If

        Return LoadEcotracerScenario(es)

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="strName"></param>
    ''' <param name="strDescription"></param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Private Function CreateEcotracerScenario(ByVal strName As String, ByVal strDescription As String, ByVal strAuthor As String, ByVal strContact As String) As Boolean

        Dim bSucces As Boolean = False

        Me.SetStatusText(String.Format(My.Resources.STATUS_ECOTRACER_CREATING, strName), TriState.True)
        bSucces = Me.Core.NewEcotracerScenario(strName, strDescription, strAuthor, strContact)
        Me.SetStatusText("", TriState.False)
        Return bSucces

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="es"></param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Private Function LoadEcotracerScenario(ByVal es As cEcotracerScenario) As Boolean

        Dim bSucces As Boolean = False

        If (es IsNot Nothing) Then
            ' #Yes: Load it
            Me.SetStatusText(String.Format(My.Resources.STATUS_ECOTRACER_LOADING, es.Name), TriState.True)
            bSucces = Me.Core.LoadEcotracerScenario(es)
            Me.SetStatusText("", TriState.False)
        End If
        Return bSucces

    End Function

#End Region ' Ecotracer

#Region " Command handlers "

#Region " Generic commands "

    Private Sub OnFileOpen(ByVal cmd As cCommand) Handles m_cmdFileOpen.OnInvoke

        Dim dlgLoad As OpenFileDialog = Nothing
        Dim foc As cFileOpenCommand = DirectCast(cmd, cFileOpenCommand)
        Dim strPath As String = foc.Directory

        If String.IsNullOrEmpty(strPath) Then strPath = Me.m_strLastSelectedPath

        dlgLoad = cEwEFileDialogHelper.OpenFileDialog(foc.Title, foc.FileName, foc.Filters, foc.FilterIndex, strPath, foc.AllowMultiple)

        foc.Result = dlgLoad.ShowDialog()
        foc.FilterIndex = dlgLoad.FilterIndex

        If (foc.Result = Windows.Forms.DialogResult.OK) Then
            foc.FileName = dlgLoad.FileName
            foc.FileNames = dlgLoad.FileNames
            Me.m_strLastSelectedPath = Path.GetDirectoryName(dlgLoad.FileName)
        End If

    End Sub

    Private Sub OnFileSave(ByVal cmd As cCommand) Handles m_cmdFileSave.OnInvoke

        Dim dlgSave As SaveFileDialog = Nothing
        Dim fsc As cFileSaveCommand = DirectCast(cmd, cFileSaveCommand)
        Dim strPath As String = fsc.Directory

        If String.IsNullOrEmpty(strPath) Then strPath = Me.m_strLastSelectedPath

        dlgSave = cEwEFileDialogHelper.SaveFileDialog(fsc.Title, fsc.FileName, fsc.Filters, fsc.FilterIndex, strPath)

        fsc.Result = dlgSave.ShowDialog()
        If (fsc.Result = Windows.Forms.DialogResult.OK) Then
            fsc.FileName = dlgSave.FileName
            fsc.FilterIndex = dlgSave.FilterIndex
            Me.m_strLastSelectedPath = Path.GetDirectoryName(dlgSave.FileName)
            Me.SaveSettings()
        End If

    End Sub

    Private Sub OnDirectoryOpen(ByVal cmd As cCommand) Handles m_cmdDirectoryOpen.OnInvoke

        Dim dlgLoad As FolderBrowserDialog = Nothing
        Dim doc As cDirectoryOpenCommand = DirectCast(cmd, cDirectoryOpenCommand)
        Dim strPath As String = doc.Directory

        If String.IsNullOrEmpty(strPath) Then strPath = Me.m_strLastSelectedPath

        dlgLoad = cEwEFileDialogHelper.FolderBrowserDialog(doc.Description, strPath)

        doc.Result = dlgLoad.ShowDialog()

        If (doc.Result = Windows.Forms.DialogResult.OK) Then
            doc.Directory = dlgLoad.SelectedPath
            Me.m_strLastSelectedPath = Path.GetDirectoryName(doc.Directory)
        End If

    End Sub

    Private Sub OnOpenDocument(ByVal cmd As cCommand) Handles m_cmdNavigate.OnInvoke

        Dim nc As cNavigationCommand = Nothing
        Dim frm As Form = Nothing
        Dim strNavPageID As String = ""
        Dim strNavPageName As String = ""
        Dim strNavHelpURL As String = ""
        Dim tNavClassType As Type = Nothing
        Dim iNavCoreState As eCoreExecutionState = eCoreExecutionState.Idle

        ' Sanity checks
        If cmd Is Nothing Then Return
        If Not (TypeOf cmd Is cNavigationCommand) Then Return

        nc = DirectCast(cmd, cNavigationCommand)

        ' Preserve properties from Nav command, because the content of the nav 
        '    command may change in response to actions in this method
        strNavPageID = nc.PageID
        strNavPageName = nc.PageName
        strNavHelpURL = nc.HelpURL
        tNavClassType = nc.ClassType
        iNavCoreState = nc.CoreExecutionState

        If strNavPageID = "ndScenario" Then
            m_coreController.LoadEcosimScenario()
            Return
        End If

        If strNavPageID = "ndEcospaceScenario" Then
            m_coreController.LoadEcospaceScenario()
            Return
        End If

        If strNavPageID = "ndEcotracerScenario" Then
            Me.CoreController.LoadEcotracerScenario()
            Return
        End If

        ' Check if core can be brought up to par
        If Me.CoreController.LoadState(iNavCoreState) Then
            ' Is form already loaded?
            If Not ActivateForm(strNavPageName) Then

                Me.SetStatusText(My.Resources.GENERIC_STATUS_LOADINGFORM, TriState.True)

                Try
                    ' Load instance of form for selected node
                    frm = Me.LoadFormFromType(strNavPageName, tNavClassType, iNavCoreState)
                    ' Was a form created?
                    If frm IsNot Nothing Then
                        ' #Yes
                        If frm.WindowState = FormWindowState.Minimized Then frm.WindowState = FormWindowState.Normal
                        ' Is this a dockable form? 
                        If (TypeOf frm Is DockContent) And (m_DockPanel.DocumentStyle = DocumentStyle.DockingMdi) Then
                            ' #Yes
                            ' Show the form in the dock panel
                            DirectCast(frm, DockContent).Show(Me.m_DockPanel, DockState.Document)
                            ' Switch help
                            Me.Help.HelpTopic(frm) = strNavHelpURL
                        Else
                            ' Show form
                            frm.MdiParent = Me
                            frm.Show()
                            ' Switch help
                            Me.Help.HelpTopic(frm) = strNavHelpURL
                        End If
                    End If
                Catch ex As Exception
                    ' Whoah!
                End Try

                Me.SetStatusText("", TriState.False)

            End If
        End If

        ' JS Jan2408: Make sure the nav tree correctly reflects the current selected page.
        ' This is important if the navigation to the requested page failed, which can happen
        ' if the core controller is unable to bring the core to the requested state.
        Me.OnTabFocusChanged(Nothing, Nothing)

    End Sub

    ''' <summary>
    ''' Close the current active document.
    ''' </summary>
    Private Sub OnCloseDocument(ByVal cmd As cCommand) Handles m_cmdCloseDocument.OnInvoke
        ' Is the window docked?
        ' Check whether an active document exists; this will occur when all panels are already closed.
        If Not Object.ReferenceEquals(Me.m_DockPanel.ActiveDocument, Nothing) Then
            ' Close active doc
            Me.m_DockPanel.ActiveDocument.DockHandler.Close()
        End If

    End Sub

    ''' <summary>
    ''' Command handler; update the 'close document' command state
    ''' </summary>
    Private Sub OnUpdateCloseDocument(ByVal cmd As cCommand) Handles m_cmdCloseDocument.OnUpdate
        cmd.Enabled = False
        ' Is the window docked?
        cmd.Enabled = Not Object.ReferenceEquals(Me.m_DockPanel.ActiveDocument, Nothing)
    End Sub

    ''' <summary>
    ''' Command handler; closes all closable child forms.
    ''' </summary>
    Private Sub OnCloseAllForms(ByVal cmd As cCommand) Handles m_cmdCloseAllForms.OnInvoke
        ' Close all child forms of the parent
        Me.CloseAllDocuments()
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Event handler, called when the MRU dropdown menu is about to open.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub OnMRUOpening(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_tsmiFileRecent.DropDownOpening
        Me.PopulateMRUDropdown()
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Event handler, called when the MRU dropdown menu has closed.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub OnMRUClosed(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_tsmiFileRecent.DropDownClosed
        ' Ok, do NOT do this here; the dropdown is closed BEFORE a MRU invoke is called. Lovely!
        'Me.ResetMRU()
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Event handler, called when the Exit menu item is selected.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub OnExit(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_tsmiFileExit.Click
        Me.Close()
    End Sub

#End Region ' Generic commands

#Region " File menu commands "

    ''' <summary>
    ''' Create new Ecopath model
    ''' </summary>
    Private Sub OnNewModel(ByVal cmd As cCommand) Handles m_cmdNewModel.OnInvoke

        Dim db As cEwEDatabase = Nothing
        Dim cmdh As cCommandHandler = Me.m_uic.CommandHandler
        Dim cmdFS As cFileSaveCommand = DirectCast(cmdh.GetCommand(cFileSaveCommand.COMMAND_NAME), cFileSaveCommand)

        cmdFS.Invoke(SharedResources.DEFAULT_NEWMODELNAME, "", SharedResources.FILEFILTER_MODEL_SAVE, 1)

        If (cmdFS.Result = Windows.Forms.DialogResult.OK) Then
            ' #Yes: able to create model at selected location?
            db = Me.CreateEcopathModel(cmdFS.FileName, Path.GetFileNameWithoutExtension(cmdFS.FileName))
            If db IsNot Nothing Then
                ' #Yes: Able to load model?
                Me.LoadEcopathModel(cmdFS.FileName, eLoadSourceType.User)
            End If
        End If

    End Sub

    ''' <summary>
    ''' Update new model command state
    ''' </summary>
    Private Sub OnUpdateNewModel(ByVal cmd As cCommand) Handles m_cmdNewModel.OnUpdate
        cmd.Enabled = True
    End Sub

    ''' <summary>
    ''' Open Ecopath model from file
    ''' </summary>
    Private Sub OnLoadModel(ByVal cmd As cCommand) Handles m_cmdLoadModel.OnInvoke

        Dim cmdh As cCommandHandler = Me.m_uic.CommandHandler
        Dim cmdFO As cFileOpenCommand = DirectCast(cmdh.GetCommand(cFileOpenCommand.COMMAND_NAME), cFileOpenCommand)
        Dim strFilter As String = SharedResources.FILEFILTER_MODEL_OPEN

        If cmd.Tag IsNot Nothing Then
            cmdFO.Invoke(Path.GetFileName(CStr(cmd.Tag)), Path.GetDirectoryName(CStr(cmd.Tag)), strFilter, 1)
        Else
            cmdFO.Invoke(strFilter, 1)
        End If

        If (cmdFO.Result = DialogResult.OK) Then

            ' Open the model
            Me.SetStatusText(My.Resources.STATUS_ECOPATH_LOADING, TriState.True)
            Me.LoadEcopathModel(cmdFO.FileName, eLoadSourceType.User)
            Me.SetStatusText("", TriState.False)

        End If

    End Sub

    ''' <summary>
    ''' Save the model
    ''' </summary>
    Private Sub OnSave(ByVal cmd As cCommand) Handles m_cmdSave.OnInvoke
        Me.SetStatusText(My.Resources.STATUS_MODEL_SAVING, TriState.True)
        Me.Core.Save()
        Me.SaveSettings()
        Me.SetStatusText("", TriState.False)
    End Sub

    ''' <summary>
    ''' Update save model command state
    ''' </summary>
    Private Sub OnUpdateSave(ByVal cmd As cCommand) Handles m_cmdSave.OnUpdate
        cmd.Enabled = Me.Core.StateMonitor.IsModified
    End Sub

    ''' <summary>
    ''' Save model under a different name
    ''' </summary>
    Private Sub OnSaveModelAs(ByVal cmd As cCommand) Handles m_cmdSaveModelAs.OnInvoke

        Dim cmdh As cCommandHandler = Me.m_uic.CommandHandler
        Dim cmdFS As cFileSaveCommand = DirectCast(cmdh.GetCommand(cFileSaveCommand.COMMAND_NAME), cFileSaveCommand)

        Dim strFileFilter As String = ""

        ' JS 27Jul08: Only able to save in current file format (save as between formats not supported by the core)
        Select Case cDataSourceFactory.GetSupportedType(Me.SelectedFileName)
            Case eDataSourceTypes.MDB
                ' Only allow saving as MDB
                strFileFilter = SharedResources.FILEFILTER_SAVE_MDB
            Case eDataSourceTypes.ACCDB
                ' Only allow saving as ACCDB
                strFileFilter = SharedResources.FILEFILTER_SAVE_ACCDB
            Case Else
                ' Not supported
                Debug.Assert(False, "Option should not have been available")
                Return
        End Select

        cmdFS.Invoke(SharedResources.DEFAULT_NEWMODELNAME, "", strFileFilter)

        If (cmdFS.Result = Windows.Forms.DialogResult.OK) Then

            ' Save the model
            Me.SetStatusText(My.Resources.STATUS_MODEL_SAVING, TriState.True)
            Try
                SaveEcopathModelAs(cmdFS.FileName)
            Catch ex As Exception

            End Try
            Me.SetStatusText("", TriState.False)

        End If

    End Sub

    ''' <summary>
    ''' Update save model command state
    ''' </summary>
    Private Sub OnUpdateSaveModelAs(ByVal cmd As cCommand) Handles m_cmdSaveModelAs.OnUpdate

        Dim bEnable As Boolean = Me.Core.StateMonitor.HasEcopathLoaded

        Select Case cDataSourceFactory.GetSupportedType(Me.SelectedFileName)
            Case eDataSourceTypes.MDB, eDataSourceTypes.ACCDB
                ' NOP
            Case Else
                ' Only allow save as when file was opened as MDB or ACCDB since the core does
                ' not support (yet: 27jul08) support saving from one file type to another)
                bEnable = False
        End Select
        ' Update command
        cmd.Enabled = bEnable

    End Sub

    ''' <summary>
    ''' Close the current open model
    ''' </summary>
    Private Sub OnCloseModel(ByVal cmd As cCommand) Handles m_cmdCloseModel.OnInvoke
        Me.CloseEcopathModel()
    End Sub

    ''' <summary>
    ''' Update close model command state
    ''' </summary>
    Private Sub OnUpdateCloseModel(ByVal cmd As cCommand) Handles m_cmdCloseModel.OnUpdate
        cmd.Enabled = Me.Core.StateMonitor.HasEcopathLoaded
    End Sub

    ''' <summary>
    ''' Compact a model
    ''' </summary>
    Private Sub OnCompactModel(ByVal cmd As cCommand) Handles m_cmdCompactModel.OnInvoke
        Me.CompactModel()
    End Sub

    ''' <summary>
    ''' Update compact model command state
    ''' </summary>
    Private Sub OnUpdateCompactModel(ByVal cmd As cCommand) Handles m_cmdCompactModel.OnUpdate
        Dim ds As IEwEDataSource = Me.Core.DataSource
        If (ds Is Nothing) Then
            cmd.Enabled = False
        Else
            cmd.Enabled = (Me.Core.StateMonitor.HasEcopathLoaded) And ds.CanCompact(Me.SelectedFileName)
        End If
    End Sub

#End Region ' File commands

#Region " View commands "

    ''' <summary>
    ''' Command handler; toggles presentation mode
    ''' </summary>
    Private Sub OnViewPresentationMode(ByVal cmd As cCommand) Handles m_cmdViewPresentationMode.OnInvoke

        cmd.Checked = Not cmd.Checked

        Dim bPresMode As Boolean = cmd.Checked

        Me.SuspendLayout()

        If (bPresMode) Then
            With Me.m_fspPresentationMode
                .ShowMenu = Me.m_menuMain.Visible : Me.m_menuMain.Visible = Not My.Settings.PresentationModeHideMainMenu
                .ShowModelBar = Me.m_tsModel.Visible : Me.m_tsModel.Visible = Not My.Settings.PresentationModeHideModelBar
                .ShowStatusBar = Me.m_ssMain.Visible : Me.m_ssMain.Visible = Not My.Settings.PresentationModeHideStatusBar
                .ShowNavPanel = Me.m_NavPanel.IsHiding : Me.m_NavPanel.AutoHide = My.Settings.PresentationModeCollapseNavPanel
                .FormState = Me.WindowState : Me.WindowState = FormWindowState.Maximized
                .BorderStyle = Me.FormBorderStyle : Me.FormBorderStyle = Windows.Forms.FormBorderStyle.None
            End With
            'Me.TopMost = True
            Me.ControlBox = False
        Else
            With Me.m_fspPresentationMode
                Me.WindowState = .FormState
                Me.FormBorderStyle = .BorderStyle
                Me.m_menuMain.Visible = .ShowMenu
                Me.m_tsModel.Visible = .ShowModelBar
                Me.m_ssMain.Visible = .ShowStatusBar
                Me.m_NavPanel.AutoHide = .ShowNavPanel
            End With
            'Me.TopMost = False
            Me.ControlBox = True
        End If

        ' BorderStyle screws up dockpanel when there it contains panels.

        Me.ResumeLayout()

    End Sub

    ''' <summary>
    ''' Command update handler; enables and disables the 
    ''' <see cref="m_cmdViewPresentationMode">View Presentation Mode command</see>.
    ''' </summary>
    Private Sub OnUpdateViewPresentationMode(ByVal cmd As EwEUtils.Commands.cCommand) Handles m_cmdViewPresentationMode.OnUpdate
        ' NOP
    End Sub

    ''' <summary>
    ''' Command handler; toggles main statusbar visibility
    ''' </summary>
    Private Sub OnViewMainStatusbar(ByVal cmd As cCommand) Handles m_cmdViewStatusbar.OnInvoke
        Me.m_ssMain.Visible = Not cmd.Checked
    End Sub

    ''' <summary>
    ''' Command update handler; enables and disables the <see cref="m_cmdViewStatusbar">View Statusbar command</see>.
    ''' </summary>
    Private Sub OnUpdateViewMainStatusbar(ByVal cmd As EwEUtils.Commands.cCommand) Handles m_cmdViewStatusbar.OnUpdate
        cmd.Checked = Me.m_ssMain.Visible
    End Sub

    ''' <summary>
    ''' Command handler; toggles main menu visibility
    ''' </summary>
    Private Sub OnViewMenu(ByVal cmd As cCommand) Handles m_cmdViewMenu.OnInvoke
        Me.m_menuMain.Visible = Not cmd.Checked
    End Sub

    ''' <summary>
    ''' Command update handler; enables and disables the <see cref="m_cmdViewMenu">View menu command</see>.
    ''' </summary>
    Private Sub OnUpdateViewMenu(ByVal cmd As EwEUtils.Commands.cCommand) Handles m_cmdViewMenu.OnUpdate
        cmd.Checked = Me.m_menuMain.Visible
    End Sub

    ''' <summary>
    ''' Command handler; shows the start page.
    ''' </summary>
    Private Sub OnViewStartPage(ByVal cmd As cCommand) Handles m_cmdViewStartPanel.OnInvoke
        ' If m_startPage has been closed, create a new reference. 
        If m_StartPage.IsDisposed() Then
            m_StartPage = New frmWebBrowser(Me.UIContext)
        End If

        If m_DockPanel.DocumentStyle = DocumentStyle.DockingMdi Then
            If cmd.Checked Then
                m_StartPage.Close()
            Else
                m_StartPage.Show(m_DockPanel, DockState.Document)
            End If
        Else
            m_StartPage.MdiParent = Me
            m_StartPage.StartPosition = FormStartPosition.WindowsDefaultLocation
            If cmd.Checked Then
                m_StartPage.Close()
            Else
                m_StartPage.Show()
            End If
        End If
    End Sub

    ''' <summary>
    ''' Command update handler; manages the <see cref="m_cmdViewStartPanel">View Start Page command</see> state.
    ''' </summary>
    Private Sub OnUpdateViewStartPage(ByVal cmd As cCommand) Handles m_cmdViewStartPanel.OnUpdate
        cmd.Checked = Not m_StartPage.IsDisposed() And Me.m_StartPage.Visible
    End Sub

    ''' <summary>
    ''' Command handler; shows the navigation panel.
    ''' </summary>
    Private Sub OnViewNavPane(ByVal cmd As cCommand) Handles m_cmdViewNavPane.OnInvoke
        If cmd.Checked Then
            m_NavPanel.DockState = DockState.Hidden
        Else
            m_NavPanel.Show(m_DockPanel, DockState.DockLeft)
        End If
    End Sub

    ''' <summary>
    ''' Command update handler; manages the <see cref="m_cmdViewStartPanel">View Navigation Panel command</see> state.
    ''' </summary>
    Private Sub OnUpdateViewNavPane(ByVal cmd As cCommand) Handles m_cmdViewNavPane.OnUpdate
        cmd.Checked = (m_NavPanel.DockState <> DockState.Hidden)
    End Sub

    ''' <summary>
    ''' Show the remark pane
    ''' </summary>
    Private Sub OnViewRemarkPane(ByVal cmd As cCommand) Handles m_cmdViewRemarkPane.OnInvoke
        If cmd.Checked Then
            m_RemarkPanel.DockState = DockState.Hidden
        Else
            ' ToDo: Restore last dock state
            m_RemarkPanel.Show(m_DockPanel, DockState.DockRightAutoHide)
        End If
    End Sub

    Private Sub OnUpdateViewRemarkPane(ByVal cmd As cCommand) Handles m_cmdViewRemarkPane.OnUpdate
        cmd.Checked = (m_RemarkPanel.DockState <> DockState.Hidden)
    End Sub

    ''' <summary>
    ''' Show the status panel
    ''' </summary>
    Private Sub OnViewStatusPane(ByVal cmd As cCommand) Handles m_cmdViewStatusPane.OnInvoke
        If cmd.Checked Then
            m_StatusPanel.DockState = DockState.Hidden
        Else
            ' ToDo: Restore last dock state
            m_StatusPanel.Show(m_DockPanel, DockState.DockBottomAutoHide)
        End If
    End Sub

    Private Sub OnUpdateViewStatusPane(ByVal cmd As cCommand) Handles m_cmdViewStatusPane.OnUpdate
        cmd.Checked = (m_StatusPanel.DockState <> DockState.Hidden)
    End Sub

    ''' <summary>
    ''' Show the button bar
    ''' </summary>
    Private Sub OnViewModelBar(ByVal cmd As cCommand) Handles m_cmdViewModelBar.OnInvoke
        Me.m_tsModel.Visible = Not cmd.Checked
    End Sub

    Private Sub OnUpdateViewModelBar(ByVal cmd As cCommand) Handles m_cmdViewModelBar.OnUpdate
        cmd.Checked = Me.m_tsModel.Visible
    End Sub

#End Region ' View commands

#Region " Tools commands "

    ''' <summary>
    ''' Open the EwE6 option dialog
    ''' </summary>
    Private Sub OptionsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_tsmiOptions.Click

        Dim dlgOptions As New dlgOptions(Me.UIContext)
        ' FG Nov 15, 2006: Should not use Show instead of using ShowDialog and specify its owner so it will
        ' be displayed at the specified location
        dlgOptions.ShowDialog(Me)
        Me.SaveSettings()

    End Sub

#End Region ' Tools commands

#Region " Help commands "

    ''' <summary>
    ''' Command handler; invokes the About... dialog.
    ''' </summary>
    Private Sub OnShowAboutDialog(ByVal cmd As EwEUtils.Commands.cCommand) Handles m_cmdHelpAbout.OnInvoke
        Dim dlgAbout As New frmAboutEwE(Me.UIContext)
        Me.Help.HelpTopic(dlgAbout) = ""
        dlgAbout.ShowDialog(Me)
    End Sub

    Private Sub OnHelpTOC(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_tsmiHelpContents.Click
        Me.Help.ShowHelp(HelpNavigator.TableOfContents)
    End Sub

    Private Sub OnHelpIndex(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_tsmiHelpIndex.Click
        Me.Help.ShowHelp(HelpNavigator.KeywordIndex)
    End Sub

    Private Sub OnHelpSearch(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_tsmiHelpSearch.Click
        Me.Help.ShowHelp(HelpNavigator.Find)
    End Sub

    Private Sub OnReportBug(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_tsmiHelpBugReport.Click
        Dim strError As String = ""
        If Not cBugReporter.InvokeBugReport(My.Resources.GENERIC_CAPTION, "mailto:ewedevteam@gmail.com", Me.m_pluginManager) Then
            Dim msg As New cMessage(My.Resources.PROMPT_ERROR_BUG_REPORT_NO_MAIL_CLIENT, _
                                    eMessageType.NotSet, _
                                    eCoreComponentType.External, _
                                    eMessageImportance.Warning)
            Me.Core.Messages.SendMessage(msg)
        End If
    End Sub

#End Region ' Main Menu - Help

#Region " Ecopath commands "

    ''' <summary>
    ''' Command handler; invokes the edit groups interface
    ''' </summary>
    Private Sub OnEditGroups(ByVal cmd As cCommand) Handles m_cmdEditGroups.OnInvoke
        Dim dlg As New EditGroups(Me.UIContext, DirectCast(cmd.Tag, cEcoPathGroupInput))
        Me.Help.HelpTopic(dlg) = "Edit groups.htm"
        dlg.ShowDialog(Me)
    End Sub

    ''' <summary>
    ''' Command update handler; enables and disables the <see cref="m_cmdEditGroups">Edit Groups command</see>.
    ''' </summary>
    Private Sub OnUpdateEditGroups(ByVal cmd As EwEUtils.Commands.cCommand) Handles m_cmdEditGroups.OnUpdate
        cmd.Enabled = Me.Core.StateMonitor.HasEcopathLoaded()
    End Sub

    ''' <summary>
    ''' Command handler; invokes the edit multi stanza interface
    ''' </summary>
    Private Sub OnEditMultiStanza(ByVal cmd As cCommand) Handles m_cmdEditMultiStanza.OnInvoke

        ' Test if all stanza groups have at least one life stage
        Dim bAllStanzaComplete As Boolean = True
        For i As Integer = 0 To Me.Core.nStanzas - 1
            bAllStanzaComplete = bAllStanzaComplete And (Me.Core.StanzaGroups(i).NStanzas > 0)
        Next

        If bAllStanzaComplete = False Then
            If Me.AskFeedback(My.Resources.PROMPT_STANZA_MISSING_LIFESTAGES, _
                              eMessageImportance.Warning, eCoreComponentType.Core, _
                              cFeedbackMessage.eReplyStyle.YES_NO) = cFeedbackMessage.eReply.YES Then
                Me.m_cmdEditGroups.Invoke()
            End If
            Return
        End If

        Dim dlg As New EditMultiStanza(Me.UIContext)
        Me.Help.HelpTopic(dlg) = "Edit multi stanza.htm"
        dlg.ShowDialog(Me)
    End Sub

    ''' <summary>
    ''' Command update handler; enables and disables the <see cref="m_cmdEditMultiStanza">Edit Multi-stanza command</see>.
    ''' </summary>
    Private Sub OnUpdateMultiStanza(ByVal cmd As EwEUtils.Commands.cCommand) Handles m_cmdEditMultiStanza.OnUpdate
        ' MultiStanza can be edited when ecopath has loaded and the core has more than one stanza group
        cmd.Enabled = (Me.Core.StateMonitor.HasEcopathLoaded() = True) And _
                      (Me.Core.nStanzas > 0)
    End Sub

    ''' <summary>
    ''' Command handler; invokes the edit fleets interface
    ''' </summary>
    Private Sub OnEditFleets(ByVal cmd As cCommand) Handles m_cmdEditFleets.OnInvoke
        Try
            Dim dlg As New EditFleets(Me.UIContext, DirectCast(cmd.Tag, cFleetInput))
            Me.Help.HelpTopic(dlg) = "Edit fleets.htm"
            dlg.ShowDialog(Me)
        Catch ex As Exception
            ' Woops
            Debug.Assert(False)
        End Try
    End Sub

    ''' <summary>
    ''' Command update handler; enables and disables the <see cref="m_cmdEditFleets">Edit Fleets command</see>.
    ''' </summary>
    Private Sub OnUpdateEditFleets(ByVal cmd As EwEUtils.Commands.cCommand) _
        Handles m_cmdEditFleets.OnUpdate
        cmd.Enabled = Me.Core.StateMonitor.HasEcopathLoaded()
    End Sub

    Private Sub OnEditPedigreeLevels(ByVal cmd As cCommand) _
        Handles m_cmdEditPedigree.OnInvoke
        Dim dlg As New dlgEditPedigree(Me.UIContext)
        dlg.ShowDialog(Me)
    End Sub

    Private Sub OnUpdateEditPedigreeLevels(ByVal cmd As cCommand) _
        Handles m_cmdEditPedigree.OnUpdate
        cmd.Enabled = Me.Core.StateMonitor.HasEcopathLoaded()
    End Sub

    Private Sub OnEditTaxa(ByVal cmd As cCommand) _
        Handles m_cmdEditTaxa.OnInvoke
        Dim dlg As New dlgEditGroupTaxon(Me.UIContext, DirectCast(cmd.Tag, cEcoPathGroupInput))
        dlg.ShowDialog(Me)
    End Sub

    Private Sub OnUpdateEditTaxa(ByVal cmd As cCommand) _
        Handles m_cmdEditTaxa.OnUpdate
        cmd.Enabled = Me.Core.StateMonitor.HasEcopathLoaded()
    End Sub

    Private Sub OnDisplayShowHideItems(ByVal cmd As cCommand) _
        Handles m_cmdShowHideItems.OnInvoke
        Dim dlg As New dlgShowHideItems(Me.UIContext, m_cmdShowHideItems.ShowGroups, m_cmdShowHideItems.ShowTotals)
        dlg.ShowDialog()
    End Sub

    Private Sub OnUpdateShowHideItems(ByVal cmd As cCommand) _
        Handles m_cmdShowHideItems.OnUpdate
        cmd.Enabled = Me.Core.StateMonitor.HasEcopathLoaded()
    End Sub

#End Region ' Main Menu - File

#Region " Ecosim commands "

    ''' <summary>
    ''' Command handler; creates a new Ecosim scenario
    ''' </summary>
    Private Sub OnNewEcosimScenario(ByVal cmd As cCommand) Handles m_cmdNewEcosimScenario.OnInvoke

        Dim dlg As New EcosimScenarioDlg(Me.UIContext, EcosimScenarioDlg.eDialogModeType.CreateScenario)

        If dlg.ShowDialog = Windows.Forms.DialogResult.OK Then

            Select Case dlg.Mode
                Case EcosimScenarioDlg.eDialogModeType.CreateScenario
                    Me.CreateEcosimScenario(dlg.ScenarioName, dlg.ScenarioDescription, dlg.ScenarioAuthor, dlg.ScenarioContact)
                Case EcosimScenarioDlg.eDialogModeType.LoadScenario
                    Me.LoadEcosimScenario(DirectCast(dlg.Scenario, cEcoSimScenario))
                Case Else
                    Debug.Assert(False)
            End Select

        End If

    End Sub

    ''' <summary>
    ''' Command update handler; takes care of enabling and disabling the
    ''' <see cref="m_cmdNewEcosimScenario">New Ecosim Scenario</see> command.
    ''' </summary>
    Private Sub OnUpdateNewEcosimScenario(ByVal cmd As cCommand) Handles m_cmdNewEcosimScenario.OnUpdate
        cmd.Enabled = Me.Core.StateMonitor.HasEcopathLoaded
    End Sub

    ''' <summary>
    ''' Command handler; loads a new Ecosim scenario
    ''' </summary>
    Private Sub OnLoadEcosimScenario(ByVal cmd As cCommand) Handles m_cmdLoadEcosimScenario.OnInvoke
        Me.CoreController.LoadEcosimScenario()
    End Sub

    ''' <summary>
    ''' Command update handler; takes care of enabling and disabling the 
    ''' <see cref="m_cmdLoadEcosimScenario">Load Ecosim Scenario</see> command.
    ''' </summary>
    Private Sub OnUpdateLoadEcosimScenario(ByVal cmd As cCommand) Handles m_cmdLoadEcosimScenario.OnUpdate
        cmd.Enabled = Me.Core.StateMonitor.HasEcopathLoaded
    End Sub

    '''' <summary>
    '''' Command handler; saves an Ecosim scenario
    '''' </summary>
    'Private Sub OnSaveEcosimScenario(ByVal cmd As cCommand) Handles m_cmdSaveEcosimScenario.OnInvoke
    '    Dim strStatus As String = String.Format(My.Resources.STATUS_ECOSIM_SAVING, Me.Core.EcosimScenarios(Me.Core.ActiveEcosimScenarioIndex).Name)
    '    Me.SetStatusText(strStatus, TriState.True)
    '    Try
    '        Me.Core.SaveEcosimScenario()
    '    Catch ex As Exception

    '    End Try
    '    Me.SetStatusText("", TriState.False)
    'End Sub

    '''' <summary>
    '''' Command update handler; enables and disables the 'save ecosim scenario' command
    '''' </summary>
    'Private Sub OnUpdateSaveEcosimScenario(ByVal cmd As cCommand) Handles m_cmdSaveEcosimScenario.OnUpdate
    '    cmd.Enabled = Me.Core.StateMonitor.IsEcosimModified
    'End Sub

    ''' <summary>
    ''' Command handler; saves an Ecosim scenario to a new name
    ''' </summary>
    Private Sub OnSaveEcosimScenarioAs(ByVal cmd As cCommand) Handles m_cmdSaveEcosimScenarioAs.OnInvoke

        Dim dlg As New EcosimScenarioDlg(Me.UIContext, EcosimScenarioDlg.eDialogModeType.SaveScenario, _
                Me.Core.EcosimScenarios(Me.Core.ActiveEcosimScenarioIndex))

        If dlg.ShowDialog() = Windows.Forms.DialogResult.OK Then
            ' Overwriting?
            If dlg.Scenario IsNot Nothing Then
                ' Prompt for overwrite confirmation
                If MessageBox.Show(String.Format(My.Resources.SCENARIO_CONFIRMOVERWRITE_PROMPT, dlg.ScenarioName), _
                        My.Resources.SCENARIO_CONFIRMOVERWRITE_CAPTION, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then

                    ' #Overwrite
                    Me.SetStatusText(String.Format(My.Resources.STATUS_ECOSIM_SAVING, dlg.ScenarioName), TriState.True)
                    Try
                        Me.Core.SaveEcosimScenarioAs(dlg.ScenarioName, dlg.ScenarioDescription)
                    Catch ex As Exception

                    End Try
                    Me.SetStatusText("", TriState.False)

                End If
                ' User does not want to overwrite? Abort
                Return
            End If

            ' Add scenario under new name
            Me.SetStatusText(String.Format(My.Resources.STATUS_ECOSIM_CREATING, dlg.ScenarioName), TriState.True)
            Try
                Me.Core.SaveEcosimScenarioAs(dlg.ScenarioName, dlg.ScenarioDescription)
            Catch ex As Exception

            End Try
            Me.SetStatusText("", TriState.False)

        End If

    End Sub

    ''' <summary>
    ''' Command update handler; enables and disables the 'save ecosim scenario as' command
    ''' </summary>
    Private Sub OnUpdateSaveEcosimScenarioAs(ByVal cmd As cCommand) _
        Handles m_cmdSaveEcosimScenarioAs.OnUpdate
        cmd.Enabled = Me.Core.StateMonitor.HasEcosimLoaded
    End Sub

    ''' <summary>
    ''' Command handler; deletes an Ecosim scenario 
    ''' </summary>
    Private Sub OnInvokeDeleteEcosimScenario(ByVal cmd As cCommand) _
         Handles m_cmdDeleteEcosimScenario.OnInvoke
        Dim dlg As New EcosimScenarioDlg(Me.UIContext, EcosimScenarioDlg.eDialogModeType.DeleteScenario)
        dlg.ShowDialog(Me)
    End Sub

    ''' <summary>
    ''' Command update handler; enables and disables the 'delete ecosim scenario' command
    ''' </summary>
    Private Sub OnUpdateDeleteEcosimScenario(ByVal cmd As cCommand) _
           Handles m_cmdDeleteEcosimScenario.OnUpdate
        cmd.Enabled = Me.Core.StateMonitor.HasEcopathLoaded And Me.Core.EcosimScenarioCount > 0
    End Sub

    ''' <summary>
    ''' Command handler; invokes the import time series dialog.
    ''' </summary>
    Private Sub m_cmdImportTimeSeries_OnInvoke(ByVal cmd As EwEUtils.Commands.cCommand) _
        Handles m_cmdImportTimeSeries.OnInvoke
        Me.ManageTimeSeries(dlgManageTimeSeries.eModeType.Import)
    End Sub

    ''' <summary>
    ''' Command update handler; enables and disables the <see cref="m_cmdImportTimeSeries">Import TimeSeries command</see>.
    ''' </summary>
    Private Sub m_cmdImportTimeSeries_OnUpdate(ByVal cmd As EwEUtils.Commands.cCommand) Handles m_cmdImportTimeSeries.OnUpdate
        cmd.Enabled = Me.Core.StateMonitor.HasEcosimLoaded()
    End Sub

    ''' <summary>
    ''' Command handler; exports the currently loaded time series dataset to a CSV file.
    ''' </summary>
    Private Sub m_cmdExportTimeSeries_OnInvoke(ByVal cmd As EwEUtils.Commands.cCommand) _
        Handles m_cmdExportTimeSeries.OnInvoke

        Dim sfd As New SaveFileDialog()
        Dim tsw As New cTimeSeriesCSVWriter(Me.Core)

        sfd.Filter = SharedResources.FILEFILTER_CSV
        sfd.FileName = tsw.DefaultFileName
        sfd.CheckFileExists = False
        sfd.CheckPathExists = True
        sfd.OverwritePrompt = True

        If (sfd.ShowDialog(Me) = Windows.Forms.DialogResult.OK) Then
            tsw.Write(sfd.FileName, ",", ".")
        End If

    End Sub

    ''' <summary>
    ''' Command update handler; enables and disables the <see cref="m_cmdExportTimeSeries">Export TimeSeries command</see>.
    ''' </summary>
    Private Sub m_cmdExportTimeSeries_OnUpdate(ByVal cmd As EwEUtils.Commands.cCommand) _
        Handles m_cmdExportTimeSeries.OnUpdate
        cmd.Enabled = (Me.Core.ActiveTimeSeriesDatasetIndex > -1)
    End Sub

    ''' <summary>
    ''' Command handler; invokes the apply time series dialog.
    ''' </summary>
    Private Sub m_cmdWeightTimeSeries_OnInvoke(ByVal cmd As EwEUtils.Commands.cCommand) Handles m_cmdWeightTimeSeries.OnInvoke
        Me.ManageTimeSeries(dlgManageTimeSeries.eModeType.Weight)
    End Sub

    ''' <summary>
    ''' Command update handler; enables and disables the <see cref="m_cmdWeightTimeSeries">Apply TimeSeries command</see>.
    ''' </summary>
    Private Sub m_cmdWeightTimeSeries_OnUpdate(ByVal cmd As EwEUtils.Commands.cCommand) Handles m_cmdWeightTimeSeries.OnUpdate
        ' JS 23sept08: dialog will switch to load mode if no ts present
        cmd.Enabled = Me.Core.StateMonitor.HasEcosimLoaded() ' And Me.Core.HasTimeSeries()
    End Sub

    ''' <summary>
    ''' Command handler; invokes the load time series dialog, or loads a time
    ''' series dataset if this dataset is provided as a tag to the command.
    ''' </summary>
    Private Sub m_cmdLoadTimeSeries_OnInvoke(ByVal cmd As EwEUtils.Commands.cCommand) _
        Handles m_cmdLoadTimeSeries.OnInvoke

        If Not Me.m_coreController.LoadState(eCoreExecutionState.EcosimLoaded) Then Return

        If (Me.m_cmdLoadTimeSeries.Tag Is Nothing) Then
            Me.ManageTimeSeries(dlgManageTimeSeries.eModeType.Load)
        ElseIf (TypeOf Me.m_cmdLoadTimeSeries.Tag Is cTimeSeriesDataset) Then
            Me.Core.LoadTimeSeries(DirectCast(Me.m_cmdLoadTimeSeries.Tag, cTimeSeriesDataset), True)
        End If

    End Sub

    ''' <summary>
    ''' Command update handler; enables and disables the <see cref="m_cmdLoadTimeSeries">Load TimeSeries command</see>.
    ''' </summary>
    Private Sub m_cmdLoadTimeSeries_OnUpdate(ByVal cmd As EwEUtils.Commands.cCommand) _
        Handles m_cmdLoadTimeSeries.OnUpdate

        cmd.Enabled = Me.Core.StateMonitor.HasEcopathLoaded()

    End Sub

    Private Sub OnExportEcosimResultsToCSV(ByVal cmd As cCommand) _
        Handles m_cmdExportEcosimResultsToCSV.OnInvoke

        Dim cmdh As cCommandHandler = Me.m_uic.CommandHandler
        Dim cmdOD As cDirectoryOpenCommand = DirectCast(cmdh.GetCommand(cDirectoryOpenCommand.COMMAND_NAME), cDirectoryOpenCommand)
        Dim iGroup As Integer = cCore.NULL_VALUE
        Dim bSaveAnnual As Boolean = False
        Dim writer As cEcosimResultWriter = Nothing

        cmdOD.Invoke("", My.Resources.ECOSIM_PROMPT_SAVEDESTINATION)

        If (cmdOD.Result <> Windows.Forms.DialogResult.OK) Then Return
        If (String.IsNullOrEmpty(cmdOD.Directory)) Then Return

        Select Case MsgBox(My.Resources.ECOSIM_PROMPT_SAVEANNUAL, MsgBoxStyle.Question Or MsgBoxStyle.YesNoCancel)
            Case MsgBoxResult.Yes
                bSaveAnnual = True
            Case MsgBoxResult.No
                bSaveAnnual = False
            Case MsgBoxResult.Cancel
                Return
        End Select

        Try
            If cmd.Tag IsNot Nothing Then
                iGroup = CInt(cmd.Tag)
            End If
        Catch ex As Exception
        End Try

        writer = New cEcosimResultWriter(Me.UIContext.Core)
        writer.WriteResults(cmdOD.Directory, bSaveAnnual, iGroup)
        writer = Nothing

    End Sub

    Private Sub OnExportEcosimResultsToCSVUpdate(ByVal cmd As cCommand) _
        Handles m_cmdExportEcosimResultsToCSV.OnUpdate
        cmd.Enabled = Me.Core.StateMonitor.HasEcosimRan
    End Sub

    Private Sub OnEstimateVsInvoke(ByVal cmd As EwEUtils.Commands.cCommand) _
        Handles m_cmdEstimateVs.OnInvoke
        Dim dlg As New dlgEstimateVs(Me.UIContext)
        dlg.ShowDialog(Me)
    End Sub

    Private Sub OnEstimateVsUpdate(ByVal cmd As EwEUtils.Commands.cCommand) _
        Handles m_cmdEstimateVs.OnUpdate
        cmd.Enabled = Me.Core.StateMonitor.HasEcosimLoaded()
    End Sub

#End Region ' Ecosim commands

#Region " Ecospace commands "

    Private Sub OnNewEcospaceScenario(ByVal cmd As cCommand) _
        Handles m_cmdNewEcospaceScenario.OnInvoke

        Dim dlg As New EcospaceScenarioDlg(Me.UIContext, EcospaceScenarioDlg.eDialogModeType.CreateScenario)

        If dlg.ShowDialog = Windows.Forms.DialogResult.OK Then

            Select Case dlg.Mode
                Case EcospaceScenarioDlg.eDialogModeType.CreateScenario
                    Me.CreateEcospaceScenario(dlg.ScenarioName, dlg.ScenarioDescription, _
                            dlg.ScenarioAuthor, dlg.ScenarioContact, _
                            10, 10, 0, 0, 0.5)
                Case EcospaceScenarioDlg.eDialogModeType.LoadScenario
                    Me.LoadEcospaceScenario(DirectCast(dlg.Scenario, cEcospaceScenario))
                Case EcospaceScenarioDlg.eDialogModeType.DeleteScenario
                    Return
                Case Else
                    Debug.Assert(False)
            End Select

        End If

    End Sub

    Private Sub OnUpdateNewEcospaceScenario(ByVal cmd As cCommand) _
        Handles m_cmdNewEcospaceScenario.OnUpdate
        cmd.Enabled = Me.Core.StateMonitor.HasEcosimLoaded
    End Sub

    Private Sub OnLoadEcospaceScenario(ByVal cmd As cCommand) _
        Handles m_cmdLoadEcospaceScenario.OnInvoke
        Me.CoreController.LoadEcospaceScenario()
    End Sub

    Private Sub OnUpdateLoadEcospaceScenario(ByVal cmd As cCommand) _
        Handles m_cmdLoadEcospaceScenario.OnUpdate
        cmd.Enabled = Me.Core.StateMonitor.HasEcopathLoaded
    End Sub

    ''' <summary>
    ''' Command handler; saves the current active Ecospace scenario under a new name.
    ''' </summary>
    Private Sub OnSaveEcospaceScenarioAs(ByVal cmd As cCommand) _
        Handles m_cmdSaveEcospaceScenarioAS.OnInvoke

        Dim dlg As New EcospaceScenarioDlg(Me.UIContext, _
                                           EcospaceScenarioDlg.eDialogModeType.SaveScenario, _
                                           Me.Core.EcospaceScenarios(Me.Core.ActiveEcospaceScenarioIndex))
        Dim scenarioTarget As cEcospaceScenario = Nothing

        If dlg.ShowDialog() = Windows.Forms.DialogResult.OK Then
            ' Has valid name?
            If Not String.IsNullOrEmpty(dlg.ScenarioName) Then
                ' #Cool. Now check if this will overwrite a scenario with the same name (case insensitive)
                scenarioTarget = Nothing
                For iScenario As Integer = 1 To Me.Core.EcospaceScenarioCount
                    If (String.Compare(Me.Core.EcospaceScenarios(iScenario).Name, dlg.ScenarioName, True) = 0) Then
                        scenarioTarget = Me.Core.EcospaceScenarios(iScenario)
                    End If
                Next

                ' About to overwrite?
                If (Not Object.ReferenceEquals(scenarioTarget, Nothing)) Then
                    ' #Yes: prompt for overwrite confirmation
                    If MessageBox.Show(String.Format(My.Resources.SCENARIO_CONFIRMOVERWRITE_PROMPT, dlg.ScenarioName), _
                            My.Resources.SCENARIO_CONFIRMOVERWRITE_CAPTION, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then

                        ' #Overwrite
                        Me.SetStatusText(String.Format(My.Resources.STATUS_ECOSPACE_SAVING, dlg.ScenarioName), TriState.True)
                        Try
                            Me.Core.SaveEcospaceScenarioAs(dlg.ScenarioName, dlg.ScenarioDescription)
                        Catch ex As Exception

                        End Try
                        Me.SetStatusText("", TriState.False)

                    End If
                    ' User does not want to overwrite? Abort
                    Return
                End If

                ' Add scenario
                Me.SetStatusText(String.Format(My.Resources.STATUS_ECOSPACE_CREATING, dlg.ScenarioName), TriState.True)
                Try
                    Me.Core.SaveEcospaceScenarioAs(dlg.ScenarioName, dlg.ScenarioDescription)
                Catch ex As Exception

                End Try
                Me.SetStatusText("", TriState.False)

            End If
        End If

    End Sub

    ''' <summary>
    ''' Command update handler; enables and disables the 
    ''' <see cref="m_cmdSaveEcospaceScenarioAs">Save Ecospace Scenario As</see> command.
    ''' </summary>
    Private Sub OnUpdateSaveEcospaceScenarioAs(ByVal cmd As cCommand) Handles m_cmdSaveEcospaceScenarioAS.OnUpdate
        cmd.Enabled = Me.Core.StateMonitor.HasEcospaceLoaded
    End Sub

    '''' <summary>
    '''' Command handler; saves the current active Ecospace scenario.
    '''' </summary>
    'Private Sub OnSaveEcospaceScenario(ByVal cmd As cCommand) Handles m_cmdSaveEcospaceScenario.OnInvoke
    '    Dim strStatus As String = String.Format(My.Resources.STATUS_ECOSPACE_SAVING, Me.Core.EcospaceScenarios(Me.Core.ActiveEcospaceScenarioIndex).Name)
    '    Me.SetStatusText(strStatus, TriState.True)
    '    Try
    '        Me.Core.SaveEcospaceScenario()
    '    Catch ex As Exception

    '    End Try
    '    Me.SetStatusText("", TriState.False)
    'End Sub

    '''' <summary>
    '''' Command update handler; enables and disables the 
    '''' <see cref="m_cmdSaveEcospaceScenario">Save Ecospace Scenario</see> command.
    '''' </summary>
    'Private Sub OnUpdateSaveEcospaceScenario(ByVal cmd As cCommand) Handles m_cmdSaveEcospaceScenario.OnUpdate
    '    cmd.Enabled = Me.Core.StateMonitor.IsEcospaceModified
    'End Sub

    ''' <summary>
    ''' Command handler; deletes an Ecosim scenario 
    ''' </summary>
    Private Sub OnInvokeDeleteEcospaceScenario(ByVal cmd As cCommand) _
         Handles m_cmdDeleteEcospaceScenario.OnInvoke
        Dim dlg As New EcospaceScenarioDlg(Me.UIContext, EwEScenarioDlg.eDialogModeType.DeleteScenario)
        dlg.ShowDialog(Me)
    End Sub

    ''' <summary>
    ''' Command update handler; enables and disables the 'delete ecospace scenario' command
    ''' </summary>
    Private Sub OnUpdateDeleteEcospaceScenario(ByVal cmd As cCommand) _
           Handles m_cmdDeleteEcospaceScenario.OnUpdate
        cmd.Enabled = Me.Core.StateMonitor.HasEcopathLoaded And Me.Core.EcospaceScenarioCount > 0
    End Sub

    ''' <summary>
    ''' Command handler; invokes the Ecospace edit basemap dialog.
    ''' </summary>
    Private Sub OnEditEcospaceBasemap(ByVal cmd As cCommand) Handles m_cmdEditBasemap.OnInvoke
        Dim dlg As New dlgEditBasemap(Me.UIContext)
        Me.Help.HelpTopic(dlg) = "Edit basemap.htm"
        dlg.ShowDialog(Me)
    End Sub

    ''' <summary>
    ''' Command handler; handles access to the Ecospace edit basemap dialog.
    ''' </summary>
    Private Sub OnUpdateEditEcospaceBasemap(ByVal cmd As cCommand) Handles m_cmdEditBasemap.OnUpdate
        cmd.Enabled = Me.Core.StateMonitor.HasEcospaceLoaded
    End Sub

    ''' <summary>
    ''' Command handler; invokes the Ecospace edit habitats dialog.
    ''' </summary>
    Private Sub OnEditEcospaceHabitats(ByVal cmd As cCommand) Handles m_cmdEditHabitats.OnInvoke
        Dim dlg As New dlgEditHabitats(Me.UIContext)
        Me.Help.HelpTopic(dlg) = "Edit habitats.htm"
        dlg.ShowDialog(Me)
    End Sub

    ''' <summary>
    ''' Command handler; handles access to the Ecospace edit habitats dialog.
    ''' </summary>
    Private Sub OnUpdateEditEcospaceHabitats(ByVal cmd As cCommand) Handles m_cmdEditHabitats.OnUpdate
        cmd.Enabled = Me.Core.StateMonitor.HasEcospaceLoaded
    End Sub

    ''' <summary>
    ''' Command handler; invokes the Ecospace edit regions dialog.
    ''' </summary>
    Private Sub OnEditEcospaceRegions(ByVal cmd As cCommand) Handles m_cmdEditRegions.OnInvoke
        Dim dlg As New dlgEditRegions(Me.UIContext)
        dlg.ShowDialog(Me)
    End Sub

    ''' <summary>
    ''' Command handler; handles access to the Ecospace edit regions dialog.
    ''' </summary>
    Private Sub OnUpdateEditEcospaceRegions(ByVal cmd As cCommand) Handles m_cmdEditRegions.OnUpdate
        cmd.Enabled = Me.Core.StateMonitor.HasEcospaceLoaded
    End Sub

    ''' <summary>
    ''' Command handler; invokes the Ecospace edit MPAs dialog.
    ''' </summary>
    Private Sub OnEditEcospaceMPAs(ByVal cmd As cCommand) Handles m_cmdEditMPAs.OnInvoke
        Dim dlg As New dlgEditMPAs(Me.UIContext)
        dlg.ShowDialog(Me)
    End Sub

    ''' <summary>
    ''' Command handler; handles access to the Ecospace edit MPAs dialog.
    ''' </summary>
    Private Sub OnUpdateEditEcospaceMPAs(ByVal cmd As cCommand) Handles m_cmdEditMPAs.OnUpdate
        cmd.Enabled = Me.Core.StateMonitor.HasEcospaceLoaded
    End Sub

    ''' <summary>
    ''' Command handler; invokes the Ecospace edit importance layers dialog.
    ''' </summary>
    Private Sub OnEditEcospaceImportanceLayers(ByVal cmd As cCommand) Handles m_cmdEditImportanceLayers.OnInvoke
        Dim dlg As New dlgEditImportanceLayers(Me.UIContext)
        dlg.ShowDialog(Me)
    End Sub

    ''' <summary>
    ''' Command handler; handles access to the Ecospace edit importance layers dialog.
    ''' </summary>
    Private Sub OnUpdateEditEcospaceImportanceLayers(ByVal cmd As cCommand) Handles m_cmdEditImportanceLayers.OnUpdate
        cmd.Enabled = Me.Core.StateMonitor.HasEcospaceLoaded
    End Sub

    ''' <summary>
    ''' Command handler; invokes the import layer data dialog.
    ''' </summary>
    Private Sub OnImportLayerData(ByVal cmd As EwEUtils.Commands.cCommand) _
        Handles m_cmdImportLayerData.OnInvoke

        Dim dlg As New dlgImportLayerData(Me.UIContext)

        If cmd.Tag IsNot Nothing Then
            Try
                dlg.Layers = DirectCast(cmd.Tag, cLayer())
            Catch ex As Exception
                Debug.Assert(False, "Expected array of cLayer")
            End Try
        End If
        dlg.ShowDialog()

    End Sub

    ''' <summary>
    ''' Command update handler; enables and disables the 
    ''' <see cref="m_cmdImportLayerData">import layer data command</see>.
    ''' </summary>
    Private Sub m_cmdImportLayerData_OnUpdate(ByVal cmd As EwEUtils.Commands.cCommand) _
        Handles m_cmdImportLayerData.OnUpdate
        cmd.Enabled = Me.Core.StateMonitor.HasEcospaceLoaded()
    End Sub

    ''' <summary>
    ''' Command handler; invokes the export layers dialog.
    ''' </summary>
    Private Sub m_cmdExportLayerData_OnInvoke(ByVal cmd As EwEUtils.Commands.cCommand) _
        Handles m_cmdExportLayerData.OnInvoke

        Dim dlg As New dlgExportLayerData(Me.UIContext)
        If cmd.Tag IsNot Nothing Then
            Try
                dlg.Layers = DirectCast(cmd.Tag, cLayer())
            Catch ex As Exception
                Debug.Assert(False, "Expected array of cLayer")
            End Try
        End If
        dlg.ShowDialog()

    End Sub

    ''' <summary>
    ''' Command update handler; enables and disables the 
    ''' <see cref="m_cmdImportLayerData">export layer data command</see>.
    ''' </summary>
    Private Sub m_cmdExportLayerData_OnUpdate(ByVal cmd As EwEUtils.Commands.cCommand) _
        Handles m_cmdExportLayerData.OnUpdate
        cmd.Enabled = Me.Core.StateMonitor.HasEcospaceLoaded()
    End Sub

#End Region ' Ecospace commands

#Region " Ecotracer commands "

    ''' <summary>
    ''' Command handler; creates a new Ecotracer scenario
    ''' </summary>
    Private Sub OnNewEcotracerScenario(ByVal cmd As cCommand) _
        Handles m_cmdNewEcotracerScenario.OnInvoke

        ' Prerequesite: Ecosim needs to be loaded
        Me.CoreController.LoadState(eCoreExecutionState.EcosimLoaded)
        ' Not succesful? abort
        If Not Me.Core.StateMonitor.HasEcosimLoaded Then Return

        Dim dlg As New EcotracerScenarioDlg(Me.UIContext, EcotracerScenarioDlg.eDialogModeType.CreateScenario)

        If dlg.ShowDialog = Windows.Forms.DialogResult.OK Then

            Select Case dlg.Mode
                Case EcotracerScenarioDlg.eDialogModeType.CreateScenario
                    Me.CreateEcotracerScenario(dlg.ScenarioName, dlg.ScenarioDescription, dlg.ScenarioAuthor, dlg.ScenarioContact)
                Case EcotracerScenarioDlg.eDialogModeType.LoadScenario
                    Me.LoadEcotracerScenario(DirectCast(dlg.Scenario, cEcotracerScenario))
                Case Else
                    Debug.Assert(False)
            End Select

        End If

    End Sub

    ''' <summary>
    ''' Command update handler; takes care of enabling and disabling the
    ''' <see cref="m_cmdNewEcotracerScenario">New Ecotracer Scenario</see> command.
    ''' </summary>
    Private Sub OnUpdateNewEcotracerScenario(ByVal cmd As cCommand) _
        Handles m_cmdNewEcotracerScenario.OnUpdate
        cmd.Enabled = Me.Core.StateMonitor.HasEcopathLoaded
    End Sub

    ''' <summary>
    ''' Command handler; loads a new Ecotracer scenario
    ''' </summary>
    Private Sub OnLoadEcotracerScenario(ByVal cmd As cCommand) _
        Handles m_cmdLoadEcotracerScenario.OnInvoke
        Me.LoadEcotracerScenario()
    End Sub

    ''' <summary>
    ''' Command update handler; takes care of enabling and disabling the 
    ''' <see cref="m_cmdLoadEcotracerScenario">Load Ecotracer Scenario</see> command.
    ''' </summary>
    Private Sub OnUpdateLoadEcotracerScenario(ByVal cmd As cCommand) _
        Handles m_cmdLoadEcotracerScenario.OnUpdate
        cmd.Enabled = Me.Core.StateMonitor.HasEcopathLoaded
    End Sub

    'Private Sub OnSaveEcotracerScenario(ByVal cmd As cCommand) _
    '    Handles m_cmdSaveEcotracerScenario.OnInvoke
    '    Dim strStatus As String = String.Format(My.Resources.STATUS_ECOTRACER_SAVING, Me.Core.EcotracerScenarios(Me.Core.ActiveEcotracerScenarioIndex).Name)
    '    Me.SetStatusText(strStatus, TriState.True)
    '    Me.Core.SaveEcotracerScenario()
    '    Me.SetStatusText("", TriState.False)
    'End Sub

    '''' <summary>
    '''' Command update handler; enables and disables the 'save ecotracer scenario' command
    '''' </summary>
    'Private Sub OnUpdateSaveEcotracerScenario(ByVal cmd As cCommand) _
    '    Handles m_cmdSaveEcotracerScenario.OnUpdate
    '    cmd.Enabled = Me.Core.StateMonitor.HasEcosimLoaded And Me.Core.StateMonitor.IsEcotracerModified
    'End Sub

    Private Sub OnSaveEcotracerScenarioAs(ByVal cmd As cCommand) _
        Handles m_cmdSaveEcotracerScenarioAS.OnInvoke

        Dim dlg As New EcotracerScenarioDlg(Me.UIContext, _
                                            EcotracerScenarioDlg.eDialogModeType.SaveScenario, _
                                            Me.Core.EcotracerScenarios(Me.Core.ActiveEcotracerScenarioIndex))

        If dlg.ShowDialog() = Windows.Forms.DialogResult.OK Then
            ' Overwriting?
            If (dlg.Scenario IsNot Nothing) Then
                ' Prompt for overwrite confirmation
                If MessageBox.Show(String.Format(My.Resources.SCENARIO_CONFIRMOVERWRITE_PROMPT, dlg.ScenarioName), _
                        My.Resources.SCENARIO_CONFIRMOVERWRITE_CAPTION, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then

                    ' #Overwrite
                    Me.SetStatusText(String.Format(My.Resources.STATUS_ECOTRACER_SAVING, dlg.ScenarioName), TriState.True)
                    Me.Core.SaveEcotracerScenario(DirectCast(dlg.Scenario, cEcotracerScenario))
                    Me.SetStatusText("", TriState.False)

                End If
                ' User does not want to overwrite? Abort
                Return
            End If

            ' Add scenario under new name
            Me.SetStatusText(String.Format(My.Resources.STATUS_ECOTRACER_CREATING, dlg.ScenarioName), TriState.True)
            Me.Core.SaveEcotracerScenarioAs(dlg.ScenarioName, dlg.ScenarioDescription)
            Me.SetStatusText("", TriState.False)

        End If

    End Sub

    ''' <summary>
    ''' Command update handler; enables and disables the 'save ecotracer scenario as' command
    ''' </summary>
    Private Sub OnUpdateSaveEcotracerScenarioAs(ByVal cmd As cCommand) _
        Handles m_cmdSaveEcotracerScenarioAS.OnUpdate
        cmd.Enabled = Me.Core.StateMonitor.HasEcotracerLoaded()
    End Sub

    ''' <summary>
    ''' Command update handler; invokes the 'delete ecotracer scenario' command
    ''' </summary>
    Private Sub OnDeleteEcotracerScenario(ByVal cmd As cCommand) _
         Handles m_cmdDeleteEcotracerScenario.OnInvoke
        Dim dlg As New EcotracerScenarioDlg(Me.UIContext, EwEScenarioDlg.eDialogModeType.DeleteScenario)
        dlg.ShowDialog(Me)
    End Sub

    ''' <summary>
    ''' Command update handler; enables and disables the 'delete ecotracer scenario' command
    ''' </summary>
    Private Sub OnUpdateDeleteEcotracerScenario(ByVal cmd As cCommand) _
        Handles m_cmdDeleteEcotracerScenario.OnUpdate
        cmd.Enabled = Me.Core.StateMonitor.HasEcopathLoaded And Me.Core.EcotracerScenarioCount > 0
    End Sub

    Private Sub OnEnableEcotracer(ByVal cmd As cCommand) _
        Handles m_cmdEnableEcotracer.OnInvoke

        Dim pm As cPropertyManager = Me.m_uic.PropertyManager
        Dim ecosimModelParams As cEcoSimModelParameters = Nothing
        Dim propSimConTracing As cBooleanProperty = Nothing
        Dim ecospaceModelParams As cEcospaceModelParameters = Nothing
        Dim propSpaceConTracing As cBooleanProperty = Nothing
        Dim tracerRunMode As eTracerRunModeTypes = CType(cmd.Tag, eTracerRunModeTypes)

        ' Try to update the core run state to satisfy the requested tracer setting
        Select Case tracerRunMode
            Case eTracerRunModeTypes.Disabled ' Ecotracer off
                ' NOP

            Case eTracerRunModeTypes.RunSim ' Ecosim
                ' Load sim
                Me.CoreController.LoadState(eCoreExecutionState.EcosimLoaded)
                ' Not succesful? abort
                If Not Me.Core.StateMonitor.HasEcosimLoaded Then Return
                ' Get property to enable tracer for Sim
                ecosimModelParams = Me.Core.EcoSimModelParameters
                propSimConTracing = DirectCast(pm.GetProperty(ecosimModelParams, eVarNameFlags.ConSimOnEcoSim), cBooleanProperty)
                ' Try to load tracer
                Me.CoreController.LoadState(eCoreExecutionState.EcotracerLoaded)

            Case eTracerRunModeTypes.RunSpace ' Ecospace
                ' Load space
                Me.CoreController.LoadState(eCoreExecutionState.EcospaceLoaded)
                ' Not succesful? abort
                If Not Me.Core.StateMonitor.HasEcospaceLoaded Then Return
                ' Get property to enable tracer for Space
                ecospaceModelParams = Me.Core.EcospaceModelParameters
                propSpaceConTracing = DirectCast(pm.GetProperty(ecospaceModelParams, eVarNameFlags.ConSimOnEcoSpace), cBooleanProperty)
                ' Try to load tracer
                Me.CoreController.LoadState(eCoreExecutionState.EcotracerLoaded)

        End Select

        ' Tracer not loaded?
        If Not Me.Core.StateMonitor.HasEcotracerLoaded Then tracerRunMode = eTracerRunModeTypes.Disabled

        ' Configure properties
        If propSimConTracing IsNot Nothing Then
            propSimConTracing.SetValue(tracerRunMode = eTracerRunModeTypes.RunSim)
        End If

        If propSpaceConTracing IsNot Nothing Then
            propSpaceConTracing.SetValue(tracerRunMode = eTracerRunModeTypes.RunSpace)
        End If

    End Sub

    Private Sub OnUpdateEnableEcotracer(ByVal cmd As cCommand) _
        Handles m_cmdEnableEcotracer.OnUpdate
        cmd.Enabled = Me.Core.StateMonitor.HasEcopathLoaded
    End Sub

#End Region ' Ecotracer commands

#Region " Plug-in commands "

    Private Sub OnRunGUIPlugin(ByVal cmd As cCommand) Handles m_cmdPluginGUICommand.OnInvoke

        ' Sanity checks
        If Not (TypeOf cmd Is cPluginGUICommand) Then Return

        ' Phew
        Dim pgcmd As cPluginGUICommand = DirectCast(cmd, cPluginGUICommand)
        ' Check if core can be brought up to par
        If Me.CoreController.LoadState(pgcmd.CoreExecutionState) Then
            ' Invoke plugin. This code does not - and cannot - verify whether the plugin has already ran,
            ' and whether any plug-in UI elements are still active. The plug-in is responsible for dealing
            ' with consecutive run requests.

            Me.SetStatusText(My.Resources.GENERIC_STATUS_LOADINGPLUGIN, TriState.True)
            Try
                pgcmd.RunPlugin()
            Catch ex As Exception

            End Try
            Me.SetStatusText("", TriState.False)

            ' See if the plug-in attached any form to the command. This form will be nested in the interface
            ' if possible.
            If pgcmd.Form IsNot Nothing Then
                ' #Yes: form detected

                ' Protect this form from auto-closing if it is supposed to stay 'always open'
                If (pgcmd.CoreExecutionState = eCoreExecutionState.Idle) And _
                   (Me.m_lstrProtectedPanelNames.IndexOf(pgcmd.Form.Name) = -1) Then
                    Me.m_lstrProtectedPanelNames.Add(pgcmd.Form.Name)
                End If

                ' Able to activate this form from the open tabs?
                If Not ActivateForm(pgcmd.Form.Text) Then
                    ' #No: form is not currently integrated in the dock panel, it must be nested in the GUI.

                    ' Make sure it is not already shown; a visible form cannot be docked.
                    If pgcmd.Form.Visible Then
                        pgcmd.Form.Hide()
                    End If

                    ' Is this a dockable form? 
                    If (TypeOf pgcmd.Form Is DockContent) And (m_DockPanel.DocumentStyle = DocumentStyle.DockingMdi) Then
                        ' #Yes
                        ' Fix dockstyle
                        If pgcmd.DockState = 0 Then pgcmd.DockState = DockState.Document
                        ' Show the form in the dock panel
                        DirectCast(pgcmd.Form, DockContent).Show(Me.m_DockPanel, DirectCast(pgcmd.DockState, DockState))

                        ' Fix window state
                        If pgcmd.Form.WindowState = FormWindowState.Minimized Then
                            pgcmd.Form.WindowState = FormWindowState.Normal
                            pgcmd.Form.Show()
                        End If

                    Else
                        ' Show form
                        pgcmd.Form.MdiParent = Me
                        pgcmd.Form.Show()
                    End If
                    ' Switch help
                    ' ToDo_JS: consider allowing plug-in provided help documents
                    Me.Help.HelpTopic(pgcmd.Form) = ""
                End If
            End If
        End If
    End Sub

#End Region ' Plug-in commands

#End Region ' Command handlers 

#Region " Big and evil event handlers "

    Private Sub OnMRUItemClicked(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim mnuItem As ToolStripMenuItem = DirectCast(sender, ToolStripMenuItem)
        Dim strFileName As String = CStr(mnuItem.Tag)
        Me.LoadEcopathModel(strFileName, eLoadSourceType.MRU)
    End Sub

    Private Sub OnLoadEcosimScenarioOrDataset(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim mnuItem As ToolStripMenuItem = CType(sender, ToolStripMenuItem)

        If (mnuItem.Tag Is Nothing) Then Return

        If (TypeOf mnuItem.Tag Is cEcoSimScenario) Then
            ' Tag! You're it
            Me.m_cmdLoadEcosimScenario.Tag = mnuItem.Tag
            Me.m_cmdLoadEcosimScenario.Invoke()
            Me.m_cmdLoadEcosimScenario.Tag = Nothing
        ElseIf (TypeOf mnuItem.Tag Is cTimeSeriesDataset) Then
            Me.m_cmdLoadTimeSeries.Tag = DirectCast(mnuItem.Tag, cTimeSeriesDataset)
            Me.m_cmdLoadTimeSeries.Invoke()
            Me.m_cmdLoadTimeSeries.Tag = Nothing
        End If

    End Sub

    Private Sub OnLoadEcospaceScenario(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim mnuItem As ToolStripMenuItem = CType(sender, ToolStripMenuItem)
        Me.m_cmdLoadEcospaceScenario.Tag = mnuItem.Tag
        Me.m_cmdLoadEcospaceScenario.Invoke()
        Me.m_cmdLoadEcospaceScenario.Tag = Nothing
    End Sub

    Private Sub OnLoadEcotracerScenario(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim mnuItem As ToolStripMenuItem = CType(sender, ToolStripMenuItem)
        Me.m_cmdLoadEcotracerScenario.Tag = mnuItem.Tag
        Me.m_cmdLoadEcotracerScenario.Invoke()
        Me.m_cmdLoadEcotracerScenario.Tag = Nothing
    End Sub

    Private Sub OnDefaultSettingLoaded(ByVal sender As Object, ByVal e As System.Configuration.SettingsLoadedEventArgs)

        Me.m_strLastSelectedPath = My.Settings.LastSelectedDirectory
        If Not Directory.Exists(Me.m_strLastSelectedPath) Then
            'the last selected directory is not a valid directory; set it to My documents by default
            Me.m_strLastSelectedPath = My.Computer.FileSystem.SpecialDirectories.MyDocuments
        End If

        ' Read form positions
        Me.m_uic.FormPositionSettings.Setting = My.Settings.FormPositions

        ' Get the form position from user settings
        Me.StartPosition = FormStartPosition.Manual
        Me.m_uic.FormPositionSettings.Apply(Me, False)

    End Sub

    Private Sub OnTabFocusChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)

        Dim idc As IDockContent = m_DockPanel.ActiveDocument
        Dim dch As DockContentHandler = Nothing
        Dim strNewNodeName As String = String.Empty
        Dim stateNew As eCoreExecutionState = eCoreExecutionState.Idle

        ' UI is CONTROLLING the nav tree, do NOT respond to events
        If Me.m_bNavigating Then Return

        If Not Object.ReferenceEquals(idc, Nothing) Then
            dch = idc.DockHandler

            If Not Object.ReferenceEquals(dch, Nothing) Then
                ' Get default nav link
                strNewNodeName = dch.TabText
            End If

            If (TypeOf idc Is frmEwE) Then
                ' Get form specific nav link
                If TypeOf DirectCast(idc, frmEwE).Tag Is String Then
                    strNewNodeName = CStr(DirectCast(idc, frmEwE).Tag)
                End If
                stateNew = DirectCast(idc, frmEwE).CoreExecutionState
            End If
        End If

        ' About to change?
        If (String.Compare(Me.m_strLastActiveContent, strNewNodeName) <> 0) Then
            ' Update core state if possible
            Me.CoreController.LoadState(stateNew)
            ' Update help
            Me.Help.ActiveHelpControl = CType(m_DockPanel.ActiveDocument, Control)
            ' Switch
            Me.UpdateSelectedNode(strNewNodeName)
        End If
    End Sub

    Private Sub OnModelPathAreaClicked(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_tsModel.OnPathAreaClicked
        Me.m_cmdLoadModel.Tag = Me.m_tsModel.Path
        Me.m_cmdLoadModel.Invoke()
        Me.m_cmdLoadModel.Tag = Nothing
    End Sub

    Private Sub OnCoreExecutionStateChanged(ByVal csm As cCoreStateMonitor)
        Me.UpdateModelControls()
        Me.PopulateMRUDropdown()
        Me.PopulateScenarioDropdowns()
    End Sub

    Private Sub OnCoreMessage(ByRef msg As cMessage)
        If msg.Type = eMessageType.DataAddedOrRemoved Then
            If (msg.DataType = eDataTypes.EcoSimScenario) Or _
               (msg.DataType = eDataTypes.EcoSpaceScenario) Or _
               (msg.DataType = eDataTypes.EcotracerScenario) Or _
               (msg.DataType = eDataTypes.TimeSeriesDataset) Then
                Me.PopulateScenarioDropdowns()
            End If
        End If
    End Sub

#End Region ' Big and evil event handlers

End Class