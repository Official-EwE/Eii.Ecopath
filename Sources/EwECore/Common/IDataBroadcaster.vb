' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.Plugins.Data

Namespace Common

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Interface for defining a data broadcaster.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Interface IDataBroadcaster

        Function BroadcastData(strDataName As String, data As IPluginData) As Boolean

    End Interface

End Namespace
