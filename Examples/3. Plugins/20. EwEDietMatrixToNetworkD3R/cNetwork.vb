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
'    UBC Institute for the Oceans and Fisheries, Vancouver BC, Canada, and
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'
#Region " Imports "

Option Strict On
Imports System.Text
Imports EwECore

#End Region ' Imports

Public MustInherit Class cNetwork

    Public Sub New(core As cCore)
        Me.Core = core
    End Sub

    Public ReadOnly Property Core As cCore = Nothing

    Public Property UseSymbolicNames As Boolean = True

    MustOverride Function Name() As String
    MustOverride Function GenerateScript() As String

#Region " Internals "

    Protected Function ToRString(strIn As String) As String

        Dim sb As New StringBuilder()

        For i As Integer = 0 To strIn.Length - 1
            Dim c As Char = strIn(i)
            If Char.IsLetterOrDigit(c) Or Char.IsWhiteSpace(c) Then
                sb.Append(c)
            End If
        Next

        Return sb.ToString()

    End Function

    Protected Function MakeArray(Of T)(strPrefix As String, items As IEnumerable(Of T)) As String

        Dim sb As New StringBuilder()
        Dim iLineLength As Integer = 0
        Dim iLeadIn As Integer = 0

        sb.Append(strPrefix)
        sb.Append(" <- c(")
        iLeadIn = sb.Length

        iLineLength = sb.Length

        For i As Integer = 0 To items.Count - 1

            Dim strBit As String = ""
            Dim item As Object = items(i)
            If TypeOf (item) Is String Then
                strBit = """" & item.ToString() & """"
            Else
                strBit = item.ToString()
            End If

            If (i < items.Count - 1) Then
                strBit = strBit & ", "
            End If

            Dim iTest As Integer = strBit.Length
            If (iLineLength + iTest >= 999) Then
                sb.AppendLine()
                strBit = strBit.PadLeft(iLeadIn + iTest, " "c)
                iLineLength = iLeadIn
            End If

            sb.Append(strBit)
            iLineLength += iTest
        Next
        sb.Append(")")
        Return sb.ToString()

    End Function

#End Region ' Internals

End Class
