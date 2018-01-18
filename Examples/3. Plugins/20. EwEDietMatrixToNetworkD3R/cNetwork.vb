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

    Private Const cSPECIAL_CHARS As String = "_-., "

    ''' <summary>
    ''' Returns a string fit for inclusion in R code by eradicating conflicting characters.
    ''' </summary>
    ''' <param name="strIn"></param>
    ''' <returns></returns>
    Protected Function ToRString(strIn As String) As String

        Dim sb As New StringBuilder()

        For i As Integer = 0 To strIn.Length - 1
            Dim c As Char = strIn(i)
            ' Spaceyfy (nothing to do with Kevin! EwE was only a victim!)
            If (Char.IsWhiteSpace(c)) Then c = " "c
            ' Accept only allowed characters
            If (Char.IsLetterOrDigit(c) Or cSPECIAL_CHARS.IndexOf(c) >= 0) Then
                sb.Append(c)
            End If
        Next

        Return sb.ToString()

    End Function

    Protected Function ModelLine() As String
        Dim sb As New StringBuilder()
        sb.AppendLine("# NetworkD3 " & Me.Name & " generated from Ecopath with Ecosim - EwEEcopathExportDietToNetworkD3 plug-in")
        sb.AppendLine("# EwE model: " & Me.Core.EwEModel.Name)
        sb.Append("# EwE file: " & Me.Core.DataSource.ToString())
        Return sb.ToString()
    End Function

    Protected Function SymbolicLine() As String
        Return "# !Group names have been replaced with symbolic names"
    End Function

    ''' <summary>
    ''' Returns a R code line that assigns the items in a collection to an array.
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="strPrefix"></param>
    ''' <param name="items"></param>
    ''' <returns></returns>
    Protected Function ArrayLine(Of T)(strPrefix As String, items As IEnumerable(Of T)) As String

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
