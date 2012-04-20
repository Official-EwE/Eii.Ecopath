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
' Copyright 1991-2012 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'
#Region " Imports "

Option Strict On
Imports EwEUtils.Core
Imports EwEUtils.SpatialData
Imports EwEUtils.Utilities

#End Region ' Imports

Namespace SpatialData

    ''' <summary>
    ''' Update core data at a given timestep.
    ''' Provide connection information.
    ''' </summary>
    Public Class cSpatialDataAdapter
        Inherits cCoreInputOutputBase

#Region " Private vars "

        ''' <summary>Converter for each layer.</summary>
        Private m_converters() As ISpatialDataConverter
        ''' <summary>Dataset for each layer.</summary>
        Private m_datasets() As ISpatialDataSet
        ''' <summary>Absolute to relative scale.</summary>
        Private m_scales() As Single

        ''' <summary>Ecospace variable to operate onto.</summary>
        Private m_varName As eVarNameFlags = Nothing
        ''' <summary>Core counter that this adapter operates onto.</summary>
        Private m_coreCounter As eCoreCounterTypes = eCoreCounterTypes.NotSet

#End Region ' Private vars

#Region " Constructor "

        Public Sub New(ByVal core As cCore, ByVal varName As eVarNameFlags, cc As eCoreCounterTypes)

            MyBase.New(core)

            Me.m_dataType = eDataTypes.SpatialDataSource
            Me.m_coreComponent = eCoreComponentType.EcoSpace
            Me.m_coreCounter = cc
            Me.m_varName = varName

            Me.DBID = -1
            Me.AllowValidation = True
        End Sub

#End Region ' Constructor

#Region " Basic bits "

        Friend Sub SetDefaults()
            Dim iNumItems As Integer = Math.Max(0, Me.m_core.GetCoreCounter(Me.m_coreCounter))
            ReDim Me.m_converters(iNumItems)
            ReDim Me.m_datasets(iNumItems)
            ReDim Me.m_scales(iNumItems)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns the number of layers for this adapter.
        ''' </summary>
        ''' <returns>The number of layers for this adapter.</returns>
        ''' -------------------------------------------------------------------
        Public Function Length() As Integer
            Return Me.m_converters.Length
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Return whether a layer is connected to external data.
        ''' </summary>
        ''' <param name="iIndex">The one-based index of the layer to query, or
        ''' <see cref="cCore.NULL_VALUE"/> if irrelevant.</param>
        ''' -------------------------------------------------------------------
        Public Function IsConnected(iIndex As Integer) As Boolean

            Dim cv As ISpatialDataConverter = Me.Converter(iIndex)
            Dim ds As ISpatialDataSet = Me.Dataset(iIndex)

            If (cv Is Nothing) Or (ds Is Nothing) Then Return False
            Return cv.IsConfigured And ds.IsEnabled

        End Function

        Public Property Converter(iIndex As Integer) As ISpatialDataConverter
            Get
                Debug.Assert(iIndex < Me.Length, "Index out of range")
                Return Me.m_converters(Math.Max(0, iIndex))
            End Get
            Set(ByVal value As ISpatialDataConverter)
                Debug.Assert(iIndex < Me.Length, "Index out of range")
                Me.m_converters(Math.Max(0, iIndex)) = value
            End Set
        End Property

        Public Property Dataset(iIndex As Integer) As ISpatialDataSet
            Get
                Debug.Assert(iIndex < Me.Length, "Index out of range")
                Return Me.m_datasets(Math.Max(0, iIndex))
            End Get
            Set(ByVal value As ISpatialDataSet)
                Debug.Assert(iIndex < Me.Length, "Index out of range")
                Me.m_datasets(Math.Max(0, iIndex)) = value
            End Set
        End Property

        Private Property DataScale(iIndex As Integer) As Single
            Get
                Debug.Assert(iIndex < Me.Length, "Index out of range")
                Return Me.m_scales(Math.Max(0, iIndex))
            End Get
            Set(value As Single)
                Debug.Assert(iIndex < Me.Length, "Index out of range")
                Me.m_scales(Math.Max(0, iIndex)) = value
            End Set
        End Property

        Public ReadOnly Property VarName() As eVarNameFlags
            Get
                Return Me.m_varName
            End Get
        End Property

        Protected Overridable Sub InitRun()
            For i As Integer = 0 To Me.m_scales.Length - 1
                Me.m_scales(i) = cCore.NULL_VALUE
            Next
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Populate the core data that this adapter is responsible for.
        ''' </summary>
        ''' <param name="iTime">The Ecospace time step to process.</param>
        ''' <returns>True if successful.</returns>
        ''' -------------------------------------------------------------------
        Protected Friend Overridable Function Populate(ByVal iTime As Integer) As Boolean

            Dim msg As cMessage = Nothing
            Dim bm As cEcospaceBasemap = Me.m_core.EcospaceBasemap
            Dim layer As cEcospaceLayer = Nothing
            Dim dataExternal As ISpatialRaster = Nothing
            Dim dCellSize As Double = CDbl(bm.CellSize)
            Dim dt As Date
            Dim bSuccess As Boolean = False

            If (iTime = 1) Then
                Me.InitRun()
            End If

            ' For each layer for this adapter
            For Each layer In bm.Layers(Me.m_varName)

                ' Get dataset and converter
                Dim ds As ISpatialDataSet = Me.Dataset(layer.Index)
                Dim cv As ISpatialDataConverter = Me.Converter(layer.Index)

                ' Has both?
                If (ds IsNot Nothing) And (cv IsNot Nothing) Then
                    ' #Yes: allowed to execute?
                    If ds.IsEnabled Then
                        ' #Yes: has data for this time step?
                        dt = Me.m_core.EcospaceTimestepToAbsoluteTime(iTime)

                        If (ds.HasDataAtT(dt, bm.PosTopLeft, bm.PosBottomRight)) Then
                            ' #Yes: Can load that data?
                            If (ds.LoadDataAtT(dt, dCellSize, bm.PosTopLeft, bm.PosBottomRight)) Then
                                ' #Yes: extract external data
