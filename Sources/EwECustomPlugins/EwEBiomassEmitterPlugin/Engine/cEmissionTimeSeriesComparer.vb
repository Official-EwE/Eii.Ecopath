' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

''' <summary>
''' Helper class, compares model trends
''' </summary>
Public Class cEmissionTimeSeriesComparer
    Implements IComparer(Of cEmissionTimeSeries)

    Public Function Compare(x As cEmissionTimeSeries, y As cEmissionTimeSeries) As Integer Implements IComparer(Of cEmissionTimeSeries).Compare
        If (x.Group < y.Group) Then Return -1
        If (x.Group > y.Group) Then Return 1
        If (x.Target < y.Target) Then Return -1
        If (x.Target > y.Target) Then Return 1
        Return 0
    End Function

End Class
