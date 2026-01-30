' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Commands

    Public Class cPickColorCommand
        Inherits cCommand

        ''' -----------------------------------------------------------------------
        ''' <summary>The name of this command.</summary>
        ''' -----------------------------------------------------------------------
        Public Shared COMMAND_NAME As String = "~pickcolor"

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Constructor, initializes a new instance of the NavigationCommand class.
        ''' </summary>
        ''' <param name="cmdh">The <see cref="cCommandHandler"/> to associate this command with.</param>
        ''' -----------------------------------------------------------------------
        Public Sub New(cmdh As cCommandHandler)
            MyBase.New(cmdh, COMMAND_NAME)
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Invoke the 'Directory open' command with default parameters.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Overrides Sub Invoke()
            Me.Result = DialogResult.Cancel
            MyBase.Invoke()
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Invoke the 'Directory open' command with default path and a custom
        ''' description.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Overloads Sub Invoke(clr As Color)
            Me.Color = clr
            Me.Invoke()
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the directory that the command was invoked with.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property Color() As Color

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' The result that the dialog closed with.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property Result() As DialogResult

    End Class

End Namespace
