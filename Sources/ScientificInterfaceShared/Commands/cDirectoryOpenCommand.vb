' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Commands

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Command to launch a directory selection interface.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Class cDirectoryOpenCommand
        Inherits cCommand

        ''' -----------------------------------------------------------------------
        ''' <summary>The name of this command.</summary>
        ''' -----------------------------------------------------------------------
        Public Shared COMMAND_NAME As String = "~opendirectory"

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
        ''' <param name="strDirectory">The directory to show in the dialog.</param>
        ''' -----------------------------------------------------------------------
        Public Overloads Sub Invoke(strDirectory As String)
            Me.Directory = strDirectory
            Me.Invoke()
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Invoke the 'Directory open' command.
        ''' </summary>
        ''' <param name="strDirectory">Initial directory to open the dialog at.</param>
        ''' <param name="strDescription">The description to show in the dialog.</param>
        ''' -----------------------------------------------------------------------
        Public Overloads Sub Invoke(strDirectory As String, strDescription As String)
            Me.Prompt = strDescription
            Me.Directory = strDirectory
            Me.Invoke()
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the prompt to display in the dialog.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property Prompt() As String
            Get
                Return CStr(Me.Parameter("Prompt"))
            End Get
            Set(value As String)
                Me.Parameter("Prompt") = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the directory that the command was invoked with.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property Directory() As String
            Get
                Return CStr(Me.Parameter("Directory"))
            End Get
            Set(value As String)
                Me.Parameter("Directory") = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' The result that the dialog closed with.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property Result() As DialogResult
            Get
                Return DirectCast(Me.Parameter("Result"), DialogResult)
            End Get
            Set(value As DialogResult)
                Me.Parameter("Result") = value
            End Set
        End Property

    End Class

End Namespace
