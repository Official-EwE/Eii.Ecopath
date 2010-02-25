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
Imports ScientificInterfaceShared.Controls
Imports ScientificInterfaceShared.Controls.EwEGrid
Imports WeifenLuo.WinFormsUI.Docking
Imports EwEUtils.Commands
Imports EwEUtils.Core
Imports EwEUtils.Database
Imports EwEUtils.Utilities
Imports Microsoft.VisualBasic

#End Region ' Imports

''' <summary>
''' The main entry point for the graphics interface
''' </summary>
Public Class AppLauncher
    Implements IApplicationStatusDispatcher
    Implements IUIElement

#Region " Variables "

    Private m_uic As cUIContext = Nothing

    Private m_pluginManager As cPluginManager = Nothing
    Private m_pluginMenuHandler As cPluginMenuHandler = Nothing
    Private m_coreController As cCoreController = Nothing
    Private m_FormStateHelper As cEwEFormStateHelper = Nothing

    Private m_strLastSelectedPath As String = ""
    Private m_lstrStatus As New List(Of String)

    Private m_DockPanel As DockPanel = Nothing
    Private m_NavPanel As NavigationPanel = Nothing
    Private m_StatusPanel As StatusPanel = Nothing
    Private m_RemarkPanel As RemarkPanel = Nothing
    Private m_StartPage As WebBrowserDC = Nothing
    Private m_lstrProtectedPanelNames As New List(Of String)
    Private m_DeserializeDockContent As DeserializeDockContent
    Private m_Help As ApplicationHelp = Nothing

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
    Private WithEvents m_cmdSaveEcosimScenario As cCommand = Nothing
    Private WithEvents m_cmdSaveEcosimScenarioAs As cCommand = Nothing
    Private WithEvents m_cmdNewEcospaceScenario As cCommand = Nothing
    Private WithEvents m_cmdLoadEcospaceScenario As cCommand = Nothing
    Private WithEvents m_cmdSaveEcospaceScenario As cCommand = Nothing
    Private WithEvents m_cmdSaveEcospaceScenarioAS As cCommand = Nothing
    Private WithEvents m_cmdNewEcotracerScenario As cCommand = Nothing
    Private WithEvents m_cmdLoadEcotracerScenario As cCommand = Nothing
    Private WithEvents m_cmdSaveEcotracerScenario As cCommand = Nothing
    Private WithEvents m_cmdSaveEcotracerScenarioAS As cCommand = Nothing
    Private WithEvents m_cmdCloseAllForms As cCommand = Nothing
    Private WithEvents m_cmdNavigate As cNavigationCommand = Nothing
    Private WithEvents m_cmdViewNavPane As cCommand = Nothing
    Private WithEvents m_cmdViewStatusPane As cCommand = Nothing
    Private WithEvents m_cmdViewStartPanel As cCommand = Nothing
    Private WithEvents m_cmdViewRemarkPane As cCommand = Nothing
    Private WithEvents m_cmdViewModelBar As cCommand = Nothing
    Private WithEvents m_cmdViewStatusbar As cCommand = Nothing
    Private WithEvents m_cmdEditGroups As cCommand = Nothing
    Private WithEvents m_cmdEditMultiStanza As cCommand = Nothing
    Private WithEvents m_cmdEditFleets As cCommand = Nothing
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
    Private WithEvents m_cmdLoadWeightTimeSeries As cCommand = Nothing
    Private WithEvents m_cmdPluginGUICommand As PluginGUICommand = Nothing
    Private WithEvents m_cmdHelpAbout As cCommand = Nothing
    Private WithEvents m_cmdPropertySelection As PropertySelectionCommand = Nothing
    Private WithEvents m_cmdDisplayGroups As cDisplayGroupsCommand = Nothing
    Private WithEvents m_cmdEnableEcotracer As cCommand = Nothing
    Private WithEvents m_cmdEstimateVs As cCommand = Nothing
    ' ToDo_JS: Discontinue, move to Ecosim UI
    Private WithEvents m_cmdExportBiomassToCSV As cCommand = Nothing

#End Region ' Commands

    ''' <summary>Style guide updater.</summary>
    Private m_styleguideupdater As StyleGuideUpdater = Nothing
    Private m_applictionStatusNotifier As cApplicationStatusNotifier = Nothing
    Private m_applicationComponents As ApplicationComponents = Nothing

    Private m_SyncObj As System.Threading.SynchronizationContext

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

        If Me.m_SyncObj Is Nothing Then
            'create the sync object on the same thread that created the AppLauncher
            Me.m_SyncObj = System.Threading.SynchronizationContext.Current
            'if there is no current context then create a new one on this thread. 
            If (Me.m_SyncObj Is Nothing) Then Me.m_SyncObj = New System.Threading.SynchronizationContext()
        End If

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

#End Region ' IUIElement implementation

#Region " Properties "

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

    Public ReadOnly Property ApplicationComponents() As ScientificInterface.ApplicationComponents
        Get
            Return Me.m_applicationComponents
        End Get
    End Property

    Public ReadOnly Property SyncObject() As System.Threading.SynchronizationContext
        Get
            Return Me.m_SyncObj
        End Get
    End Property

#End Region ' Properties

#Region " Public interfaces "

    Private Delegate Sub SendMessageDelegate(ByVal strMsg As String, _
                                             ByVal importance As eMessageImportance, _
                                             ByVal component As eCoreComponentType)

    ''' <summary>
    ''' Send a message via the core.
    ''' </summary>
    ''' <param name="strMsg">Message text to send.</param>
    ''' <param name="importance">Message importance.</param>
    ''' <param name="component">Core component to represent as message origin.</param>
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


    Private Delegate Function AskFeedbackDelegate(ByVal strMsg As String, _
                             ByVal replystyle As cFeedbackMessage.eReplyStyle, _
                             ByVal defaultreply As cFeedbackMessage.eReply, _
                             ByVal importance As eMessageImportance, _
                             ByVal component As eCoreComponentType) As cFeedbackMessage.eReply

    ''' <summary>
    ''' Ask for user feedback via the core.
    ''' </summary>
    ''' <param name="strMsg">Message prompt to send.</param>
    ''' <param name="replystyle">Allowed feedback replies.</param>
    ''' <param name="defaultreply">Default reply if the core decided to auto-answer the message.</param>
    ''' <param name="importance">Message importance.</param>
    ''' <param name="component">Core component to represent as message origin.</param>
    ''' <returns>A message reply.</returns>
    Public Function AskFeedback(ByVal strMsg As String, _
                            Optional ByVal replystyle As cFeedbackMessage.eReplyStyle = cFeedbackMessage.eReplyStyle.YES_NO, _
                            Optional ByVal defaultreply As cFeedbackMessage.eReply = cFeedbackMessage.eReply.YES, _
                            Optional ByVal importance As eMessageImportance = eMessageImportance.Warning, _
                            Optional ByVal component As eCoreComponentType = eCoreComponentType.Core) As cFeedbackMessage.eReply

        If Me.InvokeRequired() Then
            Dim objReply As Object = Me.Invoke(New AskFeedbackDelegate(AddressOf Me.AskFeedback), _
                                               New Object() {strMsg, replystyle, defaultreply, importance, component})
            Return CType(objReply, cFeedbackMessage.eReply)
        End If

        Dim fmsg As New cFeedbackMessage(strMsg, component, importance, replystyle, eDataTypes.NotSet, defaultreply)
        Me.Core.Messages.SendMessage(fmsg)
        Return fmsg.Reply

    End Function

    ''' <summary>
    ''' Enumerated type, states how a database was loaded.
    ''' </summary>
    Public Enum eLoadSourceType As Integer
        ''' <summary>Database open attempt originated from the internal API.</summary>
        API = 0
        ''' <summary>Database open attempt originated from the command line.</summary>
        CommandLine
        ''' <summary>Database open attempt originated from the MRU list.</summary>
        MRU
        ''' <summary>Database open attempt originated from the user interface.</summary>
        User
    End Enum

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Open Ecopath model from given location.
    ''' </summary>
    ''' <param name="strFileName">Location of the model to open.</param>
    ''' <param name="loadsource">Flag indicating where the load request came from.</param>
    ''' <remarks>This code is designed for strFileName to indicate a path. It should 
    ''' be possible to indicate a database as well. One day...</remarks>
    ''' ---------------------------------------------------------------------------
    Public Function LoadEcopathModel(ByVal strFileName As String, _
                                     ByVal loadsource As eLoadSourceType) As Boolean

        Dim ds As IEwEDataSource = Nothing
        Dim atResult As eDatasourceAccessType = eDatasourceAccessType.Failed_Unknown

        ' Check if target file exists at all before affecting anything
        If Not File.Exists(strFileName) Then

            ' Handle failure
            Select Case loadsource

                Case eLoadSourceType.MRU
                    ' Unable to locate a file from the MRU list: prompt to remove the entry 
                    Dim fmsg As New cFeedbackMessage(String.Format(My.Resources.PROMPT_MODELNOTFOUND_REMOVEMRU, strFileName), _
                                                     eCoreComponentType.DataSource, _
                                                     eMessageImportance.Warning, _
                                                     cFeedbackMessage.eReplyStyle.YES_NO)

                    If Me.Core.Messages.SendMessage(fmsg) Then
                        If (fmsg.Reply = cFeedbackMessage.eReply.YES) Then
                            Me.RemoveRecentFilesSetting(strFileName)
                        End If
                    End If

                Case eLoadSourceType.User, _
                     eLoadSourceType.CommandLine
                    ' Unable to load model, show generic error
                    Dim msg As New cMessage(String.Format(My.Resources.PROMPT_MODELNOTFOUND, strFileName), _
                                            eMessageType.Any, _
                                            eCoreComponentType.DataSource, _
                                            eMessageImportance.Warning)
                    Me.Core.Messages.SendMessage(msg)

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

        Select Case cDataSourceFactory.GetSupportedType(strFileName)
            Case eDataSourceTypes.ACCDB, _
                 eDataSourceTypes.MDB
                If Not CovertToEwE6(strFileName) Then
                    ' #No: EwE6 database? abort
                    Return False
                End If
        End Select

        ' Abort if no new file name given
        If String.IsNullOrEmpty(strFileName) Then Return True

        ' Create datasource on the selected file
        ds = cDataSourceFactory.Create(strFileName)

        If (ds Is Nothing) Then
            Select Case loadsource

                Case eLoadSourceType.MRU
                    Dim fmsg As New cFeedbackMessage(String.Format(My.Resources.PROMPT_INVALIDMODEL_REMOVEMRU, strFileName), _
                                                     eCoreComponentType.DataSource, _
                                                     eMessageImportance.Critical, _
                                                     cFeedbackMessage.eReplyStyle.YES_NO)
                    If Me.Core.Messages.SendMessage(fmsg) Then
                        If (fmsg.Reply = cFeedbackMessage.eReply.YES) Then
                            Me.RemoveRecentFilesSetting(strFileName)
                        End If
                    End If

                Case eLoadSourceType.User, eLoadSourceType.CommandLine
                    Dim msg As New cMessage(String.Format(My.Resources.PROMPT_INVALIDMODEL, strFileName), _
                                            eMessageType.Any, _
                                            eCoreComponentType.DataSource, _
                                            eMessageImportance.Critical)
                    Me.Core.Messages.SendMessage(msg)

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
            Dim strContentLayout As String = My.Settings.ContentLayoutSaveDirectory

            ' Is the directory a valid path? 
            If Directory.Exists(strContentLayout) Then

                ' #Path is valid, proceed
                Dim fi As New FileInfo(strFileName)
                Dim strConfigFilePath As String = Path.Combine(strContentLayout, fi.Name.Replace(".", "_") + ".config")

                ' Does config file exist?
                If File.Exists(strConfigFilePath) Then
                    ' Found config file, let's restore its layout setting
                    m_DockPanel.SuspendLayout(True)
                    Try
                        CloseAllContents()
                        m_DockPanel.LoadFromXml(strConfigFilePath, m_DeserializeDockContent)
                    Catch ex As Exception
                        'FG: Bug fix Mar 21, 2007
                        'LoadFromXML method requires unintialized dock panel, but when the LoadFromXML
                        'generates the exception and the exception is being caught, the closed
                        'Dock panel needs to be reinitialized.
                        InitDockPanelPositions()
                        ' Operation is not sucessful, delete the file instead
                        If File.Exists(strConfigFilePath) Then
                            File.Delete(strConfigFilePath)
                        End If
                    End Try
                    m_DockPanel.ResumeLayout(True, True)
                End If
            End If

            ' JS 08Aug07: Whatever happened, at least the default node needs to be visible.
            '             This also overcomes bug 133 (see bug description in ActivateForm). The Dock engine
            '             may create forms from crippled XML settings where a doc parent section is missing.
            '             Such forms get instantiated but GetContentFromPersistentString never gets called
            '             because the forms are not content as far as the dock engine is concerned. Nice!
            '             This logic makes sure that at least the default form is properly selected (and indirectly activated)
            Me.EnsureDefaultNodeSelected()
            ' Keep at it, Maurice
            Me.UpdateModelControls()

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
    Public Function SaveEcopathModelAs(ByVal strFileName As String) As Boolean

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
    Public Function CreateEcopathModel(ByVal strFileName As String, _
                                       ByVal strModelName As String, _
                                       ByVal format As eDataSourceTypes) As cEwEDatabase

        Dim db As cEwEDatabase = Nothing
        Dim atResult As eDatasourceAccessType = eDatasourceAccessType.Failed_Unknown
        Dim msg As cMessage = Nothing

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
                msg = New cMessage(String.Format(My.Resources.PROMPT_MODELCREATED, strFileName), _
                    eMessageType.Any, _
                    eCoreComponentType.DataSource, eMessageImportance.Information)

                ' Hackety-hack: destroy any layout file for this model
                Dim fi As New FileInfo(strFileName)
                Dim strName As String = FileUtilities.ToValidFileName(CStr(fi.Name & ".config"), True)
                Dim strConfigFile As String = Path.Combine(My.Settings.ContentLayoutSaveDirectory, strName)

                Try
                    If File.Exists(strConfigFile) Then File.Delete(strConfigFile)
                Catch ex As Exception
                    ' Woops
                End Try

            Case eDatasourceAccessType.Failed_CannotSave
                msg = New cMessage(String.Format(My.Resources.PROMPT_INVALIDTARGETPATH, strFileName), _
                    eMessageType.Any, _
                    eCoreComponentType.DataSource, _
                    eMessageImportance.Critical)
                db = Nothing

            Case eDatasourceAccessType.Failed_OSUnsupported
                msg = New cMessage(My.Resources.PROMPT_DRIVERERROR, _
                    eMessageType.Any, _
                    eCoreComponentType.DataSource, _
                    eMessageImportance.Critical)
                db = Nothing

            Case eDatasourceAccessType.Failed_UnknownType
                msg = New cMessage(My.Resources.PROMPT_INVALIDFILE, _
                    eMessageType.Any, _
                    eCoreComponentType.DataSource, _
                    eMessageImportance.Critical)
                db = Nothing

            Case eDatasourceAccessType.Failed_DeprecatedOperation
                msg = New cMessage(My.Resources.PROMPT_FILETYPEDEPRECATED, _
                    eMessageType.Any, _
                    eCoreComponentType.DataSource, _
                    eMessageImportance.Critical)

            Case eDatasourceAccessType.Failed_Unknown
                msg = New cMessage(String.Format(My.Resources.PROMPT_CREATE_GENERICERROR, strFileName), _
                    eMessageType.Any, _
                    eCoreComponentType.DataSource, _
                    eMessageImportance.Warning)
                db = Nothing

        End Select

        If (msg IsNot Nothing) Then Me.Core.Messages.SendMessage(msg)

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
    Public Function CreateEcopathModel(ByVal strFileName As String, _
                                       ByVal strModelName As String) As cEwEDatabase
        Return Me.CreateEcopathModel(strFileName, _
                                     strModelName, _
                                     cDataSourceFactory.GetSupportedType(strFileName))
    End Function

    Private Delegate Sub SetStatusTextDelegate(ByVal strText As String, _
        ByVal tsUseWaitCursor As TriState, _
        ByVal sProgress As Single)

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

        ' Give app a chance to render
        Application.DoEvents()

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

