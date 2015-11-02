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
Imports System.IO
Imports System.Text
Imports System.Reflection
Imports DotSpatial.Controls
Imports DotSpatial.Data
Imports DotSpatial.Projections
Imports EwEUtils.SystemUtilities
Imports EwEUtils.Utilities
Imports DotSpatial.Topology
Imports System.Drawing
Imports EwEUtils.SpatialData
Imports EwESpatialAssetsPlugin.SpatialData
Imports EwEUtils.Core
Imports EwECore
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Miscellaneous utilities for working with the DotSpatial libraries.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cDotSpatialUtils

    ''' <summary>Threshold factor to determine if two values can be considered equal.</summary>
    Public Shared EQUALS_FACTOR As Double = 1 / 100

#Region " Singleton "

    Private Shared g_DotSpatialAppMan As AppManager = Nothing

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Initialize DotSpatial.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Shared Sub InitDotSpatial()

        ' ToDo_JS: place DotSpatial initialization in a separate thread, called as soon as the plug-in initializes instead of on first use

        If (cDotSpatialUtils.g_DotSpatialAppMan IsNot Nothing) Then Return

        Dim appman As New AppManager()
        Dim bitness As String = ""
        Dim strBasePath As String = Path.GetDirectoryName(Assembly.GetAssembly(GetType(cDotSpatialUtils)).Location)

        ' Build the path to the correct directory(s) for 32 and 64 bit binary dlls
        If cSystemUtils.Is64BitProcess() Then bitness = "win64" Else bitness = "win32"
        'Hardwire the "Includes/GDAL" path so that it matches the directory structure in the development environment
        Dim strGDALPath As String = Path.Combine(strBasePath, "Includes", "GDAL", bitness)
        Dim bGDALFound As Boolean = False

        appman.Directories.Clear()
        bGDALFound = Directory.Exists(strGDALPath)
        If (bGDALFound) Then
            'This tells DotSpatial which directories to explicitly search for Extentions
            appman.Directories.Add(strGDALPath)
        End If
        appman.LoadExtensions()

#If DEBUG Then
        ' Dump which file extensions are supported by the spatial framework
        Console.WriteLine("DotSpatial file support (expecting GDAL in '{0}', {1}):", strGDALPath, cSystemUtils.IIF(bGDALFound, "found", "missing"))
        For Each prov As IDataProvider In DataManager.DefaultDataManager.DataProviders
            Console.WriteLine(" - " & prov.Name & "::" & prov.GetType.ToString & "; " & prov.DialogReadFilter)
        Next
#End If
        cDotSpatialUtils.g_DotSpatialAppMan = appman

    End Sub

#End Region ' Singleton

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Returns the default extension for cached files. This extension must
    ''' be natively supported by DotSpatial.
    ''' </summary>
    ''' <returns>The default extension for cached files.</returns>
    ''' -------------------------------------------------------------------
    Public Shared Function DefaultCacheExtension() As String
        ' Binary Grids (.bgd files) are natively supported by DotSpatial
        Return ".bgd"
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Read a file from DotSpatial.
    ''' </summary>
    ''' <param name="strFileName">The file to open.</param>
    ''' <returns>A <see cref="IDataSet"/></returns>
    ''' <remarks>
    ''' <para>DotSpatial allows files to be loaded in a gazillion different ways, 
    ''' that eventually boil down to a call to a specific IDataProvider. This method
    ''' should be used as the one entry point to all DotSpatial-based file read
    ''' functionality because it allows us to insert specific logic.</para>
    ''' <para>For instance, GDAL rasters include a half-cell degree margin around
    ''' rasters, which have unwanted effects on spatial operations.</para>
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Public Shared Function OpenFile(ByVal strFileName As String) As IDataSet

        ' Just to make sure
        cDotSpatialUtils.InitDotSpatial()

        Dim man As IDataManager = DataManager.DefaultDataManager
        Dim ds As IDataSet = man.OpenFile(strFileName)

        If (TypeOf ds Is IRaster) Then
            CorrectBounds(DirectCast(ds, IRaster).Bounds)
        End If

        Return ds

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns the data format for a given file.
    ''' </summary>
    ''' <param name="strFileName"></param>
    ''' <returns>A data format.</returns>
    ''' -----------------------------------------------------------------------
    Public Shared Function GetDataFormat(ByVal strFileName As String) As String

        ' Just to make sure
        cDotSpatialUtils.InitDotSpatial()

        Dim man As IDataManager = DataManager.DefaultDataManager
        Select Case man.GetFileFormat(strFileName)
            Case DataFormat.Image
                Return "DotSpatialImage"
            Case DataFormat.Raster
                Return "DotSpatialRaster"
            Case DataFormat.Vector
                Return "DotSpatialVector"
            Case DataFormat.Custom
                ' NOP
        End Select
        Return ""

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Return the combined file extensions supported by installed DotSpatial data providers.
    ''' </summary>
    ''' <param name="bRead">Flag indicating whether the <see cref="IDataProvider.DialogReadFilter">read</see>
    ''' or <see cref="IDataProvider.DialogWriteFilter">write</see> filters need to be returned.</param>
    ''' <param name="bRaster">Flag indicating whether <see cref="IRasterProvider">rasters</see> must be supported.</param>
    ''' <param name="bImage">Flag indicating whether <see cref="IImageDataProvider">images</see> must be supported.</param>
    ''' <param name="bVector">Flag indicating whether <see cref="IVectorProvider">vectors</see> must be supported.</param>
    ''' <returns>A combined <see cref="System.Windows.Forms.FileDialog.Filter">dialog filter</see>.</returns>
    ''' -----------------------------------------------------------------------
    Public Shared Function DialogFilter(ByVal bRead As Boolean, _
                                        Optional ByVal bRaster As Boolean = True, _
                                        Optional ByVal bImage As Boolean = True, _
                                        Optional ByVal bVector As Boolean = True) As String

        ' Just to make sure
        cDotSpatialUtils.InitDotSpatial()

        Dim sb As New StringBuilder()
        Dim man As IDataManager = DataManager.DefaultDataManager
        Dim bUseProvider As Boolean = False
        Dim lFilters() As String = New String() {"", "", ""}
        Dim lFilterNames() As String = New String() {My.Resources.DIALOGFILTER_RASTER, My.Resources.DIALOGFILTER_IMAGE, My.Resources.DIALOGFILTER_VECTOR}

        For Each prov In man.DataProviders

            Dim astrFilter() As String = CStr(cSystemUtils.IIF(bRead, prov.DialogReadFilter, prov.DialogWriteFilter)).Split("|"c)

            If (astrFilter.Length = 2) Then
                If (TypeOf prov Is IRasterProvider) And bRaster Then lFilters(0) &= (";" & astrFilter(1))
                If (TypeOf prov Is IImageDataProvider) And bImage Then lFilters(1) &= (";" & astrFilter(1))
                If (TypeOf prov Is IVectorProvider) And bVector Then lFilters(2) &= (";" & astrFilter(1))
            End If
        Next

        ' Concoct total
        For i As Integer = 0 To 2
            If Not String.IsNullOrEmpty(lFilters(i)) Then
                If sb.Length > 0 Then sb.Append("|"c)
                sb.Append(lFilterNames(i))
                sb.Append("|")
                sb.Append(cFileUtils.CleanupExtensions(lFilters(i)))
            End If
        Next

        Return sb.ToString

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Check whether two <see cref="IExtent">spatial extents</see> can be considered 
    ''' equal, which requires all bounds to fall within a given threshold.
    ''' </summary>
    ''' <param name="ext1">The first <see cref="IExtent"/> to compare.</param>
    ''' <param name="ext2">The second <see cref="IExtent"/> to compare.</param>
    ''' <param name="dThreshold">Max threshold difference for the extent bounds.</param>
    ''' <returns>True if the extents can be considered equal.</returns>
    ''' -----------------------------------------------------------------------
    Public Shared Function Approximates(ext1 As IExtent, ext2 As IExtent, dThreshold As Double) As Boolean

        Return cNumberUtils.Approximates(ext1.MinX, ext2.MinX, dThreshold) And _
               cNumberUtils.Approximates(ext1.MaxX, ext2.MaxX, dThreshold) And _
               cNumberUtils.Approximates(ext1.MinY, ext2.MinY, dThreshold) And _
               cNumberUtils.Approximates(ext1.MaxY, ext2.MaxY, dThreshold)

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Check whether two <see cref="IRasterBounds">raster bounds</see> can be considered 
    ''' equal, which requires all bounds to fall within a given threshold.
    ''' </summary>
    ''' <param name="bnds1">The first <see cref="IRasterBounds"/> to compare.</param>
    ''' <param name="bnds2">The second <see cref="IRasterBounds"/> to compare.</param>
    ''' <param name="dThreshold">Max threshold difference for the bounds.</param>
    ''' <returns>True if the extents can be considered equal.</returns>
    ''' -----------------------------------------------------------------------
    Public Shared Function Approximates(ByVal bnds1 As IRasterBounds, ByVal bnds2 As IRasterBounds, ByVal dThreshold As Double) As Boolean

        Return cDotSpatialUtils.Approximates(bnds1.Extent, bnds2.Extent, dThreshold)

    End Function

    Public Shared Function Extent(ptfTL As PointF, ptfBR As PointF) As Extent
        Return New Extent(ptfTL.X, ptfBR.Y, ptfBR.X, ptfTL.Y)
    End Function

    Public Shared Function TopLeft(ext As Extent) As PointF
        Return New PointF(CSng(ext.MinX), CSng(ext.MaxY))
    End Function

    Public Shared Function BottomRight(ext As Extent) As PointF
        Return New PointF(CSng(ext.MaxX), CSng(ext.MinY))
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Convert an Ecospace lat/lon area to <see cref="IRasterBounds"/>.
    ''' </summary>
    ''' <param name="ptfTL">The top-left location (in decimal decrees) of the Ecospace area.</param>
    ''' <param name="ptfBR">The bottom-right location (in decimal decrees) of the Ecospace area.</param>
    ''' <param name="dCellSize">The cells size to take into account.</param>
    ''' <returns>A valid <see cref="IRasterBounds"/> instance.</returns>
    ''' -----------------------------------------------------------------------
    Public Shared Function EcospaceToBounds(ByVal ptfTL As PointF, ByVal ptfBR As PointF, ByVal dCellSize As Double) As IRasterBounds

        Dim iNumRows As Integer = CInt(Math.Ceiling((ptfTL.Y - ptfBR.Y) / dCellSize - (dCellSize * cDotSpatialUtils.EQUALS_FACTOR)))
        Dim iNumCols As Integer = CInt(Math.Ceiling((ptfBR.X - ptfTL.X) / dCellSize - (dCellSize * cDotSpatialUtils.EQUALS_FACTOR)))
        Dim ext As New Extent(ptfTL.X, ptfTL.Y - iNumRows * dCellSize, ptfTL.X + dCellSize * iNumCols, ptfTL.Y)
        Dim bounds As New RasterBounds(iNumRows, iNumCols, ext)

        CorrectBounds(bounds)

        ' Sanity checks
        Debug.Assert(cDotSpatialUtils.Approximates(ext, bounds.Extent, dCellSize * cDotSpatialUtils.EQUALS_FACTOR))
        Debug.Assert(cNumberUtils.Approximates(bounds.CellWidth, dCellSize, cDotSpatialUtils.EQUALS_FACTOR * dCellSize))
        Debug.Assert(cNumberUtils.Approximates(bounds.CellHeight, dCellSize, cDotSpatialUtils.EQUALS_FACTOR * dCellSize))

        Return bounds

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Fix for unresolved error bounds: http://dotspatial.codeplex.com/discussions/294853
    ''' </summary>
    ''' <param name="bnds"></param>
    ''' -----------------------------------------------------------------------
    <Obsolete("Get rid of this horrendous fix as soon as DotSpatial RasterBounds behave properly")> _
    Private Shared Sub CorrectBounds(ByVal bnds As IRasterBounds)
        ' This is awful
        bnds.X += bnds.CellWidth * 0.5
        bnds.Y -= bnds.CellHeight * 0.5
    End Sub

    Public Shared Function CreateRaster(ptfTL As PointF, ptfBR As PointF, dCellSize As Double,
                                        datatype As Type, dNoDataValue As Double, strFile As String, _
                                        Optional ByRef iNumRow As Integer = Nothing, _
                                        Optional ByRef iNumCol As Integer = Nothing) As IRaster

        If (iNumRow = Nothing) Then iNumRow = New Integer
        If (iNumCol = Nothing) Then iNumCol = New Integer

        iNumRow = Convert.ToInt32(Math.Abs(Math.Round((ptfTL.Y - ptfBR.Y) / dCellSize)))
        iNumCol = Convert.ToInt32(Math.Abs(Math.Round((ptfBR.X - ptfTL.X) / dCellSize)))

        Dim output As IRaster = Raster.CreateRaster(strFile, String.Empty, iNumCol, iNumRow, 1, datatype, New String() {})
        output.Bounds = cDotSpatialUtils.EcospaceToBounds(ptfTL, ptfBR, dCellSize)
        output.NoDataValue = dNoDataValue

        For iRow As Integer = output.StartRow To output.EndRow
            For iCol As Integer = output.StartColumn To output.EndColumn
                output.Value(iRow, iCol) = dNoDataValue
            Next iCol
        Next iRow

        Return output

    End Function

