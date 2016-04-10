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
'    UBC Fisheries Centre, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports System.Drawing
Imports System.IO
Imports DotSpatial.Data
Imports DotSpatial.Topology
Imports EwECore
Imports EwEUtils.SpatialData
Imports EwEUtils.Utilities
Imports EwEUtils.Core
Imports DotSpatial.Projections

#End Region ' Imports

Namespace SpatialData

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Wrapper to pass a <see cref="IValueGrid"/> out of this assembly 
    ''' without the need for the DotSpatial assemblies.
    ''' </summary>
    ''' -------------------------------------------------------------------
    Public Class cSpatialRaster
        Implements ISpatialRaster

#Region " Private vars "

        ''' <summary>The raster to wrap.</summary>
        Private m_rs As IRaster = Nothing

        Private m_bStatsCalculated As Boolean = False
        Private m_lNumValueCells As Long = 0
        Private m_dMax As Double = 0
        Private m_dMin As Double = 0
        Private m_dMean As Double = 0
        Private m_dStdDev As Double = 0.0#

#End Region ' Private vars

#Region " Construction / destruction "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor, wraps a raster.
        ''' </summary>
        ''' <param name="raster">Raster to wrap.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal raster As IRaster)

            Me.m_rs = raster
 
        End Sub

        Public Sub New(ByVal strFile As String)
            Me.Load(strFile)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="IDisposable.Dispose"/>
        ''' -------------------------------------------------------------------
        Public Sub Dispose() _
            Implements ISpatialRaster.Dispose
            If (Me.m_rs IsNot Nothing) Then
                Me.m_rs = Nothing
            End If
        End Sub

#End Region ' Construction / destruction

#Region " Public access "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns a cell value for a given Ecospace row and column.
        ''' </summary>
        ''' <param name="iRow">One-based row index in the Ecospace grid.</param>
        ''' <param name="iCol">One-based column index in the Ecospace grid.</param>
        ''' <param name="dNoDataValue">No data value to use if either row or 
        ''' column are invalid, or if the cell does not hold any data.</param>
        ''' <returns>A value, or <paramref name="dNoDataValue"/> if either row or 
        ''' column are invalid, or if the cell does not hold any data.</returns>
        ''' -------------------------------------------------------------------
        Public Function Cell(ByVal iRow As Integer, _
                             ByVal iCol As Integer, _
                             Optional ByVal dNoDataValue As Double = -9999) As Double _
            Implements ISpatialRaster.Cell

            iRow -= 1
            iCol -= 1

            ' Perform range check
            If (iRow < 0 Or iRow > Me.m_rs.EndRow) Then Return dNoDataValue
            If (iCol < 0 Or iCol > Me.m_rs.EndColumn) Then Return dNoDataValue

            Try
                Dim dValue As Double = Me.m_rs.Value(iRow, iCol)
                If (dValue = Me.m_rs.NoDataValue) Or (dValue = dNoDataValue) Then Return dNoDataValue
                Return dValue
            Catch ex As Exception
                Return dNoDataValue
            End Try

        End Function

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="ISpatialRaster.Max"/>
        ''' -------------------------------------------------------------------
        Public Function Max() As Double Implements ISpatialRaster.Max
            If (Not Me.IsValid()) Then Return cCore.NULL_VALUE
            Me.CalculateStats()
            Return Me.m_dMax
        End Function

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="ISpatialRaster.Mean"/>
        ''' -------------------------------------------------------------------
        Public Function Mean() As Double Implements ISpatialRaster.Mean
            If (Not Me.IsValid()) Then Return cCore.NULL_VALUE
            Me.CalculateStats()
            Return Me.m_dMean
        End Function

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="ISpatialRaster.Min"/>
        ''' -------------------------------------------------------------------
        Public Function Min() As Double Implements ISpatialRaster.Min
            If (Not Me.IsValid()) Then Return cCore.NULL_VALUE
            Me.CalculateStats()
            Return Me.m_dMin
        End Function

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="ISpatialRaster.StandardDeviation"/>
        ''' -------------------------------------------------------------------
        Public Function StandardDeviation() As Double _
            Implements EwEUtils.SpatialData.ISpatialRaster.StandardDeviation
            If (Not Me.IsValid()) Then Return cCore.NULL_VALUE
            Me.CalculateStats()
            Return Me.m_dStdDev
        End Function

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="ISpatialRaster.NoData"/>
        ''' -------------------------------------------------------------------
        Public Function NoData() As Single Implements ISpatialRaster.NoData
            If (Not Me.IsValid()) Then Return cCore.NULL_VALUE
            Return CSng(Me.m_rs.NoDataValue)
        End Function

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="ISpatialRaster.NumValueCells"/>
        ''' -------------------------------------------------------------------
        Public Function NumValueCells() As Long Implements ISpatialRaster.NumValueCells
            If (Not Me.IsValid()) Then Return cCore.NULL_VALUE
            Me.CalculateStats()
            Return Me.m_lNumValueCells
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns a string representation of this class.
        ''' </summary>
        ''' <returns>A string representation of this class.</returns>
        ''' -------------------------------------------------------------------
        Public Overrides Function ToString() As String
            Return cDotSpatialUtils.FormatRaster(Me)
        End Function

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="ISpatialRaster.CellSize"/>
        ''' -------------------------------------------------------------------
        Public Function CellSize() As Double _
            Implements ISpatialRaster.CellSize
            If (Not Me.IsValid()) Then Return cCore.NULL_VALUE
            Return Me.m_rs.CellWidth
        End Function

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="ISpatialRaster.NumCols"/>
        ''' -------------------------------------------------------------------
        Public Function NumCols() As Integer _
            Implements ISpatialRaster.NumCols
            If (Not Me.IsValid()) Then Return cCore.NULL_VALUE
            Return Me.m_rs.NumColumns
        End Function

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="ISpatialRaster.NumRows"/>
        ''' -------------------------------------------------------------------
        Public Function NumRows() As Integer _
            Implements ISpatialRaster.NumRows
            If (Not Me.IsValid()) Then Return cCore.NULL_VALUE
            Return Me.m_rs.NumRows
        End Function

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="ISpatialRaster.TopLeft"/>
        ''' -------------------------------------------------------------------
        Public Function TopLeft() As PointF _
            Implements ISpatialRaster.TopLeft
            If (Not Me.IsValid()) Then Return New PointF(cCore.NULL_VALUE, cCore.NULL_VALUE)
            Dim ext As Extent = Me.m_rs.Extent
            Return New PointF(CSng(ext.MinX), CSng(ext.MaxY))
        End Function

        Public Function Load(strFile As String) As Boolean
            Dim ds As IDataSet = cDotSpatialUtils.OpenFile(strFile)
            If (ds Is Nothing) Then Return False
            If (Not TypeOf ds Is IRaster) Then Return False
            Dim rs As IRaster = DirectCast(ds, IRaster)
            If (rs Is Nothing) Then Return False
            Me.m_rs = rs
            Return True
        End Function

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="ISpatialRaster.Save"/>
        ''' -------------------------------------------------------------------
        Public Function Save(strFile As String) As Boolean _
            Implements ISpatialRaster.Save

            If (Not Me.IsValid()) Then Return False

            If Not cFileUtils.IsDirectoryAvailable(Path.GetDirectoryName(strFile), True) Then
                Return False
            End If

            Select Case Path.GetExtension(strFile).ToLower
                Case ".asc"
                    Return Me.SaveAsc(strFile)
                Case Else
                    Return Me.SaveDotSpatial(strFile)
            End Select
            Return False

        End Function

        Public Function IsValid() As Boolean _
            Implements EwEUtils.SpatialData.ISpatialRaster.IsValid
            If (Me.m_rs Is Nothing) Then Return False
            If (Not cNumberUtils.Approximates(Me.m_rs.CellHeight, Me.m_rs.CellWidth, Me.m_rs.CellHeight / 100)) Then Return False
            Return True
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns a string representing the projection of this raster.
        ''' </summary>
        ''' <returns>The ESRI projection string that represents the projection of this raster.</returns>
        ''' -------------------------------------------------------------------
        Public Function ProjectionString() As String
            Dim pi As ProjectionInfo = Nothing
            If (Me.m_rs IsNot Nothing) Then pi = Me.m_rs.Projection
            Return cDotSpatialUtils.ToProjectionString(pi)
        End Function

