' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Commands

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' The NavigationCommand class implements a <see cref="cCommand">Command</see>
    ''' that is used in EwE6 to navigate to embedded and plugin-provided 
    ''' <see cref="System.Windows.Forms.Form">Forms</see>.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Class cNavigationCommand
        Inherits cCommand

        ''' -----------------------------------------------------------------------
        ''' <summary>The name of this command.</summary>
        ''' <example>
        ''' The folowing shows how to retrieve the one instance of the cNavigationCommand
        ''' from anywhere in the EwE6 user interface:
        ''' <code>
        ''' ' Get the one and only command 
        ''' Dim cdmH As cCommandHandler = cCommandHandler.GetInstance()
        ''' ' Get the one and only navigation command
        ''' Dim cmd As cNavigationCommand = DirectCast(GetCommand(cNavigationCommand.COMMAND_NAME), cNavigationCommand)
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
        Public Sub New(cmdh As cCommandHandler)
            MyBase.New(cmdh, COMMAND_NAME)
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
        Public Overloads Sub Invoke(strPageName As String,
                                    strPageID As String,
                                    coreExecutionState As eCoreExecutionState,
                                    typeClass As Type,
                                    Optional strHelpURL As String = "")

            Me.Parameter("Pagename") = strPageName
            Me.Parameter("PageID") = strPageID
            Me.Parameter("CoreExecState") = coreExecutionState
            Me.Parameter("TypeClass") = typeClass
            Me.Parameter("HelpURL") = strHelpURL

            MyBase.Invoke()
        End Sub

        ''' <summary>
        ''' Get the Page name.
        ''' </summary>
        Public ReadOnly Property PageName() As String
            Get
                Return CStr(Me.Parameter("Pagename"))
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the Page ID.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property PageID() As String
            Get
                Return CStr(Me.Parameter("PageID"))
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the Core execution state.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property CoreExecutionState() As eCoreExecutionState
            Get
                Return DirectCast(Me.Parameter("CoreExecState"), eCoreExecutionState)
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the <see cref="Type">Type</see> of the Form to launch for this command.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property ClassType() As Type
            Get
                Return DirectCast(Me.Parameter("TypeClass"), Type)
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the help URL for this page.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property HelpURL() As String
            Get
                Return CStr(Me.Parameter("HelpURL"))
            End Get
        End Property

    End Class

End Namespace
