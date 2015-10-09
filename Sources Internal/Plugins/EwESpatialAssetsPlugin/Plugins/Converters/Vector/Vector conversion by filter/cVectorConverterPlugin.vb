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
Imports EwECore.SpatialData
Imports EwEUtils.Core
Imports EwEUtils.SpatialData
Imports EwEUtils.Utilities
Imports DotSpatial.Projections

#End Region ' Imports

Namespace SpatialData

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Default spatial data converter.
    ''' </summary>
    ''' <remarks>
    ''' Converts an incoming vector map to a raster of an Ecospace raster.
    ''' </remarks>
    ''' ---------------------------------------------------------------------------
    Public Class cVectorConverterPlugin
        Inherits cSpatialDataConverter
        Implements IConfigurable

        Public Sub New()
            MyBase.New()
        End Sub

        Public Overridable Function GetConfigUI() As System.Windows.Forms.Control _
            Implements EwEUtils.Core.IConfigurable.GetConfigUI
            Dim pg As New ucGenericVectorConverterConfigPage()
            pg.Converter = Me
            Return pg
        End Function

        ''' -----------------------------------------------------------------------
        ''' <inheritdocs cref="cSpatialDataConverter.IsConfigured"/>
        ''' -----------------------------------------------------------------------
        Public Overrides Function IsConfigured() As Boolean _
            Implements EwEUtils.Core.IConfigurable.IsConfigured
            Return True
        End Function

        ''' -----------------------------------------------------------------------
        ''' <inheritdocs cref="cSpatialDataConverter.IsCompatible"/>
        ''' -----------------------------------------------------------------------
        Public Overrides Function IsCompatible(ds As ISpatialDataSet) As Boolean
            If (ds Is Nothing) Then Return False
            Return (ds.ConversionFormat = "DotSpatialVector")
        End Function

        ''' -----------------------------------------------------------------------
        ''' <inheritdocs cref="ISpatialDataConverter.Convert"/>
        ''' -----------------------------------------------------------------------
        Public Overrides Function Convert(ByVal data As Object, _
                                          ByVal ptfTL As PointF,
                                          ByVal ptfBR As PointF,
                                          ByVal dCellSize As Double,
                                          ByVal strProjectionString As String, _
                                          ByVal strFile As String) As ISpatialRaster

            Dim log As cSpatialOperationLog = Nothing
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

            ' Get Ecospace raster bounds
            Dim bnds As IRasterBounds = cDotSpatialUtils.EcospaceToBounds(ptfTL, ptfBR, dCellSize)
            Dim rstResult As IRaster = Nothing

            If (TypeOf data Is IFeatureSet) Then
                Try

                    Dim fs As IFeatureSet = CType(data, IFeatureSet)

                    ' Is attribute filter specified?
                    If Not String.IsNullOrWhiteSpace(Me.AttributeFilter) Then
                        ' #Yes: extract features that match the filter
                        Dim lFeatures As List(Of IFeature) = fs.SelectByAttribute(Me.AttributeFilter)
                        fs = New FeatureSet(lFeatures)
                        Me.LogMessage(cStringUtils.Localize(My.Resources.OPERATION_EXTRACTPLOYGONS, Me.AttributeFilter), eStatusFlags.ValueComputed)
                    End If

                    ' Rasterize the features
                    rstResult = cVectorTools.Rasterize(fs, ptfTL, ptfBR, dCellSize, cCore.NULL_VALUE, strProjectionString, strFile, log, _
                                                       New cVectorTools.TranslateValueDelegate(AddressOf ToValue))
                    rstResult.Close()
                    Debug.Assert(rstResult IsNot Nothing)

                    Me.LogMessage(cStringUtils.Localize(My.Resources.STATUS_RASTER_CACHED, strFile), eStatusFlags.OK)

                Catch ex As Exception
                    Me.LogMessage(cStringUtils.Localize(My.Resources.STATUS_VECTORCONVERSION_EXCEPTION, ex.Message), eStatusFlags.ErrorEncountered)
                End Try
            Else
                ' Log error
                Me.LogMessage(My.Resources.STATUS_VALIDATIONFAILED_VECTORONLY, eStatusFlags.ErrorEncountered Or eStatusFlags.FailedValidation)
            End If
            Return New cSpatialRaster(rstResult)

        End Function

        ''' -----------------------------------------------------------------------
        ''' <inheritdocs cref="cSpatialDataConverter.DisplayName"/>
        ''' -----------------------------------------------------------------------
        Public Overrides ReadOnly Property DisplayName As String
            Get
                Return My.Resources.CONVERTER_DIRECTVECTOR_NAME
            End Get
        End Property

        ''' -----------------------------------------------------------------------
        ''' <inheritdocs cref="cSpatialDataConverter.Description"/>
        ''' -----------------------------------------------------------------------
        Public Overrides ReadOnly Property Description As String
            Get
                Return My.Resources.CONVERTER_DIRECTVECTOR_DESCR
            End Get
        End Property

        ''' -----------------------------------------------------------------------
        ''' <inheritdocs cref="cSpatialDataConverter.PluginName"/>
        ''' -----------------------------------------------------------------------
        Public Overrides ReadOnly Property PluginName As String
            Get
                Return "DotSpatial.DefaultVectorConverter"
            End Get
        End Property

    End Class

End Namespace
