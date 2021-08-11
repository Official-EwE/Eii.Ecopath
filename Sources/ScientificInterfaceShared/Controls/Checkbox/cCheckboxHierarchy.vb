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

Imports ScientificInterfaceShared.Controls.EwEGrid
Imports System.Windows.Forms

#End Region ' Imports

Namespace Controls

#Disable Warning CA1063 ' Implement IDisposable Correctly
    ''' <summary>
    ''' Management class for a hierarchy of check boxes.
    ''' </summary>
    Public Class cCheckboxHierarchy
        Implements IDisposable

#Region " Private vars "

        Private m_dtLinks As New Dictionary(Of Object, cLink)
        Private m_linkRoots As New List(Of cLink)
        Private m_bManageChecks As Boolean = False
        Private m_iLockCount As Integer = 0

#End Region ' Private vars

#Region " Checkbox links "

#Region " Foundation "

        ''' <summary>
        ''' Link in a checkbox hierarchy chain. Each link has a checkbox, an
        ''' optional parent link, and zero or more child links.
        ''' </summary>
        Private MustInherit Class cLink
            Implements IDisposable

#Region " Private vars "

            ''' <summary>Parent hierarchy.</summary>
            Protected m_hr As cCheckboxHierarchy = Nothing
            ''' <summary>Parent link in the hierarchy.</summary>
            Protected m_parent As cLink = Nothing
            ''' <summary>List of child links in the hierarchy.</summary>
            Protected m_children As New List(Of cLink)

#End Region ' Private vars

#Region " Public access "

            ''' -------------------------------------------------------------------
            ''' <summary>
            ''' Constructor
            ''' </summary>
            ''' <param name="hr">The <see cref="cCheckboxHierarchy"/> this link is 
            ''' created for.</param>
            ''' <param name="parent">An optional parent link.</param>
            ''' -------------------------------------------------------------------
            Protected Sub New(hr As cCheckboxHierarchy, parent As cLink)
                Me.m_hr = hr
                Me.m_parent = parent
            End Sub

            ''' -------------------------------------------------------------------
            ''' <summary>
            ''' Cleanup.
            ''' </summary>
            ''' -------------------------------------------------------------------
            Public Overridable Sub Dispose() Implements IDisposable.Dispose
                GC.SuppressFinalize(Me)
                Me.m_hr = Nothing
                Me.m_parent = Nothing
                Me.m_children.Clear()
            End Sub

            ''' -------------------------------------------------------------------
            ''' <summary>
            ''' Add a child link.
            ''' </summary>
            ''' <param name="child">The <see cref="cLink"/> to add as a child.</param>
            ''' -------------------------------------------------------------------
            Public Sub AddChild(child As cLink)
                Me.m_children.Add(child)
            End Sub

            ''' -------------------------------------------------------------------
            ''' <summary>
            ''' Update the checked state of this link, based on the checked state 
            ''' of all of its children.
            ''' </summary>
            ''' -------------------------------------------------------------------
            Public Overridable Sub Update()

                Dim iNumChecked As Integer = 0
                Dim iNumInterm As Integer = 0
                Dim state As CheckState = CheckState.Unchecked

                ' Only affect links with children
                If (Me.m_children.Count > 0) Then

                    ' For every child
                    For Each child As cLink In Me.m_children
                        ' Update its checked state
                        child.Update()
                        ' Count checked state of children
                        If child.Checkstate = CheckState.Checked Then iNumChecked += 1
                        If child.Checkstate = CheckState.Indeterminate Then iNumInterm += 1
                    Next

                    ' Determine checked state of this node
                    If (iNumChecked = 0) Then
                        If (iNumInterm > 0) Then state = CheckState.Indeterminate
                    ElseIf (iNumChecked > 0) And (iNumChecked < Me.m_children.Count) Then
                        state = CheckState.Indeterminate
                    Else
                        state = CheckState.Checked
                    End If

                    ' Apply state
                    Me.Checkstate = state

                End If

            End Sub

            Public MustOverride Property Checkstate() As CheckState

#End Region ' Public access

        End Class

#End Region ' Foundation

#Region " Checkbox support "

        Private Class cCheckboxLink
            Inherits cLink

            ''' <summary>Checkbox the link is created for.</summary>
            Private m_cb As CheckBox

            ''' -------------------------------------------------------------------
            ''' <summary>
            ''' Constructor
            ''' </summary>
            ''' <param name="hr">The <see cref="cCheckboxHierarchy"/> this link is 
            ''' created for.</param>
            ''' <param name="cb">The checkbox to define this link for.</param>
            ''' <param name="parent">An optional parent link.</param>
            ''' -------------------------------------------------------------------
            Public Sub New(cb As CheckBox, hr As cCheckboxHierarchy, parent As cLink)
                MyBase.New(hr, parent)
                Me.m_cb = cb
                AddHandler Me.m_cb.CheckedChanged, AddressOf OnCheckChanged
            End Sub

            Public Overrides Sub Dispose()
                If (Me.m_cb IsNot Nothing) Then
                    RemoveHandler Me.m_cb.CheckedChanged, AddressOf OnCheckChanged
                    Me.m_cb = Nothing
                End If
                MyBase.Dispose()
            End Sub

            Public Overrides Property Checkstate As CheckState
                Get
                    Return Me.m_cb.CheckState
                End Get
                Set(value As CheckState)
                    Me.m_cb.CheckState = value
                End Set
            End Property

#Region " Event handling "

            ''' -------------------------------------------------------------------
            ''' <summary>
            ''' Respond to checkbox check state changes.
            ''' </summary>
            ''' -------------------------------------------------------------------
            Private Sub OnCheckChanged(sender As Object, args As EventArgs)

                ' If allowed to dispatch checks
                If (Me.m_hr.ManageCheckedStates) Then
                    ' Engage check lock
                    Me.m_hr.BeginCheckChange()
                    ' Apply check state to all children
                    For Each linkChild As cLink In Me.m_children
                        linkChild.Checkstate = Me.Checkstate
                    Next
                    ' Release check lock
                    Me.m_hr.EndCheckChange()
                End If

            End Sub

