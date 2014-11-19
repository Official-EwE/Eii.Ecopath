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
Imports System.IO
Imports System.Reflection
Imports System.Xml
Imports EwEPlugin
Imports EwEUtils.Core
Imports EwEUtils.SpatialData
Imports EwEUtils.SystemUtilities
Imports EwEUtils.Utilities

#End Region ' Imports

' ToDo: change this class to solely work with cSpatialDataConfigFile instances

Namespace SpatialData

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Manager class for loading and saving globally shared spatial data sets.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cSpatialDataSetManager
        Inherits cThreadWaitBase
        Implements IList(Of ISpatialDataSet)
        Implements IDisposable

#Region " Private vars "

        Private Shared cCONFIG_FILE As String = "ewe_datasets.xml"

        Private m_lAvailable As List(Of ISpatialDataSet) = Nothing
        Private m_lDeleted As List(Of Guid) = Nothing
        Private m_lVirtual As List(Of ISpatialDataSet) = Nothing

        'Private m_fswSpy As FileSystemWatcher = Nothing
        Private m_core As cCore = Nothing
        Private m_bReadOnly As Boolean = False

        Private m_indexer As cSpatialDatasetIndexer = Nothing
        Private m_bIndexingAllowed As Boolean = False

        Private m_bAllowValidation As Boolean = True
        Private m_bValidationPending As Boolean = False

        ' Current config metadata
        Private m_strConfigFile As String = ""
        Private m_strAuthor As String = ""
        Private m_strContact As String = ""
        Private m_strDescription As String = ""

        Private m_lConfigFiles As List(Of cSpatialDataConfigFile)

#End Region ' Private vars

#Region " Construction "

        Public Sub New(core As cCore)

            Me.m_core = core

            Me.m_lAvailable = New List(Of ISpatialDataSet)
            Me.m_lDeleted = New List(Of Guid)
            Me.m_lVirtual = New List(Of ISpatialDataSet)
            Me.m_lConfigFiles = New List(Of cSpatialDataConfigFile)

            '' Create folder watcher
            'Me.m_fswSpy = New FileSystemWatcher()
            'Me.m_fswSpy.Path = Path.GetDirectoryName(cSpatialDataSetManager.DefaultConfigFileName())
            'Me.m_fswSpy.NotifyFilter = NotifyFilters.LastWrite
            'Me.m_fswSpy.Filter = "*.xml"
            'Me.m_fswSpy.EnableRaisingEvents = True

            Me.m_indexer = New cSpatialDatasetIndexer(core)

            'AddHandler Me.m_fswSpy.Changed, AddressOf OnConfigFileChanged

        End Sub

        Public Sub Dispose() _
            Implements IDisposable.Dispose

            Me.IndexDataset = Nothing

            ' Cleanup
            'If (Me.m_fswSpy IsNot Nothing) Then
            '    RemoveHandler Me.m_fswSpy.Changed, AddressOf OnConfigFileChanged
            '    Me.m_fswSpy = Nothing
            'End If

            Me.m_lAvailable = Nothing
            Me.m_lDeleted = Nothing
            Me.m_lVirtual = Nothing
            GC.SuppressFinalize(Me)

        End Sub

#End Region ' Construction

