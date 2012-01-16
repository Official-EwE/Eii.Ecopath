#Region " Imports "

Option Strict On
Imports EwEUtils.SpatialData
Imports EwEUtils.Core
Imports System.Drawing
Imports EwEPlugin

#End Region ' Imports

Namespace SpatialData

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Manages the connections between available <see cref="cSpatialDataAdapter"/>s 
    ''' and <see cref="ISpatialDataSet"/>s
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cSpatialDataConnectionManager
        Implements IDisposable

#Region " Variables "

        ''' <summary>Manager of active data sets.</summary>
        Private m_datasetManager As cSpatialDataSetManager = Nothing
        ''' <summary>Hard-coded dictionary of data adapters.</summary>
        Private m_dtAdapters As Dictionary(Of eVarNameFlags, cSpatialDataAdapter) = Nothing

        Private m_core As cCore = Nothing

#End Region ' Variables

#Region " Construction/ destruction "

        Friend Sub New()
        End Sub

        Friend Sub Init(ByVal core As cCore)

            Dim adapter As cSpatialDataAdapter = Nothing

            Me.m_core = core
            Me.m_dtAdapters = New Dictionary(Of eVarNameFlags, cSpatialDataAdapter)

            Me.m_datasetManager = New cSpatialDataSetManager()

        End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            If (Me.m_core IsNot Nothing) Then
                Me.m_dtAdapters.Clear()
                Me.m_dtAdapters = Nothing
                Me.m_core = Nothing
            End If
            GC.SuppressFinalize(Me)
        End Sub

#End Region ' Construction/ destruction

        Public Sub Load()
            ' Spatial data adapters are hard-coded. This should perhaps change, discuss w Joe B
            Me.AddAdapter(New cSpatialDataAdapter(Me.m_core, eVarNameFlags.LayerRelPP, 1))
            Me.AddAdapter(New cSpatialDataAdapter(Me.m_core, eVarNameFlags.LayerHabitatCapacityInput, Me.m_core.GetCoreCounter(eCoreCounterTypes.nGroups)))
            Me.AddAdapter(New cSpatialDataAdapter(Me.m_core, eVarNameFlags.LayerDriver, Me.m_core.GetCoreCounter(eCoreCounterTypes.nEnvironmentalLayers)))
        End Sub

        Public Sub Clear()
            Me.m_dtAdapters.Clear()
        End Sub

#Region " Adapters "

        Public ReadOnly Property Adapter(ByVal varname As eVarNameFlags) As cSpatialDataAdapter
            Get
                If Me.m_dtAdapters.ContainsKey(varname) Then
                    Return Me.m_dtAdapters(varname)
                End If
                Return Nothing
            End Get
        End Property

        Public ReadOnly Property nAdapters As Integer
            Get
                Return Me.m_dtAdapters.Count
            End Get
        End Property

        Public ReadOnly Property Adapters() As cSpatialDataAdapter()
            Get
                Dim lAdapters As New List(Of cSpatialDataAdapter)
                For Each ad As cSpatialDataAdapter In Me.m_dtAdapters.Values
                    lAdapters.Add(ad)
                Next
                Return lAdapters.ToArray
            End Get
        End Property

        Private Sub AddAdapter(adapter As cSpatialDataAdapter)
            Me.m_dtAdapters(adapter.VarName) = adapter
            Me.m_core.m_EcoSpaceData.DataAdapter(adapter.VarName) = adapter
        End Sub

#End Region ' Adapters

#Region " Data sets "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Load the datasets.
        ''' </summary>
        ''' <param name="strFile">Optional file to load dataset information from.
        ''' If this parameter is left empty the 
        ''' <see cref="cSpatialDataSetManager.ConfigFileName">default file path</see> 
        ''' is used.</param>
        ''' -------------------------------------------------------------------
        Public Sub LoadSystemSettings(Optional strFile As String = "")
            Try
                Me.m_datasetManager.Load(strFile, False)
            Catch ex As Exception
                cLog.Write(ex, "cSpatialDataConnectionManager.Load")
            End Try
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get a dataset with a given <see cref="GUID"/>
        ''' </summary>
        ''' <param name="guidDS"></param>
        ''' <returns>A dataset, or nothing if no matching dataset could be found.</returns>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property Dataset(ByVal guidDS As Guid) As ISpatialDataSet
            Get
                Return Me.m_datasetManager.ItemByGUID(guidDS)
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the number of datasets in the manager
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property nDatasets As Integer
            Get
                Return Me.m_datasetManager.Count
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get a dataset at a given position.
        ''' </summary>
        ''' <param name="iDataset">Zero-based index of the dataset.</param>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property Dataset(ByVal iDataset As Integer) As ISpatialDataSet
            Get
                Debug.Assert(iDataset > 0 And iDataset <= nDatasets, "Index out of range")
                Return Me.m_datasetManager(iDataset)
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns an array of dataset templates.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Function DatasetTemplates() As ISpatialDataSet()

            Dim lDatasets As New List(Of ISpatialDataSet)
            Dim pm As cPluginManager = Me.m_core.PluginManager

            If (pm IsNot Nothing) Then
                For Each ip As IPlugin In pm.GetPlugins(GetType(ISpatialDataSetPlugin))
                    If (TypeOf ip Is ISpatialDataSet) Then
                        lDatasets.Add(DirectCast(ip, ISpatialDataSet))
                    End If
                Next
            End If
            Return lDatasets.ToArray

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns a reference to the embedded dataset manager.
        ''' </summary>
        ''' <returns>A reference to the embedded dataset manager.</returns>
        ''' -------------------------------------------------------------------
        Function DatasetManager() As cSpatialDataSetManager
            Return Me.m_datasetManager
        End Function

#End Region ' Data sets

#Region " Converters "

        ''' <summary>
        ''' Returns an array of data converter templates.
        ''' </summary>
        ''' <returns></returns>
        Public Function ConverterTemplates() As ISpatialDataConverter()

            Dim lConverters As New List(Of ISpatialDataConverter)
            Dim pm As cPluginManager = Me.m_core.PluginManager

            If (pm IsNot Nothing) Then
                For Each ip As IPlugin In pm.GetPlugins(GetType(ISpatialDataConverterPlugin))
                    If (TypeOf ip Is ISpatialDataConverter) Then
                        lConverters.Add(DirectCast(ip, ISpatialDataConverter))
                    End If
                Next
            End If
            Return lConverters.ToArray

        End Function

#End Region ' Converters

    End Class

End Namespace
