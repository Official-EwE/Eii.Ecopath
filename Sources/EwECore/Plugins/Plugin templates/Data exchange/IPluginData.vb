' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.Common

Namespace Plugins.Data

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Base type for data shared by plugins.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Interface IPluginData

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Name of the <see cref="IPlugin">type name</see> of the plug-in that 
        ''' exposed this data.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        ReadOnly Property PluginName() As String

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' The <see cref="IRunType">run type</see> that this data was produced with.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        ReadOnly Property RunType() As IRunType

    End Interface

End Namespace
