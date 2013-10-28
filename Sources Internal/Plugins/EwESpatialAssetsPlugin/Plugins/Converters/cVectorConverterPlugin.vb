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
Imports DotSpatial.Analysis
Imports DotSpatial.Data
Imports EwECore
Imports EwEPlugin
Imports EwEUtils.Core
Imports EwEUtils.SpatialData
Imports EwEUtils.Utilities
Imports EwECore.SpatialData

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
        Implements ISpatialDataConverterPlugin

        ''' <summary>Filter for extracting features (vector set only).</summary>
        Private m_strAttributeFilter As String = ""
        ''' <summary>Name of attribute value to rasterize (vector set only).</summary>
        Private m_strAttributeName As String = ""
        Private m_dtAttributeValues As Dictionary(Of Object, Object) = Nothing

        Private m_core As cCore = Nothing

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
        Function IsConfigured() As Boolean _
            Implements EwEUtils.SpatialData.ISpatialDataConverter.IsConfigured
            Return Not String.IsNullOrWhiteSpace(Me.m_strAttributeName)
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
                Return Me.m_dtAttributeValues
            End Get
            Set(value As System.Collections.Generic.Dictionary(Of Object, Object))
                Me.m_dtAttributeValues = value
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
            Me.LogMessage(String.Format(My.Resources.STATUS_CONVERTER, Me.DisplayName), eStatusFlags.OK)

            ' Get Ecospace raster bounds
            Dim bnds As IRasterBounds = cDotSpatialUtils.EcospaceToBounds(ptfTL, ptfBR, dCellSize)
            Dim rstResult As IRaster = Nothing

            If (TypeOf data Is IRaster) Then
                  Me.LogMessage(My.Resources.STATUS_VALIDATIONFAILED_RASTERONLY, eStatusFlags.ErrorEncountered Or eStatusFlags.FailedValidation)
            ElseIf (TypeOf data Is IFeatureSet) Then
                Try

                    Dim fs As IFeatureSet = CType(data, IFeatureSet)

                    ' Is attribute filter specified?
                    If Not String.IsNullOrWhiteSpace(Me.m_strAttributeFilter) Then
                        ' #Yes: extract features that match the filter
                        Dim lFeatures As List(Of IFeature) = fs.SelectByAttribute(Me.m_strAttributeFilter)
                        fs = New FeatureSet(lFeatures)
                        Me.LogMessage(String.Format(My.Resources.OPERATION_EXTRACTPLOYGONS, Me.m_strAttributeFilter), eStatusFlags.ValueComputed)
                    End If

                    If Not fs.Projection.Equals(cDotSpatialUtils.EcospaceProjection) Then
                        fs.Reproject(cDotSpatialUtils.EcospaceProjection)
                        Me.LogMessage(String.Format(My.Resources.OPERATION_REPROJECT, fs.ProjectionString), eStatusFlags.ValueComputed)
                    End If

                    ' Rasterize the features
                    rstResult = cVectorTools.Rasterize(fs, ptfTL, ptfBR, dCellSize, m_strAttributeName, m_dtAttributeValues, strFile, log)
                    rstResult.Close()
                    Debug.Assert(rstResult IsNot Nothing)

                    Me.LogMessage(String.Format(My.Resources.STATUS_RASTER_CACHED, strFile), eStatusFlags.OK)

                Catch ex As Exception
                    Me.LogMessage(String.Format(My.Resources.STATUS_VECTORCONVERSION_EXCEPTION, ex.Message), eStatusFlags.ErrorEncountered)
                End Try

            End If
            Return New cSpatialRaster(rstResult)

        End Function

        ''' -----------------------------------------------------------------------
        ''' <inheritdocs cref="ISpatialDataConverter.DisplayName"/>
        ''' -----------------------------------------------------------------------
        Public Property DisplayName As String _
            Implements ISpatialDataConverter.DisplayName
            Get
                Return My.Resources.CONVERTER_DIRECTVECTOR_NAME
            End Get
            Set(value As String)
                ' NOP
            End Set
        End Property

        ''' -----------------------------------------------------------------------
        ''' <inheritdocs cref="ISpatialDataConverter.Description"/>
        ''' -----------------------------------------------------------------------
        Public Property Description As String _
            Implements ISpatialDataConverter.Description
            Get
                Return My.Resources.CONVERTER_DIRECTVECTOR_DESCR
            End Get
            Set(value As String)
                'NOP
            End Set
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
        ''' <inheritdocs cref="IPlugin.Description"/>
        ''' -----------------------------------------------------------------------
        Public ReadOnly Property PluginDescription As String _
            Implements EwEPlugin.IPlugin.Description
            Get
                Return Me.Description
            End Get
        End Property

        ''' -----------------------------------------------------------------------
        ''' <inheritdocs cref="IPlugin.Name"/>
        ''' -----------------------------------------------------------------------
        Public ReadOnly Property PlugingName As String _
            Implements EwEPlugin.IPlugin.Name
            Get
                Return "DotSpatial.DefaultVectorConverter"
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
