#Region " Imports "

Option Strict On
Imports EwEPlugin
Imports EwECore
Imports EwEUtils.Core
Imports Microsoft.Toolkit.Uwp.Notifications ' VS no longer can resolve this assembly. It's magic, and it's fragile

#End Region ' Imports

Public Class cEwEModernWinIntegrationPlugin
    Implements IPlugin

    Private m_core As cCore = Nothing
    Private m_mh As cMessageHandler = Nothing
    Private m_SyncObj As System.Threading.SynchronizationContext = Nothing

    Public ReadOnly Property Name As String Implements IPlugin.Name
        Get
            Return "cEwEModernWinIntegrationPlugin"
        End Get
    End Property

    Public ReadOnly Property DisplayName As String Implements IPlugin.DisplayName
        Get
            Return "EwE modern windows integration"
        End Get
    End Property

    Public ReadOnly Property Description As String Implements IPlugin.Description
        Get
            Return ""
        End Get
    End Property

    Public ReadOnly Property Author As String Implements IPlugin.Author
        Get
            Return ""
        End Get
    End Property

    Public ReadOnly Property Contact As String Implements IPlugin.Contact
        Get
            Return ""
        End Get
    End Property

    Public Sub Initialize(core As Object) Implements IPlugin.Initialize
        Try
            Me.m_core = DirectCast(core, cCore)
            Me.m_SyncObj = System.Threading.SynchronizationContext.Current
            SetupMessageHandler(eCoreComponentType.EcoSim, eMessageType.Any)
            SetupMessageHandler(eCoreComponentType.EcoSpace, eMessageType.Any)
        Catch ex As Exception

        End Try
    End Sub

    Private Sub MessageHandler(ByRef msg As cMessage)
        Try
            If msg.Type = eMessageType.EcosimRunCompleted Or msg.Type = eMessageType.EcospaceRunCompleted Then
                Toast(msg.Message)
            End If
        Catch ex As Exception

        End Try
    End Sub

    Private Sub Toast(text As String)
        Try
            Dim t As New ToastContentBuilder()
            t.AddText(text)
            t.Show()
        Catch ex As Exception

        End Try
    End Sub

    Private Sub SetupMessageHandler(src As eCoreComponentType, typ As eMessageType)
        Dim mh As New cMessageHandler(AddressOf Me.MessageHandler, src, eMessageType.Any, Me.m_SyncObj)
#If DEBUG Then
        ' Name the message handler for profiling
        mh.Name = "cEwEModernWinIntegrationPlugin::" & CStr(src) & ":" & CStr(typ)
#End If
        Me.m_core.Messages.AddMessageHandler(mh)
    End Sub

End Class