#End Region ' Public access

#Region " Limited access "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns the <see cref="IExtent"/> of the DotSpatial raster wrapped
        ''' by this class.
        ''' </summary>
        ''' <returns>The <see cref="IExtent"/> of the DotSpatial raster wrapped
        ''' by this class.</returns>
        ''' -------------------------------------------------------------------
        Friend Function Ext() As IExtent
            Return Me.m_rs.Extent
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns the <see cref="IRaster"/> wrapped by this class.
        ''' </summary>
        ''' <returns>The <see cref="IRaster"/> wrapped by this class.</returns>
        ''' -------------------------------------------------------------------
        Friend Function Raster() As IRaster
            Return Me.m_rs
        End Function

#End Region ' Limited access

#Region " Internals "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Calculate the statistics of the <see cref="IRaster"/> wrapped by this class.
        ''' </summary>
        ''' <remarks>
        ''' <para>Implemented this method to make sure that NoData cells and cCore.NULL_VALE
        ''' cells are ignored in the calculations. The DotSpatial statistics methods
        ''' do not seem to do this; for instance, <see cref="IRaster.Minimum"/>
        ''' will return <see cref="IRaster.NoDataValue"/> if this is in fact the
        ''' minimum value in the raster. This is not desirable.</para>
        ''' <para>However, this method does not take water and land cells into account.
        ''' Since a layer does not know its intended purpose, calculations of stats
        ''' should be performed by adapters, not by the individual rasters.</para>
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Private Sub CalculateStats()

            If (Me.m_bStatsCalculated) Then Return

            Dim dVal As Double = 0
            Dim dNoData As Double = Me.m_rs.NoDataValue
            Dim dTot As Double = 0
            Dim dMax As Double = Double.MinValue
            Dim dMin As Double = Double.MaxValue
            Dim dStdDev As Double = cCore.NULL_VALUE
            Dim iNumCols As Integer = Me.NumCols
            Dim iNumRows As Integer = Me.NumRows

            ' ToDo: take cell area for a row into account!

            Me.m_lNumValueCells = 0

            Try

                For iRow As Integer = 1 To iNumRows
                    For iCol As Integer = 1 To iNumCols
                        dVal = Me.Cell(iRow, iCol)
                        If (dVal <> dNoData) And (dVal <> cCore.NULL_VALUE) Then
                            Me.m_lNumValueCells += 1
                            dMax = Math.Max(dMax, dVal)
                            dMin = Math.Min(dMin, dVal)
                            dTot += dVal
                        End If
                    Next
                Next

                If (Me.m_lNumValueCells > 0) Then
                    Me.m_dMax = dMax
                    Me.m_dMin = dMin
                    Me.m_dMean = dTot / Me.m_lNumValueCells

                    ' Standard deviation
                    dTot = 0

                    For iRow As Integer = 1 To iNumRows
                        For iCol As Integer = 1 To iNumCols
                            dVal = Me.Cell(iRow, iCol)
                            If (dVal <> dNoData) And (dVal <> cCore.NULL_VALUE) Then
                                dTot += (dVal - Me.m_dMean) * (dVal - Me.m_dMean)
                            End If
                        Next
                    Next
                    Me.m_dStdDev = Math.Sqrt(dTot / Me.m_lNumValueCells)
                Else
                    Me.m_dMin = cCore.NULL_VALUE
                    Me.m_dMax = cCore.NULL_VALUE
                    Me.m_dMean = cCore.NULL_VALUE
                    Me.m_dStdDev = cCore.NULL_VALUE
                End If

            Catch ex As Exception
                ' Overflow?!
            End Try

            Me.m_bStatsCalculated = True

        End Sub

        Private Function SaveAsc(strFile As String) As Boolean

            Try
                Dim writer As New StreamWriter(strFile)
                writer.WriteLine("ncols         " & Me.NumCols)
                writer.WriteLine("nrows         " & Me.NumRows)
                writer.WriteLine("xllcorner     " & Me.m_rs.Extent.MinX)
                writer.WriteLine("yllcorner     " & Me.m_rs.Extent.MinY)
                writer.WriteLine("cellsize      " & Me.CellSize)
                writer.WriteLine("NODATA_value  " & Me.NoData)

                For ir As Integer = 1 To Me.NumRows
                    For ic As Integer = 1 To Me.NumCols
                        If ic > 1 Then writer.Write(" ")
                        writer.Write(cStringUtils.FormatNumber(Me.Cell(ir, ic)))
                    Next ic
                    writer.WriteLine("")
                Next ir
                writer.Flush()
                writer.Close()
                writer.Dispose()

            Catch ex As Exception
                cLog.Write(ex, "cSpatialRaster.SaveAsc(" & strFile & ")")
                Return False
            End Try
            Return True

        End Function

        Private Function SaveDotSpatial(strFile As String) As Boolean
            Try
                Me.m_rs.SaveAs(strFile)
            Catch ex As Exception
                cLog.Write(ex, "cSpatialRaster.SaveDotSpatial(" & strFile & ")")
                Return False
            End Try
            Return True
        End Function

#End Region ' Internals

    End Class

End Namespace
