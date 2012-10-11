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
' Copyright 1991-2012 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'
#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore
Imports SharedResources = ScientificInterfaceShared.My.Resources

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

#End Region ' Private variables

#Region " Constructor "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Create a new dialog.
        ''' </summary>
        ''' <param name="uic">The UI context to connect to.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal uic As cUIContext)
            Me.InitializeComponent()
            Debug.Assert(uic IsNot Nothing)
            Me.m_uic = uic
        End Sub

#End Region ' Constructor

#Region " Form overrides "

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)

            Dim group As cEcoPathGroupInput = Nothing
            Dim fleet As cFleetInput = Nothing

            Me.m_bInSync = True

            Me.m_clbGroups.Items.Clear()
            For iGroup As Integer = 1 To Me.m_uic.Core.nGroups
                group = Me.m_uic.Core.EcoPathGroupInputs(iGroup)
                Me.m_clbGroups.Items.Add(String.Format(SharedResources.GENERIC_LABEL_INDEXED, iGroup, group.Name), _
                                         Me.m_uic.StyleGuide.GroupVisible(iGroup))
            Next

            Me.m_clbFleets.Items.Clear()
            For iFleet As Integer = 1 To Me.m_uic.Core.nFleets
                fleet = Me.m_uic.Core.FleetInputs(iFleet)
                Me.m_clbFleets.Items.Add(String.Format(SharedResources.GENERIC_LABEL_INDEXED, iFleet, fleet.Name), _
                                         Me.m_uic.StyleGuide.FleetVisible(iFleet))
            Next

            Me.m_bInSync = False
            Me.m_cbSyncGroupsAndFleets.Checked = My.Settings.LinkVisibleGroupsFleets

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

            Dim iIndex As Integer = 0

            Me.m_uic.StyleGuide.SuspendEvents()

            For iGroup As Integer = 1 To Me.m_uic.Core.nGroups
                Me.m_uic.StyleGuide.GroupVisible(iGroup) = Me.m_clbGroups.GetItemChecked(iGroup - 1)
            Next
            iIndex += Me.m_uic.Core.nGroups

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

            Me.m_clbGroups.SuspendLayout()
            For iGroup As Integer = 1 To Me.m_uic.Core.nGroups
                Me.m_clbGroups.SetItemChecked(iGroup - 1, Me.m_uic.Core.EcoPathGroupInputs(iGroup).IsProducer)
            Next
            Me.m_clbGroups.ResumeLayout()

        End Sub

        Private Sub OnSelectComsumers(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnConsumers.Click

            Me.m_clbGroups.SuspendLayout()
            For iGroup As Integer = 1 To Me.m_uic.Core.nGroups
                Me.m_clbGroups.SetItemChecked(iGroup - 1, Me.m_uic.Core.EcoPathGroupInputs(iGroup).IsConsumer)
            Next
            Me.m_clbGroups.ResumeLayout()
            Me.SyncFleets()

        End Sub

        Private Sub OnSelectDetritus(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnNonLiving.Click

            Me.m_clbGroups.SuspendLayout()
            For iGroup As Integer = 1 To Me.m_uic.Core.nGroups
                Me.m_clbGroups.SetItemChecked(iGroup - 1, Me.m_uic.Core.EcoPathGroupInputs(iGroup).IsDetritus)
            Next
            Me.m_clbGroups.ResumeLayout()
            Me.SyncFleets()

        End Sub

        Private Sub OnSelectLiving(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnLiving.Click

            Me.m_clbGroups.SuspendLayout()
            For iGroup As Integer = 1 To Me.m_uic.Core.nGroups
                Me.m_clbGroups.SetItemChecked(iGroup - 1, iGroup <= Me.m_uic.Core.nLivingGroups)
            Next
            Me.m_clbGroups.ResumeLayout()
            Me.SyncFleets()

        End Sub

        Private Sub OnSelectFished(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnFished.Click

            Me.m_clbGroups.SuspendLayout()
            Dim core As cCore = Me.m_uic.Core
            Dim asIsFished(core.nGroups) As Boolean
            For iFleet As Integer = 1 To core.nFleets
                Dim fleet As cFleetInput = core.FleetInputs(iFleet)
                For iGroup As Integer = 1 To core.nGroups
                    asIsFished(iGroup) = asIsFished(iGroup) Or ((fleet.Landings(iGroup) > 0) Or (fleet.Discards(iGroup) > 0))
                Next
            Next
            For iGroup As Integer = 1 To Me.m_uic.Core.nGroups
                Me.m_clbGroups.SetItemChecked(iGroup - 1, asIsFished(iGroup))
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
                For iGroup As Integer = 1 To core.nGroups
                    If Me.m_clbGroups.GetItemChecked(iGroup - 1) Then
                        abLanded(iFleet) = abLanded(iFleet) Or ((fleet.Landings(iGroup) > 0) Or (fleet.Discards(iGroup) > 0))
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
                    For iGroup As Integer = 1 To core.nGroups
                        abLanded(iGroup) = abLanded(iGroup) Or ((fleet.Landings(iGroup) > 0) Or (fleet.Discards(iGroup) > 0))
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

#End Region ' Internals

    End Class

End Namespace
