' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.Drawing

Namespace Common

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Interface for directly importing and exporting spatial data into Ecospace
    ''' without the intervention of fancy spatial engines.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Interface IEcospaceImportExport

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set a grid value.
        ''' </summary>
        ''' <param name="iRow">One-based row index to access a value for.</param>
        ''' <param name="iCol">One-based column index to access a value for.</param>
        ''' <param name="strField">Optional field to access a value for.</param>
        ''' -------------------------------------------------------------------
        Property Value(iRow As Integer, iCol As Integer, Optional strField As String = "") As Object

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns data in the form of a <see cref="ISpatialRaster"/>.
        ''' </summary>
        ''' <param name="strField">Optional field name for filtering data if 
        ''' imported data is multi-dimensional.</param>
        ''' <returns>A raster.</returns>
        ''' -------------------------------------------------------------------
        Function ToRaster(Optional strField As String = "") As ISpatialRaster

        ReadOnly Property CellSize As Double
        ReadOnly Property InCol As Integer
        ReadOnly Property InRow As Integer
        ReadOnly Property NoDataValue As Double
        ReadOnly Property PosTopLeft As PointF
        ReadOnly Property ProjectionString As String

    End Interface

End Namespace
