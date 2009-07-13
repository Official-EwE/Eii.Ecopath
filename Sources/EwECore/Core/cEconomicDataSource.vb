
Option Strict On

Imports EwEPlugin
Imports EwEUtils.Database
Imports System.Data
Imports EwEUtils.Core

''' <summary>
''' Implemention of IDataConsumerPlugin that fires an event when ever Economic data is available.
''' </summary>
''' <remarks>This could be extented to be a source for any data the is broadcasted via the IDataBroadcaster plugin interface </remarks>
Public Class cEconomicDataSource
    Implements Data.IDataConsumerPlugin

#Region "Public events"

    ''' <summary>
    ''' Event that get fired when IEconomicData is available
    ''' </summary>
    ''' <param name="EconomicData"></param>
    ''' <remarks></remarks>
    Public Event onEconomicData(ByVal EconomicData As IEconomicData)

#End Region

#Region "Singleton 'Shared' interface"

    ''' <summary>
    ''' Return the instance of this class created by the PluginManager
    ''' </summary>
    ''' <returns>The only instance of cEconomicDataSource. Otherwise nothing</returns>
    ''' <remarks>An instance of this class is loaded from the Core via the Plugin manager. This allows classes in the core to retrieve an instance of cEconomicDataSource for Economic data.</remarks>
    Public Shared Function getInstance() As cEconomicDataSource
        Dim dataSource As cEconomicDataSource

        Try

            Dim plugins As ICollection(Of EwEPlugin.IPlugin)

            plugins = cCore.GetInstance.PluginManager.GetPlugins("ewecore.ceconomicdatasource")
            For Each plugin As IPlugin In plugins
                If TypeOf plugin Is cEconomicDataSource Then
                    dataSource = DirectCast(plugin, cEconomicDataSource)
                    Exit For
                End If
            Next

        Catch ex As Exception
            System.Console.WriteLine("cEconomicDataSource.getInstance() Error: " & ex.Message)
        End Try

        Debug.Assert(dataSource IsNot Nothing, "cEconomicDataSource.getInstance() Failed to create instance.")

        Return dataSource

    End Function

#End Region

#Region "Private methods"

    Private Sub FireonEconomicData(ByVal data As IEconomicData)
        Try
            RaiseEvent onEconomicData(data)
        Catch ex As Exception
            Debug.Assert(False, Me.ToString & "FireonEconomicData() Error: " & ex.Message)
        End Try
    End Sub

#End Region

#Region "IDataConsumerPlugin implementation"


    Public Function ReceiveData(ByVal strDataName As String, ByVal data As EwEPlugin.Data.IPluginData) As Boolean Implements EwEPlugin.Data.IDataConsumerPlugin.ReceiveData

        Try
            If TypeOf data Is IEconomicData Then
                Dim ecoData As IEconomicData = DirectCast(data, IEconomicData)
                Me.FireonEconomicData(ecoData)
            End If
        Catch ex As Exception
            'make sure all exceptions are handled here and not thrown back to the PluginManager
            cLog.Write(ex)
        End Try

    End Function

    Public ReadOnly Property Author() As String Implements EwEPlugin.IPlugin.Author
        Get
            Return "UBC Fisheries Centre"
        End Get
    End Property

    Public ReadOnly Property Contact() As String Implements EwEPlugin.IPlugin.Contact
        Get
            Return "mailto:support@ecopath.org"
        End Get
    End Property

    Public ReadOnly Property Description() As String Implements EwEPlugin.IPlugin.Description
        Get
            Return "Core plugin to provide economic data from an external source."
        End Get
    End Property

    Public Sub Initialize(ByVal core As Object) Implements EwEPlugin.IPlugin.Initialize

    End Sub

    Public ReadOnly Property Name() As String Implements EwEPlugin.IPlugin.Name
        Get
            Return Me.ToString
        End Get
    End Property

#End Region

End Class
