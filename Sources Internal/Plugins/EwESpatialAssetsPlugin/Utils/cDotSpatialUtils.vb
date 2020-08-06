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
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

' A note about licensing in EwE 6.6.2 and newer
' =============================================
'
' Licenses are managed in the EwE core through Treek's licensing server, and users
' activate and deactivate licenses through the scientific interface. To prevent
' sabotage, this cannot be overridden in either the core or ScInt; and neither
' can the spatial temporal data framework issue temporal licenses EwE-wide.
' 
' However, compiler directive USE_LICENSE_LIB can be set to 0 to stop the STDF
' from checking the core license, and use its internal expiration scheme. This is
' necessary when EwE is used in development contracts without a proper license.
'
' The USE_LICENSE_LIB flag is ONLY checked in release mode; the STDF will NOT
' check the core license in DEBUG mode.

#Const USE_LICENSE_LIB = 1

#If Not DEBUG Then
#Const USE_LICENSE_LIB = 1
#End If

#Region " Imports "

Option Strict On
Imports System.Collections.Generic
Imports System.Drawing
Imports System.IO
Imports System.Reflection
Imports System.Text
Imports DotSpatial.Controls
Imports DotSpatial.Data
Imports DotSpatial.Projections
Imports DotSpatial.Topology
Imports EwECore
Imports EwESpatialAssetsPlugin.SpatialData
Imports EwEUtils.Core
Imports EwEUtils.SystemUtilities
Imports EwEUtils.Utilities
Imports ScientificInterfaceShared.Controls
Imports Microsoft.VisualBasic
Imports ScientificInterfaceShared.Commands

#If USE_LICENSE_LIB = 1 Then
' Do not remove this reference, it's needed when
Imports TreeksLicensingLibrary2.EasyIntegration
#End If

#End Region ' Imports

' When not using Treek's licensing library, license conditions are embedded in this file and 
' must be changed in the fields below
#If USE_LICENSE_LIB = 0 Then

<HideModuleName()>
Friend Module modLicense

    ' To use the STDF for evaluation purposes without a core license, change the two fields below. 
    ' The License_eval_days is an offset to the STDF compilation date
    Public Const IsEvaluation As Boolean = False
    Public Const License_eval_days As Integer = 120

    ' To use the STDF without a core license, change the four fields below
    Public Const License_start_year As Integer = 2020
    Public Const License_start_month As Integer = 4
    Public Const License_start_day As Integer = 15
    Public Const License_years As Integer = 1

End Module

