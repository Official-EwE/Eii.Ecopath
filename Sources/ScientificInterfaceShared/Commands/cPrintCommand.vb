#Region " Imports "

Option Strict On
Imports EwEUtils.Core
Imports EwEUtils.Commands
Imports System.Drawing.Printing

#End Region ' Imports

Namespace Commands

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' The cPrintCommand class implements a <see cref="cCommand">Command</see>
    ''' that is used in EwE6 to print any content.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Class cPrintCommand
        Inherits cCommand

        ''' -----------------------------------------------------------------------
        ''' <summary>The name of this command.</summary>
        ''' <example>
        ''' The folowing shows how to retrieve the one instance of the cPrintCommand
        ''' from anywhere in the EwE6 user interface:
        ''' <code>
        ''' ' Get the one and only command 
        ''' Dim cdmH As cCommandHandler = cCommandHandler.GetInstance()
        ''' ' Get the one and only print command
        ''' Dim cmd As cPrintCommand = DirectCast(GetCommand(cPrintCommand.COMMAND_NAME), cPrintCommand)
        ''' ' Invoke the command
        ''' ...
        ''' </code>
        ''' </example>
        ''' -----------------------------------------------------------------------
        Public Shared COMMAND_NAME As String = "~print"

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Constructor, initializes a new instance of the NavigationCommand class.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Sub New(ByVal cmdh As cCommandHandler)
            MyBase.New(cmdh, COMMAND_NAME)
        End Sub

    End Class

End Namespace
