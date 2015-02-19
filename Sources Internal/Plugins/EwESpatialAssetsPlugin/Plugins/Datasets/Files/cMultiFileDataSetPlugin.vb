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
Imports System.Drawing
Imports System.IO
Imports System.Text
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
        ''' <remarks>
        ''' Declared as Protected so this class can be Inherited
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Protected Class cTemporalFile
            Public Property [Date] As DateTime
            Public Property FileName As String
            Public Property IndexStatus As ISpatialDataSet.eIndexStatus = ISpatialDataSet.eIndexStatus.NotIndexed
            Public Property TopLeft As PointF
            Public Property BottomRight As PointF

            Public Sub New(ByVal dt As DateTime, ByVal strFile As String)
                Me.Date = dt
                Me.FileName = strFile
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
                Return DateTime.Compare(x.Date, y.Date)
            End Function

        End Class

#End Region ' Private classes

#Region " Private vars "

        ''' <summary>List of time-indexed <see cref="cTemporalFile">files</see>.</summary>
        Protected m_lFiles As List(Of cTemporalFile) = Nothing
        ''' <summary>Index in the file list for the current date.</summary>
        Protected m_iFileIndex As Integer = -1

        ''' <summary>Flag stating whether the dataset is sorted.</summary>
        Protected m_bSorted As Boolean = False
        Protected m_bCanSort As Boolean = True

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
            Me.VarName = eVarNameFlags.NotSet
        End Sub

#End Region ' Construction / destruction

#Region " Information "

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
                    lTimeSteps.Add(Me.m_lFiles(i).Date)
                Next
                Return lTimeSteps.ToArray
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="cFileDataSetPlugin.TimeEnd"/>
        ''' -------------------------------------------------------------------
        Public Overrides ReadOnly Property TimeEnd As DateTime
            Get
                If Me.IsSeasonal Then
                    Return Me.SeasonsEnd
                End If
                If (Me.m_lFiles.Count = 0) Then Return DateTime.MinValue
                Me.Sort()
                Return Me.m_lFiles(Me.m_lFiles.Count - 1).Date
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="cFileDataSetPlugin.TimeStart"/>
        ''' -------------------------------------------------------------------
        Public Overrides ReadOnly Property TimeStart As DateTime
            Get
                If (Me.m_lFiles.Count = 0) Then Return DateTime.MaxValue
                Me.Sort()
                Return Me.m_lFiles(0).Date
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="cFileDataSetPlugin.DialogReadFilter"/>"
        ''' -------------------------------------------------------------------
        Public Overrides ReadOnly Property DialogReadFilter(ByVal bRaster As Boolean, _
                                                            ByVal bImage As Boolean, _
                                                            ByVal bVector As Boolean) As String
            Get
                Return cDotSpatialUtils.DialogFilter(True, bRaster, bImage, bVector)
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="cFileDataSetPlugin.ConversionFormat"/>
        ''' -------------------------------------------------------------------
        Public Overrides ReadOnly Property ConversionFormat() As String
            Get
                If (Me.m_lFiles.Count = 0) Then Return ""
                Return cDotSpatialUtils.GetDataFormat(Me.m_lFiles(0).FileName)
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
                Return Me.m_lFiles(i).FileName
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
                        Me.m_lFiles(i).FileName = strFilePath
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

        ''' -------------------------------------------------------------------
        ''' <summary>Get/set whether the dataset repeats seasonally.</summary>
        ''' -------------------------------------------------------------------
        Public Property IsSeasonal As Boolean

        ''' -------------------------------------------------------------------
        ''' <summary>Get/set whether date where 'seasonality' ends.</summary>
        ''' <remarks>Thanks, Marillion ;)</remarks>
        ''' -------------------------------------------------------------------
        Public Property SeasonsEnd As DateTime = New DateTime(2100, 1, 1)

#End Region ' Configuration

