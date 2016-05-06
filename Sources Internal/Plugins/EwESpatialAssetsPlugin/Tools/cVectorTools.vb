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
'    UBC Institute for the Oceans and Fisheries, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports System.Collections.Generic
Imports System.Drawing
Imports DotSpatial.Data
Imports DotSpatial.Projections
Imports DotSpatial.Topology
Imports EwECore
Imports EwECore.SpatialData
Imports EwEUtils.Utilities

#End Region ' Imports

Public Class cVectorTools

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Delegate to call for translating a rasterized value.
    ''' </summary>
    ''' <param name="drow">The datarow with metadata from a feature that was hit, if any.
    ''' If nothing, a feature was not hit.</param>
    ''' <param name="dNoData">The nodata value of the raster.</param>
    ''' <returns>The converted value.</returns>
    ''' -----------------------------------------------------------------------
    Public Delegate Function TranslateValueDelegate(ByVal drow As DataRow, ByVal dNoData As Double) As Double

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Convert features in a polygon feature set to a raster, populating each
    ''' occurrence data.
    ''' </summary>
    ''' <param name="fs">The polygon feature set to convert.</param>
    ''' <param name="dCellSize">Cell size, in decimal degrees, of the raster to create.</param>
    ''' <param name="dValueNull">No data value</param>
    ''' <param name="ptfBR"></param>
    ''' <param name="ptfTL"></param>
    ''' <param name="dgt"></param>
    ''' <param name="strFileName">The output file name to write the raster to.</param>
    ''' <returns>A raster.</returns>
    ''' <remarks>
    ''' <para>This operation uses 'cookie cutter' polygons for clipping polygons and
    ''' performing area ratio calculations.</para>
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Public Shared Function Rasterize(ByVal fs As IFeatureSet,
                                     ByVal ptfTL As PointF, _
                                     ByVal ptfBR As PointF, _
                                     ByVal dCellSize As Double, _
                                     ByVal dValueNull As Double, _
                                     ByVal strFileName As String,
                                     ByVal dgt As TranslateValueDelegate) As IRaster

        Debug.Assert(dgt IsNot Nothing)

        Dim dValClear As Double
        Dim dValSet As Double

        ' -----
        ' Create and position raster 
        ' -----
        Dim bnds As IRasterBounds = cDotSpatialUtils.EcospaceToBounds(ptfTL, ptfBR, dCellSize, fs.Projection)
        Dim rs As IRaster = Raster.Create(strFileName, "", bnds.NumColumns, bnds.NumRows, 1, GetType(Double), Nothing)
        rs.Bounds = bnds
        rs.NoDataValue = dValueNull
        rs.Projection = fs.Projection

        ' Wipe array
        dValClear = dgt.Invoke(Nothing, 0)
        For iRow As Integer = 0 To bnds.NumRows - 1
            For iCol As Integer = 0 To bnds.NumColumns - 1
                rs.Value(iRow, iCol) = dValClear
            Next
        Next

        'Try
        '    rs.SaveAs("vectorized.tiff")
        'Catch ex As Exception

        'End Try

        Dim dtAttribs As DataTable = fs.DataTable
        For i As Integer = 0 To fs.Features.Count - 1

            Dim f As IFeature = fs.Features(i)
            Dim drow As DataRow = dtAttribs.Rows(i)
            dValSet = dgt.Invoke(drow, rs.NoDataValue)

            Select Case f.FeatureType

                Case FeatureType.Point
                    ' JS17Feb14: tested OK
                    If rs.ContainsFeature(f) Then
                        Dim pt As IPoint = New DotSpatial.Topology.Point(f.Coordinates(0))
                        Dim cellpos As RcIndex = rs.ProjToCell(pt.X, pt.Y)
                        rs.Value(cellpos.Row, cellpos.Column) = dValSet
                    End If

                Case FeatureType.Line
                    Throw New NotImplementedException("Spatial Assets plug-in - cVectorTools cannot convert line features yet")

                Case FeatureType.MultiPoint
                    ' To test
                    Dim mp As IMultiPoint = New MultiPoint(f.Coordinates)
                    For j As Integer = 0 To mp.NumPoints - 1
                        Dim pt As IPoint = mp.Item(j)
                        If rs.ContainsFeature(CType(pt, IFeature)) Then
                            Dim cellpos As RcIndex = rs.ProjToCell(CType(pt.Centroid, Coordinate))
                            rs.Value(cellpos.Row, cellpos.Column) = dValSet
                        End If
                    Next

                Case FeatureType.Polygon

                    ' 18Feb14: Only process cells overlapping with a valid polygon
                    Dim poly As New Polygon(f.Coordinates)
                    Dim ext As New Extent(poly.Envelope)
                    Dim x0 As Integer = 0
                    Dim x1 As Integer = bnds.NumColumns - 1
                    Dim y0 As Integer = 0
                    Dim y1 As Integer = bnds.NumRows

                    If (Not poly.IsValid) Then
                        ' ToDo: notify user 
                    Else
                        If (ext.Intersects(rs.Bounds.Extent)) Then
                            ' Get intersection extent
                            Dim extIntersect As Extent = ext.Intersection(rs.Extent)
                            Dim tl As RcIndex = rs.ProjToCell(extIntersect.MinX, extIntersect.MaxY)
                            Dim br As RcIndex = rs.ProjToCell(extIntersect.MaxX, extIntersect.MinY)

                            If Not tl.IsEmpty And Not br.IsEmpty Then
                                x0 = Math.Max(x0, tl.Column) : x1 = Math.Min(x1, br.Column)
                                y0 = Math.Max(y0, tl.Row) : y1 = Math.Min(y1, br.Row)
                            End If
                        End If

                        Dim n As Integer = 0
                        For iRow As Integer = y0 To y1
                            For iCol As Integer = x0 To x1

                                ' Create cookie cutter for a cell
                                Dim ptCut = rs.CellToProj(iRow, iCol)
                                Dim coords As New List(Of Coordinate)
                                coords.Add(New Coordinate(ptCut.X - rs.CellWidth / 2, ptCut.Y - rs.CellHeight / 2))
                                coords.Add(New Coordinate(ptCut.X + rs.CellWidth / 2, ptCut.Y - rs.CellHeight / 2))
                                coords.Add(New Coordinate(ptCut.X + rs.CellWidth / 2, ptCut.Y + rs.CellHeight / 2))
                                coords.Add(New Coordinate(ptCut.X - rs.CellWidth / 2, ptCut.Y + rs.CellHeight / 2))
                                coords.Add(coords(0)) ' Close polygon

                                Dim polyCut As New Polygon(coords)

                                If polyCut.Overlaps(poly) Then
                                    rs.Value(iRow, iCol) = dValSet
                                    n += 1
                                End If
                            Next
                        Next

                    End If

                Case FeatureType.Line
                    ' Dunno how to process

            End Select
        Next

        rs.Save()
        Return rs

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="fs"></param>
    ''' <param name="bRejectComplex"></param>
    ''' <param name="bRejectInvalid"></param>
    ''' <returns>The number of rejected features</returns>
    ''' <remarks></remarks>
    Public Shared Function CheckUsablePolygons(ByRef fs As IFeatureSet, ByVal bRejectComplex As Boolean, ByVal bRejectInvalid As Boolean) As Integer

        Dim iNumRejected As Integer = 0

        If (fs Is Nothing) Then Return iNumRejected

        ' Merge all valid polygons to avoid double-counting areas when polygons overlap
        For Each f As IFeature In fs.Features.CloneList

            Select Case f.FeatureType
                Case FeatureType.Point
                    Dim pt As New DotSpatial.Topology.Point(f.Coordinates(0))
                    If Not pt.IsValid Then
                        iNumRejected += 1
                        fs.Features.Remove(f)
                    End If
                Case FeatureType.MultiPoint
                Case FeatureType.Line

                Case FeatureType.Polygon
                    Dim polyTemp As New Polygon(f.Coordinates)
                    Dim bUsePolygon As Boolean = True

                    If (Not polyTemp.IsValid) Then
                        If (bRejectInvalid) Then
                            bUsePolygon = False
                            iNumRejected += 1
                        End If
                    ElseIf Not polyTemp.IsSimple Then
                        If bRejectComplex Then
                            bUsePolygon = False
                            iNumRejected += 1
                        End If
                    End If

                    If (Not bUsePolygon) Then
                        fs.Features.Remove(f)
                    End If
            End Select
        Next

        Return iNumRejected

    End Function

End Class
