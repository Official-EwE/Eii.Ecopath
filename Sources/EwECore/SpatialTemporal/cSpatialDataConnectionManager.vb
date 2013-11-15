' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 2 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see <http://www.gnu.org/licenses/gpl-2.0.html>. 
'
' Copyright 1991-2013 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports EwEUtils.SpatialData
Imports EwEUtils.Core
Imports System.Drawing
Imports EwEPlugin
Imports EwEUtils.Utilities

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
        Implements ICoreInterface

#Region " Variables "

        ''' <summary>Manager of active data sets.</summary>
        Private m_datasetManager As cSpatialDataSetManager = Nothing
        Private m_core As cCore = Nothing
        Private m_data As cSpatialDataStructures = Nothing

        ''' <summary>Pre-determined number of configured data connections</summary>
        Private m_iNumConnected As Integer = cCore.NULL_VALUE

#End Region ' Variables

#Region " Construction/ destruction "

        Friend Sub New()
        End Sub

        Friend Sub Init(ByVal core As cCore, ByVal data As cSpatialDataStructures)

            Me.m_core = core
            Me.m_data = data
            Me.m_datasetManager = New cSpatialDataSetManager(core)

            Me.CreateAdapters()

        End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            If (Me.m_core IsNot Nothing) Then
                Me.Clear()
                Me.m_core = Nothing
            End If
            GC.SuppressFinalize(Me)
        End Sub

#End Region ' Construction/ destruction

#Region " Generic information "

        Public Sub Load()

            Dim guid As Guid
            Dim ds As ISpatialDataSet = Nothing
            Dim cv As ISpatialDataConverter = Nothing
            Dim cfg As cSpatialDataStructures.cAdapaterConfiguration = Nothing
            Dim t As Type = Nothing

            For Each adt As cSpatialDataAdapter In Me.Adapters
                For i As Integer = 0 To adt.Length - 1

                    ds = Nothing
                    cv = Nothing
                    cfg = Me.m_data.Item(adt.VarName, i)

                    If (cfg IsNot Nothing) Then

                        ' Try to resolve dataset
                        If guid.TryParse(cfg.DatasetGUID, guid) Then
                            ds = Me.m_datasetManager.ItemByGUID(guid)
                        End If

                        ' Try to create converter
                        t = cTypeUtils.StringToType(cfg.Converter)
                        If (t IsNot Nothing) Then
                            Try
                                cv = DirectCast(Activator.CreateInstance(t), ISpatialDataConverter)

                                If TypeOf (adt) Is cSpatialScalarDataAdapterBase Then
                                    With DirectCast(adt, cSpatialScalarDataAdapterBase)
                                        .DataScale(i) = cfg.Scale
                                        .DataScaleType(i) = DirectCast(cfg.ScaleType, cSpatialScalarDataAdapterBase.eScaleType)
                                    End With
                                End If

                                ' Properly initialuize
                                If (TypeOf cv Is IPlugin) Then DirectCast(cv, IPlugin).Initialize(Me.m_core)
                            Catch ex As Exception

                            End Try
                        End If
                    End If

                    adt.Dataset(i) = ds
                    adt.Converter(i) = cv
                Next
            Next

            ' Invalidate connection count
            Me.m_iNumConnected = cCore.NULL_VALUE

        End Sub

        Public Sub Update()

            Dim ds As ISpatialDataSet = Nothing
            Dim cv As ISpatialDataConverter = Nothing
            Dim cfg As cSpatialDataStructures.cAdapaterConfiguration = Nothing

            For Each adt As cSpatialDataAdapter In Me.Adapters
                For i As Integer = 0 To adt.Length - 1

                    ds = adt.Dataset(i)
                    cv = adt.Converter(i)
                    cfg = Me.m_data.Item(adt.VarName, i)

                    If (cfg IsNot Nothing) Then
                        If (ds IsNot Nothing) Then
                            cfg.DatasetGUID = ds.GUID.ToString
                        Else
                            cfg.DatasetGUID = ""
                        End If

                        If (cv IsNot Nothing) Then
                            cfg.Converter = cTypeUtils.TypeToString(cv.GetType)
                        Else
                            cfg.Converter = ""
                        End If
                        cfg.ConverterConfig = ""

                        If TypeOf adt Is cSpatialScalarDataAdapterBase Then
                            With DirectCast(adt, cSpatialScalarDataAdapterBase)
                                cfg.Scale = CSng(.DataScale(i))
                                cfg.ScaleType = CByte(.DataScaleType(i))
                            End With
                        End If

                    End If
                Next
            Next

            ' Invalidate connection count
            Me.m_iNumConnected = cCore.NULL_VALUE

            Me.m_core.onChanged(Me, eMessageType.DataModified)

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the number of <see cref="cSpatialDataAdapter.IsConnected">live data connections</see>.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property NumConnectedAdapters As Integer
            Get
                If (Me.m_iNumConnected = cCore.NULL_VALUE) Then
                    Me.UpdateConnectionCount()
                End If
                Return Me.m_iNumConnected
            End Get
        End Property

