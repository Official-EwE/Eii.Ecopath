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
    Private m_results(,,,) As Single

#End Region ' Private vars

#Region " Constructor "

    Public Sub New(strName As String)
        Me.Name = strName
    End Sub

#End Region ' Constructor

#Region " Public access "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the name of the transect.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property Name As String = ""

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the start location (expressed in map units lon, lat) of the transect.
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
    ''' Get/set the end location (expressed in map units lon, lat) of the transect.
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
    ''' Returns all modelled cells that the transect passes through. The cells
    ''' are given as col, row.
    ''' </summary>
    ''' <param name="bm">The basemap to determine the cells from.</param>
    ''' <returns>The cells.</returns>
    ''' <remarks>
    ''' Once determined, the cells are cached until the transect is modified.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Public Function Cells(bm As cEcospaceBasemap) As Point()

        If (Me.m_cells.Count = 0) Then

            Dim x0 As Integer = bm.LonToCol(Me.m_ptStart.X)
            Dim y0 As Integer = bm.LatToRow(Me.m_ptStart.Y)
            Dim x1 As Integer = bm.LonToCol(Me.m_ptEnd.X)
            Dim y1 As Integer = bm.LatToRow(Me.m_ptEnd.Y)

            Dim difX As Double = x1 - x0
            Dim difY As Double = y1 - y0
            Dim dist As Double = Math.Abs(difX) + Math.Abs(difY)

            Dim dx As Double = 0
            Dim dy As Double = 0

            If (dist > 0) Then
                dx = difX / dist : dy = difY / dist
            End If

            For i As Integer = 0 To CInt(Math.Ceiling(dist))
                Dim iCol As Integer = x0 + CInt(Math.Round(dx * i))
                Dim iRow As Integer = y0 + CInt(Math.Round(dy * i))
                ' Note reversal of row and col here. It's messy, but it's deliberate
                If bm.IsModelledCell(iRow, iCol) Then
                    Me.m_cells.Add(New Point(iCol, iRow))
                End If
            Next
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

    Public Sub InitRun(core As cCore)
        Me.m_core = core
        ReDim Me.m_results(Me.m_core.nEcospaceTimeSteps, Me.m_core.nGroups, Me.m_cells.Count, 2)
    End Sub

    Public Sub Record(results As cEcospaceTimestep)
        If (Me.m_core IsNot Nothing) Then Return
        Dim t As Integer = results.iTimeStep
        For iGroup As Integer = 1 To Me.m_core.nGroups
            For iCell As Integer = 0 To Me.m_cells.Count - 1
                Dim pt As Point = Me.m_cells(iCell)
                Me.m_results(t, iGroup, iCell, 0) = results.BiomassMap(pt.Y, pt.X, iGroup)
                Me.m_results(t, iGroup, iCell, 1) = results.CatchMap(pt.Y, pt.X, iGroup)
            Next
        Next
    End Sub

    Public Sub EndRun()

    End Sub

    ''' <summary>
    ''' Transect average
    ''' </summary>
    ''' <param name="iTimestep"></param>
    ''' <param name="iGroup"></param>
    ''' <param name="iValue"></param>
    ''' <returns></returns>
    Public Function Result(iTimestep As Integer, iGroup As Integer, iValue As Integer) As Single
        Dim t As Single = 0
        Dim n As Integer = Math.Max(1, Me.m_cells.Count - 1)
        For iCell As Integer = 0 To Me.m_cells.Count - 1
            t += Me.m_results(iTimestep, iGroup, iCell, iValue)
        Next
        Return t / n
    End Function

    ''' <summary>
    ''' Transect value for given cell
    ''' </summary>
    ''' <param name="iTimestep"></param>
    ''' <param name="iGroup"></param>
    ''' <param name="iValue"></param>
    ''' <param name="iCell"></param>
    ''' <returns></returns>
    Public Function Result(iTimestep As Integer, iGroup As Integer, iValue As Integer, iCell As Integer) As Single
        Return Me.m_results(iTimestep, iGroup, iCell, iValue)
    End Function

#End Region ' Public access

#Region " Internals "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Remove all cached cells, to be determined again when needed next.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Protected Sub Invalidate()
        Me.m_cells.Clear()
        If (Me.m_results IsNot Nothing) Then
            Erase Me.m_results
            Me.m_results = Nothing
        End If
    End Sub

#End Region ' Internals

End Class