#End Region ' Public interfaces

#Region " Form overrides "

    ''' <summary>
    ''' </summary>
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
        MRUHelper.BugFix(al)
        My.Settings.MdbRecentlyUsedList = al

        ' Initialize app components
        Me.m_applicationComponents = New ApplicationComponents()

        ' Peeks at key but does not consume it
        Me.KeyPreview = True

        Me.InitCoreParams()
        Me.InitCommands()
        Me.InitPanels()
        Me.InitEventHandlers()
        Me.InitHelp()

        AddHandler Me.Core.StateMonitor.CoreExecutionStateEvent, AddressOf OnCoreExecutionStateChanged

        Me.InitDockPanelPositions()
        'Show start page
        Me.m_StartPage.Show(Me.m_DockPanel, DockState.Document)

        ' Set app caption
        Me.Text = Me.GetApplicationCaption()
        ' Start controlling the status strip
        Me.m_ssMain.Connect(Me.Core)
        ' Start controlling forms
        Me.m_FormStateHelper = New cEwEFormStateHelper(Me.Core.StateMonitor, Me.m_DockPanel)

        ' Load plugins once GUI has been created.
        Me.LoadPlugins()
        ' Auto-launch plugins
        Me.AutolaunchPlugins()

        Me.ProcessCommandLine()
        Me.DefaultSettingLoadedEventHandler(Nothing, Nothing) ' Ugh!
        Me.UpdateModelControls()

    End Sub

    ''' <summary>
    ''' Event handler, catches the form closing event to make sure the core is finalized.
    ''' Application shut-down is cancelled if the core does not finalize correctly.
    ''' </summary>
    Protected Overrides Sub OnFormClosing(ByVal e As System.Windows.Forms.FormClosingEventArgs)

        ' Cancel application shut down if the core does not terminate succesfully.
        e.Cancel = Not Me.Core.CloseModel()

        ' The core does not terminate sucessfully
        If e.Cancel = True Then Return

        ' Save form settings
        Me.SaveMainFormSettings()

        ' Cleanup: disconnect command handler from idle event
        Dim cmdh As cCommandHandler = cCommandHandler.GetInstance()
        RemoveHandler Application.Idle, AddressOf cmdh.OnIdle

        RemoveHandler Me.Core.StateMonitor.CoreExecutionStateEvent, AddressOf OnCoreExecutionStateChanged

        MyBase.OnFormClosing(e)

    End Sub

    Protected Overrides Sub OnResizeEnd(ByVal e As System.EventArgs)
        Me.UpdateModelPathText(Me.SelectedFileName)
    End Sub

#End Region ' Form overrides

