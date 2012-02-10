#Region " Imports "

Option Strict On
Imports System
Imports System.Drawing

#End Region ' Imports

Namespace SpatialData

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Interface for classes that cache converted spatio-temporal data.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Interface ISpatialDataCache

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the path to the cache root folder.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Property RootFolder As String

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Return the path to a cache for a dataset.
        ''' </summary>
        ''' <param name="ds"><see cref="ISpatialDataSet"/> to obtain the cache path for.</param>
        ''' <param name="ptfTL">Top-left location (in decimal degrees lon,lat) of the bounding box of the data.</param>
        ''' <param name="ptfBR">Bottom-right location (in decimal degrees lon,lat) of the bounding box of the data.</param>
        ''' <param name="dCellSize">Cell size to obtain the cache path for.</param>
        ''' <param name="time">Time to create the file name for.</param>
        ''' <param name="strExt">File extension tpo create the file name for.</param>
        ''' <returns>A cache path.</returns>
        ''' -------------------------------------------------------------------
        Function GetFileName(ds As ISpatialDataSet, _
                             ptfTL As PointF, ptfBR As PointF, dCellSize As Double, time As DateTime, _
                             Optional strExt As String = ".tif") As String
    End Interface

End Namespace