#End Region ' Event handling

        End Class

#End Region ' Checkbox support

#Region " Sourcegrid support "

        Private Class cCheckboxCellLink
            Inherits cLink
            Implements SourceGrid2.BehaviorModels.IBehaviorModel

            ''' <summary>Checkbox the link is created for.</summary>
            Private m_cb As cEwECheckboxCell
            Private m_pos As SourceGrid2.Position

            ''' -------------------------------------------------------------------
            ''' <summary>
            ''' Constructor
            ''' </summary>
            ''' <param name="hr">The <see cref="cCheckboxHierarchy"/> this link is 
            ''' created for.</param>
            ''' <param name="cb">The <see cref="cEwECheckboxCell"/> to define this link for.</param>
            ''' <param name="parent">An optional parent link.</param>
            ''' -------------------------------------------------------------------
            Public Sub New(cb As cEwECheckboxCell, hr As cCheckboxHierarchy, parent As cLink)
                MyBase.New(hr, parent)
                Me.m_cb = cb
                Me.m_cb.Behaviors.Add(Me)
            End Sub

            Public Overrides Sub Dispose()
                If (Me.m_cb IsNot Nothing) Then
                    Me.m_cb.Behaviors.Remove(Me)
                    Me.m_cb = Nothing
                End If
                MyBase.Dispose()
            End Sub

            Public Overrides Property Checkstate As CheckState
                Get
                    Dim pos As New SourceGrid2.Position(Me.m_cb.Row, Me.m_cb.Column)
                    Select Case Me.m_cb.GetCheckedValue(pos)
                        Case True
                            Return CheckState.Checked
                        Case False
                            Return CheckState.Unchecked
                    End Select
                    Return CheckState.Indeterminate
                End Get
                Set(value As CheckState)
                    Dim pos As New SourceGrid2.Position(Me.m_cb.Row, Me.m_cb.Column)
                    Me.m_cb.SetCheckedValue(pos, (value <> CheckState.Unchecked))
                End Set
            End Property