#Region " Internal implementation "

    Private Sub InitCommands()

        Dim cmdh As cCommandHandler = cCommandHandler.GetInstance()

        ' Create and configure File Open command
        Me.m_cmdFileOpen = cFileOpenCommand.GetInstance()
        cmdh.Add(Me.m_cmdFileOpen)

        ' Create and configure File Save command
        Me.m_cmdFileSave = cFileSaveCommand.GetInstance()
        cmdh.Add(Me.m_cmdFileSave)

        ' Create and configure Directory Open command
        Me.m_cmdDirectoryOpen = cDirectoryOpenCommand.GetInstance()
        cmdh.Add(Me.m_cmdDirectoryOpen)

        ' Create and configure Execute command
        Me.m_cmdExecute = cExecuteCommand.GetInstance()
        cmdh.Add(Me.m_cmdExecute)

        ' Create and configure new command
        m_cmdNewModel = New cCommand("NewEcopathModel")
        m_cmdNewModel.AddControl(Me.m_tsmiFileNew)
        cmdh.Add(Me.m_cmdNewModel)

        ' Create and configure open command
        m_cmdLoadModel = New cCommand("LoadEcopathModel")
        m_cmdLoadModel.AddControl(Me.m_tsmiFileOpen)
        m_cmdLoadModel.AddControl(Me.m_tsbEcopath)
        cmdh.Add(Me.m_cmdLoadModel)

        ' Create and configure save commands
        m_cmdSaveModelAs = New cCommand("SaveModelAs")
        m_cmdSaveModelAs.AddControl(Me.m_tsmiFileSaveAs)
        cmdh.Add(Me.m_cmdSaveModelAs)

        m_cmdSave = New cCommand("SaveModel")
        m_cmdSave.AddControl(Me.m_tsmiFileSave)
        cmdh.Add(Me.m_cmdSave)

        ' Create and configure 'close model' command
        m_cmdCloseModel = New cCommand("CloseModel")
        m_cmdCloseModel.AddControl(Me.m_tsmiFileClose)
        cmdh.Add(Me.m_cmdCloseModel)

        ' Create and configure 'compact model' command
        m_cmdCompactModel = New cCommand("CompactModel")
        m_cmdCompactModel.AddControl(Me.m_tsmiFileCompact)
        cmdh.Add(Me.m_cmdCompactModel)

        ' Create and configure 'close document' command
        m_cmdCloseDocument = New cCommand("CloseDocument")
        m_cmdCloseDocument.AddControl(Me.m_tsmiWindowsClose)
        cmdh.Add(Me.m_cmdCloseDocument)

        ' Create and configure navigate command
        m_cmdNavigate = New cNavigationCommand()
        cmdh.Add(Me.m_cmdNavigate)

        ' Create and configure 'close all forms' command
        m_cmdCloseAllForms = New cCommand("CloseAllForms")
        m_cmdCloseAllForms.AddControl(Me.m_tsmiWindowsCloseAll)
        cmdh.Add(Me.m_cmdCloseAllForms)

        'Create and configure 'new ecosim scenario' command
        m_cmdNewEcosimScenario = New cCommand("NewEcosimScenario")
        m_cmdNewEcosimScenario.AddControl(Me.m_tsmiEcosimNew)
        cmdh.Add(Me.m_cmdNewEcosimScenario)

        'Create and configure 'load ecosim scenario' command
        m_cmdLoadEcosimScenario = New cCommand("LoadEcosimScenario")
        m_cmdLoadEcosimScenario.AddControl(Me.m_tsmiEcosimLoad)
        m_cmdLoadEcosimScenario.AddControl(Me.m_tsbEcosim)
        cmdh.Add(Me.m_cmdLoadEcosimScenario)

        'Create and configure 'save ecosim scenario' command
        m_cmdSaveEcosimScenario = New cCommand("SaveEcosimScenario")
        m_cmdSaveEcosimScenario.AddControl(Me.m_tsmiEcosimSave)
        cmdh.Add(Me.m_cmdSaveEcosimScenario)

        'Create and configure 'save ecosim scenario as' command
        m_cmdSaveEcosimScenarioAs = New cCommand("SaveEcosimScenarioAs")
        m_cmdSaveEcosimScenarioAs.AddControl(Me.m_tsmiEcosimSaveAs)
        cmdh.Add(Me.m_cmdSaveEcosimScenarioAs)

        'Create and configure 'new ecospace scenario' command
        m_cmdNewEcospaceScenario = New cCommand("NewEcospaceScenario")
        m_cmdNewEcospaceScenario.AddControl(Me.m_tsmiEcospaceNew)
        cmdh.Add(Me.m_cmdNewEcospaceScenario)

        'Create and configure 'load ecospace scenario' command
        m_cmdLoadEcospaceScenario = New cCommand("LoadEcospaceScenario")
        m_cmdLoadEcospaceScenario.AddControl(Me.m_tsmiEcospaceLoad)
        m_cmdLoadEcospaceScenario.AddControl(Me.m_tsbEcospace)
        cmdh.Add(Me.m_cmdLoadEcospaceScenario)

        'Create and configure 'save ecospace scenario' command
        m_cmdSaveEcospaceScenario = New cCommand("SaveEcospaceScenario")
        m_cmdSaveEcospaceScenario.AddControl(Me.m_tsmiEcospaceSave)
        cmdh.Add(Me.m_cmdSaveEcospaceScenario)

        'Create and configure 'save ecospace scenario as' command
        m_cmdSaveEcospaceScenarioAS = New cCommand("SaveEcospaceScenarioAs")
        m_cmdSaveEcospaceScenarioAS.AddControl(Me.m_tsmiEcospaceSaveAs)
        cmdh.Add(Me.m_cmdSaveEcospaceScenarioAS)

        'Create and configure 'new ecotracer scenario' command
        Me.m_cmdNewEcotracerScenario = New cCommand("NewEcotracerScenario")
        Me.m_cmdNewEcotracerScenario.AddControl(Me.m_tsmiEcotracerNew)
        cmdh.Add(Me.m_cmdNewEcotracerScenario)

        'Create and configure 'load ecotracer scenario' command
        m_cmdLoadEcotracerScenario = New cCommand("LoadEcotracerScenario")
        m_cmdLoadEcotracerScenario.AddControl(Me.m_tsmiEcotracerLoad)
        cmdh.Add(Me.m_cmdLoadEcotracerScenario)

        'Create and configure 'save ecotracer scenario' command
        m_cmdSaveEcotracerScenario = New cCommand("SaveEcotracerScenario")
        m_cmdSaveEcotracerScenario.AddControl(Me.m_tsmiEcotracerSave)
        cmdh.Add(Me.m_cmdSaveEcotracerScenario)

        'Create and configure 'save ecotracer scenario as' command
        m_cmdSaveEcotracerScenarioAS = New cCommand("SaveEcotracerScenarioAs")
        m_cmdSaveEcotracerScenarioAS.AddControl(Me.m_tsmiEcotracerSaveAs)
        cmdh.Add(Me.m_cmdSaveEcotracerScenarioAS)

        'Create and configure 'view Navtree' command
        Me.m_cmdViewNavPane = New cCommand("ViewNavPane")
        Me.m_cmdViewNavPane.AddControl(Me.m_tsmiViewNavigation)
        cmdh.Add(Me.m_cmdViewNavPane)

        'Create and configure 'view start page' command
        Me.m_cmdViewStartPanel = New cCommand("ViewStartPage")
        Me.m_cmdViewStartPanel.AddControl(Me.m_tsmiViewStartPage)
        cmdh.Add(Me.m_cmdViewStartPanel)

        'Create and configure 'view status pane' command
        Me.m_cmdViewStatusPane = New cCommand("ViewStatusPane")
        Me.m_cmdViewStatusPane.AddControl(Me.m_tsmiViewStatus)
        cmdh.Add(Me.m_cmdViewStatusPane)

        'Create and configure 'view properties pane' command
        Me.m_cmdViewRemarkPane = New cCommand("ViewPropertiesPane")
        Me.m_cmdViewRemarkPane.AddControl(Me.m_tsmiViewRemarks)
        cmdh.Add(Me.m_cmdViewRemarkPane)

        'Create and configure 'view Buttonbar' command
        Me.m_cmdViewModelBar = New cCommand("ViewButtonBar")
        Me.m_cmdViewModelBar.AddControl(Me.m_tsmiViewModelBar)
        cmdh.Add(Me.m_cmdViewModelBar)

        'Create and configure 'view statusbar' command
        Me.m_cmdViewStatusbar = New cCommand("ViewStatusbar")
        Me.m_cmdViewStatusbar.AddControl(Me.m_tsmiViewStatusBar)
        cmdh.Add(Me.m_cmdViewStatusbar)

        'Create and configure EditGroups command
        Me.m_cmdEditGroups = New cCommand("EditGroups")
        Me.m_cmdEditGroups.AddControl(Me.m_tsmiEcopathEditGroups)
        cmdh.Add(Me.m_cmdEditGroups)

        'Create and configure EditMultiStanza cammand
        Me.m_cmdEditMultiStanza = New cCommand("EditMultiStanza")
        Me.m_cmdEditMultiStanza.AddControl(Me.m_tsmiEcopathEditMultiStanza)
        cmdh.Add(Me.m_cmdEditMultiStanza)

        'Create and configure EditFleets command
        Me.m_cmdEditFleets = New cCommand("EditFleets")
        Me.m_cmdEditFleets.AddControl(Me.m_tsmiEcopathEditFleets)
        cmdh.Add(Me.m_cmdEditFleets)

        Me.m_cmdEditBasemap = New cCommand("EditBasemap")
        Me.m_cmdEditBasemap.AddControl(Me.m_tsmiEcospaceEditMap)
        cmdh.Add(Me.m_cmdEditBasemap)

        Me.m_cmdEditHabitats = New cCommand("EditHabitats")
        Me.m_cmdEditHabitats.AddControl(Me.m_tsmiEcospaceEditHabitats)
        cmdh.Add(Me.m_cmdEditHabitats)

        Me.m_cmdEditRegions = New cCommand("EditRegions")
        Me.m_cmdEditRegions.AddControl(Me.m_tsmiEcospaceEditRegions)
        cmdh.Add(Me.m_cmdEditRegions)

        Me.m_cmdEditMPAs = New cCommand("EditMPAs")
        Me.m_cmdEditMPAs.AddControl(Me.m_tsmiEcospaceEditMPAs)
        cmdh.Add(Me.m_cmdEditMPAs)

        Me.m_cmdEditImportanceLayers = New cCommand("EditImportanceLayers")
        Me.m_cmdEditImportanceLayers.AddControl(Me.m_tsmiEcospaceEditImportanceLayers)
        cmdh.Add(Me.m_cmdEditImportanceLayers)

        Me.m_cmdImportLayerData = New cCommand("ImportLayerData")
        Me.m_cmdImportLayerData.AddControl(Me.m_tsmiEcospaceImportLayers)
        cmdh.Add(Me.m_cmdImportLayerData)

        Me.m_cmdExportLayerData = New cCommand("ExportLayerData")
        cmdh.Add(Me.m_cmdExportLayerData)

        'Create and configure ImportTimeSeries command
        Me.m_cmdImportTimeSeries = New cCommand("ImportTimeSeries")
        Me.m_cmdImportTimeSeries.AddControl(Me.m_tsmiTimeSeriesImport)
        cmdh.Add(Me.m_cmdImportTimeSeries)

        'Create and configure LoadTimeSeries command
        Me.m_cmdLoadTimeSeries = New cCommand("LoadTimeSeries")
        Me.m_cmdLoadTimeSeries.AddControl(Me.m_tsmiTimeSeriesLoad)
        cmdh.Add(Me.m_cmdLoadTimeSeries)

        'Create and configure WeightTimeSeries command
        Me.m_cmdWeightTimeSeries = New cCommand("WeightTimeSeries")
        Me.m_cmdWeightTimeSeries.AddControl(Me.m_tsmiTimeSeriesEditWeights)
        cmdh.Add(Me.m_cmdWeightTimeSeries)

        'Create and configure LoadApplyTimeSeries command
        Me.m_cmdLoadWeightTimeSeries = New cCommand("LoadWeightTimeSeries")
        Me.m_cmdLoadWeightTimeSeries.AddControl(Me.m_tsmiTimeSeriesReloadLast)
        cmdh.Add(Me.m_cmdLoadWeightTimeSeries)

        'Create and configure ExportTimeSeries command
        Me.m_cmdExportTimeSeries = New cCommand("ExportTimeSeries")
        Me.m_cmdExportTimeSeries.AddControl(Me.m_tsmiTimeSeriesExport)
        cmdh.Add(Me.m_cmdExportTimeSeries)

        'Create and configure Help>About command
        Me.m_cmdHelpAbout = New cCommand("HelpAbout")
        Me.m_cmdHelpAbout.AddControl(Me.m_tsmiHelpAbout)
        cmdh.Add(Me.m_cmdHelpAbout)

        ' Create plugin gui command for GUI plugins to use
        Me.m_cmdPluginGUICommand = New PluginGUICommand()
        cmdh.Add(Me.m_cmdPluginGUICommand)

        ' Create the one and only selection command
        Me.m_cmdPropertySelection = New PropertySelectionCommand()
        cmdh.Add(Me.m_cmdPropertySelection)

        Me.m_cmdDisplayGroups = New cDisplayGroupsCommand()
        cmdh.Add(Me.m_cmdDisplayGroups)

        Me.m_cmdEnableEcotracer = New cCommand("EnableEcotracer")
        cmdh.Add(Me.m_cmdEnableEcotracer)

        Me.m_cmdEstimateVs = New cCommand("EstimateVs")
        Me.m_cmdEstimateVs.AddControl(Me.m_tsmiEcosimEstimateVs)
        cmdh.Add(Me.m_cmdEstimateVs)

        Me.m_cmdExportBiomassToCSV = New cCommand("ExportEcosimBiomassToCSV")
        cmdh.Add(Me.m_cmdExportBiomassToCSV)

        ' Listen to application Idle events to update command states
        AddHandler Application.Idle, AddressOf cmdh.OnIdle

    End Sub

    Private Sub InitPanels()

        m_DeserializeDockContent = New DeserializeDockContent(AddressOf GetContentFromPersistentString)
        ' Init panels
        m_NavPanel = New NavigationPanel(Me.Core, Me.m_pluginManager)
        m_StatusPanel = New StatusPanel()
        m_RemarkPanel = New RemarkPanel()
        m_StartPage = New WebBrowserDC()

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

        Me.UIContext = New cUIContext(cCore.GetInstance(), _
                                      cStyleGuide.GetInstance(), _
                                      cPropertyManager.GetInstance(), _
                                      cCommandHandler.GetInstance())

        ' Config state monitor
        Me.Core.StateMonitor.SyncObject = Me

        ' Create plugin manager for this GUI
        Me.m_pluginManager = New cPluginManager()
        ' Distribute plugin manager
        Me.Core.PluginManager = Me.m_pluginManager
        Me.m_pluginManager.Core = Me.Core
        ' Create plugin menu handler to position plugin menu items in the main menu from this form
        Me.m_pluginMenuHandler = New cPluginMenuHandler(Me.MainMenuStrip, Me.m_pluginManager)

        ' Initialize core controller
        Me.m_coreController = New cCoreController(Me.Core.StateMonitor, Me.Core.StateManager)

        ' Initialize style guide updater
        Me.m_styleguideupdater = New StyleGuideUpdater(Me.Core, cStyleGuide.GetInstance())
        Me.m_styleguideupdater.Load()

    End Sub

    Private Sub AutolaunchPlugins()
        Dim pl As New cPluginAutolaunchHandler(Me.m_pluginManager)
    End Sub

    Private Sub LoadPlugins()

        Dim alDisabledPlugins As ArrayList = My.Settings.DisabledPlugins
        Dim msg As cMessage = Nothing

        Me.m_pluginManager.LoadPlugins()

        ' Set up settings for disabling plug-ins
        If alDisabledPlugins Is Nothing Then
            alDisabledPlugins = New ArrayList()
        End If

        ' For every plug-in
        For Each pa As cPluginAssembly In Me.m_pluginManager.PluginAssemblies
            ' Not an 'always on' assembly?
            If pa.AlwaysEnabled = False Then
                ' Disabled?
                pa.Enabled = (alDisabledPlugins.IndexOf(pa.Filename) = -1)
            End If

            ' Check for enabled and incompatible plug-ins
            If pa.Enabled Then

                msg = Nothing

                Select Case pa.Compatibility

                    Case cPluginAssembly.ePluginCompatibilityTypes.VersionCompatible
                        ' NOP

                    Case cPluginAssembly.ePluginCompatibilityTypes.VersionCompatibleCaution
                        msg = New cMessage(String.Format(My.Resources.PROMPT_PLUGIN_CAUTION, pa.Filename), _
                                           eMessageType.Any, eCoreComponentType.External, eMessageImportance.Warning)

                    Case cPluginAssembly.ePluginCompatibilityTypes.VersionIncompatible
                        msg = New cFeedbackMessage(String.Format(My.Resources.PROMPT_PLUGIN_INCOMPATIBLE, pa.Filename), _
                                           eCoreComponentType.External, eMessageImportance.Warning, cFeedbackMessage.eReplyStyle.YES_NO)

                    Case cPluginAssembly.ePluginCompatibilityTypes.IncompatibleUndetermined
                        msg = New cFeedbackMessage(String.Format(My.Resources.PROMPT_PLUGIN_UNDETERMINED, pa.Filename), _
                                           eCoreComponentType.External, eMessageImportance.Warning, cFeedbackMessage.eReplyStyle.YES_NO)

                End Select

                ' Has a message to send?
                If msg IsNot Nothing Then
                    ' #Yes: Send message
                    Me.Core.Messages.SendMessage(msg)
                    ' Feedback required?
                    If TypeOf (msg) Is cFeedbackMessage Then
                        ' #Yes: if replied with 'yes'
                        If DirectCast(msg, cFeedbackMessage).Reply = cFeedbackMessage.eReply.YES Then
                            ' #Yes: disable the plug-in
                            pa.Enabled = False
                            alDisabledPlugins.Add(pa.Filename)
                        End If
                    End If
                End If

            End If

        Next

        My.Settings.DisabledPlugins = alDisabledPlugins
        My.Settings.Save()

    End Sub

    Private Sub InitEventHandlers()

        AddHandler My.Settings.SettingsLoaded, AddressOf DefaultSettingLoadedEventHandler
        AddHandler m_DockPanel.ActiveDocumentChanged, AddressOf ActiveDocumentChangedEventHandler

    End Sub

    Private Sub InitHelp()
        Me.m_Help = New ApplicationHelp(Me, "UserGuide\EwE6_userguide.chm", "User Interface.htm", "EWE_UsersGuide")
        Me.m_Help.HelpTopic(Me.m_StartPage) = "Ecopath with Ecosim 6 Getting started.htm"
    End Sub

    ''' <summary>
    ''' Private method to close all open child forms PLUS all panels on the parent form.
    ''' </summary>
    Private Sub CloseAllContents()
        m_NavPanel.DockPanel = Nothing
        m_RemarkPanel.DockPanel = Nothing
        m_StatusPanel.DockPanel = Nothing

        'Close all other documents windows
        CloseAllDocuments()
    End Sub

    ''' <summary>
    ''' Private method to close all open child forms of the parent form.
    ''' </summary>
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

        Me.m_NavPanel.SelectedNodeName = ""

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Helper method, tries to activate an opened dock panel or MDI child 
    ''' window.
    ''' </summary>
    ''' <param name="strText">Tab text to find the panel with.</param>
    ''' <returns>True if an existing panel was found.</returns>
    ''' -----------------------------------------------------------------------
    Private Function ActivateForm(ByVal strText As String) As Boolean
        ' Dock settings, loop through current opened 
        For Each cnt As DockContent In m_DockPanel.Contents
            If (String.Compare(cnt.Text, strText, False) = 0) Then
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
                End With
                Return True
            End If
        Next
        ' Failed to find an existing panel with this tab text.
        Return False
    End Function

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Create a form or dock panel for a given type.
    ''' </summary>
    ''' <param name="strText">Text to assign to the form.</param>
    ''' <param name="t"><see cref="Type">Type</see> of the form to create.</param>
    ''' <returns>A <see cref="Form">Form</see>-derived instance, or Nothing if the
    ''' form could not be created.
    ''' </returns>
    ''' ---------------------------------------------------------------------------
    Private Function LoadFormFromType(ByVal strText As String, ByVal t As Type, ByVal state As eCoreExecutionState) As Form

        Dim classObject As Object
        Dim frmNew As Form = Nothing

        If Object.ReferenceEquals(t, Nothing) Then Return Nothing

        ' Test the instance if it loads properly
        Me.SetStatusText(My.Resources.GENERIC_STATUS_LOADINGFORM, TriState.True)
        Try
            classObject = Activator.CreateInstance(t)

            If TypeOf classObject Is DockContent Then
                ' Is dock content
                Dim cnt As DockContent = DirectCast(classObject, DockContent)
                cnt.Text = strText
                cnt.TabText = strText
                frmNew = cnt
            ElseIf TypeOf classObject Is EwEGrid Then
                ' Is a grid
                Dim cnt As DockContent = New frmEwEGrid(DirectCast(classObject, EwEGrid))
                cnt.Text = strText
                cnt.TabText = strText
                frmNew = cnt
            ElseIf TypeOf classObject Is Form Then
                ' Is a generic form
                frmNew = DirectCast(classObject, Form)
                frmNew.Text = strText
            End If

            If TypeOf frmNew Is frmEwE Then
                ' Provide form with state
                DirectCast(frmNew, frmEwE).CoreExecutionState = state
            End If

            If (TypeOf (frmNew) Is IUIElement) Then
                ' Configure new object with UI context
                DirectCast(frmNew, IUIElement).UIContext = Me.UIContext
            End If


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
            Debug.Assert(False, "Creation of Form was not successful.  Please contact help: '" & strText & "' threw exception " & ex.ToString)
        End Try
        Me.SetStatusText("", TriState.False)

        Return frmNew
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

            My.Settings.LastSelectedDirectory = Me.m_strLastSelectedPath
            ' Save the current model layout
            Me.SaveDockPanelLayout()
            'Then close all open documents
            Me.CloseAllDocuments()

            ' Reset components
            Me.m_NavPanel.Reset()
            Me.m_StatusPanel.Reset()

            ' Clear the properties cache
            cPropertyManager.GetInstance().Clear(eCoreComponentType.EcoPath)
            ' Clean up
            GC.Collect()
            ' Redraw everything immediately
            Me.Refresh()
            ' Report succes
            Me.UpdateModelControls()
        End If

        Return True

    End Function

    Private Function CompactModel() As Boolean

        Dim ds As IEwEDataSource = Me.Core.DataSource
        Dim result As eDatasourceAccessType = eDatasourceAccessType.Success
        Dim strFileName As String = Me.SelectedFileName()
        Dim msg As cMessage = Nothing
        Dim bSucces As Boolean = True

        If Me.AskFeedback(My.Resources.PROMPT_MODEL_COMPACT, _
             cFeedbackMessage.eReplyStyle.YES_NO, cFeedbackMessage.eReply.YES) <> cFeedbackMessage.eReply.YES Then
            Return False
        End If

        If Me.CloseEcopathModel() = False Then Return False

        Me.SetStatusText(My.Resources.STATUS_MODEL_COMPACTING, TriState.True)
        result = ds.Compact(strFileName)
        Me.SetStatusText("", TriState.False)

        If result = eDatasourceAccessType.Success Then
            bSucces = Me.LoadEcopathModel(strFileName, eLoadSourceType.API)
            If bSucces Then
                msg = New cMessage(My.Resources.STATUS_MODEL_COMPACT_SUCCESS, _
                                   eMessageType.Any, eCoreComponentType.DataSource, eMessageImportance.Critical)
            Else
                msg = New cMessage(My.Resources.STATUS_MODEL_COMPACT_RELOADFAIL, _
                                   eMessageType.Any, eCoreComponentType.DataSource, eMessageImportance.Critical)
            End If
        Else
            ' Report error
            Select Case result
                Case eDatasourceAccessType.Failed_OSUnsupported
                    msg = New cMessage(My.Resources.STATUS_MODEL_COMPACTING_OS, _
                                       eMessageType.Any, eCoreComponentType.DataSource, eMessageImportance.Critical)
                Case eDatasourceAccessType.Failed_CannotSave
                    msg = New cMessage(My.Resources.STATUS_MODEL_COMPACTING_TEMPFILE, _
                                       eMessageType.Any, eCoreComponentType.DataSource, eMessageImportance.Critical)
                Case eDatasourceAccessType.Failed_FileNotFound
                Case eDatasourceAccessType.Failed_Unknown
                    msg = New cMessage(My.Resources.STATUS_MODEL_COMPACTING_FAILED, _
                                       eMessageType.Any, eCoreComponentType.DataSource, eMessageImportance.Critical)
            End Select
            bSucces = False
        End If

        ' Send error
        Me.Core.Messages.SendMessage(msg)

        Return bSucces

    End Function

    Private Sub UpdateModelControls()

        Dim mnuItem As ToolStripMenuItem = Nothing

        ' Clear the dropdown items first
        Me.m_tsbEcosim.DropDownItems.Clear()
        ' Clear the dropdown items first
        Me.m_tsbEcospace.DropDownItems.Clear()

        'Load Ecosim scenarios.
        If Me.Core.StateMonitor.HasEcopathLoaded() Then
            If Me.Core.EcosimScenarioCount > 0 Then
                For i As Integer = 1 To Me.Core.EcosimScenarioCount
                    mnuItem = New ToolStripMenuItem()
                    mnuItem.Text = Me.Core.EcosimScenarios(i).Name
                    mnuItem.Tag = i
                    AddHandler mnuItem.Click, AddressOf EcosimScenarioClickEventHandler
                    Me.m_tsbEcosim.DropDownItems.Add(mnuItem)
                Next
            End If

            'Load Ecospace scenarios
            If Me.Core.EcospaceScenarioCount > 0 Then
                For i As Integer = 1 To Me.Core.EcospaceScenarioCount
                    mnuItem = New ToolStripMenuItem()
                    mnuItem.Text = Me.Core.EcospaceScenarios(i).Name
                    mnuItem.Tag = i
                    AddHandler mnuItem.Click, AddressOf EcospaceScenarioClickEventHandler
                    Me.m_tsbEcospace.DropDownItems.Add(mnuItem)
                Next
            End If
        End If

        Me.UpdateModelPathText(Me.SelectedFileName)

        Me.Text = GetApplicationCaption()

    End Sub

    Private Sub UpdateModelPathText(ByVal strText As String)
        Me.m_tsbModel.Text = cStringUtils.TruncatePath(strText, Me.m_tsbModel.Font, Me.m_tsbModel.Width)
        Me.m_tsbModel.Visible = Not String.IsNullOrEmpty(strText)
    End Sub

    Private Function GetApplicationCaption() As String
        If String.IsNullOrEmpty(Me.SelectedFileName) Then
            Return String.Format(My.Resources.GENERIC_CAPTION)
        Else
            Return String.Format(My.Resources.GENERIC_CAPTION_OPENMODEL, Me.SelectedFileName(False))
        End If
    End Function

    Private Sub ReportFileAccessError(ByVal atResult As eDatasourceAccessType, ByVal strFileName As String)

        Dim msg As cMessage = Nothing

        Select Case atResult
            Case eDatasourceAccessType.Failed_OSUnsupported
                msg = New cMessage(String.Format(My.Resources.STATUS_MODEL_ACCESS_OS, strFileName), _
                                   eMessageType.Any, eCoreComponentType.DataSource, eMessageImportance.Warning)
            Case eDatasourceAccessType.Failed_FileNotFound
                msg = New cMessage(String.Format(My.Resources.STATUS_MODEL_ACCESS_404, strFileName), _
                                   eMessageType.Any, eCoreComponentType.DataSource, eMessageImportance.Warning)
            Case eDatasourceAccessType.Failed_CannotSave
                msg = New cMessage(String.Format(My.Resources.STATUS_MODEL_SAVE_404, strFileName), _
                                   eMessageType.Any, eCoreComponentType.DataSource, eMessageImportance.Warning)
            Case Else
                msg = New cMessage(String.Format(My.Resources.STATUS_MODEL_ACCESS_FAILED, strFileName), _
                                   eMessageType.Any, eCoreComponentType.DataSource, eMessageImportance.Warning)
        End Select

        Me.Core.Messages.SendMessage(msg)

    End Sub

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' To test if it is an EwE5 Access database, if it is, convert it using the 
    ''' database conversion wizard.
    ''' </summary>
    ''' <param name="strFileName">File name of the Access database to convert.</param>
    ''' <returns>True if the database specified by <paramref name="fileName">filename</paramref>
    ''' is already an EwE6 database, or if the conversion was succesful.</returns>
    ''' <remarks>Note that this method only works for Access databases.</remarks>
    ''' ---------------------------------------------------------------------------
    Private Function CovertToEwE6(ByRef strFileName As String) As Boolean

        Dim sVersion As Single = 0.0!
        Dim db As cEwEDatabase = Nothing
        Dim bSucces As Boolean = True

        db = New cEwEAccessDatabase()
        If db.Open(strFileName) = eDatasourceAccessType.Opened Then

            Select Case db.Compatibility()

                Case cEwEDatabase.eCompatibilityTypes.EwE5TooOld
                    Me.SendMessage(My.Resources.PROMPT_ERROR_IMPORT_EWE5_TOO_OLD)
                    bSucces = False

                Case cEwEDatabase.eCompatibilityTypes.EwE5Supported
                    AddRecentFilesSetting(strFileName)

                    Dim dlg As New Import.dlgImportDatabase(Me.UIContext, db)
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

            db.Close()
        End If

        Return bSucces

    End Function

    Private Sub SaveMainFormSettings()

        ' Save the user settings when EwE exits
        My.Settings.LastSelectedDirectory = Me.m_strLastSelectedPath

        Dim fs As cFormPositionSettings = cFormPositionSettings.GetInstance()
        fs.Store(Me, False)

        Me.SaveDockPanelLayout()
        Me.m_styleguideupdater.Save()

        My.Settings.FormPositions = fs.Setting
        My.Settings.Save()

    End Sub

    Private Sub SaveDockPanelLayout()

        'If a model is open, then we save dockpanel Layout settings 
        If Not String.IsNullOrEmpty(Me.SelectedFileName) Then

            ' Get the config file path
            Dim fi As New FileInfo(Me.SelectedFileName)
            Dim name As String = fi.Name.Replace(".", "_") + ".config"

            Dim dstr As String = My.Settings.ContentLayoutSaveDirectory

            'Get the directory where the layout files are stored
            If Not Directory.Exists(dstr) Then
                dstr = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
                My.Settings.ContentLayoutSaveDirectory = dstr
            End If

            Dim configFile As String = Path.Combine(dstr, name)

            If My.Settings.SaveContentLayout Then

                Try
                    ' save the layout
                    ' JS 03aug07: something needs to be done here to ensure that not a single form state is saved as hidden
                    '             to fix bug 133 (http://www.ecopath.org/developers/bugtracker/view.php?id=133)
                    m_DockPanel.SaveAsXml(configFile)
                Catch ex As Exception
                    ' Operation is not sucessful, delete the file instead
                    If File.Exists(configFile) Then
                        File.Delete(configFile)
                    End If
                End Try
            Else
                ' The user prefers not to save the model layout
                If File.Exists(configFile) Then
                    File.Delete(configFile)
                End If
            End If
        End If

    End Sub

    Private Sub AddRecentFilesSetting(ByVal strFileName As String)

        Dim alMDBmru As ArrayList = My.Settings.MdbRecentlyUsedList

        If (alMDBmru Is Nothing) Then Return

        ' Insert at head
        alMDBmru.Insert(0, strFileName)

        ' Remove first occurrence from further down the list
        For iEntry As Integer = 1 To alMDBmru.Count - 2
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
        My.Settings.Save()

    End Sub

    Private Sub RemoveRecentFilesSetting(ByVal strFileName As String)

        Dim alMDBmru As ArrayList = My.Settings.MdbRecentlyUsedList

        If (alMDBmru Is Nothing) Then Return

        ' Remove first occurrence from down the list
        For iEntry As Integer = 0 To alMDBmru.Count - 2
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
        My.Settings.Save()

    End Sub

    Private Sub DisplayFileLists(ByVal menuItem As ToolStripMenuItem, ByVal fileList As ArrayList, ByVal cnt As Integer)

        ' no recently accessed files yet
        If fileList.Count = 1 Then

            If menuItem.DropDownItems.Count = 1 AndAlso _
                   menuItem.DropDownItems.Item(0).Text = My.Resources.GENERIC_VALUE_NONE Then
                Return
            End If

            menuItem.DropDownItems.Clear()
            Dim mnuItem As New ToolStripMenuItem
            mnuItem.Text = My.Resources.GENERIC_VALUE_NONE
            mnuItem.Enabled = False
            menuItem.DropDownItems.Add(mnuItem)
            Return
        End If

        ' Has recent accessed file
        Dim showCnt As Integer = cnt
        If showCnt > fileList.Count - 1 Then
            showCnt = fileList.Count - 1
        End If

        ' Have new recent files, the list needs to be updated.
        menuItem.DropDownItems.Clear()

        For i As Integer = 0 To showCnt - 1
            Dim mnuItem As New ToolStripMenuItem
            Dim str As String() = CStr(fileList.Item(i)).Split(New Char() {";"c})
            mnuItem.Text = String.Format(My.Resources.GENERIC_LABEL_INDEXEDLABEL, i + 1, str(0))
            'Add event handler to invoke the model
            AddHandler mnuItem.Click, AddressOf RecentFileClickEventHandler

            menuItem.DropDownItems.Add(mnuItem)
        Next

    End Sub

    Private Sub ProcessCommandLine()

        Dim astrCmd As String() = cStringUtils.SplitQualified(Microsoft.VisualBasic.Command(), " ")

        If (astrCmd.Length > 0) Then
            If Not String.IsNullOrEmpty(astrCmd(0)) Then
                ' Open the model
                Me.LoadEcopathModel(astrCmd(0).Replace("""", ""), eLoadSourceType.CommandLine)
            End If
        End If

    End Sub

    Private Sub EnsureDefaultNodeSelected()
        ' BAAAAAD!
        If Me.m_NavPanel.SelectedNodeName = "" Then Me.UpdateSelectedNode("Basic input")
    End Sub

    Private Sub UpdateSelectedNode(ByVal strNodeName As String)
        Me.m_NavPanel.SelectedNodeName = strNodeName
    End Sub

    ''' <summary>
    ''' Callback for DockContent deserialization; used to properly resurrect
    ''' forms from a settings file.
    ''' </summary>
    ''' <param name="persistString"></param>
    ''' <returns></returns>
    Private Function GetContentFromPersistentString(ByVal persistString As String) As IDockContent

        Select Case persistString
            Case (GetType(NavigationPanel)).ToString
                Return m_NavPanel
            Case (GetType(StatusPanel)).ToString
                Return m_StatusPanel
            Case (GetType(RemarkPanel)).ToString
                Return m_RemarkPanel
            Case (GetType(WebBrowserDC)).ToString
                If (m_StartPage Is Nothing) Then m_StartPage = New WebBrowserDC()
                Return m_StartPage

            Case Else
                Dim nd As cNavigationCommand = m_NavPanel.GetTemporaryNavCommand(persistString)
                Return CreateDocument(nd)
        End Select

        Return Nothing

    End Function

    Private Function CreateDocument(ByVal nc As cNavigationCommand) As IDockContent

        If nc Is Nothing Then Return Nothing

        Dim frm As Form = Nothing

        ' Check if core can be brought up to par
        If Me.CoreController.LoadPersistState(DirectCast(nc.CoreExecutionState, eCoreExecutionState)) Then
            ' Is form already loaded?
            If Not ActivateForm(nc.PageName) Then
                ' Load instance of form for selected node
                frm = Me.LoadFormFromType(nc.PageName, nc.ClassType, nc.CoreExecutionState)
                ' Was a form created?
                If frm IsNot Nothing Then
                    ' #Yes
                    ' Is this a dockable form? 
                    If TypeOf frm Is DockContent And m_DockPanel.DocumentStyle = DocumentStyle.DockingMdi Then
                        ' #Yes
                        Dim cnt As DockContent = DirectCast(frm, DockContent)
                        ' Show the form in the dock panel
                        Return cnt
                    End If
                Else
                    ' Show form
                    frm.MdiParent = Me
                    frm.Show()
                End If
            End If
        End If

        Return Nothing

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
    ''' 
    ''' </summary>
    ''' <param name="es"></param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Private Function LoadEcosimScenario(ByVal es As cEcoSimScenario) As Boolean

        Dim bSucces As Boolean = False

        If (es IsNot Nothing) Then
            ' #Yes: Load it
            Me.SetStatusText(String.Format(My.Resources.STATUS_ECOSIM_LOADING, es.Name), TriState.True)
            bSucces = Me.Core.LoadEcosimScenario(es)
            Me.SetStatusText("", TriState.False)

            ' Update MRU list
            MRUHelper.UpdateMRUString(My.Settings.MdbRecentlyUsedList, es.Name, MRUHelper.eModuleType.Ecosim)
            My.Settings.Save()

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

            ' Update MRU list
            MRUHelper.UpdateMRUString(My.Settings.MdbRecentlyUsedList, es.Name, MRUHelper.eModuleType.Ecotracer)
            My.Settings.Save()

        End If
        Return bSucces

    End Function

