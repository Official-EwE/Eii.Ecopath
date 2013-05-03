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
Imports System.Collections.Generic
Imports System.Drawing
Imports System.IO
Imports System.Xml
Imports DotSpatial.Data
Imports EwECore
Imports EwEUtils.Core
Imports EwEUtils.SpatialData
Imports EwEUtils.Utilities

#End Region ' Imports

Namespace SpatialData

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' <see cref="ISpatialDataSet"/> for accessing a folder of spatio-temporal files.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cMultiFileDataSetPlugin
        Inherits cFileDataSetPlugin

#Region " Private classes "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Wrapper to maintain a file / time stamp link.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Class cTemporalFile
            Public time As DateTime
            Public file As String
            Public IndexStatus As ISpatialDataSet.eIndexStatus = ISpatialDataSet.eIndexStatus.NotIndexed
            Public ptTL As PointF
            Public ptBR As PointF

            Public Sub New(ByVal time As DateTime, ByVal strFile As String)
                Me.time = time
                Me.file = strFile
            End Sub

        End Class

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Helper class for sorting <see cref="cTemporalFile"/> instances
        ''' by time, ascending.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Class cTemporalFileSorter
            Implements IComparer(Of cTemporalFile)

            Public Function Compare(ByVal x As cTemporalFile, ByVal y As cTemporalFile) As Integer _
                Implements System.Collections.Generic.IComparer(Of cTemporalFile).Compare
                Return DateTime.Compare(x.time, y.time)
            End Function

        End Class

#End Region ' Private classes

#Region " Private vars "

        ''' <summary>List of time-indexed <see cref="cTemporalFile">files</see>.</summary>
        Private m_lFiles As List(Of cTemporalFile) = Nothing
        ''' <summary>Index in the file list for the current date.</summary>
        Protected m_iFileIndex As Integer = -1
        ''' <summary>States whether the data represents an annual pattern.</summary>
        Private m_bSeasonal As Boolean = False

        ''' <summary>Flag stating whether the dataset is sorted.</summary>
        Private m_bSorted As Boolean = False
        Private m_bCanSort As Boolean = True

#End Region ' Private vars

#Region " Construction / destruction "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Sub New()
            MyBase.New()
            Me.m_lFiles = New List(Of cTemporalFile)
            Me.m_strName = My.Resources.DATASET_MULTIPLE_NAME
        End Sub

#End Region ' Construction / destruction

#Region " Information "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the repetition interval, if any.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property IsAnnual As Boolean
            Get
                Return Me.m_bSeasonal
            End Get
            Set(ByVal value As Boolean)
                Me.m_bSeasonal = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="cFileDataSetPlugin.DisplayName" />
        ''' -------------------------------------------------------------------
        Public Overrides Property DisplayName As String
            Get
                Return Me.m_strName
            End Get
            Set(value As String)
                Me.m_strName = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="cFileDataSetPlugin.TimeSteps" />
        ''' -------------------------------------------------------------------
        Public Overrides ReadOnly Property TimeSteps As DateTime()
            Get
                Dim lTimeSteps As New List(Of DateTime)
                Me.Sort()
                For i As Integer = 0 To Me.m_lFiles.Count - 1
                    lTimeSteps.Add(Me.m_lFiles(i).time)
                Next
                Return lTimeSteps.ToArray
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="cFileDataSetPlugin.TimeEnd"/>
        ''' -------------------------------------------------------------------
        Public Overrides ReadOnly Property TimeEnd As DateTime
            Get
                If (Me.m_lFiles.Count = 0) Then Return DateTime.MinValue
                Me.Sort()
                Return Me.m_lFiles(Me.m_lFiles.Count - 1).time
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="cFileDataSetPlugin.TimeStart"/>
        ''' -------------------------------------------------------------------
        Public Overrides ReadOnly Property TimeStart As DateTime
            Get
                If (Me.m_lFiles.Count = 0) Then Return DateTime.MaxValue
                Me.Sort()
                Return Me.m_lFiles(0).time
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="cFileDataSetPlugin.DialogReadFilter"/>"
        ''' -------------------------------------------------------------------
        Public Overrides ReadOnly Property DialogReadFilter As String
            Get
                ' Support reading rasters only, for now
                Return cDotSpatialUtils.DialogFilter(True, True, False, False)
            End Get
        End Property

