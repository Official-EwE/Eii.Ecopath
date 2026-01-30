' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.Reflection
Imports EwEUtils
Imports EwEUtils.Utilities

Public Class cDataTableConverter

    Public Shared Function ToList(Of T)(dt As DataTable, data As List(Of T), bAppend As Boolean) As Boolean
        If Not bAppend Then data.Clear()
        If (dt IsNot Nothing) Then
            Dim e As New cDataTableEnumerable(Of T)(dt)
            For Each obj As T In e
                data.Add(obj)
            Next
        Else
            Return False
        End If
        Return True
    End Function

    Public Shared Function ToDictionary(Of T)(dt As DataTable, data As Dictionary(Of String, T), strField As String, bAppend As Boolean) As Boolean

        If (dt Is Nothing) Then Return False
        If (data Is Nothing) Then Return False

        If Not bAppend Then data.Clear()

        Dim e As New cDataTableEnumerable(Of T)(dt)
        For Each obj As T In e
            Dim key As String = obj.Value(strField).ToLower()
            data(key) = obj
        Next
        Return True
    End Function

    Public Shared Function ToDatatable(Of T)(data As ICollection(Of T), Optional excludedproperties As String() = Nothing) As DataTable

        Dim dt As New DataTable()
        Dim type As Type = GetType(T)

        For Each prop As PropertyInfo In type.GetProperties()
            If (cPropertyUtils.IsWritableElemental(prop) And (excludedproperties Is Nothing OrElse Array.IndexOf(excludedproperties, prop.Name) = -1)) Then
                dt.Columns.Add(prop.Name, prop.PropertyType)
            End If
        Next
        Try
            For Each obj As T In data
                Dim row As DataRow = dt.NewRow()
                For Each prop As PropertyInfo In type.GetProperties()
                    If (cPropertyUtils.IsWritableElemental(prop) And (excludedproperties Is Nothing OrElse Array.IndexOf(excludedproperties, prop.Name) = -1)) Then
                        row(prop.Name) = prop.GetValue(obj)
                    End If
                Next
                dt.Rows.Add(row)
            Next
        Catch ex As Exception

        End Try
        Return dt
    End Function

End Class
