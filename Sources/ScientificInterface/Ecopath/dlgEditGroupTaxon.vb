#Region " Imports "

Option Strict On
Imports System.Windows.Forms
Imports EwECore
Imports EwECore.ExternalData
Imports EwEUtils.Core
Imports EwEPlugin
Imports EwEPlugin.Data
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region ' Imports

''' ===========================================================================
''' <summary>
''' Dialog class implementing the Edit Group Taxon interface.
''' </summary>
''' <remarks>
''' <para>Note that this class breaks the EwE/Core/Plugin convention that all 
''' interaction with plug-ins should happen via the plug-in manager!</para>
''' <para>This class directly interfaces wit the plug-in manager to find taxonomy 
''' data producing search engines. These plug-ins are directly called to execute 
''' searches and to scoop up search results. This is all very neat, but this 
''' means that the core will not be able to use this behaviour at all.</para>
''' <para>This crucial behavour should probably be contained within a core class 
''' called 'cDataSearchManager(Of T)'. This will yield generic behaviour that can
''' be used for other purposes at core level.</para>
''' </remarks>
''' ===========================================================================
Public Class dlgEditGroupTaxon

#Region " Private vars "

    ''' <summary>UI context to connect to.</summary>
    Private m_uic As cUIContext = Nothing
    ''' <summary>Datasource delivering taxonomy data.</summary>
    Private m_tds As cTaxonDataSource = Nothing
    ''' <summary>Looped update prevention flag.</summary>
    Private m_bInUpdate As Boolean = False
    ''' <summary>Flag stating whether search engines were found.</summary>
    Private m_bHasSearchEngines As Boolean = False
    ''' <summary>Start up group.</summary>
    Private m_groupStartup As cEcoPathGroupInput = Nothing

#End Region ' Private vars

#Region " Private classes "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Helper class for listing data producers in a combo box.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Class cDataProducerSearchItem

        Private m_prod As IDataSearchProducerPlugin = Nothing

        Public Sub New(ByVal prod As IDataSearchProducerPlugin)
            Me.m_prod = prod
        End Sub

        Public ReadOnly Property Producer() As IDataSearchProducerPlugin
            Get
                Return Me.m_prod
            End Get
        End Property

        Public Overrides Function ToString() As String
            If Me.m_prod IsNot Nothing Then Return Me.m_prod.Name
            Return "No search engines installed"
        End Function

    End Class

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Helper class to wait for search results to be formatted and delivered.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Class cWaitForSearch

        ''' <summary>UI to notify when search complete.</summary>
        Private m_ui As dlgEditGroupTaxon = Nothing
        ''' <summary>Data producer that is searching.</summary>
        Private m_producer As IDataSearchProducerPlugin = Nothing
        ''' <summary>Search results.</summary>
        Private m_results As IDataSearchResults = Nothing

        Public Sub New(ByVal form As dlgEditGroupTaxon, ByVal prod As IDataSearchProducerPlugin, ByVal res As IDataSearchResults)
            Me.m_ui = form
            Me.m_producer = prod
            Me.m_results = res
        End Sub

        Public Sub Wait()
            While m_producer.IsSeaching
                ' NOP
            End While
            Me.m_ui.OnProcessSearchResults(Me.m_results)
        End Sub

    End Class

#End Region ' Private classes

#Region " Constructor "

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Create a new instance of this class.
    ''' </summary>
    ''' <param name="uic">The <see cref="cUIContext">UI context</see> to connect to.</param>
    ''' <param name="group">A group to select, if any.</param>
    ''' -------------------------------------------------------------------
    Public Sub New(ByVal uic As cUIContext, _
                   Optional ByVal group As cEcoPathGroupInput = Nothing)

        Me.InitializeComponent()
        Me.m_uic = uic
        Me.m_groupStartup = group
        Me.m_gridGroups.UIContext = uic
        Me.m_gridResults.UIContext = uic

    End Sub

#End Region ' Constructor

