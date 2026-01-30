' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Common

    ''' <summary>
    ''' A generic command to centralized trigger executions in EwE.
    ''' </summary>
    Public Interface ICommand

        ReadOnly Property Name As String

        Function Parameters() As String()

        Property Parameter(name As String) As Object

    End Interface

End Namespace
