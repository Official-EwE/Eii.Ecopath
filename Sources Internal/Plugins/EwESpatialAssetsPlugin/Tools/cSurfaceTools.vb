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
' Copyright 1991- UBC Fisheries Centre, Vancouver BC, Canada.
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

Public Class cSurfaceTools

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Convert features in a polygon feature set to a raster, populating each
    ''' raster cell with a ratio [0, 1] that the polygon area overlapped with
    ''' the cell area.
    ''' </summary>
    ''' <param name="fs">The polygon feature set to convert.</param>
    ''' <param name="dCellSize">Cell size, in decimal degrees, of the raster to create.</param>
    ''' <param name="strFilter">Attribute value filter to find the polygons to 
    ''' rasterize. If left empty, all features in the feature set will be 
    ''' rasterized.</param>
    ''' <param name="strFileName">The output file name to write the raster to.</param>
    ''' <param name="Log"><see cref="cSpatialOperationLog"/> for logging operations.</param>
    ''' <returns>A raster.</returns>
    ''' <remarks>
    ''' <para>This operation uses 'cookie cutter' polygons for clipping polygons and
    ''' performing area ratio calculations. Due to limitations in the cookie cutter
    ''' positioning logic this operation is forced to operate on a WGS84 projection,
    ''' This projection should not be used to calculate areas. Since both the feature 
    ''' set and the produced raster are sharing this projection this effect is 
    ''' somewhat mitigated, but area ratios may still be inaccurate in
    ''' extreme lattitude ranges.</para>
    ''' <para>The obvious solution would be to make this operation work with 
    ''' a global cylindrical equal area projection.</para>
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Public Shared Function RasterizeArea(ByVal fs As IFeatureSet,
                                         ByVal ptfTL As PointF, _
                                         ByVal ptfBR As PointF, _
                                         ByVal dCellSize As Double, _
                                         ByVal strFilter As String, _
                                         ByVal strFileName As String,
                                         ByVal log As cSpatialOperationLog) As IRaster

        'Dim projWork As ProjectionInfo = KnownCoordinateSystems.Projected.World.CylindricalEqualAreaworld
        Dim coords As New List(Of Coordinate)
        Dim featToConvert As IFeatureSet = Nothing
        Dim polyToConvert As IGeometry = Nothing
        Dim iNumRejected As Integer = 0

        If (Not fs.Projection.Equals(cDotSpatialUtils.EcospaceProjection)) Then
            fs.Reproject(cDotSpatialUtils.EcospaceProjection)
            If (log IsNot Nothing) Then log.LogOperation(cStringUtils.Localize(My.Resources.OPERATION_REPROJECT, fs.ProjectionString), _
                                                         eStatusFlags.ValueComputed)
        End If

        ' -----
        ' Build list of features to rasterize
        ' -----

        ' Has a filter?
        If Not String.IsNullOrWhiteSpace(strFilter) Then
            ' #Yes: Grab all features that match the filter
            featToConvert = New FeatureSet()
            featToConvert.Projection = fs.Projection
            For Each i As Integer In fs.SelectIndexByAttribute(strFilter)
                featToConvert.AddFeature(fs.Features(i))
            Next

            If (log IsNot Nothing) Then log.LogOperation(cStringUtils.Localize(My.Resources.OPERATION_EXTRACTPLOYGONS, strFilter), eStatusFlags.ValueComputed)
        Else
            ' #No: grab entire feature set
            featToConvert = fs
        End If

        iNumRejected = cVectorTools.CheckUsablePolygons(featToConvert, True, True)

        If ((iNumRejected > 0) And (log IsNot Nothing)) Then
            log.LogOperation(cStringUtils.Localize(My.Resources.STATUS_POLYGONSFAILED_INVALID, iNumRejected), eStatusFlags.MissingParameter)
        End If

        ' -----
        ' Create and position raster 
        ' -----
        Dim bnds As IRasterBounds = cDotSpatialUtils.EcospaceToBounds(ptfTL, ptfBR, dCellSize)
        Dim rs As IRaster = Raster.Create(strFileName, "", bnds.NumColumns, bnds.NumRows, 1, GetType(Double), Nothing)
        rs.Projection = cDotSpatialUtils.EcospaceProjection
        rs.Bounds = bnds
        rs.NoDataValue = 0
        'rs.Reproject(fs.Projection)

        If (log IsNot Nothing) Then log.LogOperation(cStringUtils.Localize(My.Resources.OPERATION_EXTRACTPLOYGONS, strFilter), eStatusFlags.ValueComputed)

        ' ToDo: This loop can be sped up by only processing those cells that overlap with the extent of the dataset
        ' For all cols, rows
        For iRow As Integer = 0 To rs.NumRows - 1
            For iCol As Integer = 0 To rs.NumColumns - 1

                If (polyToConvert IsNot Nothing) Then

                    ' Create cookie cutter for a cell
                    Dim ptCut = rs.CellToProj(iRow, iCol)
                    coords.Clear()
                    coords.Add(New Coordinate(ptCut.X - rs.CellWidth / 2, ptCut.Y - rs.CellHeight / 2))
                    coords.Add(New Coordinate(ptCut.X + rs.CellWidth / 2, ptCut.Y - rs.CellHeight / 2))
                    coords.Add(New Coordinate(ptCut.X + rs.CellWidth / 2, ptCut.Y + rs.CellHeight / 2))
                    coords.Add(New Coordinate(ptCut.X - rs.CellWidth / 2, ptCut.Y + rs.CellHeight / 2))
                    coords.Add(coords(0)) ' Close polygon

                    Dim polyCut As New Polygon(coords)
                    Dim extCut As Extent = polyCut.Envelope.ToExtent
                    Dim dAreaCell As Double = polyCut.Area
                    Dim dAreaOverlap As Double = 0.0

                    ' See useful discussion for faster polygon overlap processing: http://dotspatial.codeplex.com/discussions/265535
                    Dim candidates As List(Of IFeature) = featToConvert.Select(extCut)
                    If (candidates IsNot Nothing) Then

                        For Each featTmp As IFeature In candidates
                            If (featTmp.FeatureType = FeatureType.Polygon) Then
                                Dim polyTemp As New Polygon(featTmp.Coordinates)
                                If polyToConvert Is Nothing Then
                                    polyToConvert = polyTemp
                                Else
                                    polyToConvert = polyToConvert.Union(polyTemp)
                                End If
                            End If
                        Next

                        Try
                            ' Get intersection of cell with feature
                            Dim fIntersect As IGeometry = polyCut.Intersection(polyToConvert)
                            ' Sum fraction area
                            dAreaOverlap += fIntersect.Area
                        Catch ex As Exception
                            ' Woops
                            If (log IsNot Nothing) Then
                                log.LogOperation(cStringUtils.Localize("An error occurred in RasterizeArea({0}.{1}). {2}", iRow, iCol, ex.Message), eStatusFlags.MissingParameter)
                            End If
                            ' Do not obliterate polygon, keep plowing on
                            'polyToConvert = Nothing
                        End Try
                    End If

                    rs.Value(iRow, iCol) = dAreaOverlap / dAreaCell

                End If

            Next iCol
        Next iRow

        rs.Save()
        Return rs

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Convert features in a polygon feature set to a raster, populating each
    ''' raster cell with a ratio [0, 1] that the polygon area overlapped with
    ''' the cell area.
    ''' </summary>
    ''' <param name="fs">The polygon feature set to convert.</param>
    ''' <param name="dCellWidth">Cell width, in decimal degrees, of the raster to create.</param>
    ''' <param name="strField">Attribute field to convert.</param>
    ''' <param name="strFileName">The output file name to write the raster to.</param>
    ''' <param name="Log"><see cref="cSpatialOperationLog"/> for logging operations.</param>
    ''' <returns>A raster.</returns>
    ''' <remarks>
    ''' <para>This operation uses 'cookie cutter' polygons for clipping polygons and
    ''' performing area ratio calculations. Due to limitations in the cookie cutter
    ''' positioning logic this operation is forced to operate on a WGS84 projection,
    ''' This projection should not be used to calculate areas. Since both the feature 
    ''' set and the produced raster are sharing this projection this effect is 
    ''' somewhat mitigated, but area ratios may still be inaccurate in
    ''' extreme lattitude ranges.</para>
    ''' <para>The obvious solution would be to make this operation work with 
    ''' a global cylindrical equal area projection.</para>
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Public Shared Function RasterizeIsobar(ByVal fs As IFeatureSet,
                                           ByVal ptfTL As PointF, _
                                           ByVal ptfBR As PointF, _
                                           ByVal dCellWidth As Double, _
                                           ByVal strField As String, _
                                           ByVal strFileName As String,
                                           ByVal log As cSpatialOperationLog) As IRaster

        'Dim projWork As ProjectionInfo = KnownCoordinateSystems.Projected.World.CylindricalEqualAreaworld
        Dim featToConvert As IFeatureSet = Nothing
        Dim iNumRejected As Integer = 0

        If (Not fs.Projection.Equals(cDotSpatialUtils.EcospaceProjection)) Then
            fs.Reproject(cDotSpatialUtils.EcospaceProjection)
            If (log IsNot Nothing) Then log.LogOperation(cStringUtils.Localize(My.Resources.OPERATION_REPROJECT, fs.ProjectionString), eStatusFlags.ValueComputed)
        End If

        featToConvert = fs

        iNumRejected = cVectorTools.CheckUsablePolygons(featToConvert, True, True)

        If ((iNumRejected > 0) And (log IsNot Nothing)) Then
            log.LogOperation(cStringUtils.Localize(My.Resources.STATUS_POLYGONSFAILED_INVALID, iNumRejected), eStatusFlags.MissingParameter)
        End If

        ' -----
        ' Create and position raster 
        ' -----
        Dim bnds As IRasterBounds = cDotSpatialUtils.EcospaceToBounds(ptfTL, ptfBR, dCellWidth)
        Dim rs As IRaster = Raster.Create(strFileName, "", bnds.NumColumns, bnds.NumRows, 1, GetType(Double), Nothing)
        rs.Projection = cDotSpatialUtils.EcospaceProjection
        rs.Bounds = bnds
        rs.NoDataValue = 0

        Dim dCellHeight As Double = rs.CellHeight

        Debug.Assert(cNumberUtils.Approximates(rs.CellWidth, dCellWidth, dCellWidth * 0.1))
        Debug.Assert(cNumberUtils.Approximates(dCellHeight, dCellWidth, rs.CellHeight * 0.1))

        If (log IsNot Nothing) Then log.LogOperation(cStringUtils.Localize(My.Resources.OPERATION_EXTRACTPLOYGONS, ""), eStatusFlags.ValueComputed)

        ' ToDo: This loop can be sped up by only processing those cells that overlap with the extent of the dataset
        ' For all cols, rows
        For iRow As Integer = 0 To rs.NumRows - 1
            For iCol As Integer = 0 To rs.NumColumns - 1

                ' Create cookie cutter for a cell
                Dim ptCut = rs.CellToProj(iRow, iCol)
                Dim coords As New List(Of Coordinate)
                coords.Add(New Coordinate(ptCut.X - dCellWidth / 2, ptCut.Y - dCellHeight / 2))
                coords.Add(New Coordinate(ptCut.X + dCellWidth / 2, ptCut.Y - dCellHeight / 2))
                coords.Add(New Coordinate(ptCut.X + dCellWidth / 2, ptCut.Y + dCellHeight / 2))
                coords.Add(New Coordinate(ptCut.X - dCellWidth / 2, ptCut.Y + dCellHeight / 2))
                coords.Add(coords(0)) ' Close polygon

                Dim polyCut As New Polygon(coords)
                Dim dAreaTot As Double = 0
                Dim dValueTot As Double = 0

                ' See useful discussion for faster polygon overlap processing: http://dotspatial.codeplex.com/discussions/265535
                Dim candidates As List(Of IFeature) = featToConvert.Select(polyCut.Envelope.ToExtent)
                If (candidates IsNot Nothing) Then

                    For Each featTmp As IFeature In candidates

                        If (featTmp.FeatureType = FeatureType.Polygon) Then
                            Dim polyTemp As New Polygon(featTmp.Coordinates)
                            Dim dr As DataRow = featTmp.DataRow

                            Try
                                ' Get intersection of cell with feature
                                Dim fIntersect As IGeometry = polyCut.Intersection(polyTemp)
                                Dim dArea As Double = fIntersect.Area
                                ' Sum fraction area
                                dAreaTot += dArea
                                dValueTot += CDbl(dr(strField)) * dArea
                            Catch ex As Exception
                                ' Woops
                                If (log IsNot Nothing) Then
                                    log.LogOperation(cStringUtils.Localize("An error occurred in RasterizeArea({0}.{1}). {2}", iRow, iCol, ex.Message), eStatusFlags.MissingParameter)
                                End If
                                ' Do not obliterate polygon, keep plowing on
                                'polyToConvert = Nothing
                            End Try
                        End If
                    Next
                End If

                If dAreaTot > 0 Then
                    rs.Value(iRow, iCol) = dValueTot / dAreaTot
                Else
                    rs.Value(iRow, iCol) = cCore.NULL_VALUE
                End If

            Next iCol
        Next iRow

        rs.Save()
        Return rs

    End Function
End Class
