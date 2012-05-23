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
    ''' Base spatial data adapter for inserting external spatial/temporal raster data into
    ''' Ecospace map data structures.
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

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Initialize the adapter in response to an Ecospace scenario (re)load.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Friend Overridable Sub Initialize()

            Me.m_converters = Nothing
            Me.m_datasets = Nothing

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
            Return cv.IsConfigured() And ds.IsConfigured()

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set a <see cref="ISpatialDataSet">data set</see> for layer <paramref name="iIndex"/>.
        ''' </summary>
        ''' <param name="iIndex">Layer index [0, <see cref="Length"/>].</param>
        ''' -------------------------------------------------------------------
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

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set a <see cref="ISpatialDataConverter">data converter</see> 
        ''' for layer <paramref name="iIndex"/>.
        ''' </summary>
        ''' <param name="iIndex">Layer index [0, <see cref="Length"/>].</param>
        ''' -------------------------------------------------------------------
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

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the <see cref="eVarNameFlags">variable name</see> for the type
        ''' of layer that this adapter operates onto.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property VarName() As eVarNameFlags
            Get
                Return Me.m_varName
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Perform pre-run initializations. 
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overridable Sub InitRun()
            ' NOP
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Perform post-run cleanup. 
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overridable Sub EndRun()
            ' NOP
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Populate the core data that this adapter is responsible for.
        ''' </summary>
        ''' <param name="iTime">The one-based Ecospace time step to populate data for.</param>
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

            ' For each layer for this adapter
            For Each layer In bm.Layers(Me.m_varName)

                ' Get dataset and converter
                Dim ds As ISpatialDataSet = Me.Dataset(layer.Index)
                Dim cv As ISpatialDataConverter = Me.Converter(layer.Index)

                ' Has both?
                If (ds IsNot Nothing) And (cv IsNot Nothing) Then
                    ' #Yes: allowed to execute?
                    If ds.IsConfigured And cv.IsConfigured Then
                        ' #Yes: has data for this time step?
                        dt = Me.m_core.EcospaceTimestepToAbsoluteTime(iTime)

                        If (ds.HasDataAtT(dt, bm.PosTopLeft, bm.PosBottomRight)) Then
                            ' #Yes: Can load that data?
                            If (ds.LoadDataAtT(dt, dCellSize, bm.PosTopLeft, bm.PosBottomRight)) Then
                                ' #Yes: extract external data
                                Me.m_core.SpatialOperationLog.BeginLayerLog(iTime, dt, layer)

                                Try
                                    ' The raster returned here MUST have the extent and projection compatible with Ecospace
                                    dataExternal = ds.GetRaster(cv, layer.Name)
                                Catch ex As Exception
                                    Me.m_core.SpatialOperationLog.LogOperation(String.Format(My.Resources.CoreMessages.STATUS_EXCEPTION, ex.Message), eStatusFlags.ErrorEncountered)
                                    cLog.Write(ex, "cSpatialDataAdapter::Populate(" & layer.ToString() & ")")
                                End Try

                                If (dataExternal IsNot Nothing) Then

                                    ' Stop any validation
                                    Dim bAllow As Boolean = layer.AllowValidation
                                    layer.AllowValidation = False

                                    ' Integrate data
                                    Me.Adapt(bm, layer, iTime, dt, dataExternal)

                                    ' Restore layer validation
                                    layer.AllowValidation = bAllow

                                    ' Done, clean up
                                    dataExternal.Dispose()
                                    dataExternal = Nothing

                                    ' Notify core - use AddedOrRemoved flag to not dirty the DB; just broadcast the layer change
                                    Me.m_core.onChanged(layer, eMessageType.DataAddedOrRemoved)

                                Else
                                    Dim strMsg As String = "cSpatialDataAdapter::Populate({0}) external data missing for T{2}, ext({3},{4}) to ({5},{6}), cell size {7}"
                                    cLog.Write(String.Format(strMsg, layer.ToString(), ds.DisplayName, iTime, bm.PosTopLeft.X, bm.PosTopLeft.Y, bm.PosBottomRight.X, bm.PosBottomRight.Y, dCellSize), cLog.eVerboseLevel.Detailed)
                                End If

                                ' Unload dataset
                                ds.Unload()
                                Me.m_core.SpatialOperationLog.EndLayerLog()
                            Else
                                Dim strMsg As String = "cSpatialDataAdapter::Populate({0}) dataset {1} failed to load data for T{2}, ext({3},{4}) to ({5},{6}), cell size {7}"
                                cLog.Write(String.Format(strMsg, layer.ToString(), ds.DisplayName, iTime, bm.PosTopLeft.X, bm.PosTopLeft.Y, bm.PosBottomRight.X, bm.PosBottomRight.Y, dCellSize), cLog.eVerboseLevel.Detailed)
                            End If
                        Else
                            Dim strMsg As String = "cSpatialDataAdapter::Populate({0}) dataset {1} missing data for T{2}, ext({3},{4}) to ({5},{6})"
                            cLog.Write(String.Format(strMsg, layer.ToString(), ds.DisplayName, iTime, bm.PosTopLeft.X, bm.PosTopLeft.Y, bm.PosBottomRight.X, bm.PosBottomRight.Y), cLog.eVerboseLevel.Detailed)
                        End If
                    Else
                        Dim strMsg As String = "cSpatialDataAdapter::Populate({0}) dataset {1} or converter {2} not configured"
                        cLog.Write(String.Format(strMsg, layer.ToString(), ds.DisplayName, cv.DisplayName()), cLog.eVerboseLevel.Detailed)
                    End If
                Else
                    'cLog.Write("cSpatialDataAdapter.Populate: layer " & layer.ToString() & " not connected", cLog.eVerboseLevel.Detailed)
                End If
            Next

            Return bSuccess

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Load data from an external data raster into an Ecospace array.
        ''' </summary>
        ''' <param name="bm">The <see cref="cEcospaceBasemap"/> for the scenario to load data into.</param>
        ''' <param name="layer">The <see cref="cEcospaceLayer"/> that will receive the data.</param>
        ''' <param name="iTime">The Ecospace time step to load data for.</param>
        ''' <param name="dataExternal">The <see cref="ISpatialRaster"/> that holds the loaded external data.</param>
        ''' <returns>True if successful.</returns>
        ''' <remarks>Note that this method writes values straight into the underlying data structures!</remarks>
        ''' -------------------------------------------------------------------
        Protected Friend Overridable Function Adapt(ByVal bm As cEcospaceBasemap, _
                                                    ByVal layer As cEcospaceLayer, _
                                                    ByVal iTime As Integer, _
                                                    ByVal dt As Date, _
                                                    ByVal dataExternal As ISpatialRaster) As Boolean

            ' To ensure proper usage by inherited classes
            Debug.Assert(bm IsNot Nothing)
            Debug.Assert(layer IsNot Nothing)
            Debug.Assert(dataExternal IsNot Nothing)

            Dim layerDepth As cEcospaceLayerDepth = bm.LayerDepth
            Dim msg As cMessage = Nothing
            Dim sValue As Single = 0
            Dim bSuccess As Boolean = True ' Think positive. Really
            Dim iRow As Integer
            Dim iCol As Integer

            Try
                ' For all rows
                iRow = 1
                While (iRow <= bm.InRow) And (bSuccess = True)
                    ' For all columns
                    iCol = 1
                    While (iCol <= bm.InCol) And (bSuccess = True)
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
                    End While ' iCol
                    iRow += 1
                End While ' iRow

                If bSuccess Then
                    Me.m_core.SpatialOperationLog.LogOperation(String.Format(My.Resources.CoreMessages.STATUS_SPATIALTEMPORAL_APPLIED, dataExternal.ToString()), eStatusFlags.OK)
                End If

            Catch ex As Exception
                Me.m_core.SpatialOperationLog.LogOperation(String.Format(My.Resources.CoreMessages.STATUS_EXCEPTION, ex.Message), eStatusFlags.ErrorEncountered)
                cLog.Write(ex, "cSpatialDataAdapter::Adapt(" & layer.ToString() & ")")
                bSuccess = False
            End Try

            Return bSuccess

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Set a cell value into the underlying layer.
        ''' </summary>
        ''' <param name="layer">The layer to set the value into.</param>
        ''' <param name="iRow">One-based row index for setting the value.</param>
        ''' <param name="iCol">One-based column index for setting the value.</param>
        ''' <param name="sCellValueAtT">The value to set in the cell, as obtained from 
        ''' external data.</param>
        ''' <returns>True if successful.</returns>
        ''' -------------------------------------------------------------------
        Protected Overridable Function SetCell(ByVal layer As cEcospaceLayer, _
                                               ByVal iRow As Integer, _
                                               ByVal iCol As Integer, _
                                               ByVal sCellValueAtT As Single) As Boolean
            Try
                layer.Cell(iRow, iCol) = sCellValueAtT
            Catch ex As Exception

                Dim strMsg As String = "cSpatialDataAdapter::SetCell({0}) at ({1},{2})={3}: exception {4}"
                cLog.Write(ex, String.Format(strMsg, layer.ToString, iCol, iRow, sCellValueAtT))

                Me.m_core.SpatialOperationLog.LogOperation(String.Format(My.Resources.CoreMessages.STATUS_EXCEPTION, ex.Message), eStatusFlags.ErrorEncountered)
                Return False
            End Try
            Return True

        End Function

#End Region ' Basic bits

    End Class

End Namespace
