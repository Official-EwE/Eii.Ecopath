' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.Drawing
Imports System.Xml

Namespace Common

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Interface for implementing spatial data conversions.
    ''' </summary>
    ''' -------------------------------------------------------------------
    Public Interface ISpatialDataConverter
        Inherits ISummarizable

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the dataset to link to this converter.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Property Dataset As ISpatialDataSet

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the name for displaying the converter in a user interface.
        ''' </summary>
        ''' -------------------------------------------------------------------
        ReadOnly Property DisplayName As String

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the description for displaying the converter in a user interface.
        ''' </summary>
        ''' -------------------------------------------------------------------
        ReadOnly Property Description As String

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set an attribute filter, if needed.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Property AttributeFilter As String

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the name of the attribute to rasterize, if needed.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Property AttributeName As String

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Optional mappings for rasterizing features
        ''' </summary>
        ''' -------------------------------------------------------------------
        ReadOnly Property AttributeValueMappings() As Dictionary(Of Object, Object)

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Convert data with a given extent and cell size into a <see cref="ISpatialRaster">raster</see>.
        ''' </summary>
        ''' <param name="data">Data to convert.</param>
        ''' <param name="ptfNE">North-east corner of the area to load data for. 
        ''' Values are interpreted as decimal degrees, <see cref="Point.X"/> as longitude, 
        ''' <see cref="Point.Y"/> as latiude.</param>
        ''' <param name="ptfSW">South-west corner of the area to load data for. 
        ''' Values are interpreted as decimal degrees, <see cref="Point.X"/> as longitude, 
        ''' <see cref="Point.Y"/> as latiude.</param>
        ''' <param name="dCellSize">Cell size (in decimal degrees) to convert data to.</param>
        ''' <param name="strProjToWkt">The target WKT projection string.</param>
        ''' <param name="strFile">Name of the file to store the converted raster.</param>
        ''' <returns>A <see cref="ISpatialRaster">raster</see> with data, trimmed to the Ecospace 
        ''' bounding box indicated by <paramref name="ptfNE"/>, <paramref name="ptfSW"/> and 
        ''' <paramref name="dCellSize">cell size</paramref>.</returns>
        ''' -------------------------------------------------------------------
        Function Convert(data As Object,
                         ptfNE As PointF,
                         ptfSW As PointF,
                         dCellSize As Double,
                         strProjToWkt As String,
                         strFile As String) As ISpatialRaster

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the configuration information for the converter.
        ''' </summary>
        ''' <param name="doc"><see cref="XmlDocument"/> for creating and parsing nodes.</param>
        ''' -------------------------------------------------------------------
        Property Configuration(doc As XmlDocument) As XmlNode

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns whether the converter is configured and ready to operate.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Function IsConfigured() As Boolean

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns whether the converter is compatible with the data provided 
        ''' by a given dataset.
        ''' </summary>
        ''' <param name="ds">The dataset to the data.</param>
        ''' <returns>True if compatible.</returns>
        ''' -------------------------------------------------------------------
        Function IsCompatible(ds As ISpatialDataSet) As Boolean

    End Interface

End Namespace
