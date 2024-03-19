' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 2 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see <http://www.gnu.org/licenses/gpl-2.0.html>. 
'
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Explicit On
Option Strict On

Imports System.ComponentModel
Imports System.IO
Imports System.Threading
Imports EwECore
Imports EwECore.Database
Imports EwECore.DataSources
Imports EwECore.SpatialData
Imports EwEPlugin
Imports EwEUtils.Core
Imports EwEUtils.Database
Imports EwEUtils.SystemUtilities
Imports EwEUtils.Utilities
Imports ScientificInterface.Ecopath
Imports ScientificInterface.Ecosim
Imports ScientificInterface.Ecospace
Imports ScientificInterface.Ecospace.Basemap
Imports ScientificInterface.Ecospace.Basemap.Layers
Imports ScientificInterface.Ecospace.Controls
Imports ScientificInterface.Ecotracer
Imports ScientificInterface.Other
Imports ScientificInterface.Wizard
Imports ScientificInterfaceShared
Imports ScientificInterfaceShared.Integration
Imports WeifenLuo.WinFormsUI.Docking
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' The main form of the EwE6 Scientific Interface
''' </summary>
''' ---------------------------------------------------------------------------
Public Class frmEwE6
    Implements IUIElement

#Region " Variables "

    ' - Message handlers 
    Private m_mhProgress As cMessageHandler = Nothing
    Private m_mhEcosim As cMessageHandler = Nothing
    Private m_mhEcospace As cMessageHandler = Nothing
    Private m_mhEcotracer As cMessageHandler = Nothing
    Private m_mhTimeseries As cMessageHandler = Nothing

    ' - Big nasty UI objects
    Private m_coreController As cCoreController = Nothing
    Private m_pluginManager As cPluginManager = Nothing
    Private m_pluginMenuHandler As cPluginMenuHandler = Nothing
    Private m_formstatemanager As cEwEFormStateManager = Nothing
    Private m_styleguideupdater As cStyleGuideUpdater = Nothing
    Private m_autosavemanager As cAutosaveSettingsManager = Nothing

    ''' <summary>Foundation for undo stack?</summary>
    Private m_messageHistory As cMessageHistory = Nothing

    ''' <summary>Status messages stack.</summary>
    Private m_statusmessages As New List(Of String)

    ''' <summary>Flag indicating that the EwE is fully initialized</summary>
    Private m_bIsInitialized As Boolean = False

#Region " Panels "

    Private Const cPANEL_REMARKS As String = "remarks"
    Private Const cPANEL_STATUS As String = "Status"
    Private Const cPANEL_NAV As String = "navigation"

    Private m_DockPanel As DockPanel = Nothing
    Private m_dtPanels As New Dictionary(Of String, frmEwEDockContent)

#End Region ' Panels

#Region " Presentation mode "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Helper class to toggle the EwE main form between a normal state
    ''' and a 'presentation mode' state.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Class cPresentationMode

#Region " Private vars "

        Private m_frm As frmEwE6 = Nothing
        Private m_bInPresentationMode As Boolean = False

        ' -- cached main form states  --
        Private m_bShowMenu As Boolean
        Private m_bShowModelBar As Boolean
        Private m_bShowStatusBar As Boolean
        Private m_bShowNavPanel As Boolean
        Private m_bFormState As FormWindowState
        Private m_bBorderStyle As FormBorderStyle
        Private m_bControlBox As Boolean
        Private m_bBounds As Rectangle
        Private m_bUseOpacity As Boolean = False

#End Region ' Private vars

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor.
        ''' </summary>
        ''' <param name="frm">The <see cref="frmEwE6"/> to toggle presentation mode for.</param>
        ''' <param name="bUseOpacity">If set to true, the main form will be totally 
        ''' opaque during a presentation mode switch.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(frm As frmEwE6, Optional bUseOpacity As Boolean = False)
            Me.m_frm = frm
            Me.m_bUseOpacity = bUseOpacity
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Toggle between presentation mode and regular mode.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Sub TogglePresentationMode()
            Me.IsPresentationModeActive = Not Me.IsPresentationModeActive
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set whether presentation mode is active.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property IsPresentationModeActive As Boolean
            Get
                Return Me.m_bInPresentationMode
            End Get
            Set(value As Boolean)
                If (value = Me.m_bInPresentationMode) Then Return
                Me.m_bInPresentationMode = value

                If (Me.m_bUseOpacity) Then Me.m_frm.Opacity = 0

                ' Presentation mode active?
                If (Me.m_bInPresentationMode) Then
                    ' #Yes: hide bits and stretch form
                    Me.m_bShowMenu = Me.m_frm.m_menuMain.Visible : Me.m_frm.m_menuMain.Visible = Not My.Settings.PresentationModeHideMainMenu
                    Me.m_bShowModelBar = Me.m_frm.m_tsModel.Visible : Me.m_frm.m_tsModel.Visible = Not My.Settings.PresentationModeHideModelBar
                    Me.m_bShowStatusBar = Me.m_frm.m_ssMain.Visible : Me.m_frm.m_ssMain.Visible = Not My.Settings.PresentationModeHideStatusBar
                    Me.m_bShowNavPanel = Me.m_frm.Panel(cPANEL_NAV).IsHiding : Me.m_frm.Panel(cPANEL_NAV).AutoHide = My.Settings.PresentationModeCollapseNavPanel

                    ' JS 28Mar14: This now works
                    ' - Using screen bounds works better than maximizing frmEwE6
                    ' - TopMost is not needed anymore
                    ' - Do not change the order of the next three statements!
                    Me.m_bFormState = Me.m_frm.WindowState : Me.m_frm.WindowState = FormWindowState.Normal
                    Me.m_bBorderStyle = Me.m_frm.FormBorderStyle : Me.m_frm.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
                    Me.m_bBounds = Me.m_frm.Bounds : Me.m_frm.Bounds = Screen.GetBounds(Me.m_frm)
                    Me.m_bControlBox = Me.m_frm.ControlBox : Me.m_frm.ControlBox = False
                    '.TopMost = Me.TopMost : Me.TopMost = True
                Else
                    Me.m_frm.FormBorderStyle = Me.m_bBorderStyle
                    Me.m_frm.WindowState = Me.m_bFormState
                    'Me.TopMost = .TopMost
                    Me.m_frm.Bounds = Me.m_bBounds
                    Me.m_frm.m_menuMain.Visible = Me.m_bShowMenu
                    Me.m_frm.m_tsModel.Visible = Me.m_bShowModelBar
                    Me.m_frm.m_ssMain.Visible = Me.m_bShowStatusBar
                    Me.m_frm.Panel(cPANEL_NAV).AutoHide = Me.m_bShowNavPanel
                    Me.m_frm.ControlBox = Me.m_bControlBox
                End If

                If (Me.m_bUseOpacity) Then Me.m_frm.Opacity = 1

            End Set
        End Property
    End Class

    Private m_presentationmode As cPresentationMode = Nothing

#End Region ' Presentation mode

#Region " Commands "

    Private WithEvents m_cmdFileOpen As cFileOpenCommand = Nothing
    Private WithEvents m_cmdFileSave As cFileSaveCommand = Nothing
    Private WithEvents m_cmdDirectoryOpen As cDirectoryOpenCommand = Nothing
    Private WithEvents m_cmdExecute As cExecuteCommand = Nothing
    Private WithEvents m_cmdNewModel As cCommand = Nothing
    Private WithEvents m_cmdLoadModel As cCommand = Nothing
    Private WithEvents m_cmdOpenOutput As cCommand = Nothing
    Private WithEvents m_cmdSave As cCommand = Nothing
    Private WithEvents m_cmdSaveModelAs As cCommand = Nothing
    Private WithEvents m_cmdCloseModel As cCommand = Nothing
    Private WithEvents m_cmdCompactModel As cCommand = Nothing
    Private WithEvents m_cmdCloseDocument As cCommand = Nothing
    Private WithEvents m_cmdNewEcosimScenario As cCommand = Nothing
    Private WithEvents m_cmdLoadEcosimScenario As cCommand = Nothing
    'Private WithEvents m_cmdSaveEcosimScenario As cCommand = Nothing
    Private WithEvents m_cmdCloseEcosimScenario As cCommand = Nothing
    Private WithEvents m_cmdSaveEcosimScenarioAs As cCommand = Nothing
    Private WithEvents m_cmdDeleteEcosimScenario As cCommand = Nothing
    Private WithEvents m_cmdNewEcospaceScenario As cCommand = Nothing
    Private WithEvents m_cmdLoadEcospaceScenario As cCommand = Nothing
    'Private WithEvents m_cmdSaveEcospaceScenario As cCommand = Nothing
    Private WithEvents m_cmdCloseEcospaceScenario As cCommand = Nothing
    Private WithEvents m_cmdSaveEcospaceScenarioAS As cCommand = Nothing
    Private WithEvents m_cmdDeleteEcospaceScenario As cCommand = Nothing
    Private WithEvents m_cmdNewEcotracerScenario As cCommand = Nothing
    Private WithEvents m_cmdLoadEcotracerScenario As cCommand = Nothing
    'Private WithEvents m_cmdSaveEcotracerScenario As cCommand = Nothing
    Private WithEvents m_cmdCloseEcotracerScenario As cCommand = Nothing
    Private WithEvents m_cmdSaveEcotracerScenarioAS As cCommand = Nothing
    Private WithEvents m_cmdDeleteEcotracerScenario As cCommand = Nothing
    Private WithEvents m_cmdCloseAllForms As cCommand = Nothing
    Private WithEvents m_cmdNavigate As cNavigationCommand = Nothing
    Private WithEvents m_cmdViewNavPane As cCommand = Nothing
    Private WithEvents m_cmdViewStatusPane As cCommand = Nothing
    Private WithEvents m_cmdBrowseURI As cBrowserCommand = Nothing
    Private WithEvents m_cmdViewRemarkPane As cCommand = Nothing
    Private WithEvents m_cmdViewMenu As cCommand = Nothing
    Private WithEvents m_cmdViewModelBar As cCommand = Nothing
    Private WithEvents m_cmdViewStatusbar As cCommand = Nothing
    Private WithEvents m_cmdViewPresentationMode As cCommand = Nothing
    Private WithEvents m_cmdAutosaveConfig As cCommand = Nothing
    Private WithEvents m_cmdAutorunConfig As cCommand = Nothing
    Private WithEvents m_cmdEditGroups As cCommand = Nothing
    Private WithEvents m_cmdEditMultiStanza As cCommand = Nothing
    Private WithEvents m_cmdEditFleets As cCommand = Nothing
    Private WithEvents m_cmdEditTaxonomy As cCommand = Nothing
    Private WithEvents m_cmdEditPedigree As cEditPedigreeCommand = Nothing
    Private WithEvents m_cmdImportTimeSeries As cCommand = Nothing
    Private WithEvents m_cmdEcosimLoadTimeSeries As cCommand = Nothing
    Private WithEvents m_cmdEcospaceLoadTimeSeries As cCommand = Nothing
    Private WithEvents m_cmdWeightTimeSeries As cCommand = Nothing
    Private WithEvents m_cmdExportTimeSeries As cCommand = Nothing
    Private WithEvents m_cmdEditBasemap As cCommand = Nothing
    Private WithEvents m_cmdEditHabitats As cCommand = Nothing
    Private WithEvents m_cmdEditRegions As cCommand = Nothing
    Private WithEvents m_cmdEditEffortZones As cCommand = Nothing
    Private WithEvents m_cmdEditMPAs As cCommand = Nothing
    Private WithEvents m_cmdDefineImportanceMaps As cCommand = Nothing
    Private WithEvents m_cmdDefineInputLayers As cCommand = Nothing
    Private WithEvents m_cmdImportLayerData As cImportLayerCommand = Nothing
    Private WithEvents m_cmdExportLayerData As cExportLayerCommand = Nothing
    Private WithEvents m_cmdEditLayer As cEditLayerCommand = Nothing
    Private WithEvents m_cmdShowOptions As cShowOptionsCommand = Nothing
    Private WithEvents m_cmdShowTools As cCommand = Nothing
    Private WithEvents m_cmdEditReferenceMap As cCommand = Nothing
    Private WithEvents m_cmdPluginGUICommand As cPluginGUICommand = Nothing
    Private WithEvents m_cmdHelpAbout As cCommand = Nothing
    Private WithEvents m_cmdHelpReportIssue As cCommand = Nothing
    Private WithEvents m_cmdHelpRequestCodeAccess As cCommand
    Private WithEvents m_cmdHelpFeedback As cCommand = Nothing
    Private WithEvents m_cmdPropertySelection As cPropertySelectionCommand = Nothing
    Private WithEvents m_cmdShowHideItems As cShowHideItemsCommand = Nothing
    Private WithEvents m_cmdEnableEcotracer As cCommand = Nothing
    Private WithEvents m_cmdEstimateVs As cCommand = Nothing
    Private WithEvents m_cmdExportEcosimResultsToCSV As cEcosimSaveDataCommand = Nothing
    Private WithEvents m_cmdPrint As cCommand = Nothing
    Private WithEvents m_cmdEcosimTrimShapes As cCommand = Nothing
    Private WithEvents m_cmdEcosimChangeShape As cCommand = Nothing
    Private WithEvents m_cmdPickColor As cPickColorCommand = Nothing

    Private WithEvents m_cmdEcospaceLoadXYRefData As cCommand = Nothing

    ' --- Ecospace external data ---

    ''' <summary>Command to define external spatial temporal data connections.</summary>
    Private WithEvents m_cmdDefineSpatialDatasets As cCommand = Nothing
    ''' <summary>Command to define export spatial temporal data connections.</summary>
    Private WithEvents m_cmdEcospaceExportSpatialDatasets As cCommand = Nothing
    ''' <summary>Command to edit an external data set.</summary>
    Private WithEvents m_cmdEditSpatialDataset As cEditSpatialDatasetCommand = Nothing
    ''' <summary>Command to configure the external data connection(s) to a single layer.</summary>
    Private WithEvents m_cmdEcospaceConfigureConnection As cEcospaceConfigureConnectionCommand = Nothing
    ''' <summary>Command to manage external data configurations.</summary>
    Private WithEvents m_cmdEcospaceManageConfigs As cCommand = Nothing

    ' --- Ecobase --

    Private WithEvents m_cmdEcobaseImport As cCommand = Nothing
    Private WithEvents m_cmdEcobaseExport As cCommand = Nothing

    ' --- EIIXML --

    Private WithEvents m_cmdEIIXMLExport As cCommand = Nothing

    ' --- Pro --

    Private WithEvents m_cmdClearLicense As cClearLicenseCommand = Nothing
    Private WithEvents m_cmdEnterLicense As cEnterLicenseCommand = Nothing

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

    Private Shared __inst__ As frmEwE6 = Nothing

#End Region ' Singleton

#Region " Constructors "

    Public Sub New()

#If 0 Then
        ' Uncomment to torture EwE and see if all decimal comma / point issues have been solved
        Dim culture As CultureInfo = DirectCast(CultureInfo.CurrentCulture.Clone(), CultureInfo)
        culture.NumberFormat.NumberDecimalSeparator = ","
        Thread.CurrentThread.CurrentCulture = culture
        Thread.CurrentThread.CurrentUICulture = culture
#End If

        Me.InitializeComponent()

        Debug.Assert(frmEwE6.__inst__ Is Nothing, "Only one instance of frmEwE6 allowed")
        frmEwE6.__inst__ = Me
        cLog.VerboseLevel = DirectCast(My.Settings.LogVerboseLevel, eVerboseLevel)

        Me.SetStyle(ControlStyles.OptimizedDoubleBuffer, True)
        Me.m_presentationmode = New cPresentationMode(Me)

        ' Prepare caption
        Me.Text = My.Resources.GENERIC_CAPTION
        ' Prepare Icon
        Me.Icon = cEwEIcon.Current()

        ' Write diagnostics info
        cLog.Write("RenderWithVisualStyles", CStr(Application.RenderWithVisualStyles))

    End Sub

#End Region ' Constructors

#Region " IUIElement implementation "

    Public Property UIContext() As cUIContext _
        Implements IUIElement.UIContext

    Public ReadOnly Property Core() As cCore
        Get
            Return Me.UIContext.Core
        End Get
    End Property

    Public ReadOnly Property CoreController() As cCoreController
        Get
            Return Me.m_coreController
        End Get
    End Property

    Public ReadOnly Property StyleGuide() As cStyleGuide
        Get
            Return Me.UIContext.StyleGuide
        End Get
    End Property

    Public ReadOnly Property Help() As cHelp
        Get
            Return Me.UIContext.Help
        End Get
    End Property

    Public ReadOnly Property PropertyManager() As cPropertyManager
        Get
            Return Me.UIContext.PropertyManager
        End Get
    End Property

    Public ReadOnly Property CommandHandler() As cCommandHandler
        Get
            Return Me.UIContext.CommandHandler
        End Get
    End Property

    Public ReadOnly Property SyncObject() As SynchronizationContext
        Get
            Return Me.UIContext.SyncObject
        End Get
    End Property

#End Region ' IUIElement implementation

#Region " Initialization "

    Private Sub ProcessCommandLine()

        ' ToDo: parse command line parameters for flags
        ' Add flags:
        '   /NoPlugins
        '   /NoSplash

        Dim astrCmd As String() = Environment.GetCommandLineArgs

        ' Has args?
        If (astrCmd.Length > 1) Then
            ' #Yes: get database parameter
            Dim strDB As String = astrCmd(1).Replace("""", "")
            ' #Yes: is compatible?
            If (cDataSourceFactory.GetSupportedType(strDB) <> eDataSourceTypes.NotSet) Then
                ' #Yes: try to open the model
                Me.LoadEcopathModel(strDB, eLoadSourceType.CommandLine)
            End If
        End If

    End Sub

    Private Sub InitCommands()

        Dim cmdh As cCommandHandler = Me.UIContext.CommandHandler

        ' Create and configure File Open command
        Me.m_cmdFileOpen = New cFileOpenCommand(cmdh)

        ' Create and configure File Save command
        Me.m_cmdFileSave = New cFileSaveCommand(cmdh)

        ' Create and configure Directory Open command
        Me.m_cmdDirectoryOpen = New cDirectoryOpenCommand(cmdh)
        Me.m_cmdDirectoryOpen.Directory = Me.Core.OutputPath

        ' Create and configure Execute command
        Me.m_cmdExecute = New cExecuteCommand(cmdh)

        ' Create and configure new command
        Me.m_cmdNewModel = New cCommand(cmdh, "NewEcopathModel")
        Me.m_cmdNewModel.AddControl(Me.m_tsmiFileNew)

        ' Create and configure open command
        Me.m_cmdLoadModel = New cCommand(cmdh, "LoadEcopathModel")
        Me.m_cmdLoadModel.AddControl(Me.m_tsmiFileOpen)
        Me.m_cmdLoadModel.AddControl(Me.m_tsbEcopath)

        ' Create and configure open output location command
        Me.m_cmdOpenOutput = New cCommand(cmdh, "OpenOutputLocation")
        Me.m_cmdOpenOutput.AddControl(Me.m_tsmiOpenOutput)

        Me.m_cmdSave = New cCommand(cmdh, "SaveModel", My.Resources.COMMAND_SAVECHANGES)
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

        ' Create and configure print command
        Me.m_cmdPrint = New cPrintCommand(cmdh)
        Me.m_cmdPrint.AddControl(Me.m_tsmiPrint)

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

        'Create and configure 'close ecosim scenario' command
        Me.m_cmdCloseEcosimScenario = New cCommand(cmdh, "CloseEcosimScenario")
        Me.m_cmdCloseEcosimScenario.AddControl(Me.m_tsmiEcosimClose)

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

        'Create and configure 'close ecospace scenario' command
        Me.m_cmdCloseEcospaceScenario = New cCommand(cmdh, "CloseEcospaceScenario")
        Me.m_cmdCloseEcospaceScenario.AddControl(Me.m_tsmiEcospaceClose)

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

        'Create and configure 'close ecotracer scenario' command
        Me.m_cmdCloseEcotracerScenario = New cCommand(cmdh, "CloseEcotracerScenario")

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
        Me.m_cmdBrowseURI = New cBrowserCommand(cmdh)
        Me.m_cmdBrowseURI.AddControl(Me.m_tsmiViewOnline)

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

        'Create and configure 'show options' command
        Me.m_cmdShowOptions = New cShowOptionsCommand(cmdh)
        Me.m_cmdShowOptions.AddControl(Me.m_tsmiOptions)

        'Create and configure 'show tools' command
        Me.m_cmdShowTools = New cCommand(cmdh, "ShowTools")
        Me.m_cmdShowTools.AddControl(Me.m_tsmiExternalTools)

        'Create and configure 'edit reference map' command
        Me.m_cmdEditReferenceMap = New cCommand(cmdh, "EditRefMap")

        'Create and configure 'Autosave config' command
        Me.m_cmdAutosaveConfig = New cCommand(cmdh, "AutosaveConfig", My.Resources.COMMAND_AUTOSAVE)
        Me.m_cmdAutosaveConfig.AddControl(Me.m_tsbnAutosaveConfig)

        'Create and configure 'Autorun config' command
        Me.m_cmdAutorunConfig = New cCommand(cmdh, "AutorunConfig", My.Resources.COMMAND_AUTORUN)
        Me.m_cmdAutorunConfig.AddControl(Me.m_tsbnAutorunConfig)

        'Create and configure EditGroups command
        Me.m_cmdEditGroups = New cCommand(cmdh, "EditGroups")
        Me.m_cmdEditGroups.AddControl(Me.m_tsmiEcopathDefineGroups)

        'Create and configure EditMultiStanza command
        Me.m_cmdEditMultiStanza = New cCommand(cmdh, "EditMultiStanza")
        Me.m_cmdEditMultiStanza.AddControl(Me.m_tsmiEcopathDefineMultiStanza)

        'Create and configure EditFleets command
        Me.m_cmdEditFleets = New cCommand(cmdh, "EditFleets")
        Me.m_cmdEditFleets.AddControl(Me.m_tsmiEcopathDefineFleets)

        Me.m_cmdEditPedigree = New cEditPedigreeCommand(cmdh)
        Me.m_cmdEditPedigree.AddControl(Me.m_tsmiEcopathDefinePedigree)

        Me.m_cmdEditTaxonomy = New cCommand(cmdh, "EditTaxonomy")
        Me.m_cmdEditTaxonomy.AddControl(Me.m_tsmiEcopathDefineTraits)

        Me.m_cmdEditBasemap = New cCommand(cmdh, "EditBasemap")
        Me.m_cmdEditBasemap.AddControl(Me.m_tsmiEcospaceEditMap)

        Me.m_cmdEditHabitats = New cEditHabitatsCommand(cmdh)
        Me.m_cmdEditHabitats.AddControl(Me.m_tsmiEcospaceDefineHabitats)

        Me.m_cmdEditMPAs = New cEditMPAsCommand(cmdh)
        Me.m_cmdEditMPAs.AddControl(Me.m_tsmiEcospaceDefineMPAs)

        Me.m_cmdEditRegions = New cEditRegionsCommand(cmdh)
        Me.m_cmdEditRegions.AddControl(Me.m_tsmiEcospaceDefineRegions)

        Me.m_cmdEditEffortZones = New cEditEffortZonesCommand(cmdh)

        Me.m_cmdDefineImportanceMaps = New cEditImportanceLayersCommand(cmdh)
        Me.m_cmdDefineImportanceMaps.AddControl(Me.m_tsmiEcospaceDefineImportanceLayers)

        Me.m_cmdDefineInputLayers = New cEditDriverLayersCommand(cmdh)
        Me.m_cmdDefineInputLayers.AddControl(Me.m_tsmiEcospaceInputMaps)

        Me.m_cmdDefineSpatialDatasets = New cCommand(cmdh, "EditSpatialDatasets")
        Me.m_cmdDefineSpatialDatasets.AddControl(Me.m_tsmiEcospaceDatasets)

        Me.m_cmdEditSpatialDataset = New cEditSpatialDatasetCommand(cmdh)

        Me.m_cmdEcospaceExportSpatialDatasets = New cCommand(cmdh, "ExportSpatialDatasets")
        Me.m_cmdEcospaceConfigureConnection = New cEcospaceConfigureConnectionCommand(cmdh)

        Me.m_cmdEcospaceManageConfigs = New cCommand(cmdh, "ManageSpatialDatasetConfigurations")

        Me.m_cmdImportLayerData = New cImportLayerCommand(cmdh)
        Me.m_cmdImportLayerData.AddControl(Me.m_tsmiEcospaceImportLayers)

        Me.m_cmdExportLayerData = New cExportLayerCommand(cmdh)
        Me.m_cmdExportLayerData.AddControl(Me.m_tsmiEcospaceExportLayers)

        Me.m_cmdEditLayer = New cEditLayerCommand(cmdh)

        Me.m_cmdEcosimTrimShapes = New cCommand(cmdh, "TrimUnusedShapeData")
        Me.m_cmdEcosimChangeShape = New cCommand(cmdh, "ChangeEcosimShape")

        Me.m_cmdImportTimeSeries = New cCommand(cmdh, "ImportTimeSeries")
        Me.m_cmdImportTimeSeries.AddControl(Me.m_tsmiTimeSeriesImport)

        Me.m_cmdEcosimLoadTimeSeries = New cCommand(cmdh, "LoadTimeSeries")
        Me.m_cmdEcosimLoadTimeSeries.AddControl(Me.m_tsmiTimeSeriesLoad)

        Me.m_cmdEcospaceLoadTimeSeries = New cCommand(cmdh, "LoadSpatialTemporalDataset")
        'Me.m_cmdEcospaceLoadTimeSeries.AddControl(Me.m_tsmiTimeSeriesLoad)

        Me.m_cmdWeightTimeSeries = New cCommand(cmdh, "WeightTimeSeries")
        Me.m_cmdWeightTimeSeries.AddControl(Me.m_tsmiTimeSeriesEditWeights)

        Me.m_cmdExportTimeSeries = New cCommand(cmdh, "ExportTimeSeries")
        Me.m_cmdExportTimeSeries.AddControl(Me.m_tsmiTimeSeriesExport)

        Me.m_cmdHelpAbout = New cCommand(cmdh, "HelpAbout")
        Me.m_cmdHelpAbout.AddControl(Me.m_tsmiHelpAbout)

        Me.m_cmdHelpReportIssue = New cCommand(cmdh, "ReportIssue")
        Me.m_cmdHelpReportIssue.AddControl(Me.m_tsmiHelpReportIssue)

        Me.m_cmdHelpRequestCodeAccess = New cCommand(cmdh, "RequesCodeAccess")
        Me.m_cmdHelpRequestCodeAccess.AddControl(Me.m_tsmiHelpRequestSourceCodeAccess)

        Me.m_cmdHelpFeedback = New cCommand(cmdh, "HelpFeedback")

        Me.m_cmdPickColor = New cPickColorCommand(cmdh)

#If BETA = 1 Then
        Me.m_cmdHelpReportIssue.AddControl(Me.m_tsbnPreview)
        Me.m_tsbnPreview.Visible = True

        Me.m_cmdHelpFeedback.AddControl(Me.m_tsbnFeedback)
        Me.m_cmdHelpFeedback.AddControl(Me.m_tsmiHelpFeedback)
        Me.m_tsbnFeedback.Visible = True
        Me.m_tsmiHelpFeedback.Visible = True
#Else
        Me.m_tsbnPreview.Visible = False
        Me.m_tsbnFeedback.Visible = False
        Me.m_tsmiHelpFeedback.Visible = False
#End If

        Me.m_cmdPluginGUICommand = New cPluginGUICommand(cmdh)

        Me.m_cmdPropertySelection = New cPropertySelectionCommand(cmdh)

        Me.m_cmdShowHideItems = New cShowHideItemsCommand(cmdh)
        Me.m_cmdShowHideItems.AddControl(Me.m_tsmiViewItems)
        Me.m_cmdShowHideItems.AddControl(Me.m_tsddViewItems)

        Me.m_cmdEnableEcotracer = New cCommand(cmdh, "EnableEcotracer")

        Me.m_cmdEstimateVs = New cCommand(cmdh, "EstimateVs")

        Me.m_cmdExportEcosimResultsToCSV = New cEcosimSaveDataCommand(cmdh)

        Me.m_cmdEcospaceLoadXYRefData = New cCommand(cmdh, "EcospaceLoadXYRefData")
        Me.m_cmdEcospaceLoadXYRefData.AddControl(Me.m_tsmiEcospaceLoadXYRefData)

        ' --- Ecobase ---

        Me.m_cmdEcobaseImport = New cCommand(cmdh, "EcobaseImport")
        Me.m_cmdEcobaseImport.AddControl(Me.m_tsmiEcobaseImport)
        'Me.m_tsmiEcobaseImport.Image = My.Resources.EcoBase

        Me.m_cmdEcobaseExport = New cCommand(cmdh, "EcobaseExport")
        Me.m_cmdEcobaseExport.AddControl(Me.m_tsmiEcobaseExport)

        ' --- EIIXML ---

        Me.m_cmdEIIXMLExport = New cCommand(cmdh, "EIIXMLExport")
        Me.m_cmdEIIXMLExport.AddControl(Me.m_tsmiEIIXMLExport)

        ' --- Pro ---

        Me.m_cmdClearLicense = New cClearLicenseCommand(cmdh)

        Me.m_cmdEnterLicense = New cEnterLicenseCommand(cmdh)
        Me.m_cmdEnterLicense.AddControl(Me.m_tsmiHelpRegister)

        ' ---

        Me.m_tslbReadOnly.Image = SharedResources.ProtectFormHS
        Me.m_tslbReadOnly.Enabled = False

        ' ToDo: make button image responsive to actual section: open, gray, closed
        Me.m_tsddViewItems.Image = SharedResources.Eye_open
        Me.m_tsbnAutosaveConfig.Image = SharedResources.AutoSaveHS
        Me.m_tsbnAutorunConfig.Image = SharedResources.AutoPlayHS

        ' Listen to application Idle events to update command states
        AddHandler Application.Idle, AddressOf cmdh.OnIdle
        AddHandler Application.Idle, AddressOf Me.m_pluginMenuHandler.OnIdle

    End Sub

    Private Sub InitPanels()

        ' Initialize panels
        Try
            Me.m_dtPanels(cPANEL_NAV) = New frmNavigationPanel(Me.UIContext, Me.m_pluginManager)
            Me.m_dtPanels(cPANEL_STATUS) = New frmStatusPanel(Me.UIContext, Me.m_messageHistory)
            Me.m_dtPanels(cPANEL_REMARKS) = New frmRemarkPanel(Me.UIContext)
        Catch ex As Exception

        End Try

    End Sub

    Private Function Panel(strPanelName As String) As frmEwEDockContent
        If Me.m_dtPanels.ContainsKey(strPanelName) Then
            Return Me.m_dtPanels(strPanelName)
        End If
        Return Nothing
    End Function

    Private Sub InitDockPanelPositions()

        If cSystemUtils.IsRightToLeft Then
            Me.Panel(cPANEL_NAV).Show(Me.m_DockPanel, DockState.DockRight)
        Else
            Me.Panel(cPANEL_NAV).Show(Me.m_DockPanel, DockState.DockLeft)
        End If
        Me.Panel(cPANEL_STATUS).Show(Me.m_DockPanel, DockState.DockBottomAutoHide)
        Me.Panel(cPANEL_REMARKS).Show(Me.m_DockPanel, DockState.DockBottomAutoHide)

    End Sub

    Private Sub InitCoreParams()

        Dim so As SynchronizationContext = SynchronizationContext.Current

        If so Is Nothing Then
            'create the sync object on the same thread that created the frmEwE6
            so = New SynchronizationContext()
        End If

        Dim core As New cCore()
        Dim sg As New cStyleGuide(core, EwE6ApplicationFramework.ReleaseMode)
        Dim cmdh As New cCommandHandler()
        Dim pm As New cPropertyManager(core, so)
        Dim fps As New cFormSettings()
        Dim help As New cHelp(Me, "UserGuide\EwE6_userguide.chm", "User Interface.htm", "EWE_UsersGuide")

        Me.UIContext = New cUIContext(core, sg, pm, cmdh, Me, fps, help, so)

        ' Configure state monitor
        Me.Core.StateMonitor.SyncObject = Me
        Me.m_mhProgress = New cMessageHandler(AddressOf Me.OnProgressMessage, eCoreComponentType.External, eMessageType.Progress, Me.SyncObject)
        Me.m_mhEcosim = New cMessageHandler(AddressOf Me.OnCoreMessage, eCoreComponentType.Ecosim, eMessageType.DataAddedOrRemoved, Me.SyncObject)
        Me.m_mhEcospace = New cMessageHandler(AddressOf Me.OnCoreMessage, eCoreComponentType.Ecospace, eMessageType.DataAddedOrRemoved, Me.SyncObject)
        Me.m_mhEcotracer = New cMessageHandler(AddressOf Me.OnCoreMessage, eCoreComponentType.Ecotracer, eMessageType.DataAddedOrRemoved, Me.SyncObject)
        Me.m_mhTimeseries = New cMessageHandler(AddressOf Me.OnCoreMessage, eCoreComponentType.TimeSeries, eMessageType.DataAddedOrRemoved, Me.SyncObject)

#If DEBUG Then
        Me.m_mhProgress.Name = "frmEwE6:Progress"
        Me.m_mhEcosim.Name = "frmEwE6:Ecosim"
        Me.m_mhEcospace.Name = "frmEwE6:EcoSpace"
        Me.m_mhEcotracer.Name = "frmEwE6:EcoTracer"
        Me.m_mhTimeseries.Name = "frmEwE6:TimeSeries"
#End If

        Me.Core.Messages.AddMessageHandler(Me.m_mhProgress)
        Me.Core.Messages.AddMessageHandler(Me.m_mhEcosim)
        Me.Core.Messages.AddMessageHandler(Me.m_mhEcospace)
        Me.Core.Messages.AddMessageHandler(Me.m_mhEcotracer)
        Me.Core.Messages.AddMessageHandler(Me.m_mhTimeseries)

        ' Create message history
        Me.m_messageHistory = New cMessageHistory()
        Me.m_messageHistory.UIContext = Me.UIContext

        ' Create plug-in manager for this GUI
        Me.m_pluginManager = New cPluginManager()
        Me.m_pluginManager.UIContext = Me.UIContext
        Me.m_pluginManager.SyncObject = Me.UIContext.SyncObject

        ' Configure plug-in manager
        Me.m_pluginManager.Core = Me.Core
        Me.m_pluginManager.UIContext = Me.UIContext

        ' Distribute plug-in manager
        Me.Core.PluginManager = Me.m_pluginManager

        ' Create plug-in menu handler to position plug-in menu items in the main menu from this form
        Me.m_pluginMenuHandler = New cPluginMenuHandler(Me.MainMenuStrip, Me.m_pluginManager, Me.UIContext.CommandHandler)

        ' Initialize core controller
        Me.m_coreController = New cCoreController(Me.Core.StateMonitor, Me.Core.StateManager, Me)

        ' Initialize style guide updater
        Me.m_styleguideupdater = New cStyleGuideUpdater(Me.UIContext)
        Me.m_styleguideupdater.Load()

        ' Initialize autosave logic
        Me.m_autosavemanager = New cAutosaveSettingsManager(Me.UIContext.Core)

        Me.Core.SetMessagePumpDelegate(AddressOf Me.OnPumpCoreMessages)

    End Sub

    Public ReadOnly Property PluginManager As cPluginManager
        Get
            Return Me.m_pluginManager
        End Get
    End Property

    Private Sub OnPumpCoreMessages()
        Try
            System.Windows.Forms.Application.DoEvents()
        Catch ex As Exception

        End Try
    End Sub

    Private Sub InitEventHandlers()

        AddHandler My.Settings.SettingsLoaded, AddressOf Me.OnSettingsLoaded
        AddHandler My.Settings.SettingsSaving, AddressOf Me.OnSettingsSaving
        AddHandler My.Settings.PropertyChanged, AddressOf Me.OnSettingsChanged

        ' JS 27Apr10: ActiveContent seems to track much more accurately than ActiveDocument
        AddHandler Me.m_DockPanel.ActiveContentChanged, AddressOf Me.OnTabFocusChanged

    End Sub

#End Region ' Initialization

#Region " Validation "

    Private Sub ValidateSetup()
        Me.m_bgw.RunWorkerAsync()
    End Sub

    Private Sub OnObtainServerTime(sender As Object, args As DoWorkEventArgs) Handles m_bgw.DoWork
        If Not cDateUtils.GetNetworkTime() Then
            cLog.Write("Unable to obtain server time")
        End If
    End Sub

    Private Sub OnServerTimeObtained(sender As Object, e As RunWorkerCompletedEventArgs) Handles m_bgw.RunWorkerCompleted
        If (Me.InvokeRequired()) Then
            Me.Invoke(New MethodInvoker(AddressOf Me.DoValidateSetup))
        Else
            Me.DoValidateSetup()
        End If
    End Sub

    Private Sub DoValidateSetup()
#If BETA = 1 Then
        Me.CheckBetaExpired()
#End If

        ' Auto-launch plugins
        Me.AutolaunchPlugins()
        ' Just in case some licensed plug-in updated itself upon launch
        Me.UpdateModelControls()

    End Sub

#End Region ' Validation

#Region " Properties "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns the file name of the current loaded model.
    ''' </summary>
    ''' <param name="bFullPath">Flag stating thether the full path needs to be 
    ''' returned.</param>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property SelectedFileName(Optional bFullPath As Boolean = True) As String
        Get
            If (Me.Core Is Nothing) Then Return ""
            Dim ds As IEwEDataSource = Me.Core.DataSource
            If (ds Is Nothing) Then
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

    Private Delegate Sub SendMessageDelegate(strMsg As String, importance As eMessageImportance, component As eCoreComponentType)

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Send a message via the core.
    ''' </summary>
    ''' <param name="strMsg">Message text to send.</param>
    ''' <param name="importance">Message importance.</param>
    ''' <param name="component">Core component to represent as message origin.</param>
    ''' -----------------------------------------------------------------------
    Public Sub SendMessage(strMsg As String,
                           Optional importance As eMessageImportance = eMessageImportance.Warning,
                           Optional component As eCoreComponentType = eCoreComponentType.External,
                           Optional strHyperlink As String = "")

        If Me.InvokeRequired() Then
            Me.Invoke(New SendMessageDelegate(AddressOf Me.SendMessage),
                                              New Object() {strMsg, importance, component})
            Return
        End If

        Dim msg As New cMessage(strMsg, eMessageType.Any, component, importance)
        msg.Hyperlink = strHyperlink

        Me.Core.Messages.SendMessage(msg)

    End Sub

    Private Delegate Function AskFeedbackDelegate(strMsg As String, importance As eMessageImportance, component As eCoreComponentType, replies As eMessageReplyStyle, defaultReply As eMessageReply, strHyperlink As String, vars As cVariableStatus()) As eMessageReply

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Ask for user feedback via the core feedback messaging system.
    ''' </summary>
    ''' <param name="strMsg">Message text to send.</param>
    ''' <param name="importance">Message importance.</param>
    ''' <param name="component">Core component to represent as message origin.</param>
    ''' -----------------------------------------------------------------------
    Public Function AskFeedback(strMsg As String,
                                Optional importance As eMessageImportance = eMessageImportance.Warning,
                                Optional component As eCoreComponentType = eCoreComponentType.Core,
                                Optional replystyle As eMessageReplyStyle = eMessageReplyStyle.YES_NO_CANCEL,
                                Optional defaultreply As eMessageReply = eMessageReply.YES,
                                Optional strHyperlink As String = "",
                                Optional vars As cVariableStatus() = Nothing) As eMessageReply

        If Me.InvokeRequired() Then
            Dim dlgt As New AskFeedbackDelegate(AddressOf Me.AskFeedback)
            Dim aparms() As Object = New Object() {strMsg, importance, component, replystyle, defaultreply, vars}
            Return DirectCast(Me.Invoke(dlgt, aparms), eMessageReply)
        End If

        Dim fmsg As New cFeedbackMessage(strMsg, component, eMessageType.Any, importance, replystyle, eDataTypes.NotSet, defaultreply)
        If (vars IsNot Nothing) Then fmsg.Variables.AddRange(vars)
        fmsg.Hyperlink = strHyperlink
        Me.Core.Messages.SendMessage(fmsg)
        Return fmsg.Reply

    End Function