#Region " Form overrides "

    Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
        MyBase.OnLoad(e)

        Me.m_tds = cTaxonDataSource.GetInstance()

        Me.PopulateTaxonDetailControls()
        Me.PopulateTaxonDataProducerControls()
        Me.UpdateControls()

        ' Connect to group grid selection changes
        AddHandler Me.m_gridGroups.OnSelectionChanged, AddressOf OnRowSelectionChanged
        ' Connect to search result changes
        AddHandler Me.m_tds.OnTaxonSearchResults, AddressOf OnProcessResults
        ' Connect to result selection changes
        AddHandler Me.m_gridResults.OnResultSelected, AddressOf OnResultSelected

        If (Me.m_groupStartup Is Nothing) Then Me.m_groupStartup = Me.m_uic.Core.EcoPathGroupInputs(1)
        Me.m_gridGroups.SelectedGroup = Me.m_groupStartup

        If Me.m_bHasSearchEngines Then
            Me.m_tcMain.SelectTab(Me.m_tpSearch)
        Else
            Me.m_tcMain.SelectTab(Me.m_tpDetails)
        End If
    End Sub

    Protected Overrides Sub OnFormClosed(ByVal e As System.Windows.Forms.FormClosedEventArgs)

        RemoveHandler Me.m_gridGroups.OnSelectionChanged, AddressOf OnRowSelectionChanged
        RemoveHandler Me.m_tds.OnTaxonSearchResults, AddressOf OnProcessResults
        RemoveHandler Me.m_gridResults.OnResultSelected, AddressOf OnResultSelected

        MyBase.OnFormClosed(e)
    End Sub

#End Region ' Form overrides

