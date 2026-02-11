' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.Common

''' ---------------------------------------------------------------------------
''' <summary>
''' Foundation raster class for <see cref="IEcospaceImportExport">importing and
''' exporting</see> external data without a true spatial engine.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cEcospaceImportExportRaster
    Implements ISpatialRaster

#Region " Private vars "

    Private m_parent As IEcospaceImportExport = Nothing
    Private m_strField As String = ""

    ' -- statistics --

    Private m_bStatsCalculated As Boolean = False
    Private m_lNumValueCells As Long = 0
    Private m_dMax As Double = 0
    Private m_dMin As Double = 0
    Private m_dMean As Double = 0
    Private m_dStdDev As Double = 0

#End Region ' Private vars

    Public Sub New(parent As IEcospaceImportExport,
                   Optional strField As String = "")
        Me.m_parent = parent
        Me.m_strField = strField
    End Sub

#Region " Interface implementations "

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="ISpatialRaster.Cell"/>
    ''' -----------------------------------------------------------------------
    Public Function Cell(iRow As Integer, iCol As Integer, Optional dNoDataValue As Double = -9999.0) As Double _
        Implements EwECore.Common.ISpatialRaster.Cell
        Return Convert.ToDouble(Me.m_parent.Value(iRow, iCol, Me.m_strField))
    End Function

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="ISpatialRaster.CellSize"/>
    ''' -----------------------------------------------------------------------
    Public Function CellSize() As Double _
         Implements EwECore.Common.ISpatialRaster.CellSize
        Return Me.m_parent.CellSize
    End Function

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="ISpatialRaster.Max"/>
    ''' -----------------------------------------------------------------------
    Public Function Max() As Double _
        Implements EwECore.Common.ISpatialRaster.Max
        Me.CalculateStats()
        Return Me.m_dMax
    End Function

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="ISpatialRaster.Mean"/>
    ''' -----------------------------------------------------------------------
    Public Function Mean() As Double _
        Implements EwECore.Common.ISpatialRaster.Mean
        Me.CalculateStats()
        Return Me.m_dMean
    End Function

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="ISpatialRaster.Min"/>
    ''' -----------------------------------------------------------------------
    Public Function Min() As Double _
            Implements EwECore.Common.ISpatialRaster.Min
        Me.CalculateStats()
        Return Me.m_dMin
    End Function

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="ISpatialRaster.NoData"/>
    ''' -----------------------------------------------------------------------
    Public Function NoData() As Single _
        Implements EwECore.Common.ISpatialRaster.NoData
        Return cCore.NULL_VALUE
    End Function

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="ISpatialRaster.NumCols"/>
    ''' -----------------------------------------------------------------------
    Public Function NumCols() As Integer _
        Implements EwECore.Common.ISpatialRaster.NumCols
        Return Me.m_parent.InCol
    End Function

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="ISpatialRaster.NumRows"/>
    ''' -----------------------------------------------------------------------
    Public Function NumRows() As Integer _
        Implements EwECore.Common.ISpatialRaster.NumRows
        Return Me.m_parent.InRow
    End Function

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="ISpatialRaster.NumValueCells"/>
    ''' -----------------------------------------------------------------------
    Public Function NumValueCells() As Long _
        Implements EwECore.Common.ISpatialRaster.NumValueCells
        Me.CalculateStats()
        Return Me.m_lNumValueCells
    End Function

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="ISpatialRaster.Save"/>
    ''' -----------------------------------------------------------------------
    Public Function Save(strFile As String) As Boolean _
        Implements EwECore.Common.ISpatialRaster.Save
        ' ToDo: execute this via the parent
        Return False
    End Function

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="ISpatialRaster.StandardDeviation"/>
    ''' -----------------------------------------------------------------------
    Public Function StandardDeviation() As Double _
        Implements EwECore.Common.ISpatialRaster.StandardDeviation
        Return Me.m_dStdDev
    End Function

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="ISpatialRaster.TopLeft"/>
    ''' -----------------------------------------------------------------------
    Public Function TopLeft() As System.Drawing.PointF _
        Implements EwECore.Common.ISpatialRaster.TopLeft
        Return Me.m_parent.PosTopLeft
    End Function

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="ISpatialRaster.Dispose"/>
    ''' -----------------------------------------------------------------------
    Public Sub Dispose() Implements IDisposable.Dispose
        GC.SuppressFinalize(Me)
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="ISpatialRaster.IsValid"/>
    ''' -----------------------------------------------------------------------
    Public Function IsValid() As Boolean _
            Implements EwECore.Common.ISpatialRaster.IsValid
        Return True
    End Function

#End Region ' Interface implementations

#Region " Internals "

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Calculate the statistics of the data wrapped by this class.
    ''' </summary>
    ''' -------------------------------------------------------------------
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

#End Region ' Internals

End Class ' cEcospaceImportExportRaster