#End Region ' Messages

#Region " Form overrides "

    Public Event OnLoadCompleted(sender As Object, args As EventArgs)

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Overridden to initialize the app launcer form.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Protected Overrides Sub OnLoad(e As System.EventArgs)

        ' Add the dock panel 
        Me.m_DockPanel = New DockPanel()
        Me.m_DockPanel.Parent = Me
        Me.m_DockPanel.Dock = DockStyle.Fill
        Me.m_DockPanel.ShowDocumentIcon = True
        Me.m_DockPanel.BringToFront()

        My.Settings.Reload()

        ' Peek at key presses but does not consume them
        Me.KeyPreview = True

        Me.InitCoreParams()
        Me.InitCommands()
        Me.InitPanels()
        Me.InitEventHandlers()

        Me.InitDockPanelPositions()

        ' Start controlling the status strip
        Me.m_ssMain.Attach(Me.UIContext, Me)
        ' Start controlling forms
        Me.m_formstatemanager = New cEwEFormStateManager(Me.Core.StateMonitor, Me.m_coreController, Me.m_DockPanel)

        ' Load plugins once GUI has been created.
        Me.LoadPlugins()

        ' JS 11Sep14: this will be done when settings are loaded
        'Me.Core.SpatialDataConnectionManager.DatasetManager.Load(My.Settings.SpatialTemporalConfigFile)

        Me.ProcessCommandLine()
        Me.OnSettingsLoaded(Nothing, Nothing) ' Ugh!
        Me.UpdateModelControls()
        Me.PopulateModelMRUDropdown()
        Me.UpdateRegistrationControls()

        Me.m_cmdHelpAbout.AddControl(Me.m_tsbnLicense)

        AddHandler Me.Core.StateMonitor.CoreExecutionStateEvent, AddressOf Me.OnCoreExecutionStateChanged

        Try
            RaiseEvent OnLoadCompleted(Me, New EventArgs())
        Catch ex As Exception
            cLog.Write(ex)
        End Try

        Me.m_bIsInitialized = True
        Me.Activate()

        Me.ValidateSetup()

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Event handler, overridden to keep the form hidden until the UI has fully
    ''' initialized
    ''' </summary>
    ''' <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param>
    ''' -----------------------------------------------------------------------
    Protected Overrides Sub OnActivated(e As EventArgs)
        If Me.m_bIsInitialized Then Me.Show()
        MyBase.OnActivated(e)
        If Not Me.m_bIsInitialized Then Me.Hide() Else Me.Show()
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Event handler, catches the form closing event to make sure the core is finalized.
    ''' Application shut-down is cancelled if the core does not finalize correctly.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Protected Overrides Sub OnFormClosing(e As FormClosingEventArgs)

        Try
            ' Cancel application shut down if the core does not terminate succesfully.
            e.Cancel = Not Me.CloseEcopathModel()
            ' Abort if Ecopath model did not close sucessfully
            If e.Cancel Then Return

        Catch ex As Exception
            cLog.Write(ex, "frmEwE6.OnFormClosing")
        End Try

        ' Resume shutdown
        MyBase.OnFormClosing(e)

    End Sub

    Protected Overrides Sub OnFormClosed(e As System.Windows.Forms.FormClosedEventArgs)

        If (Me.UIContext IsNot Nothing) Then

            Try

                Dim cmdh As cCommandHandler = Me.UIContext.CommandHandler
                RemoveHandler Application.Idle, AddressOf cmdh.OnIdle
                RemoveHandler Application.Idle, AddressOf Me.m_pluginMenuHandler.OnIdle

                RemoveHandler Me.Core.StateMonitor.CoreExecutionStateEvent, AddressOf Me.OnCoreExecutionStateChanged
                RemoveHandler Me.m_DockPanel.ActiveContentChanged, AddressOf Me.OnTabFocusChanged
                RemoveHandler My.Settings.SettingsLoaded, AddressOf Me.OnSettingsLoaded
                RemoveHandler My.Settings.SettingsSaving, AddressOf Me.OnSettingsSaving
                RemoveHandler My.Settings.PropertyChanged, AddressOf Me.OnSettingsChanged

                Me.m_formstatemanager.Dispose()
                Me.m_formstatemanager = Nothing

                Me.Core.Messages.RemoveMessageHandler(Me.m_mhProgress)
                Me.Core.Messages.RemoveMessageHandler(Me.m_mhEcosim)
                Me.Core.Messages.RemoveMessageHandler(Me.m_mhEcospace)
                Me.Core.Messages.RemoveMessageHandler(Me.m_mhEcotracer)
                Me.Core.Messages.RemoveMessageHandler(Me.m_mhTimeseries)
                Me.m_mhProgress = Nothing
                Me.m_mhEcosim = Nothing
                Me.m_mhEcospace = Nothing
                Me.m_mhEcotracer = Nothing
                Me.m_mhTimeseries = Nothing

                ' Terminate all model-independent UI components
                Me.CloseAllDocuments()
                Me.ClearScenarioDropdowns()
                Me.ClearModelMRUDropdowns()

                ' JS 13Dec10: Another attempt to free tooltip memory 
                Dim ts As cToolTipShared = cToolTipShared.GetInstance()
                ts.RemoveAll()
                ts.Dispose()

                Me.m_dtPanels.Clear()

                Me.m_messageHistory.Dispose()
                Me.m_messageHistory = Nothing

                Me.UIContext.PropertyManager.Dispose()
                Me.UIContext.StyleGuide.Dispose()

                Me.m_pluginManager.UIContext = Nothing
                Me.UIContext = Nothing

                ' Clear commands after all UI elements have lost their UI context, which 
                ' should have triggered proper cleanups
                cmdh.Clear()

                Try
                    ' For good measure, non-critical
                    Me.m_DockPanel.Dispose()
                Catch ex As Exception

                End Try

            Catch ex As Exception
                cLog.Write(ex, "frmEwE6.OnFormClosed")
            End Try
        End If

        MyBase.OnFormClosed(e)

    End Sub

#Region " KeyDown "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Cluck?
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Protected Overrides Sub OnKeyDown(e As KeyEventArgs)

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
            If e.Alt And e.Control And e.Shift Then
                Dim strURL As String = ""
                Select Case e.KeyCode
                    Case Keys.Oemtilde : strURL = "http://farm1.static.flickr.com/160/374820104_5ec655655c.jpg"
                End Select

                If Not String.IsNullOrEmpty(strURL) Then
                    Me.m_cmdBrowseURI.Invoke(strURL)
                End If

            End If
        Catch ex As Exception

        End Try
    End Sub

#End Region ' KeyDown

#Region " Drag and drop "

    Protected Overrides Sub OnDragOver(e As System.Windows.Forms.DragEventArgs)
        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            Dim files() As String = CType(e.Data.GetData(DataFormats.FileDrop), String())
            If files.Length > 0 Then
                If cDataSourceFactory.GetSupportedType(files(0)) <> eDataSourceTypes.NotSet Then
                    e.Effect = DragDropEffects.All
                End If
            End If
        End If
        MyBase.OnDragOver(e)
    End Sub

    Protected Overrides Sub OnDragDrop(e As System.Windows.Forms.DragEventArgs)
        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            Try
                Dim files() As String = CType(e.Data.GetData(DataFormats.FileDrop), String())
                If files.Length > 0 Then
                    Me.LoadEcopathModel(files(0), eLoadSourceType.User)
                End If
            Catch ex As Exception

            End Try
        End If
        MyBase.OnDragDrop(e)
    End Sub

#End Region ' Drag and drop

#End Region ' Form overrides

#Region " Status feedback "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Set the application status strip text and wait cursor.
    ''' </summary>
    ''' <param name="strText">Status text to display, if any.</param>
    ''' <param name="state">
    ''' <para>Flag stating whether a wait cursor should be shown.
    ''' Values are interpreted as follows:</para>
    ''' <list type="bullet">
    ''' <item><description><see cref="eProgressState.Start"/>: wait cursor will be set.</description></item>
    ''' <item><description><see cref="eProgressState.Finished"/>: wait cursor will be cleared.</description></item>
    ''' <item><description><see cref="eProgressState.Running"/>: wait cursor state will not change.</description></item>
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
    Private Sub ShowProgress(state As eProgressState, strText As String, sProgress As Single)

        ' Should have been handled
        If Me.InvokeRequired() Then Return

        ' ToDo_JS: Consider using a timer to clear any status text after a certain interval

        ' Update wait cursor
        Select Case state

            Case eProgressState.Start

                ' Push text to the status text stack
                Me.m_statusmessages.Insert(0, strText)
                ' Set wait cursor
                Me.Cursor = Cursors.WaitCursor

            Case eProgressState.Finished

                ' Has wait cursors pending?
                If Me.m_statusmessages.Count > 0 Then
                    ' #Yes: no text specified?
                    If String.IsNullOrEmpty(strText) Then
                        ' #Yes: obtain text from the status text stack
                        strText = Me.m_statusmessages(0)
                    End If
                    ' Pop text from the status text stack
                    Me.m_statusmessages.RemoveAt(0)
                End If

                ' Status stack empty?
                If Me.m_statusmessages.Count = 0 Then
                    ' #Yes: restore default cursor
                    Me.Cursor = Cursors.Default
                    strText = ""
                    sProgress = 0
                End If

            Case eProgressState.Running
                ' Don't do anything. Really.

        End Select

        ' Update status text
        Me.m_ssMain.SetStatusText(strText, sProgress)

    End Sub

