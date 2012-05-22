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
' Copyright 1991-2012 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'
#Region " Imports "

Option Strict On
Imports EwEUtils.Commands
Imports System.Windows.Forms

#End Region ' Imports

Namespace Commands

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Command to launch a directory selection interface.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Class cDirectoryOpenCommand
        Inherits cCommand

#Region " Privates "

        ''' <summary>Dialog prompt</summary>
        Private m_strDescription As String = ""
        ''' <summary>Name of the file to open.</summary>
        Private m_strDirectory As String = ""
        ''' <summary>The dialog result.</summary>
        Private m_iResult As DialogResult = DialogResult.OK

#End Region ' Privates

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
        Public Sub New(ByVal cmdh As cCommandHandler)
            MyBase.New(cmdh, COMMAND_NAME)
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Invoke the 'Directory open' command with default parameters.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Overrides Sub Invoke()
            Me.m_iResult = DialogResult.Cancel
            MyBase.Invoke()
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Invoke the 'Directory open' command with default path and a custom
        ''' description.
        ''' </summary>
        ''' <param name="strDescription">The description to show in the dialog.</param>
        ''' -----------------------------------------------------------------------
        Public Overloads Sub Invoke(ByVal strDescription As String)
            Me.m_strDescription = strDescription
            Me.Invoke()
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Invoke the 'Directory open' command.
        ''' </summary>
        ''' <param name="strDirectory">Initial directory to open the dialog at.</param>
        ''' <param name="strDescription">The description to show in the dialog.</param>
        ''' -----------------------------------------------------------------------
        Public Overloads Sub Invoke(ByVal strDirectory As String, ByVal strDescription As String)
            Me.m_strDescription = strDescription
            Me.m_strDirectory = strDirectory
            Me.Invoke()
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the prompt to display in the dialog.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property Description() As String
            Get
                Return Me.m_strDescription
            End Get
            Set(ByVal strDescription As String)
                Me.m_strDescription = strDescription
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the directory that the command was invoked with.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property Directory() As String
            Get
                Return Me.m_strDirectory
            End Get
            Set(ByVal strDirectory As String)
                Me.m_strDirectory = strDirectory
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' The result that the dialog closed with.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property Result() As DialogResult
            Get
                Return Me.m_iResult
            End Get
            Set(ByVal value As DialogResult)
                Me.m_iResult = value
            End Set
        End Property

    End Class

End Namespace