#End Region ' Generic information

#Region " Adapters "

        Public ReadOnly Property Adapter(ByVal varname As eVarNameFlags) As cSpatialDataAdapter
            Get
                For Each adt As cSpatialDataAdapter In Me.m_data.DataAdapters
                    If (adt.VarName = varname) Then
                        Return adt
                    End If
                Next
                Return Nothing
            End Get
        End Property

        Public ReadOnly Property nAdapters As Integer
            Get
                Return Me.m_data.DataAdapters.Count
            End Get
        End Property

        Public ReadOnly Property Adapters() As cSpatialDataAdapter()
            Get
                Return Me.m_data.DataAdapters.ToArray
            End Get
        End Property

        Public Sub AddAdapter(adapter As cSpatialDataAdapter)
            Me.m_data.DataAdapters.Add(adapter)
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
        Public Function LoadSystemSettings(Optional strFile As String = "") As Boolean
            Try
                Return Me.m_datasetManager.Load(strFile, False)
            Catch ex As Exception
                cLog.Write(ex, "cSpatialDataConnectionManager.Load")
            End Try
            Return False
        End Function

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

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns an array of data converter templates compatible with a <see cref="ISpatialDataSet"/>.
        ''' </summary>
        ''' <returns>An array of compatible <see cref="ISpatialDataConverter">converters</see>.</returns>
        ''' -------------------------------------------------------------------
        Public Function ConverterTemplates(ds As ISpatialDataSet) As ISpatialDataConverter()

            Dim lConverters As New List(Of ISpatialDataConverter)
            Dim pm As cPluginManager = Me.m_core.PluginManager

            If (pm IsNot Nothing And ds IsNot Nothing) Then
                For Each ip As IPlugin In pm.GetPlugins(GetType(ISpatialDataConverterPlugin))
                    If (TypeOf ip Is ISpatialDataConverter) Then
                        Dim conv As ISpatialDataConverter = DirectCast(ip, ISpatialDataConverter)
                        If (conv.IsCompatible(ds)) Then
                            lConverters.Add(conv)
                        End If
                    End If
                Next
            End If
            Return lConverters.ToArray

        End Function

#End Region ' Converters

#Region " ICoreInterface implementation "

        Public ReadOnly Property CoreComponent As EwEUtils.Core.eCoreComponentType Implements ICoreInterface.CoreComponent
            Get
                Return eCoreComponentType.EcoSpace
            End Get
        End Property

        Public ReadOnly Property DataType As EwEUtils.Core.eDataTypes Implements ICoreInterface.DataType
            Get
                Return eDataTypes.EcospaceSpatialDataConnection
            End Get
        End Property

        Public Property DBID As Integer Implements ICoreInterface.DBID
            Get
                Return -1
            End Get
            Set(value As Integer)
                ' NOP
            End Set
        End Property

        Public Function GetID() As String Implements ICoreInterface.GetID
            Return ""
        End Function

        Public Property Index As Integer Implements ICoreInterface.Index
            Get
                Return -1
            End Get
            Set(value As Integer)
                ' NOP
            End Set
        End Property

        Public Property Name As String Implements ICoreInterface.Name
            Get
                Return "SpatialDataConnectionManager"
            End Get
            Set(value As String)

            End Set
        End Property

#End Region ' ICoreInterface implementation

#Region " Internals "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Create fixed data adapters for ecospace data layers.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub CreateAdapters()

            Me.Clear()


            Me.AddAdapter(New cRelPPDataAdapter(Me.m_core, eVarNameFlags.LayerRelPP, eCoreCounterTypes.NotSet))
            Me.AddAdapter(New cSpatialScalarDataAdapter(Me.m_core, eVarNameFlags.LayerRelCin, eCoreCounterTypes.NotSet))
            Me.AddAdapter(New cCapacityDataAdapter(Me.m_core, eVarNameFlags.LayerHabitatCapacityInput, eCoreCounterTypes.nGroups))
            Me.AddAdapter(New cCapacityDataAdapter(Me.m_core, eVarNameFlags.LayerDriver, eCoreCounterTypes.nEnvironmentalDriverLayers))
            Me.AddAdapter(New cBiomassForcingAdapter(Me.m_core, eVarNameFlags.LayerBiomassForcing, eCoreCounterTypes.nGroups))

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Clear fixed data adapters.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub Clear()
            Me.m_data.DataAdapters.Clear()
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Update the count of configured adapters.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub UpdateConnectionCount()

            Me.m_iNumConnected = 0
            For Each adt As cSpatialDataAdapter In Me.Adapters
                For i As Integer = 0 To adt.Length - 1
                    If adt.IsConnected(i) Then m_iNumConnected += 1
                Next
            Next
        End Sub

#End Region ' Internals

    End Class

End Namespace
