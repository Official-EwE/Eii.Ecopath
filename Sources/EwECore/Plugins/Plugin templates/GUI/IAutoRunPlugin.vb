' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Plugins.UI

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Interface for implementing a plugin point that automatically executes with
    ''' one or more of the EwE <see cref="eCoreComponentType">core components</see>.
    ''' Note that this plug-in point just serves to centrally identify the auto-run
    ''' setting in the user interface. The plug-in is responsible for triggering and
    ''' implementing the auto-run behaviour by implementing the desired plug-in points.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Interface IAutoRunPlugin
        Inherits IPlugin

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Returns an array of <see cref="eCoreComponentType"/> identifiers that this
        ''' plug-in can execute with.
        ''' </summary>
        ''' <returns>An array of <see cref="eCoreComponentType"/> identifiers that this
        ''' plug-in can execute with.</returns>
        ''' -----------------------------------------------------------------------
        Function AutoRunTypes() As eCoreComponentType()

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get/set if this plug-in is enabled to auto-run with a given <see cref="eCoreComponentType">core component</see>..
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Property AutoRun(type As eCoreComponentType) As Boolean

    End Interface

End Namespace