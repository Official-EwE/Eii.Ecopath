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
' Copyright 1991-2012 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'
#Region " Imports "

Option Strict On
Imports System.Drawing
Imports System.IO
Imports EwEUtils.SpatialData
Imports EwEUtils.SystemUtilities
Imports EwEUtils.Utilities

#End Region ' Imports

Namespace SpatialData

    ''' <summary>
    ''' Cache for spatial data files
    ''' </summary>
    Public Class cSpatialDataCache
        Implements ISpatialDataCache

#Region " Private vars "

        ''' <summary>Root folder for the cache.</summary>
        Private m_strRootPath As String = ""

#End Region ' Private vars

#Region " Singleton "

        ''' <summary>Default spatial data cache instance.</summary>
        Private Shared s_default As cSpatialDataCache = Nothing

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns the default spatial data cache.
        ''' </summary>
        ''' <returns>The default spatial data cache.</returns>
        ''' -------------------------------------------------------------------
        Public Shared Function DefaultDataCache() As cSpatialDataCache
            If (s_default Is Nothing) Then
                s_default = New cSpatialDataCache()
            End If
            Return s_default
        End Function

#End Region ' Singleton

#Region " Construction / destruction "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructs a new cache.
        ''' </summary>
        ''' <param name="strRootPath">The <see cref="Path">cache folder</see> to use.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(Optional strRootPath As String = "")
            Me.m_strRootPath = strRootPath
        End Sub

#End Region ' Construction / destruction

#Region " Maintenance "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns the total size of the cache.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Function GetSize() As Long
            Return Me.GetTotalFileSize(Me.GetCachePaths(Nothing))
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns the size of the cache of datasets no longer present in the
        ''' datamanager.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Function GetUnusedSize(man As cSpatialDataSetManager) As Long
            Return Me.GetTotalFileSize(Me.GetCachePaths(man))
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Clear cached data files
        ''' </summary>
        ''' <param name="man">If provided, only datasets no longer present in the
        ''' datamanager will be cleared.</param>
        ''' <returns>True if successful.</returns>
        ''' -------------------------------------------------------------------
        Public Function Clear(Optional man As cSpatialDataSetManager = Nothing) As Boolean

            Dim bSucces As Boolean = True
            For Each strCachePath As String In Me.GetCachePaths(man)
                Try
                    bSucces = bSucces And cFileUtils.DeleteDirectory(strCachePath)
                Catch ex As Exception
                    cLog.Write(ex, String.Format("cSpatialDataCache::Clear {0}", strCachePath))
                    bSucces = False
                End Try
            Next strCachePath
            Return True

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Clear cached files for a given dataset.
        ''' </summary>
        ''' <param name="ds"></param>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Public Function Clear(ds As ISpatialDataSet) As Boolean
            Dim strCachePath As String = Me.GetCacheFolder(ds)
            Try
                cFileUtils.DeleteDirectory(strCachePath)
            Catch ex As Exception
                cLog.Write(ex, String.Format("cSpatialDataCache::Clear {0}", strCachePath))
                Return False
            End Try
            Return True
        End Function

#End Region ' Maintenance

