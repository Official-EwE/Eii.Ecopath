' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Public Class cRowCol

    Public Sub New(ByVal theRow As Integer, ByVal theCol As Integer)
        Me.Row = theRow
        Me.Col = theCol
    End Sub

    Public ReadOnly Property Row As Integer
    Public ReadOnly Property Col As Integer

    Public Overrides Function ToString() As String
        Return "Row: " & Me.Row & ", col: " & Me.Col
    End Function

End Class