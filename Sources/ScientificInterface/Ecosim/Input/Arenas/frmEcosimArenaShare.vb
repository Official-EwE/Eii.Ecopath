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
                Me.m_groups.Attach(UIContext)
            End Set
        End Property

        Protected Overrides Sub OnLoad(e As EventArgs)
            MyBase.OnLoad(e)

            Me.m_groups.GroupListTracking = cGroupListBox.eGroupTrackingType.Manual
            Me.m_tsbnSumToOne.Image = SharedResources.CalculatorHS
            Me.m_tsbnReset.Image = SharedResources.ResetHS

            If (Me.UIContext Is Nothing) Then Return

            Dim man As cEcosimArenaManager = Me.Core.EcosimArenaManager
            Me.m_groups.VisibleGroups = man.Groups(False)
            Me.m_groups.Populate()

            ' Go
            Me.m_groups.SelectedIndex = 0

        End Sub

        Protected Overrides Sub OnFormClosed(e As FormClosedEventArgs)
            MyBase.OnFormClosed(e)
        End Sub

        Private Sub OnGroupSelected(sender As Object, e As EventArgs) Handles m_groups.SelectedIndexChanged

            Me.m_grid.SelectedGroup = Me.m_groups.SelectedGroup
            Me.m_grid.RefreshContent()

            Me.m_hdrArenas.Text = cStringUtils.Localize(My.Resources.HEADER_ECOSIM_ARENA_PREY, Me.m_groups.SelectedGroup.Name)

        End Sub

        Private Sub OnSumPreyArenaToOne(sender As Object, e As EventArgs) Handles m_tsbnSumToOne.Click
            MessageBox.Show("Not implemented yet")
        End Sub

        Private Sub OnResetPreyArena(sender As Object, e As EventArgs) Handles m_tsbnReset.Click
            MessageBox.Show("Not implemented yet")
        End Sub

    End Class

End Namespace