#End Region ' Status feedback

#Region " Plug-ins "

    Private Sub LoadPlugins()
        If (Me.IsDisposed) Then Return
        If (Me.UIContext Is Nothing) Then Return
        Try
            Dim alDisabledPlugins As ArrayList = My.Settings.DisabledPlugins
            ' Load plug-ins from EwE root folder
            Me.m_pluginManager.LoadPlugins(alDisabledPlugins, [option]:=SearchOption.TopDirectoryOnly)
            ' Load plug-ins from dedicated plug-ins subfolder, recursively
            Me.m_pluginManager.LoadPlugins(alDisabledPlugins, ".\plugins")
        Catch ex As Exception
            ' Ouch!
        End Try
    End Sub

    Private Sub AutolaunchPlugins()
        If (Me.IsDisposed) Then Return
        If (Me.UIContext Is Nothing) Then Return
        Try
            Using pl As New cPluginAutolaunchHandler(Me.m_pluginManager, Me.UIContext.CommandHandler)
                ' Hah! The 'using' construction here will deal with proper disposal
            End Using
        Catch ex As Exception

        End Try
    End Sub

#End Region ' Plug-ins

#Region " Database utils "

    Private Function CompactModel(strFileName As String) As eDatasourceAccessType

        Debug.Assert(Not Me.Core.StateMonitor.HasEcopathLoaded())

        Dim ds As IEwEDataSource = cDataSourceFactory.Create(strFileName)
        Dim result As eDatasourceAccessType = eDatasourceAccessType.Success

        If Not ds.CanCompact(strFileName) Then
            Return eDatasourceAccessType.Failed_OSUnsupported
        End If

        cApplicationStatusNotifier.StartProgress(Me.Core, My.Resources.STATUS_MODEL_COMPACTING)
        ' Compacting should happen on separate thread. For now, bluntly refresh and worry about threading some other day
        Me.Refresh()
        result = ds.Compact(strFileName)
        cApplicationStatusNotifier.EndProgress(Me.Core)

        Return result

    End Function

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Test an incoming model link, and convert it to a local Ecopath 6 model if
    ''' possible.
    ''' </summary>
    ''' <param name="strFileName">File name of the Access database to convert. If a
    ''' conversion is necessary this parameter will receive the file name of the
    ''' converted file.</param>
    ''' <returns>A <see cref="cEwEDatabase.eCompatibilityTypes"/> value</returns>
    ''' <remarks>
    ''' This logic will need to change entirely. A database 
    ''' </remarks>
    ''' ---------------------------------------------------------------------------
    Private Function CovertToEwE6(ByRef strFileName As String) As cEwEDatabase.eCompatibilityTypes

        Dim comp As cEwEDatabase.eCompatibilityTypes = cEwEDatabase.eCompatibilityTypes.Unknown
        Dim access As eDatasourceAccessType = eDatasourceAccessType.Failed_Unknown
        Dim links As New cWebLinks(Me.Core)

        ' Get compatibility
        comp = cDataSourceFactory.GetCompatibility(strFileName, access)

        ' Has access problems?
        If (access <> eDatasourceAccessType.Opened) Then
            ' #Yes: report access error
            Me.ReportFileAccessError(access, strFileName)
            Return cEwEDatabase.eCompatibilityTypes.Unknown
        End If

        ' Able to access ok; assess compatibility next
        Select Case comp

            Case cEwEDatabase.eCompatibilityTypes.TooOld
                Me.SendMessage(cStringUtils.Localize(My.Resources.PROMPT_ERROR_IMPORT_EWE5_TOO_OLD, links.GetURL(cWebLinks.eLinkType.Home)),
                               strHyperlink:=links.GetURL(cWebLinks.eLinkType.Home))

            Case cEwEDatabase.eCompatibilityTypes.Importable

                If (File.Exists(strFileName)) Then
                    Me.AddModelMRU(strFileName)
                End If

                Dim dlg As New Import.dlgImportDatabase(Me.UIContext, strFileName)
                If dlg.ShowDialog(Me) = DialogResult.OK Then
                    ' Update file name
                    strFileName = dlg.ImportedFileName
                    comp = cEwEDatabase.eCompatibilityTypes.EwE6
                End If

            Case cEwEDatabase.eCompatibilityTypes.EwE6
                ' Yippee

            Case cEwEDatabase.eCompatibilityTypes.Future
                If Me.AskFeedback(cStringUtils.Localize(My.Resources.PROMPT_ERROR_IMPORT_EWE6_TOO_NEW, links.GetURL(cWebLinks.eLinkType.Home)),
                                  eMessageImportance.Question,
                                  eCoreComponentType.DataSource,
                                  eMessageReplyStyle.YES_NO,
                                  strHyperlink:=links.GetURL(cWebLinks.eLinkType.Home)) = eMessageReply.NO Then
                    comp = cEwEDatabase.eCompatibilityTypes.Unknown
                End If

            Case cEwEDatabase.eCompatibilityTypes.Unknown
                Me.SendMessage(My.Resources.PROMPT_ERROR_IMPORT_INVALIDDB)

            Case Else
                ' Unsupported enum value?!
                Debug.Assert(False)
                comp = cEwEDatabase.eCompatibilityTypes.Unknown

        End Select

        Return comp

    End Function

    Private Sub ReportFileAccessError(atResult As eDatasourceAccessType, strFileName As String)

        Dim strMessage As String = ""
        Dim strHyperlink As String = ""

        Select Case atResult
            Case eDatasourceAccessType.Failed_ReadOnly
                strMessage = cStringUtils.Localize(My.Resources.STATUS_MODEL_ACCESS_READONLY, strFileName)
            Case eDatasourceAccessType.Failed_OSUnsupported
                strMessage = cStringUtils.Localize(My.Resources.STATUS_MODEL_ACCESS_OS, strFileName)
                Dim link As New cWebLinks(Me.Core)
                strHyperlink = link.GetURL(cWebLinks.eLinkType.Access2010)
            Case eDatasourceAccessType.Failed_FileNotFound
                strMessage = cStringUtils.Localize(My.Resources.STATUS_MODEL_ACCESS_404, strFileName)
            Case eDatasourceAccessType.Failed_CannotSave
                strMessage = cStringUtils.Localize(My.Resources.STATUS_MODEL_SAVE_404, strFileName)
            Case Else
                strMessage = cStringUtils.Localize(My.Resources.STATUS_MODEL_ACCESS_FAILED, strFileName)
        End Select

        Me.SendMessage(strMessage, eMessageImportance.Warning, eCoreComponentType.DataSource, strHyperlink:=strHyperlink)

    End Sub

#End Region ' Database utils

#Region " UI updates "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Helper method, updates the state of controls reflecting the current model. 
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub UpdateModelControls()

        Dim strCaption As String = EwE6ApplicationFramework.EwEVersion(False, True, True)
        Dim model As cEwEModel = Me.Core.EwEModel
        Dim bIsReadOnly As Boolean = False

        Me.m_tsModel.Path = Me.SelectedFileName
        If Me.Core.StateMonitor.HasEcopathLoaded Then
            Dim strModel As String = model.Name
            bIsReadOnly = Me.Core.DataSource.IsReadOnly
            If (bIsReadOnly) Then
                strModel = cStringUtils.Localize(SharedResources.GENERIC_LABEL_DETAILED, strModel, My.Resources.MODE_READ_ONLY)
            End If
            strCaption = cStringUtils.Localize(SharedResources.GENERIC_LABEL_CAPTION, strCaption, strModel)
        End If

        Me.Text = strCaption
        Me.m_tslbReadOnly.Visible = bIsReadOnly

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Helper method, populate the content of the scenario drop-down controls
    ''' with lists of scenarios available in the current model. 
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub PopulateScenarioDropdowns()

        Dim tsi As ToolStripMenuItem = Nothing
        Dim fmt As New cTimeSeriesDatasetIntervalTypeFormatter()

        Me.ClearScenarioDropdowns()

        ' Has a model loaded?
        If Me.Core.StateMonitor.HasEcopathLoaded() Then

            ' #Yes: add scenario lists

            ' VERIFY_JS: Should scenarios be sorted in the most recent load order, or is that going to be highly confusing?

            ' List available Ecosim scenarios.
            For i As Integer = 1 To Me.Core.nEcosimScenarios
                tsi = New ToolStripMenuItem()
                tsi.Tag = Me.Core.EcosimScenarios(i)
                tsi.Checked = (Me.Core.ActiveEcosimScenarioIndex = i)
                AddHandler tsi.Click, AddressOf Me.OnLoadEcosimScenarioOrDataset
                Me.m_tsbEcosim.DropDownItems.Add(tsi)
            Next

            ' List available Ecosim time series datasets
            For i As Integer = 1 To Me.Core.nTimeSeriesDatasets

                ' Is first dataset?
                If (i = 1) Then
                    ' #Yes: add a separator
                    Me.m_tsbEcosim.DropDownItems.Add(New ToolStripSeparator())
                End If

                tsi = New ToolStripMenuItem()
                tsi.Tag = Me.Core.TimeSeriesDataset(i)
                tsi.Checked = (Me.Core.ActiveTimeSeriesDatasetIndex = i)

                AddHandler tsi.Click, AddressOf Me.OnLoadEcosimScenarioOrDataset
                Me.m_tsbEcosim.DropDownItems.Add(tsi)

            Next i

            ' List available Ecospace scenarios
            For i As Integer = 1 To Me.Core.nEcospaceScenarios
                tsi = New ToolStripMenuItem()
                tsi.Tag = Me.Core.EcospaceScenarios(i)
                tsi.Checked = (Me.Core.ActiveEcospaceScenarioIndex = i)
                AddHandler tsi.Click, AddressOf Me.OnLoadEcospaceScenario
                Me.m_tsbEcospace.DropDownItems.Add(tsi)
            Next

            '' List available spatial temporal datasets
            'Dim man As cSpatialDataSetManager = Me.Core.SpatialDataConnectionManager.DatasetManager
            'For i As Integer = 1 To man.ConfigFiles.Count

            '    ' Is first dataset?
            '    If (i = 1) Then
            '        ' #Yes: add a separator
            '        Me.m_tsbEcospace.DropDownItems.Add(New ToolStripSeparator())
            '        tsmi = New ToolStripMenuItem()
            '        tsmi.Text = SharedResources.GENERIC_VALUE_DEFAULT
            '        tsmi.Tag = ""
            '        tsmi.Checked = (cSpatialDataSetManager.DefaultConfigFile = man.CurrentConfigFile)

            '        AddHandler tsmi.Click, AddressOf OnLoadEcospaceScenario
            '        Me.m_tsbEcospace.DropDownItems.Add(tsmi)
            '    End If

            '    strItem = CStr(man.ConfigFiles(i - 1))
            '    tsmi = New ToolStripMenuItem()
            '    tsmi.Text = strItem
            '    tsmi.Tag = strItem
            '    tsmi.Checked = (strItem = man.CurrentConfigFile)

            '    AddHandler tsmi.Click, AddressOf OnLoadEcospaceScenario
            '    Me.m_tsbEcospace.DropDownItems.Add(tsmi)

            'Next i

            ' List available Ecotracer scenarios
            For i As Integer = 1 To Me.Core.nEcotracerScenarios
                tsi = New ToolStripMenuItem()
                tsi.Tag = Me.Core.EcotracerScenarios(i)
                tsi.Checked = (Me.Core.ActiveEcotracerScenarioIndex = i)
                AddHandler tsi.Click, AddressOf Me.OnLoadEcotracerScenario
                Me.m_tsbEcotracer.DropDownItems.Add(tsi)
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
                RemoveHandler tsi.Click, AddressOf Me.OnLoadEcosimScenarioOrDataset
            End If
        Next
        Me.m_tsbEcosim.DropDownItems.Clear()

        ' Properly release space menu items
        For Each tsi In Me.m_tsbEcospace.DropDownItems
            If Not (TypeOf tsi Is ToolStripSeparator) Then
                RemoveHandler tsi.Click, AddressOf Me.OnLoadEcospaceScenario
            End If
        Next
        Me.m_tsbEcospace.DropDownItems.Clear()

        ' Properly release tracer menu items
        For Each tsi In Me.m_tsbEcotracer.DropDownItems
            RemoveHandler tsi.Click, AddressOf Me.OnLoadEcotracerScenario
        Next
        Me.m_tsbEcotracer.DropDownItems.Clear()

    End Sub

    Private Sub UpdateRegistrationControls()

        Try
            If Me.Core.License.IsRegistered Then
                Dim diff As Integer = Me.Core.License.Expiry.Subtract(Date.Now).Days
                If diff > 28 Then ' Start warning four weeks prior expiration
                    Me.m_tsbnLicense.Image = SharedResources.license_ok
                ElseIf diff > 0 Then
                    Me.m_tsbnLicense.Image = SharedResources.license_expiring
                Else
                    Me.m_tsbnLicense.Image = SharedResources.license_expired
                End If

                Me.m_tsbnLicense.Text = EwELicense(Me.Core.License)
                Me.m_tsbnLicense.ToolTipText = EwE6ApplicationFramework.EwERegistration(Me.Core.License)
                Me.m_tsbnLicense.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText
            Else
                Me.m_tsbnLicense.Text = EwELicense(Nothing)
                Me.m_tsbnLicense.ToolTipText = ""
                Me.m_tsbnLicense.DisplayStyle = ToolStripItemDisplayStyle.Text
            End If
        Catch ex As Exception

        End Try

    End Sub

#End Region ' UI updates

#Region " Settings "

    Private Sub SaveMainFormSettings()

        If (Me.UIContext IsNot Nothing) Then
            Me.UIContext.FormSettings.Store(Me, False)
            Me.m_styleguideupdater.Save()
            My.Settings.FormSettings = Me.UIContext.FormSettings.Setting
        End If
        Me.SaveSettings()

    End Sub

    Private Sub SaveSettings()

        Dim man As cSpatialDataSetManager = Me.Core.SpatialDataConnectionManager.DatasetManager

        If (man IsNot Nothing) Then
            My.Settings.SpatialTempConfigurations = man.ConfigFiles
            My.Settings.SpatialTemporalConfigFile = man.CurrentConfigFile
        End If

        Dim pm As cPluginManager = Me.Core.PluginManager
        If (pm IsNot Nothing) Then
            My.Settings.DisabledPlugins = pm.DisabledPlugins
        End If

        My.Settings.Save()

    End Sub

#End Region ' Settings

#Region " MRU "

#Region " Models "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Add a EwE DB name to the top of the MRU list.
    ''' </summary>
    ''' <param name="strFileName">Name of the file to add.</param>
    ''' -----------------------------------------------------------------------
    Private Sub AddModelMRU(strFileName As String)

        Dim alMDBmru As ArrayList = My.Settings.MdbRecentlyUsedList

        If (alMDBmru Is Nothing) Then Return

        ' Insert at head
        alMDBmru.Insert(0, strFileName)
        ' Remove any occurrences further down the list
        Me.RemoveModelMRU(strFileName, 1)

        ' Update system settings
        My.Settings.MdbRecentlyUsedList = alMDBmru
        Me.SaveSettings()

        ' Update UI
        Me.PopulateModelMRUDropdown()

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
    Private Sub RemoveModelMRU(strFileName As String,
                               Optional iStartPos As Integer = 0)

        Dim alMDBmru As ArrayList = My.Settings.MdbRecentlyUsedList

        If (alMDBmru Is Nothing) Then Return

        ' Remove all occurrences from the list
        While iStartPos < alMDBmru.Count - 1
            If (TypeOf alMDBmru(iStartPos) Is String) Then
                ' Get entry
                Dim strEntry As String = CStr(alMDBmru(iStartPos))
                ' Is same file?
                If (String.Compare(strEntry, strFileName, True) = 0) Then
                    ' #Yes: remove 
                    alMDBmru.RemoveAt(iStartPos)
                    iStartPos -= 1
                End If
            End If
            iStartPos += 1
        End While
        My.Settings.MdbRecentlyUsedList = alMDBmru

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Show the list of MRU items in the menu structure.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub PopulateModelMRUDropdown()

        Dim alMRU As ArrayList = My.Settings.MdbRecentlyUsedList
        Dim iNumItems As Integer = Math.Min(alMRU.Count - 1, My.Settings.MdbRecentlyUsedCount)
        Dim item As ToolStripMenuItem = Nothing
        Dim bHasMRU As Boolean = False

        ' Clear MRU list
        Me.ClearModelMRUDropdowns()

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
            item.Text = cStringUtils.Localize(SharedResources.GENERIC_LABEL_INDEXED, i + 1, str(0))
            item.Tag = str(0)

            'Add event handler to invoke the model
            AddHandler item.Click, AddressOf Me.OnModelMRUItemClicked

            Me.m_tsmiFileRecent.DropDownItems.Add(item)

            item = New ToolStripMenuItem()
            item.Text = str(0)
            item.Tag = str(0)
            item.Checked = (String.Compare(str(0), Me.SelectedFileName, True) = 0)

            'Add event handler to invoke the model
            AddHandler item.Click, AddressOf Me.OnModelMRUItemClicked

            Me.m_tsbEcopath.DropDownItems.Add(item)
        Next

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Clear the list of MRU items and attached event handlers.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub ClearModelMRUDropdowns()

        Dim item As ToolStripMenuItem = Nothing

        For Each item In Me.m_tsmiFileRecent.DropDownItems
            If (item.Tag IsNot Nothing) Then
                ' Remove dangling event handler
                RemoveHandler item.Click, AddressOf Me.OnModelMRUItemClicked
            End If
        Next
        ' Eradicate menu items
        Me.m_tsmiFileRecent.DropDownItems.Clear()


        For Each item In Me.m_tsbEcopath.DropDownItems
            If (item.Tag IsNot Nothing) Then
                ' Remove dangling event handler
                RemoveHandler item.Click, AddressOf Me.OnModelMRUItemClicked
            End If
        Next
        Me.m_tsbEcopath.DropDownItems.Clear()

    End Sub

#End Region ' Models

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
    Private Function LoadFormFromType(strNavLink As String,
                                      t As Type,
                                      state As eCoreExecutionState) As Form

        Dim classObject As Object
        Dim frmNew As Form = Nothing
        Dim strCaption As String = ""

        If t Is Nothing Then Return Nothing

        Try

            classObject = Activator.CreateInstance(t)

            If TypeOf classObject Is DockContent Then
                ' Is dock content
                frmNew = DirectCast(classObject, DockContent)
            ElseIf TypeOf classObject Is cEwEGrid Then
                ' Is a grid
                Dim grid As cEwEGrid = DirectCast(classObject, cEwEGrid)
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

            ' JS March 19: Form icons are now handled by frmEwE baseclass to ensure disposal

        Catch ex As Exception
            cLog.Write(ex, "frmEwE6.LoadFormFromType(" & t.ToString & ", " & strNavLink & ")")
            ' Notify user
            Me.SendMessage(My.Resources.UI_ERROR_LAUNCHFORM, eMessageImportance.Warning, strHyperlink:="command:" & cBrowserCommand.COMMAND_NAME & "?URL=" & cLog.LogFile)
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
    Private Function ActivateForm(strNavLink As String) As Boolean

        Dim bFound As Boolean = False

        ' Dock settings, loop through current opened 
        For Each cnt As DockContent In Me.m_DockPanel.Contents

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

    Private Sub UpdateSelectedNode(strNodeName As String,
                                   Optional bAllowDefault As Boolean = False)

        If Me.m_bNavigating Then Return

        ' Default switching?
        If String.IsNullOrEmpty(strNodeName) And bAllowDefault Then
            ' #Yes: can reactivate current node?
            If Me.ActivateForm(Me.m_strLastActiveContent) Then Return
        End If

        Me.m_bNavigating = True

        ' Remember new page
        Me.m_strLastActiveContent = strNodeName
        ' Kick nav panel
        DirectCast(Me.Panel(cPANEL_NAV), frmNavigationPanel).SelectedNodeName(bAllowDefault) = strNodeName

        Me.m_bNavigating = False

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Private method to close all open child forms of the parent form.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub CloseAllDocuments()

        Dim lForms As New List(Of Form)
        Dim bIsReserved As Boolean = False

        ' Make temp list of all documents that may be closed. This cannot
        ' be performed in a for..ech loop because that affects the iterator
        ' used in the loop.
        For Each f As DockContent In Me.m_DockPanel.Contents

            If TypeOf (f) Is frmEwEDockContent Then
                ' Keep system panels open
                bIsReserved = (DirectCast(f, frmEwEDockContent).PanelType = frmEwEDockContent.ePanelType.SystemPanel)
            Else
                bIsReserved = False
            End If

            If Not bIsReserved Then
                lForms.Add(f)
            End If
        Next

        ' Now close the forms
        For Each f As Form In lForms
            f.Close()
        Next
        lForms = Nothing

        Me.UpdateSelectedNode("", False)
        Me.UIContext.Help.Clear()

    End Sub

#End Region ' Content navigation

