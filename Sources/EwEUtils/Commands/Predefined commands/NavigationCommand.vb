'==============================================================================
'
' $Log: NavigationCommand.vb,v $
' Revision 1.1  2008/09/26 07:31:10  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.1  2008/09/09 14:41:45  jeroens
' Moved
'
'==============================================================================

#Region " Imports "

Option Strict On
Imports EwEUtils.Core

#End Region ' Imports

Namespace Commands

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' The NavigationCommand class implements a <see cref="Command">Command</see>
    ''' that is used in EwE6 to navigate to embedded and plugin-provided 
    ''' <see cref="System.Windows.Forms.Form">Forms</see>.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Class NavigationCommand
        Inherits Command

        ''' <summary>A human readable name for the page to open. This text may be
        ''' used in tabs, in tooltips and other user interface elements.</summary>
        Private m_strPageName As String = ""

        ''' <summary>A unique ID for the page to open. This ID is used internally
        ''' in the GUI to manage opened pages.</summary>
        Private m_strPageID As String = ""

        ''' <summary>The core state that this page requires to display its
        ''' content.</summary>
        Private m_coreExecutionState As eCoreExecutionState = eCoreExecutionState.Idle

        ''' <summary>The Type of the Form to invoke.</summary>
        Private m_typeClass As Type = Nothing

        ''' <summary>Help URL for this form</summary>
        Private m_strHelpURL As String = ""

        ''' -----------------------------------------------------------------------
        ''' <summary>The name of this command.</summary>
        ''' <example>
        ''' The folowing shows how to retrieve the one instance of the NavigationCommand
        ''' from anywhere in the EwE6 user interface:
        ''' <code>
        ''' ' Get the one and only command 
        ''' Dim cdmH As CommandHandler = CommandHandler.GetInstance()
        ''' ' Get the one and only navigation command
        ''' Dim cmd As NavigationCommand = DirectCast(GetCommand(NavigationCommand.COMMAND_NAME), NavigationCommand)
        ''' ' Invoke the command
        ''' ...
        ''' </code>
        ''' </example>
        ''' -----------------------------------------------------------------------
        Public Shared COMMAND_NAME As String = "~navigate"

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Constructor, initializes a new instance of the NavigationCommand class.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Sub New()
            MyBase.New(COMMAND_NAME)
        End Sub

        Public Sub New(ByVal strPageName As String, ByVal strPageID As String, _
                ByVal coreExecutionState As eCoreExecutionState, _
                ByVal typeClass As Type, _
                Optional ByVal strHelpURL As String = "")

            MyBase.New(COMMAND_NAME)

            Me.m_strPageName = strPageName
            Me.m_strPageID = strPageID
            Me.m_coreExecutionState = coreExecutionState
            Me.m_typeClass = typeClass
            Me.m_strHelpURL = strHelpURL

        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Invokes the command to make the EwE6 GUI navigate to user interface
        ''' element defined by this call.
        ''' </summary>
        ''' <param name="strPageName">A pleasantly legible name of the page to 
        ''' navigate to.</param>
        ''' <param name="strPageID">A unique page ID, used by the EwE6 GIU to
        ''' check whether this page is already open and needs merely focusing,
        ''' or whether this page needs to be constructed.</param>
        ''' <param name="coreExecutionState"><para>An enumerated value obtained
        ''' from the EwE6 Core State monitor, indicating what state the EwE6
        ''' core should be running at to be able to provide data for the form
        ''' that will be launched from this command.</para>
        ''' <para>The EwE6 GUI will attempt to bring the core up to this desired 
        ''' running state prior to launching the form.</para></param>
        ''' <param name="typeClass">A Type of a Windows.Forms derived user
        ''' interface that is to be created.</param>
        ''' <param name="strHelpURL">Help URL for this page.</param>
        ''' -----------------------------------------------------------------------
        Public Overloads Sub Invoke(ByVal strPageName As String, ByVal strPageID As String, _
                ByVal coreExecutionState As eCoreExecutionState, _
                ByVal typeClass As Type, _
                Optional ByVal strHelpURL As String = "")

            Me.m_strPageName = strPageName
            Me.m_strPageID = strPageID
            Me.m_coreExecutionState = coreExecutionState
            Me.m_typeClass = typeClass
            Me.m_strHelpURL = strHelpURL

            MyBase.Invoke()
        End Sub

        ''' <summary>
        ''' Get the <see cref="m_strPageName">Page name</see>.
        ''' </summary>
        Public ReadOnly Property PageName() As String
            Get
                Return Me.m_strPageName
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the <see cref="m_strPageID">Page ID</see>.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property PageID() As String
            Get
                Return Me.m_strPageID
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the <see cref="m_coreExecutionState">Core execution state</see>.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property CoreExecutionState() As eCoreExecutionState
            Get
                Return Me.m_coreExecutionState
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the <see cref="Type">Type</see> of the <see cref="m_typeClass">Form</see>
        ''' to create for this command.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property ClassType() As Type
            Get
                Return Me.m_typeClass
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the <see cref="m_strHelpURL">help URL</see> for this page.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property HelpURL() As String
            Get
                Return Me.m_strHelpURL
            End Get
        End Property

    End Class

End Namespace
