#Region " Imports "

Option Strict On
Imports System.Windows.Forms
Imports EwECore
Imports EwEUtils.Core
Imports EwEPlugin
Imports EwEPlugin.Data
Imports EwECore.ExternalData

#End Region ' Imports

''' ===========================================================================
''' <summary>
''' Dialog class implementing the Edit Group Taxon interface.
''' </summary>
''' ===========================================================================
Public Class dlgEditGroupTaxon2

#Region " Private vars "

    ''' <summary>UI context to connect to.</summary>
    Private m_uic As cUIContext = Nothing
    ''' <summary>Datasource delivering taxonomy data.</summary>
    Private m_tds As cTaxonDataSource = Nothing
    ''' <summary>Looped update prevention flag.</summary>
    Private m_bInUpdate As Boolean = False

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
            Return ""
        End Function

    End Class

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Helper class to wait for search results to be formatted and delivered.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Class cWaitForSearch

        ''' <summary>UI to notify when search complete.</summary>
        Private m_ui As dlgEditGroupTaxon2 = Nothing
        ''' <summary>Data producer that is searching.</summary>
        Private m_producer As IDataSearchProducerPlugin = Nothing
        ''' <summary>Search results.</summary>
        Private m_results As IDataSearchResults = Nothing

        Public Sub New(ByVal form As dlgEditGroupTaxon2, ByVal prod As IDataSearchProducerPlugin, ByVal res As IDataSearchResults)
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
        Me.m_gridGroups.UIContext = uic

    End Sub

#End Region ' Constructor

#Region " Form overrides "

    Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
        MyBase.OnLoad(e)

        Me.m_tds = cTaxonDataSource.GetInstance()

        Me.PopulateTaxonDetailControls()
        Me.PopulateTaxonDataProducerControls()
        Me.UpdateControls()

        AddHandler Me.m_gridGroups.OnSelectionChanged, AddressOf OnRowSelectionChanged
        AddHandler Me.m_tds.OnTaxonSearchResults, AddressOf OnProcessResults

    End Sub

    Protected Overrides Sub OnFormClosed(ByVal e As System.Windows.Forms.FormClosedEventArgs)

        RemoveHandler Me.m_gridGroups.OnSelectionChanged, AddressOf OnRowSelectionChanged
        RemoveHandler Me.m_tds.OnTaxonSearchResults, AddressOf OnProcessResults

        MyBase.OnFormClosed(e)
    End Sub

#End Region ' Form overrides

#Region " Events "

    Private Sub OnRowSelectionChanged(ByVal selection As SourceGrid2.CellVirtualCollection)
        Me.UpdateControls()
    End Sub

    Private Sub OnOK(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles OK_Button.Click
        If Me.m_gridGroups.Apply Then
            Me.DialogResult = System.Windows.Forms.DialogResult.OK
            Me.Close()
        End If
    End Sub

    Private Sub OnCancel(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles Cancel_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub m_btnAdd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_btnAdd.Click
        Me.m_gridGroups.AddTaxon()
    End Sub

    Private Sub m_btnRemove_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_btnRemove.Click
        Me.m_gridGroups.ToggleDeleteRow()
    End Sub

    Private Sub m_btnKeep_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_btnKeep.Click
        Me.m_gridGroups.ToggleDeleteRow()
    End Sub

    Private Sub m_btnMoveUp_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_btnMoveUp.Click
        Me.m_gridGroups.MoveTaxon(-1)
    End Sub

    Private Sub m_btnMoveDown_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_btnMoveDown.Click
        Me.m_gridGroups.MoveTaxon(1)
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

        Dim strTerm As String = m_tbSearch.Text
        If strTerm.Length > 4 Then
            Dim searchterm As ITaxonData = Me.SelectedDataProducer.CreateSearchTerm()
            If searchterm IsNot Nothing Then
                searchterm.Common = strTerm
                Me.SearchTaxon(searchterm)
            End If
        End If
    End Sub

    Private Sub OnConfigure(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_btnConfigure.Click
        Me.ConfigureSelectedDataProducer()
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

    Private Sub OnClassChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_cmbClass.TextChanged

        If (Me.m_bInUpdate) Then Return
        If (Me.m_gridGroups.SelectedTaxon Is Nothing) Then Return

        Me.m_gridGroups.SelectedTaxon.Class = Me.m_cmbClass.Text
        Me.m_gridGroups.UpdateSelectedTaxonRow()

    End Sub

    Private Sub OnOrderChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_cmbOrder.TextChanged

        If (Me.m_bInUpdate) Then Return
        If (Me.m_gridGroups.SelectedTaxon Is Nothing) Then Return

        Me.m_gridGroups.SelectedTaxon.Order = Me.m_cmbOrder.Text
        Me.m_gridGroups.UpdateSelectedTaxonRow()

    End Sub

    Private Sub OnFamilyChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_cmbFamily.TextChanged

        If (Me.m_bInUpdate) Then Return
        If (Me.m_gridGroups.SelectedTaxon Is Nothing) Then Return

        Me.m_gridGroups.SelectedTaxon.Family = Me.m_cmbFamily.Text
        Me.m_gridGroups.UpdateSelectedTaxonRow()

    End Sub

    Private Sub OnGenusChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_cmbGenus.TextChanged

        If (Me.m_bInUpdate) Then Return
        If (Me.m_gridGroups.SelectedTaxon Is Nothing) Then Return

        Me.m_gridGroups.SelectedTaxon.Genus = Me.m_cmbGenus.Text
        Me.m_gridGroups.UpdateSelectedTaxonRow()

    End Sub

    Private Sub OnSpeciesChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_cmbSpecies.TextChanged

        If (Me.m_bInUpdate) Then Return
        If (Me.m_gridGroups.SelectedTaxon Is Nothing) Then Return

        Me.m_gridGroups.SelectedTaxon.Species = Me.m_cmbSpecies.Text
        Me.m_gridGroups.UpdateSelectedTaxonRow()

    End Sub

    Private m_wait As cWaitForSearch = Nothing

    Private Sub OnProcessResults(ByVal results As IDataSearchResults)

        ' Ignore search terms of different data types
        If Not TypeOf results.SearchTerm Is ITaxonData Then Return

        '' Is a search result that we fired ourselves?
        'If Not Me.m_gridGroups.IsSearchTerm(DirectCast(results.SearchTerm, ITaxonData)) Then Return

        ' ToDo: process results async
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

        'Select Case results.SearchResults.Count
        '    Case 0
        '        Dim msg As New cMessage(My.Resources.PROMPT_SEARCH_NORESULTS, _
        '                                eMessageType.Any, eCoreComponentType.External, eMessageImportance.Information)
        '        Me.m_uic.Core.Messages.SendMessage(msg)

        '    Case 1
        '        ' Apply the first result
        '        Me.ApplyTaxon(DirectCast(results.SearchResults(0), ITaxonData))

        '    Case Else
        '        ' Show selected results
        '        Dim dlg As New frmSearchResults(Me.m_uic, results)
        '        If dlg.ShowDialog(Me) = Windows.Forms.DialogResult.OK Then
        '            Select Case dlg.Choice
        '                Case frmSearchResults.eChoiceTypes.UseSelected
        '                    Me.ApplyTaxon(DirectCast(dlg.SelectedResult, ITaxonData))
        '                Case frmSearchResults.eChoiceTypes.SearchWithSelected
        '                    Me.SearchTaxon(DirectCast(dlg.SelectedResult, ITaxonData))
        '            End Select
        '        End If
        'End Select

    End Sub

#End Region ' Events

#Region " Internals "

    Private Sub UpdateControls()

        Dim taxon As ITaxonData = Me.m_gridGroups.SelectedTaxon
        Dim group As cEcoPathGroupInput = Me.m_gridGroups.SelectedGroup
        Dim bHasEngines As Boolean = (Me.m_cmbEngine.Items.Count > 0)
        Dim bCanSearch As Boolean = False
        Dim bCanConfig As Boolean = False

        Me.m_bInUpdate = True

        Dim prod As IDataProducerPlugin = Me.SelectedDataProducer
        If prod IsNot Nothing Then
            bCanSearch = (TypeOf prod Is IDataSearchProducerPlugin) And (prod.IsDataAvailable(GetType(ITaxonData)))
            If TypeOf prod Is IConfigurablePlugin Then
                bCanSearch = bCanSearch And DirectCast(prod, IConfigurablePlugin).IsConfigured
            End If
            bCanConfig = (TypeOf prod Is IConfigurablePlugin)
        End If

        Me.m_cmbEngine.Enabled = bHasEngines
        Me.m_btnConfigure.Enabled = bCanConfig
        Me.m_tbSearch.Enabled = bCanSearch

        If (taxon Is Nothing) Then
            Me.m_tbCommon.Enabled = False : Me.m_tbSearch.Text = ""
            Me.m_cmbClass.Enabled = False : Me.m_cmbClass.SelectedIndex = -1
            Me.m_cmbOrder.Enabled = False : Me.m_cmbClass.SelectedIndex = -1
            Me.m_cmbFamily.Enabled = False : Me.m_cmbClass.SelectedIndex = -1
            Me.m_cmbGenus.Enabled = False : Me.m_cmbClass.SelectedIndex = -1
            Me.m_cmbSpecies.Enabled = False : Me.m_cmbClass.SelectedIndex = -1
            Me.m_btnAdd.Enabled = (group IsNot Nothing)
            Me.m_btnRemove.Enabled = False
            Me.m_btnKeep.Enabled = False
            Me.m_btnMoveUp.Enabled = False
            Me.m_btnMoveDown.Enabled = False
            Me.m_btnUpdate.Enabled = False
        Else
            Me.m_tbCommon.Enabled = True : Me.m_tbSearch.Text = taxon.Common
            Me.m_cmbClass.Enabled = True : Me.m_cmbClass.Text = taxon.Class
            Me.m_cmbOrder.Enabled = True : Me.m_cmbOrder.Text = taxon.Order
            Me.m_cmbFamily.Enabled = True : Me.m_cmbFamily.Text = taxon.Family
            Me.m_cmbGenus.Enabled = True : Me.m_cmbGenus.Text = taxon.Genus
            Me.m_cmbSpecies.Enabled = True : Me.m_cmbSpecies.Text = taxon.Species
            Me.m_btnAdd.Enabled = True
            Me.m_btnRemove.Enabled = Not Me.m_gridGroups.IsFlaggedForDeletionRow
            Me.m_btnKeep.Enabled = Me.m_gridGroups.IsFlaggedForDeletionRow
            Me.m_btnMoveUp.Enabled = (group.Index > 1)
            Me.m_btnMoveDown.Enabled = (group.Index < Me.m_uic.Core.nGroups)
            Me.m_btnUpdate.Enabled = Not String.IsNullOrEmpty(taxon.Source)
        End If

        Me.m_bInUpdate = False

    End Sub

    Private Sub PopulateTaxonDetailControls()

        Dim taxon As ITaxonData = Nothing

        For Each taxon In Me.m_gridGroups.Taxa
            Me.AddText(Me.m_cmbClass, taxon.Class)
            Me.AddText(Me.m_cmbOrder, taxon.Order)
            Me.AddText(Me.m_cmbGenus, taxon.Genus)
            Me.AddText(Me.m_cmbFamily, taxon.Family)
            Me.AddText(Me.m_cmbSpecies, taxon.Species)
        Next

    End Sub

    Private Sub PopulateTaxonDataProducerControls()

        Dim pm As cPluginManager = Me.m_uic.Core.PluginManager
        Dim pi As IPlugin = Nothing
        Dim dpi As IDataSearchProducerPlugin = Nothing
        Dim coll As ICollection(Of IPlugin) = Nothing
        Dim bHasItems As Boolean = False

        Me.m_cmbEngine.Items.Clear()

        If (pm Is Nothing) Then Return

        coll = pm.GetPlugins(GetType(IDataSearchProducerPlugin))

        ' Only show data producers that provide taxon data
        For Each pi In coll
            dpi = DirectCast(pi, IDataSearchProducerPlugin)
            If (dpi.IsDataAvailable(GetType(ITaxonData))) Then
                Me.m_cmbEngine.Items.Add(New cDataProducerSearchItem(dpi))
                bHasItems = True
            End If
        Next

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

    Private Sub ApplyTaxon(ByVal taxon As ITaxonData)
        Me.m_gridGroups.UpdateSelectedTaxon(taxon)
        Me.m_gridGroups.UpdateSelectedTaxonRow()
        Me.UpdateControls()
    End Sub

    Private Sub SearchTaxon(ByVal taxon As ITaxonData)

        If (Me.SelectedDataProducer Is Nothing) Then Return

        Try
            Dim taxonSearch As ITaxonData = Me.m_gridGroups.GetSearchTerm(taxon)

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

    Private Sub RefreshTaxon(ByVal taxon As ITaxonData)

        If (Me.SelectedDataProducer Is Nothing) Then Return

        Try
            ' Has a search key for this specific producer?
            If (Not String.IsNullOrEmpty(taxon.SourceKey)) And _
               (String.Compare(taxon.Source, Me.SelectedDataProducer.Name, True) = 0) Then
                ' #Yes: Start searching (expected to return only one result)
                Me.SelectedDataProducer.StartSearch(Me.m_gridGroups.GetSearchTerm(taxon))
            End If
        Catch ex As Exception
            ' Woops
        End Try

    End Sub

#End Region ' Internals

End Class