#Region " Persistent storage "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns the full path to the configuration file. This creates the directory if needed.
        ''' </summary>
        ''' <returns>The full path to the configuration file.</returns>
        ''' -------------------------------------------------------------------
        Public Shared Function DefaultConfigFile() As String

            Dim strFolder As String = cSystemUtils.ApplicationSettingsPath()
            Return Path.Combine(strFolder, cCONFIG_FILE)

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the full path to the current active config file.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property CurrentConfigFile As String
            Get
                If (Not String.IsNullOrWhiteSpace(Me.m_strConfigFile)) Then Return Me.m_strConfigFile
                Return DefaultConfigFile()
            End Get
        End Property

        Public Function Reload(Optional bClearFirst As Boolean = True) As Boolean
            Me.Load(Me.CurrentConfigFile, bClearFirst)
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Initializes the manager with datasets, loaded from persistent storage.
        ''' </summary>
        ''' <param name="strFile">The name of the file to load. If not specified, 
        ''' the <see cref="cSpatialDataSetManager.DefaultConfigFile">default configuration file</see>
        ''' is used.</param>
        ''' <param name="bClearFirst">Flag, stating that the content currently in 
        ''' the manager should be cleared first.</param>
        ''' <returns>False if the config file is corrupted, True otherwise.</returns>
        ''' <remarks>This method can also be used to import extra datasets.</remarks>
        ''' -------------------------------------------------------------------
        Public Function Load(Optional strFile As String = "", _
                             Optional bClearFirst As Boolean = True) As Boolean

            Dim bSuccess As Boolean = False

            If (bClearFirst) Then Me.Clear()

            If (String.IsNullOrWhiteSpace(strFile)) Then strFile = cSpatialDataSetManager.DefaultConfigFile()

            Me.m_strConfigFile = strFile

            ' JS: moved load to dedicated class; with multiple config files we'll need better descriptions of
            '     file content and purpose, etc. This warrants a unique class to maintain this info.
            Dim cfg As New cSpatialDataConfigFile()
            If cfg.Initialize(strFile) Then
                If cfg.Load(Me.m_core, Me) Then

                    Me.m_strDescription = cfg.Description
                    Me.m_strAuthor = cfg.Author
                    Me.m_strContact = cfg.Contact

                    bSuccess = True
                End If
            End If

            Me.Changed()

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
        Public Function Save(Optional strFile As String = "", _
                             Optional datasets As ISpatialDataSet() = Nothing, _
                             Optional strDescription As String = "", _
                             Optional strAuthor As String = "", _
                             Optional strContact As String = "") As Boolean

            Dim bChanged As Boolean = False
            Dim nExported As Integer = 0
            Dim strPath As String = ""
            Dim bSuccess As Boolean = True

            ' Complete missing file name, if any
            If (String.IsNullOrWhiteSpace(strFile)) Then
                strFile = Me.CurrentConfigFile()
            End If

            If (datasets Is Nothing) Then
                datasets = Me.Datasets()
            End If
            If (datasets.Length = 0) Then Return False

            If (String.IsNullOrWhiteSpace(strAuthor)) Then strAuthor = Me.DataAuthor
            If (String.IsNullOrWhiteSpace(strContact)) Then strContact = Me.DataContact

            ' Any switch of destination other than to the default location is considered as an export
            Dim bExporting As Boolean = (cFileUtils.Equals(strFile, cSpatialDataSetManager.DefaultConfigFile) = False) And _
                                        (cFileUtils.Equals(strFile, Me.CurrentConfigFile()) = False)

            If bExporting Then
                Console.WriteLine("@@ Exporting from " & Me.CurrentConfigFile & " to " & strFile)
                'Stop
            End If

            ' Create dir
            strPath = Path.GetDirectoryName(strFile)
            If Not cFileUtils.IsDirectoryAvailable(strPath, True) Then
                Return False
            End If

            ' During the export process the dataset manager has to set its config file to the export path
            ' in order for file-based datasets to resolve absolute / relative paths. At the end of the
            ' export process the path is restored
            Dim strRescue As String = Me.m_strConfigFile
            Me.m_strConfigFile = strFile

            ' Make sure save exceptions do not affect current configuration
            Try
                Dim cfg As New cSpatialDataConfigFile(strFile, _
                                                      Path.GetFileNameWithoutExtension(strFile), _
                                                      strDescription, _
                                                      cSystemUtils.GetHostName(), _
                                                      strAuthor, _
                                                      strContact)
                bSuccess = cfg.Save(Me.m_core, Me, datasets, bExporting)
            Catch ex As Exception
                ' NOP
                Debug.Assert(False, ex.Message)
            End Try

            ' Always restore original config file name
            Me.m_strConfigFile = strRescue

            Return bSuccess

        End Function

        Public Property AllowValidation As Boolean
            Get
                Return Me.m_bAllowValidation
            End Get
            Set(value As Boolean)
                Me.m_bAllowValidation = value
                If Me.m_bAllowValidation And Me.m_bValidationPending Then
                    Me.Changed()
                End If
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Send a change notification
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Sub Changed()
            If Me.AllowValidation Then
                Try
                    'Notify the world
                    RaiseEvent OnConfigurationChanged(Me)
                Catch ex As Exception
                    cLog.Write(ex, "cSpatialDatasetManager.Update")
                End Try
                Me.m_bValidationPending = False
            Else
                Me.m_bValidationPending = True
            End If
        End Sub

