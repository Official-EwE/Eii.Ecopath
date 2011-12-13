#Region " Imports "

Option Strict On
Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Reflection
Imports System.Xml
Imports EwEUtils.SystemUtilities

#End Region ' Imports

Namespace SpatialData

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Manager class for loading and saving globally shared spatial data sets.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cSpatialDataSetManager
        Implements IList(Of ISpatialDataSet)
        Implements IDisposable

#Region " Private vars "

        Private Shared cCONFIG_FILE As String = "ewe_datasets.xml"

        Private m_lDatasets As List(Of ISpatialDataSet) = Nothing
        Private m_fswSpy As FileSystemWatcher = Nothing
        Private m_bReadOnly As Boolean = False

#End Region ' Private vars

#Region " Construction "

        Public Sub New()

            ' Create list of datasets
            Me.m_lDatasets = New List(Of ISpatialDataSet)

            ' Create folder watcher
            Me.m_fswSpy = New FileSystemWatcher()
            Me.m_fswSpy.Path = Path.GetDirectoryName(cSpatialDataSetManager.ConfigFileName())
            Me.m_fswSpy.NotifyFilter = NotifyFilters.LastWrite
            Me.m_fswSpy.Filter = "*.xml"
            Me.m_fswSpy.EnableRaisingEvents = True

            AddHandler Me.m_fswSpy.Changed, AddressOf OnConfigFileChanged

        End Sub

        Public Sub Dispose() _
            Implements IDisposable.Dispose

            ' Cleanup
            If (Me.m_lDatasets IsNot Nothing) Then

                RemoveHandler Me.m_fswSpy.Changed, AddressOf OnConfigFileChanged
                Me.m_fswSpy = Nothing

                Me.m_lDatasets = Nothing

            End If
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
        ''' <param name="bClearFirst">Flag, stating that the content currently in the manager should be cleared first.</param>
        ''' <returns>True if successful.</returns>
        ''' -------------------------------------------------------------------
        Public Function Load(Optional strFile As String = "", _
                             Optional bClearFirst As Boolean = False) As Boolean


            Dim doc As New XmlDocument()
            Dim xnRoot As XmlNode = Nothing
            Dim xa As XmlAttribute = Nothing
            Dim ds As ISpatialDataSet = Nothing
            Dim an As AssemblyName = Nothing

            If (bClearFirst) Then Me.Clear()
            If (String.IsNullOrEmpty(strFile)) Then strFile = cSpatialDataSetManager.ConfigFileName()

            If Not File.Exists(strFile) Then Return True

            ' Load datasets
            doc.Load(strFile)

            For Each xnRoot In doc.GetElementsByTagName("Datasets")
                For Each xn As XmlNode In xnRoot.ChildNodes
                    If (xn.Name = "Dataset") Then
                        xa = xn.Attributes("Type")
                        If (xa IsNot Nothing) Then
                            Try
                                Dim strTypeName As String = xa.InnerText
                                Dim t As Type = Me.StringToType(strTypeName)
                                ds = DirectCast(Activator.CreateInstance(t), ISpatialDataSet)

                                ds.Configuration(doc) = xn.ChildNodes(0)
                                xa = xn.Attributes("GUID")
                                ' Assign GUID
                                ds.GUID = Guid.Parse(xa.InnerText)
                            Catch ex As Exception
                                ds = Nothing
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
            Return True

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Saves all datasets currently loaded by the manager to persistent storage.
        ''' </summary>
        ''' <returns>True if successful.</returns>
        ''' <remarks>If the manager is read-only, which is set when the datafile
        ''' is externally modified, any save attempt will abort and fail.</remarks>
        ''' -------------------------------------------------------------------
        Public Function Save() As Boolean

            Dim doc As New XmlDocument()
            Dim lNodes As New List(Of XmlNode)
            Dim xnRoot As XmlNode = Nothing
            Dim xnDataset As XmlNode = Nothing
            Dim xnDetails As XmlNode = Nothing
            Dim xaDataset As XmlAttribute = Nothing
            Dim bSuccess As Boolean = True

            ' Declaration
            doc.AppendChild(doc.CreateXmlDeclaration("1.0", "", "yes"))

            xnRoot = doc.CreateElement("Datasets")
            doc.AppendChild(xnRoot)

            ' Gather dataset config nodes, but do not add to the doc until all done
            For Each ds As ISpatialDataSet In Me.m_lDatasets

                xnDataset = doc.CreateElement("Dataset")

                xaDataset = doc.CreateAttribute("Type")
                xaDataset.Value = Me.TypeToString(ds.GetType)
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

                lNodes.Add(xnDataset)

            Next

            ' Add dataset nodes
            For Each xnDataset In lNodes
                xnRoot.AppendChild(xnDataset)
            Next

            ' Save
            Me.m_fswSpy.EnableRaisingEvents = False
            Try
                doc.Save(ConfigFileName)
            Catch ex As Exception
                bSuccess = False
            End Try
            Me.m_fswSpy.EnableRaisingEvents = True

            Return bSuccess

        End Function

