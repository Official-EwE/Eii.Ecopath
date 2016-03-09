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
Imports EwECore
Imports ScientificInterfaceShared.Controls
Imports System.Windows.Forms

#End Region ' Imports

Public Class dlgMergeGroups

    Private m_uic As cUIContext = Nothing
    Private m_engine As cEcopathMergeGroups = Nothing

    Public Sub New(uic As cUIContext)

        Me.m_uic = uic
        Me.m_engine = New cEcopathMergeGroups(Me.m_uic.Core)

        Me.InitializeComponent()

    End Sub

#Region " Overrides "

    Protected Overrides Sub OnLoad(e As System.EventArgs)
        MyBase.OnLoad(e)

        Dim core As cCore = Me.m_uic.Core

        For i As Integer = 1 To core.nGroups
            Me.m_cmbGroup1.Items.Add(core.EcoPathGroupInputs(i))
        Next

        Me.UpdateControls()

    End Sub

    Protected Overrides Sub OnFormClosed(e As System.Windows.Forms.FormClosedEventArgs)
        MyBase.OnFormClosed(e)
    End Sub

#End Region ' Overrides 

#Region " Events "

    Private Sub OnFormatGroupItem(sender As Object, e As System.Windows.Forms.ListControlConvertEventArgs) _
        Handles m_cmbGroup1.Format, m_cmbGroup2.Format

        Dim fmt As New ScientificInterfaceShared.Style.cCoreInterfaceFormatter()
        e.Value = fmt.GetDescriptor(e.ListItem)

    End Sub

    Private Sub OnGroup1Selected(sender As System.Object, e As System.EventArgs) _
        Handles m_cmbGroup1.SelectedIndexChanged

        Dim core As cCore = Me.m_uic.Core
        Dim grps As Integer() = Me.m_engine.CompatibleGroups(Me.SelectedGroup(Me.m_cmbGroup1))

        Me.m_cmbGroup2.Items.Clear()
        For i As Integer = 0 To grps.Count - 1
            Me.m_cmbGroup2.Items.Add(Me.m_uic.Core.EcoPathGroupInputs(grps(i)))
        Next

        If (Me.m_cmbGroup2.Items.Count > 0) Then
            Me.m_cmbGroup2.SelectedIndex = 0
        End If

        Me.UpdateControls()

    End Sub

    Private Sub OnGroup2Selected(sender As System.Object, e As System.EventArgs) _
        Handles m_cmbGroup2.SelectedIndexChanged

        Me.m_tbxNewName.Text = Me.m_engine.GroupName(Me.SelectedGroup(Me.m_cmbGroup1), Me.SelectedGroup(Me.m_cmbGroup2))
        Me.UpdateControls()

    End Sub

    Private Sub OnNameChanged(sender As System.Object, e As System.EventArgs) _
        Handles m_tbxNewName.TextChanged

        Me.UpdateControls()

    End Sub

    Private Sub OnOK(sender As System.Object, e As System.EventArgs) _
        Handles m_btnOK.Click

        If (Not Me.m_engine.Merge(Me.SelectedGroup(Me.m_cmbGroup1), Me.SelectedGroup(Me.m_cmbGroup2), Me.m_tbxNewName.Text)) Then
            ' ToDo: some kind of message
            Return
        End If

        Me.DialogResult = Windows.Forms.DialogResult.OK
        Me.Close()

    End Sub

    Private Sub OnCancel(sender As System.Object, e As System.EventArgs) _
        Handles m_btnCancel.Click

        Me.DialogResult = Windows.Forms.DialogResult.Cancel
        Me.Close()

    End Sub

#End Region ' Events

#Region " Internals "

    Private Function SelectedGroup(cmd As ComboBox) As Integer

        Dim item As Object = cmd.SelectedItem
        If (item Is Nothing) Then Return cCore.NULL_VALUE
        If (Not TypeOf (item) Is cCoreGroupBase) Then Return cCore.NULL_VALUE
        Return DirectCast(item, cCoreGroupBase).Index

    End Function

    Private Sub UpdateControls()

        Dim i1 As Integer = Me.SelectedGroup(Me.m_cmbGroup1)
        Dim i2 As Integer = Me.SelectedGroup(Me.m_cmbGroup2)
        Dim strName As String = Me.m_tbxNewName.Text
        Dim bCanMerge As Boolean = Me.m_engine.CanMergeGroups(i1, i2, strName, False)

        Me.m_cmbGroup2.Enabled = (Me.m_cmbGroup2.Items.Count > 0)
        Me.m_btnOK.Enabled = bCanMerge

    End Sub

#End Region ' Internals

End Class