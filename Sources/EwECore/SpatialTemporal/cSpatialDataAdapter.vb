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
    ''' Base spatial data adapter to insert external spatial/temporal map data into
    ''' the Ecospace data structures at any given moment.
    ''' </summary>
    Public Class cSpatialDataAdapter
        Inherits cCoreInputOutputBase

#Region " Private vars "

        ''' <summary>Converter for each layer.</summary>
        Protected m_converters() As ISpatialDataConverter
        ''' <summary>Dataset for each layer.</summary>
        Protected m_datasets() As ISpatialDataSet

        ''' <summary>Ecospace variable to operate onto.</summary>
        Protected m_varName As eVarNameFlags = Nothing
        ''' <summary>Core counter that this adapter operates onto.</summary>
        Protected m_coreCounter As eCoreCounterTypes = eCoreCounterTypes.NotSet

#End Region ' Private vars

#Region " Constructor "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Create a new instance of this class.
        ''' </summary>
        ''' <param name="core">The core to use.</param>
        ''' <param name="varName">The ecospace layer, identified by <see cref="eVarNameFlags">varname</see>,
        ''' that this adapter will interface with.</param>
        ''' <param name="cc">The <see cref="eCoreCounterTypes">core counter</see> that states the
        ''' number of layers that this adapter will interface with.</param>
        ''' -------------------------------------------------------------------
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

        ''' <summary>
        ''' Redimension when an Ecospace scenario has loaded. 
        ''' </summary>
        Friend Overridable Sub Initialize()
            Dim iNumItems As Integer = Math.Max(0, Me.m_core.GetCoreCounter(Me.m_coreCounter))
            ReDim Me.m_converters(iNumItems)
            ReDim Me.m_datasets(iNumItems)
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
        ''' Return whether a layer in this adapter is connected to external data.
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

        ''' <summary>
        ''' Get/set a data converter for this 
        ''' </summary>
        ''' <param name="iIndex"></param>
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

        Public ReadOnly Property VarName() As eVarNameFlags
            Get
                Return Me.m_varName
            End Get
        End Property

        ''' <summary>
        ''' Perform pre-run initializations. 
        ''' </summary>
        Public Overridable Sub InitRun()
            ' NOP
        End Sub

        ''' <summary>
        ''' Perform post-run cleanup. 
        ''' </summary>
        Public Overridable Sub EndRun()
            ' NOP
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Populate the core data that this adapter is responsible for.
        ''' </summary>
        ''' <param name="iTime">The Ecospace time step to populate data for.</param>
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

            ' Perhaps this should be called by the core?
            If (iTime = 1) Then
                Me.InitRun()
            End If

            If Not Me.PrePopulate(iTime) Then Return False

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

                                    ' Notify core - use AddedOrRemoved flag to not dirty the DB; just broadcast the layer change
                                    Me.m_core.onChanged(layer, eMessageType.DataAddedOrRemoved)

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

            Return bSuccess And Me.PostPopulate(iTime, bSuccess)

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Pre-population insertion point for inheriting classes.
        ''' </summary>
        ''' <param name="iTime">Ecospace time step.</param>
        ''' <returns>Return false to abort the population process.</returns>
        ''' -------------------------------------------------------------------
        Protected Overridable Function PrePopulate(iTime As Integer) As Boolean
            Return True
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Pre-population insertion point for inheriting classes.
        ''' </summary>
        ''' <param name="iTime">Ecospace time step.</param>
        ''' <param name="bSuccess">Flag stating whether the populate process was
        ''' successful.</param>
        ''' <returns>True by default.</returns>
        ''' -------------------------------------------------------------------
        Protected Overridable Function PostPopulate(iTime As Integer, bSuccess As Boolean) As Boolean
            Return True
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

            If Not Me.PreAdapt(bm, layer, iTime) Then Return False

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
                                bSuccess = bSuccess And Me.SetCell(layer, iRow, iCol, sValue)
                            End If
                        End If
                    Next iCol
                Next iRow

#If DEBUG Then
                Console.WriteLine("Adapted raster " & VarName & " at " & iTime & ":")
                Console.WriteLine("   Mean: " & dataExternal.Mean)
                Console.WriteLine("   Min.: " & dataExternal.Min)
                Console.WriteLine("   Max.: " & dataExternal.Max)
                Console.WriteLine("   Num.: " & dataExternal.NumValueCells)
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

            bSuccess = bSuccess And Me.PostAdapt(bm, layer, iTime, bSuccess)

            Return bSuccess
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="bm"></param>
        ''' <param name="layer"></param>
        ''' <param name="iTime"></param>
        ''' <returns>Return false to cancel the adaptation process.</returns>
        ''' -------------------------------------------------------------------
        Protected Overridable Function PreAdapt(ByVal bm As cEcospaceBasemap, _
                                                ByVal layer As cEcospaceLayer, _
                                                ByVal iTime As Integer) As Boolean
            Return True
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Called after a single layer has been adapted.
        ''' </summary>
        ''' <param name="bm">Basemap</param>
        ''' <param name="layer">Layer</param>
        ''' <param name="iTime">Time step</param>
        ''' <param name="bSuccess">Adaptation success</param>
        ''' <returns>True if successful.</returns>
        ''' -------------------------------------------------------------------
        Protected Overridable Function PostAdapt(ByVal bm As cEcospaceBasemap, _
                                                 ByVal layer As cEcospaceLayer, _
                                                 ByVal iTime As Integer, _
                                                 ByVal bSuccess As Boolean) As Boolean
            Return True
        End Function

        Protected Overridable Function SetCell(ByVal layer As cEcospaceLayer, _
                                               ByVal iRow As Integer, _
                                               ByVal iCol As Integer, _
                                               ByVal sValue As Single) As Boolean
            layer.Cell(iRow, iCol) = sValue
            Return True
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