#Region " Ecopath "

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Open Ecopath model from a given location.
    ''' </summary>
    ''' <param name="strFileName">Location of the model to open.</param>
    ''' <param name="loadsource">Flag indicating where the load request came from.</param>
    ''' <remarks>This code is designed for strFileName to indicate a path. It should 
    ''' be possible to indicate a database as well. One day...</remarks>
    ''' ---------------------------------------------------------------------------
    Private Function LoadEcopathModel(strFileName As String,
                                      loadsource As eLoadSourceType) As Boolean

        Dim ds As IEwEDataSource = Nothing
        Dim atResult As eDatasourceAccessType = eDatasourceAccessType.Failed_Unknown
        Dim bReadOnly As Boolean = False

        Select Case cDataSourceFactory.GetSupportedType(strFileName)

            Case eDataSourceTypes.Access2003, eDataSourceTypes.Access2007,
                 eDataSourceTypes.EII, eDataSourceTypes.EIIXML

                ' Check if target file exists at all before affecting anything
                If Not File.Exists(strFileName) Then

                    ' Handle failure
                    Select Case loadsource

                        Case eLoadSourceType.MRU
                            If Me.AskFeedback(cStringUtils.Localize(My.Resources.PROMPT_MODELNOTFOUND_REMOVEMRU, strFileName),
                                              replystyle:=eMessageReplyStyle.YES_NO) = eMessageReply.YES Then
                                Me.RemoveModelMRU(strFileName)
                                Me.PopulateModelMRUDropdown()
                            End If

                        Case eLoadSourceType.User,
                             eLoadSourceType.CommandLine
                            ' Unable to load model, show generic error
                            Me.SendMessage(cStringUtils.Localize(My.Resources.PROMPT_MODELNOTFOUND, strFileName),
                                           eMessageImportance.Warning, eCoreComponentType.DataSource)

                        Case eLoadSourceType.API
                            ' Do not provide user feedback in response to an API call

                    End Select

                    ' Update system settings
                    Me.SaveSettings()
                    Return False
                End If

            Case Else
                'NOP

        End Select

        ' Can close the current open model, if any?
        If Not Me.CloseEcopathModel() Then
            ' #No: cannot close - abort
            Return False
        End If

        Select Case Me.CovertToEwE6(strFileName)
            Case cEwEDatabase.eCompatibilityTypes.EwE6
                ' EwE6 database? OK
            Case cEwEDatabase.eCompatibilityTypes.Future
                ' Newer version: try to open
                bReadOnly = True
            Case Else
                ' No EwE6 database? abort
                Return False
        End Select

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
                    Me.SendMessage(cStringUtils.Localize(My.Resources.PROMPT_INVALIDMODEL, strFileName),
                                   eMessageImportance.Warning, eCoreComponentType.DataSource)

                Case eLoadSourceType.API
                    ' Ok then

            End Select
            Return False
        End If

        ' Update MRU
        Me.AddModelMRU(strFileName)

        ' Open the datasource
        atResult = ds.Open(strFileName, Me.Core, eDataSourceTypes.NotSet, bReadOnly)

        If (atResult <> eDatasourceAccessType.Success) Then
            Me.ReportFileAccessError(atResult, strFileName)
            Return False
        End If

        ' Ok, now let's see if the core can work with this
        If Me.Core.LoadModel(ds) Then
            ' Set core paths
            Me.UpdateCorePaths(True)
            Me.PopulateScenarioDropdowns()

            Me.m_propModelName = Me.PropertyManager.GetProperty(Me.Core.EwEModel, eVarNameFlags.Name)
            AddHandler Me.m_propModelName.PropertyChanged, AddressOf Me.OnModelNameChanged

            ' Remember last used model directory
            My.Settings.LastSelectedDirectory = Path.GetDirectoryName(strFileName)
            My.Settings.Save()
            Return True
        Else
            Dim msg As New cMessage(cStringUtils.Localize(My.Resources.GENERIC_ERROR_FILEOPEN, strFileName), eMessageType.Any, eCoreComponentType.Core, eMessageImportance.Critical)
            Me.Core.Messages.SendMessage(msg)
            ds.Close()
            Return False
        End If

    End Function

    Private m_propModelName As cProperty = Nothing

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Save model to a different datasource and switch to that new datasource. 
    ''' </summary>
    ''' <param name="strFileName">Full path + extension of the file to save.</param>
    ''' ---------------------------------------------------------------------------
    Private Function SaveEcopathModelAs(strFileName As String) As Boolean

        If (Me.Core.Save(strFileName)) Then
            Me.AddModelMRU(strFileName)
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
    ''' <returns>An Ecopath database, if successful.</returns>
    ''' <remarks>
    ''' Note that this will NOT load the new model! For this, 
    ''' <see cref="LoadEcopathModel"/> will need to be called.
    ''' </remarks>
    ''' ---------------------------------------------------------------------------
    Friend Function CreateEcopathModel(strFileName As String,
                                        strModelName As String,
                                        format As eDataSourceTypes) As cEwEDatabase

        Dim db As cEwEDatabase = Nothing
        Dim atResult As eDatasourceAccessType = eDatasourceAccessType.Failed_Unknown
        Dim strPrompt As String = ""
        Dim importance As eMessageImportance = eMessageImportance.Warning

        Select Case format
            Case eDataSourceTypes.Access2003, eDataSourceTypes.Access2007
                If File.Exists(strFileName) Then
                    Dim fmsg As New cFeedbackMessage(cStringUtils.Localize(My.Resources.GENERIC_PROMPT_OVERWRITEFILE, strFileName),
                                                     eCoreComponentType.DataSource, eMessageType.DataValidation,
                                                     eMessageImportance.Question, eMessageReplyStyle.YES_NO)
                    fmsg.Reply = eMessageReply.NO
                    Me.Core.Messages.SendMessage(fmsg)
                    If fmsg.Reply = eMessageReply.NO Then Return Nothing
                End If
                db = New cEwEAccessDatabase()
                atResult = db.Create(strFileName, strModelName, True, format, Me.Core.DefaultAuthor)

            Case eDataSourceTypes.EII
                atResult = eDatasourceAccessType.Failed_DeprecatedOperation

            Case eDataSourceTypes.NotSet
                atResult = eDatasourceAccessType.Failed_UnknownType
        End Select

        ' Provide status feedback
        Select Case atResult

            Case eDatasourceAccessType.Success, eDatasourceAccessType.Opened
                strPrompt = cStringUtils.Localize(My.Resources.PROMPT_MODELCREATED, strFileName)
                importance = eMessageImportance.Information

            Case eDatasourceAccessType.Failed_CannotSave
                strPrompt = cStringUtils.Localize(My.Resources.PROMPT_INVALIDTARGETPATH, strFileName)
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
                strPrompt = cStringUtils.Localize(My.Resources.PROMPT_CREATE_GENERICERROR, strFileName)
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
    ''' <returns>An Ecopath database, if successful.</returns>
    ''' <remarks>
    ''' <para>Note that this will NOT load the new model! For this, 
    ''' <see cref="LoadEcopathModel"/> will need to be called.</para>
    ''' <para>This method distills the database type from the provided file name.</para>
    ''' </remarks>
    ''' ---------------------------------------------------------------------------
    Friend Function CreateEcopathModel(strFileName As String,
                                        strModelName As String) As cEwEDatabase
        Return Me.CreateEcopathModel(strFileName,
                                     strModelName,
                                     cDataSourceFactory.GetSupportedType(strFileName))
    End Function

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Close the current open Ecopath Model
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Private Function CloseEcopathModel() As Boolean

        Dim strFileName As String = Me.SelectedFileName

        ' Save form settings
        Me.SaveMainFormSettings()

        If Not String.IsNullOrEmpty(strFileName) Then

            Me.m_cmdPropertySelection.Invoke()

            ' Not allowed to terminate core?
            If (Not Me.Core.CloseModel()) Then
                ' #Not allowed: abort
                Return False
            End If

            RemoveHandler Me.m_propModelName.PropertyChanged, AddressOf Me.OnModelNameChanged
            Me.m_propModelName = Nothing

            ' Close all open documents
            Me.CloseAllDocuments()
            Me.ClearScenarioDropdowns()

            Me.m_autosavemanager.GatherSettings()

            ' Automatic maintenance
            If My.Settings.AutoCompact Then
                Me.CompactModel(strFileName)
            End If

            ' Reset components
            DirectCast(Me.Panel(cPANEL_NAV), frmNavigationPanel).Reset()
            DirectCast(Me.Panel(cPANEL_STATUS), frmStatusPanel).Reset()

            ' Clear the properties cache
            Me.UIContext.PropertyManager.Clear(eCoreComponentType.Ecopath)

            ' Clean up UI bits
            Me.UpdateModelControls()
            Me.ClearScenarioDropdowns()

            ' Invalidate, do not redraw
            Me.Invalidate()
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
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Friend Function LoadEcosimScenario(Optional bTryReuse As Boolean = False) As Boolean

        Dim dlg As EcosimScenarioDlg = Nothing
        Dim bSucces As Boolean = False
        Dim es As cEcoSimScenario = Nothing

        ' Try to obtain ecosim scenario to load

        ' Invoked from a command?
        If (Me.m_cmdLoadEcosimScenario.IsInvoking()) Then
            ' #Yes: try to obtain scenario from command
            es = DirectCast(Me.m_cmdLoadEcosimScenario.Tag, cEcoSimScenario)
            ' #No: Are we reloading and an active scenario is present
        ElseIf (bTryReuse = True) And (Me.Core.ActiveEcosimScenarioIndex >= 0) Then
            Return True
        ElseIf Me.Core.nEcosimScenarios = 1 Then
            ' Automatically load the only available scenario
            es = Me.Core.EcosimScenarios(1)
        End If

        ' No scenario found yet?
        If (es Is Nothing) Then
            ' #No scenario: invoke ecosim scenario selection dialog
            dlg = New EcosimScenarioDlg(Me.UIContext, EcosimScenarioDlg.eDialogModeType.LoadScenario)
            If (dlg.ShowDialog() = System.Windows.Forms.DialogResult.OK) Then

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

        Return Me.LoadEcosimScenario(es)

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Load an Ecosim scenario.
    ''' </summary>
    ''' <param name="es">The <see cref="cEcoSimScenario">Scenario</see> to load.</param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Private Function LoadEcosimScenario(es As cEcoSimScenario) As Boolean

        Dim bSucces As Boolean = False

        If (es IsNot Nothing) Then
            ' #Yes: Load it
            cApplicationStatusNotifier.StartProgress(Me.Core, cStringUtils.Localize(My.Resources.STATUS_ECOSIM_LOADING, es.Name))
            bSucces = Me.Core.LoadEcosimScenario(es)
            Me.m_autosavemanager.ApplySettingsAndEnsureDefaults()
            cApplicationStatusNotifier.EndProgress(Me.Core)
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
    Private Function CreateEcosimScenario(strName As String, strDescription As String, strAuthor As String, strContact As String) As Boolean

        Dim bSucces As Boolean = False

        cApplicationStatusNotifier.StartProgress(Me.Core, cStringUtils.Localize(My.Resources.STATUS_ECOSIM_CREATING, strName))
        bSucces = Me.Core.NewEcosimScenario(strName, strDescription, strAuthor, strContact)
        cApplicationStatusNotifier.EndProgress(Me.Core)
        Return bSucces

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Invoke the manage time series interface.
    ''' </summary>
    ''' <param name="mode"><see cref="dlgManageTimeSeries.eModeType">Mode</see>
    ''' specifying how to open the interface.</param>
    ''' -----------------------------------------------------------------------
    Private Sub ManageTimeSeries(mode As dlgManageTimeSeries.eModeType)

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
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Friend Function LoadEcospaceScenario(Optional bTryReuse As Boolean = False) As Boolean

        Dim dlg As dlgEcospaceScenario = Nothing
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
        ElseIf (Me.Core.nEcospaceScenarios = 1) Then
            ' Automatically load the only available scenario
            es = Me.Core.EcospaceScenarios(1)
        End If

        ' No scenario found yet?
        If (es Is Nothing) Then
            ' #No scenario: invoke ecospace scenario selection dialog
            dlg = New dlgEcospaceScenario(Me.UIContext, dlgEcospaceScenario.eDialogModeType.LoadScenario)
            If (dlg.ShowDialog() = System.Windows.Forms.DialogResult.OK) Then

                Select Case dlg.Mode
                    Case dlgEcospaceScenario.eDialogModeType.CreateScenario
                        ' User wants to create a scenario instead
                        Return Me.CreateEcospaceScenario(dlg.ScenarioName, dlg.ScenarioDescription,
                                dlg.ScenarioAuthor, dlg.ScenarioContact,
                                10, 10, 0, 0, 0.5)
                    Case dlgEcospaceScenario.eDialogModeType.LoadScenario
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
    Private Function CreateEcospaceScenario(strName As String, strDescription As String,
            strAuthor As String, strContact As String,
            iNumRows As Integer, iNumCols As Integer,
            sLatTL As Single, sLonTL As Single, sCellSize As Single) As Boolean

        Dim bSucces As Boolean = False

        cApplicationStatusNotifier.StartProgress(Me.Core, cStringUtils.Localize(My.Resources.STATUS_ECOSPACE_CREATING, strName))
        bSucces = Me.Core.NewEcospaceScenario(strName, strDescription,
            strAuthor, strContact, iNumRows, iNumCols, sLatTL, sLonTL, sCellSize)
        cApplicationStatusNotifier.EndProgress(Me.Core)
        Return bSucces

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="es"></param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Private Function LoadEcospaceScenario(es As cEcospaceScenario) As Boolean

        Dim bSucces As Boolean = False

        If (es IsNot Nothing) Then
            ' #Yes: Load it
            cApplicationStatusNotifier.StartProgress(Me.Core, cStringUtils.Localize(My.Resources.STATUS_ECOSPACE_LOADING, es.Name))
            bSucces = Me.Core.LoadEcospaceScenario(es)
            Me.m_autosavemanager.ApplySettingsAndEnsureDefaults()
            cApplicationStatusNotifier.EndProgress(Me.Core)
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
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Friend Function LoadEcotracerScenario(Optional bTryReuse As Boolean = False) As Boolean

        Dim dlg As dlgEcotracerScenario = Nothing
        Dim bSucces As Boolean = False
        Dim es As cEcotracerScenario = Nothing

        ' Prerequesite: Ecosim needs to be loaded
        Me.CoreController.LoadState(eCoreExecutionState.EcosimLoaded)
        ' Not successful? abort
        If Not Me.Core.StateMonitor.HasEcosimLoaded Then Return False

        ' Try to obtain ecotracer scenario to load

        ' Invoked from a command?
        If (Me.m_cmdLoadEcotracerScenario.IsInvoking()) Then
            ' #Yes: try to obtain scenario from command
            es = CType(Me.m_cmdLoadEcotracerScenario.Tag, cEcotracerScenario)
            ' #No: Are we reloading and an active scenario is present?
        ElseIf (bTryReuse = True) And (Me.Core.ActiveEcotracerScenarioIndex >= 0) Then
            Return True
        ElseIf (Me.Core.nEcotracerScenarios = 1) Then
            ' Automatically load the only available scenario
            es = Me.Core.EcotracerScenarios(1)
        End If

        ' No scenario found yet?
        If (es Is Nothing) Then
            ' #No scenario: invoke ecotracer scenario selection dialog
            dlg = New dlgEcotracerScenario(Me.UIContext, dlgEcotracerScenario.eDialogModeType.LoadScenario)
            If (dlg.ShowDialog() = System.Windows.Forms.DialogResult.OK) Then

                Select Case dlg.Mode
                    Case dlgEcotracerScenario.eDialogModeType.CreateScenario
                        ' User wants to create a scenario instead
                        Return Me.CreateEcotracerScenario(dlg.ScenarioName, dlg.ScenarioDescription, dlg.ScenarioAuthor, dlg.ScenarioContact)
                    Case dlgEcotracerScenario.eDialogModeType.LoadScenario
                        ' User wants to load a scenario
                        es = DirectCast(dlg.Scenario, cEcotracerScenario)
                    Case Else
                        Debug.Assert(False)
                End Select

            End If
        End If

        Return Me.LoadEcotracerScenario(es)

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="strName"></param>
    ''' <param name="strDescription"></param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Private Function CreateEcotracerScenario(strName As String, strDescription As String, strAuthor As String, strContact As String) As Boolean

        Dim bSucces As Boolean = False

        cApplicationStatusNotifier.StartProgress(Me.Core, cStringUtils.Localize(My.Resources.STATUS_ECOTRACER_CREATING, strName))
        bSucces = Me.Core.NewEcotracerScenario(strName, strDescription, strAuthor, strContact)
        cApplicationStatusNotifier.EndProgress(Me.Core)
        Return bSucces

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="es"></param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Private Function LoadEcotracerScenario(es As cEcotracerScenario) As Boolean

        Dim bSucces As Boolean = False

        If (es IsNot Nothing) Then
            ' #Yes: Load it
            cApplicationStatusNotifier.StartProgress(Me.Core, cStringUtils.Localize(My.Resources.STATUS_ECOTRACER_LOADING, es.Name))
            bSucces = Me.Core.LoadEcotracerScenario(es)
            Me.m_autosavemanager.ApplySettingsAndEnsureDefaults()
            cApplicationStatusNotifier.EndProgress(Me.Core)
        End If
        Return bSucces

    End Function

#End Region ' Ecotracer

#Region " Command handlers "

#Region " Generic commands "

    Private Sub OnFileOpen(cmd As cCommand) Handles m_cmdFileOpen.OnInvoke

        Dim dlgLoad As OpenFileDialog = Nothing
        Dim foc As cFileOpenCommand = DirectCast(cmd, cFileOpenCommand)
        Dim strPath As String = foc.Directory

        dlgLoad = cEwEFileDialogHelper.OpenFileDialog(foc.Title, foc.FileName, foc.Filters, foc.FilterIndex, strPath, foc.AllowMultiple)

        foc.Result = dlgLoad.ShowDialog()

        If (foc.Result = System.Windows.Forms.DialogResult.OK) Then
            foc.FileName = dlgLoad.FileName
            foc.FileNames = dlgLoad.FileNames
            foc.FilterIndex = dlgLoad.FilterIndex

            If (foc.AllowMultiple = False) Then
                foc.Directory = Path.GetDirectoryName(dlgLoad.FileName)
            End If
        End If

    End Sub

    Private Sub OnFileSave(cmd As cCommand) Handles m_cmdFileSave.OnInvoke

        Dim dlgSave As SaveFileDialog = Nothing
        Dim fsc As cFileSaveCommand = DirectCast(cmd, cFileSaveCommand)
        Dim strPath As String = fsc.Directory

        dlgSave = cEwEFileDialogHelper.SaveFileDialog(fsc.Title, fsc.FileName, fsc.Filters, fsc.FilterIndex, strPath)

        fsc.Result = dlgSave.ShowDialog()

        If (fsc.Result = System.Windows.Forms.DialogResult.OK) Then
            fsc.FileName = dlgSave.FileName
            fsc.FilterIndex = dlgSave.FilterIndex
            fsc.Directory = Path.GetDirectoryName(fsc.FileName)
        End If

    End Sub

    Private Sub OnDirectoryOpen(cmd As cCommand) Handles m_cmdDirectoryOpen.OnInvoke

        ' JS 19Nov13: Restored old path if something went wrong
        Dim doc As cDirectoryOpenCommand = Me.m_cmdDirectoryOpen
        Dim strPath As String = doc.Directory

        Try
            Dim dlgLoad As FolderBrowserDialog = Nothing
            dlgLoad = cEwEFileDialogHelper.FolderBrowserDialog(doc.Prompt, strPath)
            doc.Result = dlgLoad.ShowDialog()
            strPath = dlgLoad.SelectedPath
        Catch ex As Exception
            cLog.Write(ex, "OnDirectoryOpen")
        End Try

        If (doc.Result = System.Windows.Forms.DialogResult.OK) Then
            doc.Directory = strPath
        End If

    End Sub

    Private Sub OnPickColor(cmd As cCommand) Handles m_cmdPickColor.OnInvoke

        Try
            Dim dlg As New ColorDialog()
            dlg.Color = Me.m_cmdPickColor.Color
            dlg.AllowFullOpen = True
            dlg.AnyColor = True

            If (My.Settings.ColorCustom IsNot Nothing) Then
                dlg.CustomColors = CType(My.Settings.ColorCustom.ToArray(GetType(Integer)), Integer())
            End If

            Me.m_cmdPickColor.Result = dlg.ShowDialog(Me)

            If (Me.m_cmdPickColor.Result = System.Windows.Forms.DialogResult.OK) Then
                Me.m_cmdPickColor.Color = dlg.Color
                Dim al As New ArrayList()
                al.AddRange(dlg.CustomColors)
                My.Settings.ColorCustom = al
            End If

        Catch ex As Exception

        End Try

    End Sub

    Private Sub OnOpenDocument(cmd As cCommand) Handles m_cmdNavigate.OnInvoke

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

        ' Preserve properties from Nav command, because the content of the nav command may change in response to actions in this method
        strNavPageID = nc.PageID
        strNavPageName = nc.PageName
        strNavHelpURL = nc.HelpURL
        tNavClassType = nc.ClassType
        iNavCoreState = nc.CoreExecutionState

        If strNavPageID = "ndScenario" Then
            Me.m_coreController.LoadEcosimScenario()
            Return
        End If

        If strNavPageID = "ndEcospaceScenario" Then
            Me.m_coreController.LoadEcospaceScenario()
            Return
        End If

        If strNavPageID = "ndEcotracerScenario" Then
            Me.CoreController.LoadEcotracerScenario()
            Return
        End If

        ' Check if core can be brought up to par
        If Me.CoreController.LoadState(iNavCoreState) Then
            ' Is form already loaded?
            If Not Me.ActivateForm(strNavPageName) Then

                'cApplicationStatusNotifier.StartProgress(Me.Core)

                Try
                    ' Load instance of form for selected node
                    frm = Me.LoadFormFromType(strNavPageName, tNavClassType, iNavCoreState)
                    ' Was a form created?
                    If (frm IsNot Nothing) Then
                        ' #Yes
                        If frm.WindowState = FormWindowState.Minimized Then frm.WindowState = FormWindowState.Normal
                        ' Is this a dockable form? 
                        If (TypeOf frm Is DockContent) And (Me.m_DockPanel.DocumentStyle = DocumentStyle.DockingMdi) Then
                            ' #Yes: show the form in the dock panel
                            DirectCast(frm, DockContent).Show(Me.m_DockPanel, DockState.Document)
                        Else
                            ' #No: Just show the form
                            frm.MdiParent = Me
                            frm.Show()
                        End If
                        ' Switch help
                        Me.Help.HelpTopic(frm) = strNavHelpURL
                    Else
                        cLog.Write("Form cannot be resolved", "frmEwE6 cmdNavigate OnInvoke",)
                    End If
                Catch ex As Exception
                    ' Whoah!
                    cLog.Write(ex, "frmEwE6 cmdNavigate OnInvoke")
                End Try

                'cApplicationStatusNotifier.EndProgress(Me.Core)

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
    Private Sub OnCloseDocument(cmd As cCommand) Handles m_cmdCloseDocument.OnInvoke
        ' Is the window docked?
        ' Check whether an active document exists; this will occur when all panels are already closed.
        If Me.m_DockPanel.ActiveDocument IsNot Nothing Then
            ' Close active doc
            Me.m_DockPanel.ActiveDocument.DockHandler.Close()
        End If

    End Sub

    ''' <summary>
    ''' Command handler; update the 'close document' command state
    ''' </summary>
    Private Sub OnUpdateCloseDocument(cmd As cCommand) Handles m_cmdCloseDocument.OnUpdate, m_cmdCloseAllForms.OnUpdate
        cmd.Enabled = False
        ' Is the window docked?
        cmd.Enabled = Me.m_DockPanel.ActiveDocument IsNot Nothing
    End Sub

    ''' <summary>
    ''' Command handler; closes all closable child forms.
    ''' </summary>
    Private Sub OnCloseAllForms(cmd As cCommand) Handles m_cmdCloseAllForms.OnInvoke
        ' Close all child forms of the parent
        Me.CloseAllDocuments()
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Event handler, called when the MRU dropdown menu is about to open.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub OnMRUOpening(sender As System.Object, e As System.EventArgs) _
        Handles m_tsmiFileRecent.DropDownOpening
        Me.PopulateModelMRUDropdown()
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Event handler, called when the MRU dropdown menu has closed.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub OnMRUClosed(sender As System.Object, e As System.EventArgs) _
        Handles m_tsmiFileRecent.DropDownClosed
        ' Ok, do NOT do this here; the dropdown is closed BEFORE a MRU invoke is called. Lovely!
        'Me.ResetMRU()
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Event handler, called when the scenario dropdowns open. Implemented to
    ''' update item texts, in case scenario or time series names have changed.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub OnScenarioDropdownOpening(sender As Object, args As Object) _
        Handles m_tsbEcosim.DropDownOpening, m_tsbEcospace.DropDownOpening, m_tsbEcotracer.DropDownOpening

        If (TypeOf sender Is ToolStripDropDownItem) Then
            Dim dd As ToolStripDropDownItem = DirectCast(sender, ToolStripDropDownItem)
            Dim fmt As New cCoreInterfaceFormatter()
            For Each tsi As ToolStripItem In dd.DropDownItems
                If (tsi.Tag IsNot Nothing) Then
                    If (TypeOf tsi.Tag Is cCoreInputOutputBase) Then
                        tsi.Text = fmt.ToString(DirectCast(tsi.Tag, cCoreInputOutputBase))
                    End If
                End If
            Next
        End If

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Event handler, called when the Exit menu item is selected.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub OnExit(sender As System.Object, e As System.EventArgs) _
        Handles m_tsmiFileExit.Click
        Me.Close()
    End Sub

#End Region ' Generic commands

