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
    ''' Derived spatial data adapter to insert scaled external spatial/temporal map data into
    ''' the Ecospace data structures at any given moment. This adapter maintains a scale
    ''' for every map layer attached to the adapter, and will translate map values
    ''' to relative values when <see cref="cSpatialScalarDataAdapter.DataScaleType"/> is set to
    ''' <see cref="cSpatialScalarDataAdapter.eScaleType.Relative">relative</see>.
    ''' </summary>
    Public Class cSpatialScalarDataAdapter
        Inherits cSpatialDataAdapter

#Region " Private variables "

        Private m_scales() As Single
        Private m_scaleType() As eScaleType

#End Region ' Private variables

#Region " Constructor "

        Public Sub New(ByVal core As cCore, ByVal varName As eVarNameFlags, ByVal cc As eCoreCounterTypes)
            MyBase.New(core, varName, cc)
        End Sub

#End Region ' Constructor

#Region " Public access "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Supported value conversion modes for a <see cref="cSpatialScalarDataAdapter"/>.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Enum eScaleType As Byte
            ''' <summary>Values are applied as-is: no scaling is performed.</summary>
            Absolute = 0
            ''' <summary>Value are scaled before being applied.</summary>
            Relative
        End Enum

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the <see cref="eScaleType">scale type</see> for the layer identified by <paramref name="iIndex"/>.
        ''' If set to <see cref="eScaleType.Relative"/>, external values are <see cref="DataScale">scaled</see>.
        ''' </summary>
        ''' <param name="iIndex">Layer index [0, <see cref="Length"/>]</param>
        ''' -------------------------------------------------------------------
        Public Property DataScaleType(iIndex As Integer) As eScaleType
            Get
                Debug.Assert(iIndex < Me.Length, "Index out of range")
                Return Me.m_scaleType(Math.Max(0, iIndex))
            End Get
            Set(value As eScaleType)
                Debug.Assert(iIndex < Me.Length, "Index out of range")
                Me.m_scaleType(Math.Max(0, iIndex)) = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the external data scale value. Scaling occurs only when the
        ''' <see cref="DataScaleType"/> for layer <paramref name="iIndex"/> is
        ''' set to <see cref="eScaleType.Relative"/>.
        ''' </summary>
        ''' <param name="iIndex">Layer index [0, <see cref="Length"/>]</param>
        ''' -------------------------------------------------------------------
        Public Property DataScale(iIndex As Integer) As Single
            Get
                Debug.Assert(iIndex < Me.Length, "Index out of range")
                Return Me.m_scales(Math.Max(0, iIndex))
            End Get
            Set(value As Single)
                Debug.Assert(iIndex < Me.Length, "Index out of range")
                Me.m_scales(Math.Max(0, iIndex)) = value
            End Set
        End Property

        Public Function CalculateScaleFromEcopathTimePeriod(Optional iFirstTimeStep As Integer = 1) As cDatasetCompatilibity.eCompatibilityTypes

            Dim result As cDatasetCompatilibity.eCompatibilityTypes = cDatasetCompatilibity.eCompatibilityTypes.Errors

            ' Early bail-out
            If Not Me.IsConnected(Index) Then Return result

            Dim ds As ISpatialDataSet = Me.Dataset(Me.Index)
            Dim cv As ISpatialDataConverter = Me.Converter(Me.Index)
            Dim bm As cEcospaceBasemap = Me.m_core.EcospaceBasemap
            Dim layer As cEcospaceLayer = bm.Layer(Me.VarName, Me.Index)
            Dim strName As String = layer.Name
            Dim ldtData As New List(Of DateTime)
            Dim iTSMin As Integer = Integer.MaxValue
            Dim iTSMax As Integer = 1
            Dim rs As ISpatialRaster = Nothing

            ' Determine time steps with overlap
            For i As Integer = iFirstTimeStep To CInt(Me.m_core.EcospaceModelParameters.NumberOfTimeStepsPerYear)
                Dim dt As Date = Me.m_core.EcospaceTimestepToAbsoluteTime(i)
                If ds.HasDataAtT(dt) Then
                    iTSMin = Math.Min(iTSMin, i)
                    iTSMax = Math.Max(iTSMax, i)
                    ldtData.Add(dt)
                End If
            Next

            ' Determine compatibility
            Dim comp As New cDatasetCompatilibity(Me.m_core, ds, iTSMin, iTSMax - iTSMin)
            result = comp.Compatibility

            Select Case result
                Case cDatasetCompatilibity.eCompatibilityTypes.Errors, _
                     cDatasetCompatilibity.eCompatibilityTypes.NoTemporal, _
                     cDatasetCompatilibity.eCompatibilityTypes.NoSpatial
                    Return result
            End Select

            Dim rst As ISpatialRaster = Nothing
            Dim depth As cEcospaceLayerDepth = bm.LayerDepth
            Dim dCellSize As Double = bm.CellSize
            Dim dMean As Double = 0.0
            Dim dMapTotValue As Double = 0.0
            Dim lNumValCells As Long = 0
            Dim iNumWaterCells As Long = 0
            Dim iInRow As Integer = bm.InRow
            Dim iInCol As Integer = bm.InCol
            Dim msg As cProgressMessage = Nothing

            ' Stop any indexing
            Me.m_core.SpatialDataConnectionManager.DatasetManager.IndexDataset(Nothing)

            Try
                Me.m_core.Messages.SendMessage(New cProgressMessage(eProgressState.Start, 1.0!, 0.0!, "", eMessageType.Progress))
                For i As Integer = 0 To ldtData.Count - 1

                    Dim dt As DateTime = ldtData(i)
                    msg = New cProgressMessage(eProgressState.Running, 1.0!, CSng((i + 1) / ldtData.Count), _
                                               String.Format(My.Resources.CoreMessages.STATUS_SPATIALTERMPORAL_CALCULATING, strName, ds.DisplayName, cv.DisplayName), _
                                               eMessageType.Progress, eDataTypes.EcospaceSpatialDataConnection)
                    Me.m_core.Messages.SendMessage(msg)

                    ' Do the spatial magics
                    If (ds.LockDataAtT(dt, dCellSize, bm.PosTopLeft, bm.PosBottomRight)) Then
                        rst = ds.GetRaster(cv, strName)

                        For iRow As Integer = 1 To iInRow
                            For iCol As Integer = 1 To iInCol
                                If depth.IsWaterCell(iRow, iCol) Then
                                    Dim dval As Double = rst.Cell(iRow, iCol)
                                    If (dval <> cCore.NULL_VALUE And dval <> rst.NoData) Then
                                        iNumWaterCells += 1
                                        dMapTotValue += dval
                                    End If
                                End If
                            Next
                        Next

                        ds.Unlock()
                    End If

                Next
            Catch ex As Exception

            End Try
            Me.m_core.Messages.SendMessage(New cProgressMessage(eProgressState.Finished, 1.0!, 1.0!, "", eMessageType.Progress))

            If dMapTotValue = 0 Then dMapTotValue = 1
            Me.DataScale(Me.Index) = CSng(iNumWaterCells / dMapTotValue)

            Return result

        End Function

#End Region ' Public access

#Region " Overrides "

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="cSpatialDataAdapter.Initialize"/>.
        ''' -------------------------------------------------------------------
        Friend Overrides Sub Initialize()

            MyBase.Initialize()

            Dim iNumItems As Integer = Math.Max(0, Me.m_core.GetCoreCounter(Me.m_coreCounter))

            ReDim Me.m_scales(iNumItems)
            ReDim Me.m_scaleType(iNumItems)

            For i As Integer = 0 To iNumItems - 1
                Me.m_scales(i) = 1.0!
                Me.m_scaleType(i) = eScaleType.Relative
            Next

        End Sub

       ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="cSpatialDataAdapter.SetCell"/>.
        ''' <remarks>Overridden to scale values prior to being set in the 
        ''' Ecospace data structures.</remarks>
        ''' -------------------------------------------------------------------
        Protected Overrides Function SetCell(ByVal layer As cEcospaceLayer, _
                                             ByVal iRow As Integer, _
                                             ByVal iCol As Integer, _
                                             ByVal sValueAtT As Single) As Boolean

            If (Me.m_scaleType(layer.Index) = eScaleType.Relative) Then
                sValueAtT *= Me.DataScale(layer.Index)
            End If
            Return MyBase.SetCell(layer, iRow, iCol, sValueAtT)

        End Function

#End Region ' Overrides

    End Class

End Namespace
