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
Imports EwEPlugin
Imports EwECore
Imports EwEUtils.Utilities
Imports System.Xml
Imports EwEUtils.SpatialData
Imports System.IO
Imports EwEUtils.Core
Imports System.Windows.Forms

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Dataset that provides access to a series of pre-prepared CSV map files.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cEwECSVMapDataset
    Implements ISpatialDataSetPlugin
    Implements IConfigurable

#Region " Private vars "

    Private m_core As cCore = Nothing
    Private m_strFolder As String = ""
    Private m_strName As String = ""
    Private m_iFileIndex As Integer = -1
    Private m_raster As cCSVRaster = Nothing

    Private m_bHasHeaderColRow As Boolean = True
    Private m_files As New List(Of String)

#End Region ' Private vars

#Region " Construction / destruction "

    Public Sub New()
        Me.DBID = Guid.Empty
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
    Public Property DisplayName As String _
        Implements ISpatialDataSet.DisplayName
        Get
            If (Not String.IsNullOrWhiteSpace(Me.m_strName)) Then
                Return Me.m_strName
            End If
            If (Not String.IsNullOrWhiteSpace(Me.Source)) Then
                Return cStringUtils.Localize("CSV fiels {0}", Path.GetFileName(Me.Source))
            End If
            Return Me.Description
        End Get
        Set(value As String)
            Me.m_strName = value
        End Set
    End Property

    ''' -------------------------------------------------------------------
    ''' <inheritdocs cref="ISpatialDataSet.DataDescription" />
    ''' -------------------------------------------------------------------
    Public Property DataDescription As String _
        Implements ISpatialDataSet.DataDescription

    ''' -------------------------------------------------------------------
    ''' <inheritdocs cref="IPlugin.Description" />
    ''' -------------------------------------------------------------------
    Public ReadOnly Property Description As String _
        Implements IPlugin.Description
        Get
            Return "CSV files prepared for a specific basemap"
        End Get
    End Property

    ''' -------------------------------------------------------------------
    ''' <inheritdocs cref="ISpatialDataSet.TimeStart"/>
    ''' -------------------------------------------------------------------
    Public ReadOnly Property TimeStart As DateTime _
        Implements ISpatialDataSet.TimeStart
        Get
            Return Me.m_core.EcospaceTimestepToAbsoluteTime(1)
        End Get
    End Property

    ''' -------------------------------------------------------------------
    ''' <inheritdocs cref="ISpatialDataSet.TimeEnd"/>
    ''' -------------------------------------------------------------------
    Public ReadOnly Property TimeEnd As DateTime _
        Implements ISpatialDataSet.TimeEnd
        Get
            If (Me.m_files.Count = 0) Then Return Date.MinValue
            If (Me.IsRepeating) Then Return Me.m_core.EcospaceTimestepToAbsoluteTime(Me.m_core.nEcospaceTimeSteps)
            Return Me.m_core.EcospaceTimestepToAbsoluteTime(Me.m_files.Count)
        End Get
    End Property

    ''' -------------------------------------------------------------------
    ''' <inheritdocs cref="ISpatialDataSet.DialogReadFilter"/>"
    ''' -------------------------------------------------------------------
    Public ReadOnly Property DialogReadFilter(ByVal bRaster As Boolean,
                                              ByVal bImage As Boolean,
                                              ByVal bVector As Boolean) As String _
         Implements ISpatialDataSet.DialogReadFilter
        Get
            Return ScientificInterfaceShared.My.Resources.FILEFILTER_CSV
        End Get
    End Property

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
    Public ReadOnly Property ConversionFormat As String _
        Implements EwEUtils.SpatialData.ISpatialDataSet.ConversionFormat
        Get
            Return ""
        End Get
    End Property

    Public ReadOnly Property Author As String _
        Implements EwEPlugin.IPlugin.Author
        Get
            Return "Jeroen Steenbeek"
        End Get
    End Property

    Public ReadOnly Property Contact As String _
        Implements EwEPlugin.IPlugin.Contact
        Get
            Return "mailto:ewedevteam@gmail.com"
        End Get
    End Property

    Public Sub Initialize(core As Object) _
        Implements EwEPlugin.IPlugin.Initialize
        Try
            Me.m_core = DirectCast(core, cCore)
        Catch ex As Exception
            cLog.Write(ex, "cEwECSVMapDataset")
        End Try
    End Sub

    Public ReadOnly Property Name As String _
        Implements EwEPlugin.IPlugin.Name
        Get
            Return "Spatial.CSVMapDataset"
        End Get
    End Property

#End Region ' Information

#Region " Configuration "

    ''' -------------------------------------------------------------------
    ''' <inheritdocs cref="ISpatialDataSet.Configuration"/>
    ''' -------------------------------------------------------------------
    Public Property Configuration(doc As System.Xml.XmlDocument, strFolderRoot As String) As System.Xml.XmlNode _
        Implements ISpatialDataSet.Configuration
        Get
            Return Me.ToXML(doc, strFolderRoot)
        End Get
        Set(ByVal value As XmlNode)
            Me.FromXML(doc, value, strFolderRoot)
        End Set
    End Property

    ''' -------------------------------------------------------------------
    ''' <inheritdocs cref="IConfigurablePlugin.IsConfigured"/>
    ''' -------------------------------------------------------------------
    Public Function IsConfigured() As Boolean _
        Implements ISpatialDataSet.IsConfigured, IConfigurable.IsConfigured
        Return Not String.IsNullOrWhiteSpace(Me.Source)
    End Function

    ''' -------------------------------------------------------------------
    ''' <inheritdocs cref="IExternalDataSource.EnableData"/>
    ''' -------------------------------------------------------------------
    Public Property EnableData(runtype As EwEUtils.Core.IRunType) As Boolean _
        Implements IExternalDataSource.EnableData
        Get
            Return True
        End Get
        Set(value As Boolean)
            ' NOP
        End Set
    End Property

    Public Function IsDataAvailable(runtype As EwEUtils.Core.IRunType) As Boolean _
        Implements EwEUtils.Core.IExternalDataSource.IsDataAvailable
        Return (Me.m_files.Count > 0)
    End Function

    Public Property IsRepeating As Boolean = False

    Friend ReadOnly Property Files As List(Of String)
        Get
            Return Me.m_files
        End Get
    End Property

