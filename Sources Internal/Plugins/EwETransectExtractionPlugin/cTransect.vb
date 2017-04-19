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

#Region " Imports "

Option Strict On
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

        If (Me.m_cells.Count = 0) Then

            Dim x0 As Integer = bm.LonToCol(Me.m_ptStart.X)
            Dim y0 As Integer = bm.LatToRow(Me.m_ptStart.Y)
            Dim x1 As Integer = bm.LonToCol(Me.m_ptEnd.X)
            Dim y1 As Integer = bm.LatToRow(Me.m_ptEnd.Y)

            '' Bresenham
            'Dim dx As Integer = x1 - x0
            'Dim dy As Integer = y1 - y0
            'Dim d As Single = 2 * dy - dx
            'Dim y = y0

            'For x As Integer = x0 To x1
            '    m_cells.Add(New Point(x, y))
            '    If (d > 0) Then
            '        y += 1
            '        d -= 2 * dx
            '    End If
            '    d += 2 * dy
            'Next

            ' https://gamedev.stackexchange.com/questions/81267/how-do-i-generalise-bresenhams-line-algorithm-to-floating-point-endpoints
            'float difX = end.x - start.x;
            'float difY = end.y - start.y;
            'float dist = abs(difX) + abs(difY);

            'float dx = difX / dist;
            'float dy = difY / dist;

            'For (int i = 0, int x, int y; i <= ceil(dist); i++) {
            '    x = floor(Start.X + dx * i);
            '    y = floor(Start.Y + dy * i);
            '    draw(x, y);
            '}
            'Return True;

            Dim difX As Double = x1 - x0
            Dim difY As Double = y1 - x0
            Dim dist As Double = Math.Abs(difX) + Math.Abs(difY)

            Dim dx As Double = 0
            Dim dy As Double = 0

            If (dist > 0) Then dx = difX / dist : dy = difY / dist

            For i As Integer = 0 To CInt(Math.Ceiling(dist))
                Dim x As Integer = CInt(Math.Round(x0 + dx * i))
                Dim y As Integer = CInt(Math.Round(y0 + dy * i))
                Me.m_cells.Add(New Point(x, y))
            Next
        End If

        Return Me.m_cells.ToArray()

    End Function

    Public Sub Invalidate()
        Me.m_cells.Clear()
    End Sub

    Public Overrides Function ToString() As String
        Return Me.Name
    End Function

End Class
