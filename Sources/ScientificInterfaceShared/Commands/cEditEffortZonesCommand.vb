' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Commands

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Command to invoke the 'Edit Effort Zones' interface
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Class cEditEffortZonesCommand
        Inherits cCommand

        ''' <summary>Static name for this command.</summary>
        Public Shared cCOMMAND_NAME As String = "EditEffortZones"

        Public Sub New(cmdh As cCommandHandler)
            MyBase.New(cmdh, cEditEffortZonesCommand.cCOMMAND_NAME, My.Resources.COMMAND_DEFINEEFFORTZONES)
        End Sub

    End Class

End Namespace ' Commands
