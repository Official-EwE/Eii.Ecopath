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
