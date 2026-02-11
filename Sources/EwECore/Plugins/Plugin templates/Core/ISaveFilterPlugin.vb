' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Plugins.Core

    ''' ===========================================================================
    ''' <summary>
    ''' Interface for implementing plug-ins that may prevent the core from saving or
    ''' discarding data.
    ''' </summary>
    ''' ===========================================================================
    Public Interface ISaveFilterPlugin
        Inherits ICorePlugin

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Plug-in point called when the core is about to save changes.
        ''' </summary>
        ''' <param name="bCancel">Setting this to False will abort the save attempt.</param>
        ''' -----------------------------------------------------------------------
        Function SaveChanges(ByRef bCancel As Boolean) As Boolean

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Plug-in point called when the core is about to save changes.
        ''' </summary>
        ''' <param name="bCancel">Setting this to False will abort the save attempt.</param>
        ''' -----------------------------------------------------------------------
        Function DiscardChanges(ByRef bCancel As Boolean) As Boolean

    End Interface

End Namespace