#End Region ' Internal implementation

#Region " Friend interfaces "

#Region " Helpers "

    Private Function FindEcosimScenario(ByVal strName As String) As cEcoSimScenario
        ' Got a scenario name?
        If Not String.IsNullOrEmpty(strName) Then
            ' #Yes: try to find a scenario with this same name
            For i As Integer = 1 To Me.Core.EcosimScenarioCount
                If Me.Core.EcosimScenarios(i).Name = strName Then
                    ' Got it!
                    Return Me.Core.EcosimScenarios(i)
                End If
            Next
        End If
        Return Nothing
    End Function

    Private Function FindEcospaceScenario(ByVal strName As String) As cEcospaceScenario
        ' Got a scenario name?
        If Not String.IsNullOrEmpty(strName) Then
            ' #Yes: try to find a scenario with this same name
            For i As Integer = 1 To Me.Core.EcospaceScenarioCount
                If Me.Core.EcospaceScenarios(i).Name = strName Then
                    ' Got it!
                    Return Me.Core.EcospaceScenarios(i)
                End If
            Next
        End If
        Return Nothing
    End Function

    Private Function FindEcotracerScenario(ByVal strName As String) As cEcotracerScenario
        ' Got a scenario name?
        If Not String.IsNullOrEmpty(strName) Then
            ' #Yes: try to find a scenario with this same name
            For i As Integer = 1 To Me.Core.EcotracerScenarioCount
                If Me.Core.EcotracerScenarios(i).Name = strName Then
                    ' Got it!
                    Return Me.Core.EcotracerScenarios(i)
                End If
            Next
        End If
        Return Nothing
    End Function

