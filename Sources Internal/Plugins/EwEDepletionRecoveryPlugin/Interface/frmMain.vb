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
'    UBC Institute for the Oceans and Fisheries, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports System.IO
Imports System.Text
Imports System.Windows.Forms
Imports EwECore
Imports EwECore.DataSources
Imports EwECore.Ecosim
Imports EwEUtils.Core
Imports EwEUtils.Utilities
Imports ScientificInterfaceShared.Commands
Imports ScientificInterfaceShared.Controls

#End Region ' Imports

' ToDo: localize this class
' ToDo: use typeformatters for categories

''' ===========================================================================
''' <summary>
''' Main form (and basically everything else except for plug-in point and data)
''' of the depletion/recovery plug-in.
''' </summary>
''' ===========================================================================
Public Class frmMain

#Region " Private vars "

    ''' <summary>Flag stating whether the plug-in is running</summary>
    Private m_bRunning As Boolean = False
    ''' <summary>Selected session</summary>
    Private m_session As cSession = Nothing
    ''' <summary>Selected model settings</summary>
    Private m_ms As cModelSettings = Nothing
    ''' <summary>Selected group category</summary>
    Private m_groupcat As eGroupCategoryTypes = eGroupCategoryTypes.All
    ''' <summary>Selected fleet category</summary>
    Private m_fleetcat As eFleetCategoryTypes = eFleetCategoryTypes.All

    Private m_prog As frmProgress = Nothing
    Private m_naManager As EwENetworkAnalysis.cNetworkManager = Nothing

#End Region ' Private vars

#Region " GUI helper classes "

    Private Class cCoreInputOutputListboxItem

        Private m_source As cCoreInputOutputBase = Nothing

        Public Sub New(ByVal source As cCoreInputOutputBase)
            Debug.Assert(source IsNot Nothing)
            Me.m_source = source
        End Sub

        Public Overrides Function ToString() As String
            Return Me.m_source.Name
        End Function

        Public ReadOnly Property Source() As cCoreInputOutputBase
            Get
                Return Me.m_source
            End Get
        End Property

    End Class

#End Region ' GUI helper classes

#Region " Construction "

    Public Sub New(ByVal uic As cUIContext)
        MyBase.New()
        Me.InitializeComponent()
        Me.m_session = New cSession()
        Me.UIContext = uic
    End Sub

#End Region ' Construction

#Region " Overrides "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Load and initialize the form
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
        MyBase.OnLoad(e)

        Dim nap As EwENetworkAnalysis.cEwENetworkAnalysisPlugin = EwENetworkAnalysis.cEwENetworkAnalysisPlugin.thePlugin
        If (nap IsNot Nothing) Then
            Me.m_naManager = nap.Manager
        End If

        Dim lvi As ListViewItem = Nothing

        Me.m_bInUpdate = True

        ' Prepare group categories combo
        For Each cat As eGroupCategoryTypes In [Enum].GetValues(GetType(eGroupCategoryTypes))
            lvi = New ListViewItem(New String() {cat.ToString, "0"})
            Me.m_lvCategoriesGroup.Items.Add(lvi)
            lvi.Selected = (cat = eGroupCategoryTypes.All)
        Next

        ' Prepare fleet categories combo
        For Each cat As eFleetCategoryTypes In [Enum].GetValues(GetType(eFleetCategoryTypes))
            lvi = New ListViewItem(New String() {cat.ToString, "0"})
            Me.m_lvCategoriesFleet.Items.Add(lvi)
            lvi.Selected = (cat = eFleetCategoryTypes.All)
        Next

        ' Prepare Ecosim results checked listbox
        For Each result As cEcosimResultWriter.eResultTypes In [Enum].GetValues(GetType(cEcosimResultWriter.eResultTypes))
            Me.m_clbEcosimResults.Items.Add(result)
        Next

        ' Phah
        Me.m_btnAllResults_Click(Nothing, Nothing)

        Me.m_bInUpdate = False

        Me.UpdateSessionControls()

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Close the form.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Protected Overrides Sub OnFormClosed(ByVal e As FormClosedEventArgs)
        Me.SelectedModel = Nothing
        MyBase.OnFormClosed(e)
    End Sub

    Protected Overrides Sub OnHelpRequested(hevent As System.Windows.Forms.HelpEventArgs)
        MyBase.OnHelpRequested(hevent)
    End Sub

#End Region ' Overrides

#Region " Events "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Reset the session
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub m_tsbReset_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_tsbReset.Click
        Me.ResetSession()
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Load an existing session from file.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub m_tssbLoad_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_tssbLoad.Click

        Dim ofd As New OpenFileDialog()
        Dim ms As cModelSettings = Nothing

        With ofd
            .Title = "Select session to load"
            .Filter = "Depletion/recovery XML file (*.drfxml)|*.drfxml"
            .FileName = Me.m_session.FileName
        End With

        If (ofd.ShowDialog() <> Windows.Forms.DialogResult.OK) Then Return

        Me.LoadSession(ofd.FileName)

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Save the current session to file.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub m_tsbSave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_tsbSave.Click

        Dim sfd As New SaveFileDialog()
        Dim ms As cModelSettings = Nothing

        With sfd
            .Title = "Select location to save this session to"
            .Filter = "Depletion/recovery XML file (*.drfxml)|*.drfxml"
            .FileName = Me.m_session.FileName
            .AddExtension = True
        End With

        If (sfd.ShowDialog() = Windows.Forms.DialogResult.OK) Then
            Me.SaveSession(sfd.FileName)
        End If

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Add a model to the current sesison.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub m_btnAddModel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_btnAddModel.Click

        If (Me.m_session Is Nothing) Then Return
        If (Me.m_bInUpdate = True) Then Return

        Dim ofd As New OpenFileDialog()

        With ofd
            .Title = "Select model to add"
            .Filter = "EwE6 model files (*.ewemdb,*.eweaccdb)|*.ewemdb;*.eweaccdb|All files (*.*)|*.*"
        End With

        If (ofd.ShowDialog() = Windows.Forms.DialogResult.OK) Then
            Me.AddModel(ofd.FileName)
        End If

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Remove a model from the current session.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub OnRemoveModel(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_bntRemoveModel.Click

        If (Me.m_session Is Nothing) Then Return
        If (Me.m_bInUpdate = True) Then Return

        Me.RemoveModel(DirectCast(Me.m_clbModels.SelectedItem, cModelSettings))

    End Sub

    Private Sub OnDragFiles(sender As Object, e As System.Windows.Forms.DragEventArgs) _
        Handles m_clbModels.DragEnter

        If (Me.m_session Is Nothing) Then Return
        If (Me.m_bInUpdate = True) Then Return

        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            e.Effect = DragDropEffects.Copy
        End If

    End Sub

    Private Sub OnDropFiles(sender As Object, e As System.Windows.Forms.DragEventArgs) _
        Handles m_clbModels.DragDrop

        If (Me.m_session Is Nothing) Then Return
        If (Me.m_bInUpdate = True) Then Return

        Dim files() As String = CType(e.Data.GetData(DataFormats.FileDrop), String())
        For Each file As String In files
            Me.AddModel(file)
        Next

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Process model selection change.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub OnModelSelected(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_clbModels.SelectedIndexChanged

        If (Me.m_session Is Nothing) Then Return
        If (Me.m_bInUpdate = True) Then Return

        Me.SelectedModel = DirectCast(Me.m_clbModels.SelectedItem, cModelSettings)

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Process model check change.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub OnModelChecked(ByVal sender As System.Object, ByVal e As ItemCheckEventArgs) _
        Handles m_clbModels.ItemCheck

        If (Me.m_session Is Nothing) Then Return
        If (Me.m_bInUpdate = True) Then Return

        Dim ms As cModelSettings = DirectCast(Me.m_clbModels.Items(e.Index), cModelSettings)
        ms.Enabled = (e.NewValue <> CheckState.Unchecked)

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Select all groups into the current group category.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub OnSelectAllModels(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_btnAllModels.Click

        If (Me.m_session Is Nothing) Then Return
        If (Me.m_bInUpdate = True) Then Return

        For i As Integer = 0 To Me.m_clbModels.Items.Count - 1
            Me.m_clbModels.SetItemChecked(i, True)
        Next

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Select all groups into the current group category.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub OnSelectNoModels(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_btnNoModels.Click

        If (Me.m_session Is Nothing) Then Return
        If (Me.m_bInUpdate = True) Then Return

        For i As Integer = 0 To Me.m_clbModels.Items.Count - 1
            Me.m_clbModels.SetItemChecked(i, False)
        Next

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Process group category selection change.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub m_lvCategoriesGroup_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_lvCategoriesGroup.SelectedIndexChanged

        If (Me.m_session Is Nothing) Then Return
        If (Me.m_bInUpdate = True) Then Return

        Dim sic As ListView.SelectedIndexCollection = Me.m_lvCategoriesGroup.SelectedIndices
        If (sic.Count > 0) Then
            Me.SelectedGroupCategory = DirectCast(sic(0), eGroupCategoryTypes)
        End If

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Process group selection change.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub m_clbGroups_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_glbGroups.SelectedIndexChanged

        If (Me.m_session Is Nothing) Then Return
        If (Me.m_bInUpdate = True) Then Return
        Me.ApplyGroupControls(Me.SelectedGroupCategory)

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Select all groups into the current group category.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub m_btnAllGroups_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_btnAllGroups.Click

        If (Me.m_session Is Nothing) Then Return
        If (Me.m_bInUpdate = True) Then Return

        For i As Integer = 0 To Me.m_glbGroups.Items.Count - 1
            Me.m_glbGroups.SetSelected(i, True)
        Next

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Unselect all groups frome the current groups category.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub m_btnNoneGroups_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_btnNoneGroups.Click, m_btnNoModels.Click

        If (Me.m_session Is Nothing) Then Return
        If (Me.m_bInUpdate = True) Then Return

        For i As Integer = 0 To Me.m_glbGroups.Items.Count - 1
            Me.m_glbGroups.SetSelected(i, False)
        Next

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Process fleet category selection change.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub m_lvCategoriesFleet_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_lvCategoriesFleet.SelectedIndexChanged

        If (Me.m_session Is Nothing) Then Return
        If (Me.m_bInUpdate = True) Then Return

        Dim sic As ListView.SelectedIndexCollection = Me.m_lvCategoriesFleet.SelectedIndices
        If (sic.Count > 0) Then
            Me.SelectedFleetCategory = DirectCast(sic(0), eFleetCategoryTypes)
        End If

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Process fleet selection change.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub m_lbFleets_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_flbFleets.SelectedIndexChanged

        If (Me.m_session Is Nothing) Then Return
        If (Me.m_bInUpdate = True) Then Return
        Me.ApplyFleetControls(Me.SelectedFleetCategory)

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Select all fleets into the current fleet category.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub m_btnAllFleets_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_btnAllFleets.Click

        If (Me.m_session Is Nothing) Then Return
        If (Me.m_bInUpdate = True) Then Return

        For i As Integer = 0 To Me.m_flbFleets.Items.Count - 1
            Me.m_flbFleets.SetSelected(i, True)
        Next

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Deselect all fleets into the current fleet category.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub m_btnNoneFleets_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_btnNoneFleets.Click

        If (Me.m_session Is Nothing) Then Return
        If (Me.m_bInUpdate = True) Then Return

        For i As Integer = 0 To Me.m_flbFleets.Items.Count - 1
            Me.m_flbFleets.SetSelected(i, False)
        Next

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Process scenario listbox focus loss.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub OnScenariosLBLostFocus(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_clbScenarios.LostFocus

        If (Me.m_session Is Nothing) Then Return
        If (Me.m_bInUpdate = True) Then Return
        Me.ApplyScenarioControls()

    End Sub


    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Select all Ecosim results to be written to CSV file.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub m_btnAllResults_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_btnAllResults.Click

        If (Me.m_session Is Nothing) Then Return
        If (Me.m_bInUpdate = True) Then Return

        For i As Integer = 0 To Me.m_clbEcosimResults.Items.Count - 1
            Me.m_clbEcosimResults.SetItemChecked(i, True)
        Next

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Clear all Ecosim results to be written to CSV file.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub m_btnNoneResults_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_btnNoneResults.Click

        If (Me.m_session Is Nothing) Then Return
        If (Me.m_bInUpdate = True) Then Return

        For i As Integer = 0 To Me.m_clbEcosimResults.Items.Count - 1
            Me.m_clbEcosimResults.SetItemChecked(i, False)
        Next

    End Sub

    Private Sub OnEcosimResultsCheck(sender As Object, e As System.Windows.Forms.ItemCheckEventArgs) _
        Handles m_clbEcosimResults.ItemCheck

        If (Me.m_session Is Nothing) Then Return
        If (Me.m_bInUpdate = True) Then Return

        Try
            Dim lResults As New List(Of cEcosimResultWriter.eResultTypes)
            For Each item As Object In Me.m_clbEcosimResults.CheckedItems
                lResults.Add(DirectCast(item, cEcosimResultWriter.eResultTypes))
            Next
            Me.m_session.EcosimResults = lResults.ToArray()
        Catch ex As Exception

        End Try

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Browse for output directory.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub m_btnBrowse_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_btnBrowse.Click

        If (Me.m_session Is Nothing) Then Return
        If (Me.m_bInUpdate = True) Then Return

        Dim cmdDOC As cDirectoryOpenCommand = DirectCast(Me.CommandHandler.GetCommand(cDirectoryOpenCommand.COMMAND_NAME), cDirectoryOpenCommand)

        cmdDOC.Directory = Me.m_tbxOutputDirectory.Text
        cmdDOC.Invoke()

        If (cmdDOC.Result = Windows.Forms.DialogResult.OK) Then
            Me.m_tbxOutputDirectory.Text = cmdDOC.Directory
            Me.m_session.OutputPath = Me.m_tbxOutputDirectory.Text
        End If
        Me.UpdateControls()

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Directory mask has changed; validate the mask
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub m_tbxMask_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_tbxMask.TextChanged

        If (Me.m_session Is Nothing) Then Return
        Me.m_session.DirectoryMask = Me.m_tbxMask.Text

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Process a change in the number of years that a simulation must run for.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub m_nudNumberOfYears_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_nudNumberOfYears.ValueChanged

        If (Me.m_session Is Nothing) Then Return
        Me.m_session.NumberOfYears = Convert.ToInt32(Me.m_nudNumberOfYears.Value)
        Me.UpdateControls()

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Process a change in the way values need to be aggregated.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub m_cbxAnnualAverages_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)


        If (Me.m_session Is Nothing) Then Return
        Me.UpdateControls()

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Run!
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub m_btnRun_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_btnRun.Click

        If (Me.m_session Is Nothing) Then Return

        Me.m_bRunning = True
        Me.UpdateControls()

        Me.RunSession()

        Me.m_bRunning = False
        Me.UpdateControls()

    End Sub

#End Region ' Events

#Region " Internals "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Local flag, stating whether controls are being updated and thus should 
    ''' refrain from adusting model variables.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private m_bInUpdate As Boolean = False

