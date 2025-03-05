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

''' <summary>
''' Helper class for sorting a number of <see cref="cCostPoint">cost-at-location</see>
''' data points, sorted by cost, to speed up the sailing cost calculations.
''' </summary>
Friend Class cSailCostCalculatorPriorityQueue

#Region " Helper classes "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Helper class for remembering a single cost-at-location data point.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cCostPoint
        Public ReadOnly Property Cost As Double
        Public ReadOnly Property Row As Integer
        Public ReadOnly Property Col As Integer
        Public Sub New(cost As Double, row As Integer, col As Integer)
            Me.Cost = cost
            Me.Row = row
            Me.Col = col
        End Sub
        Public Overrides Function ToString() As String ' For easy of debugging
            Return String.Format("r: {0}, c: {1} = cost {2}", Me.Row, Me.Col, Me.Cost)
        End Function
    End Class

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Helper class for comparing <see cref="cCostPoint">cost-at-location</see> data points.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cCostPointComparer
        Implements IComparer(Of cCostPoint)

        Public Function Compare(x As cCostPoint, y As cCostPoint) As Integer _
        Implements IComparer(Of cCostPoint).Compare

            ' Note that the SortedSet does not allow for duplicate items. Whether an
            ' item is a duplicate is decided here. Although sorting is only relevant
            ' on Cost, the sorting needs to be extended to  include the full location
            ' just to avoid different data points with the same costs to be
            ' accidentally considered euqal.
            Dim i As Integer = x.Cost.CompareTo(y.Cost)
            If (i = 0) Then
                i = x.Row.CompareTo(y.Row)
                If (i = 0) Then
                    i = x.Col.CompareTo(y.Col)
                End If
            End If
            Return i
        End Function

    End Class

#End Region ' Helper classes

#Region " Internal vars "

    Private m_queue As New SortedSet(Of cCostPoint)(New cCostPointComparer())

#End Region ' Internal vars

    Public Sub New(inrow As Integer, incol As Integer)
        'NOP
    End Sub

    Public Function Enqueue(priority As Double, row As Integer, col As Integer) As Boolean
        Return Me.m_queue.Add(New cCostPoint(priority, row, col))
    End Function

    Public Function Dequeue(ByRef row As Integer, ByRef col As Integer) As Boolean
        Dim minItem As cCostPoint = m_queue.Min
        Me.m_queue.Remove(minItem)
        row = minItem.Row
        col = minItem.Col
        Return True
    End Function

    Public Function Count() As Integer
        Return Me.m_queue.Count
    End Function

    Public Sub Clear()
        Me.m_queue.Clear()
    End Sub

End Class