#End If

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

    Friend Shared Property UIContext As cUIContext

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Initialize DotSpatial.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Shared Sub InitDotSpatial()

        If (cDotSpatialUtils.g_DotSpatialAppMan IsNot Nothing) Then Return

        Dim appman As New AppManager()
        Dim bitness As String = ""

        ' JS 21Mar19: DotSpatial may reside in plug-ins path, while GDAL is installed in includes\64bit
        Dim strBasePath As String = Path.GetDirectoryName(Assembly.GetAssembly(GetType(cCore)).Location)

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

        ' Always log this
        cLog.Write(String.Format("DotSpatial file support (expecting GDAL in '{0}', {1}):", strGDALPath, If(bGDALFound, "found", "missing")), eVerboseLevel.Standard)
        For Each prov As IDataProvider In DataManager.DefaultDataManager.DataProviders
            cLog.Write(String.Format("DotSpatial loaded provider " & prov.Name & "::" & prov.GetType.ToString & "; " & prov.DialogReadFilter), eVerboseLevel.Standard)
        Next
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
    Public Shared Function DialogFilter(ByVal bRead As Boolean,
                                        Optional ByVal bRaster As Boolean = True,
                                        Optional ByVal bImage As Boolean = True,
                                        Optional ByVal bVector As Boolean = True,
                                        Optional ByVal bAllFiles As Boolean = True) As String

        ' Just to make sure
        cDotSpatialUtils.InitDotSpatial()

        Dim sb As New StringBuilder()
        Dim man As IDataManager = DataManager.DefaultDataManager
        Dim bUseProvider As Boolean = False
        Dim lFilters() As String = New String() {"", "", ""}
        Dim lFilterNames() As String = New String() {My.Resources.DIALOGFILTER_RASTER, My.Resources.DIALOGFILTER_IMAGE, My.Resources.DIALOGFILTER_VECTOR}

        For Each prov In man.DataProviders
            Dim astrFilter() As String = If(bRead, prov.DialogReadFilter, prov.DialogWriteFilter).Split("|"c)
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

        If bAllFiles Then
            sb.Append("|")
            sb.Append(ScientificInterfaceShared.My.Resources.FILEFILTER_ALL)
        End If

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

        Return cNumberUtils.Approximates(ext1.MinX, ext2.MinX, dThreshold) And
               cNumberUtils.Approximates(ext1.MaxX, ext2.MaxX, dThreshold) And
               cNumberUtils.Approximates(ext1.MinY, ext2.MinY, dThreshold) And
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
    Public Shared Function EcospaceToBounds(ByVal ptfTL As PointF, ByVal ptfBR As PointF, ByVal dCellSize As Double, ByVal proj As ProjectionInfo) As IRasterBounds

        Dim iNumRows As Integer = 0
        Dim iNumCols As Integer = 0

        If (proj Is Nothing) Then
            proj = ProjectionInfo.FromProj4String(cEcospaceDataStructures.DEFAULT_COORDINATESYSTEM)
        End If

        If proj.IsLatLon Then
            iNumRows = CInt(Math.Ceiling((ptfTL.Y - ptfBR.Y) / dCellSize - (dCellSize * cDotSpatialUtils.EQUALS_FACTOR)))
            iNumCols = CInt(Math.Ceiling((ptfBR.X - ptfTL.X) / dCellSize - (dCellSize * cDotSpatialUtils.EQUALS_FACTOR)))
        Else
            iNumRows = CInt(Math.Ceiling((ptfTL.Y - ptfBR.Y) / dCellSize))
            iNumCols = CInt(Math.Ceiling((ptfBR.X - ptfTL.X) / dCellSize))
        End If
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
    <Obsolete("Get rid of this horrendous fix as soon as DotSpatial RasterBounds behave properly")>
    Private Shared Sub CorrectBounds(ByVal bnds As IRasterBounds)
        ' This is awful
        bnds.X += bnds.CellWidth * 0.5
        bnds.Y -= bnds.CellHeight * 0.5
    End Sub

    Public Shared Function CreateRaster(ptfTL As PointF, ptfBR As PointF, dCellSize As Double,
                                        datatype As Type, dNoDataValue As Double, strFile As String,
                                        Optional ByRef iNumRow As Integer = Nothing,
                                        Optional ByRef iNumCol As Integer = Nothing,
                                        Optional ByVal proj As ProjectionInfo = Nothing) As IRaster

        If (iNumRow = Nothing) Then iNumRow = New Integer
        If (iNumCol = Nothing) Then iNumCol = New Integer

        iNumRow = Convert.ToInt32(Math.Abs(Math.Round((ptfTL.Y - ptfBR.Y) / dCellSize)))
        iNumCol = Convert.ToInt32(Math.Abs(Math.Round((ptfBR.X - ptfTL.X) / dCellSize)))

        Dim output As IRaster = Raster.CreateRaster(strFile, String.Empty, iNumCol, iNumRow, 1, datatype, New String() {})
        output.Bounds = cDotSpatialUtils.EcospaceToBounds(ptfTL, ptfBR, dCellSize, proj)
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
        Return String.Format(My.Resources.FORMAT_RASTER,
                             cDotSpatialUtils.FormatExtent(rs.Ext),
                             cDotSpatialUtils.FormatRasterGrid(rs.Raster),
                             rs.ProjectionString,
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
    Public Shared Function ResampleToEcospace(rsSource As IRaster,
                                              ptfTL As PointF, ptfBR As PointF, dCellSize As Double,
                                              strFile As String) As IRaster

        If (rsSource Is Nothing) Then Return Nothing

        Dim iNumRow, iNumCol As Integer
        Dim output As IRaster = cDotSpatialUtils.CreateRaster(ptfTL, ptfBR, dCellSize, rsSource.DataType, rsSource.NoDataValue, strFile, iNumRow, iNumCol, rsSource.Projection)
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
        Dim fmt As New EwECore.Style.cMapUnitFormatter()

        strName = proj.Name
        bIsLatLon = proj.IsLatLon() Or proj.IsGeocentric()
        If (bIsLatLon) Then
            strUnit = fmt.ToString(EwEUtils.Core.eUnitMapRefType.dd)
        Else
            Select Case proj.Unit.Name.ToLower()
                Case "meter", "meters" : strUnit = fmt.ToString(EwEUtils.Core.eUnitMapRefType.dd)
                Case "kilometer", "kilometers" : strUnit = fmt.ToString(EwEUtils.Core.eUnitMapRefType.km)
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

#Region " License "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Determines whether the specified core is licensed.
    ''' </summary>
    ''' <param name="core">The core.</param>
    ''' <returns>
    '''   <c>true</c> if the specified core is licensed; otherwise, <c>false</c>.
    ''' </returns>
    ''' -----------------------------------------------------------------------
    Public Shared Function IsLicensed(core As cCore) As Boolean

#If DEBUG Then
        Return True
#End If

        Debug.Assert(core IsNot Nothing)
        Dim bValid As Boolean = False

#If USE_LICENSE_LIB = 1 Then

        Try
            ' Validate if the core license class has not been tampered with
            If Not cDotSpatialUtils.TreekValid(core) Then
                Dim msg As New cFeedbackMessage(My.Resources.LICENSE_INVALID, eCoreComponentType.External, eMessageType.DataExport, eMessageImportance.Warning, eMessageReplyStyle.OK)
                core.Messages.SendMessage(msg)
                Return False
            End If

            Dim lic As EwELicense.cLicense = core.License

            ' ToDo: globalize this
            cApplicationStatusNotifier.StartProgress(core, "Checking EwE Pro license")
            Try
                bValid = lic.IsLicensed()
            Catch ex As Exception
                cLog.Write(ex, "cDotSpatialUtils.IsLicensed")
            End Try
            cApplicationStatusNotifier.EndProgress(core)

        Catch ex As Exception
            cLog.Write(ex, "cDotSpatialUtils.IsLicensed")
            Return False
        End Try
#Else
        bValid = (cDateUtils.StartTime < cDotSpatialUtils.ExpiryDate(core))
#End If
        If (bValid = False) Then
            Dim msg As New cFeedbackMessage(My.Resources.LICENSE_NONE, eCoreComponentType.External, eMessageType.DataExport, eMessageImportance.Warning, eMessageReplyStyle.OK_CANCEL)

            ' ToDo: globalize this
            msg.CustomReplyLabel(eMessageReply.OK) = "Enter license"
            msg.Hyperlink = "command:" & ScientificInterfaceShared.Commands.cEnterLicenseCommand.cCOMMAND_NAME
            core.Messages.SendMessage(msg)

            If (msg.Reply = eMessageReply.OK) Then
                Dim cmd As cEnterLicenseCommand = CType(UIContext.CommandHandler.GetCommand(cEnterLicenseCommand.cCOMMAND_NAME), cEnterLicenseCommand)
                cmd.Invoke()
            End If

        End If
        Return bValid

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Gets the expiry date of the STDF.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Shared ReadOnly Property ExpiryDate(core As cCore) As DateTime
        Get
#If USE_LICENSE_LIB = 1 Then
            If Not cDotSpatialUtils.TreekValid(core) Then Return Date.MinValue
            Return core.License.Expiry

            '' Potential paranoia-fix: bypass the cLicense class that can have been messed with
            'Dim treekCore As TLLInterface = core.License.Treek
            'Dim lic As TreeksLicensingLibrary2.License = treekCore.MyLicense
            'Return lic.ExpirationDate
#Else
            If (IsEvaluation) Then
                Return cAssemblyUtils.GetCompileDate(System.Reflection.Assembly.GetAssembly(GetType(cLifespanPlugin))).AddDays(License_eval_days)
            Else
                Return New DateTime(License_start_year + License_years, License_start_month, License_start_day)
            End If
#End If
        End Get
    End Property

#If USE_LICENSE_LIB = 1 Then

    Private Shared g_treekvalidated As Boolean = False
    Private Shared g_treekvalid As Boolean = False

    Private Shared Function TreekValid(core As cCore) As Boolean

        If (Not g_treekvalidated) Then
            Dim lic As EwELicense.cLicense = core.License
            Dim treekCore As TLLInterface = lic.Treek
            Dim chunk() As Byte = {59, 73, 118, 210, 3, 141, 246, 216, 157, 52, 18, 46, 129, 246, 84, 250, 141, 184, 105, 155, 123, 106, 106, 237, 134, 198, 199, 63, 12, 250, 237, 48, 92, 17, 15, 115, 96, 5, 81, 217, 53, 56, 89, 10, 52, 245, 208, 26, 101, 43, 196, 158, 128, 116, 218, 190, 28, 2, 127, 100, 2, 185, 194, 34, 221, 197, 122, 197, 222, 40, 180, 241, 204, 104, 17, 31, 199, 104, 252, 86, 57, 125, 185, 228, 49, 68, 134, 60, 255, 129, 156, 253, 184, 226, 190, 6, 223, 212, 253, 230, 202, 194, 49, 208, 76, 103, 245, 13, 219, 32, 31, 38, 210, 48, 105, 207, 155, 127, 223, 113, 227, 64, 201, 104, 52, 152, 58, 227, 114, 71, 87, 246, 222, 120, 111, 113, 12, 244, 17, 82, 65, 131, 132, 217, 117, 163, 141, 82, 67, 249, 48, 96, 96, 189, 248, 58, 107, 254, 229, 219, 182, 24, 226, 189, 199, 5, 175, 83, 31, 236, 0, 177, 185, 60, 204, 174, 227, 204, 168, 228, 112, 166, 117, 92, 143, 183, 0, 144, 34, 6, 7, 152, 184, 229, 213, 51, 8, 168, 113, 110, 82, 176, 205, 181, 10, 161, 98, 83, 227, 143, 186, 95, 153, 27, 25, 115, 196, 104, 145, 104, 99, 167, 138, 150, 45, 109, 2, 99, 3, 35, 3, 174, 140, 105, 156, 4, 24, 163, 188, 168, 217, 129, 199, 199, 237, 203, 99, 99, 88, 92, 241, 189, 242, 210, 80, 47, 165, 235, 152, 223, 55, 23, 16, 16, 100, 184, 166, 6, 36, 251, 224, 150, 57, 94, 155, 66, 150, 70, 162, 34, 211, 226, 75, 5, 21, 157, 242, 181, 45, 111, 155, 3, 166, 188, 32, 145, 215, 217, 184, 226, 70, 127, 86, 187, 253, 253, 79, 226, 25, 64, 209, 24, 62, 14, 46, 89, 211, 105, 112, 10, 33, 65, 207, 88, 131, 14, 6, 133, 216, 67, 54, 253, 92, 171, 250, 48, 66, 7, 50, 32, 167, 192, 132, 146, 67, 214, 229, 196, 31, 105, 199, 166, 201, 177, 13, 125, 142, 186, 114, 22, 82, 63, 120, 10, 172, 98, 87, 87, 12, 240, 220, 160, 194, 150, 248, 95, 129, 103, 123, 113, 110, 234, 100, 134, 205, 217, 122, 72, 243, 42, 66, 198, 102, 191, 115, 63, 220, 193, 67, 87, 147, 236, 223, 109, 156, 42, 150, 182, 49, 254, 70, 72, 189, 244, 157, 250, 160, 156, 64, 21, 83, 41, 235, 84, 0, 172, 95, 81, 166, 173, 213, 17, 93, 254, 144, 140, 81, 84, 194, 98, 85, 211, 150, 242, 109, 127, 75, 172, 23, 233, 41, 219, 181, 100, 0, 72, 126, 2, 204, 134, 36, 93, 215, 173, 90, 241, 72, 180, 202, 113, 240, 173, 213, 83, 178, 248, 196, 169, 166, 136, 159, 48, 90, 106, 35, 75, 106, 134, 136, 186, 14, 109, 37, 217, 11, 80, 195, 126, 65, 96, 159, 132, 208, 157, 194, 231, 254, 93, 83, 81, 122, 22, 117, 155, 39, 155, 35, 106, 120, 6, 114, 185, 224, 21, 151, 167, 67, 61, 214, 198, 166, 232, 15, 91, 96, 159, 204, 85, 132, 114, 174, 166, 105, 245, 62, 148, 253, 158, 19, 99, 72, 170, 47, 4, 246, 95, 118, 202, 135, 18, 188, 237, 173, 191, 117, 152, 248, 150, 127, 222, 170, 104, 211, 213, 229, 46, 68, 241, 103, 84, 70, 239, 170, 95, 66, 211, 37, 120, 91, 143, 141, 191, 107, 175, 26, 227, 137, 71, 96, 57, 213, 242, 107, 98, 188, 140, 95, 83, 138, 15, 183, 15, 227, 137, 225, 210, 116, 197, 177, 243, 26, 117, 217, 115, 8, 88, 218, 75, 1, 152, 153, 21, 71, 206, 81, 48, 100, 243, 185, 231, 195, 107, 69, 147, 84, 67, 10, 109, 171, 166, 59, 172, 96, 11, 159, 235, 63, 123, 6, 86, 248, 178, 139, 31, 144, 83, 157, 56, 105, 160, 200, 153, 241, 104, 115, 104, 161, 209, 36, 218, 33, 248, 213, 56, 111, 221, 68, 69, 216, 147, 204, 153, 61, 67, 183, 81, 166, 106, 190, 174, 69, 26, 31, 172, 2, 144, 195, 29, 62, 184, 184, 206, 3, 72, 12, 115, 190, 81, 170, 82, 155, 46, 227, 170, 84, 213, 16, 60, 181, 218, 241, 87, 2, 253, 248, 51, 106, 51, 254, 66, 226, 160, 132, 235, 96, 195, 216, 245, 228, 171, 205, 192, 76, 142, 86, 174, 162, 242, 53, 41, 35, 166, 51, 64, 8, 146, 72, 15, 71, 248, 157, 185, 20, 118, 9, 87, 150, 171, 86, 224, 41, 156, 118, 133, 51, 133, 13, 76, 59, 180, 234, 17, 153, 184, 37, 5, 72, 88, 20, 225, 38, 113, 15, 216, 115, 130, 149, 206, 101, 39, 22, 51, 173, 64, 158, 176, 96, 241, 200, 141, 6, 82, 35, 168, 190, 16, 214, 201, 206, 192, 176, 239, 248, 41, 42, 253, 220, 98, 108, 214, 83, 171, 234, 99, 61, 67, 237, 23, 39, 192, 134, 246, 87, 59, 166, 122, 29, 69, 47, 197, 1, 98, 241, 76, 111, 82, 95, 204, 116, 221, 162, 221, 130, 164, 46, 8, 127, 25, 110, 232, 24, 7, 180, 118, 63, 158, 189, 36, 14, 198, 125, 147, 78, 234, 132, 131, 116, 198, 249, 79, 139, 89, 88, 133, 6, 64, 37, 81, 22, 74, 70, 149, 6, 167, 22, 34, 170, 6, 246, 236, 151, 49, 216, 246, 241, 200, 37, 122, 73, 36, 168, 8, 167, 197, 106, 55, 157, 57, 1, 107, 34, 189, 59, 54, 181, 163, 247, 251, 55, 145, 25, 48, 252, 217, 70, 245, 8, 136, 98, 184, 242, 106, 96, 86, 89, 248, 89, 246, 196, 69, 51, 141, 64, 172, 111, 46, 107, 90, 42, 100, 88, 22, 224, 55, 16, 145, 71, 207, 131, 102, 172, 193, 7, 10, 193, 132, 90, 45, 115, 46, 35, 150, 70, 194, 156, 231, 43, 23, 154, 16, 53, 187, 67, 239, 153, 93, 116, 236, 245, 19, 184, 118, 182, 25, 146, 115, 118, 141, 13, 104, 139, 96, 245, 64, 248, 122, 195, 133, 80, 164, 134, 53, 123, 124, 155, 0, 148, 137, 105, 57, 181, 5, 192, 60, 177, 175, 129, 179, 166, 41, 248, 125, 123, 217, 177, 214, 2, 139, 110, 241, 124, 243, 99, 210, 162, 72, 66, 175, 67, 45, 94, 72, 61, 132, 103, 144, 25, 198, 32, 154, 57, 61, 240, 77, 170, 236, 192, 127, 93, 118, 230, 121, 78, 252, 191, 107, 119, 252, 44, 247, 202, 102, 149, 101, 19, 242, 120, 77, 219, 226, 78, 26, 230, 153, 223, 83, 158, 160, 140, 48, 124, 160, 118, 242, 180, 162, 6, 242, 220, 225, 218, 28, 209, 12, 103, 112, 13, 26, 33, 186, 210, 162, 9, 62, 67, 107, 133, 27, 234, 168, 30, 121, 97, 176, 131, 150, 146, 24, 215, 251, 187, 108, 111, 49, 39, 125, 73, 67, 125, 78, 91, 185, 60, 212, 11, 193, 166, 162, 130, 105, 110, 209, 185, 240, 86, 244, 8, 172, 184, 68, 138, 38, 96, 140, 29, 109, 142, 211, 210, 63, 226, 34, 67, 210, 233, 161, 12, 52, 171, 99, 125, 13, 215, 202, 192, 225, 111, 239, 113, 114, 52, 120, 239, 163, 200, 121, 17, 177, 193, 204, 254, 166, 241, 24, 243, 239, 117, 112, 240, 83, 38, 101, 118, 250, 247, 82, 215, 39, 247, 104, 218, 59, 61, 59, 126, 123, 62, 152, 248, 56, 148, 136, 175, 224, 190, 122, 190, 187, 225, 206, 199, 198, 6, 120, 210, 253, 6, 221, 167, 189, 210, 11, 78, 169, 145, 151, 202, 13, 44, 244, 180, 249, 3, 248, 191, 163, 68, 125, 92, 34, 244, 34, 182, 11, 26, 138, 3, 13, 249, 141, 235, 185, 228, 124, 17, 248, 77, 27, 252, 29, 29, 158, 25, 5, 150, 223, 89, 88, 94, 210, 114, 174, 29, 114, 148, 94, 111, 120, 148, 71, 196, 126, 106, 72, 96, 135, 96, 46, 232, 248, 165, 86, 20, 45, 166, 83, 180, 109, 1, 208, 211, 197, 206, 40, 216, 59, 174, 39, 198, 165, 169, 209, 229, 127, 119, 144, 82, 182, 38, 133, 14, 155, 120, 38, 42, 72, 65, 8, 35, 247, 238, 172, 10, 1, 219, 206, 71, 37, 216, 195, 156, 83, 153, 120, 159, 126, 171, 112, 220, 58, 154, 168, 168, 212, 145, 19, 2, 107, 50, 235, 183, 39, 161, 253, 121, 60, 174, 240, 94, 173, 168, 173, 214, 140, 228, 200, 10, 59, 200, 13, 200, 29, 88, 230, 152, 238, 183, 2, 204, 72, 228, 152, 96, 254, 138, 125, 139, 31, 47, 209, 117, 220, 13, 25, 140, 138, 15, 5, 177, 30, 66, 31, 234, 12, 18, 73, 140, 165, 137, 159, 40, 249, 10, 79, 171, 182, 196, 46, 79, 149, 100, 71, 15, 181, 113, 17, 138, 247, 48, 240, 123, 115, 117, 38, 167, 44, 175, 10, 95, 205, 179, 2, 221, 28, 186, 67, 24, 178, 65, 129, 26, 198, 5, 37, 208, 255, 172, 189, 48, 108, 42, 152, 190, 27, 105, 156, 255, 121, 117, 242, 26, 166, 79, 23, 156, 119, 203, 127, 55, 61, 43, 4, 174, 52, 57, 203, 114, 151, 125, 184, 52, 243, 221, 54, 94, 194, 80, 157, 119, 101, 96, 53, 149, 224, 190, 229, 148, 152, 56, 147, 245, 85, 70, 253, 111, 110, 52, 208, 130, 106, 208, 242, 118, 251, 153, 129, 23, 47, 132, 51, 124, 0, 44, 177, 130, 147, 57, 206, 193, 14, 176, 181, 33, 232, 219, 81, 148, 41, 67, 16, 146, 163, 122, 194, 105, 144, 119, 232, 183, 210, 139, 40, 156, 84, 201, 141, 249, 82, 233, 172, 193, 209, 224, 35, 80, 127, 225, 233, 147, 233, 227, 185, 111, 204, 173, 208, 145, 77, 151, 108, 58, 169, 2, 15, 50, 109, 42, 87, 193, 106, 106, 210, 223, 160, 150, 84, 33, 98, 31, 20, 169, 31, 108, 215, 58, 161, 182, 71, 191, 13, 176, 74, 176, 119, 54, 191, 218, 179, 248, 64, 62, 226, 6, 73, 105, 70, 47, 115, 61, 212, 72, 101, 90, 147, 229, 143, 89, 107, 8, 149, 38, 37, 51, 62, 63, 95, 161, 71, 192, 61, 144, 168, 142, 137, 246, 51, 20, 87, 141, 44, 110, 22, 234, 133, 116, 16, 240, 247, 156, 76, 84, 92, 198, 55, 69, 116, 53, 171, 97, 108, 37, 174, 173, 8, 94, 49, 56, 198, 243, 216, 64, 244, 41, 32, 248, 92, 234, 37, 133, 60, 44, 119, 6, 78, 231, 135, 29, 56, 146, 124, 106, 217, 158, 37, 26, 201, 187, 166, 208, 61, 13, 93, 162, 185, 228, 212, 158, 142, 135, 50, 171, 135, 240, 206, 254, 234, 174, 154, 106, 7, 170, 173, 252, 66, 136, 2, 111, 240, 242, 247, 85, 44, 29, 18, 76, 212, 211, 196, 186, 140, 141, 84, 98, 96, 57, 160, 169, 102, 219, 37, 10, 13, 236, 156, 167, 102, 85, 61, 142, 60, 121, 245, 23, 91, 250, 38, 50, 110, 150, 124, 26, 146, 235, 178, 229, 149, 220, 36, 241, 131, 66, 87, 67, 96, 94, 56, 24, 245, 226, 98, 228, 182, 168, 41, 217, 226, 149, 37, 104, 48, 246, 178, 62, 237, 222, 243, 146, 216, 253, 10, 221, 133, 213, 192, 31, 157, 218, 5, 169, 123, 114, 153, 56, 176, 168, 203, 58, 41, 206, 115, 72, 72, 248, 218, 172, 215, 145, 1, 78, 3, 158, 152, 223, 122, 141, 22, 222, 139, 74, 13, 202, 86, 164, 57, 48, 62, 46, 154, 113, 221, 198, 160, 20, 159, 129, 235, 101, 138, 203, 170, 56, 189, 130, 159, 141, 127, 200, 107, 116, 41, 84, 29, 13, 54, 195, 241, 245, 110, 199, 83, 101, 182, 245, 117, 25, 141, 22, 175, 146, 183, 171, 239, 10, 146, 218, 211, 142, 243, 254, 162, 134, 220, 131, 93, 240, 195, 70, 205, 202, 71, 255, 10, 81, 219, 112, 98, 226, 137, 163, 26, 218, 54, 50, 9, 43, 252, 44, 189, 40, 67, 151, 102, 178, 55, 70, 178, 217, 184, 235, 182, 154, 115, 226, 25, 218, 156, 104, 86, 172, 88, 235, 114, 226, 102, 244, 39, 213, 184, 161, 6, 98, 43, 134, 11, 168, 27, 251, 4, 145, 18, 28, 54, 94, 252, 167, 37, 42, 191, 60, 102, 183, 116, 65, 40, 125, 174, 2, 207, 107, 252, 161, 102, 140, 110, 176, 252, 2, 103, 112, 79, 83, 171, 119, 118, 212, 203, 11, 228, 81, 108, 185, 107, 9, 133, 19, 80, 69, 36, 203, 67, 137, 248, 203, 107, 78, 201, 73, 54, 46, 109, 21, 41, 216, 186, 139, 184, 176, 12, 21, 61, 2, 38, 136, 73, 194, 160, 159, 151, 11, 15, 192, 73, 132, 153, 190, 240, 86, 130, 235, 191, 217, 16, 94, 149, 166, 140, 171, 61, 14, 17, 215, 26, 114, 173, 128, 60, 29, 53, 16, 236, 0, 126, 149, 36, 152, 224, 230, 0, 173, 47, 147, 30, 49, 42, 33, 176, 57, 138, 216, 199, 83, 160, 226, 101, 161, 214, 165, 99, 21, 160, 49, 34, 14, 226, 251, 127, 171, 238, 228, 239, 209, 68, 208, 20, 254, 136, 31, 153, 247, 233, 91, 84, 43, 50, 99, 101, 52, 69, 106, 10, 71, 74, 197, 139, 129, 219, 89, 60, 65, 102, 133, 42, 21, 75, 42, 168, 173, 162, 205, 47, 116, 217, 209, 149, 32, 194, 20, 144, 127, 213, 172, 232, 108, 73, 128, 186, 137, 67, 216, 232, 19, 42, 120, 231, 120, 245, 199, 56, 243, 251, 47, 157, 193, 248, 79, 149, 179, 161, 4, 193, 90, 123, 190, 238, 25, 239, 171, 52, 95, 39, 213, 77, 129, 76, 241, 159, 222, 71, 136, 40, 22, 200, 76, 115, 160, 222, 242, 208, 242, 58, 227, 14, 47, 128, 52, 55, 173, 64, 253, 110, 180, 213, 214, 115, 20, 2, 15, 49, 104, 222, 22, 46, 127, 212, 12, 124, 224, 255, 132, 132, 119, 84, 250, 168, 115, 158, 186, 182, 244, 78, 222, 116, 43, 44, 70, 20, 210, 8, 188, 60, 167, 170, 127, 59, 115, 74, 237, 21, 57, 120, 150, 81, 152, 1, 137, 163, 203, 198, 123, 137, 82, 178, 5, 185, 118, 60, 190, 25, 1, 20, 246, 206, 220, 19, 60, 128, 26, 174, 43, 160, 200, 168, 98, 42, 132, 182, 138, 4, 232, 106, 224, 182, 202, 122, 56, 217, 206, 121, 154, 28, 89, 250, 248, 163, 165, 173, 90, 141, 111, 221, 120, 124, 181, 61, 207, 137, 43, 156, 31, 118, 240, 191, 253, 4, 26, 127, 161, 157, 114, 94, 161, 211, 45, 202, 6, 25, 36, 142, 37, 207, 93, 84, 117, 218, 46, 54, 52, 224, 149, 146, 12, 175, 107, 220, 151, 119, 11, 107, 167, 136, 19, 96, 41, 55, 77, 127, 174, 125, 182, 221, 174, 168, 239, 92, 89, 123, 19, 10, 188, 146, 87, 33, 168, 50, 45, 66, 86, 255, 86, 158, 24, 23, 38, 99, 196, 58, 207, 19, 88, 73, 20, 186, 195, 253, 3, 187, 149, 85, 201, 208, 21, 129, 14, 189, 139, 148, 148, 7, 138, 235, 153, 149, 145, 93, 24, 90, 99, 96, 182, 157, 25, 27, 143, 162, 238, 131, 147, 140, 243, 110, 175, 201, 57, 174, 229, 193, 243, 171, 154, 39, 181, 108, 206, 91, 80, 98, 222, 16, 4, 199, 133, 195, 181, 214, 109, 153, 97, 113, 175, 157, 34, 137, 13, 111, 145, 75, 200, 77, 173, 25, 57, 225, 1, 162, 33, 184, 71, 53, 49, 191, 173, 31, 2, 178, 166, 132, 245, 196, 147, 112, 209, 155, 205, 2, 69, 135, 169, 84, 246, 216, 171, 249, 43, 99, 24, 224, 228, 200, 217, 50, 203, 70, 63, 175, 113, 16, 179, 213, 124, 6, 53, 134, 147, 42, 99, 236, 122, 48, 155, 18, 84, 145, 28, 87, 70, 217, 132, 103, 130, 194, 48, 116, 72, 20, 187, 242, 139, 56, 237, 148, 86, 250, 66, 17, 224, 137, 112, 218, 20, 159, 70, 115, 170, 33, 163, 121, 245, 141, 68, 167, 245, 62, 63, 63, 93, 85, 185, 218, 186, 15, 82, 17, 91, 78, 204, 243, 57, 158, 106, 221, 100, 54, 166, 193, 167, 126, 188, 237, 85, 2, 139, 79, 54, 74, 91, 177, 59, 240, 65, 248, 143, 125, 44, 101, 86, 183, 172, 184, 35, 11, 238, 81, 179, 218, 176, 129, 59, 253, 171, 94, 129, 52, 201, 40, 28, 155, 227, 3, 85, 219, 125, 47, 35, 99, 42, 75, 235, 162, 96, 46, 250, 31, 127, 30, 6, 199, 128, 3, 234, 201, 99, 117, 188, 158, 87, 80, 215, 115, 62, 236, 81, 206, 115, 219, 201, 87, 66, 160, 187, 148, 22, 30, 232, 208, 250, 237, 43, 112, 251, 202, 155, 71, 225, 66, 146, 117, 220, 216, 71, 110, 105, 146, 61, 52, 164, 251, 20, 177, 121, 200, 138, 25, 179, 35, 230, 147, 86, 177, 33, 131, 250, 207, 92, 213, 97, 233, 255, 38, 161, 94, 19, 145, 16, 69, 170, 216, 174, 183, 81, 211, 16, 108, 1, 4, 144, 61, 71, 82, 122, 56, 124, 18, 34, 156, 27, 255, 133, 18, 16, 0, 127, 186, 26, 85, 216, 253, 63, 162, 213, 121, 43, 72, 252, 161, 132, 176, 209, 152, 115, 67, 3, 166, 41, 156, 11, 162, 115, 96, 214, 68, 210, 147, 170, 255, 39, 31, 122, 118, 135, 148, 69, 159, 178, 4, 14, 87, 20, 85, 165, 209, 212, 16, 247, 90, 150, 177, 76, 79, 92, 62, 155, 113, 39, 178, 214, 47, 16, 149, 198, 86, 10, 238, 134, 101, 96, 252, 131, 3, 109, 105, 154, 102, 172, 122, 241, 40, 61, 36, 102, 196, 10, 229, 209, 36, 91, 207, 246, 99, 90, 49, 204, 30, 240, 222, 217, 34, 4, 20, 141, 48, 126, 51, 227, 139, 51, 34, 72, 54, 222, 199, 180, 40, 234, 207, 114, 178, 224, 6, 15, 28, 252, 40, 43, 32, 102, 206, 143, 93, 146, 24, 8, 18, 217, 24, 141, 60, 28, 203, 64, 86, 166, 128, 192, 41, 102, 18, 204, 62, 83, 123, 1, 61, 26, 47, 3, 247, 244, 129, 66, 224, 87, 22, 22, 216, 161, 163, 109, 180, 218, 102, 20, 6, 14, 88, 229, 62, 183, 78, 73, 207, 31, 121, 117, 73, 4, 11, 191, 248, 169, 223, 211, 33, 12, 168, 97, 22, 108, 69, 199, 100, 2, 37, 90, 149, 117, 180, 196, 157, 251, 160, 211, 132, 159, 39, 73, 87, 230, 104, 172, 147, 247, 61, 249, 33, 33, 218, 5, 178, 177, 39, 29, 35, 77, 91, 25, 186, 3, 240, 35, 153, 181, 206, 188, 228, 217, 128, 175, 9, 246, 247, 105, 147, 28, 250, 162, 178, 8, 229, 184, 175, 212, 8, 103, 255, 106, 162, 2, 84, 72, 117, 97, 178, 240, 235, 247, 157, 183, 22, 106, 228, 243, 129, 73, 24, 249, 75, 65, 60, 71, 150, 237, 189, 18, 24, 201, 205, 62, 151, 189, 183, 118, 137, 66, 96, 147, 142, 38, 47, 45, 225, 127, 132, 124, 237, 180, 141, 8, 19, 209, 43, 30, 246, 96, 101, 62, 214, 217, 11, 124, 242, 141, 181, 29, 80, 66, 36, 92, 81, 194, 36, 149, 243, 71, 135, 196, 20, 158, 83, 30, 21, 67, 53, 77, 212, 191, 247, 161, 185, 33, 188, 204, 201, 123, 17, 133, 226, 205, 116, 113, 171, 164, 76, 78, 152, 163, 46, 93, 38, 211, 184, 247, 144, 161, 4, 235, 217, 51, 149, 5, 185, 46, 159, 224, 200, 84, 232, 27, 0, 134, 216, 12, 55, 255, 34, 247, 50, 153, 233, 110, 147, 128, 29, 204, 233, 64, 54, 13, 153, 247, 194, 20, 146, 144, 129, 240, 1, 220, 159, 120, 98, 129, 146, 87, 44, 66, 45, 20, 53, 115, 5, 214, 4, 203, 44, 66, 209, 225, 76, 135, 180, 126, 163, 117, 247, 219, 181, 150, 10, 233, 67, 34, 18, 210, 228, 51, 59, 185, 8, 140, 186, 203, 148, 163, 79, 248, 138, 228, 32, 211, 44, 180, 91, 195, 162, 254, 161, 164, 209, 183, 45, 42, 187, 115, 42, 179, 244, 168, 119, 34, 34, 73, 37, 14, 248, 41, 71, 175, 17, 211, 224, 19, 165, 129, 43, 91, 230, 186, 77, 164, 30, 205, 21, 87, 229, 128, 36, 9, 13, 160, 91, 67, 136, 43, 19, 36, 185, 42, 209, 176, 118, 69, 131, 65, 13, 123, 103, 129, 159, 158, 51, 207, 26, 77, 148, 221, 26, 113, 164, 64, 117, 228, 220, 0, 57, 99, 93, 117, 223, 166, 28, 106, 76, 84, 214, 248, 80, 214, 86, 24, 111, 63, 89, 1, 220, 77, 37, 148, 251, 35, 146, 112, 43, 80, 173, 20, 124, 18, 228, 182, 115, 125, 100, 95, 53, 76, 209, 128, 177, 130, 122, 213, 192, 78, 101, 35, 45, 115, 24, 217, 169, 53, 175, 171, 79, 40, 164, 168, 254}
            Dim treekVal = New TLLInterface(chunk, "5ss8:,UaAUhzTE?9trSjSynsxDxTRbn")
            g_treekvalid = (String.Compare(treekCore.InitChunk.TLLLicense, treekVal.InitChunk.TLLLicense, True) = 0)
            g_treekvalidated = True
        End If
        Return g_treekvalid

    End Function

#End If

#End Region ' License

End Class
