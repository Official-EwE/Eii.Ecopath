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
#Region " Imports "

Option Strict On
Imports System.Reflection
Imports EwEUtils
Imports EwEUtils.Utilities

#End Region ' Imports

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
