' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Plugins.UI

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Plugin interface that defines all functionality required to intercept the 
    ''' execution of a user interface command.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Interface ICommandHandlerPlugin
        Inherits IGUIPlugin

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Implement this point to intercept execution of commands.
        ''' </summary>
        ''' <param name="cmd">The cCommand taht is being executed.</param>
        ''' <returns>True if EwE should consider the command as handled, or
        ''' Fals if EwE needs to handle the command.</returns>
        ''' -----------------------------------------------------------------------
        ReadOnly Property HandleCommand(cmd As Object) As Boolean

    End Interface

End Namespace