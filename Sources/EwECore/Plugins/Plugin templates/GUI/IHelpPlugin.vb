' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Plugins.UI

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' IPluginHelpPlugin, interface for providing help information for a 
    ''' <see cref="IPlugin">plugin</see>.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Interface IHelpPlugin

        ''' <summary>Get the URL to the help file for a plug-in.</summary>
        ReadOnly Property HelpURL As String

        ''' <summary>Get the URL to the topic in the <see cref="HelpURL">help file</see>.</summary>
        ReadOnly Property HelpTopic As String

    End Interface

End Namespace