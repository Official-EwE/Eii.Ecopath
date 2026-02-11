' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Commands

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Command to request remote execution of an instruction.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cExecuteCommand
        Inherits cCommand

        Public Sub New(cmdh As cCommandHandler)
            MyBase.New(cmdh, COMMAND_NAME)
        End Sub

#Region " Public interfaces "

        ''' -----------------------------------------------------------------------
        ''' <summary>The name of this command.</summary>
        ''' -----------------------------------------------------------------------
        Public Shared COMMAND_NAME As String = "~execute"

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' The command (string) to execute.
        ''' </summary>
        ''' <remarks>The command string is converted to LOWER CASE.</remarks>
        ''' -----------------------------------------------------------------------
        Public Property Command() As String
            Get
                Return CStr(Me.Parameter("Command"))
            End Get
            Private Set(value As String)
                Me.Parameter("Command") = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Invoke the command.
        ''' </summary>
        ''' <param name="strCommand">Command string to pass to the command.</param>
        ''' -----------------------------------------------------------------------
        Public Shadows Sub Invoke(strCommand As String)

            ' Sanity check
            Debug.Assert(Not String.IsNullOrEmpty(strCommand))

            ' Store command
            Me.Command = strCommand.ToLower()
            ' Invoke!
            MyBase.Invoke()
            ' Clear command values to prepare it for next usage
            Me.Command = ""
        End Sub

#End Region ' Public interfaces

    End Class

End Namespace
