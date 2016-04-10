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
'    UBC Fisheries Centre, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports System.Collections.Generic
Imports System.Drawing
Imports System.IO
Imports System.Xml
Imports DotSpatial.Data
Imports DotSpatial.Projections
Imports EwECore
Imports EwEPlugin
Imports EwEUtils.Core
Imports EwEUtils.SpatialData
Imports EwEUtils.Utilities
Imports System.Windows.Forms
Imports ScientificInterfaceShared.Controls
Imports EwECore.SpatialData

#End Region ' Imports

' ToDo: figure out how to process an data that spans the 180 degree meridian

Namespace SpatialData

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' <see cref="ISpatialDataSet"/> for accessing a folder of spatio-temporal files.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public MustInherit Class cFileDataSetPlugin
        Implements ISpatialDataSetPlugin
        Implements IConfigurablePlugin
        Implements IDisposable

#Region " Private vars "

        Protected m_core As cCore = Nothing

        ''' <summary>Name of the data set.</summary>
        Protected m_strName As String = ""

        ''' <summary>Source data file (not loaded from cache).</summary>
        Private m_dsSourceData As IDataSet = Nothing
        ''' <summary>Date that the loaded data represents.</summary>
        Protected m_dtTime As DateTime = Nothing
        ''' <summary>Ecospace raster extent.</summary>
        Protected m_extModelArea As Extent = Nothing
        ''' <summary>Ecospace cell size.</summary>
        Protected m_dModelCellSize As Double = 0
        ''' <summary>Ecospace target projection.</summary>
        Protected m_strProjectionString As String = ""

        ''' <summary>States whether the dataset is allowed to deliver data.</summary>
        Private m_bEnabled As Boolean = True

        Private m_bRelative As Boolean = False

#End Region ' Private vars

#Region " Construction / destruction "

        Public Sub New()
            Me.DBID = Guid.Empty
        End Sub

        Public Sub Dispose() _
            Implements IDisposable.Dispose
            If Me.IsLocked Then Me.UnlockData()
            GC.SuppressFinalize(Me)
        End Sub

#End Region ' Construction / destruction

#Region " Information "

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="ISpatialDataSet.GUID" />
        ''' -------------------------------------------------------------------
        Public Property DBID As Guid _
            Implements ISpatialDataSet.GUID

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="ISpatialDataSet.DisplayName" />
        ''' -------------------------------------------------------------------
        Public MustOverride Property DisplayName As String _
            Implements ISpatialDataSet.DisplayName

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="ISpatialDataSet.DataDescription" />
        ''' -------------------------------------------------------------------
        Public Overridable Property DataDescription As String _
            Implements ISpatialDataSet.DataDescription

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="IPlugin.Description" />
        ''' -------------------------------------------------------------------
        Public MustOverride ReadOnly Property Description As String _
            Implements IPlugin.Description

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="ISpatialDataSet.Source" />
        ''' -------------------------------------------------------------------
        Public Property Source As String _
            Implements ISpatialDataSet.Source

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="ISpatialDataSet.TimeEnd"/>
        ''' -------------------------------------------------------------------
        Public MustOverride ReadOnly Property TimeEnd As DateTime _
            Implements ISpatialDataSet.TimeEnd

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="ISpatialDataSet.TimeStart"/>
        ''' -------------------------------------------------------------------
        Public MustOverride ReadOnly Property TimeStart As DateTime _
            Implements ISpatialDataSet.TimeStart

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="ISpatialDataSet.DialogReadFilter"/>"
        ''' -------------------------------------------------------------------
        Public MustOverride ReadOnly Property DialogReadFilter(ByVal bRaster As Boolean, _
                                                               ByVal bImage As Boolean, _
                                                               ByVal bVector As Boolean) As String _
             Implements ISpatialDataSet.DialogReadFilter

        Public Overrides Function ToString() As String
            Return Me.DisplayName()
        End Function

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="ISpatialDataSet.VarName"/>"
        ''' -------------------------------------------------------------------
        Public Property VarName As EwEUtils.Core.eVarNameFlags _
             Implements EwEUtils.SpatialData.ISpatialDataSet.VarName

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="ISpatialDataSet.ConversionFormat"/>"
        ''' -------------------------------------------------------------------
        Public MustOverride ReadOnly Property ConversionFormat As String _
            Implements EwEUtils.SpatialData.ISpatialDataSet.ConversionFormat

#End Region ' Information

#Region " Configuration "

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="ISpatialDataSet.Configuration"/>
        ''' -------------------------------------------------------------------
        Public Property Configuration(ByVal doc As XmlDocument, _
                                      ByVal strFolderRoot As String) As XmlNode _
            Implements ISpatialDataSet.Configuration
            Get
                Return Me.ToXML(doc, strFolderRoot)
            End Get
            Set(ByVal value As XmlNode)
                Me.FromXML(doc, value, strFolderRoot)
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="IConfigurablePlugin.GetConfigUI"/>
        ''' -------------------------------------------------------------------
        Public MustOverride Function GetConfigUI() As Windows.Forms.Control _
            Implements IConfigurablePlugin.GetConfigUI

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="IConfigurablePlugin.IsConfigured"/>
        ''' -------------------------------------------------------------------
        Public MustOverride Function IsConfigured() As Boolean _
            Implements IConfigurablePlugin.IsConfigured, ISpatialDataSet.IsConfigured

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="IExternalDataSource.EnableData"/>
        ''' -------------------------------------------------------------------
        Public Property EnableData(ByVal runtype As IRunType) As Boolean _
            Implements IExternalDataSource.EnableData
            Get
                Return True
            End Get
            Set(ByVal value As Boolean)
                ' NOP
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="IExternalDataSource.IsDataAvailable"/>
        ''' -------------------------------------------------------------------
        Public MustOverride Function IsDataAvailable(ByVal runtype As IRunType) As Boolean _
            Implements IExternalDataSource.IsDataAvailable

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Write configuration to XML.
        ''' </summary>
        ''' <param name="doc">The doc to generate nodes for.</param>
        ''' <returns>
        ''' An XML node that contains the configuration of the dataset.
        ''' </returns>
        ''' -------------------------------------------------------------------
        Protected MustOverride Function ToXML(ByVal doc As XmlDocument, _
                                              ByVal strFolderRoot As String) As XmlNode

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Read configuration from XML.
        ''' </summary>
        ''' <param name="doc">The doc to read the configuration from.</param>
        ''' <param name="node">The node that contains the configuration of the dataset.</param>
        ''' <returns>
        ''' True if successful.
        ''' </returns>
        ''' -------------------------------------------------------------------
        Protected MustOverride Function FromXML(ByVal doc As XmlDocument, _
                                                ByVal node As XmlNode, _
                                                ByVal strFolderRoot As String) As Boolean

#End Region ' Configuration

#Region " Import / export "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set whether files are found on a path, dictated by source,
        ''' relative to the current configuration file.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Friend Property IsSourceRelative As Boolean
            Get
                Return Me.m_bRelative
            End Get
            Set(value As Boolean)
                If (value <> Me.m_bRelative) Then
                    Me.m_bRelative = value
                End If
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="ISpatialDataSet.ExportTo"/>
        ''' -------------------------------------------------------------------
        Public MustOverride Function ExportTo(ByVal strPath As String) As ISpatialDataSet _
            Implements ISpatialDataSet.ExportTo

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns an absolute version of a relative path.
        ''' </summary>
        ''' <param name="strPath"></param>
        ''' <param name="strPathBase">The absolute path to resolve to. If not specified,
        ''' the path to the current Dataset configuration file is obtained from the
        ''' EwE ccore.</param>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Protected Function ToAbsolutePath(ByVal strPath As String, ByVal strPathBase As String) As String

            Return cFileUtils.NormalizePath(Path.Combine(strPathBase, strPath))

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns a relative version of an absolute path.
        ''' </summary>
        ''' <param name="strPath"></param>
        ''' <param name="strPathBase">The absolute path to resolve from. If not specified,
        ''' the path to the current Dataset configuration file is obtained from the
        ''' EwE ccore.</param>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Protected Function ToRelativePath(ByVal strPath As String, ByVal strPathBase As String) As String

            If (strPath.StartsWith(".\")) Then
                strPath = strPath.Substring(2)
            End If

            Return cFileUtils.RelativePath(strPathBase, strPath)

        End Function

#End Region ' Import / export

#Region " Data "

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="ISpatialDataSet.GetRaster"/>
        ''' -------------------------------------------------------------------
        Public Overridable Function GetRaster(ByVal converter As ISpatialDataConverter, _
                                              ByVal strLayerName As String) As ISpatialRaster _
            Implements ISpatialDataSet.GetRaster

            Debug.Assert(converter IsNot Nothing)
            If (Not Me.IsLocked) Then Return Nothing

            ' Get cache file name
            Dim strFileName As String = Me.CacheFileName(strLayerName)
            ' Does file exist in cache AND allowed to use it?
            If (System.IO.File.Exists(strFileName)) And (Me.ReadFromCache = True) Then
                ' #Yes: grab file from cache
                Dim ds As IDataSet = cDotSpatialUtils.OpenFile(strFileName)
                ' Is loaded?
                If (ds IsNot Nothing) Then
                    ds.Close()
                    ' Is a raster?
                    If (TypeOf ds Is IRaster) Then
                        ' Is really valid?
                        Dim rs As IRaster = DirectCast(ds, IRaster)

                        ' Sanity checks
                        Debug.Assert(cNumberUtils.Approximates(Me.m_dModelCellSize, rs.CellWidth, Me.m_dModelCellSize * cDotSpatialUtils.EQUALS_FACTOR))
                        Debug.Assert(cNumberUtils.Approximates(Me.m_dModelCellSize, rs.CellHeight, Me.m_dModelCellSize * cDotSpatialUtils.EQUALS_FACTOR))

                        Dim rsOut As New cSpatialRaster(rs)
                        Me.LogMessage(My.Resources.STATUS_LOADED_FROM_CACHE, eStatusFlags.OK)
                        Return rsOut
                    End If
                End If
            End If

            ' Fallback: cache somehow did not work, reload data
            ' Able to load source?
            If Not Me.LoadSource() Then
                ' #No: something is screwed up but can't do anything about it
                Return Nothing
            End If

            Return converter.Convert(Me.m_dsSourceData, _
                                     cDotSpatialUtils.TopLeft(Me.m_extModelArea), _
                                     cDotSpatialUtils.BottomRight(Me.m_extModelArea), _
                                     Me.m_dModelCellSize, _
                                     Me.m_strProjectionString, _
                                     strFileName)

        End Function

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="ISpatialDataSet.HasDataAtT"/>
        ''' -------------------------------------------------------------------
        Public MustOverride Function HasDataAtT(ByVal datetime As DateTime) As Boolean _
            Implements ISpatialDataSet.HasDataAtT

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="ISpatialDataSet.GetExtentAtT"/>
        ''' -------------------------------------------------------------------
        Public MustOverride Function GetExtentAtT(ByVal datetime As DateTime, _
                                                  ByRef ptfTL As PointF, _
                                                  ByRef ptfBR As PointF) As Boolean _
            Implements ISpatialDataSet.GetExtentAtT

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="ISpatialDataSet.IndexStatusAtT"/>
        ''' -------------------------------------------------------------------
        Protected MustOverride Function IndexStatusAtT(dt As DateTime) As ISpatialDataSet.eIndexStatus _
            Implements ISpatialDataSet.IndexStatusAtT

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="ISpatialDataSet.UpdateIndexAtT"/>
        ''' -------------------------------------------------------------------
        Protected MustOverride Sub UpdateIndexAtT(dt As DateTime) _
            Implements ISpatialDataSet.UpdateIndexAtT

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="ISpatialDataSet.IsLocked"/>
        ''' -------------------------------------------------------------------
        Public Overridable Function IsLocked() As Boolean _
            Implements EwEUtils.SpatialData.ISpatialDataSet.IsLocked
            Return (Me.m_extModelArea IsNot Nothing)
        End Function

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="ISpatialDataSet.LockDataAtT"/>
        ''' -------------------------------------------------------------------
        Public Overridable Function LockDataAtT(ByVal datetime As Date, _
                                                ByVal dCellSize As Double, _
                                                ByVal ptfTL As System.Drawing.PointF, _
                                                ByVal ptfBR As System.Drawing.PointF, _
                                                ByVal strProjectionString As String) As Boolean _
            Implements EwEUtils.SpatialData.ISpatialDataSet.LockDataAtT

            ' Sanity checks
            Debug.Assert(dCellSize > 0)
            Debug.Assert(ptfTL.X < ptfBR.X)
            Debug.Assert(ptfTL.Y > ptfBR.Y)

            Me.m_extModelArea = New Extent(ptfTL.X, ptfBR.Y, ptfBR.X, ptfTL.Y)
            Me.m_dModelCellSize = dCellSize
            Me.m_strProjectionString = strProjectionString
            Me.m_dtTime = datetime

            Return True

        End Function

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="ISpatialDataSet.Unlock"/>
        ''' -------------------------------------------------------------------
        Public Overridable Function UnlockData() As Boolean _
            Implements EwEUtils.SpatialData.ISpatialDataSet.Unlock

            If (Me.m_dsSourceData IsNot Nothing) Then
                Try
                    Me.m_dsSourceData.Close()
                Catch ex As ApplicationException
                    ' Swallow this - GDAL may complain
                Catch ex As Exception
                    Debug.Assert(False)
                End Try
                Me.m_dsSourceData.Dispose()
                Me.m_dsSourceData = Nothing
            End If
            Me.m_extModelArea = Nothing

            Return True

        End Function

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="ISpatialDataSet.GetAttributes"/>
        ''' -------------------------------------------------------------------
        Public Function GetAttributes() As String() _
            Implements EwEUtils.SpatialData.ISpatialDataSet.GetAttributes

            Dim lstrAttributes As New List(Of String)
            Dim fs As IFeatureSet = Nothing
            Dim dt As DataTable = Nothing

            Try

                If (Me.LoadSource()) Then
                    If (TypeOf Me.m_dsSourceData Is IFeatureSet) Then
                        fs = DirectCast(Me.m_dsSourceData, IFeatureSet)
                        dt = fs.DataTable
                        For iCol As Integer = 0 To dt.Columns.Count - 1
                            Dim col As DataColumn = dt.Columns(iCol)
                            lstrAttributes.Add(col.ColumnName)
                        Next
                    End If
                End If

            Catch ex As Exception
                cLog.Write(ex, "cFileDataSet::GetAttributes")
            End Try

            Return lstrAttributes.ToArray()

        End Function

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="ISpatialDataSet.GetAttributeDataTypes"/>
        ''' -------------------------------------------------------------------
        Public Function GetAttributeDataTypes() As Type() _
             Implements ISpatialDataSet.GetAttributeDataTypes

            Dim ltAttributes As New List(Of Type)
            Dim fs As IFeatureSet = Nothing
            Dim dt As DataTable = Nothing

            Try

                If (Me.LoadSource()) Then
                    If (TypeOf Me.m_dsSourceData Is IFeatureSet) Then
                        fs = DirectCast(Me.m_dsSourceData, IFeatureSet)
                        dt = fs.DataTable
                        For iCol As Integer = 0 To dt.Columns.Count - 1
                            ltAttributes.Add(dt.Columns(iCol).DataType)
                        Next
                    End If
                End If

            Catch ex As Exception
                cLog.Write(ex, "cFileDataSet::GetAttributeDataTypes")
            End Try

            Return ltAttributes.ToArray()

        End Function

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="ISpatialDataSet.GetAttributeValues"/>
        ''' -------------------------------------------------------------------
        Public Function GetAttributeValues() As DataTable _
            Implements EwEUtils.SpatialData.ISpatialDataSet.GetAttributeValues

            Dim fs As IFeatureSet = Nothing
            Dim dt As DataTable = Nothing

            Try

                If (Me.LoadSource()) Then
                    If (TypeOf Me.m_dsSourceData Is IFeatureSet) Then
                        fs = DirectCast(Me.m_dsSourceData, IFeatureSet)
                        If Not fs.AttributesPopulated Then
                            fs.FillAttributes()
                        End If
                        dt = fs.DataTable
                    End If
                End If

            Catch ex As Exception
                cLog.Write(ex, "cFileDataSet::GetAttributeValues")
            End Try

            Return dt

        End Function

