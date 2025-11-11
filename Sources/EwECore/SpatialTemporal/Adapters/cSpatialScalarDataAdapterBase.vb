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
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports EwEUtils.Core
Imports EwEUtils.SpatialData
Imports EwEUtils.Logging
Imports Microsoft.Extensions.Logging
Imports Debug = System.Diagnostics.Debug

#End Region ' Imports

Namespace SpatialData

    ''' <summary>
    ''' Derived spatial data adapter to insert scaled external spatial/temporal map data into
    ''' the Ecospace data structures at any given moment. This adapter maintains a scale
    ''' for every map layer attached to the adapter, and will translate map values
    ''' to relative values when <see cref="cSpatialScalarDataAdapter.eScaleType"/> is set to
    ''' <see cref="cSpatialScalarDataAdapter.eScaleType.Relative">relative</see>.
    ''' </summary>
    Public MustInherit Class cSpatialScalarDataAdapterBase
        Inherits cSpatialDataAdapter

        Friend iScaleLayerIndex As Integer

#Region " Constructor "

        Public Sub New(core As cCore, varName As eVarNameFlags, cc As eCoreCounterTypes)
            MyBase.New(core, varName, cc)
        End Sub

#End Region ' Constructor

#Region " Public access "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' States if this adapter can scale to a base value.
        ''' </summary>
        ''' <seealso cref="CalculateScalar(Double, Double)"/>
        ''' -------------------------------------------------------------------
        Public Overridable Function CanCalculateScalar() As Boolean
            Return True
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Calculate the average of a dataset for a year, starting at a given
        ''' Ecospace time step.
        ''' </summary>
        ''' <param name="iFirstTimeStep">The time step that the calculation
        ''' is requested for.</param>
        ''' <param name="dScale">The calculated scale.</param>
        ''' <returns>A <see cref="cDatasetCompatilibity.Compatibility"/> result
        ''' indicating the outcome of the calculation.</returns>
        ''' -------------------------------------------------------------------
        Public Function CalculateScaleFromEcopathTimePeriod(iLayerIndex As Integer,
                                                            conn As cSpatialDataConnection,
                                                            iFirstTimeStep As Integer,
                                                            ByRef dScale As Double) As cDatasetCompatilibity.eCompatibilityTypes

            Dim manConn As cSpatialDataConnectionManager = Me.m_core.SpatialDataConnectionManager
            Dim manSets As cSpatialDataSetManager = manConn.DatasetManager
            Dim result As cDatasetCompatilibity.eCompatibilityTypes = cDatasetCompatilibity.eCompatibilityTypes.Errors

            ' Early bail-out
            If Not Me.IsConnected(iLayerIndex) Then Return result
            If Not Me.CanCalculateScalar() Then Return result

            Me.iScaleLayerIndex = iLayerIndex

            ' Suspend indexing
            manSets.SuspendIndexing()

            Dim ds As ISpatialDataSet = conn.Dataset
            Dim cv As ISpatialDataConverter = conn.Converter
            Dim bm As cEcospaceBasemap = Me.m_core.EcospaceBasemap
            Dim iInRow As Integer = bm.InRow
            Dim iInCol As Integer = bm.InCol
            Dim dCellSize As Double = bm.CellSize
            Dim layer As cEcospaceLayer = bm.Layer(Me.VarName, iLayerIndex - 1)
            Dim depth As cEcospaceLayerDepth = bm.LayerDepth
            Dim excl As cEcospaceLayerExclusion = bm.LayerExclusion
            Dim strLayerName As String = layer.Name
            Dim ldtData As New List(Of DateTime)
            Dim iTSMin As Integer = Integer.MaxValue
            Dim iTSMax As Integer = 1
            Dim rs As ISpatialRaster = Nothing
            Dim dMapTotValue As Double = 0.0
            Dim iNumWaterCells As Integer = 0
            Dim msg As cProgressMessage = Nothing

            ' Determine time steps with overlap
            For i As Integer = iFirstTimeStep To iFirstTimeStep + CInt(Me.m_core.EcospaceModelParameters.NumberOfTimeStepsPerYear) - 1
                ' JS Oct23: this also should support datasets that have been extended back in time
                Dim dtVirt As Date = conn.ToDataTime(Me.m_core, Me.m_core.EcospaceTimestepToAbsoluteTime(i))
                If conn.Dataset.HasDataAtT(dtVirt) Then
                    iTSMin = Math.Min(iTSMin, i)
                    iTSMax = Math.Max(iTSMax, i)
                    ldtData.Add(dtVirt)
                End If
            Next

            Me.m_core.Messages.SendMessage(New cProgressMessage(eProgressState.Start, 1.0!, 0.0!, "", eMessageType.Progress))
            Try
                For i As Integer = 0 To ldtData.Count - 1

                    Dim dt As DateTime = ldtData(i)
                    msg = New cProgressMessage(eProgressState.Running, 1.0!, CSng((i + 1) / ldtData.Count),
                                               String.Format(My.Resources.CoreMessages.STATUS_SPATIALTERMPORAL_CALCULATING, strLayerName, ds.CustomName, ""),
                                               eMessageType.Progress, eDataTypes.EcospaceSpatialDataConnection)
                    Me.m_core.Messages.SendMessage(msg)

                    ' Do the spatial magics
                    If (ds.LockDataAtT(dt, dCellSize, bm.PosTopLeft, bm.PosBottomRight, bm.ProjectionString)) Then
                        rs = ds.GetRaster(cv, strLayerName)
                        If rs IsNot Nothing Then
                            For iRow As Integer = 1 To iInRow
                                For iCol As Integer = 1 To iInCol
                                    'If depth.IsWaterCell(iRow, iCol) Then
                                    If depth.IsWaterCell(iRow, iCol) And (Not excl.IsExcludedCell(iRow, iCol)) Then
                                        Dim dval As Double = rs.Cell(iRow, iCol)
                                        If (dval <> cCore.NULL_VALUE And dval <> rs.NoData) Then
                                            iNumWaterCells += 1
                                            dMapTotValue += dval
                                        End If
                                    End If
                                Next
                            Next
                        Else
                            'Log message raster not read
                        End If

                        ds.Unlock()
                    End If

                Next
            Catch ex As Exception
                ' Log error
                cLog.Write(ex, eVerboseLevel.Detailed, "Failed to calculate scaling factor for dataset " & ds.CustomName)
            End Try
            Me.m_core.Messages.SendMessage(New cProgressMessage(eProgressState.Finished, 1.0!, 1.0!, "", eMessageType.Progress))

            If dMapTotValue = 0 Then dMapTotValue = 1
            If iNumWaterCells = 0 Then iNumWaterCells = 1

            'Get the scalar value from a function
            'So different adapters can use a different scalar calculation
            dScale = Me.CalculateScalar(dMapTotValue, iNumWaterCells)

            manSets.ResumeIndexing()

            ' Report for the calculation period
            Dim man As cSpatialDataSetManager = Me.m_core.SpatialDataConnectionManager.DatasetManager
            Dim comp As cDatasetCompatilibity = man.Compatibility(ds)
            comp.Refresh()
            Return comp.Compatibility

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Calculates a scale value based on the sum and number of cells over the time period.
        ''' </summary>
        ''' <param name="SumOverPeriod">Sum of values over the time period.</param>
        ''' <param name="nMapCells">Total number of cells included in the sum.</param>
        ''' <returns> (1 / mean)</returns>
        ''' <remarks>Default scalar for relative adapters. Return the mean scalar as a multiplier.</remarks>
        ''' <seealso cref="CanCalculateScalar()"/>
        ''' -------------------------------------------------------------------
        Public Overridable Function CalculateScalar(SumOverPeriod As Double, nMapCells As Double) As Double
            Try
                'This is the default value for a relative scalar that is used as multiplier 
                'RelPP and Relative biomass can use this 
                'Biomass forcing overrides this to return the average
                Return nMapCells / SumOverPeriod
            Catch ex As Exception
                cLog.Write(ex, "Failed to calculate scale value.")
            End Try
            'Ohh... my...
            Return 1.0
        End Function

#End Region ' Public access

    End Class

End Namespace
