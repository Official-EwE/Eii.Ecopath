#Region " Imports "

Option Strict On
Imports System.Xml
Imports EwEUtils.SpatialData
Imports EwEUtils.Core
Imports EwEUtils.Utilities
Imports System.Reflection
Imports System.IO
Imports EwEPlugin

#End Region ' Imports

Namespace SpatialData

    Public Class cSpatialDataConfigFile

#Region " Internal vars "

        Private m_strFileName As String = ""
        Private m_strDatasetName As String = ""

#End Region ' Internal vars

        Friend Sub New()
        End Sub

        Friend Sub New(ByVal strFile As String, _
                       ByVal strName As String, _
                       ByVal strDescription As String, _
                       ByVal strSource As String, _
                       ByVal strAuthor As String, _
                       ByVal strContact As String)
            Me.m_strFileName = strFile
            Me.DatasetName = strName
            Me.Description = strDescription
            Me.Source = Source
            Me.Author = Author
            Me.Contact = Contact
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
                If (String.IsNullOrWhiteSpace(Me.m_strDatasetName)) And _
                   (Not String.IsNullOrWhiteSpace(Me.m_strFileName)) Then
                    Return Path.GetFileNameWithoutExtension(Me.m_strFileName)
                End If
                Return Me.m_strDatasetName
            End Get
            Set(value As String)
                Me.m_strDatasetName = value
            End Set
        End Property

        Public Property Source As String = ""
        Public Property Description As String = ""
        Public Property Author As String = ""
        Public Property Contact As String = ""

#End Region ' Public properties

#Region " Internals "

        Friend Function Create(ByVal strFile As String) As Boolean
            Me.FileName = strFile
        End Function

        Friend Function Initialize(ByVal strFile As String) As Boolean

            Me.FileName = strFile

            If (Not File.Exists(Me.FileName)) Then
                ' Init OK on missing default config file. Any other file has to exist
                Return (String.Compare(Me.FileName, cSpatialDataSetManager.DefaultConfigFile, True) = 0)
            End If

            ' ToDo: Read header info (author, contact, etc)
            Dim doc As New XmlDocument()
            Dim xnRoot As XmlNode = Nothing
            Dim xa As XmlAttribute = Nothing

            ' Load datasets
            doc.Load(strFile)

            For Each xnRoot In doc.GetElementsByTagName("Datasets")
                For Each xa In xnRoot.Attributes
                    Select Case xa.Name
                        Case "Name" : Me.DatasetName = xa.InnerText
                        Case "Author" : Me.Author = xa.InnerText
                        Case "Contact" : Me.Contact = xa.InnerText
                        Case "Source" : Me.Source = xa.InnerText
                        Case "Description" : Me.Description = xa.InnerText
                    End Select
                Next
            Next
            Return True

        End Function


        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Initializes the manager with datasets, loaded from persistent storage.
        ''' </summary>
        ''' <returns>False if the config file is corrupted, True otherwise.</returns>
        ''' <remarks>This method can also be used to import extra datasets.</remarks>
        ''' -------------------------------------------------------------------
        Friend Function Load(ByVal core As cCore, _
                             ByVal man As cSpatialDataSetManager) As Boolean

            Dim strFile As String = Me.FileName
            Dim doc As New XmlDocument()
            Dim xnRoot As XmlNode = Nothing
            Dim xa As XmlAttribute = Nothing
            Dim ds As ISpatialDataSet = Nothing
            Dim an As AssemblyName = Nothing
            Dim msgWarning As cMessage = Nothing
            Dim bSuccess As Boolean = False

            If Not File.Exists(strFile) Then Return False

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
                                ' Hack
                                strTypeName = strTypeName.Replace("cAAASFileDataSetPlugin", "cASCIIFilesDataSetPlugin")
                                ' Get plug-in
                                Dim t As Type = cTypeUtils.StringToType(strTypeName)
                                If (t IsNot Nothing) Then

                                    ds = DirectCast(Activator.CreateInstance(t), ISpatialDataSet)
                                    If (TypeOf ds Is IPlugin) Then DirectCast(ds, IPlugin).Initialize(core)
                                    ds.Configuration(doc) = xn.ChildNodes(0)

                                    ' Assign GUID
                                    xa = xn.Attributes("GUID")
                                    ds.GUID = GUID.Parse(xa.InnerText)


                                Else '(t IsNot Nothing)
                                    cLog.Write("Unable to instantiate data set " & strTypeName)

                                    If (msgWarning Is Nothing) Then
                                        msgWarning = New cMessage(My.Resources.CoreMessages.SPATIALTEMPORAL_LOAD_ERROR_GENERIC, _
                                                                  eMessageType.ErrorEncountered, eCoreComponentType.EcoSpace, _
                                                                  eMessageImportance.Warning)
#If DEBUG Then
                                        ' When debugging turn this message to a mere info message ;)
                                        msgWarning.Importance = eMessageImportance.Information
