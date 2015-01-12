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
Imports System.Drawing
Imports System.IO
Imports System.Windows.Forms
Imports System.Xml
Imports DotSpatial.Data
Imports EwEUtils.Core
Imports EwEUtils.SpatialData
Imports EwEUtils.Utilities

#End Region ' Imports

Namespace SpatialData

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' <see cref="ISpatialDataSet"/> for accessing a single spatial file without
    ''' any temporal attribute.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cSingleFileDataSetPlugin
        Inherits cFileDataSetPlugin

#Region " Private vars "

        Private m_indexstatus As ISpatialDataSet.eIndexStatus = ISpatialDataSet.eIndexStatus.NotIndexed
        Private m_ptTL As New PointF(0, 0)
        Private m_ptBR As New PointF(0, 0)
        Private m_dtStart As DateTime = DateTime.MinValue
        Private m_dtEnd As DateTime = DateTime.MaxValue

#End Region ' Private vars

#Region " Construction / destruction "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Sub New()
            MyBase.New()
            Me.VarName = eVarNameFlags.NotSet
        End Sub

#End Region ' Construction / destruction

#Region " Information "

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="cFileDataSetPlugin.DisplayName" />
        ''' -------------------------------------------------------------------
        Public Overrides Property DisplayName As String
            Get
                If (Not String.IsNullOrWhiteSpace(Me.m_strName)) Then
                    Return Me.m_strName
                End If
                If (Not String.IsNullOrWhiteSpace(Me.Source)) Then
                    Return cStringUtils.Localize(My.Resources.DATASET_SINGLE_DISPLAYNAME, Path.GetFileName(Me.Source))
                End If
                Return My.Resources.DATASET_SINGLE_NAME
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
                Return Nothing
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="cFileDataSetPlugin.TimeStart"/>
        ''' -------------------------------------------------------------------
        Public Overrides ReadOnly Property TimeStart As DateTime
            Get
                Return Me.m_dtStart
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="cFileDataSetPlugin.TimeEnd"/>
        ''' -------------------------------------------------------------------
        Public Overrides ReadOnly Property TimeEnd As DateTime
            Get
                Return Me.m_dtEnd
            End Get
        End Property

        Public Property Time As DateTime
            Get
                Return Me.m_dtStart
            End Get
            Set(value As DateTime)
                If (value = DateTime.MinValue) Or (value = DateTime.MaxValue) Then
                    Me.m_dtStart = DateTime.MinValue
                    Me.m_dtEnd = DateTime.MaxValue
                Else
                    Me.m_dtStart = value
                    Me.m_dtEnd = value
                End If
            End Set
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
        ''' <summary>
        ''' Returns whether the dataset equals another.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overrides Function Equals(obj As Object) As Boolean
            If (obj Is Nothing) Then Return False
            If (Not TypeOf obj Is cSingleFileDataSetPlugin) Then Return False

            Dim sfd As cSingleFileDataSetPlugin = DirectCast(obj, cSingleFileDataSetPlugin)

            Return (String.Compare(Me.SourceFileName, sfd.SourceFileName, True) = 0) And _
                   (Me.TimeStart = sfd.TimeStart) And _
                   (Me.TimeEnd = sfd.TimeEnd)
        End Function

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="cFileDataSetPlugin.ConversionFormat"/>
        ''' -------------------------------------------------------------------
        Public Overrides ReadOnly Property ConversionFormat() As String
            Get
                If (String.IsNullOrWhiteSpace(Me.Source)) Then Return ""
                Return cDotSpatialUtils.GetDataFormat(Me.Source)
            End Get
        End Property

#End Region ' Information

#Region " Configuration "

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="cFileDataSetPlugin.GetConfigUI"/>
        ''' -------------------------------------------------------------------
        Public Overrides Function GetConfigUI() As Windows.Forms.Control
            Dim pg As New ucSingleFileDataSetConfigPage()
            pg.Dataset = Me
            Return pg
        End Function

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="cFileDataSetPlugin.IsConfigured"/>
        ''' -------------------------------------------------------------------
        Public Overrides Function IsConfigured() As Boolean
            Return Not String.IsNullOrWhiteSpace(Me.Source)
        End Function

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="cFileDataSetPlugin.IsDataAvailable"/>
        ''' -------------------------------------------------------------------
        Public Overrides Function IsDataAvailable(ByVal runtype As IRunType) As Boolean
            Return System.IO.File.Exists(Me.Source)
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
        Protected Overrides Function ToXML(ByVal doc As XmlDocument) As XmlNode

            Dim xnMaster As XmlNode = Nothing
            Dim xn As XmlNode = Nothing
            Dim xnFile As XmlNode = Nothing
            Dim xaFile As XmlAttribute = Nothing

            xnMaster = doc.CreateElement("Configuration")

            xn = doc.CreateElement("Name")
            xn.InnerText = Me.m_strName
            xnMaster.AppendChild(xn)

            xn = doc.CreateElement("Description")
            xn.InnerText = Me.DataDescription
            xnMaster.AppendChild(xn)

            xn = doc.CreateElement("Variable")
            xn.InnerText = CStr(CInt(Me.VarName))
            xnMaster.AppendChild(xn)

            xnFile = doc.CreateElement("File")

            xaFile = doc.CreateAttribute("Source")
            If (Me.IsSourceRelative) Then
                xaFile.Value = Me.ToRelativePath(Me.Source)
            Else
                xaFile.Value = Me.Source
            End If
            xnFile.Attributes.Append(xaFile)

            xaFile = doc.CreateAttribute("IsSourceRelative")
            xaFile.Value = Convert.ToString(Me.IsSourceRelative)
            xnFile.Attributes.Append(xaFile)

            xaFile = doc.CreateAttribute("Date")
            xaFile.Value = cStringUtils.FormatDate(Me.m_dtStart, "d")
            xnFile.Attributes.Append(xaFile)

            xaFile = doc.CreateAttribute("Indexed")
            xaFile.Value = Convert.ToString(Me.m_indexstatus = ISpatialDataSet.eIndexStatus.Indexed)
            xnFile.Attributes.Append(xaFile)

            If (Me.m_indexstatus = ISpatialDataSet.eIndexStatus.Indexed) Then

                xaFile = doc.CreateAttribute("LonMin")
                xaFile.Value = cStringUtils.FormatSingle(Me.m_ptTL.X)
                xnFile.Attributes.Append(xaFile)

                xaFile = doc.CreateAttribute("LonMax")
                xaFile.Value = cStringUtils.FormatSingle(Me.m_ptBR.X)
                xnFile.Attributes.Append(xaFile)

                xaFile = doc.CreateAttribute("LatMin")
                xaFile.Value = cStringUtils.FormatSingle(Me.m_ptBR.Y)
                xnFile.Attributes.Append(xaFile)

                xaFile = doc.CreateAttribute("LatMax")
                xaFile.Value = cStringUtils.FormatSingle(Me.m_ptTL.Y)
                xnFile.Attributes.Append(xaFile)

            End If

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
        Protected Overrides Function FromXML(ByVal doc As XmlDocument, ByVal node As XmlNode) As Boolean

            Dim xn As XmlNode = Nothing

            If (String.Compare(node.Name, "Configuration") <> 0) Then Return False

            Try

                For Each xn In node.ChildNodes
                    Select Case xn.Name
                        Case "Name" : Me.m_strName = xn.InnerText
                        Case "Description" : Me.DataDescription = xn.InnerText
                        Case "Variable" : Me.VarName = DirectCast(CInt(xn.InnerText), eVarNameFlags)
                        Case "File"
                            ' -- Source --
                            Me.Source = xn.Attributes("Source").InnerText
                            If (xn.Attributes.GetNamedItem("IsSourceRelative") IsNot Nothing) Then
                                Me.IsSourceRelative = Boolean.Parse(xn.Attributes("IsSourceRelative").InnerText)
                            Else
                                Me.IsSourceRelative = False
                            End If

                            ' -- Date --
                            Dim strDate As String = ""
                            Dim dt As DateTime = Nothing
                            If (xn.Attributes.GetNamedItem("DateRef") IsNot Nothing) Then
                                strDate = xn.Attributes("DateRef").InnerText
                                dt = cStringUtils.ConvertToDate(strDate, "dd/MM/yyyy")
                            Else
                                strDate = xn.Attributes("Date").InnerText
                                dt = cStringUtils.ConvertToDate(strDate)
                            End If
                            If (dt = DateTime.MinValue) Or (dt = DateTime.MaxValue) Then
                                Me.m_dtStart = DateTime.MinValue
                                Me.m_dtEnd = DateTime.MaxValue
                            Else
                                Me.m_dtStart = dt
                                Me.m_dtEnd = dt
                            End If

                            ' -- Index status --
                            Me.m_indexstatus = ISpatialDataSet.eIndexStatus.NotIndexed
                            If (xn.Attributes.GetNamedItem("Indexed") IsNot Nothing) Then
                                ' JS 06Nov13: added file exist check when loading dataset metadata
                                If Boolean.Parse(xn.Attributes("Indexed").InnerText) And IO.File.Exists(Me.SourceFileName) Then
                                    Me.m_indexstatus = ISpatialDataSet.eIndexStatus.Indexed
                                    Me.m_ptTL = New PointF(CSng(cStringUtils.ConvertToNumber(xn.Attributes("lonmin").InnerText, GetType(Single))), _
                                                           CSng(cStringUtils.ConvertToNumber(xn.Attributes("latmax").InnerText, GetType(Single))))
                                    Me.m_ptBR = New PointF(CSng(cStringUtils.ConvertToNumber(xn.Attributes("lonmax").InnerText, GetType(Single))), _
                                                           CSng(cStringUtils.ConvertToNumber(xn.Attributes("latmin").InnerText, GetType(Single))))
                                End If
                            End If

                            ' Set initial index status
                            If Not File.Exists(Me.Source) Then
                                Me.m_indexstatus = ISpatialDataSet.eIndexStatus.Failed
                            End If

                    End Select
                Next

            Catch ex As Exception
                Return False
            End Try

            ' Resolve relative source path to the current config file location
            If (Me.IsSourceRelative) Then
                Me.Source = Me.ToAbsolutePath(Me.Source)
            End If

            Return True

        End Function