#End Region ' Information

#Region " Configuration "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the path to a file for a given time. Remove a file reference 
        ''' by providing an empty path.
        ''' </summary>
        ''' <param name="time"></param>
        ''' -------------------------------------------------------------------
        Public Property File(ByVal time As DateTime) As String
            Get
                Dim i As Integer = Me.FileIndex(time)
                If i = -1 Then Return ""
                Return Me.m_lFiles(i).file
            End Get
            Set(ByVal strFilePath As String)

                Dim i As Integer = Me.FileIndex(time)

                ' New file
                If (i = -1) Then
                    ' Assume the file exists; checking across the network will be too slow
                    Me.m_lFiles.Add(New cTemporalFile(time, strFilePath))
                Else
                    ' Change or remove file
                    If String.IsNullOrWhiteSpace(strFilePath) Then
                        Me.m_lFiles.RemoveAt(i)
                    Else
                        Me.m_lFiles(i).file = strFilePath
                    End If
                End If
                Me.m_bSorted = False

            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="cFileDataSetPlugin.GetConfigUI"/>
        ''' -------------------------------------------------------------------
        Public Overrides Function GetConfigUI() As Windows.Forms.Control
            Dim pg As New ucMultiFileDatasetConfigPage()
            pg.Dataset = Me
            Return pg
        End Function

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="cFileDataSetPlugin.IsConfigured"/>
        ''' -------------------------------------------------------------------
        Public Overrides Function IsConfigured() As Boolean
            Return (Me.m_lFiles.Count > 0)
        End Function

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="IExternalDataSource.IsDataAvailable"/>
        ''' -------------------------------------------------------------------
        Public Overrides Function IsDataAvailable(ByVal runtype As IRunType) As Boolean
            Return Me.IsConfigured And Me.EnableData(runtype)
        End Function

#End Region ' Configuration

#Region " Data "

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="ISpatialDataSet.HasDataAtT"/>
        ''' -------------------------------------------------------------------
        Public Overrides Function HasDataAtT(ByVal time As DateTime) As Boolean

            Dim strFile As String = ""
            Dim iFile As Integer = Me.FileIndex(time)
            If (iFile = -1) Then Return False

            strFile = Me.m_lFiles(iFile).file
            If (String.IsNullOrWhiteSpace(strFile)) Then Return False

            Return True

        End Function

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="cFileDataSetPlugin.LockDataAtT"/>
        ''' -------------------------------------------------------------------
        Public Overrides Function LockDataAtT(ByVal datetime As Date, _
                                              ByVal dCellSize As Double, _
                                              ByVal ptfTL As PointF, _
                                              ByVal ptfBR As PointF) As Boolean

            Me.m_iFileIndex = Me.FileIndex(datetime)
            If (Me.m_iFileIndex < 0) Then
                Debug.Assert(Me.m_iFileIndex >= 0, "Framework error: if a data set does not have data it should not be requested to lock data!")
                Return False
            End If

            Return MyBase.LockDataAtT(datetime, dCellSize, ptfTL, ptfBR)

        End Function

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="cFileDataSetPlugin.UnlockData"/>
        ''' -------------------------------------------------------------------
        Public Overrides Function UnlockData() As Boolean
            Me.m_iFileIndex = cCore.NULL_VALUE
            Return MyBase.UnlockData()
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Clear the dataset.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Sub Clear()
            Me.m_lFiles.Clear()
            Me.m_bSorted = False
        End Sub

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="ISpatialDataSet.GetExtentAtT"/>
        ''' -------------------------------------------------------------------
        Public Overrides Function GetExtentAtT(ByVal dt As Date, _
                                               ByRef ptfTL As System.Drawing.PointF, _
                                               ByRef ptfBR As System.Drawing.PointF) As Boolean

            Dim iFile As Integer = Me.FileIndex(dt)
            If (iFile = -1) Then Return False

            Dim f As cTemporalFile = Me.m_lFiles(iFile)
            ptfTL = f.ptTL
            ptfBR = f.ptBR
            Return (f.IndexStatus = ISpatialDataSet.eIndexStatus.Indexed)

        End Function

#End Region ' Data

#Region " Internals "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Write dataset configuration to XML.
        ''' </summary>
        ''' <param name="doc">The doc to generate nodes for.</param>
        ''' <returns>
        ''' An XML node that contains the content of the dataset.
        ''' </returns>
        ''' -------------------------------------------------------------------
        Protected Overrides Function ToXML(ByVal doc As XmlDocument) As XmlNode

            Dim xnMaster As XmlNode = Nothing
            Dim xn As XmlNode = Nothing
            Dim xnFile As XmlNode = Nothing
            Dim xaFile As XmlAttribute = Nothing

            xnMaster = doc.CreateElement("Configuration")

            xn = doc.CreateElement("Name")
            xn.InnerText = Me.DisplayName
            xnMaster.AppendChild(xn)

            xn = doc.CreateElement("Description")
            xn.InnerText = Me.Description
            xnMaster.AppendChild(xn)

            xn = doc.CreateElement("Source")
            xn.InnerText = Me.Source
            xnMaster.AppendChild(xn)

            xn = doc.CreateElement("Annual")
            xn.InnerText = Convert.ToString(Me.IsAnnual)
            xnMaster.AppendChild(xn)

            xn = doc.CreateElement("Files")
            xnMaster.AppendChild(xn)

            For Each tf As cTemporalFile In Me.m_lFiles

                xnFile = doc.CreateElement("File")

                xaFile = doc.CreateAttribute("Name")
                'jb 3-May-2012 still need to confirm files are relative to the "Source" node
                'not what-every it is that cFileUtils.RelativePath returns 
                'so strip of the path part of the file
                xaFile.Value = Path.GetFileName(tf.file)
                xnFile.Attributes.Append(xaFile)

                xaFile = doc.CreateAttribute("Date")
                xaFile.Value = Convert.ToString(tf.time.ToOADate)
                xnFile.Attributes.Append(xaFile)

                xaFile = doc.CreateAttribute("Indexed")
                xaFile.Value = Convert.ToString(tf.IndexStatus = ISpatialDataSet.eIndexStatus.Indexed)
                xnFile.Attributes.Append(xaFile)

                If (tf.IndexStatus = ISpatialDataSet.eIndexStatus.Indexed) Then

                    xaFile = doc.CreateAttribute("lonmin")
                    xaFile.Value = cStringUtils.FormatSingle(tf.ptTL.X)
                    xnFile.Attributes.Append(xaFile)

                    xaFile = doc.CreateAttribute("lonmax")
                    xaFile.Value = cStringUtils.FormatSingle(tf.ptBR.X)
                    xnFile.Attributes.Append(xaFile)

                    xaFile = doc.CreateAttribute("latmin")
                    xaFile.Value = cStringUtils.FormatSingle(tf.ptBR.Y)
                    xnFile.Attributes.Append(xaFile)

                    xaFile = doc.CreateAttribute("latmax")
                    xaFile.Value = cStringUtils.FormatSingle(tf.ptTL.Y)
                    xnFile.Attributes.Append(xaFile)

                End If

                xn.AppendChild(xnFile)

            Next

            Return xnMaster

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Read dataset configuration from XML.
        ''' </summary>
        ''' <param name="doc">The doc to read nodes from.</param>
        ''' <param name="node">The configuration node that contains the content
        ''' of the dataset. Happy, happy, happy.</param>
        ''' <returns>
        ''' True if successful.
        ''' </returns>
        ''' -------------------------------------------------------------------
        Protected Overrides Function FromXML(ByVal doc As XmlDocument, ByVal node As XmlNode) As Boolean

            Dim xn As XmlNode = Nothing
            Dim xnFile As XmlNode = Nothing
            Dim xaFile As XmlAttribute = Nothing

            If (String.Compare(node.Name, "Configuration") <> 0) Then Return False

            Try
                Me.m_bCanSort = False
                For Each xn In node.ChildNodes
                    Select Case xn.Name
                        Case "Name" : Me.m_strName = xn.InnerText
                        Case "Description" : Me.m_strDescription = xn.InnerText
                        Case "Source" : Me.m_strSource = xn.InnerText
                        Case "Annual" : Me.m_bSeasonal = Convert.ToBoolean(xn.InnerText)
                        Case "Files"
                            For Each xnFile In xn.ChildNodes
                                Dim strName As String = xnFile.Attributes("Name").InnerText
                                Dim strDate As String = xnFile.Attributes("Date").InnerText
                                Dim dt As DateTime = DateTime.FromOADate(Convert.ToDouble(strDate))
                                Dim f As New cTemporalFile(dt, Path.Combine(Me.m_strSource, strName))
                                f.IndexStatus = ISpatialDataSet.eIndexStatus.NotIndexed
                                If (xnFile.Attributes.GetNamedItem("Indexed") IsNot Nothing) Then
                                    If (Boolean.Parse(xnFile.Attributes("Indexed").InnerText)) Then
                                        f.IndexStatus = ISpatialDataSet.eIndexStatus.Indexed
                                        f.ptTL = New PointF(CSng(cStringUtils.ConvertToNumber(xnFile.Attributes("lonmin").InnerText, GetType(Single))), _
                                                            CSng(cStringUtils.ConvertToNumber(xnFile.Attributes("latmax").InnerText, GetType(Single))))
                                        f.ptBR = New PointF(CSng(cStringUtils.ConvertToNumber(xnFile.Attributes("lonmax").InnerText, GetType(Single))), _
                                                            CSng(cStringUtils.ConvertToNumber(xnFile.Attributes("latmin").InnerText, GetType(Single))))
                                    End If
                                End If
                                Me.m_lFiles.Add(f)
                             Next
                    End Select
                Next
                Me.m_bCanSort = True

            Catch ex As Exception
                Me.Clear()
                Return False
            End Try

            Return True

        End Function

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="cFileDataSetPlugin.StoreExtent"/>
        ''' -------------------------------------------------------------------
        Protected Overrides Function StoreExtent(ByVal ext As IExtent) As Boolean

            Debug.Assert(Me.m_iFileIndex >= 0)
            Debug.Assert(Me.m_iFileIndex < Me.m_lFiles.Count)

            Dim f As cTemporalFile = Me.m_lFiles(Me.m_iFileIndex)

            If (ext IsNot Nothing) Then
                f.ptTL = New PointF(CSng(ext.MinX), CSng(ext.MaxY))
                f.ptBR = New PointF(CSng(ext.MaxX), CSng(ext.MinY))
                f.IndexStatus = ISpatialDataSet.eIndexStatus.Indexed
            Else
                f.IndexStatus = ISpatialDataSet.eIndexStatus.Failed
            End If

            Return True

        End Function

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="cFileDataSetPlugin.SourceFileName"/>
        ''' -------------------------------------------------------------------
        Protected Overrides Function SourceFileName() As String

            Debug.Assert(Me.m_iFileIndex >= 0)
            Debug.Assert(Me.m_iFileIndex < Me.m_lFiles.Count)

            Return Me.m_lFiles(Me.m_iFileIndex).file

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Return the full path file name where to store reprojected, trimmed and 
        ''' interpolated data.
        ''' </summary>
        ''' <returns>A file name to store the raster in.</returns>
        ''' <remarks>
        ''' If a <see cref="Cache"/> is provided the file name will point at the
        ''' relevant Cache path. If not, a temporary file is generated.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Protected Overrides Function CacheFileName(ByVal strLayerName As String) As String

            Dim c As ISpatialDataCache = Me.Cache
            Dim strExt As String = cDotSpatialUtils.DefaultCacheExtension()

            If (c IsNot Nothing) Then
                Return c.GetFileName(Me, _
                                     cDotSpatialUtils.TopLeft(Me.m_extModelArea), _
                                     cDotSpatialUtils.BottomRight(Me.m_extModelArea), _
                                     Me.m_dModelCellSize, _
                                     Me.m_lFiles(Me.m_iFileIndex).time, _
                                     strLayerName, strExt)
            End If
            Return cFileUtils.MakeTempFile(strExt)

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns the index of a file in the local list.
        ''' </summary>
        ''' <param name="time"></param>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Private Function FileIndex(ByVal time As DateTime) As Integer

            Dim t As DateTime

            For i As Integer = 0 To Me.m_lFiles.Count - 1
                t = Me.m_lFiles(i).time
                If (time = t) Then Return i
            Next i
            Return -1

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Sort the list of files by time. For the sort algorithm see
        ''' <see cref="cTemporalFileSorter"/>
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub Sort()
            If (Me.m_bSorted Or Not Me.m_bCanSort) Then Return
            Me.m_lFiles.Sort(New cTemporalFileSorter())
            Me.m_bSorted = True
        End Sub

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="cFileDataSetPlugin.FractionIndexed"/>
        ''' -------------------------------------------------------------------
        Protected Overrides Function FractionIndexed() As Single

            If (Me.m_lFiles.Count = 0) Then Return 0
            Dim iNumIndexed As Integer = 0

            For Each f As cTemporalFile In Me.m_lFiles
                If (f.IndexStatus = ISpatialDataSet.eIndexStatus.Indexed) Then iNumIndexed += 1
            Next
            Return CSng(iNumIndexed / Me.m_lFiles.Count)

        End Function

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="cFileDataSetPlugin.IndexStatusAtT"/>
        ''' -------------------------------------------------------------------
        Protected Overrides Function IndexStatusAtT(dt As Date) As ISpatialDataSet.eIndexStatus

            If (Me.m_lFiles.Count = 0) Then Return ISpatialDataSet.eIndexStatus.Indexed
            Dim iFile As Integer = Me.FileIndex(dt)

            If (iFile = -1) Then Return ISpatialDataSet.eIndexStatus.NotIndexed
            Return Me.m_lFiles(iFile).IndexStatus

        End Function

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="cFileDataSetPlugin.BuildIndex"/>
        ''' -------------------------------------------------------------------
        Protected Overrides Sub BuildIndex(ByVal dateStart As DateTime, _
                                           ByVal dateEnd As DateTime, _
                                           Optional ByVal dgt As ISpatialDataSet.BuildIndexUpdateDelegate = Nothing)

            Dim ptfTL As New PointF(-180, 90)
            Dim ptfBR As New PointF(180, -90)
            Dim f As cTemporalFile = Nothing

            ' Truncate dates
            If (Me.m_lFiles.Count > 0) Then
                If dateStart < Me.m_lFiles(0).time Then dateStart = Me.m_lFiles(0).time
                If dateEnd > Me.m_lFiles(Me.m_lFiles.Count - 1).time Then dateEnd = Me.m_lFiles(Me.m_lFiles.Count - 1).time
            End If

            Dim iStart As Integer = Me.FileIndex(dateStart)
            Dim iEnd As Integer = Me.FileIndex(dateEnd)

            For i As Integer = Math.Max(0, iStart) To Math.Min(Me.m_lFiles.Count - 1, iEnd)
                f = Me.m_lFiles(i Mod Me.m_lFiles.Count)
                If (f.IndexStatus <> ISpatialDataSet.eIndexStatus.Indexed) Then

                    ' Limit cache access
                    Dim bOldFlag = Me.ReadFromCache
                    Me.ReadFromCache = False

                    Try
                        If Me.LockDataAtT(f.time, 1.0!, ptfTL, ptfBR) Then
                            Me.LoadSource()
                            dgt.Invoke(Me)
                            Me.UnlockData()
                        End If
                    Catch ex As Exception
                        cLog.Write(ex, "cMultiFileDatasetPlugin::BuildIndex")
                    End Try

                    ' Restore cache access
                    Me.ReadFromCache = bOldFlag
                End If
            Next

        End Sub

#End Region ' Internals

#Region " Plug-in implementation "

        ''' -----------------------------------------------------------------------
        ''' <inheritdocs cref="EwEPlugin.IPlugin.Description"/>
        ''' -----------------------------------------------------------------------
        Public Overrides ReadOnly Property PluginDescription As String
            Get
                Return My.Resources.DATASET_MULTIPLE_DESCR
            End Get
        End Property

        ''' -----------------------------------------------------------------------
        ''' <inheritdocs cref="EwEPlugin.IPlugin.Name"/>
        ''' -----------------------------------------------------------------------
        Public Overrides ReadOnly Property PluginName As String
            Get
                Return "DotSpatial.MultiFileSetPlugin"
            End Get
        End Property

#End Region ' Plug-in implementation

    End Class

End Namespace