#Region " File menu commands "

    ''' <summary>
    ''' Create new Ecopath model
    ''' </summary>
    Private Sub OnNewModel(cmd As cCommand) Handles m_cmdNewModel.OnInvoke

        Dim db As cEwEDatabase = Nothing
        Dim cmdh As cCommandHandler = Me.UIContext.CommandHandler
        Dim cmdFS As cFileSaveCommand = DirectCast(cmdh.GetCommand(cFileSaveCommand.COMMAND_NAME), cFileSaveCommand)

        cmdFS.Invoke(SharedResources.DEFAULT_NEWMODELNAME, SharedResources.FILEFILTER_MODEL_SAVE, 1)

        If (cmdFS.Result = System.Windows.Forms.DialogResult.OK) Then
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
    Private Sub OnUpdateNewModel(cmd As cCommand) Handles m_cmdNewModel.OnUpdate
        cmd.Enabled = True
    End Sub

    ''' <summary>
    ''' Open Ecopath model from a given location
    ''' </summary>
    Private Sub OnLoadModel(cmd As cCommand) Handles m_cmdLoadModel.OnInvoke

        Dim cmdh As cCommandHandler = Me.UIContext.CommandHandler
        Dim cmdFO As cFileOpenCommand = DirectCast(cmdh.GetCommand(cFileOpenCommand.COMMAND_NAME), cFileOpenCommand)
        Dim strFilter As String = SharedResources.FILEFILTER_MODEL_OPEN

        If String.IsNullOrWhiteSpace(cmdFO.Directory) Then
            cmdFO.Directory = My.Settings.LastSelectedDirectory
        End If

        If (cmd.Tag IsNot Nothing) Then
            cmdFO.Invoke(CStr(cmd.Tag), strFilter, 1)
        Else
            cmdFO.Invoke(strFilter, 1)
        End If

        If (cmdFO.Result = DialogResult.OK) Then

            ' Open the model
            cApplicationStatusNotifier.StartProgress(Me.Core, My.Resources.STATUS_ECOPATH_LOADING)
            Me.LoadEcopathModel(cmdFO.FileName, eLoadSourceType.User)
            cApplicationStatusNotifier.EndProgress(Me.Core)

        End If

    End Sub

    ''' <summary>
    ''' Update Load Ecopath model command state
    ''' </summary>
    Private Sub OnUpdateLoadModel(cmd As cCommand) Handles m_cmdLoadModel.OnUpdate
        cmd.Enabled = Not Me.Core.StateMonitor.IsBusy
    End Sub

    ''' <summary>
    ''' Save the model
    ''' </summary>
    Private Sub OnSave(cmd As cCommand) Handles m_cmdSave.OnInvoke
        cApplicationStatusNotifier.StartProgress(Me.Core, My.Resources.STATUS_MODEL_SAVING)
        Try
            Me.Core.Save()
            Me.SaveSettings()
        Catch ex As Exception
            ' Whoah!
        End Try
        cApplicationStatusNotifier.EndProgress(Me.Core)
    End Sub

    ''' <summary>
    ''' Update save model command state
    ''' </summary>
    Private Sub OnUpdateSave(cmd As cCommand) Handles m_cmdSave.OnUpdate
        cmd.Enabled = Me.Core.StateMonitor.IsModified And Not Me.Core.StateMonitor.IsBusy
    End Sub

    ''' <summary>
    ''' Save model under a different name
    ''' </summary>
    Private Sub OnSaveModelAs(cmd As cCommand) Handles m_cmdSaveModelAs.OnInvoke

        Dim cmdh As cCommandHandler = Me.UIContext.CommandHandler
        Dim cmdFS As cFileSaveCommand = DirectCast(cmdh.GetCommand(cFileSaveCommand.COMMAND_NAME), cFileSaveCommand)

        Dim strFileFilter As String = ""

        ' JS 27Jul08: Only able to save in current file format (save as between formats not supported by the core)
        Select Case cDataSourceFactory.GetSupportedType(Me.SelectedFileName)
            Case eDataSourceTypes.Access2003
                ' Only allow saving as MDB
                strFileFilter = SharedResources.FILEFILTER_SAVE_MDB
            Case eDataSourceTypes.Access2007
                ' Only allow saving as ACCDB
                strFileFilter = SharedResources.FILEFILTER_SAVE_ACCDB
            Case Else
                ' Not supported
                Debug.Assert(False, "Option should not have been available")
                Return
        End Select

        ' Special case: invoke save model command on last used model path
        If (String.IsNullOrWhiteSpace(cmdFS.Directory)) Then
            cmdFS.Directory = My.Settings.LastSelectedDirectory
        End If
        cmdFS.Invoke(SharedResources.DEFAULT_NEWMODELNAME, strFileFilter)

        If (cmdFS.Result = System.Windows.Forms.DialogResult.OK) Then

            ' Save the model
            cApplicationStatusNotifier.StartProgress(Me.Core, My.Resources.STATUS_MODEL_SAVING)
            Try
                Me.SaveEcopathModelAs(cmdFS.FileName)
            Catch ex As Exception

            End Try
            cApplicationStatusNotifier.EndProgress(Me.Core)

        End If

    End Sub

    ''' <summary>
    ''' Update save model command state
    ''' </summary>
    Private Sub OnUpdateSaveModelAs(cmd As cCommand) Handles m_cmdSaveModelAs.OnUpdate

        Dim bEnable As Boolean = Me.Core.StateMonitor.HasEcopathLoaded

        Select Case cDataSourceFactory.GetSupportedType(Me.SelectedFileName)
            Case eDataSourceTypes.Access2003, eDataSourceTypes.Access2007
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
    Private Sub OnCloseModel(cmd As cCommand) Handles m_cmdCloseModel.OnInvoke
        Me.CloseEcopathModel()
    End Sub

    ''' <summary>
    ''' Update close model command state
    ''' </summary>
    Private Sub OnUpdateCloseModel(cmd As cCommand) Handles m_cmdCloseModel.OnUpdate
        Dim m As cCoreStateMonitor = Me.Core.StateMonitor
        cmd.Enabled = m.HasEcopathLoaded And Not m.IsBusy
    End Sub

    ''' <summary>
    ''' Compact a model as requested by the user
    ''' </summary>
    Private Sub OnCompactModel(cmd As cCommand) Handles m_cmdCompactModel.OnInvoke

        Dim strFilename As String = Me.SelectedFileName
        Dim result As eDatasourceAccessType = eDatasourceAccessType.Success
        Dim strMessage As String = ""
        Dim bSuccess As Boolean = True

        If (Me.AskFeedback(My.Resources.PROMPT_MODEL_COMPACT) <> eMessageReply.YES) Then Return
        If Me.CloseEcopathModel() = False Then Return

        result = Me.CompactModel(strFilename)

        If result = eDatasourceAccessType.Success Then
            bSuccess = Me.LoadEcopathModel(strFilename, eLoadSourceType.API)
            If bSuccess Then
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
                Case eDatasourceAccessType.Failed_FileNotFound,
                     eDatasourceAccessType.Failed_Unknown
                    strMessage = My.Resources.STATUS_MODEL_COMPACTING_FAILED
                Case eDatasourceAccessType.Failed_ReadOnly
                    strMessage = My.Resources.STATUS_MODEL_ACCESS_READONLY
            End Select
            bSuccess = False
        End If

        If (bSuccess) Then
            Me.SendMessage(strMessage, eMessageImportance.Information, eCoreComponentType.DataSource)
        Else
            Me.SendMessage(strMessage, eMessageImportance.Critical, eCoreComponentType.DataSource)
        End If

    End Sub

    ''' <summary>
    ''' Update compact model command state
    ''' </summary>
    Private Sub OnUpdateCompactModel(cmd As cCommand) Handles m_cmdCompactModel.OnUpdate
        Dim ds As IEwEDataSource = Me.Core.DataSource
        If (ds Is Nothing) Then
            cmd.Enabled = False
        Else
            cmd.Enabled = (Me.Core.StateMonitor.HasEcopathLoaded) And ds.CanCompact(Me.SelectedFileName)
        End If
    End Sub

    ''' <summary>
    ''' Open the output file location
    ''' </summary>
    Private Sub OnOpenOutputLocation(cmd As cCommand) Handles m_cmdOpenOutput.OnInvoke
        Try
            Process.Start("explorer.exe", Me.Core.OutputPath)
        Catch ex As Exception

        End Try
    End Sub

    Private Sub OnPrintInvoke(cmd As cCommand) Handles m_cmdPrint.OnInvoke

        Dim dlg As New PrintPreviewDialog()
        Dim cnt As IDockContent = Me.m_DockPanel.ActiveDocument

        If (TypeOf cnt Is frmEwE) Then
            Dim frm As frmEwE = DirectCast(cnt, frmEwE)
            dlg.Document = frm.BeginPrint
            dlg.ShowDialog()
            frm.EndPrint()
        End If

    End Sub

    Private Sub OnPrintEnable(cmd As cCommand) Handles m_cmdPrint.OnUpdate

        Dim cnt As IDockContent = Me.m_DockPanel.ActiveDocument
        Dim bEnable As Boolean = False

        If (cnt IsNot Nothing) Then
            bEnable = (TypeOf cnt Is frmEwE)
        End If

        cmd.Enabled = bEnable

    End Sub

    Private Sub OnEcobaseImportInvoke(cmd As cCommand) Handles m_cmdEcobaseImport.OnInvoke

        Dim strModel As String = ""

        If (String.IsNullOrWhiteSpace(strModel)) Then
            Dim frm As New dlgEcobaseImport(Me.UIContext)
            If (frm.ShowDialog() = DialogResult.OK) Then
                Dim model As EwECore.WebServices.Ecobase.cModelData = frm.SelectedModel
                strModel = "ewe-ecobase:" & model.EcobaseCode
            End If
        End If

        If (Not String.IsNullOrWhiteSpace(strModel)) Then
            Me.LoadEcopathModel(strModel, eLoadSourceType.User)
        End If

    End Sub

    Private Sub OnEcobaseImportEnable(cmd As cCommand) Handles m_cmdEcobaseImport.OnUpdate
        cmd.Enabled = Not Me.Core.StateMonitor.IsBusy
    End Sub

    Private Sub OnEcobaseExportInvoke(cmd As cCommand) _
        Handles m_cmdEcobaseExport.OnInvoke

        Try
            Me.m_coreController.LoadState(eCoreExecutionState.EcopathCompleted)

            ' Ecopath must run ok
            If (Not Me.Core.IsModelBalanced()) Then
                Me.SendMessage(My.Resources.ECOBASE_ERROR_BALANCE)
                Return
            End If

            ' All pending changes must be saved prior to this
            If (Not Me.Core.SaveChanges()) Then Return

            ' Export
            Dim dlg As New dlgEcobaseExport(Me.UIContext)
            dlg.ShowDialog(Me)

        Catch ex As Exception

        End Try

    End Sub

    Private Sub OnEcobaseExportEnable(cmd As cCommand) Handles m_cmdEcobaseExport.OnUpdate
        cmd.Enabled = Not Me.Core.StateMonitor.IsBusy And Me.Core.StateMonitor.HasEcopathLoaded
    End Sub

    Private Sub OnEIIXMLExportInvoke(cmd As cCommand) _
        Handles m_cmdEIIXMLExport.OnInvoke

        Dim ds As IEwEDataSource = Me.Core.DataSource
        If Not (TypeOf ds Is cDBDataSource) Then Return
        Dim dbds As cDBDataSource = DirectCast(ds, cDBDataSource)
        If Not (TypeOf dbds.Connection Is cEwEAccessDatabase) Then Return
        Dim db As cEwEAccessDatabase = DirectCast(dbds.Connection, cEwEAccessDatabase)
        Dim msg As cMessage = Nothing

        If Not Me.Core.SaveChanges(False) Then Return

        Try
            Dim strFileName As String = Path.ChangeExtension(dbds.ToString, ".eiixml")

            If (File.Exists(strFileName)) Then
                Dim fmsg As New cFeedbackMessage(cStringUtils.Localize(My.Resources.GENERIC_PROMPT_OVERWRITEFILE, strFileName),
                                                 eCoreComponentType.DataSource, eMessageType.DataValidation,
                                                 eMessageImportance.Question, eMessageReplyStyle.YES_NO)
                fmsg.Reply = eMessageReply.NO
                Me.Core.Messages.SendMessage(fmsg)
                If fmsg.Reply = eMessageReply.NO Then Return
            End If

            ds = cDataSourceFactory.Create(eDataSourceTypes.EIIXML)
            If DirectCast(ds, cEIIXMLDataSource).SaveFromDB(db, strFileName) Then
                msg = New cMessage(cStringUtils.Localize(My.Resources.STATUS_EXPORT_SUCCESS, strFileName),
                                   eMessageType.DataExport, eCoreComponentType.External, eMessageImportance.Information)
                msg.Hyperlink = Path.GetDirectoryName(strFileName)
            Else
                msg = New cMessage(cStringUtils.Localize(My.Resources.STATUS_EXPORT_FAILURE, strFileName),
                   eMessageType.DataExport, eCoreComponentType.External, eMessageImportance.Information)
            End If
            Me.Core.Messages.SendMessage(msg)
            ds.Close()

        Catch ex As Exception

        End Try

    End Sub

    Private Sub OnEIIXMLExportEnable(cmd As cCommand) Handles m_cmdEIIXMLExport.OnUpdate

        Dim bEnabled As Boolean = False

        If (Not Me.Core.StateMonitor.IsBusy) Then
            If (Me.Core.DataSource IsNot Nothing) Then
                If (TypeOf Me.Core.DataSource Is cDBDataSource) Then
                    Dim dbds As cDBDataSource = DirectCast(Me.Core.DataSource, cDBDataSource)
                    bEnabled = (TypeOf dbds.Connection Is cEwEAccessDatabase)
                End If
            End If
        End If

        cmd.Enabled = bEnabled

    End Sub

#End Region ' File commands

#Region " View commands "

    ''' <summary>
    ''' Command handler; toggles presentation mode
    ''' </summary>
    Private Sub OnViewPresentationMode(cmd As cCommand) Handles m_cmdViewPresentationMode.OnInvoke

        Me.m_presentationmode.TogglePresentationMode()

    End Sub

    ''' <summary>
    ''' Command update handler; enables and disables the 
    ''' <see cref="m_cmdViewPresentationMode">View Presentation Mode command</see>.
    ''' </summary>
    Private Sub OnUpdateViewPresentationMode(cmd As cCommand) _
        Handles m_cmdViewPresentationMode.OnUpdate
        Me.m_cmdViewPresentationMode.Checked = Me.m_presentationmode.IsPresentationModeActive
    End Sub

    ''' <summary>
    ''' Command handler; toggles main statusbar visibility
    ''' </summary>
    Private Sub OnViewMainStatusbar(cmd As cCommand) Handles m_cmdViewStatusbar.OnInvoke
        Me.m_ssMain.Visible = Not cmd.Checked
    End Sub

    ''' <summary>
    ''' Command update handler; enables and disables the <see cref="m_cmdViewStatusbar">View Statusbar command</see>.
    ''' </summary>
    Private Sub OnUpdateViewMainStatusbar(cmd As cCommand) Handles m_cmdViewStatusbar.OnUpdate
        cmd.Checked = Me.m_ssMain.Visible
    End Sub

    ''' <summary>
    ''' Command handler; toggles main menu visibility
    ''' </summary>
    Private Sub OnViewMenu(cmd As cCommand) Handles m_cmdViewMenu.OnInvoke
        Me.m_menuMain.Visible = Not cmd.Checked
    End Sub

    ''' <summary>
    ''' Command update handler; enables and disables the <see cref="m_cmdViewMenu">View menu command</see>.
    ''' </summary>
    Private Sub OnUpdateViewMenu(cmd As cCommand) Handles m_cmdViewMenu.OnUpdate
        cmd.Checked = Me.m_menuMain.Visible
    End Sub

    ''' <summary>
    ''' Command handler; toggles auto save results
    ''' </summary>
    Private Sub OnAutosaveResults(cmd As cCommand) Handles m_cmdAutosaveConfig.OnInvoke
        Me.m_cmdShowOptions.Invoke(eApplicationOptionTypes.Autosave)
    End Sub

    ''' <summary>
    ''' Command update handler; enables and disables the <see cref="m_cmdAutosaveConfig">Auto save results command</see>.
    ''' </summary>
    Private Sub OnUpdateAutosaveResults(cmd As cCommand) Handles m_cmdAutosaveConfig.OnUpdate
        ' Check if any autosave option set
        Dim bAutoSaving As Boolean = False
        Dim nodes As eAutosaveTypes() = New eAutosaveTypes() {eAutosaveTypes.Ecopath, eAutosaveTypes.Ecosim, eAutosaveTypes.Ecospace}
        For Each setting As eAutosaveTypes In [Enum].GetValues(GetType(eAutosaveTypes))
            ' Exclude nodes
            If Me.Core.Autosave(setting) And Array.IndexOf(nodes, setting) = -1 Then
                bAutoSaving = True
                Exit For
            End If
        Next
        If (Me.m_pluginManager IsNot Nothing) And Not bAutoSaving Then
            For Each pi As IAutoSavePlugin In Me.m_pluginManager.GetPlugins(GetType(IAutoSavePlugin))
                If pi.AutoSave Then
                    bAutoSaving = True
                    Exit For
                End If
            Next
        End If
        cmd.Checked = bAutoSaving
    End Sub

    ''' <summary>
    ''' Command handler; toggles auto run results
    ''' </summary>
    Private Sub OnAutorunConfig(cmd As cCommand) Handles m_cmdAutorunConfig.OnInvoke
        Me.m_cmdShowOptions.Invoke(eApplicationOptionTypes.AutoRun)
    End Sub

    ''' <summary>
    ''' Command update handler; enables and disables the <see cref="m_cmdAutorunConfig">Auto run results command</see>.
    ''' </summary>
    Private Sub OnUpdateAutorunConfig(cmd As cCommand) Handles m_cmdAutorunConfig.OnUpdate
        ' Check if any autosave option set
        Dim bAutoRunning As Boolean = False
        If (Me.m_pluginManager IsNot Nothing) Then
            For Each pi As IAutoRunPlugin In Me.m_pluginManager.GetPlugins(GetType(IAutoRunPlugin))
                For Each comp As eCoreComponentType In pi.AutoRunTypes
                    If pi.AutoRun(comp) Then
                        bAutoRunning = True
                        Exit For
                    End If
                Next
            Next
        End If
        cmd.Checked = bAutoRunning
    End Sub

    ''' <summary>
    ''' Command handler; shows the start page.
    ''' </summary>
    Private Sub OnBrowseURI(cmd As cCommand) Handles m_cmdBrowseURI.OnInvoke

        Dim bcmd As cBrowserCommand = DirectCast(cmd, cBrowserCommand)
        Dim strURL As String = bcmd.URL(New cWebLinks(Me.Core))

        ' Is a hyperlink?
        If cUriBuilder.IsValidURI(strURL) Or String.IsNullOrWhiteSpace(strURL) Then
            Try
                ' Fire off system default URL handling
                System.Diagnostics.Process.Start(strURL)
            Catch ex As Exception
                ' Failed to launch
                Dim msg As New cMessage(cStringUtils.Localize(My.Resources.PROMPT_SHELL_FAILURE, ex.Message),
                                        eMessageType.DataExport, eCoreComponentType.External, eMessageImportance.Warning)
                Me.Core.Messages.SendMessage(msg)
            End Try

        ElseIf cStringUtils.BeginsWith(strURL, "command:", True) Then
            ' #No: Is command?
            Dim strCommand As String = strURL.Substring(8)
            Dim bits() As String = strCommand.Split("?"c)
            ' Get command name
            cmd = Me.UIContext.CommandHandler.GetCommand(bits(0))
            ' Did this work?
            If (cmd IsNot Nothing) Then
                If (bits.Count = 2) Then
                    For Each pv As String In bits(1).Split("&"c)
                        Dim parm() As String = pv.Split("="c)
                        If (parm.Count = 2) Then
                            cmd.Parameter(parm(0)) = parm(1)
                        End If
                    Next
                End If
                cmd.Invoke()
            End If
        ElseIf cStringUtils.BeginsWith(strURL, "ewe-ecobase:", True) Then
            ' #No: Is ecobase link?
            Me.LoadEcopathModel(strURL, eLoadSourceType.User)
        Else
            ' #No: presume we're talking files here. Let the OS deal with it
            Try
                ' JS 10Jan15: Do not even use explorer; just use default protocol handlers
                'Process.Start("explorer.exe", strURL)
                System.Diagnostics.Process.Start(strURL)
            Catch ex As Exception
                ' Failed to launch
                Dim msg As New cMessage(cStringUtils.Localize(My.Resources.PROMPT_SHELL_FAILURE, ex.Message),
                                        eMessageType.DataExport, eCoreComponentType.External, eMessageImportance.Warning)
                Me.Core.Messages.SendMessage(msg)
            End Try
        End If

    End Sub

    ''' <summary>
    ''' Command handler; shows the navigation panel.
    ''' </summary>
    Private Sub OnViewNavPane(cmd As cCommand) Handles m_cmdViewNavPane.OnInvoke
        If cmd.Checked Then
            Me.Panel(cPANEL_NAV).DockState = DockState.Hidden
        Else
            Me.Panel(cPANEL_NAV).Show(Me.m_DockPanel, DockState.DockLeft)
        End If
    End Sub

    ''' <summary>
    ''' Command update handler; manages the <see cref="m_cmdViewNavPane">View Navigation Panel command</see> state.
    ''' </summary>
    Private Sub OnUpdateViewNavPane(cmd As cCommand) Handles m_cmdViewNavPane.OnUpdate
        cmd.Checked = (Me.Panel(cPANEL_NAV).DockState <> DockState.Hidden)
    End Sub

    ''' <summary>
    ''' Show the remark pane
    ''' </summary>
    Private Sub OnViewRemarkPane(cmd As cCommand) Handles m_cmdViewRemarkPane.OnInvoke
        If cmd.Checked Then
            Me.Panel(cPANEL_REMARKS).DockState = DockState.Hidden
        Else
            Me.Panel(cPANEL_REMARKS).Show(Me.m_DockPanel, DockState.DockBottomAutoHide)
        End If
    End Sub

    Private Sub OnUpdateViewRemarkPane(cmd As cCommand) Handles m_cmdViewRemarkPane.OnUpdate
        cmd.Checked = (Me.Panel(cPANEL_REMARKS).DockState <> DockState.Hidden)
    End Sub

    ''' <summary>
    ''' Show the status panel
    ''' </summary>
    Private Sub OnViewStatusPane(cmd As cCommand) Handles m_cmdViewStatusPane.OnInvoke
        If cmd.Checked Then
            Me.Panel(cPANEL_STATUS).DockState = DockState.Hidden
        Else
            Me.Panel(cPANEL_STATUS).Show(Me.m_DockPanel, DockState.DockBottomAutoHide)
        End If
    End Sub

    Private Sub OnUpdateViewStatusPane(cmd As cCommand) Handles m_cmdViewStatusPane.OnUpdate
        cmd.Checked = (Me.Panel(cPANEL_STATUS).DockState <> DockState.Hidden)
    End Sub

    ''' <summary>
    ''' Show the button bar
    ''' </summary>
    Private Sub OnViewModelBar(cmd As cCommand) Handles m_cmdViewModelBar.OnInvoke
        Me.m_tsModel.Visible = Not cmd.Checked
    End Sub

    Private Sub OnUpdateViewModelBar(cmd As cCommand) Handles m_cmdViewModelBar.OnUpdate
        cmd.Checked = Me.m_tsModel.Visible
    End Sub

#End Region ' View commands

#Region " Tools commands "

    Private Sub OnShowOptions(cmd As cCommand) Handles m_cmdShowOptions.OnInvoke
        Try
            Dim dlgOptions As New dlgOptions(Me.UIContext, Me.m_cmdShowOptions.Verb)
            cmd.UserHandled = (dlgOptions.ShowDialog(Me) = System.Windows.Forms.DialogResult.OK)
            Me.SaveSettings()
        Catch ex As Exception

        End Try
    End Sub

    Private Sub OnShowTools(cmd As cCommand) Handles m_cmdShowTools.OnInvoke
        Try
            Dim strPath As String = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "Tools")
            Me.m_cmdBrowseURI.Invoke(strPath)
        Catch ex As Exception

        End Try
    End Sub

    Private Sub OnEditRefMap(cmd As cCommand) Handles m_cmdEditReferenceMap.OnInvoke
        Me.m_cmdShowOptions.Invoke(eApplicationOptionTypes.ReferenceMaps)
    End Sub

#End Region ' Tools commands