#End If
                                    End If
                                    Dim vs As New cVariableStatus(eStatusFlags.MissingParameter, _
                                                                  String.Format(My.Resources.CoreMessages.SPATIALTEMPORAL_LOAD_ERROR_DETAIL, strTypeName), _
                                                                  eVarNameFlags.NotSet, eDataTypes.NotSet, eCoreComponentType.EcoSpace, 0)
                                    msgWarning.AddVariable(vs)
                                End If

                            Catch ex As Exception
                                ds = Nothing
                                bSuccess = False
                                cLog.Write(ex, "cSpatialDataSetManager.Load(" & strFile & ")")
                            End Try

                            Dim bAdd As Boolean = False
                            If (ds IsNot Nothing) Then
                                bAdd = True
                                If (Not (ds.GUID.Equals(GUID.Empty))) Then
                                    bAdd = (man.Find(ds.GUID) Is Nothing)
                                End If
                            End If
                            If bAdd Then man.Add(ds)
                        End If
                    End If
                Next ' xn
            Next ' xnRoot

            If (msgWarning IsNot Nothing) Then
                core.Messages.SendMessage(msgWarning)
            End If

            Return bSuccess

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
        Friend Function Save(ByVal core As cCore, _
                             ByVal man As cSpatialDataSetManager, _
                             ByVal datasets As ISpatialDataSet(), _
                             ByVal bExporting As Boolean) As Boolean

            Dim strFile As String = Me.FileName
            Dim doc As New XmlDocument()
            Dim xnRoot As XmlNode = Nothing
            Dim xaRoot As XmlAttribute = Nothing
            Dim xnDataset As XmlNode = Nothing
            Dim xnDetails As XmlNode = Nothing
            Dim xaDataset As XmlAttribute = Nothing
            Dim bChanged As Boolean = False
            Dim nExported As Integer = 0
            Dim strPath As String = ""
            Dim bSuccess As Boolean = True

            If (datasets Is Nothing) Then Return False
            If (datasets.Length = 0) Then Return False

            ' Create dir
            strPath = Path.GetDirectoryName(strFile)
            If Not cFileUtils.IsDirectoryAvailable(strPath, True) Then
                Return False
            End If

            Try
                ' Load existing datasets from file if not exporting. This is done to ensure that
                ' datasets that are defined but that could not be instantiated (for example due to
                ' a missing plug-in) are not destroyed in the save process.
                If ((Not bExporting) And (File.Exists(strFile))) Then
                    doc.Load(strFile)
                    xnRoot = doc.GetElementsByTagName("Datasets")(0)
                End If
            Catch ex As Exception
                ' Plop
            End Try

            ' Create a new XML doc if needed.
            If (xnRoot Is Nothing) Then
                ' Build new base doc
                doc = cSpatialDataSetManager.NewDoc(xnRoot)
            End If

            ' Remove all deleted or current datasets from the XML nodes; these will be
            ' recreated by the save process.
            Dim lDelete As New List(Of XmlNode)
            For Each xnDataset In xnRoot.ChildNodes
                Dim guid As Guid
                Dim xa As XmlAttribute = xnDataset.Attributes("GUID")
                Dim bDelete As Boolean = False
                If (xa IsNot Nothing) Then
                    Try
                        guid = guid.Parse(xa.InnerText)
                    Catch ex As Exception
                        guid = guid.Empty
                    End Try
                End If
                For Each gTest As Guid In man.Deleted : bDelete = bDelete Or gTest.Equals(gTest) : Next
                For Each ds As ISpatialDataSet In datasets : bDelete = bDelete Or guid.Equals(ds.GUID) : Next
                If bDelete Then lDelete.Add(xnDataset)
            Next
            For Each xnDataset In lDelete
                xnRoot.RemoveChild(xnDataset)
                bChanged = True
            Next
            lDelete.Clear()

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

            xaRoot = CType(xnRoot.Attributes.GetNamedItem("Source"), XmlAttribute)
            If (xaRoot Is Nothing) Then
                xaRoot = doc.CreateAttribute("Source")
                xnRoot.Attributes.Append(xaRoot)
            End If
            xaRoot.InnerText = Me.Source

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
                    If (Array.IndexOf(man.Virtual, ds) = -1) Then

                        xnDataset = doc.CreateElement("Dataset")

                        xaDataset = doc.CreateAttribute("Type")
                        xaDataset.Value = cTypeUtils.TypeToString(ds.GetType)
                        xnDataset.Attributes.Append(xaDataset)

                        xaDataset = doc.CreateAttribute("GUID")
                        xaDataset.Value = Convert.ToString(ds.GUID)
                        xnDataset.Attributes.Append(xaDataset)

                        Try
                            xnDetails = ds.Configuration(doc)
                        Catch ex As Exception
                            xnDetails = Nothing
                        End Try

                        If (xnDetails IsNot Nothing) Then
                            xnDataset.AppendChild(xnDetails)
                            nExported += 1
                        End If

                        ' Add dataset nodes
                        xnRoot.AppendChild(xnDataset)
                        bChanged = True

                    End If
                Else
                    bSuccess = False
                End If
            Next

            ' Save
            Try
                If bChanged Then
                    doc.Save(strFile)
                End If
            Catch ex As Exception
                bSuccess = False
            End Try

            If (bExporting) Then
                ' Send export status message
                Dim msg As cMessage = Nothing
                If bSuccess Then
                    msg = New cMessage(String.Format(My.Resources.CoreMessages.SPATIALTEMPORAL_EXPORT_SUCCESS, nExported, strPath), _
                                       eMessageType.DataExport, eCoreComponentType.External, eMessageImportance.Information)
                    msg.Hyperlink = strPath
                Else
                    msg = New cMessage(String.Format(My.Resources.CoreMessages.SPATIALTEMPORAL_EXPORT_ERROR, strPath), _
                                       eMessageType.DataExport, eCoreComponentType.External, eMessageImportance.Critical)
                End If
                core.Messages.SendMessage(msg)
            End If

            Return bSuccess

        End Function

#End Region ' Internals

    End Class

End Namespace