#End Region ' Configuration

#Region " Import / export "

    Public Function ExportTo(strPath As String) As ISpatialDataSet _
        Implements ISpatialDataSet.ExportTo
        Return Nothing
    End Function

#End Region ' Import / export

#Region " Metadata "

    Public Function GetAttributeDataTypes() As System.Type() _
        Implements ISpatialDataSet.GetAttributeDataTypes
        Return Nothing
    End Function

    Public Function GetAttributes() As String() _
        Implements ISpatialDataSet.GetAttributes
        Return Nothing
    End Function

    Public Function GetAttributeValues() As System.Data.DataTable _
        Implements ISpatialDataSet.GetAttributeValues
        Return Nothing
    End Function

#End Region ' Metadata

#Region " Data "

    Public Function GetRaster(converter As ISpatialDataConverter, strLayerName As String) As ISpatialRaster _
        Implements ISpatialDataSet.GetRaster

        If (Not Me.IsLocked) Then Return Nothing
        Me.LoadSource()
        Return Me.m_raster

    End Function

    Public Function GetExtentAtT(datetime As Date, ByRef ptfNW As System.Drawing.PointF, ByRef ptfSE As System.Drawing.PointF) As Boolean _
        Implements ISpatialDataSet.GetExtentAtT

        Dim bm As cEcospaceBasemap = Me.m_core.EcospaceBasemap
        ptfNW = bm.PosTopLeft
        ptfSE = bm.PosBottomRight
        Return True

    End Function

    Public Property Cache As ISpatialDataCache _
        Implements ISpatialDataSet.Cache

    Public Function HasDataAtT(datetime As Date) As Boolean _
        Implements ISpatialDataSet.HasDataAtT

        Return (Me.FileIndex(datetime) <> -1)

    End Function

    Public Function IndexStatusAtT(datetime As Date) As ISpatialDataSet.eIndexStatus _
        Implements ISpatialDataSet.IndexStatusAtT
        Return ISpatialDataSet.eIndexStatus.Indexed
    End Function

    ''' -------------------------------------------------------------------
    ''' <inheritdocs cref="ISpatialDataSet.Source" />
    ''' -------------------------------------------------------------------
    Public Property Source As String _
        Implements ISpatialDataSet.Source
        Get
            Return Me.m_strFolder
        End Get
        Set(value As String)
            Me.m_strFolder = value
            Me.m_files.Clear()
            Me.m_files.AddRange(Me.Read(Me.m_strFolder))
        End Set
    End Property

    Public Function LockDataAtT(datetime As Date, dCellSize As Double, ptfNE As System.Drawing.PointF, ptfSW As System.Drawing.PointF, strProjectionString As String) As Boolean _
        Implements ISpatialDataSet.LockDataAtT

        Me.m_iFileIndex = Me.FileIndex(datetime)
        Return Me.IsLocked()

    End Function

    Public Function IsLocked() As Boolean _
        Implements ISpatialDataSet.IsLocked

        Return (Me.m_iFileIndex >= 0)

    End Function

    Public Function Unlock() As Boolean _
        Implements ISpatialDataSet.Unlock

        Me.m_iFileIndex = -1
        Me.m_raster = Nothing

        Return Not Me.IsLocked()

    End Function

    Public Sub UpdateIndexAtT(datetime As Date) _
        Implements ISpatialDataSet.UpdateIndexAtT

        ' NOP

    End Sub

#End Region ' Data

#Region " Summary "

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="ISpatialDataSet.Summary"/>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Summary As String _
        Implements EwEUtils.Core.ISummarizable.Summary
        Get
            Return cEncryptionUtilities.MD5(Me.Source)
        End Get
    End Property

#End Region ' Summary

#Region " Internals "

    Public Function Read(strFolder As String) As String()
        Try
            If Directory.Exists(Me.Source) Then
                Return System.IO.Directory.GetFiles(strFolder, "*.csv")
            End If
        Catch ex As Exception

        End Try
        Return New String() {}
    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Write content to XML.
    ''' </summary>
    ''' <param name="doc">The doc to generate nodes for.</param>
    ''' <returns>
    ''' An XML node that contains the content of the dataset.
    ''' </returns>
    ''' -------------------------------------------------------------------
    Protected Function ToXML(ByVal doc As XmlDocument,
                             ByVal strFolderRoot As String) As XmlNode

        Dim xnMaster As XmlNode = Nothing
        Dim xn As XmlNode = Nothing
        Dim xnFile As XmlNode = Nothing
        Dim xaFile As XmlAttribute = Nothing
        Dim cin As cCoreEnumNamesIndex = cCoreEnumNamesIndex.GetInstance()

        xnMaster = doc.CreateElement("Configuration")

        xn = doc.CreateElement("Name")
        xn.InnerText = Me.DisplayName
        xnMaster.AppendChild(xn)

        xn = doc.CreateElement("Description")
        xn.InnerText = Me.DataDescription
        xnMaster.AppendChild(xn)

        xn = doc.CreateElement("Variable")
        xn.InnerText = cin.GetVarName(Me.VarName)
        xnMaster.AppendChild(xn)

        xnFile = doc.CreateElement("Source")
        xnFile.InnerText = Me.Source

        xnMaster.AppendChild(xnFile)

        Return xnMaster

    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Read content from XML.
    ''' </summary>
    ''' <param name="doc">The doc to read nodes from.</param>
    ''' <param name="node">The configuration node that contains the content
    ''' of the dataset. Happy, happy, happy.</param>
    ''' <returns>
    ''' True if successful.
    ''' </returns>
    ''' -------------------------------------------------------------------
    Protected Function FromXML(ByVal doc As XmlDocument,
                               ByVal node As XmlNode,
                               ByVal strFolderRoot As String) As Boolean

        Dim xn As XmlNode = Nothing
        Dim cin As cCoreEnumNamesIndex = cCoreEnumNamesIndex.GetInstance()

        If (String.Compare(node.Name, "Configuration") <> 0) Then Return False

        Try

            For Each xn In node.ChildNodes
                Select Case xn.Name
                    Case "Name"
                        Me.DisplayName = xn.InnerText

                    Case "Description"
                        Me.DataDescription = xn.InnerText

                    Case "Variable"
                        Me.VarName = cin.GetVarName(xn.InnerText)

                    Case "Source"
                        Me.Source = xn.InnerText

                End Select
            Next

        Catch ex As Exception
            Return False
        End Try

        Return True

    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Returns the index of a file in the local list.
    ''' </summary>
    ''' <param name="time"></param>
    ''' <returns></returns>
    ''' -------------------------------------------------------------------
    Private Function FileIndex(ByVal time As DateTime) As Integer

        Dim i As Integer = Me.m_core.AbsoluteTimeToEcospaceTimestep(time) - 1
        If (i >= 0) And (i < Me.m_core.nEcospaceTimeSteps) Then
            If (Me.IsRepeating) Then Return (i Mod Me.m_files.Count)
            If (i < Me.m_files.Count) Then Return i
        End If
        Return -1

    End Function

    Protected Function LoadSource() As Boolean

        Dim bm As cEcospaceBasemap = Me.m_core.EcospaceBasemap
        Dim strFileName As String = Me.m_files(Me.m_iFileIndex)
        Dim rs As New cCSVRaster(bm)
        Dim reader As StreamReader = Nothing
        Dim line As String = ""
        Dim bits As String() = Nothing

        ' Already read? Ok!
        If (Me.m_raster IsNot Nothing) Then Return True

        ' File missing?
        If (Not System.IO.File.Exists(strFileName)) Then
            ' #Yes: report error
            'Me.LogMessage(cStringUtils.Localize(My.Resources.STATUS_LOAD_FILENOTFOUND, strFileName), eStatusFlags.MissingParameter)
            ' Run away
            Return False
        End If

        Try
            ' Try to get reader
            reader = New StreamReader(strFileName)
        Catch ex As Exception
            ' Panic!
            'Me.LogMessage(cStringUtils.Localize(My.Resources.STATUS_LOAD_FAILED, ex.Message), eStatusFlags.MissingParameter)
            Return False
        End Try

        Try
            For i As Integer = 1 To bm.InRow
                line = reader.ReadLine()
                bits = cStringUtils.SplitQualified(line, ",")
                For j As Integer = 1 To bm.InCol
                    If j < bits.Length Then
                        Double.TryParse(bits(j), rs.Data(i, j))
                    Else
                        rs.Data(i, j) = rs.NoData
                    End If
                Next
            Next
            Me.m_raster = rs

        Catch ex As Exception
            ' Log generic panic message
            'Me.LogMessage(cStringUtils.Localize(My.Resources.STATUS_LOAD_FAILED, ex.Message), eStatusFlags.MissingParameter)
        End Try

        ' Clean up
        reader.Close()

        ' Report all over success
        Return (Me.m_raster IsNot Nothing)

    End Function

#End Region ' Internals

#Region " Configuration "

    Public Function GetConfigUI() As System.Windows.Forms.Control _
        Implements EwEUtils.Core.IConfigurable.GetConfigUI

        Return New dlgConfig(Me)

    End Function

#End Region ' Configuration

End Class