#Region " Data "

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="ISpatialDataSet.HasDataAtT"/>
        ''' -------------------------------------------------------------------
        Public Overrides Function HasDataAtT(ByVal time As DateTime) As Boolean

            Dim strFile As String = ""
            Dim iFile As Integer = Me.FileIndex(time)
            If (iFile = -1) Then Return False

            strFile = Me.m_lFiles(iFile).FileName
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
            ptfTL = f.TopLeft
            ptfBR = f.BottomRight
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
            Dim xa As XmlAttribute = Nothing
            Dim xnChild As XmlNode = Nothing
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

            xn = doc.CreateElement("Source")
            xa = doc.CreateAttribute("IsRelative")
            If (Me.IsSourceRelative) Then
                xn.InnerText = Me.ToRelativePath(Me.Source)
                xa.Value = Convert.ToString(True)
            Else
                xn.InnerText = Me.Source
                xa.Value = Convert.ToString(False)
            End If
            xn.Attributes.Append(xa)
            xnMaster.AppendChild(xn)

            xn = doc.CreateElement("Seasonal")
            xn.InnerText = Convert.ToString(Me.IsSeasonal)
            xa = doc.CreateAttribute("EndDate")
            xa.InnerText = cStringUtils.FormatDate(Me.SeasonsEnd)
            xn.Attributes.Append(xa)
            xnMaster.AppendChild(xn)

            xn = doc.CreateElement("Files")
            xnMaster.AppendChild(xn)

            For Each tf As cTemporalFile In Me.m_lFiles

                Dim strFile As String = tf.FileName

                xnChild = doc.CreateElement("File")

                strFile = Me.ToRelativePath(strFile, Me.Source)
                xnChild.InnerText = strFile

                xa = doc.CreateAttribute("Date")
                xa.Value = cStringUtils.FormatDate(tf.Date)
                xnChild.Attributes.Append(xa)

                xa = doc.CreateAttribute("Indexed")
                xa.Value = Convert.ToString(tf.IndexStatus = ISpatialDataSet.eIndexStatus.Indexed)
                xnChild.Attributes.Append(xa)

                If (tf.IndexStatus = ISpatialDataSet.eIndexStatus.Indexed) Then

                    xa = doc.CreateAttribute("LonMin")
                    xa.Value = cStringUtils.FormatSingle(tf.TopLeft.X)
                    xnChild.Attributes.Append(xa)

                    xa = doc.CreateAttribute("LonMax")
                    xa.Value = cStringUtils.FormatSingle(tf.BottomRight.X)
                    xnChild.Attributes.Append(xa)

                    xa = doc.CreateAttribute("LatMin")
                    xa.Value = cStringUtils.FormatSingle(tf.BottomRight.Y)
                    xnChild.Attributes.Append(xa)

                    xa = doc.CreateAttribute("LatMax")
                    xa.Value = cStringUtils.FormatSingle(tf.TopLeft.Y)
                    xnChild.Attributes.Append(xa)

                End If

                xn.AppendChild(xnChild)

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
            Dim xa As XmlAttribute = Nothing
            Dim xnChild As XmlNode = Nothing
            Dim cin As cCoreEnumNamesIndex = cCoreEnumNamesIndex.GetInstance()

            If (String.Compare(node.Name, "Configuration") <> 0) Then Return False

            Try
                Me.m_bCanSort = False
                For Each xn In node.ChildNodes
                    Select Case xn.Name
                        Case "Name"
                            Me.m_strName = xn.InnerText
                        Case "Description"
                            Me.DataDescription = xn.InnerText
                        Case "Source"
                            Me.Source = xn.InnerText
                            Me.IsSourceRelative = False
                            If xn.Attributes.GetNamedItem("IsRelative") IsNot Nothing Then
                                Me.IsSourceRelative = Convert.ToBoolean(xn.Attributes("IsRelative").InnerText)
                            End If
                        Case "IsSourceRelative"
                            ' Backwards compatibility
                            Me.IsSourceRelative = Convert.ToBoolean(xn.InnerText)
                        Case "Variable"
                            Me.VarName = cin.GetVarName(xn.InnerText)
                            ' Backwards compatibility
                            If (Me.VarName = eVarNameFlags.NotSet And cStringUtils.IsNumber(xn.InnerText)) Then
                                Me.VarName = DirectCast(CInt(xn.InnerText), eVarNameFlags)
                            End If
                        Case "Seasonal"
                            Me.IsSeasonal = Convert.ToBoolean(xn.InnerText)
                            Me.SeasonsEnd = Date.MaxValue
                            ' Backwards compatibility
                            If xn.Attributes.GetNamedItem("DateRef") IsNot Nothing Then
                                Me.SeasonsEnd = cStringUtils.ConvertToDate(xn.Attributes("DateRef").InnerText, "dd/MM/yyyy")
                            ElseIf (xn.Attributes.GetNamedItem("EndDate") IsNot Nothing) Then
                                Me.SeasonsEnd = cStringUtils.ConvertToDate(xn.Attributes("EndDate").InnerText)
                            End If

                        Case "Files"

                            If (Me.IsSourceRelative) Then
                                Me.Source = Me.ToAbsolutePath(Me.Source)
                            End If

                            For Each xnChild In xn.ChildNodes

                                Dim strName As String = ""

                                ' Backwards compatibility
                                If (xnChild.Attributes.GetNamedItem("Name") IsNot Nothing) Then
                                    strName = Me.ToRelativePath(xnChild.Attributes("Name").InnerText, Me.Source)
                                Else
                                    strName = Me.ToRelativePath(xnChild.InnerText, Me.Source)
                                End If

                                ' -- Date --
                                Dim strDate As String = ""
                                Dim dt As DateTime = Nothing
                                ' Backwards compatibility
                                If (xnChild.Attributes.GetNamedItem("DateRef") IsNot Nothing) Then
                                    strDate = xnChild.Attributes("DateRef").InnerText
                                    dt = cStringUtils.ConvertToDate(strDate, "dd/MM/yyyy")
                                Else
                                    strDate = xnChild.Attributes("Date").InnerText
                                    dt = cStringUtils.ConvertToDate(strDate)
                                End If

                                Dim f As New cTemporalFile(dt, Path.Combine(Me.Source, strName))

                                f.IndexStatus = ISpatialDataSet.eIndexStatus.NotIndexed
                                ' JS 24Nov14: Fixed file exist check when loading dataset metadata
                                If Not IO.File.Exists(f.FileName) Then
                                    f.IndexStatus = ISpatialDataSet.eIndexStatus.Failed
                                ElseIf (xnChild.Attributes.GetNamedItem("Indexed") IsNot Nothing) Then
                                    If (Boolean.Parse(xnChild.Attributes("Indexed").InnerText)) Then
                                        f.IndexStatus = ISpatialDataSet.eIndexStatus.Indexed
                                        f.TopLeft = New PointF(CSng(cStringUtils.ConvertToNumber(xnChild.Attributes("lonmin").InnerText, GetType(Single))), _
                                                            CSng(cStringUtils.ConvertToNumber(xnChild.Attributes("latmax").InnerText, GetType(Single))))
                                        f.BottomRight = New PointF(CSng(cStringUtils.ConvertToNumber(xnChild.Attributes("lonmax").InnerText, GetType(Single))), _
                                                            CSng(cStringUtils.ConvertToNumber(xnChild.Attributes("latmin").InnerText, GetType(Single))))
                                    End If
                                End If

                                Me.m_lFiles.Add(f)
                            Next
                    End Select
                Next
                Me.m_bCanSort = True

                ' Correction
                Me.IsSourceRelative = Me.IsSourceRelative And Not Path.IsPathRooted(Me.Source)

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

            If (Me.m_iFileIndex < 0) Then Return False
            If (Me.m_iFileIndex >= Me.m_lFiles.Count) Then Return False

            Dim f As cTemporalFile = Me.m_lFiles(Me.m_iFileIndex)

            If (ext IsNot Nothing) Then
                f.TopLeft = New PointF(CSng(ext.MinX), CSng(ext.MaxY))
                f.BottomRight = New PointF(CSng(ext.MaxX), CSng(ext.MinY))
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

            Return Me.m_lFiles(Me.m_iFileIndex).FileName

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
                                     Me.m_lFiles(Me.m_iFileIndex).Date, _
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
            Dim timeFile As New DateTime(time.Ticks)

            ' Take seasonality into account
            If Me.IsSeasonal Then
                ' Is within date range?
                If (timeFile >= Me.TimeStart) And (timeFile < Me.TimeEnd) Then
                    ' #Yes: translate 'wrap' date
                    timeFile = Me.TimeStart.AddMonths(cDateUtils.MonthDifference(timeFile, Me.TimeStart) Mod 12)
                Else
                    ' Outside data range: exit
                    Return -1
                End If
            End If

            For i As Integer = 0 To Me.m_lFiles.Count - 1
                t = Me.m_lFiles(i).Date
                If (timeFile = t) Then Return i
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
        ''' <inheritdocs cref="cFileDataSetPlugin.IndexStatusAtT"/>
        ''' -------------------------------------------------------------------
        Protected Overrides Function IndexStatusAtT(dt As Date) As ISpatialDataSet.eIndexStatus

            If (Me.m_lFiles.Count = 0) Then Return ISpatialDataSet.eIndexStatus.Indexed
            Dim iFile As Integer = Me.FileIndex(dt)

            If (iFile = -1) Then Return ISpatialDataSet.eIndexStatus.NotIndexed
            Return Me.m_lFiles(iFile).IndexStatus

        End Function

        Protected Overrides Sub UpdateIndexAtT(dt As Date)

            Dim ptfTL As New PointF(-180, 90)
            Dim ptfBR As New PointF(180, -90)
            Dim c As ISpatialDataCache = Me.Cache

            If (Me.IndexStatusAtT(dt) = ISpatialDataSet.eIndexStatus.NotIndexed) And (Me.IsConfigured) Then
                Try
                    If Me.LockDataAtT(dt, 1.0!, ptfTL, ptfBR) Then
                        If Not Me.LoadSource() Then
                            ' Log indexing failure
                            Dim iFile As Integer = Me.FileIndex(dt)
                            Me.m_lFiles(iFile).IndexStatus = ISpatialDataSet.eIndexStatus.Failed
                        End If
                    End If
                Catch ex As Threading.ThreadAbortException
                    ' OK
                Catch ex As Exception
                    ' Not ok
                Finally
                    Me.UnlockData()
                    Me.Cache = c
                End Try
            End If

        End Sub

#End Region ' Internals

#Region " Plug-in implementation "

        ''' -----------------------------------------------------------------------
        ''' <inheritdocs cref="EwEPlugin.IPlugin.Description"/>
        ''' -----------------------------------------------------------------------
        Public Overrides ReadOnly Property Description As String
            Get
                Return "Plug-in that provides dataset access to a series of time-stamped spatial files."
            End Get
        End Property

        ''' -----------------------------------------------------------------------
        ''' <inheritdocs cref="EwEPlugin.IPlugin.Name"/>
        ''' -----------------------------------------------------------------------
        Public Overrides ReadOnly Property PluginName As String
            Get
                Return "DotSpatial.DataSet.0020"
            End Get
        End Property

#End Region ' Plug-in implementation

#Region " Import & export "

        Public Overrides Function ExportTo(ByVal strPath As String) As EwEUtils.SpatialData.ISpatialDataSet

            ' Sanity checks
            Debug.Assert(Not Convert.Equals(Guid.Empty, Me.DBID), "Dataset has no valid ID yet")

            ' Clone DS
            Dim strFolder As String = cFileUtils.ToValidFileName(Me.DisplayName, False)
            Dim strAbsPath As String = Path.Combine(strPath, strFolder)

            ' Make sure that the path exists
            If Not cFileUtils.IsDirectoryAvailable(strAbsPath, True) Then
                ' ToDo: send some kind of message
                Return Nothing
            End If

            Dim ds As cMultiFileDataSetPlugin = DirectCast(Me.MemberwiseClone, cMultiFileDataSetPlugin)
            Dim bIgnoreFileErrors As Boolean = False
            ds.IsSourceRelative = True
            ds.Source = strAbsPath

            ' Copy all files
            For i As Integer = 0 To Me.m_lFiles.Count - 1
                Dim strSrc As String = ds.m_lFiles(i).FileName
                Dim strTgt As String = Path.Combine(strAbsPath, Path.GetFileName(ds.m_lFiles(i).FileName))

                Try
                    ' Copy file
                    System.IO.File.Copy(strSrc, strTgt, True)
                    ' Reroute to new location to finish export properly
                    ds.m_lFiles(i).FileName = strTgt

                Catch exd As DirectoryNotFoundException
                    If Not bIgnoreFileErrors Then
                        Dim msg As New cFeedbackMessage(cStringUtils.Localize(My.Resources.PROMPT_EXPORT_ERROR_NOPATH, Me.Source, Me.DisplayName), _
                                                        eCoreComponentType.External, eMessageType.DataExport, _
                                                        eMessageImportance.Question, eMessageReplyStyle.YES_NO)
                        msg.Reply = eMessageReply.NO
                        Me.m_core.Messages.SendMessage(msg)
                        If msg.Reply = eMessageReply.YES Then
                            bIgnoreFileErrors = True
                        Else
                            Return Nothing
                        End If
                    End If
                Catch exf As FileNotFoundException
                    If Not bIgnoreFileErrors Then
                        Dim msg As New cFeedbackMessage(cStringUtils.Localize(My.Resources.PROMPT_EXPORT_ERROR_NOFILES, Me.DisplayName), _
                                                        eCoreComponentType.External, eMessageType.DataExport, _
                                                        eMessageImportance.Question, eMessageReplyStyle.YES_NO)
                        msg.Reply = eMessageReply.NO
                        Me.m_core.Messages.SendMessage(msg)
                        If msg.Reply = eMessageReply.YES Then
                            bIgnoreFileErrors = True
                        Else
                            Return Nothing
                        End If
                    End If
                Catch ex As Exception
                    ' ToDo: send some kind of message
                    Return Nothing
                End Try

                ' Clear index status for each file; file presence must be re-assessed wherever the dataset is used
                ds.m_lFiles(i).IndexStatus = ISpatialDataSet.eIndexStatus.NotIndexed

            Next i

            ' Return clone
            Return ds

        End Function

#End Region ' Import & export

#Region " Summary "

        Public Overrides ReadOnly Property Summary As String
            Get
                Dim sb As New StringBuilder()

                sb.Append("id:" & Me.GetType().ToString())
                For i As Integer = 0 To Me.m_lFiles.Count - 1
                    Dim f As cTemporalFile = Me.m_lFiles(i)
                    sb.Append(",")
                    sb.Append("t" & i & ":" & cStringUtils.FormatDate(f.Date))
                    sb.Append("f" & i & ":" & Path.GetFileName(f.FileName))
                Next
                Return sb.ToString()

            End Get

        End Property

#End Region ' Summary

    End Class

End Namespace
