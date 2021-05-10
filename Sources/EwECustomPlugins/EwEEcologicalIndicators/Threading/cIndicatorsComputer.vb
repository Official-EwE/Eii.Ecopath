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
Imports System.Threading

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Utility class to compute a number of indicators in a separate thread. To use 
''' this, make sure that <see cref="ThreadIncrementer"/> and <see cref="WaitHandle"/> 
''' are set. <see cref="Add"/> a number of <see cref="cIndicators"/> and call
''' <see cref="Compute"/> on a thread. The calling thread will block until the
''' <see cref="WaitHandle"/> decrements <see cref="ThreadIncrementer"/> to zero.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cTreadCalculator

    Friend Shared ThreadIncrementer As Integer

    Private m_inds As New List(Of cIndicators)
    Private m_id As Integer = 0

    Public Sub New(Optional id As Integer = 0)
        Me.m_id = id
    End Sub

    Public Sub Add(ind As cIndicators)
        Me.m_inds.Add(ind)
    End Sub

    Public Property WaitHandle As ManualResetEvent = Nothing

    Public Sub Compute()

        For Each ind As cIndicators In Me.m_inds
            ind.Compute()
        Next

        If (Me.WaitHandle IsNot Nothing) Then
            If Interlocked.Decrement(cTreadCalculator.ThreadIncrementer) = 0 Then
                Me.WaitHandle.Set()
            End If
        End If

    End Sub

End Class
