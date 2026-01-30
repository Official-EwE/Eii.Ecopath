' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System
Imports System.Collections.Generic
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic

<HideModuleNameAttribute()>
Public Module Extensions

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Extension method; extract a value from an object by property name.
    ''' </summary>
    ''' <param name="o"></param>
    ''' <param name="strField"></param>
    ''' -----------------------------------------------------------------------
    <Extension()>
    Public Function Value(o As Object, strField As String) As String
        Dim t As Type = o.GetType()
        For Each pi As PropertyInfo In t.GetProperties(BindingFlags.Public Or BindingFlags.Instance)
            If (String.Compare(pi.Name, strField, True) = 0) Then
                Return CStr(pi.GetValue(o, Nothing))
            End If
        Next
        Return ""
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Extension method; fill an array with a given value.
    ''' </summary>
    ''' <typeparam name="T">Value type.</typeparam>
    ''' <param name="arr">The array to fill.</param>
    ''' <param name="value">The value to fill the array with.</param>
    ''' -----------------------------------------------------------------------
    <Extension()>
    Public Sub Fill(Of T)(ByRef arr As T(), value As T)
        If (arr Is Nothing) Then Return
        For i As Integer = 0 To arr.Length - 1
            arr(i) = value
        Next
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Extension method; check if two arrays are the same.
    ''' </summary>
    ''' <typeparam name="T">Value type.</typeparam>
    ''' <param name="a">The first array to check.</param>
    ''' <param name="b">The second array to check.</param>
    ''' -----------------------------------------------------------------------
    <Extension()>
    Public Function IsIdenticalTo(Of T)(a As T(), b() As T) As Boolean

        If (a Is Nothing) And (b Is Nothing) Then Return True
        If (a Is Nothing) Or (b Is Nothing) Then Return False
        If (a.Length <> b.Length) Then Return False

        For i As Integer = 0 To a.GetUpperBound(0)
            If Not Object.Equals(a(i), b(i)) Then Return False
        Next
        Return True
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Extension method; fill an array with a given value.
    ''' </summary>
    ''' <typeparam name="T">Value type.</typeparam>
    ''' <param name="arr">The array to fill.</param>
    ''' <param name="value">The value to fill the array with.</param>
    ''' -----------------------------------------------------------------------
    <Extension()>
    Public Sub Fill(Of T)(ByRef arr As T(,), value As T)
        If (arr Is Nothing) Then Return
        For i As Integer = 0 To arr.GetUpperBound(0)
            For j As Integer = 0 To arr.GetUpperBound(1)
                arr(i, j) = value
            Next j
        Next i
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Extension method; fill an array with a given value.
    ''' </summary>
    ''' <typeparam name="T">Value type.</typeparam>
    ''' <param name="arr">The array to fill.</param>
    ''' <param name="value">The value to fill the array with.</param>
    ''' -----------------------------------------------------------------------
    <Extension()>
    Public Sub Fill(Of T)(ByRef arr As T(,,), value As T)
        If (arr Is Nothing) Then Return
        For i As Integer = 0 To arr.GetUpperBound(0)
            For j As Integer = 0 To arr.GetUpperBound(1)
                For k As Integer = 0 To arr.GetUpperBound(2)
                    arr(i, j, k) = value
                Next k
            Next j
        Next i
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Extension method; extract a sub-array.
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="values"></param>
    ''' <param name="iStart"></param>
    ''' <param name="iEnd"></param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    <Extension()>
    Public Function SubArray(Of T)(values() As T, iStart As Integer, iEnd As Integer) As T()
        Dim n As Integer = iEnd - iStart + 1
        Dim result(n - 1) As T
        Array.Copy(values, iStart, result, 0, n)
        Return result
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Extension method; shuffles the specified list.
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="list">The list to shuffle.</param>
    ''' -----------------------------------------------------------------------
    <Extension()>
    Public Sub Shuffle(Of T)(list As IList(Of T))
        Dim r As Random = New Random()
        For i As Integer = 0 To list.Count - 1
            Dim index As Integer = r.Next(i, list.Count)
            If i <> index Then
                Dim temp As T = list(i)
                list(i) = list(index)
                list(index) = temp
            End If
        Next
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Extension method; checks if two arrays are equal.
    ''' </summary>
    ''' <typeparam name="T">Value type.</typeparam>
    ''' <param name="arr">The array to compare.</param>
    ''' <param name="arr2">The other array to compare to.</param>
    ''' -----------------------------------------------------------------------
    <Extension()>
    Public Function EqualsArray(Of T)(ByRef arr As T(), arr2 As T()) As Boolean
        If (arr Is Nothing) And (arr2 Is Nothing) Then Return True
        If (arr Is Nothing) Or (arr2 Is Nothing) Then Return False
        If (arr.Length <> arr2.Length) Then Return False
        For i As Integer = 0 To arr.Length - 1
            If (Array.IndexOf(arr2, arr(i)) = -1) Then Return False
        Next
        Return True
    End Function

    <Extension()>
    Public Function Approximates(s1 As Single, s2 As Single, Optional sThreshold As Single = 0.00001) As Boolean
        Return EwEUtils.Utilities.cNumberUtils.Approximates(s1, s2, sThreshold)
    End Function

    <Extension()>
    Public Function Approximates(s1 As Double, s2 As Double, Optional sThreshold As Single = 0.00001) As Boolean
        Return EwEUtils.Utilities.cNumberUtils.Approximates(s1, s2, sThreshold)
    End Function

End Module