#End Region ' Helpers

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Load or reload an Ecosim scenario.
    ''' </summary>
    ''' <param name="bPersist">Flag indicating whether scenario should be obtained from persistent setting.</param>
    ''' <param name="bTryReuse">Flag indicating whether current scenario should reused, not reloaded, if possible.</param>
    ''' <returns>True if succesful.</returns>
    ''' -----------------------------------------------------------------------
    Friend Function LoadEcosimScenario(Optional ByVal bPersist As Boolean = False, _
            Optional ByVal bTryReuse As Boolean = False) As Boolean

        Dim dlg As EcosimScenarioDlg = Nothing
        Dim bSucces As Boolean = False
        Dim strName As String = String.Empty
        Dim es As cEcoSimScenario = Nothing

        ' Try to obtain ecosim scenario to load

        ' Invoked from a command?
        If (Me.m_cmdLoadEcosimScenario.IsInvoking()) Then
            ' #Yes: try to obtain scenario from command
            es = CType(Me.m_cmdLoadEcosimScenario.Tag, cEcoSimScenario)
            ' #No: Are we reloading and an active scenario is present?
        ElseIf (bTryReuse = True) And (Me.Core.ActiveEcosimScenarioIndex >= 0) Then
            '' '' '' Try to reload current scenario
            ' '' ''es = Me.Core.EcosimScenarios(Me.Core.ActiveEcosimScenarioIndex)
            ' '' '' Reuse existing scenario (maybe tell core to reload Ecosim GUI objects?)
            Return True
        End If

        strName = MRUHelper.GetMRUString(My.Settings.MdbRecentlyUsedList, Me.SelectedFileName, MRUHelper.eModuleType.Ecosim)

        ' No scenario found?
        If (es Is Nothing) And (bPersist = True) Then
            ' Try to load persistent scenario from MRU settings if allowed
            es = Me.FindEcosimScenario(strName)
        End If

        ' No scenario found yet?
        If (es Is Nothing) Then
            ' #No scenario: invoke ecosim scenario selection dialog
            dlg = New EcosimScenarioDlg(EcosimScenarioDlg.eDialogModeType.LoadScenario)
            dlg.Scenario = Me.FindEcosimScenario(strName)
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

    ''' <summary>
    ''' Load or reload an Ecospace scenario.
    ''' </summary>
    ''' <param name="bPersist">Flag indicating whether scenario should be obtained from persistent setting.</param>
    ''' <param name="bTryReuse">Flag indicating whether current scenario should reused, not reloaded, if possible.</param>
    ''' <returns>True if succesful.</returns>
    Friend Function LoadEcospaceScenario(ByVal bPersist As Boolean, _
            Optional ByVal bTryReuse As Boolean = False) As Boolean

        Dim dlg As EcospaceScenarioDlg = Nothing
        Dim bSucces As Boolean = False
        Dim strName As String = String.Empty
        Dim es As cEcospaceScenario = Nothing

        ' Try to obtain ecospace scenario to load

        ' Invoked from a command?
        If (Me.m_cmdLoadEcospaceScenario.IsInvoking()) Then
            ' #Yes: try to obtain scenario from command
            es = CType(Me.m_cmdLoadEcospaceScenario.Tag, cEcospaceScenario)
            ' #No: Are we reloading and an active scenario is present?
        ElseIf (bTryReuse = True) And (Me.Core.ActiveEcospaceScenarioIndex >= 0) Then
            '' Try to reload current scenario
            'es = Me.Core.EcospaceScenarios(Me.Core.ActiveEcospaceScenarioIndex)

            ' Reuse existing scenario (maybe tell core to reload Ecosim GUI objects?)
            Return True
        End If

        strName = MRUHelper.GetMRUString(My.Settings.MdbRecentlyUsedList, Me.SelectedFileName, MRUHelper.eModuleType.Ecospace)

        ' No scenario found?
        If (es Is Nothing) And (bPersist = True) Then
            ' Try to load persistent scenario from MRU settings if allowed
            es = Me.FindEcospaceScenario(strName)
        End If

        ' No scenario found yet?
        If (es Is Nothing) Then
            ' #No scenario: invoke ecospace scenario selection dialog
            dlg = New EcospaceScenarioDlg(EcospaceScenarioDlg.eDialogModeType.LoadScenario)
            dlg.Scenario = Me.FindEcospaceScenario(strName)
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
    ''' Load or reload an Ecotracer scenario.
    ''' </summary>
    ''' <param name="bPersist">Flag indicating whether scenario should be obtained from persistent setting.</param>
    ''' <param name="bTryReuse">Flag indicating whether current scenario should reused, not reloaded, if possible.</param>
    ''' <returns>True if succesful.</returns>
    ''' -----------------------------------------------------------------------
    Friend Function LoadEcotracerScenario(Optional ByVal bPersist As Boolean = False, _
            Optional ByVal bTryReuse As Boolean = False) As Boolean

        Dim dlg As EcotracerScenarioDlg = Nothing
        Dim bSucces As Boolean = False
        Dim strName As String = String.Empty
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
            '' Try to reload current scenario
            'es = Me.Core.EcotracerScenarios(Me.Core.ActiveEcotracerScenarioIndex)
            ' Reuse existing scenario (maybe tell core to reload Ecotracer GUI objects?)
            Return True
        End If

        strName = MRUHelper.GetMRUString(My.Settings.MdbRecentlyUsedList, Me.SelectedFileName, MRUHelper.eModuleType.Ecotracer)

        ' No scenario found?
        If (es Is Nothing) And (bPersist = True) Then
            ' Try to load persistent scenario from MRU settings if allowed
            es = Me.FindEcotracerScenario(strName)
        End If

        ' No scenario found yet?
        If (es Is Nothing) Then
            ' #No scenario: invoke ecotracer scenario selection dialog
            dlg = New EcotracerScenarioDlg(EcotracerScenarioDlg.eDialogModeType.LoadScenario)
            dlg.Scenario = Me.FindEcotracerScenario(strName)
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

#End Region ' Friend interfaces

#Region " Command handlers "

#Region " Generic commands "

    Private Sub OnFileOpen(ByVal cmd As cCommand) Handles m_cmdFileOpen.OnInvoke

        Dim dlgLoad As New OpenFileDialog()
        Dim foc As cFileOpenCommand = DirectCast(cmd, cFileOpenCommand)
        Dim strPath As String = foc.Directory

        If String.IsNullOrEmpty(strPath) Then strPath = Me.m_strLastSelectedPath

        cEwEFileDialogHelper.Configure(dlgLoad, foc.Title, foc.FileName, foc.Filters, foc.FilterIndex, strPath, foc.AllowMultiple)

        foc.Result = dlgLoad.ShowDialog()
        foc.FilterIndex = dlgLoad.FilterIndex

        If (foc.Result = Windows.Forms.DialogResult.OK) Then
            foc.FileName = dlgLoad.FileName
            foc.FileNames = dlgLoad.FileNames
            Me.m_strLastSelectedPath = Path.GetDirectoryName(dlgLoad.FileName)
        End If

    End Sub

    Private Sub OnFileSave(ByVal cmd As cCommand) Handles m_cmdFileSave.OnInvoke

        Dim dlgSave As New SaveFileDialog()
        Dim fsc As cFileSaveCommand = DirectCast(cmd, cFileSaveCommand)
        Dim strPath As String = fsc.Directory

        If String.IsNullOrEmpty(strPath) Then strPath = Me.m_strLastSelectedPath

        cEwEFileDialogHelper.Configure(dlgSave, fsc.Title, fsc.FileName, fsc.Filters, fsc.FilterIndex, strPath)

        fsc.Result = dlgSave.ShowDialog()
        If (fsc.Result = Windows.Forms.DialogResult.OK) Then
            fsc.FileName = dlgSave.FileName
            fsc.FilterIndex = dlgSave.FilterIndex
            Me.m_strLastSelectedPath = Path.GetDirectoryName(dlgSave.FileName)
        End If

    End Sub

    Private Sub OnDirectoryOpen(ByVal cmd As cCommand) Handles m_cmdDirectoryOpen.OnInvoke

        Dim dlgLoad As New FolderBrowserDialog()
        Dim doc As cDirectoryOpenCommand = DirectCast(cmd, cDirectoryOpenCommand)
        Dim strPath As String = doc.Directory

        If String.IsNullOrEmpty(strPath) Then strPath = Me.m_strLastSelectedPath

        cEwEFileDialogHelper.Configure(dlgLoad, doc.Description, strPath)

        doc.Result = dlgLoad.ShowDialog()

        If (doc.Result = Windows.Forms.DialogResult.OK) Then
            doc.Directory = dlgLoad.SelectedPath
            Me.m_strLastSelectedPath = Path.GetDirectoryName(doc.Directory)
        End If

    End Sub

    ''' <summary>
    ''' Create new Ecopath model
    ''' </summary>
    Private Sub OnNewFile(ByVal cmd As cCommand) Handles m_cmdNewModel.OnInvoke

        Dim db As cEwEDatabase = Nothing
        Dim cmdh As cCommandHandler = cCommandHandler.GetInstance()
        Dim cmdFS As cFileSaveCommand = DirectCast(cmdh.GetCommand(cFileSaveCommand.COMMAND_NAME), cFileSaveCommand)

        cmdFS.Invoke(My.Resources.DEFAULT_NEWMODELNAME, "", My.Resources.FILEFILTER_MODEL_SAVE, 1)

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
    Private Sub OnUpdateNewFile(ByVal cmd As cCommand) Handles m_cmdNewModel.OnUpdate
        cmd.Enabled = True
    End Sub

    ''' <summary>
    ''' Open Ecopath model from file
    ''' </summary>
    Private Sub OnLoadModel(ByVal cmd As cCommand) Handles m_cmdLoadModel.OnInvoke

        Dim cmdh As cCommandHandler = cCommandHandler.GetInstance()
        Dim cmdFO As cFileOpenCommand = DirectCast(cmdh.GetCommand(cFileOpenCommand.COMMAND_NAME), cFileOpenCommand)

        If cmd.Tag IsNot Nothing Then
            cmdFO.Invoke(Path.GetFileName(CStr(cmd.Tag)), Path.GetDirectoryName(CStr(cmd.Tag)), My.Resources.FILEFILTER_MODEL_OPEN, 1)
        Else
            cmdFO.Invoke(My.Resources.FILEFILTER_MODEL_OPEN, 1)
        End If

        If (cmdFO.Result = DialogResult.OK) Then

            ' Open the model
            Me.SetStatusText(My.Resources.STATUS_ECOPATH_LOADING, TriState.True)
            Me.LoadEcopathModel(cmdFO.FileName, eLoadSourceType.User)
            Me.SetStatusText("", TriState.False)

        End If

    End Sub

    Private Sub OnOpenDocument(ByVal cmd As cCommand) Handles m_cmdNavigate.OnInvoke

        Dim nc As cNavigationCommand = Nothing
        Dim frm As Form = Nothing

        ' Sanity checks
        If cmd Is Nothing Then Return
        If Not (TypeOf cmd Is cNavigationCommand) Then Return

        nc = DirectCast(cmd, cNavigationCommand)

        If nc.PageID = "ndScenario" Then
            m_coreController.LoadEcosimScenario()
            Return
        End If

        If nc.PageID = "ndEcospaceScenario" Then
            m_coreController.LoadEcospaceScenario()
        End If

        If nc.PageID = "ndEcotracerScenario" Then
            Me.CoreController.LoadEcotracerScenario()
        End If

        ' Check if core can be brought up to par
        If Me.CoreController.LoadState(CType(nc.CoreExecutionState, eCoreExecutionState)) Then
            ' Is form already loaded?
            If Not ActivateForm(nc.PageName) Then
                ' Load instance of form for selected node
                frm = Me.LoadFormFromType(nc.PageName, nc.ClassType, nc.CoreExecutionState)
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
                        Me.m_Help.HelpTopic(frm) = nc.HelpURL
                    Else
                        ' Show form
                        frm.MdiParent = Me
                        frm.Show()
                        ' Switch help
                        Me.m_Help.HelpTopic(frm) = nc.HelpURL
                    End If
                End If
            End If
        End If

        ' JS Jan2408: Make sure the nav tree correctly reflects the current selected page.
        ' This is important if the navigation to the requested page failed, which can happen
        ' if the core controller is unable to bring the core to the requested state.
        Me.ActiveDocumentChangedEventHandler(Nothing, Nothing)

    End Sub

    Private Sub OnRunGUIPlugin(ByVal cmd As cCommand) Handles m_cmdPluginGUICommand.OnInvoke

        ' Sanity checks
        If Not (TypeOf cmd Is PluginGUICommand) Then Return

        ' Phew
        Dim pgcmd As PluginGUICommand = DirectCast(cmd, PluginGUICommand)
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
                    Me.m_Help.HelpTopic(pgcmd.Form) = ""
                End If
            End If
        End If
    End Sub

    ''' <summary>
    ''' Save model under a different name
    ''' </summary>
    Private Sub OnSaveModelAs(ByVal cmd As cCommand) Handles m_cmdSaveModelAs.OnInvoke

        Dim cmdh As cCommandHandler = cCommandHandler.GetInstance()
        Dim cmdFS As cFileSaveCommand = DirectCast(cmdh.GetCommand(cFileSaveCommand.COMMAND_NAME), cFileSaveCommand)

        Dim strFileFilter As String = ""

        ' JS 27Jul08: Only able to save in current file format (save as between formats not supported by the core)
        Select Case cDataSourceFactory.GetSupportedType(Me.SelectedFileName)
            Case eDataSourceTypes.MDB
                ' Only allow saving as MDB
                strFileFilter = My.Resources.FILEFILTER_SAVE_MDB
            Case eDataSourceTypes.ACCDB
                ' Only allow saving as ACCDB
                strFileFilter = My.Resources.FILEFILTER_SAVE_ACCDB
            Case Else
                ' Not supported
                Debug.Assert(False, "Option should not have been available")
                Return
        End Select

        cmdFS.Invoke(My.Resources.DEFAULT_NEWMODELNAME, "", strFileFilter)

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
    ''' Save the model
    ''' </summary>
    Private Sub OnSave(ByVal cmd As cCommand) Handles m_cmdSave.OnInvoke
        Me.SetStatusText(My.Resources.STATUS_MODEL_SAVING, TriState.True)
        Me.Core.Save()
        Me.SetStatusText("", TriState.False)
    End Sub

    ''' <summary>
    ''' Update save model command state
    ''' </summary>
    Private Sub OnUpdateSave(ByVal cmd As cCommand) Handles m_cmdSave.OnUpdate
        cmd.Enabled = Me.Core.StateMonitor.IsModified
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
    ''' Command handler; invokes the edit groups interface
    ''' </summary>
    Private Sub OnEditGroups(ByVal cmd As cCommand) Handles m_cmdEditGroups.OnInvoke

        Dim dlg As New EditGroups(Me.UIContext)
        Me.m_Help.HelpTopic(dlg) = "Edit groups.htm"
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
        Dim dlg As New EditMultiStanza(Me.UIContext)
        Me.m_Help.HelpTopic(dlg) = "Edit multi stanza.htm"
        dlg.ShowDialog(Me)
    End Sub

    ''' <summary>
    ''' Command update handler; enables and disables the <see cref="m_cmdEditMultiStanza">Edit Multi-stanza command</see>.
    ''' </summary>
    Private Sub OnUpdateMultiStanza(ByVal cmd As EwEUtils.Commands.cCommand) Handles m_cmdEditMultiStanza.OnUpdate
        ' MultiStanza can be edited when ecopath has loaded and the core has more than one stanza group
        cmd.Enabled = Me.Core.StateMonitor.HasEcopathLoaded() And (Me.Core.nStanzas > 0)
    End Sub

    ''' <summary>
    ''' Command handler; invokes the edit fleets interface
    ''' </summary>
    Private Sub OnEditFleets(ByVal cmd As cCommand) Handles m_cmdEditFleets.OnInvoke
        Dim dlg As New EditFleets(Me.UIContext)
        Me.m_Help.HelpTopic(dlg) = "Edit fleets.htm"
        dlg.ShowDialog(Me)
    End Sub

    ''' <summary>
    ''' Command update handler; enables and disables the <see cref="m_cmdEditFleets">Edit Fleets command</see>.
    ''' </summary>
    Private Sub OnUpdateEditFleets(ByVal cmd As EwEUtils.Commands.cCommand) Handles m_cmdEditFleets.OnUpdate
        cmd.Enabled = Me.Core.StateMonitor.HasEcopathLoaded()
    End Sub

    Private Sub OnDisplayGroups(ByVal cmd As cCommand) Handles m_cmdDisplayGroups.OnInvoke
        Dim dlg As New dlgDisplayGroups(m_cmdDisplayGroups.ShowGroups, m_cmdDisplayGroups.ShowTotals)
        dlg.ShowDialog()
    End Sub

    Private Sub OnUpdateDisplayGroups(ByVal cmd As cCommand) Handles m_cmdDisplayGroups.OnUpdate
        cmd.Enabled = Me.Core.StateMonitor.HasEcopathLoaded()
    End Sub

    ''' <summary>
    ''' Command handler; invokes the import layers dialog.
    ''' </summary>
    Private Sub m_cmdImportLayerData_OnInvoke(ByVal cmd As EwEUtils.Commands.cCommand) _
        Handles m_cmdImportLayerData.OnInvoke
        Dim dlg As New dlgImportLayerData()

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
        Dim dlg As New dlgExportLayerData()
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

    Private Sub RecentMDBToolStripMenuItem_DropDownOpening(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_tsmiFileRecent.DropDownOpening
        DisplayFileLists(m_tsmiFileRecent, My.Settings.MdbRecentlyUsedList, CInt(My.Settings.MdbRecentlyUsedCount))
    End Sub

    Private Sub ExitToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_tsmiFileExit.Click
        Me.Close()
    End Sub

#End Region ' Main Menu - File

#Region " Main Menu - View "

    ''' <summary>
    ''' Command handler; shows the start page.
    ''' </summary>
    Private Sub OnViewStartPage(ByVal cmd As cCommand) Handles m_cmdViewStartPanel.OnInvoke
        ' If m_startPage has been closed, create a new reference. 
        If m_StartPage.IsDisposed() Then
            m_StartPage = New WebBrowserDC()
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
        cmd.Checked = Not m_StartPage.IsDisposed()
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

