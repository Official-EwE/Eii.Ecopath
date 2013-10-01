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

Imports System.IO

''' <summary>
''' Class to group a list of Harvest Control Rules into an object
''' </summary>
''' <remarks></remarks>
Public Class Strategy
    Inherits List(Of HCR_Group)

    Public Name As String
    Public FileName As String

    Public Sub New()

    End Sub

    Public Sub New(StrategyName As String)
        Me.New()
        Me.Name = StrategyName
    End Sub

    Public Sub New(ByVal StrategyName As String, ByVal theFilename As String)
        Me.New(StrategyName)
        Me.FileName = theFilename
    End Sub


    Public Shadows Sub Add(Item As HCR_Group)
        If Not Me.Contains(Item) Then
            MyBase.Add(Item)
        End If
    End Sub


    Public Shadows Function Contains(Item As HCR_Group) As Boolean

        For Each Rule As HCR_Group In Me
            If Object.ReferenceEquals(Item.GroupB, Rule.GroupB) And Object.ReferenceEquals(Item.GroupF, Rule.GroupF) Then
                Return True
            End If
        Next
        Return False

    End Function


End Class
