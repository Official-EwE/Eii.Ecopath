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
' Copyright 1991-2013 UBC Fisheries Centre, Vancouver BC, Canada.
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
        Private m_fswSpy As FileSystemWatcher = Nothing
        Private m_core As cCore = Nothing
        Private m_bReadOnly As Boolean = False

        Private m_indexer As cSpatialDatasetIndexer = Nothing

#End Region ' Private vars

#Region " Construction "

        Public Sub New(core As cCore)

            Me.m_lAvailable = New List(Of ISpatialDataSet)
            Me.m_lDeleted = New List(Of Guid)
            Me.m_core = core

            ' Create folder watcher
            Me.m_fswSpy = New FileSystemWatcher()
            Me.m_fswSpy.Path = Path.GetDirectoryName(cSpatialDataSetManager.ConfigFileName())
            Me.m_fswSpy.NotifyFilter = NotifyFilters.LastWrite
            Me.m_fswSpy.Filter = "*.xml"
            Me.m_fswSpy.EnableRaisingEvents = True

            Me.m_indexer = New cSpatialDatasetIndexer(Me.m_core)

            AddHandler Me.m_fswSpy.Changed, AddressOf OnConfigFileChanged

        End Sub

        Public Sub Dispose() _
            Implements IDisposable.Dispose

            Me.IndexDataset = Nothing

            ' Cleanup
            If (Me.m_fswSpy IsNot Nothing) Then
                RemoveHandler Me.m_fswSpy.Changed, AddressOf OnConfigFileChanged
                Me.m_fswSpy = Nothing
            End If
            Me.m_lAvailable = Nothing
            Me.m_lDeleted = Nothing
            GC.SuppressFinalize(Me)

        End Sub

#End Region ' Construction

