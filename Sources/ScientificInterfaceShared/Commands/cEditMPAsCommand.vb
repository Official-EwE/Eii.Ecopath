' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Commands

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Command to invoke the 'Edit MPAs' interface
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Class cEditMPAsCommand
        Inherits cCommand

        ''' <summary>Static name for this command.</summary>
        Public Shared cCOMMAND_NAME As String = "EditMPAs"

        Public Sub New(cmdh As cCommandHandler)
            MyBase.New(cmdh, cEditMPAsCommand.cCOMMAND_NAME, My.Resources.COMMAND_DEFINEMPAS)
        End Sub

    End Class

End Namespace ' Commands