#Region " Session "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Load session from file.
    ''' </summary>
    ''' <param name="strFileName"></param>
    ''' -----------------------------------------------------------------------
    Private Sub LoadSession(ByVal strFileName As String)

        Debug.Assert(Me.m_session IsNot Nothing)
        Debug.Assert(String.IsNullOrEmpty(strFileName) = False)

        Me.m_session.Load(strFileName)
        Me.UpdateSessionControls()

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Save session to file.
    ''' </summary>
    ''' <param name="strFileName"></param>
    ''' -----------------------------------------------------------------------
    Private Sub SaveSession(ByVal strFileName As String)

        Debug.Assert(Me.m_session IsNot Nothing)
        Debug.Assert(String.IsNullOrEmpty(strFileName) = False)

        Me.m_session.Save(strFileName)

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Reset session.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub ResetSession()

        Debug.Assert(Me.m_session IsNot Nothing)

        Me.m_session.Reset()
        Me.UpdateSessionControls()

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Update all session-specific UI controls
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub UpdateSessionControls()

        Dim result As cEcosimResultWriter.eResultTypes

        Me.m_bInUpdate = True
        Me.m_tbxOutputDirectory.Text = Me.m_session.OutputPath
        Me.m_tbxMask.Text = Me.m_session.DirectoryMask
        Me.m_nudNumberOfYears.Value = Me.m_session.NumberOfYears

        Me.m_clbModels.Items.Clear()
        For Each ms As cModelSettings In Me.m_session.Models
            Dim i As Integer = Me.m_clbModels.Items.Add(ms)
            Me.m_clbModels.SetItemChecked(i, ms.Enabled)
        Next

        For i As Integer = 0 To Me.m_clbEcosimResults.Items.Count - 1
            result = DirectCast(Me.m_clbEcosimResults.Items(i), cEcosimResultWriter.eResultTypes)
            Me.m_clbEcosimResults.SetItemChecked(i, Array.IndexOf(Me.m_session.EcosimResults, result) > -1)
        Next
        Me.m_bInUpdate = False

        ' Select first model
        If (Me.m_clbModels.Items.Count = 0) Then
            Me.SelectedModel = Nothing
        Else
            Me.SelectedModel = DirectCast(Me.m_clbModels.Items(0), cModelSettings)
        End If

    End Sub