#Region " Help commands "

    ''' <summary>
    ''' Command handler; invokes the About... dialog.
    ''' </summary>
    Private Sub OnShowAboutDialog(cmd As cCommand) Handles m_cmdHelpAbout.OnInvoke
        Dim dlgAbout As New frmAboutEwE(Me.UIContext)
        Me.Help.HelpTopic(dlgAbout) = ""
        dlgAbout.ShowDialog(Me)
    End Sub

    Private Sub OnHelpTOC(sender As System.Object, e As System.EventArgs) Handles m_tsmiHelpContents.Click
        Me.Help.ShowHelp(HelpNavigator.TableOfContents)
    End Sub

    Private Sub OnHelpIndex(sender As System.Object, e As System.EventArgs) Handles m_tsmiHelpIndex.Click
        Me.Help.ShowHelp(HelpNavigator.KeywordIndex)
    End Sub

    Private Sub OnHelpSearch(sender As System.Object, e As System.EventArgs) Handles m_tsmiHelpSearch.Click
        Me.Help.ShowHelp(HelpNavigator.Find)
    End Sub

    Private Sub OnReportBug(cmd As cCommand) Handles m_cmdHelpReportIssue.OnInvoke
        Try
            Dim strReport As String = cBugReporter.BugReport(My.Resources.GENERIC_CAPTION, "ewedevteam@gmail.com", Me.m_pluginManager)
            Me.m_cmdBrowseURI.Invoke(strReport)
        Catch ex As Exception

        End Try
    End Sub

    Private Sub OnRequestSourceCodeAccess(cmd As cCommand) Handles m_cmdHelpRequestCodeAccess.OnInvoke
        Try
            Me.m_cmdBrowseURI.Invoke("mailto:ewedevteam@gmail.com?subject=Request source code access")
        Catch ex As Exception

        End Try
    End Sub

    Private Sub OnProvideFeedback(cmd As cCommand) Handles m_cmdHelpFeedback.OnInvoke
        ' Survey monkey URL
        Me.m_cmdBrowseURI.Invoke(cWebLinks.eLinkType.Feedback)
    End Sub

    Private Sub m_tsmiHelpViewMainSite_Click(sender As System.Object, e As System.EventArgs) Handles m_tsmiHelpViewMainSite.Click
        Me.m_cmdBrowseURI.Invoke(cWebLinks.eLinkType.Home)
    End Sub

    Private Sub m_tsmiHelpViewEcobase_Click(sender As System.Object, e As System.EventArgs) Handles m_tsmiHelpViewEcobase.Click
        Me.m_cmdBrowseURI.Invoke(cWebLinks.eLinkType.EcoBase)
    End Sub

    Private Sub m_tsmiHelpViewFacebook_Click(sender As System.Object, e As System.EventArgs) Handles m_tsmiHelpViewFacebook.Click
        Me.m_cmdBrowseURI.Invoke(cWebLinks.eLinkType.Facebook)
    End Sub

    Private Sub m_tsmiHelpViewReports_Click(sender As System.Object, e As System.EventArgs) Handles m_tsmiHelpViewReports.Click
        Me.m_cmdBrowseURI.Invoke(cWebLinks.eLinkType.Trac)
    End Sub

    Private Sub m_tsmiViewLog_Click(sender As System.Object, e As System.EventArgs) Handles m_tsmiViewLog.Click
        Me.m_cmdBrowseURI.Invoke(cLog.LogFile)
    End Sub

#End Region ' Main Menu - Help

#Region " Ecopath commands "

    ''' <summary>
    ''' Command handler; invokes the edit groups interface
    ''' </summary>
    Private Sub OnEditGroups(cmd As cCommand) Handles m_cmdEditGroups.OnInvoke
        Dim dlg As New dlgDefineGroups(Me.UIContext, DirectCast(cmd.Tag, cEcoPathGroupInput))
        Me.Help.HelpTopic(dlg) = "Edit groups.htm"
        dlg.ShowDialog(Me)
    End Sub

    ''' <summary>
    ''' Command update handler; enables and disables the <see cref="m_cmdEditGroups">Edit Groups command</see>.
    ''' </summary>
    Private Sub OnUpdateEditGroups(cmd As cCommand) Handles m_cmdEditGroups.OnUpdate
        Dim m As cCoreStateMonitor = Me.Core.StateMonitor
        cmd.Enabled = m.HasEcopathLoaded() And Not m.IsBusy
    End Sub

    ''' <summary>
    ''' Command handler; invokes the edit multi stanza interface
    ''' </summary>
    Private Sub OnEditMultiStanza(cmd As cCommand) Handles m_cmdEditMultiStanza.OnInvoke

        ' Test if all stanza groups have at least one life stage
        Dim vars As New List(Of cVariableStatus)

        For i As Integer = 0 To Me.Core.nStanzas - 1
            If (Me.Core.StanzaGroups(i).nLifeStages = 0) Then
                vars.Add(New cVariableStatus(eStatusFlags.MissingParameter, cStringUtils.Localize(My.Resources.PROMPT_STANZA_MISSING_LIFESTAGES_DETAIL, Me.Core.StanzaGroups(i).Name),
                                             eVarNameFlags.NotSet, eDataTypes.Stanza, eCoreComponentType.Core, 0))
            End If
        Next

        If (vars.Count > 0) Then
            If Me.AskFeedback(My.Resources.PROMPT_STANZA_MISSING_LIFESTAGES,
                              eMessageImportance.Warning, eCoreComponentType.Core,
                              eMessageReplyStyle.YES_NO, vars:=vars.ToArray()) = eMessageReply.YES Then
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
    Private Sub OnUpdateMultiStanza(cmd As cCommand) Handles m_cmdEditMultiStanza.OnUpdate
        ' MultiStanza can be edited when ecopath has loaded and the core has more than one stanza group
        cmd.Enabled = (Me.Core.StateMonitor.HasEcopathLoaded() = True) And
                      (Me.Core.nStanzas > 0) And
                      (Not Me.Core.StateMonitor.IsBusy)
    End Sub

    ''' <summary>
    ''' Command handler; invokes the edit fleets interface
    ''' </summary>
    Private Sub OnEditFleets(cmd As cCommand) Handles m_cmdEditFleets.OnInvoke
        Try
            Dim dlg As New EditFleets(Me.UIContext, DirectCast(cmd.Tag, cEcopathFleetInput))
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
    Private Sub OnUpdateEditFleets(cmd As cCommand) _
        Handles m_cmdEditFleets.OnUpdate
        Dim m As cCoreStateMonitor = Me.Core.StateMonitor
        cmd.Enabled = m.HasEcopathLoaded() And Not m.IsBusy
    End Sub

    Private Sub OnEditPedigreeLevels(cmd As cCommand) _
        Handles m_cmdEditPedigree.OnInvoke
        Try
            Dim dlg As New dlgEditPedigree(Me.UIContext, DirectCast(cmd, cEditPedigreeCommand).Variable)
            dlg.ShowDialog(Me)
        Catch ex As Exception
            cLog.Write(ex, "frmEwE6::OnEditPedigreeLevels")
        End Try
    End Sub

    Private Sub OnUpdateEditPedigreeLevels(cmd As cCommand) _
        Handles m_cmdEditPedigree.OnUpdate
        Dim m As cCoreStateMonitor = Me.Core.StateMonitor
        cmd.Enabled = m.HasEcopathLoaded() And Not m.IsBusy
    End Sub

    Private Sub OnEditTaxonomy(cmd As cCommand) _
        Handles m_cmdEditTaxonomy.OnInvoke
        Dim dlg As New dlgDefineTaxonomy(Me.UIContext)
        dlg.ShowDialog(Me)
    End Sub

    Private Sub OnUpdateEditTaxonomy(cmd As cCommand) _
        Handles m_cmdEditTaxonomy.OnUpdate
        Dim m As cCoreStateMonitor = Me.Core.StateMonitor
        cmd.Enabled = m.HasEcopathLoaded() And Not m.IsBusy
    End Sub

    Private Sub OnDisplayShowHideItems(cmd As cCommand) _
        Handles m_cmdShowHideItems.OnInvoke
        Dim dlg As New dlgShowHideItems(Me.UIContext)
        dlg.ShowDialog()
        cmd.Checked = Me.UIContext.StyleGuide.HasHiddenItems()
    End Sub

    Private Sub OnUpdateShowHideItems(cmd As cCommand) _
        Handles m_cmdShowHideItems.OnUpdate
        cmd.Enabled = Me.Core.StateMonitor.HasEcopathLoaded()
    End Sub

#End Region ' Main Menu - File

#Region " Ecosim commands "

    ''' <summary>
    ''' Command handler; creates a new Ecosim scenario
    ''' </summary>
    Private Sub OnNewEcosimScenario(cmd As cCommand) Handles m_cmdNewEcosimScenario.OnInvoke

        Dim dlg As New EcosimScenarioDlg(Me.UIContext, EcosimScenarioDlg.eDialogModeType.CreateScenario)

        If dlg.ShowDialog = System.Windows.Forms.DialogResult.OK Then

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
    Private Sub OnUpdateNewEcosimScenario(cmd As cCommand) Handles m_cmdNewEcosimScenario.OnUpdate
        Dim m As cCoreStateMonitor = Me.Core.StateMonitor
        cmd.Enabled = m.HasEcopathLoaded And Not m.IsBusy
    End Sub

    ''' <summary>
    ''' Command handler; loads a new Ecosim scenario
    ''' </summary>
    Private Sub OnLoadEcosimScenario(cmd As cCommand) Handles m_cmdLoadEcosimScenario.OnInvoke
        Me.CoreController.LoadEcosimScenario()
    End Sub

    ''' <summary>
    ''' Command update handler; takes care of enabling and disabling the 
    ''' <see cref="m_cmdLoadEcosimScenario">Load Ecosim Scenario</see> command.
    ''' </summary>
    Private Sub OnUpdateLoadEcosimScenario(cmd As cCommand) Handles m_cmdLoadEcosimScenario.OnUpdate
        Dim m As cCoreStateMonitor = Me.Core.StateMonitor
        cmd.Enabled = m.HasEcopathLoaded And Not m.IsBusy
    End Sub

    ''' <summary>
    ''' Command handler; closes the current Ecosim scenario
    ''' </summary>
    Private Sub OnCloseEcosimScenario(cmd As cCommand) Handles m_cmdCloseEcosimScenario.OnInvoke
        Me.m_autosavemanager.GatherSettings()
        Me.Core.CloseEcosimScenario()
    End Sub

    ''' <summary>
    ''' Command update handler; takes care of enabling and disabling the 
    ''' <see cref="m_cmdCloseEcosimScenario">Close Ecosim Scenario</see> command.
    ''' </summary>
    Private Sub OnUpdateCloseEcosimScenario(cmd As cCommand) Handles m_cmdCloseEcosimScenario.OnUpdate
        Dim m As cCoreStateMonitor = Me.Core.StateMonitor
        cmd.Enabled = m.HasEcosimLoaded And Not m.IsBusy
    End Sub

    ''' <summary>
    ''' Command handler; saves an Ecosim scenario to a new name
    ''' </summary>
    Private Sub OnSaveEcosimScenarioAs(cmd As cCommand) Handles m_cmdSaveEcosimScenarioAs.OnInvoke

        Dim dlg As New EcosimScenarioDlg(Me.UIContext, EcosimScenarioDlg.eDialogModeType.SaveScenario,
                Me.Core.EcosimScenarios(Me.Core.ActiveEcosimScenarioIndex))

        If dlg.ShowDialog() = System.Windows.Forms.DialogResult.OK Then
            ' Overwriting?
            If dlg.Scenario IsNot Nothing Then
                ' #Yes: prompt for overwrite confirmation
                Dim fmsg As New cFeedbackMessage(cStringUtils.Localize(My.Resources.SCENARIO_CONFIRMOVERWRITE_PROMPT, dlg.ScenarioName), eCoreComponentType.Core, eMessageType.Any, eMessageImportance.Question, eMessageReplyStyle.YES_NO)
                Me.Core.Messages.SendMessage(fmsg)

                If (fmsg.Reply = eMessageReply.YES) Then
                    ' #Overwrite
                    cApplicationStatusNotifier.StartProgress(Me.Core, cStringUtils.Localize(My.Resources.STATUS_ECOSIM_SAVING, dlg.ScenarioName))
                    Try
                        Me.Core.SaveEcosimScenarioAs(dlg.ScenarioName, dlg.ScenarioDescription)
                    Catch ex As Exception
                        cLog.Write(ex, "frmEwE6::SaveEcosimScenarioAs")
                    End Try
                    cApplicationStatusNotifier.EndProgress(Me.Core)

                End If
                ' User does not want to overwrite? Abort
                Return
            End If

            ' Add scenario under new name
            cApplicationStatusNotifier.StartProgress(Me.Core, cStringUtils.Localize(My.Resources.STATUS_ECOSIM_CREATING, dlg.ScenarioName))
            Try
                Me.Core.SaveEcosimScenarioAs(dlg.ScenarioName, dlg.ScenarioDescription)
            Catch ex As Exception

            End Try
            cApplicationStatusNotifier.EndProgress(Me.Core)

        End If

    End Sub

    ''' <summary>
    ''' Command update handler; enables and disables the 'save ecosim scenario as' command
    ''' </summary>
    Private Sub OnUpdateSaveEcosimScenarioAs(cmd As cCommand) _
        Handles m_cmdSaveEcosimScenarioAs.OnUpdate
        cmd.Enabled = Me.Core.StateMonitor.HasEcosimLoaded
    End Sub

    ''' <summary>
    ''' Command handler; deletes an Ecosim scenario 
    ''' </summary>
    Private Sub OnInvokeDeleteEcosimScenario(cmd As cCommand) _
         Handles m_cmdDeleteEcosimScenario.OnInvoke
        Dim dlg As New EcosimScenarioDlg(Me.UIContext, EcosimScenarioDlg.eDialogModeType.DeleteScenario)
        dlg.ShowDialog(Me)
    End Sub

    ''' <summary>
    ''' Command update handler; enables and disables the 'delete ecosim scenario' command
    ''' </summary>
    Private Sub OnUpdateDeleteEcosimScenario(cmd As cCommand) _
           Handles m_cmdDeleteEcosimScenario.OnUpdate
        Dim m As cCoreStateMonitor = Me.Core.StateMonitor
        cmd.Enabled = (m.HasEcopathLoaded) And
                      (Me.Core.nEcosimScenarios > 0) And
                      (Not m.IsBusy)
    End Sub

    ''' <summary>
    ''' Command handler; invokes the import time series dialog.
    ''' </summary>
    Private Sub m_cmdImportTimeSeries_OnInvoke(cmd As cCommand) _
        Handles m_cmdImportTimeSeries.OnInvoke
        Me.ManageTimeSeries(dlgManageTimeSeries.eModeType.Import)
    End Sub

    ''' <summary>
    ''' Command update handler; enables and disables the <see cref="m_cmdImportTimeSeries">Import TimeSeries command</see>.
    ''' </summary>
    Private Sub m_cmdImportTimeSeries_OnUpdate(cmd As cCommand) Handles m_cmdImportTimeSeries.OnUpdate
        Dim m As cCoreStateMonitor = Me.Core.StateMonitor
        cmd.Enabled = m.HasEcosimLoaded() And Not m.IsBusy
    End Sub

    ''' <summary>
    ''' Command handler; exports the currently loaded time series dataset to a CSV file.
    ''' </summary>
    Private Sub m_cmdExportTimeSeries_OnInvoke(cmd As cCommand) _
        Handles m_cmdExportTimeSeries.OnInvoke

        Dim tsw As New cTimeSeriesCSVWriter(Me.Core)
        tsw.WriteTimeseries()

    End Sub

    ''' <summary>
    ''' Command update handler; enables and disables the <see cref="m_cmdExportTimeSeries">Export TimeSeries command</see>.
    ''' </summary>
    Private Sub m_cmdExportTimeSeries_OnUpdate(cmd As cCommand) _
        Handles m_cmdExportTimeSeries.OnUpdate
        Dim m As cCoreStateMonitor = Me.Core.StateMonitor
        cmd.Enabled = m.HasEcosimLoaded()
    End Sub

    ''' <summary>
    ''' Command handler; invokes the apply time series dialog.
    ''' </summary>
    Private Sub m_cmdWeightTimeSeries_OnInvoke(cmd As cCommand) Handles m_cmdWeightTimeSeries.OnInvoke
        Me.ManageTimeSeries(dlgManageTimeSeries.eModeType.Weight)
    End Sub

    ''' <summary>
    ''' Command update handler; enables and disables the <see cref="m_cmdWeightTimeSeries">Apply TimeSeries command</see>.
    ''' </summary>
    Private Sub m_cmdWeightTimeSeries_OnUpdate(cmd As cCommand) Handles m_cmdWeightTimeSeries.OnUpdate
        ' JS 23sept08: dialog will switch to load mode if no ts present
        Dim m As cCoreStateMonitor = Me.Core.StateMonitor
        cmd.Enabled = m.HasEcosimLoaded() And
                      Not m.IsBusy ' And Me.Core.HasTimeSeries()
    End Sub

    ''' <summary>
    ''' Command handler; invokes the load time series dialog, or loads a time
    ''' series dataset if this dataset is provided as a tag to the command.
    ''' </summary>
    Private Sub m_cmdEcosimLoadTimeSeries_OnInvoke(cmd As cCommand) _
        Handles m_cmdEcosimLoadTimeSeries.OnInvoke

        If Not Me.m_coreController.LoadState(eCoreExecutionState.EcosimLoaded) Then Return

        If (Me.m_cmdEcosimLoadTimeSeries.Tag Is Nothing) Then
            Me.ManageTimeSeries(dlgManageTimeSeries.eModeType.Load)
        ElseIf (TypeOf Me.m_cmdEcosimLoadTimeSeries.Tag Is cTimeSeriesDataset) Then
            Dim ds As cTimeSeriesDataset = DirectCast(Me.m_cmdEcosimLoadTimeSeries.Tag, cTimeSeriesDataset)
            cApplicationStatusNotifier.StartProgress(Me.Core, cStringUtils.Localize(My.Resources.STATUS_TIMESERIES_LOADING, ds.Name))
            Me.Core.LoadTimeSeries(ds, True)
            cApplicationStatusNotifier.EndProgress(Me.Core)
        End If

    End Sub

    ''' <summary>
    ''' Command update handler; enables and disables the <see cref="m_cmdEcosimLoadTimeSeries">Load TimeSeries command</see>.
    ''' </summary>
    Private Sub m_cmdEcosimLoadTimeSeries_OnUpdate(cmd As cCommand) _
        Handles m_cmdEcosimLoadTimeSeries.OnUpdate

        Dim m As cCoreStateMonitor = Me.Core.StateMonitor
        cmd.Enabled = m.HasEcopathLoaded() And Not m.IsBusy

    End Sub

    Private Sub OnExportEcosimResultsToCSV(cmd As cCommand) _
        Handles m_cmdExportEcosimResultsToCSV.OnInvoke

        Dim writer As EwECore.Ecosim.cEcosimResultWriter = Nothing
        Dim strPath As String = ""

        Me.m_cmdDirectoryOpen.Invoke(Me.Core.DefaultOutputPath(eAutosaveTypes.Ecosim))
        If (Me.m_cmdDirectoryOpen.Result <> System.Windows.Forms.DialogResult.OK) Then Return
        strPath = Me.m_cmdDirectoryOpen.Directory

        writer = New EwECore.Ecosim.cEcosimResultWriter(Me.UIContext.Core)
        writer.WriteResults(strPath, DirectCast(cmd, cEcosimSaveDataCommand).Results)
        writer = Nothing

    End Sub

    Private Sub OnExportEcosimResultsToCSVUpdate(cmd As cCommand) _
        Handles m_cmdExportEcosimResultsToCSV.OnUpdate
        cmd.Enabled = Me.Core.StateMonitor.HasEcosimRan
    End Sub

    Private Sub OnEstimateVsInvoke(cmd As cCommand) _
        Handles m_cmdEstimateVs.OnInvoke
        Dim dlg As New dlgEstimateVs(Me.UIContext)
        dlg.ShowDialog(Me)
    End Sub

    Private Sub OnEstimateVsUpdate(cmd As cCommand) _
        Handles m_cmdEstimateVs.OnUpdate
        Dim m As cCoreStateMonitor = Me.Core.StateMonitor
        cmd.Enabled = m.HasEcosimLoaded() And Not m.IsBusy
    End Sub

    Private Sub OnTrimEcosimShapesInvoke(cmd As cCommand) _
        Handles m_cmdEcosimTrimShapes.OnInvoke

        Dim fmsg As New cFeedbackMessage(My.Resources.PROMPT_TRIM_SHAPES,
                                         eCoreComponentType.ShapesManager, eMessageType.Any, eMessageImportance.Question,
                                         eMessageReplyStyle.YES_NO)
        Me.Core.Messages.SendMessage(fmsg)

        If fmsg.Reply = eMessageReply.YES Then
            Me.Core.TrimUnusedShapeData()
        End If

    End Sub

    Private Sub OnTrimEcosimShapesUpdate(cmd As cCommand) _
        Handles m_cmdEcosimTrimShapes.OnUpdate
        cmd.Enabled = Me.Core.HasUnusedShapeData And Not Me.Core.StateMonitor.IsBusy
    End Sub

    Private Sub OnEcosimChangeShapeInvoke(cmd As cCommand) _
        Handles m_cmdEcosimChangeShape.OnInvoke

        Try
            Dim dlg As New dlgChangeShape(Me.UIContext, DirectCast(cmd.Tag, cForcingFunction))
            dlg.ShowDialog(Me.UIContext.FormMain)
        Catch ex As Exception

        End Try

    End Sub

    Private Sub OnEcosimChangeShapeUpdate(cmd As cCommand) _
        Handles m_cmdEcosimChangeShape.OnUpdate
        cmd.Enabled = Me.Core.StateMonitor.HasEcosimLoaded And Not Me.Core.StateMonitor.IsBusy
    End Sub

#End Region ' Ecosim commands

#Region " Ecospace commands "

    Private Sub OnNewEcospaceScenario(cmd As cCommand) _
        Handles m_cmdNewEcospaceScenario.OnInvoke

        Dim dlg As New dlgEcospaceScenario(Me.UIContext, dlgEcospaceScenario.eDialogModeType.CreateScenario)

        If dlg.ShowDialog = System.Windows.Forms.DialogResult.OK Then

            Select Case dlg.Mode
                Case dlgEcospaceScenario.eDialogModeType.CreateScenario
                    Me.CreateEcospaceScenario(dlg.ScenarioName, dlg.ScenarioDescription,
                            dlg.ScenarioAuthor, dlg.ScenarioContact,
                            10, 10, 0, 0, 0.5)
                Case dlgEcospaceScenario.eDialogModeType.LoadScenario
                    Me.LoadEcospaceScenario(DirectCast(dlg.Scenario, cEcospaceScenario))
                Case dlgEcospaceScenario.eDialogModeType.DeleteScenario
                    Return
                Case Else
                    Debug.Assert(False)
            End Select

        End If

    End Sub

    Private Sub OnUpdateNewEcospaceScenario(cmd As cCommand) _
        Handles m_cmdNewEcospaceScenario.OnUpdate
        Dim m As cCoreStateMonitor = Me.Core.StateMonitor
        cmd.Enabled = m.HasEcosimLoaded And Not m.IsBusy
    End Sub

    Private Sub OnLoadEcospaceScenario(cmd As cCommand) _
        Handles m_cmdLoadEcospaceScenario.OnInvoke
        Me.CoreController.LoadEcospaceScenario()
    End Sub

    Private Sub OnUpdateLoadEcospaceScenario(cmd As cCommand) _
        Handles m_cmdLoadEcospaceScenario.OnUpdate
        Dim m As cCoreStateMonitor = Me.Core.StateMonitor
        cmd.Enabled = m.HasEcopathLoaded And Not m.IsBusy
    End Sub

    Private Sub OnCloseEcospaceScenario(cmd As cCommand) _
        Handles m_cmdCloseEcospaceScenario.OnInvoke
        Me.m_autosavemanager.GatherSettings()
        Me.Core.CloseEcospaceScenario()
    End Sub

    Private Sub OnUpdateCloseEcospaceScenario(cmd As cCommand) _
        Handles m_cmdCloseEcospaceScenario.OnUpdate
        Dim m As cCoreStateMonitor = Me.Core.StateMonitor
        cmd.Enabled = m.HasEcospaceLoaded And Not m.IsBusy
    End Sub

    ''' <summary>
    ''' Command handler; saves the current active Ecospace scenario under a new name.
    ''' </summary>
    Private Sub OnSaveEcospaceScenarioAs(cmd As cCommand) _
        Handles m_cmdSaveEcospaceScenarioAS.OnInvoke

        Dim dlg As New dlgEcospaceScenario(Me.UIContext,
                                           dlgEcospaceScenario.eDialogModeType.SaveScenario,
                                           Me.Core.EcospaceScenarios(Me.Core.ActiveEcospaceScenarioIndex))
        Dim scenarioTarget As cEcospaceScenario = Nothing

        If dlg.ShowDialog() = System.Windows.Forms.DialogResult.OK Then
            ' Has valid name?
            If Not String.IsNullOrEmpty(dlg.ScenarioName) Then
                ' #Cool. Now check if this will overwrite a scenario with the same name (case insensitive)
                scenarioTarget = Nothing
                For iScenario As Integer = 1 To Me.Core.nEcospaceScenarios
                    If (String.Compare(Me.Core.EcospaceScenarios(iScenario).Name, dlg.ScenarioName, True) = 0) Then
                        scenarioTarget = Me.Core.EcospaceScenarios(iScenario)
                    End If
                Next

                ' About to overwrite?
                If (scenarioTarget IsNot Nothing) Then
                    ' #Yes: prompt for overwrite confirmation
                    Dim fmsg As New cFeedbackMessage(cStringUtils.Localize(My.Resources.SCENARIO_CONFIRMOVERWRITE_PROMPT, dlg.ScenarioName), eCoreComponentType.Core, eMessageType.Any, eMessageImportance.Question, eMessageReplyStyle.YES_NO)
                    Me.Core.Messages.SendMessage(fmsg)

                    If (fmsg.Reply = eMessageReply.YES) Then

                        ' #Overwrite
                        cApplicationStatusNotifier.StartProgress(Me.Core, cStringUtils.Localize(My.Resources.STATUS_ECOSPACE_SAVING, dlg.ScenarioName))
                        Try
                            Me.Core.SaveEcospaceScenarioAs(dlg.ScenarioName, dlg.ScenarioDescription)
                        Catch ex As Exception
                            cLog.Write(ex, "frmEwE6::SaveEcopaceScenarioAs")
                        End Try
                        cApplicationStatusNotifier.EndProgress(Me.Core)

                    End If
                    ' User does not want to overwrite? Abort
                    Return
                End If

                ' Add scenario
                cApplicationStatusNotifier.StartProgress(Me.Core, cStringUtils.Localize(My.Resources.STATUS_ECOSPACE_CREATING, dlg.ScenarioName))
                Try
                    Me.Core.SaveEcospaceScenarioAs(dlg.ScenarioName, dlg.ScenarioDescription)
                Catch ex As Exception

                End Try
                cApplicationStatusNotifier.EndProgress(Me.Core)

            End If
        End If

    End Sub

    ''' <summary>
    ''' Command update handler; enables and disables the 
    ''' <see cref="m_cmdSaveEcospaceScenarioAs">Save Ecospace Scenario As</see> command.
    ''' </summary>
    Private Sub OnUpdateSaveEcospaceScenarioAs(cmd As cCommand) Handles m_cmdSaveEcospaceScenarioAS.OnUpdate
        cmd.Enabled = Me.Core.StateMonitor.HasEcospaceLoaded
    End Sub

    ''' <summary>
    ''' Command handler; deletes an Ecosim scenario 
    ''' </summary>
    Private Sub OnInvokeDeleteEcospaceScenario(cmd As cCommand) _
         Handles m_cmdDeleteEcospaceScenario.OnInvoke
        Dim dlg As New dlgEcospaceScenario(Me.UIContext, dlgScenario.eDialogModeType.DeleteScenario)
        dlg.ShowDialog(Me)
    End Sub

    ''' <summary>
    ''' Command update handler; enables and disables the 'delete ecospace scenario' command
    ''' </summary>
    Private Sub OnUpdateDeleteEcospaceScenario(cmd As cCommand) _
           Handles m_cmdDeleteEcospaceScenario.OnUpdate
        Dim m As cCoreStateMonitor = Me.Core.StateMonitor
        cmd.Enabled = m.HasEcopathLoaded And
                      Not m.IsBusy And
                      Me.Core.nEcospaceScenarios > 0
    End Sub

    ''' <summary>
    ''' Command handler; loads an Ecospace spatial temporal data set
    ''' </summary>
    Private Sub m_cmdEcospaceLoadTimeSeries_OnInvoke(cmd As cCommand) _
        Handles m_cmdEcospaceLoadTimeSeries.OnInvoke

        If Not Me.m_coreController.LoadState(eCoreExecutionState.EcospaceLoaded) Then Return

        If (Me.m_cmdEcospaceLoadTimeSeries.Tag IsNot Nothing) Then
            Dim strFile As String = CType(Me.m_cmdEcospaceLoadTimeSeries.Tag, String)
            Dim man As cSpatialDataSetManager = Me.Core.SpatialDataConnectionManager.DatasetManager
            man.Load(strFile)
        End If

    End Sub

    ''' <summary>
    ''' Command update handler; enables and disables the <see cref="m_cmdEcosimLoadTimeSeries">Load TimeSeries command</see>.
    ''' </summary>
    Private Sub m_cmdEcospaceLoadTimeSeries_OnUpdate(cmd As cCommand) _
        Handles m_cmdEcospaceLoadTimeSeries.OnUpdate

        Dim m As cCoreStateMonitor = Me.Core.StateMonitor
        cmd.Enabled = m.HasEcospaceLoaded() And Not m.IsBusy

    End Sub

    ''' <summary>
    ''' Command handler; invokes the Ecospace edit basemap dialog.
    ''' </summary>
    Private Sub OnEditEcospaceBasemap(cmd As cCommand) Handles m_cmdEditBasemap.OnInvoke
        Dim dlg As New dlgEditBasemap(Me.UIContext)
        Me.Help.HelpTopic(dlg) = "Edit basemap.htm"
        dlg.ShowDialog(Me)
    End Sub

    ''' <summary>
    ''' Command handler; handles access to the Ecospace edit basemap dialog.
    ''' </summary>
    Private Sub OnUpdateEditEcospaceBasemap(cmd As cCommand) Handles m_cmdEditBasemap.OnUpdate
        Dim m As cCoreStateMonitor = Me.Core.StateMonitor
        cmd.Enabled = m.HasEcospaceLoaded And Not m.IsBusy
    End Sub

    ''' <summary>
    ''' Command handler; invokes the Ecospace edit habitats dialog.
    ''' </summary>
    Private Sub OnEditEcospaceHabitats(cmd As cCommand) Handles m_cmdEditHabitats.OnInvoke
        Dim dlg As New dlgEditHabitats(Me.UIContext)
        Me.Help.HelpTopic(dlg) = "Edit habitats.htm"
        dlg.ShowDialog(Me)
    End Sub

    ''' <summary>
    ''' Command handler; handles access to the Ecospace edit habitats dialog.
    ''' </summary>
    Private Sub OnUpdateEditEcospaceHabitats(cmd As cCommand) Handles m_cmdEditHabitats.OnUpdate
        Dim m As cCoreStateMonitor = Me.Core.StateMonitor
        cmd.Enabled = m.HasEcospaceLoaded And Not m.IsBusy
    End Sub

    ''' <summary>
    ''' Command handler; handles access to the Ecospace edit regions dialog.
    ''' </summary>
    Private Sub OnUpdateEditEcospaceRegions(cmd As cCommand) Handles m_cmdEditRegions.OnUpdate
        Dim m As cCoreStateMonitor = Me.Core.StateMonitor
        cmd.Enabled = m.HasEcospaceLoaded And Not m.IsBusy
    End Sub

    ''' <summary>
    ''' Command handler; invokes the Ecospace edit regions dialog.
    ''' </summary>
    Private Sub OnEditEcospaceRegions(cmd As cCommand) Handles m_cmdEditRegions.OnInvoke
        Dim dlg As New dlgDefineRegions(Me.UIContext)
        Me.Help.HelpTopic(dlg) = "Edit regions.htm"
        dlg.ShowDialog(Me)
    End Sub

    ''' <summary>
    ''' Command handler; handles access to the Ecospace edit regions dialog.
    ''' </summary>
    Private Sub OnUpdateEditEcospaceEffortZOnes(cmd As cCommand) Handles m_cmdEditEffortZones.OnUpdate
        Dim m As cCoreStateMonitor = Me.Core.StateMonitor
        cmd.Enabled = m.HasEcospaceLoaded And Not m.IsBusy
    End Sub

    ''' <summary>
    ''' Command handler; invokes the Ecospace edit MPAs dialog.
    ''' </summary>
    Private Sub OnEditEcospaceMPAs(cmd As cCommand) Handles m_cmdEditMPAs.OnInvoke
        Dim dlg As New dlgEditMPAs(Me.UIContext)
        dlg.ShowDialog(Me)
    End Sub

    ''' <summary>
    ''' Command handler; handles access to the Ecospace edit MPAs dialog.
    ''' </summary>
    Private Sub OnUpdateEditEcospaceMPAs(cmd As cCommand) Handles m_cmdEditMPAs.OnUpdate
        Dim m As cCoreStateMonitor = Me.Core.StateMonitor
        cmd.Enabled = m.HasEcospaceLoaded And Not m.IsBusy
    End Sub

    ''' <summary>
    ''' Command handler; invokes the Ecospace edit importance layers dialog.
    ''' </summary>
    Private Sub OnEditEcospaceImportanceLayers(cmd As cCommand) Handles m_cmdDefineImportanceMaps.OnInvoke
        Dim dlg As New dlgDefineImportanceMaps(Me.UIContext)
        dlg.ShowDialog(Me)
    End Sub

    ''' <summary>
    ''' Command handler; updates the Ecospace edit importance layers command.
    ''' </summary>
    Private Sub OnUpdateEcospaceImportanceLayers(cmd As cCommand) Handles m_cmdDefineImportanceMaps.OnUpdate
        Dim m As cCoreStateMonitor = Me.Core.StateMonitor
        cmd.Enabled = m.HasEcospaceLoaded And Not m.IsBusy
    End Sub

    ''' <summary>
    ''' Command handler; handles access to the Ecospace define input layers dialog.
    ''' </summary>
    Private Sub OnUpdateDefineInputLayers(cmd As cCommand) Handles m_cmdDefineInputLayers.OnUpdate
        Dim m As cCoreStateMonitor = Me.Core.StateMonitor
        cmd.Enabled = m.HasEcospaceLoaded And Not m.IsBusy
    End Sub

    ''' <summary>
    ''' Command handler; invokes the Ecospace define input dialog.
    ''' </summary>
    Private Sub OnInvokeDefineInputLayers(cmd As cCommand) Handles m_cmdDefineInputLayers.OnInvoke
        Try
            Dim dlg As New dlgDefineEnvDriverMaps(Me.UIContext)
            dlg.ShowDialog(Me)
        Catch ex As Exception

        End Try
    End Sub

    ''' <summary>
    ''' Command handler
    ''' </summary>
    Private Sub OnEcospaceManageConfigs(cmd As cCommand) Handles m_cmdEcospaceManageConfigs.OnInvoke
        Try
            ' Reroute
            Me.m_cmdShowOptions.Invoke(eApplicationOptionTypes.SpatialTemporal)
        Catch ex As Exception
            cLog.Write(ex, "frmEwE6:OnEcospaceManageConfigs")
        End Try
    End Sub

    ''' <summary>
    ''' Command handler
    ''' </summary>
    Private Sub OnDefineEcospaceDatasets(cmd As cCommand) Handles m_cmdDefineSpatialDatasets.OnInvoke
        Try
            Dim dlg As New Ecospace.Controls.dlgDefineExternalSpatialData()
            dlg.UIContext = Me.UIContext
            dlg.ShowDialog(Me)
        Catch ex As Exception
            cLog.Write(ex, "frmEwE6:OnDefineEcospaceDatasets")
        End Try
    End Sub

    ''' <summary>
    ''' Command handler updater
    ''' </summary>
    Private Sub OnUpdateDefineEcospaceDatasetsInvoke(cmd As cCommand) Handles m_cmdDefineSpatialDatasets.OnUpdate
        Try
            Dim m As cCoreStateMonitor = Me.Core.StateMonitor
            cmd.Enabled = m.HasEcospaceLoaded And Not m.IsBusy
        Catch ex As Exception

        End Try
    End Sub

    ''' <summary>
    ''' Command handler
    ''' </summary>
    Private Sub OnEditEcospaceDataset(cmd As cCommand) Handles m_cmdEditSpatialDataset.OnInvoke

        Try
            Dim ds As EwEUtils.SpatialData.ISpatialDataSet = Me.m_cmdEditSpatialDataset.Dataset
            If (ds Is Nothing) Then Return
            If (Not TypeOf ds Is IConfigurable) Then Return

            '' This artifact should really not be necessary!!
            'If (TypeOf ds Is IPlugin) Then
            '    DirectCast(ds, IPlugin).Initialize(Me.Core)
            'End If

            Dim dsConf As IConfigurable = DirectCast(ds, IConfigurable)
            Dim ctrl As Control = dsConf.GetConfigUI()
            If (ctrl Is Nothing) Then Return

            Dim dlg As New dlgConfig(Me.UIContext)
            If dlg.ShowDialog(Me.FindForm, My.Resources.CAPTION_EXTERNAL_DATASET_CONFIGURE, ctrl) = System.Windows.Forms.DialogResult.OK Then
                Me.Core.SpatialDataConnectionManager.Update(ds)
            End If
        Catch ex As Exception
            cLog.Write(ex, "frmEwE6:OnDefineEcospaceDatasets")
        End Try

    End Sub

    ''' <summary>
    ''' Command handler updater
    ''' </summary>
    Private Sub OnUpdateEditEcospaceDatasetInvoke(cmd As cCommand) Handles m_cmdDefineSpatialDatasets.OnUpdate
        Try
            Dim m As cCoreStateMonitor = Me.Core.StateMonitor
            cmd.Enabled = m.HasEcospaceLoaded And Not m.IsBusy
        Catch ex As Exception

        End Try
    End Sub

    ''' <summary>
    ''' Command handler
    ''' </summary>
    Private Sub OnEcospaceConfigureConnection(cmd As cCommand) Handles m_cmdEcospaceConfigureConnection.OnInvoke

        If (Me.m_cmdEcospaceConfigureConnection.Layer Is Nothing) Then Return
        Try
            Dim adt As cSpatialDataAdapter = Me.Core.SpatialDataConnectionManager.Adapter(Me.m_cmdEcospaceConfigureConnection.Layer.VarName)
            Dim dlg As New dlgApplyConnection(Me.UIContext, adt, Me.m_cmdEcospaceConfigureConnection.Layer, Me.m_cmdEcospaceConfigureConnection.Connection)
            dlg.ShowDialog()
        Catch ex As Exception
            cLog.Write(ex, "frmEwE6:OnEcospaceConfigureConnection")
        End Try
    End Sub

    ''' <summary>
    ''' Command handler
    ''' </summary>
    Private Sub OnExportEcospaceDatasets(cmd As cCommand) Handles m_cmdEcospaceExportSpatialDatasets.OnInvoke
        Try
            Dim dlg As New Ecospace.Controls.dlgExportSpatialData(Me.UIContext)
            dlg.ShowDialog(Me)
        Catch ex As Exception
            cLog.Write(ex, "frmEwE6:OnExportEcospaceDatasets")
        End Try
    End Sub

    ''' <summary>
    ''' Command handler updater
    ''' </summary>
    Private Sub OnUpdateExportEcospaceDatasets(cmd As cCommand) Handles m_cmdEcospaceExportSpatialDatasets.OnUpdate
        Try
            Dim m As cCoreStateMonitor = Me.Core.StateMonitor
            cmd.Enabled = Not m.IsBusy And (Me.Core.SpatialDataConnectionManager.DatasetManager.Count > 0)
        Catch ex As Exception

        End Try
    End Sub

    Private Sub OnImportLayerData(cmd As cCommand) _
        Handles m_cmdImportLayerData.OnInvoke

        Dim msg As cMessage = Nothing
        Try
            Select Case Me.m_cmdImportLayerData.Format
                Case eNativeLayerFileFormatTypes.Default,
                     eNativeLayerFileFormatTypes.XYZ
                    Dim dlg As New dlgImportLayerDataXYZ(Me.UIContext)
                    dlg.Layers = Me.m_cmdImportLayerData.Layers
                    dlg.File = Me.m_cmdImportLayerData.File
                    dlg.ShowDialog(Me)

                Case eNativeLayerFileFormatTypes.ASCII,
                     eNativeLayerFileFormatTypes.TXT
                    Dim l As cEcospaceLayer = Me.m_cmdImportLayerData.Layers(0)
                    Dim file As String = Me.m_cmdImportLayerData.File

                    If (TypeOf l Is cEcospaceLayerVelocity) Then
                        ' ToDo: localize this
                        Me.SendMessage("ASCII files cannot be directy imported in velocity layers with separate U and V components", eMessageImportance.Warning)
                        Return
                    End If

                    If (String.IsNullOrWhiteSpace(file)) Then
                        Dim ofd As New OpenFileDialog()
                        ' ToDo: localize this
                        ofd.Title = "Select data for layer " & l.Name
                        ofd.Filter = SharedResources.FILEFILTER_ASC
                        If (ofd.ShowDialog() = System.Windows.Forms.DialogResult.OK) Then
                            file = ofd.FileName
                        End If
                    Else
                        ' Todo: ask for confirmation?
                    End If

                    If (Not String.IsNullOrWhiteSpace(file)) Then
                        Dim imp As New cEcospaceImportExportASCIIData(Me.Core)
                        If imp.Read(file) Then
                            Dim rs As EwEUtils.SpatialData.ISpatialRaster = imp.ToRaster
                            For ir As Integer = 1 To rs.NumRows
                                For ic As Integer = 1 To rs.NumCols
                                    l.Cell(ir, ic) = rs.Cell(ir, ic)
                                Next
                            Next
                            l.Invalidate()
                            msg = New cMessage(cStringUtils.Localize(My.Resources.IMPORT_LAYERDATA_SUCCESS, file, l.Name), eMessageType.DataImport, eCoreComponentType.External, eMessageImportance.Information)
                        Else
                            msg = New cMessage(cStringUtils.Localize(My.Resources.IMPORT_LAYERDATA_FAILED, file, l.Name), eMessageType.DataImport, eCoreComponentType.External, eMessageImportance.Critical)
                        End If
                    End If
            End Select
        Catch ex As Exception
            cLog.Write(ex, "frmEwE6:OnImportLayerData")
        End Try

        If (msg IsNot Nothing) Then
            Me.Core.Messages.SendMessage(msg)
        End If

    End Sub

    ''' <summary>
    ''' Command handler
    ''' </summary>
    Private Sub OnUpdateImportLayer(cmd As cCommand) _
        Handles m_cmdImportLayerData.OnUpdate
        Dim m As cCoreStateMonitor = Me.Core.StateMonitor
        cmd.Enabled = m.HasEcospaceLoaded() And Not m.IsBusy
    End Sub

    ''' <summary>
    ''' Command handler; invokes the export layers dialog to export data in XYZ format.
    ''' </summary>
    Private Sub OnExportLayerData(cmd As cCommand) _
        Handles m_cmdExportLayerData.OnInvoke
        Try
            Select Case Me.m_cmdExportLayerData.Format
                Case eNativeLayerFileFormatTypes.Default,
                     eNativeLayerFileFormatTypes.XYZ
                    Dim dlg As New dlgExportLayerDataXYZ(Me.UIContext)
                    dlg.Layers = Me.m_cmdExportLayerData.Layers
                    dlg.ShowDialog(Me)
                Case eNativeLayerFileFormatTypes.ASCII
                    Dim sfd As SaveFileDialog = cEwEFileDialogHelper.SaveFileDialog(SharedResources.CAPTION_SELECT_FILE, "", SharedResources.FILEFILTER_ASC)
                    If (sfd.ShowDialog() = System.Windows.Forms.DialogResult.OK) Then
                        Dim imp As New cEcospaceImportExportASCIIData(Me.Core)
                        If imp.Read(Me.m_cmdExportLayerData.Layers(0)) Then
                            Dim bSuccess As Boolean = imp.Save(sfd.FileName)
                            Dim msg As cMessage = Nothing
                            If (bSuccess) Then
                                msg = New cMessage(cStringUtils.Localize(My.Resources.STATUS_DATA_SAVING_SUCCESS, sfd.FileName),
                                                   eMessageType.DataExport, eCoreComponentType.Ecospace, eMessageImportance.Information)
                                msg.Hyperlink = Path.GetDirectoryName(sfd.FileName)
                            Else
                                msg = New cMessage(cStringUtils.Localize(My.Resources.STATUS_DATA_SAVING_FAILURE, sfd.FileName),
                                                   eMessageType.DataExport, eCoreComponentType.Ecospace, eMessageImportance.Critical)
                            End If

                            Me.Core.Messages.SendMessage(msg)
                            ' ToDo: throw message
                        End If
                    End If
            End Select
        Catch ex As Exception
            cLog.Write(ex, "frmEwE6:OnExportLayerData")
        End Try
    End Sub

    ''' <summary>
    ''' Command update handler; enables and disables the 
    ''' <see cref="m_cmdImportLayerData">export layer data command</see>.
    ''' </summary>
    Private Sub OnUpdateExportLayerData(cmd As cCommand) _
        Handles m_cmdExportLayerData.OnUpdate

        Dim m As cCoreStateMonitor = Me.Core.StateMonitor
        cmd.Enabled = m.HasEcospaceLoaded() And Not m.IsBusy

    End Sub

    ''' <summary>
    ''' Command handler; invokes the edit layers dialog.
    ''' </summary>
    Private Sub OnInvokeEditLayer(cmd As cCommand) _
        Handles m_cmdEditLayer.OnInvoke

        Try
            Dim cmdEditLayer As cEditLayerCommand = DirectCast(cmd, cEditLayerCommand)
            Dim dlg As New dlgEditLayer(Me.UIContext, cmdEditLayer.Layer, cmdEditLayer.EditType)
            dlg.ShowDialog()
        Catch ex As Exception

        End Try

    End Sub

    ''' <summary>
    ''' Command update handler; enables and disables the 
    ''' <see cref="m_cmdImportLayerData">export layer data command</see>.
    ''' </summary>
    Private Sub OnUpdateEditLayer(cmd As cCommand) _
        Handles m_cmdEditLayer.OnUpdate

        Dim m As cCoreStateMonitor = Me.Core.StateMonitor
        ' Allow layer edits when Ecospace is paused
        cmd.Enabled = m.HasEcospaceLoaded() And (Not m.IsBusy Or Me.Core.EcospacePaused)

    End Sub

    Private Sub m_cmdEcospaceLoadXYRefData_OnUpdate(cmd As cCommand) Handles m_cmdEcospaceLoadXYRefData.OnUpdate
        Dim m As cCoreStateMonitor = Me.Core.StateMonitor
        cmd.Enabled = m.HasEcospaceLoaded And Not m.IsBusy
    End Sub

    Private Sub m_cmdEcospaceLoadXYRefData_OnInvoke(cmd As cCommand) Handles m_cmdEcospaceLoadXYRefData.OnInvoke
        Dim cmdh As cCommandHandler = Me.UIContext.CommandHandler
        Dim cmdFO As cFileOpenCommand = DirectCast(cmdh.GetCommand(cFileOpenCommand.COMMAND_NAME), cFileOpenCommand)

        cmdFO.Invoke(SharedResources.FILEFILTER_CSV & "|" & SharedResources.FILEFILTER_XYZ & "|" & SharedResources.FILEFILTER_TEXT)
        If cmdFO.Result = System.Windows.Forms.DialogResult.OK Then
            Dim manager As EcospaceTimeSeries.cEcospaceTimeSeriesManager = Me.Core.EcospaceTimeSeriesManager
            Dim InputFile As String = cmdFO.FileNames(0)
            manager.Load(InputFile, "", eVarNameFlags.EcospaceMapBiomass) ' Load with default output file name
        End If

    End Sub

