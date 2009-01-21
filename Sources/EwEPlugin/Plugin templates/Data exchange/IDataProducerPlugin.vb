'==============================================================================
'
' $Log: IDataProducerPlugin.vb,v $
' Revision 1.1  2009/01/21 19:08:12  jeroens
' Moved and split into separate files
'
'==============================================================================

#Region " Imports "

Option Strict On
Imports EwEUtils.Core

#End Region ' Imports

Namespace Data

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Interface for implementing a plugin point that want to broadcast its data.
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
        ''' </remarks>
        ''' -----------------------------------------------------------------------
        Sub Broadcaster(ByVal broadcaster As IDataBroadcaster)

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
