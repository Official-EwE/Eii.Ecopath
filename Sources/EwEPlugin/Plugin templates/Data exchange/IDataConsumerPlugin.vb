#Region " Imports "

Option Strict On
Imports EwEUtils.Core

#End Region ' Imports

Namespace Data

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
        ''' <param name="data">The <see cref="IPluginData">data</see> that is being 
        ''' broadcasted.</param>
        ''' -----------------------------------------------------------------------
        Function ReceiveData(ByVal strDataName As String, ByVal data As IPluginData) As Boolean

    End Interface

End Namespace
