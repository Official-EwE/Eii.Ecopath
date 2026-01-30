' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Public Class cSelectionData

    Private m_UnSelected As List(Of String)
    Private m_Selected As List(Of cCreatedObjects)
    Private m_SelectionType As String
    Private m_ParentChildsSelected As cCreatedObjects

    Public Sub New(SelectionType As String, Input() As String)
        Me.m_SelectionType = SelectionType
        Me.m_UnSelected = New List(Of String)
        Me.m_Selected = New List(Of cCreatedObjects)
        For Each x In Input 'Load unselected with an inputted array of strings
            Me.m_UnSelected.Add(x)
        Next
    End Sub

    Public Sub Add(i As String)
        Me.m_Selected.Add(New cCreatedObjects(i))
        Me.m_UnSelected.Remove(i)
    End Sub

    Public Sub Remove(i As String)
        Dim p As Integer = 0
        Me.m_UnSelected.Add(i)
        While p < Me.m_Selected.Count
            If Me.m_Selected(p).ParentName = i Then
                Me.m_Selected.RemoveAt(p)
            End If
            p += 1
        End While

    End Sub

    Public Sub RemoveAll()
        For i = 0 To Me.m_Selected.Count - 1
            Me.Remove(Me.m_Selected(0).ParentName)
        Next
    End Sub

    Public ReadOnly Property GetSelectedItem(i As Integer) As cCreatedObjects
        Get
            Return Me.m_Selected(i)
        End Get
    End Property

    Public ReadOnly Property UnSelectedNames() As List(Of String)
        Get
            Return Me.m_UnSelected
        End Get
    End Property

    Public ReadOnly Property SelectedNames() As List(Of String)
        Get
            Dim ListOfNames As New List(Of String)
            For Each i In Me.m_Selected
                ListOfNames.Add(i.ParentName)
            Next
            Return ListOfNames
        End Get
    End Property

    Public ReadOnly Property CountSelected() As Integer
        Get
            Return Me.m_Selected.Count
        End Get
    End Property

    Public ReadOnly Property GetSelected() As List(Of cCreatedObjects)
        Get
            Return Me.m_Selected
        End Get
    End Property

    Public ReadOnly Property GetParentChild(i As String) As cCreatedObjects
        Get
            For Each x In Me.m_Selected
                If x.ParentName = i Then
                    Return x
                End If
            Next
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property GetAttached(SelectedItem As String) As List(Of String)
        Get
            For Each i In Me.m_Selected
                If i.ParentName = SelectedItem Then
                    Return i.ChildNames
                End If
            Next
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property CountSelectedChild() As Integer
        Get
            Dim Count As Integer = 0
            For Each i In Me.m_Selected
                Count += i.CountChild
            Next
            Return Count
        End Get
    End Property

    Public ReadOnly Property GetFocus() As cCreatedObjects
        Get
            Return Me.m_ParentChildsSelected
        End Get
    End Property

    Public WriteOnly Property SetFocus() As String
        Set(value As String)
            If value = Nothing Then
                Me.m_ParentChildsSelected = Nothing
                Exit Property
            End If
            For Each i In Me.m_Selected
                If i.ParentName = value Then
                    Me.m_ParentChildsSelected = i
                End If
            Next
        End Set
    End Property


End Class