#End Region ' Main Menu - View

#Region " Main Menu - Tools "
    ''' <summary>
    ''' Open the EwE6 option dialog
    ''' </summary>
    Private Sub OptionsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_tsmiOptions.Click

        Dim dlgOptions As New dlgOptions()
        ' FG: Fixed a bug..Should not use Show instead of using ShowDialog and specify its owner so it will
        ' be displayed at the specified location Nov 15, 2006
        dlgOptions.ShowDialog(Me)

    End Sub

#End Region ' Main Menu - Tools

#Region " Main Menu - Windows "

#End Region ' Main Menu - Tools

#Region " Main Menu - Help "

    ''' <summary>
    ''' Command handler; invokes the About... dialog.
    ''' </summary>
    Private Sub m_cmdHelpAbout_OnInvoke(ByVal cmd As EwEUtils.Commands.cCommand) Handles m_cmdHelpAbout.OnInvoke
        Dim dlgAbout As New frmAboutEwE

        Me.m_Help.HelpTopic(dlgAbout) = ""
        dlgAbout.ShowDialog(Me)
    End Sub

    ''' <summary>
    ''' Command update handler; enables the About.. command.
    ''' </summary>
    Private Sub m_cmdHelpAbout_OnUpdate(ByVal cmd As EwEUtils.Commands.cCommand) Handles m_cmdHelpAbout.OnUpdate
        cmd.Enabled = True
    End Sub

    Private Sub ContentsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_tsmiHelpContents.Click
        Me.m_Help.ShowHelp(HelpNavigator.TableOfContents)
    End Sub

    Private Sub IndexToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_tsmiHelpIndex.Click
        Me.m_Help.ShowHelp(HelpNavigator.KeywordIndex)
    End Sub

    Private Sub SearchToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_tsmiHelpSearch.Click
        Me.m_Help.ShowHelp(HelpNavigator.Find)
    End Sub

    Private Sub ReportBugMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_tsmiHelpBugReport.Click
        BugReporter.InvokeBugReport()
    End Sub

#End Region ' Main Menu - Help

#Region " Ecosim commands "

    ''' <summary>
    ''' Command handler; creates a new Ecosim scenario
    ''' </summary>
    Private Sub OnNewEcosimScenario(ByVal cmd As cCommand) Handles m_cmdNewEcosimScenario.OnInvoke

        Dim dlg As New EcosimScenarioDlg(EcosimScenarioDlg.eDialogModeType.CreateScenario)

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

    Private Sub OnSaveEcosimScenario(ByVal cmd As cCommand) Handles m_cmdSaveEcosimScenario.OnInvoke
        Dim strStatus As String = String.Format(My.Resources.STATUS_ECOSIM_SAVING, Me.Core.EcosimScenarios(Me.Core.ActiveEcosimScenarioIndex).Name)
        Me.SetStatusText(strStatus, TriState.True)
        Try
            Me.Core.SaveEcosimScenario()
        Catch ex As Exception

        End Try
        Me.SetStatusText("", TriState.False)
    End Sub

    ''' <summary>
    ''' Command update handler; enables and disables the 'save ecosim scenario' command
    ''' </summary>
    Private Sub OnUpdateSaveEcosimScenario(ByVal cmd As cCommand) Handles m_cmdSaveEcosimScenario.OnUpdate
        cmd.Enabled = Me.Core.StateMonitor.IsEcosimModified
    End Sub

    Private Sub OnSaveEcosimScenarioAs(ByVal cmd As cCommand) Handles m_cmdSaveEcosimScenarioAs.OnInvoke

        Dim dlg As New EcosimScenarioDlg(EcosimScenarioDlg.eDialogModeType.SaveScenario, _
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
    Private Sub OnUpdateSaveEcosimScenarioAs(ByVal cmd As cCommand) Handles m_cmdSaveEcosimScenarioAs.OnUpdate
        cmd.Enabled = Me.Core.StateMonitor.HasEcosimLoaded
    End Sub

    ''' <summary>
    ''' Command handler; invokes the import time series dialog.
    ''' </summary>
    Private Sub m_cmdImportTimeSeries_OnInvoke(ByVal cmd As EwEUtils.Commands.cCommand) Handles m_cmdImportTimeSeries.OnInvoke
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

        sfd.Filter = My.Resources.FILEFILTER_CSV
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
    ''' Command handler; invokes the load time series dialog.
    ''' </summary>
    Private Sub m_cmdLoadTimeSeries_OnInvoke(ByVal cmd As EwEUtils.Commands.cCommand) Handles m_cmdLoadTimeSeries.OnInvoke
        Me.ManageTimeSeries(dlgManageTimeSeries.eModeType.Load)
    End Sub

    ''' <summary>
    ''' Command update handler; enables and disables the <see cref="m_cmdLoadTimeSeries">Load TimeSeries command</see>.
    ''' </summary>
    Private Sub m_cmdLoadTimeSeries_OnUpdate(ByVal cmd As EwEUtils.Commands.cCommand) Handles m_cmdLoadTimeSeries.OnUpdate
        cmd.Enabled = Me.Core.StateMonitor.HasEcosimLoaded()
    End Sub

    ''' <summary>
    ''' Command handler; invokes the reload time series dialog.
    ''' </summary>
    Private Sub m_cmdReloadTimeSeries_OnInvoke(ByVal cmd As EwEUtils.Commands.cCommand) Handles m_cmdLoadWeightTimeSeries.OnInvoke
        Dim strDataset As String = MRUHelper.GetMRUString(My.Settings.MdbRecentlyUsedList, Me.SelectedFileName, MRUHelper.eModuleType.Dataset)
        For iDS As Integer = 1 To Me.Core.nTimeSeriesDatasets
            If (String.Compare(Me.Core.TimeSeriesDataset(iDS).Name, strDataset, False) = 0) Then
                Me.Core.LoadTimeSeries(iDS, True)
                Exit For
            End If
        Next
    End Sub

    ''' <summary>
    ''' Command update handler; enables and disables the 
    ''' <see cref="m_cmdLoadWeightTimeSeries">Load and weight TimeSeries command</see>.
    ''' </summary>
    Private Sub m_cmdLoadWeightTimeSeries_OnUpdate(ByVal cmd As EwEUtils.Commands.cCommand) Handles m_cmdLoadWeightTimeSeries.OnUpdate
        Dim strDataset As String = MRUHelper.GetMRUString(My.Settings.MdbRecentlyUsedList, Me.SelectedFileName, MRUHelper.eModuleType.Dataset)
        cmd.Enabled = Me.Core.StateMonitor.HasEcosimLoaded() And (Not String.IsNullOrEmpty(strDataset))
    End Sub

    Private Sub OnExportBiomassToCSV(ByVal cmd As cCommand) Handles m_cmdExportBiomassToCSV.OnInvoke
        Dim cmdh As cCommandHandler = cCommandHandler.GetInstance()
        Dim cmdFS As cFileSaveCommand = DirectCast(cmdh.GetCommand(cFileSaveCommand.COMMAND_NAME), cFileSaveCommand)

        cmdFS.Invoke(String.Format("EwE6_{0}_Biomass.csv", Me.Core.EwEModel.Name), "", My.Resources.FILEFILTER_CSV, 1)

        If cmdFS.Result = DialogResult.OK Then
            ' Save the Ecosim model result to .csv files
            Me.Core.dumpEcosimModelResults(cmdFS.FileName)
        End If
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

    Private Sub OnExportBiomassToCSVs(ByVal cmd As cCommand) Handles m_cmdExportBiomassToCSV.OnUpdate
        cmd.Enabled = Me.Core.StateMonitor.HasEcosimRan
    End Sub