#End Region ' Ecospace commands

#Region " Ecotracer commands "

    ''' <summary>
    ''' Command handler; creates a new Ecotracer scenario
    ''' </summary>
    Private Sub OnNewEcotracerScenario(cmd As cCommand) _
        Handles m_cmdNewEcotracerScenario.OnInvoke

        ' Prerequesite: Ecosim needs to be loaded
        Me.CoreController.LoadState(eCoreExecutionState.EcosimLoaded)
        ' Not successful? abort
        If Not Me.Core.StateMonitor.HasEcosimLoaded Then Return

        Dim dlg As New dlgEcotracerScenario(Me.UIContext, dlgEcotracerScenario.eDialogModeType.CreateScenario)

        If dlg.ShowDialog = System.Windows.Forms.DialogResult.OK Then

            Select Case dlg.Mode
                Case dlgEcotracerScenario.eDialogModeType.CreateScenario
                    Me.CreateEcotracerScenario(dlg.ScenarioName, dlg.ScenarioDescription, dlg.ScenarioAuthor, dlg.ScenarioContact)
                Case dlgEcotracerScenario.eDialogModeType.LoadScenario
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
    Private Sub OnUpdateNewEcotracerScenario(cmd As cCommand) _
        Handles m_cmdNewEcotracerScenario.OnUpdate
        Dim m As cCoreStateMonitor = Me.Core.StateMonitor
        cmd.Enabled = m.HasEcopathLoaded And Not m.IsBusy
    End Sub

    ''' <summary>
    ''' Command handler; loads a new Ecotracer scenario
    ''' </summary>
    Private Sub OnLoadEcotracerScenario(cmd As cCommand) _
        Handles m_cmdLoadEcotracerScenario.OnInvoke
        Me.LoadEcotracerScenario()
    End Sub

    ''' <summary>
    ''' Command update handler; takes care of enabling and disabling the 
    ''' <see cref="m_cmdLoadEcotracerScenario">Load Ecotracer Scenario</see> command.
    ''' </summary>
    Private Sub OnUpdateLoadEcotracerScenario(cmd As cCommand) _
        Handles m_cmdLoadEcotracerScenario.OnUpdate
        Dim m As cCoreStateMonitor = Me.Core.StateMonitor
        cmd.Enabled = m.HasEcopathLoaded And Not m.IsBusy
    End Sub

    ''' <summary>
    ''' Command handler; closes the current Ecotracer scenario
    ''' </summary>
    Private Sub OnCloseEcotracerScenario(cmd As cCommand) _
        Handles m_cmdCloseEcotracerScenario.OnInvoke
        Me.m_autosavemanager.GatherSettings()
        Me.Core.CloseEcotracerScenario()
    End Sub

    ''' <summary>
    ''' Command update handler; takes care of enabling and disabling the 
    ''' <see cref="m_cmdCloseEcotracerScenario">Close Ecotracer Scenario</see> command.
    ''' </summary>
    Private Sub OnUpdateCloseEcotracerScenario(cmd As cCommand) _
        Handles m_cmdCloseEcotracerScenario.OnUpdate
        Dim m As cCoreStateMonitor = Me.Core.StateMonitor
        cmd.Enabled = m.HasEcotracerLoaded And Not m.IsBusy
    End Sub

    Private Sub OnSaveEcotracerScenarioAs(cmd As cCommand) _
        Handles m_cmdSaveEcotracerScenarioAS.OnInvoke

        Dim dlg As New dlgEcotracerScenario(Me.UIContext,
                                            dlgEcotracerScenario.eDialogModeType.SaveScenario,
                                            Me.Core.EcotracerScenarios(Me.Core.ActiveEcotracerScenarioIndex))

        If dlg.ShowDialog() = System.Windows.Forms.DialogResult.OK Then
            ' Overwriting?
            If (dlg.Scenario IsNot Nothing) Then
                ' #Yes: prompt for overwrite confirmation
                Dim fmsg As New cFeedbackMessage(cStringUtils.Localize(My.Resources.SCENARIO_CONFIRMOVERWRITE_PROMPT, dlg.ScenarioName), eCoreComponentType.Core, eMessageType.Any, eMessageImportance.Question, eMessageReplyStyle.YES_NO)
                Me.Core.Messages.SendMessage(fmsg)

                If (fmsg.Reply = eMessageReply.YES) Then
                    ' #Overwrite
                    cApplicationStatusNotifier.StartProgress(Me.Core, cStringUtils.Localize(My.Resources.STATUS_ECOTRACER_SAVING, dlg.ScenarioName))
                    Try
                        Me.Core.SaveEcotracerScenario(DirectCast(dlg.Scenario, cEcotracerScenario))
                    Catch ex As Exception
                        cLog.Write(ex, "frmEwE6::SaveEcotracerScenarioAs")
                    End Try
                    cApplicationStatusNotifier.EndProgress(Me.Core)
                End If
                ' User does not want to overwrite? Abort
                Return
            End If

            ' Add scenario under new name
            cApplicationStatusNotifier.StartProgress(Me.Core, cStringUtils.Localize(My.Resources.STATUS_ECOTRACER_CREATING, dlg.ScenarioName))
            Me.Core.SaveEcotracerScenarioAs(dlg.ScenarioName, dlg.ScenarioDescription)
            cApplicationStatusNotifier.EndProgress(Me.Core)

        End If

    End Sub

    ''' <summary>
    ''' Command update handler; enables and disables the 'save ecotracer scenario as' command
    ''' </summary>
    Private Sub OnUpdateSaveEcotracerScenarioAs(cmd As cCommand) _
        Handles m_cmdSaveEcotracerScenarioAS.OnUpdate
        cmd.Enabled = Me.Core.StateMonitor.HasEcotracerLoaded()
    End Sub

    ''' <summary>
    ''' Command update handler; invokes the 'delete ecotracer scenario' command
    ''' </summary>
    Private Sub OnDeleteEcotracerScenario(cmd As cCommand) _
         Handles m_cmdDeleteEcotracerScenario.OnInvoke
        Dim dlg As New dlgEcotracerScenario(Me.UIContext, dlgScenario.eDialogModeType.DeleteScenario)
        dlg.ShowDialog(Me)
    End Sub

    ''' <summary>
    ''' Command update handler; enables and disables the 'delete ecotracer scenario' command
    ''' </summary>
    Private Sub OnUpdateDeleteEcotracerScenario(cmd As cCommand) _
        Handles m_cmdDeleteEcotracerScenario.OnUpdate
        Dim m As cCoreStateMonitor = Me.Core.StateMonitor
        cmd.Enabled = m.HasEcopathLoaded And
                      Not m.IsBusy And
                      Me.Core.nEcotracerScenarios > 0
    End Sub

    Private Sub OnEnableEcotracer(cmd As cCommand) _
        Handles m_cmdEnableEcotracer.OnInvoke

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
                ' Not successful? abort
                If Not Me.Core.StateMonitor.HasEcosimLoaded Then Return
                ' Get property to enable tracer for Sim
                ecosimModelParams = Me.Core.EcosimModelParameters
                propSimConTracing = DirectCast(Me.PropertyManager.GetProperty(ecosimModelParams, eVarNameFlags.ConSimOnEcoSim), cBooleanProperty)
                ' Try to load tracer
                Me.CoreController.LoadState(eCoreExecutionState.EcotracerLoaded)

            Case eTracerRunModeTypes.RunSpace ' Ecospace
                ' Load space
                Me.CoreController.LoadState(eCoreExecutionState.EcospaceLoaded)
                ' Not successful? abort
                If Not Me.Core.StateMonitor.HasEcospaceLoaded Then Return
                ' Get property to enable tracer for Space
                ecospaceModelParams = Me.Core.EcospaceModelParameters
                propSpaceConTracing = DirectCast(Me.PropertyManager.GetProperty(ecospaceModelParams, eVarNameFlags.ConSimOnEcoSpace), cBooleanProperty)
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

    Private Sub OnUpdateEnableEcotracer(cmd As cCommand) _
        Handles m_cmdEnableEcotracer.OnUpdate
        Dim m As cCoreStateMonitor = Me.Core.StateMonitor
        cmd.Enabled = m.HasEcopathLoaded And Not m.IsBusy
    End Sub

