' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Public Class cCreatedObjects

    Private m_Parent As String
    Private m_Child As List(Of String)

    Public Sub New(Parent As String)
        Me.m_Parent = Parent
        Me.m_Child = New List(Of String)
    End Sub

    Public Sub Add(Child As String)
        For Each x In Me.m_Child
            If x = Child Then Exit Sub
        Next
        Me.m_Child.Add(Child)
    End Sub

    Public Sub Remove(Child As String)
        Me.m_Child.Remove(Child)
    End Sub

    Public ReadOnly Property ParentName() As String
        Get
            Return Me.m_Parent
        End Get
    End Property

    Public ReadOnly Property ChildNames() As List(Of String)
        Get
            Return Me.m_Child
        End Get
    End Property

    Public ReadOnly Property CountChild() As Integer
        Get
            Return Me.m_Child.Count
        End Get
    End Property

End Class