#End Region ' Session

#Region " Model settings "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Add a model to the current session.
    ''' </summary>
    ''' <param name="strFileName"></param>
    ''' -----------------------------------------------------------------------
    Private Sub AddModel(ByVal strFileName As String)

        Debug.Assert(Me.m_session IsNot Nothing)
        Debug.Assert(String.IsNullOrEmpty(strFileName) = False)

        Dim ms As cModelSettings = (Me.m_session.Model(strFileName))
        If (ms Is Nothing) Then
            ms = New cModelSettings(strFileName)
            If Me.m_session.AddModel(ms) Then
                Dim i As Integer = Me.m_clbModels.Items.Add(ms)
                Me.m_clbModels.SetItemChecked(i, ms.Enabled)
            Else
                Return
            End If
        End If

        Me.SelectedModel = ms

        ' - add groups
        For iGroup As Integer = 1 To Me.Core.nGroups
            ms.Groups(eGroupCategoryTypes.All).Add(iGroup)
        Next
        ' Show it
        Me.UpdateModelControls()

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Remove a model from the current session.
    ''' </summary>
    ''' <param name="ms"></param>
    ''' -----------------------------------------------------------------------
    Private Sub RemoveModel(ByVal ms As cModelSettings)

        Debug.Assert(Me.m_session IsNot Nothing)
        Debug.Assert(ms IsNot Nothing)

        Dim iIndex As Integer = Me.m_clbModels.Items.IndexOf(ms)

        Me.m_clbModels.Items.Remove(ms)
        If (Me.m_session.RemoveModel(ms)) Then

            If Me.m_clbModels.Items.Count > 0 Then
                Me.SelectedModel = DirectCast(Me.m_clbModels.Items(Math.Min(Me.m_clbModels.Items.Count - 1, Math.Max(0, iIndex - 1))), cModelSettings)
            Else
                Me.SelectedModel = Nothing
            End If

        End If

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the selected Model settings
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Property SelectedModel() As cModelSettings
        Get
            Return Me.m_ms
        End Get

        Set(ByVal ms As cModelSettings)

            ' Optimizations
            If (Me.m_bInUpdate = True) Then Return
            If (Object.ReferenceEquals(Me.m_ms, ms) = True) Then Return

            Me.m_bInUpdate = True

            cApplicationStatusNotifier.StartProgress(Me.Core, "Plug-in is switching models, please wait...")

            If (Me.m_ms IsNot Nothing) Then
                If Me.CloseModel() Then
                    Me.m_glbGroups.Detach()
                    Me.m_flbFleets.Detach()
                    Me.m_clbScenarios.Items.Clear()
                End If
            End If

            Me.m_ms = ms
            Me.m_clbModels.SelectedItem = ms

            If (Me.m_ms IsNot Nothing) Then
                If Me.LoadModel(ms) Then
                    Me.m_glbGroups.Attach(Me.UIContext)
                    Me.m_flbFleets.Attach(Me.UIContext)
                End If
            End If

            cApplicationStatusNotifier.EndProgress(Me.Core)

            Me.UpdateModelControls()
            Me.m_bInUpdate = False

            Me.SelectedGroupCategory = eGroupCategoryTypes.All
            Me.SelectedFleetCategory = eFleetCategoryTypes.All

        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Update all model-specific UI controls.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub UpdateModelControls()

        For Each cat As eGroupCategoryTypes In [Enum].GetValues(GetType(eGroupCategoryTypes))
            Me.UpdateGroupControls(cat)
        Next

        For Each cat As eFleetCategoryTypes In [Enum].GetValues(GetType(eFleetCategoryTypes))
            Me.UpdateFleetControls(cat)
        Next

        Me.m_clbScenarios.Items.Clear()
        For i As Integer = 1 To Me.Core.nEcosimScenarios
            Me.m_clbScenarios.Items.Add(New cCoreInputOutputListboxItem(Me.Core.EcosimScenarios(i)))
        Next
        Me.UpdateScenarioControls()

    End Sub

