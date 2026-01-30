' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports ScientificInterfaceShared.Definitions



Namespace Commands

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Command to launch the 'options' interface in EwE.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cShowOptionsCommand
        Inherits cCommand

#Region " Private vars "

        Private m_strVerb As String = ""

#End Region ' Private vars

#Region " Public interfaces "

        ''' -----------------------------------------------------------------------
        ''' <summary>The name of this command.</summary>
        ''' <example>
        ''' The folowing shows how to retrieve the one instance of the cShowOptionsCommand
        ''' from anywhere in the EwE6 user interface:
        ''' <code>
        ''' ' Get the one and only command 
        ''' Dim cdmH As cCommandHandler = cCommandHandler.GetInstance()
        ''' ' Get the one and only ecosim save data command
        ''' Dim cmd As cShowOptionsCommand = DirectCast(GetCommand(cShowOptionsCommand.COMMAND_NAME), cShowOptionsCommand)
        ''' ' Invoke the command
        ''' ...
        ''' </code>
        ''' </example>
        ''' -----------------------------------------------------------------------
        Public Const cCOMMAND_NAME As String = "~showoptions~"

        Public Sub New(cmdh As cCommandHandler)
            MyBase.New(cmdh, cCOMMAND_NAME)
        End Sub

        Public Overloads Sub Invoke(opt As eApplicationOptionTypes)
            Me.Invoke(opt.ToString())
        End Sub

        Public Overloads Sub Invoke(Optional strVerb As String = "")
            ' Set option
            Me.m_strVerb = strVerb
            MyBase.Invoke()
            Me.m_strVerb = ""
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get the <see cref="eApplicationOptionTypes">application option</see> 
        ''' that this command was invoked for.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public ReadOnly Property Verb As String
            Get
                Return Me.m_strVerb
            End Get
        End Property

#End Region ' Public interfaces

    End Class

End Namespace
