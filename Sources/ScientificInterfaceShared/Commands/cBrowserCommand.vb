#Region " Imports "

Option Strict On
Imports EwEUtils.Core
Imports EwEUtils.Commands

#End Region ' Imports

Namespace Commands

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' The NavigationCommand class implements a <see cref="cCommand">Command</see>
    ''' that is used in EwE6 to navigate to embedded and plugin-provided 
    ''' <see cref="System.Windows.Forms.Form">Forms</see>.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Class cBrowserCommand
        Inherits cCommand

        ''' <summary>URL to show.</summary>
        Private m_strURL As String = ""

        ''' -----------------------------------------------------------------------
        ''' <summary>The name of this command.</summary>
        ''' <example>
        ''' The folowing shows how to retrieve the one instance of the cNavigationCommand
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
        Public Shared COMMAND_NAME As String = "~browse"

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Constructor, initializes a new instance of the NavigationCommand class.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Sub New(ByVal cmdh As cCommandHandler)
            MyBase.New(cmdh, COMMAND_NAME)
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Invokes the command to make the EwE6 GUI navigate to user interface
        ''' element defined by this call.
        ''' </summary>
        ''' <param name="strURL">URL to navigate to.</param>
        ''' -----------------------------------------------------------------------
        Public Overloads Sub Invoke(Optional ByVal strURL As String = "")

            Me.m_strURL = strURL

            MyBase.Invoke()
        End Sub

        ''' <summary>
        ''' Get the <see cref="m_strURL">URL</see> to navigate to.
        ''' </summary>
        Public ReadOnly Property URL() As String
            Get
                Return Me.m_strURL
            End Get
        End Property

    End Class

End Namespace
