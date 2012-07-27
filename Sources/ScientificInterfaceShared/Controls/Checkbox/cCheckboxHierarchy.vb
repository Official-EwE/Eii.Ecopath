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

#End Region ' Imports

Namespace Controls

    ''' <summary>
    ''' Management class for a hierarchy of check boxes.
    ''' </summary>
    Public Class cCheckboxHierarchy
        Implements IDisposable

#Region " Private vars "

        Private m_dtLinks As New Dictionary(Of CheckBox, cLink)
        Private m_linkRoot As cLink = Nothing
        Private m_bManageChecks As Boolean = False
        Private m_iLockCount As Integer = 0

#End Region ' Private vars

#Region " Private helper classes "

        ''' <summary>
        ''' Link in a checkbox hierarchy chain. Each link has a checkbox, an
        ''' optional parent link, and zero or more child links.
        ''' </summary>
        Private Class cLink
            Implements IDisposable

#Region " Private vars "

            ''' <summary>Parent hierarchy.</summary>
            Private m_hr As cCheckboxHierarchy = Nothing
            ''' <summary>Checkbox the link is created for.</summary>
            Private m_cb As CheckBox
            ''' <summary>Parent link in the hierarchy.</summary>
            Private m_parent As cLink = Nothing
            ''' <summary>List of child links in the hierarchy.</summary>
            Private m_children As New List(Of cLink)

#End Region ' Private vars

#Region " Public access "

            ''' -------------------------------------------------------------------
            ''' <summary>
            ''' Constructor
            ''' </summary>
            ''' <param name="hr">The <see cref="cCheckboxHierarchy"/> this link is 
            ''' created for.</param>
            ''' <param name="cb">The checkbox to define this link for.</param>
            ''' <param name="parent">An optional parent link.</param>
            ''' -------------------------------------------------------------------
            Public Sub New(hr As cCheckboxHierarchy, cb As CheckBox, Optional parent As cLink = Nothing)
                Me.m_hr = hr
                Me.m_cb = cb
                AddHandler Me.m_cb.CheckedChanged, AddressOf OnCheckChanged
                Me.m_parent = parent
            End Sub

            ''' -------------------------------------------------------------------
            ''' <summary>
            ''' Cleanup.
            ''' </summary>
            ''' -------------------------------------------------------------------
            Public Sub Dispose() Implements IDisposable.Dispose
                GC.SuppressFinalize(Me)
                If (Me.m_cb IsNot Nothing) Then
                    RemoveHandler Me.m_cb.CheckedChanged, AddressOf OnCheckChanged
                End If
                Me.m_hr = Nothing
                Me.m_cb = Nothing
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
            Public Sub Update()

                Dim iNumChecked As Integer = 0
                Dim iNumInterm As Integer = 0
                Dim state As CheckState = CheckState.Unchecked

                ' Only affect links with children
                If (Me.m_children.Count > 0) Then

                    ' For every child
                    For Each child As cLink In Me.m_children
                        ' Update its checked state
                        child.Update()
                        ' Count checked state of childred
                        If child.m_cb.CheckState = CheckState.Checked Then iNumChecked += 1
                        If child.m_cb.CheckState = CheckState.Indeterminate Then iNumInterm += 1
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
                    Me.m_cb.CheckState = state

                End If

            End Sub

#End Region ' Public access

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
                        linkChild.m_cb.Checked = Me.m_cb.Checked
                    Next
                    ' Release check lock
                    Me.m_hr.EndCheckChange()
                End If

            End Sub

#End Region ' Event handling

        End Class

#End Region ' Private helper classes

#Region " Public methods "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <param name="cbRoot">The root checkbox to use.</param>
        ''' -----------------------------------------------------------------------
        Public Sub New(cbRoot As CheckBox)
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
            Me.m_linkRoot = Nothing
            GC.SuppressFinalize(Me)
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Add a checkbox to the hierarchy.
        ''' </summary>
        ''' <param name="cb">The checkbox to add.</param>
        ''' <param name="cbParent">The parent checkbox, if any.</param>
        ''' -----------------------------------------------------------------------
        Public Function Add(cb As CheckBox, cbParent As CheckBox) As Boolean

            ' Checkbox already defined?
            If Me.m_dtLinks.ContainsKey(cb) Then Return False

            If (Me.m_linkRoot IsNot Nothing) Then
                If (cbParent Is Nothing) Then Return False
                If (Not Me.m_dtLinks.ContainsKey(cbParent)) Then Return False

                ' Locate parent link
                Dim linkParent As cLink = Me.m_dtLinks(cbParent)
                ' Create new link
                Dim linkNew As cLink = New cLink(Me, cb, linkParent)
                ' Add new link as child to parent
                linkParent.AddChild(linkNew)
                ' Remember new link
                Me.m_dtLinks(cb) = linkNew
            Else
                ' Create new root link
                Me.m_linkRoot = New cLink(Me, cb)
                ' Remember new link
                Me.m_dtLinks(cb) = Me.m_linkRoot
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
            If (Me.m_linkRoot Is Nothing) Then Return

            ' Remember dispatch state
            Dim bDispatchChecksOld As Boolean = Me.m_bManageChecks
            ' Turn off dispatching
            Me.m_bManageChecks = False
            ' Update all links
            Me.m_linkRoot.Update()
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

#End Region ' Internals

    End Class

End Namespace ' Controls