#If DEBUG Then
                                Dim sw As Stopwatch = Stopwatch.StartNew()
#End If

                                Try
                                    ' The raster returned here MUST have the extent and projection compatible with Ecospace
                                    dataExternal = ds.GetRaster(cv, layer.Name)
                                Catch ex As Exception
                                    ' ToDo_JS: Globalize this message
                                    msg = New cMessage(String.Format("Ecospace obtain external data for {0} at time step {1}. Exception {2}", Me.Name, iTime, ex.Message), _
                                                       eMessageType.DataImport, eCoreComponentType.EcoSpace, eMessageImportance.Information)
                                    Me.m_core.Messages.SendMessage(msg)
                                    cLog.Write(ex, "cSpatialDataAdapter::Populate@GetRaster")
                                End Try

                                If (dataExternal IsNot Nothing) Then

                                    ' Stop any validation
                                    Dim bAllow As Boolean = layer.AllowValidation
                                    layer.AllowValidation = False

                                    ' Integrate data
                                    Me.Adapt(bm, layer, iTime, dataExternal)

                                    ' Restore layer validation
                                    layer.AllowValidation = bAllow

                                    ' Done, clean up
                                    dataExternal.Dispose()
                                    dataExternal = Nothing

                                    ' Notify core
                                    Me.m_core.onChanged(layer)

                                End If

                                ' Unload dataset
                                ds.Unload()
#If DEBUG Then
                                sw.Stop()
                                Console.WriteLine("SpatialDataAdapter {0}::{1} {2} ms", Me.Name, layer.Name, sw.ElapsedMilliseconds)
#End If
                            End If
                        Else
                            Console.WriteLine(">> SpatialDataAdapter {0}::{1} is disabled <<", Me.Name, layer.Name)
                        End If
                    End If
                End If
            Next

            Return bSuccess

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Adapt loaded external data into a target Ecospace array.
        ''' </summary>
        ''' <param name="bm">The <see cref="cEcospaceBasemap"/> for the scenario to load data into.</param>
        ''' <param name="layer">The <see cref="cEcospaceLayer"/> that will receive the data.</param>
        ''' <param name="iTime">The Ecospace time step to load data for.</param>
        ''' <param name="dataExternal">The <see cref="ISpatialRaster"/> that holds the loaded external data.</param>
        ''' <returns>True if successful.</returns>
        ''' <remarks>Note that this method writes values straight into the underlying data structures!</remarks>
        ''' -------------------------------------------------------------------
        Protected Friend Function Adapt(ByVal bm As cEcospaceBasemap, _
                                        ByVal layer As cEcospaceLayer, _
                                        ByVal iTime As Integer, _
                                        ByVal dataExternal As ISpatialRaster) As Boolean

            ' To ensure proper usage by inherited classes
            Debug.Assert(bm IsNot Nothing)
            Debug.Assert(layer IsNot Nothing)
            Debug.Assert(dataExternal IsNot Nothing)

            Dim layerDepth As cEcospaceLayerDepth = bm.LayerDepth
            Dim msg As cMessage = Nothing
            Dim sValue As Single = 0
            'Dim sScale As Single = Me.LayerScale(bm, layer, dataExternal)
            Dim bSuccess As Boolean = True ' Think positive. Really

            Try
                ' For all rows
                For iRow As Integer = 1 To bm.InRow
                    ' For all columns
                    For iCol As Integer = 1 To bm.InCol
                        ' Is a water cell or is this layer affecting depth?
                        If layerDepth.IsWaterCell(iRow, iCol) Or (Me.m_varName = eVarNameFlags.LayerDepth) Then
                            ' #Yes: get value
                            sValue = CSng(dataExternal.Cell(iRow, iCol))
                            ' Is a valid value?
                            If (sValue <> cCore.NULL_VALUE) Then
                                ' #Yes: set value
                                layer.Cell(iRow, iCol) = sValue
                            End If
                        End If
                    Next iCol
                Next iRow