#End Region ' Ecosim commands

#Region " Ecospace scenario commands "

    Private Sub OnNewEcospaceScenario(ByVal cmd As cCommand) Handles m_cmdNewEcospaceScenario.OnInvoke
        Dim dlg As New EcospaceScenarioDlg(EcospaceScenarioDlg.eDialogModeType.CreateScenario)

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

    Private Sub OnUpdateNewEcospaceScenario(ByVal cmd As cCommand) Handles m_cmdNewEcospaceScenario.OnUpdate
        cmd.Enabled = Me.Core.StateMonitor.HasEcosimLoaded
    End Sub

    Private Sub OnLoadEcospaceScenario(ByVal cmd As cCommand) Handles m_cmdLoadEcospaceScenario.OnInvoke
        Me.CoreController.LoadEcospaceScenario()
    End Sub

    Private Sub OnUpdateLoadEcospaceScenario(ByVal cmd As cCommand) Handles m_cmdLoadEcospaceScenario.OnUpdate
        cmd.Enabled = Me.Core.StateMonitor.HasEcopathLoaded
    End Sub

    ''' <summary>
    ''' Command handler; saves the current active Ecospace scenario under a new name.
    ''' </summary>
    Private Sub OnSaveEcospaceScenarioAs(ByVal cmd As cCommand) Handles m_cmdSaveEcospaceScenarioAS.OnInvoke

        Dim dlg As New EcospaceScenarioDlg(EcospaceScenarioDlg.eDialogModeType.SaveScenario, _
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
                        Me.SetStatusText(String.Format(My.Resources.STATUS_ECOSIM_SAVING, dlg.ScenarioName), TriState.True)
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
                Me.SetStatusText(String.Format(My.Resources.STATUS_ECOSIM_CREATING, dlg.ScenarioName), TriState.True)
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

    ''' <summary>
    ''' Command handler; saves the current active Ecospace scenario.
    ''' </summary>
    Private Sub OnSaveEcospaceScenario(ByVal cmd As cCommand) Handles m_cmdSaveEcospaceScenario.OnInvoke
        Dim strStatus As String = String.Format(My.Resources.STATUS_ECOSPACE_SAVING, Me.Core.EcospaceScenarios(Me.Core.ActiveEcospaceScenarioIndex).Name)
        Me.SetStatusText(strStatus, TriState.True)
        Try
            Me.Core.SaveEcospaceScenario()
        Catch ex As Exception

        End Try
        Me.SetStatusText("", TriState.False)
    End Sub

    ''' <summary>
    ''' Command update handler; enables and disables the 
    ''' <see cref="m_cmdSaveEcospaceScenario">Save Ecospace Scenario</see> command.
    ''' </summary>
    Private Sub OnUpdateSaveEcospaceScenario(ByVal cmd As cCommand) Handles m_cmdSaveEcospaceScenario.OnUpdate
        cmd.Enabled = Me.Core.StateMonitor.IsEcospaceModified
    End Sub

    ''' <summary>
    ''' Command handler; invokes the Ecospace edit basemap dialog.
    ''' </summary>
    Private Sub OnEditEcospaceBasemap(ByVal cmd As cCommand) Handles m_cmdEditBasemap.OnInvoke
        Dim dlg As New dlgEditBasemap(Me.Core.EcospaceBasemap)
        Me.m_Help.HelpTopic(dlg) = "Edit basemap.htm"
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
        Dim dlg As New dlgEditHabitats()
        Me.m_Help.HelpTopic(dlg) = "Edit habitats.htm"
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
        Dim dlg As New dlgEditRegions()
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
        Dim dlg As New dlgEditMPAs()
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
        Dim dlg As New dlgEditImportanceLayers()
        dlg.ShowDialog(Me)
    End Sub

    ''' <summary>
    ''' Command handler; handles access to the Ecospace edit importance layers dialog.
    ''' </summary>
    Private Sub OnUpdateEditEcospaceImportanceLayers(ByVal cmd As cCommand) Handles m_cmdEditImportanceLayers.OnUpdate
        cmd.Enabled = Me.Core.StateMonitor.HasEcospaceLoaded
    End Sub

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

            ' Update MRU list
            MRUHelper.UpdateMRUString(My.Settings.MdbRecentlyUsedList, es.Name, MRUHelper.eModuleType.Ecospace)
            My.Settings.Save()

        End If
        Return bSucces

    End Function