#Region " Event handling "

            Public ReadOnly Property CanReceiveFocus As Boolean Implements SourceGrid2.BehaviorModels.IBehaviorModel.CanReceiveFocus
                Get
                    Return True
                End Get
            End Property

            Public Sub OnClick(e As SourceGrid2.PositionEventArgs) Implements SourceGrid2.BehaviorModels.IBehaviorModel.OnClick

            End Sub

            Public Sub OnContextMenuPopUp(e As SourceGrid2.PositionContextMenuEventArgs) Implements SourceGrid2.BehaviorModels.IBehaviorModel.OnContextMenuPopUp

            End Sub

            Public Sub OnDoubleClick(e As SourceGrid2.PositionEventArgs) Implements SourceGrid2.BehaviorModels.IBehaviorModel.OnDoubleClick

            End Sub

            Public Sub OnEditEnded(e As SourceGrid2.PositionCancelEventArgs) Implements SourceGrid2.BehaviorModels.IBehaviorModel.OnEditEnded

            End Sub

            Public Sub OnEditStarting(e As SourceGrid2.PositionCancelEventArgs) Implements SourceGrid2.BehaviorModels.IBehaviorModel.OnEditStarting

            End Sub

            Public Sub OnFocusEntered(e As SourceGrid2.PositionEventArgs) Implements SourceGrid2.BehaviorModels.IBehaviorModel.OnFocusEntered

            End Sub

            Public Sub OnFocusEntering(e As SourceGrid2.PositionCancelEventArgs) Implements SourceGrid2.BehaviorModels.IBehaviorModel.OnFocusEntering

            End Sub

            Public Sub OnFocusLeaving(e As SourceGrid2.PositionCancelEventArgs) Implements SourceGrid2.BehaviorModels.IBehaviorModel.OnFocusLeaving

            End Sub

            Public Sub OnFocusLeft(e As SourceGrid2.PositionEventArgs) Implements SourceGrid2.BehaviorModels.IBehaviorModel.OnFocusLeft

            End Sub

            Public Sub OnKeyDown(e As SourceGrid2.PositionKeyEventArgs) Implements SourceGrid2.BehaviorModels.IBehaviorModel.OnKeyDown

            End Sub

            Public Sub OnKeyPress(e As SourceGrid2.PositionKeyPressEventArgs) Implements SourceGrid2.BehaviorModels.IBehaviorModel.OnKeyPress

            End Sub

            Public Sub OnKeyUp(e As SourceGrid2.PositionKeyEventArgs) Implements SourceGrid2.BehaviorModels.IBehaviorModel.OnKeyUp

            End Sub

            Public Sub OnMouseDown(e As SourceGrid2.PositionMouseEventArgs) Implements SourceGrid2.BehaviorModels.IBehaviorModel.OnMouseDown

            End Sub

            Public Sub OnMouseEnter(e As SourceGrid2.PositionEventArgs) Implements SourceGrid2.BehaviorModels.IBehaviorModel.OnMouseEnter

            End Sub

            Public Sub OnMouseLeave(e As SourceGrid2.PositionEventArgs) Implements SourceGrid2.BehaviorModels.IBehaviorModel.OnMouseLeave

            End Sub

            Public Sub OnMouseMove(e As SourceGrid2.PositionMouseEventArgs) Implements SourceGrid2.BehaviorModels.IBehaviorModel.OnMouseMove

            End Sub

            Public Sub OnMouseUp(e As SourceGrid2.PositionMouseEventArgs) Implements SourceGrid2.BehaviorModels.IBehaviorModel.OnMouseUp

            End Sub

            Public Sub OnValueChanged(e As SourceGrid2.PositionEventArgs) Implements SourceGrid2.BehaviorModels.IBehaviorModel.OnValueChanged
                ' If allowed to dispatch checks
                If (Me.m_hr.ManageCheckedStates) Then
                    ' Engage check lock
                    Me.m_hr.BeginCheckChange()
                    ' Apply check state to all children
                    For Each linkChild As cLink In Me.m_children
                        linkChild.Checkstate = Me.Checkstate
                    Next
                    ' Release check lock
                    Me.m_hr.EndCheckChange()
                End If
            End Sub

#End Region ' Event handling

        End Class

#End Region ' Sourcegrid support