#Region " Formatting "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Format the extent of DotSpatial <see cref="IRasterBounds"/> to a string.
    ''' </summary>
    ''' <param name="bnds">The <see cref="IRasterBounds"/> to format.</param>
    ''' <returns>A formatted string.</returns>
    ''' -----------------------------------------------------------------------
    Public Shared Function FormatExtent(bnds As IRasterBounds) As String
        Return cDotSpatialUtils.FormatExtent(bnds.Extent)
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Format a DotSpatial <see cref="IExtent"/> to a string.
    ''' </summary>
    ''' <param name="ext">The <see cref="IExtent"/> to format.</param>
    ''' <returns>A formatted string.</returns>
    ''' -----------------------------------------------------------------------
    Public Shared Function FormatExtent(ext As IExtent) As String
        Return String.Format(My.Resources.FORMAT_BOUNDS, ext.MinX, ext.MaxY, ext.MaxX, ext.MinY)
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Format grid information (#cols, #rows, cell size) of a DotSpatial 
    ''' <see cref="IRaster"/> to a string.
    ''' </summary>
    ''' <param name="rs">The <see cref="IRaster"/> to format.</param>
    ''' <returns>A formatted string.</returns>
    ''' -----------------------------------------------------------------------
    Public Shared Function FormatRasterGrid(rs As IRaster) As String
        Return String.Format(My.Resources.FORMAT_GRID, rs.NumColumns, rs.NumRows, cStringUtils.FormatDegrees(rs.CellWidth))
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Format statistics (min, max, mean, ...?) of a DotSpatial 
    ''' <see cref="IRaster"/> to a string.
    ''' </summary>
    ''' <param name="rs">The <see cref="IRaster"/> to format.</param>
    ''' <returns>A formatted string.</returns>
    ''' -----------------------------------------------------------------------
    Public Shared Function FormatRasterStats(rs As IRaster) As String
        Return cDotSpatialUtils.FormatRasterStats(New cSpatialRaster(rs))
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Format statistics (min, max, mean, ...?) of a <see cref="cSpatialRaster"/>
    ''' to a string.
    ''' </summary>
    ''' <param name="rs">The <see cref="cSpatialRaster"/> to format.</param>
    ''' <returns>A formatted string.</returns>
    ''' -----------------------------------------------------------------------
    Public Shared Function FormatRasterStats(rs As cSpatialRaster) As String
        Return String.Format(My.Resources.FORMAT_STATS, rs.Min, rs.Max, rs.Mean)
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Format a <see cref="IRaster"/> to a string.
    ''' </summary>
    ''' <param name="rs">The <see cref="IRaster"/> to format.</param>
    ''' <returns>A formatted string.</returns>
    ''' -----------------------------------------------------------------------
    Public Shared Function FormatRaster(rs As IRaster) As String
        Return cDotSpatialUtils.FormatRaster(New cSpatialRaster(rs))
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Format a <see cref="cSpatialRaster"/> to a string.
    ''' </summary>
    ''' <param name="rs">The <see cref="cSpatialRaster"/> to format.</param>
    ''' <returns>A formatted string.</returns>
    ''' -----------------------------------------------------------------------
    Public Shared Function FormatRaster(rs As cSpatialRaster) As String
        Return String.Format(My.Resources.FORMAT_RASTER, _
                             cDotSpatialUtils.FormatExtent(rs.Ext), _
                             cDotSpatialUtils.FormatRasterGrid(rs.Raster), _
                             rs.ProjectionString, _
                             cDotSpatialUtils.FormatRasterStats(rs))
    End Function

#End Region ' Formatting

#Region " Resampling "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Resample a raster to a new extent and cell size.
    ''' </summary>
    ''' <param name="rsSource"></param>
    ''' <param name="ptfTL"></param>
    ''' <param name="ptfBR"></param>
    ''' <param name="dCellSize"></param>
    ''' <param name="strFile"></param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Public Shared Function ResampleToEcospace(rsSource As IRaster, _
                                              ptfTL As PointF, ptfBR As PointF, dCellSize As Double, _
                                              strFile As String) As IRaster

        If (rsSource Is Nothing) Then Return Nothing

        Dim iNumRow, iNumCol As Integer
        Dim output As IRaster = cDotSpatialUtils.CreateRaster(ptfTL, ptfBR, dCellSize, rsSource.DataType, rsSource.NoDataValue, strFile, iNumRow, iNumCol)
        Dim cellCenter As Coordinate = Nothing
        Dim index1 As RcIndex = Nothing
        Dim val As Double = 0

        'Loop through every cell in the output map
        For iRow As Integer = 0 To iNumRow - 1
            For iCol As Integer = 0 To iNumCol - 1

                ' Project output cell position to coordinate
                cellCenter = output.CellToProj(iRow, iCol)
                index1 = rsSource.ProjToCell(cellCenter)

                If (-1 < index1.Row) And (index1.Row <= rsSource.EndRow) And (-1 < index1.Column) And (index1.Column <= rsSource.EndColumn) Then
                    val = rsSource.Value(index1.Row, index1.Column)
                Else
                    val = output.NoDataValue
                End If
                output.Value(iRow, iCol) = val
            Next
        Next

        ' Yippee
        output.Save()
        output.Close()

        Return output

    End Function

#End Region ' Resample

#Region " Projections "

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Returns a projection from a projection string. If the provided projection
    ''' string is empty, the <seealso cref="cEcospaceDataStructures.DEFAULT_COORDINATESYSTEM">default Ecospace projection</seealso>
    ''' is assumed.
    ''' </summary>
    ''' <returns>A projection for the EwE model.</returns>
    ''' -------------------------------------------------------------------
    Friend Shared Function ToProjection(strProjectionString As String) As ProjectionInfo
        If (String.IsNullOrWhiteSpace(strProjectionString)) Then strProjectionString = cEcospaceDataStructures.DEFAULT_COORDINATESYSTEM
        Return ProjectionInfo.FromEsriString(strProjectionString)
    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Returns a projection string for a given projection. If the provided projection
    ''' is missing, the <seealso cref="cEcospaceDataStructures.DEFAULT_COORDINATESYSTEM">default Ecospace projection</seealso>
    ''' is returned.
    ''' </summary>
    ''' <returns>A projection string for a projection.</returns>
    ''' -------------------------------------------------------------------
    Friend Shared Function ToProjectionString(info As ProjectionInfo) As String
        If (info Is Nothing) Then Return cEcospaceDataStructures.DEFAULT_COORDINATESYSTEM
        Return info.ToEsriString()
    End Function

    Public Shared Sub GetProjectionInfo(ByVal strProj As String, ByRef strName As String, ByRef bIsLatLon As Boolean, ByRef strUnit As String)

        Dim proj As ProjectionInfo = cDotSpatialUtils.ToProjection(strProj)
        strName = proj.Name
        bIsLatLon = proj.IsLatLon() Or proj.IsGeocentric()
        If (bIsLatLon) Then
            strUnit = SharedResources.UNIT_DECIMALDEGREE
        Else
            Select Case proj.Unit.Name.ToLower()
                Case "meter", "meters" : strUnit = SharedResources.UNIT_METER
                Case "kilometer", "kilometers" : strUnit = SharedResources.UNIT_KILOMETER
                Case Else : strUnit = "?"
            End Select
        End If
    End Sub

#End Region ' Projections

#Region " Feature extraction "

    Friend Shared Function FeatureSet(fs As IFeatureSet, strFilter As String) As IFeatureSet

        If (fs Is Nothing) Then Return fs
        If (String.IsNullOrWhiteSpace(strFilter)) Then Return fs

        Dim features As New List(Of IFeature)
        Try
            If Not fs.AttributesPopulated Then fs.FillAttributes()

            ' Bug fix for http://dotspatial.codeplex.com/workitem/25308
            If (fs.FeatureLookup.Count <> fs.Features.Count) Then
                fs.FeatureLookup.Clear()

                For i As Integer = 0 To fs.Features.Count - 1
                    Dim f As IFeature = fs.Features(i)
                    If (f.DataRow IsNot Nothing) Then
                        fs.FeatureLookup.Add(f.DataRow, f)
                    End If
                Next
            End If

            features = fs.SelectByAttribute(strFilter)
        Catch ex As Exception
            ' Whaoh!
            Debug.Assert(False, ex.Message)
        End Try

        Dim fsNew As New FeatureSet(features)
        fsNew.Projection = fs.Projection
        Return fsNew

    End Function

#End Region ' Feature extraction

End Class