#End Region ' Data

#Region " Cache "

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="ISpatialDataSet.Cache"/>"
        ''' -------------------------------------------------------------------
        Protected Property Cache As ISpatialDataCache _
            Implements ISpatialDataSet.Cache
            Get
                Return cSpatialDataCache.DefaultDataCache
            End Get
            Set(value As ISpatialDataCache)
                ' NOP
            End Set
        End Property

#End Region ' Cache

#Region " Internals "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Load the actual source document for the current time step.
        ''' </summary>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Protected Overridable Function LoadSource() As Boolean

            Try

                Dim strFileName As String = Me.SourceFileName()
                If (Me.m_dsSourceData IsNot Nothing) Then
                    Me.LogMessage(cStringUtils.Localize(My.Resources.STATUS_LOAD_SKIPPED, strFileName), eStatusFlags.OK)
                    Return True
                End If

                If (Not File.Exists(strFileName)) Then
                    Me.LogMessage(cStringUtils.Localize(My.Resources.STATUS_LOAD_FILENOTFOUND, strFileName), eStatusFlags.MissingParameter)
                    Me.StoreExtent(Nothing)
                    Return False
                End If

                Me.m_dsSourceData = cDotSpatialUtils.OpenFile(strFileName)
                If (m_dsSourceData IsNot Nothing) Then
                    Me.StoreExtent(Me.m_dsSourceData.Extent)
                    Me.LogMessage(cStringUtils.Localize(My.Resources.STATUS_LOADED, strFileName), eStatusFlags.OK)
                    Return True
                Else
                    Me.LogMessage(cStringUtils.Localize(My.Resources.STATUS_LOAD_FAILED, ""), eStatusFlags.MissingParameter)
                    Return False
                End If

            Catch ex As Exception
                Me.LogMessage(cStringUtils.Localize(My.Resources.STATUS_LOAD_FAILED, ex.Message), eStatusFlags.MissingParameter)
                ' Log an error
                Me.StoreExtent(Nothing)
                ' Failed
                Me.m_dsSourceData = Nothing
            End Try

            Return False

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Internal helper to enable and disable cache access.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Protected ReadOnly Property ReadFromCache As Boolean
            Get
                Return (Me.Cache IsNot Nothing)
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Store the extent of a loaded dataset.
        ''' </summary>
        ''' <param name="ext"></param>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Protected MustOverride Function StoreExtent(ext As IExtent) As Boolean

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the complete path to the external data file for the current timestep.
        ''' </summary>
        ''' <returns>The complete path to the external data file for the current timestep.</returns>
        ''' -------------------------------------------------------------------
        Protected MustOverride Function SourceFileName() As String

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Create a cached file name (including path) for a file, current extent, 
        ''' time step, and cell size.
        ''' </summary>
        ''' <returns>A file name to store the raster in.</returns>
        ''' <remarks>
        ''' If a <see cref="Cache"/> is provided the file name should point at the
        ''' relevant Cache path. If not, a temporary file is generated.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Protected MustOverride Function CacheFileName(strLayerName As String) As String

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Log a GIS operation message.
        ''' </summary>
        ''' <param name="strMessage">The message to log.</param>
        ''' -------------------------------------------------------------------
        Protected Sub LogMessage(strMessage As String, status As eStatusFlags)

            If (Me.m_core IsNot Nothing) Then
                Me.m_core.SpatialOperationLog.LogOperation(strMessage, status)
            End If

        End Sub

