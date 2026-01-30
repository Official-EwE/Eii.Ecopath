' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Public Class cDataTableEnumerable(Of T)
    Implements IEnumerable(Of T)

    Private m_dt As DataTable

    Public Sub New(dt As DataTable)
        Me.m_dt = dt
    End Sub

    Public Function GetEnumerator() As IEnumerator(Of T) Implements IEnumerable(Of T).GetEnumerator
        Return New cDataTableEnumerator(Of T)(Me.m_dt)
    End Function

    Private Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
        Throw New NotImplementedException()
    End Function

End Class
