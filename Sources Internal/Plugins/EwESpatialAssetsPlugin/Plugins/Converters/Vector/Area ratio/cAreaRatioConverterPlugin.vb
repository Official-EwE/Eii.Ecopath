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
Imports System.Drawing
Imports DotSpatial.Data
Imports EwECore
Imports EwECore.SpatialData
Imports EwEUtils.Core
Imports EwEUtils.SpatialData
Imports EwEUtils.Utilities
Imports DotSpatial.Projections

#End Region ' Imports

Namespace SpatialData

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Spatial data converter that converts polygons to fractions of cell sizes.
    ''' </summary>
    ''' <remarks>
    ''' Converts an incoming vector map to a raster of a given spatial extent, cell 
    ''' size and standard Ecospace projection.
    ''' </remarks>
    ''' ---------------------------------------------------------------------------
    Public Class cAreaRatioConverterPlugin
        Inherits cSpatialDataConverter
        Implements IConfigurable

        Public Sub New()
            MyBase.New()
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <inheritdocs cref="IConfigurable.GetConfigUI"/>
        ''' -----------------------------------------------------------------------
        Public Function GetConfigUI() As System.Windows.Forms.Control _
            Implements EwEUtils.Core.IConfigurable.GetConfigUI

            Dim pg As New ucAttributeFilterConfigPage()
            pg.Converter = Me
            Return pg

        End Function

        ''' -----------------------------------------------------------------------
        ''' <inheritdocs cref="IConfigurable.IsConfigured"/>
        ''' -----------------------------------------------------------------------
        Public Overrides Function IsConfigured() As Boolean _
            Implements EwEUtils.Core.IConfigurable.IsConfigured

            Dim bConfigured As Boolean = True

            ' Cannot work with an attribute name - should be NULL
            bConfigured = bConfigured And String.IsNullOrWhiteSpace(Me.AttributeName)

            If Not String.IsNullOrWhiteSpace(Me.AttributeFilter) Then
                ' ToDo: validate if attribute filter is usable?
                ' NOP
            End If

            Return bConfigured
        End Function

        ''' -----------------------------------------------------------------------
        ''' <inheritdocs cref="cSpatialDataConverter.IsCompatible"/>
        ''' -----------------------------------------------------------------------
        Public Overrides Function IsCompatible(ds As ISpatialDataSet) As Boolean
            If (ds Is Nothing) Then Return False
            Return (ds.ConversionFormat = "DotSpatialVector")
        End Function

        ''' -----------------------------------------------------------------------
        ''' <inheritdocs cref="cSpatialDataConverter.Convert"/>
        ''' -----------------------------------------------------------------------
        Public Overrides Function Convert(ByVal data As Object, _
                                          ByVal ptfTL As PointF, _
                                          ByVal ptfBR As PointF, _
                                          ByVal dCellSize As Double, _
                                          ByVal strProjToWkt As String, _
                                          ByVal strFile As String) As ISpatialRaster

            Dim rstResult As IRaster = Nothing

            ' Sanity checks
            Debug.Assert((data IsNot Nothing) And (Not String.IsNullOrWhiteSpace(strFile)) And (dCellSize > 0))

            ' Validate data
            If (Not TypeOf data Is IFeatureSet) Then
                Me.LogMessage(My.Resources.STATUS_VALIDATIONFAILED_VECTORONLY, eStatusFlags.ErrorEncountered Or eStatusFlags.FailedValidation)
                Return Nothing
            End If

            ' Log
            Me.LogMessage(cStringUtils.Localize(My.Resources.STATUS_CONVERTER, Me.DisplayName), eStatusFlags.OK)

            Try
                Dim fs As IFeatureSet = CType(data, IFeatureSet)
                Dim projTo As ProjectionInfo = cDotSpatialUtils.ToProjection(strProjToWkt)

                ' Filter
                If Not String.IsNullOrWhiteSpace(Me.AttributeFilter) Then
                    fs = cDotSpatialUtils.FeatureSet(fs, Me.AttributeFilter)
                    Me.LogMessage(cStringUtils.Localize(My.Resources.OPERATION_EXTRACTVECTOR, Me.AttributeFilter), eStatusFlags.ValueComputed)
                End If

                ' Reproject
                If (Not fs.Projection.Equals(projTo)) Then
                    fs.Reproject(projTo)
                    Me.LogMessage(cStringUtils.Localize(My.Resources.OPERATION_REPROJECT, fs.ProjectionString), eStatusFlags.ValueComputed)
                End If

                ' Rasterize
                rstResult = cSurfaceTools.RasterizeArea(fs, ptfTL, ptfBR, dCellSize, strFile, Me.Log)

                If (rstResult IsNot Nothing) Then
                    rstResult.Close()
                Else
                    Debug.Assert(False)
                End If

                Me.LogMessage(cStringUtils.Localize(My.Resources.STATUS_RASTER_CACHED, strFile), eStatusFlags.OK)

            Catch ex As Exception
                Me.LogMessage(cStringUtils.Localize(My.Resources.STATUS_VECTORCONVERSION_EXCEPTION, ex.Message), eStatusFlags.ErrorEncountered)
            End Try

            Return New cSpatialRaster(rstResult)

        End Function

        ''' -----------------------------------------------------------------------
        ''' <inheritdocs cref="cSpatialDataConverter.DisplayName"/>
        ''' -----------------------------------------------------------------------
        Public Overrides ReadOnly Property DisplayName As String
            Get
                Return My.Resources.CONVERTER_AREARASTER_NAME
            End Get
        End Property

        ''' -----------------------------------------------------------------------
        ''' <inheritdocs cref="cSpatialDataConverter.Description"/>
        ''' -----------------------------------------------------------------------
        Public Overrides ReadOnly Property Description As String
            Get
                Return My.Resources.CONVERTER_AREARASTER_DESCR
            End Get
        End Property

        ''' -----------------------------------------------------------------------
        ''' <inheritdocs cref="cSpatialDataConverter.PluginName"/>
        ''' -----------------------------------------------------------------------
        Public Overrides ReadOnly Property PluginName As String
            Get
                Return "DotSpatial.VectorAreaConverter"
            End Get
        End Property

    End Class

End Namespace
