#Region " Imports "

Option Strict On
Imports System.Drawing
Imports System.Xml

#End Region ' Imports

Namespace SpatialData

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Interface for implementing spatial data conversions.
    ''' </summary>
    ''' -------------------------------------------------------------------
    Public Interface ISpatialDataConverter

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Name for displaying the converter in a user interface.
        ''' </summary>
        ''' -------------------------------------------------------------------
        ReadOnly Property Name As String

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Description for displaying the converter in a user interface.
        ''' </summary>
        ''' -------------------------------------------------------------------
        ReadOnly Property Description As String

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Convert data at time step <paramref name="iTime"/> with a given
        ''' <paramref name="extent"/> and <paramref name="sCellSize">cell size</paramref>
        ''' into a <see cref="ISpatialRaster">raster</see>.
        ''' </summary>
        ''' <param name="data">Data to convert.</param>
        ''' <param name="ptfNE">North-east corner of the area to load data for. 
        ''' Values are interpreted as decimal degrees, <see cref="Point.X"/> as longitude, 
        ''' <see cref="Point.Y"/> as latiude.</param>
        ''' <param name="ptfSW">South-west corner of the area to load data for. 
        ''' Values are interpreted as decimal degrees, <see cref="Point.X"/> as longitude, 
        ''' <see cref="Point.Y"/> as latiude.</param>
        ''' <param name="dCellSize">Cell size (in decimal degrees) to convert data to.</param>
        ''' <param name="strFile">Name of the file to store the converted raster.</param>
        ''' <param name="strAttributeFilterQuery">Optional attribute filter query.</param>
        ''' <returns>A <see cref="ISpatialRaster">raster</see> with data, trimmed to the Ecospace 
        ''' bounding box indicated by <paramref name="ptfNE"/>, <paramref name="ptfSW"/> and 
        ''' <paramref name="dCellSize">cell size</paramref>.</returns>
        ''' -------------------------------------------------------------------
        Function Convert(ByVal data As Object, _
                         ByVal ptfNE As PointF, _
                         ByVal ptfSW As PointF, _
                         ByVal dCellSize As Double, _
                         ByVal strFile As String, _
                         Optional ByVal strAttributeFilterQuery As String = "") As ISpatialRaster

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the configuration information for the converter.
        ''' </summary>
        ''' <param name="doc"><see cref="XmlDocument"/> for creating and parsing nodes.</param>
        ''' -------------------------------------------------------------------
        Property Configuration(ByVal doc As XmlDocument) As XmlNode

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get whether the converter is configured and ready to operate.
        ''' </summary>
        ''' -------------------------------------------------------------------
        ReadOnly Property IsConfigured() As Boolean

    End Interface

End Namespace
