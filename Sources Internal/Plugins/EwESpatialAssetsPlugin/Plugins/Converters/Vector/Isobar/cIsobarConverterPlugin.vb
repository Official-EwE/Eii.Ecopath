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
Imports System.Drawing
Imports DotSpatial.Data
Imports EwECore
Imports EwECore.SpatialData
Imports EwEUtils.Core
Imports EwEUtils.SpatialData
Imports EwEUtils.Utilities

#End Region ' Imports

Namespace SpatialData

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Spatial data converter that converts the area of all ploygons with a given
    ''' attribute value to a fractions of cell area value.
    ''' </summary>
    ''' <remarks>
    ''' Converts an incoming vector map to a raster of a given spatial extent, cell 
    ''' size and standard Ecospace projection.
    ''' </remarks>
    ''' ---------------------------------------------------------------------------
    Public Class cIsobarConverterPlugin
        Inherits cSpatialDataConverter
        Implements IConfigurable

        ''' -----------------------------------------------------------------------
        ''' <inheritdocs cref="ISpatialDataConverter.IsConfigured"/>
        ''' -----------------------------------------------------------------------
        Public Overrides Function IsConfigured() As Boolean _
            Implements IConfigurable.IsConfigured
            Return Not String.IsNullOrWhiteSpace(Me.AttributeName)
        End Function

        ''' -----------------------------------------------------------------------
        ''' <inheritdocs cref="IConfigurable.GetConfigUI"/>
        ''' -----------------------------------------------------------------------
        Public Function GetConfigUI() As System.Windows.Forms.Control _
            Implements IConfigurable.GetConfigUI
            Dim pg As New ucAttributeNameConfigPage()
            pg.Converter = Me
            Return pg
        End Function

        ''' -----------------------------------------------------------------------
        ''' <inheritdocs cref="cSpatialDataConverter.IsCompatible"/>
        ''' -----------------------------------------------------------------------
        Public Overrides Function IsCompatible(ds As ISpatialDataSet) As Boolean
            If (ds Is Nothing) Then Return False
            Return (ds.ConversionFormat = "DotSpatialVector") And ((ds.VarName = eVarNameFlags.LayerDepth) Or (ds.VarName = eVarNameFlags.NotSet))
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

            Dim log As cSpatialOperationLog = Nothing
            Dim rstResult As IRaster = Nothing

            If (Me.m_core IsNot Nothing) Then log = Me.m_core.SpatialOperationLog

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
                Me.LogMessage(My.Resources.STATUS_VALIDATIONFAILED_VECTORONLY, eStatusFlags.ErrorEncountered Or eStatusFlags.FailedValidation)
            ElseIf (TypeOf data Is IFeatureSet) Then
                Try
                    ' Rasterize the features
                    Dim fs As IFeatureSet = CType(data, IFeatureSet)
                    rstResult = cSurfaceTools.RasterizeIsobar(fs, ptfTL, ptfBR, dCellSize, strProjectionString, _
                                                              Me.AttributeName, strFile, log)
                    rstResult.Close()
                    Debug.Assert(rstResult IsNot Nothing)

                    Me.LogMessage(cStringUtils.Localize(My.Resources.STATUS_RASTER_CACHED, strFile), eStatusFlags.OK)

                Catch ex As Exception
                    Me.LogMessage(cStringUtils.Localize(My.Resources.STATUS_VECTORCONVERSION_EXCEPTION, ex.Message), eStatusFlags.ErrorEncountered)
                End Try

            End If

            Return New cSpatialRaster(rstResult)

        End Function

        ''' -----------------------------------------------------------------------
        ''' <inheritdocs cref="cSpatialDataConverter.DisplayName"/>
        ''' -----------------------------------------------------------------------
        Public Overrides ReadOnly Property DisplayName As String
            Get
                Return My.Resources.CONVERTER_ISOBAR_NAME

                'If String.IsNullOrWhiteSpace(Me.AttributeName) Then
                '    Return My.Resources.CONVERTER_ISOBAR_NAME
                'Else
                '    ' ToDo: globalize this
                '    'Return cStringUtils.Localize(SharedResources.GENERIC_LABEL_DETAILED, _
                '    '                     My.Resources.CONVERTER_ISOBAR_NAME, _
                '    '                     cStringUtils.Localize(SharedResources.GENERIC_LABEL_DOUBLE, "Field", Me.AttributeName))
                '    Return (My.Resources.CONVERTER_ISOBAR_NAME & " using values from " & Me.AttributeName)
                'End If

            End Get
        End Property

        ''' -----------------------------------------------------------------------
        ''' <inheritdocs cref="cSpatialDataConverter.Description"/>
        ''' -----------------------------------------------------------------------
        Public Overrides ReadOnly Property Description As String
            Get
                Return My.Resources.CONVERTER_ISOBAR_DESCR
            End Get
        End Property

        ''' -----------------------------------------------------------------------
        ''' <inheritdocs cref="cSpatialDataConverter.pluginName"/>
        ''' -----------------------------------------------------------------------
        Public Overrides ReadOnly Property PluginName As String
            Get
                Return "DotSpatial.IsobarConverter"
            End Get
        End Property

    End Class

End Namespace
