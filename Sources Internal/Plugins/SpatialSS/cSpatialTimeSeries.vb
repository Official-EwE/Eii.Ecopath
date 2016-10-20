Option Strict On
Imports System.Globalization
Imports System.IO
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

Imports EwECore

#End Region ' Imports

Public Class cSpatialTimeSeries

    Public Class cDataPoint

        Public Sub New(core As cCore, lon As Single, lat As Single, value As Single)
            Me.New(core.EcospaceBasemap.LonToCol(lon), core.EcospaceBasemap.LatToRow(lat), value)
        End Sub

        Public Sub New(col As Integer, row As Integer, value As Single)
            Me.Row = col
            Me.Col = row
            Me.Value = value
        End Sub

        Public Property Row As Integer
        Public Property Col As Integer
        Public Property Value As Single

    End Class

    Private m_core As cCore = Nothing
    Private m_data As New Dictionary(Of DateTime, List(Of cDataPoint))

    Public Sub New(core As cCore)
        Me.m_core = core
    End Sub

    Public Function Read(strFile As String) As Boolean

        Dim r As StreamReader = Nothing
        Dim bIsRowCol As Boolean = False
        Dim strLine As String = ""
        Dim bSucces As Boolean = True

        Me.m_data.Clear()

        Try
            r = New StreamReader(strFile)
        Catch ex As Exception
            Return False
        End Try

        strLine = r.ReadLine()
        bIsRowCol = (strLine.IndexOf("row", StringComparison.CurrentCultureIgnoreCase) >= 0) And (strLine.IndexOf("col", StringComparison.CurrentCultureIgnoreCase) >= 0)

        ' Expected order: col, row, time, value OR lon, lat, time, value
        ' Expected time format: yyyy-mm
        While Not r.EndOfStream

            strLine = r.ReadLine()
            Dim bits() As String = strLine.Split(","c)
            Dim t As DateTime = Nothing
            Dim iCol As Integer = 0
            Dim iRow As Integer = 0
            Dim sVal As Single = 0
            Dim pt As cDataPoint = Nothing

            bSucces = bSucces And DateTime.TryParseExact(bits(2), "yyyy-MM", CultureInfo.InvariantCulture, DateTimeStyles.None, t) And Single.TryParse(bits(3), sVal)

            If (bIsRowCol) Then
                bSucces = bSucces And Integer.TryParse(bits(0), iCol) And Integer.TryParse(bits(1), iRow)
            Else
                Dim sLat As Single = 0
                Dim sLon As Single = 0
                bSucces = bSucces And Single.TryParse(bits(0), sLon) And Single.TryParse(bits(1), sLat)
                iCol = Me.m_core.EcospaceBasemap.LonToCol(sLon)
                iRow = Me.m_core.EcospaceBasemap.LatToRow(sLat)
            End If

            If bSucces Then
                If Not Me.m_data.ContainsKey(t) Then
                    Me.m_data(t) = New List(Of cDataPoint)
                End If
                Me.m_data(t).Add(New cDataPoint(iCol, iRow, sVal))
            Else

            End If

        End While
        Return bSucces

    End Function

    ''' <summary>
    ''' Returns the data points for a given Ecospace time step
    ''' </summary>
    ''' <param name="iTimeStep"></param>
    ''' <returns></returns>
    Public Function DataPoints(iTimeStep As Integer) As cDataPoint()

        Dim t As DateTime = Me.m_core.EcospaceTimestepToAbsoluteTime(iTimeStep)
        If Me.m_data.ContainsKey(t) Then
            Return Me.m_data(t).ToArray()
        End If
        Return New cDataPoint() {}

    End Function

    ''' <summary>
    ''' Returns if there are reference data points for a given Ecospace time step
    ''' </summary>
    ''' <param name="iTimeStep"></param>
    ''' <returns></returns>
    Public Function HasDataPoints(iTimeStep As Integer) As Boolean

        Dim t As DateTime = Me.m_core.EcospaceTimestepToAbsoluteTime(iTimeStep)
        Return Me.m_data.ContainsKey(t)

    End Function

End Class
