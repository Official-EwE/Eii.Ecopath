#Region " Imports "

Option Strict On

#End Region ' Imports

Namespace SpatialData

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Wrapper to present raster data to Ecospace.
    ''' </summary>
    ''' -------------------------------------------------------------------
    Public Interface ISpatialRaster

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get a cell value for a given Ecospace row and column.
        ''' </summary>
        ''' <param name="iRow">One-based Ecospace row index</param>
        ''' <param name="iCol">One-based Ecospace column index</param>
        ''' <param name="dNoDataValue">No data value to use if either row or 
        ''' column are invalid, or if the cell does not hold any data.</param>
        ''' <returns>A value, or <paramref name="dNoDataValue"/> if either row or 
        ''' column are invalid, or if the cell does not hold any data.</returns>
        ''' -------------------------------------------------------------------
        Function Cell(ByVal iRow As Integer, ByVal iCol As Integer, _
                         Optional ByVal dNoDataValue As Double = -9999) As Double

    End Interface

End Namespace
