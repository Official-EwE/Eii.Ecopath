#Region " Imports "

Option Strict On

Imports EwEPlugin
Imports EwEUtils.Database
Imports System.Data
Imports EwEUtils.Core

#End Region ' Imports

Namespace ExternalData

    ''' <summary>
    ''' Implemention of IDataConsumerPlugin that fires an event when ever Economic data is available.
    ''' </summary>
    ''' <remarks>This could be extented to be a source for any data the is broadcasted via the IDataBroadcaster plugin interface </remarks>
    Public Class cEconomicDataSource
        Implements Data.IDataConsumerPlugin
        Implements IExternalData

#Region " Private vars "

        Private Shared s_core As cCore = Nothing

#End Region ' Private vars

#Region " Public events "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Event that get fired when IEconomicData is available
        ''' </summary>
        ''' <param name="EconomicData"></param>
        ''' -----------------------------------------------------------------------
        Public Event onEconomicData(ByVal EconomicData As IEconomicData)

#End Region ' Public events

#Region " Singleton 'Shared' interface "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Return the instance of this class created by the PluginManager
        ''' </summary>
        ''' <returns>The only instance of cEconomicDataSource. Otherwise nothing</returns>
        ''' <remarks>An instance of this class is loaded from the Core via the Plugin 
        ''' manager. This allows classes in the core to retrieve an instance of 
        ''' cEconomicDataSource for Economic data.</remarks>
        ''' -----------------------------------------------------------------------
        Public Shared Function getInstance() As cEconomicDataSource

            Dim dataSource As cEconomicDataSource = Nothing

            Try

                Dim plugins As ICollection(Of EwEPlugin.IPlugin)

                plugins = s_core.PluginManager.GetPlugins(cEconomicDataSource.InternalName)
                For Each plugin As IPlugin In plugins
                    If TypeOf plugin Is cEconomicDataSource Then
                        dataSource = DirectCast(plugin, cEconomicDataSource)
                        Exit For
                    End If
                Next

            Catch ex As Exception
                System.Console.WriteLine("cEconomicDataSource.getInstance() Error: " & ex.Message)
            End Try

            'JS 26May10: disabled assert; plug-in may not be available on purpose
            'Debug.Assert(dataSource IsNot Nothing, "cEconomicDataSource.getInstance() Failed to create instance.")

            Return dataSource

        End Function

#End Region ' Singleton 'Shared' interface

#Region " Public interfaces "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get/set whether any economic data producer, if available, should deliver 
        ''' data for a given <see cref="IRunType">run type</see>.
        ''' </summary>
        ''' <param name="runtype"></param>
        ''' -----------------------------------------------------------------------
        Public Property EnableData(ByVal runtype As IRunType) As Boolean _
            Implements IExternalData.EnableData
            Get
                Return s_core.PluginManager.EnableData(GetType(IEconomicData), runtype)
            End Get
            Set(ByVal value As Boolean)
                s_core.PluginManager.EnableData(GetType(IEconomicData), runtype) = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' States whether a plug-in capable of delivering Econimic data is available.
        ''' </summary>
        ''' <param name="runtype">The core run type to check availability for.</param>
        ''' <returns>True if available.</returns>
        ''' -----------------------------------------------------------------------
        Public Function IsDataAvailable(ByVal runtype As EwEUtils.Core.IRunType) As Boolean _
              Implements IExternalData.IsDataAvailable
            Return s_core.PluginManager.IsDataAvailable(GetType(IEconomicData), runtype)
        End Function

#End Region ' Public interfaces

#Region " Private methods "

        Private Sub FireonEconomicData(ByVal data As IEconomicData)
            Try
                RaiseEvent onEconomicData(data)
            Catch ex As Exception
                Debug.Assert(False, Me.ToString & "FireonEconomicData() Error: " & ex.Message)
            End Try
        End Sub

        Private Shared ReadOnly Property InternalName() As String
            Get
                Return GetType(cEconomicDataSource).ToString
            End Get
        End Property

#End Region ' Private methods

#Region " IDataConsumerPlugin implementation "

        Public Function ReceiveData(ByVal strDataName As String, ByVal data As EwEPlugin.Data.IPluginData) As Boolean _
            Implements EwEPlugin.Data.IDataConsumerPlugin.ReceiveData

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

        Public ReadOnly Property Author() As String _
            Implements EwEPlugin.IPlugin.Author
            Get
                Return "UBC Fisheries Centre"
            End Get
        End Property

        Public ReadOnly Property Contact() As String _
            Implements EwEPlugin.IPlugin.Contact
            Get
                Return "mailto:support@ecopath.org"
            End Get
        End Property

        Public ReadOnly Property Description() As String _
            Implements EwEPlugin.IPlugin.Description
            Get
                Return "Core plugin to provide economic data from an external source."
            End Get
        End Property

        Public Sub Initialize(ByVal core As Object) _
            Implements EwEPlugin.IPlugin.Initialize
            s_core = DirectCast(core, cCore)
        End Sub

        Public ReadOnly Property Name() As String _
            Implements EwEPlugin.IPlugin.Name
            Get
                Return cEconomicDataSource.InternalName
            End Get
        End Property

#End Region ' IDataConsumerPlugin implementation

    End Class

End Namespace ' ExternalData