#If DEBUG Then
                '' Validate recalculated absolute values
                'Dim ds As ISpatialDataSet = Me.Dataset(layer.Index)
                'Dim sBase As Single = Me.GetBaseValue()
                'If (Not ds.IsRelativeValues) And (iTime = 1) Then
                '    Debug.Assert(cNumberUtils.Approximates(sBase, sTot, (sBase + sTot) / 200))
                'End If

                Console.WriteLine("Adapting raster " & VarName & " at " & iTime & ":")
                Console.WriteLine("   Mean: " & dataExternal.Mean)
                Console.WriteLine("   Min.: " & dataExternal.Min)
                Console.WriteLine("   Max.: " & dataExternal.Max)
#End If

            Catch ex As Exception
                ' Whoah!
                ' ToDo_JS: Globalize this message
                msg = New cMessage(String.Format("Ecospace insert external data for {0} at time step {1} into {2}. Exception {3}", Me.Name, iTime, layer.Name, ex.Message), _
                                   eMessageType.DataImport, eCoreComponentType.EcoSpace, eMessageImportance.Information)
                Me.m_core.Messages.SendMessage(msg)
                bSuccess = False
                cLog.Write(ex, "cSpatialDataAdapter::LoadData")
            End Try

            Return bSuccess
        End Function

        'Protected Function LayerScale(ByVal bm As cEcospaceBasemap, _
        '                              ByVal layer As cEcospaceLayer, _
        '                              ByVal dataExternal As ISpatialRaster) As Single

        '    ' To ensure proper usage by inherited classes
        '    Debug.Assert(bm IsNot Nothing)
        '    Debug.Assert(layer IsNot Nothing)
        '    Debug.Assert(dataExternal IsNot Nothing)

        '    Dim ds As ISpatialDataSet = Me.Dataset(layer.Index)
        '    If ds.IsRelativeValues Then Return 1

        '    Dim sScale As Single = Me.DataScale(layer.Index)
        '    If (sScale <> cCore.NULL_VALUE) Then Return sScale

        '    Dim layerDepth As cEcospaceLayerDepth = bm.LayerDepth
        '    Dim msg As cMessage = Nothing
        '    Dim sTot As Single = 0
        '    Dim sValue As Single = 0
        '    Dim iNum As Integer = 0

        '    Try
        '        ' For all rows
        '        For iRow As Integer = 1 To bm.InRow
        '            ' For all columns
        '            For iCol As Integer = 1 To bm.InCol
        '                ' Is a water cell or is this layer affecting depth?
        '                If layerDepth.IsWaterCell(iRow, iCol) Or (Me.m_varName = eVarNameFlags.LayerDepth) Then
        '                    ' #Yes: get value
        '                    sValue = CSng(dataExternal.Cell(iRow, iCol))
        '                    ' Is a valid value?
        '                    If (sValue <> cCore.NULL_VALUE) Then
        '                        ' #Yes: calc
        '                        sTot += sValue : iNum += 1
        '                    End If
        '                End If
        '            Next iCol
        '        Next iRow

        '        ' Calc scale, allowing for errors
        '        If (sTot <= 0 Or iNum <= 0) Then    '(sBase <= 0) Or
        '            Debug.Assert(False)
        '            sScale = 1
        '        Else
        '             Dim sAvg As Single = sTot / iNum
        '            sScale = 1 / sAvg ' Scale Ecopath total to map average
        '        End If
        '        Me.DataScale(layer.Index) = sScale

        '        Console.WriteLine(">> Scaling factor for layer {0} is {1}", Me.VarName.ToString, sScale)

        '    Catch ex As Exception
        '        ' Whoah!
        '        ' ToDo_JS: Globalize this message
        '        msg = New cMessage(String.Format("Ecospace cacl scale for {0} into {1}. Exception {2}", Me.Name, layer.Name, ex.Message), _
        '                           eMessageType.DataImport, eCoreComponentType.EcoSpace, eMessageImportance.Information)
        '        Me.m_core.Messages.SendMessage(msg)
        '        sScale = 0
        '        cLog.Write(ex, "cSpatialDataAdapter::LoadData")
        '    End Try

        '    Return sScale
        'End Function

#End Region ' Basic bits

    End Class

End Namespace
