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

Option Strict On

#End Region ' Imports

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