#End Region ' Ecospace scenario commands

#Region " Ecotracer commands "

    ''' <summary>
    ''' Command handler; creates a new Ecotracer scenario
    ''' </summary>
    Private Sub OnNewEcotracerScenario(ByVal cmd As cCommand) Handles m_cmdNewEcotracerScenario.OnInvoke

        ' Prerequesite: Ecosim needs to be loaded
        Me.CoreController.LoadState(eCoreExecutionState.EcosimLoaded)
        ' Not succesful? abort
        If Not Me.Core.StateMonitor.HasEcosimLoaded Then Return

        Dim dlg As New EcotracerScenarioDlg(EcotracerScenarioDlg.eDialogModeType.CreateScenario)

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
    Private Sub OnUpdateNewEcotracerScenario(ByVal cmd As cCommand) Handles m_cmdNewEcotracerScenario.OnUpdate
        cmd.Enabled = Me.Core.StateMonitor.HasEcopathLoaded
    End Sub

    ''' <summary>
    ''' Command handler; loads a new Ecotracer scenario
    ''' </summary>
    Private Sub OnLoadEcotracerScenario(ByVal cmd As cCommand) Handles m_cmdLoadEcotracerScenario.OnInvoke
        Me.LoadEcotracerScenario()
    End Sub

    ''' <summary>
    ''' Command update handler; takes care of enabling and disabling the 
    ''' <see cref="m_cmdLoadEcotracerScenario">Load Ecotracer Scenario</see> command.
    ''' </summary>
    Private Sub OnUpdateLoadEcotracerScenario(ByVal cmd As cCommand) Handles m_cmdLoadEcotracerScenario.OnUpdate
        cmd.Enabled = Me.Core.StateMonitor.HasEcopathLoaded
    End Sub

    Private Sub OnSaveEcotracerScenario(ByVal cmd As cCommand) Handles m_cmdSaveEcotracerScenario.OnInvoke
        Dim strStatus As String = String.Format(My.Resources.STATUS_ECOTRACER_SAVING, Me.Core.EcotracerScenarios(Me.Core.ActiveEcotracerScenarioIndex).Name)
        Me.SetStatusText(strStatus, TriState.True)
        Me.Core.SaveEcotracerScenario()
        Me.SetStatusText("", TriState.False)
    End Sub

    ''' <summary>
    ''' Command update handler; enables and disables the 'save ecotracer scenario' command
    ''' </summary>
    Private Sub OnUpdateSaveEcotracerScenario(ByVal cmd As cCommand) Handles m_cmdSaveEcotracerScenario.OnUpdate
        cmd.Enabled = Me.Core.StateMonitor.IsEcotracerModified
    End Sub

    Private Sub OnSaveEcotracerScenarioAs(ByVal cmd As cCommand) Handles m_cmdSaveEcotracerScenarioAS.OnInvoke

        Dim dlg As New EcotracerScenarioDlg(EcotracerScenarioDlg.eDialogModeType.SaveScenario, _
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
    Private Sub OnUpdateSaveEcotracerScenarioAs(ByVal cmd As cCommand) Handles m_cmdSaveEcotracerScenarioAS.OnUpdate
        cmd.Enabled = Me.Core.StateMonitor.HasEcotracerLoaded()
    End Sub

    Private Sub OnEnableEcotracer(ByVal cmd As cCommand) Handles m_cmdEnableEcotracer.OnInvoke

        Dim pm As cPropertyManager = cPropertyManager.GetInstance()
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

    Private Sub OnUpdateEnableEcotracer(ByVal cmd As cCommand) Handles m_cmdEnableEcotracer.OnUpdate
        cmd.Enabled = True
    End Sub

#End Region ' Ecotracer commands

#End Region ' Command handlers 

#Region " Event Handlers "

    Private Sub RecentFileClickEventHandler(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim mnuItem As ToolStripMenuItem = CType(sender, ToolStripMenuItem)

        ' Get rid of file index - presuming that there are less than 99 MRU entries!
        Dim fn As String = mnuItem.Text.Substring(3)
        LoadEcopathModel(fn, eLoadSourceType.MRU)
    End Sub

    Private Sub EcosimScenarioClickEventHandler(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim mnuItem As ToolStripMenuItem = CType(sender, ToolStripMenuItem)
        Dim iScenario As Integer = CInt(mnuItem.Tag)

        Me.m_cmdLoadEcosimScenario.Tag = Me.Core.EcosimScenarios(iScenario)
        Me.m_cmdLoadEcosimScenario.Invoke()
        Me.m_cmdLoadEcosimScenario.Tag = Nothing
    End Sub

    Private Sub EcospaceScenarioClickEventHandler(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim mnuItem As ToolStripMenuItem = CType(sender, ToolStripMenuItem)
        Dim iScenario As Integer = CInt(mnuItem.Tag)

        Me.m_cmdLoadEcospaceScenario.Tag = Me.Core.EcospaceScenarios(iScenario)
        Me.m_cmdLoadEcospaceScenario.Invoke()
        Me.m_cmdLoadEcospaceScenario.Tag = Nothing
    End Sub

    Private Sub EcotracerScenarioClickEventHandler(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim mnuItem As ToolStripMenuItem = CType(sender, ToolStripMenuItem)
        Dim iScenario As Integer = CInt(mnuItem.Tag)

        Me.m_cmdLoadEcotracerScenario.Tag = Me.Core.EcotracerScenarios(iScenario)
        Me.m_cmdLoadEcotracerScenario.Invoke()
        Me.m_cmdLoadEcotracerScenario.Tag = Nothing
    End Sub

    Private Sub DefaultSettingLoadedEventHandler(ByVal sender As Object, ByVal e As System.Configuration.SettingsLoadedEventArgs)

        Dim fs As cFormPositionSettings = cFormPositionSettings.GetInstance()

        Me.m_strLastSelectedPath = My.Settings.LastSelectedDirectory
        If Not Directory.Exists(Me.m_strLastSelectedPath) Then
            'the last selected directory is not a valid directory; set it to My documents by default
            Me.m_strLastSelectedPath = My.Computer.FileSystem.SpecialDirectories.MyDocuments
        End If

        ' Read form positions
        fs.Setting = My.Settings.FormPositions

        ' Get the form position from user settings
        Me.StartPosition = FormStartPosition.Manual
        fs.Apply(Me, False)

    End Sub

    Private Sub ActiveDocumentChangedEventHandler(ByVal sender As System.Object, ByVal e As System.EventArgs)

        Dim idc As IDockContent = m_DockPanel.ActiveDocument
        Dim dch As DockContentHandler = Nothing
        Dim strNodeName As String = String.Empty

        If Not Object.ReferenceEquals(idc, Nothing) Then
            dch = idc.DockHandler

            If Not Object.ReferenceEquals(dch, Nothing) Then
                strNodeName = dch.TabText
            End If

            ' Kick core controller
            If (TypeOf idc Is frmEwE) Then
                ' Update core state if possible
                Me.CoreController.LoadState(DirectCast(idc, frmEwE).CoreExecutionState)
            End If
        End If

        ' Update help
        Me.m_Help.ActiveHelpControl = CType(m_DockPanel.ActiveDocument, Control)

        Me.UpdateSelectedNode(strNodeName)
    End Sub

    Private Sub m_tslModelPath_MouseEnter(ByVal sender As Object, ByVal e As System.EventArgs) Handles m_tsbModel.MouseEnter
        Me.m_tsbModel.ForeColor = SystemColors.ControlText
    End Sub

    Private Sub m_tslModelPath_MouseLeave(ByVal sender As Object, ByVal e As System.EventArgs) Handles m_tsbModel.MouseLeave
        Me.m_tsbModel.ForeColor = SystemColors.ControlDark
    End Sub

    Private Sub m_tslModelPath_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_tsbModel.Click
        Me.m_cmdLoadModel.Tag = m_tsbModel.Text
        Me.m_cmdLoadModel.Invoke()
        Me.m_cmdLoadModel.Tag = Nothing
    End Sub

    Private Sub OnCoreExecutionStateChanged(ByVal csm As cCoreStateMonitor)
        Me.UpdateModelControls()
    End Sub

#Region " Key press handlers "

    Private Sub AppLauncher_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        Select Case e.KeyCode
            Case Keys.F12
                ' process Next step
                MsgBox("Bite me")
        End Select
        If e.Alt And e.Control And e.Shift Then
            Select Case e.KeyCode
                Case Keys.Oemtilde
                    m_StartPage.URL = "http://farm1.static.flickr.com/160/374820104_5ec655655c.jpg"
                    m_StartPage.Show(m_DockPanel, DockState.Document)
                Case Keys.D1
                    m_StartPage.URL = "http://farm1.static.flickr.com/82/261884734_01ad1712a6.jpg"
                    m_StartPage.Show(m_DockPanel, DockState.Document)
                Case Keys.D2
                    m_StartPage.URL = "http://farm2.static.flickr.com/1218/536646225_09f93a0b8c.jpg"
                    m_StartPage.Show(m_DockPanel, DockState.Document)
                Case Keys.D3
                    m_StartPage.URL = "http://farm1.static.flickr.com/112/261883295_1cab2a9714.jpg"
                    m_StartPage.Show(m_DockPanel, DockState.Document)
                Case Keys.D4
                    m_StartPage.URL = "http://farm1.static.flickr.com/87/261883288_06e5599f56.jpg"
                    m_StartPage.Show(m_DockPanel, DockState.Document)
                Case Keys.D5
                    m_StartPage.URL = "http://farm1.static.flickr.com/89/261883279_6c8b139ed9.jpg"
                    m_StartPage.Show(m_DockPanel, DockState.Document)
                Case Keys.D6
                    m_StartPage.URL = "http://farm1.static.flickr.com/121/261883269_cf6fd5f287.jpg"
                    m_StartPage.Show(m_DockPanel, DockState.Document)
                Case Keys.D7
                    m_StartPage.URL = "http://farm2.static.flickr.com/1312/1400452382_47306892c0.jpg"
                    m_StartPage.Show(m_DockPanel, DockState.Document)
                Case Keys.D8
                    m_StartPage.URL = "http://farm2.static.flickr.com/1012/1400449350_7dfad8dd60.jpg"
                    m_StartPage.Show(m_DockPanel, DockState.Document)
                Case Keys.D9
                    m_StartPage.URL = "http://farm3.static.flickr.com/2344/1536185215_fe4d413654.jpg"
                    m_StartPage.Show(m_DockPanel, DockState.Document)
                Case Keys.D0
                    m_StartPage.URL = "http://farm1.static.flickr.com/143/377851455_28924928b1.jpg"
                    m_StartPage.Show(m_DockPanel, DockState.Document)
            End Select

        End If
    End Sub

#End Region ' Key press handlers

#End Region ' Event Handlers

#Region " Ecosim "

    Private Sub ManageTimeSeries(ByVal mode As dlgManageTimeSeries.eModeType)

        Dim dlg As New dlgManageTimeSeries(Me.UIContext, mode)

        ' Hmm
        dlg.StartPosition = FormStartPosition.CenterParent
        dlg.ShowInTaskbar = False
        If dlg.ShowDialog() = Windows.Forms.DialogResult.OK Then

            Select Case dlg.Mode
                Case dlgManageTimeSeries.eModeType.Load
                    MRUHelper.UpdateMRUString(My.Settings.MdbRecentlyUsedList, dlg.DatasetName, MRUHelper.eModuleType.Dataset)
                    My.Settings.Save()
                Case dlgManageTimeSeries.eModeType.Weight
                    ' NOP
                Case dlgManageTimeSeries.eModeType.Import
                    MRUHelper.UpdateMRUString(My.Settings.MdbRecentlyUsedList, dlg.DatasetName, MRUHelper.eModuleType.Dataset)
                    My.Settings.Save()
                    'Case dlgManageTimeSeries.eModeType.Export
                    '    ' NOP
                Case dlgManageTimeSeries.eModeType.Delete
                    ' NOP
            End Select
        End If
    End Sub

#End Region ' Ecosim

End Class