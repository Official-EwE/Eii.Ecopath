' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.Common

Namespace Plugins.Data

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Interface for implementing a plugin point that can broadcast data.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Interface IDataProducerPlugin
        Inherits IPlugin

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Initialization interface to inform the plug-in where to send its data
        ''' to once ready.
        ''' </summary>
        ''' <param name="broadcaster">The <see cref="IDataBroadcaster">IDataBroadcaster</see> 
        ''' to send data to.</param>
        ''' <remarks>
        ''' The plug-in should call <see cref="IDataBroadcaster.BroadcastData">IDataBroadcaster.BroadcastData</see>,
        ''' from where any <see cref="IDataConsumerPlugin">IDataConsumerPlugin</see>
        ''' -derived class gets a chance to consume the data by implementing
        ''' <see cref="IDataConsumerPlugin.ReceiveData">ReceiveData</see>.
        ''' </remarks>
        ''' -----------------------------------------------------------------------
        Sub Broadcaster(broadcaster As IDataBroadcaster)

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Requests whether data with a given <see cref="Type">Type</see> and
        ''' <see cref="IRunType">run type</see> is provided by this plug-in.
        ''' </summary>
        ''' <param name="typeData">
        ''' <see cref="Type">Type</see> of the data to request.
        ''' </param>
        ''' <param name="runType">
        ''' <see cref="IRunType">Run type</see> of the data to request.
        ''' </param>
        ''' -----------------------------------------------------------------------
        Function IsDataAvailable(typeData As Type, Optional runType As IRunType = Nothing) As Boolean

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Request data from this plug-in for a data with a specific
        ''' <see cref="Type">Type</see>.
        ''' </summary>
        ''' <param name="typeData"><see cref="Type">Type</see> of the data to request.</param>
        ''' <param name="data">The <see cref="IPluginData">data</see> offered by 
        ''' the plug-in.</param>
        ''' <returns>True if requested data is available.</returns>
        ''' -----------------------------------------------------------------------
        Function GetDataByType(typeData As Type, ByRef data As IPluginData) As Boolean

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get whether a data producer is allowed to distribute data.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Function IsEnabled() As Boolean

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Set whether a data producer is allowed to distribute data.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Function SetEnabled(bEnable As Boolean) As Boolean

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Set whether a plug-in distributes data for a given run type.
        ''' </summary>
        ''' <param name="typeData"><see cref="Type">Type</see> of the data to enable.</param>
        ''' <param name="runType">
        ''' <see cref="IRunType">Run type</see> of the data to enable or disable.
        ''' </param>
        ''' -----------------------------------------------------------------------
        Sub SetEnabled(typeData As Type, runType As IRunType, bEnable As Boolean)

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get whether a plug-in distributes data for a given run type.
        ''' </summary>
        ''' <param name="typeData"><see cref="Type">Type</see> of the data to request 
        ''' enabled state for.</param>
        ''' <param name="runType">
        ''' <see cref="IRunType">Run type</see> of the data to enable or disable.
        ''' </param>
        ''' -----------------------------------------------------------------------
        Function IsEnabled(typeData As Type, runType As IRunType) As Boolean

    End Interface

End Namespace
