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
Imports EwEUtils

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Utility class to convert the contents of a Datatable to strong typed collections.
''' </summary>
''' <remarks>
''' Original code by Jeroen Steenbeek, EII, for Safenet.
''' </remarks>
''' ---------------------------------------------------------------------------
Public Class cDataTableConverter

    Public Shared Function ToList(Of T)(dt As DataTable, data As List(Of T)) As Boolean
        data.Clear()
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

    Public Shared Function ToDictionary(Of T)(dt As DataTable, data As Dictionary(Of String, T), strField As String) As Boolean
        data.Clear()
        If (dt Is Nothing) Then Return False
        Dim e As New cDataTableEnumerable(Of T)(dt)
        For Each obj As T In e
            data(obj.Value(strField).ToLower()) = obj
        Next
        Return True
    End Function

End Class