#End Region ' Internals

#Region " Plug-in implementation "

        ''' -----------------------------------------------------------------------
        ''' <inheritdocs cref="EwEPlugin.IPlugin.Author"/>
        ''' -----------------------------------------------------------------------
        Public ReadOnly Property Author As String _
            Implements EwEPlugin.IPlugin.Author
            Get
                Return "Jeroen Steenbeek, UBC Fisheries Centre"
            End Get
        End Property

        ''' -----------------------------------------------------------------------
        ''' <inheritdocs cref="EwEPlugin.IPlugin.Contact"/>
        ''' -----------------------------------------------------------------------
        Public ReadOnly Property Contact As String _
            Implements EwEPlugin.IPlugin.Contact
            Get
                Return "mailto:ewedevteam@gmail.com"
            End Get
        End Property

        ''' -----------------------------------------------------------------------
        ''' <inheritdocs cref="EwEPlugin.IPlugin.Initialize"/>
        ''' -----------------------------------------------------------------------
        Public Sub Initialize(ByVal core As Object) _
            Implements EwEPlugin.IPlugin.Initialize
            Me.m_core = DirectCast(core, cCore)
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <inheritdocs cref="EwEPlugin.IPlugin.Name"/>
        ''' -----------------------------------------------------------------------
        Public MustOverride ReadOnly Property PluginName As String _
            Implements EwEPlugin.IPlugin.Name

#End Region ' Plug-in implementation

#Region " Summary "

        Public MustOverride ReadOnly Property Summary As String _
             Implements EwEUtils.Core.ISummarizable.Summary

#End Region ' Summary

    End Class

End Namespace
