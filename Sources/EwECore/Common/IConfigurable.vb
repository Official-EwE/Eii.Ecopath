' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Common

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Interface for adding items that can be configured with a visual interface
    ''' throughout the EwE application.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Interface IConfigurable

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Returns whether an item has been configured.
        ''' </summary>
        ''' <returns>True if an item has been configured.</returns>
        ''' -----------------------------------------------------------------------
        Function IsConfigured() As Boolean

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Returns the windows control though which the item can be configured.
        ''' </summary>
        ''' <returns>The windows control though which the item can be configured.</returns>
        ''' -----------------------------------------------------------------------
        Function GetConfigUI() As Object

    End Interface

End Namespace
