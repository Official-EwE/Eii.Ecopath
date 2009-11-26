#Region " Imports "

Option Strict On
Imports System
Imports EwEUtils.Core

#End Region ' Imports

Namespace Data

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Interface for implementing a plugin point that can broadcast data.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Interface IDataProducerPlugin
        : Inherits IPlugin

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
        ''' <param name="runType">Run type that the data is requested for, or
        ''' Null if the run type is irrelevant.</param>
        ''' </remarks>
        ''' -----------------------------------------------------------------------
        Sub Broadcaster(ByVal broadcaster As IDataBroadcaster)

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
        Function IsDataAvailable(ByVal typeData As Type, ByVal runType As IRunType) As Boolean

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Requests whether data with a given <see cref="Type">Type</see> and
        ''' <see cref="IRunType">run type</see> is provided by this plug-in.
        ''' </summary>
        ''' <param name="strDataName">Name of the data to request.</param>
        ''' <param name="runType">
        ''' <see cref="IRunType">Run type</see> of the data to request.
        ''' </param>
        ''' -----------------------------------------------------------------------
        Function IsDataAvailable(ByVal strDataName As String, ByVal runType As IRunType) As Boolean

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
        Function GetDataByType(ByVal typeData As Type, ByRef data As IPluginData) As Boolean

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Request data from this plug-in for data with a specific name.
        ''' </summary>
        ''' <param name="strDataName">Name of the data to request.</param>
        ''' <param name="data">The <see cref="IPluginData">data</see> offered by 
        ''' the plug-in.</param>
        ''' <returns>True if requested data is available.</returns>
        ''' -----------------------------------------------------------------------
        Function GetDataByName(ByVal strDataName As String, ByRef data As IPluginData) As Boolean

    End Interface

End Namespace
