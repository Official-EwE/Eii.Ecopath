' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Commands

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Command to invoke the 'Clear license' interface
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Class cClearLicenseCommand
        Inherits cCommand

        ''' <summary>Static name for this command.</summary>
        Public Shared cCOMMAND_NAME As String = "~clearLicense"

        Public Sub New(cmdh As cCommandHandler)
            MyBase.New(cmdh, cClearLicenseCommand.cCOMMAND_NAME)
        End Sub

    End Class

End Namespace ' Commands
