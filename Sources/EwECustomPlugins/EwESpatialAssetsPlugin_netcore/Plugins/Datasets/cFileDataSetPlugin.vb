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

#Region " Imports "

Option Strict On
Imports System.Data
Imports System.Drawing
Imports System.IO
Imports System.Xml
Imports EwECore
Imports EwECore.SpatialData
Imports EwEPlugin
Imports EwEUtils.Core
Imports EwEUtils.SpatialData
Imports EwEUtils.Utilities

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

        ''' <summary>Date that the loaded data represents.</summary>
        Protected m_dtTime As DateTime = Nothing
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
        ''' <inheritdocs cref="ISpatialDataSet.CustomName" />
        ''' -------------------------------------------------------------------
        Public MustOverride Property CustomName As String _
            Implements ISpatialDataSet.CustomName

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="ISpatialDataSet.CustomDescription" />
        ''' -------------------------------------------------------------------
        Public Overridable Property CustomDescription As String _
            Implements ISpatialDataSet.CustomDescription

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
        Public ReadOnly Property DialogReadFilter(bRaster As Boolean,
                                                  bImage As Boolean,
                                                  bVector As Boolean,
                                                  bAllFiles As Boolean) As String _
             Implements ISpatialDataSet.DialogReadFilter
            Get
                Return ""
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return Me.CustomName()
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
        Public Property Configuration(doc As XmlDocument,
                                      strFolderRoot As String) As XmlNode _
            Implements ISpatialDataSet.Configuration
            Get
                Return Me.ToXML(doc, strFolderRoot)
            End Get
            Set(value As XmlNode)
                Me.FromXML(doc, value, strFolderRoot)
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="IConfigurablePlugin.GetConfigUI"/>
        ''' -------------------------------------------------------------------
        Public MustOverride Function GetConfigUI() As Object _
            Implements IConfigurablePlugin.GetConfigUI

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="IConfigurablePlugin.IsConfigured"/>
        ''' -------------------------------------------------------------------
        Public MustOverride Function IsConfigured() As Boolean _
            Implements IConfigurablePlugin.IsConfigured, ISpatialDataSet.IsConfigured

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="IExternalDataSource.EnableData"/>
        ''' -------------------------------------------------------------------
        Public Property EnableData(runtype As IRunType) As Boolean _
            Implements IExternalDataSource.EnableData
            Get
                Return True
            End Get
            Set(value As Boolean)
                ' NOP
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="IExternalDataSource.IsDataAvailable"/>
        ''' -------------------------------------------------------------------
        Public MustOverride Function IsDataAvailable(runtype As IRunType) As Boolean _
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
        Protected MustOverride Function ToXML(doc As XmlDocument,
                                              strFolderRoot As String) As XmlNode

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
        Protected MustOverride Function FromXML(doc As XmlDocument,
                                                node As XmlNode,
                                                strFolderRoot As String) As Boolean

#End Region ' Configuration

#Region " Import / export "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set whether files are found on a path, dictated by source,
        ''' relative to the current configuration file.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property IsSourceRelative As Boolean
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
        Public MustOverride Function ExportTo(strPath As String) As ISpatialDataSet _
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
        Protected Function ToAbsolutePath(strPath As String, strPathBase As String) As String

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
        Protected Function ToRelativePath(strPath As String, strPathBase As String) As String

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
        Public Overridable Function GetRaster(converter As ISpatialDataConverter,
                                              strLayerName As String) As ISpatialRaster _
            Implements ISpatialDataSet.GetRaster
            Return Nothing
        End Function

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="ISpatialDataSet.HasDataAtT"/>
        ''' -------------------------------------------------------------------
        Public MustOverride Function HasDataAtT(datetime As DateTime) As Boolean _
            Implements ISpatialDataSet.HasDataAtT

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="ISpatialDataSet.GetExtentAtT"/>
        ''' -------------------------------------------------------------------
        Public MustOverride Function GetExtentAtT(datetime As DateTime,
                                                  ByRef ptfTL As PointF,
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
            Return True
        End Function

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="ISpatialDataSet.LockDataAtT"/>
        ''' -------------------------------------------------------------------
        Public Overridable Function LockDataAtT(datetime As Date,
                                                dCellSize As Double,
                                                ptfTL As System.Drawing.PointF,
                                                ptfBR As System.Drawing.PointF,
                                                strProjectionString As String) As Boolean _
            Implements EwEUtils.SpatialData.ISpatialDataSet.LockDataAtT
            Return True

        End Function

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="ISpatialDataSet.Unlock"/>
        ''' -------------------------------------------------------------------
        Public Overridable Function UnlockData() As Boolean _
            Implements EwEUtils.SpatialData.ISpatialDataSet.Unlock
            Return True

        End Function

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="ISpatialDataSet.GetAttributes"/>
        ''' -------------------------------------------------------------------
        Public Function GetAttributes() As String() _
            Implements EwEUtils.SpatialData.ISpatialDataSet.GetAttributes

            Dim lstrAttributes As New List(Of String)
            Return lstrAttributes.ToArray()

        End Function

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="ISpatialDataSet.GetAttributeDataTypes"/>
        ''' -------------------------------------------------------------------
        Public Function GetAttributeDataTypes() As Type() _
             Implements ISpatialDataSet.GetAttributeDataTypes

            Dim ltAttributes As New List(Of Type)
            Return ltAttributes.ToArray()

        End Function

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="ISpatialDataSet.GetAttributeValues"/>
        ''' -------------------------------------------------------------------
        Public Function GetAttributeValues() As DataTable _
            Implements EwEUtils.SpatialData.ISpatialDataSet.GetAttributeValues

            Return Nothing

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
                Return "Ecopath International Initiative Research Association"
            End Get
        End Property

        ''' -----------------------------------------------------------------------
        ''' <inheritdocs cref="EwEPlugin.IPlugin.Contact"/>
        ''' -----------------------------------------------------------------------
        Public ReadOnly Property Contact As String _
            Implements EwEPlugin.IPlugin.Contact
            Get
                Return "mailto:ecopathinternational@gmail.com"
            End Get
        End Property

        ''' -----------------------------------------------------------------------
        ''' <inheritdocs cref="EwEPlugin.IPlugin.Initialize"/>
        ''' -----------------------------------------------------------------------
        Public Sub Initialize(core As Object) _
            Implements EwEPlugin.IPlugin.Initialize
            Me.m_core = DirectCast(core, cCore)
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <inheritdocs cref="EwEPlugin.IPlugin.Name"/>
        ''' -----------------------------------------------------------------------
        Public MustOverride ReadOnly Property PluginName As String _
            Implements EwEPlugin.IPlugin.Name

        ''' -----------------------------------------------------------------------
        ''' <inheritdocs cref="EwEPlugin.IPlugin.DisplayName"/>
        ''' -----------------------------------------------------------------------
        Public MustOverride ReadOnly Property PluginDisplayName As String _
            Implements EwEPlugin.IPlugin.DisplayName

#End Region ' Plug-in implementation

#Region " Summary "

        Public MustOverride ReadOnly Property Summary As String _
             Implements EwEUtils.Core.ISummarizable.Summary

#End Region ' Summary

    End Class

End Namespace