#End Region ' Persistent storage

#Region " Dataset list interface "

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="ICollection(Of ISpatialDataSet).Add"/>
        ''' -------------------------------------------------------------------
        Public Sub Add(ByVal item As ISpatialDataSet) _
            Implements System.Collections.Generic.ICollection(Of ISpatialDataSet).Add
            Me.m_lDatasets.Add(item)
            ' Assign ID if necessary
            If (item.GUID = Nothing) Then
                item.GUID = Guid.NewGuid()
            End If
        End Sub

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="ICollection(Of ISpatialDataSet).Clear"/>
        ''' -------------------------------------------------------------------
        Public Sub Clear() _
            Implements System.Collections.Generic.ICollection(Of ISpatialDataSet).Clear
            Me.m_lDatasets.Clear()
        End Sub

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="ICollection(Of ISpatialDataSet).Contains"/>
        ''' -------------------------------------------------------------------
        Public Function Contains(ByVal item As ISpatialDataSet) As Boolean _
            Implements System.Collections.Generic.ICollection(Of ISpatialDataSet).Contains
            Return Me.m_lDatasets.Contains(item)
        End Function

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="ICollection(Of ISpatialDataSet).CopyTo"/>
        ''' -------------------------------------------------------------------
        Public Sub CopyTo(ByVal array() As ISpatialDataSet, ByVal arrayIndex As Integer) _
            Implements System.Collections.Generic.ICollection(Of ISpatialDataSet).CopyTo
            Me.m_lDatasets.CopyTo(array, arrayIndex)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="ICollection(Of ISpatialDataSet).Count"/>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property Count As Integer _
            Implements System.Collections.Generic.ICollection(Of ISpatialDataSet).Count
            Get
                Return Me.m_lDatasets.Count
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
            Return Me.m_lDatasets.Remove(item)
        End Function

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="ICollection(Of ISpatialDataSet).GetEnumerator"/>
        ''' -------------------------------------------------------------------
        Public Sub RemoveAt(ByVal index As Integer) _
            Implements IList(Of ISpatialDataSet).RemoveAt
            Me.m_lDatasets.RemoveAt(index)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="ICollection(Of ISpatialDataSet).GetEnumerator"/>
        ''' -------------------------------------------------------------------
        Public Function GetEnumerator() As System.Collections.Generic.IEnumerator(Of ISpatialDataSet) _
            Implements System.Collections.Generic.IEnumerable(Of ISpatialDataSet).GetEnumerator
            Return Me.m_lDatasets.GetEnumerator
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
            Return Me.m_lDatasets.IndexOf(item)
        End Function

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="ICollection(Of ISpatialDataSet).GetEnumerator"/>
        ''' -------------------------------------------------------------------
        Private Sub InaccessibleInsert(ByVal index As Integer, ByVal item As ISpatialDataSet) _
            Implements IList(Of ISpatialDataSet).Insert
            Me.m_lDatasets.Insert(index, item)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="ICollection(Of ISpatialDataSet).GetEnumerator"/>
        ''' -------------------------------------------------------------------
        Default Public Property Item(ByVal index As Integer) As ISpatialDataSet _
            Implements IList(Of ISpatialDataSet).Item
            Get
                Return Me.m_lDatasets.Item(index)
            End Get
            Protected Set(ByVal value As ISpatialDataSet)
                Me.m_lDatasets.Item(index) = value
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
                For Each ds As ISpatialDataSet In Me.m_lDatasets
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

        Public Function TypeToString(ByVal t As Type) As String

            ' Include assembly short name in the type name. This enables
            ' the OOP database logic to relocate the type from its original
            ' assembly, even if similar class names exist in similar namespaces
            ' in different asssemblies. Yes, it's far fetched, but hey...
            Return t.Assembly.GetName.Name + "!" + t.FullName()

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Helper method, locates the originating type from a type string.
        ''' </summary>
        ''' <param name="strType">The type name to locate the originating type
        ''' for.</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' The counterpart of this method, <see cref="TypeToString"/>,
        ''' can be used to create the string for a type.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Function StringToType(ByVal strType As String) As Type

            ' Split assembly short name from type name
            Dim astr As String() = strType.Split(CChar("!"))
            Dim ass As Assembly = Nothing

            For Each ass In AppDomain.CurrentDomain.GetAssemblies
                If String.Compare(ass.GetName.Name, astr(0), True) = 0 Then
                    Try
                        Return ass.GetType(astr(1))
                    Catch ex As Exception

                    End Try
                End If
            Next
            Return Nothing

        End Function
#End Region ' Internals

    End Class

End Namespace ' SpatialData
