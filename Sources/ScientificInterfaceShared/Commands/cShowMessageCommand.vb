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
Imports ScientificInterfaceShared.Commands

#End Region ' Imports

Namespace Commands


    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Command to show a message to the user in the EwE interface.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cShowMessageCommand
        Inherits cCommand

#Region " Private vars "

        ' ToDo: use eCoreComponentType, eMessageImportance here?

        ''' <summary>Message to show.</summary>
        Private m_strMessage As String = ""
        Private m_mbb As MessageBoxButtons = MessageBoxButtons.OK
        Private m_mbi As MessageBoxIcon = MessageBoxIcon.Information

#End Region ' Private vars

        Public Sub New(ByVal cmdh As cCommandHandler)
            MyBase.New(cmdh, COMMAND_NAME)
        End Sub

#Region " Public interfaces "

        ''' -----------------------------------------------------------------------
        ''' <summary>The name of this command.</summary>
        ''' -----------------------------------------------------------------------
        Public Shared COMMAND_NAME As String = "~showessage"

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' The message to show to the user.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public ReadOnly Property Message() As String
            Get
                Return Me.m_strMessage
            End Get
        End Property

        Public ReadOnly Property Buttons As MessageBoxButtons
            Get
                Return Me.m_mbb
            End Get
        End Property

        Public ReadOnly Property Icon As MessageBoxIcon
            Get
                Return Me.m_mbi
            End Get
        End Property

        Public Property Suppress As Boolean

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Invoke the command.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Shadows Sub Invoke(ByVal strMessage As String, mbb As MessageBoxButtons, mbi As MessageBoxIcon, ByRef bSuppress As Boolean)

            ' Sanity check
            Debug.Assert(Not String.IsNullOrEmpty(strMessage))

            Me.m_strMessage = strMessage
            Me.m_mbb = mbb
            Me.m_mbi = mbi
            Me.Suppress = bSuppress

            ' Invoke!
            MyBase.Invoke()

            Me.m_strMessage = ""

        End Sub

#End Region ' Public interfaces
    End Class

End Namespace
