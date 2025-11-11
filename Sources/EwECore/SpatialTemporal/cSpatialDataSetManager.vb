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
Imports System.IO
Imports System.Xml
Imports EwEPlugin
Imports EwEUtils.Core
Imports EwEUtils.SpatialData
Imports EwEUtils.SystemUtilities
Imports EwEUtils.Utilities
Imports EwEUtils.Logging
Imports Microsoft.Extensions.Logging
Imports Debug = System.Diagnostics.Debug

#End Region ' Imports

' ToDo: change this class to solely work with cSpatialDataConfigFile instances

' JS 21Feb18: The current dataset system suffers from a few problems
'   FIXED: The incremental save logic is too complicated. It is trying to prevent unloaded datasets from being destroyed, but this can be done easier
'   FIXED: Deleting a dataset does not remove its application(s) in the underlying connection manager, who simply recreates datasets. Ugh.
'   3) The virtual dataset logic can be simplified, but that is less urgent

' JS 22Feb18: Related to the above is the issue of convoluted three-location storage. This is necessary but is fragile.
'   1) Data structures contain all connections as defined in the model. This data is loaded from the model and saved back into it.
'   2) Adapters hold configured connections, read from the data structures, that contain a configured dataset, a configured converter
'   3) The dataset manager is the repository of defined datasets. This is yet a different list than maintained in the connections. 
'      The dataset manager also keeps track which dataset are defined system-wide, and which are defined in the model

' JS 23Feb18: Spatial indexing and caching needs reviewing
'   1) Spatial extents and index status should no longer be serialized between machines. Better to assess on the fly
'   2) The reported extent has no explicit extent, and neither have cached files. In the initial carnations of the STFD projection was assumed to be WGS84 but this could/should change
'   3) As a result, extent reported by datasets should be expressed in Ecospace projection units. The cache should include this projection in directory name - as a checksum on Proj4string?

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

        Private m_lComp As New Dictionary(Of ISpatialDataSet, cDatasetCompatilibity)

        Private m_lAvailable As List(Of ISpatialDataSet) = Nothing

        Private m_core As cCore = Nothing
        Private m_bReadOnly As Boolean = False

        Private m_indexer As cSpatialDatasetIndexer = Nothing
        Private m_bIndexingEnabled As Boolean = False
        Private m_iIndexingSuspendCount As Integer = 0

        Private m_bAllowValidation As Boolean = True
        Private m_bValidationPending As Boolean = False

        ' Current config metadata
        Private m_strConfigFile As String = ""
        Private m_strAuthor As String = ""
        Private m_strContact As String = ""

        Private m_lConfigFiles As List(Of cSpatialDataConfigFile)
        Private ReadOnly m_logger As ILogger = LoggingContext.CreateLogger(Of cSpatialDataSetManager)()

#End Region ' Private vars

