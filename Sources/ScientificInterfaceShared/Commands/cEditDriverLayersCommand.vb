' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Commands

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Command to invoke the 'Edit driver layers" command.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Class cEditDriverLayersCommand
        Inherits cCommand

        ''' <summary>Static name for this command.</summary>
        Public Shared cCOMMAND_NAME As String = "EditInputMaps"

        Public Sub New(cmdh As cCommandHandler)
            MyBase.New(cmdh, cEditDriverLayersCommand.cCOMMAND_NAME)
        End Sub

    End Class

End Namespace ' Commands
