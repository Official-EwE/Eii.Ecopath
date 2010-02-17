#Region " Imports "

Option Strict On
Imports EwEPlugin
Imports EwEPlugin.Data
Imports EwEUtils.Core

#End Region

Friend Class cEwENetworkAnalysisData
    Implements EwEPlugin.Data.IPluginData
    Implements EwEUtils.Core.INetworkAnalysisData

    Private m_strAssemblyName As String = ""
    Private m_strPluginName As String = ""
    Private m_assAscendancy(6, 5) As Single

    Public Sub New(ByVal strAssemblyName As String, ByVal strPluginName As String)
        Me.m_strAssemblyName = strAssemblyName
        Me.m_strPluginName = strPluginName
    End Sub

    Public ReadOnly Property AssemblyName() As String _
        Implements IPluginData.AssemblyName
        Get
            Return Me.m_strAssemblyName
        End Get
    End Property

    Public ReadOnly Property PluginName() As String _
        Implements IPluginData.PluginName
        Get
            Return Me.m_strPluginName
        End Get
    End Property

    Public ReadOnly Property Ascendancy() As Single(,) _
        Implements INetworkAnalysisData.Ascendancy
        Get
            Return Me.m_assAscendancy
        End Get
    End Property

    Public ReadOnly Property RunType() As IRunType _
        Implements IPluginData.RunType
        Get
            Return Nothing
        End Get
    End Property

End Class
