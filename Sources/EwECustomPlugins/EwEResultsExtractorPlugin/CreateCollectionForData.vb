' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Option Strict Off
Imports EwECore

Public MustInherit Class CreateCollectionForData

    Private ObjectUnderFocus As cCreatedObjects

    Protected Property Core As cCore
    Protected Property SelectionData As cSelectionData

    ''' <summary>
    ''' For the designer only
    ''' </summary>
    Public Sub New()
        Me.InitializeComponent()
    End Sub

    Public Sub New(SelectionData As cSelectionData, Core As cCore)

        Me.InitializeComponent()

        Me.Core = Core
        Me.SelectionData = SelectionData

    End Sub

    Protected Overrides Sub OnLoad(e As EventArgs)
        MyBase.OnLoad(e)

        If (Me.Core Is Nothing) Then Return

        'Load group names into selected & unselected lists
        For Each x In Me.SelectionData.UnSelectedNames
            Me.lstUnSelected.Items.Add(x)
        Next
        For Each x In Me.SelectionData.SelectedNames
            Me.lstSelected.Items.Add(x)
        Next

        If Me.lstSelected.Items.Count = 0 Then
            Me.btnRemoveAll.Enabled = False
            Me.btnRemoveSelected.Enabled = False
        End If
        If Me.lstUnSelected.Items.Count = 0 Then
            Me.btnAddAll.Enabled = False
            Me.btnAddSelected.Enabled = False
        End If

    End Sub

    ''Don't use this now because unpredictable - seemed to conflict with SelectedIndexChanged
    'Private Sub lstSelected_MouseDoubleClick(sender As Object, e As System.Windows.Forms.MouseEventArgs) Handles lstSelected.MouseDoubleClick

    '    'Dim IndexSaved As Integer

    '    ''http://msdn.microsoft.com/en-us/library/kfw3x8dc.aspx
    '    ''Prevents anything from happening when white space double clicked at bottom of listbox
    '    'If lstSelected.IndexFromPoint(e.Location) = -1 Then Exit Sub

    '    ''Get index of selected item
    '    'IndexSaved = lstSelected.SelectedIndex

    '    ''Add selected back to unselected
    '    'lstUnSelected.Items.Add(lstSelected.SelectedItem)
    '    'lstSelected.SelectedIndex = lstSelected.Items.Count - 1

    '    ''Remove from selection object
    '    'm_SelectionData.Remove(lstSelected.SelectedItem.ToString)

    '    ''Remove from selected
    '    'lstSelected.Items.RemoveAt(IndexSaved)

    '    ''If selection at top of list select 1 less else select same index as began with
    '    'If lstSelected.Items.Count = IndexSaved Or IndexSaved = 0 Then
    '    '    lstSelected.SelectedIndex = IndexSaved - 1
    '    'Else
    '    '    lstSelected.SelectedIndex = IndexSaved
    '    'End If

    '    'SetStateAddRemove()

    'End Sub

    Private Sub lstSelected_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles lstSelected.SelectedIndexChanged

        If Me.lstSelected.SelectedIndex = -1 Then
            Me.SelectionData.SetFocus = Nothing
            Me.chklstAttached.Items.Clear()
            Exit Sub
        End If

        Me.PopulateAttachedList(Me.lstSelected.SelectedItem.ToString)
        Me.SelectionData.SetFocus = Me.lstSelected.SelectedItem.ToString

        ' Ticks childs that are part of current parent
        For x = 0 To Me.chklstAttached.Items.Count - 1
            For Each i In CType(Me.SelectionData.GetFocus, cCreatedObjects).ChildNames
                If Me.chklstAttached.Items(x).ToString = i.ToString Then
                    Me.chklstAttached.SetItemChecked(x, True)
                End If
            Next
        Next

    End Sub

    Public MustOverride Sub PopulateAttachedList(i As String)

    Private Sub btnAddSelected_Click(sender As System.Object, e As System.EventArgs) Handles btnAddSelected.Click
        Dim PositionSelectedPredator As Integer

        If Me.lstUnSelected.SelectedIndex = -1 Then Exit Sub

        'Buttons to remove are now enabled
        Me.btnRemoveSelected.Enabled = True
        Me.btnRemoveAll.Enabled = True

        ' Add to virtual object current selection
        Me.SelectionData.Add(Me.lstUnSelected.SelectedItem.ToString)

        ' Remove from 1st list box and add to 2nd
        Me.lstSelected.Items.Add(Me.lstUnSelected.SelectedItem)
        Me.lstSelected.SelectedIndex = Me.lstSelected.Items.Count - 1
        PositionSelectedPredator = Me.lstUnSelected.SelectedIndex
        Me.lstUnSelected.Items.Remove(Me.lstUnSelected.SelectedItem)

        'Depending on position of selection and number of items in list select next item
        If PositionSelectedPredator = Me.lstUnSelected.Items.Count Then
            Me.lstUnSelected.SelectedIndex = PositionSelectedPredator - 1
        ElseIf PositionSelectedPredator > 0 Then
            Me.lstUnSelected.SelectedIndex = PositionSelectedPredator - 1
        ElseIf Me.lstUnSelected.Items.Count > 0 Then
            Me.lstUnSelected.SelectedIndex = 0
        End If

        Me.SetStateAddRemove()

    End Sub

    Private Sub SetStateAddRemove()

        Me.btnAddSelected.Enabled = False
        Me.btnAddAll.Enabled = False
        Me.btnRemoveSelected.Enabled = False
        Me.btnRemoveAll.Enabled = False

        If Me.lstUnSelected.Items.Count > 0 Then
            Me.btnAddSelected.Enabled = True
            Me.btnAddAll.Enabled = True
        End If
        If Me.lstSelected.Items.Count > 0 Then
            Me.btnRemoveSelected.Enabled = True
            Me.btnRemoveAll.Enabled = True
        End If

    End Sub

    Private Sub btnAddAll_Click(sender As System.Object, e As System.EventArgs) Handles btnAddAll.Click

        While Me.lstUnSelected.Items.Count > 0
            Me.SelectionData.Add(Me.lstUnSelected.Items(0).ToString)
            Me.lstSelected.Items.Add(Me.lstUnSelected.Items(0))
            Me.lstSelected.SelectedIndex = Me.lstSelected.Items.Count - 1
            Me.lstUnSelected.Items.RemoveAt(0)
        End While

        Me.SetStateAddRemove()

    End Sub

    Private Sub btnRemoveSelected_Click(sender As System.Object, e As System.EventArgs) Handles btnRemoveSelected.Click

        Dim PositionSelectedPredator As Integer

        If Me.lstSelected.SelectedIndex = -1 Then Exit Sub

        'Buttons to remove are now enabled
        Me.btnAddSelected.Enabled = True
        Me.btnAddAll.Enabled = True

        ' Remove in virtual object current selection
        Me.SelectionData.Remove(Me.lstSelected.SelectedItem.ToString)

        ' Remove from 1st list box and add to 2nd
        Me.lstUnSelected.Items.Add(Me.lstSelected.SelectedItem)
        Me.lstUnSelected.SelectedIndex = Me.lstUnSelected.Items.Count - 1
        PositionSelectedPredator = Me.lstSelected.SelectedIndex
        Me.lstSelected.Items.Remove(Me.lstSelected.SelectedItem)

        'Depending on position of selection and number of items in list select next item
        If PositionSelectedPredator = Me.lstSelected.Items.Count Then
            Me.lstSelected.SelectedIndex = PositionSelectedPredator - 1
        ElseIf PositionSelectedPredator > 0 Then
            Me.lstSelected.SelectedIndex = PositionSelectedPredator - 1
        ElseIf Me.lstUnSelected.Items.Count > 0 Then
            Me.lstSelected.SelectedIndex = 0
        End If

        Me.SetStateAddRemove()

    End Sub

    Private Sub btnRemoveAll_Click(sender As System.Object, e As System.EventArgs) Handles btnRemoveAll.Click

        While Me.lstSelected.Items.Count > 0
            Me.SelectionData.Remove(Me.lstSelected.Items(0).ToString)
            Me.lstUnSelected.Items.Add(Me.lstSelected.Items(0))
            Me.lstUnSelected.SelectedIndex = Me.lstUnSelected.Items.Count - 1
            Me.lstSelected.Items.RemoveAt(0)
        End While

        Me.SetStateAddRemove()

    End Sub

    Private Sub lstUnSelected_MouseDoubleClick(sender As Object, e As System.Windows.Forms.MouseEventArgs) Handles lstUnSelected.MouseDoubleClick
        Dim IndexSaved As Integer

        'Remove from unselection object
        Me.SelectionData.Add(Me.lstUnSelected.SelectedItem.ToString)

        'http://msdn.microsoft.com/en-us/library/kfw3x8dc.aspx
        'Prevents anything from happening when white space double clicked at bottom of listbox
        If Me.lstUnSelected.IndexFromPoint(e.Location) = -1 Then Exit Sub

        'Get index of unselected item
        IndexSaved = Me.lstUnSelected.SelectedIndex

        'Add unselected back to selected
        Me.lstSelected.Items.Add(Me.lstUnSelected.SelectedItem)
        Me.lstUnSelected.SelectedIndex = Me.lstUnSelected.Items.Count - 1

        'Remove from unselected list
        Me.lstUnSelected.Items.RemoveAt(IndexSaved)

        'If selection at top of list select 1 less else select same index as began with
        If Me.lstUnSelected.Items.Count = IndexSaved Or IndexSaved = 0 Then
            Me.lstUnSelected.SelectedIndex = IndexSaved - 1
        Else
            Me.lstUnSelected.SelectedIndex = IndexSaved
        End If

        Me.SetStateAddRemove()

    End Sub

    Private Sub chklstAttached_ItemCheck(sender As Object, e As System.Windows.Forms.ItemCheckEventArgs) Handles chklstAttached.ItemCheck

        Dim temp As cCreatedObjects

        If e.NewValue = System.Windows.Forms.CheckState.Checked Then
            'Attach checked item to currently selected parent
            temp = Me.SelectionData.GetFocus
            temp.Add(Me.chklstAttached.Items(e.Index))
        End If

        If e.NewValue = System.Windows.Forms.CheckState.Unchecked Then
            'Attach checked item to currently selected parent
            temp = Me.SelectionData.GetFocus
            temp.Remove(Me.chklstAttached.Items(e.Index))
        End If

        'SetSaveResultsState()
    End Sub

    Private Sub btnAttachAll_Click(sender As System.Object, e As System.EventArgs) Handles btnAttachAll.Click
        For i = 0 To Me.chklstAttached.Items.Count - 1
            Me.chklstAttached.SetItemChecked(i, True)
        Next
    End Sub

    Private Sub btnAttachNone_Click(sender As System.Object, e As System.EventArgs) Handles btnAttachNone.Click
        For i = 0 To Me.chklstAttached.Items.Count - 1
            Me.chklstAttached.SetItemChecked(i, False)
        Next
    End Sub

    Private Sub btnOk_Click(sender As System.Object, e As System.EventArgs) Handles btnOk.Click
        Me.Close()
    End Sub
End Class