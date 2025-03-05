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

#End Region ' Imports

Public Class cSailCostCalculator

    ' Directions (N, S, E, W)
    Private m_dcol As Integer() = {0, 0, -1, 1}
    Private m_drow As Integer() = {-1, 1, 0, 0}

    Private m_ds As cEcospaceDataStructures = Nothing

    Public Sub New(ds As cEcospaceDataStructures)
        Me.m_ds = ds
        ' Cell widths already calculated in cEcospaceDatastructures.CalculateCellWidths
    End Sub

    Public Function CalculateCostOfSailing() As Boolean

        For iFleet As Integer = 1 To m_ds.nFleets

            Dim distance(,) As Double = New Double(Me.m_ds.InRow, Me.m_ds.InCol) {}
            Dim pq As New cSailCostCalculatorPriorityQueue(Me.m_ds.InRow, Me.m_ds.InCol) ' Min-heap priority queue

            ' Initialize distances and enqueue all ports with zero distance
            For row As Integer = 1 To Me.m_ds.InRow
                For col As Integer = 1 To Me.m_ds.InCol
                    If Me.m_ds.Port(iFleet)(row, col) Then
                        distance(row, col) = 0.0
                        pq.Enqueue(0.0, row, col)
                    Else
                        distance(row, col) = Double.MaxValue
                    End If
                Next
            Next

            ' Priority Queue Dijkstra-style traversal
            While pq.Count() > 0
                Dim row, col As Integer
                If pq.Dequeue(row, col) Then

                    Dim currentDist As Double = distance(row, col)

                    ' Expand in four directions
                    For i As Integer = 0 To 3
                        Dim ncol As Integer = col + m_dcol(i)
                        Dim nrow As Integer = row + m_drow(i)

                        ' Wrap-around for east-west boundaries
                        If (Me.m_ds.IsGlobalMap) Then
                            ncol = 1 + ((ncol - 1 + Me.m_ds.InCol) Mod Me.m_ds.InCol)
                        End If

                        ' Check bounds 
                        If nrow >= 1 And nrow <= Me.m_ds.InRow And ncol >= 1 And ncol <= Me.m_ds.InCol Then
                            ' Is modelled cell?
                            If Me.m_ds.Depth(nrow, ncol) > 0 Then

                                ' Get precomputed cell width
                                Dim kmEastWest As Double = Me.m_ds.CellLength * Me.m_ds.RelativeCellWidth(nrow)
                                Dim kmNorthSouth As Double = Me.m_ds.CellLength
                                Dim stepDistance As Double = If(m_dcol(i) <> 0, kmEastWest, kmNorthSouth)

                                Dim newDist As Double = currentDist + stepDistance

                                ' Relaxation: Update if new distance is shorter
                                If newDist < distance(nrow, ncol) Then
                                    If pq.Enqueue(newDist, nrow, ncol) Then
                                        distance(nrow, ncol) = newDist
                                    End If
                                End If
                            End If
                        End If
                    Next i
                End If
            End While

            ' Apply to core arrays
            For row As Integer = 1 To Me.m_ds.InRow
                For col As Integer = 1 To Me.m_ds.InCol
                    Dim val As Double = distance(row, col)
                    If Me.m_ds.Depth(row, col) > 0 Then
                        val = distance(row, col)
                        If val = Double.MaxValue Then val = 0
                    Else
                        val = 0
                    End If
                    Me.m_ds.Sail(iFleet)(row, col) = CSng(val)
                Next col
            Next row
        Next iFleet

        Return True

    End Function

End Class
