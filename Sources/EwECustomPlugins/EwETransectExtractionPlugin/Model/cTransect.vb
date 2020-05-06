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
Imports System.Drawing
Imports EwECore

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Container for a single transect.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cTransect

#Region " Private vars "

    Private m_core As cCore = Nothing
    Private m_cells As New List(Of Point)
    Private m_ptStart As PointF
    Private m_ptEnd As PointF
    Private m_summaries As New Dictionary(Of String, cTransectSummary)

#End Region ' Private vars

    Public Enum eSummaryType As Byte
        Biomass
        [Catch]
    End Enum

#Region " Constructor "

    Public Sub New(strName As String)
        Me.Name = strName
    End Sub

#End Region ' Constructor

#Region " Transect properties "


    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the name of the transect.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property Name As String = ""

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the start location (expressed in map units lon, lat) of the transect,
    ''' in real-world coodinates.
    ''' </summary>
    ''' -----------------------------------------------------------------------
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

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the end location (expressed in map units lon, lat) of the transect,
    ''' in real-world coodinates.
    ''' </summary>
    ''' -----------------------------------------------------------------------
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

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns the number of cells in the transect, or -1 if this number is not determined yet.
    ''' </summary>
    ''' <returns>The number of cells in the transect, or -1 if this number is 
    ''' not determined yet.</returns>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property NumCells As Integer
        Get
            'jb remove the -1 because 
            'anywhere NumCells is used it is already -1 from the value
            Return Me.m_cells.Count ' - 1
        End Get
    End Property

#End Region ' Transect properties

#Region " Cell access "

#End Region ' Editing

    ''' <summary>
    ''' Returns all modelled cells that the transect passes through. The cells
    ''' are given as col, row.
    ''' </summary>
    ''' <param name="bm">The basemap to determine the cells from.</param>
    ''' <returns>The cells.</returns>
    ''' <remarks>
    ''' Once determined, the cells are cached until the transect is modified.
    ''' See https://en.wikipedia.org/wiki/Bresenham's_line_algorithm 
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Public Function Cells(bm As cEcospaceBasemap) As Point()

        If (Me.m_cells.Count = 0) Then

            Dim x0 As Integer = CInt(Math.Floor(bm.LonToCol(Me.m_ptStart.X)))
            Dim y0 As Integer = CInt(Math.Floor(bm.LatToRow(Me.m_ptStart.Y)))
            Dim x1 As Integer = CInt(Math.Floor(bm.LonToCol(Me.m_ptEnd.X)))
            Dim y1 As Integer = CInt(Math.Floor(bm.LatToRow(Me.m_ptEnd.Y)))

            If Math.Abs(y1 - y0) < Math.Abs(x1 - x0) Then

                If x0 > x1 Then
                    GetCellsX(x1, y1, x0, y0)
                Else
                    GetCellsX(x0, y0, x1, y1)
                End If
            Else
                If y0 > y1 Then
                    GetCellsY(x1, y1, x0, y0)
                Else
                    GetCellsY(x0, y0, x1, y1)
                End If
            End If
        End If

        Return Me.m_cells.ToArray()

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Your friendly helpful neighbourhood identifier.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Overrides Function ToString() As String
        Return Me.Name
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Remove all cached cells, to be determined again when needed next.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Sub Invalidate()
        Me.m_cells.Clear()
        Me.m_summaries.Clear()
    End Sub

    Public Sub SortLocations()
        ' Make sure start is to the west and north of end 
        If (Me.Start.X > Me.End.X) Or ((Me.Start.X = Me.End.X) And (Me.Start.Y > Me.End.Y)) Then
            Dim ptTemp As PointF = Me.m_ptStart
            Me.m_ptStart = Me.m_ptEnd
            Me.m_ptEnd = ptTemp
        End If
    End Sub

