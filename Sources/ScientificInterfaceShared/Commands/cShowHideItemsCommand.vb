' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Commands

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Command to launch a the interface to select which groups and fleets to
    ''' display in the UI.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cShowHideItemsCommand
        Inherits cCommand

        Public Shared COMMAND_NAME As String = "~showhideitems"

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor.
        ''' </summary>
        ''' <param name="cmdh">The <see cref="cCommandHandler"/> to associate this command with.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(cmdh As cCommandHandler)
            MyBase.New(cmdh, cShowHideItemsCommand.COMMAND_NAME, My.Resources.COMMAND_DISPLAYGROUPS)
        End Sub

    End Class

End Namespace
