' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Public Class cMPAStateComparer
    Implements IComparer(Of cMPAState)

    Public Function Compare(x As cMPAState, y As cMPAState) As Integer Implements IComparer(Of cMPAState).Compare
        If (x.TimeStamp < y.TimeStamp) Then Return -1
        If (x.TimeStamp > y.TimeStamp) Then Return 1
        If (x.MPA < y.MPA) Then Return -1
        If (x.MPA > y.MPA) Then Return 1
        Return 0
    End Function

End Class
