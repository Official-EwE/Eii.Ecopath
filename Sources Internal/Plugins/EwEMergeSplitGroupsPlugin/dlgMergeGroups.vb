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
Imports System.Windows.Forms
Imports EwECore
Imports ScientificInterfaceShared.Controls

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
            Me.m_cmbTarget.Items.Add(core.EcoPathGroupInputs(i))
        Next

        Me.UpdateControls()

    End Sub

    Protected Overrides Sub OnFormClosed(e As System.Windows.Forms.FormClosedEventArgs)
        MyBase.OnFormClosed(e)
    End Sub

#End Region ' Overrides 

#Region " Events "

    Private Sub OnFormatGroupItem(sender As Object, e As System.Windows.Forms.ListControlConvertEventArgs) _
        Handles m_cmbTarget.Format, m_clbGroups.Format

        Try
            Dim fmt As New ScientificInterfaceShared.Style.cCoreInterfaceFormatter()
            Dim grp As cCoreGroupBase = DirectCast(e.ListItem, cCoreGroupBase)

            If (Not grp.Disposed) Then
                e.Value = fmt.GetDescriptor(e.ListItem)
            End If
        Catch ex As Exception
            ' mmm
        End Try

    End Sub

    Private Sub OnTargetSelected(sender As System.Object, e As System.EventArgs) _
        Handles m_cmbTarget.SelectedIndexChanged

        Dim core As cCore = Me.m_uic.Core
        Dim iTarget As Integer = Me.SelectedTarget()
        Dim grps As Integer() = Me.m_engine.CompatibleGroups(iTarget)

        Me.m_clbGroups.Items.Clear()
        For i As Integer = 0 To grps.Count - 1
            Me.m_clbGroups.Items.Add(Me.m_uic.Core.EcoPathGroupInputs(grps(i)))
        Next

        If (iTarget > 0) Then
            Me.m_tbxNewName.Text = Me.m_uic.Core.EcoPathGroupInputs(iTarget).Name
        End If

        Me.UpdateControls()

    End Sub


    Private Sub OnNameChanged(sender As System.Object, e As System.EventArgs) _
        Handles m_tbxNewName.TextChanged

        Me.UpdateControls()

    End Sub

    Private Sub OnGroupCheck(sender As Object, e As ItemCheckEventArgs) _
        Handles m_clbGroups.ItemCheck

        ' Lazy update when item check is complete
        BeginInvoke(New MethodInvoker(AddressOf UpdateControls))

    End Sub

    Private Sub OnOK(sender As System.Object, e As System.EventArgs) _
        Handles m_btnOK.Click

        If (Not Me.m_engine.Merge(Me.SelectedGroups(), Me.m_tbxNewName.Text)) Then
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

    Private Function SelectedTarget() As Integer

        Dim item As Object = Me.m_cmbTarget.SelectedItem

        If (item Is Nothing) Then Return cCore.NULL_VALUE
        If (Not TypeOf (item) Is cCoreGroupBase) Then Return cCore.NULL_VALUE
        Return DirectCast(item, cCoreGroupBase).Index

    End Function

    Private Function SelectedGroups() As Integer()

        Dim lgroups As New List(Of Integer)
        Dim iTarget As Integer = Me.SelectedTarget

        If (iTarget > 0) Then lgroups.Add(iTarget)

        For Each item As Object In Me.m_clbGroups.CheckedItems
            Dim group As cCoreGroupBase = DirectCast(item, cCoreGroupBase)
            If Not lgroups.Contains(group.Index) Then lgroups.Add(group.Index)
        Next
        Return lgroups.ToArray()

    End Function

    Private Sub UpdateControls()

        Dim iTarget As Integer = Me.SelectedTarget()
        Dim bCanMerge As Boolean = Me.m_engine.CanMergeGroups(SelectedGroups(), Me.m_tbxNewName.Text) And (iTarget > 0)

        Me.m_clbGroups.Enabled = (iTarget > 0)
        Me.m_btnOK.Enabled = bCanMerge

    End Sub

#End Region ' Internals

End Class