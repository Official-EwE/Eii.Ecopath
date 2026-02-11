' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Controls

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Interface for implementing an configuration page for integration in the 
    ''' EwE system-wide options
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Interface IOptionsPage
        Inherits IUIElement

        Event OnChanged(sender As IOptionsPage, args As EventArgs)

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Enumerated type stating all possible results when applying the content
        ''' of an options page.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Enum eApplyResultType As Integer
            ''' <summary>Application was successful.</summary>
            Success
            ''' <summary>Application was successful but requires a restart.</summary>
            Success_restart
            ''' <summary>Application successful, but need administrator privileges to work.</summary>
            Success_administrator
            ''' <summary>Application failed.</summary>
            Failed
        End Enum

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Check whether the options page can be applied.
        ''' </summary>
        ''' <returns>True if the options page can be applied.</returns>
        ''' -----------------------------------------------------------------------
        Function CanApply() As Boolean

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Apply the content of an options page to the 'system'.
        ''' </summary>
        ''' <returns>An <see cref="eApplyResultType">apply result</see>.</returns>
        ''' -----------------------------------------------------------------------
        Function Apply() As eApplyResultType

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Revert the current page to default values
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Sub SetDefaults()

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Returns whether this options page can set defaults.
        ''' </summary>
        ''' <returns>True if the options page can set defaults.</returns>
        ''' -----------------------------------------------------------------------
        Function CanSetDefaults() As Boolean

    End Interface

End Namespace