#End Region ' Model settings

#Region " Scenario settings "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Update all scenario UI controls.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub UpdateScenarioControls()

        Dim ms As cModelSettings = Me.SelectedModel

        ' Optimizations
        If (ms Is Nothing) Then Return

        Me.m_bInUpdate = True

        For i As Integer = 0 To Me.m_clbScenarios.Items.Count - 1
            Me.m_clbScenarios.SetSelected(i, False)
        Next
        For Each iScenario As Integer In ms.Scenarios
            Me.m_clbScenarios.SetSelected(iScenario - 1, True)
        Next

        Me.m_bInUpdate = False

        Me.UpdateControls()

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Apply scenario UI control content to the current selected model.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub ApplyScenarioControls()

        Dim ms As cModelSettings = Me.SelectedModel
        Dim lin As New List(Of Integer)

        If ms Is Nothing Then Return

        For Each obj As Object In Me.m_clbScenarios.CheckedItems
            lin.Add(DirectCast(obj, cCoreInputOutputListboxItem).Source.Index)
        Next
        ms.Scenarios = lin

    End Sub

#End Region ' Scenario settings

#Region " Groups "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the selected group category.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Property SelectedGroupCategory() As eGroupCategoryTypes
        Get
            Return Me.m_groupcat
        End Get

        Set(ByVal cat As eGroupCategoryTypes)

            Dim ms As cModelSettings = Me.SelectedModel

            ' Optimizations
            If (Me.m_bInUpdate = True) Then Return
            'If (Me.m_groupcat = cat) Then Return
            If (ms Is Nothing) Then Return

            Me.m_bInUpdate = True

            Me.m_groupcat = cat
            Me.m_lvCategoriesGroup.Items(cat).Selected = True

            For i As Integer = 0 To Me.m_glbGroups.Items.Count - 1
                Me.m_glbGroups.SetSelected(i, False)
            Next
            For Each iGroup As Integer In ms.Groups(cat)
                Me.m_glbGroups.IsGroupSelected(iGroup) = True
            Next

            Me.m_bInUpdate = False
            Me.UpdateControls()

        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Apply group UI control content for a given group category to the 
    ''' current selected model.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub ApplyGroupControls(ByVal cat As eGroupCategoryTypes)

        Dim ms As cModelSettings = Me.SelectedModel
        Dim lin As New List(Of Integer)

        If ms Is Nothing Then Return

        For Each iItem As Integer In Me.m_glbGroups.SelectedIndices
            Dim grp As cCoreGroupBase = Me.m_glbGroups.GetGroupAt(iItem)
            If grp IsNot Nothing Then lin.Add(grp.Index)
        Next
        ms.Groups(cat) = lin
        Me.UpdateGroupControls(cat)

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Update group-specific UI control content for a given group category.
    ''' </summary>
    ''' <param name="cat"></param>
    ''' -----------------------------------------------------------------------
    Private Sub UpdateGroupControls(ByVal cat As eGroupCategoryTypes)

        Dim ms As cModelSettings = Me.SelectedModel
        Dim iCount As Integer = 0

        If (ms IsNot Nothing) Then
            iCount = ms.Groups(cat).Count
        End If
        Me.m_lvCategoriesGroup.Items(cat).SubItems(1).Text = CStr(iCount)

    End Sub

