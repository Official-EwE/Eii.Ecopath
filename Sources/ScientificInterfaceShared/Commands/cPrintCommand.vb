' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

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
        Public Sub New(cmdh As cCommandHandler)
            MyBase.New(cmdh, COMMAND_NAME)
        End Sub

    End Class

End Namespace
