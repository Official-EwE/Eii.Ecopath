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
Imports System.Xml
Imports DotSpatial.Data
Imports DotSpatial.Projections
Imports DotSpatial.Topology
Imports EwECore
Imports EwEUtils.Core
Imports EwEUtils.SpatialData
Imports EwEUtils.Utilities

#End Region ' Imports

Namespace SpatialData

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Default spatial data converter.
    ''' </summary>
    ''' <remarks>
    ''' Clips and resamples an incoming raster to an Ecospace raster.
    ''' </remarks>
    ''' ---------------------------------------------------------------------------
    Public Class cRasterConverterPlugin
        Inherits cSpatialDataConverter

        Public Sub New()
            MyBase.New()
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <inheritdocs cref="cSpatialDataConverter.IsConfigured"/>
        ''' -----------------------------------------------------------------------
        Public Overrides Function IsConfigured() As Boolean
            Return True
        End Function

        ''' -----------------------------------------------------------------------
        ''' <inheritdocs cref="cSpatialDataConverter.IsCompatible"/>
        ''' -----------------------------------------------------------------------
        Public Overrides Function IsCompatible(ds As ISpatialDataSet) As Boolean
            If (ds Is Nothing) Then Return False
            Return (ds.ConversionFormat = "DotSpatialRaster")
        End Function

        ''' -----------------------------------------------------------------------
        ''' <inheritdocs cref="ISpatialDataConverter.Convert"/>
        ''' -----------------------------------------------------------------------
        Public Overrides Function Convert(ByVal data As Object, _
                                          ByVal ptfTL As PointF, _
                                          ByVal ptfBR As PointF, _
                                          ByVal dCellSize As Double, _
                                          ByVal strProjectionString As String, _
                                          ByVal strFile As String) As ISpatialRaster

            Dim rstResult As IRaster = Nothing
            Dim ext As Extent = cDotSpatialUtils.Extent(ptfTL, ptfBR)
            Dim proj As ProjectionInfo = cDotSpatialUtils.ToProjection(strProjectionString)

            ' Sanity checks
            Debug.Assert((data IsNot Nothing) And (Not String.IsNullOrWhiteSpace(strFile)) And (dCellSize > 0))

            ' Validate data
            If (Not TypeOf data Is IDataSet) Then
                cLog.Write(Me.DisplayName & ": cannot convert data of type " & data.GetType().ToString, eVerboseLevel.Detailed)
                Return Nothing
            End If

            ' Log
            Me.LogMessage(cStringUtils.Localize(My.Resources.STATUS_CONVERTER, Me.DisplayName), eStatusFlags.OK)

            ' Perform conversion
            If (TypeOf data Is IRaster) Then
                Try
                    Dim rs As IRaster = CType(data, IRaster)
                    Dim bMustCache As Boolean = False

                    ' Same projection?
                    If (Not rs.Projection.Equals(proj)) Then
                        rs.Reproject(proj)
                        Me.LogMessage(cStringUtils.Localize(My.Resources.OPERATION_REPROJECT, strProjectionString), eStatusFlags.ValueComputed)
                    End If

                    ' Does overlap?
                    If (rs.Bounds.Extent.Intersects(ext)) Then
                        ' Perform extraction if extents do not match
                        If (Not cDotSpatialUtils.Approximates(rs.Extent, ext, dCellSize * cDotSpatialUtils.EQUALS_FACTOR)) Then

                            ' JS Verified 18/feb/13

                            ' Get intersection extent
                            Dim extIntersect As Extent = rs.Extent.Intersection(ext)

                            ' Extract intersection area
                            Dim tl As RcIndex = rs.ProjToCell(extIntersect.MinX, extIntersect.MaxY)
                            Dim br As RcIndex = rs.ProjToCell(extIntersect.MaxX, extIntersect.MinY)
                            Dim x As Integer = Math.Max(tl.Column, 0)
                            Dim y As Integer = Math.Max(tl.Row, 0)
                            Dim dx As Integer = br.Column - x
                            Dim dy As Integer = br.Row - y
                            Dim bndsCheck As IRasterBounds = Nothing

                            ' Extract data block for this area
                            rstResult = rs.ReadBlock(x, y, Math.Max(dx, 2), Math.Max(dy, 2))

                            ' JS: Earlier code that used a rectangular extraction only succeeded if the entire rectangular area was contained within rs
                            ' Check if bounds rows and cols are reversed (this is a bug in DotSpatial.Data.Raster(T).ReadBlock)
                            bndsCheck = rstResult.Bounds
                            If (bndsCheck.NumRows = dx And bndsCheck.NumColumns = dy) Then
                                ' #Yes: reconstruct bounds properly which does not affect the raster data
                                ' Checked, JS 18Jan14
                                Dim topleft As Coordinate = rs.Bounds.CellCenter_ToProj(y, x)
                                Dim aff(6) As Double
                                Array.Copy(rs.Bounds.AffineCoefficients, 0, aff, 0, 6)
                                aff(0) = topleft.X
                                aff(3) = topleft.Y
                                rstResult.Bounds = New RasterBounds(dy, dx, aff)
                            End If

                            rs.Close()
                            rs = rstResult

                            ' Log
                            Me.LogMessage(cStringUtils.Localize(My.Resources.OPERATION_EXTRACTRASTER, cDotSpatialUtils.FormatExtent(rs.Bounds), cDotSpatialUtils.FormatRasterGrid(rs)), eStatusFlags.ValueComputed)
                            ' Converted data must be cached
                            bMustCache = True

                        End If
                    Else
                        ' Log
                        Me.LogMessage(cStringUtils.Localize(My.Resources.STATUS_NO_OVERLAP, cDotSpatialUtils.FormatExtent(rs.Bounds)), eStatusFlags.ErrorEncountered)
                        Return Nothing
                    End If

                    ' Cell sizes differ?
                    If Not (cNumberUtils.Approximates(rs.CellHeight, dCellSize, dCellSize * cDotSpatialUtils.EQUALS_FACTOR) And _
                            cNumberUtils.Approximates(rs.CellWidth, dCellSize, dCellSize * cDotSpatialUtils.EQUALS_FACTOR)) Then

                        ' #Yes: need to resample cells

                        ' JS Verified 18/feb/13

                        'rstResult = DotSpatial.Analysis.ResampleCells.Resample(rs, dCellSize, dCellSize, cFileUtils.MakeTempFile(IO.Path.GetExtension(strFile)))
                        rstResult = cDotSpatialUtils.ResampleToEcospace(rs, ptfTL, ptfBR, dCellSize, cFileUtils.MakeTempFile(IO.Path.GetExtension(strFile)))
                        rstResult.Close()
                        bMustCache = True

                        'Dim wrap As New cSpatialRaster(rstResult)
                        'wrap.Save("D:\Nereus\EwE output\NCAdriaticSea_Fitted7502_Ecospace_Fouzaicorrected\Ecospace_new6 (Valid)\_debug_\resample.asc")

                        ' Sanity checks
                        Debug.Assert(cNumberUtils.Approximates(dCellSize, rstResult.Bounds.CellWidth, dCellSize * cDotSpatialUtils.EQUALS_FACTOR))
                        Debug.Assert(cNumberUtils.Approximates(dCellSize, rstResult.Bounds.CellHeight, dCellSize * cDotSpatialUtils.EQUALS_FACTOR))

                        ' Log
                        Me.LogMessage(cStringUtils.Localize(My.Resources.OPERATION_RESAMPLE, cDotSpatialUtils.FormatRasterGrid(rstResult), cDotSpatialUtils.FormatRasterStats(rs)), eStatusFlags.ValueComputed)
                    Else
                        rstResult = rs
                    End If

                    ' Need to cache?
                    If bMustCache Then
                        ' #Yes: save
                        rstResult.SaveAs(strFile)
                        ' Log
                        Me.LogMessage(cStringUtils.Localize(My.Resources.STATUS_RASTER_CACHED, strFile), eStatusFlags.OK)
                    End If

                Catch ex As Exception
                    ' Log
                    Me.LogMessage(cStringUtils.Localize(My.Resources.STATUS_RASTERCONVERSION_EXCEPTION, ex.Message), eStatusFlags.ErrorEncountered)
                    Return Nothing
                End Try

            Else
                ' Log error
                Me.LogMessage(My.Resources.STATUS_VALIDATIONFAILED_RASTERONLY, _
                              eStatusFlags.ErrorEncountered Or eStatusFlags.FailedValidation)
            End If

            Return New cSpatialRaster(rstResult)

        End Function

        ''' -----------------------------------------------------------------------
        ''' <inheritdocs cref="cSpatialDataConverter.DisplayName"/>
        ''' -----------------------------------------------------------------------
        Public Overrides ReadOnly Property DisplayName As String
            Get
                Return My.Resources.CONVERTER_DIRECTRASTER_NAME
            End Get
        End Property

        ''' -----------------------------------------------------------------------
        ''' <inheritdocs cref="cSpatialDataConverter.Description"/>
        ''' -----------------------------------------------------------------------
        Public Overrides ReadOnly Property Description As String
            Get
                Return My.Resources.CONVERTER_DIRECTRASTER_DESCR
            End Get
        End Property

        ''' -----------------------------------------------------------------------
        ''' <inheritdocs cref="cSpatialDataConverter.PluginName"/>
        ''' -----------------------------------------------------------------------
        Public Overrides ReadOnly Property PluginName As String
            Get
                Return "DotSpatial.DefaultRasterConverter"
            End Get
        End Property

#Region " Configuration "

        Protected Overrides Function FromXML(doc As XmlDocument, node As XmlNode) As Boolean
            ' Not needed for this type of converter
            Return True
        End Function

        Protected Overrides Function ToXML(doc As XmlDocument) As XmlNode
            ' Not needed for this type of converter
            Return Nothing
        End Function

#End Region ' Configuration

    End Class

End Namespace
