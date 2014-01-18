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
Imports EwECore
Imports EwEPlugin
Imports EwEUtils.Core
Imports EwEUtils.SpatialData
Imports EwEUtils.Utilities
Imports DotSpatial.Topology

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
        Implements ISpatialDataConverterPlugin

        ''' <summary>Filter for extracting features (vector set only).</summary>
        Private m_strAttributeFilter As String = ""
        ''' <summary>Name of attribute value to rasterize (vector set only).</summary>
        Private m_strAttributeName As String = ""
        Private m_core As cCore = Nothing

        ''' -----------------------------------------------------------------------
        ''' <inheritdocs cref="EwEUtils.SpatialData.ISpatialDataConverter.Dataset"/>
        ''' -----------------------------------------------------------------------
        Public Property Dataset As EwEUtils.SpatialData.ISpatialDataSet _
            Implements EwEUtils.SpatialData.ISpatialDataConverter.Dataset

        ''' -----------------------------------------------------------------------
        ''' <inheritdocs cref="ISpatialDataConverter.Configuration"/>
        ''' -----------------------------------------------------------------------
        Public Property Configuration(ByVal doc As System.Xml.XmlDocument) As System.Xml.XmlNode _
            Implements EwEUtils.SpatialData.ISpatialDataConverter.Configuration
            Get
                Return Nothing
            End Get
            Set(ByVal value As System.Xml.XmlNode)
                ' NOP: nothing to configure
            End Set
        End Property

        ''' -----------------------------------------------------------------------
        ''' <inheritdocs cref="ISpatialDataConverter.IsConfigured"/>
        ''' -----------------------------------------------------------------------
        Public Function IsConfigured() As Boolean _
            Implements EwEUtils.SpatialData.ISpatialDataConverter.IsConfigured
            Return True
        End Function

        ''' -----------------------------------------------------------------------
        ''' <inheritdocs cref="ISpatialDataConverter.IsCompatible"/>
        ''' -----------------------------------------------------------------------
        Public Function IsCompatible(ds As ISpatialDataSet) As Boolean _
            Implements ISpatialDataConverter.IsCompatible
            If (ds Is Nothing) Then Return False
            Return (ds.ConversionFormat = "DotSpatialRaster")
        End Function

        ''' -----------------------------------------------------------------------
        ''' <inheritdocs cref="ISpatialDataConverter.AttributeName"/>
        ''' -----------------------------------------------------------------------
        Public Property AttributeName As String Implements ISpatialDataConverter.AttributeName
            Get
                Return Me.m_strAttributeName
            End Get
            Set(value As String)
                Me.m_strAttributeName = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------
        ''' <inheritdocs cref="ISpatialDataConverter.AttributeFilter"/>
        ''' -----------------------------------------------------------------------
        Public Property AttributeFilter As String Implements ISpatialDataConverter.AttributeFilter
            Get
                Return Me.m_strAttributeFilter
            End Get
            Set(value As String)
                Me.m_strAttributeFilter = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------
        ''' <inheritdocs cref="ISpatialDataConverter.AttributeValueMappings"/>
        ''' -----------------------------------------------------------------------
        Public Property AttributeValueMappings As Dictionary(Of Object, Object) Implements ISpatialDataConverter.AttributeValueMappings
            Get
                Return Nothing
            End Get
            Set(value As System.Collections.Generic.Dictionary(Of Object, Object))
                ' Ignored
            End Set
        End Property

        ''' -----------------------------------------------------------------------
        ''' <inheritdocs cref="ISpatialDataConverter.Convert"/>
        ''' -----------------------------------------------------------------------
        Public Function Convert(ByVal data As Object, _
                                ByVal ptfTL As PointF, _
                                ByVal ptfBR As PointF, _
                                ByVal dCellSize As Double, _
                                ByVal strFile As String) As ISpatialRaster _
            Implements EwEUtils.SpatialData.ISpatialDataConverter.Convert

            Dim rstResult As IRaster = Nothing
            Dim ext As Extent = cDotSpatialUtils.Extent(ptfTL, ptfBR)

            ' Sanity checks
            Debug.Assert((data IsNot Nothing) And (Not String.IsNullOrWhiteSpace(strFile)) And (dCellSize > 0))

            ' Validate data
            If (Not TypeOf data Is IDataSet) Then
                cLog.Write(Me.DisplayName & ": cannot convert data of type " & data.GetType().ToString, eVerboseLevel.Detailed)
                Return Nothing
            End If

            ' Log
            Me.LogMessage(String.Format(My.Resources.STATUS_CONVERTER, Me.DisplayName), eStatusFlags.OK)

            ' Perform conversion
            If (TypeOf data Is IRaster) Then
                Try
                    Dim rs As IRaster = CType(data, IRaster)
                    Dim bMustCache As Boolean = False

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
                            ' DotSpatial ReadBlock has a bug
                            ' JS: Earlier code that used a rectangular extraction only succeeded if the entir erectangular area was contained withint rs
                            ' 
                            rstResult = rs.ReadBlock(x, y, Math.Max(dx, 2), Math.Max(dy, 2))
                            bndsCheck = rstResult.Bounds

                            ' Bounds reversed? (bug in DotSpatial.Data.Raster(T).ReadBlock)
                            If (bndsCheck.NumRows = dx And bndsCheck.NumColumns = dy) Then
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
                            Me.LogMessage(String.Format(My.Resources.OPERATION_EXTRACTRASTER, cDotSpatialUtils.FormatExtent(rs.Bounds), cDotSpatialUtils.FormatRasterGrid(rs)), eStatusFlags.ValueComputed)
                            ' Converted data must be cached
                            bMustCache = True

                        End If
                    Else
                        ' Log
                        Me.LogMessage(String.Format(My.Resources.STATUS_NO_OVERLAP, cDotSpatialUtils.FormatExtent(rs.Bounds)), eStatusFlags.ErrorEncountered)
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
                        Me.LogMessage(String.Format(My.Resources.OPERATION_RESAMPLE, cDotSpatialUtils.FormatRasterGrid(rstResult), cDotSpatialUtils.FormatRasterStats(rs)), eStatusFlags.ValueComputed)
                    Else
                        rstResult = rs
                    End If

                    ' Need to cache?
                    If bMustCache Then
                        ' #Yes: save
                        rstResult.SaveAs(strFile)
                        ' Log
                        Me.LogMessage(String.Format(My.Resources.STATUS_RASTER_CACHED, strFile), eStatusFlags.OK)
                    End If

                Catch ex As Exception
                    ' Log
                    Me.LogMessage(String.Format(My.Resources.STATUS_RASTERCONVERSION_EXCEPTION, ex.Message), eStatusFlags.ErrorEncountered)
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
        ''' <inheritdocs cref="ISpatialDataConverter.DisplayName"/>
        ''' -----------------------------------------------------------------------
        Public ReadOnly Property DisplayName As String _
            Implements ISpatialDataConverter.DisplayName
            Get
                Return My.Resources.CONVERTER_DIRECTRASTER_NAME
            End Get
        End Property

        ''' -----------------------------------------------------------------------
        ''' <inheritdocs cref="ISpatialDataConverter.Description"/>
        ''' -----------------------------------------------------------------------
        Public ReadOnly Property Description As String _
            Implements ISpatialDataConverter.Description, EwEPlugin.IPlugin.Description
            Get
                Return My.Resources.CONVERTER_DIRECTRASTER_DESCR
            End Get
        End Property

        ''' -----------------------------------------------------------------------
        ''' <inheritdocs cref="ISpatialDataConverterPlugin.Author"/>
        ''' -----------------------------------------------------------------------
        Public ReadOnly Property Author As String _
            Implements ISpatialDataConverterPlugin.Author
            Get
                Return "Jeroen Steenbeek, UBC Fisheries Centre"
            End Get
        End Property

        ''' -----------------------------------------------------------------------
        ''' <inheritdocs cref="ISpatialDataConverterPlugin.Contact"/>
        ''' -----------------------------------------------------------------------
        Public ReadOnly Property Contact As String _
            Implements ISpatialDataConverterPlugin.Contact
            Get
                Return "mailto:ewedevteam@gmail.com"
            End Get
        End Property

        ''' -----------------------------------------------------------------------
        ''' <inheritdocs cref="ISpatialDataConverterPlugin.Initialize"/>
        ''' -----------------------------------------------------------------------
        Public Sub Initialize(ByVal core As Object) _
            Implements ISpatialDataConverterPlugin.Initialize
            Me.m_core = DirectCast(core, cCore)
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <inheritdocs cref="IPlugin.Name"/>
        ''' -----------------------------------------------------------------------
        Public ReadOnly Property PlugingName As String _
            Implements EwEPlugin.IPlugin.Name
            Get
                Return "DotSpatial.DefaultRasterConverter"
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return Me.DisplayName()
        End Function

        Private Sub LogMessage(strMessage As String, status As eStatusFlags)

            If (Me.m_core IsNot Nothing) Then
                Me.m_core.SpatialOperationLog.LogOperation(strMessage, status)
            End If

        End Sub

    End Class

End Namespace