#End Region ' Ecotracer commands

#Region " Plug-in commands "

    Private Sub OnRunGUIPlugin(cmd As cCommand) Handles m_cmdPluginGUICommand.OnInvoke

        ' Sanity checks
        If Not (TypeOf cmd Is cPluginGUICommand) Then Return

        ' Phew
        Dim pgcmd As cPluginGUICommand = DirectCast(cmd, cPluginGUICommand)
        Dim iDockState As Integer = 0

        ' Check if core can be brought up to par
        If Me.CoreController.LoadState(pgcmd.CoreExecutionState) Then
            ' Invoke plugin. This code does not - and cannot - verify whether the plugin has already ran,
            ' and whether any plug-in UI elements are still active. The plug-in is responsible for dealing
            ' with consecutive run requests.

            cApplicationStatusNotifier.StartProgress(Me.Core, SharedResources.GENERIC_STATUS_RUNNINGPLUGIN)
            Try
                pgcmd.RunPlugin()
            Catch ex As Exception

            End Try
            cApplicationStatusNotifier.EndProgress(Me.Core)

            ' See if the plug-in attached any form to the command. This form will be nested in the interface
            ' if possible.
            If (pgcmd.Form IsNot Nothing) Then
                ' #Yes: form detected

                ' Inherit plug-in execution state if needed
                If (TypeOf pgcmd.Form Is frmEwE) Then
                    Dim frmEwE As frmEwE = DirectCast(pgcmd.Form, frmEwE)
                    If (frmEwE.CoreExecutionState = eCoreExecutionState.Idle) Then
                        frmEwE.CoreExecutionState = pgcmd.CoreExecutionState
                    End If
                End If

                ' Able to activate this form from the open tabs?
                If Not Me.ActivateForm(pgcmd.Form.Text) Then
                    ' #No: form is not currently integrated in the dock panel, it must be nested in the GUI.

                    ' Make sure it is not already shown; a visible form cannot be docked.
                    If pgcmd.Form.Visible Then
                        pgcmd.Form.Hide()
                    End If

                    ' Is this a dockable form? 
                    If (TypeOf pgcmd.Form Is DockContent) And (Me.m_DockPanel.DocumentStyle = DocumentStyle.DockingMdi) Then
                        ' #Yes
                        ' Fix dockstyle
                        iDockState = pgcmd.DockState
                        If iDockState = 0 Then iDockState = DockState.Document

                        Try
                            ' Show the form in the dock panel
                            DirectCast(pgcmd.Form, DockContent).Show(Me.m_DockPanel, DirectCast(iDockState, DockState))
                        Catch ex As Exception

                        End Try

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
                    ' Attach to help
                    Me.Help.HelpTopic(pgcmd.Form, pgcmd.HelpURL) = pgcmd.HelpTopic
                End If
            End If
        End If
    End Sub

#End Region ' Plug-in commands

#Region " License commands "

    Private Sub OnEnterLicense(cmd As cCommand) Handles m_cmdEnterLicense.OnInvoke
        Dim l As New cWebLinks(Me.Core)
        If Me.Core.License.ShowRegistrationForm(Me, Me.Text, l.GetURL(cWebLinks.eLinkType.GoPro), SharedResources.Ecopath_install) = DialogResult.OK Then
            Me.UpdateRegistrationControls()
        End If
    End Sub

    Private Sub OnClearLicense(cmd As cCommand) Handles m_cmdClearLicense.OnInvoke
        If (Not Me.Core.License.IsRegistered) Then Return
        Me.Core.License.Unregister()
        Me.UpdateRegistrationControls()
    End Sub

    Private Sub OnClearLicenseUpdate(cmd As cCommand) Handles m_cmdClearLicense.OnUpdate
        cmd.Enabled = Me.Core.License.IsRegistered()
    End Sub

#End Region ' License commands

#End Region ' Command handlers 

#Region " Event handlers "

    Private Sub OnModelMRUItemClicked(sender As Object, e As System.EventArgs)
        Try
            Dim mnuItem As ToolStripMenuItem = DirectCast(sender, ToolStripMenuItem)
            Dim strFileName As String = CStr(mnuItem.Tag)
            cApplicationStatusNotifier.StartProgress(Me.Core, My.Resources.STATUS_ECOPATH_LOADING)
            Me.LoadEcopathModel(strFileName, eLoadSourceType.MRU)
            cApplicationStatusNotifier.EndProgress(Me.Core)
        Catch ex As Exception
            ' Whoah!
        End Try
    End Sub

    Private Sub OnSpatialTempMRUItemClicked(sender As Object, e As System.EventArgs)
        Try
            Dim mnuItem As ToolStripMenuItem = DirectCast(sender, ToolStripMenuItem)
            Me.m_cmdEcospaceLoadTimeSeries.Tag = mnuItem.Tag
            Me.m_cmdEcospaceLoadTimeSeries.Invoke()
            Me.m_cmdEcospaceLoadTimeSeries.Tag = Nothing
        Catch ex As Exception
            ' Whoah!
        End Try
    End Sub

    Private Sub OnLoadEcosimScenarioOrDataset(sender As Object, e As System.EventArgs)
        Dim mnuItem As ToolStripMenuItem = CType(sender, ToolStripMenuItem)

        If (mnuItem.Tag Is Nothing) Then Return

        If (TypeOf mnuItem.Tag Is cEcoSimScenario) Then
            Me.m_cmdLoadEcosimScenario.Tag = mnuItem.Tag
            Me.m_cmdLoadEcosimScenario.Invoke()
            Me.m_cmdLoadEcosimScenario.Tag = Nothing
        ElseIf (TypeOf mnuItem.Tag Is cTimeSeriesDataset) Then
            Me.m_cmdEcosimLoadTimeSeries.Tag = DirectCast(mnuItem.Tag, cTimeSeriesDataset)
            Me.m_cmdEcosimLoadTimeSeries.Invoke()
            Me.m_cmdEcosimLoadTimeSeries.Tag = Nothing
        End If

    End Sub

    Private Sub OnLoadEcospaceScenario(sender As Object, e As System.EventArgs)

        Dim mnuItem As ToolStripMenuItem = CType(sender, ToolStripMenuItem)

        If (mnuItem.Tag Is Nothing) Then Return

        Me.m_cmdLoadEcospaceScenario.Tag = mnuItem.Tag
        Me.m_cmdLoadEcospaceScenario.Invoke()
        Me.m_cmdLoadEcospaceScenario.Tag = Nothing

    End Sub

    Private Sub OnLoadEcotracerScenario(sender As Object, e As System.EventArgs)
        Dim mnuItem As ToolStripMenuItem = CType(sender, ToolStripMenuItem)
        Me.m_cmdLoadEcotracerScenario.Tag = mnuItem.Tag
        Me.m_cmdLoadEcotracerScenario.Invoke()
        Me.m_cmdLoadEcotracerScenario.Tag = Nothing
    End Sub

    Private Sub OnViewItemsMenuOpening(sender As Object, e As EventArgs) Handles m_tsmiViewItems.DropDownOpening

        Me.m_tsmiViewItems.DropDownItems.Clear()
        If Me.Core.StateMonitor.HasEcopathLoaded Then
            For Each preset As String In Me.StyleGuide.ItemVisibilityPresetNames
                Me.m_tsmiViewItems.DropDownItems.Add(preset, Nothing, AddressOf Me.OnClickItemVisibilityPreset)
            Next
        End If

    End Sub

    Private Sub OnViewItemsDropDownOpening(sender As Object, e As EventArgs) Handles m_tsddViewItems.DropDownOpening

        Me.m_tsddViewItems.DropDownItems.Clear()
        If Me.Core.StateMonitor.HasEcopathLoaded Then
            For Each preset As String In Me.StyleGuide.ItemVisibilityPresetNames
                Me.m_tsddViewItems.DropDownItems.Add(preset, Nothing, AddressOf Me.OnClickItemVisibilityPreset)
            Next
        End If

    End Sub

    Private Sub OnClickItemVisibilityPreset(sender As Object, args As EventArgs)
        Try
            Me.StyleGuide.SelectedItemVisibilityPresetName = DirectCast(sender, ToolStripMenuItem).Text
        Catch ex As Exception

        End Try
    End Sub

#Region " Settings handling "

    Private Sub OnSettingsLoaded(sender As Object, e As System.Configuration.SettingsLoadedEventArgs)

        Try

            ' Fix last selected dir
            If Not Directory.Exists(My.Settings.LastSelectedDirectory) Then
                My.Settings.LastSelectedDirectory = My.Computer.FileSystem.SpecialDirectories.MyDocuments
            End If

            ' Read form positions
            Me.UIContext.FormSettings.Setting = My.Settings.FormSettings

            ' Get the form position from user settings
            Me.StartPosition = FormStartPosition.Manual
            Me.UIContext.FormSettings.Apply(Me, False)

            ' Kick the core
            Me.UpdateCorePaths(True)

            If String.IsNullOrWhiteSpace(My.Settings.Author) Then
                My.Settings.Author = Environment.UserName
            End If
            Me.Core.DefaultAuthor = My.Settings.Author
            Me.Core.DefaultContact = My.Settings.Contact

            Me.Core.SaveWithFileHeader = My.Settings.AutosaveHeaders
            Me.m_autosavemanager.Settings = My.Settings.AutosaveResults

            Dim man As cSpatialDataSetManager = Me.Core.SpatialDataConnectionManager.DatasetManager
            man.IsIndexingEnabled = My.Settings.SpatialTempAllowIndexing
            man.ConfigFiles = My.Settings.SpatialTempConfigurations

            ' Wait for Ecospace
            man.Load(My.Settings.SpatialTemporalConfigFile)

        Catch ex As Exception

        End Try

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Event handler to respond to individual settings changes.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' -----------------------------------------------------------------------
    Private Sub OnSettingsChanged(sender As Object, e As PropertyChangedEventArgs)

        Try

            Select Case e.PropertyName

                Case "StatusMaxMessages", "StatusShowTime", "StatusSortNewestFirst"
                    If (Me.m_messageHistory IsNot Nothing) Then Me.m_messageHistory.Refresh()

                Case "MdbRecentlyUsedCount"
                    Me.PopulateModelMRUDropdown()

                Case "BackupFileMask", "OutputPathMask"
                    Me.UpdateCorePaths(False)

                Case "LogVerboseLevel"
                    Try
                        cLog.VerboseLevel = DirectCast(My.Settings.LogVerboseLevel, eVerboseLevel)
                    Catch ex As Exception
                        cLog.VerboseLevel = eVerboseLevel.Standard
                    End Try

                Case "Author"
                    Me.Core.DefaultAuthor = My.Settings.Author

                Case "Contact"
                    Me.Core.DefaultContact = My.Settings.Contact

                Case "SpatialTempAllowIndexing"
                    Me.Core.SpatialDataConnectionManager.DatasetManager.IsIndexingEnabled = My.Settings.SpatialTempAllowIndexing

                Case "AutosaveResults"
                    Me.m_autosavemanager.ApplySettingsAndEnsureDefaults()

                Case "AutosaveHeaders"
                    Me.Core.SaveWithFileHeader = My.Settings.AutosaveHeaders

                Case "Author"
                    Me.Core.DefaultAuthor = My.Settings.Author

                Case "Contact"
                    Me.Core.DefaultContact = My.Settings.Contact

            End Select

            Me.m_ssMain.UpdateModelPanes()

        Catch ex As Exception

        End Try

    End Sub

    Private Sub OnSettingsSaving(sender As Object, args As CancelEventArgs)

        My.Settings.AutosaveResults = Me.m_autosavemanager.Settings()
        My.Settings.SpatialTempAllowIndexing = Me.Core.SpatialDataConnectionManager.DatasetManager.IsIndexingEnabled

        args.Cancel = False

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Update the directories in the Core to match any regular expressions.
    ''' </summary>
    ''' <remarks>
    ''' Note that this will also reset the base directory for commands 
    ''' <see cref="m_cmdFileOpen"/> and <see cref="m_cmdDirectoryOpen"/>.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Private Sub UpdateCorePaths(Optional bResetUI As Boolean = False)

        Dim strPath As String = ""

        If String.IsNullOrWhiteSpace(My.Settings.BackupFileMask) Then
            My.Settings.BackupFileMask = CStr(My.Settings.GetDefaultValue("BackupFileMask"))
        End If

        If cPathUtility.ResolvePath(My.Settings.BackupFileMask, Me.Core, strPath) Then
            ' Pass MASK in because the core will need to substitute fields into the mask
            Me.Core.BackupFileMask = My.Settings.BackupFileMask
        End If

        If String.IsNullOrWhiteSpace(My.Settings.OutputPathMask) Then
            My.Settings.OutputPathMask = CStr(My.Settings.GetDefaultValue("OutputPathMask"))
        End If

        If cPathUtility.ResolvePath(My.Settings.OutputPathMask, Me.Core, strPath) Then
            ' Pass actual formatted path because the core will not change this further.
            Me.Core.OutputPath = Path.GetFullPath(strPath)

            If (bResetUI) Then
                ' Also reset file and directory commands to use output dir by default
                Me.m_cmdFileOpen.Directory = Me.Core.OutputPath
                Me.m_cmdFileSave.Directory = Me.Core.OutputPath
                Me.m_cmdDirectoryOpen.Directory = Me.Core.OutputPath
            End If
        End If

    End Sub

#End Region ' Settings handling

    Private Sub OnTabFocusChanged(sender As System.Object, e As System.EventArgs)

        Dim idc As IDockContent = Me.m_DockPanel.ActiveDocument
        Dim dch As DockContentHandler = Nothing
        Dim strNewNodeName As String = String.Empty
        Dim stateNew As eCoreExecutionState = eCoreExecutionState.Idle

        ' UI is CONTROLLING the nav tree, do NOT respond to events
        If Me.m_bNavigating Then Return

        If idc IsNot Nothing Then
            dch = idc.DockHandler

            If dch IsNot Nothing Then
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
            Me.Help.ActiveHelpControl = CType(Me.m_DockPanel.ActiveDocument, Control)
            ' Switch
            Me.UpdateSelectedNode(strNewNodeName)
        End If
    End Sub

    Private Sub OnModelPathAreaClicked(sender As System.Object, e As System.EventArgs) _
        Handles m_tsModel.OnPathAreaClicked
        Me.m_cmdLoadModel.Tag = Me.m_tsModel.Path
        Me.m_cmdLoadModel.Invoke()
        Me.m_cmdLoadModel.Tag = Nothing
    End Sub

    Private Sub OnCoreExecutionStateChanged(csm As cCoreStateMonitor)

        Try
            ' Busy loading or unloading Ecopath?
            If (csm.CoreExecutionState = eCoreExecutionState.Idle) Or
               (csm.CoreExecutionState = eCoreExecutionState.EcopathLoaded) Then
                ' Set or clear initial nav node
                Me.UpdateSelectedNode("", (csm.CoreExecutionState = eCoreExecutionState.EcopathLoaded))
            End If
            'Me.PopulateModelMRUDropdown()
            'Me.PopulateScenarioDropdowns()
            Me.UpdateModelControls()

        Catch ex As Exception
            cLog.Write(ex, "frmEwE6::OnCoreExecutionStateChanged(" & csm.CoreExecutionState.ToString() & ")")
        End Try

    End Sub

    Private Sub OnCoreMessage(ByRef msg As cMessage)
        Try
            If msg.Type = eMessageType.DataAddedOrRemoved Then
                If (msg.DataType = eDataTypes.EcoSimScenario) Or
                   (msg.DataType = eDataTypes.EcoSpaceScenario) Or
                   (msg.DataType = eDataTypes.EcotracerScenario) Or
                   (msg.DataType = eDataTypes.TimeSeriesDataset) Or
                   (msg.DataType = eDataTypes.EcospaceSpatialDataConnection) Then
                    Me.PopulateScenarioDropdowns()
                End If
            End If
            If msg.Source = eCoreComponentType.Core Then
                If (msg.Type = eMessageType.GlobalSettingsChanged) Then
                    My.Settings.Save()
                End If
            End If
        Catch ex As Exception
            cLog.Write(ex, "frmEwE6::OnCoreMessage(" & msg.Message & ")")
        End Try
    End Sub

    Private Sub OnProgressMessage(ByRef msg As cMessage)
        If Not TypeOf (msg) Is cProgressMessage Then Return
        Debug.Assert(msg.Type = eMessageType.Progress)
        Try
            Dim pmsg As cProgressMessage = DirectCast(msg, cProgressMessage)
            Me.ShowProgress(pmsg.ProgressState, pmsg.Message, pmsg.Progress)
        Catch ex As Exception
            cLog.Write(ex, "frmEwE6::OnProgressMessage(" & msg.Message & ")")
        End Try
    End Sub

    Private Sub OnModelNameChanged(prop As cProperty, cf As cProperty.eChangeFlags)
        Me.UpdateModelControls()
    End Sub


#End Region  ' Event handlers

End Class