#End Region ' Configuration

#Region " Data "

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="ISpatialDataSet.HasDataAtT"/>
        ''' -------------------------------------------------------------------
        Public Overrides Function HasDataAtT(ByVal time As DateTime) As Boolean
            If (Me.m_core Is Nothing) Then Return True
            Return (Me.m_core.AbsoluteTimeToEcospaceTimestep(time) = 1)
        End Function

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="ISpatialDataSet.GetExtentAtT"/>
        ''' -------------------------------------------------------------------
        Public Overrides Function GetExtentAtT(ByVal datetime As Date, _
                                               ByRef ptfTL As System.Drawing.PointF, _
                                               ByRef ptfBR As System.Drawing.PointF) As Boolean

            ' Return cached extent
            ptfTL = Me.m_ptTL
            ptfBR = Me.m_ptBR
            Return (Me.m_indexstatus = ISpatialDataSet.eIndexStatus.Indexed)

        End Function

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="cFileDataSetPlugin.IndexStatusAtT"/>
        ''' -------------------------------------------------------------------
        Protected Overrides Function IndexStatusAtT(dt As Date) As EwEUtils.SpatialData.ISpatialDataSet.eIndexStatus
            Return Me.m_indexstatus
        End Function

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="cFileDataSetPlugin.UpdateIndexAtT"/>
        ''' -------------------------------------------------------------------
        Protected Overrides Sub UpdateIndexAtT(ByVal dateStart As DateTime)

            Dim ptfTL As New PointF(-180, 90)
            Dim ptfBR As New PointF(180, -90)
            Dim c As ISpatialDataCache = Me.Cache

            If (Me.m_indexstatus <> ISpatialDataSet.eIndexStatus.Indexed) And (Me.IsConfigured) Then
                Try
                    Me.Cache = Nothing
                    If Me.LockDataAtT(Nothing, 1.0!, ptfTL, ptfBR) Then
                        Me.LoadSource()
                        Me.UnlockData()
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

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="cFileDataSetPlugin.StoreExtent"/>
        ''' -------------------------------------------------------------------
        Protected Overrides Function StoreExtent(ByVal ext As IExtent) As Boolean

            If (ext IsNot Nothing) Then
                Me.m_ptTL = New PointF(CSng(ext.MinX), CSng(ext.MaxY))
                Me.m_ptBR = New PointF(CSng(ext.MaxX), CSng(ext.MinY))
                Me.m_indexstatus = ISpatialDataSet.eIndexStatus.Indexed
            Else
                Me.m_indexstatus = ISpatialDataSet.eIndexStatus.Failed
            End If

            Return True

        End Function

#End Region ' Data

#Region " Internals "

        Protected Overrides Function CacheFileName(ByVal strLayerName As String) As String

            Dim c As ISpatialDataCache = Me.Cache
            Dim strExt As String = cDotSpatialUtils.DefaultCacheExtension()

            If (c IsNot Nothing) Then
                Return c.GetFileName(Me, _
                                     cDotSpatialUtils.TopLeft(Me.m_extModelArea), _
                                     cDotSpatialUtils.BottomRight(Me.m_extModelArea), _
                                     Me.m_dModelCellSize, _
                                     New DateTime(0), _
                                     strLayerName, strExt)
            End If
            Return cFileUtils.MakeTempFile(strExt)

        End Function

        Protected Overrides Function SourceFileName() As String
            Return Me.Source
        End Function

#End Region ' Internals

#Region " Plug-in implementation "

        ''' -----------------------------------------------------------------------
        ''' <inheritdocs cref="EwEPlugin.IPlugin.Description"/>
        ''' -----------------------------------------------------------------------
        Public Overrides ReadOnly Property Description As String
            Get
                Return "Plug-in that provides access to a dataset that contains a single time-stamped spatial file"
            End Get
        End Property

        ''' -----------------------------------------------------------------------
        ''' <inheritdocs cref="EwEPlugin.IPlugin.Name"/>
        ''' -----------------------------------------------------------------------
        Public Overrides ReadOnly Property PluginName As String
            Get
                Return "DotSpatial.DataSet.0010"
            End Get
        End Property

#End Region ' Plug-in implementation

#Region " Import & export "

        Public Overrides Function ExportTo(ByVal strPath As String) As EwEUtils.SpatialData.ISpatialDataSet

            ' Sanity checks
            Debug.Assert(Not Convert.Equals(Guid.Empty, Me.DBID), "Dataset has no valid ID yet")

            ' Clone DS
            Dim ds As cSingleFileDataSetPlugin = DirectCast(Me.MemberwiseClone, cSingleFileDataSetPlugin)

            ' Export file content to a folder in strPath that is identified by the current GUID
            ' Note that the exported dataset will inherit the same GUID. It makes sense but may cause confusion...
            Dim strFolder As String = cFileUtils.ToValidFileName(Me.DisplayName, False)
            Dim strAbsPath As String = Path.Combine(strPath, strFolder)
            Dim strAbsFile As String = Path.Combine(strAbsPath, Path.GetFileName(Me.Source))

            ' Internally, source is ALWAYS absolute
            ds.IsSourceRelative = True
            ds.Source = strAbsFile

            ' Make sure that the path exists
            If Not cFileUtils.IsDirectoryAvailable(strAbsPath, True) Then
                ' ToDo: send some kind of message
                Return Nothing
            End If

            ' Copy file
            Try
                File.Copy(Me.Source, ds.Source, True)
            Catch ex As Exception
                ' ToDo: send some kind of message
                Return Nothing
            End Try

            ' Clear index status on the new dataset; file presence must be re-assessed wherever the dataset is used
            ds.m_indexstatus = ISpatialDataSet.eIndexStatus.NotIndexed

            ' Return clone
            Return ds

        End Function

#End Region ' Import & export

    End Class

End Namespace
