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
'    UBC Fisheries Centre, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore
Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports ScientificInterfaceShared.Commands
Imports ScientificInterfaceShared.Controls

#End Region

Namespace Ecosim

    ''' =======================================================================
    ''' <summary>
    ''' Dialog class, implements the generic show/hide items interface.
    ''' </summary>
    ''' =======================================================================
    Public Class dlgShowHideItems

#Region " Private variables "

        Private m_uic As cUIContext = Nothing
        Private m_bInSync As Boolean = False
        Private m_il As ImageList = Nothing

        Private m_groupOptions As cDisplayGroupsCommand.eGroupDisplayOptions = cDisplayGroupsCommand.eGroupDisplayOptions.All

#End Region ' Private variables

#Region " Constructor "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Create a new dialog.
        ''' </summary>
        ''' <param name="uic">The UI context to connect to.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal uic As cUIContext, _
                       Optional groupOptions As cDisplayGroupsCommand.eGroupDisplayOptions = cDisplayGroupsCommand.eGroupDisplayOptions.All)
            Me.InitializeComponent()
            Debug.Assert(uic IsNot Nothing)
            Me.m_uic = uic
            Me.m_groupOptions = groupOptions
        End Sub

#End Region ' Constructor

#Region " Form overrides "

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)

            If (Me.m_uic Is Nothing) Then Return

            Dim group As cEcoPathGroupInput = Nothing
            Dim fleet As cFleetInput = Nothing
            Dim bShowGroup As Boolean = True

            Me.m_bInSync = True

            Me.m_clbGroups.Items.Clear()
            For iGroup As Integer = 1 To Me.m_uic.Core.nGroups
                group = Me.m_uic.Core.EcoPathGroupInputs(iGroup)
                If (Me.IncludeGroup(group)) Then
                    Me.m_clbGroups.Items.Add(New cCoreInputOutputControlItem(group), _
                                             Me.m_uic.StyleGuide.GroupVisible(iGroup))
                End If
            Next

            Me.m_clbFleets.Items.Clear()
            For iFleet As Integer = 1 To Me.m_uic.Core.nFleets
                fleet = Me.m_uic.Core.FleetInputs(iFleet)
                Me.m_clbFleets.Items.Add(New cCoreInputOutputControlItem(fleet), _
                                         Me.m_uic.StyleGuide.FleetVisible(iFleet))
            Next

            Me.m_bInSync = False
            Me.m_cbSyncGroupsAndFleets.Checked = My.Settings.LinkVisibleGroupsFleets

            Me.m_il = New ImageList()
            Me.m_il.Images.Add(SharedResources.fish)
            Me.m_il.Images.Add(SharedResources.fishing_gear)

        End Sub

        Protected Overrides Sub OnFormClosed(e As System.Windows.Forms.FormClosedEventArgs)
            My.Settings.LinkVisibleGroupsFleets = Me.m_cbSyncGroupsAndFleets.Checked
            My.Settings.Save()
            MyBase.OnFormClosed(e)
        End Sub

#End Region ' Form overrides

#Region " Events "

        Private Sub OnGroupChecked(sender As Object, e As System.Windows.Forms.ItemCheckEventArgs) _
            Handles m_clbGroups.ItemCheck
            ' Abort if triggered by a sync call
            If Me.m_bInSync Then Return
            ' Delay invoke until check state has been processed
            Me.BeginInvoke(New MethodInvoker(AddressOf SyncFleets), Nothing)
        End Sub

        Private Sub OnFleetChecked(sender As Object, e As System.Windows.Forms.ItemCheckEventArgs) _
            Handles m_clbFleets.ItemCheck
            ' Abort if triggered by a sync call
            If Me.m_bInSync Then Return
            ' Delay invoke until check state has been processed
            Me.BeginInvoke(New MethodInvoker(AddressOf SyncGroups), Nothing)
        End Sub

        Private Sub OnOK(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles OK_Button.Click

            Me.m_uic.StyleGuide.SuspendEvents()

            For i As Integer = 0 To Me.m_clbGroups.Items.Count - 1
                Dim grp As cCoreGroupBase = Me.GroupAt(i)
                If (grp IsNot Nothing) Then
                    Me.m_uic.StyleGuide.GroupVisible(grp.Index) = Me.m_clbGroups.GetItemChecked(i)
                End If
            Next

            For iFleet As Integer = 1 To Me.m_uic.Core.nFleets
                Me.m_uic.StyleGuide.FleetVisible(iFleet) = Me.m_clbFleets.GetItemChecked(iFleet - 1)
            Next

            Me.m_uic.StyleGuide.ResumeEvents()
            Me.m_uic.StyleGuide.ItemVisibilityChanged()

            ' And done
            Me.DialogResult = System.Windows.Forms.DialogResult.OK
            Me.Close()

        End Sub

        Private Sub OnCancel(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles Cancel_Button.Click
            Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
            Me.Close()
        End Sub

        Private Sub OnSelectAllGroups(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnAllGroups.Click

            ' Check all items
            Me.m_clbGroups.SuspendLayout()
            For iItem As Integer = 0 To Me.m_clbGroups.Items.Count - 1
                Me.m_clbGroups.SetItemChecked(iItem, True)
            Next
            Me.m_clbGroups.ResumeLayout()
            Me.SyncFleets()

        End Sub

        Private Sub OnSelectNoneGroups(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnNoneGroups.Click

            ' Uncheck all items
            Me.m_clbGroups.SuspendLayout()
            For iItem As Integer = 0 To Me.m_clbGroups.Items.Count - 1
                Me.m_clbGroups.SetItemChecked(iItem, False)
            Next
            Me.m_clbGroups.ResumeLayout()

        End Sub

        Private Sub OnSelectProducers(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnProducers.Click

            Dim grp As cCoreGroupBase = Nothing

            Me.m_clbGroups.SuspendLayout()
            For i As Integer = 0 To Me.m_clbGroups.Items.Count - 1
                grp = Me.GroupAt(i)
                If (grp IsNot Nothing) Then
                    Me.m_clbGroups.SetItemChecked(i, grp.IsProducer)
                End If
            Next
            Me.m_clbGroups.ResumeLayout()
            Me.SyncFleets()

        End Sub

        Private Sub OnSelectComsumers(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnConsumers.Click

            Dim grp As cCoreGroupBase = Nothing

            Me.m_clbGroups.SuspendLayout()
            For i As Integer = 0 To Me.m_clbGroups.Items.Count - 1
                grp = Me.GroupAt(i)
                If (grp IsNot Nothing) Then
                    Me.m_clbGroups.SetItemChecked(i, grp.IsConsumer)
                End If
            Next
            Me.m_clbGroups.ResumeLayout()
            Me.SyncFleets()

        End Sub

        Private Sub OnSelectDetritus(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnNonLiving.Click

            Dim grp As cCoreGroupBase = Nothing

            Me.m_clbGroups.SuspendLayout()
            For i As Integer = 0 To Me.m_clbGroups.Items.Count - 1
                grp = Me.GroupAt(i)
                If (grp IsNot Nothing) Then
                    Me.m_clbGroups.SetItemChecked(i, grp.IsDetritus)
                End If
            Next
            Me.m_clbGroups.ResumeLayout()
            Me.SyncFleets()

        End Sub

        Private Sub OnSelectLiving(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnLiving.Click

            Dim grp As cCoreGroupBase = Nothing

            Me.m_clbGroups.SuspendLayout()
            For i As Integer = 0 To Me.m_clbGroups.Items.Count - 1
                grp = Me.GroupAt(i)
                If (grp IsNot Nothing) Then
                    Me.m_clbGroups.SetItemChecked(i, grp.IsLiving)
                End If
            Next
            Me.m_clbGroups.ResumeLayout()
            Me.SyncFleets()

        End Sub

        Private Sub OnSelectFished(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnFished.Click

            Dim core As cCore = Me.m_uic.Core
            Dim asIsFished(core.nGroups) As Boolean

            For iFleet As Integer = 1 To core.nFleets
                Dim fleet As cFleetInput = core.FleetInputs(iFleet)
                For iGroup As Integer = 1 To core.nGroups
                    asIsFished(iGroup) = asIsFished(iGroup) Or ((fleet.Landings(iGroup) > 0) Or (fleet.Discards(iGroup) > 0))
                Next
            Next

            Dim grp As cCoreGroupBase = Nothing

            Me.m_clbGroups.SuspendLayout()
            For i As Integer = 0 To Me.m_clbGroups.Items.Count - 1
                grp = Me.GroupAt(i)
                If (grp IsNot Nothing) Then
                    Me.m_clbGroups.SetItemChecked(i, asIsFished(grp.Index))
                End If
            Next
            Me.m_clbGroups.ResumeLayout()
            Me.SyncFleets()

        End Sub

        Private Sub OnSelectNonFished(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnNonFished.Click

            Dim core As cCore = Me.m_uic.Core
            Dim asIsFished(core.nGroups) As Boolean

            For iFleet As Integer = 1 To core.nFleets
                Dim fleet As cFleetInput = core.FleetInputs(iFleet)
                For iGroup As Integer = 1 To core.nGroups
                    asIsFished(iGroup) = asIsFished(iGroup) Or ((fleet.Landings(iGroup) > 0) Or (fleet.Discards(iGroup) > 0))
                Next
            Next

            Dim grp As cCoreGroupBase = Nothing

            Me.m_clbGroups.SuspendLayout()
            For i As Integer = 0 To Me.m_clbGroups.Items.Count - 1
                grp = Me.GroupAt(i)
                If (grp IsNot Nothing) Then
                    Me.m_clbGroups.SetItemChecked(i, Not asIsFished(grp.Index))
                End If
            Next
            Me.m_clbGroups.ResumeLayout()
            Me.SyncFleets()

        End Sub

        Private Sub OnSelectStanza(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnStanza.Click

            Dim grp As cCoreGroupBase = Nothing

            Me.m_clbGroups.SuspendLayout()
            For i As Integer = 0 To Me.m_clbGroups.Items.Count - 1
                grp = Me.GroupAt(i)
                If (grp IsNot Nothing) Then
                    Me.m_clbGroups.SetItemChecked(i, grp.isMultiStanza)
                End If
            Next
            Me.m_clbGroups.ResumeLayout()
            Me.SyncFleets()

        End Sub

        Private Sub OnSelectNonStanza(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnNonStanza.Click

            Dim grp As cCoreGroupBase = Nothing

            Me.m_clbGroups.SuspendLayout()
            For i As Integer = 0 To Me.m_clbGroups.Items.Count - 1
                grp = Me.GroupAt(i)
                If (grp IsNot Nothing) Then
                    Me.m_clbGroups.SetItemChecked(i, Not grp.isMultiStanza)
                End If
            Next
            Me.m_clbGroups.ResumeLayout()
            Me.SyncFleets()

        End Sub

        Private Sub OnSelectAllFleets(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnAllFleets.Click

            ' Check all items
            Me.m_clbFleets.SuspendLayout()
            For iItem As Integer = 0 To Me.m_clbFleets.Items.Count - 1
                Me.m_clbFleets.SetItemChecked(iItem, True)
            Next
            Me.m_clbFleets.ResumeLayout()
            Me.SyncGroups()

        End Sub

        Private Sub OnSelectNoneFleets(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnNoneFleets.Click

            ' Uncheck all items
            Me.m_clbFleets.SuspendLayout()
            For iItem As Integer = 0 To Me.m_clbFleets.Items.Count - 1
                Me.m_clbFleets.SetItemChecked(iItem, False)
            Next
            Me.m_clbFleets.ResumeLayout()
            Me.SyncGroups()

        End Sub

        'Private Sub OnSyncItemsChecked(sender As System.Object, e As System.EventArgs) _
        '    Handles m_cbSyncGroupsAndFleets.CheckedChanged

        '    If Me.m_cbSyncGroupsAndFleets.Checked Then
        '        If Object.ReferenceEquals(Me.m_tcDisplayBits.SelectedTab, Me.m_tpGroups) Then
        '            Me.SyncFleets()
        '        Else
        '            Me.SyncGroups()
        '        End If
        '    End If

        'End Sub

#End Region ' Events

#Region " Internals "

        Private Sub SyncFleets()

            If (Not Me.m_cbSyncGroupsAndFleets.Checked) Then Return

            ' Bail-out
            If Me.m_bInSync Then Return
            Me.m_bInSync = True

            Dim core As cCore = Me.m_uic.Core
            Dim abLanded(core.nFleets) As Boolean
            For iFleet As Integer = 1 To core.nFleets
                Dim fleet As cFleetInput = core.FleetInputs(iFleet)
                For i As Integer = 0 To Me.m_clbGroups.Items.Count - 1
                    If Me.m_clbGroups.GetItemChecked(i) Then
                        Dim grp As cCoreGroupBase = Me.GroupAt(i)
                        If (grp IsNot Nothing) Then
                            Dim iGroup As Integer = grp.Index
                            abLanded(iFleet) = abLanded(iFleet) Or ((fleet.Landings(iGroup) > 0) Or (fleet.Discards(iGroup) > 0))
                        End If
                    End If
                Next
            Next

            Me.m_clbFleets.SuspendLayout()

            For iFleet As Integer = 1 To core.nFleets
                Me.m_clbFleets.SetItemChecked(iFleet - 1, abLanded(iFleet))
            Next

            Me.m_clbFleets.ResumeLayout()
            Me.m_bInSync = False

        End Sub

        Private Sub SyncGroups()

            If (Not Me.m_cbSyncGroupsAndFleets.Checked) Then Return

            ' Bail-out
            If Me.m_bInSync Then Return
            Me.m_bInSync = True

            Dim core As cCore = Me.m_uic.Core
            Dim abLanded(core.nGroups) As Boolean
            For iFleet As Integer = 1 To core.nFleets
                Dim fleet As cFleetInput = core.FleetInputs(iFleet)
                If Me.m_clbFleets.GetItemChecked(iFleet - 1) Then
                    For i As Integer = 0 To Me.m_clbGroups.Items.Count - 1
                        Dim grp As cCoreGroupBase = Me.GroupAt(i)
                        If (grp IsNot Nothing) Then
                            Dim iGroup As Integer = grp.Index
                            abLanded(iGroup) = abLanded(iGroup) Or ((fleet.Landings(iGroup) > 0) Or (fleet.Discards(iGroup) > 0))
                        End If
                    Next
                End If
            Next

            Me.m_clbGroups.SuspendLayout()

            For iGroup As Integer = 1 To core.nGroups
                Me.m_clbGroups.SetItemChecked(iGroup - 1, abLanded(iGroup))
            Next

            Me.m_clbGroups.ResumeLayout()
            Me.m_bInSync = False
        End Sub

        Private Function IncludeGroup(grp As cEcoPathGroupInput) As Boolean

            Dim bInclude As Boolean = False

            If (Me.m_groupOptions And cDisplayGroupsCommand.eGroupDisplayOptions.Consumers) > 0 Then bInclude = bInclude Or grp.IsConsumer
            If (Me.m_groupOptions And cDisplayGroupsCommand.eGroupDisplayOptions.Producers) > 0 Then bInclude = bInclude Or grp.IsProducer
            If (Me.m_groupOptions And cDisplayGroupsCommand.eGroupDisplayOptions.Living) > 0 Then bInclude = bInclude Or grp.IsLiving
            If (Me.m_groupOptions And cDisplayGroupsCommand.eGroupDisplayOptions.NonLiving) > 0 Then bInclude = bInclude Or (Not grp.IsLiving)
            If (Me.m_groupOptions And cDisplayGroupsCommand.eGroupDisplayOptions.Fished) > 0 Then bInclude = bInclude Or grp.IsFished
            If (Me.m_groupOptions And cDisplayGroupsCommand.eGroupDisplayOptions.NonFished) > 0 Then bInclude = bInclude Or (Not grp.IsFished)
            If (Me.m_groupOptions And cDisplayGroupsCommand.eGroupDisplayOptions.Detritus) > 0 Then bInclude = bInclude Or (grp.IsDetritus)
            If (Me.m_groupOptions And cDisplayGroupsCommand.eGroupDisplayOptions.Stanza) > 0 Then bInclude = bInclude Or (grp.isMultiStanza)
            If (Me.m_groupOptions And cDisplayGroupsCommand.eGroupDisplayOptions.NonStanza) > 0 Then bInclude = bInclude Or (Not grp.isMultiStanza)

            Return bInclude

        End Function

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
