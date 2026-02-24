' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.Drawing
Imports System.IO
Imports EwECore
Imports EwECore.Common
Imports EwEUtils.Logging
Imports EwEUtils.Utilities
Imports Microsoft.Extensions.Logging

Public Class cSpatialRaster
    Implements ISpatialRaster

#Region " Private vars "

    Private m_bm As cEcospaceBasemap = Nothing
    Private m_data As Single(,) = Nothing
    Private m_bStatsCalculated As Boolean = False
    Private m_lNumValueCells As Long = 0
    Private m_dMax As Double = 0
    Private m_dMin As Double = 0
    Private m_dMean As Double = 0
    Private m_dStdDev As Double = 0.0#
    Private ReadOnly m_logger As ILogger = LoggingContext.CreateLogger(Of cSpatialRaster)()

#End Region ' Private vars

#Region " Construction / destruction "

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Constructor, wraps a raster.
    ''' </summary>
    ''' -------------------------------------------------------------------
    Public Sub New(ByVal bm As cEcospaceBasemap, data As Single(,))
        Me.m_bm = bm
        Me.m_data = data
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        GC.SuppressFinalize(Me)
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
    Public Function Cell(ByVal iRow As Integer,
                         ByVal iCol As Integer,
                         Optional ByVal dNoDataValue As Double = -9999) As Double _
        Implements ISpatialRaster.Cell
        Return Me.m_data(iRow, iCol)
    End Function

    ''' -------------------------------------------------------------------
    ''' <inheritdocs cref="ISpatialRaster.Max"/>
    ''' -------------------------------------------------------------------
    Public Function Max() As Double Implements ISpatialRaster.Max
        Me.CalculateStats()
        Return Me.m_dMax
    End Function

    ''' -------------------------------------------------------------------
    ''' <inheritdocs cref="ISpatialRaster.Mean"/>
    ''' -------------------------------------------------------------------
    Public Function Mean() As Double Implements ISpatialRaster.Mean
        Me.CalculateStats()
        Return Me.m_dMean
    End Function

    ''' -------------------------------------------------------------------
    ''' <inheritdocs cref="ISpatialRaster.Min"/>
    ''' -------------------------------------------------------------------
    Public Function Min() As Double Implements ISpatialRaster.Min
        Me.CalculateStats()
        Return Me.m_dMin
    End Function

    ''' -------------------------------------------------------------------
    ''' <inheritdocs cref="ISpatialRaster.StandardDeviation"/>
    ''' -------------------------------------------------------------------
    Public Function StandardDeviation() As Double _
        Implements EwECore.Common.ISpatialRaster.StandardDeviation
        Me.CalculateStats()
        Return Me.m_dStdDev
    End Function

    ''' -------------------------------------------------------------------
    ''' <inheritdocs cref="ISpatialRaster.NoData"/>
    ''' -------------------------------------------------------------------
    Public Function NoData() As Single Implements ISpatialRaster.NoData
        Return cCore.NULL_VALUE
    End Function

    ''' -------------------------------------------------------------------
    ''' <inheritdocs cref="ISpatialRaster.NumValueCells"/>
    ''' -------------------------------------------------------------------
    Public Function NumValueCells() As Long Implements ISpatialRaster.NumValueCells
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
        Return My.Resources.RASTER_NAME
    End Function

    ''' -------------------------------------------------------------------
    ''' <inheritdocs cref="ISpatialRaster.CellSize"/>
    ''' -------------------------------------------------------------------
    Public Function CellSize() As Double _
        Implements ISpatialRaster.CellSize
        Return Me.m_bm.CellSize
    End Function

    ''' -------------------------------------------------------------------
    ''' <inheritdocs cref="ISpatialRaster.NumCols"/>
    ''' -------------------------------------------------------------------
    Public Function NumCols() As Integer _
        Implements ISpatialRaster.NumCols
        Return Me.m_bm.InCol
    End Function

    ''' -------------------------------------------------------------------
    ''' <inheritdocs cref="ISpatialRaster.NumRows"/>
    ''' -------------------------------------------------------------------
    Public Function NumRows() As Integer _
        Implements ISpatialRaster.NumRows
        Return Me.m_bm.InRow
    End Function

    ''' -------------------------------------------------------------------
    ''' <inheritdocs cref="ISpatialRaster.TopLeft"/>
    ''' -------------------------------------------------------------------
    Public Function TopLeft() As PointF _
        Implements ISpatialRaster.TopLeft
        Return Me.m_bm.PosTopLeft
    End Function

    Public Function Load(strFile As String) As Boolean
        Return False
    End Function

    ''' -------------------------------------------------------------------
    ''' <inheritdocs cref="ISpatialRaster.Save"/>
    ''' -------------------------------------------------------------------
    Public Function Save(strFile As String) As Boolean _
        Implements ISpatialRaster.Save

        If Not cFileUtils.IsDirectoryAvailable(Path.GetDirectoryName(strFile), True) Then
            Return False
        End If
        Return Me.SaveAsc(strFile)

    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Returns a string representing the projection of this raster.
    ''' </summary>
    ''' <returns>The ESRI projection string that represents the projection of this raster.</returns>
    ''' -------------------------------------------------------------------
    Public Function ProjectionString() As String
        Return Me.m_bm.ProjectionString
    End Function

#End Region ' Public access

#Region " Internals "

    Private Sub CalculateStats()

        If (Me.m_bStatsCalculated) Then Return

        Dim dVal As Double = 0
        Dim dNoData As Double = cCore.NULL_VALUE
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
            writer.WriteLine("xllcorner     " & Me.m_bm.PosTopLeft.X)
            writer.WriteLine("yllcorner     " & Me.m_bm.PosBottomRight.Y)
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
            m_logger.LogError(ex, "cSpatialRaster.SaveAsc(" & strFile & ")")
            Return False
        End Try
        Return True

    End Function

#End Region ' Internals

    Public Function IsValid() As Boolean Implements EwECore.Common.ISpatialRaster.IsValid
        Return True
    End Function

End Class
