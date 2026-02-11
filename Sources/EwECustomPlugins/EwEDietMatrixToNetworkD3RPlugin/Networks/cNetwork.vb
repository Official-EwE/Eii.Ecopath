' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.Text
Imports EwECore

Public MustInherit Class cNetwork

    Public Sub New(core As cCore)
        Me.Core = core
    End Sub

    Public ReadOnly Property Core As cCore = Nothing

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

    Protected Function HeaderLine() As String
        Dim sb As New StringBuilder()
        sb.AppendLine("# NetworkD3 " & Me.Name & " generated from Ecopath with Ecosim - EwEEcopathExportDietToNetworkD3 plug-in")
        sb.AppendLine("# EwE model: " & Me.Core.EwEModel.Name)
        sb.Append("# EwE file: " & Me.Core.DataSource.ToString())
        If (My.Settings.UseSymbolicNames) Then sb.AppendLine("# !EwE names have been replaced with symbolic names")
        sb.AppendLine("")
        sb.AppendLine("# !-- markdown is needed to export NeworkD3 models to a webpage")
        sb.AppendLine("# !-- if R does not let you install the correct version of markdown, run the following line:")
        sb.AppendLine("# install.packages(""rmarkdown"", repos = ""https://cran.revolutionanalytics.com"")")
        Return sb.ToString()
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