#Region " Construction "

        Public Sub New(core As cCore)

            Me.m_core = core

            Me.m_lAvailable = New List(Of ISpatialDataSet)
            Me.m_lConfigFiles = New List(Of cSpatialDataConfigFile)

            Me.m_indexer = New cSpatialDatasetIndexer(core, Me)

            AddHandler Me.m_core.StateMonitor.CoreExecutionStateEvent, AddressOf Me.OnCoreExecutionStateChanged

        End Sub

        Public Sub Dispose() _
            Implements IDisposable.Dispose

            RemoveHandler Me.m_core.StateMonitor.CoreExecutionStateEvent, AddressOf Me.OnCoreExecutionStateChanged

            Me.IndexDataset = Nothing

            Me.m_lAvailable = Nothing
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
        Public Function Load(Optional strFile As String = "", Optional bClearFirst As Boolean = True) As Boolean

            Dim bSuccess As Boolean = False

            If (bClearFirst) Then Me.Clear()

            If (String.IsNullOrWhiteSpace(strFile)) Then
                strFile = Me.CurrentConfigFile()
            End If

            Me.m_strConfigFile = strFile

            ' JS: moved load to dedicated class; with multiple config files we'll need better descriptions of
            '     file content and purpose, etc. This warrants a unique class to maintain this info.
            Dim cfg As New cSpatialDataConfigFile()
            If cfg.Initialize(strFile) Then

                Dim datasets() As ISpatialDataSet = cfg.Load(strFile)
                If (datasets.Count > 0) Then
                    Me.AddRange(datasets, False)
                    Me.DataDescription = cfg.Description
                    Me.DataAuthor = cfg.Author
                    Me.DataContact = cfg.Contact
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
        Public Function Save(Optional strFile As String = "",
                             Optional datasets As ISpatialDataSet() = Nothing,
                             Optional strDescription As String = "",
                             Optional strAuthor As String = "",
                             Optional strContact As String = "",
                             Optional bExportData As Boolean = True) As Boolean

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

            ' Any switch of destination other than to the default location is considered as an export
            Dim bExporting As Boolean = (cFileUtils.Equals(strFile, cSpatialDataSetManager.DefaultConfigFile) = False) And
                                        (cFileUtils.Equals(strFile, Me.CurrentConfigFile()) = False) And bExportData

            ' No need to do this ;-)
            If (bExporting And datasets.Length = 0) Then Return False

            ' Complement missing properties
            If (String.IsNullOrWhiteSpace(strAuthor)) Then strAuthor = Me.DataAuthor
            If (String.IsNullOrWhiteSpace(strContact)) Then strContact = Me.DataContact
            If (String.IsNullOrWhiteSpace(strDescription)) Then strDescription = Me.DataDescription

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
                Dim cfg As New cSpatialDataConfigFile(strFile,
                                                      Path.GetFileNameWithoutExtension(strFile),
                                                      strDescription,
                                                      cSystemUtils.GetHostName(),
                                                      strAuthor,
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
                    ' Need full reassessment
                    Me.m_lComp.Clear()

                    'Notify the world
                    RaiseEvent OnConfigurationChanged(Me)
                Catch ex As Exception
                    m_logger.LogError(ex, "cSpatialDatasetManager.Update")
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
        ''' Get/set user preference for indexing datasets.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property IsIndexingEnabled As Boolean
            Get
                Return Me.m_bIndexingEnabled
            End Get
            Set(value As Boolean)
                If (value <> Me.m_bIndexingEnabled) Then
                    Me.m_bIndexingEnabled = value
                    Me.UpdateIndexer()
                End If
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set whether the indexer is paused.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Sub SuspendIndexing()
            Me.m_iIndexingSuspendCount += 1
            Me.UpdateIndexer()
        End Sub

        Public Sub ResumeIndexing()
            Me.m_iIndexingSuspendCount -= 1
            Me.UpdateIndexer()
        End Sub

        Private Sub UpdateIndexer()
            Dim sm As cCoreStateMonitor = Me.m_core.StateMonitor
            Me.m_indexer.Enabled = (Not sm.IsBusy()) And (sm.HasEcospaceLoaded = True) And
                                   (Me.IsIndexingEnabled = True) And
                                   (Me.m_iIndexingSuspendCount <= 0)

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the dataset to index.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property IndexDataset As ISpatialDataSet
            Get
                If (Not Me.IsIndexingEnabled) Then
                    Return Nothing
                End If
                Return Me.m_indexer.Current
            End Get
            Set(ds As ISpatialDataSet)
                If (Me.IsIndexingEnabled) Then
                    Me.m_indexer.Prioritize(ds)
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

#End Region ' Dataset indexing

#Region " Dataset list interface "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Determine if one or more datasets are already defined. The dataset check
        ''' is performed by checking the <see cref="ISpatialDataSet.GUID"/>.
        ''' </summary>
        ''' <param name="datasets">The dataset(s) to check.</param>
        ''' <returns>True if one or more dataset(s) are already present.</returns>
        ''' -------------------------------------------------------------------
        Public Function Contains(datasets As ICollection(Of ISpatialDataSet)) As Boolean
            If (datasets Is Nothing) Then Return False
            For Each ds As ISpatialDataSet In datasets
                If (Not (ds.GUID.Equals(Guid.Empty))) Then
                    If (Me.Find(ds.GUID) IsNot Nothing) Then Return True
                End If
            Next
            Return False
        End Function

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="ICollection(Of ISpatialDataSet).Add"/>
        ''' -------------------------------------------------------------------
        Public Sub Add(item As ISpatialDataSet) _
            Implements ICollection(Of ISpatialDataSet).Add
            Me.m_lAvailable.Add(item)
            ' Assign ID if necessary
            If (Guid.Equals(Guid.Empty, item.GUID)) Then
                item.GUID = Guid.NewGuid()
            End If
            Me.UpdateIndexer()
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' <see cref="Add(ISpatialDataSet)"/> a range of spatial datasets.
        ''' </summary>
        ''' <param name="datasets">The dataset(s) to add.</param>
        ''' <param name="bInvalidateCore">Flag, states whether to flag the EwE
        ''' database as needing for saving.</param>
        ''' -------------------------------------------------------------------
        Public Sub AddRange(datasets As ICollection(Of ISpatialDataSet), bInvalidateCore As Boolean)
            For Each ds As ISpatialDataSet In datasets
                If (ds IsNot Nothing) Then
                    Dim bAdd As Boolean = False
                    If (Not (ds.GUID.Equals(Guid.Empty))) Then
                        bAdd = (Me.Find(ds.GUID) Is Nothing)
                    End If
                    If bAdd Then
                        If TypeOf ds Is IPlugin Then DirectCast(ds, IPlugin).Initialize(Me.m_core)
                        Me.Add(ds)
                    End If
                End If
            Next
            If bInvalidateCore Then Me.Changed()
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' <see cref="Remove(ISpatialDataSet)"/> a range of spatial datasets.
        ''' </summary>
        ''' <param name="datasets">The dataset(s) to add.</param>
        ''' <param name="bInvalidateCore">Flag, states whether to flag the EwE
        ''' database as needing for saving.</param>
        ''' -------------------------------------------------------------------
        Public Sub RemoveRange(datasets As ICollection(Of ISpatialDataSet), bInvalidateCore As Boolean)
            For Each ds As ISpatialDataSet In datasets
                If (ds IsNot Nothing) Then
                    If (Not (ds.GUID.Equals(Guid.Empty))) Then
                        Dim dsTest As ISpatialDataSet = Me.Find(ds.GUID)
                        If (dsTest IsNot Nothing) Then
                            Me.Remove(ds)
                        End If
                    End If
                End If
            Next
            If bInvalidateCore Then Me.Changed()
        End Sub

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="ICollection(Of ISpatialDataSet).Clear"/>
        ''' -------------------------------------------------------------------
        Public Sub Clear() _
            Implements ICollection(Of ISpatialDataSet).Clear
            Me.m_lAvailable.Clear()
            Me.m_lComp.Clear()
            Me.UpdateIndexer()
        End Sub

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="ICollection(Of ISpatialDataSet).Contains"/>
        ''' -------------------------------------------------------------------
        Public Function Contains(item As ISpatialDataSet) As Boolean _
            Implements ICollection(Of ISpatialDataSet).Contains
            If (item Is Nothing) Then Return False
            Return Me.m_lAvailable.Contains(item)
        End Function

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="ICollection(Of ISpatialDataSet).CopyTo"/>
        ''' -------------------------------------------------------------------
        Public Sub CopyTo(array() As ISpatialDataSet, arrayIndex As Integer) _
            Implements ICollection(Of ISpatialDataSet).CopyTo
            Me.m_lAvailable.CopyTo(array, arrayIndex)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="ICollection(Of ISpatialDataSet).Count"/>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property Count As Integer _
            Implements ICollection(Of ISpatialDataSet).Count
            Get
                Return Me.m_lAvailable.Count
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="ICollection(Of ISpatialDataSet).IsReadOnly"/>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property IsReadOnly As Boolean _
            Implements ICollection(Of ISpatialDataSet).IsReadOnly
            Get
                Return Me.m_bReadOnly
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="ICollection(Of ISpatialDataSet).Remove"/>
        ''' -------------------------------------------------------------------
        Public Function Remove(item As ISpatialDataSet) As Boolean _
            Implements ICollection(Of ISpatialDataSet).Remove
            If (item Is Nothing) Then Return False
            Dim bOK As Boolean = Me.m_lAvailable.Remove(item)
            Me.UpdateIndexer()

            Try
                ' Kapow!
                Me.m_core.SpatialDataConnectionManager.OnDatasetRemoved(item)
            Catch ex As Exception

            End Try

            Return bOK
        End Function

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="ICollection(Of ISpatialDataSet).GetEnumerator"/>
        ''' -------------------------------------------------------------------
        Public Sub RemoveAt(index As Integer) _
            Implements IList(Of ISpatialDataSet).RemoveAt
            Me.Remove(Me.m_lAvailable(index))
        End Sub

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="ICollection(Of ISpatialDataSet).GetEnumerator"/>
        ''' -------------------------------------------------------------------
        Public Function GetEnumerator() As IEnumerator(Of ISpatialDataSet) _
            Implements IEnumerable(Of ISpatialDataSet).GetEnumerator
            Return Me.m_lAvailable.GetEnumerator
        End Function

        Private Function InaccessibleGetEnumerator() As System.Collections.IEnumerator _
            Implements System.Collections.IEnumerable.GetEnumerator
            Return Nothing
        End Function

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="ICollection(Of ISpatialDataSet).GetEnumerator"/>
        ''' -------------------------------------------------------------------
        Public Function IndexOf(item As ISpatialDataSet) As Integer _
             Implements IList(Of ISpatialDataSet).IndexOf
            Return Me.m_lAvailable.IndexOf(item)
        End Function

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="ICollection(Of ISpatialDataSet).GetEnumerator"/>
        ''' -------------------------------------------------------------------
        Private Sub InaccessibleInsert(index As Integer, item As ISpatialDataSet) _
            Implements IList(Of ISpatialDataSet).Insert
            Me.m_lAvailable.Insert(index, item)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="ICollection(Of ISpatialDataSet).GetEnumerator"/>
        ''' -------------------------------------------------------------------
        Default Public Property Item(index As Integer) As ISpatialDataSet _
            Implements IList(Of ISpatialDataSet).Item
            Get
                Return Me.m_lAvailable.Item(index)
            End Get
            Protected Set(value As ISpatialDataSet)
                Me.m_lAvailable.Item(index) = value
            End Set
        End Property

        Public Function Find(guidDS As Guid) As ISpatialDataSet

            If Guid.Equals(guidDS, Guid.Empty) Then
                Console.WriteLine("Cannot search for an unknown dataset")
                Return Nothing
            End If

            For Each ds As ISpatialDataSet In Me.m_lAvailable
                If (guidDS.Equals(ds.GUID)) Then Return ds
            Next
            Return Nothing
        End Function

        Public Function Find(strName As String) As ISpatialDataSet

            If String.IsNullOrWhiteSpace(strName) Then
                Console.WriteLine("Cannot search for an unknown dataset")
                Return Nothing
            End If

            For Each ds As ISpatialDataSet In Me.m_lAvailable
                If (String.Compare(ds.CustomName, strName, True) = 0) Then Return ds
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
                If ((var = eVarNameFlags.NotSet) Or
                    (ds.VarName = eVarNameFlags.NotSet) Or
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

#End Region ' Internal lists

#Region " Config files "

        Public Event OnConfigurationChanged(sender As cSpatialDataSetManager)

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the paths to all defined config files.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property ConfigFiles As ArrayList
            Get
                Dim al As New ArrayList()
                For Each cfg As cSpatialDataConfigFile In Me.ConfigFileDefinitions
                    al.Add(cfg.FileName)
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
        Public Function CreateConfigFile(strFile As String,
                                         strName As String,
                                         strDescription As String) As cSpatialDataConfigFile

            Dim cfg As cSpatialDataConfigFile = Nothing

            Dim n As Integer = -1
            ' Check if file name does not exist
            For i As Integer = 0 To Me.m_lConfigFiles.Count - 1
                ' Do something smart here
                If String.Compare(Me.m_lConfigFiles(i).FileName, strFile, True) = 0 Then
                    n = i
                    Exit For
                End If
            Next
            If (n >= 0) Then Me.m_lConfigFiles.RemoveAt(n)

            cfg = New cSpatialDataConfigFile(strFile, strName, strDescription, cSystemUtils.GetHostName(), Me.DataAuthor, Me.DataContact)
            cfg.Save(Me.m_core, Me, Nothing, False)
            Me.m_lConfigFiles.Add(cfg)
            Return cfg

        End Function

        Public Function AddConfigFile(strFile As String) As cSpatialDataConfigFile

            ' Abort on missing info
            If (String.IsNullOrWhiteSpace(strFile)) Then Return Nothing

            Dim cfg As cSpatialDataConfigFile = Nothing

            ' Abort on duplicate entry
            For Each cfg In Me.m_lConfigFiles
                ' Do something smart here
                If String.Compare(cfg.FileName, strFile, True) = 0 Then Return Nothing
            Next

            ' Abort on missing file
            If (Not File.Exists(strFile)) Then Return Nothing

            ' Ok, go for it
            cfg = New cSpatialDataConfigFile()
            If (Not cfg.Initialize(strFile)) Then Return Nothing
            Me.m_lConfigFiles.Add(cfg)
            Return cfg

        End Function

#End Region ' Config files

#Region " Data ownership "

        Public Property DataAuthor As String
            Get
                If (String.IsNullOrWhiteSpace(Me.m_strAuthor)) Then Return Me.m_core.DefaultAuthor
                Return Me.m_strAuthor
            End Get
            Set(value As String)
                Me.m_strAuthor = value
            End Set
        End Property

        Public Property DataContact As String
            Get
                If (String.IsNullOrWhiteSpace(Me.m_strContact)) Then Return Me.m_core.DefaultContact
                Return Me.m_strContact
            End Get
            Set(value As String)
                Me.m_strContact = value
            End Set
        End Property

        Public Property DataDescription As String = ""

#End Region ' Data ownership

#Region " Compatibility "

        Public Function Compatibility(ds As ISpatialDataSet) As cDatasetCompatilibity

            If (ds Is Nothing) Then Return Nothing
            If Not Me.m_lComp.ContainsKey(ds) Then
                Me.m_lComp(ds) = New cDatasetCompatilibity(Me.m_core, ds)
            End If
            Return Me.m_lComp(ds)

        End Function

#End Region ' Compatibility

#Region " Events "

        Private Sub OnCoreExecutionStateChanged(statemonitor As cCoreStateMonitor)
            Me.UpdateIndexer()
        End Sub

        Public Overrides Function StopRun(Optional WaitTimeInMillSec As Integer = -1) As Boolean
            Return Me.m_indexer.StopRun(WaitTimeInMillSec)
        End Function

#End Region ' Events

#Region " Internals "

        Friend Shared Function NewDoc(ByRef xnRoot As XmlNode) As XmlDocument
            Return cXMLUtils.NewDoc("Datasets", xnRoot)
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Create a converter from provided configuration info.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Friend ReadOnly Property CreateConverter(cfg As cSpatialDataStructures.cAdapaterConfiguration) As ISpatialDataConverter
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

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Store a dataset into provided configuration info.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Friend Function UpdateDataset(ds As ISpatialDataSet,
                                      cfg As cSpatialDataStructures.cAdapaterConfiguration) As Boolean

            If (ds Is Nothing) Then
                cfg.DatasetTypeName = ""
                cfg.DatasetGUID = ""
            Else
                cfg.DatasetTypeName = cTypeUtils.TypeToString(ds.GetType)
                cfg.DatasetGUID = ds.GUID.ToString
            End If
            Return True

        End Function

        Friend Function UpdateConverter(cv As ISpatialDataConverter,
                                        cfg As cSpatialDataStructures.cAdapaterConfiguration) As Boolean

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
        'Private Sub OnConfigFileChanged(sender As Object, args As FileSystemEventArgs)

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
        Public Function Save(strFile As String, bExport As Boolean, Optional description As String = "", Optional author As String = "", Optional contact As String = "", Optional station As String = "") As Boolean

            Dim doc As New XmlDocument()
            Dim strPath As String = Path.GetDirectoryName(strFile)
            Dim xnRoot As XmlNode = Nothing
            Dim xnDataset As XmlNode = Nothing
            Dim xnDetails As XmlNode = Nothing
            Dim xaDataset As XmlAttribute = Nothing
            Dim bMustSave As Boolean = bExport
            Dim nDataset As Integer = 0
            Dim bSuccess As Boolean = True

            ' Create directory
            If Not cFileUtils.IsDirectoryAvailable(Path.GetDirectoryName(strFile), True) Then
                Return False
            End If

            ' Build new base doc
            doc = cSpatialDataSetManager.NewDoc(xnRoot)
            If (Not String.IsNullOrWhiteSpace(description)) Then
                xaDataset = doc.CreateAttribute("Description")
                xaDataset.Value = description
                doc.Attributes.Append(xaDataset)
            End If
            If (Not String.IsNullOrWhiteSpace(author)) Then
                xaDataset = doc.CreateAttribute("Author")
                xaDataset.Value = author
                doc.Attributes.Append(xaDataset)
            End If
            If (Not String.IsNullOrWhiteSpace(contact)) Then
                xaDataset = doc.CreateAttribute("Contact")
                xaDataset.Value = contact
                doc.Attributes.Append(xaDataset)
            End If
            If (Not String.IsNullOrWhiteSpace(station)) Then
                xaDataset = doc.CreateAttribute("Station")
                xaDataset.Value = station
                doc.Attributes.Append(xaDataset)
            End If

            For Each ds As ISpatialDataSet In Me.m_lAvailable

                nDataset += 1

                If (bExport) Then
                    Me.SendProgress(cStringUtils.Localize(My.Resources.CoreMessages.EXPORT_PROGRESS_DATASET, ds.CustomName), CInt(100 * nDataset / Me.m_lAvailable.Count))
                    ds = ds.ExportTo(Path.GetDirectoryName(strFile))
                End If
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
                    End If

                    ' Add dataset nodes
                    xnRoot.AppendChild(xnDataset)
                    bMustSave = True

                End If

            Next

            If (bExport) Then
                Me.SendProgress("", 100)
            End If

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

        Private Sub SendProgress(strMessage As String, nProgress As Integer)
            Dim state As eProgressState = eProgressState.Running
            If (nProgress = 0) Then state = eProgressState.Start
            If (nProgress = 100) Then state = eProgressState.Finished
            Dim msg As New cProgressMessage(state, 100.0!, CSng(nProgress), strMessage)
            msg.Source = eCoreComponentType.External
            Try
                Me.m_core.Messages.SendMessage(msg)
            Catch ex As Exception

            End Try
        End Sub

#End Region ' Internals

    End Class

End Namespace ' SpatialData
