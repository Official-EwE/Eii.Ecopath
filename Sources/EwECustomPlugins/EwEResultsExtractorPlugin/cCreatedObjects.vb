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