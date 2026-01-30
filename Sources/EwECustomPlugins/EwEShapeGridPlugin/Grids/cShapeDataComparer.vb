' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore

Public Class cShapeDataComparer
    Implements IComparer(Of cShapeData)

    Public Function Compare(x As cShapeData, y As cShapeData) As Integer Implements IComparer(Of cShapeData).Compare
        If (x Is Nothing) Then Return -1
        If (y Is Nothing) Then Return 1
        Return x.Index.CompareTo(y.Index)
    End Function

End Class
