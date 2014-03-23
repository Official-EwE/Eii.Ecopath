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
' The Cefas MSE plug-in was developed by the Centre for Environment, Fisheries and 
' Aquaculture Science (Cefas). 
'
' EwE copyright: 1991- UBC Fisheries Centre, Vancouver BC, Canada.
' Cefas MSE plug-in copyright: 2013- Cefas, Lowestoft, UK.
' ===============================================================================
'
#Region " Imports "

Option Strict On
Option Explicit On

#End Region ' Imports 

''' <summary>
''' Class to group a list of Harvest Control Rules into an object
''' </summary>
Public Class Strategy
    Implements IList(Of HCR_Group)

    Private m_list As New List(Of HCR_Group)

    Public Sub New()
        ' Hm
    End Sub

    Public Sub New(StrategyName As String)
        Me.New()
        Me.Name = StrategyName
    End Sub

    Public Sub New(ByVal StrategyName As String, ByVal theFilename As String)
        Me.New(StrategyName)
        Me.FileName = theFilename
    End Sub

    Public Property Name As String
    Public Property FileName As String

    Public Overrides Function ToString() As String
        Return MyBase.ToString() & ":" & Me.Name
    End Function

    Public Sub Add(item As HCR_Group) Implements System.Collections.Generic.ICollection(Of HCR_Group).Add
        If Not Me.Contains(item) Then
            Me.m_list.Add(item)
        End If
    End Sub

    Public Sub Clear() Implements System.Collections.Generic.ICollection(Of HCR_Group).Clear
        Me.m_list.Clear()
    End Sub

    Public Function Contains(item As HCR_Group) As Boolean Implements System.Collections.Generic.ICollection(Of HCR_Group).Contains
        For Each Rule As HCR_Group In Me
            If Object.ReferenceEquals(item.GroupB, Rule.GroupB) And Object.ReferenceEquals(item.GroupF, Rule.GroupF) Then
                Return True
            End If
        Next
        Return False
    End Function

    Public Sub CopyTo(array() As HCR_Group, arrayIndex As Integer) Implements System.Collections.Generic.ICollection(Of HCR_Group).CopyTo
        ' NOP
    End Sub

    Public ReadOnly Property Count As Integer Implements System.Collections.Generic.ICollection(Of HCR_Group).Count
        Get
            Return Me.m_list.Count
        End Get
    End Property

    Public ReadOnly Property IsReadOnly As Boolean Implements System.Collections.Generic.ICollection(Of HCR_Group).IsReadOnly
        Get
            Return False
        End Get
    End Property

    Public Function Remove(item As HCR_Group) As Boolean Implements System.Collections.Generic.ICollection(Of HCR_Group).Remove
        Return Me.m_list.Remove(item)
    End Function

    Public Function GetEnumerator() As System.Collections.Generic.IEnumerator(Of HCR_Group) Implements System.Collections.Generic.IEnumerable(Of HCR_Group).GetEnumerator
        Return Me.m_list.GetEnumerator()
    End Function

    Public Function IndexOf(item As HCR_Group) As Integer Implements System.Collections.Generic.IList(Of HCR_Group).IndexOf
        Return Me.m_list.IndexOf(item)
    End Function

    Public Sub Insert(index As Integer, item As HCR_Group) Implements System.Collections.Generic.IList(Of HCR_Group).Insert
        Me.m_list.Insert(index, item)
    End Sub

    Default Public Property Item(index As Integer) As HCR_Group Implements System.Collections.Generic.IList(Of HCR_Group).Item
        Get
            Return Me.m_list.Item(index)
        End Get
        Set(value As HCR_Group)
            Me.m_list(index) = value
        End Set
    End Property

    Public Sub RemoveAt(index As Integer) Implements System.Collections.Generic.IList(Of HCR_Group).RemoveAt
        Me.m_list.RemoveAt(index)
    End Sub

    Private Function Bogus() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
        ' NOP
        Return Nothing
    End Function

End Class
