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
'    UBC Fisheries Centre, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

Option Strict On
#Region " Imports "

Imports System.Drawing
Imports EwECore

#End Region ' Imports

Public Class cTransect

    Private m_cells As New List(Of Point)
    Private m_ptStart As PointF
    Private m_ptEnd As PointF

    Public Property Name As String = ""

    ''' <summary>
    ''' Start coordinate (lon. lat) of the transect.
    ''' </summary>
    Public Property Start As PointF
        Get
            Return Me.m_ptStart
        End Get
        Set(value As PointF)
            If (value <> Me.m_ptStart) Then
                Me.m_ptStart = value
                Me.Invalidate()
            End If
        End Set
    End Property

    ''' <summary>
    ''' End coordinate (lon. lat) of the transect.
    ''' </summary>
    Public Property [End] As PointF
        Get
            Return Me.m_ptEnd
        End Get
        Set(value As PointF)
            If (value <> Me.m_ptEnd) Then
                Me.m_ptEnd = value
                Me.Invalidate()
            End If
        End Set
    End Property

    Public Function Cells(bm As cEcospaceBasemap) As Point()

        ' ToDo: calculate the cell X, Y locations that the transect intersects with

        'If (Me.m_cells.Count = 0) Then
        '    Dim dx As Integer = Me.m_ptEnd.X - Me.m_ptStart.X
        '    Dim dy As Integer = Me.m_ptEnd.Y - Me.m_ptStart.Y

        '    If (dx = 0 And dy = 0) Then
        '        m_cells.Add(Me.m_ptStart)
        '    ElseIf (dx = 0) Then
        '        For y As Integer = 0 To dy Step Math.Sign(dy) : m_cells.Add(New Point(Me.m_ptStart.X, Me.m_ptStart.Y + y)) : Next
        '    Else
        '        Dim sx As Single = CSng(dx / Math.Max(Math.Abs(dx), Math.Abs(dy))) * Math.Sign(dx)
        '        Dim sy As Single = CSng(dy / Math.Max(Math.Abs(dx), Math.Abs(dy))) * Math.Sign(dy)

        '        For t As Integer = 0 To Math.Max(Math.Abs(dx), Math.Abs(dy))
        '            m_cells.Add(New Point(Me.m_ptStart.X + CInt(Math.Round(t * sx)), Me.m_ptStart.Y + CInt(Math.Round(t * sy))))
        '        Next
        '    End If
        'End If
        Return Me.m_cells.ToArray()
    End Function

    Public Sub Invalidate()
        Me.m_cells.Clear()
    End Sub

    Public Overrides Function ToString() As String
        Return Me.Name
    End Function

End Class
