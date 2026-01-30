' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

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
