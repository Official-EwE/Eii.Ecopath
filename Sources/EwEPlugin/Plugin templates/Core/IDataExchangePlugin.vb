'==============================================================================
'
' $Log: IDataExchangePlugin.vb,v $
' Revision 1.2  2008/10/07 21:20:57  jeroens
' Implemented data exchange plugin structure
'
' Revision 1.1  2008/07/06 17:24:01  jeroens
' Initial version
'
'==============================================================================

Option Strict On
Imports EwEUtils.Core

''' ---------------------------------------------------------------------------
''' <summary>
''' Interface for defining a data broadcaster.
''' </summary>
''' ---------------------------------------------------------------------------
Public Interface IDataBroadcaster

    Function BroadcastData(ByVal strDataName As String, ByVal ds As DataSet) As Boolean

End Interface

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
    ''' Request data from this plug-in.
    ''' </summary>
    ''' <param name="strDataName">Name of the data to request.</param>
    ''' <param name="objData">The data offered by the plug-in.</param>
    ''' <returns>True if requested data is available.</returns>
    ''' -----------------------------------------------------------------------
    Function GetData(ByVal strDataName As String, ByRef objData As Object) As Boolean

End Interface

''' ---------------------------------------------------------------------------
''' <summary>
''' Interface for implementing a plugin point that is able to receive broadcasted
''' data.
''' </summary>
''' ---------------------------------------------------------------------------
Public Interface IDataConsumerPlugin
    : Inherits IPlugin

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Interface to receive data originating from 
    ''' <see cref="IDataBroadcaster.BroadcastData">IDataBroadcaster.BroadcastData</see>.
    ''' </summary>
    ''' <param name="strDataName">Name of the data that is being broadcasted.</param>
    ''' <param name="ds"><see cref="DataSet">Data set</see> holding the data 
    ''' that is being broadcasted.</param>
    ''' -----------------------------------------------------------------------
    Function ReceiveData(ByVal strDataName As String, ByVal ds As DataSet) As Boolean

End Interface