#Region " Persistent storage "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Initializes the manager with datasets, loaded from persistent storage.
        ''' </summary>
        ''' <param name="strFile">Optional file to load datasets from. If this 
        ''' parameter is left empty the <see cref="cSpatialDataSetManager.ConfigFileName">default file path</see>
        ''' is used.</param>
        ''' <param name="bClearFirst">Flag, stating that the content currently in 
        ''' the manager should be cleared first.</param>
        ''' <returns>True if successful.</returns>
        ''' <remarks>This method can also be used to import extra datasets.</remarks>
        ''' -------------------------------------------------------------------
        Public Function Load(Optional strFile As String = "", _
                             Optional bClearFirst As Boolean = False) As Boolean

            Dim doc As New XmlDocument()
            Dim xnRoot As XmlNode = Nothing
            Dim xa As XmlAttribute = Nothing
            Dim ds As ISpatialDataSet = Nothing
            Dim an As AssemblyName = Nothing
            Dim bReturn As Boolean = False

            If (bClearFirst) Then Me.Clear()
            If (String.IsNullOrEmpty(strFile)) Then strFile = cSpatialDataSetManager.ConfigFileName()

            'jb if it failed to find the config file shouldn't it return False
            If Not File.Exists(strFile) Then Return False '  True

            ' Load datasets
            doc.Load(strFile)

            For Each xnRoot In doc.GetElementsByTagName("Datasets")
                'Found a "Datasets" tag in the file
                bReturn = True
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

                                    ds.Configuration(doc) = xn.ChildNodes(0)
                                    xa = xn.Attributes("GUID")
                                    ' Assign GUID
                                    ds.GUID = Guid.Parse(xa.InnerText)

                                    If (TypeOf ds Is IPlugin) Then DirectCast(ds, IPlugin).Initialize(Me.m_core)

                                Else '(t IsNot Nothing)
                                    'Failed to convert the Dataset.Type node into a dataset
                                    Me.m_core.Messages.SendMessage(New cMessage(My.Resources.CoreMessages.SPATIALTEMPORAL_MISSINGDATASET, _
                                                                               eMessageType.ErrorEncountered, eCoreComponentType.External, eMessageImportance.Information))
                                End If

                            Catch ex As Exception
                                ds = Nothing
                                bReturn = False
                                cLog.Write(ex, "Exception loading Spatial Configuration file " + strFile)
                            End Try

                            Dim bAdd As Boolean = False
                            If (ds IsNot Nothing) Then
                                bAdd = True
                                If (Not (ds.GUID.Equals(Guid.Empty))) Then
                                    bAdd = (Me.ItemByGUID(ds.GUID) Is Nothing)
                                End If
                            End If
                            If bAdd Then Me.Add(ds)
                        End If
                    End If
                Next ' xn
            Next ' xnRoot

            Return bReturn

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
                             Optional datasets As ISpatialDataSet() = Nothing) As Boolean

            Dim doc As New XmlDocument()
            Dim xnRoot As XmlNode = Nothing
            Dim xnDataset As XmlNode = Nothing
            Dim xnDetails As XmlNode = Nothing
            Dim xaDataset As XmlAttribute = Nothing
            Dim bChanged As Boolean = False
            Dim bSuccess As Boolean = True

            ' Complete missing file name, if any
            If (String.IsNullOrWhiteSpace(strFile)) Then
                strFile = cSpatialDataSetManager.ConfigFileName()
            End If

            ' Complete missing datasets, if any
            If (datasets Is Nothing) Then
                datasets = Me.m_lAvailable.ToArray()
            End If

            ' Create dir
            If Not cFileUtils.IsDirectoryAvailable(Path.GetDirectoryName(strFile)) Then
                Return False
            End If

            Try
                If File.Exists(strFile) Then
                    doc.Load(strFile)
                    xnRoot = doc.GetElementsByTagName("Datasets")(0)
                End If
            Catch ex As Exception
                ' Plop
            End Try

            If (xnRoot Is Nothing) Then
                ' Wipe and recreate
                doc.RemoveAll()
                doc.AppendChild(doc.CreateXmlDeclaration("1.0", "", "yes"))
                xnRoot = doc.CreateElement("Datasets")
                doc.AppendChild(xnRoot)
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
                bChanged = True
            Next
            lDelete.Clear()

            ' Gather dataset config nodes, but do not add to the doc until all done
            For Each ds As ISpatialDataSet In datasets

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
                bChanged = True
            Next

            ' Save
            Me.m_fswSpy.EnableRaisingEvents = False
            Try
                If bChanged Then
                    doc.Save(ConfigFileName)
                End If
            Catch ex As Exception
                bSuccess = False
            End Try
            Me.m_fswSpy.EnableRaisingEvents = True

            Return bSuccess

        End Function

#End Region ' Persistent storage

#Region " Dataset indexing "

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
                Return Me.m_indexer.Current
            End Get
            Set(ds As ISpatialDataSet)
                Me.m_indexer.Add(ds)
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
        ''' <inheritdocs cref="ICollection(Of ISpatialDataSet).Add"/>
        ''' -------------------------------------------------------------------
        Public Sub Add(ByVal item As ISpatialDataSet) _
            Implements System.Collections.Generic.ICollection(Of ISpatialDataSet).Add
            Me.m_lAvailable.Add(item)
            ' Assign ID if necessary
            If (item.GUID = Nothing) Then
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
            Me.m_lDeleted.Clear()
        End Sub

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="ICollection(Of ISpatialDataSet).Contains"/>
        ''' -------------------------------------------------------------------
        Public Function Contains(ByVal item As ISpatialDataSet) As Boolean _
            Implements System.Collections.Generic.ICollection(Of ISpatialDataSet).Contains
            Return Me.m_lAvailable.Contains(item)
        End Function

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

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get a dataset with a given <see cref="GUID"/>
        ''' </summary>
        ''' <param name="guidDS"></param>
        ''' <returns>A dataset, or nothing if no matching dataset could be found.</returns>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property ItemByGUID(ByVal guidDS As Guid) As ISpatialDataSet
            Get
                For Each ds As ISpatialDataSet In Me.m_lAvailable
                    If (guidDS.Equals(ds.GUID)) Then Return ds
                Next
                Return Nothing
            End Get
        End Property

#End Region ' Dataset list interface

#Region " Internals "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns the full path to the configuration file. This creates the directory if needed.
        ''' </summary>
        ''' <returns>The full path to the configuration file.</returns>
        ''' -------------------------------------------------------------------
        Public Shared Function ConfigFileName() As String

            Dim strFolder As String = cSystemUtils.ApplicationSettingsPath()
            Return Path.Combine(strFolder, cCONFIG_FILE)

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Event handler, invoked when the watched folder has changed.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub OnConfigFileChanged(ByVal sender As Object, ByVal args As FileSystemEventArgs)

            If Path.Equals(args.FullPath, cSpatialDataSetManager.ConfigFileName()) Then
                ' Lock up list
                m_bReadOnly = True
            End If

        End Sub

#End Region ' Internals

    End Class

End Namespace ' SpatialData
