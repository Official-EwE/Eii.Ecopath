#Region " Imports "

Option Strict On
Imports EwEUtils.Core

#End Region ' Imports

Namespace Data

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Interface for defining a data broadcaster.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Interface IDataBroadcaster

        Function BroadcastData(ByVal strDataName As String, ByVal data As IPluginData) As Boolean

    End Interface

End Namespace