#Region " DataGridView support "

        Private Class cDataGridViewCheckboxLink
            Inherits cLink

            Private m_cb As DataGridViewCheckBoxCell = Nothing


            ''' -------------------------------------------------------------------
            ''' <summary>
            ''' Constructor
            ''' </summary>
            ''' <param name="hr">The <see cref="cCheckboxHierarchy"/> this link is 
            ''' created for.</param>
            ''' <param name="cb">The <see cref="EwECheckboxCell"/> to define this link for.</param>
            ''' <param name="parent">An optional parent link.</param>
            ''' -------------------------------------------------------------------
            Public Sub New(cb As DataGridViewCheckBoxCell, hr As cCheckboxHierarchy, parent As cLink)
                MyBase.New(hr, parent)
                Me.m_cb = cb
                AddHandler Me.m_cb.DataGridView.CellContentClick, AddressOf OnCellValueChanged
            End Sub

            Public Overrides Sub Dispose()
                If (Me.m_cb IsNot Nothing) Then
                    RemoveHandler Me.m_cb.DataGridView.CellContentClick, AddressOf OnCellValueChanged
                    Me.m_cb = Nothing
                End If
                MyBase.Dispose()
            End Sub

            Public Overrides Property Checkstate As CheckState
                Get
                    Return CType(Me.m_cb.Value, CheckState)
                End Get
                Set(value As CheckState)
                    Me.m_cb.Value = value
                End Set
            End Property

#Region " Event handling "

            Private Sub OnCellValueChanged(sender As Object, e As DataGridViewCellEventArgs)

                If (Not Me.m_hr.ManageCheckedStates) Then Return
                If (e.ColumnIndex <> Me.m_cb.ColumnIndex) Then Return
                If (e.RowIndex <> Me.m_cb.RowIndex) Then Return

                ' https://stackoverflow.com/questions/11843488/how-to-detect-datagridview-checkbox-event-change#15011844

                If (Me.m_cb.IsInEditMode) And (Me.m_cb.DataGridView.IsCurrentCellDirty) Then
                    Me.m_cb.DataGridView.EndEdit()
                End If

                ' If allowed to dispatch checks
                ' Engage check lock
                Me.m_hr.BeginCheckChange()
                ' Apply check state to all children
                For Each linkChild As cLink In Me.m_children
                    linkChild.Checkstate = Me.Checkstate
                Next
                ' Release check lock
                Me.m_hr.EndCheckChange()
            End Sub

#End Region ' Event handling

        End Class

#End Region ' DataGridView support

#Region " Treeview support "

        Private Class cTreeNodeLink
            Inherits cLink

            Private m_node As TreeNode

            Public Sub New(nd As TreeNode, hr As cCheckboxHierarchy, parent As cLink)
                MyBase.New(hr, parent)
                Me.m_node = nd
                AddHandler Me.m_node.TreeView.AfterCheck, AddressOf OnCheckChanged
            End Sub

            Public Overrides Sub Dispose()
                If (Me.m_node IsNot Nothing) Then
                    RemoveHandler Me.m_node.TreeView.AfterCheck, AddressOf OnCheckChanged
                    Me.m_node = Nothing
                End If
                MyBase.Dispose()
            End Sub

            Public Overrides Property Checkstate As CheckState
                Get
                    Return If(Me.m_node.Checked, CheckState.Checked, CheckState.Unchecked)
                End Get
                Set(value As CheckState)
                    Me.m_node.Checked = (value = CheckState.Checked)
                End Set
            End Property


#Region " Event handling "

            ''' -------------------------------------------------------------------
            ''' <summary>
            ''' Respond to checkbox check state changes.
            ''' </summary>
            ''' -------------------------------------------------------------------
            Private Sub OnCheckChanged(sender As Object, args As TreeViewEventArgs)

                If Not Object.ReferenceEquals(args.Node, Me.m_node) Then Return
                'If Me.m_hr.IsBusy Then Return

                ' If allowed to dispatch checks
                If (Me.m_hr.ManageCheckedStates) Then
                    ' Engage check lock
                    Me.m_hr.BeginCheckChange()
                    ' Apply check state to all children
                    For Each linkChild As cLink In Me.m_children
                        linkChild.Checkstate = Me.Checkstate
                    Next
                    ' Release check lock
                    Me.m_hr.EndCheckChange()
                End If

            End Sub

#End Region ' Event handling

        End Class

#End Region ' Treeview support

#End Region ' Checkbox links

