' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Plugins.Data

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Interface for implementing a plugin point that is able to receive broadcasted
    ''' data.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Interface IDataConsumerPlugin
        Inherits IPlugin

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Interface to receive data originating from 
        ''' <see cref="IDataBroadcaster.BroadcastData">IDataBroadcaster.BroadcastData</see>.
        ''' </summary>
        ''' <param name="strDataName">Name of the data that is being broadcasted.</param>
        ''' <param name="data">The <see cref="IPluginData">data</see> that is being 
        ''' broadcasted.</param>
        ''' -----------------------------------------------------------------------
        Function ReceiveData(strDataName As String, data As IPluginData) As Boolean

    End Interface

End Namespace
