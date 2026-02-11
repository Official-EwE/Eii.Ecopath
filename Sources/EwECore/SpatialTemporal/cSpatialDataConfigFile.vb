' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.IO
Imports System.Reflection
Imports System.Xml
Imports EwECore.Common
Imports EwEUtils.Utilities
Imports Microsoft.Extensions.Logging

Namespace SpatialData

    Public Class cSpatialDataConfigFile

#Region " Internal vars "

        Private m_strFileName As String = ""
        Private m_strDatasetName As String = ""
        Private m_nDatasets As Integer = 0
        Private ReadOnly m_logger As ILogger = LoggingContext.CreateLogger(Of cSpatialDataConfigFile)()

#End Region ' Internal vars

        Friend Sub New()
        End Sub

        Public Sub New(strFile As String, strName As String, strDescription As String, strSource As String, strAuthor As String, strContact As String)
            Me.m_strFileName = strFile
            Me.DatasetName = strName
            Me.Description = strDescription
            Me.Station = strSource
            Me.Author = strAuthor
            Me.Contact = strContact
        End Sub

#Region " Public properties "

        Public Property FileName As String
            Get
                If (String.IsNullOrWhiteSpace(Me.m_strFileName)) Then
                    Return cSpatialDataSetManager.DefaultConfigFile()
                End If
                Return Me.m_strFileName
            End Get
            Private Set(value As String)
                Me.m_strFileName = value
            End Set
        End Property

        Public Property DatasetName As String
            Get
                If (String.IsNullOrWhiteSpace(Me.m_strDatasetName)) And
                   (Not String.IsNullOrWhiteSpace(Me.m_strFileName)) Then
                    Return Path.GetFileNameWithoutExtension(Me.m_strFileName)
                End If
                Return Me.m_strDatasetName
            End Get
            Set(value As String)
                Me.m_strDatasetName = value
            End Set
        End Property

        Public Property Station As String = ""
        Public Property Description As String = ""
        Public Property Author As String = ""
        Public Property Contact As String = ""

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the number of datasets in the configuration file.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property nDatasets As Integer
            Get
                Return Me.m_nDatasets
            End Get
        End Property

        Public ReadOnly Property IsDefault As Boolean
            Get
                Return (String.Compare(Me.m_strFileName, cSpatialDataSetManager.DefaultConfigFile, True) = 0)
            End Get
        End Property

#End Region ' Public properties