#End Region ' Persistent storage    

#Region " Dataset indexing "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set whether spatial dataset indexing is allowed.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property IsIndexingAllowed As Boolean
            Get
                Return m_bIndexingAllowed
            End Get
            Set(value As Boolean)
                Me.m_bIndexingAllowed = value
                If Not Me.m_bIndexingAllowed Then
                    Me.m_indexer.Add(Nothing)
                End If
            End Set
        End Property

        Public Overrides Function StopRun(Optional WaitTimeInMillSec As Integer = -1) As Boolean
            Dim result As Boolean = True
            Try
                Me.IndexDataset = Nothing
                result = Me.Wait(WaitTimeInMillSec)
            Catch ex As Exception
                result = False
            End Try
            Return result
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the dataset to index.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property IndexDataset As ISpatialDataSet
            Get
                If (Not Me.IsIndexingAllowed) Then
                    Return Nothing
                End If
                Return Me.m_indexer.Current
            End Get
            Set(ds As ISpatialDataSet)
                If (Me.IsIndexingAllowed) Then
                    Me.m_indexer.Add(ds)
                End If
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns whether a dataset is being indexed.
        ''' </summary>
        ''' <param name="ds">The dataset to check. If this parameter is omitted 
        ''' this method will return whether any dataset is being indexed.</param>
        ''' <returns>True if a dataset is being indexed.</returns>
        ''' -------------------------------------------------------------------
        Public Function IsIndexing(Optional ds As ISpatialDataSet = Nothing) As Boolean
            Return Me.m_indexer.IsIndexing(ds)
        End Function

        Public Sub StopIndexing()
            Me.StopRun(5000)
        End Sub

#End Region ' Dataset indexing

#Region " Dataset list interface "

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="ICollection(Of ISpatialDataSet).Add"/>
        ''' -------------------------------------------------------------------
        Public Sub Add(ByVal item As ISpatialDataSet) _
            Implements System.Collections.Generic.ICollection(Of ISpatialDataSet).Add
            Me.m_lAvailable.Add(item)
            ' Assign ID if necessary
            If (Guid.Equals(Guid.Empty, item.GUID)) Then
                item.GUID = Guid.NewGuid()
            End If
        End Sub

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="ICollection(Of ISpatialDataSet).Clear"/>
        ''' -------------------------------------------------------------------
        Private Sub Clear() _
            Implements System.Collections.Generic.ICollection(Of ISpatialDataSet).Clear
            Me.m_indexer.Stop()
            Me.m_lAvailable.Clear()
            Me.m_lVirtual.Clear()
            Me.m_lDeleted.Clear()
        End Sub

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="ICollection(Of ISpatialDataSet).Contains"/>
        ''' -------------------------------------------------------------------
        Public Function Contains(ByVal item As ISpatialDataSet) As Boolean _
            Implements System.Collections.Generic.ICollection(Of ISpatialDataSet).Contains
            If (item Is Nothing) Then Return False
            Return Me.m_lAvailable.Contains(item)
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set whether a given dataset is declared virtually by the 
        ''' loaded model, instead of via a configuration file.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property IsVirtual(ByVal item As ISpatialDataSet) As Boolean
            Get
                If (item Is Nothing) Then Return False
                Return Me.m_lVirtual.Contains(item)
            End Get
            Set(value As Boolean)
                If (item Is Nothing) Then Return
                If (value = True) Then
                    If Not Me.m_lVirtual.Contains(item) Then Me.m_lVirtual.Add(item)
                Else
                    Me.m_lVirtual.Remove(item)
                End If
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="ICollection(Of ISpatialDataSet).CopyTo"/>
        ''' -------------------------------------------------------------------
        Public Sub CopyTo(ByVal array() As ISpatialDataSet, ByVal arrayIndex As Integer) _
            Implements System.Collections.Generic.ICollection(Of ISpatialDataSet).CopyTo
            Me.m_lAvailable.CopyTo(array, arrayIndex)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="ICollection(Of ISpatialDataSet).Count"/>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property Count As Integer _
            Implements System.Collections.Generic.ICollection(Of ISpatialDataSet).Count
            Get
                Return Me.m_lAvailable.Count
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="ICollection(Of ISpatialDataSet).IsReadOnly"/>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property IsReadOnly As Boolean _
            Implements System.Collections.Generic.ICollection(Of ISpatialDataSet).IsReadOnly
            Get
                Return Me.m_bReadOnly
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="ICollection(Of ISpatialDataSet).Remove"/>
        ''' -------------------------------------------------------------------
        Public Function Remove(ByVal item As ISpatialDataSet) As Boolean _
            Implements System.Collections.Generic.ICollection(Of ISpatialDataSet).Remove
            If (item Is Nothing) Then Return False
            If (Me.IsIndexing(item)) Then Me.IndexDataset = Nothing
            Me.m_lDeleted.Add(item.GUID)
            Return Me.m_lAvailable.Remove(item)
        End Function

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="ICollection(Of ISpatialDataSet).GetEnumerator"/>
        ''' -------------------------------------------------------------------
        Public Sub RemoveAt(ByVal index As Integer) _
            Implements IList(Of ISpatialDataSet).RemoveAt
            Me.Remove(Me.m_lAvailable(index))
        End Sub

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="ICollection(Of ISpatialDataSet).GetEnumerator"/>
        ''' -------------------------------------------------------------------
        Public Function GetEnumerator() As System.Collections.Generic.IEnumerator(Of ISpatialDataSet) _
            Implements System.Collections.Generic.IEnumerable(Of ISpatialDataSet).GetEnumerator
            Return Me.m_lAvailable.GetEnumerator
        End Function

        Private Function InaccessibleGetEnumerator() As System.Collections.IEnumerator _
            Implements System.Collections.IEnumerable.GetEnumerator
            Return Nothing
        End Function

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="ICollection(Of ISpatialDataSet).GetEnumerator"/>
        ''' -------------------------------------------------------------------
        Public Function IndexOf(ByVal item As ISpatialDataSet) As Integer _
             Implements IList(Of ISpatialDataSet).IndexOf
            Return Me.m_lAvailable.IndexOf(item)
        End Function

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="ICollection(Of ISpatialDataSet).GetEnumerator"/>
        ''' -------------------------------------------------------------------
        Private Sub InaccessibleInsert(ByVal index As Integer, ByVal item As ISpatialDataSet) _
            Implements IList(Of ISpatialDataSet).Insert
            Me.m_lAvailable.Insert(index, item)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="ICollection(Of ISpatialDataSet).GetEnumerator"/>
        ''' -------------------------------------------------------------------
        Default Public Property Item(ByVal index As Integer) As ISpatialDataSet _
            Implements IList(Of ISpatialDataSet).Item
            Get
                Return Me.m_lAvailable.Item(index)
            End Get
            Protected Set(ByVal value As ISpatialDataSet)
                Me.m_lAvailable.Item(index) = value
            End Set
        End Property

        Public Function Find(ByVal guidDS As Guid) As ISpatialDataSet

            If Guid.Equals(guidDS, Guid.Empty) Then
                Console.WriteLine("Cannot search for an unknown dataset")
                Return Nothing
            End If

            For Each ds As ISpatialDataSet In Me.m_lAvailable
                If (guidDS.Equals(ds.GUID)) Then Return ds
            Next
            Return Nothing
        End Function

        Public Function Find(ByVal strName As String) As ISpatialDataSet

            If String.IsNullOrWhiteSpace(strName) Then
                Console.WriteLine("Cannot search for an unknown dataset")
                Return Nothing
            End If

            For Each ds As ISpatialDataSet In Me.m_lAvailable
                If (String.Compare(ds.DisplayName, strName, True) = 0) Then Return ds
            Next
            Return Nothing
        End Function

