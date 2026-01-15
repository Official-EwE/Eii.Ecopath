' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 3 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see https://www.gnu.org/licenses/gpl-3.0.html>. 
'
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'



Namespace Ecosim

    ''' =======================================================================
    ''' <summary>
    ''' Helper class, contains data for a single all fits data plot.
    ''' </summary>
    ''' =======================================================================
    Public Class cShowAllFitsPlotData

        Private m_ts As cTimeSeries
        Private m_lSimData As New List(Of Single)

        Public Sub New(ts As cTimeSeries, asSimData As Single())

            ' Sanity check(s)
            Debug.Assert(ts IsNot Nothing)
            Debug.Assert(asSimData IsNot Nothing)

            Me.m_ts = ts
            Me.m_lSimData.AddRange(asSimData)

            Me.CalculateScale()

        End Sub

        Public Function TimeSeries() As cTimeSeries
            Return Me.m_ts
        End Function

        Public Function SimData() As Single()
            Return Me.m_lSimData.ToArray
        End Function

        Public Property YMax() As Single = 1

        Public Property YMaxDefault() As Single

        Public Property TSDataScale() As Single = 1.0

        Public Property Visible() As Boolean = True

        ''' <summary>
        ''' States whether the user has selected this plot or viewing
        ''' </summary>
        Public Property Selected() As Boolean = True

        Private Sub CalculateScale()

            Dim data As Single() = Me.m_lSimData.ToArray
            Dim sMax As Single = 0

            ' Find data max across sim results
            For j As Integer = 1 To data.Length - 1
                sMax = Math.Max(data(j), sMax)
            Next

            Me.TSDataScale = 1.0

            ' Find data max across time series
            If (Me.m_ts IsNot Nothing) Then
                If (Me.m_ts.IsRelative) Then
                    If (Me.m_ts.DataQ <> 0) Then Me.TSDataScale = CSng(1.0! / Me.m_ts.eDataQ)
                End If

                data = Me.m_ts.ShapeData
                For j As Integer = 1 To data.Length - 1
                    sMax = Math.Max(data(j) * Me.TSDataScale, sMax)
                Next
            End If

            ' Store
            Me.YMax = (sMax / 0.8!)
            Me.YMaxDefault = Me.YMax

        End Sub

    End Class

End Namespace ' Ecosim