#Region " Ecospace run integration "

    Public Sub InitRun(core As cCore)
        Me.m_core = core
        Me.m_summaries.Clear()
    End Sub

    Public Sub Record(results As cEcospaceTimestep)
        If (Me.m_core Is Nothing) Then Return
        Dim t As Integer = results.iTimeStep
        Dim bm As cEcospaceBasemap = Me.m_core.EcospaceBasemap
        For iGroup As Integer = 1 To Me.m_core.nGroups
            Me.m_summaries(Key(t, iGroup, eSummaryType.Biomass)) = New cTransectSummary(Me, bm, "Biomass " & t, results.BiomassMap, iGroup)
            Me.m_summaries(Key(t, iGroup, eSummaryType.Catch)) = New cTransectSummary(Me, bm, "Catch " & t, results.CatchMap, iGroup)
        Next
    End Sub

#End Region ' Ecospace run integration

#Region " Summary access "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get a transect summary for a given time step, group, and variable.
    ''' </summary>
    ''' <param name="iTimestep"></param>
    ''' <param name="iGroup"></param>
    ''' <param name="variable"></param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Public Function Summary(iTimestep As Integer, iGroup As Integer, variable As eSummaryType) As cTransectSummary
        Dim strKey As String = Key(iTimestep, iGroup, variable)
        If Me.m_summaries.ContainsKey(strKey) Then Return Me.m_summaries(strKey)
        Return Nothing
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get a transect summary for a specific input layer.
    ''' </summary>
    ''' <param name="l"></param>
    ''' <param name="iIndex"></param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Public Function Summary(bm As cEcospaceBasemap, l As cEcospaceLayer, iIndex As Integer) As cTransectSummary
        Return New cTransectSummary(Me, bm, l, iIndex)
    End Function

    Public ReadOnly Property HasSummaries As Boolean
        Get
            Return (Me.m_summaries.Count > 0)
        End Get
    End Property

#End Region ' Summary access

#Region " Internals "

    Private Function Key(t As Integer, iGroup As Integer, value As Byte) As String
        Return t & "_" & iGroup & "_" & value
    End Function

    ''' <summary>
    ''' Get pixels on a line by increasing along the X axis.
    ''' </summary>
    ''' <param name="x0"></param>
    ''' <param name="y0"></param>
    ''' <param name="x1"></param>
    ''' <param name="y1"></param>
    ''' <remarks>
    ''' See https://en.wikipedia.org/wiki/Bresenham's_line_algorithm 
    ''' </remarks>
    Private Sub GetCellsX(x0 As Integer, y0 As Integer, x1 As Integer, y1 As Integer)

        Dim dx As Integer = x1 - x0
        Dim dy As Integer = Math.Abs(y1 - y0)
        Dim yi As Integer = If(y1 < y0, -1, 1)
        Dim D As Integer = 2 * dy - dx
        Dim y As Integer = y0

        For x As Integer = x0 To x1
            Me.m_cells.Add(New Point(x, y))
            If D > 0 Then
                y = y + yi
                D = D - 2 * dx
            End If
            D = D + 2 * dy
        Next

    End Sub

    ''' <summary>
    ''' Get pixels on a line by increasing along the Y axis.
    ''' </summary>
    ''' <param name="x0"></param>
    ''' <param name="y0"></param>
    ''' <param name="x1"></param>
    ''' <param name="y1"></param>
    ''' <remarks>
    ''' See https://en.wikipedia.org/wiki/Bresenham's_line_algorithm 
    ''' </remarks>
    Private Sub GetCellsY(x0 As Integer, y0 As Integer, x1 As Integer, y1 As Integer)

        Dim dx As Integer = Math.Abs(x1 - x0)
        Dim dy As Integer = y1 - y0
        Dim xi As Integer = If(x1 < x0, -1, 1)
        Dim D As Integer = 2 * dx - dy
        Dim x As Integer = x0

        For y As Integer = y0 To y1
            Me.m_cells.Add(New Point(x, y))
            If D > 0 Then
                x = x + xi
                D = D - 2 * dy
            End If
            D = D + 2 * dx
        Next

    End Sub

#End Region ' Internals

End Class