#End Region ' Dataset list interface

#Region " Internal lists "

        ''' --------------------------------------------------------------------
        ''' <summary>
        ''' Returns all datasets compatible with a given <see cref="eVarNameFlags">variable</see>.
        ''' </summary>
        ''' <param name="var">The <see cref="eVarNameFlags">variable</see> to filter by.</param>
        ''' <returns>An array of datasets, compatible with the given <paramref name="var">variable</paramref>.</returns>
        ''' --------------------------------------------------------------------
        Public Function Datasets(var As eVarNameFlags) As ISpatialDataSet()
            Dim lFiltered As New List(Of ISpatialDataSet)
            For Each ds As ISpatialDataSet In Me.m_lAvailable
                If ((var = eVarNameFlags.NotSet) Or _
                    (ds.VarName = eVarNameFlags.NotSet) Or _
                    (var = ds.VarName)) Then
                    lFiltered.Add(ds)
                End If
            Next
            Return lFiltered.ToArray()
        End Function

        ''' --------------------------------------------------------------------
        ''' <summary>
        ''' Returns all available datasets.
        ''' </summary>
        ''' <returns>All available datasets.</returns>
        ''' --------------------------------------------------------------------
        Public Function Datasets() As ISpatialDataSet()
            Return Me.m_lAvailable.ToArray()
        End Function

        Friend Function Virtual() As ISpatialDataSet()
            Return Me.m_lVirtual.ToArray()
        End Function

        Friend Function Deleted() As Guid()
            Return Me.m_lDeleted.ToArray()
        End Function

#End Region ' Internal lists

#Region " Config files "

        Public Event OnConfigurationChanged(ByVal sender As cSpatialDataSetManager)

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the paths to all defined config files.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property ConfigFiles As ArrayList
            Get
                Dim al As New ArrayList()
                For Each cfg As cSpatialDataConfigFile In Me.ConfigFileDefinitions
                    If File.Exists(cfg.FileName) Then
                        al.Add(cfg.FileName)
                    End If
                Next
                Return al
            End Get
            Set(value As ArrayList)
                Me.m_lConfigFiles.Clear()
                If (value Is Nothing) Then Return
                For i As Integer = 0 To value.Count - 1
                    If (TypeOf value(i) Is String) Then
                        Me.AddConfigFile(CStr(value(i)))
                    End If
                Next
            End Set
        End Property

        ''' <summary>
        ''' Get all custom configuration files defined on the local system.
        ''' </summary>
        Public ReadOnly Property ConfigFileDefinitions As List(Of cSpatialDataConfigFile)
            Get
                Return Me.m_lConfigFiles
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Creates a new configuration file, and adds it to the internal list of
        ''' defined spatial temporal data configuration files.
        ''' </summary>
        ''' <param name="strFile"></param>
        ''' <param name="strName"></param>
        ''' <param name="strDescription"></param>
        ''' <remarks>The dataset configuration file will inherit the local computer 
        ''' name, and <see cref="cCore.DefaultAuthor">author</see> and <see cref="cCore.DefaultContact">contact</see> 
        ''' information as configured in the core.
        ''' </remarks>
        ''' <returns>The created dataset, or nothing if an error occurred.</returns>
        ''' -------------------------------------------------------------------
        Public Function CreateConfigFile(ByVal strFile As String, _
                                         ByVal strName As String, _
                                         ByVal strDescription As String) As cSpatialDataConfigFile

            Dim cfg As cSpatialDataConfigFile = Nothing

            ' Check if file name does not exist
            For Each cfg In Me.m_lConfigFiles
                ' Do something smart here
                If String.Compare(cfg.FileName, strFile, True) = 0 Then Return Nothing
            Next

            cfg = New cSpatialDataConfigFile(strFile, strName, strDescription, _
                                             cSystemUtils.GetHostName(), _
                                             Me.m_core.DefaultAuthor, Me.m_core.DefaultContact)
            cfg.Save(Me.m_core, Me, Nothing, False)
            Me.m_lConfigFiles.Add(cfg)
            Return cfg

        End Function

        Public Function AddConfigFile(ByVal strFile As String) As cSpatialDataConfigFile

            If (String.IsNullOrWhiteSpace(strFile)) Then Return Nothing

            Dim cfg As cSpatialDataConfigFile = Nothing

            ' Check if file name does not exist
            For Each cfg In Me.m_lConfigFiles
                ' Do something smart here
                If String.Compare(cfg.FileName, strFile, True) = 0 Then Return Nothing
            Next

            cfg = New cSpatialDataConfigFile()
            If (Not cfg.Initialize(strFile)) Then Return Nothing
            Me.m_lConfigFiles.Add(cfg)
            Return cfg

        End Function

#End Region ' Config files

#Region " Data ownership "

        Public ReadOnly Property DataAuthor As String
            Get
                If (String.IsNullOrWhiteSpace(Me.m_strAuthor)) Then Return Me.m_core.DefaultAuthor
                Return Me.m_strAuthor
            End Get
        End Property

        Public ReadOnly Property DataContact As String
            Get
                If (String.IsNullOrWhiteSpace(Me.m_strContact)) Then Return Me.m_core.DefaultContact
                Return Me.m_strContact
            End Get
        End Property

        Public ReadOnly Property DataDescription As String
            Get
                Return Me.m_strDescription
            End Get
        End Property

#End Region ' Data ownership

#Region " Internals "

        Friend Shared Function NewDoc(ByRef xnRoot As XmlNode) As XmlDocument
            Dim doc As New XmlDocument()
            Dim xnData As XmlElement = Nothing
            Dim xaData As XmlAttribute = Nothing
            doc.AppendChild(doc.CreateXmlDeclaration("1.0", "", "yes"))
            xnRoot = doc.CreateElement("Datasets")
            doc.AppendChild(xnRoot)
            Return doc
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Return an existing dataset if available. If not available, a 
        ''' dataset is dynamically created from provided configuration info.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Friend Function CreateDataset(ByVal cfg As cSpatialDataStructures.cAdapaterConfiguration) As ISpatialDataSet
            If (String.IsNullOrWhiteSpace(cfg.DatasetGUID)) Then Return Nothing

            Dim guidDS As Guid = Guid.Empty
            Guid.TryParse(cfg.DatasetGUID, guidDS)

            Dim ds As ISpatialDataSet = Me.Find(guidDS)

            If (ds Is Nothing) Then

                ' Abort if missing dataset creation info
                If (String.IsNullOrWhiteSpace(cfg.DatasetTypeName)) Then Return Nothing

                ' Abort if dataset cannot be instantiated
                ' yuck...
                Dim t As Type = cTypeUtils.StringToType(cfg.DatasetTypeName.Replace("cAAASFileDataSetPlugin", "cASCIIFilesDataSetPlugin"))
                If (t Is Nothing) Then Return Nothing

                Try
                    ds = DirectCast(Activator.CreateInstance(t), ISpatialDataSet)
                    If (TypeOf ds Is IPlugin) Then DirectCast(ds, IPlugin).Initialize(Me.m_core)

                    ' This needs some restructuring. Perhaps it is easiest to add XML serializer classes
                    ' for datasets and converters. This XML logic is becoming too fragmented

                    If Not String.IsNullOrWhiteSpace(cfg.DatasetConfig) Then
                        Dim xnRoot As XmlNode = Nothing
                        Dim doc As XmlDocument = cSpatialDataSetManager.NewDoc(xnRoot)
                        Dim xnData As XmlElement = doc.CreateElement("Configuration")
                        xnData.InnerXml = cfg.DatasetConfig

                        ds.Configuration(doc) = xnData

                        ' Try to find Dataset by name 
                        Dim ds2 As ISpatialDataSet = Me.Find(ds.DisplayName)
                        If (ds2 IsNot Nothing) Then
                            ' Ok, use that one
                            ds = ds2
                        Else
                            ' Dataset is new? try to complement GUID and use it as a virtual dataset
                            If Guid.Equals(Guid.Empty, guidDS) Then guidDS = Guid.NewGuid
                            ds.GUID = guidDS
                            Me.m_lAvailable.Add(ds)
                        End If

                        ' This dataset is obtained from the model, not from a properly defined dataset
                        Me.IsVirtual(ds) = True
                    End If

                Catch ex As Exception
                    cLog.Write(ex, "cSpatialDatasetManager.CreateDataset " & cfg.DatasetTypeName)
                End Try


            End If

            Return ds

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Store a dataset into provided configuration info.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Friend Function UpdateDataset(ByVal ds As ISpatialDataSet, _
                                      ByVal cfg As cSpatialDataStructures.cAdapaterConfiguration) As Boolean

            If (ds Is Nothing) Then
                cfg.DatasetTypeName = ""
                cfg.DatasetGUID = ""
                cfg.DatasetConfig = ""
                Return True
            End If

            Dim doc As XmlDocument = Nothing
            Dim xnRoot As XmlNode = Nothing
            Dim xnData As XmlNode = Nothing

            Try
                doc = cSpatialDataSetManager.NewDoc(xnRoot)

                cfg.DatasetTypeName = cTypeUtils.TypeToString(ds.GetType)
                cfg.DatasetGUID = ds.GUID.ToString

                xnData = ds.Configuration(doc)

                If (xnData IsNot Nothing) Then
                    cfg.DatasetConfig = xnData.InnerXml
                Else
                    cfg.DatasetConfig = ""
                End If
            Catch ex As Exception

            End Try

            Return True
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Create a converter from provided configuration info.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Friend ReadOnly Property CreateConverter(ByVal cfg As cSpatialDataStructures.cAdapaterConfiguration) As ISpatialDataConverter
            Get
                If (String.IsNullOrWhiteSpace(cfg.ConverterTypeName)) Then Return Nothing

                Dim cv As ISpatialDataConverter = Nothing
                Dim t As Type = cTypeUtils.StringToType(cfg.ConverterTypeName)
                If (t Is Nothing) Then Return Nothing

                Try
                    cv = DirectCast(Activator.CreateInstance(t), ISpatialDataConverter)
                    ' Properly initialize
                    If (TypeOf cv Is IPlugin) Then
                        DirectCast(cv, IPlugin).Initialize(Me.m_core)
                    End If

                    If Not String.IsNullOrWhiteSpace(cfg.ConverterConfig) Then
                        Dim xnRoot As XmlNode = Nothing
                        Dim doc As XmlDocument = cSpatialDataSetManager.NewDoc(xnRoot)
                        Dim xnData As XmlElement = doc.CreateElement("Configuration")
                        xnData.InnerXml = cfg.ConverterConfig
                        cv.Configuration(doc) = xnData
                    End If

                Catch ex As Exception

                End Try
                Return cv

            End Get
        End Property

        Friend Function UpdateConverter(ByVal cv As ISpatialDataConverter, _
                                        ByVal cfg As cSpatialDataStructures.cAdapaterConfiguration) As Boolean

            If (cv Is Nothing) Then
                cfg.ConverterTypeName = ""
                cfg.ConverterConfig = ""
                Return True
            End If

            Dim doc As XmlDocument = Nothing
            Dim xnRoot As XmlNode = Nothing
            Dim xnData As XmlNode = Nothing

            Try
                doc = cSpatialDataSetManager.NewDoc(xnRoot)

                cfg.ConverterTypeName = cTypeUtils.TypeToString(cv.GetType)
                xnData = cv.Configuration(doc)

                If (xnData IsNot Nothing) Then
                    cfg.ConverterConfig = xnData.InnerXml
                Else
                    cfg.ConverterConfig = ""
                End If
            Catch ex As Exception

            End Try

            Return True
        End Function

        ' ''' -------------------------------------------------------------------
        ' ''' <summary>
        ' ''' Event handler, invoked when the watched folder has changed.
        ' ''' </summary>
        ' ''' -------------------------------------------------------------------
        'Private Sub OnConfigFileChanged(ByVal sender As Object, ByVal args As FileSystemEventArgs)

        '    If Path.Equals(args.FullPath, cSpatialDataSetManager.DefaultConfigFile()) Then
        '        ' Lock up list
        '        m_bReadOnly = True
        '    End If

        'End Sub

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
        Public Function Save(strFile As String, bExport As Boolean) As Boolean

            Dim doc As New XmlDocument()
            Dim xnRoot As XmlNode = Nothing
            Dim xnDataset As XmlNode = Nothing
            Dim xnDetails As XmlNode = Nothing
            Dim xaDataset As XmlAttribute = Nothing
            Dim datasets As ISpatialDataSet() = Me.Datasets()
            Dim bMustSave As Boolean = bExport
            Dim bSuccess As Boolean = True

            ' Create dir
            If Not cFileUtils.IsDirectoryAvailable(Path.GetDirectoryName(strFile), True) Then
                Return False
            End If

            Try
                ' To make sure that defined datasets for unknown providers (due to missing plug-ins) are not lost
                If File.Exists(strFile) And Not bExport Then
                    doc.Load(strFile)
                    xnRoot = doc.GetElementsByTagName("Datasets")(0)
                End If
            Catch ex As Exception
                ' Plop
            End Try

            If (xnRoot Is Nothing) Then
                ' Build new base doc
                doc = cSpatialDataSetManager.NewDoc(xnRoot)
            End If

            ' Remove all deleted or current datasets
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
                For Each gTest As Guid In Me.m_lDeleted : bDelete = bDelete Or gTest.Equals(gTest) : Next
                For Each ds As ISpatialDataSet In Me.m_lAvailable : bDelete = bDelete Or guid.Equals(ds.GUID) : Next
                If bDelete Then lDelete.Add(xnDataset)
            Next
            For Each xnDataset In lDelete
                xnRoot.RemoveChild(xnDataset)
                bMustSave = True
            Next
            lDelete.Clear()

            ' Gather dataset config nodes, but do not add to the doc until all done
            For Each ds As ISpatialDataSet In datasets

                If (bExport) Then ds = ds.ExportTo(Path.GetDirectoryName(strFile))
                If (ds IsNot Nothing) Then

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
                    End If

                    ' Add dataset nodes
                    xnRoot.AppendChild(xnDataset)
                    bMustSave = True

                End If

            Next

            ' Save
            'Me.m_fswSpy.EnableRaisingEvents = False
            Try
                If bMustSave Then
                    doc.Save(strFile)
                End If
            Catch ex As Exception
                bSuccess = False
            End Try
            'Me.m_fswSpy.EnableRaisingEvents = True

            Return bSuccess

        End Function

#End Region ' Internals

    End Class

End Namespace ' SpatialData
