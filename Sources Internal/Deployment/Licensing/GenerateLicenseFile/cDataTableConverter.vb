' ===============================================================================
' This file is part of the Safenet toolkit.
'
' To use Safenet tools please contact Marta Coll or Jeroen Steenbeek at
' ecopathinternational@gmail.com
'
' Copyright 2017- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

Option Strict On
Imports System.ComponentModel

Public Class cDataTableConverter

    Public Shared Function ToList(Of T)(dt As DataTable, data As List(Of T), bAppend As Boolean) As Boolean
        If Not bAppend Then data.Clear()
        If (dt IsNot Nothing) Then
            Dim e As New DataTableEnumerable(Of T)(dt)
            For Each obj As T In e
                data.Add(obj)
            Next
        Else
            Return False
        End If
        Return True
    End Function

    Public Shared Function ToBindingList(Of T)(dt As DataTable, data As BindingList(Of T), bAppend As Boolean) As Boolean
        If Not bAppend Then data.Clear()
        If (dt IsNot Nothing) Then
            Dim e As New DataTableEnumerable(Of T)(dt)
            For Each obj As T In e
                data.Add(obj)
            Next
        Else
            Return False
        End If
        Return True
    End Function

    Public Shared Function ToDictionary(Of T)(dt As DataTable, data As Dictionary(Of String, T), strField As String, bAppend As Boolean) As Boolean
        If Not bAppend Then data.Clear()
        If (dt Is Nothing) Then Return False
        Dim e As New DataTableEnumerable(Of T)(dt)
        For Each obj As T In e
            Dim key As String = obj.Value(strField).ToLower()
            data(key) = obj
        Next
        Return True
    End Function

End Class