#Region " Internals "

        Shared Function GetDatasets(strFile As String) As ISpatialDataSet()
            Dim cfg As New cSpatialDataConfigFile()
            Return cfg.Load(strFile)
        End Function

        Friend Function Initialize(strFile As String) As Boolean

            ' Clear properties just to be sure in case of re-initializing
            Me.DatasetName = ""
            Me.Author = ""
            Me.Contact = ""
            Me.Description = ""
            Me.Station = ""

            Me.FileName = strFile

            If (Not File.Exists(Me.FileName)) Then
                ' Init OK on missing default config file. Any other file has to exist
                Return (String.Compare(Me.FileName, cSpatialDataSetManager.DefaultConfigFile, True) = 0)
            End If

            Dim doc As New XmlDocument()
            Dim xnRoot As XmlNode = Nothing
            Dim xa As XmlAttribute = Nothing

            ' Load datasets
            Try
                Me.m_nDatasets = 0
                doc.Load(strFile)

                For Each xnRoot In doc.GetElementsByTagName("Datasets")
                    For Each xa In xnRoot.Attributes
                        Select Case xa.Name
                            Case "Name" : Me.DatasetName = xa.InnerText
                            Case "Author" : Me.Author = xa.InnerText
                            Case "Contact" : Me.Contact = xa.InnerText
                            Case "Source", "Station" : Me.Station = xa.InnerText
                            Case "Description" : Me.Description = xa.InnerText
                        End Select
                    Next
                Next

                ' Update number of datasets
                Me.m_nDatasets = doc.GetElementsByTagName("Dataset").Count
                Return True

            Catch ex As Exception
                m_logger.LogError(ex, "cSpatialDataSetManager.Initialize({0})", strFile)
            End Try
            Return False

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Load the spatial datasets from a config file.
        ''' </summary>
        ''' <returns>False if the config file is corrupted, True otherwise.</returns>
        ''' <remarks>This method can also be used to import extra datasets.</remarks>
        ''' -------------------------------------------------------------------
        Public Function Load(strFile As String) As ISpatialDataSet()

            Dim strRoot As String = Path.GetDirectoryName(Path.GetFullPath(strFile))
            Dim doc As New XmlDocument()
            Dim xnRoot As XmlNode = Nothing
            Dim xa As XmlAttribute = Nothing
            Dim ds As ISpatialDataSet = Nothing
            Dim an As AssemblyName = Nothing
            Dim bSuccess As Boolean = False
            Dim datasets As New List(Of ISpatialDataSet)

            If File.Exists(strFile) Then

                ' Load datasets
                doc.Load(strFile)

                For Each xnRoot In doc.GetElementsByTagName("Datasets")
                    'Found a "Datasets" tag in the file
                    bSuccess = True
                    For Each xn As XmlNode In xnRoot.ChildNodes
                        ds = Nothing
                        If (xn.Name = "Dataset") Then
                            xa = xn.Attributes("Type")
                            If (xa IsNot Nothing) Then
                                Try
                                    Dim strTypeName As String = xa.InnerText
                                    ' Type name mapping
                                    strTypeName = strTypeName.Replace("cAAASFileDataSetPlugin", "cASCIIFilesDataSetPlugin")
                                    ' Get plug-in
                                    Dim t As Type = cTypeUtils.StringToType(strTypeName)
                                    If (t Is Nothing) Then
                                        t = GetType(cSpatialDatasetPlaceholder)
                                    End If

                                    ds = DirectCast(Activator.CreateInstance(t), ISpatialDataSet)
                                    ' If (TypeOf ds Is IPlugin) Then DirectCast(ds, IPlugin).Initialize(core)

                                    If (TypeOf ds Is cSpatialDatasetPlaceholder) Then
                                        DirectCast(ds, cSpatialDatasetPlaceholder).PreservedType = xa.InnerText
                                    End If
                                    ds.Configuration(doc, strRoot) = xn.ChildNodes(0)

                                    ' Assign GUID
                                    xa = xn.Attributes("GUID")
                                    ds.GUID = Guid.Parse(xa.InnerText)

                                Catch ex As Exception
                                    ds = Nothing
                                    bSuccess = False
                                    m_logger.LogError(ex, "cSpatialDataSetManager.Load(" & strFile & ")")
                                End Try

                                datasets.Add(ds)
                            End If
                        End If
                    Next ' xn
                Next ' xnRoot
            End If

            Return datasets.ToArray()

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Saves all datasets currently loaded by the manager to persistent storage.
        ''' </summary>
        ''' <returns>True if successful.</returns>
        ''' <remarks>
        ''' <para>If the manager is read-only, which is set when the datafile
        ''' is externally modified, any save attempt will abort and fail.</para>
        ''' <para>Note that this method can also be used to export datasets.</para>
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Friend Function Save(core As cCore,
                             man As cSpatialDataSetManager,
                             datasets As ISpatialDataSet(),
                             bExporting As Boolean) As Boolean

            Dim strFile As String = Me.FileName
            Dim strPath As String = Path.GetDirectoryName(Me.FileName)
            Dim doc As New XmlDocument()
            Dim xnRoot As XmlNode = Nothing
            Dim xaRoot As XmlAttribute = Nothing
            Dim xnDataset As XmlNode = Nothing
            Dim xnDetails As XmlNode = Nothing
            Dim xaDataset As XmlAttribute = Nothing
            Dim nExported As Integer = 0
            Dim msg As cMessage = Nothing
            Dim bSuccess As Boolean = True

            ' Make sure we have something to iterate over, even if it is an empty list
            If (datasets Is Nothing) Then
                datasets = New ISpatialDataSet() {}
            End If

            ' Create dir
            If Not cFileUtils.IsDirectoryAvailable(strPath, True) Then
                Return False
            End If

            ' Build new base doc
            doc = cSpatialDataSetManager.NewDoc(xnRoot)

            ' Complete root info
            xaRoot = CType(xnRoot.Attributes.GetNamedItem("Name"), XmlAttribute)
            If (xaRoot Is Nothing) Then
                xaRoot = doc.CreateAttribute("Name")
                xnRoot.Attributes.Append(xaRoot)
            End If
            xaRoot.InnerText = Me.DatasetName

            xaRoot = CType(xnRoot.Attributes.GetNamedItem("Author"), XmlAttribute)
            If (xaRoot Is Nothing) Then
                xaRoot = doc.CreateAttribute("Author")
                xnRoot.Attributes.Append(xaRoot)
            End If
            xaRoot.InnerText = Me.Author

            xaRoot = CType(xnRoot.Attributes.GetNamedItem("Contact"), XmlAttribute)
            If (xaRoot Is Nothing) Then
                xaRoot = doc.CreateAttribute("Contact")
                xnRoot.Attributes.Append(xaRoot)
            End If
            xaRoot.InnerText = Me.Contact

            xaRoot = CType(xnRoot.Attributes.GetNamedItem("Station"), XmlAttribute)
            If (xaRoot Is Nothing) Then
                xaRoot = doc.CreateAttribute("Station")
                xnRoot.Attributes.Append(xaRoot)
            End If
            xaRoot.InnerText = Me.Station

            ' Remove obsolete Source node
            xnRoot.Attributes.RemoveNamedItem("Source")

            xaRoot = CType(xnRoot.Attributes.GetNamedItem("Description"), XmlAttribute)
            If (xaRoot Is Nothing) Then
                xaRoot = doc.CreateAttribute("Description")
                xnRoot.Attributes.Append(xaRoot)
            End If
            xaRoot.InnerText = Me.Description

            ' Gather dataset config nodes, but do not add to the doc until all done
            For Each ds As ISpatialDataSet In datasets

                If (bExporting) Then ds = ds.ExportTo(Path.GetDirectoryName(strFile))

                ' Exclude virtual datasets from ending up in a config file
                If (ds IsNot Nothing) Then

                    xnDataset = doc.CreateElement("Dataset")

                    xaDataset = doc.CreateAttribute("Type")
                    If (TypeOf ds Is cSpatialDatasetPlaceholder) Then
                        xaDataset.Value = DirectCast(ds, cSpatialDatasetPlaceholder).PreservedType
                    Else
                        xaDataset.Value = cTypeUtils.TypeToString(ds.GetType)
                    End If
                    xnDataset.Attributes.Append(xaDataset)

                    xaDataset = doc.CreateAttribute("GUID")
                    xaDataset.Value = Convert.ToString(ds.GUID)
                    xnDataset.Attributes.Append(xaDataset)

                    Try
                        xnDetails = ds.Configuration(doc, strPath)
                    Catch ex As Exception
                        xnDetails = Nothing
                    End Try

                    If (xnDetails IsNot Nothing) Then
                        xnDataset.AppendChild(xnDetails)
                        nExported += 1
                    End If

                    ' Add dataset nodes
                    xnRoot.AppendChild(xnDataset)

                Else
                    bSuccess = False
                End If
            Next

            ' Save
            Try
                doc.Save(strFile)

                If (bExporting) Then
                    ' Send export status message
                    If bSuccess Then
                        msg = New cMessage(cStringUtils.Localize(My.Resources.CoreMessages.SPATIALTEMPORAL_SAVE_SUCCESS, nExported, strPath),
                                           eMessageType.DataExport, eCoreComponentType.External, eMessageImportance.Information)
                        msg.Hyperlink = strPath
                    Else
                        msg = New cMessage(cStringUtils.Localize(My.Resources.CoreMessages.SPATIALTEMPORAL_SAVE_ERROR, strPath),
                                           eMessageType.DataExport, eCoreComponentType.External, eMessageImportance.Critical)
                    End If
                End If

            Catch ex As Exception
                bSuccess = False

                msg = New cMessage(cStringUtils.Localize(My.Resources.CoreMessages.SPATIALTEMPORAL_SAVE_EXCEPTION, ex.Message),
                                   eMessageType.DataExport, eCoreComponentType.External, eMessageImportance.Information)
                If (bExporting) Then
                    msg.Importance = eMessageImportance.Critical
                End If
            End Try

            If (msg IsNot Nothing) Then
                core.Messages.SendMessage(msg)
            End If

            Return bSuccess

        End Function

#End Region ' Internals

    End Class

End Namespace
