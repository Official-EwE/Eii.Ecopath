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

Public Class lstCreateMember

    Public Sub New()

        ' This call is required by the Windows Form Designer.
        Me.InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Me.btnAddSelected.Enabled = False
        Me.btnAddAll.Enabled = False
        Me.btnRemoveSelected.Enabled = False
        Me.btnRemoveAll.Enabled = False

    End Sub

    Public Sub AddToNonMember(ByRef InputArray() As String)
        For Each i In InputArray
            Me.lstNonMember.Items.Add(i)
        Next
    End Sub

    Private Sub btnAddSelected_Click(sender As System.Object, e As System.EventArgs) Handles btnAddSelected.Click

        'Change the state of buttons
        Me.btnRemoveSelected.Enabled = True
        Me.btnRemoveAll.Enabled = True

        'Add item to member list
        Me.lstMember.Items.Add(Me.lstNonMember.SelectedItem)

        'If number in non-member list is only 1 remove and set index to -1
        If Me.lstNonMember.Items.Count = 1 Then
            Me.lstNonMember.Items.Remove(Me.lstNonMember.SelectedItem)
            Me.lstNonMember.SelectedIndex = -1

            'If top of list remove and set in index to count-1
        ElseIf Me.lstNonMember.SelectedIndex = Me.lstNonMember.Items.Count Then
            Me.lstNonMember.Items.Remove(Me.lstNonMember.SelectedItem)
            Me.lstNonMember.SelectedIndex = Me.lstNonMember.Items.Count

            'if bottom of list then remove and set index to 0
        ElseIf Me.lstNonMember.SelectedIndex = 0 Then
            Me.lstNonMember.Items.Remove(Me.lstNonMember.SelectedItem)
            Me.lstNonMember.SelectedIndex = 0

            ' must be in middle of list so move index up and remove item beneath
        Else
            Me.lstNonMember.SelectedIndex = Me.lstNonMember.SelectedIndex + 1
            Me.lstNonMember.Items.RemoveAt(Me.lstNonMember.SelectedIndex - 1)
        End If

    End Sub

    Private Sub btnAddAll_Click(sender As System.Object, e As System.EventArgs) Handles btnAddAll.Click

        'Move each individually from non-member list to member list
        While Me.lstNonMember.Items.Count > 0
            Me.lstMember.Items.Add(Me.lstNonMember.Items(0))
            Me.lstNonMember.Items.RemoveAt(0)
        End While

        'if needed modify button states
        Me.SetAddRemoveState()

    End Sub

    Private Sub btnRemoveSelected_Click(sender As System.Object, e As System.EventArgs) Handles btnRemoveSelected.Click

        'Change the state of buttons
        Me.btnAddSelected.Enabled = True
        Me.btnAddAll.Enabled = True

        'Add item to non-member list
        Me.lstNonMember.Items.Add(Me.lstMember.SelectedItem)

        'If number in member list is only 1 remove and set index to -1
        If Me.lstMember.Items.Count = 1 Then
            Me.lstMember.Items.Remove(Me.lstMember.SelectedItem)
            Me.lstMember.SelectedIndex = -1

            'If top of list remove and set in index to count-1
        ElseIf Me.lstMember.SelectedIndex = Me.lstMember.Items.Count Then
            Me.lstMember.Items.Remove(Me.lstMember.SelectedItem)
            Me.lstMember.SelectedIndex = Me.lstMember.Items.Count

            'if bottom of list then remove and set index to 0
        ElseIf Me.lstMember.SelectedIndex = 0 Then
            Me.lstMember.Items.Remove(Me.lstMember.SelectedItem)
            Me.lstMember.SelectedIndex = 0

            ' must be in middle of list so move index up and remove item beneath
        Else
            Me.lstMember.SelectedIndex = Me.lstMember.SelectedIndex + 1
            Me.lstMember.Items.RemoveAt(Me.lstMember.SelectedIndex - 1)
        End If

    End Sub

    Private Sub btnRemoveAll_Click(sender As System.Object, e As System.EventArgs) Handles btnRemoveAll.Click

        'Move each individually from non-member list to member list
        While Me.lstMember.Items.Count > 0
            Me.lstNonMember.Items.Add(Me.lstMember.Items(0))
            Me.lstMember.Items.RemoveAt(0)
        End While

        'if needed modify button states
        Me.SetAddRemoveState()

    End Sub

    Private Sub SetAddRemoveState()

        'For adding buttons
        If Me.lstNonMember.Items.Count = 0 Then
            Me.btnAddSelected.Enabled = False
            Me.btnAddAll.Enabled = False
        ElseIf Me.lstNonMember.Items.Count > 0 Then
            Me.btnAddSelected.Enabled = True
            Me.btnAddAll.Enabled = True
        End If

        'For removing buttons
        If Me.lstMember.Items.Count = 0 Then
            Me.btnRemoveSelected.Enabled = False
            Me.btnRemoveAll.Enabled = False
        ElseIf Me.lstNonMember.Items.Count > 0 Then
            Me.btnRemoveSelected.Enabled = True
            Me.btnRemoveAll.Enabled = True
        End If

    End Sub

End Class
