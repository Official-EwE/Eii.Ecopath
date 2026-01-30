' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

''' -----------------------------------------------------------------------------
''' <summary>
''' Utility class to calculate sailing costs for each fleet based on the location
''' of their ports. Costs are calculated as port distances in km, working around 
''' land masses.
''' </summary>
''' -----------------------------------------------------------------------------
Public Class cSailCostCalculator

#Region " Private vars "

    ' -- Horizontal and vertical movement vectors --
    'Private m_dcol As Integer() = {0, 0, -1, 1}
    'Private m_drow As Integer() = {-1, 1, 0, 0}

    ' -- Movement vectors, also diagonally --
    Private m_dcol As Integer() = {-1, 0, 1, -1, 1, -1, 0, 1}
    Private m_drow As Integer() = {-1, -1, -1, 0, 0, 1, 1, 1}

    Private m_ds As cEcospaceDataStructures = Nothing

#End Region ' Private vars

    Public Sub New(ds As cEcospaceDataStructures)
        Me.m_ds = ds
    End Sub

    Public Function CalculateCostOfSailing() As Boolean

        Dim distance(,) As Double = New Double(Me.m_ds.InRow, Me.m_ds.InCol) {}
        Dim pq As New cSailCostCalculatorPriorityQueue(Me.m_ds.InRow, Me.m_ds.InCol)
        Dim n As Integer = Me.m_dcol.Count

        For iFleet As Integer = 1 To m_ds.nFleets

            ' Should not be neceessary, but hey
            pq.Clear()

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

                    ' Explore all movement vectors
                    For i As Integer = 0 To n - 1

                        Dim ncol As Integer = col + Me.m_dcol(i)
                        Dim nrow As Integer = row + Me.m_drow(i)

                        ' Wrap-around east-west boundaries for global models
                        If (Me.m_ds.IsGlobalMap) Then
                            ncol = 1 + ((ncol - 1 + Me.m_ds.InCol) Mod Me.m_ds.InCol)
                        End If

                        ' Check bounds 
                        If ((nrow >= 1) And (nrow <= Me.m_ds.InRow) And (ncol >= 1) And (ncol <= Me.m_ds.InCol)) Then

                            ' Is modelled cell?
                            If Me.m_ds.Depth(nrow, ncol) > 0 Then

                                ' Obtain EW and NS movement vectors
                                Dim kmEastWest As Double = Me.m_ds.CellLength * Me.m_ds.RelativeCellWidth(nrow)
                                Dim kmNorthSouth As Double = Me.m_ds.CellLength
                                Dim stepDistance As Double = 0
                                ' Determine relevant directions
                                Dim bIsDiag As Boolean = (Me.m_drow(i) <> 0) And (Me.m_dcol(i) <> 0)
                                Dim bIsEW As Boolean = (Me.m_dcol(i) <> 0)

                                ' Calculate actual step distance
                                If (bIsDiag) Then
                                    stepDistance = Math.Sqrt(kmEastWest * kmEastWest + kmNorthSouth * kmNorthSouth)
                                Else
                                    stepDistance = If(bIsEW, kmEastWest, kmNorthSouth)
                                End If
                                Dim newDist As Double = currentDist + stepDistance

                                ' Update and queue a new point if new distance is shorter
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
