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
Imports EwECore
Imports EwEUtils.Utilities
Imports EwEUtils.Core
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region ' Imports

Namespace Ecosim

    Public Class frmEcosimArenaShare

        Public Sub New()
            Me.InitializeComponent()
            Me.Grid = Me.m_grid

            Me.Text = My.Resources.LABEL_NAV_ECOSIM_INPUT_ARENA
            Me.TabText = Me.Text
        End Sub

        Public Overrides Property UIContext As cUIContext
            Get
                Return MyBase.UIContext
            End Get
            Set(value As cUIContext)
                Me.m_groups.Detach()
                MyBase.UIContext = value
                Me.m_groups.Attach(Me.UIContext)
            End Set
        End Property

        Protected Overrides Sub OnLoad(e As EventArgs)
            MyBase.OnLoad(e)

            Me.m_groups.GroupListTracking = cGroupListBox.eGroupTrackingType.Manual
            Me.m_tsbnResetAll.Image = SharedResources.ResetHS
            Me.m_tsbnResetSelected.Image = SharedResources.ResetHS

            If (Me.UIContext Is Nothing) Then Return

            Dim man As cEcosimArenaManager = Me.Core.EcosimArenaManager
            Me.m_groups.VisibleGroups = man.Groups(False)
            Me.m_groups.Populate()

            ' Go
            'jb some models can have no shared arenas
            'in that case don't select any group
            If Me.m_groups.Items.Count > 0 Then
                Me.m_groups.SelectedIndex = 0
            End If

        End Sub

        Protected Overrides Sub OnFormClosed(e As FormClosedEventArgs)
            MyBase.OnFormClosed(e)
        End Sub

        Private Sub OnGroupSelected(sender As Object, e As EventArgs) Handles m_groups.SelectedIndexChanged

            Me.m_grid.SelectedGroup = Me.m_groups.SelectedGroup
            Me.m_grid.RefreshContent()

            Me.m_hdrArenas.Text = cStringUtils.Localize(My.Resources.HEADER_ECOSIM_ARENA_PREY, Me.m_groups.SelectedGroup.Name)

        End Sub

        Private Sub OnResetAll(sender As Object, e As EventArgs) Handles m_tsbnResetAll.Click

            ' ToDo: globalize this

            Dim man As cEcosimArenaManager = Me.Core.EcosimArenaManager
            Dim fmsg As New cFeedbackMessage("This will reset all shared arenas. Are you sure that you want to do this?",
                                             eCoreComponentType.EcoSim, eMessageType.Any, eMessageImportance.Question, eMessageReplyStyle.YES_NO)
            Me.Core.Messages.SendMessage(fmsg)
            If (fmsg.Reply = eMessageReply.YES) Then
                man.ResetArenas(0)
            End If

        End Sub

        Private Sub OnResetPrey(sender As Object, e As EventArgs) Handles m_tsbnResetSelected.Click

            Dim man As cEcosimArenaManager = Me.Core.EcosimArenaManager
            Dim grp As cCoreGroupBase = Me.m_groups.SelectedGroup
            If (grp IsNot Nothing) Then
                man.ResetArenas(grp.Index)
            End If

        End Sub

    End Class

End Namespace
