' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System

Public Class cEnumUtils

    ''' <summary>
    ''' Fail-save templated enum parsingummy thing.
    ''' </summary>
    ''' <typeparam name="t"></typeparam>
    ''' <param name="input"></param>
    ''' <param name="valDefault"></param>
    ''' <returns></returns>
    Public Shared Function EnumParse(Of t)(input As String, valDefault As t) As t
        Dim type As Type = GetType(t)
        For Each value As String In [Enum].GetNames(type)
            If (String.Compare(value, input, True) = 0) Then
                Return CType([Enum].Parse(type, input), t)
            End If
        Next
        Return valDefault
    End Function

End Class
