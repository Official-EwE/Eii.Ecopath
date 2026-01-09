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




Imports System.Threading



''' ---------------------------------------------------------------------------
''' <summary>
''' An indicator calculator
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cTreadCalculator

    Private m_inds As New List(Of cIndicators)
    Private m_id As Integer = 0

    Public Sub New(Optional id As Integer = 0)
        Me.m_id = id
    End Sub

    Public Sub Add(ind As cIndicators)
        Me.m_inds.Add(ind)
    End Sub

    Public Sub Compute()
        For Each ind As cIndicators In Me.m_inds
            ind.Compute()
        Next
    End Sub

End Class