#Region " Cache access "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the path to the cache root folder.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property RootFolder As String _
            Implements ISpatialDataCache.RootFolder
            Get
                If (String.IsNullOrWhiteSpace(Me.m_strRootPath)) Then
                    Return System.IO.Path.Combine(cSystemUtils.ApplicationSettingsPath, "Cache\Spatial")
                End If
                Return Me.m_strRootPath
            End Get
            Set(value As String)
                Me.m_strRootPath = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="ISpatialDataCache.GetFileName"/>"
        ''' -------------------------------------------------------------------
        Public Function GetFileName(ds As ISpatialDataSet, _
                                    ptfTL As PointF, ptfBR As PointF, dCellSize As Double, time As DateTime, _
                                    strFilter As String, strExt As String) As String _
                                Implements ISpatialDataCache.GetFileName
            If Guid.Empty.Equals(ds.GUID) Then
                Return cFileUtils.MakeTempFile(strExt)
            Else
                Return Me.GetCacheFileName(ds, ptfTL, ptfBR, dCellSize, time, strFilter, strExt)
            End If
        End Function

#End Region ' Cache access

#Region " Internals "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns the path to a cache for a dataset.
        ''' </summary>
        ''' <param name="ds"><see cref="ISpatialDataSet"/> to obtain the cache path for.</param>
        ''' -------------------------------------------------------------------
        Private Function GetCacheFolder(ds As ISpatialDataSet) As String

            ' Sanity checks
            Debug.Assert(ds IsNot Nothing, "Need valid dataset")
            Debug.Assert(Not Guid.Empty.Equals(ds.GUID), "Need dataset with valid GUID")

            Return System.IO.Path.Combine(Me.RootFolder, ds.GUID.ToString)

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns the path to a cache for a dataset, spatial extent, cell size and time.
        ''' </summary>
        ''' <param name="ds"><see cref="ISpatialDataSet"/> to obtain the cache path for.</param>
        ''' <param name="ptfTL">Top-left location (in decimal degrees lon,lat) of the bounding box of the data.</param>
        ''' <param name="ptfBR">Bottom-right location (in decimal degrees lon,lat) of the bounding box of the data.</param>
        ''' <param name="dCellSize">Cell size to obtain the cache path for.</param>
        ''' <param name="bCreateIfMissing">Flag, stating that the path can be created
        ''' if missing.</param>
        ''' <returns>A cache path.</returns>
        ''' -------------------------------------------------------------------
        Private Function GetCacheFolder(ds As ISpatialDataSet, _
                                        ptfTL As PointF, ptfBR As PointF, _
                                        dCellSize As Double, bCreateIfMissing As Boolean) As String

            ' Sanity checks
            Debug.Assert(ds IsNot Nothing, "Need valid dataset")
            Debug.Assert(Not Guid.Empty.Equals(ds.GUID), "Need dataset with valid GUID")
            Debug.Assert(dCellSize > 0.0, "Need dataset with cell size> 0")
            Debug.Assert(Not ptfTL.Equals(ptfBR), "Need non-empty bounding box")

            Dim strDSFolder As String = Me.GetCacheFolder(ds)
            Dim strBoxFolder As String = Path.Combine(strDSFolder, _
                                                      String.Format("[{0},{1}-{2},{3}]", cStringUtils.FormatSingle(ptfTL.X), cStringUtils.FormatSingle(ptfBR.Y), cStringUtils.FormatSingle(ptfBR.X), cStringUtils.FormatSingle(ptfTL.Y)))
            Dim strCacheFolder As String = System.IO.Path.Combine(strBoxFolder, cStringUtils.FormatSingle(CSng(dCellSize)))

            If (bCreateIfMissing And Not Directory.Exists(strCacheFolder)) Then
                Try
                    Directory.CreateDirectory(strCacheFolder)
                Catch ex As Exception

                End Try
            End If
            Return strCacheFolder
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns the path to a cache for a dataset.
        ''' </summary>
        ''' <param name="ds"><see cref="ISpatialDataSet"/> to obtain the cache path for.</param>
        ''' <param name="ptfTL">Top-left location (in decimal degrees lon,lat) of the bounding box of the data.</param>
        ''' <param name="ptfBR">Bottom-right location (in decimal degrees lon,lat) of the bounding box of the data.</param>
        ''' <param name="dCellSize">Cell size to obtain the cache path for.</param>
        ''' <param name="dt">Date to create the file name for.</param>
        ''' <param name="strExt">File extension tpo create the file name for.</param>
        ''' <returns>A cache path.</returns>
        ''' -------------------------------------------------------------------
        Private Function GetCacheFileName(ds As ISpatialDataSet, _
                                          ptfTL As PointF, ptfBR As PointF, _
                                          dCellSize As Double, _
                                          dt As DateTime, _
                                          strFilter As String, _
                                          strExt As String) As String

            Dim strPath As String = GetCacheFolder(ds, ptfTL, ptfBR, dCellSize, True)
            Dim strFileName As String = cFileUtils.ToValidFileName(String.Format("{0}[{1}]{2}", dt.ToString("yyyy-MM-dd"), strFilter, strExt), False)

            Return System.IO.Path.Combine(strPath, strFileName)

        End Function

        Private Function GetCachePaths(man As cSpatialDataSetManager) As String()

            Dim lstrPaths As New List(Of String)
            Dim lstrRemove As New List(Of String)

            ' Get all cache dirs
            If (Directory.Exists(Me.RootFolder)) Then
                lstrPaths.AddRange(Directory.GetDirectories(Me.RootFolder))
            End If

            If (man IsNot Nothing) Then
                ' Remove all folder entries for datasets that are defined
                For Each ds As ISpatialDataSet In man
                    lstrRemove.Add(Me.GetCacheFolder(ds))
                Next
            End If

            ' Validate folder names; we only want folders that deserialize into proper GUIDs
            For Each strPath As String In lstrPaths
                Dim guid As Guid = Nothing
                Dim bValid As Boolean = False
                Try
                    Dim astrBits As String() = strPath.Split(Path.DirectorySeparatorChar)
                    guid = System.Guid.Parse(astrBits(astrBits.Length - 1))
                    bValid = Not guid.Empty.Equals(guid)
                Catch ex As Exception
                    ' Totally false, lol
                    bValid = False
                    bValid = False
                    bValid = False
                    bValid = False
                End Try
                If Not bValid Then lstrRemove.Add(strPath)
            Next

            ' Delete all invalid paths
            For Each strPath As String In lstrRemove
                lstrPaths.Remove(strPath)
            Next

            ' Return valid cache paths
            Return lstrPaths.ToArray

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns the total file size of a series of folders.
        ''' </summary>
        ''' <param name="astrFolders"></param>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Private Function GetTotalFileSize(astrFolders As String()) As Long
            Dim lTotalSize As Long = 0

            For Each strPath As String In astrFolders
                Dim di As New DirectoryInfo(strPath)
                Dim afsi() As FileInfo = di.GetFiles("*.*", SearchOption.AllDirectories)
                For Each fsi As FileInfo In afsi
                    lTotalSize += fsi.Length
                Next
            Next
            Return lTotalSize

        End Function
#End Region ' Internals

    End Class

End Namespace
