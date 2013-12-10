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
    Public Shared Function Rasterize(ByVal fs As IFeatureSet,
                                     ByVal ptfTL As PointF, _
                                     ByVal ptfBR As PointF, _
                                     ByVal dCellSize As Double, _
                                     ByVal strFileName As String,
                                     ByVal log As cSpatialOperationLog, _
                                     ByVal dgt As TranslateValueDelegate) As IRaster

        Debug.Assert(dgt IsNot Nothing)

        Dim coords As New List(Of Coordinate)
        Dim featToConvert As IFeatureSet = Nothing
        Dim polyToConvert As IGeometry = Nothing
        Dim dValClear As Double
        Dim dValSet As Double

        ' -----
        ' Create and position raster 
        ' -----
        Dim bnds As IRasterBounds = cDotSpatialUtils.EcospaceToBounds(ptfTL, ptfBR, dCellSize)
        Dim rs As IRaster = Raster.Create(strFileName, "", bnds.NumColumns, bnds.NumRows, 1, GetType(Double), Nothing)
        rs.Projection = cDotSpatialUtils.EcospaceProjection
        rs.Bounds = bnds
        rs.NoDataValue = cCore.NULL_VALUE

        ' Wipe array
        dValClear = dgt.Invoke(Nothing, rs.NoDataValue)
        For iRow As Integer = 0 To bnds.NumRows - 1
            For iCol As Integer = 0 To bnds.NumColumns - 1
                rs.Value(iRow, iCol) = dValClear
            Next
        Next

        If (Not fs.Projection.Equals(cDotSpatialUtils.EcospaceProjection)) Then
            fs.Reproject(cDotSpatialUtils.EcospaceProjection)
            If (log IsNot Nothing) Then log.LogOperation(String.Format(My.Resources.OPERATION_REPROJECT, fs.ProjectionString), eStatusFlags.ValueComputed)
        End If

        Dim dtAttribs As DataTable = fs.DataTable
        For i As Integer = 0 To fs.Features.Count - 1

            Dim f As IFeature = fs.Features(i)
            Dim drow As DataRow = dtAttribs.Rows(i)
            dValSet = dgt.Invoke(drow, rs.NoDataValue)

            Select Case f.FeatureType
                Case FeatureType.Point
                    ' To test
                    Dim pt As IPoint = New DotSpatial.Topology.Point(f.Coordinates(0))
                    If rs.ContainsFeature(f) Then
                        Dim cellpos As RcIndex = rs.ProjToCell(CType(pt.Centroid, Coordinate))
                        rs.Value(cellpos.Row, cellpos.Column) = dValSet
                    End If

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
                    Dim poly As IPolygon = New Polygon(f.Coordinates)
                    For iRow As Integer = 0 To bnds.NumRows - 1
                        For iCol As Integer = 0 To bnds.NumColumns - 1
                            Dim pt As IPoint = New DotSpatial.Topology.Point(rs.CellToProj(iRow, iCol))
                            If poly.Contains(pt) Then
                                rs.Value(iRow, iCol) = dValSet
                            End If
                        Next
                    Next

                Case FeatureType.Line
                    ' Dunno how to process

            End Select
        Next

        rs.Save()
        Return rs

    End Function

    'Private Shared Function ToValue(objValue As Object, dtMapping As Dictionary(Of Object, Object)) As Double
    '    Dim dVal As Double = cCore.NULL_VALUE

    '    If (dtMapping IsNot Nothing) Then
    '        If dtMapping.ContainsKey(objValue) Then
    '            objValue = dtMapping(objValue)
    '        Else
    '            objValue = cCore.NULL_VALUE
    '        End If
    '    End If

    '    Try
    '        dVal = Convert.ToDouble(objValue)
    '    Catch ex As Exception
    '        ' Whoah!
    '    End Try
    '    Return dVal
    'End Function

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
        For Each featTmp As IFeature In fs.Features.CloneList
            If (featTmp.FeatureType = FeatureType.Polygon) Then
                Dim polyTemp As New Polygon(featTmp.Coordinates)
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
                    fs.Features.Remove(featTmp)
                End If
            End If
        Next

        Return iNumRejected

    End Function

End Class