#Region " Events "

    Private Sub OnRowSelectionChanged(ByVal selection As SourceGrid2.CellVirtualCollection)
        Me.UpdateControls()
    End Sub

    Private Sub OnResultSelected(ByVal result As Object)

        If (result Is Nothing) Then Return
        If Not (TypeOf result Is ITaxonSearchData) Then Return
        Me.m_gridGroups.AddTaxon(DirectCast(result, ITaxonSearchData))

    End Sub

    Private Sub OnOK(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles OK_Button.Click
        Try
            If Me.m_gridGroups.Apply Then
                Me.DialogResult = System.Windows.Forms.DialogResult.OK
                Me.Close()
            End If
        Catch ex As Exception
            cLog.Write(ex)
        End Try
    End Sub

    Private Sub OnCancel(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles Cancel_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub m_btnAdd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_btnAdd.Click
        Try
            Me.m_gridGroups.AddTaxon()
        Catch ex As Exception
            cLog.Write(ex)
        End Try
    End Sub

    Private Sub m_btnRemove_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_btnRemove.Click
        Try
            Me.m_gridGroups.ToggleDeleteRow()
        Catch ex As Exception
            cLog.Write(ex)
        End Try
    End Sub

    Private Sub m_btnKeep_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_btnKeep.Click
        Try
            Me.m_gridGroups.ToggleDeleteRow()
        Catch ex As Exception
            cLog.Write(ex)
        End Try
    End Sub

    Private Sub m_btnMoveUp_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_btnMoveUp.Click
        Try
            Me.m_gridGroups.MoveTaxon(-1)
        Catch ex As Exception
            cLog.Write(ex)
        End Try
    End Sub

    Private Sub m_btnMoveDown_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_btnMoveDown.Click
        Try
            Me.m_gridGroups.MoveTaxon(1)
        Catch ex As Exception
            cLog.Write(ex)
        End Try
    End Sub

    'Private Sub OnUpdateCurrent(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    '    Handles m_btnUpdate.Click
    '    ' Hmm
    'End Sub

    'Private Sub OnUpdateAll(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    '    Handles m_btnUpdateAll.Click
    '    ' Hmm
    'End Sub

    Private Sub OnSearchTextChanged(ByVal sender As System.Object, ByVal e As EventArgs) _
        Handles m_tbSearch.TextChanged
        Try
            Me.RefreshSearch()
        Catch ex As Exception
            cLog.Write(ex)
        End Try
    End Sub

    Private Sub m_cbIncludeExtent_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
        Handles m_cbIncludeExtent.CheckedChanged
        Try
            Me.RefreshSearch()
        Catch ex As Exception
            cLog.Write(ex)
        End Try
    End Sub

    Private Sub OnConfigure(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_btnConfigure.Click
        Try
            Me.ConfigureSelectedDataProducer()
        Catch ex As Exception
            cLog.Write(ex)
        End Try
    End Sub

    Private Sub OnSourceChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_cmbEngine.SelectedIndexChanged

        If (Me.m_bInUpdate) Then Return
        Me.UpdateControls()

    End Sub

    Private Sub OnCommonChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_tbCommon.TextChanged

        If (Me.m_bInUpdate) Then Return
        If (Me.m_gridGroups.SelectedTaxon Is Nothing) Then Return

        Me.m_gridGroups.SelectedTaxon.Common = Me.m_tbCommon.Text
        Me.m_gridGroups.UpdateSelectedTaxonRow()

    End Sub

    Private Sub OnSearchCommon(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_btnSearchCommon.Click
        Me.Search(Me.m_tbCommon.Text)
    End Sub

    Private Sub OnPhylumChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_cmbPhylum.TextChanged

        If (Me.m_bInUpdate) Then Return
        If (Me.m_gridGroups.SelectedTaxon Is Nothing) Then Return

        Me.m_gridGroups.SelectedTaxon.Phylum = Me.m_cmbPhylum.Text
        Me.m_gridGroups.UpdateSelectedTaxonRow()

    End Sub

    Private Sub OnSearchPhylum(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_btnSearchPhylum.Click
        Me.Search(Me.m_cmbPhylum.Text)
    End Sub

    Private Sub OnClassChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_cmbClass.TextChanged

        If (Me.m_bInUpdate) Then Return
        If (Me.m_gridGroups.SelectedTaxon Is Nothing) Then Return

        Me.m_gridGroups.SelectedTaxon.Class = Me.m_cmbClass.Text
        Me.m_gridGroups.UpdateSelectedTaxonRow()

    End Sub

    Private Sub OnSearchClass(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_btnSearchClass.Click
        Me.Search(Me.m_cmbClass.Text)
    End Sub

    Private Sub OnOrderChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_cmbOrder.TextChanged

        If (Me.m_bInUpdate) Then Return
        If (Me.m_gridGroups.SelectedTaxon Is Nothing) Then Return

        Me.m_gridGroups.SelectedTaxon.Order = Me.m_cmbOrder.Text
        Me.m_gridGroups.UpdateSelectedTaxonRow()

    End Sub

    Private Sub OnSearchOrder(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_btnSearchOrder.Click
        Me.Search(Me.m_cmbOrder.Text)
    End Sub

    Private Sub OnFamilyChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_cmbFamily.TextChanged

        If (Me.m_bInUpdate) Then Return
        If (Me.m_gridGroups.SelectedTaxon Is Nothing) Then Return

        Me.m_gridGroups.SelectedTaxon.Family = Me.m_cmbFamily.Text
        Me.m_gridGroups.UpdateSelectedTaxonRow()

    End Sub

    Private Sub OnSearchFamily(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_btnSearchFamily.Click
        Me.Search(Me.m_cmbFamily.Text)
    End Sub

    Private Sub OnGenusChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_cmbGenus.TextChanged

        If (Me.m_bInUpdate) Then Return
        If (Me.m_gridGroups.SelectedTaxon Is Nothing) Then Return

        Me.m_gridGroups.SelectedTaxon.Genus = Me.m_cmbGenus.Text
        Me.m_gridGroups.UpdateSelectedTaxonRow()

    End Sub

    Private Sub OnSearchGenus(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_btnSearchGenus.Click
        Me.Search(Me.m_cmbGenus.Text)
    End Sub

    Private Sub OnSpeciesChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_cmbSpecies.TextChanged

        If (Me.m_bInUpdate) Then Return
        If (Me.m_gridGroups.SelectedTaxon Is Nothing) Then Return

        Me.m_gridGroups.SelectedTaxon.Species = Me.m_cmbSpecies.Text
        Me.m_gridGroups.UpdateSelectedTaxonRow()

    End Sub

    Private Sub OnSearchSpecies(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_btnSearchSpecies.Click
        Me.Search(Me.m_cmbSpecies.Text)
    End Sub

    Private m_wait As cWaitForSearch = Nothing

    Private Sub OnProcessResults(ByVal results As IDataSearchResults)

        ' Ignore search terms of different data types
        If Not TypeOf results.SearchTerm Is ITaxonSearchData Then Return

        ' Process results async
        Me.m_wait = New cWaitForSearch(Me, Me.SelectedDataProducer, results)

        ' Handle this in a separate thread to allow the search to complete without stalling
        Dim thr As New Threading.Thread(AddressOf Me.m_wait.Wait)
        thr.Start()

    End Sub

    Protected Delegate Sub OnProcessSearchResultsDelegate(ByVal results As IDataSearchResults)

    Friend Sub OnProcessSearchResults(ByVal results As IDataSearchResults)

        If Me.InvokeRequired Then
            Me.Invoke(New OnProcessSearchResultsDelegate(AddressOf OnProcessSearchResults), New Object() {results})
            Return
        End If

        Me.m_gridResults.AddResults(results)

    End Sub

#End Region ' Events

#Region " Internals "

    Private Sub UpdateControls()

        Dim taxon As ITaxonSearchData = Me.m_gridGroups.SelectedTaxon
        Dim group As cEcoPathGroupInput = Me.m_gridGroups.SelectedGroup
        Dim bCanSearch As Boolean = False
        Dim bCanConfig As Boolean = False

        Me.m_bInUpdate = True

        If (Me.m_bHasSearchEngines) Then
            Dim prod As IDataProducerPlugin = Me.SelectedDataProducer
            If prod IsNot Nothing Then
                bCanSearch = (TypeOf prod Is IDataSearchProducerPlugin) And (prod.IsDataAvailable(GetType(ITaxonSearchData)))
                If TypeOf prod Is IConfigurablePlugin Then
                    bCanSearch = bCanSearch And DirectCast(prod, IConfigurablePlugin).IsConfigured
                End If
                bCanConfig = (TypeOf prod Is IConfigurablePlugin)
            End If
        End If

        ' Config search controls
        Me.m_cmbEngine.Enabled = Me.m_bHasSearchEngines
        Me.m_btnConfigure.Enabled = bCanConfig
        Me.m_tbSearch.Enabled = bCanSearch
        Me.m_cbIncludeExtent.Enabled = bCanSearch
        Me.m_gridResults.Enabled = bCanSearch

        ' Config taxon controls
        If (taxon Is Nothing) Then
            Me.m_lblSelectedGroup.Text = SharedResources.GENERIC_VALUE_NONE
            Me.m_tbCommon.Enabled = False : Me.m_tbCommon.Text = ""
            Me.m_btnSearchCommon.Enabled = False
            Me.m_cmbClass.Enabled = False : Me.m_cmbClass.SelectedIndex = -1
            Me.m_btnSearchClass.Enabled = False
            Me.m_cmbOrder.Enabled = False : Me.m_cmbOrder.SelectedIndex = -1
            Me.m_btnSearchOrder.Enabled = False
            Me.m_cmbFamily.Enabled = False : Me.m_cmbFamily.SelectedIndex = -1
            Me.m_btnSearchFamily.Enabled = False
            Me.m_cmbGenus.Enabled = False : Me.m_cmbGenus.SelectedIndex = -1
            Me.m_btnSearchGenus.Enabled = False
            Me.m_cmbSpecies.Enabled = False : Me.m_cmbSpecies.SelectedIndex = -1
            Me.m_btnSearchSpecies.Enabled = False
            Me.m_cmbPhylum.Enabled = False : Me.m_cmbPhylum.SelectedIndex = -1
            Me.m_btnSearchPhylum.Enabled = False
            Me.m_btnAdd.Enabled = (group IsNot Nothing)
            Me.m_btnRemove.Enabled = False
            Me.m_btnKeep.Enabled = False
            Me.m_btnMoveUp.Enabled = False
            Me.m_btnMoveDown.Enabled = False
        Else
            Me.m_lblSelectedGroup.Text = group.Name
            Me.m_tbCommon.Enabled = True : Me.m_tbCommon.Text = taxon.Common
            Me.m_btnSearchCommon.Enabled = bCanSearch And (Not String.IsNullOrEmpty(Me.m_tbCommon.Text.Trim))
            Me.m_cmbClass.Enabled = True : Me.m_cmbClass.Text = taxon.Class
            Me.m_btnSearchClass.Enabled = bCanSearch And (Not String.IsNullOrEmpty(Me.m_cmbClass.Text.Trim))
            Me.m_cmbOrder.Enabled = True : Me.m_cmbOrder.Text = taxon.Order
            Me.m_btnSearchOrder.Enabled = bCanSearch And (Not String.IsNullOrEmpty(Me.m_cmbOrder.Text.Trim))
            Me.m_cmbFamily.Enabled = True : Me.m_cmbFamily.Text = taxon.Family
            Me.m_btnSearchFamily.Enabled = bCanSearch And (Not String.IsNullOrEmpty(Me.m_cmbFamily.Text.Trim))
            Me.m_cmbGenus.Enabled = True : Me.m_cmbGenus.Text = taxon.Genus
            Me.m_btnSearchGenus.Enabled = bCanSearch And (Not String.IsNullOrEmpty(Me.m_cmbGenus.Text.Trim))
            Me.m_cmbSpecies.Enabled = True : Me.m_cmbSpecies.Text = taxon.Species
            Me.m_btnSearchSpecies.Enabled = bCanSearch And (Not String.IsNullOrEmpty(Me.m_cmbSpecies.Text.Trim))
            Me.m_cmbPhylum.Enabled = True : Me.m_cmbPhylum.Text = taxon.Phylum
            Me.m_btnSearchPhylum.Enabled = bCanSearch And (Not String.IsNullOrEmpty(Me.m_cmbPhylum.Text.Trim))
            Me.m_btnAdd.Enabled = True
            Me.m_btnRemove.Enabled = Not Me.m_gridGroups.IsFlaggedForDeletionRow
            Me.m_btnKeep.Enabled = Me.m_gridGroups.IsFlaggedForDeletionRow
            Me.m_btnMoveUp.Enabled = (group.Index > 1)
            Me.m_btnMoveDown.Enabled = (group.Index < Me.m_uic.Core.nGroups)
        End If

        Me.m_bInUpdate = False

    End Sub

    Private Sub PopulateTaxonDetailControls()

        Dim taxon As ITaxonSearchData = Nothing

        For Each taxon In Me.m_gridGroups.Taxa
            Me.AddText(Me.m_cmbClass, taxon.Class)
            Me.AddText(Me.m_cmbOrder, taxon.Order)
            Me.AddText(Me.m_cmbGenus, taxon.Genus)
            Me.AddText(Me.m_cmbFamily, taxon.Family)
            Me.AddText(Me.m_cmbSpecies, taxon.Species)
            Me.AddText(Me.m_cmbPhylum, taxon.Phylum)
        Next

    End Sub

    Private Sub PopulateTaxonDataProducerControls()

        Dim pm As cPluginManager = Me.m_uic.Core.PluginManager
        Dim pi As IPlugin = Nothing
        Dim dpi As IDataSearchProducerPlugin = Nothing
        Dim coll As ICollection(Of IPlugin) = Nothing

        Me.m_cmbEngine.Items.Clear()
        Me.m_bHasSearchEngines = False

        If (pm Is Nothing) Then Return

        coll = pm.GetPlugins(GetType(IDataSearchProducerPlugin))

        ' Only show data producers that provide taxon data
        For Each pi In coll
            dpi = DirectCast(pi, IDataSearchProducerPlugin)
            If (dpi.IsDataAvailable(GetType(ITaxonSearchData))) Then
                Me.m_cmbEngine.Items.Add(New cDataProducerSearchItem(dpi))
                Me.m_bHasSearchEngines = True
            End If
        Next

        If Not Me.m_bHasSearchEngines Then
            Me.m_cmbEngine.Items.Add(New cDataProducerSearchItem(Nothing))
        End If

        Me.m_cmbEngine.SelectedIndex = 0

    End Sub

    Private Sub AddText(ByVal cmb As ComboBox, ByVal strText As String)

        If cmb.FindStringExact(strText) = -1 Then
            cmb.Items.Add(strText)
        End If

    End Sub

    Private ReadOnly Property SelectedDataProducer() As IDataSearchProducerPlugin
        Get
            Dim item As cDataProducerSearchItem = DirectCast(Me.m_cmbEngine.SelectedItem, cDataProducerSearchItem)
            If item Is Nothing Then Return Nothing
            Return item.Producer
        End Get
    End Property

    Private Sub ConfigureSelectedDataProducer()

        Dim prod As IDataSearchProducerPlugin = Me.SelectedDataProducer
        Dim frm As Form = Nothing
        If (prod Is Nothing) Then Return
        If Not (TypeOf prod Is IConfigurablePlugin) Then Return

        frm = DirectCast(prod, IConfigurablePlugin).GetConfigUI()

        If (frm Is Nothing) Then Return

        Try

            frm.ShowInTaskbar = False
            frm.ShowIcon = False
            frm.ShowDialog(Me)

        Catch ex As Exception

        End Try

        Me.UpdateControls()

    End Sub

    Private Sub ApplyTaxon(ByVal taxon As ITaxonSearchData)
        Me.m_gridGroups.UpdateSelectedTaxon(taxon)
        Me.m_gridGroups.UpdateSelectedTaxonRow()
        Me.UpdateControls()
    End Sub

    Private Sub RefreshSearch()
        Me.Search(Me.m_tbSearch.Text)
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Search for a textual search term.
    ''' </summary>
    ''' <param name="strTerm">The text to search for.</param>
    ''' -----------------------------------------------------------------------
    Private Sub Search(ByVal strTerm As String)

        ' Clear grid
        Me.OnProcessSearchResults(Nothing)

        ' No term? Abort
        If String.IsNullOrEmpty(strTerm) Then Return
        ' Term less than 3 chars? Abort
        If (strTerm.Length < 3) Then Return

        ' Make search term
        Dim objTerm As Object = Me.SelectedDataProducer.CreateSearchTerm()
        ' No valid term? Abort
        If Not (TypeOf objTerm Is ITaxonSearchData) Then Return

        ' Create search term
        Dim searchterm As ITaxonSearchData = DirectCast(objTerm, ITaxonSearchData)
        ' Successful?
        If searchterm IsNot Nothing Then
            '#Yes: populate term
            searchterm.Common = strTerm
            ' Switch to search tab
            Me.m_tcMain.SelectTab(Me.m_tpSearch)
            Me.m_tbSearch.Text = strTerm
            ' Go Jimmy
            Me.Search(searchterm)
        End If

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Search for a <see cref="ITaxonData">taxonomoy data</see> search term.
    ''' </summary>
    ''' <param name="term">The <see cref="ITaxonData">taxonomoy data</see> 
    ''' search term to search for.</param>
    ''' -----------------------------------------------------------------------
    Private Sub Search(ByVal term As ITaxonSearchData)

        If (Me.SelectedDataProducer Is Nothing) Then Return

        Try
            Dim taxonSearch As ITaxonSearchData = Me.m_gridGroups.GetSearchTerm(term)

            ' Clear search key to initiate a full search
            taxonSearch.SourceKey = ""

            ' Set bounding box if necessary
            If Me.m_cbIncludeExtent.Checked Then
                Dim model As cEwEModel = Me.m_uic.Core.EwEModel
                taxonSearch.North = model.North
                taxonSearch.South = model.South
                taxonSearch.East = model.East
                taxonSearch.West = model.West
            Else
                taxonSearch.North = cCore.NULL_VALUE
                taxonSearch.South = cCore.NULL_VALUE
                taxonSearch.East = cCore.NULL_VALUE
                taxonSearch.West = cCore.NULL_VALUE
            End If

            ' Start searching
            Me.SelectedDataProducer.StartSearch(taxonSearch)
        Catch ex As Exception

        End Try

    End Sub

    Private Sub UpdateRecord(ByVal term As ITaxonSearchData)

        If (Me.SelectedDataProducer Is Nothing) Then Return

        Try
            ' Has a search key for this specific producer?
            If (Not String.IsNullOrEmpty(term.SourceKey)) And _
               (String.Compare(term.Source, Me.SelectedDataProducer.Name, True) = 0) Then
                ' #Yes: Start searching (expected to return only one result)
                Me.SelectedDataProducer.StartSearch(Me.m_gridGroups.GetSearchTerm(term))
            End If
        Catch ex As Exception
            ' Woops
        End Try

    End Sub

#End Region ' Internals

End Class