#End Region ' Groups

#Region " Fleets "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the selected fleet category.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Property SelectedFleetCategory() As eFleetCategoryTypes
        Get
            Return Me.m_fleetcat
        End Get
        Set(ByVal cat As eFleetCategoryTypes)

            Dim ms As cModelSettings = Me.SelectedModel

            If (Me.m_bInUpdate = True) Then Return
            If (ms Is Nothing) Then Return
            'If (Me.m_fleetcat = cat) Then Return

            Me.m_bInUpdate = True

            Me.m_fleetcat = cat
            Me.m_lvCategoriesFleet.Items(cat).Selected = True

            For i As Integer = 0 To Me.m_flbFleets.Items.Count - 1
                Me.m_flbFleets.SetSelected(i, False)
            Next
            For Each iFleet As Integer In ms.Fleets(cat)
                Me.m_flbFleets.SetSelected(iFleet - 1, True)
            Next

            Me.m_bInUpdate = False

            Me.UpdateControls()

        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Apply fleet UI control content to the current model.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub ApplyFleetControls(ByVal cat As eFleetCategoryTypes)

        Dim ms As cModelSettings = Me.SelectedModel
        Dim lin As New List(Of Integer)

        If ms Is Nothing Then Return

        For Each iItem As Integer In Me.m_flbFleets.SelectedIndices
            lin.Add(Me.m_flbFleets.GetFleetAt(iItem).Index)
        Next

        ms.Fleets(cat) = lin
        Me.UpdateFleetControls(cat)

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Update fleet-specific UI control content for a given fleet category.
    ''' </summary>
    ''' <param name="cat"></param>
    ''' -----------------------------------------------------------------------
    Private Sub UpdateFleetControls(ByVal cat As eFleetCategoryTypes)

        Dim ms As cModelSettings = Me.SelectedModel
        Dim iCount As Integer = 0

        If (ms IsNot Nothing) Then
            iCount = ms.Fleets(cat).Count
        End If
        Me.m_lvCategoriesFleet.Items(cat).SubItems(1).Text = CStr(iCount)

    End Sub

#End Region ' Fleets

#Region " Generic "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Update editable state of all UI controls.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Protected Overrides Sub UpdateControls()

        Dim bIsRunning As Boolean = Me.m_bRunning
        Dim bHasModel As Boolean = (Me.SelectedModel IsNot Nothing)
        Dim bHasOutputDir As Boolean = (String.IsNullOrEmpty(Me.m_session.OutputPath) = False)
        Dim bHasValidMask As Boolean = String.IsNullOrEmpty(Me.m_session.DirectoryMask) Or _
                Not String.IsNullOrEmpty(Me.GetMaskedDirectoryName(Me.m_session.DirectoryMask, "a", "b", "c", "d"))

        Me.m_tsbReset.Enabled = Not bIsRunning
        Me.m_tssbLoad.Enabled = Not bIsRunning
        Me.m_tsbSave.Enabled = Not bIsRunning

        Me.m_clbModels.Enabled = Not bIsRunning
        Me.m_btnAddModel.Enabled = Not bIsRunning
        Me.m_bntRemoveModel.Enabled = Not bIsRunning

        Me.m_lvCategoriesGroup.Enabled = (bIsRunning = False) And (bHasModel = True)
        Me.m_glbGroups.Enabled = (bIsRunning = False) And (bHasModel = True)
        Me.m_btnAllGroups.Enabled = (bIsRunning = False) And (bHasModel = True)
        Me.m_btnNoneGroups.Enabled = (bIsRunning = False) And (bHasModel = True)

        Me.m_lvCategoriesFleet.Enabled = (bIsRunning = False) And (bHasModel = True)
        Me.m_flbFleets.Enabled = (bIsRunning = False) And (bHasModel = True)
        Me.m_btnAllFleets.Enabled = (bIsRunning = False) And (bHasModel = True)
        Me.m_btnNoneFleets.Enabled = (bIsRunning = False) And (bHasModel = True)

        Me.m_clbEcosimResults.Enabled = (bIsRunning = False) And (bHasModel = True)
        Me.m_btnAllResults.Enabled = (bIsRunning = False) And (bHasModel = True)
        Me.m_btnNoneResults.Enabled = (bIsRunning = False) And (bHasModel = True)

        Me.m_tbxOutputDirectory.Enabled = (bIsRunning = False)
        Me.m_tbxMask.Enabled = (bIsRunning = False)
        Me.m_btnBrowse.Enabled = (bIsRunning = False)
        Me.m_nudNumberOfYears.Enabled = (bIsRunning = False)

        Me.m_btnRun.Enabled = (bIsRunning = False) And _
                              (bHasModel = True) And _
                              (bHasOutputDir = True) And _
                              (bHasValidMask = True)

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Report an error via the EwE core.
    ''' </summary>
    ''' <param name="strError"></param>
    ''' <param name="importance"></param>
    ''' -----------------------------------------------------------------------
    Private Sub ReportError(ByVal strError As String, Optional ByVal importance As eMessageImportance = eMessageImportance.Critical)
        Dim msg As New cMessage(strError, eMessageType.Any, EwEUtils.Core.eCoreComponentType.External, importance)
        Me.Core.Messages.SendMessage(msg)
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Interpret the directory mask and turn it into a folder path.
    ''' </summary>
    ''' <returns>An interpreted version of the mask, or an empty string when 
    ''' either the input mask was empty OR an error occurred.</returns>
    ''' -----------------------------------------------------------------------
    Private Function GetMaskedDirectoryName(ByVal strMask As String, _
                                    ByVal strModel As String, _
                                    ByVal strScenario As String, _
                                    ByVal strCategory As String, _
                                    ByVal strTimeSeries As String) As String

        Dim sbResult As New StringBuilder()

        ' i holds the index of the next placeholder start character '{', or -1 if no more placeholders are present
        Dim i As Integer
        ' j holds the position of the character right after the last placeholder end character '}'
        Dim j As Integer
        ' k holds the position of the next precision character ':'
        Dim k As Integer
        ' l holds j or k, whichever comes first
        Dim l As Integer
        ' n holds the precision for a placeholder
        Dim n As Integer
        ' Placeholder replacement value
        Dim strTag As String = ""

        ' Use curly brackets instead of '['
        strMask = strMask.Replace("["c, "{"c).Replace("]"c, "}"c)

        ' Ok, this routing is unpleasantly hard to read. It reads the input mask, looking
        ' for fields that look like "{<fieldname>{:<precision>}", replacing the fieldname
        ' text with a replacement string. If a precision is given, only that number of characters
        ' of the replacement string are used. If no precision is given all characters of the input
        ' string are used.

        ' If a mask has been provided
        If Not String.IsNullOrEmpty(strMask) Then
            ' Until no more placeholders are found
            While (i <> -1)
                ' Find next placeholder
                i = strMask.IndexOf("{"c, j)
                ' No more placeholders?
                If (i = -1) Then
                    ' #Yes: add rest of the mask
                    sbResult.Append(strMask.Substring(j))
                Else
                    ' #No: copy the part between the last '}' and the next '{'
                    sbResult.Append(strMask.Substring(j, i - j))

                    ' Find accompanying closing placeholder character '}'
                    j = strMask.IndexOf("}", i)
                    ' Find accompanying precision character ':'
                    k = strMask.IndexOf(":", i)
                    ' If no ':' found then set k to j. This will trick the code below to ignore the precision
                    If k = -1 Then k = j
                    ' Find length of the placeholder text, which is indicated by either j or k - whichever comes first
                    l = Math.Min(j, k)

                    ' Validate
                    If (j = -1) Then Return ""
                    If (l = i + 1) Then Return ""

                    ' Get precision, if any
                    If (k < j) Then
                        Try
                            ' Try to read precision number
                            n = CInt(Val(strMask.Substring(k + 1, j - k - 1)))
                        Catch ex As Exception
                            ' Did not work? Fail without complaining (which is not handy)
                            Return ""
                        End Try
                    Else
                        ' No precision: use entire placeholder value
                        n = 9999
                    End If

                    ' Extract placeholder tag
                    strTag = strMask.Substring(i + 1, l - i - 1)
                    ' Find replacement value
                    Select Case strTag.ToLower
                        Case "model" : strTag = strModel
                        Case "scenario" : strTag = strScenario
                        Case "category" : strTag = strCategory
                        Case "timeseries" : strTag = strTimeSeries
                    End Select
                    ' Truncate precision to the number of characters in a tag
                    n = Math.Min(n, strTag.Length)
                    ' Append number of desired characters to the resulting output
                    sbResult.Append(strTag.Substring(0, n))

                    ' Skip mask
                    j += 1
                End If
            End While
        End If

        Return cFileUtils.ToValidFileName(sbResult.ToString(), True)

    End Function

#End Region ' Generic

#Region " Running "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub RunSession()

        Dim model As cEwEModel = Nothing
        Dim scenario As cEcoSimScenario = Nothing
        Dim parms As cEcoSimModelParameters = Nothing
        Dim tsds As cTimeSeriesDataset = Nothing
        Dim ts As cTimeSeries = Nothing
        Dim iTotalSteps As Integer = Me.m_session.NumSteps
        Dim iStep As Integer = 0

        Me.m_prog = New frmProgress(Me.UIContext)
        Me.m_prog.Show(Me)

        ' For each model in the session
        For Each ms As cModelSettings In Me.m_session.Models
            '   Load model
            If Me.LoadModel(ms) Then
                model = Me.Core.EwEModel
                ' For all desired scenarios
                For Each iScenario In ms.Scenarios
                    scenario = Me.Core.EcosimScenarios(iScenario)
                    ' Forget any changes
                    Me.Core.DiscardChanges()
                    ' Able to load?
                    If Me.Core.LoadEcosimScenario(scenario) Then
                        ' Get parameters
                        parms = Me.Core.EcoSimModelParameters
                        ' For each available time series dataset
                        For iTSDS As Integer = 1 To Me.Core.nTimeSeriesDatasets
                            ' Forget any changes
                            Me.Core.DiscardChanges()
                            ' Load this time series dataset
                            If (Me.Core.LoadTimeSeries(iTSDS, False)) Then

                                ' Apply time series dataset
                                tsds = Me.Core.TimeSeriesDataset(iTSDS)
                                ' For each group category
                                For Each cat As eGroupCategoryTypes In [Enum].GetValues(GetType(eGroupCategoryTypes))

                                    iStep += 1

                                    ' Are groups assigned to this category?
                                    If (ms.Groups(cat).Count > 0) Then

                                        Me.m_prog.SetStatus(String.Format("Processing model {0}, scenario {1}, groups category {2}, time series {3}",
                                                                          Path.GetFileNameWithoutExtension(ms.FileName),
                                                                          scenario.Name, cat.ToString(), tsds.Name), CSng(iStep / iTotalSteps))

                                        ' Apply TS to each category
                                        ' ..first disable all TS
                                        For iTS As Integer = 1 To Me.Core.nTimeSeries
                                            Me.Core.EcosimTimeSeries(iTS).Enabled = False
                                        Next

                                        ' ..enable all TS for selected groups
                                        For Each iGroup As Integer In ms.Groups(cat)
                                            ' Enable all time series that apply to this group
                                            For iTS As Integer = 1 To Me.Core.nTimeSeries
                                                ts = Me.Core.EcosimTimeSeries(iTS)
                                                If TypeOf (ts) Is cGroupTimeSeries Then
                                                    If (DirectCast(ts, cGroupTimeSeries).GroupIndex = iGroup) Then
                                                        ' Enable TS for this group
                                                        ts.Enabled = True
                                                    End If
                                                End If
                                            Next
                                        Next

                                        ' Apply TS
                                        Me.Core.UpdateTimeSeries()

                                        ' Set ecosim num years to overwite the number of years set by TS
                                        parms.SetVariable(eVarNameFlags.EcoSimNYears, _
                                                          Me.m_session.NumberOfYears)

                                        ' Run Ecosim
                                        If Me.Core.RunEcoSim() Then
                                            ' Create directory name
                                            Dim strDirectory As String = Path.Combine(Me.m_session.OutputPath, _
                                                Me.GetMaskedDirectoryName(Me.m_session.DirectoryMask, _
                                                                          model.Name, _
                                                                          scenario.Name, _
                                                                          cat.ToString(), _
                                                                          tsds.Name))

                                            ' Create directory if not existent
                                            If cFileUtils.IsDirectoryAvailable(strDirectory, True) Then
                                                Me.ReportError(String.Format("Unable to create directory '{0}', please check for write access", strDirectory))
                                                Return
                                            End If

                                            ' Write Ecosim data
                                            Dim writer As New cEcosimResultWriter(Me.Core)
                                            Try
                                                ' Write selected results only
                                                writer.WriteResults(strDirectory, Me.m_session.EcosimResults)
                                            Catch ex As Exception
                                                Me.ReportError(String.Format("Ecosim failed to save data, please check for write access or disk space on '{0}", strDirectory))
                                                Return
                                            End Try

                                            Try
                                                ' Tell Network Analysis to write data
                                                If (Me.m_naManager IsNot Nothing) Then
                                                    Dim writerNA As New EwENetworkAnalysis.cResultWriter(Me.m_naManager)
                                                    writerNA.WriteCurrentResults(strDirectory)
                                                End If
                                            Catch ex As Exception
                                                Me.ReportError(String.Format("Network Analysis error: {0}", ex.Message))
                                                Return
                                            End Try
                                        Else
                                            Me.ReportError("Ecosim failed to run, please check this model in the scientific interface")
                                            Return
                                        End If
                                    Else
                                        ' Category skipped
                                    End If
                                Next cat

                                ' For each fleet category
                                For Each cat As eFleetCategoryTypes In [Enum].GetValues(GetType(eFleetCategoryTypes))

                                    iStep += 1

                                    ' Are groups assigned to this category?
                                    If (ms.Fleets(cat).Count > 0) Then

                                        Me.m_prog.SetStatus(String.Format("Processing model {0}, scenario {1}, fleet category {2}, time series {3}",
                                                                          Path.GetFileNameWithoutExtension(ms.FileName),
                                                                          scenario.Name, cat.ToString(), tsds.Name), CSng(iStep / iTotalSteps))

                                        ' Apply TS to each category
                                        ' ..first disable all TS
                                        For iTS As Integer = 1 To Me.Core.nTimeSeries
                                            Me.Core.EcosimTimeSeries(iTS).Enabled = False
                                        Next

                                        ' ..enable all TS for selected fleets
                                        For Each iFleet As Integer In ms.Fleets(cat)
                                            ' Enable all time series that apply to this fleet
                                            For iTS As Integer = 1 To Me.Core.nTimeSeries
                                                ts = Me.Core.EcosimTimeSeries(iTS)
                                                If TypeOf (ts) Is cFleetTimeSeries Then
                                                    If (DirectCast(ts, cFleetTimeSeries).FleetIndex = iFleet) Then
                                                        ' Enable TS for this fleet
                                                        ts.Enabled = True
                                                    End If
                                                End If
                                            Next
                                        Next

                                        ' Apply TS
                                        Me.Core.UpdateTimeSeries()

                                        ' Set ecosim num years to overwite the number of years set by TS
                                        parms.SetVariable(eVarNameFlags.EcoSimNYears, _
                                                          Me.m_session.NumberOfYears)
                                        ' Forget this change
                                        Me.Core.DiscardChanges()

                                        ' Run Ecosim
                                        If Me.Core.RunEcoSim() Then
                                            ' Create directory name
                                            Dim strDirectory As String = Path.Combine(Me.m_session.OutputPath, _
                                                Me.GetMaskedDirectoryName(Me.m_session.DirectoryMask, _
                                                                          model.Name, _
                                                                          scenario.Name, _
                                                                          cat.ToString(), _
                                                                          tsds.Name))
                                            ' Create directory if not existent
                                            If cFileUtils.IsDirectoryAvailable(strDirectory) Then
                                                Me.ReportError(String.Format("Unable to create directory '{0}', please check for write access", strDirectory))
                                                Return
                                            End If

                                            ' Write Ecosim data
                                            Dim writer As New cEcosimResultWriter(Me.Core)
                                            Try
                                                ' Write selected results only
                                                writer.WriteResults(strDirectory, Me.m_session.EcosimResults)
                                            Catch ex As Exception
                                                Me.ReportError(String.Format("Ecosim failed to save data, please check for write access or disk space on '{0}", strDirectory))
                                                Return
                                            End Try


                                            Try
                                                ' Tell Network Analysis to write data
                                                If (Me.m_naManager IsNot Nothing) Then
                                                    Dim writerNA As New EwENetworkAnalysis.cResultWriter(Me.m_naManager)
                                                    writerNA.WriteCurrentResults(strDirectory)
                                                End If
                                            Catch ex As Exception
                                                Me.ReportError(String.Format("Network Analysis error: {0}", ex.Message))
                                                Return
                                            End Try
                                        Else
                                            Me.ReportError("Ecosim failed to run, please check this model in the scientific interface")
                                            Return
                                        End If
                                    Else
                                        ' Category skipped
                                    End If
                                Next cat

                            Else
                                Me.ReportError(String.Format("Unable to load time series dataset {0}, please check this data in the scientific interface", iTSDS))
                                Return
                            End If
                        Next iTSDS
                    Else
                        Me.ReportError("Unable to load Ecosim scenario 1")
                        Return
                    End If
                Next iScenario
            Else
                Me.ReportError("Unable to load model?! That is bizarre!")
                Return
            End If
        Next

        Me.m_prog.Close()
        Me.m_prog.Dispose()
        Me.m_prog = Nothing

    End Sub

#End Region ' Running

#Region " EwE control "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Close the EwE model
    ''' </summary>
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Private Function CloseModel() As Boolean
        Me.Core.DiscardChanges()
        Return Me.Core.CloseModel()
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Load an EwE model.
    ''' </summary>
    ''' <param name="ms"></param>
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Private Function LoadModel(ByVal ms As cModelSettings) As Boolean

        Dim ds As IEwEDataSource = Nothing
        Dim bSucces As Boolean = False

        If (ms Is Nothing) Then Return bSucces

        ds = cDataSourceFactory.Create(ms.FileName)
        If (ds.Open(ms.FileName, Me.Core) = EwEUtils.Core.eDatasourceAccessType.Opened) Then
            bSucces = Me.Core.LoadModel(ds)
        End If

        Return bSucces

    End Function

#End Region ' EwE control

#End Region ' Internals

End Class