#Region " Public methods "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <param name="cbRoot">The root checkbox to use.</param>
        ''' -----------------------------------------------------------------------
        Public Sub New(cbRoot As Object)
            Me.Add(cbRoot, Nothing)
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Clean-up.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Sub Dispose() Implements IDisposable.Dispose
            For Each link As cLink In Me.m_dtLinks.Values
                link.Dispose()
            Next
            Me.m_dtLinks.Clear()
            Me.m_linkRoots.Clear()
            GC.SuppressFinalize(Me)
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Add a checkbox to the hierarchy.
        ''' </summary>
        ''' <param name="checkbox">The checkbox to add.</param>
        ''' <param name="checkboxParent">The parent checkbox, if any. If left empty,
        ''' the root of the hierarchy is assumed.</param>
        ''' -----------------------------------------------------------------------
        Public Function Add(checkbox As Object, checkboxParent As Object) As Boolean

            Dim linkParent As cLink = Nothing

            If (checkbox Is Nothing) Then Return True

            ' Checkbox already defined?
            If Me.m_dtLinks.ContainsKey(checkbox) Then Return False

            If (checkboxParent IsNot Nothing) Then
                If (Not Me.m_dtLinks.ContainsKey(checkboxParent)) Then Return False
                linkParent = Me.m_dtLinks(checkboxParent)
            End If

            If (linkParent IsNot Nothing) Then
                ' Create new link
                Dim linkNew As cLink = Me.GetLink(checkbox, linkParent)
                ' Add new link as child to parent
                linkParent.AddChild(linkNew)
                ' Remember new link
                Me.m_dtLinks(checkbox) = linkNew
            Else
                ' Create new root link
                Dim linkRoot As cLink = Me.GetLink(checkbox, Nothing)
                Me.m_linkRoots.Add(linkRoot)
                ' Remember new link
                Me.m_dtLinks(checkbox) = linkRoot
            End If

            Return True

        End Function

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get/set whether this class is allowed to cascade <see cref="CheckBox.CheckState"/>
        ''' changes through the hierarchy of check boxes. This flag is turned off by default
        ''' to prevent unneccessary check state management while checkboxes are being configured.
        ''' When the hierarchy is established and all checkboxes have been set to their 
        ''' initial check states this management should be enabled.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Property ManageCheckedStates As Boolean
            Get
                Return Me.m_bManageChecks
            End Get
            Set(value As Boolean)
                Me.m_bManageChecks = value
                Me.Update()
            End Set
        End Property

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Update the state of checkboxes in the tree. Note that the update will
        ''' not be performed as long as there are checked changes in progress via
        ''' <see cref="BeginCheckChange"/> and <see cref="EndCheckChange"/>.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Sub Update()

            If (Me.m_iLockCount <> 0) Then Return
            If (Me.m_linkRoots.Count = 0) Then Return

            ' Remember dispatch state
            Dim bDispatchChecksOld As Boolean = Me.m_bManageChecks
            ' Turn off dispatching
            Me.m_bManageChecks = False
            ' Update all links
            For Each link As cLink In Me.m_linkRoots
                link.Update()
            Next
            ' Restore dispatching state
            Me.m_bManageChecks = bDispatchChecksOld

        End Sub

#End Region ' Public methods

#Region " Internals "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Notify the hierarchy that a checkbox checked state is going to get set.
        ''' This will increase a check lock counter; the hierarchy will not update 
        ''' check states as long as the check lock counter is non-zero.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Protected Sub BeginCheckChange()
            Me.m_iLockCount += 1
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Notify the hierarchy that a checkbox checked state has been set.
        ''' This will decrease a check lock counter; the hierarchy will not update 
        ''' check states as long as the check lock counter is non-zero.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Protected Sub EndCheckChange()
            Me.m_iLockCount -= 1
            Me.Update()
        End Sub

        Protected Function IsBusy() As Boolean
            Return (Me.m_iLockCount > 0)
        End Function

        Private Function GetLink(cb As Object, parent As cLink) As cLink
            Select Case cb.GetType
                Case GetType(CheckBox)
                    Return New cCheckboxLink(DirectCast(cb, CheckBox), Me, parent)
                Case GetType(cEwECheckboxCell)
                    Return New cCheckboxCellLink(DirectCast(cb, cEwECheckboxCell), Me, parent)
                Case GetType(DataGridViewCheckBoxCell)
                    Return New cDataGridViewCheckboxLink(DirectCast(cb, DataGridViewCheckBoxCell), Me, parent)
                Case GetType(TreeNode)
                    Return New cTreeNodeLink(DirectCast(cb, TreeNode), Me, parent)
                Case Else
                    Debug.Assert(False, "Type not supported")
            End Select
            Return Nothing

        End Function

#End Region ' Internals

    End Class

End Namespace ' Controls

#Enable Warning CA1063 ' Implement IDisposable Correctly
