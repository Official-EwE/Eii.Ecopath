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
Option Explicit On
Imports EwEUtils.Utilities
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region

Namespace Ecosim

    ''' =======================================================================
    ''' <summary>
    ''' Dialog class, implements the generic show/hide items interface.
    ''' </summary>
    ''' =======================================================================
    Public Class dlgShowHideItems

        Private Class cPresetData

            Private m_lHiddenGroups As New List(Of Integer)
            Private m_lHiddenFleets As New List(Of Integer)

            Public Sub New(n As String, bIsDefault As Boolean)
                Me.Name = n
                Me.IsDefault = bIsDefault
            End Sub

            Public Property Name As String

            Public Property GroupVisible(iGroup As Integer) As Boolean
                Get
                    Return Not Me.m_lHiddenGroups.Contains(iGroup)
                End Get
                Set(value As Boolean)
                    Me.m_lHiddenGroups.Remove(iGroup)
                    If (value = False) Then
                        Me.m_lHiddenGroups.Add(iGroup)
                    End If
                End Set
            End Property

            Public Property FleetVisible(iFleet As Integer) As Boolean
                Get
                    Return Not Me.m_lHiddenFleets.Contains(iFleet)
                End Get
                Set(value As Boolean)
                    Me.m_lHiddenFleets.Remove(iFleet)
                    If (value = False) Then
                        Me.m_lHiddenFleets.Add(iFleet)
                    End If
                End Set
            End Property

            Public Property IsDefault As Boolean = False
            Public Overrides Function ToString() As String
                Return Me.Name
            End Function

        End Class

#Region " Private variables "

        Private m_uic As cUIContext = Nothing
        Private m_bInSync As Boolean = False
        Private m_il As ImageList = Nothing

        ' -- item visibility presets --
        Private m_lPresets As New List(Of cPresetData)

#End Region ' Private variables

#Region " Constructor "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Create a new dialog.
        ''' </summary>
        ''' <param name="uic">The UI context to connect to.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(uic As cUIContext)
            Me.InitializeComponent()
            Debug.Assert(uic IsNot Nothing)
            Me.m_uic = uic
        End Sub

#End Region ' Constructor

#Region " Form overrides "

        Protected Overrides Sub OnLoad(e As System.EventArgs)
            MyBase.OnLoad(e)

            If (Me.m_uic Is Nothing) Then Return

            Dim core As cCore = Me.m_uic.Core
            Dim sg As cStyleGuide = Me.m_uic.StyleGuide
            Dim names As String() = sg.ItemVisibilityPresetNames
            Dim prSel As cPresetData = Nothing

            For i As Integer = 1 To names.Count - 1
                ' Create snapshot of preset
                Dim prNew As New cPresetData(names(i), sg.IsItemVisibilityPresetDefault(names(i)))
                For j As Integer = 1 To core.nGroups
                    prNew.GroupVisible(j - 1) = sg.GroupVisible(j)
                Next
                For j As Integer = 1 To core.nFleets
                    prNew.FleetVisible(j - 1) = sg.FleetVisible(j)
                Next
                Me.m_lPresets.Add(prNew)
                ' Is selected?
                If (names(i) = sg.SelectedItemVisibilityPresetName) Then
                    prSel = prNew
                End If
            Next
            If (prSel Is Nothing) Then prSel = Me.m_lPresets(0)

            Me.m_bInSync = True

            Me.m_clbGroups.Items.Clear()
            For iGroup As Integer = 1 To core.nGroups
                Dim group As cEcoPathGroupInput = core.EcopathGroupInputs(iGroup)
                Me.m_clbGroups.Items.Add(New cCoreInputOutputControlItem(group), False)
            Next

            Me.m_clbFleets.Items.Clear()
            For iFleet As Integer = 1 To Me.m_uic.Core.nFleets
                Dim fleet As cEcopathFleetInput = Me.m_uic.Core.EcopathFleetInputs(iFleet)
                Me.m_clbFleets.Items.Add(New cCoreInputOutputControlItem(fleet), False)
            Next

            Me.m_bInSync = False

            Me.m_cbSyncViaFishing.Checked = My.Settings.SelectionLinkFishing
            Me.m_cbSyncPredators.Checked = My.Settings.SelectionLinkPredators
            Me.m_cbSyncPrey.Checked = My.Settings.SelectionLinkPreys

            Me.m_il = New ImageList()
            Me.m_il.Images.Add(SharedResources.fish)
            Me.m_il.Images.Add(SharedResources.fishing_gear)

            Me.UpdatePresetsDropdown(prSel)
            Me.UpdateControls()

        End Sub

        Protected Overrides Sub OnFormClosed(e As System.Windows.Forms.FormClosedEventArgs)
            My.Settings.SelectionLinkFishing = Me.m_cbSyncViaFishing.Checked
            My.Settings.SelectionLinkPredators = Me.m_cbSyncPredators.Checked
            My.Settings.SelectionLinkPreys = Me.m_cbSyncPrey.Checked
            My.Settings.Save()
            MyBase.OnFormClosed(e)
        End Sub

#End Region ' Form overrides

#Region " Events "

        Private Sub OnPresetNameChanged(sender As Object, e As EventArgs) Handles m_tbxName.TextChanged
            Me.UpdateControls()
        End Sub

        Private Sub OnPresetSelected(sender As Object, e As EventArgs) Handles m_cmbPresets.SelectedIndexChanged
            Me.LoadPreset()
            Me.UpdateControls()
        End Sub

        Private Sub OnAddPreset(sender As Object, e As EventArgs) Handles m_btnAdd.Click

            Dim pr As New cPresetData(Me.m_tbxName.Text.Trim, False)
            Me.m_lPresets.Add(pr)
            Me.UpdatePresetsDropdown(pr)

        End Sub

        Private Sub OnRenamePreset(sender As Object, e As EventArgs) Handles m_btnRename.Click
            Dim pr As cPresetData = DirectCast(Me.m_cmbPresets.SelectedItem, cPresetData)
            pr.Name = Me.m_tbxName.Text
            Me.UpdatePresetsDropdown(pr)
        End Sub

        Private Sub OnDeletePreset(sender As Object, e As EventArgs) Handles m_btnDelete.Click

            Dim pr As cPresetData = DirectCast(Me.m_cmbPresets.SelectedItem, cPresetData)
            Dim i As Integer = Me.m_lPresets.IndexOf(pr)
            Me.m_lPresets.Remove(pr)
            Me.UpdatePresetsDropdown(Me.m_lPresets(i - 1))

        End Sub

        Public Delegate Sub ProcessChecksDelegate(iGroup As Integer, bGroupChecked As Boolean, iFleet As Integer)

        Private Sub OnGroupChecked(sender As Object, e As System.Windows.Forms.ItemCheckEventArgs) _
            Handles m_clbGroups.ItemCheck

            ' Abort if triggered by a sync call
            If Me.m_bInSync Then Return

            ' Delay invoke until check state has been processed
            Me.BeginInvoke(New ProcessChecksDelegate(AddressOf Me.ProcessChecks), New Object() {e.Index + 1, e.NewValue = CheckState.Checked, 0})
        End Sub

        Private Sub OnFleetChecked(sender As Object, e As System.Windows.Forms.ItemCheckEventArgs) _
            Handles m_clbFleets.ItemCheck

            ' Abort if triggered by a sync call
            If Me.m_bInSync Then Return
            ' Delay invoke until check state has been processed
            Me.BeginInvoke(New ProcessChecksDelegate(AddressOf Me.ProcessChecks), New Object() {0, False, e.Index + 1})

        End Sub

        Private Sub OnOK(sender As System.Object, e As System.EventArgs) _
            Handles OK_Button.Click

            Dim core As cCore = Me.m_uic.Core
            Dim sg As cStyleGuide = Me.m_uic.StyleGuide
            Dim pr As cPresetData = Nothing

            sg.SuspendEvents()

            ' Rebuild presets
            sg.ClearItemVisibilityPresets()
            For i As Integer = 0 To Me.m_lPresets.Count - 1
                pr = Me.m_lPresets(i)
                For iGroup As Integer = 1 To core.nGroups
                    Dim grp As cCoreGroupBase = core.EcopathGroupInputs(iGroup)
                    sg.GroupVisible(iGroup, pr.Name) = pr.GroupVisible(iGroup - 1)
                Next

                For iFleet As Integer = 1 To core.nFleets
                    Dim flt As cEcopathFleetInput = core.EcopathFleetInputs(iFleet)
                    sg.FleetVisible(iFleet, pr.Name) = pr.FleetVisible(iFleet - 1)
                Next
            Next

            ' Select the current preset in the StyleGuide
            pr = DirectCast(Me.m_cmbPresets.SelectedItem, cPresetData)
            sg.SelectedItemVisibilityPresetName = pr.Name

            sg.ResumeEvents()
            sg.ItemVisibilityChanged()

            ' And done
            Me.DialogResult = System.Windows.Forms.DialogResult.OK
            Me.Close()

        End Sub

        Private Sub OnCancel(sender As System.Object, e As System.EventArgs) _
            Handles Cancel_Button.Click
            Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
            Me.Close()
        End Sub

        Private Sub OnSelectAllGroups(sender As System.Object, e As System.EventArgs) _
            Handles m_btnAllGroups.Click

            ' Check all items
            Me.m_clbGroups.SuspendLayout()
            For iItem As Integer = 0 To Me.m_clbGroups.Items.Count - 1
                Me.m_clbGroups.SetItemChecked(iItem, True)
            Next
            Me.m_clbGroups.ResumeLayout()
            Me.SyncCatchingFleets()

        End Sub

        Private Sub OnSelectNoneGroups(sender As System.Object, e As System.EventArgs) _
            Handles m_btnNoneGroups.Click

            ' Uncheck all items
            Me.m_clbGroups.SuspendLayout()
            For iItem As Integer = 0 To Me.m_clbGroups.Items.Count - 1
                Me.m_clbGroups.SetItemChecked(iItem, False)
            Next
            Me.m_clbGroups.ResumeLayout()
            Me.UpdateControls()

        End Sub

        Private Sub OnSelectProducers(sender As System.Object, e As System.EventArgs) _
            Handles m_btnProducers.Click

            Dim grp As cCoreGroupBase = Nothing

            Me.m_clbGroups.SuspendLayout()
            For i As Integer = 0 To Me.m_clbGroups.Items.Count - 1
                grp = Me.GroupAt(i)
                If (grp IsNot Nothing) Then
                    If (grp.IsProducer) Then
                        Me.m_clbGroups.SetItemChecked(i, True)
                        Me.SyncPredation(grp.Index)
                    End If
                End If
            Next
            Me.m_clbGroups.ResumeLayout()
            Me.SyncCatchingFleets()

        End Sub

        Private Sub OnSelectConsumers(sender As System.Object, e As System.EventArgs) _
            Handles m_btnConsumers.Click

            Dim grp As cCoreGroupBase = Nothing

            Me.m_clbGroups.SuspendLayout()
            For i As Integer = 0 To Me.m_clbGroups.Items.Count - 1
                grp = Me.GroupAt(i)
                If (grp IsNot Nothing) Then
                    If (grp.IsConsumer) Then
                        Me.m_clbGroups.SetItemChecked(i, True)
                        Me.SyncPredation(grp.Index)
                    End If
                End If
            Next
            Me.m_clbGroups.ResumeLayout()
            Me.SyncCatchingFleets()

        End Sub

        Private Sub OnSelectDetritus(sender As System.Object, e As System.EventArgs) _
            Handles m_btnNonLiving.Click

            Dim grp As cCoreGroupBase = Nothing

            Me.m_clbGroups.SuspendLayout()
            For i As Integer = 0 To Me.m_clbGroups.Items.Count - 1
                grp = Me.GroupAt(i)
                If (grp IsNot Nothing) Then
                    If (grp IsNot Nothing) Then
                        If (grp.IsDetritus) Then
                            Me.m_clbGroups.SetItemChecked(i, True)
                            Me.SyncPredation(grp.Index)
                        End If
                    End If
                End If
            Next
            Me.m_clbGroups.ResumeLayout()
            Me.SyncCatchingFleets()

        End Sub

        Private Sub OnSelectLiving(sender As System.Object, e As System.EventArgs) _
            Handles m_btnLiving.Click

            Dim grp As cCoreGroupBase = Nothing

            Me.m_clbGroups.SuspendLayout()
            For i As Integer = 0 To Me.m_clbGroups.Items.Count - 1
                grp = Me.GroupAt(i)
                If (grp IsNot Nothing) Then
                    Me.m_clbGroups.SetItemChecked(i, grp.IsLiving Or Me.m_clbGroups.GetItemChecked(i))
                End If
            Next
            Me.m_clbGroups.ResumeLayout()
            Me.SyncCatchingFleets()

        End Sub

        Private Sub OnSelectFished(sender As System.Object, e As System.EventArgs) _
            Handles m_btnFished.Click

            Dim core As cCore = Me.m_uic.Core
            Dim asIsFished(core.nGroups) As Boolean

            For iFleet As Integer = 1 To core.nFleets
                Dim fleet As cEcopathFleetInput = core.EcopathFleetInputs(iFleet)
                For iGroup As Integer = 1 To core.nGroups
                    asIsFished(iGroup) = asIsFished(iGroup) Or ((fleet.Landings(iGroup) > 0) Or (fleet.Discards(iGroup) > 0))
                Next
            Next

            Dim grp As cCoreGroupBase = Nothing

            Me.m_clbGroups.SuspendLayout()
            For i As Integer = 0 To Me.m_clbGroups.Items.Count - 1
                grp = Me.GroupAt(i)
                If (grp IsNot Nothing) Then
                    If (asIsFished(grp.Index)) Then
                        Me.m_clbGroups.SetItemChecked(i, True)
                        Me.SyncPredation(grp.Index)
                    End If
                End If
            Next

            Me.m_clbGroups.ResumeLayout()
            Me.SyncCatchingFleets()

        End Sub

        Private Sub OnSelectNonFished(sender As System.Object, e As System.EventArgs) _
            Handles m_btnNonFished.Click

            Dim core As cCore = Me.m_uic.Core
            Dim IsFished(core.nGroups) As Boolean

            For iFleet As Integer = 1 To core.nFleets
                Dim fleet As cEcopathFleetInput = core.EcopathFleetInputs(iFleet)
                For iGroup As Integer = 1 To core.nGroups
                    IsFished(iGroup) = IsFished(iGroup) Or ((fleet.Landings(iGroup) > 0) Or (fleet.Discards(iGroup) > 0))
                Next
            Next

            Dim grp As cCoreGroupBase = Nothing

            Me.m_clbGroups.SuspendLayout()
            For i As Integer = 0 To Me.m_clbGroups.Items.Count - 1
                grp = Me.GroupAt(i)
                If (grp IsNot Nothing) Then
                    If (Not IsFished(grp.Index)) Then
                        Me.m_clbGroups.SetItemChecked(i, True)
                        Me.SyncPredation(grp.Index)
                    End If
                End If
            Next
            Me.m_clbGroups.ResumeLayout()
            Me.SyncCatchingFleets()

        End Sub

        Private Sub OnSelectStanza(sender As System.Object, e As System.EventArgs) _
            Handles m_btnStanza.Click

            Dim grp As cCoreGroupBase = Nothing

            Me.m_clbGroups.SuspendLayout()
            For i As Integer = 0 To Me.m_clbGroups.Items.Count - 1
                grp = Me.GroupAt(i)
                If (grp IsNot Nothing) Then
                    If (grp.IsMultiStanza) Then
                        Me.m_clbGroups.SetItemChecked(i, True)
                        Me.SyncPredation(grp.Index)
                    End If
                End If
            Next
            Me.m_clbGroups.ResumeLayout()
            Me.SyncCatchingFleets()

        End Sub

        Private Sub OnSelectNonStanza(sender As System.Object, e As System.EventArgs) _
            Handles m_btnNonStanza.Click

            Dim grp As cCoreGroupBase = Nothing

            Me.m_clbGroups.SuspendLayout()
            For i As Integer = 0 To Me.m_clbGroups.Items.Count - 1
                grp = Me.GroupAt(i)
                If (grp IsNot Nothing) Then
                    If (Not grp.IsMultiStanza) Then
                        Me.m_clbGroups.SetItemChecked(i, True)
                        Me.SyncPredation(grp.Index)
                    End If
                End If
            Next
            Me.m_clbGroups.ResumeLayout()
            Me.SyncCatchingFleets()

        End Sub

        Private Sub OnSelectAllFleets(sender As System.Object, e As System.EventArgs) _
            Handles m_btnAllFleets.Click

            ' Check all items
            Me.m_clbFleets.SuspendLayout()
            For iItem As Integer = 0 To Me.m_clbFleets.Items.Count - 1
                Me.m_clbFleets.SetItemChecked(iItem, True)
            Next
            Me.m_clbFleets.ResumeLayout()
            Me.SyncLandedGroups()

        End Sub

        Private Sub OnSelectNoneFleets(sender As System.Object, e As System.EventArgs) _
            Handles m_btnNoneFleets.Click

            ' Uncheck all items
            Me.m_clbFleets.SuspendLayout()
            For iItem As Integer = 0 To Me.m_clbFleets.Items.Count - 1
                Me.m_clbFleets.SetItemChecked(iItem, False)
            Next
            Me.m_clbFleets.ResumeLayout()
            Me.SyncLandedGroups()

        End Sub

#End Region ' Events

#Region " Internals "

        Private Sub UpdatePresetsDropdown(sel As cPresetData)

            Me.m_bInSync = True
            Me.m_cmbPresets.Items.Clear()
            For i As Integer = 0 To Me.m_lPresets.Count - 1
                Me.m_cmbPresets.Items.Add(Me.m_lPresets(i))
            Next
            Me.m_cmbPresets.SelectedItem = sel
            Me.m_bInSync = False

        End Sub

        Private Sub LoadPreset()

            Dim pr As cPresetData = DirectCast(Me.m_cmbPresets.SelectedItem, cPresetData)
            Dim core As cCore = Me.m_uic.Core

            Debug.Assert(pr IsNot Nothing)

            Me.m_bInSync = True
            For iGroup As Integer = 0 To core.nGroups - 1
                Me.m_clbGroups.SetItemChecked(iGroup, pr.GroupVisible(iGroup))
            Next
            For iFleet As Integer = 0 To core.nFleets - 1
                Me.m_clbFleets.SetItemChecked(iFleet, pr.FleetVisible(iFleet))
            Next
            Me.m_bInSync = False

        End Sub

        Private Sub CommitChecks()

            Dim pr As cPresetData = DirectCast(Me.m_cmbPresets.SelectedItem, cPresetData)
            Dim core As cCore = Me.m_uic.Core

            Debug.Assert(pr IsNot Nothing)

            For iGroup As Integer = 0 To core.nGroups - 1
                pr.GroupVisible(iGroup) = Me.m_clbGroups.GetItemChecked(iGroup)
            Next

            For iFleet As Integer = 0 To core.nFleets - 1
                pr.FleetVisible(iFleet) = Me.m_clbFleets.GetItemChecked(iFleet)
            Next

        End Sub

        Private Sub UpdateControls()

            Dim sg As cStyleGuide = Me.m_uic.StyleGuide
            Dim strLabel As String = ""
            Dim i As Integer = 0
            Dim j As Integer = 0

            i = Me.m_clbGroups.CheckedItems().Count
            j = Me.m_clbGroups.Items.Count

            If (j <= 0) Then
                strLabel = SharedResources.HEADER_GROUPS
            Else
                strLabel = cStringUtils.Localize(SharedResources.GENERIC_LABEL_DETAILED,
                                                 SharedResources.HEADER_GROUPS,
                                                 cStringUtils.Localize(SharedResources.GENERIC_LABEL_N_OF_M, i, j))
            End If
            Me.m_hdrGroups.Text = strLabel

            i = Me.m_clbFleets.CheckedItems().Count
            j = Me.m_clbFleets.Items.Count

            If (j <= 0) Then
                strLabel = SharedResources.HEADER_FLEETS
            Else
                strLabel = cStringUtils.Localize(SharedResources.GENERIC_LABEL_DETAILED,
                                                 SharedResources.HEADER_FLEETS,
                                                 cStringUtils.Localize(SharedResources.GENERIC_LABEL_N_OF_M, i, j))
            End If
            Me.m_hdrFleets.Text = strLabel

            Dim pr As cPresetData = DirectCast(Me.m_cmbPresets.SelectedItem, cPresetData)
            Dim bValidName As Boolean = Not String.IsNullOrWhiteSpace(Me.m_tbxName.Text)
            Me.m_btnAdd.Enabled = bValidName
            Me.m_btnRename.Enabled = bValidName
            Me.m_btnDelete.Enabled = Not pr.IsDefault

        End Sub

        Private Sub ProcessChecks(iGroup As Integer, bGroupChecked As Boolean, iFleet As Integer)

            If Me.m_bInSync Then Return
            Me.m_bInSync = True

            If (iGroup > 0) Then
                Me.SyncCatchingFleets()
                If (bGroupChecked) Then Me.SyncPredation(iGroup)
            End If

            If (iFleet > 0) Then
                Me.SyncLandedGroups()
            End If

            Me.m_bInSync = False

            Me.CommitChecks()
            Me.UpdateControls()

        End Sub

        Private Sub SyncCatchingFleets()

            ' Bail-out
            If (Not Me.m_cbSyncViaFishing.Checked) Then Return

            Dim core As cCore = Me.m_uic.Core
            Dim bIsLinked(core.nFleets) As Boolean

            For iFleet As Integer = 1 To core.nFleets
                Dim fleet As cEcopathFleetInput = core.EcopathFleetInputs(iFleet)
                For i As Integer = 0 To Me.m_clbGroups.Items.Count - 1
                    If Me.m_clbGroups.GetItemChecked(i) Then
                        Dim grp As cCoreGroupBase = Me.GroupAt(i)
                        If (grp IsNot Nothing) Then
                            Dim iGroup As Integer = grp.Index
                            bIsLinked(iFleet) = bIsLinked(iFleet) Or ((fleet.Landings(iGroup) > 0) Or (fleet.Discards(iGroup) > 0))
                        End If
                    End If
                Next
            Next

            Me.m_clbFleets.SuspendLayout()

            For iFleet As Integer = 1 To core.nFleets
                Me.m_clbFleets.SetItemChecked(iFleet - 1, bIsLinked(iFleet))
            Next
            Me.m_clbFleets.ResumeLayout()

        End Sub

        Private Sub SyncLandedGroups()

            ' Bail-out
            If (Not Me.m_cbSyncViaFishing.Checked) Then Return

            Dim core As cCore = Me.m_uic.Core
            Dim bIsLinked(core.nGroups) As Boolean
            For iFleet As Integer = 1 To core.nFleets
                Dim fleet As cEcopathFleetInput = core.EcopathFleetInputs(iFleet)
                If Me.m_clbFleets.GetItemChecked(iFleet - 1) Then
                    For i As Integer = 0 To Me.m_clbGroups.Items.Count - 1
                        Dim grp As cCoreGroupBase = Me.GroupAt(i)
                        If (grp IsNot Nothing) Then
                            Dim iGroup As Integer = grp.Index
                            bIsLinked(iGroup) = bIsLinked(iGroup) Or ((fleet.Landings(iGroup) > 0) Or (fleet.Discards(iGroup) > 0))
                        End If
                    Next
                End If
            Next

            Me.m_clbGroups.SuspendLayout()

            For iGroup As Integer = 1 To core.nGroups
                Me.m_clbGroups.SetItemChecked(iGroup - 1, bIsLinked(iGroup))
            Next

            Me.m_clbGroups.ResumeLayout()

        End Sub

        Private Sub SyncPredation(iGroup As Integer)

            Dim core As cCore = Me.m_uic.Core
            Dim grp As cEcoPathGroupInput = core.EcopathGroupInputs(iGroup)

            Me.m_clbGroups.SuspendLayout()
            For iGroupTest As Integer = 1 To core.nGroups
                If ((grp.IsPred(iGroupTest) And Me.m_cbSyncPredators.Checked) Or (grp.IsPrey(iGroupTest) And Me.m_cbSyncPrey.Checked)) Then
                    Me.m_clbGroups.SetItemChecked(iGroupTest - 1, True)
                End If
            Next
            Me.m_clbGroups.ResumeLayout()

        End Sub

        Private Function GroupAt(i As Integer) As cCoreGroupBase
            Dim item As Object = Me.m_clbGroups.Items(i)
            If Not TypeOf item Is cCoreInputOutputControlItem Then Return Nothing
            Dim cci As cCoreInputOutputControlItem = DirectCast(item, cCoreInputOutputControlItem)
            If Not TypeOf cci.Source Is cCoreGroupBase Then Return Nothing
            Return DirectCast(cci.Source, cCoreGroupBase)
        End Function


#End Region ' Internals

    End Class

End Namespace
